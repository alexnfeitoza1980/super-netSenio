---
id: aprenda-cap03
order: 13
kind: capitulo
num: 3
group: C# · Parte 1 · Primeiros passos
title: Objetos, orientem-se!
subtitle: Fazendo o código ter sentido — classes, instâncias, campos, métodos e diagramas de classe.
minutes: 55
hue: 262
objectives:
- Pensar no **problema** primeiro e deixar o código **espelhar** esse problema
- Entender **classe** (a planta) × **instância/objeto** (a casa construída)
- Usar `new`, **campos** e **métodos**; saber quando algo deve ser `static`
- Desenhar um **diagrama de classe** e usar **inicializadores de objeto**
origem: livros/01-aprenda-csharp/content/03-cap03.md
---

::: ilustracao img/aprenda/cap03.svg | Classe é a planta; objetos são as casas construídas, cada uma com seus próprios valores.
:::

## Fazendo o código ter sentido

Todo programa resolve um problema. Por isso, antes de escrever código, convém perguntar: **qual problema isto resolve?** Os **objetos** existem para que você organize o código *segundo o problema*, e gaste a cabeça pensando no problema — não na mecânica de escrever código. Bem usados, os objetos geram código fácil de escrever, de ler e de mudar.

::: vila Miguel e o GPS do carro
Miguel é um programador a caminho da entrevista de emprego. Ele escolhe uma rota, ouve no rádio que está congestionada e precisa **desviar** para outra, ainda chegando a tempo. O sistema de navegação do carro dele pensa assim: *"tenho um destino, uma rota atual, posso modificá-la para evitar uma rua ou para incluir outra, e consigo dizer a distância total"*.
:::

Repare que o sistema do carro agrupa **tudo sobre rotas** num lugar só. Em código, esse lugar é uma **classe** `Navigator`, com métodos como:

```cs
class Navigator
{
    public void SetDestination(string destinationName) { /* ... */ }
    public void ModifyRouteToAvoid(string streetName) { /* ... */ }
    public void ModifyRouteToInclude(string streetName) { /* ... */ }
    public string GetRoute() { /* ... */ return ""; }
    public int TotalDistance() { /* ... */ return 0; }
}
```

Diferente do `button1_Click`, que faz só uma coisa, os métodos aqui giram em torno de **um mesmo assunto**: criar e alterar rotas. É isso que torna a classe coesa e o código intuitivo.

## Uma classe é uma planta; objetos são as casas

Suponha que Miguel queira **comparar três rotas** para o mesmo destino. Ele poderia copiar e colar a classe `Navigator` três vezes... e manter três cópias do mesmo código. Péssima ideia. A saída é usar **objetos**.

::: analogia A planta de uma casa
Uma **classe** é como a **planta** de uma casa: define onde ficam as paredes e os cômodos. Para um condomínio com cinco casas iguais você **não pede cinco plantas** ao arquiteto: usa **uma** planta para construir **cinco casas**. Cada casa é uma **instância** (um **objeto**) — com seu próprio endereço, seus próprios moradores e sua própria cor de parede.
:::

```cs
Navigator navigator1 = new Navigator();   // 1ª casa construída a partir da planta
Navigator navigator2 = new Navigator();   // 2ª casa
Navigator navigator3 = new Navigator();   // 3ª casa
```

O comando **`new`** cria uma **instância** da classe. Cada instância tem os mesmos **métodos**, mas **dados próprios**. Você já usava isso sem saber: cada item da Caixa de Ferramentas é uma classe — `Button`, `TextBox`, `Label`. Quando você arrasta um botão, a IDE faz `button1 = new Button()`; arrasta outro, ganha `button2`. Dois botões, comportamento igual, texto e posição diferentes.

::: pense Procure o `new` na sua IDE
Abra um formulário com um botão e use **Localizar** ([[Ctrl]]+[[Shift]]+[[F]]) por `new System.Windows.Forms.Button` em todo o projeto. Você encontrará, no arquivo do designer, o `new` que a IDE escreveu por você.
:::

### Uma solução melhor com objetos

Com a classe pronta, o formulário de Miguel (três caixas de texto: destino, rua a evitar, rua a incluir) fica assim:

