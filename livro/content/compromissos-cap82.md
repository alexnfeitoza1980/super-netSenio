---
id: compromissos-cap82
order: 106
kind: capitulo
num: 82
group: C# & Mentalidade · Oficina · O Caderno dos Quatro Compromissos
title: O melhor de hoje cabe em quanto tempo?
subtitle: Uma experiência do tema, uma escolha e uma nova capacidade no mesmo programa.
minutes: 45
hue: 168
objectives:
- Compreender a ideia estudada e seu contexto na obra de Ruiz
- Construir a capacidade deste capítulo no Caderno
- Executar o cenário, variar uma entrada e explicar seu resultado
- Distinguir a proposta do autor das regras do software
origem: livros/05-quatro-compromissos-csharp/content/118-cap82.md
---

::: vila Uma página da nossa história
Ontem Lia tinha duas horas. Hoje tem quarenta minutos entre trabalho e família. Se copiar o plano de ontem, terminará olhando apenas para o que faltou. Ela decide escolher duas ações menores. O programa vai ajudá-la a somar o que planejou, sem dar uma nota para seu esforço.
:::

::: ilustracao img/compromissos/jornada82.svg | O melhor de hoje cabe em quanto tempo?: uma escolha visível e sua consequência no caderno.
:::

::: html
<figure style="max-width:420px;margin:2rem auto"><div style="overflow:hidden;aspect-ratio:3/4;border-radius:24px"><img src="img/compromissos/quatro-jornada.png" alt="Cultivar uma prática possível hoje" style="display:block;width:400%;max-width:none;transform:translateX(-75%)"></div><figcaption>Cultivar uma prática possível hoje. Uma imagem para acompanhar a experiência deste compromisso.</figcaption></figure>
:::

## A ideia que vamos experimentar

O quarto compromisso considera que o melhor possível varia com as condições de cada momento. Ruiz distingue a prática de uma cobrança por perfeição e usa uma história sobre meditação para discutir excesso. Fazer mais horas não garante uma vida melhor. O capítulo também relaciona ação, aprendizagem com erros e repetição.

**Fonte de leitura:** Don Miguel Ruiz, *Os Quatro Compromissos*, páginas físicas 32–36 do PDF fornecido. Este texto é uma explicação própria, não uma citação literal. A paginação de outras edições pode variar.

## Do tema para uma decisão no programa

Escolhemos uma medida operacional modesta: minutos disponíveis declarados. O plano soma passos e recusa um passo que ultrapasse esse limite. Não medimos energia psíquica nem transformamos emoções em porcentagens. A regra organiza uma agenda; o julgamento sobre o que é possível continua com a pessoa.

::: pense Antes de executar
Descreva o que você espera ver mudar. Qual informação já existe? Qual ainda não existe? O que o programa deve recusar? Escreva sua previsão antes de olhar a saída.
:::

## Vamos fazer acontecer

Execute **a partir da pasta deste livro**. Todos os comandos cap74 a cap87 existem no mesmo projeto. Os cenários começam com dados novos e fictícios para serem reproduzíveis; o diário pessoal persistente é apresentado no laboratório.

```bash
dotnet run --project codigo/compromissos/Caderno.Console -- cap82
```

O trecho abaixo está dentro do método `Cap82` em `codigo/compromissos/Caderno.Console/Exemplos.cs`. Usa `using Quatro;`. É um corpo de método, não um arquivo independente: não o cole ao lado das declarações das classes.

```cs
var plano = new PlanoDoDia(40);
plano.Incluir("Esclarecer uma dúvida", 10);
plano.Incluir("Revisar minha promessa", 20);
Console.WriteLine($"Planejados: {plano.Planejados} de {plano.Disponiveis} minutos");
Console.WriteLine("Tempo disponível é informado, não diagnosticado.");
```

### Leia o que o código faz

`Sum` agrega durações. O construtor permite zero minutos: um dia sem espaço para ações é representável. Incluir exige duração positiva e suficiente espaço restante. Validar antes de adicionar evita que uma tentativa recusada aumente o total. As unidades aparecem nos nomes para não confundir minutos com horas.

Observe os verbos do código: registrar, perguntar, escolher, cumprir, salvar. Cada um deve produzir uma mudança que você consiga explicar. Execute de novo: nos cenários, os dados são recriados. Para conservar sua própria prática, use os comandos do laboratório.

### A saída real desta experiência

```saida
Planejados: 30 de 40 minutos
Tempo disponível é informado, não diagnosticado.
```

Compare sua previsão com a execução. Não basta obter a mesma tela: explique qual instrução produziu cada linha e qual objeto foi consultado. Troque apenas uma entrada por vez para observar a causa da diferença.

## Por dentro da capacidade

