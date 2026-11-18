using Jornada.Joao.Cap126;

namespace Joao.Testes;

public class Cap126Tests
{
    [Fact]
    public void Transformacoes_se_encadeiam_e_podem_mudar_de_tipo()
    {
        var contarLetras = new Transformacao<string, int>(texto => texto.Length)
            .Depois(n => n * 2)
            .Depois(n => $"dobro: {n}");

        Assert.Equal("dobro: 10", contarLetras.Aplicar("água!"));
    }

    [Fact]
    public void Da_agua_ao_vinho_muda_o_nome_e_melhora_a_qualidade()
    {
        var vinho = new Banquete().DaAguaAoVinho.Aplicar(new Liquido("água", 1));

        Assert.Equal("vinho", vinho.Nome);
        Assert.Equal(5, vinho.Qualidade);
    }

    [Fact]
    public void Nada_e_transformado_ate_que_alguem_prove()
    {
        var banquete = new Banquete();

        var taças = banquete.Servir(6);                    // só uma "receita": nada aconteceu ainda

        Assert.Equal(0, banquete.Transformadas);
        _ = taças.First();
        Assert.Equal(1, banquete.Transformadas);
    }

    [Fact]
    public void Quem_prova_so_as_duas_primeiras_causa_so_duas_transformacoes()
    {
        var banquete = new Banquete();

        var provadas = banquete.Servir(6).Take(2).ToList();

        Assert.Equal(2, provadas.Count);
        Assert.Equal(2, banquete.Transformadas);
    }

    [Fact]
    public void Servir_tudo_transforma_as_seis_vasos_e_o_melhor_fica_para_o_fim()
    {
        var banquete = new Banquete();

        var todas = banquete.Servir(6).ToList();

        Assert.Equal(6, banquete.Transformadas);
        Assert.Equal(todas.Max(t => t.Conteudo.Qualidade), todas[^1].Conteudo.Qualidade);
        Assert.Equal("o melhor ficou para o fim", Provador.Avaliar(todas[^1].Conteudo));
    }

    [Theory]
    [InlineData("água", 3, "só água")]
    [InlineData("vinho", 5, "vinho")]
    [InlineData("vinho", 7, "vinho muito bom")]
    [InlineData("vinho", 10, "o melhor ficou para o fim")]
    public void O_provador_avalia_so_o_que_esta_na_taca(string nome, double qualidade, string esperado) =>
        Assert.Equal(esperado, Provador.Avaliar(new Liquido(nome, qualidade)));

    [Fact]
    public void AplicarA_tambem_e_adiado()
    {
        int chamadas = 0;
        var t = new Transformacao<int, int>(x => { chamadas++; return x * x; });

        var quadrados = t.AplicarA(new[] { 1, 2, 3, 4 });

        Assert.Equal(0, chamadas);
        Assert.Equal(new[] { 1, 4 }, quadrados.Take(2));
        Assert.Equal(2, chamadas);
    }
}
