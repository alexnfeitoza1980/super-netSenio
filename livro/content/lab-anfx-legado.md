---
id: lab-anfx-legado
order: 262
kind: lab
num: 29
group: Laboratório · Parte 42 · Banco ANFX: Clean Architecture na prática
title: Conhecendo o ANFX Legacy
subtitle: O Banco ANFX é fictício.
minutes: 8
hue: 292
origem: aula_guiada_mcroservicos_net_ia/portal-anfx/dist/lessons.js
---

O Banco ANFX é fictício. Este é o cenário que vamos usar como referência para decidir quais responsabilidades separar, como integrar serviços e quais problemas não podem atravessar a migração.

::: nota Nota
**Hipótese de trabalho:** o ANFX Legacy é uma aplicação .NET com PostgreSQL compartilhado. Clientes, contas, pagamentos, seguros e investimentos são publicados juntos. Essas características descrevem nossa simulação, não uma aplicação existente nesta pasta.
:::

## Uma visão, duas arquiteturas

COMPARE OS CENÁRIOS

{{architecture}}

## As áreas do Banco ANFX

| Área | O que conhece | Limite a discutir |
|---|---|---|
| Clientes | Identificação e situação cadastral | Quem pode alterar o cadastro? |
| Contas | Titulares e ciclo de vida da conta | Conta aberta é suficiente para permitir uma operação? |
| Movimentação financeira | Lançamentos, reservas e saldo | Como manter invariantes com operações concorrentes? |
| Pagamentos | Agendamento e estado da operação | Como reconhecer uma repetição da mesma solicitação? |
| Seguros | Propostas e apólices | Como isolar a indisponibilidade da seguradora? |
| Investimentos | Aplicações, resgates e posições | Como conciliar solicitações e confirmações externas? |

## O que nos leva a mudar?

### Publicações acopladas

Uma alteração em seguros exige publicar a aplicação inteira. Serviços podem permitir publicação independente, desde que seus contratos sejam compatíveis e suas dependências estejam controladas.

### Dados sem fronteiras

Pagamentos acessa diretamente tabelas de contas. Após a separação, precisaremos de contratos e de um responsável por cada dado. Consultar outro banco diretamente manteria o acoplamento.

### Recursos compartilhados

Uma integração lenta pode ocupar conexões e threads da aplicação. Separar processos ajuda a isolar recursos; limites de tempo e concorrência ainda precisam ser definidos.

### Transações distribuídas

No monólito, alterações no mesmo banco podem usar uma transação local. Entre serviços, não vamos presumir um commit único: estados intermediários, repetição e compensação entram no desenho.

## O primeiro fluxo: pagar uma conta

MODELO CONCEITUAL

1

### Receber a intenção de pagamento

Identificar o pedido e reconhecer repetições antes de iniciar um novo efeito financeiro.

2

### Validar a conta e reservar o valor

Conferir a elegibilidade e impedir que operações concorrentes utilizem os mesmos recursos.

3

### Encaminhar ao processador externo

Um timeout não prova que a operação falhou. O resultado pode precisar de consulta ou conciliação.

4

### Confirmar ou liberar a reserva

Finalizar de acordo com o resultado confirmado, preservando o histórico e tratando duplicidades.

Este é o fluxo que queremos construir. A comunicação, o modelo de estados e as garantias serão decididos nas respectivas aulas.

## Ganhos esperados e custos assumidos

| Possível ganho | Custo ou condição |
|---|---|
| Publicar uma área de forma independente | Versionar contratos e manter compatibilidade entre serviços. |
| Escalar uma carga específica | Operar mais processos, conexões e infraestrutura. |
| Isolar falhas de integração | Definir timeout, limites, recuperação e observabilidade. |
| Dar responsabilidade clara pelos dados | Tratar sincronização, duplicação controlada e consistência eventual onde forem aceitáveis. |

::: nota Nota
**Sem divisão automática:** uma área de negócio não obriga a criação de um serviço. Saldo, reservas e lançamentos exigem cuidado com consistência. Vamos justificar os limites antes de separá-los.
:::


::: quiz
? O processador não respondeu ao pagamento dentro do tempo limite. O que sabemos?
- O pagamento certamente falhou e pode ser repetido livremente.
- O pagamento certamente foi concluído.
+ O resultado é desconhecido; precisamos de consulta ou conciliação e controle de repetição.
> O efeito externo pode ter ocorrido antes da perda da resposta. Repetir sem controle pode duplicar o pagamento. Vamos estudar idempotência e conciliação quando implementarmos esse fluxo.
:::
