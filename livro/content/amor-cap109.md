---
id: amor-cap109
order: 141
kind: capitulo
num: 109
group: C# & Mentalidade · Parte 17 · Você é aquilo que ama, em C#
title: As liturgias "seculares": a forma ensina
subtitle: Shoppings, estádios e telas como práticas que treinam o desejo, e a ideia de que a forma de um encontro ensina tanto quanto o conteúdo. Em C#, é o capítulo dos flags em profundidade e dos valores padrão com consequência moral.
minutes: 70
hue: 305
objectives:
- Entender a ideia de **"liturgia secular"**: práticas do dia a dia que carregam uma visão de vida boa
- Ver por que a **forma** de um produto ou encontro **ensina**, mesmo sem dizer nada
- Aprofundar `[Flags]`: **`1 << n`**, **`BitOperations.PopCount`**, **`HasFlag`** e `B & ~A`
- Usar **`required`** e **`init`**, e entender por que o **valor padrão** é uma decisão de design com peso ético
origem: livros/08-voce-e-aquilo-que-ama-csharp/content/129-cap109.md
---

::: vila O aplicativo que não deixava parar
Beto estava feliz com o app de estudos que a empresa lançou: sequência diária, "medalhas", notificações no fim da tarde, tela que nunca acabava. Em duas semanas, usava 40 minutos por dia. Numa noite, percebeu que estava abrindo o app **sem decidir** abrir, só porque o polegar sabia onde ir. "Eu aprendi mais sobre o app", disse a Lia, "ou o app me **ensinou** a abrir o app?".
:::

::: ilustracao img/amor/cap109.svg | Um shopping, um feed e um estádio têm algo em comum: ritmos repetidos que ensinam o que desejar. A forma conta uma história.
:::

## O que o livro diz

Smith propõe um exercício que ele chama de **leitura "apocalíptica"** (no sentido antigo de "revelar o que está oculto") das práticas do cotidiano: olhar para instituições familiares como se as víssemos pela primeira vez, **atentos ao que fazem conosco**, e não apenas ao que "dizem". O exemplo central é o **shopping center**, lido como se fosse um **templo**:

- **Arquitetura que acolhe.** Entradas amplas, espaços que lembram **santuários**, um percurso pensado para nos conduzir, num tempo "fora do relógio".
- **Ritmos e rituais.** Um calendário de ocasiões, rotinas de entrada, escolha e **consumação** (o ato de pagar), até a saída "abençoada" pela atendente.
- **Uma promessa de "salvação".** A imagem de uma vida **completa** (a família sorridente do cartaz, o produto que nos faz felizes) sugere que algo **em nós** está faltando e que **aquele produto** resolve.
- **Retornos decrescentes.** A emoção da compra e o brilho do novo **passam**; a insatisfação exige uma **nova** aquisição.
- **Os custos invisíveis.** O que sustenta esse modo de vida (recursos, trabalho barato) raramente aparece.

O ponto central dele, **não** é "shoppings são maus". É que o shopping ensina **pela forma**, e não por "mensagens" que possamos checar e refutar com argumentos: ninguém lê no recibo "consumo, logo existo". Ainda assim, **é isso que a prática ensina**. E, como o amor é hábito, ele conclui que não nos defendemos desse tipo de formação só **pensando** bem sobre ela.

Mais adiante no livro, o autor leva a ideia para dentro das **próprias comunidades de fé**: ao adotar o **formato** de um shopping, de um show ou de um café, para ficar "relevante", um encontro pode estar ensinando, pela forma, algo que o conteúdo nega. Em suas palavras (livres), as **formas não são recipientes neutros**: elas **fazem algo** com quem passa por elas.

::: cuidado Sem demonizar
Shoppings, estádios e aplicativos também reúnem pessoas, sustentam empregos e dão prazer legítimo. A leitura "litúrgica" é uma **lente** para perceber o que as práticas ensinam; não é um veredito sobre quem as frequenta. E o autor escreve de dentro de uma tradição religiosa, com critérios próprios do que é "boa vida"; aqui, usamos só o **método**: ler a forma, perguntar o que ela treina.
:::

## Da vida para o código

