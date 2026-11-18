---
id: compromissos-cap74
order: 98
kind: capitulo
num: 74
group: C# & Mentalidade · Oficina · O Caderno dos Quatro Compromissos
title: Os acordos que chegaram antes de nós
subtitle: Uma experiência do tema, uma escolha e uma nova capacidade no mesmo programa.
minutes: 45
hue: 168
objectives:
- Compreender a ideia estudada e seu contexto na obra de Ruiz
- Construir a capacidade deste capítulo no Caderno
- Executar o cenário, variar uma entrada e explicar seu resultado
- Distinguir a proposta do autor das regras do software
origem: livros/05-quatro-compromissos-csharp/content/110-cap74.md
---

::: vila Uma página da nossa história
Lia recebe uma mensagem no jantar. Responde antes mesmo de terminar de ler. Quando Caio pergunta por quê, ela diz: “Se eu demorar, vão achar que não me importo”. Ninguém combinou esse prazo. Ainda assim, ela vive como se tivesse assinado um contrato. Hoje não vamos julgá-la: vamos tornar esse acordo visível.
:::

::: ilustracao img/compromissos/jornada74.svg | Os acordos que chegaram antes de nós: uma escolha visível e sua consequência no caderno.
:::

## A ideia que vamos experimentar

Ruiz chama de domesticação o aprendizado de crenças e regras através da linguagem, da aprovação e da punição. O “sonho do planeta” é sua maneira espiritual de falar de um mundo de significados compartilhados. Sua proposta começa com perceber os acordos que aceitamos antes de escolher conscientemente. Não se trata apenas de promessas feitas em voz alta: um “tenho que agradar a todos” também pode orientar a vida.

**Fonte de leitura:** Don Miguel Ruiz, *Os Quatro Compromissos*, páginas físicas 5–13 do PDF fornecido. Este texto é uma explicação própria, não uma citação literal. A paginação de outras edições pode variar.

## Do tema para uma decisão no programa

Nosso primeiro objeto representará um registro desse inventário. Ele guarda a frase, a origem informada e se a pessoa escolheu mantê-la. Registrar não significa aprovar. Essa diferença será visível: o acordo entra com `Escolhido = false`. Um identificador mantém a identidade mesmo que, mais tarde, façamos uma nova versão dos dados.

::: pense Antes de executar
Descreva o que você espera ver mudar. Qual informação já existe? Qual ainda não existe? O que o programa deve recusar? Escreva sua previsão antes de olhar a saída.
:::

## Vamos fazer acontecer

Execute **a partir da pasta deste livro**. Todos os comandos cap74 a cap87 existem no mesmo projeto. Os cenários começam com dados novos e fictícios para serem reproduzíveis; o diário pessoal persistente é apresentado no laboratório.

```bash
dotnet run --project codigo/compromissos/Caderno.Console -- cap74
```

O trecho abaixo está dentro do método `Cap74` em `codigo/compromissos/Caderno.Console/Exemplos.cs`. Usa `using Quatro;`. É um corpo de método, não um arquivo independente: não o cole ao lado das declarações das classes.

```cs
var caderno = new Caderno();
var acordo = caderno.Inventario.Registrar("Responder imediatamente", "Hábito do grupo");
Console.WriteLine($"Acordos registrados: {caderno.Inventario.Todos.Count}");
Console.WriteLine($"Escolhido por mim: {acordo.Escolhido}");
```

### Leia o que o código faz

`Acordo` descreve um item; `Inventario` guarda e encontra muitos itens. A lista fica privada porque queremos controlar como os registros entram. `Todos` devolve uma cópia do conjunto: alterar essa cópia não troca os itens internos. O `record` permite comparar dados; `with` produz uma nova versão. Não recomeçamos uma aula inteira de classes: damos a elas uma responsabilidade concreta.

Observe os verbos do código: registrar, perguntar, escolher, cumprir, salvar. Cada um deve produzir uma mudança que você consiga explicar. Execute de novo: nos cenários, os dados são recriados. Para conservar sua própria prática, use os comandos do laboratório.

### A saída real desta experiência

```saida
Acordos registrados: 1
Escolhido por mim: False
```

Compare sua previsão com a execução. Não basta obter a mesma tela: explique qual instrução produziu cada linha e qual objeto foi consultado. Troque apenas uma entrada por vez para observar a causa da diferença.

## Por dentro da capacidade

O arquivo de referência é `codigo/compromissos/Caderno.Core/Inventario.cs`. Alguns membros serão usados nos próximos capítulos; hoje concentre-se no caminho percorrido pela experiência. A definição completa evita que o leitor precise adivinhar membros omitidos.

