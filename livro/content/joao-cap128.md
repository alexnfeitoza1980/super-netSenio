---
id: joao-cap128
order: 166
kind: capitulo
num: 128
group: C# & Mentalidade · Parte 18 · O Evangelho segundo João em C#
title: O poço de Jacó
subtitle: A mulher samaritana no poço fundo, a água que "voltará a ter sede" e a que se faz "fonte". Em C#, é a diferença entre buscar sempre no fundo e guardar o que já se buscou, com o cuidado de não confundir as duas coisas.
minutes: 70
hue: 212
objectives:
- Ler a conversa do **poço** (Jo 4,5-15) frase por frase
- Transformar "o poço é fundo" e "voltará a ter sede" em um código que **sempre desce ao fundo**
- Transformar "se fará nele fonte" em um código que **guarda** o que já buscou (*cache*)
- Saber por que guardar um dado **não é** a "água viva" do texto
origem: livros/10-evangelho-joao-csharp/content/105-cap128.md
---

::: vila A consulta que todo mundo repetia
O catálogo de produtos chamava o banco **a cada clique**, e a mesma consulta rodava mil vezes por minuto. Caio propôs guardar a resposta: "mas por quanto tempo?". Se guardasse para sempre, o preço antigo ficaria; se não guardasse, o banco cairia. A resposta foi: **um tempo curto, e um jeito de esquecer à força**.
:::

::: ilustracao img/joao/cap128.svg | O balde desce ao fundo uma vez; depois, enquanto a água estiver boa, o pedido se resolve à beira do poço. Quando a validade acaba, desce de novo.
:::

## O texto

> **5** Veio pois a uma cidade de Samaria, chamada Sicar, junto à propriedade que Jacó deu a seu filho José. **6** E ali estava a fonte de Jacó. Então Jesus, cansado do caminho, sentou-se assim junto à fonte; era isto quase à hora sexta. **7** Veio uma mulher de Samaria para tirar água; Disse-lhe Jesus: Dá-me de beber. **8** (Porque seus discípulos haviam ido à cidade para a comprar de comer). **9** Disse-lhe pois a mulher samaritana: Como, sendo tu judeu, me pedes de beber, que sou mulher samaritana? (Porque os judeus não se comunicam com o samaritanos.) **10** Respondeu Jesus, e disse-lhe: Se tu conhecesses o dom de Deus, e quem é o que te diz: Dá-me de beber, tu lhe pedirias, e ele te daria água viva. **11** Disse a mulher: Senhor, tu não tens com que a tirar, e o poço é fundo; de onde pois tens a água viva? **12** És tu maior que nosso pai Jacó, que nos deu o poço? E ele mesmo dele bebeu, e seus filhos, e seu gado? **13** Jesus respondeu, e disse-lhe: Todo aquele que beber desta água voltará a ter sede; **14** Porém aquele que beber da água que eu lhe der, para sempre não terá sede, mas a água que eu lhe der se fará nele fonte de água, que salte para vida eterna. **15** Disse-lhe a mulher: Senhor, dá-me desta água, para que eu não tenha mais sede, nem venha aqui para tirar.
>
> *João 4,5-15 (Bíblia Livre)*

## Lendo devagar

- Jesus, **cansado do caminho**, senta-se junto à fonte de Jacó, perto da hora sexta (4,6). Uma mulher samaritana vem **tirar água**, e ele lhe pede de beber (4,7).
- Ela se espanta, porque judeus e samaritanos não se comunicavam (4,9). Jesus fala em **água viva** (4,10).
- A mulher responde de modo prático: "tu não tens com que a tirar, e **o poço é fundo**" (4,11). (Em grego, o texto usa *pēgē*, "fonte", em 4,6, e *phrear*, "poço", em 4,11-12.)
- Jesus compara duas águas: quem bebe **desta** água **voltará a ter sede** (4,13); quem bebe da que ele der **para sempre não terá sede**, e essa água "**se fará nele fonte de água, que salte para vida eterna**" (4,14).
- A mulher pede dessa água: "para que eu **não tenha mais sede**, nem venha aqui para tirar" (4,15). Ela fala do **esforço de ir tirar**; Jesus falou de **vida eterna**.

