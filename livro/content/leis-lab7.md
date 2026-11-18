---
id: leis-lab7
order: 78
kind: lab
num: 7
group: C# & Mentalidade · Parte 12 · As 16 leis do sucesso em C#
title: Laboratório 7 — O Diário da Jornada
subtitle: Um pequeno aplicativo que junta as 16 leis: meta e plano, progresso comemorado, quadro com limite, leitura tolerante, gravação com retentativas, resultados em vez de exceções, contratos testados.
minutes: 120
hue: 25
badge: L7
objectives:
- **Combinar** em um único projeto os módulos criados nos capítulos 42 a 57
- Desenhar um **núcleo testável** com interface de repositório, **contrato** compartilhado e *composition root*
- Ler dados de forma **tolerante** e devolver **`Resultado<T>`** com todos os erros
- Gravar com **troca atômica** e **retentativa**, respeitando **cancelamento**
- Verificar o conjunto com **testes de contrato** e de **cenário**
origem: livros/03-leis-do-sucesso-csharp/content/77-lab7.md
---

::: vila A jornada vira ferramenta
Depois de dezesseis leis, Lia quer um lugar para **registrar a própria jornada**: a meta, os passos, quantos dias seguidos ela estudou, o que está em andamento. Ela decide transformar o *Jornada* — a biblioteca que vem crescendo a cada capítulo — em um **Diário** de verdade: grava em arquivo, lê arquivos antigos (e mal formatados), comemora os marcos, limita o trabalho em andamento e **não se perde** quando algo dá errado.
:::

::: ilustracao img/leis/lab7.svg | O Diário da Jornada junta metas, marcos, quadro com limite, leitura tolerante e gravação segura.
:::

## O que vamos construir

O **Diário da Jornada** é uma biblioteca (com um pequeno programa de console) que:

1. guarda uma **meta** e um **plano de ação** (Lei 1);
2. permite **iniciar** e **concluir** passos, com **no máximo 2 em andamento** (Lei 12);
3. **comemora marcos** de 25/50/75/100% e conta a **sequência de dias** (Lei 7);
4. **lê** arquivos de qualquer época — com nomes em caixas diferentes, números como texto, comentários e vírgulas sobrando (Lei 15) — e **devolve todos os erros de uma vez** quando o arquivo é inválido (Leis 10 e 14);
5. **grava** com **troca atômica** e **retentativa** para falhas passageiras, respeitando **cancelamento** (Leis 8 e 13);
6. tem **testes de contrato** para cada repositório e **testes de cenário** para a sessão (Leis 3 e 16).

::: figura Arquitetura do Diário
<svg viewBox="0 0 760 330" role="img" aria-label="Programa usa SessaoDaJornada; sessão usa interface IRepositorioDoDiario implementada por Memoria e Arquivo; usa Plano, Quadro, Rastreador, Serializador">
  <rect class="f-prim" x="290" y="10" width="180" height="46" rx="10"/><text x="380" y="38" text-anchor="middle" font-weight="800">Program (composition root)</text>
  <rect class="f-info" x="270" y="86" width="220" height="56" rx="10"/><text x="380" y="112" text-anchor="middle" font-weight="800">SessaoDaJornada</text><text x="380" y="130" text-anchor="middle" font-size="11" class="t-dim">orquestra o diário</text>
  <rect class="f-good" x="20" y="90" width="200" height="48" rx="10"/><text x="120" y="110" text-anchor="middle" font-size="13">PlanoDeAcao + Meta</text><text x="120" y="128" text-anchor="middle" font-size="10" class="t-dim">Lei 1</text>
  <rect class="f-good" x="20" y="150" width="200" height="48" rx="10"/><text x="120" y="170" text-anchor="middle" font-size="13">RastreadorDeProgresso</text><text x="120" y="188" text-anchor="middle" font-size="10" class="t-dim">Lei 7 · eventos</text>
  <rect class="f-good" x="540" y="90" width="200" height="48" rx="10"/><text x="640" y="110" text-anchor="middle" font-size="13">QuadroKanban (WIP 2)</text><text x="640" y="128" text-anchor="middle" font-size="10" class="t-dim">Lei 12</text>
  <rect class="f-good" x="540" y="150" width="200" height="48" rx="10"/><text x="640" y="170" text-anchor="middle" font-size="13">Sequencia (dias)</text><text x="640" y="188" text-anchor="middle" font-size="10" class="t-dim">Lei 7</text>
  <rect class="f-warn" x="270" y="180" width="220" height="48" rx="10"/><text x="380" y="200" text-anchor="middle" font-size="13">«interface» IRepositorioDoDiario</text><text x="380" y="218" text-anchor="middle" font-size="10" class="t-dim">Leis 2 e 16</text>
  <rect class="f-plain" x="200" y="262" width="170" height="44" rx="8"/><text x="285" y="288" text-anchor="middle" font-size="13">RepositorioEmMemoria</text>
  <rect class="f-plain" x="390" y="262" width="170" height="44" rx="8"/><text x="475" y="288" text-anchor="middle" font-size="13">RepositorioEmArquivo</text>
  <path class="arrow" d="M380 58V84"/><path class="arrow" d="M268 112H222"/><path class="arrow" d="M268 128L222 168"/><path class="arrow" d="M492 112H538"/><path class="arrow" d="M492 128L538 168"/><path class="arrow" d="M380 144V178"/>
  <path d="M285 260V240H380V230" stroke="var(--ink-2)" fill="none" marker-end="url(#arr2)"/><path d="M475 260V240H380" stroke="var(--ink-2)" fill="none"/>
