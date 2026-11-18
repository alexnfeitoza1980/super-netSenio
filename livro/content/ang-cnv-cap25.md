---
id: ang-cnv-cap25
order: 344
kind: capitulo
num: 242
group: Angular · Parte 60 · Bloco II — Os Quatro Pilares da Interface
title: O Pedido que Enriquece a Vida
subtitle: HttpClient, DTOs Tipados, Typed Forms e Pedidos vs Exigências
minutes: 30
hue: 32
badge: 🎯
image: img/ang-cnv/cap25.svg
- Analisar a canção infantil de Ruth Bebermeyer ("Como é que você faz um 'Não faça'?"): 
- Compreender a anedota de Marshall Rosenberg sobre o pedido negativo e o torneio de golfe: 
- Implementar contratos tipados com DTOs em TypeScript e chamadas REST com HttpClient: 
- Utilizar Typed Forms no Angular para assegurar segurança estrita em tempo de compilação: 
origem: angular/livros/02-cnv-angular/content/25-pedindo-com-clareza.md
---

::: ilustracao img/ang-cnv/cap25.svg | VS Code à esquerda exibindo DTOs fortemente tipados em TypeScript e Postman / Thunder Client à direita com a resposta 201 Created afirmativa da API.
:::

No sexto capítulo de *Comunicação Não-Violenta*, Marshall Rosenberg chega ao quarto e decisivo componente do processo: **o pedido**. 

Depois de observar fatos com neutralidade, identificar honestamente as emoções e reconhecer as necessidades universais subjacentes, resta a pergunta essencial: *o que gostaríamos de pedir aos outros para enriquecer a nossa vida?*

Marshall compartilha uma canção infantil comovente composta por sua parceira Ruth Bebermeyer:
> *"Como é que você faz um 'Não faça'?*  
> *Tudo o que sei é que sinto 'Não vou'*  
> *Quando me dizem 'Não faça'."*

Marshall nos ensina que o cérebro humano tem imensa dificuldade em processar ordens negativas. Quando você diz a alguém o que **não** quer que ela faça, você não forneceu nenhuma direção clara sobre o que ela **deve** fazer.

### A História do Marido e do Torneio de Golfe

Marshall ilustra essa armadilha com uma história real de um casal que participava de seus workshops. A mulher se queixava de que seu marido trabalhava demais e quase nunca ficava em casa.

Ela formulou um pedido negativo para o esposo:
> — *Eu pedi claramente: "Por favor, não passe tanto tempo no trabalho!"*

O que aconteceu em seguida? O marido ouviu o pedido, concordou e, três semanas depois, a mulher voltou ao workshop de Marshall ainda mais desesperada.

> — *O que houve?* — perguntou Marshall.  
> — *Ele cumpriu o pedido! Ele parou de passar tanto tempo no trabalho. Só que ele se matriculou em um campeonato de golfe e agora passa os fins de semana inteiros no clube! Ele continua longe de casa!*

A mulher havia pedido o que ela **não queria** (ficar no trabalho), mas não havia comunicado em **linguagem de ação positiva** o que ela **realmente desejava**: *"Gostaria que você passasse ao menos uma noite por semana jantando comigo e com as crianças em casa."*

---

## As Quatro Regras de Ouro do Pedido na CNV

1. **Usar linguagem de ação positiva**: pedir ações afirmativas, concretas e realizáveis, em vez de proibições abstratas.
2. **Evitar pedidos vagos ou nebulosos**: termos como *"seja mais compreensivo"*, *"respeite meu espaço"* ou *"seja responsável"* não dizem ao outro o que fazer no mundo físico. Em vez disso, peça: *"Gostaria que você me deixasse concluir minha frase antes de responder"*.
3. **Pedir a confirmação do que foi ouvido**: para garantir que o interlocutor não ouviu uma crítica ou exigência: *"Você se importaria de me dizer o que ouviu eu pedir?"*.
4. **Diferenciar pedidos de exigências**: o teste supremo de um pedido na CNV é a reação diante da palavra **"NÃO"**. Se quem pediu reagir com hostilidade, punição, julgamento ou culpa, tratava-se de uma **exigência disfarçada**. Se acolher o "não" com curiosidade empática, tratava-se de um **pedido autêntico**.

---

## O Pedido Tecnológico: A Falácia do Tipo `any`

