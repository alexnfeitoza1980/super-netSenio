---
id: foco-cap70
order: 92
kind: capitulo
num: 70
group: C# & Mentalidade · Parte 13 · Foco em C#
title: Jogos de atenção: o teste Go/No-Go e a transferência
subtitle: Treinos de atenção em forma de jogo existem e medem coisas reais, como a inibição de impulsos. Um analisador Go/No-Go em C# e, sobretudo, a pergunta honesta: melhorar no jogo melhora a vida?
minutes: 50
hue: 15
objectives:
- Entender o **teste Go/No-Go** e o que ele mede (**inibição** de respostas)
- Distinguir **omissões**, **comissões** e **acertos** e calcular a **taxa de inibição**
- Conhecer a controvérsia sobre **treinos cerebrais** e o que é **transferência**
- Avaliar se o ganho em uma **tarefa treinada** passa para outra **não treinada**
- Evitar tanto o **entusiasmo ingênuo** quanto o **cinismo**
origem: livros/04-foco-em-csharp/content/102-cap70.md
---

::: vila O aplicativo milagroso
Caio ganhou de presente um aplicativo que promete "turbinar o foco em 10 minutos por dia". Depois de duas semanas, ele está **ótimo no jogo**. Mas ainda perde a concentração na leitura de código. Lia pergunta: "Você melhorou **no jogo** ou **no foco**? São coisas **diferentes** — e a segunda é a que importa".
:::

::: ilustracao img/foco/cap70.svg | No Go/No-Go o difícil é frear: a maioria dos estímulos pede apertar, e o raro pede segurar.
:::

## O que o livro diz

> Os **jogos de treino** da atenção têm base científica: o cérebro é **plástico**, e a prática com tarefas bem desenhadas fortalece circuitos de atenção, inibição e memória de trabalho. Mas o campo também tem **exageros comerciais**.

O que vale guardar:

- A **neuroplasticidade** é real: repetir uma tarefa que exige atenção muda circuitos cerebrais envolvidos.
- Treinos de **atenção seletiva** e de **inibição** (como o Go/No-Go) tiveram resultados promissores em alguns estudos, inclusive com crianças com dificuldades de atenção.
- A grande pergunta é a **transferência**: o ganho **na tarefa treinada** se estende a **outras** tarefas e à **vida real**?
- Há **controvérsia**: algumas revisões grandes concluíram que os ganhos tendem a ficar **na tarefa treinada**, sem transferência clara; outras encontraram efeitos em grupos e tarefas específicas.
- Resultados mais consistentes aparecem em **práticas** como atividade física, meditação e aprender habilidades novas e complexas, que também treinam atenção.

::: cuidado Não é nem milagre nem fraude
Dizer que "treinar com jogos **não** funciona" é tão exagerado quanto dizer que "funciona". A resposta honesta é: **melhora no que se treina**, com **transferência pequena ou incerta** para o resto, e o **efeito depende** do tipo de treino, da pessoa e do que se mede. Duvide de qualquer promessa de "+X% de foco" sem **grupo de controle** e **tarefa diferente** no teste.
:::

## Da vida para o código

| No livro | Em engenharia de software |
|---|---|
| Teste Go/No-Go | **Pressionar** ou **não pressionar** conforme a regra |
| Inibição de resposta | **Não responder** a uma notificação sem relevância |
| Tarefa treinada × não treinada | **Teste unitário** do que treinou × **uso real** |
| Transferência | O **ganho vale na vida**? |
| Efeito de placebo/expectativa | **Benchmarks** que você escolheu para o código que otimizou |
| Grupo de controle | **Comparar** com quem não fez a mudança |

::: figura O que é Go/No-Go
<svg viewBox="0 0 740 190" role="img" aria-label="Estímulos verdes pedem resposta e vermelhos pedem inibição">
  <g font-size="13" text-anchor="middle" font-weight="800">
    <circle class="f-good" cx="70"  cy="80" r="44"/><text x="70"  y="86">GO</text>
    <circle class="f-bad"  cx="180" cy="80" r="44"/><text x="180" y="86">NO-GO</text>
    <circle class="f-good" cx="290" cy="80" r="44"/><text x="290" y="86">GO</text>
    <circle class="f-good" cx="400" cy="80" r="44"/><text x="400" y="86">GO</text>
  </g>
  <g font-size="12" text-anchor="middle">
    <text x="70" y="150">pressione</text><text x="180" y="150">NÃO pressione</text>
    <text x="290" y="150">pressione</text><text x="400" y="150">pressione</text>
  </g>
  <g font-size="12" class="t-dim">
    <text x="480" y="70">A maioria dos estímulos é "go":</text>
    <text x="480" y="90">o hábito de pressionar se forma,</text>
    <text x="480" y="110">e frear no "no-go" fica difícil.</text>
  </g>
