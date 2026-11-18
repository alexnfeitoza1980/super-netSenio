---
id: foco-cap67
order: 89
kind: capitulo
num: 67
group: C# & Mentalidade · Parte 13 · Foco em C#
title: Foco no mundo: pensamento sistêmico e laços de feedback
subtitle: O terceiro foco olha para fora e para o todo. Sistemas têm laços de feedback, atrasos e efeitos colaterais, e uma simulação simples de projeto mostra por que a pressa por atalhos costuma sair cara.
minutes: 60
hue: 320
objectives:
- Entender o **foco externo** como a capacidade de ver **sistemas**, não só peças
- Reconhecer **laços de feedback**, **atrasos** e **efeitos colaterais** ("não há efeitos colaterais, só efeitos")
- Modelar um **projeto** como um sistema simples em C# e **simular** duas práticas
- Interpretar a simulação com **humildade**: modelo é mapa, não território
- Evitar a **ilusão de profundidade explicativa** ("eu entendo como isso funciona")
origem: livros/04-foco-em-csharp/content/99-cap67.md
---

::: vila O atalho que ficou caro
O prazo apertou, e a equipe decide **pular os testes** nas próximas entregas. Nas duas primeiras semanas, entrega-se mais. Em poucas semanas, boa parte do tempo vai para consertar defeitos que o próprio atalho produziu. Lia desenha no quadro um **círculo com setas** e diz: "Não é que alguém errou. É a **estrutura** do sistema que empurra para esse resultado".
:::

::: ilustracao img/foco/cap67.svg | Os eventos são a ponta; o que os produz é a estrutura: laços, atrasos e pressa.
:::

## O que o livro diz

> O **foco externo** inclui entender os **sistemas** que moldam nossa vida e nosso trabalho. Sistemas têm **laços de feedback**, **atrasos** entre causa e efeito e **consequências não intencionais**.

O que vale guardar:

- O **pensamento sistêmico** olha o conjunto: as partes interagem, e as soluções locais podem **piorar** o todo.
- Em sistemas complexos **não há "efeitos colaterais", só efeitos**: o que chamamos de "colateral" é apenas o efeito que **não queríamos ver**.
- A **cegueira sistêmica** é a nossa tendência de focar o evento imediato, ignorando o laço maior que o produz.
- A **ilusão de profundidade explicativa**: achamos que entendemos como algo funciona **até tentarmos explicar** passo a passo.
- Os "**problemas complicados**" (*wicked problems*) não têm solução definitiva; cada intervenção muda o problema.
- Um bom começo é **mapear** o sistema: quem influencia quem, onde há **atraso** e onde o laço **se reforça** ou **se equilibra**.

::: cuidado Mapa, não território
Modelos de sistemas (como o desta página) **simplificam**. Eles ajudam a **enxergar a estrutura** e a fazer perguntas melhores, mas não **prevêem** o futuro com precisão. Um bom modelador é quem **desconfia do próprio modelo** e o testa contra a realidade.
:::

## Da vida para o código

| No livro | Em engenharia de software |
|---|---|
| Laço de reforço | **Dívida técnica** → mais bugs → menos tempo → mais atalhos |
| Laço de equilíbrio | **Testes** → menos bugs → mais tempo para melhorar |
| Atraso | O custo do atalho **só aparece semanas depois** |
| Efeito colateral | **Regressões** de uma mudança "local" |
| Cegueira sistêmica | Consertar o **sintoma** (o bug) e não a **causa** (processo) |
| Ilusão de profundidade | "Eu sei como esse módulo funciona" (até explicar) |

