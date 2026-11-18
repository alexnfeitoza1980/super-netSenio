---
id: ang-antimedo-cap14
order: 314
kind: capitulo
num: 219
group: Angular · Parte 55 · Bloco IV — A Invencibilidade
title: Lutar ou Fugir
subtitle: Angular Router — escolhendo o caminho com intenção
minutes: 30
hue: 175
badge: 🧭
- Entender a resposta instintiva de lutar ou fugir e a terceira via: 
- Navegar entre rotas com Router, RouterLink e navegação programática: 
- Usar parâmetros de rota e query params para navegação intencional: 
- Implementar lazy loading de rotas para performance: 
origem: angular/livros/01-antimedo-angular/content/14-lutar-ou-fugir.md
---

# Lutar ou Fugir

> *"O sistema nervoso autônomo escolhe entre lutar ou fugir. Mas há uma terceira via: a ação estratégica."*
> — Pablo Marçal

Quando você enfrenta um medo, seu sistema nervoso reage antes mesmo que você pense. O coração acelera, as palmas suam, a adrenalina dispara. É o instinto primitivo: **lutar** (enfrentar) ou **fugir** (escapar). Esse sistema salvou vidas por milhões de anos.

Mas hoje, em contextos modernos, ele frequentemente sabota. Você não enfrenta a apresentação — você foge com procrastinação. Você não enfrenta a conversa difícil — você luta de forma agressiva e sem estratégia.

Pablo Marçal propõe a **terceira via**: nem lutar cegamente, nem fugir covardemente. Agir com intenção. Escolher o caminho consciente. Em Angular, isso é exatamente o que o **Router** faz — ele não apenas navega; ele escolhe o caminho com intenção, passando contexto, guardando histórico e permitindo navegação estratégica.

::: nota
**Exercício do livro:** Liste 10 coisas que você tem fugido. Para cada uma, escreva: "O que eu vou fazer sobre isso?" A ação intencional é a terceira via.
:::

---

## As três vias no Router Angular

| Via | Instinto | Equivalente Angular |
|-----|---------|-------------------|
| **Fugir** | Evitar o desafio | `router.navigate(['/inicio'])` sem contexto |
| **Lutar** | Avançar sem estratégia | `router.navigate(['/desafio'])` sem parâmetros |
| **Ação estratégica** | Avançar com plano | `router.navigate(['/desafio'], { queryParams: { nivel: 'avancado', retorno: '/progresso' } })` |

---

## Configurando o Router

```typescript
// src/app/app.routes.ts
import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  // Rota raiz
  { path: '', redirectTo: '/inicio', pathMatch: 'full' },

  // Rota simples
  { path: 'inicio', loadComponent: () =>
      import('./features/inicio/inicio.component')
        .then(m => m.InicioComponent)
  },

  // Rota com parâmetro (:id)
  { path: 'medo/:id', loadComponent: () =>
      import('./features/medo/medo-detalhe/medo-detalhe.component')
        .then(m => m.MedoDetalheComponent)
  },

  // Rota protegida com lazy loading
  { path: 'dashboard',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/dashboard/dashboard.component')
        .then(m => m.DashboardComponent)
  },

  // Rota com filhos (feature module pattern)
  { path: 'medos',
    canActivate: [authGuard],
    loadChildren: () =>
      import('./features/medos/medos.routes')
        .then(m => m.medosRoutes)
  },

  // Wildcard — para rotas não encontradas
  { path: '**', loadComponent: () =>
      import('./features/nao-encontrado/nao-encontrado.component')
        .then(m => m.NaoEncontradoComponent)
  }
];
```

```typescript
// src/app/features/medos/medos.routes.ts
import { Routes } from '@angular/router';

export const medosRoutes: Routes = [
  { path: '', loadComponent: () =>
      import('./lista-medos/lista-medos.component')
        .then(m => m.ListaMedosComponent)
  },
  { path: 'novo', loadComponent: () =>
      import('./cadastrar-medo/cadastrar-medo.component')
        .then(m => m.CadastrarMedoComponent)
  },
  { path: ':id/progresso', loadComponent: () =>
      import('./progresso/progresso.component')
        .then(m => m.ProgressoComponent)
  }
];
```

---

## Navegação declarativa com `RouterLink`

