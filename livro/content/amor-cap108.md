---
id: amor-cap108
order: 140
kind: capitulo
num: 108
group: C# & Mentalidade · Parte 17 · Você é aquilo que ama, em C#
title: Você pode não amar aquilo que acredita amar
subtitle: A distância entre o que declaramos querer e o que nossos hábitos revelam. Em C#, é o capítulo dos joins do LINQ: juntar duas listas (o que se diz e o que se faz) e ler a diferença com curiosidade, e não com culpa.
minutes: 70
hue: 315
objectives:
- Entender a ideia de que o desejo mais profundo aparece **nos hábitos**, e não só nas declarações
- Conhecer o **inconsciente "prático"** (automatismos) e por que ele pede **curiosidade**, não vergonha
- Usar LINQ avançado: **`GroupJoin`** (junção à esquerda), **`ExceptBy`** e **`Select`** com índice
- Escrever um comparador "o que se diz × o que se faz" que **observa sem julgar**
origem: livros/08-voce-e-aquilo-que-ama-csharp/content/128-cap108.md
---

::: vila A lista da geladeira
Marta colou na geladeira a lista de prioridades do ano: 1) família, 2) saúde, 3) estudar, 4) trabalho. Em dezembro, olhou a agenda do ano inteiro: o trabalho ocupava quase tudo, a família tinha as sobras, a saúde, quase nada. Ela se sentiu mal por um instante, mas Lia perguntou: "e se a lista da geladeira for **o que você quer querer**, e a agenda for **o que você, hoje, escolhe**? O que você aprende comparando as duas?".
:::

::: ilustracao img/amor/cap108.svg | Duas listas lado a lado: o que dissemos que importa e para onde o tempo realmente foi. A diferença não é uma sentença: é uma pergunta.
:::

## O que o livro diz

O capítulo do livro em que isso aparece começa com um filme: *Stalker*, de Andrei Tarkovsky. Três homens atravessam uma região misteriosa, a Zona, rumo a uma **Sala** onde, diz o guia, **cada um conseguirá o desejo mais profundo do seu coração**. E é exatamente aí que dois deles hesitam: **e se o que a Sala conceder não for o que eles pensam que querem?** Um crítico, citado pelo autor, observa que a Sala entrega não o que você *acha* que deseja, mas o que você **mais profundamente** deseja, e que às vezes alguém não quer fazer o que pensa querer fazer.

Smith extrai disso uma ideia desconfortável e útil: **"seu desejo mais profundo é aquele que você manifesta em seus hábitos e na sua vida cotidiana"**. A explicação dele tem duas partes:

1. **O amor é hábito.** Como vimos no capítulo anterior, o que amamos se grava por prática, e o hábito opera **sob o radar** da consciência, como respirar ou piscar.
2. **A formação também é inconsciente.** Ele recorre à psicologia (autores como Timothy Wilson, John Bargh e Tanya Chartrand) para lembrar que boa parte do nosso comportamento é conduzida por **automatismos**: modos aprendidos de perceber e agir que **não escolhemos** de forma deliberada. Alguns automatismos adquirimos de propósito (como aprender a dirigir ou a digitar); outros se infiltram **sem intenção**, por repetição de rotinas que nunca reconhecemos como "práticas". O exemplo mais forte que ele dá é o dos **estereótipos**: ninguém decide acreditar neles, e ainda assim eles se tornam hábitos de percepção.

A imagem do **iceberg** resume: o que decidimos conscientemente é a ponta; o resto da navegação é feito debaixo da água.

::: cuidado Sem vergonha, por favor
Nada disso é um convite ao autojulgamento. O próprio autor trata a distância entre o que pensamos amar e o que amamos como uma **realidade humana comum**, e não como uma falha pessoal. Em tradições religiosas, isso se acolhe com **confissão**; em um contexto de trabalho, o equivalente saudável é a **curiosidade honesta**. Se olhar para a própria agenda lhe dá vergonha ou ansiedade forte, **pause**: converse com alguém de confiança. Um programa não deve ocupar o lugar disso.
:::

