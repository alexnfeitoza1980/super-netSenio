using FluentValidation;
using SuperNetSenior.Banking.Application.Abstractions;
using SuperNetSenior.Banking.Application.DTOs;
using SuperNetSenior.Banking.Application.Events;
using SuperNetSenior.Banking.Application.Validators;
using SuperNetSenior.Banking.Domain.Entities;
using SuperNetSenior.BuildingBlocks.Messaging;
using SuperNetSenior.BuildingBlocks.Persistence;
using SuperNetSenior.BuildingBlocks.Results;
using SuperNetSenior.BuildingBlocks.Security;

namespace SuperNetSenior.Banking.Application.UseCases;

// ============================ Pix agendado (Outbox) ============================
public interface IAgendarPagamentoPixUseCase
{
    Task<Result<OperacaoPagamentoDto>> ExecutarAsync(AgendarPagamentoPixRequest request, CancellationToken cancellationToken = default);
}

public sealed class AgendarPagamentoPixUseCase(
    IPagamentoRepository pagamentos,
    IContaRepository contas,
    IOutbox outbox,
    ICurrentUser usuarioAtual,
    IUnitOfWork unitOfWork,
    IValidator<AgendarPagamentoPixRequest> validator) : IAgendarPagamentoPixUseCase
{
    public async Task<Result<OperacaoPagamentoDto>> ExecutarAsync(
        AgendarPagamentoPixRequest request, CancellationToken cancellationToken = default)
    {
        var cliente = Guard.ClienteAutenticado(usuarioAtual);
        if (cliente.IsFailure)
            return Result.Failure<OperacaoPagamentoDto>(cliente.Error);

        var validacao = (await validator.ValidateAsync(request, cancellationToken)).ToResult();
        if (validacao.IsFailure)
            return Result.Failure<OperacaoPagamentoDto>(validacao.Error);

        var conta = Guard.ContaDoCliente(await contas.ObterPorIdAsync(request.ContaOrigemId, cancellationToken), cliente.Value);
        if (conta.IsFailure)
            return Result.Failure<OperacaoPagamentoDto>(conta.Error);

        if (!conta.Value.Ativa)
            return Result.Failure<OperacaoPagamentoDto>(Error.Validation(
                "Pagamento.ContaInativa", "A conta de origem precisa estar ativa para efetuar pagamentos."));

        var operacao = OperacaoPagamento.Criar(request.ContaOrigemId, request.Valor, request.ChavePix, request.AgendadoPara);
        if (operacao.IsFailure)
            return Result.Failure<OperacaoPagamentoDto>(operacao.Error);

        var op = operacao.Value;

        // Pagamento + evento na MESMA transação (Transactional Outbox): sem chamada de rede, sem dual-write.
        await pagamentos.AdicionarAsync(op, cancellationToken);
        outbox.Adicionar(new PagamentoPixAgendadoEvent(
            Guid.NewGuid(), op.Id, op.ContaOrigemId, op.Valor.Amount, op.CodigoBarrasOuPix, op.AgendadoPara, DateTime.UtcNow));
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new OperacaoPagamentoDto(
            op.Id, op.ContaOrigemId, op.Valor.Amount, op.CodigoBarrasOuPix, op.Status, op.AgendadoPara, op.SolicitadoEm));
    }
}

// ============================ Resgate de investimento ============================
public interface IResgatarInvestimentoUseCase
{
    Task<Result<PosicaoInvestimentoDto>> ExecutarAsync(ResgatarInvestimentoRequest request, CancellationToken cancellationToken = default);
}

public sealed class ResgatarInvestimentoUseCase(
    IInvestimentoRepository investimentos,
    IContaRepository contas,
    ICurrentUser usuarioAtual,
    IUnitOfWork unitOfWork,
    IValidator<ResgatarInvestimentoRequest> validator) : IResgatarInvestimentoUseCase
{
    public async Task<Result<PosicaoInvestimentoDto>> ExecutarAsync(
        ResgatarInvestimentoRequest request, CancellationToken cancellationToken = default)
    {
        var cliente = Guard.ClienteAutenticado(usuarioAtual);
        if (cliente.IsFailure)
            return Result.Failure<PosicaoInvestimentoDto>(cliente.Error);

        var validacao = (await validator.ValidateAsync(request, cancellationToken)).ToResult();
        if (validacao.IsFailure)
            return Result.Failure<PosicaoInvestimentoDto>(validacao.Error);

        var posicao = await investimentos.ObterPorIdAsync(request.PosicaoInvestimentoId, cancellationToken);
        if (posicao is null)
            return Result.Failure<PosicaoInvestimentoDto>(Error.NotFound(
                "Investimento.NaoEncontrado", "Posição de investimento não encontrada."));

        // A posição só pode ser mexida pelo dono da conta onde ela está.
        var conta = Guard.ContaDoCliente(await contas.ObterPorIdAsync(posicao.ContaId, cancellationToken), cliente.Value);
        if (conta.IsFailure)
            return Result.Failure<PosicaoInvestimentoDto>(conta.Error);

        var resgate = posicao.Resgatar(request.ValorResgate);
        if (resgate.IsFailure)
            return Result.Failure<PosicaoInvestimentoDto>(resgate.Error);

        // O resgate reduz a posição. O crédito do valor na conta ainda NÃO é feito (igual ao monolito original).
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new PosicaoInvestimentoDto(
            posicao.Id, posicao.ContaId, posicao.Ativo, posicao.Tipo,
            posicao.ValorAplicado.Amount, posicao.SaldoAtual.Amount, posicao.DataAplicacao));
    }
}
