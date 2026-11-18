namespace Jornada.Habitos.Cap106;

public static class BussolaExtensoes
{
    /// <summary>As iniciativas que mais apontam para o rumo; no empate, a ordem alfabética.</summary>
    public static IReadOnlyList<Iniciativa> MaisAlinhadas(this Bussola bussola, IEnumerable<Iniciativa> iniciativas, int quantas) =>
        iniciativas
            .OrderByDescending(bussola.Alinhamento)
            .ThenBy(i => i.Nome)
            .Take(quantas)
            .ToList();
}
