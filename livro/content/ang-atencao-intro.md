---
id: ang-atencao-intro
order: 321
kind: intro
group: Angular · Parte 57 · Sair do piloto automático
title: Atenção Plena em Angular — apresentação
subtitle: Oito semanas de atenção plena como roteiro, Angular 22 como matéria. Um conceito por vez, com código que roda e é testado.
minutes: 15
hue: 330
badge: ★
objectives:
- Entender a ideia do livro: cada tema de **atenção plena** vira um **conceito do Angular**
- Ver o **mapa** dos 12 capítulos e dos 2 laboratórios
- Preparar o ambiente (**Node**, **Angular CLI 22**) e rodar o código do livro
origem: angular/livros/01-atencao-plena-angular/content/00-intro.md
---

::: ilustracao img/ang-atencao/intro.svg | Duas trilhas que andam juntas: uma para a cabeça (atenção), outra para o código (Angular).
:::

## Sobre este livro

Este é o **Livro 1** da coleção *Angular com Atenção*. Ele parte de **Atenção Plena** (*Mindfulness: um guia prático para encontrar a paz em um mundo frenético*), de **Mark Williams** e **Danny Penman**, um programa de oito semanas que ensina a perceber quando a mente está no **piloto automático** e a escolher, com mais calma, o que fazer em seguida.

A proposta aqui é dupla:

1. **Aprender Angular moderno de verdade**, do zero: componentes, signals, templates, rotas, injeção de dependência, RxJS, HTTP, formulários, desempenho e testes, na versão **22**.
2. **Estudar com menos pressa**. Cada capítulo traz uma prática curta e opcional, porque programar cansado e distraído custa caro, e a gente costuma perceber tarde.

O texto, os exemplos, o código e as ilustrações são **originais**. O livro **se inspira** nos temas da obra, não a resume nem a substitui: se o assunto atenção plena interessar a você, leia o original, que traz as práticas completas e os áudios guiados.

::: cuidado O que este livro não é
Nada aqui é terapia, diagnóstico ou tratamento. O programa de Williams e Penman nasceu de pesquisa clínica, e em contextos de saúde ele é conduzido por profissionais. As pausas deste livro são **exercícios de atenção para estudar melhor**, que você pode pular sem prejuízo. Se você está passando por sofrimento intenso ou persistente, procure um profissional de saúde. No Brasil, o **CVV atende pelo 188** (24 h, gratuito) e o **SAMU, pelo 192**.
:::

## A ideia central: o framework também tem um piloto automático

Atenção plena, em poucas palavras, é **prestar atenção ao que está acontecendo agora, de propósito e sem julgar**. Parece longe de programação, mas não é. Repare nos paralelos:

| Na atenção plena | No Angular |
|---|---|
| A mente faz muita coisa no **piloto automático** | O framework decide **sozinho** quando redesenhar a tela |
| Perceber o que realmente mudou | **Signals**: o Angular sabe exatamente o que mudou e o que depende disso |
| Tratar pensamentos como **eventos que passam** | **Observables (RxJS)**: fluxos de eventos que chegam, são tratados e passam |
| **Aproximar-se** do que é difícil em vez de fugir | Tratar **erros** de rede com cuidado, em vez de escondê-los |
| Notar o que **nutre** e o que **desgasta** | **Desempenho**: o que vale carregar agora e o que pode esperar |
| A prática que fica | **Testes**: o hábito que mantém o código saudável |

Não vamos forçar metáforas. Quando o paralelo ajuda a lembrar uma ideia, ele aparece. Quando não ajuda, o capítulo fala só de Angular.

## O mapa: 12 capítulos e 2 laboratórios

| Cap. | Tema da vida | O que você aprende de Angular |
|---|---|---|
| 1 | Correndo atrás da própria cauda | O primeiro projeto, componentes **standalone**, interpolação, `signal` |
| 2 | Por que nos atacamos? | **TypeScript** para quem vem do C#: uniões, `never`, genéricos, `strict` |
| 3 | Dois modos da mente | Imperativo × declarativo: **`signal`, `computed`, `effect`, `linkedSignal`** |
| 4 | O programa de oito semanas | Composição: **`input`, `output`, `model`**, projeção de conteúdo |
| Lab 1 | Pausa de um minuto | Um temporizador com signals, `DestroyRef` e testes com relógio falso |
| 5 | Semana 1 · Piloto automático | **Detecção de mudanças**, zoneless, **OnPush** × **Eager** |
| 6 | Semana 2 · Voltar ao corpo | **Templates**: `@if`, `@for`, `@switch`, `@let`, bindings e **acessibilidade** |
| 7 | Semana 3 · O rato no labirinto | **Rotas**, parâmetros como input, *lazy loading*, guardas, `resource` |
| 8 | Semana 4 · Pensamentos são eventos | **Serviços**, **injeção de dependência**, **RxJS** e interoperabilidade com signals |
| 9 | Semana 5 · Encarar o que é difícil | **HTTP**, `httpResource`, **interceptores**, retentativas e erros |
| 10 | Semana 6 · Passado e presente | **Signal Forms** (estáveis no 22), validação gentil, desfazer/refazer |
| 11 | Semana 7 · O que nutre e o que desgasta | **Desempenho**: `computed`, `track`, pipes puros, **`@defer`** |
| 12 | Semana 8 · Seguir praticando | **Testes** com Vitest, injeção de relógio, manutenção |
| Lab 2 | Diário de Atenção | Um app completo: rotas, store com signals, formulário, armazenamento, testes |

