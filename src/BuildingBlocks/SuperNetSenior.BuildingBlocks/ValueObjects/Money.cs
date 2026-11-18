using System.Globalization;
using SuperNetSenior.BuildingBlocks.Results;

namespace SuperNetSenior.BuildingBlocks.ValueObjects;

/// <summary>Valor monetário não negativo com no máximo 2 casas decimais.</summary>
public sealed record Money
{
    private static readonly CultureInfo PtBr = CultureInfo.GetCultureInfo("pt-BR");

    public decimal Amount { get; }

    private Money(decimal amount) => Amount = amount;

    public static Money Zero { get; } = new(0m);

    public static Result<Money> Create(decimal amount)
    {
        if (amount < 0)
            return Result.Failure<Money>(Error.Validation("Money.Negative", "O valor não pode ser negativo."));

        if (decimal.Round(amount, 2) != amount)
            return Result.Failure<Money>(Error.Validation("Money.Precision", "O valor aceita no máximo 2 casas decimais."));

        return Result.Success(new Money(amount));
    }

    public Money Add(Money other) => new(Amount + other.Amount);

    public Result<Money> Subtract(Money other) =>
        other.Amount > Amount
            ? Result.Failure<Money>(Error.Validation("Money.Insufficient", "Saldo insuficiente."))
            : Result.Success(new Money(Amount - other.Amount));

    public static implicit operator decimal(Money money) => money.Amount;

    public override string ToString() => Amount.ToString("C", PtBr);
}
