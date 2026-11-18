---
id: amor-cap105
order: 137
kind: capitulo
num: 105
group: C# & Mentalidade · Parte 17 · Você é aquilo que ama, em C#
title: O que você quer?
subtitle: Antes de "o que você sabe?", vem "o que você deseja?". A ideia de que somos mais amantes do que coisas pensantes, e o que isso muda em como aprendemos. Em C#, é o capítulo dos enums de bits e das decisões movidas por pesos.
minutes: 60
hue: 345
objectives:
- Entender a tese do livro: **você é aquilo que ama**, e não só aquilo que sabe
- Reconhecer o **hiato entre saber e fazer** e por que mais informação não o fecha sozinha
- Modelar dimensões com um **`[Flags] enum`** e operadores bit a bit (`|`, `&`, `~`)
- Escrever um **decisor** em que o comportamento sai de desejos com peso, usando LINQ e `GetValueOrDefault`
origem: livros/08-voce-e-aquilo-que-ama-csharp/content/125-cap105.md
---

::: vila O curso que ninguém aplicou
No mês passado, a empresa pagou um curso de testes automatizados para toda a equipe. Foi ótimo: slides claros, exemplos bons, prova no final. Hoje, Caio olha o repositório e murmura: "ninguém escreveu um teste novo desde o curso". Lia pergunta: "o que cada um **quer** que aconteça na sexta-feira à tarde, quando a entrega está atrasada?". Silêncio. Todos **sabiam**. Ninguém **desejava**, naquele momento, parar para testar. O que o time queria, de verdade, era ver a tarefa saindo da lista.
:::

::: ilustracao img/amor/cap105.svg | Uma cabeça enorme e um corpo pequeno: a imagem que o autor critica. Somos também coração e mãos, e é aí que os hábitos moram.
:::

## O que o livro diz

Smith abre o livro com uma pergunta que, segundo ele, vem antes de todas as outras no caminho de quem quer mudar: **"o que você quer?"**. Não "o que você sabe?" nem "em que você acredita?". A razão, na leitura dele, é que **nós somos o que queremos**: nossos desejos ficam no centro da nossa identidade e **orientam** o que fazemos, muitas vezes sem passar pela reflexão.

A partir daí, ele critica um modelo de pessoa que muita gente assume sem perceber: o de **"coisa pensante"**. Nessa imagem, somos como um cérebro num palito: se mudarmos as **ideias**, mudamos o **comportamento**. É a lógica do curso de testes da nossa cena: informar, e esperar que o resto siga.

Os problemas desse modelo, segundo o autor, aparecem na vida de qualquer um:

- **O hiato entre saber e fazer.** Quase todo mundo já saiu de uma palestra decidido a mudar e, uma semana depois, estava igual. Ele lembra um ditado dos grupos de apoio a quem tenta largar um vício: *"seu melhor raciocínio o meteu nessa encrenca"*.
- **A educação "bancária".** Ele cita a crítica de educadores como Paulo Freire: tratar quem aprende como uma **conta** em que se depositam informações, para sacá-las quando necessário.
- **A tentação oposta.** Ele não propõe trocar a cabeça pelo sentimento ("siga seus impulsos"); propõe **acrescentar**, e não subtrair: conhecimento continua importando, mas fica **a serviço** de algo mais fundamental, o que desejamos.

Numa leitura de uma antiga carta de Paulo, ele destaca a ordem: primeiro o **amor** cresce, e então ele se torna uma **condição para discernir** o que importa. Em resumo: **não é só conhecer para amar; é também amar para conhecer**.

::: cuidado De onde vem, e até onde vale
Para o autor, o desejo humano mais profundo é dirigido a Deus, e essa é a base de todo o livro. Aqui, usamos a parte que vale para qualquer pessoa: **o desejo orienta mais que a informação, e é formado pelo que repetimos**. Não precisamos concordar sobre *o que* é o desejo mais profundo para observar que ele existe e que muda o que fazemos.
:::