```cs
string destination = textBox1.Text;
string route2StreetToAvoid = textBox2.Text;
string route3StreetToInclude = textBox3.Text;

Navigator navigator1 = new Navigator();
navigator1.SetDestination(destination);
int distance1 = navigator1.TotalDistance();

Navigator navigator2 = new Navigator();
navigator2.SetDestination(destination);
navigator2.ModifyRouteToAvoid(route2StreetToAvoid);
int distance2 = navigator2.TotalDistance();

Navigator navigator3 = new Navigator();
navigator3.SetDestination(destination);
navigator3.ModifyRouteToInclude(route3StreetToInclude);
int distance3 = navigator3.TotalDistance();

int shortestDistance = Math.Min(distance1, Math.Min(distance2, distance3));
```

`Math.Min` compara dois números e devolve o menor. A classe foi escrita **uma vez** e usada **três vezes**.

::: regra Um objeto lembra de si mesmo
Quando `navigator2` recebe `ModifyRouteToAvoid`, **só a rota guardada naquele objeto** muda. `navigator1` e `navigator3` continuam intactos. É assim que o programa acompanha três rotas ao mesmo tempo.
:::

## Métodos estáticos × métodos de instância

Você já escreveu métodos `static`, como `Talker.BlahBlahBlah(...)`, e chamou sem usar `new`. A palavra-chave **`static`** significa *"pertence à classe, não a uma instância"*.

| | Método **estático** | Método de **instância** |
|---|---|---|
| Declaração | `public static int Somar(int a, int b)` | `public int TotalDistance()` |
| Como chamar | `Calculadora.Somar(2, 3)` | `navigator1.TotalDistance()` |
| Precisa de `new`? | **Não** | **Sim** |
| Tem dados próprios? | Não (usa só parâmetros) | **Sim** (usa os campos do objeto) |

::: dica Quando usar static?
Use `static` para operações **sem estado**, que só dependem dos parâmetros (como `Math.Min`). Use instâncias quando o objeto precisa **lembrar** algo. Se você marcar a classe inteira como `static`, **todos** os membros precisam ser estáticos.
:::

### Instâncias usam campos para guardar o que sabem

Os **métodos** definem o que o objeto **faz**. Os **campos** são o que ele **sabe**: variáveis declaradas dentro da classe, fora de qualquer método. Cada instância tem sua cópia.

```cs
class Navigator
{
    public string Destination;   // campo: cada Navigator tem o seu
    public int Distance;         // outro campo

    public void SetDestination(string destinationName)
    {
        Destination = destinationName;   // usa o campo do OBJETO atual
    }
}
```

Quando você mudou o texto de um botão, a IDE escreveu `this.button1.Text = "OK"` — você alterou o **campo** `Text` daquela instância. Lembre-se: para ter dados próprios por objeto, **não** use `static`.

## O que acontece na memória

Quando seu código usa `new`, o C# reserva um pedaço da memória chamado **heap** (a "pilha dinâmica" — não confunda com a *stack*) e coloca ali os dados do objeto. A variável guarda uma **referência** (como um endereço) para esse pedaço.

::: passos Três Navigators na memória
Código: `Navigator navigator1 = new Navigator();` e depois `navigator1.SetDestination("Aeroporto");`

<br>

::: html
<svg viewBox="0 0 600 190"><rect class="f-prim" x="20" y="30" width="170" height="52" rx="10"/><text x="105" y="62" text-anchor="middle" class="t-mono" font-size="14">navigator1</text><text x="105" y="20" text-anchor="middle" font-size="12" class="t-dim">variável</text><text x="400" y="20" text-anchor="middle" font-size="12" class="t-dim">heap</text><rect class="f-plain" stroke-dasharray="6 4" x="270" y="30" width="310" height="140" rx="12"/><text x="425" y="100" text-anchor="middle" class="t-dim" font-size="14">(vazio por enquanto)</text></svg>
:::
===
**`new Navigator()`** reserva espaço no heap para o objeto, com os campos vazios (`Destination = null`).

::: html
<svg viewBox="0 0 600 190"><rect class="f-prim" x="20" y="30" width="170" height="52" rx="10"/><text x="105" y="62" text-anchor="middle" class="t-mono" font-size="14">navigator1</text><rect class="f-plain" stroke-dasharray="6 4" x="270" y="30" width="310" height="140" rx="12"/><rect class="f-info" x="300" y="55" width="250" height="95" rx="10"/><text x="425" y="80" text-anchor="middle" font-weight="800" class="t-info" font-size="14">objeto Navigator</text><text x="320" y="106" class="t-mono" font-size="13">Destination = null</text><text x="320" y="128" class="t-mono" font-size="13">Distance = 0</text><path class="arrow" d="M192 56C230 56 260 90 296 96"/></svg>
:::
===
**`SetDestination("Aeroporto")`** altera o **campo** do objeto apontado por `navigator1`.

