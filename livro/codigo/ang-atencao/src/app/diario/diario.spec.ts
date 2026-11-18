import { TestBed } from '@angular/core/testing';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { RouterTestingHarness } from '@angular/router/testing';
import { RELOGIO } from '../capitulos/cap08/tokens';
import { Armazenamento, ArmazenamentoEmMemoria } from './armazenamento';
import { DIARIO_ROTAS } from './diario.routes';
import { CHAVE, DiarioStore } from './diario.store';
import { NovaEntrada } from './entrada';

const HOJE = '2026-11-10';
const nova = (parcial: Partial<NovaEntrada> = {}): NovaEntrada => ({
  data: HOJE, pratica: 'respiração', minutos: 5, palavra: 'calmo', nota: '', ...parcial,
});

function configurar(armazenamento = new ArmazenamentoEmMemoria()) {
  TestBed.configureTestingModule({
    providers: [
      { provide: Armazenamento, useValue: armazenamento },
      { provide: RELOGIO, useValue: () => new Date(`${HOJE}T12:00:00Z`) },
      provideRouter([{ path: 'diario', children: DIARIO_ROTAS }], withComponentInputBinding()),
    ],
  });
  return armazenamento;
}

describe('DiarioStore (lab 2)', () => {
  it('adiciona, ordena do mais recente e grava no armazenamento', () => {
    const mem = configurar();
    const store = TestBed.inject(DiarioStore);
    store.adicionar(nova({ data: '2026-11-08', palavra: ' antigo ' }));
    store.adicionar(nova({ minutos: 10 }));

    expect(store.entradas().map((e) => e.data)).toEqual([HOJE, '2026-11-08']);
    expect(store.entradas()[1].palavra).toBe('antigo');
    expect(store.deHoje().length).toBe(1);
    expect(store.minutosTotais()).toBe(15);
    expect(JSON.parse(mem.ler(CHAVE)!).entradas.length).toBe(2);
  });

  it('recusa entrada inválida sem mexer no estado', () => {
    configurar();
    const store = TestBed.inject(DiarioStore);
    expect(() => store.adicionar(nova({ minutos: 0 }))).toThrow(RangeError);
    expect(() => store.adicionar(nova({ pratica: 'levitação' as never }))).toThrow(RangeError);
    expect(store.entradas()).toEqual([]);
  });

  it('remove por id e diz se removeu', () => {
    configurar();
    const store = TestBed.inject(DiarioStore);
    const e = store.adicionar(nova());
    expect(store.remover('nao-existe')).toBe(false);
    expect(store.remover(e.id)).toBe(true);
    expect(store.porId(e.id)).toBeUndefined();
  });

  it('dados corrompidos: começa vazio, avisa e PRESERVA uma cópia', () => {
    const mem = new ArmazenamentoEmMemoria();
    mem.gravar(CHAVE, '{isto não é json');
    configurar(mem);
    const store = TestBed.inject(DiarioStore);

    expect(store.entradas()).toEqual([]);
    expect(store.avisoDeCarga()).toContain('Uma cópia foi preservada');
    expect([...mem.dados.keys()].some((k) => k.startsWith(`${CHAVE}.ilegivel-`))).toBe(true);
  });

  it('registros ilegíveis no meio de válidos são ignorados, e os válidos ficam', () => {
    const mem = new ArmazenamentoEmMemoria();
    const valida = { id: 'a', ...nova() };
    mem.gravar(CHAVE, JSON.stringify({ versao: 1, entradas: [valida, { id: 'b', minutos: 'dez' }] }));
    configurar(mem);
    const store = TestBed.inject(DiarioStore);

    expect(store.entradas()).toEqual([valida]);
    expect(store.avisoDeCarga()).toBe('1 registro(s) ilegível(is) foram ignorados.');
  });
});

describe('páginas do Diário (lab 2)', () => {
  it('registrar pela página Hoje mostra o item na lista de hoje', async () => {
    configurar();
    const harness = await RouterTestingHarness.create('/diario/hoje');
    const tela = harness.routeNativeElement!;
    expect(tela.textContent).toContain('Nada ainda. Tudo bem.');

    const palavra = tela.querySelector<HTMLInputElement>('#palavra')!;
    palavra.value = 'presente';
    palavra.dispatchEvent(new Event('input'));
    tela.querySelector('form')!.dispatchEvent(new Event('submit', { cancelable: true }));
    await harness.fixture.whenStable();

    expect(tela.querySelector('ul li a')!.textContent).toBe('respiração · 5 min · presente');
  });

  it('o histórico agrupa por dia e filtra por prática', async () => {
    configurar();
    const store = TestBed.inject(DiarioStore);
    store.adicionar(nova({ data: '2026-11-09', pratica: 'movimento', minutos: 20 }));
    store.adicionar(nova({ data: '2026-11-09', minutos: 10 }));
    store.adicionar(nova());
    const harness = await RouterTestingHarness.create('/diario/historico');
    const tela = harness.routeNativeElement!;

    expect([...tela.querySelectorAll('h2')].map((h) => h.textContent!.trim())).toEqual([`${HOJE} (5 min)`, '2026-11-09 (30 min)']);

    const filtro = tela.querySelector<HTMLSelectElement>('#filtro')!;
    filtro.value = 'movimento';
    filtro.dispatchEvent(new Event('change'));
    await harness.fixture.whenStable();
    expect(tela.querySelectorAll('li').length).toBe(1);
  });

  it('detalhe: mostra o registro pelo :id e apagar volta ao histórico', async () => {
    configurar();
    const e = TestBed.inject(DiarioStore).adicionar(nova({ nota: 'ombros soltaram' }));
    const harness = await RouterTestingHarness.create(`/diario/registro/${e.id}`);
    expect(harness.routeNativeElement!.textContent).toContain('ombros soltaram');

    harness.routeNativeElement!.querySelector('button')!.click();
    await harness.fixture.whenStable();
    expect(harness.routeNativeElement!.querySelector('h1')!.textContent).toBe('Histórico');
  });

  it('detalhe de um id que não existe não quebra', async () => {
    configurar();
    const harness = await RouterTestingHarness.create('/diario/registro/fantasma');
    expect(harness.routeNativeElement!.textContent).toContain('Registro não encontrado');
  });
});
