---
id: agil-cap22
order: 38
kind: capitulo
num: 22
group: C# · Parte 8 · Projeto ágil e princípios SOLID
title: Princípio da Substituição de Liskov (LSP)
subtitle: Subtipos devem poder substituir seus tipos base — "é-um" é sobre comportamento, não sobre vocabulário. Contratos, heurísticas e fatoração.
minutes: 55
hue: 15
objectives:
- Enunciar o **LSP** e explicar por que uma violação é uma **violação latente do OCP**
- Mostrar, com o clássico **quadrado × retângulo**, que "é-um" vale **no comportamento**
- Usar **projeto por contrato** (pré e pós-condições) e **testes** para tornar suposições explícitas
- Corrigir hierarquias com **fatoração** (classe/interface comum) e reconhecer **métodos degenerados**
origem: livros/02-projeto-agil-csharp/content/37-cap22.md
---

::: ilustracao img/agil/cap22.svg | LSP: se parece com pato, grasna como pato, mas precisa de pilhas, a abstração está errada.
:::

## O princípio

O OCP depende de **abstração** e **polimorfismo**; em C#, o principal mecanismo que os sustenta é a **herança**. Que regras governam um bom uso da herança? Que armadilhas nos fazem criar hierarquias que quebram o OCP? Essas perguntas são respondidas pelo princípio que Barbara Liskov formulou em 1988:

::: regra LSP — Liskov Substitution Principle
**Os subtipos devem ser substituíveis por seus tipos base.**
Se uma função `f` funciona com um `B`, ela deve continuar funcionando, **sem saber disso**, quando receber qualquer `D` derivado de `B`.
:::

Pense assim: se `f(Base b)` se comporta mal quando recebe um `Derivada`, então `Derivada` viola o LSP — e é **frágil** na presença de `f`. A tentação do autor de `f` é "dar um jeitinho": testar se o objeto é `Derivada` e tratá-la à parte. Esse teste viola o **OCP**, pois `f` deixa de ser fechada para as derivadas de `B`. Por isso:

::: regra Uma violação do LSP é uma violação latente do OCP
Toda vez que você vê `if (x is Subclasse)` num código que recebe a classe base, é provável que uma hierarquia mal projetada esteja por trás.
:::

## Violação simples: verificar tipos em tempo de execução

```cs Violação evidente
public class Forma { public TipoForma Tipo; }
public class Circulo  : Forma { public void Desenhar() { /* ... */ } }   // sem override!
public class Quadrado : Forma { public void Desenhar() { /* ... */ } }

public static void DesenharForma(Forma f)
{
    if (f.Tipo == TipoForma.Circulo)       ((Circulo)f).Desenhar();
    else if (f.Tipo == TipoForma.Quadrado) ((Quadrado)f).Desenhar();
}
```

`Circulo` e `Quadrado` **não podem ser substituídos** por `Forma`: ninguém consegue chamar `Desenhar` pela base. Isso obriga `DesenharForma` a espiar o tipo (e a mudar a cada forma nova). A correção é óbvia: `Desenhar` abstrato/virtual em `Forma` e **polimorfismo de verdade**.

## Violação sutil: o quadrado que "é um" retângulo

Um aplicativo funciona bem com a classe `Retangulo`. Um dia os usuários pedem **quadrados**. Em matemática, quadrado é um retângulo; "herança é é-um"; então:

```cs
public class Retangulo
{
    public virtual double Largura { get; set; }
    public virtual double Altura { get; set; }
    public double Area { get { return Largura * Altura; } }
}

public class Quadrado : Retangulo
{
    public override double Largura { set { base.Largura = value; base.Altura = value; } get { return base.Largura; } }
    public override double Altura  { set { base.Largura = value; base.Altura = value; } get { return base.Altura; } }
}
```

Sinais de alerta logo no começo: o quadrado **herda dois campos** (largura e altura) de que não precisa; e para manter o invariante "lados iguais", sobrescrevemos os setters — **e precisamos tornar virtual** o que na classe base não era, ou seja, **alteramos a base por causa da derivada** (cheiro de OCP quebrado).

Mas o verdadeiro problema aparece num cliente:

```cs
void Verificar(Retangulo r)
{
    r.Largura = 5;
    r.Altura = 4;
    if (r.Area != 20)
        throw new Exception("Área incorreta!");
}
```

Com um `Retangulo`, tudo bem. Com um `Quadrado`, `Altura = 4` também muda a largura para 4 e a área é **16** — exceção. O autor de `Verificar` fez uma suposição razoável: **mudar a largura não altera a altura**. Isso é um **invariante** de `Retangulo`; o `Quadrado` o violou.

::: figura Por que o "é-um" falhou
<svg viewBox="0 0 740 230" role="img" aria-label="Cliente espera comportamento independente de largura e altura; Quadrado não cumpre">
  <rect class="f-prim" x="10" y="70" width="170" height="70" rx="12"/><text x="95" y="98" text-anchor="middle" font-weight="800">Verificar(Retangulo r)</text><text x="95" y="120" text-anchor="middle" font-size="12" class="t-dim">r.Largura = 5; r.Altura = 4;</text>
  <path class="arrow" d="M182 105H300"/><text x="240" y="95" text-anchor="middle" font-size="12" class="t-dim">espera área = 20</text>
  <rect class="f-good" x="302" y="20" width="200" height="70" rx="12"/><text x="402" y="50" text-anchor="middle" font-weight="800" class="t-good">Retangulo ✔</text><text x="402" y="72" text-anchor="middle" font-size="12">5 × 4 = 20</text>
  <rect class="f-bad" x="302" y="130" width="200" height="70" rx="12"/><text x="402" y="160" text-anchor="middle" font-weight="800" class="t-bad">Quadrado ✘</text><text x="402" y="182" text-anchor="middle" font-size="12">4 × 4 = 16 → exceção!</text>
  <path class="arrow" d="M182 118L300 160" style="stroke:var(--bad);marker-end:url(#arr3)"/>
  <text x="560" y="115" font-size="13" class="t-dim">O modelo é coerente</text><text x="560" y="135" font-size="13" class="t-dim">sozinho… e inválido</text><text x="560" y="155" font-size="13" class="t-dim">para quem o usa.</text>
</svg>
:::

### A validade não é intrínseca

Examinadas isoladamente, as duas classes eram **coerentes e válidas**. O LSP revela: **um modelo não pode ser validado isoladamente; a validade só se mede em relação a seus clientes** e às suposições razoáveis que eles fazem. Curiosamente, o autor de `Quadrado` não violou nenhum invariante **de `Quadrado`** — violou um invariante de `Retangulo`.

### "É-um" diz respeito ao comportamento

Um quadrado *poderia* ser um retângulo na geometria. Mas do ponto de vista de `Verificar`, um `Quadrado` **não é** um `Retangulo`, porque seu **comportamento** não é compatível com o que o cliente espera. Software é sobre comportamento.

::: regra "É-um" = substituível
A definição verdadeira de subtipo não é "se parece com", é **"pode ser usado no lugar do tipo base, segundo um contrato explícito ou implícito"**.
:::

## Projeto por contrato

Como saber que suposições os clientes farão? Uma técnica é tornar o **contrato** explícito (*Design by Contract*, de Bertrand Meyer): cada método declara

- **pré-condições** — o que deve ser verdade **antes** de executar;
- **pós-condições** — o que o método **garante** depois.

A regra de Meyer para derivadas:

::: regra Regra das pré e pós-condições
Uma classe derivada pode **enfraquecer** (ou manter) as **pré-condições** — aceitar **mais** do que a base — e pode **fortalecer** (ou manter) as **pós-condições** — prometer **mais**. Nunca o contrário.
:::

Em linguagem comum: **quem usa a base não pode ser surpreendido** — nem por exigir mais na entrada, nem por entregar menos na saída. A pós-condição do `set Largura` do retângulo inclui "a altura continua a mesma"; a do quadrado é **mais fraca** (não garante isso) — logo, viola o contrato.

C# não tem pré/pós-condições nativas como Eiffel; é preciso **declará-las você mesmo**: comentários XML, validações (`ArgumentException`), `Debug.Assert`, **Code Contracts** (histórico) e, principalmente, **testes de unidade** que descrevem o comportamento esperado da base:

