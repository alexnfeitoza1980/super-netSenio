---
id: resiliencia-cap90
order: 116
kind: capitulo
num: 90
group: C# & Mentalidade · Parte 15 · Resiliência em C#
title: O que está na sua mochila?
subtitle: Recursos que já temos, o que depende de nós e a diferença entre fato e história. Em C#, é o capítulo do LINQ, a linguagem de perguntas sobre coleções.
minutes: 60
hue: 245
objectives:
- Separar o que **depende de mim**, o que posso **influenciar** e o que **não depende**
- Distinguir **fato** de **história** sem transformar a história em bronca
- Fazer perguntas a coleções com **LINQ**: `Where`, `Select`, `GroupBy`, `ToLookup`, `Except`
- Criar uma API **fluente** (`Guardar(...).Guardar(...)`) e devolver **tuplas**
origem: livros/06-resiliencia-csharp/content/127-cap90.md
---

::: vila A lista que não cabia na cabeça
Beto dizia que "tudo estava dando errado": prova na sexta, notebook com a tela rachada, a mãe doente no interior, o professor que não respondia e uma entrega de projeto. Lia pegou um papel e propôs outra coisa: "vamos separar. O que **depende só de você**? O que você **pode influenciar**? E o que **não depende** de ninguém aqui?". A pilha virou três montinhos. A prova na sexta ficou na zona "depende de mim", e o professor que não respondia, em "posso influenciar" (pedir de outro jeito). A doença da mãe foi para a terceira zona, e Beto, pela primeira vez no dia, respirou.
:::

::: ilustracao img/resiliencia/cap90.svg | A mochila guarda recursos; as preocupações se dividem em três zonas de controle: depende de mim, posso influenciar, não depende.
:::

## O que o livro diz

O texto sobre resiliência fala em **analisar o ambiente**: olhar com cuidado para a situação e para **o que está disponível**: habilidades, pessoas, materiais, informações. A ideia, dita em palavras nossas: antes de se perguntar "por que isso aconteceu comigo?", vale perguntar **"com o que eu conto agora?"** e **"onde minha energia rende mais?"**.

Dois movimentos práticos aparecem nessa leitura:

1. **Inventariar os recursos**: o que está na minha "mochila"? Isso inclui qualidades pessoais, pessoas que podem ajudar, coisas concretas e informações.
2. **Separar fatos de histórias**: o **fato** é o que se pode observar ("entreguei dois dias depois do prazo"); a **história** é a explicação que imaginamos ("sou um fracasso"). Histórias não são "mentiras": são **hipóteses** que merecem uma checagem gentil.

::: analogia O guarda-chuva não controla o tempo
O guarda-chuva não decide se vai chover, mas **decide se você se molha**. Separar o que controlamos (levar o guarda-chuva) do que não controlamos (a chuva) não é desistir: é **escolher onde pôr a energia**.
:::

::: cuidado Separar não é minimizar
Dizer "isso não depende de você" **não** significa "então não sinta nada". Há coisas que não controlamos e que **doem**, como uma doença ou uma perda. A separação serve para escolher **onde agir**, não para apagar o sentimento nem a importância do que está fora do nosso alcance.
:::

## Da vida para o código

| Na vida | Em C# |
|---|---|
| Três zonas de controle | `enum Controle` e **`ToLookup`** (agrupar e consultar por chave) |
| "Por onde começo?" | `ProximoFoco`: `FirstOrDefault` e o operador **`??`** |
| Fato × história | `enum NaturezaDaFrase` e **`Where` + `Select`** |
| Quantos fatos, quantas histórias? | Método que devolve uma **tupla** `(int Fatos, int Historias)` |
| A mochila de recursos | `Mochila` com **API fluente** e `GroupBy` |
| "O que ainda falta na mochila?" | `Enum.GetValues<T>().Except(...)` |

