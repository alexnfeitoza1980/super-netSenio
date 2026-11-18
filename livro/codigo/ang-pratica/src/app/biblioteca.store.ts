import { computed, Injectable, signal } from '@angular/core';
import { LIVROS } from './catalogo';
import { EstadoBiblioteca, Livro } from './livro';

const CHAVE_ESTADO = 'biblioteca-viva:estado:v1';

function normalizar(texto: string): string {
  return texto.normalize('NFD').replace(/\p{Diacritic}/gu, '').toLocaleLowerCase('pt-BR').trim();
}

@Injectable({ providedIn: 'root' })
export class BibliotecaStore {
  private readonly estado = signal<EstadoBiblioteca>({ favoritos: [], progresso: {} });
  readonly busca = signal('');
  readonly categoria = signal('Todas');
  readonly avisoPersistencia = signal<string | null>(null);

  readonly categorias = ['Todas', ...new Set(LIVROS.map((livro) => livro.categoria))];
  readonly livros = LIVROS;

  readonly resultados = computed(() => {
    const termo = normalizar(this.busca());
    const categoria = this.categoria();
    return LIVROS.filter((livro) => {
      const correspondeCategoria = categoria === 'Todas' || livro.categoria === categoria;
      const texto = normalizar(`${livro.titulo} ${livro.autor} ${livro.categoria} ${livro.descricao}`);
      return correspondeCategoria && (!termo || texto.includes(termo));
    });
  });

  readonly favoritos = computed(() => LIVROS.filter((livro) => this.estado().favoritos.includes(livro.id)));
  readonly quantidadeFavoritos = computed(() => this.estado().favoritos.length);

  constructor() {
    this.carregar();
  }

  encontrar(id: string): Livro | undefined {
    return LIVROS.find((livro) => livro.id === id);
  }

  progressoDe(id: string): number {
    return this.estado().progresso[id] ?? 0;
  }

  ehFavorito(id: string): boolean {
    return this.estado().favoritos.includes(id);
  }

  atualizarBusca(busca: string): void {
    this.busca.set(busca);
  }

  selecionarCategoria(categoria: string): void {
    if (this.categorias.includes(categoria)) this.categoria.set(categoria);
  }

  alternarFavorito(id: string): void {
    if (!this.encontrar(id)) return;
    const favoritos = this.estado().favoritos;
    const atualizados = favoritos.includes(id) ? favoritos.filter((favorito) => favorito !== id) : [...favoritos, id];
    this.atualizar({ ...this.estado(), favoritos: atualizados });
  }

  atualizarProgresso(id: string, valor: number): void {
    if (!this.encontrar(id) || !Number.isFinite(valor)) return;
    const progresso = Math.max(0, Math.min(100, Math.round(valor)));
    this.atualizar({ ...this.estado(), progresso: { ...this.estado().progresso, [id]: progresso } });
  }

  private carregar(): void {
    try {
      const salvo = localStorage.getItem(CHAVE_ESTADO);
      if (!salvo) return;
      const candidato: unknown = JSON.parse(salvo);
      if (!this.estadoValido(candidato)) throw new Error('Os dados salvos não têm o formato esperado.');
      const idsValidos = new Set(LIVROS.map((livro) => livro.id));
      const favoritos = candidato.favoritos.filter((id) => idsValidos.has(id));
      const progresso = Object.fromEntries(
        Object.entries(candidato.progresso).filter(([id, valor]) => idsValidos.has(id) && Number.isFinite(valor)),
      );
      this.estado.set({ favoritos, progresso });
    } catch (erro: unknown) {
      this.avisoPersistencia.set(
        `Não foi possível recuperar suas preferências deste navegador. O catálogo continua disponível. Detalhe: ${this.mensagem(erro)}`,
      );
    }
  }

  private atualizar(estado: EstadoBiblioteca): void {
    this.estado.set(estado);
    try {
      localStorage.setItem(CHAVE_ESTADO, JSON.stringify(estado));
      this.avisoPersistencia.set(null);
    } catch (erro: unknown) {
      this.avisoPersistencia.set(
        `A alteração aparece nesta sessão, mas não pôde ser salva neste navegador. Detalhe: ${this.mensagem(erro)}`,
      );
    }
  }

  private estadoValido(valor: unknown): valor is EstadoBiblioteca {
    if (!valor || typeof valor !== 'object') return false;
    const estado = valor as Record<string, unknown>;
    if (!Array.isArray(estado['favoritos']) || !estado['favoritos'].every((item) => typeof item === 'string')) return false;
    const progresso = estado['progresso'];
    if (!progresso || typeof progresso !== 'object' || Array.isArray(progresso)) return false;
    return Object.entries(progresso).every(([id, numero]) => id.length > 0 && typeof numero === 'number' && numero >= 0 && numero <= 100);
  }

  private mensagem(erro: unknown): string {
    return erro instanceof Error ? erro.message : 'erro desconhecido';
  }
}
