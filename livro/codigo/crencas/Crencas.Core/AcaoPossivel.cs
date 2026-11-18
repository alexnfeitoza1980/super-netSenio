namespace Jornada.Crencas;

public enum AlcanceDaAcao
{
    Individual,
    Compartilhado,
    IndisponivelAgora
}

public sealed record AcaoPossivel(
    string Descricao,
    AlcanceDaAcao Alcance,
    string? ApoioNecessario);
