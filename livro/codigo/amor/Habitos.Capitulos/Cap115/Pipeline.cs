namespace Jornada.Habitos.Cap115;

/// <summary>"Chame a próxima etapa". Uma etapa decide se, quando e como a cadeia continua.</summary>
public delegate Task Proximo();

/// <summary>Uma etapa do ritual de entrega: recebe o contexto e a próxima etapa.</summary>
public delegate Task Etapa(ContextoDeEntrega contexto, Proximo proximo);

public sealed class ContextoDeEntrega
{
    private readonly List<string> registro = new();

    public ContextoDeEntrega(string mudanca) => Mudanca = mudanca;

    public string Mudanca { get; }

    public IReadOnlyList<string> Registro => registro;

    public bool Interrompido { get; private set; }

    public string? Motivo { get; private set; }

    public void Anotar(string linha) => registro.Add(linha);

    public void Interromper(string motivo)
    {
        Interrompido = true;
        Motivo = motivo;
        registro.Add($"✗ {motivo}");
    }
}

/// <summary>
/// Uma esteira de etapas (o mesmo desenho dos "middlewares" que você verá em ASP.NET). Cada etapa é uma função.
/// A ordem forma o hábito da equipe: o que sempre vem antes, o que nunca se pula.
/// </summary>
public sealed class Pipeline
{
    private readonly List<Etapa> etapas = new();

    public Pipeline Usar(Etapa etapa)
    {
        etapas.Add(etapa);
        return this;
    }

    public async Task ExecutarAsync(ContextoDeEntrega contexto)
    {
        // Monta a cadeia de trás para a frente: cada "Proximo" chama a etapa seguinte.
        Proximo Montar(int indice) =>
            indice == etapas.Count
                ? () => Task.CompletedTask
                : () => etapas[indice](contexto, Montar(indice + 1));

        await Montar(0)();
    }
}

public static class Etapas
{
    /// <summary>Fábrica de etapas: cria uma etapa que roda uma verificação e interrompe se ela falhar.</summary>
    public static Etapa Verificar(string nome, Func<ContextoDeEntrega, Task<bool>> verificacao, string motivoSeFalhar) =>
        async (contexto, proximo) =>
        {
            if (await verificacao(contexto))
            {
                contexto.Anotar($"✓ {nome}");
                await proximo();                        // só continua se esta etapa aprovou
            }
            else
            {
                contexto.Interromper($"{nome}: {motivoSeFalhar}");
            }
        };

    /// <summary>Uma etapa "em volta": roda antes e depois do que vem pela frente (como um cronômetro).</summary>
    public static Etapa Medir(Action<string> aoFinal) =>
        async (contexto, proximo) =>
        {
            var inicio = System.Diagnostics.Stopwatch.GetTimestamp();
            await proximo();
            var gasto = System.Diagnostics.Stopwatch.GetElapsedTime(inicio);
            aoFinal($"Entrega de \"{contexto.Mudanca}\" levou {gasto.TotalMilliseconds:0} ms");
        };
}
