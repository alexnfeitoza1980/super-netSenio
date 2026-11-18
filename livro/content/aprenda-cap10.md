---
id: aprenda-cap10
order: 22
kind: capitulo
num: 10
group: C# · Parte 5 · Robustez e eventos
title: Tratamento de exceções
subtitle: Apagar incêndios é coisa do passado — o depurador, `try`/`catch`/`finally`, exceções próprias e boas práticas.
minutes: 60
hue: 340
objectives:
- Entender o que é uma **exceção** e de onde ela vem (a hierarquia de `Exception`)
- Usar o **depurador** (pontos de interrupção, passo a passo, observação de variáveis)
- Escrever **`try`/`catch`/`finally`** e vários `catch`
- **Lançar** exceções, criar as suas e evitar maus hábitos (como o `catch` vazio)
origem: livros/01-aprenda-csharp/content/12-cap10.md
---

::: ilustracao img/aprenda/cap10.svg | Exceções: o catch é a rede que pega o erro e evita que o programa caia.
:::

## Programadores não foram treinados para bombeiros

Você estudou muito e chegou longe… mas seu programa **cai** de vez em quando, mostra uma janela feia de erro e fecha. Nada tira mais o seu foco do que ter de caçar um defeito estranho. Com o **tratamento de exceções** você escreve código para lidar com os problemas **quando eles aparecem** — e, melhor ainda, **reagir** a eles e manter tudo funcionando.

::: vila O gerenciador de desculpas do Aldo
Aldo trabalha demais e sonha com pesca submarina. Ele escreveu um programa que guarda **desculpas** para faltar ao trabalho: lê um arquivo de desculpas do disco, escolhe uma e registra a data. Mas o programa quebra o tempo todo: arquivo faltando, campo em branco, data inválida... Cada quebra é uma **exceção**.
:::

## O que é uma exceção?

Quando algo **inesperado** acontece em tempo de execução — dividir por zero, abrir um arquivo que não existe, converter `"abc"` em número — o .NET cria um **objeto `Exception`** descrevendo o problema e o **lança** (*throw*). Se ninguém **captura** esse objeto, o programa encerra com uma mensagem de erro.

```cs
int a = 10, b = 0;
int resultado = a / b;      // 💥 DivideByZeroException
```

```cs
string texto = "abc";
int n = int.Parse(texto);   // 💥 FormatException
```

```cs
string[] v = new string[3];
Console.WriteLine(v[5]);    // 💥 IndexOutOfRangeException
```

### Todas as exceções herdam de `Exception`

::: figura A hierarquia de exceções (parcial)
<svg viewBox="0 0 740 280" role="img" aria-label="Hierarquia de Exception">
  <g text-anchor="middle" font-size="13">
    <rect class="f-prim" x="290" y="8" width="160" height="38" rx="10"/><text x="370" y="33" font-weight="800" class="t-prim">Exception</text>
    <path class="line" d="M370 46V66M110 66H630M110 66V84M300 66V84M470 66V84M630 66V84"/>
    <rect class="f-info" x="20" y="84" width="180" height="38" rx="9"/><text x="110" y="108" font-weight="700">SystemException</text>
    <rect class="f-plain" x="220" y="84" width="160" height="38" rx="9"/><text x="300" y="108">ApplicationException</text>
    <rect class="f-plain" x="400" y="84" width="140" height="38" rx="9"/><text x="470" y="108" class="t-dim">(as suas…)</text>
    <path class="line" d="M110 122V146M60 146H340M60 146V164M200 146V164M340 146V164"/>
    <rect class="f-warn" x="5" y="164" width="110" height="46" rx="9"/><text x="60" y="185" font-weight="700">Arithmetic…</text><text x="60" y="201" font-size="11" class="t-dim">DivideByZero</text>
    <rect class="f-warn" x="140" y="164" width="120" height="46" rx="9"/><text x="200" y="185" font-weight="700">IndexOutOf…</text><text x="200" y="201" font-size="11" class="t-dim">RangeException</text>
    <rect class="f-warn" x="285" y="164" width="110" height="46" rx="9"/><text x="340" y="185" font-weight="700">IO…</text><text x="340" y="201" font-size="11" class="t-dim">FileNotFound</text>
  </g>
  <text x="20" y="250" font-size="12" class="t-dim">Outras: NullReferenceException, FormatException, InvalidCastException, ArgumentException, InvalidOperationException…</text>
</svg>
:::

Como são todas subclasses de `Exception`, um `catch (Exception)` captura **qualquer** exceção — mas capturar o tipo mais **específico** possível é uma boa prática.

