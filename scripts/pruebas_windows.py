"""Pruebas funcionales de la app de Windows (docs/PRUEBAS_WINDOWS.md).

    python scripts/pruebas_windows.py preparar <carpeta>    # escribe <carpeta>/entrada/*.txt
    python scripts/pruebas_windows.py esperado              # lista lo que debe salir en cada prueba
    python scripts/pruebas_windows.py verificar <carpeta>   # compara <carpeta>/salida/*.txt con lo esperado

El «oráculo» de este script es independiente de la app: genera y filtra las columnas por fuerza bruta en Python con
las reglas del manual (y, donde el manual no llega, con las del WinForms original, la referencia del proyecto). No
usa el motor de la app. Cada prueba que la app graba en un fichero se comprueba aquí; las que solo muestran números en
pantalla traen su valor esperado en el documento.
"""
import itertools
import math
import random
import sys
from pathlib import Path

SIGNOS = "1X2"


# ---------------------------------------------------------------------------------------------- utilidades
def lineas(texto):
    """Líneas de un fichero de columnas: como la app, recorta y pasa a mayúsculas; sin la vacía final."""
    ls = texto.replace("\r\n", "\n").replace("\r", "\n").split("\n")
    if ls and ls[-1] == "":
        ls.pop()
    return [l.strip().upper() for l in ls]


def leer(ruta):
    return lineas(Path(ruta).read_text(encoding="utf-8-sig"))


def escribir(ruta, cols):
    Path(ruta).parent.mkdir(parents=True, exist_ok=True)
    Path(ruta).write_bytes("".join(c + "\r\n" for c in cols).encode("utf-8"))


def aciertos(a, b):
    return sum(x == y for x, y in zip(a, b))


def desarrollar(pron):
    """Todas las columnas de un boleto: pron = 14 textos con los signos de cada partido ('1', '1X', '1X2'…)."""
    return ["".join(t) for t in itertools.product(*[[s for s in SIGNOS if s in p] for p in pron])]


def racha(c, ok):
    mejor = act = 0
    for s in c:
        act = act + 1 if ok(s) else 0
        mejor = max(mejor, act)
    return mejor


def rango(texto):
    """'1,2' o '3-6' o '1,4-6' ⇒ conjunto. Vacío = 0..14."""
    if not texto:
        return set(range(15))
    r = set()
    for parte in texto.split(","):
        a, _, b = parte.partition("-")
        r.update(range(int(a), int(b or a) + 1))
    return r


def cumple(c, pron, var="", x="", dos="", s_var="", s_1="", s_x="", s_2=""):
    """Boleto + «Variantes, X y 2» + «Signos seguidos», según el manual (§6)."""
    return (all(c[i] in pron[i] for i in range(14))
            and c.count("X") + c.count("2") in rango(var) and c.count("X") in rango(x) and c.count("2") in rango(dos)
            and racha(c, lambda s: s != "1") in rango(s_var) and racha(c, "1".__eq__) in rango(s_1)
            and racha(c, "X".__eq__) in rango(s_x) and racha(c, "2".__eq__) in rango(s_2))


def interrupciones(c):
    """Interrupciones de una columna (manual §6 y el WinForms original): cada cambio de signo entre dos partidos
    seguidos. Global = todos; «1»/«X»/«2» = los que salen de ese signo; Var = los que van de X o 2 a 1. Seguidas = la
    racha más larga de cambios consecutivos; la de un tipo se corta con un signo repetido o con un cambio entre los
    otros dos signos (para Var, entre X y 2)."""
    n = {"global": 0, "var": 0, "1": 0, "X": 0, "2": 0}
    seg = dict.fromkeys(n, 0)
    racha = dict.fromkeys(n, 0)

    def cierra(*claves):
        for k in claves:
            seg[k] = max(seg[k], racha[k])
            racha[k] = 0

    for i in range(1, len(c)):
        a, b = c[i - 1], c[i]
        if a == b:
            cierra(*n)
            continue
        n["global"] += 1
        racha["global"] += 1
        n[a] += 1
        racha[a] += 1
        cierra({"1", "X", "2"}.difference((a, b)).pop())        # el signo que no interviene corta su racha
        if b == "1":
            n["var"] += 1
            racha["var"] += 1
        elif a != "1":
            cierra("var")                                         # X↔2 corta la racha de Var
    cierra(*n)
    return n, seg


