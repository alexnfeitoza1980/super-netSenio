---
id: ang-amigos-cap50
order: 371
kind: capitulo
num: 267
group: Angular · Parte 68 · Liderança Técnica e o Ecossistema Amigos UI
title: Fale Primeiro dos Próprios Erros
subtitle: Injeção de Dependências Flexível, Provedores Opcionais e Factory Functions
minutes: 30
hue: 215
badge: 🤝
image: img/ang-amigos/cap50.svg
- Analisar o aprendizado de Dale Carnegie com sua sobrinha Josephine sobre a tolerância e o reconhecimento da própria imperfeição: 
- Compreender a arrogância de bibliotecas que lançam NullInjectorError por ausência de configurações secundárias: 
- Implementar o padrão InjectionToken com valores padrão seguros através de providedIn: 'root' e factory providers
- Utilizar a função inject() com o modificador { optional: true } para injeção não-punitiva e resiliente
- Projetar a função modular provideAmigosUI() para parametrização global elegante em app.config.ts: 
origem: angular/livros/03-amigos-angular/content/50-injecao-dependencias-factory-providers.md
---

::: ilustracao img/ang-amigos/cap50.svg | Diagrama de injeção de dependências hierárquico exibindo a resolução de tokens do Root Injector até o componente filho com fallback opcional seguro.
:::

Alguns anos após consolidar sua carreira como escritor e palestrante, Dale Carnegie recebeu em seu escritório em Nova York sua jovem sobrinha, **Josephine Carnegie**, de apenas dezenove anos, para trabalhar como sua secretária particular.

Josephine era uma moça dedicada, mas inexperiente. Certo dia, ao redigir uma correspondência importante para uma grande editora, cometeu uma série de erros crassos de digitação e arquivou documentos confidenciais na pasta errada. Carnegie sentiu o sangue subir-lhe à cabeça; estava prestes a explodir em uma repreensão furiosa quando uma voz interior sussurrou-lhe à consciência:

> *"Espere um minuto, Dale. Você tem o dobro da idade de Josephine e tem dez mil vezes mais experiência no mundo dos negócios do que ela. Como pode esperar que ela tenha o seu discernimento, o seu senso de proporção e a sua capacidade de organização? E mais: o que você mesmo estava fazendo quando tinha dezenove anos? Lembra-se dos erros tolos e vergonhosos que você cometeu naquela época?"*

Após refletir honestamente e constatar que seu próprio desempenho aos dezenove anos havia sido incomparavelmente pior do que o de Josephine, Carnegie acalmou-se, respirou fundo e aproximou-se da mesa da moça:

> *"Josephine, você cometeu um erro aqui, mas Deus sabe que não foi um erro pior do que os incontáveis erros que eu mesmo cometi quando tinha a sua idade. Ninguém nasce com discernimento ou bom senso; isso só vem com a prática e a experiência. Na verdade, você é muito mais competente aos dezenove anos do que eu jamais fui. Mas não acha que, se tivéssemos feito essa carta desta outra maneira, o resultado teria sido mais feliz?"*

Josephine nunca mais cometeu aquele erro. A admissão humilde de Carnegie sobre as suas próprias fraquezas desarmou qualquer ressentimento e inspirou na jovem um desejo ardente de superar as expectativas do tio.

Carnegie sintetiza:

> *"Não é tão difícil ouvir falar de nossos próprios defeitos se a pessoa que nos repreende começa admitindo humildemente que ela mesma está longe de ser perfeita. Fale primeiro sobre os seus próprios erros antes de criticar a outra pessoa."*

---

## A Arrogância do `NullInjectorError`

No ecossistema de Injeção de Dependências (DI) do Angular, quantas bibliotecas você já viu que agem com a arrogância de quem nunca cometeu um erro?

Você importa um componente visual simples para exibir um ícone ou um toast na tela, salva o arquivo e o Angular explode com:

```text
NullInjectorError: R3InjectorError(Standalone[_AppComponent])[AmgToastService -> AmgConfigToken]: 
  NullInjectorError: No provider for AmgConfigToken!
```

