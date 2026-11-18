namespace Jornada.Resiliencia.Cap96;

/// <summary>Um serviço de mentira que falha um número combinado de vezes e depois passa a funcionar.</summary>
public sealed class ServicoInstavel
{
    private readonly int falhasAntesDeFuncionar;

    public int Chamadas { get; private set; }

    public ServicoInstavel(int falhasAntesDeFuncionar) => this.falhasAntesDeFuncionar = falhasAntesDeFuncionar;

    public Task<string> BuscarFraseAsync(CancellationToken ct = default)
    {
        Chamadas++;

        if (Chamadas <= falhasAntesDeFuncionar)
            throw new FalhaPassageiraException($"Falha {Chamadas}: serviço ocupado.");

        return Task.FromResult("Um passo de cada vez.");
    }
}
