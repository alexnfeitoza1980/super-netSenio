---
id: lab-sns-arquitetura
order: 272
kind: lab
num: 39
group: Laboratório · Parte 45 · super-netSenior · da repetição à solução única
title: Banking por dentro: camadas, atomicidade e idempotência
subtitle: Como o módulo Banking garante que uma transferência grave tudo ou nada, nunca gaste o mesmo saldo duas vezes e só seja feita pelo dono da conta.
minutes: 35
hue: 200
objectives:
- Ler a estrutura em camadas e a **regra de dependência** do `super-netSenior`
- Entender por que um **único commit** torna a transferência atômica
- Explicar como `Conta.Versao` evita gasto duplo e como o índice único garante a idempotência
- Aplicar a regra "só o dono opera a conta" com a FK `Usuario → Cliente`
origem: livro/tools/conteudo/sns-02-arquitetura.md
---

## A estrutura

```text
src/
├─ BuildingBlocks/
│  ├─ SuperNetSenior.BuildingBlocks            Result/Error, Cpf, Money, Email, contratos de Outbox e UnitOfWork
│  └─ SuperNetSenior.BuildingBlocks.Infrastructure   EF Core: EfUnitOfWork, EfOutbox, OutboxDispatcher<TContext>
└─ Banking/
   ├─ Domain          Cliente, Usuario, Conta, LancamentoFinanceiro, Transferencia, Pix, Investimento
   ├─ Application     use cases (retornam Result) + FluentValidation
   ├─ Infrastructure  BankingDbContext (PostgreSQL), repositórios, BCrypt
   └─ Api             controllers, JWT, ProblemDetails
```

::: regra Dependências sempre para dentro
`Api → Infrastructure → Application → Domain → BuildingBlocks`. O domínio não conhece EF, ASP.NET nem JWT; quem precisa deles implementa interfaces definidas mais para dentro.
:::

## Um commit, uma transação

Nos projetos de origem, a transferência fazia **quatro** `SaveChanges` soltos (débito, crédito, a transferência e a chave de idempotência): se o segundo falhasse, havia débito sem crédito. No `Banking`, os repositórios **só rastreiam** mudanças e o use case chama `CommitAsync` **uma vez**:

```csharp TransferirUseCase (o essencial)
var debito = origem.Value.Debitar(request.Valor);
if (debito.IsFailure) return Result.Failure<TransferenciaDto>(debito.Error);

var credito = destino.Creditar(request.Valor);
if (credito.IsFailure) return Result.Failure<TransferenciaDto>(credito.Error);

await lancamentos.AdicionarAsync(lancDebito.Value, cancellationToken);
await lancamentos.AdicionarAsync(lancCredito.Value, cancellationToken);
await transferencias.AdicionarAsync(transferencia.Value, cancellationToken);

// Débito, crédito, lançamentos e transferência gravam juntos ou nada.
await unitOfWork.CommitAsync(cancellationToken);
```

::: dica Valide o destino antes de mexer na origem
O use case checa se a conta de destino está **ativa** antes de chamar `Debitar`. Assim, se algo falhar, **nenhum** saldo foi alterado em memória.
:::

## Gasto duplo: concorrência otimista

Duas transferências simultâneas lendo o mesmo saldo poderiam gastar o mesmo dinheiro. A entidade carrega um **token de concorrência**:

```csharp Conta
public int Versao { get; private set; }

public Result Debitar(decimal valor)
{
    var validacao = ValidarMovimento(valor);
    if (validacao.IsFailure) return validacao;

    if (Saldo < valor)
        return Result.Failure(Error.Validation("Conta.SaldoInsuficiente",
            $"Saldo insuficiente. Saldo atual: {Saldo:N2}, valor solicitado: {valor:N2}."));

    Saldo -= valor;
    Versao++;
    return Result.Success();
}
```

```csharp Mapeamento (EF Core)
e.Property(x => x.Versao).IsConcurrencyToken();
e.HasIndex(x => new { x.Agencia, x.Numero }).IsUnique();
```

