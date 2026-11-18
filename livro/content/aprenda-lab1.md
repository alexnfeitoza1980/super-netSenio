---
id: aprenda-lab1
order: 15
kind: lab
num: 1
group: C# · Parte 2 · Dados e referências
title: Laboratório 1 — Um dia de corridas
subtitle: Seu primeiro projeto de verdade: um simulador de corrida de galgos com apostas, objetos que conversam e um formulário completo.
minutes: 90
hue: 205
badge: L1
objectives:
- Juntar tudo dos capítulos 1 a 4 em um **programa completo**
- Modelar o problema com **três classes** (`Greyhound`, `Guy`, `Bet`) antes de escrever código
- Compartilhar **uma única** referência `Random` entre vários objetos
- Usar **matrizes** de objetos e um **Timer** para animar a corrida
origem: livros/01-aprenda-csharp/content/05-lab1.md
---

::: vila Um negócio arriscado
Joe, Bob e Al adoram apostar em corridas de galgos — e estão cansados de perder dinheiro. Querem que **você** construa um simulador para descobrir os vencedores *antes* de apostar de verdade. Se o trabalho ficar bom, dividem os lucros com você. 🐕💨
:::

::: ilustracao img/aprenda/lab1.svg | Um dia de corridas: objetos com nome, estado e comportamento correndo na pista.
:::

Os laboratórios são projetos maiores, com **menos mão na mão**: você recebe o problema, um plano e dicas — e precisa **pensar**. Faça **você mesmo** antes de abrir as seções de resposta; é aqui que o aprendizado se consolida.

## A especificação

O simulador é um aplicativo Windows Forms com:

- Uma **pista** com **quatro galgos** (um `PictureBox` cada) que correm da esquerda para a direita.
- Três **apostadores** (Joe, Bob e Al), cada um com uma quantia em dinheiro (por exemplo, R$ 50, R$ 75 e R$ 45).
- Para cada um: um `RadioButton` para escolher "quem vai apostar", e um `Label` que mostra o que ele apostou ("Joe não fez aposta").
- Um `NumericUpDown` para a **quantia** e outro para o **número do galgo** (1 a 4), e um botão **Apostar**.
- Um botão **Correr!** que larga os galgos. Quando um cruzar a linha de chegada, o programa **mostra o vencedor** e **paga** quem apostou nele.
- Após a corrida, os galgos voltam à largada e as apostas são zeradas.

::: figura Esboço da tela
<svg viewBox="0 0 700 330" role="img" aria-label="Esboço do formulário de corrida">
  <rect class="f-plain" x="10" y="10" width="680" height="310" rx="12"/>
  <rect class="f-prim" x="25" y="25" width="650" height="170" rx="10"/>
  <g class="t-mono" font-size="12"><text x="40" y="60">🐕  galgo 1 ─────────────────────────▶</text><text x="40" y="92">🐕  galgo 2 ──────────────────▶</text><text x="40" y="124">🐕  galgo 3 ───────────────────────▶</text><text x="40" y="156">🐕  galgo 4 ───────────────▶</text></g>
  <text x="640" y="180" text-anchor="end" font-size="12" class="t-dim">linha de chegada ┃</text>
  <rect class="f-info" x="25" y="210" width="310" height="100" rx="10"/>
  <text x="40" y="236" font-weight="800" class="t-info" font-size="14">Apostas</text>
  <text x="40" y="260" font-size="13">◉ Joe   fez aposta de R$ 10 no galgo 3</text><text x="40" y="282" font-size="13">○ Bob   não fez aposta</text><text x="40" y="302" font-size="13">○ Al    não fez aposta</text>
  <rect class="f-good" x="355" y="210" width="320" height="100" rx="10"/>
  <text x="370" y="236" font-weight="800" class="t-good" font-size="14">Controles</text>
  <text x="370" y="262" font-size="13">Quantia: [ 10 ]   Galgo nº: [ 3 ]</text><text x="370" y="292" font-size="13">[ Apostar ]       [ Correr! ]</text>
</svg>
:::

## Planejando: três classes, três responsabilidades

Antes do código, o **desenho**. Quais são as "coisas" do problema? E o que cada uma **sabe** e **faz**?

