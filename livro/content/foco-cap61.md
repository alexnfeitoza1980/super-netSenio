---
id: foco-cap61
order: 83
kind: capitulo
num: 61
group: C# & Mentalidade · Parte 13 · Foco em C#
title: Mente que divaga, casulo criativo
subtitle: Divagar não é só distração: é quando a mente conecta ideias soltas. Uma agenda que protege o foco e reserva espaço de propósito para a criatividade, mais um caderno para capturar o que surge.
minutes: 50
hue: 230
objectives:
- Distinguir **divagação como defeito** de **divagação como fonte de ideias**
- Entender a **rede de modo padrão** e o **casulo criativo** como ideias, com suas ressalvas
- Modelar uma **agenda** com blocos de foco, reunião, casulo e pausa, e **auditá-la** em C#
- Montar um **caderno de ideias** que captura o que a mente solta sem quebrar o foco
- Saber quando **abrir** a atenção (exploração) e quando **estreitá-la** (execução)
origem: livros/04-foco-em-csharp/content/93-cap61.md
---

::: vila A ideia do chuveiro
Lia passou duas tardes tentando resolver um problema de cache e nada. No sábado, lavando a louça, a solução surgiu inteira. "Foi o cérebro divagando", ela diz. A Bia pergunta: "Então posso passar o dia olhando para o teto?". **Não.** O ponto é outro: a divagação **também** serve; ela só precisa ter **lugar certo** na agenda.
:::

::: ilustracao img/foco/cap61.svg | A ideia aparece fora do teclado, depois de trabalhar o problema: preparar, incubar, iluminar.
:::

## O que o livro diz

> Quando a mente **divaga**, entra em ação uma rede cerebral conhecida como **rede de modo padrão**, que costura memórias, imaginação e planos. É aí que muitas **ideias criativas** aparecem, depois de o problema ter sido trabalhado com foco.

O que vale guardar:

- Divagar custa **compreensão** quando você precisa estar presente (lendo, escutando), mas ajuda quando o objetivo é **criar**.
- A criatividade costuma seguir um ritmo: **preparação** (foco no problema), **incubação** (soltar, divagar) e **iluminação** (a ideia aparece).
- Pessoas muito criativas costumam ter um **"casulo"**: um período em que ficam **abertas** a ideias sem ter que entregar nada.
- A **atenção aberta** (percepção ampla, sem um alvo único) complementa a **atenção focada**: o equilíbrio é o ponto.
- A **serendipidade** (achar algo valioso sem procurar) favorece quem **não filtra tudo** o tempo todo.

::: cuidado Sem exagero
"Divagar gera criatividade" **não** significa que toda divagação seja produtiva, nem que sem divagar não haja criatividade. A pesquisa mostra que a **incubação** costuma ajudar **depois** de um esforço sério no problema; sem preparação, não há o que incubar. Trate a ideia como **hipótese para experimentar na sua agenda**, não como promessa.
:::

## Da vida para o código

| No livro | Em engenharia de software |
|---|---|
| Foco no problema (preparação) | **Bloco de foco** sem interrupção |
| Divagação / incubação | **Casulo**: caminhada, rascunho livre, bloco sem entrega |
| Iluminação | A solução que aparece **fora** do teclado |
| Atenção aberta | **Exploração**: ler código alheio, protótipo, *spike* |
| Serendipidade | **Caderno de ideias** que captura o que surgiu |

::: figura Preparação, incubação, iluminação
<svg viewBox="0 0 740 200" role="img" aria-label="Linha do tempo: foco no problema, casulo de incubação e ideia que surge">
  <path class="arrow" d="M20 120L720 120"/>
  <rect class="f-info" x="40" y="70" width="230" height="46" rx="10"/>
  <text x="155" y="92" text-anchor="middle" font-weight="800">Preparação</text>
  <text x="155" y="108" text-anchor="middle" font-size="11" class="t-dim">foco no problema</text>
  <rect class="f-warn" x="290" y="70" width="230" height="46" rx="10"/>
  <text x="405" y="92" text-anchor="middle" font-weight="800">Incubação (casulo)</text>
  <text x="405" y="108" text-anchor="middle" font-size="11" class="t-dim">caminhar, rabiscar, soltar</text>
  <circle class="f-good" cx="600" cy="93" r="26"/>
  <text x="600" y="98" text-anchor="middle" font-weight="800">💡</text>
  <text x="600" y="145" text-anchor="middle" font-size="12" font-weight="700">Iluminação</text>
  <text x="370" y="180" text-anchor="middle" font-size="12">Sem a primeira caixa, a segunda vira só distração.</text>
