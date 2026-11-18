using SuperNetSenior.BuildingBlocks.Messaging;

namespace SuperNetSenior.Banking.Application.Events;

public sealed record PagamentoPixAgendadoEvent(
    Guid EventId,
    Guid OperacaoId,
    Guid ContaOrigemId,
    decimal Valor,
    string ChavePix,
    DateTime AgendadoPara,
    DateTime OcorridoEm) : IIntegrationEvent
{
    public Guid Id => EventId;
}

public sealed record PagamentoPixLiquidadoEvent(
    Guid EventId,
    Guid OperacaoId,
    Guid ContaOrigemId,
    decimal Valor,
    string ChavePix,
    DateTime LiquidadoEm,
    DateTime OcorridoEm) : IIntegrationEvent
{
    public Guid Id => EventId;
}
