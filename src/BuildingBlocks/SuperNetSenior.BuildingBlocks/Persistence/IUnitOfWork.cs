namespace SuperNetSenior.BuildingBlocks.Persistence;

/// <summary>
/// Os repositórios apenas rastreiam mudanças; um único <see cref="CommitAsync"/> grava tudo
/// numa só transação — é isso que torna operações como transferência atômicas.
/// </summary>
public interface IUnitOfWork
{
    Task<int> CommitAsync(CancellationToken cancellationToken = default);
}
