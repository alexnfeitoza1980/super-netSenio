---
id: joao-cap126
order: 164
kind: capitulo
num: 126
group: C# & Mentalidade · Parte 18 · O Evangelho segundo João em C#
title: Da água ao vinho
subtitle: As bodas de Caná, em C#: seis vasos, "Enchei estes vasos com água", "Agora tirai", o mestre de cerimônia que prova. E o que o texto diz, e não diz, sobre o momento da mudança.
minutes: 65
hue: 216
objectives:
- Ler as **bodas de Caná** (Jo 2,1-11) frase por frase
- Transformar cada ordem do texto em um **método** (`Encher`, `Tirar`, `Provar`)
- Separar o que o **texto** diz do que é **escolha de implementação**
- Entender a **execução adiada**: o trabalho só acontece quando alguém pede
origem: livros/10-evangelho-joao-csharp/content/103-cap126.md
---

::: vila A festa que não esperava ninguém provar
Beto montou um relatório que lia 200 mil linhas, convertia cada uma e só então mostrava as 10 primeiras na tela. Demorava dois minutos. Lia olhou o código e riu: "você **transforma tudo antes de saber o que vai usar**". Bastou trocar a lista pronta por uma receita que só trabalha **quando alguém pede o próximo item**: a tela abriu em meio segundo.
:::

::: ilustracao img/joao/cap126.svg | "Enchei estes vasos com água... Agora tirai": em código, o método Tirar. O texto não diz quando a água virou vinho; mudar em Tirar é uma escolha de implementação.
:::

## O texto

> **1** E no terceiro dia houve um casamento em Caná da Galileia; e a mãe de Jesus estava ali. **2** E também Jesus foi convidado com seus discípulos ao casamento. **3** E tendo faltado vinho, a mãe de Jesus lhe disse: Não têm vinho. **4** Jesus lhe disse: O que eu tenho contigo, mulher? A minha hora ainda não chegou. **5** Sua mãe disse aos serventes: Fazei tudo quanto ele vos disser. **6** E estavam ali postos seis vasos de pedra, conforme à purificação dos judeus, em cada uma cabiam duas ou três metretas. **7** Disse-lhes Jesus: Enchei estes vasos com água. E encheram-nas até encima. **8** E disse-lhes: Agora tirai, e a levai ao mestre de cerimônia. Então levaram. **9** E quando o mestre de cerimônia experimentou a água feita vinho (sem saber de onde era, porém os serventes que haviam tirado a água sabiam), o mestre de cerimônia chamou o noivo, **10** E disse-lhe: Todos põem primeiro o vinho bom, e quando os convidados já estão bêbados, então se dá o pior; porém tu guardaste o bom vinho até agora. **11** Este princípio de sinais Jesus fez em Caná da Galileia, e manifestou sua glória; e seus discípulos creram nele.
>
> *João 2,1-11 (Bíblia Livre)*

## Lendo devagar

- **Há uma festa de casamento, e o vinho acaba** (2,1-3). A mãe de Jesus avisa: "Não têm vinho".
- **Havia seis vasos de pedra** (2,6), usados para a purificação dos judeus.
- **"Enchei estes vasos com água"** (2,7). Os serventes enchem os vasos até em cima.
- **"Agora tirai, e a levai ao mestre de cerimônia"** (2,8). Eles tiram e levam.
- **O mestre de cerimônia experimenta "a água feita vinho"** (2,9). Ele **não sabia de onde era**, mas os serventes que haviam tirado a água sabiam.
- **"Tu guardaste o bom vinho até agora"** (2,10): o mestre elogia o noivo, porque o melhor veio por último.
- **"Este princípio de sinais Jesus fez em Caná da Galileia"** (2,11): é o **primeiro** sinal que João registra.

Olhe a ordem: primeiro **enchem**, depois **tiram**, e só então **provam**. É uma sequência de **ordens**, e cada ordem vira um método.

