import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { LivroCard } from './livro-card';
import { BibliotecaStore } from './biblioteca.store';

@Component({
  selector: 'app-favoritos-page',
  imports: [LivroCard, RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <section class="cabecalho">
      <p class="sobretitulo">Sua seleção pessoal</p>
      <h1>Livros favoritos</h1>
      <p>Uma lista só sua, guardada neste navegador.</p>
    </section>
    @if (biblioteca.favoritos().length) {
      <div class="grade">
        @for (livro of biblioteca.favoritos(); track livro.id) {
          <app-livro-card [livro]="livro" />
        }
      </div>
    } @else {
      <section class="vazio" role="status">
        <h2>Seus favoritos aparecem aqui</h2>
        <p>Explore o catálogo e use o coração para guardar livros que chamaram sua atenção.</p>
        <a routerLink="/">Ir ao catálogo</a>
      </section>
    }
  `,
  styles: `
    :host { display: block; padding-bottom: 48px; }
    .cabecalho { padding: 42px 0 24px; }
    .sobretitulo { margin: 0; color: var(--primaria); font-size: .8rem; font-weight: 800; letter-spacing: .12em; text-transform: uppercase; }
    h1 { margin: 9px 0; font: 800 clamp(2.2rem, 6vw, 3.5rem)/1.05 Georgia, serif; }
    .cabecalho > p:last-child { color: var(--tinta-2); }
    .grade { display: grid; grid-template-columns: repeat(auto-fit, minmax(min(100%, 235px), 1fr)); gap: 18px; }
    .vazio { max-width: 620px; padding: 28px; border: 1px solid var(--linha); border-radius: 17px; background: var(--superficie); }
    .vazio h2 { margin-top: 0; font: 750 1.5rem Georgia, serif; }
    .vazio p { color: var(--tinta-2); }
    .vazio a { color: var(--primaria); font-weight: 750; }
  `,
})
export class FavoritosPage {
  readonly biblioteca = inject(BibliotecaStore);
}
