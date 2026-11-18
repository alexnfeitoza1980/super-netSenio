import { Component, computed, input } from '@angular/core';
import { Resumo } from './atividades';

/** Cap. 11 — um pedaço "pesado" de propósito: só é carregado quando aparece na tela (@defer). */
@Component({
  selector: 'app-grafico-do-dia',
  template: `
    <svg viewBox="0 0 300 24" role="img" [attr.aria-label]="descricao()">
      <rect x="0" y="0" height="24" [attr.width]="larguraNutre()" fill="#2e7d32" />
      <rect y="0" height="24" [attr.x]="larguraNutre()" [attr.width]="300 - larguraNutre()" fill="#c62828" />
    </svg>
  `,
})
export class GraficoDoDia {
  readonly resumo = input.required<Resumo>();
  protected readonly larguraNutre = computed(() => Math.round(this.resumo().proporcaoQueNutre * 300));
  protected readonly descricao = computed(
    () => `${Math.round(this.resumo().proporcaoQueNutre * 100)}% do tempo classificado foi em atividades que nutrem`,
  );
}
