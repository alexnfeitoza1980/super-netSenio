using System.Text.Json;
using System.Text.Json.Serialization;
using Jornada.Habitos.Cap108;
using Jornada.Habitos.Cap112;

namespace Jornada.Habitos.Lab12;

/// <summary>Os dados de UMA semana, como a pessoa os escreveu num arquivo JSON dela. Nada sai do computador.</summary>
public sealed record DiarioDaSemana(
    IReadOnlyList<Prioridade>? Prioridades,
    IReadOnlyList<Registro>? Registros,
    IReadOnlyList<Pratica>? Praticas);

public static class Carregador
{
    private static readonly JsonSerializerOptions Opcoes = new()
    {
        PropertyNameCaseInsensitive = true,          // "area" ou "Area": tanto faz
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        Converters = { new JsonStringEnumConverter() }  // "Alinhar" em vez de 0; "Monday" em vez de 1
    };

    /// <summary>Lê o JSON. Arquivo escrito à mão erra com facilidade, então a falha é esperada: devolvemos um aviso claro.</summary>
    public static bool TentarCarregar(string json, out DiarioDaSemana? diario, out string? problema)
    {
        diario = null;
        problema = null;

        try
        {
            diario = JsonSerializer.Deserialize<DiarioDaSemana>(json, Opcoes);
            if (diario is null)
                problema = "O arquivo está vazio.";
        }
        catch (JsonException e)
        {
            problema = $"Não consegui ler o arquivo (linha {e.LineNumber! + 1}): confira vírgulas, aspas e nomes.";
        }

        return diario is not null;
    }

    public static string Exemplo => """
        {
          // Tudo aqui é só um exemplo. Troque pelos seus dados, ou apague o que preferir não dizer.
          "prioridades": [
            { "area": "Família", "ordem": 1 },
            { "area": "Saúde",   "ordem": 2 },
            { "area": "Estudo",  "ordem": 3 },
            { "area": "Trabalho","ordem": 4 }
          ],
          "registros": [
            { "dia": "2026-11-02", "area": "Trabalho",      "minutos": 540 },
            { "dia": "2026-11-02", "area": "Redes sociais", "minutos": 80 },
            { "dia": "2026-11-03", "area": "Trabalho",      "minutos": 520 },
            { "dia": "2026-11-03", "area": "Estudo",        "minutos": 45 },
            { "dia": "2026-11-04", "area": "Família",       "minutos": 90 },
            { "dia": "2026-11-04", "area": "Redes sociais", "minutos": 100 },
            { "dia": "2026-11-05", "area": "Saúde",         "minutos": 30 }
          ],
          "praticas": [
            { "nome": "Daily", "intencao": "Alinhar", "minutos": 15, "dias": ["Monday", "Tuesday", "Wednesday", "Thursday", "Friday"] },
            { "nome": "Demo",  "intencao": "Entregar", "minutos": 45, "dias": ["Friday"] }
          ]
        }
        """;
}

public static class Auditor
{
    /// <summary>
    /// Monta o relatório juntando as peças dos capítulos 108 (o que se diz × o que se faz)
    /// e 104 (as histórias que os ritmos contam). Só observa: não dá nota, não compara pessoas.
    /// </summary>
    public static IReadOnlyList<string> Auditar(DiarioDaSemana diario)
    {
        var linhas = new List<string>();

        if (diario.Prioridades is { Count: > 0 } prioridades && diario.Registros is { Count: > 0 } registros)
        {
            var relatorio = Revelacao.Comparar(prioridades, registros);

            linhas.Add($"Tempo registrado: {relatorio.TotalDeMinutos / 60.0:0.#} h.");
            linhas.AddRange(relatorio.Declaradas.Select(Revelacao.Descrever));

            foreach (var area in relatorio.NaoDeclaradas)
                linhas.Add($"{area.Area}: apareceu em {area.Parte:P0} do tempo sem estar na sua lista. Quer dar um nome (e um lugar) a isso?");
        }
        else
        {
            linhas.Add("Sem prioridades ou registros nesta semana: tudo bem, pule esta parte.");
        }

        if (diario.Praticas is { Count: > 0 } praticas)
        {
            var calendario = praticas.Aggregate(new CalendarioDoTime(), (c, p) => c.Adicionar(p));

            foreach (var (intencao, minutos) in calendario.MinutosPorSemana().OrderByDescending(p => p.Value))
                linhas.Add($"Ritmo: {intencao} recebe {minutos} min por semana.");

            if (calendario.Lacunas() is { Count: > 0 } lacunas)
                linhas.Add($"Nenhum ritmo cuida de: {string.Join(", ", lacunas)}. Quer incluir algo pequeno?");
        }

        linhas.Add("Pergunta para levar: qual prática pequena você repetiria esta semana, sabendo o que ela vai formar?");
        return linhas;
    }
}