Há um problema prático claro na fala da mulher: **tirar água do fundo custa esforço**, e a sede **volta**. Jesus fala de algo de outra ordem, e o texto deixa essa diferença à vista.

## Do versículo ao código

### Passo 1: "o poço é fundo"

O poço é uma coisa **cara de usar**: cada vez que se quer água, é preciso descer até o fundo. Em código, uma operação cara tem um **custo**; vamos contar quantas vezes se desce:

```cs
public class PocoFundo
{
    public int Descidas { get; private set; }

    public string TirarAgua()
    {
        Descidas++;                       // "o poço é fundo": cada vez custa uma descida
        return "água do poço";
    }
}
```

### Passo 2: "Todo aquele que beber desta água voltará a ter sede"

Se alguém bebe **três vezes**, desce **três vezes**:

```cs
var poco = new PocoFundo();
poco.TirarAgua(); poco.TirarAgua(); poco.TirarAgua();
// poco.Descidas == 3
```

### Passo 3: "a água que eu lhe der se fará nele fonte"

Agora a ideia que o **código** toma do texto: depois da primeira vez, a água **fica por dentro**, e não é preciso descer de novo. Em C#, isso é **guardar o resultado** em um campo e reaproveitá-lo. O operador `??=` faz exatamente isso: "se ainda não tem, busque; se já tem, use":

```cs
public class FonteDentro
{
    private readonly PocoFundo poco;
    private string? guardada;

    public FonteDentro(PocoFundo poco) => this.poco = poco;

    public string Beber() => guardada ??= poco.TirarAgua();     // "se fará nele fonte"
}
```

### O código completo, rodando

```cs Jornada/Joao/Texto/Cap128Poco.cs
namespace Jornada.Joao.Texto;

// Jo 4,11-15 (Bíblia Livre): "[...] tu não tens com que a tirar, e o poço é fundo; de onde pois tens a água viva? [...]
//  Todo aquele que beber desta água voltará a ter sede; Porém aquele que beber da água que eu lhe der, para sempre não terá sede,
//  mas a água que eu lhe der se fará nele fonte de água, que salte para vida eterna. [...] para que eu não tenha mais sede,
//  nem venha aqui para tirar."
//
// TEXTO     : duas águas: a do poço, que obriga a voltar, e a que Jesus dá ("água viva"), que se faz "fonte" em quem a bebe.
//             A mulher entende de modo prático; Jesus fala de "vida eterna".
// ANALOGIA  : só a ESTRUTURA do problema prático: buscar no fundo toda vez custa; ter por dentro evita voltar.
// LIMITE    : guardar um resultado (cache) não é a "água viva" do texto, que não é um dado nem uma economia de esforço.

// <poco>
public class PocoFundo
{
    public int Descidas { get; private set; }

    // "o poço é fundo": cada vez que se quer água, é preciso descer de novo
    public string TirarAgua()
    {
        Descidas++;
        return "água do poço";
    }
}
// </poco>

// <fonte>
public class FonteDentro
{
    private readonly PocoFundo poco;
    private string? guardada;

    public FonteDentro(PocoFundo poco) => this.poco = poco;

    // "se fará nele fonte": depois da primeira vez, a água já está por dentro e não é preciso voltar ao poço
    public string Beber() => guardada ??= poco.TirarAgua();
}
// </fonte>
```

```bash
dotnet run --project codigo/joao/Joao.Console -- exemplo t128
```

```cs Uso
var semFonte = new PocoFundo();
for (int i = 0; i < 3; i++) semFonte.TirarAgua();                       // "voltará a ter sede"
Console.WriteLine($"Sem fonte dentro: 3 goles, {semFonte.Descidas} descidas ao poço");

var poco = new PocoFundo();
var fonte = new FonteDentro(poco);
for (int i = 0; i < 3; i++) fonte.Beber();                              // "se fará nele fonte"
Console.WriteLine($"Com fonte dentro: 3 goles, {poco.Descidas} descida ao poço");
```

```saida
Sem fonte dentro: 3 goles, 3 descidas ao poço
Com fonte dentro: 3 goles, 1 descida ao poço
```

