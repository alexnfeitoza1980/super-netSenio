---
id: leis-lei12
order: 73
kind: capitulo
num: 53
group: C# & Mentalidade · Parte 12 · As 16 leis do sucesso em C#
title: Lei 12 — Concentração
subtitle: Uma coisa de cada vez, bem feita. Limite de trabalho em andamento (WIP), Channel<T> limitado com contrapressão, ciclos de foco (Pomodoro) e o SRP como foco no próprio código.
minutes: 50
hue: 245
objectives:
- Entender a lei da **concentração**: focar a atenção em um único ponto até realizar
- Aplicar um **limite de trabalho em andamento (WIP)** e entender por que "terminar antes de começar" funciona
- Usar um **`Channel<T>` limitado** para dar **contrapressão** (não aceitar mais do que se consegue processar)
- Modelar ciclos de **foco e pausa** (Pomodoro) como função pura e testável
- Reconhecer o **SRP** como concentração aplicada ao código
origem: livros/03-leis-do-sucesso-csharp/content/72-lei12.md
---

::: vila Doze abas, zero entregas
Na quinta-feira, Lia tem doze tarefas "em andamento" no quadro. Cada vez que uma trava, ela pula para outra. No fim do dia, nenhuma foi concluída — mas **todas** consumiram atenção. O líder propõe uma regra radical: **no máximo duas tarefas em andamento**; para começar uma nova, termine uma. Em duas semanas, o número de entregas dobra. Nada mudou na capacidade do time; mudou o **foco**.
:::

::: ilustracao img/leis/lei12.svg | Concentração: limitar o trabalho em andamento deixa o fluxo calmo e as entregas firmes.
:::

## A lei, em uma frase

> Para criar a realidade que se deseja, não basta pensar nela: é preciso **concentrar a atenção** num único propósito, **pelo tempo necessário**, até que os meios de realizá-lo sejam encontrados e empregados. Desejo vago e entusiasmo momentâneo **não valem** — e o ambiente (livros, pessoas, estímulos) influencia a qualidade do foco.

A lição acrescenta ideias úteis: **"precisamos ser antes de poder"** (só se faz o que se tem capacidade de fazer — logo, desenvolva-se); a **percepção de águia** (ver o panorama, mas fixar o ponto **crucial**); e **semear de propósito**: escolher que ideias plantar, em vez de deixar qualquer pensamento crescer. Também orienta a **não esperar** ter todos os meios antes de agir: *comece com o que está ao alcance*.

## Da vida para o código

| O que a lei diz | O que isso significa para quem programa |
|---|---|
| Um único ponto de atenção | **Uma tarefa por vez** — limite de trabalho em andamento (WIP) |
| Concentração prolongada | **Blocos de foco** (Pomodoro) sem interrupções |
| Visão de águia: o ponto crucial | Otimizar o **caminho crítico** (medir antes) e o **requisito essencial** |
| Semear com cuidado | **Backlog curado**: poucas tarefas bem escolhidas |
| O ambiente influencia | Ambiente de desenvolvimento limpo (menos ruído, notificações desligadas) |
| Foco de cada coisa | **SRP**: cada classe e método faz **uma** coisa |
| Não aceitar mais do que se processa | **Contrapressão** (*backpressure*) em filas e *pipelines* |

::: figura Quadro com limite de WIP
<svg viewBox="0 0 700 220" role="img" aria-label="Três colunas: A fazer, Em andamento (limite 2) e Pronto">
  <rect class="f-plain" x="10" y="30" width="200" height="170" rx="10"/><text x="110" y="22" text-anchor="middle" font-weight="800">A fazer</text>
  <rect class="f-prim" x="25" y="50" width="170" height="28" rx="6"/><rect class="f-prim" x="25" y="86" width="170" height="28" rx="6"/><rect class="f-prim" x="25" y="122" width="170" height="28" rx="6"/><rect class="f-prim" x="25" y="158" width="170" height="28" rx="6"/>
  <rect class="f-warn" x="250" y="30" width="200" height="170" rx="10"/><text x="350" y="22" text-anchor="middle" font-weight="800" class="t-warn">Em andamento · limite 2</text>
  <rect class="f-good" x="265" y="50" width="170" height="28" rx="6"/><rect class="f-good" x="265" y="86" width="170" height="28" rx="6"/>
  <rect x="265" y="122" width="170" height="28" rx="6" fill="none" stroke="var(--bad)" stroke-dasharray="5 4"/><text x="350" y="141" text-anchor="middle" font-size="11" class="t-bad">cheio: termine uma antes</text>
  <rect class="f-plain" x="490" y="30" width="200" height="170" rx="10"/><text x="590" y="22" text-anchor="middle" font-weight="800">Pronto</text>
  <rect class="f-good" x="505" y="50" width="170" height="28" rx="6"/><rect class="f-good" x="505" y="86" width="170" height="28" rx="6"/><rect class="f-good" x="505" y="122" width="170" height="28" rx="6"/>
  <path class="arrow" d="M212 100H248"/><path class="arrow" d="M452 100H488"/>
