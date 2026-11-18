---
id: lab-micro-comunicacao
order: 249
kind: lab
num: 16
group: Laboratório · Parte 40 · Microsserviços na prática: Gateway, Saga, Circuit Breaker e Tracing
title: Comunicação: Síncrona (HTTP/gRPC) vs Assíncrona (Mensageria)
subtitle: Quando você deve esperar a resposta na hora e quando deve apenas mandar uma mensagem e seguir em frente.
minutes: 5
hue: 178
origem: microservico-senior/site-ensino/index.html
---

::: cuidado O Problema no Mundo Real
Se a cada clique do usuário sua API precisa chamar: 1. Estoque (espera 100ms) + 2. Pagamento (espera 150ms) + 3. Nota Fiscal (espera 500ms) + 4. Disparo de E-mail (espera 800ms) + 5. Notificação Push no celular (espera 400ms)...

> O tempo total passa de 2 segundos! E o pior: se o servidor de e-mail estiver fora do ar, **a compra inteira falha** mesmo que o cartão já tenha sido cobrado!
:::

::: dica O Nosso Objetivo
Dividir a comunicação em duas categorias inteligentes:

- **Síncrona:** Usar apenas para o que o usuário *precisa saber imediatamente* para continuar (ex: saldo suficiente no cartão).
- **Assíncrona:** Usar filas (RabbitMQ, Kafka, SQS) para tudo que pode acontecer nos próximos segundos sem travar a tela (ex: gerar nota fiscal, enviar e-mail de obrigado).
:::

::: ilustracao img/lab-micro/sync-async-infographic.jpg | Infográfico comparativo de comunicação síncrona bloqueante versus assíncrona desacoplada com filas
:::

**👁️ Entendendo a imagem num piscar de olhos:**

- **Lado Esquerdo (Síncrono / HTTP):** O usuário Alex fica preso olhando para a ampulheta. Cada serviço (User ➜ Order ➜ Payment ➜ Inventory) precisa responder em fila indiana. Tempo total de espera: **2000ms travados**! Se um falhar, a compra inteira cai.

- **Lado Direito (Assíncrono / Mensageria):** O usuário clica em "Comprar" e recebe confirmação verde em apenas **50ms**! A mensagem cai na esteira (RabbitMQ / Kafka) e robôs trabalhadores cuidam da separação e envio em segundo plano.

::: analogia vs 💬 Exemplo do Dia a Dia: Ligação Telefônica vs Mensagem no WhatsApp
#### 📞 Síncrono = Ligação Telefônica

Você liga para alguém. Você fala "Alô", e precisa que a pessoa esteja com o telefone no ouvido respondendo "Alô" naquele exato instante.

Se a pessoa não atender ou o sinal cair, a conversa não acontece e você fica esperando no mudo.

#### 💬 Assíncrono = Mensagem no WhatsApp

Você envia: "Segue o comprovante de pagamento". Você **não fica olhando para a tela** esperando a pessoa ler.

Você guarda o celular no bolso e vai almoçar. Quando a pessoa estiver disponível, ela lê a mensagem e processa o que tiver que processar.
:::

**2️⃣ Comparativo Passo a Passo: Quando Usar Cada Uma**

| Tipo | Como Funciona | Vantagens | Quando Usar? |
|---|---|---|---|
| Síncrono (HTTP / gRPC) | O serviço A chama o serviço B e **fica travado esperando** a resposta antes de prosseguir. | Simples de entender; resposta imediata para o usuário. | Consultas em tempo real (ex: checar login, validar senha, consultar saldo). |
| Assíncrono (Mensageria / Eventos) | O serviço A publica uma mensagem em uma fila (RabbitMQ/Kafka) e **continua sua vida imediatamente**. | Altíssimo desempenho; se o serviço de destino cair, a mensagem fica guardada na fila e não se perde. | Processamento de pedidos, envio de e-mails, emissão de nota fiscal, geração de relatórios. |

#### Fluxograma: Comparando o Bloqueio Síncrono vs Desacoplamento Assíncrono

```mermaid
flowchart TD
    subgraph Sincrono ["🔴 Modelo Síncrono (Bloqueante)"]
        A1[Cliente] -->|1. Faz Pedido| B1[OrderApi]
        B1 -->|2. Bloqueia e Espera| C1[PaymentApi]
        C1 -->|3. Bloqueia e Espera| D1[EmailService]
        D1 -.->|4. Responde 800ms| C1
        C1 -.->|5. Responde 150ms| B1
        B1 -.->|6. Responde 1000ms| A1
    end

    subgraph Assincrono ["🟢 Modelo Híbrido Recomendado (Sênior)"]
        A2[Cliente] -->|1. Faz Pedido| B2[OrderApi]
        B2 -->|2. Valida Imediato HTTP| C2[PaymentApi]
        C2 -.->|3. Cartão OK 150ms| B2
        B2 -->|4. Publica Evento em 5ms| Q[(Fila RabbitMQ)]
        B2 -.->|5. Responde Pedido Criado 160ms!| A2
        Q -.->|6. Consome quando puder| D2[EmailService]
        Q -.->|7. Consome quando puder| E2[FiscalService]
    end
```
