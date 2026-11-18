---
id: leis-lei09
order: 70
kind: capitulo
num: 50
group: C# & Mentalidade · Parte 12 · As 16 leis do sucesso em C#
title: Lei 9 — Faça sempre mais do que o exigido
subtitle: A diferença entre "funciona" e "excelente" está nos extras de qualidade que ninguém pediu: validação, mensagens claras, documentação, logs e testes de borda — sem cair no excesso de escopo.
minutes: 50
hue: 168
objectives:
- Entender a lei: **ganha mais quem serve melhor**, e o hábito de entregar um pouco além do pedido
- Comparar uma implementação **mínima** com uma **excelente** do mesmo requisito
- Distinguir **extra de qualidade** (bom) de **extra de escopo** (risco / *gold plating*)
- Usar **XML docs**, **`ILogger`** com *message templates* e **testes de borda**
- Montar uma **definição de pronto** que institucionaliza o hábito
origem: livros/03-leis-do-sucesso-csharp/content/69-lei09.md
---

::: vila O "só isso" que virou diferencial
A tarefa de Lia era curtinha: *"calcular o total do carrinho"*. Ela podia ter escrito uma linha e fechado o ticket. Em vez disso, fez o que pediram — e mais **cinco coisas pequenas**: recusar quantidade negativa com uma mensagem útil, documentar o método, registrar um log, testar o carrinho vazio e arredondar com uma regra explícita. Seis meses depois, esse trecho é o **único** do módulo em que ninguém nunca precisou abrir um chamado.
:::

::: ilustracao img/leis/lei09.svg | Além do exigido: qualidade a mais (testes, validações, documentação), sem inventar escopo.
:::

## A lei, em uma frase

> **Ganha mais quem serve melhor.** Seu sucesso é proporcional ao **valor** que você cria para os outros; uma das maneiras mais eficazes de criar valor é adquirir o hábito de **entregar sempre um pouco mais do que se espera** — sem exigir recompensa imediata — e buscar a **excelência**, não apenas o "bom o bastante".

A lição também diz que sua **segurança profissional** não está no cargo, mas na **capacidade de gerar valor, suprir necessidades e resolver problemas**; que fazer um trabalho por amor costuma produzir **mais e melhor** do que fazer só pelo salário; e sugere uma inversão de postura: em vez de "mostre-me o pagamento que eu mostro o trabalho", **mostre o trabalho primeiro**.

## Da vida para o código

| O que a lei diz | O que isso significa para quem programa |
|---|---|
| Entregar mais do que o pedido | **Qualidade embutida**: validações, mensagens claras, testes de borda |
| Excelência, não só o bom | **Código legível** e **documentado** (XML docs, exemplos) |
| Mostrar o trabalho primeiro | **Demonstrações** e *pull requests* pequenos com contexto |
| Valor para quem usa | **Observabilidade** (logs úteis) para quem vai operar o sistema |
| "Uma ilha de excelência" | **Definição de pronto** compartilhada |
| Cuidado: servir ≠ complicar | Não confundir **extra de qualidade** com **extra de escopo** |

::: figura Camadas além do exigido
<svg viewBox="0 0 700 220" role="img" aria-label="Camadas: funciona, valida, testa bordas, documenta, registra logs">
  <rect class="f-prim" x="140" y="170" width="420" height="36" rx="6"/><text x="350" y="194" text-anchor="middle" font-weight="800">1 · O que foi pedido: "funciona"</text>
  <rect class="f-info" x="165" y="132" width="370" height="34" rx="6"/><text x="350" y="154" text-anchor="middle">2 · valida entradas, mensagens úteis</text>
  <rect class="f-good" x="190" y="94" width="320" height="34" rx="6"/><text x="350" y="116" text-anchor="middle">3 · testes de borda</text>
  <rect class="f-warn" x="215" y="56" width="270" height="34" rx="6"/><text x="350" y="78" text-anchor="middle">4 · documentação (XML docs)</text>
  <rect class="f-bad" x="240" y="18" width="220" height="34" rx="6"/><text x="350" y="40" text-anchor="middle">5 · logs para quem opera</text>
  <text x="640" y="110" text-anchor="middle" class="t-dim" font-size="12">excelência</text><path class="arrow" d="M640 180V120"/>
</svg>
:::

## O requisito e as duas respostas

**Requisito:** *"Calcular o total do carrinho."*

**Resposta mínima** — cumpre a letra do pedido:

```cs Jornada/Lei09/Minimo.cs
namespace Jornada.Lei09;

public sealed record ItemDoCarrinho(string Nome, decimal PrecoUnitario, int Quantidade);

public static class CarrinhoMinimo
{
    public static decimal Total(IEnumerable<ItemDoCarrinho> itens) =>
        itens.Sum(i => i.PrecoUnitario * i.Quantidade);
}
```

Funciona — até alguém passar `null`, um item com quantidade `-3` (que **reduz** o total), ou um carrinho com desconto de 10% cujo arredondamento ninguém definiu.

**Resposta excelente** — cumpre o pedido e **cuida de quem vai usar e de quem vai operar**:

```cs Jornada/Lei09/Excelente.cs
using Microsoft.Extensions.Logging;

namespace Jornada.Lei09;

public sealed record ResumoDoCarrinho(int Itens, decimal Subtotal, decimal Desconto, decimal Total);

public sealed class CalculadoraDeCarrinho
{
    private readonly ILogger logger;

    public CalculadoraDeCarrinho(ILogger logger) => this.logger = logger;

    /// <summary>
    /// Calcula o resumo do carrinho: subtotal, desconto e total.
    /// </summary>
    /// <param name="itens">Itens do carrinho (pode ser vazio, mas não nulo). Quantidade deve ser positiva e preço, não negativo.</param>
    /// <param name="percentualDesconto">Percentual entre 0 e 100 (inclusive), aplicado sobre o subtotal.</param>
    /// <returns>O resumo; o desconto é arredondado para 2 casas, "para longe do zero" (R$ 1,005 → R$ 1,01).</returns>
    /// <exception cref="ArgumentNullException"><paramref name="itens"/> é nulo.</exception>
    /// <exception cref="ArgumentException">Há um item com quantidade ou preço inválidos.</exception>
    /// <exception cref="ArgumentOutOfRangeException">O percentual está fora de 0–100.</exception>
    /// <example>
    /// <code>
    /// var resumo = calculadora.Calcular(new[] { new ItemDoCarrinho("Caneta", 3.50m, 2) }, percentualDesconto: 10);
    /// // resumo.Total == 6.30m
    /// </code>
    /// </example>
    public ResumoDoCarrinho Calcular(IReadOnlyCollection<ItemDoCarrinho> itens, decimal percentualDesconto = 0m)
    {
        ArgumentNullException.ThrowIfNull(itens);

        if (percentualDesconto is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(percentualDesconto), percentualDesconto,
                "O desconto deve estar entre 0 e 100.");

        foreach (var item in itens)
            if (item.Quantidade <= 0 || item.PrecoUnitario < 0)
                throw new ArgumentException(
                    $"Item inválido: '{item.Nome}' (quantidade={item.Quantidade}, preço={item.PrecoUnitario}).",
                    nameof(itens));

        decimal subtotal = itens.Sum(i => i.PrecoUnitario * i.Quantidade);
        decimal desconto = Math.Round(subtotal * percentualDesconto / 100m, 2, MidpointRounding.AwayFromZero);
        var resumo = new ResumoDoCarrinho(itens.Sum(i => i.Quantidade), subtotal, desconto, subtotal - desconto);

        logger.LogInformation("Carrinho calculado: {Itens} itens, subtotal {Subtotal}, desconto {Desconto}",
                              resumo.Itens, resumo.Subtotal, resumo.Desconto);
        return resumo;
    }
}
```

O que foi acrescentado **além do exigido**, sem mudar o requisito:

1. **Validação com mensagem clara** (diz *qual* item e *qual* valor estão errados — Lei 8 e Lei 10).
2. **Regra de arredondamento explícita** e documentada (Lei 11, precisão).
3. **Documentação XML** com parâmetros, exceções e exemplo — aparece no *IntelliSense* de quem usa.
4. **Log estruturado** com *message template* (`{Itens}`, `{Subtotal}`…): quem opera o sistema consegue **filtrar e somar** esses campos, em vez de caçar texto.
5. Resultado como **`record` imutável** (`ResumoDoCarrinho`): mais informativo que um `decimal` solto.

Para ver o log sem um provedor completo, um coletor mínimo de `ILogger` serve nos exemplos e nos testes:

```cs Jornada/Lei09/ColetorDeLogs.cs
using Microsoft.Extensions.Logging;

namespace Jornada.Lei09;

public sealed class ColetorDeLogs : ILogger
{
    public List<string> Linhas { get; } = new();

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
                            Exception? exception, Func<TState, Exception?, string> formatter) =>
        Linhas.Add($"{logLevel}: {formatter(state, exception)}");
}
```

