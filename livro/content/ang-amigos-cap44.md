---
id: ang-amigos-cap44
order: 365
kind: capitulo
num: 261
group: Angular · Parte 67 · Conquistando a Equipe sem Conflitos de Estado
title: Se Errar, Reconheça Imediatamente
subtitle: Tratamento Resiliente de Falhas, ErrorHandler Global e Fallbacks Humanizados
minutes: 30
hue: 215
badge: 🤝
image: img/ang-amigos/cap44.svg
- Analisar a história de Dale Carnegie com o policial montado e seu cãozinho Rex no Forest Park: 
- Compreender o efeito psicológico desarmador do auto-reconhecimento imediato e sincero de falhas: 
- Implementar uma classe ErrorHandler personalizada no Angular 19 para interceptar exceções globais: 
- Criar um interceptor HTTP funcional (HttpInterceptorFn) com políticas de retry gentil e circuit breaker: 
- Projetar componentes de Fallback (Empty States e Error Screens) acolhedores que oferecem saídas dignas ao usuário: 
origem: angular/livros/03-amigos-angular/content/44-tratamento-resiliente-erros.md
---

::: ilustracao img/ang-amigos/cap44.svg | Browser exibindo tela de erro acolhedora (Empty State / Fallback Gracioso) com mensagem humanizada, botão de retry e diagnóstico técnico preservado.
:::

A apenas um minuto a pé da residência de Dale Carnegie em Nova York, estendia-se uma reserva florestal selvagem e tranquila chamada Forest Park. Carnegie costumava levar para passear ali seu pequeno cão da raça Boston Terrier, carinhosamente chamado **Rex**. Como o parque costumava estar deserto nas primeiras horas da manhã, Carnegie deixava Rex correr solto pela relva, sem coleira e sem focinheira.

Certa manhã, um policial montado em um cavalo vistoso surgiu de trás de uma colina e repreendeu Carnegie com aspereza: *"O que significa isto? O senhor não sabe que é contra a lei deixar um cão solto sem focinheira neste parque? Ele poderia morder uma criança ou matar um esquilo! Da próxima vez que eu flagrá-lo sem coleira, terei que levá-lo perante o juiz!"*.

Carnegie pediu desculpas com submissão e prometeu obedecer. Mas Rex odiava a focinheira. Poucos dias depois, vendo o parque vazio novamente, Carnegie soltou o cãozinho. Minutos depois, ao olhar para a colina, avistou o mesmo policial trotando em sua direção.

Carnegie sabia que estava em apuros. O que a maioria das pessoas faria naquela situação? Tentaria arrumar desculpas esfarrapadas: *"O senhor não viu que ele não estava fazendo mal a ninguém?"* ou *"Ele é tão mansinho..."*.

Carnegie decidiu fazer exatamente o oposto: **antecipou-se à repreensão, assumiu a culpa absoluta e atacou a si mesmo com rigor implacável**. Antes mesmo de o policial abrir a boca, Carnegie aproximou-se e disse:

> *"Guarda, o senhor me pegou em flagrante. Sou culpado. Não tenho desculpas nem álibis. O senhor me avisou na semana passada que me multaria se me visse aqui de novo com o cachorro solto."*

O efeito sobre o policial foi estarrecedor. Ao ver que Carnegie não se defendia, mas se condenava voluntariamente, o policial desarmou-se por completo. Sua postura rígida desfez-se em um tom protetor e magnânimo:

> *"Ora, ora... eu compreendo que é uma tentação deixar um cachorrinho correr um pouco por aqui quando não há ninguém por perto."*

Carnegie insistiu na própria culpa: *"Mas é contra a lei!"*. O policial respondeu: *"Bem, um cachorrinho desse tamanho não faria mal a ninguém. Apenas deixe-o correr do outro lado da colina, onde eu não possa vê-lo, e esqueceremos o assunto"*.

Carnegie conclui com uma das máximas mais brilhantes de seu livro:

> *"Se nós sabemos que seremos repreendidos de qualquer maneira, não é infinitamente melhor antecipar-nos e dizer tudo aquilo que a outra pessoa está pensando sobre nós? Quando você reconhece os seus erros com presteza e entusiasmo, você desarma a agressividade do outro e desperta nele o desejo generoso de ser compreensivo e tolerante."*

---

## O Software que Tenta Esconder seus Erros

Como as aplicações web tradicionais costumam reagir quando uma requisição HTTP falha ou um banco de dados temporariamente sai do ar?

Costumam agir da pior maneira possível:
1. **A negação fria:** A tela fica congelada para sempre em um *spinner* infinito de carregamento;
2. **A acusação grosseira:** Exibem um modal vermelho com o texto: `Erro Fatal: Falha de Conexão (500 Internal Server Error) — A culpa é da sua rede`;
3. **O colapso silencioso:** O componente desmonta e a tela inteira se transforma em um retângulo em branco.

