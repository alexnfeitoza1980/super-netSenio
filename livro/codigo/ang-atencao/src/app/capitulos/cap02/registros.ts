/**
 * Cap. 2 — TypeScript para quem vem do C#.
 * Um "registro" do dia pode ser um fato, uma interpretação ou uma autocrítica.
 * A união discriminada obriga o código a tratar cada caso, e o compilador avisa quando esquecemos um.
 */

// #region tipos
export type Registro =
  | { readonly tipo: 'fato'; readonly descricao: string }
  | { readonly tipo: 'interpretacao'; readonly descricao: string; readonly certeza: number }
  | { readonly tipo: 'autocritica'; readonly frase: string };

export type TipoDeRegistro = Registro['tipo'];

export const TIPOS = ['fato', 'interpretacao', 'autocritica'] as const satisfies readonly TipoDeRegistro[];
// #endregion

// #region descrever
/** Usado no `default` de um switch: só compila se todos os casos foram tratados. */
export function casoEsquecido(valor: never): never {
  throw new Error(`Caso não tratado: ${JSON.stringify(valor)}`);
}

/** Descreve um registro sem julgá-lo. */
export function descrever(r: Registro): string {
  switch (r.tipo) {
    case 'fato':
      return `Aconteceu: ${r.descricao}`;
    case 'interpretacao':
      return `Uma hipótese (${Math.round(r.certeza * 100)}% de certeza): ${r.descricao}`;
    case 'autocritica':
      return `Um pensamento, não um veredito: "${r.frase}"`;
    default:
      return casoEsquecido(r);
  }
}
// #endregion

// #region contar
/** Genéricos + Record: conta quantos registros há de cada tipo, começando todos em zero. */
export function contarPorTipo(registros: readonly Registro[]): Record<TipoDeRegistro, number> {
  const contagem = Object.fromEntries(TIPOS.map((t) => [t, 0])) as Record<TipoDeRegistro, number>;
  for (const r of registros) contagem[r.tipo]++;
  return contagem;
}
// #endregion

// #region incerta
/** Predicado de tipo: depois do filter, o TypeScript sabe que só sobraram interpretações. */
export function ehInterpretacao(r: Registro): r is Extract<Registro, { tipo: 'interpretacao' }> {
  return r.tipo === 'interpretacao';
}

/** A interpretação em que temos MENOS certeza: boa candidata a ser checada. `undefined` se não houver. */
export function maisIncerta(registros: readonly Registro[]) {
  return registros
    .filter(ehInterpretacao)
    .reduce<Extract<Registro, { tipo: 'interpretacao' }> | undefined>(
      (menor, r) => (menor === undefined || r.certeza < menor.certeza ? r : menor),
      undefined,
    );
}
// #endregion