::: analogia A bússola e o manual
Um manual de navegação explica tudo sobre ventos e correntes. Mas é a **bússola** que diz para onde o barco vai. Quem lê o manual sem bússola sabe muito e não chega a lugar nenhum; quem tem bússola e nenhum manual chega, mesmo que devagar. O ideal é ter os dois, só que **a bússola vem primeiro**.
:::

## Da vida para o código

| No livro | No dia a dia de quem programa |
|---|---|
| "Coisa pensante" × "amante" | Equipe que **sabe** boas práticas × equipe que **quer** aplicá-las |
| Cabeça, coração, mãos | Estudar (cabeça) · querer (coração) · repetir na prática (mãos) |
| Hiato entre saber e fazer | Curso concluído, repositório igual |
| O desejo orienta o comportamento | Escolhas na sexta à tarde saem do que se quer naquele momento |
| "Acrescentar, não subtrair" | Um plano de aprendizado que **soma** as três dimensões |

::: figura Um plano só de cabeça deixa duas regiões de fora
<svg viewBox="0 0 740 220" role="img" aria-label="Dois planos de aprendizado: o primeiro só envolve a cabeça; o segundo envolve cabeça, coração e mãos">
  <text x="185" y="26" text-anchor="middle" font-weight="800">Plano A: só informação</text>
  <rect x="40" y="44" width="90" height="60" rx="10" class="f-info"/><text x="85" y="80" text-anchor="middle" font-size="13">Cabeça ✓</text>
  <rect x="140" y="44" width="90" height="60" rx="10" class="f-bad" opacity=".45"/><text x="185" y="80" text-anchor="middle" font-size="13">Coração ✗</text>
  <rect x="240" y="44" width="90" height="60" rx="10" class="f-bad" opacity=".45"/><text x="285" y="80" text-anchor="middle" font-size="13">Mãos ✗</text>
  <text x="185" y="132" text-anchor="middle" font-size="12" class="t-dim">curso em vídeo · leitura · palestra</text>
  <text x="555" y="26" text-anchor="middle" font-weight="800">Plano B: inteiro</text>
  <rect x="410" y="44" width="90" height="60" rx="10" class="f-info"/><text x="455" y="80" text-anchor="middle" font-size="13">Cabeça ✓</text>
  <rect x="510" y="44" width="90" height="60" rx="10" class="f-good"/><text x="555" y="80" text-anchor="middle" font-size="13">Coração ✓</text>
  <rect x="610" y="44" width="90" height="60" rx="10" class="f-good"/><text x="655" y="80" text-anchor="middle" font-size="13">Mãos ✓</text>
  <text x="555" y="132" text-anchor="middle" font-size="12" class="t-dim">estudo + um exemplo que inspira + prática semanal pequena</text>
  <text x="370" y="196" text-anchor="middle" font-size="13" class="t-dim">Informação importa, mas sozinha costuma não mudar o que se faz na sexta à tarde.</text>
</svg>
:::

## 1) Três dimensões em um único número: `[Flags] enum`

Para representar "o que uma atividade de aprendizado envolve", precisamos de uma coisa que pode ser **várias coisas ao mesmo tempo**: uma atividade pode tocar a cabeça **e** as mãos. Um `enum` comum guarda **um** valor. Com o atributo **`[Flags]`**, cada valor vira um **bit** (1, 2, 4, 8...), e vários cabem no mesmo número:

| `Dimensao` | Valor | Em binário |
|---|---|---|
| `Cabeca` | 1 | `001` |
| `Coracao` | 2 | `010` |
| `Maos` | 4 | `100` |
| Cabeça e mãos juntas | 5 | `101` |

Os operadores **bit a bit** trabalham sobre esses bits:

- **`|` (ou)** junta: `Cabeca | Maos` liga os dois bits.
- **`&` (e)** interseciona: `Todas & Cabeca` fica só com a cabeça.
- **`~` (negação)** inverte: `~Cobertura` é "tudo que **não** está coberto".
- **`HasFlag`** pergunta: "este bit está ligado?".

