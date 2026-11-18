/** Cap. 4 — o mapa das oito semanas: um tema para a vida e um tema do Angular em cada uma. */
export interface Semana {
  readonly numero: number;
  readonly tema: string;
  readonly pratica: string;
  readonly angular: string;
  readonly capitulo: number;
}

export const SEMANAS: readonly Semana[] = [
  { numero: 1, tema: 'Perceber o piloto automático', pratica: 'Comer devagar uma fruta, notando cada detalhe', angular: 'Detecção de mudanças e zoneless', capitulo: 5 },
  { numero: 2, tema: 'Voltar ao corpo', pratica: 'Percorrer o corpo com a atenção, da cabeça aos pés', angular: 'Templates e acessibilidade', capitulo: 6 },
  { numero: 3, tema: 'Aproximar em vez de fugir', pratica: 'Alongar devagar e fazer uma pausa de três minutos', angular: 'Rotas e navegação', capitulo: 7 },
  { numero: 4, tema: 'Pensamentos são eventos', pratica: 'Ouvir sons e ver pensamentos passarem', angular: 'Serviços, injeção e RxJS', capitulo: 8 },
  { numero: 5, tema: 'Encarar o que é difícil', pratica: 'Chegar perto de uma dificuldade, com gentileza', angular: 'HTTP e tratamento de erros', capitulo: 9 },
  { numero: 6, tema: 'Passado, futuro e presente', pratica: 'Desejar o bem a si e aos outros', angular: 'Formulários com signals', capitulo: 10 },
  { numero: 7, tema: 'O que nutre e o que desgasta', pratica: 'Rever o dia e equilibrar as atividades', angular: 'Desempenho e @defer', capitulo: 11 },
  { numero: 8, tema: 'Seguir praticando', pratica: 'Escolher a prática que fica', angular: 'Testes e manutenção', capitulo: 12 },
];
