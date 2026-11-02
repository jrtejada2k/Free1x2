"""Oráculo del grupo C de pruebas de filtros (FC1…FC12): Columnas probables, Grupos de equipos, If-Then, Pares, Tríos
y Dependencia lineal.

    python -I scripts/pruebas_filtros_c.py                      # corre los asserts y lista clave · nº de columnas · descripción
    python -I scripts/pruebas_filtros_c.py preparar <carpeta>   # escribe <carpeta>/entrada/FC_*.{txt,par,tri}
    python -I scripts/pruebas_filtros_c.py verificar <carpeta>  # compara <carpeta>/salida/FC*.txt con lo esperado

Independiente de la app: genera las columnas por fuerza bruta en Python y aplica cada regla tal como la describe el
manual (docs/MANUAL_USUARIO.md) y, donde el manual no llega, tal como la aplica el motor / el WinForms original (se
dice en cada función). No llama a C#. Formato de salida = el de scripts/pruebas_windows.py: 14 signos en mayúsculas,
una columna por línea.
"""
import itertools
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import pruebas_windows as pw  # noqa: E402

SIGNOS = "1X2"
B3 = pw.desarrollar(pw.P_B3)          # 216 columnas (P1 1X2 · P2 1 · P3 X2 · P4 1X2 · P5 1 · P6 1X · P7 X · P8 1X2 · P9 12 · …)
assert len(B3) == 216


def conj(texto):
    """'3,4,5' o '3-5' o '' (= cualquiera) ⇒ conjunto de enteros (0..40 para sumas)."""
    if texto is None or texto == "":
        return set(range(0, 41))
    return pw.rango(texto)


# =================================================================================================== Columnas probables
def cp_de_texto(t):
    """Lo que admite la casilla «Pronóstico» de la app: un carácter por partido; los que faltan no entran en la CP."""
    return [t[i] if i < len(t) and t[i] in SIGNOS else "" for i in range(14)]


def cp_valores(c, cp):
    """Manual §7: una CP es un segundo pronóstico sobre un grupo de partidos. Por cada partido con pronóstico en la CP:
    acierto si el signo de la columna está entre los de la CP. Devuelve (aciertos, máx. aciertos seguidos, máx. fallos
    seguidos). Los partidos sin pronóstico en la CP no cuentan ni cortan rachas (motor: ColumnaProbable.AnalizaColumna)."""
    ac = acs = fs = ra = rf = 0
    for s, p in zip(c, cp):
        if not p:
            continue
        if s in p:
            ac += 1; ra += 1; rf = 0
        else:
            rf += 1; ra = 0
        acs = max(acs, ra); fs = max(fs, rf)
    return ac, acs, fs


def o_cp(cols, cps, rel1=()):
    """cps = [(cp, aciertos, ac_seguidos, fallos_seguidos)] con textos de rango ('' = cualquiera).
    rel1 = [(índices 1-based de CPs, rango de la suma de aciertos)] (pestaña Relaciones I, «Suma Aciertos»)."""
    sal = []
    for c in cols:
        v = [cp_valores(c, cp) for cp, *_ in cps]
        if not all(a in conj(ra) and s in conj(rs) and f in conj(rf) for (a, s, f), (_, ra, rs, rf) in zip(v, cps)):
            continue
        if all(sum(v[i - 1][0] for i in idx) in conj(rs_) for idx, rs_ in rel1):
            sal.append(c)
    return sal


