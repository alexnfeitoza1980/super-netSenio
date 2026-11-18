using System.Globalization;

namespace Jornada.Habitos.Cap114;

/// <summary>
/// Um valor que só existe se for válido: de 0 a 100. Quem recebe um Percentual não precisa conferir nada.
/// A restrição vira liberdade: o resto do código fica mais simples.
/// </summary>
public readonly record struct Percentual : IParsable<Percentual>
{
    public double Valor { get; }

    public Percentual(double valor)
    {
        if (double.IsNaN(valor) || valor is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(valor), "Um percentual vai de 0 a 100.");

        Valor = valor;
    }

    /// <summary>Conversão automática: onde se espera um double, um Percentual serve.</summary>
    public static implicit operator double(Percentual p) => p.Valor;

    public static Percentual Parse(string s, IFormatProvider? provider)
    {
        if (!TryParse(s, provider, out var resultado))
            throw new FormatException($"\"{s}\" não é um percentual válido (use um número de 0 a 100).");
        return resultado;
    }

    public static bool TryParse(string? s, IFormatProvider? provider, out Percentual resultado)
    {
        resultado = default;
        if (s is null) return false;

        var texto = s.Trim().TrimEnd('%').Trim();
        if (!double.TryParse(texto, NumberStyles.Float, provider ?? CultureInfo.InvariantCulture, out var numero))
            return false;
        if (double.IsNaN(numero) || numero is < 0 or > 100)
            return false;

        resultado = new Percentual(numero);
        return true;
    }

    public override string ToString() => $"{Valor.ToString("0.##", CultureInfo.InvariantCulture)}%";
}

/// <summary>Um e-mail "bom o bastante": a classe controla a própria criação e nunca existe inválida.</summary>
public sealed class Email
{
    public string Endereco { get; }

    private Email(string endereco) => Endereco = endereco;

    public static bool TryCriar(string? texto, out Email? email)
    {
        email = null;
        if (string.IsNullOrWhiteSpace(texto)) return false;

        var limpo = texto.Trim();
        int arroba = limpo.IndexOf('@');
        bool formatoOk = arroba > 0 && arroba == limpo.LastIndexOf('@') && limpo.IndexOf('.', arroba) > arroba + 1
                         && !limpo.EndsWith('.') && !limpo.Contains(' ');
        if (!formatoOk) return false;

        email = new Email(limpo.ToLowerInvariant());
        return true;
    }

    public override string ToString() => Endereco;
}

/// <summary>Uma divisão do tempo (ou do esforço) que fecha 100%: impossível criar uma que não feche.</summary>
public sealed class Distribuicao
{
    private readonly IReadOnlyList<(string Nome, Percentual Parte)> partes;

    private Distribuicao(IReadOnlyList<(string, Percentual)> partes) => this.partes = partes;

    public IReadOnlyList<(string Nome, Percentual Parte)> Partes => partes;

    public static Distribuicao Criar(params (string Nome, Percentual Parte)[] partes)
    {
        if (partes.Length == 0)
            throw new ArgumentException("Informe ao menos uma parte.", nameof(partes));

        double soma = partes.Sum(p => p.Parte.Valor);
        if (Math.Abs(soma - 100) > 0.001)
            throw new ArgumentException($"As partes somam {soma:0.##}%, e precisam somar 100%.", nameof(partes));

        return new Distribuicao(partes);
    }

    /// <summary>Converte "Foco 50, Reuniões 30, Aprender 20" em partes. Usa o Parse genérico abaixo.</summary>
    public static Distribuicao Ler(string texto) =>
        Criar(texto.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                   .Select(item =>
                   {
                       int espaco = item.LastIndexOf(' ');
                       if (espaco < 0) throw new FormatException($"Use \"Nome valor\", e não \"{item}\".");
                       return (item[..espaco].Trim(), Leitor.Ler<Percentual>(item[(espaco + 1)..]));
                   })
                   .ToArray());
}

public static class Leitor
{
    /// <summary>Genérico com restrição estática: qualquer T que saiba se "parsear" (IParsable) serve.</summary>
    public static T Ler<T>(string texto) where T : IParsable<T> => T.Parse(texto, null);

    public static bool TentarLer<T>(string? texto, out T? resultado) where T : IParsable<T> =>
        T.TryParse(texto, null, out resultado);
}
