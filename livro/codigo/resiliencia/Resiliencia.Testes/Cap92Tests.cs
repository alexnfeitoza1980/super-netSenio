using Jornada.Resiliencia.Cap92;

namespace Resiliencia.Testes;

public class Cap92Tests
{
    private static readonly DateOnly Hoje = new(2026, 11, 2);

    [Fact]
    public void Passo_grande_demais_e_recusado_com_uma_dica()
    {
        var meta = new Meta("Estudar para a prova");
        var e = Assert.Throws<ArgumentException>(() => meta.AdicionarPasso("Estudar tudo", 120));
        Assert.Contains("Divida", e.Message);
    }

    [Fact]
    public void Dividir_gera_passos_dentro_do_limite()
    {
        var partes = Meta.Dividir("Ler o capítulo", 60).ToList();

        Assert.Equal(3, partes.Count);
        Assert.Equal(new[] { 25, 25, 10 }, partes.Select(p => p.Minutos));
        Assert.Equal("Ler o capítulo (parte 1 de 3)", partes[0].Descricao);
    }

    [Fact]
    public void Concluir_cria_uma_pequena_prova_e_avanca_o_andamento()
    {
        var meta = new Meta("Estudar").AdicionarDividindo("Resolver exercícios", 50);

        Assert.Equal((0, 2), meta.Andamento);
        meta.ConcluirProximo(Hoje);

        Assert.Equal((1, 2), meta.Andamento);
        Assert.Single(meta.Provas);
        Assert.Equal("Resolver exercícios (parte 2 de 2)", meta.ProximoPasso!.Descricao);
    }

    [Fact]
    public void Sem_passo_pendente_nao_ha_o_que_concluir()
    {
        var meta = new Meta("Nada a fazer");
        Assert.Null(meta.ProximoPasso);
        Assert.Throws<InvalidOperationException>(() => meta.ConcluirProximo(Hoje));
    }

    [Theory]
    [InlineData("", 10)]
    [InlineData("ok", 0)]
    public void Passo_invalido_e_recusado(string descricao, int minutos) =>
        Assert.ThrowsAny<ArgumentException>(() => new Meta("M").AdicionarPasso(descricao, minutos));
}