Isso é agir com covardia técnica. A aplicação finge que nada aconteceu ou joga a culpa no colo do usuário.

---

## O ErrorHandler Diplomático do Angular 19

Uma aplicação construída sob os princípios de Dale Carnegie **reconhece suas falhas imediatamente**. Ela possui um `ErrorHandler` global que acolhe a exceção, não permite que a tela congele e exibe uma interface graciosa que tranquiliza o usuário final.

Vejamos como implementar esse tratador global:

```typescript
// amg-friendly-error-handler.ts
import { ErrorHandler, Injectable, inject, isDevMode } from '@angular/core';
import { AmgTelemetryService } from './amg-telemetry.service';

@Injectable({
  providedIn: 'root'
})
export class AmgFriendlyErrorHandler implements ErrorHandler {
  private readonly telemetry = inject(AmgTelemetryService);

  handleError(error: unknown): void {
    const timestamp = new Date().toISOString();
    const mensagem = error instanceof Error ? error.message : String(error);
    const stack = error instanceof Error ? error.stack : '';

    // 1. Em desenvolvimento: exibe diagnóstico pedagógico no console
    if (isDevMode()) {
      console.group('%c🤝 [Amigos UI · Reconhecimento Gracioso de Falha]', 'color: #f59e0b; font-weight: bold;');
      console.error(`Ocorrência capturada às: ${timestamp}`);
      console.error(`Detalhes técnicos: ${mensagem}`);
      if (stack) console.debug(stack);
      console.groupEnd();
    }

    // 2. Em produção: envia telemetria silenciosa para monitoramento da equipe
    this.telemetry.registrarIncidente({
      mensagem,
      timestamp,
      url: window.location.href
    });
  }
}
```

---

## Interceptor HTTP Funcional com Retry Gentil

No Angular 19, configuramos interceptores através de funções puras (`HttpInterceptorFn`). 

Vejamos como criar um interceptor que reconhece a instabilidade de rede e tenta uma recuperação automática antes de incomodar o usuário:

```typescript
// amg-resilient-http.interceptor.ts
import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { catchError, retry, timer, throwError } from 'rxjs';

/**
 * Interceptor que acolhe falhas temporárias de rede com política de retry progressivo.
 */
export const amgResilientHttpInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    // Tenta até 2 vezes com intervalo exponencial para erros 503 ou falhas de conexão
    retry({
      count: 2,
      delay: (error: HttpErrorResponse, count: number) => {
        // Se for erro de autenticação (401/403) ou validação (400), não insiste
        if (error.status < 500 && error.status !== 0) {
          throw error;
        }
        // Espera 1s na primeira tentativa, 2s na segunda
        return timer(count * 1000);
      }
    }),
    catchError((error: HttpErrorResponse) => {
      // Reconhece a falha e formata mensagem amigável para a UI
      const erroAmigavel = {
        status: error.status,
        mensagemHumana: 'Não conseguimos carregar as informações no momento.',
        sugestao: 'Por favor, verifique sua conexão ou tente novamente em alguns instantes.',
        protocolo: `ERR-${Date.now().toString(36).toUpperCase()}`
      };

      return throwError(() => erroAmigavel);
    })
  );
};
```

---

## O Componente de Fallback Gracioso

Agora veja como renderizar a tela de falha. Em vez de assustar o usuário, servimos uma xícara de café virtual:

