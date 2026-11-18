namespace Jornada.Resiliencia.Cap95;

/// <summary>Um limite pessoal dito com clareza: o que acontece, como afeta e o que se pede.</summary>
public sealed record Limite(string Situacao, string Impacto, string Pedido)
{
    public static Resultado<Limite> Criar(string? situacao, string? impacto, string? pedido)
    {
        var problemas = new List<string>();

        if (string.IsNullOrWhiteSpace(situacao)) problemas.Add("descreva o que acontece (sem rótulos sobre a pessoa)");
        if (string.IsNullOrWhiteSpace(impacto))  problemas.Add("diga como isso afeta você");
        if (string.IsNullOrWhiteSpace(pedido))   problemas.Add("faça um pedido concreto");

        return problemas.Count == 0
            ? Resultado<Limite>.Ok(new Limite(situacao!.Trim(), impacto!.Trim(), pedido!.Trim()))
            : Resultado<Limite>.Falha("Falta: " + string.Join("; ", problemas) + ".");
    }

    public string Montar() =>
        $"Quando {Situacao}, eu me sinto {Impacto}. Peço que {Pedido}.";
}