```cs
namespace Quatro;

public sealed record Acordo(Guid Id, string Texto, string Origem, bool Escolhido);

public sealed class Inventario
{
    private readonly List<Acordo> acordos = [];
    public IReadOnlyList<Acordo> Todos => acordos.ToArray();

    public Acordo Registrar(string texto, string origem)
    {
        var acordo = new Acordo(Guid.NewGuid(), Regras.Texto(texto),
            Regras.Texto(origem), false);
        acordos.Add(acordo);
        return acordo;
    }

    public void Escolher(Guid id)
    {
        int i = acordos.FindIndex(a => a.Id == id);
        if (i < 0) throw new KeyNotFoundException("Acordo não encontrado.");
        acordos[i] = acordos[i] with { Escolhido = true };
    }

    public IReadOnlyList<Acordo> Buscar(string texto)
    {
        string termo = Regras.Texto(texto);
        return acordos.Where(a => a.Texto.Contains(termo,
            StringComparison.OrdinalIgnoreCase)).ToArray();
    }
}
```



## Agora escolha outro caminho

::: exercicio Experimente e explique
Registre “Preciso responder imediatamente” e busque “RESPONDER”. Quantos resultados aparecem? Depois escolha o acordo usando seu Id e consulte novamente.

Altere o método Cap74 em uma cópia do projeto, execute o mesmo comando e compare com sua previsão. Quando houver uma exceção esperada, capture-a com `catch (ArgumentException erro)` ou `catch (InvalidOperationException erro)`, conforme a regra estudada, e mostre `erro.Message`.
--- resposta
A busca encontra um registro sem distinguir maiúsculas e minúsculas. Escolher troca o registro por uma cópia com o mesmo Id e `Escolhido = true`; a referência guardada antes continua sendo a versão anterior.

Procure o teste correspondente em `codigo/compromissos/Caderno.Testes/Program.cs`. Os testes exercitam entradas, mudanças e falhas. Eles não comprovam a filosofia do autor: verificam o contrato que escolhemos implementar.
:::

## Limite da analogia

A classe não detecta crenças inconscientes e não diagnostica uma pessoa. A origem é declarada pelo usuário. `Escolhido` registra uma decisão no caderno, não a transformação completa de um hábito.

## O que entrou na sua mochila

`Acordo` descreve um item; `Inventario` guarda e encontra muitos itens. Agora você consegue relacionar uma necessidade humana, uma regra escolhida e um resultado executável. Na próxima etapa reutilizaremos os objetos; não recomeçaremos um curso de sintaxe.

::: quiz Confira sua leitura e sua execução
? Registrar um acordo significa aprová-lo?
+ Não; ele começa não escolhido.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Por que guardar Origem?
+ Para registrar de onde a pessoa percebe ter recebido a regra.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? O que permanece ao usar Escolher?
+ O Id do acordo permanece; uma nova versão dos dados é guardada.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? O que Buscar devolve quando não encontra?
+ Uma coleção vazia.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Registre “Preciso responder imediatamente” e busque “RESPONDER”?
+ A busca encontra um registro sem distinguir maiúsculas e minúsculas.
- O programa avalia a intenção da pessoa
- Nada pode ser observado pela execução
- A linguagem decide o sentido do texto
> Reproduza a variação descrita no exercício e acompanhe o estado.

? Qual limite precisa acompanhar esta experiência?
+ A classe não detecta crenças inconscientes e não diagnostica uma pessoa. A origem é declarada pelo usuário. `Escolhido` registra uma decisão no caderno, não a transformação completa de um hábito.
- O estado do programa certifica o desenvolvimento pessoal.
- O código define como toda pessoa deve sentir e agir.
- Não precisamos distinguir texto e aplicação.
> A utilidade da comparação depende de manter claro o que o modelo representa e o que não representa.
:::

::: cartas Lembre sem decorar
Identidade :: `Acordo` descreve um item; `Inventario` guarda e encontra muitos itens.
coleção privada :: Nosso primeiro objeto representará um registro desse inventário.
record :: A classe não detecta crenças inconscientes e não diagnostica uma pessoa.
busca :: A busca encontra um registro sem distinguir maiúsculas e minúsculas.
Passagem estudada :: Páginas físicas 5–13 do PDF recebido; explicação e exemplos são próprios.
Experiência executável :: dotnet run --project codigo/compromissos/Caderno.Console -- cap74
Variação para experimentar :: Registre “Preciso responder imediatamente” e busque “RESPONDER”. Quantos resultados aparecem? Depois escolha o acordo usando seu Id e consulte novamente.
Limite desta comparação :: A classe não detecta crenças inconscientes e não diagnostica uma pessoa. A origem é declarada pelo usuário. `Escolhido` registra uma decisão no caderno, não a transformação completa de um hábito.
:::