::: analogia O extrato bancário
Ninguém tem dúvida de que "valoriza" a poupança. O extrato mostra para onde o dinheiro realmente foi. Não é uma acusação; é uma **leitura de fatos** que pode nos surpreender e nos orientar. A agenda é o extrato do tempo.
:::

## Da vida para o código

| No livro | Em C# |
|---|---|
| O que declaro que importa | `Prioridade(Area, Ordem)` |
| O que meus hábitos mostram | `Registro(Dia, Area, Minutos)` |
| Comparar as duas listas | **`GroupJoin`** (cada prioridade com o tempo registrado nela) |
| Uma prioridade sem nenhum registro | O grupo **vazio** do `GroupJoin`: aparece com 0 min, não some |
| Tempo gasto em algo que ninguém listou | **`ExceptBy`** (quem está em uma lista e não na outra) |
| A posição "observada" | `OrderByDescending` + **`Select((x, indice) => ...)`** |
| Um tom curioso, não acusador | Frases testadas para **não conter** "culpa" nem "fracasso" |

::: figura O que se diz × o que se faz
<svg viewBox="0 0 740 250" role="img" aria-label="Duas colunas: prioridades declaradas (família, saúde, estudo, trabalho) e ordem do tempo registrado (trabalho, estudo, família, saúde), com linhas ligando cada área">
  <text x="170" y="26" text-anchor="middle" font-weight="800">O que eu disse que importa</text>
  <text x="570" y="26" text-anchor="middle" font-weight="800">Para onde o tempo foi</text>
  <g font-size="14" text-anchor="middle">
    <rect x="90" y="42" width="160" height="36" rx="9" class="f-prim"/><text x="170" y="66">1º Família</text>
    <rect x="90" y="88" width="160" height="36" rx="9" class="f-prim"/><text x="170" y="112">2º Saúde</text>
    <rect x="90" y="134" width="160" height="36" rx="9" class="f-prim"/><text x="170" y="158">3º Estudo</text>
    <rect x="90" y="180" width="160" height="36" rx="9" class="f-prim"/><text x="170" y="204">4º Trabalho</text>
    <rect x="490" y="42" width="160" height="36" rx="9" class="f-warn"/><text x="570" y="66">1º Trabalho · 55%</text>
    <rect x="490" y="88" width="160" height="36" rx="9" class="f-warn"/><text x="570" y="112">2º Estudo · 18%</text>
    <rect x="490" y="134" width="160" height="36" rx="9" class="f-warn"/><text x="570" y="158">3º Família · 11%</text>
    <rect x="490" y="180" width="160" height="36" rx="9" class="f-warn"/><text x="570" y="204">4º Saúde · 3%</text>
  </g>
  <path d="M252 60L488 150" stroke="var(--ink-3)" stroke-width="2"/><path d="M252 106L488 198" stroke="var(--ink-3)" stroke-width="2"/>
  <path d="M252 152L488 106" stroke="var(--ink-3)" stroke-width="2"/><path d="M252 198L488 60" stroke="var(--ink-3)" stroke-width="2"/>
  <text x="370" y="240" text-anchor="middle" font-size="13" class="t-dim">As linhas que se cruzam não são erros: são perguntas.</text>
</svg>
:::

## 1) Dois tipos de dado, e uma junção

Temos duas listas. Uma tem as **prioridades** declaradas; outra, **registros** de tempo (um por dia e por área). Para compará-las, queremos **juntar** as duas pela área.

O LINQ oferece vários jeitos de juntar:

| Operação | O que faz | Parecido com |
|---|---|---|
| `Join` | Só devolve quem tem par nas **duas** listas | `INNER JOIN` em SQL |
| **`GroupJoin`** | Para **cada** item da esquerda, o **grupo** (talvez vazio) de pares da direita | `LEFT JOIN` (agrupado) |
| `ExceptBy` | Quem está em uma lista e **não** na outra, comparando por uma chave | "o que sobra" |

