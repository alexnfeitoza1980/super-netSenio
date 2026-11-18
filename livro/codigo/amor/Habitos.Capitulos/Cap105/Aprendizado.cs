namespace Jornada.Habitos.Cap105;

/// <summary>As três "regiões" de uma pessoa: cabeça (saber), coração (querer) e mãos (praticar).</summary>
[Flags]
public enum Dimensao
{
    Nenhuma = 0,
    Cabeca  = 1,        // conhecimento
    Coracao = 2,        // desejo
    Maos    = 4,        // prática
    Todas   = Cabeca | Coracao | Maos
}

public sealed record AtividadeDeAprendizado(string Nome, Dimensao Envolve);

public sealed class PlanoDeAprendizado
{
    private readonly List<AtividadeDeAprendizado> atividades = new();

    public PlanoDeAprendizado Adicionar(string nome, Dimensao envolve)
    {
        atividades.Add(new AtividadeDeAprendizado(nome, envolve));
        return this;
    }

    /// <summary>União (OR) das dimensões de todas as atividades.</summary>
    public Dimensao Cobertura() =>
        atividades.Aggregate(Dimensao.Nenhuma, (acumulado, a) => acumulado | a.Envolve);

    /// <summary>O que ainda não é tocado por nenhuma atividade: tudo, menos o que já está coberto.</summary>
    public Dimensao Faltando() => Dimensao.Todas & ~Cobertura();

    public bool EnvolveSo(Dimensao dimensao) => atividades.All(a => a.Envolve == dimensao);

    public IEnumerable<string> Sugestoes()
    {
        var falta = Faltando();

        if (falta.HasFlag(Dimensao.Cabeca))
            yield return "Falta estudo: alguma leitura ou aula que dê nome e modelo ao assunto?";
        if (falta.HasFlag(Dimensao.Coracao))
            yield return "Falta desejo: o que faz você querer isto? Há uma história, um exemplo, alguém que inspire?";
        if (falta.HasFlag(Dimensao.Maos))
            yield return "Falta prática: o que você pode repetir toda semana, em tamanho pequeno?";
    }
}
