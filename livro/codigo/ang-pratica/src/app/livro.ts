export interface Livro {
  readonly id: string;
  readonly titulo: string;
  readonly autor: string;
  readonly categoria: string;
  readonly ano: number;
  readonly paginas: number;
  readonly descricao: string;
  readonly cor: string;
}

export interface EstadoBiblioteca {
  readonly favoritos: readonly string[];
  readonly progresso: Readonly<Record<string, number>>;
}
