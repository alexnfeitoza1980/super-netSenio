---
id: joao-cap129
order: 167
kind: capitulo
num: 129
group: C# & Mentalidade · Parte 18 · O Evangelho segundo João em C#
title: Cinco pães, dois peixes e nada se perde
subtitle: A multidão que comeu e os doze cestos que sobraram. Em C#, repartir "quanto queriam" e recolher "para que nada se perca", sem fingir que o código reproduz a multiplicação.
minutes: 75
hue: 210
objectives:
- Ler a **multiplicação dos pães** (Jo 6,5-13) frase por frase
- Transformar "repartiu-os [...] quanto queriam" e "recolhei os pedaços" em código com **fila**, **laço** e **lista**
- Verificar a regra "**para que nada se perca**": o que foi servido mais o que sobrou é igual ao total
- Saber o que o código **não** modela
origem: livros/10-evangelho-joao-csharp/content/106-cap129.md
---

::: vila A fila que engasgava
O sistema de e-mails lia 28 mil mensagens de uma fila e as entregava a cinco "trabalhadores". Se o leitor fosse rápido demais, a memória enchia; se fosse lento demais, os trabalhadores ficavam parados. Lia desenhou no quadro: "precisamos de um **cesto com tamanho limitado**: quem enche espera quando está cheio, quem esvazia espera quando está vazio. E, no fim, **conferir se nada sumiu**".
:::

::: ilustracao img/joao/cap129.svg | Cinco pães, dois peixes e nada se perde: o canal limitado faz o produtor esperar, e as sobras são recolhidas em cestos.
:::

## O texto

> **5** Levantando pois Jesus os olhos, e vendo que uma grande multidão vinha a ele, disse a Filipe: De onde comparemos pães, para que estes comam? **6** (Mas ele disse isto para o testar; pois ele bem sabia o que havia de fazer.) **7** Respondeu-lhe Filipe: Duzentos dinheiros de pão não lhes bastarão, para que cada um deles tome um pouco. **8** Disse-lhe um de seus discípulos, André, o irmão de Simão Pedro: **9** Um menino está aqui que tem cinco pães de cevada e dois peixinhos; mas que é isto entre tantos? **10** E disse Jesus: Fazei sentar as pessoas; e havia muita erva naquele lugar. Sentaram-se, pois, os homens, em número de cinco mil. **11** E tomou Jesus os pães, e havendo dado graças, repartiu-os aos discípulos, e os discípulos aos que estavam sentados, semelhantemente também dos peixes, quanto queriam. **12** E quando já estiveram fartos, disse ele a seus discípulos: Recolhei os pedaços que sobraram, para que nada se perca. **13** Então eles os recolheram, e encheram doze cestos de pedaços dos cinco pães de cevada, que sobraram aos que tinham comido.
>
> *João 6,5-13 (Bíblia Livre)*

## Lendo devagar

- Uma **grande multidão** vem a Jesus, e ele pergunta a Filipe onde comprar pães para todos (6,5). Filipe responde que nem **duzentos dinheiros** de pão bastariam (6,7).
- André diz que há ali **um menino** com **cinco pães de cevada e dois peixinhos**, e logo duvida: "que é isto entre tantos?" (6,9).
- As pessoas se sentam, **cinco mil** homens (6,10). Jesus toma os pães, dá graças e **reparte** (6,11).
- Um detalhe importante: ele reparte **"quanto queriam"**. Cada um recebe **o quanto quer**, e não uma porção fixa.
- Quando estão **fartos**, vem a ordem: **"Recolhei os pedaços que sobraram, para que nada se perca"** (6,12). Enchem **doze cestos** (6,13).

Duas regras claras no texto: **cada um recebe quanto quer** e **nada se perde**. E um fato central, que o código **não** cobre: com cinco pães e dois peixes, o texto narra que muita gente comeu até ficar farta.

## Do versículo ao código

### Passo 1: o que há para repartir, e quem está sentado

