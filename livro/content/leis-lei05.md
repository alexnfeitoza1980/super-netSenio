---
id: leis-lei05
order: 66
kind: capitulo
num: 46
group: C# & Mentalidade · Parte 12 · As 16 leis do sucesso em C#
title: Lei 5 — Iniciativa e liderança
subtitle: Começar sem esperar ordem, vencer a procrastinação e liderar de verdade — em C#, isso é async/await, Task.WhenAll, Parallel.ForEachAsync, IProgress e verificações proativas.
minutes: 55
hue: 12
objectives:
- Entender a lei: **iniciativa** é fazer o necessário sem que mandem, e a **liderança** nasce dela
- Usar **`async/await`** com *iniciativa*: iniciar o trabalho independente **antes** de esperar o resultado
- Construir um **"líder"** que orquestra tarefas com limite de simultaneidade, isolando falhas
- Reportar progresso com **`IProgress<T>`** e cancelar com **`CancellationToken`**
- Criar **verificações proativas** com `BackgroundService` e `PeriodicTimer`
origem: livros/03-leis-do-sucesso-csharp/content/65-lei05.md
---

::: vila O alerta que ninguém pediu
Numa terça-feira, Lia nota que a fila de e-mails da empresa está crescendo devagar. Ninguém reclamou, nenhum alarme tocou. Em vez de "esperar alguém pedir", ela escreve uma verificação que avisa quando a fila passa de mil itens. Na sexta, o alarme dispara **antes** de qualquer cliente perceber. Foi a *iniciativa* de Lia — não uma tarefa de ninguém — que evitou um incidente.
:::

::: ilustracao img/leis/lei05.svg | Iniciativa: começar sem esperar e deixar tarefas independentes correrem em paralelo.
:::

## A lei, em uma frase

> Sonhos, desejos e propósitos **sem ação** servem para pouco: é preciso **iniciativa** — fazer o que precisa ser feito **sem que ninguém mande** — e dela nasce a **liderança**, pois quem age primeiro e organiza os meios para o propósito acaba conduzindo os outros.

A lição propõe um roteiro de quatro passos para agir diante de uma ideia ousada: **(1) questionar** (estudar quem já fez algo parecido), **(2) analisar vantagens e obstáculos** (e imaginar alternativas), **(3) simular** (colocar um plano no papel, passo a passo) e **(4) assumir a responsabilidade e agir**. E um treino contra a procrastinação: **fazer todos os dias uma coisa útil que ninguém pediu**.

## Da vida para o código

| O que a lei diz | O que isso significa para quem programa |
|---|---|
| Agir sem esperar ordem | **Iniciar** operações independentes em paralelo (`Task`) |
| Procrastinação | **Esperar sem necessidade** (`await` em fila quando não há dependência) |
| Líder organiza os meios | Um **orquestrador** que distribui trabalho com limites e isola falhas |
| Fazer algo útil que ninguém pediu | **Verificações proativas**, alertas, testes e logs que ninguém cobrou |
| Assumir a responsabilidade | Tratar **erros e cancelamento** em vez de ignorá-los |
| Liderar é inspirar | Dar **visibilidade** do progresso (`IProgress<T>`) |

::: figura Procrastinar × tomar iniciativa
<svg viewBox="0 0 740 190" role="img" aria-label="Sequencial: três tarefas em fila, 500 ms. Com iniciativa: três tarefas ao mesmo tempo, 200 ms">
  <text x="10" y="20" font-weight="800" class="t-bad">Esperando uma por vez  ≈ 500 ms</text>
  <rect class="f-prim" x="10" y="30" width="100" height="26" rx="5"/><text x="60" y="48" text-anchor="middle" font-size="12">A · 100</text>
  <rect class="f-info" x="110" y="30" width="200" height="26" rx="5"/><text x="210" y="48" text-anchor="middle" font-size="12">B · 200</text>
  <rect class="f-good" x="310" y="30" width="200" height="26" rx="5"/><text x="410" y="48" text-anchor="middle" font-size="12">C · 200</text>
  <text x="10" y="100" font-weight="800" class="t-good">Com iniciativa (todas começam já)  ≈ 200 ms</text>
  <rect class="f-prim" x="10" y="110" width="100" height="26" rx="5"/><text x="60" y="128" text-anchor="middle" font-size="12">A · 100</text>
  <rect class="f-info" x="10" y="140" width="200" height="26" rx="5"/><text x="110" y="158" text-anchor="middle" font-size="12">B · 200</text>
  <rect class="f-good" x="10" y="170" width="200" height="14" rx="5"/><text x="110" y="181" text-anchor="middle" font-size="10">C · 200</text>
  <path d="M10 76H520" stroke="var(--line)"/>
