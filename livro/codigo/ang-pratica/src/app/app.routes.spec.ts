import { TestBed } from '@angular/core/testing';
import { provideRouter, withComponentInputBinding } from '@angular/router';
import { RouterTestingHarness } from '@angular/router/testing';
import { routes } from './app.routes';
import { BibliotecaStore } from './biblioteca.store';

const CHAVE = 'biblioteca-viva:estado:v1';

describe('navegação da Biblioteca Viva', () => {
  beforeEach(() => {
    localStorage.removeItem(CHAVE);
    TestBed.configureTestingModule({
      providers: [provideRouter(routes, withComponentInputBinding())],
    });
  });

  afterEach(() => {
    TestBed.resetTestingModule();
    localStorage.removeItem(CHAVE);
  });

  it('abre o catálogo e apresenta os seis livros', async () => {
    const harness = await RouterTestingHarness.create('/');

    expect(harness.routeNativeElement?.querySelectorAll('app-livro-card')).toHaveLength(6);
  });

  it('usa o parâmetro da URL para abrir um livro', async () => {
    const harness = await RouterTestingHarness.create('/livros/mapas-cotidiano');

    expect(harness.routeNativeElement?.querySelector('h1')?.textContent).toContain('Mapas do Cotidiano');
  });

  it('apresenta recuperação para um identificador de livro que não existe', async () => {
    const harness = await RouterTestingHarness.create('/livros/nao-existe');

    expect(harness.routeNativeElement?.textContent).toContain('Este livro não está no catálogo');
  });

  it('rota desconhecida oferece caminho para o catálogo', async () => {
    const harness = await RouterTestingHarness.create('/caminho/inexistente');

    expect(harness.routeNativeElement?.textContent).toContain('Essa página não está na estante');
  });

  it('favoritos compartilham o estado da store', async () => {
    TestBed.inject(BibliotecaStore).alternarFavorito('oceano-silencioso');
    const harness = await RouterTestingHarness.create('/favoritos');

    expect(harness.routeNativeElement?.textContent).toContain('Oceano Silencioso');
  });
});
