import { TestBed } from '@angular/core/testing';
import { firstValueFrom, Observable, of, Subject, take, toArray } from 'rxjs';
import { TestScheduler } from 'rxjs/testing';
import { FluxoDePensamentos } from './fluxo-de-pensamentos';
import { Observador } from './observador';
import { classificar } from './rotulos';
import { buscaCalma, FonteDeSugestoes } from './sugestoes';
import { INTERVALO_DE_CALMA, RELOGIO } from './tokens';

describe('classificar (cap. 8)', () => {
  it.each([
    ['e se a demo falhar?', 'preocupando'],
    ['eu sempre esqueço o teste', 'julgando'],
    ['amanhã reviso o PR', 'planejando'],
    ['ontem o deploy foi tranquilo', 'lembrando'],
    ['café', 'outro'],
  ])('"%s" → %s', (texto, rotulo) => expect(classificar(texto)).toBe(rotulo));
});

describe('FluxoDePensamentos (cap. 8)', () => {
  it('usa o relógio injetado e guarda só os cinco mais recentes', async () => {
    const instante = new Date('2026-11-02T09:00:00Z');
    TestBed.configureTestingModule({ providers: [{ provide: RELOGIO, useValue: () => instante }] });
    const fluxo = TestBed.inject(FluxoDePensamentos);

    const ultimo = firstValueFrom(fluxo.recentes$.pipe(take(7), toArray()));
    for (let i = 1; i <= 7; i++) fluxo.notar(`pensamento ${i}`);
    fluxo.notar('   '); // vazio: nem entra

    const listas = await ultimo;
    expect(listas.at(-1)!.map((e) => e.texto)).toEqual(['pensamento 7', 'pensamento 6', 'pensamento 5', 'pensamento 4', 'pensamento 3']);
    expect(listas.at(-1)![0].em).toBe(instante);
  });
});

describe('buscaCalma (cap. 8) com diagramas de bolinhas', () => {
  const novoAgendador = () => new TestScheduler((atual, esperado) => expect(atual).toEqual(esperado));

  it('espera a calma, devolve vazio para termo curto e ignora repetidos', () => {
    novoAgendador().run(({ cold, expectObservable }) => {
      const termos = cold('a 100ms b 100ms b', { a: 'r', b: 'resp' });
      const buscar = (t: string) => of([`achou ${t}`]);
      expectObservable(buscaCalma(termos, buscar, 50)).toBe('50ms v 100ms x', { v: [], x: ['achou resp'] });
    });
  });

  it('um termo novo cancela a busca antiga (switchMap)', () => {
    novoAgendador().run(({ cold, expectObservable }) => {
      const termos = cold('a 20ms b', { a: 'sons', b: 'corpo' });
      const buscar = (t: string): Observable<readonly string[]> =>
        t === 'sons' ? cold('100ms x', { x: ['lenta'] }) : cold('10ms y', { y: ['rápida'] });
      // a busca por "sons" demoraria 100 ms, mas "corpo" chega antes: só "rápida" aparece
      expectObservable(buscaCalma(termos, buscar, 0)).toBe('31ms y', { y: ['rápida'] });
    });
  });
});

describe('Observador (cap. 8)', () => {
  it('troca a fonte por injeção e mostra as sugestões', async () => {
    const respostas = new Subject<readonly string[]>();
    TestBed.configureTestingModule({
      providers: [
        { provide: FonteDeSugestoes, useValue: { buscar: () => respostas } },
        { provide: INTERVALO_DE_CALMA, useValue: 0 },
      ],
    });
    const fixture = TestBed.createComponent(Observador);
    await fixture.whenStable();
    const tela = fixture.nativeElement as HTMLElement;

    const busca = tela.querySelector<HTMLInputElement>('#busca')!;
    busca.value = 'resp';
    busca.dispatchEvent(new Event('input'));
    await fixture.whenStable();
    await new Promise((r) => setTimeout(r, 10)); // debounce de 0 ms ainda é assíncrono
    respostas.next(['respiração de um minuto']);
    await fixture.whenStable();

    expect(tela.textContent).toContain('respiração de um minuto');
  });

  it('notar um pensamento o mostra rotulado', async () => {
    const fixture = TestBed.createComponent(Observador);
    await fixture.whenStable();
    const tela = fixture.nativeElement as HTMLElement;
    tela.querySelector<HTMLInputElement>('#pensamento')!.value = 'e se eu errar?';
    tela.querySelector('form')!.dispatchEvent(new Event('submit'));
    await fixture.whenStable();

    expect(tela.querySelector('li')!.textContent).toBe('preocupando: e se eu errar?');
  });
});
