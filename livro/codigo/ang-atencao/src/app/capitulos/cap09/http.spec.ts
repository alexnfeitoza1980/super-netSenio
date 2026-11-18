import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { firstValueFrom } from 'rxjs';
import { ampulhetaInterceptor, CONFIG_DA_AMPULHETA, mensagemPara, ProblemaDeRede } from './ampulheta.interceptor';
import { Frases } from './frases';
import { TratadorDeErros } from './tratador-de-erros';

describe('ampulhetaInterceptor (cap. 9)', () => {
  let linhas: string[];

  beforeEach(() => {
    linhas = [];
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([ampulhetaInterceptor])),
        provideHttpClientTesting(),
        { provide: CONFIG_DA_AMPULHETA, useValue: { tentativas: 2, esperaMs: 1, registrar: (l: string) => linhas.push(l) } },
      ],
    });
  });

  afterEach(() => TestBed.inject(HttpTestingController).verify());

  const esperar = (ms: number) => new Promise((r) => setTimeout(r, ms));

  it('repete um GET que falhou de forma passageira e entrega o resultado', async () => {
    const http = TestBed.inject(HttpClient), ctrl = TestBed.inject(HttpTestingController);
    const resposta = firstValueFrom(http.get<string[]>('/api/x'));

    ctrl.expectOne('/api/x').flush('ocupado', { status: 503, statusText: 'Service Unavailable' });
    await esperar(5);
    ctrl.expectOne('/api/x').flush(['ok']);

    expect(await resposta).toEqual(['ok']);
    expect(linhas[0]).toBe('→ GET /api/x');
    expect(linhas.at(-1)).toMatch(/^← GET \/api\/x \(\d+ ms\)$/);
  });

  it('não repete POST (poderia duplicar um registro) e traduz o erro', async () => {
    const http = TestBed.inject(HttpClient), ctrl = TestBed.inject(HttpTestingController);
    const resposta = firstValueFrom(http.post('/api/x', {}));

    ctrl.expectOne('/api/x').flush('ocupado', { status: 503, statusText: 'Service Unavailable' });

    await expect(resposta).rejects.toEqual(new ProblemaDeRede(503, mensagemPara(503)));
  });

  it('não repete 404: repetir não muda a resposta', async () => {
    const http = TestBed.inject(HttpClient), ctrl = TestBed.inject(HttpTestingController);
    const resposta = firstValueFrom(http.get('/api/nada'));
    ctrl.expectOne('/api/nada').flush('', { status: 404, statusText: 'Not Found' });

    await expect(resposta).rejects.toMatchObject({ status: 404, message: 'Não encontrei o que você pediu.' });
  });

  it('desiste depois das tentativas combinadas', async () => {
    const http = TestBed.inject(HttpClient), ctrl = TestBed.inject(HttpTestingController);
    // a expectativa é preparada ANTES: a promessa vai falhar durante o laço
    const falhou = expect(firstValueFrom(http.get('/api/x'))).rejects.toMatchObject({ status: 0 });
    for (let i = 0; i < 3; i++) {
      ctrl.expectOne('/api/x').error(new ProgressEvent('error'), { status: 0 });
      await esperar(10);
    }
    await falhou;
  });
});

describe('Frases com httpResource (cap. 9)', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([ampulhetaInterceptor])),
        provideHttpClientTesting(),
        { provide: CONFIG_DA_AMPULHETA, useValue: { tentativas: 0, esperaMs: 0, registrar: () => {} } },
      ],
    });
  });

  // Atenção: com uma requisição pendente, o app NÃO está estável (whenStable esperaria para sempre).
  // Por isso: TestBed.tick() dispara o resource, respondemos a requisição e só então esperamos.
  function criar() {
    const fixture = TestBed.createComponent(Frases);
    TestBed.tick();
    return { fixture, tela: fixture.nativeElement as HTMLElement, ctrl: TestBed.inject(HttpTestingController) };
  }

  it('busca pelo tema, mostra a lista e refaz ao trocar o tema', async () => {
    const { fixture, tela, ctrl } = criar();
    ctrl.expectOne('api/frases-pausa.json').flush([{ id: 1, texto: 'Uma coisa de cada vez.', tema: 'pausa' }]);
    await fixture.whenStable();
    expect(tela.querySelector('li')!.textContent).toBe('Uma coisa de cada vez.');

    const select = tela.querySelector('select')!;
    select.value = 'foco';
    select.dispatchEvent(new Event('change'));
    TestBed.tick();
    ctrl.expectOne('api/frases-foco.json').flush([]);
    await fixture.whenStable();
    expect(tela.textContent).toContain('Nenhuma frase para este tema ainda.');
  });

  it('erro: mostra a mensagem gentil e deixa tentar de novo', async () => {
    const { fixture, tela, ctrl } = criar();
    ctrl.expectOne('api/frases-pausa.json').flush('', { status: 500, statusText: 'Erro' });
    await fixture.whenStable();
    expect(tela.querySelector('[role=alert]')!.textContent).toContain('Não é com você');

    tela.querySelector<HTMLButtonElement>('[role=alert] button')!.click();
    TestBed.tick();
    ctrl.expectOne('api/frases-pausa.json').flush([{ id: 2, texto: 'De novo, com calma.', tema: 'pausa' }]);
    await fixture.whenStable();
    expect(tela.textContent).toContain('De novo, com calma.');
  });

  it('resposta em formato inesperado vira erro conferido, não dado quebrado na tela', async () => {
    const { fixture, tela, ctrl } = criar();
    ctrl.expectOne('api/frases-pausa.json').flush({ mensagem: 'oi' });
    await fixture.whenStable();
    expect(tela.querySelector('[role=alert]')!.textContent).toContain('formato que eu não reconheço');
  });
});

describe('TratadorDeErros (cap. 9)', () => {
  it('guarda os últimos erros com nome e mensagem', () => {
    const erro = vi.spyOn(console, 'error').mockImplementation(() => {});
    const t = new TratadorDeErros();
    t.handleError(new TypeError('x is undefined'));
    t.handleError('texto solto');
    expect(t.ultimos()).toEqual(['texto solto', 'TypeError: x is undefined']);
    erro.mockRestore();
  });
});
