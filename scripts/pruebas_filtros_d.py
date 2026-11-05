"""Oráculo del grupo D de pruebas de filtros (plan_d.md): GEPT, Control Tol (Tolerancias), Separador de porcentajes
Juan Bellas (FiltroPorcenJB), Filtrar por límites (Tramificar ▸ Filtrar) y Listado de condiciones.

    python -I scripts/pruebas_filtros_d.py            # corre los asserts y lista lo esperado (ficheros y pantallas)
    python -I scripts/pruebas_filtros_d.py preparar <carpeta>   # escribe <carpeta>/entrada/FD_*.txt (como pw.preparar)
    python -I scripts/pruebas_filtros_d.py verificar <carpeta>  # compara <carpeta>/salida/FD1..FD6.txt con lo esperado

Independiente de la app: reimplementa cada regla en Python a partir del manual (docs/MANUAL_USUARIO.md) y, donde el
manual no llega, del WinForms original (Free1X2/UI/...), que es la referencia del proyecto. No llama a C#.
Mismo formato que scripts/pruebas_windows.py: ENTRADAS (nombre -> lambda con las líneas) y esperados() ->
{clave: (descripción, columnas_esperadas, None)}; las columnas se comparan con pw.leer() (recorta y pasa a mayúsculas).
"""
import collections
import itertools
import random
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import pruebas_windows as pw  # noqa: E402

N = 14
TOTAL = 3 ** N  # 4.782.969


# ============================================================================================ GEPT (pantalla)
# Free1X2/UI/GEPTFrm.cs:183-198 (Traductor). Manual §«Columnas GEPT» (MANUAL_USUARIO.md:594-603): 1.ª columna = último
# resultado del LOCAL, 2.ª = último del VISITANTE (G ganó, E empató, P perdió).
def gept(local, visitante):
    """Pronóstico GEPT razonado desde el fútbol, no copiado de la tabla: el 1 entra si el local ganó o el visitante
    perdió; el 2, si el visitante ganó o el local perdió; la X, si alguno empató. Si los dos ganaron o los dos perdieron
    (sin empate) queda «12»; un vacío no da pronóstico (el original escribe un espacio)."""
    if local not in "GEP" or visitante not in "GEP" or not local or not visitante:
        return " "
    s = ""
    if local == "G" or visitante == "P":
        s += "1"
    if local == "E" or visitante == "E":
        s += "X"
    if visitante == "G" or local == "P":
        s += "2"
    return s                     # se comprueba abajo (asserts) contra la tabla del original: coinciden las 9


GEPT_TABLA_ORIGINAL = {"GG": "12", "GE": "1X", "GP": "1", "EG": "X2", "EE": "X", "EP": "1X", "PG": "2", "PE": "X2",
                       "PP": "12"}                                   # GEPTFrm.cs:186-194, copiada solo para el assert
# Filas 1-9: las 9 combinaciones; 10 y 11: una casilla vacía; 12-14 sin tocar (las dos vacías).
GEPT_FILAS = [("G", "G"), ("G", "E"), ("G", "P"), ("E", "G"), ("E", "E"), ("E", "P"), ("P", "G"), ("P", "E"),
              ("P", "P"), ("G", ""), ("", "P"), ("", ""), ("", ""), ("", "")]


# ============================================================================================ Control Tol
# Motor: Free1X2.Domain/MotorCalculo/ControladorTol.cs, ToleranciaFiltros.cs, Grupo.cs:191-201 y 331-406; los valores con
# letra («7A») los leen FiltroNoVariantes.SetNoVariantes (FiltroNoVariantes.cs:229-252) y FiltroSignosSeguidos.
# Manual §6/§7 (MANUAL_USUARIO.md:350, 377, 389).
def lista(texto, todos=range(N + 1)):
    """'5,6,7A' -> {5: ' ', 6: ' ', 7: 'A'} (valor admitido -> letra de tolerancia). Vacío = todos, sin letra."""
    if not texto:
        return {v: " " for v in todos}
    r = {}
    for t in texto.split(","):
        t = t.strip()
        if t[-1].isalpha():
            r[int(t[:-1])] = t[-1]
        else:
            r[int(t)] = " "
    return r


