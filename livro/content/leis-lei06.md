---
id: leis-lei06
order: 67
kind: capitulo
num: 47
group: C# & Mentalidade · Parte 12 · As 16 leis do sucesso em C#
title: Lei 6 — Explore sua imaginação
subtitle: Tudo que foi criado nasceu de uma ideia. Imaginar a API antes de implementá-la, combinar peças com LINQ, criar pequenas linguagens com records e pattern matching — e saber parar antes do exagero.
minutes: 50
hue: 280
objectives:
- Entender a lei da **imaginação** como o "laboratório" onde ideias e planos são criados
- Praticar o **"desejo primeiro"**: escrever o uso ideal de uma API e só então implementá-la (*fluent builder*)
- Usar LINQ para a imaginação **sintética**: combinar o que já existe em coisas novas
- Criar uma **mini-linguagem** (árvore de expressões) com `record` e *pattern matching*
- Equilibrar imaginação com **YAGNI** para não construir o que ninguém pediu
origem: livros/03-leis-do-sucesso-csharp/content/66-lei06.md
---

::: vila A API dos sonhos
Lia precisa gerar receitas de culinária para um aplicativo de cozinha. A primeira ideia que vem é uma classe com onze parâmetros no construtor. Ela fecha os olhos e imagina: *"Como eu gostaria de escrever isso?"* — e rabisca, num arquivo de texto, **o código que queria poder escrever**. Só depois começa a fazer esse código existir. A API ficou tão natural que os colegas começaram a usá-la sem ler documentação.
:::

::: ilustracao img/leis/lei06.svg | Imaginação: do rascunho ao protótipo, e só então ao código definitivo.
:::

## A lei, em uma frase

> Tudo que o ser humano criou começou na **imaginação**: primeiro vem a **ideia**, depois a **convicção** de que é possível, em seguida o **plano** e, por fim, a **ação**. Quem aprende a usar a imaginação — sozinho ou aliado a quem a tem desenvolvida — e a combina com iniciativa, vai a lugares que a lógica sozinha não alcança.

A lição também ensina que a imaginação tem duas faces: a **sintética** (reorganizar e combinar ideias, conceitos e planos *já existentes* em algo novo) e a **criadora** (ver o que ainda não existe); que a grande sacada **não é ver o que os outros não veem, mas pensar o que ninguém pensou sobre o que todos veem**; que *imaginar é perguntar "e se...?"*; e que, para quem imagina, **erros e fracassos viram ativos valiosos**. Os exercícios finais: **começar algo**, **criar algo na mente antes** e **manter o controle** do foco.

## Da vida para o código

| O que a lei diz | O que isso significa para quem programa |
|---|---|
| Primeiro a ideia, depois o plano | **Rascunhar o uso** da API antes de implementar |
| Criar primeiro na mente | **TDD / design por desejo**: o teste escrito primeiro é a imaginação executável |
| Imaginação sintética | **Combinar peças** existentes: LINQ, composição, `SelectMany` |
| Imaginação criadora | **Novas abstrações**: DSLs, *fluent APIs*, mini-linguagens |
| "E se...?" | **Protótipos descartáveis** (*spikes*) para explorar |
| Erros viram ativos | **Aprender** com protótipos falhos (Lei 14) |
| Manter o controle | **YAGNI**: imagine à vontade, construa o mínimo |

