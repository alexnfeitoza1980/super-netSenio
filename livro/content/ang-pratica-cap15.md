---
id: ang-pratica-cap15
order: 290
kind: capitulo
num: 200
group: Angular · Parte 49 · Qualidade e cuidado
title: Testes que dão confiança
subtitle: Testes transformam expectativas importantes em verificações repetíveis, sem tentar provar tudo com uma única ferramenta.
minutes: 55
hue: 210
objectives:
- Escrever testes de comportamento para componentes e serviços
- Usar TestBed, fixtures e RouterTestingHarness
- Distinguir teste unitário, integração, browser e E2E
origem: angular/livros/01-biblioteca-angular/content/15-cap15.md
---

## O que já vale testar

A store tem regras claras: filtrar sem diferenciar acentos, combinar termo e categoria, alternar favorito, limitar progresso e recuperar estado persistido. Esses comportamentos podem ser testados sem depender da aparência de um botão.

<<< src/app/biblioteca.store.spec.ts

Testes de componente usam `TestBed` para criar a view e simular eventos. Testes do Router podem verificar navegação real. Testes de integração avaliam peças conectadas. E2E confirma fluxos críticos no navegador. Nenhuma camada precisa repetir todos os casos.

## Arrange, act, assert

Prepare um estado; execute uma ação; confira uma observação. O teste deve expressar uma regra que importa a alguém. Evite verificar detalhe privado ou uma chamada interna sem efeito externo. Um teste que falha deve indicar o comportamento ausente, não apenas “mock chamado uma vez”.

Use um storage falso ou limpe a chave antes/depois. Testes isolados não devem depender da ordem. HTTP usa `HttpTestingController` para controlar respostas, erros e solicitações sem rede real.

## Código como conversa com o time

Um nome como “filtra sem diferenciar maiúsculas nem acentos” explica o contrato sem ler o corpo. Comentários de teste e revisão devem reduzir ambiguidade. Quando um teste fica difícil de escrever, pode ser sinal de uma fronteira confusa — não sempre, mas vale investigar.

::: quiz
? Qual é um bom alvo para um teste de unidade da store?
+ A regra de que busca por “nao ficcao” retorna o livro da categoria “Não ficção”.
- A quantidade de arquivos privados criada pelo Angular CLI.
- A cor exata de um pixel em qualquer monitor.
:::

::: cartas
O que o `TestBed` prepara? :: Um contexto Angular controlado para criar componentes e injetar dependências no teste.
Qual a diferença entre teste unitário e E2E? :: Unitário foca uma unidade com dependências controladas; E2E verifica um fluxo integrado no navegador.
:::
