using Confluent.Kafka;
using NETKafka.Common.Events;
using System.Text.Json;

namespace NETKafka.Kafka
{
    public class KafkaEventConsumer
    {
        private readonly string _topic;       

        public KafkaEventConsumer(string topic)
        {
            _topic = topic;            
        }

        public void ConsumerEvent()
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = "localhost:29092",
                GroupId = $"group-{_topic}",
                AutoOffsetReset = AutoOffsetReset.Latest //Consumo de eventos mais recentes
            };

            using var consumer = new ConsumerBuilder<string, string>(config).Build();
            consumer.Subscribe(_topic);

            while (true)
            {
                var result = consumer.Consume();              

                if (result is not null)
                {
                    Console.WriteLine($"[Kafka] Evento received: {result.GetType().Name} - {JsonSerializer.Serialize(result.Message.Value)}");                  
                }
            }

        }
    }
}
