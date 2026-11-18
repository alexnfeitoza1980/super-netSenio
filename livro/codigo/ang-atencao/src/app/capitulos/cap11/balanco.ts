import { Component, computed, signal } from '@angular/core';
import { Atividade, Efeito, resumir } from './atividades';
import { GraficoDoDia } from './grafico-do-dia';
import { MinutosPipe } from './minutos.pipe';

/**
 * Cap. 11 — desempenho sem sofrimento:
 * - computed guarda o resumo (só recalcula se a lista mudar);
 * - track a.id deixa o @for reaproveitar os itens que não mudaram;
 * - o pipe puro formata sem recalcular à toa;
 * - @defer adia o gráfico até ele entrar na tela.
 */
@Component({
  selector: 'app-balanco',
  imports: [MinutosPipe, GraficoDoDia],
  template: `
    <h2>Balanço do dia</h2>
    <ul>
      @for (a of atividades(); track a.id) {
        <li>
          {{ a.nome }} · {{ a.minutos | minutos }}
          <select [attr.aria-label]="'Efeito de ' + a.nome" (change)="classificar(a.id, $any($event.target).value)">
            @for (e of efeitos; track e) {
              <option [value]="e" [selected]="e === a.efeito">{{ e }}</option>
            }
          </select>
        </li>
      }
    </ul>
    <p>Nutre: {{ resumo().nutre | minutos }} · Desgasta: {{ resumo().desgasta | minutos }}</p>

    @defer (on viewport) {
      <app-grafico-do-dia [resumo]="resumo()" />
    } @placeholder {
      <p>O gráfico aparece quando você chegar aqui.</p>
    }

    @if (resumo().desgasta > resumo().nutre) {
      <p>Hoje pesou mais do que nutriu. Há uma coisa pequena, de cinco minutos, que nutre você?</p>
    }
  `,
})
export class Balanco {
  protected readonly efeitos: readonly Efeito[] = ['nutre', 'desgasta', 'neutro'];
  readonly atividades = signal<readonly Atividade[]>([
    { id: 1, nome: 'Reunião longa', minutos: 90, efeito: 'desgasta' },
    { id: 2, nome: 'Pareamento com colega', minutos: 60, efeito: 'nutre' },
    { id: 3, nome: 'Responder e-mails', minutos: 45, efeito: 'neutro' },
    { id: 4, nome: 'Caminhar no almoço', minutos: 20, efeito: 'nutre' },
  ]);

  protected readonly resumo = computed(() => resumir(this.atividades()));

  protected classificar(id: number, efeito: Efeito): void {
    this.atividades.update((lista) => lista.map((a) => (a.id === id ? { ...a, efeito } : a)));
  }
}
