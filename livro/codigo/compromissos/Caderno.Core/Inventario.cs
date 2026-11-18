namespace Quatro;

public sealed record Acordo(Guid Id, string Texto, string Origem, bool Escolhido);

public sealed class Inventario
{
    private readonly List<Acordo> acordos = [];
    public IReadOnlyList<Acordo> Todos => acordos.ToArray();

    public Acordo Registrar(string texto, string origem)
    {
        var acordo = new Acordo(Guid.NewGuid(), Regras.Texto(texto),
            Regras.Texto(origem), false);
        acordos.Add(acordo);
        return acordo;
    }

    public void Escolher(Guid id)
    {
        int i = acordos.FindIndex(a => a.Id == id);
        if (i < 0) throw new KeyNotFoundException("Acordo não encontrado.");
        acordos[i] = acordos[i] with { Escolhido = true };
    }

    public IReadOnlyList<Acordo> Buscar(string texto)
    {
        string termo = Regras.Texto(texto);
        return acordos.Where(a => a.Texto.Contains(termo,
            StringComparison.OrdinalIgnoreCase)).ToArray();
    }
}
