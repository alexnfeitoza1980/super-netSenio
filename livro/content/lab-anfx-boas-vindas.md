---
id: lab-anfx-boas-vindas
order: 261
kind: lab
num: 28
group: Laboratório · Parte 42 · Banco ANFX: Clean Architecture na prática
title: O ponto de partida
subtitle: Bem-vindo ao laboratório do Banco ANFX.
minutes: 6
hue: 280
origem: aula_guiada_mcroservicos_net_ia/portal-anfx/dist/lessons.js
---

Bem-vindo ao laboratório do Banco ANFX. Vamos construir microserviços bancários com código, contexto e decisões explicadas. Uma aula de cada vez, do problema à implementação.

**06**áreas de negócio

para explorar

**04**aulas e guias

disponíveis agora

**01**primeiro fluxo

pagamento de uma conta

## Antes de escrever o próximo serviço

CONTEXTO → DECISÃO → CÓDIGO

↗O CENÁRIO

### O legado é fictício. O aprendizado, prático.

O ANFX Legacy representa o sistema anterior do banco. Vamos descrever seu comportamento e suas limitações, sem construir esse monólito. O desenvolvimento acontece diretamente nos serviços.

[Conhecer o ANFX Legacy ↗](#legado)

⌘O MÉTODO

### Cada decisão tem um custo.

Separar serviços pode melhorar a autonomia, mas introduz rede, falhas parciais e mais operação. Aqui, cada tecnologia precisa resolver uma necessidade concreta do banco.

[Explorar a base de código ↗](#estrutura)

## O mapa da nossa evolução

VISÃO INICIAL · AINDA NÃO IMPLEMENTADA

{{architecture}}

::: nota Nota
**Onde estamos de verdade?** A solução .NET possui quatro camadas e um exemplo de produtos. Os domínios bancários ainda serão implementados. Este portal documenta esse ponto de partida e não apresenta o planejamento como código pronto.
:::

## Como vamos aprender

01

### Entender o problema do banco

Começamos por um comportamento do ANFX Legacy e identificamos a necessidade de negócio.

02

### Tomar uma decisão com alternativas

Definimos responsabilidades, contratos e consistência. Explicamos o ganho e o custo da escolha.

03

### Construir, verificar e registrar

A aula acompanha o código com arquivos, exemplos, verificações e limitações. O material cresce junto com o projeto.


::: quiz
? Por que não vamos implementar o ANFX Legacy?
- Porque monólitos não funcionam em bancos.
+ Porque ele é um cenário didático; construiremos os serviços diretamente.
- Porque microserviços dispensam regras de negócio.
> O legado fornece contexto para comparar decisões. Ele não será construído, e a arquitetura distribuída continua precisando de regras de negócio bem definidas.
:::
