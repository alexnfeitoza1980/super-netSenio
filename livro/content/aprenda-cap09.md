---
id: aprenda-cap09
order: 21
kind: capitulo
num: 9
group: C# · Parte 4 · Coleções e arquivos
title: Lendo e salvando arquivos
subtitle: Salve o conjunto de bytes, salve o mundo — streams, `File`, `using`, serialização, binário e hexadecimal.
minutes: 70
hue: 30
objectives:
- Entender **streams** e usar `StreamWriter`/`StreamReader`, `FileStream` e `BinaryWriter`
- Usar as classes **`File`** e **`Directory`** e as **caixas de diálogo** padrão do Windows
- Garantir a liberação de recursos com **`IDisposable`** e **`using`**
- **Serializar** objetos e compreender **bytes**, **Unicode**, **hexadecimal** e binário
origem: livros/01-aprenda-csharp/content/11-cap09.md
---

::: ilustracao img/aprenda/cap09.svg | Arquivos: o stream é o cano por onde os bytes passam, um pouco de cada vez.
:::

## Às vezes vale a pena ser persistente

Até agora seus programas viveram pouco: abrem, executam, fecham — e **tudo some**. Isso não serve quando há informação importante: você precisa **salvar** o trabalho e **ler de volta** depois. Neste capítulo vamos escrever dados em **arquivos** e lê-los de novo.

## O C# usa streams para ler e escrever

Um **stream** (fluxo, "riacho") é um **canal por onde os dados correm**, em ordem, de uma origem para um destino. Você não "abre o arquivo inteiro" de uma vez: você **escreve** ou **lê** pelo canal, como água por uma mangueira.

::: analogia Streams = mangueiras
Para **ler** um arquivo você liga uma mangueira **do arquivo até o seu programa**; para **escrever**, uma mangueira **do programa até o arquivo**. Dá para **emendar** mangueiras: uma que leva bytes (`FileStream`) e outra que converte bytes em texto (`StreamReader`).
:::

### Streams diferentes leem e escrevem coisas diferentes

| Classe | Para que serve |
|---|---|
| `FileStream` | Lê/escreve **bytes** em um arquivo |
| `StreamWriter` | Escreve **texto** num stream (ex.: arquivo) |
| `StreamReader` | Lê **texto** de um stream |
| `BinaryWriter` / `BinaryReader` | Escreve/lê **tipos primitivos** em binário (`int`, `double`…) |
| `MemoryStream` | Stream dentro da **memória** (sem arquivo) |

### Escrever texto: o jeito mais simples

```cs
using System.IO;

StreamWriter sw = new StreamWriter(@"C:\Temp\segredo.txt");
sw.WriteLine("Esta é a primeira linha");
sw.Write("Esta ");
sw.Write("é a segunda");
sw.Close();     // IMPORTANTE: fecha e descarrega os dados no arquivo
```

