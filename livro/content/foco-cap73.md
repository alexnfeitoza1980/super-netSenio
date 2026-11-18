---
id: foco-cap73
order: 95
kind: capitulo
num: 73
group: C# & Mentalidade · Parte 13 · Foco em C#
title: Decidir com horizonte: quem paga a conta?
subtitle: O líder de foco amplo olha para o longo prazo e para quem não está na sala. Uma análise de decisão em C# que separa horizontes, expõe custos externos e avisa quando o atalho de hoje cobra caro amanhã.
minutes: 50
hue: 95
objectives:
- Ver o **foco de longo prazo** como parte da liderança
- Separar os efeitos de uma decisão em **horizontes** (semanas, meses, anos)
- Identificar **custos externos**: quem paga e **não** decidiu
- Perguntar **"quem não está na sala?"** antes de decidir
- Usar a análise como **conversa estruturada**, não como cálculo definitivo
origem: livros/04-foco-em-csharp/content/105-cap73.md
---

::: vila A gambiarra do relatório
Para entregar no prazo, a equipe propõe uma gambiarra no relatório financeiro: "é rápido e só a gente vai ver". Lia desenha três colunas no quadro: **semanas**, **meses**, **anos**. Na primeira, só ganhos. Nas outras duas, o **suporte** recebe mais chamados, os **clientes** veem números inconsistentes e **quem entrar na equipe daqui a dois anos** vai herdar o problema. Ninguém dessas três categorias está na reunião.
:::

::: ilustracao img/foco/cap73.svg | Decidir com horizonte é lembrar de quem sente o efeito depois: suporte, clientes, a futura equipe.
:::

## O que o livro diz

> Os líderes mais eficazes ampliam o **horizonte**: pensam em **prazos longos**, em **consequências em cadeia** e em **quem será afetado** — inclusive quem ainda não está presente.

Pontos do capítulo original:

- A **liderança de longo prazo** combina os três focos: **autoconsciência** (reconhecer o próprio viés para o curto prazo), **empatia** (considerar os outros) e **visão de sistemas** (ver os laços e atrasos).
- O ser humano tende a **desvalorizar o futuro** (como vimos no desconto hiperbólico, cap. 64); as organizações, ainda mais, quando o **bônus** é trimestral.
- Os **custos externos** (o que o livro trata como as consequências que recaem sobre outros) ficam **fora da conta** de quem decide, a menos que alguém os **traga à mesa**.
- Decidir "pelas gerações seguintes" exige **imaginar** pessoas que não conhecemos; ferramentas concretas (listas de afetados, cenários) ajudam.
- Transparência sobre **impactos reais** muda o comportamento de quem decide.

::: cuidado Estimativas são palpites
Os pontos que vamos atribuir aos efeitos são **julgamentos subjetivos**, não medições. O valor da análise **não** está no número final, mas nas **perguntas** que ela força ("quem mais é afetado? em quanto tempo?"). Duas pessoas podem pontuar diferente e **tudo bem**: a **diferença** é uma conversa que vale ter.
:::

## Da vida para o código

| No livro | Em engenharia de software |
|---|---|
| Horizonte longo | **Manutenção** e **evolução**, não só a entrega |
| Custos externos | **Suporte, usuários, operação** pagam pela nossa pressa |
| "Quem não está na sala" | **Futuras pessoas da equipe** e **clientes** |
| Viés pelo curto prazo | **Bônus por entrega**, nunca por manutenção |
| Atraso causa-efeito | **Dívida técnica** que só aparece depois |
| Imaginar os afetados | **Personas** e **cenários de falha** |

