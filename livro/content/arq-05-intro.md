---
id: arq-05-intro
order: 198
kind: intro
group: Arquitetura .NET · Parte 23 · Comunicação entre Microsserviços
title: Comunicação entre Microsserviços — visão geral
subtitle: Bem-vindo ao quinto capítulo.
minutes: 5
hue: 244
origem: Arquitetura-microservicos.NET/05-comunicacao-microsservicos/README.md
---

Bem-vindo ao quinto capítulo. Aqui desvendamos a espinha dorsal dos sistemas distribuídos: como os microsserviços conversam entre si sem criar acoplamento temporal frágil ou inconsistências catastróficas em produção.

## 📑 Aulas e Tópicos Deste Capítulo

1. **[Comunicação Síncrona via HTTP e IHttpClientFactory no .NET 10](#/c/arq-05-01-comunicacao-sincrona-http)**
   - O perigo do `new HttpClient()`: Socket Exhaustion vs DNS Staleness.
   - Resolução definitiva com `IHttpClientFactory` e Typed Clients.
   - O efeito cascata de disponibilidade e latência na comunicação síncrona.
2. **[Comunicação Assíncrona e Eventos de Integração no .NET 10](#/c/arq-05-02-comunicacao-assincrona-eventos)**
   - Desacoplamento temporal com mensageria e RabbitMQ.
   - Contratos de mensagens imutáveis com records do C# 14.
   - O mito do "Exactly-Once Delivery": Teorema dos Dois Generais e a realidade *At-Least-Once*.
3. **[O Problema da Escrita Dupla e os Padrões Outbox e Inbox](#/c/arq-05-03-outbox-inbox-patterns)**
   - Por que salvar no banco e publicar na fila cria inconsistência silenciosa.
   - O Transactional Outbox Pattern implementado no EF Core 10 com MassTransit.
   - O Transactional Inbox Pattern para proteção contra processamentos repetidos.
4. **[Consistência Eventual, Idempotência e Dead Letter Queues (DLQ)](#/c/arq-05-04-consistencia-eventual-e-idempotencia)**
   - Consistência eventual na prática e expectativas de negócio.
   - Idempotência natural vs sintética com chaves de deduplicação.
   - Tratamento de mensagens fora de ordem e proteção contra *Poison Messages* com DLQ.

---
⬅️ Voltar para o **[Capítulo 4: APIs e HTTP](#/c/arq-04-intro)** | Avançar para o **[Capítulo 6: Resiliência](#/c/arq-06-intro)** ➡️