def _asserts_cp():
    # Manual §7, «los 4 grandes»: 4 triples (81) y una CP con los 4 ganando en casa.
    u4 = pw.desarrollar(["1X2"] * 4 + ["1"] * 10)
    assert len(u4) == 81
    cp_unos = ["1"] * 4 + [""] * 10
    cp_doses = ["2"] * 4 + [""] * 10
    assert len(o_cp(u4, [(cp_unos, "3,4", "", "")])) == 9
    assert len(o_cp(u4, [(cp_unos, "2,3,4", "", "")])) == 33
    assert len(o_cp(u4, [(cp_unos, "2,3,4", "", ""), (cp_doses, "0,1", "", "")])) == 27
    # Manual §7, restricción progresiva: 6 triples + 6 dobles + 2 fijos = 46.656.
    u = pw.desarrollar(["1X2"] * 6 + ["1X"] * 6 + ["1"] * 2)
    assert len(u) == 46656
    cp1 = ["1X"] * 6 + [""] * 8              # los 6 triples pasan a dobles
    cp2 = [""] * 6 + ["1"] * 6 + [""] * 2    # los 6 dobles pasan a fijos
    assert len(o_cp(u, [(cp1, "3,4,5", "", "")])) == 37888
    assert len(o_cp(u, [(cp1, "3,4,5", "", ""), (cp2, "3,4,5", "", "")])) == 24272
    assert len(o_cp(u, [(cp1, "4,5,6", "", ""), (cp2, "4,5,6", "", "")])) == 10912
    dos36 = [(cp1, "3,4,5,6", "", ""), (cp2, "3,4,5,6", "", "")]
    assert len(o_cp(u, dos36)) == 27552
    assert len(o_cp(u, dos36, rel1=[((1, 2), "7-11")])) == 24288      # «la suma entre 7 y 11»
    # Manual §6 (Signos Seguidos) no aplica; ejemplo propio de rachas: CP 1111 sobre 1X11 ⇒ 3 aciertos, 2 seguidos, 1 fallo.
    assert cp_valores("1X11" + "1" * 10, cp_unos) == (3, 2, 1)


CP_A = "112X11X21121X1"        # 14 signos simples (lo único que admite la casilla «Pronóstico»)
CP_B = "2X1X1XX1"              # 8 signos: la CP solo abarca P1–P8
FC1_COND = dict(aciertos="10,11,12", ac_seg="5,6,7,8", fallos_seg="0,1")


# =================================================================================================== Grupos de equipos
def ge_valores(c, sel):
    """Manual §6 «Grupos de Equipos»: un conjunto de equipos; victorias, empates, derrotas y puntos (3-1-0).
    sel = {partido 1-based: 'C' (casa), 'F' (fuera) o 'A' (los dos)}. Con los dos equipos de un partido en el grupo,
    cuentan los dos (motor GrupoEquipos.AnalizaColumna: gana uno ⇒ 1 V + 1 D; empate ⇒ 2 E y 2 puntos)."""
    v = e = d = p = 0
    for k, t in sel.items():
        s = c[k - 1]
        for lado in {"C": "C", "F": "F", "A": "CF"}[t]:
            if s == "X":
                e += 1; p += 1
            elif (s == "1") == (lado == "C"):
                v += 1; p += 3
            else:
                d += 1
    return v, e, d, p


def o_ge(cols, grupos, relaciones=()):
    """grupos = [(sel, V, E, D, P)] con rangos texto ('' = sin límite). relaciones = [(índices de grupo, V, E, D, P)]
    sobre la SUMA de esos grupos."""
    sal = []
    for c in cols:
        vals = [ge_valores(c, g[0]) for g in grupos]
        ok = all(all(x in conj(r) for x, r in zip(val, g[1:])) for val, g in zip(vals, grupos))
        if ok:
            for idx, *rs in relaciones:
                suma = [sum(vals[i - 1][j] for i in idx) for j in range(4)]
                ok = ok and all(x in conj(r) for x, r in zip(suma, rs))
        if ok:
            sal.append(c)
    return sal


def _asserts_ge():
    # Manual §6: grupo de 5 equipos, Victorias 3-5, Empates 0-1, Derrotas 0-2, Puntos 10-15. «3 victorias, 0 empates
    # y 2 derrotas cumple las tres primeras casillas, pero suma 9 puntos y queda fuera».
    sel = {i: "C" for i in range(1, 6)}
    lim = (sel, "3-5", "0-1", "0-2", "10-15")
    col_9 = "11122" + "1" * 9
    assert ge_valores(col_9, sel) == (3, 0, 2, 9)
    assert o_ge([col_9], [lim]) == []
    col_10 = "1112X" + "1" * 9           # 3 V, 1 E, 1 D = 10 puntos: entra
    assert ge_valores(col_10, sel) == (3, 1, 1, 10) and o_ge([col_10], [lim]) == [col_10]
    # Los dos equipos de un partido: gana el local ⇒ 1 V y 1 D (3 puntos); empate ⇒ 2 E (2 puntos).
    assert ge_valores("1" + "1" * 13, {1: "A"}) == (1, 0, 1, 3)
    assert ge_valores("X" + "1" * 13, {1: "A"}) == (0, 2, 0, 2)