::: html
<svg viewBox="0 0 600 190"><rect class="f-prim" x="20" y="30" width="170" height="52" rx="10"/><text x="105" y="62" text-anchor="middle" class="t-mono" font-size="14">navigator1</text><rect class="f-plain" stroke-dasharray="6 4" x="270" y="30" width="310" height="140" rx="12"/><rect class="f-info" x="300" y="55" width="250" height="95" rx="10"/><text x="425" y="80" text-anchor="middle" font-weight="800" class="t-info" font-size="14">objeto Navigator</text><text x="320" y="106" class="t-mono" font-size="13">Destination = "Aeroporto"</text><text x="320" y="128" class="t-mono" font-size="13">Distance = 0</text><path class="arrow" d="M192 56C230 56 260 90 296 96"/></svg>
:::
===
Com **três** `new Navigator()`, existem **três objetos independentes** no heap, cada um com seus próprios valores. As variáveis `navigator1`, `navigator2` e `navigator3` apontam para um objeto cada.

::: html
<svg viewBox="0 0 600 250"><g font-size="13"><rect class="f-prim" x="10" y="20" width="140" height="42" rx="9"/><text x="80" y="46" text-anchor="middle" class="t-mono">navigator1</text><rect class="f-prim" x="10" y="100" width="140" height="42" rx="9"/><text x="80" y="126" text-anchor="middle" class="t-mono">navigator2</text><rect class="f-prim" x="10" y="180" width="140" height="42" rx="9"/><text x="80" y="206" text-anchor="middle" class="t-mono">navigator3</text></g><rect class="f-info" x="300" y="10" width="290" height="62" rx="9"/><text x="315" y="35" class="t-mono" font-size="12">Destination = "Aeroporto"</text><text x="315" y="55" class="t-mono" font-size="12">rota original</text><rect class="f-warn" x="300" y="90" width="290" height="62" rx="9"/><text x="315" y="115" class="t-mono" font-size="12">Destination = "Aeroporto"</text><text x="315" y="135" class="t-mono" font-size="12">evita: Av. Brasil</text><rect class="f-good" x="300" y="170" width="290" height="62" rx="9"/><text x="315" y="195" class="t-mono" font-size="12">Destination = "Aeroporto"</text><text x="315" y="215" class="t-mono" font-size="12">inclui: Rua da Ponte</text><path class="arrow" d="M152 41C220 41 240 41 296 41"/><path class="arrow" d="M152 121C220 121 240 121 296 121"/><path class="arrow" d="M152 201C220 201 240 201 296 201"/></svg>
:::
:::

::: cuidado Heap e *stack* são coisas diferentes
Neste livro, o "heap" é onde ficam os **objetos**. Variáveis locais simples (como `int`) ficam em outra região (a *stack*). Veremos a diferença em detalhes no capítulo 4 e no 14.
:::

## Dê nomes que façam sentido

Código bom se lê como uma frase do problema. Compare duas versões de um sistema de fábrica de doces:

```cs
// Difícil de entender
T t = new T();
if (t.Temp > 80) {
    CCS c = new CCS();
    c.F();
    c.R();
}
```

```cs
// Intuitivo
Turbine turbineController = new Turbine();
if (turbineController.CreamTemperature > 80) {
    CandyCoolingSystem cooling = new CandyCoolingSystem();
    cooling.FillSystem();
    cooling.ReleaseContents();
}
```

Mesmo sem conhecer o sistema, a segunda versão explica o que acontece quando o creme está quente demais.

::: regra Código bom espelha o problema
Escolha nomes de **classes** e **métodos** que façam sentido para alguém que entende o *problema*, não só o código. Quanto mais seu código se parecer com o problema, mais fácil será escrever, ler e corrigir.
:::

### Dê às classes uma estrutura natural

Perguntas para organizar suas classes:

1. **O que a classe representa?** (um cliente, uma rota, uma turbina)
2. **O que ela sabe?** → **campos**
3. **O que ela faz?** → **métodos**
4. Os nomes soam como o vocabulário do problema?

## Diagramas de classe

Um **diagrama de classe** é um desenho simples, em três faixas, que ajuda a pensar antes de codificar:

::: figura Diagrama de classe de Navigator
<svg viewBox="0 0 560 300" role="img" aria-label="Diagrama de classe Navigator">
  <rect class="f-plain" x="100" y="10" width="360" height="280" rx="8"/>
  <rect class="f-prim" x="100" y="10" width="360" height="50" rx="8"/><text x="280" y="42" text-anchor="middle" font-weight="800" font-size="19" class="t-prim">Navigator</text>
  <path class="line" d="M100 60H460"/>
  <text x="116" y="88" class="t-dim" font-size="12">CAMPOS — o que o objeto SABE</text>
  <text class="t-mono" x="116" y="112" font-size="14">Destination : string</text>
  <text class="t-mono" x="116" y="134" font-size="14">CurrentRoute : string</text>
  <path class="line" d="M100 154H460"/>
  <text x="116" y="180" class="t-dim" font-size="12">MÉTODOS — o que o objeto FAZ</text>
  <text class="t-mono" x="116" y="204" font-size="14">SetDestination(destinationName)</text>
  <text class="t-mono" x="116" y="226" font-size="14">ModifyRouteToAvoid(streetName)</text>
  <text class="t-mono" x="116" y="248" font-size="14">ModifyRouteToInclude(streetName)</text>
  <text class="t-mono" x="116" y="270" font-size="14">GetRoute() / TotalDistance()</text>
</svg>
:::

Comece **pelo diagrama**, não pelo código: rascunhe em papel os campos e métodos; só depois escreva a classe. Alterar um desenho é muito mais barato do que alterar um sistema.

::: exercicio Qual é o problema de design de cada classe?
Uma classe `CaixaRegistradora` de uma loja de conveniência tem estes membros: `ValorTotal`, `Cliente`, `RaioDoCaminhao`, `AdicionarItem()`, `LigarForno()`.

1. Quais membros não pertencem a ela?
2. Para onde você os levaria?

--- resposta
1. `RaioDoCaminhao` e `LigarForno()` não têm relação com cobrança — misturam assuntos distintos. `Cliente` talvez devesse ser uma classe própria.
2. Crie classes coesas: `CaixaRegistradora` (valor total, adicionar item, fechar compra), `Cliente` (nome, histórico) e algo como `Forno` e `Caminhao` em seus próprios contextos. Uma classe deve ter **um assunto**.
:::

## Desenvolva uma classe: o jogo de apostas dos caras

::: vila Joe e Bob
Joe e Bob se encontram toda semana para apostar. Joe começa com R$ 100 e Bob com R$ 50. Há também um **banco** com R$ 200 para todos. Precisamos de um programa para registrar quem deu e quem recebeu dinheiro, sem trapaça.
:::

Pelo diagrama, cada "cara" **sabe** seu nome e seu dinheiro; e **faz** duas coisas: dar dinheiro e receber dinheiro.

```cs Guy.cs
class Guy
{
    public string Name;      // o que ele sabe
    public int Cash;

    public int GiveCash(int amount)
    {
        if (amount <= Cash && amount > 0)
        {
            Cash -= amount;
            return amount;
        }
        else
        {
            MessageBox.Show("Não tenho dinheiro para dar " + amount, Name + " diz...");
            return 0;
        }
    }

    public int ReceiveCash(int amount)
    {
        if (amount > 0)
        {
            Cash += amount;
            return amount;
        }
        else
        {
            MessageBox.Show(amount + " não é uma quantia que eu aceite", Name + " diz...");
            return 0;
        }
    }
}
```

No formulário (campos do próprio `Form1`, criados uma só vez):

```cs Form1.cs
Guy joe;
Guy bob;
int bank = 200;

private void Form1_Load(object sender, EventArgs e)
{
    joe = new Guy();
    joe.Name = "Joe";
    joe.Cash = 100;
    bob = new Guy();
    bob.Name = "Bob";
    bob.Cash = 50;
}

private void buttonBankToJoe_Click(object sender, EventArgs e)   // "Dar R$ 10 ao Joe"
{
    if (bank >= 10)
    {
        bank -= joe.ReceiveCash(10);
    }
}

private void buttonJoeToBob_Click(object sender, EventArgs e)     // "Joe dá R$ 10 ao Bob"
{
    bob.ReceiveCash(joe.GiveCash(10));
}
```

::: dica O truque do `bob.ReceiveCash(joe.GiveCash(10))`
Um método pode servir de **argumento** para outro: `joe.GiveCash(10)` devolve o valor entregue (ou 0 se Joe não tiver) e esse número vai direto para `bob.ReceiveCash`. Troca de dinheiro numa linha, sem criar dinheiro do nada.
:::

::: erro Esquecer o `new`
Se você declarar `Guy joe;` e chamar `joe.GiveCash(10)` sem antes fazer `joe = new Guy()`, o programa quebra com uma `NullReferenceException`: a variável existe, mas **não aponta para objeto algum**. Falaremos disso no capítulo 4.
:::

