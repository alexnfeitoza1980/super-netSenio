---
id: foco-cap68
order: 90
kind: capitulo
num: 68
group: C# & Mentalidade · Parte 13 · Foco em C#
title: Impacto, alavancas e a regra 80/20
subtitle: Dentro de um sistema, nem todo ponto pesa igual. Mapear quem é afetado por uma mudança, achar as poucas causas que explicam a maioria dos problemas e enxergar também o lado positivo do impacto.
minutes: 55
hue: 335
objectives:
- Fazer uma **análise de impacto**: quem é afetado, direta e indiretamente, por uma mudança
- Aplicar a **regra 80/20 (Pareto)** para achar as **causas vitais**
- Entender a **análise de ciclo de vida** como olhar para **todas as etapas**, não só a visível
- Trocar o foco só na "pegada" (dano) por também medir a "**pegada de mão**" (o bem que se faz)
- Reconhecer a **alfabetização em sistemas** como habilidade a praticar
origem: livros/04-foco-em-csharp/content/100-cap68.md
---

::: vila A tabela que mudou a conversa
A equipe lista, durante um trimestre, a causa de cada defeito em produção. Dos 100 casos, três causas respondem por 84. Lia aponta: "Estávamos tentando melhorar **tudo um pouco**. A tabela mostra onde **uma melhora grande** rende mais". E a Bia faz a outra pergunta que o livro sugere: "Se mudarmos o módulo de pagamentos, **quem mais sente**?".
:::

::: ilustracao img/foco/cap68.svg | Poucas causas explicam a maioria dos defeitos; e uma mudança num módulo derruba vários outros.
:::

## O que o livro diz

> Para agir bem num sistema, é preciso saber **onde ele é sensível** (as alavancas), **o que cada mudança toca** (o impacto indireto) e **o que acontece em todo o ciclo de vida**, não só na parte visível.

Pontos do capítulo original:

- A **análise do ciclo de vida** de um produto (do berço ao túmulo) revela custos escondidos: o que parece "ecológico" ou "barato" numa etapa pode **mover** o custo para outra.
- A **regra 80/20 (princípio de Pareto)**: com frequência, poucas causas (cerca de 20%) respondem pela maioria dos efeitos (cerca de 80%). Os números não são mágicos; o padrão de **concentração** é que importa.
- Em vez de enquadrar tudo como **redução de dano** ("pegada"), o livro apresenta a ideia de **"pegada de mão"** (*handprint*): medir também o **bem** que as ações produzem e como ampliá-lo.
- **Alfabetização em sistemas**: um conjunto de hábitos (desenhar o sistema, buscar atrasos, procurar alavancas) que se **aprende** com prática.
- Informação transparente sobre os **efeitos reais** muda o comportamento: quando o impacto fica **visível**, as pessoas reagem.

::: cuidado 80/20 não é lei natural
O "80/20" é uma **regra de bolso**: às vezes é 70/30, às vezes 95/5. E a análise de Pareto responde a **uma** pergunta (onde está a concentração de *ocorrências* ou *custo*), mas não diz se a causa mais comum é a **mais grave**. Combine frequência com **severidade**.
:::

## Da vida para o código

| No livro | Em engenharia de software |
|---|---|
| Alavanca | A **causa** que, atacada, reduz muitos defeitos |
| Impacto indireto | **Quem depende** de um módulo que vou mudar |
| Ciclo de vida | **Custo total**: escrever, operar, manter, desativar |
| Pareto 80/20 | **Poucos** tipos de bug respondem pela maioria |
| Pegada de mão | Medir o **valor entregue**, não só o dano evitado |
| Visibilidade muda comportamento | **Painéis** de qualidade e de custo |

::: figura Concentração: poucas barras, muito efeito
<svg viewBox="0 0 740 240" role="img" aria-label="Gráfico de Pareto: barras de causas em ordem decrescente e linha de percentual acumulado">
  <path class="arrow" d="M60 200L60 20"/><path class="arrow" d="M60 200L700 200"/>
  <g class="f-bad"><rect x="90" y="62" width="60" height="138"/></g>
  <g class="f-bad"><rect x="170" y="111" width="60" height="89"/></g>
  <g class="f-warn"><rect x="250" y="151" width="60" height="49"/></g>
  <g class="f-plain"><rect x="330" y="174" width="60" height="26"/></g>
  <g class="f-plain"><rect x="410" y="183" width="60" height="17"/></g>
  <g class="f-plain"><rect x="490" y="190" width="60" height="10"/></g>
  <polyline points="120,62 200,44 280,32 360,26 440,24 520,22" fill="none" stroke-width="3" style="stroke:var(--accent,#4f7cff)"/>
  <line x1="60" y1="38" x2="700" y2="38" class="f-info" stroke-dasharray="5 5" style="fill:none;stroke-width:1.5"/>
  <text x="698" y="32" text-anchor="end" font-size="11">80%</text>
  <g font-size="11" text-anchor="middle">
    <text x="120" y="216">Timeout</text><text x="200" y="216">Validação</text><text x="280" y="216">Nulos</text>
    <text x="360" y="216">Concorrência</text><text x="440" y="216">Config</text><text x="520" y="216">Outros</text>
  </g>
