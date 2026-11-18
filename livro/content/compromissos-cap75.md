---
id: compromissos-cap75
order: 99
kind: capitulo
num: 75
group: C# & Mentalidade · Oficina · O Caderno dos Quatro Compromissos
title: O juiz interno encontra uma página em branco
subtitle: Uma experiência do tema, uma escolha e uma nova capacidade no mesmo programa.
minutes: 45
hue: 168
objectives:
- Compreender a ideia estudada e seu contexto na obra de Ruiz
- Construir a capacidade deste capítulo no Caderno
- Executar o cenário, variar uma entrada e explicar seu resultado
- Distinguir a proposta do autor das regras do software
origem: livros/05-quatro-compromissos-csharp/content/111-cap75.md
---

::: vila Uma página da nossa história
Lia esqueceu de responder a um amigo. No caderno, digita “Sou uma amiga horrível”. Caio pergunta: “Isso descreve o que aconteceu ou uma sentença sobre você inteira?”. Ela tenta outra vez: “Não respondi hoje”. Há um fato a examinar e uma ação possível. O programa vai oferecer espaço para essa diferença, sem fingir que consegue fazê-la sozinho.
:::

::: ilustracao img/compromissos/jornada75.svg | O juiz interno encontra uma página em branco: uma escolha visível e sua consequência no caderno.
:::

## A ideia que vamos experimentar

No texto, Juiz e Vítima são imagens usadas por Ruiz para descrever a condenação interna e o sofrimento associado às regras aprendidas. Ele critica a repetição do castigo por um mesmo erro. Podemos estudar essa imagem sem tratá-la como diagnóstico psicológico ou afirmar que todo sofrimento é produzido pela própria pessoa.

**Fonte de leitura:** Don Miguel Ruiz, *Os Quatro Compromissos*, páginas físicas 8–12 do PDF fornecido. Este texto é uma explicação própria, não uma citação literal. A paginação de outras edições pode variar.

## Do tema para uma decisão no programa

A página terá três campos: observação, interpretação e próximo passo. A pessoa escreve o que observou, o significado que atribuiu e o que deseja fazer. O programa mantém esses campos distintos. Não troca automaticamente “sou horrível” por uma frase positiva: uma substituição mecânica pode esconder a necessidade de reparar uma ação.

::: pense Antes de executar
Descreva o que você espera ver mudar. Qual informação já existe? Qual ainda não existe? O que o programa deve recusar? Escreva sua previsão antes de olhar a saída.
:::

## Vamos fazer acontecer

Execute **a partir da pasta deste livro**. Todos os comandos cap74 a cap87 existem no mesmo projeto. Os cenários começam com dados novos e fictícios para serem reproduzíveis; o diário pessoal persistente é apresentado no laboratório.

```bash
dotnet run --project codigo/compromissos/Caderno.Console -- cap75
```

O trecho abaixo está dentro do método `Cap75` em `codigo/compromissos/Caderno.Console/Exemplos.cs`. Usa `using Quatro;`. É um corpo de método, não um arquivo independente: não o cole ao lado das declarações das classes.

```cs
var caderno = new Caderno();
var reflexao = new Reflexao("Não respondi hoje", "Sou um péssimo amigo", "Perguntar quando podemos conversar");
caderno.Refletir(reflexao);
Console.WriteLine($"Observação: {reflexao.Observacao}");
Console.WriteLine($"Próximo passo: {reflexao.ProximoPasso}");
```

### Leia o que o código faz

Um construtor impede campos vazios. As propriedades apenas de leitura preservam o registro depois de criado. O mesmo método `Regras.Texto` já usado no inventário elimina repetição de validação. Ele valida presença e espaços nas bordas, não a verdade da observação. Nomear bem uma propriedade ajuda o pensamento, mas não prova o conteúdo.

Observe os verbos do código: registrar, perguntar, escolher, cumprir, salvar. Cada um deve produzir uma mudança que você consiga explicar. Execute de novo: nos cenários, os dados são recriados. Para conservar sua própria prática, use os comandos do laboratório.

### A saída real desta experiência

```saida
Observação: Não respondi hoje
Próximo passo: Perguntar quando podemos conversar
```

Compare sua previsão com a execução. Não basta obter a mesma tela: explique qual instrução produziu cada linha e qual objeto foi consultado. Troque apenas uma entrada por vez para observar a causa da diferença.

## Por dentro da capacidade