Aqui precisamos do **`GroupJoin`**: uma prioridade **sem nenhum tempo registrado** continua na lista, com **zero minutos**. Com um `Join` comum, ela **desapareceria**, e justamente a prioridade esquecida é a mais importante de enxergar.

```cs Jornada/Habitos/Cap108/Revelacao.cs
namespace Jornada.Habitos.Cap108;

/// <summary>O que a pessoa DISSE que importa (1 = mais importante).</summary>
public sealed record Prioridade(string Area, int Ordem);

/// <summary>O que o calendário e os registros MOSTRAM: tempo gasto num dia, numa área.</summary>
public sealed record Registro(DateOnly Dia, string Area, int Minutos);

public sealed record Comparacao(string Area, int OrdemDeclarada, int OrdemObservada, int Minutos, double Parte)
{
    public int Diferenca => OrdemObservada - OrdemDeclarada;
}

public sealed record AreaSemPrioridade(string Area, int Minutos, double Parte);

public sealed record Relatorio(
    IReadOnlyList<Comparacao> Declaradas,
    IReadOnlyList<AreaSemPrioridade> NaoDeclaradas,
    int TotalDeMinutos);

public static class Revelacao
{
    public static Relatorio Comparar(IEnumerable<Prioridade> declaradas, IEnumerable<Registro> registros)
    {
        var prioridades = declaradas.OrderBy(p => p.Ordem).ToList();

        // 1) Soma o tempo por área.
        var porArea = registros
            .GroupBy(r => r.Area)
            .Select(g => new { Area = g.Key, Minutos = g.Sum(r => r.Minutos) })
            .ToList();
        int total = porArea.Sum(a => a.Minutos);

        // 2) GroupJoin: para cada prioridade, o grupo (possivelmente vazio) do tempo registrado nela.
        var comTempo = prioridades
            .GroupJoin(porArea, p => p.Area, a => a.Area,
                       (p, grupo) => new { p.Area, p.Ordem, Minutos = grupo.Sum(a => a.Minutos) })
            .ToList();

        // 3) A ordem observada entre as áreas declaradas: quem recebeu mais tempo vem primeiro.
        var observada = comTempo
            .OrderByDescending(x => x.Minutos)
            .ThenBy(x => x.Ordem)
            .Select((x, indice) => (x.Area, Ordem: indice + 1))
            .ToDictionary(x => x.Area, x => x.Ordem);

        var comparacoes = comTempo
            .Select(x => new Comparacao(x.Area, x.Ordem, observada[x.Area], x.Minutos, Parte(x.Minutos, total)))
            .ToList();

        // 4) Áreas com tempo registrado que ninguém declarou: ExceptBy compara só pela chave.
        var semPrioridade = porArea
            .ExceptBy(prioridades.Select(p => p.Area), a => a.Area)
            .OrderByDescending(a => a.Minutos)
            .Select(a => new AreaSemPrioridade(a.Area, a.Minutos, Parte(a.Minutos, total)))
            .ToList();

        return new Relatorio(comparacoes, semPrioridade, total);
    }

    private static double Parte(int minutos, int total) => total == 0 ? 0 : (double)minutos / total;

    /// <summary>Uma frase curiosa, não acusadora, para cada diferença.</summary>
    public static string Descrever(Comparacao c) => c.Diferenca switch
    {
        0   => $"{c.Area}: o que você disse e o que o calendário mostra caminham juntos ({c.Parte:P0} do tempo).",
        > 0 => $"{c.Area}: você a pôs em {c.OrdemDeclarada}º, mas ela aparece em {c.OrdemObservada}º ({c.Parte:P0} do tempo). O que está no caminho?",
        _   => $"{c.Area}: você a pôs em {c.OrdemDeclarada}º e ela aparece em {c.OrdemObservada}º ({c.Parte:P0} do tempo). Isso é uma escolha ou um acaso?"
    };
}
```

Acompanhe o raciocínio, passo a passo, no método `Comparar`:

