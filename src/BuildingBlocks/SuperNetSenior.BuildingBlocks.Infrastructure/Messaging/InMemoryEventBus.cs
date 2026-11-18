using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using SuperNetSenior.BuildingBlocks.Messaging;

namespace SuperNetSenior.BuildingBlocks.Infrastructure.Messaging;

public sealed class InMemoryEventBus(ILogger<InMemoryEventBus> logger) : IEventBus
{
    private readonly Channel<IIntegrationEvent> _channel = Channel.CreateUnbounded<IIntegrationEvent>();

    public ChannelReader<IIntegrationEvent> Reader => _channel.Reader;

    public async Task PublicarAsync(IIntegrationEvent evento, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("[EventBus] Evento publicado: {Tipo} | Id: {Id}", evento.GetType().Name, evento.Id);
        await _channel.Writer.WriteAsync(evento, cancellationToken);
    }
}