def conjunto(texto):
    """Campo «Aciertos» de una tolerancia o «Fallos Controles»: lista con comas o rango a-b."""
    if "," in texto:
        return {int(x) for x in texto.split(",")}
    if "-" in texto:
        a, b = texto.split("-")
        return set(range(int(a), int(b) + 1))
    return {int(texto)}


def medidas(c):
    """Lo que miran «Variantes X y 2» y «Signos Seguidos» (manual §6; pw.cumple)."""
    return {"var": sum(s != "1" for s in c), "x": c.count("X"), "dos": c.count("2"),
            "s_var": pw.racha(c, lambda s: s != "1"), "s_1": pw.racha(c, "1".__eq__),
            "s_x": pw.racha(c, "X".__eq__), "s_2": pw.racha(c, "2".__eq__)}


def cumple_tol(valores_col, conds, tolerancias=(), fallos=""):
    """valores_col: {nombre: valor de la columna}; conds: {nombre: texto del campo ('' = todos)}.
    1) cada valor tiene que estar admitido (con o sin letra);
    2) por cada tolerancia (letras, aciertos): nº de condiciones cuyo valor lleva una de esas letras ∈ aciertos;
    3) con «Fallos Controles» vacío deben cumplirse todas; si no, el nº de tolerancias falladas ∈ fallos."""
    tags = {}
    for k, texto in conds.items():
        m = lista(texto)
        if valores_col[k] not in m:
            return False
        tags[k] = m[valores_col[k]]
    falladas = 0
    for letras, aciertos in tolerancias:
        n = sum(1 for letra in letras.upper() for t in tags.values() if t == letra)
        falladas += n not in conjunto(aciertos)
    return falladas == 0 if not fallos else falladas in conjunto(fallos)


# Pruebas en la app (boleto B3 del arnés: 216 columnas)
CT1_VX2 = {"var": "5,6,7,8A,9A", "x": "", "dos": "1,2,3,4A,5A,6A"}
CT1_SS = {"s_var": "", "s_1": "1,2,3A", "s_x": "", "s_2": ""}
CT1_TOL = [("A", "0,1")]
CT2_VX2 = dict(CT1_VX2, x="2,3,4B,5B,6B,7B")
CT2_TOL = [("A", "0,1"), ("B", "0")]
CT2_FALLOS = "1"


def o_ct(vx2, ss, tol, fallos=""):
    return [c for c in pw.desarrollar(pw.P_B3) if cumple_tol(medidas(c), {**vx2, **ss}, tol, fallos)]


def assert_ejemplo_manual_tolerancias():
    """MANUAL_USUARIO.md:384-389 (§7): 6 triples + 6 dobles + 2 fijos = 46.656; CP1 = los 6 triples en 6 dobles, CP2 =
    los 6 dobles en 6 fijos. Las dos a 3–6 ⇒ 27.552; las dos a 4–5 con 3 y 6 de tolerancia y como mucho una en un valor
    tolerado ⇒ 22.848. Con la regla de cumple_tol: aciertos '3A,4,5,6A' en las dos y la tolerancia A con aciertos 0,1."""
    pron = ["1X2"] * 6 + ["1X"] * 6 + ["1", "1"]
    cp1 = ["1X"] * 6               # el doble de cada triple (por simetría da igual cuál)
    cp2 = ["1"] * 6                # el fijo de cada doble
    cols = pw.desarrollar(pron)
    assert len(cols) == 46656
    def ac(c):
        return {"cp1": sum(c[i] in cp1[i] for i in range(6)), "cp2": sum(c[6 + i] == cp2[i] for i in range(6))}
    assert sum(cumple_tol(ac(c), {"cp1": "3,4,5,6", "cp2": "3,4,5,6"}) for c in cols) == 27552
    assert sum(cumple_tol(ac(c), {"cp1": "3A,4,5,6A", "cp2": "3A,4,5,6A"}, [("A", "0,1")]) for c in cols) == 22848
    # y los recuentos de la «restricción progresiva» del mismo apartado (MANUAL_USUARIO.md:368-371), sin tolerancias
    assert sum(cumple_tol(ac(c), {"cp1": "3,4,5", "cp2": ""}) for c in cols) == 37888
    assert sum(cumple_tol(ac(c), {"cp1": "3,4,5", "cp2": "3,4,5"}) for c in cols) == 24272
    assert sum(cumple_tol(ac(c), {"cp1": "4,5,6", "cp2": "4,5,6"}) for c in cols) == 10912


