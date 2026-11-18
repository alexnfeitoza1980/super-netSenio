---
id: leis-lei04
order: 65
kind: capitulo
num: 45
group: C# & Mentalidade · Parte 12 · As 16 leis do sucesso em C#
title: Lei 4 — O hábito da economia
subtitle: Gastar menos do que se ganha também vale para memória, tempo e atenção. Dívida técnica como dívida de verdade, a "reserva de 10%" da iteração, IDisposable, Span, StringBuilder e um cache LRU.
minutes: 55
hue: 95
objectives:
- Traduzir a lei da **economia** em dois "orçamentos": o de **recursos do programa** e o de **capacidade da equipe**
- Tratar **dívida técnica** como dívida financeira: juros, custo de quitação e **ordem de pagamento**
- Reservar uma **fatia fixa** de cada iteração para quitar dívidas ("pague-se primeiro")
- Economizar **memória e CPU** com `using`/`IDisposable`, `StringBuilder`, `Span<T>`, `ArrayPool<T>` e um **cache LRU**
- Evitar o exagero oposto: otimização prematura
origem: livros/03-leis-do-sucesso-csharp/content/64-lei04.md
---

::: vila O mês sem sobra
Lia olha o extrato do cartão e depois olha o repositório. Os dois têm algo em comum: **cada pequena conveniência "por enquanto" virou um juro mensal.** O cartão cobra juros em reais; o código cobra em **horas perdidas** — testes de 12 minutos, um deploy manual, uma configuração copiada em cinco lugares. Lia decide aplicar a mesma disciplina às duas contas.
:::

::: ilustracao img/leis/lei04.svg | Economia: tapar os vazamentos (memória, tempo) e guardar uma reserva de capacidade.
:::

## A lei, em uma frase

> Quem gasta mais do que ganha vive em escassez permanente e perde disciplina, autoconfiança e credibilidade; por isso, é preciso **separar uma parte fixa de tudo o que se recebe** (a lei sugere pelo menos 10%), **quitar as dívidas** começando pelas de juros mais altos, e só então **investir** — e, sobretudo, **criar o hábito**, porque qualquer hábito pode ser substituído por outro.

A lição lista um roteiro prático para sair das dívidas: **(1)** identificar todas; **(2)** criar um orçamento; **(3)** renegociar; **(4)** pagar, começando pelos juros mais altos; **(5)** seguir o plano com rigor; **(6)** começar a investir.

## Da vida para o código

| O que a lei diz | O que isso significa para quem programa |
|---|---|
| Não gastar mais do que ganha | Não consumir mais **memória, CPU, conexões e tempo** do que o necessário |
| Identificar as dívidas | **Inventariar a dívida técnica** (o que custa horas toda semana?) |
| Pagar primeiro os juros mais altos | **Ordenar a quitação** pelo impacto (e pelo retorno) |
| Guardar ao menos 10% de tudo | **Reservar uma fatia da iteração** para refatoração e melhoria |
| Orçamento rigoroso | **Orçamentos** de desempenho, de memória e de capacidade |
| Investir depois de quitar | Investir em **automação, testes e ferramentas** |
| Hábito, não esforço pontual | Uma **política permanente** (por exemplo, "regra do escoteiro") |

::: figura Reserva de 10% da iteração
<svg viewBox="0 0 700 150" role="img" aria-label="Barra de 200 horas: 180 para funcionalidades e 20 de reserva para dívida técnica">
  <rect class="f-prim" x="20" y="40" width="540" height="50" rx="8"/><text x="290" y="70" text-anchor="middle" font-weight="800">180 h — funcionalidades novas</text>
  <rect class="f-good" x="560" y="40" width="120" height="50" rx="8"/><text x="620" y="62" text-anchor="middle" font-weight="800">20 h</text><text x="620" y="80" text-anchor="middle" font-size="11">reserva (10%)</text>
  <text x="350" y="125" text-anchor="middle" class="t-dim" font-size="12">Uma iteração de 200 h: primeiro separa-se a reserva; depois planeja-se o resto.</text>
</svg>
:::

## Dívida técnica: uma dívida de verdade

Uma dívida técnica tem **juros** (as horas que ela faz você perder a cada semana) e **custo de quitação** (as horas que levaria para consertá-la). Com esses dois números dá para tomar decisões racionais — as mesmas que um educador financeiro recomendaria:

