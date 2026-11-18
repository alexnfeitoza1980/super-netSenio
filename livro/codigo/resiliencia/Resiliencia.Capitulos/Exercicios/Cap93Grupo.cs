namespace Jornada.Resiliencia.Cap93;

/// <summary>Um tipo novo de apoio. O MapaDeApoio não precisa mudar nada para aceitá-lo.</summary>
public sealed class GrupoDeEstudo : ApoioBase
{
    public GrupoDeEstudo(string nome) : base(nome, Necessidade.Estudar, Necessidade.Ouvir) { }

    public override string ComoPedir(Necessidade n) =>
        $"Posso participar do encontro do grupo {Nome} desta semana? Levo as minhas dúvidas.";
}
