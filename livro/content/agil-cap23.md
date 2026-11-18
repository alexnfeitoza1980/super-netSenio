---
id: agil-cap23
order: 39
kind: capitulo
num: 23
group: C# · Parte 8 · Projeto ágil e princípios SOLID
title: Princípio da Inversão de Dependência (DIP)
subtitle: Políticas de alto nível não devem depender de detalhes de baixo nível — ambos dependem de abstrações. Camadas invertidas, posse de interface e injeção de dependência.
minutes: 50
hue: 15
objectives:
- Enunciar as **duas partes** do DIP e explicar por que a dependência é "invertida"
- Reconhecer a **dependência transitiva** das camadas ingênuas e a solução com **interfaces pertencentes ao cliente**
- Aplicar a heurística **"dependa de abstrações"** (e saber suas exceções)
- Usar **injeção de dependência** em C# e conectar o DIP a Strategy e frameworks
origem: livros/02-projeto-agil-csharp/content/38-cap23.md
---

::: ilustracao img/agil/cap23.svg | DIP: dependa da tomada (abstração), e não da fiação da parede (detalhe).
:::

## O princípio

::: regra DIP — Dependency Inversion Principle
**A.** Módulos de alto nível **não** devem depender de módulos de baixo nível. **Ambos** devem depender de **abstrações**.
**B.** Abstrações **não** devem depender de detalhes. **Detalhes** devem depender de abstrações.
:::

Por que "inversão"? Porque os métodos tradicionais (análise e projeto estruturados) produzem justamente o oposto: uma **hierarquia de sub-rotinas** em que os módulos de alto nível *chamam* os de baixo nível — e portanto **dependem** deles. O primeiro programa de cópia do capítulo 19 é um exemplo: o módulo `Copiar` (política) dependia diretamente de `Teclado` e `Impressora` (detalhes).

Pense no que isso significa. Os módulos de **alto nível** guardam as **decisões de política** e os **modelos de negócio** — a **identidade** do aplicativo. Se eles dependem dos módulos de baixo nível, qualquer mudança nos detalhes (outro banco, outro dispositivo, outra API) **força mudança na política**. Absurdo: quem deveria mandar é a política, e os detalhes é que deveriam se adaptar a ela.

Há ainda um motivo prático: são as políticas de alto nível que **queremos reutilizar**. Bibliotecas de baixo nível já reaproveitamos bem; mas uma política presa a detalhes específicos não sai do contexto onde nasceu. O DIP está no **coração do projeto de frameworks**.

::: figura Inversão da direção da dependência
<svg viewBox="0 0 740 260" role="img" aria-label="Antes: política depende do detalhe; depois: ambos dependem da abstração, de posse da política">
  <text x="165" y="20" text-anchor="middle" font-weight="800" class="t-bad">Procedural: a política depende do detalhe</text>
  <rect class="f-prim" x="75" y="45" width="180" height="55" rx="12"/><text x="165" y="78" text-anchor="middle" font-weight="800">Política (alto nível)</text>
  <path class="arrow" d="M165 102V150" style="stroke:var(--bad);marker-end:url(#arr3)"/>
  <rect class="f-bad" x="75" y="152" width="180" height="55" rx="12"/><text x="165" y="185" text-anchor="middle" font-weight="800">Detalhe (baixo nível)</text>
  <text x="560" y="20" text-anchor="middle" font-weight="800" class="t-good">Orientado a objetos: ambos dependem da abstração</text>
  <rect class="f-prim" x="465" y="45" width="190" height="55" rx="12"/><text x="560" y="78" text-anchor="middle" font-weight="800">Política (alto nível)</text>
  <path class="arrow" d="M560 102V128"/>
  <rect class="f-info" x="455" y="130" width="210" height="45" rx="12"/><text x="560" y="158" text-anchor="middle" font-weight="700" font-size="13">«interface» (pertence à política)</text>
  <path class="arrow" d="M560 215V177" style="stroke:var(--good);marker-end:url(#arr)"/>
  <rect class="f-good" x="465" y="215" width="190" height="40" rx="12"/><text x="560" y="241" text-anchor="middle" font-weight="800">Detalhe (implementa)</text>
