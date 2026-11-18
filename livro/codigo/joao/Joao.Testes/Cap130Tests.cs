using System.Text;
using Jornada.Joao.Cap130;

namespace Joao.Testes;

public class Cap130Tests
{
    private static readonly byte[] Segredo = Encoding.UTF8.GetBytes("segredo-de-estudo-com-mais-de-16-bytes");

    private static (Voz Voz, RelogioDeTeste Relogio) Nova()
    {
        var relogio = new RelogioDeTeste();
        return (new Voz(Segredo, relogio), relogio);
    }

    [Fact]
    public void Uma_voz_emitida_e_reconhecida_com_nome_e_perfil()
    {
        var (voz, _) = Nova();

        var token = voz.Emitir("Ana", "cuidadora", TimeSpan.FromHours(1));

        Assert.Equal(Acesso.Permitido, voz.Reconhecer(token, out var quem));
        Assert.Equal("Ana", quem!.Nome);
        Assert.Equal("cuidadora", quem.Perfil);
    }

    [Fact]
    public void Voz_adulterada_e_estranha_mesmo_que_pareca_certa()
    {
        var (voz, _) = Nova();
        var token = voz.Emitir("Beto", "visitante", TimeSpan.FromHours(1));

        // o ladrão troca o corpo (o que diz quem é) mas não sabe refazer a assinatura
        var corpoFalso = Convert.ToBase64String(Encoding.UTF8.GetBytes("Beto|admin|9999999999")).TrimEnd('=');
        var falsificado = corpoFalso + "." + token.Split('.')[1];

        Assert.Equal(Acesso.Estranho, voz.Reconhecer(falsificado, out var quem));
        Assert.Null(quem);
    }

    [Fact]
    public void Voz_de_outro_pastor_com_outro_segredo_nao_e_reconhecida()
    {
        var (voz, _) = Nova();
        var outro = new Voz(Encoding.UTF8.GetBytes("outro-segredo-qualquer-0123456789"), new RelogioDeTeste());

        var token = outro.Emitir("Lia", "dev", TimeSpan.FromHours(1));

        Assert.Equal(Acesso.Estranho, voz.Reconhecer(token, out _));
    }

    [Fact]
    public void Com_o_tempo_a_voz_expira()
    {
        var (voz, relogio) = Nova();
        var token = voz.Emitir("Caio", "dev", TimeSpan.FromMinutes(10));

        relogio.Avancar(TimeSpan.FromMinutes(9));
        Assert.Equal(Acesso.Permitido, voz.Reconhecer(token, out _));

        relogio.Avancar(TimeSpan.FromMinutes(2));
        Assert.Equal(Acesso.Expirado, voz.Reconhecer(token, out _));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("sem-ponto")]
    [InlineData("a.b.c")]
    [InlineData("corpo.@@@")]
    public void Lixo_e_malformado_e_nunca_derruba_o_programa(string? token)
    {
        var (voz, _) = Nova();
        Assert.Equal(Acesso.Malformado, voz.Reconhecer(token, out _));
    }

    [Fact]
    public void Segredo_curto_demais_e_recusado() =>
        Assert.Throws<ArgumentException>(() => new Voz(new byte[8]));

    [Fact]
    public void Nome_com_separador_e_recusado_para_nao_forjar_campos()
    {
        var (voz, _) = Nova();
        Assert.Throws<ArgumentException>(() => voz.Emitir("Ana|admin", "dev", TimeSpan.FromHours(1)));
    }

    [Fact]
    public void A_porta_confere_a_voz_e_depois_a_politica()
    {
        var (voz, _) = Nova();
        var porta = new Porta(voz);
        var token = voz.Emitir("Ana", "cuidadora", TimeSpan.FromHours(1));

        Assert.Equal(Acesso.Permitido, porta.Entrar(token));
        Assert.Equal(Acesso.Permitido, porta.Entrar(token, id => id.Perfil == "cuidadora"));
        Assert.Equal(Acesso.SemPermissao, porta.Entrar(token, id => id.Perfil == "admin"));
        Assert.Equal(Acesso.Estranho, porta.Entrar(token + "x"));
        Assert.Equal(Acesso.Malformado, porta.Entrar(null));
    }
}
