---
id: foco-cap58
order: 80
kind: capitulo
num: 58
group: C# & Mentalidade · Parte 13 · Foco em C#
title: Foco em C#: o mapa
subtitle: A atenção é o recurso mais escasso de quem programa. Veja o plano da Parte 13 — o foco interno, o foco no outro e o foco externo — e como cada ideia vira hábito, estrutura e código C# testável.
minutes: 30
hue: 262
objectives:
- Entender a tese central: **a atenção é uma habilidade que se treina** e sustenta todo o resto
- Conhecer o **foco triplo** (interno, no outro e externo) e o mapa da Parte 13
- Saber **como a Parte 13 usa a ciência**: como metáfora e ponto de partida, com honestidade sobre os limites
- Montar uma primeira **autoavaliação de foco** em C#
origem: livros/04-foco-em-csharp/content/90-cap58.md
---

::: vila O dia em que Lia não conseguiu ler duas páginas
Lia abre um artigo técnico de que gosta. Duas páginas depois, percebe que **não leu nada**: os olhos passaram pelas frases, mas a cabeça estava na reunião de ontem, no chat aberto na outra tela e no *build* que podia ter terminado. O código dela anda bom; o que anda curto é a **atenção**. Foi isso que a fez procurar um livro sobre foco — e decidir, de novo, traduzi-lo em prática de engenharia.
:::

::: ilustracao img/foco/cap58.svg | Sem uma das pernas o banco cai: o foco precisa do interno, do outro e do externo.
:::

## De onde vêm as ideias desta parte

*Foco*, do psicólogo **Daniel Goleman** (autor de *Inteligência Emocional*), reúne pesquisas de psicologia e neurociência para defender uma ideia simples: **a qualidade da nossa atenção determina a qualidade do que fazemos** — aprender, decidir, nos relacionar, liderar. E mais: a atenção é como um **músculo** — pouco usada, definha; bem treinada, melhora.

Para organizar o assunto, o livro propõe uma tríade:

| Foco | Pergunta que ele responde | Se falta… |
|---|---|---|
| **Interno** | O que acontece dentro de mim? (emoções, intuição, valores, autocontrole) | você fica **desorientado** |
| **No outro** | O que as pessoas ao meu redor sentem e pensam? (empatia, contexto) | você fica **desinformado** |
| **Externo** | Em que **sistema** maior tudo isso acontece? (padrões, efeitos distantes) | você é **pego de surpresa** |

::: figura O foco triplo
<svg viewBox="0 0 740 292" role="img" aria-label="Três círculos que se cruzam: interno, no outro e externo, em torno do foco bem equilibrado">
  <circle cx="270" cy="110" r="90" fill="var(--prim, #6a3fd6)" opacity=".25" stroke="var(--prim, #6a3fd6)"/>
  <circle cx="470" cy="110" r="90" fill="var(--info, #1f7ae0)" opacity=".25" stroke="var(--info, #1f7ae0)"/>
  <circle cx="370" cy="190" r="90" fill="var(--good, #1a9b6c)" opacity=".25" stroke="var(--good, #1a9b6c)"/>
  <text x="230" y="95" text-anchor="middle" font-weight="800">Foco interno</text><text x="230" y="114" text-anchor="middle" font-size="11" class="t-dim">autoconsciência · autocontrole</text>
  <text x="510" y="95" text-anchor="middle" font-weight="800">Foco no outro</text><text x="510" y="114" text-anchor="middle" font-size="11" class="t-dim">empatia · contexto</text>
  <text x="370" y="238" text-anchor="middle" font-weight="800">Foco externo</text><text x="370" y="254" text-anchor="middle" font-size="11" class="t-dim">sistemas · longo prazo</text>
  <text x="370" y="150" text-anchor="middle" font-weight="800" font-size="13">equilíbrio</text>
</svg>
:::

## O mapa da Parte 13

| Parte do livro | Capítulos aqui | O que vira em C# |
|---|---|---|
| **A anatomia da atenção** | 59 a 62 | Política de interrupções, automação que libera a mente, agenda com "casulo criativo", modelo de fadiga e pausas |
| **Autoconsciência** | 63 e 64 | Avaliação 360, calibração de intuição, "se–então" e confirmações com atraso |
| **Lendo os outros** | 65 e 66 | Sinais de frustração do usuário, comunicação por contexto, latência de *review* |
| **O contexto maior** | 67 e 68 | Simulação de laços de feedback, análise de impacto e regra 80/20 |
| **Prática inteligente** | 69 a 71 | Prática deliberada adaptativa, jogo *Go/No-Go*, ciclo "divaguei–percebi–voltei" |
| **O líder bem focado** | 72 e 73 | Estilos de liderança (Strategy), decisões com horizonte e custos externos |
| **Projeto final** | Lab 8 | **Guardião do Foco**: tudo junto em um pequeno sistema |