::: dica O que o texto diz, e o que não diz
O texto afirma que a água dos vasos foi **tirada** e que, quando o mestre de cerimônia a provou, ela era **"água feita vinho"**. Ele **não diz em que momento** a mudança aconteceu: só que, na hora da prova, já era vinho. Qualquer código que escolha um momento está **decidindo por conta própria**. Faremos isso mais adiante, e diremos qual foi a nossa escolha.
:::

## Do versículo ao código

### Passo 1: "seis vasos de pedra"

Seis vasos iguais são seis **objetos** do mesmo molde. Em C#, o molde é uma classe, e os seis ficam em uma **lista**:

```cs
var vasos = Enumerable.Range(1, 6).Select(_ => new Vaso()).ToList();   // "seis vasos de pedra"
```

### Passo 2: "Enchei estes vasos com água"

É uma **ordem**: "enchei". Cada ordem do texto vira um **método** do vaso, com o mesmo nome do verbo:

```cs
public void Encher(Bebida agua)          // "Enchei estes vasos com água"
{
    if (agua.Nome != "água")
        throw new ArgumentException("Este vaso é enchido com água.", nameof(agua));

    conteudo = agua;
}
```

O `if` faz o vaso **recusar** o que não é água (lança uma exceção), como o texto pede.

### Passo 3: "Agora tirai"

Outra ordem, outro método: `Tirar`. Aqui entra a **nossa escolha**, e vale ler com atenção:

```cs
public Bebida Tirar()                    // "Agora tirai"
{
    if (conteudo is null)
        throw new InvalidOperationException("Não há o que tirar: o vaso está vazio.");

    conteudo = new Bebida("vinho", Qualidade: 9);   // ESCOLHA DE IMPLEMENTAÇÃO
    return conteudo;
}
```

A linha marcada **não é relato**: é a nossa decisão de que o que sai do vaso já é vinho. O texto só garante que, na prova, era "a água feita vinho". Escolhemos esse ponto porque ele nos permite mostrar uma coisa útil de C# (passo 5).

### Passo 4: "o mestre de cerimônia experimentou a água feita vinho"

O mestre **não sabia de onde era**: ele só vê o que tem no copo. Em código, `Provar` recebe a bebida e responde só pelo que ela é, sem conhecer a história:

```cs
public string Provar(Bebida bebida) => bebida switch
{
    { Nome: "vinho", Qualidade: > 5 } => "tu guardaste o bom vinho até agora",
    { Nome: "vinho" }                 => "vinho comum",
    _                                 => "só água",
};
```

O `switch` olha as propriedades da bebida: vinho de qualidade maior que 5 recebe o elogio de 2,10. O número 5 também é **escolha nossa**: o texto não dá notas.

### O código completo, rodando

Texto e implementação, lado a lado:

| O que o texto diz | O que o código faz |
|---|---|
| seis vasos de pedra (2,6) | lista de seis `Vaso` |
| "Enchei estes vasos com água" (2,7) | `Encher(água)` |
| "Agora tirai, e a levai ao mestre de cerimônia" (2,8) | `Tirar()` |
| o mestre experimenta "a água feita vinho" (2,9) | `Provar(bebida)` |
| *(o texto não diz quando a água virou vinho)* | **escolha nossa**: acontece em `Tirar` |

