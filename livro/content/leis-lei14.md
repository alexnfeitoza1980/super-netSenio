---
id: leis-lei14
order: 75
kind: capitulo
num: 55
group: C# & Mentalidade · Parte 12 · As 16 leis do sucesso em C#
title: Lei 14 — Aprenda a tirar proveito dos fracassos
subtitle: Falhas viram dados, dados viram aprendizado. Resultado<T> (falha como valor), métricas com Meter, post-mortem sem culpa e o ciclo praticar–aplicar–monitorar–modificar.
minutes: 55
hue: 8
objectives:
- Entender a lei: o fracasso **inevitável** é fonte de aprendizado quando **analisado** e não escondido
- Modelar **falhas como valores** com `Resultado<T>` (e acumular vários erros de uma vez)
- **Monitorar** com `System.Diagnostics.Metrics` ("quem não monitora, não administra")
- Escrever um **post-mortem sem culpa** (linha do tempo, cinco porquês, ações)
- Aplicar o ciclo **praticar → aplicar → monitorar → modificar**
origem: livros/03-leis-do-sucesso-csharp/content/74-lei14.md
---

::: vila A segunda-feira do post-mortem
Depois do incidente das 312 vendas sumidas, o gerente pergunta: *"De quem foi a culpa?"* Lia propõe outra pergunta: *"O que o sistema permitiu que acontecesse — e o que vamos mudar para que não aconteça de novo?"* A reunião dura uma hora e termina com **quatro ações com responsável e prazo**. Seis semanas depois, um erro parecido é detectado em **dois minutos**, por um alerta criado nessa reunião. O fracasso se pagou — porque foi **estudado**.
:::

::: ilustracao img/leis/lei14.svg | Fracassos viram aprendizado: causa no sistema, lição escrita e a planta que cresce.
:::

## A lei, em uma frase

> No caminho do sucesso, **tropeços e derrotas temporárias são inevitáveis e fazem parte do processo**; quem aprende a **analisá-los** extrai deles conhecimento que nenhum outro lugar oferece. Para o empreendedor, "fracasso absoluto" não existe — existe um **plano que não deu certo**. Se um plano falha, **troque-o por outro** e siga adiante.

A lição propõe quatro passos para evoluir uma habilidade: **(1) desenvolver pela prática, (2) aplicar de forma eficiente, (3) monitorar os resultados e (4) modificar o que não funciona** — lembrando que **"quem não monitora, não administra"**, e que é preciso refletir **sem se culpar nem se rotular** ("não sou bom nisso"). Ela também diz: *as adversidades não estão lá à toa* — mantenha a **mente aberta**, observe o seu trabalho e **prepare-se**.

## Da vida para o código

| O que a lei diz | O que isso significa para quem programa |
|---|---|
| Fracasso é parte do processo | **Falhas esperadas**: modelar como **valores** (`Resultado<T>`), não só como exceções |
| Analisar os fracassos | **Logs, métricas e traces** (observabilidade) |
| Monitorar os resultados | **Métricas** (`Meter`, `Counter`) e **alertas** |
| Não se culpar nem se rotular | ***Post-mortem* sem culpa** (foco no sistema) |
| Modificar o que não funciona | **Refatorar** e **corrigir a causa-raiz** (cinco porquês) |
| Plano falhou? Troque por outro | **Feature flags**, *rollback*, experimentos pequenos |
| Mente aberta, preparo | **Estudar** incidentes alheios (*post-mortems* públicos) |

