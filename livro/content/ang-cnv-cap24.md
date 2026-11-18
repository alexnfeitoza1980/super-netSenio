---
id: ang-cnv-cap24
order: 343
kind: capitulo
num: 241
group: Angular · Parte 60 · Bloco II — Os Quatro Pilares da Interface
title: Assumindo a Responsabilidade pelos Nossos Sentimentos
subtitle: HttpInterceptorFn, HttpContext, ErrorHandler Global e a Filosofia de Epicteto
minutes: 30
hue: 32
badge: 🛡️
image: img/ang-cnv/cap24.svg
- Analisar a citação do filósofo estoico Epicteto e as 4 opções ao ouvir uma mensagem negativa: 
- Compreender os 3 estágios do desenvolvimento emocional descritos por Marshall Rosenberg: 
- Implementar interceptores HTTP funcionais com HttpInterceptorFn e tokens de contexto HttpContext: 
- Construir um ErrorHandler global no Angular 17+ que acolhe falhas sem acusar o usuário: 
origem: angular/livros/02-cnv-angular/content/24-assumindo-responsabilidade.md
---

::: ilustracao img/ang-cnv/cap24.svg | Pipeline de Rede em Alta Fidelidade: o estímulo externo hostil (500 Server Error) é acolhido pelo HttpInterceptorFn, higienizado e transformado em assistência compassiva na interface.
:::

No quinto capítulo de *Comunicação Não-Violenta*, Marshall Rosenberg nos apresenta a uma das mais libertadoras — e ao mesmo tempo desconcertantes — premissas de toda a sua vida:

> *"O que as outras pessoas dizem ou fazem pode ser o estímulo para os nossos sentimentos, mas nunca a causa."*

Marshall abre o capítulo resgatando as palavras imortais do filósofo estoico grego Epicteto (século I d.C.):
> *"As pessoas não são perturbadas pelas coisas que acontecem, mas pela maneira como elas as veem."*

Quando alguém nos dirige uma mensagem hostil, ríspida ou negativa (por exemplo: *"Você é a pessoa mais egoísta e desatenta que já conheci!"*), Marshall explica que nosso cérebro tem **quatro opções de como receber essa mensagem**:

```
                                  [ MENSAGEM NEGATIVA RECEBIDA ]
                                                │
         ┌───────────────────────┬──────────────┴───────────────┬───────────────────────┐
         ▼                       ▼                              ▼                       ▼
  1. CULPAR A NÓS MESMOS   2. CULPAR OS OUTROS       3. ILUMINAR NOSSOS PRÓPRIOS  4. ILUMINAR OS SENTIMENTOS
     "Eu sou um desastre,     "Você é um grosseiro      SENTIMENTOS E NECESSIDADES   E NECESSIDADES DO OUTRO
     a culpa é toda minha"    que não entende nada"     "Fico triste, pois preciso   "Você está irritado porque
                                                         de reconhecimento"           precisa de pontualidade?"
```

1. **Opção 1: Culpar a nós mesmos**: engolir o veneno, aceitar o julgamento alheio como verdade absoluta e afundar em culpa, autoflagelação e vergonha.
2. **Opção 2: Culpar os outros**: contra-atacar com raiva, apontar o dedo de volta e devolver a violência em dobro.
3. **Opção 3: Perceber nossos próprios sentimentos e necessidades**: reconhecer que o que dói em nós não são as palavras do outro, mas a nossa própria necessidade de valorização que não está sendo atendida.
4. **Opção 4: Perceber os sentimentos e necessidades do outro**: colocar as "orelhas de girafa" e escutar o grito de socorro que está por trás do ataque: *"Essa pessoa está sofrendo e precisa de clareza."*

---

## Os 3 Estágios do Desenvolvimento Emocional na CNV

Marshall relata que, no processo de aprender a assumir a responsabilidade pelos próprios sentimentos, os seres humanos atravessam três estágios universais:

