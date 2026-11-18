---
id: ang-antimedo-cap12
order: 312
kind: capitulo
num: 217
group: Angular · Parte 54 · Bloco III — A Transformação
title: Ame problemas
subtitle: Error handling com RxJS — operadores de retry e recovery
minutes: 30
hue: 300
badge: 💪
- Entender por que amar problemas acelera a resolução: 
- Tratar erros em Observables com catchError e retry: 
- Implementar um GlobalErrorHandler para capturar falhas inesperadas: 
- Criar estratégias de recuperação resilientes em vez de deixar o app travar: 
origem: angular/livros/01-antimedo-angular/content/12-ame-problemas.md
---

# Ame problemas

> *"Tudo que você deseja está do outro lado da ponte chamada 'problema'."*
> — Pablo Marçal

Existe uma verdade contraintuitiva que os vencedores descobriram: **problemas são o caminho, não o obstáculo**. Toda riqueza, toda realização, toda conquista está escondida atrás de um problema que a maioria das pessoas se recusou a resolver.

O desenvolvedor mediano olha para um erro no console e sente pânico. O desenvolvedor extraordinário olha para o mesmo erro e pensa: *"Ótimo. Aqui está uma oportunidade de aprender algo que vai me distinguir."*

Amar o problema não significa gostar do sofrimento. Significa **não fugir**. Significa sentar na frente do erro, lê-lo com atenção, entender o que ele está tentando te dizer. O erro é um professor — e os melhores professores são exigentes.

::: nota
Pablo Marçal afirma: "Se você decidir amar o problema, será mais fácil resolvê-lo. Não procrastine — o meteoro que você pode desviar agora vai te atingir se você esperar." Em código: o bug que você ignora hoje vira incidente em produção amanhã.
:::

---

## Erros em Observables — o problema que chegou até você

No mundo RxJS, erros são cidadãos de primeira classe. Um Observable pode emitir três tipos de notificações:

- **`next`** — dados chegando (as boas notícias)
- **`error`** — algo deu errado (o problema)
- **`complete`** — terminou sem erros

A maioria dos desenvolvedores trata apenas o `next`. Os resilientes também tratam o `error` — e isso faz toda a diferença.

```typescript
// A maioria faz isso — ignora o problema:
this.http.get('/api/medos').subscribe(dados => {
  this.medos = dados; // E se der erro? App trava.
});

// O desenvolvedor que ama problemas faz isso:
this.http.get('/api/medos').pipe(
  catchError(erro => {
    console.error('Problema encontrado:', erro);
    return of([]); // Retorna lista vazia em vez de travar
  })
).subscribe(dados => {
  this.medos = dados; // Sempre funciona
});
```

---

## `catchError` — o primeiro abraço no problema

O `catchError` intercepta erros e permite que você decida o que fazer. É como aquela capacidade de, ao receber uma má notícia, respirar fundo antes de reagir.

```typescript
// src/app/core/services/medo.service.ts
import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError, of, EMPTY } from 'rxjs';
import { catchError, map } from 'rxjs/operators';

interface Medo {
  id: string;
  descricao: string;
  nivel: number;
  enfrentado: boolean;
}

@Injectable({ providedIn: 'root' })
export class MedoService {
  private readonly API = 'https://api.antimedo.app';

  constructor(private http: HttpClient) {}

  listarMedos(): Observable<Medo[]> {
    return this.http.get<Medo[]>(`${this.API}/medos`).pipe(
      map(medos => medos.sort((a, b) => b.nivel - a.nivel)),
      catchError((erro: HttpErrorResponse) => {
        if (erro.status === 404) {
          // Problema conhecido: lista vazia. Não é o fim do mundo.
          return of([]);
        }
        if (erro.status === 403) {
          // Problema de permissão: propaga para o guard tratar
          return throwError(() => new Error('Sem permissão'));
        }
        // Problema desconhecido: propaga para o GlobalErrorHandler
        return throwError(() => erro);
      })
    );
  }

  salvarMedo(medo: Omit<Medo, 'id'>): Observable<Medo> {
    return this.http.post<Medo>(`${this.API}/medos`, medo).pipe(
      catchError(this.tratarErroHttp.bind(this))
    );
  }

  private tratarErroHttp(erro: HttpErrorResponse): Observable<never> {
    let mensagem = 'Erro desconhecido';
    if (erro.error instanceof ErrorEvent) {
      // Erro do cliente (rede, etc.)
      mensagem = `Erro de rede: ${erro.error.message}`;
    } else {
      // Erro do servidor
      mensagem = `Servidor retornou ${erro.status}: ${erro.message}`;
    }
    console.error(mensagem);
    return throwError(() => new Error(mensagem));
  }
}
```

---

## `retry` e `retryWhen` — tentar de novo antes de desistir

