---
id: foco-cap60
order: 82
kind: capitulo
num: 60
group: C# & Mentalidade · Parte 13 · Foco em C#
title: Hábitos, piloto automático e o sequestro emocional
subtitle: O cérebro tem dois modos, o automático (rápido, barato) e o deliberado (lento, caro). Automatizar a rotina libera atenção para o que é novo; mas sob pressão o piloto automático pode falhar.
minutes: 55
hue: 215
objectives:
- Distinguir **sistemas de baixo para cima** (automáticos) e **de cima para baixo** (deliberados)
- Ver a **automação de rotina** como forma de **liberar atenção**, e priorizá-la por custo de atenção
- Reconhecer onde o **piloto automático falha**: surpresa, risco alto, desatenção
- Usar **pausas e checklists** em pontos críticos, em vez de confiar só no hábito
- Entender o "sequestro emocional" como **metáfora** útil, sem exagerar a ciência
origem: livros/04-foco-em-csharp/content/92-cap60.md
---

::: vila A sexta-feira do deploy
Todo fim de mês Caio faz o *deploy* de uma versão. Já fez trinta vezes; é tão automático que ele conversa com a Bia enquanto digita os comandos. Na trigésima primeira vez, o servidor responde diferente do habitual, e Caio, no piloto automático, **não percebe**. Lia propõe duas coisas: **automatizar** o que é rotina e criar **pausas** nos pontos onde a surpresa é possível.
:::

::: ilustracao img/foco/cap60.svg | O piloto automático é ótimo na rotina e cego à surpresa: nos pontos críticos, pare e confira.
:::

## O que o livro diz

> A mente trabalha com **dois sistemas**: um **de baixo para cima**, rápido, intuitivo e automático, que roda no "piloto automático"; e outro **de cima para baixo**, lento, deliberado, que exige esforço e atenção. O primeiro é eficiente; o segundo é o que usamos para o novo.

Pontos do capítulo original que importam aqui:

- **Hábitos** são rotinas que o cérebro automatiza para **economizar esforço**; a repetição transforma o deliberado em automático.
- **Especialistas** executam o que já dominam em piloto automático — o que libera atenção para o **imprevisto**. Mas essa mesma automação pode ser **cega** quando algo muda.
- Sob **pressão forte**, o desempenho pode **desmoronar** (o "*choke*" de atletas): excesso de preocupação ou de autoescuta atrapalha o que deveria fluir.
- Quando uma emoção forte "toma conta", o raciocínio deliberado perde força — a imagem do **"sequestro da amígdala"** (termo popularizado pelo próprio autor).
- Ellen Langer chama de **desatenção** o estado de agir sem notar: os detalhes novos passam batido porque "já sabemos".

::: cuidado Metáfora, sim; mágica, não
"Sequestro da amígdala" é uma **imagem útil**, não um diagnóstico: o cérebro é mais complexo do que "uma parte rouba o controle". E as explicações do *choke* ainda são debatidas na pesquisa. Use as ideias como **lentes** para pensar no seu trabalho — não como verdades definitivas.
:::

## Da vida para o código

| No livro | Em engenharia de software |
|---|---|
| Sistema automático (baixo-cima) | **Rotinas automatizadas** (scripts, CI/CD, formatadores) |
| Sistema deliberado (cima-baixo) | **Revisão, design, decisões novas** |
| Hábito = economia de esforço | **Automatize o repetitivo**, liberando atenção |
| Piloto automático falha na surpresa | **Checklists** em pontos críticos (deploy, migração) |
| Pressão derruba o desempenho | **Incidentes**: protocolos e "pausa antes de agir" |
| Desatenção de Langer | **Revisar o que "sempre funciona"** quando o ambiente muda |

::: figura Dois modos de atenção
<svg viewBox="0 0 740 230" role="img" aria-label="Rotina vai para o modo automático; o novo vai para o modo deliberado; surpresa aciona a troca">
  <rect class="f-info" x="20" y="30" width="300" height="140" rx="14"/>
  <text x="170" y="58" text-anchor="middle" font-weight="800" font-size="14">Automático (baixo → cima)</text>
  <g font-size="12" text-anchor="middle">
    <text x="170" y="86">rápido · barato · paralelo</text>
    <text x="170" y="108">build, formatação, testes de rotina</text>
    <text x="170" y="130">hábitos bem treinados</text>
    <text x="170" y="152" class="t-dim">risco: cego a mudanças</text>
  </g>
  <rect class="f-warn" x="420" y="30" width="300" height="140" rx="14"/>
  <text x="570" y="58" text-anchor="middle" font-weight="800" font-size="14">Deliberado (cima → baixo)</text>
  <g font-size="12" text-anchor="middle">
    <text x="570" y="86">lento · caro · um de cada vez</text>
    <text x="570" y="108">design, revisão, incidentes</text>
    <text x="570" y="130">o que é novo ou arriscado</text>
    <text x="570" y="152" class="t-dim">custo: atenção limitada</text>
  </g>
  <path class="arrow" d="M322 80L418 80"/><path class="arrow" d="M418 122L322 122"/>
  <text x="370" y="72" text-anchor="middle" font-size="11">surpresa</text>
  <text x="370" y="140" text-anchor="middle" font-size="11">dominou</text>
  <text x="370" y="205" text-anchor="middle" font-size="12" font-weight="700">Meta: automatizar a rotina para sobrar atenção ao novo</text>