::: regra A frase que guia a parte
**Onde você coloca a atenção, você coloca a vida.** Em código: o que você deixa entrar na mente (notificações), o que você automatiza para ela não gastar energia (rotinas), o que você mede para enxergar o invisível (sistemas) e o que você cultiva em equipe (empatia e liderança).
:::

## Uma nota de honestidade científica

Goleman é um divulgador: ele **resume** estudos complexos para o grande público, e a ciência continua avançando. Algumas observações, que você verá repetidas ao longo da parte:

- **Metáfora útil não é lei da natureza.** Dizer que "a atenção é um músculo" ajuda a pensar em treino e descanso; não significa que o cérebro seja literalmente um bíceps.
- **Estudos são revistos.** Resultados famosos (como o teste do *marshmallow* ou a "regra das 10 mil horas") foram **qualificados** por pesquisas posteriores; faremos isso quando chegarmos lá.
- **Números ilustrativos.** Nos exemplos de código usamos valores (10 minutos perdidos por interrupção, 4 horas de prática profunda por dia) como **parâmetros ajustáveis**, não como constantes universais.
- **Teste na sua vida.** O ponto forte do software é que você pode **medir** o próprio caso: se um hábito ajuda ou não, **os dados dizem**.

::: pense Aplicando a regra do livro à própria Parte 13
Ao ler cada capítulo, pergunte-se: *"Que parte disso eu consigo observar e medir no meu dia?"* Fique com isso. O resto trate como hipótese interessante.
:::

## Mãos ao código: uma autoavaliação de foco

Para aquecer, vamos modelar a tríade como dados e criar uma **autoavaliação** simples (de 0 a 10) que aponta qual dos três focos está mais fraco:

```cs Jornada/Foco/Cap58/AutoavaliacaoDeFoco.cs
namespace Jornada.Foco.Cap58;

public enum TipoDeFoco { Interno, NoOutro, Externo }

public sealed record AutoavaliacaoDeFoco(int Interno, int NoOutro, int Externo)
{
    public AutoavaliacaoDeFoco() : this(0, 0, 0) { }

    public int Nota(TipoDeFoco tipo) => tipo switch
    {
        TipoDeFoco.Interno => Interno,
        TipoDeFoco.NoOutro => NoOutro,
        TipoDeFoco.Externo => Externo,
        _ => throw new ArgumentOutOfRangeException(nameof(tipo))
    };

    /// <summary>O foco com a menor nota (em empate, a ordem Interno → NoOutro → Externo desempata).</summary>
    public TipoDeFoco MaisFraco() =>
        Enum.GetValues<TipoDeFoco>().OrderBy(Nota).First();

    public double Media => (Interno + NoOutro + Externo) / 3.0;

    /// <summary>Equilíbrio: 1,0 = três notas iguais; cai conforme a distância entre a maior e a menor.</summary>
    public double Equilibrio
    {
        get
        {
            var notas = new[] { Interno, NoOutro, Externo };
            return 1.0 - (notas.Max() - notas.Min()) / 10.0;
        }
    }

    public static AutoavaliacaoDeFoco Criar(int interno, int noOutro, int externo)
    {
        foreach (var (nome, nota) in new[] { ("interno", interno), ("no outro", noOutro), ("externo", externo) })
            if (nota is < 0 or > 10)
                throw new ArgumentOutOfRangeException(nome, nota, "A nota deve estar entre 0 e 10.");

        return new AutoavaliacaoDeFoco(interno, noOutro, externo);
    }

    public string Orientacao() => MaisFraco() switch
    {
        TipoDeFoco.Interno => "Treine autoconsciência: registre decisões e emoções, peça feedback (caps. 63 e 64).",
        TipoDeFoco.NoOutro => "Treine empatia e leitura de contexto: ouça mais, observe sinais do usuário (caps. 65 e 66).",
        TipoDeFoco.Externo => "Treine visão de sistemas: mapeie efeitos de longo prazo (caps. 67 e 68).",
        _ => ""
    };
}
```

```cs Uso
var lia = AutoavaliacaoDeFoco.Criar(interno: 5, noOutro: 8, externo: 4);

Console.WriteLine($"Média: {lia.Media:F1} · equilíbrio: {lia.Equilibrio:P0}".Replace(" %", "%"));
Console.WriteLine($"Foco mais fraco: {lia.MaisFraco()}");
Console.WriteLine(lia.Orientacao());
```

```saida
Média: 5,7 · equilíbrio: 60%
Foco mais fraco: Externo
Treine visão de sistemas: mapeie efeitos de longo prazo (caps. 67 e 68).
```

