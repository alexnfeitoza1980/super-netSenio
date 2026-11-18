namespace Jornada.Resiliencia.Cap89;

public static class TextoDaPausa
{
    public static string Descrever(PausaConsciente pausa)
    {
        if (pausa.Terminou)
            return "Pausa concluída. Você decide o que fazer agora.";

        var restante = pausa.Restante;
        return $"Faltam {(int)restante.TotalMinutes} min e {restante.Seconds} s.";
    }
}
