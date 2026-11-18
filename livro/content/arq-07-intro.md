---
id: arq-07-intro
order: 206
kind: intro
group: Arquitetura .NET · Parte 25 · Segurança
title: Segurança — visão geral
subtitle: Bem-vindo ao sétimo capítulo.
minutes: 5
hue: 266
origem: Arquitetura-microservicos.NET/07-seguranca/README.md
---

Bem-vindo ao sétimo capítulo. Aqui construímos as defesas criptográficas e os padrões de governança do nosso ecossistema de microsserviços em **.NET 10**, cobrindo desde autenticação e autorização por políticas com JWT até segurança machine-to-machine, mTLS e prevenção das ameaças da OWASP Top 10.

## 📑 Aulas e Tópicos Deste Capítulo

1. **[Autenticação, JWT, Tokens e Autorização Baseada em Políticas (.NET 10)](#/c/arq-07-01-autenticacao-jwt-e-identity)**
   - O fluxo completo de OAuth 2.0 e OpenID Connect com Identity Provider (IdP).
   - Anatomia de um JWT: Header, Claims e Assinatura criptográfica assíncrona.
   - Resource Server no .NET 10: validação local via chave pública sem I/O.
   - Autorização baseada em Políticas (*Policy-Based Authorization*) e Refresh Token Rotation.
2. **[Segurança Service-to-Service, Propagação de Tokens e OWASP Top 10](#/c/arq-07-02-service-to-service-e-owasp)**
   - Comunicação Machine-to-Machine com OAuth2 Client Credentials e mTLS.
   - Propagação de identidade de usuário via `DelegatingHandler`.
   - As maiores ameaças da OWASP em APIs: foco em BOLA (Broken Object Level Authorization).
   - Headers de segurança obrigatórios para proteção em produção.

---
⬅️ Voltar para o **[Capítulo 6: Resiliência](#/c/arq-06-intro)** | Avançar para o **[Capítulo 8: Observabilidade](#/c/arq-08-intro)** ➡️