```cs Jornada/Lei04/Divida.cs
namespace Jornada.Lei04;

public sealed record Divida(string Nome, double HorasPerdidasPorSemana, double HorasParaQuitar)
{
    /// <summary>Em quantas semanas o conserto se paga sozinho (menor = melhor investimento).</summary>
    public double SemanasParaRetorno => HorasParaQuitar / HorasPerdidasPorSemana;
}

public static class Quitacao
{
    /// <summary>"Avalanche": primeiro os juros mais altos (o que mais custa por semana).</summary>
    public static IReadOnlyList<Divida> Avalanche(IEnumerable<Divida> dividas) =>
        dividas.OrderByDescending(d => d.HorasPerdidasPorSemana).ThenBy(d => d.HorasParaQuitar).ToList();

    /// <summary>"Bola de neve": primeiro o que é mais barato de consertar (vitórias rápidas).</summary>
    public static IReadOnlyList<Divida> BolaDeNeve(IEnumerable<Divida> dividas) =>
        dividas.OrderBy(d => d.HorasParaQuitar).ToList();

    /// <summary>Por retorno do investimento: o conserto que se paga mais depressa vem antes.</summary>
    public static IReadOnlyList<Divida> PorRetorno(IEnumerable<Divida> dividas) =>
        dividas.OrderBy(d => d.SemanasParaRetorno).ToList();
}
```

```cs Uso
var dividas = new[]
{
    new Divida("Testes lentos (12 min)",          HorasPerdidasPorSemana: 6, HorasParaQuitar: 16),
    new Divida("Relatórios sem testes",           HorasPerdidasPorSemana: 3, HorasParaQuitar: 40),
    new Divida("Configuração copiada em 5 pontos",HorasPerdidasPorSemana: 2, HorasParaQuitar: 4),
    new Divida("Deploy manual",                   HorasPerdidasPorSemana: 5, HorasParaQuitar: 12),
};

void Mostrar(string titulo, IEnumerable<Divida> ordem)
{
    Console.WriteLine(titulo);
    foreach (var d in ordem)
        Console.WriteLine($"  {d.Nome,-34} juros {d.HorasPerdidasPorSemana,2} h/sem · custo {d.HorasParaQuitar,2} h · retorno {d.SemanasParaRetorno,4:F1} sem");
}

Mostrar("Avalanche (maior juro primeiro):", Quitacao.Avalanche(dividas));
Mostrar("Bola de neve (mais barata primeiro):", Quitacao.BolaDeNeve(dividas));
Mostrar("Por retorno (se paga mais rápido primeiro):", Quitacao.PorRetorno(dividas));
```

```saida
Avalanche (maior juro primeiro):
  Testes lentos (12 min)             juros  6 h/sem · custo 16 h · retorno  2,7 sem
  Deploy manual                      juros  5 h/sem · custo 12 h · retorno  2,4 sem
  Relatórios sem testes              juros  3 h/sem · custo 40 h · retorno 13,3 sem
  Configuração copiada em 5 pontos   juros  2 h/sem · custo  4 h · retorno  2,0 sem
Bola de neve (mais barata primeiro):
  Configuração copiada em 5 pontos   juros  2 h/sem · custo  4 h · retorno  2,0 sem
  Deploy manual                      juros  5 h/sem · custo 12 h · retorno  2,4 sem
  Testes lentos (12 min)             juros  6 h/sem · custo 16 h · retorno  2,7 sem
  Relatórios sem testes              juros  3 h/sem · custo 40 h · retorno 13,3 sem
Por retorno (se paga mais rápido primeiro):
  Configuração copiada em 5 pontos   juros  2 h/sem · custo  4 h · retorno  2,0 sem
  Deploy manual                      juros  5 h/sem · custo 12 h · retorno  2,4 sem
  Testes lentos (12 min)             juros  6 h/sem · custo 16 h · retorno  2,7 sem
  Relatórios sem testes              juros  3 h/sem · custo 40 h · retorno 13,3 sem
```

::: pense Qual ordem escolher?
A lei diz "juros mais altos primeiro" (avalanche) — que **minimiza o custo total**. Mas a *bola de neve* dá vitórias rápidas que **mantêm o hábito vivo** (e, como vimos, o hábito é o que importa). A ordenação **por retorno** equilibra as duas. Repare que o item "Relatórios sem testes" fica por último nas três: é caro e rende pouco *agora* — mas pode ser o que mais **risco** carrega. Dívida não é só juro; é também **risco**. Decida com a equipe.
:::