## O depurador: seu melhor amigo

Antes de tratar exceções, aprenda a **entender** o que está acontecendo. O **depurador** do Visual Studio deixa você **pausar** o programa e examiná-lo.

| Ferramenta | Atalho | O que faz |
|---|---|---|
| **Ponto de interrupção** (*breakpoint*) | [[F9]] | Pausa o programa **antes** de executar aquela linha |
| **Continuar** | [[F5]] | Segue até o próximo breakpoint |
| **Passo adiante (Step Over)** | [[F10]] | Executa a linha atual, sem entrar nos métodos chamados |
| **Passo para dentro (Step Into)** | [[F11]] | Entra no método chamado pela linha |
| **Passo para fora (Step Out)** | [[Shift]]+[[F11]] | Termina o método atual e volta ao chamador |
| **Inspecionar** | passe o mouse | Mostra o valor de uma variável |
| **Janelas Locais / Inspeção** | | Listam as variáveis e deixam avaliar expressões |

::: ide Passo a passo da depuração
1. Clique na margem cinza à esquerda da linha suspeita (aparece uma **bola vermelha**).
2. Pressione **F5**. Ao chegar na linha, o programa **para** e a linha fica destacada em amarelo.
3. Passe o mouse sobre as variáveis ou abra **Depurar → Janelas → Locais**.
4. Use **F10**/**F11** para avançar uma linha de cada vez e **observe como os valores mudam**.
:::

::: pense Antes de corrigir, entenda
O depurador é uma máquina de **fazer perguntas**: "qual o valor de `x` aqui?", "chegamos neste `if`?". Muitos bugs somem quando você **vê** o que o programa realmente está fazendo, em vez do que você *acha* que ele faz.
:::

## `try` e `catch`: captar o problema

```cs
try
{
    // código que PODE lançar exceção
    string texto = File.ReadAllText(@"C:\desculpas.txt");
    Console.WriteLine("Li " + texto.Length + " caracteres");
}
catch (FileNotFoundException)
{
    // o que fazer se esta exceção específica acontecer
    Console.WriteLine("O arquivo de desculpas não existe. Vou criar um novo.");
}
```

Funcionamento: o C# executa o **bloco `try`**. Se **nenhuma** exceção ocorre, o `catch` é ignorado. Se ocorre, o resto do `try` **é pulado** e o fluxo vai direto para o `catch` correspondente — e depois o programa **continua normalmente**.

::: passos O fluxo de try / catch / finally
Considere:

```cs
try { A(); B(); C(); }
catch (Exception ex) { D(); }
finally { E(); }
F();
```
===
**Caminho feliz:** nenhuma exceção. Executa `A → B → C`, **pula** o `catch`, executa o `finally` (`E`) e segue para `F`.
===
**Exceção em `B`:** `A` roda, `B` lança. O C# **abandona** o `try` (`C` nunca roda), vai ao `catch` (`D`), depois ao `finally` (`E`) e segue em `F`.
===
**Exceção que ninguém captura:** `A`, `B` lança, nenhum `catch` serve. O `finally` (`E`) **ainda executa**, mas depois o erro sobe para quem chamou — e, se ninguém tratar, o programa encerra.
:::

### Use o depurador para seguir o fluxo

Coloque um breakpoint na primeira linha do `try` e avance com **F10**: você verá a execução **saltar** do ponto da exceção para dentro do `catch`. Ver isso uma vez vale mais do que ler dez parágrafos.

### Um bloco `finally` para o que SEMPRE deve executar

```cs
StreamWriter sw = null;
try
{
    sw = new StreamWriter("log.txt");
    sw.WriteLine("Início do processo");
    ProcessarDados();           // pode lançar exceção
}
catch (Exception ex)
{
    Console.WriteLine("Erro: " + ex.Message);
}
finally
{
    if (sw != null) sw.Close();  // executa SEMPRE: com ou sem erro
}
```

O `finally` é o lugar de **limpeza**: fechar arquivos, conexões, restaurar estados. Roda mesmo que haja `return` dentro do `try`.

## O objeto `Exception` traz informações

```cs
catch (Exception ex)
{
    Console.WriteLine(ex.Message);        // texto explicativo
    Console.WriteLine(ex.GetType().Name); // tipo, ex.: FormatException
    Console.WriteLine(ex.StackTrace);     // a "pilha de chamadas": onde ocorreu e por que caminho
    Console.WriteLine(ex.InnerException); // exceção que causou esta (se houver)
}
```

A **pilha de chamadas** (*stack trace*) lista, do ponto da falha para trás, **quais métodos** foram chamados até ali — o mapa para a causa do bug.

### Vários `catch` para vários tipos

```cs
try
{
    string[] partes = linha.Split(';');
    int dia = int.Parse(partes[0]);
    DateTime data = new DateTime(2024, 1, dia);
}
catch (FormatException)
{
    Console.WriteLine("O dia não é um número válido.");
}
catch (IndexOutOfRangeException)
{
    Console.WriteLine("A linha não tem campos suficientes.");
}
catch (ArgumentOutOfRangeException ex)
{
    Console.WriteLine("Data fora do intervalo: " + ex.Message);
}
catch (Exception ex)                    // por último: o mais geral
{
    Console.WriteLine("Erro inesperado: " + ex.Message);
}
```

::: cuidado A ordem dos `catch` importa
O C# testa os `catch` **de cima para baixo** e usa o **primeiro** que casa. Se você puser `catch (Exception)` antes de um específico, o específico **nunca** é alcançado (o compilador nem deixa). Regra: **do mais específico para o mais geral**.
:::

## Lançando exceções: uma classe lança, outra captura

Você também pode **lançar** exceções, para sinalizar que **um método não consegue** fazer o que prometeu:

```cs
class Desculpa
{
    public void Usar(string pessoa)
    {
        if (string.IsNullOrWhiteSpace(pessoa))
            throw new ArgumentException("Informe a pessoa para quem a desculpa será usada.", "pessoa");
        /* ... */
    }
}

