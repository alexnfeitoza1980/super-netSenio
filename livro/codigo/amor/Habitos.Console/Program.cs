using Jornada.Habitos.Lab12;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("Auditoria litúrgica: o que o seu calendário diz que você ama?");
Console.WriteLine("Tudo é lido do seu computador e nada é gravado nem enviado.");
Console.WriteLine();

string json;
if (args.Length == 0)
{
    Console.WriteLine("(Sem arquivo informado: usando o exemplo. Para usar o seu: dotnet run -- meu-diario.json)");
    json = Carregador.Exemplo;
}
else if (File.Exists(args[0]))
{
    json = File.ReadAllText(args[0]);
}
else
{
    Console.WriteLine($"Não encontrei o arquivo \"{args[0]}\".");
    return 1;
}

if (!Carregador.TentarCarregar(json, out var diario, out var problema))
{
    Console.WriteLine(problema);
    return 1;
}

foreach (var linha in Auditor.Auditar(diario!))
    Console.WriteLine("• " + linha);

return 0;
