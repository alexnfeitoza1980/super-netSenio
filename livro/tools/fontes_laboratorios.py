"""Aulas que viviam em sites, scripts e documentos dos projetos práticos, convertidas em capítulos do livro.

Fontes:
  microservico-senior/site-ensino        HTML   -> Microsserviços na prática (Gateway, Saga, Circuit Breaker, Tracing)
  aula-guiada-monolito-gemini/site       JS     -> Banco Digital: Outbox, Inbox e Pix
  aula_guiada_mcroservicos_net_ia/portal-anfx/dist/lessons.js  -> Banco ANFX: Clean Architecture na prática
  Anfsusax-solid-principles (PrincipiosSolid.cs)               -> SOLID com analogias bíblicas
  bankmore-microservices/SETUP-COMPLETO.md                     -> BankMore
  tools/conteudo/*.md  e  super-netSenior/entrevista/README.md -> super-netSenior
"""
from __future__ import annotations

import json
import re
import subprocess
from pathlib import Path

from comum import (Capitulo, Imagem, Resultado, frase_inicial, limpar_titulo, minutos_de_leitura,
                   normalizar_quebras, separar_frontmatter)
from html2md import Conversor, html_para_md, parse, sem_emoji

MACRO = "Laboratório"
AQUI = Path(__file__).resolve().parent


def _node(arquivo: Path, expressao: str, res: Resultado):
    try:
        r = subprocess.run(["node", str(AQUI / "extrair_js.js"), str(arquivo), expressao],
                           capture_output=True, encoding="utf-8", timeout=30)
    except (FileNotFoundError, subprocess.TimeoutExpired) as e:
        res.avisos.append(f"node indisponível ({e}); aulas de {arquivo.name} não importadas")
        return None
    if r.returncode != 0:
        res.avisos.append(f"falha ao ler {arquivo.name}: {r.stderr.strip()[:160]}")
        return None
    return json.loads(r.stdout)


def _resolver(base: Path, destino: str, res: Resultado):
    """Cria um resolvedor de imagens: copia só o que foi realmente referenciado."""
    vistos: set[str] = set()

    def resolver(src: str) -> str | None:
        if not src or re.match(r"^(https?:|data:)", src):
            return None
        arq = (base / src).resolve()
        if not arq.is_file():
            res.avisos.append(f"imagem não encontrada: {src} (em {base.name})")
            return None
        rel = f"img/{destino}/{arq.name}"
        if rel not in vistos:
            vistos.add(rel)
            res.arquivos.append(Imagem(arq, rel))
        return rel
    return resolver


def _lab(res: Resultado, *, id: str, slug: str, parte: str, titulo: str, md: str, hue: int, subtitle: str = "",
         kind: str = "lab", objectives: list[str] | None = None, minutes: int | None = None, origem: str = "") -> None:
    res.capitulos.append(Capitulo(
        id=id, kind=kind, title=titulo, md=md.strip(), macro=MACRO, parte=parte, subtitle=subtitle or frase_inicial(md),
        minutes=minutes or minutos_de_leitura(md), hue=hue, objectives=objectives or [], origem=origem, slug=slug))


# ---------------------------------------------------------------------------- microservico-senior/site-ensino
def _site_ensino(origens: Path, res: Resultado) -> None:
    base = origens / "microservico-senior" / "site-ensino"
    arq = base / "index.html"
    if not arq.is_file():
        res.avisos.append("site-ensino não encontrado")
        return
    parte, slug = "Microsserviços na prática: Gateway, Saga, Circuit Breaker e Tracing", "lab-micro"
    resolver = _resolver(base, "lab-micro", res)
    raiz = parse(normalizar_quebras(arq.read_text(encoding="utf-8")))
    for i, sec in enumerate(s for s in raiz.todos("section") if s.attrs.get("id")):
        sid = sec.attrs["id"]
        h2 = sec.achar("h2")
        titulo = limpar_titulo(" ".join((h2.texto() if h2 else sid).split()))
        sub = sec.achar(classe="module-subtitle")
        conv = Conversor(resolver)
        corpo = "\n\n".join(b for b in conv.corpo(sec) if b.strip())
        res.avisos += conv.avisos
        if sid == "intro":
            kind, titulo = "intro", "Microsserviços na prática — apresentação"
        elif sid == "simuladores":
            titulo = "Teste rápido: perguntas de entrevista sênior"
            corpo += ("\n\n> Os simuladores interativos (disjuntor, saga e carga) continuam no projeto de origem: "
                      "`microservico-senior/site-ensino`.")
            kind = "lab"
        else:
            kind = "lab"
        if not corpo.strip():
            res.avisos.append(f"site-ensino/{sid}: seção vazia após conversão")
            continue
        _lab(res, id=f"lab-micro-{sid.replace('modulo-', '')}", slug=slug, parte=parte, titulo=titulo,
             md=re.sub(r"\n{3,}", "\n\n", corpo), hue=(160 + 9 * i) % 360, kind=kind,
             subtitle=sem_emoji(sub.texto()) if sub else "", origem="microservico-senior/site-ensino/index.html")


