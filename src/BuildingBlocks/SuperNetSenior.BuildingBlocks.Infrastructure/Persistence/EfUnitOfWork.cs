using Microsoft.EntityFrameworkCore;
using SuperNetSenior.BuildingBlocks.Persistence;

namespace SuperNetSenior.BuildingBlocks.Infrastructure.Persistence;

public sealed class EfUnitOfWork<TContext>(TContext context) : IUnitOfWork
    where TContext : DbContext
{
    public Task<int> CommitAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);
}
