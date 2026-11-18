---
id: leis-lei03
order: 64
kind: capitulo
num: 44
group: C# & Mentalidade · Parte 12 · As 16 leis do sucesso em C#
title: Lei 3 — Autoconfiança
subtitle: Confiança que não é otimismo vazio, e sim evidência acumulada: testes automatizados, tipos que protegem e um histórico de pequenas vitórias que dá coragem para mudar o código.
minutes: 50
hue: 150
objectives:
- Entender a lei da **autoconfiança** e a diferença entre confiança baseada em **evidência** e arrogância
- Escrever **testes xUnit** com *Arrange–Act–Assert*, `[Fact]` e `[Theory]`
- Usar **testes de caracterização** para ganhar coragem de refatorar código legado
- Ver os **tipos anuláveis** e as **asserções** como "confiança verificada pelo compilador"
- Formar o **hábito** (a corda de náilon) com integração contínua e um diário de conquistas
origem: livros/03-leis-do-sucesso-csharp/content/63-lei03.md
---

::: vila O medo de mexer
Lia herdou o método `Calcular` do frete: 40 linhas de `if` aninhados, escritos por alguém que já saiu da empresa. Todo mundo evita tocar nele — "se mexer, quebra". Esse **medo** custa caro: o código só piora. Lia decide que não vai confiar em *sorte*, nem em *coragem cega*. Vai construir **confiança com evidência**.
:::

::: ilustracao img/leis/lei03.svg | Autoconfiança vem de evidência: testes verdes, tipos e histórico de conquistas.
:::

## A lei, em uma frase

> Quem busca realizar algo grande precisa **acreditar em si e no próprio propósito**, ignorar a palavra "impossível" e tratar derrotas como parte do caminho — e essa crença se fortalece por **hábitos repetidos**, como os fios que, somados dia após dia, formam uma corda impossível de romper.

A lei também dá três conselhos práticos: **afastar-se de quem puxa para baixo**, **exaltar os próprios pontos fortes** (para que as críticas alheias não ocupem o espaço) e **lembrar os momentos de orgulho** para os dias em que a confiança falhar. E ressalva que *autoconfiança* não é *arrogância*.

## Da vida para o código

| O que a lei diz | O que isso significa para quem programa |
|---|---|
| Acreditar em si e no propósito | Confiar no **código** porque há **provas**, não esperança |
| Ignorar o "impossível" | Perder o medo de refatorar — **com rede de segurança** |
| Hábitos formam a corda | **Integração contínua**: testes rodando a cada commit |
| Exaltar seus pontos fortes | Um **diário de conquistas** (testes verdes, entregas) |
| Autossugestão (afirmações repetidas) | **Asserções**: *assert* literalmente significa "afirmação" |
| Confiança ≠ arrogância | **Cobertura alta não prova ausência de bugs** |
| Afastar-se de influências negativas | Evitar "cultura do medo" e da culpa; **post-mortems sem acusação** (Lei 14) |

::: figura O ciclo da confiança baseada em evidência
<svg viewBox="0 0 700 200" role="img" aria-label="Teste falha, código mínimo, teste passa, refatorar, mais confiança">
  <rect class="f-bad" x="20" y="60" width="140" height="64" rx="32"/><text x="90" y="88" text-anchor="middle" font-weight="800">🔴 Vermelho</text><text x="90" y="108" text-anchor="middle" font-size="11" class="t-dim">teste que falha</text>
  <rect class="f-good" x="230" y="60" width="140" height="64" rx="32"/><text x="300" y="88" text-anchor="middle" font-weight="800">🟢 Verde</text><text x="300" y="108" text-anchor="middle" font-size="11" class="t-dim">código mínimo</text>
  <rect class="f-info" x="440" y="60" width="140" height="64" rx="32"/><text x="510" y="88" text-anchor="middle" font-weight="800">🔵 Refatorar</text><text x="510" y="108" text-anchor="middle" font-size="11" class="t-dim">sem medo</text>
  <path class="arrow" d="M162 92H228"/><path class="arrow" d="M372 92H438"/>
  <path d="M510 126Q510 180 90 180Q90 150 90 126" stroke="var(--ink-3)" stroke-dasharray="5 4" fill="none" marker-end="url(#arr2)"/>
  <rect x="600" y="40" width="80" height="110" rx="10" fill="none" stroke="var(--line)"/><rect x="612" y="110" width="56" height="30" rx="4" fill="var(--good)" opacity=".8"/><rect x="612" y="78" width="56" height="30" rx="4" fill="var(--good)" opacity=".55"/><rect x="612" y="46" width="56" height="30" rx="4" fill="var(--good)" opacity=".3"/><text x="640" y="170" text-anchor="middle" font-size="11" class="t-dim">confiança</text>
