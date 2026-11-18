import { Component, computed, inject, signal } from '@angular/core';
import { form, FormField, max, maxLength, min, required, submit } from '@angular/forms/signals';
import { RouterLink } from '@angular/router';
import { convite as textoDeConvite } from '../capitulos/cap12/retornos';
import { DiarioStore } from './diario.store';
import { Pratica, PRATICAS } from './entrada';

interface Rascunho {
  pratica: Pratica;
  minutos: number;
  palavra: string;
  nota: string;
}

const NOVO: Rascunho = { pratica: 'respiração', minutos: 5, palavra: '', nota: '' };

/** Lab 2 — a página "Hoje": registrar uma prática e ver as de hoje. */
@Component({
  selector: 'app-hoje',
  imports: [FormField, RouterLink],
  template: `
    <h1>Hoje</h1>
    <p class="convite">{{ convite() }}</p>
    @if (store.avisoDeCarga(); as aviso) {
      <p role="alert">{{ aviso }}</p>
    }

    <form (submit)="salvar($event)" novalidate aria-labelledby="titulo-form">
      <h2 id="titulo-form">Registrar uma prática</h2>

      <label for="pratica">Prática</label>
      <select id="pratica" [formField]="rascunho.pratica">
        @for (p of praticas; track p) {
          <option [value]="p">{{ p }}</option>
        }
      </select>

      <label for="minutos">Minutos</label>
      <input id="minutos" type="number" [formField]="rascunho.minutos" />
      @if (rascunho.minutos().touched()) {
        @for (e of rascunho.minutos().errors(); track e.kind) {
          <p class="erro">{{ e.message }}</p>
        }
      }

      <label for="palavra">Uma palavra para o momento</label>
      <input id="palavra" [formField]="rascunho.palavra" />
      @if (rascunho.palavra().touched()) {
        @for (e of rascunho.palavra().errors(); track e.kind) {
          <p class="erro">{{ e.message }}</p>
        }
      }

      <label for="nota">Nota (opcional)</label>
      <textarea id="nota" [formField]="rascunho.nota"></textarea>

      <button type="submit" class="primario">Guardar</button>
    </form>

    <h2>Registros de hoje</h2>
    <ul>
      @for (e of store.deHoje(); track e.id) {
        <li><a [routerLink]="['/diario/registro', e.id]">{{ e.pratica }} · {{ e.minutos }} min · {{ e.palavra }}</a></li>
      } @empty {
        <li>Nada ainda. Tudo bem.</li>
      }
    </ul>
  `,
  styles: `
    form { display: grid; gap: .4rem; max-width: 28rem; }
    label { font-weight: 600; margin-top: .4rem; }
    .convite { color: var(--tinta-2); }
  `,
})
export class Hoje {
  protected readonly store = inject(DiarioStore);
  protected readonly praticas = PRATICAS;
  protected readonly modelo = signal<Rascunho>({ ...NOVO });

  protected readonly rascunho = form(this.modelo, (c) => {
    required(c.palavra, { message: 'Escreva uma palavra, qualquer uma serve.' });
    maxLength(c.palavra, 30, { message: 'Uma palavra curta basta (até 30 letras).' });
    min(c.minutos, 1, { message: 'Mesmo um minuto conta. Use 1 ou mais.' });
    max(c.minutos, 120, { message: 'Até 120 minutos por registro.' });
    maxLength(c.nota, 280, { message: 'Até 280 caracteres.' });
  });

  protected readonly convite = computed(() => textoDeConvite(this.store.resumo(), this.store.hoje()));

  protected async salvar(evento: Event): Promise<void> {
    evento.preventDefault();
    await submit(this.rascunho, async () => {
      this.store.adicionar({ ...this.modelo(), data: this.store.hoje() });
      this.rascunho().reset({ ...NOVO });
      return undefined;
    });
  }
}