::: figura Efeitos por horizonte e por quem paga
<svg viewBox="0 0 740 230" role="img" aria-label="Matriz de efeitos: linhas são quem sente, colunas são semanas, meses e anos; só a nossa equipe ganha em semanas">
  <g font-size="12" font-weight="800" text-anchor="middle">
    <text x="330" y="28">Semanas</text><text x="470" y="28">Meses</text><text x="610" y="28">Anos</text>
  </g>
  <g font-size="12" text-anchor="end">
    <text x="250" y="68">Nossa equipe</text><text x="250" y="108">Suporte</text><text x="250" y="148">Clientes</text><text x="250" y="188">Futura equipe</text>
  </g>
  <g>
    <rect class="f-good" x="270" y="46" width="120" height="30" rx="6"/><rect class="f-bad" x="410" y="46" width="120" height="30" rx="6"/><rect class="f-bad" x="550" y="46" width="120" height="30" rx="6"/>
    <rect class="f-plain" x="270" y="86" width="120" height="30" rx="6"/><rect class="f-bad" x="410" y="86" width="120" height="30" rx="6"/><rect class="f-plain" x="550" y="86" width="120" height="30" rx="6"/>
    <rect class="f-plain" x="270" y="126" width="120" height="30" rx="6"/><rect class="f-bad" x="410" y="126" width="120" height="30" rx="6"/><rect class="f-plain" x="550" y="126" width="120" height="30" rx="6"/>
    <rect class="f-plain" x="270" y="166" width="120" height="30" rx="6"/><rect class="f-plain" x="410" y="166" width="120" height="30" rx="6"/><rect class="f-bad" x="550" y="166" width="120" height="30" rx="6"/>
  </g>
  <g font-size="12" text-anchor="middle" font-weight="800">
    <text x="330" y="66">+5</text><text x="470" y="66">−3</text><text x="610" y="66">−2</text>
    <text x="470" y="106">−4</text><text x="470" y="146">−2</text><text x="610" y="186">−6</text>
  </g>
</svg>
:::

## 1) A análise de decisão

Cada **efeito** diz **quem** sente, **quando** e **quanto** (de −10 a +10), com uma frase de justificativa. O código agrega por **horizonte** e por **quem**:

```cs Jornada/Foco/Cap73/AnaliseDeDecisao.cs
namespace Jornada.Foco.Cap73;

public enum Horizonte { Semanas, Meses, Anos }

public sealed record Efeito(string Quem, Horizonte Quando, int Pontos, string Descricao);

public sealed class AnaliseDeDecisao
{
    public string Titulo { get; }
    private readonly List<Efeito> efeitos = new();

    public AnaliseDeDecisao(string titulo) => Titulo = titulo;

    public AnaliseDeDecisao Adicionar(string quem, Horizonte quando, int pontos, string descricao)
    {
        if (pontos is < -10 or > 10)
            throw new ArgumentOutOfRangeException(nameof(pontos), "Use de -10 a +10.");
        efeitos.Add(new Efeito(quem, quando, pontos, descricao));
        return this;
    }

    public int Saldo(Horizonte horizonte) =>
        efeitos.Where(e => e.Quando == horizonte).Sum(e => e.Pontos);

    /// <summary>Soma dos efeitos NEGATIVOS que recaem sobre quem não é "nós" (sempre ≤ 0).</summary>
    public int CustosExternos(string nos) =>
        efeitos.Where(e => e.Quem != nos && e.Pontos < 0).Sum(e => e.Pontos);

    /// <summary>Quem é afetado pela decisão mas não está na conversa.</summary>
    public IReadOnlyList<string> AusentesDaSala(IEnumerable<string> presentes)
    {
        var naSala = new HashSet<string>(presentes, StringComparer.Ordinal);
        return efeitos.Select(e => e.Quem)
                      .Where(q => !naSala.Contains(q))
                      .Distinct()
                      .OrderBy(q => q, StringComparer.Ordinal)
                      .ToList();
    }

    public string? ArmadilhaDoCurtoPrazo()
    {
        int curto = Saldo(Horizonte.Semanas);
        int longo = Saldo(Horizonte.Anos);
        return curto > 0 && longo < 0
            ? $"Armadilha do curto prazo: {curto:+0;-0} em semanas, mas {longo:+0;-0} em anos."
            : null;
    }
}
```