Três goles pelo poço fundo custam **três descidas**. Três goles pela fonte de dentro custam **uma**. É o desejo expresso em 4,15 ("nem venha aqui para tirar"), **na leitura da mulher**.

### Passo 4: guardar tem um preço

Guardar o que já se buscou chama-se **cache**, e é um dos recursos mais usados em programação: sites e aplicativos guardam respostas para não buscar de novo. O cuidado: **o que fica guardado pode envelhecer** (o programa continua mostrando o valor antigo). Este cuidado é **nosso** (da engenharia), e não do texto. As próximas seções constroem um cache com **prazo de validade**.

::: cuidado Limite da analogia
- **Guardar um dado não é a "água viva".** O texto fala de "vida eterna" (4,14), e a mulher entende a promessa de modo prático (não ter mais de tirar água). O código toma **só a estrutura prática** do problema: buscar no fundo toda vez custa; ter por dentro evita voltar. Ele não diz nada sobre o que Jesus quer dizer com "água viva".
- **"Fonte" no código é um campo** que guarda um texto. A "fonte" do texto "salta para vida eterna": o código não tem equivalente.
- **O envelhecimento do cache** (o dado velho) e a **validade** (que veremos abaixo) são conceitos de engenharia, sem paralelo no texto.
:::

::: curiosidade Lupa no grego
- **ὕδωρ** (*hydōr*, "água") aparece **21 vezes** em João, **9** no capítulo 4 (`onde hydōr`).
- **πηγή** (*pēgē*, "fonte, nascente", G4077) aparece **3 vezes**: duas em 4,6 ("a fonte de Jacó") e uma em 4,14. **φρέαρ** (*phrear*, "poço, cisterna", G5421) aparece **2 vezes**: 4,11 e 4,12 (`onde pēgē`, `onde phrear`). João usa palavras diferentes para o lugar de Jacó (4,6) e para o poço de que a mulher fala (4,11-12).
- **διψάω** (*dipsaō*, "ter sede") aparece **6 vezes**: 4,13.14.15; 6,35; 7,37; 19,28 (`onde dipsaō`).
- **ἀντλέω** (*antleō*, "tirar água") aparece em 4,7 e 4,15 (e em 2,8-9). **ὑδρία** aparece em 4,28: a mulher deixa o seu jarro.

*Fontes: texto grego SBLGNT (CC BY 4.0); contagens por lema do MorphGNT (CC BY-SA 3.0), reproduzíveis com `dotnet run --project codigo/joao/Joao.Console -- onde <palavra>`; sentidos do dicionário de Strong (OpenScriptures), com a entrada indicada.*
:::


## Indo além: um cache com validade

A `FonteDentro` do passo 3 guarda a água **para sempre**. Um cache de verdade precisa decidir **por quanto tempo** vale o que guardou, atender vários pedidos ao mesmo tempo sem descer duas vezes ao fundo e contar os acertos e as falhas. É o que as seções abaixo fazem.

::: figura Três caminhos de um pedido
<svg viewBox="0 0 740 230" role="img" aria-label="Um pedido ao poço pode achar o valor guardado (acerto), não achar nada e ir à fonte (falha), ou achar um valor vencido e ir à fonte de novo (expirada)">
  <rect x="20" y="86" width="110" height="52" rx="14" class="f-info"/><text x="75" y="118" text-anchor="middle" font-weight="800">Obter(chave)</text>
  <path class="arrow" d="M132 100L220 52"/><path class="arrow" d="M132 112L220 112"/><path class="arrow" d="M132 124L220 172"/>
  <rect x="222" y="26" width="270" height="52" rx="12" class="f-good"/><text x="357" y="49" text-anchor="middle" font-weight="800">Acerto</text><text x="357" y="66" text-anchor="middle" font-size="12" class="t-dim">está guardado e ainda vale</text>
  <rect x="222" y="86" width="270" height="52" rx="12" class="f-warn"/><text x="357" y="109" text-anchor="middle" font-weight="800">Falha</text><text x="357" y="126" text-anchor="middle" font-size="12" class="t-dim">não havia nada: vai à fonte</text>
  <rect x="222" y="146" width="270" height="52" rx="12" class="f-bad" opacity=".6"/><text x="357" y="169" text-anchor="middle" font-weight="800">Expirada</text><text x="357" y="186" text-anchor="middle" font-size="12" class="t-dim">havia, mas venceu: vai à fonte</text>
  <rect x="530" y="60" width="190" height="110" rx="14" class="f-prim"/><text x="625" y="92" text-anchor="middle" font-weight="800">Taxa de acerto</text><text x="625" y="116" text-anchor="middle" font-size="12">acertos ÷ pedidos</text><text x="625" y="138" text-anchor="middle" font-size="11" class="t-dim">alta = poço bem usado</text>
