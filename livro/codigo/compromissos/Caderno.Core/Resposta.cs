namespace Quatro;

public sealed record RespostaEscolhida
{
    public string MensagemRecebida { get; }
    public string LeituraPessoal { get; }
    public string Resposta { get; }
    public string Limite { get; }

    public RespostaEscolhida(string mensagemRecebida, string leituraPessoal,
        string resposta, string limite)
    {
        MensagemRecebida = Regras.Texto(mensagemRecebida);
        LeituraPessoal = Regras.Texto(leituraPessoal);
        Resposta = Regras.Texto(resposta);
        Limite = Regras.Texto(limite);
    }
}
