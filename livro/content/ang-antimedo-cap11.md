---
id: ang-antimedo-cap11
order: 311
kind: capitulo
num: 216
group: Angular · Parte 54 · Bloco III — A Transformação
title: Crenças limitantes
subtitle: Guards e Interceptors — regras que controlam o acesso
minutes: 35
hue: 290
badge: 🔓
- Entender como crenças limitantes bloqueiam o fluxo de informação: 
- Criar Route Guards que protegem rotas como crenças protegem comportamentos: 
- Implementar HTTP Interceptors para transformar requisições: 
- Ressignificar guards rígidos em lógica flexível e consciente: 
origem: angular/livros/01-antimedo-angular/content/11-crencas-limitantes.md
---

# Crenças limitantes

> *"Se quer saber no que alguém acredita, observe o que faz — não o que diz."*
> — Pablo Marçal

Toda pessoa carrega um sistema interno de regras. Algumas dizem "eu consigo", "eu mereço", "eu sou capaz". Outras sussurram: "isso não é pra mim", "eu nunca fui bom nisso", "quem sou eu para tentar?".

Essas regras invisíveis são as **crenças**. E as crenças limitantes são as mais perigosas — porque bloqueiam o acesso antes mesmo de você tentar. Funcionam como porteiros implacáveis: chegou uma oportunidade nova? *"Não pode entrar."* Uma ideia ousada? *"Acesso negado."* Um sonho grande? *"Sem permissão."*

Em Angular, temos um mecanismo exatamente assim: os **Route Guards**. Eles decidem se uma rota pode ser acessada antes de qualquer coisa ser renderizada. E da mesma forma que uma crença limitante pode ser ressignificada, um guard pode ser reescrito para abrir novas possibilidades.

::: nota
As crenças são adquiridas na infância, reforçadas pela mídia e pelo ambiente. Mas o cérebro é plástico — toda crença pode ser ressignificada. O ciclo é: **Insatisfação → Nova perspectiva → Crer diferente → Nova crença → Novas ações**.
:::

---

## O que são Route Guards?

Route Guards são funções (ou classes) que o Angular executa **antes de ativar uma rota**. Eles retornam `true` para permitir navegação ou `false` (ou um `UrlTree`) para bloquear.

Existem cinco tipos de guards:

| Guard | Quando executa | Análogo humano |
|-------|---------------|----------------|
| `CanActivate` | Antes de entrar na rota | Crença sobre merecimento |
| `CanActivateChild` | Antes de entrar em rota filha | Crença sobre capacidade |
| `CanDeactivate` | Antes de sair da rota | Medo de mudança |
| `CanLoad` | Antes de carregar módulo lazy | Crença sobre acesso |
| `CanMatch` | Antes de combinar a rota | Crença sobre identidade |

::: dica
No Angular 15+, a abordagem recomendada são os **functional guards** — funções simples que retornam `boolean` ou `UrlTree`. São mais fáceis de testar e mais idiomáticos com o Angular moderno.
:::

---

## Crença limitante = Guard que sempre retorna `false`

Imagine um guard assim:

```typescript
// guards/merecimento.guard.ts
import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';

// Crença limitante: "Não sou bom o suficiente para isso"
export const merecimentoGuard: CanActivateFn = (route, state) => {
  // Sempre bloqueia. Nunca questiona. Nunca aprende.
  return false;
};
```

Esse guard é uma crença limitante pura: **decide antes de verificar**. Não analisa contexto. Não consulta dados. Simplesmente bloqueia.

Agora veja o mesmo guard ressignificado:

```typescript
// guards/merecimento-consciente.guard.ts
import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

// Crença ressignificada: "Verifico minha situação real antes de decidir"
export const merecimentoConscienteGuard: CanActivateFn = (route, state) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (auth.usuarioAutenticado()) {
    return true; // Acesso concedido baseado em fato real
  }

  // Redireciona com contexto — não simplesmente rejeita
  return router.createUrlTree(['/login'], {
    queryParams: { retorno: state.url }
  });
};
```

A diferença é profunda: o segundo guard **verifica a realidade** antes de decidir. Assim como a ressignificação de crenças: em vez de aceitar o "não posso" automático, você questiona: *"Baseado em que? O que é verdadeiramente verdadeiro aqui?"*

---

## Criando um Guard funcional passo a passo

::: passos Implementando Guards no Angular 17+

=== Passo 1 — Crie o serviço de autenticação

```typescript
// src/app/core/services/auth.service.ts
import { Injectable, signal } from '@angular/core';

interface Usuario {
  id: string;
  nome: string;
  perfil: 'aluno' | 'mentor' | 'admin';
}

@Injectable({ providedIn: 'root' })
export class AuthService {
  private _usuario = signal<Usuario | null>(null);

  readonly usuario = this._usuario.asReadonly();

  usuarioAutenticado(): boolean {
    return this._usuario() !== null;
  }

  temPerfil(perfil: string): boolean {
    return this._usuario()?.perfil === perfil;
  }

  login(usuario: Usuario): void {
    this._usuario.set(usuario);
    localStorage.setItem('usuario', JSON.stringify(usuario));
  }

  logout(): void {
    this._usuario.set(null);
    localStorage.removeItem('usuario');
  }

  restaurarSessao(): void {
    const dados = localStorage.getItem('usuario');
    if (dados) {
      this._usuario.set(JSON.parse(dados));
    }
  }
}
```

