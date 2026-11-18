using System.Collections.Immutable;

namespace Jornada.Joao.Cap124;

/// <summary>Quanto de luz queremos: do detalhe miúdo ao erro que não pode passar despercebido.</summary>
public enum Nivel { Detalhe, Informacao, Aviso, Erro }

/// <summary>Uma linha do registro: quando, quão grave, o quê, com quais dados e dentro de quais "cenas" (escopos).</summary>
public sealed record Registro(
    DateTimeOffset Quando,
    Nivel Nivel,
    string Mensagem,
    IReadOnlyDictionary<string, object?> Campos,
    IReadOnlyList<string> Escopos);

public interface ILanterna
{
    void Escrever(Nivel nivel, string mensagem, params (string Nome, object? Valor)[] campos);

    /// <summary>Abre uma "cena": tudo escrito até o Dispose carrega o nome dela. Use com <c>using</c>.</summary>
    IDisposable Escopo(string nome);
}

/// <summary>
/// A lanterna apagada aceita tudo e não mostra nada. É o padrão "objeto nulo": quem usa não precisa testar se há lanterna antes de escrever.
/// Atenção: isto NÃO representa as "trevas" de Jo 1,5. As trevas do texto não acolhem a luz; o objeto nulo só obedece e se cala.
/// </summary>
public sealed class LanternaApagada : ILanterna
{
    public static readonly LanternaApagada Instancia = new();

    private LanternaApagada() { }

    public void Escrever(Nivel nivel, string mensagem, params (string Nome, object? Valor)[] campos) { }

    public IDisposable Escopo(string nome) => Vazio.Instancia;

    private sealed class Vazio : IDisposable
    {
        public static readonly Vazio Instancia = new();
        public void Dispose() { }
    }
}

public sealed class Lanterna : ILanterna
{
    private readonly Nivel minimo;
    private readonly Action<Registro> destino;
    private readonly TimeProvider relogio;

    // Cada fluxo de execução (cada tarefa) enxerga a sua própria lista de escopos abertos. A lista é imutável: abrir um escopo cria
    // uma lista nova, e fechar volta à anterior. Com uma pilha comum e compartilhada, duas tarefas misturariam os escopos.
    private readonly AsyncLocal<ImmutableList<string>?> escopos = new();

    private ImmutableList<string> Abertos => escopos.Value ?? ImmutableList<string>.Empty;

    public Lanterna(Nivel minimo, Action<Registro> destino, TimeProvider? relogio = null)
    {
        this.minimo = minimo;
        this.destino = destino;
        this.relogio = relogio ?? TimeProvider.System;
    }

    public void Escrever(Nivel nivel, string mensagem, params (string Nome, object? Valor)[] campos)
    {
        if (nivel < minimo)
            return;                                   // abaixo do nível mínimo, a luz nem acende

        var dados = new Dictionary<string, object?>();
        foreach (var (nome, valor) in campos)
            dados[nome] = valor;

        destino(new Registro(relogio.GetUtcNow(), nivel, mensagem, dados, Abertos.ToList()));
    }

    public IDisposable Escopo(string nome)
    {
        var anteriores = Abertos;
        escopos.Value = anteriores.Add(nome);
        return new SaidaDoEscopo(this, anteriores);
    }

    private sealed class SaidaDoEscopo : IDisposable
    {
        private readonly Lanterna lanterna;
        private readonly ImmutableList<string> anteriores;
        private bool fechado;

        public SaidaDoEscopo(Lanterna lanterna, ImmutableList<string> anteriores)
        {
            this.lanterna = lanterna;
            this.anteriores = anteriores;
        }

        public void Dispose()
        {
            if (fechado)
                return;                               // Dispose pode ser chamado duas vezes sem estrago

            if (lanterna.Abertos.Count != anteriores.Count + 1)
                throw new InvalidOperationException("Os escopos precisam ser fechados na ordem inversa da abertura.");

            lanterna.escopos.Value = anteriores;
            fechado = true;
        }
    }

    /// <summary>Transforma um registro em uma linha legível: [Nivel] hora > cena > cena mensagem {campo=valor}.</summary>
    public static string Formatar(Registro r)
    {
        var caminho = r.Escopos.Count > 0 ? " > " + string.Join(" > ", r.Escopos) : "";
        var campos = r.Campos.Count > 0
            ? " {" + string.Join(", ", r.Campos.Select(c => $"{c.Key}={c.Value}")) + "}"
            : "";

        return $"[{r.Nivel}] {r.Quando:HH:mm:ss}{caminho}: {r.Mensagem}{campos}";
    }
}
