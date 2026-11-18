---
id: aprenda-cap02
order: 12
kind: capitulo
num: 2
group: C# · Parte 1 · Primeiros passos
title: Tudo é somente código
subtitle: Nos bastidores — namespaces, classes, métodos, variáveis, laços e decisões. Você é programador, não só usuário de IDE.
minutes: 55
hue: 262
objectives:
- Ler a **anatomia** de um programa C#: namespace → classe → método → comandos
- Saber onde fica o **ponto de entrada** (`Main`) e como mudá-lo
- Declarar **variáveis**, usar **operadores** e entender atribuição vs. comparação
- Escrever **laços** (`while`, `for`) e decisões (`if`/`else`)
origem: livros/01-aprenda-csharp/content/02-cap02.md
---

::: ilustracao img/aprenda/cap02.svg | Tudo é somente código: o que você arrasta gera linhas de C# que você pode ler e mudar.
:::

## Você é um programador, não só um usuário de IDE

A IDE faz muita coisa por você — e é ótima nisso. Mas ela só vai até certo ponto: o que seu programa **faz de verdade** está no código C# que **você** escreve. Neste capítulo você abre o capô.

::: figura Quem faz o quê?
<svg viewBox="0 0 760 230" role="img" aria-label="Divisão de trabalho entre você e a IDE">
  <rect class="f-prim" x="15" y="20" width="350" height="190" rx="16"/>
  <text x="190" y="52" text-anchor="middle" font-weight="800" class="t-prim" font-size="18">Enquanto você…</text>
  <g font-size="14"><text x="40" y="88">• arrasta um botão para o formulário</text><text x="40" y="116">• altera propriedades na janela</text><text x="40" y="144">• dá duplo clique para criar um evento</text><text x="40" y="172">• digita o código dos seus métodos</text></g>
  <rect class="f-good" x="395" y="20" width="350" height="190" rx="16"/>
  <text x="570" y="52" text-anchor="middle" font-weight="800" class="t-good" font-size="18">…a IDE:</text>
  <g font-size="14"><text x="418" y="88">• escreve a criação do botão</text><text x="418" y="116">• reescreve o <tspan class="t-mono">Form1.Designer.cs</tspan></text><text x="418" y="144">• gera o método vazio do evento</text><text x="418" y="172">• compila e avisa dos erros</text></g>
</svg>
:::

## De onde vêm os programas

Um formulário é guardado em **dois arquivos** de código que formam **uma só classe**:

- `Form1.Designer.cs` — código gerado pelo designer (a IDE mexe aqui);
- `Form1.cs` — código **seu** (os eventos que você escreve).

Isso é possível por causa da palavra-chave **`partial`** (classe parcial): ela permite dividir a mesma classe em vários arquivos. Você raramente a usará em código próprio; a IDE a usa justamente para separar "código gerado" de "código do programador".

### Ferramentas da IDE que valem ouro

| Recurso | O que faz |
|---|---|
| **Gerenciador de Soluções** | Navega entre arquivos (e, na *Exibição de Classes*, entre classes). |
| **Abas** | Cada arquivo aberto ganha uma aba; um asterisco `*` indica alterações não salvas. |
| **IntelliSense** | Janelinhas que sugerem como completar o que você digita e mostram os parâmetros de cada método. |
| **Snippets (trechos)** | Digite `mbox` + [[Tab]] [[Tab]] e vem um `MessageBox.Show(...)`; `for` + Tab Tab gera um laço completo. |
| **Lista de Erros** | Mostra o que impede a compilação; duplo clique leva à linha com problema. |

::: dica Use o IntelliSense como livro de consulta
Digitou `MessageBox.` e apareceu uma lista? Aquilo é tudo que você pode fazer com `MessageBox`. Ao abrir o parêntese de `Show(`, a IDE mostra as **sobrecargas** (formas diferentes de chamar) e o que cada parâmetro significa.
:::

## Quando você muda a IDE, o código muda

Faça este experimento (leva 3 minutos e ensina muito):

1. Crie um projeto *Windows Forms* e adicione um botão.
2. Abra `Form1.Designer.cs` (botão direito no `Form1.cs` → *Exibir Código*, ou abra o arquivo pelo Gerenciador de Soluções).
3. Procure `this.button1.Text = "button1";`. Troque o texto por `"Oi!"` **no código**. Volte ao designer: o botão mudou.
4. Agora mude `Location` e `Size` pelo designer e volte ao código: os números mudaram lá.

