---
id: foco-cap69
order: 91
kind: capitulo
num: 69
group: C# & Mentalidade · Parte 13 · Foco em C#
title: Prática deliberada: treinar no ponto certo de dificuldade
subtitle: Horas de repetição não bastam: o que melhora o desempenho é prática focada, com feedback e dificuldade ajustada. Um treinador adaptativo em C#, o limite diário de esforço e o mito das 10 mil horas.
minutes: 55
hue: 350
objectives:
- Distinguir **repetir** de **praticar deliberadamente**
- Conhecer os ingredientes: **foco total**, **feedback imediato**, **dificuldade na medida** e **descanso**
- Entender por que "**10 mil horas**" é uma **meia verdade**
- Implementar um **treinador adaptativo** que sobe e desce o nível conforme o acerto
- Respeitar o **limite diário** de esforço concentrado
origem: livros/04-foco-em-csharp/content/101-cap69.md
---

::: vila O kata que não evolui
Caio pratica há meses um exercício de código (um *kata*), sempre o mesmo, sempre fácil. Resolve em dez minutos, sem pensar. "Estou treinando muito!" Lia pergunta: "Quando foi a última vez que você **errou** nele?". Silêncio. **Repetir o que já sabemos não cria habilidade nova.**
:::

::: ilustracao img/foco/cap69.svg | A prática deliberada fica na zona de desafio: nem tão fácil que entedie, nem tão difícil que paralise.
:::

## O que o livro diz

> Não é só o **tempo** de prática que importa, mas a **qualidade**: a prática **deliberada** exige atenção total, **metas específicas**, **feedback imediato** e **esforço no limite** do que você consegue.

Pontos do capítulo original:

- O número famoso das "**10 mil horas**" (de estudos sobre violinistas, popularizado em livros) é uma **simplificação**: o que mais importa é **como** se pratica, e a pesquisa mostra variação grande entre áreas e pessoas.
- Quando uma habilidade se torna **automática**, o desenvolvimento **para**: é preciso **voltar a se desafiar** (voltar ao controle consciente) para continuar melhorando.
- A prática eficaz é **cansativa**: especialistas raramente passam de **cerca de 4 horas por dia** de prática intensa.
- O especialista **enxerga estruturas** ("**chunks**"): onde o iniciante vê 20 peças, o mestre vê 3 padrões.
- Ter um **mentor ou feedback** que apresente os erros, em vez de repetir sozinho, acelera tudo.
- O ambiente também conta: **emoções positivas** e um clima de apoio ajudam o aprendizado (mas a famosa "razão de positividade" tem críticas, ver abaixo).

::: cuidado Duas ressalvas
**1)** A frase "10 mil horas" **não** é uma regra: estudos posteriores mostram que a prática deliberada explica **parte** das diferenças de desempenho, variando muito conforme o campo; talento inicial, idade em que começou, qualidade do ensino e sorte também contam.
**2)** A "**razão de positividade**" (a ideia de que ≥ 3 interações positivas para 1 negativa define equipes de sucesso) foi duramente criticada do ponto de vista **matemático**. Vale a **mensagem geral** (ambientes de apoio ajudam), não os números exatos.
:::

## Da vida para o código

| No livro | Em engenharia de software |
|---|---|
| Prática deliberada | **Katas** novos, **desafios** no limite, *code review* feito para aprender |
| Feedback imediato | **Testes** que falham na hora; **pareamento** |
| Dificuldade na medida | Nem tão fácil que entedie, nem tão difícil que paralise |
| Chunks do especialista | **Padrões de projeto**: reconhecer estruturas, não linhas |
| Voltar ao consciente | Revisitar o que "já sabe" com **restrições** novas |
| Limite diário | **Blocos** de foco, não dias de 10 horas "praticando" |