```cs Um teste que expressa o contrato da base
// Todas as implementações de IRepositorio devem passar nele.
public abstract class RepositorioContractTests
{
    protected abstract IRepositorio Criar();

    [Test]
    public void Adicionar_DepoisContem()
    {
        var repo = Criar();
        repo.Adicionar("x");
        Assert.IsTrue(repo.Contem("x"));
    }

    [Test]
    public void Remover_ItemInexistente_NaoLancaExcecao()
    {
        Assert.DoesNotThrow(() => Criar().Remover("nao existe"));
    }
}
```

Cada nova implementação herda esses testes (`class RepositorioSqlTests : RepositorioContractTests`) — e o contrato passa a ser **verificado automaticamente**.

## Um caso real: o conjunto "persistente"

Considere uma interface `IConjunto` com `Adicionar(object)`, `Remover(object)` e `Contem(object)`, e duas implementações (uma baseada em array, outra em lista). Um cliente pode escolher qualquer uma sem se importar. Quer-se agora um `ConjuntoPersistente`, que grava em stream. Ele só aceita elementos que herdam de `ObjetoPersistente`:

```cs
public class ConjuntoPersistente : IConjunto
{
    public void Adicionar(object o)
    {
        var p = (ObjetoPersistente)o;      // 💥 InvalidCastException para outros objetos
        interno.Adicionar(p);
    }
    /* ... */
}
```

Nenhum cliente de `IConjunto` espera uma **exceção** em `Adicionar`. A **pré-condição** ficou **mais forte** que a da base ("só aceita objetos persistentes"). Funções que nunca falharam passam a falhar quando recebem um `ConjuntoPersistente` — e o erro de execução aparece **longe** da decisão errada (a milhões de instruções de distância). Depurar é difícil; corrigir, pior.

### A solução "por convenção" não funciona

Esconder a classe do resto do sistema por convenção ("ninguém usa `ConjuntoPersistente` fora deste módulo") **parece** resolver, mas convenções **precisam ser vendidas a cada desenvolvedor**; quem não as conhece ou discorda as viola — e uma violação pode comprometer a estrutura toda.

### A solução que respeita o LSP

Reconhecer que `ConjuntoPersistente` **não é-um** `IConjunto` (no comportamento) e **separar as hierarquias**, mantendo o que há em comum:

::: figura Compatível com o LSP
<svg viewBox="0 0 700 250" role="img" aria-label="IContainer com Contem e Remover; IConjunto e IConjuntoPersistente irmãs">
  <rect class="f-info" x="240" y="10" width="220" height="60" rx="12"/><text x="350" y="36" text-anchor="middle" font-weight="800" class="t-info">«interface» IContainer</text><text x="350" y="56" text-anchor="middle" font-size="12" class="t-mono">Contem(item)  Remover(item)</text>
  <path class="line" d="M350 72V100M170 100H530M170 100V130M530 100V130"/>
  <rect class="f-good" x="40" y="130" width="260" height="70" rx="12"/><text x="170" y="158" text-anchor="middle" font-weight="800" class="t-good">«interface» IConjunto&lt;T&gt;</text><text x="170" y="180" text-anchor="middle" font-size="12" class="t-mono">Adicionar(T item)</text>
  <rect class="f-warn" x="400" y="130" width="260" height="70" rx="12"/><text x="530" y="158" text-anchor="middle" font-weight="800" class="t-warn">«interface» IConjuntoPersistente</text><text x="530" y="180" text-anchor="middle" font-size="12" class="t-mono">Adicionar(ObjetoPersistente p)</text>
  <text x="350" y="232" text-anchor="middle" font-size="12" class="t-dim">Parte comum (testar, iterar) numa base; a diferença de tipo em Adicionar fica explícita.</text>
</svg>
:::

Agora quem recebe um `IContainer` pode iterar e testar pertinência; quem precisa de `Adicionar` sabe **exatamente** o que aceita. O compilador impede o erro, em vez do tempo de execução.

## Fatorar em vez de derivar

