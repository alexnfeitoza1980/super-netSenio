#!/usr/bin/env python3
"""Constrói o livro único a partir de `content/*.md` + `livro.json`.

Gera, na pasta do livro: index.html, assets/book-data.js e assets/tema.css.
O resultado funciona abrindo index.html direto no navegador (sem servidor).
Depois de gerar, roda as checagens de integridade (tools/validar.py); use --sem-validar para pular.
"""
import colorsys
import json
import re
import sys
from pathlib import Path
from urllib.parse import quote

raiz = Path(__file__).parent
motor = raiz / "motor"


def ler_capitulos(pasta: Path):
    capitulos = []
    for f in sorted((pasta / "content").glob("*.md")):
        raw = f.read_text(encoding="utf-8").replace("\r\n", "\n")
        m = re.match(r"^---\n(.*?)\n---\n(.*)$", raw, re.S)
        if not m:
            sys.exit(f"{f.name}: faltou o frontmatter")
        meta = {"objectives": []}
        chave = None
        for linha in m.group(1).splitlines():
            if linha.startswith("- ") and chave == "objectives":
                meta["objectives"].append(linha[2:].strip())
                continue
            k, _, v = linha.partition(":")
            chave = k.strip()
            if chave != "objectives":
                meta[chave] = v.strip()
        meta["num"] = int(meta["num"]) if meta.get("num", "").isdigit() else None
        meta["minutes"] = int(meta.get("minutes", "20"))
        meta["md"] = m.group(2).strip()
        meta["file"] = f.name
        meta.pop("origem", None)  # rastreabilidade fica nos arquivos .md; não precisa ir para o navegador
        capitulos.append(meta)
    capitulos.sort(key=lambda c: int(c.get("order", "9999")))
    return capitulos


def hex_hsl(h):
    r, g, b = (int(h[i:i + 2], 16) / 255 for i in (1, 3, 5))
    hh, l, s = colorsys.rgb_to_hls(r, g, b)
    return hh * 360, s, l


def hsl_hex(h, s, l):
    r, g, b = colorsys.hls_to_rgb((h % 360) / 360, max(0, min(1, l)), max(0, min(1, s)))
    return "#%02x%02x%02x" % (round(r * 255), round(g * 255), round(b * 255))


def rgba(h, a):
    return "rgba(%d,%d,%d,%s)" % (int(h[1:3], 16), int(h[3:5], 16), int(h[5:7], 16), a)


def tema_css(l):
    p = l["paleta"]
    mh = l["matiz"]
    ph, ps, pl = hex_hsl(p["primary"])
    ah, as_, al = hex_hsl(p["accent"])
    ch, cs, cl = hex_hsl(p["codigo"])
    c0, c1, c2 = p["capa"]
    escuro = {
        "bg": f"hsl({mh} 26% 8%)", "surface": f"hsl({mh} 22% 11%)", "surface-2": f"hsl({mh} 20% 14%)",
        "ink": f"hsl({mh} 20% 93%)", "ink-2": f"hsl({mh} 14% 76%)", "ink-3": f"hsl({mh} 10% 58%)",
        "line": f"hsl({mh} 18% 21%)",
        "primary": hsl_hex(ph, max(ps, .7), .68), "primary-ink": hsl_hex(ph, .4, .1), "primary-soft": hsl_hex(ph, .4, .22),
        "accent": hsl_hex(ah, as_, min(.72, al + .08)),
        "code-bg": hsl_hex(ch, cs, max(.04, cl - .05)), "code-bar": hsl_hex(ch, cs, min(.2, cl + .05)),
    }
    bloco_escuro = ";".join(f"--{k}:{v}" for k, v in escuro.items()) + ";--shadow:0 1px 2px rgba(0,0,0,.4),0 8px 24px rgba(0,0,0,.35)"
    return f"""/* Tema do livro «{l['titulo']}» (gerado por build.py a partir de livro.json) */
:root {{
  --bg: {p['bg']}; --surface: #ffffff; --surface-2: {p['surface2']};
  --ink: {p['ink']}; --ink-2: {p['ink2']}; --ink-3: {p['ink3']}; --line: {p['line']};
  --primary: {p['primary']}; --primary-ink: #ffffff; --primary-soft: {p['soft']}; --accent: {p['accent']};
  --code-bg: {p['codigo']}; --code-bar: {hsl_hex(ch, cs, min(.22, cl + .06))};
  --font-display: {l['display']};
}}
:root[data-theme="dark"] {{ {bloco_escuro} }}
@media (prefers-color-scheme: dark) {{ :root:not([data-theme="light"]) {{ {bloco_escuro} }} }}

body {{ background-color: var(--bg); background-image: radial-gradient(900px 320px at 88% -90px, {rgba(p['accent'], .12)}, transparent 70%); background-repeat: no-repeat; }}
h1, h2, h3, .sec-title, .hero h1, .cover h1 {{ font-family: var(--font-display); }}

.cover {{ background: radial-gradient(1100px 380px at 90% -10%, {rgba(p['accent'], .38)}, transparent 60%), linear-gradient(135deg, {c0}, {c1} 58%, {c2}); }}
.cover::after {{ content: "{l['marca']}"; }}
.cover .btn.light {{ color: {c0}; }}
.cover h1 {{ max-width: 22ch; }}
.brand-mark {{ background: linear-gradient(135deg, {c1}, {c2}); color: #fff; }}
.hero {{ background: linear-gradient(135deg, hsl(var(--hue) 58% 36%), hsl(calc(var(--hue) + 14) 62% 24%)); }}
.progress-bar {{ background: linear-gradient(90deg, {p['primary']}, {p['accent']}); }}
"""


