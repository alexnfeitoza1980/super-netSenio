using SuperNetSenior.Banking.Domain.Enums;
using SuperNetSenior.BuildingBlocks.Results;
using SuperNetSenior.BuildingBlocks.ValueObjects;

namespace SuperNetSenior.Banking.Domain.Entities;

public class OperacaoPagamento
{
    public Guid Id { get; private set; }
    public Guid ContaOrigemId { get; private set; }
    public Money Valor { get; private set; } = null!;
    public string CodigoBarrasOuPix { get; private set; } = string.Empty;
    public StatusOperacao Status { get; private set; }
    public DateTime AgendadoPara { get; private set; }
    public DateTime SolicitadoEm { get; private set; }

    private OperacaoPagamento() { }

    private OperacaoPagamento(Guid id, Guid contaOrigemId, Money valor, string codigo, StatusOperacao status, DateTime agendadoPara, DateTime solicitadoEm)
    {
        Id = id;
        ContaOrigemId = contaOrigemId;
        Valor = valor;
        CodigoBarrasOuPix = codigo;
        Status = status;
        AgendadoPara = agendadoPara;
        SolicitadoEm = solicitadoEm;
    }

    public static Result<OperacaoPagamento> Criar(Guid contaOrigemId, decimal valor, string? codigo, DateTime? agendadoPara = null)
    {
        if (contaOrigemId == Guid.Empty)
            return Result.Failure<OperacaoPagamento>(Error.Validation("Pagamento.ContaInvalida", "Conta de origem é obrigatória."));

        if (string.IsNullOrWhiteSpace(codigo))
            return Result.Failure<OperacaoPagamento>(Error.Validation("Pagamento.DestinoInvalido", "Chave Pix ou código de barras é obrigatório."));

        if (valor <= 0)
            return Result.Failure<OperacaoPagamento>(Error.Validation("Pagamento.ValorInvalido", "O valor do pagamento deve ser maior que zero."));

        var money = Money.Create(valor);
        if (money.IsFailure)
            return Result.Failure<OperacaoPagamento>(money.Error);

        return Result.Success(new OperacaoPagamento(
            Guid.NewGuid(), contaOrigemId, money.Value, codigo.Trim(), StatusOperacao.Agendado,
            agendadoPara ?? DateTime.UtcNow, DateTime.UtcNow));
    }

    public void Liquidar() => Status = StatusOperacao.Liquidado;
    public void Falhar() => Status = StatusOperacao.Falha;
    public void Cancelar() => Status = StatusOperacao.Cancelado;
}