</svg>
:::

## 1) Análise de impacto: quem sente a mudança?

Em um sistema, uma mudança **não fica onde você a fez**. Um grafo de dependências permite perguntar, antes: "se eu mexer em X, **quem** pode quebrar?"

```cs Jornada/Foco/Cap68/GrafoDeDependencias.cs
namespace Jornada.Foco.Cap68;

public sealed class GrafoDeDependencias
{
    // chave: módulo; valor: módulos que dependem dele (quem "usa" a chave)
    private readonly Dictionary<string, HashSet<string>> usadoPor = new(StringComparer.Ordinal);

    /// <summary>Declara que 'modulo' depende de 'dependeDe'.</summary>
    public void Depende(string modulo, string dependeDe)
    {
        if (!usadoPor.TryGetValue(dependeDe, out var usuarios))
            usadoPor[dependeDe] = usuarios = new HashSet<string>(StringComparer.Ordinal);
        usuarios.Add(modulo);
    }

    /// <summary>Todos os módulos afetados, direta ou indiretamente, por uma mudança em 'alterado'.</summary>
    public IReadOnlyList<string> Impactados(string alterado)
    {
        var visitados = new HashSet<string>(StringComparer.Ordinal);
        var fila = new Queue<string>();
        fila.Enqueue(alterado);

        while (fila.Count > 0)
        {
            var atual = fila.Dequeue();
            if (!usadoPor.TryGetValue(atual, out var usuarios)) continue;

            foreach (var u in usuarios)
                if (u != alterado && visitados.Add(u))
                    fila.Enqueue(u);
        }

        return visitados.OrderBy(m => m, StringComparer.Ordinal).ToList();
    }
}
```

```cs Uso
var grafo = new GrafoDeDependencias();
grafo.Depende("Pedidos",      "Pagamentos");
grafo.Depende("Relatorios",   "Pagamentos");
grafo.Depende("Entrega",      "Pedidos");
grafo.Depende("Notificacoes", "Pedidos");
grafo.Depende("Auditoria",    "Relatorios");

foreach (var modulo in new[] { "Pagamentos", "Pedidos", "Entrega" })
{
    var impactados = grafo.Impactados(modulo);
    Console.WriteLine($"Mudar {modulo,-10} → {(impactados.Count == 0 ? "(ninguém)" : string.Join(", ", impactados))}");
}
```

```saida
Mudar Pagamentos → Auditoria, Entrega, Notificacoes, Pedidos, Relatorios
Mudar Pedidos    → Entrega, Notificacoes
Mudar Entrega    → (ninguém)
```

Uma pequena mudança em **Pagamentos** toca **cinco** módulos; uma mudança em **Entrega** não toca ninguém. Isso orienta **onde ter mais testes** e **quem avisar**. É a ideia de "efeitos, não colaterais" posta em código: os "efeitos colaterais" são os módulos que o grafo **mostra** e que você, antes, não enxergava.

::: dica O grafo mente por omissão
O grafo só conhece o que **foi declarado**. Dependências por **banco de dados compartilhado**, **filas**, **arquivos** ou **convenções** costumam faltar. Complete-o a partir da **realidade** (análise do código, rastreamento em produção), e trate as ausências como **risco**.
:::

## 2) Pareto: onde está a concentração?

```cs Jornada/Foco/Cap68/Pareto.cs
namespace Jornada.Foco.Cap68;

public sealed record Causa(string Nome, int Ocorrencias);

public sealed record LinhaDePareto(string Nome, int Ocorrencias, double PercentualAcumulado);

public static class Pareto
{
    public static IReadOnlyList<LinhaDePareto> Calcular(IEnumerable<Causa> causas)
    {
        var ordenadas = causas.OrderByDescending(c => c.Ocorrencias).ThenBy(c => c.Nome, StringComparer.Ordinal).ToList();
        int total = ordenadas.Sum(c => c.Ocorrencias);
        if (total == 0) throw new ArgumentException("É preciso haver ocorrências.", nameof(causas));

        int soma = 0;
        var linhas = new List<LinhaDePareto>();
        foreach (var c in ordenadas)
        {
            soma += c.Ocorrencias;
            linhas.Add(new LinhaDePareto(c.Nome, c.Ocorrencias, 100.0 * soma / total));
        }
        return linhas;
    }

    /// <summary>As causas necessárias para atingir (ou passar) o percentual acumulado de 'limite'.</summary>
    public static IReadOnlyList<string> Vitais(IReadOnlyList<LinhaDePareto> linhas, double limite = 80)
    {
        var vitais = new List<string>();
        foreach (var l in linhas)
        {
            vitais.Add(l.Nome);
            if (l.PercentualAcumulado >= limite) break;
        }
        return vitais;
    }
}
```

