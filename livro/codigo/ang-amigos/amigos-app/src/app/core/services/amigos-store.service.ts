import { Injectable, signal, computed } from '@angular/core';
import { Amigo, Elogio } from '../models/amigo.model';

@Injectable({
  providedIn: 'root'
})
export class AmigosStoreService {
  private readonly _amigos = signal<readonly Amigo[]>([
    { id: '1', nome: 'Frank Gammond', cargo: 'Especialista Shakespeariano', departamento: 'Letras e Diplomacia', totalElogios: 14 },
    { id: '2', nome: 'Josephine Carnegie', cargo: 'Secretária Executiva', departamento: 'Comunicação', totalElogios: 22 },
    { id: '3', nome: 'Adolph Seltz', cargo: 'Desenhista Comercial', departamento: 'Design de Estampas', totalElogios: 35 },
    { id: '4', nome: 'Theodore Roosevelt', cargo: 'Presidente Honorário', departamento: 'Relações Humanas', totalElogios: 58 }
  ]);

  private readonly _elogios = signal<readonly Elogio[]>([]);

  readonly termoFiltro = signal<string>('');

  /** Visão somente-leitura pública */
  readonly amigos = this._amigos.asReadonly();
  readonly elogios = this._elogios.asReadonly();

  /** Lista computada filtrada por nome ou cargo */
  readonly amigosFiltrados = computed(() => {
    const termo = this.termoFiltro().trim().toLowerCase();
    const lista = this._amigos();
    if (!termo) return lista;
    return lista.filter(a => 
      a.nome.toLowerCase().includes(termo) || 
      a.cargo.toLowerCase().includes(termo)
    );
  });

  /** Total consolidado de elogios */
  readonly totalElogiosGeral = computed(() => {
    return this._amigos().reduce((acc, cur) => acc + cur.totalElogios, 0);
  });

  registrarElogio(novoElogio: Omit<Elogio, 'id' | 'dataCriacao'>): void {
    const elogioCompleto: Elogio = {
      ...novoElogio,
      id: `elg-${Date.now()}`,
      dataCriacao: new Date().toLocaleTimeString('pt-BR')
    };

    // Atualiza imutavelmente a lista de elogios
    this._elogios.update(lista => [elogioCompleto, ...lista]);

    // Incrementa a contagem do amigo homenageado
    this._amigos.update(lista => 
      lista.map(a => a.id === novoElogio.amigoId ? { ...a, totalElogios: a.totalElogios + 1 } : a)
    );
  }
}

