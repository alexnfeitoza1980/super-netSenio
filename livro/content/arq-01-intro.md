---
id: arq-01-intro
order: 174
kind: intro
group: Arquitetura .NET · Parte 19 · Fundamentos de Software e Arquitetura
title: Fundamentos de Software e Arquitetura — visão geral
subtitle: Bem-vindo ao primeiro capítulo do nosso livro técnico.
minutes: 5
hue: 200
origem: Arquitetura-microservicos.NET/01-fundamentos-arquitetura/README.md
---

Bem-vindo ao primeiro capítulo do nosso livro técnico. Aqui construímos as bases que sustentam qualquer arquitetura de software sólida, moderna e pronta para produção com **.NET 10**.

## 📑 Aulas e Tópicos Deste Capítulo

1. **[Clean Architecture em Camadas Modernas](#/c/arq-01-01-clean-architecture)**
   - O porquê do isolamento do domínio.
   - Divisão de responsabilidades: Domain, Application, Infrastructure e API.
   - Regra de dependência estrita e inversão de controle.
2. **[Injeção de Dependência e IoC no .NET 10](#/c/arq-01-02-injecao-dependencia-e-ioc)**
   - Lifetimes: Transient, Scoped e Singleton.
   - A armadilha de Captive Dependencies e como o .NET valida escopos.
   - Keyed Services em C# 14 / .NET 10.
3. **[SOLID e Princípios de POO em C# Moderno](#/c/arq-01-03-solid-e-oop-moderno)**
   - SRP, OCP, LSP, ISP e DIP aplicados sem burocracia.
   - Encapsulamento de dados, classes `sealed` e records imutáveis.
   - Por que favorecer Composição sobre Herança.
4. **[Configuração, Options Pattern, Contratos e DTOs](#/c/arq-01-04-configuracao-options-e-contratos)**
   - `IOptions`, `IOptionsSnapshot` e `IOptionsMonitor`.
   - Fail-fast no startup com validações de `DataAnnotations`.
   - Contratos de entrada e saída (DTOs) e mapeamento zero-alocação com Riok.Mapperly.
5. **[Versionamento de APIs e Architecture Decision Records (ADR)](#/c/arq-01-05-versionamento-e-adrs)**
   - Estratégias de versionamento HTTP (URL, Query, Headers).
   - O que é breaking change e estratégias de deprecation.
   - Como documentar decisões técnicas irreversíveis com ADRs.
6. **[Monólito vs Monólito Modular vs Microsserviços](#/c/arq-01-06-monolito-vs-modular-vs-microsservicos)**
   - Quando usar cada abordagem e a matriz de trade-offs.
   - O padrão inegociável do *Database per Service*.
   - Serviços Stateless e os princípios dos Twelve-Factor Apps.

---
⬅️ Voltar para o **Sumário Geral Mestre** | Avançar para o **[Capítulo 2: Domínio e DDD Prático](#/c/arq-02-intro)** ➡️