</svg>
:::

## 1) Priorizar o que automatizar

Se a atenção é limitada, **automatize primeiro o que mais a consome**. Um modelo simples: cada passo de uma rotina tem **custo de atenção** (1 a 5) e **frequência semanal**; a carga é o produto.

```cs Jornada/Foco/Cap60/PlanoDeAutomacao.cs
namespace Jornada.Foco.Cap60;

public sealed record Passo(string Nome, int CustoDeAtencao, int VezesPorSemana, bool Automatizado = false)
{
    /// <summary>Passos automatizados não consomem atenção.</summary>
    public int CargaSemanal => Automatizado ? 0 : CustoDeAtencao * VezesPorSemana;
}

public static class PlanoDeAutomacao
{
    public static int CargaTotal(IEnumerable<Passo> passos) => passos.Sum(p => p.CargaSemanal);

    /// <summary>Passos ainda manuais, dos que mais pesam para os que menos pesam.</summary>
    public static IReadOnlyList<Passo> Priorizar(IEnumerable<Passo> passos) =>
        passos.Where(p => !p.Automatizado)
              .OrderByDescending(p => p.CargaSemanal)
              .ThenBy(p => p.Nome)
              .ToList();

    public static IReadOnlyList<Passo> Automatizar(IEnumerable<Passo> passos, string nome) =>
        passos.Select(p => p.Nome == nome ? p with { Automatizado = true } : p).ToList();
}
```

```cs Uso
var rotina = new List<Passo>
{
    new("Rodar testes locais",        CustoDeAtencao: 2, VezesPorSemana: 30),
    new("Formatar código",            CustoDeAtencao: 1, VezesPorSemana: 40),
    new("Gerar notas da versão",      CustoDeAtencao: 3, VezesPorSemana: 4),
    new("Conferir dependências",      CustoDeAtencao: 4, VezesPorSemana: 5),
};

Console.WriteLine($"Carga atual: {PlanoDeAutomacao.CargaTotal(rotina)}");
foreach (var p in PlanoDeAutomacao.Priorizar(rotina))
    Console.WriteLine($"  {p.Nome,-24} {p.CargaSemanal,3}");

var depois = PlanoDeAutomacao.Automatizar(rotina, "Rodar testes locais");
Console.WriteLine($"Após automatizar os testes: {PlanoDeAutomacao.CargaTotal(depois)}");
```

```saida
Carga atual: 132
  Rodar testes locais       60
  Formatar código           40
  Conferir dependências     20
  Gerar notas da versão     12
Após automatizar os testes: 72
```

::: dica O custo de atenção é seu juízo
Os valores de 1 a 5 são **estimativas suas** (ou da equipe), não medidas. O objetivo do modelo não é a precisão, mas a **conversa**: ele deixa claro que "formatar código" (barato, mas 40 vezes) pesa mais que "gerar notas" (caro, mas 4 vezes).
:::

## 2) Quando o piloto automático não basta

Automático é ótimo para o **familiar e de baixo risco**. Quando há **surpresa** ou **risco alto**, o modo deliberado precisa assumir — mesmo para quem tem experiência:

```cs Jornada/Foco/Cap60/ModoDeExecucao.cs
namespace Jornada.Foco.Cap60;

public enum ModoDeExecucao { Automatico, Deliberado }

/// <param name="Familiaridade">0 (nunca fiz) a 10 (faço de olhos fechados).</param>
public sealed record Situacao(int Familiaridade, bool Surpresa = false, bool AltoRisco = false);

public static class EscolhaDeModo
{
    public static ModoDeExecucao Decidir(Situacao s)
    {
        if (s.Familiaridade is < 0 or > 10)
            throw new ArgumentOutOfRangeException(nameof(s), "Familiaridade deve ficar entre 0 e 10.");

        if (s.Surpresa || s.AltoRisco)
            return ModoDeExecucao.Deliberado;   // desconfie do hábito justo onde ele pode custar caro

        return s.Familiaridade >= 7 ? ModoDeExecucao.Automatico : ModoDeExecucao.Deliberado;
    }
}
```

E um **checklist** para o ponto em que o hábito é perigoso (o *deploy* de Caio):

