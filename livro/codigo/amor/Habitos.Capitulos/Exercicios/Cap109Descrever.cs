namespace Jornada.Habitos.Cap109;

public static class PadraoExtensoes
{
    /// <summary>"RolagemInfinita + NotificacaoPush", ou "nenhum" quando nada está ligado.</summary>
    public static string Descrever(this PadraoDeDesign padroes)
    {
        var ligados = Enum.GetValues<PadraoDeDesign>()
                          .Where(p => p != PadraoDeDesign.Nenhum && padroes.HasFlag(p))
                          .ToList();

        return ligados.Count == 0 ? "nenhum" : string.Join(" + ", ligados);
    }
}