</svg>
:::

### 1) O poço de memória

O `Poco<TChave, TValor>` combina três ferramentas:

- **`ConcurrentDictionary`**: um dicionário seguro para vários *threads*. Usamos `AddOrUpdate`, que decide **de forma atômica** se mantém a entrada atual (ainda válida) ou a troca por uma nova.
- **`Lazy<T>`**: guarda **uma função** que será executada **no primeiro uso** e cujo resultado fica guardado. Dentro de cada entrada, o `Lazy` garante que a fonte seja chamada **uma única vez**, mesmo que cem pessoas peçam a mesma chave ao mesmo tempo.
- **`TimeProvider`**: a fonte da **hora**, injetada de fora. Nos testes, trocamos por um relógio falso e **avançamos o tempo** sem esperar de verdade.

Repare em como `Obter` descobre se foi acerto: a entrada devolvida pelo `AddOrUpdate` **é a nova** que acabamos de criar? Se sim, houve ida à fonte (falha ou expirada, conforme a chave já existisse); se não, **reaproveitamos** a antiga (acerto).

```cs Jornada/Joao/Cap128/Poco.cs
using System.Collections.Concurrent;

namespace Jornada.Joao.Cap128;

/// <summary>
/// Um poço de memória: guarda por um tempo o que já foi buscado na fonte, para não descer ao fundo toda vez.
/// A fonte é a "verdade"; o poço é uma cópia à mão, com prazo de validade.
/// </summary>
public sealed class Poco<TChave, TValor> where TChave : notnull
{
    private sealed record Entrada(Lazy<TValor> Valor, DateTimeOffset CriadaEm);

    private readonly ConcurrentDictionary<TChave, Entrada> agua = new();
    private readonly Func<TChave, TValor> fonte;
    private readonly TimeSpan validade;
    private readonly TimeProvider relogio;

    private long acertos, falhas, expiradas;

    public Poco(Func<TChave, TValor> fonte, TimeSpan validade, TimeProvider? relogio = null)
    {
        if (validade <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(validade), "A validade precisa ser positiva.");

        this.fonte = fonte;
        this.validade = validade;
        this.relogio = relogio ?? TimeProvider.System;
    }

    public long Acertos => Interlocked.Read(ref acertos);      // achou no poço
    public long Falhas => Interlocked.Read(ref falhas);        // não havia nada: foi à fonte
    public long Expiradas => Interlocked.Read(ref expiradas);  // havia, mas estava velho: foi à fonte de novo
    public int Quantidade => agua.Count;

    public TValor Obter(TChave chave)
    {
        var agora = relogio.GetUtcNow();
        bool existia = agua.ContainsKey(chave);

        // Lazy garante que a fonte seja chamada UMA vez por entrada, mesmo com várias pessoas pedindo juntas.
        var nova = new Entrada(new Lazy<TValor>(() => fonte(chave)), agora);

        var entrada = agua.AddOrUpdate(
            chave,
            nova,
            (_, atual) => agora - atual.CriadaEm < validade ? atual : nova);

        if (ReferenceEquals(entrada, nova))
        {
            if (existia) Interlocked.Increment(ref expiradas);
            else Interlocked.Increment(ref falhas);
        }
        else
        {
            Interlocked.Increment(ref acertos);
        }

        return entrada.Valor.Value;
    }

    public bool Invalidar(TChave chave) => agua.TryRemove(chave, out _);

    public void Esvaziar() => agua.Clear();
}
```

