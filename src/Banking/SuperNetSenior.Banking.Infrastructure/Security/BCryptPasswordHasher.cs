using SuperNetSenior.Banking.Application.Abstractions;

namespace SuperNetSenior.Banking.Infrastructure.Security;

public sealed class BCryptPasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    // O BCrypt já embute um salt aleatório no próprio hash. O BankMore original guardava um "salt"
    // extra concatenado à senha, que só adicionava complexidade sem ganho de segurança.
    public string Hash(string senha) => BCrypt.Net.BCrypt.HashPassword(senha, WorkFactor);

    public bool Verify(string senha, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(senha, hash);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            return false;
        }
    }
}
