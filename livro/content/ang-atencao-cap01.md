---
id: ang-atencao-cap01
order: 322
kind: capitulo
num: 225
group: Angular · Parte 57 · Sair do piloto automático
title: Correndo atrás da própria cauda
subtitle: Quanto mais força fazemos para entender tudo de uma vez, menos entendemos. Começamos devagar, com o primeiro projeto Angular e um único componente.
minutes: 50
hue: 330
objectives:
- Entender por que "pensar mais forte" nem sempre resolve, e o que o livro propõe no lugar
- Criar um projeto com o **Angular CLI 22** e reconhecer as partes que importam
- Ler e escrever um **componente standalone**: decorador, seletor, template, estilos e classe
- Usar **interpolação**, `@for`, bindings de propriedade e de evento, e um primeiro **`signal`**
- Rodar o primeiro **teste** de componente
origem: angular/livros/01-atencao-plena-angular/content/01-cap01.md
---

::: cena O primeiro dia
Caio abriu o projeto Angular da equipe às nove da manhã. Às dez, tinha vinte abas abertas: documentação, um vídeo de 2019, uma pergunta antiga no fórum, a lista de mudanças da versão 22. Às onze, sabia um pouco de tudo e não conseguia explicar nada. Lia passou pela mesa, olhou as abas e disse: "Fecha tudo. Vamos fazer **um** componente que mostra **uma** lista. Só isso hoje."
:::

::: ilustracao img/ang-atencao/cap01.svg | Muitas abas puxando a atenção para todos os lados. Um componente por vez é um ponto de apoio.
:::

## O que o livro diz

Williams e Penman abrem o livro com uma cena conhecida: alguém deitado, tentando **forçar** a mente a se acalmar para dormir. Quanto mais força faz, mais acordado fica. A ideia que atravessa o livro começa aí: muitas vezes tentamos resolver cansaço, ansiedade e insatisfação **pensando mais**, analisando mais, cobrando mais de nós mesmos, e isso costuma piorar o problema.

O que eles propõem não é parar de pensar, e sim **prestar atenção de outro jeito**: perceber o que está acontecendo agora, de propósito, sem brigar com isso. E propõem começar pequeno, com uma prática de **um minuto**.

Nas nossas palavras: **quando a cabeça está correndo atrás da própria cauda, o próximo passo útil costuma ser menor, e não maior.**

::: cuidado Sobre aprender com pressa
Ninguém aprende um framework inteiro numa manhã, e não há nada de errado com você por isso. Sentir-se perdido no começo é parte normal do caminho, não um sinal de que você "não leva jeito".
:::

## Da vida para o código

| Na vida | No Angular |
|---|---|
| Tentar entender tudo de uma vez | Abrir vinte arquivos do projeto sem saber por onde entrar |
| Um ponto de apoio pequeno | **Um componente**: uma classe com um pedaço de tela |
| Prestar atenção no que está aqui | O **template** mostra exatamente o que a classe tem agora |
| Um passo de cada vez | Um **`signal`** guarda o passo atual e avisa quando ele muda |
| Conferir sem julgar | Um **teste** que diz o que funciona e o que não funciona |

## 1) Criando um projeto

O **Angular CLI** é a ferramenta de linha de comando que cria, roda, testa e empacota projetos. Você não precisa instalá-la globalmente: o `npx` baixa e executa a versão pedida.

::: terminal Criando o projeto do livro
```bash
npx @angular/cli@22 new atencao-plena --style=css --ssr=false
cd atencao-plena
npm start
```
:::

O CLI pergunta algumas coisas e, em um minuto, você tem um projeto que roda em `http://localhost:4200`. Das muitas pastas criadas, só algumas importam agora:

```texto
codigo/ang-atencao/
├─ angular.json          ← como construir, servir e testar o projeto
├─ package.json          ← dependências e scripts (npm start, npm test)
├─ tsconfig.json         ← regras do TypeScript (capítulo 2)
├─ public/               ← arquivos copiados como estão (ícone, JSON de exemplo)
└─ src/
   ├─ index.html         ← a única página HTML do app
   ├─ main.ts            ← o ponto de partida
   ├─ styles.css         ← estilos globais
   └─ app/
      ├─ app.ts          ← o componente raiz
      ├─ app.config.ts   ← a configuração do app inteiro
      └─ app.routes.ts   ← as rotas (capítulo 7)
```

::: novo O que mudou em relação ao Angular antigo
Se você já viu Angular antes, talvez estranhe a ausência de `app.module.ts`, de `zone.js` e de arquivos chamados `*.component.ts`. Nas versões recentes, **componentes são standalone** (não precisam de módulo), o app **não usa zone.js** (capítulo 5) e o CLI gera nomes curtos (`app.ts`, e a classe se chama `App`). O livro usa o Angular como ele é hoje.
:::

## 2) Como o app sobe

Tudo começa em `main.ts`. Ele pede ao Angular para **iniciar** o componente raiz com uma configuração:

<<< src/main.ts

A configuração lista os **provedores**, isto é, os serviços que o app inteiro pode usar: o roteador, o cliente HTTP, o tratamento de erros. Não se preocupe com os detalhes agora; cada um tem seu capítulo.

<<< src/app/app.config.ts

::: passos O que acontece quando você abre o app
O navegador baixa `index.html`, que tem uma tag estranha no `<body>`: `<app-root></app-root>`.
===
O navegador baixa e executa o JavaScript gerado a partir de `main.ts`.
===
`bootstrapApplication` encontra o componente `App`, cujo **seletor** é `app-root`, e o desenha dentro daquela tag.
===
A partir daí, o Angular cuida da tela: quando algo muda, ele atualiza só o pedaço necessário.
:::

## 3) Anatomia de um componente

Um **componente** é uma classe TypeScript com um **decorador** `@Component` que diz ao Angular: "esta classe controla um pedaço de tela". Este é o primeiro componente do livro, um roteiro curto para parar antes de codar:

<<< src/app/capitulos/cap01/pausa.ts

Leia de cima para baixo:

- **`@Component({...})`** é o decorador. Ele recebe os metadados do componente.
- **`selector: 'app-pausa'`** é o nome da tag HTML. Em qualquer template, `<app-pausa />` desenha este componente. O prefixo `app-` evita colisão com tags reais do HTML.
- **`template`** é o HTML do componente, aqui escrito no próprio arquivo (entre crases, que em TypeScript criam um texto de várias linhas). Componentes maiores costumam usar `templateUrl: './pausa.html'`, com o HTML num arquivo separado.
- **`styles`** são estilos que valem **só** para este componente. O `li.atual` daqui não afeta nenhum outro `li` do app.
- **A classe** guarda os dados e as ações. O template enxerga os membros `protected` e `public` da classe.

::: csharp Decorador × atributo
O decorador `@Component({...})` lembra um atributo do C# como `[ApiController]`: são metadados sobre a classe. A diferença é que, em TypeScript, o decorador é uma **função** que roda quando a classe é definida. Já `protected readonly` significa o mesmo que em C#: visível para a própria classe (e, no Angular, para o template dela) e sem reatribuição depois de criado.
:::

## 4) O template conversa com a classe

O template usa quatro formas de ligação (*binding*). Elas aparecem em praticamente todo componente:

| Forma | Exemplo | Direção | O que faz |
|---|---|---|---|
| Interpolação | `{{ titulo }}` | classe → tela | Escreve um valor como texto |
| Propriedade | `[disabled]="atual() === passos.length - 1"` | classe → tela | Liga uma propriedade do elemento a uma expressão |
| Classe | `[class.atual]="$index === atual()"` | classe → tela | Liga ou desliga uma classe CSS |
| Evento | `(click)="avancar()"` | tela → classe | Chama um método quando o evento acontece |

