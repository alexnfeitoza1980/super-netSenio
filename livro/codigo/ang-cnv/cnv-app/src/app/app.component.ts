import { Component } from '@angular/core';
import { TradutorCnvComponent } from './features/tradutor-cnv/tradutor-cnv.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [TradutorCnvComponent],
  template: `
    <header class="app-header">
      <div class="header-container">
        <div class="logo-bloco">
          <span class="logo-marca">♥</span>
          <div>
            <h1>Comunicação Não-Violenta em Angular</h1>
            <p class="tagline">Laboratório 2: O Tradutor de Empatia</p>
          </div>
        </div>
      </div>
    </header>

    <main class="app-main">
      <app-tradutor-cnv />
    </main>

    <footer class="app-footer">
      <p>Desenvolvido com Angular 17+ Standalone, Signals e os ensinamentos de Marshall B. Rosenberg.</p>
    </footer>
  `,
  styles: [`
    .app-header {
      background: linear-gradient(135deg, #1f0c03, #7a300a 58%, #d96b27);
      color: white;
      padding: 1.5rem 1rem;
      border-bottom: 2px solid #f59e0b;
    }
    .header-container {
      max-width: 860px;
      margin: 0 auto;
    }
    .logo-bloco {
      display: flex;
      align-items: center;
      gap: 1rem;
    }
    .logo-marca {
      background: rgba(255, 255, 255, 0.15);
      width: 48px;
      height: 48px;
      border-radius: 12px;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 1.8rem;
    }
    .app-header h1 {
      margin: 0;
      font-size: 1.4rem;
      font-weight: 700;
    }
    .tagline {
      margin: 0.2rem 0 0 0;
      font-size: 0.9rem;
      color: #fde68a;
    }
    .app-main {
      padding: 2.5rem 1rem;
      min-height: calc(100vh - 180px);
    }
    .app-footer {
      text-align: center;
      padding: 1.5rem;
      font-size: 0.85rem;
      color: #6c3c1e;
      border-top: 1px solid #f5dfd0;
      background: #fdf4eb;
    }
  `]
})
export class AppComponent {}