# ============================================================================================ Listado de condiciones
# ListadoCondicionesFrm.cs:80-776 (original) = ListadoCondicionesFrmViewModel.cs:72-170 (port); exportación a texto:
# ListadoCondicionesFrm.cs:777-806 («ExportarATexto»).
def exportar_texto(hijos, prof=0):
    t = ""
    for texto, nietos in hijos:
        if nietos:
            t += " " * prof + texto + "\r\n\r\n" + exportar_texto(nietos, prof + 1) + "\r\n"
        else:
            t += " " * prof + texto + "\r\n"
    return t


def todos():
    return ",".join(str(i) for i in range(N + 1))


def arbol_listado(pron, vx2, ss):
    """Árbol para: combinación nueva, boleto pron, «Variantes X y 2» y «Signos Seguidos» (campos vacíos = 0..14),
    sin filtro de columnas, sin grupos, sin If-Then. Las tolerancias del grupo no salen (ni en el original)."""
    h = lambda t: (t, [])
    pron_txt = [",".join(s for s in "1X2" if s in p) for p in pron]
    cond = [("NoVariantes", [h("Cantidad de V: " + (vx2["var"] or todos())),
                             h("Cantidad de X: " + (vx2["x"] or todos())),
                             h("Cantidad de 2: " + (vx2["dos"] or todos()))]),
            ("SignosSeguidos", [h("Cantidad de V Seguidas: " + (ss["s_var"] or todos())),
                                h("Cantidad de 1 Seguidos: " + (ss["s_1"] or todos())),
                                h("Cantidad de X Seguidas: " + (ss["s_x"] or todos())),
                                h("Cantidad de 2 Seguidos: " + (ss["s_2"] or todos()))])]
    return [("Pronóstico Base", [h(f"Partido {i + 1}: {p}") for i, p in enumerate(pron_txt)]),
            ("Filtro de Columnas", [h("No se usa ningún Filtro")]),
            ("Grupos", [("Boleto Base", [h("Partidos Activos: "), ("Condiciones", cond)])]),
            ("Control de grupos", [("Control Conjuntos", [h("Conjuntos: 0"), h("Fallos: 0")])]),
            ("IfThen", [h("IfThen no activado")])]


# ============================================================================================ Separador JB
# Free1X2/UI/FiltroPorcenJB.cs: RecuperaPantalla 224-237, RecuperaRangos 239-264, valora 265-277, Cambia 386-397,
# Calcular 424-464, Valida 465-482, Analizar 536-568, ExporCols 602-618, s2n 629-637. El manual no lo describe.
LIM_DEF = (15, 22, 29, 36, 48, 61)                       # FiltroPorcenJBViewModel.cs:45-61 (defaults del Designer)
RANGOS_DEF = ["0-3"] * 7
RECO_DEF = "0-14"
PCT = [(45, 30, 25), (70, 20, 10), (20, 35, 45), (40, 31, 29), (65, 22, 13), (61, 22, 17), (33, 40, 27),
       (38, 31, 31), (50, 15, 35), (80, 12, 8), (27, 25, 48), (60, 25, 15), (36, 39, 25), (75, 15, 10)]