GE_G1 = {1: "C", 4: "C", 6: "C", 8: "A", 11: "F"}     # P1 casa, P4 casa, P6 casa, P8 los dos, P11 fuera
GE_G2 = {3: "C", 9: "F", 13: "C"}                      # P3 casa, P9 fuera, P13 casa


# =================================================================================================== If-Then
def cuenta(signo):
    return lambda c: c.count(signo)


def o_ifthen(cols, relaciones, rango_ok):
    """Manual §6: «si se cumple A, entonces exige B». Una relación se cumple si no se da A o si se da B (motor
    CondicionIfThen.CompruebaPronostico). «Condiciones que deben cumplirse» = cuántas relaciones han de cumplirse,
    número exacto o lista (motor ControladorIfThen: AciertoCondiciones[nº cumplidas])."""
    ok = pw.rango(rango_ok)
    return [c for c in cols if sum((not si(c)) or entonces(c) for si, entonces in relaciones) in ok]


def rel(si_signo, si_valor, ent_signo, ent_valor):
    """«Cantidad de <si_signo>» = si_valor ⇒ «Cantidad de <ent_signo>» = ent_valor (el Valor es un número exacto)."""
    return (lambda c: c.count(si_signo) == si_valor, lambda c: c.count(ent_signo) == ent_valor)


def _asserts_ifthen():
    # Sin manual con números: comprobaciones de la lógica. Si A no se da, la relación se cumple.
    r = rel("X", 4, "2", 2)
    assert o_ifthen(["1" * 14], [r], "1") == ["1" * 14]
    assert o_ifthen(["XXXX22" + "1" * 8], [r], "1") == ["XXXX22" + "1" * 8]
    assert o_ifthen(["XXXX2" + "1" * 9], [r], "1") == []
    assert o_ifthen(["XXXX2" + "1" * 9], [r], "0") == ["XXXX2" + "1" * 9]


IT_R1 = rel("X", 4, "2", 2)          # FC5 y FC8
IT_R2 = rel("X", 4, "2", 3)          # FC6/FC7, relación 1
IT_R3 = rel("X", 3, "2", 4)          # FC6/FC7, relación 2
VAR_FC8 = "7,8,9"


# =================================================================================================== Pares y Tríos
ORDEN_BASE3 = {"X": 0, "1": 1, "2": 2}


def orden_base3(cols):
    """Pares y Tríos graban las válidas sin repetir y en orden X < 1 < 2 (ParejasFrm/TriosFrm.n1s)."""
    return sorted(set(cols), key=lambda c: [ORDEN_BASE3[s] for s in c])


PARES_COMB = ["11", "1X", "12", "X1", "XX", "X2", "21", "2X", "22"]       # orden de las casillas de la app
TRIOS_COMB = ["".join(t) for t in itertools.product("1X2", repeat=3)]   # 111, 11X, 112, 1X1 … 222 (orden de la app)
assert TRIOS_COMB[:4] == ["111", "11X", "112", "1X1"] and TRIOS_COMB[-1] == "222"


def niveles(c, filas):
    """filas = [(posiciones 1-based, {combinación: nivel})]. Cada fila mira los signos de esos partidos y suma 1 al
    nivel que tenga asignada esa combinación (0 = ninguno). Sin manual: regla de ParejasFrm/TriosFrm.Valida."""
    n = {}
    for pos, mapa in filas:
        lv = mapa.get("".join(c[p - 1] for p in pos), 0)
        if lv:
            n[lv] = n.get(lv, 0) + 1
    return n


def o_niveles(cols, filas, limites):
    """limites = {nivel: (mín, máx)}; los niveles sin límite valen siempre."""
    ok = []
    for c in cols:
        c = c.strip().upper()
        if len(c) != 14:
            continue
        n = niveles(c, filas)
        if all(a <= n.get(lv, 0) <= b for lv, (a, b) in limites.items()):
            ok.append(c)
    return orden_base3(ok)


def nivel_igualdad(t):
    return {1: 1, 2: 2, 3: 3}[len(set(t))]     # 3 iguales ⇒ 1 · 2 iguales ⇒ 2 · 3 distintos ⇒ 3