</svg>
:::

::: dica Antes de começar
Crie uma solução com dois projetos (`Jornada` e `JornadaTests`) e **reúna os módulos** dos capítulos anteriores na pasta `Jornada/`: `Meta` e `PlanoDeAcao` (cap. 42), `RastreadorDeProgresso` e `Sequencia` (cap. 48), `Persistencia` (cap. 49), `QuadroKanban` (cap. 53) e `Resultado<T>` (cap. 55). Todo o código novo deste laboratório fica no *namespace* `Jornada.Lab7`.
:::

## Passo 1 — O estado que vai para o arquivo

Primeiro, **o dado**. Usamos classes simples (DTOs) — o que é gravado deve ser fácil de ler, de editar à mão e de evoluir:

```cs Jornada/Lab7/EstadoDoDiario.cs
namespace Jornada.Lab7;

public sealed class PassoSalvo
{
    public string? Descricao { get; set; }
    public DateOnly Prazo { get; set; }
    public bool Concluido { get; set; }
}

public sealed class EstadoDoDiario
{
    public string? Objetivo { get; set; }
    public string? Motivo { get; set; }
    public string? Medida { get; set; }
    public DateOnly Prazo { get; set; }
    public List<PassoSalvo> Passos { get; set; } = new();
    public List<DateOnly> Dias { get; set; } = new();       // dias em que houve progresso
}
```

## Passo 2 — Ler com tolerância, devolver com rigor

O serializador **aceita** formatos variados (Lei 15) e **valida** o conteúdo, devolvendo um `Resultado` com **todos** os problemas (Leis 10 e 14). Na gravação, o formato é **canônico**:

```cs Jornada/Lab7/SerializadorDoDiario.cs
using System.Text.Json;
using System.Text.Json.Serialization;
using Jornada.Lei14;

namespace Jornada.Lab7;

public static class SerializadorDoDiario
{
    private static readonly JsonSerializerOptions Leitura = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        ReadCommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true,
    };

    private static readonly JsonSerializerOptions Escrita = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true,
    };

    public static string Escrever(EstadoDoDiario estado) => JsonSerializer.Serialize(estado, Escrita);

    public static Resultado<EstadoDoDiario> Ler(string json)
    {
        EstadoDoDiario? estado;
        try
        {
            estado = JsonSerializer.Deserialize<EstadoDoDiario>(json, Leitura);
        }
        catch (JsonException e)
        {
            return Resultado<EstadoDoDiario>.Falha(new[] { new Erro("json", $"O arquivo não é um JSON válido ({e.Message}).") });
        }

        if (estado is null)
            return Resultado<EstadoDoDiario>.Falha(new[] { new Erro("json", "O arquivo está vazio.") });

        var erros = new List<Erro>();
        if (string.IsNullOrWhiteSpace(estado.Objetivo)) erros.Add(new Erro("objetivo", "Informe o objetivo da meta."));
        if (string.IsNullOrWhiteSpace(estado.Motivo))   erros.Add(new Erro("motivo",   "Informe por que essa meta importa."));
        if (string.IsNullOrWhiteSpace(estado.Medida))   erros.Add(new Erro("medida",   "Informe como você vai saber que chegou lá."));
        if (estado.Passos.Any(p => string.IsNullOrWhiteSpace(p.Descricao)))
            erros.Add(new Erro("passos", "Todo passo precisa de uma descrição."));

        return erros.Count == 0 ? Resultado<EstadoDoDiario>.Ok(estado) : Resultado<EstadoDoDiario>.Falha(erros);
    }
}
```

