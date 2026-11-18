namespace Jornada.Habitos.Cap108;

public static class RelatorioExtensoes
{
    /// <summary>A prioridade cuja posição declarada mais difere da observada. Se tudo caminha junto, não há descompasso (null).</summary>
    public static Comparacao? MaiorDescompasso(this Relatorio relatorio)
    {
        var candidata = relatorio.Declaradas.MaxBy(c => Math.Abs(c.Diferenca));
        return candidata is { Diferenca: not 0 } ? candidata : null;
    }
}
