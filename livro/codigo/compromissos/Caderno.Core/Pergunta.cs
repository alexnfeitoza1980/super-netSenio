namespace Quatro;

public enum EstadoDaPergunta { Pendente, Confirmada, Refutada }
public sealed record Evidencia(string Resposta, string Fonte);

public sealed class Pergunta
{
    public string Texto { get; }
    public EstadoDaPergunta Estado { get; private set; }
    public Evidencia? Evidencia { get; private set; }

    public Pergunta(string texto) => Texto = Regras.Texto(texto);

    public void Esclarecer(bool confirmou, string resposta, string fonte)
    {
        if (Estado != EstadoDaPergunta.Pendente)
            throw new InvalidOperationException("Registre outra pergunta para uma nova investigação.");
        var evidencia = new Evidencia(Regras.Texto(resposta), Regras.Texto(fonte));
        Evidencia = evidencia;
        Estado = confirmou ? EstadoDaPergunta.Confirmada : EstadoDaPergunta.Refutada;
    }
}