Cada capítulo tem o mesmo ritmo: **uma cena** com a equipe, **uma ilustração**, **a ideia do livro** em palavras nossas, **a ponte para o código**, o **código real** (o mesmo que está em `codigo/ang-atencao/` e passa nos testes), uma **prática curta**, um **exercício com resposta**, um **resumo**, um **quiz** e **cartões de revisão**.

## A equipe das cenas

As cenas acompanham uma pequena equipe que está migrando uma ferramenta interna de C# para Angular:

- **Caio**, desenvolvedor C# experiente, chegando agora ao front-end. Ele é você, em muitos momentos.
- **Lia**, desenvolvedora Angular, que explica as coisas com paciência e exemplos.
- **Bia**, líder técnica, que pergunta "como a gente sabe que funciona?".
- **Marta**, designer, que lembra que do outro lado da tela há uma pessoa.
- **Beto**, de qualidade, que encontra o caso que ninguém testou.

## O que você precisa saber antes

- **C# básico**: classes, interfaces, `async/await`, LINQ. É o conteúdo da coleção *Aprenda C#*. O livro compara C# e TypeScript sempre que ajuda (procure as caixas **Vindo do C#**).
- **HTML e CSS** no nível "sei o que é uma tag e uma classe". O resto é explicado quando aparece.
- **Não** precisa saber JavaScript, TypeScript nem nenhuma versão anterior do Angular. Aliás, se você conheceu o Angular antigo (com `NgModule`, `*ngIf` e zone.js), vai encontrar um framework bem diferente, e mais simples.

## Preparando o ambiente

::: terminal Instalar e rodar o código do livro
Você precisa do **Node.js 22.12 ou mais novo** (a versão LTS do site oficial serve). Depois, dentro da pasta `codigo/ang-atencao/` deste livro:

```bash
npm install          # baixa o Angular e as ferramentas (só na primeira vez)
npm start            # abre o Diário de Atenção em http://localhost:4200
npm run test:ci      # roda todos os testes uma vez
npm test             # roda os testes e fica observando as mudanças
```
:::

O código de cada capítulo mora em `codigo/ang-atencao/src/app/capitulos/capNN/`, com o arquivo de teste ao lado (`*.spec.ts`). Os laboratórios ficam em `codigo/ang-atencao/src/app/labs/lab01/` e em `codigo/ang-atencao/src/app/diario/`. Com o app rodando, o menu **Demonstrações** abre os componentes de cada capítulo.

::: dica Todo trecho de código deste livro é o arquivo de verdade
Os blocos de código com o caminho do arquivo no topo **não são cópias**: o gerador do livro lê o arquivo em `codigo/ang-atencao/` na hora de montar a página. Se o código mudar, o livro muda junto. E como o código tem testes, o que você lê **compila e funciona**.
:::

## Como aproveitar a leitura

- **Digite** o código em vez de copiar. Os dedos aprendem junto com a cabeça.
- **Leia um capítulo por vez**, de preferência com uma pausa entre eles. O programa original tem oito semanas por um motivo: hábitos levam tempo.
- **Rode os testes.** Quando um teste falha, ele está contando algo, não acusando ninguém.
- As **práticas curtas** são opcionais. Se uma delas não fizer sentido para você hoje, pule.

::: pratica Antes de seguir
Tire as mãos do teclado por alguns segundos. Note onde seu corpo encosta na cadeira e no chão. Acompanhe uma respiração inteira, do começo ao fim. Pronto: foi só isso. Pode seguir.
:::

::: cartas
Qual é a ideia do livro? :: Cada tema do programa de oito semanas de atenção plena vira um conceito do Angular 22, com código real e testado.
O que é atenção plena, em uma frase? :: Prestar atenção ao que está acontecendo agora, de propósito e sem julgar.
Este livro é terapia? :: Não. As pausas são exercícios de atenção para estudar melhor; em sofrimento, procure um profissional.
Onde fica o código de cada capítulo? :: Em `codigo/ang-atencao/src/app/capitulos/capNN/`, com o teste `*.spec.ts` ao lado.
Como rodar os testes uma vez só? :: `npm run test:ci` dentro da pasta `codigo/ang-atencao/`.
:::