// em outro lugar:
try { desculpa.Usar(textBox1.Text); }
catch (ArgumentException ex) { MessageBox.Show(ex.Message); }
```

A grande vantagem: **quem sabe detectar o problema** (a classe) é uma coisa, e **quem sabe o que fazer a respeito** (a interface, o chamador) é outra. A classe lança; quem a usa decide: mostrar mensagem, tentar de novo, registrar em log.

### Criando uma exceção própria

```cs
class MelEsgotadoException : Exception
{
    public int Faltando { get; private set; }

    public MelEsgotadoException(int faltando, string mensagem) : base(mensagem)
    {
        Faltando = faltando;
    }
}

// uso:
if (mel < necessario)
    throw new MelEsgotadoException(necessario - mel, "O mel acabou! Faltam " + (necessario - mel) + " doses.");
```

::: dica Exceções próprias dizem MUITO
`ProdutoSemEstoqueException` comunica o problema melhor do que um genérico `Exception("erro")` — e permite um `catch` **específico** para ele.
:::

### `throw;` — relançando

Dentro de um `catch`, use **`throw;`** (sem o objeto) para capturar, registrar e **deixar subir** a mesma exceção **preservando a pilha**:

```cs
catch (Exception ex)
{
    Log(ex);
    throw;          // NÃO escreva "throw ex;" — isso apaga o stack trace original
}
```

## `using`: um `try`/`finally` pronto

No capítulo 9 você viu `using`. Agora sabemos o que ele faz por baixo:

```cs
using (StreamWriter sw = new StreamWriter("log.txt"))
{
    sw.WriteLine("...");
}

// é equivalente a:
StreamWriter sw = new StreamWriter("log.txt");
try
{
    sw.WriteLine("...");
}
finally
{
    if (sw != null) sw.Dispose();
}
```

E ao escrever **suas próprias** classes que seguram recursos, implemente `IDisposable` para que possam ser usadas dentro de `using`:

```cs
class Conexao : IDisposable
{
    public void Dispose()
    {
        /* fecha a conexão, libera recursos */
    }
}
```

## Maus hábitos e boas ideias

### A pior captura: o bloco vazio

```cs
try { Salvar(); }
catch { }               // 🚫 engole o erro e esconde o problema
```

Ninguém fica sabendo que algo deu errado; o programa segue como se nada tivesse acontecido — e **os dados podem estar corrompidos**. Se você captura, **faça algo**: avise o usuário, registre em log, tente de novo, ou **relance**.

::: erro Exceções não são fluxo de controle
Não use `try/catch` para controle normal de lógica (como detectar se um texto é número). Prefira **testar antes**: `int.TryParse(texto, out int n)`, `File.Exists(...)`, `if (x != null)`. Exceções são caras e devem ser **exceções**.
:::

### Ideias simples para tratamento de exceções

- **Previna** quando possível (valide, teste `null`, use `TryParse`).
- **Capture o mais específico** e o mais perto de onde você sabe **o que fazer**.
- Use **`finally`** ou **`using`** para limpar recursos.
- **Nunca** deixe `catch` vazio; no mínimo registre.
- Mensagens para o **usuário** devem ser claras e acionáveis; os detalhes técnicos vão para o **log**.
- Soluções temporárias são aceitáveis **se** forem temporárias: deixe um comentário `// TODO` e volte depois.

