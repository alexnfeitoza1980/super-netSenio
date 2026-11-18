---
id: resiliencia-cap94
order: 120
kind: capitulo
num: 94
group: C# & Mentalidade · Parte 15 · Resiliência em C#
title: Cada história tem muitas raízes
subtitle: Contexto, lugar e momento de vida: por que uma explicação só raramente basta. Em C#, é o capítulo dos genéricos e dos delegates, com lentes que geram perguntas para qualquer tipo de situação.
minutes: 60
hue: 205
objectives:
- Entender por que **uma única explicação** raramente dá conta de uma situação
- Usar **delegates** (`Func<T, TResult>`) e **lambdas** para passar comportamento como dado
- Escrever **tipos e métodos genéricos** (`Lente<T>`, `Gerar<T>`) com restrição `where T : notnull`
- Aplicar `params`, `SelectMany` e `ToLookup`
origem: livros/06-resiliencia-csharp/content/131-cap94.md
---

::: vila A mesma chuva, calçados diferentes
Depois de uma semana de chuva forte, três pessoas chegaram atrasadas ao trabalho. Beto, que mora longe e vai de ônibus, pegou o caminho alagado. Marta, que cuidava do irmão com febre, saiu mais tarde. Lia saiu no horário, mas o aplicativo de trânsito errou a rota. O gerente pensou: "falta de compromisso". A conversa mais justa começou com outra pergunta: **"o que aconteceu no caminho de cada um?"**.
:::

::: ilustracao img/resiliencia/cap94.svg | A mesma chuva molha de formas diferentes. Cada lente mostra uma parte da história, e nenhuma delas conta tudo.
:::

## O que o livro diz

O texto sobre resiliência lembra que as dificuldades têm **causas e contextos diversos**: história pessoal, relações, ambiente, momento da vida. Em palavras nossas, **uma experiência difícil raramente tem uma só raiz**, e explicar tudo por um único fator ("é preguiça", "é falta de força de vontade") costuma ser **injusto** e **inútil**: injusto, porque deixa de fora o que pesou; inútil, porque não aponta nada que se possa mudar.

::: cuidado Contexto não é desculpa, e também não é diagnóstico
Olhar o contexto **não** significa que "ninguém responde por nada", nem que podemos **diagnosticar** a vida de alguém a partir de um fator ("deve ser trauma"). Significa **ampliar as perguntas** antes de chegar a conclusões, e aceitar que **nós mesmos** também somos vistos só por algumas lentes. Falar de experiências muito dolorosas é um trabalho para fazer **com apoio profissional**, e não com um programa.
:::

## Da vida para o código

| Na vida | Em C# |
|---|---|
| Uma "lente" que olha a situação por um ângulo | `record Lente<T>(Nome, Func<T, IEnumerable<string>> Perguntas)` |
| Perguntas que dependem da situação | **Delegate** `Func<T, ...>` e **lambdas** |
| A mesma ideia para tipos de situação diferentes | **Genéricos**: `Lente<Situacao>`, `Lente<int>`, ... |
| "Use pelo menos duas lentes" | Validação com `params` + `lentes.Length` |
| Juntar as perguntas de todas as lentes | **`SelectMany`** |
| Agrupar por lente | **`ToLookup`** |
| Detalhes que podem faltar (quando, onde) | Parâmetros opcionais `string? Quando = null` |

::: figura Uma situação, várias lentes
<svg viewBox="0 0 740 240" role="img" aria-label="Uma situação no centro e quatro lentes ao redor, cada uma gerando perguntas diferentes">
  <rect x="285" y="90" width="170" height="60" rx="14" class="f-warn"/><text x="370" y="116" text-anchor="middle" font-weight="800">Situação</text><text x="370" y="136" text-anchor="middle" font-size="12" class="t-dim">"Entreguei atrasado"</text>
  <g font-size="12" text-anchor="middle">
    <rect x="30" y="20" width="190" height="64" rx="12" class="f-info"/><text x="125" y="44" font-weight="800">Eu</text><text x="125" y="64" class="t-dim">sono, carga, experiências</text>
    <rect x="520" y="20" width="190" height="64" rx="12" class="f-good"/><text x="615" y="44" font-weight="800">Relações</text><text x="615" y="64" class="t-dim">quem estava envolvido?</text>
    <rect x="30" y="156" width="190" height="64" rx="12" class="f-prim"/><text x="125" y="180" font-weight="800">Lugar</text><text x="125" y="200" class="t-dim">ambiente, recursos</text>
    <rect x="520" y="156" width="190" height="64" rx="12" class="f-warn"/><text x="615" y="180" font-weight="800">Momento</text><text x="615" y="200" class="t-dim">o que mais acontecia?</text>
  </g>
  <path class="arrow" d="M222 62L285 100"/><path class="arrow" d="M518 62L455 100"/><path class="arrow" d="M222 178L285 140"/><path class="arrow" d="M518 178L455 140"/>
