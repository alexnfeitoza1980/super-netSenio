---
id: resiliencia-cap92
order: 118
kind: capitulo
num: 92
group: C# & Mentalidade · Parte 15 · Resiliência em C#
title: Pequenas provas de que você consegue aprender
subtitle: Autoeficácia cresce em passos do tamanho certo, com provas pequenas de que dá para fazer. Em C#, é o capítulo da classe bem protegida, dos iteradores e de um limite que ajuda.
minutes: 60
hue: 170
objectives:
- Entender **autoeficácia** como confiança **em uma tarefa específica** (e não "dou conta de tudo")
- Projetar uma classe com **encapsulamento**: campos privados, `IReadOnlyList`, `internal`
- Escrever um **iterador** com `yield return` que divide uma tarefa grande em passos
- Usar **constantes**, validação e **tuplas** para um modelo pequeno e claro
origem: livros/06-resiliencia-csharp/content/129-cap92.md
---

::: vila A primeira bicicleta
Quando Beto aprendeu a andar de bicicleta, ninguém disse "ande até a esquina". Primeiro, ele só sentou e pisou o pedal com os dois pés no chão. Depois, deslizou. Depois, equilibrou por três segundos. No dia em que chegou à esquina, a esquina era **a décima pequena prova** de que ele conseguia. Hoje, diante de um projeto de 40 horas, Beto trava. A Lia pergunta: "qual é a **primeira pedalada**?".
:::

::: ilustracao img/resiliencia/cap92.svg | A confiança cresce em degraus do tamanho dos pés. Cada passo concluído é uma pequena prova de que dá para aprender.
:::

## O que o livro diz

O texto sobre resiliência chama a atenção para a **autoeficácia**: a **crença de que podemos participar da solução** de um desafio. Na nossa leitura, três pontos são úteis:

1. A autoeficácia é **específica**: posso confiar em mim para **aprender SQL** e ainda duvidar de mim para **falar em público**. Não é uma "nota geral".
2. Ela **cresce com experiências**: cada vez que fazemos algo difícil, ainda que pequeno, ganhamos uma **prova**. Observar outras pessoas conseguindo e receber incentivo verdadeiro também ajudam.
3. Ela **não é** "eu dou conta de tudo": quem acredita que pode participar da solução também sabe **pedir ajuda** quando precisa.

::: cuidado Autoeficácia não é "pensamento mágico"
Acreditar que consegue **não faz** o problema sumir, e a falta de confiança **não** é defeito de caráter: costuma refletir poucas experiências de sucesso em **aquela** tarefa, ou muita cobrança junto. Por isso, o caminho não é "acredite mais em você", e sim **criar provas pequenas, reais e repetíveis**.
:::

::: ilustracao img/resiliencia/photos/trilha-na-floresta.jpg | A trilha não se percorre de uma vez: cada trecho iluminado é uma pequena prova de que dá para seguir.
:::

## Da vida para o código

| Na vida | Em C# |
|---|---|
| Uma meta com nome | Classe `Meta` com **validação** no construtor |
| Passos pequenos, de até 25 min | **`const`** `LimiteDeMinutosPorPasso` e validação em `AdicionarPasso` |
| "Dividir o monstro em pedaços" | Iterador **`yield return`** em `Dividir` |
| Só quem organiza a meta pode concluir um passo | **`internal`** e `private set` (encapsulamento) |
| Cada passo concluído é uma prova | `record Prova(Dia, Descricao)` |
| "Quanto já andei?" | Propriedade que devolve uma **tupla** `(Feitos, Total)` |

::: figura Do "tudo" para o próximo passo
<svg viewBox="0 0 740 230" role="img" aria-label="Uma meta enorme dividida em quatro passos de até 25 minutos, cada um virando uma pequena prova">
  <rect x="20" y="80" width="170" height="70" rx="12" class="f-bad"/><text x="105" y="108" text-anchor="middle" font-weight="800">"Estudar tudo"</text><text x="105" y="130" text-anchor="middle" font-size="12" class="t-dim">100 min: paralisa</text>
  <path class="arrow" d="M192 115H240"/>
  <g font-size="12" text-anchor="middle" font-weight="800">
    <rect x="245" y="40" width="110" height="46" rx="10" class="f-good"/><text x="300" y="68">parte 1 · 25 min</text>
    <rect x="365" y="40" width="110" height="46" rx="10" class="f-good"/><text x="420" y="68">parte 2 · 25 min</text>
    <rect x="485" y="40" width="110" height="46" rx="10" class="f-info"/><text x="540" y="68">parte 3 · 25 min</text>
    <rect x="605" y="40" width="110" height="46" rx="10" class="f-info"/><text x="660" y="68">parte 4 · 25 min</text>
  </g>
  <g font-size="12" text-anchor="middle" class="t-dim">
    <text x="300" y="108">✔ prova 1</text><text x="420" y="108">✔ prova 2</text><text x="540" y="108">próximo</text><text x="660" y="108">depois</text>
  </g>
  <text x="480" y="170" text-anchor="middle" font-size="14" font-weight="800">Um passo por vez, e cada passo conta.</text>
  <text x="480" y="192" text-anchor="middle" font-size="12" class="t-dim">Se o passo parece grande demais, ele é grande demais: divida de novo.</text>
