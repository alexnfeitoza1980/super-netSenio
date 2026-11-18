namespace Jornada.Joao.Texto;

// Jo 3,8 (Bíblia Livre): "O vento sopra onde quer, e ouves o seu som; porém não sabes de onde vem, nem para onde vai;
//  assim é todo aquele que é nascido do Espírito."
//
// TEXTO     : uma imagem do vento (em grego, a mesma palavra, pneuma, é "vento" e "espírito") para falar de quem nasce do Espírito.
// ANALOGIA  : o vento é quem emite sem escolher quem ouve; o som chega a quem estiver por perto.
// LIMITE    : o código só dá forma à imagem (emitir e ouvir sem se conhecerem). Não trata de nascer de novo nem do Espírito.
// Esta versão simples não é segura para uso com várias threads (os inscritos ficam em uma lista comum).

// <vento>
public class VentoSimples
{
    // quem quiser ouvir, se inscreve neste evento
    public event Action<string>? Soprou;

    public int Ouvintes => Soprou?.GetInvocationList().Length ?? 0;

    // "O vento sopra onde quer": ele só sopra; não escolhe quem ouve
    public void Soprar(string som) => Soprou?.Invoke(som);
}

public class OuvinteDoVento
{
    public List<string> Ouvido { get; } = new();

    // "e ouves o seu som": só recebe o som, sem saber quem soprou
    public void Ouvir(string som) => Ouvido.Add(som);
}
// </vento>
