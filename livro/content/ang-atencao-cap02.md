---
id: ang-atencao-cap02
order: 323
kind: capitulo
num: 226
group: Angular · Parte 57 · Sair do piloto automático
title: Por que nos atacamos?
subtitle: A voz interna que acusa e o compilador que informa. TypeScript para quem vem do C#, com tipos que separam fato, interpretação e autocrítica.
minutes: 60
hue: 340
objectives:
- Reconhecer a diferença entre **uma crítica que informa** e **uma crítica que ataca**
- Traduzir o que você já sabe de **C#** para **TypeScript**: tipos, `interface`, `class`, `null` e `undefined`
- Modelar casos com **uniões discriminadas** e garantir, com **`never`**, que nenhum caso foi esquecido
- Usar **genéricos**, **`Record`**, **`as const`**, **`satisfies`** e **predicados de tipo**
- Entender o que o modo **`strict`** liga, e por que ele é seu aliado
origem: angular/livros/01-atencao-plena-angular/content/02-cap02.md
---

::: cena Os sublinhados vermelhos
Caio colou um trecho de código num arquivo `.ts` e o editor se encheu de sublinhados vermelhos. "Eu sou péssimo nisso", murmurou. Lia puxou uma cadeira: "Lê a primeira mensagem em voz alta." Ele leu: *Argument of type 'string \| undefined' is not assignable to parameter of type 'string'.* "Ela não está dizendo que você é péssimo. Está dizendo que **esse valor pode não existir** e que ninguém tratou esse caso. É uma informação. E é das boas."
:::

::: ilustracao img/ang-atencao/cap02.svg | Duas vozes diante do mesmo erro: uma acusa a pessoa, a outra aponta o que pode ser corrigido.
:::

## O que o livro diz

Williams e Penman descrevem um mecanismo que quase todo mundo conhece: quando estamos cansados ou tristes, a mente tenta **resolver o sentimento** do mesmo jeito que resolve um problema prático. Pergunta "por que estou assim?", "o que há de errado comigo?", compara como as coisas estão com como deveriam estar. Essa análise, que funciona tão bem para consertar um carro, aqui costuma **alimentar o próprio mal-estar**: o humor puxa lembranças parecidas, as lembranças confirmam o humor, e a voz interna vai ficando mais dura.

Os autores não pedem que a gente pare de pensar nem que se convença de coisas positivas. Pedem que a gente **perceba** esses pensamentos como pensamentos, e não como fatos sobre quem somos.

Nas nossas palavras: **uma crítica útil diz o que aconteceu e o que dá para fazer; um ataque diz quem você é.** O compilador do TypeScript, quando bem configurado, é do primeiro tipo.

## Da vida para o código

| Na vida | No TypeScript |
|---|---|
| "Eu nunca acerto nada" (ataque, sem informação) | Um `any` que aceita tudo e não diz nada |
| "Esse valor pode faltar; trate esse caso" (crítica útil) | Um erro de compilação com `string \| undefined` |
| Separar **fato**, **interpretação** e **autocrítica** | Uma **união discriminada** com um campo `tipo` |
| Não deixar nenhum caso sem atenção | **`never`** no `default` de um `switch` |
| Contar com quem confere com cuidado | O modo **`strict`** e o `strictTemplates` do Angular |

## 1) TypeScript em dez minutos, vindo do C#

TypeScript é **JavaScript com tipos**. O compilador confere os tipos e depois os **apaga**: o navegador recebe JavaScript puro. Para quem vem do C#, a maior parte é familiar:

| C# | TypeScript | Observação |
|---|---|---|
| `int`, `double`, `decimal` | `number` | Um só tipo numérico (ponto flutuante) |
| `string`, `bool` | `string`, `boolean` | |
| `List<T>`, `T[]` | `T[]` ou `Array<T>` | `readonly T[]` para listas que não mudam |
| `Dictionary<K, V>` | `Record<K, V>` ou `Map<K, V>` | `Record` para objetos simples; `Map` para chaves de qualquer tipo |
| `var x = 1;` | `const x = 1;` / `let x = 1;` | Prefira `const`; use `let` só se for reatribuir |
| `null` | `null` **e** `undefined` | `undefined`: "nunca recebeu valor"; `null`: "vazio de propósito" |
| `string?` | `string \| undefined` | Uma **união**: um valor ou outro |
| `interface` | `interface` ou `type` | Estrutural (veja abaixo) |
| `enum` | `enum` **ou** união de literais | No Angular, uniões de literais são mais comuns |
| `x == y` | `x === y` | Use sempre três iguais |
| `async Task<T>` | `async (): Promise<T>` | `await` funciona igual |
| LINQ `Where`, `Select` | `filter`, `map` | Métodos de array, imediatos (não preguiçosos) |

::: csharp Tipagem estrutural: o que importa é a forma
Em C#, uma classe só "é" um `IComparable` se declarar `: IComparable`. Em TypeScript, **qualquer objeto com a forma certa** serve: se a interface pede `{ texto: string }` e o objeto tem `texto: string` (e mais coisas), ele passa. É a chamada **tipagem estrutural**. Ela deixa os testes muito simples: um objeto literal com os campos certos substitui uma classe inteira.
:::

::: erro `==` em vez de `===`
Em JavaScript, `0 == ''` é `true`, porque `==` converte tipos antes de comparar. Use **sempre** `===` e `!==`, que comparam valor **e** tipo, como o `==` do C#.
:::

## 2) Uniões discriminadas: fato, interpretação, autocrítica

Imagine um diário em que cada registro é uma de três coisas: um **fato** ("o build quebrou às 10h"), uma **interpretação** ("a equipe vai achar que fui descuidado", com algum grau de certeza) ou uma **autocrítica** ("eu nunca acerto nada"). Cada tipo tem campos diferentes. Em TypeScript, isso vira uma **união discriminada**:

<<< src/app/capitulos/cap02/registros.ts#tipos

Leia com calma:

- **`|`** quer dizer "ou": um `Registro` é um fato **ou** uma interpretação **ou** uma autocrítica.
- O campo **`tipo`** é o **discriminante**: um texto literal diferente em cada caso. Olhando para ele, o compilador sabe quais outros campos existem.
- **`Registro['tipo']`** extrai o tipo do campo: `'fato' | 'interpretacao' | 'autocritica'`.
- **`as const`** transforma o array em uma tupla de literais imutável, e **`satisfies`** confere que todos os itens são tipos válidos **sem** perder essa precisão. Se alguém digitar `'fatto'`, o compilador acusa.

## 3) Nenhum caso esquecido: `never`

Agora, uma função que descreve cada registro **sem julgá-lo**:

<<< src/app/capitulos/cap02/registros.ts#descrever

Dentro de cada `case`, o TypeScript **estreita** o tipo: no `case 'interpretacao'`, ele sabe que `r.certeza` existe; no `case 'fato'`, `r.certeza` nem compila. Isso se chama **estreitamento** (*narrowing*).

E o `default`? Se todos os casos foram tratados, o que sobra para o `default` é **nada**, um tipo chamado **`never`**. A função `casoEsquecido(valor: never)` só aceita esse "nada". No dia em que alguém acrescentar um quarto tipo ao `Registro` e esquecer de tratá-lo, o que sobra deixa de ser `never`, e o código **para de compilar** exatamente nessa linha. É o mesmo papel do `_ => throw` de um `switch` em C#, só que o aviso chega **antes** de rodar.

::: dica Por que ainda lançar uma exceção?
Os tipos somem depois da compilação. Se um dado chegar de fora (de uma API, do `localStorage`) com um `tipo` inesperado, o `throw` dentro de `casoEsquecido` evita que ele passe em silêncio. Um dos testes do capítulo confere justamente isso.
:::

## 4) Genéricos, `Record` e predicados de tipo

<<< src/app/capitulos/cap02/registros.ts#contar

`Record<TipoDeRegistro, number>` é um objeto com **exatamente** as chaves `fato`, `interpretacao` e `autocritica`, todas com números. Começamos todas em zero (inclusive as que não aparecem), e o compilador garante que nenhuma ficou de fora.

