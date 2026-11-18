import { TestBed } from '@angular/core/testing';
import { provideRouter, Router, withComponentInputBinding } from '@angular/router';
import { RouterTestingHarness } from '@angular/router/testing';
import { Catalogo } from './catalogo';
import { DemoDoCapitulo } from './demo-do-capitulo';
import { demoExiste } from './guardas';
import { NaoEncontrada } from './nao-encontrada';

describe('rotas das demonstrações (cap. 7)', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideRouter(
          [
            { path: 'capitulos', component: Catalogo },
            { path: 'capitulos/:id', component: DemoDoCapitulo, canActivate: [demoExiste] },
            { path: '**', component: NaoEncontrada },
          ],
          withComponentInputBinding(),
        ),
      ],
    });
  });

  it('o catálogo lista as demonstrações com links', async () => {
    const harness = await RouterTestingHarness.create('/capitulos');
    const links = harness.routeNativeElement!.querySelectorAll('a');
    expect(links.length).toBeGreaterThan(5);
    expect(links[0].getAttribute('href')).toBe('/capitulos/pausa');
  });

  it('o :id vira input e a demonstração é carregada sob demanda', async () => {
    const harness = await RouterTestingHarness.create();
    await harness.navigateByUrl('/capitulos/pausa');
    await vi.waitFor(async () => {
      await harness.fixture.whenStable();
      expect(harness.routeNativeElement!.querySelector('app-pausa')).toBeTruthy();
    });
    expect(harness.routeNativeElement!.querySelector('h1')!.textContent).toContain('(cap. 1)');
  });

  it('id desconhecido: a guarda redireciona ao catálogo com um aviso', async () => {
    const harness = await RouterTestingHarness.create();
    await harness.navigateByUrl('/capitulos/inexistente');
    expect(TestBed.inject(Router).url).toBe('/capitulos?naoAchei=inexistente');
    expect(harness.routeNativeElement!.textContent).toContain('Não encontrei a demonstração “inexistente”');
  });

  it('qualquer outro caminho cai na página coringa', async () => {
    const harness = await RouterTestingHarness.create('/um/lugar/qualquer');
    expect(harness.routeNativeElement!.textContent).toContain('Esta página não existe');
  });
});
