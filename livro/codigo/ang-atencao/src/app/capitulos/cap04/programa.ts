import { Component, computed, signal } from '@angular/core';
import { CartaoDaSemana } from './cartao-da-semana';
import { SEMANAS } from './semanas';

/** Cap. 4 — o componente "inteligente": guarda o estado e compõe os cartões. */
@Component({
  selector: 'app-programa',
  imports: [CartaoDaSemana],
  template: `
    <h2>O programa de oito semanas</h2>
    <p aria-live="polite">{{ feitas().size }} de {{ semanas.length }} semanas praticadas</p>

    @for (s of semanas; track s.numero) {
      <app-cartao-da-semana
        [semana]="s"
        [feita]="feitas().has(s.numero)"
        (feitaChange)="marcar(s.numero, $event)"
        (concluiu)="ultima.set($event)">
        <small>No Angular: {{ s.angular }} (cap. {{ s.capitulo }})</small>
      </app-cartao-da-semana>
    }

    @if (ultima(); as n) {
      <p>Última semana marcada: {{ n }}. Uma de cada vez.</p>
    }
  `,
})
export class Programa {
  protected readonly semanas = SEMANAS;
  protected readonly feitas = signal<ReadonlySet<number>>(new Set());
  protected readonly ultima = signal<number | null>(null);
  readonly completo = computed(() => this.feitas().size === this.semanas.length);

  protected marcar(numero: number, feita: boolean): void {
    this.feitas.update((atual) => {
      const nova = new Set(atual); // imutável: um conjunto novo a cada mudança
      if (feita) nova.add(numero);
      else nova.delete(numero);
      return nova;
    });
  }
}
