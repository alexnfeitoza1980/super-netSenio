---
id: ang-antimedo-cap15
order: 315
kind: capitulo
num: 220
group: Angular · Parte 55 · Bloco IV — A Invencibilidade
title: Controle mental absoluto
subtitle: NgRx — controle absoluto do estado da aplicação
minutes: 40
hue: 185
badge: 🧠
- Entender o fluxo Pensamentos → Emoções → Ações → Resultados: 
- Configurar o NgRx Store com Actions, Reducers e Selectors: 
- Criar Effects para efeitos colaterais assíncronos: 
- Aplicar o padrão Redux para dominar o estado da aplicação: 
origem: angular/livros/01-antimedo-angular/content/15-controle-mental.md
---

# Controle mental absoluto

> *"Controlar a mente é mais poderoso do que um guerreiro que conquista uma cidade."*
> — Pablo Marçal

A filosofia estoica, os monges budistas, os atletas de elite e os grandes líderes da história têm algo em comum: **controle mental**. Não significa ausência de emoção — significa que as emoções não dirigem o carro. Você dirige.

Pablo Marçal traduz isso em uma equação clara:

**Pensamentos → Emoções → Ações → Resultados**

Mude o pensamento e tudo o mais muda. Não é autoajuda vaga — é uma cadeia de causalidade que pode ser gerenciada intencionalmente.

Em aplicações Angular de grande escala, existe exatamente esse problema: como gerenciar o estado de forma previsível, rastreável e controlada? A resposta é o **NgRx** — um framework de gerenciamento de estado baseado no padrão Redux.

::: nota
**NgRx** = **N**o **G**rade **Rx**. Combina Angular com RxJS para criar um fluxo unidirecional de dados: Action → Reducer → Store → Selector → Component → Action...
:::

---

## A analogia perfeita

| Conceito mental | Equivalente NgRx |
|----------------|-----------------|
| Pensamentos | **Actions** — eventos que descrevem o que aconteceu |
| Mente que processa | **Reducer** — função pura que transforma o estado |
| Estado mental | **Store** — a fonte da verdade, imutável e observável |
| Percepção | **Selector** — lê o estado de forma eficiente |
| Reações ao mundo | **Effects** — efeitos colaterais (HTTP, localStorage, etc.) |

---

## Instalando o NgRx

```bash
ng add @ngrx/store@latest
ng add @ngrx/effects@latest
ng add @ngrx/store-devtools@latest
```

```saida
✔ Packages installed successfully.
UPDATE src/app/app.config.ts
```

---

## Actions — os pensamentos do sistema

Actions são objetos simples que descrevem o que aconteceu. Eles não **fazem** nada — apenas **descrevem** o evento.

```typescript
// src/app/store/medos/medos.actions.ts
import { createAction, props } from '@ngrx/store';

interface Medo {
  id: string;
  descricao: string;
  nivel: number;
  enfrentado: boolean;
}

// Pensamento: "Quero carregar meus medos"
export const carregarMedos = createAction('[Medos] Carregar Medos');

// Pensamento: "Os medos chegaram com sucesso"
export const carregarMedosSucesso = createAction(
  '[Medos] Carregar Medos Sucesso',
  props<{ medos: Medo[] }>()
);

// Pensamento: "Algo deu errado ao carregar"
export const carregarMedosErro = createAction(
  '[Medos] Carregar Medos Erro',
  props<{ erro: string }>()
);

// Pensamento: "Quero adicionar um novo medo"
export const adicionarMedo = createAction(
  '[Medos] Adicionar Medo',
  props<{ medo: Omit<Medo, 'id'> }>()
);

// Pensamento: "Quero marcar um medo como enfrentado"
export const marcarEnfrentado = createAction(
  '[Medos] Marcar Enfrentado',
  props<{ id: string }>()
);

// Pensamento: "Quero remover um medo"
export const removerMedo = createAction(
  '[Medos] Remover Medo',
  props<{ id: string }>()
);
```

---

## Reducer — a mente que processa e transforma

O Reducer é uma **função pura**: dado o estado atual e uma action, retorna o novo estado. Sem efeitos colaterais. Sem mutação. Puro e previsível.

