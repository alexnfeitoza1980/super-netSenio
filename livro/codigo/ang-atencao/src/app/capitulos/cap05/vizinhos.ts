import { ChangeDetectionStrategy, Component, signal } from '@angular/core';

/** Cap. 5 — mesmo componente, duas estratégias. Os dois guardam um campo COMUM (sem signal). */
@Component({
  selector: 'app-placa-onpush',
  template: `<span>{{ texto }}</span>`,
  // OnPush é o padrão no Angular 22: não precisaria escrever. Está aqui para comparar.
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class PlacaOnPush {
  texto = 'antes';
}

@Component({
  selector: 'app-placa-eager',
  template: `<span>{{ texto }}</span>`,
  // Eager (o antigo "Default"): é conferido em TODA passada de detecção que chega até ele.
  changeDetection: ChangeDetectionStrategy.Eager,
})
export class PlacaEager {
  texto = 'antes';
}

@Component({
  selector: 'app-vizinhos',
  imports: [PlacaOnPush, PlacaEager],
  template: `
    <app-placa-onpush #a />
    <app-placa-eager #b />
    <button type="button" (click)="a.texto = 'depois'; b.texto = 'depois'; cliques.set(cliques() + 1)">
      Mudar as duas ({{ cliques() }})
    </button>
  `,
})
export class Vizinhos {
  protected readonly cliques = signal(0);
}
