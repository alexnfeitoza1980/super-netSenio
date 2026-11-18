---
id: leis-lei13
order: 74
kind: capitulo
num: 54
group: C# & Mentalidade · Parte 12 · As 16 leis do sucesso em C#
title: Lei 13 — Cooperação
subtitle: Grandes coisas dependem de forças maiores do que nós. Em C#, isso é cooperar com o runtime e o ecossistema: cancelamento cooperativo, encerramento elegante de serviços, pipelines de middleware e gratidão pelas bibliotecas em que você se apoia.
minutes: 55
hue: 190
objectives:
- Reinterpretar a lei da **cooperação** como **trabalhar com** (não contra) o runtime, o framework e o ecossistema
- Usar **`CancellationToken`** como exemplo de **cancelamento cooperativo**
- Fazer **encerramento elegante** (*graceful shutdown*) de um `BackgroundService`
- Montar um **pipeline de middleware**: componentes que cooperam passando a vez
- Aplicar **humildade e gratidão** técnicas: medir, observar e creditar as dependências
origem: livros/03-leis-do-sucesso-csharp/content/73-lei13.md
---

::: vila O deploy que matou o pedido
Numa quarta-feira, o time faz um deploy no meio do expediente. O servidor antigo é desligado **na marra**; três pedidos ficam pela metade. Lia investiga e descobre que o serviço de fila nunca olhava para o sinal de desligamento: ele **não cooperava** com quem o gerenciava. A correção levou vinte linhas — e uma lição: sistemas existem dentro de **forças maiores** (o sistema operacional, o orquestrador, o *runtime*), e vivem melhor quando **cooperam** com elas.
:::

::: ilustracao img/leis/lei13.svg | Cooperação: passar o bastão e parar com elegância quando pedem (cancelamento cooperativo).
:::

## A lei, em uma frase (e uma nota honesta)

> A lição original afirma que o Universo é regido por uma "Inteligência Infinita" e que grandes realizações só são possíveis quando se **coopera com essas forças superiores**: aceitar que existe algo que escapa à razão, abrir a mente para o incognoscível, **agir com gratidão** e perceber que nossos pensamentos e propósitos interagem com um todo maior.

::: cuidado Metáfora, sim; mágica, não
Neste livro **não** afirmamos nenhuma causa metafísica. Tomamos da lição o que é **útil e verificável para quem programa**: *(1)* **humildade** — nossos programas vivem dentro de sistemas complexos que não controlamos por completo (o SO, a rede, o *garbage collector*, o orquestrador, as bibliotecas); *(2)* **cooperação** — projetar para **trabalhar com** esses sistemas, não contra eles; *(3)* **gratidão** — reconhecer o que outras pessoas construíram e em que nos apoiamos; *(4)* **observação** — como não dá para prever tudo, **medir** em vez de presumir.
:::

## Da vida para o código

| O que a lição diz | O que isso significa para quem programa |
|---|---|
| Forças maiores que nós | **Runtime, SO, rede, GC, orquestrador** — ambientes que você não controla |
| Cooperar com elas | **Cancelamento cooperativo**, *graceful shutdown*, uso das convenções do framework |
| Abrir a mente ao incognoscível | **Humildade**: sistemas complexos têm comportamentos que você não prevê → **observar e medir** |
| Gratidão | **Dar crédito** às bibliotecas e licenças em que você se apoia |
| Cada mente soma ao todo | **Pipelines** de componentes que passam a vez uns aos outros |
| Alinhar-se ao propósito comum | Seguir **contratos** e **convenções** (interfaces, ciclo de vida, DI) |

::: figura Sua aplicação dentro de forças maiores
<svg viewBox="0 0 740 230" role="img" aria-label="Aplicação dentro do runtime, dentro do sistema operacional, dentro do orquestrador; setas de sinal de desligamento descendo">
  <rect class="f-plain" x="10" y="10" width="720" height="210" rx="16"/><text x="30" y="34" font-weight="800" class="t-dim">Orquestrador / nuvem (Kubernetes, App Service…)</text>
  <rect class="f-info" x="40" y="48" width="660" height="160" rx="14"/><text x="60" y="72" font-weight="800" class="t-info">Sistema operacional + runtime .NET (GC, threads, I/O)</text>
  <rect class="f-prim" x="80" y="88" width="580" height="104" rx="12"/><text x="100" y="112" font-weight="800">Seu serviço</text>
  <rect class="f-good" x="110" y="128" width="170" height="44" rx="8"/><text x="195" y="155" text-anchor="middle" font-size="13">CancellationToken</text>
  <rect class="f-good" x="300" y="128" width="170" height="44" rx="8"/><text x="385" y="155" text-anchor="middle" font-size="13">BackgroundService</text>
  <rect class="f-good" x="490" y="128" width="150" height="44" rx="8"/><text x="565" y="155" text-anchor="middle" font-size="13">Middleware</text>
  <text x="660" y="40" text-anchor="end" font-size="12" class="t-warn">⚡ "vou desligar você" (SIGTERM)</text>
