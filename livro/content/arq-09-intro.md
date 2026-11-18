---
id: arq-09-intro
order: 215
kind: intro
group: Arquitetura .NET · Parte 27 · Containerização
title: Containerização — visão geral
subtitle: Bem-vindo ao nono capítulo.
minutes: 5
hue: 288
origem: Arquitetura-microservicos.NET/09-containerizacao/README.md
---

Bem-vindo ao nono capítulo. Aqui transformamos nosso código em artefatos reproduzíveis, portáveis e seguros através do **Docker**, dominando a construção de imagens ultraleves para **.NET 10** e a orquestração completa do ecossistema local via **Docker Compose**.

## 📑 Aulas e Tópicos Deste Capítulo

1. **[Dockerfiles Multi-Stage Otimizados e Docker Compose no .NET 10](#/c/arq-09-01-docker-e-docker-compose)**
   - O padrão Multi-Stage Build: isolando o SDK de compilação do runtime de execução.
   - Imagens Chiseled da Microsoft: segurança Zero-CVE e containers sem shell nem privilégios de root (`USER $APP_UID`).
   - Otimização radical de tamanho: de 900MB para menos de 80MB.
   - `docker-compose.yml` mestre: orquestrando Microsserviços .NET 10, SQL Server, Redis, RabbitMQ e Jaeger em uma rede privada.

---
⬅️ Voltar para o **[Capítulo 8: Observabilidade](#/c/arq-08-intro)** | Avançar para o **[Capítulo 10: Escalabilidade](#/c/arq-10-intro)** ➡️
