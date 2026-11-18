namespace Jornada.Joao.Texto;

// Jo 11,43-44 (Bíblia Livre): "[...] clamou com grande voz: Lázaro, sai fora. E o que estava morto saiu, com as mãos e os pés atados,
//  e seu rosto envolto em um lenço. Disse-lhes Jesus: Desatai-o, e deixai-o ir."
//
// TEXTO     : Lázaro sai; ainda está atado; manda-se desatá-lo e deixá-lo ir.
// ANALOGIA  : a SEQUÊNCIA de estados (morto, vivo e atado, livre) e as ações que os mudam (chamar, desatar).
// LIMITE    : o código não modela a ressurreição, a fé nem o poder que o texto narra. Ele só dá forma à ordem dos acontecimentos.

public enum Condicao { Morto, Vivo }

// <lazaro>
public class Lazaro
{
    public Condicao Condicao { get; private set; } = Condicao.Morto;

    // "com as mãos e os pés atados, e seu rosto envolto em um lenço"
    public List<string> Ataduras { get; } = new() { "atadura nas mãos", "atadura nos pés", "lenço no rosto" };

    public bool Livre => Condicao == Condicao.Vivo && Ataduras.Count == 0;

    // "Lázaro, sai fora."
    public void Chamar() => Condicao = Condicao.Vivo;

    // "Desatai-o, e deixai-o ir."
    public bool Desatar(string atadura) => Ataduras.Remove(atadura);
}
// </lazaro>
