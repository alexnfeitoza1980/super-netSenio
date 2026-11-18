import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { App } from './app';
import { routes } from './app.routes';

describe('App (casca)', () => {
  it('mostra a navegação principal com os quatro destinos', async () => {
    TestBed.configureTestingModule({ providers: [provideRouter(routes)] });
    const fixture = TestBed.createComponent(App);
    await fixture.whenStable();
    const links = [...(fixture.nativeElement as HTMLElement).querySelectorAll('nav a')].map((a) => a.getAttribute('href'));
    expect(links).toEqual(['/diario/hoje', '/diario/historico', '/pausa', '/capitulos']);
  });
});
