using Jornada.Joao.Cap124;

namespace Joao.Testes;

public class Cap124Tests
{
    private static (Lanterna Lanterna, List<Registro> Linhas) Nova(Nivel minimo = Nivel.Detalhe)
    {
        var linhas = new List<Registro>();
        return (new Lanterna(minimo, linhas.Add, new RelogioDeTeste()), linhas);
    }

    [Fact]
    public void Abaixo_do_nivel_minimo_a_luz_nem_acende()
    {
        var (lanterna, linhas) = Nova(Nivel.Aviso);

        lanterna.Escrever(Nivel.Detalhe, "miudeza");
        lanterna.Escrever(Nivel.Informacao, "rotina");
        lanterna.Escrever(Nivel.Aviso, "atenção");
        lanterna.Escrever(Nivel.Erro, "problema");

        Assert.Equal(new[] { "atenção", "problema" }, linhas.Select(l => l.Mensagem));
    }

    [Fact]
    public void Os_campos_chegam_como_dados_e_nao_so_como_texto()
    {
        var (lanterna, linhas) = Nova();

        lanterna.Escrever(Nivel.Informacao, "pedido pago", ("pedido", 42), ("valor", 19.9m));

        Assert.Equal(42, linhas[0].Campos["pedido"]);
        Assert.Equal(19.9m, linhas[0].Campos["valor"]);
    }

    [Fact]
    public void Escopos_se_acumulam_e_desaparecem_ao_fechar()
    {
        var (lanterna, linhas) = Nova();

        using (lanterna.Escopo("entrega"))
        {
            using (lanterna.Escopo("revisão"))
                lanterna.Escrever(Nivel.Informacao, "dentro");

            lanterna.Escrever(Nivel.Informacao, "no meio");
        }

        lanterna.Escrever(Nivel.Informacao, "fora");

        Assert.Equal(new[] { "entrega", "revisão" }, linhas[0].Escopos);
        Assert.Equal(new[] { "entrega" }, linhas[1].Escopos);
        Assert.Empty(linhas[2].Escopos);
    }

    [Fact]
    public void Fechar_escopos_fora_de_ordem_e_um_erro_de_programacao()
    {
        var (lanterna, _) = Nova();

        var externo = lanterna.Escopo("externo");
        var interno = lanterna.Escopo("interno");

        Assert.Throws<InvalidOperationException>(() => externo.Dispose());
        interno.Dispose();
        externo.Dispose();                                  // agora, na ordem certa, funciona
    }

    [Fact]
    public void Descartar_duas_vezes_nao_faz_mal()
    {
        var (lanterna, linhas) = Nova();

        var escopo = lanterna.Escopo("a");
        escopo.Dispose();
        escopo.Dispose();

        lanterna.Escrever(Nivel.Informacao, "livre");
        Assert.Empty(linhas[0].Escopos);
    }

    [Fact]
    public void A_hora_vem_do_relogio_injetado_e_o_formato_e_legivel()
    {
        var (lanterna, linhas) = Nova();

        using (lanterna.Escopo("entrega"))
            lanterna.Escrever(Nivel.Aviso, "revisão pendente", ("dias", 2));

        Assert.Equal("[Aviso] 09:00:00 > entrega: revisão pendente {dias=2}", Lanterna.Formatar(linhas[0]));
    }

    [Fact]
    public void A_lanterna_apagada_aceita_tudo_e_nao_mostra_nada()
    {
        ILanterna semRegistro = LanternaApagada.Instancia;

        using (semRegistro.Escopo("qualquer"))
            semRegistro.Escrever(Nivel.Erro, "ninguém vê");        // não lança, não registra, não exige teste de nulo
    }

    [Fact]
    public async Task Cada_fluxo_de_execucao_tem_os_seus_proprios_escopos()
    {
        var registros = new System.Collections.Concurrent.ConcurrentBag<Registro>();
        var lanterna = new Lanterna(Nivel.Detalhe, registros.Add, new RelogioDeTeste());

        async Task Trabalhar(string nome)
        {
            using (lanterna.Escopo(nome))
            {
                await Task.Delay(5);
                lanterna.Escrever(Nivel.Informacao, "dentro");
                await Task.Delay(5);
            }
        }

        await Task.WhenAll(Enumerable.Range(1, 20).Select(i => Trabalhar($"tarefa {i}")));

        Assert.Equal(20, registros.Count);
        Assert.All(registros, r => Assert.Single(r.Escopos));        // nenhuma tarefa enxerga o escopo de outra
    }
}
