---
id: joao-cap125
order: 163
kind: capitulo
num: 125
group: C# & Mentalidade · Parte 18 · O Evangelho segundo João em C#
title: "Eu sou": as sete imagens
subtitle: Pão, luz, porta, pastor, ressurreição e vida, caminho, videira. Cada "Eu sou" vem com uma promessa, e cada um vira, em C#, uma declaração que o programa guarda, etiqueta e encontra sozinho.
minutes: 70
hue: 218
objectives:
- Ler os **"Eu sou" com imagem** de João e perceber o padrão: *uma imagem* + *uma promessa*
- Guardar cada declaração em um **`record`** e listar todas
- Colocar uma **etiqueta** (atributo) em cada classe e fazer o programa **descobri-las sozinho** (reflexão)
- Saber o que a lista de "sete" é (e o que não é)
origem: livros/10-evangelho-joao-csharp/content/102-cap125.md
---

::: vila O catálogo que se montou sozinho
A equipe tinha 30 serviços e uma planilha de "quem faz o quê" que ninguém atualizava. Beto propôs: "e se **cada serviço dissesse** o que é, e o programa **lesse isso**, em vez de a gente escrever uma lista à mão?". Bastou etiquetar cada classe. Na manhã seguinte, o catálogo estava pronto, sempre **igual ao código**, e Lia notou que dois serviços descreviam o mesmo papel: a planilha nunca tinha mostrado isso.
:::

::: ilustracao img/joao/cap125.svg | Sete tipos etiquetados e um sem etiqueta: a reflexão lê as etiquetas e ignora o oitavo. A lupa percorre o assembly em ordem.
:::

## O texto

João registra frases que começam com **"Eu sou"** e continuam com uma imagem do dia a dia. Estas são as mais conhecidas:

> - **Jo 6,35** E Jesus lhes disse: Eu sou o pão da vida; quem vem a mim de maneira nenhuma terá fome, e quem crê em mim nunca terá sede.
> - **Jo 8,12** Falou-lhes pois Jesus outra vez, dizendo: Eu sou a luz do mundo; quem me seguir não andará em trevas, mas terá luz de vida.
> - **Jo 10,9** Eu sou a porta; se alguém entrar por mim, será salvo; e entrará, e sairá, e achará pasto.
> - **Jo 10,11** Eu sou o bom Pastor; o bom Pastor dá sua vida pelas ovelhas.
> - **Jo 11,25** Disse-lhe Jesus: Eu sou a ressurreição, e a vida; quem crê em mim, ainda que esteja morto, viverá.
> - **Jo 14,6** Jesus lhe disse: Eu sou o caminho, e a verdade, e a vida. Ninguém vem ao Pai, a não ser por mim.
> - **Jo 15,5** Eu sou a videira, vós sois os ramos; quem está em mim, e eu nele, esse dá muito fruto; porque sem mim nada podeis fazer.
>
> *Bíblia Livre (CC BY 4.0)*

## Lendo devagar

Todas seguem o **mesmo molde**: "**Eu sou** *uma imagem*; **quem** faz tal coisa, **recebe** tal promessa".

| Imagem | Onde | A promessa, nas palavras do texto |
|---|---|---|
| o pão da vida | Jo 6,35 | quem vem a mim de maneira nenhuma **terá fome** |
| a luz do mundo | Jo 8,12 | quem me seguir **não andará em trevas** |
| a porta | Jo 10,9 | quem entrar por mim **será salvo**, entrará, sairá e achará pasto |
| o bom Pastor | Jo 10,11 | **dá sua vida** pelas ovelhas |
| a ressurreição, e a vida | Jo 11,25 | quem crê em mim, ainda que esteja morto, **viverá** |
| o caminho, e a verdade, e a vida | Jo 14,6 | **ninguém vem ao Pai**, a não ser por mim |
| a videira | Jo 15,5 | quem está em mim, e eu nele, **dá muito fruto** |

::: dica A lista de "sete" não é de João
O texto **não numera** essas imagens: o agrupamento em "sete" **não vem do texto**. Além disso, a expressão grega *egō eimi* ("eu sou") aparece **24 vezes** em João (contagem no texto grego, reproduzível no Laboratório 14), e nem todas têm imagem: em algumas, como 8,58 ou 18,5, o "Eu sou" vem **sem** predicado. **Este capítulo modela só as declarações com imagem.**
:::

