---
id: crencas-cap102
order: 131
kind: capitulo
num: 102
group: C# & Mentalidade · Parte 16 · Crenças e C#
title: Perguntas que abrem espaço
subtitle: Questionar não é acusar a si mesmo. É trocar uma sentença ampla por perguntas específicas sobre origem, evidência, contexto e apoio.
minutes: 45
hue: 340
objectives:
- Formular perguntas abertas sobre uma ideia que parece definitiva
- Buscar uma explicação alternativa sem obrigar-se a adotá-la
- Representar uma lista de perguntas em C# sem armazenar dados pessoais
origem: livros/07-crencas-csharp/content/140-cap102.md
---

::: vila Uma frase no quadro
Durante uma aula, Marta escreve “não consigo aprender interfaces”. Em vez de tentar contradizer a frase com “consigo qualquer coisa”, a professora pergunta: “Qual exemplo você tentou? O que funcionou? O que ainda está confuso? Que tipo de demonstração ajudaria?”. A conversa sai do julgamento e encontra um próximo tópico de estudo.
:::

## Investigar é diferente de discutir consigo

Uma pergunta pode abrir opções sem exigir uma resposta otimista. Use frases como “o que sei até agora?”, “em que condições isso aconteceu?” e “que outra explicação seria possível?”. Se não houver resposta, anote a lacuna ou pause; não é necessário forçar uma conclusão.

1. **Origem:** quando ou em que contexto aprendi esta ideia?
2. **Observação:** que fatos específicos tenho?
3. **Contraponto:** há observações diferentes ou exceções?
4. **Contexto:** que recursos, instruções ou barreiras influenciaram?
5. **Precisão:** qual versão mais restrita da frase os dados realmente sustentam?
6. **Apoio:** quem ou o que pode ajudar a investigar com segurança?

Essas perguntas não são uma ferramenta diagnóstica e não precisam ser todas respondidas. Se uma pergunta gerar desconforto, escolha outra situação ou encerre o exercício.

::: regra Prefira curiosidade a uma sentença contrária
Trocar “não consigo nada” por “consigo tudo” apenas inverte a generalização. Uma formulação equilibrada pode ser: “Ainda não resolvi este tipo de exercício; posso pedir um exemplo menor e tentar outra vez quando houver tempo.”
:::

## Perguntas reutilizáveis

Uma função pode devolver perguntas genéricas, que a pessoa escolhe usar ou ignorar:

```cs Jornada/Crencas/PerguntasDeInvestigacao.cs
namespace Jornada.Crencas;

public static class PerguntasDeInvestigacao
{
    public static IReadOnlyList<string> Para() =>
    [
        "Que observação específica levou a esta hipótese?",
        "Que evidência ou contexto ainda está faltando?",
        "Existe outra explicação possível?",
        "Que apoio ou condição tornaria um próximo passo mais acessível?"
    ];
}
```

Como a lista é genérica, ela não precisa repetir o texto da hipótese; desse modo, um aplicativo pode orientar a reflexão sem imprimir ou gravar relatos particulares.

::: exercicio Escolha uma pergunta
Uma pessoa diz “não sou boa com apresentações”. Escolha duas perguntas da lista que ajudem a delimitar a afirmação sem negar a experiência dela.
---
**Resposta possível:** “Que observação específica levou a esta hipótese?” e “Que apoio ou condição tornaria um próximo passo mais acessível?”. Pode-se descobrir que a dificuldade ocorreu em apresentações sem tempo de preparação ou que o ponto difícil é responder perguntas, não falar em público.
:::

::: quiz
? Qual pergunta tende a produzir informação mais específica?
+ “Que observação aconteceu nesta apresentação?”
- “Por que sou assim?”
- “Por que sempre estrago tudo?”
> A pergunta específica investiga o episódio em vez de transformar uma situação em identidade.

? É obrigatório responder a todas as perguntas?
+ Não; a investigação é voluntária e pode ser pausada.
- Sim, ou o resultado não vale.
- Sim, mesmo se causar desconforto.
> Segurança e autonomia importam mais do que completar uma lista.

? Uma formulação equilibrada serve para:
+ Descrever melhor o que se sabe e o que ainda falta saber.
- Substituir todo pensamento difícil por uma frase positiva.
- Garantir que o próximo resultado será bom.
> A linguagem pode ser mais justa sem prometer um resultado.
:::

::: resumo
- Perguntas específicas reduzem rótulos e generalizações.
- Procure fatos, contrapontos, contexto e apoio possível.
- Uma hipótese pode continuar em aberto; pausar também é válido.
- Uma lista de perguntas não é diagnóstico nem obrigação.
:::

::: cartas
Pergunta aberta :: Pergunta que investiga contexto e opções sem pressupor uma resposta.
Formulação equilibrada :: Frase específica que inclui o que se sabe e o que permanece incerto.
Autonomia :: Liberdade para escolher, pular ou encerrar uma atividade.
:::