::: regra O designer e o código são a mesma coisa
O designer é só uma **forma visual** de editar código. Mexeu em um, mexeu no outro. E se você apagar `InitializeComponent`, o formulário perde todos os controles.
:::

::: cuidado O comentário "não modifique"
No topo do método `InitializeComponent` a IDE avisa: *"Não modifique o conteúdo deste método com o editor de código"*. Pequenas mudanças funcionam, mas o designer pode **sobrescrever** o que você escreveu à mão, ou até parar de abrir o formulário se o código ficar inconsistente. Use isso para aprender, não no dia a dia.
:::

## Anatomia de um programa C#

Todo programa C# tem a mesma estrutura em camadas. Abra o `Program.cs` e o `Form1.cs` e veja:

::: figura As camadas de um programa
<svg viewBox="0 0 760 330" role="img" aria-label="Namespace contém classes, que contêm métodos, que contêm comandos">
  <rect class="f-plain" x="10" y="10" width="740" height="310" rx="18"/>
  <text x="28" y="38" font-weight="800">namespace <tspan class="t-mono t-prim">Contatos</tspan></text><text x="740" y="38" text-anchor="end" font-size="13" class="t-dim">agrupa classes — mantém o código organizado</text>
  <rect class="f-prim" x="30" y="55" width="700" height="250" rx="14"/>
  <text x="48" y="82" font-weight="800" class="t-prim">class <tspan class="t-mono">Program</tspan></text><text x="712" y="82" text-anchor="end" font-size="13" class="t-dim">uma classe tem métodos (e campos)</text>
  <rect class="f-info" x="50" y="98" width="660" height="190" rx="12"/>
  <text x="68" y="124" font-weight="800" class="t-info">método <tspan class="t-mono">static void Main()</tspan></text><text x="692" y="124" text-anchor="end" font-size="13" class="t-dim">um método é feito de comandos</text>
  <rect class="f-good" x="70" y="140" width="620" height="38" rx="9"/><text class="t-mono" x="86" y="164" font-size="14">Application.EnableVisualStyles();</text>
  <rect class="f-good" x="70" y="186" width="620" height="38" rx="9"/><text class="t-mono" x="86" y="210" font-size="14">Application.SetCompatibleTextRenderingDefault(false);</text>
  <rect class="f-good" x="70" y="232" width="620" height="38" rx="9"/><text class="t-mono" x="86" y="256" font-size="14">Application.Run(new Form1());</text>
  <text x="680" y="262" text-anchor="end" font-size="13" class="t-dim">comandos terminam com ;</text>
</svg>
:::

```cs Program.cs
using System;                      // 1) quais "caixas de ferramentas" do .NET usar
using System.Windows.Forms;

namespace Contatos                 // 2) o namespace do SEU programa
{
    static class Program           // 3) uma classe
    {
        [STAThread]
        static void Main()         // 4) um método (o ponto de entrada!)
        {
            Application.EnableVisualStyles();            // 5) comandos
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());                 // mostra o formulário
        }
    }
}
```

- **`using`** diz quais *namespaces* do .NET você vai utilizar (assim você escreve `MessageBox` em vez de `System.Windows.Forms.MessageBox`).
- **`namespace`** agrupa as suas classes e as separa das do .NET.
- **Classe** reúne um pedaço do programa; programas pequenos podem ter uma só.
- **Método** é onde a ação acontece: recebe **parâmetros** (entradas), executa comandos e pode devolver um **valor de retorno** (`void` = não devolve nada).
- **Comando** (*statement*) é uma instrução; o ponto e vírgula `;` marca o fim.

::: analogia O prédio de escritórios
O **namespace** é o prédio; as **classes** são os andares (cada um com sua função); os **métodos** são as pessoas que trabalham ali; os **comandos** são as tarefas que cada pessoa executa, uma após a outra.
:::

### Métodos: parâmetros e retorno

```cs
public int MultiplicarDois(int primeiro, int segundo)
{
    return primeiro * segundo;   // return encerra o método e devolve o valor
}

// quem chama:
int resultado = MultiplicarDois(3, 5);   // resultado = 15
```

- `int` antes do nome é o **tipo de retorno**.
- `int primeiro, int segundo` são os **parâmetros**: valores que o método recebe.
- Ao encontrar `return`, o método termina na hora. Em métodos `void`, escreva só `return;` (e nem é obrigatório).

### O ponto de entrada

Quando o programa inicia, o C# procura **um único** método `static void Main()` e executa seu primeiro comando. Não importa em que classe ele está.

