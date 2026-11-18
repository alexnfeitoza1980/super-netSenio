---
id: compromissos-cap78
order: 102
kind: capitulo
num: 78
group: C# & Mentalidade · Oficina · O Caderno dos Quatro Compromissos
title: Uma opinião não precisa virar sua identidade
subtitle: Uma experiência do tema, uma escolha e uma nova capacidade no mesmo programa.
minutes: 45
hue: 168
objectives:
- Compreender a ideia estudada e seu contexto na obra de Ruiz
- Construir a capacidade deste capítulo no Caderno
- Executar o cenário, variar uma entrada e explicar seu resultado
- Distinguir a proposta do autor das regras do software
origem: livros/05-quatro-compromissos-csharp/content/114-cap78.md
---

::: vila Uma página da nossa história
Lia lê “Não gostei do texto”. Na mesma hora pensa “Ninguém gosta de mim”. São duas frases diferentes, mas a segunda ocupa todo o espaço. Ela abre o caderno e coloca a mensagem recebida ao lado da leitura que fez. A pergunta seguinte pode ser simples: “Que trecho você mudaria?”.
:::

::: ilustracao img/compromissos/jornada78.svg | Uma opinião não precisa virar sua identidade: uma escolha visível e sua consequência no caderno.
:::

::: html
<figure style="max-width:420px;margin:2rem auto"><div style="overflow:hidden;aspect-ratio:3/4;border-radius:24px"><img src="img/compromissos/quatro-jornada.png" alt="Uma mensagem não define quem você é" style="display:block;width:400%;max-width:none;transform:translateX(-25%)"></div><figcaption>Uma mensagem não define quem você é. Uma imagem para acompanhar a experiência deste compromisso.</figcaption></figure>
:::

## A ideia que vamos experimentar

O segundo compromisso convida a perceber que a fala do outro passa por seu próprio mundo de crenças. O autor aplica isso tanto à crítica quanto ao elogio. Essa proposta busca reduzir a dependência da aprovação alheia. Não exige negar o que sentimos, aceitar desrespeito ou ignorar uma informação útil.

**Fonte de leitura:** Don Miguel Ruiz, *Os Quatro Compromissos*, páginas físicas 22–26 do PDF fornecido. Este texto é uma explicação própria, não uma citação literal. A paginação de outras edições pode variar.

## Do tema para uma decisão no programa

O objeto guarda mensagem recebida, leitura pessoal, resposta escolhida e limite. A diferença de campos é a experiência central: aquilo que ouvi não é idêntico ao significado que atribuí. A pessoa decide sua resposta. Um campo de limite permite registrar, por exemplo, que aceita discutir o trabalho sem continuar uma conversa com insultos.

::: pense Antes de executar
Descreva o que você espera ver mudar. Qual informação já existe? Qual ainda não existe? O que o programa deve recusar? Escreva sua previsão antes de olhar a saída.
:::

## Vamos fazer acontecer

Execute **a partir da pasta deste livro**. Todos os comandos cap74 a cap87 existem no mesmo projeto. Os cenários começam com dados novos e fictícios para serem reproduzíveis; o diário pessoal persistente é apresentado no laboratório.

```bash
dotnet run --project codigo/compromissos/Caderno.Console -- cap78
```

O trecho abaixo está dentro do método `Cap78` em `codigo/compromissos/Caderno.Console/Exemplos.cs`. Usa `using Quatro;`. É um corpo de método, não um arquivo independente: não o cole ao lado das declarações das classes.

```cs
var caderno = new Caderno();
var resposta = new RespostaEscolhida("Não gostei do texto", "Acho que não gostam de mim",
    "Que trecho precisa melhorar?", "Posso ouvir críticas sem aceitar insultos");
caderno.Responder(resposta);
Console.WriteLine($"Recebido: {resposta.MensagemRecebida}");
Console.WriteLine($"Responderei: {resposta.Resposta}");
```

### Leia o que o código faz

Usamos composição: o Caderno guarda uma RespostaEscolhida. Não fazemos uma classe Pessoa herdar de Crítica, porque receber uma mensagem não transforma sua identidade em um tipo da linguagem. O construtor valida os quatro textos e o registro permanece estável. A saída mostra apenas mensagem e resposta, evitando imprimir toda a reflexão pessoal por padrão.

Observe os verbos do código: registrar, perguntar, escolher, cumprir, salvar. Cada um deve produzir uma mudança que você consiga explicar. Execute de novo: nos cenários, os dados são recriados. Para conservar sua própria prática, use os comandos do laboratório.

### A saída real desta experiência

```saida
Recebido: Não gostei do texto
Responderei: Que trecho precisa melhorar?
```

