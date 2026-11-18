import { TestBed } from '@angular/core/testing';
import { CartaoDaSemana } from './cartao-da-semana';
import { Programa } from './programa';
import { SEMANAS } from './semanas';

describe('CartaoDaSemana (cap. 4)', () => {
  it('mostra a semana recebida por input e emite ao concluir', async () => {
    const fixture = TestBed.createComponent(CartaoDaSemana);
    fixture.componentRef.setInput('semana', SEMANAS[1]);
    const emitidos: number[] = [];
    fixture.componentInstance.concluiu.subscribe((n) => emitidos.push(n));
    await fixture.whenStable();
    const tela = fixture.nativeElement as HTMLElement;

    expect(tela.querySelector('h3')?.textContent).toContain('Semana 2 · Voltar ao corpo');

    tela.querySelector('input')!.click(); // marca
    tela.querySelector('input')!.click(); // desmarca: não emite de novo
    expect(emitidos).toEqual([2]);
    expect(fixture.componentInstance.feita()).toBe(false);
  });
});

describe('Programa (cap. 4)', () => {
  it('compõe oito cartões, projeta o conteúdo e conta as semanas feitas', async () => {
    const fixture = TestBed.createComponent(Programa);
    await fixture.whenStable();
    const tela = fixture.nativeElement as HTMLElement;

    expect(tela.querySelectorAll('app-cartao-da-semana').length).toBe(8);
    expect(tela.querySelector('small')?.textContent).toContain('cap. 5');

    const caixas = tela.querySelectorAll<HTMLInputElement>('input[type=checkbox]');
    caixas[0].click();
    caixas[3].click();
    await fixture.whenStable();

    expect(tela.textContent).toContain('2 de 8 semanas praticadas');
    expect(tela.textContent).toContain('Última semana marcada: 4');
    expect(fixture.componentInstance.completo()).toBe(false);
  });
});