```cs Uso
var relogio = new RelogioManual();
int idasAFonte = 0;

var poco = new Poco<string, string>(
    nome => { idasAFonte++; return $"água do {nome} (busca nº {idasAFonte})"; },
    validade: TimeSpan.FromMinutes(5),
    relogio);

Console.WriteLine(poco.Obter("poço de Jacó"));
Console.WriteLine(poco.Obter("poço de Jacó"));          // vem do poço, sem ir à fonte

relogio.Avancar(TimeSpan.FromMinutes(6));
Console.WriteLine(poco.Obter("poço de Jacó"));          // venceu: vai à fonte de novo

Console.WriteLine($"acertos={poco.Acertos} · falhas={poco.Falhas} · expiradas={poco.Expiradas} · idas à fonte={idasAFonte}");
```

```saida
água do poço de Jacó (busca nº 1)
água do poço de Jacó (busca nº 1)
água do poço de Jacó (busca nº 2)
acertos=1 · falhas=1 · expiradas=1 · idas à fonte=2
```

O cenário da demonstração tem **três pedidos** e **duas idas à fonte**: o primeiro vai ao fundo (falha); o segundo acha água guardada (acerto); depois que o relógio avança além da validade, o terceiro encontra a água velha (expirada) e desce de novo. Os contadores confirmam: `acertos=1 · falhas=1 · expiradas=1 · idas à fonte=2`.

::: erro Os dois problemas difíceis
*"Há só duas coisas difíceis em ciência da computação: invalidar cache e dar nome às coisas."* (frase popular entre programadores.) A **validade** resolve o caso comum, mas dados que **mudam antes do prazo** continuam mostrando o valor velho. Por isso existe `Invalidar(chave)`. Em sistemas reais, também é preciso decidir o **tamanho máximo** do cache: este poço **cresce sem limite**, e isso é uma escolha de ensino, e não uma recomendação para produção.
:::

::: cuidado Limites desta implementação
- O `Poco` **cresce sem limite** e só descarta uma entrada vencida quando alguém a pede de novo.
- Os contadores (acertos, falhas, expiradas) são **aproximados** quando há muita concorrência: a decisão entre "falha" e "expirada" olha o dicionário antes de gravar.
- Se a fonte **lançar uma exceção**, o `Lazy<T>` a **guarda** até a entrada vencer: o pedido seguinte recebe a mesma exceção, sem tentar de novo (comportamento comprovado pelo teste `Falha_da_fonte_fica_guardada_ate_a_entrada_vencer`).
:::

### 2) Testes

