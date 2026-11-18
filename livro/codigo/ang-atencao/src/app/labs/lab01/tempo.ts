/** Lab 1 — formata segundos como m:ss (ex.: 65 → "1:05"). Função pura: fácil de testar. */
export function formatarTempo(segundos: number): string {
  const s = Math.max(0, Math.ceil(segundos));
  const min = Math.floor(s / 60);
  return `${min}:${String(s % 60).padStart(2, '0')}`;
}

/**
 * Lab 1 — a duração pode chegar de qualquer lugar: de um input (número), da URL (?duracao=120, texto)
 * ou de lugar nenhum (withComponentInputBinding entrega `undefined` quando a rota não tem o valor).
 * Tudo que não for um número inteiro de 1 a 3600 vira o padrão de 60 s.
 */
export function duracaoValida(valor: unknown): number {
  const n = typeof valor === 'string' && valor.trim() !== '' ? Number(valor) : valor;
  return typeof n === 'number' && Number.isInteger(n) && n >= 1 && n <= 3600 ? n : 60;
}
