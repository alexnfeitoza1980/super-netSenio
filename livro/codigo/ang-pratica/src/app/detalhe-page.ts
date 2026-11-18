import { ChangeDetectionStrategy, Component, computed, inject, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { BibliotecaStore } from './biblioteca.store';

@Component({
  selector: 'app-detalhe-page',
  imports: [RouterLink],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    @if (livro(); as item) {
      <a class="voltar" routerLink="/">← Voltar ao catálogo</a>
      <article class="detalhe">
        <div class="capa" [style.--cor]="item.cor" aria-hidden="true">
          <span>{{ item.categoria }}</span>
          <strong>{{ item.titulo }}</strong>
          <small>{{ item.autor }}</small>
        </div>
        <div class="texto">
          <p class="categoria">{{ item.categoria }} · {{ item.ano }} · {{ item.paginas }} páginas</p>
          <h1>{{ item.titulo }}</h1>
          <p class="autor">por {{ item.autor }}</p>
          <p class="descricao">{{ item.descricao }}</p>
          <button type="button" class="primario" (click)="alternarConclusao(item.id)">
            {{ biblioteca.progressoDe(item.id) === 100 ? 'Reabrir leitura' : 'Marcar leitura como concluída' }}
          </button>
          <button type="button" class="secundario" (click)="biblioteca.alternarFavorito(item.id)"
            [attr.aria-pressed]="biblioteca.ehFavorito(item.id)">
            {{ biblioteca.ehFavorito(item.id) ? '♥ Nos favoritos' : '♡ Guardar nos favoritos' }}
          </button>
          <section class="progresso" aria-labelledby="progresso-titulo">
            <h2 id="progresso-titulo">Seu progresso</h2>
            <label [for]="'progresso-' + item.id">{{ biblioteca.progressoDe(item.id) }}% concluído</label>
            <input [id]="'progresso-' + item.id" type="range" min="0" max="100" step="5"
              [value]="biblioteca.progressoDe(item.id)" (input)="atualizarProgresso($event, item.id)" />
            <progress [value]="biblioteca.progressoDe(item.id)" max="100">{{ biblioteca.progressoDe(item.id) }}%</progress>
          </section>
        </div>
      </article>
    } @else {
      <section class="nao-encontrado" role="status">
        <p class="sobretitulo">Página não encontrada</p>
        <h1>Este livro não está no catálogo.</h1>
        <p>O endereço pode estar incorreto ou o livro pode ter saído da seleção.</p>
        <a class="primario" routerLink="/">Explorar o catálogo</a>
      </section>
    }
  `,
  styles: `
    :host { display: block; padding: 22px 0 52px; }
    .voltar { color: var(--primaria); font-weight: 750; text-decoration: none; }
    .detalhe { display: grid; grid-template-columns: minmax(210px, 330px) minmax(0, 1fr); align-items: start; gap: clamp(24px, 6vw, 64px); margin: 28px auto; }
    .capa { display: flex; min-height: 430px; flex-direction: column; justify-content: flex-end; gap: 18px; padding: 28px; border-radius: 20px; background: linear-gradient(145deg, color-mix(in srgb, var(--cor) 78%, #13212c), var(--cor)); color: white; box-shadow: 0 20px 46px #14253626; }
    .capa span { align-self: flex-start; padding: 5px 10px; border: 1px solid #ffffff70; border-radius: 999px; background: #ffffff20; font-size: .82rem; }
    .capa strong { font: 800 2rem/1.1 Georgia, serif; }
    .capa small { font-size: .96rem; }
    .texto { padding-top: 10px; }
    .categoria, .autor { color: var(--tinta-2); }
    h1 { margin: 10px 0 5px; font: 800 clamp(2rem, 5vw, 3.5rem)/1.04 Georgia, serif; }
    .descricao { max-width: 62ch; margin: 26px 0; font-size: 1.12rem; line-height: 1.7; }
    button, .primario { display: inline-flex; align-items: center; justify-content: center; min-height: 44px; margin: 0 9px 10px 0; padding: 10px 16px; border: 1px solid var(--primaria); border-radius: 11px; color: var(--primaria); background: transparent; font: inherit; font-weight: 750; text-decoration: none; cursor: pointer; }
    .primario { background: var(--primaria); color: #fff; }
    .progresso { max-width: 480px; margin-top: 24px; padding: 18px; border: 1px solid var(--linha); border-radius: 16px; background: var(--superficie); }
    .progresso h2 { margin: 0 0 10px; font: 750 1.25rem Georgia, serif; }
    .progresso label { display: block; margin-bottom: 5px; }
    .progresso input, progress { display: block; width: 100%; accent-color: var(--primaria); }
    progress { height: 9px; margin-top: 8px; }
    .nao-encontrado { max-width: 640px; margin: 70px auto; padding: 32px; border: 1px solid var(--linha); border-radius: 18px; background: var(--superficie); }
    .sobretitulo { color: var(--primaria); font-weight: 800; text-transform: uppercase; letter-spacing: .1em; }
    @media (max-width: 640px) { .detalhe { grid-template-columns: 1fr; } .capa { min-height: 300px; } }
  `,
})
export class DetalhePage {
  readonly id = input.required<string>();
  readonly biblioteca = inject(BibliotecaStore);
  readonly livro = computed(() => this.biblioteca.encontrar(this.id()));

  alternarConclusao(id: string): void {
    this.biblioteca.atualizarProgresso(id, this.biblioteca.progressoDe(id) === 100 ? 0 : 100);
  }

  atualizarProgresso(evento: Event, id: string): void {
    if (evento.target instanceof HTMLInputElement) this.biblioteca.atualizarProgresso(id, Number(evento.target.value));
  }
}
