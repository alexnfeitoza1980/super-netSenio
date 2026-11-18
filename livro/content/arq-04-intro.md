---
id: arq-04-intro
order: 193
kind: intro
group: Arquitetura .NET · Parte 22 · APIs e HTTP
title: APIs e HTTP — visão geral
subtitle: Bem-vindo ao quarto capítulo.
minutes: 5
hue: 233
origem: Arquitetura-microservicos.NET/04-apis-e-http/README.md
---

Bem-vindo ao quarto capítulo. Aqui construímos a porta de entrada dos nossos microsserviços, dominando o estilo arquitetural REST, o pipeline HTTP do ASP.NET Core no **.NET 10**, benchmarks reais entre Minimal APIs e Controllers, validações robustas, tratamento global de erros conforme a RFC 7807 (ProblemDetails) e proteção por Rate Limiting.

## 📑 Aulas e Tópicos Deste Capítulo

1. **[Fundamentos de REST, Protocolo HTTP e Serialização no .NET 10](#/c/arq-04-01-fundamentos-rest-e-http)**
   - Métodos HTTP seguros e idempotentes (`GET`, `PUT`, `DELETE` vs `POST`, `PATCH`).
   - Tabela de status codes essenciais para microsserviços.
   - Serialização JSON de alta velocidade com Source Generators (`System.Text.Json`).
2. **[Minimal APIs vs Controllers no .NET 10: Comparativo e Arquitetura](#/c/arq-04-02-minimal-apis-vs-controllers)**
   - Comparativo de Throughput (+88% RPS), tempo de startup e alocação de memória.
   - Arquitetura limpa para Minimal APIs usando `MapGroup`, Feature Folders e `IEndpoint`.
   - Endpoint Filters integrados ao FluentValidation.
3. **[Padronização de Erros com ProblemDetails (RFC 7807) e IExceptionHandler](#/c/arq-04-03-padronizacao-erros-problemdetails)**
   - A anatomia formal da RFC 7807 e extensões para rastreabilidade (`traceId`).
   - O manipulador global nativo do .NET 10: `IExceptionHandler`.
   - Como evitar o vazamento de exceções e stack traces em produção (CWE-209).
4. **[Pipeline HTTP: Middlewares, Filtros, Rate Limiting e OpenAPI](#/c/arq-04-04-middlewares-e-filtros)**
   - A ordem canônica dos middlewares no ASP.NET Core.
   - Middlewares globais vs Endpoint Filters cirúrgicos.
   - Algoritmos de Rate Limiting nativos: Sliding Window, Token Bucket, Concurrency.
   - Documentação moderna de APIs com OpenAPI nativo e Scalar UI.

---
⬅️ Voltar para o **[Capítulo 3: Dados e Persistência](#/c/arq-03-intro)** | Avançar para o **[Capítulo 5: Comunicação entre Microsserviços](#/c/arq-05-intro)** ➡️
