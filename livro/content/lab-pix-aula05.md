---
id: lab-pix-aula05
order: 258
kind: lab
num: 25
group: Laboratório · Parte 41 · Banco Digital: Outbox, Inbox e Pix
title: Transactional Outbox: corrija as duas falhas
subtitle: Coloque o pagamento e a intenção de publicar na mesma transação. Depois teste separadamente o que acontece quando o despachante tenta publicar.
minutes: 5
hue: 34
origem: aula-guiada-monolito-gemini/site/assets/js/pix-lessons.js
---

### 1. Use duas vias da mesma ficha

Volte ao quadro 2 da confeitaria. Uma via da ficha 42 fica no livro e a outra entra na bandeja de expedição. As duas fazem parte do mesmo fechamento do pedido. O telefone pode continuar quebrado. Quando a comunicação voltar, alguém consegue consultar a bandeja e descobrir o que ainda precisa ser entregue.

No código, a operação vai para `operacoes_pagamento` e a intenção de publicar vai para `outbox_messages`. Ambas entram no mesmo `AppDbContext` e são gravadas pelo mesmo `SaveChangesAsync`. A mensagem contém um identificador estável, o tipo de evento, o JSON e a data de criação. Ela nasce com `ProcessadoEm = null`.

```csharp
var outboxMessage = new OutboxMessage(
    evento.EventId,
    nameof(PagamentoPixAgendadoEvent),
    JsonSerializer.Serialize(evento),
    DateTime.UtcNow);

await _pagamentoRepository.AdicionarAsync(operacao, cancellationToken);
await _outboxRepository.AdicionarAsync(outboxMessage, cancellationToken);
await _unitOfWork.CommitAsync(cancellationToken);
```

Abra `AgendarPagamentoPixUseCase.cs`. Siga as chamadas de `AdicionarAsync` nos dois repositórios e confirme que elas preparam inserções no contexto. Abra `UnitOfWork.cs` e localize o `SaveChangesAsync`. Se cada repositório confirmasse a própria transação, a garantia do exemplo desapareceria.

**Estado esperado da API.** Uma resposta de sucesso confirma o agendamento. A liquidação é uma etapa posterior. Se houver retry da requisição HTTP depois de a resposta se perder, um novo evento pode ser criado. Esse problema exige idempotência na entrada, com um identificador estável da solicitação do cliente.

### 2. Prove a gravação conjunta no PostgreSQL

Primeiro agende um Pix com os repositórios reais. Em outro contexto, busque a operação e a mensagem. Abra o JSON e compare o `OperacaoId` com o ID do pagamento e o `EventId` com o ID da outbox. Verifique também que o carimbo de processamento ainda está vazio.

Depois force a falha na **segunda tabela** do agendamento. No teste de rollback, a restrição `CHECK (false)` está na outbox. Isso obriga o banco a rejeitar o commit. Abra um contexto novo e confirme que as duas tabelas continuam vazias. A conta criada para preparar o cenário foi gravada antes e continua existindo.

```bash
dotnet test $projeto --filter 'FullyQualifiedName~LaboratorioOutboxTests.Outbox_' --logger 'console;verbosity=detailed'
```

Compare os dois blocos de saída. Quando o commit dá certo, aparece um Pix de R$ 150 e um aviso pendente vinculado a ele. Quando o banco rejeita a inserção na outbox, aparece o erro `23514` da tabela `outbox_messages`. A consulta seguinte deve mostrar zero pagamentos e zero avisos.

O teste unitário do caso de uso conta chamadas ao commit com um dublê. O teste com PostgreSQL comprova o efeito da transação no banco. Os dois protegem partes diferentes do comportamento.

### 3. Ligue o despachante e simule uma falha

1. O `OutboxMessageDispatcher` consulta as mensagens pendentes em lotes. Para um `PagamentoPixAgendadoEvent` , ele lê o JSON e recupera o mesmo identificador criado no agendamento.
2. Ele chama `IEventBus.PublicarAsync` . Se a publicação falhar, registra o erro e deixa o carimbo vazio para permitir nova tentativa.
3. Se o barramento aceitar a mensagem, ele marca a linha como processada e grava a alteração. Se esse commit falhar, uma próxima leitura do banco decide o que continua pendente.

```bash
dotnet test $projeto --filter 'FullyQualifiedName~OutboxMessageDispatcherTests'
```

O teste verifica dois casos. Quando a publicação é aceita, o mesmo evento é enviado e recebe carimbo. Quando o barramento lança uma exceção, o erro aparece na linha e o carimbo continua vazio. Antes da correção, o teste expõe o carimbo de sucesso sem uma chamada de publicação.

::: nota Checkpoint · O que o carimbo prova?
Neste repositório o barramento é um canal em memória. O carimbo indica que esse canal aceitou o evento. Se o processo morrer, os itens do canal podem desaparecer. Um serviço em outro processo também precisa de um transporte que atravesse essa fronteira. A próxima aula propõe a confirmação de um broker durável e a deduplicação no consumidor.
:::

[Avançar para 3 · Investigue a Reentrega →](#aula06)
