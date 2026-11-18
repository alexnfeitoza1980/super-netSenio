import { TestBed } from '@angular/core/testing';
import { Pausa } from './pausa';

describe('Pausa (cap. 1)', () => {
  async function criar() {
    const fixture = TestBed.createComponent(Pausa);
    await fixture.whenStable();
    const tela = fixture.nativeElement as HTMLElement;
    const botao = (texto: string) =>
      [...tela.querySelectorAll('button')].find((b) => b.textContent?.includes(texto))!;
    return { fixture, tela, botao };
  }

  it('mostra os cinco passos e começa no primeiro', async () => {
    const { tela } = await criar();
    expect(tela.querySelectorAll('li').length).toBe(5);
    expect(tela.querySelector('li.atual')?.textContent).toContain('Tire as mãos');
    expect(tela.textContent).toContain('Passo 1 de 5');
  });

  it('avança um passo por clique e para no último', async () => {
    const { fixture, tela, botao } = await criar();
    for (let i = 0; i < 10; i++) botao('Próximo').click();
    await fixture.whenStable();

    expect(tela.textContent).toContain('Passo 5 de 5');
    expect(botao('Próximo').disabled).toBe(true);
  });

  it('recomeçar volta ao primeiro passo', async () => {
    const { fixture, tela, botao } = await criar();
    botao('Próximo').click();
    botao('Recomeçar').click();
    await fixture.whenStable();

    expect(tela.textContent).toContain('Passo 1 de 5');
  });
});