| No livro | Em C# |
|---|---|
| Práticas que carregam uma visão de vida boa | **`PadraoDeDesign`**: rolagem infinita, sequência diária, recompensa variável... |
| Vários padrões ao mesmo tempo | **`[Flags] enum`** com `1 << n` |
| "Que prática é essa, afinal?" | `Examinar` devolve **uma pergunta de cuidado por padrão** |
| Quantos estão ligados? | `BitOperations.PopCount` |
| O que a nova versão acrescentou? | `Depois.Padroes & ~Antes.Padroes` |
| A forma não é neutra: o **padrão** ensina | `PreferenciasDeNotificacao` com valores padrão **cuidadosos** |
| Quem cria é obrigado a dizer o nome | **`required`** + **`init`** |

::: figura Uma tela, vários "rituais" ligados ao mesmo tempo
<svg viewBox="0 0 740 230" role="img" aria-label="Um fluxo de produto com sete interruptores de padrões de design, três deles ligados: rolagem infinita, recompensa variável e notificação push">
  <rect x="20" y="20" width="700" height="190" rx="16" class="f-prim" opacity=".25"/>
  <text x="370" y="46" text-anchor="middle" font-weight="800">Fluxo: "Feed de novidades"</text>
  <g font-size="12" text-anchor="middle">
    <rect x="40" y="64" width="150" height="40" rx="10" class="f-bad"/><text x="115" y="89">Rolagem infinita · ON</text>
    <rect x="200" y="64" width="150" height="40" rx="10" class="f-bad"/><text x="275" y="89">Recompensa variável · ON</text>
    <rect x="360" y="64" width="150" height="40" rx="10" class="f-bad"/><text x="435" y="89">Notificação push · ON</text>
    <rect x="520" y="64" width="150" height="40" rx="10" class="f-info" opacity=".6"/><text x="595" y="89">Sequência diária · off</text>
    <rect x="120" y="116" width="150" height="40" rx="10" class="f-info" opacity=".6"/><text x="195" y="141">Opt-in padrão · off</text>
    <rect x="280" y="116" width="150" height="40" rx="10" class="f-info" opacity=".6"/><text x="355" y="141">Prova social · off</text>
    <rect x="440" y="116" width="150" height="40" rx="10" class="f-info" opacity=".6"/><text x="515" y="141">Escassez falsa · off</text>
  </g>
  <text x="370" y="190" text-anchor="middle" font-size="13" class="t-dim">Em número: 1 + 4 + 8 = 13 = 0000 1101 → três bits ligados → PopCount = 3. Cada um vira uma pergunta de cuidado.</text>
</svg>
:::

## 1) Padrões como bits

No capítulo 105, vimos o básico de `[Flags]`. Aqui vamos mais fundo: cada padrão de design é um **bit** do número, e escrevemos o valor com **deslocamento de bits** (`1 << n`, "o número 1 empurrado `n` casas para a esquerda"), o que é mais legível que 1, 2, 4, 8, 16...

| Padrão | `1 << n` | Valor |
|---|---|---|
| `RolagemInfinita` | `1 << 0` | 1 |
| `SequenciaDiaria` | `1 << 1` | 2 |
| `RecompensaVariavel` | `1 << 2` | 4 |
| `NotificacaoPush` | `1 << 3` | 8 |
| `OptInPadrao` | `1 << 4` | 16 |
| `ProvaSocial` | `1 << 5` | 32 |
| `EscassezFalsa` | `1 << 6` | 64 |

Com isso, `Examinar` percorre **todos os valores do enum** (`Enum.GetValues<T>()`), **filtra** os que estão ligados (`HasFlag`) e devolve, para cada um, **uma pergunta de cuidado dirigida a quem desenha o produto**. Para contar quantos estão ligados, usamos **`BitOperations.PopCount`**: "conte quantos bits 1 há neste número".

E um truque útil de conjuntos: **`depois & ~antes`** devolve o que a nova versão **acrescentou**.

