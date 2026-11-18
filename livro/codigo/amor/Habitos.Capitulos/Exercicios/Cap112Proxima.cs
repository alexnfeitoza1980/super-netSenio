namespace Jornada.Habitos.Cap112;

public static class CalendarioExtensoes
{
    /// <summary>A próxima vez, depois de uma data, em que a prática acontece (olhando uma semana à frente).</summary>
    public static Ocorrencia? ProximaOcorrencia(this CalendarioDoTime calendario, string nomeDaPratica, DateOnly depoisDe) =>
        calendario
            .Ocorrencias(depoisDe.AddDays(1), depoisDe.AddDays(7))
            .FirstOrDefault(o => o.Pratica.Nome == nomeDaPratica);
}
