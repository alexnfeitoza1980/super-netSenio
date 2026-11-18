---
id: joao-cap132
order: 170
kind: capitulo
num: 132
group: C# & Mentalidade · Parte 18 · O Evangelho segundo João em C#
title: A toalha e a bacia
subtitle: Na ceia, Jesus tira as roupas, envolve-se com uma toalha e lava os pés dos discípulos. Em C#, é uma "camada" de serviço que se veste sem mudar quem a veste.
minutes: 70
hue: 204
objectives:
- Ler o **lava-pés** (Jo 13,3-15) frase por frase
- Transformar os gestos do texto em uma lista de **atos** e a toalha em uma **camada** (padrão *decorator*)
- Entender "**também vós deveis**": o mesmo padrão serve para qualquer pessoa
- Saber a diferença entre o **exemplo** do texto e o **reuso** de código
origem: livros/10-evangelho-joao-csharp/content/109-cap132.md
---

::: vila O serviço que ficou irreconhecível
Beto começou com um repositório de tarefas de 20 linhas. Em seis meses, ele tinha **registro, tentativas, cache, métricas e validação** dentro do mesmo método, e ninguém mais ousava mexer. Bia propôs um jeito diferente: "mantenha o miolo **simples**. Cada cuidado novo vira **uma camada** que embrulha o serviço e **parece com ele por fora**". Foi como vestir um avental sem trocar a pessoa.
:::

::: ilustracao img/joao/cap132.svg | Camadas com a mesma interface: o pedido atravessa registro, tentativas e cache até o núcleo, e quem pediu não percebe a pilha.
:::

## O texto

> **3** Sabendo Jesus que o Pai já tinha lhe dado todas as coisas nas mãos, e que ele era vindo de Deus, e para Deus ele iria, **4** Levantou-se da ceia, e tirou as roupas, e tomando uma toalha, envolveu-a em si; **5** Depois pôs água em uma bacia, e começou a lavar os pés dos discípulos, e limpá-los com a toalha com que estava envolto. **6** Veio, pois, a Simão Pedro; e ele lhe disse: Senhor, tu a mim lavas meus pés? **7** Respondeu Jesus, e disse-lhe: O que eu faço tu não o sabes agora; mas depois o entenderás. **8** Disse-lhe Pedro: Nunca lavarás meus pés. Respondeu-lhe então Jesus: Se eu não te lavar, não tens parte comigo. **9** Disse-lhe Simão Pedro: Senhor, lava não só meus pés, mas também as mãos e a cabeça. **10** Disse-lhe Jesus: Aquele que está lavado não necessita lavar, a não ser os pés, mas está todo limpo. E vós limpos estais, porém não todos. **11** Porque ele bem sabia quem o trairia; por isso disse: Nem todos estais limpos. **12** Quando então, tendo eles lavado os pés, e tomado suas roupas, voltou a se sentar à mesa, e disse-lhes: Entendeis o que vos tenho feito? **13** Vós me chamais Mestre, e Senhor, e bem dizeis; que eu o sou; **14** Pois se eu, o Senhor, e o Mestre, tenho lavado vossos pés, também vós deveis lavar vossos pés uns aos outros. **15** Porque vos tenho dado exemplo, para que como eu vos tenho feito, façais vós também.
>
> *João 13,3-15 (Bíblia Livre)*

## Lendo devagar

- Durante a ceia, Jesus faz uma sequência de **gestos concretos** (13,4-5): **levanta-se**, **tira as roupas**, **toma uma toalha** e envolve-se com ela, **põe água em uma bacia**, **lava os pés** dos discípulos e **os limpa** com a toalha.
- Pedro se opõe: "Nunca lavarás meus pés" (13,8), e Jesus responde que, sem isso, ele não tem parte com ele.
- Depois, Jesus volta à mesa e explica (13,12-15): "**Vós me chamais Mestre, e Senhor, e bem dizeis; que eu o sou**". Ele **continua sendo** o Mestre e o Senhor.
- E conclui: "se eu, o Senhor, e o Mestre, tenho lavado vossos pés, **também vós deveis lavar vossos pés uns aos outros**. Porque vos tenho dado **exemplo**".

Dois pontos que dá para levar ao código: Jesus **continua sendo Mestre e Senhor** enquanto serve (isso está **no texto**, 13,13-14), e o gesto é dado como **exemplo** para os outros repetirem.

## Do versículo ao código

