---
id: amor-cap107
order: 139
kind: capitulo
num: 107
group: C# & Mentalidade · Parte 17 · Você é aquilo que ama, em C#
title: Amar é um hábito
subtitle: Virtude como segunda natureza, formada por imitação e prática. Em C#, é o capítulo da imutabilidade: um hábito que nunca muda por dentro, só gera outro, mais forte ou mais fraco.
minutes: 65
hue: 325
objectives:
- Entender **virtude** como hábito: uma inclinação que se grava com **prática** e **imitação** até virar "segunda natureza"
- Ver por que, para o autor, **amar é um hábito**, e por que isso muda o que significa "mudar"
- Modelar um hábito **imutável**, usando `record` e a expressão **`with`**
- Usar **parâmetros opcionais**, constantes e `Aggregate` para simular a formação (e o desgaste) de um hábito
origem: livros/08-voce-e-aquilo-que-ama-csharp/content/127-cap107.md
---

::: vila O teclado que sabia sozinho
Lia digitava sem olhar o teclado. Um dia, Beto pediu: "diz onde fica a tecla do ponto e vírgula". Lia travou. Teve que **mexer as mãos no ar** para descobrir. Anos de prática tinham levado o conhecimento para os dedos, a um lugar onde a cabeça já não precisava ir. Beto riu: "eu sei onde fica, e não consigo digitar sem olhar". Lia respondeu: "é, saber e **ter na mão** são coisas diferentes".
:::

::: ilustracao img/amor/cap107.svg | Praticar é fazer a mesma coisa várias vezes até a mão saber sozinha. Parar de praticar faz o hábito se desfazer devagar.
:::

## O que o livro diz

Smith faz aqui uma ponte com uma tradição antiga de filosofia moral, de Aristóteles a Tomás de Aquino. **Virtudes**, diz ele, são simplesmente **bons hábitos morais**: disposições que se **incorporam** ao caráter, de modo que a pessoa passa a ser alguém **inclinada** ao bem. (Os hábitos morais ruins são os **vícios**.) A diferença para uma **regra** é esta: a regra é **de fora**; a virtude é **de dentro**. Quem a tem já **não precisa pensar** a cada vez, porque ela virou o que Aristóteles chamou de **segunda natureza**.

Duas coisas, nessa tradição, formam um hábito:

1. **Imitação.** Aprendemos a ser de certo jeito **vendo alguém** que já é. É por isso que mestres, exemplos e histórias de gente admirável pesam tanto.
2. **Prática.** E nenhum hábito se forma sem **repetição**: como a pessoa que toca escalas no piano até os dedos saberem de cor.

A partir disso, o autor chega à conclusão que dá o título ao capítulo do livro: **se você é aquilo que ama, e o amor é um hábito, então o seu amor é formado por imitação e prática**. Ou seja, o que desejamos não é só uma questão de convicção: é uma questão **de treino**.

::: cuidado Hábito não é destino
A tradição que o autor invoca insiste que **hábitos podem ser formados e re-formados**: é justamente a boa notícia. Também não é culpar ninguém por "ter hábitos ruins": **é preciso tempo, exemplos e companhia** para mudar um, e isso depende de condições que nem sempre dependem da pessoa.
:::

::: analogia As escalas do pianista
Ninguém toca uma sonata difícil lendo um livro sobre a sonata. O pianista passa **meses** nas mesmas escalas, aparentemente "sem sentido". Mas é ali que os dedos aprendem. Um dia, a sonata sai **quase sozinha**. O talento é de quem treina todo dia, e não de quem entende mais.
:::

## Da vida para o código

| No livro | Em C# |
|---|---|
| Hábito como disposição gravada | `record Habito(Nome, Forca, UltimaPratica)` |
| Segunda natureza (anda sozinho) | `Automatico` (força ≥ 0,8) |
| Prática: cada repetição grava | `Praticar` devolve um hábito **novo**, mais forte |
| Cada repetição soma menos que a anterior | `atual + (1 − atual) × ganho` |
| Imitação (mentor) | Parâmetro opcional `comMentor` (ganho maior) |
| Parar de praticar desfaz aos poucos | `ForcaEm` com **meia-vida** (cai pela metade a cada 14 dias) |
| Histórico de práticas | `PraticarVarias` com `Aggregate` |

