import { TestBed } from '@angular/core/testing';
import { RELOGIO } from '../cap08/tokens';
import { LembreteDePratica } from './lembrete';
import { convite, retrospecto } from './retornos';

/*
 * Cap. 12 — um arquivo de testes que serve de modelo:
 * 1. nomes que são frases; 2. Arrange / Act / Assert visíveis; 3. tabelas com it.each;
 * 4. dependências trocadas por injeção (o relógio); 5. um comportamento por teste.
 */

describe('retrospecto', () => {
  it('conta dias distintos, ignora datas inválidas e acha a última', () => {
    // Arrange
    const datas = ['2026-11-03', '2026-11-01', '2026-11-03', 'ontem', '2026-02-30', '2026-11-02'];
    // Act
    const r = retrospecto(datas);
    // Assert
    expect(r).toEqual({ diasPraticados: 3, retornos: 0, ultimaPratica: '2026-11-03' });
  });

  it('cada recomeço depois de pelo menos um dia sem prática conta como retorno', () => {
    const r = retrospecto(['2026-11-01', '2026-11-02', '2026-11-05', '2026-11-09', '2026-11-10']);
    expect(r.retornos).toBe(2);
  });

  it('sem datas, sem última prática', () => {
    expect(retrospecto([])).toEqual({ diasPraticados: 0, retornos: 0, ultimaPratica: null });
  });
});

describe('convite', () => {
  it.each([
    [[], '2026-11-10', 'um minuto já é um começo'],
    [['2026-11-10'], '2026-11-10', 'praticou hoje'],
    [['2026-11-09'], '2026-11-10', 'Ontem você praticou'],
    [['2026-11-01'], '2026-11-10', 'Voltar é a própria prática'],
    [['2026-11-01', '2026-11-05'], '2026-11-10', 'já voltou 1 vez'],
  ])('datas %j, hoje %s → "%s"', (datas, hoje, trecho) => {
    expect(convite(retrospecto(datas), hoje)).toContain(trecho);
  });

  it('nunca usa palavras de culpa', () => {
    for (const dias of [2, 10, 100]) {
      const ultima = new Date(Date.UTC(2026, 0, 1) - dias * 86_400_000).toISOString().slice(0, 10);
      expect(convite(retrospecto([ultima]), '2026-01-01')).not.toMatch(/perdeu|falhou|quebrou|deveria/i);
    }
  });
});

describe('LembreteDePratica', () => {
  function criar(agoraIso: string) {
    const relogio = vi.fn(() => new Date(agoraIso));
    TestBed.configureTestingModule({ providers: [{ provide: RELOGIO, useValue: relogio }] });
    return { servico: TestBed.inject(LembreteDePratica), relogio };
  }

  it('registrar hoje usa o relógio injetado e não duplica', () => {
    const { servico, relogio } = criar('2026-11-10T21:00:00Z');
    servico.registrarHoje();
    servico.registrarHoje();
    expect(servico.datas()).toEqual(['2026-11-10']);
    expect(relogio).toHaveBeenCalled();
    expect(servico.mensagem()).toContain('praticou hoje');
  });
});
