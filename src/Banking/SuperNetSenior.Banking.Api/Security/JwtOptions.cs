namespace SuperNetSenior.Banking.Api.Security;

public sealed class JwtOptions
{
    public const string Secao = "Jwt";
    public const int TamanhoMinimoChave = 32;

    public string Key { get; init; } = string.Empty;
    public string Issuer { get; init; } = "SuperNetSenior";
    public string Audience { get; init; } = "SuperNetSenior.Clients";
    public int ExpiresInMinutes { get; init; } = 60;
}
