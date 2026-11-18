---
id: arq-02-intro
order: 181
kind: intro
group: Arquitetura .NET · Parte 20 · Domínio e DDD Prático
title: Domínio e DDD Prático — visão geral
subtitle: Bem-vindo ao segundo capítulo.
minutes: 5
hue: 211
origem: Arquitetura-microservicos.NET/02-dominio-ddd-pratico/README.md
---

Bem-vindo ao segundo capítulo. Aqui desmistificamos o Domain-Driven Design (DDD), aplicando seus princípios táticos e estratégicos com foco no negócio, sem rituais desnecessários e sem transformar o projeto em uma "religião".

## 📑 Aulas e Tópicos Deste Capítulo

1. **[Entidades, Identidade e Regras de Negócio Invioláveis](#/c/arq-02-01-entidades-e-regras-de-dominio)**
   - O que define uma entidade: identidade contínua vs atributos.
   - Modelo Anêmico vs Modelo Rico em .NET 10.
   - Protegendo coleções com `IReadOnlyCollection<T>` e protegendo invariantes.
2. **[Value Objects e Imutabilidade com C# 14 Records](#/c/arq-02-02-value-objects-e-imutabilidade)**
   - Eliminando a *Primitive Obsession* (`Money`, `Address`, `Sku`).
   - Igualdade estrutural e imutabilidade nativa com `readonly record struct`.
   - Mapeamento no EF Core 10 com `ComplexProperty`.
3. **[Agregados, Aggregate Roots e Limites Transacionais](#/c/arq-02-03-agregados-e-aggregate-roots)**
   - O papel da Raiz de Agregado e limites de consistência.
   - As 4 regras de ouro do design de agregados (Vaughn Vernon).
   - Por que alterar apenas 1 agregado por transação.
4. **[Domain Services, Domain Events e Abstração de Repositórios](#/c/arq-02-04-domain-services-e-events)**
   - Domain Service vs Application Service: a fronteira cirúrgica.
   - Domain Events (em memória) vs Integration Events (rede/RabbitMQ).
   - Despacho transacional via interceptors no EF Core 10.
5. **[DDD Estratégico vs Tático: Bounded Contexts e Context Mapping](#/c/arq-02-05-ddd-estrategico-vs-tatico)**
   - Linguagem Ubíqua sem ambiguidades entre squads.
   - Bounded Contexts como fronteiras naturais de microsserviços.
   - Anti-Corruption Layer (ACL) e onde o DDD ajuda vs onde é exagero.

---
⬅️ Voltar para o **[Capítulo 1: Fundamentos de Arquitetura](#/c/arq-01-intro)** | Avançar para o **[Capítulo 3: Dados e Persistência](#/c/arq-03-intro)** ➡️
