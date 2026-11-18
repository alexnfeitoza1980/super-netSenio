import { TestBed } from '@angular/core/testing';
import { PainelDaSessao } from './painel-da-sessao';

describe('PainelDaSessao (cap. 3)', () => {
  it('o template acompanha os signals sem nenhuma atualização manual', async () => {
    const info = vi.spyOn(console, 'info').mockImplementation(() => {});
    const fixture = TestBed.createComponent(PainelDaSessao);
    await fixture.whenStable();
    const tela = fixture.nativeElement as HTMLElement;
    const [mais, menos] = tela.querySelectorAll('button');

    expect(tela.textContent).toContain('Faltam 10 min.');

    for (let i = 0; i < 8; i++) mais.click();
    menos.click();
    menos.click();
    await fixture.whenStable();

    expect(tela.textContent).toContain('Sessão concluída');
    expect(info).toHaveBeenCalledWith('[diário] sessão de 8 min concluída');
    info.mockRestore();
  });
});
