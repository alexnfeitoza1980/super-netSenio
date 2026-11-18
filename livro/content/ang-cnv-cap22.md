---
id: ang-cnv-cap22
order: 341
kind: capitulo
num: 239
group: Angular · Parte 60 · Bloco II — Os Quatro Pilares da Interface
title: Observar sem Avaliar
subtitle: Signals, Grafo de Reatividade, linkedSignal e resource() no Angular.dev
minutes: 30
hue: 32
badge: 👁️
image: img/ang-cnv/cap22.svg
- Analisar a distinção entre fatos observáveis e julgamentos interpretativos no poema de Marshall Rosenberg: 
- Dominar a mecânica interna do Grafo de Signals do Angular (Push/Pull, Glitch-Free e Memoization): 
- Implementar computações puras com computed() e efeitos de borda com effect() e untracked(): 
- Explorar as novas primitivas do Angular 18 e 19: linkedSignal(), resource() e rxResource()
origem: angular/livros/02-cnv-angular/content/22-observar-sem-avaliar.md
---

::: ilustracao img/ang-cnv/cap22.svg | Chrome DevTools exibindo a inspeção em tempo real do Grafo de Reatividade dos Signals: Produtores (WritableSignal) ➔ Visões (.asReadonly) ➔ Derivações Puras (computed).
:::

No terceiro capítulo de *Comunicação Não-Violenta*, Marshall Rosenberg nos convida a enfrentar o primeiro e mais exigente pilar de seu método: **separar a observação da avaliação**.

Marshall abre o capítulo com as palavras contundentes do teólogo e romancista Frederick Buechner:
> *"OBSERVEM!!! Há poucas coisas tão importantes, tão religiosas, quanto isso."*

E nos presenteia com um de seus poemas mais emblemáticos, declamado em salas de conferência e tribunais pelo mundo afora:

> *"Posso lidar com você me dizendo o que fiz ou deixei de fazer.*  
> *E posso lidar com suas interpretações.*  
> *Mas, por favor, não misture as duas coisas.*  
>  
> *Se você quer deixar qualquer assunto confuso,*  
> *Posso lhe dizer como fazer:*  
> *Misture o que eu faço*  
> *Com a maneira que você reage a isso.*  
>  
> *Diga-me que você está decepcionada*  
> *Com as tarefas inacabadas que você vê,*  
> *Mas me chamar de 'irresponsável'*  
> *Não é um modo de me motivar.*  
>  
> *E me diga que fica magoada*  
> *Quando digo 'não' às suas aproximações,*  
> *Mas me chamar de um homem 'frígido'*  
> *Não vai melhorar suas chances.*  
>  
> *Sim, posso lidar com você me dizendo o que fiz ou deixei de fazer.*  
> *E posso lidar com suas interpretações.*  
> *Mas, por favor, não misture as duas coisas."*

Marshall apresenta um guia prático para identificar a contaminação da observação por avaliações moralizantes:

```
+---------------------------------------------------------------------------------------------------------+
|                               DISTINÇÃO ENTRE OBSERVAÇÃO E AVALIAÇÃO                                    |
+----------------------------------------------------+----------------------------------------------------+
| AVALIAÇÃO MISTURADA COM OBSERVAÇÃO (TÓXICA)        | OBSERVAÇÃO PURA DE FATOS (COMPASSIVA)              |
+----------------------------------------------------+----------------------------------------------------+
| "Você é desorganizado e preguiçoso no trabalho."  | "Nas últimas três semanas, duas entregas foram     |
|                                                    | enviadas após o prazo acordado no sprint."         |
| "Aquele desenvolvedor sênior é arrogante."         | "Na reunião de hoje, ele interrompeu a fala da     |
|                                                    | colega júnior quatro vezes."                       |
| "Essa API é horrível e nunca funciona direito."    | "Em 100 requisições realizadas hoje, 12 retornaram |
|                                                    | status HTTP 504 Gateway Timeout."                  |
| "Você sempre ignora meus comentários no PR."       | "Enviei três comentários no seu pull request ontem |
|                                                    | e até o momento não houve respostas neles."        |
+----------------------------------------------------+----------------------------------------------------+
```

