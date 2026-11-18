using SuperNetSenior.Banking.Domain.Enums;
using SuperNetSenior.BuildingBlocks.Results;
using SuperNetSenior.BuildingBlocks.ValueObjects;

namespace SuperNetSenior.Banking.Domain.Entities;

public class PosicaoInvestimento
{
    public Guid Id { get; private set; }
    public Guid ContaId { get; private set; }
    public string Ativo { get; private set; } = string.Empty; // ex.: CDB Prefixado 12% a.a., Tesouro Selic 2029
    public TipoPosicao Tipo { get; private set; }
    public Money ValorAplicado { get; private set; } = null!;
    public Money SaldoAtual { get; private set; } = null!;
    public DateTime DataAplicacao { get; private set; }

    private PosicaoInvestimento() { }

    private PosicaoInvestimento(Guid id, Guid contaId, string ativo, TipoPosicao tipo, Money valorAplicado, Money saldoAtual, DateTime dataAplicacao)
    {
        Id = id;
        ContaId = contaId;
        Ativo = ativo;
        Tipo = tipo;
        ValorAplicado = valorAplicado;
        SaldoAtual = saldoAtual;
        DataAplicacao = dataAplicacao;
    }

    public static Result<PosicaoInvestimento> Aplicar(Guid contaId, string? ativo, TipoPosicao tipo, decimal valor)
    {
        if (contaId == Guid.Empty)
            return Result.Failure<PosicaoInvestimento>(Error.Validation("Investimento.ContaInvalida", "Conta é obrigatória."));

        if (string.IsNullOrWhiteSpace(ativo))
            return Result.Failure<PosicaoInvestimento>(Error.Validation("Investimento.AtivoObrigatorio", "Nome do ativo é obrigatório."));

        if (valor <= 0)
            return Result.Failure<PosicaoInvestimento>(Error.Validation("Investimento.ValorInvalido", "O valor da aplicação deve ser maior que zero."));

        var money = Money.Create(valor);
        if (money.IsFailure)
            return Result.Failure<PosicaoInvestimento>(money.Error);

        return Result.Success(new PosicaoInvestimento(
            Guid.NewGuid(), contaId, ativo.Trim(), tipo, money.Value, money.Value, DateTime.UtcNow));
    }

    public Result Resgatar(decimal valorResgate)
    {
        if (valorResgate <= 0)
            return Result.Failure(Error.Validation("Investimento.ValorInvalido", "Valor de resgate deve ser maior que zero."));

        var resgate = Money.Create(valorResgate);
        if (resgate.IsFailure)
            return Result.Failure(resgate.Error);

        var novoSaldo = SaldoAtual.Subtract(resgate.Value);
        if (novoSaldo.IsFailure)
            return Result.Failure(Error.Validation("Investimento.SaldoInsuficiente", "Saldo insuficiente para o resgate do investimento."));

        SaldoAtual = novoSaldo.Value;
        return Result.Success();
    }
}