```cs Jornada/Habitos/Cap109/Fluxo.cs
using System.Numerics;

namespace Jornada.Habitos.Cap109;

/// <summary>
/// Padrões de desenho que treinam hábitos sem dizer nada. Cada um é um "bit" ligado ou desligado,
/// então vários cabem no mesmo número: é o que o atributo [Flags] permite.
/// </summary>
[Flags]
public enum PadraoDeDesign
{
    Nenhum             = 0,
    RolagemInfinita    = 1 << 0,
    SequenciaDiaria    = 1 << 1,
    RecompensaVariavel = 1 << 2,
    NotificacaoPush    = 1 << 3,
    OptInPadrao        = 1 << 4,
    ProvaSocial        = 1 << 5,
    EscassezFalsa      = 1 << 6
}

/// <summary>Descrição de um fluxo de produto, escrita por quem o desenhou. Nada aqui lê pessoas.</summary>
public sealed record Fluxo(string Nome, PadraoDeDesign Padroes, bool PermiteSairFacilmente);

public sealed record Observacao(PadraoDeDesign Padrao, string Pergunta);

public enum Gravidade { Leve, Moderada, Alta }

public static class AuditoriaDeFluxo
{
    private static readonly IReadOnlyDictionary<PadraoDeDesign, string> Perguntas = new Dictionary<PadraoDeDesign, string>
    {
        [PadraoDeDesign.RolagemInfinita]    = "Onde a pessoa encontra um ponto natural de parada?",
        [PadraoDeDesign.SequenciaDiaria]    = "O que acontece com quem perde um dia? O fim da sequência pune ou acolhe?",
        [PadraoDeDesign.RecompensaVariavel] = "A recompensa imprevisível serve à pessoa ou só a prende?",
        [PadraoDeDesign.NotificacaoPush]    = "A notificação traz algo que a pessoa pediu ou só um motivo para voltar?",
        [PadraoDeDesign.OptInPadrao]        = "O padrão veio ligado? Quem decidiu isso, e por quê?",
        [PadraoDeDesign.ProvaSocial]        = "O número mostrado ajuda a decidir ou só pressiona?",
        [PadraoDeDesign.EscassezFalsa]      = "A escassez é real? Se não for, por que mostrá-la?",
    };

    /// <summary>Uma pergunta de cuidado para cada padrão presente no fluxo.</summary>
    public static IReadOnlyList<Observacao> Examinar(Fluxo fluxo) =>
        Enum.GetValues<PadraoDeDesign>()
            .Where(p => p != PadraoDeDesign.Nenhum && fluxo.Padroes.HasFlag(p))
            .Select(p => new Observacao(p, Perguntas[p]))
            .ToList();

    /// <summary>Quantos padrões estão ligados ao mesmo tempo: conta os bits do número.</summary>
    public static int Quantidade(PadraoDeDesign padroes) => BitOperations.PopCount((uint)padroes);

    public static Gravidade Avaliar(Fluxo fluxo) => (Quantidade(fluxo.Padroes), fluxo.PermiteSairFacilmente) switch
    {
        (<= 1, true)  => Gravidade.Leve,
        (<= 2, true)  => Gravidade.Moderada,
        (_, false)    => Gravidade.Alta,
        (>= 5, _)     => Gravidade.Alta,
        _             => Gravidade.Moderada
    };

    /// <summary>Padrões que só estão ligados no fluxo B, em relação ao fluxo A. (B &amp; ~A)</summary>
    public static PadraoDeDesign Acrescentou(Fluxo antes, Fluxo depois) => depois.Padroes & ~antes.Padroes;
}
```

Repare no `switch` com **tupla** do `Avaliar`: ele combina **quantidade de padrões** e **facilidade de sair**. Um fluxo de que **não se sai facilmente** é "alta", **independentemente** da quantidade; e muitos padrões juntos também.

::: cuidado O que esta auditoria analisa
Ela analisa **o desenho de um produto**, descrito por quem o desenha. **Não** lê pessoas nem mede quem usa. A pergunta é de quem constrói: "**o que o meu produto está treinando?**".
:::

## 2) O valor padrão também ensina

O autor insiste que as **formas** não são neutras. Em software, uma das formas mais poderosas é o **valor padrão**: quase ninguém muda as configurações iniciais. Se as notificações **vêm ligadas**, a grande maioria as **terá ligadas**. O padrão **decide** pela pessoa, por omissão.

A classe abaixo mostra um padrão cuidadoso: notificações **desligadas**, **silêncio** das 21h às 8h, **limite** de três por dia. E introduz dois recursos novos do C#:

- **`required`**: uma propriedade que **precisa** ser informada ao criar o objeto. Se alguém escrever `new PreferenciasDeNotificacao { }` sem o `Produto`, o **compilador** recusa.
- **`init`**: a propriedade só pode ser definida **na criação** (no inicializador de objeto), nunca depois.

