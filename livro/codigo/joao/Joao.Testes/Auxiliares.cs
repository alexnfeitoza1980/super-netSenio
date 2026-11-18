namespace Joao.Testes;

/// <summary>Um relógio que só anda quando mandamos: o teste controla o tempo.</summary>
public sealed class RelogioDeTeste : TimeProvider
{
    private DateTimeOffset agora = new(2026, 11, 2, 9, 0, 0, TimeSpan.Zero);

    public override DateTimeOffset GetUtcNow() => agora;

    public void Avancar(TimeSpan tempo) => agora += tempo;
}