</svg>
:::

## Camadas: o esquema simplista e o invertido

Grady Booch diz que as arquiteturas orientadas a objetos bem estruturadas têm **camadas** claramente definidas, cada uma oferecendo serviços por meio de uma **interface** controlada. Uma leitura ingênua desenha:

> **Política** → usa **Mecanismo** → usa **Utilitário**

Parece correto, mas a camada **Política** fica **sensível às mudanças** em Utilitário: a dependência é **transitiva** — ela depende de algo que depende de Utilitário.

A versão correta **inverte** isso: **cada camada de nível superior declara uma interface abstrata** para os serviços de que precisa; as camadas inferiores **implementam** essas interfaces.

::: figura Camadas invertidas
<svg viewBox="0 0 760 270" role="img" aria-label="Cada camada declara uma interface; a camada abaixo a implementa">
  <rect class="f-prim" x="20" y="10" width="180" height="60" rx="12"/><text x="110" y="46" text-anchor="middle" font-weight="800">Camada Política</text>
  <rect class="f-info" x="230" y="10" width="200" height="60" rx="12"/><text x="330" y="40" text-anchor="middle" font-size="13" font-weight="700">«interface»</text><text x="330" y="58" text-anchor="middle" font-size="12" class="t-mono">IServicoDaPolitica</text>
  <path class="arrow" d="M202 40H228"/>
  <rect class="f-prim" x="20" y="110" width="180" height="60" rx="12" opacity=".75"/><text x="110" y="146" text-anchor="middle" font-weight="800">Camada Mecanismo</text>
  <path class="arrow" d="M330 112V72" style="stroke:var(--good);marker-end:url(#arr)"/><path class="line" d="M200 140H330" stroke-dasharray="4 3"/>
  <rect class="f-info" x="230" y="110" width="200" height="60" rx="12" opacity=".0"/>
  <rect class="f-info" x="480" y="110" width="220" height="60" rx="12"/><text x="590" y="140" text-anchor="middle" font-size="13" font-weight="700">«interface»</text><text x="590" y="158" text-anchor="middle" font-size="12" class="t-mono">IServicoDoMecanismo</text>
  <path class="arrow" d="M202 140H478" style="stroke:var(--primary)"/>
  <rect class="f-good" x="20" y="205" width="180" height="55" rx="12"/><text x="110" y="238" text-anchor="middle" font-weight="800" class="t-good">Camada Utilitário</text>
  <path class="arrow" d="M200 232H590V172" style="stroke:var(--good);marker-end:url(#arr)"/>
  <text x="480" y="260" font-size="12" class="t-dim">as camadas de baixo implementam interfaces declaradas pelas de cima</text>
</svg>
:::

Resultado: some a dependência **transitiva** da Política em relação a Utilitário, e some também a dependência **direta** da Política em relação a Mecanismo.

### Inversão de posse

A inversão é também de **posse da interface**. Costumamos pensar que as bibliotecas de utilidades "têm" suas interfaces. Com o DIP, os **clientes possuem** as interfaces abstratas e os **servidores derivam** delas. Em outras palavras, a interface mora **no mesmo assembly (projeto) do cliente**; o projeto do servidor passa a **referenciar** o do cliente — não o contrário.

Às vezes isso se chama **Princípio de Hollywood**: *"não nos procure; nós procuramos você"*. A camada de baixo implementa o que a de cima declarou e chamará.

::: dica Mais de um cliente?
Se existem **muitos clientes** e **um** servidor, não faz sentido o servidor depender de um cliente específico. Nesse caso, os clientes combinam a interface de serviço e a publicam em um **assembly separado**, nem do cliente, nem do servidor.
:::

## A heurística "dependa de abstrações"

