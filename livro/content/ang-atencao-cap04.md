---
id: ang-atencao-cap04
order: 325
kind: capitulo
num: 228
group: Angular · Parte 57 · Sair do piloto automático
title: O programa de oito semanas
subtitle: Um plano feito de partes pequenas que conversam. Componentes que recebem dados, avisam o que aconteceu e se encaixam uns nos outros.
minutes: 60
hue: 310
objectives:
- Entender a estrutura do programa do livro: oito semanas, uma prática principal e um pequeno hábito para soltar
- Separar componentes **de apresentação** (recebem e avisam) de componentes **que guardam estado**
- Usar **`input`**, **`input.required`**, **`output`** e **`model`** (duas vias)
- Encaixar conteúdo de fora com **`<ng-content />`** (projeção)
- Atualizar estado de forma **imutável** e testar componentes com `setInput`
origem: angular/livros/01-atencao-plena-angular/content/04-cap04.md
---

::: cena Quem é responsável pelo quê
Caio escreveu um componente "cartão da semana" que lia as semanas de um arquivo, guardava quais estavam feitas, mostrava o cartão e ainda salvava tudo. Funcionava. Marta pediu o mesmo cartão em outra tela, com outra lista. Caio percebeu que teria de copiar o componente inteiro. Lia sugeriu: "E se o cartão só **mostrasse** a semana e **avisasse** quando alguém marcasse? Quem guarda o estado é outra peça."
:::

::: ilustracao img/ang-atencao/cap04.svg | Oito cartões iguais por fora, cada um com a sua semana. Quem guarda o caminho todo é o componente de cima.
:::

## O que o livro diz

O coração do livro é um **programa de oito semanas**. Cada semana tem uma **prática principal**, feita na maioria dos dias, e um **hábito pequeno para soltar** (os autores chamam de *liberadores de hábitos*): coisas simples, como mudar de cadeira numa reunião ou fazer um caminho diferente, que quebram a rotina automática. O programa é progressivo: o que se aprende numa semana sustenta a seguinte.

Os autores também fazem advertências honestas: as práticas pedem tempo, nem todo dia vai "dar certo", e quem atravessa um momento muito difícil talvez precise começar com acompanhamento profissional, ou esperar um momento melhor para começar.

Nas nossas palavras: **um plano grande é feito de partes pequenas, cada uma com uma responsabilidade clara, que se encaixam.**

## Da vida para o código

| Na vida | No Angular |
|---|---|
| Uma semana do programa, com tema e prática | Um componente **de apresentação**: `CartaoDaSemana` |
| O que a semana recebe de fora | **`input`** (e `input.required`, quando é obrigatório) |
| Avisar que a semana foi praticada | **`output`**: um evento que o componente pai escuta |
| Um "feito/não feito" que os dois lados mexem | **`model`**: valor de duas vias |
| Quem acompanha o programa inteiro | Um componente **que guarda estado**: `Programa` |
| Um bilhete extra dentro do cartão | **Projeção de conteúdo** com `<ng-content />` |

## 1) Os dados: o mapa das semanas

<<< src/app/capitulos/cap04/semanas.ts

`readonly` nos campos e `readonly Semana[]` na lista avisam: ninguém deve mudar esses dados. O capítulo e o tema de Angular de cada semana estão aqui, e você vai reconhecê-los no sumário do livro.

## 2) Um componente que só mostra e avisa

<<< src/app/capitulos/cap04/cartao-da-semana.ts

Três funções novas, as três do pacote `@angular/core`:

- **`input.required<Semana>()`** declara uma entrada obrigatória. Dentro da classe, `semana` é um **signal somente leitura**: lê-se `this.semana()`. Se o componente pai esquecer de passar `[semana]`, o template **não compila** (lembra do `strictTemplates`?). Para entradas opcionais, `input(valorPadrao)`.
- **`output<number>()`** declara um evento. `this.concluiu.emit(4)` avisa "a semana 4 foi concluída", e o pai escuta com `(concluiu)="..."`.
- **`model(false)`** é uma entrada **e** uma saída ao mesmo tempo. O componente pode fazer `this.feita.set(true)`, e o pai fica sabendo pelo evento `feitaChange`. Com a sintaxe **banana na caixa** `[(feita)]="algumSignal"`, a ligação de duas vias fica completa.

