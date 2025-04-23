
using Confluent.Kafka;
using FN.DataAccess;
using FN.DataAccess.Entities;
using FN.DataAccess.Enums;
using FN.Utilities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FN.CatalogService.Kafka
{
    public class KafkaConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<KafkaConsumer> _logger;

        private DateTime _now;
        public KafkaConsumer(IServiceScopeFactory serviceProvider,
            ILogger<KafkaConsumer> logger,
            IConfiguration configuration)
        {
            _logger = logger;
            _scopeFactory = serviceProvider;
            _configuration = configuration;
            

        }
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                _ = Consumer(SystemConstant.EVENT_PAYMENT_TOPIC_KAFKA, stoppingToken);
            }, stoppingToken);
        }
        public async Task Consumer(string topic, CancellationToken cancellationToken)
        {

            var config = new ConsumerConfig
            {
                BootstrapServers = _configuration["Kafka:BootstrapServers"],
                GroupId = SystemConstant.EVENT_PAYMENT_GROUP_KAFKA,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
            };

            using var consumer = new ConsumerBuilder<string, string>(config)
                .SetErrorHandler((_, e) => _logger.LogError("Kafka error: {Reason}", e.Reason))
                .Build();
            consumer.Subscribe(topic);
           
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    _now = new TimeHelper.Builder()
                  .SetTimestamp(DateTime.UtcNow)
                  .SetTimeZone("SE Asia Standard Time")
                  .SetRemoveTick(true).Build();
                    var consumeResult = consumer.Consume(cancellationToken);
                    var epMessage = JsonConvert.DeserializeObject<EventMessage>(consumeResult.Message.Value);
                    _logger.LogInformation("Received message at {Time}: {Message}", DateTime.UtcNow, consumeResult.Message.Value);

                    using var scope = _scopeFactory.CreateScope();
                    var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    using var transaction = await _db.Database.BeginTransactionAsync();

                    try
                    {
                        var eventProduct = await _db.SaleEventProducts
                    .Include(ep => ep.SaleEvent)
                    .Include(ep => ep.ProductDetail)
                    .FirstOrDefaultAsync(ep => ep.Id == epMessage.ProductEventId);
                        if (eventProduct == null) continue;

                        if (_now < eventProduct.SaleEvent.StartDate || _now > eventProduct.SaleEvent.EndDate)
                            continue;

                        if (eventProduct.CurrentPurchases >= eventProduct.MaxPurchases)
                            continue;

                        eventProduct.CurrentPurchases++;
                        var owner = new ProductOwner
                        {
                            ProductId = epMessage.ProductId,
                            UserId = epMessage.UserId,
                        };
                        await _db.ProductOwners.AddAsync(owner).ConfigureAwait(false);
                        if (eventProduct.CurrentPurchases >= eventProduct.MaxPurchases)
                        {
                            eventProduct.IsActive = false;
                            var productPrice = await _db.ProductPrices
                            .FirstOrDefaultAsync(pp => pp.ProductDetailId == eventProduct.ProductDetailId
                                                    && pp.PriceType == PriceType.SALE_EVENT
                                                    && pp.SaleEventId == eventProduct.SaleEventId);
                            if (productPrice != null)
                                productPrice.EndDate = _now;
                        }
                        await _db.SaveChangesAsync();
                        await transaction.CommitAsync();
                        consumer.Commit(consumeResult);
                    }
                    catch (Exception ex)
                    {
                        _logger.BeginScope("Error processing message: {Message}", ex.Message);
                        await transaction.RollbackAsync();
                        consumer.Commit(consumeResult);
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError("Error processing message: {Message}", ex.Message);
                }
            }
            consumer.Close();
        }
    }
}
