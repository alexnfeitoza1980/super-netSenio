---
id: arq-12-intro
order: 222
kind: intro
group: Arquitetura .NET · Parte 30 · Testes
title: Testes — visão geral
subtitle: Bem-vindo ao décimo segundo capítulo.
minutes: 5
hue: 321
origem: Arquitetura-microservicos.NET/12-testes/README.md
---

Bem-vindo ao décimo segundo capítulo. Aqui construímos a fortaleza de qualidade dos nossos microsserviços em **.NET 10**, dominando desde a pureza e velocidade dos testes unitários de domínio e aplicação com **xUnit**, **FluentAssertions** e **NSubstitute**, até testes de integração reais contra bancos e filas efêmeras com **Testcontainers** e testes de contrato com **Pact**.

## 📑 Aulas e Tópicos Deste Capítulo

1. **[A Pirâmide de Testes em Microsserviços e Testes Unitários no .NET 10](#/c/arq-12-01-piramide-de-testes)**
   - A estrutura da pirâmide: Unitários vs Contrato/Integração vs E2E.
   - Test Doubles desmistificados: Dummies, Stubs, Mocks e Fakes.
   - Testes unitários puros de Domínio (sem mocks) e testes de Aplicação com NSubstitute.
   - Por que o banco em memória do EF Core (`UseInMemoryDatabase`) é uma armadilha.
2. **[Testcontainers para .NET 10, WebApplicationFactory e Testes de Contrato](#/c/arq-12-02-testcontainers-e-testes-integracao)**
   - O fim dos mocks de banco: subindo SQL Server e RabbitMQ reais sob demanda em containers efêmeros.
   - `WebApplicationFactory<Program>` e fixtures assíncronas reutilizáveis no .NET 10.
   - Testes de ponta a ponta de endpoints HTTP e validação com `ProblemDetails`.
   - Consumer-Driven Contract Testing com Pact: blindando quebras de contrato entre squads.

---
⬅️ Voltar para o **[Capítulo 11: CI/CD e DevOps](#/c/arq-11-intro)** | Avançar para o **[Capítulo 13: Mensageria e Eventos](#/c/arq-13-intro)** ➡️