</svg>
:::

## 1) Cancelamento cooperativo

No .NET, **ninguém mata uma tarefa à força** (o antigo `Thread.Abort` não existe mais nas versões modernas — e era perigoso). O que existe é um **pedido de cancelamento**: o `CancellationToken`. Quem faz o trabalho **coopera**, verificando o sinal em pontos seguros e parando de forma organizada.

```cs Jornada/Lei13/ProcessadorDeLote.cs
namespace Jornada.Lei13;

public sealed record ResultadoDoLote(int Processados, int Total, bool Cancelado);

public static class ProcessadorDeLote
{
    public static async Task<ResultadoDoLote> ProcessarAsync(
        IReadOnlyList<string> itens,
        Action<string> processar,
        CancellationToken ct = default)
    {
        int feitos = 0;

        foreach (var item in itens)
        {
            // Coopera: em um ponto seguro (entre itens), olha se pediram para parar.
            if (ct.IsCancellationRequested)
                return new ResultadoDoLote(feitos, itens.Count, Cancelado: true);

            processar(item);
            feitos++;
            await Task.Yield();
        }

        return new ResultadoDoLote(feitos, itens.Count, Cancelado: false);
    }
}
```

```cs Uso
var itens = Enumerable.Range(1, 10).Select(n => $"pedido {n}").ToList();
using var cts = new CancellationTokenSource();
int vistos = 0;

var resultado = await ProcessadorDeLote.ProcessarAsync(
    itens,
    processar: item =>
    {
        Console.WriteLine($"processando {item}");
        if (++vistos == 3) cts.Cancel();          // alguém (o orquestrador, o usuário…) pede para parar
    },
    ct: cts.Token);

Console.WriteLine($"Processou {resultado.Processados} de {resultado.Total}; cancelado = {resultado.Cancelado}");
```

```saida
processando pedido 1
processando pedido 2
processando pedido 3
Processou 3 de 10; cancelado = True
```

Observe: o cancelamento aconteceu **depois** do item 3, porque a verificação está **entre** os itens — nenhum pedido ficou pela metade. Esse é o ponto da cooperação: **o programa escolhe onde é seguro parar**. (Se cancelar for decisão de cima — como na Lei 5 —, também é comum lançar `OperationCanceledException` com `ct.ThrowIfCancellationRequested()`; escolha conforme o contrato.)

::: dica Passe o token adiante
Toda API assíncrona que você chamar e que aceite `CancellationToken` (`HttpClient`, `Task.Delay`, `DbContext`, `Stream.ReadAsync`…) deve **receber o seu**. Um token que não é repassado é um sinal que se perde no meio do caminho. Quando criar métodos `async` públicos, ofereça `CancellationToken ct = default` por último.
:::

## 2) Encerramento elegante de um serviço

Serviços de longa duração recebem sinais de desligamento. Com o *Generic Host* e o `BackgroundService`, o `stoppingToken` é essa conexão com o ambiente: quando você faz *deploy* ou o contêiner é parado, **ele é cancelado**. Seu serviço deve **terminar o que está fazendo e salvar o estado**:

```cs Jornada/Lei13/ServicoDeFila.cs
using Microsoft.Extensions.Hosting;

namespace Jornada.Lei13;

public sealed class ServicoDeFila : BackgroundService
{
    private readonly Action<string> registrar;

    public ServicoDeFila(Action<string> registrar) => this.registrar = registrar;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        registrar("serviço iniciado");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
                await Task.Delay(50, stoppingToken);          // aqui entraria: ler a fila e processar uma mensagem
        }
        catch (OperationCanceledException)
        {
            // pedido de desligamento: esperado, não é erro
        }

        registrar("terminando o item atual e salvando o estado…");
        await Task.Delay(10);                                  // conclui o que estava em andamento
        registrar("serviço encerrado com elegância");
    }
}
```

```cs Uso
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder();
builder.Logging.ClearProviders();                              // sem o ruído padrão do host
builder.Services.AddHostedService(_ => new ServicoDeFila(Console.WriteLine));

using var host = builder.Build();

await host.StartAsync();
await Task.Delay(120);                                         // o serviço "trabalha" um pouco
await host.StopAsync();                                        // desligamento: stoppingToken é cancelado
```

```saida
serviço iniciado
terminando o item atual e salvando o estado…
serviço encerrado com elegância
```