- **Estágio 1: A Escravidão Emocional**: acreditamos que somos pessoalmente responsáveis pela felicidade de todos ao nosso redor. Se alguém estiver triste ou frustrado, sentimo-nos culpados e nos anulamos para agradar o outro.
- **Estágio 2: O Estágio Ranzinza (Rebeldia)**: percebemos o preço absurdo da escravidão emocional e nos tornamos agressivos e desdenhosos: *"O problema é seu! Não sou responsável por você!"*. Sentimos raiva toda vez que alguém expressa uma dor.
- **Estágio 3: A Libertação Emocional**: assumimos plena autorresponsabilidade por nossos sentimentos e necessidades, ao mesmo tempo em que nos importamos com o outro por compaixão autêntica e liberdade. Respondemos às necessidades alheias por amor, e nunca por medo, culpa ou obrigação.

---

## O Estímulo Externo na Rede: A Queda da API

No universo da engenharia de software com Angular, o que acontece quando uma requisição HTTP retorna um status `500 Internal Server Error` ou `504 Gateway Timeout`?
- **O que é o fato?** Um estímulo físico externo: o servidor caiu, a conexão oscilou ou o banco de dados atingiu o limite de memória.
- **O que a maioria das aplicações faz?** Reagem no Estágio 2 (Ranzinza): cospem um erro vermelho na tela e gritam com o usuário: *"Erro 500 do usuário: sua solicitação foi recusada!"*.

A interface se recusa a assumir a autorresponsabilidade. Ela culpa o usuário por uma falha de infraestrutura que ela própria deveria acolher com maturidade técnica.

---

## A Camada de Cuidado: HttpInterceptorFn (angular.dev)

