---
id: crencas-cap103
order: 132
kind: capitulo
num: 103
group: C# & Mentalidade · Parte 16 · Crenças e C#
title: Um experimento pequeno e seguro
subtitle: Um passo observável pode produzir informação. Ele não precisa provar seu valor, resolver tudo ou ser feito sem apoio.
minutes: 50
hue: 352
objectives:
- Diferenciar um experimento de uma prova sobre a capacidade de alguém
- Definir uma ação pequena, observável e ajustada ao contexto
- Validar os dados de um plano de experimento em C#
origem: livros/07-crencas-csharp/content/141-cap103.md
---

::: vila A próxima linha
Lia acredita que precisa dominar todo o framework antes de contribuir no projeto. Em vez de se obrigar a “provar que consegue”, ela combina com uma colega um experimento pequeno: ler um teste existente e perguntar qual comportamento ele protege. O resultado que ela observará é apenas se consegue explicar aquela regra com as informações e o apoio disponíveis.
:::

## Um teste de hipótese, não uma prova de valor

Um experimento pequeno é uma tentativa delimitada para aprender algo, com possibilidade de parar ou ajustar. Não se trata de medir coragem, produtividade ou valor pessoal. Um passo pode ser ler um exemplo, pedir explicação, observar alguém executando uma tarefa ou decidir que aquele não é o momento certo.

Antes de começar, defina:

- **Ação:** algo concreto e viável, não “mudar toda a minha vida”.
- **Observação:** o que será possível notar, sem transformar um resultado em nota pessoal.
- **Contexto:** tempo, ferramenta, acessibilidade e suporte necessários.
- **Limite:** o que faria pausar, pedir ajuda ou mudar o plano.
- **Revisão:** quando decidir o que aprendeu, sem assumir que um ensaio vale para todos os casos.

O objetivo é obter informação. Um experimento que não funciona pode revelar que faltava documentação, tempo, acesso ou apoio — não que a pessoa “falhou”.

## Validar o plano

O projeto do livro usa `PlanoDeExperimento` para impedir planos sem ação ou sem observação definida:

```cs Jornada/Crencas/PlanoDeExperimento.cs
namespace Jornada.Crencas;

public sealed record PlanoDeExperimento
{
    public string Acao { get; }
    public string OQueObservar { get; }
    public string LimiteOuApoio { get; }

    public PlanoDeExperimento(string acao, string oQueObservar, string limiteOuApoio)
    {
        Acao = Validar(acao, nameof(acao));
        OQueObservar = Validar(oQueObservar, nameof(oQueObservar));
        LimiteOuApoio = Validar(limiteOuApoio, nameof(limiteOuApoio));
    }

    private static string Validar(string valor, string parametro)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ArgumentException("Preencha este campo do plano.", parametro);

        return valor.Trim();
    }
}
```

Um exemplo leve, relacionado a aprender código:

```cs
var plano = new PlanoDeExperimento(
    Acao: "Ler um teste de LINQ com uma colega por dez minutos.",
    OQueObservar: "Que parte do teste ficou clara e que parte pede outra explicação.",
    LimiteOuApoio: "Parar se faltar tempo; pedir um exemplo menor.");
```

Não há propriedade `Sucesso` nem pontuação. O plano registra uma intenção, e a pessoa continua livre para mudar de ideia.

::: cuidado Não transforme o plano em cobrança
Circunstâncias mudam. Se o plano não couber no dia, revise, adie ou descarte. Uma ferramenta de organização deve apoiar a autonomia, não criar mais uma obrigação.
:::

::: exercicio Reduza o tamanho sem reduzir o respeito
Plano inicial: “Vou aprender toda a programação esta semana e provar que sou capaz”. Reescreva-o em uma ação observável para um exercício técnico leve e inclua uma possibilidade de apoio.
---
**Resposta possível:** “Na quarta, vou estudar um exemplo de `Where` por 15 minutos e escrever uma pergunta sobre a parte que não entendi. Se eu travar, posso pedir um exemplo menor ou deixar para outra sessão.” Isso descreve uma ação, não um teste de valor.
:::

::: quiz
? O que torna uma ação adequada a um experimento pequeno?
+ Ela é concreta, delimitada e tem um resultado observável.
- Ela precisa provar que a crença é falsa.
- Ela deve ser feita sem ajuda para que conte.
> Um experimento ajuda a aprender; apoio e revisão fazem parte do desenho.

? Se o plano não foi executado porque faltou acesso:
+ O contexto mudou; revise ou pause sem atribuir culpa.
- Está provado que a hipótese original é verdadeira.
- A pessoa deve insistir até conseguir.
> A falta de acesso é informação sobre as condições, não sobre o valor pessoal.

? Por que o tipo não tem uma propriedade `Sucesso`?
+ Para não resumir uma experiência complexa a um julgamento binário da pessoa.
- Porque planos nunca precisam de resultado observável.
- Porque o compilador não suporta propriedades booleanas.
> O resultado deve ser observado e contextualizado, não convertido em nota pessoal.
:::

::: resumo
- Um experimento é uma tentativa para aprender, não uma prova de valor.
- Defina ação, observação, contexto e limite.
- Apoio, pausa e mudança de plano são opções legítimas.
- Um resultado informa sobre aquela tentativa e suas condições.
:::

::: cartas
Experimento pequeno :: Ação delimitada para obter informação, sem medir o valor da pessoa.
Observação :: Resultado que pode ser descrito sem transformá-lo em nota pessoal.
Limite do plano :: Condição para parar, pausar, pedir apoio ou rever a ação.
:::
