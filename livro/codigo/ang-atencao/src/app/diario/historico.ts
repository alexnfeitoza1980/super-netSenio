import { Component, computed, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { MinutosPipe } from '../capitulos/cap11/minutos.pipe';
import { DiarioStore } from './diario.store';
import { Entrada, PRATICAS } from './entrada';

/** Lab 2 — o histórico: filtro por prática, agrupado por dia, com totais. */
@Component({
  selector: 'app-historico',
  imports: [RouterLink, MinutosPipe],
  template: `
    <h1>Histórico</h1>
    <p>
      {{ store.resumo().diasPraticados }} dia(s) com prática · {{ store.minutosTotais() | minutos }} no total ·
      {{ store.resumo().retornos }} retorno(s) depois de uma pausa
    </p>

    <label for="filtro">Mostrar</label>
    <select id="filtro" (change)="filtro.set($any($event.target).value)">
      <option value="">todas as práticas</option>
      @for (p of praticas; track p) {
        <option [value]="p">{{ p }}</option>
      }
    </select>

    @for (dia of porDia(); track dia.data) {
      <section>
        <h2>{{ dia.data }} <small>({{ dia.minutos | minutos }})</small></h2>
        <ul>
          @for (e of dia.entradas; track e.id) {
            <li><a [routerLink]="['/diario/registro', e.id]">{{ e.pratica }} · {{ e.minutos }} min · {{ e.palavra }}</a></li>
          }
        </ul>
      </section>
    } @empty {
      <p>Nenhum registro {{ filtro() ? 'de ' + filtro() : '' }} ainda.</p>
    }
  `,
})
export class Historico {
  protected readonly store = inject(DiarioStore);
  protected readonly praticas = PRATICAS;
  protected readonly filtro = signal<string>('');

  protected readonly porDia = computed(() => {
    const grupos = new Map<string, Entrada[]>();
    for (const e of this.store.entradas()) {
      if (this.filtro() && e.pratica !== this.filtro()) continue;
      grupos.set(e.data, [...(grupos.get(e.data) ?? []), e]);
    }
    return [...grupos].map(([data, entradas]) => ({ data, entradas, minutos: entradas.reduce((s, e) => s + e.minutos, 0) }));
  });
}