PAR_FILAS = [
    ((1, 4), {k: (1 if k[0] == k[1] else 2) for k in PARES_COMB}),
    ((8, 1), {k: (1 if k[0] == k[1] else 2) for k in PARES_COMB}),
    ((3, 9), {"X2": 3, "21": 3}),
]
PAR_LIMITES = {1: (1, 1), 2: (0, 10), 3: (1, 1), 4: (0, 10), 5: (0, 10), 6: (0, 10), 7: (0, 10)}
TRIO_FILAS = [
    ((1, 4, 8), {k: nivel_igualdad(k) for k in TRIOS_COMB}),
    ((3, 6, 9), {k: (4 if "2" in k else 5 if "X" in k else 0) for k in TRIOS_COMB}),
]
TRIO_LIMITES = {1: (0, 270), 2: (0, 270), 3: (0, 0), 4: (1, 1), 5: (0, 270)}


def fichero_par():
    lineas = []
    for pos, mapa in PAR_FILAS:
        lineas.append(",".join([str(pos[0]), str(pos[1])] + [str(mapa.get(k, 0)) for k in PARES_COMB]))
    lineas += ["0," * 10 + "0"] * (14 - len(PAR_FILAS))
    lineas += [f"{PAR_LIMITES[n][0]},{PAR_LIMITES[n][1]}" for n in range(1, 8)]
    return lineas


def fichero_tri():
    lineas = []
    for pos, mapa in TRIO_FILAS:
        lineas.append(",".join([str(p) for p in pos] + [str(mapa.get(k, 0)) for k in TRIOS_COMB]))
    lineas += [",".join(["0"] * 30)] * (12 - len(TRIO_FILAS))
    lineas += [f"{TRIO_LIMITES[n][0]},{TRIO_LIMITES[n][1]}" for n in range(1, 6)]
    return lineas


def _asserts_niveles():
    assert len(fichero_par()) == 21 and all(len(l.split(",")) == 11 for l in fichero_par()[:14])
    assert len(fichero_tri()) == 17 and all(len(l.split(",")) == 30 for l in fichero_tri()[:12])
    assert orden_base3(["1" * 14, "X" * 14, "2" * 14]) == ["X" * 14, "1" * 14, "2" * 14]
    # «Analizar» (pantalla) con la columna 111X11X222X111: Pares 0,2,0,0,0,0,0 · Tríos 0,0,1,1,0.
    g = "111X11X222X111"
    assert [niveles(g, PAR_FILAS).get(n, 0) for n in range(1, 8)] == [0, 2, 0, 0, 0, 0, 0]
    assert [niveles(g, TRIO_FILAS).get(n, 0) for n in range(1, 6)] == [0, 0, 1, 1, 0]


# =================================================================================================== Dependencia lineal
VAL = {"1": 0, "X": 1, "2": 2}
SIG = "1X2"


def o_dl(cols, tratar, signos, coef):
    """Manual §10: el signo del partido a tratar pasa a ser una combinación lineal (módulo 3 ó 2) de los signos de los
    demás, ponderados por sus coeficientes. El manual no da la codificación; se sigue el WinForms original
    (FrmDependenciaLineal.btAceptar_Click/TestSignosPartidoATratar): 1=0, X=1, 2=2; triple ⇒ suma mód 3; 1X ⇒ mód 2
    (1/X); X2 ⇒ mód 2 + 1 (X/2); 12 ⇒ (mód 2)·2 (1/2). Salida sin repetidas, ordenada por el número en base 3 con el
    partido 1 como cifra de menor peso."""
    modulo, corr, indep = {"1X2": (3, 1, 0), "1X": (2, 1, 0), "X2": (2, 1, 1), "12": (2, 2, 0)}[signos]
    sal = set()
    for c in cols:
        c = c.strip().upper()
        v = [VAL.get(s, 1) for s in c]
        n = sum(coef.get(p + 1, 0) * v[p] for p in range(14) if p + 1 != tratar)
        v[tratar - 1] = (n % modulo) * corr + indep
        sal.add(tuple(v))
    return ["".join(SIG[x] for x in v) for v in sorted(sal, key=lambda v: sum(x * 3 ** i for i, x in enumerate(v)))]


