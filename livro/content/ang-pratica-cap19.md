---
id: ang-pratica-cap19
order: 294
kind: capitulo
num: 204
group: Angular · Parte 50 · Preparar para crescer
title: Arquitetura, APIs e evolução
subtitle: Arquitetura é uma resposta ao tamanho e às mudanças do produto, não uma competição de pastas ou padrões.
minutes: 50
hue: 210
objectives:
- Organizar aplicação por domínio e fronteiras de responsabilidade
- Separar UI, estado, acesso a dados e modelos de rede
- Planejar migrações de versão, módulos e APIs em evolução
origem: angular/livros/01-biblioteca-angular/content/19-cap19.md
---

## Um domínio por vez

A aplicação atual é pequena: modelo `Livro`, catálogo local, store e páginas. Quando o catálogo virar domínio grande, separe por funcionalidade — `catalogo`, `favoritos`, `leitura` — e mantenha abstrações perto de onde são usadas. Uma pasta `shared` não é lugar para qualquer coisa sem dono.

O contrato de domínio do app pode diferir do DTO da API. Traduza na fronteira quando isso reduz acoplamento. Documente nomes, nulabilidade, paginação, ordenação e erros. Versione mudanças incompatíveis; estabeleça quem mantém o contrato com backend.

## Escolha de estado

Signals e `computed` servem bem ao estado de interface local e derivado. RxJS atende streams e coordenação temporal. Uma biblioteca externa de estado pode ser apropriada para regras compartilhadas complexas, time travel ou fluxos de efeitos grandes, mas introduz conceitos e dependências. Escolha pela necessidade demonstrável.

## As peças Angular além de componentes

Uma **directive** adiciona comportamento a um elemento existente; uma **pipe** transforma um valor para apresentação. Prefira `host` metadata para declarar listeners e atributos de host quando isso deixa o contrato mais claro. Pipes costumam ser puros e sem efeitos. `hostDirectives` compõem comportamento reutilizável sem herança. Não esconda regra de negócio numa diretiva visual.

```ts
import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'paginas', standalone: true })
export class PaginasPipe implements PipeTransform {
  transform(total: number): string {
    return `${total} ${total === 1 ? 'página' : 'páginas'}`;
  }
}
```

No componente standalone, importe `PaginasPipe` e use `{{ livro.paginas | paginas }}`. A função não altera o livro. Em dados já formatados na origem, não duplique tradução e apresentação sem necessidade.

`ngOnInit` serve para inicialização após criação dos inputs; `ngOnDestroy` e `DestroyRef` delimitam recursos. Em muitos casos, o framework encerra inscrições do `AsyncPipe` e de `toSignal`; uma inscrição manual precisa de cancelamento. `ViewChild` e `ContentChild` consultam elementos/componentes dentro da view ou conteúdo projetado: use-os quando uma relação real exigir, não para substituir inputs e outputs.

`ViewEncapsulation.Emulated` escopa estilos por padrão; `ShadowDom` usa Shadow DOM nativo; `None` deixa estilos globais e pode causar colisões. `ng-content` projeta conteúdo. `ViewContainerRef.createComponent` permite criação dinâmica controlada. Para cada recurso imperativo, explique quem cria, quem destrói e como o foco é mantido.

## Ecossistema Angular e escolha consciente

Angular CDK oferece blocos sem estilo visual para overlay, acessibilidade, arrastar/soltar, scrolling virtual e testes com Component Harnesses. Angular Material é uma biblioteca visual própria do ecossistema Angular; esta aplicação não depende dela para manter o projeto focado no framework. `@defer` cuida de carregamento tardio; `animate.enter`/`animate.leave` cobrem transições declarativas em versões que os suportam.

Quando uma equipe escolhe CDK ou Material, isso ainda é Angular, mas adiciona dependência, API e decisões de design. Verifique a compatibilidade das versões e use a peça que resolve uma necessidade. Não adicione uma biblioteca só para tornar cada card diferente.

## Versões e migração

Atualização do Angular combina `ng update`, migrations, changelog, testes e revisão do diff. Faça uma versão por vez em aplicação crítica e leia o resultado. `NgModule` continua relevante em projetos legados e bibliotecas; aplicações novas podem usar standalone. Não migre arquitetura apenas para seguir uma moda.

::: pratica Explique o limite em uma revisão
Escolha uma classe da aplicação. Complete: “Esta peça é responsável por ___ e não é responsável por ___.” Se a frase for difícil, investigue dependências e nome. Não renomeie tudo antes de saber qual problema quer resolver.
:::

::: quiz
? Qual critério deve guiar a adoção de uma biblioteca externa de estado?
+ Uma necessidade concreta que a arquitetura atual não atende bem, comparada ao custo de conceito, dependência e manutenção.
- O número de pastas no projeto.
- O desejo de substituir todos os signals por uma ferramenta nova.
:::

::: cartas
Para que serve um pipe puro? :: Transformar/formatar valores com dependências explícitas, sem efeitos colaterais em cada renderização.
Como abordar uma migração de Angular? :: Use a ferramenta oficial, revise as migrations e execute testes/build e validações relevantes.
:::
