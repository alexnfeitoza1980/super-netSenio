using SuperNetSenior.Banking.Application.DTOs;
using SuperNetSenior.Banking.Domain.Entities;
using SuperNetSenior.BuildingBlocks.Results;
using SuperNetSenior.BuildingBlocks.Security;

namespace SuperNetSenior.Banking.Application.UseCases;

/// <summary>Regras de acesso compartilhadas pelos use cases: "quem é você" e "esta conta é sua?".</summary>
internal static class Guard
{
    public static Result<Guid> ClienteAutenticado(ICurrentUser usuario) =>
        usuario.EstaAutenticado && usuario.ClienteId is { } id && id != Guid.Empty
            ? Result.Success(id)
            : Result.Failure<Guid>(Error.Unauthorized("Auth.NaoAutenticado", "Usuário não autenticado."));

    public static Result<Conta> ContaDoCliente(Conta? conta, Guid clienteId)
    {
        if (conta is null)
            return Result.Failure<Conta>(Error.NotFound("Conta.NaoEncontrada", "Conta não encontrada."));

        return conta.PertenceA(clienteId)
            ? Result.Success(conta)
            : Result.Failure<Conta>(Error.Forbidden("Conta.AcessoNegado", "A conta informada não pertence ao usuário autenticado."));
    }
}

internal static class Mapping
{
    public static ContaDto ToDto(this Conta c) =>
        new(c.Id, c.ClienteId, c.Numero, c.Agencia, c.Status, c.Saldo, c.AbertaEm);

    public static TransferenciaDto ToDto(this Transferencia t) =>
        new(t.Id, t.ContaOrigemId, t.ContaDestinoId, t.Valor.Amount, t.ChaveIdempotencia, t.CriadaEm);
}