Para ter o que repartir, temos uma **lista de pedaços**, e as pessoas sentadas, cada uma com a sua **fome**. Um `record` guarda uma pessoa:

```cs
public record Pessoa(string Nome, int Fome);

var sentados = new[] { new Pessoa("André", 3), new Pessoa("Filipe", 4), new Pessoa("Tomé", 2) };
```

Aqui há **duas escolhas nossas**: os pedaços já existem no começo (a **multiplicação não é modelada**), e a "fome" é um **número** de pedaços (o texto diz "quanto queriam" e "fartos", sem contar).

Os pedaços ficam em uma **fila** (`Queue`): o primeiro que entra é o primeiro que sai, como quem tira da cesta um de cada vez.

### Passo 2: "repartiu-os [...] quanto queriam"

Para cada pessoa sentada, entregue pedaços **até a fome dela acabar ou até os pedaços acabarem**:

```cs
foreach (var pessoa in sentados)
{
    int recebeu = 0;
    while (recebeu < pessoa.Fome && Sobra.Count > 0)
    {
        Sobra.Dequeue();            // tira um pedaço da fila e entrega
        recebeu++;
    }
    Servido[pessoa.Nome] = recebeu;
}
```

O `while` tem **duas condições**: a pessoa ainda tem fome (`recebeu < pessoa.Fome`) **e** ainda há pedaços (`Sobra.Count > 0`). O "quanto queriam" do texto está na primeira; o "enquanto houver" está na segunda.

### Passo 3: "Recolhei os pedaços que sobraram"

Depois que todos foram servidos, o que **ficou na fila** é o que sobrou. Recolher é esvaziar a fila **guardando** o que havia nela:

```cs
public List<string> Recolher()
{
    var recolhidos = Sobra.ToList();     // copia o que sobrou para uma lista
    Sobra.Clear();                       // e esvazia a fila
    return recolhidos;
}
```

### Passo 4: "para que nada se perca"

Esta regra dá para **conferir por conta**: o que foi servido **mais** o que foi recolhido tem que ser **igual ao total** de pedaços. Se sobrar ou faltar algo, o programa tem um erro.

```cs
servido + recolhidos == total       // nada se perdeu?
```

### O código completo, rodando

```cs Jornada/Joao/Texto/Cap129Paes.cs
namespace Jornada.Joao.Texto;

// Jo 6,9-13 (Bíblia Livre): "Um menino está aqui que tem cinco pães de cevada e dois peixinhos [...]
//  [Jesus] repartiu-os aos discípulos, e os discípulos aos que estavam sentados, semelhantemente também dos peixes, quanto queriam.
//  [...] Recolhei os pedaços que sobraram, para que nada se perca. [...] encheram doze cestos."
//
// TEXTO     : cinco pães e dois peixes bastam para cerca de cinco mil; cada um recebe quanto quer; recolhe-se o que sobra.
// ANALOGIA  : só a DISTRIBUIÇÃO ("quanto queriam") e a CONTA que fecha ("para que nada se perca").
// LIMITE    : o código NÃO modela a multiplicação, que é o centro do relato. Ele começa com os pedaços já existentes.
//             "Fome" aqui é um número de pedaços, escolha nossa: o texto fala em "quanto queriam" e em "fartos", sem contar.

public record Pessoa(string Nome, int Fome);

// <repartir>
public class Distribuicao
{
    public Dictionary<string, int> Servido { get; } = new();
    public Queue<string> Sobra { get; }

    public Distribuicao(IEnumerable<string> pedacos) => Sobra = new Queue<string>(pedacos);

    // "repartiu-os [...] quanto queriam": cada um recebe até a sua fome, enquanto houver
    public void Repartir(IEnumerable<Pessoa> sentados)
    {
        foreach (var pessoa in sentados)
        {
            int recebeu = 0;
            while (recebeu < pessoa.Fome && Sobra.Count > 0)
            {
                Sobra.Dequeue();
                recebeu++;
            }
            Servido[pessoa.Nome] = recebeu;
        }
    }

    // "Recolhei os pedaços que sobraram, para que nada se perca"
    public List<string> Recolher()
    {
        var recolhidos = Sobra.ToList();
        Sobra.Clear();
        return recolhidos;
    }
}
// </repartir>
```

