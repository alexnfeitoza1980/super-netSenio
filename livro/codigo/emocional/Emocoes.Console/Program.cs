using Jornada.Emocoes;

var registro = new RegistroEmocional(
    EmocaoNomeada.Frustracao,
    "A revisão terminou sem responsáveis definidos.",
    "Exemplo fictício de uma reunião de projeto.",
    DateTimeOffset.Parse("2026-10-29T10:00:00-03:00"));

var observacao = new ObservacaoEmocional(
    "A revisão terminou sem responsáveis definidos.",
    "Talvez as pendências tenham ficado pouco claras.",
    "Frustração");

var pausa = new PlanoDePausa(
    OpcaoDePausa.PedirUmIntervalo,
    "Retomar a revisão depois do almoço.",
    "Se o prazo mudar, avisar a equipe.");

var retomada = new PlanoDeRetomada(
    "Revisar as pendências abertas.",
    "Listar responsáveis antes de encerrar.",
    "Pedir à pessoa facilitadora que registre as decisões.",
    DateTimeOffset.Parse("2026-10-29T13:00:00-03:00"));

var pedido = new PedidoDeConversa(
    "A revisão terminou sem responsáveis definidos.",
    "Ficou incerto quem valida o próximo passo.",
    "Podemos definir responsáveis antes de encerrar?");

var acordo = new AcordoDeEquipe(
    "Encerrar revisões listando decisões e pendências.",
    "Qualquer pessoa pode pedir intervalo e combinar a retomada.",
    "A pessoa facilitadora ajuda a distribuir a conversa.",
    "Rever o formato na retrospectiva seguinte.");

var revisao = new RevisaoDePratica(
    "A reunião terminou sem definir responsáveis.",
    "Pedi para rever as opções.",
    "Combinar responsáveis antes da próxima revisão.",
    "Pedir apoio à pessoa facilitadora.");

var roteiro = new RoteiroDePausaEConversa(
    registro,
    observacao,
    pausa,
    retomada,
    pedido,
    acordo,
    revisao);

foreach (var linha in roteiro.Apresentar())
    Console.WriteLine(linha);
