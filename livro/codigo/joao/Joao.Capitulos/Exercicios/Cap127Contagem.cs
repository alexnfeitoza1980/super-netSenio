using Jornada.Joao.Cap127;

namespace Jornada.Joao.Exercicios;

public static class ContagemExtensoes
{
    /// <summary>Cada vez que a fonte emite, emite quantas vezes ela já emitiu. Cada ouvinte tem a sua própria conta.</summary>
    public static IObservable<int> Contagem<T>(this IObservable<T> fonte) => new Contador<T>(fonte);

    private sealed class Contador<T> : IObservable<int>
    {
        private readonly IObservable<T> fonte;

        public Contador(IObservable<T> fonte) => this.fonte = fonte;

        public IDisposable Subscribe(IObserver<int> ouvinte)
        {
            int n = 0;                                    // uma variável por inscrição: nada de conta compartilhada
            return fonte.Ouvir(_ => ouvinte.OnNext(++n), ouvinte.OnCompleted, ouvinte.OnError);
        }
    }
}