::: figura O ciclo de aprendizagem
<svg viewBox="0 0 740 230" role="img" aria-label="Ciclo: praticar, aplicar, monitorar, modificar">
  <rect class="f-prim" x="40" y="30" width="180" height="60" rx="12"/><text x="130" y="58" text-anchor="middle" font-weight="800">1 · Praticar</text><text x="130" y="76" text-anchor="middle" font-size="11" class="t-dim">desenvolver a habilidade</text>
  <rect class="f-info" x="280" y="30" width="180" height="60" rx="12"/><text x="370" y="58" text-anchor="middle" font-weight="800">2 · Aplicar</text><text x="370" y="76" text-anchor="middle" font-size="11" class="t-dim">no trabalho real</text>
  <rect class="f-warn" x="520" y="30" width="180" height="60" rx="12"/><text x="610" y="58" text-anchor="middle" font-weight="800">3 · Monitorar</text><text x="610" y="76" text-anchor="middle" font-size="11" class="t-dim">métricas e logs</text>
  <rect class="f-good" x="280" y="140" width="180" height="60" rx="12"/><text x="370" y="168" text-anchor="middle" font-weight="800">4 · Modificar</text><text x="370" y="186" text-anchor="middle" font-size="11" class="t-dim">mudar o que falha</text>
  <path class="arrow" d="M222 60H278"/><path class="arrow" d="M462 60H518"/><path class="arrow" d="M610 92Q610 170 462 170"/><path class="arrow" d="M278 170Q130 170 130 92"/>
</svg>
:::

## 1) Falhas esperadas como valores: `Resultado<T>`

Nem toda falha é uma "exceção excepcional". Um cadastro com e-mail inválido é **um resultado normal do mundo real**. Tratá-lo como valor torna o código **mais honesto**: o tipo de retorno já avisa que pode dar errado — e o chamador é forçado a decidir o que fazer.

```cs Jornada/Lei14/Resultado.cs
namespace Jornada.Lei14;

public sealed record Erro(string Campo, string Mensagem);

public sealed class Resultado<T>
{
    private readonly T? valor;
    private readonly IReadOnlyList<Erro>? erros;

    private Resultado(T valor) => this.valor = valor;
    private Resultado(IReadOnlyList<Erro> erros) => this.erros = erros;

    public bool Sucesso => erros is null;
    public T Valor => Sucesso ? valor! : throw new InvalidOperationException("Um resultado de falha não tem valor.");
    public IReadOnlyList<Erro> Erros => erros ?? Array.Empty<Erro>();

    public static Resultado<T> Ok(T valor) => new(valor);
    public static Resultado<T> Falha(IReadOnlyList<Erro> erros) => new(erros);

    /// <summary>Transforma o valor, se houver; propaga os erros, se não houver.</summary>
    public Resultado<U> Mapear<U>(Func<T, U> funcao) =>
        Sucesso ? Resultado<U>.Ok(funcao(valor!)) : Resultado<U>.Falha(erros!);

    /// <summary>Encadeia outra operação que também pode falhar.</summary>
    public Resultado<U> Encadear<U>(Func<T, Resultado<U>> funcao) =>
        Sucesso ? funcao(valor!) : Resultado<U>.Falha(erros!);

    public TSaida Resolver<TSaida>(Func<T, TSaida> aoSucesso, Func<IReadOnlyList<Erro>, TSaida> aoFalhar) =>
        Sucesso ? aoSucesso(valor!) : aoFalhar(erros!);
}
```

```cs Jornada/Lei14/Cadastro.cs
namespace Jornada.Lei14;

public sealed record Usuario(string Nome, string Email, int Idade);

public static class Cadastro
{
    /// <summary>Valida TUDO e devolve todos os erros de uma vez (e não só o primeiro).</summary>
    public static Resultado<Usuario> Criar(string? nome, string? email, int idade)
    {
        var erros = new List<Erro>();

        if (string.IsNullOrWhiteSpace(nome))
            erros.Add(new Erro("nome", "Informe o nome."));

        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            erros.Add(new Erro("email", "Informe um e-mail válido (precisa conter @)."));

        if (idade is < 0 or > 130)
            erros.Add(new Erro("idade", "A idade deve estar entre 0 e 130."));

        return erros.Count == 0
            ? Resultado<Usuario>.Ok(new Usuario(nome!.Trim(), email!.Trim().ToLowerInvariant(), idade))
            : Resultado<Usuario>.Falha(erros);
    }
}
```