A biblioteca parte do pressuposto arrogante de que o desenvolvedor é onisciente, que ele leu todas as cem páginas do manual de instalação e que ele decorou que precisava registrar um token interno no `app.config.ts`. Se o desenvolvedor esqueceu, a biblioteca o castiga com um erro fatal que derruba toda a aplicação.

Essa biblioteca não admite as próprias limitações. Ela exige perfeição absoluta do recém-chegado.

---

## Injeção Humilde: Defaults Seguros e `{ optional: true }`

Uma biblioteca construída com a sabedoria de Carnegie é **humilde e tolerante**. Ela reconhece que o desenvolvedor pode esquecer ou simplesmente não querer configurar nada naquele momento.

No Angular moderno, utilizamos a função nativa `inject()` combinada com o modificador de injeção `{ optional: true }`:

```typescript
// amg-config.token.ts
import { InjectionToken } from '@angular/core';

export interface AmgConfig {
  /** Duração padrão de toasts em milissegundos */
  readonly toastDuration: number;
  /** Tema padrão da aplicação */
  readonly defaultTheme: 'light' | 'dark' | 'corporate';
  /** Se deve registrar logs de depuração no console */
  readonly debugMode: boolean;
}

/** Configuração padrão segura para quem não registrou nada */
export const AMG_DEFAULT_CONFIG: AmgConfig = {
  toastDuration: 4000,
  defaultTheme: 'light',
  debugMode: false
};

/**
 * Token de Injeção para customização global da biblioteca.
 * Possui providedIn: 'root' com factory default segura!
 */
export const AMG_CONFIG = new InjectionToken<AmgConfig>('AMG_CONFIG', {
  providedIn: 'root',
  factory: () => AMG_DEFAULT_CONFIG
});
```

Observe que fornecemos uma **função de fábrica (*factory*)** diretamente na criação do `InjectionToken`. Se o consumidor não configurar nada, o próprio Angular instancia a `AMG_DEFAULT_CONFIG` automaticamente, com **zero risco de NullInjectorError**!

---

## Injeção no Serviço: O Acolhimento Gracioso

Veja como o `AmgToastService` consome essa dependência:

```typescript
// amg-toast.service.ts
import { Injectable, inject } from '@angular/core';
import { AMG_CONFIG, AMG_DEFAULT_CONFIG, AmgConfig } from './amg-config.token';

@Injectable({
  providedIn: 'root'
})
export class AmgToastService {
  /**
   * Injeção opcional e graciosa:
   * Se por algum motivo bizarro o token for anulado, assumimos o fallback local.
   */
  private readonly config: AmgConfig = 
    inject(AMG_CONFIG, { optional: true }) ?? AMG_DEFAULT_CONFIG;

  exibirElogio(mensagem: string): void {
    const duracao = this.config.toastDuration;
    
    // Cria e exibe o toast de forma suave...
    console.log(`[Toast ${duracao}ms] 🤝 ${mensagem}`);
  }
}
```

---

## A Função de Fábrica Amigável: `provideAmigosUI()`