E o **`<ng-content />`**? É um buraco no template. O que o pai escrever **entre** as tags `<app-cartao-da-semana>...</app-cartao-da-semana>` aparece ali. O cartão não precisa saber o que é.

::: csharp Inputs e outputs, vindo do C#
`input` lembra um parâmetro de construtor (o que o objeto recebe para existir), com a diferença de que pode mudar ao longo do tempo e por isso é um signal. `output` é o `event` do C#: o componente dispara, quem estiver inscrito reage. E `model` é como uma propriedade com `INotifyPropertyChanged` ligada nos dois sentidos.
:::

## 3) O componente que guarda o estado

<<< src/app/capitulos/cap04/programa.ts

- **`imports: [CartaoDaSemana]`**: um componente standalone declara o que usa no template. Esqueceu de importar? O compilador avisa que não conhece `<app-cartao-da-semana>`.
- **`[semana]="s"`** passa a semana do `@for`; **`[feita]`** e **`(feitaChange)`** são as duas metades do `model`, escritas separadamente porque o valor vem de um `Set` (e não de um signal próprio por cartão).
- **`(concluiu)="ultima.set($event)"`**: `$event` é o valor emitido, aqui o número da semana.
- **`@if (ultima(); as n)`** guarda o resultado da expressão em `n` para usar dentro do bloco.

### Atualizar sem mutar

Repare em `marcar`: em vez de mexer no `Set` que já existe (`atual.add(numero)`), ele cria um **novo** `Set` a cada mudança. Por quê? Um signal compara o valor novo com o antigo **pela referência**. Se você muta o mesmo objeto e devolve ele mesmo, para o signal "nada mudou", e quem depende dele não é avisado.

::: erro Mutar e devolver o mesmo objeto
`this.feitas.update(s => { s.add(n); return s; })` parece funcionar, mas é o mesmo `Set`: o signal considera que nada mudou. Crie um novo (`new Set(s)`, `[...lista]`, `{ ...objeto }`) e devolva o novo.
:::

## 4) Testando entradas e saídas

<<< src/app/capitulos/cap04/composicao.spec.ts

- **`fixture.componentRef.setInput('semana', SEMANAS[1])`** é o jeito de passar um `input` num teste: exatamente como o pai faria.
- **`fixture.componentInstance.concluiu.subscribe(...)`** escuta o `output`, como o pai escutaria.
- O segundo teste monta o `Programa` inteiro e confere o comportamento de fora: oito cartões, o texto projetado, a contagem. Não testamos `marcar` diretamente: testamos o que uma pessoa vê ao clicar.

::: dica Teste pelo lado de fora
Prefira testar componentes como uma pessoa os usa: clicar, digitar, ler a tela. Testes assim sobrevivem a refatorações por dentro (trocar um `Set` por um array, por exemplo) e quebram só quando o comportamento muda de verdade.
:::

::: pratica Um hábito pequeno para soltar
Escolha uma coisa automática de hoje e faça diferente, de propósito: sente em outra cadeira, use a outra mão para o mouse por cinco minutos, tome outro caminho até a cozinha. Não é para melhorar nada. É só para notar o quanto do dia acontece sem que a gente perceba.
:::

::: exercicio Um contador reutilizável
Crie um componente `app-contador-de-semanas` de **apresentação** que recebe `feitas` (número) e `total` (número, padrão 8) e mostra, por exemplo, *3 de 8 · faltam 5*. Use-o no `Programa` no lugar do parágrafo atual.

