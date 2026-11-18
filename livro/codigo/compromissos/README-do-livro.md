# O Caderno dos Quatro Compromissos

Livro 5 reconstruído como uma experiência contínua em C#/.NET. Capítulos 74–87 e laboratório 9 preservam os identificadores da coleção. Leia o index.html; fontes editoriais em content/, código em codigo/, imagens em img/.

## Executar (a partir desta pasta)

Requer .NET SDK 10 e Python 3 para o verificador editorial. Sem pacotes NuGet externos.

```bash
dotnet build codigo/Caderno.slnx -m:1
dotnet run --project codigo/Caderno.Console -- cap74
dotnet run --project codigo/Caderno.Console -- cap87
dotnet run --project codigo/Caderno.Testes
python ferramentas/verificar.py
python ferramentas/testar-console.py
```

Caderno.Testes é uma suíte executável própria, não xUnit. dotnet test não executa seus casos. Os cenários cap74 a cap87 começam com dados novos; as saídas publicadas ficam em saidas/.

```bash
dotnet run --project codigo/Caderno.Console -- jornada minha-jornada.json
dotnet run --project codigo/Caderno.Console -- novo meu-caderno.json
dotnet run --project codigo/Caderno.Console -- praticar meu-caderno.json "Perguntei antes de supor" "A resposta mudou meu plano"
dotnet run --project codigo/Caderno.Console -- ler meu-caderno.json
```

A jornada guiada percorre os quatro temas com respostas do leitor e salva novos registros de todas as entidades sem apagar as jornadas anteriores. O diário rápido permite acrescentar práticas. Ainda não há menu para alterar registros antigos. A data das práticas pessoais vem do computador.

## Escopo e limites

Um núcleo em C#, um console e uma suíte de verificações. Arquivo local simples, sem criptografia, transações, acesso compartilhado ou validação profunda de importações. Recupera um retrato de dados, não reconstitui automaticamente entidades mutáveis. Banco, API e Angular são próximos passos documentados, não recursos entregues.

Textos explicativos, exemplos e ilustrações editoriais são próprios, inspirados em Don Miguel Ruiz. A edição exata do PDF fornecido não foi identificada: as referências usam páginas físicas. Nenhum PDF de terceiros é necessário para executar ou distribuído por esta entrega. Consulte ESTUDO/fontes.md e ESTUDO/decisoes.md.
