---
id: joao-lab14
order: 172
kind: lab
num: 14
group: C# & Mentalidade · Parte 18 · O Evangelho segundo João em C#
title: "Laboratório: Concordância e roteiro"
subtitle: Um programa de console que consulta dados reais do texto grego de João (onde cada palavra aparece, e com que densidade) e traça rotas entre os lugares do Evangelho com busca em largura e Dijkstra.
minutes: 110
hue: 220
objectives:
- **Reunir** peças dos capítulos 124 (`Lanterna`) e 128 (`Poco`) em um guia que registra e guarda rotas
- Ler **dados reais** embutidos no programa (recurso incorporado + `System.Text.Json`) e consultá-los com **LINQ**
- Calcular **densidade** e **picos** para comparar capítulos de tamanhos diferentes
- Implementar **busca em largura** (menos paradas) e **Dijkstra** com `PriorityQueue` (menor tempo)
origem: livros/10-evangelho-joao-csharp/content/140-lab14.md
---

::: ilustracao img/joao/lab14.svg | O laboratório junta o mapa (busca em largura e Dijkstra) e a concordância (onde a palavra aparece, e com que densidade).
:::

## O que vamos construir

Em cada capítulo, a seção *Lupa no grego* trouxe números: "*pisteuō* aparece 98 vezes", "*menō*, 11 vezes só no capítulo 15". Agora você pode **conferir tudo sozinho**. O laboratório reúne dois programinhas em um:

1. **Concordância.** Uma *concordância* é um índice de palavras: dado um termo, diz **onde ele aparece**. A nossa lê os dados do **texto grego de João** (15.438 palavras, contadas por lema) e responde: as palavras mais frequentes, onde uma palavra se concentra, qual capítulo é o mais "denso" nela, e as 24 frases "Eu sou".
2. **Roteiro.** Um mapa dos **lugares** do Evangelho, com os caminhos entre eles, e dois jeitos de achar a rota: a que tem **menos paradas** (busca em largura) e a de **menor tempo** (Dijkstra).

Duas regras guiam o desenho:

- **Dados conferíveis.** Os números de palavras vêm do **SBLGNT/MorphGNT**; os testes fixam os totais (98, 40, 24...) para que qualquer erro de digitação apareça.
- **Honestidade sobre o que é ilustrativo.** Os **dias de viagem** do mapa são **inventados para ensinar o algoritmo**; eles não são distâncias históricas, e o programa avisa isso na saída.

::: cuidado Sobre os dados
O texto grego usado é o **SBLGNT** (*SBL Greek New Testament*, licença CC BY 4.0), com anotações morfológicas do **MorphGNT** (CC BY-SA 3.0). O arquivo `lemas.json` do projeto **não contém o texto**, apenas **contagens por lema e por capítulo** derivadas dele, e o crédito está no README. O SBLGNT **não traz Jo 7,53 a 8,11** no corpo do texto; por isso as contagens não incluem esse trecho (e o total de 15.438 palavras também não). Um *lema* é a forma de dicionário de uma palavra (*lēmma*): "ἀγαπᾷ", "ἠγάπησεν" e "ἀγαπᾶτε" contam como **ἀγαπάω**.
:::

O código está em [codigo/joao/Joao.Console](codigo/joao/Joao.Console/) (o `Program.cs`) e [codigo/joao/Joao.Capitulos/Lab14](codigo/joao/Joao.Capitulos/Lab14/) (a lógica). Rode com o .NET 8 SDK:

```bash
dotnet run --project codigo/joao/Joao.Console -- topo 8
dotnet run --project codigo/joao/Joao.Console -- onde menō
dotnet run --project codigo/joao/Joao.Console -- rota jordao betania
```

