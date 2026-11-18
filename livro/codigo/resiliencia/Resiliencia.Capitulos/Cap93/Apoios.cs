namespace Jornada.Resiliencia.Cap93;

public enum Necessidade { Ouvir, Estudar, TarefaPratica, Orientacao, Urgencia }

public interface IApoio
{
    string Nome { get; }
    bool Ajuda(Necessidade necessidade);
    string ComoPedir(Necessidade necessidade);
}

public abstract class ApoioBase : IApoio
{
    private readonly HashSet<Necessidade> atende;

    protected ApoioBase(string nome, params Necessidade[] atende)
    {
        Nome = nome;
        this.atende = new HashSet<Necessidade>(atende);
    }

    public string Nome { get; }

    public bool Ajuda(Necessidade necessidade) => atende.Contains(necessidade);

    public abstract string ComoPedir(Necessidade necessidade);

    protected static string Assunto(Necessidade n) => n switch
    {
        Necessidade.Ouvir         => "conversar um pouco",
        Necessidade.Estudar       => "estudar junto",
        Necessidade.TarefaPratica => "dar uma mão numa tarefa",
        Necessidade.Orientacao    => "pedir uma orientação",
        _                         => "falar com urgência"
    };
}

public sealed class Pessoa : ApoioBase
{
    public Pessoa(string nome, params Necessidade[] atende) : base(nome, atende) { }

    public override string ComoPedir(Necessidade n) =>
        $"Oi, {Nome}. Você teria um tempinho para {Assunto(n)}? Pode ser rápido.";
}

public sealed class Profissional : ApoioBase
{
    public Profissional(string nome, params Necessidade[] atende) : base(nome, atende) { }

    public override string ComoPedir(Necessidade n) =>
        $"Olá. Gostaria de marcar um horário com {Nome} para {Assunto(n)}.";
}

public sealed class Servico : ApoioBase
{
    public string Telefone { get; }

    public Servico(string nome, string telefone, params Necessidade[] atende) : base(nome, atende) =>
        Telefone = telefone;

    public override string ComoPedir(Necessidade n) =>
        $"{Nome}: ligue {Telefone}. Você não precisa explicar tudo de uma vez.";
}

public static class ServicosPublicos
{
    /// <summary>Serviços do Brasil que existem para urgências. Valem para qualquer pessoa, a qualquer hora.</summary>
    public static IReadOnlyList<Servico> Brasil { get; } = new[]
    {
        new Servico("CVV (apoio emocional)", "188", Necessidade.Ouvir, Necessidade.Urgencia),
        new Servico("SAMU (emergência médica)", "192", Necessidade.Urgencia),
    };
}
