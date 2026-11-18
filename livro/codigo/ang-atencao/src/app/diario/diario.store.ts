import { computed, inject, Injectable, signal } from '@angular/core';
import { RELOGIO } from '../capitulos/cap08/tokens';
import { retrospecto } from '../capitulos/cap12/retornos';
import { Armazenamento } from './armazenamento';
import { ehEntrada, Entrada, NovaEntrada, Pratica } from './entrada';

export const CHAVE = 'diario-de-atencao.v1';

interface Arquivo {
  readonly versao: 1;
  readonly entradas: readonly Entrada[];
}

/**
 * Lab 2 — o estado do Diário: uma fonte (entradas), muitas consequências (computed).
 * Gravar é explícito, dentro dos métodos que mudam o estado (e não num effect): fica claro QUANDO grava.
 */
@Injectable({ providedIn: 'root' })
export class DiarioStore {
  private readonly armazenamento = inject(Armazenamento);
  private readonly agora = inject(RELOGIO);
  /** Avisa a tela quando os dados guardados não puderam ser lidos (sem apagar nada). Vem antes de
   *  `_entradas` porque os campos são iniciados na ordem em que aparecem, e carregar() o usa. */
  readonly avisoDeCarga = signal<string | null>(null);
  private readonly _entradas = signal<readonly Entrada[]>(this.carregar());

  /** Lista somente leitura, do mais recente para o mais antigo. */
  readonly entradas = computed(() =>
    [...this._entradas()].sort((a, b) => b.data.localeCompare(a.data) || b.id.localeCompare(a.id)),
  );
  readonly hoje = computed(() => this.agora().toISOString().slice(0, 10));
  readonly deHoje = computed(() => this.entradas().filter((e) => e.data === this.hoje()));
  readonly minutosTotais = computed(() => this._entradas().reduce((s, e) => s + e.minutos, 0));
  readonly resumo = computed(() => retrospecto(this._entradas().map((e) => e.data)));
  readonly porPratica = computed(() => {
    const contagem = new Map<Pratica, number>();
    for (const e of this._entradas()) contagem.set(e.pratica, (contagem.get(e.pratica) ?? 0) + e.minutos);
    return [...contagem].sort((a, b) => b[1] - a[1]);
  });
  porId(id: string): Entrada | undefined {
    return this._entradas().find((e) => e.id === id);
  }

  adicionar(nova: NovaEntrada): Entrada {
    const entrada: Entrada = { ...nova, palavra: nova.palavra.trim(), nota: nova.nota.trim(), id: crypto.randomUUID() };
    if (!ehEntrada(entrada)) throw new RangeError('Entrada inválida: confira data, prática e minutos.');
    this._entradas.update((l) => [...l, entrada]);
    this.persistir();
    return entrada;
  }

  remover(id: string): boolean {
    const antes = this._entradas().length;
    this._entradas.update((l) => l.filter((e) => e.id !== id));
    const removeu = this._entradas().length < antes;
    if (removeu) this.persistir();
    return removeu;
  }

  private carregar(): readonly Entrada[] {
    const bruto = this.armazenamento.ler(CHAVE);
    if (bruto === null) return [];
    try {
      const arquivo = JSON.parse(bruto) as Partial<Arquivo>;
      if (arquivo.versao !== 1 || !Array.isArray(arquivo.entradas)) throw new Error('formato desconhecido');
      const validas = arquivo.entradas.filter(ehEntrada);
      if (validas.length < arquivo.entradas.length) {
        this.avisoDeCarga.set(`${arquivo.entradas.length - validas.length} registro(s) ilegível(is) foram ignorados.`);
      }
      return validas;
    } catch {
      // Não sobrescrevemos o que não entendemos: guardamos uma cópia antes de seguir.
      this.armazenamento.gravar(`${CHAVE}.ilegivel-${Date.now()}`, bruto);
      this.avisoDeCarga.set('Não consegui ler o diário guardado. Uma cópia foi preservada.');
      return [];
    }
  }

  private persistir(): void {
    const arquivo: Arquivo = { versao: 1, entradas: this._entradas() };
    this.armazenamento.gravar(CHAVE, JSON.stringify(arquivo));
  }
}
