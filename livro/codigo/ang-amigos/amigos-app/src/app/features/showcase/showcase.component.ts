import { Component, ChangeDetectionStrategy, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule, Validators } from '@angular/forms';
import { AmigosStoreService } from '../../core/services/amigos-store.service';
import { AmgButtonComponent } from '../../ui/button/amg-button.component';
import { AmgDialogComponent } from '../../ui/dialog/amg-dialog.component';
import { AmgSelectComponent, AmgOption } from '../../ui/select/amg-select.component';

@Component({
  selector: 'app-showcase',
  standalone: true,
  imports: [
    CommonModule, 
    ReactiveFormsModule, 
    AmgButtonComponent, 
    AmgDialogComponent, 
    AmgSelectComponent
  ],
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="showcase-container">
      <header class="showcase-header">
        <div class="brand">
          <span class="icon">🤝</span>
          <div>
            <h1>Amigos UI Showcase</h1>
            <p>Design System com Angular 19, CDK & Dale Carnegie</p>
          </div>
        </div>

        <div class="header-stats">
          <span class="pill-stat">Total Elogios: <strong>{{ store.totalElogiosGeral() }}</strong></span>
          <button type="button" class="btn-theme" (click)="alternarTema()">
            {{ temaEscuro() ? '☀️ Claro' : '🌙 Escuro' }}
          </button>
        </div>
      </header>

      <main class="showcase-grid">
        <!-- Card 1: Formulário de Envio de Elogio com CVA e Reactive Forms -->
        <section class="card">
          <h2>💌 Enviar Elogio Sincero</h2>
          <p class="subtitle">Componentes integrados com ControlValueAccessor e Signals.</p>

          <form class="form-elogio">
            <label>
              <span>Destinatário:</span>
              <amg-select 
                [options]="opcoesAmigos()"
                [formControl]="destinatarioControl"
                placeholder="Selecione um colega..." />
            </label>

            <label>
              <span>Mensagem de Reconhecimento:</span>
              <textarea 
                rows="3" 
                class="amg-textarea"
                [formControl]="mensagemControl"
                placeholder="Descreva a atitude positiva que você observou..."></textarea>
            </label>

            <div class="form-actions">
              <amg-button 
                variant="primary" 
                [disabled]="!formularioValido()"
                (click)="abrirConfirmacao()">
                Visualizar e Confirmar
              </amg-button>
            </div>
          </form>
        </section>

        <!-- Card 2: Lista Reativa de Amigos e Conexões -->
        <section class="card">
          <h2>👥 Conexões de Confiança</h2>
          <p class="subtitle">Filtragem síncrona com Signals unidirecionais.</p>

          <input 
            type="search" 
            class="search-input"
            placeholder="Filtrar por nome ou cargo..."
            [value]="store.termoFiltro()"
            (input)="aoFiltrar($event)" />

          <ul class="amigos-list">
            @for (amigo of store.amigosFiltrados(); track amigo.id) {
              <li class="amigo-item">
                <div class="amigo-dados">
                  <strong>{{ amigo.nome }}</strong>
                  <small>{{ amigo.cargo }} · {{ amigo.departamento }}</small>
                </div>
                <div class="amigo-badge">
                  ★ {{ amigo.totalElogios }}
                </div>
              </li>
            } @empty {
              <li class="empty-msg">Nenhum colega encontrado com esse nome.</li>
            }
          </ul>
        </section>
      </main>

      <!-- Diálogo Acolhedor de Confirmação com CDK e A11y -->
      <amg-dialog [(isOpen)]="dialogoAberto" (closed)="aoFecharDialogo()">
        <h3>Confirmar Envio do Elogio</h3>
        <p>Você está prestes a valorizar o trabalho sincero de um colega.</p>

        <div class="dialog-resumo">
          <p><strong>Para:</strong> {{ nomeDestinatarioSelecionado() }}</p>
          <p><strong>Mensagem:</strong> "{{ mensagemControl.value }}"</p>
        </div>

        <div class="dialog-actions">
          <amg-button variant="ghost" (click)="dialogoAberto.set(false)">Cancelar</amg-button>
          <amg-button variant="success" (click)="confirmarEnvioElogio()">🎉 Enviar Agora</amg-button>
        </div>
      </amg-dialog>
    </div>
  `,
  styles: [`
    .showcase-container { max-width: 960px; margin: 0 auto; padding: 24px 16px; font-family: inherit; }
    .showcase-header {
      display: flex; justify-content: space-between; align-items: center;
      margin-bottom: 24px; padding-bottom: 16px; border-bottom: 1px solid var(--amg-border);
    }
    .brand { display: flex; align-items: center; gap: 12px; }
    .brand .icon { font-size: 32px; }
    .brand h1 { margin: 0; font-size: 22px; color: var(--amg-text-main); }
    .brand p { margin: 2px 0 0; font-size: 13px; color: var(--amg-text-muted); }
    .header-stats { display: flex; align-items: center; gap: 12px; }
    .pill-stat {
      background: var(--amg-primary-soft); color: var(--amg-primary);
      padding: 6px 14px; border-radius: 9999px; font-size: 13px;
    }
    .btn-theme {
      background: var(--amg-surface); border: 1px solid var(--amg-border);
      color: var(--amg-text-main); padding: 6px 12px; border-radius: 6px; cursor: pointer;
    }
    .showcase-grid { display: grid; grid-template-columns: 1fr 1fr; gap: 24px; }
    @media (max-width: 768px) { .showcase-grid { grid-template-columns: 1fr; } }
    .card {
      background: var(--amg-surface); border: 1px solid var(--amg-border);
      border-radius: var(--amg-radius); padding: 20px; box-shadow: var(--amg-shadow);
    }
    .card h2 { margin: 0; font-size: 17px; }
    .subtitle { margin: 4px 0 16px; font-size: 12px; color: var(--amg-text-muted); }
    .form-elogio { display: flex; flex-direction: column; gap: 14px; }
    label { display: flex; flex-direction: column; gap: 6px; font-size: 13px; font-weight: 600; }
    .amg-textarea, .search-input {
      width: 100%; padding: 10px 12px; border: 1px solid var(--amg-border);
      border-radius: 6px; background: var(--amg-surface); color: var(--amg-text-main); font-family: inherit;
    }
    .form-actions { margin-top: 8px; display: flex; justify-content: flex-end; }
    .amigos-list { list-style: none; padding: 0; margin: 16px 0 0; display: flex; flex-direction: column; gap: 8px; }
    .amigo-item {
      display: flex; justify-content: space-between; align-items: center;
      padding: 10px 12px; border-radius: 6px; border: 1px solid var(--amg-border);
    }
    .amigo-dados { display: flex; flex-direction: column; }
    .amigo-dados strong { font-size: 13px; }
    .amigo-dados small { font-size: 11px; color: var(--amg-text-muted); }
    .amigo-badge { background: #dbeafe; color: #1e40af; font-weight: 700; font-size: 11px; padding: 2px 8px; border-radius: 9999px; }
    .empty-msg { text-align: center; color: var(--amg-text-muted); padding: 20px; font-size: 13px; }
    .dialog-resumo { background: var(--amg-surface-subtle); padding: 12px; border-radius: 6px; margin: 16px 0; font-size: 13px; }
    .dialog-actions { display: flex; justify-content: flex-end; gap: 8px; }
  `]
})
export class ShowcaseComponent {
  protected readonly store = inject(AmigosStoreService);

  readonly temaEscuro = signal(false);
  readonly dialogoAberto = signal(false);

  readonly destinatarioControl = new FormControl<string>('1', Validators.required);
  readonly mensagemControl = new FormControl<string>('', [Validators.required, Validators.minLength(10)]);

  readonly opcoesAmigos = signal<AmgOption<string>[]>(
    this.store.amigos().map(a => ({
      label: a.nome,
      value: a.id,
      badge: a.cargo
    }))
  );

  protected formularioValido(): boolean {
    return this.destinatarioControl.valid && this.mensagemControl.valid;
  }

  protected nomeDestinatarioSelecionado(): string {
    const id = this.destinatarioControl.value;
    const amigo = this.store.amigos().find(a => a.id === id);
    return amigo ? amigo.nome : 'Colega';
  }

  protected aoFiltrar(event: Event): void {
    const inputEl = event.target as HTMLInputElement;
    this.store.termoFiltro.set(inputEl.value);
  }

  protected alternarTema(): void {
    this.temaEscuro.update(t => !t);
    document.documentElement.setAttribute('data-theme', this.temaEscuro() ? 'dark' : 'light');
  }

  protected abrirConfirmacao(): void {
    if (this.formularioValido()) {
      this.dialogoAberto.set(true);
    }
  }

  protected confirmarEnvioElogio(): void {
    const amigoId = this.destinatarioControl.value!;
    const mensagem = this.mensagemControl.value!;

    this.store.registrarElogio({
      amigoId,
      remetente: 'Você',
      mensagem,
      categoria: 'gentileza'
    });

    this.mensagemControl.reset('');
    this.dialogoAberto.set(false);
  }

  protected aoFecharDialogo(): void {
    // Ação pós fechamento se necessário
  }
}