```cs Jornada/Joao/Texto/Cap126Cana.cs
namespace Jornada.Joao.Texto;

// Jo 2,6-10 (Bíblia Livre): "E estavam ali postos seis vasos de pedra [...] Enchei estes vasos com água. E encheram-nas até encima.
//  E disse-lhes: Agora tirai, e a levai ao mestre de cerimônia. [...] E quando o mestre de cerimônia experimentou a água feita vinho
//  (sem saber de onde era, porém os serventes que haviam tirado a água sabiam) [...] tu guardaste o bom vinho até agora."
//
// TEXTO     : os vasos são enchidos com água (2,7); o que se leva ao mestre de cerimônia é provado como "a água feita vinho" (2,9).
//             O texto NÃO diz em que momento a água se tornou vinho: só que, na prova, já era vinho.
// IMPLEMENTAÇÃO (escolha nossa, não relato): a mudança acontece no método Tirar, para que o exemplo possa mostrar uma
//             coisa útil de C#: o trabalho só é feito quando alguém pede.

public record Bebida(string Nome, int Qualidade);

// <vaso>
public class Vaso
{
    private Bebida? conteudo;

    public string Conteudo => conteudo?.Nome ?? "vazio";

    // "Enchei estes vasos com água"
    public void Encher(Bebida agua)
    {
        if (agua.Nome != "água")
            throw new ArgumentException("Este vaso é enchido com água.", nameof(agua));

        conteudo = agua;
    }

    // "Agora tirai": o que sai daqui já é "a água feita vinho". ESCOLHA DE IMPLEMENTAÇÃO: o texto não diz quando mudou.
    public Bebida Tirar()
    {
        if (conteudo is null)
            throw new InvalidOperationException("Não há o que tirar: o vaso está vazio.");

        conteudo = new Bebida("vinho", Qualidade: 9);
        return conteudo;
    }
}
// </vaso>

// <mestre>
public class MestreDeCerimonia
{
    private const int QualidadeDoVinhoDeAntes = 5;

    // "o mestre de cerimônia experimentou a água feita vinho": ele só vê o que tem no copo
    public string Provar(Bebida bebida) => bebida switch
    {
        { Nome: "vinho", Qualidade: > QualidadeDoVinhoDeAntes } => "tu guardaste o bom vinho até agora",
        { Nome: "vinho" }                                      => "vinho comum",
        _                                                      => "só água",
    };
}
// </mestre>
```

Para rodar:

```bash
dotnet run --project codigo/joao/Joao.Console -- exemplo t126
```

```cs Uso
var vasos = Enumerable.Range(1, 6).Select(_ => new Vaso()).ToList();

foreach (var vaso in vasos)
    vaso.Encher(new Bebida("água", Qualidade: 3));                      // "Enchei estes vasos com água"

Console.WriteLine("Depois de encher:          " + string.Join(", ", vasos.Select(v => v.Conteudo)));

var amostra = vasos[0].Tirar();                                           // "Agora tirai"
Console.WriteLine("O mestre de cerimônia diz: " + new MestreDeCerimonia().Provar(amostra));

Console.WriteLine("Depois de tirar do 1º:     " + string.Join(", ", vasos.Select(v => v.Conteudo)));
```

```saida
Depois de encher:          água, água, água, água, água, água
O mestre de cerimônia diz: tu guardaste o bom vinho até agora
Depois de tirar do 1º:     vinho, água, água, água, água, água
```

Na última linha, **só o vaso do qual se tirou** tem vinho; os outros cinco continuam com água. Isso **decorre da nossa escolha** (a mudança acontece em `Tirar`), e **não** do texto, que não diz nada sobre os outros vasos nem sobre quando a mudança ocorreu.

### Passo 5: o que a nossa escolha ensina

Por termos escolhido `Tirar`, o programa **só faz o trabalho de um vaso no momento em que alguém pede**. Os outros ficam como estão, sem gastar nada. Esse jeito de trabalhar (**fazer só quando pedirem**) chama-se **execução adiada**, e C# o usa o tempo todo (por exemplo, no LINQ). As próximas seções mostram isso com o recurso `yield return`.

::: cuidado Limite da analogia
- **O código não modela o sinal.** A transformação da água em vinho, que é o centro do relato, vira uma linha (`new Bebida("vinho", ...)`). Ele só dá forma à **sequência de ordens** (encher, tirar, provar).
- **A lição da execução adiada é consequência de uma escolha nossa**, e não do que o texto diz.
- **O mestre de cerimônia** só ilustra "quem prova vê o resultado sem saber a origem". Não há, no código, nenhum papel para os serventes (que sabiam) além de `Encher` e `Tirar`.
- O texto fala de **"sinais"** e de **glória** (2,11): isso não é objeto do código.
:::

