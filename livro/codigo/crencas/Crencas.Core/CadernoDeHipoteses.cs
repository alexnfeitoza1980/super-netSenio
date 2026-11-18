namespace Jornada.Crencas;

public sealed class CadernoDeHipoteses
{
    private readonly List<Investigacao> investigacoes = new();

    public IReadOnlyList<Investigacao> Investigacoes => investigacoes.AsReadOnly();

    public Investigacao Adicionar(Hipotese hipotese)
    {
        var nova = new Investigacao(hipotese);
        investigacoes.Add(nova);
        return nova;
    }

    public Investigacao? Encontrar(string enunciado)
    {
        if (string.IsNullOrWhiteSpace(enunciado))
            throw new ArgumentException("Informe o enunciado para a busca.", nameof(enunciado));

        return investigacoes.FirstOrDefault(i =>
            i.Hipotese.Enunciado.Equals(enunciado, StringComparison.OrdinalIgnoreCase));
    }
}
