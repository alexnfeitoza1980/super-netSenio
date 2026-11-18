using System.Runtime.CompilerServices;
using System.Threading.Channels;

namespace Jornada.Joao.Cap129;

public sealed record Porcao(int Numero, string Origem);

public sealed record Resultado(IReadOnlyList<int> ServidosPorPessoa, IReadOnlyList<Porcao> Sobras)
{
    public int TotalServido => ServidosPorPessoa.Sum();
}

public static class Distribuicao
{
    /// <summary>
    /// Um iterador assíncrono: entrega as porções uma a uma, esperando entre elas (como quem parte o pão na hora).
    /// IAsyncEnumerable&lt;T&gt; permite consumir com "await foreach".
    /// </summary>
    public static async IAsyncEnumerable<Porcao> PartirAsync(
        int paes, int peixes, int pedacosPorAlimento,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        int numero = 0;

        for (int p = 1; p <= paes; p++)
            for (int k = 0; k < pedacosPorAlimento; k++)
            {
                await Task.Yield();
                ct.ThrowIfCancellationRequested();
                yield return new Porcao(++numero, $"pão {p}");
            }

        for (int p = 1; p <= peixes; p++)
            for (int k = 0; k < pedacosPorAlimento; k++)
            {
                await Task.Yield();
                ct.ThrowIfCancellationRequested();
                yield return new Porcao(++numero, $"peixe {p}");
            }
    }

    /// <summary>
    /// Um produtor enche um canal com capacidade limitada (o cesto); várias pessoas retiram até ficarem satisfeitas.
    /// O que não foi comido é recolhido ao final: "para que nada se perca".
    /// </summary>
    public static async Task<Resultado> DistribuirAsync(
        IAsyncEnumerable<Porcao> origem,
        IReadOnlyList<int> fomes,
        int capacidadeDoCesto,
        CancellationToken ct = default)
    {
        var canal = Channel.CreateBounded<Porcao>(new BoundedChannelOptions(capacidadeDoCesto)
        {
            FullMode = BoundedChannelFullMode.Wait,    // cesto cheio: o produtor espera (contrapressão)
            SingleWriter = true
        });

        var servidos = new int[fomes.Count];

        var produtor = Task.Run(async () =>
        {
            try
            {
                await foreach (var porcao in origem.WithCancellation(ct))
                    await canal.Writer.WriteAsync(porcao, ct);
            }
            finally
            {
                canal.Writer.Complete();               // sem isso, quem espera nunca saberia que acabou
            }
        }, ct);

        var pessoas = fomes.Select((fome, i) => Task.Run(async () =>
        {
            while (servidos[i] < fome && await canal.Reader.WaitToReadAsync(ct))
            {
                if (canal.Reader.TryRead(out _))
                    servidos[i]++;
            }
        }, ct)).ToArray();

        await Task.WhenAll(pessoas);

        // Todos saciados (ou nada mais veio). O que restar no cesto são as sobras; ler também destrava o produtor.
        var sobras = new List<Porcao>();
        await foreach (var resto in canal.Reader.ReadAllAsync(ct))
            sobras.Add(resto);

        await produtor;
        return new Resultado(servidos, sobras);
    }

    /// <summary>Agrupa as sobras em cestos de tamanho fixo.</summary>
    public static IReadOnlyList<IReadOnlyList<Porcao>> EmCestos(IEnumerable<Porcao> sobras, int porCesto) =>
        sobras.Chunk(porCesto).Select(c => (IReadOnlyList<Porcao>)c).ToList();
}