</svg>
:::

## 1) Limite de trabalho em andamento (WIP)

Por que limitar o WIP funciona? Porque **trocar de contexto é caro** e, quanto mais coisas abertas, **mais tempo cada uma fica esperando** para terminar. A regra é simples: **termine antes de começar**. Dá para colocar a regra **no código** de um quadro:

```cs Jornada/Lei12/QuadroKanban.cs
namespace Jornada.Lei12;

public enum Coluna { AFazer, EmAndamento, Pronto }

public sealed class LimiteDeTrabalhoException : InvalidOperationException
{
    public LimiteDeTrabalhoException(int limite)
        : base($"Limite de {limite} tarefa(s) em andamento atingido: termine uma antes de começar outra.") { }
}

public sealed class QuadroKanban
{
    private readonly Dictionary<string, Coluna> tarefas = new();
    private readonly int limiteEmAndamento;

    public QuadroKanban(int limiteEmAndamento)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(limiteEmAndamento);
        this.limiteEmAndamento = limiteEmAndamento;
    }

    public void Adicionar(string tarefa) => tarefas.Add(tarefa, Coluna.AFazer);

    public void Mover(string tarefa, Coluna destino)
    {
        if (!tarefas.TryGetValue(tarefa, out var atual))
            throw new KeyNotFoundException($"A tarefa '{tarefa}' não está no quadro.");

        if (destino == Coluna.EmAndamento && atual != Coluna.EmAndamento && Contar(Coluna.EmAndamento) >= limiteEmAndamento)
            throw new LimiteDeTrabalhoException(limiteEmAndamento);

        tarefas[tarefa] = destino;
    }

    public int Contar(Coluna coluna) => tarefas.Values.Count(c => c == coluna);

    public IEnumerable<string> Em(Coluna coluna) => tarefas.Where(p => p.Value == coluna).Select(p => p.Key);
}
```

```cs Uso
var quadro = new QuadroKanban(limiteEmAndamento: 1);
foreach (var t in new[] { "login", "relatório", "e-mail" }) quadro.Adicionar(t);

quadro.Mover("login", Coluna.EmAndamento);
Console.WriteLine("Em andamento: " + string.Join(", ", quadro.Em(Coluna.EmAndamento)));

try { quadro.Mover("relatório", Coluna.EmAndamento); }
catch (LimiteDeTrabalhoException e) { Console.WriteLine("Tentou começar 'relatório': " + e.Message); }

quadro.Mover("login", Coluna.Pronto);          // termina...
quadro.Mover("relatório", Coluna.EmAndamento); // ...e só então começa a próxima

Console.WriteLine($"Pronto: {string.Join(", ", quadro.Em(Coluna.Pronto))} · Em andamento: {string.Join(", ", quadro.Em(Coluna.EmAndamento))}");
```

```saida
Em andamento: login
Tentou começar 'relatório': Limite de 1 tarefa(s) em andamento atingido: termine uma antes de começar outra.
Pronto: login · Em andamento: relatório
```

```cs JornadaTests/Lei12/QuadroKanbanTests.cs
using Jornada.Lei12;
using Xunit;

public class QuadroKanbanTests
{
    [Fact]
    public void Nao_permite_passar_do_limite()
    {
        var quadro = new QuadroKanban(2);
        foreach (var t in new[] { "a", "b", "c" }) quadro.Adicionar(t);
        quadro.Mover("a", Coluna.EmAndamento);
        quadro.Mover("b", Coluna.EmAndamento);

        Assert.Throws<LimiteDeTrabalhoException>(() => quadro.Mover("c", Coluna.EmAndamento));
        Assert.Equal(2, quadro.Contar(Coluna.EmAndamento));
    }

    [Fact]
    public void Concluir_libera_espaco()
    {
        var quadro = new QuadroKanban(1);
        quadro.Adicionar("a"); quadro.Adicionar("b");
        quadro.Mover("a", Coluna.EmAndamento);
        quadro.Mover("a", Coluna.Pronto);

        quadro.Mover("b", Coluna.EmAndamento);          // agora cabe
        Assert.Equal(1, quadro.Contar(Coluna.EmAndamento));
    }
}
```

