using Jornada.Joao.Cap131;

namespace Jornada.Joao.Exercicios;

public static class LivroExtensoes
{
    /// <summary>Conta o que aconteceu, um evento por linha: "1. faleceu", "2. a pedra foi tirada"...</summary>
    public static IReadOnlyList<string> Historia(this LivroDeEventos livro) =>
        livro.Eventos
            .Select((evento, i) => $"{i + 1}. " + evento switch
            {
                Faleceu     => "faleceu",
                PedraTirada => "a pedra foi tirada",
                Chamado     => "foi chamado",
                Saiu        => "saiu",
                Desatado d  => $"desatado: {d.Atadura}",
                _           => evento.GetType().Name
            })
            .ToList();
}
