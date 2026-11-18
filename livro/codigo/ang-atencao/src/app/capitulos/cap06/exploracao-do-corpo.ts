import { Component, computed, input, signal } from '@angular/core';

export type Sensacao = 'tensa' | 'neutra' | 'relaxada' | 'nao-sei';

export interface Regiao {
  readonly id: string;
  readonly nome: string;
}

export const REGIOES_PADRAO: readonly Regiao[] = [
  { id: 'pes', nome: 'Pés' },
  { id: 'pernas', nome: 'Pernas' },
  { id: 'barriga', nome: 'Barriga' },
  { id: 'ombros', nome: 'Ombros' },
  { id: 'mandibula', nome: 'Mandíbula' },
];

/**
 * Cap. 6 — percorrer o corpo como quem percorre um template:
 * @for com track e @empty, @if/@else, @switch, @let, bindings de classe, atributo e evento,
 * variável de template (#nota) e cuidados de acessibilidade (rótulos, aria-pressed, aria-live).
 */
@Component({
  selector: 'app-exploracao-do-corpo',
  template: `
    <h2 id="titulo-corpo">Exploração do corpo</h2>
    <ul aria-labelledby="titulo-corpo">
      @for (r of regioes(); track r.id; let i = $index) {
        <li>
          <button type="button" (click)="selecionada.set(r.id)" [attr.aria-pressed]="r.id === selecionada()">
            {{ i + 1 }}. {{ r.nome }}
          </button>
          @if (sensacoes()[r.id]; as s) {
            <span class="marca" [class]="s">{{ rotulo(s) }}</span>
          }
        </li>
      } @empty {
        <li>Nenhuma região para explorar agora. Tudo bem.</li>
      }
    </ul>

    @let atual = regiaoAtual();
    @if (atual) {
      <fieldset>
        <legend>Como estão {{ atual.nome.toLowerCase() }}?</legend>
        @for (s of opcoes; track s) {
          <label>
            <input type="radio" name="sensacao" [value]="s" [checked]="sensacoes()[atual.id] === s" (change)="anotar(atual.id, s)" />
            {{ rotulo(s) }}
          </label>
        }
      </fieldset>

      <p aria-live="polite">
        @switch (sensacoes()[atual.id]) {
          @case ('tensa') { Só note a tensão. Não precisa soltar à força. }
          @case ('relaxada') { Note como é estar assim. }
          @case ('nao-sei') { "Não sei" também é uma observação válida. }
          @default { Perceba sem pressa. }
        }
      </p>

      <label for="nota">Uma palavra sobre essa região (Enter para guardar)</label>
      <input id="nota" #nota (keyup.enter)="guardarPalavra(atual.id, nota.value); nota.value = ''" />
    } @else {
      <p>Escolha uma região para começar.</p>
    }

    @if (palavras().length) {
      <p>Palavras: {{ palavras().join(', ') }}</p>
    }
  `,
  styles: `
    .marca { margin-left: .5em; font-size: .85em; padding: 0 .5em; border-radius: 999px; background: var(--suave, #eee); }
    .marca.tensa { background: #ffe0b2; color: #6d3a00; }
    .marca.relaxada { background: #c8e6c9; color: #1b4d1e; }
  `,
})
export class ExploracaoDoCorpo {
  readonly regioes = input<readonly Regiao[]>(REGIOES_PADRAO);

  protected readonly opcoes: readonly Sensacao[] = ['tensa', 'neutra', 'relaxada', 'nao-sei'];
  protected readonly selecionada = signal<string | null>(null);
  protected readonly sensacoes = signal<Readonly<Record<string, Sensacao>>>({});
  protected readonly palavras = signal<readonly string[]>([]);

  protected readonly regiaoAtual = computed(() => this.regioes().find((r) => r.id === this.selecionada()));

  protected rotulo(s: Sensacao): string {
    return { tensa: 'Tensa', neutra: 'Neutra', relaxada: 'Relaxada', 'nao-sei': 'Não sei' }[s];
  }

  protected anotar(id: string, s: Sensacao): void {
    this.sensacoes.update((atual) => ({ ...atual, [id]: s }));
  }

  protected guardarPalavra(id: string, texto: string): void {
    const palavra = texto.trim();
    if (palavra) this.palavras.update((ps) => [...ps, `${id}: ${palavra}`]);
  }
}
