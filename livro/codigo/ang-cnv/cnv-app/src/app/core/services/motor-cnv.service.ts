import { Injectable, signal, computed } from '@angular/core';
import { ResultadoTraducao, PilarCNV } from '../models/traducao-cnv.model';

@Injectable({
  providedIn: 'root'
})
export class MotorCnvService {
  private readonly _historico = signal<ResultadoTraducao[]>([]);

  readonly historico = this._historico.asReadonly();
  readonly totalTraducoes = computed(() => this._historico().length);
  readonly ultimaTraducao = computed(() => this._historico()[0] ?? null);

  traduzir(textoOriginal: string): ResultadoTraducao {
    const texto = textoOriginal.trim();
    const tensaoCalculada = this.estimarTensao(texto);

    const obs = this.extrairObservacao(texto);
    const sent = this.identificarSentimento(texto, tensaoCalculada);
    const nec = this.mapearNecessidade(texto);
    const ped = this.formularPedido(obs.conteudo, sent.conteudo, nec.conteudo);

    const resultado: ResultadoTraducao = {
      id: crypto.randomUUID(),
      textoOriginal: texto,
      nivelTensaoOriginal: tensaoCalculada,
      observacao: obs,
      sentimento: sent,
      necessidade: nec,
      pedido: ped,
      respostaSugerida: `Quando ${obs.conteudo.toLowerCase()}, eu me sinto ${sent.conteudo.toLowerCase()} porque preciso de ${nec.conteudo.toLowerCase()}. ${ped.conteudo}`,
      dataHora: new Date()
    };

    this._historico.update(lista => [resultado, ...lista.slice(0, 9)]);
    return resultado;
  }

  limparHistorico(): void {
    this._historico.set([]);
  }

  private estimarTensao(texto: string): number {
    let tensao = 4;
    const termosHostis = ['nunca', 'sempre', 'incompetente', 'absurdo', 'culpa', 'burrice', 'odeio', 'atrasou', 'inaceitável'];
    const lower = texto.toLowerCase();

    for (const termo of termosHostis) {
      if (lower.includes(termo)) tensao += 1.5;
    }
    if (texto.includes('!')) tensao += 1;
    if (texto === texto.toUpperCase() && texto.length > 10) tensao += 2;

    return Math.min(10, Math.round(tensao));
  }

  private extrairObservacao(texto: string): PilarCNV {
    return {
      nome: 'Observação',
      icone: '👁️',
      conteudo: 'Houve uma divergência em relação a prazos ou entregas combinadas nas tarefas.',
      orientacao: 'Fatos objetivos e neutros sem generalizações como "sempre" ou "nunca".'
    };
  }

  private identificarSentimento(texto: string, tensao: number): PilarCNV {
    const sentimento = tensao >= 7 ? 'Profunda Insegurança e Frustração' : 'Preocupação e Cansaço';
    return {
      nome: 'Sentimento',
      icone: '💓',
      conteudo: sentimento,
      orientacao: 'Vulnerabilidade humana e estado emocional sem acusações moralizantes.'
    };
  }

  private mapearNecessidade(texto: string): PilarCNV {
    return {
      nome: 'Necessidade',
      icone: '🌱',
      conteudo: 'Previsibilidade, Clareza no Alinhamento e Respeito Mútuo',
      orientacao: 'Valores humanos universais que buscam ser atendidos.'
    };
  }

  private formularPedido(obs: string, sent: string, nec: string): PilarCNV {
    return {
      nome: 'Pedido',
      icone: '🤝',
      conteudo: 'Você estaria disposto a reservar 15 minutos em nosso calendário amanhã às 10h para alinharmos as prioridades?',
      orientacao: 'Ação positiva, realizável e concreta, aberta à cooperação voluntária.'
    };
  }
}

