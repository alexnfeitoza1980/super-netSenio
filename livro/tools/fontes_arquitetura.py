"""Importa o guia `Arquitetura-microservicos.NET` (21 módulos em Markdown estilo Obsidian) para o dialeto do leitor."""
from __future__ import annotations

import re
from pathlib import Path, PurePosixPath

from comum import (Capitulo, Imagem, Resultado, frase_inicial, limpar_titulo, minutos_de_leitura,
                   normalizar_quebras, separar_frontmatter)

MACRO = "Arquitetura .NET"
PASTA = "Arquitetura-microservicos.NET"

# Callouts do Obsidian (`> [!TIPO]`) -> diretivas do leitor.
CALLOUT = {"NOTE": "nota", "INFO": "nota", "TIP": "dica", "HINT": "dica", "IMPORTANT": "regra",
           "WARNING": "cuidado", "CAUTION": "cuidado", "DANGER": "cuidado", "QUESTION": "pense"}
ROTULO = {"nota": "Nota", "dica": "Dica", "regra": "Importante", "cuidado": "Cuidado", "pense": "Pense"}

_IGNORAR = {"bin", "obj", "node_modules", ".git", ".vs"}


def _converter_callouts(md: str) -> str:
    """`> [!CAUTION] Título` + linhas `> ...`  ->  `::: cuidado Título` ... `:::`."""
    linhas, saida, i = md.split("\n"), [], 0
    dentro_codigo = False
    while i < len(linhas):
        l = linhas[i]
        if l.lstrip().startswith("```"):
            dentro_codigo = not dentro_codigo
        m = None if dentro_codigo else re.match(r"^>\s*\[!(\w+)\][+-]?\s*(.*)$", l)
        if m and m.group(1).upper() in CALLOUT:
            tipo = CALLOUT[m.group(1).upper()]
            titulo = m.group(2).strip() or ROTULO[tipo]
            corpo, i = [], i + 1
            while i < len(linhas) and linhas[i].startswith(">"):
                corpo.append(re.sub(r"^>\s?", "", linhas[i]))
                i += 1
            saida += [f"::: {tipo} {titulo}", *corpo, ":::"]
            continue
        saida.append(l)
        i += 1
    return "\n".join(saida)


def _limpar_obsidian(md: str) -> str:
    md = re.sub(r"\[\[([^\]|]+)\|([^\]]+)\]\]", r"\2", md)   # [[alvo|texto]] -> texto
    md = re.sub(r"\[\[([^\]]+)\]\]", r"\1", md)              # [[alvo]] -> alvo
    md = re.sub(r"^%%.*?%%$", "", md, flags=re.M)            # comentários do Obsidian
    return md


def _normalizar_titulos(md: str) -> str:
    """O leitor entende `##`–`####`. `#` extra vira `##`; níveis 5–6 viram `####`. Ignora blocos de código."""
    saida, dentro = [], False
    for l in md.split("\n"):
        if l.startswith("```"):
            dentro = not dentro
        elif not dentro:
            m = re.match(r"^(#{1,6})(\s+.*)$", l)
            if m:
                n = len(m.group(1))
                l = "#" * (2 if n == 1 else 4 if n > 4 else n) + m.group(2)
        saida.append(l)
    return "\n".join(saida)