```cs Jornada/Foco/Cap60/ChecklistCritico.cs
namespace Jornada.Foco.Cap60;

public sealed class ChecklistCritico
{
    private readonly Dictionary<string, bool> itens;

    public ChecklistCritico(IEnumerable<string> itens) =>
        this.itens = itens.ToDictionary(i => i, _ => false);

    public void Marcar(string item)
    {
        if (!itens.ContainsKey(item))
            throw new KeyNotFoundException($"Item desconhecido: {item}");
        itens[item] = true;
    }

    public bool Completo => itens.Values.All(v => v);

    public IReadOnlyList<string> Pendentes => itens.Where(kv => !kv.Value).Select(kv => kv.Key).ToList();
}
```

```cs Uso
var deploy = new ChecklistCritico(new[]
{
    "Backup do banco confirmado",
    "Versão de produção anotada",
    "Plano de volta (rollback) lido",
});

deploy.Marcar("Backup do banco confirmado");
Console.WriteLine($"Completo? {deploy.Completo}");
Console.WriteLine("Faltam: " + string.Join("; ", deploy.Pendentes));

Console.WriteLine(EscolhaDeModo.Decidir(new Situacao(Familiaridade: 9)));
Console.WriteLine(EscolhaDeModo.Decidir(new Situacao(Familiaridade: 9, AltoRisco: true)));
Console.WriteLine(EscolhaDeModo.Decidir(new Situacao(Familiaridade: 3)));
```

```saida
Completo? False
Faltam: Versão de produção anotada; Plano de volta (rollback) lido
Automatico
Deliberado
Deliberado
```

## 3) Antes de reagir: a pausa

Quando uma emoção forte surge (um incidente às 3h, um comentário duro num *review*), o raciocínio deliberado **perde terreno**. Um protocolo simples é **esperar antes de reagir**, proporcional à intensidade:

```cs Jornada/Foco/Cap60/PausaAntesDeReagir.cs
namespace Jornada.Foco.Cap60;

public static class PausaAntesDeReagir
{
    /// <summary>Intensidade 0–10 → segundos sugeridos. Valores ILUSTRATIVOS: ajuste à sua realidade.</summary>
    public static int Segundos(int intensidade) => intensidade switch
    {
        < 0 or > 10 => throw new ArgumentOutOfRangeException(nameof(intensidade)),
        <= 3 => 0,
        <= 6 => 30,
        _    => 90,
    };
}
```

```cs Jornada/Foco/Cap60/PausaTests.cs
using Jornada.Foco.Cap60;
using Xunit;

public class Cap60Tests
{
    [Fact]
    public void Automatizar_zera_a_carga_do_passo()
    {
        var rotina = new[] { new Passo("A", 2, 10), new Passo("B", 1, 5) };
        var depois = PlanoDeAutomacao.Automatizar(rotina, "A");

        Assert.Equal(5, PlanoDeAutomacao.CargaTotal(depois));
        Assert.Equal(25, PlanoDeAutomacao.CargaTotal(rotina));
    }

    [Fact]
    public void Priorizar_ignora_passos_ja_automatizados()
    {
        var rotina = new[] { new Passo("A", 5, 5, Automatizado: true), new Passo("B", 1, 1) };
        Assert.Equal(new[] { "B" }, PlanoDeAutomacao.Priorizar(rotina).Select(p => p.Nome));
    }

    [Theory]
    [InlineData(9, false, false, ModoDeExecucao.Automatico)]
    [InlineData(9, true,  false, ModoDeExecucao.Deliberado)]
    [InlineData(9, false, true,  ModoDeExecucao.Deliberado)]
    [InlineData(4, false, false, ModoDeExecucao.Deliberado)]
    public void Escolha_de_modo(int fam, bool surpresa, bool risco, ModoDeExecucao esperado) =>
        Assert.Equal(esperado, EscolhaDeModo.Decidir(new Situacao(fam, surpresa, risco)));

    [Fact]
    public void Checklist_so_fica_completo_com_todos_os_itens()
    {
        var c = new ChecklistCritico(new[] { "x", "y" });
        c.Marcar("x");
        Assert.False(c.Completo);
        c.Marcar("y");
        Assert.True(c.Completo);
    }

    [Theory]
    [InlineData(2, 0)]
    [InlineData(5, 30)]
    [InlineData(9, 90)]
    public void Pausa_cresce_com_a_intensidade(int intensidade, int segundos) =>
        Assert.Equal(segundos, PausaAntesDeReagir.Segundos(intensidade));
}
```

::: curiosidade O "paradoxo do especialista"
Quanto mais você domina uma tarefa, **menos atenção** ela exige — e mais fácil é você deixar de notar o que mudou. Por isso pilotos, cirurgiões e equipes de operação usam **checklists** mesmo sendo experientes: não por falta de competência, mas porque o automático é cego por definição.
:::

