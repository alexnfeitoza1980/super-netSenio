using Jornada.Joao.Cap124;
using Jornada.Joao.Lab14;

namespace Joao.Testes;

public class Lab14Tests
{
    private static readonly Concordancia Conc = Concordancia.Embutida();

    [Fact]
    public void Os_dados_embutidos_tem_o_tamanho_do_texto_grego()
    {
        Assert.Equal(15438, Conc.Dados.TotalDePalavras);
        Assert.Equal(21, Conc.Dados.PalavrasPorCapitulo.Count);
        Assert.Equal(66, Conc.Dados.Lemas.Count);
        Assert.All(Conc.Dados.Lemas, l => Assert.Equal(l.Total, l.PorCapitulo.Values.Sum()));   // a soma por capítulo fecha com o total
    }

    [Theory]
    [InlineData("pisteuō", 98)]          // crer
    [InlineData("menō", 40)]             // permanecer
    [InlineData("sēmeion", 17)]          // sinal
    [InlineData("logos", 40)]            // palavra
    [InlineData("katalambanō", 2)]         // apanhar, alcançar, compreender
    [InlineData("skēnoō", 1)]         // armar tenda, habitar
    [InlineData("anōthen", 5)]         // do alto, de novo
    [InlineData("pneō", 2)]         // soprar
    [InlineData("hydria", 3)]         // jarro de água
    [InlineData("antleō", 4)]         // tirar água
    [InlineData("pēgē", 3)]         // fonte, nascente
    [InlineData("phrear", 2)]         // poço, cisterna
    [InlineData("paidarion", 1)]         // menino
    [InlineData("opsarion", 5)]         // peixe
    [InlineData("klasma", 2)]         // pedaço
    [InlineData("kophinos", 1)]         // cesto
    [InlineData("synagō", 7)]         // reunir, recolher
    [InlineData("apollymi", 10)]         // perder, destruir
    [InlineData("phōnē", 15)]         // voz, som
    [InlineData("allotrios", 2)]         // estranho
    [InlineData("misthōtos", 2)]         // assalariado
    [InlineData("lyō", 6)]         // soltar, desatar
    [InlineData("aphiēmi", 15)]         // deixar, soltar, perdoar
    [InlineData("dakryō", 1)]         // chorar
    [InlineData("keiria", 1)]         // atadura, faixa
    [InlineData("soudarion", 2)]         // lenço
    [InlineData("lention", 2)]         // toalha
    [InlineData("niptēr", 1)]         // bacia
    [InlineData("niptō", 13)]         // lavar
    [InlineData("hypodeigma", 1)]         // exemplo
    [InlineData("doulos", 11)]         // servo
    [InlineData("klēma", 4)]         // ramo
    [InlineData("karpos", 10)]         // fruto
    [InlineData("kathairō", 1)]         // limpar, podar
    [InlineData("airō", 26)]         // tirar, levantar
    [InlineData("geōrgos", 1)]         // lavrador
    [InlineData("akoloutheō", 19)]         // seguir
    [InlineData("Lazaros", 11)]         // Lázaro
    [InlineData("Nikodēmos", 5)]         // Nicodemos
    [InlineData("oinos", 6)]         // vinho
    [InlineData("architriklinos", 3)]         // mestre de cerimônia
    [InlineData("nyx", 6)]         // noite
    [InlineData("dipsaō", 6)]         // ter sede
    public void Os_totais_conferem_com_o_texto_original(string translit, int total) =>
        Assert.Equal(total, Conc.Buscar(translit)!.Total);

    [Fact]
    public void Buscar_aceita_grego_transliteracao_ou_sentido_sem_ligar_para_maiusculas()
    {
        Assert.Same(Conc.Buscar("μένω"), Conc.Buscar("MENŌ"));
        Assert.Same(Conc.Buscar("permanecer"), Conc.Buscar("menō"));
        Assert.Null(Conc.Buscar("inexistente"));
    }

    [Fact]
    public void Permanecer_se_concentra_no_capitulo_quinze()
    {
        var menō = Conc.Buscar("menō")!;

        var pico = Conc.Pico(menō)!.Value;

        Assert.Equal(15, pico.Capitulo);
        Assert.Equal(11, menō.Em(15));
        Assert.InRange(Conc.Concentracao(menō, 15), 0.27, 0.28);      // 11 de 40
    }

    [Fact]
    public void A_densidade_compara_capitulos_de_tamanhos_diferentes()
    {
        var menō = Conc.Buscar("menō")!;

        // capítulo 15 tem 500 palavras e 11 ocorrências → 22 a cada mil
        Assert.Equal(22.0, Conc.Densidade(menō, 15), 6);
        Assert.Equal(0.0, Conc.Densidade(menō, 13), 6);
    }