Outro caso enigmático: **reta** e **segmento de reta**. `Segmento` precisa de tudo que `Reta` tem (dois pontos, inclinação…) e acrescenta `Comprimento`; sobrescreve `EstaSobre(Ponto)`. Parece herança natural. Mas um usuário de `Reta` tem o direito de esperar que o **ponto onde a reta cruza o eixo Y** esteja sobre ela: `EstaSobre(InterceptoY) == true`. Num segmento, isso muitas vezes falha — LSP violado, de forma sutil.

A solução: **fatorar** o que é comum numa **classe base abstrata** e deixar as duas como **irmãs**:

```cs
public abstract class ObjetoLinear
{
    public Ponto P1 { get; }
    public Ponto P2 { get; }
    protected ObjetoLinear(Ponto p1, Ponto p2) { P1 = p1; P2 = p2; }
    public double Inclinacao { get { /* ... */ return 0; } }
    public abstract bool EstaSobre(Ponto p);
}

public class Reta : ObjetoLinear
{
    public Reta(Ponto a, Ponto b) : base(a, b) { }
    public override bool EstaSobre(Ponto p) { /* em toda a reta */ return true; }
}

public class Segmento : ObjetoLinear
{
    public Segmento(Ponto a, Ponto b) : base(a, b) { }
    public double Comprimento { get { /* ... */ return 0; } }
    public override bool EstaSobre(Ponto p) { /* só entre P1 e P2 */ return true; }
}

public class Semirreta : ObjetoLinear { /* e amanhã outras, sem quebrar ninguém */
    public Semirreta(Ponto a, Ponto b) : base(a, b) { }
    public override bool EstaSobre(Ponto p) { return true; } }
```

Quem usa `ObjetoLinear` **não assume** que conhece a extensão do objeto; aceita reta, segmento ou semirreta sem surpresas.

::: dica Fatoração é poderosa
Se duas subclasses compartilham qualidades, é provável que **outras** apareçam. **Mova as responsabilidades comuns para uma superclasse** (provavelmente abstrata) e deixe as classes originais como **irmãs**, em vez de uma derivar da outra.
:::

::: cuidado Às vezes aceita-se o compromisso
Engenharia é compromisso. Há raras ocasiões em que é melhor **aceitar uma falha sutil** no polimorfismo do que contorcer o projeto atrás de compatibilidade completa. Mas **não abandone o LSP levianamente**: a garantia de que uma subclasse funciona em qualquer lugar da base é uma forma poderosa de gerenciar complexidade; ao abandoná-la, você terá de analisar **cada subclasse individualmente**.
:::

## Heurísticas para encontrar violações

Quase todas as pistas têm a mesma raiz: **a derivada faz menos que a base**.

| Sinal | Exemplo | Risco |
|---|---|---|
| **Método degenerado** | `override void Voar() { }` (vazio) | Clientes de `Ave` chamam `Voar()` e nada acontece |
| **`NotImplementedException` / `NotSupportedException`** | `override Salvar() => throw new NotSupportedException();` | O cliente é surpreendido por exceção |
| **Verificação de tipo no cliente** | `if (obj is Subclasse)` | Violação latente do OCP |
| **Pré-condição mais forte** | A derivada recusa valores que a base aceita | Falha em tempo de execução |
| **Pós-condição mais fraca** | Retorna `null` onde a base nunca retornava | `NullReferenceException` longe da causa |
| **Efeito colateral inesperado** | Setter que altera outra propriedade | Estados incoerentes (caso do quadrado) |

::: analogia O pinguim
"Toda ave voa" parece natural, até você criar `Pinguim : Ave`. Se `Ave` tem `Voar()`, o pinguim ou lança exceção (clientes se surpreendem) ou fica vazio (mente). O modelo correto separa **comportamentos**: `Ave` (comer, botar ovos) e uma interface `IVoador` só para quem voa. O código que precisa de voo pede `IVoador`; ninguém é enganado.
:::

::: exercicio Substituível ou não?
Qual destes designs viola o LSP? Justifique e proponha uma correção.

```cs
class Conta { public virtual void Sacar(decimal v) { saldo -= v; } }
class ContaPoupanca : Conta
{
    public override void Sacar(decimal v)
    {
        if (v > 500) throw new InvalidOperationException("Limite de saque excedido");
        base.Sacar(v);
    }
}
```

