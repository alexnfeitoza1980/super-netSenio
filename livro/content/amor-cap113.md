---
id: amor-cap113
order: 145
kind: capitulo
num: 113
group: C# & Mentalidade · Parte 17 · Você é aquilo que ama, em C#
title: Aprender de cor
subtitle: Conhecer "nos ossos", aprender por imitação e repetição, e a sabedoria de rever em intervalos crescentes. Em C#, é o capítulo da repetição espaçada: uma caixa de Leitner com `record`, `with` e `Random` injetado.
minutes: 70
hue: 265
objectives:
- Entender o **conhecimento "de cor"** (corporificado, prático) como algo diferente de "saber que"
- Ver por que o aprendizado **formador** vai além de depositar informações, e como **tradições e rituais** o ajudam
- Implementar uma **repetição espaçada** (caixa de Leitner) com `record`, `with` e `Dictionary`
- **Injetar** um `Random` para tornar testável o que é aleatório
origem: livros/08-voce-e-aquilo-que-ama-csharp/content/133-cap113.md
---

::: vila Os atalhos que a mão sabe
Depois de um ano usando o mesmo editor, Caio notou algo estranho: sabia usar mais de 50 atalhos... mas, se lhe perguntassem **quais**, não conseguia listar dez. Os dedos sabiam sozinhos. Já a Marta, que tinha uma lista colada no monitor, **conhecia** todos... e usava dois. Lia sorriu: "um sabe **de cor**; a outra sabe **de cabeça**. Cada um precisa do outro".
:::

::: ilustracao img/amor/cap113.svg | Uma caixa com cinco compartimentos: o que acertamos vai para a frente e volta mais tarde; o que erramos volta logo. É assim que o conhecimento desce para a mão.
:::

## O que o livro diz

O capítulo do livro sobre educação começa com uma lembrança pessoal: o autor cresceu numa cidadezinha tão pequena que nem tinha semáforo, e conhece **cada atalho**, cada quintal, cada caminho para a casa de um amigo, **mas talvez não consiga explicá-los** a um estranho. Ele "sabe a cidade **de cor**", com os pés. É o que **"aprender de cor"** significa para ele: um conhecimento **incorporado**, adquirido por **viver** dentro de um lugar, de ritmos e de histórias, e não por ler um mapa.

A partir daí, ele critica um modelo de escola e de formação que vê o aluno como **recipiente** de informações, e defende um **aprendizado formador**: um que alcança a **imaginação**, forma **hábitos** e convida a **participar** de uma história. Algumas ideias que ele reúne:

- **Crianças amam tradições.** Em casa, qualquer coisa repetida duas vezes vira "tradição" (ele conta do "bolo do segundo dia de escola"). Ritmos e repetições não são entediantes: são **âncoras**.
- **Aprender é imitar e praticar.** Ele cita uma reflexão sobre tênis (um grande jogador responde a um saque em frações de segundo, graças a uma "sensação cinestésica" formada por **anos de treino**) para mostrar que há um tipo de saber que **só se tem na prática**.
- **O saber que forma é "intuição para o mundo".** Quem foi formado de modo profundo age bem "sem nem pensar a respeito". Isso é **mais fundo** do que o conhecimento: é uma experiência prática que a pessoa traz nos ossos.
- **Quem ensina também é aprendiz.** Ele convida professores a verem as próprias práticas (começar a aula, preparar o café, cantar juntos, ler juntos) como **rituais que formam**.

::: cuidado Sobre o contexto
Muito do capítulo trata de **educação cristã** (escola dominical, ministérios de jovens, universidades de fé), e as propostas dele dependem dessa tradição. Aqui, ficamos com o princípio: **o conhecimento que muda a prática é aquele que foi praticado**, em **ritmos**, com **exemplos**, e **de volta, mais de uma vez**.
:::

::: analogia Rever o caminho até a casa
A primeira vez que você vai a um lugar novo, olha o mapa o tempo todo. Na segunda, olha menos. Na décima, nem percebe que **sabia** o caminho. A cada repetição, o trajeto desce da cabeça para as pernas. A repetição **espaçada** é isso em ordem: rever logo o que é novo, e cada vez mais tarde o que já está firme.
:::

## Da vida para o código

