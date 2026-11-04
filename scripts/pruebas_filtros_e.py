"""Oráculo del grupo E de filtros: Rentabilidad, CP parcial, Control de fallos entre grupos, Figuras de Signos
Seguidos, Simetrías en espejo y Variantes con límite solo de doses.

    python -I scripts/pruebas_filtros_e.py                      # asserts + clave y nº de columnas esperadas
    python -I scripts/pruebas_filtros_e.py preparar <carpeta>   # escribe <carpeta>/entrada/FE_*  (con pw.escribir)
    python -I scripts/pruebas_filtros_e.py verificar <carpeta>  # compara <carpeta>/salida/FE*.txt con lo esperado

Independiente de la app: cada regla se programa aquí con cadenas y posiciones (definición del manual y, donde el manual
no llega, del motor C#, que se cita en docs/pruebas/filtros/plan_e.md). Además, `emulacion_motor()` reprograma la
lógica del motor (bits, máscaras y, en Rentabilidad, la aritmética float32 del recorrido EncontrarDistantes1) y
comprueba que da lo mismo que la regla «de manual» en las 216 columnas de B3 (y en 3.000 al azar donde aplica).

Formato de salida: {clave: (descripción, columnas, None)}; columnas de 14 caracteres «1X2», como las graba la app.
"""
import random
import struct
import sys
from collections import Counter
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import pruebas_windows as pw  # noqa: E402

B3 = pw.desarrollar(pw.P_B3)                       # 216 columnas (3 triples, 3 dobles, 8 fijos)
TODOS14 = "0,1,2,3,4,5,6,7,8,9,10,11,12,13,14"     # «todos» escrito a mano en Variantes (= ObtenerTodosValores)


def lista_comas(texto, todos=range(15)):
    """Variantes / Signos seguidos / Interrupciones: solo números con comas; vacío = todos (el VM rellena con
    ObtenerTodosValores, 0..14)."""
    if not texto.strip():
        return set(todos)
    return {int(p) for p in texto.split(",")}


def lista_rangos(texto):
    """Campos con rangos: Aciertos de Simetrías y de CP (tras ec6683e), Fallos de Control de grupos."""
    r = set()
    for p in texto.split(","):
        a, _, b = p.strip().partition("-")
        r.update(range(int(a), int(b or a) + 1))
    return r


# =========================================================================================== FE1 Rentabilidad
# Porcentajes (cada fila: partido 1..14, % del 1, de la X y del 2). Se pegan con «Pegar desde Excel».
AP = [(60, 25, 15), (50, 30, 20), (70, 20, 10), (45, 30, 25)]
RE = [(50, 30, 20), (40, 30, 30), (55, 25, 20), (35, 35, 30)]
APOSTADOS = [AP[i % 4] for i in range(14)]
REALES = [RE[(3 * i + 1) % 4] for i in range(14)]
PRECIO, PCT14 = 0.75, 12.0                       # Configuración ▸ Parámetros LAE de fábrica (parametros.free1x2:10)
REC_FE1 = 14000000.0                             # se escribe a mano en «Recaudación considerada»
REC_DEFECTO = 8000000.0                          # lo que trae la pantalla (RentabilidadFrmViewModel.cs:63), hallazgo R-1
EM_MIN, EM_MAX = 1.0, 2.0


def em_manual(c, rec=REC_FE1, ap=APOSTADOS, re_=REALES, precio=PRECIO, pct=PCT14):
    """Manual «Rentabilidad» (MANUAL_USUARIO.md:552): EM = Premio × Probabilidad. Premio de 14 = recaudación × %14 ÷
    acertantes, con acertantes = prob. apostada × recaudación ÷ precio (:540-542) = precio × %14 ÷ prob. apostada, sin
    pasar del bote del 14 (recaudación × %14; RentabilidadFrmViewModel.cs:357-358). En doble precisión."""
    pa = pr = 1.0
    for i, s in enumerate(c):
        k = "1X2".index(s)
        pa *= ap[i][k] / sum(ap[i])
        pr *= re_[i][k] / sum(re_[i])
    premio = precio * pct / 100 / pa
    tope = rec * pct / 100
    return min(premio, tope) * pr, premio, tope