```cs Jornada/Habitos/Cap105/Aprendizado.cs
namespace Jornada.Habitos.Cap105;

/// <summary>As três "regiões" de uma pessoa: cabeça (saber), coração (querer) e mãos (praticar).</summary>
[Flags]
public enum Dimensao
{
    Nenhuma = 0,
    Cabeca  = 1,        // conhecimento
    Coracao = 2,        // desejo
    Maos    = 4,        // prática
    Todas   = Cabeca | Coracao | Maos
}

public sealed record AtividadeDeAprendizado(string Nome, Dimensao Envolve);

public sealed class PlanoDeAprendizado
{
    private readonly List<AtividadeDeAprendizado> atividades = new();

    public PlanoDeAprendizado Adicionar(string nome, Dimensao envolve)
    {
        atividades.Add(new AtividadeDeAprendizado(nome, envolve));
        return this;
    }

    /// <summary>União (OR) das dimensões de todas as atividades.</summary>
    public Dimensao Cobertura() =>
        atividades.Aggregate(Dimensao.Nenhuma, (acumulado, a) => acumulado | a.Envolve);

    /// <summary>O que ainda não é tocado por nenhuma atividade: tudo, menos o que já está coberto.</summary>
    public Dimensao Faltando() => Dimensao.Todas & ~Cobertura();

    public bool EnvolveSo(Dimensao dimensao) => atividades.All(a => a.Envolve == dimensao);

    public IEnumerable<string> Sugestoes()
    {
        var falta = Faltando();

        if (falta.HasFlag(Dimensao.Cabeca))
            yield return "Falta estudo: alguma leitura ou aula que dê nome e modelo ao assunto?";
        if (falta.HasFlag(Dimensao.Coracao))
            yield return "Falta desejo: o que faz você querer isto? Há uma história, um exemplo, alguém que inspire?";
        if (falta.HasFlag(Dimensao.Maos))
            yield return "Falta prática: o que você pode repetir toda semana, em tamanho pequeno?";
    }
}
```

Repare em duas linhas:

- **`Aggregate(Dimensao.Nenhuma, (acumulado, a) => acumulado | a.Envolve)`** é uma "dobra": começa de `Nenhuma` e vai juntando, atividade por atividade, as dimensões com `|`. No fim, tem a **união** de tudo.
- **`Dimensao.Todas & ~Cobertura()`** calcula o que **falta**: de "todas", tira o que já está coberto. É a mesma lógica de conjuntos, só que em bits.

O método `Sugestoes` usa `yield return` (você o viu no Livro 6) para devolver **uma pergunta curiosa** para cada dimensão ausente, e **não** uma cobrança.

## 2) Quando o comportamento sai do que se quer

Agora, a ideia central do capítulo em código. Um `Desejo` tem um **peso** (o quanto aquilo importa naquele momento). Uma `Opcao` diz o quanto **atrai** para cada desejo. O `Decisor` dá nota a cada opção somando `peso × atrativo` e **escolhe a maior**.

Repare no que **não existe** no código: nenhum campo de "conhecimento". O comportamento sai **só** do que se quer. É uma simplificação de propósito, para tornar visível o argumento do autor: a mesma pessoa, com o mesmo conhecimento, age diferente quando **o que ela quer** muda.

```cs Jornada/Habitos/Cap105/Decisor.cs
namespace Jornada.Habitos.Cap105;

/// <summary>Algo que a pessoa quer, e o quanto (peso de 0 a 1).</summary>
public sealed record Desejo(string Nome, double Peso);

/// <summary>Uma opção na mesa e o quanto ela "atrai" para cada desejo (0 a 1).</summary>
public sealed record Opcao(string Nome, IReadOnlyDictionary<string, double> Atrativos);

public sealed record Pontuacao(string Opcao, double Pontos);

public static class Decisor
{
    /// <summary>
    /// Pontua cada opção pela soma de (peso do desejo × atrativo da opção para aquele desejo).
    /// Repare: não existe aqui nenhum "conhecimento". O comportamento sai do que se quer.
    /// </summary>
    public static IReadOnlyList<Pontuacao> Ranquear(IEnumerable<Desejo> desejos, IEnumerable<Opcao> opcoes)
    {
        var lista = desejos.ToList();

        return opcoes
            .Select(o => new Pontuacao(o.Nome, lista.Sum(d => d.Peso * o.Atrativos.GetValueOrDefault(d.Nome))))
            .OrderByDescending(p => p.Pontos)
            .ThenBy(p => p.Opcao)
            .ToList();
    }

    public static string Escolher(IEnumerable<Desejo> desejos, IEnumerable<Opcao> opcoes) =>
        Ranquear(desejos, opcoes).FirstOrDefault()?.Opcao
        ?? throw new InvalidOperationException("Sem opções para escolher.");
}
```