```cs Jornada/Habitos/Cap109/Preferencias.cs
namespace Jornada.Habitos.Cap109;

/// <summary>
/// Os valores PADRÃO ensinam. Quase ninguém muda as configurações iniciais, então o padrão é uma decisão
/// de design com consequência moral. Aqui, o padrão cuida de quem usa.
/// </summary>
public sealed class PreferenciasDeNotificacao
{
    /// <summary>"required": quem cria o objeto é obrigado a dizer o nome. O compilador cobra.</summary>
    public required string Produto { get; init; }

    public bool NotificacoesAtivas { get; init; } = false;

    public TimeSpan InicioDoSilencio { get; init; } = TimeSpan.FromHours(21);

    public TimeSpan FimDoSilencio { get; init; } = TimeSpan.FromHours(8);

    public int MaximoPorDia { get; init; } = 3;

    public bool EstaNoSilencio(TimeSpan horaDoDia) =>
        InicioDoSilencio <= FimDoSilencio
            ? horaDoDia >= InicioDoSilencio && horaDoDia < FimDoSilencio
            : horaDoDia >= InicioDoSilencio || horaDoDia < FimDoSilencio;     // atravessa a meia-noite

    public bool PodeNotificar(TimeSpan horaDoDia, int jaEnviadasHoje) =>
        NotificacoesAtivas && !EstaNoSilencio(horaDoDia) && jaEnviadasHoje < MaximoPorDia;
}
```

Veja o `EstaNoSilencio`: a janela das 21h às 8h **atravessa a meia-noite**, e o código trata os dois casos (início antes do fim, e início depois do fim) com cuidado. Esse é o tipo de detalhe em que bugs vivem.

```cs Uso
var feed = new Fluxo("Feed de novidades",
    PadraoDeDesign.RolagemInfinita | PadraoDeDesign.RecompensaVariavel | PadraoDeDesign.NotificacaoPush,
    PermiteSairFacilmente: true);

foreach (var o in AuditoriaDeFluxo.Examinar(feed))
    Console.WriteLine($"[{o.Padrao}] {o.Pergunta}");

Console.WriteLine($"Padrões ligados: {AuditoriaDeFluxo.Quantidade(feed.Padroes)} · gravidade: {AuditoriaDeFluxo.Avaliar(feed)}");

var prefs = new PreferenciasDeNotificacao { Produto = "App de estudos" };
Console.WriteLine($"Notificações ligadas por padrão? {prefs.NotificacoesAtivas}");
Console.WriteLine($"Silêncio às 23h? {prefs.EstaNoSilencio(TimeSpan.FromHours(23))} · às 15h? {prefs.EstaNoSilencio(TimeSpan.FromHours(15))}");
```

```saida
[RolagemInfinita] Onde a pessoa encontra um ponto natural de parada?
[RecompensaVariavel] A recompensa imprevisível serve à pessoa ou só a prende?
[NotificacaoPush] A notificação traz algo que a pessoa pediu ou só um motivo para voltar?
Padrões ligados: 3 · gravidade: Moderada
Notificações ligadas por padrão? False
Silêncio às 23h? True · às 15h? False
```

A saída mostra como o código **fala**: três perguntas para quem desenha o feed, uma gravidade "moderada" (pouca coisa ligada e saída fácil) e um padrão de notificações que **respeita** o silêncio da noite.

::: dica Pergunte antes de ligar
Antes de adicionar uma "gamificação" ao seu produto, pergunte: **a quem isso serve**? Se o usuário, ótimo. Se só serve para a métrica de "tempo na tela", você está escrevendo uma liturgia que ensina a voltar sem querer. Quem programa **escreve** rituais, quer perceba, quer não.
:::

## 3) Testes

