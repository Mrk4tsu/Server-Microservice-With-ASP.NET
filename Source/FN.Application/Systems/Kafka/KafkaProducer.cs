using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FN.Application.Systems.Kafka
{
    public interface IKafkaProducer
    {
        Task Produce(string topic, Message<string, string> message);
    }
    public class KafkaProducer : IKafkaProducer
    {
        private readonly IProducer<string, string> _producer;
        private readonly ILogger<KafkaProducer> _logger;
        private readonly IConfiguration _config;
        public KafkaProducer(string groupName,
            ILogger<KafkaProducer> logger,
            IConfiguration configuration)
        {
            _config = configuration;
            _logger = logger;
            var config = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"],
            };
            _producer = new ProducerBuilder<string, string>(config).Build();
            _logger.LogInformation("KafkaConsumer started. Connecting to: {Servers}", config.BootstrapServers);
        }
        public async Task Produce(string topic, Message<string, string> message)
        {
            await _producer.ProduceAsync(topic, message);
        }
    }
}