::: figura As três zonas de controle
<svg viewBox="0 0 740 230" role="img" aria-label="Três círculos concêntricos: o que depende de mim no centro, o que posso influenciar ao redor e o que não depende de mim, fora">
  <circle cx="160" cy="115" r="105" class="f-info" opacity=".55"/>
  <circle cx="160" cy="115" r="72" class="f-warn" opacity=".75"/>
  <circle cx="160" cy="115" r="38" class="f-good"/>
  <text x="160" y="119" text-anchor="middle" font-size="12" font-weight="800">depende de mim</text>
  <text x="160" y="57" text-anchor="middle" font-size="12" font-weight="800">posso influenciar</text>
  <text x="160" y="22" text-anchor="middle" font-size="12" font-weight="800">não depende</text>
  <g font-size="14">
    <text x="360" y="60" font-weight="800">Começar o exercício</text><text x="360" y="80" class="t-dim" font-size="12">→ depende de mim: agir primeiro</text>
    <text x="360" y="124" font-weight="800">Tirar a dúvida com a monitora</text><text x="360" y="144" class="t-dim" font-size="12">→ posso influenciar: pedir de forma clara</text>
    <text x="360" y="188" font-weight="800">A prova ser difícil</text><text x="360" y="208" class="t-dim" font-size="12">→ não depende: acolher, e preparar o que der</text>
  </g>
</svg>
:::

## 1) LINQ em dois minutos

**LINQ** (*Language Integrated Query*) é um conjunto de métodos para **fazer perguntas a coleções**: "quais são os que...?", "quantos?", "agrupe por...". Cada método recebe uma **lambda**, uma pequena função escrita como `x => condição`, e devolve algo novo, sem mexer na coleção original.

| Pergunta | Método |
|---|---|
| Quais atendem a uma condição? | `Where(x => ...)` |
| Transforme cada item em outra coisa | `Select(x => ...)` |
| Quantos? Algum? Todos? | `Count`, `Any`, `All` |
| Agrupe pela chave | `GroupBy`, `ToLookup` |
| O primeiro (ou nada) | `FirstOrDefault` |
| Tudo menos isto | `Except` |

Ao longo do capítulo você vai vê-los trabalhando juntos, em código curto e legível.

## 2) Separando as preocupações e os fatos

`ToLookup` agrupa por chave e permite consultar qualquer chave depois, mesmo uma sem itens (devolve uma coleção vazia, nunca um erro). `ProximoFoco` usa **`??`** (o operador "se for nulo, use isto") para tentar a zona 1 e, só se ela estiver vazia, a zona 2. E `Contar` devolve uma **tupla** nomeada, de modo que quem chama pode escrever `var (fatos, historias) = ...`.

```cs Jornada/Resiliencia/Cap90/Organizador.cs
namespace Jornada.Resiliencia.Cap90;

public enum Controle { DependeDeMim, PossoInfluenciar, NaoDepende }

public sealed record Preocupacao(string Texto, Controle Controle);

public enum NaturezaDaFrase { Fato, Historia }

public sealed record Anotacao(string Texto, NaturezaDaFrase Natureza);

public static class Organizador
{
    /// <summary>Agrupa as preocupações pelas três "zonas" de controle.</summary>
    public static ILookup<Controle, Preocupacao> Separar(IEnumerable<Preocupacao> itens) =>
        itens.ToLookup(p => p.Controle);

    /// <summary>Sugere onde pôr energia primeiro: o que depende de mim; se não houver, o que posso influenciar.</summary>
    public static Preocupacao? ProximoFoco(IEnumerable<Preocupacao> itens) =>
        itens.FirstOrDefault(p => p.Controle == Controle.DependeDeMim)
        ?? itens.FirstOrDefault(p => p.Controle == Controle.PossoInfluenciar);

    /// <summary>Quantos fatos e quantas histórias há nas anotações. Devolve uma tupla.</summary>
    public static (int Fatos, int Historias) Contar(IEnumerable<Anotacao> anotacoes)
    {
        var lista = anotacoes.ToList();
        return (lista.Count(a => a.Natureza == NaturezaDaFrase.Fato),
                lista.Count(a => a.Natureza == NaturezaDaFrase.Historia));
    }

    /// <summary>Cada história merece uma pergunta de checagem, não uma bronca.</summary>
    public static IEnumerable<string> PerguntasParaAsHistorias(IEnumerable<Anotacao> anotacoes) =>
        anotacoes
            .Where(a => a.Natureza == NaturezaDaFrase.Historia)
            .Select(a => $"\"{a.Texto}\": o que me faz pensar assim? Existe outra explicação possível?");
}
```