Detalhes de C# que valem atenção:

- **`o.Atrativos.GetValueOrDefault(d.Nome)`**: se a opção não menciona aquele desejo, vale **0** em vez de lançar erro. É o jeito elegante de ler um dicionário que pode não ter a chave.
- **`OrderByDescending(...).ThenBy(...)`**: ordena pelo maior primeiro e, no empate, pelo **nome**. Isso torna o resultado **determinístico**: o mesmo conjunto de dados sempre dá o mesmo ranking.
- **`?? throw ...`**: se não há nenhuma opção, não existe escolha, e dizemos isso com uma exceção clara.

```cs Uso
var plano = new PlanoDeAprendizado()
    .Adicionar("Ler o livro", Dimensao.Cabeca)
    .Adicionar("Assistir à palestra", Dimensao.Cabeca);

Console.WriteLine($"Cobertura: {plano.Cobertura()}");
Console.WriteLine($"Falta: {plano.Faltando()}");
foreach (var sugestao in plano.Sugestoes())
    Console.WriteLine("- " + sugestao);

var noite = new[]
{
    new Opcao("Estudar mais um tutorial", new Dictionary<string, double> { ["ser bom no que faço"] = 0.7, ["descansar"] = 0.1 }),
    new Opcao("Ver série", new Dictionary<string, double> { ["descansar"] = 0.9 }),
    new Opcao("Treinar um kata", new Dictionary<string, double> { ["ser bom no que faço"] = 0.8, ["descansar"] = 0.2 }),
};

var querCrescer = new[] { new Desejo("ser bom no que faço", 0.9), new Desejo("descansar", 0.2) };
var querDescansar = new[] { new Desejo("ser bom no que faço", 0.2), new Desejo("descansar", 0.9) };

Console.WriteLine($"Quem quer crescer escolhe: {Decisor.Escolher(querCrescer, noite)}");
Console.WriteLine($"Quem quer descansar escolhe: {Decisor.Escolher(querDescansar, noite)}");

foreach (var p in Decisor.Ranquear(querDescansar, noite))
    Console.WriteLine($"  {p.Opcao}: {p.Pontos:0.00}");
```

```saida
Cobertura: Cabeca
Falta: Coracao, Maos
- Falta desejo: o que faz você querer isto? Há uma história, um exemplo, alguém que inspire?
- Falta prática: o que você pode repetir toda semana, em tamanho pequeno?
Quem quer crescer escolhe: Treinar um kata
Quem quer descansar escolhe: Ver série
  Ver série: 0,81
  Treinar um kata: 0,34
  Estudar mais um tutorial: 0,23
```

Leia a saída: com **as mesmas três opções**, quem quer crescer escolhe treinar, e quem quer descansar escolhe a série. Nenhum dos dois "não sabe" que treinar faz bem. O que muda é **o peso do que cada um deseja naquele momento**. É a diferença entre o curso (informar) e o que de fato muda o comportamento (formar o desejo, como veremos nos próximos capítulos).

::: cuidado Um modelo não é uma pessoa
Esse decisor é uma **caricatura útil**: gente de verdade tem desejos que se contradizem, mudam no meio do dia e nem sempre conhece os próprios. Ele serve para **enxergar um argumento**, não para prever o que alguém fará. E repare: nada aqui diz que a pessoa que escolhe a série "errou". Descansar também é um desejo legítimo.
:::

## 3) Testes