::: pense O limite é uma decisão de foco, não de capacidade
Um limite de WIP parece "desperdiçar" capacidade — mas o que se **perde** são as trocas de contexto e as tarefas penduradas. Ao limitar, você **expõe os gargalos** (se todos esperam por revisão, o problema aparece) e reduz o **tempo de ciclo** (da ideia à entrega). É a mesma lógica de **concentrar toda a energia em um ponto**.
:::

## 2) Contrapressão: não aceitar mais do que se consegue processar

A mesma ideia vale para **programas**. Um produtor que gera trabalho mais rápido do que o consumidor processa vai **encher a memória** (e travar). O `Channel<T>` do .NET resolve com um canal **limitado**: quando a fila enche, o produtor **espera** — é a concentração do sistema:

```cs Jornada/Lei12/LinhaDeProducao.cs
using System.Threading.Channels;

namespace Jornada.Lei12;

public sealed record Pedido(int Numero);

public static class LinhaDeProducao
{
    public static async Task<(int Processados, int MaiorFila)> RodarAsync(int total, int capacidade)
    {
        var canal = Channel.CreateBounded<Pedido>(new BoundedChannelOptions(capacidade)
        {
            FullMode = BoundedChannelFullMode.Wait,        // cheio? o produtor espera (contrapressão)
            SingleReader = true,
            SingleWriter = true
        });

        int maiorFila = 0;

        var produtor = Task.Run(async () =>
        {
            for (int i = 1; i <= total; i++)
            {
                await canal.Writer.WriteAsync(new Pedido(i));
                maiorFila = Math.Max(maiorFila, canal.Reader.Count);
            }
            canal.Writer.Complete();
        });

        int processados = 0;
        await foreach (var pedido in canal.Reader.ReadAllAsync())
        {
            await Task.Delay(5);                           // trabalho lento do consumidor
            processados++;
        }

        await produtor;
        return (processados, maiorFila);
    }
}
```

```cs Uso
var (processados, maiorFila) = await LinhaDeProducao.RodarAsync(total: 20, capacidade: 2);
Console.WriteLine($"Processados: {processados} · maior fila observada: {maiorFila} (capacidade 2)");
```

```saida
Processados: 20 · maior fila observada: 2 (capacidade 2)
```

Sem o limite, o produtor enfileiraria os 20 (ou 20 milhões) de pedidos imediatamente. Com ele, o sistema **só trabalha no ritmo em que consegue concluir** — outra forma de "uma coisa de cada vez".

## 3) Blocos de foco: o Pomodoro como função pura

A concentração prolongada precisa de **pausas** para se sustentar. A técnica Pomodoro alterna **25 min de foco** e **5 min de pausa**; a cada quatro ciclos, uma **pausa longa** de 15 min. Em vez de depender de relógios e *timers*, modelamos a técnica como uma **função pura** — fácil de testar:

```cs Jornada/Lei12/Pomodoro.cs
namespace Jornada.Lei12;

public enum Fase { Foco, PausaCurta, PausaLonga }

public static class Pomodoro
{
    private static readonly (Fase Fase, int Minutos)[] Ciclo =
    {
        (Fase.Foco, 25), (Fase.PausaCurta, 5),
        (Fase.Foco, 25), (Fase.PausaCurta, 5),
        (Fase.Foco, 25), (Fase.PausaCurta, 5),
        (Fase.Foco, 25), (Fase.PausaLonga, 15),
    };

    public static readonly TimeSpan DuracaoDoCiclo = TimeSpan.FromMinutes(Ciclo.Sum(c => c.Minutos));

    /// <summary>Em que fase você está, depois de 'decorrido' desde o início? E quanto falta nela?</summary>
    public static (Fase Fase, TimeSpan Restante) Em(TimeSpan decorrido)
    {
        if (decorrido < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(decorrido));

        var t = TimeSpan.FromTicks(decorrido.Ticks % DuracaoDoCiclo.Ticks);     // o ciclo se repete
        foreach (var (fase, minutos) in Ciclo)
        {
            var duracao = TimeSpan.FromMinutes(minutos);
            if (t < duracao) return (fase, duracao - t);
            t -= duracao;
        }
        throw new InvalidOperationException("Inalcançável.");
    }
}
```