</svg>
:::

## 1) Uma classe que protege o que importa

Começamos pelo **encapsulamento**: esconder os detalhes por trás de uma interface pequena e segura. Na classe `Passo`, o `Concluido` só pode ser **lido** por fora (`private set`) e só pode ser **marcado** por quem está no mesmo projeto (`internal`): ninguém, de fora, "conclui" um passo sem passar pela `Meta`. Na `Meta`, as listas são **privadas** e a tela vê apenas `IReadOnlyList`, uma visão de **leitura**.

O método `AdicionarPasso` guarda a **regra do capítulo**: um passo precisa caber em 25 minutos. Se passar, o erro **ensina o caminho**: "Divida: use AdicionarDividindo". Mensagem de erro boa é uma pequena aula.

```cs Jornada/Resiliencia/Cap92/Meta.cs
namespace Jornada.Resiliencia.Cap92;

public sealed class Passo
{
    public string Descricao { get; }
    public int Minutos { get; }
    public bool Concluido { get; private set; }

    public Passo(string descricao, int minutos)
    {
        Descricao = descricao;
        Minutos = minutos;
    }

    internal void Marcar() => Concluido = true;      // só a Meta pode concluir um passo
}

public sealed record Prova(DateOnly Dia, string Descricao);

public sealed class Meta
{
    public const int LimiteDeMinutosPorPasso = 25;

    private readonly List<Passo> passos = new();
    private readonly List<Prova> provas = new();

    public string Titulo { get; }

    public Meta(string titulo)
    {
        if (string.IsNullOrWhiteSpace(titulo))
            throw new ArgumentException("Dê um nome à meta.", nameof(titulo));
        Titulo = titulo.Trim();
    }

    public IReadOnlyList<Passo> Passos => passos;
    public IReadOnlyList<Prova> Provas => provas;

    public Passo? ProximoPasso => passos.FirstOrDefault(p => !p.Concluido);

    public (int Feitos, int Total) Andamento => (passos.Count(p => p.Concluido), passos.Count);

    public Meta AdicionarPasso(string descricao, int minutos)
    {
        if (string.IsNullOrWhiteSpace(descricao))
            throw new ArgumentException("Descreva o passo.", nameof(descricao));
        if (minutos <= 0)
            throw new ArgumentOutOfRangeException(nameof(minutos), "O passo precisa de um tempo maior que zero.");
        if (minutos > LimiteDeMinutosPorPasso)
            throw new ArgumentException(
                $"Um passo precisa caber em {LimiteDeMinutosPorPasso} min. Divida: use AdicionarDividindo.", nameof(minutos));

        passos.Add(new Passo(descricao.Trim(), minutos));
        return this;
    }

    /// <summary>Recebe uma tarefa grande e a quebra em passos que cabem no limite.</summary>
    public Meta AdicionarDividindo(string tarefa, int minutosTotais)
    {
        foreach (var (descricao, minutos) in Dividir(tarefa, minutosTotais))
            AdicionarPasso(descricao, minutos);
        return this;
    }

    public void ConcluirProximo(DateOnly hoje)
    {
        var passo = ProximoPasso ?? throw new InvalidOperationException("Não há passo pendente.");
        passo.Marcar();
        provas.Add(new Prova(hoje, $"Concluí: {passo.Descricao}"));    // cada passo vira uma pequena prova
    }

    /// <summary>Iterador: devolve um passo de cada vez, sem montar a lista inteira antes.</summary>
    public static IEnumerable<(string Descricao, int Minutos)> Dividir(string tarefa, int minutosTotais)
    {
        int partes = (int)Math.Ceiling(minutosTotais / (double)LimiteDeMinutosPorPasso);
        int restante = minutosTotais;

        for (int parte = 1; restante > 0; parte++)
        {
            int minutos = Math.Min(LimiteDeMinutosPorPasso, restante);
            yield return ($"{tarefa} (parte {parte} de {partes})", minutos);
            restante -= minutos;
        }
    }
}
```

