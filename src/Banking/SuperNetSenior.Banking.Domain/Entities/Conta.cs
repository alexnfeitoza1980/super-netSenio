using SuperNetSenior.Banking.Domain.Enums;
using SuperNetSenior.BuildingBlocks.Results;

namespace SuperNetSenior.Banking.Domain.Entities;

public class Conta
{
    public Guid Id { get; private set; }
    public Guid ClienteId { get; private set; }
    public string Numero { get; private set; } = string.Empty;
    public string Agencia { get; private set; } = string.Empty;
    public StatusConta Status { get; private set; }
    public DateTime AbertaEm { get; private set; }
    public decimal Saldo { get; private set; }

    /// <summary>
    /// Token de concorrência otimista: se duas operações alterarem a mesma conta ao mesmo tempo,
    /// a segunda falha em vez de sobrescrever o saldo (evita gastar o mesmo dinheiro duas vezes).
    /// </summary>
    public int Versao { get; private set; }

    private Conta() { }

    private Conta(Guid id, Guid clienteId, string numero, string agencia, StatusConta status, DateTime abertaEm)
    {
        Id = id;
        ClienteId = clienteId;
        Numero = numero;
        Agencia = agencia;
        Status = status;
        AbertaEm = abertaEm;
    }

    public bool Ativa => Status == StatusConta.Ativa;

    public bool PertenceA(Guid clienteId) => clienteId != Guid.Empty && ClienteId == clienteId;

    public static Result<Conta> Abrir(Guid clienteId, string? numero, string agencia = "0001")
    {
        if (clienteId == Guid.Empty)
            return Result.Failure<Conta>(Error.Validation("Conta.ClienteInvalido", "Cliente titular é obrigatório."));

        if (string.IsNullOrWhiteSpace(numero))
            return Result.Failure<Conta>(Error.Validation("Conta.NumeroObrigatorio", "Número da conta é obrigatório."));

        return Result.Success(new Conta(
            Guid.NewGuid(), clienteId, numero.Trim(), agencia.Trim(), StatusConta.Ativa, DateTime.UtcNow));
    }

    public Result Creditar(decimal valor)
    {
        var validacao = ValidarMovimento(valor);
        if (validacao.IsFailure)
            return validacao;

        Saldo += valor;
        Versao++;
        return Result.Success();
    }

    public Result Debitar(decimal valor)
    {
        var validacao = ValidarMovimento(valor);
        if (validacao.IsFailure)
            return validacao;

        if (Saldo < valor)
            return Result.Failure(Error.Validation("Conta.SaldoInsuficiente",
                $"Saldo insuficiente. Saldo atual: {Saldo:N2}, valor solicitado: {valor:N2}."));

        Saldo -= valor;
        Versao++;
        return Result.Success();
    }

    private Result ValidarMovimento(decimal valor)
    {
        if (!Ativa)
            return Result.Failure(Error.Validation("Conta.Inativa", "A conta precisa estar ativa para movimentar."));

        if (valor <= 0)
            return Result.Failure(Error.Validation("Conta.ValorInvalido", "O valor deve ser maior que zero."));

        if (decimal.Round(valor, 2) != valor)
            return Result.Failure(Error.Validation("Conta.ValorPrecisao", "O valor aceita no máximo 2 casas decimais."));

        return Result.Success();
    }

    public void Bloquear() => Status = StatusConta.Bloqueada;
    public void Encerrar() => Status = StatusConta.Encerrada;
}
