import { Component, input, model, output } from '@angular/core';
import { Semana } from './semanas';

/**
 * Cap. 4 — um componente "de apresentação": recebe dados (input), avisa o que aconteceu (output)
 * e tem um valor de duas vias (model). Não sabe de onde vêm as semanas nem para onde vai o aviso.
 */
@Component({
  selector: 'app-cartao-da-semana',
  template: `
    <article [class.feita]="feita()">
      <h3>Semana {{ semana().numero }} · {{ semana().tema }}</h3>
      <p>{{ semana().pratica }}</p>
      <ng-content />
      <label>
        <input type="checkbox" [checked]="feita()" (change)="alternar()" />
        Pratiquei esta semana
      </label>
    </article>
  `,
  styles: `
    article { padding: 12px 16px; border: 1px solid var(--linha, #ddd); border-radius: 12px; }
    article.feita { border-color: var(--bom, green); }
  `,
})
export class CartaoDaSemana {
  readonly semana = input.required<Semana>();
  readonly feita = model(false);
  readonly concluiu = output<number>();

  protected alternar(): void {
    const agora = !this.feita();
    this.feita.set(agora);
    if (agora) this.concluiu.emit(this.semana().numero);
  }
}