::: figura Como as peças se encaixam
<svg viewBox="0 0 760 260" role="img" aria-label="O arquivo lemas.json embutido alimenta a Concordância; o mapa de lugares alimenta o Guia, que usa a Lanterna do capítulo 124 e o Poço do capítulo 128; o programa de console exibe tudo na tela">
  <rect x="14" y="30" width="150" height="60" rx="12" class="f-warn"/><text x="89" y="56" text-anchor="middle" font-weight="800">lemas.json</text><text x="89" y="74" text-anchor="middle" font-size="11" class="t-dim">recurso embutido</text>
  <rect x="200" y="30" width="170" height="60" rx="12" class="f-info"/><text x="285" y="56" text-anchor="middle" font-weight="800">Concordancia</text><text x="285" y="74" text-anchor="middle" font-size="11" class="t-dim">LINQ · densidade · pico</text>
  <rect x="14" y="170" width="150" height="60" rx="12" class="f-warn"/><text x="89" y="196" text-anchor="middle" font-weight="800">Mapa (JSON)</text><text x="89" y="214" text-anchor="middle" font-size="11" class="t-dim">8 lugares · 10 caminhos</text>
  <rect x="200" y="170" width="170" height="60" rx="12" class="f-info"/><text x="285" y="196" text-anchor="middle" font-weight="800">Mapa</text><text x="285" y="214" text-anchor="middle" font-size="11" class="t-dim">BFS · Dijkstra</text>
  <rect x="410" y="170" width="150" height="60" rx="12" class="f-prim"/><text x="485" y="196" text-anchor="middle" font-weight="800">Guia</text><text x="485" y="214" text-anchor="middle" font-size="11" class="t-dim">junta e lembra</text>
  <rect x="410" y="96" width="150" height="40" rx="10" class="f-good" opacity=".75"/><text x="485" y="121" text-anchor="middle" font-size="12">Cap. 124 · Lanterna</text>
  <rect x="590" y="170" width="160" height="40" rx="10" class="f-good" opacity=".75"/><text x="670" y="195" text-anchor="middle" font-size="12">Cap. 128 · Poço</text>
  <rect x="590" y="30" width="160" height="60" rx="12" class="f-good"/><text x="670" y="56" text-anchor="middle" font-weight="800">Tela (Console)</text><text x="670" y="74" text-anchor="middle" font-size="11" class="t-dim">nada gravado, nada enviado</text>
  <path class="arrow" d="M166 60H198"/><path class="arrow" d="M372 60H588"/><path class="arrow" d="M166 200H198"/><path class="arrow" d="M372 200H408"/>
  <path class="arrow" d="M485 138V168"/><path class="arrow" d="M562 192H588"/>
</svg>
:::

## Passo 1: dados reais como recurso embutido

Em vez de ler um arquivo do disco (que pode não estar lá), o `lemas.json` é um **recurso incorporado** ao programa. No `.csproj`:

```xml
<EmbeddedResource Include="Dados\lemas.json" LogicalName="lemas.json" />
```

e o código o abre com `Assembly.GetExecutingAssembly().GetManifestResourceStream("lemas.json")`. O formato do arquivo é este (uma entrada, resumida):

```json
{ "grego": "λόγος", "translit": "logos", "sentido": "palavra, razão", "total": 40,
  "porCapitulo": { "1": 4, "2": 1, "4": 4, "5": 2, "6": 1, "...": 0 } }
```

Cada lema guarda o **total** e as ocorrências **por capítulo**. Para que o `System.Text.Json` crie `record`s direto do JSON, os nomes das propriedades do `record` casam com os do arquivo (`PropertyNameCaseInsensitive`).

## Passo 2: a concordância

O coração é `Concordancia`. Veja o que cada método faz:

- **`Buscar(termo)`** aceita **grego, transliteração ou sentido**, sem ligar para maiúsculas: `menō`, `permanecer` e `μένω` chegam ao mesmo lema.
- **`MaisFrequentes(n)`** ordena por total (e, no empate, pelo nome) com `OrderByDescending` + `ThenBy` + `Take`.
- **`Densidade(lema, capitulo)`** divide as ocorrências pelas palavras do capítulo e multiplica por mil: **ocorrências a cada mil palavras**. É assim que se **compara** um capítulo curto com um longo sem se enganar com o tamanho.
- **`Pico(lema)`** usa `MaxBy` para achar o capítulo de maior densidade. Se a palavra nunca aparece, devolve `null` (e não um capítulo qualquer).
- **`Barras(lema)`** desenha um gráfico de barras em **texto**, uma linha por capítulo, com a maior barra ocupando a largura toda.