::: figura A zona de desafio
<svg viewBox="0 0 740 240" role="img" aria-label="Eixo de dificuldade versus habilidade: zona de ansiedade acima, zona de desafio na diagonal e zona de tédio abaixo">
  <path class="arrow" d="M60 205L60 20"/><path class="arrow" d="M60 205L700 205"/>
  <text x="54" y="30" text-anchor="end" font-size="11">dificuldade</text>
  <text x="700" y="224" text-anchor="end" font-size="11">habilidade →</text>
  <polygon class="f-bad"  points="62,25 698,25 62,140" style="fill-opacity:.3"/>
  <polygon class="f-good" points="62,140 698,25 698,95 62,195" style="fill-opacity:.5"/>
  <polygon class="f-info" points="62,198 698,98 698,203 62,203" style="fill-opacity:.3"/>
  <text x="150" y="62" font-size="13" font-weight="800">ansiedade</text>
  <text x="150" y="80" font-size="11" class="t-dim">difícil demais: frustra</text>
  <text x="330" y="112" font-size="13" font-weight="800">desafio na medida</text>
  <text x="330" y="130" font-size="11" class="t-dim">o nível acompanha você</text>
  <text x="520" y="175" font-size="13" font-weight="800">tédio</text>
  <text x="520" y="192" font-size="11" class="t-dim">fácil demais: não cria habilidade</text>
</svg>
:::

## 1) Um treinador adaptativo

A ideia central: **ajustar a dificuldade** ao acerto recente. Se você acerta **quase tudo**, é fácil demais: suba. Se erra **muito**, é difícil demais: desça. O ponto bom fica no meio.

```cs Jornada/Foco/Cap69/TreinadorAdaptativo.cs
namespace Jornada.Foco.Cap69;

public sealed record Sessao(int Acertos, int Tentativas, int Minutos)
{
    public double Taxa => Tentativas == 0 ? 0 : (double)Acertos / Tentativas;
}

public enum Ajuste { Descer, Manter, Subir }

public sealed class TreinadorAdaptativo
{
    public const double LimiarParaSubir = 0.85;
    public const double LimiarParaDescer = 0.60;
    public const int NivelMaximo = 10;

    public int Nivel { get; private set; } = 1;

    public Ajuste Registrar(Sessao sessao)
    {
        if (sessao.Tentativas <= 0)
            throw new ArgumentException("A sessão precisa ter tentativas.", nameof(sessao));
        if (sessao.Acertos < 0 || sessao.Acertos > sessao.Tentativas)
            throw new ArgumentException("Acertos devem estar entre 0 e o número de tentativas.", nameof(sessao));

        Ajuste ajuste =
            sessao.Taxa >= LimiarParaSubir ? Ajuste.Subir :
            sessao.Taxa <  LimiarParaDescer ? Ajuste.Descer :
                                              Ajuste.Manter;

        Nivel = ajuste switch
        {
            Ajuste.Subir  => Math.Min(NivelMaximo, Nivel + 1),
            Ajuste.Descer => Math.Max(1, Nivel - 1),
            _             => Nivel,
        };

        return ajuste;
    }
}
```

```cs Uso
var treinador = new TreinadorAdaptativo();
var sessoes = new[]
{
    new Sessao(8, 10, 20),
    new Sessao(9, 10, 25),
    new Sessao(9, 10, 25),
    new Sessao(5, 10, 20),
    new Sessao(7, 10, 30),
};

for (int i = 0; i < sessoes.Length; i++)
{
    var s = sessoes[i];
    var ajuste = treinador.Registrar(s);
    Console.WriteLine($"Sessão {i + 1}: {s.Taxa * 100:F0}% → {ajuste,-6} (nível agora: {treinador.Nivel})");
}
```

```saida
Sessão 1: 80% → Manter (nível agora: 1)
Sessão 2: 90% → Subir  (nível agora: 2)
Sessão 3: 90% → Subir  (nível agora: 3)
Sessão 4: 50% → Descer (nível agora: 2)
Sessão 5: 70% → Manter (nível agora: 2)
```

Os limiares (85% e 60%) são **parâmetros de partida**: pesquisas de aprendizagem sugerem uma zona de **acerto moderado**, mas o valor ideal varia com a tarefa. Experimente e ajuste.

::: dica O feedback é a parte que falta
O treinador só funciona se você **sabe** se acertou. Em programação, isso significa **testes automatizados** ou um **par** que diga "isso está bom?". Sem feedback rápido, praticar é só repetir. Se não houver teste para o seu exercício, **escreva primeiro** o teste: ele **é** o feedback.
:::

## 2) O limite diário de esforço

A prática deliberada cansa (lembre do capítulo 62). Um alerta simples sobre a **carga do dia**:

