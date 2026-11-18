import { httpResource } from '@angular/common/http';
import { Component, computed, signal } from '@angular/core';
import { ehListaDeFrases, Frase } from './frase';
import { ProblemaDeRede } from './ampulheta.interceptor';

/**
 * Cap. 9 — httpResource: a requisição é DERIVADA de signals (o tema escolhido).
 * Mudou o tema, nova requisição; a anterior é cancelada. Os estados (carregando, erro, valor)
 * também são signals, e o template trata cada um, inclusive o difícil.
 */
@Component({
  selector: 'app-frases',
  template: `
    <h2>Frases para pausar</h2>
    <label for="tema">Tema</label>
    <select id="tema" (change)="tema.set($any($event.target).value)">
      @for (t of temas; track t) {
        <option [value]="t" [selected]="t === tema()">{{ t }}</option>
      }
    </select>

    @switch (frases.status()) {
      @case ('loading') { <p aria-live="polite">Buscando frases…</p> }
      @case ('reloading') { <p aria-live="polite">Buscando frases…</p> }
      @case ('error') {
        <div role="alert">
          <p>{{ mensagemDeErro() }}</p>
          <button type="button" (click)="frases.reload()">Tentar de novo</button>
        </div>
      }
      @default {
        <ul>
          @for (f of lista(); track f.id) {
            <li>{{ f.texto }}</li>
          } @empty {
            <li>Nenhuma frase para este tema ainda.</li>
          }
        </ul>
      }
    }
  `,
})
export class Frases {
  protected readonly temas = ['pausa', 'gentileza', 'foco'];
  protected readonly tema = signal('pausa');

  protected readonly frases = httpResource<Frase[]>(() => `api/frases-${encodeURIComponent(this.tema())}.json`, {
    defaultValue: [],
    parse: (dado) => {
      if (!ehListaDeFrases(dado)) throw new ProblemaDeRede(200, 'O servidor respondeu num formato que eu não reconheço.');
      return dado;
    },
  });

  protected readonly lista = computed(() => (this.frases.hasValue() ? this.frases.value() : []));

  protected readonly mensagemDeErro = computed(() => {
    const e = this.frases.error();
    return e instanceof ProblemaDeRede ? e.message : 'Algo não saiu como esperado.';
  });
}
