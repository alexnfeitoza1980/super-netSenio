---
id: foco-cap71
order: 93
kind: capitulo
num: 71
group: C# & Mentalidade · Parte 13 · Foco em C#
title: Divaguei, percebi, voltei: a repetição da atenção
subtitle: Na meditação, o objetivo não é não divagar; é perceber que divagou e voltar. Cada retorno é uma repetição do músculo da atenção. Duas ferramentas em C# para contar repetições e acompanhar o progresso.
minutes: 50
hue: 40
objectives:
- Entender a **atenção plena** como treino: **divaguei → percebi → voltei → sustentei**
- Ver cada **retorno** como uma **repetição** do "músculo" da atenção
- Medir uma sessão: **repetições**, tempo divagando, **tempo para perceber**
- Acompanhar o **progresso** ao longo das sessões sem se cobrar perfeição
- Tratar as **evidências** da meditação com equilíbrio
origem: livros/04-foco-em-csharp/content/103-cap71.md
---

::: vila Cinco minutos de silêncio
Lia propõe um experimento à equipe: cinco minutos prestando atenção só na respiração. Caio tenta e se irrita: "Divaguei vinte vezes, fui péssimo!". Lia responde: "Vinte vezes que **você percebeu** e **voltou**. Vinte repetições. Foi o melhor treino que você teve hoje".
:::

::: ilustracao img/foco/cap71.svg | Divaguei, percebi, voltei: cada retorno é uma repetição do músculo da atenção.
:::

## O que o livro diz

> **Meditação** de atenção plena é, no fundo, um **treino de atenção**: você escolhe um **âncora** (a respiração, por exemplo), **nota** quando a mente se desvia e **volta** para ela, sem se julgar.

Pontos do capítulo original:

- O ciclo básico: **a mente divaga → você percebe → você a traz de volta → você sustenta o foco**. Cada vez que você **percebe e volta** é uma "**repetição**" (como no levantamento de peso).
- **Perceber que divagou** é o momento-chave, e não o fracasso; o objetivo **não** é uma mente sem pensamentos.
- Praticantes experientes têm **maior capacidade** de sustentar a atenção e de **notar mais rápido** que divagaram, e alguns estudos mostram mudanças na atividade e na estrutura de áreas ligadas à atenção.
- Há também a **atenção aberta** (observar tudo que surge sem se prender): a outra "marcha" da atenção plena.
- Em **escolas**, programas de atenção plena e de aprendizado socioemocional associam-se a melhor atenção e menos conflitos, embora os estudos variem em qualidade.

::: cuidado Evidência real, mas moderada
A pesquisa sobre meditação cresceu muito, mas ainda tem **limitações** (grupos pequenos, controle difícil, resultados mistos conforme o tipo de prática e o que se mede). A leitura equilibrada é: há **benefícios plausíveis** para atenção e regulação emocional, **não** uma cura universal. Para algumas pessoas (especialmente com histórico de trauma), a meditação pode ser **desconfortável**; nesses casos, procure orientação qualificada.
:::

## Da vida para o código

| No livro | Em engenharia de software |
|---|---|
| Âncora (respiração) | **Tarefa atual**: o alvo do foco |
| Divagar | Abrir outra aba, pensar em outra coisa |
| Perceber | **Notar** o desvio (o passo decisivo) |
| Voltar | **Retomar** a tarefa, com a próxima ação clara |
| Repetição | Cada **retorno** conta |
| Sem se julgar | **Cultura de equipe** que não pune o desvio |