## Do versículo ao código

### Passo 1: uma declaração tem três partes

Pegue Jo 6,35: "Eu sou **o pão da vida**; quem vem a mim de maneira nenhuma **terá fome**...". Há três informações: **onde** está escrito (Jo 6,35), **o que é dito** (o pão da vida) e **o que se promete**. Em C#, um `record` é o jeito mais curto de guardar um pequeno conjunto de dados assim:

```cs
public record Declaracao(string Passagem, string EuSou, string Promessa);

var pao = new Declaracao("Jo 6,35", "o pão da vida", "quem vem a mim de maneira nenhuma terá fome, e quem crê em mim nunca terá sede");
```

### Passo 2: as sete, em uma lista

Com o molde pronto, as declarações viram uma lista, e um método junta as três partes em uma frase:

```cs
public static string Frase(this Declaracao d) => $"{d.Passagem}: Eu sou {d.EuSou}; {d.Promessa}.";
```

Para rodar (arquivo `Cap125Declaracoes.cs`):

```bash
dotnet run --project codigo/joao/Joao.Console -- exemplo t125
```

```cs Uso
foreach (var declaracao in Declaracoes.Todas)
    Console.WriteLine(declaracao.Frase());
```

```saida
Jo 6,35: Eu sou o pão da vida; quem vem a mim de maneira nenhuma terá fome, e quem crê em mim nunca terá sede.
Jo 8,12: Eu sou a luz do mundo; quem me seguir não andará em trevas, mas terá luz de vida.
Jo 10,9: Eu sou a porta; se alguém entrar por mim, será salvo; e entrará, e sairá, e achará pasto.
Jo 10,11: Eu sou o bom Pastor; o bom Pastor dá sua vida pelas ovelhas.
Jo 11,25: Eu sou a ressurreição, e a vida; quem crê em mim, ainda que esteja morto, viverá.
Jo 14,6: Eu sou o caminho, e a verdade, e a vida; Ninguém vem ao Pai, a não ser por mim.
Jo 15,5: Eu sou a videira; quem está em mim, e eu nele, esse dá muito fruto.
```

É o **texto do Evangelho**, saindo do programa linha por linha.

### Passo 3: cada imagem vira uma classe, com uma etiqueta

Agora, cada imagem (o pão, a luz, a porta...) vira **uma classe**. Para o programa saber **quem fala "Eu sou"** sem mantermos uma lista à mão, colocamos uma **etiqueta** em cada classe. Em C#, essa etiqueta se chama **atributo** e se escreve entre colchetes:

```cs
[EuSou("o pão da vida", Referencia = "Jo 6,35")]
public class Pao : IImagem
{
    public string Promete() => "quem vem a mim de maneira nenhuma terá fome, e quem crê em mim nunca terá sede";
}
```

A etiqueta `[EuSou(...)]` repete "Eu sou o pão da vida", e `Promete()` repete a promessa: tudo é **palavra do versículo**.

### Passo 4: o programa acha as etiquetas sozinho

Com as sete classes etiquetadas, o programa pode **procurar** todas as que têm a etiqueta `[EuSou]`. Esse recurso de C# (um programa olhando para as próprias classes) chama-se **reflexão**. As próximas seções mostram o código completo.

::: cuidado Limite da analogia
- **A classe não "é" o pão.** `Pao` é um **modelo nosso**: um tipo de C# que carrega o texto do versículo. O código **não** interpreta o que cada imagem significa; só **organiza o que o texto diz**.
- **As promessas são cópias do versículo**, não resumos nossos. Onde há escolha de organização (por exemplo, usar Jo 15,5 para a videira, e deixar 15,1 para o capítulo 133), é **escolha editorial**.
- **A reflexão e o atributo** são técnicas de C#; que o programa "descubra" as classes não tem nenhum paralelo no texto: é só um jeito de **não esquecer nenhuma**.
:::