No Angular moderno em [angular.dev/overview](https://angular.dev/overview), todas as requisições e respostas de rede passam pela esteira de **Interceptores Funcionais (`HttpInterceptorFn`)**.

O interceptor atua como a mente serena de Epicteto: ele acolhe o estímulo negativo que veio do servidor e o traduz com sabedoria antes que ele toque a interface:

```typescript
// src/app/core/interceptors/empatia-rede.interceptor.ts
import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { NotificadorEmpatiaService } from '../services/notificador-empatia.service';
import { PULAR_ACOLHIMENTO_GLOBAL } from './http-context-tokens';

export const empatiaRedeInterceptor: HttpInterceptorFn = (req, next) => {
  const notificador = inject(NotificadorEmpatiaService);

  // 1. Verifica se a requisição possui um token de contexto pedindo para ignorar o handler
  const devePular = req.context.get(PULAR_ACOLHIMENTO_GLOBAL);

  // 2. Clone imutável da requisição para injetar cabeçalhos de respeito mútuo
  const reqCompassiva = req.clone({
    setHeaders: {
      'X-Cliente-Intencao': 'dialogo-cooperativo',
      'Accept': 'application/json'
    }
  });

  return next(reqCompassiva).pipe(
    catchError((erro: HttpErrorResponse) => {
      if (devePular) {
        return throwError(() => erro);
      }

      // 3. Tradução compassiva dos códigos de erro da rede
      let mensagemTraduzida = 'Identificamos uma oscilação na conexão.';
      let acaoSugerida = 'Tentar novamente em instantes.';

      switch (erro.status) {
        case 0:
          mensagemTraduzida = 'Parece que sua conexão com a internet oscilou. Todas as suas alterações estão protegidas localmente.';
          acaoSugerida = 'Aguardar a internet restabelecer.';
          break;
        case 400:
          mensagemTraduzida = 'Alguns dados enviados precisam de ajuste mútuo para que possamos avançar com segurança.';
          acaoSugerida = 'Revisar os campos destacados.';
          break;
        case 401:
          mensagemTraduzida = 'Sua chave de acesso seguro expirou após um período de inatividade.';
          acaoSugerida = 'Fazer login novamente para proteger seus dados.';
          break;
        case 403:
          mensagemTraduzida = 'Esse espaço de diálogo requer permissões adicionais que seu perfil atual não possui.';
          acaoSugerida = 'Solicitar acesso fraterno ao gestor da sala.';
          break;
        case 404:
          mensagemTraduzida = 'O recurso ou diálogo procurado não foi localizado no momento.';
          acaoSugerida = 'Verificar se o endereço foi atualizado.';
          break;
        case 500:
        case 502:
        case 503:
          mensagemTraduzida = 'Nossos servidores estão enfrentando alta demanda momentânea. A equipe técnica já foi avisada.';
          acaoSugerida = 'Aguardar alguns segundos; realizaremos novas tentativas serenamente.';
          break;
      }

      // Comunica com o serviço de acolhimento visual
      notificador.exibirBannerAcolhedor({
        statusHttp: erro.status,
        mensagemHumana: mensagemTraduzida,
        acaoOrientada: acaoSugerida
      });

      // Repassa o erro tipado para a camada de serviço consumidora
      return throwError(() => new Error(mensagemTraduzida));
    })
  );
};
```

---

## O Poder do HttpContext e HttpContextToken

Como passar instruções específicas para a cadeia de interceptores sem poluir a URL ou o corpo da requisição? O Angular oferece o **`HttpContext`**:

```typescript
// src/app/core/interceptors/http-context-tokens.ts
import { HttpContextToken } from '@angular/common/http';

/**
 * Token que permite a uma chamada específica dispensar o tratamento
 * global (por exemplo, em verificações silenciosas de ping em background).
 */
export const PULAR_ACOLHIMENTO_GLOBAL = new HttpContextToken<boolean>(() => false);
```

Para consumir esse token em um serviço:

```typescript
import { HttpClient, HttpContext } from '@angular/common/http';
import { PULAR_ACOLHIMENTO_GLOBAL } from './http-context-tokens';

verificarPingSilencioso() {
  return this.http.get('/api/ping', {
    context: new HttpContext().set(PULAR_ACOLHIMENTO_GLOBAL, true)
  });
}
```

---

## O Guardião Supremo: Global ErrorHandler

E se um componente lançar um erro imprevisível em tempo de execução (como um cálculo matemático que resulta em divisão por zero ou um objeto inesperadamente corrompido)?

O Angular possui uma classe centralizadora chamada **`ErrorHandler`**. Por padrão, ela apenas joga o erro em vermelho no console. Nós a customizamos para acolher a falha sem permitir que a aplicação desmorone:

```typescript
// src/app/core/handlers/global-error-handler.ts
import { ErrorHandler, Injectable, inject, NgZone } from '@angular/core';
import { NotificadorEmpatiaService } from '../services/notificador-empatia.service';

@Injectable()
export class CompassivoErrorHandler implements ErrorHandler {
  private readonly notificador = inject(NotificadorEmpatiaService);
  private readonly zone = inject(NgZone);

  handleError(error: unknown): void {
    const detalhe = error instanceof Error ? error.message : String(error);

    // Registra silenciosamente para a telemetria da equipe de engenharia
    console.warn('[Acolhimento Global de Exceção]', error);

    // Executa na zona do Angular para garantir que a UI reflita a notificação de apoio
    this.zone.run(() => {
      this.notificador.exibirModalRecuperacao({
        titulo: 'Espaço de Conexão Preservado',
        mensagem: 'Ocorreu uma instabilidade imprevista, mas suas reflexões continuam a salvo.',
        detalheTecnico: detalhe
      });
    });
  }
}
```

No `app.config.ts`, registramos o provedor com simplicidade:

```typescript
import { ApplicationConfig, ErrorHandler } from '@angular/core';
import { provideHttpClient, withInterceptors, withFetch } from '@angular/common/http';
import { empatiaRedeInterceptor } from './core/interceptors/empatia-rede.interceptor';
import { CompassivoErrorHandler } from './core/handlers/global-error-handler';

export const appConfig: ApplicationConfig = {
  providers: [
    provideHttpClient(
      withFetch(), // Usa a moderna Fetch API do navegador para máxima performance
      withInterceptors([empatiaRedeInterceptor])
    ),
    { provide: ErrorHandler, useClass: CompassivoErrorHandler }
  ]
};
```

::: nota
Observe o uso de `withFetch()` dentro de `provideHttpClient()`. Introduzido recentemente no Angular, ele substitui a antiga implementação baseada em `XMLHttpRequest` pela nativa `fetch()` do navegador, viabilizando streaming de respostas, suporte superior a Service Workers e compatibilidade nativa com SSR.
:::

---

::: quiz
? Qual é a tese central de Epicteto resgatada por Marshall Rosenberg no capítulo sobre autorresponsabilidade?
- Que o destino de todos os seres humanos já está traçado pelas estrelas
+ Que as pessoas não são perturbadas pelas coisas que acontecem no mundo exterior, mas pela maneira como escolhem interpretá-las e reagir a elas
- Que a dor física não existe de verdade
- Que os computadores governam as emoções humanas
> Epicteto e Marshall ensinam que o estímulo externo (a queda da API ou uma crítica alheia) não tem o poder de ditar nossos sentimentos; como processamos e acolhemos o evento é uma escolha de autorresponsabilidade nossa.

? Quais são os três estágios do desenvolvimento emocional identificados por Marshall Rosenberg?
- Sono, Vigília e Meditação
+ Escravidão Emocional, Estágio Ranzinza (Rebelde) e Libertação Emocional
- Entrada, Processamento e Saída de Dados
- Infância, Adolescência e Aposentadoria
> O ser humano evolui de sentir-se culpado por tudo (escravidão), para rejeitar a dor alheia com frieza (ranzinza), até atingir a compaixão livre e autônoma (libertação emocional).

? Como o `HttpContext` e o `HttpContextToken` auxiliam na comunicação entre serviços e interceptores no Angular?
- Permitindo que a senha do usuário seja gravada em arquivos de texto
+ Permitindo passar metadados e configurações tipadas customizadas (como flags para ignorar tratamentos de erro padrão) diretamente na requisição HTTP
- Fazendo com que o Angular rode em qualquer sistema operacional
- Reduzindo o número de cliques que o usuário precisa dar
> O `HttpContext` é o canal oficial de transmissão de parâmetros de controle entre o local que faz a chamada HTTP e a esteira de interceptores que a processa.

? Qual é a função da classe `ErrorHandler` no Angular?
- Interromper a compilação do TypeScript se houver erros de sintaxe
+ Capturar e tratar de forma centralizada todas as exceções não capturadas lançadas durante a execução da aplicação
- Excluir o banco de dados caso ocorra uma falha grave
- Enviar mensagens de correio de voz para o desenvolvedor
> O `ErrorHandler` customizado atua como uma rede de proteção de última instância, impedindo que a aplicação trave em uma tela branca ao sofrer uma falha inesperada.

? Por que o uso de `withFetch()` é recomendado na configuração moderna de `provideHttpClient()`?
- Porque ele desativa todas as checagens de CORS
+ Porque utiliza a moderna Fetch API do navegador, habilitando melhor performance, compatibilidade avançada com SSR e recursos de streaming de rede
- Porque faz com que as requisições não gastem dados móveis
- Porque impede que hackers acessem a API
> A Fetch API é o padrão moderno da web para comunicação de rede, oferecendo uma arquitetura mais limpa e otimizada do que a legada XMLHttpRequest.
:::

---

::: cartas
Autorresponsabilidade pelos Sentimentos :: Compreensão da CNV de que as ações alheias são apenas o estímulo, mas a causa de nossas emoções são nossas próprias necessidades.
Epicteto :: Filósofo estoico grego do século I cuja máxima ensina que o julgamento sobre os fatos é o que perturba a mente humana.
Estágio da Escravidão Emocional :: Fase em que nos sentimos pessoalmente responsáveis pela felicidade e pelo sofrimento de todos ao nosso redor.
Estágio da Libertação Emocional :: Fase madura onde nos responsabilizamos por nossos próprios atos e cuidamos dos outros por pura compaixão livre.
HttpInterceptorFn :: Função interceptora do Angular que trata e transforma requisições e respostas de rede de forma composível.
HttpContextToken :: Identificador tipado para anexar metadados específicos de controle a uma requisição HTTP individual.
CompassivoErrorHandler :: Implementação customizada de ErrorHandler que acolhe exceções do sistema sem desamparar nem culpar o usuário.
withFetch() :: Configuração moderna do HttpClient no Angular que adota a Fetch API nativa da plataforma web.
:::
