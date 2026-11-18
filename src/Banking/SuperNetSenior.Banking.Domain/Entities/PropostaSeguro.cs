using SuperNetSenior.Banking.Domain.Enums;
using SuperNetSenior.BuildingBlocks.Results;
using SuperNetSenior.BuildingBlocks.ValueObjects;

namespace SuperNetSenior.Banking.Domain.Entities;

public class PropostaSeguro
{
    public Guid Id { get; private set; }
    public Guid ClienteId { get; private set; }
    public string TipoSeguro { get; private set; } = string.Empty; // Vida, Auto, Residencial
    public Money PremioMensal { get; private set; } = null!;
    public Money CapitalSegurado { get; private set; } = null!;
    public StatusApolice Status { get; private set; }
    public DateTime SolicitadaEm { get; private set; }

    private PropostaSeguro() { }

    private PropostaSeguro(Guid id, Guid clienteId, string tipoSeguro, Money premioMensal, Money capitalSegurado, StatusApolice status, DateTime solicitadaEm)
    {
        Id = id;
        ClienteId = clienteId;
        TipoSeguro = tipoSeguro;
        PremioMensal = premioMensal;
        CapitalSegurado = capitalSegurado;
        Status = status;
        SolicitadaEm = solicitadaEm;
    }

    public static Result<PropostaSeguro> Criar(Guid clienteId, string? tipoSeguro, decimal premioMensal, decimal capitalSegurado)
    {
        if (clienteId == Guid.Empty)
            return Result.Failure<PropostaSeguro>(Error.Validation("Seguro.ClienteInvalido", "Cliente é obrigatório."));

        if (string.IsNullOrWhiteSpace(tipoSeguro))
            return Result.Failure<PropostaSeguro>(Error.Validation("Seguro.TipoObrigatorio", "Tipo de seguro é obrigatório."));

        var premio = Money.Create(premioMensal);
        if (premio.IsFailure) return Result.Failure<PropostaSeguro>(premio.Error);

        var capital = Money.Create(capitalSegurado);
        if (capital.IsFailure) return Result.Failure<PropostaSeguro>(capital.Error);

        return Result.Success(new PropostaSeguro(
            Guid.NewGuid(), clienteId, tipoSeguro.Trim(), premio.Value, capital.Value, StatusApolice.Proposta, DateTime.UtcNow));
    }

    public void EmitirApolice() => Status = StatusApolice.Emitida;
    public void Cancelar() => Status = StatusApolice.Cancelada;
}
