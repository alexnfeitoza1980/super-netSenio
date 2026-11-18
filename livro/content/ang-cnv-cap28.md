---
id: ang-cnv-cap28
order: 347
kind: capitulo
num: 245
group: Angular · Parte 62 · Bloco IV — Cura e Transformação Técnica
title: Conectando-nos com Nós Mesmos com Compaixão
subtitle: Self-Care do Desenvolvedor: TestBed, TypeScript Strict e Refatoração sem Culpa
minutes: 30
hue: 32
badge: 🧘
image: img/ang-cnv/cap28.svg
- Analisar a peça "Mil Palhaços" de Herb Gardner citada por Marshall Rosenberg: 
- Praticar o luto e o perdão na CNV diante de bugs e débitos técnicos em produção: 
- Configurar o compilador do TypeScript (strict, strictNullChecks) como guardião de sono: 
- Dominar testes unitários com TestBed, flushEffects() e fakeAsync no Angular moderno: 
origem: angular/livros/02-cnv-angular/content/28-autocompaixao-codigo.md
---

::: ilustracao img/ang-cnv/cap28.svg | VS Code Terminal exibindo testes automatizados com TestBed passando com 100% de sucesso e TypeScript em modo estrito atuando como guardião amoroso do desenvolvedor.
:::

No nono capítulo de *Comunicação Não-Violenta*, Marshall Rosenberg chega à aplicação mais decisiva — e com frequência a mais dolorosamente negligenciada — de toda a sua vida: **a compaixão voltada para dentro, para nós mesmos**.

Marshall faz uma advertência contundente:
> *"A aplicação mais importante da CNV pode ser no desenvolvimento da autocompaixão. Quando internamente somos violentos para conosco, é praticamente impossível ter compaixão verdadeira pelos outros."*

Para ilustrar o valor intrínseco de cada vida humana, Marshall cita uma cena comovente da peça teatral *Mil Palhaços* (*A Thousand Clowns*), escrita por Herb Gardner em 1962. Na peça, o protagonista Murray se recusa a entregar a guarda de seu sobrinho de 12 anos aos burocratas do serviço social, declarando com paixão:

> *"Quero que ele conheça exatamente a coisa especial que ele é, senão ele não perceberá quando ela começar a ir embora.*  
>  
> *Quero que ele permaneça desperto e veja todas as possibilidades mais loucas deste mundo. Quero que ele saiba que vale a pena fazer de tudo só para dar ao mundo um pequeno pontapé quando se tem essa chance.*  
>  
> *E quero que ele saiba a razão sutil, fugidia e importante pela qual ele nasceu um ser humano — e não uma cadeira!"*

Muitos de nós, contudo, fomos educados em uma cultura corporativa punitiva que nos trata como engrenagens descartáveis. E o pior: passamos a tratar a nós mesmos com essa mesma ferocidade:
- *"Como fui burro de subir esse commit com erro!"*
- *"Eu deveria ter previsto essa falha de concorrência!"*
- *"Eu nunca vou ser um desenvolvedor sênior de verdade!"*

Essa voz interior autoflageladora é a manifestação mais virulenta da **Linguagem de Chacal**. Ela não ensina nada; ela apenas destrói a autoestima, alimenta a paralisante **Síndrome do Impostor** e transforma a rotina de programação em um calvário de ansiedade e pavor.

---

## O Luto na CNV: Acolhendo o Código Legado sem Culpa

Como a CNV propõe que reajamos quando descobrimos que escrevemos um código ruim, lento ou cheio de bugs?

Marshall nos ensina a praticar o **Luto Empático**:
1. **Conectar-se com a dor**: reconhecer que o código causou frustração ou retrabalho.
2. **Perguntar-se com amor**: *"Qual era a minha necessidade naquele momento em que escrevi aquilo?"*. Você estava sob pressão implacável de entrega? Faltavam especificações? Você estava exausto após horas de reuniões?
3. **Perdoar o passado**: reconhecer que você fez o melhor que podia com a consciência e os recursos disponíveis.
4. **Celebrar o aprendizado**: agora você vê o problema com mais clareza e pode evoluir.

> **O código legado não foi escrito por maldade. Foi escrito por um ser humano que sustentou a empresa em pé até o dia de hoje.**

---

## O Compilador como Guardião Amoroso (tsconfig.json)

Muitos desenvolvedores encaram o modo estrito do **TypeScript** como um capataz chato que fica jogando linhas vermelhas na tela.

