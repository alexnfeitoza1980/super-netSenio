using System.Collections.Concurrent;
using Jornada.Joao.Cap124;

namespace Jornada.Joao.Cap132;

public sealed record Tarefa(int Id, string Titulo, bool Feita);

public interface IRepositorioDeTarefas
{
    Task<Tarefa?> ObterAsync(int id, CancellationToken ct = default);
    Task SalvarAsync(Tarefa tarefa, CancellationToken ct = default);
}

public sealed class FalhaPassageiraException : Exception
{
    public FalhaPassageiraException(string mensagem) : base(mensagem) { }
}

/// <summary>O serviço de verdade, "no chão": guarda em memória e, quando pedimos, falha algumas vezes antes de funcionar.</summary>
public sealed class RepositorioEmMemoria : IRepositorioDeTarefas
{
    private readonly ConcurrentDictionary<int, Tarefa> dados = new();
    private int falhasPendentes;
    private int leituras;

    public int Leituras => Volatile.Read(ref leituras);

    public void FalharProximas(int vezes) => falhasPendentes = vezes;

    public Task<Tarefa?> ObterAsync(int id, CancellationToken ct = default)
    {
        Interlocked.Increment(ref leituras);
        TentarFalhar();
        return Task.FromResult(dados.TryGetValue(id, out var t) ? t : null);
    }

    public Task SalvarAsync(Tarefa tarefa, CancellationToken ct = default)
    {
        TentarFalhar();
        dados[tarefa.Id] = tarefa;
        return Task.CompletedTask;
    }

    private void TentarFalhar()
    {
        if (falhasPendentes > 0)
        {
            falhasPendentes--;
            throw new FalhaPassageiraException("o serviço está ocupado");
        }
    }
}

/// <summary>
/// Decorador: tem a MESMA interface do que embrulha, faz um serviço extra e repassa o trabalho.
/// Quem usa não sabe se fala com o serviço de verdade ou com uma pilha de decoradores.
/// </summary>
public sealed class ComRegistro : IRepositorioDeTarefas
{
    private readonly IRepositorioDeTarefas interno;
    private readonly ILanterna lanterna;

    public ComRegistro(IRepositorioDeTarefas interno, ILanterna lanterna)
    {
        this.interno = interno;
        this.lanterna = lanterna;
    }

    public async Task<Tarefa?> ObterAsync(int id, CancellationToken ct = default)
    {
        using var _ = lanterna.Escopo($"obter {id}");
        try
        {
            var tarefa = await interno.ObterAsync(id, ct);
            lanterna.Escrever(Nivel.Informacao, tarefa is null ? "não encontrada" : "encontrada");
            return tarefa;
        }
        catch (Exception e)
        {
            lanterna.Escrever(Nivel.Erro, "falhou", ("erro", e.Message));
            throw;
        }
    }

    public async Task SalvarAsync(Tarefa tarefa, CancellationToken ct = default)
    {
        using var _ = lanterna.Escopo($"salvar {tarefa.Id}");
        await interno.SalvarAsync(tarefa, ct);
        lanterna.Escrever(Nivel.Informacao, "salva");
    }
}

/// <summary>Insiste algumas vezes quando a falha é passageira. Humilde: ninguém vê, mas o serviço fica mais firme.</summary>
public sealed class ComTentativas : IRepositorioDeTarefas
{
    private readonly IRepositorioDeTarefas interno;
    private readonly int maximo;

    public ComTentativas(IRepositorioDeTarefas interno, int maximo)
    {
        if (maximo < 1) throw new ArgumentOutOfRangeException(nameof(maximo));
        this.interno = interno;
        this.maximo = maximo;
    }

    public Task<Tarefa?> ObterAsync(int id, CancellationToken ct = default) =>
        Repetir(() => interno.ObterAsync(id, ct));

    public Task SalvarAsync(Tarefa tarefa, CancellationToken ct = default) =>
        Repetir(async () => { await interno.SalvarAsync(tarefa, ct); return 0; });

    private async Task<T> Repetir<T>(Func<Task<T>> acao)
    {
        for (int tentativa = 1; ; tentativa++)
        {
            try { return await acao(); }
            catch (FalhaPassageiraException) when (tentativa < maximo) { }    // filtro: só repete se ainda restam tentativas
        }
    }
}

/// <summary>Guarda as leituras já feitas. Salvar invalida o item, para nunca servir um dado velho.</summary>
public sealed class ComCache : IRepositorioDeTarefas
{
    private readonly IRepositorioDeTarefas interno;
    private readonly ConcurrentDictionary<int, Tarefa> guardadas = new();

    public ComCache(IRepositorioDeTarefas interno) => this.interno = interno;

    public async Task<Tarefa?> ObterAsync(int id, CancellationToken ct = default)
    {
        if (guardadas.TryGetValue(id, out var tarefa))
            return tarefa;

        var lida = await interno.ObterAsync(id, ct);
        if (lida is not null) guardadas[id] = lida;
        return lida;
    }

    public async Task SalvarAsync(Tarefa tarefa, CancellationToken ct = default)
    {
        await interno.SalvarAsync(tarefa, ct);
        guardadas[tarefa.Id] = tarefa;
    }
}

public static class DecoradoresExtensoes
{
    // Métodos de extensão deixam empilhar em leitura natural: repositorio.ComCache().ComTentativas(3).ComRegistro(lanterna)
    public static IRepositorioDeTarefas ComRegistro(this IRepositorioDeTarefas r, ILanterna lanterna) => new ComRegistro(r, lanterna);
    public static IRepositorioDeTarefas ComTentativas(this IRepositorioDeTarefas r, int maximo) => new ComTentativas(r, maximo);
    public static IRepositorioDeTarefas ComCache(this IRepositorioDeTarefas r) => new ComCache(r);
}