```cs Jornada/Foco/Cap69/CargaDeTreino.cs
namespace Jornada.Foco.Cap69;

public static class CargaDeTreino
{
    public const int LimiteDiarioEmMinutos = 240;

    public static int Total(IEnumerable<Sessao> sessoesDoDia) => sessoesDoDia.Sum(s => s.Minutos);

    public static string? Aviso(IEnumerable<Sessao> sessoesDoDia, int limite = LimiteDiarioEmMinutos)
    {
        int total = Total(sessoesDoDia);
        return total > limite
            ? $"Treino de {total} min passa de {limite}: a qualidade cai com a fadiga."
            : null;
    }
}
```

```cs Uso
var dia1 = new[]                                // 120 min no total
{
    new Sessao(8, 10, 20), new Sessao(9, 10, 25), new Sessao(9, 10, 25),
    new Sessao(5, 10, 20), new Sessao(7, 10, 30),
};
var dia2 = new[] { new Sessao(7, 10, 150), new Sessao(6, 10, 150) }; // 300 min

Console.WriteLine($"Dia 1: {CargaDeTreino.Total(dia1)} min — {CargaDeTreino.Aviso(dia1) ?? "dentro do limite"}");
Console.WriteLine($"Dia 2: {CargaDeTreino.Total(dia2)} min — {CargaDeTreino.Aviso(dia2) ?? "dentro do limite"}");
```

```saida
Dia 1: 120 min — dentro do limite
Dia 2: 300 min — Treino de 300 min passa de 240: a qualidade cai com a fadiga.
```

```cs JornadaTests/Foco/Cap69/Cap69Tests.cs
using Jornada.Foco.Cap69;
using Xunit;

public class Cap69Tests
{
    [Theory]
    [InlineData(9, 10, Ajuste.Subir)]
    [InlineData(7, 10, Ajuste.Manter)]
    [InlineData(5, 10, Ajuste.Descer)]
    public void Ajuste_conforme_a_taxa(int acertos, int tentativas, Ajuste esperado) =>
        Assert.Equal(esperado, new TreinadorAdaptativo().Registrar(new Sessao(acertos, tentativas, 10)));

    [Fact]
    public void Nivel_nunca_fica_abaixo_de_um()
    {
        var t = new TreinadorAdaptativo();
        t.Registrar(new Sessao(0, 10, 10));
        Assert.Equal(1, t.Nivel);
    }

    [Fact]
    public void Nivel_nunca_passa_do_maximo()
    {
        var t = new TreinadorAdaptativo();
        for (int i = 0; i < 30; i++) t.Registrar(new Sessao(10, 10, 10));
        Assert.Equal(TreinadorAdaptativo.NivelMaximo, t.Nivel);
    }

    [Fact]
    public void Sessao_sem_tentativas_e_rejeitada() =>
        Assert.Throws<ArgumentException>(() => new TreinadorAdaptativo().Registrar(new Sessao(0, 0, 10)));

    [Fact]
    public void Acertos_maiores_que_tentativas_sao_rejeitados() =>
        Assert.Throws<ArgumentException>(() => new TreinadorAdaptativo().Registrar(new Sessao(11, 10, 10)));

    [Fact]
    public void Aviso_so_aparece_acima_do_limite()
    {
        Assert.Null(CargaDeTreino.Aviso(new[] { new Sessao(1, 1, 240) }));
        Assert.NotNull(CargaDeTreino.Aviso(new[] { new Sessao(1, 1, 241) }));
    }
}
```

## 3) Chunks: o olhar do especialista

Um iniciante lê `for`, `if`, `new`, `+=`... e **cada token** ocupa sua memória de trabalho. Um especialista lê "**acumulador com filtro**" e já está no próximo passo. Isso não é mágica: são **padrões** reconhecidos pela exposição repetida, com feedback. Dois jeitos de acelerar:

1. **Nomeie os padrões** que você encontra ("isso é um *Strategy*", "isso é um *map-reduce*") e anote em um caderno.
2. **Reescreva** o mesmo trecho de duas maneiras (laço e LINQ, herança e composição), comparando o que cada uma **comunica**.

::: exercicio Meta do dia
Crie uma classe `MetaDeTreino` com a **meta de acertos por dia** (por exemplo, 30) e o método `bool Atingida(IEnumerable<Sessao> sessoes)`. Em seguida, escreva um teste mostrando que **sessões em que a pessoa errou muito** (taxa < 60%) **não contam** para a meta (só somam as sessões em zona útil: taxa ≥ 60%).