def cumple_interrupciones(c, conds_int=None, conds_seg=None):
    n, seg = interrupciones(c)
    return (all(n[k] in rango(v) for k, v in (conds_int or {}).items())
            and all(seg[k] in rango(v) for k, v in (conds_seg or {}).items()))


# ---------------------------------------------------------------------------------------------- datos de prueba
P_I1 = ["1"] * 10 + ["1X", "X2", "1X2", "1"]
P_I2 = ["1X2"] * 6 + ["1X", "X2"] + ["1"] * 6
C_I1 = dict(var="1,2", s_1="10")
C_I2 = dict(var="3-6", x="1-4", s_var="1-3")
P_B3 = ["1X2", "1", "X2", "1X2", "1", "1X", "X", "1X2", "12", "1", "2", "1", "X", "1"]   # 3 triples, 3 dobles, 8 fijos
INT_C = {"global": "9,10", "2": "2,3"}                 # Interrupciones
SEG_C = {"global": "4,5,6", "var": "2,3"}              # Interrupciones seguidas
SS_C = dict(s_1="1,2", s_var="2,3")                    # Signos seguidos
P_JD01 = ["1X2"] * 6 + ["1"] * 8                       # máscara JD-01 de TAREAS_WINDOWS W9 (729 columnas)
P_ONCE = ["1X2"] * 11 + ["1"] * 3                       # 177.147 columnas: escrutinio en varios núcleos
GAN_ONCE = "1X21X21X21X111"

A_, X_, D_ = "1" * 14, "X" * 14, "2" * 14
D4 = "1X21X21X21X21X"


def filtro_medio():
    """500 líneas: 300 del boleto I2 (algunas repetidas) y 200 de cualquier sitio. Siempre las mismas (semilla)."""
    rnd = random.Random(1234)
    boleto = desarrollar(P_I2)
    cols = [rnd.choice(boleto) for _ in range(280)] + ["".join(rnd.choice(SIGNOS) for _ in range(14)) for _ in range(200)]
    cols += cols[:20]                                   # 20 repetidas
    rnd.shuffle(cols)
    return cols


ENTRADAS = {
    # Inicio con filtro
    "filtro_medio.txt": filtro_medio,
    "filtro_con_blanco.txt": lambda: ["X" * 14, "", "2" * 14, "1X" * 7],
    # Combinar filtros (F1)
    "F1_A.txt": lambda: ["11111111111111", "1111111111111X", "1111111111111X", "11111111111112"],
    "F1_B.txt": lambda: ["1111111111111x", "11111111111112", "111111111111X1"],
    "F1_C.txt": lambda: ["11111111111112", "111111111111X1", "11111111111121"],
    # Diferencias entre filtros (F2)
    "F2_1.txt": lambda: ["11111111111111", "1111111111111X", "11111111111112", "111111111111XX", "11111111111122"],
    "F2_2.txt": lambda: ["11111111111111"],
    "F2_3.txt": lambda: ["111111111111X1", "1111111111111X"],
    "F2_G.txt": lambda: ["11111111111111"],
    # Coincidencias (F3)
    "F3_E.txt": lambda: ["11111111111111", "1111111111111X", "11111111111112", "X1111111111112", "111111111111XX"],
    "F3_GA.txt": lambda: ["11111111111111"],
    "F3_GR.txt": lambda: ["1111111111111X"],
    # Aidomnou (F4) y Pim (F5)
    "F4_entrada.txt": lambda: ["XXXXXXXXXXXXXX", "11111111111111", "", "22222222222222", "1111111111111x", "11111111111111"],
    "F5_entrada.txt": lambda: ["111XXX11X1XX12", "11111111111111", "111XXX11X1XX12", "111xxx11x1xx12"],
    # Reductor (R1): 2 triples
    "R1_9.txt": lambda: [a + b + "1" * 12 for a in SIGNOS for b in SIGNOS],
    # Escrutinios
    "E1_A.txt": lambda: [D4, A_, "1X21X21X21X212"],
    "E2_R.txt": lambda: [D4, A_],
    "E3_once_triples.txt": lambda: desarrollar(P_ONCE),
    # Operaciones
    "O1_F1.txt": lambda: [A_, X_, A_, D_],
    "O1_F2.txt": lambda: [D_, D4, D_],
    "O2_entrada.txt": lambda: ["1X2111111111X2"],
    "O3_E1.txt": lambda: [A_, D_],
    "O3_E2.txt": lambda: [X_],
    "O4_diez.txt": lambda: ["%02d" % i + "1" * 12 for i in range(1, 11)],
    "O5_origen.txt": lambda: [A_],
    "O6_entrada.txt": lambda: [A_, "1X111111111111"],
    # W18 / W19
    "W18_entrada.txt": lambda: ["11111111111111", "2222222222222X"],
    "W19_banco.txt": lambda: ["11111111111111", "X1111111111111", "22222222222222"],
}

