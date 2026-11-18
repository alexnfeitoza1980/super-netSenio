namespace Jornada.Resiliencia.Cap88;

public static class Notas
{
    /// <summary>Lê uma nota de 0 a 10 sem lançar exceção. Devolve null quando não dá.</summary>
    public static int? LerNota(string? texto) =>
        int.TryParse(texto, out int nota) && nota is >= 0 and <= 10
            ? nota
            : null;
}
