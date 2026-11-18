namespace Jornada.Resiliencia.Cap91;

/// <summary>Uma evidência é um fato que se pode conferir. "Fonte" pode faltar, e isso muda a leitura.</summary>
public sealed record Evidencia(string Texto, string? Fonte = null, DateOnly? Data = null)
{
    public bool TemFonte => !string.IsNullOrWhiteSpace(Fonte);
}

public enum Leitura
{
    Vazio,
    SoObstaculos,
    SoPossibilidades,
    PossibilidadesSemFonte,
    Equilibrado
}

public sealed class QuadroDeEvidencias
{
    private readonly List<Evidencia> obstaculos = new();
    private readonly List<Evidencia> possibilidades = new();

    public QuadroDeEvidencias Obstaculo(Evidencia e) { obstaculos.Add(e); return this; }
    public QuadroDeEvidencias Possibilidade(Evidencia e) { possibilidades.Add(e); return this; }

    public Leitura Ler()
    {
        int o = obstaculos.Count;
        int pTotal = possibilidades.Count;
        int pComFonte = possibilidades.Count(e => e.TemFonte);

        return (o, pTotal, pComFonte) switch
        {
            (0, 0, _)    => Leitura.Vazio,
            (_, > 0, 0)  => Leitura.PossibilidadesSemFonte,   // só frases prontas, sem evidência
            (> 0, 0, _)  => Leitura.SoObstaculos,
            (0, _, > 0)  => Leitura.SoPossibilidades,
            _            => Leitura.Equilibrado
        };
    }

    /// <summary>Possibilidades que ainda são "frases prontas" até alguém conferir.</summary>
    public IEnumerable<Evidencia> AConferir() => possibilidades.Where(e => !e.TemFonte);

    public static string Mensagem(Leitura leitura) => leitura switch
    {
        Leitura.Vazio                  => "Comece anotando um obstáculo real e uma possibilidade.",
        Leitura.SoObstaculos           => "Os obstáculos são reais. Que possibilidade, mesmo pequena, você consegue confirmar?",
        Leitura.SoPossibilidades       => "Ótimo ter possibilidades. E quais obstáculos também existem? Olhar os dois é realismo.",
        Leitura.PossibilidadesSemFonte => "Essas possibilidades ainda são ideias. De onde vem cada uma? Dá para conferir?",
        Leitura.Equilibrado            => "Você olhou os obstáculos e as possibilidades. Qual é o próximo passo possível?",
        _                              => throw new ArgumentOutOfRangeException(nameof(leitura))
    };
}
