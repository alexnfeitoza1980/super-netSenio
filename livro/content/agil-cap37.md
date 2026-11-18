---
id: agil-cap37
order: 54
kind: capitulo
num: 37
group: C# · Parte 11 · Pacotes, padrões e arquitetura
title: Proxy e Gateway
subtitle: Cruzando barreiras (banco de dados, rede, APIs de terceiros) sem contaminar as regras de negócio — o Proxy completo, o Table Data Gateway mais leve e quando usar cada um.
minutes: 60
hue: 165
objectives:
- Reconhecer as **barreiras** (banco, rede, API de terceiros) e o dano de misturá-las às regras de negócio
- Entender a estrutura do **Proxy** (interface + implementação + proxy) e suas dificuldades reais
- Conhecer os **tipos de proxy** (virtual/lazy, proteção, remoto, cache, log)
- Aplicar **Table Data Gateway / Repository** (com versão em memória para testes) e decidir quando o Proxy compensa
origem: livros/02-projeto-agil-csharp/content/53-cap37.md
---

::: ilustracao img/agil/cap37.svg | Proxy controla o acesso ao objeto real; Gateway esconde o mundo externo atrás de uma porta.
:::

## Barreiras em sistemas de software

Há muitas **barreiras** em sistemas: ao mover dados do programa para o **banco**, cruzamos a barreira do banco; ao enviar uma mensagem entre computadores, a **barreira da rede**. Cruzá-las é complicado e, se não tomarmos cuidado, o software acaba **mais relacionado às barreiras do que ao problema** que deveria resolver. O padrão **Proxy** ajuda a cruzá-las **mantendo o programa centrado no problema**.

## O problema: um carrinho de compras

Sistema de loja virtual: objetos `Cliente`, `Pedido` (o carrinho), `Item` e `Produto`.

```cs
public class Pedido
{
    private readonly List<Item> itens = new List<Item>();

    public void AdicionarItem(Produto produto, int quantidade)
    {
        itens.Add(new Item(produto, quantidade));
    }

    public decimal Total { get { return itens.Sum(i => i.Produto.Preco * i.Quantidade); } }
}
```

Agora imagine que esses dados vivem num **banco relacional**: tabelas `Clientes`, `Pedidos`, `Itens`, `Produtos` ligadas por chaves (`ClienteId`, `PedidoId`, `Sku`). Adicionar um item **no banco** seria:

```cs
public class AdicionarItemTransacao
{
    public void Executar(SqlConnection conexao, int pedidoId, string sku, int quantidade)
    {
        var cmd = new SqlCommand("INSERT INTO Itens(PedidoId, Sku, Quantidade) VALUES (@p, @s, @q)", conexao);
        cmd.Parameters.AddWithValue("@p", pedidoId);
        cmd.Parameters.AddWithValue("@s", sku);
        cmd.Parameters.AddWithValue("@q", quantidade);
        cmd.ExecuteNonQuery();
    }
}
```

Os dois trechos executam **a mesma função lógica** — ligar um item a um pedido —, mas o primeiro **ignora que existe um banco**; o segundo **se orgulha dele**. O programa de carrinho trata de pedidos, itens e produtos; se usarmos o segundo, passa a tratar de **SQL, conexões e strings de consulta**. É uma violação grave do **SRP** (e do CCP): misturam-se dois conceitos que mudam por motivos diferentes — o de **itens e pedidos** e o de **esquema relacional**. E viola o **DIP**: a política depende dos detalhes do armazenamento.

## O padrão Proxy

O Proxy cura esses males. Cada objeto "proxificado" é dividido em **três partes**:

1. Uma **interface** com os métodos que os clientes chamam.
2. Uma **implementação** dessa interface **sem conhecimento do banco**.
3. O **proxy**, que implementa a interface **conhecendo o banco**: busca os dados, cria a implementação e **delega** a ela.

