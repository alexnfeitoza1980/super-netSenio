---
id: ang-antimedo-cap08
order: 308
kind: capitulo
num: 213
group: Angular · Parte 53 · Bloco II — Quebrando o Ciclo
title: O ciclo do medo
subtitle: Services e Injeção de Dependência — quebrando amarras tóxicas
minutes: 35
hue: 220
badge: 🔄
- Mapear a anatomia de um ciclo vicioso e o perigo do acoplamento forte: 
- Compreender o papel dos Services como provedores de lógica e estado: 
- Dominar o sistema de Injeção de Dependência (DI) e seus níveis de escopo: 
- Adotar a moderna função inject() em substituição à injeção via construtor: 
origem: angular/livros/01-antimedo-angular/content/08-ciclo-do-medo.md
---

::: ilustracao img/ang-antimedo/cap08.svg | Uma engrenagem quebrada sendo substituída por conexões limpas e desacopladas de injeção.
:::

No capítulo sobre o Ciclo do Medo, Pablo Marçal desenha um diagrama contundente:

1. **A Semente:** Um pensamento negativo ou relato de derrota plantado por outros.
2. **A Terra Fértil:** Uma mente ociosa, sem propósito definido, aberta a qualquer estímulo.
3. **O Adubo:** A ruminação constante, o diálogo interno de incapacidade e a busca por validação externa.
4. **O Resultado:** A paralisia total, que gera mais frustração e fecha o ciclo vicioso.

Para romper esse ciclo, não adianta tentar "pensar positivo" enquanto se mantém a mesma estrutura. É preciso aplicar o que Marçal chama de **agrotóxico mental**: uma ação deliberada, externa e cirúrgica que quebra a continuidade tóxica.

Na arquitetura de software, quando os componentes começam a fazer tudo sozinhos — buscar dados na API, validar regras de negócio, gerenciar autenticação e manipular o DOM —, nós criamos o temido **Código Espaguete Acoplado**.

A ferramenta do Angular para estancar essa contaminação e manter cada peça limpa e independente é o **Mecanismo de Injeção de Dependência (DI)**.

---

## O Problema do Acoplamento Forte (`new`)

Em linguagens orientadas a objetos e JavaScript puro, o erro mais comum de quem está começando é instanciar serviços diretamente dentro do componente usando a palavra-chave `new`:

```typescript
// ❌ CÓDIGO ACOPLADO E PROBLEMÁTICO
export class MedoListaComponent {
  private servico: MedoService;

  constructor() {
    // O componente assume a responsabilidade de criar a dependência!
    // Se MedoService mudar de parâmetros amanhã, o componente quebra.
    this.servico = new MedoService('http://api.local', new LoggerService(), new CacheService());
  }
}
```

Isso cria uma **amizade tóxica** entre classes: o componente fica refém da implementação interna do serviço. Fica impossível testar, impossível trocar a fonte de dados e impossível reaproveitar a lógica em outros lugares.

---

## O Injetor do Angular: A Fábrica de Soluções

Com a **Injeção de Dependência**, o componente não cria suas ferramentas. Ele apenas **declara o que precisa**, e o ecossistema do Angular entrega a instância pronta na porta dele:

```
[ Injetor Raiz do Angular ]
       │  (Mantém uma única instância: Singleton)
       ▼
 ┌──────────────┐     ┌──────────────┐
 │ Componente A │     │ Componente B │
 └──────────────┘     └──────────────┘
```

Para transformar uma classe em um serviço injetável, usamos o decorador `@Injectable()`:

```typescript
// core/services/antimedo.service.ts
import { Injectable } from '@angular/core';

export interface Desafio {
  id: number;
  descricao: string;
  enfrentado: boolean;
}

@Injectable({
  // providedIn: 'root' cria um Singleton disponível em toda a aplicação
  providedIn: 'root'
})
export class AntimedoService {
  private desafios: Desafio[] = [
    { id: 1, descricao: 'Fazer deploy em produção numa sexta-feira', enfrentado: false },
    { id: 2, descricao: 'Apresentar arquitetura para o time sênior', enfrentado: true },
    { id: 3, descricao: 'Aprender RxJS sem fugir para Promises', enfrentado: true }
  ];

  listar(): Desafio[] {
    return [...this.desafios];
  }

  marcarComoVencido(id: number): void {
    const item = this.desafios.find(d => d.id === id);
    if (item) {
      item.enfrentado = true;
    }
  }
}
```

---

## A Nova Forma: `inject()` vs. Injeção por Construtor

Historicamente, o Angular injetava serviços pelos parâmetros do construtor da classe. No Angular moderno (a partir da versão 14 e consolidado no Angular 16+), a recomendação da equipe do Google é usar a função **`inject()`**:

Compare as duas abordagens:

### Forma Legada (Via Construtor)
```typescript
@Component({ ... })
export class ExemploAntigoComponent {
  constructor(private antimedoService: AntimedoService) {}
}
```

### Forma Moderna (Via Função `inject()`)
```typescript
import { Component, inject } from '@angular/core';
import { AntimedoService } from './core/services/antimedo.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-desafios-lista',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="card-desafios">
      <h2>Lista de Ações de Enfrentamento</h2>
      <ul>
        <li *ngFor="let d of lista">
          <span [class.vencido]="d.enfrentado">{{ d.descricao }}</span>
          <button *ngIf="!d.enfrentado" (click)="vencer(d.id)">Enfrentar Agora</button>
        </li>
      </ul>
    </div>
  `
})
export class DesafiosListaComponent {
  // Limpo, conciso e com inferência de tipos imediata!
  private antimedoService = inject(AntimedoService);
  