O `@` antes das aspas cria uma **string literal** (*verbatim*): as barras invertidas `\` não são tratadas como escape (sem ele precisaria escrever `"C:\\Temp\\..."`).

::: cuidado Sempre feche o stream
Se você esquecer `Close()` (ou `Dispose()`), os dados podem ficar presos no **buffer** (memória) e nunca chegar ao arquivo; e o arquivo continua **bloqueado** — outros programas não conseguem abri-lo. A seção sobre `using` mostra como nunca mais esquecer.
:::

### Ler texto

```cs
StreamReader sr = new StreamReader(@"C:\Temp\segredo.txt");
while (!sr.EndOfStream)
{
    string linha = sr.ReadLine();
    Console.WriteLine(linha);
}
sr.Close();
```

```saida
Esta é a primeira linha
Esta é a segunda
```

### Um `FileStream` escreve bytes

Por baixo, tudo em arquivo são **bytes**. O `FileStream` é a mangueira básica; `StreamWriter` é um **adaptador** que transforma texto em bytes.

```cs
FileStream fs = new FileStream(@"C:\Temp\bytes.dat", FileMode.Create);
byte[] dados = { 72, 101, 108, 108, 111 };    // "Hello" em ASCII
fs.Write(dados, 0, dados.Length);
fs.Close();
```

::: passos Ler e escrever envolve dois objetos
**1)** Para **escrever**, seu programa usa um `StreamWriter`…
===
**2)** …que escreve os caracteres em um **`FileStream`** (convertendo texto em bytes)…
===
**3)** …e o `FileStream` entrega os bytes ao **arquivo** no disco. Para ler, o caminho é o inverso.

::: html
<svg viewBox="0 0 720 120"><g text-anchor="middle" font-size="14"><rect class="f-prim" x="10" y="30" width="150" height="58" rx="12"/><text x="85" y="64" font-weight="800" class="t-prim">seu programa</text><rect class="f-info" x="210" y="30" width="150" height="58" rx="12"/><text x="285" y="58" font-weight="800" class="t-info">StreamWriter</text><text x="285" y="76" font-size="11" class="t-dim">texto → bytes</text><rect class="f-good" x="410" y="30" width="130" height="58" rx="12"/><text x="475" y="64" font-weight="800" class="t-good">FileStream</text><rect class="f-warn" x="585" y="30" width="125" height="58" rx="12"/><text x="647" y="64" font-weight="800" class="t-warn">arquivo.txt</text></g><path class="arrow" d="M162 59H206"/><path class="arrow" d="M362 59H406"/><path class="arrow" d="M542 59H581"/></svg>
:::
:::

### Dados podem passar por mais de um stream

Você pode **encadear** streams: um `StreamWriter` sobre um `FileStream`, sobre um `GZipStream` (compressão)… Cada elo faz uma transformação.

## Caixas de diálogo padrão

Para o usuário escolher onde salvar ou abrir, não crie telas: use os **diálogos nativos do Windows**, que também são **objetos**.

```cs
SaveFileDialog salvar = new SaveFileDialog();
salvar.Filter = "Arquivos de texto (*.txt)|*.txt|Todos os arquivos (*.*)|*.*";
salvar.InitialDirectory = @"C:\Temp";
if (salvar.ShowDialog() == DialogResult.OK)
{
    File.WriteAllText(salvar.FileName, textBox1.Text);
}
```

- `SaveFileDialog` e `OpenFileDialog` têm `Filter`, `InitialDirectory`, `Title`, `FileName`.
- `ShowDialog()` abre a janela e devolve um `DialogResult` (`OK` ou `Cancel`).
- Também há `FolderBrowserDialog`, `ColorDialog`, `FontDialog`.

::: ide Dá para arrastar para o formulário
Na Caixa de Ferramentas, em **Diálogos**, arraste `OpenFileDialog` ou `SaveFileDialog`: ele aparece na bandeja de componentes abaixo do formulário e você configura as propriedades no painel.
:::

## As classes `File` e `Directory`

Para tarefas comuns, não precisa criar streams: as classes **estáticas** `File` e `Directory` fazem tudo em uma linha.

```cs
if (File.Exists(@"C:\Temp\a.txt"))
{
    string texto = File.ReadAllText(@"C:\Temp\a.txt");          // lê tudo
    string[] linhas = File.ReadAllLines(@"C:\Temp\a.txt");      // linha a linha
    File.WriteAllText(@"C:\Temp\b.txt", texto.ToUpper());       // cria/sobrescreve
    File.Copy(@"C:\Temp\a.txt", @"C:\Temp\c.txt");
    File.Delete(@"C:\Temp\b.txt");
}

Directory.CreateDirectory(@"C:\Temp\Nova");
string[] arquivos = Directory.GetFiles(@"C:\Temp", "*.txt");
foreach (string caminho in arquivos)
    Console.WriteLine(Path.GetFileName(caminho));
```

| Classe | Alguns métodos |
|---|---|
| `File` | `Exists`, `ReadAllText`, `ReadAllLines`, `WriteAllText`, `AppendAllText`, `Copy`, `Move`, `Delete` |
| `Directory` | `Exists`, `CreateDirectory`, `GetFiles`, `GetDirectories`, `Delete` |
| `Path` | `Combine`, `GetFileName`, `GetExtension`, `GetDirectoryName` |

::: dica Use `Path.Combine`
Nunca monte caminhos concatenando strings com `\`. `Path.Combine("C:\\Temp", "arq.txt")` cuida das barras corretamente (e funciona em qualquer sistema operacional).
:::

## `IDisposable` e a instrução `using`

Streams seguram **recursos do sistema** (o arquivo aberto). Eles precisam ser **fechados** mesmo que o programa tenha um erro no meio. Classes que fazem isso implementam a interface **`IDisposable`**, cujo único método é **`Dispose()`** (que libera os recursos).

A instrução **`using`** garante que `Dispose()` seja chamado **no fim do bloco** — mesmo se ocorrer uma exceção:

```cs
using (StreamWriter sw = new StreamWriter(@"C:\Temp\agenda.txt"))
{
    sw.WriteLine("Ana;11 99999-0000");
    sw.WriteLine("Bruno;21 98888-1111");
}   // <- aqui o stream é fechado automaticamente
```

```cs
// Forma mais moderna (C# 8+): termina no fim do método
using var sr = new StreamReader(@"C:\Temp\agenda.txt");
string primeira = sr.ReadLine();
```

::: regra Arquivo aberto? Use `using`
Toda vez que você criar um objeto `IDisposable` (streams, conexões de banco, imagens), coloque-o dentro de um `using`. É o equivalente a **"feche a porta ao sair"**, mesmo que você saia correndo.
:::

## Tomando decisões com `switch`

Ao salvar, o programa costuma oferecer **várias opções** (salvar, abrir, apagar, sair). Em vez de uma cadeia de `if/else`, use `switch`:

```cs
Console.Write("Opção (S=salvar, A=abrir, X=sair): ");
string opcao = Console.ReadLine().ToUpper();