1. **`GroupBy` + `Select`** soma o tempo por área.
2. **`GroupJoin`**: para cada prioridade, o grupo (possivelmente vazio) do tempo registrado nela; `grupo.Sum(...)` transforma o grupo vazio em **0**.
3. **`OrderByDescending(...).ThenBy(...).Select((x, indice) => ...)`**: a versão do `Select` que também entrega o **índice**. O primeiro da lista ordenada é a posição **1** na ordem *observada*.
4. **`ExceptBy(prioridades.Select(p => p.Area), a => a.Area)`**: as áreas que têm tempo e **não estão na lista declarada**. Elas aparecem à parte, e é frequente que sejam as mais interessantes ("redes sociais", "reuniões").

Também repare no cuidado com a **divisão por zero** em `Parte`: sem registros, o total é 0, e o método devolve 0 em vez de `NaN`.

```cs Uso
var declaradas = new[]
{
    new Prioridade("Família", 1), new Prioridade("Saúde", 2),
    new Prioridade("Estudo", 3), new Prioridade("Trabalho", 4),
};

var d = new DateOnly(2026, 11, 2);
var registros = new[]
{
    new Registro(d, "Trabalho", 600), new Registro(d, "Estudo", 200), new Registro(d, "Família", 120),
    new Registro(d, "Saúde", 30), new Registro(d, "Redes sociais", 150),
};

var relatorio = Revelacao.Comparar(declaradas, registros);

foreach (var c in relatorio.Declaradas)
    Console.WriteLine(Revelacao.Descrever(c));

foreach (var a in relatorio.NaoDeclaradas)
    Console.WriteLine($"Sem prioridade declarada: {a.Area} ({a.Parte:P0} do tempo)");
```

```saida
Família: você a pôs em 1º, mas ela aparece em 3º (11% do tempo). O que está no caminho?
Saúde: você a pôs em 2º, mas ela aparece em 4º (3% do tempo). O que está no caminho?
Estudo: você a pôs em 3º e ela aparece em 2º (18% do tempo). Isso é uma escolha ou um acaso?
Trabalho: você a pôs em 4º e ela aparece em 1º (55% do tempo). Isso é uma escolha ou um acaso?
Sem prioridade declarada: Redes sociais (14% do tempo)
```

Leia o resultado como um **convite**, não como um veredito. A família, que ocupa o 1º lugar na lista, aparece em 3º no tempo; o trabalho, 4º na lista, aparece em 1º. E 14% do tempo foi para "redes sociais", uma área que **ninguém tinha listado**. O programa não diz **por que**: pode haver razões muito boas (uma entrega, uma fase). Ele apenas **pergunta**.

::: dica Tom de voz importa em código
Veja `Revelacao.Descrever`: em vez de "você falhou com sua família", a frase pergunta "**o que está no caminho?**" e, quando a atividade ocupa mais do que o declarado, "**isso é uma escolha ou um acaso?**". O teste `A_descricao_pergunta_com_curiosidade_e_nao_acusa` garante que palavras como "culpa" e "fracasso" **nunca** apareçam. Mensagens de ferramentas também formam hábitos em quem as lê.
:::

## 2) Testes

