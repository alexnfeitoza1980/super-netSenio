---
id: aprenda-cap06
order: 17
kind: capitulo
num: 6
group: C# · Parte 3 · Orientação a objetos
title: Herança
subtitle: Sua árvore genealógica de objetos — reutilize código com classes base e subclasses, `override`, `base` e construtores.
minutes: 60
hue: 150
objectives:
- Criar uma **hierarquia de classes** do geral para o específico
- Usar `:` para herdar, `virtual`/`override` para **substituir** comportamento e `base` para chamar o pai
- Entender por que **onde cabe a classe base, cabe a subclasse**
- Fazer **construtores** em cadeia entre subclasse e classe base
origem: livros/01-aprenda-csharp/content/07-cap06.md
---

::: ilustracao img/aprenda/cap06.svg | Herança: filhos ganham o que o pai tem e acrescentam o que é só seu.
:::

## Às vezes você REALMENTE quer ser igual aos seus pais

Já aconteceu de achar um objeto que faz **quase** exatamente o que você precisa? Se você pudesse mudar só um pedacinho, ficaria perfeito. A **herança** resolve isso: você cria uma **subclasse** que **herda** o comportamento de uma classe existente e muda apenas o que for diferente.

Resultado: **menos código duplicado**, um modelo mais próximo do mundo real e programas mais fáceis de manter.

::: vila Catarina organiza aniversários também
Depois do orçamento de festas, Catarina quer estimar **festas de aniversário**. Elas se parecem **muito** com os eventos de antes (convidados, comida, decoração) — com extras: o **bolo** (preço depende do número de letras da mensagem) e **velas**. E uma taxa a mais para festas com mais de 12 pessoas.
:::

## O problema de copiar e colar

Poderíamos copiar `PartyPlanner` e criar `BirthdayParty`... mas teríamos **duas** cópias de quase tudo. Se amanhã mudar o preço da comida, é preciso lembrar de mexer nos dois lugares — e algum será esquecido. Esse é o **custo do código duplicado**: bugs.

Com herança, `BirthdayParty` **estende** `Party` e acrescenta só o que é novo:

```cs
class Party                          // classe base (superclasse)
{
    public int NumberOfPeople { get; set; }
    public bool FancyDecorations { get; set; }
    public virtual decimal Cost       // virtual: subclasses podem substituir
    {
        get
        {
            decimal cost = NumberOfPeople * 25m;        // comida
            cost += FancyDecorations ? NumberOfPeople * 15m + 50m : NumberOfPeople * 7.5m + 30m;
            return cost;
        }
    }
}

class BirthdayParty : Party          // subclasse: herda tudo de Party
{
    public string CakeWriting { get; set; }

    public override decimal Cost     // substitui o cálculo
    {
        get
        {
            decimal cost = base.Cost;            // reaproveita o cálculo do pai
            cost += CakeWriting.Length * 0.25m;   // + cobertura do bolo
            if (NumberOfPeople > 12) cost += 100m;  // taxa de festa grande
            return cost;
        }
    }
}
```

::: regra Escreva uma vez só
Quando suas classes usam herança, você escreve cada trecho de lógica **uma única vez**: na classe base, onde todos os "filhos" o encontram.
:::

## Modele do geral para o específico

Ao projetar uma hierarquia, comece **no topo, pelo mais geral**, e vá especializando.

::: pense Como você modelaria um simulador de zoológico?
Pense em hipopótamos, leões, lobos, cães, gatos... Que **características** eles têm em comum? Todos comem, dormem, fazem barulho e se movem. Alguns são **felinos**, outros **canídeos**. O que seria a classe mais geral?
:::

### Tudo começa pela classe mais geral

Comece perguntando: *"o que todo animal sabe e faz?"* Isso vai para a classe **base** (`Animal`). Depois, agrupe os animais **parecidos**: canídeos (lobo, cão), felinos (leão, tigre), e assim por diante.

::: figura A árvore de classes do zoológico
<svg viewBox="0 0 700 330" role="img" aria-label="Animal na raiz; Hipopótamo, Canino e Felino como filhos; Lobo e Cão sob Canino">
  <g text-anchor="middle">
    <rect class="f-prim" x="270" y="10" width="160" height="50" rx="12"/><text x="350" y="42" font-weight="800" class="t-prim">Animal</text>
    <path class="line" d="M350 60V85M120 85H580M120 85V110M350 85V110M580 85V110"/>
    <rect class="f-info" x="40" y="110" width="160" height="50" rx="12"/><text x="120" y="142" font-weight="800" class="t-info">Hipopótamo</text>
    <rect class="f-good" x="270" y="110" width="160" height="50" rx="12"/><text x="350" y="142" font-weight="800" class="t-good">Canino</text>
    <rect class="f-warn" x="500" y="110" width="160" height="50" rx="12"/><text x="580" y="142" font-weight="800" class="t-warn">Felino</text>
    <path class="line" d="M350 160V185M240 185H460M240 185V210M460 185V210"/>
    <rect class="f-plain" x="160" y="210" width="160" height="50" rx="12"/><text x="240" y="242" font-weight="700">Lobo</text>
    <rect class="f-plain" x="380" y="210" width="160" height="50" rx="12"/><text x="460" y="242" font-weight="700">Cão</text>
  </g>
  <text x="20" y="300" font-size="13" class="t-dim">Cada seta sobe para a classe base: “Lobo É UM Canino, que É UM Animal”.</text>