### Passo 1: Jesus, e quem está à mesa

Vamos começar com uma **interface**: um contrato que diz o que toda pessoa da cena tem. Toda pessoa tem um **nome** e uma lista de **atos** (o que ela faz):

```cs
public interface IPessoa
{
    string Nome { get; }
    IReadOnlyList<string> Atos();
}

public class Jesus : IPessoa
{
    public string Nome => "Jesus (Mestre e Senhor)";
    public IReadOnlyList<string> Atos() => new[] { "está à mesa" };
}
```

### Passo 2: "tirou as roupas, e tomando uma toalha, envolveu-a em si"

A toalha acrescenta **gestos**. Mas atenção ao que o texto diz em 13,13: ele **continua sendo** Mestre e Senhor. Então a toalha **não deve mudar quem a veste**: o nome continua o mesmo e a pessoa continua sendo uma `IPessoa`. Em programação, isso tem nome: **decorator**: um objeto que **tem o mesmo contrato** do que ele envolve e **acrescenta** algo.

```cs
public class ComToalha : IPessoa
{
    private readonly IPessoa pessoa;                    // quem veste a toalha

    public ComToalha(IPessoa pessoa) => this.pessoa = pessoa;

    public string Nome => pessoa.Nome;                  // continua sendo o mesmo
    // ... e os atos: os da pessoa, mais os da toalha
}
```

### Passo 3: "pôs água em uma bacia, e começou a lavar os pés dos discípulos"

Os atos da toalha são os do texto, **na mesma ordem**:

```cs
public IReadOnlyList<string> Atos() => pessoa.Atos().Concat(new[]
{
    "levanta-se da ceia",
    "tira as roupas",
    "envolve-se com uma toalha",
    "põe água em uma bacia",
    "lava os pés dos discípulos",
    "limpa-os com a toalha",
}).ToList();
```

`pessoa.Atos().Concat(...)` junta **os atos que a pessoa já fazia** com os do serviço.

### O código completo, rodando

```cs Jornada/Joao/Texto/Cap132LavaPes.cs
namespace Jornada.Joao.Texto;

// Jo 13,4-5.13-15 (Bíblia Livre): "Levantou-se da ceia, e tirou as roupas, e tomando uma toalha, envolveu-a em si; Depois pôs água em uma bacia,
//  e começou a lavar os pés dos discípulos, e limpá-los com a toalha [...] Vós me chamais Mestre, e Senhor, e bem dizeis; que eu o sou;
//  Pois se eu, o Senhor, e o Mestre, tenho lavado vossos pés, também vós deveis lavar vossos pés uns aos outros. Porque vos tenho dado exemplo [...]"
//
// TEXTO     : Jesus serve e continua sendo Mestre e Senhor (13,13-14); o gesto é dado como exemplo (13,15).
// ANALOGIA  : o decorator: uma camada que acrescenta o serviço SEM mudar quem a veste (mesmo nome, mesmo contrato).
// LIMITE    : o código não explica o sentido do gesto. A relação entre o Verbo (cap. 124) e Jesus (Jo 1,14) é uma leitura
//             cristã do próprio Evangelho que NÃO é modelada aqui: neste capítulo, Jesus é só uma pessoa à mesa (IPessoa).

public interface IPessoa
{
    string Nome { get; }
    IReadOnlyList<string> Atos();
}

public class Jesus : IPessoa
{
    public string Nome => "Jesus (Mestre e Senhor)";
    public IReadOnlyList<string> Atos() => new[] { "está à mesa" };
}

public class Discipulo : IPessoa
{
    public Discipulo(string nome) => Nome = nome;
    public string Nome { get; }
    public IReadOnlyList<string> Atos() => new[] { "está à mesa" };
}

// <toalha>
// A toalha não muda quem a veste: continua sendo uma IPessoa, com o mesmo nome. Só acrescenta o serviço.
public class ComToalha : IPessoa
{
    private readonly IPessoa pessoa;

    public ComToalha(IPessoa pessoa) => this.pessoa = pessoa;

    public string Nome => pessoa.Nome;

    public IReadOnlyList<string> Atos() => pessoa.Atos().Concat(new[]
    {
        "levanta-se da ceia",
        "tira as roupas",
        "envolve-se com uma toalha",
        "põe água em uma bacia",
        "lava os pés dos discípulos",
        "limpa-os com a toalha",
    }).ToList();
}
// </toalha>
```

