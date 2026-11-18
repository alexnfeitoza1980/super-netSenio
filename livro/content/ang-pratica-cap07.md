---
id: ang-pratica-cap07
order: 282
kind: capitulo
num: 192
group: Angular · Parte 47 · Estado e interação
title: Injeção de dependência e fronteiras
subtitle: A injeção de dependência torna colaboração entre objetos explícita e ajuda a trocar implementações sem acoplar a tela ao detalhe.
minutes: 45
hue: 210
objectives:
- Entender providers e o ciclo básico de injeção do Angular
- Separar a view de um contrato de acesso a dados
- Reconhecer escopo de aplicação, rota e componente
origem: angular/livros/01-biblioteca-angular/content/07-cap07.md
---

::: ilustracao img/ang-pratica/camadas-angular.svg | A página apresenta, a store aplica regras e o repositório decide a origem dos dados.
:::

## Pedir uma colaboração ao framework

`inject(BibliotecaStore)` pede ao injetor uma instância do serviço registrado. Com `providedIn: 'root'`, o Angular disponibiliza uma instância compartilhada para a aplicação. A página de catálogo, o detalhe e os cartões leem o mesmo estado.

Esse compartilhamento é uma escolha de escopo. Um provider na rota pode criar estado por área; um provider no componente pode limitar uma instância àquela árvore. Não há “sempre singleton” sem observar onde o provider foi declarado.

## Uma interface antes de um servidor

Uma evolução para HTTP pode introduzir um token `LIVRO_REPOSITORIO` e implementações locais e remotas. A página passa a depender do contrato; configuração decide qual implementação fornecer. Isso facilita testes e migração gradual, sem fingir que o protótipo já possui backend.

::: cuidado Não injete tudo em todo lugar
Um serviço global que conhece autenticação, apresentação, armazenamento, métricas e todas as páginas vira um objeto difícil de compreender. Agrupe responsabilidades por domínio e mantenha as dependências na direção que faz sentido: interface chama caso de uso/serviço, não o contrário.
:::

## Revisão como conversa técnica

Ao discordar de uma fronteira, descreva o efeito observável: “Esta página grava diretamente em três lugares; como poderíamos centralizar a regra e testar a persistência?” A pergunta investiga a intenção e apresenta um pedido concreto. Ela é mais acionável do que “isso está errado”.

::: quiz
? O que `providedIn: 'root'` indica neste serviço?
+ Disponibiliza uma instância no injetor raiz, compartilhada no escopo da aplicação.
- Faz o serviço executar somente no servidor.
- Converte automaticamente a classe em endpoint HTTP.
:::

::: cartas
Qual benefício de separar contrato e implementação? :: A interface pode usar uma implementação local ou remota sem reescrever a tela.
Onde uma instância pode ser providenciada? :: No injetor raiz, em providers de rota ou de componente, conforme o escopo necessário.
:::
