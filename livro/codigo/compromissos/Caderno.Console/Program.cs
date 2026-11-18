using CadernoApp;
using Quatro;
using System.Globalization;
CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
try
{
    if (args.Length == 1 && args[0].StartsWith("cap") && int.TryParse(args[0][3..], out int n) && n >= 74 && n <= 87)
    {
        typeof(Exemplos).GetMethod($"Cap{n}")!.Invoke(null, null);
        return 0;
    }
    if (args.Length == 2 && args[0] == "jornada")
    {
        JornadaGuiada.Executar(args[1]);
        return 0;
    }
    if (args.Length == 2 && args[0] == "ler")
    {
        var r = new ArquivoJson(args[1]).Ler();
        Console.WriteLine(r is null ? "Arquivo ainda não existe." : $"Caderno v{r.Versao}: {r.Promessas.Length} promessas; {r.Praticas.Length} práticas.");
        return 0;
    }
    if (args.Length == 2 && args[0] == "novo")
    {
        if (File.Exists(args[1])) throw new IOException("O arquivo já existe; escolha outro nome.");
        new ArquivoJson(args[1]).Salvar(new Caderno().Exportar());
        Console.WriteLine("Caderno vazio criado. Use praticar para registrar sua jornada.");
        return 0;
    }
    if (args.Length == 4 && args[0] == "praticar")
    {
        var arquivo = new ArquivoJson(args[1]);
        var r = arquivo.Ler() ?? throw new FileNotFoundException("Crie o caderno primeiro com novo.");
        var pratica = new Pratica(DateOnly.FromDateTime(DateTime.Today), Regras.Texto(args[2]), Regras.Texto(args[3]));
        arquivo.Salvar(r with { Praticas = [..r.Praticas, pratica] });
        Console.WriteLine($"Prática registrada. Total: {r.Praticas.Length + 1}.");
        return 0;
    }
    Console.WriteLine("Use: cap74 ... cap87 | jornada <arquivo> | novo <arquivo> | ler <arquivo> | praticar <arquivo> <ação> <aprendizado>");
    return args.Length == 0 ? 0 : 1;
}
catch (Exception e)
{
    Console.Error.WriteLine((e.InnerException ?? e).Message);
    return 1;
}