DL_COEF = {1: 1, 3: 1, 4: 2, 8: 1, 9: 1}       # P1 (propuesto, ignorado en FC11), P3, P4, P8, P9


def _asserts_dl():
    # Triple: P1 = (P3 + 2·P4 + P8 + P9) mód 3.
    assert o_dl(["1" * 14], 1, "1X2", DL_COEF) == ["1" * 14]
    assert o_dl(["1" * 3 + "X" + "1" * 10], 1, "1X2", DL_COEF) == ["2" + "11X" + "1" * 10]
    # 12: nunca sale X; X2: nunca sale 1.
    assert all(c[5] in "12" for c in o_dl(B3, 6, "12", DL_COEF))
    assert all(c[5] in "X2" for c in o_dl(B3, 6, "X2", DL_COEF))


# =================================================================================================== entradas y esperados
ENTRADAS = {
    "FC_B3.txt": lambda: list(B3),          # entrada de Pares, Tríos y Dependencia lineal (216 columnas)
    "FC_pares.par": fichero_par,            # «Leer condiciones» de Pares
    "FC_trios.tri": fichero_tri,            # «Abrir Condiciones» de Tríos
}


def esperados():
    r = {}
    r["FC1"] = ("Columnas probables: B3 + 1 CP «112X11X21121X1», Aciertos 10,11,12 · Ac. Seguidos 5,6,7,8 · "
                "Fallos Seguidos 0,1",
                o_cp(B3, [(cp_de_texto(CP_A), FC1_COND["aciertos"], FC1_COND["ac_seg"], FC1_COND["fallos_seg"])]),
                None)
    r["FC2"] = ("Columnas probables: B3 + CP1 «112X11X21121X1» y CP2 «2X1X1XX1» (P1–P8) sin rangos + Relaciones I: "
                "Columnas 1-2, Suma Aciertos 13-14",
                o_cp(B3, [(cp_de_texto(CP_A), "", "", ""), (cp_de_texto(CP_B), "", "", "")],
                     rel1=[((1, 2), "13-14")]), None)
    triples = o_cp(B3, [(["1X2"] * 14, "14", "", "")])
    rota = o_cp(B3, [(cp_de_texto("1X21X21X21X21X"), "14", "", "")])
    r["FC13"] = ("Columnas probables: «Todos a Triple» + Aciertos 14 (sonda): una CP de 14 triples la acierta toda "
                 f"columna ⇒ {len(triples)}. El código (ColProbablesFrmViewModel.cs:345-351 y 429-435) la convierte al "
                 f"Aceptar en «1X21X21X21X21X» ⇒ predice {len(rota)}", triples, None)
    r["FC3"] = ("Grupos de equipos: B3 + grupo 1 (P1 casa, P4 casa, P6 casa, P8 los dos, P11 fuera) "
                "Victorias 2-3 · Empates 1-2 · Suma Puntos 8-12",
                o_ge(B3, [(GE_G1, "2-3", "1-2", "", "8-12")]), None)
    r["FC4"] = ("Grupos de equipos: grupo 1 (los de FC3) Victorias 2-4 · grupo 2 (P3 casa, P9 fuera, P13 casa) "
                "Derrotas 0-1 · Relación «1-2» Suma Puntos 12-15",
                o_ge(B3, [(GE_G1, "2-4", "", "", ""), (GE_G2, "", "", "0-1", "")], [((1, 2), "", "", "", "12-15")]),
                None)
    r["FC5"] = ("If-Then: B3 + «Si Cantidad de X = 4 ⇒ Cantidad de 2 = 2», deben cumplirse 1",
                o_ifthen(B3, [IT_R1], "1"), None)
    r["FC6"] = ("If-Then: «X=4 ⇒ 2=3» y «X=3 ⇒ 2=4», deben cumplirse 2", o_ifthen(B3, [IT_R2, IT_R3], "2"), None)
    r["FC7"] = ("If-Then: las mismas dos relaciones, deben cumplirse 1 (exactamente una)",
                o_ifthen(B3, [IT_R2, IT_R3], "1"), None)
    fc8 = [c for c in o_ifthen(B3, [IT_R1], "1") if pw.cumple(c, pw.P_B3, var=VAR_FC8)]
    solo_var = [c for c in B3 if pw.cumple(c, pw.P_B3, var=VAR_FC8)]
    r["FC8"] = (f"If-Then de FC5 + «Variantes X y 2» Var {VAR_FC8} (las dos a la vez; solo Var darían "
                f"{len(solo_var)}). El código (Analizador.cs:97-102, igual en el original) predice {len(r['FC5'][1])}: "
                "el If-Then sustituye al resultado de las demás condiciones", fc8, None)
    r["FC9"] = ("Pares: FC_pares.par sobre FC_B3.txt ▸ Grabar resultado",
                o_niveles(B3, PAR_FILAS, PAR_LIMITES), None)
    r["FC10"] = ("Tríos: FC_trios.tri sobre FC_B3.txt ▸ Grabar Resultado",
                 o_niveles(B3, TRIO_FILAS, TRIO_LIMITES), None)
    r["FC11"] = ("Dependencia lineal: FC_B3.txt, partido a tratar P1 (1X2), coeficientes P3=1 P4=2 P8=1 P9=1",
                 o_dl(B3, 1, "1X2", DL_COEF), None)
    alt = o_dl(r["FC11"][1], 6, "12", DL_COEF)
    r["FC12"] = ("Dependencia lineal: SIN volver a elegir la entrada, partido a tratar P6 (1 y 2), mismos coeficientes "
                 f"(como el original, entrada = FC_B3.txt). Si salen {len(alt)}, la app ha tomado FC11.txt como entrada",
                 o_dl(B3, 6, "12", DL_COEF), None)
    return r