| No livro | Em C# |
|---|---|
| Saber "de cor" (corporificado) | Cartões que descem de **caixa** a cada acerto |
| Repetição como âncora | Intervalos crescentes: 1, 2, 4, 8, 16 dias |
| Errou? Volta logo | `Responder(..., acertou: false)` → caixa 1, amanhã |
| O que está "vencido" hoje | `Devidos(hoje)` (LINQ + `OrderBy`/`ThenBy`) |
| Cada cartão é um valor que muda de estado | **`record` + `with`** (imutável) |
| Ordem da sessão varia, mas testável | `Random` **injetado** |
| "Quanto já está firme?" | `PorCaixa` (`Enumerable.Range` + `ToDictionary`) |

::: figura A caixa de Leitner
<svg viewBox="0 0 740 230" role="img" aria-label="Cinco caixas numeradas de 1 a 5 com intervalos de 1, 2, 4, 8 e 16 dias; setas para a direita em caso de acerto e uma seta de volta à caixa 1 em caso de erro">
  <g font-size="13" text-anchor="middle" font-weight="800">
    <rect x="30" y="60" width="120" height="80" rx="12" class="f-bad"/><text x="90" y="92">Caixa 1</text><text x="90" y="114" font-size="11" font-weight="400" class="t-dim">volta em 1 dia</text>
    <rect x="165" y="60" width="120" height="80" rx="12" class="f-warn"/><text x="225" y="92">Caixa 2</text><text x="225" y="114" font-size="11" font-weight="400" class="t-dim">volta em 2 dias</text>
    <rect x="300" y="60" width="120" height="80" rx="12" class="f-warn"/><text x="360" y="92">Caixa 3</text><text x="360" y="114" font-size="11" font-weight="400" class="t-dim">volta em 4 dias</text>
    <rect x="435" y="60" width="120" height="80" rx="12" class="f-info"/><text x="495" y="92">Caixa 4</text><text x="495" y="114" font-size="11" font-weight="400" class="t-dim">volta em 8 dias</text>
    <rect x="570" y="60" width="120" height="80" rx="12" class="f-good"/><text x="630" y="92">Caixa 5</text><text x="630" y="114" font-size="11" font-weight="400" class="t-dim">volta em 16 dias</text>
  </g>
  <path class="arrow" d="M152 100H163"/><path class="arrow" d="M287 100H298"/><path class="arrow" d="M422 100H433"/><path class="arrow" d="M557 100H568"/>
  <text x="360" y="44" text-anchor="middle" font-size="12" class="t-dim">acertou → sobe de caixa e demora mais para voltar</text>
  <path class="arrow" d="M630 142C630 210 90 210 90 144"/><text x="360" y="204" text-anchor="middle" font-size="12" class="t-dim">errou → volta para a caixa 1 (e revisa amanhã)</text>
</svg>
:::

## 1) Cartões imutáveis e uma caixa

Um cartão tem uma **frente** (pergunta), um **verso** (resposta), a **caixa** em que está e o **dia da próxima revisão**. É um `record`: cada resposta **gera um cartão novo** com `with`, e o antigo não é alterado (o mesmo princípio do hábito imutável do capítulo 107).

A **caixa de Leitner** é uma técnica de estudo simples e conhecida: o que você acerta **sobe** para uma caixa em que volta **mais tarde**; o que você erra **cai** para a primeira e volta **logo**. O estudo se concentra no que ainda é frágil, e o que está firme **só é revisitado de vez em quando**.

```cs Jornada/Habitos/Cap113/CaixaDeLeitner.cs
namespace Jornada.Habitos.Cap113;

/// <summary>Um cartão de estudo: uma pergunta, a resposta e em que "caixa" ele está (1 a 5).</summary>
public sealed record Cartao(string Frente, string Verso, int Caixa = 1, DateOnly Proxima = default);

/// <summary>
/// Repetição espaçada: o que se acerta volta cada vez mais tarde; o que se erra volta logo.
/// É uma forma de transformar "saber" em "saber de cor", aquilo que a mão faz sem pedir licença.
/// </summary>
public sealed class CaixaDeLeitner
{
    /// <summary>Dias até rever um cartão, conforme a caixa em que ele está (caixa 1 = índice 0).</summary>
    public static readonly IReadOnlyList<int> Intervalos = new[] { 1, 2, 4, 8, 16 };

    private readonly List<Cartao> cartoes = new();

    public IReadOnlyList<Cartao> Todos => cartoes;

    public void Adicionar(string frente, string verso, DateOnly hoje) =>
        cartoes.Add(new Cartao(frente, verso, 1, hoje));

    /// <summary>Os cartões que já "venceram" hoje: primeiro os mais frágeis (caixa baixa).</summary>
    public IReadOnlyList<Cartao> Devidos(DateOnly hoje) =>
        cartoes.Where(c => c.Proxima <= hoje).OrderBy(c => c.Caixa).ThenBy(c => c.Frente).ToList();

    /// <summary>A sessão do dia, embaralhada com um Random recebido de fora (assim o teste controla o sorteio).</summary>
    public IReadOnlyList<Cartao> SessaoDoDia(DateOnly hoje, Random sorteio) =>
        Devidos(hoje).OrderBy(_ => sorteio.Next()).ToList();

    public void Responder(string frente, bool acertou, DateOnly hoje)
    {
        int indice = cartoes.FindIndex(c => c.Frente == frente);
        if (indice < 0)
            throw new KeyNotFoundException($"Não há cartão com a frente \"{frente}\".");

        var cartao = cartoes[indice];
        int nova = acertou ? Math.Min(cartao.Caixa + 1, Intervalos.Count) : 1;

        cartoes[indice] = cartao with { Caixa = nova, Proxima = hoje.AddDays(Intervalos[nova - 1]) };
    }

    /// <summary>Quantos cartões há em cada caixa (as caixas vazias aparecem com 0).</summary>
    public IReadOnlyDictionary<int, int> PorCaixa() =>
        Enumerable.Range(1, Intervalos.Count)
                  .ToDictionary(caixa => caixa, caixa => cartoes.Count(c => c.Caixa == caixa));
}
```

