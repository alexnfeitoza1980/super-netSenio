#!/usr/bin/env python3
"""Importa o conteúdo dos projetos de estudo para o livro único e grava `livro/content/*.md`.

Uso:
  git -C ../../livro-csharp-senior archive HEAD | tar -x -C <pasta-snapshot>   # snapshot do Git (inclui o que foi apagado do disco)
  python tools/importar.py --snapshot <pasta-snapshot> --origens <pasta-entrevistas>

É idempotente: cada capítulo vira `content/<id>.md` e é sobrescrito a cada execução. Nada é apagado;
arquivos que sobrarem de execuções antigas são apenas listados.
"""
from __future__ import annotations

import argparse
import re
import shutil
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).parent))

from comum import Capitulo, Resultado, rebaixar_titulos  # noqa: E402
import fontes_livros  # noqa: E402

# Ordem de leitura. Os livros C# 1–10 ficam contíguos porque sua numeração (cap. 1–133, lab. 1–14)
# já é contínua e é citada pelo glossário; o conteúdo novo continua a partir dali.
ORDEM_MACRO = ["C#", "C# & Mentalidade", "Arquitetura .NET", "Laboratório", "Angular"]


def carregar_fontes_opcionais(nome: str):
    try:
        return __import__(nome)
    except ModuleNotFoundError as e:
        if e.name == nome:
            return None
        raise


def coletar(snapshot: Path, origens: Path) -> Resultado:
    total = fontes_livros.importar(snapshot)
    for nome in ("fontes_arquitetura", "fontes_laboratorios"):
        mod = carregar_fontes_opcionais(nome)
        if mod:
            r = mod.importar(origens)
            total.capitulos += r.capitulos
            total.arquivos += r.arquivos
            total.avisos += r.avisos
            total.titulos.update(r.titulos)
    return total


def ordenar(caps: list[Capitulo]) -> list[Capitulo]:
    """Mantém a ordem de importação dentro de cada área e ordena as áreas conforme ORDEM_MACRO."""
    indice = {m: i for i, m in enumerate(ORDEM_MACRO)}
    return [c for _, c in sorted(enumerate(caps), key=lambda t: (indice.get(t[1].macro, 99), t[0]))]


def atribuir_numeracao(caps: list[Capitulo]) -> dict[str, dict[tuple[str, int], int]]:
    """C# preserva a numeração original; o restante continua a partir do maior número existente."""
    mapa: dict[str, dict[tuple[str, int], int]] = {}
    prox = {"capitulo": 0, "lab": 0}
    for c in caps:  # primeiro, o que já tem número fixo (livros C#)
        if c.macro.startswith("C#") and c.num_original and c.kind in prox:
            c.num = c.num_original
            prox[c.kind] = max(prox[c.kind], c.num)
    for c in caps:
        if c.kind not in prox or c.num is not None:
            continue
        prox[c.kind] += 1
        c.num = prox[c.kind]
        if c.num_original:
            mapa.setdefault(c.slug, {})[(c.kind, c.num_original)] = c.num
    return mapa


def numerar_partes(caps: list[Capitulo]) -> None:
    """Partes dos livros C# mantêm o número original; as demais continuam a sequência."""
    maior = max((c.parte_num or 0) for c in caps if c.macro.startswith("C#"))
    novos: dict[tuple[str, str, str], int] = {}
    for c in caps:
        # glossários e folhas de cola serão fundidos em um só depois: não podem consumir número de parte
        if c.kind in ("glossario", "extra") or c.sem_numero or (c.macro.startswith("C#") and c.parte_num):
            continue
        chave = (c.macro, c.slug, c.parte)  # cada livro/módulo tem as suas próprias partes
        if chave not in novos:
            maior += 1
            novos[chave] = maior
        c.parte_num = novos[chave]


def grupo_de(c: Capitulo) -> str:
    if c.sem_numero or c.parte_num is None:
        return c.macro if c.parte == c.macro else f"{c.macro} · {c.parte}"
    return f"{c.macro} · Parte {c.parte_num} · {c.parte}"


def remapear_refs(ref: str, slug: str, mapa) -> str:
    """Glossários de livros fora do C# citam 'cap. 3'/'Lab 1' na numeração antiga do livro."""
    def cap(m):
        return m.group(1) + str(mapa.get(slug, {}).get(("capitulo", int(m.group(2))), m.group(2)))

    def lab(m):
        return m.group(1) + str(mapa.get(slug, {}).get(("lab", int(m.group(2))), m.group(2)))

    ref = re.sub(r"(cap\.\s*)(\d+)", cap, ref)
    return re.sub(r"(Lab\s*)(\d+)", lab, ref)


def fundir_glossarios_e_folhas(caps: list[Capitulo], titulos: dict[str, str], mapa) -> list[Capitulo]:
    glossarios = [c for c in caps if c.kind == "glossario"]
    folhas = [c for c in caps if c.kind == "extra" and c.id.endswith("-folha-de-cola")]
    resto = [c for c in caps if c not in glossarios and c not in folhas]

    linhas, vistos = [], set()
    for g in glossarios:
        for linha in g.md.split("\n"):
            m = re.match(r"^- \*\*(.+?)\*\*(?: \((.+?)\))? — (.+)$", linha)
            if not m:
                continue
            if g.macro not in ("C#", "C# & Mentalidade") and m.group(2):
                linha = f"- **{m.group(1)}** ({remapear_refs(m.group(2), g.slug, mapa)}) — {m.group(3)}"
            if linha not in vistos:
                vistos.add(linha)
                linhas.append(linha)
    if linhas:
        resto.append(Capitulo(
            id="glossario", kind="glossario", title="Glossário geral", macro="Referência", parte="Referência",
            subtitle="Os termos de todos os livros — C#, arquitetura e Angular — em um só lugar.",
            minutes=15, hue=200, md="\n".join(linhas) + "\n", origem="glossários de todos os livros"))

    if folhas:
        partes = []
        for f in folhas:
            partes.append(f"## {titulos.get(f.slug, f.slug)}\n\n{rebaixar_titulos(f.md, 1)}")
        resto.append(Capitulo(
            id="folha-de-cola", kind="extra", title="Folhas de cola", macro="Referência", parte="Referência",
            subtitle="Resumos de consulta rápida de cada livro, reunidos.", minutes=20, hue=30,
            md="\n\n".join(partes) + "\n", origem="folhas de cola de todos os livros"))
    return resto