  lista = this.antimedoService.listar();

  vencer(id: number): void {
    this.antimedoService.marcarComoVencido(id);
    this.lista = this.antimedoService.listar();
  }
}
```

::: dica Por que a função inject() é superior?
1. **Mais concisa:** Não precisa declarar construtor apenas para injeção.
2. **Herança amigável:** Se você estender uma classe filha, não precisa ficar chamando `super(servicoA, servicoB, servicoC)`.
3. **Pode ser usada em funções funcionais:** É a base dos novos Guards e Interceptors funcionais do Angular!
:::

---

## Escopos de Injeção: Onde a Semente Brota

O Angular possui uma árvore hierárquica de injetores. Dependendo de onde você provê o serviço, o tempo de vida dele muda completamente:

| Escopo | Configuração | Tempo de Vida | Uso Típico |
|---|---|---|---|
| **Root (Raiz)** | `@Injectable({ providedIn: 'root' })` | Dura a sessão inteira da aplicação (Singleton) | Autenticação, Estado Global, Conexão HTTP |
| **Componente** | `@Component({ providers: [MeuServico] })` | Criado quando o componente nasce, destruído quando ele morre | Estados locais complexos, formulários multietapas |

::: cuidado Perigo de Duplicar Instâncias
Se você colocar um serviço com `providedIn: 'root'` também no array `providers: [...]` de um componente, aquele componente criará uma **segunda instância isolada**, perdendo o compartilhamento com o restante do sistema.
:::

---

::: exercicio Exercício: Injetando Múltiplos Serviços com inject()
Escreva um componente `NotificadorComponent` que utilize simultaneamente o `AntimedoService` e o serviço nativo `Router` usando exclusivamente a sintaxe funcional `inject()`.

--- resposta
```typescript
import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AntimedoService } from './antimedo.service';

@Component({
  selector: 'app-notificador',
  standalone: true,
  template: `<button (click)="agir()">Ir para Próximo Desafio</button>`
})
export class NotificadorComponent {
  private antimedo = inject(AntimedoService);
  private router = inject(Router);

  agir(): void {
    this.antimedo.marcarComoVencido(1);
    this.router.navigate(['/vitoria']);
  }
}
```
:::

---

::: quiz
? Qual a finalidade principal do padrão Injeção de Dependência no Angular?
+ Desacoplar a criação de objetos da sua utilização, facilitando testes e manutenção
- Impedir que o código TypeScript seja compilado para JavaScript
- Aumentar o consumo de memória RAM do navegador
- Forçar que todo componente tenha um arquivo CSS dedicado
> O DI centraliza a criação e fornecimento de instâncias, garantindo baixo acoplamento e alta testabilidade.

? O que significa a propriedade `providedIn: 'root'` no decorador @Injectable?
+ O serviço será registrado como um Singleton global disponível em toda a aplicação
- O serviço só funcionará dentro da pasta raiz do Windows
- O serviço será executado com privilégios de administrador do sistema operacional
- O serviço será recriado toda vez que qualquer botão for clicado
> `providedIn: 'root'` cria uma única instância gerenciada pelo injetor raiz da aplicação.

? Qual a vantagem da função `inject(MeuServico)` em relação aos parâmetros de construtor?
+ É mais limpa, infere tipos diretamente e funciona em guards/interceptors funcionais
- Ela roda mais rápido porque pula a máquina virtual do JavaScript
- Ela permite chamar código C# de dentro do navegador
- Ela desativa a detecção de mudanças do componente
> A função `inject()` modernizou o Angular, simplificando herança e viabilizando arquiteturas funcionais.

? O que acontece com um serviço declarado na propriedade `providers` de um `@Component`?
+ Uma nova instância é criada para o componente e destruída junto com ele
- Ele é transformado em um banco de dados relacional SQLite
- Ele impede que outros componentes sejam renderizados na mesma página
- Ele é compartilhado automaticamente com outras abas do navegador
> Serviços providos no componente pertencem ao ciclo de vida daquela árvore de visualização.

? Como evitar o acoplamento forte entre componentes e serviços?
+ Injetando a dependência via DI em vez de usar `new Servico()` dentro da classe
- Criando todo o código dentro de um único arquivo de 10.000 linhas
- Evitando o uso de TypeScript e programando em JavaScript puro de 1995
- Desativando o compilador do Angular
> Nunca instancie serviços com `new`; permita que o Injetor forneça as instâncias.
:::

---

::: cartas
Ciclo do Medo no Código :: Acoplamento vicioso onde componentes dependem diretamente de implementações concretas e quebram em cadeia.
Service no Angular :: Classe dedicada a encapsular regras de negócio, dados, comunicação HTTP e orquestração.
Injeção de Dependência :: Padrão de projeto no qual o framework fornece instâncias prontas às classes que as solicitam.
providedIn: 'root' :: Configuração padrão que registra um serviço como Singleton acessível globalmente.
função inject() :: API moderna do Angular para solicitar dependências sem poluir o construtor da classe.
Component Provider :: Escopo que cria uma instância única para a visualização daquele componente específico.
:::

::: resumo
Pablo Marçal ensina que para vencer o ciclo do medo é preciso cortar o adubo das velhas conexões e introduzir a força de um novo princípio. No Angular, a Injeção de Dependência liberta seus componentes de carregar o mundo nas costas, distribuindo responsabilidades com elegância e precisão cirúrgica.
:::
