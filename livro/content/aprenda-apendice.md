---
id: aprenda-apendice
order: 29
kind: apendice
group: C# · Parte 6 · Gráficos, memória e LINQ
title: Sobras — o que não coube no livro (Aprenda C#)
subtitle: Cinco tópicos para continuar estudando — LINQ to XML, refatoração, componentes úteis, aplicativos de console e WPF — e para onde ir depois.
minutes: 35
hue: 300
badge: A
objectives:
- Conhecer **cinco tópicos** importantes que ficam como "próximos passos"
- Dar um passo prático em cada um, com um exemplo curto
- Montar um **roteiro de estudos** depois deste livro
origem: livros/01-aprenda-csharp/content/19-apendice.md
---

::: ilustracao img/aprenda/apendice.svg | Sobras: cinco caminhos para continuar estudando depois deste livro.
:::

## A diversão está apenas começando

Você viu ferramentas ótimas para construir programas poderosos em C#. Mas nenhum livro cabe **todas** as ferramentas, tecnologias e técnicas. Estes são os cinco tópicos que mais merecem uma **pequena introdução** para você seguir sozinho.

## 1. LINQ to XML

XML é um formato de texto hierárquico, usado em configurações e integrações. O **LINQ to XML** (`System.Xml.Linq`) permite **criar** e **consultar** XML com a mesma sintaxe do LINQ.

```cs
using System.Xml.Linq;

XElement catalogo = new XElement("catalogo",
    new XElement("quadrinho", new XAttribute("ano", 1962),
        new XElement("titulo", "Aventuras Espaciais #1"), new XElement("preco", 4500)),
    new XElement("quadrinho", new XAttribute("ano", 1988),
        new XElement("titulo", "Mistério na Selva #3"), new XElement("preco", 90)));

catalogo.Save("catalogo.xml");

// consulta
var antigos = from q in catalogo.Elements("quadrinho")
              where (int) q.Attribute("ano") < 1980
              select (string) q.Element("titulo");
```

Gera:

```xml
<catalogo>
  <quadrinho ano="1962">
    <titulo>Aventuras Espaciais #1</titulo>
    <preco>4500</preco>
  </quadrinho>
</catalogo>
```

::: dica E o JSON?
Hoje o **JSON** é mais usado que o XML. A biblioteca `System.Text.Json` (e `JsonNode`) faz o papel equivalente: serializar, consultar e transformar.
:::

## 2. Refatoração

**Refatorar** é melhorar a **estrutura do código sem mudar o comportamento**. O Visual Studio tem ferramentas prontas (menu **Editar → Refatorar**, ou o ícone de lâmpada 💡 / [[Ctrl]]+[[.]]):

| Refatoração | O que faz |
|---|---|
| **Renomear** ([[Ctrl]]+[[R]], [[R]]) | Muda o nome de variável/método/classe em **todos** os usos, com segurança |
| **Extrair método** | Transforma um trecho selecionado em um método com nome |
| **Extrair interface** | Cria uma interface a partir dos membros públicos de uma classe |
| **Encapsular campo** | Transforma um campo público em propriedade |
| **Remover parâmetros / reordenar** | Ajusta a assinatura e atualiza os chamadores |

```cs
// antes: um método longo, com um bloco que calcula o desconto
decimal total = 0;
foreach (var item in itens) total += item.Preco * item.Quantidade;
if (total > 500) total *= 0.9m;

// depois: "extrair método" dá nome à ideia
decimal total = CalcularTotalComDesconto(itens);
```

::: regra Refatore com testes
Antes de refatorar, tenha **como verificar** que nada quebrou (testes automatizados, ou ao menos um roteiro manual). Refatorar sem rede é pedir bug.
:::

## 3. Alguns componentes favoritos da Caixa de Ferramentas

| Componente | Para que serve |
|---|---|
| `BackgroundWorker` | Executar trabalho **pesado em segundo plano** sem congelar a tela |
| `FileSystemWatcher` | Reagir quando um arquivo/pasta é criado, alterado ou apagado |
| `PerformanceCounter` | Ler contadores de desempenho do Windows (CPU, memória…) |
| `NotifyIcon` | Ícone na bandeja do sistema |
| `ErrorProvider` | Mostrar ícone de erro ao lado de campos inválidos |
| `ToolTip` | Dicas ao passar o mouse |
| `DataGridView` | Grade de dados (planilha) ligada a coleções e bancos |

```cs
// FileSystemWatcher: avisa quando algo muda na pasta
var watcher = new FileSystemWatcher(@"C:\Temp", "*.txt");
watcher.Created += (s, e) => Console.WriteLine("Novo arquivo: " + e.Name);
watcher.EnableRaisingEvents = true;
```

::: dica Hoje: `async` / `await`
Para trabalho pesado sem travar a tela, o jeito moderno é `async`/`await`:
```cs
private async void botao_Click(object sender, EventArgs e)
{
    botao.Enabled = false;
    string conteudo = await File.ReadAllTextAsync("grande.txt");   // não congela a janela
    label1.Text = "Li " + conteudo.Length + " caracteres";
    botao.Enabled = true;
}
```
:::

## 4. Aplicativos de console

Nem tudo precisa de janela. **Aplicativos de console** rodam no terminal: ferramentas, scripts, serviços, automações.

```cs
// .NET 6+: instruções de nível superior (sem Main explícito)
Console.Write("Seu nome: ");
string nome = Console.ReadLine();
Console.WriteLine($"Olá, {nome}!");          // interpolação de strings

foreach (string arg in args)                  // argumentos da linha de comando
    Console.WriteLine("arg: " + arg);
```

| Método | Faz |
|---|---|
| `Console.WriteLine(x)` | Escreve e quebra a linha |
| `Console.ReadLine()` | Lê uma linha digitada |
| `Console.ReadKey()` | Lê uma tecla |
| `Console.ForegroundColor` | Muda a cor do texto |
| `Environment.Exit(code)` | Encerra com um código de saída |

```bash
dotnet new console -n MeuApp
cd MeuApp
dotnet run
```

## 5. Windows Presentation Foundation (WPF)

O **WPF** é a tecnologia de interface mais moderna da época do livro: a tela é descrita em **XAML** (um XML), com **vinculação de dados**, **estilos**, **animações** e gráficos **vetoriais** escaláveis.

```xml
<Window x:Class="MeuApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        Title="Olá WPF" Height="200" Width="320">
    <StackPanel Margin="16">
        <TextBox x:Name="nomeBox" />
        <Button Content="Cumprimentar" Click="Cumprimentar_Click" Margin="0,8,0,0" />
        <TextBlock x:Name="resultado" FontSize="20" />
    </StackPanel>
</Window>
```

```cs
private void Cumprimentar_Click(object sender, RoutedEventArgs e)
{
    resultado.Text = "Olá, " + nomeBox.Text + "!";
}
```

::: curiosidade E hoje?
Hoje, além de Windows Forms e WPF, há **.NET MAUI** (aplicativos para Windows, Android, iOS e macOS), **Blazor** (web com C#), **ASP.NET Core** (APIs e sites) e **Unity** (jogos com C#). Os conceitos do livro — classes, herança, interfaces, coleções, eventos, LINQ — são exatamente os mesmos em todos eles.
:::

## Você sabia que, com C# e o .NET, você pode…

- Criar **sites e APIs** (ASP.NET Core)
- Criar **jogos** 2D/3D (Unity, MonoGame)
- Criar **aplicativos móveis** (.NET MAUI)
- Criar **serviços na nuvem** (Azure Functions) e **contêineres**
- **Automatizar** tarefas com aplicativos de console
- Trabalhar com **aprendizado de máquina** (ML.NET)

## Um roteiro para os próximos passos

::: passos Depois deste livro
**1. Consolide (1–2 semanas).** Refaça um laboratório sem olhar as respostas. Revise os cartões. Releia os capítulos em que errou o quiz.
===
**2. Escolha um projeto próprio (2–4 semanas).** Controle de gastos, agenda, jogo simples, automação de planilhas. Um projeto **seu** vale mais que dez tutoriais.
===
**3. Aprofunde a linguagem.** Genéricos próprios, `async`/`await`, expressões lambda avançadas, `record`, *pattern matching*, `nullable reference types`.
===
**4. Aprenda a testar e versionar.** xUnit/NUnit para testes automatizados; **Git** e GitHub para versionamento.
===
**5. Escolha uma trilha.** Web (ASP.NET Core), desktop (WPF/MAUI), jogos (Unity), dados/nuvem (EF Core, Azure).
:::

::: exercicio Seu mini-projeto de conclusão
Escolha **uma** ideia e escreva, em 5 linhas: (1) qual problema resolve; (2) quais classes terá; (3) o que cada uma *sabe* e *faz*; (4) onde usará herança ou interface; (5) como vai salvar os dados.

--- resposta
Não há resposta única. Um bom sinal de que o desenho está correto: você consegue explicar o programa **só olhando o diagrama de classes**, sem abrir o código.
:::

::: resumo
- **LINQ to XML** cria e consulta XML (hoje, JSON em muitos casos).
- **Refatorar** melhora a estrutura sem mudar o comportamento — use as ferramentas da IDE e tenha testes.
- Componentes como **BackgroundWorker**, **FileSystemWatcher** e **DataGridView** economizam trabalho; hoje use também `async`/`await`.
- **Console** serve para automações e ferramentas; **WPF** traz interfaces ricas com XAML.
- Os conceitos do livro valem para **qualquer** plataforma .NET.
:::

::: quiz
? O que é refatoração?
+ Melhorar a estrutura do código sem mudar seu comportamento
- Adicionar novas funcionalidades
- Trocar a linguagem do projeto
- Apagar código que não compila
> O comportamento externo permanece igual.

? Qual componente executa trabalho em segundo plano para a tela não travar?
- `Timer`
+ `BackgroundWorker` (ou `async`/`await`)
- `PictureBox`
- `ToolTip`
> Tarefas longas não devem rodar na thread da interface.

? Em que o WPF descreve a interface?
- Em arquivos `.resx`
+ Em XAML (um formato baseado em XML)
- Em arquivos `.sql`
- Em arquivos binários
> XAML é a linguagem declarativa de interface do WPF.

? Qual comando cria um novo aplicativo de console pela linha de comando?
- `dotnet make console`
+ `dotnet new console -n MeuApp`
- `csc console`
- `msbuild new`
> A CLI do .NET cria projetos a partir de templates.
:::

::: cartas
LINQ to XML :: API (System.Xml.Linq) para criar e consultar XML com LINQ.
Refatoração :: Melhorar a estrutura do código sem alterar o comportamento.
BackgroundWorker :: Componente que executa trabalho pesado em segundo plano.
FileSystemWatcher :: Dispara eventos quando arquivos/pastas mudam.
Aplicativo de console :: Programa executado no terminal, sem janelas.
WPF / XAML :: Tecnologia de interface com layout declarativo em XAML.
async / await :: Sintaxe moderna para operações assíncronas sem travar a interface.
:::