```cs Joao.Testes/Cap128Tests.cs
using Jornada.Joao.Cap128;

namespace Joao.Testes;

public class Cap128Tests
{
    [Fact]
    public void Na_primeira_vez_vai_a_fonte_e_na_segunda_usa_o_poco()
    {
        int idasAFonte = 0;
        var poco = new Poco<string, int>(chave => { idasAFonte++; return chave.Length; }, TimeSpan.FromMinutes(5), new RelogioDeTeste());

        Assert.Equal(4, poco.Obter("água"));
        Assert.Equal(4, poco.Obter("água"));

        Assert.Equal(1, idasAFonte);
        Assert.Equal((1L, 1L, 0L), (poco.Acertos, poco.Falhas, poco.Expiradas));
    }

    [Fact]
    public void Passado_o_prazo_o_poco_busca_de_novo()
    {
        var relogio = new RelogioDeTeste();
        int versao = 0;
        var poco = new Poco<string, string>(_ => $"v{++versao}", TimeSpan.FromMinutes(1), relogio);

        Assert.Equal("v1", poco.Obter("a"));
        relogio.Avancar(TimeSpan.FromSeconds(59));
        Assert.Equal("v1", poco.Obter("a"));                // ainda vale
        relogio.Avancar(TimeSpan.FromSeconds(2));
        Assert.Equal("v2", poco.Obter("a"));                // venceu: nova busca

        Assert.Equal(1, poco.Expiradas);
    }

    [Fact]
    public void Chaves_diferentes_tem_entradas_diferentes()
    {
        var poco = new Poco<int, int>(n => n * n, TimeSpan.FromMinutes(1), new RelogioDeTeste());

        Assert.Equal(new[] { 4, 9, 16 }, new[] { 2, 3, 4 }.Select(poco.Obter));
        Assert.Equal(3, poco.Quantidade);
    }

    [Fact]
    public void Invalidar_obriga_a_ir_a_fonte_na_proxima()
    {
        int idas = 0;
        var poco = new Poco<string, int>(_ => ++idas, TimeSpan.FromMinutes(1), new RelogioDeTeste());

        poco.Obter("x");
        Assert.True(poco.Invalidar("x"));
        Assert.False(poco.Invalidar("x"));
        poco.Obter("x");

        Assert.Equal(2, idas);
    }

    [Fact]
    public void Cem_pedidos_ao_mesmo_tempo_chamam_a_fonte_uma_unica_vez()
    {
        int idas = 0;
        var poco = new Poco<string, int>(_ => { Interlocked.Increment(ref idas); Thread.Sleep(20); return 7; }, TimeSpan.FromMinutes(1));

        Parallel.For(0, 100, _ => Assert.Equal(7, poco.Obter("chave")));

        Assert.Equal(1, idas);                              // o Lazy cuidou de a fonte ser consultada só uma vez
        Assert.Equal(100, poco.Acertos + poco.Falhas);
    }

    [Fact]
    public void Validade_precisa_ser_positiva() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new Poco<int, int>(n => n, TimeSpan.Zero));

    [Fact]
    public void Esvaziar_limpa_tudo()
    {
        var poco = new Poco<int, int>(n => n, TimeSpan.FromMinutes(1));
        poco.Obter(1); poco.Obter(2);

        poco.Esvaziar();

        Assert.Equal(0, poco.Quantidade);
    }

    [Fact]
    public void Falha_da_fonte_fica_guardada_ate_a_entrada_vencer()
    {
        var relogio = new RelogioDeTeste();
        int tentativas = 0;
        var poco = new Poco<string, int>(_ => { tentativas++; throw new InvalidOperationException("fonte fora do ar"); }, TimeSpan.FromMinutes(1), relogio);

        Assert.Throws<InvalidOperationException>(() => poco.Obter("x"));
        Assert.Throws<InvalidOperationException>(() => poco.Obter("x"));      // a mesma exceção, sem nova tentativa
        Assert.Equal(1, tentativas);

        relogio.Avancar(TimeSpan.FromMinutes(2));
        Assert.Throws<InvalidOperationException>(() => poco.Obter("x"));
        Assert.Equal(2, tentativas);                                          // venceu: tentou de novo
    }
}
```

Os testes usam um `RelogioDeTeste` (uma subclasse de `TimeProvider` que você controla) para que **nada dependa do tempo real**. O mais interessante é **`Cem_pedidos_ao_mesmo_tempo_chamam_a_fonte_uma_unica_vez`**: ele usa `Parallel.For` e prova que o `Lazy<T>` funciona. Sem ele, os cem pedidos fariam cem idas ao fundo do poço (o problema conhecido como *cache stampede*).

::: exercicio A saúde do poço
Escreva `TaxaDeAcerto<TChave, TValor>(this Poco<TChave, TValor> poco)`, que devolve a **fração de pedidos atendidos sem ir à fonte** (acertos ÷ pedidos). Sem nenhum pedido, devolva `0`, e não uma divisão por zero.

--- resposta
```cs Jornada/Joao/Exercicios/Cap128Taxa.cs
using Jornada.Joao.Cap128;

namespace Jornada.Joao.Exercicios;

public static class PocoExtensoes
{
    /// <summary>De cada pedido, que fração foi atendida sem ir à fonte? Sem pedidos, zero (e não uma divisão por zero).</summary>
    public static double TaxaDeAcerto<TChave, TValor>(this Poco<TChave, TValor> poco) where TChave : notnull
    {
        long pedidos = poco.Acertos + poco.Falhas + poco.Expiradas;
        return pedidos == 0 ? 0 : (double)poco.Acertos / pedidos;
    }
}
```

`pedidos` soma as três contagens. O cuidado está no `pedidos == 0 ? 0 : ...`: dividir `double` por zero em C# dá `NaN` ou infinito, e **não** exceção, o que é pior, pois o erro passa despercebido. Repare também no `(double)`: sem o *cast*, a divisão de dois `long` seria inteira (1/2 daria 0).
:::