::: exercicio Experimente: quebre o ponto de entrada
1. Em `Program.cs`, renomeie `Main` para `NotMain` e tente executar. O que acontece?
2. Crie uma classe `OutraClasse` com um `static void Main()` que apenas exibe uma `MessageBox`. Execute.
3. Como fazer o programa voltar a mostrar o formulário?

--- resposta
1. Erro de compilação: o programa não tem **ponto de entrada**.
2. O programa exibe só a caixa de mensagem e termina: o `Main` novo virou o ponto de entrada (se houver dois `Main`, o compilador reclama de ambiguidade — é preciso escolher um em *Propriedades do projeto → Objeto de inicialização*).
3. Dentro do novo `Main`, chame `Application.Run(new Form1());`, ou volte o nome original.
:::

### Classes no mesmo namespace se enxergam

Se `Dog`, `Cat` e `Fish` estão no mesmo namespace, o método `Dog.Bark()` pode chamar `Cat.Meow()` e `Fish.Swim()` — mesmo que estejam em **arquivos diferentes**. Já classes de outros namespaces exigem um `using` (ou o nome completo). E uma classe pode ficar espalhada em vários arquivos se for marcada `partial`.

## Variáveis: onde o programa guarda dados

Quase todo programa é, no fundo, um **processador de dados**: um documento, uma imagem de jogo, uma mensagem. Dados vivem em **variáveis**.

```cs
int minhaAltura;          // declara: tipo + nome
minhaAltura = 63;         // atribui um valor
string nome = "Lucília";  // declara E atribui, tudo de uma vez
bool ativo = true;
```

Tipos mais populares por enquanto (o capítulo 4 detalha todos):

| Tipo | Guarda | Exemplo |
|---|---|---|
| `int` | números inteiros | `42`, `-7` |
| `string` | texto | `"Olá"` |
| `bool` | verdadeiro/falso | `true`, `false` |

::: analogia Variável = caixa etiquetada
Uma variável é uma **caixa** com **etiqueta** (nome) que só aceita um **tipo** de coisa. O conteúdo pode mudar (por isso "variável"), mas a etiqueta e o tipo ficam. Em qualquer ponto do código que usar `minhaAltura`, o C# troca o nome pelo valor atual.
:::

::: cuidado Use só depois de atribuir
O C# se recusa a compilar se você **ler** uma variável antes de dar a ela um valor. Isso é uma proteção: evita lixo. A forma mais simples de nunca esquecer é **declarar e atribuir na mesma linha**.
:::

### Operadores: fazendo contas

```cs
int x = 5;
x = x + 3;     // 8
x += 2;        // 10  (atalho de x = x + 2)
x++;           // 11  (soma 1)
x--;           // 10  (subtrai 1)
x = x * 3;     // 30
x = x / 4;     // 7   (divisão de inteiros descarta o resto)
int resto = 30 % 4;   // 2 (o módulo devolve o resto)
```

::: curiosidade O `=` não é igual à matemática
Em C#, `x = x + 1` **não é uma equação** (que seria absurda); é uma **instrução**: "calcule `x + 1` e guarde o resultado em `x`". O `=` sempre significa **atribuir**.
:::

## Laços: executar algo várias vezes

Programas reais repetem tarefas o tempo todo. Um **laço** (*loop*) executa um bloco **enquanto** uma condição for verdadeira.

### `while`

```cs
int x = 0;
while (x < 5)
{
    MessageBox.Show("x vale " + x);
    x++;
}
```

### `for` — o `while` com tudo numa linha

```cs
for (int i = 0; i < 5; i++)
{
    MessageBox.Show("i vale " + i);
}
```

O cabeçalho do `for` tem **três partes**, separadas por `;`: **(1)** inicialização (roda uma vez), **(2)** teste condicional (antes de cada volta), **(3)** incremento (depois de cada volta).

::: passos Acompanhe um for, volta a volta
O código: `int total = 0; for (int i = 1; i <= 3; i++) { total = total + i; }`

**Antes do laço:** `total = 0`. O `for` cria `i = 1`.
===
**Teste:** `i <= 3`? `1 <= 3` → **verdadeiro**. Executa o bloco: `total = 0 + 1 = 1`. Depois o `i++` faz `i = 2`.
===
**Teste:** `2 <= 3` → verdadeiro. `total = 1 + 2 = 3`. `i++` → `i = 3`.
===
**Teste:** `3 <= 3` → verdadeiro. `total = 3 + 3 = 6`. `i++` → `i = 4`.
===
**Teste:** `4 <= 3` → **falso**. O laço termina. Resultado: `total = 6` (1 + 2 + 3).
:::

