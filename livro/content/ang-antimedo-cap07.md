---
id: ang-antimedo-cap07
order: 307
kind: capitulo
num: 212
group: Angular · Parte 53 · Bloco II — Quebrando o Ciclo
title: O preço da inércia
subtitle: Lazy Loading — dividindo o peso para mover rápido
minutes: 30
hue: 210
badge: ⚡
- Entender como o conceito físico de inércia se manifesta no carregamento web: 
- Distinguir carregamento antecipado (Eager Loading) de sob demanda (Lazy Loading): 
- Configurar rotas assíncronas com loadComponent e loadChildren no Angular moderno: 
- Utilizar estratégias de pré-carregamento (PreloadAllModules) para eliminar atritos: 
origem: angular/livros/01-antimedo-angular/content/07-preco-da-inercia.md
---

::: ilustracao img/ang-antimedo/cap07.svg | Um foguete se libertando do peso das cargas desnecessárias para atingir velocidade de escape.
:::

A Primeira Lei de Newton define a inércia: um corpo em repouso tende a permanecer em repouso, e um corpo em movimento tende a permanecer em movimento, a menos que uma força externa atue sobre ele.

No livro *Antimedo*, Pablo Marçal faz um diagnóstico cirúrgico da inércia humana:

> *"A inércia não é apenas não fazer nada. A inércia é ter gastado toda a sua energia carregando coisas inúteis, de modo que não sobra combustível para o que realmente importa. Você tenta se mover, mas o peso do que você arrasta te ancora no mesmo lugar."*

Se você transportar essa verdade para o ecossistema frontend, descobrirá o maior vilão da performance na web: **o bundle monolítico inicial**.

---

## O Peso Morto do Eager Loading

Quando você constrói uma aplicação Angular sem planejamento de rotas, o compilador empacota todos os componentes, todas as bibliotecas, todas as telas de administração e todos os relatórios em um único arquivo gigantesco: o `main.js`.

Isso é chamado de **Eager Loading** (carregamento ansioso).

Imagine um usuário que só queria acessar a página inicial no celular 4G na rua. A sua aplicação força o navegador dele a baixar:
- A tela de login (que ele já fez)
- O painel financeiro avançado (que só o gerente usa)
- A tela de exportação de relatórios em PDF de 5MB
- Centenas de ícones e gráficos que ele nunca verá

**O resultado?** Uma tela branca por 8 segundos. O usuário desiste. A inércia da aplicação matou a experiência.

---

## A Cura: Lazy Loading (Carregamento Sob Demanda)

A solução arquitetural é o **Lazy Loading**. Em vez de carregar a aplicação inteira de uma vez só, o Angular quebra seu código em pequenos pedaços (*chunks*). 

O usuário só baixa o código de uma funcionalidade **no exato instante em que decide navegar até ela**.

Antes da navegação: zero bytes transferidos.  
No momento do clique: apenas o pedaço necessário é trazido pela rede.

```
          [ main.js ] (leve: apenas casca e rota inicial)
               │
      ┌────────┴────────┐
      ▼                 ▼
[ chunk-home.js ]  [ chunk-admin.js ]  <-- Baixado apenas se clicar em /admin!
```

---

## Implementando Rotas com Lazy Loading no Angular 17+

Com os **Standalone Components**, o Lazy Loading no Angular ficou mais simples e intuitivo do que nunca. Não precisamos mais de `NgModules` complexos; usamos a função nativa do JavaScript `import()` dentro de `loadComponent`:

```typescript
// app.routes.ts
import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'inicio'
  },
  {
    path: 'inicio',
    // Carregamento Eager (apenas para a rota de entrada crítica)
    loadComponent: () => import('./features/home/home.component')
      .then(m => m.HomeComponent)
  },
  {
    path: 'painel-antimedo',
    // LAZY LOADING: O arquivo só trafega na rede quando a rota for ativada
    loadComponent: () => import('./features/painel/painel.component')
      .then(m => m.PainelComponent)
  },
  {
    path: 'desafios',
    // Lazy loading de um subconjunto inteiro de rotas filhas
    loadChildren: () => import('./features/desafios/desafios.routes')
      .then(m => m.DESAFIOS_ROUTES)
  },
  {
    path: '**',
    loadComponent: () => import('./features/not-found/not-found.component')
      .then(m => m.NotFoundComponent)
  }
];
```

Veja como fica a subrota de desafios em um arquivo isolado:

```typescript
// features/desafios/desafios.routes.ts
import { Routes } from '@angular/router';

export const DESAFIOS_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./lista-desafios.component')
      .then(m => m.ListaDesafiosComponent)
  },
  {
    path: ':id',
    loadComponent: () => import('./detalhe-desafio.component')
      .then(m => m.DetalheDesafioComponent)
  }
];
```

---

## O Ensaio Mental do Navegador: Preloading Strategies