</svg>
:::

## 1) A agenda como código

Uma agenda é uma **lista de blocos**. Dá para escrever regras sobre ela, como em qualquer validação:

```cs Jornada/Foco/Cap61/AgendaDoDia.cs
namespace Jornada.Foco.Cap61;

public enum TipoDeBloco { Foco, Reuniao, Casulo, Pausa, Administrativo }

/// <param name="InicioEmMinutos">Minutos desde 00:00 (9h = 540).</param>
public sealed record Bloco(TipoDeBloco Tipo, int InicioEmMinutos, int DuracaoEmMinutos)
{
    public int FimEmMinutos => InicioEmMinutos + DuracaoEmMinutos;
}

public sealed class AgendaDoDia
{
    public const int FocoMaximoSemPausa = 120;
    public const int CasuloMinimoPorDia = 30;

    private readonly List<Bloco> blocos;

    public AgendaDoDia(IEnumerable<Bloco> blocos)
    {
        this.blocos = blocos.OrderBy(b => b.InicioEmMinutos).ToList();
        foreach (var b in this.blocos)
            if (b.DuracaoEmMinutos <= 0)
                throw new ArgumentException($"Bloco com duração inválida às {Hora(b.InicioEmMinutos)}.");
    }

    public int Minutos(TipoDeBloco tipo) =>
        blocos.Where(b => b.Tipo == tipo).Sum(b => b.DuracaoEmMinutos);

    public IReadOnlyList<string> Avisos()
    {
        var avisos = new List<string>();

        for (int i = 0; i < blocos.Count; i++)
        {
            var b = blocos[i];

            if (b.Tipo == TipoDeBloco.Foco && b.DuracaoEmMinutos > FocoMaximoSemPausa)
                avisos.Add($"{Hora(b.InicioEmMinutos)}: foco de {b.DuracaoEmMinutos} min sem pausa (máx. {FocoMaximoSemPausa}).");

            if (i > 0)
            {
                var anterior = blocos[i - 1];
                if (b.InicioEmMinutos < anterior.FimEmMinutos)
                    avisos.Add($"{Hora(b.InicioEmMinutos)}: sobrepõe o bloco anterior.");
                else if (b.Tipo == TipoDeBloco.Reuniao && anterior.Tipo == TipoDeBloco.Reuniao
                         && b.InicioEmMinutos == anterior.FimEmMinutos)
                    avisos.Add($"{Hora(b.InicioEmMinutos)}: reunião colada na anterior, sem respiro.");
            }
        }

        if (Minutos(TipoDeBloco.Casulo) < CasuloMinimoPorDia)
            avisos.Add($"Casulo de apenas {Minutos(TipoDeBloco.Casulo)} min (mínimo sugerido: {CasuloMinimoPorDia}).");

        return avisos;
    }

    public static string Hora(int minutos) => $"{minutos / 60:00}:{minutos % 60:00}";
}
```

```cs Uso
var agenda = new AgendaDoDia(new[]
{
    new Bloco(TipoDeBloco.Foco,           9 * 60,        150),
    new Bloco(TipoDeBloco.Reuniao,       11 * 60 + 30,    30),
    new Bloco(TipoDeBloco.Reuniao,       12 * 60,         30),
    new Bloco(TipoDeBloco.Administrativo,14 * 60,         60),
    new Bloco(TipoDeBloco.Foco,          15 * 60,         90),
});

Console.WriteLine($"Foco: {agenda.Minutos(TipoDeBloco.Foco)} min · Casulo: {agenda.Minutos(TipoDeBloco.Casulo)} min");
foreach (var aviso in agenda.Avisos())
    Console.WriteLine("⚠ " + aviso);
```

```saida
Foco: 240 min · Casulo: 0 min
⚠ 09:00: foco de 150 min sem pausa (máx. 120).
⚠ 12:00: reunião colada na anterior, sem respiro.
⚠ Casulo de apenas 0 min (mínimo sugerido: 30).
```

::: dica Limites são parâmetros
Os 120 min e os 30 min são **valores de partida** (constantes públicas, fáceis de mudar). A pesquisa sobre ritmos de atenção varia bastante entre pessoas e tarefas: observe **o seu** padrão e ajuste.
:::

## 2) O caderno de ideias

A mente que divaga produz **ideias fora de hora**. Se você corre para anotá-las no meio do foco, quebra o foco; se não anota, perde a ideia. A saída é um **caderno de captura**: anotar em 5 segundos e **revisar depois**.

