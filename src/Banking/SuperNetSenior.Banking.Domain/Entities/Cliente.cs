using SuperNetSenior.Banking.Domain.Enums;
using SuperNetSenior.BuildingBlocks.Results;
using SuperNetSenior.BuildingBlocks.ValueObjects;

namespace SuperNetSenior.Banking.Domain.Entities;

public class Cliente
{
    public Guid Id { get; private set; }
    public string Nome { get; private set; } = string.Empty;
    public Email Email { get; private set; } = null!;
    public Cpf Cpf { get; private set; } = null!;
    public SituacaoCadastral Situacao { get; private set; }
    public DateTime CriadoEm { get; private set; }

    private Cliente() { }

    private Cliente(Guid id, string nome, Email email, Cpf cpf, SituacaoCadastral situacao, DateTime criadoEm)
    {
        Id = id;
        Nome = nome;
        Email = email;
        Cpf = cpf;
        Situacao = situacao;
        CriadoEm = criadoEm;
    }

    public static Result<Cliente> Criar(string? nome, string? email, string? cpf)
    {
        if (string.IsNullOrWhiteSpace(nome))
            return Result.Failure<Cliente>(Error.Validation("Cliente.NomeVazio", "Nome do cliente é obrigatório."));

        var emailResult = Email.Create(email);
        if (emailResult.IsFailure)
            return Result.Failure<Cliente>(emailResult.Error);

        var cpfResult = Cpf.Create(cpf);
        if (cpfResult.IsFailure)
            return Result.Failure<Cliente>(cpfResult.Error);

        return Result.Success(new Cliente(
            Guid.NewGuid(), nome.Trim(), emailResult.Value, cpfResult.Value, SituacaoCadastral.Ativo, DateTime.UtcNow));
    }

    public void Bloquear() => Situacao = SituacaoCadastral.Bloqueado;
    public void Ativar() => Situacao = SituacaoCadastral.Ativo;
}