switch (opcao)
{
    case "S":
        SalvarAgenda();
        break;
    case "A":
        AbrirAgenda();
        break;
    case "X":
        return;
    default:
        Console.WriteLine("Opção inválida!");
        break;
}
```

::: cuidado Não esqueça o `break`
Cada `case` precisa terminar com `break` (ou `return`/`throw`). Diferente de outras linguagens, o C# **não deixa** a execução "escorregar" para o próximo `case` — dá erro de compilação. `default` trata tudo que não casou com nenhum `case`.
:::

## Serialização: salvar um objeto inteiro de uma vez

Gravar cada campo do objeto à mão é trabalhoso (e repleto de chances de erro). A **serialização** converte um objeto — com **todos os objetos a que ele se refere** — numa sequência de bytes que pode ser gravada num stream; a **desserialização** reconstrói o objeto a partir dela.

::: pense O que é, exatamente, o "estado" de um objeto?
É o conjunto dos valores dos seus **campos**. Para salvar e restaurar um baralho, você precisa guardar **cada carta**, e cada carta precisa guardar seu naipe e valor. Se um objeto aponta para outros, **tudo que ele alcança** precisa ser salvo também.
:::

::: figura Serializando um objeto e tudo que ele referencia
<svg viewBox="0 0 700 220" role="img" aria-label="Objeto Baralho referenciando Cartas, serializado em bytes">
  <rect class="f-info" x="10" y="70" width="170" height="80" rx="12"/><text x="95" y="102" text-anchor="middle" font-weight="800" class="t-info">Baralho</text><text x="95" y="126" text-anchor="middle" class="t-mono" font-size="12">List&lt;Carta&gt; cartas</text>
  <rect class="f-good" x="240" y="20" width="150" height="50" rx="10"/><text x="315" y="50" text-anchor="middle" class="t-mono" font-size="12">Carta: As ♠</text>
  <rect class="f-good" x="240" y="85" width="150" height="50" rx="10"/><text x="315" y="115" text-anchor="middle" class="t-mono" font-size="12">Carta: 7 ♥</text>
  <rect class="f-good" x="240" y="150" width="150" height="50" rx="10"/><text x="315" y="180" text-anchor="middle" class="t-mono" font-size="12">Carta: Rei ♦</text>
  <path class="arrow" d="M182 100C210 100 210 45 238 45"/><path class="arrow" d="M182 110H238"/><path class="arrow" d="M182 120C210 120 210 175 238 175"/>
  <rect class="f-warn" x="470" y="70" width="220" height="80" rx="12"/><text x="580" y="102" text-anchor="middle" font-weight="800" class="t-warn">arquivo .dat</text><text x="580" y="126" text-anchor="middle" class="t-mono" font-size="11">00 01 00 00 00 FF FF …</text>
  <path class="arrow" d="M392 110H466"/><text x="428" y="100" text-anchor="middle" font-size="11" class="t-dim">serializar</text>
</svg>
:::

### Marcando a classe

No livro (era .NET Framework), o caminho clássico era o atributo **`[Serializable]`** com o `BinaryFormatter`:

```cs
[Serializable]
class Carta { public Naipe Naipe; public Valor Valor; }

[Serializable]
class Baralho { public List<Carta> Cartas = new List<Carta>(); }

// salvar
BinaryFormatter bf = new BinaryFormatter();
using (Stream saida = File.Create("baralho.dat"))
    bf.Serialize(saida, baralho);

// ler
using (Stream entrada = File.OpenRead("baralho.dat"))
    baralho = (Baralho) bf.Deserialize(entrada);
