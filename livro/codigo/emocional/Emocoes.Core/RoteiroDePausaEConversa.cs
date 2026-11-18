namespace Jornada.Emocoes;

public sealed class RoteiroDePausaEConversa
{
    private readonly RegistroEmocional _registro;
    private readonly ObservacaoEmocional _observacao;
    private readonly PlanoDePausa _pausa;
    private readonly PlanoDeRetomada _retomada;
    private readonly PedidoDeConversa _pedido;
    private readonly AcordoDeEquipe _acordo;
    private readonly RevisaoDePratica? _revisao;

    public RoteiroDePausaEConversa(
        RegistroEmocional registro,
        ObservacaoEmocional observacao,
        PlanoDePausa pausa,
        PlanoDeRetomada retomada,
        PedidoDeConversa pedido,
        AcordoDeEquipe acordo,
        RevisaoDePratica? revisao = null)
    {
        _registro = registro ?? throw new ArgumentNullException(nameof(registro));
        _observacao = observacao ?? throw new ArgumentNullException(nameof(observacao));
        _pausa = pausa ?? throw new ArgumentNullException(nameof(pausa));
        _retomada = retomada ?? throw new ArgumentNullException(nameof(retomada));
        _pedido = pedido ?? throw new ArgumentNullException(nameof(pedido));
        _acordo = acordo ?? throw new ArgumentNullException(nameof(acordo));
        _revisao = revisao;
    }

    public IReadOnlyList<string> Apresentar()
    {
        var linhas = new List<string>
        {
            "Exemplo fictício — roteiro educativo, sem avaliação.",
            $"Sinal observado: {_registro.SinalObservado}",
            $"Contexto: {_registro.Contexto}",
            $"Palavra escolhida: {RotulosEmocionais.Descrever(_registro.Emocao)}",
            $"Fato descrito: {_observacao.FatoObservado}",
            $"Interpretação informada: {_observacao.Interpretacao}",
            $"Palavra emocional opcional: {_observacao.EmocaoNomeada ?? "não informada"}",
            $"Opção escolhida: {RotulosDePausa.Descrever(_pausa.Opcao)}",
            $"Próximo passo: {_pausa.ProximoPasso}",
            $"Limite ou apoio: {_pausa.LimiteOuApoio}",
            $"Tarefa a retomar: {_retomada.Tarefa}",
            $"Ajuste de contexto: {_retomada.AjusteDeContexto}",
            $"Apoio combinado: {_retomada.Apoio}",
            $"Rever em: {_retomada.RevisarEm:O}",
            $"Observação para a conversa: {_pedido.Observacao}",
            $"Impacto relatado: {_pedido.Impacto}",
            $"Pedido: {_pedido.Pedido}",
            $"Mensagem estruturada: {_pedido.Formatar()}",
            $"Acordo de equipe: {_acordo.Rotina}",
            $"Opção de pausa do grupo: {_acordo.OpcaoDePausa}",
            $"Apoio disponível: {_acordo.ApoioDisponivel}",
            $"Revisão do acordo: {_acordo.Revisao}"
        };

        if (_revisao is not null)
        {
            linhas.Add($"Reflexão opcional: {_revisao.OQueNotei}");
            linhas.Add($"Escolha observada: {_revisao.EscolhaFeita}");
            linhas.Add($"Próxima tentativa: {_revisao.ProximaTentativa}");
            if (_revisao.ApoioOuLimite is not null)
                linhas.Add($"Apoio ou limite informado: {_revisao.ApoioOuLimite}");
        }

        linhas.AddRange(PerguntasDeEscuta.Exemplos.Select(pergunta => $"Pergunta possível: {pergunta}"));
        linhas.Add("O roteiro apresenta somente as informações fornecidas.");
        return linhas.AsReadOnly();
    }
}