```cs Uso
var analise = new AnaliseDeDecisao("Gambiarra no relatório financeiro")
    .Adicionar("Nossa equipe",  Horizonte.Semanas,  5, "Entrega dentro do prazo")
    .Adicionar("Nossa equipe",  Horizonte.Meses,   -3, "Retrabalho quando alguém reclamar")
    .Adicionar("Suporte",       Horizonte.Meses,   -4, "Mais chamados sobre números diferentes")
    .Adicionar("Clientes",      Horizonte.Meses,   -2, "Relatórios inconsistentes")
    .Adicionar("Futura equipe", Horizonte.Anos,    -6, "Código difícil de manter")
    .Adicionar("Nossa equipe",  Horizonte.Anos,    -2, "Dívida acumulada");

Console.WriteLine(analise.Titulo);
foreach (var h in Enum.GetValues<Horizonte>())
    Console.WriteLine($"  {h,-8} {analise.Saldo(h):+0;-0;0}");

Console.WriteLine($"Custos externos: {analise.CustosExternos("Nossa equipe")}");
Console.WriteLine("Ausentes da sala: " + string.Join(", ", analise.AusentesDaSala(new[] { "Nossa equipe", "Gerência" })));
Console.WriteLine(analise.ArmadilhaDoCurtoPrazo());
```

```saida
Gambiarra no relatório financeiro
  Semanas  +5
  Meses    -9
  Anos     -8
Custos externos: -12
Ausentes da sala: Clientes, Futura equipe, Suporte
Armadilha do curto prazo: +5 em semanas, mas -8 em anos.
```

A análise não decide nada, mas **muda a pergunta**: em vez de "isso entrega no prazo?", passa a ser "**quem vai pagar**, e **eles sabem disso**?". Os **custos externos** (−12) são **o dobro** do ganho de curto prazo (+5), e quem os paga **nem estava na reunião**.

::: dica Traga os ausentes (de verdade)
O melhor uso da lista "ausentes da sala" é **convidá-los**: perguntar ao suporte quanto custa um chamado, ao cliente o que muda se o relatório divergir. Se não for possível, **escolha alguém para falar por eles** na reunião (o "advogado do suporte"). Isso é a empatia cognitiva do capítulo 65, aplicada a uma **decisão**.
:::

```cs JornadaTests/Foco/Cap73/Cap73Tests.cs
using Jornada.Foco.Cap73;
using Xunit;

public class Cap73Tests
{
    private static AnaliseDeDecisao Exemplo() =>
        new AnaliseDeDecisao("x")
            .Adicionar("Nós",  Horizonte.Semanas,  5, "a")
            .Adicionar("Eles", Horizonte.Anos,    -6, "b")
            .Adicionar("Nós",  Horizonte.Anos,    -1, "c");

    [Fact]
    public void Saldo_soma_por_horizonte()
    {
        Assert.Equal(5, Exemplo().Saldo(Horizonte.Semanas));
        Assert.Equal(-7, Exemplo().Saldo(Horizonte.Anos));
        Assert.Equal(0, Exemplo().Saldo(Horizonte.Meses));
    }

    [Fact]
    public void Custos_externos_ignoram_o_que_recai_sobre_nos() =>
        Assert.Equal(-6, Exemplo().CustosExternos("Nós"));

    [Fact]
    public void Ausentes_sao_os_afetados_fora_da_sala() =>
        Assert.True(Exemplo().AusentesDaSala(new[] { "Nós" }).SequenceEqual(new[] { "Eles" }));

    [Fact]
    public void Armadilha_aparece_quando_curto_positivo_e_longo_negativo() =>
        Assert.NotNull(Exemplo().ArmadilhaDoCurtoPrazo());

    [Fact]
    public void Sem_armadilha_quando_o_longo_prazo_tambem_ganha()
    {
        var a = new AnaliseDeDecisao("y")
            .Adicionar("Nós", Horizonte.Semanas, 2, "a")
            .Adicionar("Nós", Horizonte.Anos, 3, "b");
        Assert.Null(a.ArmadilhaDoCurtoPrazo());
    }

    [Fact]
    public void Pontos_fora_da_escala_sao_rejeitados() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new AnaliseDeDecisao("z").Adicionar("Nós", Horizonte.Anos, 11, "a"));
}
```