O `as Record<...>` é uma **asserção de tipo**: estamos dizendo ao compilador "confie em mim". `Object.fromEntries` devolve um tipo genérico demais, e aqui temos certeza do formato porque montamos o objeto a partir de `TIPOS`. Use asserções com parcimônia: cada uma é um lugar onde o compilador deixa de conferir.

<<< src/app/capitulos/cap02/registros.ts#incerta

A função **`ehInterpretacao`** devolve `r is Extract<...>`: é um **predicado de tipo**. Quando ela devolve `true`, o compilador passa a tratar o valor como uma interpretação. Por isso, depois do `.filter(ehInterpretacao)`, o `.reduce` enxerga `r.certeza` sem reclamar. `Extract<Registro, { tipo: 'interpretacao' }>` pega, da união, só o caso que tem esse `tipo`.

O `reduce` recebe um parâmetro de tipo explícito (`<... | undefined>`), porque o valor inicial é `undefined`. O resultado é honesto: **pode não haver** interpretação nenhuma, e quem chama precisa lidar com isso, como o teste faz com `?.descricao`.

::: csharp `?.` e `??` funcionam como no C#
`maisIncerta(dia)?.descricao` devolve `undefined` em vez de quebrar quando não há resultado, e `valor ?? 'padrão'` usa o padrão quando o valor é `null` ou `undefined`. A diferença está no `undefined`, que o C# não tem.
:::

## 5) O modo `strict`: um crítico que informa

O arquivo `tsconfig.json` diz ao compilador o quanto ele deve conferir. O projeto do livro liga tudo:

<<< tsconfig.json

As opções que mais fazem diferença no dia a dia:

| Opção | O que ela cobra |
|---|---|
| `strict` | Um pacote: `strictNullChecks` (o valor pode ser `undefined`? trate), `noImplicitAny` (sem tipos "qualquer coisa" escondidos) e outras |
| `noImplicitReturns` | Toda saída de uma função precisa devolver algo, se ela promete devolver |
| `noPropertyAccessFromIndexSignature` | Chaves dinâmicas se acessam com `obj['chave']`, o que deixa claro que podem não existir |
| `strictTemplates` (Angular) | Os **templates** também são conferidos: um `{{ semana().tem }}` com erro de digitação não compila |

Sem essas opções, o compilador se cala diante de muita coisa que vai quebrar em produção. Com elas, ele fala mais, e fala **antes**. Isso não é ataque: é o tipo de crítica que poupa você de uma noite de suporte.

## 6) Os testes

<<< src/app/capitulos/cap02/registros.spec.ts

Este arquivo testa funções puras, sem Angular nenhum: não precisa de `TestBed`. O último teste usa `as unknown as Registro` para **simular um dado vindo de fora** com um tipo que o compilador nunca deixaria passar, e confere que ele não passa em silêncio.

::: pratica Notar a voz que acusa
Na próxima vez que um erro aparecer, repare no primeiro pensamento que vem junto. Se for algo como "eu sou péssimo", só dê um nome a ele: **"autocrítica"**. Não precisa discutir com o pensamento nem trocá-lo por outro. Depois, leia a mensagem de erro inteira, devagar, como se fosse de um colega.
:::

::: exercicio Um quarto tipo de registro
Acrescente ao `Registro` um tipo `'gratidao'`, com um campo `motivo: string`. O que acontece com o código **antes** de você mexer em `descrever`? Depois, complete a função para que ela devolva `Grato por: <motivo>`.

--- resposta
Ao acrescentar o caso à união:

```ts
| { readonly tipo: 'gratidao'; readonly motivo: string };
```

o compilador acusa a linha `return casoEsquecido(r);`, com uma mensagem parecida com *Argument of type '{ tipo: "gratidao"; ... }' is not assignable to parameter of type 'never'*. É o `never` fazendo seu trabalho: o caso esquecido aparece **antes** de rodar.

Repare também no que ele **não** acusa: `TIPOS` sem `'gratidao'` continua passando no `satisfies` (ele confere que cada item é válido, não que **todos** estão lá), e `contarPorTipo` contaria `NaN` para gratidão, porque a chave não começou em zero. Os tipos ajudam muito, mas não substituem um teste com um registro de cada tipo.

