---
id: leis-lei01
order: 62
kind: capitulo
num: 42
group: C# & Mentalidade · Parte 12 · As 16 leis do sucesso em C#
title: Lei 1 — Crie um propósito
subtitle: Metas claras, específicas e atingíveis viram tipos, validações e planos de ação em C# — e testes que dizem se você chegou lá.
minutes: 45
hue: 28
objectives:
- Entender a lei do **propósito definido** e por que a falta de metas claras dissipa energia
- Modelar uma **meta** (o quê, por quê, como medir, até quando) com `record` e `required`
- Construir um **plano de ação** com passos, próximo passo e progresso
- Usar **testes de aceitação** como forma executável de "saber onde se quer chegar"
origem: livros/03-leis-do-sucesso-csharp/content/61-lei01.md
---

::: vila Lia e a lista infinita
Lia tem uma lista de "coisas para aprender": Blazor, Docker, microsserviços, Rust, machine learning, Unity… Ela começa tudo e termina nada. Na sexta-feira, olha para o repositório e vê **onze projetos pela metade**. O problema não é falta de esforço — é falta de **um propósito definido**.
:::

::: ilustracao img/leis/lei01.svg | Uma meta clara tem quatro partes: o quê, por quê, como medir e até quando.
:::

## A lei, em uma frase

> Enquanto você não souber **com clareza o que quer, por que quer, até quando e como vai medir**, sua energia se espalha em várias direções; quando você escolhe **um propósito** e organiza o esforço em torno dele, o esforço vira resultado.

O processo descrito pela lei tem quatro passos simples: **(1)** definir com clareza o que se quer; **(2)** agir de forma consistente em direção a isso; **(3)** esse esforço **organizado** se acumula em capacidade; **(4)** usar essa capacidade para alcançar o objetivo. A lei também alerta para a **indecisão**: quem hesita entre duas coisas costuma não fazer nenhuma.

## Da vida para o código

| O que a lei diz | O que isso significa para quem programa |
|---|---|
| Propósito claro, específico e atingível | **Requisitos e histórias de usuário** bem escritos; metas *SMART* |
| "O quê, por quê, quando, como" | Um tipo (`Meta`) que **não deixa existir** uma meta incompleta |
| Esforço organizado em torno do propósito | **Plano de ação**: passos ordenados, com um *próximo passo* único |
| Quem hesita não faz nenhuma das duas | **Priorizar**: um item "em andamento" por vez (voltaremos na Lei 12) |
| Saber que chegou | **Critério de aceitação** executável: um teste |
| Classes e funções "faz-tudo" | Código **sem propósito**: `Utils`, `Helper`, `Manager` |

::: figura O ciclo do propósito
<svg viewBox="0 0 740 150" role="img" aria-label="Definir, agir, esforço organizado, resultado">
  <rect class="f-prim" x="10" y="40" width="150" height="64" rx="12"/><text x="85" y="68" text-anchor="middle" font-weight="800">1 · Definir</text><text x="85" y="88" text-anchor="middle" font-size="11" class="t-dim">o quê, por quê, quando</text>
  <rect class="f-info" x="200" y="40" width="150" height="64" rx="12"/><text x="275" y="68" text-anchor="middle" font-weight="800">2 · Agir</text><text x="275" y="88" text-anchor="middle" font-size="11" class="t-dim">passo a passo</text>
  <rect class="f-good" x="390" y="40" width="150" height="64" rx="12"/><text x="465" y="68" text-anchor="middle" font-weight="800">3 · Organizar</text><text x="465" y="88" text-anchor="middle" font-size="11" class="t-dim">esforço que se acumula</text>
  <rect class="f-warn" x="580" y="40" width="150" height="64" rx="12"/><text x="655" y="68" text-anchor="middle" font-weight="800">4 · Resultado</text><text x="655" y="88" text-anchor="middle" font-size="11" class="t-dim">medido por testes</text>
  <path class="arrow" d="M162 72H198"/><path class="arrow" d="M352 72H388"/><path class="arrow" d="M542 72H578"/>
  <path d="M655 108Q655 140 360 140Q85 140 85 106" stroke="var(--ink-3)" stroke-dasharray="5 4" fill="none" marker-end="url(#arr2)"/><text x="370" y="134" text-anchor="middle" font-size="11" class="t-dim">ajustar o plano com o que aprendeu</text>
</svg>
:::

## Modelando uma meta: o tipo que não aceita vagueza