::: figura Estrutura estática do Proxy
<svg viewBox="0 0 720 220" role="img" aria-label="Cliente usa IProduto; ProdutoImpl e ProdutoProxy implementam; o proxy usa o banco">
  <rect class="f-prim" x="10" y="80" width="130" height="50" rx="10"/><text x="75" y="110" text-anchor="middle" font-weight="800">Cliente</text>
  <path class="arrow" d="M142 105H230"/>
  <rect class="f-info" x="232" y="78" width="160" height="54" rx="10"/><text x="312" y="98" text-anchor="middle" font-size="10" class="t-dim">«interface»</text><text x="312" y="118" text-anchor="middle" font-weight="800" class="t-info">IProduto</text>
  <path class="line" d="M312 134V160M200 160H430M200 160V176M430 160V176" style="marker-end:none"/>
  <rect class="f-good" x="110" y="176" width="180" height="38" rx="8"/><text x="200" y="200" text-anchor="middle" font-size="13">ProdutoImpl (sem banco)</text>
  <rect class="f-warn" x="350" y="176" width="160" height="38" rx="8"/><text x="430" y="200" text-anchor="middle" font-size="13" font-weight="700">ProdutoProxy</text>
  <path class="arrow" d="M350 195H292" style="stroke:var(--warn);marker-end:url(#arr)"/><text x="321" y="188" text-anchor="middle" font-size="10" class="t-dim">delega</text>
  <path class="arrow" d="M512 195H570"/><rect class="f-plain" x="572" y="170" width="130" height="50" rx="8"/><text x="637" y="200" text-anchor="middle">Banco de dados</text>
</svg>
:::

::: figura Estrutura dinâmica
<svg viewBox="0 0 720 170" role="img" aria-label="Cliente pede Preco; proxy busca no banco, cria a implementação e delega">
  <g text-anchor="middle" font-size="12"><rect class="f-prim" x="10" y="10" width="100" height="30" rx="4"/><text x="60" y="30">cliente</text><rect class="f-warn" x="190" y="10" width="130" height="30" rx="4"/><text x="255" y="30">: ProdutoProxy</text><rect class="f-plain" x="400" y="10" width="100" height="30" rx="4"/><text x="450" y="30">Banco</text><rect class="f-good" x="570" y="10" width="130" height="30" rx="4"/><text x="635" y="30">: ProdutoImpl</text></g>
  <path d="M60 42V160M255 42V160M450 42V160M635 42V160" stroke="var(--ink-3)" stroke-dasharray="5 4"/>
  <path class="arrow" d="M62 62H252"/><text x="157" y="55" text-anchor="middle" font-size="11" class="t-mono">1: Preco</text>
  <path class="arrow" d="M257 84H448"/><text x="352" y="77" text-anchor="middle" font-size="11" class="t-mono">2: Buscar(sku)</text>
  <path class="arrow" d="M257 108H632"/><text x="445" y="101" text-anchor="middle" font-size="11" class="t-mono">3: new ProdutoImpl(dados) → Preco</text>
  <path d="M632 132H257" stroke="var(--ink-2)" stroke-dasharray="5 4" fill="none" marker-end="url(#arr2)"/><path d="M252 150H62" stroke="var(--ink-2)" stroke-dasharray="5 4" fill="none" marker-end="url(#arr2)"/>
</svg>
:::

O cliente envia `Preco` ao que **acha** ser um produto — mas é um `ProdutoProxy`. O proxy **busca** os dados no banco, cria um `ProdutoImpl` e **delega** `Preco` a ele. **Nem o cliente nem `ProdutoImpl` sabem** que isso aconteceu: o banco foi **inserido** na aplicação sem que nenhuma das partes soubesse. Em teoria, um proxy pode ser posto **entre dois colaboradores** sem que percebam — e assim atravessar um banco ou uma rede.

