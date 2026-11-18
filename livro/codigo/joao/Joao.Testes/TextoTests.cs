using Jornada.Joao.Texto;

namespace Joao.Testes;

// Um teste por frase do versículo: o nome do teste diz o que o texto afirma.
public class TextoTests
{
    // ---- Jo 1,1-5
    [Fact]
    public void O_Verbo_estava_com_Deus_e_e_outro_objeto()
    {
        var deus = new Deus();
        var verbo = new Verbo { EstavaCom = deus };

        Assert.Same(deus, verbo.EstavaCom);
        Assert.NotSame(deus, verbo);
    }

    [Fact]
    public void O_Verbo_era_Deus_e_uma_relacao_entre_tipos_que_vale_sem_criar_objetos() =>
        Assert.True(typeof(Deus).IsAssignableFrom(typeof(Verbo)));

    [Fact]
    public void Ser_do_tipo_Deus_nao_e_ser_a_mesma_referencia()
    {
        var deus = new Deus();
        var verbo = new Verbo { EstavaCom = deus };

        Assert.IsAssignableFrom<Deus>(verbo);          // "é": teste de tipo
        Assert.NotSame(deus, verbo);                   // identidade na memória: outra pergunta, outra resposta
    }

    [Fact]
    public void A_vida_era_a_luz()
    {
        var verbo = new Verbo { Vida = new Vida() };

        Assert.IsAssignableFrom<Luz>(verbo.Vida);
    }

    [Fact]
    public void A_luz_resplandece_nas_trevas_e_as_trevas_nao_a_compreenderam()
    {
        var luz = new Luz();
        var trevas = new Trevas();

        luz.Resplandecer(trevas);

        Assert.True(trevas.Iluminadas);
        Assert.False(trevas.Compreendeu(luz));
    }

    // ---- as sete declarações
    [Fact]
    public void Sao_sete_declaracoes_com_passagem_imagem_e_promessa()
    {
        Assert.Equal(7, Declaracoes.Todas.Count);
        Assert.All(Declaracoes.Todas, d =>
        {
            Assert.StartsWith("Jo ", d.Passagem);
            Assert.False(string.IsNullOrWhiteSpace(d.EuSou));
            Assert.False(string.IsNullOrWhiteSpace(d.Promessa));
        });
    }

    [Fact]
    public void A_frase_repete_o_texto_do_versiculo() =>
        Assert.Equal("Jo 6,35: Eu sou o pão da vida; quem vem a mim de maneira nenhuma terá fome, e quem crê em mim nunca terá sede.",
                     Declaracoes.Todas[0].Frase());

    // ---- Jo 2,7-10
    [Fact]
    public void Encher_de_agua_os_vasos()
    {
        var vaso = new Vaso();

        vaso.Encher(new Bebida("água", 3));

        Assert.Equal("água", vaso.Conteudo);
    }

    [Fact]
    public void Vaso_so_aceita_agua()
    {
        var vaso = new Vaso();

        Assert.Throws<ArgumentException>(() => vaso.Encher(new Bebida("suco", 1)));
    }

    [Fact]
    public void Ao_tirar_a_agua_esta_feita_vinho()
    {
        var vaso = new Vaso();
        vaso.Encher(new Bebida("água", 3));

        var tirada = vaso.Tirar();

        Assert.Equal("vinho", tirada.Nome);
        Assert.Equal("vinho", vaso.Conteudo);
    }

    [Fact]
    public void Vaso_vazio_nao_tem_o_que_tirar() =>
        Assert.Throws<InvalidOperationException>(() => new Vaso().Tirar());

    [Fact]
    public void O_mestre_de_cerimonia_prova_e_nao_sabe_de_onde_veio()
    {
        var mestre = new MestreDeCerimonia();

        Assert.Equal("tu guardaste o bom vinho até agora", mestre.Provar(new Bebida("vinho", 9)));
        Assert.Equal("vinho comum", mestre.Provar(new Bebida("vinho", 4)));
        Assert.Equal("só água", mestre.Provar(new Bebida("água", 3)));
    }

    // ---- Jo 3,8
    [Fact]
    public void O_vento_sopra_e_quem_ouve_recebe_o_som()
    {
        var vento = new VentoSimples();
        var ouvinte = new OuvinteDoVento();
        vento.Soprou += ouvinte.Ouvir;

        vento.Soprar("shhh");

        Assert.Equal(new[] { "shhh" }, ouvinte.Ouvido);
    }

    [Fact]
    public void O_vento_sopra_mesmo_sem_ninguem_ouvindo()
    {
        var vento = new VentoSimples();

        var erro = Record.Exception(() => vento.Soprar("ninguém ouve"));

        Assert.Null(erro);
        Assert.Equal(0, vento.Ouvintes);
    }

    [Fact]
    public void Quem_deixa_de_ouvir_nao_recebe_mais()
    {
        var vento = new VentoSimples();
        var ouvinte = new OuvinteDoVento();
        vento.Soprou += ouvinte.Ouvir;
        vento.Soprar("um");

        vento.Soprou -= ouvinte.Ouvir;
        vento.Soprar("dois");

        Assert.Equal(new[] { "um" }, ouvinte.Ouvido);
    }

