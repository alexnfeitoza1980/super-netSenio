import { Injectable } from '@angular/core';
import { asyncScheduler, debounceTime, delay, distinctUntilChanged, map, Observable, of, SchedulerLike, switchMap } from 'rxjs';

// #region fonte
/** Cap. 8 — o "contrato" de quem sugere práticas. Classe abstrata = token de injeção + tipo. */
@Injectable({ providedIn: 'root', useFactory: () => new SugestoesLocais() })
export abstract class FonteDeSugestoes {
  abstract buscar(termo: string): Observable<readonly string[]>;
}

const PRATICAS = [
  'respiração de um minuto', 'respiração em três passos', 'exploração do corpo', 'caminhada atenta',
  'sons e pensamentos', 'alongamento atento', 'comer com atenção', 'pausa antes da reunião',
];

/** Implementação local, com um pequeno atraso para parecer uma rede. */
export class SugestoesLocais extends FonteDeSugestoes {
  buscar(termo: string): Observable<readonly string[]> {
    const t = termo.toLowerCase();
    return of(PRATICAS.filter((p) => p.includes(t))).pipe(delay(150));
  }
}
// #endregion

// #region busca
/**
 * A "fila frustrante" resolvida: espera a pessoa parar de digitar (debounceTime), ignora repetições
 * (distinctUntilChanged) e, se chegar um termo novo, CANCELA a busca anterior (switchMap).
 */
export function buscaCalma(
  termos$: Observable<string>,
  buscar: (termo: string) => Observable<readonly string[]>,
  calmaMs: number,
  agendador: SchedulerLike = asyncScheduler,
): Observable<readonly string[]> {
  return termos$.pipe(
    map((t) => t.trim()),
    debounceTime(calmaMs, agendador),
    distinctUntilChanged(),
    switchMap((t) => (t.length < 2 ? of([]) : buscar(t))),
  );
}
// #endregion