--- resposta
Viola: a **pré-condição ficou mais forte**. Quem usa `Conta` pode sacar qualquer valor ≤ saldo; com `ContaPoupanca`, saques acima de 500 lançam exceção — o cliente é surpreendido.

Correções possíveis: (1) tornar o limite **parte do contrato da base** (`Conta.LimiteDeSaque` e um método `PodeSacar(v)` que todos respeitam); (2) **não herdar**: `ContaPoupanca` e `ContaCorrente` implementam `IConta` e o código que precisa de saques ilimitados pede um tipo/interface que garanta isso; (3) usar **composição** com uma `RegraDeSaque` (Strategy) injetada.
:::

::: resumo
- **LSP:** subtipos devem ser **substituíveis** por seus tipos base sem que o cliente perceba.
- Violação ⇒ clientes passam a **checar tipos** ⇒ **violação do OCP**.
- **"É-um" é sobre comportamento**: o contrato (invariantes, pré e pós-condições) que os clientes razoavelmente supõem.
- Derivadas podem **aceitar mais** (pré-condições mais fracas) e **prometer mais** (pós-condições mais fortes); nunca o inverso.
- Torne contratos explícitos com **testes**, validações e documentação.
- Conserte hierarquias **fatorando** o que é comum numa abstração e fazendo as classes **irmãs**.
- Sinais: **métodos vazios**, `NotImplementedException`, `is` no cliente, pré-condições mais fortes.
:::

::: quiz
? Qual é o enunciado do LSP?
+ Subtipos devem ser substituíveis por seus tipos base
- Uma classe deve ter apenas um motivo para mudar
- Módulos de alto nível não devem depender de baixo nível
- Interfaces devem ser pequenas
> Funções escritas para a base devem funcionar com qualquer derivada.

? Por que o LSP é chamado de "capacitador" do OCP?
+ Porque a substituibilidade permite que módulos escritos para a base sejam estendidos sem modificação
- Porque substitui o OCP
- Porque proíbe herança
- Porque exige interfaces
> Sem substituibilidade, clientes passam a testar tipos e deixam de ser fechados.

? No exemplo, por que `Quadrado` não é um `Retangulo` para o método `Verificar`?
+ Porque o comportamento do quadrado (largura e altura acopladas) viola o que o cliente supõe do retângulo
- Porque quadrados não existem em C#
- Porque Quadrado não tem construtor
- Porque Retangulo é abstrata
> "É-um" depende do comportamento esperado pelo cliente.

? Qual regra vale para pré-condições em classes derivadas?
+ Podem ser iguais ou mais fracas (aceitar mais)
- Devem ser mais fortes
- Devem ser removidas
- Podem ser quaisquer
> A derivada não pode exigir mais do que a base exige.

? Qual é um sinal típico de violação do LSP?
+ Um método sobrescrito vazio ou que lança NotSupportedException
- O uso de propriedades automáticas
- Herança de duas classes
- Nomes longos
> A derivada faz menos que a base.

? Como corrigir `Reta`/`Segmento` quando uma derivar da outra viola o LSP?
+ Fatorar o que é comum numa classe base abstrata e tornar as duas irmãs
- Tornar tudo `static`
- Copiar o código das duas
- Usar `object`
> Fatoração em vez de herança direta.

? Por que uma "convenção" é uma solução frágil para uma violação do LSP?
+ Porque precisa ser comunicada a todos e basta uma violação para comprometer a estrutura
- Porque convenções são proibidas
- Porque não podem ser escritas
- Porque o compilador as ignora
> Prefira que o compilador (tipos/interfaces) impeça o erro.
:::

::: cartas
LSP :: Subtipos devem poder substituir seus tipos base sem alterar a correção do programa.
Invariante :: Propriedade que sempre deve ser verdadeira para um tipo.
Pré-condição :: O que deve ser verdade antes de chamar um método.
Pós-condição :: O que o método garante depois de executar.
Projeto por contrato :: Declarar explicitamente pré/pós-condições e invariantes de cada tipo.
Método degenerado :: Método sobrescrito que não faz nada ou lança exceção; sinal de violação.
Fatoração :: Extrair o comum a duas classes para uma abstração e torná-las irmãs.
"É-um" comportamental :: Relação de subtipo definida por substituibilidade, não por semelhança.
:::
