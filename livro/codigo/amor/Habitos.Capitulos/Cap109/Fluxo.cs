using System.Numerics;

namespace Jornada.Habitos.Cap109;

/// <summary>
/// Padrões de desenho que treinam hábitos sem dizer nada. Cada um é um "bit" ligado ou desligado,
/// então vários cabem no mesmo número: é o que o atributo [Flags] permite.
/// </summary>
[Flags]
public enum PadraoDeDesign
{
    Nenhum             = 0,
    RolagemInfinita    = 1 << 0,
    SequenciaDiaria    = 1 << 1,
    RecompensaVariavel = 1 << 2,
    NotificacaoPush    = 1 << 3,
    OptInPadrao        = 1 << 4,
    ProvaSocial        = 1 << 5,
    EscassezFalsa      = 1 << 6
}

/// <summary>Descrição de um fluxo de produto, escrita por quem o desenhou. Nada aqui lê pessoas.</summary>
public sealed record Fluxo(string Nome, PadraoDeDesign Padroes, bool PermiteSairFacilmente);

public sealed record Observacao(PadraoDeDesign Padrao, string Pergunta);

public enum Gravidade { Leve, Moderada, Alta }

public static class AuditoriaDeFluxo
{
    private static readonly IReadOnlyDictionary<PadraoDeDesign, string> Perguntas = new Dictionary<PadraoDeDesign, string>
    {
        [PadraoDeDesign.RolagemInfinita]    = "Onde a pessoa encontra um ponto natural de parada?",
        [PadraoDeDesign.SequenciaDiaria]    = "O que acontece com quem perde um dia? O fim da sequência pune ou acolhe?",
        [PadraoDeDesign.RecompensaVariavel] = "A recompensa imprevisível serve à pessoa ou só a prende?",
        [PadraoDeDesign.NotificacaoPush]    = "A notificação traz algo que a pessoa pediu ou só um motivo para voltar?",
        [PadraoDeDesign.OptInPadrao]        = "O padrão veio ligado? Quem decidiu isso, e por quê?",
        [PadraoDeDesign.ProvaSocial]        = "O número mostrado ajuda a decidir ou só pressiona?",
        [PadraoDeDesign.EscassezFalsa]      = "A escassez é real? Se não for, por que mostrá-la?",
    };

    /// <summary>Uma pergunta de cuidado para cada padrão presente no fluxo.</summary>
    public static IReadOnlyList<Observacao> Examinar(Fluxo fluxo) =>
        Enum.GetValues<PadraoDeDesign>()
            .Where(p => p != PadraoDeDesign.Nenhum && fluxo.Padroes.HasFlag(p))
            .Select(p => new Observacao(p, Perguntas[p]))
            .ToList();

    /// <summary>Quantos padrões estão ligados ao mesmo tempo: conta os bits do número.</summary>
    public static int Quantidade(PadraoDeDesign padroes) => BitOperations.PopCount((uint)padroes);

    public static Gravidade Avaliar(Fluxo fluxo) => (Quantidade(fluxo.Padroes), fluxo.PermiteSairFacilmente) switch
    {
        (<= 1, true)  => Gravidade.Leve,
        (<= 2, true)  => Gravidade.Moderada,
        (_, false)    => Gravidade.Alta,
        (>= 5, _)     => Gravidade.Alta,
        _             => Gravidade.Moderada
    };

    /// <summary>Padrões que só estão ligados no fluxo B, em relação ao fluxo A. (B &amp; ~A)</summary>
    public static PadraoDeDesign Acrescentou(Fluxo antes, Fluxo depois) => depois.Padroes & ~antes.Padroes;
}