::: curiosidade Lupa no grego
- **σημεῖον** (*sēmeion*, "sinal") aparece **17 vezes** em João, em **10 capítulos** (`onde sēmeion`). João numera **só dois**: em 2,11 (*archēn tōn sēmeiōn*, "o princípio dos sinais") e em 4,54 (*deuteron sēmeion*, "segundo sinal"). Uma lista de sete sinais **não está no texto**.
- **ὑδρία** (*hydria*, "jarro de água", G5201) aparece **3 vezes**: em 2,6 e 2,7 (os vasos de Caná) e em 4,28, quando a mulher samaritana **deixa o seu jarro** (`onde hydria`).
- **ἀντλέω** (*antleō*, "tirar água", G501) aparece **4 vezes**: 2,8 e 2,9 (Caná) e 4,7 e 4,15 (o poço) (`onde antleō`).
- **οἶνος** (*oinos*, "vinho") aparece **6 vezes**, **5** em Jo 2 (`onde oinos`); **ἀρχιτρίκλινος** (*architriklinos*, o "mestre de cerimônia") aparece **3 vezes**, todas em 2,8-9 (`onde architriklinos`).
- Em 2,10, o mestre diz *tetērēkas ton kalon oinon heōs arti*: "guardaste o bom vinho até agora" (SBLGNT).

*Fontes: texto grego SBLGNT (CC BY 4.0); contagens por lema do MorphGNT (CC BY-SA 3.0), reproduzíveis com `dotnet run --project codigo/joao/Joao.Console -- onde <palavra>`; sentidos do dicionário de Strong (OpenScriptures), com a entrada indicada.*
:::


## Indo além: execução adiada com `yield return`

A cena inspira uma ideia útil para programas grandes: se você tem **seis mil** vasos e só vai provar dois, **não precisa transformar os seis mil**. As seções abaixo generalizam o passo 3: uma "transformação" que serve para qualquer coisa que entra e sai, e um jeito de servir um item de cada vez.

::: figura Receita ou resultado?
<svg viewBox="0 0 740 230" role="img" aria-label="Duas formas de trabalhar: ansiosa, que transforma as seis vasos antes de servir, e adiada, que transforma só as que forem provadas">
  <text x="190" y="26" text-anchor="middle" font-weight="800">Ansiosa: tudo antes</text>
  <g class="f-bad">
    <rect x="30" y="44" width="48" height="48" rx="8"/><rect x="86" y="44" width="48" height="48" rx="8"/><rect x="142" y="44" width="48" height="48" rx="8"/>
    <rect x="198" y="44" width="48" height="48" rx="8"/><rect x="254" y="44" width="48" height="48" rx="8"/><rect x="310" y="44" width="48" height="48" rx="8"/>
  </g>
  <text x="190" y="120" text-anchor="middle" font-size="13">6 transformações, mesmo que só 2 taças sejam provadas</text>
  <text x="550" y="26" text-anchor="middle" font-weight="800">Adiada: só o que se pede</text>
  <g>
    <rect x="400" y="44" width="48" height="48" rx="8" class="f-good"/><rect x="456" y="44" width="48" height="48" rx="8" class="f-good"/>
    <rect x="512" y="44" width="48" height="48" rx="8" class="f-info" opacity=".4"/><rect x="568" y="44" width="48" height="48" rx="8" class="f-info" opacity=".4"/>
    <rect x="624" y="44" width="48" height="48" rx="8" class="f-info" opacity=".4"/><rect x="680" y="44" width="48" height="48" rx="8" class="f-info" opacity=".4"/>
  </g>
  <text x="560" y="120" text-anchor="middle" font-size="13">2 transformações; os outros 4 vasos continuam água</text>
  <rect x="30" y="150" width="680" height="56" rx="12" class="f-warn"/>
  <text x="370" y="174" text-anchor="middle" font-size="13" font-weight="700">Cuidado: provar duas vezes a mesma receita transforma duas vezes.</text>
  <text x="370" y="194" text-anchor="middle" font-size="12" class="t-dim">Quando o resultado será usado várias vezes, guarde-o com ToList().</text>
</svg>
:::

### 1) Uma classe com dois tipos

Você já usou `List<int>`: uma classe **genérica** com **um** parâmetro de tipo. Aqui precisamos de **dois**: o que **entra** e o que **sai**. Por isso a classe se chama `Transformacao<TEntrada, TSaida>`. Ela guarda uma função `Func<TEntrada, TSaida>` e oferece:

