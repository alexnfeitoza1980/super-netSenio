"""Checagens de integridade do livro único. Erros reprovam o build; avisos só informam."""
from __future__ import annotations

import re
from collections import Counter
from pathlib import Path

KINDS = {"intro", "capitulo", "lab", "apendice", "extra", "glossario"}
ESPECIAIS = {"intro", "folha-de-cola", "glossario"}


def _sem_codigo(md: str) -> str:
    """Remove blocos de código (``` ... ```), onde `:::` e caminhos não têm significado de estrutura."""
    # Mesma regra do motor: só vale cerca na coluna 0; fecha com ``` sozinho na linha.
    saida, dentro = [], False
    for linha in md.split("\n"):
        if not dentro and linha.startswith("```"):
            dentro = True
            continue
        if dentro:
            if re.match(r"^```\s*$", linha):
                dentro = False
            continue
        saida.append(linha)
    return "\n".join(saida)


def validar(raiz: Path, caps: list[dict]) -> int:
    erros: list[str] = []
    avisos: list[str] = []
    ids = [c["id"] for c in caps]

    # ---- estrutura
    for i, n in Counter(ids).items():
        if n > 1:
            erros.append(f"id repetido: {i} ({n}x)")
    orders = [int(c["order"]) for c in caps]
    if len(set(orders)) != len(orders):
        erros.append("há capítulos com o mesmo 'order'")
    for c in caps:
        if c["kind"] not in KINDS:
            erros.append(f"{c['id']}: kind desconhecido '{c['kind']}'")
        if not c.get("title"):
            erros.append(f"{c['id']}: sem título")
        if not c.get("group"):
            erros.append(f"{c['id']}: sem group")
    for k in ("capitulo", "lab"):
        nums = [c["num"] for c in caps if c["kind"] == k and c["num"]]
        for n, q in Counter(nums).items():
            if q > 1:
                erros.append(f"{k}: número {n} repetido")
        sem = [c["id"] for c in caps if c["kind"] == k and not c["num"]]
        if sem:
            avisos.append(f"{k} sem número: {', '.join(sem[:5])}")
    for e in ESPECIAIS:
        if e not in ids:
            erros.append(f"falta o capítulo especial '{e}'")

    # ---- conteúdo
    idset = set(ids)
    for c in caps:
        md = c["md"]
        fora = _sem_codigo(md)

        # cercas de código: mesma regra do motor (só vale ``` na coluna 0; fecha com ``` sozinho na linha)
        aberta = False
        for linha in md.split("\n"):
            if not aberta and linha.startswith("```"):
                aberta = True
            elif aberta and re.match(r"^```\s*$", linha):
                aberta = False
        if aberta:
            erros.append(f"{c['id']}: bloco ``` sem fechamento")

        # blocos ::: com a MESMA regex do motor: abre com `:::tipo` (espaço opcional), fecha com `:::` sozinho
        # (`[ \t]*` e não `\s*`: o motor lê linha a linha; `\s` atravessaria a quebra de linha e contaria texto como abertura)
        abre = len(re.findall(r"^:::[ \t]*[\w-]+", fora, re.M))
        fecha = len(re.findall(r"^:::[ \t]*$", fora, re.M))
        if abre != fecha:
            avisos.append(f"{c['id']}: blocos ':::' abrem {abre} e fecham {fecha}")

        # links internos
        for alvo in re.findall(r"#/c/([\w-]+)", md):
            if alvo not in idset:
                erros.append(f"{c['id']}: link para capítulo inexistente '{alvo}'")

        # arquivos locais referenciados (ilustrações, imagens, src/href)
        refs = set(re.findall(r"^:::\s+ilustracao\s+(\S+)", fora, re.M))
        refs |= set(re.findall(r"!\[[^\]]*\]\(([^)\s]+)\)", fora))
        # só tags que carregam arquivo de verdade (um `<script src>` dentro de um trecho didático não conta)
        refs |= set(re.findall(r"""<(?:img|iframe|video|audio|source)\b[^>]*?\bsrc=["']([^"']+)["']""", fora))
        for r in refs:
            if re.match(r"^(https?:|data:|#|//)", r) or "{{" in r:  # {{ }} é interpolação Angular dentro de texto
                continue
            if not (raiz / r.split("#")[0].split("?")[0]).is_file():
                erros.append(f"{c['id']}: arquivo não existe: {r}")
        if c.get("image") and not (raiz / c["image"]).is_file():
            avisos.append(f"{c['id']}: image do frontmatter não existe: {c['image']}")

        # restos de caminhos dos projetos antigos (links para o index.html de outro livro)
        if re.search(r"\]\((?:\.\./)+[\w-]+/(?:index\.html)?\)", fora):
            avisos.append(f"{c['id']}: ainda há link para outro livro")

    # ---- glossário
    gl = next((c for c in caps if c["id"] == "glossario"), None)
    if gl:
        entradas = len(re.findall(r"^- \*\*(.+?)\*\*(?: \((.+?)\))? — (.+)$", gl["md"], re.M))
        if entradas < 50:
            erros.append(f"glossário com poucas entradas legíveis: {entradas}")
        else:
            print(f"glossário: {entradas} termos")

    for a in avisos[:40]:
        print("AVISO:", a)
    if len(avisos) > 40:
        print(f"... e mais {len(avisos) - 40} aviso(s)")
    for e in erros[:60]:
        print("ERRO:", e)
    if len(erros) > 60:
        print(f"... e mais {len(erros) - 60} erro(s)")
    print(f"validação: {len(erros)} erro(s), {len(avisos)} aviso(s)")
    return 1 if erros else 0