E o bloco **`@for`** repete um pedaço do template para cada item de uma lista:

```html
@for (passo of passos; track passo) {
  <li [class.atual]="$index === atual()">{{ passo }}</li>
}
```

O **`track`** diz ao Angular como reconhecer cada item entre uma atualização e outra (aqui, o próprio texto). Ele é obrigatório, e o capítulo 11 mostra por que importa para o desempenho. Dentro do `@for` existem variáveis prontas, como **`$index`** (a posição, a partir de 0).

::: erro Esquecer os parênteses do signal
Num template, `{{ atual }}` mostra algo estranho, porque `atual` é o signal inteiro, e não o número. Para **ler** um signal, chame-o como função: `{{ atual() }}`.
:::

## 5) O primeiro signal

Um **`signal`** é uma caixinha que guarda um valor e **avisa quem depende dele** quando o valor muda. É a peça central do Angular moderno, e o capítulo 3 é todo sobre ele. Por enquanto, três operações bastam:

| Operação | Código | Efeito |
|---|---|---|
| Criar | `signal(0)` | Uma caixinha começando em 0 |
| Ler | `atual()` | O valor atual |
| Trocar | `atual.set(0)` | Põe um valor novo |
| Atualizar a partir do anterior | `atual.update(n => n + 1)` | Calcula o novo a partir do velho |

No componente `Pausa`, `avancar()` usa `update` com `Math.min` para nunca passar do último passo, e `recomecar()` usa `set(0)`. Como o template **lê** `atual()`, o Angular sabe que precisa redesenhar aquele trecho quando o valor muda. Ninguém precisa avisar "a tela mudou": essa é a diferença entre um app que corre atrás da própria cauda e um que sabe o que mudou.

## 6) O primeiro teste

Um teste de componente cria o componente de verdade, sem navegador, e confere o que aparece na tela. O livro usa **Vitest**, o executor de testes padrão do Angular desde a versão 21:

<<< src/app/capitulos/cap01/pausa.spec.ts

- **`TestBed.createComponent(Pausa)`** monta o componente num ambiente de teste.
- **`await fixture.whenStable()`** espera o Angular terminar de desenhar. Use-o depois de criar o componente e depois de cada interação.
- **`fixture.nativeElement`** é o elemento HTML real, que você consulta com `querySelector`, como no navegador.
- **`describe`**, **`it`** e **`expect`** organizam o teste e fazem as conferências. Os nomes são frases: "avança um passo por clique e para no último".

::: terminal Rodando
```bash
npm run test:ci
```
A saída mostra cada arquivo de teste e quantos passaram. Se um falhar, o Vitest mostra o valor esperado e o valor recebido, lado a lado.
:::

::: pratica Um minuto
Ponha um cronômetro de um minuto (ou use o laboratório 1 deste livro, quando chegar lá). Sente-se com as costas apoiadas e os pés no chão. Leve a atenção para a respiração, onde ela for mais fácil de notar: no nariz, no peito ou na barriga. Quando perceber que a mente foi para outro lugar, e ela vai, só note isso e volte para a respiração. Não há como errar: perceber que se distraiu **é** a prática.
:::

::: exercicio Um passo para trás
Acrescente ao componente `Pausa` um botão **Voltar**, que volta um passo e fica **desabilitado** no primeiro. Depois, escreva um teste para ele.

--- resposta
No template, ao lado do botão "Próximo passo":

```html
<button type="button" (click)="voltar()" [disabled]="atual() === 0">Voltar</button>
```

Na classe:

```ts
protected voltar(): void {
  this.atual.update((n) => Math.max(n - 1, 0));
}
```

E o teste, no mesmo `describe`:

```ts
it('voltar não passa do primeiro passo', async () => {
  const { fixture, tela, botao } = await criar();
  botao('Próximo').click();
  botao('Voltar').click();
  botao('Voltar').click();
  await fixture.whenStable();

  expect(tela.textContent).toContain('Passo 1 de 5');
  expect(botao('Voltar').disabled).toBe(true);
});
```