</svg>
:::

## Iniciativa em `async/await`: comece antes de esperar

O erro mais comum com `await` é **esperar demais**: chamar uma operação, aguardar o resultado, só então chamar a próxima — mesmo quando uma não depende da outra. É a procrastinação em forma de código.

```cs Jornada/Lei05/Trabalho.cs
namespace Jornada.Lei05;

public sealed record Tarefa(string Nome, int DuracaoMs);

public static class Trabalho
{
    public static async Task<string> ExecutarAsync(Tarefa tarefa, CancellationToken ct = default)
    {
        await Task.Delay(tarefa.DuracaoMs, ct);                       // simula E/S: rede, disco, banco…
        if (tarefa.Nome.Contains("falha"))
            throw new InvalidOperationException($"{tarefa.Nome} falhou");
        return $"{tarefa.Nome} concluída";
    }
}

public static class Estilos
{
    /// <summary>Procrastinação: espera cada uma terminar antes de começar a próxima.</summary>
    public static async Task<IReadOnlyList<string>> UmaPorVezAsync(IEnumerable<Tarefa> tarefas)
    {
        var resultados = new List<string>();
        foreach (var t in tarefas)
            resultados.Add(await Trabalho.ExecutarAsync(t));
        return resultados;
    }

    /// <summary>Iniciativa: dá a largada em todas e só então espera o conjunto.</summary>
    public static async Task<IReadOnlyList<string>> TodasJuntasAsync(IEnumerable<Tarefa> tarefas)
    {
        var emAndamento = tarefas.Select(t => Trabalho.ExecutarAsync(t)).ToList();   // inicia todas
        return await Task.WhenAll(emAndamento);                                       // espera todas
    }
}
```

```cs Uso
using System.Diagnostics;

var tarefas = new[] { new Tarefa("analisar", 100), new Tarefa("implementar", 200), new Tarefa("documentar", 200) };

static string Aprox(Stopwatch sw) => $"≈{Math.Round(sw.ElapsedMilliseconds / 100.0) * 100:F0} ms";

var sw = Stopwatch.StartNew();
await Estilos.UmaPorVezAsync(tarefas);
Console.WriteLine($"Uma por vez:   {Aprox(sw)}");

sw.Restart();
await Estilos.TodasJuntasAsync(tarefas);
Console.WriteLine($"Todas juntas:  {Aprox(sw)}");
```

```saida
Uma por vez:   ≈500 ms
Todas juntas:  ≈200 ms
```

::: cuidado Iniciativa ≠ imprudência
Só dá para iniciar em paralelo o que **é independente**. Se a tarefa B precisa do resultado de A, ela tem que esperar — isso não é procrastinação, é dependência. E "paralelo" também tem custo (threads, conexões, limites de serviços externos). Por isso o próximo passo é **liderar** com limites.
:::

## O líder: orquestrar com limites e isolar falhas

Um líder eficaz **não faz tudo sozinho**: distribui o trabalho, **limita a carga** (ninguém trabalha 100 tarefas ao mesmo tempo), **mostra o progresso** e **não deixa uma falha derrubar o time inteiro**. Em C#:

```cs Jornada/Lei05/Lider.cs
using System.Collections.Concurrent;

namespace Jornada.Lei05;

public sealed record ResultadoDaTarefa(string Nome, bool Sucesso, string Detalhe);

public sealed class Lider
{
    private readonly int maxSimultaneas;

    public Lider(int maxSimultaneas)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxSimultaneas);
        this.maxSimultaneas = maxSimultaneas;
    }

    public async Task<IReadOnlyList<ResultadoDaTarefa>> LiderarAsync(
        IEnumerable<Tarefa> tarefas,
        IProgress<string>? progresso = null,
        CancellationToken ct = default)
    {
        var resultados = new ConcurrentBag<ResultadoDaTarefa>();
        var opcoes = new ParallelOptions { MaxDegreeOfParallelism = maxSimultaneas, CancellationToken = ct };

        await Parallel.ForEachAsync(tarefas, opcoes, async (tarefa, token) =>
        {
            try
            {
                var detalhe = await Trabalho.ExecutarAsync(tarefa, token);
                resultados.Add(new ResultadoDaTarefa(tarefa.Nome, true, detalhe));
            }
            catch (OperationCanceledException)
            {
                throw;                                           // cancelamento é decisão do líder: propaga
            }
            catch (Exception e)
            {
                resultados.Add(new ResultadoDaTarefa(tarefa.Nome, false, e.Message));   // falha isolada
            }

            progresso?.Report($"{tarefa.Nome} terminou");
        });

        return resultados.OrderBy(r => r.Nome).ToList();
    }
}

/// <summary>Um IProgress síncrono e simples (a classe Progress do .NET publica no SynchronizationContext, de forma assíncrona).</summary>
public sealed class ColetorDeProgresso : IProgress<string>
{
    private readonly ConcurrentQueue<string> mensagens = new();
    public int Quantidade => mensagens.Count;
    public void Report(string valor) => mensagens.Enqueue(valor);
}
```

```cs Uso
var equipe = new[]
{
    new Tarefa("analisar", 100), new Tarefa("implementar", 200),
    new Tarefa("falha-no-deploy", 150), new Tarefa("documentar", 100),
};

var progresso = new ColetorDeProgresso();
var lider = new Lider(maxSimultaneas: 2);

var resultados = await lider.LiderarAsync(equipe, progresso);

foreach (var r in resultados)
    Console.WriteLine($"{(r.Sucesso ? "✔" : "✘")} {r.Nome,-16} {r.Detalhe}");
Console.WriteLine($"Avisos de progresso recebidos: {progresso.Quantidade}");
```

```saida
✔ analisar         analisar concluída
✔ documentar       documentar concluída
✘ falha-no-deploy  falha-no-deploy falhou
✔ implementar      implementar concluída
Avisos de progresso recebidos: 4
```

Repare nos detalhes de liderança:

1. **`MaxDegreeOfParallelism = 2`**: o líder limita a carga — a mesma ideia de "um de cada vez" que veremos na Lei 12.
2. **Falhas isoladas**: a tarefa `falha-no-deploy` não derruba as outras; vira um resultado `✘` com mensagem (Lei 8).
3. **`OperationCanceledException` propagada**: se o líder cancela, todos param — é uma decisão **de cima**, não um erro de uma tarefa.
4. **`IProgress<string>`**: liderar também é **dar visibilidade** (Lei 7). Em apps com interface gráfica, use `Progress<T>`, que despacha para o *thread* da tela.

::: dica Cancelamento com prazo
Liderança inclui **saber desistir a tempo**. Com `new CancellationTokenSource(TimeSpan.FromSeconds(30))`, tudo que usa o *token* é cancelado automaticamente depois de 30 segundos — uma "meta com prazo" (Lei 1) aplicada ao runtime.
:::

## "Faça uma coisa por dia que ninguém pediu": verificações proativas

A lei sugere um treino: todos os dias, **faça uma coisa útil que ninguém lhe pediu**. Em sistemas, a melhor versão disso é a **verificação proativa**: o próprio programa checa a própria saúde e avisa **antes** de o usuário notar.