```cs Uso
var causas = new[]
{
    new Causa("Timeout de integração", 42),
    new Causa("Validação de entrada",  27),
    new Causa("Dados nulos",           15),
    new Causa("Concorrência",           8),
    new Causa("Configuração errada",    5),
    new Causa("Outros",                 3),
};

var linhas = Pareto.Calcular(causas);
foreach (var l in linhas)
    Console.WriteLine($"{l.Nome,-23} {l.Ocorrencias,3}  acumulado {l.PercentualAcumulado,5:F1}%");

Console.WriteLine("Causas vitais (≥ 80%): " + string.Join("; ", Pareto.Vitais(linhas)));
```

```saida
Timeout de integração    42  acumulado  42,0%
Validação de entrada     27  acumulado  69,0%
Dados nulos              15  acumulado  84,0%
Concorrência              8  acumulado  92,0%
Configuração errada       5  acumulado  97,0%
Outros                    3  acumulado 100,0%
Causas vitais (≥ 80%): Timeout de integração; Validação de entrada; Dados nulos
```

Três de seis causas respondem por **84%** dos defeitos. A equipe não precisa "melhorar tudo": pode atacar **timeouts, validação e nulos** e eliminar cerca de **84** dos 100 casos. (Se um dos defeitos "raros" derrubar o banco por uma hora, **severidade** importa mais do que frequência: use Pareto também com o **custo** de cada causa.)

```cs JornadaTests/Foco/Cap68/Cap68Tests.cs
using Jornada.Foco.Cap68;
using Xunit;

public class Cap68Tests
{
    [Fact]
    public void Impacto_inclui_dependencias_indiretas()
    {
        var g = new GrafoDeDependencias();
        g.Depende("B", "A");
        g.Depende("C", "B");
        Assert.True(g.Impactados("A").SequenceEqual(new[] { "B", "C" }));
    }

    [Fact]
    public void Modulo_sem_dependentes_nao_impacta_ninguem() =>
        Assert.Empty(new GrafoDeDependencias().Impactados("X"));

    [Fact]
    public void Ciclos_nao_geram_laco_infinito()
    {
        var g = new GrafoDeDependencias();
        g.Depende("B", "A");
        g.Depende("A", "B");
        Assert.True(g.Impactados("A").SequenceEqual(new[] { "B" }));
    }

    [Fact]
    public void Pareto_acumula_ate_cem()
    {
        var linhas = Pareto.Calcular(new[] { new Causa("a", 3), new Causa("b", 1) });
        Assert.Equal(75, linhas[0].PercentualAcumulado, 6);
        Assert.Equal(100, linhas[^1].PercentualAcumulado, 6);
    }

    [Fact]
    public void Vitais_param_ao_atingir_o_limite()
    {
        var linhas = Pareto.Calcular(new[] { new Causa("a", 80), new Causa("b", 15), new Causa("c", 5) });
        Assert.True(Pareto.Vitais(linhas).SequenceEqual(new[] { "a" }));
    }

    [Fact]
    public void Sem_ocorrencias_e_rejeitado() =>
        Assert.Throws<ArgumentException>(() => Pareto.Calcular(new[] { new Causa("a", 0) }));
}
```

## 3) Pegada e "pegada de mão"

Quase tudo o que medimos em qualidade é **dano evitado**: defeitos, incidentes, dívida. Isso é a "pegada": o que **deixamos de piorar**. A ideia do **handprint** é medir também o **bem**: tempo que as pessoas **economizam** com o sistema, erros que **deixam de cometer**, acessibilidade que **ganham**. Em um painel, as duas colunas lado a lado mudam a **conversa**: a equipe deixa de ser só "quem causa problema" e passa a ser "quem entrega valor".

Uma regra prática para **escolher o quadro** de uma meta: prefira o **enquadramento positivo** quando ele for **verdadeiro e mensurável** ("ganhar 2 horas por semana para cada pessoa") em vez de só "reduzir defeitos em 30%". Mas cuidado: o positivo **não** substitui a conta do dano; um sistema que entrega muito valor e vaza dados continua em **dívida**.