Em produção, o orquestrador envia `SIGTERM`, o host converte isso em `StopAsync` e dá um prazo (`HostOptions.ShutdownTimeout`, 30 segundos por padrão). **Cooperar** é aproveitar esse prazo; **não cooperar** é ser encerrado à força no meio de um pedido.

## 3) Componentes que passam a vez: um pipeline de middleware

O ASP.NET Core, o `HttpClient` (com `DelegatingHandler`) e muitos frameworks se baseiam em **cadeias de componentes cooperantes**: cada etapa faz sua parte e **decide** se chama a próxima. Veja o princípio em miniatura:

```cs Jornada/Lei13/Pipeline.cs
namespace Jornada.Lei13;

public delegate Task Proximo();

public sealed class PipelineDeRequisicao
{
    private readonly List<Func<string, Proximo, Task>> etapas = new();

    public PipelineDeRequisicao Usar(Func<string, Proximo, Task> etapa)
    {
        etapas.Add(etapa);
        return this;
    }

    public Task ExecutarAsync(string requisicao)
    {
        int indice = -1;
        Proximo proximo = null!;
        proximo = () =>
        {
            indice++;
            return indice < etapas.Count ? etapas[indice](requisicao, proximo) : Task.CompletedTask;
        };
        return proximo();
    }
}
```

```cs Uso
var pipeline = new PipelineDeRequisicao()
    .Usar(async (req, proximo) =>                               // 1ª etapa: registro (antes e depois)
    {
        Console.WriteLine($"→ recebeu {req}");
        await proximo();
        Console.WriteLine($"← respondeu {req}");
    })
    .Usar(async (req, proximo) =>                               // 2ª etapa: autorização (pode interromper)
    {
        if (req.StartsWith("/admin")) { Console.WriteLine("✘ acesso negado"); return; }
        await proximo();
    })
    .Usar((req, proximo) =>                                     // 3ª etapa: o trabalho em si
    {
        Console.WriteLine($"  processando {req}");
        return Task.CompletedTask;
    });

await pipeline.ExecutarAsync("/home");
await pipeline.ExecutarAsync("/admin/usuarios");
```

```saida
→ recebeu /home
  processando /home
← respondeu /home
→ recebeu /admin/usuarios
✘ acesso negado
← respondeu /admin/usuarios
```

Cada etapa **conhece só o contrato** (`Proximo`), não as outras etapas. Elas cooperam sem se conhecerem — a **Mente Mestra** da Lei 2 em ação, agora na forma de *middleware*. No ASP.NET Core é exatamente assim: `app.UseAuthentication()`, `app.UseAuthorization()`, `app.UseEndpoints(...)` são etapas de um pipeline.

## 4) Humildade e observação

A lição pede para **abrir a mente ao que a razão não alcança**. Em engenharia, a versão sensata disso é: **sistemas complexos surpreendem**. Você não consegue prever a latência da rede, o comportamento do *garbage collector* sob carga, ou como duas bibliotecas interagem num cenário raro. Atitude cooperativa:

- **Não lute contra o GC**: evite `GC.Collect()` "por garantia"; implemente `IDisposable` e deixe o coletor trabalhar.
- **Meça em vez de presumir**: *profilers*, contadores, `Stopwatch`, **BenchmarkDotNet**, **OpenTelemetry** (`System.Diagnostics.Activity`) — o sistema "fala", se você ouvir (Lei 14).
- **Use as convenções do framework** (DI, `appsettings.json`, `ILogger`): você se alinha ao que a comunidade já entende.
- **Dê *timeouts* a tudo** que cruza a rede (Lei 8): você não controla o outro lado.

## 5) Gratidão técnica: dê crédito ao que sustenta o seu código

Quase tudo que você roda tem **autores**: o próprio .NET, bibliotecas de código aberto, ferramentas. A atitude de gratidão da lição se traduz em **práticas concretas**:

```bash
# Quais pacotes (diretos e transitivos) o projeto usa?
dotnet list package --include-transitive

# Há versões desatualizadas ou vulneráveis?
dotnet list package --outdated
dotnet list package --vulnerable
```

- Mantenha um arquivo **`THIRD-PARTY-NOTICES`** com os pacotes, licenças e links.
- **Respeite as licenças** (MIT, Apache-2.0, GPL… têm exigências diferentes).
- **Contribua de volta**: relate um *bug* com um exemplo mínimo, corrija um erro de documentação, patrocine mantenedores. Código aberto é uma cooperação em larga escala.

::: pense Cooperação com pessoas também
A lição conecta "cooperar com forças maiores" à **Mente Mestra**: a união de mentes em harmonia amplia o que cada uma consegue. Os dois lados se complementam — **cooperar com o sistema** (runtime, framework) e **cooperar com as pessoas** (equipe, comunidade). Programas e times que fazem as duas coisas **escalam** melhor.
:::