</svg>
:::

## Confiança começa por uma afirmação — e um teste

Um **teste automatizado** é uma afirmação (*assertion*) sobre o comportamento do código que o computador **verifica sozinho**, quantas vezes você quiser. É a "autossugestão" certa para um desenvolvedor: repetida a cada compilação, e **não pode mentir**.

Vamos a uma regra de negócio simples e testável:

```cs Jornada/Lei03/CalculadoraDeDesconto.cs
namespace Jornada.Lei03;

public enum Perfil { Regular, Premium }

public sealed class CalculadoraDeDesconto
{
    private static readonly Dictionary<string, decimal> Cupons =
        new(StringComparer.OrdinalIgnoreCase) { ["BEMVINDO5"] = 0.05m, ["SUPER15"] = 0.15m };

    public decimal Aplicar(decimal subtotal, Perfil perfil, string? cupom)
    {
        if (subtotal < 0)
            throw new ArgumentOutOfRangeException(nameof(subtotal), "O subtotal não pode ser negativo.");

        decimal desconto = perfil == Perfil.Premium ? 0.10m : 0m;

        if (cupom is not null && Cupons.TryGetValue(cupom, out var extra))
            desconto += extra;

        desconto = Math.Min(desconto, 0.20m);                       // regra: teto de 20%

        return Math.Round(subtotal * (1 - desconto), 2, MidpointRounding.AwayFromZero);
    }
}
```

Os testes seguem o padrão **Arrange–Act–Assert** (preparar, agir, verificar). Cada `[Fact]` é um cenário; `[Theory]` roda o mesmo teste com **vários dados** (`[InlineData]`):

```cs JornadaTests/Lei03/CalculadoraDeDescontoTests.cs
using Jornada.Lei03;
using Xunit;

public class CalculadoraDeDescontoTests
{
    private readonly CalculadoraDeDesconto calc = new();

    [Theory]
    [InlineData(100, Perfil.Regular, null,        100)]   // sem desconto
    [InlineData(100, Perfil.Premium, null,         90)]   // premium: 10%
    [InlineData(100, Perfil.Premium, "BEMVINDO5",  85)]   // premium + cupom: 15%
    [InlineData(100, Perfil.Regular, "bemvindo5",  95)]   // cupom ignora maiúsculas
    [InlineData(100, Perfil.Premium, "SUPER15",    80)]   // 25% pedido, mas o teto é 20%
    [InlineData(100, Perfil.Regular, "INEXISTENTE",100)]  // cupom inválido não quebra
    public void Aplica_as_regras_de_desconto(int subtotal, Perfil perfil, string? cupom, int esperado)
    {
        var resultado = calc.Aplicar(subtotal, perfil, cupom);

        Assert.Equal((decimal)esperado, resultado);
    }

    [Fact]
    public void Subtotal_negativo_e_recusado()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => calc.Aplicar(-1, Perfil.Regular, null));
    }
}
```

Quando esses testes ficam verdes, você não "acha" que a regra funciona — **você sabe**. E se alguém alterar o teto de 20% por engano, **o teste grita antes do cliente**.

::: dica O que faz um bom teste
**Rápido** (milissegundos), **independente** (não depende de ordem ou de rede), **repetível** (mesmo resultado sempre), **autoavaliável** (passa ou falha, sem interpretação) e **legível** (o nome diz o cenário). São os princípios **F.I.R.S.T.** — e combinam com a injeção de dependências da Lei 2 e com o relógio injetado da Lei 1.
:::

## Coragem para mudar: testes de caracterização

E o frete do começo do capítulo? Ninguém sabe exatamente o que ele faz — mas **ele funciona em produção**, então o seu comportamento atual *é* a especificação. A técnica é escrever **testes de caracterização**: você chama o código legado, anota o que ele devolve e transforma isso em testes. Depois disso, pode refatorar com a rede de segurança.

