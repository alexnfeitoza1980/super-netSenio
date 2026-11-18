using Jornada.Joao.Cap133;

namespace Joao.Testes;

public class Cap133Tests
{
    // Peças de teste: cada uma declara no construtor do que precisa.
    public interface ISeiva { string Nome { get; } }
    public sealed class SeivaDaTerra : ISeiva { public string Nome => "seiva"; }

    public interface IRamo { ISeiva Seiva { get; } }
    public sealed class Ramo : IRamo
    {
        public Ramo(ISeiva seiva) => Seiva = seiva;
        public ISeiva Seiva { get; }
    }

    public interface IFruto { IRamo Ramo { get; } ISeiva Seiva { get; } }
    public sealed class Fruto : IFruto
    {
        public Fruto(IRamo ramo, ISeiva seiva) { Ramo = ramo; Seiva = seiva; }
        public IRamo Ramo { get; }
        public ISeiva Seiva { get; }
    }

    public sealed class Raiz : IDisposable
    {
        public static List<string> Descartes { get; } = new();
        public string Id { get; } = Guid.NewGuid().ToString("N")[..4];
        public void Dispose() => Descartes.Add(Id);
    }

    [Fact]
    public void O_recipiente_liga_a_cadeia_inteira_de_dependencias()
    {
        using var videira = new Recipiente()
            .Registrar<ISeiva, SeivaDaTerra>()
            .Registrar<IRamo, Ramo>()
            .Registrar<IFruto, Fruto>();

        var fruto = videira.Obter<IFruto>();

        Assert.Equal("seiva", fruto.Ramo.Seiva.Nome);
    }

    [Fact]
    public void Transiente_cria_uma_peça_nova_a_cada_pedido()
    {
        using var videira = new Recipiente().Registrar<ISeiva, SeivaDaTerra>(Ciclo.Transiente);

        Assert.NotSame(videira.Obter<ISeiva>(), videira.Obter<ISeiva>());
    }

    [Fact]
    public void Permanente_permanece_e_e_a_mesma_para_todos()
    {
        using var videira = new Recipiente()
            .Registrar<ISeiva, SeivaDaTerra>(Ciclo.Permanente)
            .Registrar<IRamo, Ramo>()
            .Registrar<IFruto, Fruto>();

        var fruto = videira.Obter<IFruto>();

        Assert.Same(fruto.Seiva, fruto.Ramo.Seiva);          // ramo e fruto bebem da MESMA seiva
        Assert.Same(fruto.Seiva, videira.Obter<ISeiva>());
    }

    [Fact]
    public void Peça_nao_registrada_gera_erro_que_diz_o_que_falta()
    {
        using var videira = new Recipiente().Registrar<IRamo, Ramo>();   // falta a seiva

        var e = Assert.Throws<InvalidOperationException>(() => videira.Obter<IRamo>());
        Assert.Contains("nenhum construtor tem todas as peças registradas", e.Message);
        Assert.Throws<InvalidOperationException>(() => videira.Obter<IFruto>());
    }

    public interface IA { }
    public interface IB { }
    public sealed class A : IA { public A(IB b) { } }
    public sealed class B : IB { public B(IA a) { } }

    [Fact]
    public void Dependencia_circular_e_detectada_com_o_caminho_do_ciclo()
    {
        using var videira = new Recipiente().Registrar<IA, A>().Registrar<IB, B>();

        var e = Assert.Throws<InvalidOperationException>(() => videira.Obter<IA>());

        Assert.Equal("Dependência circular: IA → IB → IA.", e.Message);
    }

    [Fact]
    public void Fabrica_permite_registrar_com_logica_propria()
    {
        using var videira = new Recipiente()
            .Registrar<ISeiva>(_ => new SeivaDaTerra(), Ciclo.Permanente);

        Assert.Equal("seiva", videira.Obter<ISeiva>().Nome);
    }

    [Fact]
    public void Podar_descarta_na_ordem_inversa_da_criacao_e_so_uma_vez()
    {
        Raiz.Descartes.Clear();
        Raiz? primeira = null, segunda = null;

        var videira = new Recipiente().Registrar<Raiz>(_ => new Raiz(), Ciclo.Transiente);
        primeira = videira.Obter<Raiz>();
        segunda = videira.Obter<Raiz>();

        videira.Dispose();
        videira.Dispose();                                   // segunda vez: nada acontece

        Assert.Equal(new[] { segunda.Id, primeira.Id }, Raiz.Descartes);
    }

    [Fact]
    public void Depois_de_podado_o_recipiente_nao_serve_mais()
    {
        var videira = new Recipiente().Registrar<ISeiva, SeivaDaTerra>();
        videira.Dispose();

        Assert.Throws<ObjectDisposedException>(() => videira.Obter<ISeiva>());
    }
}