```cs Jornada/Joao/Lab14/Concordancia.cs
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jornada.Joao.Lab14;

public sealed record Lema(
    string Grego,
    string Translit,
    string Sentido,
    int Total,
    IReadOnlyDictionary<string, int> PorCapitulo)
{
    public int Em(int capitulo) => PorCapitulo.GetValueOrDefault(capitulo.ToString());
}

public sealed record EuSouNoTexto(int Capitulo, int Versiculo, string? Predicado)
{
    public override string ToString() => $"Jo {Capitulo},{Versiculo}" + (Predicado is null ? " (uso absoluto ou outra construção)" : $": {Predicado}");
}

public sealed record DadosDoTexto(
    string Obra,
    string Fonte,
    IReadOnlyDictionary<string, int> PalavrasPorCapitulo,
    IReadOnlyList<Lema> Lemas,
    IReadOnlyList<EuSouNoTexto> EuSou)
{
    public int TotalDePalavras => PalavrasPorCapitulo.Values.Sum();
}

/// <summary>Uma concordância: onde cada palavra do original aparece, e com que "densidade" em cada capítulo.</summary>
public sealed class Concordancia
{
    private static readonly JsonSerializerOptions Opcoes = new() { PropertyNameCaseInsensitive = true };

    private readonly DadosDoTexto dados;

    public Concordancia(DadosDoTexto dados) => this.dados = dados;

    /// <summary>Lê os dados embutidos no programa (recurso incorporado), sem depender de nenhum arquivo externo.</summary>
    public static Concordancia Embutida()
    {
        using var fluxo = Assembly.GetExecutingAssembly().GetManifestResourceStream("lemas.json")
            ?? throw new InvalidOperationException("O recurso lemas.json não foi encontrado no programa.");

        var dados = JsonSerializer.Deserialize<DadosDoTexto>(fluxo, Opcoes)
            ?? throw new InvalidOperationException("Os dados estão vazios.");

        return new Concordancia(dados);
    }

    public DadosDoTexto Dados => dados;

    /// <summary>Procura por qualquer pedaço do original, da transliteração ou do sentido, sem se importar com maiúsculas.</summary>
    public Lema? Buscar(string termo) =>
        dados.Lemas.FirstOrDefault(l =>
            l.Grego == termo ||
            l.Translit.Equals(termo, StringComparison.OrdinalIgnoreCase) ||
            l.Sentido.Contains(termo, StringComparison.OrdinalIgnoreCase));

    public IReadOnlyList<Lema> MaisFrequentes(int quantos) =>
        dados.Lemas.OrderByDescending(l => l.Total).ThenBy(l => l.Translit).Take(quantos).ToList();

    /// <summary>Ocorrências a cada mil palavras do capítulo: permite comparar capítulos de tamanhos diferentes.</summary>
    public double Densidade(Lema lema, int capitulo)
    {
        var palavras = dados.PalavrasPorCapitulo.GetValueOrDefault(capitulo.ToString());
        return palavras == 0 ? 0 : lema.Em(capitulo) * 1000.0 / palavras;
    }

    /// <summary>O capítulo onde a palavra é mais densa. Sem nenhuma ocorrência, não há pico (null).</summary>
    public (int Capitulo, double Densidade)? Pico(Lema lema)
    {
        if (lema.Total == 0) return null;

        return dados.PalavrasPorCapitulo.Keys
            .Select(k => int.Parse(k))
            .Select(c => (Capitulo: c, Densidade: Densidade(lema, c)))
            .MaxBy(x => x.Densidade);
    }

    /// <summary>Que fatia das ocorrências da palavra está concentrada em um único capítulo?</summary>
    public double Concentracao(Lema lema, int capitulo) => lema.Total == 0 ? 0 : (double)lema.Em(capitulo) / lema.Total;

    /// <summary>Um gráfico de barras em texto, um capítulo por linha.</summary>
    public string Barras(Lema lema, int largura = 30)
    {
        int maximo = Math.Max(1, Enumerable.Range(1, 21).Max(lema.Em));
        var texto = new StringBuilder();

        for (int capitulo = 1; capitulo <= 21; capitulo++)
        {
            int n = lema.Em(capitulo);
            texto.AppendLine($"{capitulo,2} {new string('█', n * largura / maximo)}{(n > 0 ? " " + n : "")}");
        }

        return texto.ToString().TrimEnd();
    }
}
```

Repare em dois cuidados. O `Embutida()` **falha com mensagem clara** se o recurso não existir (`?? throw new InvalidOperationException(...)`). E `Densidade` evita a divisão por zero quando o capítulo não tem palavras registradas.

## Passo 3: o `Program.cs` e a conversa com os dados

O programa escolhe o comando pelo primeiro argumento (`topo`, `onde`, `eusou`, `lugares`, `rota`) e imprime. O `switch` com `when` decide, por exemplo, que `onde` só vale se houver uma palavra depois.

```cs Joao.Console/Program.cs
using Jornada.Joao.Cap124;
using Jornada.Joao.Lab14;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var concordancia = Concordancia.Embutida();
var mapa = Mapa.Padrao();

string comando = args.Length > 0 ? args[0].ToLowerInvariant() : "ajuda";

switch (comando)
{
    case "topo":
        int quantos = args.Length > 1 && int.TryParse(args[1], out var n) ? n : 8;
        foreach (var l in concordancia.MaisFrequentes(quantos))
            Console.WriteLine($"{l.Grego,-10} {l.Translit,-10} {l.Total,4}  {l.Sentido}");
        break;

    case "onde" when args.Length > 1:
        var lema = concordancia.Buscar(args[1]);
        if (lema is null) { Console.WriteLine($"Não achei \"{args[1]}\". Tente: pisteuō, menō, logos, phōs..."); return 1; }

        Console.WriteLine($"{lema.Grego} ({lema.Translit}): {lema.Sentido}. {lema.Total} vezes em {concordancia.Dados.TotalDePalavras} palavras.");
        Console.WriteLine(concordancia.Barras(lema));
        if (concordancia.Pico(lema) is { } pico)
            Console.WriteLine($"Mais denso no capítulo {pico.Capitulo}: {pico.Densidade:0.0} a cada mil palavras.");
        break;

    // ... "eusou", "rota", "lugares" e a ajuda
}

return 0;
```

Os comandos, um por um. Primeiro, **as palavras mais frequentes** (`topo 8`). A lista é das 66 palavras que o livro acompanha (as das "Lupas no grego"), e não de todo o vocabulário:

```saida
πατήρ      patēr       136  pai
πιστεύω    pisteuō      98  crer, confiar
οἶδα       oida         80  saber
κόσμος     kosmos       78  mundo
ὁράω       horaō        72  ver
γινώσκω    ginōskō      56  conhecer
λόγος      logos        40  palavra, razão
μένω       menō         40  permanecer, ficar
```

*Patēr* ("pai", 136 vezes) lidera: um Evangelho em que a relação **pai e filho** é o fio do vocabulário. *Pisteuō* ("crer", 98) vem em seguida. Agora, onde *menō* ("permanecer") se concentra:

```saida
μένω (menō): permanecer, ficar. 40 vezes em 15438 palavras.
 1 █████████████ 5
 2 ██ 1
 3 ██ 1
 4 █████ 2
 5 ██ 1
 6 █████ 2
 7 ██ 1
 8 ████████ 3
 9 ██ 1
10 ██ 1
11 █████ 2
12 ████████ 3
13 
14 ████████ 3
15 ██████████████████████████████ 11
16 
17 
18 
19 ██ 1
20 
21 █████ 2
Mais denso no capítulo 15: 22,0 a cada mil palavras.
```

O gráfico conta o que o capítulo 133 prometeu: **11 das 40 ocorrências estão no capítulo 15**, e a densidade lá é **22 por mil palavras**, a maior. Compare com *pisteuō*, que se espalha pelo Evangelho todo:

```saida
πιστεύω (pisteuō): crer, confiar. 98 vezes em 15438 palavras.
 1 ██████████ 3
 2 █████████████ 4
 3 ██████████████████████████ 8
 4 ███████████████████████ 7
 5 ███████████████████████ 7
 6 ██████████████████████████████ 9
 7 ████████████████ 5
 8 ████████████████ 5
 9 █████████████ 4
10 ████████████████████ 6
11 ██████████████████████████████ 9
12 ██████████████████████████████ 9
13 ███ 1
14 ███████████████████████ 7
15 
16 █████████████ 4
17 ██████████ 3
18 
19 ███ 1
20 ████████████████████ 6
21
Mais denso no capítulo 3: 12,2 a cada mil palavras.
```

Olhe os **buracos**: *pisteuō* não aparece nos capítulos **15, 18 e 21**. E o pico de densidade é o capítulo **3** (12,2 por mil), embora os capítulos 6, 11 e 12 tenham mais ocorrências **absolutas** (9 cada), porque são bem mais longos. É por isso que existe a densidade. Mais um exemplo, a **luz** do capítulo 124:

```saida
φῶς (phōs): luz. 23 vezes em 15438 palavras.
 1 ██████████████████████████████ 6
 2 
 3 █████████████████████████ 5
 4 
 5 █████ 1
 6 
 7 
 8 ██████████ 2
 9 █████ 1
10 
11 ██████████ 2
12 ██████████████████████████████ 6
13 
14 
15 
16 
17 
18 
19 
20 
21
Mais denso no capítulo 3: 7,6 a cada mil palavras.
```

Seis ocorrências no capítulo 1 (o prólogo) e seis no 12 (o último discurso público), mas o **pico de densidade** está no capítulo 3 (a conversa com Nicodemos, capítulo 127). E por fim, **as 24 frases "Eu sou"** (cap. 125):

```saida
Jo 4,26 (uso absoluto ou outra construção)
Jo 6,20 (uso absoluto ou outra construção)
Jo 6,35: o pão da vida
Jo 6,41: o pão que desceu
Jo 6,48: o pão da vida
Jo 6,51: o pão vivo
Jo 8,12: a luz do mundo
Jo 8,18 (uso absoluto ou outra construção)
Jo 8,24 (uso absoluto ou outra construção)
Jo 8,28 (uso absoluto ou outra construção)
Jo 8,58 (uso absoluto ou outra construção)
Jo 9,9 (uso absoluto ou outra construção)
Jo 10,7: a porta das ovelhas
Jo 10,9: a porta
Jo 10,11: o bom pastor
Jo 10,14: o bom pastor
Jo 11,25: a ressurreição e a vida
Jo 13,19 (uso absoluto ou outra construção)
Jo 14,6: o caminho, a verdade e a vida
Jo 15,1: a videira verdadeira
Jo 15,5: a videira
Jo 18,5 (uso absoluto ou outra construção)
Jo 18,6 (uso absoluto ou outra construção)
Jo 18,8 (uso absoluto ou outra construção)
```

Treze têm um predicado, que são as imagens do capítulo 125; as outras onze ficam sem predicado: são os "sou eu" (4,26; 6,20; 9,9), o "sou eu quem testemunha" (8,18) e o "Eu sou" absoluto (8,24.28.58; 13,19; 18,5.6.8). O programa **não interpreta** essas passagens, só as **lista**.

## Passo 4: o roteiro