</svg>
:::

## 1) Funções como dados: `Func<>` e lambdas

Até aqui, passamos **valores** para métodos. Um **delegate** permite passar **comportamento**. `Func<Situacao, IEnumerable<string>>` quer dizer "uma função que recebe uma `Situacao` e devolve um `IEnumerable<string>`". E uma **lambda** é a forma curta de escrevê-la: `s => new[] { ... }`.

Isso resolve um problema real: cada lente precisa de uma **lógica própria** para gerar perguntas. Em vez de uma classe por lente, uma `Lente` é só **um nome** e **uma função**.

## 2) Genéricos: o mesmo código para vários tipos

Veja o `<T>` em `Lente<T>`: o **`T`** é um **tipo ainda por definir**. Uma `Lente<Situacao>` gera perguntas sobre uma `Situacao`; uma `Lente<int>` poderia gerar perguntas sobre um número. O código é escrito **uma vez** e vale para qualquer tipo, **com checagem do compilador**: não dá para colocar uma `Lente<int>` onde se espera `Lente<Situacao>`.

O método `Gerar<T>` ainda traz uma **restrição**, `where T : notnull`: a situação **não pode ser nula**. O `params Lente<T>[] lentes` permite chamar com **qualquer quantidade** de lentes separadas por vírgula.

```cs Jornada/Resiliencia/Cap94/Lentes.cs
namespace Jornada.Resiliencia.Cap94;

public sealed record Situacao(string Resumo, string? Quando = null, string? Onde = null);

/// <summary>Uma lente é um nome e uma função que, dada uma situação, gera perguntas.</summary>
public sealed record Lente<T>(string Nome, Func<T, IEnumerable<string>> Perguntas);

public static class Lentes
{
    public static IReadOnlyList<Lente<Situacao>> Padrao { get; } = new[]
    {
        new Lente<Situacao>("Eu", s => new[]
        {
            $"Como eu estava (sono, alimentação, carga de trabalho) quando \"{s.Resumo}\" aconteceu?",
            "Que experiências anteriores eu poderia reaproveitar?"
        }),

        new Lente<Situacao>("Relações", s => new[]
        {
            "Quem estava envolvido ou poderia ter ajudado?",
            "Alguém combinou algo comigo que não ficou claro?"
        }),

        new Lente<Situacao>("Lugar", s => s.Onde is null
            ? new[] { "Em que ambiente isso aconteceu? Ele facilitou ou atrapalhou?" }
            : new[] { $"Como era estar em \"{s.Onde}\"? Havia barulho, pressa, falta de recurso?" }),

        new Lente<Situacao>("Momento", s => s.Quando is null
            ? new[] { "Em que fase da vida ou da semana isso caiu?" }
            : new[] { $"O que mais estava acontecendo em \"{s.Quando}\"?" }),
    };
}

public static class Conversa
{
    /// <summary>Gera as perguntas de todas as lentes. Exige duas ou mais: uma só vira "a explicação".</summary>
    public static IReadOnlyList<string> Gerar<T>(T situacao, params Lente<T>[] lentes) where T : notnull
    {
        if (lentes.Length < 2)
            throw new ArgumentException("Use pelo menos duas lentes: uma só vira a explicação total.", nameof(lentes));

        return lentes
            .SelectMany(lente => lente.Perguntas(situacao).Select(p => $"[{lente.Nome}] {p}"))
            .ToList();
    }

    public static ILookup<string, string> PorLente<T>(T situacao, IEnumerable<Lente<T>> lentes) where T : notnull =>
        lentes
            .SelectMany(l => l.Perguntas(situacao).Select(p => (l.Nome, Pergunta: p)))
            .ToLookup(x => x.Nome, x => x.Pergunta);
}
```

Repare em dois detalhes de cuidado. Primeiro, `Gerar` **exige pelo menos duas lentes** ("uma só vira a explicação total"): a regra do livro virou uma **validação**. Segundo, as perguntas das lentes são **curiosas** ("o que mais estava acontecendo?") e **nenhuma** aponta um culpado (o teste `Nenhuma_pergunta_padrao_aponta_um_culpado` garante isso).