```cs Jornada/Foco/Cap61/CadernoDeIdeias.cs
namespace Jornada.Foco.Cap61;

public sealed record Ideia(int Numero, string Texto, int MinutoDoDia, bool Revisada = false);

public sealed class CadernoDeIdeias
{
    private readonly List<Ideia> ideias = new();

    public int Pendentes => ideias.Count(i => !i.Revisada);

    /// <summary>Captura rápida: só guarda. Nada de avaliar agora.</summary>
    public Ideia Capturar(string texto, int minutoDoDia)
    {
        if (string.IsNullOrWhiteSpace(texto))
            throw new ArgumentException("Ideia vazia.", nameof(texto));

        var ideia = new Ideia(ideias.Count + 1, texto.Trim(), minutoDoDia);
        ideias.Add(ideia);
        return ideia;
    }

    /// <summary>Hora da revisão (no casulo): devolve as pendentes e marca como revisadas.</summary>
    public IReadOnlyList<Ideia> Revisar()
    {
        var pendentes = ideias.Where(i => !i.Revisada).ToList();
        for (int i = 0; i < ideias.Count; i++)
            ideias[i] = ideias[i] with { Revisada = true };
        return pendentes;
    }
}
```

```cs Uso
var caderno = new CadernoDeIdeias();
caderno.Capturar("  cache por tenant em vez de global ", 10 * 60 + 12);
caderno.Capturar("renomear 'Manager' para algo com sentido", 10 * 60 + 40);

Console.WriteLine($"Pendentes: {caderno.Pendentes}");
foreach (var i in caderno.Revisar())
    Console.WriteLine($"#{i.Numero} ({AgendaDoDia.Hora(i.MinutoDoDia)}): {i.Texto}");
Console.WriteLine($"Pendentes depois da revisão: {caderno.Pendentes}");
```

```saida
Pendentes: 2
#1 (10:12): cache por tenant em vez de global
#2 (10:40): renomear 'Manager' para algo com sentido
Pendentes depois da revisão: 0
```

```cs JornadaTests/Foco/Cap61/Cap61Tests.cs
using Jornada.Foco.Cap61;
using Xunit;

public class Cap61Tests
{
    [Fact]
    public void Agenda_sem_problemas_nao_gera_avisos()
    {
        var agenda = new AgendaDoDia(new[]
        {
            new Bloco(TipoDeBloco.Foco,   9 * 60, 90),
            new Bloco(TipoDeBloco.Casulo, 10 * 60 + 30, 30),
        });
        Assert.Empty(agenda.Avisos());
    }

    [Fact]
    public void Detecta_sobreposicao()
    {
        var agenda = new AgendaDoDia(new[]
        {
            new Bloco(TipoDeBloco.Foco,    9 * 60, 60),
            new Bloco(TipoDeBloco.Casulo,  9 * 60 + 30, 60),
        });
        Assert.True(agenda.Avisos().Any(a => a.Contains("sobrepõe")));
    }

    [Fact]
    public void Duracao_invalida_e_rejeitada() =>
        Assert.Throws<ArgumentException>(() => new AgendaDoDia(new[] { new Bloco(TipoDeBloco.Foco, 540, 0) }));

    [Fact]
    public void Caderno_captura_e_revisa()
    {
        var c = new CadernoDeIdeias();
        c.Capturar(" ideia ", 600);
        Assert.Equal(1, c.Pendentes);
        Assert.Single(c.Revisar());
        Assert.Equal(0, c.Pendentes);
        Assert.Empty(c.Revisar());
    }

    [Fact]
    public void Caderno_rejeita_ideia_vazia() =>
        Assert.Throws<ArgumentException>(() => new CadernoDeIdeias().Capturar("   ", 0));
}
```

## 3) Quando abrir, quando estreitar

Uma regra prática: **explore** no começo (entender o problema, ler, prototipar, divagar) e **execute** depois (focar, implementar, testar). Misturar os dois é a receita do desânimo: ou você "explora" para sempre sem entregar, ou "executa" cedo demais sem entender.

| Fase | Atenção | Sinal de que está funcionando |
|---|---|---|
| Entender o problema | **Aberta** | Perguntas melhores do que no início |
| Explorar soluções | **Aberta → focada** | Duas ou três opções comparadas |
| Implementar | **Focada** | Poucas interrupções, testes verdes |
| Revisar e refletir | **Aberta** | Ideias do caderno revisadas |

