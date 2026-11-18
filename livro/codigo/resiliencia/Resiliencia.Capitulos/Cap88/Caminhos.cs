namespace Jornada.Resiliencia.Cap88;

public enum Resposta { Recuperar, Adaptar, BuscarApoio }

public sealed record Dificuldade(
    string Descricao,
    bool RotinaAindaPossivel,
    bool TemAlguemDeConfianca);

public static class Caminhos
{
    /// <summary>Devolve os caminhos que fazem sentido agora. Nenhum deles é obrigatório.</summary>
    public static IReadOnlyList<Resposta> Possiveis(Dificuldade d)
    {
        var caminhos = new List<Resposta>();

        if (d.RotinaAindaPossivel)
            caminhos.Add(Resposta.Recuperar);

        caminhos.Add(Resposta.Adaptar);          // adaptar-se é sempre uma possibilidade

        if (d.TemAlguemDeConfianca)
            caminhos.Add(Resposta.BuscarApoio);

        return caminhos;
    }

    public static string Descrever(Resposta r) => r switch
    {
        Resposta.Recuperar   => "Retomar o que for possível da rotina, no seu ritmo.",
        Resposta.Adaptar     => "Procurar outro caminho para chegar a um objetivo parecido.",
        Resposta.BuscarApoio => "Contar com alguém de confiança para atravessar junto.",
        _                    => throw new ArgumentOutOfRangeException(nameof(r))
    };
}
