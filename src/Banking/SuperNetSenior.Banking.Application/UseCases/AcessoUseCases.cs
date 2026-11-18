using FluentValidation;
using SuperNetSenior.Banking.Application.Abstractions;
using SuperNetSenior.Banking.Application.DTOs;
using SuperNetSenior.Banking.Application.Validators;
using SuperNetSenior.Banking.Domain.Entities;
using SuperNetSenior.BuildingBlocks.Persistence;
using SuperNetSenior.BuildingBlocks.Results;
using SuperNetSenior.BuildingBlocks.ValueObjects;

namespace SuperNetSenior.Banking.Application.UseCases;

public interface IRegistrarUsuarioUseCase
{
    Task<Result<RegistrarUsuarioResponse>> ExecutarAsync(RegistrarUsuarioRequest request, CancellationToken cancellationToken = default);
}

/// <summary>Cria o titular (Cliente) e a credencial (Usuario) na mesma transação.</summary>
public sealed class RegistrarUsuarioUseCase(
    IClienteRepository clientes,
    IUsuarioRepository usuarios,
    IPasswordHasher hasher,
    IUnitOfWork unitOfWork,
    IValidator<RegistrarUsuarioRequest> validator) : IRegistrarUsuarioUseCase
{
    public async Task<Result<RegistrarUsuarioResponse>> ExecutarAsync(
        RegistrarUsuarioRequest request, CancellationToken cancellationToken = default)
    {
        var validacao = (await validator.ValidateAsync(request, cancellationToken)).ToResult();
        if (validacao.IsFailure)
            return Result.Failure<RegistrarUsuarioResponse>(validacao.Error);

        var clienteResult = Cliente.Criar(request.Nome, request.Email, request.Cpf);
        if (clienteResult.IsFailure)
            return Result.Failure<RegistrarUsuarioResponse>(clienteResult.Error);

        var cliente = clienteResult.Value;

        if (await clientes.ExisteCpfAsync(cliente.Cpf.Value, cancellationToken))
            return Result.Failure<RegistrarUsuarioResponse>(Error.Conflict("Cliente.CpfExistente", "CPF já cadastrado."));

        if (await usuarios.ExisteEmailAsync(cliente.Email.Value, cancellationToken))
            return Result.Failure<RegistrarUsuarioResponse>(Error.Conflict("Usuario.EmailExistente", "E-mail já cadastrado."));

        var usuarioResult = Usuario.Criar(cliente.Id, cliente.Email, hasher.Hash(request.Senha));
        if (usuarioResult.IsFailure)
            return Result.Failure<RegistrarUsuarioResponse>(usuarioResult.Error);

        await clientes.AdicionarAsync(cliente, cancellationToken);
        await usuarios.AdicionarAsync(usuarioResult.Value, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        return Result.Success(new RegistrarUsuarioResponse(
            usuarioResult.Value.Id, cliente.Id, cliente.Nome, cliente.Email.Value));
    }
}

public interface IAutenticarUseCase
{
    Task<Result<LoginResponse>> ExecutarAsync(LoginRequest request, CancellationToken cancellationToken = default);
}

public sealed class AutenticarUseCase(
    IUsuarioRepository usuarios,
    IPasswordHasher hasher,
    ITokenService tokens,
    IValidator<LoginRequest> validator) : IAutenticarUseCase
{
    // Mensagem única para "usuário inexistente", "inativo" e "senha errada":
    // o original diferenciava os três e permitia descobrir quais e-mails/CPFs estavam cadastrados.
    private static readonly Error CredenciaisInvalidas =
        Error.Unauthorized("Auth.CredenciaisInvalidas", "E-mail ou senha inválidos.");

    public async Task<Result<LoginResponse>> ExecutarAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var validacao = (await validator.ValidateAsync(request, cancellationToken)).ToResult();
        if (validacao.IsFailure)
            return Result.Failure<LoginResponse>(validacao.Error);

        var email = Email.Create(request.Email);
        if (email.IsFailure)
            return Result.Failure<LoginResponse>(CredenciaisInvalidas);

        var usuario = await usuarios.ObterPorEmailAsync(email.Value.Value, cancellationToken);
        if (usuario is null || !usuario.Ativo || !hasher.Verify(request.Senha, usuario.SenhaHash))
            return Result.Failure<LoginResponse>(CredenciaisInvalidas);

        var token = tokens.Gerar(usuario);
        return Result.Success(new LoginResponse(token.Token, token.ExpiraEm, usuario.Id, usuario.ClienteId));
    }
}