::: exercicio Checklist com ordem
O checklist atual aceita marcar os itens em **qualquer ordem**. Em um deploy real, "Backup" deve vir **antes** de "Migrar o banco".
1. Crie `ChecklistOrdenado`, onde `Marcar` só aceita o **próximo** item da lista (caso contrário lança `InvalidOperationException`).
2. Escreva um teste que prove que marcar fora de ordem falha.

--- resposta
```cs Jornada/Foco/Cap60/ChecklistOrdenado.cs
namespace Jornada.Foco.Cap60;

public sealed class ChecklistOrdenado
{
    private readonly IReadOnlyList<string> itens;
    private int proximo;

    public ChecklistOrdenado(IEnumerable<string> itens) => this.itens = itens.ToList();

    public bool Completo => proximo == itens.Count;

    public void Marcar(string item)
    {
        if (Completo || itens[proximo] != item)
            throw new InvalidOperationException($"Agora é a vez de: {(Completo ? "(nada)" : itens[proximo])}");
        proximo++;
    }
}
```
```cs Jornada/Foco/Cap60/ChecklistOrdenadoTests.cs
using Jornada.Foco.Cap60;
using Xunit;

public class ChecklistOrdenadoTests
{
    [Fact]
    public void Fora_de_ordem_falha()
    {
        var c = new ChecklistOrdenado(new[] { "Backup", "Migrar" });
        Assert.Throws<InvalidOperationException>(() => c.Marcar("Migrar"));
    }

    [Fact]
    public void Na_ordem_certa_completa()
    {
        var c = new ChecklistOrdenado(new[] { "Backup", "Migrar" });
        c.Marcar("Backup");
        c.Marcar("Migrar");
        Assert.True(c.Completo);
    }
}
```
Ordem importa quando há **dependência real** entre os passos; se não importa, um checklist ordenado só cria atrito. Pergunte sempre: "esse passo existe por quê?".
:::

::: resumo
- **Dois modos:** automático (rápido, barato) e deliberado (lento, caro); **hábito** é a mente economizando esforço.
- **Automatizar a rotina** libera atenção; priorize pelo **custo de atenção × frequência** (os valores são estimativas, não medidas).
- O piloto automático **falha na surpresa** e onde o **risco é alto**: use **checklists** e modo deliberado nesses pontos.
- Sob emoção forte, vale uma **pausa proporcional** antes de reagir (os segundos do exemplo são ilustrativos).
- "Sequestro da amígdala" e *choke* são **lentes úteis**, não verdades definitivas.
:::

::: quiz
? O que caracteriza o sistema "de baixo para cima"?
+ É rápido, automático e exige pouco esforço
- É lento e deliberado
- Só funciona com meditação
- É exclusivo de especialistas
> O deliberado é o "de cima para baixo".

? Por que automatizar rotinas ajuda o foco?
+ Libera atenção limitada para o que é novo ou difícil
- Elimina a necessidade de testar
- Deixa o cérebro preguiçoso
- Evita qualquer erro
> A atenção é um recurso escasso.

? Por que um especialista ainda deve usar checklists?
+ O piloto automático é cego a mudanças e surpresas
- Porque especialistas esquecem tudo
- Por exigência burocrática
- Para atrasar o trabalho
> A automação do hábito pode esconder o que mudou.

? Como tratar o "sequestro da amígdala"?
+ Como metáfora útil, sem tomá-la como diagnóstico exato
- Como lei científica definitiva
- Como defeito de caráter
- Como algo que só acontece com atletas
> O cérebro é mais complexo que a imagem.

? Qual passo priorizar para automatizar no modelo do capítulo?
+ O de maior carga semanal (custo de atenção × frequência)
- O mais fácil de explicar
- O mais caro, qualquer que seja a frequência
- O que ninguém faz
> Barato e muito frequente pode pesar mais que caro e raro.

? Para que serve a pausa antes de reagir?
+ Dar tempo ao raciocínio deliberado quando a emoção está forte
- Evitar responder para sempre
- Aumentar o prazo do projeto
- Substituir testes
> Proporcional à intensidade.
:::

::: cartas
Sistema de baixo para cima :: Modo automático, rápido e intuitivo, que economiza esforço.
Sistema de cima para baixo :: Modo deliberado, lento e que exige atenção.
Hábito :: Rotina automatizada pela repetição.
Piloto automático :: Execução sem atenção consciente; eficiente, mas cega a mudanças.
Desatenção (Langer) :: Agir sem notar o que é novo, por "já saber".
Checklist crítico :: Lista de verificação nos pontos onde o hábito pode custar caro.
Sequestro emocional :: Metáfora para quando a emoção forte domina o raciocínio.
Pausa antes de reagir :: Intervalo proporcional à emoção, antes de agir.
:::
