namespace Jornada.Resiliencia.Cap89;

public sealed class PausaConsciente
{
    private readonly TimeProvider relogio;
    private readonly TimeSpan duracao;
    private DateTimeOffset? inicio;

    public PausaConsciente(TimeProvider relogio, TimeSpan duracao)
    {
        this.relogio = relogio;
        this.duracao = duracao;
    }

    public void Iniciar() => inicio = relogio.GetUtcNow();

    public TimeSpan Restante
    {
        get
        {
            if (inicio is null) return duracao;
            var passou = relogio.GetUtcNow() - inicio.Value;
            return passou >= duracao ? TimeSpan.Zero : duracao - passou;
        }
    }

    public bool Terminou => inicio is not null && Restante == TimeSpan.Zero;
}