::: figura A curva de um hábito: sobe devagar, desfaz devagar
<svg viewBox="0 0 740 240" role="img" aria-label="Curva de força de um hábito subindo com a prática diária até passar do limiar de automático, e depois caindo pela metade a cada quatorze dias sem prática">
  <line x1="50" y1="200" x2="700" y2="200" stroke="var(--ink-3)" stroke-width="1.5"/><line x1="50" y1="30" x2="50" y2="200" stroke="var(--ink-3)" stroke-width="1.5"/>
  <line x1="50" y1="76" x2="700" y2="76" stroke="var(--good)" stroke-width="2" stroke-dasharray="6 6"/><text x="696" y="68" text-anchor="end" font-size="12" class="t-dim">limiar: automático (0,8)</text>
  <path d="M50 200 C150 140, 210 90, 330 70" fill="none" stroke="var(--primary)" stroke-width="4"/>
  <path d="M330 70 C400 100, 470 150, 560 168" fill="none" stroke="#d8454f" stroke-width="4" stroke-dasharray="8 6"/>
  <text x="190" y="224" text-anchor="middle" font-size="12" class="t-dim">13 dias praticando todo dia</text>
  <text x="450" y="224" text-anchor="middle" font-size="12" class="t-dim">sem praticar: metade a cada 14 dias</text>
  <text x="600" y="120" font-size="12" class="t-dim">cada repetição soma</text><text x="600" y="136" font-size="12" class="t-dim">menos que a anterior</text>
</svg>
:::

## 1) Um hábito que nunca muda por dentro

Pense em como representar um hábito: um nome, uma **força** (de 0, "nada", a 1, "totalmente automático") e a data da última prática. Poderíamos usar uma classe com um método `Praticar()` que altera a força. Mas há uma alternativa mais clara: o **hábito imutável**. Cada prática **devolve um hábito novo**. O antigo permanece igual.

Isso se faz com o **`record`** e a expressão **`with`**:

```cs Uma amostra
var antes = new Habito("Escrever o teste primeiro");
var depois = antes with { Forca = 0.12 };   // cria uma CÓPIA, mudando só a Forca
// 'antes' continua com Forca = 0
```

Por que vale a pena? Primeiro, o código fica **fácil de testar** (o mesmo hábito, a mesma prática, o mesmo resultado, sempre). Segundo, dá para guardar o **histórico** sem medo de que alguém mude um valor antigo. Terceiro, elimina uma família inteira de bugs (os "quem mexeu nisso?").

Veja o modelo completo:

```cs Jornada/Habitos/Cap107/Habito.cs
namespace Jornada.Habitos.Cap107;

/// <summary>
/// Um hábito em formação. Imutável: cada prática devolve um hábito novo (com "with"),
/// o que deixa o histórico fácil de testar e de raciocinar.
/// </summary>
public sealed record Habito(string Nome, double Forca = 0, DateOnly? UltimaPratica = null)
{
    /// <summary>A partir daqui o hábito "anda sozinho": pede pouca decisão consciente.</summary>
    public const double Limiar = 0.8;

    public bool Automatico => Forca >= Limiar;
}

public static class Formacao
{
    public const double GanhoSozinho = 0.12;
    public const double GanhoComMentor = 0.18;
    public const double MeiaVidaEmDias = 14;

    /// <summary>A força que o hábito tem num dia: sem prática, ela vai se desfazendo pela metade a cada 14 dias.</summary>
    public static double ForcaEm(Habito habito, DateOnly dia)
    {
        if (habito.UltimaPratica is not { } ultima || dia <= ultima)
            return habito.Forca;

        int dias = dia.DayNumber - ultima.DayNumber;
        return habito.Forca * Math.Pow(0.5, dias / MeiaVidaEmDias);
    }

    /// <summary>
    /// Cada repetição soma menos que a anterior: o que falta para chegar a 1 vai diminuindo.
    /// O desgaste só corre nos dias SEM prática (por isso olhamos a força "de ontem").
    /// </summary>
    public static Habito Praticar(Habito habito, DateOnly dia, bool comMentor = false)
    {
        double atual = ForcaEm(habito, dia.AddDays(-1));
        double ganho = comMentor ? GanhoComMentor : GanhoSozinho;

        return habito with { Forca = atual + (1 - atual) * ganho, UltimaPratica = dia };
    }

    /// <summary>Aplica uma sequência de práticas, em ordem de data, e devolve o hábito resultante.</summary>
    public static Habito PraticarVarias(Habito inicial, IEnumerable<DateOnly> dias) =>
        dias.OrderBy(d => d).Aggregate(inicial, (h, dia) => Praticar(h, dia));

    /// <summary>Quantos dias de prática diária seriam necessários para o hábito ficar automático? (null se passar do limite)</summary>
    public static int? DiasAteAutomatico(Habito inicial, DateOnly comeco, bool comMentor = false, int limiteDeDias = 365)
    {
        var habito = inicial;

        for (int i = 0; i < limiteDeDias; i++)
        {
            habito = Praticar(habito, comeco.AddDays(i), comMentor);
            if (habito.Automatico)
                return i + 1;
        }

        return null;
    }
}
```