## 2) Dividir com `yield return`

O método `Dividir` é um **iterador**: em vez de montar uma lista inteira e devolvê-la, ele **entrega um passo de cada vez** com `yield return`. Quem consome (aqui, o `foreach` de `AdicionarDividindo`) pede o próximo, o método **retoma de onde parou**, entrega outro, e assim por diante, até o laço terminar. É preguiçoso (só trabalha quando pedem) e fácil de ler.

Dentro do `foreach`, **`var (descricao, minutos)`** é uma **desconstrução de tupla**: o item devolvido tem dois valores nomeados e já os separamos em duas variáveis.

```cs Uso
var meta = new Meta("Estudar para a prova")
    .AdicionarDividindo("Resolver a lista de exercícios", 60)
    .AdicionarPasso("Revisar as anotações", 15);

var hoje = new DateOnly(2026, 11, 2);
meta.ConcluirProximo(hoje);
meta.ConcluirProximo(hoje);

var (feitos, total) = meta.Andamento;
Console.WriteLine($"Andamento: {feitos} de {total}");
Console.WriteLine($"Próximo: {meta.ProximoPasso?.Descricao} ({meta.ProximoPasso?.Minutos} min)");

foreach (var prova in meta.Provas)
    Console.WriteLine($"{prova.Dia:dd/MM}: {prova.Descricao}");
```

```saida
Andamento: 2 de 4
Próximo: Resolver a lista de exercícios (parte 3 de 3) (10 min)
02/11: Concluí: Resolver a lista de exercícios (parte 1 de 3)
02/11: Concluí: Resolver a lista de exercícios (parte 2 de 3)
```

Repare no `Andamento`: `2 de 4`. **Não há porcentagem, nem nota, nem barra vermelha.** O programa registra provas ("Concluí: ..."), que a pessoa pode reler quando precisar lembrar do que **já** conseguiu.

::: dica Para que serve o `const`?
`public const int LimiteDeMinutosPorPasso = 25;` é um **número com nome**, definido em um só lugar. Se um dia a regra mudar para 30, a mudança é de **uma linha**. E, ao ler o código, `LimiteDeMinutosPorPasso` explica o que o 25 significa, ao contrário de um "25" solto.
:::

## 3) Testes

```cs Resiliencia.Testes/Cap92Tests.cs
using Jornada.Resiliencia.Cap92;

namespace Resiliencia.Testes;

public class Cap92Tests
{
    private static readonly DateOnly Hoje = new(2026, 11, 2);

    [Fact]
    public void Passo_grande_demais_e_recusado_com_uma_dica()
    {
        var meta = new Meta("Estudar para a prova");
        var e = Assert.Throws<ArgumentException>(() => meta.AdicionarPasso("Estudar tudo", 120));
        Assert.Contains("Divida", e.Message);
    }

    [Fact]
    public void Dividir_gera_passos_dentro_do_limite()
    {
        var partes = Meta.Dividir("Ler o capítulo", 60).ToList();

        Assert.Equal(3, partes.Count);
        Assert.Equal(new[] { 25, 25, 10 }, partes.Select(p => p.Minutos));
        Assert.Equal("Ler o capítulo (parte 1 de 3)", partes[0].Descricao);
    }

    [Fact]
    public void Concluir_cria_uma_pequena_prova_e_avanca_o_andamento()
    {
        var meta = new Meta("Estudar").AdicionarDividindo("Resolver exercícios", 50);

        Assert.Equal((0, 2), meta.Andamento);
        meta.ConcluirProximo(Hoje);

        Assert.Equal((1, 2), meta.Andamento);
        Assert.Single(meta.Provas);
        Assert.Equal("Resolver exercícios (parte 2 de 2)", meta.ProximoPasso!.Descricao);
    }

    [Fact]
    public void Sem_passo_pendente_nao_ha_o_que_concluir()
    {
        var meta = new Meta("Nada a fazer");
        Assert.Null(meta.ProximoPasso);
        Assert.Throws<InvalidOperationException>(() => meta.ConcluirProximo(Hoje));
    }

    [Theory]
    [InlineData("", 10)]
    [InlineData("ok", 0)]
    public void Passo_invalido_e_recusado(string descricao, int minutos) =>
        Assert.ThrowsAny<ArgumentException>(() => new Meta("M").AdicionarPasso(descricao, minutos));
}
```