def f32(x):
    return struct.unpack("f", struct.pack("f", x))[0]


def base100_f32(filas):
    """Porcentajes.ValoresBase100 (Porcentajes.cs:341-352): factor = (float)(a+b+c); valor = (float)(v / factor)."""
    out = []
    for fila in filas:
        factor = f32(float(sum(fila)))
        out.append([f32(v / factor) for v in fila])
    return out


def em_motor_f32(c, rec=REC_FE1, ap=APOSTADOS, re_=REALES, precio=PRECIO, pct=PCT14):
    """Emulación literal de RentabilidadFrmViewModel.Calcular + EncontrarDistantes1 (líneas 217-293 y 340-371): todo en
    float32. Prob. de la 11111111111111 (producto en orden de partido) y, para cada partido que no es 1 (en orden
    creciente, que es el camino de la recursión), se multiplica por Cra = pa[X o 2] / pa[1] (y Crp para la real)."""
    pa, pr = base100_f32(ap), base100_f32(re_)
    premio14 = f32(f32(f32(precio) * f32(pct)) / 100)
    tope = f32(f32(f32(rec) * f32(pct)) / 100)
    pA = pR = f32(1.0)
    for i in range(14):
        pA = f32(pA * pa[i][0])
        pR = f32(pR * pr[i][0])
    for i, s in enumerate(c):
        if s == "1":
            continue
        z = 1 if s == "X" else 2
        pA = f32(pA * f32(pa[i][z] / pa[i][0]))
        pR = f32(pR * f32(pr[i][z] / pr[i][0]))
    premio = f32(premio14 / pA)
    if premio > tope:
        premio = tope
    return f32(premio * pR)


def acepta_rentabilidad(em, mn=EM_MIN, mx=EM_MAX):
    """GrabacionColumnas (RentabilidadFrmViewModel.cs:379-403): -EM >= -máx y -EM <= -mín, o sea mín <= EM <= máx."""
    return mn <= em <= mx


COL_VALORACION = "11211XX22121X1"


def valoracion_columna(c, rec=REC_FE1):
    """«Calcular valoración de una columna» (CalcularValoracionColumna, :158-195): el premio estimado que se ve NO lleva el
    tope (:191), la EM sí (:186-188)."""
    em, premio, tope = em_manual(c, rec)
    pr = em / min(premio, tope)
    return round(premio, 2), pr, em


# =========================================================================================== FE2 CP parcial
CP_FE2 = {1: "1", 3: "X", 4: "X2", 8: "1", 9: "2"}        # partido -> signos marcados; los otros 9 sin marcar
CP_AC = "3-5"
CP_FS = "0,1"


def cp_valores(c, cp=CP_FE2):
    """ColumnaProbable.AnalizaColumna (ColumnaProbable.cs:271-323): solo cuentan los partidos con signo en la CP;
    acierto si el signo de la columna está entre los marcados; rachas de aciertos y fallos sobre esos partidos, en
    orden (los partidos sin signo ni cuentan ni cortan la racha)."""
    ac = acs = fs = ra = rf = 0
    for p in sorted(cp):
        if c[p - 1] in cp[p]:
            ac, ra, rf = ac + 1, ra + 1, 0
        else:
            rf, ra = rf + 1, 0
        acs, fs = max(acs, ra), max(fs, rf)
    return ac, acs, fs


def cumple_cp(c, ac=CP_AC, acs="", fs=""):
    a, s, f = cp_valores(c)
    todos = set(range(15))
    return (a in (lista_rangos(ac) if ac else todos) and s in (lista_rangos(acs) if acs else todos)
            and f in (lista_rangos(fs) if fs else todos))


def cp_corta_rachas(c, cp=CP_FE2):
    """Variante FALSA (solo para la sensibilidad): un partido sin signo cortaría las rachas."""
    fs = rf = 0
    for p in range(1, 15):
        if p not in cp:
            rf = 0
        elif c[p - 1] not in cp[p]:
            rf += 1
        else:
            rf = 0
        fs = max(fs, rf)
    return fs