## 2) O pré-mortem

Uma técnica famosa de **pensar no futuro** é o **pré-mortem**: antes de decidir, imagine que a decisão **deu errado** daqui a um ano e escreva **por quê**. Ela contorna o **otimismo** do grupo (ninguém quer ser o chato) e traz à tona **riscos** que a equipe já "sentia" e não dizia.

```cs Jornada/Foco/Cap73/PreMortem.cs
namespace Jornada.Foco.Cap73;

public sealed record Risco(string Autor, string Motivo, int Probabilidade /*1-5*/, int Impacto /*1-5*/)
{
    public int Pontuacao => Probabilidade * Impacto;
}

public static class PreMortem
{
    /// <summary>Riscos ordenados do mais grave ao menos grave; os 'n' primeiros são os que a equipe deve discutir.</summary>
    public static IReadOnlyList<Risco> Prioritarios(IEnumerable<Risco> riscos, int n = 3) =>
        riscos.OrderByDescending(r => r.Pontuacao)
              .ThenBy(r => r.Motivo, StringComparer.Ordinal)
              .Take(n)
              .ToList();
}
```

```cs Uso
var riscos = new[]
{
    new Risco("Ana",   "Ninguém sabe mais como o relatório é gerado",  4, 4),
    new Risco("Bruno", "O cliente grande descobre a divergência",      3, 5),
    new Risco("Carla", "O suporte fica sobrecarregado",                 4, 3),
    new Risco("Davi",  "A gambiarra quebra na virada do ano",           2, 5),
    new Risco("Eva",   "Falta documentação",                            5, 2),
};

foreach (var r in PreMortem.Prioritarios(riscos))
    Console.WriteLine($"{r.Pontuacao,2}  {r.Motivo} ({r.Autor})");
```

```saida
16  Ninguém sabe mais como o relatório é gerado (Ana)
15  O cliente grande descobre a divergência (Bruno)
12  O suporte fica sobrecarregado (Carla)
```

::: exercicio Descontar o futuro de propósito
Hoje, `Saldo` soma os horizontes **sem peso**. Crie `SaldoPonderado(AnaliseDeDecisao, double pesoSemanas, double pesoMeses, double pesoAnos)` (em uma classe estática, usando só `Saldo` público) e mostre, com um teste, que com pesos **1, 1, 1** o saldo da gambiarra é **−12** (5 − 9 − 8), mas, se você **pesar o curto prazo 5 vezes mais** (5, 1, 1), ela parece **boa** (25 − 9 − 8 = **+8**). A lição: **a decisão está escondida nos pesos**.

