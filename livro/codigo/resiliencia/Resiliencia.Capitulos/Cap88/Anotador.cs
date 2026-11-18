namespace Jornada.Resiliencia.Cap88;

public sealed class Anotador
{
    public int Tentativas { get; private set; }

    /// <summary>Versão com exceções: tenta converter e trata cada tipo de falha.</summary>
    public string AnotarHorasDeSono(string texto)
    {
        try
        {
            int horas = int.Parse(texto);                       // pode lançar FormatException
            if (horas is < 0 or > 24)
                throw new ArgumentOutOfRangeException(nameof(texto), "Use um valor entre 0 e 24.");

            return $"{horas} h anotadas.";
        }
        catch (FormatException)
        {
            return "Não entendi o número. Tente algo como 7.";
        }
        catch (ArgumentOutOfRangeException)
        {
            return "Esse valor não cabe em um dia. Use de 0 a 24.";
        }
        finally
        {
            Tentativas++;                                        // roda sempre, deu certo ou não
        }
    }

    /// <summary>Versão sem exceções: falha esperada vira um resultado, não um susto.</summary>
    public string AnotarComTryParse(string texto)
    {
        Tentativas++;

        if (!int.TryParse(texto, out int horas))
            return "Não entendi o número. Tente algo como 7.";

        return horas is < 0 or > 24
            ? "Esse valor não cabe em um dia. Use de 0 a 24."
            : $"{horas} h anotadas.";
    }
}
