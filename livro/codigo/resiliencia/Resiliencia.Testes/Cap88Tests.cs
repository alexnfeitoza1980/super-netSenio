using Jornada.Resiliencia.Cap88;

namespace Resiliencia.Testes;

public class Cap88Tests
{
    [Fact]
    public void Adaptar_esta_sempre_entre_os_caminhos()
    {
        var d = new Dificuldade("Perdi o emprego", RotinaAindaPossivel: false, TemAlguemDeConfianca: false);
        Assert.Equal(new[] { Resposta.Adaptar }, Caminhos.Possiveis(d));
    }

    [Fact]
    public void Os_tres_caminhos_podem_aparecer_juntos()
    {
        var d = new Dificuldade("Fui mal na prova", true, true);
        Assert.Equal(new[] { Resposta.Recuperar, Resposta.Adaptar, Resposta.BuscarApoio }, Caminhos.Possiveis(d));
    }

    [Theory]
    [InlineData(Resposta.Recuperar)]
    [InlineData(Resposta.Adaptar)]
    [InlineData(Resposta.BuscarApoio)]
    public void Todo_caminho_tem_descricao(Resposta r) =>
        Assert.False(string.IsNullOrWhiteSpace(Caminhos.Descrever(r)));

    [Theory]
    [InlineData("7", "7 h anotadas.")]
    [InlineData("sete", "Não entendi o número. Tente algo como 7.")]
    [InlineData("40", "Esse valor não cabe em um dia. Use de 0 a 24.")]
    public void As_duas_versoes_dao_a_mesma_resposta(string entrada, string esperado)
    {
        Assert.Equal(esperado, new Anotador().AnotarHorasDeSono(entrada));
        Assert.Equal(esperado, new Anotador().AnotarComTryParse(entrada));
    }

    [Fact]
    public void Finally_conta_a_tentativa_mesmo_quando_falha()
    {
        var a = new Anotador();
        a.AnotarHorasDeSono("7");
        a.AnotarHorasDeSono("oi");
        a.AnotarHorasDeSono("99");
        Assert.Equal(3, a.Tentativas);
    }
}