Pablo Marçal ensina a técnica do *ensaio mental*: antes do corpo chegar a um lugar desafiador, a mente já deve ter visualizado e chegado lá primeiro.

No Angular, podemos fazer o mesmo com **Preloading Strategies**!

1. O navegador carrega o pacote inicial ultrarrápido (rompe a inércia imediatamente).
2. Enquanto o usuário lê a página inicial com calma, em segundo plano (*background idle*), o Angular baixa silenciosamente as próximas telas prováveis.
3. Quando o usuário clica no botão, a transição é **instantânea (zero milissegundos)**, pois o arquivo já está na memória cache.

Veja como configurar no `app.config.ts`:

```typescript
import { ApplicationConfig } from '@angular/core';
import { provideRouter, withPreloading, PreloadAllModules } from '@angular/router';
import { routes } from './app.routes';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(
      routes,
      // Faz o download silencioso de todos os chunks lazy após o boot inicial
      withPreloading(PreloadAllModules)
    )
  ]
};
```

::: cuidado Cuidado com Redes Móveis Limitadas
O `PreloadAllModules` é excelente para a maioria das aplicações corporativas, mas se os seus usuários acessam via planos de dados 3G/4G restritos, você pode criar uma estratégia de pré-carregamento personalizada que só baixa rotas que tenham uma flag `data: { preload: true }`.
:::

---

::: exercicio Exercício: Criando um Preload Seletivo
Crie uma classe que implemente `PreloadingStrategy` para carregar em segundo plano apenas as rotas que possuam `route.data?.['preload'] === true`.

--- resposta
```typescript
import { Injectable } from '@angular/core';
import { PreloadingStrategy, Route } from '@angular/router';
import { Observable, of } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class PreloadEstrategicoService implements PreloadingStrategy {
  preload(route: Route, carregarChunk: () => Observable<any>): Observable<any> {
    // Se a rota tiver a marcação de prioridade, pré-carrega; senão, espera o clique real
    const devePrecarregar = route.data && route.data['preload'] === true;
    return devePrecarregar ? carregarChunk() : of(null);
  }
}
```
:::

---

::: quiz
? Qual a principal vantagem do Lazy Loading em relação ao Eager Loading?
+ Reduz o tamanho do pacote inicial e acelera o primeiro carregamento da tela
- Impede que erros de TypeScript aconteçam em produção
- Dispensa a necessidade de usar o Angular Router
- Converte requisições assíncronas em síncronas
> O Lazy Loading reduz o bundle inicial, baixando o código apenas quando o usuário visita a rota.

? No Angular com Standalone Components, qual função é usada para carregar um componente sob demanda?
+ loadComponent com dynamic import()
- requireComponent()
- importEager()
- moduleLoader()
> Usa-se a propriedade `loadComponent: () => import('./arquivo').then(m => m.Componente)`.

? O que faz a estratégia `PreloadAllModules`?
+ Baixa os módulos em segundo plano logo após o carregamento inicial da aplicação
- Força todos os componentes a carregarem sincronamente na largada
- Bloqueia requisições HTTP de páginas não visitadas
- Desativa o roteamento de componentes standalone
> Ela deixa a largada leve e pré-carrega os pedaços restantes no tempo ocioso do navegador.

? Como o arquivo resultante do build reflete o uso de Lazy Loading?
+ O build gera múltiplos arquivos pequenos (.js) chamados chunks, em vez de um só main.js gigante
- O build gera um arquivo .zip compactado
- O build elimina a pasta dist
- O build roda sem precisar do Node.js
> O compilador divide o código em chunks isolados que são requisitados sob demanda.

? Em qual momento o chunk de uma rota com Lazy Loading sem preloading é transferido pela rede?
+ No momento em que o usuário navega para a URL da rota correspondente
- Assim que o navegador abre a index.html
- Apenas quando o computador é reiniciado
- Durante a digitação do código no editor
> Sem preloading, o download ocorre no exato instante em que a rota é acionada.
:::

---

::: cartas
Inércia no Frontend :: Lentidão gerada por carregar código desnecessário antes que o usuário precise dele.
Eager Loading :: Carregamento sôfrego de toda a aplicação no bundle inicial, atrasando a primeira interação.
Lazy Loading :: Carregamento sob demanda que divide a aplicação em chunks independentes.
loadComponent :: Instrução de rota para importar dinamicamente um componente standalone.
loadChildren :: Instrução de rota para importar um grupo inteiro de subrotas sob demanda.
PreloadingStrategy :: Mecanismo para carregar chunks adicionais em segundo plano no tempo ocioso.
:::

::: resumo
Pablo Marçal ensina que a inércia é o peso do que você carrega sem precisar. No Angular, o Lazy Loading é a disciplina de empacotar apenas o necessário para o momento presente, deixando a aplicação veloz, responsiva e pronta para decolar.
:::