::: curiosidade Lupa no grego
- **ἐγώ εἰμι** (*egō eimi*, "eu sou") aparece **24 vezes** em João; **13** delas têm uma destas imagens (pão, luz, porta, pastor, ressurreição e vida, caminho, videira); nas outras (por exemplo 8,58 e 18,5) o "Eu sou" vem **sem predicado**. Veja todas com `dotnet run --project codigo/joao/Joao.Console -- eusou`.
- **ἄρτος** (*artos*, "pão", G740) aparece **24 vezes**, **21** delas no capítulo 6 (`onde artos`).
- **ἄμπελος** (*ampelos*, "videira") aparece **3 vezes**, todas em Jo 15 (`onde ampelos`); **ὁδός** (*hodos*, "caminho") aparece **4 vezes** (`onde hodos`).
- **Atenção:** a lista de "sete" imagens **não está no texto** de João, que não as numera. O texto grego que usamos, o SBLGNT, não traz Jo 7,53 a 8,11 no corpo; isso não afeta as passagens deste capítulo.

*Fontes: texto grego SBLGNT (CC BY 4.0); contagens por lema do MorphGNT (CC BY-SA 3.0), reproduzíveis com `dotnet run --project codigo/joao/Joao.Console -- onde <palavra>`; sentidos do dicionário de Strong (OpenScriptures), com a entrada indicada.*
:::


## Indo além: etiquetas e reflexão

Os passos 3 e 4 merecem o código completo, com atributos de verdade e reflexão. É a parte mais "C#" do capítulo, e a que costuma assustar: vamos devagar.

::: figura O programa lê as etiquetas
<svg viewBox="0 0 740 230" role="img" aria-label="Classes com a etiqueta Eu sou são encontradas pela reflexão e ordenadas pela passagem; uma classe sem etiqueta fica de fora">
  <g font-size="12" text-anchor="middle">
    <rect x="20" y="30" width="170" height="44" rx="10" class="f-info"/><text x="105" y="50" font-weight="800">[EuSou("a videira")]</text><text x="105" y="66" class="t-dim">class Videira</text>
    <rect x="20" y="84" width="170" height="44" rx="10" class="f-info"/><text x="105" y="104" font-weight="800">[EuSou("o pão da vida")]</text><text x="105" y="120" class="t-dim">class Pao</text>
    <rect x="20" y="138" width="170" height="44" rx="10" class="f-info"/><text x="105" y="158" font-weight="800">[EuSou("a porta")]</text><text x="105" y="174" class="t-dim">class Porta</text>
    <rect x="20" y="192" width="170" height="30" rx="10" class="f-bad" opacity=".5"/><text x="105" y="212" class="t-dim">class ServicoSemEtiqueta</text>
  </g>
  <rect x="270" y="60" width="170" height="110" rx="16" class="f-warn"/><text x="355" y="104" text-anchor="middle" font-weight="800">Reflexão</text><text x="355" y="126" text-anchor="middle" font-size="12" class="t-dim">GetTypes() +</text><text x="355" y="144" text-anchor="middle" font-size="12" class="t-dim">GetCustomAttribute</text>
  <path class="arrow" d="M192 105H268"/>
  <g font-size="12" text-anchor="middle">
    <rect x="520" y="40" width="200" height="34" rx="10" class="f-good"/><text x="620" y="62">Jo 6,35 · o pão da vida</text>
    <rect x="520" y="82" width="200" height="34" rx="10" class="f-good"/><text x="620" y="104">Jo 10,7 · a porta</text>
    <rect x="520" y="124" width="200" height="34" rx="10" class="f-good"/><text x="620" y="146">Jo 15,1 · a videira</text>
  </g>
  <path class="arrow" d="M442 110H518"/>
  <text x="370" y="214" text-anchor="middle" font-size="13" class="t-dim">Na ordem do texto, e não na ordem em que o código foi escrito.</text>
</svg>
:::

### 1) Atributos: etiquetas que o código lê

Um **atributo** é uma **etiqueta** que se cola em uma classe, método ou propriedade, entre colchetes: `[EuSou("o pão da vida")]`. Ele **não muda** o que a classe faz; só **acrescenta informação** sobre ela. Qualquer programa pode **ler** essa informação depois, e é o que faremos.

Criar um atributo é criar uma classe que herda de `Attribute`. O `[AttributeUsage]` diz **onde** ele pode ser usado (aqui, só em classes, uma vez cada). Os parâmetros do construtor são **obrigatórios** (`imagem`); as propriedades com `init` são **opcionais** e se escrevem com nome (`Referencia = "Jo 6,35"`).

