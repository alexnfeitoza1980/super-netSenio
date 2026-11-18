using FluentValidation;
using SuperNetSenior.Banking.Application.Abstractions;
using SuperNetSenior.Banking.Application.DTOs;
using SuperNetSenior.Banking.Application.Validators;
using SuperNetSenior.Banking.Domain.Entities;
using SuperNetSenior.Banking.Domain.Enums;
using SuperNetSenior.BuildingBlocks.Persistence;
using SuperNetSenior.BuildingBlocks.Results;
using SuperNetSenior.BuildingBlocks.Security;

namespace SuperNetSenior.Banking.Application.UseCases;

// ============================ Abrir conta ============================
public interface IAbrirContaUseCase
{
    Task<Result<ContaDto>> ExecutarAsync(AbrirContaRequest request, CancellationToken cancellationToken = default);
}

public sealed class AbrirContaUseCase(
    IContaRepository contas,
    ICurrentUser usuarioAtual,
    IUnitOfWork unitOfWork,
    IValidator<AbrirContaRequest> validator) : IAbrirContaUseCase
{
    public async Task<Result<ContaDto>> ExecutarAsync(AbrirContaRequest request, CancellationToken cancellationToken = default)
    {
        var cliente = Guard.ClienteAutenticado(usuarioAtual);
        if (cliente.IsFailure)
            return Result.Failure<ContaDto>(cliente.Error);

        var validacao = (await validator.ValidateAsync(request, cancellationToken)).ToResult();
        if (validacao.IsFailure)
            return Result.Failure<ContaDto>(validacao.Error);

        if (await contas.ExisteNumeroAsync(request.Agencia, request.Numero, cancellationToken))
            return Result.Failure<ContaDto>(Error.Conflict("Conta.NumeroExistente", "Conta já cadastrada para esta agência."));

        var conta = Conta.Abrir(cliente.Value, request.Numero, request.Agencia);
        if (conta.IsFailure)
            return Result.Failure<ContaDto>(conta.Error);

        await contas.AdicionarAsync(conta.Value, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(conta.Value.ToDto());
    }
}

// ============================ Consultar conta / saldo ============================
public interface IConsultarContaUseCase
{
    Task<Result<ContaDto>> ExecutarAsync(Guid contaId, CancellationToken cancellationToken = default);
}

public sealed class ConsultarContaUseCase(IContaRepository contas, ICurrentUser usuarioAtual) : IConsultarContaUseCase
{
    public async Task<Result<ContaDto>> ExecutarAsync(Guid contaId, CancellationToken cancellationToken = default)
    {
        var cliente = Guard.ClienteAutenticado(usuarioAtual);
        if (cliente.IsFailure)
            return Result.Failure<ContaDto>(cliente.Error);

        var conta = Guard.ContaDoCliente(await contas.ObterPorIdAsync(contaId, cancellationToken), cliente.Value);
        return conta.IsFailure
            ? Result.Failure<ContaDto>(conta.Error)
            : Result.Success(conta.Value.ToDto());
    }
}

// ============================ Depósito / saque ============================
public interface IMovimentarContaUseCase
{
    Task<Result<ContaDto>> ExecutarAsync(MovimentarContaRequest request, CancellationToken cancellationToken = default);
}

public sealed class MovimentarContaUseCase(
    IContaRepository contas,
    ILancamentoRepository lancamentos,
    ICurrentUser usuarioAtual,
    IUnitOfWork unitOfWork,
    IValidator<MovimentarContaRequest> validator) : IMovimentarContaUseCase
{
    public async Task<Result<ContaDto>> ExecutarAsync(MovimentarContaRequest request, CancellationToken cancellationToken = default)
    {
        var cliente = Guard.ClienteAutenticado(usuarioAtual);
        if (cliente.IsFailure)
            return Result.Failure<ContaDto>(cliente.Error);

        var validacao = (await validator.ValidateAsync(request, cancellationToken)).ToResult();
        if (validacao.IsFailure)
            return Result.Failure<ContaDto>(validacao.Error);

        var conta = Guard.ContaDoCliente(await contas.ObterPorIdAsync(request.ContaId, cancellationToken), cliente.Value);
        if (conta.IsFailure)
            return Result.Failure<ContaDto>(conta.Error);

        // Reenvio com a mesma chave: não lança de novo, devolve o estado atual da conta.
        if (await lancamentos.ObterPorChaveAsync(conta.Value.Id, request.ChaveIdempotencia, cancellationToken) is not null)
            return Result.Success(conta.Value.ToDto());

        var credito = request.Tipo == TipoLancamento.Credito;
        var movimento = credito ? conta.Value.Creditar(request.Valor) : conta.Value.Debitar(request.Valor);
        if (movimento.IsFailure)
            return Result.Failure<ContaDto>(movimento.Error);

        var lancamento = LancamentoFinanceiro.Criar(
            conta.Value.Id, request.Valor, request.Tipo,
            request.Descricao ?? (credito ? "Depósito" : "Saque"), request.ChaveIdempotencia);
        if (lancamento.IsFailure)
            return Result.Failure<ContaDto>(lancamento.Error);

        await lancamentos.AdicionarAsync(lancamento.Value, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(conta.Value.ToDto());
    }
}

// ============================ Transferência ============================
public interface ITransferirUseCase
{
    Task<Result<TransferenciaDto>> ExecutarAsync(TransferirRequest request, CancellationToken cancellationToken = default);
}

public sealed class TransferirUseCase(
    IContaRepository contas,
    ILancamentoRepository lancamentos,
    ITransferenciaRepository transferencias,
    ICurrentUser usuarioAtual,
    IUnitOfWork unitOfWork,
    IValidator<TransferirRequest> validator) : ITransferirUseCase
{
    public async Task<Result<TransferenciaDto>> ExecutarAsync(TransferirRequest request, CancellationToken cancellationToken = default)
    {
        var cliente = Guard.ClienteAutenticado(usuarioAtual);
        if (cliente.IsFailure)
            return Result.Failure<TransferenciaDto>(cliente.Error);

        var validacao = (await validator.ValidateAsync(request, cancellationToken)).ToResult();
        if (validacao.IsFailure)
            return Result.Failure<TransferenciaDto>(validacao.Error);

        // Só o dono da conta de origem transfere dela.
        var origem = Guard.ContaDoCliente(await contas.ObterPorIdAsync(request.ContaOrigemId, cancellationToken), cliente.Value);
        if (origem.IsFailure)
            return Result.Failure<TransferenciaDto>(origem.Error);

        // Reenvio com a mesma chave devolve a MESMA transferência (200), em vez de erro.
        var existente = await transferencias.ObterPorChaveAsync(origem.Value.Id, request.ChaveIdempotencia, cancellationToken);
        if (existente is not null)
        {
            var mesmaOperacao = existente.ContaDestinoId == request.ContaDestinoId && existente.Valor.Amount == request.Valor;
            return mesmaOperacao
                ? Result.Success(existente.ToDto())
                : Result.Failure<TransferenciaDto>(Error.Conflict(
                    "Transferencia.ChaveReutilizada", "A chave de idempotência já foi usada em outra operação."));
        }

        var destino = await contas.ObterPorIdAsync(request.ContaDestinoId, cancellationToken);
        if (destino is null)
            return Result.Failure<TransferenciaDto>(Error.NotFound("Transferencia.DestinoNaoEncontrado", "Conta de destino não encontrada."));

        // Valida o destino ANTES de mexer no saldo da origem (nada é alterado se algo falhar).
        if (!destino.Ativa)
            return Result.Failure<TransferenciaDto>(Error.Validation("Transferencia.DestinoInativo", "A conta de destino está inativa."));

        var debito = origem.Value.Debitar(request.Valor);
        if (debito.IsFailure)
            return Result.Failure<TransferenciaDto>(debito.Error);

        var credito = destino.Creditar(request.Valor);
        if (credito.IsFailure)
            return Result.Failure<TransferenciaDto>(credito.Error);

        var transferencia = Transferencia.Criar(origem.Value.Id, destino.Id, request.Valor, request.ChaveIdempotencia);
        if (transferencia.IsFailure)
            return Result.Failure<TransferenciaDto>(transferencia.Error);

        // Chaves derivadas: o índice único (conta, chave) barra duplicidade até em requisições simultâneas.
        var lancDebito = LancamentoFinanceiro.Criar(origem.Value.Id, request.Valor, TipoLancamento.Debito,
            "Transferência enviada", $"{request.ChaveIdempotencia}:D");
        var lancCredito = LancamentoFinanceiro.Criar(destino.Id, request.Valor, TipoLancamento.Credito,
            "Transferência recebida", $"{request.ChaveIdempotencia}:C");
        if (lancDebito.IsFailure) return Result.Failure<TransferenciaDto>(lancDebito.Error);
        if (lancCredito.IsFailure) return Result.Failure<TransferenciaDto>(lancCredito.Error);

        await lancamentos.AdicionarAsync(lancDebito.Value, cancellationToken);
        await lancamentos.AdicionarAsync(lancCredito.Value, cancellationToken);
        await transferencias.AdicionarAsync(transferencia.Value, cancellationToken);

        // Um único commit: débito, crédito, lançamentos e transferência gravam juntos ou nada.
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(transferencia.Value.ToDto());
    }
}
