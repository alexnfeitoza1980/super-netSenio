---
id: foco-cap63
order: 85
kind: capitulo
num: 63
group: C# & Mentalidade · Parte 13 · Foco em C#
title: Autoconsciência: pontos cegos e palpites calibrados
subtitle: Foco interno começa em saber o que você sente e o que realmente sabe. Avaliação 360 para achar pontos cegos, e um diário de palpites para calibrar a intuição com dados.
minutes: 55
hue: 260
objectives:
- Entender a **autoconsciência** como o primeiro pilar do foco interno
- Distinguir **ponto cego**, **força oculta** e **autoavaliação alinhada** numa avaliação 360
- Reconhecer o efeito **"melhor que a média"** e por que feedback externo corrige a visão
- Registrar **palpites com confiança** e medir a **calibração** da própria intuição
- Tratar a intuição como **hipótese a verificar**, não como oráculo
origem: livros/04-foco-em-csharp/content/95-cap63.md
---

::: vila O PR "perfeito"
Caio tem certeza de que se comunica muito bem e de que revisa código de forma apenas "razoável". No *feedback* anônimo da equipe, o resultado vem **ao contrário**: as pessoas acham a comunicação dele confusa e a revisão, excelente. Ele fica incomodado e depois curioso: "Será que eu me conheço menos do que penso?".
:::

::: ilustracao img/foco/cap63.svg | O espelho mostra como me vejo; a avaliação 360 mostra como me veem. A diferença é o ponto cego.
:::

## O que o livro diz

> **Autoconsciência** é a capacidade de monitorar o que se passa dentro de você: pensamentos, emoções, sinais do corpo. É a base para decidir bem, regular-se e entender os outros.

O que vale levar:

- O **corpo** dá pistas: a parte do cérebro chamada **ínsula** acompanha sinais internos, e as "**sensações viscerais**" (as *gut feelings*) podem guiar decisões — especialmente em áreas onde você tem **muita experiência**.
- A intuição é **conhecimento acumulado**, não adivinhação: funciona melhor onde há **repetição e retorno** (feedback) sobre acertos e erros.
- A maioria de nós se avalia **acima da média** em quase tudo (o chamado efeito "**melhor que a média**"); por isso o **feedback externo**, como a avaliação **360 graus**, corrige pontos cegos.
- **Falar sobre o que você sente** (nomear emoções) tende a reduzir sua intensidade e a facilitar a escolha seguinte.
- Em grupos, o **pensamento de manada** abafa dissidências; quem tem autoconsciência social ajuda a trazer opiniões diferentes à tona.

::: cuidado Intuição não é infalível
O livro valoriza as sensações viscerais, mas isso **não** é um convite a decidir tudo "pelo instinto". A intuição é boa onde há **experiência com retorno rápido** (xadrez, depuração de um sistema que você conhece) e ruim onde **não há** (prever o mercado, estimar projetos inéditos). A solução, em C# e na vida, é **medir**: um diário que mostre quando seu palpite acerta.
:::

## Da vida para o código

| No livro | Em engenharia de software |
|---|---|
| Ponto cego | **Hábito que você não vê**: o time vê, você não |
| Feedback 360 | **Retrospectivas**, avaliação por pares, revisão de código |
| "Melhor que a média" | **Superestimar** a qualidade do próprio código |
| Sensação visceral | O **"cheiro de código"** (*code smell*) que algo está errado |
| Nomear a emoção | **Nomear o erro** com clareza ao relatar um incidente |
| Pensamento de manada | **Concordar com o líder técnico** sem avaliar |

::: figura A janela entre o que eu vejo e o que os outros veem
<svg viewBox="0 0 740 240" role="img" aria-label="Quatro quadrantes: aberto, cego, oculto e desconhecido, de acordo com o que eu vejo e o que os outros veem">
  <g font-size="12" text-anchor="middle">
    <text x="245" y="22">Eu vejo</text><text x="495" y="22">Eu não vejo</text>
    <text x="40" y="82" transform="rotate(-90 40 82)">Outros veem</text>
    <text x="40" y="190" transform="rotate(-90 40 190)">Outros não veem</text>
  </g>
  <rect class="f-good" x="120" y="30" width="250" height="90" rx="10"/>
  <text x="245" y="70" text-anchor="middle" font-weight="800">Aberto</text>
  <text x="245" y="92" text-anchor="middle" font-size="11" class="t-dim">alinhado</text>
  <rect class="f-bad" x="380" y="30" width="250" height="90" rx="10"/>
  <text x="505" y="70" text-anchor="middle" font-weight="800">Ponto cego</text>
  <text x="505" y="92" text-anchor="middle" font-size="11" class="t-dim">360 revela</text>
  <rect class="f-info" x="120" y="130" width="250" height="90" rx="10"/>
  <text x="245" y="170" text-anchor="middle" font-weight="800">Fachada</text>
  <text x="245" y="192" text-anchor="middle" font-size="11" class="t-dim">só eu sei</text>
  <rect class="f-plain" x="380" y="130" width="250" height="90" rx="10"/>
  <text x="505" y="170" text-anchor="middle" font-weight="800">Desconhecido</text>
  <text x="505" y="192" text-anchor="middle" font-size="11" class="t-dim">a explorar</text>
