"""Importa os livros C# (1–10) e Angular (5) do repositório `livro-csharp-senior` (snapshot do Git HEAD)."""
from __future__ import annotations

import json
import re
from pathlib import Path

from comum import Capitulo, Imagem, Resultado, separar_frontmatter

# slug, pasta relativa ao snapshot, rótulo da área no sumário
LIVROS_CSHARP = [
    ("aprenda", "livros/01-aprenda-csharp", "C#"),
    ("agil", "livros/02-projeto-agil-csharp", "C#"),
]
LIVROS_MENTALIDADE = [
    ("leis", "livros/03-leis-do-sucesso-csharp", "C# & Mentalidade"),
    ("foco", "livros/04-foco-em-csharp", "C# & Mentalidade"),
    ("compromissos", "livros/05-quatro-compromissos-csharp", "C# & Mentalidade"),
    ("resiliencia", "livros/06-resiliencia-csharp", "C# & Mentalidade"),
    ("crencas", "livros/07-crencas-csharp", "C# & Mentalidade"),
    ("amor", "livros/08-voce-e-aquilo-que-ama-csharp", "C# & Mentalidade"),
    ("emocional", "livros/09-inteligencia-emocional-csharp", "C# & Mentalidade"),
    ("joao", "livros/10-evangelho-joao-csharp", "C# & Mentalidade"),
]
LIVROS_ANGULAR = [
    ("ang-pratica", "angular/livros/01-biblioteca-angular", "Angular"),
    ("ang-antimedo", "angular/livros/01-antimedo-angular", "Angular"),
    ("ang-atencao", "angular/livros/01-atencao-plena-angular", "Angular"),
    ("ang-cnv", "angular/livros/02-cnv-angular", "Angular"),
    ("ang-amigos", "angular/livros/03-amigos-angular", "Angular"),
]
TODOS = LIVROS_CSHARP + LIVROS_MENTALIDADE + LIVROS_ANGULAR

# "Parte 5 · Dados" (C#) ou "Parte II · Técnicas" (Angular, algarismos romanos)
_PARTE = re.compile(r"^Parte\s+([IVXLCDM]+|\d+)\s*[·\-—:]\s*(.+)$")
_IGNORAR_COPIA = {"bin", "obj", "node_modules", ".vs", "TestResults", ".angular", "dist", ".git"}


# Correções pontuais de defeitos que JÁ existiam nos livros de origem. Cada troca precisa casar exatamente 1 vez.
CORRECOES: dict[str, list[tuple[str, str]]] = {
    # Bloco ```suggestion aninhado dentro de ```markdown: o motor só fecha cerca com três crases,
    # então o bloco externo fechava cedo e o resto do capítulo virava "código".
    # Faltava o `:::` que fecha o quiz: o bloco `cartas` ficava aninhado dentro dele e os cartões sumiam.
    "joao-cap125": [(
        "> O `OrderBy` usa o `CompareTo` do tipo.\n\n::: cartas\n",
        "> O `OrderBy` usa o `CompareTo` do tipo.\n:::\n\n::: cartas\n",
    )],
    "ang-amigos-cap49": [(
        "```suggestion\n  readonly isOpen = model<boolean>(false);\n```\n",
        "~~~suggestion\n  readonly isOpen = model<boolean>(false);\n~~~\n",
    )],
}


def _aplicar_correcoes(cap_id: str, md: str) -> str:
    for antigo, novo in CORRECOES.get(cap_id, []):
        n = md.count(antigo)
        if n != 1:
            raise SystemExit(f"correção em {cap_id}: esperava 1 ocorrência, achei {n} (a origem mudou?)")
        md = md.replace(antigo, novo)
    return md


def _pasta_para_slug() -> dict[str, str]:
    return {Path(p).name: slug for slug, p, _ in TODOS}


def _reescrever(md: str, slug: str, ids_do_livro: set[str]) -> str:
    # Ilustrações e imagens: cada livro vive em img/<slug>/ (nomes como cap01.svg repetem entre livros).
    md = re.sub(r"(?<![\w/.-])assets/images/", f"img/{slug}/", md)
    md = re.sub(r"(?<![\w/.-])img/(?!%s/)" % re.escape(slug), f"img/{slug}/", md)
    # Código que acompanha o livro.
    md = re.sub(r"(?<![\w/.-])codigo/(?!%s/)" % re.escape(slug), f"codigo/{slug}/", md)
    # Páginas HTML que acompanham o livro (ex.: o mapa vivo do livro de João) vão para codigo/<slug>/.
    md = md.replace('src="mapa.html"', f'src="codigo/{slug}/mapa.html"')
    # Links entre livros: ../NN-livro/index.html -> introdução daquele livro.
    mapa = _pasta_para_slug()

    def entre_livros(m: re.Match) -> str:
        alvo = mapa.get(m.group(1))
        return f"#/c/{alvo}-intro" if alvo else m.group(0)

    md = re.sub(r"(?:\.\./)+([\w-]+)/index\.html(?:#[^)\s\"']*)?", entre_livros, md)
    # Links para capítulos do próprio livro: ids ganham o prefixo do livro.
    md = re.sub(r"#/c/([\w-]+)", lambda m: f"#/c/{slug}-{m.group(1)}" if m.group(1) in ids_do_livro else m.group(0), md)
    return md


