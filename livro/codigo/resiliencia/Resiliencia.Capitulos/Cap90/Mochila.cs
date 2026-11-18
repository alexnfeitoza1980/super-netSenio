namespace Jornada.Resiliencia.Cap90;

public enum TipoRecurso { Pessoal, Pessoas, Material, Informacao }

public sealed record Recurso(string Nome, TipoRecurso Tipo);

public sealed class Mochila
{
    private readonly List<Recurso> recursos = new();

    public IReadOnlyList<Recurso> Todos => recursos;

    public Mochila Guardar(string nome, TipoRecurso tipo)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Dê um nome ao recurso.", nameof(nome));

        recursos.Add(new Recurso(nome.Trim(), tipo));
        return this;                                    // devolver "this" permite encadear chamadas
    }

    public IEnumerable<IGrouping<TipoRecurso, Recurso>> PorTipo() =>
        recursos.GroupBy(r => r.Tipo).OrderBy(g => g.Key);

    /// <summary>Tipos de recurso em que a mochila ainda está vazia.</summary>
    public IReadOnlyList<TipoRecurso> TiposVazios() =>
        Enum.GetValues<TipoRecurso>().Except(recursos.Select(r => r.Tipo)).ToList();

    /// <summary>Para cada lacuna, uma pergunta curiosa, nunca uma cobrança.</summary>
    public IEnumerable<string> Perguntas() =>
        TiposVazios().Select(tipo => tipo switch
        {
            TipoRecurso.Pessoal    => "Que habilidade ou qualidade sua já ajudou em outra ocasião?",
            TipoRecurso.Pessoas    => "Quem costuma ouvir você ou ajudar com ideias?",
            TipoRecurso.Material   => "Que objeto, lugar ou ferramenta poderia facilitar o próximo passo?",
            TipoRecurso.Informacao => "Que informação ou orientação ainda falta descobrir?",
            _                      => throw new ArgumentOutOfRangeException(nameof(tipo))
        });
}