```cs Jornada/Joao/Cap125/EuSou.cs
using System.Reflection;

namespace Jornada.Joao.Cap125;

/// <summary>
/// Um atributo é uma etiqueta que se cola em um tipo e que pode ser lida em tempo de execução.
/// Este diz: "eu sou isto", com a imagem, onde ela aparece no texto e o que promete.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class EuSouAttribute : Attribute
{
    public EuSouAttribute(string imagem) => Imagem = imagem;

    public string Imagem { get; }

    /// <summary>Onde a imagem aparece, no formato "Jo 6,35".</summary>
    public string Referencia { get; init; } = "";

    public string Promessa { get; init; } = "";
}

public interface IImagem
{
    string Promete();
}

/// <summary>Uma referência de capítulo e versículo, que sabe se comparar para ser colocada em ordem.</summary>
public readonly record struct Passagem(int Capitulo, int Versiculo) : IComparable<Passagem>
{
    public static bool TryParse(string? texto, out Passagem passagem)
    {
        passagem = default;
        if (string.IsNullOrWhiteSpace(texto)) return false;

        var partes = texto.Trim().Split(' ', 2, StringSplitOptions.TrimEntries);
        if (partes.Length != 2 || partes[0] != "Jo") return false;

        var numeros = partes[1].Split(',', StringSplitOptions.TrimEntries);
        if (numeros.Length != 2 || !int.TryParse(numeros[0], out var c) || !int.TryParse(numeros[1], out var v))
            return false;

        passagem = new Passagem(c, v);
        return true;
    }

    public int CompareTo(Passagem outra) =>
        Capitulo != outra.Capitulo ? Capitulo.CompareTo(outra.Capitulo) : Versiculo.CompareTo(outra.Versiculo);

    public override string ToString() => $"Jo {Capitulo},{Versiculo}";
}

public sealed record Imagem(Type Tipo, string Texto, Passagem? Passagem, string Promessa);

/// <summary>Procura, por reflexão, todos os tipos marcados com [EuSou] e os coloca na ordem do texto.</summary>
public static class Catalogo
{
    public static IReadOnlyList<Imagem> Descobrir(Assembly assembly) =>
        assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(IImagem).IsAssignableFrom(t))
            .Select(t => (Tipo: t, Marca: t.GetCustomAttribute<EuSouAttribute>()))
            .Where(x => x.Marca is not null)
            .Select(x => new Imagem(
                x.Tipo,
                x.Marca!.Imagem,
                Passagem.TryParse(x.Marca.Referencia, out var p) ? p : null,
                x.Marca.Promessa))
            .OrderBy(i => i.Passagem ?? new Passagem(int.MaxValue, 0))
            .ToList();

    /// <summary>Cria uma instância sem que o código conheça o tipo concreto na hora de escrever.</summary>
    public static IImagem Criar(Imagem imagem) => (IImagem)Activator.CreateInstance(imagem.Tipo)!;
}
```

Observe a `Passagem`: é um `readonly record struct` (como o `Vetor` do Livro 8) que implementa **`IComparable<Passagem>`**, para que o C# saiba ordená-la por capítulo e, no empate, por versículo. O `TryParse` lê o formato `"Jo 6,35"` **sem lançar exceção** em caso de lixo.

As sete classes são pequenas: cada uma tem a etiqueta e um método `Promete()`. Uma delas, de propósito, **não** tem etiqueta:

```cs Jornada/Joao/Cap125/Imagens.cs
namespace Jornada.Joao.Cap125;

// As sete imagens "Eu sou" do Evangelho (com predicado) viram sete serviços de um sistema.
// Cada classe leva o texto do versículo: a etiqueta diz quem fala ("Eu sou ...") e Promete() repete a promessa dita logo em seguida.
// Os textos são da Bíblia Livre (CC BY 4.0).

[EuSou("o pão da vida", Referencia = "Jo 6,35", Promessa = "de maneira nenhuma terá fome")]
public sealed class Pao : IImagem
{
    public string Promete() => "quem vem a mim de maneira nenhuma terá fome, e quem crê em mim nunca terá sede";
}

[EuSou("a luz do mundo", Referencia = "Jo 8,12", Promessa = "terá luz de vida")]
public sealed class Luz : IImagem
{
    public string Promete() => "quem me seguir não andará em trevas, mas terá luz de vida";
}

[EuSou("a porta", Referencia = "Jo 10,9", Promessa = "será salvo")]
public sealed class Porta : IImagem
{
    public string Promete() => "se alguém entrar por mim, será salvo; e entrará, e sairá, e achará pasto";
}

[EuSou("o bom pastor", Referencia = "Jo 10,11", Promessa = "dá sua vida pelas ovelhas")]
public sealed class Pastor : IImagem
{
    public string Promete() => "o bom Pastor dá sua vida pelas ovelhas";
}

[EuSou("a ressurreição, e a vida", Referencia = "Jo 11,25", Promessa = "viverá")]
public sealed class Ressurreicao : IImagem
{
    public string Promete() => "quem crê em mim, ainda que esteja morto, viverá";
}

[EuSou("o caminho, e a verdade, e a vida", Referencia = "Jo 14,6", Promessa = "ninguém vem ao Pai, a não ser por mim")]
public sealed class Caminho : IImagem
{
    public string Promete() => "Ninguém vem ao Pai, a não ser por mim";
}

[EuSou("a videira", Referencia = "Jo 15,5", Promessa = "dá muito fruto")]
public sealed class Videira : IImagem
{
    public string Promete() => "quem está em mim, e eu nele, esse dá muito fruto";
}

/// <summary>Este NÃO é marcado: o catálogo o ignora. Reflexão enxerga só o que está etiquetado.</summary>
public sealed class ImagemSemEtiqueta : IImagem
{
    public string Promete() => "existe, mas não se apresenta";
}
```

### 2) Reflexão: descobrir em tempo de execução

**Reflexão** é a capacidade de um programa **examinar a si mesmo**: listar os tipos que existem, ler suas etiquetas, criar objetos **sem saber o tipo na hora de escrever o código**. O `Catalogo.Descobrir` faz isso em três passos:

1. **`assembly.GetTypes()`** lista **todos** os tipos do assembly (o "pacote" compilado);
2. **filtra** quem é classe concreta e implementa `IImagem`;
3. **`GetCustomAttribute<EuSouAttribute>()`** lê a etiqueta; quem não tem (`null`) é descartado com o `Where`.

Depois, **`Activator.CreateInstance(tipo)`** cria um objeto a partir de um `Type`, sem um `new Pastor()` escrito em lugar algum. É o que permite ao catálogo mostrar o que cada imagem promete sem conhecê-los de antemão.

```cs Uso
var imagens = Catalogo.Descobrir(typeof(IImagem).Assembly);

foreach (var imagem in imagens)
{
    var servico = Catalogo.Criar(imagem);
    Console.WriteLine($"{imagem.Passagem,-9} {imagem.Texto,-31} {servico.Promete()}");
}

Console.WriteLine($"{imagens.Count} imagens descobertas por reflexão");
Console.WriteLine($"O serviço sem etiqueta aparece? {imagens.Any(i => i.Tipo == typeof(ImagemSemEtiqueta))}");
```

```saida
Jo 6,35   o pão da vida                   quem vem a mim de maneira nenhuma terá fome, e quem crê em mim nunca terá sede
Jo 8,12   a luz do mundo                  quem me seguir não andará em trevas, mas terá luz de vida
Jo 10,9   a porta                         se alguém entrar por mim, será salvo; e entrará, e sairá, e achará pasto
Jo 10,11  o bom pastor                    o bom Pastor dá sua vida pelas ovelhas
Jo 11,25  a ressurreição, e a vida        quem crê em mim, ainda que esteja morto, viverá
Jo 14,6   o caminho, e a verdade, e a vida Ninguém vem ao Pai, a não ser por mim
Jo 15,5   a videira                       quem está em mim, e eu nele, esse dá muito fruto
7 imagens descobertas por reflexão
O serviço sem etiqueta aparece? False
```

A ordem é a **do texto** (6, 8, 10, 10, 11, 14, 15), e não a do código. Sete classes etiquetadas apareceram; a oitava, **sem etiqueta**, **não**: reflexão só vê o que se declara.

::: dica Reflexão é poderosa, e tem custo
Ler tipos em tempo de execução é **mais lento** que chamar código normal e **foge da checagem do compilador** (um nome errado só falha ao rodar). Por isso, usa-se em **bordas** do sistema (catálogos, plugins, injeção de dependência, serializadores) e **não** no meio de laços quentes. No capítulo 133, você vai usá-la para montar um contêiner de dependências.
:::