## Passo 3 — O contrato do repositório (e duas implementações)

A **sessão** não pode saber *onde* o diário é guardado (DIP, Lei 2). Definimos o **contrato** com cuidado (Lei 16): ele diz o que acontece quando **não existe diário** e o que significa "salvar".

```cs Jornada/Lab7/Repositorios.cs
using Jornada.Lei08;
using Jornada.Lei14;

namespace Jornada.Lab7;

/// <summary>
/// Contrato: CarregarAsync devolve Falha (campo "arquivo") se ainda não existe diário,
/// e Falha com os erros de validação se o conteúdo é inválido. SalvarAsync sobrescreve o diário anterior.
/// </summary>
public interface IRepositorioDoDiario
{
    Task SalvarAsync(EstadoDoDiario estado, CancellationToken ct = default);
    Task<Resultado<EstadoDoDiario>> CarregarAsync(CancellationToken ct = default);
}

internal static class Falhas
{
    public static Resultado<EstadoDoDiario> NaoExiste() =>
        Resultado<EstadoDoDiario>.Falha(new[] { new Erro("arquivo", "Ainda não existe um diário. Crie um primeiro.") });
}

public sealed class RepositorioEmMemoria : IRepositorioDoDiario
{
    private string? json;

    /// <summary>Útil em testes e demonstrações: injeta um conteúdo "como se estivesse no disco".</summary>
    public void Semear(string conteudo) => json = conteudo;

    public Task SalvarAsync(EstadoDoDiario estado, CancellationToken ct = default)
    {
        json = SerializadorDoDiario.Escrever(estado);
        return Task.CompletedTask;
    }

    public Task<Resultado<EstadoDoDiario>> CarregarAsync(CancellationToken ct = default) =>
        Task.FromResult(json is null ? Falhas.NaoExiste() : SerializadorDoDiario.Ler(json));
}

public sealed class RepositorioEmArquivo : IRepositorioDoDiario
{
    private readonly string caminho;
    private readonly Action<string>? registrar;

    public RepositorioEmArquivo(string caminho, Action<string>? registrar = null)
    {
        this.caminho = caminho;
        this.registrar = registrar;
    }

    public async Task SalvarAsync(EstadoDoDiario estado, CancellationToken ct = default)
    {
        var json = SerializadorDoDiario.Escrever(estado);

        await Persistencia.ExecutarAsync<bool>(
            async (tentativa, token) =>
            {
                var temporario = caminho + ".tmp";
                await File.WriteAllTextAsync(temporario, json, token);
                File.Move(temporario, caminho, overwrite: true);       // troca atômica: nunca fica "meio escrito"
                return true;
            },
            maxTentativas: 3,
            atraso: n => Persistencia.Exponencial(n, baseMs: 50),
            ehPassageira: e => e is IOException,                       // disco ocupado, arquivo em uso…
            registrar: registrar,
            ct: ct);
    }

    public async Task<Resultado<EstadoDoDiario>> CarregarAsync(CancellationToken ct = default)
    {
        if (!File.Exists(caminho)) return Falhas.NaoExiste();
        return SerializadorDoDiario.Ler(await File.ReadAllTextAsync(caminho, ct));
    }
}
```

::: pense Por que "temporário + Move"?
Se o programa cair **no meio** de `WriteAllText`, o arquivo ficaria truncado — o diário **inteiro** perdido. Gravando num arquivo temporário e **trocando** só quando termina, o arquivo final **ou é o antigo, ou é o novo**: nunca um pedaço. Pequeno cuidado, grande respeito pelos dados de quem usa (Leis 9 e 16).
:::

