import { TestBed } from '@angular/core/testing';
import { duracaoValida, formatarTempo } from './tempo';
import { Temporizador } from './temporizador';

describe('formatarTempo (lab 1)', () => {
  it.each([
    [60, '1:00'],
    [65, '1:05'],
    [9, '0:09'],
    [0, '0:00'],
    [-3, '0:00'],
    [59.2, '1:00'],
  ])('%s s → %s', (s, esperado) => expect(formatarTempo(s)).toBe(esperado));
});

describe('duracaoValida (lab 1)', () => {
  it.each([
    [120, 120],
    ['180', 180],
    [undefined, 60],
    ['', 60],
    ['abc', 60],
    [0, 60],
    [1.5, 60],
    [99999, 60],
  ])('%j → %s', (entrada, esperado) => expect(duracaoValida(entrada)).toBe(esperado));
});

describe('Temporizador (lab 1)', () => {
  // Só o setInterval é falso: o agendador do Angular (zoneless) continua usando os timers de verdade,
  // e fixture.whenStable() funciona normalmente.
  beforeEach(() => vi.useFakeTimers({ toFake: ['setInterval', 'clearInterval'] }));
  afterEach(() => vi.useRealTimers());

  async function criar(duracao = 3) {
    const fixture = TestBed.createComponent(Temporizador);
    fixture.componentRef.setInput('duracao', duracao);
    let terminadas = 0;
    fixture.componentInstance.terminada.subscribe(() => terminadas++);
    await fixture.whenStable();
    const tela = fixture.nativeElement as HTMLElement;
    const botao = (t: string) => [...tela.querySelectorAll('button')].find((b) => b.textContent?.includes(t))!;
    const relogio = () => tela.querySelector('.relogio')!.textContent!.trim();
    return { fixture, tela, botao, relogio, terminadas: () => terminadas };
  }

  it('conta um segundo por tique e avisa uma única vez ao terminar', async () => {
    const t = await criar(3);
    expect(t.relogio()).toBe('0:03');

    t.botao('Começar').click();
    await vi.advanceTimersByTimeAsync(1000);
    await t.fixture.whenStable();
    expect(t.relogio()).toBe('0:02');

    await vi.advanceTimersByTimeAsync(5000);
    await t.fixture.whenStable();
    expect(t.relogio()).toBe('0:00');
    expect(t.terminadas()).toBe(1);
    expect(t.tela.textContent).toContain('Note como você está agora');

    await vi.advanceTimersByTimeAsync(10_000); // se o intervalo tivesse ficado vivo, avisaria de novo
    expect(t.terminadas()).toBe(1);
  });

  it('pausar congela o tempo; continuar retoma de onde parou', async () => {
    const t = await criar(10);
    t.botao('Começar').click();
    await vi.advanceTimersByTimeAsync(2000);
    await t.fixture.whenStable();
    t.botao('Pausar').click();
    await vi.advanceTimersByTimeAsync(5000);
    await t.fixture.whenStable();

    expect(t.relogio()).toBe('0:08');
    expect(t.botao('Continuar')).toBeTruthy();
  });

  it('mudar a duração reinicia a contagem (linkedSignal)', async () => {
    const t = await criar(10);
    t.botao('Começar').click();
    await vi.advanceTimersByTimeAsync(4000);
    t.botao('Pausar').click();
    t.fixture.componentRef.setInput('duracao', 120);
    await t.fixture.whenStable();

    expect(t.relogio()).toBe('2:00');
    expect(t.tela.querySelector('h2')!.textContent).toContain('2 min');
  });

  it('duração ausente (rota sem o valor) não vira NaN na tela', async () => {
    const t = await criar(10);
    t.fixture.componentRef.setInput('duracao', undefined);
    await t.fixture.whenStable();
    expect(t.relogio()).toBe('1:00');
    expect(t.tela.querySelector('progress')!.value).toBe(0);
  });

  it('sair da tela limpa o intervalo', async () => {
    const t = await criar(30);
    t.botao('Começar').click();
    const rodando = vi.getTimerCount();
    t.fixture.destroy();
    expect(vi.getTimerCount()).toBe(rodando - 1);
  });
});