def preparar(carpeta):
    e = Path(carpeta) / "entrada"
    for nombre, f in ENTRADAS.items():
        pw.escribir(e / nombre, f())
    (Path(carpeta) / "salida").mkdir(parents=True, exist_ok=True)
    print(f"{len(ENTRADAS)} ficheros en {e}")


def verificar(carpeta):
    sal = Path(carpeta) / "salida"
    ok = mal = falta = 0
    for clave, (desc, cols, _) in esperados().items():
        f = sal / (clave + ".txt")
        if not f.exists():
            print(f"·   {clave:5} sin salida            {desc}"); falta += 1; continue
        real = pw.leer(f)
        bien = pw.multiconjunto(real) == pw.multiconjunto(cols)
        det = f"{len(real)} columnas" + ("" if bien else f", se esperaban {len(cols)}")
        if not bien:
            sobran, faltan = sorted(set(real) - set(cols))[:3], sorted(set(cols) - set(real))[:3]
            det += (f"; sobran {sobran}" if sobran else "") + (f"; faltan {faltan}" if faltan else "")
        elif real != cols:
            det += " (mismo contenido, otro orden)"
        print(f"{'OK' if bien else 'MAL':3} {clave:5} {det:30} {desc}")
        ok += bien; mal += not bien
    print(f"\n{ok} bien, {mal} mal, {falta} sin salida")
    return mal == 0


def _asserts():
    _asserts_cp(); _asserts_ge(); _asserts_ifthen(); _asserts_niveles(); _asserts_dl()
    r = esperados()
    for clave, (_, cols, _) in r.items():
        if clave != "FC13":                                       # FC13 es una sonda: lo correcto es «todas»
            assert 0 < len(cols) < 216, (clave, len(cols))        # ni 0 ni todo: la prueba discrimina
        assert len(set(cols)) == len(cols), clave
    for clave in ("FC1", "FC2", "FC3", "FC4", "FC5", "FC6", "FC7", "FC8", "FC9", "FC10"):
        assert set(r[clave][1]) <= set(B3), clave                 # filtros: solo columnas del boleto


if __name__ == "__main__":
    sys.stdout.reconfigure(encoding="utf-8")
    _asserts()
    orden = sys.argv[1] if len(sys.argv) > 1 else ""
    if orden == "preparar":
        preparar(sys.argv[2])
    elif orden == "verificar":
        sys.exit(0 if verificar(sys.argv[2]) else 1)
    else:
        print("asserts OK (manual §6 Grupos de Equipos, §7 Columnas Probables; lógica If-Then, Pares, Tríos, Dep. lineal)")
        for clave, (desc, cols, _) in esperados().items():
            print(f"{clave:5} {len(cols):4} col.  {desc}")
