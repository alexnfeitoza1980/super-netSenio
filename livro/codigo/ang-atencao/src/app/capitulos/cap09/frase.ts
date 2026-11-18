/** Cap. 9 — o formato que esperamos do servidor. */
export interface Frase {
  readonly id: number;
  readonly texto: string;
  readonly tema: string;
}

/** Dados que chegam da rede são boatos até serem conferidos. */
export function ehListaDeFrases(dado: unknown): dado is Frase[] {
  return (
    Array.isArray(dado) &&
    dado.every(
      (f) => typeof f === 'object' && f !== null && typeof f.id === 'number' && typeof f.texto === 'string' && typeof f.tema === 'string',
    )
  );
}