def _copiar_arvore(origem: Path, destino_rel: str, res: Resultado) -> None:
    for f in sorted(origem.rglob("*")):
        if f.is_file() and not (set(f.relative_to(origem).parts) & _IGNORAR_COPIA):
            res.arquivos.append(Imagem(f, f"{destino_rel}/{f.relative_to(origem).as_posix()}"))


def importar(snapshot: Path, slugs: list[str] | None = None) -> Resultado:
    res = Resultado()
    for slug, pasta, macro in TODOS:
        if slugs and slug not in slugs:
            continue
        base = snapshot / pasta
        meta_livro = json.loads((base / "livro.json").read_text(encoding="utf-8"))
        titulo_livro = meta_livro["titulo"]
        res.titulos[slug] = titulo_livro
        brutos = []
        for f in sorted((base / "content").glob("*.md")):
            lido = separar_frontmatter(f.read_text(encoding="utf-8"))
            if not lido:
                res.avisos.append(f"{slug}/{f.name}: sem frontmatter, ignorado")
                continue
            brutos.append((int(lido[0].get("order", "999")), f, lido[0], lido[1]))
        brutos.sort(key=lambda t: t[0])
        ids = {m["id"] for _, _, m, _ in brutos}

        for _, f, m, corpo in brutos:
            kind = m["kind"]
            grupo = m.get("group", "")
            pm = _PARTE.match(grupo)
            # Só os livros C# têm numeração de partes que vale preservar (o glossário e textos a citam).
            # Nos demais, a numeração é refeita em sequência na montagem; o prefixo "Parte I ·" sai do título.
            parte_num = int(pm.group(1)) if pm and pm.group(1).isdigit() and macro.startswith("C#") else None
            parte = pm.group(2).strip() if pm else (grupo or titulo_livro)
            # Parte que nunca teve número em um livro C# (ex.: "Oficina · ...") fica sem "Parte N".
            sem_numero = (not pm) and macro.startswith("C#") and kind in ("capitulo", "lab")

            titulo = m["title"]
            if kind == "intro":
                titulo = f"{titulo_livro} — apresentação"
            elif kind == "extra" and m["id"] == "encerramento":
                kind, titulo = "apendice", f"Encerramento — {titulo_livro}"
            elif kind == "apendice" and titulo_livro not in titulo:
                titulo = f"{titulo} ({titulo_livro})"

            conhecidas = {"id", "order", "kind", "num", "group", "title", "subtitle", "minutes", "hue",
                          "objectives", "badge", "image"}
            res.capitulos.append(Capitulo(
                id=f"{slug}-{m['id']}", kind=kind, title=titulo,
                md=_aplicar_correcoes(f"{slug}-{m['id']}", _reescrever(corpo, slug, ids)),
                macro=macro, parte=parte, parte_num=parte_num, sem_numero=sem_numero,
                subtitle=m.get("subtitle", ""), minutes=int(m.get("minutes", "20")), hue=int(m.get("hue", "262")),
                objectives=m["objectives"], badge=m.get("badge", ""),
                num_original=int(m["num"]) if m.get("num", "").isdigit() else None,
                image=re.sub(r"^(?!img/)", f"img/{slug}/", m["image"]) if m.get("image") else "",
                extras={k: v for k, v in m.items() if k not in conhecidas},
                origem=f"{pasta}/content/{f.name}", slug=slug,
            ))

        # Apresentação entra na primeira parte do livro e apêndices/encerramento na última: nenhum deles vira "parte" própria.
        deste_livro = [c for c in res.capitulos if c.slug == slug]
        corpo_do_livro = [c for c in deste_livro if c.kind in ("capitulo", "lab")]
        if corpo_do_livro:
            for c in deste_livro:
                alvo = corpo_do_livro[0] if c.kind == "intro" else corpo_do_livro[-1] if c.kind == "apendice" else None
                if alvo:
                    c.parte, c.parte_num, c.sem_numero = alvo.parte, alvo.parte_num, alvo.sem_numero

        # Arquivos que acompanham o livro: imagens, fotos e o código dos exemplos.
        for sub in ("img", "assets/images"):
            if (base / sub).is_dir():
                _copiar_arvore(base / sub, f"img/{slug}", res)
        if (base / "codigo").is_dir():
            _copiar_arvore(base / "codigo", f"codigo/{slug}", res)
        if (base / "mapa.html").is_file():
            res.arquivos.append(Imagem(base / "mapa.html", f"codigo/{slug}/mapa.html"))
        if (base / "README.md").is_file():
            res.arquivos.append(Imagem(base / "README.md", f"codigo/{slug}/README-do-livro.md"))
    return res
