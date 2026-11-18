---
id: lab-pix-aula04
order: 257
kind: lab
num: 24
group: Laboratório · Parte 41 · Banco Digital: Outbox, Inbox e Pix
title: Antes da Outbox: faça o Pix falhar
subtitle: Agende um Pix de R$ 150, derrube uma das duas gravações e compare o que ficou no banco com o que chegou à fila.
minutes: 5
hue: 20
origem: aula-guiada-monolito-gemini/site/assets/js/pix-lessons.js
---

### A encomenda que ninguém foi buscar

Você encomendou um bolo de aniversário. A confeitaria anotou o pedido no livro, mas o telefone que avisa o entregador quebrou. A loja consegue provar que aceitou o pedido. O entregador sequer sabe que existe um bolo para buscar. Agora inverta a ordem. O entregador sai com a ficha antes de o caixa registrar a encomenda. Se o caixa rejeitar o pedido, alguém pode tentar entregar um bolo que nunca foi confirmado.

::: ilustracao img/lab-pix/outbox-confeitaria.png | Comece pelo quadro 1. O livro representa o PostgreSQL e o aviso ao entregador representa a publicação. Os quadros 2 e 3 voltam nas próximas etapas.
:::

No Banco Digital, o livro guarda a operação com status `Agendado`. O aviso é o evento de agendamento. O laboratório acompanha esses registros. O agendamento deste projeto ainda não realiza débito nem liquidação.

**Dual-write** ocorre aqui quando o mesmo pedido depende de uma gravação no PostgreSQL e de outra no broker, cada uma com sua própria confirmação. Mudar a ordem muda o rastro deixado pela falha.

### 1. Prepare o laboratório

1. Abra um PowerShell na raiz do repositório. Use o SDK .NET 10 e um PostgreSQL de laboratório com permissão para criar schemas. Cada teste cria e remove um schema isolado.
2. Se já tem um banco de testes, aponte `BANCO_TEST_POSTGRES` para ele. A opção abaixo cria um banco descartável com Docker em uma porta local separada.
3. Espere o `pg_isready` indicar que o PostgreSQL aceita conexões. O broker, nesta etapa, é simulado no teste para que você controle com precisão onde ocorre a falha.

```bash
docker run --rm -d --name banco-outbox-aula -p 127.0.0.1:55432:5432 -e POSTGRES_PASSWORD=aula-local -e POSTGRES_DB=outbox_aula postgres:18
docker exec banco-outbox-aula pg_isready -U postgres -d outbox_aula
$env:BANCO_TEST_POSTGRES = 'Host=127.0.0.1;Port=55432;Database=outbox_aula;Username=postgres;Password=aula-local'
$projeto = (Get-ChildItem tests/*IntegrationTests/*.csproj).FullName
```

Use a senha do exemplo apenas no contêiner local descartável. Se a variável de conexão estiver ausente, os testes de PostgreSQL aparecem como ignorados. Mantenha o banco ligado para a etapa seguinte.

### 2. Escreva o teste com o banco primeiro

Antes de executar, anote sua previsão. Depois de confirmar a transação do Pix e antes de publicar o evento, o broker fica indisponível. Quantos pagamentos, eventos e mensagens de outbox você espera encontrar?

```csharp
await escrita.SaveChangesAsync();
await Assert.ThrowsAsync<TimeoutException>(
    () => fila.PublicarAsync(pagamento.Id));

await using var leitura = new AppDbContext(options);
Assert.True(await leitura.Pagamentos.AnyAsync(p => p.Id == pagamento.Id));
Assert.Empty(fila.OperacoesRecebidas);
Assert.False(await leitura.OutboxMessages.AnyAsync());
```

`SaveChangesAsync` confirma o pagamento antes da tentativa de publicação. A falha é injetada pelo teste, antes de a fila simulada aceitar o evento. O novo `DbContext` lê o que foi realmente persistido. Uma entidade acompanhada pelo contexto original pode existir só na memória.

Em um sistema distribuído, um timeout também pode acontecer depois que o broker aceitou o evento, com perda da resposta. Aqui escolhemos deliberadamente o caso em que a publicação nem chegou à fila. A ambiguidade da confirmação reaparece na terceira etapa.

### 3. Escreva o teste na ordem inversa

Agora a fila aceita o evento primeiro. O teste adiciona uma restrição `CHECK (false)` à tabela vazia de pagamentos do schema descartável. O PostgreSQL rejeita a próxima inserção de forma determinística. O erro de persistência não consegue retirar o evento que já foi aceito pela fila.

```csharp
await fila.PublicarAsync(pagamento.Id);
await Assert.ThrowsAsync<DbUpdateException>(() => escrita.SaveChangesAsync());

await using var leitura = new AppDbContext(options);
Assert.Equal(pagamento.Id, Assert.Single(fila.OperacoesRecebidas));
Assert.False(await leitura.Pagamentos.AnyAsync(p => p.Id == pagamento.Id));
```

Encontre os dois testes em `tests/aula-guiada-monolito-gemini.IntegrationTests/LaboratorioOutboxTests.cs`. Rode apenas os cenários de dual-write.

```bash
dotnet test $projeto --filter 'FullyQualifiedName~DualWrite_' --logger 'console;verbosity=detailed'
```

Procure o bloco `Saída padrão` ou `Standard Output` abaixo de cada teste. No primeiro, você verá o Pix que a consulta encontrou no PostgreSQL e o erro da fila simulada. No segundo, a fila aceita o aviso e o próprio PostgreSQL retorna o erro `23514`, pois rejeitou a gravação. Os testes mostram o estado lido antes de apagar as tabelas temporárias que usaram.

::: nota Confira o resultado esperado
Banco primeiro

Um pagamento, zero eventos e zero mensagens pendentes. O registro ficou sem um caminho de entrega criado por essa execução.

Fila primeiro

Zero pagamentos e um evento. O consumidor pode receber uma referência para uma operação que o banco rejeitou.

Os dois testes devem passar. Um teste verde aqui confirma que você conseguiu reproduzir a inconsistência da abordagem experimental.
:::

**Checkpoint.** Explique por que colocar `try/catch` em volta das duas chamadas não faz uma transação do PostgreSQL desfazer uma publicação que a fila já aceitou.

[Avançar para 2 · Implemente a Outbox →](#aula05)
