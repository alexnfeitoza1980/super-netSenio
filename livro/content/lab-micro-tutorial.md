---
id: lab-micro-tutorial
order: 255
kind: lab
num: 22
group: Laboratório · Parte 40 · Microsserviços na prática: Gateway, Saga, Circuit Breaker e Tracing
title: Tutorial Passo a Passo: Como Criar Tudo do Zero
subtitle: O guia definitivo com todos os comandos de terminal, códigos C# no .NET 10, Dockerfiles e docker-compose.yml que usamos para construir este laboratório.
minutes: 7
hue: 232
origem: microservico-senior/site-ensino/index.html
---

::: ilustracao img/lab-micro/hands-on-build-infographic.jpg | Infográfico mostrando o passo a passo de construção do laboratório com .NET 10, Docker, Jaeger e k6
:::

**👁️ Entendendo o mapa da construção:**

- **Passo 1 (.NET 10 SDK):** Criamos as APIs minimalistas em C# com arquitetura leve e rápida.

- **Passo 2 (Docker Containerization):** Empacotamos OrderApi, InventoryApi e PaymentApi em imagens multi-stage isoladas.

- **Passo 3 (Jaeger OpenTelemetry):** Conectamos a telemetria ponta a ponta na porta gRPC 4317 do Jaeger.

- **Passo 4 (k6 Load Testing):** Simulamos tráfego real com usuários virtuais simultâneos e validamos a resiliência no velocímetro de performance!

- [ ] 1. Verificar .NET 10 e Docker instalados
- [ ] 2. Criar a pasta e a Solução (.sln)
- [ ] 3. Criar os projetos Order, Inventory e Payment
- [ ] 4. Adicionar pacotes OpenTelemetry e Polly
- [ ] 5. Programar a InventoryApi (delay 100ms)
- [ ] 6. Programar a PaymentApi (delay 150ms)
- [ ] 7. Programar a OrderApi com Circuit Breaker
- [ ] 8. Criar os Dockerfiles de cada API
- [ ] 9. Montar o docker-compose.yml
- [ ] 10. Executar docker compose up --build -d
- [ ] 11. Rodar o teste de carga com k6
- [ ] 12. Abrir o Jaeger e analisar o trace completo!

**Roteiro Geral da Construção (Passo a Passo)**

| Etapa | O que fazer (Comando / Arquivo) | Por quê? |
|---|---|---|
| **1** | Criar a pasta e a solução com `dotnet new sln -n DistributedTracingDemo`. | Centraliza os 3 projetos em uma única solução para fácil compilação. |
| **2** | Criar as 3 APIs com `dotnet new web -n OrderApi`, `InventoryApi` e `PaymentApi`. | O template `web` gera Minimal APIs limpas, sem controllers pesados. |
| **3** | Instalar os pacotes do `OpenTelemetry` e `Microsoft.Extensions.Http.Polly` via `dotnet add package`. | Habilita o rastreamento automático distribuído e a proteção contra quedas (disjuntor). |
| **4** | Escrever o `Program.cs` de cada microsserviço configurando a porta OTLP 4317. | Define os endpoints de negócio e conecta o envio de telemetria ao Jaeger. |
| **5** | Criar os `Dockerfile` multi-stage baseados no .NET 10 SDK e ASP.NET 10.0 runtime. | Permite que as APIs rodem em containers leves e idênticos em qualquer máquina. |
| **6** | Criar o `docker-compose.yml` unindo as 3 APIs e o container `jaegertracing/all-in-one`. | Sobe o cluster inteiro e a rede interna de Service Discovery com apenas um comando. |
| **7** | Disparar carga com o container `grafana/k6` e abrir o Jaeger na porta `16686`. | Gera tráfego real e visualiza os spans coloridos confirmando o funcionamento perfeito! |

### Passo 1 & 2: Terminal · 1. Criando a Estrutura de Pastas e Projetos no Terminal

Abra seu terminal (PowerShell ou Bash) na pasta onde deseja criar o projeto e execute:

mkdir DistributedTracingDemo cd DistributedTracingDemo dotnet new sln -n DistributedTracingDemo dotnet new web -n OrderApi dotnet new web -n InventoryApi dotnet new web -n PaymentApi dotnet sln add OrderApi/OrderApi.csproj InventoryApi/InventoryApi.csproj PaymentApi/PaymentApi.csproj

### Passo 3: Pacotes NuGet · 2. Instalando as Bibliotecas do OpenTelemetry e Polly

