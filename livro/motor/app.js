/* Motor dos livros da coleção Aprenda C#. Sem dependências: funciona abrindo index.html direto. */
(function () {
  'use strict';
  const DATA = (window.BOOK_DATA || []).map((c, i) => Object.assign({ idx: i }, c));
  const META = window.BOOK_META || {};
  const COL = window.COLECAO || [];
  const NOME = META.tituloCurto || META.titulo || 'Livro';
  const K = 'lcs.' + (META.id || 'livro') + '.';
  const $ = (s, r = document) => r.querySelector(s);
  const $$ = (s, r = document) => Array.from(r.querySelectorAll(s));
  const main = $('#main'), rail = $('#rail'), sidebar = $('#sidebar');

  /* ---------- armazenamento (à prova de localStorage bloqueado) ---------- */
  const mem = {};
  const store = {
    get(k, d) { try { const v = localStorage.getItem(k); return v == null ? d : JSON.parse(v); } catch (e) { return k in mem ? mem[k] : d; } },
    set(k, v) { mem[k] = v; try { localStorage.setItem(k, JSON.stringify(v)); } catch (e) { /* ok */ } }
  };
  /* progresso por livro; migra o progresso do livro único antigo (só os capítulos que existem aqui) */
  function migrarProgresso() {
    const velho = store.get('sns.progress', null); if (!velho) return {};
    const ids = new Set(DATA.map(c => c.id)), f = o => Object.fromEntries(Object.entries(o || {}).filter(([k]) => ids.has(k)));
    return { done: f(velho.done), quiz: f(velho.quiz), last: ids.has(velho.last) ? velho.last : null };
  }
  const progress = Object.assign({ done: {}, quiz: {}, last: null }, store.get(K + 'progress', null) || migrarProgresso());
  const saveProgress = () => store.set(K + 'progress', progress);

  /* ---------- utilidades ---------- */
  const esc = s => String(s).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
  const rotuloDaSecao = c => c && c.num ? (c.kind === 'lab' ? 'Lab ' : 'Cap. ') + c.num + ' · ' : '';
  const slug = s => s.toLowerCase().normalize('NFD').replace(/[̀-ͯ]/g, '').replace(/[^a-z0-9]+/g, '-').replace(/^-|-$/g, '') || 'secao';
  const shuffle = a => { a = a.slice(); for (let i = a.length - 1; i > 0; i--) { const j = Math.floor(Math.random() * (i + 1)); [a[i], a[j]] = [a[j], a[i]]; } return a; };
  const byId = id => DATA.find(c => c.id === id);
  /* a cor de cada capítulo fica numa faixa de 60° em torno da cor do livro */
  const hueOf = c => {
    const raw = (c.hue != null && c.hue !== '') ? +c.hue : (c.order * 37 + 250) % 360;
    return META.matiz != null ? (META.matiz + (((raw % 60) + 60) % 60) - 30 + 360) % 360 : raw;
  };
  function toast(msg) {
    let t = $('.toast'); if (!t) { t = document.createElement('div'); t.className = 'toast'; document.body.appendChild(t); }
    t.textContent = msg; t.classList.add('show'); clearTimeout(t._h); t._h = setTimeout(() => t.classList.remove('show'), 1800);
  }

  /* marcador de seta global para os diagramas SVG */
  document.body.insertAdjacentHTML('beforeend', '<svg width="0" height="0" style="position:absolute" aria-hidden="true"><defs>' +
    '<marker id="arr" viewBox="0 0 10 10" refX="8.5" refY="5" markerWidth="7" markerHeight="7" orient="auto-start-reverse"><path d="M0 0L10 5L0 10z" style="fill:var(--primary)"/></marker>' +
    '<marker id="arr2" viewBox="0 0 10 10" refX="8.5" refY="5" markerWidth="7" markerHeight="7" orient="auto-start-reverse"><path d="M0 0L10 5L0 10z" style="fill:var(--ink-2)"/></marker>' +
    '<marker id="arr3" viewBox="0 0 10 10" refX="8.5" refY="5" markerWidth="7" markerHeight="7" orient="auto-start-reverse"><path d="M0 0L10 5L0 10z" style="fill:var(--bad)"/></marker>' +
    '</defs></svg>');

  /* =====================================================================
     REALCE DE SINTAXE
     ===================================================================== */
  const CS_KW = new Set(('abstract as base bool break byte case catch char checked class const continue decimal default delegate do double else enum event explicit extern false finally fixed float for foreach goto if implicit in int interface internal is lock long namespace new null object operator out override params private protected public readonly ref return sbyte sealed short sizeof stackalloc static string struct switch this throw true try typeof uint ulong unchecked unsafe ushort using virtual void volatile while var get set value partial yield').split(' '));
  const LINQ_KW = new Set('from where select orderby join into equals ascending descending group by on let'.split(' '));
  const SQL_KW = new Set('select from where insert into values update set delete create table primary key not null identity int varchar nvarchar datetime bit order by group join inner left on and or as distinct top count sum avg min max'.split(' '));

  function wrapLines(cls, text) {
    const safe = esc(text);
    if (!cls) return safe;
    return safe.split('\n').map(p => p ? '<span class="tk-' + cls + '">' + p + '</span>' : p).join('\n');
  }
  function hlCS(src) {
    const hasLinq = /\bfrom\s+\w+\s+in\b/.test(src);
    const re = /(\/\/[^\n]*|\/\*[\s\S]*?\*\/)|(\$?@"(?:[^"]|"")*"|\$?"(?:\\.|[^"\\\n])*"|'(?:\\.|[^'\\\n])+')|(0x[0-9a-fA-F]+|\b\d[\d_]*(?:\.\d+)?(?:[fFdDmMlLuU]{1,2})?\b)|([A-Za-z_][A-Za-z0-9_]*)|(\[[A-Z][A-Za-z]*(?:\([^\]\n]*\))?\])|([\s\S])/g;
    let out = '', m, prev = '';
    while ((m = re.exec(src))) {
      if (m[1]) out += wrapLines('c', m[1]);
      else if (m[2]) out += wrapLines('s', m[2]);
      else if (m[3]) out += wrapLines('n', m[3]);
      else if (m[4]) {
        const w = m[4]; const rest = src.slice(re.lastIndex);
        let cls = '';
        if (CS_KW.has(w) || (hasLinq && LINQ_KW.has(w))) cls = 'k';
        else if (/^\s*\(/.test(rest) && prev !== 'new') cls = 'm';
        else if (/^[A-Z]/.test(w) && prev !== '.') cls = 't';
        else if (/^[A-Z]/.test(w) && prev === '.' && /^\s*[.<\w]/.test(rest) && /^\s*[.<]/.test(rest)) cls = 't';
        else if (prev === 'new') cls = 't';
        out += wrapLines(cls, w); prev = w;
        continue;
      }
      else if (m[5]) out += wrapLines('a', m[5]);
      else out += esc(m[6]);
      if (m[6] && m[6].trim()) prev = m[6];
    }
    return out;
  }
  function hlSQL(src) {
    return esc(src).replace(/(--[^\n]*)|('(?:[^']|'')*')|\b(\d+)\b|\b([A-Za-z_]+)\b/g, (x, c, s, n, w) =>
      c ? '<span class="tk-c">' + c + '</span>' : s ? '<span class="tk-s">' + s + '</span>' : n ? '<span class="tk-n">' + n + '</span>' :
        SQL_KW.has(w.toLowerCase()) ? '<span class="tk-k">' + w + '</span>' : x);
  }
  function hlXML(src) {
    return esc(src).replace(/(&lt;!--[\s\S]*?--&gt;)|(&quot;[^&]*?&quot;)|(&lt;\/?)([\w:.-]+)|([\w:.-]+)(?==)|(\/?&gt;)/g, (x, c, s, o, tag, at, cl) =>
      c ? '<span class="tk-c">' + c + '</span>' : s ? '<span class="tk-s">' + s + '</span>' : o ? '<span class="tk-p">' + o + '</span><span class="tk-t">' + tag + '</span>' :
        at ? '<span class="tk-m">' + at + '</span>' : '<span class="tk-p">' + cl + '</span>');
  }
  function highlight(code, lang, hl) {
    let h;
    if (lang === 'cs' || lang === 'csharp' || lang === 'c#') h = hlCS(code);
    else if (lang === 'sql') h = hlSQL(code);
    else if (lang === 'xml' || lang === 'html') h = hlXML(code);
    else h = esc(code);
    if (hl && hl.size) h = h.split('\n').map((l, i) => hl.has(i + 1) ? '<span class="ln-hl">' + (l || ' ') + '</span>' : l).join('\n');
    return h;
  }
  function parseRange(s) {
    const set = new Set(); if (!s) return set;
    s.split(',').forEach(p => { const [a, b] = p.trim().split('-').map(Number); for (let i = a; i <= (b || a); i++) set.add(i); });
    return set;
  }

  /* =====================================================================
     MARKDOWN (mini) + componentes
     ===================================================================== */
  function inline(s) {
    const codes = [];
    s = s.replace(/`([^`]+)`/g, (m, c) => { codes.push(c); return '\u0000' + (codes.length - 1) + '\u0000'; });
    s = esc(s)
      .replace(/&lt;br&gt;/g, '<br>')
      .replace(/\[\[(.+?)\]\]/g, '<kbd>$1</kbd>')
      .replace(/\*\*(.+?)\*\*/g, '<strong>$1</strong>')
      .replace(/(^|[^*\w])\*(?![\s*])(.+?)(?<![\s*])\*(?!\*)/g, '$1<em>$2</em>')
      .replace(/==(.+?)==/g, '<mark>$1</mark>')
      .replace(/\[([^\]]+)\]\(([^)\s]+)\)/g, (m, t, u) => '<a href="' + u + '"' + (/^https?:/.test(u) ? ' target="_blank" rel="noopener"' : '') + '>' + t + '</a>');
    return s.replace(/\u0000(\d+)\u0000/g, (m, i) => '<code>' + esc(codes[+i]) + '</code>');
  }

  const CALLOUTS = {
    dica: ['💡', 'Dica'], cuidado: ['⚠️', 'Cuidado'], erro: ['🐞', 'Erro comum'], pense: ['🧠', 'Pense nisso'],
    analogia: ['🔍', 'Analogia'], regra: ['📌', 'Regra de ouro'], curiosidade: ['✨', 'Curiosidade'],
    ide: ['🖱️', 'No Visual Studio'], vila: ['🏘️', 'Na Vila Objeto'], resumo: ['📝', 'Resumo do capítulo']
  };

  function blocks(lines, ctx) {
    const out = []; let i = 0;
    const isBlockStart = l => /^(```|:::|#{2,4}\s|>\s?|\s*([-*]|\d+\.)\s+|\|)/.test(l) || /^---+\s*$/.test(l);
    while (i < lines.length) {
      const line = lines[i];
      if (!line.trim()) { i++; continue; }

      let m = line.match(/^```([\w+#-]*)(?:\{([\d,\- ]+)\})?\s*(.*)$/);
      if (m) {
        const buf = []; i++;
        while (i < lines.length && !/^```\s*$/.test(lines[i])) buf.push(lines[i++]);
        i++;
        out.push(codeBlock(m[1].toLowerCase(), m[3], buf.join('\n'), m[2]));
        continue;
      }
      m = line.match(/^:::\s*([\w-]+)\s*(.*)$/);
      if (m) {
        let depth = 1, j = i + 1;
        for (; j < lines.length; j++) {
          if (/^:::\s*[\w-]+/.test(lines[j])) depth++;
          else if (/^:::\s*$/.test(lines[j])) { depth--; if (!depth) break; }
        }
        out.push(container(m[1], m[2].trim(), lines.slice(i + 1, j), ctx));
        i = j + 1; continue;
      }
      m = line.match(/^(#{2,4})\s+(.*)$/);
      if (m) {
        const level = m[1].length, text = m[2].trim();
        let id = slug(text); const seen = ctx.ids || (ctx.ids = {});
        seen[id] = (seen[id] || 0) + 1; if (seen[id] > 1) id += '-' + seen[id];
        if (level === 2) ctx.headings.push({ id, text });
        out.push('<h' + level + ' id="' + id + '">' + inline(text) + '</h' + level + '>');
        i++; continue;
      }
      if (/^---+\s*$/.test(line)) { out.push('<hr>'); i++; continue; }
      if (/^\|/.test(line) && i + 1 < lines.length && /^\|?\s*:?-{2,}/.test(lines[i + 1])) {
        const cells = r => r.trim().replace(/\\\|/g, '\u0001').replace(/^\||\|$/g, '').split('|').map(c => c.trim().replace(/\u0001/g, '|'));
        const head = cells(line); i += 2; const rows = [];
        while (i < lines.length && /^\|/.test(lines[i])) rows.push(cells(lines[i++]));
        out.push('<table><thead><tr>' + head.map(h => '<th>' + inline(h) + '</th>').join('') + '</tr></thead><tbody>' +
          rows.map(r => '<tr>' + r.map(c => '<td>' + inline(c) + '</td>').join('') + '</tr>').join('') + '</tbody></table>');
        continue;
      }
      if (/^>\s?/.test(line)) {
        const buf = []; while (i < lines.length && /^>\s?/.test(lines[i])) buf.push(lines[i++].replace(/^>\s?/, ''));
        out.push('<blockquote>' + blocks(buf, ctx) + '</blockquote>'); continue;
      }
      if (/^\s*([-*]|\d+\.)\s+/.test(line)) {
        const buf = [];
        while (i < lines.length) {
          const l = lines[i];
          if (/^\s*([-*]|\d+\.)\s+/.test(l) || (/^\s+\S/.test(l) && buf.length)) buf.push(l);
          else if (!l.trim() && i + 1 < lines.length && (/^\s*([-*]|\d+\.)\s+/.test(lines[i + 1]) || /^\s{2,}\S/.test(lines[i + 1])) && buf.length) { /* linha em branco dentro da lista */ }
          else break;
          i++;
        }
        out.push(listHtml(buf)); continue;
      }
      const buf = [line]; i++;
      while (i < lines.length && lines[i].trim() && !isBlockStart(lines[i])) buf.push(lines[i++]);
      out.push('<p>' + inline(buf.join(' ').replace(/\\\s*$/, '<br>')) + '</p>');
    }
    return out.join('\n');
  }

  function listHtml(lines) {
    const items = [];
    lines.forEach(l => {
      const m = l.match(/^(\s*)([-*]|\d+\.)\s+(.*)$/);
      if (m) items.push({ ind: m[1].length, ord: /\d/.test(m[2]), text: m[3], kids: [] });
      else if (l.trim() && items.length) items[items.length - 1].text += ' ' + l.trim();
    });
    function build(from, to, ind) {
      let html = '', k = from; const ord = items[from].ord;
      while (k < to) {
        let e = k + 1; while (e < to && items[e].ind > ind) e++;
        html += '<li>' + inline(items[k].text) + (e > k + 1 ? build(k + 1, e, items[k + 1].ind) : '') + '</li>';
        k = e;
      }
      return '<' + (ord ? 'ol' : 'ul') + '>' + html + '</' + (ord ? 'ol' : 'ul') + '>';
    }
    return build(0, items.length, items[0].ind);
  }

  function codeBlock(lang, title, code, hlSpec) {
    if (lang === 'mermaid') return '<figure class="fig mmd"><pre class="mermaid">' + esc(code) + '</pre></figure>';
    if (lang === 'saida') return '<div class="out">' + esc(code) + '</div>';
    const label = lang === 'cs' || lang === 'csharp' || lang === 'c#' ? 'C#' : (lang || 'texto');
    return '<pre class="code"><div class="bar"><span class="dots"><i></i><i></i><i></i></span>' + (title ? '<span>' + esc(title) + '</span>' : '') +
      '<span class="lang">' + esc(label) + '</span><button class="copy" type="button">Copiar</button></div><code>' +
      highlight(code, lang, parseRange(hlSpec)) + '</code></pre>';
  }

  function container(kind, rest, inner, ctx) {
    if (CALLOUTS[kind]) {
      const [ico, label] = CALLOUTS[kind];
      return '<aside class="callout ' + (kind === 'resumo' ? 'regra' : kind) + '"><div class="callout-title"><span class="ico">' + ico + '</span>' + inline(rest || label) + '</div>' + blocks(inner, ctx) + '</aside>';
    }
    if (kind === 'html') return inner.join('\n');
    if (kind === 'figura') return '<figure class="fig">' + inner.join('\n') + (rest ? '<figcaption>' + inline(rest) + '</figcaption>' : '') + '</figure>';
    if (kind === 'ilustracao') {
      /* ::: ilustracao img/arquivo.svg | legenda   (imagem-metáfora do capítulo; clique para ampliar) */
      const [src, ...leg] = (rest || '').split(' | '); const legenda = leg.join(' | ');
      return '<figure class="ilu"><button type="button" class="ilu-zoom" aria-label="Ampliar a ilustração"><img src="' + esc(src.trim()) + '" alt="' + esc(legenda || 'Ilustração do capítulo') + '" loading="lazy"></button>' +
        (legenda ? '<figcaption>' + inline(legenda) + '</figcaption>' : '') + '</figure>';
    }
    if (kind === 'exercicio') {
      const k = inner.findIndex(l => /^---\s*resposta/i.test(l));
      const q = k < 0 ? inner : inner.slice(0, k), a = k < 0 ? [] : inner.slice(k + 1);
      return '<div class="exercise"><div class="ex-title">✏️ ' + inline(rest || 'Exercício') + '</div>' + blocks(q, ctx) +
        (a.length ? '<details><summary>Ver resposta</summary>' + blocks(a, ctx) + '</details>' : '') + '</div>';
    }
    if (kind === 'cartas') {
      const cards = inner.filter(l => l.includes(' :: ')).map(l => l.split(' :: '));
      return '<div class="deck">' + cards.map(([f, b]) => cardHtml(f, b)).join('') + '</div>';
    }
    if (kind === 'passos') {
      const steps = []; let cur = [];
      inner.forEach(l => { if (/^===\s*$/.test(l)) { steps.push(cur); cur = []; } else cur.push(l); });
      steps.push(cur);
      return '<div class="steps"><div class="steps-head">▶ ' + inline(rest || 'Passo a passo') + '<span class="count"></span></div>' +
        steps.map((s, i) => '<div class="step' + (i ? '' : ' on') + '">' + blocks(s, ctx) + '</div>').join('') +
        '<div class="steps-nav"><button class="btn prev" type="button">← Anterior</button><div class="dots">' + steps.map((_, i) => '<i' + (i ? '' : ' class="on"') + '></i>').join('') + '</div><button class="btn primary next" type="button">Próximo →</button></div></div>';
    }
    if (kind === 'quiz') return quizHtml(inner, ctx);
    return '<div class="unknown">[componente desconhecido: ' + esc(kind) + ']</div>';
  }

  function cardHtml(f, b) {
    return '<button class="card" type="button"><div class="card-in"><div class="face front">' + inline(f.trim()) + '<small>clique para virar</small></div><div class="face back">' + inline(b.trim()) + '</div></div></button>';
  }

  function quizHtml(inner, ctx) {
    const qs = []; let q = null;
    inner.forEach(l => {
      if (/^\?\s/.test(l)) { q = { text: l.slice(2), opts: [], why: [] }; qs.push(q); }
      else if (q && /^[-+]\s/.test(l)) q.opts.push({ t: l.slice(2), ok: l[0] === '+' });
      else if (q && /^>\s?/.test(l)) q.why.push(l.replace(/^>\s?/, ''));
    });
    ctx.quiz = (ctx.quiz || 0) + qs.length;
    return '<section class="quiz" data-total="' + qs.length + '"><div class="quiz-head">🎯 Teste seus conhecimentos<span class="score">0 / ' + qs.length + '</span></div>' +
      qs.map((x, n) => '<div class="q"><p class="q-text"><span class="n">' + (n + 1) + '</span>' + inline(x.text) + '</p>' +
        shuffle(x.opts).map(o => '<button class="opt" type="button" data-ok="' + (o.ok ? 1 : 0) + '">' + inline(o.t) + '</button>').join('') +
        '<div class="why">' + inline(x.why.join(' ')) + '</div></div>').join('') +
      '<div class="quiz-foot"><strong class="final"></strong><button class="btn retry" type="button">↺ Refazer</button></div></section>';
  }

  /* =====================================================================
     Interatividade (hydrate)
     ===================================================================== */
  /* Mermaid é carregado sob demanda (só se o capítulo tiver diagrama). Offline ou bloqueado: o texto do diagrama continua legível. */
  const MERMAID_URL = 'https://cdn.jsdelivr.net/npm/mermaid@11.4.0/dist/mermaid.min.js';
  function renderMermaid(root) {
    const nodes = $$('pre.mermaid:not([data-processed])', root);
    if (!nodes.length) return;
    const run = () => {
      try {
        const dark = document.documentElement.getAttribute('data-theme') === 'dark' || (!document.documentElement.getAttribute('data-theme') && matchMedia('(prefers-color-scheme: dark)').matches);
        window.mermaid.initialize({ startOnLoad: false, securityLevel: 'strict', theme: dark ? 'dark' : 'default' });
        Promise.resolve(window.mermaid.run({ nodes })).catch(() => { /* mantém o texto */ });
      } catch (e) { /* mantém o texto */ }
    };
    if (window.mermaid) return run();
    if (renderMermaid.pendente) { renderMermaid.pendente.push(run); return; }
    renderMermaid.pendente = [run];
    const s = document.createElement('script');
    s.src = MERMAID_URL; s.async = true;
    s.onload = () => { const fila = renderMermaid.pendente; renderMermaid.pendente = null; fila.forEach(f => f()); };
    s.onerror = () => { renderMermaid.pendente = null; };
    document.head.appendChild(s);
  }

  function hydrate(root, chapter) {
    renderMermaid(root);
    $$('pre.code .copy', root).forEach(b => b.addEventListener('click', () => {
      const text = b.closest('pre').querySelector('code').innerText;
      const done = () => { b.textContent = 'Copiado!'; setTimeout(() => (b.textContent = 'Copiar'), 1400); };
      if (navigator.clipboard) navigator.clipboard.writeText(text).then(done, done); else done();
    }));
    $$('.card', root).forEach(c => c.addEventListener('click', () => c.classList.toggle('flip')));
    $$('.steps', root).forEach(st => {
      const steps = $$('.step', st), dots = $$('.dots i', st), count = $('.count', st); let n = 0;
      const show = k => {
        n = Math.max(0, Math.min(steps.length - 1, k));
        steps.forEach((s, i) => s.classList.toggle('on', i === n)); dots.forEach((d, i) => d.classList.toggle('on', i === n));
        count.textContent = (n + 1) + ' / ' + steps.length;
        $('.prev', st).disabled = n === 0; $('.next', st).disabled = n === steps.length - 1;
      };
      $('.prev', st).onclick = () => show(n - 1); $('.next', st).onclick = () => show(n + 1);
      dots.forEach((d, i) => d.onclick = () => show(i)); show(0);
    });
    $$('.quiz', root).forEach(qz => {
      const qsEls = $$('.q', qz), total = qsEls.length; let right = 0, answered = 0;
      const score = $('.score', qz);
      const old = chapter && progress.quiz[chapter.id];
      if (old) score.textContent = 'melhor: ' + old.best + ' / ' + old.total;
      qsEls.forEach(q => $$('.opt', q).forEach(o => o.addEventListener('click', () => {
        if (q.classList.contains('answered')) return;
        q.classList.add('answered'); answered++;
        const ok = o.dataset.ok === '1'; if (ok) right++;
        $$('.opt', q).forEach(x => { x.disabled = true; if (x.dataset.ok === '1') x.classList.add('right'); else if (x === o) x.classList.add('wrong'); else x.classList.add('dim'); });
        if (!ok) o.classList.add('wrong');
        score.textContent = right + ' / ' + total;
        if (answered === total) finish();
      })));
      function finish() {
        qz.classList.add('finished');
        const pct = Math.round(right / total * 100);
        $('.final', qz).textContent = pct >= 80 ? '🏆 Excelente! ' + right + ' de ' + total + ' (' + pct + '%)' : pct >= 50 ? '👍 Bom! ' + right + ' de ' + total + ' — releia os pontos que errou.' : '📖 ' + right + ' de ' + total + ' — releia o capítulo e tente de novo.';
        if (chapter) {
          const prev = progress.quiz[chapter.id];
          if (!prev || right >= prev.best) progress.quiz[chapter.id] = { best: right, total };
          if (pct >= 70) progress.done[chapter.id] = true;
          saveProgress(); renderSidebar(); syncDoneRow();
        }
      }
      $('.retry', qz).onclick = () => {
        right = answered = 0; qz.classList.remove('finished'); score.textContent = '0 / ' + total;
        qsEls.forEach(q => { q.classList.remove('answered'); $$('.opt', q).forEach(x => { x.disabled = false; x.className = 'opt'; }); });
        qz.scrollIntoView({ behavior: 'smooth', block: 'start' });
      };
    });
  }

  /* =====================================================================
     Views
     ===================================================================== */
  const KIND_LABEL = { intro: 'Antes de começar', capitulo: 'Capítulo', lab: 'Laboratório', apendice: 'Apêndice', extra: 'Guia rápido', glossario: 'Referência' };
  const trackable = c => c.kind === 'capitulo' || c.kind === 'lab';
  const countQuestions = md => (md.match(/^\? /gm) || []).length;
  const countCards = md => (md.match(/^.+ :: .+$/gm) || []).length;

  function renderSidebar(current) {
    const cur = current !== undefined ? current : sidebar.dataset.cur;
    sidebar.dataset.cur = cur || '';
    const tr = DATA.filter(trackable), done = tr.filter(c => progress.done[c.id]).length, pct = tr.length ? Math.round(done / tr.length * 100) : 0;
    let html = '<div class="side-head"><div class="ring" style="--p:' + pct + '"><span>' + pct + '%</span></div><div><b>Seu progresso</b><small>' + done + ' de ' + tr.length + ' concluídos</small></div></div>';
    if (COL.length) html += '<a class="side-link side-col" href="../../csharp.html"><span class="side-num" style="--hue:' + (META.matiz || 262) + '">📚</span><span class="side-title">Coleção Aprenda C#</span></a>';
    html += '<a class="side-link" href="#/" ' + (cur === 'home' ? 'aria-current="page"' : '') + '><span class="side-num" style="--hue:' + (META.matiz || 262) + '">🏠</span><span class="side-title">Início</span></a>';
    let lastGroup = null;
    DATA.filter(c => c.kind !== 'glossario' && c.kind !== 'extra').forEach(c => {
      if (c.group !== lastGroup) { html += '<div class="side-group">' + esc(c.group || '') + '</div>'; lastGroup = c.group; }
      html += link(c);
    });
    html += '<div class="side-group">Estudo e consulta</div>';
    DATA.filter(c => c.kind === 'extra').forEach(c => html += link(c));
    html += '<a class="side-link" href="#/cartas" ' + (cur === 'cartas' ? 'aria-current="page"' : '') + '><span class="side-num" style="--hue:30">🃏</span><span class="side-title">Cartões de revisão</span></a>';
    DATA.filter(c => c.kind === 'glossario').forEach(c => html += link(c));
    sidebar.innerHTML = html;
    function link(c) {
      return '<a class="side-link' + (progress.done[c.id] ? ' done' : '') + '" href="#/c/' + c.id + '" ' + (cur === c.id ? 'aria-current="page"' : '') + '><span class="side-num" style="--hue:' + hueOf(c) + '">' + esc(c.badge || c.num || '•') + '</span><span class="side-title">' + esc(c.title) + '</span>' +
        (trackable(c) ? '<span class="side-done"><svg viewBox="0 0 24 24" width="12" height="12" fill="none" stroke="currentColor" stroke-width="3.5" stroke-linecap="round" stroke-linejoin="round"><path d="M5 12l5 5 9-10"/></svg></span>' : '') + '</a>';
    }
    const active = $('[aria-current="page"]', sidebar);
    if (active && !sidebar._scrolled) { active.scrollIntoView({ block: 'center' }); sidebar._scrolled = true; }
  }

  function viewHome() {
    renderSidebar('home'); rail.innerHTML = '';
    const tr = DATA.filter(trackable);
    const hours = Math.round(DATA.reduce((a, c) => a + (c.minutes || 0), 0) / 60);
    const qs = DATA.reduce((a, c) => a + countQuestions(c.md), 0), cards = DATA.reduce((a, c) => a + countCards(c.md), 0);
    const last = progress.last && byId(progress.last);
    const first = DATA.find(c => c.kind === 'intro') || DATA[0];
    const temFolha = !!byId('folha-de-cola');
    let html = '<div class="home"><section class="cover"><span class="kicker">' + esc(META.kicker || 'Livro interativo de programação') + '</span><h1>' + esc(META.tituloHome || NOME) + '</h1>' +
      (META.blurb || '') +
      '<div class="btns"><a class="btn light" href="#/c/' + (last ? last.id : first.id) + '">' + (last ? '▶ Continuar leitura' : '▶ Começar agora') + '</a><a class="btn ghost" href="#/cartas">🃏 Cartões de revisão</a>' + (temFolha ? '<a class="btn ghost" href="#/c/folha-de-cola">📋 Folha de cola</a>' : '') + '</div></section>';
    html += '<div class="stats"><div class="stat"><b>' + tr.filter(c => c.kind === 'capitulo').length + '</b><span>capítulos</span></div><div class="stat"><b>' + tr.filter(c => c.kind === 'lab').length + '</b><span>laboratórios (projetos)</span></div><div class="stat"><b>' + qs + '</b><span>perguntas de quiz</span></div><div class="stat"><b>' + cards + '</b><span>cartões de revisão</span></div><div class="stat"><b>~' + hours + 'h</b><span>de estudo</span></div></div>';
    if (last) html += '<a class="resume" href="#/c/' + last.id + '"><span style="font-size:2em">📖</span><div><small>Você parou em</small><b>' + esc(rotuloDaSecao(last) + last.title) + '</b></div><span class="btn primary go">Continuar →</span></a>';
    html += '<h2 class="sec-title">Como estudar</h2><div class="how">' +
      '<div><div class="em">🔁</div><b>Leia, depois faça</b><p>Cada ideia vem com código curto. Digite no seu ambiente (Visual Studio, VS Code ou Rider) — não copie e cole.</p></div>' +
      '<div><div class="em">🧩</div><b>Passo a passo</b><p>Os diagramas animados mostram o que acontece na memória, linha a linha.</p></div>' +
      '<div><div class="em">🎯</div><b>Quiz no fim</b><p>Responda sem olhar. Errar agora é barato — e fixa o conteúdo.</p></div>' +
      '<div><div class="em">🃏</div><b>Revisão espaçada</b><p>Volte aos cartões de revisão no dia seguinte e na semana seguinte.</p></div></div>';
    let lg = null;
    DATA.filter(c => c.kind !== 'glossario' && c.kind !== 'extra').forEach(c => {
      if (c.group !== lg) { if (lg !== null) html += '</div>'; html += '<h2 class="sec-title">' + esc(c.group) + '</h2><div class="grid">'; lg = c.group; }
      html += '<a class="tile' + (progress.done[c.id] ? ' done' : '') + '" style="--hue:' + hueOf(c) + '" href="#/c/' + c.id + '"><span class="tick">✓</span><span class="n">' + esc(KIND_LABEL[c.kind] + (c.num ? ' ' + c.num : '')) + '</span><h3>' + esc(c.title) + '</h3><p>' + esc(c.subtitle || '') + '</p><span class="foot"><span>⏱ ' + c.minutes + ' min</span><span>' + (countQuestions(c.md) ? countQuestions(c.md) + ' perguntas' : '') + '</span></span></a>';
    });
    html += '</div>' + outrosLivros() + '</div>';
    main.innerHTML = html; document.title = (META.titulo || NOME) + (META.legenda ? ' — ' + META.legenda : '');
    window.scrollTo(0, 0);
  }

  function viewChapter(c) {
    progress.last = c.id; saveProgress();
    renderSidebar(c.id);
    if (c.kind === 'glossario') return viewGlossary(c);
    const ctx = { headings: [] };
    const body = blocks(c.md.replace(/\r/g, '').split('\n'), ctx);
    const i = DATA.indexOf(c), prev = DATA[i - 1], next = DATA[i + 1];
    const pagerLink = (x, cls, lbl) => x ? '<a class="' + cls + '" href="#/c/' + x.id + '"><small>' + lbl + '</small><b>' + esc(rotuloDaSecao(x) + x.title) + '</b></a>' : '<span></span>';
    const kicker = KIND_LABEL[c.kind] + (c.num ? ' ' + c.num : '');
    let html = '<article class="page" style="--hue:' + hueOf(c) + '"><header class="hero" data-num="' + esc(c.badge || c.num || '') + '"><div class="kicker">' + esc(kicker) + '</div><h1>' + esc(c.title) + '</h1>' +
      (c.subtitle ? '<p class="sub">' + esc(c.subtitle) + '</p>' : '') +
      '<div class="meta"><span class="chip">⏱ ' + c.minutes + ' min</span>' + (ctx.quiz ? '<span class="chip">🎯 ' + ctx.quiz + ' perguntas</span>' : '') + '<span class="chip">' + esc(c.group || '') + '</span></div></header>';
    if (c.objectives && c.objectives.length) html += '<section class="goals"><h3>O que você vai aprender</h3><ul>' + c.objectives.map(o => '<li>' + inline(o) + '</li>').join('') + '</ul></section>';
    html += body;
    if (trackable(c)) html += '<div class="done-row" id="doneRow"></div>';
    html += '<nav class="pager">' + pagerLink(prev, 'prev', '← Anterior') + pagerLink(next, 'next', 'Próximo →') + '</nav></article>';
    main.innerHTML = html;
    hydrate(main, c); syncDoneRow(); linkRefs(main);
    document.title = rotuloDaSecao(c) + c.title + ' — ' + NOME;
    rail.innerHTML = ctx.headings.length ? '<h4>Neste capítulo</h4>' + ctx.headings.map(h => '<a href="#" data-id="' + h.id + '">' + esc(String(h.text).replace(/`/g, '')) + '</a>').join('') : '';
    $$('a', rail).forEach(a => a.addEventListener('click', e => { e.preventDefault(); const t = document.getElementById(a.dataset.id); if (t) t.scrollIntoView({ behavior: 'smooth' }); }));
    window.scrollTo(0, 0); onScroll();
  }

  function syncDoneRow() {
    const row = $('#doneRow'); if (!row) return;
    const id = sidebar.dataset.cur, done = progress.done[id], q = progress.quiz[id];
    row.innerHTML = done ? '<span style="font-size:1.6em">✅</span><div><b>Capítulo concluído!</b>' + (q ? '<br><small>Melhor resultado no quiz: ' + q.best + '/' + q.total + '</small>' : '') + '</div><button class="btn" type="button" id="undoBtn" style="margin-left:auto">Desmarcar</button>'
      : '<span style="font-size:1.6em">🏁</span><div><b>Terminou de estudar?</b><br><small>Faça o quiz acima ou marque manualmente.</small></div><button class="btn primary" type="button" id="doneBtn" style="margin-left:auto">Marcar como concluído</button>';
    row.style.background = done ? '' : 'var(--surface)'; row.style.borderColor = done ? '' : 'var(--line)';
    const b = $('#doneBtn'), u = $('#undoBtn');
    if (b) b.onclick = () => { progress.done[id] = true; saveProgress(); renderSidebar(); syncDoneRow(); toast('Capítulo concluído! 🎉'); };
    if (u) u.onclick = () => { delete progress.done[id]; saveProgress(); renderSidebar(); syncDoneRow(); };
  }

  function viewGlossary(c) {
    const entries = [];
    c.md.split('\n').forEach(l => { const m = l.match(/^- \*\*(.+?)\*\*(?: \((.+?)\))? — (.+)$/); if (m) entries.push({ t: m[1], ch: m[2] || '', d: m[3] }); });
    entries.sort((a, b) => a.t.localeCompare(b.t, 'pt-BR', { sensitivity: 'base' }));
    main.innerHTML = '<article class="page gloss"><header class="hero" style="--hue:' + hueOf(c) + '" data-num="Aa"><div class="kicker">Referência</div><h1>' + esc(c.title) + '</h1><p class="sub">' + esc(c.subtitle || '') + '</p></header>' +
      '<input class="gloss-filter" type="search" placeholder="Filtrar termos… (' + entries.length + ' no total)" aria-label="Filtrar glossário"><div id="glossBody"></div></article>';
    const draw = q => {
      const f = entries.filter(e => !q || (e.t + ' ' + e.d).toLowerCase().includes(q.toLowerCase()));
      let h = '<dl>', L = '';
      f.forEach(e => { const l = e.t[0].toUpperCase().normalize('NFD').replace(/[̀-ͯ]/g, ''); if (l !== L) { h += '</dl><div class="letter">' + l + '</div><dl>'; L = l; } h += '<dt>' + esc(e.t) + (e.ch ? '<small>cap. ' + esc(e.ch) + '</small>' : '') + '</dt><dd>' + inline(e.d) + '</dd>'; });
      $('#glossBody').innerHTML = f.length ? h + '</dl>' : '<p>Nada encontrado.</p>';
    };
    draw(''); $('.gloss-filter').addEventListener('input', e => draw(e.target.value));
    rail.innerHTML = ''; document.title = c.title + ' — ' + NOME; window.scrollTo(0, 0);
  }

  function allCards() {
    const list = [];
    DATA.forEach(c => { const re = /^::: cartas[^\n]*\n([\s\S]*?)\n:::/gm; let m; while ((m = re.exec(c.md))) m[1].split('\n').filter(l => l.includes(' :: ')).forEach(l => { const [f, b] = l.split(' :: '); list.push({ f, b, ch: c }); }); });
    return list;
  }
  function viewCards() {
    renderSidebar('cartas'); rail.innerHTML = '';
    const cards = allCards();
    const opts = '<option value="">Todos os capítulos (' + cards.length + ')</option>' + DATA.filter(c => cards.some(x => x.ch === c)).map(c => '<option value="' + c.id + '">' + esc((c.num ? c.num + '. ' : '') + c.title) + ' (' + cards.filter(x => x.ch === c).length + ')</option>').join('');
    main.innerHTML = '<article class="page study"><header class="hero" style="--hue:30" data-num="🃏"><div class="kicker">Revisão ativa</div><h1>Cartões de revisão</h1><p class="sub">Tente responder de cabeça antes de virar o cartão. Os que você errar voltam ao final da fila.</p></header>' +
      '<div class="study-bar"><select id="deckSel" aria-label="Capítulo">' + opts + '</select><button class="btn" id="deckShuffle" type="button">🔀 Embaralhar</button></div><div id="deckArea"></div></article>';
    let queue = [], total = 0, hits = 0;
    const area = $('#deckArea');
    function start() {
      const id = $('#deckSel').value; queue = shuffle(cards.filter(x => !id || x.ch.id === id)); total = queue.length; hits = 0; draw();
    }
    function draw() {
      if (!queue.length) { area.innerHTML = '<div class="done-row" style="justify-content:center;margin-top:20px"><span style="font-size:2em">🎉</span><div><b>Fila zerada!</b><br><small>Você revisou ' + total + ' cartões.</small></div><button class="btn primary" id="again" type="button">Recomeçar</button></div>'; $('#again').onclick = start; return; }
      const x = queue[0];
      area.innerHTML = '<p style="color:var(--ink-3)">Restam ' + queue.length + ' · acertos ' + hits + ' · <em>' + esc(x.ch.title) + '</em></p><div class="study-card">' + cardHtml(x.f, x.b).replace('class="card"', 'class="card" style="width:100%;height:240px"') + '</div><div class="study-bar"><button class="btn" id="miss" type="button">😅 Errei / revisar</button><button class="btn primary" id="hit" type="button">✅ Acertei</button></div>';
      hydrate(area); $('.study-card .card', area).style.cssText = 'width:100%;height:240px';
      $('#miss').onclick = () => { queue.push(queue.shift()); draw(); };
      $('#hit').onclick = () => { queue.shift(); hits++; draw(); };
    }
    $('#deckSel').onchange = start; $('#deckShuffle').onclick = start; start();
    document.title = 'Cartões de revisão — ' + NOME; window.scrollTo(0, 0);
  }

  function plain(md) {
    return md.replace(/```[\w#+-]*(?:\{[^}]*\})?[^\n]*\n/g, ' ').replace(/^```$/gm, ' ').replace(/^:::.*$/gm, ' ').replace(/<[^>]+>/g, ' ').replace(/[*`#>|=\[\]]/g, '').replace(/^[-+?]\s/gm, '').replace(/\s+/g, ' ');
  }
  let searchIndex = null;
  function viewSearch(q) {
    renderSidebar('search'); rail.innerHTML = '';
    if (!searchIndex) searchIndex = DATA.map(c => ({ c, text: plain(c.md), t: (c.title + ' ' + (c.subtitle || '')).toLowerCase() }));
    const terms = q.toLowerCase().normalize('NFD').replace(/[̀-ͯ]/g, '').split(/\s+/).filter(Boolean);
    const norm = s => s.toLowerCase().normalize('NFD').replace(/[̀-ͯ]/g, '');
    const res = terms.length ? searchIndex.map(x => {
      const lt = norm(x.text), tt = norm(x.t); let score = 0;
      for (const t of terms) { const n = lt.split(t).length - 1; if (!n && !tt.includes(t)) return null; score += n + (tt.includes(t) ? 15 : 0); }
      const p = lt.indexOf(terms[0]);
      const snip = p < 0 ? x.text.slice(0, 160) : (p > 70 ? '…' : '') + x.text.slice(Math.max(0, p - 70), p + 150) + '…';
      return { x, score, snip };
    }).filter(Boolean).sort((a, b) => b.score - a.score) : [];
    const mark = s => { let h = esc(s); terms.forEach(t => { h = h.replace(new RegExp('(' + t.replace(/[.*+?^${}()|[\]\\]/g, '\\$&') + ')', 'ig'), '<mark>$1</mark>'); }); return h; };
    main.innerHTML = '<article class="page"><h2 style="margin-top:0">Resultados para “' + esc(q) + '”</h2><p>' + res.length + ' seção(ões) encontrada(s).</p><ul class="results">' +
      res.slice(0, 30).map(r => '<li><a href="#/c/' + r.x.c.id + '"><small>' + esc(KIND_LABEL[r.x.c.kind] + (r.x.c.num ? ' ' + r.x.c.num : '')) + '</small><b>' + esc(r.x.c.title) + '</b><p>' + mark(r.snip) + '</p></a></li>').join('') + '</ul></article>';
    document.title = 'Busca: ' + q; window.scrollTo(0, 0);
  }

  /* =====================================================================
     Coleção: outros livros e links entre capítulos de livros diferentes
     ===================================================================== */
  function outrosLivros() {
    const outros = COL.filter(l => l.id !== META.id);
    if (!outros.length) return '';
    return '<h2 class="sec-title">Outros livros da coleção</h2><div class="mini-livros">' + outros.map(l =>
      '<a class="mini-livro" style="--c0:' + l.paleta.capa[0] + ';--c1:' + l.paleta.capa[1] + ';--c2:' + l.paleta.capa[2] + '" href="../' + l.pasta + '/index.html">' +
      '<span class="mk">' + esc(l.marca) + '</span><span class="tx"><small>' + esc(l.numero) + '</small><b>' + esc(l.titulo) + '</b><em>' + esc(l.faixa) + '</em></span></a>').join('') + '</div>';
  }

  /* "cap. 55", "caps. 41–57", "Lab 7" viram links; se o capítulo é de outro livro, abre o livro certo */
  function linkRefs(root) {
    if (!COL.length) return;
    const mapa = {};
    COL.forEach(l => {
      Object.entries(l.capitulos || {}).forEach(([n, id]) => { mapa['c' + n] = { l, id }; });
      Object.entries(l.labs || {}).forEach(([n, id]) => { mapa['l' + n] = { l, id }; });
    });
    const aqui = t => t.l.id === META.id;
    const mk = (txt, t) => '<a class="xref' + (aqui(t) ? '' : ' out') + '" href="' + (aqui(t) ? '#/c/' + t.id : '../' + t.l.pasta + '/index.html#/c/' + t.id) + '"' +
      (aqui(t) ? '' : ' title="No livro: ' + esc(t.l.titulo) + '"') + '>' + txt + '</a>';
    const nums = () => /\b((?:[Cc]aps?\.|[Cc]apítulos?)\s+)(\d{1,2}(?:\s*(?:,|\be\b|\ba\b|–|—|-)\s*\d{1,2})*)/g;
    const labs = () => /\b((?:Lab|Laboratório)s?\.?\s+)(\d+)\b/g;
    const bloqueado = el => !el || el.closest('svg,pre,code,a,button,header.hero,h1,h2,h3,h4,.card,.opt,.kicker,.side-link,textarea,kbd');
    const walker = document.createTreeWalker(root, NodeFilter.SHOW_TEXT);
    const nos = [];
    for (let n = walker.nextNode(); n; n = walker.nextNode()) {
      if (bloqueado(n.parentElement)) continue;
      if (nums().test(n.nodeValue) || labs().test(n.nodeValue)) nos.push(n);
    }
    nos.forEach(n => {
      const base = esc(n.nodeValue);
      const html = base
        .replace(nums(), (m, pre, lista) => pre + lista.replace(/\d{1,2}/g, d => { const t = mapa['c' + d]; return t ? mk(d, t) : d; }))
        .replace(labs(), (m, pre, d) => { const t = mapa['l' + d]; return t ? pre + mk(d, t) : m; });
      if (html === base) return;
      const span = document.createElement('span'); span.innerHTML = html; n.replaceWith(...span.childNodes);
    });
  }

  /* ---------- ampliar ilustrações ---------- */
  document.addEventListener('click', e => {
    const b = e.target.closest('.ilu-zoom');
    if (b) {
      const img = $('img', b), box = document.createElement('div');
      box.className = 'lightbox'; box.innerHTML = '<img src="' + esc(img.getAttribute('src')) + '" alt="' + esc(img.alt) + '"><button type="button" aria-label="Fechar">✕</button>';
      box.onclick = () => box.remove(); document.body.appendChild(box); return;
    }
  });
  document.addEventListener('keydown', e => { if (e.key === 'Escape') { const l = $('.lightbox'); if (l) l.remove(); } });

  /* ---------- roteador ---------- */
  function route() {
    document.body.classList.remove('menu-open');
    const h = decodeURIComponent(location.hash.replace(/^#\/?/, ''));
    if (!h) return viewHome();
    if (h === 'cartas') return viewCards();
    if (h.startsWith('busca/')) return viewSearch(h.slice(6));
    if (h.startsWith('c/')) { const c = byId(h.slice(2)); if (c) return viewChapter(c); }
    viewHome();
  }
  window.addEventListener('hashchange', route);

  /* ---------- chrome: tema, fonte, busca, menu, scroll ---------- */
  $('#themeBtn').onclick = () => {
    const d = document.documentElement, isDark = d.getAttribute('data-theme') === 'dark' || (!d.getAttribute('data-theme') && matchMedia('(prefers-color-scheme: dark)').matches);
    const t = isDark ? 'light' : 'dark'; d.setAttribute('data-theme', t); try { localStorage.setItem('sns.theme', t); } catch (e) { /* ok */ }
  };
  let fs = +store.get('sns.fs', store.get('sns.fs.legado', 17.5));
  const applyFs = () => document.documentElement.style.setProperty('--fs', fs + 'px');
  applyFs();
  $('#fontUp').onclick = () => { fs = Math.min(23, fs + 1); store.set('sns.fs', fs); applyFs(); };
  $('#fontDown').onclick = () => { fs = Math.max(14, fs - 1); store.set('sns.fs', fs); applyFs(); };
  $('#menuBtn').onclick = () => document.body.classList.toggle('menu-open');
  $('#scrim').onclick = () => document.body.classList.remove('menu-open');
  $('#searchForm').addEventListener('submit', e => { e.preventDefault(); const q = $('#searchInput').value.trim(); if (q) location.hash = '#/busca/' + encodeURIComponent(q); });
  document.addEventListener('keydown', e => {
    const typing = /^(INPUT|TEXTAREA|SELECT)$/.test((e.target.tagName || ''));
    if (e.key === '/' && !typing) { e.preventDefault(); $('#searchInput').focus(); }
    if (e.key === 'Escape') { document.body.classList.remove('menu-open'); $('#searchInput').blur(); }
    if (e.shiftKey && !typing && (e.key === 'ArrowRight' || e.key === 'ArrowLeft')) {
      const cur = byId(sidebar.dataset.cur); if (!cur) return;
      const t = DATA[DATA.indexOf(cur) + (e.key === 'ArrowRight' ? 1 : -1)]; if (t) location.hash = '#/c/' + t.id;
    }
  });
  let ticking = false;
  function onScroll() {
    if (ticking) return; ticking = true;
    requestAnimationFrame(() => {
      ticking = false;
      const h = document.documentElement, max = h.scrollHeight - h.clientHeight;
      $('#readbar').style.width = (max > 0 ? Math.min(100, h.scrollTop / max * 100) : 0) + '%';
      const links = $$('a', rail); if (!links.length) return;
      let on = links[0];
      links.forEach(a => { const t = document.getElementById(a.dataset.id); if (t && t.getBoundingClientRect().top < 140) on = a; });
      links.forEach(a => a.classList.toggle('on', a === on));
    });
  }
  window.addEventListener('scroll', onScroll, { passive: true });

  route();
})();