No desenvolvimento de software com Angular, o antipattern da exigência vaga se manifesta na utilização do tipo `any` nas requisições HTTP e nas assinaturas de métodos:

```typescript
// CÓDIGO DA LINGUAGEM DE CHACAL: CONTRATO VAGO E EXIGÊNCIA CEGA
salvarSolicitacao(dados: any): Observable<any> {
  // O que tem dentro desses dados? Ninguém sabe.
  // Se faltar um campo obrigatório no backend, a API explode em 500.
  // O desenvolvedor força o servidor a adivinhar e pune o sistema se errar.
  return this.http.post('/api/pedidos', dados);
}
```

Usar `any` é dizer ao sistema: *"Faça alguma coisa para me agradar, e se você não adivinhar o que eu quero, eu vou quebrar a tela!"*. Não há clareza. Não há tipagem. Não há respeito pelo contrato.

---

## Contratos Claros com DTOs Tipados e TypeScript Estrito

Na arquitetura compassiva com Angular, cada pedido enviado ao mundo exterior é expresso através de um **DTO (Data Transfer Object)** formal, imutável e fortemente tipado:

```typescript
// src/app/core/models/solicitacao-conciliacao.dto.ts

/**
 * Contrato Explícito do Pedido:
 * Linguagem positiva de ação, documentando cada campo exigido.
 */
export interface SolicitarConciliacaoDTO {
  readonly idTopico: string;
  readonly observacaoFactual: string;
  readonly sentimentoIdentificado: string;
  readonly necessidadeSubjacente: string;
  readonly acaoPositivaSolicitada: string;
  readonly participantesEmails: readonly string[];
  readonly dataSugeridaEncontro: string; // ISO 8601 UTC
}

/**
 * Resposta Afirmativa do Servidor:
 * Confirma o acolhimento com clareza cristalina.
 */
export interface RespostaConciliacaoDTO {
  readonly idProtocolo: string;
  readonly status: 'agendado_com_sucesso' | 'em_analise_previa';
  readonly mensagemBoasVindas: string;
  readonly linkSalaVirtual: string;
}
```

---

## Consumo com HttpClient e Genéricos (angular.dev)

No Angular moderno, consumimos o `HttpClient` tipando explicitamente as requisições com os genéricos do TypeScript, combinando-o com operadores de resiliência do RxJS:

```typescript
// src/app/core/services/conciliacao.service.ts
import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, retry, timer } from 'rxjs';
import { SolicitarConciliacaoDTO, RespostaConciliacaoDTO } from '../models/solicitacao-conciliacao.dto';

@Injectable({
  providedIn: 'root'
})
export class ConciliacaoService {
  private readonly http = inject(HttpClient);
  private readonly endpoint = 'https://api.dialogo.org/v1/conciliacoes';

  /**
   * Envia um pedido formal, afirmativo e tipado ao servidor.
   * Se houver oscilação de rede, o operador retry tenta novamente
   * com serenidade através de um temporizador de backoff.
   */
  enviarPedido(pedido: SolicitarConciliacaoDTO): Observable<RespostaConciliacaoDTO> {
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'X-Intencao-Comunicao': 'cooperacao-mutua'
    });

    return this.http.post<RespostaConciliacaoDTO>(this.endpoint, pedido, { headers }).pipe(
      // Resiliência compassiva: tenta mais 2 vezes com intervalo progressivo
      retry({
        count: 2,
        delay: (erro, tentativa) => timer(tentativa * 1000)
      })
    );
  }
}
```

---

## Typed Forms: A Pureza dos Formulários Reativos no Angular

A partir do Angular 14 e consolidado nas versões 17 e 18, o pacote `@angular/forms` é **completamente tipado**. Isso significa que cada controle do formulário sabe o tipo exato de dado que manipula:

```typescript
// src/app/features/pedidos/formulario-pedido.component.ts
import { Component, inject, signal } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ConciliacaoService } from '../../core/services/conciliacao.service';
import { SolicitarConciliacaoDTO } from '../../core/models/solicitacao-conciliacao.dto';

// Interface estrita que define a forma do formulário
export interface FormularioPedidoCNV {
  observacao: FormControl<string>;
  sentimento: FormControl<string>;
  necessidade: FormControl<string>;
  pedidoAcao: FormControl<string>;
  emailColega: FormControl<string>;
}

@Component({
  selector: 'app-formulario-pedido-claro',
  standalone: true,
  imports: [ReactiveFormsModule, CommonModule],
  template: `
    <article class="card-pedido">
      <header>
        <h3>Formulando um Pedido que Enriquece a Vida</h3>
        <p class="legenda">Ações afirmativas, realizáveis e abertas à cooperação sincera.</p>
      </header>

      <form [formGroup]="form" (ngSubmit)="aoEnviarPedido()">
        <div class="campo">
          <label for="obs">1. O que foi observado (sem generalizações como "sempre"):</label>
          <input id="obs" formControlName="observacao" type="text" placeholder="Ex: O relatório foi entregue na quinta-feira..." />
        </div>

        <div class="campo">
          <label for="ped">2. Ação concreta e positiva que você gostaria de pedir:</label>
          <textarea
            id="ped"
            rows="3"
            formControlName="pedidoAcao"
            placeholder="Ex: Você estaria disposto a nos reunirmos amanhã às 14h por 20 minutos para alinharmos os pontos?"
          ></textarea>
        </div>

        <!-- Botão com CTA Concreto e Positivo (não apenas 'Enviar') -->
        <button
          type="submit"
          class="btn-cta-positivo"
          [disabled]="form.invalid || enviando()"
        >
          @if (enviando()) {
            Transmitindo Solicitação Serena...
          } @else {
            Apresentar Proposta de Diálogo Positivo
          }
        </button>
      </form>
    </article>
  `,
  styles: [`
    .card-pedido { background: #fffaf5; border: 1px solid #f5dfd0; border-radius: 12px; padding: 2rem; max-width: 600px; }
    .campo { margin-bottom: 1.25rem; display: flex; flex-direction: column; gap: 0.5rem; }
    input, textarea { padding: 0.75rem; border: 1px solid #f5dfd0; border-radius: 6px; font-family: inherit; font-size: 0.95rem; }
    .btn-cta-positivo { background: #d96b27; color: white; border: none; padding: 0.85rem 1.5rem; border-radius: 6px; font-weight: bold; cursor: pointer; }
    .btn-cta-positivo:disabled { background: #cbd5e1; cursor: not-allowed; }
  `]
})
export class FormularioPedidoClaroComponent {
  private readonly fb = inject(FormBuilder);
  private readonly conciliacao = inject(ConciliacaoService);

  readonly enviando = signal(false);

  // Formulário 100% tipado: o TypeScript valida os campos em tempo de compilação
  readonly form: FormGroup<FormularioPedidoCNV> = this.fb.group({
    observacao: this.fb.control('', { nonNullable: true, validators: [Validators.required, Validators.minLength(5)] }),
    sentimento: this.fb.control('Preocupação e Busca de Clareza', { nonNullable: true }),
    necessidade: this.fb.control('Previsibilidade e Cooperação', { nonNullable: true }),
    pedidoAcao: this.fb.control('', { nonNullable: true, validators: [Validators.required, Validators.minLength(10)] }),
    emailColega: this.fb.control('colegas@squad.org', { nonNullable: true, validators: [Validators.required, Validators.email] })
  });

  aoEnviarPedido(): void {
    if (this.form.invalid) return;

    this.enviando.set(true);
    const valores = this.form.getRawValue();

    const dto: SolicitarConciliacaoDTO = {
      idTopico: crypto.randomUUID(),
      observacaoFactual: valores.observacao,
      sentimentoIdentificado: valores.sentimento,
      necessidadeSubjacente: valores.necessidade,
      acaoPositivaSolicitada: valores.pedidoAcao,
      participantesEmails: [valores.emailColega],
      dataSugeridaEncontro: new Date().toISOString()
    };

    this.conciliacao.enviarPedido(dto).subscribe({
      next: (resposta) => {
        this.enviando.set(false);
        this.form.reset();
        alert(`Pedido acolhido! Protocolo: ${resposta.idProtocolo}`);
      },
      error: () => {
        this.enviando.set(false);
      }
    });
  }
}
```

::: dica
Observe o texto do botão: **"Apresentar Proposta de Diálogo Positivo"**. Ele descreve com exatidão o que o clique provocará no mundo real, eliminando a hesitação do usuário e substituindo comandos vagos como "Submeter" por linguagem de ação clara e consciente.
:::

---

::: quiz
? Qual foi a lição da história do marido que começou a jogar golfe após o pedido de sua esposa?
- Que o golfe é o melhor esporte para casais
+ Que pedidos formulados de maneira negativa ("não faça isso") não fornecem nenhuma direção sobre qual ação positiva concreta você gostaria que a pessoa fizesse
- Que os maridos nunca ouvem os pedidos das esposas
- Que pedidos devem ser feitos por cartas registradas
> Marshall destaca que o marido cumpriu exatamente o que ela pediu (passou menos tempo no trabalho), mas isso não atendeu à necessidade dela. Pedidos devem sempre expressar o que desejamos que seja feito.

? O que diferencia fundamentalmente um pedido genuíno de uma exigência na CNV?
- Pedidos são feitos de joelhos; exigências são feitas de pé
+ O teste do pedido é como reagimos se a outra pessoa disser "NÃO": se reagirmos com culpa, punição ou julgamento, era uma exigência. Se ouvirmos com empatia, era um pedido genuíno
- Exigências funcionam muito melhor com desenvolvedores de software
- Pedidos só podem ser feitos por pessoas com autoridade
> A liberdade do outro de recusar o pedido sem sofrer retaliação é o critério inegociável da não-violência. O "não" é apenas a expressão de outra necessidade que precisa ser ouvida.

? Por que o uso de `any` em chamadas do `HttpClient` é considerado uma prática anti-empática no código?
- Porque faz o navegador desligar sozinho
+ Porque anula a segurança de tipos em tempo de compilação, tornando o contrato opaco e transferindo o risco de quebras inesperadas para o tempo de execução
- Porque a documentação oficial do Angular proíbe compilar com any
- Porque servidores Linux não aceitam arquivos com a palavra any
> Tipar requisições e respostas com DTOs documenta o contrato de forma transparente, permitindo que qualquer colega de equipe compreenda a estrutura de dados sem adivinhações.

? Como os Typed Forms introduzidos no Angular moderno protegem o desenvolvedor contra erros bobos?
- Corrigindo automaticamente erros de digitação de palavras em português
+ Garantindo que métodos como `form.getRawValue()` e `form.value` retornem objetos com os tipos exatos de cada controle, prevenindo acessos a propriedades inexistentes
- Impedindo o usuário de digitar números ímpares
- Mudando o tema do sistema para preto se houver erro
> Com formulários estritamente tipados, o compilador do TypeScript avisa imediatamente se você tentar acessar um campo inexistente ou passar um tipo incompatível.

? Como o operador `retry` do RxJS exemplifica a resiliência compassiva na camada de rede?
- Disparando 1.000 requisições simultâneas para derrubar a API
+ Realizando tentativas automáticas discretas e espaçadas no tempo antes de alarmar o usuário, absorvendo oscilações passageiras de conexão com serenidade
- Excluindo os arquivos temporários do computador
- Bloqueando o acesso do usuário à internet
> O operador `retry` com delay progressivo protege a tranquilidade psicológica do usuário, resolvendo instabilidades passageiras nos bastidores sem gerar frustração visual.
:::

---

::: cartas
Pedido na CNV :: Quarto passo: solicitar ações afirmativas, concretas e realizáveis, respeitando a autonomia e o direito do outro de recusar.
Exigência na CNV :: Solicitação acompanhada pela ameaça implícita de punição, culpa ou depreciação em caso de recusa.
Linguagem de Ação Positiva :: Expressar o que desejamos que seja feito, em vez de focar no que desejamos proibir ou interromper.
Canção de Ruth Bebermeyer :: "Como é que você faz um 'Não faça'? Tudo o que sei é que sinto 'Não vou' quando me dizem 'Não faça'."
DTO (Data Transfer Object) :: Objeto tipado utilizado para transportar dados estruturados entre camadas de frontend e backend.
Typed Forms no Angular :: Formulários reativos onde cada `FormControl` e `FormGroup` possui tipagem estrita de seus valores no TypeScript.
retry (RxJS) :: Operador que repete uma subscrição em caso de erro após um temporizador de resiliência.
Call to Action (CTA) :: Elemento interativo de interface cujo texto descreve com exatidão e clareza a ação positiva pretendida.
:::