Repare em duas escolhas de cuidado: as **perguntas para as histórias** são curiosas ("o que me faz pensar assim? Existe outra explicação possível?"), e **não** do tipo "isso é mentira". E o que **não depende de mim** nunca vira o foco da ação: o código **não manda esquecer** essa preocupação, só não a coloca como tarefa.

## 3) A mochila de recursos

A `Mochila` usa a técnica de **API fluente**: `Guardar` devolve `this`, e por isso as chamadas se encadeiam. `TiposVazios` usa `Enum.GetValues<TipoRecurso>()` (todos os valores do `enum`) e **`Except`** (menos os que já têm algo): o resultado são as **lacunas**. E cada lacuna vira uma **pergunta curiosa**, nunca uma cobrança.

```cs Jornada/Resiliencia/Cap90/Mochila.cs
namespace Jornada.Resiliencia.Cap90;

public enum TipoRecurso { Pessoal, Pessoas, Material, Informacao }

public sealed record Recurso(string Nome, TipoRecurso Tipo);

public sealed class Mochila
{
    private readonly List<Recurso> recursos = new();

    public IReadOnlyList<Recurso> Todos => recursos;

    public Mochila Guardar(string nome, TipoRecurso tipo)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Dê um nome ao recurso.", nameof(nome));

        recursos.Add(new Recurso(nome.Trim(), tipo));
        return this;                                    // devolver "this" permite encadear chamadas
    }

    public IEnumerable<IGrouping<TipoRecurso, Recurso>> PorTipo() =>
        recursos.GroupBy(r => r.Tipo).OrderBy(g => g.Key);

    /// <summary>Tipos de recurso em que a mochila ainda está vazia.</summary>
    public IReadOnlyList<TipoRecurso> TiposVazios() =>
        Enum.GetValues<TipoRecurso>().Except(recursos.Select(r => r.Tipo)).ToList();

    /// <summary>Para cada lacuna, uma pergunta curiosa, nunca uma cobrança.</summary>
    public IEnumerable<string> Perguntas() =>
        TiposVazios().Select(tipo => tipo switch
        {
            TipoRecurso.Pessoal    => "Que habilidade ou qualidade sua já ajudou em outra ocasião?",
            TipoRecurso.Pessoas    => "Quem costuma ouvir você ou ajudar com ideias?",
            TipoRecurso.Material   => "Que objeto, lugar ou ferramenta poderia facilitar o próximo passo?",
            TipoRecurso.Informacao => "Que informação ou orientação ainda falta descobrir?",
            _                      => throw new ArgumentOutOfRangeException(nameof(tipo))
        });
}
```

```cs Uso
var preocupacoes = new[]
{
    new Preocupacao("Começar o primeiro exercício", Controle.DependeDeMim),
    new Preocupacao("Tirar uma dúvida com a monitora", Controle.PossoInfluenciar),
    new Preocupacao("A prova ser difícil", Controle.NaoDepende),
    new Preocupacao("Organizar a mesa de estudo", Controle.DependeDeMim),
};

foreach (var zona in Organizador.Separar(preocupacoes))
    Console.WriteLine($"{zona.Key}: {string.Join(" | ", zona.Select(p => p.Texto))}");

Console.WriteLine($"Foco: {Organizador.ProximoFoco(preocupacoes)?.Texto}");

var mochila = new Mochila()
    .Guardar("Sei pedir ajuda por mensagem", TipoRecurso.Pessoal)
    .Guardar("Colega de turma", TipoRecurso.Pessoas);

foreach (var pergunta in mochila.Perguntas())
    Console.WriteLine("? " + pergunta);
```

