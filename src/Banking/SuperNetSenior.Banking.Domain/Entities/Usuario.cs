using SuperNetSenior.BuildingBlocks.Results;
using SuperNetSenior.BuildingBlocks.ValueObjects;

namespace SuperNetSenior.Banking.Domain.Entities;

/// <summary>
/// Credencial de acesso. O vínculo com o titular é por <see cref="ClienteId"/> (FK real) —
/// no BankMore original o vínculo usuário→conta era feito pelo NOME.
/// </summary>
public class Usuario
{
    public Guid Id { get; private set; }
    public Guid ClienteId { get; private set; }
    public Email Email { get; private set; } = null!;
    public string SenhaHash { get; private set; } = string.Empty;
    public bool Ativo { get; private set; }
    public DateTime CriadoEm { get; private set; }

    private Usuario() { }

    private Usuario(Guid id, Guid clienteId, Email email, string senhaHash)
    {
        Id = id;
        ClienteId = clienteId;
        Email = email;
        SenhaHash = senhaHash;
        Ativo = true;
        CriadoEm = DateTime.UtcNow;
    }

    public static Result<Usuario> Criar(Guid clienteId, Email email, string senhaHash)
    {
        if (clienteId == Guid.Empty)
            return Result.Failure<Usuario>(Error.Validation("Usuario.ClienteInvalido", "Cliente é obrigatório."));

        if (string.IsNullOrWhiteSpace(senhaHash))
            return Result.Failure<Usuario>(Error.Validation("Usuario.SenhaObrigatoria", "Senha é obrigatória."));

        return Result.Success(new Usuario(Guid.NewGuid(), clienteId, email, senhaHash));
    }

    public void Inativar() => Ativo = false;
}