```cs JornadaTests/Foco/Cap58/AutoavaliacaoDeFocoTests.cs
using Jornada.Foco.Cap58;
using Xunit;

public class AutoavaliacaoDeFocoTests
{
    [Fact]
    public void Aponta_o_foco_mais_fraco()
    {
        var a = AutoavaliacaoDeFoco.Criar(7, 3, 9);
        Assert.Equal(TipoDeFoco.NoOutro, a.MaisFraco());
    }

    [Fact]
    public void Notas_iguais_dao_equilibrio_total() =>
        Assert.Equal(1.0, AutoavaliacaoDeFoco.Criar(6, 6, 6).Equilibrio);

    [Theory]
    [InlineData(-1, 5, 5)]
    [InlineData(5, 11, 5)]
    public void Notas_fora_do_intervalo_sao_recusadas(int i, int o, int e) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => AutoavaliacaoDeFoco.Criar(i, o, e));
}
```

::: exercicio Sua própria tríade
1. Preencha `AutoavaliacaoDeFoco.Criar(...)` com notas **honestas** para você hoje.
2. Qual foco ficou mais fraco? Escreva **uma ação de 10 minutos** para o foco mais fraco e agende-a.
3. Por que a pergunta "como as *outras* pessoas avaliariam minha nota?" ajuda a calibrar? (Pista: capítulo 63.)

--- resposta
1–2. Não há resposta única. Um exemplo: foco **externo** fraco → "10 minutos por semana lendo o *post-mortem* de um incidente de outra empresa e anotando qual **sistema** falhou". Foco **interno** fraco → "ao fim do dia, anotar uma decisão e como me senti". Foco **no outro** fraco → "na próxima reunião, repetir com minhas palavras o que a pessoa disse antes de responder".
3. Porque a **autoimagem costuma ser otimista** (a maioria se acha "acima da média"). Comparar a sua nota com a de colegas de confiança — a ideia da avaliação 360 — revela os **pontos cegos**. Veremos como modelar isso no capítulo 63.
:::

::: resumo
- **Foco** (Daniel Goleman): a atenção é uma habilidade treinável que sustenta aprendizado, decisão, relacionamento e liderança.
- **Foco triplo:** **interno** (autoconsciência e autocontrole), **no outro** (empatia e contexto), **externo** (sistemas e longo prazo). Falta de cada um tem um custo (desorientado, desinformado, surpreendido).
- A Parte 13 traduz cada ideia em **hábito + estrutura + código C#** testado, e termina no **Laboratório 8 (Guardião do Foco)**.
- Honestidade: ciência popular **simplifica**; trate resultados como **hipóteses** e **meça no seu caso**.
:::

::: quiz
? Qual é a tese central do livro Foco, segundo este capítulo?
+ A qualidade da atenção determina a qualidade do que fazemos, e a atenção pode ser treinada
- A atenção é fixa e não muda
- Só o QI importa
- Distração é sempre prejudicial
> A atenção é como um músculo: bem usada, melhora.

? Quais são os três focos da tríade?
+ Interno, no outro e externo
- Curto, médio e longo
- Visual, auditivo e tátil
- Seletivo, aberto e restrito
> Cada um responde a uma pergunta diferente.

? O que acontece, segundo a tríade, a quem ignora o foco externo?
+ É pego de surpresa pelos sistemas maiores em que atua
- Fica desorientado em relação aos valores
- Fica desinformado sobre os sentimentos dos outros
- Nada
> Interno ausente = desorientado; no outro ausente = desinformado.

? Como a Parte 13 trata os números científicos citados?
+ Como parâmetros ilustrativos e hipóteses a testar no seu caso
- Como constantes universais
- Como leis jurídicas
- Ignora-os
> A ciência divulgada simplifica e é revista.

? Por que modelar a autoavaliação em um record com validação?
+ Para impedir notas inválidas e permitir testes automáticos
- Para tornar o código mais lento
- Para esconder os dados
- Porque records são obrigatórios
> Validação na criação evita estados impossíveis.

? Qual é a metáfora usada para a atenção?
+ Um músculo que se fortalece com uso inteligente e descanso
- Um interruptor
- Um arquivo
- Uma moeda fixa
> Como toda metáfora, tem limites.
:::

::: cartas
Foco triplo :: Interno, no outro e externo: três direções da atenção que devem ser equilibradas.
Foco interno :: Atenção às próprias emoções, intuições e valores.
Foco no outro :: Atenção ao que os outros sentem e pensam (empatia, contexto).
Foco externo :: Atenção aos sistemas maiores e aos efeitos de longo prazo.
Atenção como músculo :: Metáfora: treino e descanso fortalecem a atenção.
Divulgação científica :: Resumo de pesquisas para o grande público; simplifica e é revista.
Autoavaliação :: Notas dadas por você a si mesmo; compare com a visão de outros.
Parâmetro ilustrativo :: Valor de exemplo no código, ajustável ao seu contexto.
:::
