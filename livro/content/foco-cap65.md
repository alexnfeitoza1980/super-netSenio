---
id: foco-cap65
order: 87
kind: capitulo
num: 65
group: C# & Mentalidade · Parte 13 · Foco em C#
title: Empatia: os três tipos e os sinais do usuário
subtitle: Foco no outro tem três faces: entender (cognitiva), sentir junto (emocional) e agir (compassiva). Um detector de frustração do usuário e respostas de suporte que usam as três, com cuidados éticos.
minutes: 55
hue: 290
objectives:
- Distinguir **empatia cognitiva**, **emocional** e **compassiva**
- Entender por que **atenção** é a base da empatia, e **pressa** e **poder** a prejudicam
- Transformar **sinais de comportamento** em um indicador de **frustração do usuário**
- Compor uma **resposta de suporte** que reconhece, entende e ajuda
- Pesar os **limites éticos**: privacidade, inferência imprecisa, dados mínimos
origem: livros/04-foco-em-csharp/content/97-cap65.md
---

::: vila O formulário que não deixava enviar
Bia assiste a uma gravação anônima (e autorizada) de uso do sistema. O usuário clica quatro vezes no mesmo botão, volta duas vezes à página anterior e, no fim, fecha tudo. Ela comenta: "Não temos nenhum e-mail de reclamação, mas **essa pessoa nos disse tudo** — só que sem palavras".
:::

::: ilustracao img/foco/cap65.svg | As três empatias: a cognitiva entende, a emocional sente junto, a compassiva age.
:::

## O que o livro diz

> A **empatia** não é uma coisa só. Há três tipos: a **cognitiva** (entender o ponto de vista do outro), a **emocional** (sentir o que o outro sente) e a **compassiva** (preocupar-se e **agir** para ajudar). Todas dependem de **dar atenção** ao outro.

O que vale levar:

- **Empatia cognitiva** sem emoção pode ser fria (e manipuladora, no extremo dos sociopatas, que entendem bem as pessoas, mas não sentem junto).
- **Empatia emocional** em excesso **esgota**: médicos que sentem demais a dor do paciente se desgastam; um equilíbrio permite cuidar sem se afogar.
- **Compaixão** é empatia + **vontade de ajudar** — a que mais produz ações úteis.
- **Atenção plena ao outro** exige tempo; a **pressa** reduz a empatia (o célebre estudo do "bom samaritano" com seminaristas atrasados).
- Pessoas com **mais poder** tendem a prestar **menos atenção** nas de menos poder; o livro cita que, no tempo de resposta de e-mails, a **hierarquia** aparece.
- Ler o **contexto** (quem está na sala, que regras valem) é parte da sensibilidade social.

::: cuidado Sem exagero
Os resultados de laboratório sobre **poder** e **pressa** são interessantes, mas não significam que toda pessoa em posição de liderança seja desatenta, nem que sob pressão ninguém se importe. Eles mostram **tendências**, e as tendências se combatem com **hábitos**, não com culpa.
:::

## Da vida para o código

| No livro | Em engenharia de software |
|---|---|
| Empatia cognitiva | **Entender o usuário**: pesquisa, personas, *logs* |
| Empatia emocional | **Sentir o atrito**: assistir a uma sessão de uso |
| Compaixão | **Agir**: corrigir o problema, responder bem |
| Pressa reduz empatia | **Prazo apertado** → pior cuidado com o usuário |
| Poder reduz atenção | **Decisões sem ouvir** quem usa o produto |
| Sensibilidade ao contexto | **Mensagem certa**, no lugar certo, para a pessoa certa |

::: figura As três empatias
<svg viewBox="0 0 740 246" role="img" aria-label="Três círculos: entender, sentir junto, agir; compaixão no encontro dos três">
  <g font-size="13" text-anchor="middle" font-weight="800">
    <circle class="f-info" cx="230" cy="100" r="80" style="fill-opacity:.55"/><text x="188" y="90">Entender</text><text x="188" y="108" font-size="11" font-weight="400">cognitiva</text>
    <circle class="f-warn" cx="320" cy="100" r="80" style="fill-opacity:.55"/><text x="368" y="90">Sentir junto</text><text x="368" y="108" font-size="11" font-weight="400">emocional</text>
    <circle class="f-good" cx="275" cy="150" r="80" style="fill-opacity:.55"/><text x="275" y="178">Agir</text><text x="275" y="194" font-size="11" font-weight="400">compassiva</text>
  </g>
  <text x="520" y="80" font-size="13" font-weight="700">Para o produto:</text>
  <text x="520" y="104" font-size="12">• entender: dados e pesquisa</text>
  <text x="520" y="124" font-size="12">• sentir: ver o atrito de perto</text>
  <text x="520" y="144" font-size="12">• agir: corrigir e responder bem</text>
