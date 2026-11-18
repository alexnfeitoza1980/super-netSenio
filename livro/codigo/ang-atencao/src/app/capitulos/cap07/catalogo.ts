import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { DEMOS } from './demos';

/** Cap. 7 — a lista de demonstrações; `naoAchei` chega da query string (?naoAchei=...). */
@Component({
  selector: 'app-catalogo',
  imports: [RouterLink],
  template: `
    <h1>Demonstrações dos capítulos</h1>
    @if (naoAchei()) {
      <p role="status">Não encontrei a demonstração “{{ naoAchei() }}”. Que tal uma destas?</p>
    }
    <ul>
      @for (d of demos; track d.id) {
        <li><a [routerLink]="['/capitulos', d.id]">Cap. {{ d.capitulo }} · {{ d.titulo }}</a></li>
      }
    </ul>
  `,
})
export class Catalogo {
  readonly naoAchei = input<string>();
  protected readonly demos = Object.entries(DEMOS).map(([id, d]) => ({ id, ...d }));
}
