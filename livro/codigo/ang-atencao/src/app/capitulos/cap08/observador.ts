import { Component, inject, signal } from '@angular/core';
import { toObservable, toSignal } from '@angular/core/rxjs-interop';
import { FluxoDePensamentos } from './fluxo-de-pensamentos';
import { buscaCalma, FonteDeSugestoes } from './sugestoes';
import { INTERVALO_DE_CALMA } from './tokens';

/** Cap. 8 — o componente só pede o que precisa (inject) e converte Observables em signals. */
@Component({
  selector: 'app-observador',
  template: `
    <h2>Pensamentos como eventos</h2>
    <form (submit)="$event.preventDefault(); fluxo.notar(campo.value); campo.value = ''">
      <label for="pensamento">O que passou pela cabeça agora?</label>
      <input id="pensamento" #campo autocomplete="off" />
      <button type="submit">Notar</button>
    </form>
    <ul aria-live="polite">
      @for (e of recentes(); track e.em.getTime() + e.texto) {
        <li><strong>{{ e.rotulo }}</strong>: {{ e.texto }}</li>
      }
    </ul>

    <label for="busca">Procurar uma prática</label>
    <input id="busca" (input)="termo.set($any($event.target).value)" />
    <ul>
      @for (s of sugestoes(); track s) {
        <li>{{ s }}</li>
      }
    </ul>
  `,
})
export class Observador {
  protected readonly fluxo = inject(FluxoDePensamentos);
  private readonly fonte = inject(FonteDeSugestoes);
  private readonly calma = inject(INTERVALO_DE_CALMA);

  protected readonly termo = signal('');
  protected readonly recentes = toSignal(this.fluxo.recentes$, { initialValue: [] });
  protected readonly sugestoes = toSignal(
    buscaCalma(toObservable(this.termo), (t) => this.fonte.buscar(t), this.calma),
    { initialValue: [] },
  );
}