::: figura O laço da dívida
<svg viewBox="0 0 740 260" role="img" aria-label="Laço de reforço: pressa, atalhos, mais bugs, menos tempo livre, mais pressa; e laço de equilíbrio com testes">
  <g font-size="13" font-weight="800" text-anchor="middle">
    <rect class="f-bad" x="40" y="100" width="130" height="46" rx="10"/><text x="105" y="128">Pressa</text>
    <rect class="f-warn" x="250" y="30" width="150" height="46" rx="10"/><text x="325" y="58">Pular testes</text>
    <rect class="f-bad" x="490" y="100" width="130" height="46" rx="10"/><text x="555" y="128">Mais bugs</text>
    <rect class="f-plain" x="250" y="180" width="150" height="46" rx="10"/><text x="325" y="208">Menos tempo livre</text>
    <rect class="f-good" x="250" y="100" width="150" height="46" rx="10"/><text x="325" y="128">Testes (equilíbrio)</text>
  </g>
  <path class="arrow" d="M170 112L248 60"/><path class="arrow" d="M402 56L490 108"/><path class="arrow" d="M520 148L400 200"/><path class="arrow" d="M250 205L110 148"/>
  <path class="arrow" d="M402 124L488 124"/>
  <text x="205" y="86" font-size="11">(+)</text><text x="455" y="76" font-size="11">(+) depois de semanas</text>
  <text x="470" y="196" font-size="11">(−)</text><text x="438" y="116" font-size="11">(−)</text><text x="150" y="190" font-size="11">(+)</text>
  <text x="560" y="230" font-size="12" font-weight="700" text-anchor="middle">R: reforço · cada volta piora</text>
</svg>
:::

## 1) Um projeto como sistema

Vamos modelar **duas práticas** em um time de capacidade fixa. As regras (inventadas, mas verossímeis) são:

- A **capacidade** semanal é de 10 unidades de esforço.
- Cada **bug aberto** consome 1 unidade para ser corrigido; a correção vem **antes** de funcionalidades novas.
- **Sem testes:** cada funcionalidade gera **0,5** bug.
- **Com testes:** a funcionalidade custa **20% mais** de esforço, mas gera só **0,1** bug.

```cs Jornada/Foco/Cap67/SimuladorDeProjeto.cs
namespace Jornada.Foco.Cap67;

public enum Pratica { SemTestes, ComTestes }

public sealed record Parametros(
    double Capacidade = 10,
    double CustoPorBug = 1,
    double CustoExtraDeTestes = 0.2,
    double DefeitosPorFeatureSemTestes = 0.5,
    double DefeitosPorFeatureComTestes = 0.1);

public sealed record Semana(int Numero, double Features, double BugsAbertos, double FeaturesAcumuladas);

public static class SimuladorDeProjeto
{
    public static IReadOnlyList<Semana> Simular(Pratica pratica, int semanas, Parametros? parametros = null)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(semanas);
        var p = parametros ?? new Parametros();

        double bugs = 0, acumulado = 0;
        var historico = new List<Semana>();

        for (int n = 1; n <= semanas; n++)
        {
            // 1) o esforço vai primeiro para consertar o que já está aberto
            double esforcoParaBugs = Math.Min(bugs * p.CustoPorBug, p.Capacidade);
            double corrigidos = esforcoParaBugs / p.CustoPorBug;

            // 2) o que sobrou vira funcionalidade (testes custam um pouco mais)
            double esforcoLivre = p.Capacidade - esforcoParaBugs;
            double fator = pratica == Pratica.ComTestes ? 1 + p.CustoExtraDeTestes : 1;
            double features = esforcoLivre / fator;

            // 3) cada funcionalidade nova produz defeitos, que só serão sentidos na PRÓXIMA semana (o atraso)
            double defeitosPorFeature = pratica == Pratica.ComTestes
                ? p.DefeitosPorFeatureComTestes
                : p.DefeitosPorFeatureSemTestes;

            bugs = bugs - corrigidos + features * defeitosPorFeature;
            acumulado += features;
            historico.Add(new Semana(n, features, bugs, acumulado));
        }

        return historico;
    }

    /// <summary>Primeira semana em que 'a' já entregou ao menos tanto, acumulado, quanto 'b' (ou null).</summary>
    public static int? SemanaDeCruzamento(IReadOnlyList<Semana> a, IReadOnlyList<Semana> b)
    {
        for (int i = 0; i < Math.Min(a.Count, b.Count); i++)
            if (a[i].FeaturesAcumuladas >= b[i].FeaturesAcumuladas)
                return a[i].Numero;
        return null;
    }
}
```