--- resposta
```cs Jornada/Foco/Cap69/MetaDeTreino.cs
namespace Jornada.Foco.Cap69;

public sealed class MetaDeTreino
{
    private readonly int acertosPorDia;

    public MetaDeTreino(int acertosPorDia)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(acertosPorDia);
        this.acertosPorDia = acertosPorDia;
    }

    public bool Atingida(IEnumerable<Sessao> sessoes) =>
        sessoes.Where(s => s.Taxa >= TreinadorAdaptativo.LimiarParaDescer)
               .Sum(s => s.Acertos) >= acertosPorDia;
}
```
```cs Jornada/Foco/Cap69/MetaDeTreinoTests.cs
using Jornada.Foco.Cap69;
using Xunit;

public class MetaDeTreinoTests
{
    [Fact]
    public void Sessoes_dificeis_demais_nao_contam()
    {
        var meta = new MetaDeTreino(15);
        var sessoes = new[]
        {
            new Sessao(8, 10, 20),   // conta: 8
            new Sessao(5, 10, 20),   // taxa 50%: não conta
            new Sessao(9, 10, 20),   // conta: 9
        };

        Assert.True(meta.Atingida(sessoes));          // 8 + 9 = 17 ≥ 15
        Assert.False(new MetaDeTreino(18).Atingida(sessoes)); // sem os 5, só 17
    }
}
```
Por que não contar as sessões muito difíceis? Para que a meta **não incentive** o pior tipo de prática: errar o tempo todo, em um nível acima do seu, só gera frustração; o objetivo é a **zona de desafio**.
:::

::: resumo
- **Repetir** o que já se domina não cria habilidade; a prática **deliberada** pede foco, **feedback imediato** e **dificuldade na medida**.
- "**10 mil horas**" é meia verdade: a **qualidade** da prática e outros fatores (talento, idade de início, ensino) contam.
- Um **treinador adaptativo** sobe e desce o nível conforme a taxa de acerto (limiares são parâmetros a ajustar).
- Respeite o **limite diário** de esforço concentrado (cerca de 4 horas, como ordem de grandeza).
- **Chunks** e **padrões** nomeados fazem o especialista "ver" mais com menos esforço.
:::

::: quiz
? O que diferencia a prática deliberada de simplesmente repetir?
+ Foco, feedback imediato e esforço no limite da habilidade
- Mais horas por dia
- Fazer sempre o mesmo exercício fácil
- Praticar em silêncio absoluto
> Qualidade, não só quantidade.

? Qual a ressalva sobre as "10 mil horas"?
+ É uma simplificação: o que mais importa é como se pratica, e há muita variação entre áreas e pessoas
- É uma lei exata
- Vale só para músicos
- Foi totalmente desmentida e não significa nada
> Prática explica parte das diferenças.

? Para que serve o feedback imediato?
+ Permitir corrigir o erro enquanto ainda se lembra do que foi feito
- Dar elogios
- Substituir a prática
- Marcar o tempo
> Em código: testes e pareamento.

? Por que o treinador desce o nível quando o acerto é baixo?
+ Difícil demais gera frustração, e não aprendizado
- Para punir
- Para economizar tempo
- Porque o limiar é sempre 50%
> Fique na zona de desafio.

? O que são "chunks"?
+ Estruturas que o especialista reconhece de uma vez, em vez de peça por peça
- Pedaços de memória do sistema
- Reuniões curtas
- Um tipo de teste
> Como padrões de projeto.

? Como tratar os limiares 85% e 60%?
+ Como parâmetros de partida para experimentar e ajustar
- Como constantes científicas
- Como erros
- Como regras legais
> Dependem da tarefa e da pessoa.
:::

::: cartas
Prática deliberada :: Prática focada, com feedback, no limite da habilidade.
Feedback imediato :: Retorno rápido que mostra o erro enquanto ele está fresco.
Zona de desafio :: Nível em que a tarefa não é fácil demais nem difícil demais.
Chunk :: Estrutura reconhecida de uma vez pelo especialista.
Kata :: Exercício curto e repetível para treinar uma habilidade.
Limite diário :: Teto de esforço concentrado por dia (ordem de grandeza: 4 h).
Razão de positividade :: Ideia de proporção de interações positivas, com críticas matemáticas.
10 mil horas :: Meia verdade: a qualidade da prática importa mais que o número.
:::
