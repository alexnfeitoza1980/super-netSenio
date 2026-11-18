---
id: lab-anfx-guia
order: 264
kind: lab
num: 31
group: Laboratório · Parte 42 · Banco ANFX: Clean Architecture na prática
title: Guia do laboratório
subtitle: Este portal é a memória didática do projeto.
minutes: 4
hue: 352
origem: aula_guiada_mcroservicos_net_ia/portal-anfx/dist/lessons.js
---

Este portal é a memória didática do projeto. A conversa coordena o trabalho; as aulas preservam as explicações para você consultar depois.

## Como navegar

### Encontre o assunto

Use a navegação lateral ou a busca. A busca considera títulos, categorias e o conteúdo das aulas. Pressione / para focar a busca e Escape para limpar ou fechar o menu no celular.

### Retome de onde parou

O endereço identifica a aula. Compartilhe o link ou use os botões de próxima aula. Marque a leitura como concluída para acompanhar seu estudo neste navegador.

## O que significam os status?

| Status | Significado |
|---|---|
| Disponível | A aula tem conteúdo para leitura. Não significa que um módulo bancário está implementado. |
| Concluída | Você marcou a leitura como concluída neste navegador. Não é aprovação de testes ou certificação. |
| A seguir | Planejamento visível, com implementação pendente. |
| Guia | Material de apoio que pode ser consultado a qualquer momento. |

## O compromisso de cada nova aula

- Problema de negócio e comportamento no ANFX Legacy.
- Objetivos e o que realmente foi implementado.
- Decisões de arquitetura e alternativas consideradas.
- Tecnologias utilizadas, ganhos e custos.
- Diagramas e fluxos relevantes.
- Caminhos reais dos arquivos e exemplos comentados.
- Como executar, verificar e reproduzir o resultado.
- Limitações, exercício e próximo passo.

## Onde estão o portal e o código?

```
aula_guiada_microservicos/
├── portal-anfx/
│   ├── dist/              → site pronto, sem etapa de build
│   │   ├── index.html
│   │   ├── styles.css
│   │   ├── lessons.js     → conteúdo das aulas
│   │   └── app.js         → navegação e interações
│   ├── server.mjs         → servidor local e acesso ao código
│   └── README.md          → execução e edição
└── aula_guiada_mcroservicos_net_ia/
    ├── src/              → código .NET existente
    └── tests/            → projetos de testes
```

::: nota Os botões de código abrem o arquivo?
No servidor local do portal, o link “Ver código” exibe uma cópia somente leitura do arquivo atual. Em uma publicação hospedada, o código local não estará acessível; use “Copiar caminho” e localize o arquivo na solução. O caminho é relativo à pasta aula_guiada_mcroservicos_net_ia.
:::

::: nota Meu progresso é sincronizado?
Não. As leituras concluídas ficam no armazenamento local deste navegador, sem conta ou servidor. Se o navegador bloquear esse armazenamento, a navegação continua funcionando e o progresso dura apenas enquanto a página estiver aberta.
:::

::: nota Como acrescentamos uma aula?
Adicione um registro em dist/lessons.js seguindo o modelo em AULA-MODELO.md. Atualize o conteúdo junto com o módulo e mantenha as partes planejadas como “A seguir” até existir implementação verificável.
:::