```cs Jornada/Lei03/FreteLegado.cs
namespace Jornada.Lei03;

public static class FreteLegado
{
    public static decimal Calcular(decimal peso, string regiao)
    {
        decimal v;
        if (regiao == "SUL" || regiao == "SUDESTE")
        {
            if (peso <= 1) v = 10;
            else if (peso <= 5) v = 18;
            else v = 18 + (peso - 5) * 3;
        }
        else
        {
            if (peso <= 1) v = 15;
            else if (peso <= 5) v = 25;
            else v = 25 + (peso - 5) * 4.5m;
        }
        return v;
    }
}
```

```cs JornadaTests/Lei03/FreteTests.cs
using Jornada.Lei03;
using Xunit;

public class FreteTests
{
    [Theory]
    [InlineData(0.5, "SUL",   10)]
    [InlineData(3,   "SUL",   18)]
    [InlineData(7,   "SUL",   24)]
    [InlineData(0.5, "NORTE", 15)]
    [InlineData(3,   "NORTE", 25)]
    [InlineData(7,   "NORTE", 34)]
    public void Caracteriza_o_comportamento_atual(double peso, string regiao, double esperado)
    {
        Assert.Equal((decimal)esperado, FreteLegado.Calcular((decimal)peso, regiao));
    }
}
```

Agora, com os seis testes verdes, reescrevemos a lógica como **dados** (uma tabela de faixas) — e rodamos os **mesmos testes** contra a nova versão:

```cs Jornada/Lei03/Frete.cs
namespace Jornada.Lei03;

public sealed record Faixa(decimal AteKg, decimal Preco);

public sealed record TabelaDeFrete(IReadOnlyList<Faixa> Faixas, decimal AdicionalPorKg)
{
    public decimal Calcular(decimal peso)
    {
        foreach (var faixa in Faixas)
            if (peso <= faixa.AteKg) return faixa.Preco;

        var ultima = Faixas[^1];
        return ultima.Preco + (peso - ultima.AteKg) * AdicionalPorKg;
    }
}

public static class Frete
{
    private static readonly TabelaDeFrete SulSudeste =
        new(new[] { new Faixa(1, 10), new Faixa(5, 18) }, AdicionalPorKg: 3m);

    private static readonly TabelaDeFrete Demais =
        new(new[] { new Faixa(1, 15), new Faixa(5, 25) }, AdicionalPorKg: 4.5m);

    public static decimal Calcular(decimal peso, string regiao) =>
        (regiao is "SUL" or "SUDESTE" ? SulSudeste : Demais).Calcular(peso);
}
```

```cs Uso
// Trocamos FreteLegado por Frete nos testes: se continuarem verdes, a refatoração foi segura.
foreach (var (peso, regiao) in new[] { (0.5m, "SUL"), (3m, "SUL"), (7m, "SUL"), (0.5m, "NORTE"), (3m, "NORTE"), (7m, "NORTE") })
{
    var antigo = FreteLegado.Calcular(peso, regiao);
    var novo   = Frete.Calcular(peso, regiao);
    Console.WriteLine($"{regiao,-5} {peso,4} kg  legado={antigo,5}  novo={novo,5}  {(antigo == novo ? "✔" : "✘")}");
}
```

```saida
SUL    0,5 kg  legado=   10  novo=   10  ✔
SUL      3 kg  legado=   18  novo=   18  ✔
SUL      7 kg  legado=   24  novo=   24  ✔
NORTE  0,5 kg  legado=   15  novo=   15  ✔
NORTE    3 kg  legado=   25  novo=   25  ✔
NORTE    7 kg  legado= 34,0  novo= 34,0  ✔
```

::: pense Sem testes, "coragem" é só imprudência
A lei fala em não se importar com derrotas temporárias. Em código, isso só é sensato **com rede de segurança**: sem testes, o "ignore o impossível" vira "mexa e reze". Com testes, o mesmo desejo de ousar vira **refatoração segura** — e é por isso que o TDD e a refatoração (cap. 17 e 18) andam de mãos dadas.
:::

## A confiança que o compilador também dá

Boa parte da confiança vem de **tipos**. Ative `<Nullable>enable</Nullable>` e o compilador passa a avisar quando algo pode ser `null`:

```cs Jornada/Lei03/Nulos.cs
namespace Jornada.Lei03;

public sealed class Cliente
{
    public required string Nome { get; init; }       // nunca nulo: garantido na criação
    public string? Apelido { get; init; }            // pode ser nulo: o tipo diz isso

    public string NomeParaExibir => Apelido ?? Nome; // sem NullReferenceException possível
}
```

E para invariantes que você **afirma** como verdadeiras, use guardas e asserções:

```cs Jornada/Lei03/Conta.cs
namespace Jornada.Lei03;

public sealed class Conta
{
    public decimal Saldo { get; private set; }

    public Conta(decimal saldoInicial)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(saldoInicial);   // afirmação que nunca pode ser violada
        Saldo = saldoInicial;
    }

    public void Sacar(decimal valor)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(valor);
        if (valor > Saldo) throw new InvalidOperationException("Saldo insuficiente.");
        Saldo -= valor;
        System.Diagnostics.Debug.Assert(Saldo >= 0, "Invariante: o saldo nunca fica negativo.");
    }
}
```

## O hábito: a corda de náilon

Um teste isolado dá pouca confiança; **milhares de execuções automáticas** dão muita. Coloque o teste no seu **pipeline de integração contínua**, para rodar a cada commit — um "fio por dia" que, somado, vira corda:

```yaml
# .github/workflows/ci.yml
name: ci
on: [push, pull_request]
jobs:
  testes:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      - uses: actions/setup-dotnet@v4
        with: { dotnet-version: '8.0.x' }
      - run: dotnet test --configuration Release
```

E, como a lei recomenda, **registre suas vitórias**. Num dia ruim, seu "diário de conquistas" é a prova concreta de que você é capaz:

```cs Jornada/Lei03/Diario.cs
namespace Jornada.Lei03;

public sealed record Conquista(DateOnly Data, string Descricao);

public sealed class DiarioDeConquistas
{
    private readonly List<Conquista> itens = new();

    public void Registrar(DateOnly data, string descricao) => itens.Add(new Conquista(data, descricao));

    public IEnumerable<Conquista> Ultimas(int quantas) =>
        itens.OrderByDescending(c => c.Data).Take(quantas);
}
```

```cs Uso
var diario = new DiarioDeConquistas();
diario.Registrar(new DateOnly(2026, 10, 12), "Primeiro teste verde no módulo de frete");
diario.Registrar(new DateOnly(2026, 10, 19), "Refatorei o frete sem quebrar nada");
diario.Registrar(new DateOnly(2026, 10, 26), "Pipeline de CI rodando a cada commit");

Console.WriteLine("Minhas últimas conquistas:");
foreach (var c in diario.Ultimas(2))
    Console.WriteLine($" ✔ {c.Data:dd/MM}  {c.Descricao}");
```

```saida
Minhas últimas conquistas:
 ✔ 26/10  Pipeline de CI rodando a cada commit
 ✔ 19/10  Refatorei o frete sem quebrar nada
```

::: cuidado Confiança não é arrogância
A lei distingue autoconfiança de arrogância; no código, o paralelo é **confiança vs. falsa segurança**. Testes verdes mostram que os cenários **testados** passam — não que o programa não tenha bugs. Cobertura de 100% pode coexistir com testes que não verificam nada (`Assert` ausentes). Para ganhar profundidade: testes de **propriedades**, **testes de mutação**, revisão de código (Lei 2) e **observabilidade** em produção (Lei 14).
:::

::: exercicio Teste primeiro
Escreva (antes do código!) os testes de uma função `Idade.Em(DateOnly nascimento, DateOnly hoje)` que devolve a idade completa em anos, e então implemente-a.

Cenários mínimos: já fez aniversário no ano; ainda não fez; faz aniversário **hoje**; nascido em 29/02 (ano bissexto); data de nascimento no futuro (deve lançar exceção).

