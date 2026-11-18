using System.Reflection;
using Jornada.Joao.Cap125;

namespace Joao.Testes;

public class Cap125Tests
{
    private static readonly Assembly Montagem = typeof(IImagem).Assembly;

    [Fact]
    public void A_reflexao_encontra_as_sete_imagens_e_ignora_quem_nao_tem_etiqueta()
    {
        var imagens = Catalogo.Descobrir(Montagem);

        Assert.Equal(7, imagens.Count);
        Assert.DoesNotContain(imagens, i => i.Tipo == typeof(ImagemSemEtiqueta));
    }

    [Fact]
    public void As_imagens_vem_na_ordem_do_texto_e_nao_na_do_codigo()
    {
        var textos = Catalogo.Descobrir(Montagem).Select(i => i.Texto).ToList();

        Assert.Equal(new[]
        {
            "o pão da vida", "a luz do mundo", "a porta", "o bom pastor",
            "a ressurreição, e a vida", "o caminho, e a verdade, e a vida", "a videira"
        }, textos);
    }

    [Theory]
    [InlineData("Jo 6,35", 6, 35)]
    [InlineData(" Jo 14,6 ", 14, 6)]
    public void Passagem_le_o_formato_do_texto(string texto, int capitulo, int versiculo)
    {
        Assert.True(Passagem.TryParse(texto, out var p));
        Assert.Equal(new Passagem(capitulo, versiculo), p);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("Mt 5,3")]
    [InlineData("Jo 6")]
    [InlineData("Jo x,y")]
    public void Passagem_recusa_o_que_nao_entende_sem_lancar_excecao(string? texto) =>
        Assert.False(Passagem.TryParse(texto, out _));

    [Fact]
    public void Passagens_se_ordenam_por_capitulo_e_depois_por_versiculo()
    {
        var lista = new[] { new Passagem(10, 11), new Passagem(6, 35), new Passagem(10, 7) };

        Assert.Equal(new[] { "Jo 6,35", "Jo 10,7", "Jo 10,11" }, lista.Order().Select(p => p.ToString()));
    }

    [Fact]
    public void Cria_instancias_sem_conhecer_o_tipo_na_hora_de_escrever()
    {
        var pastor = Catalogo.Descobrir(Montagem).Single(i => i.Texto == "o bom pastor");

        IImagem servico = Catalogo.Criar(pastor);

        Assert.IsType<Pastor>(servico);
        Assert.Contains("dá sua vida pelas ovelhas", servico.Promete());
    }

    [Fact]
    public void O_atributo_guarda_o_que_foi_declarado()
    {
        var marca = typeof(Videira).GetCustomAttribute<EuSouAttribute>();

        Assert.NotNull(marca);
        Assert.Equal("a videira", marca!.Imagem);
        Assert.Equal("Jo 15,5", marca.Referencia);
        Assert.Equal("dá muito fruto", marca.Promessa);
    }
}