```bash
dotnet run --project codigo/joao/Joao.Console -- exemplo t129
```

```cs Uso
var pedacos = Enumerable.Range(1, 12).Select(i => $"pedaço {i}");      // o que havia para repartir (a multiplicação NÃO é modelada)
var distribuicao = new Distribuicao(pedacos);

var sentados = new[] { new Pessoa("André", 3), new Pessoa("Filipe", 4), new Pessoa("Tomé", 2) };
distribuicao.Repartir(sentados);

foreach (var (nome, recebeu) in distribuicao.Servido)
    Console.WriteLine($"{nome} recebeu {recebeu}");

var recolhidos = distribuicao.Recolher();
Console.WriteLine($"Sobraram e foram recolhidos: {recolhidos.Count}");
Console.WriteLine($"Nada se perdeu? {distribuicao.Servido.Values.Sum() + recolhidos.Count == 12}");
```

```saida
André recebeu 3
Filipe recebeu 4
Tomé recebeu 2
Sobraram e foram recolhidos: 3
Nada se perdeu? True
```

Eram 12 pedaços e três pessoas: André recebeu 3, Filipe 4, Tomé 2, **9 no total**; **3 sobraram** e foram recolhidos. `9 + 3 = 12`: **nada se perdeu**. (O exemplo é pequeno de propósito, para dar para conferir de cabeça.)

::: dica Testar a regra
Esse tipo de regra ("a soma sempre fecha") é muito útil em programas: contas bancárias, estoques, pedidos. Dá para escrever um teste que a verifica com **várias combinações** de fome e de pedaços. O teste `Nada_se_perde_servidos_mais_recolhidos_e_o_total` faz isso.
:::

::: cuidado Limite da analogia
- **O código não modela a multiplicação**, que é o centro do relato. Ele **começa** com os pedaços já existentes e só cobre a **distribuição** e o **recolhimento**.
- **"Fome" como número** e "pedaços" como itens iguais são **simplificações nossas**. O texto fala de pães e peixes diferentes, de "quanto queriam" e de "fartos".
- O exemplo tem **3 pessoas e 12 pedaços**; o texto fala de cerca de **cinco mil** e **doze cestos**, e não há "cestos" no código.
- A regra "nada se perde" no código é uma **conta** (soma). No texto, é uma **ordem** de Jesus (6,12), e a conta é nossa.
:::

::: curiosidade Lupa no grego
- **ἄρτος** (*artos*, "pão") aparece **24 vezes**, **21** no capítulo 6 (`onde artos`).
- **παιδάριον** (*paidarion*, "menino", G3808) aparece **uma única vez** em João: 6,9 (`onde paidarion`).
- **ὀψάριον** (*opsarion*, "peixe", G3795) aparece **5 vezes**: 6,9 e 6,11 e, na refeição de 21,9.10.13 (`onde opsarion`).
- **κλάσμα** (*klasma*, "pedaço", G2801) aparece **2 vezes** (6,12-13) e **κόφινος** (*kophinos*, "cesto", G2894) **1** (6,13).
- Em 6,12, o texto grego traz *synagagete ta perisseusanta klasmata, hina mē ti apolētai*: "recolhei os pedaços que sobraram, para que nada se perca" (SBLGNT). **συνάγω** ("reunir, recolher") aparece **7 vezes** e **ἀπόλλυμι** ("perder, destruir", G622) **10** (`onde synagō`, `onde apollymi`).

