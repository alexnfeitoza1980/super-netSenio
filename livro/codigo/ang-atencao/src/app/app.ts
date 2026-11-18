import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';

/** A casca do Diário de Atenção: navegação e o lugar onde as páginas aparecem. */
@Component({
  selector: 'app-root',
  imports: [RouterOutlet, RouterLink, RouterLinkActive],
  template: `
    <a class="pular" href="#conteudo">Pular para o conteúdo</a>
    <header>
      <strong>Diário de Atenção</strong>
      <nav aria-label="Principal">
        <a routerLink="/diario/hoje" routerLinkActive="ativo" ariaCurrentWhenActive="page">Hoje</a>
        <a routerLink="/diario/historico" routerLinkActive="ativo" ariaCurrentWhenActive="page">Histórico</a>
        <a routerLink="/pausa" routerLinkActive="ativo" ariaCurrentWhenActive="page">Pausa de 1 minuto</a>
        <a routerLink="/capitulos" routerLinkActive="ativo" ariaCurrentWhenActive="page">Demonstrações</a>
      </nav>
    </header>
    <main id="conteudo" tabindex="-1">
      <router-outlet />
    </main>
    <footer>
      Nada aqui é terapia. Os dados ficam só neste navegador. Em sofrimento intenso, procure ajuda (no Brasil, CVV 188).
    </footer>
  `,
  styles: `
    :host { display: block; max-width: 52rem; margin: 0 auto; padding: 1rem; }
    header { display: flex; flex-wrap: wrap; gap: .75rem 1.5rem; align-items: center; justify-content: space-between; padding-bottom: .75rem; border-bottom: 1px solid var(--linha); }
    nav { display: flex; flex-wrap: wrap; gap: .25rem 1rem; }
    nav a { text-decoration: none; padding: .2rem 0; border-bottom: 2px solid transparent; }
    nav a.ativo { border-bottom-color: var(--primaria); font-weight: 700; }
    main { padding: 1rem 0 2rem; }
    footer { font-size: .85rem; color: var(--tinta-2); border-top: 1px solid var(--linha); padding-top: .75rem; }
    .pular { position: absolute; left: -999px; }
    .pular:focus { left: 1rem; top: 1rem; background: var(--superficie); padding: .5rem 1rem; }
  `,
})
export class App {}
