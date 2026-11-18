---
id: agil-cap32
order: 48
kind: capitulo
num: 32
group: C# · Parte 10 · Padrões de projeto (I)
title: Singleton e Monostate
subtitle: Duas formas de garantir uma única "instância" — uma impõe a estrutura, a outra o comportamento — com custos, benefícios e alternativas modernas.
minutes: 45
hue: 320
objectives:
- Distinguir **Singleton** (estrutura única) de **Monostate** (comportamento único)
- Implementar Singleton **thread-safe** em C# (`static readonly`, `Lazy<T>`)
- Listar **benefícios e custos** de cada padrão
- Conhecer a alternativa **"basta criar um"** e o ciclo de vida **singleton** da injeção de dependência
origem: livros/02-projeto-agil-csharp/content/47-cap32.md
---

::: ilustracao img/agil/cap32.svg | Singleton: uma única instância para todos; Monostate: várias referências, um só estado.
:::

## Quando só pode haver um

Normalmente a relação entre classes e instâncias é **um para muitos**: criamos objetos quando precisamos e descartamos quando acabam. Mas algumas classes devem ter **apenas uma instância**, que parece existir desde o início do programa e só morre com ele. São as **raízes** do aplicativo (de onde se alcança muita coisa), as **fábricas** que criam os demais objetos, os **gerenciadores** que supervisionam outros objetos.

Se um desses objetos for criado **mais de uma vez**, é uma **falha lógica grave**: com duas raízes, parte do sistema vê um subconjunto dos objetos sem saber; com duas fábricas, perde-se o controle administrativo; com dois gerenciadores, atividades que deveriam ser **seriais** viram **concorrentes**.

### A estratégia mais simples: "basta criar um"

Pode parecer exagero criar mecanismos para impor a singularidade. Afinal, na inicialização você pode **criar um de cada** e pronto — um padrão informal chamado **JUST CREATE ONE** ("basta criar um"). **Quase sempre é a melhor estratégia.** Mecanismos de imposição devem ser **evitados** quando não houver necessidade **imediata e significativa**.

Por outro lado, queremos que o **código comunique a intenção**: se impor a unicidade for **trivial**, o ganho de comunicação pode compensar o custo. Dois padrões fazem isso, com **compromissos muito diferentes**.

## Singleton

A estrutura do **Singleton** é simples: um **construtor privado**, uma **variável estática** que guarda a instância e uma **propriedade estática** (`Instance`) que a entrega.

```cs
public sealed class Configuracao
{
    private static readonly Configuracao instancia = new Configuracao();   // criada uma vez, pelo runtime
    public static Configuracao Instance { get { return instancia; } }

    private Configuracao() { /* carrega do arquivo, etc. */ }

    public string Ambiente { get; set; } = "Producao";
}

// uso:
var a = Configuracao.Instance;
var b = Configuracao.Instance;
Console.WriteLine(ReferenceEquals(a, b));     // True — sempre a mesma
// new Configuracao();                         // ERRO: construtor privado
```

Os testes que "especificam" o padrão dizem duas coisas: (1) várias chamadas a `Instance` devolvem **exatamente a mesma instância**; (2) a classe **não tem construtor público**.

::: figura Singleton
<svg viewBox="0 0 700 190" role="img" aria-label="Vários clientes obtêm a mesma instância por Instance">
  <g text-anchor="middle" font-size="13"><rect class="f-plain" x="10" y="10" width="110" height="36" rx="6"/><text x="65" y="33">Cliente A</text><rect class="f-plain" x="10" y="75" width="110" height="36" rx="6"/><text x="65" y="98">Cliente B</text><rect class="f-plain" x="10" y="140" width="110" height="36" rx="6"/><text x="65" y="163">Cliente C</text></g>
  <path class="arrow" d="M122 30L290 80"/><path class="arrow" d="M122 93H290"/><path class="arrow" d="M122 158L290 108"/>
  <text x="206" y="62" font-size="11" class="t-mono">.Instance</text>
  <rect class="f-prim" x="292" y="50" width="200" height="90" rx="12"/><text x="392" y="76" text-anchor="middle" font-weight="800">Configuracao</text><g class="t-mono" font-size="11"><text x="306" y="98">- instancia (static)</text><text x="306" y="114">- Configuracao()  «private»</text><text x="306" y="130">+ Instance (static)</text></g>
  <rect class="f-good" x="540" y="68" width="140" height="54" rx="10"/><text x="610" y="100" text-anchor="middle" font-size="13">a única instância</text>
  <path class="arrow" d="M494 95H538"/>