::: figura Da ideia à ação
<svg viewBox="0 0 740 150" role="img" aria-label="Ideia, convicção, plano, ação — e seus equivalentes em código">
  <g font-size="13">
    <rect class="f-prim" x="10" y="20" width="160" height="60" rx="12"/><text x="90" y="48" text-anchor="middle" font-weight="800">1 · Ideia</text><text x="90" y="68" text-anchor="middle" font-size="11" class="t-dim">rascunho do uso</text>
    <rect class="f-info" x="190" y="20" width="160" height="60" rx="12"/><text x="270" y="48" text-anchor="middle" font-weight="800">2 · Convicção</text><text x="270" y="68" text-anchor="middle" font-size="11" class="t-dim">protótipo/spike</text>
    <rect class="f-good" x="370" y="20" width="160" height="60" rx="12"/><text x="450" y="48" text-anchor="middle" font-weight="800">3 · Plano</text><text x="450" y="68" text-anchor="middle" font-size="11" class="t-dim">testes e design</text>
    <rect class="f-warn" x="550" y="20" width="180" height="60" rx="12"/><text x="640" y="48" text-anchor="middle" font-weight="800">4 · Ação</text><text x="640" y="68" text-anchor="middle" font-size="11" class="t-dim">implementar e entregar</text>
  </g>
  <path class="arrow" d="M172 50H188"/><path class="arrow" d="M352 50H368"/><path class="arrow" d="M532 50H548"/>
  <text x="370" y="125" text-anchor="middle" class="t-dim" font-size="12">Imagine à vontade nas etapas 1 e 2; construa o mínimo nas etapas 3 e 4.</text>
</svg>
:::

## 1) Imagine primeiro o uso: o *fluent builder*

Comece **pelo fim**: escreva, como se já existisse, o código que gostaria de usar.

```cs Uso (rascunho)
// O que eu GOSTARIA de escrever (ainda nada compila):
var receita = ConstrutorDeReceita.Para("Bolo de cenoura")
    .Serve(8)
    .Com("cenoura", 3, "un")
    .Com("farinha", 2, "xícaras")
    .Com("ovos", 3, "un")
    .Passo("Bata a cenoura com os ovos no liquidificador")
    .Passo("Misture a farinha e leve ao forno a 180 °C por 40 min")
    .Montar();
```

Esse rascunho já é uma **especificação**: diz que a receita tem título, porções, ingredientes e passos; que a construção é encadeada, que o resultado é imutável. Agora a "ação" — fazer o rascunho funcionar:

```cs Jornada/Lei06/Receita.cs
using System.Globalization;
using System.Text;

namespace Jornada.Lei06;

public sealed record Ingrediente(string Nome, decimal Quantidade, string Unidade);

public sealed record Receita(string Titulo, int Porcoes, IReadOnlyList<Ingrediente> Ingredientes, IReadOnlyList<string> Passos)
{
    private static readonly CultureInfo Br = new("pt-BR");

    /// <summary>A mesma receita para outro número de porções (as quantidades são proporcionais).</summary>
    public Receita ParaPorcoes(int novasPorcoes)
    {
        decimal fator = (decimal)novasPorcoes / Porcoes;
        return this with
        {
            Porcoes = novasPorcoes,
            Ingredientes = Ingredientes.Select(i => i with { Quantidade = i.Quantidade * fator }).ToList()
        };
    }

    public string ComoTexto()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"{Titulo} (serve {Porcoes})");
        sb.AppendLine("Ingredientes:");
        foreach (var i in Ingredientes)
            sb.AppendLine($" - {i.Quantidade.ToString("0.##", Br)} {i.Unidade} de {i.Nome}");
        sb.AppendLine("Modo de preparo:");
        for (int n = 0; n < Passos.Count; n++)
            sb.AppendLine($" {n + 1}. {Passos[n]}");
        return sb.ToString().TrimEnd();
    }
}

public sealed class ConstrutorDeReceita
{
    private readonly string titulo;
    private int porcoes = 1;
    private readonly List<Ingrediente> ingredientes = new();
    private readonly List<string> passos = new();

    private ConstrutorDeReceita(string titulo) => this.titulo = titulo;

    public static ConstrutorDeReceita Para(string titulo)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(titulo);
        return new ConstrutorDeReceita(titulo);
    }

    public ConstrutorDeReceita Serve(int porcoes)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(porcoes);
        this.porcoes = porcoes;
        return this;
    }

    public ConstrutorDeReceita Com(string nome, decimal quantidade, string unidade)
    {
        ingredientes.Add(new Ingrediente(nome, quantidade, unidade));
        return this;
    }

    public ConstrutorDeReceita Passo(string descricao)
    {
        passos.Add(descricao);
        return this;
    }

    public Receita Montar()
    {
        if (ingredientes.Count == 0) throw new InvalidOperationException("Uma receita precisa de ingredientes.");
        if (passos.Count == 0)       throw new InvalidOperationException("Uma receita precisa de pelo menos um passo.");
        return new Receita(titulo, porcoes, ingredientes.ToList(), passos.ToList());
    }
}
```