```bash
dotnet run --project codigo/joao/Joao.Console -- exemplo t132
```

```cs Uso
IPessoa jesus = new Jesus();
IPessoa servindo = new ComToalha(jesus);

Console.WriteLine($"{servindo.Nome} (o mesmo nome, antes e depois da toalha):");
foreach (var ato in servindo.Atos())
    Console.WriteLine("  - " + ato);

// "também vós deveis": qualquer pessoa pode vestir a toalha
var pedro = new ComToalha(new Discipulo("Pedro"));
Console.WriteLine($"{pedro.Nome} lava os pés dos discípulos? {pedro.Atos().Contains("lava os pés dos discípulos")}");
```

```saida
Jesus (Mestre e Senhor) (o mesmo nome, antes e depois da toalha):
  - está à mesa
  - levanta-se da ceia
  - tira as roupas
  - envolve-se com uma toalha
  - põe água em uma bacia
  - lava os pés dos discípulos
  - limpa-os com a toalha
Pedro lava os pés dos discípulos? True
```

O nome continua **"Jesus (Mestre e Senhor)"**, e a lista de atos tem primeiro o que ele já fazia ("está à mesa") e depois, **na ordem de João 13,4-5**, os gestos do serviço.

### Passo 4: "também vós deveis lavar vossos pés uns aos outros"

Olhe a última linha da saída: **Pedro**, que é um `Discipulo` (outra classe!), também veste a toalha. Isso é possível porque `ComToalha` só exige **uma `IPessoa`**: não importa **quem** a veste.

::: cuidado Limite da analogia
- **"Exemplo" no texto e "reuso" no código são coisas diferentes.** Em 13,14-15, Jesus pede que os discípulos **sirvam uns aos outros**: é uma exortação **ética** e pessoal. No código, `ComToalha` funcionar para qualquer `IPessoa` é **reuso técnico**. A semelhança é só de **estrutura** (o mesmo gesto, repetido por outros); o código não diz nada sobre servir.
- **O decorator representa "continuar sendo quem é"** porque o texto o diz (13,13); o código não explica o sentido do gesto nem o que significa ser Mestre e Senhor.
- **Jesus aqui é só uma pessoa à mesa** (`IPessoa`). O capítulo 124 modelou a **Palavra** (Jo 1,1) com herança; a leitura cristã identifica a Palavra com Jesus (1,14). Este livro **não tenta unir** os dois modelos (a identificação **não** está no código).
- Em 13,10-11, o texto fala de "limpos" e de quem trairia: **não** está modelado.
:::

::: dica Onde você já viu isso
Programas usam a mesma ideia: um *stream* de arquivo envolvido em um de compressão; um serviço envolvido em outro que **registra** o que acontece, outro que **tenta de novo** se falhar, outro que **guarda em cache**. Cada camada é uma "toalha": cuida de algo, e quem usa **não percebe** que há camadas.
:::

::: curiosidade Lupa no grego
- **λέντιον** (*lention*, "toalha", G3012) aparece **2 vezes** (13,4-5); **νιπτήρ** (*niptēr*, "bacia", G3537) **1** vez (13,5) (`onde lention`, `onde niptēr`).
- **νίπτω** (*niptō*, "lavar" mãos, pés ou rosto, G3538) aparece **13 vezes**: **8** em Jo 13 e **5** em Jo 9, no cego que se lava em Siloé (`onde niptō`).
- **ὑπόδειγμα** (*hypodeigma*, "exemplo, modelo", G5262) aparece **uma só vez**: 13,15, *hypodeigma gar edōka hymin*, "porque vos tenho dado exemplo" (SBLGNT) (`onde hypodeigma`).
- **δοῦλος** (*doulos*, "servo", G1401) aparece **11 vezes** (`onde doulos`).

*Fontes: texto grego SBLGNT (CC BY 4.0); contagens por lema do MorphGNT (CC BY-SA 3.0), reproduzíveis com `dotnet run --project codigo/joao/Joao.Console -- onde <palavra>`; sentidos do dicionário de Strong (OpenScriptures), com a entrada indicada.*
:::


## Indo além: camadas de serviço de verdade

Agora o mesmo padrão em um caso do dia a dia: um repositório de tarefas, com camadas que **registram**, **tentam de novo** e **guardam em cache**. A pergunta interessante é: **a ordem das camadas importa?** (Importa, e muito.)