LIM_FD5 = (10, 20, 30, 40, 50, 60)
RANGOS_FD5 = ["1-3", "2-4", "1-4", "1-5", "1-3", "1-4", "0-2"]
RECO_FD5 = "3-5"
GAN_JB = "1X21X21X21X211"


def banda(pct, lims):
    """Banda 1..7 (índice 0..6): la primera cuyo límite es >= el porcentaje (comparación «<=», entero truncado)."""
    for i, l in enumerate(lims):
        if int(pct) <= l:
            return i
    return 6


def jb_cuenta(col, lims):
    """Por banda, cuántos partidos tienen el signo de la columna clasificado en esa banda."""
    n = [0] * 7
    for i, s in enumerate(col[:N]):
        n[banda(PCT[i]["1X2".index(s)], lims)] += 1
    return n


def jb_rango(t):
    a, b = t.split("-")
    return int(a), int(b)


def jb_filtrar(lineas, rangos, reco, lims):
    """Devuelve (procesadas, admitidas en el orden del fichero). Repetidas: solo cuenta la primera aparición
    (índice ternario s2n, FiltroPorcenJB.cs:451-454); las líneas vacías las salta el port (ver hallazgo)."""
    vistos, adm, proc = set(), [], 0
    R = [jb_rango(r) for r in rangos]
    ra, rb = jb_rango(reco)
    for l in lineas:
        c = l.strip().upper()
        if not c:
            continue
        proc += 1
        if c[:N] in vistos:
            continue
        vistos.add(c[:N])
        n = jb_cuenta(c, lims)
        if all(a <= x <= b for x, (a, b) in zip(n, R)) and ra <= max(n) - min(n) <= rb:
            adm.append(c)
    return proc, adm


def jb_exportar(lims):
    """Fichero de «Exportar»: 7 líneas (bandas) × 14 partidos con los signos de cada partido en esa banda."""
    out = []
    for b in range(7):
        celdas = []
        for p in PCT:
            s = "".join(sig for sig, v in zip("1X2", p) if banda(v, lims) == b)
            celdas.append(s or "-")
        out.append(",".join(celdas))
    return out


def jb_entrada():
    rnd = random.Random(2026)
    cols = ["".join(rnd.choice("1X2") for _ in range(N)) for _ in range(300)]
    rep = [cols[i].lower() if i % 3 == 0 else cols[i] for i in range(10)]   # 10 repetidas (4 en minúsculas)
    return cols + rep


def jb_porcentajes():
    return ["Part.\t1\tX\t2"] + [f"{i + 1}\t{a}\t{x}\t{d}" for i, (a, x, d) in enumerate(PCT)]


def assert_jb():
    # Cada partido aporta su signo a exactamente una banda: la suma de las 7 cuentas es 14.
    for c in jb_entrada():
        assert sum(jb_cuenta(c.upper(), LIM_DEF)) == N
    # Ejemplo a mano: P1 45/30/25 con límites por defecto ⇒ 1 en banda 5 (37-48), X en 4 (30-36), 2 en 3 (23-29).
    assert [banda(v, LIM_DEF) for v in PCT[0]] == [4, 3, 2]
    # Fronteras «<=»: 29 cae en la banda 3 (≤29), 36 en la 4, 48 en la 5, 61 en la 6, 22 en la 2, 15 en la 1.
    assert [banda(v, LIM_DEF) for v in (15, 22, 29, 36, 48, 61, 62)] == [0, 1, 2, 3, 4, 5, 6]
    # Las bandas que exporta coinciden con las cuentas: la columna de 14 unos tiene en cada banda tantos partidos como
    # «1» aparezcan en esa línea del fichero exportado.
    exp = jb_exportar(LIM_DEF)
    assert jb_cuenta("1" * N, LIM_DEF) == [sum("1" in celda for celda in linea.split(",")) for linea in exp]


