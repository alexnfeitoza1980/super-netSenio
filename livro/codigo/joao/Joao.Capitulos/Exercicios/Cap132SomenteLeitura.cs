using Jornada.Joao.Cap132;

namespace Jornada.Joao.Exercicios;

/// <summary>Um decorador que deixa ler, mas recusa qualquer escrita.</summary>
public sealed class SomenteLeitura : IRepositorioDeTarefas
{
    private readonly IRepositorioDeTarefas interno;

    public SomenteLeitura(IRepositorioDeTarefas interno) => this.interno = interno;

    public Task<Tarefa?> ObterAsync(int id, CancellationToken ct = default) => interno.ObterAsync(id, ct);

    public Task SalvarAsync(Tarefa tarefa, CancellationToken ct = default) =>
        throw new InvalidOperationException("Este repositório é somente leitura.");
}