::: figura O ciclo da repetição
<svg viewBox="0 0 740 246" role="img" aria-label="Ciclo: foco na âncora, divagar, perceber, voltar, e de novo foco, formando uma repetição">
  <g font-size="13" font-weight="800" text-anchor="middle">
    <rect class="f-info" x="40" y="90" width="130" height="50" rx="12"/><text x="105" y="120">Foco na âncora</text>
    <rect class="f-warn" x="230" y="30" width="130" height="50" rx="12"/><text x="295" y="60">Divaguei</text>
    <rect class="f-good" x="420" y="30" width="130" height="50" rx="12"/><text x="485" y="60">Percebi!</text>
    <rect class="f-info" x="570" y="90" width="130" height="50" rx="12"/><text x="635" y="120">Voltei</text>
  </g>
  <path class="arrow" d="M170 100L230 70"/><path class="arrow" d="M362 55L418 55"/><path class="arrow" d="M552 70L610 90"/>
  <path class="arrow" d="M570 130C400 210 300 210 172 135"/>
  <text x="370" y="232" text-anchor="middle" font-size="12" font-weight="700">cada volta completa = 1 repetição do "músculo"</text>
</svg>
:::

## 1) Medindo uma sessão de atenção

Uma sessão é uma **duração** com uma lista de **divagações** (início e momento em que a pessoa percebeu). O que interessa é **quantas repetições** e **quão rápido** se percebe:

```cs Jornada/Foco/Cap71/SessaoDeAtencao.cs
namespace Jornada.Foco.Cap71;

/// <param name="InicioEmSegundos">Quando a mente se desviou.</param>
/// <param name="PercebeuEmSegundos">Quando a pessoa percebeu e voltou.</param>
public sealed record Divagacao(int InicioEmSegundos, int PercebeuEmSegundos)
{
    public int Duracao => PercebeuEmSegundos - InicioEmSegundos;
}

public sealed class SessaoDeAtencao
{
    public int DuracaoEmSegundos { get; }
    public IReadOnlyList<Divagacao> Divagacoes { get; }

    public SessaoDeAtencao(int duracaoEmSegundos, IEnumerable<Divagacao> divagacoes)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(duracaoEmSegundos);

        var lista = divagacoes.OrderBy(d => d.InicioEmSegundos).ToList();
        int fimAnterior = 0;
        foreach (var d in lista)
        {
            if (d.InicioEmSegundos < fimAnterior || d.Duracao <= 0 || d.PercebeuEmSegundos > duracaoEmSegundos)
                throw new ArgumentException($"Divagação inválida ou sobreposta: {d}.", nameof(divagacoes));
            fimAnterior = d.PercebeuEmSegundos;
        }

        DuracaoEmSegundos = duracaoEmSegundos;
        Divagacoes = lista;
    }

    /// <summary>Cada vez que percebeu e voltou conta como uma repetição.</summary>
    public int Repeticoes => Divagacoes.Count;

    public int TempoDivagandoEmSegundos => Divagacoes.Sum(d => d.Duracao);

    public double PercentualDivagando => 100.0 * TempoDivagandoEmSegundos / DuracaoEmSegundos;

    /// <summary>Quanto tempo, em média, levou para perceber (0 se não houve divagação).</summary>
    public double TempoMedioParaPerceber => Repeticoes == 0 ? 0 : Divagacoes.Average(d => d.Duracao);
}
```

```cs Uso
var primeira = new SessaoDeAtencao(600, new[]
{
    new Divagacao(60, 95), new Divagacao(180, 200), new Divagacao(330, 420), new Divagacao(500, 515),
});

Console.WriteLine($"Repetições: {primeira.Repeticoes}");
Console.WriteLine($"Divagando: {primeira.TempoDivagandoEmSegundos} s de {primeira.DuracaoEmSegundos} ({primeira.PercentualDivagando:F0}%)");
Console.WriteLine($"Tempo médio para perceber: {primeira.TempoMedioParaPerceber:F0} s");
```

```saida
Repetições: 4
Divagando: 160 s de 600 (27%)
Tempo médio para perceber: 40 s
```

A **repetição** é o que se conta; a **qualidade** do treino aparece em outro indicador: o **tempo para perceber**. Com a prática, a mente continua divagando, mas você **percebe mais cedo**. Esse é o progresso real.

## 2) Acompanhando o progresso

