namespace Jornada.Resiliencia.Cap96;

/// <summary>Falha que costuma passar sozinha (rede instável, serviço ocupado). Só essas merecem nova tentativa.</summary>
public sealed class FalhaPassageiraException : Exception
{
    public FalhaPassageiraException(string mensagem) : base(mensagem) { }
}

public sealed class TentativasEsgotadasException : Exception
{
    public int Tentativas { get; }

    public TentativasEsgotadasException(int tentativas, Exception causa)
        : base($"Desisti depois de {tentativas} tentativas.", causa) => Tentativas = tentativas;
}

public sealed record PoliticaDeTentativas(int MaximoDeTentativas, TimeSpan EsperaInicial, double Fator = 2.0)
{
    /// <summary>Espera antes da próxima tentativa: cresce a cada falha (1 s, 2 s, 4 s, ...).</summary>
    public TimeSpan EsperaApos(int tentativaQueFalhou) =>
        TimeSpan.FromTicks((long)(EsperaInicial.Ticks * Math.Pow(Fator, tentativaQueFalhou - 1)));
}

public static class Tentador
{
    public static async Task<T> ExecutarAsync<T>(
        Func<CancellationToken, Task<T>> acao,
        PoliticaDeTentativas politica,
        Func<TimeSpan, CancellationToken, Task>? esperar = null,
        CancellationToken ct = default)
    {
        esperar ??= Task.Delay;                                 // nos testes, trocamos por uma espera falsa

        for (int tentativa = 1; ; tentativa++)
        {
            try
            {
                return await acao(ct);
            }
            catch (FalhaPassageiraException falha)
            {
                if (tentativa >= politica.MaximoDeTentativas)
                    throw new TentativasEsgotadasException(tentativa, falha);

                await esperar(politica.EsperaApos(tentativa), ct);
            }
            // qualquer outra exceção (um erro de programação, por exemplo) sobe na hora: repetir não adianta
        }
    }

    /// <summary>Dá um limite de tempo à ação. Passou do limite, vira TimeoutException.</summary>
    public static async Task<T> ComLimiteAsync<T>(
        Func<CancellationToken, Task<T>> acao, TimeSpan limite, CancellationToken ct = default)
    {
        using var origem = CancellationTokenSource.CreateLinkedTokenSource(ct);
        origem.CancelAfter(limite);

        try
        {
            return await acao(origem.Token);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            throw new TimeoutException($"A operação passou de {limite.TotalSeconds:0.#} s.");
        }
    }

    /// <summary>Plano B: se as tentativas esgotarem, entrega uma resposta mais simples em vez de quebrar.</summary>
    public static async Task<T> ComPlanoBAsync<T>(Func<Task<T>> principal, Func<T> planoB)
    {
        try
        {
            return await principal();
        }
        catch (TentativasEsgotadasException)
        {
            return planoB();
        }
    }
}
