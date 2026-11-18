---
id: leis-lei07
order: 68
kind: capitulo
num: 48
group: C# & Mentalidade · Parte 12 · As 16 leis do sucesso em C#
title: Lei 7 — Alimente seu entusiasmo
subtitle: Entusiasmo vem de um propósito que serve a alguém e de progresso que dá para ver. Eventos, IProgress, marcos comemorados, sequência de dias — e a frase de propósito que acende o motor.
minutes: 45
hue: 340
objectives:
- Entender o **entusiasmo** como combustível da ação, ligado a propósito e a servir outras pessoas
- Dar **visibilidade ao progresso** com eventos (`event`), `IProgress<T>` e uma barra de progresso
- Comemorar **marcos** com o padrão **Observer** (vários "ouvintes" reagindo ao mesmo evento)
- Medir **constância** com uma sequência (*streak*) de dias — e testá-la
- Escrever uma **frase de propósito** em cinco perguntas e evitar o entusiasmo insustentável
origem: livros/03-leis-do-sucesso-csharp/content/67-lei07.md
---

::: vila A barra que não andava
Lia está há três semanas numa migração enorme. O trabalho avança, mas **ela não sente**: o painel mostra "Em andamento" desde o primeiro dia. Quando o time acrescenta uma barra de progresso e uma mensagem a cada 25% concluídos, tudo muda: as pessoas comentam, comemoram, se ajudam nos últimos itens. Nada no código de negócio mudou — mudou o **feedback**.
:::

::: ilustracao img/leis/lei07.svg | Entusiasmo: comemorar cada marco mantém o ritmo da jornada.
:::

## A lei, em uma frase

> A ação prática que leva ao sucesso começa com um estado de espírito chamado **entusiasmo**: a força que cria a ação. Ele nasce de um **propósito claro**, é alimentado por pensamentos e metas que você retoma com frequência e se **fortalece quando você tem claro a quem serve** — e é contagioso: pessoas entusiasmadas atraem aliados.

Um detalhe interessante da lição: ao montar a "frase de propósito" (quem sou, o que faço, para quem, do que essas pessoas precisam e como mudam depois do meu trabalho), **só duas das cinco perguntas falam de você; as outras três falam dos outros**. A conclusão é que o entusiasmo duradouro vem de **pôr o seu melhor a serviço de alguém**, não de se motivar no vazio.

## Da vida para o código

| O que a lei diz | O que isso significa para quem programa |
|---|---|
| Entusiasmo cria a ação | **Feedback rápido** mantém o time em movimento |
| Propósito claro alimenta a emoção | Um **objetivo visível** (barra, painel, *burndown*) |
| Pensar com frequência no propósito | **Rituais**: demonstração, retro, quadro de tarefas |
| Entusiasmo é contagioso | **Eventos** que notificam várias pessoas/sistemas ao mesmo tempo |
| Servir os outros dá sentido | Conectar tarefas ao **usuário que será beneficiado** |
| Constância | **Sequência** de dias com progresso (*streak*) |
| Entusiasmo sustentável | **Ritmo sustentável** (cap. 16), não *sprints* de heroísmo |

::: figura Progresso comemorado
<svg viewBox="0 0 700 170" role="img" aria-label="Barra de progresso com marcos em 25, 50, 75 e 100 por cento">
  <rect x="30" y="70" width="600" height="28" rx="14" fill="none" stroke="var(--line)"/>
  <rect x="30" y="70" width="450" height="28" rx="14" fill="var(--good)" opacity=".75"/>
  <g font-size="12" text-anchor="middle">
    <circle cx="180" cy="84" r="8" fill="var(--warn)"/><text x="180" y="125" >25%</text><text x="180" y="140" class="t-dim" font-size="10">primeiro quarto</text>
    <circle cx="330" cy="84" r="8" fill="var(--warn)"/><text x="330" y="125">50%</text><text x="330" y="140" class="t-dim" font-size="10">metade do caminho</text>
    <circle cx="480" cy="84" r="8" fill="var(--warn)"/><text x="480" y="125">75%</text><text x="480" y="140" class="t-dim" font-size="10">reta final</text>
    <circle cx="630" cy="84" r="8" fill="var(--bad)"/><text x="630" y="125">100%</text><text x="630" y="140" class="t-dim" font-size="10">meta alcançada</text>
  </g>
  <text x="330" y="40" text-anchor="middle" font-weight="800" class="t-good">🎉 cada marco dispara um evento</text>
</svg>
:::

## 1) Dar visibilidade ao progresso: um rastreador com eventos

Em C#, um **evento** (`event`) permite que um objeto **avise** quem estiver interessado — sem saber quem são. É o padrão **Observer** (cap. 35) nativo da linguagem. Vamos criar um rastreador que *dispara um evento* sempre que atinge um marco (25%, 50%, 75% e 100%):

