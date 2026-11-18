---
id: arq-15-intro
order: 230
kind: intro
group: Arquitetura .NET · Parte 33 · Performance
title: Performance — visão geral
subtitle: Bem-vindo ao décimo quinto capítulo.
minutes: 5
hue: 354
origem: Arquitetura-microservicos.NET/15-performance/README.md
---

Bem-vindo ao décimo quinto capítulo. Aqui levamos o runtime do **.NET 10** e o compilador C# ao limite da física da computação, dominando a medição científica com **BenchmarkDotNet**, a gestão interna de gerações do Garbage Collector (Gen 0, 1, 2, LOH e POH) e a programação com alocação zero através de **`Span<T>`**, **`ArrayPool<T>`** e **`ValueTask<T>`**.

## 📑 Aulas e Tópicos Deste Capítulo

1. **[Performance Extrema, Profiling e Otimizações de Memória no .NET 10](#/c/arq-15-01-profiling-e-otimizacoes-dotnet)**
   - Medição científica sem achismos: BenchmarkDotNet com `[MemoryDiagnoser]`.
   - As entranhas do Garbage Collector: Gen 0, Gen 1, Gen 2, Large Object Heap (LOH) e Pinned Object Heap (POH).
   - Zero-allocation com `ReadOnlySpan<char>` e `Span<T>`.
   - Reutilização de buffers com `ArrayPool<T>.Shared`.
   - A decisão cirúrgica: quando usar `Task<T>` vs `ValueTask<T>`.

---
⬅️ Voltar para o **[Capítulo 14: Gateway / BFF](#/c/arq-14-intro)** | Avançar para o **[Capítulo 16: C# / .NET para Entrevistas Sênior](#/c/arq-16-intro)** ➡️
