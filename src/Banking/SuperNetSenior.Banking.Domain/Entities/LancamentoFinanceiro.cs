using SuperNetSenior.Banking.Domain.Enums;
using SuperNetSenior.BuildingBlocks.Results;
using SuperNetSenior.BuildingBlocks.ValueObjects;

namespace SuperNetSenior.Banking.Domain.Entities;

/// <summary>Linha do extrato. A chave de idempotência (única por conta) impede lançar duas vezes a mesma operação.</summary>
public class LancamentoFinanceiro
{
    public Guid Id { get; private set; }
    public Guid ContaId { get; private set; }
    public Money Valor { get; private set; } = null!;
    public TipoLancamento Tipo { get; private set; }
    public string Descricao { get; private set; } = string.Empty;
    public string ChaveIdempotencia { get; private set; } = string.Empty;
    public DateTime RealizadoEm { get; private set; }

    private LancamentoFinanceiro() { }

    private LancamentoFinanceiro(Guid id, Guid contaId, Money valor, TipoLancamento tipo, string descricao, string chave, DateTime realizadoEm)
    {
        Id = id;
        ContaId = contaId;
        Valor = valor;
        Tipo = tipo;
        Descricao = descricao;
        ChaveIdempotencia = chave;
        RealizadoEm = realizadoEm;
    }

    public static Result<LancamentoFinanceiro> Criar(
        Guid contaId, decimal valor, TipoLancamento tipo, string? descricao, string? chaveIdempotencia)
    {
        if (contaId == Guid.Empty)
            return Result.Failure<LancamentoFinanceiro>(Error.Validation("Lancamento.ContaInvalida", "Conta é obrigatória."));

        if (valor <= 0)
            return Result.Failure<LancamentoFinanceiro>(Error.Validation("Lancamento.ValorInvalido", "O valor do lançamento deve ser maior que zero."));

        if (string.IsNullOrWhiteSpace(chaveIdempotencia))
            return Result.Failure<LancamentoFinanceiro>(Error.Validation("Lancamento.ChaveObrigatoria", "Chave de idempotência é obrigatória."));

        var money = Money.Create(valor);
        if (money.IsFailure)
            return Result.Failure<LancamentoFinanceiro>(money.Error);

        return Result.Success(new LancamentoFinanceiro(
            Guid.NewGuid(), contaId, money.Value, tipo, (descricao ?? string.Empty).Trim(),
            chaveIdempotencia.Trim(), DateTime.UtcNow));
    }
}
