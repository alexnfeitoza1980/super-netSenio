---
id: arq-11-intro
order: 219
kind: intro
group: Arquitetura .NET · Parte 29 · CI/CD e DevOps
title: CI/CD e DevOps — visão geral
subtitle: Bem-vindo ao décimo primeiro capítulo.
minutes: 5
hue: 310
origem: Arquitetura-microservicos.NET/11-cicd-devops/README.md
---

Bem-vindo ao décimo primeiro capítulo. Aqui automatizamos o ciclo de entrega de valor dos nossos microsserviços em **.NET 10**, implementando pipelines robustos de Integração Contínua (CI) e Entrega Contínua (CD) com **GitHub Actions**, escaneamento de segurança com **Trivy**, e estratégias de deploy sem tempo de inatividade (**Zero Downtime**) usando **Blue/Green** e **Canary Releases**.

## 📑 Aulas e Tópicos Deste Capítulo

1. **[Pipelines de CI/CD, Estratégias de Deploy e Zero Downtime no .NET 10](#/c/arq-11-01-pipelines-ci-cd-e-deploy)**
   - Trunk-Based Development vs GitFlow: por que evitar branches longas em microsserviços.
   - Pipeline completo com GitHub Actions: restore, build, testes com cobertura, scan de vulnerabilidades com Trivy e publicação de containers.
   - Deploys sem downtime: Blue/Green vs Canary Deployments e mitigação de risco com telemetria.
   - Como migrar esquemas de banco de dados sem quebrar versões concorrentes.

---
⬅️ Voltar para o **[Capítulo 10: Escalabilidade](#/c/arq-10-intro)** | Avançar para o **[Capítulo 12: Testes](#/c/arq-12-intro)** ➡️