```cs Jornada/Foco/Cap71/Progresso.cs
namespace Jornada.Foco.Cap71;

public static class Progresso
{
    /// <summary>Para cada sessão, o tempo médio para perceber; sessões sem divagação são ignoradas.</summary>
    public static IReadOnlyList<double> TemposParaPerceber(IEnumerable<SessaoDeAtencao> sessoes) =>
        sessoes.Where(s => s.Repeticoes > 0).Select(s => s.TempoMedioParaPerceber).ToList();

    /// <summary>Negativo: a pessoa está percebendo mais cedo (melhorando).</summary>
    public static double VariacaoDoTempoParaPerceber(IEnumerable<SessaoDeAtencao> sessoes)
    {
        var tempos = TemposParaPerceber(sessoes);
        if (tempos.Count < 2) throw new ArgumentException("São necessárias ao menos duas sessões com divagação.");
        return tempos[^1] - tempos[0];
    }
}
```

```cs Uso
var sessoes = new[]
{
    new SessaoDeAtencao(600, new[] { new Divagacao(60, 95), new Divagacao(180, 200), new Divagacao(330, 420), new Divagacao(500, 515) }),
    new SessaoDeAtencao(600, new[] { new Divagacao(100, 120), new Divagacao(300, 318), new Divagacao(450, 460) }),
    new SessaoDeAtencao(600, new[] { new Divagacao(200, 208), new Divagacao(410, 416) }),
};

for (int i = 0; i < sessoes.Length; i++)
    Console.WriteLine($"Sessão {i + 1}: {sessoes[i].Repeticoes} repetições · perceber em {sessoes[i].TempoMedioParaPerceber:F0} s");

Console.WriteLine($"Variação (última − primeira): {Progresso.VariacaoDoTempoParaPerceber(sessoes):F0} s");
```

```saida
Sessão 1: 4 repetições · perceber em 40 s
Sessão 2: 3 repetições · perceber em 16 s
Sessão 3: 2 repetições · perceber em 7 s
Variação (última − primeira): -33 s
```

Repare numa sutileza: o **número de repetições caiu** (4 → 2), e isso **não** significa que o treino foi pior. Duas repetições a mais **não** é mérito; o que mostra evolução é **perceber mais cedo**. (E, claro, três sessões de dados inventados não provam nada; são só a ilustração do cálculo.)

## 3) Um contador de respirações

Um exercício clássico: **contar respirações de 1 a 10**; ao perder a conta, **perceber e recomeçar**. Dá para modelar assim:

```cs Jornada/Foco/Cap71/ContadorDeRespiracoes.cs
namespace Jornada.Foco.Cap71;

public sealed class ContadorDeRespiracoes
{
    public const int RespiracoesPorRodada = 10;

    public int ContagemAtual { get; private set; }
    public int RodadasCompletas { get; private set; }
    public int RepeticoesDeRetorno { get; private set; }

    /// <summary>R = respirei e contei; P = percebi que perdi a conta e volto ao 1.</summary>
    public void Processar(string eventos)
    {
        foreach (char c in eventos)
        {
            switch (char.ToUpperInvariant(c))
            {
                case 'R':
                    ContagemAtual++;
                    if (ContagemAtual == RespiracoesPorRodada)
                    {
                        RodadasCompletas++;
                        ContagemAtual = 0;
                    }
                    break;

                case 'P':
                    RepeticoesDeRetorno++;
                    ContagemAtual = 0;
                    break;

                default:
                    throw new ArgumentException($"Evento desconhecido: '{c}'. Use R ou P.", nameof(eventos));
            }
        }
    }
}
```

```cs Uso
var contador = new ContadorDeRespiracoes();
contador.Processar(string.Concat("RRRRR", "P", new string('R', 10), "P", new string('R', 10)));

Console.WriteLine($"Rodadas completas: {contador.RodadasCompletas} · retornos: {contador.RepeticoesDeRetorno} · contagem atual: {contador.ContagemAtual}");
```

