using Confluent.Kafka;

namespace FN.Application.Systems.Kafka
{
    public interface IKafkaProducer
    {
        Task Produce(string topic, Message<string, string> message);
    }
    public class KafkaProducer : IKafkaProducer
    {
        private readonly IProducer<string, string> _producer;
        public KafkaProducer(string groupName)
        {
            var config = new ConsumerConfig
            {
                GroupId = groupName,
                BootstrapServers = "localhost:9092",
                AutoOffsetReset = AutoOffsetReset.Earliest
            };
            _producer = new ProducerBuilder<string, string>(config).Build();
        }
        public async Task Produce(string topic, Message<string, string> message)
        {
            await _producer.ProduceAsync(topic, message);
        }
    }
}
