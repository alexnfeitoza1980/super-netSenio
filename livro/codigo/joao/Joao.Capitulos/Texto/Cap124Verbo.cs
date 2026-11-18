namespace Jornada.Joao.Texto;

// Jo 1,1-5 (Bíblia Livre): "No princípio era a Palavra, e a Palavra estava junto de Deus, e a Palavra era Deus. [...]
// Nela estava a vida, e a vida era a luz dos seres humanos. E a luz brilha nas trevas; e as trevas não a compreenderam."
// A própria edição anota, em 1,1: "Palavra, equiv. Verbo". Os tipos abaixo se chamam Verbo, o nome tradicional.
//
// TEXTO     : duas expressões diferentes na mesma frase: "estava junto de" e "era".
// ANALOGIA  : em C#, "estar junto de" é uma REFERÊNCIA (um objeto ligado a outro) e "ser" é HERANÇA (um tipo é do tipo de outro).
// LIMITE    : o sistema de tipos de C# não define o que o texto afirma; o código só dá forma às duas relações.

/// <summary>
/// O tipo que representa "Deus" nesta analogia. Ser uma classe que se pode instanciar é um detalhe de C#:
/// o código NÃO afirma que se possa criar Deus, nem que existam vários.
/// </summary>
public class Deus
{
    public string Nome => "Deus";
}

public class Luz
{
    // "E a luz brilha nas trevas"
    public void Resplandecer(Trevas trevas) => trevas.Receber(this);
}

public class Vida : Luz
{
}

// <verbo>
// "e a Palavra era Deus": o ":" diz que Verbo é do tipo Deus. É uma relação entre TIPOS,
// que vale pela declaração, antes de existir qualquer objeto (nenhum "new" é necessário).
public class Verbo : Deus
{
    public Deus? EstavaCom { get; set; }       // "e a Palavra estava junto de Deus": referência a outro objeto
    public Vida? Vida { get; set; }            // "Nela estava a vida"
}
// </verbo>

public class Trevas
{
    public bool Iluminadas { get; private set; }

    public void Receber(Luz luz) => Iluminadas = true;

    // "e as trevas não a compreenderam": o resultado é DECLARADO pelo texto, não calculado por um mecanismo.
    // O teste confirma que o código registra a declaração; não explica o motivo.
    public bool Compreendeu(Luz luz) => false;
}