::: figura Camadas com a mesma interface
<svg viewBox="0 0 740 240" role="img" aria-label="Quatro camadas concêntricas: registro, tentativas, cache e, no centro, o repositório em memória; todas têm a interface IRepositorioDeTarefas">
  <rect x="20" y="20" width="440" height="200" rx="26" class="f-info"/><text x="40" y="46" font-weight="800">ComRegistro</text><text x="170" y="46" font-size="12" class="t-dim">anota o que acontece (por fora, vê só o resultado)</text>
  <rect x="50" y="62" width="380" height="140" rx="22" class="f-warn"/><text x="68" y="88" font-weight="800">ComTentativas</text><text x="200" y="88" font-size="12" class="t-dim">insiste se a falha for passageira</text>
  <rect x="80" y="104" width="320" height="82" rx="18" class="f-good"/><text x="98" y="130" font-weight="800">ComCache</text><text x="190" y="130" font-size="12" class="t-dim">evita ir ao serviço de novo</text>
  <rect x="120" y="144" width="240" height="30" rx="10" class="f-prim"/><text x="240" y="164" text-anchor="middle" font-size="13" font-weight="800">RepositorioEmMemoria</text>
  <rect x="500" y="40" width="220" height="160" rx="16" class="f-prim"/>
  <text x="610" y="72" text-anchor="middle" font-weight="800">IRepositorioDeTarefas</text>
  <text x="610" y="100" text-anchor="middle" font-size="12">ObterAsync(id)</text><text x="610" y="120" text-anchor="middle" font-size="12">SalvarAsync(tarefa)</text>
  <text x="610" y="152" text-anchor="middle" font-size="12" class="t-dim">todas as camadas</text><text x="610" y="170" text-anchor="middle" font-size="12" class="t-dim">cumprem este contrato</text>
</svg>
:::

### 1) Decorator: mesma interface, serviço extra

O **decorator** é um objeto que **implementa a mesma interface** do objeto que embrulha, **repassa o trabalho** e faz algo antes ou depois. Como quem chama só conhece a interface, não sabe se fala com o serviço de verdade ou com uma pilha de camadas. Três decoradores aparecem no capítulo:

- **`ComRegistro`**: usa a `Lanterna` do capítulo 124. Abre um **escopo** (`obter 1`), repassa a chamada, registra `encontrada` ou, se algo falhar, registra o **erro** e **relança** com `throw;` (que **preserva a pilha de chamadas**, ao contrário de `throw e;`).
- **`ComTentativas`**: repete quando a falha é **passageira**. Repare no **filtro de exceção**: `catch (FalhaPassageiraException) when (tentativa < maximo)`. Ele só captura enquanto ainda restam tentativas; na última, a exceção **escapa**.
- **`ComCache`**: guarda as leituras. `SalvarAsync` **atualiza** o cache, para nunca servir dado velho.

Os **métodos de extensão** (`ComCache()`, `ComTentativas(3)`, `ComRegistro(lanterna)`) permitem empilhar numa leitura natural.

