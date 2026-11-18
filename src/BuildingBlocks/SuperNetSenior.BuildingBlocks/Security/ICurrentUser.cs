namespace SuperNetSenior.BuildingBlocks.Security;

/// <summary>Usuário autenticado da requisição atual.</summary>
public interface ICurrentUser
{
    bool EstaAutenticado { get; }
    Guid? UsuarioId { get; }

    /// <summary>Cliente (titular) ao qual o usuário pertence. É a base da regra "só o dono opera a conta".</summary>
    Guid? ClienteId { get; }
}