Já `SelectMany` junta várias listas em uma só: cada lente produz uma lista de perguntas, e `SelectMany` as **achata**. E `ToLookup` as reagrupa por nome de lente.

```cs Uso
var situacao = new Situacao("Entreguei o trabalho atrasado", Quando: "semana de provas");

foreach (var pergunta in Conversa.Gerar(situacao, Lentes.Padrao.ToArray()))
    Console.WriteLine(pergunta);
```

```saida
[Eu] Como eu estava (sono, alimentação, carga de trabalho) quando "Entreguei o trabalho atrasado" aconteceu?
[Eu] Que experiências anteriores eu poderia reaproveitar?
[Relações] Quem estava envolvido ou poderia ter ajudado?
[Relações] Alguém combinou algo comigo que não ficou claro?
[Lugar] Em que ambiente isso aconteceu? Ele facilitou ou atrapalhou?
[Momento] O que mais estava acontecendo em "semana de provas"?
```

Perceba que, sem informar `Onde`, a lente "Lugar" faz uma pergunta mais **geral**; e, informando `Quando`, a lente "Momento" a **personaliza**. Dados opcionais (`string? Quando = null`) permitem que o modelo funcione **com pouco** e melhore **com mais**.

## 3) Testes

```cs Resiliencia.Testes/Cap94Tests.cs
using Jornada.Resiliencia.Cap94;

namespace Resiliencia.Testes;

public class Cap94Tests
{
    private static readonly Situacao Atraso = new("Entreguei o trabalho atrasado");

    [Fact]
    public void Cada_lente_gera_suas_perguntas_marcadas_com_o_nome()
    {
        var perguntas = Conversa.Gerar(Atraso, Lentes.Padrao.ToArray());

        Assert.Contains(perguntas, p => p.StartsWith("[Eu]"));
        Assert.Contains(perguntas, p => p.StartsWith("[Relações]"));
        Assert.Contains(perguntas, p => p.StartsWith("[Lugar]"));
        Assert.Contains(perguntas, p => p.StartsWith("[Momento]"));
    }

    [Fact]
    public void Uma_unica_lente_e_recusada()
    {
        var e = Assert.Throws<ArgumentException>(() => Conversa.Gerar(Atraso, Lentes.Padrao[0]));
        Assert.Contains("duas lentes", e.Message);
    }

    [Fact]
    public void Quando_e_onde_opcionais_mudam_as_perguntas()
    {
        var com = new Situacao("Perdi a reunião", Quando: "semana de provas", Onde: "casa");
        var lugar = Conversa.PorLente(com, Lentes.Padrao)["Lugar"].Single();
        var momento = Conversa.PorLente(com, Lentes.Padrao)["Momento"].Single();

        Assert.Contains("casa", lugar);
        Assert.Contains("semana de provas", momento);

        var sem = Conversa.PorLente(Atraso, Lentes.Padrao)["Lugar"].Single();
        Assert.DoesNotContain("casa", sem);
    }

    [Fact]
    public void Funciona_com_qualquer_tipo_porque_e_generico()
    {
        var lentes = new[]
        {
            new Lente<int>("Tamanho", n => new[] { $"{n} cabe em um passo?" }),
            new Lente<int>("Prazo", n => new[] { $"O que muda se forem {n * 2}?" }),
        };

        Assert.Equal(new[] { "[Tamanho] 3 cabe em um passo?", "[Prazo] O que muda se forem 6?" },
                     Conversa.Gerar(3, lentes));
    }

    [Fact]
    public void Nenhuma_pergunta_padrao_aponta_um_culpado()
    {
        var todas = Conversa.Gerar(Atraso, Lentes.Padrao.ToArray());
        Assert.DoesNotContain(todas, p => p.Contains("culpa", StringComparison.OrdinalIgnoreCase));
    }
}
```

O teste **`Funciona_com_qualquer_tipo_porque_e_generico`** mostra a utilidade dos genéricos: a mesma `Conversa.Gerar` funciona com lentes de `int`, sem uma linha nova de código.

::: dica Genéricos que você já usa
`List<T>`, `Dictionary<TKey, TValue>`, `IEnumerable<T>`, `Func<T, TResult>`, `Task<T>`: todos são genéricos. Quando você escreve `List<string>`, está dizendo "uma lista cujo `T` é `string`". Entender `<T>` é entender metade da biblioteca padrão do .NET.
:::