::: exercicio Cooperando melhor
Reescreva `Importador.ImportarAsync` (Lei 7) para ser **cooperativo sem lançar exceção**: quando o token for cancelado, ele deve **parar entre dois registros** e devolver quantos conseguiu importar. Qual o ganho de devolver o resultado parcial em vez de lançar `OperationCanceledException`?

--- resposta
```cs Importador.cs (trecho)
public static async Task<int> ImportarAsync(IReadOnlyList<string> registros, CancellationToken ct = default)
{
    int importados = 0;

    foreach (var registro in registros)
    {
        if (ct.IsCancellationRequested) break;        // coopera: para num ponto seguro

        await Task.Delay(10);                          // grava o registro (sem repassar 'ct' para não abortar no meio dele)
        importados++;
    }

    return importados;                                 // o chamador sabe de onde retomar: registros[importados..]
}
```
**Ganho:** o chamador recebe um **resultado parcial utilizável** (pode salvar o ponto de retomada e continuar depois). Lançar exceção faria o chamador descobrir o progresso por outros meios. A escolha depende do contrato: *"cancelar = falha"* (exceção) ou *"cancelar = parada ordenada"* (resultado parcial). Documente a que você escolheu (Lei 10).
:::

::: resumo
- **Lei 13 (traduzida):** grandes realizações dependem de forças maiores do que nós; aqui, são o **runtime, o SO, a rede e o ecossistema** — e a atitude certa é **cooperar**, com **humildade** e **gratidão**.
- **Cancelamento cooperativo:** `CancellationToken` é um pedido; o código escolhe o ponto seguro de parar e repassa o token adiante.
- **Encerramento elegante:** `BackgroundService` + `stoppingToken` + `ShutdownTimeout`: termine o item atual e salve o estado.
- **Pipeline de middleware:** etapas que conhecem só o contrato e passam a vez (`next`), como no ASP.NET Core.
- **Humildade técnica:** medir e observar; não lutar contra o GC; usar convenções; *timeouts* em tudo que é externo.
- **Gratidão:** conhecer e creditar dependências (`dotnet list package`), respeitar licenças e contribuir.
:::

::: quiz
? Por que o cancelamento no .NET é chamado de "cooperativo"?
+ Porque o código verifica o sinal e escolhe parar em um ponto seguro, em vez de ser encerrado à força
- Porque exige cooperação de duas threads
- Porque depende de várias equipes
- Porque só funciona em rede
> Não existe "matar" a tarefa; existe um pedido que ela honra.

? O que é o `stoppingToken` de um `BackgroundService`?
+ O token cancelado quando o host está sendo desligado
- Um token de autenticação
- Um tempo limite de requisição
- Um token de acesso a API
> É a conexão do seu serviço com o ciclo de vida do host.

? O que caracteriza um pipeline de middleware?
+ Etapas independentes que conhecem apenas o contrato da próxima e decidem se a chamam
- Uma única função gigante
- Herança múltipla
- Código gerado automaticamente
> É a Mente Mestra aplicada a requisições.

? Qual atitude "humilde" a lição sugere para sistemas complexos?
+ Medir e observar, em vez de presumir comportamentos que não se pode prever
- Ignorar métricas
- Chamar GC.Collect() com frequência
- Evitar timeouts
> A realidade surpreende; os dados orientam.

? Como traduzir "gratidão" em prática de engenharia?
+ Conhecer e creditar as dependências, respeitar licenças e contribuir de volta
- Remover os créditos para simplificar
- Copiar código sem licença
- Evitar atualizar pacotes
> Todo software se apoia em trabalho alheio.

? O que acontece se um serviço ignora o sinal de desligamento?
+ Pode ser encerrado à força, deixando trabalho pela metade
- Nada, ele continua para sempre
- O host espera infinitamente
- O compilador avisa
> Depois do ShutdownTimeout, o processo é finalizado.
:::

::: cartas
Cooperação (traduzida) :: Trabalhar com o runtime, o framework e as pessoas, em vez de contra eles.
CancellationToken :: Sinal cooperativo de pedido de cancelamento.
Graceful shutdown :: Encerramento em que o serviço termina o que faz e salva o estado.
BackgroundService :: Classe base do Generic Host para trabalho em segundo plano.
Middleware :: Etapa de um pipeline que pode agir antes/depois e chamar a próxima.
ShutdownTimeout :: Prazo que o host dá aos serviços para encerrarem.
Humildade técnica :: Aceitar que sistemas complexos surpreendem e basear decisões em medições.
THIRD-PARTY-NOTICES :: Arquivo que lista dependências de terceiros e suas licenças.
:::
