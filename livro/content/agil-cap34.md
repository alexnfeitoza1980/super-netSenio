---
id: agil-cap34
order: 51
kind: capitulo
num: 34
group: C# · Parte 11 · Pacotes, padrões e arquitetura
title: Factory e Composite
subtitle: Criar objetos dependendo só de abstrações (e trocar famílias inteiras), e tratar um grupo de objetos como se fosse um só.
minutes: 50
hue: 165
objectives:
- Explicar por que `new` numa classe concreta **viola o DIP** e como **Factory** resolve
- Distinguir **fábrica com método por tipo**, **fábrica parametrizada** e **fábrica abstrata (família)**
- Usar fábricas para **trocar implementações** e **substituir dependências em testes**
- Aplicar **Composite** para transformar relações 1:muitos em 1:1 e tratar grupos de forma uniforme
origem: livros/02-projeto-agil-csharp/content/50-cap34.md
---

::: ilustracao img/agil/cap34.svg | Factory decide qual objeto criar; Composite trata um e muitos do mesmo jeito.
:::

## Factory (Fábrica)

O DIP diz para **dependermos de abstrações**, não de classes concretas voláteis. Então este código **viola** o DIP:

```cs
Circulo c = new Circulo(origem, 1);
```

`Circulo` é concreta; **todo** `new` de classe concreta viola o DIP. Às vezes isso é **inofensivo**: criar `string`, `List<T>` ou `DateTime` é seguro, porque essas classes **não são voláteis**. Mas durante o desenvolvimento ativo, muitas das **suas** classes concretas mudam bastante, e depender delas é problemático.

O padrão **Factory** permite **criar instâncias de objetos concretos dependendo só de interfaces abstratas**.

### O problema

Uma classe `Aplicativo` usa `IForma` exclusivamente pela interface (nunca chama algo específico de `Quadrado` ou `Circulo`) — mas **cria** instâncias deles, e por isso precisa depender das classes concretas.

::: figura Antes e depois
<svg viewBox="0 0 760 250" role="img" aria-label="Aplicativo criando concretos × usando uma fábrica">
  <text x="170" y="16" text-anchor="middle" font-weight="800" class="t-bad">Antes: new espalhado</text>
  <g text-anchor="middle" font-size="12"><rect class="f-prim" x="10" y="40" width="120" height="40" rx="6"/><text x="70" y="65">Aplicativo</text><rect class="f-info" x="190" y="30" width="130" height="34" rx="6"/><text x="255" y="52">«interface» IForma</text><rect class="f-bad" x="190" y="120" width="60" height="34" rx="6"/><text x="220" y="142">Circulo</text><rect class="f-bad" x="260" y="120" width="70" height="34" rx="6"/><text x="295" y="142">Quadrado</text></g>
  <path class="arrow" d="M132 55H188"/><path class="arrow" d="M90 82L205 118" style="stroke:var(--bad);marker-end:url(#arr3)"/><path class="arrow" d="M100 82L285 118" style="stroke:var(--bad);marker-end:url(#arr3)"/><text x="170" y="190" text-anchor="middle" font-size="12" class="t-bad">cria concretos → depende deles</text>
  <text x="570" y="16" text-anchor="middle" font-weight="800" class="t-good">Depois: Factory</text>
  <g text-anchor="middle" font-size="12"><rect class="f-prim" x="400" y="40" width="110" height="40" rx="6"/><text x="455" y="65">Aplicativo</text><rect class="f-info" x="560" y="30" width="130" height="34" rx="6"/><text x="625" y="52">«interface» IForma</text><rect class="f-info" x="540" y="95" width="170" height="34" rx="6"/><text x="625" y="117">«interface» IFabricaDeFormas</text><rect class="f-good" x="540" y="165" width="170" height="34" rx="6"/><text x="625" y="187">FabricaDeFormas (concreta)</text></g>
  <path class="arrow" d="M512 55H558"/><path class="arrow" d="M512 72L538 108"/><path class="arrow" d="M625 165V131" style="stroke:var(--good);marker-end:url(#arr)"/><text x="570" y="230" text-anchor="middle" font-size="12" class="t-dim">só a fábrica concreta conhece Circulo e Quadrado</text>
</svg>
:::