## Passo 4 — A sessão: onde as leis se encontram

A `SessaoDaJornada` **orquestra**: carrega o estado, monta o plano, o quadro e o rastreador, expõe as ações e salva. Ela só conhece **abstrações** e módulos pequenos e testados:

```cs Jornada/Lab7/SessaoDaJornada.cs
using Jornada.Lei01;
using Jornada.Lei07;
using Jornada.Lei12;
using Jornada.Lei14;

namespace Jornada.Lab7;

public sealed class SessaoDaJornada
{
    private readonly IRepositorioDoDiario repositorio;
    private readonly Func<DateOnly> hoje;                      // relógio injetado (Lei 1)
    private readonly List<DateOnly> dias;
    private readonly QuadroKanban quadro = new(limiteEmAndamento: 2);     // foco (Lei 12)
    private readonly RastreadorDeProgresso rastreador;

    public PlanoDeAcao Plano { get; }

    public event EventHandler<MarcoAtingido>? Marco                       // entusiasmo (Lei 7)
    {
        add { rastreador.Marco += value; }
        remove { rastreador.Marco -= value; }
    }

    private SessaoDaJornada(IRepositorioDoDiario repositorio, Func<DateOnly> hoje, EstadoDoDiario estado)
    {
        this.repositorio = repositorio;
        this.hoje = hoje;
        dias = estado.Dias.ToList();

        // Reidratação: a meta já foi validada quando foi criada; aqui apenas a reconstruímos.
        var meta = new Meta { Objetivo = estado.Objetivo!, Motivo = estado.Motivo!, Medida = estado.Medida!, Prazo = estado.Prazo };
        Plano = new PlanoDeAcao(meta);
        rastreador = new RastreadorDeProgresso(Math.Max(1, estado.Passos.Count));

        foreach (var passo in estado.Passos)
        {
            Plano.Adicionar(passo.Descricao!, passo.Prazo);
            quadro.Adicionar(passo.Descricao!);
        }

        foreach (var passo in estado.Passos.Where(p => p.Concluido))
        {
            Plano.Concluir(passo.Descricao!);
            quadro.Mover(passo.Descricao!, Coluna.Pronto);
            rastreador.Avancar();                               // o que já estava concluído não é "comemorado" de novo
        }
    }

    public static async Task<Resultado<SessaoDaJornada>> AbrirAsync(
        IRepositorioDoDiario repositorio, Func<DateOnly> hoje, CancellationToken ct = default)
    {
        var carregado = await repositorio.CarregarAsync(ct);
        return carregado.Mapear(estado => new SessaoDaJornada(repositorio, hoje, estado));
    }

    public double Progresso => Plano.Progresso;
    public int SequenciaAtual => Sequencia.Atual(dias, hoje());

    public void Iniciar(string passo) => quadro.Mover(passo, Coluna.EmAndamento);   // pode lançar LimiteDeTrabalhoException

    public void Concluir(string passo)
    {
        var existente = Plano.Passos.FirstOrDefault(p => p.Descricao == passo)
                        ?? throw new KeyNotFoundException($"O passo '{passo}' não existe neste diário.");
        if (existente.Concluido) return;                         // idempotente: concluir duas vezes não conta duas

        Plano.Concluir(passo);
        quadro.Mover(passo, Coluna.Pronto);
        rastreador.Avancar();
        RegistrarDia();
    }

    public void RegistrarDia()
    {
        var dia = hoje();
        if (!dias.Contains(dia)) dias.Add(dia);
    }

    public EstadoDoDiario Estado() => new()
    {
        Objetivo = Plano.Meta.Objetivo,
        Motivo = Plano.Meta.Motivo,
        Medida = Plano.Meta.Medida,
        Prazo = Plano.Meta.Prazo,
        Passos = Plano.Passos.Select(p => new PassoSalvo { Descricao = p.Descricao, Prazo = p.Prazo, Concluido = p.Concluido }).ToList(),
        Dias = dias.OrderBy(d => d).ToList(),
    };

    public Task SalvarAsync(CancellationToken ct = default) => repositorio.SalvarAsync(Estado(), ct);
}
```

