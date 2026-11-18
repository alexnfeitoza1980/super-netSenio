import { TestBed } from '@angular/core/testing';
import { ExploracaoDoCorpo } from './exploracao-do-corpo';

describe('ExploracaoDoCorpo (cap. 6)', () => {
  async function criar() {
    const fixture = TestBed.createComponent(ExploracaoDoCorpo);
    await fixture.whenStable();
    const tela = fixture.nativeElement as HTMLElement;
    return { fixture, tela, estavel: () => fixture.whenStable() };
  }

  it('lista as regiões e pede uma escolha (@for e @else)', async () => {
    const { tela } = await criar();
    expect(tela.querySelectorAll('ul button').length).toBe(5);
    expect(tela.textContent).toContain('Escolha uma região para começar.');
  });

  it('selecionar marca aria-pressed e mostra o fieldset com legenda (@let)', async () => {
    const { tela, estavel } = await criar();
    const ombros = tela.querySelectorAll<HTMLButtonElement>('ul button')[3];
    ombros.click();
    await estavel();

    expect(ombros.getAttribute('aria-pressed')).toBe('true');
    expect(tela.querySelector('legend')!.textContent).toContain('Como estão ombros?');
  });

  it('anotar uma sensação muda a mensagem (@switch) e a marca (@if ... as)', async () => {
    const { tela, estavel } = await criar();
    tela.querySelectorAll<HTMLButtonElement>('ul button')[0].click();
    await estavel();
    tela.querySelector<HTMLInputElement>('input[value="tensa"]')!.click();
    await estavel();

    expect(tela.textContent).toContain('Só note a tensão');
    expect(tela.querySelector('.marca.tensa')!.textContent).toBe('Tensa');
  });

  it('Enter no campo guarda a palavra e limpa o campo (#nota)', async () => {
    const { tela, estavel } = await criar();
    tela.querySelectorAll<HTMLButtonElement>('ul button')[4].click();
    await estavel();
    const campo = tela.querySelector<HTMLInputElement>('#nota')!;
    campo.value = 'apertada';
    campo.dispatchEvent(new KeyboardEvent('keyup', { key: 'Enter' }));
    await estavel();

    expect(tela.textContent).toContain('Palavras: mandibula: apertada');
    expect(campo.value).toBe('');
  });

  it('lista vazia mostra a mensagem de @empty', async () => {
    const { fixture, tela, estavel } = await criar();
    fixture.componentRef.setInput('regioes', []);
    await estavel();
    expect(tela.textContent).toContain('Nenhuma região para explorar agora');
  });

  it('acessibilidade: todo campo tem rótulo e todo botão tem nome', async () => {
    const { tela, estavel } = await criar();
    tela.querySelector<HTMLButtonElement>('ul button')!.click();
    await estavel();

    for (const b of tela.querySelectorAll('button')) expect(b.textContent!.trim()).not.toBe('');
    for (const campo of tela.querySelectorAll('input')) {
      const rotulado = campo.closest('label') || (campo.id && tela.querySelector(`label[for="${campo.id}"]`));
      expect(rotulado, `campo sem rótulo: ${campo.outerHTML}`).toBeTruthy();
    }
  });
});