```cs Habitos.Testes/Cap109Tests.cs
using Jornada.Habitos.Cap109;

namespace Habitos.Testes;

public class Cap109Tests
{
    private static readonly Fluxo Feed = new("Feed", PadraoDeDesign.RolagemInfinita | PadraoDeDesign.RecompensaVariavel | PadraoDeDesign.NotificacaoPush, true);

    [Fact]
    public void Cada_padrao_ligado_gera_uma_pergunta_de_cuidado()
    {
        var observacoes = AuditoriaDeFluxo.Examinar(Feed);

        Assert.Equal(3, observacoes.Count);
        Assert.Contains(observacoes, o => o.Padrao == PadraoDeDesign.RolagemInfinita && o.Pergunta.Contains("parada"));
        Assert.All(observacoes, o => Assert.EndsWith("?", o.Pergunta));
    }

    [Fact]
    public void Fluxo_sem_padroes_nao_tem_observacoes() =>
        Assert.Empty(AuditoriaDeFluxo.Examinar(new Fluxo("Formulário simples", PadraoDeDesign.Nenhum, true)));

    [Fact]
    public void Flags_se_combinam_com_ou_e_se_testam_com_HasFlag()
    {
        var p = PadraoDeDesign.SequenciaDiaria | PadraoDeDesign.ProvaSocial;

        Assert.True(p.HasFlag(PadraoDeDesign.SequenciaDiaria));
        Assert.False(p.HasFlag(PadraoDeDesign.NotificacaoPush));
        Assert.Equal(2, AuditoriaDeFluxo.Quantidade(p));
        Assert.Equal(7, AuditoriaDeFluxo.Quantidade((PadraoDeDesign)127));
    }

    [Theory]
    [InlineData(PadraoDeDesign.Nenhum, true, Gravidade.Leve)]
    [InlineData(PadraoDeDesign.RolagemInfinita, true, Gravidade.Leve)]
    [InlineData(PadraoDeDesign.RolagemInfinita | PadraoDeDesign.NotificacaoPush, true, Gravidade.Moderada)]
    [InlineData(PadraoDeDesign.RolagemInfinita, false, Gravidade.Alta)]
    public void Avaliacao_considera_quantidade_e_facilidade_de_sair(PadraoDeDesign padroes, bool sai, Gravidade esperada) =>
        Assert.Equal(esperada, AuditoriaDeFluxo.Avaliar(new Fluxo("x", padroes, sai)));

    [Fact]
    public void Muitos_padroes_juntos_sao_alta_mesmo_com_saida_facil()
    {
        var tudo = new Fluxo("x", (PadraoDeDesign)127, PermiteSairFacilmente: true);
        Assert.Equal(Gravidade.Alta, AuditoriaDeFluxo.Avaliar(tudo));
    }

    [Fact]
    public void Acrescentou_mostra_so_o_que_a_nova_versao_ligou()
    {
        var v1 = new Fluxo("v1", PadraoDeDesign.NotificacaoPush, true);
        var v2 = new Fluxo("v2", PadraoDeDesign.NotificacaoPush | PadraoDeDesign.SequenciaDiaria | PadraoDeDesign.EscassezFalsa, true);

        Assert.Equal(PadraoDeDesign.SequenciaDiaria | PadraoDeDesign.EscassezFalsa, AuditoriaDeFluxo.Acrescentou(v1, v2));
    }

    [Fact]
    public void O_padrao_de_notificacoes_cuida_de_quem_usa()
    {
        var prefs = new PreferenciasDeNotificacao { Produto = "App de estudos" };

        Assert.False(prefs.NotificacoesAtivas);
        Assert.Equal(3, prefs.MaximoPorDia);
        Assert.False(prefs.PodeNotificar(TimeSpan.FromHours(15), 0));      // desligado por padrão
    }

    [Theory]
    [InlineData(22, true)]
    [InlineData(3, true)]
    [InlineData(8, false)]
    [InlineData(15, false)]
    public void O_silencio_atravessa_a_meia_noite(int hora, bool esperado)
    {
        var prefs = new PreferenciasDeNotificacao { Produto = "x" };
        Assert.Equal(esperado, prefs.EstaNoSilencio(TimeSpan.FromHours(hora)));
    }

    [Fact]
    public void Ligadas_respeitam_silencio_e_limite_diario()
    {
        var prefs = new PreferenciasDeNotificacao { Produto = "x", NotificacoesAtivas = true };

        Assert.True(prefs.PodeNotificar(TimeSpan.FromHours(10), 2));
        Assert.False(prefs.PodeNotificar(TimeSpan.FromHours(10), 3));
        Assert.False(prefs.PodeNotificar(TimeSpan.FromHours(23), 0));
    }
}
```

Note como os testes tratam o `[Theory]` com `[InlineData]` e **valores de enum combinados** (`RolagemInfinita | NotificacaoPush`) para cobrir as faixas da avaliação. E `Acrescentou_mostra_so_o_que_a_nova_versao_ligou` verifica o truque `B & ~A`.

::: exercicio Descrever os padrões
Escreva o método de extensão `Descrever(this PadraoDeDesign padroes)`, que devolve os nomes dos padrões ligados separados por `" + "` (por exemplo, `"RolagemInfinita + NotificacaoPush"`), ou `"nenhum"` quando nada está ligado. Dica: `Enum.GetValues`, `HasFlag` e `string.Join`.