## Passo 5 — Rodando o diário

Um "arquivo legado" escrito à mão — com maiúsculas, comentário e vírgula sobrando — e uma sessão de trabalho:

```cs Uso
using Jornada.Lab7;
using Jornada.Lei07;
using Jornada.Lei12;

var hoje = new DateOnly(2026, 11, 3);
var repositorio = new RepositorioEmMemoria();

repositorio.Semear("""
{
  // criado à mão por alguém ocupado
  "Objetivo": "Dominar async/await em C#",
  "Motivo":   "Escrever serviços que não bloqueiam",
  "Medida":   "3 exemplos com testes",
  "Prazo":    "2027-01-31",
  "Passos": [
    { "Descricao": "Ler a documentação",      "Prazo": "2026-11-10", "Concluido": true },
    { "Descricao": "Escrever 3 exemplos",     "Prazo": "2026-12-15" },
    { "Descricao": "Explicar a um colega",    "Prazo": "2027-01-20" },
    { "Descricao": "Publicar o repositório",  "Prazo": "2027-01-25" },
  ],
  "Dias": ["2026-11-01", "2026-11-02"],
}
""");

var abertura = await SessaoDaJornada.AbrirAsync(repositorio, () => hoje);
var sessao = abertura.Valor;
sessao.Marco += (_, e) => Console.WriteLine($"🎉 {e.Percentual}% — {e.Mensagem}");

Console.WriteLine($"Meta: {sessao.Plano.Meta.Objetivo} · progresso inicial: {sessao.Progresso * 100:F0}% · sequência: {sessao.SequenciaAtual} dia(s)");

sessao.Iniciar("Escrever 3 exemplos");
sessao.Iniciar("Explicar a um colega");
try { sessao.Iniciar("Publicar o repositório"); }
catch (LimiteDeTrabalhoException e) { Console.WriteLine("Foco! " + e.Message); }

sessao.Concluir("Escrever 3 exemplos");
await sessao.SalvarAsync();

Console.WriteLine($"Hoje: progresso {sessao.Progresso * 100:F0}% · sequência: {sessao.SequenciaAtual} dia(s) · próximo passo: {sessao.Plano.ProximoPasso?.Descricao}");

var reaberta = (await SessaoDaJornada.AbrirAsync(repositorio, () => hoje)).Valor;
Console.WriteLine($"Reaberta do repositório: {reaberta.Progresso * 100:F0}% · sequência: {reaberta.SequenciaAtual} dia(s)");
```

```saida
Meta: Dominar async/await em C# · progresso inicial: 25% · sequência: 2 dia(s)
Foco! Limite de 2 tarefa(s) em andamento atingido: termine uma antes de começar outra.
🎉 50% — Metade do caminho!
Hoje: progresso 50% · sequência: 3 dia(s) · próximo passo: Explicar a um colega
Reaberta do repositório: 50% · sequência: 3 dia(s)
```

E quando o arquivo é **inválido**, o diário **explica tudo de uma vez**, em vez de lançar uma exceção:

```cs Uso (continuação)
var ruim = new RepositorioEmMemoria();
ruim.Semear("""{ "objetivo": "", "prazo": "2027-01-31" }""");

var tentativa = await SessaoDaJornada.AbrirAsync(ruim, () => hoje);
foreach (var erro in tentativa.Erros)
    Console.WriteLine($"✘ {erro.Campo}: {erro.Mensagem}");

var vazio = await SessaoDaJornada.AbrirAsync(new RepositorioEmMemoria(), () => hoje);
Console.WriteLine($"✘ {vazio.Erros[0].Campo}: {vazio.Erros[0].Mensagem}");
```

```saida
✘ objetivo: Informe o objetivo da meta.
✘ motivo: Informe por que essa meta importa.
✘ medida: Informe como você vai saber que chegou lá.
✘ arquivo: Ainda não existe um diário. Crie um primeiro.
```

## Passo 6 — Testes: o contrato e o cenário

**Testes de contrato** (Lei 16): um conjunto para **todas** as implementações do repositório.