# ---------------------------------------------------------------------------- Banco Digital: Pix
def _pix(origens: Path, res: Resultado) -> None:
    base = origens / "aula-guiada-monolito-gemini" / "site"
    js = base / "assets" / "js" / "pix-lessons.js"
    if not js.is_file():
        res.avisos.append("pix-lessons.js não encontrado")
        return
    dados = _node(js, "pixLessons", res)
    if not dados:
        return
    resolver = _resolver(base, "lab-pix", res)
    parte, slug = "Banco Digital: Outbox, Inbox e Pix", "lab-pix"
    for i, (chave, aula) in enumerate(dados.items()):
        conv = Conversor(resolver)
        corpo = "\n\n".join(b for b in conv.corpo(parse(aula["content"])) if b.strip())
        res.avisos += conv.avisos
        _lab(res, id=f"lab-pix-{chave}", slug=slug, parte=parte, titulo=aula["title"], md=corpo,
             subtitle=aula.get("desc", ""), hue=(20 + 14 * i) % 360,
             origem="aula-guiada-monolito-gemini/site/assets/js/pix-lessons.js")


# ---------------------------------------------------------------------------- Banco ANFX
def _anfx(origens: Path, res: Resultado) -> None:
    js = origens / "aula_guiada_mcroservicos_net_ia" / "portal-anfx" / "dist" / "lessons.js"
    if not js.is_file():
        res.avisos.append("lessons.js do portal ANFX não encontrado")
        return
    dados = _node(js, "(window.lessons||window.ANFX_LESSONS)", res)
    if not dados:
        return
    aulas = dados if isinstance(dados, list) else list(dados.values())
    parte, slug = "Banco ANFX: Clean Architecture na prática", "lab-anfx"
    for i, a in enumerate(aulas):
        if a.get("kind") == "planned":
            continue  # aulas "em planejamento": placeholders sem conteúdo
        md, avisos = html_para_md(a.get("body", ""))
        res.avisos += avisos
        partes = []
        if a.get("intro"):
            partes.append(a["intro"].strip())
        partes.append(md)
        if a.get("files"):
            partes.append("## Arquivos para abrir\n\n" + "\n".join(f"- **{rotulo}** — `{caminho}`" for rotulo, caminho in a["files"]))
        q = a.get("quiz")
        if q:
            opcoes = "\n".join(("+ " if n == q["correct"] else "- ") + o for n, o in enumerate(q["options"]))
            expl = f"\n> {q['explanation']}" if q.get("explanation") else ""
            partes.append(f"::: quiz\n? {q['question']}\n{opcoes}{expl}\n:::")
        minutos = re.match(r"(\d+)", a.get("duration", ""))
        titulo = a["title"]
        _lab(res, id=f"lab-anfx-{a['id']}", slug=slug, parte=parte, titulo=titulo, md="\n\n".join(partes),
             hue=(280 + 12 * i) % 360, minutes=int(minutos.group(1)) if minutos else None,
             origem="aula_guiada_mcroservicos_net_ia/portal-anfx/dist/lessons.js")


# ---------------------------------------------------------------------------- SOLID com analogias bíblicas
def extrair_literais(texto: str) -> list[str]:
    """Lê, em ordem, os literais string de um trecho C# (normais e raw `\"\"\"`), sem depender de regex para casar aspas."""
    saida, i, n = [], 0, len(texto)
    while i < n:
        if texto.startswith('"""', i):
            j = texto.index('"""', i + 3)
            corpo = texto[i + 3:j]
            corpo = re.sub(r"^[ \t]*\n", "", corpo)      # linha logo após a abertura
            corpo = re.sub(r"\n[ \t]*$", "", corpo)      # indentação antes do fechamento
            saida.append(corpo)
            i = j + 3
        elif texto[i] == '"':
            j = i + 1
            while j < n and texto[j] != '"':
                j += 2 if texto[j] == "\\" else 1
            saida.append(bytes(texto[i + 1:j], "utf-8").decode("unicode_escape", "ignore") if "\\" in texto[i + 1:j] else texto[i + 1:j])
            i = j + 1
        elif texto.startswith("//", i):
            i = texto.find("\n", i) if texto.find("\n", i) != -1 else n
        else:
            i += 1
    return saida