::: exercicio Ideias velhas
1. Acrescente a `CadernoDeIdeias` o método `IdeiasAntigas(int minutoAtual, int limiteEmMinutos)` que devolve as **não revisadas** capturadas há mais de `limiteEmMinutos`.
2. Escreva um teste que prove que uma ideia revisada **não** aparece.

--- resposta
```cs Jornada/Foco/Cap61/CadernoComAntigas.cs
namespace Jornada.Foco.Cap61;

public static class CadernoExtensoes
{
    // Como 'Revisar' marca tudo como revisado, aqui lemos a lista por um método de consulta.
    public static IReadOnlyList<Ideia> IdeiasAntigas(
        this IEnumerable<Ideia> ideias, int minutoAtual, int limiteEmMinutos) =>
        ideias.Where(i => !i.Revisada && minutoAtual - i.MinutoDoDia > limiteEmMinutos).ToList();
}
```
```cs Jornada/Foco/Cap61/CadernoAntigasTests.cs
using Jornada.Foco.Cap61;
using Xunit;

public class CadernoAntigasTests
{
    [Fact]
    public void So_as_nao_revisadas_e_antigas_aparecem()
    {
        var ideias = new[]
        {
            new Ideia(1, "velha",   600),
            new Ideia(2, "nova",    900),
            new Ideia(3, "revisada", 500, Revisada: true),
        };

        var antigas = ideias.IdeiasAntigas(minutoAtual: 960, limiteEmMinutos: 120);

        Assert.Equal(new[] { 1 }, antigas.Select(i => i.Numero));
    }
}
```
Solução com **método de extensão** sobre a lista de `Ideia`, para não precisar expor o estado interno do caderno. Se você preferir, adicione `IReadOnlyList<Ideia> Todas` ao caderno e chame a extensão com ele.
:::

::: resumo
- **Divagar** atrapalha quando você precisa estar presente, mas a **incubação** ajuda a criar — **depois** de trabalhar o problema com foco.
- Criatividade em três tempos: **preparação**, **incubação** (casulo), **iluminação**.
- Uma **agenda** pode ser auditada em código: foco longo demais, reuniões coladas, **falta de casulo**; os limites são parâmetros seus.
- Um **caderno de ideias** captura o que a mente solta sem quebrar o foco; revise no casulo.
- **Explore** (atenção aberta) antes de **executar** (atenção focada); e trate tudo isso como **hipótese a testar** na sua rotina.
:::

::: quiz
? Quando a divagação é mais útil?
+ Na incubação, depois de ter trabalhado o problema com foco
- Durante uma leitura que você precisa entender
- Em qualquer momento, sem preparação
- Só à noite
> Sem preparação, não há o que incubar.

? O que é o "casulo criativo"?
+ Período de abertura a ideias, sem a pressão de entregar algo
- Uma reunião de equipe
- Uma ferramenta de edição
- Um tipo de erro
> Um espaço de propósito na agenda.

? Por que usar um caderno de ideias?
+ Capturar a ideia sem quebrar o foco e avaliá-la depois
- Substituir o controle de versão
- Gerar mais ideias automaticamente
- Evitar reuniões
> Anotar rápido; revisar no casulo.

? Qual o risco de usar o limite de 120 minutos como regra fixa?
+ Ele é um ponto de partida; o ritmo varia entre pessoas e tarefas
- Nenhum, é lei científica
- Ele impede testes
- Ele só vale para júnior
> Observe e ajuste ao seu caso.

? O que significa "explorar antes de executar"?
+ Abrir a atenção para entender e comparar opções antes de estreitar o foco para implementar
- Programar sem planejar
- Planejar para sempre
- Pular os testes
> Fases diferentes pedem atenção diferente.

? O que NÃO se pode concluir da pesquisa sobre divagação?
+ Que toda divagação seja produtiva
- Que a incubação pode ajudar a criar
- Que o foco importa
- Que ideias podem surgir fora do teclado
> Exagero é o risco.
:::

::: cartas
Divagação :: Quando a mente se afasta do alvo e passa a combinar memórias e imaginação.
Rede de modo padrão :: Rede cerebral ativa quando a mente divaga.
Incubação :: Fase de soltar o problema, depois de ter trabalhado nele.
Casulo criativo :: Período protegido para abertura de ideias sem entrega.
Atenção aberta :: Percepção ampla, sem um alvo único.
Serendipidade :: Encontrar algo valioso sem procurar.
Caderno de ideias :: Local de captura rápida para revisar depois.
Explorar × executar :: Abrir a atenção para entender; estreitá-la para entregar.
:::