Instale os pacotes que ligam o cronômetro automático do tracing e o disjuntor de segurança:

# Na OrderApi (necessita de Tracing + Polly + HttpClient Instrumentation): dotnet add OrderApi package OpenTelemetry.Extensions.Hosting dotnet add OrderApi package OpenTelemetry.Instrumentation.AspNetCore dotnet add OrderApi package OpenTelemetry.Instrumentation.Http dotnet add OrderApi package OpenTelemetry.Exporter.OpenTelemetryProtocol dotnet add OrderApi package Microsoft.Extensions.Http.Polly # Na InventoryApi e PaymentApi (necessitam de Tracing para receber e devolver o Trace ID): dotnet add InventoryApi package OpenTelemetry.Extensions.Hosting dotnet add InventoryApi package OpenTelemetry.Instrumentation.AspNetCore dotnet add InventoryApi package OpenTelemetry.Exporter.OpenTelemetryProtocol dotnet add PaymentApi package OpenTelemetry.Extensions.Hosting dotnet add PaymentApi package OpenTelemetry.Instrumentation.AspNetCore dotnet add PaymentApi package OpenTelemetry.Exporter.OpenTelemetryProtocol

### Passo 4: Código C# · 3. Programando a InventoryApi (InventoryApi/Program.cs)

A InventoryApi mantém reservas de demonstração em memória, devolve um identificador para a OrderApi e aceita a compensação:

```csharp InventoryApi/Program.cs 📋 Copiar Código
using System.Collections.Concurrent;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

// Configura OpenTelemetry Tracing
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("inventoryapi"))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddOtlpExporter(opt => opt.Endpoint = new Uri("http://jaeger:4317"));
    });

var app = builder.Build();
var reservations = new ConcurrentDictionary<Guid, byte>();

app.MapPost("/inventory/reserve", async () =>
{
    await Task.Delay(100);
    var reservationId = Guid.NewGuid();
    reservations.TryAdd(reservationId, 0);
    return Results.Ok(new { reservationId, message = "Inventory reserved" });
});

app.MapPost("/inventory/release/{reservationId:guid}", (Guid reservationId) =>
{
    return reservations.TryRemove(reservationId, out _)
        ? Results.NoContent()
        : Results.NotFound();
});

app.Run();
```

### Passo 5: Código C# · 4. Programando a PaymentApi (PaymentApi/Program.cs)

A PaymentApi simula o processamento com delay de 150ms e pode devolver HTTP 503 para o laboratório quando a flag de demonstração estiver habilitada:

```csharp PaymentApi/Program.cs 📋 Copiar Código
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("paymentapi"))
    .WithTracing(tracing =>
    {
        tracing
            .AddAspNetCoreInstrumentation()
            .AddOtlpExporter(opt => opt.Endpoint = new Uri("http://jaeger:4317"));
    });

var app = builder.Build();

app.MapPost("/payment/process", async (HttpContext context, IConfiguration configuration) =>
{
    if (context.Request.Query["simulateFailure"] == "true" &&
        configuration.GetValue<bool>("Demo:AllowFailureSimulation"))
    {
        return Results.Problem(
            title: "simulated_payment_failure",
            detail: "Falha de pagamento simulada para demonstrar o Circuit Breaker.",
            statusCode: StatusCodes.Status503ServiceUnavailable);
    }

    await Task.Delay(150);
    return Results.Ok(new { status = "Payment Approved", transactionId = Guid.NewGuid() });
});

app.Run();
```

### Passo 6: Orquestração e Resiliência · 5. Programando a OrderApi com Polly Circuit Breaker (OrderApi/Program.cs)

No projeto, a configuração registra o monitor Polly e um orquestrador de pedidos. O fluxo exige uma chave de idempotência, reserva estoque, processa pagamento e libera a reserva se houver falha:

```csharp OrderApi/Program.cs 📋 Copiar Código
// OrderApi/Program.cs — trecho de registro dos serviços
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<CircuitBreakerMonitor>();
builder.Services.AddScoped<OrderProcessor>();

builder.Services.AddHttpClient("InventoryClient", client =>
{
    client.BaseAddress = new Uri(inventoryApiUrl);
});

builder.Services.AddHttpClient("PaymentClient", client =>
{
    client.BaseAddress = new Uri(paymentApiUrl);
})
.AddPolicyHandler((services, _) =>
    services.GetRequiredService<CircuitBreakerMonitor>().Policy);

// OrderProcessor faz POST /inventory/reserve e depois
// POST /payment/process. Se o pagamento falhar, tenta
// POST /inventory/release/{reservationId}.
// O endpoint POST /order exige Idempotency-Key.
```

