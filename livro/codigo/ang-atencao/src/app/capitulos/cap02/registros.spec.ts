import { contarPorTipo, descrever, maisIncerta, Registro } from './registros';

const dia: Registro[] = [
  { tipo: 'fato', descricao: 'o build quebrou às 10h' },
  { tipo: 'interpretacao', descricao: 'a equipe vai achar que fui descuidado', certeza: 0.6 },
  { tipo: 'autocritica', frase: 'eu nunca acerto nada' },
  { tipo: 'interpretacao', descricao: 'foi a dependência atualizada', certeza: 0.3 },
];

describe('registros (cap. 2)', () => {
  it('descreve cada tipo do seu jeito', () => {
    expect(descrever(dia[0])).toBe('Aconteceu: o build quebrou às 10h');
    expect(descrever(dia[1])).toBe('Uma hipótese (60% de certeza): a equipe vai achar que fui descuidado');
    expect(descrever(dia[2])).toBe('Um pensamento, não um veredito: "eu nunca acerto nada"');
  });

  it('conta por tipo, inclusive os que não aparecem', () => {
    expect(contarPorTipo(dia)).toEqual({ fato: 1, interpretacao: 2, autocritica: 1 });
    expect(contarPorTipo([])).toEqual({ fato: 0, interpretacao: 0, autocritica: 0 });
  });

  it('acha a interpretação mais incerta, ou undefined', () => {
    expect(maisIncerta(dia)?.descricao).toBe('foi a dependência atualizada');
    expect(maisIncerta([dia[0]])).toBeUndefined();
  });

  it('um tipo desconhecido em tempo de execução não passa em silêncio', () => {
    const vindoDeFora = { tipo: 'boato', texto: '?' } as unknown as Registro;
    expect(() => descrever(vindoDeFora)).toThrow('Caso não tratado');
  });
});