O `Mapa` guarda os **lugares** e os **caminhos** (ida e volta) numa estrutura de **vizinhos**: para cada lugar, uma lista de `(destino, dias)`. Dois algoritmos clássicos percorrem esse grafo:

- **`MenosParadas`** é uma **busca em largura** (BFS). Usa uma `Queue` e explora o mapa **camada por camada**: primeiro todos os vizinhos diretos, depois os vizinhos deles. O primeiro caminho que chega ao destino tem o **menor número de paradas**, mas não necessariamente o menor tempo. O dicionário `veioDe` lembra de onde cada lugar foi alcançado, e `Montar` refaz o caminho **de trás para a frente**.
- **`MenorTempo`** é o **algoritmo de Dijkstra**. Usa uma **`PriorityQueue<string, int>`** que sempre entrega primeiro o lugar de **menor custo acumulado**. A linha `if (custo > distancia[atual]) continue;` descarta entradas **velhas** da fila (já achamos um caminho melhor). Funciona porque os dias **nunca são negativos**.

O `Guia` junta as peças de capítulos anteriores: usa a **`Lanterna`** do capítulo 124 (um escopo `rota X → Y` e uma linha com os dias) e o **`Poco`** do capítulo 128 para **guardar rotas** já calculadas. Repare no tipo `Poco<(string, string), Rota?>`: a chave é uma **tupla** `(origem, destino)`.

```cs Jornada/Joao/Lab14/Guia.cs
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
```

Duas rotas:

```bash
dotnet run --project codigo/joao/Joao.Console -- rota jordao betania
```

```saida
  [Informacao] 16:31:09 > rota jordao → betania: rota pronta {dias=3, reaproveitadas=0}
2 trecho(s), cerca de 3 dia(s) (valores ilustrativos):
  • Além do Jordão: onde João batizava e deu testemunho (Jo 1)
  • Jerusalém: o templo, Nicodemos, o cego de nascença (Jo 2 a 5, 7 a 9)
  • Betânia: a casa de Marta, Maria e Lázaro (Jo 11)
```

Do Jordão a Betânia, passando por Jerusalém: **2 trechos** e **3 dias** (ilustrativos). A linha com `[Informacao]` é a Lanterna do guia; a hora é a do seu computador, e `reaproveitadas=0` indica que a rota foi calculada agora, e não tirada do poço. Outra:

```bash
dotnet run --project codigo/joao/Joao.Console -- rota cana jerusalem
```

```saida
  [Informacao] 16:31:10 > rota cana → jerusalem: rota pronta {dias=5, reaproveitadas=0}
2 trecho(s), cerca de 5 dia(s) (valores ilustrativos):
  • Caná da Galileia: o casamento: água em vinho (Jo 2)
  • Além do Jordão: onde João batizava e deu testemunho (Jo 1)
  • Jerusalém: o templo, Nicodemos, o cego de nascença (Jo 2 a 5, 7 a 9)
```

De Caná a Jerusalém, o **caminho mais rápido** (5 dias) passa pelo Jordão, e não por Samaria (que daria 6): o Dijkstra escolhe pelo tempo, e a BFS escolheria pelo número de paradas. Os testes mostram um caso em que os dois **discordam**: `Menor_tempo_pode_escolher_um_caminho_com_mais_paradas`.

::: dica Quando usar cada algoritmo
**BFS** serve quando todas as arestas "custam o mesmo" (cliques, saltos, graus de separação). **Dijkstra** serve quando cada aresta tem um **custo não negativo** (tempo, distância, preço). Se houver custos negativos, é preciso outro algoritmo (Bellman-Ford). Em ambos, a ideia é a mesma: **explorar com uma estrutura que garanta a ordem certa**, que é a fila no primeiro e a fila de prioridade no segundo.
:::

## Passo 5: os testes do laboratório

Os testes confirmam tanto os **dados** quanto os **algoritmos**:

```cs Joao.Testes/Lab14Tests.cs
using Jornada.Joao.Cap124;
using Jornada.Joao.Lab14;

namespace Joao.Testes;

public class Lab14Tests
{
    private static readonly Concordancia Conc = Concordancia.Embutida();

    [Fact]
    public void Os_dados_embutidos_tem_o_tamanho_do_texto_grego()
    {
        Assert.Equal(15438, Conc.Dados.TotalDePalavras);
        Assert.Equal(21, Conc.Dados.PalavrasPorCapitulo.Count);
        Assert.Equal(66, Conc.Dados.Lemas.Count);
        Assert.All(Conc.Dados.Lemas, l => Assert.Equal(l.Total, l.PorCapitulo.Values.Sum()));   // a soma por capítulo fecha com o total
    }

    [Theory]
    [InlineData("pisteuō", 98)]          // crer
    [InlineData("menō", 40)]             // permanecer
    [InlineData("sēmeion", 17)]          // sinal
    [InlineData("logos", 40)]            // palavra
    [InlineData("katalambanō", 2)]         // apanhar, alcançar, compreender
    [InlineData("skēnoō", 1)]         // armar tenda, habitar
    [InlineData("anōthen", 5)]         // do alto, de novo
    [InlineData("pneō", 2)]         // soprar
    [InlineData("hydria", 3)]         // jarro de água
    [InlineData("antleō", 4)]         // tirar água
    [InlineData("pēgē", 3)]         // fonte, nascente
    [InlineData("phrear", 2)]         // poço, cisterna
    [InlineData("paidarion", 1)]         // menino
    [InlineData("opsarion", 5)]         // peixe
    [InlineData("klasma", 2)]         // pedaço
    [InlineData("kophinos", 1)]         // cesto
    [InlineData("synagō", 7)]         // reunir, recolher
    [InlineData("apollymi", 10)]         // perder, destruir
    [InlineData("phōnē", 15)]         // voz, som
    [InlineData("allotrios", 2)]         // estranho
    [InlineData("misthōtos", 2)]         // assalariado
    [InlineData("lyō", 6)]         // soltar, desatar
    [InlineData("aphiēmi", 15)]         // deixar, soltar, perdoar
    [InlineData("dakryō", 1)]         // chorar
    [InlineData("keiria", 1)]         // atadura, faixa
    [InlineData("soudarion", 2)]         // lenço
    [InlineData("lention", 2)]         // toalha
    [InlineData("niptēr", 1)]         // bacia
    [InlineData("niptō", 13)]         // lavar
    [InlineData("hypodeigma", 1)]         // exemplo
    [InlineData("doulos", 11)]         // servo
    [InlineData("klēma", 4)]         // ramo
    [InlineData("karpos", 10)]         // fruto
    [InlineData("kathairō", 1)]         // limpar, podar
    [InlineData("airō", 26)]         // tirar, levantar
    [InlineData("geōrgos", 1)]         // lavrador
    [InlineData("akoloutheō", 19)]         // seguir
    [InlineData("Lazaros", 11)]         // Lázaro
    [InlineData("Nikodēmos", 5)]         // Nicodemos
    [InlineData("oinos", 6)]         // vinho
    [InlineData("architriklinos", 3)]         // mestre de cerimônia
    [InlineData("nyx", 6)]         // noite
    [InlineData("dipsaō", 6)]         // ter sede
    public void Os_totais_conferem_com_o_texto_original(string translit, int total) =>
        Assert.Equal(total, Conc.Buscar(translit)!.Total);

    [Fact]
    public void Buscar_aceita_grego_transliteracao_ou_sentido_sem_ligar_para_maiusculas()
    {
        Assert.Same(Conc.Buscar("μένω"), Conc.Buscar("MENŌ"));
        Assert.Same(Conc.Buscar("permanecer"), Conc.Buscar("menō"));
        Assert.Null(Conc.Buscar("inexistente"));
    }

    [Fact]
    public void Permanecer_se_concentra_no_capitulo_quinze()
    {
        var menō = Conc.Buscar("menō")!;

        var pico = Conc.Pico(menō)!.Value;

        Assert.Equal(15, pico.Capitulo);
        Assert.Equal(11, menō.Em(15));
        Assert.InRange(Conc.Concentracao(menō, 15), 0.27, 0.28);      // 11 de 40
    }

    [Fact]
    public void A_densidade_compara_capitulos_de_tamanhos_diferentes()
    {
        var menō = Conc.Buscar("menō")!;

        // capítulo 15 tem 500 palavras e 11 ocorrências → 22 a cada mil
        Assert.Equal(22.0, Conc.Densidade(menō, 15), 6);
        Assert.Equal(0.0, Conc.Densidade(menō, 13), 6);
    }

    [Fact]
    public void O_ranking_mostra_as_palavras_mais_frequentes()
    {
        var topo = Conc.MaisFrequentes(3).Select(l => l.Translit).ToList();

        Assert.Equal(new[] { "patēr", "pisteuō", "oida" }, topo);
    }

    [Fact]
    public void O_texto_tem_vinte_e_quatro_eu_sou_e_treze_com_uma_das_sete_imagens()
    {
        var euSou = Conc.Dados.EuSou;

        Assert.Equal(24, euSou.Count);
        Assert.Equal(13, euSou.Count(e => e.Predicado is not null));
        Assert.Contains(euSou, e => e.Capitulo == 14 && e.Versiculo == 6 && e.Predicado == "o caminho, a verdade e a vida");
    }

    [Fact]
    public void As_barras_tem_uma_linha_por_capitulo_e_a_maior_ocupa_a_largura_toda()
    {
        var linhas = Conc.Barras(Conc.Buscar("menō")!, largura: 20).Split('\n');

        Assert.Equal(21, linhas.Length);
        Assert.Contains(new string('█', 20), linhas[14]);             // capítulo 15
    }

    // ---------------- o mapa ----------------

    private static readonly Mapa Roteiros = Mapa.Padrao();

    [Fact]
    public void Menos_paradas_usa_busca_em_largura()
    {
        var rota = Roteiros.MenosParadas("jordao", "betania")!;

        Assert.Equal(new[] { "Além do Jordão", "Jerusalém", "Betânia" }, rota.Paradas.Select(p => p.Nome));
        Assert.Equal(2, rota.Trechos);
    }

    [Fact]
    public void Menor_tempo_pode_escolher_um_caminho_com_mais_paradas()
    {
        // Cafarnaum → Jerusalém: direto são 5 dias; por Caná e Jordão: 1 + 3 + 2 = 6; por Caná, Samaria: 1+3+3 = 7.
        var rapida = Roteiros.MenorTempo("cafarnaum", "jerusalem")!;
        var curta = Roteiros.MenosParadas("cafarnaum", "jerusalem")!;

        Assert.Equal(5, rapida.Dias);
        Assert.Equal(curta.Paradas.Select(p => p.Id), rapida.Paradas.Select(p => p.Id));

        // Caná → Jerusalém: por Jordão (3+2=5) vence Samaria (3+3=6), embora ambos passem por 3 lugares.
        Assert.Equal(5, Roteiros.MenorTempo("cana", "jerusalem")!.Dias);
        Assert.Contains("jordao", Roteiros.MenorTempo("cana", "jerusalem")!.Paradas.Select(p => p.Id));
    }

    [Fact]
    public void Origem_igual_ao_destino_e_uma_rota_de_zero_dias()
    {
        var rota = Roteiros.MenorTempo("cana", "cana")!;
        Assert.Equal(0, rota.Dias);
        Assert.Single(rota.Paradas);
    }

    [Fact]
    public void Lugar_desconhecido_e_um_erro_claro_e_trecho_invalido_e_recusado()
    {
        Assert.Throws<KeyNotFoundException>(() => Roteiros.MenorTempo("cana", "atlantida"));
        Assert.Throws<ArgumentException>(() => new Mapa(new[] { new Lugar("a", "A", "") }, new[] { new Trecho("a", "b", 1) }));
    }

    [Fact]
    public void Sem_ligacao_nao_ha_rota()
    {
        var isolado = new Mapa(new[] { new Lugar("a", "A", ""), new Lugar("b", "B", "") }, Array.Empty<Trecho>());
        Assert.Null(isolado.MenosParadas("a", "b"));
        Assert.Null(isolado.MenorTempo("a", "b"));
    }

    [Fact]
    public void O_guia_registra_com_a_lanterna_e_reaproveita_rotas_pelo_poco()
    {
        var linhas = new List<Registro>();
        var guia = new Guia(Roteiros, new Lanterna(Nivel.Detalhe, linhas.Add, new RelogioDeTeste()), new RelogioDeTeste());

        var primeira = guia.Rota("jordao", "betania");
        var segunda = guia.Rota("jordao", "betania");

        Assert.Same(primeira, segunda);
        Assert.Equal(1, guia.RotasReaproveitadas);
        Assert.Equal(new[] { "rota jordao → betania" }, linhas[0].Escopos);
        Assert.Equal(3, linhas[0].Campos["dias"]);
    }
}
```

