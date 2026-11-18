namespace SuperNetSenior.BuildingBlocks.Messaging;

/// <summary>Evento publicado para outros módulos/serviços (sempre via Outbox).</summary>
public interface IIntegrationEvent
{
    Guid Id { get; }
    DateTime OcorridoEm { get; }
}

/// <summary>Barramento de eventos. A implementação padrão é em memória; troque por RabbitMQ/Kafka sem tocar nos módulos.</summary>
public interface IEventBus
{
    Task PublicarAsync(IIntegrationEvent evento, CancellationToken cancellationToken = default);
}

/// <summary>
/// Padrão Transactional Outbox: o evento é gravado na MESMA transação do dado de negócio
/// (não há chamada de rede aqui, então não existe dual-write).
/// </summary>
public interface IOutbox
{
    void Adicionar(IIntegrationEvent evento);
}

/// <summary>Mapeia o nome gravado na tabela outbox de volta para o tipo do evento.</summary>
public sealed class IntegrationEventRegistry
{
    private readonly Dictionary<string, Type> _tipos = new(StringComparer.Ordinal);

    public IntegrationEventRegistry Register<TEvent>() where TEvent : class, IIntegrationEvent
    {
        _tipos[typeof(TEvent).Name] = typeof(TEvent);
        return this;
    }

    public bool TryResolve(string nome, out Type tipo) => _tipos.TryGetValue(nome, out tipo!);
}