Observe estes detalhes:

- **`Intervalos` é um `IReadOnlyList<int>`** estático: `{ 1, 2, 4, 8, 16 }`. Mudar o ritmo é mudar **uma linha**.
- **`Math.Min(cartao.Caixa + 1, Intervalos.Count)`**: depois da última caixa, o cartão **continua** nela. Quem já sabe de cor só é revisto a cada 16 dias.
- **`Devidos`**: `Where(c => c.Proxima <= hoje)` e, para empates, `OrderBy(c => c.Caixa).ThenBy(c => c.Frente)`: os **mais frágeis primeiro**.
- **`SessaoDoDia(hoje, Random sorteio)`**: recebe o `Random` **de fora**. Em produção, `new Random()`; nos testes, `new Random(7)`, que sempre dá a **mesma** sequência. Isso é **injeção de dependência** de novo (como o `TimeProvider` do Livro 6), aplicada ao acaso.
- **`PorCaixa`** usa `Enumerable.Range(1, 5).ToDictionary(...)`: as caixas **vazias aparecem com 0** (um dicionário só com `GroupBy` as esconderia).

```cs Uso
var hoje = new DateOnly(2026, 11, 2);
var caixa = new CaixaDeLeitner();
caixa.Adicionar("O que faz o switch expression?", "Devolve um valor para cada caso", hoje);
caixa.Adicionar("Para que serve o yield return?", "Entrega itens um a um", hoje);
caixa.Adicionar("O que é um record?", "Tipo para dados imutáveis", hoje);

Console.WriteLine($"Para revisar hoje: {caixa.Devidos(hoje).Count} cartões");

caixa.Responder("O que é um record?", acertou: true, hoje);
caixa.Responder("O que faz o switch expression?", acertou: true, hoje);
caixa.Responder("Para que serve o yield return?", acertou: false, hoje);

foreach (var c in caixa.Todos)
    Console.WriteLine($"{c.Frente} → caixa {c.Caixa}, volta em {c.Proxima:dd/MM}");

Console.WriteLine("Cartões por caixa: " + string.Join(" · ", caixa.PorCaixa().Select(p => $"{p.Key}:{p.Value}")));
```

```saida
Para revisar hoje: 3 cartões
O que faz o switch expression? → caixa 2, volta em 04/11
Para que serve o yield return? → caixa 1, volta em 03/11
O que é um record? → caixa 2, volta em 04/11
Cartões por caixa: 1:1 · 2:2 · 3:0 · 4:0 · 5:0
```

Acompanhe o que aconteceu: dos três cartões, **dois acertos** subiram para a caixa 2 (voltam em 2 dias, no dia 04) e **um erro** voltou para a caixa 1 (volta amanhã, dia 03). Amanhã o foco será o que **ainda não está firme**.

::: dica Cartões bons perguntam "como", não "o quê"
Um cartão do tipo "o que é um record?" treina **cabeça**. Um cartão do tipo "escreva um `record` com `with`" treina **mão**. Para "aprender de cor", prefira o segundo tipo: **faça** o que o cartão pede, em vez de só reconhecer a resposta.
:::

## 2) Testes