```cs
public interface IProduto { string Sku { get; } string Nome { get; } decimal Preco { get; } }

public class ProdutoImpl : IProduto                    // regras de negócio, zero banco
{
    public string Sku { get; } public string Nome { get; } public decimal Preco { get; }
    public ProdutoImpl(string sku, string nome, decimal preco) { Sku = sku; Nome = nome; Preco = preco; }
}

public class ProdutoProxy : IProduto                   // conhece o banco
{
    private readonly string sku;
    private readonly BancoDeProdutos banco;
    private ProdutoImpl real;                          // carregado sob demanda (lazy)

    public ProdutoProxy(string sku, BancoDeProdutos banco) { this.sku = sku; this.banco = banco; }

    private ProdutoImpl Real
    {
        get
        {
            if (real == null)
            {
                DadosDeProduto d = banco.Buscar(sku);                 // cruza a barreira só quando preciso
                real = new ProdutoImpl(d.Sku, d.Nome, d.Preco);
            }
            return real;
        }
    }

    public string Sku { get { return sku; } }                         // não precisa ir ao banco: já sabemos
    public string Nome { get { return Real.Nome; } }
    public decimal Preco { get { return Real.Preco; } }
}
```

Observe: `Sku` é **curto-circuitado** — o proxy já o tem; delegar custaria uma ida ao banco à toa.

### Na prática, o proxy não é trivial

Quando tentamos proxificar também `Pedido` e `Item`, o modelo "puro" de delegação raramente fica tão harmonioso:

- Muitos `get`/`set` triviais viram **curto-circuitos** (o proxy responde sem delegar).
- Para relacionamentos **1:N** (pedido → itens), a delegação é **adiada**: o `PedidoProxy.AdicionarItem` pode **gravar no banco**, e só `Total` **carrega** o pedido real com todos os itens (e cada item aponta para `ProdutoProxy`).
- Surge o **fantasma do cache**.

```cs
public class PedidoProxy : IPedido
{
    private readonly int pedidoId; private readonly IBanco banco;
    public PedidoProxy(int pedidoId, IBanco banco) { this.pedidoId = pedidoId; this.banco = banco; }

    public void AdicionarItem(IProduto produto, int quantidade)
    {
        banco.InserirItem(pedidoId, produto.Sku, quantidade);        // persiste direto; não carrega nada
    }

    public decimal Total
    {
        get
        {
            var real = new PedidoImpl();
            foreach (var linha in banco.ItensDoPedido(pedidoId))     // só agora lê os itens
                real.AdicionarItem(new ProdutoProxy(linha.Sku, banco), linha.Quantidade);
            return real.Total;                                       // delega ao objeto de negócio puro
        }
    }
}
```

::: cuidado Cuidado com o cache prematuro
Não adicione **cache** automaticamente por medo de lentidão. **Adicionar cache cedo demais é uma boa maneira de piorar o desempenho** (e complicar a correção). Se o desempenho preocupa, **meça**; só quando provado o problema, pense em como acelerar.
:::

### A vantagem poderosa: separação de preocupações

Apesar do trabalho, o resultado é uma **separação completa**: `PedidoImpl` e `ProdutoImpl` **não dependem do banco**; podemos trocar o esquema ou o mecanismo de armazenamento **sem tocar** nas classes de negócio. O Proxy serve para **separar regras de negócio de qualquer problema de implementação** (banco, rede, middleware) — impedir que regras fiquem poluídas com detalhes de tecnologia.

## Tipos de proxy que você encontra

| Tipo | O que faz | Exemplo |
|---|---|---|
| **Virtual / lazy** | Cria/carrega o objeto real **só quando preciso** | `ProdutoProxy` acima; *lazy loading* do Entity Framework |
| **Remoto** | Representa um objeto **em outro processo/máquina** | Clientes gerados de **gRPC/WCF/Refit/HttpClient**; `IServicoDePedidos` que fala HTTP |
| **De proteção** | Controla **acesso** (permissões) | Um proxy que valida o papel do usuário antes de chamar o real |
| **Cache** | Guarda resultados já obtidos | `ProxyDeCotacao` que reutiliza o valor por 30 s |
| **De registro/medição** | Loga ou mede cada chamada | Decoradores de logging, *interceptors* |

