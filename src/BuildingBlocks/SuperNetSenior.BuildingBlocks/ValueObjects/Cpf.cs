using SuperNetSenior.BuildingBlocks.Results;

namespace SuperNetSenior.BuildingBlocks.ValueObjects;

/// <summary>
/// CPF com validação completa dos dígitos verificadores.
/// Unifica as duas versões que existiam (uma só conferia o tamanho; a outra validava tudo mas lançava exceção).
/// </summary>
public sealed record Cpf
{
    public string Value { get; }

    private Cpf(string value) => Value = value;

    /// <summary>CPF no formato 000.000.000-00.</summary>
    public string Formatado => $"{Value[..3]}.{Value[3..6]}.{Value[6..9]}-{Value[9..]}";

    public static Result<Cpf> Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return Result.Failure<Cpf>(Error.Validation("Cpf.Empty", "CPF é obrigatório."));

        var digits = new string(raw.Where(char.IsDigit).ToArray());

        if (digits.Length != 11)
            return Result.Failure<Cpf>(Error.Validation("Cpf.InvalidLength", "CPF deve conter 11 dígitos."));

        if (!DigitosVerificadoresValidos(digits))
            return Result.Failure<Cpf>(Error.Validation("Cpf.Invalid", "CPF inválido."));

        return Result.Success(new Cpf(digits));
    }

    public static bool EhValido(string? raw) => Create(raw).IsSuccess;

    private static bool DigitosVerificadoresValidos(string cpf)
    {
        // 000.000.000-00, 111.111.111-11 etc. passam na conta, mas são inválidos.
        if (cpf.Distinct().Count() == 1)
            return false;

        return cpf[9] - '0' == CalcularDigito(cpf, 9)
            && cpf[10] - '0' == CalcularDigito(cpf, 10);
    }

    private static int CalcularDigito(string cpf, int quantidade)
    {
        var soma = 0;
        for (var i = 0; i < quantidade; i++)
            soma += (cpf[i] - '0') * (quantidade + 1 - i);

        var resto = soma % 11;
        return resto < 2 ? 0 : 11 - resto;
    }

    public static implicit operator string(Cpf cpf) => cpf.Value;
    public override string ToString() => Value;
}
