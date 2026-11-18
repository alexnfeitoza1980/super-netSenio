import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  template: `
    <header class="app-header">
      <div class="logo-area">
        <span class="escudo">▲</span>
        <div>
          <h1>Antimedo Hub</h1>
          <small>Sistema Operacional de Enfrentamento e Coragem</small>
        </div>
      </div>
      <div class="header-badges">
        <span class="chip-tecnologia">Angular 17+</span>
        <span class="chip-tecnologia">Signals</span>
        <span class="chip-tecnologia">Standalone</span>
      </div>
    </header>
    <main class="app-main">
      <router-outlet></router-outlet>
    </main>
  `,
  styles: [`
    .app-header { background: #1a0008; color: #fff; padding: 16px 28px; border-bottom: 3px solid #dd1b44; display: flex; align-items: center; justify-content: space-between; }
    .logo-area { display: flex; align-items: center; gap: 14px; }
    .escudo { font-size: 28px; font-weight: 900; color: #dd1b44; }
    .logo-area h1 { margin: 0; font-size: 20px; font-weight: 800; letter-spacing: -0.01em; }
    .logo-area small { opacity: 0.75; font-size: 12.5px; }
    .header-badges { display: flex; gap: 8px; }
    .chip-tecnologia { background: #3a0414; color: #ff6b35; font-size: 11.5px; font-weight: 700; padding: 4px 10px; border-radius: 999px; border: 1px solid #9b1230; }
    .app-main { padding: 24px; min-height: calc(100vh - 80px); background: #faf7f5; }
  `]
})
export class AppComponent {}

