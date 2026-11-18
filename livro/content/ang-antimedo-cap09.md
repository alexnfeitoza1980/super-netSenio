---
id: ang-antimedo-cap09
order: 309
kind: capitulo
num: 214
group: Angular · Parte 53 · Bloco II — Quebrando o Ciclo
title: Alienação
subtitle: Standalone Components — a alforria dos NgModules
minutes: 28
hue: 270
badge: 🗽
- Identificar a armadilha da alienação e a dependência desnecessária de intermediários: 
- Compreender a evolução do Angular do ecossistema de NgModules para Standalone: 
- Criar e compor componentes autônomos com standalone: true e imports explícitos
- Inicializar uma aplicação moderna usando bootstrapApplication sem AppModule: 
origem: angular/livros/01-antimedo-angular/content/09-alienacao.md
---

::: ilustracao img/ang-antimedo/cap09.svg | Uma corrente sendo partida ao meio: um componente Angular flutuando livre e independente.
:::

O capítulo sobre Alienação no livro *Antimedo* traz uma das frases mais desconfortáveis de Pablo Marçal:

> *"Alienação é a melhor forma de aprisionar alguém e fazer com que obedeça comandos sem perceber que o está fazendo. É omitir a gestão da própria vida e depender de uma instituição ou grupo para ter permissão de existir."*

Durante anos, quem aprendia Angular vivia sob essa exata forma de tutela arquitetural: **a tirania dos NgModules**.

Se você quisesse criar um simples botão com um texto colorido, você não podia simplesmente criar o componente e usá-lo. Você precisava:
1. Criar o componente.
2. Achar um `NgModule` para registrá-lo no array `declarations`.
3. Se quisesse usá-lo em outro lugar, ir no mesmo módulo e exportá-lo no array `exports`.
4. Ir no módulo de destino e importar o módulo de origem no array `imports`.
5. Se esquecesse qualquer vírgula nesse labirinto burocrático, o compilador lançava um erro críptico no terminal: *"Component is not part of any NgModule or is not declared in declarations"*.

O componente não tinha identidade própria. Ele era alienado, refém de um cartório central chamado `AppModule` ou `SharedModule`.

---

## O Dia da Independência: `standalone: true`

A partir do Angular 14 e tornando-se o padrão absoluto a partir do Angular 17, o Google decretou o fim da dependência obrigatória de módulos.

Nasceram os **Standalone Components** (Componentes Autônomos).

No modelo moderno, um componente é um **cidadão livre de primeira classe**. Ele governa a si mesmo. Se ele precisa de diretivas (`*ngIf`, `*ngFor`), de formulários ou de outros componentes, ele declara isso **diretamente no seu próprio cabeçalho**:

```typescript
// card-invencivel.component.ts
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-card-invencivel',
  // A FLAG DA LIBERDADE: Este componente não pertence a nenhum NgModule!
  standalone: true,
  // Ele mesmo declara tudo que precisa consumir no seu template
  imports: [CommonModule],
  template: `
    <div class="card-soberano">
      <h3>Componente Autônomo</h3>
      <p>Eu não dependo de nenhum NgModule para existir no mundo.</p>
    </div>
  `,
  styles: [`
    .card-soberano {
      border: 2px solid #dd1b44;
      padding: 20px;
      border-radius: 12px;
      background: #fff;
    }
  `]
})
export class CardInvencivelComponent {
  // 100% autossuficiente, reutilizável e independente!
}
```

---

## Como Compor Componentes Standalone

Para usar um componente dentro de outro, basta importá-lo no array `imports` do componente pai. Sem intermediários, sem atravessadores, com rastreabilidade total:

```typescript
// dashboard.component.ts
import { Component } from '@angular/core';
import { CardInvencivelComponent } from './card-invencivel.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  // Importação direta de componente para componente!
  imports: [CardInvencivelComponent],
  template: `
    <main class="painel">
      <h1>Painel Principal</h1>
      <!-- Usando diretamente o componente autônomo -->
      <app-card-invencivel></app-card-invencivel>
    </main>
  `
})
export class DashboardComponent {}
```

Veja a clareza disso: se você abrir o código do `DashboardComponent`, você sabe exatamente de onde veio o `CardInvencivelComponent`. Não há mais a "mágica obscura" de um `SharedModule` gigantesco de 500 componentes onde ninguém sabe o que realmente está sendo usado.

---

## A Inicialização Sem Módulos: `bootstrapApplication`

Antes, o ponto de partida de toda aplicação Angular era o arquivo `app.module.ts`. Hoje, o `AppModule` sequer precisa existir!