## "Pague-se primeiro": a reserva da iteração

Quem espera "sobrar" dinheiro para poupar nunca poupa. Quem separa **no dia em que recebe**, consegue. Em equipes é igual: se a refatoração só entra "quando sobrar tempo", **nunca entra**. Então, reserve primeiro:

```cs Jornada/Lei04/Capacidade.cs
namespace Jornada.Lei04;

public sealed record Capacidade(int HorasTotais, double PercentualReserva = 0.10)
{
    public double HorasDeReserva          => HorasTotais * PercentualReserva;
    public double HorasParaFuncionalidades => HorasTotais - HorasDeReserva;

    /// <summary>Escolhe, em ordem de retorno, as dívidas que cabem na reserva.</summary>
    public IReadOnlyList<Divida> DividasQueCabem(IEnumerable<Divida> dividas)
    {
        double restante = HorasDeReserva;
        var escolhidas = new List<Divida>();

        foreach (var d in Quitacao.PorRetorno(dividas))
        {
            if (d.HorasParaQuitar > restante) continue;
            escolhidas.Add(d);
            restante -= d.HorasParaQuitar;
        }
        return escolhidas;
    }
}
```

```cs Uso (continuação)
var capacidade = new Capacidade(HorasTotais: 200);

Console.WriteLine($"Funcionalidades: {capacidade.HorasParaFuncionalidades} h · reserva: {capacidade.HorasDeReserva} h");
foreach (var d in capacidade.DividasQueCabem(dividas))
    Console.WriteLine($"  quitar nesta iteração → {d.Nome}");
```

```saida
Funcionalidades: 180 h · reserva: 20 h
  quitar nesta iteração → Configuração copiada em 5 pontos
  quitar nesta iteração → Deploy manual
```

::: dica A "regra do escoteiro"
Um hábito complementar: **deixe o código um pouco melhor do que o encontrou**. Todo *commit* paga um centavo da dívida. Combinada com a reserva fixa, essa é a versão "pequenas economias diárias" do hábito.
:::

## Economizar recursos do programa

O mesmo princípio vale para o que o programa consome. Quatro economias clássicas em C#:

### 1. Devolver o que foi emprestado: `using` e `IDisposable`

Conexões, arquivos e *sockets* são recursos escassos. `using` garante que serão **devolvidos** mesmo se ocorrer uma exceção:

```cs Jornada/Lei04/ConexaoFalsa.cs
namespace Jornada.Lei04;

public sealed class ConexaoFalsa : IDisposable
{
    public string Nome { get; }
    public ConexaoFalsa(string nome) { Nome = nome; Console.WriteLine($"abriu {nome}"); }
    public void Dispose() => Console.WriteLine($"fechou {Nome}");
}
```

```cs Uso
using (var a = new ConexaoFalsa("A"))
{
    Console.WriteLine("usando A");
}

try
{
    using var b = new ConexaoFalsa("B");        // fecha ao sair do escopo, mesmo com erro
    throw new InvalidOperationException("falha no meio do trabalho");
}
catch (InvalidOperationException e)
{
    Console.WriteLine("tratou: " + e.Message);
}
```

```saida
abriu A
usando A
fechou A
abriu B
fechou B
tratou: falha no meio do trabalho
```

### 2. Não desperdiçar memória: `StringBuilder`, `Span<T>`, `ArrayPool<T>`

```cs Jornada/Lei04/Economias.cs
using System.Buffers;
using System.Text;

namespace Jornada.Lei04;

public static class Textos
{
    // Ruim em laços grandes: cada += cria uma nova string e descarta a anterior.
    public static string JuntarRuim(IEnumerable<string> partes)
    {
        var s = "";
        foreach (var p in partes) s += p + ";";
        return s;
    }

    // Bom: um único buffer que cresce conforme o necessário.
    public static string JuntarBom(IEnumerable<string> partes)
    {
        var sb = new StringBuilder();
        foreach (var p in partes) sb.Append(p).Append(';');
        return sb.ToString();
    }

    // Span: examina o texto "sem copiar" — nenhuma string nova é alocada.
    public static int ContarCampos(ReadOnlySpan<char> linha)
    {
        int n = 1;
        foreach (var c in linha) if (c == ';') n++;
        return n;
    }

    public static int AnoDe(string dataIso) => int.Parse(dataIso.AsSpan(0, 4));   // sem Substring
}

public static class Buffers
{
    // Empresta um array do pool, usa e devolve — em vez de alocar um novo a cada chamada.
    public static int SomarBytes(ReadOnlySpan<byte> origem)
    {
        var pool = ArrayPool<byte>.Shared;
        byte[] buffer = pool.Rent(origem.Length);
        try
        {
            origem.CopyTo(buffer);
            int soma = 0;
            for (int i = 0; i < origem.Length; i++) soma += buffer[i];
            return soma;
        }
        finally
        {
            pool.Return(buffer);
        }
    }
}
```

