namespace Jornada.Habitos.Cap111;

/// <summary>Os quatro "capítulos" de um encontro que forma: reunir, escutar, partilhar e enviar.</summary>
public enum Movimento { Reunir, Escutar, Partilhar, Enviar, Encerrado }

/// <summary>Os dados que acompanham o evento: o que foi concluído, o que foi registrado e qual é o próximo.</summary>
public sealed class MovimentoConcluidoEventArgs : EventArgs
{
    public MovimentoConcluidoEventArgs(Movimento concluido, string registro, Movimento proximo)
    {
        Concluido = concluido;
        Registro = registro;
        Proximo = proximo;
    }

    public Movimento Concluido { get; }
    public string Registro { get; }
    public Movimento Proximo { get; }
}

public sealed class RitualDeReuniao
{
    // A tabela de transições: de onde se pode ir. O fluxo é dado, não improvisado.
    private static readonly IReadOnlyDictionary<Movimento, Movimento> Seguinte = new Dictionary<Movimento, Movimento>
    {
        [Movimento.Reunir]    = Movimento.Escutar,
        [Movimento.Escutar]   = Movimento.Partilhar,
        [Movimento.Partilhar] = Movimento.Enviar,
        [Movimento.Enviar]    = Movimento.Encerrado,
    };

    public static string PerguntaDe(Movimento movimento) => movimento switch
    {
        Movimento.Reunir    => "Quem está aqui e o que cada pessoa trouxe consigo hoje?",
        Movimento.Escutar   => "O que precisamos ouvir: dados, feedback, o que não vai bem?",
        Movimento.Partilhar => "O que decidimos juntos? O que merece ser agradecido?",
        Movimento.Enviar    => "Quem faz o quê, até quando?",
        _                   => "Encontro encerrado."
    };

    private readonly List<(Movimento Movimento, string Registro)> registros = new();

    public Movimento Atual { get; private set; } = Movimento.Reunir;

    public IReadOnlyList<(Movimento Movimento, string Registro)> Registros => registros;

    /// <summary>Um "evento" do C#: quem quiser ser avisado se inscreve com +=, sem que o ritual conheça os inscritos.</summary>
    public event EventHandler<MovimentoConcluidoEventArgs>? MovimentoConcluido;

    public void Concluir(string registro)
    {
        if (Atual == Movimento.Encerrado)
            throw new InvalidOperationException("O encontro já foi encerrado.");

        if (string.IsNullOrWhiteSpace(registro))
            throw new ArgumentException($"Registre algo antes de concluir: {PerguntaDe(Atual)}", nameof(registro));

        var concluido = Atual;
        Atual = Seguinte[concluido];
        registros.Add((concluido, registro.Trim()));

        MovimentoConcluido?.Invoke(this, new MovimentoConcluidoEventArgs(concluido, registro.Trim(), Atual));
    }

    /// <summary>Um encontro sem "enviar" não mudou nada: terminou sem compromissos. Só é completo se passou por todos.</summary>
    public bool Completo => Atual == Movimento.Encerrado;
}