```cs JornadaTests/Lab7/ContratoDoRepositorioTests.cs
using Jornada.Lab7;
using Xunit;

public abstract class ContratoDoRepositorio
{
    protected abstract IRepositorioDoDiario Criar();

    private static EstadoDoDiario Exemplo() => new()
    {
        Objetivo = "Aprender LINQ", Motivo = "Consultar dados com clareza", Medida = "10 exercícios",
        Prazo = new DateOnly(2027, 1, 31),
        Passos = { new PassoSalvo { Descricao = "Ler", Prazo = new DateOnly(2026, 12, 1), Concluido = true } },
        Dias = { new DateOnly(2026, 11, 2) },
    };

    [Fact]
    public async Task Sem_diario_salvo_a_falha_explica_o_motivo()
    {
        var r = await Criar().CarregarAsync();
        Assert.False(r.Sucesso);
        Assert.Equal("arquivo", r.Erros[0].Campo);
    }

    [Fact]
    public async Task Salvar_e_carregar_preserva_os_dados()
    {
        var repo = Criar();
        await repo.SalvarAsync(Exemplo());

        var r = await repo.CarregarAsync();
        Assert.True(r.Sucesso);
        Assert.Equal("Aprender LINQ", r.Valor.Objetivo);
        Assert.Single(r.Valor.Passos);
        Assert.True(r.Valor.Passos[0].Concluido);
        Assert.Equal(new DateOnly(2026, 11, 2), r.Valor.Dias[0]);
    }

    [Fact]
    public async Task Salvar_sobrescreve_o_diario_anterior()
    {
        var repo = Criar();
        await repo.SalvarAsync(Exemplo());

        var outro = Exemplo();
        outro.Objetivo = "Aprender testes";
        await repo.SalvarAsync(outro);

        Assert.Equal("Aprender testes", (await repo.CarregarAsync()).Valor.Objetivo);
    }
}

public class RepositorioEmMemoriaTests : ContratoDoRepositorio
{
    protected override IRepositorioDoDiario Criar() => new RepositorioEmMemoria();
}

public class RepositorioEmArquivoTests : ContratoDoRepositorio
{
    protected override IRepositorioDoDiario Criar() =>
        new RepositorioEmArquivo(Path.Combine(Path.GetTempPath(), $"diario-{Guid.NewGuid():N}.json"));
}
```

**Teste de cenário** (Leis 1, 3, 7 e 12 juntas):

```cs JornadaTests/Lab7/SessaoDaJornadaTests.cs
using Jornada.Lab7;
using Jornada.Lei12;
using Xunit;

public class SessaoDaJornadaTests
{
    private static readonly DateOnly Hoje = new(2026, 11, 3);

    private static async Task<SessaoDaJornada> NovaSessaoAsync()
    {
        var repo = new RepositorioEmMemoria();
        await repo.SalvarAsync(new EstadoDoDiario
        {
            Objetivo = "Meta", Motivo = "Motivo", Medida = "Medida", Prazo = new DateOnly(2027, 1, 31),
            Passos =
            {
                new PassoSalvo { Descricao = "A", Prazo = new DateOnly(2026, 12, 1) },
                new PassoSalvo { Descricao = "B", Prazo = new DateOnly(2026, 12, 2) },
                new PassoSalvo { Descricao = "C", Prazo = new DateOnly(2026, 12, 3) },
                new PassoSalvo { Descricao = "D", Prazo = new DateOnly(2026, 12, 4) },
            },
        });
        return (await SessaoDaJornada.AbrirAsync(repo, () => Hoje)).Valor;
    }

    [Fact]
    public async Task Concluir_dispara_cada_marco_uma_unica_vez()
    {
        var sessao = await NovaSessaoAsync();
        var marcos = new List<int>();
        sessao.Marco += (_, e) => marcos.Add(e.Percentual);

        foreach (var passo in new[] { "A", "B", "C", "D" }) sessao.Concluir(passo);
        sessao.Concluir("D");                                  // repetido: não conta de novo

        Assert.Equal(new[] { 25, 50, 75, 100 }, marcos);
    }

    [Fact]
    public async Task Terceiro_passo_em_andamento_e_recusado()
    {
        var sessao = await NovaSessaoAsync();
        sessao.Iniciar("A");
        sessao.Iniciar("B");

        Assert.Throws<LimiteDeTrabalhoException>(() => sessao.Iniciar("C"));
    }

    [Fact]
    public async Task Concluir_registra_o_dia_e_conta_na_sequencia()
    {
        var sessao = await NovaSessaoAsync();
        Assert.Equal(0, sessao.SequenciaAtual);

        sessao.Concluir("A");

        Assert.Equal(1, sessao.SequenciaAtual);
    }

    [Fact]
    public async Task Passo_inexistente_gera_mensagem_clara()
    {
        var sessao = await NovaSessaoAsync();
        var e = Assert.Throws<KeyNotFoundException>(() => sessao.Concluir("Z"));
        Assert.Contains("'Z'", e.Message);
    }
}
```