```cs Jornada/Lei05/Verificacoes.cs
using Microsoft.Extensions.Hosting;

namespace Jornada.Lei05;

public sealed record ResultadoDaVerificacao(string Nome, bool Saudavel, string Detalhe);

public interface IVerificacao
{
    string Nome { get; }
    Task<ResultadoDaVerificacao> ExecutarAsync(CancellationToken ct);
}

public sealed class ArquivoExiste : IVerificacao
{
    private readonly string caminho;
    public ArquivoExiste(string caminho) => this.caminho = caminho;
    public string Nome => $"arquivo {Path.GetFileName(caminho)}";

    public Task<ResultadoDaVerificacao> ExecutarAsync(CancellationToken ct) =>
        Task.FromResult(new ResultadoDaVerificacao(Nome, File.Exists(caminho),
                        File.Exists(caminho) ? "encontrado" : "NÃO encontrado"));
}

public sealed class FilaDentroDoLimite : IVerificacao
{
    private readonly Func<int> tamanhoAtual;
    private readonly int limite;
    public FilaDentroDoLimite(Func<int> tamanhoAtual, int limite) { this.tamanhoAtual = tamanhoAtual; this.limite = limite; }
    public string Nome => "fila de e-mails";

    public Task<ResultadoDaVerificacao> ExecutarAsync(CancellationToken ct)
    {
        int n = tamanhoAtual();
        return Task.FromResult(new ResultadoDaVerificacao(Nome, n <= limite, $"{n} itens (limite {limite})"));
    }
}

public sealed class VerificadorProativo : BackgroundService
{
    private readonly IReadOnlyList<IVerificacao> verificacoes;
    private readonly TimeSpan intervalo;
    private readonly Action<ResultadoDaVerificacao> aoVerificar;

    public VerificadorProativo(IEnumerable<IVerificacao> verificacoes, TimeSpan intervalo, Action<ResultadoDaVerificacao> aoVerificar)
    {
        this.verificacoes = verificacoes.ToList();
        this.intervalo = intervalo;
        this.aoVerificar = aoVerificar;
    }

    public async Task RodarUmaVezAsync(CancellationToken ct)
    {
        foreach (var v in verificacoes)
            aoVerificar(await v.ExecutarAsync(ct));
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        using var timer = new PeriodicTimer(intervalo);
        try
        {
            do { await RodarUmaVezAsync(ct); }
            while (await timer.WaitForNextTickAsync(ct));         // repete a cada 'intervalo' até o desligamento
        }
        catch (OperationCanceledException) { /* desligamento normal */ }
    }
}
```

```cs Uso
var verificador = new VerificadorProativo(
    new IVerificacao[]
    {
        new ArquivoExiste(Path.Combine(Path.GetTempPath(), "config-que-nao-existe.json")),
        new FilaDentroDoLimite(() => 1250, limite: 1000),
        new FilaDentroDoLimite(() => 3, limite: 1000),
    },
    intervalo: TimeSpan.FromSeconds(30),
    aoVerificar: r => Console.WriteLine($"{(r.Saudavel ? "OK    " : "ALERTA")} {r.Nome}: {r.Detalhe}"));

await verificador.RodarUmaVezAsync(CancellationToken.None);   // em produção: services.AddHostedService(...)
```

```saida
ALERTA arquivo config-que-nao-existe.json: NÃO encontrado
ALERTA fila de e-mails: 1250 itens (limite 1000)
OK     fila de e-mails: 3 itens (limite 1000)
```

Num serviço real, o verificador é registrado com `services.AddHostedService(...)` (ou `AddHostedService<T>()` com dependências injetadas), e o `aoVerificar` envia para o *log*, para o Slack, para o painel de monitoramento. Os frameworks de **health checks** do ASP.NET Core (`AddHealthChecks()`) fazem exatamente isso, de forma padronizada.

::: pense Liderar é servir o propósito
Tanto o `Lider` quanto o `VerificadorProativo` **não conhecem** os detalhes do trabalho: dependem de abstrações (`Tarefa`, `IVerificacao`). É assim que líderes escalam — coordenam sem fazer tudo (DIP, Lei 2). Quem tenta fazer tudo vira gargalo.
:::

::: exercicio Largue a procrastinação
O método abaixo carrega três recursos independentes **em sequência**. Reescreva-o para iniciá-los juntos, com **no máximo 250 ms** de paciência: se estourar o prazo, a operação inteira é cancelada.

```cs
public static async Task<string[]> CarregarTudoAsync()
{
    var a = await Trabalho.ExecutarAsync(new Tarefa("perfil", 100));
    var b = await Trabalho.ExecutarAsync(new Tarefa("pedidos", 200));
    var c = await Trabalho.ExecutarAsync(new Tarefa("avisos", 150));
    return new[] { a, b, c };
}
```

