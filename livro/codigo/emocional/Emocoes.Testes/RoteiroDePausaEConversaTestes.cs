using Jornada.Emocoes;

namespace Emocoes.Testes;

public sealed class RoteiroDePausaEConversaTestes
{
    [Fact]
    public void RegistroRejeitaDescricaoVazia()
    {
        var erro = Assert.Throws<ArgumentException>(() =>
            new RegistroEmocional(
                EmocaoNomeada.Alegria,
                " ",
                "Exemplo neutro",
                DateTimeOffset.UnixEpoch));

        Assert.Equal("sinalObservado", erro.ParamName);
    }

    [Fact]
    public void RegistroRejeitaOpcaoDeEmocaoInvalida()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new RegistroEmocional(
                (EmocaoNomeada)999,
                "Um sinal fictício",
                "Um contexto fictício",
                DateTimeOffset.UnixEpoch));
    }

    [Fact]
    public void RegistroPreservaOpcoesParaIncertezaEOsTextosInformados()
    {
        var registro = new RegistroEmocional(
            EmocaoNomeada.AindaNaoSei,
            "  Uma pausa antes de responder.  ",
            "Exemplo fictício.",
            DateTimeOffset.UnixEpoch);

        Assert.Equal(EmocaoNomeada.AindaNaoSei, registro.Emocao);
        Assert.Equal("Uma pausa antes de responder.", registro.SinalObservado);
    }

    [Fact]
    public void ObservacaoMantemFatoInterpretacaoEEmocaoEmCamposSeparados()
    {
        var observacao = new ObservacaoEmocional(
            "A reunião terminou sem responsáveis.",
            "Talvez minhas contribuições não tenham sido ouvidas.",
            "Frustração");

        Assert.Equal("A reunião terminou sem responsáveis.", observacao.FatoObservado);
        Assert.Contains("Talvez", observacao.Interpretacao, StringComparison.Ordinal);
        Assert.Equal("Frustração", observacao.EmocaoNomeada);
    }

    [Fact]
    public void ObservacaoPermiteNaoNomearUmaEmocao()
    {
        var observacao = new ObservacaoEmocional(
            "A reunião terminou sem responsáveis.",
            "Ainda não sei como interpretar o encerramento.");

        Assert.Null(observacao.EmocaoNomeada);
    }

    [Fact]
    public void PlanoDePausaRejeitaOpcaoInvalida()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new PlanoDePausa(
                (OpcaoDePausa)999,
                "Retomar depois.",
                "Pedir apoio."));
    }

    [Fact]
    public void RotulosEmocionaisApresentamUmaOpcaoSemAlterarSeuSignificado()
    {
        Assert.Equal("Ainda não sei", RotulosEmocionais.Descrever(EmocaoNomeada.AindaNaoSei));
        Assert.Equal("Prefiro não nomear", RotulosEmocionais.Descrever(EmocaoNomeada.PrefiroNaoNomear));
        Assert.Throws<ArgumentOutOfRangeException>(() => RotulosEmocionais.Descrever((EmocaoNomeada)999));
    }

    [Fact]
    public void RotulosDePausaTraduzemAOpcaoEscolhidaSemSelecionaLa()
    {
        Assert.Equal("Pedir um intervalo", RotulosDePausa.Descrever(OpcaoDePausa.PedirUmIntervalo));
        Assert.Throws<ArgumentOutOfRangeException>(() => RotulosDePausa.Descrever((OpcaoDePausa)999));
    }

    [Fact]
    public void PlanoDeRetomadaExigeInstanteExplicito()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new PlanoDeRetomada(
                "Investigar o teste.",
                "Comparar ambientes.",
                "Pedir uma revisão.",
                default));
    }

    [Fact]
    public void PedidoRejeitaCamposVazios()
    {
        var erro = Assert.Throws<ArgumentException>(() =>
            new PedidoDeConversa("Observação", "Impacto", ""));

        Assert.Equal("pedido", erro.ParamName);
    }

    [Fact]
    public void PedidoFormataOsCamposInformadosSemInferirIntencao()
    {
        var pedido = new PedidoDeConversa(
            "a alteração substituiu meu arquivo",
            "perdi vinte minutos refazendo a mudança",
            "você pode avisar antes?");

        Assert.Equal(
            "Observação: a alteração substituiu meu arquivo | Impacto: perdi vinte minutos refazendo a mudança | Pedido: você pode avisar antes?",
            pedido.Formatar());
    }

    [Fact]
    public void AcordoDeEquipeRejeitaRotinaVazia()
    {
        var erro = Assert.Throws<ArgumentException>(() =>
            new AcordoDeEquipe("", "Pedir intervalo", "Facilitador", "Retrospectiva"));

        Assert.Equal("rotina", erro.ParamName);
    }

    [Fact]
    public void RevisaoDePraticaPreservaUmaTentativaSemPontuacao()
    {
        var revisao = new RevisaoDePratica(
            "Esperei a colega terminar a explicação.",
            "Perguntei se ela havia concluído.",
            "Anotar a dúvida antes de responder.",
            "Pedir apoio se o prazo mudar.");

        Assert.Equal("Anotar a dúvida antes de responder.", revisao.ProximaTentativa);
    }

    [Fact]
    public void RevisaoDePraticaPermiteOmitirApoioNaoIdentificado()
    {
        var revisao = new RevisaoDePratica(
            "A conversa terminou sem decisão.",
            "Pedi para rever as opções.",
            "Combinar responsáveis antes da próxima reunião.");

        Assert.Null(revisao.ApoioOuLimite);
    }

    [Fact]
    public void RoteiroApresentaAsEscolhasInformadasSemClassificar()
    {
        var roteiro = new RoteiroDePausaEConversa(
            new RegistroEmocional(
                EmocaoNomeada.Frustracao,
                "A revisão terminou sem responsáveis definidos.",
                "Exemplo fictício.",
                DateTimeOffset.UnixEpoch),
            new ObservacaoEmocional(
                "A revisão terminou sem responsáveis definidos.",
                "O próximo passo ficou incerto.",
                "Frustração"),
            new PlanoDePausa(
                OpcaoDePausa.PedirUmIntervalo,
                "Retomar depois do almoço.",
                "Avisar a equipe se o prazo mudar."),
            new PlanoDeRetomada(
                "Rever as pendências.",
                "Listar responsáveis.",
                "Pedir revisão.",
                DateTimeOffset.UnixEpoch.AddDays(1)),
            new PedidoDeConversa(
                "A revisão terminou sem responsáveis definidos.",
                "O próximo passo ficou incerto.",
                "Podemos definir responsáveis antes de encerrar?"),
            new AcordoDeEquipe(
                "Registrar pendências.",
                "Pedir intervalo.",
                "Pessoa facilitadora.",
                "Na próxima retrospectiva."),
            new RevisaoDePratica(
                "A reunião terminou sem responsáveis.",
                "Pedi uma pausa.",
                "Definir responsáveis antes de encerrar.",
                "Pessoa facilitadora."));

        var linhas = roteiro.Apresentar();

        Assert.Contains("Opção escolhida: Pedir um intervalo", linhas);
        Assert.Contains("Pedido: Podemos definir responsáveis antes de encerrar?", linhas);
        Assert.Contains(linhas, linha => linha.StartsWith("Acordo de equipe:", StringComparison.Ordinal));
        Assert.Contains(linhas, linha => linha.StartsWith("Próxima tentativa:", StringComparison.Ordinal));
        Assert.Contains(linhas, linha => linha.StartsWith("Pergunta possível:", StringComparison.Ordinal));
        Assert.Contains(linhas, linha => linha.Contains("somente as informações fornecidas", StringComparison.Ordinal));
        Assert.DoesNotContain(linhas, linha => linha.Contains("pontuação", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public void RoteiroNaoExigeUmaReflexaoPessoal()
    {
        var roteiro = new RoteiroDePausaEConversa(
            new RegistroEmocional(
                EmocaoNomeada.PrefiroNaoNomear,
                "Um sinal fictício.",
                "Um contexto fictício.",
                DateTimeOffset.UnixEpoch),
            new ObservacaoEmocional(
                "Um fato fictício.",
                "Uma interpretação em aberto."),
            new PlanoDePausa(
                OpcaoDePausa.EncerrarPorAgora,
                "Encerrar a atividade fictícia.",
                "Retomar por um canal apropriado."),
            new PlanoDeRetomada(
                "Rever a atividade.",
                "Usar um exemplo neutro.",
                "Pedir orientação.",
                DateTimeOffset.UnixEpoch.AddDays(1)),
            new PedidoDeConversa(
                "A atividade foi interrompida.",
                "O próximo passo está em aberto.",
                "Podemos combinar um canal de retomada?"),
            new AcordoDeEquipe(
                "Rever pendências.",
                "Qualquer pessoa pode encerrar por agora.",
                "Pessoa facilitadora.",
                "Na próxima revisão."));

        Assert.DoesNotContain(roteiro.Apresentar(), linha => linha.StartsWith("Reflexão opcional:", StringComparison.Ordinal));
    }

    [Fact]
    public void PerguntasDeEscutaSaoExemplosNaoVazios()
    {
        Assert.NotEmpty(PerguntasDeEscuta.Exemplos);
        Assert.All(PerguntasDeEscuta.Exemplos, pergunta =>
            Assert.False(string.IsNullOrWhiteSpace(pergunta)));
    }
}
