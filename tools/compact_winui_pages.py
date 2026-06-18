#!/usr/bin/env python3
"""
Compacta la densidad visual de las paginas WinUI portadas para igualar la
densidad ya aceptada en MainPage. SOLO tamano: paddings, margenes, spacing,
row/column spacing y separaciones. No toca colores, bindings, estructura ni
nombres de control.

Cada regla se aplica con un ancla de frontera (\b en el nombre del atributo)
para evitar colisiones de subcadena: p.ej. la regla Spacing NO debe afectar a
RowSpacing ni ColumnSpacing. El valor se compara EXACTO entre comillas, de modo
que un numero en otro atributo (Width="12") nunca se toca.
"""
import re
import sys
import pathlib

# (atributo, valor_viejo) -> valor_nuevo
REPLACEMENTS = [
    # ---- Padding (relleno de pagina / tarjetas) ----
    ('Padding', '32',            '12,8'),
    ('Padding', '24',            '10,7'),
    ('Padding', '16',            '10,7'),
    ('Padding', '32,32,32,8',    '12,8'),
    ('Padding', '32,16',         '12,8'),
    ('Padding', '32,8,32,8',     '12,8'),
    ('Padding', '24,8,24,8',     '10,7'),
    ('Padding', '24,8,24,24',    '10,7'),
    ('Padding', '24,12',         '10,7'),
    ('Padding', '12,10',         '10,7'),
    ('Padding', '4,16,4,4',      '4,8,4,4'),
    ('Padding', '12,6',          '8,5'),

    # ---- ColumnSpacing (se procesan ANTES que Spacing por seguridad,
    #      aunque la frontera \b ya impide colisiones) ----
    ('ColumnSpacing', '24',      '12'),
    ('ColumnSpacing', '20',      '12'),
    ('ColumnSpacing', '16',      '10'),

    # ---- RowSpacing ----
    ('RowSpacing', '20',         '8'),
    ('RowSpacing', '16',         '6'),
    ('RowSpacing', '12',         '6'),
    ('RowSpacing', '10',         '6'),

    # ---- Spacing (StackPanel / StackLayout) ----
    ('Spacing', '24',            '8'),
    ('Spacing', '20',            '8'),
    ('Spacing', '16',            '6'),
    ('Spacing', '14',            '6'),
    ('Spacing', '12',            '5'),
    ('Spacing', '10',            '6'),

    # ---- Margin (separaciones de seccion y filas) ----
    ('Margin', '0,0,0,20',       '0,0,0,10'),
    ('Margin', '0,0,0,16',       '0,0,0,8'),
    ('Margin', '0,16,0,0',       '0,8,0,0'),
    ('Margin', '0,12,0,0',       '0,6,0,0'),
    ('Margin', '24,0,0,0',       '16,0,0,0'),
    ('Margin', '28,0,0,0',       '16,0,0,0'),
    ('Margin', '6,4',            '4,2'),
    ('Margin', '24',             '12'),
    ('Margin', '32',             '12'),
    ('Margin', '24,8,24,8',      '12,6'),
    ('Margin', '24,24,24,8',     '12,8'),
]


def process(text):
    n = 0
    for attr, old, new in REPLACEMENTS:
        # (?<![A-Za-z])  -> el atributo no es sufijo de otro (ColumnSpacing).
        # \b al final del nombre + valor exacto entre comillas.
        pattern = re.compile(r'(?<![A-Za-z])' + re.escape(attr) + r'="' + re.escape(old) + r'"')
        repl = f'{attr}="{new}"'
        text, c = pattern.subn(repl, text)
        n += c
    return text, n


def main():
    root = pathlib.Path(sys.argv[1])
    files = sorted(root.glob('*.xaml'))
    total = 0
    changed = 0
    for f in files:
        original = f.read_text(encoding='utf-8')
        updated, n = process(original)
        if n:
            f.write_text(updated, encoding='utf-8')
            total += n
            changed += 1
            print(f'{f.name}: {n} replacements')
    print(f'--- {changed}/{len(files)} files changed, {total} total replacements ---')


if __name__ == '__main__':
    main()