</svg>
:::

### A relação "é um"

Herança modela a relação **"é um"** (*is-a*): um lobo **é um** canino; um canino **é um** animal. Se a frase não soar natural ("um carro **é um** motor?"), provavelmente **não** é herança — talvez seja composição ("um carro **tem um** motor").

```cs
class Animal
{
    public int Hunger { get; protected set; }
    public virtual void MakeNoise() { Console.WriteLine("..."); }
    public void Eat() { Hunger = 0; }
    public void Sleep() { /* ... */ }
}

class Hippo : Animal   { public override void MakeNoise() { Console.WriteLine("Grunhido!"); } }
class Canine : Animal  { public void Roam() { /* ... */ } }
class Wolf : Canine    { public override void MakeNoise() { Console.WriteLine("Auuu!"); } }
class Dog : Canine     { public override void MakeNoise() { Console.WriteLine("Au au!"); } }
```

- Os dois-pontos **`:`** significam "herda de".
- Uma **subclasse** **estende** a classe base: tem todos os campos, propriedades e métodos dela.
- `Lobo` herda de `Canine`, que herda de `Animal`: tem `Eat()`, `Sleep()` e `Roam()` **sem escrever nada** disso.

::: dica Compartilhe o que for comum, especialize o que for diferente
Se três subclasses fazem a mesma coisa, **suba** o código para a classe base. Se só uma faz diferente, **substitua** nela (`override`).
:::

## Substituindo comportamento: `virtual` e `override`

Todo animal faz barulho, mas **cada um faz um barulho diferente**. Para isso:

1. Na classe **base**, marque o método como **`virtual`** ("pode ser substituído").
2. Na **subclasse**, declare o método com **`override`** ("estou substituindo").

```cs
Animal a = new Wolf();
a.MakeNoise();            // Auuu!   ← o C# executa a versão do objeto REAL (Wolf)
```

::: passos Qual método é executado?
Considere `Animal bicho = new Dog(); bicho.MakeNoise();`

::: html
<svg viewBox="0 0 640 150"><rect class="f-prim" x="10" y="45" width="140" height="44" rx="9"/><text x="80" y="73" text-anchor="middle" class="t-mono" font-size="13">Animal bicho</text><rect class="f-good" x="330" y="15" width="290" height="110" rx="12"/><text x="475" y="42" text-anchor="middle" font-weight="800" class="t-good">objeto Dog</text><text x="475" y="68" text-anchor="middle" class="t-mono" font-size="12">herda: Hunger, Eat(), Sleep(), Roam()</text><text x="475" y="92" text-anchor="middle" class="t-mono" font-size="12">override MakeNoise()</text><path class="arrow" d="M152 67H326"/></svg>
:::
===
A **variável** é do tipo `Animal`, mas o **objeto** apontado é um `Dog`. O C# decide **em tempo de execução**: procura `MakeNoise` primeiro em `Dog`.

::: html
<svg viewBox="0 0 640 110"><g font-size="14"><rect class="f-plain" x="20" y="30" width="170" height="50" rx="10"/><text x="105" y="60" text-anchor="middle" font-weight="700">1º: Dog tem override?</text><rect class="f-good" x="260" y="30" width="170" height="50" rx="10"/><text x="345" y="60" text-anchor="middle" font-weight="700" class="t-good">SIM → executa</text><rect class="f-plain" x="470" y="30" width="150" height="50" rx="10" stroke-dasharray="5 4"/><text x="545" y="60" text-anchor="middle" class="t-dim">(não sobe mais)</text></g><path class="arrow" d="M192 55H258"/><path class="line" d="M432 55H468"/></svg>
:::
===
Se `Dog` **não** tivesse `override`, o C# subiria para `Canine`, depois `Animal`, até achar uma versão. Esse comportamento — mesma chamada, resultados diferentes conforme o objeto — é o **polimorfismo** (aprofundado no capítulo 7).
:::

