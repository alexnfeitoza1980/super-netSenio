using SuperNetSenior.BuildingBlocks.Results;

namespace SuperNetSenior.BuildingBlocks.ValueObjects;

public sealed record Email
{
    public const int MaxLength = 120;

    public string Value { get; }

    private Email(string value) => Value = value;

    public static Result<Email> Create(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return Result.Failure<Email>(Error.Validation("Email.Empty", "E-mail é obrigatório."));

        var email = raw.Trim().ToLowerInvariant();

        if (email.Length > MaxLength)
            return Result.Failure<Email>(Error.Validation("Email.TooLong", $"E-mail não pode exceder {MaxLength} caracteres."));

        var partes = email.Split('@');
        var valido = partes.Length == 2
            && partes[0].Length > 0
            && partes[1].Contains('.')
            && !partes[1].StartsWith('.')
            && !partes[1].EndsWith('.')
            && !email.Any(char.IsWhiteSpace);

        return valido
            ? Result.Success(new Email(email))
            : Result.Failure<Email>(Error.Validation("Email.Invalid", "E-mail inválido."));
    }

    public static implicit operator string(Email email) => email.Value;
    public override string ToString() => Value;
}