```

::: cuidado `BinaryFormatter` é obsoleto e inseguro
Desserializar dados de fonte não confiável com `BinaryFormatter` pode executar código malicioso. A Microsoft o **marcou como obsoleto** (e o removeu em versões recentes do .NET). Para novos projetos, use **JSON**:

```cs
using System.Text.Json;

string json = JsonSerializer.Serialize(baralho);
File.WriteAllText("baralho.json", json);

Baralho lido = JsonSerializer.Deserialize<Baralho>(File.ReadAllText("baralho.json"));
```
O **conceito** (objeto → bytes/texto → objeto) é o mesmo; o JSON ainda tem a vantagem de ser legível por humanos.
:::

### Construtor sobrecarregado para ler o baralho

Um padrão muito útil: oferecer **dois construtores** — um que cria um baralho novo, outro que **carrega de um arquivo**:

```cs
class Baralho
{
    private List<Carta> cartas;

    public Baralho()                       // novo
    {
        cartas = new List<Carta>();
        /* preenche as 52 cartas */
    }

    public Baralho(string arquivo)         // carrega
    {
        cartas = new List<Carta>();
        using (StreamReader sr = new StreamReader(arquivo))
        {
            while (!sr.EndOfStream)
            {
                string[] partes = sr.ReadLine().Split(';');
                Naipe naipe = (Naipe) Enum.Parse(typeof(Naipe), partes[0]);
                Valor valor = (Valor) Enum.Parse(typeof(Valor), partes[1]);
                cartas.Add(new Carta(valor, naipe));
            }
        }
    }

    public void Salvar(string arquivo)
    {
        using (StreamWriter sw = new StreamWriter(arquivo))
            foreach (Carta c in cartas)
                sw.WriteLine(c.Naipe + ";" + c.Valor);
    }
}
```

## Bytes, Unicode, hexadecimal e binário

Todo arquivo é uma sequência de **bytes** (números de 0 a 255). Texto é só uma **convenção**: cada caractere corresponde a um número.

- **ASCII**: o código antigo de 128 caracteres (`'A'` = 65).
- **Unicode**: o padrão atual, com **todos** os alfabetos e emojis. O .NET trabalha com **UTF-16** em memória, e costuma gravar arquivos em **UTF-8**; ele converte automaticamente.

```cs
byte[] bytes = Encoding.UTF8.GetBytes("Olá");
// 79 108 195 163   ← o 'á' ocupa 2 bytes em UTF-8
string texto = Encoding.UTF8.GetString(bytes);
```

### Binário e hexadecimal

Um byte tem 8 bits. Humanos preferem escrever em **hexadecimal** (base 16: 0–9 e A–F), em que **um byte = dois dígitos**.

| Decimal | Binário | Hexadecimal | Caractere ASCII |
|---|---|---|---|
| 72 | 0100 1000 | `48` | `H` |
| 101 | 0110 0101 | `65` | `e` |
| 108 | 0110 1100 | `6C` | `l` |
| 111 | 0110 1111 | `6F` | `o` |

Um **editor hexadecimal** mostra o conteúdo cru de qualquer arquivo — útil para entender o que seu programa realmente gravou.

### Gravar números em binário com `BinaryWriter`

```cs
using (BinaryWriter bw = new BinaryWriter(File.Create("dados.bin")))
{
    bw.Write(42);          // int: 4 bytes
    bw.Write(3.14);        // double: 8 bytes
    bw.Write("Olá");       // string com tamanho
    bw.Write(true);        // bool: 1 byte
}

