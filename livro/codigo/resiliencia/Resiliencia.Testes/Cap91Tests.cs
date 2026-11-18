using Jornada.Resiliencia.Cap91;

namespace Resiliencia.Testes;

public class Cap91Tests
{
    private static Evidencia Obst(string t) => new(t, "o próprio edital");
    private static Evidencia ComFonte(string t) => new(t, "nota da última prova");
    private static Evidencia SemFonte(string t) => new(t);

    [Fact]
    public void Quadro_vazio_pede_o_primeiro_registro() =>
        Assert.Equal(Leitura.Vazio, new QuadroDeEvidencias().Ler());

    [Fact]
    public void So_obstaculos()
    {
        var q = new QuadroDeEvidencias().Obstaculo(Obst("A vaga é concorrida"));
        Assert.Equal(Leitura.SoObstaculos, q.Ler());
    }

    [Fact]
    public void So_possibilidades_com_fonte_pede_olhar_os_obstaculos()
    {
        var q = new QuadroDeEvidencias().Possibilidade(ComFonte("Melhorei 2 pontos desde março"));
        Assert.Equal(Leitura.SoPossibilidades, q.Ler());
    }

    [Fact]
    public void Possibilidade_sem_fonte_e_so_uma_ideia()
    {
        var q = new QuadroDeEvidencias()
            .Obstaculo(Obst("Tenho pouco tempo"))
            .Possibilidade(SemFonte("Vai dar tudo certo"));

        Assert.Equal(Leitura.PossibilidadesSemFonte, q.Ler());
        Assert.Single(q.AConferir());
    }

    [Fact]
    public void Obstaculo_e_possibilidade_confirmada_dao_equilibrio()
    {
        var q = new QuadroDeEvidencias()
            .Obstaculo(Obst("Tenho pouco tempo"))
            .Possibilidade(ComFonte("Já resolvi problemas parecidos"));

        Assert.Equal(Leitura.Equilibrado, q.Ler());
        Assert.Empty(q.AConferir());
    }

    [Theory]
    [InlineData(Leitura.Vazio)]
    [InlineData(Leitura.SoObstaculos)]
    [InlineData(Leitura.SoPossibilidades)]
    [InlineData(Leitura.PossibilidadesSemFonte)]
    [InlineData(Leitura.Equilibrado)]
    public void Toda_leitura_tem_mensagem(Leitura l) =>
        Assert.False(string.IsNullOrWhiteSpace(QuadroDeEvidencias.Mensagem(l)));
}