```cs Jornada/Lei07/RastreadorDeProgresso.cs
namespace Jornada.Lei07;

public sealed record MarcoAtingido(int Percentual, string Mensagem);   // desde o .NET Core, o argumento do evento não precisa herdar de EventArgs

public sealed class RastreadorDeProgresso
{
    private static readonly (int Percentual, string Mensagem)[] Marcos =
    {
        (25,  "Primeiro quarto concluído!"),
        (50,  "Metade do caminho!"),
        (75,  "Reta final!"),
        (100, "Meta alcançada!"),
    };

    private readonly int total;
    private readonly HashSet<int> jaComemorados = new();
    private int feito;

    public event EventHandler<MarcoAtingido>? Marco;          // quem quiser, assina

    public RastreadorDeProgresso(int total)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(total);
        this.total = total;
    }

    public double Fracao => (double)feito / total;

    public void Avancar(int quantidade = 1)
    {
        feito = Math.Min(total, feito + quantidade);
        int pct = (int)(Fracao * 100);

        foreach (var (percentual, mensagem) in Marcos)
            if (pct >= percentual && jaComemorados.Add(percentual))      // cada marco só uma vez
                Marco?.Invoke(this, new MarcoAtingido(percentual, mensagem));
    }

    public string Barra(int largura = 20)
    {
        int cheios = (int)Math.Round(Fracao * largura);
        return "[" + new string('█', cheios) + new string('░', largura - cheios) + $"] {Fracao * 100:F0}%";
    }
}
```

```cs Uso
var rastreador = new RastreadorDeProgresso(total: 8);
var logDoTime = new List<string>();

// Primeiro ouvinte: o painel na tela.
rastreador.Marco += (_, e) => Console.WriteLine($"🎉 {e.Percentual}% — {e.Mensagem}");

// Segundo ouvinte: um registro para a retrospectiva.
rastreador.Marco += (_, e) => logDoTime.Add($"{e.Percentual}%");

for (int i = 0; i < 8; i++)
    rastreador.Avancar();          // cada item migrado

Console.WriteLine(rastreador.Barra());
Console.WriteLine($"Marcos registrados no log: {logDoTime.Count}");
```

```saida
🎉 25% — Primeiro quarto concluído!
🎉 50% — Metade do caminho!
🎉 75% — Reta final!
🎉 100% — Meta alcançada!
[████████████████████] 100%
Marcos registrados no log: 4
```

Note como o rastreador **não sabe** que existe um painel nem um log: ele só levanta o evento. Amanhã dá para assinar também um bot do chat, sem mexer no rastreador (OCP). Isso é **entusiasmo contagioso, em forma de código**: um acontecimento, várias pessoas informadas.

::: cuidado Evento esquecido = vazamento
Quem assina um evento com `+=` passa a ser **mantido vivo** pelo publicador. Em objetos de vida longa (um serviço singleton), esqueça o `-=` e você terá um vazamento de memória. Para tempos de vida diferentes, use `IDisposable` para cancelar a assinatura, ou *weak events*.
:::

## 2) Reportar progresso de operações longas: `IProgress<T>`

Em operações demoradas (importar arquivos, migrar dados), quem chama quer saber **como vai**. `IProgress<T>` desacopla "quem trabalha" de "quem mostra":

```cs Jornada/Lei07/Importador.cs
namespace Jornada.Lei07;

public sealed record AvancoDaImportacao(int Processados, int Total)
{
    public double Fracao => Total == 0 ? 1 : (double)Processados / Total;
}

public static class Importador
{
    public static async Task ImportarAsync(IReadOnlyList<string> registros,
                                           IProgress<AvancoDaImportacao>? progresso = null,
                                           CancellationToken ct = default)
    {
        for (int i = 0; i < registros.Count; i++)
        {
            ct.ThrowIfCancellationRequested();
            await Task.Delay(10, ct);                                   // simula gravar o registro
            progresso?.Report(new AvancoDaImportacao(i + 1, registros.Count));
        }
    }
}
```

Um `IProgress<T>` simples e **síncrono** (útil em testes e em apps de console; em interface gráfica, prefira `new Progress<T>(...)`, que publica no *thread* da tela):

```cs Jornada/Lei07/SincronoProgress.cs
namespace Jornada.Lei07;

public sealed class SincronoProgress<T> : IProgress<T>
{
    private readonly Action<T> acao;
    public SincronoProgress(Action<T> acao) => this.acao = acao;
    public void Report(T valor) => acao(valor);
}
```

