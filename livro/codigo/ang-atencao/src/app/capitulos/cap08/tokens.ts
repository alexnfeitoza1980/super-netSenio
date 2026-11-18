import { InjectionToken } from '@angular/core';

/** Cap. 8 — dependências que não são classes também podem ser injetadas, por um InjectionToken. */

/** Quanto esperar (ms) depois da última tecla antes de buscar. */
export const INTERVALO_DE_CALMA = new InjectionToken<number>('INTERVALO_DE_CALMA', {
  providedIn: 'root',
  factory: () => 300,
});

/** "Que horas são?" injetável: nos testes, o tempo é nosso. */
export const RELOGIO = new InjectionToken<() => Date>('RELOGIO', {
  providedIn: 'root',
  factory: () => () => new Date(),
});