VALORACIONES = {                                        # para «Pegar desde Excel» (columnas Part. / Partido / 1 / X / 2)
    "val_60_25_15.txt": (60, 25, 15),
    "val_50_30_20.txt": (50, 30, 20),
    "val_34_33_33.txt": (34, 33, 33),
}
# Corrige los nombres del Fraccionador: líneas distintas y reconocibles.
ENTRADAS["O4_diez.txt"] = lambda: [("1" * 12 + "XX", "1" * 12 + "X2", "1" * 12 + "2X", "1" * 12 + "22",
                                    "1" * 11 + "XXX", "1" * 11 + "XX2", "1" * 11 + "X2X", "1" * 11 + "X22",
                                    "1" * 11 + "2XX", "1" * 11 + "2X2")[i] for i in range(10)]


def preparar(carpeta):
    e = Path(carpeta) / "entrada"
    for nombre, f in ENTRADAS.items():
        escribir(e / nombre, f())
    for nombre, (a, b, c) in VALORACIONES.items():
        filas = "".join(f"{i}\tPartido {i}\t{a}\t{b}\t{c}\r\n" for i in range(1, 15))
        (e / nombre).write_bytes(("Part.\tPartido\t1\tX\t2\r\n" + filas).encode("utf-8"))
    (Path(carpeta) / "salida").mkdir(parents=True, exist_ok=True)
    print(f"{len(ENTRADAS) + len(VALORACIONES)} ficheros en {e}")


# ---------------------------------------------------------------------------------------------- oráculos
def o_inicio(pron, cond, filtro=None):
    if filtro is None:
        return [c for c in desarrollar(pron) if cumple(c, pron, **cond)]
    return [c for c in filtro if c and cumple(c, pron, **cond)]       # línea vacía: no es columna


def o_combinar(textos, activos, mn, mx):
    cuenta = {}
    for cols, act in zip(textos, activos):
        if act:
            for c in {c for c in cols if len(c) == 14}:
                cuenta[c] = cuenta.get(c, 0) + 1
    mn, mx = max(mn, 1), min(mx, len(textos))
    return sorted(c for c, q in cuenta.items() if mn <= q <= mx)


def o_difiltros(base, filtros):
    """base: columnas; filtros: [(columnas, difs '1100', mínimo)]. Se queda c si (difs[0] y c ∈ F) o si tiene al menos
    «mínimo» columnas de F a 1, 2 o 3 signos (las distancias marcadas en difs[1..3])."""
    V = set(base)
    for cols, difs, mn in filtros:
        F = set(cols)
        t = [ch == "1" for ch in (difs + "0000")[:4]]
        V = {c for c in V if (t[0] and c in F)
             or (any(t[1:]) and sum(1 for f in F if 1 <= 14 - aciertos(c, f) <= 3 and t[14 - aciertos(c, f)]) >= mn)}
    return sorted(V)


def o_coincidencias_1a9(entrada, gan, conds):
    """Coincidencias con las características 1–9 (variantes, equis y doses de los 14, de los 7 primeros y de los 7
    últimos). conds = [(conjunto de características 1..9, rango de coincidencias)]."""
    def rx(c):
        p, u = c[:7], c[7:]
        f = lambda t: (sum(s != "1" for s in t), t.count("X"), t.count("2"))
        return f(c) + f(p) + f(u)
    ref = rx(gan)
    sal = []
    for c in sorted(set(entrada)):
        r = rx(c)
        if all(sum(ref[k - 1] == r[k - 1] for k in g) in ok for g, ok in conds):
            sal.append(c)
    return sal


def o_escrutar(cols, gan, rmin=10, rmax=14):
    P = [0] * 15
    prem = []
    for c in cols:
        n = sum(1 for i in range(14) if gan[i] == "*" or gan[i] == c[i])
        P[n] += 1
        if rmin <= n <= rmax:
            prem.append(c)
    return P, prem


