using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SuperNetSenior.BuildingBlocks.Infrastructure.Messaging;
using SuperNetSenior.BuildingBlocks.Infrastructure.Persistence;
using SuperNetSenior.BuildingBlocks.Messaging;
using SuperNetSenior.BuildingBlocks.Persistence;

namespace SuperNetSenior.BuildingBlocks.Infrastructure;

public static class DependencyInjection
{
    /// <summary>
    /// Registra Unit of Work, Outbox e o worker que despacha eventos para o módulo cujo DbContext é <typeparamref name="TContext"/>.
    /// </summary>
    public static IServiceCollection AddModuleMessaging<TContext>(
        this IServiceCollection services,
        Action<IntegrationEventRegistry> registrarEventos)
        where TContext : DbContext
    {
        var registry = new IntegrationEventRegistry();
        registrarEventos(registry);
        services.AddSingleton(registry);

        services.AddScoped<IUnitOfWork, EfUnitOfWork<TContext>>();
        services.AddScoped<IOutbox, EfOutbox<TContext>>();

        // Um barramento por processo (em memória por padrão).
        services.AddSingleton<InMemoryEventBus>();
        services.AddSingleton<IEventBus>(sp => sp.GetRequiredService<InMemoryEventBus>());

        services.AddHostedService<OutboxDispatcher<TContext>>();
        return services;
    }
}