## Existe uma forma ainda mais fácil de inicializar objetos

Atribuir campo por campo depois do `new` é verboso. O C# oferece o **inicializador de objeto**:

```cs
joe = new Guy() { Name = "Joe", Cash = 100 };
bob = new Guy() { Name = "Bob", Cash = 50 };
```

É o mesmo efeito, em uma linha. Funciona com qualquer campo ou propriedade `public`.

::: pense Seu objeto pode ser criado "pela metade"?
O que impede alguém de criar `new Guy()` e esquecer o nome? Nada — por enquanto. No **capítulo 5** você aprenderá a usar **construtores** e **encapsulamento** para exigir que o objeto nasça completo e válido.
:::

::: resumo
- Comece pelo **problema**; a classe deve espelhar o vocabulário dele.
- **Classe** = planta; **instância/objeto** = coisa criada com `new`.
- **Campos** guardam o que o objeto *sabe*; **métodos** definem o que ele *faz*.
- Métodos `static` pertencem à classe (sem `new`); métodos normais pertencem à instância.
- Objetos vivem no **heap**; a variável guarda uma referência.
- **Diagramas de classe** (nome / campos / métodos) ajudam a projetar antes de codificar.
- **Inicializadores** `new Guy() { Name = "Joe" }` simplificam a criação.
:::

::: quiz
? O que é uma **instância** de uma classe?
- O arquivo `.cs` que contém a classe
+ Um objeto criado a partir da classe com `new`
- Um método estático
- Um tipo de variável local
> A classe é a planta; cada objeto criado com `new` é uma instância dela.

? Qual palavra-chave cria um novo objeto?
- `create`
- `static`
+ `new`
- `class`
> `new Navigator()` reserva memória no heap e executa a inicialização do objeto.

? Onde ficam os dados *individuais* de cada objeto?
- Nos métodos estáticos
+ Nos campos da instância
- No namespace
- Na janela Propriedades
> Cada instância tem sua cópia dos campos; por isso `navigator1` e `navigator2` guardam rotas diferentes.

? Qual a diferença principal entre método estático e de instância?
- O estático é mais rápido e o de instância mais lento
+ O estático é chamado pela classe, sem `new`; o de instância precisa de um objeto
- O estático não pode ter parâmetros
- O de instância não pode retornar valor
> `Talker.BlahBlahBlah()` (estático) × `navigator1.TotalDistance()` (instância).

? Em um diagrama de classe, o que fica na faixa do meio?
- Os métodos
+ Os campos (o que o objeto sabe)
- O namespace
- Os comentários
> Nome em cima, campos no meio, métodos embaixo.

? O que significa `bob.ReceiveCash(joe.GiveCash(10))`?
- Bob dá 10 para Joe
+ Joe entrega até 10 (ou 0), e Bob recebe esse valor
- É um erro de sintaxe
- Cria dois objetos novos
> O valor de retorno de `GiveCash` é usado como argumento de `ReceiveCash`.

? Por que escolher nomes intuitivos para classes e métodos?
+ Porque o código passa a espelhar o problema e fica mais fácil de ler e manter
- Porque o compilador exige
- Para o programa rodar mais rápido
- Para reduzir o tamanho do executável
> Nomes bons funcionam como documentação.

? O que faz o inicializador `new Guy() { Name = "Joe", Cash = 100 }`?
+ Cria o objeto e já atribui valores aos campos informados
- Cria dois objetos
- Torna a classe estática
- Apaga os valores anteriores de todos os Guy
> É açúcar sintático para `new` seguido de várias atribuições.
:::

::: cartas
Classe :: Planta que define campos e métodos de um tipo de objeto.
Objeto / instância :: Coisa concreta criada a partir de uma classe com `new`.
new :: Operador que cria uma instância e devolve uma referência a ela.
Campo :: Variável declarada na classe; guarda o que cada objeto sabe.
Método :: O que o objeto faz; pode receber parâmetros e retornar valor.
static :: Membro que pertence à classe e não a uma instância; chamado sem `new`.
Heap (pilha dinâmica) :: Região da memória onde os objetos criados com `new` ficam.
Diagrama de classe :: Desenho em três faixas: nome, campos e métodos.
Coesão :: Uma classe deve tratar de um assunto só.
Inicializador de objeto :: Sintaxe `new T() { Campo = valor }` para criar e preencher de uma vez.
this :: Referência ao próprio objeto em que o código está executando.
:::
