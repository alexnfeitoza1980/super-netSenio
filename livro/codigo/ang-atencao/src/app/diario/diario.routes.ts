import { Routes } from '@angular/router';

/** Lab 2 — as rotas do Diário, carregadas só quando alguém entra em /diario. */
export const DIARIO_ROTAS: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'hoje' },
  { path: 'hoje', title: 'Hoje · Diário de Atenção', loadComponent: () => import('./hoje').then((m) => m.Hoje) },
  { path: 'historico', title: 'Histórico · Diário de Atenção', loadComponent: () => import('./historico').then((m) => m.Historico) },
  { path: 'registro/:id', title: 'Registro · Diário de Atenção', loadComponent: () => import('./entrada-detalhe').then((m) => m.EntradaDetalhe) },
];
