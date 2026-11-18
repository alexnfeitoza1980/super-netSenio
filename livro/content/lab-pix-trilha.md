---
id: lab-pix-trilha
order: 260
kind: lab
num: 27
group: Laboratório · Parte 41 · Banco Digital: Outbox, Inbox e Pix
title: Evolua o Banco Digital com evidências
subtitle: Em cada exercício, execute uma mudança, observe o resultado e explique o motivo da decisão.
minutes: 5
hue: 62
origem: aula-guiada-monolito-gemini/site/assets/js/pix-lessons.js
---

### Pratique no mesmo projeto

1. **Arquitetura e testes · disponível.** Siga uma regra do Pix do caso de uso até o PostgreSQL. Compare um teste de domínio com o teste de integração que provoca rollback. [Casos de uso](#aula03) · [Outbox](#aula05) .
2. **Idempotência · disponível.** Envie duas vezes o mesmo evento ao PixService, confira o ID retornado e conte os efeitos no banco. [Laboratório de Inbox](#aula06) .
3. **Mensageria e recuperação · próximo laboratório.** Ligue o monólito ao PixService com RabbitMQ. Interrompa o consumidor entre a gravação e a confirmação. Observe a reentrega e conte as liquidações. Depois experimente uma importação em etapas, com ordem por operação, tarefas independentes em paralelo e reconciliação quando o parceiro externo retorna timeout.
4. **Performance e persistência · planejado.** Gere um histórico grande de pagamentos. Compare uma busca repetida em listas com um índice em memória. No PostgreSQL, compare o plano e o tempo de uma consulta antes e depois de um índice. Use os resultados para decidir se um modelo separado para leitura compensa o custo de sincronização.
5. **Entrega e operação · planejado.** Faça o GitHub Actions rodar os testes e gerar uma imagem Docker versionada. Use a mesma imagem em homologação e produção simulada. Em Kubernetes, acompanhe uma atualização e um rollback, vendo como as verificações de saúde e o desligamento afetam o tráfego.
6. **Incidentes e decisões de equipe · planejado.** Simule fila atrasada e lentidão nas consultas ao mesmo tempo. Defina qual incidente tratar primeiro, publique uma atualização de impacto para o negócio e registre a decisão técnica em um ADR. Use revisão de código e testes para verificar a correção, inclusive quando a mudança tiver sido sugerida por IA.
7. **Transferência para Laravel · planejado.** Depois de conhecer o fluxo em .NET, faça um recorte em PHP. Compare a separação de domínio, serviços e DTOs, inspecione o SQL gerado pelo Eloquent e reproduza um job com Redis e Horizon. Registre quais garantias vieram do banco e quais dependeram da configuração da fila.

Em cada etapa, anote quatro coisas. O que você executou, onde cada parte rodou, por que escolheu essa abordagem e qual saída confirma o resultado. Use essas anotações para explicar o exercício com suas palavras.