    [Fact]
    public void O_ranking_mostra_as_palavras_mais_frequentes()
    {
        var topo = Conc.MaisFrequentes(3).Select(l => l.Translit).ToList();

        Assert.Equal(new[] { "patēr", "pisteuō", "oida" }, topo);
    }

    [Fact]
    public void O_texto_tem_vinte_e_quatro_eu_sou_e_treze_com_uma_das_sete_imagens()
    {
        var euSou = Conc.Dados.EuSou;

        Assert.Equal(24, euSou.Count);
        Assert.Equal(13, euSou.Count(e => e.Predicado is not null));
        Assert.Contains(euSou, e => e.Capitulo == 14 && e.Versiculo == 6 && e.Predicado == "o caminho, a verdade e a vida");
    }

    [Fact]
    public void As_barras_tem_uma_linha_por_capitulo_e_a_maior_ocupa_a_largura_toda()
    {
        var linhas = Conc.Barras(Conc.Buscar("menō")!, largura: 20).Split('\n');

        Assert.Equal(21, linhas.Length);
        Assert.Contains(new string('█', 20), linhas[14]);             // capítulo 15
    }

    // ---------------- o mapa ----------------

    private static readonly Mapa Roteiros = Mapa.Padrao();

    [Fact]
    public void Menos_paradas_usa_busca_em_largura()
    {
        var rota = Roteiros.MenosParadas("jordao", "betania")!;

        Assert.Equal(new[] { "Além do Jordão", "Jerusalém", "Betânia" }, rota.Paradas.Select(p => p.Nome));
        Assert.Equal(2, rota.Trechos);
    }

    [Fact]
    public void Menor_tempo_pode_escolher_um_caminho_com_mais_paradas()
    {
        // Cafarnaum → Jerusalém: direto são 5 dias; por Caná e Jordão: 1 + 3 + 2 = 6; por Caná, Samaria: 1+3+3 = 7.
        var rapida = Roteiros.MenorTempo("cafarnaum", "jerusalem")!;
        var curta = Roteiros.MenosParadas("cafarnaum", "jerusalem")!;

        Assert.Equal(5, rapida.Dias);
        Assert.Equal(curta.Paradas.Select(p => p.Id), rapida.Paradas.Select(p => p.Id));

        // Caná → Jerusalém: por Jordão (3+2=5) vence Samaria (3+3=6), embora ambos passem por 3 lugares.
        Assert.Equal(5, Roteiros.MenorTempo("cana", "jerusalem")!.Dias);
        Assert.Contains("jordao", Roteiros.MenorTempo("cana", "jerusalem")!.Paradas.Select(p => p.Id));
    }

    [Fact]
    public void Origem_igual_ao_destino_e_uma_rota_de_zero_dias()
    {
        var rota = Roteiros.MenorTempo("cana", "cana")!;
        Assert.Equal(0, rota.Dias);
        Assert.Single(rota.Paradas);
    }

    [Fact]
    public void Lugar_desconhecido_e_um_erro_claro_e_trecho_invalido_e_recusado()
    {
        Assert.Throws<KeyNotFoundException>(() => Roteiros.MenorTempo("cana", "atlantida"));
        Assert.Throws<ArgumentException>(() => new Mapa(new[] { new Lugar("a", "A", "") }, new[] { new Trecho("a", "b", 1) }));
    }

    [Fact]
    public void Sem_ligacao_nao_ha_rota()
    {
        var isolado = new Mapa(new[] { new Lugar("a", "A", ""), new Lugar("b", "B", "") }, Array.Empty<Trecho>());
        Assert.Null(isolado.MenosParadas("a", "b"));
        Assert.Null(isolado.MenorTempo("a", "b"));
    }

    [Fact]
    public void O_guia_registra_com_a_lanterna_e_reaproveita_rotas_pelo_poco()
    {
        var linhas = new List<Registro>();
        var guia = new Guia(Roteiros, new Lanterna(Nivel.Detalhe, linhas.Add, new RelogioDeTeste()), new RelogioDeTeste());

        var primeira = guia.Rota("jordao", "betania");
        var segunda = guia.Rota("jordao", "betania");

        Assert.Same(primeira, segunda);
        Assert.Equal(1, guia.RotasReaproveitadas);
        Assert.Equal(new[] { "rota jordao → betania" }, linhas[0].Escopos);
        Assert.Equal(3, linhas[0].Campos["dias"]);
    }
}