No .NET existem recursos para criar proxies dinamicamente: `System.Reflection.DispatchProxy`, **Castle DynamicProxy** (usado por EF e muitos contêineres de DI), e *interceptors* do gRPC/ASP.NET.

```cs
// Proxy de proteção + log em poucas linhas (mesma interface do real):
public class RelatorioProtegido : IRelatorio
{
    private readonly IRelatorio real; private readonly IUsuarioAtual usuario; private readonly ILogger log;
    public RelatorioProtegido(IRelatorio real, IUsuarioAtual usuario, ILogger log) { this.real = real; this.usuario = usuario; this.log = log; }

    public string Gerar()
    {
        if (!usuario.TemPapel("Gerente")) throw new UnauthorizedAccessException();
        log.Info("Gerando relatório para " + usuario.Nome);
        return real.Gerar();                       // delega
    }
}
```

::: pense Proxy × Decorator × Adapter
Os três "embrulham" um objeto. **Proxy**: **mesma interface**, controla **o acesso** ao real (carregar, proteger, tornar remoto). **Decorator**: mesma interface, **acrescenta comportamento**. **Adapter**: **interface diferente**, converte. A estrutura é parecida; a **intenção** diferencia.
:::

## APIs de terceiros: bancos, middleware e bibliotecas

APIs de terceiros são comuns: compramos bancos de dados, middleware, bibliotecas de classes, bibliotecas de threads… No começo, chamamos essas APIs **direto** do código de aplicação. Com o tempo, o código vai ficando **poluído**: strings SQL misturadas às regras de negócio. Quando a API ou o **esquema** muda, **mais e mais** código precisa ser revisto.

::: figura Isolando a API de terceiros
<svg viewBox="0 0 760 200" role="img" aria-label="Aplicação chama a API; depois chama uma camada que chama a API; depois a camada depende da aplicação">
  <text x="100" y="16" text-anchor="middle" font-weight="800" class="t-bad">1 · Chamadas diretas</text>
  <rect class="f-prim" x="40" y="30" width="120" height="40" rx="6"/><text x="100" y="55" text-anchor="middle">Aplicação</text><rect class="f-plain" x="40" y="120" width="120" height="40" rx="6"/><text x="100" y="145" text-anchor="middle">API</text><path class="arrow" d="M100 72V118"/>
  <text x="380" y="16" text-anchor="middle" font-weight="800" class="t-warn">2 · Camada de isolamento</text>
  <rect class="f-prim" x="320" y="30" width="120" height="40" rx="6"/><text x="380" y="55" text-anchor="middle">Aplicação</text><rect class="f-warn" x="320" y="85" width="120" height="34" rx="6"/><text x="380" y="107" text-anchor="middle">Camada</text><rect class="f-plain" x="320" y="135" width="120" height="34" rx="6"/><text x="380" y="157" text-anchor="middle">API</text><path class="arrow" d="M380 72V83"/><path class="arrow" d="M380 121V133"/>
  <text x="650" y="16" text-anchor="middle" font-weight="800" class="t-good">3 · Dependência invertida</text>
  <rect class="f-prim" x="590" y="30" width="120" height="40" rx="6"/><text x="650" y="55" text-anchor="middle">Aplicação</text><rect class="f-warn" x="590" y="85" width="120" height="34" rx="6"/><text x="650" y="107" text-anchor="middle">Camada</text><rect class="f-plain" x="590" y="135" width="120" height="34" rx="6"/><text x="650" y="157" text-anchor="middle">API</text><path class="arrow" d="M650 83V72" style="stroke:var(--good);marker-end:url(#arr)"/><path class="arrow" d="M650 121V133"/>
</svg>
:::