# =========================================================================================== FE3 Control de grupos
G1 = {"var": "4,5,6,7", "int": "9,10,11"}               # grupo 1: Variantes + Interrupciones (Global)
G2 = {"var": "6,7,8,9", "int": "8,9,10"}                # grupo 2
FALLOS_FE3, FALLOS_FE3b = "1", "0,1"


def variantes(c):
    return sum(s != "1" for s in c)


def cumple_grupo(c, g):
    """Un grupo con Variantes (manual §6) e Interrupciones globales (arnés pw.interrupciones, prueba C1)."""
    return variantes(c) in lista_comas(g["var"]) and pw.interrupciones(c)[0]["global"] in lista_comas(g["int"])


def cumple_control(c, grupos, fallos):
    """Manual §6 «Grupos y control de fallos» (MANUAL_USUARIO.md:353-354) y ControlGrupos.AnalizaGruposConControlFallos
    (ControlGrupos.cs:61-90): se cuentan los grupos controlados que NO se cumplen y ese número debe estar en «Fallos».
    El grupo 0 (boleto base, sin condiciones) queda como suelto en el control base y siempre se cumple."""
    n = sum(1 for g in grupos if not cumple_grupo(c, g))
    return n in lista_rangos(fallos)


# =========================================================================================== FE4 Signos seguidos
SS_FIGURAS_V = ("3-2-1-1-1", "2-2-1-1-1")             # Figuras Variantes
SS_UNOS = "1,2"                                       # campo «1»: racha máxima de unos


def figura(c, signos):
    """Manual §2 (MANUAL_USUARIO.md:86): «forma en que se agrupan consecutivamente las apariciones de un signo,
    ordenadas de mayor a menor grupo». En la app se escribe con «-» (ObtenerTextoFiguraFromLong); sin apariciones, «0»."""
    rachas, a = [], 0
    for s in c + "·":
        if s in signos:
            a += 1
        elif a:
            rachas.append(a)
            a = 0
    return "-".join(map(str, sorted(rachas, reverse=True))) or "0"


def cumple_ss(c, unos=SS_UNOS, figs_v=SS_FIGURAS_V):
    """FiltroSignosSeguidos.CumpleCondiciones (FiltroSignosSeguidos.cs:140-147): rachas en sus listas Y, si hay
    figuras, la figura de cada concepto con figuras en su lista (:351-395)."""
    ok = pw.racha(c, "1".__eq__) in lista_comas(unos)
    return ok and (not figs_v or figura(c, "X2") in figs_v)


# =========================================================================================== FE5 Simetrías espejo
ESPEJO = ["%d,%d" % (i, i + 7) for i in range(1, 8)]   # 1,8 · 2,9 · … · 7,14
ESPEJO_ACIERTOS = "0-3"


def aciertos_simetrias(c, filas):
    """Manual §6 (:342): una simetría acierta si todos sus partidos tienen el mismo signo (FiltroSimetrias.cs:64-89)."""
    grupos = [sorted(lista_rangos(f)) for f in filas]
    return sum(1 for g in grupos if len({c[p - 1] for p in g}) == 1)


# =========================================================================================== FE6 Variantes, solo doses
DOSES_FE6 = "1,2,3"


def cumple_variantes(c, var="", x="", dos=DOSES_FE6):
    """Manual §6: nº de variantes (X o 2), de equis y de doses en sus listas; campo vacío = todos
    (NoVariantesFrmViewModel.cs:94-107)."""
    return (variantes(c) in lista_comas(var) and c.count("X") in lista_comas(x)
            and c.count("2") in lista_comas(dos))


# =========================================================================================== entradas
def porcentajes_txt(filas):
    """Tabla para «Pegar desde Excel» (PorcentajesPegados.Leer toma los 3 últimos números de cada línea), como los
    val_*.txt del arnés."""
    return ["Part.\tPartido\t1\tX\t2"] + ["%d\tPartido %d\t%d\t%d\t%d" % ((i + 1, i + 1) + f) for i, f in enumerate(filas)]


ENTRADAS = {
    "FE_B3.txt": lambda: B3,
    "FE1_apostados.txt": lambda: porcentajes_txt(APOSTADOS),
    "FE1_reales.txt": lambda: porcentajes_txt(REALES),
}


