import { 
  Component, 
  ChangeDetectionStrategy, 
  model, 
  input, 
  output 
} from '@angular/core';

@Component({
  selector: 'amg-dialog',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    @if (isOpen()) {
      <div class="amg-dialog-backdrop" (click)="solicitarFechamento()">
        <div 
          class="amg-dialog-panel" 
          role="dialog"
          aria-modal="true"
          (click)="$event.stopPropagation()">
          
          <div class="amg-dialog-content">
            <ng-content />
          </div>

          @if (closable()) {
            <button 
              type="button" 
              class="amg-dialog-close" 
              aria-label="Fechar diálogo"
              (click)="solicitarFechamento()">
              ✕
            </button>
          }
        </div>
      </div>
    }
  `,
  styles: [`
    .amg-dialog-backdrop {
      position: fixed;
      inset: 0;
      background: rgba(15, 23, 42, 0.6);
      backdrop-filter: blur(4px);
      display: grid;
      place-items: center;
      z-index: 1000;
    }
    .amg-dialog-panel {
      background: #ffffff;
      border-radius: 12px;
      padding: 24px;
      max-width: 500px;
      width: calc(100% - 32px);
      position: relative;
      box-shadow: 0 20px 25px -5px rgba(0,0,0,0.1);
    }
    .amg-dialog-close {
      position: absolute;
      top: 16px;
      right: 16px;
      background: none;
      border: none;
      font-size: 16px;
      cursor: pointer;
      color: #64748b;
      padding: 4px 8px;
    }
    .amg-dialog-close:hover { color: #0f172a; }
  `]
})
export class AmgDialogComponent {
  readonly isOpen = model<boolean>(false);
  readonly closable = input<boolean>(true);
  readonly closed = output<void>();

  protected solicitarFechamento(): void {
    this.isOpen.set(false);
    this.closed.emit();
  }
}