```cs Uso
var sem = SimuladorDeProjeto.Simular(Pratica.SemTestes, 12);
var com = SimuladorDeProjeto.Simular(Pratica.ComTestes, 12);

Console.WriteLine("semana | sem testes (acum. · bugs) | com testes (acum. · bugs)");
foreach (int n in new[] { 1, 2, 4, 6, 8, 12 })
{
    var s = sem[n - 1];
    var c = com[n - 1];
    Console.WriteLine($"{n,6} | {s.FeaturesAcumuladas,6:F1} · {s.BugsAbertos,4:F1}           | {c.FeaturesAcumuladas,6:F1} · {c.BugsAbertos,4:F1}");
}

Console.WriteLine($"Com testes passa na frente na semana {SimuladorDeProjeto.SemanaDeCruzamento(com, sem)}.");
```

```saida
semana | sem testes (acum. · bugs) | com testes (acum. · bugs)
     1 |   10,0 ·  5,0           |    8,3 ·  0,8
     2 |   15,0 ·  2,5           |   16,0 ·  0,8
     4 |   28,8 ·  3,1           |   31,4 ·  0,8
     6 |   42,2 ·  3,3           |   46,7 ·  0,8
     8 |   55,5 ·  3,3           |   62,1 ·  0,8
    12 |   82,2 ·  3,3           |   92,9 ·  0,8
Com testes passa na frente na semana 2.
```

A leitura **estrutural**: no começo, **pular testes** parece ótimo (10 contra 8,3 funcionalidades na semana 1). O custo está **atrasado**: os bugs só pesam nas semanas seguintes, e ficam **realimentando** o laço. Quem olha só o **evento** ("terminamos mais esta semana") não vê a **estrutura** que produz o resultado.

Neste modelo o cruzamento acontece já na **semana 2**, porque os defeitos aparecem na semana seguinte. Num projeto real a descoberta é bem mais lenta (bugs que dormem por meses, clientes que reclamam depois), e por isso o atalho **parece bom por muito mais tempo**; quanto maior o atraso, mais difícil é perceber o laço.

::: dica Pergunte "o que muda se..."
Experimente mudar os parâmetros: e se os testes custassem 60% a mais? E se um bug custasse só 0,3 para corrigir? Na tabela do item seguinte, nos dois cenários alternativos a conclusão **inverte**, e isso é a lição: a decisão correta **depende de números do seu contexto**, que você precisa **medir** (quanto custa corrigir um bug de verdade? quantos bugs nascem por funcionalidade?).
:::

## 2) Quando o atalho vale a pena

Pular testes **pode** ser racional, quando o horizonte é muito curto (um protótipo descartável) ou o custo do bug é baixo. O código ajuda a **explorar** essa fronteira:

```cs Jornada/Foco/Cap67/Fronteira.cs
namespace Jornada.Foco.Cap67;

public static class Fronteira
{
    /// <summary>Menor horizonte (em semanas) em que "com testes" entrega mais, acumulado; null se nunca em 'maximo'.</summary>
    public static int? HorizonteMinimo(Parametros p, int maximo = 52)
    {
        var sem = SimuladorDeProjeto.Simular(Pratica.SemTestes, maximo, p);
        var com = SimuladorDeProjeto.Simular(Pratica.ComTestes, maximo, p);

        for (int i = 0; i < maximo; i++)
            if (com[i].FeaturesAcumuladas > sem[i].FeaturesAcumuladas)
                return i + 1;
        return null;
    }
}
```

```cs Uso
var cenarios = new (string Nome, Parametros P)[]
{
    ("Base",                         new Parametros()),
    ("Bug barato (custa 0,3)",       new Parametros(CustoPorBug: 0.3)),
    ("Testes caros (+60%)",          new Parametros(CustoExtraDeTestes: 0.6)),
};

foreach (var (nome, p) in cenarios)
{
    int? h = Fronteira.HorizonteMinimo(p);
    Console.WriteLine($"{nome,-26} → {(h is null ? "nunca compensa em 52 semanas" : $"compensa a partir da semana {h}")}");
}
```

