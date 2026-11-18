/** Lab 2 — o que o Diário guarda. */
export const PRATICAS = ['respiração', 'exploração do corpo', 'movimento', 'sons e pensamentos', 'pausa de três passos', 'outra'] as const;
export type Pratica = (typeof PRATICAS)[number];

export interface Entrada {
  readonly id: string;
  readonly data: string; // AAAA-MM-DD
  readonly pratica: Pratica;
  readonly minutos: number;
  readonly palavra: string;
  readonly nota: string;
}

export type NovaEntrada = Omit<Entrada, 'id'>;

/** Dados do armazenamento podem estar velhos, editados à mão ou corrompidos: conferimos campo a campo. */
export function ehEntrada(x: unknown): x is Entrada {
  if (typeof x !== 'object' || x === null) return false;
  const e = x as Record<string, unknown>;
  return (
    typeof e['id'] === 'string' &&
    typeof e['data'] === 'string' &&
    /^\d{4}-\d{2}-\d{2}$/.test(e['data']) &&
    typeof e['pratica'] === 'string' &&
    (PRATICAS as readonly string[]).includes(e['pratica']) &&
    typeof e['minutos'] === 'number' &&
    Number.isInteger(e['minutos']) &&
    e['minutos'] > 0 &&
    typeof e['palavra'] === 'string' &&
    typeof e['nota'] === 'string'
  );
}
