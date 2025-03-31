using Confluent.Kafka;
using NETKafka.Common.Events;
using NETKafka.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;

namespace NETKafka.Kafka
{
    public class KafkaEventReplayer<TAggregate> where TAggregate : IEventSourcedAggregate
    {
        private readonly string _topic;
        private readonly Guid _aggregateId;
        private readonly TAggregate _aggregate;
        private readonly ConsumerConfig _config;
        private readonly List<Event> events = new();

        public KafkaEventReplayer(string topic, Guid aggregateId, TAggregate aggregate)
        {
            _topic = topic;
            _aggregateId = aggregateId;
            _aggregate = aggregate;

            _config = new ConsumerConfig
            {
                BootstrapServers = "localhost:29092",
                GroupId = $"replayer-group-{_topic}-{_aggregateId}", // Criar um groupId único para não interferir em outras leituras
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnablePartitionEof = true // Permite detectar quando chegou ao fim da partição
            };
        }

        public TAggregate ReplayEvents()
        {
            Console.WriteLine($"[Kafka] Replaying events for {_aggregateId}");

            using var consumer = new ConsumerBuilder<string, string>(_config).Build();
           
            var numPartitions = GetPartitions();
            var partition = GetPartitionForAccount(_aggregateId.ToString(), numPartitions);
           
            var topicPartition = new TopicPartition(_topic, new Partition(partition));
            consumer.Assign(topicPartition);
           
            //consumer.Seek(new TopicPartitionOffset(topicPartition, Offset.Beginning));

            CancellationTokenSource cts = new();
            Console.CancelKeyPress += (_, e) =>
            {
                e.Cancel = true;
                cts.Cancel();
            };

            try
            {
                while (!cts.Token.IsCancellationRequested)
                {
                    var result = consumer.Consume(cts.Token);

                    if (result == null || result.IsPartitionEOF)
                        break;

                    var @event = GetEvent(result.Message.Value);

                    if (@event is not null && @event.Id == _aggregateId)
                        events.Add(@event);
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("[Kafka] Replay cancelled.");
            }
            finally
            {
                consumer.Close();
            }

            // Aplicando eventos ordenados por timestamp
            foreach (var @event in events.OrderBy(e => e.TimeStamp))
                _aggregate.Apply(@event);

            Console.WriteLine($"[Kafka] State replayed successfully!");
            return _aggregate;
        }

        public TAggregate RevertAllEvents()
        {
            Console.WriteLine($"[Kafka] Reverting all events for {_aggregateId}");

            foreach (var @event in events.OrderByDescending(x => x.TimeStamp))
                _aggregate.Apply(@event);

            Console.WriteLine($"[Kafka] State reverted successfully!");
            return _aggregate;
        }

        private Event GetEvent(string json)
        {
            try
            {
                return JsonSerializer.Deserialize<Event>(json, new JsonSerializerOptions
                {
                    Converters = { new EventJsonConverter() }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Kafka] Error deserializing event: {ex.Message}");
                return null;
            }
        }

        private int GetPartitions()
        {
            using var adminClient = new AdminClientBuilder(_config).Build();
            var metadata = adminClient.GetMetadata(_topic, TimeSpan.FromSeconds(5));
            var topicMetadata = metadata.Topics.FirstOrDefault(t => t.Topic == _topic);

            return topicMetadata?.Partitions.Count ?? throw new Exception($"Tópico {_topic} não encontrado.");
        }

        private static int GetPartitionForAccount(string accountId, int numPartitions)
        {
            var partitionHash = Math.Abs(accountId.GetHashCode());
            return partitionHash % numPartitions;
        }
    }
}