</svg>
:::

## 1) A avaliação 360, em C#

Cada competência tem uma **nota própria** e as notas de **outras pessoas**. A diferença mostra **pontos cegos** (você se acha melhor do que os outros acham) e **forças ocultas** (o contrário):

```cs Jornada/Foco/Cap63/Avaliacao360.cs
namespace Jornada.Foco.Cap63;

public enum Leitura { Alinhado, PontoCego, ForcaOculta }

public sealed record Avaliacao360
{
    public const int MinimoDeAvaliadores = 3; // preserva o anonimato

    public string Competencia { get; }
    public int Autoavaliacao { get; }
    public IReadOnlyList<int> Outros { get; }

    public Avaliacao360(string competencia, int autoavaliacao, IEnumerable<int> outros)
    {
        var lista = outros.ToList();
        if (lista.Count < MinimoDeAvaliadores)
            throw new ArgumentException($"São necessários ao menos {MinimoDeAvaliadores} avaliadores.", nameof(outros));
        if (!Valida(autoavaliacao) || lista.Any(n => !Valida(n)))
            throw new ArgumentOutOfRangeException(nameof(outros), "As notas vão de 1 a 5.");

        Competencia = competencia;
        Autoavaliacao = autoavaliacao;
        Outros = lista;
    }

    private static bool Valida(int nota) => nota is >= 1 and <= 5;

    public double MediaDosOutros => Outros.Average();

    /// <summary>Positivo: eu me vejo melhor do que os outros me veem.</summary>
    public double Diferenca => Autoavaliacao - MediaDosOutros;
}
```

A **leitura** fica em uma classe à parte, para que o limite (1,5 ponto) seja um parâmetro fácil de mudar:

```cs Jornada/Foco/Cap63/Avaliacao360Leitura.cs
namespace Jornada.Foco.Cap63;

public static class LeituraDoFeedback
{
    public static Leitura Classificar(Avaliacao360 a, double limite = 1.5) =>
        a.Diferenca >= limite  ? Leitura.PontoCego :
        a.Diferenca <= -limite ? Leitura.ForcaOculta :
                                 Leitura.Alinhado;
}
```

```cs Uso
var avaliacoes = new[]
{
    new Avaliacao360("Comunicação",       5, new[] { 3, 3, 4 }),
    new Avaliacao360("Revisão de código", 3, new[] { 5, 5, 4 }),
    new Avaliacao360("Testes",            4, new[] { 4, 4, 5 }),
};

foreach (var a in avaliacoes)
    Console.WriteLine(
        $"{a.Competencia,-18} eu {a.Autoavaliacao} · outros {a.MediaDosOutros:F1} · " +
        $"{LeituraDoFeedback.Classificar(a)}");
```

```saida
Comunicação        eu 5 · outros 3,3 · PontoCego
Revisão de código  eu 3 · outros 4,7 · ForcaOculta
Testes             eu 4 · outros 4,3 · Alinhado
```

::: dica Como receber feedback sem se defender
Um ponto cego não é uma **sentença**: é uma **pista**. O primeiro movimento útil é perguntar, não rebater: *"Pode me dar um exemplo concreto?"* Evite o "mas eu quis dizer": o efeito da sua comunicação é o dado, a intenção é secundária.
:::

## 2) O diário de palpites

Para calibrar a intuição, você precisa de **dados**. A cada decisão baseada em "feeling", anote o **palpite** e a **confiança** (50 a 100%). Depois, registre se acertou. O resultado mostra se você é **calibrado**, **confiante demais** ou **cauteloso demais**:

```cs Jornada/Foco/Cap63/DiarioDePalpites.cs
namespace Jornada.Foco.Cap63;

public sealed record Palpite(string Descricao, int Confianca, bool? Acertou = null);

public sealed record FaixaDeConfianca(string Nome, int Quantidade, double TaxaDeAcerto);

public sealed class DiarioDePalpites
{
    private readonly List<Palpite> palpites = new();

    public int Total => palpites.Count;

    public int Registrar(string descricao, int confianca)
    {
        if (confianca is < 50 or > 100)
            throw new ArgumentOutOfRangeException(nameof(confianca), "Use de 50 a 100.");
        palpites.Add(new Palpite(descricao, confianca));
        return palpites.Count - 1;
    }

    public void Resolver(int indice, bool acertou) =>
        palpites[indice] = palpites[indice] with { Acertou = acertou };

    private IEnumerable<Palpite> Resolvidos => palpites.Where(p => p.Acertou is not null);

    public double ConfiancaMedia => Resolvidos.Any() ? Resolvidos.Average(p => p.Confianca) : 0;

    public double TaxaDeAcerto =>
        Resolvidos.Any() ? 100.0 * Resolvidos.Count(p => p.Acertou == true) / Resolvidos.Count() : 0;

    /// <summary>Positivo: confiante demais. Negativo: cauteloso demais.</summary>
    public double ExcessoDeConfianca => ConfiancaMedia - TaxaDeAcerto;

    public IReadOnlyList<FaixaDeConfianca> PorFaixa() =>
        new[] { ("50–69", 50, 69), ("70–89", 70, 89), ("90–100", 90, 100) }
            .Select(f =>
            {
                var grupo = Resolvidos.Where(p => p.Confianca >= f.Item2 && p.Confianca <= f.Item3).ToList();
                double taxa = grupo.Count == 0 ? 0 : 100.0 * grupo.Count(p => p.Acertou == true) / grupo.Count;
                return new FaixaDeConfianca(f.Item1, grupo.Count, taxa);
            })
            .ToList();
}
```

```cs Uso
var diario = new DiarioDePalpites();
var casos = new (string, int, bool)[]
{
    ("O bug está no cache",                 90, false),
    ("A migração leva menos de um dia",     80, false),
    ("O teste instável é problema de tempo", 70, true),
    ("O cliente aceita a nova data",        60, true),
    ("O PR passa na primeira revisão",      90, true),
    ("O deploy sai sem incidente",          95, true),
};

foreach (var (texto, confianca, acertou) in casos)
    diario.Resolver(diario.Registrar(texto, confianca), acertou);

foreach (var f in diario.PorFaixa())
    Console.WriteLine($"{f.Nome,-7} {f.Quantidade} palpite(s) · acerto {f.TaxaDeAcerto:F0}%");

Console.WriteLine($"Confiança média {diario.ConfiancaMedia:F0}% · acerto {diario.TaxaDeAcerto:F0}% · " +
                  $"excesso {diario.ExcessoDeConfianca:+0;-0}");
```

```saida
50–69   1 palpite(s) · acerto 100%
70–89   2 palpite(s) · acerto 50%
90–100  3 palpite(s) · acerto 67%
Confiança média 81% · acerto 67% · excesso +14
```

Com **seis** palpites, a conclusão é frágil (a amostra é minúscula!), mas o diário já mostra uma **tendência a conferir**: nas faixas de maior confiança, o acerto não foi tão alto quanto a confiança prometia. O hábito, repetido por meses, é o que dá dados confiáveis.

```cs JornadaTests/Foco/Cap63/Cap63Tests.cs
using Jornada.Foco.Cap63;
using Xunit;

public class Cap63Tests
{
    [Fact]
    public void Ponto_cego_quando_me_vejo_muito_melhor()
    {
        var a = new Avaliacao360("X", 5, new[] { 3, 3, 3 });
        Assert.Equal(Leitura.PontoCego, LeituraDoFeedback.Classificar(a));
    }

    [Fact]
    public void Forca_oculta_quando_os_outros_veem_melhor()
    {
        var a = new Avaliacao360("X", 2, new[] { 4, 4, 5 });
        Assert.Equal(Leitura.ForcaOculta, LeituraDoFeedback.Classificar(a));
    }

    [Fact]
    public void Poucos_avaliadores_sao_rejeitados() =>
        Assert.Throws<ArgumentException>(() => new Avaliacao360("X", 3, new[] { 3, 4 }));

    [Fact]
    public void Nota_fora_da_escala_e_rejeitada() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new Avaliacao360("X", 6, new[] { 3, 4, 5 }));

    [Fact]
    public void Diario_calcula_excesso_de_confianca()
    {
        var d = new DiarioDePalpites();
        d.Resolver(d.Registrar("a", 90), false);
        d.Resolver(d.Registrar("b", 90), true);
        Assert.Equal(40, d.ExcessoDeConfianca, 5);
    }

    [Fact]
    public void Confianca_fora_da_faixa_e_rejeitada() =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new DiarioDePalpites().Registrar("a", 30));
}
```