=== Passo 2 — Crie os guards

```typescript
// src/app/core/guards/auth.guard.ts
import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (auth.usuarioAutenticado()) {
    return true;
  }

  return router.createUrlTree(['/login'], {
    queryParams: { retorno: state.url }
  });
};

export const mentorGuard: CanActivateFn = (route, state) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (auth.temPerfil('mentor') || auth.temPerfil('admin')) {
    return true;
  }

  return router.createUrlTree(['/acesso-negado']);
};
```

=== Passo 3 — Registre as rotas com guards

```typescript
// src/app/app.routes.ts
import { Routes } from '@angular/router';
import { authGuard, mentorGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: '/inicio', pathMatch: 'full' },
  { path: 'login', loadComponent: () =>
      import('./features/auth/login/login.component')
        .then(m => m.LoginComponent)
  },
  {
    path: 'dashboard',
    canActivate: [authGuard],
    loadComponent: () =>
      import('./features/dashboard/dashboard.component')
        .then(m => m.DashboardComponent)
  },
  {
    path: 'mentoria',
    canActivate: [authGuard, mentorGuard],
    loadComponent: () =>
      import('./features/mentoria/mentoria.component')
        .then(m => m.MentoriaComponent)
  },
  { path: 'acesso-negado', loadComponent: () =>
      import('./features/acesso-negado/acesso-negado.component')
        .then(m => m.AcessoNegadoComponent)
  }
];
```

:::

---

## HTTP Interceptors — Crenças que transformam cada mensagem

Se guards controlam o **acesso às rotas**, os **Interceptors** controlam o fluxo das **requisições HTTP**. Eles interceptam toda chamada antes de sair e toda resposta antes de chegar — como crenças que filtram o que você percebe do mundo.

Uma crença limitante intercepta a realidade e a distorce: *"Recebi um elogio? Ele deve querer algo."* Um interceptor pode fazer o mesmo com as respostas HTTP — ou pode enriquecê-las conscientemente.

```typescript
// src/app/core/interceptors/auth.interceptor.ts
import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { Router } from '@angular/router';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const auth = inject(AuthService);
  const router = inject(Router);

  // Enriquece a requisição com token (crença positiva adicionada)
  const token = localStorage.getItem('token');
  const reqEnriquecida = token
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(reqEnriquecida).pipe(
    catchError((erro: HttpErrorResponse) => {
      if (erro.status === 401) {
        // Crença ressignificada: erro 401 não é fim do mundo
        auth.logout();
        router.navigate(['/login']);
      }
      return throwError(() => erro);
    })
  );
};
```

### Interceptor de logging — consciência plena das requisições

```typescript
// src/app/core/interceptors/logging.interceptor.ts
import { HttpInterceptorFn } from '@angular/common/http';
import { tap, finalize } from 'rxjs/operators';

export const loggingInterceptor: HttpInterceptorFn = (req, next) => {
  const inicio = Date.now();
  console.log(`→ [HTTP] ${req.method} ${req.url}`);

  return next(req).pipe(
    tap({
      next: (resposta) => console.log(`✓ [HTTP] ${req.url}`, resposta),
      error: (erro) => console.error(`✗ [HTTP] ${req.url}`, erro)
    }),
    finalize(() => {
      const duracao = Date.now() - inicio;
      console.log(`⏱ [HTTP] ${req.url} — ${duracao}ms`);
    })
  );
};
```

### Registrando interceptors no `app.config.ts`

```typescript
// src/app/app.config.ts
import { ApplicationConfig } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { routes } from './app.routes';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { loggingInterceptor } from './core/interceptors/logging.interceptor';

export const appConfig: ApplicationConfig = {
  providers: [
    provideRouter(routes),
    provideHttpClient(
      withInterceptors([loggingInterceptor, authInterceptor])
    )
  ]
};
```

```saida
→ [HTTP] GET https://api.antimedo.app/medos
✓ [HTTP] https://api.antimedo.app/medos { status: 200, ... }
⏱ [HTTP] https://api.antimedo.app/medos — 234ms
```

---

## O Ciclo de Ressignificação no Código

Pablo Marçal descreve o ciclo de mudança de crenças:

**Insatisfação → Nova perspectiva → Crer diferente → Nova crença → Novas ações**

Em Angular, isso se traduz assim:

```typescript
// ANTES: Guard com crença limitante
export const antigoGuard: CanActivateFn = () => false; // "Não posso"

// INSATISFAÇÃO: O app não funciona. Usuários não chegam a lugar nenhum.

// NOVA PERSPECTIVA: Entendo que guards devem verificar contexto real

// CRER DIFERENTE: Começo a usar AuthService

// NOVA CRENÇA → NOVA AÇÃO:
export const novoGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  return auth.usuarioAutenticado(); // Decisão baseada na realidade
};
```