</svg>
:::

### Variações de implementação em C#

```cs
// 1) Inicialização estática (acima): thread-safe, simples. O .NET garante criação única.

// 2) Preguiçosa (lazy) e thread-safe com Lazy<T> — só cria no primeiro uso
public sealed class Cache
{
    private static readonly Lazy<Cache> instancia = new Lazy<Cache>(() => new Cache());
    public static Cache Instance { get { return instancia.Value; } }
    private Cache() { }
}

// 3) A versão "ingênua" (NÃO thread-safe!):
public sealed class Ruim
{
    private static Ruim instancia;
    public static Ruim Instance { get { if (instancia == null) instancia = new Ruim(); return instancia; } }
    private Ruim() { }
}   // duas threads podem passar pelo "if" ao mesmo tempo e criar duas instâncias!
```

::: cuidado Singleton "preguiçoso" ingênuo é perigoso
A versão com `if (instancia == null)` **pode criar duas instâncias** em ambiente multi-thread. Em C#, prefira **inicialização estática** (`static readonly`) ou `Lazy<T>`.
:::

### Benefícios

- **Aplicável a qualquer classe:** basta tornar o construtor privado e acrescentar o membro estático.
- **Avaliação preguiçosa:** se nunca for usado, nunca é criado (com `Lazy<T>` ou inicializador estático em classe sem outros usos).
- **Pode ser criado por derivação:** dá para criar uma subclasse singleton.
- **Estendível** a vários processos/máquinas com middleware apropriado (remoting, serviços).

### Custos

- **Destruição indefinida:** não há boa maneira de "desativar" um singleton. Se você anula a instância, outros módulos **ainda têm a referência antiga**; a próxima chamada a `Instance` cria **outra** — e agora existem **duas**.
- **Não é herdado:** a subclasse de um singleton **não é** singleton; precisa do seu próprio membro estático.
- **Eficiência:** cada acesso passa pelo `if` (ou pelo `Lazy`).
- **Não transparente:** quem usa **sabe** que usa um singleton (precisa chamar `Instance`).

### Singleton em ação: o acesso a usuários

Um sistema web precisa ler/gravar usuários num banco por uma API externa. Se cada módulo chamasse a API, o uso se espalharia e não haveria **onde impor convenções** (por exemplo, "não grave usuário sem `Username`", ou "serialize acessos a um mesmo registro"). A solução: uma **fachada** (`IBancoDeUsuarios`) **única**:

```cs
public interface IBancoDeUsuarios
{
    Usuario Ler(string nome);
    void Gravar(Usuario u);
}

public sealed class BancoDeUsuarios : IBancoDeUsuarios
{
    public static readonly IBancoDeUsuarios Instance = new BancoDeUsuarios();   // singleton exposto pela INTERFACE
    private BancoDeUsuarios() { }

    private readonly object trava = new object();

    public Usuario Ler(string nome) { /* ... */ return null; }
    public void Gravar(Usuario u)
    {
        if (string.IsNullOrWhiteSpace(u.Nome)) throw new ArgumentException("Nome obrigatório");
        lock (trava) { /* grava com exclusão mútua */ }
    }
}
```

Como todo acesso passa por **uma só instância**, é fácil colocar **verificações, contadores e travas** nela.

::: pense Singleton e testabilidade
Código que chama `BancoDeUsuarios.Instance` **por dentro** fica **acoplado** a ele e **difícil de testar** (não dá para trocá-lo por um falso). O singleton é, na prática, um **estado global** disfarçado. Para mitigar: exponha-o **como interface** e **injete** (`new Servico(BancoDeUsuarios.Instance)`) em vez de acessá-lo em todo lugar.
:::

