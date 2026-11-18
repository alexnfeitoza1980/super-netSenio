import { InjectionToken } from '@angular/core';

export interface AmigosUIConfig {
  readonly animacoesHabilitadas: boolean;
  readonly duracaoToastMs: number;
  readonly temaPadrao: 'light' | 'dark';
}

export const AMG_CONFIG_PADRAO: AmigosUIConfig = {
  animacoesHabilitadas: true,
  duracaoToastMs: 3500,
  temaPadrao: 'light'
};

export const AMG_UI_CONFIG = new InjectionToken<AmigosUIConfig>('AMG_UI_CONFIG', {
  providedIn: 'root',
  factory: () => AMG_CONFIG_PADRAO
});