```cs Habitos.Testes/Cap113Tests.cs
using Jornada.Habitos.Cap113;

namespace Habitos.Testes;

public class Cap113Tests
{
    private static DateOnly D(int dia) => new DateOnly(2026, 11, 1).AddDays(dia - 1);

    private static CaixaDeLeitner Caixa()
    {
        var c = new CaixaDeLeitner();
        c.Adicionar("O que faz o switch expression?", "Devolve um valor para cada caso", D(1));
        c.Adicionar("Para que serve o yield return?", "Entrega itens um a um", D(1));
        c.Adicionar("O que é um record?", "Tipo para dados imutáveis", D(1));
        return c;
    }

    [Fact]
    public void Cartao_novo_comeca_na_caixa_um_e_vence_no_mesmo_dia()
    {
        var c = Caixa();
        Assert.All(c.Todos, x => Assert.Equal(1, x.Caixa));
        Assert.Equal(3, c.Devidos(D(1)).Count);
    }

    [Fact]
    public void Acertar_sobe_de_caixa_e_adia_a_proxima_revisao()
    {
        var c = Caixa();
        c.Responder("O que é um record?", acertou: true, D(1));

        var cartao = c.Todos.Single(x => x.Frente == "O que é um record?");
        Assert.Equal(2, cartao.Caixa);
        Assert.Equal(D(3), cartao.Proxima);                       // caixa 2 → 2 dias
        Assert.Equal(2, c.Devidos(D(2)).Count);                   // os outros dois ainda vencem
    }

    [Fact]
    public void Errar_volta_para_a_caixa_um_e_revisa_no_dia_seguinte()
    {
        var c = Caixa();
        c.Responder("O que é um record?", true, D(1));
        c.Responder("O que é um record?", true, D(3));
        c.Responder("O que é um record?", false, D(7));

        var cartao = c.Todos.Single(x => x.Frente == "O que é um record?");
        Assert.Equal(1, cartao.Caixa);
        Assert.Equal(D(8), cartao.Proxima);
    }

    [Fact]
    public void Quem_acerta_sempre_chega_a_ultima_caixa_e_la_fica()
    {
        var c = Caixa();
        var dia = 1;
        for (int i = 0; i < 8; i++)
        {
            c.Responder("O que é um record?", true, D(dia));
            dia += 20;
        }

        Assert.Equal(5, c.Todos.Single(x => x.Frente == "O que é um record?").Caixa);
        Assert.Equal(16, CaixaDeLeitner.Intervalos[^1]);
    }

    [Fact]
    public void Devidos_mostram_primeiro_os_mais_frageis()
    {
        var c = Caixa();
        c.Responder("O que é um record?", true, D(1));                  // sobe para a caixa 2
        var ordem = c.Devidos(D(10)).Select(x => x.Caixa).ToList();

        Assert.Equal(new[] { 1, 1, 2 }, ordem);
    }

    [Fact]
    public void Mesmo_sorteio_mesma_sessao_e_e_so_uma_reordenacao()
    {
        var c = Caixa();

        var a = c.SessaoDoDia(D(1), new Random(7)).Select(x => x.Frente).ToList();
        var b = c.SessaoDoDia(D(1), new Random(7)).Select(x => x.Frente).ToList();

        Assert.Equal(a, b);
        Assert.Equal(c.Devidos(D(1)).Select(x => x.Frente).OrderBy(x => x), a.OrderBy(x => x));
    }

    [Fact]
    public void Contagem_por_caixa_inclui_as_caixas_vazias()
    {
        var c = Caixa();
        c.Responder("O que é um record?", true, D(1));

        var porCaixa = c.PorCaixa();

        Assert.Equal(2, porCaixa[1]);
        Assert.Equal(1, porCaixa[2]);
        Assert.Equal(0, porCaixa[5]);
        Assert.Equal(5, porCaixa.Count);
    }

    [Fact]
    public void Responder_um_cartao_que_nao_existe_e_um_erro_claro() =>
        Assert.Throws<KeyNotFoundException>(() => Caixa().Responder("inexistente", true, D(1)));
}
```

Note como `Mesmo_sorteio_mesma_sessao_e_e_so_uma_reordenacao` conta duas coisas: o **mesmo sorteio** dá a **mesma sessão** (por isso o `Random(7)`), e a sessão tem **os mesmos cartões**, só em outra ordem. Sem injetar o `Random`, esse teste seria **instável**.

::: exercicio Quando é a próxima revisão?
Escreva dois métodos de extensão para `CaixaDeLeitner`: `ProximaRevisao()`, que devolve o **menor** dia de próxima revisão entre os cartões (ou `null` se não houver cartões), e `Dominados()`, que devolve os cartões **já na última caixa**.

