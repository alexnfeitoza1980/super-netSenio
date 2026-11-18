using System.Reflection;

namespace Jornada.Joao.Cap133;

/// <summary>Quanto tempo uma peça "permanece": criada de novo a cada pedido, ou uma só, que fica.</summary>
public enum Ciclo { Transiente, Permanente }

/// <summary>
/// Um recipiente de dependências bem pequeno: quem precisa de uma peça pede pelo TIPO,
/// e o recipiente descobre, por reflexão, de que a peça precisa e liga tudo (o ramo na videira).
/// </summary>
public sealed class Recipiente : IDisposable
{
    private sealed record Registro(Type Implementacao, Ciclo Ciclo, Func<Recipiente, object>? Fabrica);

    private readonly Dictionary<Type, Registro> registros = new();
    private readonly Dictionary<Type, object> permanentes = new();
    private readonly List<IDisposable> descartaveis = new();
    private bool descartado;

    public Recipiente Registrar<TServico, TImplementacao>(Ciclo ciclo = Ciclo.Transiente)
        where TImplementacao : TServico
    {
        registros[typeof(TServico)] = new Registro(typeof(TImplementacao), ciclo, null);
        return this;
    }

    public Recipiente Registrar<TServico>(Func<Recipiente, TServico> fabrica, Ciclo ciclo = Ciclo.Transiente)
        where TServico : notnull
    {
        registros[typeof(TServico)] = new Registro(typeof(TServico), ciclo, r => fabrica(r)!);
        return this;
    }

    public T Obter<T>() where T : notnull => (T)Obter(typeof(T), new Stack<Type>());

    private object Obter(Type tipo, Stack<Type> caminho)
    {
        if (descartado)
            throw new ObjectDisposedException(nameof(Recipiente));

        if (!registros.TryGetValue(tipo, out var registro))
            throw new InvalidOperationException($"Não sei criar {tipo.Name}: ninguém a registrou.");

        if (caminho.Contains(tipo))
        {
            var ciclo = string.Join(" → ", caminho.Reverse().Append(tipo).Select(t => t.Name));
            throw new InvalidOperationException($"Dependência circular: {ciclo}.");
        }

        if (registro.Ciclo == Ciclo.Permanente && permanentes.TryGetValue(tipo, out var existente))
            return existente;

        caminho.Push(tipo);
        try
        {
            var criado = registro.Fabrica?.Invoke(this) ?? Construir(registro.Implementacao, caminho);

            if (criado is IDisposable descartavel)
                descartaveis.Add(descartavel);
            if (registro.Ciclo == Ciclo.Permanente)
                permanentes[tipo] = criado;

            return criado;
        }
        finally
        {
            caminho.Pop();
        }
    }

    /// <summary>Escolhe o construtor com mais parâmetros que o recipiente consiga satisfazer e resolve cada um.</summary>
    private object Construir(Type implementacao, Stack<Type> caminho)
    {
        var construtor = implementacao
            .GetConstructors(BindingFlags.Public | BindingFlags.Instance)
            .OrderByDescending(c => c.GetParameters().Length)
            .FirstOrDefault(c => c.GetParameters().All(p => registros.ContainsKey(p.ParameterType)))
            ?? throw new InvalidOperationException(
                $"Não sei criar {implementacao.Name}: nenhum construtor tem todas as peças registradas.");

        var argumentos = construtor.GetParameters()
            .Select(p => Obter(p.ParameterType, caminho))
            .ToArray();

        return construtor.Invoke(argumentos);
    }

    /// <summary>"Podar": descarta, na ordem inversa da criação, tudo o que o recipiente criou e é descartável.</summary>
    public void Dispose()
    {
        if (descartado) return;
        descartado = true;

        for (int i = descartaveis.Count - 1; i >= 0; i--)
            descartaveis[i].Dispose();

        descartaveis.Clear();
        permanentes.Clear();
    }
}
