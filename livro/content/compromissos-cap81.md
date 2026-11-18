---
id: compromissos-cap81
order: 105
kind: capitulo
num: 81
group: C# & Mentalidade · Oficina · O Caderno dos Quatro Compromissos
title: Perguntar, ouvir e guardar a fonte
subtitle: Uma experiência do tema, uma escolha e uma nova capacidade no mesmo programa.
minutes: 45
hue: 168
objectives:
- Compreender a ideia estudada e seu contexto na obra de Ruiz
- Construir a capacidade deste capítulo no Caderno
- Executar o cenário, variar uma entrada e explicar seu resultado
- Distinguir a proposta do autor das regras do software
origem: livros/05-quatro-compromissos-csharp/content/117-cap81.md
---

::: vila Uma página da nossa história
Lia responde: “Pode ser um PDF, não precisa imprimir”. Caio não ganhou apenas um texto: ganhou uma razão para mudar o plano. O caderno deve registrar o esclarecimento e de onde ele veio. Se alguém perguntar amanhã por que não houve impressão, a resposta não será “achei que era assim”.
:::

::: ilustracao img/compromissos/jornada81.svg | Perguntar, ouvir e guardar a fonte: uma escolha visível e sua consequência no caderno.
:::

## A ideia que vamos experimentar

No terceiro compromisso, a alternativa à presunção envolve comunicação clara. O movimento é sair do significado que inventei e abrir espaço para a resposta do outro. Traduzimos uma parte desse exercício em rastreabilidade: a pergunta, a resposta registrada e a fonte declarada ficam juntas.

**Fonte de leitura:** Don Miguel Ruiz, *Os Quatro Compromissos*, páginas físicas 27–31 do PDF fornecido. Este texto é uma explicação própria, não uma citação literal. A paginação de outras edições pode variar.

## Do tema para uma decisão no programa

Usamos a mesma Pergunta do capítulo anterior. `Esclarecer` recebe se a hipótese foi confirmada, a resposta e sua fonte. Uma resposta negativa com fonte não é uma falha do sistema: é uma conclusão possível. Reabrir a questão exigirá outra pergunta, preservando o registro anterior.

::: pense Antes de executar
Descreva o que você espera ver mudar. Qual informação já existe? Qual ainda não existe? O que o programa deve recusar? Escreva sua previsão antes de olhar a saída.
:::

## Vamos fazer acontecer

Execute **a partir da pasta deste livro**. Todos os comandos cap74 a cap87 existem no mesmo projeto. Os cenários começam com dados novos e fictícios para serem reproduzíveis; o diário pessoal persistente é apresentado no laboratório.

```bash
dotnet run --project codigo/compromissos/Caderno.Console -- cap81
```

O trecho abaixo está dentro do método `Cap81` em `codigo/compromissos/Caderno.Console/Exemplos.cs`. Usa `using Quatro;`. É um corpo de método, não um arquivo independente: não o cole ao lado das declarações das classes.

```cs
var caderno = new Caderno();
var pergunta = caderno.Perguntar("A entrega precisa ser impressa?");
pergunta.Esclarecer(false, "Pode ser em PDF", "Conversa com Lia em 05/11");
Console.WriteLine(pergunta.Estado);
Console.WriteLine($"Fonte: {pergunta.Evidencia?.Fonte}");
```

### Leia o que o código faz

A evidência é construída antes de mudar o estado. Se a fonte estiver vazia, o método lança uma exceção e a pergunta continua pendente. Essa ordem é verificável. O operador `?.` permite ler a fonte somente se a evidência existir. Evitamos o operador `!` aqui porque uma pergunta pendente é parte normal do fluxo.

Observe os verbos do código: registrar, perguntar, escolher, cumprir, salvar. Cada um deve produzir uma mudança que você consiga explicar. Execute de novo: nos cenários, os dados são recriados. Para conservar sua própria prática, use os comandos do laboratório.

### A saída real desta experiência

```saida
Refutada
Fonte: Conversa com Lia em 05/11
```

Compare sua previsão com a execução. Não basta obter a mesma tela: explique qual instrução produziu cada linha e qual objeto foi consultado. Troque apenas uma entrada por vez para observar a causa da diferença.

## Por dentro da capacidade

O arquivo de referência é `codigo/compromissos/Caderno.Core/Pergunta.cs`. Alguns membros serão usados nos próximos capítulos; hoje concentre-se no caminho percorrido pela experiência. A definição completa evita que o leitor precise adivinhar membros omitidos.