# ============================================================================================ Filtrar por límites
# Tramificar ▸ Filtrar: TramificarForm.cs:3015-3052 (btFiltrar_Click), ObtenerExtremos 3055, EliminarColumnas 3089;
# diálogo DialogoFiltrarPorLimitesFrm.cs:117-151 (lo que muestra), 1163-1222 (CoherenciarExtremos/PonerTextoEnVariables).
# Manual MANUAL_USUARIO.md:633-637. Sin «diferencias hasta» > 0 (eso depende del orden de las 4.782.969 columnas, que
# calcula Tramificar), el resultado es aritmética de posiciones: no depende de la valoración.
DIFS_ORIGINAL = [(0, 4), (1, 3), (1, 2), (1, 1), (1, 0), (1, 0), (1, 1), (1, 2), (1, 3), (0, 4)]  # Designer original
FL_MIN = {10: 101, 11: 201, 12: 301, 13: 401, 14: 501}      # lo que se teclea en «Mínima» (Tramificar)
FL_MAX = {14: 1000, 13: 2000, 12: 3000, 11: 4000, 10: 5000}  # y en «Máxima»


def fl_extremos(mn, mx):
    e = [[0, mn[10] - 1], [mn[10] - 1, mn[11] - 1], [mn[11] - 1, mn[12] - 1], [mn[12] - 1, mn[13] - 1],
         [mn[13] - 1, mn[14] - 1], [mx[14], mx[13]], [mx[13], mx[12]], [mx[12], mx[11]], [mx[11], mx[10]],
         [mx[10], TOTAL - 1]]
    for i in range(9):                                           # CoherenciarExtremos
        e[i + 1][0] = max(e[i + 1][0], e[i][0] + 1)
    return e


def fl_posiciones(mn, mx):
    """Texto de la columna «Posiciones» del diálogo: fila 1 «ini a fin», las demás «ini+1 a fin»."""
    e = fl_extremos(mn, mx)
    return [f"{a if i == 0 else a + 1} a {b}" for i, (a, b) in enumerate(e)]


def fl_resultado(mn, mx, difs, fila10_funciona=True):
    """Nº de columnas que quedan tras Aceptar con estas «Difs. desde / hasta» (solo «hasta» = 0). Desde D-FL4
    (4-nov-2026) la fila 10 cuenta; con fila10_funciona=False se calcula lo del original, que copiaba la fila 9
    (DialogoFiltrarPorLimitesFrm.cs:1217-1220)."""
    assert all(h == 0 for _, h in difs), "con «hasta» > 0 el resultado depende del orden de Tramificar"
    e = fl_extremos(mn, mx)
    shown = [[a if i == 0 else a + 1, b] for i, (a, b) in enumerate(e)]
    filas = [list(shown[i]) + list(difs[i]) for i in range(10)]
    if not fila10_funciona:
        filas[9] = list(filas[8])
    for i in range(9):                                           # CoherenciarExtremos otra vez, al Aceptar
        filas[i + 1][0] = max(filas[i + 1][0], filas[i][0] + 1)
    tramos = []
    for ini, fin, desde, _ in filas:
        if ini > 0:
            ini -= 1
        if desde == 0 and fin > ini:
            tramos.append((ini, fin))                            # posiciones [ini, fin) del orden de Tramificar
    fuera, hasta = 0, 0
    for a, b in sorted(tramos):                                  # tamaño de la unión de los tramos
        a = max(a, hasta)
        if b > a:
            fuera += b - a
            hasta = b
    return TOTAL - fuera


def assert_fl():
    # Manual :636: «1 a 0» no elimina nada.
    assert fl_resultado(FL_MIN, FL_MAX, [(1, 0)] * 10) == TOTAL
    # Manual :637: «0 a 0» quita solo las columnas del intervalo: la fila «< 10» (posiciones 1..100 en pantalla «0 a 100»)
    assert fl_resultado(FL_MIN, FL_MAX, [(0, 0)] + [(1, 0)] * 9) == TOTAL - 100