--- resposta
```cs Jornada/Foco/Cap73/SaldoPonderado.cs
namespace Jornada.Foco.Cap73;

public static class Ponderacao
{
    public static double SaldoPonderado(AnaliseDeDecisao a, double pesoSemanas, double pesoMeses, double pesoAnos) =>
        a.Saldo(Horizonte.Semanas) * pesoSemanas +
        a.Saldo(Horizonte.Meses)   * pesoMeses +
        a.Saldo(Horizonte.Anos)    * pesoAnos;
}
```
```cs Jornada/Foco/Cap73/SaldoPonderadoTests.cs
using Jornada.Foco.Cap73;
using Xunit;

public class SaldoPonderadoTests
{
    private static AnaliseDeDecisao Gambiarra() =>
        new AnaliseDeDecisao("Gambiarra")
            .Adicionar("Nossa equipe",  Horizonte.Semanas,  5, "prazo")
            .Adicionar("Nossa equipe",  Horizonte.Meses,   -3, "retrabalho")
            .Adicionar("Suporte",       Horizonte.Meses,   -4, "chamados")
            .Adicionar("Clientes",      Horizonte.Meses,   -2, "inconsistência")
            .Adicionar("Futura equipe", Horizonte.Anos,    -6, "manutenção")
            .Adicionar("Nossa equipe",  Horizonte.Anos,    -2, "dívida");

    [Fact]
    public void Pesos_iguais_mostram_saldo_negativo() =>
        Assert.Equal(-12, Ponderacao.SaldoPonderado(Gambiarra(), 1, 1, 1));

    [Fact]
    public void Pesar_muito_o_curto_prazo_vira_a_decisao() =>
        Assert.Equal(8, Ponderacao.SaldoPonderado(Gambiarra(), 5, 1, 1));
}
```
Repare: nada nos **fatos** mudou, só o **peso** dado ao curto prazo. Quando alguém defende a decisão, pergunte: **que pesos** estão por trás da conclusão? Torná-los **explícitos** é metade do trabalho de liderar com horizonte.
:::

::: resumo
- Liderar com **horizonte longo** combina os três focos: reconhecer o **viés de curto prazo**, considerar os **outros** e enxergar os **sistemas**.
- A análise de decisão separa efeitos por **horizonte** e por **quem sente**, expõe **custos externos** e lista **quem não está na sala**.
- Os pontos são **julgamentos subjetivos**: o valor está nas **perguntas** e na **conversa**.
- O **pré-mortem** (imaginar que deu errado e escrever por quê) traz à tona riscos não ditos.
- Os **pesos** que damos aos horizontes **escondem** a decisão: torne-os explícitos.
:::

::: quiz
? O que são custos externos em uma decisão?
+ Consequências negativas que recaem sobre quem não decidiu
- Custos de servidores externos
- Impostos
- Custos do cliente pagante
> Ficam fora da conta de quem decide, a menos que sejam trazidos à mesa.

? Qual o objetivo da lista "ausentes da sala"?
+ Lembrar de quem é afetado mas não participa, para convidá-los ou representá-los
- Excluir pessoas da reunião
- Cobrar quem faltou
- Registrar presença
> Empatia cognitiva aplicada à decisão.

? O que é uma "armadilha do curto prazo"?
+ Decisão boa em semanas e ruim em anos
- Um erro de compilação
- Uma reunião curta
- Um tipo de teste
> O atalho de hoje cobra amanhã.

? Por que os pontos da análise devem ser lidos com cautela?
+ São julgamentos subjetivos; o valor está nas perguntas e na conversa
- Porque o C# arredonda
- Porque são medições exatas
- Porque não podem ser negativos
> Duas pessoas podem pontuar diferente.

? O que é um pré-mortem?
+ Imaginar que a decisão falhou daqui a um tempo e escrever por quê, antes de decidir
- Análise após a falha
- Reunião com a diretoria
- Um tipo de backup
> Contorna o otimismo do grupo.

? O que o exercício dos pesos ensina?
+ A decisão pode estar escondida nos pesos dados a cada horizonte
- Que pesos não importam
- Que o curto prazo não conta
- Que a soma nunca muda
> Torne os pesos explícitos.
:::

::: cartas
Horizonte :: Intervalo de tempo sobre o qual os efeitos são avaliados.
Custo externo :: Consequência que recai sobre quem não participou da decisão.
Quem não está na sala :: Afetados ausentes (clientes, suporte, futura equipe).
Armadilha do curto prazo :: Ganho imediato que gera perda maior no futuro.
Viés do presente :: Tendência a desvalorizar o futuro.
Pré-mortem :: Imaginar a falha antes para listar riscos.
Peso de horizonte :: Importância dada a cada prazo na decisão.
Líder de longo prazo :: Quem combina autoconsciência, empatia e visão de sistemas.
:::
