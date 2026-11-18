import { DeferBlockBehavior, DeferBlockState, TestBed } from '@angular/core/testing';
import { resumir } from './atividades';
import { Balanco } from './balanco';
import { MinutosPipe } from './minutos.pipe';

describe('MinutosPipe (cap. 11)', () => {
  const pipe = new MinutosPipe();
  it.each([
    [0, '0 min'],
    [-5, '0 min'],
    [45, '45 min'],
    [60, '1 h'],
    [95, '1 h 35 min'],
    [Number.NaN, '0 min'],
  ])('%s → %s', (n, esperado) => expect(pipe.transform(n)).toBe(esperado));
});

describe('resumir (cap. 11)', () => {
  it('soma por efeito e calcula a proporção só entre o que nutre e o que desgasta', () => {
    const r = resumir([
      { id: 1, nome: 'a', minutos: 30, efeito: 'nutre' },
      { id: 2, nome: 'b', minutos: 90, efeito: 'desgasta' },
      { id: 3, nome: 'c', minutos: 100, efeito: 'neutro' },
    ]);
    expect(r).toEqual({ nutre: 30, desgasta: 90, neutro: 100, proporcaoQueNutre: 0.25 });
    expect(resumir([]).proporcaoQueNutre).toBe(0);
  });
});

describe('Balanco (cap. 11)', () => {
  beforeEach(() => TestBed.configureTestingModule({ deferBlockBehavior: DeferBlockBehavior.Manual }));

  it('mostra o placeholder e só depois o gráfico (@defer)', async () => {
    const fixture = TestBed.createComponent(Balanco);
    await fixture.whenStable();
    const tela = fixture.nativeElement as HTMLElement;
    expect(tela.textContent).toContain('O gráfico aparece quando você chegar aqui.');
    expect(tela.querySelector('app-grafico-do-dia')).toBeNull();

    const [bloco] = await fixture.getDeferBlocks();
    await bloco.render(DeferBlockState.Complete);

    expect(tela.querySelector('svg')!.getAttribute('aria-label')).toBe('47% do tempo classificado foi em atividades que nutrem');
  });

  it('reclassificar uma atividade atualiza o resumo e o convite', async () => {
    const fixture = TestBed.createComponent(Balanco);
    await fixture.whenStable();
    const tela = fixture.nativeElement as HTMLElement;
    expect(tela.textContent).toContain('Hoje pesou mais do que nutriu');

    const select = tela.querySelector<HTMLSelectElement>('select[aria-label="Efeito de Reunião longa"]')!;
    select.value = 'neutro';
    select.dispatchEvent(new Event('change'));
    await fixture.whenStable();

    expect(tela.textContent).toContain('Nutre: 1 h 20 min · Desgasta: 0 min');
    expect(tela.textContent).not.toContain('Hoje pesou mais');
  });

  it('track por id: itens que não mudaram são os MESMOS elementos no DOM', async () => {
    const fixture = TestBed.createComponent(Balanco);
    await fixture.whenStable();
    const tela = fixture.nativeElement as HTMLElement;
    const antes = [...tela.querySelectorAll('li')];

    fixture.componentInstance.atividades.update((l) => [...l.slice(0, 3), { ...l[3], minutos: 25 }]);
    await fixture.whenStable();
    const depois = [...tela.querySelectorAll('li')];

    expect(depois[0]).toBe(antes[0]);
    expect(depois[3].textContent).toContain('25 min');
  });
});
