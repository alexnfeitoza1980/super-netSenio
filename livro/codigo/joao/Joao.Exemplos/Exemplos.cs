namespace Jornada.Joao.Exemplos;

/// <summary>
/// Os exemplos executáveis do livro. Cada chave roda o programa de um bloco "Uso" de um capítulo e imprime a saída que o livro mostra:
/// "t124" é o passo a passo do capítulo 124 e "124" é a parte "Indo além". O mesmo registro alimenta o comando do Console
/// (<c>dotnet run --project codigo/Joao.Console -- cap124</c>) e os testes que conferem as saídas publicadas.
/// </summary>
public static class Exemplos
{
    private static Func<Task> Sync(Action acao) => () => { acao(); return Task.CompletedTask; };

    public static readonly IReadOnlyDictionary<string, Func<Task>> Todos = new Dictionary<string, Func<Task>>
    {
        ["t124"] = Sync(DemoT124.Run), ["124"] = Sync(Demo124.Run),
        ["t125"] = Sync(DemoT125.Run), ["125"] = Sync(Demo125.Run),
        ["t126"] = Sync(DemoT126.Run), ["126"] = Sync(Demo126.Run),
        ["t127"] = Sync(DemoT127.Run), ["127"] = Sync(Demo127.Run),
        ["t128"] = Sync(DemoT128.Run), ["128"] = Sync(Demo128.Run),
        ["t129"] = Sync(DemoT129.Run), ["129"] = Demo129.Run,
        ["t130"] = Sync(DemoT130.Run), ["130"] = Sync(Demo130.Run),
        ["t131"] = Sync(DemoT131.Run), ["131"] = Sync(Demo131.Run),
        ["t132"] = Sync(DemoT132.Run), ["132"] = Demo132.Run,
        ["t133"] = Sync(DemoT133.Run), ["133"] = Sync(Demo133.Run),
    };

    /// <summary>As chaves de um capítulo, na ordem em que o livro as mostra: primeiro o passo a passo, depois o "Indo além".</summary>
    public static IReadOnlyList<string> DoCapitulo(int capitulo) =>
        new[] { $"t{capitulo}", $"{capitulo}" }.Where(Todos.ContainsKey).ToList();

    public static async Task ExecutarAsync(string chave)
    {
        if (!Todos.TryGetValue(chave, out var exemplo))
            throw new ArgumentException($"Não existe o exemplo \"{chave}\". Use, por exemplo: cap124, t124 ou 124.", nameof(chave));

        await exemplo();
    }
}