```cs Uso
var registros = Enumerable.Range(1, 5).Select(n => $"registro {n}").ToList();

var ultimo = new AvancoDaImportacao(0, registros.Count);
var progresso = new SincronoProgress<AvancoDaImportacao>(a => ultimo = a);

await Importador.ImportarAsync(registros, progresso);
Console.WriteLine($"Importados {ultimo.Processados} de {ultimo.Total} ({ultimo.Fracao * 100:F0}%)");
```

```saida
Importados 5 de 5 (100%)
```

## 3) Constância: a sequência de dias

O entusiasmo flutua; **a constância sustenta**. Uma métrica motivadora e simples é a **sequência** (*streak*): quantos dias seguidos você fez progresso. Vamos calculá-la — e testá-la:

```cs Jornada/Lei07/Sequencia.cs
namespace Jornada.Lei07;

public static class Sequencia
{
    /// <summary>Dias consecutivos com progresso, terminando hoje (ou ontem, se hoje ainda não registrou).</summary>
    public static int Atual(IEnumerable<DateOnly> diasComProgresso, DateOnly hoje)
    {
        var dias = diasComProgresso.ToHashSet();
        var cursor = dias.Contains(hoje) ? hoje : hoje.AddDays(-1);   // tolera "o dia ainda não acabou"

        int contagem = 0;
        while (dias.Contains(cursor))
        {
            contagem++;
            cursor = cursor.AddDays(-1);
        }
        return contagem;
    }
}
```

```cs JornadaTests/Lei07/SequenciaTests.cs
using Jornada.Lei07;
using Xunit;

public class SequenciaTests
{
    private static DateOnly Dia(int d) => new(2026, 10, d);

    [Fact]
    public void Conta_dias_seguidos_ate_hoje()
    {
        var dias = new[] { Dia(21), Dia(22), Dia(23), Dia(24) };
        Assert.Equal(4, Sequencia.Atual(dias, hoje: Dia(24)));
    }

    [Fact]
    public void Hoje_ainda_sem_registro_nao_zera_a_sequencia()
    {
        var dias = new[] { Dia(22), Dia(23), Dia(24) };
        Assert.Equal(3, Sequencia.Atual(dias, hoje: Dia(25)));
    }

    [Fact]
    public void Uma_lacuna_interrompe_a_sequencia()
    {
        var dias = new[] { Dia(20), Dia(22), Dia(23) };
        Assert.Equal(2, Sequencia.Atual(dias, hoje: Dia(23)));
    }

    [Fact]
    public void Sem_nenhum_dia_a_sequencia_e_zero()
    {
        Assert.Equal(0, Sequencia.Atual(Array.Empty<DateOnly>(), hoje: Dia(25)));
    }
}
```

::: dica Sequência sem culpa
Sequências motivam, mas também **punem** quem falha um dia. Se o seu time (ou aplicativo) usa esse recurso, permita "dias de descanso" ou uma **tolerância** (Lei 15). O objetivo é *constância sustentável*, não ansiedade — cuidado com o entusiasmo que vira esgotamento.
:::

## 4) A frase de propósito: quem você serve

Para acender o motor, a lição propõe responder a cinco perguntas e juntá-las em uma frase. Fica bem em código — e o *tipo* pode lembrar que **três das cinco perguntas falam dos outros**:

```cs Jornada/Lei07/FraseDeProposito.cs
namespace Jornada.Lei07;

public sealed record FraseDeProposito(
    string Quem,            // quem sou eu
    string Faz,             // o que faço
    string ParaQuem,        // para quem faço
    string Necessidade,     // do que essas pessoas precisam
    string Transformacao)   // como elas ficam depois do meu trabalho
{
    public bool Completa => new[] { Quem, Faz, ParaQuem, Necessidade, Transformacao }
                            .All(p => !string.IsNullOrWhiteSpace(p));

    public override string ToString() =>
        Completa
            ? $"Eu sou {Quem}, {Faz} para {ParaQuem}, que precisam {Necessidade}, a fim de {Transformacao}."
            : "Ainda falta responder alguma das cinco perguntas.";
}
```

```cs Uso
var lia = new FraseDeProposito(
    Quem:          "Lia",
    Faz:           "crio bibliotecas .NET simples e bem testadas",
    ParaQuem:      "equipes pequenas de produto",
    Necessidade:   "entregar rápido sem perder a qualidade",
    Transformacao: "dormirem tranquilas em dia de deploy");

Console.WriteLine(lia);
Console.WriteLine(lia with { Necessidade = "" });
```

```saida
Eu sou Lia, crio bibliotecas .NET simples e bem testadas para equipes pequenas de produto, que precisam entregar rápido sem perder a qualidade, a fim de dormirem tranquilas em dia de deploy.
Ainda falta responder alguma das cinco perguntas.
```

Tente escrever a sua. Se tiver dificuldade na terceira, quarta ou quinta pergunta, é porque ainda não está claro **a quem** o seu trabalho serve — e é nessa clareza que o entusiasmo duradouro nasce.

