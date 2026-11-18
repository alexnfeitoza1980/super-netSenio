using Jornada.Resiliencia.Cap89;
using Jornada.Resiliencia.Cap90;
using Jornada.Resiliencia.Cap92;
using Jornada.Resiliencia.Cap93;
using Jornada.Resiliencia.Cap95;

namespace Jornada.Resiliencia.Lab10;

/// <summary>A conversa no console. Recebe a entrada e a saída de fora, então também dá para testar sem teclado.</summary>
public sealed class Roteiro
{
    private readonly TextReader entrada;
    private readonly TextWriter saida;
    private readonly DateOnly hoje;

    public Roteiro(TextReader entrada, TextWriter saida, DateOnly hoje)
    {
        this.entrada = entrada;
        this.saida = saida;
        this.hoje = hoje;
    }

    /// <summary>Faz uma pergunta. Resposta em branco devolve null: pular é sempre permitido.</summary>
    private string? Perguntar(string pergunta)
    {
        saida.Write(pergunta + " ");
        var texto = entrada.ReadLine();
        return string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
    }

    public CadernoDeProximoPasso? Executar()
    {
        saida.WriteLine("Caderno de um próximo passo. Nada é gravado: tudo fica só na memória.");
        saida.WriteLine("Deixe em branco o que preferir pular. Use situações leves e comuns.");
        saida.WriteLine();

        var resposta = CheckIn
            .Ler(Perguntar("Como você está? 1) bem  2) mais ou menos  3) não estou bem  4) prefiro não dizer"))
            .Combinar(r => r, _ => RespostaDoCheckIn.PreferoNaoDizer);
        saida.WriteLine(CheckIn.Acolher(resposta));

        if (resposta == RespostaDoCheckIn.NaoEstouBem)
        {
            foreach (var servico in ServicosPublicos.Brasil)
                saida.WriteLine("  - " + servico.ComoPedir(Necessidade.Urgencia));

            if (Perguntar("Quer continuar o caderno agora? (s/n)")?.ToLowerInvariant() != "s")
            {
                saida.WriteLine("Tudo bem. Ele estará aqui quando você quiser.");
                return null;
            }
        }

        var caderno = new CadernoDeProximoPasso
        {
            Situacao = Perguntar("Que situação comum você quer organizar?") ?? "uma tarefa que quero organizar"
        };

        LerSentimento(caderno);
        LerAnotacoes(caderno);
        LerPreocupacao(caderno);
        LerApoio(caderno);
        LerMeta(caderno);

        saida.WriteLine();
        saida.WriteLine("--- Seu resumo ---");
        foreach (var linha in caderno.Resumo())
            saida.WriteLine(linha);

        return caderno;
    }

    private void LerSentimento(CadernoDeProximoPasso c)
    {
        var emocao = Perguntar("Emoção (calma, alegria, tristeza, raiva, medo, cansaco):");
        if (emocao is null || !Enum.TryParse<Emocao>(emocao, ignoreCase: true, out var e))
            return;

        int intensidade = int.TryParse(Perguntar("Intensidade de 0 a 10:"), out var n) ? Math.Clamp(n, 0, 10) : 5;
        c.Sentimento = new Sentimento(e, intensidade);
    }

    private void LerAnotacoes(CadernoDeProximoPasso c)
    {
        if (Perguntar("Um FATO (o que dá para observar):") is { } fato)
            c.Anotacoes.Add(new Anotacao(fato, NaturezaDaFrase.Fato));

        if (Perguntar("Uma HISTÓRIA (a explicação que você imaginou):") is { } historia)
            c.Anotacoes.Add(new Anotacao(historia, NaturezaDaFrase.Historia));
    }

    private void LerPreocupacao(CadernoDeProximoPasso c)
    {
        if (Perguntar("Uma preocupação:") is not { } texto)
            return;

        var controle = Perguntar("Depende de você? 1) sim  2) posso influenciar  3) não depende") switch
        {
            "1" => Controle.DependeDeMim,
            "3" => Controle.NaoDepende,
            _   => Controle.PossoInfluenciar
        };

        c.Preocupacoes.Add(new Preocupacao(texto, controle));
    }

    private void LerApoio(CadernoDeProximoPasso c)
    {
        if (Perguntar("Alguém que poderia ajudar (nome):") is not { } nome)
            return;

        c.Apoio.Adicionar(new Pessoa(nome, Necessidade.Ouvir, Necessidade.Estudar));
        c.Mochila.Guardar(nome, TipoRecurso.Pessoas);
    }

    private void LerMeta(CadernoDeProximoPasso c)
    {
        if (Perguntar("Uma tarefa para dividir em passos:") is not { } tarefa)
            return;

        int minutos = int.TryParse(Perguntar("Quantos minutos ela tomaria no total?"), out var n) && n > 0 ? n : 25;
        c.Meta = new Meta(tarefa).AdicionarDividindo(tarefa, minutos);
    }
}
