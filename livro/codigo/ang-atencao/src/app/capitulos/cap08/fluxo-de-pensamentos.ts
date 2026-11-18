import { inject, Injectable } from '@angular/core';
import { map, Observable, scan, shareReplay, Subject } from 'rxjs';
import { classificar, Rotulo } from './rotulos';
import { RELOGIO } from './tokens';

export interface EventoMental {
  readonly texto: string;
  readonly rotulo: Rotulo;
  readonly em: Date;
}

/**
 * Cap. 8 — um serviço (uma instância para o app todo) que recebe pensamentos e os devolve
 * como um FLUXO de eventos: chegam, são notados, passam.
 */
@Injectable({ providedIn: 'root' })
export class FluxoDePensamentos {
  private readonly agora = inject(RELOGIO);
  private readonly entrada = new Subject<string>();

  readonly eventos$: Observable<EventoMental> = this.entrada.pipe(
    map((texto) => texto.trim()),
    map((texto) => ({ texto, rotulo: classificar(texto), em: this.agora() })),
  );

  /** Os últimos cinco, do mais novo para o mais antigo. Quem chegar depois recebe o estado atual. */
  readonly recentes$: Observable<readonly EventoMental[]> = this.eventos$.pipe(
    scan((lista: readonly EventoMental[], e) => [e, ...lista].slice(0, 5), []),
    shareReplay({ bufferSize: 1, refCount: false }),
  );

  notar(texto: string): void {
    if (texto.trim()) this.entrada.next(texto);
  }
}
