import { ChangeDetectionStrategy, Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { BibliotecaStore } from './biblioteca.store';

@Component({
  selector: 'app-root',
  imports: [RouterLink, RouterLinkActive, RouterOutlet],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <a class="pular" href="#conteudo">Pular para o conteúdo</a>
    <header class="topo">
      <div class="barra">
        <a class="marca" routerLink="/" aria-label="Biblioteca Viva, início">
          <span aria-hidden="true">B</span>
          <span>Biblioteca <strong>Viva</strong></span>
        </a>
        <nav aria-label="Navegação principal">
          <a routerLink="/" [routerLinkActiveOptions]="{ exact: true }" routerLinkActive="ativo" ariaCurrentWhenActive="page">Catálogo</a>
          <a routerLink="/favoritos" routerLinkActive="ativo" ariaCurrentWhenActive="page">
            Favoritos <span class="contador">{{ biblioteca.quantidadeFavoritos() }}</span>
          </a>
        </nav>
      </div>
    </header>
    @if (biblioteca.avisoPersistencia(); as aviso) {
      <div class="aviso" role="status">
        <p>{{ aviso }}</p>
        <button type="button" (click)="biblioteca.avisoPersistencia.set(null)" aria-label="Fechar aviso">Fechar</button>
      </div>
    }
    <main id="conteudo" class="conteudo" tabindex="-1">
      <router-outlet />
    </main>
    <footer>
      <div class="rodape">
        <p><strong>Biblioteca Viva</strong> · histórias para ler no seu tempo.</p>
        <p>Este protótipo guarda favoritos e progresso apenas neste navegador.</p>
      </div>
    </footer>
  `,
  styles: `
    :host { display: block; min-height: 100vh; }
    .topo { border-bottom: 1px solid var(--linha); background: var(--superficie); }
    .barra { display: flex; width: min(1120px, calc(100% - 36px)); min-height: 76px; align-items: center; justify-content: space-between; gap: 20px; margin: 0 auto; }
    .marca { display: flex; align-items: center; gap: 11px; color: var(--tinta); font: 700 1.1rem Georgia, serif; text-decoration: none; }
    .marca > span:first-child { display: grid; width: 42px; height: 42px; place-items: center; border-radius: 14px; background: linear-gradient(135deg, #142536, #3d5a80 65%, #ee6c4d); color: white; font: 800 1.2rem system-ui; }
    .marca strong { color: var(--primaria); }
    nav { display: flex; flex-wrap: wrap; gap: 8px; }
    nav a { padding: 9px 12px; border-radius: 9px; color: var(--tinta-2); font-weight: 700; text-decoration: none; }
    nav a.ativo { background: var(--suave); color: var(--primaria); }
    .contador { display: inline-grid; min-width: 22px; height: 22px; place-items: center; margin-left: 3px; border-radius: 50%; background: var(--primaria); color: white; font-size: .75rem; }
    .conteudo { width: min(1120px, calc(100% - 36px)); min-height: 65vh; margin: 0 auto; }
    .aviso { display: flex; width: min(1120px, calc(100% - 36px)); align-items: center; justify-content: space-between; gap: 15px; margin: 14px auto 0; padding: 12px 15px; border: 1px solid #d6a64f; border-radius: 10px; background: #fff8e8; color: #473719; }
    .aviso p { margin: 0; }
    .aviso button { border: 0; background: transparent; color: inherit; font: inherit; font-weight: 750; text-decoration: underline; cursor: pointer; }
    footer { border-top: 1px solid var(--linha); background: var(--superficie); }
    .rodape { display: flex; width: min(1120px, calc(100% - 36px)); flex-wrap: wrap; justify-content: space-between; gap: 4px 18px; margin: 0 auto; padding: 18px 0; color: var(--tinta-2); font-size: .88rem; }
    .pular { position: absolute; top: -80px; left: 12px; z-index: 3; padding: 10px 14px; border-radius: 8px; background: var(--tinta); color: white; }
    .pular:focus { top: 12px; }
    @media (max-width: 560px) { .barra { align-items: flex-start; flex-direction: column; justify-content: center; padding: 13px 0; } nav { width: 100%; } .aviso { align-items: flex-start; } }
  `,
})
export class App {
  readonly biblioteca = inject(BibliotecaStore);
}
