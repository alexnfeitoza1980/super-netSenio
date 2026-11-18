---
id: lab-pix-aula06
order: 259
kind: lab
num: 26
group: Laboratório · Parte 41 · Banco Digital: Outbox, Inbox e Pix
title: Reentrega, Inbox e investigação de incidentes
subtitle: Envie o mesmo evento duas vezes ao PixService e confira quantas liquidações ficaram no PostgreSQL.
minutes: 7
hue: 48
origem: aula-guiada-monolito-gemini/site/assets/js/pix-lessons.js
---

### O ingresso 17 já entrou

::: ilustracao img/lab-pix/inbox-cinema.png | O número do ingresso identifica uma entrada. Uma segunda cópia do mesmo ingresso não dá direito a uma segunda passagem.
:::

No cinema, a atendente consulta o número do ingresso. No Pix, o consumidor consulta ou registra o `EventId`. Dois Pix legítimos de R$ 150, para a mesma chave e no mesmo minuto, continuam sendo dois pedidos. Por isso, valor, chave e minuto são uma má chave de deduplicação. Para reentregas, use o identificador estável do evento original.

### 1. Preveja a janela de falha

Imagine que um broker durável confirme a publicação e, logo depois, o monólito caia antes de gravar `ProcessadoEm`. Ao reiniciar, a outbox ainda parece pendente e o produtor pode publicar o mesmo evento de novo. Também há outro ponto de reentrega. O consumidor pode confirmar seu banco local e cair antes de enviar o ACK ao broker.

A confirmação do produtor informa que o broker aceitou a publicação conforme a configuração do transporte. O ACK do consumidor informa que aquela entrega foi tratada. Teste as duas janelas separadamente, sempre repetindo o mesmo `EventId`.

### 2. Execute a Inbox e veja o banco responder

No mesmo PowerShell da aula de Outbox, mantenha o PostgreSQL do Docker ligado e a variável `BANCO_TEST_POSTGRES` configurada. O .NET roda no Windows e inicia a API real do `PixService` dentro do processo de teste. Ele envia duas requisições HTTP com o mesmo `EventId` para representar duas entregas. O PostgreSQL recebe as gravações pela porta `55432`.

```bash
$projeto = (Get-ChildItem 'tests/*IntegrationTests/*.csproj').FullName
dotnet test $projeto --filter 'FullyQualifiedName~PixInboxTests' --logger 'console;verbosity=detailed'
```

Procure **Mensagens de saída padrão**. As duas chamadas devem mostrar HTTP 200 e o **mesmo ID de liquidação**. Uma resposta indica reentrega. A consulta seguinte mostra um registro na Inbox e uma liquidação. Um outro Pix com o mesmo valor e destinatário recebe seu próprio registro, pois tem outro ID.

No segundo teste, o PostgreSQL rejeita a liquidação. O terminal mostra HTTP 500 e zero linhas nas duas tabelas. Depois de retirar a restrição de teste, a mesma mensagem pode ser aceita. A Inbox só lembra de mensagens cujo efeito foi gravado.

Abra `Data/PixInbox.cs` no projeto `PixService`. O `ON CONFLICT DO NOTHING` deixa o próprio banco decidir quem registra o ID. A mesma instrução SQL usa o ID retornado para gravar a liquidação. Quando já existe um registro, o serviço consulta e devolve a liquidação original.

Neste exercício, HTTP faz o papel do transporte e a liquidação é simulada. A etapa de RabbitMQ da trilha vai conectar os processos e permitir observar a confirmação do broker e a reentrega depois de uma queda.

::: nota Como eu explicaria isso para um tech lead?
Eu enviei duas requisições com o mesmo ID para representar uma reentrega. Usei a chave primária no PostgreSQL para resolver a disputa entre as chamadas e gravei o ID e a liquidação na mesma instrução. As duas respostas tinham o mesmo ID de liquidação e a consulta mostrou um único registro. Também provoquei uma falha e confirmei que a mensagem pôde ser processada na tentativa seguinte.
:::

::: nota Abra o serviço em um terminal e consulte o banco em outro
Na raiz do repositório, prepare as tabelas em um schema próprio e inicie o PixService no Windows. O schema manual é separado dos schemas descartáveis dos testes.