O arquivo de referência é `codigo/compromissos/Caderno.Core/Plano.cs`. Alguns membros serão usados nos próximos capítulos; hoje concentre-se no caminho percorrido pela experiência. A definição completa evita que o leitor precise adivinhar membros omitidos.

```cs
namespace Quatro;

public sealed record Passo(string Descricao, int Minutos);

public sealed class PlanoDoDia
{
    private readonly List<Passo> passos = [];
    public int Disponiveis { get; }
    public int Planejados => passos.Sum(p => p.Minutos);
    public IReadOnlyList<Passo> Passos => passos.ToArray();

    public PlanoDoDia(int disponiveis)
    {
        if (disponiveis < 0) throw new ArgumentOutOfRangeException(nameof(disponiveis));
        Disponiveis = disponiveis;
    }

    public void Incluir(string descricao, int minutos)
    {
        string texto = Regras.Texto(descricao);
        if (minutos <= 0 || minutos > Disponiveis - Planejados)
            throw new ArgumentOutOfRangeException(nameof(minutos), "O passo não cabe no tempo informado.");
        passos.Add(new(texto, minutos));
    }
}
```



## Agora escolha outro caminho

::: exercicio Experimente e explique
Com um plano de 30 minutos, adicione passos de 10 e 20. Tente incluir mais 1 e confira Planejados.

Altere o método Cap82 em uma cópia do projeto, execute o mesmo comando e compare com sua previsão. Quando houver uma exceção esperada, capture-a com `catch (ArgumentException erro)` ou `catch (InvalidOperationException erro)`, conforme a regra estudada, e mostre `erro.Message`.
--- resposta
Os dois primeiros cabem exatamente. O terceiro é rejeitado; o total permanece 30. Um plano com capacidade zero existe, mas não aceita passos de duração positiva.

Procure o teste correspondente em `codigo/compromissos/Caderno.Testes/Program.cs`. Os testes exercitam entradas, mudanças e falhas. Eles não comprovam a filosofia do autor: verificam o contrato que escolhemos implementar.
:::

## Limite da analogia

Tempo disponível não mede disposição, saúde, qualidade ou valor pessoal. Estimativas podem errar. Recusar um passo é comportamento desta agenda, não uma afirmação de que a pessoa falhou no quarto compromisso.

## O que entrou na sua mochila

`Sum` agrega durações. Agora você consegue relacionar uma necessidade humana, uma regra escolhida e um resultado executável. Na próxima etapa reutilizaremos os objetos; não recomeçaremos um curso de sintaxe.

::: quiz Confira sua leitura e sua execução
? Quem informa a disponibilidade?
+ A pessoa; o programa não diagnostica sua energia.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? O que Sum calcula?
+ O total de minutos dos passos incluídos.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Um plano com zero minutos pode existir?
+ Sim, mas não comporta um passo de duração positiva.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? O que acontece quando um passo não cabe?
+ É rejeitado sem alterar a soma anterior.
- O resultado prova uma qualidade moral da pessoa.
- O programa decide sozinho o significado do texto.
- Não há uma regra verificável para esse caso.
> Consulte a experiência e a implementação deste capítulo; o alcance da regra é específico.

? Com um plano de 30 minutos, adicione passos de 10 e 20?
+ Os dois primeiros cabem exatamente.
- O programa avalia a intenção da pessoa
- Nada pode ser observado pela execução
- A linguagem decide o sentido do texto
> Reproduza a variação descrita no exercício e acompanhe o estado.

? Qual limite precisa acompanhar esta experiência?
+ Tempo disponível não mede disposição, saúde, qualidade ou valor pessoal. Estimativas podem errar. Recusar um passo é comportamento desta agenda, não uma afirmação de que a pessoa falhou no quarto compromisso.
- O estado do programa certifica o desenvolvimento pessoal.
- O código define como toda pessoa deve sentir e agir.
- Não precisamos distinguir texto e aplicação.
> A utilidade da comparação depende de manter claro o que o modelo representa e o que não representa.
:::

::: cartas Lembre sem decorar
Agregação :: `Sum` agrega durações.
unidades :: Escolhemos uma medida operacional modesta: minutos disponíveis declarados.
limite declarado :: Tempo disponível não mede disposição, saúde, qualidade ou valor pessoal.
regra de negócio :: Os dois primeiros cabem exatamente.
Passagem estudada :: Páginas físicas 32–36 do PDF recebido; explicação e exemplos são próprios.
Experiência executável :: dotnet run --project codigo/compromissos/Caderno.Console -- cap82
Variação para experimentar :: Com um plano de 30 minutos, adicione passos de 10 e 20. Tente incluir mais 1 e confira Planejados.
Limite desta comparação :: Tempo disponível não mede disposição, saúde, qualidade ou valor pessoal. Estimativas podem errar. Recusar um passo é comportamento desta agenda, não uma afirmação de que a pessoa falhou no quarto compromisso.
:::