Em algum momento os desenvolvedores criam uma **camada** que concentra todo o código que usa a API e todos os conceitos a ela relacionados. Camadas assim podem até ser **compradas** (o próprio ADO.NET separa o aplicativo do mecanismo de banco) — mas elas **também são APIs de terceiros**, e a dependência indireta ainda pode doer: o ADO.NET **não** isola do esquema.

Para isolamento maior, **inverte-se a dependência** entre aplicação e camada: a aplicação **não sabe nada** da API, nem direta nem indiretamente. É exatamente o que o **Proxy** faz: **a aplicação não depende dos proxies; os proxies dependem da aplicação e da API**. Todo o conhecimento do mapeamento fica nos proxies.

::: regra Proxies são pesadelos — e é bom saber onde eles moram
Essa **concentração de conhecimento** significa que **os proxies são pesadelos**: mudou a API ou o esquema, mudam; mudou a aplicação, mudam. Mas "é bom saber onde estão seus pesadelos": sem proxies, eles se espalhariam **por todo o código** da aplicação.
:::

## Table Data Gateway (Gateway de Dados de Tabela)

**O Proxy é difícil de usar e é excessivo para a maioria dos aplicativos.** Ele só se justifica se você estiver convencido de que precisa de **separação absoluta** entre regras de negócio e esquema — tipicamente em **sistemas muito grandes** que sofrem **turbulência frequente** de esquema/API, ou que precisam rodar sobre **vários** bancos/middlewares. Normalmente algum acoplamento com o esquema é **tolerável**.

O **Table Data Gateway** (também **DAO**, hoje quase um sinônimo de **Repository**) costuma dar **separação suficiente sem o custo do Proxy**. É uma **fachada** (Façade) especializada **por tipo de objeto** a persistir.

```cs
// O aplicativo só conhece estas interfaces
public interface IGatewayDePedidos
{
    void Inserir(Pedido pedido);
    Pedido Buscar(int id);
}

public interface IGatewayDeProdutos
{
    void Inserir(Produto produto);
    Produto Buscar(string sku);
}

// Implementação com banco
public class GatewayDePedidosSql : IGatewayDePedidos
{
    private readonly SqlConnection conexao; private readonly IGatewayDeProdutos produtos;
    public GatewayDePedidosSql(SqlConnection conexao, IGatewayDeProdutos produtos) { this.conexao = conexao; this.produtos = produtos; }

    public void Inserir(Pedido pedido)
    {
        /* INSERT em Pedidos + INSERT em Itens (um loop) — o gateway cuida das duas tabelas */
    }

    public Pedido Buscar(int id)
    {
        var pedido = new Pedido(id);
        /* SELECT em Pedidos; SELECT em Itens; para cada linha: pedido.AdicionarItem(produtos.Buscar(sku), qtd) */
        return pedido;
    }
}

// Implementação em memória — "estúpida", mas inestimável nos testes
public class GatewayDePedidosEmMemoria : IGatewayDePedidos
{
    private readonly Dictionary<int, Pedido> dados = new Dictionary<int, Pedido>();
    private int proximoId = 1;
    public void Inserir(Pedido pedido) { dados[proximoId++] = pedido; }
    public Pedido Buscar(int id) { return dados.TryGetValue(id, out var p) ? p : null; }
}
```

Algumas decisões do exemplo:

- Não criamos `IGatewayDeItens`: o aplicativo **não se interessa por itens fora do contexto de um pedido**; o gateway de pedidos cuida das **duas tabelas**.
- `Pedido`, `Produto` e `Item` são **objetos simples** (DTOs/entidades), **sem** SQL.
- A implementação **em memória** guarda dados **só na memória** — "parece estúpido", mas permite **testar** toda a lógica **sem banco**: o DIP em ação.