```cs
namespace Quatro;

public enum EstadoDaPergunta { Pendente, Confirmada, Refutada }
public sealed record Evidencia(string Resposta, string Fonte);

public sealed class Pergunta
{
    public string Texto { get; }
    public EstadoDaPergunta Estado { get; private set; }
    public Evidencia? Evidencia { get; private set; }

    public Pergunta(string texto) => Texto = Regras.Texto(texto);

    public void Esclarecer(bool confirmou, string resposta, string fonte)
    {
        if (Estado != EstadoDaPergunta.Pendente)
            throw new InvalidOperationException("Registre outra pergunta para uma nova investigação.");
        var evidencia = new Evidencia(Regras.Texto(resposta), Regras.Texto(fonte));
        Evidencia = evidencia;
        Estado = confirmou ? EstadoDaPergunta.Confirmada : EstadoDaPergunta.Refutada;
    }
}
```



## Agora escolha outro caminho

::: exercicio Experimente e explique
Chame Esclarecer com uma resposta válida e fonte vazia. Consulte Estado e Evidencia após capturar o erro.

Altere o método Cap81 em uma cópia do projeto, execute o mesmo comando e compare com sua previsão. Quando houver uma exceção esperada, capture-a com `catch (ArgumentException erro)` ou `catch (InvalidOperationException erro)`, conforme a regra estudada, e mostre `erro.Message`.
--- resposta
O estado continua Pendente e a evidência continua nula. Não há confirmação parcial. Uma segunda conclusão sobre uma pergunta já esclarecida também é recusada.

Procure o teste correspondente em `codigo/compromissos/Caderno.Testes/Program.cs`. Os testes exercitam entradas, mudanças e falhas. Eles não comprovam a filosofia do autor: verificam o contrato que escolhemos implementar.
:::

## Limite da analogia

Fonte informada não é fonte autenticada. Uma conclusão pode estar errada ou envelhecer. O modelo registra o que a pessoa apurou; não certifica verdade e não substitui avaliação crítica.

## O que entrou na sua mochila

A evidência é construída antes de mudar o estado. Agora você consegue relacionar uma necessidade humana, uma regra escolhida e um resultado executável. Na próxima etapa reutilizaremos os objetos; não recomeçaremos um curso de sintaxe.

::: quiz Confira sua leitura e sua execução
? Sem fonte, o método confirma a pergunta?
+ Não; a validação falha antes de mudar o estado.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Uma resposta negativa com fonte é erro?
+ Não; Refutada é uma conclusão possível.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? O registro autentica a fonte?
+ Não; a fonte é declarada por quem registra.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Como preservar uma revisão da conclusão?
+ Abra outra pergunta em vez de sobrescrever a anterior.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Chame Esclarecer com uma resposta válida e fonte vazia?
+ O estado continua Pendente e a evidência continua nula.
- O programa avalia a intenção da pessoa
- Nada pode ser observado pela execução
- A linguagem decide o sentido do texto
> Reproduza a variação descrita no exercício e acompanhe o estado.

? Qual limite precisa acompanhar esta experiência?
+ Fonte informada não é fonte autenticada. Uma conclusão pode estar errada ou envelhecer. O modelo registra o que a pessoa apurou; não certifica verdade e não substitui avaliação crítica.
- O estado do programa certifica o desenvolvimento pessoal.
- O código define como toda pessoa deve sentir e agir.
- Não precisamos distinguir texto e aplicação.
> A utilidade da comparação depende de manter claro o que o modelo representa e o que não representa.
:::

::: cartas Lembre sem decorar
Evidência :: A evidência é construída antes de mudar o estado.
rastreabilidade :: Usamos a mesma Pergunta do capítulo anterior.
validação antes da mudança :: Fonte informada não é fonte autenticada.
acesso condicional :: O estado continua Pendente e a evidência continua nula.
Passagem estudada :: Páginas físicas 27–31 do PDF recebido; explicação e exemplos são próprios.
Experiência executável :: dotnet run --project codigo/compromissos/Caderno.Console -- cap81
Variação para experimentar :: Chame Esclarecer com uma resposta válida e fonte vazia. Consulte Estado e Evidencia após capturar o erro.
Limite desta comparação :: Fonte informada não é fonte autenticada. Uma conclusão pode estar errada ou envelhecer. O modelo registra o que a pessoa apurou; não certifica verdade e não substitui avaliação crítica.
:::