Veja `Passo_grande_demais_e_recusado_com_uma_dica`: o teste verifica não só que a exceção acontece, mas que a **mensagem ajuda** ("Divida"). Mensagem de erro também é parte do comportamento prometido.

::: exercicio Quanto falta?
Escreva dois métodos de extensão para `Meta`: `MinutosRestantes()` (soma dos minutos dos passos **ainda não concluídos**) e `Estimativa()`, que devolve `"Nada pendente."` se zero, `"Cabe em um único passo."` se até 25 minutos, e `"Cerca de X min no total, em passos de até 25 min."` nos demais casos. Use um `switch` expression.

--- resposta
```cs Jornada/Resiliencia/Exercicios/Cap92Restante.cs
namespace Jornada.Resiliencia.Cap92;

public static class MetaExtensoes
{
    public static int MinutosRestantes(this Meta meta) =>
        meta.Passos.Where(p => !p.Concluido).Sum(p => p.Minutos);

    public static string Estimativa(this Meta meta) => meta.MinutosRestantes() switch
    {
        0       => "Nada pendente.",
        <= 25   => "Cabe em um único passo.",
        var m   => $"Cerca de {m} min no total, em passos de até {Meta.LimiteDeMinutosPorPasso} min."
    };
}
```

O padrão `<= 25` é relacional e `var m` **captura** o valor para usar na mensagem. A soma usa `Where` e `Sum` do LINQ, e conta **só o que falta**: assim, a estimativa **encolhe** conforme as provas se acumulam, o oposto de uma cobrança.
:::

::: resumo
- **Autoeficácia** é a crença de poder participar da solução de **uma tarefa específica**; cresce com **provas pequenas**, não com cobrança.
- **Encapsulamento**: campos privados, `private set`, `internal` e `IReadOnlyList` protegem as regras do modelo.
- Um **limite** (aqui, 25 min por passo) com `const` e mensagem de erro que **ensina** transforma o modelo em um guia.
- `yield return` cria **iteradores** preguiçosos; a **desconstrução de tuplas** separa os valores em variáveis.
- O modelo **registra provas**, não notas: o andamento é `feitos de total`, sem porcentagem nem ranking.
:::

::: quiz
? O que é autoeficácia, segundo o capítulo?
+ A crença de que posso participar da solução de um desafio específico
- A certeza de que dou conta de tudo
- Uma nota geral sobre mim
- A ausência de dúvidas
> Ela é específica e cresce com experiências.

? Por que um passo grande demais é recusado em `AdicionarPasso`?
+ Porque um passo que parece enorme paralisa; dividir em partes mantém o próximo passo possível
- Porque o C# limita os passos a 25
- Porque passos longos são proibidos pela lei
- Porque o computador fica lento
> A regra existe para proteger a sensação de "dá para começar".

? O que `internal void Marcar()` garante?
+ Que só o código do mesmo projeto pode marcar o passo como concluído
- Que o método é invisível para todos
- Que o passo nunca pode ser concluído
- Que ele roda em segundo plano
> O encapsulamento restringe quem pode mudar o estado.

? O que faz `yield return`?
+ Entrega um item de cada vez e retoma o método de onde parou na próxima iteração
- Termina o programa
- Retorna uma lista completa
- Lança uma exceção
> É a base de um iterador preguiçoso.

? Para que serve `const` em `LimiteDeMinutosPorPasso`?
+ Dar nome a um valor fixo e definir a regra em um só lugar
- Tornar o programa mais rápido
- Esconder o valor
- Permitir que ele mude durante a execução
> `const` não muda e documenta o significado do número.

? Por que o modelo mostra "2 de 4" e não uma porcentagem ou nota?
+ Para registrar o que foi conquistado sem criar cobrança
- Porque porcentagem não existe em C#
- Porque o cálculo é difícil
- Porque "2 de 4" é mais preciso
> Provas, não notas.
:::

::: cartas
Autoeficácia :: Crença de que posso participar da solução de um desafio específico, aprendendo e usando recursos.
Pequena prova :: Passo concluído que registra que consegui fazer algo.
Encapsulamento :: Esconder detalhes e expor só o necessário por uma interface segura.
IReadOnlyList<T> :: Visão de leitura de uma lista: quem recebe não pode adicionar nem remover.
internal :: Visível apenas dentro do mesmo projeto (assembly).
const :: Valor fixo com nome, definido em tempo de compilação.
Iterador :: Método que usa `yield return` para entregar itens um a um.
Desconstrução :: Separar uma tupla em variáveis: `var (a, b) = tupla;`.
:::
