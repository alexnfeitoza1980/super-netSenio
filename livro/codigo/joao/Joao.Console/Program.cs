using System.Text.RegularExpressions;
using Jornada.Joao.Cap124;
using Jornada.Joao.Exemplos;
using Jornada.Joao.Lab14;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var concordancia = Concordancia.Embutida();
var mapa = Mapa.Padrao();

string comando = args.Length > 0 ? args[0].ToLowerInvariant() : "ajuda";

// "cap124" ... "cap133": roda os exemplos do capítulo (o passo a passo e, depois, o "Indo além"). "exemplo t124" roda um só.
if (Regex.Match(comando, @"^cap(\d+)$") is { Success: true } capitulo)
{
    var chaves = Exemplos.DoCapitulo(int.Parse(capitulo.Groups[1].Value));
    if (chaves.Count == 0) { Console.WriteLine("Esse capítulo não tem exemplos. Use cap124 a cap133."); return 1; }

    foreach (var chave in chaves)
    {
        Console.WriteLine(chave.StartsWith('t') ? "--- passo a passo ---" : "--- indo além ---");
        await Exemplos.ExecutarAsync(chave);
        Console.WriteLine();
    }
    return 0;
}

switch (comando)
{
    case "exemplo" when args.Length > 1:
        try { await Exemplos.ExecutarAsync(args[1].ToLowerInvariant()); }
        catch (ArgumentException e) { Console.WriteLine(e.Message); return 1; }
        break;

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

    case "eusou":
        foreach (var e in concordancia.Dados.EuSou)
            Console.WriteLine(e);
        break;

    case "rota" when args.Length > 2:
        var guia = new Guia(mapa, new Lanterna(Nivel.Detalhe, r => Console.WriteLine("  " + Lanterna.Formatar(r))));
        var rota = guia.Rota(args[1], args[2]);
        if (rota is null) { Console.WriteLine("Sem caminho."); return 1; }

        Console.WriteLine($"{rota.Trechos} trecho(s), cerca de {rota.Dias} dia(s) (valores ilustrativos):");
        foreach (var parada in rota.Paradas)
            Console.WriteLine($"  • {parada.Nome}: {parada.Cena}");
        break;

    case "lugares":
        foreach (var lugar in mapa.Lugares)
            Console.WriteLine($"{lugar.Id,-10} {lugar.Nome}");
        break;

    default:
        Console.WriteLine("Concordância e roteiro do Evangelho segundo João (dados do texto grego, SBLGNT/MorphGNT)");
        Console.WriteLine();
        Console.WriteLine("  cap124 ... cap133    os exemplos de um capítulo (a saída que o livro mostra)");
        Console.WriteLine("  exemplo <chave>      um exemplo só: t124 (passo a passo) ou 124 (indo além)");
        Console.WriteLine("  topo [n]             as palavras mais frequentes");
        Console.WriteLine("  onde <palavra>       onde ela aparece (grego, transliteração ou sentido): pisteuō, menō, luz...");
        Console.WriteLine("  eusou                as 24 frases \"Eu sou\"");
        Console.WriteLine("  lugares              os lugares do mapa");
        Console.WriteLine("  rota <de> <para>     o caminho mais curto no tempo, ex.: rota jordao betania");
        break;
}

return 0;