using (BinaryReader br = new BinaryReader(File.OpenRead("dados.bin")))
{
    int a = br.ReadInt32();
    double b = br.ReadDouble();
    string c = br.ReadString();
    bool d = br.ReadBoolean();      // leia NA MESMA ORDEM em que gravou!
}
```

::: erro Ordem e formato têm de bater
Num arquivo binário **não há rótulos**: só bytes. Quem lê precisa saber exatamente **o que** foi gravado, **em que ordem** e **com qual tamanho**. Gravou `int, double, string` → leia `ReadInt32, ReadDouble, ReadString`. Qualquer divergência produz lixo.
:::

::: exercicio Agenda em arquivo texto
Crie um programa de console que:

1. Pergunta nome e telefone e **acrescenta** uma linha `nome;telefone` ao arquivo `agenda.txt`;
2. Ao digitar `L`, **lista** todos os contatos lidos do arquivo.

Use `using` em todos os streams.

--- resposta
```cs
string arquivo = "agenda.txt";
while (true)
{
    Console.Write("Nome (ou L para listar, X para sair): ");
    string nome = Console.ReadLine();
    if (nome.ToUpper() == "X") break;

    if (nome.ToUpper() == "L")
    {
        if (File.Exists(arquivo))
            foreach (string linha in File.ReadAllLines(arquivo))
            {
                string[] p = linha.Split(';');
                Console.WriteLine(p[0] + " — " + p[1]);
            }
        continue;
    }

    Console.Write("Telefone: ");
    string tel = Console.ReadLine();
    using (StreamWriter sw = new StreamWriter(arquivo, true))   // true = acrescentar (append)
        sw.WriteLine(nome + ";" + tel);
}
```
O segundo parâmetro `true` do `StreamWriter` faz **acrescentar** no fim; sem ele o arquivo seria sobrescrito a cada execução.
:::

::: resumo
- **Streams** são canais de bytes; `StreamWriter`/`StreamReader` cuidam de **texto**, `BinaryWriter/Reader` de **tipos primitivos**.
- `File`, `Directory` e `Path` fazem as tarefas comuns em uma linha.
- Streams precisam ser fechados: use **`using`** (`IDisposable`).
- `switch` escolhe entre várias opções; `default` cobre o resto.
- **Serializar** = transformar o objeto (e tudo que ele referencia) em bytes/texto; hoje prefira **JSON**.
- Arquivos são bytes; **Unicode** mapeia caracteres para números; **hexadecimal** é a forma compacta de ver bytes.
:::

::: quiz
? O que é um stream?
- Um tipo de banco de dados
+ Um canal pelo qual bytes ou texto fluem de uma origem a um destino
- Uma extensão de arquivo
- Um tipo de coleção
> Streams abstraem leitura e escrita sequenciais.

? Qual classe é indicada para escrever linhas de texto em um arquivo?
- `BinaryReader`
- `Directory`
+ `StreamWriter`
- `Random`
> `StreamWriter` converte texto em bytes e escreve num stream.

? O que `using (StreamWriter sw = ...) { ... }` garante?
+ Que `Dispose()` é chamado ao sair do bloco, mesmo com exceção
- Que o arquivo seja apagado
- Que o arquivo seja criptografado
- Que o arquivo seja lido duas vezes
> `using` implementa o padrão de liberar o recurso automaticamente.

? O que acontece no `switch` se um `case` não termina com `break`?
- Executa o próximo case
+ Erro de compilação
- Ignora o case
- O programa trava em tempo de execução
> O C# exige que cada case termine explicitamente.

? Qual o objetivo da serialização?
+ Converter um objeto (e o que ele referencia) em bytes/texto para salvar ou transmitir
- Ordenar listas
- Compactar o código-fonte
- Impedir cópias do objeto
> A desserialização reconstrói o objeto.

? Por que o `BinaryFormatter` não deve ser usado em projetos novos?
+ É obsoleto e inseguro com dados não confiáveis
- Porque só funciona com `int`
- Porque cria arquivos enormes
- Porque exige internet
> Prefira `System.Text.Json` (ou outro serializador seguro).

? Quantos dígitos hexadecimais representam um byte?
- 1
+ 2
- 4
- 8
> 8 bits = 2 dígitos hexa (ex.: 0x48).

? Ao ler um arquivo binário gravado com `BinaryWriter`, o que é essencial?
+ Ler os mesmos tipos, na mesma ordem em que foram gravados
- Ler sempre como string
- Usar `StreamWriter`
- Nada; o arquivo se auto-descreve
> Não há rótulos no binário: o leitor deve conhecer o formato.
:::

::: cartas
Stream :: Canal sequencial de dados entre um programa e uma origem/destino (arquivo, memória, rede).
FileStream :: Stream que lê/escreve bytes em um arquivo.
StreamWriter / StreamReader :: Escrevem/leem texto sobre um stream.
File / Directory :: Classes estáticas utilitárias para arquivos e pastas.
IDisposable :: Interface com `Dispose()` para liberar recursos.
using :: Instrução que chama Dispose automaticamente ao fim do bloco.
switch :: Escolhe um entre vários caminhos conforme o valor de uma expressão.
Serialização :: Converter um objeto e suas referências em bytes/texto.
Desserialização :: Reconstruir o objeto a partir dos bytes/texto salvos.
Unicode / UTF-8 :: Padrão que mapeia caracteres para números / forma de gravá-los em bytes.
Hexadecimal :: Base 16: dois dígitos representam um byte.
BinaryWriter / BinaryReader :: Gravam/leem tipos primitivos em formato binário.
:::
