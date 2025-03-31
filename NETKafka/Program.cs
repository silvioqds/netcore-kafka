using NETKafka.Aggregate;
using NETKafka.Common.Events;
using NETKafka.Kafka;

var producer = new KafkaEventProducer();

Guid accountId = Guid.NewGuid();
var bankAccount = BankAccount.Open(accountId, "Silvio Queiroz", 2100);
bankAccount.Deposit(accountId, 100);
bankAccount.WithDraw(accountId, 100);
bankAccount.WithDraw(accountId, 1000);

foreach (var @event in bankAccount.Events)
    await producer.PublisherEvent("event-account", @event);

Console.WriteLine(bankAccount.ToString());

KafkaEventReplayer<BankAccount> replay = new KafkaEventReplayer<BankAccount>("event-account", accountId, bankAccount);

bankAccount = replay.ReplayEvents();
Console.WriteLine(bankAccount.ToString());

bankAccount = replay.RevertAllEvents();
Console.WriteLine($"Initial State: {bankAccount.ToString()}");