::: figura Table Data Gateway
<svg viewBox="0 0 720 200" role="img" aria-label="Aplicação usa IGatewayDePedidos; implementação SQL e em memória">
  <rect class="f-prim" x="10" y="70" width="130" height="50" rx="10"/><text x="75" y="100" text-anchor="middle" font-weight="800">Aplicação</text>
  <path class="arrow" d="M142 95H230"/>
  <rect class="f-info" x="232" y="68" width="190" height="54" rx="10"/><text x="327" y="88" text-anchor="middle" font-size="10" class="t-dim">«interface»</text><text x="327" y="108" text-anchor="middle" font-weight="800" class="t-info">IGatewayDePedidos</text>
  <path class="line" d="M327 124V148M200 148H470M200 148V162M470 148V162" style="marker-end:none"/>
  <rect class="f-good" x="110" y="162" width="180" height="34" rx="8"/><text x="200" y="184" text-anchor="middle" font-size="13">GatewayDePedidosSql</text>
  <rect class="f-plain" x="370" y="162" width="200" height="34" rx="8"/><text x="470" y="184" text-anchor="middle" font-size="13">GatewayDePedidosEmMemoria</text>
  <rect class="f-plain" x="4" y="162" width="92" height="34" rx="8"/><text x="50" y="184" text-anchor="middle">Banco SQL</text><path class="arrow" d="M108 179H98"/>
</svg>
:::

### Proxy × Gateway: quando cada um?

| | **Proxy** | **Table Data Gateway / Repository** |
|---|---|---|
| Objetos de negócio | **Não sabem nada** do armazenamento (separação absoluta) | Também não conhecem SQL, mas **a aplicação chama o gateway** explicitamente |
| Transparência | **Total**: o cliente usa o proxy como se fosse o objeto real | Parcial: o cliente sabe que "busca" e "insere" |
| Complexidade | **Alta** (delegação adiada, 1:N, cache) | **Baixa a média** |
| Quando | Sistemas **enormes**, esquema/API instáveis, múltiplos bancos | **A maioria** dos aplicativos |

### E hoje? ORMs e micro-ORMs

Na prática moderna você raramente escreve esses proxies **à mão**:

- **Entity Framework Core** (ORM): mapeia entidades a tabelas; o `DbContext` é um *Unit of Work* e os `DbSet<T>` agem como repositórios; **proxies de lazy loading** são gerados dinamicamente (Castle).
- **Dapper** e outros **micro-ORMs**: SQL escrito por você, mapeamento automático — combinam bem com o **Repository** (o gateway).
- **Repository** sobre EF Core: ainda é útil para **isolar a persistência** e permitir **versões em memória** para testes (`InMemory` provider).

```cs
public interface IRepositorioDePedidos
{
    Task<Pedido> ObterAsync(int id);
    Task AdicionarAsync(Pedido pedido);
}

public class RepositorioDePedidosEf : IRepositorioDePedidos
{
    private readonly LojaContext db;
    public RepositorioDePedidosEf(LojaContext db) { this.db = db; }
    public Task<Pedido> ObterAsync(int id) { return db.Pedidos.Include(p => p.Itens).SingleOrDefaultAsync(p => p.Id == id); }
    public async Task AdicionarAsync(Pedido pedido) { db.Pedidos.Add(pedido); await db.SaveChangesAsync(); }
}
```

::: dica Teste o gateway "de verdade" também
Os gateways em memória permitem testar a **lógica**; mas o gateway **SQL** precisa de **testes de integração** contra um banco real (ou contêiner). Combine os dois: muitos testes rápidos em memória, poucos testes de integração para o mapeamento.
:::

::: exercicio Escolha a abordagem
1. Um sistema pequeno (3 tabelas) que só roda em SQL Server, esquema estável.
2. Uma plataforma que precisa rodar sobre SQL Server, Oracle **e** serviços REST de terceiros, com regras de negócio ricas e esquemas que mudam todo trimestre.
3. Um relatório que consulta um serviço externo lento e quer evitar chamadas repetidas.

