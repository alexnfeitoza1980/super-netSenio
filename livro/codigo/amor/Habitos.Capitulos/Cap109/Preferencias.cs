namespace Jornada.Habitos.Cap109;

/// <summary>
/// Os valores PADRÃO ensinam. Quase ninguém muda as configurações iniciais, então o padrão é uma decisão
/// de design com consequência moral. Aqui, o padrão cuida de quem usa.
/// </summary>
public sealed class PreferenciasDeNotificacao
{
    /// <summary>"required": quem cria o objeto é obrigado a dizer o nome. O compilador cobra.</summary>
    public required string Produto { get; init; }

    public bool NotificacoesAtivas { get; init; } = false;

    public TimeSpan InicioDoSilencio { get; init; } = TimeSpan.FromHours(21);

    public TimeSpan FimDoSilencio { get; init; } = TimeSpan.FromHours(8);

    public int MaximoPorDia { get; init; } = 3;

    public bool EstaNoSilencio(TimeSpan horaDoDia) =>
        InicioDoSilencio <= FimDoSilencio
            ? horaDoDia >= InicioDoSilencio && horaDoDia < FimDoSilencio
            : horaDoDia >= InicioDoSilencio || horaDoDia < FimDoSilencio;     // atravessa a meia-noite

    public bool PodeNotificar(TimeSpan horaDoDia, int jaEnviadasHoje) =>
        NotificacoesAtivas && !EstaNoSilencio(horaDoDia) && jaEnviadasHoje < MaximoPorDia;
}
