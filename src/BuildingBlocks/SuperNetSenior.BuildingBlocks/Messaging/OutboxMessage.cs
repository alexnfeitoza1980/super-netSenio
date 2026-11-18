namespace SuperNetSenior.BuildingBlocks.Messaging;

public sealed class OutboxMessage
{
    public Guid Id { get; private set; }
    public string Tipo { get; private set; } = string.Empty;
    public string Payload { get; private set; } = string.Empty;
    public DateTime CriadoEm { get; private set; }
    public DateTime? ProcessadoEm { get; private set; }
    public string? Erro { get; private set; }
    public int Tentativas { get; private set; }

    private OutboxMessage() { }

    public OutboxMessage(Guid id, string tipo, string payload, DateTime criadoEm)
    {
        Id = id;
        Tipo = tipo;
        Payload = payload;
        CriadoEm = criadoEm;
    }

    public void MarcarComoProcessada()
    {
        ProcessadoEm = DateTime.UtcNow;
        Erro = null;
    }

    public void RegistrarFalha(string erro)
    {
        Tentativas++;
        Erro = erro.Length <= 1000 ? erro : erro[..1000];
    }
}
