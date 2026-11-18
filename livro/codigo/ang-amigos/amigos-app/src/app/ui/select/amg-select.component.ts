import { 
  Component, 
  ChangeDetectionStrategy, 
  input, 
  output, 
  signal, 
  computed, 
  forwardRef 
} from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';

export interface AmgOption<T> {
  readonly label: string;
  readonly value: T;
  readonly badge?: string;
}

@Component({
  selector: 'amg-select',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => AmgSelectComponent),
      multi: true
    }
  ],
  template: `
    <div class="amg-select-wrapper">
      <button
        type="button"
        class="amg-select-trigger"
        [attr.aria-expanded]="isOpen()"
        [attr.aria-haspopup]="'listbox'"
        [disabled]="disabled()"
        (click)="alternarDropdown()">
        <span>{{ textoSelecionado() ?? placeholder() }}</span>
        <span class="seta" aria-hidden="true">▾</span>
      </button>

      @if (isOpen()) {
        <ul class="amg-select-dropdown" role="listbox" tabindex="-1">
          @for (opcao of options(); track opcao.value) {
            <li
              role="option"
              class="amg-select-item"
              [class.amg-selected]="estaSelecionado(opcao.value)"
              [attr.aria-selected]="estaSelecionado(opcao.value)"
              (click)="selecionarItem(opcao)">
              <span>{{ opcao.label }}</span>
              @if (opcao.badge) {
                <small class="badge">{{ opcao.badge }}</small>
              }
            </li>
          } @empty {
            <li class="amg-empty">Nenhuma opção disponível</li>
          }
        </ul>
      }
    </div>
  `,
  styles: [`
    .amg-select-wrapper { position: relative; width: 100%; font-family: inherit; }
    .amg-select-trigger {
      display: flex; justify-content: space-between; align-items: center;
      width: 100%; padding: 10px 14px; border: 1px solid #cbd5e1;
      border-radius: 6px; background: #ffffff; color: #0f172a;
      cursor: pointer; font-size: 14px;
    }
    .amg-select-dropdown {
      position: absolute; top: calc(100% + 4px); left: 0; right: 0;
      max-height: 220px; overflow-y: auto; background: #ffffff;
      border: 1px solid #cbd5e1; border-radius: 6px; padding: 4px;
      margin: 0; list-style: none; box-shadow: 0 10px 15px -3px rgba(0,0,0,0.1);
      z-index: 50;
    }
    .amg-select-item {
      padding: 8px 12px; border-radius: 4px; cursor: pointer;
      display: flex; justify-content: space-between; align-items: center; font-size: 13px;
    }
    .amg-select-item:hover { background: #eff6ff; color: #1d4ed8; }
    .amg-selected { background: #dbeafe; font-weight: 600; color: #1e40af; }
    .badge { background: #e2e8f0; padding: 2px 6px; border-radius: 4px; font-size: 10px; }
    .amg-empty { padding: 12px; text-align: center; color: #94a3b8; font-size: 12px; }
  `]
})
export class AmgSelectComponent<T> implements ControlValueAccessor {
  readonly options = input.required<readonly AmgOption<T>[]>();
  readonly placeholder = input<string>('Selecione uma opção...');

  readonly selectionChange = output<T>();

  protected readonly valor = signal<T | null>(null);
  protected readonly disabled = signal<boolean>(false);
  protected readonly isOpen = signal<boolean>(false);

  private onChange: (v: T) => void = () => {};
  private onTouched: () => void = () => {};

  protected readonly textoSelecionado = computed(() => {
    const val = this.valor();
    if (val === null || val === undefined) return null;
    const achado = this.options().find(opt => opt.value === val);
    return achado ? achado.label : null;
  });

  writeValue(obj: T | null): void {
    this.valor.set(obj);
  }

  registerOnChange(fn: (v: T) => void): void {
    this.onChange = fn;
  }

  registerOnTouched(fn: () => void): void {
    this.onTouched = fn;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled.set(isDisabled);
  }

  protected alternarDropdown(): void {
    if (this.disabled()) return;
    this.isOpen.update(v => !v);
  }

  protected estaSelecionado(val: T): boolean {
    return this.valor() === val;
  }

  protected selecionarItem(opcao: AmgOption<T>): void {
    this.valor.set(opcao.value);
    this.onChange(opcao.value);
    this.onTouched();
    this.selectionChange.emit(opcao.value);
    this.isOpen.set(false);
  }
}