::: exercicio A pergunta do dia
Escreva `ConversaDoDia.PerguntaDoDia<T>(T situacao, IReadOnlyList<Lente<T>> lentes, DateOnly dia)`, que devolve **uma** das perguntas geradas por `Conversa.Gerar`, escolhida pelo dia (use `dia.DayNumber % quantidade`). O mesmo dia deve dar sempre a mesma pergunta, e dias diferentes, perguntas diferentes.

--- resposta
```cs Jornada/Resiliencia/Exercicios/Cap94Dia.cs
namespace Jornada.Resiliencia.Cap94;

public static class ConversaDoDia
{
    /// <summary>Escolhe uma pergunta para hoje. O mesmo dia sempre dá a mesma pergunta.</summary>
    public static string PerguntaDoDia<T>(T situacao, IReadOnlyList<Lente<T>> lentes, DateOnly dia) where T : notnull
    {
        var todas = Conversa.Gerar(situacao, lentes.ToArray());
        return todas[dia.DayNumber % todas.Count];
    }
}
```

`DayNumber` é o número de dias desde 01/01/0001, perfeito para um índice **estável**. O `%` (resto da divisão) mantém o índice dentro da lista. Como `PerguntaDoDia` é **genérico**, funciona para qualquer tipo de situação, não só para `Situacao`.
:::

::: resumo
- Uma experiência difícil raramente tem **uma só raiz**: usar várias **lentes** (eu, relações, lugar, momento) amplia as perguntas, **sem diagnosticar** ninguém.
- Um **delegate** (`Func<T, TResult>`) passa **comportamento** como dado; uma **lambda** é a forma curta de escrevê-lo.
- **Genéricos** (`Lente<T>`, `Gerar<T>`) escrevem o código uma vez para vários tipos, com checagem do compilador; `where T : notnull` restringe.
- `params` aceita quantidade variável de argumentos; `SelectMany` achata listas; `ToLookup` reagrupa.
- Parâmetros **opcionais** (`string? Quando = null`) deixam o modelo funcionar com pouco e melhorar com mais.
:::

::: quiz
? Por que usar várias lentes para olhar uma situação?
+ Porque uma única explicação costuma ser injusta e deixar de fora o que pesou
- Porque o programa precisa de mais código
- Porque lentes diferentes sempre concordam
- Para encontrar um culpado mais rápido
> Ampliar as perguntas evita conclusões apressadas.

? O que é `Func<Situacao, IEnumerable<string>>`?
+ Um delegate: uma função que recebe uma `Situacao` e devolve uma sequência de textos
- Um número inteiro
- Uma classe abstrata
- Uma coleção de situações
> Delegates permitem passar comportamento como se fosse dado.

? O que significa o `<T>` em `Lente<T>`?
+ Um tipo a ser definido por quem usa a classe: o código é genérico
- Uma tag HTML
- Uma comparação "menor que"
- Um comentário
> Com genéricos, escrevemos uma vez e funciona para vários tipos.

? O que faz `where T : notnull`?
+ Restringe `T` a tipos que não admitem `null`
- Filtra uma lista
- Converte `T` em texto
- Torna `T` obrigatório
> É uma restrição de tipo: a situação não pode ser nula.

? Para que serve `SelectMany`?
+ Juntar várias coleções (uma por item) em uma só sequência
- Ordenar uma lista
- Remover repetições
- Contar elementos
> Cada lente gera uma lista; `SelectMany` as achata.

? Contexto significa que ninguém é responsável por nada?
- Sim, o contexto explica tudo
+ Não: ampliar as perguntas não elimina responsabilidade nem autoriza diagnósticos
- Sim, quando há mais de uma lente
- Só em situações de trabalho
> Contexto amplia a conversa; não a encerra.
:::

::: cartas
Lente :: Ângulo de observação de uma situação (eu, relações, lugar, momento...).
Delegate :: Tipo que representa uma função; permite passar comportamento como dado.
Func<T, TResult> :: Delegate pronto: recebe `T` e devolve `TResult`.
Lambda :: Função curta no formato `x => expressão`.
Genérico :: Tipo ou método com parâmetro de tipo (`<T>`), válido para vários tipos.
where T : notnull :: Restrição que proíbe `T` anulável.
params :: Parâmetro que aceita um número variável de argumentos.
SelectMany :: LINQ que junta coleções aninhadas em uma única sequência.
:::
