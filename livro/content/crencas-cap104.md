---
id: crencas-cap104
order: 133
kind: capitulo
num: 104
group: C# & Mentalidade · Parte 16 · Crenças e C#
title: Revisar a ideia sem exigir certeza
subtitle: Uma conclusão equilibrada pode continuar provisória. Atualizar a hipótese é diferente de substituí-la por uma promessa positiva.
minutes: 50
hue: 4
objectives:
- Sintetizar observações que apoiam, contradizem ou contextualizam uma hipótese
- Escrever uma formulação específica que reconheça incertezas e barreiras
- Usar estados em C# que descrevem os registros, não a pessoa
origem: livros/07-crencas-csharp/content/142-cap104.md
---

::: vila O que o exercício realmente mostrou
Depois de ler o teste com a colega, Lia entende o caso de `Where`, mas continua confusa com `GroupBy`. A conclusão “agora sei todo LINQ” seria exagerada; “não entendo nada” também apagaria o que ela aprendeu. Ela registra algo mais específico: “Entendi a filtragem deste exemplo; ainda preciso de outra explicação para agrupamentos”. A dúvida permanece, mas agora tem contornos e um próximo passo.
:::

## Revisão não é apagar

Uma investigação não precisa terminar com “a crença foi eliminada”. Resultados possíveis incluem:

- há observações diferentes, então a hipótese está **mista**;
- as observações registradas apontam numa direção, mas não cobrem todos os contextos;
- falta evidência para concluir;
- a frase era ampla demais e precisa ficar mais específica;
- a pessoa prefere encerrar ou procurar apoio.

Substituir uma ideia limitante por uma afirmação fortalecedora pode parecer um caminho simples. Ainda assim, evitamos tratar frases absolutas como “verdades novas”: uma afirmação positiva sem sustentação pode soar tão absoluta quanto a negativa. Preferimos uma frase **realista, específica e revisável**.

| Ampla | Mais específica e aberta |
|---|---|
| “Não aprendo programação.” | “Ainda preciso de exemplos menores para este conceito.” |
| “Preciso fazer tudo sem errar.” | “Posso revisar uma parte e pedir ajuda nos casos que não entendi.” |
| “Nada vai mudar.” | “Não sei o resultado; posso perguntar sobre uma opção disponível.” |

Se a frase envolve condições materiais ou barreiras externas, inclua-as. Uma reformulação não deve sugerir que pensamento positivo resolve falta de renda, discriminação, saúde ou acesso.

## Sintetizar evidências registradas

O estado abaixo descreve apenas **o conteúdo da lista**, nunca o nível de capacidade ou saúde de uma pessoa:

```cs Jornada/Crencas/SinteseDaInvestigacao.cs
namespace Jornada.Crencas;

public enum SinteseDaInvestigacao
{
    SemObservacoes,
    SoApoio,
    SoContradicao,
    SoContexto,
    ApoioEContradicao,
    ApoioEContexto,
    ContradicaoEContexto,
    ApoioContradicaoEContexto
}
```

O método correspondente no código do laboratório calcula esse estado a partir das categorias presentes. Assim, apoio mais contexto não é reduzido a “só apoio”. Cada nome descreve a lista atual, não significa que a hipótese foi provada.

::: exercicio Escreva uma revisão provisória
Uma pessoa conseguiu executar um exemplo após consultar a documentação, mas ainda não consegue adaptar o exemplo sem ajuda. Reescreva “sou incapaz” de forma específica, sem prometer que ela dominará o tema.
---
**Resposta possível:** “Consegui executar o exemplo com documentação; adaptar o código ainda exige apoio.” A frase preserva as duas observações e deixa a próxima aprendizagem em aberto.
:::

::: quiz
? “A lista só tem evidências de apoio” permite afirmar:
+ Que os registros atuais estão nessa direção, e ainda podem estar incompletos.
- Que a hipótese é universalmente verdadeira.
- Que evidências contrárias não existem.
> O estado descreve a lista disponível, não todo o mundo.

? Qual é uma formulação mais equilibrada para “não consigo aprender C#”?
+ “Ainda preciso de mais prática neste recurso específico.”
- “Sou um gênio e consigo aprender qualquer coisa.”
- “Nunca devo sentir dificuldade.”
> A formulação específica reconhece uma necessidade sem prometer um resultado.

? Uma pessoa não quer continuar a atividade. O caderno deve:
+ Permitir encerrar sem exigir uma conclusão.
- Forçar uma reformulação positiva.
- Marcar a pessoa como resistente.
> Autonomia é parte do desenho, não uma exceção.
:::

::: resumo
- Revisar é atualizar o que se pensa com base em observações e contexto.
- Evidência mista, insuficiente ou uma pausa são resultados válidos.
- Uma frase útil é específica, realista e aberta a revisão.
- O estado da hipótese não é uma nota nem uma descrição da pessoa.
:::

::: cartas
Hipótese provisória :: Ideia que permanece aberta a revisão conforme surgem contexto e observações.
Síntese :: Resumo dos tipos de observação registrados; não é prova final.
Afirmação equilibrada :: Frase específica que não promete resultados nem apaga barreiras.
:::