```cs JornadaTests/Lei04/EconomiasTests.cs
using Jornada.Lei04;
using Xunit;

public class EconomiasTests
{
    [Fact]
    public void As_duas_versoes_dao_o_mesmo_resultado()
    {
        var partes = new[] { "a", "b", "c" };
        Assert.Equal(Textos.JuntarRuim(partes), Textos.JuntarBom(partes));
    }

    [Fact]
    public void Conta_campos_sem_alocar_strings()
    {
        Assert.Equal(3, Textos.ContarCampos("ana;bia;caio"));
        Assert.Equal(2026, Textos.AnoDe("2026-10-26"));
        Assert.Equal(10, Buffers.SomarBytes(new byte[] { 1, 2, 3, 4 }));
    }
}
```

::: cuidado Economia não é avareza (nem otimização prematura)
A mesma lei que manda poupar manda **investir** com sabedoria. Otimizar sem medir é gastar tempo (o recurso mais caro) para economizar centavos de memória. Regra prática: **escreva primeiro com clareza; meça (BenchmarkDotNet, *profiler*); otimize onde os números mostrarem**. `StringBuilder` num laço de milhares de itens vale a pena; em duas concatenações, só atrapalha a leitura.
:::

### 3. Guardar com limite: um cache LRU

Um **cache** guarda resultados caros de calcular — mas, sem limite, ele próprio vira um vazamento de memória. Um cache **LRU** (*least recently used*) tem **capacidade fixa**: ao encher, descarta o item menos usado recentemente. É a "conta poupança com teto".

```cs Jornada/Lei04/CacheLru.cs
namespace Jornada.Lei04;

public sealed class CacheLru<TChave, TValor> where TChave : notnull
{
    private readonly int capacidade;
    private readonly Dictionary<TChave, LinkedListNode<(TChave Chave, TValor Valor)>> indice = new();
    private readonly LinkedList<(TChave Chave, TValor Valor)> ordem = new();   // frente = mais recente

    public CacheLru(int capacidade)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(capacidade);
        this.capacidade = capacidade;
    }

    public int Quantidade => indice.Count;

    public bool TentarObter(TChave chave, out TValor? valor)
    {
        if (indice.TryGetValue(chave, out var no))
        {
            ordem.Remove(no);                 // "usou": passa para a frente
            ordem.AddFirst(no);
            valor = no.Value.Valor;
            return true;
        }
        valor = default;
        return false;
    }

    public void Colocar(TChave chave, TValor valor)
    {
        if (indice.TryGetValue(chave, out var existente))
        {
            ordem.Remove(existente);
            indice.Remove(chave);
        }
        else if (indice.Count >= capacidade)
        {
            var maisAntigo = ordem.Last!;     // o menos usado recentemente
            ordem.RemoveLast();
            indice.Remove(maisAntigo.Value.Chave);
        }

        var no = new LinkedListNode<(TChave Chave, TValor Valor)>((chave, valor));
        ordem.AddFirst(no);
        indice[chave] = no;
    }
}
```

```cs JornadaTests/Lei04/CacheLruTests.cs
using Jornada.Lei04;
using Xunit;

public class CacheLruTests
{
    [Fact]
    public void Descarta_o_menos_usado_ao_encher()
    {
        var cache = new CacheLru<string, int>(capacidade: 2);
        cache.Colocar("a", 1);
        cache.Colocar("b", 2);
        cache.TentarObter("a", out _);      // 'a' foi usado: agora 'b' é o mais antigo
        cache.Colocar("c", 3);               // estoura a capacidade: 'b' sai

        Assert.False(cache.TentarObter("b", out _));
        Assert.True(cache.TentarObter("a", out var a));
        Assert.Equal(1, a);
        Assert.True(cache.TentarObter("c", out _));
        Assert.Equal(2, cache.Quantidade);
    }

    [Fact]
    public void Capacidade_invalida_e_recusada()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new CacheLru<string, int>(0));
    }
}
```