## Monostate

O **Monostate** garante a singularidade por um **mecanismo completamente diferente**: permite **várias instâncias**, mas todas **compartilham o mesmo estado** — porque as **variáveis são estáticas** e os **métodos não são**.

```cs
public class Contador                       // monostate
{
    private static int valor;               // estado compartilhado (estático)

    public int Valor { get { return valor; } set { valor = value; } }
    public void Incrementar() { valor++; }
}

var a = new Contador();
var b = new Contador();
a.Incrementar();
a.Incrementar();
Console.WriteLine(b.Valor);                 // 2 — "a" e "b" se comportam como um objeto só
```

Duas instâncias se comportam como **um único objeto**: são apenas **nomes diferentes** para o mesmo estado. Dá até para destruir todas as instâncias sem perder os dados.

::: regra A diferença: estrutura × comportamento
**Singleton impõe a *estrutura* da singularidade** (impede mais de uma instância). **Monostate impõe o *comportamento* da singularidade** (várias instâncias, estado único). O teste do Monostate também passaria para um Singleton; o teste do Singleton **não** passaria para um Monostate.
:::

::: figura Singleton × Monostate
<svg viewBox="0 0 740 220" role="img" aria-label="Singleton: uma instância; Monostate: várias instâncias apontando para estado estático compartilhado">
  <text x="170" y="18" text-anchor="middle" font-weight="800" class="t-prim">Singleton</text>
  <g text-anchor="middle" font-size="12"><rect class="f-plain" x="10" y="40" width="90" height="34" rx="6"/><text x="55" y="62">cliente</text><rect class="f-plain" x="10" y="100" width="90" height="34" rx="6"/><text x="55" y="122">cliente</text><rect class="f-prim" x="190" y="60" width="150" height="70" rx="10"/><text x="265" y="90" font-weight="800">Instance</text><text x="265" y="110" font-size="11" class="t-dim">estado dentro</text></g>
  <path class="arrow" d="M102 57L188 85"/><path class="arrow" d="M102 117L188 105"/>
  <text x="570" y="18" text-anchor="middle" font-weight="800" class="t-good">Monostate</text>
  <g text-anchor="middle" font-size="12"><rect class="f-plain" x="410" y="30" width="90" height="34" rx="6"/><text x="455" y="52">new C()</text><rect class="f-plain" x="410" y="80" width="90" height="34" rx="6"/><text x="455" y="102">new C()</text><rect class="f-plain" x="410" y="130" width="90" height="34" rx="6"/><text x="455" y="152">new C()</text><rect class="f-good" x="600" y="65" width="130" height="64" rx="10"/><text x="665" y="92" font-weight="800">estado estático</text><text x="665" y="112" font-size="11" class="t-dim">compartilhado</text></g>
  <path class="arrow" d="M502 47L598 80"/><path class="arrow" d="M502 97H598"/><path class="arrow" d="M502 147L598 114"/>
</svg>
:::

### Benefícios do Monostate

- **Transparência:** quem usa **não percebe** nada de especial: cria com `new` como qualquer objeto.
- **Derivável:** as derivadas **também são monostate** — e **todas compartilham as mesmas variáveis estáticas** (fazem parte do mesmo monostate).
- **Polimorfismo:** como os **métodos não são estáticos**, podem ser **sobrescritos**: derivadas diferentes oferecem comportamentos diferentes sobre o mesmo estado.
- **Criação e destruição bem definidas:** as variáveis estáticas têm ciclo de vida claro.

### Custos do Monostate

- **Sem conversão:** uma classe **não monostate** não vira monostate por derivação.
- **Eficiência:** são objetos reais; criar/destruir muitos pode ser caro.
- **Presença:** as variáveis ocupam espaço **mesmo que ninguém use** a classe.
- **Local por plataforma:** não funciona para além de um único processo/CLR.
- **Estado global oculto:** como é transparente, **ninguém desconfia** de que dois objetos aparentemente independentes se afetam — fonte de **bugs sutis** e de testes que dependem da ordem (é preciso um `Reset()`).