### Passo 7: Dockerfiles · 6. Criando os Dockerfiles Multi-stage (.NET 10)

Crie um arquivo chamado `Dockerfile` dentro de cada pasta (`OrderApi/Dockerfile`, `InventoryApi/Dockerfile`, `PaymentApi/Dockerfile`):

```dockerfile OrderApi/Dockerfile 📋 Copiar Código
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["OrderApi.csproj", "./"]
RUN dotnet restore "OrderApi.csproj"
COPY . .
RUN dotnet publish "OrderApi.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV ASPNETCORE_HTTP_PORTS=8080
ENTRYPOINT ["dotnet", "OrderApi.dll"]
```

*(Faça o mesmo para a `InventoryApi` trocando `OrderApi.csproj` por `InventoryApi.csproj`, e para a `PaymentApi`).*

### Passo 8: Orquestração Docker · 7. Criando o docker-compose.yml Completo

Na raiz da pasta `DistributedTracingDemo`, crie o arquivo `docker-compose.yml` unindo o Gateway, as APIs e o Jaeger:

```yaml docker-compose.yml 📋 Copiar Código
services:
  jaeger:
    image: jaegertracing/all-in-one:1.76.0
    container_name: jaeger
    ports:
      - "16686:16686" # Interface Web do Jaeger
      - "4317:4317"   # Entrada OTLP gRPC
      - "4318:4318"   # Entrada OTLP HTTP
    environment:
      - COLLECTOR_OTLP_ENABLED=true

  gatewayapi:
    build:
      context: .
      dockerfile: GatewayApi/Dockerfile
    container_name: gatewayapi
    ports:
      - "5000:8080" # Entrada do laboratório; não habilita autenticação
    environment:
      - OTEL_EXPORTER_OTLP_ENDPOINT=http://jaeger:4317
      - OTEL_EXPORTER_OTLP_PROTOCOL=grpc
    depends_on:
      - jaeger
      - orderapi

  orderapi:
    build:
      context: .
      dockerfile: OrderApi/Dockerfile
    container_name: orderapi
    # Sem portas publicadas no host; serviços do Compose ainda podem acessá-la.
    environment:
      - InventoryApiUrl=http://inventoryapi:8080 # Service Discovery DNS
      - PaymentApiUrl=http://paymentapi:8080     # Service Discovery DNS
      - Demo__AllowFailureSimulation=true
      - Demo__AllowCircuitBreakerReset=true
      - OTEL_EXPORTER_OTLP_ENDPOINT=http://jaeger:4317
      - OTEL_EXPORTER_OTLP_PROTOCOL=grpc
    depends_on:
      - jaeger
      - inventoryapi
      - paymentapi

  inventoryapi:
    build:
      context: .
      dockerfile: InventoryApi/Dockerfile
    container_name: inventoryapi
    # Sem portas publicadas no host.
    environment:
      - OTEL_EXPORTER_OTLP_ENDPOINT=http://jaeger:4317
      - OTEL_EXPORTER_OTLP_PROTOCOL=grpc
    depends_on:
      - jaeger

  paymentapi:
    build:
      context: .
      dockerfile: PaymentApi/Dockerfile
    container_name: paymentapi
    # Sem portas publicadas no host.
    environment:
      - OTEL_EXPORTER_OTLP_ENDPOINT=http://jaeger:4317
      - OTEL_EXPORTER_OTLP_PROTOCOL=grpc
    depends_on:
      - jaeger

  blazorfrontend:
    build:
      context: .
      dockerfile: BlazorFrontend/Dockerfile
    container_name: blazorfrontend
    ports:
      - "3000:80"
    depends_on:
      - gatewayapi
```

### Passo 9 & 10: Execução e Validação · 8. Subindo o Ambiente e Executando o Teste de Carga com k6

1. Suba todos os contêineres com um único comando no terminal:

docker compose up --build -d

2. Crie o arquivo `loadtest.js` e execute o k6 simulando 10 usuários virtuais por 30 segundos:

docker run --rm -i -v "${PWD}/loadtest.js:/loadtest.js" grafana/k6 run /loadtest.js

3. Abra seu navegador em **http://localhost:16686**, selecione o serviço `orderapi` e clique em **Find Traces**! Você verá a árvore completa com os 3 serviços e a medição exata do tempo de execução! 🎉