def comprobar_reduccion(entrada, salida, nivel):
    """Garantía de una reducida: toda columna de la entrada tiene ≥ nivel aciertos con alguna de la salida."""
    E = set(entrada)
    d = 14 - nivel
    v = sum(math.comb(14, i) * 2 ** i for i in range(d + 1))
    fallos = [c for c in E if not any(aciertos(c, s) >= nivel for s in salida)]
    errores = []
    if fallos:
        errores.append(f"{len(fallos)} columnas de la entrada sin cubrir (p. ej. {fallos[0]})")
    if not set(salida) <= E:
        errores.append("hay columnas de salida que no están en la entrada")
    if not math.ceil(len(E) / v) <= len(set(salida)) <= len(E):
        errores.append(f"{len(set(salida))} columnas fuera de [{math.ceil(len(E) / v)}, {len(E)}]")
    return errores


def o_subir(origen, niveles):
    out = set()
    for c in origen:
        if 0 in niveles:
            out.add(c)
        for k in niveles - {0}:
            for P in itertools.combinations(range(14), k):
                for alt in itertools.product(*[[s for s in SIGNOS if s != c[p]] for p in P]):
                    x = list(c)
                    for p, a in zip(P, alt):
                        x[p] = a
                    out.add("".join(x))
    return sorted(out)