- `Aplicar(entrada)`: usa a função uma vez;
- `Depois<TNova>(proxima)`: **cria uma nova transformação** em que a saída desta vira a entrada da `proxima`. O tipo final **muda** de `TSaida` para `TNova`: uma transformação de `string` para `int` seguida de uma de `int` para `string` resulta em `string` para `string`;
- `AplicarA(entradas)`: aplica a vários itens com `yield return`.

O `Banquete` monta a receita da cena: duas etapas (**muda o nome** para "vinho" e **sobe a qualidade em 4**). Repare no `with`: ele **cria uma cópia** do `record` trocando só o campo indicado, sem modificar o original.

```cs Jornada/Joao/Cap126/Vasos.cs
namespace Jornada.Joao.Cap126;

public sealed record Liquido(string Nome, double Qualidade);

/// <summary>
/// Uma transformação de TEntrada em TSaida que se pode encadear com Depois().
/// Dois parâmetros de tipo: o que entra e o que sai (por isso "genérica").
/// </summary>
public sealed class Transformacao<TEntrada, TSaida>
{
    private readonly Func<TEntrada, TSaida> funcao;

    public Transformacao(Func<TEntrada, TSaida> funcao) => this.funcao = funcao;

    public TSaida Aplicar(TEntrada entrada) => funcao(entrada);

    /// <summary>Junta esta transformação com outra: a saída de uma vira a entrada da próxima.</summary>
    public Transformacao<TEntrada, TNova> Depois<TNova>(Func<TSaida, TNova> proxima) =>
        new(entrada => proxima(funcao(entrada)));

    /// <summary>Aplica a vários itens, um de cada vez e só quando alguém pede: execução adiada.</summary>
    public IEnumerable<TSaida> AplicarA(IEnumerable<TEntrada> entradas)
    {
        foreach (var entrada in entradas)
            yield return funcao(entrada);
    }
}

public sealed record Taca(int Vaso, Liquido Conteudo);

public sealed class Banquete
{
    private int transformadas;

    /// <summary>Quantos vasos já foram realmente transformados. Mostra que o trabalho só acontece quando se prova.</summary>
    public int Transformadas => transformadas;

    public Transformacao<Liquido, Liquido> DaAguaAoVinho { get; } =
        new Transformacao<Liquido, Liquido>(agua => agua with { Nome = "vinho" })
            .Depois(vinho => vinho with { Qualidade = vinho.Qualidade + 4 });   // quanto mais adiante, melhor

    /// <summary>Serve os vasos na ordem. Cada um só é transformado quando a taça é pedida.</summary>
    public IEnumerable<Taca> Servir(int vasos)
    {
        for (int i = 1; i <= vasos; i++)
        {
            var agua = new Liquido("água", i);
            transformadas++;
            yield return new Taca(i, DaAguaAoVinho.Aplicar(agua));
        }
    }
}

public static class Provador
{
    /// <summary>O mestre de cerimônia só enxerga o que está na taça: não sabe de onde veio.</summary>
    public static string Avaliar(Liquido liquido) => (liquido.Nome, liquido.Qualidade) switch
    {
        ("vinho", >= 9) => "o melhor ficou para o fim",
        ("vinho", >= 7) => "vinho muito bom",
        ("vinho", _)    => "vinho",
        _               => "só água"
    };
}
```

O `Provador` usa uma **expressão `switch` com padrões** sobre a tupla `(Nome, Qualidade)`: `("vinho", >= 9)` combina o nome com uma comparação, o que deixa a regra legível. Ele **só enxerga o líquido**, assim como o mestre de cerimônia.

### 2) Execução adiada: o poder do `yield return`

Um método com `yield return` **não executa o corpo ao ser chamado**. Ele devolve um objeto que, **a cada pedido do próximo item** (um `foreach`, um `First()`, um `Take(2)`), roda **até o próximo `yield`** e para. Por isso, `Servir(6)` não transforma nada: devolve uma **receita**.