Uma leitura mais simples do DIP — ainda poderosa: **não dependa de classes concretas; faça todo relacionamento terminar numa classe abstrata ou interface**.

- Nenhuma variável deve conter referência a uma **classe concreta volátil**.
- Nenhuma classe deve **derivar** de uma classe concreta.
- Nenhum método deve **sobrescrever** um método já implementado da base.

Naturalmente, ela é violada **pelo menos uma vez** em todo programa: alguém precisa **criar** as instâncias concretas (normalmente `Main` ou o *composition root*). E não há motivo para aplicá-la a classes concretas **não voláteis**: `string`, `DateTime`, `List<T>` não mudam (e não há outras "derivadas" a vir). **Depender de `string` não causa dano.**

O alvo são as classes concretas **que você escreve** e que **mudam com frequência**: elas devem ficar atrás de uma interface.

## Exemplo 1: botão e lâmpada (versão própria: alarme e sirene)

```cs Ruim — a política depende do detalhe
public class SensorDeFumaca
{
    private readonly SireneEletrica sirene = new SireneEletrica();   // classe concreta!

    public void Verificar(double nivel)
    {
        if (nivel > 0.3) sirene.Ligar();
    }
}
```

`SensorDeFumaca` só sabe acionar **uma** sirene elétrica. Não dá para usar o mesmo sensor para acender um **LED**, mandar um **SMS**, fechar uma **porta corta-fogo**... A política ("ao detectar fumaça, acionar *algo*") está presa ao detalhe.

**Qual é a abstração subjacente?** *"Detectar uma condição e avisar um dispositivo."* O mecanismo de detecção? Detalhe. Qual dispositivo? Também. Invertemos:

```cs Bom — ambos dependem da abstração
public interface IDispositivoAcionavel
{
    void Ligar();
    void Desligar();
}

public class SensorDeFumaca
{
    private readonly IDispositivoAcionavel dispositivo;

    public SensorDeFumaca(IDispositivoAcionavel dispositivo)   // injeção por construtor
    {
        this.dispositivo = dispositivo;
    }

    public void Verificar(double nivel)
    {
        if (nivel > 0.3) dispositivo.Ligar();
        else dispositivo.Desligar();
    }
}

public class SireneEletrica : IDispositivoAcionavel { public void Ligar() { /*...*/ } public void Desligar() { /*...*/ } }
public class PortaCortaFogo : IDispositivoAcionavel { public void Ligar() { /*fecha*/ } public void Desligar() { /*abre*/ } }
public class NotificadorSms : IDispositivoAcionavel { public void Ligar() { /*envia*/ } public void Desligar() { } }
```

O sensor agora controla **qualquer coisa** que implemente a interface — inclusive dispositivos que **ainda não foram inventados**. Repare no nome: se chamássemos a interface de `ISirene`, o sensor continuaria "dependendo do nome" de um detalhe. **Nomes importam**: `IDispositivoAcionavel` é genérico o bastante para ser **de ninguém** e de todos. Em C#, ela pode ficar em **seu próprio projeto/namespace**, referenciado por sensor e dispositivos.

::: passos Como a injeção de dependência monta o sistema
**Composition root (Main):** é o **único lugar** que conhece as classes concretas.
```cs
var sirene = new SireneEletrica();
var sensor = new SensorDeFumaca(sirene);   // "injeta" o detalhe na política
```
===
**Trocar o detalhe** é só mudar a composição — **a política não muda**:
```cs
var sensor = new SensorDeFumaca(new PortaCortaFogo());
```
===
**Testar a política** vira trivial: injeta-se um **falso**:
```cs
var falso = new DispositivoFalso();
new SensorDeFumaca(falso).Verificar(0.5);
Assert.IsTrue(falso.FoiLigado);
```
===
**Em frameworks modernos** (ASP.NET Core) o **contêiner de injeção de dependência** monta o grafo para você:
```cs
services.AddScoped<IDispositivoAcionavel, SireneEletrica>();
services.AddScoped<SensorDeFumaca>();
```
:::