::: dica Digite `for` e aperte Tab, Tab
O snippet escreve o laço inteiro. Se você trocar o nome da variável, ele atualiza nos três lugares. Tab de novo pula para o próximo campo editável.
:::

::: erro Laço infinito
Se a condição **nunca** ficar falsa (por exemplo, esquecer o `x++`), o programa nunca sai do laço e trava. Antes de rodar, pergunte: *"o que vai fazer essa condição virar falsa?"*
:::

## Decisões com `if` / `else`

Use `if` para executar código **só se** uma condição for verdadeira, e `else` para o caso contrário.

```cs
int x = 10;
if (x == 10)
{
    MessageBox.Show("x é igual a 10");
}
else
{
    MessageBox.Show("x NÃO é 10");
}
```

### Operadores condicionais

| Operador | Significa | Exemplo |
|---|---|---|
| `==` | é igual a | `x == 3` |
| `!=` | é diferente de | `x != 3` |
| `<` / `>` | menor / maior que | `x < 5` |
| `<=` / `>=` | menor ou igual / maior ou igual | `x >= 5` |
| `&&` | **E** (as duas verdadeiras) | `x == 3 && y < 5` |
| `\|\|` | **OU** (pelo menos uma verdadeira) | `x == 3 \|\| y < 5` |
| `!` | **NÃO** (inverte) | `!ativo` |

::: erro `=` versus `==`
Um sinal `=` **atribui**. Dois sinais `==` **comparam**. Trocar um pelo outro é um dos erros mais comuns do mundo, até entre veteranos. Se o compilador reclamar *"não é possível converter implicitamente o tipo 'int' em 'bool'"* num `if`, veja se você escreveu `=` onde queria `==`.
:::

Quando o valor já é `bool`, não precisa comparar com `true`:

```cs
if (checkBox1.Checked == true) { ... }   // funciona, mas é redundante
if (checkBox1.Checked) { ... }           // melhor
if (!checkBox1.Checked) { ... }          // quando NÃO está marcada
```

## Mão na massa: Diversão com if/else

O projeto do capítulo é um formulário com quatro botões, cada um exercitando uma ideia. Crie um projeto *Windows Forms* chamado `Cap2 Diversao com if-else` e coloque no `button1_Click`:

```cs
int someValue = 6;
string name = "Lucília";
if (name == "Joaquim")
{
    someValue++;
}
else
{
    someValue--;
}
MessageBox.Show("someValue vale " + someValue);
```

::: pense Antes de rodar
Qual será o valor mostrado? Escreva a resposta e só então clique no botão. Fazer **previsões** antes de executar é o treino mais eficiente que existe para ler código.
:::

::: exercicio Troque a cor de um rótulo
Adicione um `Label` (`label1`) e uma `CheckBox` (`checkBox1`) ao formulário. No clique de um botão:

- Se a caixa estiver marcada, mude o fundo do rótulo para vermelho: `label1.BackColor = Color.Red;`
- Se o fundo já for vermelho, mude-o para azul; se for azul, volte para vermelho. (Dica: o teste é `label1.BackColor == Color.Red`.)

--- resposta
```cs
private void button1_Click(object sender, EventArgs e)
{
    if (checkBox1.Checked)
    {
        label1.BackColor = Color.Red;
    }
    else if (label1.BackColor == Color.Red)
    {
        label1.BackColor = Color.Blue;
    }
    else
    {
        label1.BackColor = Color.Red;
    }
}
```
:::

::: exercicio Fundo psicodélico (laços aninhados)
Faça o fundo do formulário **percorrer cores** quando o botão for clicado: use um laço com `c` de 0 a 254 e `this.BackColor = Color.FromArgb(c, 255 - c, c);`. Depois faça uma segunda volta de 254 até 0 e, por fim, **envolva tudo** em um laço externo que repita até o formulário ser fechado.

--- resposta
```cs
private void button4_Click(object sender, EventArgs e)
{
    while (Visible)
    {
        for (int c = 0; c < 255; c++)
        {
            this.BackColor = Color.FromArgb(c, 255 - c, c);
            Application.DoEvents();
        }
        for (int c = 254; c >= 0; c--)
        {
            this.BackColor = Color.FromArgb(c, 255 - c, c);
            Application.DoEvents();
        }
    }
}
```
`Application.DoEvents()` deixa o Windows processar outros eventos (como o clique no **X**); sem ele a janela congelaria. Mesmo assim, há um pequeno atraso ao fechar a janela: os `for` internos precisam terminar antes de o `while` testar `Visible` de novo. Para eliminar o atraso, acrescente `&& Visible` à condição de cada `for` (por exemplo, `c < 255 && Visible`).