# =========================================================================================== esperado
def esperados():
    r = {}
    r["FE1"] = ("Rentabilidad: Fichero FE_B3.txt, recaudación 14.000.000, EM mín 1 máx 2",
                [c for c in B3 if acepta_rentabilidad(em_manual(c)[0])], None)
    r["FE2"] = ("B3 + CP parcial (P1 1 · P3 X · P4 X2 · P8 1 · P9 2), Aciertos 3-5",
                [c for c in B3 if cumple_cp(c)], None)
    r["FE2b"] = ("FE2 + Fallos Seguidos 0,1", [c for c in B3 if cumple_cp(c, fs=CP_FS)], None)
    r["FE3"] = ("B3 + grupo 1 (Var 4,5,6,7 · Int 9,10,11) y grupo 2 (Var 6,7,8,9 · Int 8,9,10); Control 1,2 Fallos 1",
                [c for c in B3 if cumple_control(c, (G1, G2), FALLOS_FE3)], None)
    r["FE3b"] = ("FE3 con Fallos 0,1", [c for c in B3 if cumple_control(c, (G1, G2), FALLOS_FE3b)], None)
    r["FE3x"] = ("FE3 creando los grupos con «Grupo siguiente» (sonda; el código predice 0, hallazgo G-1)",
                 r["FE3"][1], None)
    r["FE4"] = ("B3 + Signos seguidos: «1» 1,2 + Figuras Variantes " + " y ".join(SS_FIGURAS_V),
                [c for c in B3 if cumple_ss(c)], None)
    r["FE5"] = ("B3 + Simetrías espejo 1,8 … 7,14; Aciertos 0-3",
                [c for c in B3 if aciertos_simetrias(c, ESPEJO) in lista_rangos(ESPEJO_ACIERTOS)], None)
    r["FE6"] = ("B3 + Variantes: Var y X vacíos, 2 = 1,2,3", [c for c in B3 if cumple_variantes(c)], None)
    r["FE6b"] = ("B3 + Variantes: Var = 0,…,14, X vacío, 2 = 1,2,3 (igual que FE6)",
                 [c for c in B3 if cumple_variantes(c, var=TODOS14)], None)
    return r


# =========================================================================================== comprobaciones
def ejemplos_manual():
    col = "11X211X1X12111"                                     # columna de ejemplo del manual §2
    assert figura(col, "1") == "3-2-2-1-1" and figura(col, "X") == "1-1-1"            # MANUAL_USUARIO.md:86
    assert figura(col, "2") == "1-1" and figura(col, "X2") == "2-1-1-1"
    assert pw.racha(col, "1".__eq__) == 3 and pw.racha(col, lambda s: s != "1") == 2   # :83
    col2 = "1XX11XXX22XX11"                                    # :298
    assert (pw.racha(col2, "1".__eq__), pw.racha(col2, "X".__eq__), pw.racha(col2, "2".__eq__),
            pw.racha(col2, lambda s: s != "1")) == (2, 3, 2, 7)
    assert figura("1" * 14, "X") == "0"
    # Simetrías en espejo (a mano): 11X211X / 1X12111 -> (1,8) 1=1 sí · (2,9) 1≠X · (3,10) X≠1 · (4,11) 2=2 sí ·
    # (5,12) 1=1 sí · (6,13) 1=1 sí · (7,14) X≠1 -> 4
    assert aciertos_simetrias(col, ESPEJO) == 4
    # CP parcial (a mano) sobre 1XX2111X21111X2…: P1 1 ok, P3 X ok, P4 2 ok (X2), P8 X falla, P9 2 ok -> 4 aciertos,
    # racha de aciertos 3 (P1-P3-P4, los partidos sin signo no la cortan), fallos seguidos 1
    assert cp_valores("1XX2111X211111") == (4, 3, 1)
    # Manual, Configuración L.A.E. (:540-542): 13.000.000 €, 0,5 €, 15 % y 1,5 acertantes -> premio 1.300.000 €, prob. 5,77·10⁻⁸
    p = 1.5 / (13e6 / 0.5)
    assert abs(p - 5.77e-8) < 1e-10 and abs(0.5 * 15 / 100 / p - 1.3e6) < 1
    # Rentabilidad: con apostados = reales la EM es precio × %14 / 100 (0,09 €) mientras no haya tope
    em, premio, tope = em_manual("1" * 14, re_=APOSTADOS)
    assert premio < tope and abs(em - 0.09) < 1e-12
    assert abs(em_motor_f32("1" * 14, re_=APOSTADOS) - 0.09) < 1e-6
    assert lista_rangos("3-5") == {3, 4, 5} and lista_comas("") == set(range(15))