::: resumo
- "O poço é fundo": cada pedido **desce ao fundo** (custa). "Se fará nele fonte": o código toma só a **estrutura** (guardar o que já se buscou, `??=`), não o sentido da "água viva".
- Três goles pelo poço custam **3 descidas**; pela fonte de dentro, **1**.
- Em João, **hydōr** aparece 21 vezes (9 no cap. 4); o **poço** (*phrear*, 2) e a **fonte** (*pēgē*, 3) usam palavras diferentes; o **jarro** (*hydria*) aparece em Caná (2,6-7) e no poço (4,28).
- Um **cache** troca esforço por memória: guarda o que já foi buscado, por um tempo (**validade**).
- `ConcurrentDictionary.AddOrUpdate` decide de forma atômica; `Lazy<T>` garante **uma só ida à fonte** por entrada.
- `TimeProvider` permite **controlar o tempo** nos testes.
- Acompanhe **acertos, falhas e expiradas**; guarde sempre um jeito de **invalidar**, e lembre que este poço não tem limite de tamanho.
:::

::: quiz
? Quantas descidas ao poço custam três goles pela `FonteDentro`?
+ Uma
- Três
- Nenhuma
- Seis
> Depois da primeira, a água já está guardada por dentro.

? O que faz `guardada ??= poco.TirarAgua()`?
+ Só busca no poço se ainda não tem nada guardado
- Sempre busca
- Apaga o valor guardado
- Compara dois textos
> O `??=` atribui apenas quando está vazio.

? Guardar um dado (cache) é a "água viva" de Jo 4?
+ Não: o código só toma a estrutura prática do problema; o texto fala de vida eterna
- Sim, é a mesma coisa
- Sim, por causa do `??=`
- O texto fala de cache
> A mulher entende de modo prático (4,15); Jesus fala de vida eterna (4,14).

? O que mostram as palavras gregas *pēgē* e *phrear* em Jo 4?
+ João usa uma palavra para a "fonte de Jacó" (4,6) e outra para o poço de que a mulher fala (4,11-12)
- São a mesma palavra
- São dois idiomas
- São nomes de pessoas
> Strong: *pēgē* é "fonte, nascente" (G4077); *phrear* é "poço, cisterna" (G5421).

? Quantas vezes aparece *hydōr* ("água") em João?
+ 21 vezes, 9 delas no capítulo 4
- 2 vezes
- 210 vezes
- Só no capítulo 4
> Contagem feita sobre o texto grego SBLGNT.

? O que um cache guarda?
+ Valores já buscados na fonte, por um tempo
- O código-fonte
- Apenas exceções
- Os testes
> A fonte continua sendo a "verdade".

? Para que serve `Lazy<T>` dentro de cada entrada?
+ Para a fonte ser consultada uma só vez, mesmo com pedidos simultâneos
- Para deixar o código mais curto
- Para criptografar o valor
- Para ordenar chaves
> Evita o *cache stampede*.

? Por que injetar um `TimeProvider`?
+ Para os testes controlarem o tempo sem esperar
- Para o cache ficar mais rápido
- Para usar fuso horário
- Para evitar locks
> O relógio falso avança quando o teste manda.

? O que significa uma entrada "expirada"?
+ Existia, mas venceu a validade: buscou-se de novo
- Foi apagada com `Invalidar`
- Nunca existiu
- Deu erro
> Distingue-se de "falha", em que não havia nada.
:::

::: cartas
Fonte dentro (Jo 4,14) :: No código, guardar por dentro o que já se buscou (cache). Não é a "água viva" do texto.
Poço de Jacó (Jo 4) :: Cena em que a conversa passa da água do poço à "água viva".
Phrear / pēgē :: "Poço fundo" (2 vezes) e "fonte, nascente" (3 vezes).
Hydria :: "Jarro": Caná (2,6-7) e a mulher que o deixa no poço (4,28).
Cache :: Cópia à mão de um dado caro de buscar, com prazo de validade.
Validade (TTL) :: Quanto tempo um valor guardado ainda vale.
Lazy<T> :: Executa uma função só no primeiro uso e guarda o resultado.
ConcurrentDictionary :: Dicionário seguro para vários *threads*.
TimeProvider :: Fonte de hora injetável: permite controlar o tempo nos testes.
:::