```saida
Base                       → compensa a partir da semana 2
Bug barato (custa 0,3)     → nunca compensa em 52 semanas
Testes caros (+60%)        → nunca compensa em 52 semanas
```

```cs JornadaTests/Foco/Cap67/Cap67Tests.cs
using Jornada.Foco.Cap67;
using Xunit;

public class Cap67Tests
{
    [Fact]
    public void Na_primeira_semana_sem_testes_entrega_mais()
    {
        var sem = SimuladorDeProjeto.Simular(Pratica.SemTestes, 1);
        var com = SimuladorDeProjeto.Simular(Pratica.ComTestes, 1);
        Assert.True(sem[0].Features > com[0].Features);
    }

    [Fact]
    public void Com_o_tempo_com_testes_entrega_mais_acumulado()
    {
        var sem = SimuladorDeProjeto.Simular(Pratica.SemTestes, 12);
        var com = SimuladorDeProjeto.Simular(Pratica.ComTestes, 12);
        Assert.True(com[^1].FeaturesAcumuladas > sem[^1].FeaturesAcumuladas);
    }

    [Fact]
    public void Sem_testes_acumula_mais_bugs_abertos()
    {
        var sem = SimuladorDeProjeto.Simular(Pratica.SemTestes, 12);
        var com = SimuladorDeProjeto.Simular(Pratica.ComTestes, 12);
        Assert.True(sem[^1].BugsAbertos > com[^1].BugsAbertos);
    }

    [Fact]
    public void Semanas_nao_positivas_sao_rejeitadas() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => SimuladorDeProjeto.Simular(Pratica.SemTestes, 0));

    [Fact]
    public void Bug_barato_adia_a_vantagem_dos_testes()
    {
        int? baseH = Fronteira.HorizonteMinimo(new Parametros());
        int? baratoH = Fronteira.HorizonteMinimo(new Parametros(CustoPorBug: 0.3));
        Assert.NotNull(baseH);
        Assert.True(baratoH is null || baratoH > baseH);
    }
}
```

## 3) A ilusão de profundidade explicativa

Pergunte a si mesmo: "eu entendo como o módulo de pagamentos funciona?". Provavelmente a resposta é **sim**. Agora tente **explicar passo a passo**, de cabeça, o que acontece entre o clique e o recibo, incluindo os **atrasos**, as **filas** e o que ocorre se uma etapa **falha**. Costuma ser um choque: a sensação de entender era **maior** que o entendimento.

Um exercício prático: **desenhar o sistema em um papel**, com setas e marcando onde há atraso, **antes** de mudá-lo. Quase sempre aparece uma seta que ninguém sabia que existia.

::: exercicio Mais uma alavanca
Acrescente ao modelo um **parâmetro de "folga"**: uma fração da capacidade (por exemplo, 20%) reservada **sempre** para melhorar o código (reduzindo os bugs abertos em `folga × capacidade / custoPorBug` por semana, sem criar funcionalidade). Escreva uma simulação que mostre se, **sem testes**, a folga ajuda a longo prazo (dica: compare 12 semanas com e sem folga).

