using Jornada.Joao.Cap125;

namespace Jornada.Joao.Exercicios;

public static class ImagemExtensoes
{
    /// <summary>"o bom pastor (Jo 10,11): cuidado". Sem passagem conhecida, mostra "sem passagem".</summary>
    public static string Descrever(this Imagem imagem)
    {
        var onde = imagem.Passagem is { } p ? p.ToString() : "sem passagem";
        return $"{imagem.Texto} ({onde}): {imagem.Promessa}";
    }
}