::: resumo
**Guards** são as crenças do seu sistema de roteamento — eles decidem o que é possível. **Interceptors** são as crenças do seu sistema de comunicação — eles filtram e transformam cada mensagem trocada.

Crenças limitantes retornam `false` sem verificar. Crenças saudáveis consultam a realidade e respondem com inteligência.

- Use `CanActivateFn` para guards funcionais (Angular 15+)
- Use `HttpInterceptorFn` para interceptors funcionais (Angular 15+)
- Registre ambos em `app.config.ts` via `provideRouter` e `provideHttpClient`
:::

---

::: exercicio Crie um guard de "perfil completo"
Implemente um guard chamado `perfilCompletoGuard` que:
1. Verifica se o usuário está autenticado (via `AuthService`)
2. Verifica se o usuário preencheu `nome`, `email` e `telefone`
3. Se o perfil estiver incompleto, redireciona para `/completar-perfil`
4. Passa o parâmetro `?de=<url-atual>` na query string

--- resposta

```typescript
// src/app/core/guards/perfil-completo.guard.ts
import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { PerfilService } from '../services/perfil.service';

export const perfilCompletoGuard: CanActivateFn = (route, state) => {
  const auth = inject(AuthService);
  const perfil = inject(PerfilService);
  const router = inject(Router);

  // Primeiro: usuário precisa estar autenticado
  if (!auth.usuarioAutenticado()) {
    return router.createUrlTree(['/login'], {
      queryParams: { retorno: state.url }
    });
  }

  // Segundo: perfil precisa estar completo
  const dadosPerfil = perfil.obterPerfil();
  const completo = dadosPerfil?.nome
    && dadosPerfil?.email
    && dadosPerfil?.telefone;

  if (!completo) {
    return router.createUrlTree(['/completar-perfil'], {
      queryParams: { de: state.url }
    });
  }

  return true;
};
```

:::

---

::: quiz

? O que um `CanActivateFn` deve retornar para **bloquear** a navegação?

- `null`
- `undefined`
+ `false` ou um `UrlTree` de redirecionamento
- Uma Promise resolvida com `0`

> Retornar `false` bloqueia sem redirecionar. Retornar um `UrlTree` (via `router.createUrlTree()`) bloqueia E redireciona para outra rota.

? Qual função é usada para criar um `UrlTree` de redirecionamento?

- `router.navigate()`
+ `router.createUrlTree()`
- `router.parseUrl()`
- `router.serializeUrl()`

> `router.createUrlTree()` cria um objeto `UrlTree` que pode ser retornado diretamente de um guard, combinando bloqueio e redirecionamento em uma só operação.

? Como se registra um functional interceptor no Angular 17+?

- `HTTP_INTERCEPTORS` no `providers` do módulo
+ `withInterceptors([...])` dentro de `provideHttpClient()`
- `@Injectable()` com `implements HttpInterceptor`
- `useClass` no `HTTP_INTERCEPTORS` token

> A abordagem funcional usa `provideHttpClient(withInterceptors([interceptorFn]))` no `app.config.ts`, sem necessidade de classes ou tokens multi-provider.

? Qual a analogia correta entre crenças e guards?

- Guard é um serviço que salva dados
+ Crença limitante = guard que sempre retorna `false`; ressignificação = reescrever o guard para verificar a realidade
- Interceptor é equivalente a uma crença positiva apenas
- Guards só existem para autenticação

> Um guard que sempre retorna `false` nunca dá chance — exatamente como uma crença limitante. Ressignificar = reescrever a lógica para considerar a realidade antes de decidir.

? Qual guard verifica antes de **sair** de uma rota (ex: "Tem certeza que quer sair?")?

- `CanActivate`
- `CanActivateChild`
+ `CanDeactivate`
- `CanMatch`

> `CanDeactivate` é executado quando o usuário tenta navegar para fora da rota atual — útil para confirmações antes de descartar formulários não salvos.

? O interceptor `authInterceptor` no exemplo adiciona o token em qual header?

- `X-Auth-Token`
- `Token`
+ `Authorization`
- `Bearer`

> O header padrão para tokens JWT é `Authorization: Bearer <token>`, conforme o exemplo `setHeaders: { Authorization: \`Bearer ${token}\` }`.

:::

---

::: cartas

Crença limitante :: Guard que sempre retorna `false` — bloqueia sem verificar
Ressignificação :: Reescrever o guard para consultar a realidade antes de decidir
`CanActivateFn` :: Tipo de functional guard para controle de acesso a rotas
`HttpInterceptorFn` :: Tipo de functional interceptor para requisições HTTP
`router.createUrlTree()` :: Cria um `UrlTree` para redirecionar dentro de um guard
`withInterceptors([])` :: Registra interceptors funcionais no `provideHttpClient()`
`CanDeactivate` :: Guard que executa ao tentar **sair** de uma rota
Interceptor de autenticação :: Adiciona o Bearer token em todas as requisições HTTP

:::
