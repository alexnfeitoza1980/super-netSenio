using Jornada.Crencas;

namespace Crencas.Testes;

public class CadernoDeHipotesesTests
{
    [Fact]
    public void Sem_observacoes_retorna_estado_vazio()
    {
        var investigacao = CriarInvestigacao();

        Assert.Equal(SinteseDaInvestigacao.SemObservacoes, investigacao.Sintetizar());
    }

    [Theory]
    [InlineData(RelacaoDaEvidencia.Apoia, SinteseDaInvestigacao.SoApoio)]
    [InlineData(RelacaoDaEvidencia.Contradiz, SinteseDaInvestigacao.SoContradicao)]
    [InlineData(RelacaoDaEvidencia.Contextualiza, SinteseDaInvestigacao.SoContexto)]
    public void Uma_categoria_retorna_sua_sintese(
        RelacaoDaEvidencia relacao,
        SinteseDaInvestigacao esperada)
    {
        var investigacao = CriarInvestigacao();
        investigacao.Registrar(new Evidencia("Observação fictícia.", relacao));

        Assert.Equal(esperada, investigacao.Sintetizar());
    }

    [Theory]
    [InlineData(RelacaoDaEvidencia.Apoia, RelacaoDaEvidencia.Contradiz, SinteseDaInvestigacao.ApoioEContradicao)]
    [InlineData(RelacaoDaEvidencia.Apoia, RelacaoDaEvidencia.Contextualiza, SinteseDaInvestigacao.ApoioEContexto)]
    [InlineData(RelacaoDaEvidencia.Contradiz, RelacaoDaEvidencia.Contextualiza, SinteseDaInvestigacao.ContradicaoEContexto)]
    public void Duas_categorias_sao_preservadas_na_sintese(
        RelacaoDaEvidencia primeira,
        RelacaoDaEvidencia segunda,
        SinteseDaInvestigacao esperada)
    {
        var investigacao = CriarInvestigacao();
        investigacao.Registrar(new Evidencia("Primeira observação fictícia.", primeira));
        investigacao.Registrar(new Evidencia("Segunda observação fictícia.", segunda));

        Assert.Equal(esperada, investigacao.Sintetizar());
    }

    [Fact]
    public void Tres_categorias_sao_preservadas_na_sintese()
    {
        var investigacao = CriarInvestigacao();
        investigacao.Registrar(new Evidencia("Apoio fictício.", RelacaoDaEvidencia.Apoia));
        investigacao.Registrar(new Evidencia("Contraponto fictício.", RelacaoDaEvidencia.Contradiz));
        investigacao.Registrar(new Evidencia("Contexto fictício.", RelacaoDaEvidencia.Contextualiza));

        Assert.Equal(
            SinteseDaInvestigacao.ApoioContradicaoEContexto,
            investigacao.Sintetizar());
    }

    [Fact]
    public void Evidencia_vazia_e_recusada()
    {
        var investigacao = CriarInvestigacao();

        Assert.Throws<ArgumentException>(() =>
            investigacao.Registrar(new Evidencia(" ", RelacaoDaEvidencia.Apoia)));
    }

    [Fact]
    public void Busca_ignora_diferencas_de_maiusculas()
    {
        var caderno = new CadernoDeHipoteses();
        var investigacao = caderno.Adicionar(new Hipotese(
            "Um exemplo ajuda.",
            OrigemDaCrenca.Desconhecida));

        Assert.Same(investigacao, caderno.Encontrar("UM EXEMPLO AJUDA."));
    }

    [Fact]
    public void Plano_exige_acao_observacao_e_limite_ou_apoio()
    {
        Assert.Throws<ArgumentException>(() =>
            new PlanoDeExperimento(" ", "Observar algo.", "Pedir ajuda se necessário."));
    }

    [Fact]
    public void Modelos_didaticos_preservam_campos_distintos()
    {
        var observacao = new Observacao("Teste falhou.", "Não sei programar.", "Frustração.");
        var acao = new AcaoPossivel(
            "Pedir um exemplo.",
            AlcanceDaAcao.Compartilhado,
            "Colega");
        var evento = new EventoDeAprendizagem(
            "Vou errar.",
            "Evitei o exercício.",
            "Pratiquei menos.",
            "Tutorial longo, sem suporte.");

        Assert.Equal("Teste falhou.", observacao.Fato);
        Assert.Equal("Não sei programar.", observacao.Interpretacao);
        Assert.Equal(AlcanceDaAcao.Compartilhado, acao.Alcance);
        Assert.Equal("Colega", acao.ApoioNecessario);
        Assert.Equal("Tutorial longo, sem suporte.", evento.Contexto);
    }

    [Fact]
    public void Relacao_de_evidencia_invalida_e_recusada()
    {
        var investigacao = CriarInvestigacao();

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            investigacao.Registrar(new Evidencia("Observação fictícia.", (RelacaoDaEvidencia)99)));
    }

    private static Investigacao CriarInvestigacao() =>
        new(new Hipotese("Hipótese técnica fictícia.", OrigemDaCrenca.Desconhecida));
}
