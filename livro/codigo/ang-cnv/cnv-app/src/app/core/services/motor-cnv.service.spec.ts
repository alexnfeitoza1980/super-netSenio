import { TestBed } from '@angular/core/testing';
import { MotorCnvService } from './motor-cnv.service';

describe('MotorCnvService', () => {
  let service: MotorCnvService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(MotorCnvService);
  });

  it('deve ser instanciado com sucesso', () => {
    expect(service).toBeTruthy();
  });

  it('deve iniciar com histórico vazio', () => {
    expect(service.historico().length).toBe(0);
    expect(service.totalTraducoes()).toBe(0);
  });

  it('deve decompor a mensagem nos 4 pilares da CNV', () => {
    const res = service.traduzir('Você nunca me ouve e sempre atrasa!');
    expect(res).toBeDefined();
    expect(res.observacao.nome).toBe('Observação');
    expect(res.sentimento.nome).toBe('Sentimento');
    expect(res.necessidade.nome).toBe('Necessidade');
    expect(res.pedido.nome).toBe('Pedido');
    expect(res.nivelTensaoOriginal).toBeGreaterThanOrEqual(6);
  });

  it('deve atualizar o histórico de forma imutável', () => {
    service.traduzir('Primeira mensagem');
    service.traduzir('Segunda mensagem');
    expect(service.totalTraducoes()).toBe(2);
    expect(service.ultimaTraducao()?.textoOriginal).toBe('Segunda mensagem');
  });
});