*Fontes: texto grego SBLGNT (CC BY 4.0); contagens por lema do MorphGNT (CC BY-SA 3.0), reproduzíveis com `dotnet run --project codigo/joao/Joao.Console -- onde <palavra>`; sentidos do dicionário de Strong (OpenScriptures), com a entrada indicada.*
:::


## Indo além: cestos de tamanho limitado e tarefas ao mesmo tempo

No passo a passo, uma pessoa de cada vez é servida. Na vida real (e no texto, com cinco mil), há **muita gente ao mesmo tempo**. As seções abaixo mostram como C# organiza isso: um **canal** (`Channel<T>`) que funciona como um cesto de tamanho limitado entre quem produz e quem consome, e uma sequência assíncrona de porções.

::: figura O canal limitado
<svg viewBox="0 0 740 230" role="img" aria-label="Um produtor coloca porções num canal de capacidade três; cinco consumidores retiram; quando o canal está cheio, o produtor espera">
  <rect x="20" y="82" width="140" height="66" rx="14" class="f-info"/><text x="90" y="110" text-anchor="middle" font-weight="800">Produtor</text><text x="90" y="130" text-anchor="middle" font-size="11" class="t-dim">PartirAsync (28 porções)</text>
  <path class="arrow" d="M162 115H250"/>
  <rect x="252" y="76" width="220" height="78" rx="16" class="f-warn"/>
  <g class="f-prim"><circle cx="300" cy="115" r="18"/><circle cx="352" cy="115" r="18"/><circle cx="404" cy="115" r="18"/></g>
  <text x="362" y="62" text-anchor="middle" font-weight="800">Canal (capacidade 3)</text>
  <text x="362" y="176" text-anchor="middle" font-size="12" class="t-dim">cheio? o produtor espera</text>
  <path class="arrow" d="M474 100L560 44"/><path class="arrow" d="M474 108L560 82"/><path class="arrow" d="M474 115L560 118"/><path class="arrow" d="M474 122L560 154"/><path class="arrow" d="M474 130L560 192"/>
  <g font-size="12" text-anchor="middle" class="f-good">
    <rect x="562" y="26" width="150" height="32" rx="10"/><rect x="562" y="64" width="150" height="32" rx="10"/><rect x="562" y="102" width="150" height="32" rx="10"/><rect x="562" y="140" width="150" height="32" rx="10"/><rect x="562" y="178" width="150" height="32" rx="10"/>
  </g>
  <g font-size="12" text-anchor="middle"><text x="637" y="47">pessoa 1 · fome 2</text><text x="637" y="85">pessoa 2 · fome 2</text><text x="637" y="123">pessoa 3 · fome 2</text><text x="637" y="161">pessoa 4 · fome 2</text><text x="637" y="199">pessoa 5 · fome 2</text></g>
</svg>
:::

### 1) Produzir um pedaço de cada vez

Na etapa 1, `PartirAsync` é um **iterador assíncrono**: parece um método com `yield return`, mas é `async` e devolve `IAsyncEnumerable<Porcao>`. Entre uma porção e outra ele faz um `await Task.Yield()`, que **cede a vez** (como quem parte o pão na hora). Quem consome usa **`await foreach`**, que espera cada item sem travar o programa.

O atributo `[EnumeratorCancellation]` faz o `CancellationToken` passado via `WithCancellation(ct)` chegar até dentro do método. Sem ele, o cancelamento não chegaria ao iterador.

### 2) O cesto com tamanho: `Channel<T>`

Um **canal** é uma fila para comunicação entre tarefas. Em `DistribuirAsync`, criamos um canal **limitado** (`CreateBounded`) com `FullMode = Wait`. A consequência é a **contrapressão**: quando o cesto está cheio, `WriteAsync` **espera** até alguém retirar; assim, o produtor nunca enche a memória.

O desenho tem três partes:

1. **Produtor**: uma `Task` que percorre a origem com `await foreach` e escreve no canal; ao terminar, **`Writer.Complete()`**. Esse aviso é essencial: sem ele, quem espera ler **nunca saberia** que acabou. Está num `finally`, para valer mesmo com erro.
2. **Consumidores**: uma `Task` por pessoa, que roda até **sua fome** (`servidos[i] < fome`) e espera `WaitToReadAsync` quando não há nada.
3. **Recolhimento**: depois de `Task.WhenAll(pessoas)`, lê-se **o que restou** no canal com `ReadAllAsync`: as sobras. Ler também **destrava o produtor**, caso ele esteja esperando num canal cheio.

```cs Jornada/Joao/Cap129/Multidao.cs
using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace Jornada.Joao.Cap129;

public sealed record Porcao(int Numero, string Origem);

public sealed record Resultado(IReadOnlyList<int> ServidosPorPessoa, IReadOnlyList<Porcao> Sobras)
{
    public int TotalServido => ServidosPorPessoa.Sum();
}

public static class Distribuicao
{
    /// <summary>
    /// Um iterador assíncrono: entrega as porções uma a uma, esperando entre elas (como quem parte o pão na hora).
    /// IAsyncEnumerable&lt;T&gt; permite consumir com "await foreach".
    /// </summary>
    public static async IAsyncEnumerable<Porcao> PartirAsync(
        int paes, int peixes, int pedacosPorAlimento,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        int numero = 0;

        for (int p = 1; p <= paes; p++)
            for (int k = 0; k < pedacosPorAlimento; k++)
            {
                await Task.Yield();
                ct.ThrowIfCancellationRequested();
                yield return new Porcao(++numero, $"pão {p}");
            }

        for (int p = 1; p <= peixes; p++)
            for (int k = 0; k < pedacosPorAlimento; k++)
            {
                await Task.Yield();
                ct.ThrowIfCancellationRequested();
                yield return new Porcao(++numero, $"peixe {p}");
            }
    }

    /// <summary>
    /// Um produtor enche um canal com capacidade limitada (o cesto); várias pessoas retiram até ficarem satisfeitas.
    /// O que não foi comido é recolhido ao final: "para que nada se perca".
    /// </summary>
    public static async Task<Resultado> DistribuirAsync(
        IAsyncEnumerable<Porcao> origem,
        IReadOnlyList<int> fomes,
        int capacidadeDoCesto,
        CancellationToken ct = default)
    {
        var canal = Channel.CreateBounded<Porcao>(new BoundedChannelOptions(capacidadeDoCesto)
        {
            FullMode = BoundedChannelFullMode.Wait,    // cesto cheio: o produtor espera (contrapressão)
            SingleWriter = true
        });

        var servidos = new int[fomes.Count];

        var produtor = Task.Run(async () =>
        {
            try
            {
                await foreach (var porcao in origem.WithCancellation(ct))
                    await canal.Writer.WriteAsync(porcao, ct);
            }
            finally
            {
                canal.Writer.Complete();               // sem isso, quem espera nunca saberia que acabou
            }
        }, ct);

        var pessoas = fomes.Select((fome, i) => Task.Run(async () =>
        {
            while (servidos[i] < fome && await canal.Reader.WaitToReadAsync(ct))
            {
                if (canal.Reader.TryRead(out _))
                    servidos[i]++;
            }
        }, ct)).ToArray();

        await Task.WhenAll(pessoas);

        // Todos saciados (ou nada mais veio). O que restar no cesto são as sobras; ler também destrava o produtor.
        var sobras = new List<Porcao>();
        await foreach (var resto in canal.Reader.ReadAllAsync(ct))
            sobras.Add(resto);

        await produtor;
        return new Resultado(servidos, sobras);
    }

    /// <summary>Agrupa as sobras em cestos de tamanho fixo.</summary>
    public static IReadOnlyList<IReadOnlyList<Porcao>> EmCestos(IEnumerable<Porcao> sobras, int porCesto) =>
        sobras.Chunk(porCesto).Select(c => (IReadOnlyList<Porcao>)c).ToList();
}
```

