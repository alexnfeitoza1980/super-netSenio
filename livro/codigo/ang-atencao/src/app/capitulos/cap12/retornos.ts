/**
 * Cap. 12 — a prática que fica não é a sequência perfeita; é a capacidade de VOLTAR.
 * Por isso contamos retornos (recomeços depois de uma pausa), e nunca "dias perdidos".
 */
export interface Retrospecto {
  readonly diasPraticados: number;
  readonly retornos: number;
  readonly ultimaPratica: string | null;
}

const DIA_MS = 86_400_000;
const paraDia = (iso: string) => Math.floor(Date.parse(`${iso}T00:00:00Z`) / DIA_MS);

/** "2026-02-30" passa no Date.parse (vira 2 de março)! Só vale se a ida e volta der a mesma data. */
export function ehDataValida(iso: string): boolean {
  if (!/^\d{4}-\d{2}-\d{2}$/.test(iso)) return false;
  const ms = Date.parse(`${iso}T00:00:00Z`);
  return !Number.isNaN(ms) && new Date(ms).toISOString().slice(0, 10) === iso;
}

/** `datas` no formato AAAA-MM-DD, em qualquer ordem, com repetições. */
export function retrospecto(datas: readonly string[]): Retrospecto {
  const validas = datas.filter(ehDataValida);
  const dias = [...new Set(validas.map(paraDia))].sort((a, b) => a - b);
  let retornos = 0;
  for (let i = 1; i < dias.length; i++) if (dias[i] - dias[i - 1] > 1) retornos++;
  const ultima = dias.at(-1);
  return {
    diasPraticados: dias.length,
    retornos,
    ultimaPratica: ultima === undefined ? null : new Date(ultima * DIA_MS).toISOString().slice(0, 10),
  };
}

export function convite(r: Retrospecto, hojeIso: string): string {
  if (r.ultimaPratica === null) return 'Quando quiser, um minuto já é um começo.';
  const pausa = paraDia(hojeIso) - paraDia(r.ultimaPratica);
  if (pausa <= 0) return 'Você praticou hoje. Bom ter vindo.';
  if (pausa === 1) return 'Ontem você praticou. Hoje também pode ser curtinho.';
  return r.retornos > 0
    ? `Você já voltou ${r.retornos} ${r.retornos === 1 ? 'vez' : 'vezes'} depois de uma pausa. Dá para voltar hoje também.`
    : 'Faz alguns dias. Voltar é a própria prática.';
}
