/**
 * Cap. 8 — "rotular" um pensamento é uma prática simples: notar o TIPO dele, sem discutir o conteúdo.
 * Isto é um brinquedo de palavras-chave para treinar RxJS; não interpreta pessoas.
 */
export type Rotulo = 'planejando' | 'lembrando' | 'preocupando' | 'julgando' | 'outro';

/** Palavra inteira, com acentos: o \b do JavaScript não entende "ã" como letra. */
const palavras = (...lista: string[]) => new RegExp(`(?<!\\p{L})(${lista.join('|')})(?!\\p{L})`, 'iu');

const PISTAS: ReadonlyArray<readonly [Rotulo, RegExp]> = [
  ['preocupando', palavras('e se', 'vai dar errado', 'medo', 'preocupad[oa]', 'preocupa')],
  ['julgando', palavras('sempre', 'nunca', 'deveria', 'burr[oa]', 'péssim[oa]')],
  ['planejando', palavras('amanhã', 'depois', 'preciso', 'vou', 'tenho que')],
  ['lembrando', palavras('ontem', 'lembro', 'lembrei', 'naquele dia', 'antes')],
];

export function classificar(texto: string): Rotulo {
  return PISTAS.find(([, padrao]) => padrao.test(texto))?.[0] ?? 'outro';
}