```saida
Rodadas completas: 2 · retornos: 2 · contagem atual: 0
```

```cs JornadaTests/Foco/Cap71/Cap71Tests.cs
using Jornada.Foco.Cap71;
using Xunit;

public class Cap71Tests
{
    [Fact]
    public void Sessao_sem_divagacao_tem_zero_repeticoes()
    {
        var s = new SessaoDeAtencao(300, Array.Empty<Divagacao>());
        Assert.Equal(0, s.Repeticoes);
        Assert.Equal(0, s.TempoMedioParaPerceber);
    }

    [Fact]
    public void Divagacoes_sobrepostas_sao_rejeitadas() =>
        Assert.Throws<ArgumentException>(() =>
            new SessaoDeAtencao(300, new[] { new Divagacao(10, 50), new Divagacao(40, 60) }));

    [Fact]
    public void Divagacao_alem_da_sessao_e_rejeitada() =>
        Assert.Throws<ArgumentException>(() => new SessaoDeAtencao(100, new[] { new Divagacao(90, 120) }));

    [Fact]
    public void Percentual_divagando_e_calculado()
    {
        var s = new SessaoDeAtencao(200, new[] { new Divagacao(0, 20), new Divagacao(100, 120) });
        Assert.Equal(20, s.PercentualDivagando, 6);
    }

    [Fact]
    public void Variacao_negativa_significa_perceber_mais_cedo()
    {
        var a = new SessaoDeAtencao(600, new[] { new Divagacao(10, 60) });
        var b = new SessaoDeAtencao(600, new[] { new Divagacao(10, 20) });
        Assert.True(Progresso.VariacaoDoTempoParaPerceber(new[] { a, b }) < 0);
    }

    [Fact]
    public void Contador_completa_rodada_ao_chegar_em_dez()
    {
        var c = new ContadorDeRespiracoes();
        c.Processar(new string('R', 10));
        Assert.Equal(1, c.RodadasCompletas);
        Assert.Equal(0, c.ContagemAtual);
    }

    [Fact]
    public void Perder_a_conta_reinicia_e_conta_uma_repeticao()
    {
        var c = new ContadorDeRespiracoes();
        c.Processar("RRRP");
        Assert.Equal(0, c.ContagemAtual);
        Assert.Equal(1, c.RepeticoesDeRetorno);
    }

    [Fact]
    public void Evento_invalido_e_rejeitado() =>
        Assert.Throws<ArgumentException>(() => new ContadorDeRespiracoes().Processar("RX"));
}
```

::: exercicio A mesma ideia, no trabalho
Aplique o ciclo ao **código**: crie `RegistroDeDesvios` com `Registrar(int minuto, string tarefaAtual, string desvio)` e `Retomar(int minuto)` que devolve **quantos minutos** durou o último desvio. Teste: desvio às 10:00 (600), retomada às 10:07 → 7.

