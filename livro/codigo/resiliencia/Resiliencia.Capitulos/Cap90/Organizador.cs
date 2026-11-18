namespace Jornada.Resiliencia.Cap90;

public enum Controle { DependeDeMim, PossoInfluenciar, NaoDepende }

public sealed record Preocupacao(string Texto, Controle Controle);

public enum NaturezaDaFrase { Fato, Historia }

public sealed record Anotacao(string Texto, NaturezaDaFrase Natureza);

public static class Organizador
{
    /// <summary>Agrupa as preocupações pelas três "zonas" de controle.</summary>
    public static ILookup<Controle, Preocupacao> Separar(IEnumerable<Preocupacao> itens) =>
        itens.ToLookup(p => p.Controle);

    /// <summary>Sugere onde pôr energia primeiro: o que depende de mim; se não houver, o que posso influenciar.</summary>
    public static Preocupacao? ProximoFoco(IEnumerable<Preocupacao> itens) =>
        itens.FirstOrDefault(p => p.Controle == Controle.DependeDeMim)
        ?? itens.FirstOrDefault(p => p.Controle == Controle.PossoInfluenciar);

    /// <summary>Quantos fatos e quantas histórias há nas anotações. Devolve uma tupla.</summary>
    public static (int Fatos, int Historias) Contar(IEnumerable<Anotacao> anotacoes)
    {
        var lista = anotacoes.ToList();
        return (lista.Count(a => a.Natureza == NaturezaDaFrase.Fato),
                lista.Count(a => a.Natureza == NaturezaDaFrase.Historia));
    }

    /// <summary>Cada história merece uma pergunta de checagem, não uma bronca.</summary>
    public static IEnumerable<string> PerguntasParaAsHistorias(IEnumerable<Anotacao> anotacoes) =>
        anotacoes
            .Where(a => a.Natureza == NaturezaDaFrase.Historia)
            .Select(a => $"\"{a.Texto}\": o que me faz pensar assim? Existe outra explicação possível?");
}