```cs Uso
static string Descrever(Resultado<Usuario> r) => r.Resolver(
    aoSucesso: u => $"OK: {u.Nome} <{u.Email}>, {u.Idade} anos",
    aoFalhar:  erros => $"FALHA ({erros.Count}): " + string.Join(" | ", erros.Select(e => $"{e.Campo}: {e.Mensagem}")));

Console.WriteLine(Descrever(Cadastro.Criar("Ana", "ANA@Exemplo.com", 30)));
Console.WriteLine(Descrever(Cadastro.Criar("", "sem-arroba", 200)));

// Encadeando: só continua se o cadastro deu certo.
var saudacao = Cadastro.Criar("Bia", "bia@exemplo.com", 28)
                       .Mapear(u => $"Olá, {u.Nome}!");
Console.WriteLine(saudacao.Valor);
```

```saida
OK: Ana <ana@exemplo.com>, 30 anos
FALHA (3): nome: Informe o nome. | email: Informe um e-mail válido (precisa conter @). | idade: A idade deve estar entre 0 e 130.
Olá, Bia!
```

::: dica Exceção ou resultado?
**Exceções** para situações **realmente excepcionais** (bug do programador, disco cheio, falta de memória). **`Resultado<T>`** (ou `TryXxx`) para falhas **esperadas** e **do domínio** (validação, regra de negócio violada). Isso evita usar exceções como controle de fluxo (que é lento e confuso) e deixa claro, no tipo, quais operações podem falhar.
:::

## 2) Monitorar: métricas que dizem o que está acontecendo

"Quem não monitora, não administra." No .NET, o namespace `System.Diagnostics.Metrics` oferece **contadores** e **histogramas** padronizados (compatíveis com OpenTelemetry). Vamos contar pedidos processados com sucesso e com falha — e deixar um "ouvinte" calcular a **taxa de falha**:

```cs Jornada/Lei14/Monitoramento.cs
using System.Diagnostics.Metrics;

namespace Jornada.Lei14;

public sealed class MetricasDePedidos : IDisposable
{
    public const string NomeDoMedidor = "Jornada.Pedidos";

    private readonly Meter medidor = new(NomeDoMedidor);
    private readonly Counter<int> ok;
    private readonly Counter<int> falhas;

    public MetricasDePedidos()
    {
        ok     = medidor.CreateCounter<int>("pedidos.ok",     description: "Pedidos processados com sucesso");
        falhas = medidor.CreateCounter<int>("pedidos.falhas", description: "Pedidos que falharam");
    }

    public void RegistrarSucesso() => ok.Add(1);
    public void RegistrarFalha(string motivo) => falhas.Add(1, new KeyValuePair<string, object?>("motivo", motivo));

    public void Dispose() => medidor.Dispose();
}

/// <summary>Um "ouvinte" em memória; na vida real, quem escuta é o OpenTelemetry, o Prometheus, o Application Insights…</summary>
public sealed class AcumuladorDeMetricas : IDisposable
{
    private readonly MeterListener listener = new();
    private readonly Dictionary<string, long> totais = new();

    public AcumuladorDeMetricas()
    {
        listener.InstrumentPublished = (instrumento, l) =>
        {
            if (instrumento.Meter.Name == MetricasDePedidos.NomeDoMedidor)
                l.EnableMeasurementEvents(instrumento);
        };
        listener.SetMeasurementEventCallback<int>((instrumento, valor, tags, estado) =>
            totais[instrumento.Name] = totais.GetValueOrDefault(instrumento.Name) + valor);
        listener.Start();
    }

    public long Total(string nome) => totais.GetValueOrDefault(nome);

    public double TaxaDeFalha
    {
        get
        {
            long total = Total("pedidos.ok") + Total("pedidos.falhas");
            return total == 0 ? 0 : (double)Total("pedidos.falhas") / total;
        }
    }

    public void Dispose() => listener.Dispose();
}
```