Uma meta vaga ("quero melhorar em C#") não diz **quando parar nem como saber se deu certo**. Vamos fazer o **compilador** e o construtor nos obrigarem a ser específicos. Usamos `record` (dado imutável) e `required` (o C# 11 exige que o membro seja preenchido):

```cs Jornada/Lei01/Meta.cs
namespace Jornada.Lei01;

/// <summary>Uma meta clara, específica, mensurável e com prazo.</summary>
public sealed record Meta
{
    public required string Objetivo { get; init; }   // o quê
    public required string Motivo   { get; init; }   // por quê
    public required string Medida   { get; init; }   // como vou saber que cheguei
    public required DateOnly Prazo  { get; init; }   // até quando

    /// <summary>Cria uma meta validada. O relógio entra por parâmetro: assim é testável.</summary>
    public static Meta Definir(string objetivo, string motivo, string medida, DateOnly prazo, DateOnly hoje)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(objetivo);
        ArgumentException.ThrowIfNullOrWhiteSpace(motivo);
        ArgumentException.ThrowIfNullOrWhiteSpace(medida);

        if (prazo <= hoje)
            throw new ArgumentException("O prazo precisa estar no futuro.", nameof(prazo));

        return new Meta { Objetivo = objetivo, Motivo = motivo, Medida = medida, Prazo = prazo };
    }

    public int DiasRestantes(DateOnly hoje) => Prazo.DayNumber - hoje.DayNumber;
}
```

Duas coisas para reparar:

1. Se alguém tentar `new Meta { Objetivo = "x" }` **sem** os demais membros, o código **não compila**. Um propósito incompleto sequer chega a existir.
2. O método recebe `hoje` como parâmetro. Evitar `DateTime.Now` espalhado pelo código é o que permite **testar** regras de prazo (vamos ver adiante). Isso é "propósito" também: **o código declara o que depende de quê**.

::: dica SMART em uma linha
Uma meta **S**pecific (específica), **M**easurable (mensurável), **A**chievable (atingível), **R**elevant (relevante) e **T**ime-bound (com prazo). Nosso tipo cobre o *S*, o *M* e o *T*; o *A* e o *R* são conversas com a equipe.
:::

## Do propósito ao plano de ação

Ter a meta não basta: é preciso **esforço organizado**. Um plano é uma lista de passos com prazos, e uma pergunta simples: **qual é o próximo passo?** — **um só**, para não cair na indecisão.

```cs Jornada/Lei01/PlanoDeAcao.cs
namespace Jornada.Lei01;

public sealed record Passo(string Descricao, DateOnly Prazo, bool Concluido = false);

public sealed class PlanoDeAcao
{
    private readonly List<Passo> passos = new();

    public Meta Meta { get; }
    public IReadOnlyList<Passo> Passos => passos;

    public PlanoDeAcao(Meta meta) => Meta = meta;

    public PlanoDeAcao Adicionar(string descricao, DateOnly prazo)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(descricao);
        if (prazo > Meta.Prazo)
            throw new ArgumentException("Um passo não pode vencer depois da meta.", nameof(prazo));

        passos.Add(new Passo(descricao, prazo));
        return this;                                   // interface fluente: encadeia chamadas
    }

    /// <summary>O único passo em que você deve focar agora (o mais próximo ainda aberto).</summary>
    public Passo? ProximoPasso => passos.Where(p => !p.Concluido)
                                        .OrderBy(p => p.Prazo)
                                        .FirstOrDefault();

    public void Concluir(string descricao)
    {
        int i = passos.FindIndex(p => p.Descricao == descricao);
        if (i < 0) throw new KeyNotFoundException($"Passo não encontrado: {descricao}");
        passos[i] = passos[i] with { Concluido = true };   // 'with' cria uma cópia alterada
    }

    /// <summary>Fração concluída, de 0 a 1.</summary>
    public double Progresso => passos.Count == 0 ? 0 : passos.Count(p => p.Concluido) / (double)passos.Count;
}
```

```cs Uso
var hoje = new DateOnly(2026, 11, 1);

var meta = Meta.Definir(
    objetivo: "Dominar async/await em C#",
    motivo:   "Escrever serviços que não bloqueiam threads",
    medida:   "Entregar 3 exemplos com testes e explicar o fluxo a um colega",
    prazo:    new DateOnly(2027, 1, 31),
    hoje:     hoje);

var plano = new PlanoDeAcao(meta)
    .Adicionar("Ler a documentação de Task e await", new DateOnly(2026, 11, 10))
    .Adicionar("Escrever 3 exemplos com testes",     new DateOnly(2026, 12, 15))
    .Adicionar("Explicar o fluxo a um colega",       new DateOnly(2027, 1, 20));

Console.WriteLine($"Meta: {meta.Objetivo} (faltam {meta.DiasRestantes(hoje)} dias)");
Console.WriteLine($"Próximo passo: {plano.ProximoPasso?.Descricao}");

plano.Concluir("Ler a documentação de Task e await");
Console.WriteLine($"Progresso: {plano.Progresso * 100:F0}%");
Console.WriteLine($"Próximo passo: {plano.ProximoPasso?.Descricao}");
```

```saida
Meta: Dominar async/await em C# (faltam 91 dias)
Próximo passo: Ler a documentação de Task e await
Progresso: 33%
Próximo passo: Escrever 3 exemplos com testes
```

::: pense Propósito firme, plano flexível
A lei também lembra que **ninguém lê o futuro com exatidão**: o plano precisa ser ajustado pelo caminho, sem culpa. É exatamente o espírito ágil dos capítulos 16 e 17: o **objetivo** (a meta) é estável; o **plano** (a lista de passos) muda a cada iteração, conforme o que você aprende. No nosso tipo, `Meta` é imutável, enquanto `PlanoDeAcao` aceita novos passos.
:::

## Saber que chegou: o teste como definição de pronto

"Estou chegando lá" não é um estado verificável. Um **teste de aceitação** é. Escrever o teste **antes** é a forma mais poderosa de definir o propósito de uma funcionalidade — o TDD que você viu no Laboratório 4 é a Lei 1 em ação.

```cs JornadaTests/Lei01/MetaTests.cs
using Jornada.Lei01;
using Xunit;

public class MetaTests
{
    private static readonly DateOnly Hoje = new(2026, 11, 1);

    [Fact]
    public void Meta_valida_e_criada()
    {
        var meta = Meta.Definir("Aprender LINQ", "Consultar dados com clareza",
                                "Resolver 10 exercícios sem consulta", new DateOnly(2026, 12, 1), Hoje);

        Assert.Equal("Aprender LINQ", meta.Objetivo);
    }

    [Fact]
    public void Prazo_no_passado_e_recusado()
    {
        Assert.Throws<ArgumentException>(() =>
            Meta.Definir("x", "y", "z", Hoje.AddDays(-1), Hoje));
    }

    [Fact]
    public void Meta_sem_medida_e_recusada()
    {
        Assert.Throws<ArgumentException>(() =>
            Meta.Definir("Aprender LINQ", "porque sim", "   ", Hoje.AddDays(30), Hoje));
    }
}
```

Perceba que o teste **é** a descrição do propósito: "uma meta sem medida não existe; um prazo no passado não vale". Quem lê o teste entende o que o código quer ser.

## Propósito também é nome: o antídoto contra `Utils`

Classes sem propósito são a versão em código da pessoa sem propósito: tudo cabe nelas, nada tem sentido. Compare:

```cs
// Sem propósito: o que "Helper" faz? De tudo um pouco.
public static class Helper
{
    public static string Formatar(decimal v) => v.ToString("C");
    public static bool EmailValido(string s)  => s.Contains('@');
    public static void SalvarLog(string m)     => File.AppendAllText("log.txt", m);
    public static int  CalcularIdade(DateTime n) => DateTime.Today.Year - n.Year;
}

// Com propósito: cada tipo diz a que veio (e é fácil de testar e de achar).
public static class FormatadorDeMoeda  { public static string Formatar(decimal v) => v.ToString("C"); }
public sealed class ValidadorDeEmail   { public bool EhValido(string s) => s.Contains('@'); }
public interface IRegistroDeEventos     { void Registrar(string mensagem); }
public static class CalculoDeIdade      { public static int Em(DateOnly nascimento, DateOnly hoje) => hoje.Year - nascimento.Year - (hoje < nascimento.AddYears(hoje.Year - nascimento.Year) ? 1 : 0); }
```

Esse é o **SRP** (cap. 20) com outro nome: *cada coisa tem um propósito definido*. E o segundo bloco tem uma vantagem extra: ao nomear bem, você descobre quando uma classe **não tem** propósito — e deve ser dividida ou apagada.

::: cuidado Cuidado com os dois extremos
**Sem propósito:** começar dez projetos e terminar nenhum. **Propósito rígido demais:** ignorar o que o cliente aprendeu na última demonstração e entregar exatamente o que se planejou meses atrás. A lei pede *clareza e persistência*, não *teimosia*. Reavalie o propósito a cada iteração, mas **troque de propósito de propósito** (de forma consciente), nunca por dispersão.
:::

::: exercicio Sua primeira meta de verdade
1. Escreva, em código, uma `Meta` para **"Aprender testes automatizados em C#"** (prazo de 60 dias, medida concreta).
2. Monte um `PlanoDeAcao` com três passos.
3. Marque o primeiro como concluído e imprima o **progresso** e o **próximo passo**.
4. Qual validação a mais você colocaria em `Meta.Definir` para evitar metas "impossíveis"?

--- resposta
```cs
var hoje  = new DateOnly(2026, 11, 1);
var meta  = Meta.Definir(
    "Aprender testes automatizados em C#",
    "Ter confiança para refatorar sem medo",
    "Cobrir um módulo real com 15 testes passando no CI",
    hoje.AddDays(60), hoje);

var plano = new PlanoDeAcao(meta)
    .Adicionar("Escrever o primeiro teste com xUnit", hoje.AddDays(7))
    .Adicionar("Praticar TDD num kata",               hoje.AddDays(30))
    .Adicionar("Cobrir o módulo escolhido",           hoje.AddDays(55));

plano.Concluir("Escrever o primeiro teste com xUnit");
Console.WriteLine($"{plano.Progresso * 100:F0}% — próximo: {plano.ProximoPasso?.Descricao}");
```
4. Uma ideia: recusar prazos **muito distantes** (por exemplo, mais de 365 dias) ou exigir que a `Medida` contenha **algum número** — algo que obrigue a meta a ser verificável. A regra exata é uma conversa com quem vai usá-la.
:::

::: resumo
- **Lei 1:** sem um propósito claro e específico, o esforço se dispersa; com ele, o esforço se organiza e vira resultado.
- Em C#, uma **meta** vira um tipo (`record` com `required`) que **não admite vagueza**: o quê, por quê, como medir, até quando.
- Um **plano de ação** tem um **único próximo passo** — a cura para a indecisão — e mede o **progresso**.
- O **teste de aceitação** (TDD) é a forma executável de "saber que chegou".
- Injete o **relógio** (`hoje`) em vez de chamar `DateTime.Now` por todo lado: o propósito fica testável.
- Classes sem propósito (`Utils`, `Helper`) são o sintoma do problema; **nomear bem** é definir propósito (SRP).
- Propósito **firme**, plano **flexível**: ajuste a cada iteração.
:::

::: quiz
? O que a Lei 1 afirma sobre a falta de propósito?
+ Dispersa a energia em várias direções e leva à indecisão
- Garante criatividade
- Aumenta a produtividade
- Não faz diferença
> Esforço sem foco não se acumula em resultado.

? Qual recurso do C# 11 impede criar uma `Meta` sem preencher seus campos essenciais?
+ O modificador required
- O modificador readonly
- O modificador sealed
- O atributo [Obsolete]
> O compilador exige a inicialização dos membros `required`.

? Por que `Meta.Definir` recebe `hoje` como parâmetro?
+ Para tornar a regra de prazo testável, sem depender de DateTime.Now
- Porque DateOnly não tem Today
- Para economizar memória
- Para permitir herança
> Dependências ocultas (como o relógio) dificultam os testes.

? Qual a função de `ProximoPasso` único no plano?
+ Combater a indecisão: há sempre uma ação clara a fazer agora
- Esconder os demais passos
- Executar todos em paralelo
- Substituir a meta
> "Quem hesita entre duas coisas pode não fazer nenhuma."

? Como o TDD se relaciona com a Lei 1?
+ O teste escrito primeiro define, de forma executável, o que significa "pronto"
- O TDD dispensa metas
- O teste só vale depois do código
- São conceitos opostos
> O propósito vira um critério verificável.

? Qual o problema de classes como `Helper` ou `Utils`?
+ Não têm propósito definido, juntam responsabilidades e dificultam achar e testar o código
- São mais lentas
- O compilador proíbe
- Só funcionam com static
> Violam o SRP.
:::

::: cartas
Propósito definido :: Objetivo claro e específico (o quê, por quê, quando, como medir) em torno do qual se organiza o esforço.
Meta SMART :: Específica, mensurável, atingível, relevante e com prazo.
required :: Modificador (C# 11) que obriga a inicialização do membro ao criar o objeto.
record :: Tipo para dados imutáveis, com igualdade por valor e expressão `with`.
Próximo passo :: Única ação a focar agora; evita a indecisão.
Teste de aceitação :: Teste que define e verifica o que significa "pronto" para uma funcionalidade.
Injeção do relógio :: Passar a data/hora por parâmetro (ou interface) para testar regras de tempo.
Classe sem propósito :: Tipo "faz-tudo" (Utils, Helper): sintoma de falta de responsabilidade única.
:::