def favicon(l):
    c0, c1, c2 = l["paleta"]["capa"]
    svg = (f"<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 64 64'><defs><linearGradient id='g' x1='0' y1='0' x2='1' y2='1'>"
           f"<stop offset='0' stop-color='{c1}'/><stop offset='1' stop-color='{c2}'/></linearGradient></defs>"
           f"<rect width='64' height='64' rx='14' fill='url(#g)'/><text x='32' y='42' font-family='Arial' font-weight='700' "
           f"font-size='26' fill='white' text-anchor='middle'>{l['marca']}</text></svg>")
    return "data:image/svg+xml," + quote(svg, safe="/:=,' ()")


def contar(md, padrao):
    return len(re.findall(padrao, md, re.M))


def gerar_mapa(caps):
    """Tabela Markdown com todas as partes (grupos), na ordem do livro."""
    grupos = {}
    for c in caps:
        g = grupos.setdefault(c["group"], {"itens": [], "primeiro": c})
        g["itens"].append(c)
    linhas = ["| Área | Parte | Seções | Começa em |", "|---|---|---:|---|"]
    for nome, g in grupos.items():
        area, _, resto = nome.partition(" · ")
        if not resto:
            area, resto = "—", nome
        p = g["primeiro"]
        rotulo = p["title"].replace("|", "/")
        linhas.append(f"| {area} | {resto.replace('|', '/')} | {len(g['itens'])} | [{rotulo}](#/c/{p['id']}) |")
    return "\n".join(linhas)


def aplicar_marcadores(caps):
    n = lambda k: sum(1 for c in caps if c["kind"] == k)
    perguntas = sum(contar(c["md"], r"^\? ") for c in caps)
    cartoes = sum(contar(c["md"], r"^.+ :: .+$") for c in caps)
    palavras = sum(len(c["md"].split()) for c in caps)
    horas = round(sum(c["minutes"] for c in caps) / 60)
    estat = (f"> **Em números:** {n('capitulo')} capítulos, {n('lab')} laboratórios, {perguntas:,} perguntas de quiz, "
             f"{cartoes:,} cartões de revisão e cerca de {palavras // 1000} mil palavras (~{horas} horas de estudo).").replace(",", ".")
    for c in caps:
        c["md"] = c["md"].replace("<!--MAPA-->", gerar_mapa(caps)).replace("<!--ESTATISTICAS-->", estat)


def construir(validar=True):
    l = json.loads((raiz / "livro.json").read_text(encoding="utf-8"))
    caps = ler_capitulos(raiz)
    aplicar_marcadores(caps)
    (raiz / "assets").mkdir(exist_ok=True)

    meta = {k: l[k] for k in ("id", "titulo", "legenda", "kicker", "tituloHome", "blurb", "matiz", "marca")}
    meta["tituloCurto"] = l["titulo"]
    (raiz / "assets" / "book-data.js").write_text(
        "window.BOOK_META = " + json.dumps(meta, ensure_ascii=False) + ";\n"
        "window.BOOK_DATA = " + json.dumps(caps, ensure_ascii=False) + ";\n", encoding="utf-8")
    (raiz / "assets" / "tema.css").write_text(tema_css(l), encoding="utf-8")

    modelo = (motor / "modelo-livro.html").read_text(encoding="utf-8")
    descricao = (l["resumo"] + " Livro interativo com exercícios, quizzes e cartões de revisão.").replace('"', "&quot;")
    html = (modelo.replace("{{TITULO}}", f"{l['titulo']} — {l['legenda']}").replace("{{DESCRICAO}}", descricao)
            .replace("{{COR}}", l["paleta"]["primary"]).replace("{{FAVICON}}", favicon(l))
            .replace("{{MARCA}}", l["marca"]).replace("{{NOME}}", l["titulo"]).replace("{{LEGENDA}}", l["legenda"]))
    (raiz / "index.html").write_text(html, encoding="utf-8")

    palavras = sum(len(c["md"].split()) for c in caps)
    n = lambda k: sum(1 for c in caps if c["kind"] == k)
    print(f"{len(caps)} seções ({n('capitulo')} capítulos, {n('lab')} laboratórios, {n('intro')} apresentações, "
          f"{n('apendice')} apêndices, {n('extra')} guias, {n('glossario')} glossário) · ~{palavras:,} palavras".replace(",", "."))
    print(f"perguntas de quiz: {sum(contar(c['md'], r'^\? ') for c in caps)} · cartões: {sum(contar(c['md'], r'^.+ :: .+$') for c in caps)}")
    print("Livro -> index.html")

    if validar:
        sys.path.insert(0, str(raiz / "tools"))
        import validar as v
        return v.validar(raiz, caps)
    return 0


if __name__ == "__main__":
    sys.exit(construir(validar="--sem-validar" not in sys.argv))