O arquivo de referência é `codigo/compromissos/Caderno.Core/Reflexao.cs`. Alguns membros serão usados nos próximos capítulos; hoje concentre-se no caminho percorrido pela experiência. A definição completa evita que o leitor precise adivinhar membros omitidos.

```cs
namespace Quatro;

public sealed record Reflexao
{
    public string Observacao { get; }
    public string Interpretacao { get; }
    public string ProximoPasso { get; }

    public Reflexao(string observacao, string interpretacao, string proximoPasso)
    {
        Observacao = Regras.Texto(observacao);
        Interpretacao = Regras.Texto(interpretacao);
        ProximoPasso = Regras.Texto(proximoPasso);
    }
}
```

```cs
namespace Quatro;

public static class Regras
{
    public static string Texto(string? texto)
    {
        if (string.IsNullOrWhiteSpace(texto))
            throw new ArgumentException("Escreva um texto antes de registrar.");
        return texto.Trim();
    }
}
```

## Agora escolha outro caminho

::: exercicio Experimente e explique
Tente criar uma reflexão sem próximo passo. Explique por que rejeitar esse registro é uma regra do nosso caderno, não uma exigência do livro original.

Altere o método Cap75 em uma cópia do projeto, execute o mesmo comando e compare com sua previsão. Quando houver uma exceção esperada, capture-a com `catch (ArgumentException erro)` ou `catch (InvalidOperationException erro)`, conforme a regra estudada, e mostre `erro.Message`.
--- resposta
O construtor lança ArgumentException. Escolhemos exigir um próximo passo para tornar esta ferramenta orientada à prática; alguém pode refletir legitimamente sem ter ainda uma solução.

Procure o teste correspondente em `codigo/compromissos/Caderno.Testes/Program.cs`. Os testes exercitam entradas, mudanças e falhas. Eles não comprovam a filosofia do autor: verificam o contrato que escolhemos implementar.
:::

## Limite da analogia

O campo Observacao é uma declaração do usuário, não um fato certificado. O software não sabe se o relato é completo, e esta atividade não transfere a responsabilidade por uma agressão para quem a sofreu.

## O que entrou na sua mochila

Um construtor impede campos vazios. Agora você consegue relacionar uma necessidade humana, uma regra escolhida e um resultado executável. Na próxima etapa reutilizaremos os objetos; não recomeçaremos um curso de sintaxe.

::: quiz Confira sua leitura e sua execução
? Por que separar observação e interpretação?
+ Para distinguir o relato do significado atribuído a ele.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Regras.Texto comprova a verdade de uma frase?
+ Não; valida presença e remove espaços nas bordas.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? O que impede criar uma reflexão incompleta?
+ O construtor valida os três campos.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? O código representa o Juiz como diagnóstico?
+ Não; usa a imagem do autor para propor uma atividade de registro.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Tente criar uma reflexão sem próximo passo?
+ O construtor lança ArgumentException.
- O programa avalia a intenção da pessoa
- Nada pode ser observado pela execução
- A linguagem decide o sentido do texto
> Reproduza a variação descrita no exercício e acompanhe o estado.

? Qual limite precisa acompanhar esta experiência?
+ O campo Observacao é uma declaração do usuário, não um fato certificado. O software não sabe se o relato é completo, e esta atividade não transfere a responsabilidade por uma agressão para quem a sofreu.
- O estado do programa certifica o desenvolvimento pessoal.
- O código define como toda pessoa deve sentir e agir.
- Não precisamos distinguir texto e aplicação.
> A utilidade da comparação depende de manter claro o que o modelo representa e o que não representa.
:::

::: cartas Lembre sem decorar
Validação :: Um construtor impede campos vazios.
construtor :: A página terá três campos: observação, interpretação e próximo passo.
observação :: O campo Observacao é uma declaração do usuário, não um fato certificado.
interpretação :: O construtor lança ArgumentException.
Passagem estudada :: Páginas físicas 8–12 do PDF recebido; explicação e exemplos são próprios.
Experiência executável :: dotnet run --project codigo/compromissos/Caderno.Console -- cap75
Variação para experimentar :: Tente criar uma reflexão sem próximo passo. Explique por que rejeitar esse registro é uma regra do nosso caderno, não uma exigência do livro original.
Limite desta comparação :: O campo Observacao é uma declaração do usuário, não um fato certificado. O software não sabe se o relato é completo, e esta atividade não transfere a responsabilidade por uma agressão para quem a sofreu.
:::
