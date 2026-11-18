---
id: lab-micro-simuladores
order: 256
kind: lab
num: 23
group: Laboratório · Parte 40 · Microsserviços na prática: Gateway, Saga, Circuit Breaker e Tracing
title: Teste rápido: perguntas de entrevista sênior
subtitle: Brinque com os conceitos em tempo real para fixar o aprendizado na memória.
minutes: 5
hue: 241
origem: microservico-senior/site-ensino/index.html
---

::: quiz
? Para que serve o Trace ID no OpenTelemetry e Jaeger?
- Para criptografar o número do cartão de crédito do cliente.
+ Para amarrar todas as chamadas feitas entre diferentes microsserviços em uma mesma linha do tempo.
- Para reiniciar o Docker quando a memória encher.

? O que o Circuit Breaker faz quando está no estado "ABERTO" (Open)?
- Ele tenta chamar a API com mais força 100 vezes seguidas.
+ Ele bloqueia chamadas instantaneamente em 0ms para não travar a aplicação com requisições presas.
- Ele apaga os dados do banco de dados.

? Qual é a principal vantagem do Outbox Pattern?
+ Salvar o pedido e o evento na mesma transação atômica do banco, garantindo que nenhuma mensagem se perca.
- Fazer o CSS do site carregar mais rápido.
- Evitar o uso de contêineres Docker.

? Por que usamos o YARP como API Gateway e o Docker DNS como Service Discovery?
- Para deixar as portas 5001 e 5002 expostas diretamente para qualquer usuário na internet.
+ Para ter uma única entrada segura na porta 5000 (YARP) com rede interna privada, enquanto o Docker DNS resolve os IPs que mudam dinamicamente.
- Porque o Docker não permite criar mais de 1 contêiner sem um proxy.
:::

> Os simuladores interativos (disjuntor, saga e carga) continuam no projeto de origem: `microservico-senior/site-ensino`.
