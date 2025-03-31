**Kafka Event Replayer - Event Sourcing com Kafka**

Este projeto implementa um replayer de eventos para sistemas baseados em Event Sourcing, utilizando Apache Kafka como log de eventos. Ele permite reconstruir o estado de um agregado (Aggregate) a partir dos eventos publicados em um tópico do Kafka.

**Funcionalidades** 📌 

🔹 Reprocessa eventos de um agregado a partir de um tópico Kafka.

🔹 Identifica a partição correta para garantir leitura consistente.

🔹 Aplica eventos em sequência para reconstruir o estado do agregado.

🔹 Suporte a rollback, revertendo eventos em ordem inversa.

**Tecnologias utilizadas** 🚀 

🔹.NET (C#)

🔹Confluent Kafka

🔹Event Sourcing

**Como executar** ⚡ 

Tenha o Docker instalado em sua máquina e basta executar 'docker-compose up' para subir o kafka

Compile e execute o projeto, informando o tópico Kafka e o Aggregate ID.

O sistema consumirá os eventos e reconstruirá o estado do agregado.

**Melhorias**

🔹Persistência de eventos em um Event Store (Ex: PostgreSQL, EventStoreDB).

🔹Snapshots para melhorar a performance da reconstrução.

🔹Suporte a múltiplos agregados e versionamento de eventos.