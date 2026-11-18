---
id: arq-03-intro
order: 187
kind: intro
group: Arquitetura .NET · Parte 21 · Dados e Persistência
title: Dados e Persistência — visão geral
subtitle: Bem-vindo ao terceiro capítulo.
minutes: 5
hue: 222
origem: Arquitetura-microservicos.NET/03-dados-persistencia/README.md
---

Bem-vindo ao terceiro capítulo. Aqui abordamos a camada de dados e persistência em microsserviços com **EF Core 10**, **SQL Server** e **Redis / HybridCache**, dominando desde mapeamentos fluentes e migrações idempotentes até controle de concorrência, otimização extrema de queries e cache em camadas.

## 📑 Aulas e Tópicos Deste Capítulo

1. **[EF Core 10 e SQL Server: DbContext, Mapeamento e Migrations](#/c/arq-03-01-ef-core-e-sql-server)**
   - Configuração de DbContext e Fluent API sem poluir o domínio.
   - Relacionamentos, Foreign Keys e Backing Fields para coleções protegidas.
   - Migrations idempotentes em pipelines de CI/CD e Initial Seed.
2. **[Transações, Níveis de Isolamento e Concorrência no EF Core 10](#/c/arq-03-02-transacoes-e-concorrencia)**
   - Concorrência Otimista com `RowVersion` e tratamento de `DbUpdateConcurrencyException`.
   - Níveis de isolamento no SQL Server: por que usar Snapshot Isolation.
   - Execution Strategy para retries automáticos com transações manuais.
3. **[Performance e Otimização de Queries no EF Core 10](#/c/arq-03-03-performance-e-otimizacao-queries)**
   - O problema do N+1 e como eliminá-lo com Projeções diretas.
   - `AsSplitQuery()` para evitar explosão cartesiana de dados.
   - `AsNoTracking()` vs Tracking de entidades.
   - Paginação Keyset (Seek) de alta velocidade vs Offset (`Skip/Take`).
   - Índices de cobertura (`INCLUDE`) e índices filtrados.
4. **[Cache Distribuído com Redis e HybridCache no .NET 10](#/c/arq-03-04-cache-distribuido-redis-e-hybridcache)**
   - O problema do Cache Stampede (Efeito Manada).
   - A nova abstração do .NET 10: `HybridCache` (L1 em memória + L2 no Redis).
   - Invalidação cirúrgica por Tags.

---
⬅️ Voltar para o **[Capítulo 2: Domínio e DDD Prático](#/c/arq-02-intro)** | Avançar para o **[Capítulo 4: APIs e HTTP](#/c/arq-04-intro)** ➡️
