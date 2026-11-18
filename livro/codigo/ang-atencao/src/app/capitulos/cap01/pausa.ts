import { Component, signal } from '@angular/core';

/** Cap. 1 — o primeiro componente: um roteiro curto para parar antes de codar. */
@Component({
  selector: 'app-pausa',
  template: `
    <h2>{{ titulo }}</h2>
    <ol>
      @for (passo of passos; track passo) {
        <li [class.atual]="$index === atual()">{{ passo }}</li>
      }
    </ol>
    <p aria-live="polite">Passo {{ atual() + 1 }} de {{ passos.length }}</p>
    <button type="button" (click)="avancar()" [disabled]="atual() === passos.length - 1">
      Próximo passo
    </button>
    <button type="button" (click)="recomecar()">Recomeçar</button>
  `,
  styles: `
    li.atual { font-weight: 700; }
  `,
})
export class Pausa {
  protected readonly titulo = 'Um minuto antes de codar';

  protected readonly passos = [
    'Tire as mãos do teclado e apoie os pés no chão.',
    'Note três sons ao redor, sem dar nome a eles.',
    'Acompanhe três respirações, sem mudar o ritmo.',
    'Perceba para onde a mente foi; volte sem bronca.',
    'Escolha a próxima tarefa. Uma só.',
  ];

  protected readonly atual = signal(0);

  protected avancar(): void {
    this.atual.update((n) => Math.min(n + 1, this.passos.length - 1));
  }

  protected recomecar(): void {
    this.atual.set(0);
  }
}
