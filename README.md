# super-netSenior

Solução única em **.NET 10** que consolida os projetos de estudo da pasta `entrevistas`, que reimplementavam
os mesmos conceitos várias vezes, cada um do seu jeito.

> **Estado: Fase 1 concluída** — núcleo compartilhado (`BuildingBlocks`) + módulo `Banking`.
> Os projetos originais continuam intactos ao lado desta pasta, como referência.

## Estrutura

```
src/
├─ BuildingBlocks/
│  ├─ SuperNetSenior.BuildingBlocks            sem dependências de framework
│  │    Results/ (Result, Error, ErrorType) · ValueObjects/ (Cpf, Money, Email)
│  │    Messaging/ (IOutbox, IEventBus, OutboxMessage, IntegrationEventRegistry)
│  │    Persistence/ (IUnitOfWork) · Security/ (ICurrentUser)
│  └─ SuperNetSenior.BuildingBlocks.Infrastructure    EF Core
│       EfUnitOfWork · EfOutbox · OutboxDispatcher<TContext> · InMemoryEventBus
└─ Banking/
   ├─ Domain          Cliente, Usuario, Conta, LancamentoFinanceiro, Transferencia, Pix, Investimento, Seguro
   ├─ Application     use cases (Result-based) + FluentValidation
   ├─ Infrastructure  BankingDbContext (PostgreSQL), repositórios, BCrypt
   └─ Api             controllers, JWT, ProblemDetails
tests/
├─ SuperNetSenior.BuildingBlocks.Tests
└─ SuperNetSenior.Banking.Tests              domínio, use cases, mapeamento EF, JWT e pipeline HTTP real
```

Dependências sempre apontam para dentro: `Api → Infrastructure → Application → Domain → BuildingBlocks`.
Versões de pacote ficam em um só lugar: `Directory.Packages.props`.

## De onde veio cada coisa

| Conceito | Antes | Agora |
|---|---|---|
| `Cpf` | monolito: só conferia o tamanho · BankMore: validava tudo, mas lançava exceção | **um só**, com dígitos verificadores e `Result` |
| `Money`, `Email`, `Result`/`Error` | cópias mínimas e divergentes | **um só** em `BuildingBlocks` |
| Outbox | só no monolito, com o tipo de evento fixo no código | genérico (`OutboxDispatcher<TContext>`), qualquer módulo reaproveita |
| Idempotência | BankMore: tabela própria · microservico-senior: `IMemoryCache` | índice único `(conta, chave)`; reenvio devolve o mesmo resultado |
| Dono da conta | BankMore: comparava o **nome** do usuário | FK real `Usuario → Cliente`; regra `Conta.ClienteId == cliente_id` do JWT |
| Atomicidade da transferência | 4 `SaveChanges` soltos | repositórios só rastreiam; **um** `CommitAsync` + `Conta.Versao` (concorrência otimista) |
| Login | mensagens diferentes para "não existe" e "senha errada" | uma mensagem só (sem enumeração de contas) |
| Token JWT | levava CPF e e-mail | só `sub`, `cliente_id` e `jti` |
| Chave JWT | no `appsettings.json` versionado | só por ambiente (`Jwt__Key`); a API **não sobe** com chave ausente ou fraca |

## Como rodar

Pré-requisitos: .NET SDK 10 e PostgreSQL.

```bash
dotnet test super-netSenior.slnx
```

Os testes não precisam de banco. Para subir a API em desenvolvimento, ajuste a connection string em
`src/Banking/SuperNetSenior.Banking.Api/appsettings.Development.json` (a chave JWT daquele arquivo é **só de desenvolvimento**) e rode:

```bash
dotnet run --project src/Banking/SuperNetSenior.Banking.Api
```

Em Development o banco é criado com `EnsureCreated`. Para outros ambientes, use migrations e defina `Jwt__Key`
e `ConnectionStrings__Banking` por variável de ambiente.

## Endpoints

| Método | Rota | Acesso |
|---|---|---|
| POST | `/api/auth/registrar`, `/api/auth/login` | anônimo |
| POST | `/api/contas` | autenticado (titular = usuário logado) |
| GET | `/api/contas/{id}`, `/api/contas/{id}/saldo` | só o dono |
| POST | `/api/contas/{id}/movimentos` | só o dono (depósito/saque idempotente) |
| POST | `/api/transferencias` | só o dono da origem (idempotente) |
| POST | `/api/pagamentos/pix` | só o dono (grava pagamento + evento na Outbox) |
| POST | `/api/investimentos/resgatar` | só o dono da conta da posição |

## Pendências conhecidas

- **Migrations EF** ainda não geradas (Development usa `EnsureCreated`).
- **Resgate de investimento** reduz a posição, mas ainda não credita o valor na conta (igual ao monolito original).
- **Inbox / `PixService`** (consumidor idempotente do evento Pix) do monolito ainda não foi trazido.
- **Concorrência**: conflito de versão em `Conta` vira HTTP 409; o cliente deve repetir a chamada (não há retry automático).
- Não testado de ponta a ponta contra PostgreSQL real nesta fase (o mapeamento e as consultas são validados sem conexão).

## Próximas fases

Para estudar para entrevistas, veja o [roteiro de cenários do Banking](entrevista/README.md).

2. **Serviços e gateway** — Orders/Saga e Circuit Breaker (microservico-senior), Stock e Notification, YARP, OpenTelemetry.
3. **Plataforma** — `docker-compose.yml` único, CI.
4. **Portal de ensino único** — substituir os cinco front-ends e o guia de 21 módulos por um só leitor.
"# super-netSenior" 
"# super-netSenio" 
"# super-netSenior" 
"# super-netSenio" 
