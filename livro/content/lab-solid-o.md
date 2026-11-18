---
id: lab-solid-o
order: 266
kind: lab
num: 33
group: Laboratório · Parte 43 · SOLID com analogias bíblicas
title: Princípio Aberto/Fechado (Open/Closed Principle - OCP)
subtitle: As entidades do software devem estar abertas para extensão, mas fechadas para modificação.
minutes: 5
hue: 70
origem: Anfsusax-solid-principles/.../PrincipiosSolid.cs
---

> As entidades do software devem estar abertas para extensão, mas fechadas para modificação.

## A analogia bíblica

**Gênesis 6:14-16**

> “Faça para você uma arca de tábuas de cipreste: você fará compartimentos na arca e a revestirá de piche por dentro e por fora. Esta é a maneira como você a fará: a arca terá cento e trinta e cinco metros de comprimento, vinte e dois metros e meio de largura e treze metros e meio de altura. Você fará um teto para a arca, deixando uma abertura de quarenta e cinco centímetros entre o teto e as laterais. Coloque uma porta na lateral da arca. Faça um andar inferior, um segundo e um terceiro.”

Assim como a arca de Noé foi projetada com uma estrutura básica que podia acomodar diferentes tipos de animais sem precisar ser modificada, nosso código deve ser estruturado para permitir a adição de novos comportamentos através de extensões, sem modificar o código existente.

## O exemplo completo

```csharp O
namespace SolidPrinciples.OCP;

using System;
using System.Collections.Generic;

public record Order(decimal Total, decimal Weight, string DestinationZip);

public abstract class ShippingStrategy
{
    public abstract decimal Calculate(Order order);
}

public class StandardShipping : ShippingStrategy
{
    public override decimal Calculate(Order order)
        => Math.Max(15m, order.Weight * 2m);
}

public class ExpressShipping : ShippingStrategy
{
    public override decimal Calculate(Order order)
        => Math.Max(40m, order.Weight * 3.5m);
}

public class LocalPickupShipping : ShippingStrategy
{
    public override decimal Calculate(Order order) => 0m;
}

public class ShippingService
{
    private readonly Dictionary<string, ShippingStrategy> _strategies = new();

    public ShippingService()
    {
        RegisterStrategy("standard", new StandardShipping());
        RegisterStrategy("express", new ExpressShipping());
    }

    public void RegisterStrategy(string key, ShippingStrategy strategy)
        => _strategies[key] = strategy;

    public decimal CalculateCost(string option, Order order)
    {
        if (!_strategies.TryGetValue(option, out var strategy))
            throw new InvalidOperationException($"Opção '{option}' não suportada.");

        return strategy.Calculate(order);
    }
}

public static class Program
{
    public static void Main()
    {
        var service = new ShippingService();
        service.RegisterStrategy("pickup", new LocalPickupShipping());

        var order = new Order(250m, 10m, "01000-000");
        Console.WriteLine($"Entrega expressa: R$ {service.CalculateCost(\"express\", order)}");
        Console.WriteLine($"Retirada local: R$ {service.CalculateCost(\"pickup\", order)}");
    }
}
```

## Peça por peça

### Modelo do pedido

Agrupa os dados necessários para calcular frete.

```csharp model
public record Order(decimal Total, decimal Weight, string DestinationZip);
```

### Contrato de cálculo

Define o ponto de extensão sem conhecer implementações futuras.

```csharp contract
public abstract class ShippingStrategy
{
    public abstract decimal Calculate(Order order);
}
```

### Estratégias concretas

Novas opções de frete são adicionadas sem tocar no código antigo.

```csharp extension
public class StandardShipping : ShippingStrategy
{
    public override decimal Calculate(Order order)
        => Math.Max(15m, order.Weight * 2m);
}

public class ExpressShipping : ShippingStrategy
{
    public override decimal Calculate(Order order)
        => Math.Max(40m, order.Weight * 3.5m);
}

public class LocalPickupShipping : ShippingStrategy
{
    public override decimal Calculate(Order order) => 0m;
}
```

### Serviço orquestrador

Consome as extensões via dicionário, permanecendo fechado para mudanças.

```csharp service
public class ShippingService
{
    private readonly Dictionary<string, ShippingStrategy> _strategies = new();

    public ShippingService()
    {
        RegisterStrategy("standard", new StandardShipping());
        RegisterStrategy("express", new ExpressShipping());
    }

    public void RegisterStrategy(string key, ShippingStrategy strategy)
        => _strategies[key] = strategy;

    public decimal CalculateCost(string option, Order order)
    {
        if (!_strategies.TryGetValue(option, out var strategy))
            throw new InvalidOperationException($"Opção '{option}' não suportada.");

        return strategy.Calculate(order);
    }
}
```

### Demonstração

Faz uso das estratégias sem alterações no serviço principal.

```csharp bootstrap
public static class Program
{
    public static void Main()
    {
        var service = new ShippingService();
        service.RegisterStrategy("pickup", new LocalPickupShipping());

        var order = new Order(250m, 10m, "01000-000");
        Console.WriteLine($"Entrega expressa: R$ {service.CalculateCost("express", order)}");
        Console.WriteLine($"Retirada local: R$ {service.CalculateCost("pickup", order)}");
    }
}
```
