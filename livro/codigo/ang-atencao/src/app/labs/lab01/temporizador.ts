import { Component, computed, DestroyRef, inject, input, linkedSignal, output, signal } from '@angular/core';
import { duracaoValida, formatarTempo } from './tempo';

/**
 * Lab 1 — Pausa de um minuto: um temporizador gentil.
 * - `restantes` é um linkedSignal: volta ao total sempre que a duração (input) muda.
 * - O intervalo é limpo ao pausar, ao terminar e quando o componente sai da tela (DestroyRef).
 */
@Component({
  selector: 'app-temporizador',
  template: `
    <section aria-labelledby="titulo-pausa">
      <h2 id="titulo-pausa">Pausa de {{ rotuloDuracao() }}</h2>
      <p class="relogio" role="timer" aria-live="off">{{ tempo() }}</p>
      <progress [value]="progresso()" max="1" aria-label="Progresso da pausa"></progress>
      <p aria-live="polite">{{ mensagem() }}</p>

      @if (rodando()) {
        <button type="button" (click)="pausar()">Pausar</button>
      } @else {
        <button type="button" class="primario" (click)="iniciar()" [disabled]="terminou()">
          {{ restantes() === duracao() ? 'Começar' : 'Continuar' }}
        </button>
      }
      <button type="button" (click)="reiniciar()">Reiniciar</button>
    </section>
  `,
  styles: `
    .relogio { font-size: 3rem; font-variant-numeric: tabular-nums; margin: .2em 0; }
    progress { width: 100%; }
  `,
})
export class Temporizador {
  readonly duracao = input(60, { transform: duracaoValida });
  readonly terminada = output<void>();

  protected readonly restantes = linkedSignal(() => this.duracao());
  protected readonly rodando = signal(false);

  protected readonly tempo = computed(() => formatarTempo(this.restantes()));
  protected readonly progresso = computed(() => 1 - this.restantes() / this.duracao());
  protected readonly terminou = computed(() => this.restantes() === 0);
  protected readonly rotuloDuracao = computed(() =>
    this.duracao() % 60 === 0 ? `${this.duracao() / 60} min` : `${this.duracao()} s`,
  );
  protected readonly mensagem = computed(() => {
    if (this.terminou()) return 'Pronto. Note como você está agora, sem precisar mudar nada.';
    if (this.rodando()) return 'Acompanhe a respiração. Se a mente sair, é só voltar.';
    return 'Quando quiser, comece.';
  });

  private intervalo: ReturnType<typeof setInterval> | undefined;

  constructor() {
    inject(DestroyRef).onDestroy(() => this.parar());
  }

  protected iniciar(): void {
    if (this.rodando() || this.terminou()) return;
    this.rodando.set(true);
    this.intervalo = setInterval(() => this.tique(), 1000);
  }

  protected pausar(): void {
    this.parar();
  }

  protected reiniciar(): void {
    this.parar();
    this.restantes.set(this.duracao());
  }

  private tique(): void {
    this.restantes.update((s) => Math.max(0, s - 1));
    if (this.terminou()) {
      this.parar();
      this.terminada.emit();
    }
  }

  private parar(): void {
    clearInterval(this.intervalo);
    this.intervalo = undefined;
    this.rodando.set(false);
  }
}