::: exercicio Pareto por custo
Cada causa também tem um **custo médio** em horas. Crie `CausaComCusto(string Nome, int Ocorrencias, double HorasPorOcorrencia)` e uma função que **ordene por custo total** (`Ocorrencias × HorasPorOcorrencia`). Mostre com um teste que a causa **mais frequente** pode **não** ser a **mais cara**.

--- resposta
```cs Jornada/Foco/Cap68/ParetoPorCusto.cs
namespace Jornada.Foco.Cap68;

public sealed record CausaComCusto(string Nome, int Ocorrencias, double HorasPorOcorrencia)
{
    public double CustoTotal => Ocorrencias * HorasPorOcorrencia;
}

public static class ParetoPorCusto
{
    public static IReadOnlyList<CausaComCusto> OrdenarPorCusto(IEnumerable<CausaComCusto> causas) =>
        causas.OrderByDescending(c => c.CustoTotal).ThenBy(c => c.Nome, StringComparer.Ordinal).ToList();
}
```
```cs Jornada/Foco/Cap68/ParetoPorCustoTests.cs
using Jornada.Foco.Cap68;
using Xunit;

public class ParetoPorCustoTests
{
    [Fact]
    public void A_mais_frequente_pode_nao_ser_a_mais_cara()
    {
        var causas = new[]
        {
            new CausaComCusto("Timeout", 42, 0.5),    // 21 h
            new CausaComCusto("Perda de dados", 3, 40), // 120 h
        };

        var porCusto = ParetoPorCusto.OrdenarPorCusto(causas);
        var maisFrequente = causas.OrderByDescending(c => c.Ocorrencias).First();

        Assert.Equal("Perda de dados", porCusto[0].Nome);
        Assert.NotEqual(maisFrequente.Nome, porCusto[0].Nome);
    }
}
```
A decisão de onde atuar primeiro quase sempre pede **os dois eixos**: frequência e severidade.
:::

::: resumo
- A **análise de impacto** com um grafo de dependências mostra quem uma mudança toca, direta e indiretamente: os "efeitos colaterais" ficam **visíveis**.
- O grafo **só conhece o que foi declarado**: complete-o com a realidade e trate lacunas como risco.
- **Pareto (80/20)** aponta a concentração de ocorrências; é regra de bolso e deve ser combinada com **severidade/custo**.
- A **análise de ciclo de vida** lembra que o custo se desloca entre as etapas.
- Meça também a "**pegada de mão**" (o valor entregue), sem abandonar a conta do dano.
:::

::: quiz
? O que a análise de impacto mostra?
+ Quais módulos são afetados, direta ou indiretamente, por uma mudança
- Qual desenvolvedor é o mais produtivo
- Quantas linhas de código existem
- O custo do servidor
> É a "visão sistêmica" de uma mudança.

? Por que o grafo de dependências pode enganar?
+ Só conhece as dependências declaradas; banco compartilhado, filas e convenções costumam faltar
- Porque o C# não permite grafos
- Porque é sempre excessivo
- Porque calcula errado a média
> Complete com a realidade.

? O que diz a regra 80/20?
+ Poucas causas costumam responder pela maior parte dos efeitos (como regra de bolso)
- Que 80% do código é lixo
- Que 20% das pessoas trabalham
- Uma lei exata da natureza
> Os números variam; o padrão de concentração é o ponto.

? Qual o limite de usar só a frequência no Pareto?
+ A causa mais comum pode não ser a mais grave; combine com severidade ou custo
- Nenhum
- Frequência não pode ser medida
- Causas raras não existem
> Perda de dados raros pode custar mais do que muitos timeouts.

? O que é a "pegada de mão"?
+ Medir também o bem que as ações produzem, além do dano evitado
- Um tipo de gráfico
- Uma dependência circular
- Um erro de memória
> Enquadramento positivo, sem abandonar a conta do dano.

? Por que os ciclos devem ser tratados na busca em largura?
+ Para evitar laço infinito, marcando os módulos já visitados
- Para aumentar a pontuação
- Porque ciclos são proibidos
- Porque o C# otimiza
> O `visitados.Add` faz esse papel.
:::

::: cartas
Alavanca :: Ponto do sistema onde uma ação pequena rende efeito grande.
Análise de impacto :: Descobrir quem é afetado, direta e indiretamente, por uma mudança.
Ciclo de vida :: Todas as etapas, do início ao fim, e seus custos.
Pareto (80/20) :: Poucas causas costumam explicar a maioria dos efeitos.
Causa vital :: Causa que está na parte concentrada do Pareto.
Pegada :: O dano que uma ação causa.
Pegada de mão (handprint) :: O bem que uma ação produz.
Alfabetização em sistemas :: Hábitos para enxergar laços, atrasos e alavancas.
:::
