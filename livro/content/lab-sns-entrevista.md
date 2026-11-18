---
id: lab-sns-entrevista
order: 274
kind: lab
num: 41
group: Laboratório · Parte 45 · super-netSenior · da repetição à solução única
title: Cenários de entrevista com a API
subtitle: Exercícios práticos contra a API do Banking, com a evidência esperada para cada cenário.
minutes: 5
hue: 215
origem: super-netSenior/entrevista/README.md
---

## Demonstração com a API

Use um PostgreSQL local descartável. Na raiz do `super-netSenior`, configure `ConnectionStrings__Banking` para esse banco e suba a API em Development:

```powershell
dotnet run --project src/Banking/SuperNetSenior.Banking.Api
```

No Swagger local, cadastre um usuário de teste e faça login. Use a URL impressa pela API e cole o token quando solicitado. O roteiro cria duas contas de teste para o usuário autenticado e grava operações nessas contas.

```powershell
.\entrevista\Testar-Transferencia.ps1 -Api http://localhost:5000 -Token (Read-Host 'JWT do laboratório' -AsSecureString)
```

O roteiro deposita 100, dispara dois POSTs sem aguardar entre eles e consulta os saldos. A transferência de 30 deve produzir saldos de 70 e 30. O reenvio deve recuperar o mesmo identificador, e mudar o valor mantendo a chave deve gerar 409. A segunda requisição concorrente pode recuperar o resultado ou receber 409 antes do reenvio. Repita o roteiro para observar a concorrência em execuções diferentes. A evidência do roteiro é obtida pela API. Para verificar a persistência diretamente, consulte as tabelas de transferências e lançamentos pelo identificador da operação e pelas contas.

## Cenários e critérios de aceite

| Cenário | Exercício | Evidência esperada |
|---|---|---|
| Reenvio e duplicidade | Enviar mesma chave e mesmo corpo em paralelo e depois repetir | Uma transferência, mesmo identificador nas respostas de sucesso, saldo 70/30 |
| Chave reutilizada | Enviar mesma chave com outro valor ou destino | HTTP 409 e saldo preservado |
| Gasto duplo | Tentar duas transferências de 80 a partir de saldo 100, com chaves diferentes | Um único débito efetivo, saldo final 20, recusa ou conflito na segunda tentativa |
| Atomicidade | Injetar uma falha de persistência durante a operação | Ausência de transferência parcial, débito e crédito preservados |
| Acesso por titularidade | Consultar ou debitar conta com JWT de outro titular | HTTP 403 e nenhuma alteração |
| Autenticação | Repetir operação com JWT expirado ou inválido | HTTP 401 antes da regra de negócio |
| Validação | Caçar valores inválidos, origem igual ao destino e saldo insuficiente | Resposta coerente e nenhuma escrita financeira |
| Timeout ambíguo | Ignorar a primeira resposta e repetir a solicitação | Resultado consultável com a mesma chave e sem débito adicional |
| Outbox | Interromper publicação depois do commit no agendamento Pix | Evento disponível para publicação posterior |
| Inbox e broker | Reentregar mensagem após falha de confirmação | Um efeito localizado e redelivery processável. Use o laboratório Pix do monólito como referência para portar o consumidor |
| Saga | Simular falha de pagamento depois de reservar estoque | Compensação idempotente e estado recuperável. O exemplo está em `microservico-senior` |
| Dependência externa | Usar Moq para simular timeout, recusa e recuperação do provedor | Testes exercitam retry seguro e preservam a identidade da operação |
| Cache | Invalidar Redis após atualização e simular leituras antes e depois do commit | Defasagem observável, TTL definido e fonte de verdade preservada |
| SQL | Comparar planos e tempos de consulta com e sem índice | Estatísticas de leitura, tempo, seletividade e custo de escrita |
| NoSQL | Modelar consulta por cliente e período em MongoDB e DynamoDB | Índices ou chaves explicitam o padrão de acesso, consistência e custo |
| Operação e evolução | Exercitar tracing, limites de retry, rollout e mudança compatível de schema | Diagnóstico por correlação, falhas visíveis, plano de reversão |

O roteiro PowerShell cobre as duas primeiras linhas. O restante é uma pauta de prática que usa os testes existentes, os projetos originais e extensões a implementar. Para conferir as verificações automatizadas do Banking, rode `dotnet test super-netSenior.slnx`.

## Relatos do Banco BMP e do Carrefour

- **BMP, crédito consignado.** Registre a etapa em que atuou, a regra ou integração sob sua responsabilidade, o problema concreto, a decisão e o resultado que consegue sustentar.
- **Carrefour, sistemas financeiros.** Escolha um problema distinto e descreva como identificou a causa, o que implementou e como verificou o resultado.

Para cada relato, ensaie a resposta em 90 segundos e prepare as repreguntas sobre falhas, testes, alternativas e o que faria diferente hoje.