</svg>
:::

## 1) Sinais de frustração

Um sistema pode **notar** o atrito sem ler pensamentos: certos comportamentos aparecem junto da frustração. Pesos simples transformam uma sequência de sinais em um **nível**:

```cs Jornada/Foco/Cap65/DetectorDeFrustracao.cs
namespace Jornada.Foco.Cap65;

public enum TipoDeSinal { CliqueRepetido, ErroDeFormulario, VoltouPagina, TempoParado, PedidoDeAjuda }

public sealed record Sinal(TipoDeSinal Tipo, int Quantidade = 1);

public enum Nivel { Tranquilo, Atencao, Frustrado }

public static class DetectorDeFrustracao
{
    private static readonly Dictionary<TipoDeSinal, int> Pesos = new()
    {
        [TipoDeSinal.CliqueRepetido]  = 3,
        [TipoDeSinal.ErroDeFormulario] = 2,
        [TipoDeSinal.VoltouPagina]    = 1,
        [TipoDeSinal.TempoParado]     = 1,
        [TipoDeSinal.PedidoDeAjuda]   = 2,
    };

    public static int Pontuacao(IEnumerable<Sinal> sinais) =>
        sinais.Sum(s => Pesos[s.Tipo] * Math.Max(0, s.Quantidade));

    public static Nivel Classificar(int pontos) => pontos switch
    {
        < 4 => Nivel.Tranquilo,
        < 8 => Nivel.Atencao,
        _   => Nivel.Frustrado,
    };

    public static Nivel Classificar(IEnumerable<Sinal> sinais) => Classificar(Pontuacao(sinais));
}
```

```cs Uso
var sessoes = new Dictionary<string, Sinal[]>
{
    ["A"] = new[] { new Sinal(TipoDeSinal.TempoParado, 2) },
    ["B"] = new[] { new Sinal(TipoDeSinal.ErroDeFormulario, 2), new Sinal(TipoDeSinal.VoltouPagina, 2) },
    ["C"] = new[]
    {
        new Sinal(TipoDeSinal.CliqueRepetido, 4),
        new Sinal(TipoDeSinal.ErroDeFormulario, 2),
        new Sinal(TipoDeSinal.VoltouPagina),
    },
};

foreach (var (nome, sinais) in sessoes)
    Console.WriteLine($"Sessão {nome}: {DetectorDeFrustracao.Pontuacao(sinais),2} pontos → {DetectorDeFrustracao.Classificar(sinais)}");
```

```saida
Sessão A:  2 pontos → Tranquilo
Sessão B:  6 pontos → Atencao
Sessão C: 17 pontos → Frustrado
```

::: cuidado Inferir emoção é impreciso — e sensível
Cliques repetidos podem indicar frustração, mas também uma tela lenta, ou só uma pessoa muito rápida. Esses pesos são **palpites iniciais**: valide com **sessões reais** e **perguntas diretas** ("isso ajudou?"). E sobretudo: **colete o mínimo**, com **consentimento** e finalidade clara; registrar o comportamento de alguém é uma responsabilidade, não um brinquedo.
:::

## 2) Respondendo com as três empatias

O nível detectado orienta o **tom** da resposta. Em um caso frustrado, vale usar as três empatias: **reconhecer** a emoção (emocional), **mostrar que entendeu o problema** (cognitiva) e **ajudar de verdade** (compaixão):

