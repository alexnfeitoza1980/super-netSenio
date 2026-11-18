import { Injectable, signal, computed, effect } from '@angular/core';
import { Medo, CategoriaMedo } from '../models/medo.model';

const STORAGE_KEY = 'antimedo_registros_v1';

@Injectable({ providedIn: 'root' })
export class MedoService {
  // Sinal principal imutável
  private medosSignal = signal<Medo[]>(this.carregarDoStorage());

  // Sinal somente leitura público
  public medos = this.medosSignal.asReadonly();

  // Métricas computadas automaticamente via Signals
  public totalMedos = computed(() => this.medosSignal().length);
  
  public totalVencidos = computed(() => 
    this.medosSignal().filter(m => m.vencido).length
  );

  public percentualCoragem = computed(() => {
    const total = this.totalMedos();
    if (total === 0) return 100;
    return Math.round((this.totalVencidos() / total) * 100);
  });

  public ruidosDetectados = computed(() => 
    this.medosSignal().filter(m => m.ehRuido && !m.vencido).length
  );

  constructor() {
    // Sincronização automática com localStorage via effect()
    effect(() => {
      const dados = this.medosSignal();
      try {
        if (typeof localStorage !== 'undefined') {
          localStorage.setItem(STORAGE_KEY, JSON.stringify(dados));
        }
      } catch (e) {
        console.warn('Não foi possível persistir no storage:', e);
      }
    });
  }

  adicionar(medo: Omit<Medo, 'id' | 'vencido' | 'criadoEm'>): void {
    const novo: Medo = {
      ...medo,
      id: typeof crypto !== 'undefined' && crypto.randomUUID ? crypto.randomUUID() : Math.random().toString(36).substring(2, 9),
      vencido: false,
      criadoEm: new Date().toISOString()
    };
    this.medosSignal.update(lista => [novo, ...lista]);
  }

  alternarVencido(id: string): void {
    this.medosSignal.update(lista =>
      lista.map(m => m.id === id ? { ...m, vencido: !m.vencido } : m)
    );
  }

  remover(id: string): void {
    this.medosSignal.update(lista => lista.filter(m => m.id !== id));
  }

  definirMedosParaTeste(medos: Medo[]): void {
    this.medosSignal.set(medos);
  }

  private carregarDoStorage(): Medo[] {
    try {
      if (typeof localStorage !== 'undefined') {
        const salvo = localStorage.getItem(STORAGE_KEY);
        if (salvo) return JSON.parse(salvo);
      }
    } catch {}
    return [
      {
        id: '1',
        titulo: 'Medo de fazer deploy em produção na sexta',
        categoria: 'TECNICO',
        nivelParalisia: 8,
        ehRuido: false,
        vencido: false,
        acoes: [],
        criadoEm: new Date().toISOString()
      },
      {
        id: '2',
        titulo: 'Opinião de quem nunca construiu nada na web',
        categoria: 'PROFISSIONAL',
        nivelParalisia: 6,
        ehRuido: true,
        vencido: true,
        acoes: [],
        criadoEm: new Date().toISOString()
      }
    ];
  }
}