Detalhes que merecem olhar:

- **Duas fórmulas**, ambas inspiradas por ideias simples. A **formação**: `atual + (1 − atual) × ganho`: cada prática fecha uma **fração do que falta**. Por isso a primeira soma 0,12, a segunda um pouco menos, e assim por diante: **rendimentos decrescentes**. O **desgaste**: `força × 0,5^(dias / 14)`: a cada 14 dias sem praticar, a força **cai pela metade**.
- **`ForcaEm(habito, dia.AddDays(-1))`** dentro do `Praticar`: o desgaste só conta nos dias **sem** prática. Praticar todo dia não desgasta nada. (Um erro de raciocínio aqui fez a primeira versão do modelo nunca chegar ao automático!)
- **`comMentor = false`** é um **parâmetro opcional**: se quem chama não informa, vale `false`. Com mentor (a "imitação" do autor), o ganho por prática é maior (0,18, em vez de 0,12).
- **`Aggregate(inicial, (h, dia) => Praticar(h, dia))`** em `PraticarVarias`: começa do hábito inicial e **aplica uma prática por dia**, em ordem de data. O resultado de cada passo é a entrada do próximo.
- **`int?`** em `DiasAteAutomatico`: se o hábito não se forma dentro do limite de dias, devolve `null`.

```cs Uso
var habito = new Habito("Escrever o teste primeiro");
var inicio = new DateOnly(2026, 11, 2);

for (int i = 0; i < 5; i++)
{
    habito = Formacao.Praticar(habito, inicio.AddDays(i));
    Console.WriteLine($"dia {i + 1}: força {habito.Forca:0.00} · automático? {habito.Automatico}");
}

Console.WriteLine($"Sozinho: {Formacao.DiasAteAutomatico(new Habito("x"), inicio)} dias de prática diária");
Console.WriteLine($"Com mentor: {Formacao.DiasAteAutomatico(new Habito("x"), inicio, comMentor: true)} dias");

var duasSemanasDepois = inicio.AddDays(4 + 14);
Console.WriteLine($"Sem praticar por 14 dias, a força cai de {habito.Forca:0.00} para {Formacao.ForcaEm(habito, duasSemanasDepois):0.00}");
```

```saida
dia 1: força 0,12 · automático? False
dia 2: força 0,23 · automático? False
dia 3: força 0,32 · automático? False
dia 4: força 0,40 · automático? False
dia 5: força 0,47 · automático? False
Sozinho: 13 dias de prática diária
Com mentor: 9 dias
Sem praticar por 14 dias, a força cai de 0,47 para 0,24
```

Leia os números: nos cinco primeiros dias, a força foi 0,12, 0,23, 0,32, 0,40, 0,47: **cada dia soma menos**. Seriam **13 dias** de prática diária para passar de 0,8 sozinho, ou **9 com mentor**. E duas semanas de pausa derrubam a força de 0,47 para 0,24, **sem apagar** o que foi construído (começar de 0,24 é muito mais fácil que de 0).

::: dica Por que "meia-vida"?
O termo vem da física: o tempo em que uma quantidade cai pela metade. Aqui, é só uma forma simples de dizer "esquecer é gradual e acelerado no começo". Os números (0,12 de ganho, 14 dias) são **ilustrativos**: a ciência real dos hábitos tem muita variação entre pessoas e tipos de hábito. O modelo existe para dar **forma** a uma ideia, e não para prever alguém.
:::

