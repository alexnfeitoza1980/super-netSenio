---
id: arq-14-intro
order: 228
kind: intro
group: Arquitetura .NET · Parte 32 · Gateway / BFF (Backend for Frontend)
title: Gateway / BFF (Backend for Frontend) — visão geral
subtitle: Bem-vindo ao décimo quarto capítulo.
minutes: 5
hue: 343
origem: Arquitetura-microservicos.NET/14-gateway-bff/README.md
---

Bem-vindo ao décimo quarto capítulo. Aqui projetamos a camada de entrada do nosso ecossistema com **YARP (Yet Another Reverse Proxy)** no **.NET 10**, dominando o roteamento reverso, segurança e rate limiting na borda, além da especialização de contratos para experiências Web e Mobile através do padrão **BFF (Backend for Frontend)**.

## 📑 Aulas e Tópicos Deste Capítulo

1. **[API Gateway com YARP e o Padrão BFF no .NET 10](#/c/arq-14-01-api-gateway-yarp-e-bff)**
   - Por que nunca expor microsserviços diretamente para a internet.
   - O padrão BFF (Backend for Frontend): Web vs Mobile.
   - YARP da Microsoft: clusters, rotas, transformações de path e cabeçalhos.
   - Agregação de respostas em paralelo (Scatter-Gather) e validação de autenticação na borda.

---
⬅️ Voltar para o **[Capítulo 13: Mensageria e Eventos](#/c/arq-13-intro)** | Avançar para o **[Capítulo 15: Performance](#/c/arq-15-intro)** ➡️
