namespace Jornada.Resiliencia.Cap90;

public static class MochilaExtensoes
{
    /// <summary>Uma linha por tipo: "Pessoas: 2 (Beto, Marta)".</summary>
    public static IReadOnlyList<string> Resumo(this Mochila mochila) =>
        mochila.PorTipo()
               .Select(grupo => $"{grupo.Key}: {grupo.Count()} ({string.Join(", ", grupo.Select(r => r.Nome).OrderBy(n => n))})")
               .ToList();
}
