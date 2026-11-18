import { linkedSignal, signal } from '@angular/core';

/**
 * Cap. 3 — linkedSignal: um valor que você pode mudar à mão, mas que se "reacomoda"
 * quando a fonte muda. Aqui: a prática escolhida continua a mesma se ainda existir na lista;
 * se sumiu, o foco volta, sem drama, para a primeira opção.
 */
export class Foco {
  readonly praticas = signal(['respiração', 'sons', 'corpo']);

  readonly escolhida = linkedSignal<string[], string | undefined>({
    source: this.praticas,
    computation: (lista, anterior) => {
      const atual = anterior?.value;
      return atual !== undefined && lista.includes(atual) ? atual : lista[0];
    },
  });
}
