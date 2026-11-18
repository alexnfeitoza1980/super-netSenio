import { 
  Component, 
  ChangeDetectionStrategy, 
  input, 
  computed, 
  booleanAttribute 
} from '@angular/core';

export type AmgButtonVariant = 'primary' | 'secondary' | 'ghost' | 'success';

@Component({
  selector: 'amg-button',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <button 
      [class]="classesComputadas()"
      [disabled]="disabled()"
      type="button">
      <ng-content />
    </button>
  `,
  styles: [`
    :host { display: inline-block; }
    button {
      display: inline-flex;
      align-items: center;
      justify-content: center;
      gap: 8px;
      padding: 8px 16px;
      border-radius: 6px;
      font-size: 14px;
      font-weight: 600;
      font-family: inherit;
      cursor: pointer;
      border: 1px solid transparent;
      transition: all 0.15s ease-in-out;
    }
    button:focus-visible {
      outline: 2px solid #2563eb;
      outline-offset: 2px;
    }
    .amg-btn-primary { background: #2563eb; color: #ffffff; }
    .amg-btn-primary:hover:not(:disabled) { background: #1d4ed8; }
    
    .amg-btn-secondary { background: #e2e8f0; color: #1e293b; }
    .amg-btn-secondary:hover:not(:disabled) { background: #cbd5e1; }
    
    .amg-btn-ghost { background: transparent; border-color: #cbd5e1; color: #475569; }
    .amg-btn-ghost:hover:not(:disabled) { background: #f8fafc; color: #0f172a; }

    .amg-btn-success { background: #16a34a; color: #ffffff; }
    .amg-btn-success:hover:not(:disabled) { background: #15803d; }

    button:disabled { opacity: 0.5; cursor: not-allowed; }
  `]
})
export class AmgButtonComponent {
  readonly variant = input<AmgButtonVariant>('primary');

  readonly disabled = input<boolean, boolean | string>(false, {
    transform: booleanAttribute
  });

  protected readonly classesComputadas = computed(() => {
    return `amg-btn amg-btn-${this.variant()}`;
  });
}