```cs Uso
var logs = new ColetorDeLogs();
var calculadora = new CalculadoraDeCarrinho(logs);

var carrinho = new[]
{
    new ItemDoCarrinho("Caneta",  3.50m, 2),
    new ItemDoCarrinho("Caderno", 12.00m, 1),
};

var resumo = calculadora.Calcular(carrinho, percentualDesconto: 10);

Console.WriteLine($"Itens: {resumo.Itens} · Subtotal: {resumo.Subtotal:F2} · Desconto: {resumo.Desconto:F2} · Total: {resumo.Total:F2}");
Console.WriteLine(logs.Linhas[0]);

try { calculadora.Calcular(new[] { new ItemDoCarrinho("Borracha", 2m, -3) }); }
catch (ArgumentException e) { Console.WriteLine("Recusado: " + e.Message); }
```

```saida
Itens: 3 · Subtotal: 19,00 · Desconto: 1,90 · Total: 17,10
Information: Carrinho calculado: 3 itens, subtotal 19.00, desconto 1.90
Recusado: Item inválido: 'Borracha' (quantidade=-3, preço=2). (Parameter 'itens')
```

## Testes de borda: o extra que mais compensa

Os testes "felizes" mostram que o código funciona **quando tudo vai bem**. Os de **borda** mostram o que acontece nos limites — onde os bugs moram:

```cs JornadaTests/Lei09/CalculadoraDeCarrinhoTests.cs
using Jornada.Lei09;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

public class CalculadoraDeCarrinhoTests
{
    private readonly CalculadoraDeCarrinho calc = new(NullLogger.Instance);

    [Fact]
    public void Carrinho_vazio_tem_total_zero()
    {
        var r = calc.Calcular(Array.Empty<ItemDoCarrinho>());
        Assert.Equal(0m, r.Total);
        Assert.Equal(0, r.Itens);
    }

    [Fact]
    public void Desconto_de_100_por_cento_zera_o_total()
    {
        var r = calc.Calcular(new[] { new ItemDoCarrinho("A", 10m, 1) }, percentualDesconto: 100);
        Assert.Equal(0m, r.Total);
    }

    [Fact]
    public void Arredonda_o_desconto_para_longe_do_zero()
    {
        // 10% de 10,05 = 1,005 → 1,01 (e não 1,00)
        var r = calc.Calcular(new[] { new ItemDoCarrinho("A", 10.05m, 1) }, percentualDesconto: 10);
        Assert.Equal(1.01m, r.Desconto);
        Assert.Equal(9.04m, r.Total);
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(101)]
    public void Percentual_fora_de_0_a_100_e_recusado(int percentual) =>
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            calc.Calcular(Array.Empty<ItemDoCarrinho>(), percentual));

    [Fact]
    public void Quantidade_nao_positiva_e_recusada() =>
        Assert.Throws<ArgumentException>(() => calc.Calcular(new[] { new ItemDoCarrinho("A", 1m, 0) }));

    [Fact]
    public void Itens_nulos_sao_recusados() =>
        Assert.Throws<ArgumentNullException>(() => calc.Calcular(null!));
}
```

## O cuidado essencial: extra de qualidade ≠ extra de escopo

Há uma armadilha na lei, e o desenvolvimento ágil a conhece bem: **"fazer mais"** pode virar **"fazer o que ninguém pediu e que muda o comportamento"**. Isso se chama *gold plating* (banhar a peça em ouro), e pode quebrar o que o cliente esperava.

| Extra de **qualidade** (faça!) | Extra de **escopo** (converse antes!) |
|---|---|
| Recusar dados inválidos com mensagem clara | Aplicar um desconto "de brinde" que ninguém pediu |
| Documentar a regra de arredondamento | **Mudar** a regra de arredondamento "porque achei melhor" |
| Teste para carrinho vazio | Nova tela de relatório "que vai ser útil" |
| Log estruturado para operação | Configuração para 12 formatos de saída "para o futuro" |
| Nome claro, método curto | Reescrever o módulo inteiro sem necessidade |

::: pense Como saber de que lado está?
Pergunte: *"Isso muda **o que o sistema faz** para quem o usa, ou **o quão bem** ele faz o que já faria?"* Se muda o **quê**, é escopo: leve ao cliente/produto, dê preço, deixe que priorizem (Lei 1). Se melhora o **quão bem**, é qualidade: faça, é parte do ofício. E qualidade tem custo: use a **definição de pronto** da equipe para não depender de heroísmo individual.
:::

## Institucionalizando o hábito: a definição de pronto

Para que o "fazer mais" não dependa da boa vontade de uma pessoa, transforme-o em **acordo do time**:

```text
Definição de pronto (exemplo)
[ ] O que foi pedido funciona (teste de aceitação verde)
[ ] Entradas inválidas são recusadas com mensagem clara
[ ] Casos de borda têm teste (vazio, limite, nulo)
[ ] Métodos públicos têm documentação XML
[ ] Eventos importantes são registrados em log estruturado
[ ] Sem warnings novos; análise estática limpa
[ ] Alguém revisou o pull request (Lei 2)
```

E para aproveitar a documentação, ligue a geração do arquivo XML:

```xml
<PropertyGroup>
  <GenerateDocumentationFile>true</GenerateDocumentationFile>
  <!-- CS1591: aviso para membros públicos sem documentação -->
</PropertyGroup>
```

::: exercicio Qualidade ou escopo?
Classifique cada ideia como **extra de qualidade (Q)** ou **extra de escopo (E)** e diga o que faria:
1. Acrescentar um teste para o caso de quantidade `int.MaxValue`.
2. Enviar um e-mail ao cliente quando o carrinho passa de R$ 1.000 (ninguém pediu).
3. Trocar `decimal` por `double` "para ficar mais rápido".
4. Incluir o nome do item na mensagem de erro de validação.
5. Criar uma tela de administração para editar as regras de desconto.

--- resposta
1. **Q** — teste de borda barato; pode revelar *overflow* (`checked`/limites) — faça.
2. **E** — muda o comportamento visível; leve ao produto, discuta consentimento/privacidade, priorize.
3. **Nem Q nem E: risco** — para dinheiro, `double` introduz erros de arredondamento (Lei 11); recuse a troca.
4. **Q** — mensagem melhor para quem usa; já fizemos.
5. **E** — é uma funcionalidade nova; vira história de usuário com prioridade e prazo.
:::

::: resumo
- **Lei 9:** ganha mais quem serve melhor; o hábito de entregar **um pouco além do pedido**, com excelência, cria valor e segurança profissional.
- Em C#: **validação** com mensagens claras, **regras explícitas** (arredondamento), **XML docs**, **log estruturado** (`ILogger` + *message templates*) e **testes de borda**.
- Resultados mais ricos (`record`) valem mais que valores soltos.
- Cuidado com **gold plating**: **extra de qualidade** (o *quão bem*) → faça; **extra de escopo** (o *quê*) → converse antes.
- Institucionalize com uma **definição de pronto** compartilhada e *code review*.
:::

::: quiz
? Segundo a lei, de que depende o sucesso de alguém?
+ Do valor que essa pessoa cria para os outros
- Do cargo que ocupa
- Do número de horas trabalhadas
- Da quantidade de código
> Ganha mais quem serve melhor.

? Qual é um exemplo de "extra de qualidade" no código?
+ Mensagem de validação que diz qual item e qual valor estão inválidos
- Acrescentar uma nova funcionalidade ao produto
- Mudar a regra de negócio por conta própria
- Reescrever tudo
> Melhora o "quão bem", sem mudar o "o quê".

? O que é gold plating?
+ Acrescentar funcionalidades ou mudanças que ninguém pediu, correndo riscos de escopo
- Um padrão de projeto
- Otimização de memória
- Um tipo de teste
> É o lado perigoso do "fazer mais".

? Por que usar um log estruturado com message template (`{Itens}`)?
+ Para que os campos possam ser filtrados e agregados por quem opera o sistema
- Para economizar disco
- Para evitar exceções
- Para evitar testes
> O template preserva os valores como dados, não só como texto.

? Qual a vantagem dos testes de borda?
+ Revelam o comportamento nos limites (vazio, nulo, máximo), onde os bugs costumam estar
- Substituem a documentação
- São mais rápidos que os demais
- Dispensam validação
> Testes felizes não bastam.

? Como institucionalizar o hábito de "fazer mais"?
+ Com uma definição de pronto acordada pela equipe e revisão de código
- Dependendo da boa vontade individual
- Pedindo horas extras
- Evitando documentação
> Torna a qualidade parte do processo, não do heroísmo.
:::

::: cartas
Milha extra :: Entregar um pouco além do pedido, com qualidade, sem exigir recompensa imediata.
Gold plating :: Acrescentar o que ninguém pediu, correndo riscos de escopo.
Extra de qualidade :: Melhora o "quão bem" o sistema faz o que já faz.
Extra de escopo :: Muda o "o que" o sistema faz: deve ser negociado.
XML docs :: Comentários `///` que alimentam o IntelliSense e a documentação.
Log estruturado :: Registro com campos nomeados ({Itens}) que podem ser filtrados.
Teste de borda :: Teste de limites e casos extremos.
Definição de pronto :: Lista acordada do que "concluído" significa para o time.
:::
