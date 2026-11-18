import { computed, inject, Injectable, signal } from '@angular/core';
import { RELOGIO } from '../cap08/tokens';
import { convite, retrospecto } from './retornos';

/** Cap. 12 — um serviço pequeno, fácil de testar porque o tempo vem de fora (RELOGIO). */
@Injectable({ providedIn: 'root' })
export class LembreteDePratica {
  private readonly agora = inject(RELOGIO);
  readonly datas = signal<readonly string[]>([]);

  readonly resumo = computed(() => retrospecto(this.datas()));
  readonly mensagem = computed(() => convite(this.resumo(), this.agora().toISOString().slice(0, 10)));

  registrarHoje(): void {
    const hoje = this.agora().toISOString().slice(0, 10);
    this.datas.update((d) => (d.includes(hoje) ? d : [...d, hoje]));
  }
}