::: figura Diagrama das classes
<svg viewBox="0 0 760 330" role="img" aria-label="Classes Greyhound, Guy e Bet">
  <g>
    <rect class="f-plain" x="10" y="20" width="230" height="290" rx="8"/><rect class="f-info" x="10" y="20" width="230" height="40" rx="8"/><text x="125" y="47" text-anchor="middle" font-weight="800" class="t-info">Greyhound</text>
    <g class="t-mono" font-size="12"><text x="22" y="86">StartingPosition</text><text x="22" y="106">RacetrackLength</text><text x="22" y="126">MyPictureBox</text><text x="22" y="146">Location</text><text x="22" y="166">MyRandom</text></g>
    <path class="line" d="M10 180H240"/>
    <g class="t-mono" font-size="12"><text x="22" y="206">Run() : bool</text><text x="22" y="228">TakeStartingPosition()</text></g>
  </g>
  <g>
    <rect class="f-plain" x="265" y="20" width="230" height="290" rx="8"/><rect class="f-good" x="265" y="20" width="230" height="40" rx="8"/><text x="380" y="47" text-anchor="middle" font-weight="800" class="t-good">Guy</text>
    <g class="t-mono" font-size="12"><text x="277" y="86">Name</text><text x="277" y="106">MyBet : Bet</text><text x="277" y="126">Cash</text><text x="277" y="146">MyRadioButton</text><text x="277" y="166">MyLabel</text></g>
    <path class="line" d="M265 180H495"/>
    <g class="t-mono" font-size="12"><text x="277" y="206">UpdateLabels()</text><text x="277" y="226">PlaceBet(amount, dog)</text><text x="277" y="246">ClearBet()</text><text x="277" y="266">Collect(winner)</text></g>
  </g>
  <g>
    <rect class="f-plain" x="520" y="20" width="230" height="290" rx="8"/><rect class="f-warn" x="520" y="20" width="230" height="40" rx="8"/><text x="635" y="47" text-anchor="middle" font-weight="800" class="t-warn">Bet</text>
    <g class="t-mono" font-size="12"><text x="532" y="86">Amount</text><text x="532" y="106">Dog</text><text x="532" y="126">Bettor : Guy</text></g>
    <path class="line" d="M520 180H750"/>
    <g class="t-mono" font-size="12"><text x="532" y="206">GetDescription() : string</text><text x="532" y="226">PayOut(winner) : int</text></g>
  </g>
</svg>
:::

| Classe | Responsabilidade |
|---|---|
| `Greyhound` | Sabe sua posição e a pista; `Run()` avança uma quantidade **aleatória** e diz se **cruzou a linha**. |
| `Guy` | Sabe quanto dinheiro tem e qual aposta fez; atualiza o próprio `Label` e o `RadioButton`. |
| `Bet` | Sabe **quanto**, em **qual galgo** e **quem** apostou; calcula o pagamento. |

::: pense Quem deve atualizar o rótulo?
Poderia ser o formulário... ou o próprio `Guy`. Se o `Guy` guarda uma referência ao seu `Label`, ele mesmo escreve "Joe tem R$ 40". O formulário fica livre de detalhes, e o objeto **cuida de si**. Esse é um bom exemplo de **encapsulamento**, que você estudará mais a fundo no capítulo 5.
:::

## Passo a passo

### Etapa 1 — O galgo

```cs Greyhound.cs
class Greyhound
{
    public int StartingPosition;          // onde o PictureBox começa
    public int RacetrackLength;           // onde fica a linha de chegada
    public PictureBox MyPictureBox = null;
    public int Location = 0;
    public Random MyRandom;               // UMA instância compartilhada

    public bool Run()
    {
        int passos = MyRandom.Next(1, 5);                 // avança de 1 a 4
        Location += passos;
        MyPictureBox.Left = StartingPosition + Location;  // move a imagem
        return MyPictureBox.Left >= RacetrackLength;      // cruzou?
    }

    public void TakeStartingPosition()
    {
        Location = 0;
        MyPictureBox.Left = StartingPosition;
    }
}
```

