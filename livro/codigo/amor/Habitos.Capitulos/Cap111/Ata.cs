namespace Jornada.Habitos.Cap111;

/// <summary>Um "ouvinte" do ritual: se inscreve no evento e monta a ata sem que o ritual saiba que ela existe.</summary>
public sealed class Ata
{
    private readonly List<string> linhas = new();

    public Ata(RitualDeReuniao ritual) => ritual.MovimentoConcluido += AoConcluir;

    public IReadOnlyList<string> Linhas => linhas;

    private void AoConcluir(object? origem, MovimentoConcluidoEventArgs e) =>
        linhas.Add($"{linhas.Count + 1}. {e.Concluido}: {e.Registro}");
}