def importar(origens: Path) -> Resultado:
    res = Resultado()
    base = origens / PASTA
    if not base.is_dir():
        res.avisos.append(f"{PASTA} não encontrado em {origens}")
        return res

    modulos = sorted(p for p in base.iterdir() if p.is_dir() and re.match(r"^\d\d-", p.name))
    # 1ª passada: mapa arquivo -> id, para reescrever os links entre aulas.
    ids: dict[str, str] = {}
    for mod in modulos:
        num = mod.name[:2]
        for f in sorted(mod.glob("*.md")):
            ids[f"{mod.name}/{f.name}"] = f"arq-{num}-intro" if f.name.lower() == "readme.md" else f"arq-{num}-{f.stem}"

    def reescrever_links(md: str, mod: Path) -> str:
        def troca(m: re.Match) -> str:
            alvo, ancora = m.group(2), m.group(3) or ""
            resolvido = PurePosixPath(mod.name, alvo)
            partes: list[str] = []
            for p in resolvido.parts:  # resolve ../
                if p == "..":
                    if partes:
                        partes.pop()
                else:
                    partes.append(p)
            chave = "/".join(partes)
            if chave in ids:
                return f"[{m.group(1)}](#/c/{ids[chave]})"
            res.avisos.append(f"{mod.name}: link para arquivo desconhecido: {alvo}")
            return m.group(1)  # mantém só o texto, sem link quebrado
        return re.sub(r"\[([^\]]+)\]\(([^)#\s]+\.md)(#[^)]*)?\)", troca, md)

    for idx, mod in enumerate(modulos):
        num = mod.name[:2]
        hue = (200 + 11 * idx) % 360
        titulo_modulo = mod.name[3:].replace("-", " ").title()
        arquivos = sorted(mod.glob("*.md"), key=lambda p: (p.name.lower() != "readme.md", p.name))
        # título do módulo: H1 do README ("🧱 Capítulo 1: Fundamentos...") sem emoji e sem "Capítulo N:"
        readme = mod / "README.md"
        if readme.is_file():
            h1 = re.search(r"^#\s+(.+)$", normalizar_quebras(readme.read_text(encoding="utf-8")), re.M)
            if h1:
                titulo_modulo = re.sub(r"^(?:cap[ií]tulo|m[oó]dulo)\s*\d+\s*[:\-—]\s*", "",
                                       limpar_titulo(h1.group(1)), flags=re.I).strip()
        res.titulos[f"arq-{num}"] = titulo_modulo

        for f in arquivos:
            texto = normalizar_quebras(f.read_text(encoding="utf-8"))
            lido = separar_frontmatter(texto)
            meta, corpo = (lido if lido else ({}, texto.strip()))
            h1 = re.match(r"^#\s+(.+?)\s*\n", corpo + "\n")
            titulo_h1 = limpar_titulo(h1.group(1)) if h1 else ""
            if h1:
                corpo = corpo[h1.end():].lstrip("\n")
            ehreadme = f.name.lower() == "readme.md"
            titulo = limpar_titulo(meta.get("title", "").strip('"\'')) or titulo_h1 or f.stem
            if ehreadme:
                titulo = f"{titulo_modulo} — visão geral"

            corpo = _converter_callouts(_limpar_obsidian(corpo))
            corpo = _normalizar_titulos(corpo)
            corpo = reescrever_links(corpo, mod)
            res.capitulos.append(Capitulo(
                id=ids[f"{mod.name}/{f.name}"], kind="intro" if ehreadme else "capitulo", title=titulo, md=corpo.strip(),
                macro=MACRO, parte=titulo_modulo, subtitle=frase_inicial(corpo),
                minutes=minutos_de_leitura(corpo), hue=hue, origem=f"{PASTA}/{mod.name}/{f.name}", slug=f"arq-{num}",
            ))

    # Código de apoio do guia (esqueletos Clean Architecture/DDD) e infraestrutura de exemplo.
    for sub in ("Stock.Management", "Notification.API"):
        raiz = base / sub
        if raiz.is_dir():
            for f in sorted(raiz.rglob("*")):
                if f.is_file() and not (set(f.relative_to(raiz).parts) & _IGNORAR):
                    res.arquivos.append(Imagem(f, f"codigo/arquitetura/{sub}/{f.relative_to(raiz).as_posix()}"))
    for nome in ("docker-compose.yml", "prometheus.yml"):
        if (base / nome).is_file():
            res.arquivos.append(Imagem(base / nome, f"codigo/arquitetura/{nome}"))
    return res
