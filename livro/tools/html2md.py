"""Conversor HTML -> Markdown do leitor, por regras de classe. Usa só a biblioteca padrão.

Entende as classes dos sites de ensino dos projetos (cards de problema/objetivo, analogias, infográficos,
blocos de código com cabeçalho, tabelas didáticas, passos de tutorial, checklists e quizzes de botões).
O que é puramente interativo (simuladores, botões) é descartado.
"""
from __future__ import annotations

import re
from html.parser import HTMLParser
from typing import Callable

VAZIAS = {"br", "img", "hr", "input", "meta", "link", "source", "wbr"}
DESCARTAR = {"script", "style", "button", "svg", "noscript", "form", "select", "textarea", "canvas", "video", "audio"}
BLOCOS = {"div", "section", "article", "main", "header", "footer", "aside", "nav", "figure", "details", "summary",
          "ul", "ol", "li", "p", "pre", "table", "h1", "h2", "h3", "h4", "h5", "h6", "blockquote", "dl", "dt", "dd", "hr"}


class No:
    def __init__(self, tag: str, attrs: dict[str, str]):
        self.tag, self.attrs, self.filhos = tag, attrs, []  # filhos: No | str

    @property
    def classes(self) -> set[str]:
        return set((self.attrs.get("class") or "").split())

    def texto(self) -> str:
        return "".join(f if isinstance(f, str) else f.texto() for f in self.filhos)

    def achar(self, tag: str | None = None, classe: str | None = None) -> "No | None":
        for f in self.filhos:
            if isinstance(f, No):
                if (tag is None or f.tag == tag) and (classe is None or classe in f.classes):
                    return f
                achado = f.achar(tag, classe)
                if achado:
                    return achado
        return None

    def todos(self, tag: str | None = None, classe: str | None = None) -> list["No"]:
        saida: list[No] = []
        for f in self.filhos:
            if isinstance(f, No):
                if (tag is None or f.tag == tag) and (classe is None or classe in f.classes):
                    saida.append(f)
                saida += f.todos(tag, classe)
        return saida


class _Parser(HTMLParser):
    def __init__(self):
        super().__init__(convert_charrefs=True)
        self.raiz = No("raiz", {})
        self.pilha = [self.raiz]

    def handle_starttag(self, tag, attrs):
        no = No(tag, {k: (v or "") for k, v in attrs})
        self.pilha[-1].filhos.append(no)
        if tag not in VAZIAS:
            self.pilha.append(no)

    def handle_startendtag(self, tag, attrs):
        self.pilha[-1].filhos.append(No(tag, {k: (v or "") for k, v in attrs}))

    def handle_endtag(self, tag):
        for i in range(len(self.pilha) - 1, 0, -1):
            if self.pilha[i].tag == tag:
                del self.pilha[i:]
                return

    def handle_data(self, data):
        self.pilha[-1].filhos.append(data)


def parse(html: str) -> No:
    p = _Parser()
    p.feed(html)
    return p.raiz


_EMOJI = re.compile(r"^[^\w(\[«\"“]+", re.UNICODE)


def sem_emoji(t: str) -> str:
    return _EMOJI.sub("", re.sub(r"\s+", " ", t).strip()).strip()


