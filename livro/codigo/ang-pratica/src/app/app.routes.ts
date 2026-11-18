import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    title: 'Biblioteca Viva — catálogo',
    loadComponent: () => import('./catalogo-page').then((modulo) => modulo.CatalogoPage),
  },
  {
    path: 'livros/:id',
    title: 'Detalhes do livro',
    loadComponent: () => import('./detalhe-page').then((modulo) => modulo.DetalhePage),
  },
  {
    path: 'favoritos',
    title: 'Seus favoritos',
    loadComponent: () => import('./favoritos-page').then((modulo) => modulo.FavoritosPage),
  },
  {
    path: '**',
    title: 'Página não encontrada',
    loadComponent: () => import('./nao-encontrada-page').then((modulo) => modulo.NaoEncontradaPage),
  },
];
