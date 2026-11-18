namespace Jornada.Resiliencia.Cap89;

public sealed class DiarioDeEmocoes
{
    private readonly List<(DateOnly Dia, Sentimento Sentimento)> registros = new();

    public void Registrar(DateOnly dia, Sentimento sentimento) => registros.Add((dia, sentimento));

    /// <summary>Quantas vezes cada emoção apareceu. Sem nota, sem "emoção boa" ou "ruim".</summary>
    public IReadOnlyDictionary<Emocao, int> Contagem() =>
        registros
            .GroupBy(r => r.Sentimento.Emocao)
            .ToDictionary(g => g.Key, g => g.Count());

    public IReadOnlyList<string> Linha() =>
        registros
            .OrderBy(r => r.Dia)
            .Select(r => $"{r.Dia:dd/MM}: {Vocabulario.Nomear(r.Sentimento)} ({r.Sentimento.Intensidade}/10)")
            .ToList();
}