```cs Habitos.Testes/Cap108Tests.cs
using Jornada.Habitos.Cap108;

namespace Habitos.Testes;

public class Cap108Tests
{
    private static readonly DateOnly Dia = new(2026, 11, 2);

    private static readonly Prioridade[] Declaradas =
    {
        new("Família", 1),
        new("Saúde", 2),
        new("Estudo", 3),
        new("Trabalho", 4),
    };

    private static Registro R(string area, int minutos) => new(Dia, area, minutos);

    [Fact]
    public void Compara_a_ordem_declarada_com_a_ordem_do_tempo_registrado()
    {
        var registros = new[] { R("Trabalho", 600), R("Estudo", 200), R("Família", 120), R("Saúde", 30), R("Trabalho", 50) };

        var relatorio = Revelacao.Comparar(Declaradas, registros);

        var familia = relatorio.Declaradas.Single(c => c.Area == "Família");
        Assert.Equal(1, familia.OrdemDeclarada);
        Assert.Equal(3, familia.OrdemObservada);
        Assert.Equal(2, familia.Diferenca);

        var trabalho = relatorio.Declaradas.Single(c => c.Area == "Trabalho");
        Assert.Equal(650, trabalho.Minutos);
        Assert.Equal(1, trabalho.OrdemObservada);
        Assert.Equal(1000, relatorio.TotalDeMinutos);
    }

    [Fact]
    public void Prioridade_sem_nenhum_registro_aparece_com_zero_minutos_em_vez_de_sumir()
    {
        var relatorio = Revelacao.Comparar(Declaradas, new[] { R("Trabalho", 100) });

        var saude = relatorio.Declaradas.Single(c => c.Area == "Saúde");
        Assert.Equal(0, saude.Minutos);
        Assert.Equal(0, saude.Parte);
        Assert.Equal(4, relatorio.Declaradas.Count);
    }

    [Fact]
    public void Areas_com_tempo_mas_sem_prioridade_declarada_sao_listadas_a_parte()
    {
        var registros = new[] { R("Trabalho", 300), R("Redes sociais", 200), R("Jogos", 100) };

        var relatorio = Revelacao.Comparar(Declaradas, registros);

        Assert.Equal(new[] { "Redes sociais", "Jogos" }, relatorio.NaoDeclaradas.Select(a => a.Area));
        Assert.Equal(200.0 / 600, relatorio.NaoDeclaradas[0].Parte, 6);
    }

    [Fact]
    public void Sem_registros_nao_ha_divisao_por_zero()
    {
        var relatorio = Revelacao.Comparar(Declaradas, Array.Empty<Registro>());
        Assert.Equal(0, relatorio.TotalDeMinutos);
        Assert.All(relatorio.Declaradas, c => Assert.Equal(0, c.Parte));
    }

    [Fact]
    public void A_descricao_pergunta_com_curiosidade_e_nao_acusa()
    {
        var descompasso = new Comparacao("Família", 1, 3, 120, 0.12);
        var alinhado = new Comparacao("Trabalho", 2, 2, 400, 0.4);
        var acima = new Comparacao("Estudo", 3, 1, 500, 0.5);

        Assert.Contains("O que está no caminho?", Revelacao.Descrever(descompasso));
        Assert.Contains("caminham juntos", Revelacao.Descrever(alinhado));
        Assert.Contains("escolha ou um acaso", Revelacao.Descrever(acima));

        foreach (var c in new[] { descompasso, alinhado, acima })
        {
            var texto = Revelacao.Descrever(c).ToLowerInvariant();
            Assert.DoesNotContain("culpa", texto);
            Assert.DoesNotContain("fracasso", texto);
        }
    }
}
```

O teste **`Prioridade_sem_nenhum_registro_aparece_com_zero_minutos_em_vez_de_sumir`** é a razão de ser do `GroupJoin`. E **`Sem_registros_nao_ha_divisao_por_zero`** protege de um erro clássico em porcentagens.

::: exercicio O maior descompasso
Escreva o método de extensão `MaiorDescompasso(this Relatorio relatorio)`, que devolve a `Comparacao` cuja posição declarada **mais difere** da observada (em valor absoluto), ou `null` se **tudo caminha junto** (todas as diferenças são zero). Use `MaxBy` e um padrão de propriedade.

--- resposta
```cs Jornada/Habitos/Exercicios/Cap108Descompasso.cs
namespace Jornada.Habitos.Cap108;

public static class RelatorioExtensoes
{
    /// <summary>A prioridade cuja posição declarada mais difere da observada. Se tudo caminha junto, não há descompasso (null).</summary>
    public static Comparacao? MaiorDescompasso(this Relatorio relatorio)
    {
        var candidata = relatorio.Declaradas.MaxBy(c => Math.Abs(c.Diferenca));
        return candidata is { Diferenca: not 0 } ? candidata : null;
    }
}
```