O arquivo `main.ts` inicializa diretamente o componente raiz através da função **`bootstrapApplication`**:

```typescript
// main.ts
import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from './app/app.component';
import { appConfig } from './app/app.config';

// Inicializa a aplicação diretamente pelo componente raiz standalone
bootstrapApplication(AppComponent, appConfig)
  .catch(err => console.error('Erro na inicialização:', err));
```

E toda a configuração global (rotas, clientes HTTP, interceptors) fica concentrada em um objeto limpo no `app.config.ts`:

```typescript
// app/app.config.ts
import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient } from '@angular/common/http';
import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient()
  ]
};
```

::: nota Adeus SharedModule Monstruoso!
O padrão antigo de criar um `SharedModule` com 80 módulos e componentes importados em massa gerava aplicações pesadas e cheias de código morto. Com Standalone, o mecanismo de **Tree Shaking** do compilador consegue eliminar com facilidade cirúrgica qualquer função que não esteja sendo usada na tela.
:::

---

::: exercicio Exercício: Criando um Botão Standalone Reutilizável
Crie um componente standalone chamado `BotaoAcaoComponent` com `@Input() texto: string` e `@Output() acionado` que possa ser importado diretamente por qualquer outra tela.

--- resposta
```typescript
import { Component, Input, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-botao-acao',
  standalone: true,
  template: `
    <button class="btn-antimedo" (click)="acionado.emit()">
      {{ texto }}
    </button>
  `,
  styles: [`
    .btn-antimedo {
      background: linear-gradient(135deg, #9b1230, #dd1b44);
      color: #fff;
      border: none;
      padding: 10px 24px;
      border-radius: 8px;
      font-weight: 700;
      cursor: pointer;
    }
  `]
})
export class BotaoAcaoComponent {
  @Input() texto = 'Executar Ação';
  @Output() acionado = new EventEmitter<void>();
}
```
:::

---

::: quiz
? Qual propriedade do decorador @Component declara que ele não depende de um NgModule?
+ standalone: true
- moduleFree: true
- isolated: true
- direct: true
> A propriedade `standalone: true` marca o componente como autônomo no Angular.

? Onde os componentes standalone declaram outros componentes e diretivas que pretendem usar?
+ No array `imports` do próprio decorador `@Component`
- No arquivo package.json
- No arquivo angular.json
- No banco de dados local do navegador
> Cada componente standalone declara suas próprias dependências no seu array `imports`.

? Qual função substitui o método legado platformBrowserDynamic().bootstrapModule(AppModule)?
+ bootstrapApplication(AppComponent, appConfig)
- runStandaloneApp()
- initAngularRoot()
- startClientApp()
> `bootstrapApplication` inicializa a aplicação diretamente a partir de um componente standalone raiz.

? Por que o modelo Standalone melhora o Tree Shaking e a performance?
+ Porque o compilador sabe exatamente quais componentes são usados, descartando código morto
- Porque ele desativa a compilação do TypeScript
- Porque ele comprime todas as imagens em SVG automaticamente
- Porque ele impede o uso de CSS
> A rastreabilidade explícita permite que o bundler remova tudo que não foi expressamente importado.

? É possível misturar componentes Standalone com bibliotecas antigas que ainda usam NgModules?
+ Sim, basta incluir o NgModule antigo dentro do array `imports` do componente standalone
- Não, o Angular quebra imediatamente se encontrar um NgModule antigo
- Não, é necessário reescrever todas as bibliotecas de terceiros do zero
- Sim, mas apenas aos fins de semana
> Standalone tem compatibilidade retroativa total: pode importar NgModules legados no seu array `imports`.
:::

---

::: cartas
Alienação no Código :: Dependência de estruturas burocráticas centrais (NgModules) para simples tarefas locais.
Standalone Component :: Componente autônomo com `standalone: true` que gerencia suas próprias dependências.
Array imports no @Component :: Lista explícita de módulos, pipes, diretivas e componentes que este template consome.
bootstrapApplication :: Função moderna para inicializar a aplicação sem a necessidade de um AppModule.
app.config.ts :: Arquivo central que agrupa os provedores globais da aplicação no modelo Standalone.
Tree Shaking :: Capacidade do bundler de eliminar do pacote final qualquer código que não seja utilizado.
:::

::: resumo
Pablo Marçal ensina que a alienação acontece quando você entrega o controle da sua jornada para intermediários. No Angular, os Standalone Components representam a maior revolução de simplicidade do framework: cada componente é soberano, autossuficiente e dono do seu próprio destino.
:::