Pablo diz que o problema vai te atingir se você esperar. Mas às vezes, tentar novamente resolve. O RxJS tem operadores exatamente para isso.

```typescript
import { retry, catchError, timer } from 'rxjs';
import { mergeMap } from 'rxjs/operators';

// Tenta 3 vezes imediatamente
this.http.get('/api/dados').pipe(
  retry(3),
  catchError(erro => {
    console.error('Falhou após 3 tentativas:', erro);
    return of(null);
  })
);
```

### Retry com backoff exponencial — inteligência na resiliência

```typescript
// src/app/core/utils/retry-estrategia.ts
import { Observable, throwError, timer } from 'rxjs';
import { mergeMap, finalize } from 'rxjs/operators';
import { retryWhen } from 'rxjs';

interface RetryConfig {
  maxTentativas: number;
  delayMs: number;
  fatorBackoff: number;
}

export function retryComBackoff(config: RetryConfig) {
  const { maxTentativas, delayMs, fatorBackoff } = config;

  return retryWhen((erros: Observable<any>) =>
    erros.pipe(
      mergeMap((erro, tentativa) => {
        if (tentativa >= maxTentativas) {
          return throwError(() => erro);
        }
        const espera = delayMs * Math.pow(fatorBackoff, tentativa);
        console.log(`Tentativa ${tentativa + 1} em ${espera}ms...`);
        return timer(espera);
      }),
      finalize(() => console.log('Estratégia de retry finalizada'))
    )
  );
}

// Uso:
this.http.get('/api/dados').pipe(
  retryComBackoff({ maxTentativas: 3, delayMs: 1000, fatorBackoff: 2 }),
  catchError(erro => of([]))
);
```

```saida
Tentativa 1 em 1000ms...
Tentativa 2 em 2000ms...
Tentativa 3 em 4000ms...
Estratégia de retry finalizada
```

---

## `EMPTY` — às vezes o silêncio é a melhor resposta

```typescript
import { EMPTY } from 'rxjs';

// Problema aconteceu, mas não queremos propagar nem retornar dados
// Simplesmente completamos sem emitir nada
this.http.get('/api/opcional').pipe(
  catchError(() => EMPTY) // Completa silenciosamente
).subscribe({
  next: dados => this.processarDados(dados),
  complete: () => console.log('Finalizado (com ou sem dados)')
});
```

---

## GlobalErrorHandler — o problema que ninguém previu

Mesmo com `catchError` em todo lugar, erros inesperados acontecem. O `GlobalErrorHandler` é sua última linha de defesa — como a consciência plena que captura qualquer pensamento que escape.

```typescript
// src/app/core/handlers/global-error.handler.ts
import { ErrorHandler, Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  private router = inject(Router);

  handleError(erro: unknown): void {
    console.error('[GlobalErrorHandler] Erro capturado:', erro);

    if (erro instanceof Error) {
      this.registrarErro(erro);
      this.notificarUsuario(erro.message);
    }

    if (this.ehErroChunk(erro)) {
      // Chunk não encontrado = deploy novo. Recarregar resolve.
      window.location.reload();
    }
  }

  private ehErroChunk(erro: unknown): boolean {
    const mensagem = (erro as Error)?.message ?? '';
    return mensagem.includes('Loading chunk') ||
           mensagem.includes('Failed to fetch dynamically');
  }

  private registrarErro(erro: Error): void {
    // Integração com Sentry, Datadog, etc.
    console.error(`[LOG] ${erro.name}: ${erro.message}`, erro.stack);
  }

  private notificarUsuario(mensagem: string): void {
    // Mostrar toast, dialog, etc.
    console.warn('[UI] Notificando usuário:', mensagem);
  }
}
```

```typescript
// src/app/app.config.ts — registrando o handler global
import { ApplicationConfig, ErrorHandler } from '@angular/core';
import { GlobalErrorHandler } from './core/handlers/global-error.handler';

export const appConfig: ApplicationConfig = {
  providers: [
    { provide: ErrorHandler, useClass: GlobalErrorHandler },
    // ... outros providers
  ]
};
```

---

::: dica
**A regra de ouro do error handling:**
1. Se você **sabe** o que fazer com o erro → `catchError` + retorne um valor seguro
2. Se o erro é **temporário** → `retry` com backoff
3. Se o erro é **silenciável** → `catchError(() => EMPTY)`
4. Se você **não sabe** o que fazer → `throwError` para o `GlobalErrorHandler` decidir

Nunca deixe um Observable sem tratamento de erro em produção.
:::

---

::: resumo
Amar problemas em Angular significa **nunca deixar um erro sem tratamento**. O RxJS oferece um arsenal completo:

- **`catchError`** — intercepta e trata, retornando um Observable alternativo
- **`retry(n)`** — tenta novamente `n` vezes antes de propagar
- **`retryWhen`** — retry com lógica customizada (backoff exponencial)
- **`throwError`** — propaga o erro para o próximo handler
- **`EMPTY`** — completa silenciosamente sem emitir dados
- **`GlobalErrorHandler`** — captura qualquer erro não tratado na aplicação