```typescript
// amg-error-fallback.component.ts
import { Component, ChangeDetectionStrategy, input, output } from '@angular/core';

@Component({
  selector: 'amg-error-fallback',
  standalone: true,
  changeDetection: ChangeDetectionStrategy.OnPush,
  template: `
    <div class="amg-fallback-card" role="alert">
      <div class="amg-fallback-icon" aria-hidden="true">☕</div>
      
      <h3 class="amg-fallback-title">{{ titulo() }}</h3>
      <p class="amg-fallback-desc">{{ descricao() }}</p>

      <div class="amg-fallback-actions">
        <button 
          type="button" 
          class="amg-btn-retry" 
          (click)="recarregar.emit()">
          🔄 Tentar Novamente
        </button>
      </div>

      @if (protocolo()) {
        <small class="amg-fallback-protocol">
          Código da ocorrência: <code>{{ protocolo() }}</code> · Nossa equipe técnica já foi notificada.
        </small>
      }
    </div>
  `,
  styles: [`
    .amg-fallback-card {
      padding: 32px 24px;
      border-radius: 12px;
      background: #f8fafc;
      border: 1px dashed #cbd5e1;
      text-align: center;
      max-width: 480px;
      margin: 24px auto;
    }
    .amg-fallback-icon { font-size: 36px; margin-bottom: 12px; }
    .amg-fallback-title { font-size: 1.125rem; font-weight: 700; color: #0f172a; margin: 0 0 8px 0; }
    .amg-fallback-desc { font-size: 0.875rem; color: #64748b; line-height: 1.5; margin: 0 0 20px 0; }
    .amg-btn-retry {
      padding: 8px 20px;
      background: #2563eb;
      color: #fff;
      border: none;
      border-radius: 6px;
      font-weight: 600;
      cursor: pointer;
    }
    .amg-fallback-protocol { display: block; margin-top: 16px; font-size: 0.75rem; color: #94a3b8; }
  `]
})
export class AmgErrorFallbackComponent {
  readonly titulo = input<string>('Tivemos um imprevisto passageiro');
  readonly descricao = input<string>('Não se preocupe: seus dados locais permanecem salvos e seguros.');
  readonly protocolo = input<string | null>(null);
  readonly recarregar = output<void>();
}
```

O usuário não se sente humilhado nem frustrado. Ele percebe que a aplicação assumiu a responsabilidade pela falha de antemão, acolheu o imprevisto com serenidade e ofereceu um caminho de recuperação honroso. 

Como o policial montado de Dale Carnegie, qualquer usuário se desarma e responde com benevolência a quem sabe reconhecer os próprios erros.

---

::: quiz
? Como Dale Carnegie desarmou o policial montado que o flagrou com o cão Rex solto pela segunda vez?
+ Antecipou-se e reconheceu imediatamente a própria culpa com intensidade absoluta, privando o policial da necessidade de atacá-lo.
- Fugiu a cavalo em direção ao rio Hudson.
- Tentou subornar o policial com ingressos para a ópera de Nova York.
- Alegou falsamente que o cão pertencia ao prefeito da cidade.
> Quando nos acusamos primeiro com honestidade, a outra pessoa tende a adotar uma postura de indulgência, generosidade e compaixão.

? Para que serve a classe de serviço `ErrorHandler` global do Angular?
+ Para capturar todas as exceções não tratadas disparadas em qualquer parte da aplicação, centralizando o log e evitando que a tela trave.
- Para deletar arquivos com erros de sintaxe no servidor Node.js.
- Para impedir que usuários sem plano pago cliquem em botões.
- Para enviar multas automáticas para os programadores da equipe.
> A substituição do `ErrorHandler` padrão permite monitorar erros em tempo real e fornecer tratamentos visuais amigáveis ao usuário.

? Qual é o papel da função `retry()` com atraso exponencial em requisições HTTP?
+ Tentar reenviar a requisição algumas vezes com intervalos calculados (ex: 1s, 2s) para superar instabilidades momentâneas de conexão.
- Enviar a mesma mensagem dez mil vezes para travar o servidor inimigo.
- Duplicar o saldo bancário do usuário em sistemas financeiros.
- Reiniciar o computador do cliente se o servidor responder 404.
> O retry gentil absorve pequenas oscilações de Wi-Fi e rede móvel de forma transparente para o usuário final.

? Por que telas de erro devem evitar jargões técnicos indecifráveis como "500 Internal Server NullPointer"?
+ Porque causam pânico e confusão no usuário comum, passando uma impressão de amadorismo e insegurança técnica.
- Porque a lei de proteção de dados proíbe o uso da palavra "Server".
- Porque o navegador Safari fecha a aba quando lê números maiores que 400.
- Porque caracteres técnicos consomem mais dados no plano de telefonia.
> Mensagens humanizadas tranquilizam o usuário, explicam o que aconteceu em linguagem acessível e oferecem saídas claras (como tentar de novo).

? O que um bom componente de Fallback (como `AmgErrorFallbackComponent`) deve fornecer?
+ Mensagem empática, botão de nova tentativa e código de ocorrência anônimo para suporte técnico sem exposição de stack traces.
- Um formulário para o usuário preencher seu número de cartão de crédito.
- Um cronômetro de contagem regressiva para assustar quem está lendo.
- Um link para o site de um concorrente.
> Uma tela de erro bem desenhada acolhe a frustração do usuário e oferece os recursos necessários para que ele retome sua tarefa com dignidade.
:::

---

::: cartas
O Policial no Forest Park :: Relato histórico onde a autocrítica imediata de Dale Carnegie desarmou a hostilidade de uma autoridade comovida.
Reconhecer Erros :: Princípio de Dale Carnegie: se você estiver errado, reconheça o fato com presteza, entusiasmo e coragem.
ErrorHandler Global :: Interface nativa do Angular para interceptação centralizada de todas as falhas em tempo de execução.
HttpInterceptorFn :: Função interceptora do Angular moderno que intercepta requisições e respostas HTTP para tratamento de resiliência.
Retry Exponencial :: Padrão de recuperação que reaplica tentativas de rede espaçadas no tempo para vencer falhas efêmeras.
Empty State / Fallback :: Componente visual que acolhe estados vazios ou de falha oferecendo saídas práticas e apoio ao usuário.
:::