`MaxBy(c => Math.Abs(c.Diferenca))` acha o item com a maior diferença **absoluta** (a ordem pode estar acima ou abaixo do declarado). Depois, `candidata is { Diferenca: not 0 } ? candidata : null` usa um **padrão de propriedade** com `not` para devolver `null` quando o maior descompasso for zero, isto é, quando tudo está alinhado. Se a lista estiver vazia, `MaxBy` já devolve `null`.
:::

::: resumo
- Para o autor, o desejo mais profundo **se mostra nos hábitos**, e boa parte do que nos move opera **sob o radar** da consciência (automatismos). Isso pede **curiosidade**, não vergonha.
- Comparar **o que se diz** com **o que se faz** é uma leitura de fatos: a diferença é uma **pergunta**, e não uma sentença.
- **`GroupJoin`** junta cada item da esquerda ao grupo (talvez vazio) da direita: o que não tem registro **continua visível**. **`ExceptBy`** acha o que está em uma lista e não na outra. **`Select`** com índice numera.
- Proteja divisões por zero e escreva mensagens que **perguntam** em vez de acusar, e teste o tom.
- Se a comparação mexer demais com você, **pare** e converse com alguém; o programa é uma ferramenta, não um juiz.
:::

::: quiz
? O que o filme *Stalker* ilustra no capítulo do livro?
+ Que podemos hesitar diante de um desejo realizado, porque talvez não queiramos o que pensamos querer
- Que filmes russos são difíceis
- Que desejos são sempre conscientes
- Que a ficção científica prevê o futuro
> A Sala concede o desejo mais profundo, não o declarado.

? O que são "automatismos", segundo o capítulo?
+ Modos aprendidos de perceber e agir que operam sem pensarmos neles
- Programas de computador automáticos
- Decisões tomadas após muita reflexão
- Erros de digitação
> Alguns são escolhidos (dirigir, digitar); outros se infiltram sem intenção.

? Por que usamos `GroupJoin` e não `Join` para comparar prioridades com registros?
+ Para que uma prioridade sem nenhum registro continue na lista, com 0 minutos
- Porque `Join` não existe no LINQ
- Porque `GroupJoin` é mais rápido
- Porque `Join` troca a ordem
> O grupo vazio vira 0 com `Sum`; no `Join`, o item sumiria.

? O que faz `ExceptBy(prioridades.Select(p => p.Area), a => a.Area)`?
+ Devolve as áreas com tempo registrado que não estão na lista de prioridades
- Remove as áreas com mais tempo
- Ordena as áreas
- Soma os minutos
> Compara por uma chave e devolve o que "sobra".

? Qual é o tom esperado das mensagens do comparador?
+ Curioso: perguntas como "o que está no caminho?", sem acusação
- Severo, para motivar
- Neutro e sem nenhum texto
- Elogioso em qualquer caso
> Mensagens também formam hábitos em quem as lê.

? Se a comparação provoca vergonha ou ansiedade forte, o que o capítulo recomenda?
+ Pausar e conversar com alguém de confiança
- Insistir até se acostumar
- Apagar os registros e esquecer
- Ignorar o programa
> O programa é uma ferramenta, não substitui o cuidado.
:::

::: cartas
Automatismo :: Modo aprendido de perceber e agir que opera sem pensarmos nele.
Desejo revelado :: O que os hábitos e a rotina mostram querer, independentemente da declaração.
Iceberg :: Imagem: a ponta é o que decidimos conscientemente; o resto flutua sob a superfície.
GroupJoin :: LINQ que liga cada item da esquerda ao grupo (talvez vazio) de pares da direita.
Join :: LINQ que só devolve quem tem par nas duas listas.
ExceptBy :: LINQ que devolve o que está numa lista e não na outra, por uma chave.
Select com índice :: Versão do `Select` que também entrega a posição de cada item.
Curiosidade honesta :: Ler a diferença como pergunta, não como sentença.
:::