```cs Uso
using var acumulador = new AcumuladorDeMetricas();      // começa a ouvir ANTES de criar os instrumentos
using var metricas = new MetricasDePedidos();

for (int pedido = 1; pedido <= 10; pedido++)
{
    if (pedido % 4 == 0) metricas.RegistrarFalha(motivo: "estoque");          // 4 e 8
    else if (pedido == 10) metricas.RegistrarFalha(motivo: "pagamento");
    else metricas.RegistrarSucesso();
}

Console.WriteLine($"ok={acumulador.Total("pedidos.ok")}  falhas={acumulador.Total("pedidos.falhas")}  taxa de falha={acumulador.TaxaDeFalha:P0}".Replace(" %", "%"));
```

```saida
ok=7  falhas=3  taxa de falha=30%
```

O `tags` (`motivo`) permite **quebrar** a métrica por causa: com ele, um painel mostra que *"2 das 3 falhas foram de estoque"* — **informação que vira ação**. E um **alerta** ("taxa de falha > 5% por 5 minutos") transforma o monitoramento em **iniciativa** (Lei 5): o sistema avisa **antes** do cliente.

## 3) O post-mortem sem culpa

A lei manda **refletir sem se culpar nem se rotular**. Em equipes, o instrumento é o ***post-mortem* sem culpa** (*blameless*): foca nas **condições do sistema** que permitiram a falha, não em quem apertou o botão. Estruturado como dado, fica fácil de gerar, revisar e arquivar:

```cs Jornada/Lei14/PostMortem.cs
using System.Text;

namespace Jornada.Lei14;

public sealed record AcaoCorretiva(string Descricao, string Responsavel, DateOnly Prazo);

public sealed record PostMortem(
    string Titulo,
    DateOnly Data,
    string Resumo,
    IReadOnlyList<string> LinhaDoTempo,
    IReadOnlyList<string> CincoPorques,
    IReadOnlyList<string> OQueFuncionou,
    IReadOnlyList<AcaoCorretiva> Acoes)
{
    public string ComoMarkdown()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"# Post-mortem: {Titulo}");
        sb.AppendLine($"Data: {Data:yyyy-MM-dd}");
        sb.AppendLine();
        sb.AppendLine("## Resumo").AppendLine(Resumo).AppendLine();

        sb.AppendLine("## Linha do tempo");
        foreach (var item in LinhaDoTempo) sb.AppendLine($"- {item}");

        sb.AppendLine().AppendLine("## Cinco porquês");
        for (int i = 0; i < CincoPorques.Count; i++) sb.AppendLine($"{i + 1}. {CincoPorques[i]}");

        sb.AppendLine().AppendLine("## O que funcionou");
        foreach (var item in OQueFuncionou) sb.AppendLine($"- {item}");

        sb.AppendLine().AppendLine("## Ações");
        foreach (var a in Acoes) sb.AppendLine($"- [ ] {a.Descricao} — {a.Responsavel} (até {a.Prazo:yyyy-MM-dd})");

        return sb.ToString().TrimEnd();
    }
}
```

```cs Uso
var pm = new PostMortem(
    Titulo: "Importação de pedidos falhou em silêncio",
    Data: new DateOnly(2026, 10, 16),
    Resumo: "312 pedidos não foram importados porque um erro foi engolido por um catch vazio. Detectado pelo financeiro 3 dias depois.",
    LinhaDoTempo: new[]
    {
        "Sex 18:02 — a importação noturna falha ao ler um arquivo malformado",
        "Sex 18:02 — o catch vazio descarta o erro; o job termina como 'sucesso'",
        "Seg 09:40 — o financeiro percebe a diferença nos relatórios",
    },
    CincoPorques: new[]
    {
        "Por que 312 pedidos sumiram? — A importação falhou e o job terminou como sucesso.",
        "Por que terminou como sucesso? — Um catch vazio engoliu a exceção.",
        "Por que um catch vazio chegou à produção? — Nenhuma regra de análise estática o impedia.",
        "Por que ninguém notou? — Não havia alerta para a taxa de falha da importação.",
        "Por que não havia alerta? — Métricas do job nunca fizeram parte da definição de pronto.",
    },
    OQueFuncionou: new[] { "O financeiro tinha relatórios de conferência", "O arquivo original estava preservado" },
    Acoes: new[]
    {
        new AcaoCorretiva("Proibir catch vazio via analisador (TreatWarningsAsErrors)", "Lia", new DateOnly(2026, 10, 23)),
        new AcaoCorretiva("Criar métrica e alerta de taxa de falha da importação", "Caio", new DateOnly(2026, 10, 30)),
        new AcaoCorretiva("Incluir 'métricas e alertas' na definição de pronto", "Time", new DateOnly(2026, 11, 6)),
    });

Console.WriteLine(pm.ComoMarkdown());
```

