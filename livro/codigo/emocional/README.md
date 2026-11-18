# Inteligência Emocional em C#

Exemplos executáveis e testes do Livro 9 da coleção Aprenda C#.

## Requisitos

- .NET 8 SDK
- Acesso à internet somente para a primeira restauração do pacote xUnit

## Executar

Na pasta `codigo`:

```powershell
dotnet test Emocoes.sln
dotnet run --project Emocoes.Console
```

O console mostra um cenário fictício definido em `Emocoes.Console/Program.cs`. Ele não solicita relatos, lê arquivos, grava dados ou se conecta à rede.

## Organização

- `Emocoes.Core`: tipos e regras explícitas apresentados nos capítulos 116–123.
- `Emocoes.Console`: demonstração do Laboratório 13.
- `Emocoes.Testes`: testes de validação e de apresentação do roteiro.

Os modelos guardam apenas os dados fornecidos pelo chamador. Não há classificação automática, diagnóstico, pontuação de pessoas, análise de texto, webcam ou persistência.
