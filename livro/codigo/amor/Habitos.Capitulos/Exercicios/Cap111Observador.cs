namespace Jornada.Habitos.Cap111;

/// <summary>
/// Conta quantos movimentos já foram concluídos. Quem se inscreve com += precisa poder sair com -=:
/// por isso implementa IDisposable e se "desinscreve" ao ser descartado.
/// </summary>
public sealed class ContadorDeMovimentos : IDisposable
{
    private readonly RitualDeReuniao ritual;

    public ContadorDeMovimentos(RitualDeReuniao ritual)
    {
        this.ritual = ritual;
        ritual.MovimentoConcluido += AoConcluir;
    }

    public int Total { get; private set; }

    private void AoConcluir(object? origem, MovimentoConcluidoEventArgs e) => Total++;

    public void Dispose() => ritual.MovimentoConcluido -= AoConcluir;
}
