import { Type } from '@angular/core';

/** Cap. 7 — o catálogo das demonstrações. Cada uma é carregada só quando alguém a visita. */
export interface Demo {
  readonly titulo: string;
  readonly capitulo: number;
  readonly carregar: () => Promise<Type<unknown>>;
}

export const DEMOS: Readonly<Record<string, Demo>> = {
  pausa: { titulo: 'Um minuto antes de codar', capitulo: 1, carregar: () => import('../cap01/pausa').then((m) => m.Pausa) },
  sessao: { titulo: 'Modo Atuante e modo Existente', capitulo: 3, carregar: () => import('../cap03/painel-da-sessao').then((m) => m.PainelDaSessao) },
  programa: { titulo: 'O programa de oito semanas', capitulo: 4, carregar: () => import('../cap04/programa').then((m) => m.Programa) },
  vizinhos: { titulo: 'OnPush e Eager lado a lado', capitulo: 5, carregar: () => import('../cap05/vizinhos').then((m) => m.Vizinhos) },
  corpo: { titulo: 'Exploração do corpo', capitulo: 6, carregar: () => import('../cap06/exploracao-do-corpo').then((m) => m.ExploracaoDoCorpo) },
  pensamentos: { titulo: 'Pensamentos como eventos', capitulo: 8, carregar: () => import('../cap08/observador').then((m) => m.Observador) },
  frases: { titulo: 'Frases que chegam pela rede', capitulo: 9, carregar: () => import('../cap09/frases').then((m) => m.Frases) },
  registro: { titulo: 'Registro gentil', capitulo: 10, carregar: () => import('../cap10/registro-gentil').then((m) => m.RegistroGentil) },
  balanco: { titulo: 'Balanço do dia', capitulo: 11, carregar: () => import('../cap11/balanco').then((m) => m.Balanco) },
};

/** Busca segura: um id qualquer vindo da URL pode não existir (e "toString" não é uma demo!). */
export function acharDemo(id: string): Demo | undefined {
  return Object.hasOwn(DEMOS, id) ? DEMOS[id] : undefined;
}