Para resolver, acrescente `'gratidao'` em `TIPOS` e um caso no `switch`:

```ts
case 'gratidao':
  return `Grato por: ${r.motivo}`;
```
:::

::: resumo
- A mente tende a tratar sentimentos como problemas a resolver, e a autocrítica que parece ajudar costuma desgastar. O livro propõe **perceber** pensamentos como pensamentos.
- **TypeScript** é JavaScript com tipos que somem depois da compilação. `number`, `string`, `boolean`, `T[]`, `Record`, `const`/`let`, `===`.
- A tipagem é **estrutural**: vale a forma do objeto, não o nome da classe.
- **Uniões discriminadas** modelam casos com campos diferentes; o campo literal (`tipo`) permite o **estreitamento** dentro de cada `case`.
- **`never`** no `default` garante que nenhum caso foi esquecido; o `throw` protege contra dados que chegam de fora.
- **Genéricos**, **`Record`**, **`as const`**, **`satisfies`** e **predicados de tipo** (`x is T`) deixam o compilador trabalhar por você.
- O modo **`strict`** (e o `strictTemplates`) é um crítico que **informa**, e informa cedo.
:::

::: quiz
? Segundo Williams e Penman, por que analisar demais um sentimento costuma piorá-lo?
+ Porque a mente trata o sentimento como um problema prático, e as perguntas alimentam o próprio mal-estar
- Porque sentimentos não têm causa
- Porque pensar é sempre prejudicial
- Porque só a medicação resolve
> O modo que resolve problemas práticos não é o adequado para lidar com emoções, e a ruminação vira uma espiral.

? Em TypeScript, qual é o equivalente mais comum de `string?` do C#?
- `string!`
+ `string | undefined`
- `Nullable<string>`
- `optional string`
> Uma união: o valor é uma string ou não existe.

? O que torna uma união "discriminada"?
+ Um campo com um valor literal diferente em cada caso, como `tipo: 'fato'`
- O uso de `class` em vez de `interface`
- O uso de `enum`
- O uso de `any`
> Olhando o discriminante, o compilador sabe quais campos existem em cada caso.

? Para que serve `casoEsquecido(valor: never)` no `default` do `switch`?
- Para deixar o código mais rápido
- Para aceitar qualquer valor
+ Para o código parar de compilar se algum caso da união não for tratado
- Para converter o valor em texto
> Se sobrar algum caso, o tipo deixa de ser `never` e a chamada não compila.

? O que um predicado de tipo `r is Extract<Registro, { tipo: 'interpretacao' }>` permite?
+ Que, depois do `filter`, o compilador saiba que só restaram interpretações
- Que a função devolva qualquer tipo
- Que o filtro rode mais rápido
- Que o TypeScript ignore os erros
> O predicado transfere o conhecimento do `if` para o tipo do resultado.

? Por que deixar o modo `strict` ligado?
- Porque ele deixa o build mais rápido
+ Porque o compilador passa a apontar, antes de rodar, erros como valores que podem não existir
- Porque ele é obrigatório no Angular
- Porque ele desliga os testes
> É crítica que informa: aparece cedo e diz exatamente o que tratar.
:::

::: cartas
TypeScript :: JavaScript com tipos; o compilador confere e depois apaga os tipos.
Tipagem estrutural :: Vale a forma do objeto, não o nome da classe; objetos literais com os campos certos servem.
União :: `A | B`: um valor que pode ser de um tipo ou de outro.
União discriminada :: União em que cada caso tem um campo literal diferente (`tipo`), que permite estreitar o tipo.
never :: O tipo de "nenhum valor possível"; usado para garantir que todos os casos foram tratados.
Predicado de tipo :: Função que devolve `x is T`; quando dá `true`, o compilador passa a tratar `x` como `T`.
as const / satisfies :: `as const` fixa literais imutáveis; `satisfies` confere o tipo sem perder essa precisão.
strict :: Pacote de checagens do TypeScript (como `strictNullChecks`); com `strictTemplates`, vale também para os templates.
:::