class Conversor:
    """`resolver_img(src) -> caminho final dentro do livro (ex.: img/lab-pix/x.png)`; None descarta a imagem."""

    def __init__(self, resolver_img: Callable[[str], str | None] | None = None):
        self.resolver_img = resolver_img or (lambda s: s)
        self.avisos: list[str] = []

    # ------------------------------------------------------------------ inline
    def inline(self, no: No | str) -> str:
        if isinstance(no, str):
            return re.sub(r"\s+", " ", no)
        t = no.tag
        if t in DESCARTAR:
            return ""
        if t == "br":
            return "\n"
        interno = "".join(self.inline(f) for f in no.filhos)
        if t in ("strong", "b"):
            return f"**{interno.strip()}**" if interno.strip() else ""
        if t in ("em", "i"):
            return f"*{interno.strip()}*" if interno.strip() else ""
        if t == "code":
            return f"`{no.texto().strip()}`" if no.texto().strip() else ""
        if t == "small":
            return f" {interno.strip()}" if interno.strip() else ""   # evita "TítuloDescrição" colado
        if t == "a":
            href = no.attrs.get("href", "")
            return f"[{interno.strip()}]({href})" if re.match(r"^(https?://|#)", href) and interno.strip() else interno
        return interno

    def _paragrafos(self, no: No) -> list[str]:
        """Um <p> com <br> vira vários parágrafos (o leitor une quebras simples de linha)."""
        texto = "".join(self.inline(f) for f in no.filhos)
        partes = [re.sub(r"[ \t]+", " ", p).strip() for p in texto.split("\n")]
        return [p for p in partes if p]

    # ------------------------------------------------------------------ blocos
    def corpo(self, no: No) -> list[str]:
        """Converte os filhos de `no` em blocos Markdown (cada bloco é separado por linha em branco)."""
        saida: list[str] = []
        pendente: list[No | str] = []

        def descarrega():
            if pendente:
                texto = "".join(self.inline(f) for f in pendente)
                for p in (re.sub(r"[ \t]+", " ", x).strip() for x in texto.split("\n")):
                    if p:
                        saida.append(p)
                pendente.clear()

        for f in no.filhos:
            if isinstance(f, str):
                pendente.append(f)
            elif f.tag in DESCARTAR:
                continue
            elif f.tag in BLOCOS or f.tag == "img":
                descarrega()
                saida += self.bloco(f)
            else:
                pendente.append(f)
        descarrega()
        # descarta "parágrafos" que são só emoji/pontuação (ícones decorativos como 📊)
        return [b for b in saida if b.strip() and (re.search(r"[A-Za-zÀ-ÿ0-9]", b) or b.lstrip().startswith(("```", ":::", "|", "-", ">")))]

    def bloco(self, no: No) -> list[str]:
        t, c = no.tag, no.classes
        if t in DESCARTAR:
            return []
        if "mermaid" in c or (t == "pre" and (no.achar(classe="mermaid") or "mermaid" in (no.achar("code").classes if no.achar("code") else set()))):
            # diagramas Mermaid dependem das quebras de linha: não pode passar por inline()
            texto = re.sub(r"^\n+|\s+$", "", no.texto())
            linhas = texto.split("\n")
            recuo = min((len(l) - len(l.lstrip()) for l in linhas if l.strip()), default=0)
            return ["```mermaid\n" + "\n".join(l[recuo:] for l in linhas) + "\n```"]
        if t == "img":
            return self._imagem(no, "")
        if t in ("h1", "h2", "h3", "h4", "h5", "h6"):
            nivel = {"h1": 2, "h2": 2, "h3": 3}.get(t, 4)
            texto = " ".join(self._paragrafos(no))
            return [f"{'#' * nivel} {texto}"] if texto else []
        if t == "p":
            if "sub-alert" in c:
                return ["> " + p for p in self._paragrafos(no)]
            return self._paragrafos(no)
        if t in ("ul", "ol"):
            return [self._lista(no, 0)]
        if t == "pre":
            return [self._codigo(no, "")]
        if t == "table":
            return [self._tabela(no, "")]
        if t == "blockquote":
            return ["\n".join("> " + l for l in "\n\n".join(self.corpo(no)).split("\n"))]
        if t == "hr":
            return []
        if t == "details":
            resumo = no.achar("summary")
            titulo = sem_emoji(resumo.texto()) if resumo else "Detalhes"
            interno = No("x", {})
            interno.filhos = [f for f in no.filhos if f is not resumo]
            return [self._callout("nota", titulo, self.corpo(interno))]
        if t == "figure":
            img, leg = no.achar("img"), no.achar("figcaption")
            if img:
                return self._imagem(img, self._plano(leg) if leg else "")
        if t == "dl":
            return self.corpo(no)
        if t in ("dt", "dd", "summary"):
            return self._paragrafos(no)
        if t == "li":
            return self.corpo(no)
        return self._por_classe(no)

    def _por_classe(self, no: No) -> list[str]:
        c = no.classes

        if c & {"module-header", "module-number", "module-subtitle", "feedback-msg", "sim-badge", "nav-item", "btn-copy"}:
            return []
        if "infographic-card" in c or "infographic-frame" in c:
            img = no.achar("img")
            leg = no.achar(classe="infographic-caption")
            if img:
                # legenda curta (alt) na figura; o texto explicativo vira parágrafos/lista logo abaixo
                saida = self._imagem(img, img.attrs.get("alt", ""))
                if leg:
                    saida += [re.sub(r"^•\s*", "- ", b, flags=re.M) for b in self.corpo(leg)]
                return saida
            return []
        if "code-spotlight" in c:
            cab = no.achar(classe="code-header")
            titulo = sem_emoji(cab.texto()) if cab else ""
            pre = no.achar("pre")
            return [self._codigo(pre, titulo)] if pre else self.corpo(no)
        if "card-problem" in c:
            return [self._callout("cuidado", self._badge(no) or "O problema", self.corpo(self._sem_badge(no)))]
        if "card-objective" in c:
            return [self._callout("dica", self._badge(no) or "O objetivo", self.corpo(self._sem_badge(no)))]
        if "analogy-box" in c:
            cab = no.achar(classe="analogy-header")
            titulo = sem_emoji(cab.texto()) if cab else "Analogia"
            interno = No("x", {})
            interno.filhos = [f for f in no.filhos if f is not cab]
            return [self._callout("analogia", titulo, self.corpo(interno))]
        if "diagram" in c and no.todos(classe="diagram-node"):
            # diagrama feito de <div>s: vira uma descrição (aria-label) + lista ordenada dos nós
            itens = []
            for n in no.todos(classe="diagram-node"):
                pequeno = n.achar("small")
                principal = re.sub(r"\s+", " ", "".join(
                    f if isinstance(f, str) else ("" if f.tag == "small" else f.texto()) for f in n.filhos)).strip()
                if principal:
                    itens.append(f"- **{principal}**" + (f" — {self._plano(pequeno)}" if pequeno else ""))
            saida = []
            if no.attrs.get("aria-label"):
                saida.append(f"> **Diagrama.** {no.attrs['aria-label']}")
            return saida + (["\n".join(itens)] if itens else [])
        if "callout" in c:
            return [self._callout("nota", "Nota", self.corpo(no))]
        if "step-table-wrapper" in c:
            titulo = no.achar(classe="table-title")
            tabela = no.achar("table")
            if tabela:
                return [self._tabela(tabela, sem_emoji(titulo.texto()) if titulo else "")]
        if "checklist-card" in c:
            itens = [sem_emoji(l.texto()) for l in no.todos("label")]
            return ["\n".join(f"- [ ] {i}" for i in itens if i)] if itens else []
        if "tutorial-step-block" in c:
            return self._passo(no)
        if "simulator-card" in c or "quiz-container" in c:
            return self._quiz(no)
        if c & {"badge-tag", "step-badge", "step-header-tag", "step-num"}:
            t = sem_emoji(no.texto())
            return [f"**{t}**"] if t else []
        return self.corpo(no)

    # ------------------------------------------------------------------ peças
    def _plano(self, no: No | None) -> str:
        return re.sub(r"\s+", " ", no.texto()).strip() if no else ""

    def _badge(self, no: No) -> str:
        b = no.achar(classe="badge-tag")
        return sem_emoji(b.texto()) if b else ""

    def _sem_badge(self, no: No) -> No:
        b = no.achar(classe="badge-tag")
        novo = No(no.tag, no.attrs)
        novo.filhos = [f for f in no.filhos if f is not b]
        return novo

    def _callout(self, tipo: str, titulo: str, blocos: list[str]) -> str:
        corpo = "\n\n".join(blocos) or "_(sem texto)_"
        return f"::: {tipo} {titulo}\n{corpo}\n:::"

    def _imagem(self, img: No, legenda: str) -> list[str]:
        destino = self.resolver_img(img.attrs.get("src", ""))
        if not destino:
            self.avisos.append(f"imagem sem destino: {img.attrs.get('src', '')}")
            return []
        legenda = (legenda or img.attrs.get("alt", "")).replace("|", "/")
        legenda = re.sub(r"\s+", " ", legenda).strip()
        return [f"::: ilustracao {destino} | {legenda}\n:::" if legenda else f"::: ilustracao {destino}\n:::"]

    def _codigo(self, pre: No | None, titulo: str) -> str:
        if pre is None:
            return ""
        code = pre.achar("code")
        texto = (code or pre).texto().strip("\n")
        texto = re.sub(r"\n[ \t]+\n", "\n\n", texto)
        classe = " ".join(((code.attrs.get("class") if code else "") or "", pre.attrs.get("class", "")))
        lang = next((m.group(1) for m in re.finditer(r"language-([\w#+-]+)", classe)), "") or _adivinhar_linguagem(texto, titulo)
        cab = f"```{lang} {titulo}".rstrip()
        return f"{cab}\n{texto}\n```"

    def _lista(self, no: No, nivel: int) -> str:
        linhas = []
        for i, li in enumerate((f for f in no.filhos if isinstance(f, No) and f.tag == "li"), start=1):
            marca = f"{i}." if no.tag == "ol" else "-"
            pref = "  " * nivel
            texto_partes, subs = [], []
            for f in li.filhos:
                if isinstance(f, No) and f.tag in ("ul", "ol"):
                    subs.append(self._lista(f, nivel + 1))
                elif isinstance(f, No) and f.tag in ("p", "div"):
                    texto_partes.append(" ".join(self._paragrafos(f)))
                else:
                    texto_partes.append(self.inline(f))
            texto = re.sub(r"\s+", " ", " ".join(p for p in texto_partes if p)).strip()
            linhas.append(f"{pref}{marca} {texto}")
            linhas += subs
        return "\n".join(linhas)

    def _tabela(self, tabela: No, titulo: str) -> str:
        linhas = []
        for tr in tabela.todos("tr"):
            celulas = [re.sub(r"\s+", " ", " ".join(self._paragrafos(c)) or c.texto()).strip().replace("|", "\\|")
                       for c in tr.filhos if isinstance(c, No) and c.tag in ("td", "th")]
            if celulas:
                linhas.append(celulas)
        if not linhas:
            return ""
        n = max(len(l) for l in linhas)
        linhas = [l + [""] * (n - len(l)) for l in linhas]
        md = ["| " + " | ".join(linhas[0]) + " |", "|" + "|".join("---" for _ in range(n)) + "|"]
        md += ["| " + " | ".join(l) + " |" for l in linhas[1:]]
        return (f"**{titulo}**\n\n" if titulo else "") + "\n".join(md)

    def _passo(self, no: No) -> list[str]:
        tag = no.achar(classe="step-header-tag") or no.achar(classe="step-badge")
        titulo = no.achar("h3") or no.achar("h4")
        rotulo = sem_emoji(" · ".join(x for x in (tag.texto() if tag else "", titulo.texto() if titulo else "") if x))
        interno = No("x", {})
        interno.filhos = [f for f in no.filhos if f is not tag and f is not titulo]
        blocos = self.corpo(interno)
        return ([f"### {rotulo}"] if rotulo else []) + blocos

    def _quiz(self, no: No) -> list[str]:
        saida: list[str] = []
        for q in no.todos(classe="quiz-question-card"):
            texto = q.achar(classe="question-text")
            enun = re.sub(r"^\s*\d+[.)]\s*", "", self._plano(texto))
            linhas = [f"? {enun}"]
            for b in q.todos("button", "opt-btn"):
                opcao = re.sub(r"^\s*[A-Za-z][).]\s*", "", self._plano(b))
                linhas.append(("+ " if b.attrs.get("data-correct") == "true" else "- ") + opcao)
            if enun and len(linhas) > 2:
                saida.append("\n".join(linhas))
        return ["::: quiz\n" + "\n\n".join(saida) + "\n:::"] if saida else []


