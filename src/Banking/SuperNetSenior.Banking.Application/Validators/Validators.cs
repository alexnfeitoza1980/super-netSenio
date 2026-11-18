using FluentValidation;
using FluentValidation.Results;
using SuperNetSenior.Banking.Application.DTOs;
using SuperNetSenior.Banking.Domain.Enums;
using SuperNetSenior.BuildingBlocks.Results;

namespace SuperNetSenior.Banking.Application.Validators;

internal static class ValidationExtensions
{
    public static Result ToResult(this ValidationResult resultado)
    {
        if (resultado.IsValid)
            return Result.Success();

        var erro = resultado.Errors[0];
        return Result.Failure(Error.Validation(erro.ErrorCode, erro.ErrorMessage));
    }

    public static IRuleBuilderOptions<T, decimal> ValorMonetario<T>(this IRuleBuilder<T, decimal> rule, string nome) =>
        rule.GreaterThan(0).WithMessage($"{nome} deve ser maior que zero.")
            .Must(v => decimal.Round(v, 2) == v).WithMessage($"{nome} aceita no máximo 2 casas decimais.");
}

public sealed class RegistrarUsuarioRequestValidator : AbstractValidator<RegistrarUsuarioRequest>
{
    public RegistrarUsuarioRequestValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().WithMessage("Nome é obrigatório.")
            .MaximumLength(150).WithMessage("Nome não pode exceder 150 caracteres.");
        RuleFor(x => x.Email).NotEmpty().WithMessage("E-mail é obrigatório.");
        RuleFor(x => x.Cpf).NotEmpty().WithMessage("CPF é obrigatório.");
        RuleFor(x => x.Senha).NotEmpty().WithMessage("Senha é obrigatória.")
            .MinimumLength(8).WithMessage("A senha deve ter pelo menos 8 caracteres.")
            .MaximumLength(72).WithMessage("A senha não pode exceder 72 caracteres.");
    }
}

public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("E-mail é obrigatório.");
        RuleFor(x => x.Senha).NotEmpty().WithMessage("Senha é obrigatória.");
    }
}

public sealed class AbrirContaRequestValidator : AbstractValidator<AbrirContaRequest>
{
    public AbrirContaRequestValidator()
    {
        RuleFor(x => x.Numero).NotEmpty().WithMessage("Número da conta é obrigatório.")
            .MaximumLength(20).WithMessage("Número da conta não pode exceder 20 caracteres.");
        RuleFor(x => x.Agencia).NotEmpty().WithMessage("Agência é obrigatória.")
            .MaximumLength(10).WithMessage("Agência não pode exceder 10 caracteres.");
    }
}

public sealed class MovimentarContaRequestValidator : AbstractValidator<MovimentarContaRequest>
{
    public MovimentarContaRequestValidator()
    {
        RuleFor(x => x.ContaId).NotEmpty().WithMessage("Conta é obrigatória.");
        RuleFor(x => x.Tipo)
            .Must(t => t is TipoLancamento.Credito or TipoLancamento.Debito)
            .WithMessage("Tipo inválido. Utilize Credito ou Debito.");
        RuleFor(x => x.Valor).ValorMonetario("O valor da operação");
        RuleFor(x => x.ChaveIdempotencia).NotEmpty().WithMessage("Chave de idempotência é obrigatória.")
            .MaximumLength(150).WithMessage("Chave de idempotência deve ter no máximo 150 caracteres.");
        RuleFor(x => x.Descricao).MaximumLength(200).WithMessage("Descrição não pode exceder 200 caracteres.");
    }
}

public sealed class TransferirRequestValidator : AbstractValidator<TransferirRequest>
{
    public TransferirRequestValidator()
    {
        RuleFor(x => x.ContaOrigemId).NotEmpty().WithMessage("Conta de origem é obrigatória.");
        RuleFor(x => x.ContaDestinoId).NotEmpty().WithMessage("Conta de destino é obrigatória.")
            .NotEqual(x => x.ContaOrigemId).WithMessage("Conta de origem e destino devem ser diferentes.");
        RuleFor(x => x.Valor).ValorMonetario("O valor da transferência");
        RuleFor(x => x.ChaveIdempotencia).NotEmpty().WithMessage("Chave de idempotência é obrigatória.")
            .MaximumLength(150).WithMessage("Chave de idempotência deve ter no máximo 150 caracteres.");
    }
}

public sealed class AgendarPagamentoPixRequestValidator : AbstractValidator<AgendarPagamentoPixRequest>
{
    public AgendarPagamentoPixRequestValidator()
    {
        RuleFor(x => x.ContaOrigemId).NotEmpty().WithMessage("Conta de origem é obrigatória.");
        RuleFor(x => x.Valor).ValorMonetario("O valor do pagamento");
        RuleFor(x => x.ChavePix).NotEmpty().WithMessage("Chave Pix ou identificador é obrigatório.")
            .MaximumLength(150).WithMessage("Chave Pix não pode exceder 150 caracteres.");
    }
}

public sealed class ResgatarInvestimentoRequestValidator : AbstractValidator<ResgatarInvestimentoRequest>
{
    public ResgatarInvestimentoRequestValidator()
    {
        RuleFor(x => x.PosicaoInvestimentoId).NotEmpty().WithMessage("Posição de investimento é obrigatória.");
        RuleFor(x => x.ValorResgate).ValorMonetario("O valor do resgate");
    }
}
