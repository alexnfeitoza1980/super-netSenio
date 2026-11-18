import { TestBed } from '@angular/core/testing';
import { Historico } from './historico';
import { RegistroGentil } from './registro-gentil';

// #region historico
describe('Historico (cap. 10)', () => {
  it('desfaz e refaz na ordem certa', () => {
    const h = new Historico('a');
    h.registrar('b');
    h.registrar('c');
    h.desfazer();
    expect(h.presente()).toBe('b');
    h.desfazer();
    expect(h.presente()).toBe('a');
    expect(h.podeDesfazer()).toBe(false);
    h.refazer();
    expect(h.presente()).toBe('b');
  });

  it('uma mudança nova apaga o futuro', () => {
    const h = new Historico(1);
    h.registrar(2);
    h.desfazer();
    h.registrar(3);
    expect(h.podeRefazer()).toBe(false);
  });

  it('guarda no máximo o limite de estados passados', () => {
    const h = new Historico(0, 3);
    for (let i = 1; i <= 10; i++) h.registrar(i);
    let voltas = 0;
    while (h.podeDesfazer()) {
      h.desfazer();
      voltas++;
    }
    expect(voltas).toBe(3);
    expect(h.presente()).toBe(7);
  });

  it('registrar o mesmo valor não cria passo', () => {
    const h = new Historico('x');
    h.registrar('x');
    expect(h.podeDesfazer()).toBe(false);
  });

  it('limite inválido é recusado logo', () => {
    expect(() => new Historico('x', 0)).toThrow(RangeError);
  });
});
// #endregion

// #region formulario
describe('RegistroGentil com Signal Forms (cap. 10)', () => {
  async function criar() {
    const fixture = TestBed.createComponent(RegistroGentil);
    await fixture.whenStable();
    const tela = fixture.nativeElement as HTMLElement;
    const digitar = async (seletor: string, valor: string) => {
      const campo = tela.querySelector<HTMLInputElement | HTMLTextAreaElement>(seletor)!;
      campo.value = valor;
      campo.dispatchEvent(new Event('input'));
      campo.dispatchEvent(new Event('blur'));
      await fixture.whenStable();
    };
    const enviar = async () => {
      tela.querySelector('form')!.dispatchEvent(new Event('submit', { cancelable: true }));
      await fixture.whenStable();
    };
    return { fixture, tela, digitar, enviar };
  }

  it('não mostra erros antes de a pessoa tocar no campo', async () => {
    const { tela } = await criar();
    expect(tela.querySelectorAll('.erro').length).toBe(0);
  });

  it('enviar vazio marca os campos e mostra a mensagem gentil', async () => {
    const { tela, enviar } = await criar();
    await enviar();
    expect(tela.textContent).toContain('qualquer uma serve');
    expect(tela.querySelector('#palavra')!.getAttribute('aria-invalid')).toBe('true');
    expect(tela.querySelector('h3')).toBeNull();
  });

  it('minutos fora da faixa mostram a regra certa', async () => {
    const { tela, digitar } = await criar();
    await digitar('#minutos', '0');
    expect(tela.textContent).toContain('Mesmo um minuto conta');
    await digitar('#minutos', '500');
    expect(tela.textContent).toContain('Até 120 minutos');
  });

  it('registro válido é guardado e o formulário volta ao início', async () => {
    const { tela, digitar, enviar } = await criar();
    await digitar('#palavra', '  curioso ');
    await digitar('#minutos', '7');
    await enviar();

    expect(tela.querySelector('li')!.textContent).toBe('curioso · 7 min');
    expect(tela.querySelector<HTMLInputElement>('#palavra')!.value).toBe('');
    expect(tela.querySelectorAll('.erro').length).toBe(0);
  });
});
// #endregion