## Exemplo 2: o termostato

Um software controla um forno: lê a temperatura de um canal de E/S e liga/desliga o aquecedor em outro. O algoritmo inicial mistura intenção e hardware:

```cs Misturado
const byte TERMOMETRO = 0x86, FORNO = 0x87, LIGAR = 1, DESLIGAR = 0;

void Regular(double min, double max)
{
    while (true)
    {
        while (Entrada(TERMOMETRO) > min) Esperar(1);
        Saida(FORNO, LIGAR);
        while (Entrada(TERMOMETRO) < max) Esperar(1);
        Saida(FORNO, DESLIGAR);
    }
}
```

A **intenção** é nítida ("quando esfriar demais, ligue; quando esquentar o bastante, desligue"), mas está soterrada em endereços de hardware: **jamais** será reutilizada com outro controlador. Invertendo:

```cs Invertido
public interface ITermometro { double Ler(); }
public interface IAquecedor { void Ligar(); void Desligar(); }

public class Termostato
{
    private readonly ITermometro termometro;
    private readonly IAquecedor aquecedor;
    public Termostato(ITermometro t, IAquecedor a) { termometro = t; aquecedor = a; }

    public void Regular(double min, double max)
    {
        while (true)
        {
            while (termometro.Ler() > min) Esperar(1);
            aquecedor.Ligar();
            while (termometro.Ler() < max) Esperar(1);
            aquecedor.Desligar();
        }
    }
    private void Esperar(int s) { /* ... */ }
}
```

`Termostato` não conhece canais de E/S. Para cada hardware existe um par de **adaptadores** (`TermometroDeCanal`, `AquecedorDeCanal`) que implementam as interfaces. O algoritmo é **perfeitamente reutilizável**.

## DIP, OCP e Strategy

O DIP é o **mecanismo de baixo nível** por trás do OCP: para fechar `SensorDeFumaca` à troca de dispositivo, ele depende de uma abstração. Na prática, o padrão **Strategy** (capítulo 30) é exatamente "aplicar o DIP para tornar um comportamento intercambiável".

## DIP ≠ injeção de dependência ≠ inversão de controle

| Termo | O que é |
|---|---|
| **DIP** | **Princípio** de projeto: dependa de abstrações; interfaces pertencem a quem as usa |
| **Injeção de dependência (DI)** | **Técnica** de receber as dependências de fora (construtor/propriedade/método) |
| **Contêiner de DI** | **Ferramenta** que monta e injeta as dependências automaticamente |
| **Inversão de controle (IoC)** | Estilo em que o **framework** chama o seu código (Hollywood) |

::: cuidado `new` espalhado derruba o DIP
Cada `new ClasseConcreta()` dentro da lógica é uma **dependência escondida**. Isole as criações na **raiz de composição** (ou em **fábricas**, capítulo 34) e passe **interfaces** adiante.
:::

::: cuidado Interface "espelho" não é abstração
Criar `IRepositorioSql` com métodos como `ExecutarSelect(string sql)` só **renomeia** o detalhe. Uma boa abstração é definida pela **necessidade do cliente** (`IRepositorioDePedidos.ObterPorCliente(id)`), não pela API da biblioteca concreta.
:::

::: exercicio Inverta a dependência
Este código envia relatórios. Aplique o DIP.

```cs
public class Relatorio
{
    public void Enviar(string texto)
    {
        var smtp = new SmtpClient("smtp.empresa.com");
        smtp.Send("rel@empresa.com", "chefe@empresa.com", "Relatório", texto);
    }
}
```

