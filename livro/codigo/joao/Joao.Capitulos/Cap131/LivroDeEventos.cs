namespace Jornada.Joao.Cap131;

// Jo 11,14.17.39-44 (Bíblia Livre). Os eventos abaixo são os acontecimentos que o texto narra, na ordem em que ele os narra:
//  "Lázaro está morto" (14) · "Tirai a pedra" (39, 41) · "clamou com grande voz: Lázaro, sai fora" (43) ·
//  "o que estava morto saiu, com as mãos e os pés atados, e seu rosto envolto em um lenço" (44) · "Desatai-o, e deixai-o ir" (44).
//
// TEXTO       : narra uma sequência. Não explica COMO nem POR QUÊ; o poder que age ali não é objeto do código.
// ANALOGIA    : guardar os FATOS na ordem e calcular o estado a partir deles (event sourcing).
// LIMITE      : cada regra abaixo (por exemplo, "só sai depois de chamado") é a ORDEM do relato, e não uma explicação do que o texto narra.

/// <summary>Algo que aconteceu. Eventos nunca são apagados nem alterados: o estado é o que resulta deles.</summary>
public abstract record Evento(DateTimeOffset Quando);

public sealed record Faleceu(DateTimeOffset Quando) : Evento(Quando);                         // 11,14: "Lázaro está morto"
public sealed record PedraTirada(DateTimeOffset Quando) : Evento(Quando);                     // 11,39.41: "Tirai a pedra"
public sealed record Chamado(DateTimeOffset Quando) : Evento(Quando);                         // 11,43: "Lázaro, sai fora"
public sealed record Saiu(DateTimeOffset Quando) : Evento(Quando);                            // 11,44: "o que estava morto saiu"
public sealed record Desatado(DateTimeOffset Quando, string Atadura) : Evento(Quando);        // 11,44: "Desatai-o"

public enum Situacao { Vivo, Morto }

public sealed record EstadoDeLazaro(
    Situacao Situacao,
    bool SepulcroAberto,
    bool FoiChamado,
    bool JaSaiu,
    IReadOnlyList<string> Ataduras,
    int Versao)
{
    public static readonly EstadoDeLazaro Inicial = new(Situacao.Vivo, false, false, false, Array.Empty<string>(), 0);

    /// <summary>Livre = vivo, fora do sepulcro e sem nenhuma atadura: o que o final do relato descreve.</summary>
    public bool Livre => Situacao == Situacao.Vivo && JaSaiu && Ataduras.Count == 0;

    /// <summary>Calcula o estado seguinte. Um "switch" sobre o TIPO do evento decide o que muda; ordem impossível é recusada.</summary>
    public EstadoDeLazaro Aplicar(Evento evento)
    {
        var proximo = evento switch
        {
            Faleceu when Situacao == Situacao.Vivo && !JaSaiu =>
                this with { Situacao = Situacao.Morto },

            Faleceu => throw new InvalidOperationException("Só falece quem está vivo."),

            PedraTirada when Situacao == Situacao.Morto && !SepulcroAberto =>
                this with { SepulcroAberto = true },

            PedraTirada => throw new InvalidOperationException("Só se tira a pedra do sepulcro de quem faleceu, e uma vez."),

            Chamado when SepulcroAberto && !FoiChamado =>
                this with { FoiChamado = true },

            Chamado => throw new InvalidOperationException("O chamado vem depois de tirada a pedra, e uma vez."),

            Saiu when FoiChamado && !JaSaiu =>
                this with { Situacao = Situacao.Vivo, JaSaiu = true, Ataduras = new[] { "atadura nas mãos", "atadura nos pés", "lenço no rosto" } },

            Saiu => throw new InvalidOperationException("Só sai depois de chamado, e uma vez."),

            Desatado d when JaSaiu && Ataduras.Contains(d.Atadura) =>
                this with { Ataduras = Ataduras.Where(a => a != d.Atadura).ToList() },

            Desatado d => throw new InvalidOperationException($"Não há a atadura \"{d.Atadura}\" para desatar."),

            _ => throw new NotSupportedException($"Evento desconhecido: {evento.GetType().Name}.")
        };

        return proximo with { Versao = Versao + 1 };
    }
}

/// <summary>
/// O livro dos fatos. Esta versão guarda tudo na memória e foi pensada para uso em uma única thread:
/// serve para entender a ideia, não é um armazenamento de eventos para produção.
/// </summary>
public sealed class LivroDeEventos
{
    private readonly List<Evento> eventos = new();

    public IReadOnlyList<Evento> Eventos => eventos;

    public void Registrar(Evento evento)
    {
        if (eventos.Count > 0 && evento.Quando < eventos[^1].Quando)
            throw new InvalidOperationException("Os eventos precisam ser registrados em ordem de tempo.");

        // Valida aplicando sobre o estado atual: um evento fora de ordem nem chega a entrar no livro.
        _ = Estado().Aplicar(evento);
        eventos.Add(evento);
    }

    /// <summary>O estado de hoje: é só dobrar (Aggregate) todos os eventos, do início até agora.</summary>
    public EstadoDeLazaro Estado() => Reconstruir(eventos);

    /// <summary>Viagem no tempo: como estava em determinado momento?</summary>
    public EstadoDeLazaro EstadoEm(DateTimeOffset momento) =>
        Reconstruir(eventos.TakeWhile(e => e.Quando <= momento));

    public static EstadoDeLazaro Reconstruir(IEnumerable<Evento> eventos) =>
        eventos.Aggregate(EstadoDeLazaro.Inicial, (estado, evento) => estado.Aplicar(evento));
}
