import { computed, signal } from '@angular/core';

/**
 * Cap. 3 — modo Existente: declaramos O QUE É, não COMO manter em dia.
 * `restante` e `concluida` são consequências; o Angular recalcula quando (e só quando) precisa.
 */
export class SessaoExistente {
  readonly planejados = signal(10);
  readonly feitos = signal(0);

  readonly restante = computed(() => Math.max(0, this.planejados() - this.feitos()));
  readonly concluida = computed(() => this.feitos() >= this.planejados());

  praticar(minutos: number): void {
    this.feitos.update((f) => f + minutos);
  }

  replanejar(minutos: number): void {
    this.planejados.set(minutos);
  }
}