```cs Uso
var fomes = new[] { 2, 2, 2, 2, 2 };                                    // cinco pessoas, duas porções cada

var resultado = await Distribuicao.DistribuirAsync(
    Distribuicao.PartirAsync(paes: 5, peixes: 2, pedacosPorAlimento: 4),   // 28 porções no total
    fomes,
    capacidadeDoCesto: 3);

Console.WriteLine($"servidos: {string.Join(", ", resultado.ServidosPorPessoa)} (total {resultado.TotalServido})");

var cestos = Distribuicao.EmCestos(resultado.Sobras, porCesto: 6);
Console.WriteLine($"sobras: {resultado.Sobras.Count} porções, juntadas em {cestos.Count} cestos");
foreach (var (cesto, numero) in cestos.Select((c, i) => (c, i + 1)))
    Console.WriteLine($"  cesto {numero}: {cesto.Count} porções");

Console.WriteLine($"nada se perdeu? {resultado.TotalServido + resultado.Sobras.Count == 28}");
```

```saida
servidos: 2, 2, 2, 2, 2 (total 10)
sobras: 18 porções, juntadas em 3 cestos
  cesto 1: 6 porções
  cesto 2: 6 porções
  cesto 3: 6 porções
nada se perdeu? True
```

Os números fecham: 5 pessoas × 2 porções = **10 servidas**, sobraram **18**, e `10 + 18 = 28`, o total produzido. O exemplo agrupa as sobras em **3 cestos de 6**, em escala menor que a do texto (que fala de doze cestos).

::: erro Dois erros clássicos com canais
**1) Esquecer o `Complete()`**: o consumidor espera para sempre. **2) Canal sem limite** (`CreateUnbounded`) com produtor mais rápido que consumidor: a memória só cresce. O limite é uma **decisão de projeto**: diz quanto o sistema aguenta ter "no ar" ao mesmo tempo.
:::

::: cuidado Limites desta implementação
- Porções e pessoas são **números**; o texto fala de pães e peixes diferentes e de "quanto queriam".
- Não há cancelamento por pessoa nem redistribuição se um consumidor falhar: se uma tarefa de pessoa lançar exceção, o exemplo para.
- O `Task.Yield` do produtor **simula** a espera; não representa nenhum detalhe do texto.
:::

### 3) Testes