```cs Uso
var banquete = new Banquete();
var vasos = banquete.Servir(6);                                         // só uma "receita"
Console.WriteLine($"Vasos transformados até agora: {banquete.Transformadas}");

foreach (var taca in vasos)
    Console.WriteLine($"vaso {taca.Vaso}: {taca.Conteudo.Nome}, qualidade {taca.Conteudo.Qualidade} → {Provador.Avaliar(taca.Conteudo)}");

Console.WriteLine($"Transformadas depois de provar tudo: {banquete.Transformadas}");

var apenasDuas = new Banquete();
_ = apenasDuas.Servir(6).Take(2).ToList();
Console.WriteLine($"Quem prova só duas taças causa {apenasDuas.Transformadas} transformações");

var palavraParaTamanho = new Transformacao<string, int>(texto => texto.Length)
    .Depois(n => n * 2)
    .Depois(n => $"dobro do tamanho: {n}");
Console.WriteLine(palavraParaTamanho.Aplicar("caná"));
```

```saida
Vasos transformados até agora: 0
vaso 1: vinho, qualidade 5 → vinho
vaso 2: vinho, qualidade 6 → vinho
vaso 3: vinho, qualidade 7 → vinho muito bom
vaso 4: vinho, qualidade 8 → vinho muito bom
vaso 5: vinho, qualidade 9 → o melhor ficou para o fim
vaso 6: vinho, qualidade 10 → o melhor ficou para o fim
Transformadas depois de provar tudo: 6
Quem prova só duas taças causa 2 transformações
dobro do tamanho: 8
```

Os números contam a história: **zero** transformações antes de provar; **seis** depois de provar tudo; e **só duas** quando se prova duas taças. Veja a ordem no corpo de `Servir`: o contador `transformadas++` está **dentro** do laço, junto do `yield`, e é por isso que ele acompanha os pedidos.

::: erro A armadilha da enumeração dupla
Se você percorrer o mesmo `IEnumerable` produzido por `yield` **duas vezes**, o trabalho acontece **duas vezes**. Para um relatório pesado, isso é um desastre silencioso. Quando precisar do resultado várias vezes, **materialize**: `var lista = banquete.Servir(6).ToList();`. A regra prática: **execução adiada é ótima para consumir uma vez; lista é melhor para consumir várias**.
:::

::: dica LINQ também é adiado
`Where`, `Select`, `Take`, `OrderBy`... todos devolvem receitas. Nada roda até haver um `foreach`, `ToList()`, `Count()`, `First()` etc. É por isso que `Servir(6).Take(2)` só paga por duas taças.
:::

::: cuidado Limite do modelo ampliado
- O `Banquete` é um modelo **inteiramente nosso**: o texto **não** diz que os vasos tivessem vinhos de qualidades diferentes. A qualidade que sobe de vaso em vaso foi inventada para o exemplo funcionar.
- As frases do `Provador` ("o melhor ficou para o fim") são **resumos nossos**; a fala do texto é "tu guardaste o bom vinho até agora" (2,10).
:::

### 3) Testes