# ============================================================================================ entradas y esperados
ENTRADAS = {
    "FD_JB_entrada.txt": jb_entrada,
    "FD_JB_porcentajes.txt": jb_porcentajes,
}


def esperados():
    E = {k: f() for k, f in ENTRADAS.items()}
    r = {}
    r["FD1"] = ("Control Tol: B3 + Variantes (Var 5,6,7,8A,9A · 2 1,2,3,4A,5A,6A) + Signos Seguidos (1 = 1,2,3A) + "
                "Tolerancia A aciertos 0,1", o_ct(CT1_VX2, CT1_SS, CT1_TOL), None)
    r["FD2"] = ("Control Tol: FD1 + X 2,3,4B,5B,6B,7B + Tolerancia B aciertos 0 + Fallos Controles 1 (exactamente una falla)",
                o_ct(CT2_VX2, CT1_SS, CT2_TOL, CT2_FALLOS), None)
    r["FD3"] = ("Listado de condiciones ▸ Exportar a Texto con el estado de FD2",
                pw.lineas(exportar_texto(arbol_listado(pw.P_B3, CT2_VX2, CT1_SS))), None)
    proc, adm = jb_filtrar(E["FD_JB_entrada.txt"], RANGOS_DEF, RECO_DEF, LIM_DEF)
    r["FD4"] = (f"Separador JB: FD_JB_entrada.txt con límites y rangos por defecto (Procesadas {proc})", adm, None)
    proc, adm = jb_filtrar(E["FD_JB_entrada.txt"], RANGOS_FD5, RECO_FD5, LIM_FD5)
    r["FD5"] = (f"Separador JB: límites {','.join(map(str, LIM_FD5))}, bandas {' '.join(RANGOS_FD5)}, recorrido "
                f"{RECO_FD5} (Procesadas {proc})", adm, None)
    r["FD6"] = ("Separador JB: «Exportar» con límites por defecto (7 líneas, signos por banda)",
                [l.upper() for l in jb_exportar(LIM_DEF)], None)
    return r


def pantalla():
    """Lo que se comprueba mirando la pantalla (va en plan_d.md)."""
    p = {}
    for k, (vx2, tol, fl) in {"FD1": (CT1_VX2, CT1_TOL, ""), "FD2": (CT2_VX2, CT2_TOL, CT2_FALLOS)}.items():
        p[k + " pantalla de cálculo"] = [f"Procesadas 216, Aceptadas {len(o_ct(vx2, CT1_SS, tol, fl))}"]
    p["FD2 diagnóstico"] = [f"si «Fallos Controles» se tomara como «0,1»: {len(o_ct(CT2_VX2, CT1_SS, CT2_TOL, '0,1'))}; "
                            f"si se ignorara (vacío): {len(o_ct(CT2_VX2, CT1_SS, CT2_TOL))}; "
                            f"sin tolerancias: {len(o_ct(CT2_VX2, CT1_SS, []))}"]
    E = jb_entrada()
    for k, (R, reco, lims) in {"FD4": (RANGOS_DEF, RECO_DEF, LIM_DEF), "FD5": (RANGOS_FD5, RECO_FD5, LIM_FD5)}.items():
        proc, adm = jb_filtrar(E, R, reco, lims)
        p[k + " pantalla JB"] = [f"Procesadas: {proc}  Admitidas: {len(adm)}"]
    p["G1 GEPT"] = [f"{i + 1:02d} {a or '—'}/{b or '—'} ⇒ «{gept(a, b).strip() or '(vacío)'}»"
                    for i, (a, b) in enumerate(GEPT_FILAS)]
    n = jb_cuenta(GAN_JB, LIM_DEF)
    p["JB-A Analizar " + GAN_JB + " (por defecto)"] = [f"Banda {i + 1}: {v}" for i, v in enumerate(n)] + \
        [f"Recorrido: {max(n) - min(n)}"]
    n = jb_cuenta(GAN_JB, LIM_FD5)
    p["JB-B Analizar " + GAN_JB + " (límites FD5)"] = [f"Banda {i + 1}: {v}" for i, v in enumerate(n)] + \
        [f"Recorrido: {max(n) - min(n)}"]
    p["FL1 Posiciones"] = fl_posiciones(FL_MIN, FL_MAX)
    p["FL1 Difs. según el original/manual"] = [f"{a}–{b}" for a, b in DIFS_ORIGINAL]
    p["FL2 todo 0–0"] = [f"Nº columnas filtro = {fl_resultado(FL_MIN, FL_MAX, [(0, 0)] * 10)} "
                         f"(en el original: {fl_resultado(FL_MIN, FL_MAX, [(0, 0)] * 10, False)})"]
    p["FL3 solo fila 1 a 0–0, resto 1–0"] = [f"Nº columnas filtro = {fl_resultado(FL_MIN, FL_MAX, [(0, 0)] + [(1, 0)] * 9)}"]
    p["FL4 solo fila 10 a 0–0, resto 1–0"] = [
        f"Nº columnas filtro = {fl_resultado(FL_MIN, FL_MAX, [(1, 0)] * 9 + [(0, 0)])} "
        f"(en el original: {fl_resultado(FL_MIN, FL_MAX, [(1, 0)] * 9 + [(0, 0)], False)})"]
    return p