```cs Joao.Testes/Cap129Tests.cs
using Jornada.Joao.Cap129;

namespace Joao.Testes;

public class Cap129Tests
{
    [Fact]
    public async Task Partir_entrega_pedacos_numerados_em_ordem_com_await_foreach()
    {
        var porcoes = new List<Porcao>();

        await foreach (var p in Distribuicao.PartirAsync(paes: 2, peixes: 1, pedacosPorAlimento: 2))
            porcoes.Add(p);

        Assert.Equal(6, porcoes.Count);
        Assert.Equal(Enumerable.Range(1, 6), porcoes.Select(p => p.Numero));
        Assert.Equal(new[] { "pão 1", "pão 1", "pão 2", "pão 2", "peixe 1", "peixe 1" }, porcoes.Select(p => p.Origem));
    }

    [Fact]
    public async Task Com_fartura_todos_se_saciam_e_o_resto_vira_sobras()
    {
        var fomes = new[] { 2, 2, 2, 2, 2 };                                        // 10 porções necessárias

        var resultado = await Distribuicao.DistribuirAsync(
            Distribuicao.PartirAsync(5, 2, 4), fomes, capacidadeDoCesto: 3);          // 28 porções existem

        Assert.Equal(fomes, resultado.ServidosPorPessoa);
        Assert.Equal(10, resultado.TotalServido);
        Assert.Equal(18, resultado.Sobras.Count);
    }

    [Fact]
    public async Task Nada_se_perde_servidos_mais_sobras_sao_sempre_o_total()
    {
        var resultado = await Distribuicao.DistribuirAsync(
            Distribuicao.PartirAsync(5, 2, 4), new[] { 3, 3, 3 }, capacidadeDoCesto: 2);

        Assert.Equal(28, resultado.TotalServido + resultado.Sobras.Count);

        var numeros = resultado.Sobras.Select(p => p.Numero).Distinct().Count();
        Assert.Equal(resultado.Sobras.Count, numeros);                              // nenhuma porção aparece duas vezes
    }

    [Fact]
    public async Task Na_escassez_servem_so_o_que_existe_e_ninguem_passa_do_que_precisa()
    {
        var fomes = new[] { 5, 5, 5 };                                              // 15 necessárias, 6 existem

        var resultado = await Distribuicao.DistribuirAsync(Distribuicao.PartirAsync(3, 0, 2), fomes, 2);

        Assert.Equal(6, resultado.TotalServido);
        Assert.Empty(resultado.Sobras);
        Assert.All(resultado.ServidosPorPessoa.Zip(fomes), par => Assert.True(par.First <= par.Second));
    }

    [Fact]
    public async Task Cesto_pequeno_nao_trava_nada_o_produtor_apenas_espera()
    {
        // capacidade 1: o produtor só consegue colocar uma porção por vez (contrapressão)
        var resultado = await Distribuicao.DistribuirAsync(Distribuicao.PartirAsync(2, 2, 5), new[] { 4, 4 }, capacidadeDoCesto: 1)
            .WaitAsync(TimeSpan.FromSeconds(10));

        Assert.Equal(8, resultado.TotalServido);
        Assert.Equal(12, resultado.Sobras.Count);
    }

    [Fact]
    public void Sobras_vao_para_cestos_de_tamanho_fixo()
    {
        var sobras = Enumerable.Range(1, 7).Select(n => new Porcao(n, "pão 1"));

        var cestos = Distribuicao.EmCestos(sobras, porCesto: 3);

        Assert.Equal(new[] { 3, 3, 1 }, cestos.Select(c => c.Count));
    }

    [Fact]
    public async Task Cancelar_interrompe_a_distribuicao()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            Distribuicao.DistribuirAsync(Distribuicao.PartirAsync(5, 2, 4), new[] { 2 }, 3, cts.Token));
    }
}
```

O teste **`Nada_se_perde_servidos_mais_sobras_sao_sempre_o_total`** é a verificação do invariante, com várias combinações de fome e cesto. **`Cesto_pequeno_nao_trava_nada_o_produtor_apenas_espera`** testa a contrapressão com um canal de capacidade mínima. E **`Cancelar_interrompe_a_distribuicao`** garante que o `CancellationToken` chega até o fim.

::: exercicio Quem ficou saciado?
Escreva `PessoasSaciadas(this Resultado resultado, IReadOnlyList<int> fomes)`, que diz **quantas pessoas receberam pelo menos o que precisavam**. Dica: `Zip` junta duas listas, elemento a elemento.

--- resposta
```cs Jornada/Joao/Exercicios/Cap129Saciadas.cs
using Jornada.Joao.Cap129;

namespace Jornada.Joao.Exercicios;

public static class ResultadoExtensoes
{
    /// <summary>Quantas pessoas receberam tudo o que precisavam.</summary>
    public static int PessoasSaciadas(this Resultado resultado, IReadOnlyList<int> fomes) =>
        resultado.ServidosPorPessoa.Zip(fomes, (servido, fome) => servido >= fome).Count(saciada => saciada);
}
```

`Zip` combina `ServidosPorPessoa[i]` com `fomes[i]` e devolve um `bool` para cada pessoa; `Count(saciada => saciada)` conta os verdadeiros. Em uma situação de escassez (menos porções do que a fome total), o resultado mostra quantas pessoas ficaram **sem a porção completa**.
:::

