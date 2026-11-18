namespace Quatro;

public static class Regras
{
    public static string Texto(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            throw new ArgumentException("Escreva um texto antes de registrar.");
        return texto.Trim();
    }
}