COD = {"1": 4, "X": 2, "2": 1}                     # UtilColumnas: partido 1 en los 3 bits bajos


def a_long(c):
    return sum(COD[s] << (3 * i) for i, s in enumerate(c))


def bits(x):
    return bin(x).count("1")


def figura_cs(col, mascara):                       # FiltroSignosSeguidos.ObtenFiguraConMascara (:228-274)
    col &= mascara
    vals, n = [], 0
    while col:
        if col & 7:
            n += 1
        elif n:
            vals.append(n)
            n = 0
        col >>= 3
    if n:
        vals.append(n)
    fig = 0
    for v in sorted(vals, reverse=True):
        fig = fig << 4 | v
    return fig


def figura_desde_texto(t):                         # UtilidadesEntradasValores.ObtenerLongFiguraFromText (:220-231)
    fig = 0
    for v in t.split("-"):
        fig = fig << 4 | int(v)
    return fig


def cp_cs(col, cp):                                # ColumnaProbable.AnalizaColumna (:271-323)
    pron = [0] * 14
    for p, s in cp.items():
        pron[p - 1] = sum(COD[x] for x in s)
    ac = acs = fs = ta = tf = 0
    for i in range(14):
        if pron[i]:
            if pron[i] & (col & 7):
                ac, ta, tf = ac + 1, ta + 1, 0
            else:
                tf, ta = tf + 1, 0
            acs, fs = max(acs, ta), max(fs, tf)
        col >>= 3
    return ac, acs, fs


def noVariantes_cs(col):                           # FiltroNoVariantes.AnalizaColumna(long) (:61-66 sin blancos)
    x, d = bits(col & 80421421917330), bits(col & 5026338869833)
    return x + d, x, d


def interrupciones_cs(col):                        # FiltroInterrupciones, solo el global (como en pruebas_filtros_a)
    n = ant = 0
    while col:
        while ant == 0:
            ant = col & 7
            col >>= 3
        s = col & 7
        if s == 0:
            col >>= 3
            continue
        if s != ant:
            n += 1
        ant = s
        col >>= 3
    return n


def grupo_cs(col, g, mascara):
    """Grupo.AnalizaColumna(long) (Grupo.cs:165-215): si no están todos los partidos activos, la columna se filtra con
    la máscara del grupo (:171-173). Con todos activos (CrearGrupos) la máscara no se aplica."""
    if mascara is not None:
        col &= mascara
    v, _, _ = noVariantes_cs(col)
    if v not in lista_comas(g["var"]):
        return False
    return interrupciones_cs(col) in lista_comas(g["int"])


def control_cs(col, grupos, fallos, mascara=None):
    """ControladorGrupos.AnalizaColumnaSinConjuntos (:84-95): control base (grupo 0, sin control de fallos) Y control 1
    (grupos 1,2 con fallos): fallosPermitidos[nº de grupos que fallan] (ControlGrupos.cs:61-90, 199-225)."""
    permitidos = [False] * (len(grupos) + 1)
    for f in lista_rangos(fallos):
        permitidos[f] = True
    n = sum(1 for g in grupos if not grupo_cs(col, g, mascara))
    return permitidos[n]