```cs Jornada/Joao/Cap132/Servico.cs
using System.Collections.Concurrent;
using Jornada.Joao.Cap124;

namespace Jornada.Joao.Cap132;

public sealed record Tarefa(int Id, string Titulo, bool Feita);

public interface IRepositorioDeTarefas
{
    Task<Tarefa?> ObterAsync(int id, CancellationToken ct = default);
    Task SalvarAsync(Tarefa tarefa, CancellationToken ct = default);
}

public sealed class FalhaPassageiraException : Exception
{
    public FalhaPassageiraException(string mensagem) : base(mensagem) { }
}

/// <summary>O serviço de verdade, "no chão": guarda em memória e, quando pedimos, falha algumas vezes antes de funcionar.</summary>
public sealed class RepositorioEmMemoria : IRepositorioDeTarefas
{
    private readonly ConcurrentDictionary<int, Tarefa> dados = new();
    private int falhasPendentes;
    private int leituras;

    public int Leituras => Volatile.Read(ref leituras);

    public void FalharProximas(int vezes) => falhasPendentes = vezes;

    public Task<Tarefa?> ObterAsync(int id, CancellationToken ct = default)
    {
        Interlocked.Increment(ref leituras);
        TentarFalhar();
        return Task.FromResult(dados.TryGetValue(id, out var t) ? t : null);
    }

    public Task SalvarAsync(Tarefa tarefa, CancellationToken ct = default)
    {
        TentarFalhar();
        dados[tarefa.Id] = tarefa;
        return Task.CompletedTask;
    }

    private void TentarFalhar()
    {
        if (falhasPendentes > 0)
        {
            falhasPendentes--;
            throw new FalhaPassageiraException("o serviço está ocupado");
        }
    }
}

/// <summary>
/// Decorador: tem a MESMA interface do que embrulha, faz um serviço extra e repassa o trabalho.
/// Quem usa não sabe se fala com o serviço de verdade ou com uma pilha de decoradores.
/// </summary>
public sealed class ComRegistro : IRepositorioDeTarefas
{
    private readonly IRepositorioDeTarefas interno;
    private readonly ILanterna lanterna;

    public ComRegistro(IRepositorioDeTarefas interno, ILanterna lanterna)
    {
        this.interno = interno;
        this.lanterna = lanterna;
    }

    public async Task<Tarefa?> ObterAsync(int id, CancellationToken ct = default)
    {
        using var _ = lanterna.Escopo($"obter {id}");
        try
        {
            var tarefa = await interno.ObterAsync(id, ct);
            lanterna.Escrever(Nivel.Informacao, tarefa is null ? "não encontrada" : "encontrada");
            return tarefa;
        }
        catch (Exception e)
        {
            lanterna.Escrever(Nivel.Erro, "falhou", ("erro", e.Message));
            throw;
        }
    }

    public async Task SalvarAsync(Tarefa tarefa, CancellationToken ct = default)
    {
        using var _ = lanterna.Escopo($"salvar {tarefa.Id}");
        await interno.SalvarAsync(tarefa, ct);
        lanterna.Escrever(Nivel.Informacao, "salva");
    }
}

/// <summary>Insiste algumas vezes quando a falha é passageira. Humilde: ninguém vê, mas o serviço fica mais firme.</summary>
public sealed class ComTentativas : IRepositorioDeTarefas
{
    private readonly IRepositorioDeTarefas interno;
    private readonly int maximo;

    public ComTentativas(IRepositorioDeTarefas interno, int maximo)
    {
        if (maximo < 1) throw new ArgumentOutOfRangeException(nameof(maximo));
        this.interno = interno;
        this.maximo = maximo;
    }

    public Task<Tarefa?> ObterAsync(int id, CancellationToken ct = default) =>
        Repetir(() => interno.ObterAsync(id, ct));

    public Task SalvarAsync(Tarefa tarefa, CancellationToken ct = default) =>
        Repetir(async () => { await interno.SalvarAsync(tarefa, ct); return 0; });

    private async Task<T> Repetir<T>(Func<Task<T>> acao)
    {
        for (int tentativa = 1; ; tentativa++)
        {
            try { return await acao(); }
            catch (FalhaPassageiraException) when (tentativa < maximo) { }    // filtro: só repete se ainda restam tentativas
        }
    }
}

/// <summary>Guarda as leituras já feitas. Salvar invalida o item, para nunca servir um dado velho.</summary>
public sealed class ComCache : IRepositorioDeTarefas
{
    private readonly IRepositorioDeTarefas interno;
    private readonly ConcurrentDictionary<int, Tarefa> guardadas = new();

    public ComCache(IRepositorioDeTarefas interno) => this.interno = interno;

    public async Task<Tarefa?> ObterAsync(int id, CancellationToken ct = default)
    {
        if (guardadas.TryGetValue(id, out var tarefa))
            return tarefa;

        var lida = await interno.ObterAsync(id, ct);
        if (lida is not null) guardadas[id] = lida;
        return lida;
    }

    public async Task SalvarAsync(Tarefa tarefa, CancellationToken ct = default)
    {
        await interno.SalvarAsync(tarefa, ct);
        guardadas[tarefa.Id] = tarefa;
    }
}

public static class DecoradoresExtensoes
{
    // Métodos de extensão deixam empilhar em leitura natural: repositorio.ComCache().ComTentativas(3).ComRegistro(lanterna)
    public static IRepositorioDeTarefas ComRegistro(this IRepositorioDeTarefas r, ILanterna lanterna) => new ComRegistro(r, lanterna);
    public static IRepositorioDeTarefas ComTentativas(this IRepositorioDeTarefas r, int maximo) => new ComTentativas(r, maximo);
    public static IRepositorioDeTarefas ComCache(this IRepositorioDeTarefas r) => new ComCache(r);
}
```