::: exercicio Mínimo de palpites
Conclusões com **poucos dados** enganam. Crie o método de extensão `AmostraSuficiente(this IEnumerable<Palpite> palpites, int minimo = 20)`, que só conta os palpites **já resolvidos**, e escreva testes mostrando `false` com 5 palpites resolvidos e `true` com 20 (e `false` se 20 palpites existirem, mas nenhum estiver resolvido).

--- resposta
```cs Jornada/Foco/Cap63/PalpiteExtensoes.cs
namespace Jornada.Foco.Cap63;

public static class PalpiteExtensoes
{
    public static bool AmostraSuficiente(this IEnumerable<Palpite> palpites, int minimo = 20) =>
        palpites.Count(p => p.Acertou is not null) >= minimo;
}
```
```cs Jornada/Foco/Cap63/AmostraTests.cs
using Jornada.Foco.Cap63;
using Xunit;

public class AmostraTests
{
    private static List<Palpite> Gerar(int n, bool resolvidos = true) =>
        Enumerable.Range(0, n)
            .Select(i => new Palpite($"p{i}", 70, resolvidos ? i % 2 == 0 : null))
            .ToList();

    [Fact]
    public void Cinco_palpites_nao_bastam() => Assert.False(Gerar(5).AmostraSuficiente());

    [Fact]
    public void Vinte_palpites_resolvidos_bastam() => Assert.True(Gerar(20).AmostraSuficiente());

    [Fact]
    public void Palpites_sem_resolucao_nao_contam() => Assert.False(Gerar(20, resolvidos: false).AmostraSuficiente());
}
```
Contar apenas os **resolvidos** é o ponto: um palpite sem desfecho ainda não ensina nada sobre calibração.
:::

::: resumo
- **Autoconsciência** é monitorar pensamentos, emoções e sinais do corpo; é o ponto de partida do foco interno.
- A **avaliação 360** revela **pontos cegos** e **forças ocultas**; receba o dado com **curiosidade**, pedindo exemplos.
- Tendemos a nos ver **acima da média**; feedback externo corrige.
- A **intuição** vale onde há experiência com retorno; um **diário de palpites** mede sua **calibração** (e, com poucos dados, conclua com cautela).
- Sensações viscerais são **hipóteses a verificar**, não oráculos.
:::

::: quiz
? O que a avaliação 360 ajuda a descobrir?
+ Pontos cegos e forças ocultas, comparando minha visão com a dos outros
- Quem é o melhor da equipe
- Qual linguagem usar
- O salário justo
> Dá feedback externo sobre o que não vejo.

? Por que exigir um mínimo de avaliadores?
+ Para preservar o anonimato e reduzir distorções
- Porque o C# exige
- Para aumentar a nota
- Para evitar o cálculo da média
> Com poucos avaliadores, identifica-se quem disse o quê.

? Em que condições a intuição tende a ser confiável?
+ Em áreas com muita experiência e retorno frequente sobre acertos e erros
- Sempre
- Nunca
- Apenas em decisões de dinheiro
> Intuição é conhecimento acumulado.

? O que significa "excesso de confiança" no diário?
+ Confiança média maior que a taxa real de acertos
- Falta de palpites
- Notas muito altas na 360
- Cansaço
> Positivo: confiante demais.

? Por que o diário com 6 palpites deve ser interpretado com cautela?
+ A amostra é pequena e pode refletir acaso
- Porque o código está errado
- Porque palpites não valem
- Porque o C# só aceita 20 itens
> Dados confiáveis exigem repetição.

? O que é o efeito "melhor que a média"?
+ A tendência de se avaliar acima da média em quase tudo
- Um padrão de projeto
- Um erro de compilação
- Um tipo de reunião
> Por isso o feedback externo importa.
:::

::: cartas
Autoconsciência :: Capacidade de monitorar o que se passa dentro de você.
Ínsula :: Região cerebral que acompanha sinais internos do corpo.
Sensação visceral :: Pista corporal (gut feeling) ligada a experiência acumulada.
Avaliação 360 :: Feedback de várias pessoas sobre as mesmas competências.
Ponto cego :: O que os outros veem em mim e eu não.
Força oculta :: Qualidade que os outros veem e eu subestimo.
Calibração :: Concordância entre confiança declarada e acerto real.
Excesso de confiança :: Confiança média acima da taxa real de acertos.
:::
