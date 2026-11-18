using Jornada.Resiliencia.Cap89;
using Jornada.Resiliencia.Cap90;
using Jornada.Resiliencia.Cap92;
using Jornada.Resiliencia.Cap93;

namespace Jornada.Resiliencia.Lab10;

public sealed class CadernoDeProximoPasso
{
    public string Situacao { get; init; } = "uma tarefa que quero organizar";
    public Sentimento? Sentimento { get; set; }

    public List<Anotacao> Anotacoes { get; } = new();
    public List<Preocupacao> Preocupacoes { get; } = new();
    public Mochila Mochila { get; } = new();
    public Meta? Meta { get; set; }
    public MapaDeApoio Apoio { get; } = new();

    /// <summary>Monta o resumo da sessão. Só usa o que a pessoa escolheu preencher.</summary>
    public IReadOnlyList<string> Resumo()
    {
        var linhas = new List<string> { $"Situação: {Situacao}" };

        if (Sentimento is { } s)
        {
            linhas.Add($"Como me sinto: {Vocabulario.Nomear(s)} ({s.Intensidade}/10).");
            if (SugestaoDePausa.Para(s) is { } pausa && pausa > TimeSpan.Zero)
                linhas.Add($"Uma pausa de {pausa.TotalMinutes:0} min antes de decidir pode ajudar. É uma sugestão.");
        }

        var (fatos, historias) = Organizador.Contar(Anotacoes);
        if (fatos + historias > 0)
        {
            linhas.Add($"Fatos: {fatos} · Histórias que preciso checar: {historias}.");
            linhas.AddRange(Organizador.PerguntasParaAsHistorias(Anotacoes).Select(p => "  ? " + p));
        }

        if (Organizador.ProximoFoco(Preocupacoes) is { } foco)
            linhas.Add($"Onde pôr energia primeiro: {foco.Texto}.");

        if (Meta?.ProximoPasso is { } passo)
        {
            var (feitos, total) = Meta.Andamento;
            linhas.Add($"Próximo passo possível: {passo.Descricao} ({passo.Minutos} min). Andamento: {feitos}/{total}.");
        }

        var pessoas = Apoio.Quem(Necessidade.Estudar).Concat(Apoio.Quem(Necessidade.Ouvir)).Select(a => a.Nome).Distinct().ToList();
        if (pessoas.Count > 0)
            linhas.Add("Quem pode apoiar: " + string.Join(", ", pessoas) + ".");

        foreach (var pergunta in Mochila.Perguntas().Take(1))
            linhas.Add("Uma pergunta sobre a sua mochila: " + pergunta);

        return linhas;
    }
}