--- resposta
```cs
public class IdadeTests
{
    [Theory]
    [InlineData("2000-03-10", "2026-10-26", 26)]   // já fez aniversário
    [InlineData("2000-12-25", "2026-10-26", 25)]   // ainda não fez
    [InlineData("2000-10-26", "2026-10-26", 26)]   // aniversário hoje
    [InlineData("2000-02-29", "2026-02-27", 25)]   // bissexto: ainda não completou
    [InlineData("2000-02-29", "2026-02-28", 26)]   // bissexto: em anos comuns, o aniversário é 28/02
    public void Calcula_a_idade_completa(string nasc, string hoje, int esperado) =>
        Assert.Equal(esperado, Idade.Em(DateOnly.Parse(nasc), DateOnly.Parse(hoje)));

    [Fact]
    public void Nascimento_no_futuro_e_recusado() =>
        Assert.Throws<ArgumentException>(() => Idade.Em(new DateOnly(2030, 1, 1), new DateOnly(2026, 1, 1)));
}

public static class Idade
{
    public static int Em(DateOnly nascimento, DateOnly hoje)
    {
        if (nascimento > hoje) throw new ArgumentException("Nascimento no futuro.", nameof(nascimento));
        int anos = hoje.Year - nascimento.Year;
        return hoje < nascimento.AddYears(anos) ? anos - 1 : anos;
    }
}
```
Repare no caso do dia **29/02**: `AddYears` devolve 28/02 em anos comuns, então escolhemos "o aniversário cai em 28/02". Outra regra (01/03) também seria defensável — e **esse é o ponto**: escrever o teste primeiro **obriga você a decidir** o comportamento (Lei 1) antes de programar.
:::

::: resumo
- **Lei 3:** acreditar em si e no propósito, ignorar o "impossível", tratar derrotas como parte do caminho; hábitos repetidos constroem essa confiança.
- Em software, a boa confiança é **baseada em evidência**: **testes automatizados** (xUnit, *Arrange–Act–Assert*, `[Fact]`/`[Theory]`).
- **Testes de caracterização** dão coragem para refatorar legado: o comportamento atual vira especificação.
- **Tipos** (`string?`, `required`) e **guardas/asserções** são confiança verificada pelo compilador e pelo runtime.
- O **hábito** vem do pipeline de **CI** (um fio por dia) e de um **diário de conquistas**.
- Cuidado: testes verdes **não provam** ausência de bugs; confiança ≠ arrogância.
:::

::: quiz
? Qual a melhor tradução da "autoconfiança" da lei para o código?
+ Confiança baseada em evidência, como testes automatizados
- Otimismo sem verificação
- Não escrever testes para ir mais rápido
- Evitar revisões de código
> Confiança sem provas é arrogância; com provas, é conhecimento.

? Para que servem os testes de caracterização?
+ Registrar o comportamento atual do legado para refatorar com segurança
- Medir o desempenho do código
- Substituir a documentação
- Testar apenas a interface gráfica
> O comportamento que roda em produção vira a especificação.

? O que faz um `[Theory]` com `[InlineData]` no xUnit?
+ Executa o mesmo teste com vários conjuntos de dados
- Executa os testes em paralelo na GPU
- Ignora testes que falham
- Gera a documentação
> Cada InlineData é um cenário.

? Qual o papel do `string?` com Nullable ativado?
+ Declarar no tipo que o valor pode ser nulo, e fazer o compilador avisar usos perigosos
- Tornar a string mais rápida
- Impedir que seja usada
- Converter nulo em vazio automaticamente
> O compilador ajuda a evitar NullReferenceException.

? Por que colocar os testes em integração contínua?
+ Para rodarem a cada commit, criando o hábito (a "corda de náilon") e detectando regressões cedo
- Para eliminar a necessidade de revisão de código
- Para o build ficar mais lento
- Para dispensar testes locais
> A repetição automática é o fio diário.

? Qual o limite da confiança dada pelos testes?
+ Eles só provam os cenários testados; cobertura alta não garante ausência de bugs
- Nenhum
- Só valem para código novo
- Só funcionam em Windows
> Mantenha revisão, observabilidade e outros tipos de teste.
:::

::: cartas
Autoconfiança :: Crença no próprio propósito e capacidade, fortalecida por hábitos e evidências.
Asserção (assertion) :: Afirmação verificável sobre o comportamento do código.
Arrange–Act–Assert :: Padrão de teste: preparar, executar, verificar.
[Fact] / [Theory] :: Atributos do xUnit para um cenário / um cenário com vários dados.
Teste de caracterização :: Teste que registra o comportamento atual de código legado.
Tipos anuláveis :: `string?` indica que pode ser nulo; o compilador ajuda a evitar erros.
Integração contínua :: Rodar build e testes automaticamente a cada alteração.
F.I.R.S.T. :: Testes rápidos, independentes, repetíveis, autoavaliáveis e feitos no momento certo.
:::