## Passo 7 — Onde estão as 16 leis?

Percorra o projeto com esta tabela e confira se **você consegue apontar** cada lei:

| Lei | Onde aparece no Diário |
|---|---|
| **1 · Propósito** | `Meta` (o quê, por quê, como medir, até quando); `PlanoDeAcao` e o *próximo passo* único |
| **2 · Mente Mestra** | `IRepositorioDoDiario` + duas implementações; sessão que **compõe** módulos; DI na *composition root* |
| **3 · Autoconfiança** | Testes de contrato e de cenário; idempotência de `Concluir` |
| **4 · Economia** | Troca atômica (sem gravar duas vezes); reaproveitamento de módulos existentes |
| **5 · Iniciativa** | Tudo é `async`; a sessão toma a iniciativa de **registrar o dia** ao concluir |
| **6 · Imaginação** | `PlanoDeAcao` fluente; o diário nasceu como "o uso que eu queria ter" |
| **7 · Entusiasmo** | `Marco` (25/50/75/100%) e a **sequência** de dias |
| **8 · Autocontrole** | Retentativa só para `IOException`; `Resultado` em vez de exceção para dados inválidos |
| **9 · Além do exigido** | Validação completa, mensagens claras, documentação do contrato |
| **10 · Personalidade agradável** | Mensagens de erro que dizem o que fazer; todos os erros de uma vez |
| **11 · Precisão** | `DateOnly`, estado com tipos corretos, reidratação consciente |
| **12 · Concentração** | `QuadroKanban` com no máximo 2 passos em andamento |
| **13 · Cooperação** | `CancellationToken` em toda a cadeia; troca atômica cooperando com o sistema de arquivos |
| **14 · Fracassos** | `Resultado<T>`; falhas de arquivo viram dados (e, no próximo passo, métricas) |
| **15 · Tolerância** | Leitura de maiúsculas, texto-número, comentários e vírgula final; gravação canônica |
| **16 · Regra de Ouro** | Contrato honesto e testado; troca atômica protege os dados do usuário |

::: exercicio Desafios para evoluir
Escolha um e implemente, **começando pelo teste**:

1. **Persistir o quadro:** guarde no arquivo quais passos estão "em andamento", para que reabrir a sessão não "esqueça" o foco. (Qual propriedade nova no `EstadoDoDiario`? Como manter o arquivo antigo compatível — Lei 15?)
2. **Métricas:** conte passos concluídos e falhas ao salvar com `Meter` (cap. 55) e exponha a **taxa de falha**.
3. **Notificação:** assine `Marco` e envie uma mensagem (um `INotificador` com dois destinos: console e arquivo). Qual padrão você reconhece? (Dica: Observer + Decorator.)
4. **Comando de console:** crie um `Program` com os comandos `status`, `iniciar <passo>`, `concluir <passo>`, que usa `RepositorioEmArquivo`, trata `Ctrl+C` via `CancellationTokenSource` e **encerra com elegância** (Lei 13).
5. **Privacidade:** acrescente um campo opcional `TokenDeSincronizacao` do tipo `Segredo` (cap. 57) e garanta que ele **nunca** apareça em logs nem no JSON gravado em claro.

--- resposta
Sugestão para o **desafio 1** (persistir o quadro), mantendo compatibilidade:

```cs EstadoDoDiario.cs (trecho)
// EstadoDoDiario: nova propriedade OPCIONAL — arquivos antigos continuam válidos (Lei 15).
public List<string> EmAndamento { get; set; } = new();

// SessaoDaJornada.Estado(): grava quais passos estão em andamento.
EmAndamento = quadro.Em(Coluna.EmAndamento).ToList(),

// SessaoDaJornada (construtor): restaura o foco depois de reconstruir o plano.
foreach (var passo in estado.EmAndamento.Where(p => estado.Passos.Any(x => x.Descricao == p && !x.Concluido)))
    quadro.Mover(passo, Coluna.EmAndamento);
```
Teste primeiro: abra uma sessão, `Iniciar("A")`, salve, **reabra** e verifique que `Iniciar("B")` funciona e `Iniciar("C")` lança `LimiteDeTrabalhoException` (já existem 2 em andamento). Repare que o **arquivo antigo** (sem `emAndamento`) continua carregando: a lista vem vazia por padrão — compatibilidade futura na prática.
:::

## O que você praticou

Mais do que um aplicativo, você construiu um **sistema de hábitos em código**: metas claras, ritmo sustentável, comemoração de progresso, foco, leitura tolerante, gravação cuidadosa, erros que ensinam e contratos que se verificam. As **16 leis** deixaram de ser frases — viraram **estrutura**.

::: resumo
- O **Diário da Jornada** reúne módulos de vários capítulos em um núcleo **testável** (interface de repositório + contrato + sessão).
- **Leitura tolerante** + validação com `Resultado<T>`; **gravação atômica** com retentativa e cancelamento.
- **Contrato do repositório** garantido por testes herdados por todas as implementações.
- **Teste de cenário** cobre meta, marcos, limite de WIP e sequência.
- Cada lei é **localizável** no código; desafios ampliam o projeto (quadro persistido, métricas, notificações, CLI, privacidade).
:::

::: quiz
? Por que a sessão depende de IRepositorioDoDiario e não de uma classe concreta?
+ Para poder trocar o armazenamento (memória, arquivo, banco) e testar sem o disco
- Para deixar o código mais lento
- Porque o C# exige interfaces
- Para evitar o uso de DI
> Dependência de abstração (DIP).

? Para que serve a troca atômica (arquivo temporário + Move)?
+ Impedir que um erro no meio da gravação deixe o arquivo truncado
- Aumentar a velocidade de escrita
- Criptografar o arquivo
- Evitar o uso de async
> O arquivo final é o antigo ou o novo, nunca um pedaço.

? Por que a leitura devolve Resultado<T> em vez de lançar exceção para dados inválidos?
+ Porque dados inválidos são uma falha esperada do domínio e queremos devolver todos os erros de uma vez
- Porque exceções são proibidas
- Porque Resultado é mais rápido
- Porque o JSON não gera exceções
> Falhas esperadas viram valores (Lei 14).

? O que garante que RepositorioEmMemoria e RepositorioEmArquivo se comportam igualmente?
+ Os testes de contrato herdados por ambos
- O compilador
- A documentação apenas
- A sorte
> Um conjunto de testes, várias implementações.

? Por que Concluir é idempotente?
+ Para que concluir duas vezes o mesmo passo não distorça o progresso nem os marcos
- Para economizar memória
- Para simplificar o código
- Porque o quadro exige
> Operações repetidas devem ser seguras.

? Qual lei se reflete no limite de 2 passos em andamento?
+ Lei 12 — Concentração
- Lei 4 — Economia
- Lei 10 — Personalidade agradável
- Lei 16 — Regra de Ouro
> Termine antes de começar.
:::

::: cartas
Diário da Jornada :: Aplicativo do laboratório que junta as 16 leis em um projeto.
Reidratação :: Reconstruir objetos a partir do estado salvo, confiando no que já foi validado.
Troca atômica :: Gravar em arquivo temporário e substituir o final de uma vez.
Teste de contrato :: Testes que toda implementação de uma interface deve passar.
Composition root :: Ponto único onde se escolhem e montam as dependências.
Idempotência :: Repetir a operação não muda o resultado além da primeira vez.
DTO :: Objeto simples usado para transportar/gravar dados.
Teste de cenário :: Teste que exercita várias partes juntas, contando uma história de uso.
:::
