import { computed, signal } from '@angular/core';

/**
 * Cap. 10 — desfazer e refazer, com um passado de tamanho limitado:
 * lembrar ajuda, carregar tudo para sempre pesa.
 */
export class Historico<T> {
  private readonly passado = signal<readonly T[]>([]);
  private readonly futuro = signal<readonly T[]>([]);
  readonly presente;

  readonly podeDesfazer = computed(() => this.passado().length > 0);
  readonly podeRefazer = computed(() => this.futuro().length > 0);

  constructor(
    inicial: T,
    private readonly limite = 20,
  ) {
    if (limite < 1) throw new RangeError('O limite precisa ser pelo menos 1.');
    this.presente = signal(inicial);
  }

  registrar(novo: T): void {
    if (Object.is(novo, this.presente())) return;
    this.passado.update((p) => [...p, this.presente()].slice(-this.limite));
    this.futuro.set([]);
    this.presente.set(novo);
  }

  desfazer(): void {
    const p = this.passado();
    if (!p.length) return;
    this.futuro.update((f) => [this.presente(), ...f]);
    this.presente.set(p[p.length - 1]);
    this.passado.set(p.slice(0, -1));
  }

  refazer(): void {
    const [proximo, ...resto] = this.futuro();
    if (proximo === undefined) return;
    this.passado.update((p) => [...p, this.presente()].slice(-this.limite));
    this.presente.set(proximo);
    this.futuro.set(resto);
  }
}