def emulacion_motor():
    rnd = random.Random(2026)
    muestra = B3 + ["".join(rnd.choice("1X2") for _ in range(14)) for _ in range(3000)]
    mV = 120632132875995                            # FiltroSignosSeguidos.ObtenFiguraV (:276-280)
    figs = {figura_desde_texto(t) for t in SS_FIGURAS_V}
    for c in muestra:
        L = a_long(c)
        assert figura_cs(L, mV) == (figura_desde_texto(figura(c, "X2")) if figura(c, "X2") != "0" else 0), c
        assert (figura_cs(L, mV) in figs) == (figura(c, "X2") in SS_FIGURAS_V), c
        assert cp_cs(L, CP_FE2) == cp_valores(c), c
        assert noVariantes_cs(L) == (variantes(c), c.count("X"), c.count("2")), c
        assert interrupciones_cs(L) == pw.interrupciones(c)[0]["global"], c
        for fallos in (FALLOS_FE3, FALLOS_FE3b):
            assert control_cs(L, (G1, G2), fallos) == cumple_control(c, (G1, G2), fallos), c
    # Rentabilidad: la emulación float32 del motor clasifica igual que la fórmula en doble, y ninguna columna está
    # tan cerca de un límite (EM mín/máx o tope) que el redondeo pueda cambiarla de lado.
    for rec in (REC_FE1, REC_DEFECTO):
        for c in B3:
            em, premio, tope = em_manual(c, rec)
            em32 = em_motor_f32(c, rec)
            assert abs(em32 - em) / em < 1e-5, (c, em, em32)
            assert acepta_rentabilidad(em) == acepta_rentabilidad(em32), c
            assert min(abs(em - EM_MIN), abs(em - EM_MAX)) / em > 1e-3, c
            assert abs(premio - tope) / tope > 1e-3, c
    return len(muestra)


def sonda_grupos_sin_partidos():
    """FE3x por el código: el grupo creado con «Grupo siguiente» no tiene partidos activos, su máscara es 0
    (GrupoPartidos.AddGrupo -> CalcularMascara, GrupoPartidos.cs:37-39; Grupo.cs:65-90) y las condiciones ven la
    columna vacía (variantes 0, interrupciones 0): fallan los dos grupos."""
    return sum(1 for c in B3 if control_cs(a_long(c), (G1, G2), FALLOS_FE3, mascara=0))


def sensibilidad():
    def cuenta(f):
        return sum(1 for c in B3 if f(c))
    sal = [("FE1 con la recaudación por defecto 8.000.000", cuenta(lambda c: acepta_rentabilidad(em_manual(c, REC_DEFECTO)[0]))),
           ("FE1 sin tope (bote ilimitado)", cuenta(lambda c: acepta_rentabilidad(em_manual(c, 1e30)[0]))),
           ("FE1 EM mín 1, máx 3", cuenta(lambda c: acepta_rentabilidad(em_manual(c)[0], 1, 3))),
           ("FE1 columnas con tope", cuenta(lambda c: em_manual(c)[1] > em_manual(c)[2])),
           ("FE2 Aciertos 3,4 (sin el 5)", cuenta(lambda c: cumple_cp(c, ac="3,4"))),
           ("FE2b si sin signo cortara rachas", cuenta(lambda c: cumple_cp(c) and cp_corta_rachas(c) <= 1)),
           ("FE3 sin control (los 2 grupos)", cuenta(lambda c: cumple_grupo(c, G1) and cumple_grupo(c, G2))),
           ("FE3 solo grupo 1", cuenta(lambda c: cumple_grupo(c, G1))),
           ("FE3 solo grupo 2", cuenta(lambda c: cumple_grupo(c, G2))),
           ("FE3x por el código (sonda)", sonda_grupos_sin_partidos()),
           ("FE4 sin figuras", cuenta(lambda c: cumple_ss(c, figs_v=()))),
           ("FE4 solo figuras («1» vacío)", cuenta(lambda c: cumple_ss(c, unos=""))),
           ("FE4 figuras escritas al revés (1-1-1-2-3…)", cuenta(lambda c: pw.racha(c, "1".__eq__) in {1, 2} and figura_cs(
               a_long(c), 120632132875995) in {figura_desde_texto("-".join(reversed(t.split("-")))) for t in SS_FIGURAS_V})),
           ("FE5 Aciertos 0-2", cuenta(lambda c: aciertos_simetrias(c, ESPEJO) in {0, 1, 2})),
           ("FE6 2 = 1,2", cuenta(lambda c: cumple_variantes(c, dos="1,2")))]
    return sal