```cs Uso
var receita = ConstrutorDeReceita.Para("Bolo de cenoura")
    .Serve(8)
    .Com("cenoura", 3, "un")
    .Com("farinha", 2, "xícaras")
    .Com("ovos", 3, "un")
    .Passo("Bata a cenoura com os ovos no liquidificador")
    .Passo("Misture a farinha e leve ao forno a 180 °C por 40 min")
    .Montar();

Console.WriteLine(receita.ComoTexto());
Console.WriteLine();
Console.WriteLine(receita.ParaPorcoes(4).ComoTexto());
```

```saida
Bolo de cenoura (serve 8)
Ingredientes:
 - 3 un de cenoura
 - 2 xícaras de farinha
 - 3 un de ovos
Modo de preparo:
 1. Bata a cenoura com os ovos no liquidificador
 2. Misture a farinha e leve ao forno a 180 °C por 40 min

Bolo de cenoura (serve 4)
Ingredientes:
 - 1,5 un de cenoura
 - 1 xícaras de farinha
 - 1,5 un de ovos
Modo de preparo:
 1. Bata a cenoura com os ovos no liquidificador
 2. Misture a farinha e leve ao forno a 180 °C por 40 min
```

::: dica Esse é o espírito do TDD
Escrever o rascunho antes do código é a mesma ideia de escrever o **teste primeiro** (cap. 17 e Lei 3): o teste é a imaginação **executável**. Ambos nos fazem projetar pela perspectiva de quem **usa**, e não de quem implementa — por isso as APIs ficam mais agradáveis (Lei 10).
:::

## 2) Imaginação sintética: combinar o que já existe

A lei diz que muita inovação é **recombinação**. LINQ é a ferramenta perfeita para perguntar *"e se juntássemos A com B?"*:

```cs Uso
var cores    = new[] { "preta", "branca", "azul" };
var tamanhos = new[] { "P", "M", "G" };

// E se oferecêssemos todas as cores em todos os tamanhos (menos azul P, que não temos)?
var catalogo =
    from cor in cores
    from tam in tamanhos
    where !(cor == "azul" && tam == "P")
    select $"camiseta {cor} {tam}";

Console.WriteLine($"{catalogo.Count()} itens. Os quatro primeiros:");
foreach (var item in catalogo.Take(4)) Console.WriteLine(" - " + item);

// E se cruzássemos o catálogo com as promoções do mês?
var promocoes = new Dictionary<string, int> { ["branca"] = 20, ["azul"] = 10 };
var comDesconto = cores.Where(promocoes.ContainsKey).Select(c => $"{c}: {promocoes[c]}% off");
Console.WriteLine("Promoções: " + string.Join(" · ", comDesconto));
```

```saida
8 itens. Os quatro primeiros:
 - camiseta preta P
 - camiseta preta M
 - camiseta preta G
 - camiseta branca P
Promoções: branca: 20% off · azul: 10% off
```

Cada `from ... from ...` é um produto cartesiano; `where` aplica a regra de negócio; `select` dá a forma final. A "invenção" está na **pergunta**, e o LINQ responde em uma linha.

## 3) Imaginação criadora: uma mini-linguagem

Às vezes, a melhor solução é **inventar uma linguagem** pequena para o seu problema. Com `record` e *pattern matching*, uma árvore de expressões leva poucas linhas — e mostra como a imaginação cria *abstrações*:

```cs Jornada/Lei06/Expressoes.cs
using System.Globalization;

namespace Jornada.Lei06;

public abstract record Expr;
public sealed record Num(double Valor) : Expr;
public sealed record Soma(Expr Esq, Expr Dir) : Expr;
public sealed record Mult(Expr Esq, Expr Dir) : Expr;
public sealed record Neg(Expr Interno) : Expr;

public static class Calculo
{
    public static double Avaliar(Expr e) => e switch
    {
        Num n  => n.Valor,
        Soma s => Avaliar(s.Esq) + Avaliar(s.Dir),
        Mult m => Avaliar(m.Esq) * Avaliar(m.Dir),
        Neg g  => -Avaliar(g.Interno),
        _      => throw new NotSupportedException(e.GetType().Name)
    };

    public static string Mostrar(Expr e) => e switch
    {
        Num n  => n.Valor.ToString(CultureInfo.InvariantCulture),
        Soma s => $"({Mostrar(s.Esq)} + {Mostrar(s.Dir)})",
        Mult m => $"{Mostrar(m.Esq)} × {Mostrar(m.Dir)}",
        Neg g  => $"-{Mostrar(g.Interno)}",
        _      => throw new NotSupportedException(e.GetType().Name)
    };

    /// <summary>E se o programa pudesse limpar expressões sozinho? x + 0 → x, 0 × x → 0, 1 × x → x…</summary>
    public static Expr Simplificar(Expr e) => e switch
    {
        Soma(var a, var b) => (Simplificar(a), Simplificar(b)) switch
        {
            (Num { Valor: 0 }, var x) => x,
            (var x, Num { Valor: 0 }) => x,
            var (x, y)                => new Soma(x, y)
        },
        Mult(var a, var b) => (Simplificar(a), Simplificar(b)) switch
        {
            (Num { Valor: 0 }, _) or (_, Num { Valor: 0 }) => new Num(0),
            (Num { Valor: 1 }, var x)                       => x,
            (var x, Num { Valor: 1 })                       => x,
            var (x, y)                                      => new Mult(x, y)
        },
        Neg(var a) => new Neg(Simplificar(a)),
        _          => e
    };
}
```

```cs Uso
// 2 + 3 × (4 + 0)
Expr expr = new Soma(new Num(2), new Mult(new Num(3), new Soma(new Num(4), new Num(0))));

Console.WriteLine($"{Calculo.Mostrar(expr)} = {Calculo.Avaliar(expr)}");

var simples = Calculo.Simplificar(expr);
Console.WriteLine($"simplificada: {Calculo.Mostrar(simples)} = {Calculo.Avaliar(simples)}");

Expr zero = new Mult(new Num(0), new Soma(new Num(7), new Num(8)));
Console.WriteLine($"{Calculo.Mostrar(zero)} → {Calculo.Mostrar(Calculo.Simplificar(zero))}");
```

```saida
(2 + 3 × (4 + 0)) = 14
simplificada: (2 + 3 × 4) = 14
0 × (7 + 8) → 0
```

É a mesma ideia por trás de mecanismos que você já usa: as **expressões do LINQ** (`Expression<Func<T,bool>>`), os **analisadores do Roslyn**, os motores de regras e as linguagens de consulta. Foi alguém que **imaginou** "e se o programa tratasse expressões como *dados*?".

::: pense Fracassos são ativos
Se a primeira versão do protótipo ficar feia, não foi tempo perdido: você **descobriu** que aquele caminho não funciona. A lei diz que, para quem imagina, adversidades e fracassos viram "bênçãos disfarçadas", pois forçam a buscar alternativas. Em engenharia, isso se chama *spike*: um experimento **descartável**, feito para **aprender**, não para entregar. Registre o que aprendeu (Lei 14) e jogue o código fora sem culpa.
:::

::: dica Rascunhando com mínimo atrito
Para testar uma ideia rápida, use um projeto de console descartável — ou, a partir do **.NET 10**, um **arquivo único** executado com `dotnet run ideia.cs`. Menos cerimônia, mais experimentos.
:::

## Imaginação com rédea: YAGNI