```cs Habitos.Testes/Cap105Tests.cs
using Jornada.Habitos.Cap105;

namespace Habitos.Testes;

public class Cap105Tests
{
    [Fact]
    public void Cobertura_junta_as_dimensoes_de_todas_as_atividades()
    {
        var plano = new PlanoDeAprendizado()
            .Adicionar("Curso em vídeo", Dimensao.Cabeca)
            .Adicionar("Projeto pessoal", Dimensao.Maos | Dimensao.Coracao);

        Assert.Equal(Dimensao.Todas, plano.Cobertura());
        Assert.Equal(Dimensao.Nenhuma, plano.Faltando());
    }

    [Fact]
    public void Um_plano_so_de_informacao_deixa_coracao_e_maos_de_fora()
    {
        var plano = new PlanoDeAprendizado()
            .Adicionar("Ler o livro", Dimensao.Cabeca)
            .Adicionar("Assistir à palestra", Dimensao.Cabeca);

        Assert.True(plano.EnvolveSo(Dimensao.Cabeca));
        Assert.Equal(Dimensao.Coracao | Dimensao.Maos, plano.Faltando());
        Assert.Equal(2, plano.Sugestoes().Count());
    }

    [Fact]
    public void Plano_vazio_pede_as_tres_dimensoes()
    {
        var sugestoes = new PlanoDeAprendizado().Sugestoes().ToList();
        Assert.Equal(3, sugestoes.Count);
        Assert.Contains(sugestoes, s => s.Contains("prática"));
    }

    private static readonly Opcao[] Noite =
    {
        new("Estudar mais um tutorial", new Dictionary<string, double> { ["ser bom no que faço"] = 0.7, ["descansar"] = 0.1 }),
        new("Ver série", new Dictionary<string, double> { ["descansar"] = 0.9 }),
        new("Treinar um kata", new Dictionary<string, double> { ["ser bom no que faço"] = 0.8, ["descansar"] = 0.2 }),
    };

    [Fact]
    public void O_mesmo_conjunto_de_opcoes_da_resultados_diferentes_conforme_o_que_se_quer()
    {
        var quemQuerCrescer = new[] { new Desejo("ser bom no que faço", 0.9), new Desejo("descansar", 0.2) };
        var quemQuerDescansar = new[] { new Desejo("ser bom no que faço", 0.2), new Desejo("descansar", 0.9) };

        Assert.Equal("Treinar um kata", Decisor.Escolher(quemQuerCrescer, Noite));
        Assert.Equal("Ver série", Decisor.Escolher(quemQuerDescansar, Noite));
    }

    [Fact]
    public void O_ranking_vem_do_maior_para_o_menor_e_desempata_pelo_nome()
    {
        var iguais = new[]
        {
            new Opcao("B", new Dictionary<string, double> { ["x"] = 0.5 }),
            new Opcao("A", new Dictionary<string, double> { ["x"] = 0.5 }),
        };

        var ranking = Decisor.Ranquear(new[] { new Desejo("x", 1) }, iguais);

        Assert.Equal(new[] { "A", "B" }, ranking.Select(p => p.Opcao));
    }

    [Fact]
    public void Atrativo_ausente_vale_zero_e_sem_opcoes_nao_ha_escolha()
    {
        var ranking = Decisor.Ranquear(new[] { new Desejo("y", 1) }, new[] { new Opcao("A", new Dictionary<string, double>()) });
        Assert.Equal(0, ranking[0].Pontos);
        Assert.Throws<InvalidOperationException>(() => Decisor.Escolher(new[] { new Desejo("y", 1) }, Array.Empty<Opcao>()));
    }
}
```

O teste **`O_mesmo_conjunto_de_opcoes_da_resultados_diferentes_conforme_o_que_se_quer`** é a tese do capítulo em forma executável. E **`O_ranking_vem_do_maior_para_o_menor_e_desempata_pelo_nome`** garante o determinismo.

::: exercicio Um resumo das três dimensões
Escreva o método de extensão `Resumo(this PlanoDeAprendizado plano)`, que devolve uma linha como `"Cabeça ✓ · Coração ✗ · Mãos ✓"`, marcando o que o plano **cobre** e o que **não cobre**. Use `Cobertura()` e `HasFlag`. Um plano vazio deve devolver três ✗.