def reparto():
    return {"Variantes": sorted(Counter(variantes(c) for c in B3).items()),
            "Doses": sorted(Counter(c.count("2") for c in B3).items()),
            "Interrupciones": sorted(Counter(pw.interrupciones(c)[0]["global"] for c in B3).items()),
            "Espejo (aciertos)": sorted(Counter(aciertos_simetrias(c, ESPEJO) for c in B3).items()),
            "CP aciertos": sorted(Counter(cp_valores(c)[0] for c in B3).items())}


def main():
    sys.stdout.reconfigure(encoding="utf-8")
    ejemplos_manual()
    n = emulacion_motor()
    res = esperados()
    for clave, (desc, cols, _) in res.items():
        assert all(len(c) == 14 and set(c) <= set("1X2") for c in cols)
        assert 0 < len(cols) < 216 and len(set(cols)) == len(cols), clave
    assert res["FE6"][1] == res["FE6b"][1]
    assert set(res["FE2b"][1]) < set(res["FE2"][1]) and set(res["FE3"][1]) < set(res["FE3b"][1])
    assert sonda_grupos_sin_partidos() == 0
    print(f"asserts OK (ejemplos del manual + motor emulado en {n} columnas; Rentabilidad en float32)\n")
    for clave, (desc, cols, _) in res.items():
        print(f"{clave:5} {len(cols):4} col.  {desc}")
    print("\nSensibilidad (debe ser distinto del de la prueba):")
    for nombre, v in sensibilidad():
        print(f"  {nombre:40} {v}")
    print("\nReparto en B3:")
    for k, v in reparto().items():
        print(f"  {k:18} " + "  ".join(f"{a}→{b}" for a, b in v))
    c = COL_VALORACION                               # una columna de B3 con tope (premio estimado > bote)
    assert c in B3 and em_manual(c)[1] > em_manual(c)[2] and acepta_rentabilidad(em_manual(c)[0])
    premio, pr, em = valoracion_columna(c)
    print(f"\nValoración de una columna ({c}, recaudación 14.000.000): premio estimado ≈ {premio:,.2f}"
          f" (sin tope), probabilidad real ≈ {pr:.4e}, EM ≈ {em:.4f} (con tope {REC_FE1 * PCT14 / 100:,.0f})")


def preparar(carpeta):
    e = Path(carpeta) / "entrada"
    for nombre, f in ENTRADAS.items():
        pw.escribir(e / nombre, f())
    (Path(carpeta) / "salida").mkdir(parents=True, exist_ok=True)
    print(f"{len(ENTRADAS)} fichero(s) en {e}")


def verificar(carpeta):
    sal = Path(carpeta) / "salida"
    ok = mal = falta = 0
    for clave, (desc, cols, _) in esperados().items():
        f = sal / (clave + ".txt")
        if not f.exists():
            print(f"·   {clave:5} sin salida                {desc}")
            falta += 1
            continue
        real = pw.leer(f)
        bien = pw.multiconjunto(real) == pw.multiconjunto(cols)
        det = f"{len(real)} columnas" + ("" if bien else f", se esperaban {len(cols)}")
        if not bien:
            sobran, faltan = sorted(set(real) - set(cols))[:3], sorted(set(cols) - set(real))[:3]
            det += (f"; sobran {sobran}" if sobran else "") + (f"; faltan {faltan}" if faltan else "")
        print(f"{'OK' if bien else 'MAL':3} {clave:5} {det:30} {desc}")
        ok += bien
        mal += not bien
    print(f"\n{ok} bien, {mal} mal, {falta} sin salida")
    return mal == 0


if __name__ == "__main__":
    orden = sys.argv[1] if len(sys.argv) > 1 else ""
    if orden == "preparar":
        sys.stdout.reconfigure(encoding="utf-8")
        preparar(sys.argv[2])
    elif orden == "verificar":
        sys.stdout.reconfigure(encoding="utf-8")
        sys.exit(0 if verificar(sys.argv[2]) else 1)
    else:
        main()