```saida
# Post-mortem: Importação de pedidos falhou em silêncio
Data: 2026-10-16

## Resumo
312 pedidos não foram importados porque um erro foi engolido por um catch vazio. Detectado pelo financeiro 3 dias depois.

## Linha do tempo
- Sex 18:02 — a importação noturna falha ao ler um arquivo malformado
- Sex 18:02 — o catch vazio descarta o erro; o job termina como 'sucesso'
- Seg 09:40 — o financeiro percebe a diferença nos relatórios

## Cinco porquês
1. Por que 312 pedidos sumiram? — A importação falhou e o job terminou como sucesso.
2. Por que terminou como sucesso? — Um catch vazio engoliu a exceção.
3. Por que um catch vazio chegou à produção? — Nenhuma regra de análise estática o impedia.
4. Por que ninguém notou? — Não havia alerta para a taxa de falha da importação.
5. Por que não havia alerta? — Métricas do job nunca fizeram parte da definição de pronto.

## O que funcionou
- O financeiro tinha relatórios de conferência
- O arquivo original estava preservado

## Ações
- [ ] Proibir catch vazio via analisador (TreatWarningsAsErrors) — Lia (até 2026-10-23)
- [ ] Criar métrica e alerta de taxa de falha da importação — Caio (até 2026-10-30)
- [ ] Incluir 'métricas e alertas' na definição de pronto — Time (até 2026-11-06)
```

Repare como **nenhum "porquê" aponta para uma pessoa** — todos apontam para **condições do sistema**. As ações também são sobre o **sistema** (analisador, alerta, definição de pronto) — e cada uma tem **responsável e prazo**. É assim que um fracasso vira **ativo**: ele paga a si mesmo (e, como na Lei 8, o quarto passo é *aprender*).

::: pense "Mas e a responsabilidade?"
*Sem culpa* **não** significa *sem responsabilidade*: significa que a **conversa sobre o que mudar** não é contaminada pelo medo de punição. Quem tem medo esconde erros — e erros escondidos (Lei 8) são os mais caros. Ambientes que **aprendem** com as falhas as relatam mais cedo e as corrigem melhor.
:::

## 4) O ciclo no dia a dia

Os quatro passos da lição aparecem em muitas práticas que você já conhece:

| Passo | Prática de engenharia |
|---|---|
| **Praticar** | *Katas*, projetos pessoais, estudo de código alheio |
| **Aplicar** | *Pull requests* pequenos; entregas em produção frequentes |
| **Monitorar** | Logs estruturados, métricas, *traces*, relatórios de erro |
| **Modificar** | Refatoração, correção de causa-raiz, *feature flags*, *rollback* |

E um conselho final: **estude o fracasso dos outros**. Muitas empresas publicam *post-mortems* públicos de falhas famosas; ler alguns é uma forma barata de "falhar sem pagar o preço".

::: exercicio Taxa de falha por motivo
Estenda o `AcumuladorDeMetricas` para também guardar o total de falhas **por `motivo`** (use o parâmetro `tags` do *callback*) e exponha `FalhasPorMotivo(string motivo)`. No exemplo dos 10 pedidos, qual deve ser o resultado para `"estoque"` e `"pagamento"`?