```cs
public interface IForma { void Desenhar(); }
public class Circulo  : IForma { public void Desenhar() { /* ... */ } }
public class Quadrado : IForma { public void Desenhar() { /* ... */ } }

public interface IFabricaDeFormas
{
    IForma CriarCirculo();
    IForma CriarQuadrado();
}

public class FabricaDeFormas : IFabricaDeFormas
{
    public IForma CriarCirculo()  { return new Circulo(); }
    public IForma CriarQuadrado() { return new Quadrado(); }
}

public class Aplicativo
{
    private readonly IFabricaDeFormas fabrica;
    public Aplicativo(IFabricaDeFormas fabrica) { this.fabrica = fabrica; }

    public void Executar()
    {
        IForma f = fabrica.CriarCirculo();      // não conhece Circulo
        f.Desenhar();
    }
}
```

O aplicativo **não depende mais** de `Circulo` nem de `Quadrado`, mas ainda os cria. Alguém precisa criar `FabricaDeFormas` — em geral `Main` (a **raiz de composição**) — e **ninguém mais** cria as formas.

### Um problema de dependência dentro da fábrica

A fábrica tem **um método por derivada**: é uma dependência **por nome**. Cada nova forma exige **mais um método** na interface — e, em tese, recompilar e redistribuir todos os usuários da fábrica.

Solução: sacrificar **um pouco de segurança de tipos** por flexibilidade com uma **fábrica parametrizada**:

```cs
public interface IFabricaDeFormas { IForma Criar(string nome); }

public class FabricaDeFormas : IFabricaDeFormas
{
    private readonly Dictionary<string, Func<IForma>> criadores = new Dictionary<string, Func<IForma>>
    {
        { "circulo",  () => new Circulo()  },
        { "quadrado", () => new Quadrado() },
    };

    public IForma Criar(string nome)
    {
        if (!criadores.TryGetValue(nome, out var criar))
            throw new ArgumentException("Forma desconhecida: " + nome);
        return criar();
    }

    public void Registrar(string nome, Func<IForma> criar) { criadores[nome] = criar; }   // novos tipos sem alterar a interface
}
```

Agora um novo tipo **não muda a interface** (`Registrar("triangulo", () => new Triangulo())`). O preço: um nome errado dá **erro em tempo de execução** em vez de erro de compilação. Mitigação: **testes de unidade** (a prática do TDD pega esses erros cedo) e **constantes/enum** para os nomes.