```cs Uso
foreach (int minutos in new[] { 0, 10, 26, 115, 130 })
{
    var (fase, restante) = Pomodoro.Em(TimeSpan.FromMinutes(minutos));
    Console.WriteLine($"{minutos,3} min → {fase,-10} (faltam {(int)restante.TotalMinutes} min)");
}
```

```saida
  0 min → Foco       (faltam 25 min)
 10 min → Foco       (faltam 15 min)
 26 min → PausaCurta (faltam 4 min)
115 min → PausaLonga (faltam 15 min)
130 min → Foco       (faltam 25 min)
```

```cs JornadaTests/Lei12/PomodoroTests.cs
using Jornada.Lei12;
using Xunit;

public class PomodoroTests
{
    [Theory]
    [InlineData(0,   Fase.Foco,       25)]
    [InlineData(25,  Fase.PausaCurta,  5)]
    [InlineData(115, Fase.PausaLonga, 15)]
    [InlineData(130, Fase.Foco,       25)]   // o ciclo recomeça
    public void Fase_e_tempo_restante(int minutos, Fase fase, int restante)
    {
        var r = Pomodoro.Em(TimeSpan.FromMinutes(minutos));
        Assert.Equal(fase, r.Fase);
        Assert.Equal(restante, (int)r.Restante.TotalMinutes);
    }
}
```

::: dica Separar a regra do relógio
Nossa função recebe **o tempo decorrido** como parâmetro. Um aplicativo real usaria `TimeProvider` (.NET 8+) para obter a hora e um `PeriodicTimer` para atualizar a tela; **a regra** continua pura e testada. É o mesmo truque da Lei 1 (injetar o relógio).
:::

## 4) O SRP é a concentração aplicada ao código

Uma classe com várias responsabilidades "pensa em muitas coisas ao mesmo tempo" — e por isso muda por muitos motivos. Concentrar é **dar um único assunto** a cada tipo e a cada método:

```cs FecharPedido.cs (trecho)
// Disperso: valida, calcula, grava e envia e-mail no mesmo método.
public void FecharPedido(Pedido p) { /* 120 linhas */ }

// Concentrado: o método de nível alto lê como um índice do que acontece; cada passo tem seu dono.
public void FecharPedido(Pedido p)
{
    validador.Validar(p);
    var total = calculadora.Calcular(p);
    repositorio.Salvar(p, total);
    notificador.ConfirmarPorEmail(p);
}
```

(Veja o cap. 20 para a explicação completa do **SRP**.) E lembre-se da "visão de águia": use o *profiler* para achar **o ponto crucial** (o método que consome 80% do tempo) e concentre aí a otimização — não em 100 micro-melhorias dispersas (Lei 4).

::: cuidado Foco não é tunelamento
Concentrar-se **não** significa ignorar sinais importantes. A lei diz para manter o "contexto maior" enquanto lida com o que está à mão. Em equipes, isso quer dizer: foco individual **e** comunicação (Lei 2 e 13). E um limite de WIP deve ter **exceções claras** (um incidente em produção interrompe o fluxo — e isso deve ser explícito, não "furar a fila" sem avisar).
:::

::: exercicio Limite por pessoa
Estenda o `QuadroKanban` para que cada tarefa tenha um **responsável** e o limite valha **por pessoa** (por exemplo, 2 em andamento por pessoa). Escreva a assinatura nova de `Mover` e um teste que mostre que **Ana** pode ter 2 tarefas em andamento enquanto **Bia** também pode ter 2 — mas Ana não pode ter a terceira.

