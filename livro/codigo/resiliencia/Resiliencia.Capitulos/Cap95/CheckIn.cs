namespace Jornada.Resiliencia.Cap95;

public enum RespostaDoCheckIn { Bem = 1, MaisOuMenos = 2, NaoEstouBem = 3, PreferoNaoDizer = 4 }

public static class CheckIn
{
    /// <summary>Lê a resposta. Silêncio é uma resposta válida: nunca obrigamos ninguém a dizer algo.</summary>
    public static Resultado<RespostaDoCheckIn> Ler(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            return Resultado<RespostaDoCheckIn>.Ok(RespostaDoCheckIn.PreferoNaoDizer);

        return texto.Trim() switch
        {
            "1" => Resultado<RespostaDoCheckIn>.Ok(RespostaDoCheckIn.Bem),
            "2" => Resultado<RespostaDoCheckIn>.Ok(RespostaDoCheckIn.MaisOuMenos),
            "3" => Resultado<RespostaDoCheckIn>.Ok(RespostaDoCheckIn.NaoEstouBem),
            "4" => Resultado<RespostaDoCheckIn>.Ok(RespostaDoCheckIn.PreferoNaoDizer),
            _   => Resultado<RespostaDoCheckIn>.Falha("Escolha de 1 a 4. Você também pode deixar em branco.")
        };
    }

    public static string Acolher(RespostaDoCheckIn r) => r switch
    {
        RespostaDoCheckIn.Bem            => "Que bom saber. Se quiser, anote o que ajudou.",
        RespostaDoCheckIn.MaisOuMenos    => "Dias assim existem. Quer só anotar ou também pensar num passo pequeno?",
        RespostaDoCheckIn.NaoEstouBem    => "Obrigado por dizer. Você não precisa resolver isso agora. Posso mostrar quem pode apoiar.",
        RespostaDoCheckIn.PreferoNaoDizer => "Tudo bem. A porta fica aberta para quando você quiser.",
        _                                => throw new ArgumentOutOfRangeException(nameof(r))
    };
}
