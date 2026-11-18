import { ChangeDetectionStrategy, Component } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-nao-encontrada-page',
  imports: [RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <section>
      <p class="codigo">404 · Caminho não encontrado</p>
      <h1>Essa página não está na estante.</h1>
      <p>O endereço pode ter mudado. Vamos voltar juntos ao catálogo.</p>
      <a routerLink="/">Abrir o catálogo</a>
    </section>
  `,
  styles: `
    :host { display: grid; min-height: 55vh; place-items: center; }
    section { max-width: 620px; padding: 30px; text-align: center; }
    .codigo { color: var(--primaria); font-weight: 800; letter-spacing: .1em; }
    h1 { font: 800 clamp(2rem, 6vw, 3.5rem)/1.1 Georgia, serif; }
    section > p:not(.codigo) { color: var(--tinta-2); }
    a { display: inline-block; margin-top: 12px; padding: 11px 16px; border-radius: 10px; background: var(--primaria); color: white; font-weight: 750; text-decoration: none; }
  `,
})
export class NaoEncontradaPage {}
