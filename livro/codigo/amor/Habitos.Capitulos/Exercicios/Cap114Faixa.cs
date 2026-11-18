namespace Jornada.Habitos.Cap114;

/// <summary>Uma faixa de números (ex.: 25 a 50 minutos). Não existe faixa ao contrário.</summary>
public readonly record struct Faixa
{
    public int Minimo { get; }
    public int Maximo { get; }

    public Faixa(int minimo, int maximo)
    {
        if (minimo > maximo)
            throw new ArgumentException($"O mínimo ({minimo}) não pode passar do máximo ({maximo}).");

        Minimo = minimo;
        Maximo = maximo;
    }

    public bool Contem(int valor) => valor >= Minimo && valor <= Maximo;

    public bool Sobrepoe(Faixa outra) => Minimo <= outra.Maximo && outra.Minimo <= Maximo;

    public override string ToString() => $"{Minimo}–{Maximo}";
}
