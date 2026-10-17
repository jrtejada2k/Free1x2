#!/usr/bin/env python3
"""Convierte docs/MANUAL_USUARIO.md y docs/FAQ.md a HTML para pegar en WordPress (editor de código o bloque «HTML
personalizado»). Cubre solo el Markdown que usan esos dos documentos: títulos (con ancla al estilo GitHub, para que el
índice funcione), párrafos, negrita, cursiva, código (en línea y en bloque), enlaces, listas (un nivel de anidado), tablas,
citas y separadores.
Los párrafos que enlazan documentos internos del repositorio (*.md) no se publican.

Uso: python3 scripts/md-a-wordpress.py   → escribe docs/web/manual.html y docs/web/faq.html
"""
import html
import re
import sys
from pathlib import Path

RAIZ = Path(__file__).resolve().parent.parent


def ancla(texto):
    t = re.sub(r"[*`]", "", texto).strip().lower()
    t = re.sub(r"[^\w\- ]", "", t)
    return t.replace(" ", "-")


def en_linea(t):
    t = html.escape(t, quote=False)
    codigos = []

    def guardar(m):
        codigos.append(m.group(1))
        return f"\x00{len(codigos) - 1}\x00"

    t = re.sub(r"`([^`]+)`", guardar, t)
    t = re.sub(r"\*\*(.+?)\*\*", r"<strong>\1</strong>", t)
    t = re.sub(r"(?<![\w*])\*(?!\s)([^*]+?)(?<!\s)\*(?![\w*])", r"<em>\1</em>", t)

    def enlace(m):
        texto, url = m.group(1), m.group(2)
        if url.startswith("http"):
            return f'<a href="{url}" target="_blank" rel="noopener">{texto}</a>'
        return f'<a href="{url}">{texto}</a>' if url.startswith("#") else texto

    t = re.sub(r"\[([^\]]+)\]\(([^)]+)\)", enlace, t)
    return re.sub(r"\x00(\d+)\x00", lambda m: f"<code>{codigos[int(m.group(1))]}</code>", t)


def tabla(filas):
    celdas = [[c.strip() for c in f.strip().strip("|").split("|")] for f in filas]
    cab, cuerpo = celdas[0], celdas[2:]
    h = "".join(f"<th>{en_linea(c)}</th>" for c in cab)
    b = "".join("<tr>" + "".join(f"<td>{en_linea(c)}</td>" for c in f) + "</tr>" for f in cuerpo)
    return f'<figure class="wp-block-table"><table><thead><tr>{h}</tr></thead><tbody>{b}</tbody></table></figure>'


def lista(lineas):
    """Elementos `- ` o `1. ` con continuaciones sangradas y un nivel de sublista."""
    patron = re.compile(r"^(\s*)([-*]|\d+\.)\s+(.*)$")
    ordenada = bool(re.match(r"^\s*\d+\.", lineas[0]))
    items, base = [], len(patron.match(lineas[0]).group(1))
    for l in lineas:
        m = patron.match(l)
        if m and len(m.group(1)) <= base:
            items.append([m.group(3), []])
        elif m:
            items[-1][1].append(l)
        elif items[-1][1]:
            items[-1][1][-1] += " " + l.strip()
        else:
            items[-1][0] += " " + l.strip()
    etiqueta = "ol" if ordenada else "ul"
    cuerpo = "".join(f"<li>{en_linea(t)}{lista(sub) if sub else ''}</li>" for t, sub in items)
    return f"<{etiqueta}>{cuerpo}</{etiqueta}>"


def bloques(lineas):
    out, i = [], 0
    es_lista = re.compile(r"^\s*([-*]|\d+\.)\s+")
    while i < len(lineas):
        l = lineas[i]
        if not l.strip():
            i += 1
        elif l.startswith("```"):
            j = i + 1
            while j < len(lineas) and not lineas[j].startswith("```"):
                j += 1
            out.append("<pre><code>" + html.escape("\n".join(lineas[i + 1 : j]), quote=False) + "</code></pre>")
            i = j + 1
        elif m := re.match(r"^(#{1,4})\s+(.*)$", l):
            n, t = len(m.group(1)), m.group(2)
            if n > 1:  # el título (#) lo pone WordPress como título de la página
                out.append(f'<h{n} id="{ancla(t)}">{en_linea(t)}</h{n}>')
            i += 1
        elif re.match(r"^-{3,}\s*$", l):
            out.append("<hr>")
            i += 1
        elif l.startswith(">"):
            j = i
            while j < len(lineas) and lineas[j].startswith(">"):
                j += 1
            interior = [re.sub(r"^>\s?", "", x) for x in lineas[i:j]]
            out.append(f'<blockquote class="wp-block-quote">{bloques(interior)}</blockquote>')
            i = j
        elif l.lstrip().startswith("|"):
            j = i
            while j < len(lineas) and lineas[j].lstrip().startswith("|"):
                j += 1
            out.append(tabla(lineas[i:j]))
            i = j
        elif es_lista.match(l):
            j = i + 1
            while j < len(lineas) and lineas[j].strip() and (es_lista.match(lineas[j]) or lineas[j].startswith(" ")):
                j += 1
            out.append(lista(lineas[i:j]))
            i = j
        else:
            j = i
            while j < len(lineas) and lineas[j].strip() and not re.match(r"^(#|>|\||-{3,}|```|\s*([-*]|\d+\.)\s)", lineas[j]):
                j += 1
            texto = " ".join(x.strip() for x in lineas[i:j])
            if not re.search(r"\]\((?!http|#)[^)]*\.md\)", texto):
                out.append(f"<p>{en_linea(texto)}</p>")
            i = j
    return "\n".join(out)


def main():
    destino = RAIZ / "docs" / "web"
    destino.mkdir(exist_ok=True)
    for origen, salida in [("MANUAL_USUARIO.md", "manual.html"), ("FAQ.md", "faq.html")]:
        lineas = (RAIZ / "docs" / origen).read_text(encoding="utf-8").splitlines()
        (destino / salida).write_text(bloques(lineas) + "\n", encoding="utf-8")
        print(f"docs/web/{salida}")


if __name__ == "__main__":
    sys.exit(main())