::: cuidado Um único `Random` para todos
Se cada galgo criar seu próprio `new Random()` na mesma fração de segundo, todos começam com a **mesma semente** e se movem **exatamente igual** — nenhuma corrida! Crie **um** `Random` no formulário e entregue a referência a cada galgo (`MyRandom = random`). É uma ótima aplicação das referências do capítulo 4: vários objetos, um gerador.
:::

### Etapa 2 — A aposta e o apostador

```cs Bet.cs
class Bet
{
    public int Amount;     // quantia apostada
    public int Dog;        // número do galgo (1 a 4)
    public Guy Bettor;     // quem apostou

    public string GetDescription()
    {
        if (Amount > 0)
            return Bettor.Name + " apostou R$ " + Amount + " no galgo #" + Dog;
        return Bettor.Name + " não fez aposta";
    }

    public int PayOut(int winner)
    {
        return (Dog == winner) ? Amount : -Amount;   // ganha ou perde o que apostou
    }
}
```

```cs Guy.cs
class Guy
{
    public string Name;
    public Bet MyBet;
    public int Cash;
    public RadioButton MyRadioButton;
    public Label MyLabel;

    public void UpdateLabels()
    {
        MyLabel.Text = MyBet == null || MyBet.Amount == 0 ? Name + " não fez aposta" : MyBet.GetDescription();
        MyRadioButton.Text = Name + " tem R$ " + Cash;
    }

    public void ClearBet() { MyBet = null; }

    public bool PlaceBet(int amount, int dog)
    {
        if (amount > 0 && amount <= Cash)
        {
            MyBet = new Bet() { Amount = amount, Dog = dog, Bettor = this };
            UpdateLabels();
            return true;
        }
        return false;       // sem dinheiro suficiente
    }

    public void Collect(int winner)
    {
        if (MyBet != null) Cash += MyBet.PayOut(winner);
        ClearBet();
        UpdateLabels();
    }
}
```

::: dica `this` — o próprio objeto
Em `Bettor = this` o `Guy` entrega **a si mesmo** à aposta. `this` é uma referência ao objeto em que o código está executando.
:::

### Etapa 3 — O formulário

```cs Form1.cs
Greyhound[] dogs = new Greyhound[4];
Guy[] guys = new Guy[3];
Random random = new Random();

private void Form1_Load(object sender, EventArgs e)
{
    PictureBox[] boxes = { pictureBox1, pictureBox2, pictureBox3, pictureBox4 };
    for (int i = 0; i < dogs.Length; i++)
    {
        dogs[i] = new Greyhound()
        {
            StartingPosition = boxes[i].Left,
            RacetrackLength = pistaPictureBox.Width - boxes[i].Width,
            MyPictureBox = boxes[i],
            MyRandom = random
        };
    }

    guys[0] = new Guy() { Name = "Joe", Cash = 50, MyRadioButton = joeRadio, MyLabel = joeLabel };
    guys[1] = new Guy() { Name = "Bob", Cash = 75, MyRadioButton = bobRadio, MyLabel = bobLabel };
    guys[2] = new Guy() { Name = "Al",  Cash = 45, MyRadioButton = alRadio,  MyLabel = alLabel };
    foreach (Guy g in guys) g.UpdateLabels();
}
```

### Etapa 4 — Apostar e correr

```cs Form1.cs (continuação)
private void betButton_Click(object sender, EventArgs e)
{
    Guy escolhido = joeRadio.Checked ? guys[0] : bobRadio.Checked ? guys[1] : guys[2];
    if (!escolhido.PlaceBet((int)amountNumeric.Value, (int)dogNumeric.Value))
        MessageBox.Show(escolhido.Name + " não tem dinheiro suficiente.");
}

private void raceButton_Click(object sender, EventArgs e)
{
    raceButton.Enabled = false;
    timer1.Interval = 20;
    timer1.Start();
}

private void timer1_Tick(object sender, EventArgs e)
{
    for (int i = 0; i < dogs.Length; i++)
    {
        if (dogs[i].Run())              // alguém venceu?
        {
            timer1.Stop();
            int winner = i + 1;
            MessageBox.Show("O galgo #" + winner + " venceu!");
            foreach (Guy g in guys) g.Collect(winner);
            foreach (Greyhound d in dogs) d.TakeStartingPosition();
            raceButton.Enabled = true;
            return;
        }
    }
}
```