::: pense Motivação × disciplina
Entusiasmo ajuda a **começar**; **disciplina** ajuda a **continuar** quando o entusiasmo acaba (e ele vai acabar na terça-feira de manhã). Os melhores times combinam os dois: **feedback visível** (entusiasmo) + **automação e hábitos** (disciplina: Leis 3 e 4). E lembre: o *ritmo sustentável* do XP existe justamente porque o entusiasmo **não** é infinito.
:::

::: exercicio Um marco a mais
1. Adicione ao `RastreadorDeProgresso` um evento `Concluido` (sem argumentos) disparado **uma única vez**, quando chegar a 100%.
2. Escreva um teste que assina o evento, chama `Avancar` mais vezes do que o total e verifica que o evento foi disparado **exatamente 1 vez**.

--- resposta
```cs RastreadorDeProgresso.cs (trecho)
public event EventHandler? Concluido;

public void Avancar(int quantidade = 1)
{
    bool estavaCompleto = feito >= total;
    feito = Math.Min(total, feito + quantidade);
    // ... código dos marcos ...
    if (!estavaCompleto && feito >= total)
        Concluido?.Invoke(this, EventArgs.Empty);      // só na transição para 100%
}
```
```cs RastreadorTests.cs (trecho)
public class RastreadorTests
{
    [Fact]
    public void Concluido_dispara_uma_unica_vez()
    {
        var r = new RastreadorDeProgresso(total: 3);
        int disparos = 0;
        r.Concluido += (_, _) => disparos++;

        for (int i = 0; i < 10; i++) r.Avancar();       // passa do total várias vezes

        Assert.Equal(1, disparos);
    }
}
```
:::

::: resumo
- **Lei 7:** o entusiasmo é a força que cria a ação; nasce do propósito e, sobretudo, de **servir alguém**; é contagioso.
- **Feedback visível** alimenta o entusiasmo: barras de progresso, marcos comemorados, quadros de tarefas.
- **Eventos** (`event`/`EventHandler<T>`) implementam o **Observer**: um acontecimento, vários interessados; lembre do `-=`.
- `IProgress<T>` desacopla quem trabalha de quem exibe o progresso; `Progress<T>` volta ao *thread* da tela.
- **Constância** > intensidade: meça a **sequência** de dias (com tolerância) e teste a regra.
- A **frase de propósito** (cinco perguntas, três sobre os outros) acende e orienta o entusiasmo; cuidado com o esgotamento.
:::

::: quiz
? De onde, segundo a lição, nasce o entusiasmo duradouro?
+ De um propósito claro que põe o seu melhor a serviço de outras pessoas
- De motivação aleatória
- De dinheiro apenas
- De competir com colegas
> Das cinco perguntas, três falam dos outros.

? Qual recurso do C# implementa nativamente o padrão Observer?
+ event / EventHandler
- goto
- struct
- unsafe
> Publicador e assinantes sem conhecimento mútuo.

? Por que o rastreador usa um `HashSet<int>` de marcos já comemorados?
+ Para disparar cada marco no máximo uma vez
- Para ordenar os marcos
- Para economizar memória
- Para evitar exceções
> `Add` devolve false se o item já existia.

? Para que serve `IProgress<T>`?
+ Reportar o andamento de uma operação sem acoplar o trabalhador à interface que exibe
- Cancelar tarefas
- Garantir exclusão mútua
- Serializar objetos
> `Progress<T>` posta no contexto de sincronização (thread da UI).

? Qual o risco de assinar eventos com `+=` em objetos de vida longa e esquecer o `-=`?
+ Vazamento de memória: o publicador mantém o assinante vivo
- Erro de compilação
- Evento disparado duas vezes sempre
- Nenhum
> Use IDisposable ou weak events para tempos de vida distintos.

? Por que a função de sequência tolera "hoje ainda sem registro"?
+ Para não zerar a sequência de quem ainda vai trabalhar no dia (constância sem punição injusta)
- Porque o dia nunca acaba
- Para ignorar fins de semana
- Para corrigir bugs de fuso
> O dia ainda não terminou.
:::

::: cartas
Entusiasmo :: Estado de espírito que impulsiona a ação, alimentado por propósito e por servir os outros.
Feedback visível :: Mostrar o progresso para manter a motivação.
event :: Membro que permite a objetos assinarem notificações do publicador.
Observer :: Padrão em que dependentes são notificados automaticamente de mudanças.
IProgress<T> :: Contrato para reportar progresso de operações.
Marco :: Ponto intermediário significativo (25%, 50%…).
Sequência (streak) :: Número de dias consecutivos com progresso.
Ritmo sustentável :: Jornada que a equipe consegue manter indefinidamente.
:::
