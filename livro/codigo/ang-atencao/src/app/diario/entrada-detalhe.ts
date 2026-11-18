import { Component, computed, inject, input } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { DiarioStore } from './diario.store';

/** Lab 2 — um registro. O `:id` da rota chega como input. */
@Component({
  selector: 'app-entrada-detalhe',
  imports: [RouterLink],
  template: `
    <nav><a routerLink="/diario/historico">← Histórico</a></nav>
    @if (entrada(); as e) {
      <h1>{{ e.pratica }}</h1>
      <dl>
        <dt>Dia</dt><dd>{{ e.data }}</dd>
        <dt>Minutos</dt><dd>{{ e.minutos }}</dd>
        <dt>Palavra</dt><dd>{{ e.palavra }}</dd>
        @if (e.nota) {
          <dt>Nota</dt><dd>{{ e.nota }}</dd>
        }
      </dl>
      <button type="button" (click)="remover(e.id)">Apagar este registro</button>
    } @else {
      <h1>Registro não encontrado</h1>
      <p>Talvez ele tenha sido apagado. <a routerLink="/diario/hoje">Voltar para hoje</a>.</p>
    }
  `,
})
export class EntradaDetalhe {
  readonly id = input.required<string>();
  private readonly store = inject(DiarioStore);
  private readonly router = inject(Router);
  protected readonly entrada = computed(() => this.store.porId(this.id()));

  protected remover(id: string): void {
    if (this.store.remover(id)) void this.router.navigateByUrl('/diario/historico');
  }
}