## 2) Testes

```cs Habitos.Testes/Cap107Tests.cs
using Jornada.Habitos.Cap107;

namespace Habitos.Testes;

public class Cap107Tests
{
    private static DateOnly D(int dia) => new DateOnly(2026, 11, 1).AddDays(dia - 1);

    [Fact]
    public void Praticar_devolve_um_habito_novo_e_o_original_continua_igual()
    {
        var antes = new Habito("Escrever teste primeiro");
        var depois = Formacao.Praticar(antes, D(1));

        Assert.Equal(0, antes.Forca);
        Assert.Null(antes.UltimaPratica);
        Assert.Equal(0.12, depois.Forca, 6);
        Assert.Equal(D(1), depois.UltimaPratica);
    }

    [Fact]
    public void Cada_repeticao_soma_menos_que_a_anterior()
    {
        var h0 = new Habito("x");
        var h1 = Formacao.Praticar(h0, D(1));
        var h2 = Formacao.Praticar(h1, D(2));
        var h3 = Formacao.Praticar(h2, D(3));

        Assert.True(h2.Forca - h1.Forca < h1.Forca - h0.Forca);
        Assert.True(h3.Forca - h2.Forca < h2.Forca - h1.Forca);
    }

    [Fact]
    public void Sem_praticar_a_forca_cai_pela_metade_a_cada_catorze_dias()
    {
        var h = new Habito("x", 0.8, D(1));
        Assert.Equal(0.4, Formacao.ForcaEm(h, D(15)), 6);
        Assert.Equal(0.2, Formacao.ForcaEm(h, D(29)), 6);
        Assert.Equal(0.8, Formacao.ForcaEm(h, D(1)), 6);
    }

    [Fact]
    public void Com_mentor_o_habito_se_forma_mais_depressa()
    {
        var h = new Habito("revisar o próprio código");

        var sozinho = Formacao.DiasAteAutomatico(h, D(1));
        var acompanhado = Formacao.DiasAteAutomatico(h, D(1), comMentor: true);

        Assert.NotNull(sozinho);
        Assert.NotNull(acompanhado);
        Assert.True(acompanhado < sozinho);
    }

    [Fact]
    public void Praticar_todo_dia_chega_ao_automatico_e_parar_desfaz_aos_poucos()
    {
        var h = Formacao.PraticarVarias(new Habito("x"), Enumerable.Range(1, 14).Select(D));
        Assert.True(h.Automatico);

        Assert.True(Formacao.ForcaEm(h, D(14 + 14)) < Habito.Limiar);
    }

    [Fact]
    public void Praticar_de_vez_em_quando_nao_chega_ao_automatico()
    {
        var aCadaDezDias = new[] { 1, 11, 21, 31, 41, 51 }.Select(D);
        Assert.False(Formacao.PraticarVarias(new Habito("x"), aCadaDezDias).Automatico);
    }

    [Fact]
    public void Ordem_dos_dias_informados_nao_importa()
    {
        var a = Formacao.PraticarVarias(new Habito("x"), new[] { D(3), D(1), D(2) });
        var b = Formacao.PraticarVarias(new Habito("x"), new[] { D(1), D(2), D(3) });
        Assert.Equal(a, b);
    }

    [Fact]
    public void Limite_de_dias_curto_demais_devolve_null() =>
        Assert.Null(Formacao.DiasAteAutomatico(new Habito("x"), D(1), limiteDeDias: 3));
}
```

Dois testes valem um comentário. **`Praticar_devolve_um_habito_novo_e_o_original_continua_igual`** é a prova da imutabilidade. E **`Praticar_de_vez_em_quando_nao_chega_ao_automatico`** diz algo importante sobre hábitos: **a frequência importa mais que a intensidade**. Praticar uma vez a cada dez dias nunca chega lá, porque o desgaste anula o ganho.

::: exercicio Quanto tempo até deixar de ser automático?
Escreva o método de extensão `DiasAteDeixarDeSerAutomatico(this Habito habito)`: se o hábito é automático, devolve **quantos dias sem praticar** são necessários até a força cair abaixo do limiar; senão, devolve `null`. Dica: resolva `força × 0,5^(d/14) < 0,8` para `d` usando `Math.Log2`.

