---
id: lab-solid-d
order: 269
kind: lab
num: 36
group: Laboratório · Parte 43 · SOLID com analogias bíblicas
title: Princípio da Inversão de Dependência (Dependency Inversion Principle - DIP)
subtitle: Dependa de abstrações, não de implementações concretas.
minutes: 5
hue: 160
origem: Anfsusax-solid-principles/.../PrincipiosSolid.cs
---

> Dependa de abstrações, não de implementações concretas.

## A analogia bíblica

**Provérbios 3:5-6**

> “Confie no Senhor de todo o seu coração e não se apoie em seu próprio entendimento; reconheça o Senhor em todos os seus caminhos, e ele endireitará as suas veredas.”

Assim como devemos depender de Deus e não apenas de nossa própria compreensão, no desenvolvimento de software devemos depender de abstrações (interfaces) em vez de implementações concretas. Isso torna nosso código mais flexível, testável e fácil de manter, permitindo que troquemos implementações sem afetar o código que as utiliza.

## O exemplo completo

```csharp D
namespace SolidPrinciples.DIP;

using System;
using System.Threading.Tasks;

public record Pedido(int Id, string Cliente);

public interface ILogger
{
    void Log(string message);
}

public class ConsoleLogger : ILogger
{
    public void Log(string message) => Console.WriteLine($"[LOG] {message}");
}

public interface IPedidoRepository
{
    Task SaveAsync(Pedido pedido);
}

public class InMemoryPedidoRepository : IPedidoRepository
{
    public Task SaveAsync(Pedido pedido)
    {
        Console.WriteLine($"[Repository] Pedido {pedido.Id} salvo em memória.");
        return Task.CompletedTask;
    }
}

public class PedidoService
{
    private readonly ILogger _logger;
    private readonly IPedidoRepository _repository;

    public PedidoService(ILogger logger, IPedidoRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task ProcessarAsync(Pedido pedido)
    {
        _logger.Log($"Processando pedido {pedido.Id} para {pedido.Cliente}.");
        await _repository.SaveAsync(pedido);
        _logger.Log($"Pedido {pedido.Id} processado.");
    }
}

public static class Program
{
    public static async Task Main()
    {
        var service = new PedidoService(
            new ConsoleLogger(),
            new InMemoryPedidoRepository());

        await service.ProcessarAsync(new Pedido(42, "Ana"));
    }
}
```

## Peça por peça

### Modelo de pedido

Define os dados da compra processada.

```csharp model
public record Pedido(int Id, string Cliente);
```

### Contrato de logging

Camada alta fala apenas com a abstração, escolhendo qualquer implementação.

```csharp contract
public interface ILogger
{
    void Log(string message);
}

public class ConsoleLogger : ILogger
{
    public void Log(string message) => Console.WriteLine($"[LOG] {message}");
}
```

### Contrato de persistência

Permite substituir o armazenamento sem alterar a regra de negócio.

```csharp contract
public interface IPedidoRepository
{
    Task SaveAsync(Pedido pedido);
}

public class InMemoryPedidoRepository : IPedidoRepository
{
    public Task SaveAsync(Pedido pedido)
    {
        Console.WriteLine($"[Repository] Pedido {pedido.Id} salvo em memória.");
        return Task.CompletedTask;
    }
}
```

### Regra de negócio

Depende só de interfaces, cumprindo a inversão de dependência.

```csharp service
public class PedidoService
{
    private readonly ILogger _logger;
    private readonly IPedidoRepository _repository;

    public PedidoService(ILogger logger, IPedidoRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task ProcessarAsync(Pedido pedido)
    {
        _logger.Log($"Processando pedido {pedido.Id} para {pedido.Cliente}.");
        await _repository.SaveAsync(pedido);
        _logger.Log($"Pedido {pedido.Id} processado.");
    }
}
```

### Configuração de execução

Injeta as dependências concretas na borda da aplicação.

```csharp bootstrap
public static class Program
{
    public static async Task Main()
    {
        var service = new PedidoService(
            new ConsoleLogger(),
            new InMemoryPedidoRepository());

        await service.ProcessarAsync(new Pedido(42, "Ana"));
    }
}
```