Quando misturamos avaliação com observação, o interlocutor ouve **crítica**. E, ao ouvir crítica, o cérebro humano entra instantaneamente em modo de sobrevivência: contra-atacar ou se defender. O diálogo colaborativo é aniquilado.

---

## O Grafo de Reatividade do Angular: Separando o Fato da Computação

No desenvolvimento com Angular em [angular.dev/overview](https://angular.dev/overview), os **Signals** representam a manifestação mais perfeita dessa filosofia:

- **O Fato Puro** reside em um `WritableSignal` (`signal<T>()`). Ele contém apenas o estado físico do mundo naquele momento.
- **A Interpretação Factual** reside em um `Signal` derivado puro (`computed<T>()`). Ele deriva matematicamente uma nova informação sem jamais alterar o fato que o originou.
- **A Proteção do Fato** é garantida via `.asReadonly()`.

```
                    [ Fato Externo: Evento ou Resposta de Rede ]
                                          │
                                          ▼
                         [ _medicoes = signal<Metrica[]> ]  (Fato Puro)
                                          │
                   ┌──────────────────────┴──────────────────────┐
                   │                                             │
                   ▼ .asReadonly()                               ▼ computed()
        [ medicoes (Somente-Leitura) ]               [ latenciaMedia (Derivação Pura) ]
                   │                                             │
                   └──────────────────────┬──────────────────────┘
                                          ▼
                             [ Renderização Serena na UI ]
```

---

## A Arquitetura Interna: Push-Pull e Algoritmo Glitch-Free

Como o Angular garante que computações não vejam estados intermediários inválidos (*glitches*)?

O motor de reatividade do Angular utiliza um grafo direcionado acíclico de nós produtores e consumidores. Quando um sinal gravável é alterado:
1. **Fase Push (Notificação de Sujeira)**: o nó produtor notifica seus dependentes dizendo: *"Eu mudei; seus valores atuais estão potencialmente desatualizados (dirty)"*. Nenhuma computação pesada é feita nessa fase.
2. **Fase Pull (Avaliação Sob Demanda)**: quando o template ou um efeito precisa ler o valor de um `computed()`, ele percorre o grafo e recalcula o valor **apenas se algum de seus produtores de fato tiver sido alterado**. Se o valor final for idêntico ao anterior (verificado pela função `equal`), a propagação é interrompida imediatamente (*memoization*).

Esse determinismo garante que o sistema nunca caia em estados inconsistentes ou execute computações redundantes.

---

## Implementação Completa: O Serviço de Observabilidade Neutra

Vejamos uma implementação com TypeScript estrito, demonstrando `signal`, `computed`, `effect` com `untracked()` e `onCleanup`:

```typescript
// src/app/core/services/telemetria-neutra.service.ts
import { Injectable, signal, computed, effect, untracked } from '@angular/core';

export interface EventoFactual {
  readonly id: string;
  readonly milissegundosResposta: number;
  readonly statusHttp: number;
  readonly timestamp: Date;
}

@Injectable({
  providedIn: 'root'
})
export class TelemetriaNeutraService {
  // 1. O Fato Puro (Privado e Encapsulado)
  private readonly _eventos = signal<EventoFactual[]>([], {
    // Função de igualdade customizada (opcional)
    equal: (anterior, atual) => anterior.length === atual.length && anterior === atual
  });

  // 2. Visão Exposta: Observação protegida contra mutação externa
  readonly eventos = this._eventos.asReadonly();

  // 3. Computações Puras (Derivações sem efeitos colaterais)
  readonly totalEventos = computed(() => this._eventos().length);

  readonly mediaMilissegundos = computed(() => {
    const lista = this._eventos();
    if (lista.length === 0) return 0;
    const soma = lista.reduce((acc, ev) => acc + ev.milissegundosResposta, 0);
    return Math.round(soma / lista.length);
  });

  readonly taxaSucessoPercentual = computed(() => {
    const lista = this._eventos();
    if (lista.length === 0) return 100;
    const sucessos = lista.filter(e => e.statusHttp >= 200 && e.statusHttp < 400).length;
    return Math.round((sucessos / lista.length) * 100);
  });

  constructor() {
    // 4. Efeito de borda: Sincronização externa com untracked e onCleanup
    effect((onCleanup) => {
      // Registra a dependência reativa do totalEventos
      const total = this.totalEventos();
      
      // Lê a média de forma 'untracked' (sem criar dependência de reexecução se a média mudar)
      const media = untracked(() => this.mediaMilissegundos());

      console.info(`[Telemetria Factual] Total de amostras: ${total} | Média atual: ${media}ms`);

      // Função de limpeza do efeito: chamada antes da próxima execução ou na destruição
      onCleanup(() => {
        // Exemplo: cancelar timers ou limpar buffers pendentes
      });
    });
  }

  // Ação Factual de Registro
  registrarEvento(ms: number, statusHttp: number): void {
    const novoFato: EventoFactual = {
      id: crypto.randomUUID(),
      milissegundosResposta: ms,
      statusHttp,
      timestamp: new Date()
    };

    // Atualização imutável do sinal gravável
    this._eventos.update(atuais => [...atuais.slice(-49), novoFato]);
  }
}
```

---

## O Futuro no Angular 18 e 19: linkedSignal e resource()

Para expandir ainda mais a pureza reativa, o ecossistema do Angular introduziu duas primitivas revolucionárias:

### 1. `linkedSignal()`: Estado Local Sincronizado mas Sobrescrevível
Quantas vezes você precisou de um valor que depende de um sinal pai (por exemplo, a opção selecionada de uma lista), mas que o usuário pode alterar localmente na tela? Antes, sincronizar isso exigia efeitos complexos. O `linkedSignal()` resolve isso nativamente:

```typescript
import { linkedSignal } from '@angular/core';

// Quando o usuário selecionado mudar, a preferência redefine para a padrão daquele usuário,
// mas o componente local pode chamar `preferenciaTema.set('escuro')` livremente!
readonly usuarioSelecionado = signal<Usuario>(usuarioPadrao);
readonly preferenciaTema = linkedSignal({
  source: this.usuarioSelecionado,
  computation: (user) => user.temaPreferido
});
```

### 2. `resource()` / `rxResource()`: Carregamento Assíncrono Orientado a Sinais
Substitui a necessidade de orquestrar manualmente variáveis `isLoading`, `data` e `error` ao chamar APIs:

```typescript
import { resource } from '@angular/core';

// resource() executa o loader automaticamente quando seu request signal mudar!
readonly idConversa = signal('conversa-123');

readonly conversaResource = resource({
  request: () => ({ id: this.idConversa() }),
  loader: async ({ request, abortSignal }) => {
    const res = await fetch(`/api/conversas/${request.id}`, { signal: abortSignal });
    if (!res.ok) throw new Error('Não foi possível carregar a conversa.');
    return res.json() as Promise<Conversa>;
  }
});

// Acessíveis diretamente no template:
// conversaResource.value() -> Os dados carregados
// conversaResource.isLoading() -> Se está em carregamento
// conversaResource.error() -> Qualquer erro ocorrido
```

::: nota
Com `resource()` e `rxResource()`, a observação de dados remotos se torna declarativa. Você não precisa disparar chamadas em `ngOnInit` nem gerenciar subscrições manuais; o próprio sinal de requisição governa o ciclo de vida da busca assíncrona.
:::

---

::: quiz
? Qual é a consequência psicológica quando misturamos uma avaliação pessoal com uma observação factual, segundo Marshall Rosenberg?
- As pessoas acham você inteligente e concordam imediatamente
+ O ouvinte escuta uma crítica pessoal e adota uma postura defensiva ou de contra-ataque, encerrando o diálogo cooperativo
- A conversa se torna duas vezes mais rápida
- A pessoa se sente profundamente compreendida
> No poema de Marshall ("Mas por favor, não misture as duas coisas"), fica claro que rótulos como "irresponsável" ou "preguiçoso" fecham os canais de empatia, enquanto fatos objetivos convidam ao alinhamento mútuo.

? O que caracteriza o algoritmo de reatividade Glitch-Free dos Signals do Angular?
- Ele desliga o monitor se houver um erro de renderização
+ Ele divide a atualização em duas fases (Push de sujeira e Pull sob demanda), garantindo que nós dependentes nunca vejam estados intermediários inconsistentes
- Ele converte todos os dados em números inteiros
- Ele impede que o usuário clique duas vezes no mouse
> Glitch-free significa ausência de inconsistências temporárias: todos os nós computados são atualizados em ordem topológica precisa antes de serem apresentados ao DOM.

? Para que serve a função `untracked()` dentro de um `effect()` no Angular?
- Para apagar o histórico de navegação
+ Para ler o valor de um Signal dentro do efeito sem registrar uma dependência reativa nele, evitando que mudanças nesse sinal reexecutem o efeito
- Para desabilitar o TypeScript
- Para acelerar a conexão Wi-Fi
> O `untracked()` permite consultar dados de contexto (como uma média ou configuração) de dentro de um efeito sem que qualquer oscilação daquele dado acione o efeito novamente.

? Qual problema moderno a nova primitiva `linkedSignal()` do Angular resolve?
- Faz a conexão física com o cabo de rede
+ Gerencia um sinal gravável cujo valor padrão deriva de outro sinal fonte, mas que pode ser sobrescrito localmente pela interface sem quebrar a reatividade
- Substitui o motor V8 do Google Chrome
- Converte imagens PNG em vetores SVG
> `linkedSignal` é a resposta oficial do Angular para o padrão comum de estados derivados que necessitam de resets automáticos quando a fonte muda, mas admitem mutação local temporária.

? Como a nova API `resource()` simplifica requisições HTTP na arquitetura de Signals?
- Ela bloqueia todas as chamadas para servidores externos
+ Ela encapsula o estado assíncrono completo (value, isLoading, error) de forma reativa a partir de um sinal de parâmetros, com suporte nativo a cancelamento via AbortSignal
- Ela deleta o backend e cria um banco local
- Ela permite rodar Angular em televisores antigos
> `resource()` elimina dezenas de linhas de boilerplate de gerenciamento de carregamento e erros, ligando a intenção da requisição diretamente à reatividade dos Signals.
:::

---

::: cartas
Observação sem Avaliação :: Primeiro pilar da CNV: relatar o que aconteceu sem incluir interpretações, diagnósticos moralizantes ou generalizações.
Grafo de Signals :: Estrutura de dados em grafo direcionado acíclico que conecta produtores e consumidores de reatividade no Angular.
Glitch-Free Reactivity :: Propriedade matemática que assegura que nenhum nó consumidor leia um estado intermediário inconsistente durante uma atualização.
untracked() :: Função que lê o valor de um sinal dentro de um contexto reativo sem registrar dependência nele.
linkedSignal() :: Primitiva do Angular 18+ que cria um sinal gravável vinculado a uma fonte reativa, admitindo sobrescrita local.
resource() :: Primitiva do Angular 19 para carregamento assíncrono declarativo acionado diretamente por parâmetros reativos.
Frederick Buechner :: Autor citado por Marshall Rosenberg: "OBSERVEM!!! Há poucas coisas tão importantes, tão religiosas, quanto isso."
:::