--- resposta
```cs
public static async Task<string[]> CarregarTudoAsync()
{
    using var prazo = new CancellationTokenSource(TimeSpan.FromMilliseconds(250));

    var perfil  = Trabalho.ExecutarAsync(new Tarefa("perfil", 100), prazo.Token);   // sem await: já começa
    var pedidos = Trabalho.ExecutarAsync(new Tarefa("pedidos", 200), prazo.Token);
    var avisos  = Trabalho.ExecutarAsync(new Tarefa("avisos", 150), prazo.Token);

    return await Task.WhenAll(perfil, pedidos, avisos);                              // ≈ 200 ms (a mais lenta)
}
// Se qualquer tarefa passar de 250 ms, o token cancela todas e Task.WhenAll lança TaskCanceledException.
```
:::

::: resumo
- **Lei 5:** sem ação, sonhos e propósitos não valem; **iniciativa** é fazer o necessário sem ser mandado, e dela nasce a **liderança**.
- Roteiro: **questionar → analisar vantagens e obstáculos → simular → agir**; treino diário: uma coisa útil que ninguém pediu.
- Em `async/await`, **procrastinar** é aguardar em fila o que é independente; **iniciativa** é **iniciar tudo** e depois `Task.WhenAll`.
- O **líder** (`Parallel.ForEachAsync`) limita a simultaneidade, isola falhas, dá visibilidade (`IProgress<T>`) e propaga o cancelamento.
- **Verificações proativas** (`BackgroundService`, `PeriodicTimer`, *health checks*) avisam antes de o usuário reclamar.
- Iniciativa tem limites: respeite **dependências** e **recursos**.
:::

::: quiz
? O que é "procrastinação" em código assíncrono, segundo o capítulo?
+ Aguardar em sequência operações independentes que poderiam começar juntas
- Usar Task.Delay
- Lançar exceções
- Usar interfaces
> O correto é iniciar todas e só então esperar o conjunto.

? O que `Task.WhenAll` faz?
+ Espera todas as tarefas indicadas terminarem
- Cancela todas as tarefas
- Executa as tarefas em ordem
- Ignora erros
> Normalmente combinado com tarefas já iniciadas.

? Qual a função de `MaxDegreeOfParallelism` no líder?
+ Limitar quantas tarefas rodam ao mesmo tempo
- Aumentar a prioridade da thread
- Ordenar os resultados
- Desligar o cancelamento
> Liderar é também limitar a carga.

? Por que o líder propaga `OperationCanceledException` em vez de tratá-la como falha da tarefa?
+ Porque cancelar é uma decisão do coordenador, não um erro individual
- Porque exceções são proibidas
- Porque a tarefa não tem catch
- Porque o compilador exige
> Falhas individuais viram resultados; o cancelamento interrompe o conjunto.

? O que é uma verificação proativa?
+ Uma checagem automática da saúde do sistema, que avisa antes de o usuário perceber o problema
- Um teste manual feito pelo cliente
- Um tipo de exceção
- Um log de erros antigo
> É a iniciativa aplicada ao monitoramento.

? Quando NÃO se deve iniciar tarefas em paralelo?
+ Quando uma depende do resultado da outra ou quando os recursos externos não suportam a carga
- Nunca
- Sempre que houver mais de uma
- Quando se usa C# 12
> Iniciativa respeita dependências e limites.
:::

::: cartas
Iniciativa :: Fazer o necessário sem que mandem.
Procrastinação (em async) :: Esperar em fila o que poderia começar junto.
Task.WhenAll :: Aguarda a conclusão de várias tarefas.
Parallel.ForEachAsync :: Processa itens assincronamente, com limite de paralelismo.
IProgress<T> :: Interface para reportar progresso de uma operação.
CancellationToken :: Sinal cooperativo para interromper operações.
BackgroundService :: Classe base para serviços de longa duração em segundo plano.
PeriodicTimer :: Temporizador assíncrono que dispara em intervalos regulares.
:::