def _solid(origens: Path, res: Resultado) -> None:
    arq = origens / "Anfsusax-solid-principles/backend/BiblicalArchitecture.Domain/Entidades/Solid/PrincipiosSolid.cs"
    if not arq.is_file():
        res.avisos.append("PrincipiosSolid.cs não encontrado")
        return
    t = normalizar_quebras(arq.read_text(encoding="utf-8"))
    parte, slug = "SOLID com analogias bíblicas", "lab-solid"
    inicios = [m for m in re.finditer(r"class (\w+) : PrincipioSolid", t)]
    for k, m in enumerate(inicios):
        fim = inicios[k + 1].start() if k + 1 < len(inicios) else len(t)
        corpo = t[m.start():fim]
        # só os argumentos do construtor base(...): vão até o ");" que fecha a chamada
        lits = extrair_literais(corpo)
        blocos = corpo.count("new BlocoCodigoResponsabilidade(")
        esperado = 8 + 4 * blocos  # nome, sigla, descrição, código, referência, versículo, aplicação (+1: veja abaixo)
        base_n = 7
        if len(lits) < base_n:
            res.avisos.append(f"SOLID {m.group(1)}: literais insuficientes ({len(lits)})")
            continue
        nome, sigla, descricao, exemplo, referencia, versiculo, aplicacao = lits[:7]
        restante = lits[7:]
        md = [f"> {descricao}", "", "## A analogia bíblica", "",
              f"**{referencia}**", "", f"> “{versiculo}”", "", aplicacao, "",
              "## O exemplo completo", "", f"```csharp {sigla}\n{exemplo}\n```"]
        if blocos and len(restante) >= 4:
            md += ["", "## Peça por peça"]
            for b in range(len(restante) // 4):
                titulo_b, tipo_b, codigo_b, expl_b = restante[4 * b:4 * b + 4]
                md += ["", f"### {titulo_b}", "", expl_b, "", f"```csharp {tipo_b}\n{codigo_b}\n```"]
        elif blocos:
            res.avisos.append(f"SOLID {sigla}: {blocos} bloco(s) declarados mas só {len(restante)} literais após o cabeçalho")
        _lab(res, id=f"lab-solid-{sigla.lower()}", slug=slug, parte=parte, titulo=nome, md="\n".join(md),
             subtitle=descricao, hue=(40 + 30 * k) % 360, origem="Anfsusax-solid-principles/.../PrincipiosSolid.cs")


# ---------------------------------------------------------------------------- BankMore
def _bankmore(origens: Path, res: Resultado) -> None:
    arq = origens / "bankmore-microservices" / "SETUP-COMPLETO.md"
    if not arq.is_file():
        return
    md = normalizar_quebras(arq.read_text(encoding="utf-8"))
    md = re.sub(r"^#\s+.*\n+", "", md, count=1)
    md = re.sub(r"^(#{3,6})(\s)", lambda m: "#" * min(4, len(m.group(1))) + m.group(2), md, flags=re.M)
    md = re.sub(r"^##\s+", "## ", md, flags=re.M)
    _lab(res, id="lab-bankmore-setup", slug="lab-bankmore", parte="BankMore: o banco em microsserviços",
         titulo="BankMore: como subir e testar a API", md=md, hue=340,
         subtitle="Configuração do Entity Framework, injeções de dependência e como testar a API de contas.",
         origem="bankmore-microservices/SETUP-COMPLETO.md")


# ---------------------------------------------------------------------------- super-netSenior
def _super_netsenior(origens: Path, res: Resultado) -> None:
    parte, slug = "super-netSenior · da repetição à solução única", "lab-sns"
    pasta = AQUI / "conteudo"
    for f in sorted(pasta.glob("sns-*.md")):
        lido = separar_frontmatter(f.read_text(encoding="utf-8"))
        if not lido:
            res.avisos.append(f"{f.name}: sem frontmatter")
            continue
        meta, corpo = lido
        _lab(res, id=meta["id"], slug=slug, parte=meta.get("parte", parte), titulo=meta["title"], md=corpo,
             subtitle=meta.get("subtitle", ""), hue=int(meta.get("hue", "200")), minutes=int(meta.get("minutes", "25")),
             objectives=meta["objectives"], origem=f"livro/tools/conteudo/{f.name}")
    # Roteiro de cenários de entrevista escrito junto da solução (somente leitura).
    cenarios = origens / "super-netSenior" / "entrevista" / "README.md"
    if cenarios.is_file():
        md = normalizar_quebras(cenarios.read_text(encoding="utf-8"))
        md = re.sub(r"^#\s+.*\n+", "", md, count=1)
        _lab(res, id="lab-sns-entrevista", slug=slug, parte=parte, titulo="Cenários de entrevista com a API",
             md=md, hue=215, subtitle="Exercícios práticos contra a API do Banking, com a evidência esperada para cada cenário.",
             origem="super-netSenior/entrevista/README.md")


def importar(origens: Path) -> Resultado:
    res = Resultado()
    for etapa in (_site_ensino, _pix, _anfx, _solid, _bankmore, _super_netsenior):
        etapa(origens, res)
    return res