::: cuidado Esqueceu `virtual` na base?
Se a classe base não marcar o método como `virtual` (ou `abstract`), o `override` **não compila**. E se você declarar um método com o mesmo nome **sem** `override`, o C# avisa que está *ocultando* o método herdado (use `new` conscientemente, mas quase sempre você quer `override`).
:::

## Em qualquer lugar onde cabe a base, cabe a subclasse

Como `Lobo` **é um** `Animal`, você pode usar um lobo onde um animal é esperado.

```cs
void Alimentar(Animal animal) { animal.Eat(); }

Alimentar(new Hippo());
Alimentar(new Wolf());     // funciona: Wolf é um Animal

Animal[] zoo = { new Hippo(), new Wolf(), new Dog() };
foreach (Animal a in zoo) a.MakeNoise();   // cada um faz seu som
```

O contrário **não** vale automaticamente: nem todo `Animal` é um `Wolf`. (Voltaremos a isso com `is`, `as` e *downcasting* no capítulo 7.)

## Acessando a classe base: `base`

Dentro de uma subclasse, `base` refere-se à **classe pai**. Usamos para **reaproveitar** o comportamento herdado em vez de reescrevê-lo.

```cs
class Dog : Canine
{
    public override void MakeNoise()
    {
        base.MakeNoise();                  // o que o pai faz primeiro…
        Console.WriteLine("Au au!");       // …e o que é só do cão
    }
}
```

### Quando a base tem construtor, a subclasse precisa de um também

Se a classe base **só tem** construtor com parâmetros, a subclasse **precisa** chamá-lo, usando `: base(...)`:

```cs
class Party
{
    public int NumberOfPeople { get; }
    public Party(int numberOfPeople) { NumberOfPeople = numberOfPeople; }
}

class BirthdayParty : Party
{
    public string CakeWriting { get; }
    public BirthdayParty(int numberOfPeople, string cakeWriting)
        : base(numberOfPeople)        // executa primeiro o construtor do pai
    {
        CakeWriting = cakeWriting;    // depois o da subclasse
    }
}
```

::: analogia Construir a casa pelo alicerce
Ao construir um objeto de uma subclasse, o C# **constrói primeiro a parte da base** (o alicerce) e só depois a da subclasse (as paredes e o telhado). Daí a ordem: construtor do pai → construtor do filho.
:::

::: erro "Não há argumentos que correspondam…"
Se a base só tem `Party(int)` e a subclasse não escreve `: base(...)`, o compilador reclama: *"não há nenhum argumento fornecido que corresponda ao parâmetro obrigatório"*. Ele tentou chamar o construtor sem parâmetros do pai — e ele não existe.
:::

::: exercicio Monte a hierarquia
Você precisa modelar **veículos**: `Carro`, `Moto`, `Caminhao`. Todos têm `Placa` e o método `Acelerar()`. Só o caminhão tem `CapacidadeCarga`; só a moto precisa de `Capacete()`.

1. Qual classe base você criaria?
2. O que vai em cada classe?
3. Dê um exemplo de método `virtual`.

--- resposta
1. `Veiculo` (classe base).
2. `Veiculo`: `Placa`, `Acelerar()`. `Caminhao : Veiculo`: `CapacidadeCarga`. `Moto : Veiculo`: `Capacete()`. `Carro : Veiculo` pode estar vazio (ou com `NumeroDePortas`).
3. `Acelerar()` poderia ser `virtual` com implementação padrão e `Moto` poderia sobrescrever com `override` para acelerar mais rápido.
:::

## Projeto: sistema de gestão de uma colmeia

No livro, o projeto deste capítulo é um **sistema de gerenciamento de uma colmeia**, que usa herança para organizar **abelhas operárias** de diferentes funções sob a **rainha**. O desenho conceitual:

::: figura As abelhas da colmeia
<svg viewBox="0 0 700 260" role="img" aria-label="Bee é a base; Queen e Worker derivam">
  <g text-anchor="middle">
    <rect class="f-warn" x="270" y="10" width="160" height="50" rx="12"/><text x="350" y="42" font-weight="800" class="t-warn">Bee</text>
    <path class="line" d="M350 60V85M170 85H530M170 85V110M530 85V110"/>
    <rect class="f-prim" x="90" y="110" width="160" height="50" rx="12"/><text x="170" y="142" font-weight="800" class="t-prim">Queen</text>
    <rect class="f-good" x="450" y="110" width="160" height="50" rx="12"/><text x="530" y="142" font-weight="800" class="t-good">Worker</text>
  </g>
  <g font-size="13" class="t-dim"><text x="20" y="195">Bee: Weight, Cost, método virtual Consumption()</text><text x="20" y="218">Queen: gerencia as operárias e distribui as tarefas</text><text x="20" y="241">Worker: DoJob(), WorkTheNextShift()</text></g>
</svg>
:::