--- resposta
```cs
public sealed class QuadroPorPessoa
{
    private readonly Dictionary<string, (string Responsavel, Coluna Coluna)> tarefas = new();
    private readonly int limitePorPessoa;

    public QuadroPorPessoa(int limitePorPessoa) => this.limitePorPessoa = limitePorPessoa;

    public void Adicionar(string tarefa, string responsavel) =>
        tarefas.Add(tarefa, (responsavel, Coluna.AFazer));

    public void Mover(string tarefa, Coluna destino)
    {
        var (responsavel, atual) = tarefas[tarefa];

        if (destino == Coluna.EmAndamento && atual != Coluna.EmAndamento)
        {
            int emAndamento = tarefas.Values.Count(t => t.Responsavel == responsavel && t.Coluna == Coluna.EmAndamento);
            if (emAndamento >= limitePorPessoa) throw new LimiteDeTrabalhoException(limitePorPessoa);
        }
        tarefas[tarefa] = (responsavel, destino);
    }
}

public class QuadroPorPessoaTests
{
    [Fact]
    public void Limite_vale_para_cada_pessoa()
    {
        var q = new QuadroPorPessoa(2);
        q.Adicionar("a1", "Ana"); q.Adicionar("a2", "Ana"); q.Adicionar("a3", "Ana");
        q.Adicionar("b1", "Bia"); q.Adicionar("b2", "Bia");

        q.Mover("a1", Coluna.EmAndamento); q.Mover("a2", Coluna.EmAndamento);
        q.Mover("b1", Coluna.EmAndamento); q.Mover("b2", Coluna.EmAndamento);   // Bia tem o dela

        Assert.Throws<LimiteDeTrabalhoException>(() => q.Mover("a3", Coluna.EmAndamento));
    }
}
```
:::

::: resumo
- **Lei 12:** concentre a atenção num único propósito pelo tempo necessário; desejo vago e entusiasmo momentâneo não bastam; comece com o que está ao alcance.
- **WIP limitado:** *termine antes de começar* reduz trocas de contexto, expõe gargalos e encurta o tempo de ciclo.
- **`Channel<T>` limitado** (contrapressão): o produtor espera quando a fila enche — o sistema só aceita o que consegue processar.
- **Pomodoro** como função pura: foco e pausas, testáveis; separe a **regra** do **relógio** (`TimeProvider`).
- **SRP** é concentração no código; use o *profiler* para focar no **ponto crucial**.
- Foco **sem tunelamento**: mantenha o contexto e a comunicação; exceções ao limite devem ser explícitas.
:::

::: quiz
? Por que limitar o trabalho em andamento (WIP) melhora a entrega?
+ Reduz trocas de contexto, expõe gargalos e diminui o tempo de ciclo
- Aumenta o número de tarefas abertas
- Elimina a necessidade de planejar
- Faz o time trabalhar mais horas
> "Termine antes de começar."

? O que faz um Channel limitado quando está cheio e o modo é Wait?
+ O produtor espera até haver espaço (contrapressão)
- Descarta os itens para sempre
- Lança uma exceção sempre
- Cria um canal novo
> O sistema só trabalha no ritmo em que consegue concluir.

? Por que modelar o Pomodoro como função pura?
+ Para separar a regra do relógio e poder testá-la facilmente
- Para deixá-lo mais lento
- Porque funções puras não aceitam parâmetros
- Para evitar testes
> A função recebe o tempo decorrido e devolve a fase.

? Qual princípio SOLID corresponde à concentração aplicada ao código?
+ SRP (Responsabilidade Única)
- LSP
- OCP
- ISP
> Cada tipo/método com um único assunto.

? O que é a "visão de águia" da lição, em termos de otimização?
+ Ver o panorama e fixar a atenção no ponto crucial (medido pelo profiler)
- Otimizar tudo ao mesmo tempo
- Evitar medições
- Reescrever o sistema inteiro
> Foco no que realmente pesa.

? Qual cuidado deve acompanhar o foco individual?
+ Manter contexto e comunicação; exceções ao limite devem ser explícitas
- Isolar-se totalmente da equipe
- Ignorar incidentes
- Nunca pedir ajuda
> Foco não é tunelamento.
:::

::: cartas
Concentração :: Foco da atenção num único ponto pelo tempo necessário.
WIP :: Trabalho em andamento; limitá-lo reduz trocas de contexto.
Contrapressão :: O consumidor "segura" o produtor quando não dá conta do ritmo.
Channel<T> :: Fila assíncrona do .NET para produtor/consumidor.
Pomodoro :: Técnica de blocos de foco com pausas curtas e longas.
Tempo de ciclo :: Tempo entre começar e concluir uma tarefa.
TimeProvider :: Abstração de relógio (.NET 8+) para código testável.
Caminho crítico :: Parte do código/fluxo que mais pesa no resultado.
:::
