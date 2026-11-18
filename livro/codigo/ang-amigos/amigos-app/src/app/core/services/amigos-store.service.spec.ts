import { TestBed } from '@angular/core/testing';
import { AmigosStoreService } from './amigos-store.service';

describe('AmigosStoreService com Signals', () => {
  let service: AmigosStoreService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(AmigosStoreService);
  });

  it('deve inicializar com amigos cadastrados e total correto', () => {
    expect(service.amigos().length).toBe(4);
    expect(service.totalElogiosGeral()).toBe(14 + 22 + 35 + 58);
  });

  it('deve filtrar amigos reativamente por nome sem mutar o estado original', () => {
    service.termoFiltro.set('frank');
    expect(service.amigosFiltrados().length).toBe(1);
    expect(service.amigosFiltrados()[0].nome).toBe('Frank Gammond');

    // Ao limpar o filtro, todos retornam
    service.termoFiltro.set('');
    expect(service.amigosFiltrados().length).toBe(4);
  });

  it('deve registrar um novo elogio incrementando o total de forma imutável', () => {
    const totalAnterior = service.totalElogiosGeral();

    service.registrarElogio({
      amigoId: '1',
      remetente: 'Dale Carnegie',
      mensagem: 'Obrigado por salvar as aparências no banquete em Londres!',
      categoria: 'lideranca'
    });

    expect(service.totalElogiosGeral()).toBe(totalAnterior + 1);
    expect(service.elogios().length).toBe(1);
    expect(service.amigos().find(a => a.id === '1')?.totalElogios).toBe(15);
  });
});

