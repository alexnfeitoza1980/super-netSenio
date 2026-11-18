import { NgComponentOutlet } from '@angular/common';
import { Component, computed, input, resource } from '@angular/core';
import { RouterLink } from '@angular/router';
import { acharDemo } from './demos';

/**
 * Cap. 7 — o parâmetro `:id` da rota chega como input (withComponentInputBinding).
 * Um `resource` carrega o componente da demonstração; quando o id muda, carrega de novo.
 * Se `params` devolve undefined, o resource fica ocioso (não chama o loader).
 */
@Component({
  selector: 'app-demo-do-capitulo',
  imports: [NgComponentOutlet, RouterLink],
  template: `
    <nav><a routerLink="/capitulos">← Todas as demonstrações</a></nav>
    <h1>{{ demo()?.titulo }} <small>(cap. {{ demo()?.capitulo }})</small></h1>

    @if (componente.isLoading()) {
      <p>Carregando…</p>
    } @else if (componente.error()) {
      <p class="erro">Não consegui carregar esta demonstração.</p>
      <button type="button" (click)="componente.reload()">Tentar de novo</button>
    } @else if (componente.value(); as tipo) {
      <ng-container *ngComponentOutlet="tipo" />
    }
  `,
})
export class DemoDoCapitulo {
  readonly id = input.required<string>();

  protected readonly demo = computed(() => acharDemo(this.id()));

  protected readonly componente = resource({
    params: () => this.demo(),
    loader: ({ params: demo }) => demo.carregar(),
  });
}
