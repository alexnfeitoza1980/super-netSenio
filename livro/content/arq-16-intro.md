---
id: arq-16-intro
order: 232
kind: intro
group: Arquitetura .NET · Parte 34 · C# / .NET para Entrevistas Sênior
title: C# / .NET para Entrevistas Sênior — visão geral
subtitle: Bem-vindo ao décimo sexto capítulo.
minutes: 5
hue: 5
origem: Arquitetura-microservicos.NET/16-csharp-dotnet-entrevistas/README.md
---

Bem-vindo ao décimo sexto capítulo. Esta é a nossa trilha intensiva de domínio da linguagem **C# moderno** e do runtime **.NET 10**, projetada especificamente para capacitar você a gabaritar entrevistas técnicas de nível Sênior e Especialista, compreendendo os mecanismos internos de execução, máquinas de estado assíncronas, concorrência fina e gestão cirúrgica de memória.

## 📑 Aulas e Tópicos Deste Capítulo

1. **[Mecânica Interna do Async/Await, State Machine e CancellationToken no .NET 10](#/c/arq-16-01-async-await-cancellation-token)**
   - O que o compilador faz por baixo dos panos com a struct `IAsyncStateMachine`.
   - ThreadPool Starvation: o perigo mortal de `.Result` e `.Wait()`.
   - Propagação de `CancellationToken` do Kestrel até o motor SQL.
   - Concorrência moderna de produtor/consumidor com `System.Threading.Channels`.
2. **[Gestão de Memória, Boxing/Unboxing, IAsyncDisposable e BackgroundServices](#/c/arq-16-02-memory-e-recursos)**
   - Stack vs Heap: alocações efêmeras vs gerenciadas.
   - O impacto de Boxing silencioso em hot-paths.
   - O padrão `IAsyncDisposable` com `await using` para I/O não-bloqueante.
   - `BackgroundService` em produção: usando `IServiceScopeFactory` para evitar Captive Dependencies.
3. **[Top 10 Perguntas Quentes de C# e .NET para Entrevistas Sênior](#/c/arq-16-03-perguntas-entrevistas-senior)**
   - `IEnumerable<T>` vs `IQueryable<T>` (memória vs tradução SQL).
   - Vazamentos de memória por Closures em Lambdas.
   - Sincronização de concorrência: `lock` vs `SemaphoreSlim` vs `Mutex`.
   - Por que evitar `Task.Run` no ASP.NET Core e por que nunca usar `async void`.
   - Devirtualização de chamadas via classes `sealed`.

---
⬅️ Voltar para o **[Capítulo 15: Performance](#/c/arq-15-intro)** | Avançar para o **[Capítulo 17: Cloud](#/c/arq-17-intro)** ➡️