```cs Jornada/Foco/Cap65/Suporte.cs
namespace Jornada.Foco.Cap65;

public sealed record RespostaEmpatica(string? Reconhecer, string? Entender, string Ajudar)
{
    public string Texto() =>
        string.Join(" ", new[] { Reconhecer, Entender, Ajudar }.Where(t => !string.IsNullOrWhiteSpace(t)));
}

public static class Suporte
{
    public static RespostaEmpatica Responder(Nivel nivel, string problema, string solucao) => nivel switch
    {
        Nivel.Tranquilo => new(null, null, solucao),
        Nivel.Atencao   => new(null, $"Parece que {problema}.", solucao),
        Nivel.Frustrado => new("Sentimos muito pelo transtorno.", $"Vimos que {problema}.", solucao),
        _ => throw new ArgumentOutOfRangeException(nameof(nivel)),
    };
}
```

```cs Uso
const string problema = "o botão Enviar não respondeu";
const string solucao  = "Já corrigimos: tente de novo, e se persistir, responda esta mensagem.";

foreach (var nivel in Enum.GetValues<Nivel>())
    Console.WriteLine($"[{nivel}] {Suporte.Responder(nivel, problema, solucao).Texto()}");
```

```saida
[Tranquilo] Já corrigimos: tente de novo, e se persistir, responda esta mensagem.
[Atencao] Parece que o botão Enviar não respondeu. Já corrigimos: tente de novo, e se persistir, responda esta mensagem.
[Frustrado] Sentimos muito pelo transtorno. Vimos que o botão Enviar não respondeu. Já corrigimos: tente de novo, e se persistir, responda esta mensagem.
```

Repare na **ordem**: reconhecer → entender → ajudar. Muitos *chatbots* invertem (ajudam sem reconhecer) ou ficam só no pedido de desculpas **sem ajudar**; a empatia completa precisa das três partes quando o atrito é alto.

```cs JornadaTests/Foco/Cap65/Cap65Tests.cs
using Jornada.Foco.Cap65;
using Xunit;

public class Cap65Tests
{
    [Theory]
    [InlineData(0, Nivel.Tranquilo)]
    [InlineData(3, Nivel.Tranquilo)]
    [InlineData(4, Nivel.Atencao)]
    [InlineData(7, Nivel.Atencao)]
    [InlineData(8, Nivel.Frustrado)]
    public void Limites_dos_niveis(int pontos, Nivel esperado) =>
        Assert.Equal(esperado, DetectorDeFrustracao.Classificar(pontos));

    [Fact]
    public void Pontuacao_soma_peso_vezes_quantidade() =>
        Assert.Equal(3 * 4 + 2 * 2, DetectorDeFrustracao.Pontuacao(new[]
        {
            new Sinal(TipoDeSinal.CliqueRepetido, 4),
            new Sinal(TipoDeSinal.ErroDeFormulario, 2),
        }));

    [Fact]
    public void Quantidade_negativa_nao_reduz_pontuacao() =>
        Assert.Equal(0, DetectorDeFrustracao.Pontuacao(new[] { new Sinal(TipoDeSinal.CliqueRepetido, -5) }));

    [Fact]
    public void Resposta_tranquila_so_ajuda() =>
        Assert.Equal("Resolvido.", Suporte.Responder(Nivel.Tranquilo, "x", "Resolvido.").Texto());

    [Fact]
    public void Resposta_frustrada_tem_tres_partes()
    {
        var r = Suporte.Responder(Nivel.Frustrado, "x quebrou", "Resolvido.");
        Assert.NotNull(r.Reconhecer);
        Assert.NotNull(r.Entender);
        Assert.Equal("Resolvido.", r.Ajudar);
    }
}
```

::: curiosidade O e-mail e a hierarquia
O livro menciona um estudo (de Adam Galinsky e outros) em que quem tem mais poder tende a **escrever e responder menos** a quem tem menos poder; em uma empresa, o **tempo de resposta** do e-mail revelou, pelo padrão, quem estava acima de quem. Se você lidera, uma pergunta honesta é: **quanto tempo levo para responder a quem não tem poder sobre mim?**
:::

::: exercicio Resposta com contexto
Estenda `Suporte` com um parâmetro `bool UsuarioComPressa` (por exemplo, o usuário está num fluxo de pagamento): quando `true` e o nível for `Atencao` ou `Frustrado`, a resposta deve **ir direto à solução**, sem reconhecer nem explicar (a pessoa tem pressa). Escreva um teste.

