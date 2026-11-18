export type CategoriaMedo = 'TECNICO' | 'PROFISSIONAL' | 'PESSOAL' | 'FINANCEIRO';

export interface AcaoEnfrentamento {
  id: string;
  descricao: string;
  concluida: boolean;
  dataLimite: string;
}

export interface Medo {
  id: string;
  titulo: string;
  categoria: CategoriaMedo;
  nivelParalisia: number; // 1 a 10
  ehRuido: boolean;       // true se for mera opinião/distração, false se for desafio real
  vencido: boolean;
  acoes: AcaoEnfrentamento[];
  criadoEm: string;
}