### Monostate em ação: a catraca (máquina de estados)

Uma catraca de transporte tem um estado (`Travada`/`Livre`), um contador de moedas e de devoluções. Se o sistema **nunca terá mais de uma catraca**, dá para modelar a FSM como monostate, com **estados como derivadas polimórficas**:

```cs
public class Catraca                            // monostate
{
    private static bool travada = true;
    private static bool alarme = false;
    private static int moedas, devolucoes;
    protected static readonly Catraca TRAVADA = new CatracaTravada();
    protected static readonly Catraca LIVRE = new CatracaLivre();
    private static Catraca estado = TRAVADA;

    public bool Travada => travada;
    public bool Alarme => alarme;
    public int Moedas => moedas;
    public int Devolucoes => devolucoes;

    public void Reiniciar() { travada = true; alarme = false; moedas = devolucoes = 0; estado = TRAVADA; }

    public virtual void Moeda()    { estado.Moeda(); }       // delega ao estado atual
    public virtual void Passagem() { estado.Passagem(); }

    protected void Travar(bool v)  { travada = v; }
    protected void Alarmar(bool v) { alarme = v; }
    protected void Depositar()     { moedas++; }
    protected void Devolver()      { devolucoes++; }
    protected void Mudar(Catraca novo) { estado = novo; }
}

internal class CatracaTravada : Catraca
{
    public override void Moeda()    { Mudar(LIVRE); Travar(false); Alarmar(false); Depositar(); }
    public override void Passagem() { Alarmar(true); }
}

internal class CatracaLivre : Catraca
{
    public override void Moeda()    { Devolver(); }
    public override void Passagem() { Travar(true); Mudar(TRAVADA); }
}
```

Qualquer instância (`new Catraca()`) enxerga **o mesmo estado**; os subtipos não são "objetos distintos" — fazem parte da **abstração** `Catraca`. É o padrão **State** (capítulo 39) usando monostate.

::: cuidado A armadilha da catraca
Se amanhã o sistema precisar de **duas catracas**, o desenho monostate exige **refatoração significativa**. Dependência da natureza monostate é o preço.
:::

## Singleton ou Monostate?

| Situação | Melhor escolha |
|---|---|
| Quer **restringir por derivação** e aceita que todos chamem `Instance` | **Singleton** |
| Quer que a singularidade seja **transparente** para os clientes | **Monostate** |
| Quer **derivadas polimórficas** do único objeto | **Monostate** |
| Quer **criação preguiçosa** | **Singleton** (com `Lazy<T>`) |
| Pode apenas criar uma instância na inicialização e passá-la adiante | **Nenhum: "basta criar um" + injeção** |

## O jeito moderno: o ciclo de vida *singleton* da injeção de dependência

Em aplicativos .NET atuais, o contêiner de DI resolve quase tudo isso **sem** o padrão Singleton clássico:

```cs
// Program.cs (ASP.NET Core)
builder.Services.AddSingleton<IBancoDeUsuarios, BancoDeUsuarios>();     // UMA instância para a aplicação inteira
builder.Services.AddScoped<IServico, Servico>();                         // uma por requisição
builder.Services.AddTransient<IFormatador, Formatador>();                // nova a cada uso

public class Servico
{
    private readonly IBancoDeUsuarios banco;
    public Servico(IBancoDeUsuarios banco) { this.banco = banco; }       // recebe pronta; nada de .Instance
}
```

Aqui a **singularidade** vem do **ciclo de vida configurado**, e a classe **não** precisa de construtor privado nem de membro estático: continua **testável** (troca-se por um falso) e **sem estado global escondido**. É, na essência, o "basta criar um" com apoio de ferramenta.

::: exercicio Qual padrão?
Escolha entre Singleton, Monostate, ou "basta criar um + injetar" e justifique:

1. Um **registro de log** usado por 40 classes, que você quer substituir por um falso nos testes.
2. Uma classe `Preferencias` que o resto do sistema deve usar com `new Preferencias()` como qualquer objeto, mas sem nunca divergir.
3. Um **gerador de IDs** que, se criado duas vezes, geraria IDs duplicados.