--- resposta
```cs Jornada/Foco/Cap71/RegistroDeDesvios.cs
namespace Jornada.Foco.Cap71;

public sealed record Desvio(int Minuto, string TarefaAtual, string Descricao);

public sealed class RegistroDeDesvios
{
    private readonly List<Desvio> desvios = new();
    private Desvio? aberto;

    public IReadOnlyList<Desvio> Desvios => desvios;

    public void Registrar(int minuto, string tarefaAtual, string descricao)
    {
        if (aberto is not null)
            throw new InvalidOperationException("Já há um desvio aberto: retome a tarefa antes de registrar outro.");

        aberto = new Desvio(minuto, tarefaAtual, descricao);
        desvios.Add(aberto);
    }

    /// <summary>Marca a retomada e devolve a duração do desvio, em minutos.</summary>
    public int Retomar(int minuto)
    {
        if (aberto is null)
            throw new InvalidOperationException("Não há desvio aberto.");
        if (minuto < aberto.Minuto)
            throw new ArgumentOutOfRangeException(nameof(minuto), "A retomada não pode ser antes do desvio.");

        int duracao = minuto - aberto.Minuto;
        aberto = null;
        return duracao;
    }
}
```
```cs Jornada/Foco/Cap71/RegistroDeDesviosTests.cs
using Jornada.Foco.Cap71;
using Xunit;

public class RegistroDeDesviosTests
{
    [Fact]
    public void Duracao_do_desvio()
    {
        var r = new RegistroDeDesvios();
        r.Registrar(600, "Revisar PR", "Abri o e-mail");
        Assert.Equal(7, r.Retomar(607));
    }

    [Fact]
    public void Retomar_sem_desvio_falha() =>
        Assert.Throws<InvalidOperationException>(() => new RegistroDeDesvios().Retomar(10));

    [Fact]
    public void Dois_desvios_abertos_falham()
    {
        var r = new RegistroDeDesvios();
        r.Registrar(10, "A", "x");
        Assert.Throws<InvalidOperationException>(() => r.Registrar(11, "A", "y"));
    }
}
```
Ao fim de uma semana, a lista de `Desvios` mostra **padrões**: o que mais atrai a sua atenção? Mas lembre: o registro serve para **perceber mais cedo**, não para se punir.
:::

::: resumo
- A atenção plena é um **treino**: divagar → perceber → voltar → sustentar; cada **retorno** é uma **repetição**.
- O progresso se vê em **perceber mais cedo** (tempo para perceber), e não em "nunca divagar".
- Em C#, dá para contar **repetições**, medir o tempo divagando e acompanhar a **variação** entre sessões.
- A evidência é **plausível e moderada**: ajuda a atenção e a regulação emocional, mas não cura tudo, e pode ser desconfortável para algumas pessoas.
- No trabalho, a mesma ideia vira **registrar desvios** e **retomar** sem culpa.
:::

::: quiz
? O que conta como uma "repetição" na meditação de atenção?
+ Perceber que a mente divagou e trazê-la de volta
- Ficar sem pensar por 10 minutos
- Respirar profundamente
- Fechar os olhos
> O retorno é o exercício.

? Por que muitas divagações não significam "fracasso"?
+ Cada percepção seguida de retorno é uma repetição do treino
- Porque meditar não exige atenção
- Porque divagar é sempre positivo
- Porque se deve dividir a atenção
> O objetivo não é uma mente vazia.

? Qual o melhor indicador de progresso no capítulo?
+ Perceber a divagação mais cedo (menor tempo para perceber)
- Ter menos repetições
- Meditar por mais horas
- Respirar mais rápido
> A mente continua divagando; você a pega mais cedo.

? Como tratar a evidência sobre meditação?
+ Com equilíbrio: há benefícios plausíveis, mas os estudos têm limitações e não é cura universal
- Como prova definitiva
- Como pseudociência
- Como substituto de qualquer tratamento
> Para algumas pessoas pode ser desconfortável.

? No contador de respirações, o que acontece ao perder a conta?
+ Registra uma repetição de retorno e volta a contagem a zero
- Termina a sessão
- Soma dez respirações
- Gera erro
> 'P' = percebi; recomeço.

? O que o registro de desvios no trabalho pretende?
+ Perceber padrões e retomar mais cedo, sem se punir
- Vigiar quem se distrai
- Medir produtividade para bônus
- Eliminar pausas
> Cultura que não pune o desvio.
:::

::: cartas
Atenção plena :: Treino de atenção: perceber o presente e retomar o foco sem julgamento.
Âncora :: Alvo escolhido para o foco (como a respiração).
Repetição :: Cada ciclo de perceber que divagou e voltar.
Tempo para perceber :: Quanto leva para notar que a mente se desviou.
Atenção aberta :: Observar tudo o que surge sem se prender a nada.
Divagação :: Desvio da mente do alvo escolhido.
Contador de respirações :: Exercício de contar de 1 a 10 e recomeçar ao perder a conta.
Desvio no trabalho :: Saída da tarefa atual por algo não planejado.
:::
