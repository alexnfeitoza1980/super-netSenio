---
id: crencas-cap99
order: 128
kind: capitulo
num: 99
group: C# & Mentalidade · Parte 16 · Crenças e C#
title: Procure evidências nos dois sentidos
subtitle: É fácil notar o caso que confirma uma expectativa e esquecer os demais. Um registro equilibrado não garante a resposta, mas ajuda a perguntar melhor.
minutes: 55
hue: 304
objectives:
- Explicar como a atenção seletiva pode favorecer evidências que combinam com uma expectativa
- Buscar observações que apoiem, contrariem ou contextualizem uma hipótese
- Usar LINQ para organizar registros sem produzir uma pontuação de pessoas
origem: livros/07-crencas-csharp/content/137-cap99.md
---

::: vila Uma tentativa não é uma série
Caio tenta resolver um exercício de LINQ, trava numa consulta e conclui que “nunca aprende”. Na semana seguinte, termina uma consulta `Where` e uma `OrderBy`, mas não associa esse progresso ao tema porque foram tarefas menores. A hipótese já orientou a atenção dele para o fracasso mais marcante. Uma lista de observações, com limites e contexto, oferece uma visão menos seletiva.
:::

## Atenção seletiva e evidência

Quando esperamos que algo aconteça, podemos reparar mais nos exemplos compatíveis. O viés de confirmação é uma **possibilidade a considerar**, não um diagnóstico de alguém. Uma lista de evidências também pode ser incompleta, enviesada ou difícil de interpretar.

Para investigar uma hipótese, procure:

1. uma observação que a **apoie**;
2. uma observação que a **contrarie**;
3. condições que **contextualizem** ambas, como tempo disponível, tarefa, instruções e apoio;
4. o que ainda não foi observado.

Não é uma competição para “vencer” um dos lados. Se a evidência for mista, a resposta honesta é manter a dúvida e talvez formular uma hipótese mais específica.

::: figura Evidências não são um placar
<svg viewBox="0 0 700 190" role="img" aria-label="Uma hipótese recebe observações que apoiam, contradizem e contextualizam; o resultado é uma visão parcial, não uma nota">
  <rect x="248" y="12" width="205" height="48" rx="12" class="f-info"/><text x="350" y="42" text-anchor="middle" font-weight="800">Hipótese específica</text>
  <path class="arrow" d="M350 62V88M350 88H115V108M350 88V108M350 88H585V108"/>
  <rect x="15" y="110" width="200" height="52" rx="12" class="f-good"/><text x="115" y="132" text-anchor="middle" font-weight="800">Apoia</text><text x="115" y="150" text-anchor="middle" font-size="12" class="t-dim">uma observação</text>
  <rect x="250" y="110" width="200" height="52" rx="12" class="f-warn"/><text x="350" y="132" text-anchor="middle" font-weight="800">Contradiz</text><text x="350" y="150" text-anchor="middle" font-size="12" class="t-dim">outra observação</text>
  <rect x="485" y="110" width="200" height="52" rx="12" class="f-prim"/><text x="585" y="132" text-anchor="middle" font-weight="800">Contextualiza</text><text x="585" y="150" text-anchor="middle" font-size="12" class="t-dim">condições e limites</text>
</svg>
:::

## Organizar registros com LINQ

Um tipo `Evidencia` pode registrar uma observação e sua relação com **uma hipótese previamente definida**. A relação não é uma nota de confiança:

```cs Jornada/Crencas/Evidencia.cs
namespace Jornada.Crencas;

public sealed record Evidencia(string Observacao, RelacaoDaEvidencia Relacao);
```

`RelacaoDaEvidencia` é o `enum` compartilhado definido em `RelacaoDaEvidencia.cs`.

Uma consulta pode agrupar registros por relação. Ela resume a lista, não decide se a hipótese é verdadeira:

```cs
var contagem = evidencias
    .GroupBy(e => e.Relacao)
    .ToDictionary(g => g.Key, g => g.Count());
```

Um grupo ausente não é evidência do contrário. Se `Contradiz` não aparece na lista, pode significar apenas que ninguém procurou por esse tipo de exemplo.

::: exercicio Encontre o contexto que falta
Hipótese: “Preciso estudar com um curso pago para entender interfaces”. Observação: uma pessoa aprendeu `interface` num tutorial gratuito. O que essa observação sugere? O que ela não prova?
---
**Resposta possível:** é uma observação que contraria a ideia de que um curso pago seja sempre necessário para aprender interfaces. Não prova que cursos pagos nunca ajudem, que todos aprendam do mesmo modo ou que o tutorial baste para qualquer pessoa.
:::

::: quiz
? Se não há observação registrada contra uma hipótese, o que podemos concluir?
+ Que ainda não há observação contrária registrada.
- Que a hipótese está comprovada.
- Que todas as pessoas observaram e concordaram.
> Ausência de registro não é prova de ausência.

? Para que serve `GroupBy` neste exemplo?
+ Organizar as evidências por relação declarada.
- Calcular o valor verdadeiro de cada crença.
- Avaliar a personalidade da pessoa.
> O LINQ agrupa dados; a interpretação continua dependente de contexto.

? Se duas observações parecem contraditórias, o melhor próximo passo é:
+ Verificar diferenças de contexto e formular uma hipótese mais específica.
- Apagar a observação que não combina com a ideia preferida.
- Tirar a média e tratar o resultado como fato.
> Diferenças de tarefa, tempo e apoio podem explicar por que resultados variam.
:::

::: resumo
- A atenção pode privilegiar exemplos que confirmam uma expectativa.
- Procure apoio, contradição e contexto, e registre o que ainda não sabe.
- LINQ organiza registros; não comprova ideias nem mede pessoas.
- Evidência mista é um resultado legítimo.
:::

::: cartas
Viés de confirmação :: Tendência possível de buscar ou notar mais exemplos compatíveis com uma expectativa.
`GroupBy` :: Operação LINQ que organiza elementos por uma chave.
Evidência contextual :: Observação que ajuda a entender em que condições um resultado ocorreu.
:::