```saida
DependeDeMim: Começar o primeiro exercício | Organizar a mesa de estudo
PossoInfluenciar: Tirar uma dúvida com a monitora
NaoDepende: A prova ser difícil
Foco: Começar o primeiro exercício
? Que objeto, lugar ou ferramenta poderia facilitar o próximo passo?
? Que informação ou orientação ainda falta descobrir?
```

Note a ordem do `ToLookup`: as chaves aparecem na ordem em que surgem na lista, e a saída mostra que a prova difícil ficou na zona que **não depende de mim**, sem apagar sua importância.

## 4) Testes

```cs Resiliencia.Testes/Cap90Tests.cs
using Jornada.Resiliencia.Cap90;

namespace Resiliencia.Testes;

public class Cap90Tests
{
    private static readonly Preocupacao[] Lista =
    {
        new("Começar o primeiro exercício", Controle.DependeDeMim),
        new("Tirar uma dúvida com a monitora", Controle.PossoInfluenciar),
        new("A prova ser difícil", Controle.NaoDepende),
        new("Organizar a mesa de estudo", Controle.DependeDeMim),
    };

    [Fact]
    public void Separar_cria_tres_zonas()
    {
        var zonas = Organizador.Separar(Lista);

        Assert.Equal(2, zonas[Controle.DependeDeMim].Count());
        Assert.Single(zonas[Controle.PossoInfluenciar]);
        Assert.Single(zonas[Controle.NaoDepende]);
    }

    [Fact]
    public void Foco_comeca_pelo_que_depende_de_mim()
    {
        Assert.Equal("Começar o primeiro exercício", Organizador.ProximoFoco(Lista)!.Texto);
    }

    [Fact]
    public void Sem_nada_que_dependa_de_mim_o_foco_vai_para_o_que_posso_influenciar()
    {
        var so = Lista.Where(p => p.Controle != Controle.DependeDeMim);
        Assert.Equal(Controle.PossoInfluenciar, Organizador.ProximoFoco(so)!.Controle);
    }

    [Fact]
    public void Se_so_ha_o_que_nao_depende_o_foco_e_nulo()
    {
        var so = new[] { new Preocupacao("O tempo amanhã", Controle.NaoDepende) };
        Assert.Null(Organizador.ProximoFoco(so));
    }

    [Fact]
    public void Contar_separa_fatos_de_historias()
    {
        var notas = new[]
        {
            new Anotacao("Entreguei o trabalho dois dias depois", NaturezaDaFrase.Fato),
            new Anotacao("Sou um fracasso", NaturezaDaFrase.Historia),
            new Anotacao("A professora não respondeu a mensagem", NaturezaDaFrase.Fato),
        };

        var (fatos, historias) = Organizador.Contar(notas);

        Assert.Equal(2, fatos);
        Assert.Equal(1, historias);
        Assert.Single(Organizador.PerguntasParaAsHistorias(notas));
    }

    [Fact]
    public void Mochila_aponta_lacunas_com_perguntas()
    {
        var mochila = new Mochila()
            .Guardar("Sei pedir ajuda por mensagem", TipoRecurso.Pessoal)
            .Guardar("Colega de turma", TipoRecurso.Pessoas);

        Assert.Equal(new[] { TipoRecurso.Material, TipoRecurso.Informacao }, mochila.TiposVazios());
        Assert.Equal(2, mochila.Perguntas().Count());
        Assert.Equal(2, mochila.PorTipo().Count());
    }

    [Fact]
    public void Recurso_sem_nome_e_recusado() =>
        Assert.Throws<ArgumentException>(() => new Mochila().Guardar("  ", TipoRecurso.Material));
}
```

O teste `Se_so_ha_o_que_nao_depende_o_foco_e_nulo` é importante: quando só restam preocupações fora do nosso alcance, o programa devolve **nenhum foco de ação**. O `null` aqui é uma resposta honesta ("não há o que fazer agora"), e quem chama decide o que dizer: talvez "que tal conversar com alguém?".

