import { ChangeDetectionStrategy, Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { BibliotecaStore } from './biblioteca.store';
import { Livro } from './livro';

@Component({
  selector: 'app-livro-card',
  imports: [RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <article class="card">
      <a class="capa" [style.--cor]="livro().cor" [routerLink]="['/livros', livro().id]" [attr.aria-label]="'Abrir ' + livro().titulo">
        <span class="marca" aria-hidden="true">{{ letra() }}</span>
        <span class="titulo-capa">{{ livro().titulo }}</span>
        <span class="autor-capa">{{ livro().autor }}</span>
      </a>
      <div class="detalhes">
        <p class="categoria">{{ livro().categoria }} · {{ livro().ano }}</p>
        <h3><a [routerLink]="['/livros', livro().id]">{{ livro().titulo }}</a></h3>
        <p class="autor">por {{ livro().autor }}</p>
        <div class="acoes">
          <a class="link" [routerLink]="['/livros', livro().id]">Ver livro <span aria-hidden="true">→</span></a>
          <button type="button" class="favoritar" (click)="biblioteca.alternarFavorito(livro().id)"
            [attr.aria-pressed]="biblioteca.ehFavorito(livro().id)"
            [attr.aria-label]="(biblioteca.ehFavorito(livro().id) ? 'Remover dos favoritos: ' : 'Adicionar aos favoritos: ') + livro().titulo">
            {{ biblioteca.ehFavorito(livro().id) ? '♥' : '♡' }}
          </button>
        </div>
      </div>
    </article>
  `,
  styles: `
    :host { display: block; min-width: 0; }
    .card { height: 100%; overflow: hidden; border: 1px solid var(--linha); border-radius: 18px; background: var(--superficie); box-shadow: 0 8px 26px #1425360b; transition: transform .18s ease, box-shadow .18s ease; }
    .card:hover { transform: translateY(-3px); box-shadow: 0 14px 32px #14253618; }
    .capa { position: relative; display: flex; min-height: 214px; flex-direction: column; justify-content: space-between; overflow: hidden; padding: 22px; background: linear-gradient(145deg, color-mix(in srgb, var(--cor) 78%, #13212c), var(--cor)); color: white; text-decoration: none; }
    .capa::after { position: absolute; top: -34px; right: -24px; width: 170px; aspect-ratio: 1; border: 1px solid #ffffff55; border-radius: 50%; box-shadow: 0 0 0 22px #ffffff12, 0 0 0 46px #ffffff0b; content: ""; }
    .marca { z-index: 1; align-self: flex-end; color: #ffffffb8; font: 800 2.4rem Georgia, serif; }
    .titulo-capa, .autor-capa { z-index: 1; text-shadow: 0 2px 8px #0005; }
    .titulo-capa { max-width: 14ch; font: 700 1.45rem/1.1 Georgia, serif; }
    .autor-capa { margin-top: 6px; font-size: .82rem; }
    .detalhes { padding: 17px 18px 15px; }
    .categoria, .autor { margin: 0; color: var(--tinta-2); font-size: .84rem; }
    h3 { margin: 7px 0 3px; font: 700 1.2rem/1.25 Georgia, serif; }
    h3 a { color: inherit; text-decoration: none; }
    .acoes { display: flex; align-items: center; justify-content: space-between; margin-top: 14px; }
    .link { color: var(--primaria); font-weight: 750; text-decoration: none; }
    .favoritar { width: 42px; height: 42px; border: 1px solid var(--linha); border-radius: 50%; background: var(--superficie); color: #a23d55; font-size: 1.35rem; }
    .favoritar[aria-pressed="true"] { border-color: #efb0b7; background: #fff0f1; }
    @media (prefers-reduced-motion: reduce) { .card { transition: none; } }
  `,
})
export class LivroCard {
  readonly livro = input.required<Livro>();

  constructor(readonly biblioteca: BibliotecaStore) {}

  letra(): string {
    return this.livro().titulo.slice(0, 1).toLocaleUpperCase('pt-BR');
  }
}