::: exercicio Corrija este código
```cs
Console.Write("Digite a idade: ");
int idade = int.Parse(Console.ReadLine());
Console.WriteLine("Ano de nascimento: " + (DateTime.Now.Year - idade));
```
O que acontece se o usuário digitar `vinte`? Reescreva de duas formas: (a) com `try/catch`; (b) sem exceção.

--- resposta
O `int.Parse` lança `FormatException`.

**(a) com `try/catch`:**
```cs
try
{
    int idade = int.Parse(Console.ReadLine());
    Console.WriteLine("Ano de nascimento: " + (DateTime.Now.Year - idade));
}
catch (FormatException)
{
    Console.WriteLine("Digite apenas números inteiros.");
}
```
**(b) sem exceção (melhor aqui):**
```cs
if (int.TryParse(Console.ReadLine(), out int idade))
    Console.WriteLine("Ano de nascimento: " + (DateTime.Now.Year - idade));
else
    Console.WriteLine("Digite apenas números inteiros.");
```
:::

::: resumo
- Uma **exceção** é um objeto `Exception` lançado quando algo inesperado acontece.
- O **depurador** (breakpoints, F10/F11, janelas Locais) ajuda a entender o problema.
- `try` envolve o código arriscado; `catch` trata o erro; `finally` **sempre** executa.
- Use **vários `catch`**, do mais específico para o mais geral.
- **Lance** (`throw`) exceções, crie as **suas** e relance com `throw;`.
- `using` é um `try/finally` automático para objetos `IDisposable`.
- **Nunca** use `catch` vazio; prefira prevenir (`TryParse`) a capturar.
:::

::: quiz
? O que acontece com as linhas de um `try` após o ponto em que ocorre uma exceção?
- Continuam executando normalmente
+ São puladas; a execução vai para o `catch` correspondente
- São executadas duas vezes
- O programa sempre fecha
> A exceção desvia o fluxo imediatamente.

? Quando o bloco `finally` é executado?
- Só quando há exceção
- Só quando não há exceção
+ Sempre, com ou sem exceção
- Nunca, é opcional
> `finally` é para limpeza garantida.

? Qual a ordem correta de `catch` múltiplos?
+ Do mais específico para o mais geral
- Do mais geral para o mais específico
- Tanto faz
- Em ordem alfabética
> O primeiro `catch` compatível é usado.

? Qual é a diferença entre `throw;` e `throw ex;` em um `catch`?
+ `throw;` preserva a pilha de chamadas original; `throw ex;` a reinicia
- São idênticos
- `throw;` não funciona dentro de catch
- `throw ex;` é mais rápido
> Relançar com `throw;` mantém o rastro do erro.

? O que é o stack trace de uma exceção?
+ A lista de métodos chamados até o ponto da falha
- O valor de todas as variáveis
- O nome do arquivo de log
- O tipo de dado envolvido
> É o mapa de "como chegamos aqui".

? Por que o `catch { }` vazio é considerado a pior prática?
+ Esconde o erro; o programa segue possivelmente com dados corrompidos
- Porque não compila
- Porque é lento
- Porque duplica o `finally`
> Silenciar erros dificulta descobrir e corrigir problemas.

? Qual tecla percorre o código linha a linha sem entrar nos métodos?
- F5
+ F10
- F11
- F9
> F10 = Step Over; F11 = Step Into.

? Qual é uma alternativa melhor a `try/catch` para validar um número digitado?
+ `int.TryParse`
- `catch { }`
- `goto`
- `static`
> Testar antes evita o custo e a confusão de usar exceções para fluxo normal.
:::

::: cartas
Exceção :: Objeto que descreve um problema em tempo de execução e interrompe o fluxo normal.
throw :: Lança uma exceção.
try / catch :: Executa código arriscado / trata a exceção lançada.
finally :: Bloco que sempre executa; usado para limpeza.
Stack trace :: Sequência de métodos que levaram ao ponto do erro.
Breakpoint :: Marca de pausa do depurador (F9).
Step Over / Into / Out :: F10 / F11 / Shift+F11 — formas de avançar a depuração.
Exceção personalizada :: Classe que herda de Exception para representar um erro específico.
TryParse :: Converte texto em número sem lançar exceção (devolve bool).
IDisposable :: Interface para liberar recursos; usada com `using`.
:::
