export interface Amigo {
  readonly id: string;
  readonly nome: string;
  readonly cargo: string;
  readonly departamento: string;
  readonly fotoUrl?: string;
  readonly totalElogios: number;
}

export interface Elogio {
  readonly id: string;
  readonly amigoId: string;
  readonly remetente: string;
  readonly mensagem: string;
  readonly categoria: 'tecnica' | 'lideranca' | 'gentileza' | 'resiliencia';
  readonly dataCriacao: string;
}