```cs Joao.Testes/Cap126Tests.cs
using Jornada.Joao.Cap126;

namespace Joao.Testes;

public class Cap126Tests
{
    [Fact]
    public void Transformacoes_se_encadeiam_e_podem_mudar_de_tipo()
    {
        var contarLetras = new Transformacao<string, int>(texto => texto.Length)
            .Depois(n => n * 2)
            .Depois(n => $"dobro: {n}");

        Assert.Equal("dobro: 10", contarLetras.Aplicar("água!"));
    }

    [Fact]
    public void Da_agua_ao_vinho_muda_o_nome_e_melhora_a_qualidade()
    {
        var vinho = new Banquete().DaAguaAoVinho.Aplicar(new Liquido("água", 1));

        Assert.Equal("vinho", vinho.Nome);
        Assert.Equal(5, vinho.Qualidade);
    }

    [Fact]
    public void Nada_e_transformado_ate_que_alguem_prove()
    {
        var banquete = new Banquete();

        var taças = banquete.Servir(6);                    // só uma "receita": nada aconteceu ainda

        Assert.Equal(0, banquete.Transformadas);
        _ = taças.First();
        Assert.Equal(1, banquete.Transformadas);
    }

    [Fact]
    public void Quem_prova_so_as_duas_primeiras_causa_so_duas_transformacoes()
    {
        var banquete = new Banquete();

        var provadas = banquete.Servir(6).Take(2).ToList();

        Assert.Equal(2, provadas.Count);
        Assert.Equal(2, banquete.Transformadas);
    }

    [Fact]
    public void Servir_tudo_transforma_as_seis_vasos_e_o_melhor_fica_para_o_fim()
    {
        var banquete = new Banquete();

        var todas = banquete.Servir(6).ToList();

        Assert.Equal(6, banquete.Transformadas);
        Assert.Equal(todas.Max(t => t.Conteudo.Qualidade), todas[^1].Conteudo.Qualidade);
        Assert.Equal("o melhor ficou para o fim", Provador.Avaliar(todas[^1].Conteudo));
    }

    [Theory]
    [InlineData("água", 3, "só água")]
    [InlineData("vinho", 5, "vinho")]
    [InlineData("vinho", 7, "vinho muito bom")]
    [InlineData("vinho", 10, "o melhor ficou para o fim")]
    public void O_provador_avalia_so_o_que_esta_na_taca(string nome, double qualidade, string esperado) =>
        Assert.Equal(esperado, Provador.Avaliar(new Liquido(nome, qualidade)));

    [Fact]
    public void AplicarA_tambem_e_adiado()
    {
        int chamadas = 0;
        var t = new Transformacao<int, int>(x => { chamadas++; return x * x; });

        var quadrados = t.AplicarA(new[] { 1, 2, 3, 4 });

        Assert.Equal(0, chamadas);
        Assert.Equal(new[] { 1, 4 }, quadrados.Take(2));
        Assert.Equal(2, chamadas);
    }
}
```

O primeiro teste prova a **mudança de tipo** no encadeamento (`string → int → string`). Os dois seguintes são o coração do capítulo: **`Nada_e_transformado_ate_que_alguem_prove`** e **`Quem_prova_so_as_duas_primeiras_causa_so_duas_transformacoes`** medem o adiamento com um contador. O `[Theory]` testa o provador com quatro entradas.

::: exercicio Repetir uma transformação
Escreva o método de extensão `Repetir<T>(this Transformacao<T, T> t, int vezes)`, que devolve uma transformação equivalente a aplicar `t` **várias vezes em sequência** (a saída de uma volta entra na próxima). Com `vezes < 0`, lance `ArgumentOutOfRangeException`; com `0`, a transformação "não faz nada".

--- resposta
```cs Jornada/Joao/Exercicios/Cap126Repetir.cs
using Jornada.Joao.Cap126;

namespace Jornada.Joao.Exercicios;

public static class TransformacaoExtensoes
{
    /// <summary>Aplica a mesma transformação várias vezes em sequência (a saída de uma volta é a entrada da próxima).</summary>
    public static Transformacao<T, T> Repetir<T>(this Transformacao<T, T> transformacao, int vezes)
    {
        if (vezes < 0) throw new ArgumentOutOfRangeException(nameof(vezes));

        var resultado = new Transformacao<T, T>(x => x);              // começa "sem fazer nada"
        for (int i = 0; i < vezes; i++)
            resultado = resultado.Depois(transformacao.Aplicar);      // método passado direto, sem lambda

        return resultado;
    }
}
```

Começamos com a transformação **identidade** (`x => x`) e acrescentamos `Depois(t.Aplicar)` `vezes` vezes. `t.Aplicar` é passado como **grupo de métodos**, sem lambda. Só é possível porque a entrada e a saída têm o **mesmo tipo `T`**: o `where` implícito está na assinatura `Transformacao<T, T>`.
:::