--- resposta
```cs Jornada/Foco/Cap65/SuporteComContexto.cs
namespace Jornada.Foco.Cap65;

public static class SuporteComContexto
{
    public static RespostaEmpatica Responder(Nivel nivel, string problema, string solucao, bool usuarioComPressa)
    {
        if (usuarioComPressa && nivel != Nivel.Tranquilo)
            return new RespostaEmpatica(null, null, solucao);   // contexto: pressa pede brevidade

        return Suporte.Responder(nivel, problema, solucao);
    }
}
```
```cs Jornada/Foco/Cap65/SuporteComContextoTests.cs
using Jornada.Foco.Cap65;
using Xunit;

public class SuporteComContextoTests
{
    [Fact]
    public void Com_pressa_vai_direto_a_solucao() =>
        Assert.Equal("Tente de novo.",
            SuporteComContexto.Responder(Nivel.Frustrado, "x quebrou", "Tente de novo.", usuarioComPressa: true).Texto());

    [Fact]
    public void Sem_pressa_mantem_as_tres_partes() =>
        Assert.Contains("Sentimos muito",
            SuporteComContexto.Responder(Nivel.Frustrado, "x quebrou", "Tente de novo.", usuarioComPressa: false).Texto());
}
```
Esse é o **contexto** do livro: a mesma empatia, em outra forma — aqui, a atitude mais empática é **ser breve**.
:::

::: resumo
- Três empatias: **cognitiva** (entender), **emocional** (sentir junto), **compassiva** (agir); todas começam em **atenção**.
- **Pressa** e **poder** tendem a reduzir a atenção ao outro; hábitos contornam tendências.
- O código pode **notar atrito** com sinais de comportamento, mas a inferência é **imprecisa**: valide, pergunte, colete o **mínimo** e com **consentimento**.
- Respostas de suporte boas **reconhecem, entendem e ajudam**; o contexto (pressa, canal) muda a forma.
- Empatia emocional em excesso esgota: o ponto é **equilíbrio**.
:::

::: quiz
? O que é empatia compassiva?
+ Preocupar-se com o outro e agir para ajudar
- Apenas entender o ponto de vista
- Apenas sentir a mesma emoção
- Evitar conflitos
> É a que mais produz ações úteis.

? Por que um detector de frustração deve ser usado com cuidado?
+ Inferir emoção de comportamento é impreciso e envolve privacidade
- Porque cliques não podem ser medidos
- Porque a lei proíbe cliques
- Porque é muito lento
> Valide, pergunte e colete o mínimo.

? Qual a ordem sugerida para uma resposta a um usuário frustrado?
+ Reconhecer, entender, ajudar
- Ajudar, culpar, encerrar
- Pedir desculpas apenas
- Ignorar o sentimento
> As três empatias juntas.

? O que o estudo do "bom samaritano" sugere?
+ Que a pressa reduz a atenção e a disposição de ajudar
- Que seminaristas são insensíveis
- Que ajudar é desnecessário
- Que religião impede empatia
> As tendências se combatem com hábitos.

? Por que pessoas com mais poder podem prestar menos atenção?
+ Estudos mostram uma tendência a atender menos a quem tem menos poder
- Porque são sempre insensíveis
- Porque trabalham menos
- Porque não usam e-mail
> É uma tendência, não uma regra.

? No exercício, por que ser breve é empático?
+ Quando o usuário tem pressa, ir direto à solução respeita o contexto
- Porque economiza código
- Porque evita pedir desculpas sempre
- Porque a empatia não importa
> A mesma empatia, em outra forma.
:::

::: cartas
Empatia cognitiva :: Entender o ponto de vista do outro.
Empatia emocional :: Sentir junto o que o outro sente.
Compaixão :: Empatia mais vontade de ajudar.
Sociopata :: Quem entende os outros, mas não sente junto.
Sensibilidade ao contexto :: Perceber regras e situação para agir de forma adequada.
Sinal de frustração :: Comportamento associado ao atrito (cliques repetidos, erros).
Privacidade por padrão :: Coletar o mínimo de dados, com consentimento.
Efeito do poder :: Tendência de quem tem mais poder a atender menos os outros.
:::