```typescript
// src/app/features/inicio/inicio.component.ts
import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-inicio',
  standalone: true,
  imports: [RouterLink, RouterLinkActive],
  template: `
    <nav>
      <!-- Link simples -->
      <a routerLink="/dashboard" routerLinkActive="ativo">
        Dashboard
      </a>

      <!-- Link com parâmetro -->
      <a [routerLink]="['/medo', medo.id]">
        Ver medo {{ medo.nome }}
      </a>

      <!-- Link com query params -->
      <a
        [routerLink]="['/medos']"
        [queryParams]="{ ordem: 'nivel', filtro: 'nao-enfrentados' }"
      >
        Meus medos pendentes
      </a>

      <!-- Link com fragment (#ancora) -->
      <a routerLink="/sobre" fragment="contato">
        Falar com a equipe
      </a>
    </nav>

    <!-- Onde as rotas filhas são renderizadas -->
    <router-outlet />
  `
})
export class InicioComponent {
  medo = { id: '42', nome: 'falar em público' };
}
```

---

## Lendo parâmetros de rota — `ActivatedRoute`

```typescript
// src/app/features/medo/medo-detalhe/medo-detalhe.component.ts
import { Component, OnInit, inject, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { switchMap, map } from 'rxjs/operators';
import { MedoService } from '../../../core/services/medo.service';

interface Medo {
  id: string;
  descricao: string;
  nivel: number;
  enfrentado: boolean;
}

@Component({
  selector: 'app-medo-detalhe',
  standalone: true,
  template: `
    @if (medo(); as m) {
      <h1>{{ m.descricao }}</h1>
      <p>Nível: {{ m.nivel }}/10</p>
      <p>Status: {{ m.enfrentado ? '✅ Enfrentado' : '⏳ Pendente' }}</p>
      <button (click)="enfrentar()">Enfrentar agora!</button>
      <button (click)="voltar()">← Voltar</button>
    } @else {
      <p>Carregando...</p>
    }
  `
})
export class MedoDetalheComponent {
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private medoService = inject(MedoService);

  // Lê o parâmetro :id e busca o medo automaticamente
  medo = toSignal(
    this.route.paramMap.pipe(
      map(params => params.get('id')!),
      switchMap(id => this.medoService.buscarPorId(id))
    )
  );

  // Lendo query params
  filtro = toSignal(
    this.route.queryParamMap.pipe(
      map(params => params.get('filtro') ?? 'todos')
    ),
    { initialValue: 'todos' }
  );

  enfrentar(): void {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.medoService.marcarEnfrentado(id).subscribe(() => {
      // Terceira via: navega com contexto
      this.router.navigate(['/medos'], {
        queryParams: { destaque: id, mensagem: 'parabens' }
      });
    });
  }

  voltar(): void {
    // Usa o histórico do browser — não foge, volta conscientemente
    history.back();
  }
}
```

---

## Navegação programática — a terceira via em ação

```typescript
// src/app/features/desafio/desafio.component.ts
import { Component, inject } from '@angular/core';
import { Router, NavigationExtras } from '@angular/router';

@Component({
  selector: 'app-desafio',
  standalone: true,
  template: `
    <h2>Qual caminho você vai escolher?</h2>
    <button (click)="lutar()">⚔️ Enfrentar o desafio</button>
    <button (click)="terceirVia()">🎯 Planejar estrategicamente</button>
    <button (click)="fugir()">🏃 Voltar ao início</button>
  `
})
export class DesafioComponent {
  private router = inject(Router);

  // Lutar — navega direto, sem contexto
  lutar(): void {
    this.router.navigate(['/desafio/avancado']);
  }

  // A terceira via — navega com intenção e contexto
  terceirVia(): void {
    const extras: NavigationExtras = {
      queryParams: {
        nivel: 'estrategico',
        tempo: '30min',
        suporte: 'ativo'
      },
      fragment: 'plano-de-acao',
      state: { origemDesafio: true, timestampInicio: Date.now() }
    };
    this.router.navigate(['/desafio/plano'], extras);
  }

  // Fugir — volta sem contexto
  fugir(): void {
    this.router.navigate(['/inicio']);
  }

  // Verificar rotas ativas
  verificarRota(): void {
    const url = this.router.url;
    console.log('URL atual:', url);
    console.log('Histórico navegação:', this.router.getCurrentNavigation());
  }
}
```

---

::: dica
Use `router.navigate()` para navegação programática (após ação do usuário, resposta de HTTP). Use `routerLink` no template para links declarativos. Para dados temporários entre rotas sem expô-los na URL, use `state` em `NavigationExtras` — acessível via `router.getCurrentNavigation()?.extras.state`.
:::

---

::: passos Implementando lazy loading com performance

=== Passo 1 — Estruture as features como módulos independentes

```
src/app/features/
├── medos/
│   ├── medos.routes.ts      ← rotas da feature
│   ├── lista-medos/
│   ├── cadastrar-medo/
│   └── progresso/
└── dashboard/
    └── dashboard.component.ts
```

=== Passo 2 — Configure lazy loading nas rotas principais

```typescript
// src/app/app.routes.ts
export const routes: Routes = [
  {
    path: 'medos',
    loadChildren: () =>
      import('./features/medos/medos.routes')
        .then(m => m.medosRoutes)
  }
];
```

=== Passo 3 — Adicione preloading para rotas críticas

```typescript
// src/app/app.config.ts
import { PreloadAllModules, provideRouter, withPreloading } from '@angular/router';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(
      routes,
      withPreloading(PreloadAllModules) // Carrega em background após início
    )
  ]
};
```

:::

---

::: resumo
O **Angular Router** é a terceira via do seu app: nem navega cegamente, nem fica parado.

- `RouterLink` / `routerLink` — links declarativos no template
- `ActivatedRoute` — lê parâmetros, query params e fragmentos da rota atual
- `Router.navigate()` — navegação programática com `NavigationExtras`
- `loadComponent` — lazy loading de componentes standalone
- `loadChildren` — lazy loading de módulos/features completos
- `router.createUrlTree()` — cria URLs para redirecionamento em guards

A ação estratégica passa parâmetros. A fuga não deixa rastro. O combate cego não tem contexto. Navegue com intenção.
:::

---

::: exercicio Navegação com "terceira via"
Crie um componente `OpcoesComponent` que:
1. Recebe um `medo` via query param `?medoId=...`
2. Exibe três botões: "Fugir", "Enfrentar" e "Planejar"
3. "Fugir" navega para `/inicio` sem contexto
4. "Enfrentar" navega para `/medo/:id/desafio` imediatamente
5. "Planejar" navega para `/medo/:id/plano` com `queryParams: { prazo: '7d', suporte: 'true' }`

--- resposta

```typescript
// src/app/features/opcoes/opcoes.component.ts
import { Component, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-opcoes',
  standalone: true,
  template: `
    <h2>Como você vai lidar com este medo?</h2>
    <p>Medo ID: {{ medoId() }}</p>
    <button (click)="fugir()">🏃 Fugir</button>
    <button (click)="enfrentar()">⚔️ Enfrentar agora</button>
    <button (click)="planejar()">🎯 Planejar com estratégia</button>
  `
})
export class OpcoesComponent {
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  medoId = toSignal(
    this.route.queryParamMap.pipe(
      map(p => p.get('medoId') ?? '')
    ),
    { initialValue: '' }
  );

  fugir(): void {
    this.router.navigate(['/inicio']);
  }

  enfrentar(): void {
    this.router.navigate(['/medo', this.medoId(), 'desafio']);
  }

  planejar(): void {
    this.router.navigate(
      ['/medo', this.medoId(), 'plano'],
      { queryParams: { prazo: '7d', suporte: 'true' } }
    );
  }
}
```

:::

---

::: quiz

? Como se acessa o valor de um parâmetro de rota `:id` de forma reativa?

- `this.route.params.id`
+ `this.route.paramMap.pipe(map(p => p.get('id')))`
- `this.route.snapshot.params['id']` (reativo)
- `this.router.params.get('id')`

> `route.paramMap` é um Observable que emite sempre que os parâmetros mudam. `snapshot.paramMap` é síncrono mas não reage a mudanças. Para componentes reutilizados na mesma rota, use o Observable.

? O que `loadChildren` retorna para configurar lazy loading?

- Um componente
+ Uma Promise de um objeto com as rotas (`Routes`)
- Um módulo diretamente
- Uma string com o caminho

> `loadChildren: () => import('./feature/feature.routes').then(m => m.featureRoutes)` carrega o arquivo de rotas de forma lazy, sem incluir o código no bundle principal.

? Como passar dados temporários entre rotas sem expô-los na URL?

- `queryParams`
- `fragment`
+ `state` em `NavigationExtras`
- `localStorage`

> `router.navigate(['/rota'], { state: { dados: 'secretos' } })` passa dados de navegação acessíveis via `router.getCurrentNavigation()?.extras.state` — não aparecem na URL.

? O que `routerLinkActive` faz?

- Desativa o link quando a rota está ativa
+ Adiciona uma classe CSS ao elemento quando a rota do link está ativa
- Redireciona automaticamente quando clicado
- Faz prefetch da rota

> `routerLinkActive="classe-css"` adiciona a classe quando a URL corresponde ao `routerLink`, permitindo estilizar o link da rota atual (ex: item de menu destacado).

? Qual estratégia de preloading carrega os módulos lazy em background após o app iniciar?

- `NoPreloading`
- `LazyPreloading`
+ `PreloadAllModules`
- `SmartPreloading`

> `PreloadAllModules` carrega todos os módulos lazy em background após o carregamento inicial, eliminando o delay na primeira navegação a cada rota.

:::

---

::: cartas

`RouterLink` :: Diretiva para links declarativos no template `[routerLink]="['/rota']"`
`RouterLinkActive` :: Adiciona CSS class quando a rota do link está ativa
`ActivatedRoute` :: Serviço com parâmetros, query params e dados da rota atual
`Router.navigate()` :: Navegação programática com `NavigationExtras`
`loadComponent` :: Lazy loading de um componente standalone na rota
`loadChildren` :: Lazy loading de um conjunto de rotas (feature)
`paramMap` :: Observable de parâmetros de rota — reativo a mudanças
`NavigationExtras` :: Objeto com `queryParams`, `fragment`, `state` para navegação rica

:::