### 3) Testes

```cs Joao.Testes/Cap125Tests.cs
using System.Reflection;
using Jornada.Joao.Cap125;

namespace Joao.Testes;

public class Cap125Tests
{
    private static readonly Assembly Montagem = typeof(IImagem).Assembly;

    [Fact]
    public void A_reflexao_encontra_as_sete_imagens_e_ignora_quem_nao_tem_etiqueta()
    {
        var imagens = Catalogo.Descobrir(Montagem);

        Assert.Equal(7, imagens.Count);
        Assert.DoesNotContain(imagens, i => i.Tipo == typeof(ImagemSemEtiqueta));
    }

    [Fact]
    public void As_imagens_vem_na_ordem_do_texto_e_nao_na_do_codigo()
    {
        var textos = Catalogo.Descobrir(Montagem).Select(i => i.Texto).ToList();

        Assert.Equal(new[]
        {
            "o pão da vida", "a luz do mundo", "a porta", "o bom pastor",
            "a ressurreição, e a vida", "o caminho, e a verdade, e a vida", "a videira"
        }, textos);
    }

    [Theory]
    [InlineData("Jo 6,35", 6, 35)]
    [InlineData(" Jo 14,6 ", 14, 6)]
    public void Passagem_le_o_formato_do_texto(string texto, int capitulo, int versiculo)
    {
        Assert.True(Passagem.TryParse(texto, out var p));
        Assert.Equal(new Passagem(capitulo, versiculo), p);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("Mt 5,3")]
    [InlineData("Jo 6")]
    [InlineData("Jo x,y")]
    public void Passagem_recusa_o_que_nao_entende_sem_lancar_excecao(string? texto) =>
        Assert.False(Passagem.TryParse(texto, out _));

    [Fact]
    public void Passagens_se_ordenam_por_capitulo_e_depois_por_versiculo()
    {
        var lista = new[] { new Passagem(10, 11), new Passagem(6, 35), new Passagem(10, 7) };

        Assert.Equal(new[] { "Jo 6,35", "Jo 10,7", "Jo 10,11" }, lista.Order().Select(p => p.ToString()));
    }

    [Fact]
    public void Cria_instancias_sem_conhecer_o_tipo_na_hora_de_escrever()
    {
        var pastor = Catalogo.Descobrir(Montagem).Single(i => i.Texto == "o bom pastor");

        IImagem servico = Catalogo.Criar(pastor);

        Assert.IsType<Pastor>(servico);
        Assert.Contains("dá sua vida pelas ovelhas", servico.Promete());
    }

    [Fact]
    public void O_atributo_guarda_o_que_foi_declarado()
    {
        var marca = typeof(Videira).GetCustomAttribute<EuSouAttribute>();

        Assert.NotNull(marca);
        Assert.Equal("a videira", marca!.Imagem);
        Assert.Equal("Jo 15,5", marca.Referencia);
        Assert.Equal("dá muito fruto", marca.Promessa);
    }
}
```

O teste **`As_imagens_vem_na_ordem_do_texto_e_nao_na_do_codigo`** é a razão de existir do `IComparable<Passagem>`. E **`Passagem_recusa_o_que_nao_entende_sem_lancar_excecao`** mostra o cuidado com entradas ruins: `Mt 5,3`, `Jo 6` e `Jo x,y` não derrubam nada.

::: exercicio Descrever uma imagem
Escreva o método de extensão `Descrever(this Imagem imagem)`, que devolve `"o bom pastor (Jo 10,11): dá sua vida pelas ovelhas"`. Se a imagem não tiver passagem conhecida, mostre `"sem passagem"` no lugar. Dica: padrão `is { } p` para o `Passagem?`.

--- resposta
```cs Jornada/Joao/Exercicios/Cap125Descrever.cs
using Jornada.Joao.Cap125;

namespace Jornada.Joao.Exercicios;

public static class ImagemExtensoes
{
    /// <summary>"o bom pastor (Jo 10,11): cuidado". Sem passagem conhecida, mostra "sem passagem".</summary>
    public static string Descrever(this Imagem imagem)
    {
        var onde = imagem.Passagem is { } p ? p.ToString() : "sem passagem";
        return $"{imagem.Texto} ({onde}): {imagem.Promessa}";
    }
}
```