</svg>
:::

## 1) Analisando uma rodada de Go/No-Go

O jogo apresenta estímulos; a regra é **responder** aos "Go" e **não responder** aos "No-Go". O que se mede:

- **Acerto** (resposta em Go);
- **Omissão** (não respondeu a um Go: falha de atenção);
- **Comissão** (respondeu a um No-Go: falha de **inibição**);
- **Rejeição correta** (não respondeu a um No-Go);
- **Tempo de reação** nos acertos.

```cs Jornada/Foco/Cap70/GoNoGo.cs
namespace Jornada.Foco.Cap70;

public enum TipoDeEstimulo { Go, NoGo }

public sealed record Tentativa(TipoDeEstimulo Estimulo, bool Respondeu, int? TempoDeReacaoMs = null);

public sealed record ResultadoGoNoGo(
    int Acertos, int Omissoes, int Comissoes, int RejeicoesCorretas, double TempoMedioDosAcertosMs)
{
    public int TotalGo => Acertos + Omissoes;
    public int TotalNoGo => Comissoes + RejeicoesCorretas;

    /// <summary>Fração de No-Go em que a pessoa soube NÃO responder (0 a 1).</summary>
    public double TaxaDeInibicao => TotalNoGo == 0 ? 0 : (double)RejeicoesCorretas / TotalNoGo;
}

public static class AnalisadorGoNoGo
{
    public static ResultadoGoNoGo Analisar(IEnumerable<Tentativa> tentativas)
    {
        int acertos = 0, omissoes = 0, comissoes = 0, rejeicoes = 0;
        var tempos = new List<int>();

        foreach (var t in tentativas)
        {
            if (t.Respondeu && t.TempoDeReacaoMs is null)
                throw new ArgumentException("Resposta sem tempo de reação.", nameof(tentativas));

            switch (t.Estimulo, t.Respondeu)
            {
                case (TipoDeEstimulo.Go, true):
                    acertos++;
                    tempos.Add(t.TempoDeReacaoMs!.Value);
                    break;
                case (TipoDeEstimulo.Go, false):   omissoes++;  break;
                case (TipoDeEstimulo.NoGo, true):  comissoes++; break;
                case (TipoDeEstimulo.NoGo, false): rejeicoes++; break;
            }
        }

        double media = tempos.Count == 0 ? 0 : tempos.Average();
        return new ResultadoGoNoGo(acertos, omissoes, comissoes, rejeicoes, media);
    }
}
```

```cs Uso
var rodada = new[]
{
    new Tentativa(TipoDeEstimulo.Go,   true,  320),
    new Tentativa(TipoDeEstimulo.Go,   true,  300),
    new Tentativa(TipoDeEstimulo.NoGo, false),
    new Tentativa(TipoDeEstimulo.Go,   true,  350),
    new Tentativa(TipoDeEstimulo.NoGo, true,  280),   // errou: apertou quando não devia
    new Tentativa(TipoDeEstimulo.Go,   false),        // errou: deixou passar
    new Tentativa(TipoDeEstimulo.Go,   true,  310),
    new Tentativa(TipoDeEstimulo.NoGo, false),
    new Tentativa(TipoDeEstimulo.NoGo, false),
    new Tentativa(TipoDeEstimulo.Go,   true,  330),
};

var r = AnalisadorGoNoGo.Analisar(rodada);
Console.WriteLine($"Go: {r.Acertos}/{r.TotalGo} acertos · {r.Omissoes} omissão(ões)");
Console.WriteLine($"No-Go: {r.Comissoes} comissão(ões) · inibição {r.TaxaDeInibicao * 100:F0}%");
Console.WriteLine($"Tempo médio dos acertos: {r.TempoMedioDosAcertosMs:F0} ms");
```

```saida
Go: 5/6 acertos · 1 omissão(ões)
No-Go: 1 comissão(ões) · inibição 75%
Tempo médio dos acertos: 322 ms
```

Por que o teste tem **mais** Go do que No-Go? Porque a maioria "go" cria o **hábito de responder**, e o raro "no-go" exige **frear** um impulso já em andamento. É a mesma situação de quem sente o dedo ir para o botão de **"enviar"** sem ter relido.

## 2) Melhorou no jogo… e na vida?

Para avaliar a **transferência**, meça uma tarefa **diferente** da treinada, antes e depois. O código abaixo compara os dois ganhos:

```cs Jornada/Foco/Cap70/Transferencia.cs
namespace Jornada.Foco.Cap70;

public sealed record Medidas(double AntesTreinada, double DepoisTreinada, double AntesOutra, double DepoisOutra);

public sealed record AvaliacaoDeTransferencia(double GanhoTreinada, double GanhoOutra)
{
    /// <summary>Quanto do ganho na tarefa treinada "apareceu" na outra (0 a 1+).</summary>
    public double Razao => GanhoTreinada <= 0 ? 0 : Math.Max(0, GanhoOutra) / GanhoTreinada;

    public string Resumo(double limiar = 0.5) => GanhoTreinada <= 0
        ? "Sem ganho nem na tarefa treinada."
        : Razao >= limiar
            ? $"Houve transferência ({Razao * 100:F0}% do ganho apareceu na outra tarefa)."
            : $"Melhorou na tarefa treinada; transferência pequena ({Razao * 100:F0}%).";
}

public static class Transferencia
{
    public static AvaliacaoDeTransferencia Avaliar(Medidas m) =>
        new(m.DepoisTreinada - m.AntesTreinada, m.DepoisOutra - m.AntesOutra);
}
```

```cs Uso
var apps = new (string Nome, Medidas M)[]
{
    ("Aplicativo A (só jogo)",            new Medidas(60, 85, 50, 52)),
    ("Programa B (jogo + exercício)",     new Medidas(60, 80, 50, 62)),
    ("Programa C (sem efeito)",           new Medidas(60, 58, 50, 50)),
};

foreach (var (nome, m) in apps)
    Console.WriteLine($"{nome,-30} {Transferencia.Avaliar(m).Resumo()}");
```

```saida
Aplicativo A (só jogo)         Melhorou na tarefa treinada; transferência pequena (8%).
Programa B (jogo + exercício)  Houve transferência (60% do ganho apareceu na outra tarefa).
Programa C (sem efeito)        Sem ganho nem na tarefa treinada.
```

Os dados são **inventados** para ilustrar a lógica de avaliação, e não representam nenhum produto real. O que importa é o **hábito**: antes de acreditar em um treino (ou em uma otimização de código!), pergunte **"o que foi medido, e é diferente do que foi treinado?"**.

```cs JornadaTests/Foco/Cap70/Cap70Tests.cs
using Jornada.Foco.Cap70;
using Xunit;

public class Cap70Tests
{
    [Fact]
    public void Conta_as_quatro_categorias()
    {
        var r = AnalisadorGoNoGo.Analisar(new[]
        {
            new Tentativa(TipoDeEstimulo.Go, true, 300),
            new Tentativa(TipoDeEstimulo.Go, false),
            new Tentativa(TipoDeEstimulo.NoGo, true, 250),
            new Tentativa(TipoDeEstimulo.NoGo, false),
        });

        Assert.Equal(1, r.Acertos);
        Assert.Equal(1, r.Omissoes);
        Assert.Equal(1, r.Comissoes);
        Assert.Equal(1, r.RejeicoesCorretas);
        Assert.Equal(0.5, r.TaxaDeInibicao, 6);
    }

    [Fact]
    public void Resposta_sem_tempo_e_rejeitada() =>
        Assert.Throws<ArgumentException>(() =>
            AnalisadorGoNoGo.Analisar(new[] { new Tentativa(TipoDeEstimulo.Go, true) }));

    [Fact]
    public void Sem_nogo_a_inibicao_e_zero() =>
        Assert.Equal(0, AnalisadorGoNoGo.Analisar(new[] { new Tentativa(TipoDeEstimulo.Go, true, 300) }).TaxaDeInibicao);

    [Fact]
    public void Ganho_pequeno_na_outra_tarefa_nao_e_transferencia()
    {
        var a = Transferencia.Avaliar(new Medidas(60, 85, 50, 52));
        Assert.True(a.Razao < 0.5);
    }

    [Fact]
    public void Piora_na_outra_tarefa_nao_gera_razao_negativa()
    {
        var a = Transferencia.Avaliar(new Medidas(60, 80, 50, 45));
        Assert.Equal(0, a.Razao);
    }

    [Fact]
    public void Sem_ganho_na_treinada_o_resumo_avisa() =>
        Assert.StartsWith("Sem ganho", Transferencia.Avaliar(new Medidas(60, 58, 50, 50)).Resumo());
}
```

