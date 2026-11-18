namespace Jornada.Joao.Cap127;

/// <summary>
/// Um "vento": publica valores para quem estiver ouvindo, sem saber quem são.
/// Implementa IObservable&lt;T&gt;, a interface padrão do .NET para fluxos de eventos.
/// </summary>
public sealed class Vento<T> : IObservable<T>
{
    private readonly List<IObserver<T>> ouvintes = new();
    private bool terminou;

    public int Ouvintes => ouvintes.Count;

    public IDisposable Subscribe(IObserver<T> ouvinte)
    {
        if (terminou)
        {
            ouvinte.OnCompleted();                    // quem chega depois do fim só ouve o silêncio
            return Inscricao.Vazia;
        }

        ouvintes.Add(ouvinte);
        return new Inscricao(() => ouvintes.Remove(ouvinte));
    }

    public void Soprar(T valor)
    {
        if (terminou) return;

        foreach (var ouvinte in ouvintes.ToArray())   // cópia: um ouvinte pode sair durante o sopro
            ouvinte.OnNext(valor);
    }

    public void Completar()
    {
        if (terminou) return;
        terminou = true;
        foreach (var ouvinte in ouvintes.ToArray()) ouvinte.OnCompleted();
        ouvintes.Clear();
    }

    public void Falhar(Exception erro)
    {
        if (terminou) return;
        terminou = true;
        foreach (var ouvinte in ouvintes.ToArray()) ouvinte.OnError(erro);
        ouvintes.Clear();
    }
}

/// <summary>Um vento que lembra os últimos valores: quem "nasce de novo" mais tarde recebe o que já aconteceu.</summary>
public sealed class VentoComMemoria<T> : IObservable<T>
{
    private readonly Vento<T> vento = new();
    private readonly Queue<T> memoria = new();
    private readonly int capacidade;

    public VentoComMemoria(int capacidade)
    {
        if (capacidade < 1) throw new ArgumentOutOfRangeException(nameof(capacidade));
        this.capacidade = capacidade;
    }

    public IDisposable Subscribe(IObserver<T> ouvinte)
    {
        foreach (var antigo in memoria.ToArray())
            ouvinte.OnNext(antigo);                   // primeiro o que já passou...

        return vento.Subscribe(ouvinte);              // ...depois o que vier
    }

    public void Soprar(T valor)
    {
        memoria.Enqueue(valor);
        if (memoria.Count > capacidade) memoria.Dequeue();
        vento.Soprar(valor);
    }

    public void Completar() => vento.Completar();
}

/// <summary>Desinscrever é descartar: Subscribe devolve um IDisposable que desliga a escuta.</summary>
internal sealed class Inscricao : IDisposable
{
    public static readonly Inscricao Vazia = new(() => { });

    private Action? aoDescartar;

    public Inscricao(Action aoDescartar) => this.aoDescartar = aoDescartar;

    public void Dispose()
    {
        aoDescartar?.Invoke();
        aoDescartar = null;                           // descartar duas vezes não faz mal
    }
}

internal sealed class Ouvinte<T> : IObserver<T>
{
    private readonly Action<T> aoReceber;
    private readonly Action? aoCompletar;
    private readonly Action<Exception>? aoFalhar;

    public Ouvinte(Action<T> aoReceber, Action? aoCompletar, Action<Exception>? aoFalhar)
    {
        this.aoReceber = aoReceber;
        this.aoCompletar = aoCompletar;
        this.aoFalhar = aoFalhar;
    }

    public void OnNext(T valor) => aoReceber(valor);
    public void OnCompleted() => aoCompletar?.Invoke();
    public void OnError(Exception erro) => aoFalhar?.Invoke(erro);
}

/// <summary>Operadores: pequenos "filtros de vento" que se encadeiam, como o LINQ, mas para fluxos que ainda vão acontecer.</summary>
public static class Observaveis
{
    public static IDisposable Ouvir<T>(this IObservable<T> fonte, Action<T> aoReceber, Action? aoCompletar = null, Action<Exception>? aoFalhar = null) =>
        fonte.Subscribe(new Ouvinte<T>(aoReceber, aoCompletar, aoFalhar));

    public static IObservable<T> Onde<T>(this IObservable<T> fonte, Func<T, bool> condicao) =>
        new Operador<T, T>(fonte, (valor, adiante) => { if (condicao(valor)) adiante(valor); });

    public static IObservable<TSaida> Transformar<T, TSaida>(this IObservable<T> fonte, Func<T, TSaida> funcao) =>
        new Operador<T, TSaida>(fonte, (valor, adiante) => adiante(funcao(valor)));

    private sealed class Operador<TEntrada, TSaida> : IObservable<TSaida>
    {
        private readonly IObservable<TEntrada> fonte;
        private readonly Action<TEntrada, Action<TSaida>> passo;

        public Operador(IObservable<TEntrada> fonte, Action<TEntrada, Action<TSaida>> passo)
        {
            this.fonte = fonte;
            this.passo = passo;
        }

        public IDisposable Subscribe(IObserver<TSaida> ouvinte) =>
            fonte.Subscribe(new Ouvinte<TEntrada>(
                valor => passo(valor, ouvinte.OnNext),
                ouvinte.OnCompleted,
                ouvinte.OnError));
    }
}
