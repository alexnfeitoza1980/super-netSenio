using System.Security.Cryptography;
using System.Text;

namespace Jornada.Joao.Cap130;

public sealed record Identidade(string Nome, string Perfil, DateTimeOffset ExpiraEm);

public enum Acesso { Permitido, Malformado, Estranho, Expirado, SemPermissao }

/// <summary>
/// A "voz": um pedaço de texto que só quem tem o segredo consegue produzir e que qualquer um com o segredo confere.
/// Formato: corpo.assinatura (ambos em base64url). Material de estudo: em produção, use bibliotecas consolidadas (como JWT).
/// </summary>
public sealed class Voz
{
    private readonly byte[] segredo;
    private readonly TimeProvider relogio;

    public Voz(byte[] segredo, TimeProvider? relogio = null)
    {
        if (segredo.Length < 16)
            throw new ArgumentException("O segredo precisa ter ao menos 16 bytes.", nameof(segredo));

        this.segredo = segredo;
        this.relogio = relogio ?? TimeProvider.System;
    }

    public string Emitir(string nome, string perfil, TimeSpan validade)
    {
        if (nome.Contains('|') || perfil.Contains('|'))
            throw new ArgumentException("Nome e perfil não podem conter o caractere '|'.");

        var expira = relogio.GetUtcNow().Add(validade).ToUnixTimeSeconds();
        var corpo = CodificarBase64Url(Encoding.UTF8.GetBytes($"{nome}|{perfil}|{expira}"));

        return $"{corpo}.{CodificarBase64Url(Assinar(corpo))}";
    }

    public Acesso Reconhecer(string? token, out Identidade? identidade)
    {
        identidade = null;

        var partes = token?.Split('.');
        if (partes is not { Length: 2 })
            return Acesso.Malformado;

        byte[] enviada;
        try { enviada = DecodificarBase64Url(partes[1]); }
        catch (FormatException) { return Acesso.Malformado; }

        // Comparação em tempo constante: o tempo gasto não revela quantos bytes já "bateram".
        if (!CryptographicOperations.FixedTimeEquals(Assinar(partes[0]), enviada))
            return Acesso.Estranho;                      // a voz não é a que conhecemos

        string[] campos;
        try { campos = Encoding.UTF8.GetString(DecodificarBase64Url(partes[0])).Split('|'); }
        catch (FormatException) { return Acesso.Malformado; }

        if (campos.Length != 3 || !long.TryParse(campos[2], out var segundos))
            return Acesso.Malformado;

        var expiraEm = DateTimeOffset.FromUnixTimeSeconds(segundos);
        if (relogio.GetUtcNow() >= expiraEm)
            return Acesso.Expirado;

        identidade = new Identidade(campos[0], campos[1], expiraEm);
        return Acesso.Permitido;
    }

    private byte[] Assinar(string dados) => HMACSHA256.HashData(segredo, Encoding.UTF8.GetBytes(dados));

    private static string CodificarBase64Url(byte[] bytes) =>
        Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');

    private static byte[] DecodificarBase64Url(string texto)
    {
        var padrao = texto.Replace('-', '+').Replace('_', '/');
        padrao = padrao.PadRight(padrao.Length + (4 - padrao.Length % 4) % 4, '=');
        return Convert.FromBase64String(padrao);
    }
}

/// <summary>A porta: só deixa entrar quem tem uma voz reconhecida, e que passe na política desta porta.</summary>
public sealed class Porta
{
    private readonly Voz voz;

    public Porta(Voz voz) => this.voz = voz;

    public Acesso Entrar(string? token, Func<Identidade, bool>? politica = null)
    {
        var acesso = voz.Reconhecer(token, out var identidade);
        if (acesso != Acesso.Permitido)
            return acesso;

        return politica is null || politica(identidade!) ? Acesso.Permitido : Acesso.SemPermissao;
    }
}
