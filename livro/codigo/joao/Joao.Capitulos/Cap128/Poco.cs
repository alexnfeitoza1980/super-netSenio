using System.Collections.Concurrent;

namespace Jornada.Joao.Cap128;

/// <summary>
/// Um poço de memória: guarda por um tempo o que já foi buscado na fonte, para não descer ao fundo toda vez.
/// A fonte é a "verdade"; o poço é uma cópia à mão, com prazo de validade.
/// </summary>
public sealed class Poco<TChave, TValor> where TChave : notnull
{
    private sealed record Entrada(Lazy<TValor> Valor, DateTimeOffset CriadaEm);

    private readonly ConcurrentDictionary<TChave, Entrada> agua = new();
    private readonly Func<TChave, TValor> fonte;
    private readonly TimeSpan validade;
    private readonly TimeProvider relogio;

    private long acertos, falhas, expiradas;

    public Poco(Func<TChave, TValor> fonte, TimeSpan validade, TimeProvider? relogio = null)
    {
        if (validade <= TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(validade), "A validade precisa ser positiva.");

        this.fonte = fonte;
        this.validade = validade;
        this.relogio = relogio ?? TimeProvider.System;
    }

    public long Acertos => Interlocked.Read(ref acertos);      // achou no poço
    public long Falhas => Interlocked.Read(ref falhas);        // não havia nada: foi à fonte
    public long Expiradas => Interlocked.Read(ref expiradas);  // havia, mas estava velho: foi à fonte de novo
    public int Quantidade => agua.Count;

    public TValor Obter(TChave chave)
    {
        var agora = relogio.GetUtcNow();
        bool existia = agua.ContainsKey(chave);

        // Lazy garante que a fonte seja chamada UMA vez por entrada, mesmo com várias pessoas pedindo juntas.
        var nova = new Entrada(new Lazy<TValor>(() => fonte(chave)), agora);

        var entrada = agua.AddOrUpdate(
            chave,
            nova,
            (_, atual) => agora - atual.CriadaEm < validade ? atual : nova);

        if (ReferenceEquals(entrada, nova))
        {
            if (existia) Interlocked.Increment(ref expiradas);
            else Interlocked.Increment(ref falhas);
        }
        else
        {
            Interlocked.Increment(ref acertos);
        }

        return entrada.Valor.Value;
    }

    public bool Invalidar(TChave chave) => agua.TryRemove(chave, out _);

    public void Esvaziar() => agua.Clear();
}