Para os desenvolvedores que desejam customizar a biblioteca de forma explícita e elegante no `app.config.ts`, fornecemos uma função de conveniência no estilo padrão do Angular 19 ([angular.dev/overview](https://angular.dev/overview)):

```typescript
// provide-amigos.ts
import { EnvironmentProviders, makeEnvironmentProviders } from '@angular/core';
import { AMG_CONFIG, AMG_DEFAULT_CONFIG, AmgConfig } from './amg-config.token';

/**
 * Função de fábrica moderna para inicializar a biblioteca Amigos UI no app.config.ts.
 *
 * @example
 * ```typescript
 * export const appConfig: ApplicationConfig = {
 *   providers: [
 *     provideAmigosUI({ toastDuration: 2500, defaultTheme: 'corporate' })
 *   ]
 * };
 * ```
 */
export function provideAmigosUI(
  configuracaoCustomizada?: Partial<AmgConfig>
): EnvironmentProviders {
  const configFinal: AmgConfig = {
    ...AMG_DEFAULT_CONFIG,
    ...configuracaoCustomizada
  };

  return makeEnvironmentProviders([
    {
      provide: AMG_CONFIG,
      useValue: configFinal
    }
  ]);
}
```

Quando o desenvolvedor vê que a biblioteca funciona perfeitamente sem nenhuma configuração — mas que, se ele desejar customizar, existe uma função de primeira classe esperando por ele —, ele se sente valorizado e respeitado.

Falar primeiro sobre as próprias falhas e prever que erros acontecerão é o segredo para construir sistemas tolerantes a falhas e profundamente humanos.

---

::: quiz
? Qual foi a reflexão interna de Dale Carnegie antes de repreender os erros de digitação de sua sobrinha Josephine?
+ Lembrou-se de que aos dezenove anos ele próprio cometia erros infinitamente piores e que ninguém nasce com discernimento pronto.
- Decidiu descontar os erros do salário da sobrinha no final do mês.
- Concluiu que máquinas de escrever deveriam ser proibidas em escritórios modernos.
- Planejou contratar um substituto mais velho sem avisar a sobrinha.
> Carnegie reconheceu a própria imperfeição antes de falar, transformando uma bronca destrutiva em um momento inesquecível de aprendizado.

? Por que o erro `NullInjectorError` no Angular causa tanta frustração nos desenvolvedores?
+ Porque interrompe a execução de toda a aplicação devido à ausência de um token de injeção que a biblioteca poderia ter suprido com um valor padrão.
- Porque apaga o histórico de commits do repositório Git.
- Porque faz o monitor do computador piscar na cor amarela.
- Porque desinstala o pacote npm sem a autorização do usuário.
> O `NullInjectorError` pune o desenvolvedor por não adivinhar dependências internas que deveriam possuir fallbacks automáticos.

? Qual é o benefício de fornecer a propriedade `factory` diretamente na declaração de um `InjectionToken`?
+ Permite que o Angular crie uma instância padrão automática do serviço ou configuração caso o consumidor não declare nenhum provedor explícito.
- Faz o token funcionar exclusivamente em sistemas operacionais Linux.
- Impede que hackers descubram a senha do banco de dados.
- Reduz o consumo de banda de internet em 90%.
> O token auto-suficiente (*tree-shakable provider*) elimina a obrigatoriedade de configurações manuais no `app.config.ts`.

? O que o modificador `{ optional: true }` na função `inject()` garante para o componente?
+ Evita o lançamento de exceções caso o token solicitado não esteja presente na árvore de injeção, retornando `null` de forma segura.
- Permite que o desenvolvedor escolha se deseja pagar pela licença do software.
- Converte funções assíncronas em funções síncronas.
- Oculta o componente da visualização do usuário comum.
> A injeção opcional permite que o código utilize o operador de coalescência nula (`??`) para aplicar fallbacks sem quebrar a execução.

? Para que serve a função `makeEnvironmentProviders()` na criação de funções como `provideAmigosUI()`?
+ Empacota um conjunto de provedores em um tipo restrito (`EnvironmentProviders`), impedindo que eles sejam injetados acidentalmente no nível de componentes.
- Cria conexões seguras com servidores de inteligência artificial.
- Formata o código do projeto de acordo com as regras do Prettier.
- Cancela requisições HTTP que demorarem mais de 1 segundo.
> `makeEnvironmentProviders` é a API oficial do Angular 15+ para provedores de escopo de aplicação (*Environment Scope*).
:::

---

::: cartas
Josephine Carnegie :: Sobrinha de Dale Carnegie que foi acolhida com paciência após o tio recordar suas próprias falhas na juventude.
Falar dos Próprios Erros :: Princípio de liderança: reconhecer as próprias limitações antes de apontar os deslizes alheios desconcerta o ressentimento.
NullInjectorError :: Falha crítica do injetor do Angular quando uma dependência obrigatória não foi encontrada na árvore.
InjectionToken com Factory :: Token que se auto-provê com um valor default seguro na ausência de declarações explícitas.
inject({ optional: true }) :: Padrão de injeção resiliente que acolhe ausências de dependências sem lançar erros fatais.
makeEnvironmentProviders :: Utilitário moderno do Angular para estruturar funções de configuração global (provideXxx).
:::
