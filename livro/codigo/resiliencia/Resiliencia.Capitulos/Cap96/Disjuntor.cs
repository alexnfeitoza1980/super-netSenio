namespace Jornada.Resiliencia.Cap96;

public enum EstadoDoDisjuntor { Fechado, Aberto, MeioAberto }

public sealed class CircuitoAbertoException : Exception
{
    public CircuitoAbertoException() : base("O serviço está descansando. Tente mais tarde ou use o plano B.") { }
}

/// <summary>Depois de falhas seguidas, para de insistir por um tempo e deixa o serviço respirar.</summary>
public sealed class DisjuntorDeCircuito
{
    private readonly int limiteDeFalhas;
    private readonly TimeSpan descanso;
    private readonly TimeProvider relogio;

    private int falhasSeguidas;
    private DateTimeOffset? abertoEm;

    public DisjuntorDeCircuito(int limiteDeFalhas, TimeSpan descanso, TimeProvider relogio)
    {
        this.limiteDeFalhas = limiteDeFalhas;
        this.descanso = descanso;
        this.relogio = relogio;
    }

    public EstadoDoDisjuntor Estado
    {
        get
        {
            if (abertoEm is null) return EstadoDoDisjuntor.Fechado;
            return relogio.GetUtcNow() - abertoEm.Value >= descanso
                ? EstadoDoDisjuntor.MeioAberto          // passou o descanso: deixa UMA chamada de teste
                : EstadoDoDisjuntor.Aberto;
        }
    }

    public async Task<T> ExecutarAsync<T>(Func<CancellationToken, Task<T>> acao, CancellationToken ct = default)
    {
        var estadoAntes = Estado;
        if (estadoAntes == EstadoDoDisjuntor.Aberto)
            throw new CircuitoAbertoException();

        try
        {
            var resultado = await acao(ct);
            falhasSeguidas = 0;                         // deu certo: o circuito fecha de novo
            abertoEm = null;
            return resultado;
        }
        catch (Exception) when (!ct.IsCancellationRequested)
        {
            falhasSeguidas++;
            if (estadoAntes == EstadoDoDisjuntor.MeioAberto || falhasSeguidas >= limiteDeFalhas)
                abertoEm = relogio.GetUtcNow();         // abre (ou reabre) e começa a contar o descanso
            throw;
        }
    }
}