--- resposta
1. **Basta criar um + injetar** (`ILogger` registrado como *singleton* no contêiner): testável e sem estado global.
2. **Monostate**: transparência; todas as instâncias compartilham o estado. (Cuidado com testes: precisa de `Reset`.)
3. **Singleton** (ou, melhor, um único objeto registrado como singleton no DI): a unicidade é **requisito de correção**; um construtor privado ou o ciclo de vida do contêiner a impõem. Proteja o gerador com `lock`/`Interlocked` por causa da concorrência.
:::

::: resumo
- Algumas classes (raízes, fábricas, gerenciadores) **devem ter uma única instância**; duas seriam uma falha lógica grave.
- A melhor estratégia costuma ser **"basta criar um"** na inicialização e passá-lo adiante; só imponha unicidade com necessidade real.
- **Singleton:** construtor privado + membro estático (`Instance`); em C#, use **inicialização estática** ou `Lazy<T>` para ser *thread-safe*. Custos: destruição indefinida, não herdado, não transparente.
- **Monostate:** várias instâncias, **estado estático compartilhado**; transparente, derivável e polimórfico; custa presença, ligação à plataforma e "estado global escondido".
- **Singleton = estrutura**; **Monostate = comportamento** da singularidade.
- Hoje, o **ciclo de vida *singleton* do contêiner de DI** resolve a maioria dos casos com **testabilidade**.
:::

::: quiz
? O que o Singleton impõe?
+ A estrutura: só pode existir uma instância (construtor privado + membro estático)
- Que todas as variáveis sejam estáticas
- Que a classe seja abstrata
- Que não haja métodos
> Monostate, ao contrário, impõe comportamento.

? Como o Monostate faz várias instâncias se comportarem como uma só?
+ Tornando as variáveis estáticas (e os métodos não estáticos)
- Tornando o construtor privado
- Usando herança múltipla
- Usando reflexão
> O estado é compartilhado por todas as instâncias.

? Qual é um problema da implementação ingênua `if (instancia == null) instancia = new ...`?
+ Em ambiente multi-thread pode criar duas instâncias
- Não compila em C#
- É lenta demais
- Exige herança
> Use inicialização estática ou Lazy<T>.

? Qual a vantagem de transparência do Monostate?
+ Os usuários criam e usam o objeto como qualquer outro, sem saber que é monostate
- O objeto nunca é destruído
- Só pode haver uma instância
- Dispensa testes
> Em contraste, o Singleton exige chamar Instance.

? Por que o Singleton dificulta os testes?
+ Cria acoplamento a estado global; não dá para trocar facilmente por um falso
- Porque é abstrato
- Porque usa interfaces
- Porque é lento
> Mitigue expondo por interface e injetando.

? Qual é a estratégia "basta criar um"?
+ Criar uma instância na inicialização e passá-la a quem precisa, sem mecanismos de imposição
- Usar Singleton em todos os casos
- Criar várias e escolher uma
- Evitar objetos
> Frequentemente é a melhor opção.

? No ASP.NET Core, como se obtém uma instância única para toda a aplicação sem o padrão Singleton clássico?
+ Registrando o serviço com `AddSingleton` no contêiner de injeção de dependência
- Usando `static` em tudo
- Usando `goto`
- Reiniciando o servidor
> A singularidade vem do ciclo de vida configurado.
:::

::: cartas
Singleton :: Padrão que garante uma única instância (construtor privado + acesso estático).
Monostate :: Várias instâncias que compartilham o mesmo estado estático.
Basta criar um (Just Create One) :: Criar uma instância na inicialização e distribuí-la.
Lazy<T> :: Tipo que cria o valor no primeiro acesso, de forma thread-safe.
Estado global :: Dados acessíveis de qualquer lugar; dificulta testes e raciocínio.
Ciclo de vida singleton (DI) :: O contêiner cria uma só instância para toda a aplicação.
Gateway/Façade de dados :: Classe única que centraliza o acesso a uma fonte de dados.
:::