::: resumo
- "Quanto queriam": cada pessoa recebe **até a sua fome**, enquanto houver (`while` com duas condições). A **multiplicação não é modelada**.
- "Para que nada se perca": **servidos + recolhidos = total**, uma conta que o teste confere.
- Em João, **artos** (pão) aparece 24 vezes, **21 no cap. 6**; **paidarion** (o menino) aparece uma só vez; **apollymi** em 6,12 (*hina mē ti apolētai*) é o "perder" de "para que nada se perca".
- **`IAsyncEnumerable<T>`** + `await foreach` consome itens produzidos aos poucos, sem travar.
- **`Channel<T>` limitado** cria **contrapressão**: o produtor espera quando o cesto está cheio.
- `Writer.Complete()` (num `finally`) avisa que **acabou**; sem ele, quem lê espera para sempre.
- Um **invariante** (`servidos + sobras = total`) é um bom teste: a conta precisa fechar.
:::

::: quiz
? Qual regra do código corresponde a "para que nada se perca"?
+ Servidos mais recolhidos é igual ao total de pedaços
- As sobras são sempre zero
- Todos recebem igual
- A fila nunca esvazia
> É um invariante: a conta sempre fecha.

? O código reproduz a multiplicação dos pães?
+ Não: ele começa com os pedaços já existentes e só modela a distribuição e o recolhimento
- Sim, no método `Repartir`
- Sim, no `Recolher`
- Sim, na fila
> A multiplicação é o centro do relato, e não é modelada.

? Qual estrutura entrega os pedaços um de cada vez, na ordem?
+ Uma fila (`Queue`)
- Um dicionário
- Um `enum`
- Uma pilha de pratos
> `Dequeue` tira o primeiro da fila.

? Quantas das 24 ocorrências de *artos* (pão) em João estão no capítulo 6?
+ 21
- 2
- 12
- Todas
> O capítulo 6 é o do pão, de longe.

? Quantas vezes aparece *paidarion* (o menino) em João?
+ Uma única vez, em 6,9
- Dez vezes
- Nenhuma
- Em todo capítulo 6
> O menino não tem nome.

? O que é contrapressão num `Channel<T>` limitado?
+ O produtor espera quando o canal está cheio
- O consumidor lê mais rápido
- O canal descarta itens
- O canal cresce sem limite
> Evita que a memória encha.

? Por que `Writer.Complete()` está num `finally`?
+ Para avisar o fim mesmo se o produtor falhar; senão o consumidor espera para sempre
- Para deixar o código bonito
- Para limpar a memória
- Porque o C# exige
> O consumidor depende desse aviso.

? O que `[EnumeratorCancellation]` faz num iterador assíncrono?
+ Faz o `CancellationToken` passado em `WithCancellation` chegar ao método
- Cancela o método sempre
- Ordena os itens
- Evita exceções
> Sem ele o cancelamento não chega ao iterador.

? Qual é o invariante "nada se perde" do capítulo?
+ Servidos mais sobras sempre soma o total produzido
- As sobras são sempre zero
- Todos comem a mesma quantidade
- O canal fica vazio
> É o que o teste e a demonstração conferem.
:::

::: cartas
Nada se perca (Jo 6,12) :: Ordem do texto. No código, é uma conta: o que foi servido mais o que sobrou é o total.
Multiplicação (Jo 6) :: Cena dos cinco pães e dois peixes; sobram cestos "para que nada se perca".
Artos :: "Pão": 24 vezes em João, 21 no capítulo 6.
Paidarion :: "Menino": aparece só em Jo 6,9.
Apollymi :: "Perder, destruir": 10 vezes; em 6,12, perder pedaços de pão.
IAsyncEnumerable<T> :: Sequência produzida de forma assíncrona; consumida com `await foreach`.
Channel<T> :: Fila entre tarefas, com ou sem limite de capacidade.
Contrapressão :: O produtor espera quando o consumidor não dá conta.
Invariante :: Regra que precisa valer sempre; aqui, servidos + sobras = total.
:::