::: pense Tipagem estática × dinâmica
Esse compromisso entre **segurança de tipos** e **flexibilidade** resume um debate antigo. Linguagens **estáticas** (C#, Java, C++) verificam tipos na compilação; as **dinâmicas** (Python, Ruby) só na execução. A tipagem estática pode criar **laços de dependência** que forçam alterações de arquivos só para manter a coerência de tipos (aqui, mudar `IFabricaDeFormas` a cada nova forma). Quem defende a estática diz que a segurança compensa; quem defende a dinâmica diz que **testes unitários** acham a maioria dos problemas que os tipos achariam. Dica prática: em C#, `dynamic`, reflexão e DI trazem alguma flexibilidade; **bons testes** equilibram a conta.
:::

### Fábricas substituíveis

Uma das maiores vantagens é **trocar uma implementação de fábrica por outra** e, com isso, **trocar famílias inteiras de objetos**. É o padrão **Abstract Factory**:

```cs
public interface IFabricaDeRepositorios
{
    IRepositorioDeClientes Clientes();
    IRepositorioDePedidos Pedidos();
}

public class FabricaSql : IFabricaDeRepositorios
{
    public IRepositorioDeClientes Clientes() { return new ClientesSql(); }
    public IRepositorioDePedidos Pedidos()   { return new PedidosSql(); }
}

public class FabricaEmArquivo : IFabricaDeRepositorios
{
    public IRepositorioDeClientes Clientes() { return new ClientesJson(); }
    public IRepositorioDePedidos Pedidos()   { return new PedidosJson(); }
}

// o aplicativo escolhe a família uma vez, na inicialização:
IFabricaDeRepositorios fabrica = config.UsaSql ? new FabricaSql() : new FabricaEmArquivo();
```

O aplicativo **nem sabe nem se importa** com qual família usa. O próprio .NET traz isso pronto: `DbProviderFactory` (cria `DbConnection`, `DbCommand`, `DbDataAdapter` de um provedor — SQL Server, SQLite, Oracle…).

::: figura Fábrica substituível (família)
<svg viewBox="0 0 740 230" role="img" aria-label="Aplicativo usa IFabricaDeRepositorios; duas fábricas criam famílias diferentes">
  <rect class="f-prim" x="10" y="85" width="130" height="50" rx="10"/><text x="75" y="115" text-anchor="middle" font-weight="800">Aplicativo</text>
  <path class="arrow" d="M142 110H232"/>
  <rect class="f-info" x="234" y="80" width="190" height="60" rx="10"/><text x="329" y="104" text-anchor="middle" font-size="10" class="t-dim">«interface»</text><text x="329" y="124" text-anchor="middle" font-weight="800" class="t-info">IFabricaDeRepositorios</text>
  <path class="line" d="M329 142V172M200 172H500M200 172V186M500 172V186" style="marker-end:none"/>
  <rect class="f-good" x="90" y="186" width="220" height="38" rx="8"/><text x="200" y="210" text-anchor="middle" font-size="12">FabricaSql → ClientesSql, PedidosSql</text>
  <rect class="f-good" x="390" y="186" width="220" height="38" rx="8"/><text x="500" y="210" text-anchor="middle" font-size="12">FabricaEmArquivo → …Json</text>
</svg>
:::

### Fábricas para dispositivos de teste

Em testes unitários queremos testar um módulo **separado** do que ele usa. `FolhaDePagamento` usa um banco; para testá-la **sem banco real**, ela deve depender de **`IBanco`** e o teste fornece um **falso** (*stub*/*mock*/*spy*) que **registra as chamadas** e permite **simular falhas** difíceis de gerar de verdade.

Mas como `FolhaDePagamento` obtém esse falso? Opções:

1. O teste **passa** a dependência ao construtor (**injeção** — o mais natural).
2. O teste configura uma **variável/serviço global**.
3. Se a classe **cria** o banco internamente, ela pede à **fábrica** — e o teste fornece uma **fábrica alternativa** que devolve o falso.

```cs
public class FolhaDePagamento
{
    private readonly IFabricaDeBanco fabrica;
    public FolhaDePagamento(IFabricaDeBanco fabrica) { this.fabrica = fabrica; }
    public void Processar() { IBanco banco = fabrica.Criar(); /* usa banco */ }
}

class FabricaDeBancoFalso : IFabricaDeBanco
{
    public readonly BancoFalso Banco = new BancoFalso();
    public IBanco Criar() { return Banco; }
}
```

### Não use fábricas por padrão

::: cuidado A tentação das fábricas
Uma leitura rígida do DIP exigiria fábrica para **toda classe volátil**; e o poder do padrão é sedutor. Mas **usar fábricas por padrão** é má prática: elas são **complexidade que muitas vezes pode ser evitada**, sobretudo no início. Para criar **uma** classe nova, você pode acabar escrevendo até **quatro**: a interface do tipo, a interface da fábrica e as duas implementações concretas. O autor só introduz fábricas **quando se tornam necessárias** (por exemplo, quando surge um **Proxy** ou quando um teste exige falsificar quem cria o objeto).
:::

| Se você… | Então |
|---|---|
| Cria `string`, `List<T>`, tipos estáveis | `new` direto — sem fábrica |
| Cria um tipo volátil em **um** lugar | `new` direto + extraia a interface depois (refatore quando doer) |
| Precisa **escolher o tipo** por dado/config ou **trocar famílias** | Fábrica (ou registro no **contêiner de DI**) |
| Precisa **injetar um falso** no teste | **Injeção por construtor** (fábrica só se o objeto for criado dentro) |

No .NET moderno, o **contêiner de injeção de dependência** é, na prática, uma fábrica gigante: `services.AddTransient<IForma, Circulo>()` + `provider.GetRequiredService<IForma>()`; e `Func<IForma>` injetada serve de fábrica leve.

## Composite

O **Composite** é muito simples e tem implicações grandes. Numa hierarquia de formas: `IForma` tem `Circulo`, `Quadrado` e uma terceira derivada, o **composto** (`FormaComposta`), que **guarda uma lista** de formas e, ao ser chamado em `Desenhar()`, **repassa** a chamada a todas as formas da lista.

```cs
public class FormaComposta : IForma
{
    private readonly List<IForma> formas = new List<IForma>();
    public void Adicionar(IForma f) { formas.Add(f); }
    public void Desenhar() { foreach (IForma f in formas) f.Desenhar(); }
}

var casa = new FormaComposta();
casa.Adicionar(new Quadrado());
casa.Adicionar(new Triangulo());
casa.Desenhar();                  // "casa" se comporta como UMA forma
```

Para o sistema, uma `FormaComposta` **parece ser um único objeto `IForma`**: pode ser passada a qualquer função que receba `IForma`. Na verdade é um **proxy** de um grupo (note a semelhança estrutural com o Proxy). E o composto pode **conter outros compostos**: uma árvore.

::: figura Composite
<svg viewBox="0 0 700 220" role="img" aria-label="IForma com Circulo, Quadrado e FormaComposta que contém 0..* IForma">
  <rect class="f-info" x="250" y="10" width="200" height="46" rx="10"/><text x="350" y="30" text-anchor="middle" font-size="10" class="t-dim">«interface»</text><text x="350" y="48" text-anchor="middle" font-weight="800" class="t-info">IForma</text>
  <path class="line" d="M350 58V84M110 84H590M110 84V104M350 84V104M590 84V104" style="marker-end:none"/>
  <rect class="f-plain" x="40" y="104" width="140" height="38" rx="8"/><text x="110" y="128" text-anchor="middle">Circulo</text>
  <rect class="f-plain" x="280" y="104" width="140" height="38" rx="8"/><text x="350" y="128" text-anchor="middle">Quadrado</text>
  <rect class="f-good" x="500" y="104" width="180" height="60" rx="8"/><text x="590" y="126" text-anchor="middle" font-weight="800" class="t-good">FormaComposta</text><text x="590" y="146" text-anchor="middle" font-size="11" class="t-mono">Adicionar(f) · Desenhar()</text>
  <path class="arrow" d="M590 166C590 205 720 205 700 150C690 125 690 120 684 128" style="stroke:var(--good);marker-end:url(#arr)"/><text x="620" y="204" font-size="11" class="t-good">0..* (contém IForma, inclusive outros compostos)</text>
</svg>
:::

### Comandos compostos

Retomando o padrão **Command** (capítulo 29): um sensor dispara um comando quando detecta algo. Muitas vezes é preciso disparar **vários** comandos (parar um motor, ligar outro, engatar uma embreagem). A tentação: dar ao sensor uma **lista** de comandos. Mas o sensor **sempre** trata todos de forma idêntica (percorre a lista e chama `Execute`) — situação **ideal** para Composite:

```cs
public class ComandoComposto : ICommand
{
    private readonly List<ICommand> comandos = new List<ICommand>();
    public ComandoComposto Adicionar(ICommand c) { comandos.Add(c); return this; }
    public void Execute() { foreach (var c in comandos) c.Execute(); }
}

var aoPassarPapel = new ComandoComposto()
    .Adicionar(new DesligarMotor(1))
    .Adicionar(new LigarMotor(2))
    .Adicionar(new EngatarEmbreagem(3));

var sensor = new SensorOptico(aoPassarPapel);       // continua 1:1 com UM comando
```

`Sensor` e `ICommand` **não precisaram mudar**: ganhamos "um sensor, muitos comandos" **sem alterar** nenhum dos dois — aplicação do **OCP**.

### Multiplicidade ou nenhuma multiplicidade

O interessante: obtivemos comportamento **1:muitos** **sem** um relacionamento 1:muitos. E **1:1 é bem mais fácil** de entender, codificar e manter. **Quantos dos seus relacionamentos 1:muitos poderiam ser 1:1 com Composite?**

Nem todos: só os em que **todos os objetos da lista são tratados de forma idêntica**. Uma lista de funcionários da qual você **procura** quem tem pagamento hoje (cada um é tratado de modo diferente) **não** é candidata.

**Vantagem:** em vez de duplicar o código de **gerenciar lista e iterar** em cada cliente, ele aparece **uma só vez**, na classe composta.

### Composite no mundo real (.NET)

| Onde | Folha | Composto |
|---|---|---|
| Sistema de arquivos | Arquivo | Pasta |
| Controles Windows Forms/WPF | `Button`, `TextBox` | `Panel`, `Form` (`Controls` contém controles) |
| Menus | Item de menu | Submenu |
| Expressões/ASTs | Número, variável | Soma, produto… |
| Validação | Regra simples | Regra "todas" / "alguma" |
| Command | Comando simples | Macro (comando composto) |

::: dica Composite + Visitor
Quando você tem uma árvore e quer **operações novas** sem mexer nas classes (calcular tamanho, imprimir, exportar), o **Visitor** (capítulo 38) combina com o Composite.
:::

::: exercicio Aplique Composite
Um `Pedido` deve calcular o preço total de **itens** (cada um com `Preco()`), mas também existem **kits** (conjuntos de itens com 10% de desconto) que podem conter outros kits. Modele de forma que `Pedido` trate itens e kits **igualmente**.

--- resposta
```cs
public interface IItemDeVenda { decimal Preco(); }

public class Produto : IItemDeVenda
{
    private readonly decimal preco;
    public Produto(decimal preco) { this.preco = preco; }
    public decimal Preco() { return preco; }
}

public class Kit : IItemDeVenda
{
    private readonly List<IItemDeVenda> itens = new List<IItemDeVenda>();
    public Kit Adicionar(IItemDeVenda i) { itens.Add(i); return this; }
    public decimal Preco() { return itens.Sum(i => i.Preco()) * 0.90m; }     // composto: soma e aplica o desconto
}

public class Pedido
{
    private readonly List<IItemDeVenda> itens = new List<IItemDeVenda>();
    public void Adicionar(IItemDeVenda i) { itens.Add(i); }
    public decimal Total() { return itens.Sum(i => i.Preco()); }     // nem sabe se é item ou kit
}
```
`Pedido` só conhece `IItemDeVenda`; um kit dentro de um kit funciona sem alterar nada.
:::

::: resumo
- **Factory:** cria objetos concretos dependendo só de abstrações; só a fábrica concreta (criada na raiz) conhece as classes concretas.
- **Fábrica por método** (seguro, mas dependente do nome) × **fábrica parametrizada** (flexível, erros em tempo de execução — cubra com testes).
- **Abstract Factory** troca **famílias** de objetos (ex.: repositórios SQL × arquivos); `DbProviderFactory` é um exemplo no .NET.
- Fábricas ajudam a **injetar falsos** em testes — mas **não use fábricas por padrão**: elas acrescentam classes e complexidade; introduza-as quando necessárias (ou use DI).
- **Composite:** o composto implementa a mesma interface das folhas e repassa chamadas; transforma **1:muitos em 1:1**.
- Só aplique onde **todos os elementos são tratados de modo idêntico**; elimina código duplicado de lista/iteração.
:::

::: quiz
? Por que `new Circulo()` num módulo de alto nível viola o DIP?
+ Porque ele passa a depender de uma classe concreta (potencialmente volátil)
- Porque `new` é proibido
- Porque Circulo é abstrato
- Porque cria objetos demais
> Fábricas isolam esse conhecimento.

? Onde normalmente se cria a fábrica concreta?
+ Na raiz de composição (por exemplo, Main ou na configuração de DI)
- Dentro de cada classe de negócio
- Em cada método
- No banco de dados
> Só ali se conhecem as classes concretas.

? Qual a desvantagem da fábrica parametrizada (`Criar(string nome)`)?
+ Erros de nome só aparecem em tempo de execução (cubra com testes)
- Exige uma interface por tipo
- Não permite novos tipos
- É sempre mais lenta que `new`
> Em troca, novos tipos não alteram a interface.

? O que uma Abstract Factory permite trocar?
+ Famílias inteiras de objetos relacionados (ex.: repositórios SQL por repositórios em arquivo)
- Apenas um objeto
- Apenas o nome das classes
- O compilador
> O cliente usa a interface da fábrica e não sabe qual família está em uso.

? Por que não se deve usar fábricas por padrão?
+ Porque acrescentam complexidade (podem multiplicar classes) e frequentemente podem ser evitadas
- Porque são proibidas pelo DIP
- Porque só funcionam em Java
- Porque são lentas demais
> Use quando for necessário.

? O que o Composite faz?
+ Permite tratar um grupo de objetos como se fosse um só, implementando a mesma interface
- Cria objetos sem new
- Garante uma única instância
- Observa eventos
> O composto repassa a chamada para seus filhos.

? Que tipo de relação 1:muitos NÃO é candidata a Composite?
+ Aquela em que cada elemento da lista é tratado de forma diferente (ex.: procurar quem tem pagamento hoje)
- Uma lista de comandos executados em sequência
- Uma lista de formas a desenhar
- Uma lista de itens de menu
> Composite exige tratamento idêntico.

? Qual é a vantagem de usar um ComandoComposto num Sensor?
+ O sensor continua 1:1 com um comando e ganha vários comandos sem ser alterado (OCP)
- O sensor fica mais rápido
- Elimina o Command
- Remove a necessidade de testes
> Composite + Command.
:::

::: cartas
Factory :: Padrão que cria instâncias concretas retornando abstrações, isolando as classes concretas.
Fábrica parametrizada :: Fábrica com um único método de criação que recebe um nome/chave.
Abstract Factory :: Fábrica que cria uma família de objetos relacionados; permite trocar a família toda.
Raiz de composição :: Lugar onde as fábricas/objetos concretos são criados e ligados.
Composite :: Objeto composto que implementa a mesma interface das folhas e repassa chamadas aos filhos.
Folha (leaf) :: Elemento simples de uma estrutura Composite.
Comando composto :: Command que executa uma lista de outros comandos.
Tipagem estática × dinâmica :: Verificação de tipos na compilação × na execução.
:::