::: exercicio O resumo da mochila
Escreva o método de extensão `Resumo(this Mochila mochila)`, que devolve **uma linha por tipo** de recurso, no formato `"Pessoas: 2 (Beto, Marta)"`, com os nomes em ordem alfabética. Use `PorTipo()`, `Select`, `Count` e `string.Join`.

--- resposta
```cs Jornada/Resiliencia/Exercicios/Cap90Resumo.cs
namespace Jornada.Resiliencia.Cap90;

public static class MochilaExtensoes
{
    /// <summary>Uma linha por tipo: "Pessoas: 2 (Beto, Marta)".</summary>
    public static IReadOnlyList<string> Resumo(this Mochila mochila) =>
        mochila.PorTipo()
               .Select(grupo => $"{grupo.Key}: {grupo.Count()} ({string.Join(", ", grupo.Select(r => r.Nome).OrderBy(n => n))})")
               .ToList();
}
```

O `this` antes do primeiro parâmetro transforma o método em **extensão**: agora se escreve `mochila.Resumo()`, como se `Resumo` fosse da classe. É assim que o próprio LINQ funciona (`Where`, `Select`... são métodos de extensão de `IEnumerable<T>`).
:::

::: resumo
- Antes de "por que comigo?", pergunte **"com o que eu conto?"** e **"onde minha energia rende?"**.
- Separar **depende de mim / posso influenciar / não depende** orienta a ação, **sem minimizar** o que dói.
- **Fato** é observável; **história** é uma hipótese a checar com curiosidade, não com bronca.
- **LINQ** faz perguntas a coleções com lambdas: `Where`, `Select`, `GroupBy`, `ToLookup`, `Except`, `FirstOrDefault`.
- `??` escolhe o primeiro valor não nulo; **tuplas** devolvem mais de um valor; **APIs fluentes** devolvem `this` para encadear chamadas.
:::

::: quiz
? O que é um "fato" neste capítulo?
+ Algo observável, como "entreguei dois dias depois do prazo"
- Qualquer coisa que eu sinto com muita força
- A explicação que imagino para o que ocorreu
- Uma opinião de outra pessoa
> A história é a interpretação; o fato é o que dá para observar.

? Por que as histórias recebem perguntas e não correções?
+ Porque são hipóteses que merecem checagem gentil
- Porque histórias sempre estão certas
- Porque o programa não sabe diferenciar
- Porque perguntas são mais curtas
> A pergunta abre espaço; a bronca fecha.

? O que `ToLookup` faz?
+ Agrupa os itens por chave e permite consultar qualquer chave depois
- Ordena a lista pelo tamanho
- Remove os itens duplicados
- Converte a lista em texto
> Uma chave sem itens devolve uma coleção vazia, sem erro.

? Para que serve o operador `??` em `ProximoFoco`?
+ Usar a segunda opção quando a primeira é nula
- Comparar dois valores
- Converter para `int`
- Lançar uma exceção
> `a ?? b` significa "a, ou b se a for nulo".

? O que torna uma API "fluente", como a da `Mochila`?
+ Cada método devolve `this`, permitindo encadear chamadas
- Ela usa muitos comentários
- Ela é escrita em inglês
- Ela não tem classes
> `Guardar(...).Guardar(...)` só funciona porque cada chamada devolve a mochila.

? Dizer "isso não depende de você" significa que a pessoa não deve sentir nada?
- Sim, é melhor esquecer
+ Não; serve para escolher onde agir, sem apagar o sentimento
- Sim, se o problema for grande
- Só nos casos de doença
> Há coisas fora do controle que doem, e isso é legítimo.
:::

::: cartas
Mochila de recursos :: Conjunto do que temos para atravessar uma dificuldade: qualidades, pessoas, coisas e informações.
Depende de mim :: Zona do que está sob meu controle direto.
Posso influenciar :: Zona em que posso agir, mas sem garantir o resultado.
Fato :: Informação observável sobre a situação.
História :: Explicação que imaginamos; merece checagem.
LINQ :: Conjunto de métodos para consultar e transformar coleções.
Lambda :: Função curta escrita como `x => expressão`.
Tupla :: Pequeno grupo de valores, como `(int Fatos, int Historias)`.
:::
