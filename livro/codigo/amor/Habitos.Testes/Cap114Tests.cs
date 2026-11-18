using Jornada.Habitos.Cap114;

namespace Habitos.Testes;

public class Cap114Tests
{
    [Theory]
    [InlineData(-1)]
    [InlineData(100.5)]
    [InlineData(double.NaN)]
    public void Percentual_fora_da_faixa_nao_consegue_existir(double valor) =>
        Assert.Throws<ArgumentOutOfRangeException>(() => new Percentual(valor));

    [Fact]
    public void Percentual_vira_double_sozinho_e_se_escreve_com_sinal()
    {
        double d = new Percentual(37.5);
        Assert.Equal(37.5, d);
        Assert.Equal("37.5%", new Percentual(37.5).ToString());
        Assert.Equal("50%", new Percentual(50).ToString());
    }

    [Theory]
    [InlineData("42", 42)]
    [InlineData(" 7.5 % ", 7.5)]
    [InlineData("100%", 100)]
    public void Parse_aceita_numero_com_ou_sem_sinal(string texto, double esperado) =>
        Assert.Equal(esperado, Leitor.Ler<Percentual>(texto).Valor);

    [Theory]
    [InlineData("abc")]
    [InlineData("101")]
    [InlineData("")]
    [InlineData(null)]
    public void TryParse_recusa_sem_lancar_excecao(string? texto)
    {
        Assert.False(Leitor.TentarLer<Percentual>(texto, out var p));
        Assert.Equal(default, p);
    }

    [Fact]
    public void Parse_invalido_lanca_uma_mensagem_que_ajuda() =>
        Assert.Contains("0 a 100", Assert.Throws<FormatException>(() => Leitor.Ler<Percentual>("200")).Message);

    [Fact]
    public void O_leitor_generico_funciona_com_tipos_da_biblioteca_padrao_tambem()
    {
        Assert.Equal(42, Leitor.Ler<int>("42"));
        Assert.Equal(new DateOnly(2026, 11, 2), Leitor.Ler<DateOnly>("2026-11-02"));
    }

    [Theory]
    [InlineData("Lia@Exemplo.com", "lia@exemplo.com")]
    [InlineData("  caio@time.dev  ", "caio@time.dev")]
    public void Email_valido_e_normalizado(string texto, string esperado)
    {
        Assert.True(Email.TryCriar(texto, out var email));
        Assert.Equal(esperado, email!.ToString());
    }

    [Theory]
    [InlineData("")]
    [InlineData("sem-arroba")]
    [InlineData("a@@b.com")]
    [InlineData("a@b")]
    [InlineData("a b@c.com")]
    [InlineData(null)]
    public void Email_invalido_nao_vira_objeto(string? texto)
    {
        Assert.False(Email.TryCriar(texto, out var email));
        Assert.Null(email);
    }

    [Fact]
    public void Distribuicao_que_fecha_cem_e_aceita()
    {
        var d = Distribuicao.Criar(("Foco", new Percentual(50)), ("Reuniões", new Percentual(30)), ("Aprender", new Percentual(20)));
        Assert.Equal(3, d.Partes.Count);
    }

    [Fact]
    public void Distribuicao_que_nao_fecha_cem_diz_quanto_somou()
    {
        var e = Assert.Throws<ArgumentException>(() =>
            Distribuicao.Criar(("Foco", new Percentual(70)), ("Reuniões", new Percentual(40))));

        Assert.Contains("110%", e.Message);
    }

    [Fact]
    public void Distribuicao_pode_ser_lida_de_texto()
    {
        var d = Distribuicao.Ler("Foco profundo 50, Reuniões 30, Aprender 20");
        Assert.Equal("Foco profundo", d.Partes[0].Nome);
        Assert.Equal(50, d.Partes[0].Parte.Valor);
        Assert.Throws<FormatException>(() => Distribuicao.Ler("SóNome"));
    }
}
