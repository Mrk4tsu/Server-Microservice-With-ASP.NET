
using Confluent.Kafka;
using FN.Application.Catalog.Product.Pattern;
using FN.Application.Systems.Events;
using FN.DataAccess;
using FN.DataAccess.Enums;
using FN.Utilities;
using FN.ViewModel.Helper.API;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FN.CatalogService.Kafka
{
    public class KafkaConsumer : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly DateTime _now;
        public KafkaConsumer(IServiceScopeFactory serviceProvider)
        {
            _scopeFactory = serviceProvider;
            _now = new TimeHelper.Builder()
               .SetTimestamp(DateTime.UtcNow)
               .SetTimeZone("SE Asia Standard Time")
               .SetRemoveTick(true).Build();
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
                BootstrapServers = "localhost:9092",
                GroupId = SystemConstant.EVENT_PAYMENT_GROUP_KAFKA,
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
       
            using var consumer = new ConsumerBuilder<string, string>(config).Build();
            consumer.Subscribe(topic);

            while (!cancellationToken.IsCancellationRequested)
            {
                var consumeResult = consumer.Consume(cancellationToken);
                var epMessage = JsonConvert.DeserializeObject<EventMessage>(consumeResult.Message.Value);

                using var scope = _scopeFactory.CreateScope();
                var _db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var eventProduct = await _db.SaleEventProducts
                .Include(ep => ep.SaleEvent)
                .Include(ep => ep.ProductDetail)
                .FirstOrDefaultAsync(ep => ep.Id == epMessage.ProductId);
                if (eventProduct == null) return;

                if (_now < eventProduct.SaleEvent.StartDate || _now > eventProduct.SaleEvent.EndDate)
                    return ;

                if (eventProduct.CurrentPurchases >= eventProduct.MaxPurchases)
                    return;

                eventProduct.CurrentPurchases++;

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
            }
            consumer.Close();
        }
    }
}