::: exercicio Cuidado com o chute
Um jogador que **responde a tudo** terá 100% de acertos nos Go, mas também 100% de comissões. Crie `double Pontuacao(ResultadoGoNoGo r)` que combine as duas taxas (por exemplo, a **média** entre a **taxa de acerto em Go** e a **taxa de inibição**) e mostre, em um teste, que o jogador que **aperta sempre** tem pontuação **menor** do que um que errou algumas vezes em cada tipo.

--- resposta
```cs Jornada/Foco/Cap70/Pontuacao.cs
namespace Jornada.Foco.Cap70;

public static class Pontuacao
{
    public static double Calcular(ResultadoGoNoGo r)
    {
        double acertoGo = r.TotalGo == 0 ? 0 : (double)r.Acertos / r.TotalGo;
        return (acertoGo + r.TaxaDeInibicao) / 2;
    }
}
```
```cs Jornada/Foco/Cap70/PontuacaoTests.cs
using Jornada.Foco.Cap70;
using Xunit;

public class PontuacaoTests
{
    [Fact]
    public void Apertar_sempre_nao_vence_quem_erra_pouco()
    {
        var apertaSempre = new ResultadoGoNoGo(Acertos: 10, Omissoes: 0, Comissoes: 10, RejeicoesCorretas: 0, TempoMedioDosAcertosMs: 250);
        var cuidadoso    = new ResultadoGoNoGo(Acertos: 8,  Omissoes: 2, Comissoes: 2,  RejeicoesCorretas: 8, TempoMedioDosAcertosMs: 340);

        Assert.True(Pontuacao.Calcular(cuidadoso) > Pontuacao.Calcular(apertaSempre));
    }
}
```
A métrica **única** (só acertos em Go) seria "enganável" pelo chute; combiná-la com a inibição **fecha o atalho**. É o mesmo cuidado que se tem ao escolher métricas de software: uma métrica sozinha convida a "otimizar a métrica" em vez do que importa.
:::

::: resumo
- O **teste Go/No-Go** mede **inibição**: responder aos "go" e **frear** nos "no-go"; analise **acertos, omissões, comissões e rejeições corretas**.
- O cérebro é **plástico** e treinos podem melhorar a **tarefa treinada**; a **transferência** para outras tarefas e para a vida é **incerta e debatida**.
- Avalie treinos (e otimizações!) com uma **tarefa diferente** da treinada e, idealmente, **grupo de controle**.
- Prefira práticas com evidência mais consistente (exercício, meditação, habilidades novas e complexas), sem prometer milagres.
- **Métricas únicas** são enganáveis; combine pelo menos duas.
:::

::: quiz
? O que o teste Go/No-Go mede?
+ A capacidade de inibir uma resposta quando a regra pede
- A inteligência geral
- A memória de longo prazo
- A velocidade de digitação apenas
> Frear o impulso nos "no-go".

? Qual o erro de "comissão"?
+ Responder a um estímulo No-Go
- Não responder a um Go
- Responder devagar
- Responder duas vezes
> É uma falha de inibição.

? O que é "transferência" em treino cerebral?
+ O ganho na tarefa treinada se estender a outras tarefas e à vida real
- Copiar o jogo para outro aparelho
- Mudar de aplicativo
- Transferir dados para a nuvem
> A grande pergunta do campo.

? Qual a leitura honesta da evidência sobre jogos de atenção?
+ Melhoram o que se treina, e a transferência é pequena ou incerta e depende do caso
- Funcionam sempre
- Nunca funcionam
- Só funcionam em adultos
> Evite o entusiasmo ingênuo e o cinismo.

? Por que a maioria dos estímulos é Go?
+ Para criar o hábito de responder, tornando a inibição mais difícil
- Para economizar tempo
- Por acaso
- Para facilitar o código
> O raro no-go exige frear um impulso em andamento.

? Por que combinar duas taxas na pontuação?
+ Para impedir que "apertar sempre" gere uma nota alta
- Para dobrar a pontuação
- Para evitar testes
- Para usar mais variáveis
> Métrica única convida a "otimizar a métrica".
:::

::: cartas
Go/No-Go :: Teste em que se responde a alguns estímulos e se inibe a resposta a outros.
Inibição :: Capacidade de frear uma resposta automática.
Omissão :: Não responder a um estímulo Go.
Comissão :: Responder a um estímulo No-Go.
Neuroplasticidade :: Capacidade do cérebro de se reorganizar com a prática.
Transferência :: Ganho na tarefa treinada que aparece em outras tarefas.
Grupo de controle :: Grupo de comparação que não recebe a intervenção.
Métrica enganável :: Número que pode subir sem melhorar o que realmente importa.
:::
