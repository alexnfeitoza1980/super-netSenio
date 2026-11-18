# O Evangelho segundo João em C# (Livro 10 da coleção Aprenda C#)

Livro interativo de **estudo**: dez trechos do *Evangelho segundo João*, lidos **versículo por versículo** e transformados em C# **passo a passo**. Cada frase do texto vira algumas linhas de código, e cada capítulo diz, em uma seção própria, **onde a analogia deixa de valer**. Há uma "lupa no grego" (palavras do texto original, com contagens reproduzíveis), um mapa interativo e um laboratório que consulta os dados do texto grego.

A essência do livro é religiosa e o texto é tratado com respeito; o livro é de estudo, e o código **não** define nem explica o que o texto afirma. As explicações, os exemplos, o código e as ilustrações são originais; o livro não substitui a leitura do texto nem a orientação de quem o estuda em sua tradição.

## Abrir o livro

Abra `index.html` em um navegador (funciona sem servidor). Para regenerar as páginas depois de editar `content/`, rode `python build.py` na raiz da coleção. O **mapa vivo** da introdução é o arquivo `mapa.html`, que também abre sozinho.

## Rodar o código

Com o .NET 8 SDK ou superior:

```bash
dotnet test codigo/Joao.sln                                   # todos os testes
dotnet run --project codigo/Joao.Console -- cap124            # exemplos do capítulo 124 (cap124 a cap133)
dotnet run --project codigo/Joao.Console -- exemplo t126      # um exemplo só: t126 = passo a passo, 126 = "indo além"
dotnet run --project codigo/Joao.Console -- onde menō         # concordância (Laboratório 14)
dotnet run --project codigo/Joao.Console -- rota jordao betania
```

A pasta `codigo/` tem uma solução com quatro projetos:

- `Joao.Capitulos`: o código de cada capítulo (`Cap124` a `Cap133`), o **passo a passo** a partir dos versículos (`Texto/`), as soluções dos exercícios (`Exercicios/`), o laboratório (`Lab14/`) e os dados do texto grego (`Dados/lemas.json`, embutido no programa).
- `Joao.Exemplos`: os programas de exemplo cujas saídas aparecem no livro.
- `Joao.Testes`: testes xUnit. Cada frase do passo a passo tem um teste (`TextoTests`), e `ExemplosTests` confere que a saída de cada exemplo é igual à publicada em `Saidas/`. As saídas dos comandos do laboratório também estão em `Saidas/` (`lab_*.txt`); a de `rota` inclui a hora do seu computador.
- `Joao.Console`: o programa de linha de comando.

## Textos bíblicos e licenças

- **Versículos:** *Bíblia Livre* (BLIVRE), © 2018 Diego Santos, Mario Sérgio e Marco Teles, licença [Creative Commons Atribuição 4.0](https://creativecommons.org/licenses/by/4.0/) (ebible.org/porbr2018). **Foram feitos pequenos ajustes de pontuação na exibição** (espaços antes de sinais, um ")" solto em 2,6 e "mesa,e" em 13,12); as notas de rodapé foram omitidas. A edição traduz "Palavra" em Jo 1,1 e anota "equiv. Verbo"; os tipos de C# do capítulo 124 usam o nome tradicional **Verbo**.
- **Texto grego:** SBLGNT (*SBL Greek New Testament*, M. Holmes), licença [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/) (sblgnt.com/license). O texto usado **não traz Jo 7,53–8,11** no corpo.
- **Lemas e análise morfológica:** MorphGNT (J. Tauber), [CC BY-SA 3.0](https://creativecommons.org/licenses/by-sa/3.0/). O arquivo `Dados/lemas.json` **não contém o texto**, apenas contagens por lema e por capítulo (66 lemas, 15.438 palavras), derivadas do MorphGNT, e por isso é compartilhado sob **CC BY-SA 3.0**.
- **Sentidos das palavras gregas:** dicionário de Strong (edição OpenScriptures, CC BY-SA; derivado de Strong, 1890). Os sentidos em português são tradução livre.

O registro completo (com o que foi verificado e o que está pendente) está em `AUDITORIA/fontes-e-licencas.md`.

## Auditoria e registros

A pasta `AUDITORIA/` guarda o que sustenta o livro:

- `registro-de-decisoes.md`: achados, evidências, decisões e pendências.
- `registro-de-analogias.md`: cada analogia, seus limites e a decisão tomada.
- `mapa-de-conceitos.md`: o que cada capítulo ensina e exige.
- `fontes-e-licencas.md`: fontes, edições e licenças.
- `validacao-tecnica.md`: comandos executados e resultados.

## Respeito e cuidado

O Evangelho é um texto religioso central para o cristianismo. Este livro o trata como **objeto de estudo** e separa sempre **texto**, **interpretação**, **analogia** e **realidade técnica**. Nenhum programa do livro mede, compara ou rotula pessoas ou crenças. Os dias de viagem do mapa do Laboratório 14 são **ilustrativos**. O código de segurança do capítulo 130 é material de estudo: em produção, use bibliotecas consolidadas. Os programas não gravam nem enviam nada: tudo roda localmente.

Se algum tema pesar (o capítulo 131 trata de morte), procure alguém de confiança ou apoio profissional. No Brasil: CVV 188 (24 h, gratuito) e SAMU 192.

## Ilustrações e mapa

As ilustrações animadas são geradas por `ilustracoes/livro10.py` (SVG com animações CSS, que respeitam a preferência "reduzir movimento"). O mapa vivo (`mapa.html`) é uma página única, sem dependências externas, que acompanha o tema claro/escuro do livro.
