"""Tipos e utilitários compartilhados pelos importadores do livro único."""
from __future__ import annotations

import re
import unicodedata
from dataclasses import dataclass, field
from pathlib import Path


@dataclass
class Capitulo:
    """Um capítulo já no dialeto do leitor (Markdown com diretivas `:::`)."""

    id: str
    kind: str                      # intro | capitulo | lab | apendice | extra | glossario
    title: str
    md: str
    macro: str                     # rótulo da área no sumário (ex.: "C#", "Arquitetura .NET")
    parte: str                     # título da parte (sem o "Parte N ·")
    parte_num: int | None = None   # número original da parte, se houver (C# preserva)
    sem_numero: bool = False       # parte que nunca teve número (ex.: "Oficina", "Referência"): não ganha "Parte N"
    subtitle: str = ""
    minutes: int = 20
    hue: int = 262
    objectives: list[str] = field(default_factory=list)
    num_original: int | None = None  # numeração original (livros C# preservam)
    badge: str = ""
    image: str = ""
    extras: dict[str, str] = field(default_factory=dict)
    origem: str = ""               # de onde veio (para rastreabilidade)
    slug: str = ""                 # livro/módulo de origem
    num: int | None = None         # número final, atribuído na montagem


@dataclass
class Imagem:
    """Arquivo a copiar para dentro do livro único."""

    origem: Path
    destino: str  # relativo à pasta do livro, ex.: "img/aprenda/cap01.svg"


@dataclass
class Resultado:
    capitulos: list[Capitulo] = field(default_factory=list)
    arquivos: list[Imagem] = field(default_factory=list)
    avisos: list[str] = field(default_factory=list)
    titulos: dict[str, str] = field(default_factory=dict)  # slug -> título do livro/módulo de origem


def normalizar_quebras(texto: str) -> str:
    return texto.replace("\r\n", "\n").replace("\r", "\n")


def separar_frontmatter(texto: str) -> tuple[dict, str] | None:
    """Lê o frontmatter simples dos capítulos (chave: valor e listas `objectives`)."""
    m = re.match(r"^---\n(.*?)\n---\n(.*)$", normalizar_quebras(texto), re.S)
    if not m:
        return None
    meta: dict = {"objectives": []}
    chave = None
    for linha in m.group(1).splitlines():
        if linha.startswith("- ") and chave == "objectives":
            meta["objectives"].append(linha[2:].strip())
            continue
        k, _, v = linha.partition(":")
        chave = k.strip()
        if chave != "objectives":
            meta[chave] = v.strip()
    return meta, m.group(2).strip()


def slugificar(texto: str) -> str:
    t = unicodedata.normalize("NFD", texto).encode("ascii", "ignore").decode()
    return re.sub(r"[^a-z0-9]+", "-", t.lower()).strip("-") or "secao"


_EMOJI_INICIO = re.compile(r"^[\W_]+", re.UNICODE)


def limpar_titulo(titulo: str) -> str:
    """Remove emoji/pontuação inicial de títulos vindos de Markdown ("🛡️ Resiliência" -> "Resiliência")."""
    t = _EMOJI_INICIO.sub("", titulo.strip())
    return t.strip() or titulo.strip()


def rebaixar_titulos(md: str, niveis: int = 1) -> str:
    """Aumenta o nível dos títulos (## -> ###) ignorando o que está dentro de blocos de código."""
    saida, dentro = [], False
    for linha in md.split("\n"):
        if linha.lstrip().startswith("```"):
            dentro = not dentro
        elif not dentro:
            m = re.match(r"^(#{1,5})(\s.*)$", linha)
            if m:
                linha = "#" * min(6, len(m.group(1)) + niveis) + m.group(2)
        saida.append(linha)
    return "\n".join(saida)


def frase_inicial(md: str, limite: int = 150) -> str:
    """Primeira frase útil de um texto (para virar subtítulo)."""
    for bloco in re.split(r"\n\s*\n", md):
        b = bloco.strip()
        if not b or b.startswith(("#", "```", "|", "---", ":::", "<", "!", "- ", "1.")):
            continue
        b = re.sub(r"^>\s*", "", b, flags=re.M)
        b = re.sub(r"\*\*|__|`|\[([^\]]+)\]\([^)]*\)", lambda m: m.group(1) or "", b)
        b = re.sub(r"\s+", " ", b).strip().strip('"“”')
        if len(b) < 20:
            continue
        corte = re.search(r"[.!?](\s|$)", b)
        fim = corte.end() if corte else len(b)
        frase = b[:fim].strip()
        return frase if len(frase) <= limite else frase[: limite - 1].rstrip() + "…"
    return ""


def minutos_de_leitura(md: str, palavras_por_minuto: int = 180, minimo: int = 5) -> int:
    return max(minimo, round(len(md.split()) / palavras_por_minuto))
