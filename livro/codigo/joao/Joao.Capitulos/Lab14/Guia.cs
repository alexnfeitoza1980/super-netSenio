using System.Text.Json;
using Jornada.Joao.Cap124;
using Jornada.Joao.Cap128;

namespace Jornada.Joao.Lab14;

public sealed record Lugar(string Id, string Nome, string Cena);

public sealed record Trecho(string De, string Para, int Dias);

public sealed record Rota(IReadOnlyList<Lugar> Paradas, int Dias)
{
    public int Trechos => Paradas.Count - 1;
}

/// <summary>Um mapa de lugares do Evangelho e dos caminhos entre eles. Os "dias de viagem" são ILUSTRATIVOS.</summary>
public sealed class Mapa
{
    private readonly Dictionary<string, Lugar> lugares;
    private readonly Dictionary<string, List<(string Para, int Dias)>> vizinhos = new();

    public Mapa(IEnumerable<Lugar> lugares, IEnumerable<Trecho> trechos)
    {
        this.lugares = lugares.ToDictionary(l => l.Id);

        foreach (var t in trechos)
        {
            if (!this.lugares.ContainsKey(t.De) || !this.lugares.ContainsKey(t.Para))
                throw new ArgumentException($"Trecho com lugar desconhecido: {t.De} → {t.Para}.");

            Ligar(t.De, t.Para, t.Dias);
            Ligar(t.Para, t.De, t.Dias);              // caminhos de ida e de volta
        }
    }

    public IReadOnlyCollection<Lugar> Lugares => lugares.Values;

    public Lugar Obter(string id) =>
        lugares.TryGetValue(id, out var lugar) ? lugar : throw new KeyNotFoundException($"Não conheço o lugar \"{id}\".");

    private void Ligar(string de, string para, int dias)
    {
        if (!vizinhos.TryGetValue(de, out var lista))
            vizinhos[de] = lista = new();
        lista.Add((para, dias));
    }

    /// <summary>Busca em largura: acha o caminho com MENOS PARADAS, explorando camada por camada.</summary>
    public Rota? MenosParadas(string origem, string destino)
    {
        Obter(origem); Obter(destino);

        var veioDe = new Dictionary<string, string?> { [origem] = null };
        var fila = new Queue<string>();
        fila.Enqueue(origem);

        while (fila.Count > 0)
        {
            var atual = fila.Dequeue();
            if (atual == destino) break;

            foreach (var (proximo, _) in vizinhos.GetValueOrDefault(atual, new()))
            {
                if (veioDe.ContainsKey(proximo)) continue;      // já visitado
                veioDe[proximo] = atual;
                fila.Enqueue(proximo);
            }
        }

        return veioDe.ContainsKey(destino) ? Montar(veioDe, destino) : null;
    }

    /// <summary>Dijkstra: acha o caminho de MENOR TEMPO, usando uma fila de prioridade.</summary>
    public Rota? MenorTempo(string origem, string destino)
    {
        Obter(origem); Obter(destino);

        var distancia = new Dictionary<string, int> { [origem] = 0 };
        var veioDe = new Dictionary<string, string?> { [origem] = null };
        var fila = new PriorityQueue<string, int>();
        fila.Enqueue(origem, 0);

        while (fila.TryDequeue(out var atual, out var custo))
        {
            if (atual == destino) break;
            if (custo > distancia[atual]) continue;              // entrada velha: já achamos algo melhor

            foreach (var (proximo, dias) in vizinhos.GetValueOrDefault(atual, new()))
            {
                var novo = custo + dias;
                if (novo >= distancia.GetValueOrDefault(proximo, int.MaxValue)) continue;

                distancia[proximo] = novo;
                veioDe[proximo] = atual;
                fila.Enqueue(proximo, novo);
            }
        }

        return veioDe.ContainsKey(destino) ? Montar(veioDe, destino) : null;
    }

    private Rota Montar(Dictionary<string, string?> veioDe, string destino)
    {
        var ids = new List<string>();
        for (string? id = destino; id is not null; id = veioDe[id])
            ids.Add(id);
        ids.Reverse();

        int dias = ids.Zip(ids.Skip(1), (a, b) => vizinhos[a].First(v => v.Para == b).Dias).Sum();
        return new Rota(ids.Select(Obter).ToList(), dias);
    }

    private sealed record Arquivo(List<Lugar> Lugares, List<Trecho> Trechos);

    public static Mapa Padrao()
    {
        var arquivo = JsonSerializer.Deserialize<Arquivo>(DadosDoMapa, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!;
        return new Mapa(arquivo.Lugares, arquivo.Trechos);
    }

    private const string DadosDoMapa = """
        {
          "lugares": [
            { "id": "jordao",    "nome": "Além do Jordão",   "cena": "onde João batizava e deu testemunho (Jo 1)" },
            { "id": "cana",      "nome": "Caná da Galileia", "cena": "o casamento: água em vinho (Jo 2)" },
            { "id": "cafarnaum", "nome": "Cafarnaum",        "cena": "o discurso sobre o pão da vida (Jo 6)" },
            { "id": "mar",       "nome": "Mar da Galileia",  "cena": "os pães e os peixes; a travessia à noite (Jo 6)" },
            { "id": "samaria",   "nome": "Sicar, em Samaria","cena": "o poço de Jacó (Jo 4)" },
            { "id": "jerusalem", "nome": "Jerusalém",        "cena": "o templo, Nicodemos, o cego de nascença (Jo 2 a 5, 7 a 9)" },
            { "id": "betania",   "nome": "Betânia",          "cena": "a casa de Marta, Maria e Lázaro (Jo 11)" },
            { "id": "efraim",    "nome": "Éfraim",           "cena": "um retiro antes da última Páscoa (Jo 11)" }
          ],
          "trechos": [
            { "de": "jordao",    "para": "cana",      "dias": 3 },
            { "de": "jordao",    "para": "jerusalem", "dias": 2 },
            { "de": "cana",      "para": "cafarnaum", "dias": 1 },
            { "de": "cafarnaum", "para": "mar",       "dias": 1 },
            { "de": "cana",      "para": "samaria",   "dias": 3 },
            { "de": "samaria",   "para": "jerusalem", "dias": 3 },
            { "de": "cafarnaum", "para": "jerusalem", "dias": 5 },
            { "de": "jerusalem", "para": "betania",   "dias": 1 },
            { "de": "jerusalem", "para": "efraim",    "dias": 1 },
            { "de": "betania",   "para": "efraim",    "dias": 1 }
          ]
        }
        """;
}

/// <summary>O guia junta peças de capítulos anteriores: registra com a lanterna (124) e guarda rotas no poço (128).</summary>
public sealed class Guia
{
    private readonly Mapa mapa;
    private readonly ILanterna lanterna;
    private readonly Poco<(string, string), Rota?> poco;

    public Guia(Mapa mapa, ILanterna lanterna, TimeProvider? relogio = null)
    {
        this.mapa = mapa;
        this.lanterna = lanterna;
        poco = new Poco<(string, string), Rota?>(par => mapa.MenorTempo(par.Item1, par.Item2), TimeSpan.FromMinutes(10), relogio);
    }

    public long RotasReaproveitadas => poco.Acertos;

    public Rota? Rota(string origem, string destino)
    {
        using var _ = lanterna.Escopo($"rota {origem} → {destino}");
        var rota = poco.Obter((origem, destino));
        lanterna.Escrever(Nivel.Informacao, rota is null ? "sem caminho" : "rota pronta",
            ("dias", rota?.Dias), ("reaproveitadas", poco.Acertos));
        return rota;
    }
}