--- resposta
```cs Jornada/Habitos/Exercicios/Cap107Esquecimento.cs
namespace Jornada.Habitos.Cap107;

public static class HabitoExtensoes
{
    /// <summary>
    /// Quantos dias sem praticar até o hábito deixar de ser automático?
    /// Resolve força × 0,5^(d/14) &lt; limiar. Se o hábito já não é automático, devolve null.
    /// </summary>
    public static int? DiasAteDeixarDeSerAutomatico(this Habito habito)
    {
        if (!habito.Automatico)
            return null;

        double dias = Formacao.MeiaVidaEmDias * Math.Log2(habito.Forca / Habito.Limiar);
        return (int)Math.Floor(dias) + 1;
    }
}
```

A matemática: `força × 0,5^(d/14) < 0,8` ⇔ `d > 14 × log₂(força / 0,8)`. Como `d` precisa ser um número **inteiro de dias** e queremos o primeiro que cruza o limiar, usamos `Math.Floor(...) + 1`. O teste confere os dois lados: no dia `d` a força está **abaixo** do limiar, e no dia `d − 1` ainda está **acima**. É um bom hábito (outro!) **testar a fronteira** das contas.
:::

::: resumo
- Para o autor, **virtudes** são bons **hábitos morais**, uma segunda natureza gravada por **imitação** e **prática**; por isso **amar é um hábito**, e o desejo é formado por treino, não só por convicção.
- Hábitos **podem** ser formados e re-formados, e isso depende de tempo, exemplos e companhia.
- Um **`record`** imutável, com a expressão **`with`**, modela um hábito: cada prática devolve um **novo** valor e o antigo continua igual.
- **Parâmetros opcionais** (`comMentor = false`) dão um comportamento padrão sem sobrecargas extras; `Aggregate` aplica uma sequência de práticas.
- **Frequência** vence intensidade: práticas espaçadas demais são desfeitas pelo desgaste.
:::

::: quiz
? Segundo o capítulo, o que é uma virtude?
+ Um bom hábito moral, gravado por prática até virar "segunda natureza"
- Uma regra externa a obedecer
- Uma ideia que se decora
- Uma opinião sobre o bem
> Diferente da regra, a virtude vem de dentro.

? Quais são os dois caminhos que formam um hábito, nessa tradição?
+ Imitação e prática
- Leitura e memorização
- Punição e recompensa
- Informação e opinião
> Aprendemos vendo quem já é assim e repetindo até incorporar.

? O que significa "imutável" no `Habito`?
+ Cada prática devolve um hábito novo; o antigo não é alterado
- O hábito nunca muda de força
- O hábito não tem nome
- O hábito não pode ser praticado
> `with` cria uma cópia, mudando só o que for dito.

? Por que `Praticar` olha a força "de ontem" (`dia.AddDays(-1)`)?
+ Porque o desgaste só deve contar nos dias sem prática
- Porque ontem é sempre mais fraco
- Porque o `DateOnly` não aceita hoje
- Porque isso torna a conta mais rápida
> Praticar em dias seguidos não pode desgastar nada.

? Por que praticar uma vez a cada dez dias não chega ao automático, no modelo?
+ Porque o desgaste entre as práticas anula o ganho
- Porque o ganho é zero
- Porque o limite de dias é pequeno
- Porque o código tem um erro
> A frequência importa mais que a intensidade.

? Os números do modelo (0,12; 14 dias) devem ser lidos como...
+ Ilustrações simples, não medidas reais de ninguém
- Leis científicas dos hábitos
- Metas obrigatórias
- Valores exatos para todas as pessoas
> O modelo dá forma a uma ideia; não prevê pessoas.
:::

::: cartas
Virtude :: Bom hábito moral: disposição gravada por prática até virar segunda natureza.
Vício :: Hábito moral ruim.
Segunda natureza :: O que se faz sem precisar pensar, depois de praticado.
Imitação :: Aprender uma maneira de ser vendo quem já é assim.
Imutabilidade :: Um valor que não muda por dentro: mudar gera outro valor.
Expressão with :: Cria uma cópia de um record alterando só algumas propriedades.
Parâmetro opcional :: Parâmetro com valor padrão: `bool comMentor = false`.
Meia-vida :: Tempo para uma quantidade cair pela metade; aqui, 14 dias sem praticar.
:::