Três merecem destaque:

- **`Os_totais_conferem_com_o_texto_original`** é um `[Theory]` que fixa os totais das palavras do livro (98 de *pisteuō*, 40 de *menō*, 17 de *sēmeion*...). Se alguém mexer no JSON por engano, o teste aponta qual palavra mudou.
- **`O_texto_tem_vinte_e_quatro_eu_sou_e_treze_com_uma_das_sete_imagens`** garante a contagem do capítulo 125.
- **`Menor_tempo_pode_escolher_um_caminho_com_mais_paradas`** é o teste que **distingue** Dijkstra de BFS.

::: dica Rode, quebre, conserte
Em `Concordancia.Densidade`, troque `1000.0` por `100.0` e rode `dotnet test`. O teste `A_densidade_compara_capitulos_de_tamanhos_diferentes` falha, e o `Program` passa a mostrar 2,2 em vez de 22,0. Testes são o jeito de **proteger um número** que você prometeu a quem lê.
:::

## Para ir além

::: exercicio Desafios do laboratório
**1.** Escreva `MaisConcentradas(this Concordancia c, int capitulo, int minimo = 3)`, que devolve as 5 palavras com **maior fatia de suas ocorrências** naquele capítulo (use `Concentracao`), ignorando as que aparecem menos de `minimo` vezes no livro.

**2.** Acrescente ao `Program.cs` o comando `capitulo <n>`, que mostra as 5 palavras mais frequentes **naquele capítulo**.

**3.** Escreva `ComEscala(this Mapa mapa, string de, string via, string para)`, que devolve a rota de menor tempo **passando obrigatoriamente por `via`** (ou `null` se algum trecho for impossível).

**4.** Escreva um teste que garanta que, **para todo lema**, a **soma das ocorrências por capítulo** é igual ao **total**.