```typescript
// src/app/store/medos/medos.reducer.ts
import { createReducer, on } from '@ngrx/store';
import * as MedosActions from './medos.actions';

interface Medo {
  id: string;
  descricao: string;
  nivel: number;
  enfrentado: boolean;
}

export interface MedosState {
  medos: Medo[];
  carregando: boolean;
  erro: string | null;
}

const estadoInicial: MedosState = {
  medos: [],
  carregando: false,
  erro: null
};

export const medosReducer = createReducer(
  estadoInicial,

  // Quando o pensamento "carregar" chega → ativa carregando
  on(MedosActions.carregarMedos, state => ({
    ...state, carregando: true, erro: null
  })),

  // Quando os dados chegam → atualiza e desativa carregando
  on(MedosActions.carregarMedosSucesso, (state, { medos }) => ({
    ...state, medos, carregando: false
  })),

  // Quando há erro → registra e desativa carregando
  on(MedosActions.carregarMedosErro, (state, { erro }) => ({
    ...state, erro, carregando: false
  })),

  // Quando adiciona → novo array com o item adicionado
  on(MedosActions.adicionarMedo, (state, { medo }) => ({
    ...state,
    medos: [...state.medos, { ...medo, id: crypto.randomUUID() }]
  })),

  // Quando marca como enfrentado → imutavelmente atualiza apenas 1
  on(MedosActions.marcarEnfrentado, (state, { id }) => ({
    ...state,
    medos: state.medos.map(m =>
      m.id === id ? { ...m, enfrentado: true } : m
    )
  })),

  // Quando remove → filtra o item
  on(MedosActions.removerMedo, (state, { id }) => ({
    ...state,
    medos: state.medos.filter(m => m.id !== id)
  }))
);
```

---

## Selectors — a percepção afinada do estado

Selectors são funções que leem o estado de forma eficiente. Eles **memoizam** o resultado — só recalculam quando o estado relevante muda.

```typescript
// src/app/store/medos/medos.selectors.ts
import { createFeatureSelector, createSelector } from '@ngrx/store';
import { MedosState } from './medos.reducer';

// Feature selector — porta de entrada para a feature
const selectMedosState = createFeatureSelector<MedosState>('medos');

// Selectors específicos
export const selectTodosMedos = createSelector(
  selectMedosState,
  state => state.medos
);

export const selectCarregando = createSelector(
  selectMedosState,
  state => state.carregando
);

export const selectErro = createSelector(
  selectMedosState,
  state => state.erro
);

// Selectors derivados (como computed em Signals)
export const selectMedosNaoEnfrentados = createSelector(
  selectTodosMedos,
  medos => medos.filter(m => !m.enfrentado)
);

export const selectMedosCriticos = createSelector(
  selectTodosMedos,
  medos => medos.filter(m => m.nivel >= 8)
);

export const selectEstatisticas = createSelector(
  selectTodosMedos,
  medos => ({
    total: medos.length,
    enfrentados: medos.filter(m => m.enfrentado).length,
    pendentes: medos.filter(m => !m.enfrentado).length,
    nivelMedio: medos.length
      ? medos.reduce((s, m) => s + m.nivel, 0) / medos.length
      : 0
  })
);
```

---

## Effects — ações que geram reações no mundo

Effects lidam com efeitos colaterais: chamadas HTTP, localStorage, timers. Eles ouvem Actions e disparam novas Actions como resultado.

```typescript
// src/app/store/medos/medos.effects.ts
import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { switchMap, map, catchError } from 'rxjs/operators';
import { of } from 'rxjs';
import { MedoService } from '../../core/services/medo.service';
import * as MedosActions from './medos.actions';

@Injectable()
export class MedosEffects {
  private actions$ = inject(Actions);
  private medoService = inject(MedoService);

  // Quando "carregarMedos" é despachada → vai ao servidor
  carregarMedos$ = createEffect(() =>
    this.actions$.pipe(
      ofType(MedosActions.carregarMedos),
      switchMap(() =>
        this.medoService.listarMedos().pipe(
          map(medos => MedosActions.carregarMedosSucesso({ medos })),
          catchError(erro =>
            of(MedosActions.carregarMedosErro({ erro: erro.message }))
          )
        )
      )
    )
  );
}
```