`Math.max(n - 1, 0)` é o espelho do `Math.min` de `avancar`: as duas bordas ficam protegidas na classe, e o `[disabled]` só deixa isso visível para quem usa.
:::

::: resumo
- O livro começa com uma observação: forçar a mente a resolver tudo de uma vez costuma piorar as coisas. O próximo passo útil costuma ser **menor**.
- Um projeto Angular 22 nasce com `npx @angular/cli@22 new`. Tudo começa em `main.ts`, que inicia o componente raiz com a configuração de `app.config.ts`.
- Um **componente** é uma classe com `@Component`: **seletor** (a tag), **template** (o HTML), **estilos** (só dele) e os dados e ações da classe.
- O template liga dados por **interpolação** `{{ }}`, **propriedade** `[x]`, **classe** `[class.x]` e **evento** `(x)`, e repete trechos com **`@for`** (sempre com `track`).
- Um **`signal`** guarda um valor e avisa quem depende dele. Lê-se com `()`, troca-se com `set` e `update`.
- Um **teste** com `TestBed` cria o componente, interage com ele e confere a tela; `whenStable()` espera o Angular desenhar.
:::

::: quiz
? O que o livro de Williams e Penman propõe quando a mente está "correndo atrás da própria cauda"?
- Pensar com mais força até achar a solução
+ Prestar atenção de outro jeito, começando com algo pequeno
- Ignorar o que está sentindo
- Esperar as circunstâncias mudarem
> A proposta não é parar de pensar, e sim mudar a forma de prestar atenção, começando com um minuto.

? Para que serve o `selector` de um componente?
+ É o nome da tag HTML que desenha o componente em outros templates
- É o nome do arquivo do componente
- É o seletor CSS que estiliza o componente
- É a rota que abre o componente
> Com `selector: 'app-pausa'`, qualquer template pode usar `<app-pausa />`.

? Como se lê o valor de um signal chamado `atual` no template?
- `{{ atual }}`
+ `{{ atual() }}`
- `{{ atual.value }}`
- `{{ $atual }}`
> Signals são lidos chamando-os como função.

? Qual binding chama um método quando o botão é clicado?
- `[click]="avancar()"`
- `{{ avancar() }}`
+ `(click)="avancar()"`
- `[class.click]="avancar()"`
> Parênteses: da tela para a classe (eventos). Colchetes: da classe para a tela.

? Por que o `@for` exige `track`?
+ Para o Angular reconhecer cada item entre uma atualização e outra e reaproveitar o que não mudou
- Para ordenar a lista
- Para contar os itens
- Por compatibilidade com versões antigas
> O capítulo 11 mostra o efeito do `track` no desempenho.

? No teste, para que serve `await fixture.whenStable()`?
- Para esperar o navegador abrir
+ Para esperar o Angular terminar de atualizar a tela depois de uma mudança
- Para limpar o componente
- Para rodar os testes em paralelo
> Depois de criar ou interagir com o componente, espere a tela ficar estável antes de conferir.
:::

::: cartas
Angular CLI :: Ferramenta de linha de comando que cria, serve, testa e empacota projetos (`npx @angular/cli@22 new`).
main.ts :: Ponto de partida do app: chama `bootstrapApplication(App, appConfig)`.
Componente standalone :: Classe com `@Component` que não precisa de módulo; declara no `imports` o que usa.
selector :: Nome da tag que desenha o componente (`app-pausa`).
Interpolação :: `{{ expressao }}`: escreve um valor como texto no template.
Binding de propriedade :: `[propriedade]="expressao"`: liga a propriedade de um elemento a um valor da classe.
Binding de evento :: `(evento)="metodo()"`: chama um método quando o evento acontece.
signal :: Caixinha de valor que avisa quem depende dela; lê-se com `()`, muda-se com `set`/`update`.
:::
