using System.Text.Json;
namespace Quatro;

public interface IArquivoDoCaderno
{
    void Salvar(Retrato retrato);
    Retrato? Ler();
}

public sealed class ArquivoJson(string caminho) : IArquivoDoCaderno
{
    private static readonly JsonSerializerOptions Opcoes = new() { WriteIndented = true };

    public void Salvar(Retrato retrato)
    {
        // Um processo, arquivo local: sem transação ou controle de concorrência.
        File.WriteAllText(caminho, JsonSerializer.Serialize(retrato, Opcoes));
    }

    public Retrato? Ler()
    {
        if (!File.Exists(caminho)) return null;
        var r = JsonSerializer.Deserialize<Retrato>(File.ReadAllText(caminho), Opcoes)
            ?? throw new InvalidDataException("O arquivo não contém um caderno.");
        if (r.Versao != 1) throw new InvalidDataException("Versão não suportada.");
        if (r.Acordos is null || r.Promessas is null || r.Reflexoes is null ||
            r.Respostas is null || r.Perguntas is null || r.Praticas is null)
            throw new InvalidDataException("Faltam coleções no arquivo.");
        return r;
    }
}

public sealed class ArquivoEmMemoria : IArquivoDoCaderno
{
    private string? json;
    public void Salvar(Retrato r) => json = JsonSerializer.Serialize(r);
    public Retrato? Ler() => json is null ? null : JsonSerializer.Deserialize<Retrato>(json);
}
