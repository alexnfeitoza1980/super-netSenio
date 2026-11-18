import { DiagnosticoMedoPipe } from './diagnostico-medo.pipe';
import { Medo } from '../../core/models/medo.model';

describe('DiagnosticoMedoPipe', () => {
  const pipe = new DiagnosticoMedoPipe();

  it('deve retornar mensagem de vitória para medos vencidos', () => {
    const medo: Medo = { id: '1', titulo: 'Teste', categoria: 'TECNICO', nivelParalisia: 9, ehRuido: false, vencido: true, acoes: [], criadoEm: '' };
    expect(pipe.transform(medo)).toContain('CONQUISTADO');
  });

  it('deve alertar ruído para medos marcados como ruído', () => {
    const medo: Medo = { id: '2', titulo: 'Fofoca', categoria: 'PROFISSIONAL', nivelParalisia: 4, ehRuido: true, vencido: false, acoes: [], criadoEm: '' };
    expect(pipe.transform(medo)).toContain('MERO RUÍDO');
  });

  it('deve apontar desafio crítico para paralisia alta', () => {
    const medo: Medo = { id: '3', titulo: 'Deploy', categoria: 'TECNICO', nivelParalisia: 8, ehRuido: false, vencido: false, acoes: [], criadoEm: '' };
    expect(pipe.transform(medo)).toContain('DESAFIO CRÍTICO');
  });

  it('deve apontar desafio normal para paralisia moderada', () => {
    const medo: Medo = { id: '4', titulo: 'Aprender Git', categoria: 'TECNICO', nivelParalisia: 5, ehRuido: false, vencido: false, acoes: [], criadoEm: '' };
    expect(pipe.transform(medo)).toContain('DESAFIO NORMAL');
  });
});