# ---------------------------------------------------------------------------------------------- esperado
def esperados():
    """{prueba: (descripción, columnas esperadas o None, comprobación especial o None)}."""
    E = {k: [c.strip().upper() for c in f()] for k, f in ENTRADAS.items()}
    r = {}
    r["I1"] = ("Inicio: boleto pequeño + Variantes + Signos seguidos, «Grabar archivo»", o_inicio(P_I1, C_I1), None)
    r["I2"] = ("Inicio: 6 triples y 2 dobles + condiciones", o_inicio(P_I2, C_I2), None)
    r["I3"] = ("Inicio: I2 + filtro de columnas filtro_medio.txt (repetidas incluidas)",
               o_inicio(P_I2, C_I2, E["filtro_medio.txt"]), None)
    r["I4"] = ("I3 abriendo «Calcular columnas» desde el panel lateral", r["I3"][1], None)
    r["I5"] = ("14 triples sin condiciones + filtro con una línea en blanco",
               o_inicio(["1X2"] * 14, {}, E["filtro_con_blanco.txt"]), None)
    F = [E["F1_A.txt"], E["F1_B.txt"], E["F1_C.txt"]]
    r["F1a"] = ("Combinar filtros A+B+C, mínimo 2 máximo 3", o_combinar(F, [1, 1, 1], 2, 3), None)
    r["F1b"] = ("Combinar filtros, 1–1", o_combinar(F, [1, 1, 1], 1, 1), None)
    r["F1c"] = ("Combinar filtros, 3–3", o_combinar(F, [1, 1, 1], 3, 3), None)
    r["F1d"] = ("Combinar filtros, 2–99", o_combinar(F, [1, 1, 1], 2, 99), None)
    r["F1e"] = ("Combinar filtros con B desactivado, 2–3", o_combinar(F, [1, 0, 1], 2, 3), None)
    b, f2, f3 = E["F2_1.txt"], E["F2_2.txt"], E["F2_3.txt"]
    r["F2a"] = ("Diferencias entre filtros: F2_1 + F2_2 (Difs 1100, Mín 1)", o_difiltros(b, [(f2, "1100", 1)]), None)
    r["F2b"] = ("Diferencias: Difs 0010", o_difiltros(b, [(f2, "0010", 1)]), None)
    r["F2c"] = ("Diferencias: Difs 1000", o_difiltros(b, [(f2, "1000", 1)]), None)
    r["F2d"] = ("Diferencias: + F2_3 (Difs 0100, Mín 2)", o_difiltros(b, [(f2, "1100", 1), (f3, "0100", 2)]), None)
    e3, ga = E["F3_E.txt"], E["F3_GA.txt"][0]
    r["F3a"] = ("Coincidencias 1-9;0-9", o_coincidencias_1a9(e3, ga, [(range(1, 10), rango("0-9"))]), None)
    r["F3b"] = ("Coincidencias 1-9;5-9", o_coincidencias_1a9(e3, ga, [(range(1, 10), rango("5-9"))]), None)
    r["F3c"] = ("Coincidencias 1-9;5-9 y 1-3;0-3",
                o_coincidencias_1a9(e3, ga, [(range(1, 10), rango("5-9")), (range(1, 4), rango("0-3"))]), None)
    b3 = desarrollar(P_B3)
    r["C1"] = ("B3 (3 triples, 3 dobles) + Interrupciones: Global 9,10 · «2» 2,3 · seguidas Global 4,5,6 · Var 2,3",
               [c for c in b3 if cumple_interrupciones(c, INT_C, SEG_C)], None)
    r["C2"] = ("B3 + Signos seguidos: «1» 1,2 · Var 2,3", [c for c in b3 if cumple(c, P_B3, **SS_C)], None)
    r["C3"] = ("B3 + Interrupciones (C1) + Signos seguidos (C2)",
               [c for c in b3 if cumple_interrupciones(c, INT_C, SEG_C) and cumple(c, P_B3, **SS_C)], None)
    r["F4"] = ("Aidomnou 60/25/15, C6 10-14, suma 60-84", ["11111111111111", "1111111111111X"], None)
    r["F5"] = ("Pim 60/25/15, rangos por defecto (algoritmo del WinForms original)", ["111XXX11X1XX12"], None)
    for m in ("a", "b", "c"):
        r["R1" + m] = ("Reductor 2 triples a 13 (método " + {"a": "menos tiempo", "b": "menos columnas",
                                                                "c": "grandes archivos"}[m] + ")",
                       None, ("reduccion", E["R1_9.txt"], 13, 3))
    r["R2"] = ("Inicio I2 ▸ Reducir a 12 (menos tiempo) ▸ Guardar resultado", None, ("reduccion", r["I2"][1], 12, None))
    r["R3"] = ("Máscara JD-01 ▸ Reducir a 13 (menos columnas)", None, ("reduccion", desarrollar(P_JD01), 13, None))
    P, prem = o_escrutar(E["E1_A.txt"], D4)
    r["E1"] = ("Escrutinio simple, «Guardar Todas» las premiadas", prem, None)
    P, prem = o_escrutar(E["E3_once_triples.txt"], GAN_ONCE)
    r["E3"] = ("Escrutinio de 177.147 columnas, «Guardar Todas» las premiadas (" +
               ", ".join(f"{k}: {P[k]}" for k in range(14, 9, -1)) + ")", prem, None)
    f1, f2 = E["O1_F1.txt"], E["O1_F2.txt"]
    r["O1-0"] = ("Álgebra: elimina repetidas de 1", list(dict.fromkeys(f1)), None)
    r["O1-1"] = ("Álgebra: suma sin repetidas", list(dict.fromkeys(f1 + f2)), None)
    r["O1-2"] = ("Álgebra: comunes (como el original: en el orden de 2 y con sus repetidas)",
                [c for c in f2 if c in set(f1)], None)
    r["O1-3"] = ("Álgebra: resta (1)-(2)", sorted(set(f1) - set(f2)), None)
    r["O2"] = ("Transposición 14..1", [c[::-1] for c in E["O2_entrada.txt"]], None)
    t = [1, 2, 3, 4, 5, 15, 16, 17, 18, 19, 20, 21, 13, 13]
    r["O3"] = ("Multiplicador con la plantilla por defecto",
               ["".join((a + b)[k - 1] for k in t) for a in E["O3_E1.txt"] for b in E["O3_E2.txt"]], None)
    r["O4"] = ("Fraccionador por tramos (4): los 4 ficheros juntos = las 10 líneas", None,
               ("trozos", E["O4_diez.txt"]))
    r["O5a"] = ("Subir categoría, nivel 1", o_subir(E["O5_origen.txt"], {1}), None)
    r["O5b"] = ("Subir categoría, niveles 0 y 1", o_subir(E["O5_origen.txt"], {0, 1}), None)
    r["O6a"] = ("Rotación: partido 1 «1 → X», sin corrección", ["X1111111111111", "XX111111111111"], None)
    r["O6b"] = ("Rotación: con corrección de fallos",
                ["11111111111111", "X1111111111111", "1X111111111111", "XX111111111111"], None)
    r["W18a"] = ("Ordenar por probabilidad, premio de 14 entre 1.000 y 2.000", ["11111111111111"], None)
    r["W18b"] = ("Ordenar por probabilidad, entre 2.000.000 y 3.000.000", ["2222222222222X"], None)
    r["W19"] = ("Banco de pruebas: filtro «Nº de veces 13» 1–1 y Grabar las seleccionadas",
                ["11111111111111", "X1111111111111"], None)
    return r


