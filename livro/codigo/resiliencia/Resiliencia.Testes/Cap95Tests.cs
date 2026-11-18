using Jornada.Resiliencia.Cap95;

namespace Resiliencia.Testes;

public class Cap95Tests
{
    [Theory]
    [InlineData("1", RespostaDoCheckIn.Bem)]
    [InlineData("2", RespostaDoCheckIn.MaisOuMenos)]
    [InlineData(" 3 ", RespostaDoCheckIn.NaoEstouBem)]
    [InlineData("4", RespostaDoCheckIn.PreferoNaoDizer)]
    public void Numeros_de_1_a_4_sao_lidos(string entrada, RespostaDoCheckIn esperado)
    {
        var r = CheckIn.Ler(entrada).Combinar(valor => valor, _ => throw new Exception("não era para falhar"));
        Assert.Equal(esperado, r);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Silencio_e_uma_resposta_valida(string? entrada)
    {
        var r = CheckIn.Ler(entrada).Combinar(valor => valor, _ => throw new Exception("silêncio não é erro"));
        Assert.Equal(RespostaDoCheckIn.PreferoNaoDizer, r);
    }

    [Fact]
    public void Entrada_estranha_vira_uma_falha_gentil()
    {
        var mensagem = CheckIn.Ler("talvez").Combinar(_ => "", erro => erro);
        Assert.Contains("1 a 4", mensagem);
    }

    [Theory]
    [InlineData(RespostaDoCheckIn.Bem)]
    [InlineData(RespostaDoCheckIn.MaisOuMenos)]
    [InlineData(RespostaDoCheckIn.NaoEstouBem)]
    [InlineData(RespostaDoCheckIn.PreferoNaoDizer)]
    public void Nenhum_acolhimento_cobra_positividade(RespostaDoCheckIn resposta)
    {
        var texto = CheckIn.Acolher(resposta).ToLowerInvariant();

        foreach (var palavra in new[] { "sorria", "seja forte", "pense positivo", "supere", "ânimo", "força" })
            Assert.DoesNotContain(palavra, texto);
    }

    [Fact]
    public void Limite_completo_vira_uma_frase_clara()
    {
        var frase = Limite.Criar("as mensagens chegam depois das 22h", "sem tempo para descansar",
                                 "combinemos responder só até as 20h")
                          .Combinar(l => l.Montar(), e => e);

        Assert.Equal(
            "Quando as mensagens chegam depois das 22h, eu me sinto sem tempo para descansar. Peço que combinemos responder só até as 20h.",
            frase);
    }

    [Fact]
    public void Limite_incompleto_lista_tudo_que_falta_de_uma_vez()
    {
        var erro = Limite.Criar("algo acontece", null, "  ").Combinar(_ => "", e => e);

        Assert.Contains("como isso afeta", erro);
        Assert.Contains("pedido concreto", erro);
        Assert.DoesNotContain("o que acontece", erro);
    }
}