Na engenharia compassiva, invertemos essa percepção: **o compilador é o seu mais dedicado anjo da guarda**. Ele trabalha em silêncio para que você não seja acordado às 3 da madrugada de sábado por um incidente em produção!

```json
// tsconfig.json — Guardiões compassivos ativados
{
  "compilerOptions": {
    "strict": true,
    "noImplicitAny": true,
    "strictNullChecks": true,
    "noPropertyAccessFromIndexSignature": true,
    "noImplicitReturns": true,
    "noFallthroughCasesInSwitch": true
  }
}
```

Quando o TypeScript grita `Object is possibly 'undefined'`, ele está dizendo:
> *"Amigo, se a internet cair aqui, essa propriedade vai estar vazia. Vamos tratar isso juntos agora com calma para você dormir em paz com sua família no fim de semana?"*.

---

## Testes Unitários com TestBed (angular.dev): A Rede de Proteção Psicológica

O instrumento que confere coragem verdadeira para refatorar código legado sem medo é o **Teste de Unidade automatizado**. No Angular moderno em [angular.dev/overview](https://angular.dev/overview), testamos componentes standalone e Signals com o utilitário nativo `TestBed`:

```typescript
// src/app/core/services/motor-cnv.service.spec.ts
import { TestBed } from '@angular/core/testing';
import { MotorCnvService } from './motor-cnv.service';

describe('MotorCnvService — Prática de Autocompaixão e Resiliência', () => {
  let service: MotorCnvService;

  beforeEach(() => {
    // TestBed cria uma bancada de trabalho isolada e limpa
    TestBed.configureTestingModule({
      providers: [MotorCnvService]
    });
    service = TestBed.inject(MotorCnvService);
  });

  it('deve iniciar com o histórico de traduções sereno e vazio', () => {
    expect(service.historico().length).toBe(0);
    expect(service.totalTraducoes()).toBe(0);
  });

  it('deve acolher um texto com generalizações agressivas e extrair os 4 pilares', () => {
    const textoAgressivo = 'Você NUNCA me ouve e SEMPRE atrasa o sprint!';
    const res = service.traduzir(textoAgressivo);

    // Valida os 4 pilares da CNV decompostos
    expect(res.observacao.nome).toBe('Observação');
    expect(res.sentimento.nome).toBe('Sentimento');
    expect(res.necessidade.nome).toBe('Necessidade');
    expect(res.pedido.nome).toBe('Pedido');
    
    // A heurística identificou a alta tensão e desarmou com sucesso
    expect(res.nivelTensaoOriginal).toBeGreaterThanOrEqual(6);
  });

  it('deve registrar no histórico através de Signal imutável', () => {
    service.traduzir('Preciso de alinhamento claro com você.');
    
    expect(service.totalTraducoes()).toBe(1);
    expect(service.ultimaTraducao()?.observacao).toBeDefined();
  });
});
```

---

## Testando Signals e Efeitos com flushEffects() e fakeAsync

Quando testamos componentes com `effect()`, o Angular não dispara o efeito imediatamente para economizar ciclos de CPU. Em testes unitários, utilizamos a função `TestBed.flushEffects()`:

```typescript
// Testando Signals com Efeitos Reativos de Borda
it('deve disparar efeito de sincronização ao alterar o sinal', () => {
  const componente = fixture.componentInstance;
  
  // Altera o sinal de entrada
  componente.total.set(10);
  
  // Força o processamento imediato dos efeitos pendentes na bancada de testes
  TestBed.flushEffects();

  expect(consoleSpy).toHaveBeenCalledWith('Total sincronizado: 10');
});
```

E para operações assíncronas temporizadas (como um `debounceTime(300)` ou um `setTimeout`), usamos `fakeAsync` e `tick()`:

```typescript
import { fakeAsync, tick } from '@angular/core/testing';

it('deve aguardar o tempo de debounce antes de emitir a busca', fakeAsync(() => {
  componente.buscaControl.setValue('compaixão');
  
  // O valor ainda não foi emitido porque estamos na janela de silêncio
  expect(componente.estaEscutando()).toBeTrue();

  // Avança o relógio virtual de testes em 350 milissegundos
  tick(350);

  // Agora a emissão serena foi processada
  expect(componente.estaEscutando()).toBeFalse();
}));
```

::: dica
Quando você possui uma suíte de testes com cobertura real, **o pavor de refatorar desaparece**. Você pode trocar a implementação interna de um serviço, migrar de RxJS para Signals, atualizar versões do Angular e rodar `ng test`. A luz verde do terminal é o abraço de autocompaixão da engenharia: a certeza matemática de que suas alterações cuidaram da integridade do sistema.
:::

---

::: quiz
? Qual é a mensagem central da peça teatral "Mil Palhaços" citada por Marshall Rosenberg no capítulo sobre autocompaixão?
- Que palhaços são ótimos atores de cinema
+ Que cada ser humano é uma criatura única e especial, que nasceu com dignidade e propósito incondicionais, e não para ser tratado como um objeto ou uma cadeira descartável
- Que crianças devem trabalhar desde cedo
- Que serviços sociais não devem existir
> Marshall resgata Murray para nos lembrar: o valor de um ser humano não reside na sua produtividade econômica ou na ausência de erros, mas na sua própria condição humana.

? O que é o "Luto Empático" ensinado pela CNV quando cometemos um erro técnico?
- Chorar na frente do gestor durante a reunião diária
+ Conectar-se com a dor da necessidade não atendida pelo erro, compreendendo as circunstâncias e pressões que levaram àquela ação, perdoando a si mesmo e aprendendo
- Fazer um minuto de silêncio antes de ligar o computador
- Desligar todos os servidores da empresa
> Em vez de nos punirmos com auto-ódio ("eu sou um incompetente"), o luto empático investiga com amor: "qual necessidade minha não foi atendida que me levou a agir daquela forma?".

? Para que serve a função `TestBed.flushEffects()` nos testes unitários do Angular?
- Para apagar todos os efeitos visuais do CSS
+ Para forçar a execução síncrona e determinística de todos os Signals declarados dentro de `effect()` pendentes na bancada de testes
- Para desinstalar o Node.js
- Para reiniciar o navegador Karma
> Como os efeitos do Angular rodam de forma assíncrona agendada, `TestBed.flushEffects()` garante que o desenvolvedor possa testar os efeitos colaterais de forma previsível e imediata.

? Qual é a função da zona de teste `fakeAsync` e da instrução `tick()`?
- Enganar o cliente sobre os prazos de entrega
+ Criar um relógio virtual simulado onde operações assíncronas baseadas em tempo (como timers e debounceTime) podem ser avançadas instantaneamente sem esperas reais
- Deixar o computador duas vezes mais rápido
- Converter código assíncrono em código síncrono no servidor
> Com `fakeAsync` e `tick(300)`, você testa um atraso de 5 minutos em menos de 2 milissegundos de execução de teste!

? Por que o modo estrito do TypeScript (`strictNullChecks: true`) é considerado uma ferramenta de autocompaixão?
- Porque ele reduz a quantidade de linhas de código pela metade
+ Porque ele atua como uma rede de proteção silenciosa que avisa sobre riscos de nulidade antes do deploy, poupando o desenvolvedor de chamadas de emergência e estresse no fim de semana
- Porque ele corrige os bugs automaticamente sem o programador ver
- Porque ele impede que outras pessoas mexam no repositório
> Cuidar de si mesmo na engenharia significa adotar ferramentas rigorosas que assumem o fardo cognitivo de detectar falhas bobas antes que elas machuquem usuários reais.
:::

---

::: cartas
Autocompaixão na CNV :: Prática de tratar a si mesmo com a mesma ternura, compreensão e perdão que dedicamos a um ente querido que comete um erro.
Peça Mil Palhaços :: Obra de Herb Gardner lembrando que nascemos seres humanos com dignidade sagrada, e não cadeiras ou engrenagens frias.
Luto na CNV :: Reconhecimento compassivo de nossas escolhas imperfeitas sem afundar em culpa, vergonha ou auto-ódio.
TypeScript strictNullChecks :: Guardião de código que impede o esquecimento de valores nulos ou indefinidos em tempo de compilação.
TestBed no Angular :: Bancada oficial do framework para instanciar, isolar e validar componentes e serviços com alta fidelidade.
TestBed.flushEffects() :: Utilitário de teste que processa de imediato os efeitos reativos pendentes de Signals.
fakeAsync & tick() :: Ambiente de teste virtual que permite avançar cronômetros e timers assíncronos instantaneamente.
Psychological Safety :: Segurança psicológica do time para errar, aprender, refatorar e inovar sem pavor de humilhação.
:::