--- resposta
**1.** Um método de extensão com `Where`, `Select` e `OrderByDescending`:

```cs Desafio 1
public static IReadOnlyList<(Lema Lema, double Fatia)> MaisConcentradas(this Concordancia c, int capitulo, int minimo = 3) =>
    c.Dados.Lemas
        .Where(l => l.Total >= minimo)
        .Select(l => (Lema: l, Fatia: c.Concentracao(l, capitulo)))
        .OrderByDescending(x => x.Fatia).ThenBy(x => x.Lema.Translit)
        .Take(5)
        .ToList();
```

**2.** Um novo `case` com `when`:

```cs Desafio 2
case "capitulo" when args.Length > 1 && int.TryParse(args[1], out var cap):
    foreach (var l in concordancia.Dados.Lemas.Where(l => l.Em(cap) > 0).OrderByDescending(l => l.Em(cap)).Take(5))
        Console.WriteLine($"{l.Grego,-10} {l.Translit,-10} {l.Em(cap),3}");
    break;
```

**3.** Duas buscas de Dijkstra, uma de `de` até `via` e outra de `via` até `para`, emendadas sem repetir a parada do meio:

```cs Desafio 3
public static Rota? ComEscala(this Mapa mapa, string de, string via, string para)
{
    var ida = mapa.MenorTempo(de, via);
    var volta = mapa.MenorTempo(via, para);

    return ida is null || volta is null
        ? null
        : new Rota(ida.Paradas.Concat(volta.Paradas.Skip(1)).ToList(), ida.Dias + volta.Dias);
}
```

**4.** `Assert.All` aplica uma verificação a cada elemento e **diz qual falhou**:

```cs Desafio 4
[Fact]
public void A_soma_por_capitulo_de_cada_lema_e_o_total()
{
    var dados = Concordancia.Embutida().Dados;

    Assert.All(dados.Lemas, l => Assert.Equal(l.Total, l.PorCapitulo.Values.Sum()));
}
```
:::

::: resumo
- A **concordância** responde onde cada palavra do original aparece; a **densidade** (por mil palavras) permite comparar capítulos de tamanhos diferentes, e o **pico** é o capítulo de maior densidade.
- Dados vêm de um **recurso embutido** lido com `System.Text.Json` e consultados com **LINQ** (`OrderByDescending`, `MaxBy`, `Take`).
- **BFS** acha o caminho com menos paradas (`Queue`); **Dijkstra** acha o de menor custo (`PriorityQueue`), com custos não negativos.
- O `Guia` reúne a **Lanterna** (cap. 124) e o **Poço** (cap. 128): peças pequenas, bem testadas, se combinam.
- Os testes **fixam os números** que o livro promete; os "dias de viagem" do mapa são **ilustrativos**, e o programa avisa.
:::

::: quiz
? O que mede a "densidade" de uma palavra num capítulo?
+ Ocorrências a cada mil palavras do capítulo
- O total de ocorrências no livro
- O número de versículos
- A posição da palavra
> Permite comparar capítulos curtos e longos.

? Em qual capítulo *menō* tem o pico de densidade?
+ No capítulo 15 (22 por mil palavras)
- No capítulo 1
- No capítulo 21
- Em nenhum
> Onze das 40 ocorrências estão ali.

? Qual estrutura a busca em largura usa?
+ Uma fila (`Queue`)
- Uma pilha
- Uma árvore binária
- Um dicionário apenas
> Explora camada por camada.

? Para que serve a `PriorityQueue` no Dijkstra?
+ Entregar sempre primeiro o lugar de menor custo acumulado
- Ordenar os nomes dos lugares
- Guardar os resultados
- Evitar exceções
> É o que garante a ordem de exploração.

? Os dias de viagem do mapa são...
+ Valores ilustrativos para ensinar o algoritmo, e não dados históricos
- Distâncias medidas por arqueólogos
- Tempos reais
- Dados do texto grego
> O programa avisa isso na saída.

? O que o `Assert.All` faz num teste?
+ Aplica uma verificação a cada elemento e aponta qual falhou
- Executa todos os testes
- Ignora erros
- Conta os elementos
> Útil para propriedades que valem para todos.
:::

::: cartas
Concordância :: Índice de palavras: onde cada termo aparece num texto.
Lema :: Forma de dicionário de uma palavra (ἀγαπᾷ e ἠγάπησεν contam como ἀγαπάω).
Densidade :: Ocorrências por mil palavras: compara capítulos de tamanhos diferentes.
Recurso embutido :: Arquivo incorporado ao programa e lido sem depender do disco.
Busca em largura (BFS) :: Explora camada por camada com uma fila; menos paradas.
Dijkstra :: Caminho de menor custo com custos não negativos, usando fila de prioridade.
PriorityQueue<T, P> :: Fila que entrega primeiro o elemento de menor prioridade.
Assert.All :: Verifica uma condição em cada elemento de uma coleção.
:::