### 2) A ordem importa

Na demonstração, o serviço "no chão" é programado para **falhar as duas próximas leituras**. A pilha é `chão → cache → tentativas → registro`, da mais interna à mais externa.

```cs Uso
var chao = new RepositorioEmMemoria();
var linhas = new List<Registro>();

IRepositorioDeTarefas repositorio = chao
    .ComCache()
    .ComTentativas(3)
    .ComRegistro(new Lanterna(Nivel.Informacao, linhas.Add, relogio));

await chao.SalvarAsync(new Tarefa(1, "Lavar os pés dos discípulos", false));   // direto no "chão"
chao.FalharProximas(2);                                                         // o serviço vai falhar duas vezes

var primeira = await repositorio.ObterAsync(1);     // falha, falha e, na terceira, funciona
var segunda = await repositorio.ObterAsync(1);      // agora vem do cache

Console.WriteLine($"tarefa: {primeira!.Titulo}");
Console.WriteLine($"leituras que chegaram ao serviço de verdade: {chao.Leituras}");
foreach (var registro in linhas)
    Console.WriteLine(Lanterna.Formatar(registro));
```

```saida
tarefa: Lavar os pés dos discípulos
leituras que chegaram ao serviço de verdade: 3
[Informacao] 09:00:00 > obter 1: encontrada
[Informacao] 09:00:00 > obter 1: encontrada
```

Leia com calma. **Três** leituras chegaram ao serviço de verdade: **duas que falharam e uma que funcionou**, porque `ComTentativas` insistiu sem que ninguém de fora percebesse. A segunda chamada da aplicação nem chegou ao chão: o cache respondeu. E o registro, a camada **mais externa**, só viu **duas leituras bem-sucedidas** (`encontrada`, `encontrada`): as falhas ficaram escondidas **dentro** de `ComTentativas`.

::: pense E se a ordem fosse outra?
Troque para `chao.ComRegistro(lanterna).ComTentativas(3).ComCache()`. Agora o registro fica **por dentro**: ele vê **cada tentativa**, inclusive as que falham, e o cache fica por fora. Qual das duas ordens é a certa? Depende do que você quer **observar**: o que a aplicação sentiu, ou o que o serviço sofreu. O teste `A_ordem_dos_decoradores_muda_o_comportamento` mostra a diferença.
:::

::: dica Decorator no dia a dia
Você já usou decoradores: `Stream` embrulha `Stream` (como `GZipStream` sobre `FileStream`), `HttpClient` usa *handlers* encadeados, e frameworks de injeção de dependência (o tema do próximo capítulo) costumam oferecer um jeito de registrá-los. Quando você ouvir "middleware", pense em decoradores empilhados.
:::

::: cuidado Limites desta implementação
- `ComTentativas` repete **sem esperar** entre uma tentativa e outra e só trata `FalhaPassageiraException`; em um serviço real, convém esperar (com *backoff*) para não sobrecarregar.
- `ComCache` guarda **sem prazo de validade**; o `ComRegistro` só enxerga o que passa por ele, e **a ordem das camadas decide o que cada uma vê**.
- O exemplo é didático: a pilha de camadas não é uma recomendação de arquitetura.
:::

### 3) Testes