```cs
class Bee
{
    public virtual float CostPerShift { get; }
    public string Job { get; private set; }
    public Bee(string job) { Job = job; }
}

class Worker : Bee
{
    private string[] jobsICanDo;
    private int shiftsToWork;
    private int shiftsWorked;

    public Worker(string[] jobsICanDo) : base("") { this.jobsICanDo = jobsICanDo; }
    public int ShiftsLeft { get { return shiftsToWork - shiftsWorked; } }

    public bool DoThisJob(string job, int numberOfShifts)
    {
        if (!string.IsNullOrEmpty(Job)) return false;         // já está ocupada
        foreach (string possible in jobsICanDo)
            if (possible == job) { /* assume o trabalho */ shiftsToWork = numberOfShifts; shiftsWorked = 0; return true; }
        return false;
    }

    public bool WorkOneShift()
    {
        if (string.IsNullOrEmpty(Job)) return false;
        shiftsWorked++;
        return shiftsWorked >= shiftsToWork;                   // terminou?
    }
}
```

::: pense Por que `Queen` e `Worker` herdam de `Bee`?
Porque **ambas são abelhas**: compartilham, por exemplo, peso e custo de manutenção. O que as diferencia (a rainha comanda; a operária trabalha) vai na própria subclasse.
:::

::: resumo
- **Herança**: a subclasse reutiliza campos, propriedades e métodos da base (`class Filha : Base`).
- Modele do **geral ao específico**; use a relação **"é um"**.
- `virtual` (base) + `override` (filha) permitem **substituir** comportamento.
- `base.Metodo()` chama a versão do pai; `: base(...)` chama o construtor do pai.
- **Onde cabe a base, cabe a subclasse**: uma variável `Animal` pode apontar para um `Wolf`.
- Eliminar duplicação em subclasses = menos bugs e manutenção mais simples.
:::

::: quiz
? Como declarar que `Cachorro` herda de `Animal`?
- `class Cachorro extends Animal`
+ `class Cachorro : Animal`
- `class Cachorro inherits Animal`
- `class Cachorro(Animal)`
> Em C#, a herança usa dois-pontos.

? Para que um método da base possa ser substituído na subclasse, ele deve ser…
- `static`
- `private`
+ `virtual` (ou `abstract`)
- `readonly`
> A subclasse usa `override` para substituir um método `virtual`.

? O que `base.MakeNoise()` faz dentro de uma subclasse?
+ Chama a versão do método na classe base
- Cria um novo objeto da base
- Apaga o método da base
- Chama o método de outra subclasse
> `base` é a referência à classe pai.

? Qual é a relação que a herança modela?
- "tem um"
+ "é um"
- "usa um"
- "conhece um"
> Um lobo é um canino; um canino é um animal.

? `Animal a = new Wolf(); a.MakeNoise();` executa…
- Sempre a versão de `Animal`
+ A versão sobrescrita em `Wolf` (se houver override)
- Gera erro de compilação
- Chama os dois métodos
> A versão executada depende do tipo do objeto real.

? Se a classe base só tem `Party(int pessoas)`, o que a subclasse precisa fazer?
- Nada, o C# resolve sozinho
+ Chamar esse construtor com `: base(pessoas)`
- Declarar o campo como `static`
- Remover o construtor da base
> Sem construtor padrão na base, a subclasse deve escolher qual chamar.

? Qual é a ordem de execução dos construtores em `new BirthdayParty(...)`?
+ Primeiro o da classe base, depois o da subclasse
- Primeiro o da subclasse, depois o da base
- Só o da subclasse
- Ordem aleatória
> A parte base do objeto é construída antes da derivada.

? Qual o principal benefício de colocar código comum na classe base?
- O executável fica menor, só isso
+ Escrever a lógica uma vez e corrigir bugs em um só lugar
- Proibir que subclasses sejam criadas
- Impedir o uso de `override`
> Evita duplicação e inconsistência.
:::

::: cartas
Herança :: Mecanismo em que uma subclasse reutiliza e estende os membros de uma classe base.
Classe base (superclasse) :: Classe da qual outras herdam.
Subclasse (derivada) :: Classe que herda de uma base: `class B : A`.
"é um" (is-a) :: Relação modelada pela herança: lobo é um canino.
virtual :: Marca um método da base como substituível.
override :: Na subclasse, substitui um método virtual herdado.
base :: Referência à classe pai: `base.Metodo()` ou `: base(args)` no construtor.
Polimorfismo :: Mesma chamada, comportamentos diferentes conforme o tipo real do objeto.
Upcast implícito :: Usar uma subclasse onde a base é esperada (sem cast).
Duplicação de código :: Mesmo trecho em vários lugares: fonte clássica de bugs.
:::
