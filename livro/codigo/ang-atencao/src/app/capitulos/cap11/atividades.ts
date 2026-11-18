/** Cap. 11 — o que nutre e o que desgasta. Quem classifica é você; o código só soma. */
export type Efeito = 'nutre' | 'desgasta' | 'neutro';

export interface Atividade {
  readonly id: number;
  readonly nome: string;
  readonly minutos: number;
  readonly efeito: Efeito;
}

export interface Resumo {
  readonly nutre: number;
  readonly desgasta: number;
  readonly neutro: number;
  readonly proporcaoQueNutre: number;
}

export function resumir(atividades: readonly Atividade[]): Resumo {
  const soma = { nutre: 0, desgasta: 0, neutro: 0 };
  for (const a of atividades) soma[a.efeito] += Math.max(0, a.minutos);
  const comEfeito = soma.nutre + soma.desgasta;
  return { ...soma, proporcaoQueNutre: comEfeito ? soma.nutre / comEfeito : 0 };
}