--- resposta
```ts
import { Component, computed, input } from '@angular/core';

@Component({
  selector: 'app-contador-de-semanas',
  template: `<p aria-live="polite">{{ feitas() }} de {{ total() }} · faltam {{ faltam() }}</p>`,
})
export class ContadorDeSemanas {
  readonly feitas = input.required<number>();
  readonly total = input(8);
  protected readonly faltam = computed(() => Math.max(0, this.total() - this.feitas()));
}
```

No `Programa`, acrescente `ContadorDeSemanas` ao `imports` e troque o parágrafo por:

```html
<app-contador-de-semanas [feitas]="feitas().size" [total]="semanas.length" />
```

Como `feitas` e `total` são signals (inputs), `faltam` pode ser um `computed`: o contador continua certo quando o pai muda qualquer um dos dois.
:::

::: resumo
- O programa do livro tem **oito semanas**, cada uma com uma prática principal e um pequeno hábito para soltar, e vem com advertências honestas sobre tempo e momento.
- Componentes **de apresentação** recebem dados e avisam o que aconteceu; componentes **com estado** guardam o estado e compõem os outros.
- **`input`** / **`input.required`** são entradas (signals somente leitura); **`output`** é um evento; **`model`** é entrada e saída (`[(x)]`).
- **`<ng-content />`** projeta o conteúdo escrito entre as tags do componente.
- Um componente standalone declara no **`imports`** os componentes que usa.
- Signals comparam pela **referência**: atualize criando um objeto novo, nunca mutando o antigo.
- Nos testes, **`setInput`** passa entradas e **`subscribe`** escuta saídas.
:::

::: quiz
? O que os autores chamam de "liberadores de hábitos"?
+ Pequenas mudanças de rotina, como trocar de cadeira, que quebram o piloto automático
- Remédios para insônia
- Meditações longas
- Listas de tarefas
> São atividades simples e propositalmente diferentes do automático.

? O que acontece se o componente pai não passar um `input.required`?
- O valor vira `undefined` em silêncio
+ O template do pai não compila
- O componente usa o valor padrão
- O app quebra só em produção
> Com `strictTemplates`, a entrada obrigatória ausente é um erro de compilação.

? Para que serve `model()`?
- Para buscar dados no servidor
+ Para um valor de duas vias: o componente pode mudá-lo e o pai fica sabendo
- Para declarar estilos
- Para criar um formulário
> É entrada e saída ao mesmo tempo; o pai usa `[(x)]` ou `[x]` + `(xChange)`.

? Por que `marcar` cria um novo `Set` em vez de usar `add` no existente?
+ Porque o signal compara pela referência; mutar o mesmo objeto não avisa ninguém
- Porque `Set` não tem `add`
- Porque é mais rápido
- Por exigência do TypeScript
> Objeto novo, referência nova, dependentes avisados.

? Onde aparece o conteúdo escrito entre `<app-cartao-da-semana>` e `</app-cartao-da-semana>`?
- Em lugar nenhum
- Antes do componente
+ No lugar do `<ng-content />` do template do cartão
- No console
> É a projeção de conteúdo.

? Num teste, como se passa um input para o componente?
- Atribuindo `fixture.componentInstance.semana = ...`
+ `fixture.componentRef.setInput('semana', valor)`
- Mudando o template
- Não é possível testar inputs
> `setInput` faz exatamente o que o pai faria.
:::

::: cartas
Componente de apresentação :: Recebe dados por input e avisa por output; não sabe de onde vêm nem para onde vão.
input / input.required :: Entrada do componente, lida como signal; `required` faz o pai ser obrigado a passar.
output :: Evento do componente: `emit(valor)` de um lado, `(evento)="..."` do outro.
model :: Entrada e saída ao mesmo tempo; `[(valor)]` liga nos dois sentidos.
ng-content :: Ponto do template onde aparece o conteúdo escrito entre as tags do componente.
imports (standalone) :: Lista dos componentes, diretivas e pipes que o template usa.
Atualização imutável :: Criar um objeto novo em vez de mudar o antigo, para o signal perceber a mudança.
setInput :: `fixture.componentRef.setInput(nome, valor)`: passa um input num teste.
:::