--- resposta
```cs Jornada/Habitos/Exercicios/Cap113Proxima.cs
namespace Jornada.Habitos.Cap113;

public static class CaixaExtensoes
{
    /// <summary>O dia da próxima revisão de qualquer cartão. Sem cartões, não há próxima revisão (null).</summary>
    public static DateOnly? ProximaRevisao(this CaixaDeLeitner caixa) =>
        caixa.Todos.Count == 0 ? null : caixa.Todos.Min(c => c.Proxima);

    /// <summary>Os cartões já na última caixa: o que a mão já sabe de cor.</summary>
    public static IEnumerable<Cartao> Dominados(this CaixaDeLeitner caixa) =>
        caixa.Todos.Where(c => c.Caixa == CaixaDeLeitner.Intervalos.Count);
}
```

`DateOnly?` (um `DateOnly` que pode ser nulo) diz honestamente "sem cartões, sem próxima revisão". E `Min(c => c.Proxima)` com uma lista vazia **lançaria** exceção, por isso a checagem de `Count == 0` vem **antes**. `Dominados` reaproveita a **mesma constante** (`Intervalos.Count`), e assim, se amanhã o sistema passar a ter seis caixas, o exercício continua certo sem mudar.
:::

::: resumo
- Para o autor, o saber que **forma** é **incorporado** ("de cor"): vem de viver em ritmos, tradições e histórias, e não só de receber informações; **crianças amam tradições**, e a repetição é âncora.
- Aprender é **imitar e praticar**, e o conhecimento que muda a conduta é o que foi **praticado**.
- A **repetição espaçada** (caixa de Leitner) revê logo o que está frágil e, cada vez mais tarde, o que já está firme.
- **`record` + `with`** modelam o cartão imutável; **`Dictionary` por `Enumerable.Range`** mostra também as caixas vazias.
- **Injetar** o `Random` torna testável o que é aleatório: mesma semente, mesma sequência.
:::

::: quiz
? O que significa "aprender de cor", neste capítulo?
+ Ter um conhecimento incorporado, que a prática gravou, mesmo sem saber explicá-lo
- Decorar sem entender
- Copiar em voz alta
- Ler muitas vezes
> É o saber "nos ossos", adquirido por viver e praticar.

? O que o autor critica no modelo "bancário" de aprendizado?
+ Tratar quem aprende como recipiente de informações, sem formar hábitos nem imaginação
- Usar livros
- Ensinar crianças
- Ter professores
> A formação vai além de depositar conteúdo.

? Na caixa de Leitner, o que acontece quando você erra um cartão?
+ Ele volta para a caixa 1 e é revisto no dia seguinte
- Ele é descartado
- Ele sobe uma caixa
- Ele espera 16 dias
> O que está frágil volta logo.

? Por que `SessaoDoDia` recebe um `Random` como parâmetro?
+ Para o teste controlar o sorteio e obter sempre a mesma ordem
- Porque o C# proíbe criar `Random` lá dentro
- Para deixar o código mais rápido
- Para esconder as respostas
> Injetar a dependência torna o resultado previsível nos testes.

? Por que `PorCaixa` usa `Enumerable.Range(1, 5)`?
+ Para que as caixas vazias também apareçam, com zero cartões
- Para ordenar os cartões
- Para sortear caixas
- Para criar novas caixas
> Um `GroupBy` sozinho só mostraria as caixas que têm cartões.

? Qual tipo de cartão treina melhor o "saber de cor"?
+ O que pede para fazer algo na prática, e não só reconhecer uma resposta
- O que tem a resposta mais longa
- O que usa mais cores
- O que é o mais antigo
> O saber de cor é de mão, e não só de cabeça.
:::

::: cartas
Aprender de cor :: Conhecer de modo incorporado, adquirido por viver e praticar.
Formação × informação :: Formar toca hábitos e imaginação; informar só deposita dados.
Repetição espaçada :: Rever em intervalos crescentes: logo o que é frágil, tarde o que é firme.
Caixa de Leitner :: Técnica com caixas numeradas e intervalos de revisão crescentes.
record + with :: Valor imutável que "muda" criando uma cópia alterada.
Injeção de Random :: Receber o gerador de números aleatórios de fora, para testar.
DateOnly? :: Data que pode não existir (null).
Tradição (para o autor) :: Ritmo repetido que ancora e forma, especialmente na infância.
:::
