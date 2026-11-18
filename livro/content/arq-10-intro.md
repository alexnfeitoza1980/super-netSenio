---
id: arq-10-intro
order: 217
kind: intro
group: Arquitetura .NET · Parte 28 · Escalabilidade
title: Escalabilidade — visão geral
subtitle: Bem-vindo ao décimo capítulo.
minutes: 5
hue: 299
origem: Arquitetura-microservicos.NET/10-escalabilidade/README.md
---

Bem-vindo ao décimo capítulo. Aqui preparamos nossos microsserviços para suportar cargas astronômicas de tráfego, dominando a transição de escalabilidade vertical para horizontal, a identificação cirúrgica dos 4 grandes gargalos de infraestrutura e o autoscaling orientado a eventos com **KEDA** e testes com **k6**.

## 📑 Aulas e Tópicos Deste Capítulo

1. **[Escalabilidade Horizontal, Gargalos Críticos e Autoscaling no .NET 10](#/c/arq-10-01-escalabilidade-horizontal-e-gargalos)**
   - Escalabilidade Vertical (Scale Up) vs Horizontal (Scale Out) em arquiteturas Stateless.
   - Os 4 gargalos mortais: Connection Pool de Banco, CPU, Garbage Collection e I/O bloqueante.
   - Autoscaling orientado a eventos com KEDA: escalando pods pela profundidade da fila do RabbitMQ.
   - Testes de carga e estresse com scripts reais do k6.

---
⬅️ Voltar para o **[Capítulo 9: Containerização](#/c/arq-09-intro)** | Avançar para o **[Capítulo 11: CI/CD e DevOps](#/c/arq-11-intro)** ➡️