O `UPDATE` do EF inclui `WHERE versao = <valor lido>`. Se outra operação alterou a conta no meio, nenhuma linha é afetada, o EF lança `DbUpdateConcurrencyException` e a API responde **HTTP 409**. O cliente repete a chamada.

::: cuidado Sem retry automático
Um conflito de versão **não** é repetido pelo servidor. Sob carga, parte das transferências simultâneas na mesma conta retornará 409. É uma decisão consciente: evita transações Serializable (que geram falhas de serialização e exigem retry) ao custo de o cliente repetir.
:::

## Idempotência por índice único

A chave de idempotência vem do cliente. Dois índices únicos tornam a duplicidade impossível mesmo com requisições paralelas:

| Tabela | Índice único | Efeito |
|---|---|---|
| `transferencias` | `(ContaOrigemId, ChaveIdempotencia)` | A mesma chave não cria duas transferências |
| `lancamentos_financeiros` | `(ContaId, ChaveIdempotencia)` | Os lançamentos derivados (`chave:D` e `chave:C`) não duplicam |

O comportamento do reenvio é deliberado: com a **mesma chave e a mesma operação**, o use case devolve **a mesma transferência** (200), em vez de um erro. Com a mesma chave e **outro valor ou destino**, devolve **409 Conflict**.

## Só o dono opera a conta

```csharp Conta
public bool PertenceA(Guid clienteId) => clienteId != Guid.Empty && ClienteId == clienteId;
```

O JWT leva só identificadores (`sub`, `cliente_id`, `jti`). Todo use case começa com `Guard.ClienteAutenticado` e usa `Guard.ContaDoCliente`, que devolve **404** se a conta não existe e **403** se existe mas é de outro cliente. No BankMore original o vínculo usuário → conta era feito pelo **nome**; aqui é uma **chave estrangeira** real.

::: exercicio Quebre de propósito
1. Em `Conta.PertenceA`, troque o corpo por `true` e rode os testes. Quantos falham?
2. Remova o `IsConcurrencyToken()` e descreva um cenário que passa a gastar o mesmo saldo duas vezes.
3. Troque a chave derivada `chave:D` por um `Guid.NewGuid()` a cada chamada. O que deixa de ser garantido num reenvio?
:::

::: resumo
- **Um `CommitAsync`** por operação de negócio: repositórios só rastreiam mudanças.
- **`Conta.Versao`** como token de concorrência barra o gasto duplo; conflito vira **409**.
- **Índice único** `(conta, chave)` garante idempotência no banco, não só no código.
- **FK `Usuario → Cliente`** e `Conta.PertenceA` fazem a autorização; 404 para inexistente e 403 para conta alheia.
:::

::: quiz
? Por que a transferência deixou de ter "débito sem crédito" no `Banking`?
- Porque cada repositório faz o seu próprio SaveChanges mais rápido
+ Porque os repositórios apenas rastreiam mudanças e um único CommitAsync grava tudo numa só transação
- Porque o banco desfaz sozinho qualquer erro de negócio
> Com um único commit, a gravação é atômica: ou todas as mudanças são persistidas, ou nenhuma.

? O que acontece quando duas transferências alteram a mesma conta ao mesmo tempo?
+ Uma conclui; a outra recebe um conflito de concorrência (HTTP 409) e o cliente precisa repetir
- As duas concluem e o saldo fica negativo
- O servidor repete a segunda automaticamente até conseguir
> O token `Versao` faz o UPDATE da segunda falhar. Não há retry automático no servidor.

? Qual a diferença entre responder 403 e 404 para uma conta?
- Nenhuma: são intercambiáveis
+ 404 quando a conta não existe; 403 quando existe mas pertence a outro cliente
- 403 quando o token expirou
> Token inválido ou ausente é 401. 403 indica autenticado, porém sem permissão sobre aquele recurso.
:::

::: cartas
Conta.Versao :: Token de concorrência otimista; impede dois updates sobre a mesma leitura do saldo.
IUnitOfWork :: Contrato com CommitAsync: um único commit grava a operação inteira.
Chave de idempotência :: Valor enviado pelo cliente; com índice único, o reenvio não duplica a operação.
Guard.ContaDoCliente :: Devolve 404 se a conta não existe e 403 se é de outro cliente.
:::