def _adivinhar_linguagem(texto: str, titulo: str = "") -> str:
    base = f"{titulo}\n{texto}"
    if re.search(r"\b(using |namespace |public (class|record|sealed)|async Task|var \w+ =|builder\.Services)", base):
        return "csharp"
    if re.search(r"^\s*(version:|services:|image:|FROM |WORKDIR )", texto, re.M) and ("services:" in texto or "FROM " in texto):
        return "yaml" if "services:" in texto else "dockerfile"
    if re.search(r"^\s*(dotnet|docker|curl|cd|npm|k6|git) ", texto, re.M):
        return "bash"
    if texto.lstrip().startswith(("{", "[")):
        return "json"
    if re.search(r"^\s*(SELECT|INSERT|CREATE|UPDATE)\b", texto, re.I | re.M):
        return "sql"
    if re.search(r"^\s*(import |export |const |function |export default)", texto, re.M):
        return "javascript"
    return ""


def html_para_md(html: str, resolver_img: Callable[[str], str | None] | None = None) -> tuple[str, list[str]]:
    """Converte um trecho de HTML e devolve (markdown, avisos)."""
    conv = Conversor(resolver_img)
    blocos = conv.corpo(parse(html))
    md = "\n\n".join(b.strip("\n") for b in blocos if b.strip())
    md = re.sub(r"\n{3,}", "\n\n", md)
    return md.strip() + "\n", conv.avisos