def sanear_svg(arquivo: Path) -> str:
    """Repara SVGs com XML inválido (o navegador não os desenha). Só mexe em arquivo que NÃO parseia.

    Devolve "ok" (já válido), "reparado" ou "falhou".
    """
    import xml.etree.ElementTree as ET

    def valido(texto: str) -> bool:
        try:
            ET.fromstring(texto.encode("utf-8"))
            return True
        except ET.ParseError:
            return False

    texto = arquivo.read_text(encoding="utf-8")
    if valido(texto):
        return "ok"
    # 1) `&` solto em texto ("Code & Angular") precisa ser `&amp;`.
    tentativa = re.sub(r"&(?!#?\w+;)", "&amp;", texto)
    # 2) atributo sem nome/aspas: `<text ... font: 16px system-ui>` -> `style="font: 16px system-ui"`.
    tentativa = re.sub(r"(<\w+[^<>]*?)\s(font:\s*[^<>\"=]+?)\s*>", r'\1 style="\2">', tentativa)
    if tentativa != texto and valido(tentativa):
        arquivo.write_text(tentativa, encoding="utf-8", newline="\n")
        return "reparado"
    return "falhou"


def emitir(c: Capitulo, ordem: int) -> str:
    fm = [f"id: {c.id}", f"order: {ordem}", f"kind: {c.kind}"]
    if c.num is not None:
        fm.append(f"num: {c.num}")
    fm += [f"group: {grupo_de(c)}", f"title: {c.title}"]
    if c.subtitle:
        fm.append(f"subtitle: {c.subtitle}")
    fm += [f"minutes: {c.minutes}", f"hue: {c.hue}"]
    if c.badge:
        fm.append(f"badge: {c.badge}")
    if c.image:
        fm.append(f"image: {c.image}")
    if c.objectives:
        fm.append("objectives:")
        fm += [f"- {o}" for o in c.objectives]
    fm += [f"{k}: {v}" for k, v in c.extras.items()]
    if c.origem:
        fm.append(f"origem: {c.origem}")
    return "---\n" + "\n".join(fm) + "\n---\n\n" + c.md.strip() + "\n"


def main() -> int:
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--snapshot", required=True, type=Path, help="pasta com o HEAD extraído do livro-csharp-senior")
    ap.add_argument("--origens", required=True, type=Path, help="pasta `entrevistas` (projetos de origem)")
    ap.add_argument("--saida", type=Path, default=Path(__file__).resolve().parent.parent, help="pasta `livro/`")
    args = ap.parse_args()

    res = coletar(args.snapshot, args.origens)
    caps = ordenar(res.capitulos)
    mapa = atribuir_numeracao(caps)
    numerar_partes(caps)
    caps = fundir_glossarios_e_folhas(caps, res.titulos, mapa)
    caps = ordenar(caps)

    # glossário e folha de cola ficam no fim, na área "Referência"
    fixos = [c for c in caps if c.macro == "Referência"]
    caps = [c for c in caps if c.macro != "Referência"] + fixos
    for c in fixos:
        c.sem_numero = True

    # a introdução geral (escrita à mão) fica fora da importação: content/00-introducao.md
    destino = args.saida / "content"
    destino.mkdir(parents=True, exist_ok=True)
    gravados = set()
    for i, c in enumerate(caps, start=10):  # 1–9 reservados para a introdução e o mapa do livro
        arq = destino / f"{c.id}.md"
        arq.write_text(emitir(c, i), encoding="utf-8", newline="\n")
        gravados.add(arq.name)

    copiados, reparados = 0, []
    for a in res.arquivos:
        alvo = args.saida / a.destino
        alvo.parent.mkdir(parents=True, exist_ok=True)
        shutil.copy2(a.origem, alvo)
        copiados += 1
        if alvo.suffix.lower() == ".svg":
            situacao = sanear_svg(alvo)
            if situacao == "reparado":
                reparados.append(a.destino)
            elif situacao == "falhou":
                res.avisos.append(f"SVG inválido e sem conserto automático: {a.destino}")

    manuais = {"00-introducao.md", "01-mapa-do-livro.md"}
    sobras = sorted(p.name for p in destino.glob("*.md") if p.name not in gravados and p.name not in manuais)
    print(f"{len(caps)} capítulos gravados em {destino}")
    print(f"{copiados} arquivos copiados (imagens, código, READMEs)")
    if reparados:
        print(f"{len(reparados)} SVG(s) com XML inválido na origem foram reparados (& solto, atributo sem aspas)")
    for a in res.avisos:
        print("AVISO:", a)
    if sobras:
        print(f"ATENÇÃO: {len(sobras)} arquivo(s) em content/ não vêm desta importação (apague à mão se forem obsoletos):")
        for s in sobras[:15]:
            print("  -", s)
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