Problema é o caminho. `catchError` é o abraço que você dá nele.
:::

---

::: exercicio Serviço resiliente com retry e fallback
Implemente um `NoticiasService` que:
1. Busca notícias de `/api/noticias`
2. Tenta 3 vezes com delay crescente (1s, 2s, 4s) antes de desistir
3. Em caso de falha total, retorna as últimas notícias do `localStorage`
4. Loga cada tentativa no console

--- resposta

```typescript
// src/app/core/services/noticias.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, timer, throwError } from 'rxjs';
import { retryWhen, mergeMap, catchError, tap } from 'rxjs/operators';

interface Noticia {
  id: string;
  titulo: string;
  conteudo: string;
}

const CACHE_KEY = 'noticias_cache';

@Injectable({ providedIn: 'root' })
export class NoticiasService {
  constructor(private http: HttpClient) {}

  buscarNoticias(): Observable<Noticia[]> {
    return this.http.get<Noticia[]>('/api/noticias').pipe(
      tap(noticias => {
        // Salva no cache quando bem-sucedido
        localStorage.setItem(CACHE_KEY, JSON.stringify(noticias));
      }),
      retryWhen(erros => erros.pipe(
        mergeMap((erro, tentativa) => {
          if (tentativa >= 3) return throwError(() => erro);
          const delay = 1000 * Math.pow(2, tentativa);
          console.log(`[Notícias] Tentativa ${tentativa + 1} em ${delay}ms`);
          return timer(delay);
        })
      )),
      catchError(erro => {
        console.error('[Notícias] Falha total. Usando cache local.', erro);
        const cache = localStorage.getItem(CACHE_KEY);
        return cache ? of(JSON.parse(cache)) : of([]);
      })
    );
  }
}
```

:::

---

::: quiz

? O que `catchError` deve retornar obrigatoriamente?

- Um array vazio
- Uma Promise
+ Um Observable
- Um valor síncrono

> `catchError` deve retornar um Observable. Pode ser `of([])` para dados, `EMPTY` para silêncio, ou `throwError()` para propagar o erro.

? Qual operador implementa retry com lógica customizada?

- `retry()`
+ `retryWhen()`
- `catchError()`
- `delay()`

> `retryWhen` recebe uma função que recebe o Observable de erros e decide quando (e se) tentar novamente, permitindo backoff exponencial e limites de tentativas.

? O que `EMPTY` faz quando usado em um `catchError`?

- Emite `null`
- Propaga o erro
+ Completa o Observable sem emitir nenhum valor
- Reinicia o Observable

> `EMPTY` é um Observable que completa imediatamente sem emitir valores. Útil para "engolir" erros silenciosamente.

? Como se registra um `GlobalErrorHandler` customizado?

- `@Injectable()` com `implements ErrorHandler`
+ `{ provide: ErrorHandler, useClass: GlobalErrorHandler }` nos providers
- Decorando a classe com `@GlobalHandler()`
- Chamando `app.registerErrorHandler()` no `main.ts`

> O Angular usa injeção de dependência para resolver o `ErrorHandler`. Substituindo o provider com `useClass`, sua implementação captura todos os erros não tratados.

? Pablo Marçal diz que o meteoro que você pode desviar agora... O que isso representa em código?

- Refatoração desnecessária
- Adicionar mais features
+ Bug ou erro não tratado que se tornará incidente em produção
- Excesso de comentários no código

> Um bug ignorado em desenvolvimento cresce até virar incidente em produção — exatamente o meteoro que você não desviou. Tratar erros com `catchError` é desviar o meteoro.

? Qual é a assinatura correta de um `GlobalErrorHandler`?

- `handle(error: Error): void`
+ `handleError(error: unknown): void`
- `onError(error: any): Promise<void>`
- `catch(error: Error): Observable<void>`

> `ErrorHandler` do Angular define o método `handleError(error: unknown): void`. O tipo `unknown` força você a verificar o tipo antes de usar, tornando o código mais seguro.

:::

---

::: cartas

`catchError` :: Intercepta erros em um Observable e retorna um Observable alternativo
`retry(n)` :: Tenta resubscrever o Observable até `n` vezes após um erro
`retryWhen` :: Retry com lógica customizada — backoff, limites e condições
`throwError` :: Cria um Observable que emite imediatamente um erro
`EMPTY` :: Observable que completa sem emitir — "silencia" o erro
`GlobalErrorHandler` :: Última linha de defesa — captura erros não tratados
Backoff exponencial :: Estratégia de retry onde o delay dobra a cada tentativa
`HttpErrorResponse` :: Tipo do Angular para erros HTTP — tem `status`, `error`, `message`

:::
