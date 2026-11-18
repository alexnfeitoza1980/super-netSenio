namespace Jornada.Habitos.Cap110;

public static class CurvasExtensoes
{
    /// <summary>
    /// Uma pausa (doença, viagem, semana ruim) atrasa o caminho, mas não o apaga:
    /// durante a pausa o "contador de dias" fica parado e depois continua de onde parou.
    /// </summary>
    public static Func<int, double> ComPausa(this Func<int, double> curva, int diaDoInicio, int diasDePausa) =>
        dia => curva(dia switch
        {
            _ when dia < diaDoInicio                 => dia,
            _ when dia < diaDoInicio + diasDePausa   => diaDoInicio,
            _                                        => dia - diasDePausa
        });
}
