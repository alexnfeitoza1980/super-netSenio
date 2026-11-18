using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SuperNetSenior.BuildingBlocks.Messaging;

namespace SuperNetSenior.BuildingBlocks.Infrastructure.Messaging;

/// <summary>Grava o evento no mesmo DbContext do negócio; quem comita é o IUnitOfWork.</summary>
public sealed class EfOutbox<TContext>(TContext context) : IOutbox
    where TContext : DbContext
{
    public void Adicionar(IIntegrationEvent evento)
    {
        var tipo = evento.GetType();
        var mensagem = new OutboxMessage(
            evento.Id,
            tipo.Name,
            JsonSerializer.Serialize(evento, tipo),
            DateTime.UtcNow);

        context.Set<OutboxMessage>().Add(mensagem);
    }
}
