---
id: lab-solid-l
order: 267
kind: lab
num: 34
group: Laboratório · Parte 43 · SOLID com analogias bíblicas
title: Princípio da Substituição de Liskov (Liskov Substitution Principle - LSP)
subtitle: As classes derivadas devem ser substituíveis por suas classes base sem quebrar a aplicação.
minutes: 5
hue: 100
origem: Anfsusax-solid-principles/.../PrincipiosSolid.cs
---

> As classes derivadas devem ser substituíveis por suas classes base sem quebrar a aplicação.

## A analogia bíblica

**Mateus 5:17-18**

> “Não pensem que vim abolir a Lei ou os Profetas; não vim abolir, mas cumprir. Digo a verdade: Enquanto existirem céus e terra, de forma alguma desaparecerá da Lei a menor letra ou o menor traço, até que tudo se cumpra.”

Assim como Jesus cumpriu perfeitamente a Lei sem quebrar nenhum de seus princípios, as classes derivadas devem cumprir rigorosamente o contrato estabelecido por suas classes base. Se uma subclasse não pode ser usada no lugar de sua classe base sem causar comportamentos inesperados, o princípio de Liskov está sendo violado.

## O exemplo completo

```csharp L
namespace SolidPrinciples.LSP;

using System;
using System.Collections.Generic;

public interface IBird
{
    void Eat();
}

public interface IFlyingBird : IBird
{
    void Fly();
}

public class Sparrow : IFlyingBird
{
    public void Eat() => Console.WriteLine("O pardal está bicando sementes.");

    public void Fly() => Console.WriteLine("O pardal está voando.");
}

public class Penguin : IBird
{
    public void Eat() => Console.WriteLine("O pinguim está pescando no gelo.");
}

public class BirdShow
{
    public void StartFlightShow(IEnumerable<IFlyingBird> birds)
    {
        foreach (var bird in birds)
        {
            bird.Fly();
        }
    }
}

public static class Program
{
    public static void Main()
    {
        var birdsThatFly = new List<IFlyingBird>
        {
            new Sparrow()
            // Um pinguim não pode ser adicionado aqui, porque não implementa IFlyingBird.
        };

        var show = new BirdShow();
        show.StartFlightShow(birdsThatFly);

        IBird penguin = new Penguin(); // Ainda conseguimos tratar o pinguim como uma ave.
        penguin.Eat();
    }
}
```

## Peça por peça

### Contrato das aves

Define comportamentos separados: nem toda ave precisa voar.

```csharp contract
public interface IBird
{
    void Eat();
}

public interface IFlyingBird : IBird
{
    void Fly();
}
```

### Ave que voa corretamente

Cumpre o contrato de `IFlyingBird` sem surpresas.

```csharp implementation
public class Sparrow : IFlyingBird
{
    public void Eat() => Console.WriteLine("O pardal está bicando sementes.");

    public void Fly() => Console.WriteLine("O pardal está voando.");
}
```

### Ave que não voa

Não implementa `Fly`, evitando exceções em tempo de execução.

```csharp implementation
public class Penguin : IBird
{
    public void Eat() => Console.WriteLine("O pinguim está pescando no gelo.");
}
```

### Uso seguro do contrato

Aceita apenas aves que sabem voar, mantendo a coerência do contrato.

```csharp service
public class BirdShow
{
    public void StartFlightShow(IEnumerable<IFlyingBird> birds)
    {
        foreach (var bird in birds)
        {
            bird.Fly();
        }
    }
}
```

### Demonstração

Mostra como cada classe pode ser usada sem quebrar expectativas.

```csharp bootstrap
public static class Program
{
    public static void Main()
    {
        var birdsThatFly = new List<IFlyingBird>
        {
            new Sparrow()
        };

        var show = new BirdShow();
        show.StartFlightShow(birdsThatFly);

        IBird penguin = new Penguin();
        penguin.Eat();
    }
}
```
