using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SuperNetSenior.BuildingBlocks.Messaging;

namespace SuperNetSenior.BuildingBlocks.Infrastructure.Messaging;

/// <summary>
/// Worker que lê a outbox e publica no <see cref="IEventBus"/>. Entrega "pelo menos uma vez":
/// os consumidores precisam ser idempotentes (use uma Inbox).
/// Genérico: serve a qualquer módulo, bastando informar o DbContext e registrar os tipos de evento.
/// </summary>
public sealed class OutboxDispatcher<TContext>(
    IServiceScopeFactory scopeFactory,
    IntegrationEventRegistry registry,
    ILogger<OutboxDispatcher<TContext>> logger) : BackgroundService
    where TContext : DbContext
{
    private const int TamanhoLote = 20;
    private const int MaxTentativas = 5;
    private static readonly TimeSpan Intervalo = TimeSpan.FromSeconds(3);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("[Outbox<{Contexto}>] Monitorando mensagens pendentes.", typeof(TContext).Name);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessarPendentesAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "[Outbox<{Contexto}>] Erro transitório ao varrer a outbox.", typeof(TContext).Name);
            }

            await Task.Delay(Intervalo, stoppingToken).ConfigureAwait(false);
        }
    }

    /// <returns>Quantidade de mensagens publicadas com sucesso.</returns>
    public async Task<int> ProcessarPendentesAsync(CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();
        var bus = scope.ServiceProvider.GetRequiredService<IEventBus>();

        var pendentes = await context.Set<OutboxMessage>()
            .Where(m => m.ProcessadoEm == null && m.Tentativas < MaxTentativas)
            .OrderBy(m => m.CriadoEm)
            .Take(TamanhoLote)
            .ToListAsync(cancellationToken);

        var publicadas = 0;
        foreach (var mensagem in pendentes)
        {
            try
            {
                if (!registry.TryResolve(mensagem.Tipo, out var tipo))
                    throw new InvalidOperationException($"Tipo de evento não registrado: {mensagem.Tipo}.");

                var evento = (IIntegrationEvent?)JsonSerializer.Deserialize(mensagem.Payload, tipo)
                    ?? throw new JsonException("O payload da mensagem está vazio.");

                await bus.PublicarAsync(evento, cancellationToken);
                mensagem.MarcarComoProcessada();
                publicadas++;
            }
            catch (Exception ex) when (!cancellationToken.IsCancellationRequested)
            {
                logger.LogError(ex, "[Outbox<{Contexto}>] Falha ao despachar {Id}.", typeof(TContext).Name, mensagem.Id);
                mensagem.RegistrarFalha(ex.Message);
            }

            await context.SaveChangesAsync(cancellationToken);
        }

        return publicadas;
    }
}