Compare sua previsão com a execução. Não basta obter a mesma tela: explique qual instrução produziu cada linha e qual objeto foi consultado. Troque apenas uma entrada por vez para observar a causa da diferença.

## Por dentro da capacidade

O arquivo de referência é `codigo/compromissos/Caderno.Core/Resposta.cs`. Alguns membros serão usados nos próximos capítulos; hoje concentre-se no caminho percorrido pela experiência. A definição completa evita que o leitor precise adivinhar membros omitidos.

```cs
namespace Quatro;

public sealed record RespostaEscolhida
{
    public string MensagemRecebida { get; }
    public string LeituraPessoal { get; }
    public string Resposta { get; }
    public string Limite { get; }

    public RespostaEscolhida(string mensagemRecebida, string leituraPessoal,
        string resposta, string limite)
    {
        MensagemRecebida = Regras.Texto(mensagemRecebida);
        LeituraPessoal = Regras.Texto(leituraPessoal);
        Resposta = Regras.Texto(resposta);
        Limite = Regras.Texto(limite);
    }
}
```



## Agora escolha outro caminho

::: exercicio Experimente e explique
Registre um elogio em vez de uma crítica. Escreva uma leitura e uma resposta que não dependam de transformar esse elogio em definição de quem você é.

Altere o método Cap78 em uma cópia do projeto, execute o mesmo comando e compare com sua previsão. Quando houver uma exceção esperada, capture-a com `catch (ArgumentException erro)` ou `catch (InvalidOperationException erro)`, conforme a regra estudada, e mostre `erro.Message`.
--- resposta
Os mesmos campos funcionam. Isso mostra que o modelo trata a interpretação de mensagens, e não apenas um filtro de frases negativas.

Procure o teste correspondente em `codigo/compromissos/Caderno.Testes/Program.cs`. Os testes exercitam entradas, mudanças e falhas. Eles não comprovam a filosofia do autor: verificam o contrato que escolhemos implementar.
:::

## Limite da analogia

O programa não decide se uma mensagem é hostil nem afirma que toda crítica é projeção sem fundamento. Não levar para o lado pessoal também não remove a responsabilidade de quem ofende.

## O que entrou na sua mochila

Usamos composição: o Caderno guarda uma RespostaEscolhida. Agora você consegue relacionar uma necessidade humana, uma regra escolhida e um resultado executável. Na próxima etapa reutilizaremos os objetos; não recomeçaremos um curso de sintaxe.

::: quiz Confira sua leitura e sua execução
? Mensagem recebida e leitura pessoal são iguais?
+ Não necessariamente; o modelo guarda os dois textos separadamente.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? O campo Limite decide o caráter de quem falou?
+ Não; registra o limite que a pessoa deseja preservar.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Por que usar composição neste caso?
+ O caderno contém uma resposta; uma pessoa não se transforma numa crítica por herança.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? O mesmo registro serve para um elogio?
+ Sim; também podemos distinguir elogio e interpretação pessoal.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Registre um elogio em vez de uma crítica?
+ Os mesmos campos funcionam.
- O programa avalia a intenção da pessoa
- Nada pode ser observado pela execução
- A linguagem decide o sentido do texto
> Reproduza a variação descrita no exercício e acompanhe o estado.

? Qual limite precisa acompanhar esta experiência?
+ O programa não decide se uma mensagem é hostil nem afirma que toda crítica é projeção sem fundamento. Não levar para o lado pessoal também não remove a responsabilidade de quem ofende.
- O estado do programa certifica o desenvolvimento pessoal.
- O código define como toda pessoa deve sentir e agir.
- Não precisamos distinguir texto e aplicação.
> A utilidade da comparação depende de manter claro o que o modelo representa e o que não representa.
:::

::: cartas Lembre sem decorar
Composição :: Usamos composição: o Caderno guarda uma RespostaEscolhida.
dados pessoais :: O objeto guarda mensagem recebida, leitura pessoal, resposta escolhida e limite.
limites :: O programa não decide se uma mensagem é hostil nem afirma que toda crítica é projeção sem fundamento.
resposta escolhida :: Os mesmos campos funcionam.
Passagem estudada :: Páginas físicas 22–26 do PDF recebido; explicação e exemplos são próprios.
Experiência executável :: dotnet run --project codigo/compromissos/Caderno.Console -- cap78
Variação para experimentar :: Registre um elogio em vez de uma crítica. Escreva uma leitura e uma resposta que não dependam de transformar esse elogio em definição de quem você é.
Limite desta comparação :: O programa não decide se uma mensagem é hostil nem afirma que toda crítica é projeção sem fundamento. Não levar para o lado pessoal também não remove a responsabilidade de quem ofende.
:::
