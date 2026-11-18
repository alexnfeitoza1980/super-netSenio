---
id: lab-solid-i
order: 268
kind: lab
num: 35
group: Laboratório · Parte 43 · SOLID com analogias bíblicas
title: Princípio da Segregação de Interface (Interface Segregation Principle - ISP)
subtitle: Muitas interfaces específicas são melhores do que uma interface geral.
minutes: 5
hue: 130
origem: Anfsusax-solid-principles/.../PrincipiosSolid.cs
---

> Muitas interfaces específicas são melhores do que uma interface geral.

## A analogia bíblica

**1 Coríntios 12:4-11**

> “Há diferentes tipos de dons, mas o Espírito é o mesmo. Há diferentes tipos de ministérios, mas o Senhor é o mesmo. Há diferentes formas de atuação, mas é o mesmo Deus quem efetua tudo em todos. A cada um, porém, é dada a manifestação do Espírito, visando ao bem comum. Pelo Espírito, a um é dada a palavra de sabedoria; a outro, a palavra de conhecimento; a outro, fé; a outro, dons de curar; a outro, poder para operar milagres; a outro, profecia; a outro, discernimento de espíritos; a outro, variedade de línguas; e ainda a outro, interpretação de línguas. Todas essas coisas, porém, são realizadas pelo mesmo e único Espírito, e ele as distribui individualmente, a cada um, como quer.”

Assim como o Espírito Santo distribui diferentes dons espirituais de acordo com a necessidade e capacidade de cada um, nossas interfaces devem ser específicas e focadas, permitindo que as classes implementem apenas o que realmente precisam. Isso evita a criação de classes com métodos que não fazem sentido para elas, mantendo o código mais limpo e coeso.

## O exemplo completo

```csharp I
namespace SolidPrinciples.ISP;

using System;

public class Document
{
    public string Content { get; init; } = string.Empty;
}

public interface IPrinter
{
    void Print(Document document);
}

public interface IScanner
{
    void Scan(Document document);
}

public interface IFax
{
    void SendFax(Document document);
}

public class MultiFunctionPrinter : IPrinter, IScanner, IFax
{
    public void Print(Document document) => Console.WriteLine("Imprimindo documento...");
    public void Scan(Document document) => Console.WriteLine("Digitalizando documento...");
    public void SendFax(Document document) => Console.WriteLine("Enviando fax...");
}

public class SimplePrinter : IPrinter
{
    public void Print(Document document) => Console.WriteLine("Impressora simples: impressão concluída.");
}

public static class Program
{
    public static void Main()
    {
        Document doc = new() { Content = "Contrato confidencial" };

        IPrinter printer = new SimplePrinter();
        printer.Print(doc); // Não obriga a implementar métodos desnecessários.

        MultiFunctionPrinter officeDevice = new();
        officeDevice.Scan(doc);
    }
}
```

## Peça por peça

### Documento simples

Objeto que trafega entre as interfaces.

```csharp model
public class Document
{
    public string Content { get; init; } = string.Empty;
}
```

### Interfaces pequenas

Divide capacidades para que cada classe implemente só o necessário.

```csharp contract
public interface IPrinter
{
    void Print(Document document);
}

public interface IScanner
{
    void Scan(Document document);
}

public interface IFax
{
    void SendFax(Document document);
}
```

### Equipamento multifuncional

Implementa várias interfaces porque tem suporte a todas as funções.

```csharp implementation
public class MultiFunctionPrinter : IPrinter, IScanner, IFax
{
    public void Print(Document document) => Console.WriteLine("Imprimindo documento...");
    public void Scan(Document document) => Console.WriteLine("Digitalizando documento...");
    public void SendFax(Document document) => Console.WriteLine("Enviando fax...");
}
```

### Equipamento simples

Imprime sem precisar declarar métodos inúteis.

```csharp implementation
public class SimplePrinter : IPrinter
{
    public void Print(Document document) => Console.WriteLine("Impressora simples: impressão concluída.");
}
```

### Demonstração

Mostra como cada classe usa apenas as interfaces relevantes.

```csharp bootstrap
public static class Program
{
    public static void Main()
    {
        Document doc = new() { Content = "Contrato confidencial" };

        IPrinter printer = new SimplePrinter();
        printer.Print(doc);

        MultiFunctionPrinter officeDevice = new();
        officeDevice.Scan(doc);
    }
}
```
