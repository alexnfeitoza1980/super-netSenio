using SuperNetSenior.BuildingBlocks.Results;
using SuperNetSenior.BuildingBlocks.ValueObjects;

namespace SuperNetSenior.Banking.Domain.Entities;

public class Transferencia
{
    public Guid Id { get; private set; }
    public Guid ContaOrigemId { get; private set; }
    public Guid ContaDestinoId { get; private set; }
    public Money Valor { get; private set; } = null!;
    public string ChaveIdempotencia { get; private set; } = string.Empty;
    public DateTime CriadaEm { get; private set; }

    private Transferencia() { }

    private Transferencia(Guid id, Guid origem, Guid destino, Money valor, string chave, DateTime criadaEm)
    {
        Id = id;
        ContaOrigemId = origem;
        ContaDestinoId = destino;
        Valor = valor;
        ChaveIdempotencia = chave;
        CriadaEm = criadaEm;
    }

    public static Result<Transferencia> Criar(Guid origem, Guid destino, decimal valor, string? chaveIdempotencia)
    {
        if (origem == Guid.Empty || destino == Guid.Empty)
            return Result.Failure<Transferencia>(Error.Validation("Transferencia.ContaInvalida", "Conta de origem e de destino são obrigatórias."));

        if (origem == destino)
            return Result.Failure<Transferencia>(Error.Validation("Transferencia.MesmaConta", "Conta de origem e destino devem ser diferentes."));

        if (valor <= 0)
            return Result.Failure<Transferencia>(Error.Validation("Transferencia.ValorInvalido", "O valor da transferência deve ser maior que zero."));

        if (string.IsNullOrWhiteSpace(chaveIdempotencia))
            return Result.Failure<Transferencia>(Error.Validation("Transferencia.ChaveObrigatoria", "Chave de idempotência é obrigatória."));

        var money = Money.Create(valor);
        if (money.IsFailure)
            return Result.Failure<Transferencia>(money.Error);

        return Result.Success(new Transferencia(
            Guid.NewGuid(), origem, destino, money.Value, chaveIdempotencia.Trim(), DateTime.UtcNow));
    }
}