--- resposta
```cs
public interface IEnviadorDeMensagens { void Enviar(string destino, string assunto, string corpo); }

public class Relatorio
{
    private readonly IEnviadorDeMensagens enviador;
    public Relatorio(IEnviadorDeMensagens enviador) { this.enviador = enviador; }
    public void Enviar(string texto) { enviador.Enviar("chefe@empresa.com", "Relatório", texto); }
}

public class EnviadorSmtp : IEnviadorDeMensagens
{
    public void Enviar(string destino, string assunto, string corpo)
    {
        var smtp = new SmtpClient("smtp.empresa.com");
        smtp.Send("rel@empresa.com", destino, assunto, corpo);
    }
}
```
Agora `Relatorio` pode enviar por e-mail, Teams ou SMS sem mudar, e pode ser **testado** com um enviador falso. A interface foi definida pela **necessidade de Relatorio**, não pela API do SMTP.
:::

::: resumo
- **DIP:** (A) alto nível e baixo nível dependem de **abstrações**; (B) abstrações não dependem de detalhes — detalhes dependem de abstrações.
- Programação procedural faz a **política depender do detalhe**; OO **inverte** isso — marca registrada do bom projeto OO.
- **Inversão de posse:** a interface pertence ao **cliente** (ou a um pacote neutro); o servidor implementa.
- Heurística: **dependa de abstrações**, ao menos para classes concretas **voláteis**; `string` e afins podem ser usados diretamente.
- **Composição** (criar e ligar objetos) fica na **raiz**; **injeção de dependência** passa as interfaces.
- É a base do **OCP**, do **Strategy**, dos **frameworks** e da **testabilidade**.
:::

::: quiz
? Qual é a ideia central do DIP?
+ Políticas de alto nível e detalhes de baixo nível devem depender de abstrações
- Detalhes devem sempre depender de políticas concretas
- Classes concretas são sempre melhores que interfaces
- Toda classe deve ter uma interface
> A direção da dependência é "invertida" em relação ao projeto procedural.

? Em "inversão de posse", quem possui a interface?
+ O cliente (ou um pacote neutro), não o servidor que a implementa
- O servidor que a implementa
- O sistema operacional
- Ninguém
> Assim o servidor depende do cliente e não o contrário.

? Por que depender de `string` não viola o espírito do DIP?
+ Porque é uma classe concreta estável (não volátil)
- Porque string é uma interface
- Porque string não existe em .NET
- Porque string é abstrata
> O DIP importa para detalhes que mudam.

? Em qual lugar é aceitável instanciar classes concretas?
+ Na raiz de composição (por exemplo, `Main`), onde o grafo de objetos é montado
- Dentro de toda regra de negócio
- Dentro de cada método
- Em lugar nenhum
> Alguém precisa criar as instâncias; isole isso.

? O que é injeção de dependência?
+ Receber as dependências (como interfaces) de fora, em vez de criá-las internamente
- Um tipo de exceção
- Um comando SQL
- Um modificador de acesso
> Técnica que viabiliza o DIP.

? O que há de errado numa interface `IRepositorioSql` com `ExecutarSelect(string sql)`?
+ Ela só renomeia o detalhe; a abstração deve refletir a necessidade do cliente
- Nada
- Interfaces não podem ter strings
- Ela é concreta demais para C#
> Defina a abstração pela necessidade do cliente.

? Qual padrão é a aplicação direta do DIP para tornar comportamento intercambiável?
+ Strategy
- Singleton
- Visitor
- Proxy
> O cliente depende de uma interface e a implementação é plugável.
:::

::: cartas
DIP :: Alto e baixo nível dependem de abstrações; abstrações não dependem de detalhes.
Política (alto nível) :: Regras de negócio e decisões centrais do aplicativo.
Detalhe (baixo nível) :: Mecanismos específicos: banco, dispositivo, API, arquivo.
Inversão de posse :: A interface pertence ao cliente, não ao servidor.
Princípio de Hollywood :: "Não nos procure; nós procuramos você."
Injeção de dependência :: Passar as dependências de fora (construtor, propriedade).
Raiz de composição :: Ponto único onde objetos concretos são criados e ligados.
Dependência transitiva :: A depende de B que depende de C ⇒ A depende de C.
:::
