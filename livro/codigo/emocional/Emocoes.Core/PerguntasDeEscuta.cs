namespace Jornada.Emocoes;

public static class PerguntasDeEscuta
{
    public static IReadOnlyList<string> Exemplos { get; } = Array.AsReadOnly(
    [
        "O que você gostaria que eu entendesse?",
        "Quer pensar em opções comigo ou prefere que eu escute?",
        "Há algo prático que ajudaria agora?",
        "Está tudo bem deixar isso para outro momento?",
        "O que eu posso fazer diferente na próxima conversa?"
    ]);
}
