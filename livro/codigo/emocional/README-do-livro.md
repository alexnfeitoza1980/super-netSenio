# Inteligência Emocional em C# (Livro 9 da coleção Aprenda C#)

Livro interativo **inspirado** em temas amplos de *Inteligência Emocional*, de Daniel Goleman. As cenas, explicações, exercícios, ilustrações e todo o código são originais; o livro não substitui nem resume a obra. Os temas são tratados como **lente de prática** (perceber, compreender, escolher), e não como ciência, terapia ou diagnóstico.

## Abrir o livro

Abra `index.html` em um navegador (funciona sem servidor). Para regenerar as páginas depois de editar `content/`, rode `python build.py` na raiz da coleção. As ilustrações vêm de `ilustracoes/livro09.py` (`python ilustracoes/gerar.py livro09`).

## Código

`codigo/` tem uma solução do Visual Studio com três projetos:

- `Emocoes.Core`: os tipos e regras apresentados nos capítulos 116 a 123.
- `Emocoes.Console`: a demonstração do Laboratório 13 (**Roteiro de Pausa e Conversa**), com dados fictícios.
- `Emocoes.Testes`: testes xUnit de validação e de apresentação do roteiro.

Com o .NET 8 SDK ou superior:

```bash
dotnet test codigo/Emocoes.sln
dotnet run --project codigo/Emocoes.Console
```

O console mostra um cenário fictício definido em `Program.cs`: não pede relatos, não lê arquivos, não grava dados e não usa a rede. Os trechos de código mostrados nos capítulos vêm desses arquivos.

## Privacidade e cuidado

Nenhum modelo deste livro infere sentimentos, mede inteligência emocional, pontua, diagnostica ou compara pessoas: eles guardam apenas o que o chamador informa, na memória. As atividades são opcionais, e pular é sempre permitido. Se um tema pesar, procure alguém de confiança ou apoio profissional. No Brasil: CVV 188 (24 h, gratuito) e SAMU 192.
