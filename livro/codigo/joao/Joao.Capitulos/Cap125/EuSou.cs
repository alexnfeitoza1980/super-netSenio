using System.Reflection;

namespace Jornada.Joao.Cap125;

/// <summary>
/// Um atributo é uma etiqueta que se cola em um tipo e que pode ser lida em tempo de execução.
/// Este diz: "eu sou isto", com a imagem, onde ela aparece no texto e o que promete.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class EuSouAttribute : Attribute
{
    public EuSouAttribute(string imagem) => Imagem = imagem;

    public string Imagem { get; }

    /// <summary>Onde a imagem aparece, no formato "Jo 6,35".</summary>
    public string Referencia { get; init; } = "";

    public string Promessa { get; init; } = "";
}

public interface IImagem
{
    string Promete();
}

/// <summary>Uma referência de capítulo e versículo, que sabe se comparar para ser colocada em ordem.</summary>
public readonly record struct Passagem(int Capitulo, int Versiculo) : IComparable<Passagem>
{
    public static bool TryParse(string? texto, out Passagem passagem)
    {
        passagem = default;
        if (string.IsNullOrWhiteSpace(texto)) return false;

        var partes = texto.Trim().Split(' ', 2, StringSplitOptions.TrimEntries);
        if (partes.Length != 2 || partes[0] != "Jo") return false;

        var numeros = partes[1].Split(',', StringSplitOptions.TrimEntries);
        if (numeros.Length != 2 || !int.TryParse(numeros[0], out var c) || !int.TryParse(numeros[1], out var v))
            return false;

        passagem = new Passagem(c, v);
        return true;
    }

    public int CompareTo(Passagem outra) =>
        Capitulo != outra.Capitulo ? Capitulo.CompareTo(outra.Capitulo) : Versiculo.CompareTo(outra.Versiculo);

    public override string ToString() => $"Jo {Capitulo},{Versiculo}";
}

public sealed record Imagem(Type Tipo, string Texto, Passagem? Passagem, string Promessa);

/// <summary>Procura, por reflexão, todos os tipos marcados com [EuSou] e os coloca na ordem do texto.</summary>
public static class Catalogo
{
    public static IReadOnlyList<Imagem> Descobrir(Assembly assembly) =>
        assembly.GetTypes()
            .Where(t => t is { IsClass: true, IsAbstract: false } && typeof(IImagem).IsAssignableFrom(t))
            .Select(t => (Tipo: t, Marca: t.GetCustomAttribute<EuSouAttribute>()))
            .Where(x => x.Marca is not null)
            .Select(x => new Imagem(
                x.Tipo,
                x.Marca!.Imagem,
                Passagem.TryParse(x.Marca.Referencia, out var p) ? p : null,
                x.Marca.Promessa))
            .OrderBy(i => i.Passagem ?? new Passagem(int.MaxValue, 0))
            .ToList();

    /// <summary>Cria uma instância sem que o código conheça o tipo concreto na hora de escrever.</summary>
    public static IImagem Criar(Imagem imagem) => (IImagem)Activator.CreateInstance(imagem.Tipo)!;
}
