using Confluent.Kafka;
using NETKafka.Common.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NETKafka.Kafka
{
    public class KafkaEventProducer
    {
        private readonly IProducer<string, string> _producer;        

        public KafkaEventProducer()
        {
            var config = new ProducerConfig { BootstrapServers = "localhost:29092" };
            _producer = new ProducerBuilder<string, string>(config).Build();
        }

        public async Task PublisherEvent(string topic, Event @event)
        {
            var json = JsonSerializer.Serialize(@event, new JsonSerializerOptions
            {
                Converters = { new EventJsonConverter() },
                WriteIndented = true
            });

            var message = new Message<string, string>
            {
                Key = @event.Id.ToString(),
                Value = json
            };

            await _producer.ProduceAsync(topic, message);
        }
    }
}