A lei manda "manter o controle". Para quem programa, o freio é o **YAGNI** (*You Aren't Gonna Need It* — "você não vai precisar disso"): imagine à vontade, mas **implemente apenas o que o requisito pede hoje**. Um construtor com 15 métodos "para o futuro", ou uma linguagem completa onde bastava um `if`, é imaginação sem propósito (Lei 1).

| Imaginar (barato, sem limite) | Construir (caro, com limite) |
|---|---|
| Rascunhos de uso, diagramas, conversas | Código de produção, testes, documentação |
| Protótipos descartáveis | Funcionalidades entregues |
| "E se...?" | "Qual é a menor coisa que prova a ideia?" |

::: exercicio Imagine uma potência
1. **Rascunhe** como você gostaria de escrever "2 elevado a 3" na nossa mini-linguagem.
2. Acrescente `Pot(Expr Base, int Expoente)` a `Avaliar` e `Mostrar`.
3. Em `Simplificar`, trate `x^1 → x` e `x^0 → 1`.

--- resposta
```cs Expressoes.cs (trecho)
// 1) Rascunho:  Expr dois = new Pot(new Num(2), 3);   →  "2^3 = 8"

public sealed record Pot(Expr Base, int Expoente) : Expr;

// Em Avaliar:
Pot p => Math.Pow(Avaliar(p.Base), p.Expoente),

// Em Mostrar:
Pot p => $"{Mostrar(p.Base)}^{p.Expoente}",

// Em Simplificar (antes do caso padrão):
Pot(var b, 1) => Simplificar(b),
Pot(_, 0)     => new Num(1),
Pot(var b, var n) => new Pot(Simplificar(b), n),
```
Repare como o **rascunho** orientou a implementação: o uso (`new Pot(new Num(2), 3)`) veio primeiro; o resto foi consequência. E note que o *switch* te avisa o que falta tratar quando você adiciona um novo tipo — um ponto a favor das hierarquias fechadas.
:::

::: resumo
- **Lei 6:** tudo nasce na imaginação: ideia → convicção → plano → ação; a imaginação é o laboratório de ideias e planos.
- **"Desejo primeiro":** escreva o uso ideal (*fluent API*) antes de implementar — é o TDD aplicado à API.
- **Imaginação sintética:** recombinar o que existe (LINQ, `from … from …`, composição).
- **Imaginação criadora:** novas abstrações e mini-linguagens (`record` + *pattern matching*).
- **Protótipos descartáveis (*spikes*)** transformam fracassos em aprendizado.
- **YAGNI** é a rédea: imagine livremente, construa o mínimo que serve ao propósito.
:::

::: quiz
? Qual a sequência dos estágios de realização citada na lei?
+ Ideia, convicção de que é possível, plano e ação
- Plano, ação, ideia
- Ação, ideia, plano
- Convicção, ação, plano
> Primeiro vem o pensamento; só depois a organização e a ação.

? O que significa "imaginar o uso primeiro" ao projetar uma API?
+ Escrever o código que gostaria de usar antes de implementá-lo
- Implementar tudo antes de testar
- Evitar comentários
- Copiar a API de outra biblioteca
> O rascunho é uma especificação e antecipa o TDD.

? O que é imaginação sintética em programação?
+ Combinar peças existentes em algo novo (por exemplo, com LINQ)
- Criar linguagens do zero
- Copiar código sem entender
- Apagar o histórico do Git
> Muita inovação é recombinação.

? Qual recurso do C# torna fácil representar uma árvore de expressões de forma imutável?
+ record (com herança) e pattern matching em switch expressions
- goto
- ponteiros
- dynamic
> Cada tipo de nó é um record; o switch trata cada caso.

? O que é um spike?
+ Um experimento descartável feito para aprender, não para entregar
- Um tipo de teste de carga
- Um erro de compilação
- Uma branch protegida
> Imaginar custa pouco; o spike transforma dúvida em conhecimento.

? Qual princípio freia a imaginação "desgovernada" no código?
+ YAGNI: implemente apenas o que o requisito atual pede
- DRY
- SOLID apenas
- KISS não existe
> Imagine à vontade; construa o mínimo.
:::

::: cartas
Imaginação sintética :: Combinar ideias e peças já existentes em algo novo.
Imaginação criadora :: Conceber o que ainda não existe.
Fluent API :: API que se lê como uma frase, encadeando métodos.
Spike :: Protótipo descartável para explorar uma ideia.
YAGNI :: "Você não vai precisar disso": não construa o que ninguém pediu.
record :: Tipo de dados imutável com igualdade por valor e `with`.
Pattern matching :: Verificar a forma de um valor e extrair partes dele (`switch`, `is`).
Desejo primeiro :: Escrever o uso ideal antes da implementação (TDD/design por uso).
:::