::: resumo
- Cada ordem do texto virou um **método**: `Encher` ("Enchei estes vasos com água"), `Tirar` ("Agora tirai") e `Provar` (o mestre de cerimônia).
- O texto **não diz quando** a água virou vinho (só que, na prova, já era). Escolhemos fazer a mudança em `Tirar`: é **escolha de implementação**, e dela vem a lição da **execução adiada**.
- Em João, **sēmeion** aparece **17 vezes** em 10 capítulos, mas só **dois** são numerados (2,11 e 4,54): uma lista de **sete sinais** não está no texto.
- O mesmo **jarro de água** (*hydria*) aparece em Caná (2,6-7) e no poço (4,28); o verbo **tirar água** (*antleō*) aparece nas duas cenas.
- Uma classe com **dois parâmetros de tipo** descreve algo que **muda de tipo**; `Depois` encadeia etapas.
- `yield return` **adia o trabalho**: o corpo só roda quando alguém pede o próximo item.
- Percorrer duas vezes uma sequência adiada **repete o trabalho**; use `ToList()` quando precisar reutilizar.
:::

::: quiz
? Em qual método do código a água do vaso vira vinho, e de quem é essa decisão?
+ Em `Tirar`; é escolha nossa, porque o texto não diz quando a água se tornou vinho
- Em `Encher`; o texto diz isso em 2,7
- Em `Provar`; o texto diz isso em 2,9
- No construtor, por exigência do C#
> O texto garante só que, na prova, era "água feita vinho" (2,9).

? O que o mestre de cerimônia sabe sobre a origem da bebida?
+ Nada: só vê o que tem no copo (`Provar` recebe só a `Bebida`)
- Tudo
- Só o nome do vaso
- Só a quantidade
> Em 2,9, só os serventes sabiam de onde era.

? Quantos "sinais" João numera explicitamente?
+ Dois: o primeiro (2,11) e o segundo (4,54)
- Sete
- Doze
- Nenhum
> Uma lista de sete sinais não está no texto de João.

? Quantas vezes aparece *sēmeion* ("sinal") em João?
+ 17 vezes, em 10 capítulos
- 7 vezes
- 70 vezes
- Só em Caná
> Contagem feita sobre o texto grego SBLGNT/MorphGNT.

? Por que `Transformacao` tem dois parâmetros de tipo?
+ Um para o que entra e outro para o que sai, pois o tipo pode mudar
- Para guardar duas funções
- Por exigência do C#
- Para ser mais rápida
> `Transformacao<string, int>` converte texto em número.

? O que acontece quando você chama `banquete.Servir(6)` e não usa o resultado?
+ Nada é transformado: devolve só uma "receita" de execução adiada
- As seis vasos são transformadas
- Lança exceção
- O primeiro vaso é transformado
> O corpo do método só roda quando se pede o próximo item.

? Como evitar que a sequência adiada seja recalculada a cada uso?
+ Materializando com `ToList()` quando for reutilizar
- Usando `yield` duas vezes
- Trocando `List` por `Array`
- Não há como
> A enumeração dupla repete o trabalho.

? O que faz o `with` em `agua with { Nome = "vinho" }`?
+ Cria uma cópia do record trocando só o campo indicado
- Altera o record original
- Apaga o campo
- Compara dois records
> Os records são imutáveis; o `with` é a forma de "alterar" criando outro.
:::

::: cartas
Agora tirai (Jo 2,8) :: Método `Tirar`. O texto não diz quando a água virou vinho; no código, é escolha nossa.
Caná (Jo 2) :: Cena em que o mestre de cerimônia prova "a água feita vinho"; João a chama "princípio de sinais" (2,11).
Sēmeion :: "Sinal": 17 vezes em João; só dois são numerados (2,11 e 4,54).
Hydria :: "Jarro de água": 2,6-7 (Caná) e 4,28 (poço).
Genérico de dois tipos :: Classe como `Transformacao<TEntrada, TSaida>`, em que o tipo pode mudar.
Func<A,B> :: Tipo de uma função que recebe A e devolve B.
yield return :: Devolve um item e pausa o método até o próximo ser pedido.
Execução adiada :: Trabalho só feito quando alguém consome o resultado.
ToList() :: Materializa a sequência: faz o trabalho agora e guarda o resultado.
:::
