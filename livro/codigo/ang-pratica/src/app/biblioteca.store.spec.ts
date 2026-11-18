import { TestBed } from '@angular/core/testing';
import { BibliotecaStore } from './biblioteca.store';

const CHAVE = 'biblioteca-viva:estado:v1';

describe('BibliotecaStore', () => {
  beforeEach(() => {
    localStorage.removeItem(CHAVE);
    TestBed.configureTestingModule({ providers: [BibliotecaStore] });
  });

  afterEach(() => {
    TestBed.resetTestingModule();
    localStorage.removeItem(CHAVE);
  });

  it('filtra sem diferenciar maiúsculas nem acentos', () => {
    const store = TestBed.inject(BibliotecaStore);
    store.atualizarBusca('nao ficcao');

    expect(store.resultados().map((livro) => livro.id)).toEqual(['mapas-cotidiano']);
  });

  it('combina texto e categoria sem alterar a lista original', () => {
    const store = TestBed.inject(BibliotecaStore);
    store.selecionarCategoria('Ficção');
    store.atualizarBusca('oceano');

    expect(store.resultados().map((livro) => livro.id)).toEqual(['oceano-silencioso']);
    expect(store.livros).toHaveLength(6);
  });

  it('alterna favoritos e persiste o estado no navegador', () => {
    const store = TestBed.inject(BibliotecaStore);
    store.alternarFavorito('jardim-das-ideias');

    expect(store.ehFavorito('jardim-das-ideias')).toBe(true);
    expect(store.quantidadeFavoritos()).toBe(1);
    expect(localStorage.getItem(CHAVE)).toContain('jardim-das-ideias');

    store.alternarFavorito('jardim-das-ideias');
    expect(store.quantidadeFavoritos()).toBe(0);
  });

  it('limita progresso entre zero e cem e ignora valores inválidos', () => {
    const store = TestBed.inject(BibliotecaStore);
    store.atualizarProgresso('mapas-cotidiano', 155);
    expect(store.progressoDe('mapas-cotidiano')).toBe(100);

    store.atualizarProgresso('mapas-cotidiano', Number.NaN);
    expect(store.progressoDe('mapas-cotidiano')).toBe(100);
    store.atualizarProgresso('inexistente', 40);
    expect(store.progressoDe('inexistente')).toBe(0);
  });

  it('descarta favoritos desconhecidos ao recuperar uma gravação antiga', () => {
    localStorage.setItem(CHAVE, JSON.stringify({ favoritos: ['livro-removido'], progresso: {} }));
    const store = TestBed.inject(BibliotecaStore);

    expect(store.favoritos()).toEqual([]);
    expect(store.avisoPersistencia()).toBeNull();
  });

  it('avisa quando a gravação local está corrompida sem bloquear o catálogo', () => {
    localStorage.setItem(CHAVE, '{');
    const store = TestBed.inject(BibliotecaStore);

    expect(store.livros).toHaveLength(6);
    expect(store.avisoPersistencia()).toContain('Não foi possível recuperar');
  });
});