--- resposta
```cs Jornada/Habitos/Exercicios/Cap109Descrever.cs
namespace Jornada.Habitos.Cap109;

public static class PadraoExtensoes
{
    /// <summary>"RolagemInfinita + NotificacaoPush", ou "nenhum" quando nada está ligado.</summary>
    public static string Descrever(this PadraoDeDesign padroes)
    {
        var ligados = Enum.GetValues<PadraoDeDesign>()
                          .Where(p => p != PadraoDeDesign.Nenhum && padroes.HasFlag(p))
                          .ToList();

        return ligados.Count == 0 ? "nenhum" : string.Join(" + ", ligados);
    }
}
```

Observe que `string.Join(" + ", ligados)` aceita uma lista de **enums** e chama o `ToString()` de cada um. E o `p != PadraoDeDesign.Nenhum` evita listar o valor 0, que **não é um padrão**, é a ausência de todos. (`HasFlag(Nenhum)` é sempre verdadeiro, uma pegadinha clássica dos flags.)
:::

::: resumo
- Para o autor, práticas do cotidiano (como o shopping) funcionam como **liturgias**: ensinam o que desejar **pela forma**, e não só por "mensagens" que dá para refutar.
- **A forma não é neutra**: o formato de um encontro ou produto **faz algo** com quem passa por ele.
- **`[Flags]`** com `1 << n` guarda vários padrões em um número; `HasFlag` pergunta; `B & ~A` mostra o que foi acrescentado; `BitOperations.PopCount` conta os ligados.
- **O valor padrão é uma decisão moral**: `required` e `init` ajudam a tornar as escolhas explícitas e imutáveis.
- Quem programa **escreve rituais**: vale perguntar, antes de ligar um padrão, "a quem isso serve?".
:::

::: quiz
? O que significa chamar o shopping de "liturgia secular", no capítulo do livro?
+ Que suas práticas e arquitetura ensinam, pela forma, uma visão de vida boa, mesmo sem uma "mensagem" explícita
- Que shoppings são igrejas
- Que quem compra é religioso
- Que o comércio é errado
> A ideia é ler a forma e o ritual, e não só o discurso.

? O que o autor quer dizer ao afirmar que as formas "não são recipientes neutros"?
+ Que o formato de um encontro ou produto ensina algo, independentemente do conteúdo
- Que recipientes são sempre ruins
- Que formatos não podem mudar
- Que o conteúdo não importa
> A forma faz algo com quem passa por ela.

? Para que serve `1 << 3` na definição de um enum `[Flags]`?
+ Gerar o valor 8 (o bit número 3), de forma legível
- Dividir 1 por 3
- Girar o enum
- Criar um vetor
> Deslocar o 1 para a esquerda `n` casas gera potências de 2.

? O que faz `BitOperations.PopCount((uint)padroes)`?
+ Conta quantos bits estão ligados no número
- Converte o enum em texto
- Remove os bits ligados
- Soma os valores do enum
> É o jeito direto de contar quantos padrões estão ativos.

? O que o modificador `required` faz?
+ Obriga quem cria o objeto a informar aquela propriedade
- Impede que a propriedade seja lida
- Torna a propriedade estática
- Gera um valor aleatório
> O compilador recusa a criação sem a propriedade.

? Por que o valor padrão de `NotificacoesAtivas` é `false`?
+ Porque quase ninguém muda os padrões; então o padrão deve cuidar de quem usa
- Porque `false` é mais rápido
- Porque o C# exige
- Porque notificações são proibidas
> O padrão é uma decisão de design com peso ético.
:::

::: cartas
Liturgia secular :: Prática do dia a dia que ensina, pela forma e pelo ritmo, uma visão de vida boa.
Leitura apocalíptica :: Olhar práticas familiares para revelar o que fazem conosco.
Retornos decrescentes :: Cada nova aquisição dá menos satisfação que a anterior.
1 << n :: Desloca o bit 1 para a esquerda `n` casas: gera potências de 2.
PopCount :: Conta quantos bits 1 há em um número.
B & ~A :: O que está em B e não está em A.
required :: Propriedade que o criador do objeto é obrigado a informar.
init :: Propriedade que só pode ser definida na criação do objeto.
:::