def asserts():
    for (a, b), v in zip([(x, y) for x in "GEP" for y in "GEP"], GEPT_TABLA_ORIGINAL.values()):
        assert GEPT_TABLA_ORIGINAL[a + b] == gept(a, b), (a, b)
    swap = str.maketrans("12", "21")
    for a in "GEP":                                               # simetría local/visitante ⇔ 1/2
        for b in "GEP":
            assert "".join(sorted(gept(b, a), key="1X2".index)) == \
                "".join(sorted(gept(a, b).translate(swap), key="1X2".index))
    assert gept("G", "") == " " and gept("", "") == " "
    assert_ejemplo_manual_tolerancias()
    assert_jb()
    assert_fl()
    # Las columnas de FD1/FD2 cumplen, una por una, las condiciones sin tolerancias (y las que faltan no).
    b3 = pw.desarrollar(pw.P_B3)
    fd1 = set(o_ct(CT1_VX2, CT1_SS, CT1_TOL))
    sin_tol = set(o_ct(CT1_VX2, CT1_SS, []))
    assert fd1 < sin_tol < set(b3)
    for c in sin_tol - fd1:                                       # rechazadas solo por la tolerancia: ≥ 2 valores con A
        m = medidas(c)
        assert (m["var"] in (8, 9)) + (m["dos"] in (4, 5, 6)) + (m["s_1"] == 3) >= 2


def preparar(carpeta):
    e = Path(carpeta) / "entrada"
    for nombre, f in ENTRADAS.items():
        pw.escribir(e / nombre, f())
    (Path(carpeta) / "salida").mkdir(parents=True, exist_ok=True)
    print(f"{len(ENTRADAS)} ficheros en {e}")


if __name__ == "__main__":
    sys.stdout.reconfigure(encoding="utf-8")
    asserts()
    if len(sys.argv) > 2 and sys.argv[1] == "preparar":
        preparar(sys.argv[2])
        sys.exit(0)
    if len(sys.argv) > 2 and sys.argv[1] == "verificar":              # compara <carpeta>/salida/FD*.txt
        pw.esperados = esperados                                     # el verificador del arnés, con estos esperados
        sys.exit(0 if pw.verificar(sys.argv[2]) else 1)
    print("asserts OK\n")
    for clave, (desc, cols, _) in esperados().items():
        print(f"{clave:4} {len(cols):5} líneas  {desc}")
    print()
    for k, v in pantalla().items():
        print(k)
        for linea in v:
            print("     " + linea)
