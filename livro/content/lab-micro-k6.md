---
id: lab-micro-k6
order: 254
kind: lab
num: 21
group: Laboratório · Parte 40 · Microsserviços na prática: Gateway, Saga, Circuit Breaker e Tracing
title: Testes de Carga com k6 & Diagnóstico de Gargalos
subtitle: Como testar se o sistema aguenta o tráfego real e encontrar os pontos lentos usando o Jaeger.
minutes: 5
hue: 223
origem: microservico-senior/site-ensino/index.html
---

::: cuidado O Problema no Mundo Real
No computador do desenvolvedor, testar 1 clique na API parece super rápido ("na minha máquina funciona!"). Mas no dia do lançamento, 5.000 clientes entram ao mesmo tempo.

> Sem testes de carga prévios, o servidor atinge 100% de CPU, o banco entra em deadlock e a loja perde milhões de reais em vendas ao vivo.
:::

::: dica O Nosso Objetivo
Usar a ferramenta **k6** para simular dezenas de usuários virtuais simultâneos (VUs) fazendo pedidos sem parar por 30 segundos.

> Ao mesmo tempo em que a carga bate nas APIs, abrimos o **Jaeger** para cruzar os dados e responder cientificamente: *"Quantas requisições por segundo aguentamos?"* e *"Qual serviço foi o vilão da lentidão?"*
:::

::: analogia Exemplo do Dia a Dia: O Teste de Carga de Peso em uma Nova Ponte
Antes de liberar uma grande ponte estaiada para a população:

- Os engenheiros colocam **40 caminhões pesados carregados de areia** parados em cima da pista ao mesmo tempo.
- Sensores medem o quanto os cabos de aço esticam e se há vibrações perigosas.
- **O k6 é o caminhão de areia do software:** Ele força o limite máximo do sistema em um ambiente controlado para garantir que a ponte não caia na hora do rush!
:::

**7️⃣ Ideia Geral (Passo a Passo do que Executamos no Lab)**

| Etapa | O que acontece | Por quê? |
|---|---|---|
| **1** | Criamos o script `loadtest.js` configurando 10 Usuários Virtuais (`vus: 10`) rodando por 30 segundos (`duration: '30s'`). | Define o volume de estresse que queremos aplicar de forma padronizada. |
| **2** | Executamos o k6 apontando para `http://host.docker.internal:5000/order`. | Gera um bombardeio de requisições reais contra a nossa `OrderApi` no Docker. |
| **3** | A `OrderApi` processa cada pedido chamando a `InventoryApi` (100ms) e a `PaymentApi` (150ms). | Simula o comportamento de microsserviços reais com regras de negócio e processamento de banco. |
| **4** | O OpenTelemetry gera milhares de Spans e envia via gRPC para o Jaeger. | Alimenta o Jaeger com métricas reais de cada uma das centenas de requisições feitas. |
| **5** | O k6 apresenta a duração e a taxa de falhas observadas naquela execução. | Os resultados dependem da máquina e da execução; não são valores fixos nem uma garantia de zero erros. |
| **6** | Abrimos o Jaeger na porta 16686 e inspecionamos os traces gerados durante a carga. | Confirma visualmente a saúde do fluxo e prova que o gargalo foi o tempo simulado do pagamento (150ms). |

```javascript O Script loadtest.js que rodamos no laboratório JavaScript / k6
import http from 'k6/http';
import { check, sleep } from 'k6';

export const options = {
  vus: 10,          // 10 usuários simultâneos
  duration: '30s',  // Durante 30 segundos ininterruptos
};

export default function () {
  // Use uma chave diferente para cada pedido, inclusive entre VUs e iterações.
  const res = http.post('http://host.docker.internal:5000/order', null, {
    headers: {
      'Idempotency-Key': `loadtest-${__VU}-${__ITER}`,
    },
  });

  // Valida se o status retornado foi 200 OK
  check(res, {
    'status is 200': (r) => r.status === 200,
  });

  sleep(0.1); // Pequena pausa entre pedidos
}
```
