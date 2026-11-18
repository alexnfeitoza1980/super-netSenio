using System.Text;
using Jornada.Joao.Exemplos;

namespace Joao.Testes;

[CollectionDefinition("Console", DisableParallelization = true)]
public class ColecaoDoConsole { }

/// <summary>
/// As saídas mostradas no livro estão na pasta Saidas/. Estes testes rodam cada exemplo de novo e conferem que a saída continua igual:
/// se o código mudar, a saída publicada precisa ser revista de propósito. Para gerar uma saída: dotnet run --project codigo/Joao.Console -- exemplo t124
/// </summary>
[Collection("Console")]
public class ExemplosTests
{
    public static IEnumerable<object[]> Chaves() => Exemplos.Todos.Keys.Select(chave => new object[] { chave });

    [Theory]
    [MemberData(nameof(Chaves))]
    public async Task A_saida_publicada_e_a_que_o_exemplo_produz(string chave)
    {
        var arquivo = Path.Combine(AppContext.BaseDirectory, "Saidas", chave + ".txt");
        var esperado = Normalizar(File.ReadAllText(arquivo, Encoding.UTF8));

        var original = Console.Out;
        var captura = new StringWriter();
        Console.SetOut(captura);
        try { await Exemplos.ExecutarAsync(chave); }
        finally { Console.SetOut(original); }

        Assert.Equal(esperado, Normalizar(captura.ToString()));
    }

    [Fact]
    public void Todo_exemplo_tem_a_sua_saida_publicada()
    {
        var pasta = Path.Combine(AppContext.BaseDirectory, "Saidas");

        Assert.All(Exemplos.Todos.Keys, chave => Assert.True(File.Exists(Path.Combine(pasta, chave + ".txt")), $"falta Saidas/{chave}.txt"));
    }

    [Fact]
    public async Task Exemplo_desconhecido_e_um_erro_claro() =>
        await Assert.ThrowsAsync<ArgumentException>(() => Exemplos.ExecutarAsync("cap999"));

    private static string Normalizar(string texto) => texto.TrimStart('﻿').Replace("\r\n", "\n").TrimEnd();
}