```cs Joao.Testes/Cap132Tests.cs
using Jornada.Joao.Cap124;
using Jornada.Joao.Cap132;

namespace Joao.Testes;

public class Cap132Tests
{
    private static readonly Tarefa Lavar = new(1, "Lavar os pés dos discípulos", false);

    [Fact]
    public async Task Quem_usa_nao_sabe_se_fala_com_o_servico_ou_com_uma_pilha_de_decoradores()
    {
        IRepositorioDeTarefas simples = new RepositorioEmMemoria();
        IRepositorioDeTarefas empilhado = new RepositorioEmMemoria().ComCache().ComTentativas(3).ComRegistro(LanternaApagada.Instancia);

        foreach (var repositorio in new[] { simples, empilhado })
        {
            await repositorio.SalvarAsync(Lavar);
            Assert.Equal(Lavar, await repositorio.ObterAsync(1));
            Assert.Null(await repositorio.ObterAsync(99));
        }
    }

    [Fact]
    public async Task Com_tentativas_uma_falha_passageira_e_superada_sem_que_ninguem_perceba()
    {
        var chao = new RepositorioEmMemoria();
        await chao.SalvarAsync(Lavar);
        chao.FalharProximas(2);

        var repositorio = chao.ComTentativas(3);

        Assert.Equal(Lavar, await repositorio.ObterAsync(1));
        Assert.Equal(3, chao.Leituras);                     // duas falhas + uma que deu certo
    }

    [Fact]
    public async Task Esgotadas_as_tentativas_a_falha_aparece()
    {
        var chao = new RepositorioEmMemoria();
        chao.FalharProximas(5);

        await Assert.ThrowsAsync<FalhaPassageiraException>(() => chao.ComTentativas(3).ObterAsync(1));
        Assert.Equal(3, chao.Leituras);
    }

    [Fact]
    public async Task Com_cache_a_segunda_leitura_nem_chega_ao_servico()
    {
        var chao = new RepositorioEmMemoria();
        await chao.SalvarAsync(Lavar);
        var repositorio = chao.ComCache();

        await repositorio.ObterAsync(1);
        await repositorio.ObterAsync(1);

        Assert.Equal(1, chao.Leituras);
    }

    [Fact]
    public async Task Salvar_atualiza_o_cache_para_nunca_servir_dado_velho()
    {
        var repositorio = new RepositorioEmMemoria().ComCache();
        await repositorio.SalvarAsync(Lavar);
        await repositorio.ObterAsync(1);

        await repositorio.SalvarAsync(Lavar with { Feita = true });

        Assert.True((await repositorio.ObterAsync(1))!.Feita);
    }

    [Fact]
    public async Task A_ordem_dos_decoradores_muda_o_comportamento()
    {
        // Registro POR FORA do cache: registra todas as leituras. Registro POR DENTRO: só as que chegam ao serviço.
        var porFora = new List<Registro>();
        var porDentro = new List<Registro>();

        var a = new RepositorioEmMemoria().ComCache().ComRegistro(new Lanterna(Nivel.Detalhe, porFora.Add));
        var b = new RepositorioEmMemoria().ComRegistro(new Lanterna(Nivel.Detalhe, porDentro.Add)).ComCache();
        foreach (var r in new[] { a, b }) await r.SalvarAsync(Lavar);
        porFora.Clear(); porDentro.Clear();

        for (int i = 0; i < 3; i++) { await a.ObterAsync(1); await b.ObterAsync(1); }

        Assert.Equal(3, porFora.Count);                     // viu os três pedidos
        Assert.Empty(porDentro);                            // o cache respondeu antes de chegar lá
    }

    [Fact]
    public async Task O_registro_mostra_escopo_e_erro_quando_o_servico_falha()
    {
        var linhas = new List<Registro>();
        var chao = new RepositorioEmMemoria();
        chao.FalharProximas(1);
        var repositorio = chao.ComRegistro(new Lanterna(Nivel.Detalhe, linhas.Add));

        await Assert.ThrowsAsync<FalhaPassageiraException>(() => repositorio.ObterAsync(7));

        var erro = Assert.Single(linhas);
        Assert.Equal(Nivel.Erro, erro.Nivel);
        Assert.Equal(new[] { "obter 7" }, erro.Escopos);
        Assert.Equal("o serviço está ocupado", erro.Campos["erro"]);
    }
}
```

Cada decorador é testado **sozinho** (basta um "chão" falso) e depois em conjunto. **`Esgotadas_as_tentativas_a_falha_aparece`** garante que a insistência tem limite, e **`O_registro_mostra_escopo_e_erro_quando_o_servico_falha`** verifica o formato `[Erro] ... > obter 1: falhou {erro=...}`.

::: exercicio Somente leitura
Escreva o decorador `SomenteLeitura`: ele **repassa** as leituras e **recusa** qualquer escrita com `InvalidOperationException("Este repositório é somente leitura.")`. Atenção ao tipo de retorno de `SalvarAsync`.

--- resposta
```cs Jornada/Joao/Exercicios/Cap132SomenteLeitura.cs
using Jornada.Joao.Cap132;

namespace Jornada.Joao.Exercicios;

/// <summary>Um decorador que deixa ler, mas recusa qualquer escrita.</summary>
public sealed class SomenteLeitura : IRepositorioDeTarefas
{
    private readonly IRepositorioDeTarefas interno;

    public SomenteLeitura(IRepositorioDeTarefas interno) => this.interno = interno;

    public Task<Tarefa?> ObterAsync(int id, CancellationToken ct = default) => interno.ObterAsync(id, ct);

    public Task SalvarAsync(Tarefa tarefa, CancellationToken ct = default) =>
        throw new InvalidOperationException("Este repositório é somente leitura.");
}
```