--- resposta
```cs Jornada/Foco/Cap67/ComFolga.cs
namespace Jornada.Foco.Cap67;

public static class ComFolga
{
    public static double FeaturesAcumuladasSemTestes(int semanas, double folga, Parametros? parametros = null)
    {
        if (folga is < 0 or >= 1) throw new ArgumentOutOfRangeException(nameof(folga));
        var p = parametros ?? new Parametros();

        double bugs = 0, acumulado = 0;
        for (int n = 1; n <= semanas; n++)
        {
            double capacidade = p.Capacidade * (1 - folga);
            double saneamento = Math.Min(bugs, p.Capacidade * folga / p.CustoPorBug);
            bugs -= saneamento;

            double esforcoParaBugs = Math.Min(bugs * p.CustoPorBug, capacidade);
            double corrigidos = esforcoParaBugs / p.CustoPorBug;
            double features = capacidade - esforcoParaBugs;

            bugs = bugs - corrigidos + features * p.DefeitosPorFeatureSemTestes;
            acumulado += features;
        }
        return acumulado;
    }
}
```
```cs Jornada/Foco/Cap67/ComFolgaTests.cs
using Jornada.Foco.Cap67;
using Xunit;

public class ComFolgaTests
{
    [Fact]
    public void Folga_zero_equivale_ao_modelo_sem_testes()
    {
        double esperado = SimuladorDeProjeto.Simular(Pratica.SemTestes, 12)[^1].FeaturesAcumuladas;
        Assert.Equal(esperado, ComFolga.FeaturesAcumuladasSemTestes(12, 0), 6);
    }

    [Fact]
    public void Folga_fora_do_intervalo_e_rejeitada() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => ComFolga.FeaturesAcumuladasSemTestes(12, 1));
}
```
Execute para comparar (por exemplo, 12 semanas com `folga: 0` e `folga: 0.2`) e **leia o resultado** com a pergunta: a folga é um investimento que se paga, ou só um custo, neste modelo? Depois, **mude os parâmetros** e veja se a resposta muda.
:::

::: resumo
- O **foco externo** inclui enxergar **sistemas**: laços de **reforço** e de **equilíbrio**, **atrasos** e **efeitos** (não "colaterais", só efeitos).
- Na simulação, **pular testes** ganha na semana 1 e **perde** depois, porque o custo é **atrasado** e **realimenta** o laço.
- A decisão certa **depende dos números do contexto**: meça (custo de um bug, defeitos por funcionalidade).
- Modelo é **mapa**: serve para **fazer perguntas**, não para prever com precisão.
- Cuidado com a **ilusão de profundidade explicativa**: desenhe o sistema antes de mudá-lo.
:::

::: quiz
? O que significa "não há efeitos colaterais, só efeitos"?
+ O que chamamos de colateral é só o efeito que não queríamos enxergar
- Que todo efeito é bom
- Que efeitos não podem ser medidos
- Que sistemas são simples
> Em sistemas, tudo o que a intervenção causa é efeito dela.

? Por que o atraso é importante em sistemas?
+ Porque o custo de uma decisão só aparece depois, escondendo a causa
- Porque o código fica lento
- Porque aumenta a capacidade
- Porque elimina bugs
> O atalho parece ótimo na primeira semana.

? O que a simulação do capítulo mostra?
+ Que pular testes parece vantajoso no início, mas pode perder no acumulado
- Que testes são sempre mais rápidos
- Que bugs não custam nada
- Que a capacidade é infinita
> Depende dos parâmetros.

? Qual a postura correta diante de um modelo?
+ Usá-lo para fazer perguntas, desconfiando dele e testando contra a realidade
- Tratá-lo como previsão exata
- Ignorá-lo
- Escondê-lo da equipe
> Mapa, não território.

? O que é a ilusão de profundidade explicativa?
+ Achar que entende algo até tentar explicá-lo passo a passo
- Explicar demais
- Entender tudo de um sistema
- Um padrão de projeto
> Desenhar o sistema revela o que faltava.

? Em que caso pular testes pode ser racional?
+ Horizonte muito curto ou custo de bug baixo (ex.: protótipo descartável)
- Sempre
- Nunca
- Apenas em produção
> A fronteira depende do contexto.
:::

::: cartas
Pensamento sistêmico :: Olhar o conjunto e suas interações, não só as peças.
Laço de reforço :: Ciclo que amplifica o que já acontece (dívida gera mais dívida).
Laço de equilíbrio :: Ciclo que tende a estabilizar (testes reduzem bugs).
Atraso :: Intervalo entre causa e efeito que esconde a relação.
Efeito colateral :: Efeito não pretendido; "só efeitos" em sistemas complexos.
Cegueira sistêmica :: Focar no evento e ignorar a estrutura que o produz.
Problema complicado (wicked) :: Sem solução definitiva; cada intervenção o muda.
Ilusão de profundidade explicativa :: Achar que entende algo até ter de explicar.
:::
