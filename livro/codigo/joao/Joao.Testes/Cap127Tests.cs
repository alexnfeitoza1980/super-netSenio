using Jornada.Joao.Cap127;

namespace Joao.Testes;

public class Cap127Tests
{
    [Fact]
    public void Quem_se_inscreve_ouve_o_que_o_vento_trouxer_sem_saber_quem_sopra()
    {
        var vento = new Vento<string>();
        var ouvido = new List<string>();

        vento.Ouvir(ouvido.Add);
        vento.Soprar("um");
        vento.Soprar("dois");

        Assert.Equal(new[] { "um", "dois" }, ouvido);
    }

    [Fact]
    public void Varios_ouvintes_recebem_o_mesmo_sopro()
    {
        var vento = new Vento<int>();
        int a = 0, b = 0;

        vento.Ouvir(x => a += x);
        vento.Ouvir(x => b += x * 10);
        vento.Soprar(3);

        Assert.Equal((3, 30), (a, b));
    }

    [Fact]
    public void Descartar_a_inscricao_para_de_ouvir()
    {
        var vento = new Vento<int>();
        var ouvido = new List<int>();

        var inscricao = vento.Ouvir(ouvido.Add);
        vento.Soprar(1);
        inscricao.Dispose();
        vento.Soprar(2);

        Assert.Equal(new[] { 1 }, ouvido);
        Assert.Equal(0, vento.Ouvintes);
    }

    [Fact]
    public void Operadores_se_encadeiam_como_no_LINQ()
    {
        var vento = new Vento<int>();
        var ouvido = new List<string>();

        vento.Onde(n => n % 2 == 0)
             .Transformar(n => $"par {n}")
             .Ouvir(ouvido.Add);

        foreach (var n in Enumerable.Range(1, 6))
            vento.Soprar(n);

        Assert.Equal(new[] { "par 2", "par 4", "par 6" }, ouvido);
    }

    [Fact]
    public void Depois_de_completar_nada_mais_chega_e_quem_chega_tarde_so_ouve_o_fim()
    {
        var vento = new Vento<int>();
        var ouvido = new List<int>();
        int completou = 0;

        vento.Ouvir(ouvido.Add, () => completou++);
        vento.Soprar(1);
        vento.Completar();
        vento.Soprar(2);                                    // ignorado

        vento.Ouvir(_ => Assert.Fail("não deveria ouvir nada"), () => completou++);

        Assert.Equal(new[] { 1 }, ouvido);
        Assert.Equal(2, completou);
    }

    [Fact]
    public void Um_erro_avisa_todo_mundo_e_encerra()
    {
        var vento = new Vento<int>();
        Exception? recebido = null;

        vento.Ouvir(_ => { }, aoFalhar: e => recebido = e);
        vento.Falhar(new InvalidOperationException("tempestade"));

        Assert.Equal("tempestade", recebido!.Message);
    }

    [Fact]
    public void Com_memoria_quem_nasce_de_novo_recebe_o_que_ja_passou()
    {
        var vento = new VentoComMemoria<string>(capacidade: 2);
        vento.Soprar("a");
        vento.Soprar("b");
        vento.Soprar("c");                                  // "a" foi esquecido

        var recebido = new List<string>();
        vento.Ouvir(recebido.Add);
        vento.Soprar("d");

        Assert.Equal(new[] { "b", "c", "d" }, recebido);
    }

    [Fact]
    public void Ouvinte_que_sai_durante_o_sopro_nao_quebra_os_outros()
    {
        var vento = new Vento<int>();
        var ouvido = new List<int>();
        IDisposable? primeira = null;

        primeira = vento.Ouvir(_ => primeira!.Dispose());   // sai logo no primeiro sopro
        vento.Ouvir(ouvido.Add);

        vento.Soprar(1);
        vento.Soprar(2);

        Assert.Equal(new[] { 1, 2 }, ouvido);
    }
}