--- resposta
1. **Table Data Gateway / Repository** (ou diretamente EF Core). Proxy seria excessivo.
2. Aqui a **separação absoluta** se justifica: **Proxy/ORM com mapeamento isolado** (ou uma camada de **adaptadores** por fonte), mantendo o domínio puro; é o "pesadelo concentrado".
3. Um **proxy de cache** (mesma interface do serviço, guardando o resultado por um tempo) — ou `IMemoryCache`/`HybridCache` por trás de uma interface.
:::

::: resumo
- Barreiras (banco, rede, API de terceiros) **não devem contaminar** as regras de negócio (SRP, CCP, DIP).
- **Proxy:** interface + implementação sem barreira + proxy que cruza a barreira e **delega**; cliente e objeto real **nem sabem**.
- Na prática: curto-circuitos de get/set, delegação adiada em 1:N, tentação de cache — **meça antes de otimizar**.
- Tipos: **virtual/lazy**, **remoto**, **de proteção**, **cache**, **log**. Proxy ≠ Decorator ≠ Adapter (intenção diferente).
- **Camada de isolamento invertida**: a aplicação não depende da API; os proxies dependem de ambos — são "pesadelos", mas **concentrados**.
- **Table Data Gateway / Repository**: separação suficiente, custo menor; ofereça versão **em memória** para testes. Use Proxy só quando precisar de separação absoluta.
- Hoje: **EF Core/Dapper** + repositório fazem o trabalho pesado.
:::

::: quiz
? Que problema o Proxy resolve em um sistema com banco de dados?
+ Cruzar a barreira do banco sem misturar SQL às regras de negócio
- Criar objetos sem new
- Garantir uma única instância
- Observar eventos
> O proxy conhece o banco; o objeto de negócio não.

? Quais são as três partes de um objeto proxificado?
+ Interface, implementação sem conhecimento do banco e proxy que conhece o banco
- Classe, subclasse e interface
- Controller, view e model
- Fábrica, produto e cliente
> O proxy cria/usa a implementação e delega a ela.

? Por que o proxy "não é trivial" na prática?
+ Há curto-circuitos de get/set, delegação adiada em relações 1:N e a tentação do cache
- Porque C# não permite proxies
- Porque exige reflexão obrigatória
- Porque não pode usar interfaces
> A delegação pura raramente se materializa tão harmoniosamente.

? Qual a recomendação sobre cache?
+ Não adicionar cache automaticamente; medir e só então otimizar
- Sempre usar cache em todo proxy
- Nunca usar cache
- Usar cache só em testes
> Cache prematuro pode piorar o desempenho e a correção.

? Qual é o tipo de proxy que carrega o objeto real apenas quando necessário?
+ Proxy virtual (lazy)
- Proxy de proteção
- Proxy remoto
- Proxy de registro
> Exemplo: lazy loading de relacionamentos no ORM.

? Para a maioria dos aplicativos, qual alternativa mais leve ao Proxy é recomendada?
+ Table Data Gateway / Repository
- Singleton
- Observer
- Visitor
> Fornece separação suficiente sem o custo do proxy.

? Por que oferecer um gateway "em memória"?
+ Para testar a lógica de negócio sem banco de dados real
- Para substituir o banco em produção
- Para evitar interfaces
- Para economizar memória
> É inestimável nos testes (DIP em ação).
:::

::: cartas
Proxy :: Objeto com a mesma interface do real que controla o acesso (carrega, protege, cruza rede/banco).
Proxy virtual (lazy) :: Cria/carrega o objeto real só quando necessário.
Proxy remoto :: Representa um objeto que vive em outro processo/máquina.
Table Data Gateway (DAO) :: Fachada por tipo de objeto que encapsula o acesso a uma tabela.
Repository :: Coleção abstrata de entidades que esconde a persistência.
Camada de isolamento :: Código que concentra o uso de uma API de terceiros.
Separação de preocupações :: Regras de negócio independentes de detalhes de armazenamento.
DTO :: Objeto simples de transferência de dados.
:::