Hoje se prefere um **`Timer`** no lugar de laços com `DoEvents` — você o verá no capítulo 12.
:::

## Perguntas rápidas

::: pense Toda linha de código fica dentro de uma classe?
Sim. Comandos ficam em métodos, métodos em classes, classes em namespaces. Até o código gerado pelo designer segue essa hierarquia.
:::

::: pense Posso dar qualquer nome ao namespace?
Quase. Use letras, números e sublinhados. A IDE sugere um com base no nome do projeto. Evite os que o .NET já usa (como `System`).
:::

::: resumo
- Um programa C# é **namespace → classe → método → comandos**.
- A execução começa no único método **`Main`** (o *ponto de entrada*).
- **Variáveis** têm tipo e nome; use **depois** de atribuir.
- `=` atribui, `==` compara; `&&`, `||` e `!` combinam condições.
- **`while`** e **`for`** repetem; **`if`/`else`** decidem.
- O designer e o código são a mesma coisa: tudo que a IDE faz vira C#.
:::

::: quiz
? Qual é a hierarquia correta de um programa C#?
- Método → classe → namespace → comando
+ Namespace → classe → método → comando
- Classe → namespace → comando → método
- Comando → método → namespace → classe
> Namespaces contêm classes; classes contêm métodos; métodos são feitos de comandos.

? O que é o método `Main`?
- Um método que só existe em formulários
+ O ponto de entrada: o primeiro método executado pelo programa
- O método que fecha o programa
- Um método gerado pelo IntelliSense
> O C# procura o `static void Main()` e executa seu primeiro comando.

? Qual a diferença entre `=` e `==`?
- Nenhuma, são intercambiáveis
+ `=` atribui um valor; `==` compara dois valores
- `=` compara; `==` atribui
- `==` só serve para texto
> Atribuição altera a variável; comparação produz verdadeiro ou falso.

? O que `x++` faz?
- Multiplica x por ele mesmo
+ Soma 1 a x
- Compara x com 1
- Declara uma variável chamada x
> `x++` é o mesmo que `x = x + 1`.

? Quantas vezes executa o bloco: `for (int i = 0; i < 5; i++) { ... }`?
- 4 vezes
+ 5 vezes (i = 0, 1, 2, 3, 4)
- 6 vezes
- Infinitas vezes
> O teste `i < 5` falha quando i chega a 5, então o corpo roda para i de 0 a 4.

? O que acontece se você usar uma variável que nunca recebeu valor?
- Ela vale 0 automaticamente
- O programa roda e mostra lixo
+ O compilador recusa e mostra um erro
- O C# pergunta ao usuário
> O C# garante a atribuição antes do uso para evitar resultados imprevisíveis.

? Por que o formulário tem dois arquivos (`Form1.cs` e `Form1.Designer.cs`)?
+ É uma classe parcial (`partial`): o código gerado fica separado do código do programador
- São duas classes diferentes
- Um deles é só comentário
- O segundo arquivo é opcional
> `partial` permite dividir uma classe em vários arquivos.

? Qual snippet cria rapidamente um laço `for` no editor?
- Digitar `loop` e Enter
+ Digitar `for` e pressionar Tab duas vezes
- Pressionar F5
- Digitar `mbox` e Tab
> Snippets são abreviações que expandem em código completo.
:::

::: cartas
Namespace :: Agrupador de classes; evita conflito de nomes e organiza o código.
Classe :: Molde que reúne campos e métodos relacionados.
Método :: Bloco de comandos nomeado; pode receber parâmetros e devolver um valor.
Parâmetro :: Valor de entrada que um método recebe ao ser chamado.
Valor de retorno :: Valor que o método devolve ao chamador com `return`; `void` = nada.
Ponto de entrada (Main) :: Método `static void Main()` onde a execução do programa começa.
Variável :: Nome associado a um tipo e a um valor guardado na memória.
Declarar × atribuir :: Declarar cria a variável (tipo + nome); atribuir dá um valor a ela.
`=` × `==` :: `=` atribui; `==` compara e produz bool.
Laço (loop) :: Estrutura que repete comandos enquanto uma condição for verdadeira (while, for).
Teste condicional :: Expressão que resulta em true/false, usada por if e laços.
IntelliSense :: Sugestões automáticas da IDE enquanto você digita.
Snippet :: Abreviação que a IDE expande em um trecho de código (ex.: `for` + Tab Tab).
partial :: Palavra-chave que permite dividir uma classe em vários arquivos.
:::
