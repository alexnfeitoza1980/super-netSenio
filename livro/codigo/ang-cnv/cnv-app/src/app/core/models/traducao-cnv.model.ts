export interface PilarCNV {
  readonly nome: 'Observação' | 'Sentimento' | 'Necessidade' | 'Pedido';
  readonly icone: string;
  readonly conteudo: string;
  readonly orientacao: string;
}

export interface ResultadoTraducao {
  readonly id: string;
  readonly textoOriginal: string;
  readonly nivelTensaoOriginal: number; // Escala de 1 a 10
  readonly observacao: PilarCNV;
  readonly sentimento: PilarCNV;
  readonly necessidade: PilarCNV;
  readonly pedido: PilarCNV;
  readonly respostaSugerida: string;
  readonly dataHora: Date;
}