--- resposta
```cs Jornada/Habitos/Exercicios/Cap105Resumo.cs
namespace Jornada.Habitos.Cap105;

public static class PlanoExtensoes
{
    /// <summary>Uma linha com as três dimensões e um ✓ ou ✗ em cada: "Cabeça ✓ · Coração ✗ · Mãos ✓".</summary>
    public static string Resumo(this PlanoDeAprendizado plano)
    {
        var coberto = plano.Cobertura();

        string Marca(Dimensao dimensao, string nome) => $"{nome} {(coberto.HasFlag(dimensao) ? "✓" : "✗")}";

        return string.Join(" · ", Marca(Dimensao.Cabeca, "Cabeça"), Marca(Dimensao.Coracao, "Coração"), Marca(Dimensao.Maos, "Mãos"));
    }
}
```

A **função local** `Marca` evita repetir a mesma lógica três vezes: recebe a dimensão e o nome e devolve `"Cabeça ✓"` ou `"Cabeça ✗"`. E `string.Join(" · ", ...)` junta os pedaços com o separador, sem precisar de laço.
:::

::: resumo
- Para Smith, **você é aquilo que ama**: o desejo orienta o comportamento mais que a informação, e a pergunta que vem antes é "o que você quer?".
- O modelo de **"coisa pensante"** (mudar ideias muda a pessoa) falha no **hiato entre saber e fazer**; a proposta é **acrescentar** coração e mãos, e não tirar a cabeça.
- **`[Flags] enum`** guarda várias opções em um número: `|` junta, `&` interseciona, `~` inverte, `HasFlag` pergunta.
- **`Aggregate`** "dobra" uma coleção em um único valor; **`GetValueOrDefault`** lê dicionários sem medo de chaves ausentes.
- Um modelo em que o comportamento sai de **desejos com peso** deixa visível o argumento, **sem** pretender prever pessoas.
:::

::: quiz
? Qual é a pergunta que, para o autor, vem antes de "o que você sabe?"
+ O que você quer?
- Em que você acredita?
- Quanto você já estudou?
- Quem pode ajudar?
> A tese é que somos o que desejamos e que o desejo orienta nosso comportamento.

? O que é o "hiato entre saber e fazer"?
+ A distância entre o que sabemos que deveríamos fazer e o que realmente fazemos
- A falta de conhecimento sobre um assunto
- Um erro de compilação
- O tempo entre duas reuniões
> Mais informação, sozinha, costuma não fechar esse hiato.

? O que `Dimensao.Todas & ~Cobertura()` calcula?
+ As dimensões que ainda não estão cobertas
- As dimensões cobertas
- O número de atividades
- A soma dos valores do enum
> `~` inverte os bits da cobertura; `&` com `Todas` deixa só o que falta.

? Para que serve o atributo `[Flags]` em um `enum`?
+ Para que vários valores possam ser combinados em um só, usando bits
- Para impedir valores repetidos
- Para tornar o enum mais rápido
- Para gerar texto automaticamente
> Cada valor é uma potência de 2, e a combinação usa `|`.

? No `Decisor`, por que o resultado é determinístico?
+ Porque o empate é desfeito por um critério fixo (o nome)
- Porque não há desejos
- Porque usa números aleatórios
- Porque ignora os pesos
> Mesmos dados, mesmo ranking: é o que torna o código testável.

? O que o decisor do capítulo deve ser usado para fazer?
+ Tornar visível um argumento, sem pretender prever o que alguém fará
- Avaliar o caráter das pessoas
- Substituir a decisão de cada um
- Medir a força de vontade
> É uma caricatura útil, não um instrumento de avaliação.
:::

::: cartas
Coisa pensante :: Modelo de pessoa que a reduz ao que sabe; para o autor, incompleto.
Hiato entre saber e fazer :: Distância entre conhecer o certo e praticá-lo.
Educação "bancária" :: Tratar quem aprende como conta de depósito de informações.
[Flags] :: Atributo que permite combinar valores de um enum, bit a bit.
Operador | (ou) :: Liga os bits de ambos os lados: junta opções.
Operador & (e) :: Mantém só os bits ligados dos dois lados: interseção.
Operador ~ (não) :: Inverte todos os bits.
Aggregate :: LINQ que "dobra" uma sequência em um valor acumulado.
:::