::: ide Um `Timer` em vez de um laço com `DoEvents`
Arraste um componente **Timer** da Caixa de Ferramentas. A cada `Interval` milissegundos ele dispara o evento `Tick`, e a interface continua respondendo. É mais limpo do que repetir `Application.DoEvents()` dentro de um laço (você usou isso no capítulo 2, e verá `Timer` com mais calma no capítulo 12).
:::

## Autoavaliação: o que você deveria ter pensado

::: exercicio Teste seu design
1. Por que `Bet` guarda uma referência a `Guy` em vez de apenas o nome?
2. O que acontece se dois apostadores apostam no mesmo galgo?
3. Que erro aparece se você esquecer `MyRandom = random`?

--- resposta
1. Com a referência, `Bet` pode usar *qualquer* informação do apostador (nome, caixa) e o objeto `Guy` pode ser atualizado em tempo real; o nome seria só uma cópia de texto.
2. Nada de especial: cada apostador tem seu próprio objeto `Bet`, e `Collect` paga cada um individualmente.
3. `NullReferenceException` na primeira chamada de `Run()`: `MyRandom` é `null` porque ninguém apontou para um `Random`.
:::

## Desafios extras

- **Sem fiado:** impeça apostar quando o apostador já tem aposta ativa.
- **Falência:** se o caixa chegar a zero, desative o `RadioButton` do apostador.
- **Placar:** mostre quantas vitórias cada galgo teve.
- **Pagamento diferente:** pague o dobro da aposta se o galgo for o último a largar (use outra regra em `PayOut`).
- **Som e foto do vencedor:** adicione um recurso de imagem e mostre o vencedor em uma `PictureBox`.

::: resumo
- Antes de programar, **desenhe as classes** (nome / campos / métodos).
- Objetos podem guardar **referências a outros objetos** (`Bet.Bettor`, `Guy.MyLabel`).
- **Compartilhe** um único `Random` entre os objetos; `new Random()` repetido gera sequências iguais.
- Matrizes de objetos (`Greyhound[]`) + laços `for`/`foreach` simplificam o formulário.
- Um **Timer** anima o programa sem travar a interface.
:::

::: quiz
? Por que todos os galgos devem usar a **mesma** instância de `Random`?
+ Para evitar que várias instâncias criadas ao mesmo tempo gerem a mesma sequência
- Porque `Random` só pode ser criado uma vez no programa inteiro
- Porque `Next` não funciona com várias instâncias
- Para economizar memória em disco
> Instâncias de `Random` criadas juntas partilham a semente do relógio e sorteiam os mesmos números.

? Qual o papel de `this` em `Bettor = this;` dentro de `Guy`?
- Cria um novo `Guy`
+ Passa uma referência ao próprio objeto `Guy`
- Torna o campo estático
- Refere-se à classe `Bet`
> `this` é a referência ao objeto corrente.

? Qual componente dispara `Tick` repetidamente para animar a corrida?
- `PictureBox`
- `Label`
+ `Timer`
- `RadioButton`
> O `Timer` executa o evento `Tick` a cada intervalo.

? Em `Greyhound.Run()`, o que significa o retorno `bool`?
+ Se o galgo cruzou a linha de chegada
- Se o galgo está vivo
- Se a aposta foi aceita
- Se o `Random` foi inicializado
> `true` indica que a posição alcançou o fim da pista.

? Qual é a melhor justificativa para dividir o programa em `Greyhound`, `Guy` e `Bet`?
- É obrigatório ter exatamente três classes
+ Cada classe tem uma responsabilidade clara e o código reflete o problema
- Assim o programa fica mais lento e seguro
- Para evitar o uso de métodos
> Coesão: cada classe trata de um assunto.
:::

::: cartas
Laboratório :: Projeto maior que junta os conceitos de vários capítulos.
Responsabilidade (de uma classe) :: O assunto único do qual a classe cuida.
this :: Referência ao objeto em que o método está executando.
Timer :: Componente que dispara o evento Tick em intervalos regulares.
Random compartilhado :: Uma única instância de Random reutilizada para que as sequências não se repitam.
:::