--- resposta
```cs AcumuladorDeMetricas.cs (trecho)
private readonly Dictionary<string, long> porMotivo = new();

// no construtor:
listener.SetMeasurementEventCallback<int>((instrumento, valor, tags, estado) =>
{
    totais[instrumento.Name] = totais.GetValueOrDefault(instrumento.Name) + valor;

    if (instrumento.Name == "pedidos.falhas")
        foreach (var tag in tags)
            if (tag.Key == "motivo" && tag.Value is string motivo)
                porMotivo[motivo] = porMotivo.GetValueOrDefault(motivo) + valor;
});

public long FalhasPorMotivo(string motivo) => porMotivo.GetValueOrDefault(motivo);

// estoque → 2 (pedidos 4 e 8) · pagamento → 1 (pedido 10)
```
Com esse dado, a ação deixa de ser "melhorar o processamento" e passa a ser "olhar o estoque", que causa dois terços das falhas — **monitorar** aponta **onde modificar**.
:::

::: resumo
- **Lei 14:** fracassos são inevitáveis; quem os **analisa** extrai aprendizado. Ciclo: **praticar → aplicar → monitorar → modificar**, sem culpa nem rótulos.
- **`Resultado<T>`** modela falhas **esperadas** como valores (acumulando todos os erros); exceções ficam para o realmente excepcional.
- **Monitoramento** com `Meter`/`Counter` (e *tags*) e alertas: "quem não monitora, não administra".
- **Post-mortem sem culpa:** linha do tempo, **cinco porquês** (causas no sistema), o que funcionou e **ações com responsável e prazo**.
- **Mudar o sistema**, não punir pessoas: ambientes sem medo relatam e corrigem erros mais cedo.
- Estude também as falhas **de outros** (post-mortems públicos).
:::

::: quiz
? Segundo a lei, o que é "fracasso absoluto" para o verdadeiro empreendedor?
+ Não existe: é apenas um plano que não deu certo e do qual se extrai aprendizado
- Algo definitivo e vergonhoso
- Culpa de alguém
- Um sinal para desistir
> Analisar o fracasso é a fonte do conhecimento.

? Quando usar Resultado<T> em vez de exceção?
+ Para falhas esperadas do domínio (validação, regras de negócio)
- Para bugs do programador
- Para falta de memória
- Nunca
> Exceções ficam para o realmente excepcional.

? O que o "tags" (por exemplo, motivo) de uma métrica permite?
+ Quebrar a métrica por causa, indicando onde agir
- Esconder dados
- Reduzir o custo de armazenamento
- Substituir os logs
> Informação que vira ação.

? Qual a característica de um post-mortem sem culpa?
+ Focar nas condições do sistema que permitiram a falha e gerar ações com responsável e prazo
- Descobrir e punir o culpado
- Evitar registrar o ocorrido
- Ser feito só por gerentes
> Responsabilidade sem medo de punição.

? O que são os "cinco porquês"?
+ Perguntar "por quê?" repetidamente para chegar à causa-raiz
- Cinco testes obrigatórios
- Cinco ferramentas de monitoramento
- Uma técnica de otimização
> Normalmente leva de sintomas a condições sistêmicas.

? Qual é o quarto passo do ciclo da lição?
+ Modificar o que não funciona (e repetir o que funciona)
- Esquecer
- Culpar
- Parar de monitorar
> Praticar, aplicar, monitorar, modificar.
:::

::: cartas
Fracasso útil :: Falha analisada que gera aprendizado e mudança.
Resultado<T> :: Tipo que representa sucesso (valor) ou falha (erros) sem exceção.
Observabilidade :: Capacidade de entender o sistema a partir de logs, métricas e traces.
Meter / Counter :: API do .NET para métricas contadas (System.Diagnostics.Metrics).
Post-mortem sem culpa :: Análise de incidente focada no sistema, não em pessoas.
Cinco porquês :: Técnica de perguntar "por quê?" até a causa-raiz.
Feature flag :: Interruptor que liga/desliga funcionalidades sem novo deploy.
"Quem não monitora, não administra" :: Sem medir, não há como gerir ou melhorar.
:::