`imagem.Passagem is { } p` testa "não é nulo" **e** captura o valor (do tipo `Passagem`, e não `Passagem?`) em `p`, em uma só expressão. É o padrão de propriedade que você viu no Livro 6.
:::

::: resumo
- Cada "Eu sou" com imagem tem **uma imagem e uma promessa**; guarda-se em `record Declaracao(Passagem, EuSou, Promessa)`, com o texto do versículo.
- Uma **etiqueta** (`[EuSou(...)]`) em cada classe permite que o programa **as encontre sozinho** por reflexão. A lista de "sete" **não está no texto**.
- Um **atributo** é uma etiqueta lida em tempo de execução; `[AttributeUsage]` diz onde pode ser usado; parâmetros do construtor são obrigatórios e propriedades `init` são opcionais.
- **Reflexão** examina o próprio programa: `GetTypes()`, `GetCustomAttribute<T>()`, `Activator.CreateInstance`.
- `IComparable<T>` ensina o C# a ordenar o tipo; `TryParse` lê formatos **sem exceção**.
- Reflexão é boa nas **bordas** (catálogos, plugins), mas é mais lenta e **sem checagem do compilador**.
:::

::: quiz
? Que três informações guarda uma `Declaracao`?
+ Onde está escrito, o que é dito ("Eu sou...") e a promessa
- Só o número do capítulo
- O nome do autor e a data
- Uma imagem e um som
> É o molde comum às declarações.

? O que faz a etiqueta `[EuSou("o pão da vida")]` sobre uma classe?
+ Acrescenta informação que o programa pode ler depois
- Muda o que a classe faz
- Apaga a classe
- Cria um objeto
> É um atributo: informação sobre a classe.

? A lista de "sete" imagens "Eu sou" está no texto de João?
+ Não: João não numera as imagens; o agrupamento em sete é uma organização de quem estuda
- Sim, em Jo 6
- Sim, em Jo 15
- Sim, em Jo 21
> O texto tem 24 ocorrências de "eu sou" (*egō eimi*), e nem todas têm imagem.

? Quantas vezes aparece "Eu sou" (*egō eimi*, em primeira pessoa) em João, segundo a contagem do capítulo?
+ 24 vezes
- 7 vezes
- 100 vezes
- Só uma vez
> Dessas, 13 levam uma das sete imagens; o restante tem outros usos.

? O que é um atributo em C#?
+ Uma etiqueta colocada em um tipo ou membro, que outros programas podem ler
- Uma variável global
- Um tipo de exceção
- Um comentário
> Ele acrescenta informação sem mudar o comportamento da classe.

? Para que serve `GetCustomAttribute<EuSouAttribute>()`?
+ Para ler a etiqueta `[EuSou]` de um tipo, ou `null` se não tiver
- Para criar uma classe nova
- Para apagar o atributo
- Para compilar o código
> Quem devolve `null` é descartado pelo `Where`.

? O que faz `Activator.CreateInstance(tipo)`?
+ Cria um objeto a partir de um `Type`, sem o código conhecer a classe na hora de escrever
- Compila uma classe
- Converte texto em número
- Fecha o programa
> Permite criar objetos descobertos por reflexão.

? Por que a `Passagem` implementa `IComparable<Passagem>`?
+ Para que as imagens possam ser ordenadas por capítulo e versículo
- Para converter em texto
- Para serializar em JSON
- Para ser imutável
> O `OrderBy` usa o `CompareTo` do tipo.
:::

::: cartas
Declaracao :: `record` com passagem, o que é dito e a promessa de cada "Eu sou".
Eu sou (egō eimi) :: Expressão de João; 24 vezes em primeira pessoa, 13 com predicado.
As imagens "Eu sou" :: Pão, luz, porta, pastor, ressurreição e vida, caminho/verdade/vida, videira. A lista de "sete" não é de João.
Atributo :: Etiqueta em C# lida em tempo de execução, escrita entre colchetes.
Reflexão :: Capacidade de um programa examinar seus próprios tipos e etiquetas.
Assembly :: O "pacote" compilado (DLL ou EXE) onde moram os tipos.
Activator.CreateInstance :: Cria um objeto a partir de um `Type`.
IComparable<T> :: Contrato para o tipo saber ordenar-se.
TryParse :: Lê um formato sem lançar exceção: devolve `bool`.
:::
