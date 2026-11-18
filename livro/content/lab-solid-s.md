---
id: lab-solid-s
order: 265
kind: lab
num: 32
group: Laboratório · Parte 43 · SOLID com analogias bíblicas
title: Princípio da Responsabilidade Única (Single Responsibility Principle - SRP)
subtitle: Uma classe deve ter apenas um motivo para mudar, ou seja, deve ter apenas uma responsabilidade.
minutes: 5
hue: 40
origem: Anfsusax-solid-principles/.../PrincipiosSolid.cs
---

> Uma classe deve ter apenas um motivo para mudar, ou seja, deve ter apenas uma responsabilidade.

## A analogia bíblica

**1 Coríntios 12:12-27**

> “Porque, assim como o corpo é um e tem muitos membros, e todos os membros do corpo, embora muitos, formam um só corpo, assim também é Cristo. Pois em um só corpo todos nós fomos batizados em um único Espírito: quer judeus, quer gregos, quer escravos, quer livres. E a todos nós foi dado beber de um único Espírito. O corpo não é feito de um só membro, mas de muitos.”

Assim como cada parte do corpo tem uma função específica (olhos para ver, ouvidos para ouvir), no código cada classe deve ter uma única responsabilidade bem definida. Isso torna o sistema mais organizado, fácil de manter e testar.

## O exemplo completo

```csharp S
namespace SolidPrinciples.SRP;

using System;
using System.Threading.Tasks;

public record User(string Name, string Email);

public interface IUserValidator
{
    bool IsValid(User user);
}

public class UserValidator : IUserValidator
{
    public bool IsValid(User user)
    {
        if (string.IsNullOrWhiteSpace(user.Name))
            return false;

        if (string.IsNullOrWhiteSpace(user.Email) || !user.Email.Contains("@"))
            return false;

        return true;
    }
}

public interface IUserRepository
{
    Task SaveAsync(User user);
}

public class SqlUserRepository : IUserRepository
{
    public Task SaveAsync(User user)
    {
        Console.WriteLine($"[Repository] Salvando {user.Name} no banco de dados.");
        return Task.CompletedTask;
    }
}

public interface IEmailSender
{
    Task SendWelcomeAsync(User user);
}

public class SmtpEmailSender : IEmailSender
{
    public Task SendWelcomeAsync(User user)
    {
        Console.WriteLine($"[Email] Enviando boas-vindas para {user.Email}.");
        return Task.CompletedTask;
    }
}

public class UserRegistrationService
{
    private readonly IUserValidator _validator;
    private readonly IUserRepository _repository;
    private readonly IEmailSender _emailSender;

    public UserRegistrationService(
        IUserValidator validator,
        IUserRepository repository,
        IEmailSender emailSender)
    {
        _validator = validator;
        _repository = repository;
        _emailSender = emailSender;
    }

    public async Task RegisterAsync(User user)
    {
        if (!_validator.IsValid(user))
            throw new InvalidOperationException("Usuário inválido.");

        await _repository.SaveAsync(user);
        await _emailSender.SendWelcomeAsync(user);
    }
}

public static class Program
{
    public static async Task Main()
    {
        var service = new UserRegistrationService(
            new UserValidator(),
            new SqlUserRepository(),
            new SmtpEmailSender());

        await service.RegisterAsync(new User("Débora", "debora@example.com"));
    }
}
```

## Peça por peça

### Modelo de dados

Representa os dados do usuário de forma imutável.

```csharp model
public record User(string Name, string Email);
```

### Validação de usuário

Toda a lógica de validação fica isolada aqui.

```csharp validator
public interface IUserValidator
{
    bool IsValid(User user);
}

public class UserValidator : IUserValidator
{
    public bool IsValid(User user)
    {
        if (string.IsNullOrWhiteSpace(user.Name))
            return false;

        if (string.IsNullOrWhiteSpace(user.Email) || !user.Email.Contains("@"))
            return false;

        return true;
    }
}
```

### Persistência

Responsável apenas por salvar dados.

```csharp repository
public interface IUserRepository
{
    Task SaveAsync(User user);
}

public class SqlUserRepository : IUserRepository
{
    public Task SaveAsync(User user)
    {
        Console.WriteLine($"[Repository] Salvando {user.Name} no banco de dados.");
        return Task.CompletedTask;
    }
}
```

### Infraestrutura de e-mail

Cuida somente do envio de notificações.

```csharp infrastructure
public interface IEmailSender
{
    Task SendWelcomeAsync(User user);
}

public class SmtpEmailSender : IEmailSender
{
    public Task SendWelcomeAsync(User user)
    {
        Console.WriteLine($"[Email] Enviando boas-vindas para {user.Email}.");
        return Task.CompletedTask;
    }
}
```

### Regra de negócio

Orquestra as dependências, mantendo cada responsabilidade separada.

```csharp service
public class UserRegistrationService
{
    private readonly IUserValidator _validator;
    private readonly IUserRepository _repository;
    private readonly IEmailSender _emailSender;

    public UserRegistrationService(
        IUserValidator validator,
        IUserRepository repository,
        IEmailSender emailSender)
    {
        _validator = validator;
        _repository = repository;
        _emailSender = emailSender;
    }

    public async Task RegisterAsync(User user)
    {
        if (!_validator.IsValid(user))
            throw new InvalidOperationException("Usuário inválido.");

        await _repository.SaveAsync(user);
        await _emailSender.SendWelcomeAsync(user);
    }
}
```

### Ponto de entrada

Configura as dependências e executa o fluxo.

```csharp bootstrap
public static class Program
{
    public static async Task Main()
    {
        var service = new UserRegistrationService(
            new UserValidator(),
            new SqlUserRepository(),
            new SmtpEmailSender());

        await service.RegisterAsync(new User("Débora", "debora@example.com"));
    }
}
```