---

## Conectando tudo — componente com NgRx

```typescript
// src/app/features/medos/lista-medos/lista-medos.component.ts
import { Component, inject, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { AsyncPipe, NgIf } from '@angular/common';
import * as MedosActions from '../../../store/medos/medos.actions';
import * as MedosSelectors from '../../../store/medos/medos.selectors';

@Component({
  selector: 'app-lista-medos',
  standalone: true,
  imports: [AsyncPipe, NgIf],
  template: `
    @if (carregando$ | async) {
      <p>Carregando seus medos...</p>
    }
    @if (erro$ | async; as erro) {
      <p class="erro">{{ erro }}</p>
    }
    <div class="stats">
      @if (stats$ | async; as stats) {
        <p>Total: {{ stats.total }} | Enfrentados: {{ stats.enfrentados }}</p>
        <p>Nível médio: {{ stats.nivelMedio | number:'1.1-1' }}</p>
      }
    </div>
    <ul>
      @for (medo of medos$ | async; track medo.id) {
        <li>
          <span>{{ medo.descricao }} ({{ medo.nivel }}/10)</span>
          @if (!medo.enfrentado) {
            <button (click)="enfrentar(medo.id)">Enfrentar!</button>
          }
        </li>
      }
    </ul>
    <button (click)="recarregar()">↻ Recarregar</button>
  `
})
export class ListaMedosComponent implements OnInit {
  private store = inject(Store);

  // Conecta com selectors — percepção do estado
  medos$ = this.store.select(MedosSelectors.selectMedosNaoEnfrentados);
  carregando$ = this.store.select(MedosSelectors.selectCarregando);
  erro$ = this.store.select(MedosSelectors.selectErro);
  stats$ = this.store.select(MedosSelectors.selectEstatisticas);

  ngOnInit(): void {
    // Despacha o pensamento inicial
    this.store.dispatch(MedosActions.carregarMedos());
  }

  enfrentar(id: string): void {
    // Despacha o pensamento de ação
    this.store.dispatch(MedosActions.marcarEnfrentado({ id }));
  }

  recarregar(): void {
    this.store.dispatch(MedosActions.carregarMedos());
  }
}
```

---

## Configurando o NgRx no `app.config.ts`

```typescript
// src/app/app.config.ts
import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideStore } from '@ngrx/store';
import { provideEffects } from '@ngrx/effects';
import { provideStoreDevtools } from '@ngrx/store-devtools';
import { routes } from './app.routes';
import { medosReducer } from './store/medos/medos.reducer';
import { MedosEffects } from './store/medos/medos.effects';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideStore({ medos: medosReducer }),
    provideEffects([MedosEffects]),
    provideStoreDevtools({
      maxAge: 25,        // Guarda últimas 25 actions
      logOnly: false     // false = permite time-travel debugging
    })
  ]
};
```

::: dica
Instale a extensão **Redux DevTools** no navegador para visualizar cada action despachada, inspecionar o estado após cada mudança e até fazer "time-travel" — voltar no tempo para qualquer estado anterior. É o controle mental absoluto materializado em ferramenta.
:::

---

::: resumo
**NgRx** implementa o controle mental absoluto da sua aplicação:

- **Actions** — pensamentos que descrevem o que aconteceu (sem efeitos)
- **Reducer** — mente pura que processa e transforma o estado
- **Store** — o estado mental global, imutável e observável
- **Selectors** — percepção afinada que lê o estado com memoização
- **Effects** — reações ao mundo externo (HTTP, storage, timers)

O fluxo é sempre unidirecional: **Action → Reducer → Store → Selector → Component → Action**. Previsível, rastreável, testável.
:::

---

::: exercicio Store para progresso de enfrentamento
Implemente uma feature NgRx `progresso` que:
1. Rastreia ações de enfrentamento com `data` e `medoId`
2. Calcula o `totalEnfrentados` com um selector
3. Expõe um selector `sequenciaAtual` que conta dias consecutivos

--- resposta