```bash
$pix = (Get-ChildItem 'src/*PixService/*.csproj').FullName
$sql = Join-Path (Split-Path $pix) 'Data/PixInbox.sql'
docker exec banco-outbox-aula psql -v ON_ERROR_STOP=1 -U postgres -d outbox_aula -c 'CREATE SCHEMA IF NOT EXISTS pix_service'
Get-Content -Raw $sql | docker exec -i -e 'PGOPTIONS=-c search_path=pix_service' banco-outbox-aula psql -v ON_ERROR_STOP=1 -U postgres -d outbox_aula
$env:ConnectionStrings__PixInbox = "$env:BANCO_TEST_POSTGRES;Search Path=pix_service"
dotnet run --project $pix --no-launch-profile -- --urls http://127.0.0.1:5056
```

Em outro PowerShell, crie um evento e envie o mesmo JSON duas vezes. Os IDs são gerados uma vez, antes dos envios.

```bash
$agora = [DateTime]::UtcNow.ToString('o')
$id = [guid]::NewGuid()
$evento = @{
    eventId = $id
    operacaoId = [guid]::NewGuid()
    contaOrigemId = [guid]::NewGuid()
    valor = 150
    chavePix = 'confeitaria@example.test'
    agendadoPara = $agora
    ocorridoEm = $agora
} | ConvertTo-Json
1..2 | ForEach-Object {
    $r = Invoke-RestMethod 'http://127.0.0.1:5056/pix/liquidar' -Method Post -ContentType 'application/json' -Body $evento
    [pscustomobject]@{ Reentrega = $r.reentrega; LiquidacaoId = $r.evento.eventId }
}
docker exec banco-outbox-aula psql -U postgres -d outbox_aula -c "SELECT evento_recebido_id FROM pix_service.pix_liquidacoes WHERE evento_recebido_id = '$id'"
```

A primeira resposta marca `False` e a segunda marca `True`. As duas trazem o mesmo ID de liquidação. A consulta feita diretamente no contêiner retorna uma linha. Para encerrar a API, use Ctrl+C no primeiro terminal.
:::

#### Como levar o mesmo comportamento para um broker

1. **Comece pelo teste.** Entregue duas vezes o mesmo evento ao consumidor. Use uma barreira no teste para simular também dois consumidores recebendo o evento ao mesmo tempo.
2. **Defina a Inbox.** Crie uma tabela local de mensagens tratadas com unicidade em `EventId` . Dentro da transação, tente inserir esse ID de forma atômica, por exemplo com `INSERT ... ON CONFLICT DO NOTHING` . Use o resultado da inserção para decidir se aplica a mudança de estado.
3. **Grave o efeito na mesma transação.** Se a primeira tentativa falhar antes do commit, o ID e a mudança de estado devem voltar juntos. Se o commit concluir e o ACK se perder, a próxima tentativa detecta o ID e pode confirmar a entrega sem repetir o efeito.
4. **Mostre a evidência.** Capture duas entregas nos logs, uma linha na Inbox e uma única transição local de estado. Em seguida envie um segundo Pix legítimo com outro ID e confirme que ele também é processado.

Se a liquidação chamar um provedor externo, a transação local da Inbox só protege os efeitos no próprio banco. Investigue como o provedor recebe uma chave de idempotência ou permite consultar e reconciliar uma tentativa ambígua.

::: nota Checkpoint · Em qual ordem eu confirmo?
Receba o evento, abra a transação, registre o ID e aplique a mudança local, confirme o banco e só então envie o ACK. Para uma duplicata cujo ID já está confirmado, encerre a tentativa e envie o ACK. Para falha transitória, combine retentativas com espera crescente e limite. Para uma mensagem inválida, defina um destino de quarentena e uma forma explícita de revisar e reprocessar.
:::

### 3. Investigue um Pix que ficou parado

Você recebeu um chamado sobre um pagamento agendado há dez minutos. Comece pelo `OperacaoId`. Confira o estado da operação, o `EventId` correspondente, a idade da mensagem e o último erro. Se a linha tiver carimbo, procure confirmação de publicação e, depois, o registro da Inbox. Use os IDs nos logs e evite registrar a chave Pix e o JSON completo.

Para acompanhar o sistema, proponha um alerta pela idade da mensagem pendente mais antiga, métricas de falhas de publicação e reentregas, e uma forma de correlacionar as etapas. Se uma dependência remota ficar instável, explique onde usar retry com jitter e onde um circuit breaker reduziria chamadas inúteis durante a falha.

**Entrega do laboratório.** Registre a previsão, o teste reproduzível, o estado observado em cada parada e uma decisão de arquitetura. Em que condições o Pix merece virar um processo separado? Que garantias e custos essa escolha muda?

```bash
# Encerre apenas o contêiner descartável criado na primeira etapa
docker stop banco-outbox-aula
Remove-Item Env:BANCO_TEST_POSTGRES
```

[Veja os próximos exercícios da trilha →](#trilha)
