import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject, InjectionToken } from '@angular/core';
import { catchError, finalize, retry, throwError, timer } from 'rxjs';

export interface ConfigDaAmpulheta {
  /** Quantas NOVAS tentativas para falhas passageiras (503, rede caída). */
  readonly tentativas: number;
  /** Espera antes da primeira nova tentativa; dobra a cada uma. */
  readonly esperaMs: number;
  readonly registrar: (linha: string) => void;
}

export const CONFIG_DA_AMPULHETA = new InjectionToken<ConfigDaAmpulheta>('CONFIG_DA_AMPULHETA', {
  providedIn: 'root',
  factory: () => ({ tentativas: 2, esperaMs: 500, registrar: (l) => console.info(l) }),
});

/** Um erro com uma mensagem que dá para mostrar a uma pessoa. */
export class ProblemaDeRede extends Error {
  constructor(
    readonly status: number,
    mensagem: string,
  ) {
    super(mensagem);
    this.name = 'ProblemaDeRede';
  }
}

const passageira = (e: unknown) => e instanceof HttpErrorResponse && (e.status === 0 || e.status === 503);

export function mensagemPara(status: number): string {
  if (status === 0) return 'Sem conexão agora. Seus dados continuam aqui; tente de novo quando a rede voltar.';
  if (status === 404) return 'Não encontrei o que você pediu.';
  if (status >= 500) return 'O servidor está com dificuldade. Não é com você; tente daqui a pouco.';
  return 'Algo não saiu como esperado.';
}

/**
 * Cap. 9 — um interceptor em três passos, como uma ampulheta:
 * 1. PERCEBER: registra o que está saindo e quanto demora;
 * 2. REDIRECIONAR: só repete o que faz sentido repetir (GET com falha passageira), esperando mais a cada vez;
 * 3. AMPLIAR: transforma o erro técnico numa mensagem que a tela consegue mostrar com calma.
 */
export const ampulhetaInterceptor: HttpInterceptorFn = (req, next) => {
  const { tentativas, esperaMs, registrar } = inject(CONFIG_DA_AMPULHETA);
  const inicio = performance.now();
  registrar(`→ ${req.method} ${req.urlWithParams}`);

  const podeRepetir = req.method === 'GET';
  return next(req).pipe(
    retry({
      count: podeRepetir ? tentativas : 0,
      delay: (erro, n) => (passageira(erro) ? timer(esperaMs * 2 ** (n - 1)) : throwError(() => erro)),
    }),
    catchError((erro: unknown) => {
      const status = erro instanceof HttpErrorResponse ? erro.status : -1;
      return throwError(() => new ProblemaDeRede(status, mensagemPara(status)));
    }),
    finalize(() => registrar(`← ${req.method} ${req.urlWithParams} (${Math.round(performance.now() - inicio)} ms)`)),
  );
};
