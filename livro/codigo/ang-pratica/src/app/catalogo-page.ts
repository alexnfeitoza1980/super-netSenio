import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { LivroCard } from './livro-card';
import { BibliotecaStore } from './biblioteca.store';

@Component({
  selector: 'app-catalogo-page',
  imports: [LivroCard],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <section class="intro" aria-labelledby="titulo-catalogo">
      <p class="sobretitulo">Sua próxima história começa aqui</p>
      <h1 id="titulo-catalogo">Uma biblioteca para explorar.</h1>
      <p>Encontre algo que desperte sua curiosidade. Guarde seus favoritos e continue de onde parou.</p>
    </section>
    <section class="ferramentas" aria-label="Filtros do catálogo">
      <label for="busca">Buscar livros</label>
      <input id="busca" type="search" autocomplete="off" placeholder="Título, autora ou assunto"
        [value]="biblioteca.busca()" (input)="buscar($event)" />
      <fieldset>
        <legend>Categoria</legend>
        <div class="filtros">
          @for (categoria of biblioteca.categorias; track categoria) {
            <button type="button" [class.selecionado]="biblioteca.categoria() === categoria"
              [attr.aria-pressed]="biblioteca.categoria() === categoria"
              (click)="biblioteca.selecionarCategoria(categoria)">{{ categoria }}</button>
          }
        </div>
      </fieldset>
    </section>
    <section aria-labelledby="resultados-titulo">
      <div class="cabecalho-resultados">
        <h2 id="resultados-titulo">Catálogo</h2>
        <p aria-live="polite">{{ biblioteca.resultados().length }} {{ biblioteca.resultados().length === 1 ? 'livro encontrado' : 'livros encontrados' }}</p>
      </div>
      @if (biblioteca.resultados().length) {
        <div class="grade">
          @for (livro of biblioteca.resultados(); track livro.id) {
            <app-livro-card [livro]="livro" />
          }
        </div>
      } @else {
        <div class="vazio" role="status">
          <h3>Nenhum livro por aqui ainda</h3>
          <p>Tente outro termo ou escolha a categoria “Todas”.</p>
          <button type="button" (click)="limparFiltros()">Limpar busca e filtros</button>
        </div>
      }
    </section>
  `,
  styles: `
    :host { display: block; }
    .intro { max-width: 760px; padding: 42px 0 22px; }
    .sobretitulo { margin: 0 0 10px; color: var(--primaria); font-size: .78rem; font-weight: 800; letter-spacing: .13em; text-transform: uppercase; }
    h1 { margin: 0; font: 800 clamp(2.2rem, 6vw, 4.2rem)/1.02 Georgia, serif; letter-spacing: -.04em; }
    .intro > p:last-child { max-width: 62ch; color: var(--tinta-2); font-size: 1.1rem; }
    .ferramentas { display: grid; gap: 9px; margin: 12px 0 32px; }
    .ferramentas > label, legend { font-weight: 750; }
    input { width: min(100%, 560px); padding: 13px 15px; border: 1px solid var(--linha); border-radius: 12px; background: var(--superficie); color: var(--tinta); font: inherit; }
    fieldset { margin: 7px 0 0; padding: 0; border: 0; }
    legend { margin-bottom: 8px; }
    .filtros { display: flex; flex-wrap: wrap; gap: 8px; }
    .filtros button, .vazio button { padding: 8px 13px; border: 1px solid var(--linha); border-radius: 999px; background: var(--superficie); color: var(--tinta); font: inherit; }
    .filtros button.selecionado { border-color: var(--primaria); background: var(--primaria); color: white; }
    .cabecalho-resultados { display: flex; flex-wrap: wrap; align-items: baseline; justify-content: space-between; gap: 8px; margin-bottom: 15px; }
    h2 { margin: 0; font: 750 1.65rem Georgia, serif; }
    .cabecalho-resultados p { margin: 0; color: var(--tinta-2); }
    .grade { display: grid; grid-template-columns: repeat(auto-fit, minmax(min(100%, 235px), 1fr)); gap: 18px; }
    .vazio { padding: 30px; border: 1px dashed var(--linha); border-radius: 16px; background: var(--superficie); }
    .vazio h3 { margin: 0 0 5px; }
    .vazio p { color: var(--tinta-2); }
  `,
})
export class CatalogoPage {
  readonly biblioteca = inject(BibliotecaStore);

  buscar(evento: Event): void {
    if (evento.target instanceof HTMLInputElement) this.biblioteca.atualizarBusca(evento.target.value);
  }

  limparFiltros(): void {
    this.biblioteca.atualizarBusca('');
    this.biblioteca.selecionarCategoria('Todas');
  }
}
