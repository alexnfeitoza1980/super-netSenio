import { TestBed } from '@angular/core/testing';
import { MedoService } from './medo.service';
import { Medo } from '../models/medo.model';

describe('MedoService (Testes Unitários com Signals)', () => {
  let service: MedoService;

  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [MedoService]
    });
    service = TestBed.inject(MedoService);
  });

  it('deve ser instanciado com sucesso', () => {
    expect(service).toBeTruthy();
  });

  it('deve calcular métricas corretas via computed()', () => {
    const medosMock: Medo[] = [
      { id: '1', titulo: 'Medo A', categoria: 'TECNICO', nivelParalisia: 9, ehRuido: false, vencido: true, acoes: [], criadoEm: '' },
      { id: '2', titulo: 'Medo B', categoria: 'PROFISSIONAL', nivelParalisia: 5, ehRuido: true, vencido: false, acoes: [], criadoEm: '' }
    ];

    service.definirMedosParaTeste(medosMock);

    expect(service.totalMedos()).toBe(2);
    expect(service.totalVencidos()).toBe(1);
    expect(service.percentualCoragem()).toBe(50);
    expect(service.ruidosDetectados()).toBe(1);
  });

  it('deve alternar status de vencido com imutabilidade', () => {
    const medosMock: Medo[] = [
      { id: '10', titulo: 'Medo Teste', categoria: 'PESSOAL', nivelParalisia: 7, ehRuido: false, vencido: false, acoes: [], criadoEm: '' }
    ];

    service.definirMedosParaTeste(medosMock);
    expect(service.medos()[0].vencido).toBeFalse();

    service.alternarVencido('10');
    expect(service.medos()[0].vencido).toBeTrue();
  });

  it('deve adicionar um novo medo no início da lista', () => {
    service.definirMedosParaTeste([]);
    expect(service.totalMedos()).toBe(0);

    service.adicionar({
      titulo: 'Novo Desafio',
      categoria: 'FINANCEIRO',
      nivelParalisia: 6,
      ehRuido: false,
      acoes: []
    });

    expect(service.totalMedos()).toBe(1);
    expect(service.medos()[0].titulo).toBe('Novo Desafio');
    expect(service.medos()[0].vencido).toBeFalse();
  });
});