```typescript
// src/app/store/progresso/progresso.actions.ts
import { createAction, props } from '@ngrx/store';

export const registrarEnfrentamento = createAction(
  '[Progresso] Registrar Enfrentamento',
  props<{ medoId: string; data: string }>()
);

// src/app/store/progresso/progresso.reducer.ts
import { createReducer, on } from '@ngrx/store';
import { registrarEnfrentamento } from './progresso.actions';

interface Enfrentamento { medoId: string; data: string; }

export interface ProgressoState {
  enfrentamentos: Enfrentamento[];
}

const inicial: ProgressoState = { enfrentamentos: [] };

export const progressoReducer = createReducer(
  inicial,
  on(registrarEnfrentamento, (state, { medoId, data }) => ({
    enfrentamentos: [...state.enfrentamentos, { medoId, data }]
  }))
);

// src/app/store/progresso/progresso.selectors.ts
import { createFeatureSelector, createSelector } from '@ngrx/store';
import { ProgressoState } from './progresso.reducer';

const selectProgresso = createFeatureSelector<ProgressoState>('progresso');

export const selectTotalEnfrentados = createSelector(
  selectProgresso,
  s => s.enfrentamentos.length
);

export const selectSequenciaAtual = createSelector(
  selectProgresso,
  ({ enfrentamentos }) => {
    const datas = [...new Set(enfrentamentos.map(e => e.data))].sort();
    let sequencia = 0;
    const hoje = new Date();
    for (let i = datas.length - 1; i >= 0; i--) {
      const esperado = new Date(hoje);
      esperado.setDate(esperado.getDate() - (datas.length - 1 - i));
      if (datas[i] === esperado.toISOString().split('T')[0]) sequencia++;
      else break;
    }
    return sequencia;
  }
);
```

:::

---

::: quiz

? Qual é o papel de uma Action no NgRx?

- Modifica o estado diretamente
- Faz chamadas HTTP
+ Descreve o que aconteceu, sem efeitos colaterais
- É equivalente a um service do Angular

> Actions são objetos simples que descrevem eventos. Eles **não** modificam o estado — quem faz isso é o Reducer. Separar o "o que aconteceu" do "o que fazer com isso" é o princípio central do Redux.

? O que garante que o Reducer é previsível?

- É assíncrono por padrão
- Usa `HttpClient` internamente
+ É uma função pura — dado o mesmo input, sempre retorna o mesmo output
- Modifica o estado via referência

> Uma função pura não tem efeitos colaterais e não modifica o estado — ela retorna um **novo** objeto. Isso permite time-travel debugging, testes fáceis e comportamento previsível.

? Para que serve `createSelector` com memoização?

- Para criar Actions tipadas
- Para fazer chamadas HTTP
+ Para ler o estado de forma eficiente, recalculando apenas quando as dependências mudam
- Para registrar efeitos colaterais

> Selectors com memoização guardam o último resultado. Se o estado de entrada não mudou, retornam o mesmo objeto — evitando re-renders desnecessários em componentes.

? O que `Effects` devem fazer que `Reducers` não podem?

- Transformar o estado
- Receber Actions
+ Realizar efeitos colaterais: chamadas HTTP, acesso ao localStorage, timers
- Criar selectors

> Reducers são funções puras — sem efeitos colaterais. Effects existem para o mundo "sujo": HTTP, storage, WebSockets. Eles escutam Actions e emitem novas Actions com o resultado.

? Como se despacha uma Action de um componente?

- `actions.emit(minhaAction())`
- `reducer.dispatch(minhaAction())`
+ `store.dispatch(minhaAction())`
- `effects.send(minhaAction())`

> `Store` é o único ponto de entrada para despachar actions. O componente injeta `Store` e chama `store.dispatch(action())`.

:::

---

::: cartas

`createAction` :: Cria uma action tipada com `props<{}>()` opcionais
`createReducer` + `on()` :: Cria o reducer com handlers para cada action
`createSelector` :: Cria selector com memoização para leitura eficiente do estado
`createEffect` :: Cria effect para efeitos colaterais assíncronos
`ofType()` :: Filtra o stream de Actions pelo tipo — ex: `ofType(carregarMedos)`
`provideStore` :: Registra os reducers no `app.config.ts`
`provideEffects` :: Registra os effects no `app.config.ts`
`store.dispatch` :: Despacha uma action de um componente ou service

:::