# ---------------------------------------------------------------------------------------------- verificar
def multiconjunto(cols):
    d = {}
    for c in cols:
        d[c] = d.get(c, 0) + 1
    return d


def verificar(carpeta):
    sal = Path(carpeta) / "salida"
    res = esperados()
    ok = mal = falta = 0
    for clave, (desc, cols, especial) in res.items():
        if especial and especial[0] == "trozos":
            trozos = sorted(sal.glob(clave + "_*.txt"))
            if not trozos:
                print(f"·  {clave:5} sin salida            {desc}"); falta += 1; continue
            juntas = [c for t in trozos for c in leer(t)]
            bien = multiconjunto(juntas) == multiconjunto(especial[1])
            detalle = f"{len(trozos)} ficheros, {len(juntas)} de {len(especial[1])} líneas"
        else:
            f = sal / (clave + ".txt")
            if not f.exists():
                print(f"·  {clave:5} sin salida            {desc}"); falta += 1; continue
            real = leer(f)
            if especial and especial[0] == "reduccion":
                _, entrada, nivel, exacto = especial
                errores = comprobar_reduccion(entrada, real, nivel)
                if exacto is not None and len(set(real)) != exacto:
                    errores.append(f"{len(set(real))} columnas, se esperaban {exacto}")
                bien = not errores
                detalle = f"{len(real)} columnas" + ("" if bien else ": " + "; ".join(errores))
            else:
                bien = multiconjunto(real) == multiconjunto(cols)
                detalle = f"{len(real)} columnas" + ("" if bien else f", se esperaban {len(cols)}")
                if not bien:
                    sobran = sorted(set(real) - set(cols))[:3]
                    faltan = sorted(set(cols) - set(real))[:3]
                    detalle += (f"; sobran {sobran}" if sobran else "") + (f"; faltan {faltan}" if faltan else "")
                elif real != cols and sorted(real) != real:
                    detalle += " (mismo contenido, otro orden)"
        print(f"{'OK' if bien else 'MAL':3} {clave:5} {detalle:30} {desc}")
        ok += bien
        mal += not bien
    print(f"\n{ok} bien, {mal} mal, {falta} sin salida")
    return mal == 0


def listar():
    for clave, (desc, cols, especial) in esperados().items():
        if cols is not None:
            muestra = ", ".join(cols[:4]) + (" …" if len(cols) > 4 else "")
            print(f"{clave:5} {len(cols):7} col.  {desc}\n      {muestra}")
        else:
            print(f"{clave:5}    —       {desc}")


def mascara_jd01(liga):
    """W9/W22: premios de la máscara JD-01 contra cada ganadora de Historico/<liga>.txt (líneas «jornada;columna»).
    Con m aciertos en los 8 fijos, las columnas con m + j aciertos son C(6, j)·2^(6−j) (6 triples, 1 signo de 3)."""
    tot = {k: 0 for k in range(14, 9, -1)}
    jornadas = con_premio = 0
    for l in Path(liga).read_text(encoding="utf-8-sig").splitlines():
        if ";" not in l:
            continue
        g = l.split(";", 1)[1].strip().upper()[:14]
        if len(g) < 14:
            continue
        jornadas += 1
        m = sum(1 for i in range(6, 14) if g[i] == "1")
        premio = False
        for j in range(7):
            k = m + j
            if k >= 10:
                tot[k] += math.comb(6, j) * 2 ** (6 - j)
                premio = True
        con_premio += premio
    print(f"{jornadas} jornadas escrutadas, {con_premio} con premio")
    print("  ".join(f"{k}: {v:,}".replace(",", ".") for k, v in tot.items()))


if __name__ == "__main__":
    sys.stdout.reconfigure(encoding="utf-8")
    orden = sys.argv[1] if len(sys.argv) > 1 else ""
    if orden == "preparar":
        preparar(sys.argv[2])
    elif orden == "esperado":
        listar()
    elif orden == "mascara":
        mascara_jd01(sys.argv[2])
    elif orden == "verificar":
        sys.exit(0 if verificar(sys.argv[2]) else 1)
    else:
        print(__doc__)