    // ---- Jo 4,11-14
    [Fact]
    public void Sem_fonte_dentro_toda_vez_e_preciso_descer_de_novo()
    {
        var poco = new PocoFundo();

        poco.TirarAgua(); poco.TirarAgua(); poco.TirarAgua();

        Assert.Equal(3, poco.Descidas);
    }

    [Fact]
    public void Com_fonte_dentro_desce_uma_vez_so()
    {
        var poco = new PocoFundo();
        var fonte = new FonteDentro(poco);

        fonte.Beber(); fonte.Beber(); fonte.Beber();

        Assert.Equal(1, poco.Descidas);
    }

    // ---- Jo 6,9-13
    [Fact]
    public void Cada_um_recebe_quanto_queria_enquanto_houver()
    {
        var distribuicao = new Distribuicao(Enumerable.Range(1, 10).Select(i => $"pedaço {i}"));

        distribuicao.Repartir(new[] { new Pessoa("A", 3), new Pessoa("B", 4) });

        Assert.Equal(3, distribuicao.Servido["A"]);
        Assert.Equal(4, distribuicao.Servido["B"]);
        Assert.Equal(3, distribuicao.Sobra.Count);
    }

    [Fact]
    public void Nada_se_perde_servidos_mais_recolhidos_e_o_total()
    {
        var distribuicao = new Distribuicao(Enumerable.Range(1, 12).Select(i => $"pedaço {i}"));

        distribuicao.Repartir(new[] { new Pessoa("A", 5), new Pessoa("B", 2) });
        var recolhidos = distribuicao.Recolher();

        Assert.Equal(12, distribuicao.Servido.Values.Sum() + recolhidos.Count);
        Assert.Empty(distribuicao.Sobra);
    }

    [Fact]
    public void Quando_nao_ha_o_bastante_ninguem_recebe_mais_do_que_existe()
    {
        var distribuicao = new Distribuicao(new[] { "a", "b", "c" });

        distribuicao.Repartir(new[] { new Pessoa("A", 2), new Pessoa("B", 2) });

        Assert.Equal(2, distribuicao.Servido["A"]);
        Assert.Equal(1, distribuicao.Servido["B"]);
        Assert.Empty(distribuicao.Recolher());
    }

    // ---- Jo 10,3-5
    [Fact]
    public void A_ovelha_segue_a_voz_que_conhece_e_foge_do_estranho()
    {
        var ovelha = new Ovelha("voz do pastor");

        Assert.True(ovelha.Segue("voz do pastor"));
        Assert.False(ovelha.Segue("voz de um estranho"));
    }

    // ---- Jo 11,43-44
    [Fact]
    public void Lazaro_sai_vivo_mas_ainda_atado()
    {
        var lazaro = new Lazaro();

        lazaro.Chamar();

        Assert.Equal(Condicao.Vivo, lazaro.Condicao);
        Assert.False(lazaro.Livre);
        Assert.Equal(3, lazaro.Ataduras.Count);
    }

    [Fact]
    public void Desatai_o_e_deixai_o_ir()
    {
        var lazaro = new Lazaro();
        lazaro.Chamar();

        foreach (var atadura in lazaro.Ataduras.ToList())
            lazaro.Desatar(atadura);

        Assert.True(lazaro.Livre);
    }

    [Fact]
    public void Desatar_o_que_nao_existe_nao_muda_nada()
    {
        var lazaro = new Lazaro();

        Assert.False(lazaro.Desatar("coroa"));
        Assert.Equal(3, lazaro.Ataduras.Count);
    }

    // ---- Jo 13,4-15
    [Fact]
    public void Com_a_toalha_continua_sendo_a_mesma_pessoa()
    {
        IPessoa mestre = new ComToalha(new Jesus());

        Assert.Equal("Jesus (Mestre e Senhor)", mestre.Nome);
    }

    [Fact]
    public void A_toalha_acrescenta_o_servico_na_ordem_do_texto()
    {
        var atos = new ComToalha(new Jesus()).Atos();

        Assert.Equal("está à mesa", atos[0]);
        Assert.Equal(new[] { "levanta-se da ceia", "tira as roupas", "envolve-se com uma toalha", "põe água em uma bacia", "lava os pés dos discípulos", "limpa-os com a toalha" },
                     atos.Skip(1));
    }

    [Fact]
    public void Vos_deveis_tambem_qualquer_um_pode_vestir_a_toalha()
    {
        var pedro = new ComToalha(new Discipulo("Pedro"));

        Assert.Equal("Pedro", pedro.Nome);
        Assert.Contains("lava os pés dos discípulos", pedro.Atos());
    }

    // ---- Jo 15,1-5
    [Fact]
    public void O_ramo_precisa_estar_na_videira()
    {
        Assert.Throws<ArgumentNullException>(() => new Ramo(null!));
    }

    [Fact]
    public void Ligado_a_videira_o_ramo_da_fruto_com_a_seiva_dela() =>
        Assert.Equal("fruto feito da seiva da videira", new Ramo(new Videira()).DarFruto());

    [Fact]
    public void O_lavrador_tira_o_ramo_que_nao_da_fruto_e_limpa_o_que_da()
    {
        var videira = new Videira();
        var ramos = new List<Ramo> { new(videira), new(videira) { DaFruto = false }, new(videira) };

        var (tirados, limpos) = new Lavrador().Podar(ramos);

        Assert.Single(tirados);
        Assert.Equal(2, limpos.Count);
        Assert.Equal(2, ramos.Count);
    }
}