::: exercicio Orçamento com limite de dívidas
A equipe decidiu que, além da reserva de 10% das horas, **no máximo 2 dívidas** podem ser atacadas por iteração (para manter o foco — Lei 12). Altere `DividasQueCabem` para aceitar um parâmetro `maximo` e use o exemplo de 200 horas com 4 dívidas.

--- resposta
```cs Capacidade.cs (trecho)
public IReadOnlyList<Divida> DividasQueCabem(IEnumerable<Divida> dividas, int maximo = int.MaxValue)
{
    double restante = HorasDeReserva;
    var escolhidas = new List<Divida>();

    foreach (var d in Quitacao.PorRetorno(dividas))
    {
        if (escolhidas.Count >= maximo) break;     // limite de foco
        if (d.HorasParaQuitar > restante) continue;
        escolhidas.Add(d);
        restante -= d.HorasParaQuitar;
    }
    return escolhidas;
}

// new Capacidade(200).DividasQueCabem(dividas, maximo: 2) → "Configuração…" e "Deploy manual"
// Com maximo: 1 → só "Configuração copiada em 5 pontos".
```
Observe a tensão de projeto: dois limites (horas e quantidade) competindo — é bem parecido com orçamentos reais.
:::

::: resumo
- **Lei 4:** gaste menos do que ganha; separe uma parte fixa (≥10%) de tudo o que recebe; quite dívidas (juros altos primeiro); depois invista; faça disso um **hábito**.
- **Dívida técnica** tem *juros* (horas perdidas por semana) e *custo de quitação*; ordens possíveis: **avalanche**, **bola de neve** e **por retorno**.
- **"Pague-se primeiro":** reserve (por exemplo) 10% das horas da iteração para quitar dívidas **antes** de planejar o resto.
- **Recursos do programa:** `using`/`IDisposable` devolve o que foi emprestado; `StringBuilder`, `Span<T>` e `ArrayPool<T>` evitam alocações; um **cache LRU** guarda com limite.
- **Meça antes de otimizar** — economia não é avareza nem otimização prematura.
:::

::: quiz
? Qual o equivalente, na engenharia de software, dos "juros" de uma dívida financeira?
+ As horas que a dívida técnica faz a equipe perder repetidamente
- O número de linhas de código
- O tamanho do repositório
- O preço da licença do IDE
> Os juros são o custo recorrente de conviver com o problema.

? O que significa "pagar-se primeiro" no planejamento de uma iteração?
+ Separar a reserva (por exemplo 10%) para dívidas antes de planejar as funcionalidades
- Fazer refatoração só quando sobrar tempo
- Pagar o time antes do prazo
- Evitar estimativas
> O que depende de "sobrar" nunca acontece.

? Por que o `using` ajuda na economia de recursos?
+ Garante que o recurso seja devolvido (Dispose) mesmo se ocorrer uma exceção
- Torna o recurso gratuito
- Evita o uso de classes
- Impede exceções
> Conexões e arquivos são recursos escassos.

? Quando `StringBuilder` compensa?
+ Em laços com muitas concatenações; em poucas concatenações apenas piora a legibilidade
- Sempre, em qualquer concatenação
- Nunca
- Só em Windows
> Meça antes de otimizar.

? Qual a característica de um cache LRU?
+ Capacidade fixa; descarta o item menos usado recentemente ao encher
- Nunca descarta nada
- Descarta aleatoriamente
- Guarda apenas números
> O limite evita que o cache vire vazamento de memória.

? A estratégia "bola de neve" para quitar dívidas prioriza...
+ O que é mais barato de consertar, para ganhar vitórias rápidas
- O maior juro
- O mais antigo
- O mais arriscado
> Mantém a motivação e o hábito vivos.
:::

::: cartas
Hábito da economia :: Separar sempre uma parte do que se recebe e gastar menos do que se ganha.
Dívida técnica :: Atalho no código que cobra "juros" em horas perdidas até ser quitado.
Avalanche :: Quitar primeiro a dívida de juros mais altos.
Bola de neve :: Quitar primeiro a dívida mais barata de consertar.
Reserva da iteração :: Fatia fixa do tempo reservada para refatoração e melhoria.
IDisposable / using :: Mecanismo para liberar recursos de forma determinística.
Span<T> :: Visão sobre memória contígua sem copiar dados.
Cache LRU :: Cache de capacidade fixa que descarta o item menos usado recentemente.
:::