`ObterAsync` apenas repassa. `SalvarAsync` devolve `Task`, mas aqui **lança de forma síncrona** (`=> throw ...`), e não uma `Task` falhada: para quem chama sem `await`, isso muda o momento do erro. Em código de produção, prefira devolver `Task.FromException(...)`, para que o erro apareça só quando alguém aguardar a tarefa.
:::

::: resumo
- A toalha é um **decorator**: `ComToalha` implementa `IPessoa` como quem veste e **só acrescenta** os atos; o nome continua o mesmo (Jo 13,13).
- "Também vós deveis": **qualquer** `IPessoa` pode vestir a toalha. O **exemplo** do texto é uma exortação ética; o **reuso** do código é só técnico.
- Em João, **lention** (toalha, 2 vezes), **niptēr** (bacia, **uma** vez) e **hypodeigma** (exemplo, **uma** vez) dão vocabulário próprio à cena; **niptō** liga o lava-pés (8) ao cego de Siloé (5).
- **Decorator**: mesma interface, serviço extra, trabalho repassado; o cliente não percebe a pilha.
- `throw;` **preserva a pilha**; `catch ... when (...)` filtra a exceção sem engoli-la.
- **A ordem dos decoradores muda o que cada um enxerga.**
- Cada camada se testa sozinha; o núcleo continua simples.
:::

::: quiz
? O que `ComToalha` mantém igual ao de quem veste?
+ O nome e o contrato (`IPessoa`)
- Os atos
- A lista de ataduras
- Nada
> O texto diz que Jesus continua Mestre e Senhor enquanto serve (13,13).

? Reusar `ComToalha` com outra pessoa é o mesmo que o "exemplo" de 13,15?
+ Não: o exemplo do texto é uma exortação a servir; o reuso do código é só uma semelhança de estrutura
- Sim, é a mesma coisa
- Sim, porque usa `Concat`
- O texto fala de classes
> O código não diz nada sobre servir.

? Quantas vezes aparece *hypodeigma* ("exemplo") em João?
+ Uma única vez, em 13,15
- 13 vezes
- Em todo o capítulo 13
- Nenhuma
> "Dei-vos um exemplo".

? Em quantos capítulos aparece o verbo *niptō* ("lavar")?
+ Em dois: 13 (8 vezes) e 9 (5 vezes)
- Só no 13
- Em cinco capítulos
- Em nenhum
> O verbo une o lava-pés e o cego de nascença.

? O que caracteriza um decorator?
+ Implementa a mesma interface do que embrulha e repassa o trabalho
- Herda do que embrulha
- Substitui o serviço
- Só funciona com cache
> O cliente não percebe a diferença.

? Qual a diferença entre `throw;` e `throw e;`?
+ `throw;` preserva a pilha de chamadas original; `throw e;` a reinicia
- Nenhuma
- `throw;` não relança
- `throw e;` é mais rápido
> Por isso se usa `throw;` num `catch`.

? O que faz o filtro `when (tentativa < maximo)`?
+ Só captura a exceção enquanto ainda há tentativas; na última, ela escapa
- Ignora a exceção
- Conta as exceções
- Muda o tipo da exceção
> O filtro decide se o `catch` entra.

? Por que a ordem dos decoradores importa?
+ Porque cada camada só enxerga o que acontece por dentro dela
- Porque o C# exige
- Porque muda a interface
- Não importa
> Registro por fora vê o resultado; por dentro vê cada tentativa.
:::

::: cartas
Toalha (Jo 13,4) :: Camada que se veste sem mudar quem a veste: o decorator.
Lava-pés (Jo 13) :: Cena em que Jesus lava os pés dos discípulos e fala de exemplo.
Lention / niptēr :: "Toalha" (2 vezes) e "bacia" (1 vez, 13,5).
Hypodeigma :: "Exemplo": uma só vez em João (13,15).
Niptō :: "Lavar": 13 vezes (8 em Jo 13, 5 em Jo 9).
Decorator :: Objeto que implementa a mesma interface do que embrulha e acrescenta um comportamento.
Filtro de exceção :: `catch (...) when (condição)`: só captura se a condição valer.
throw; :: Relança preservando a pilha de chamadas.
Middleware :: Pilha de decoradores por onde passa um pedido.
:::
