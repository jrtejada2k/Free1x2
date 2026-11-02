"""Oráculo del grupo B de pruebas de filtros (plan_b.md, en esta misma carpeta).

    python -I scripts/pruebas_filtros_b.py                     # asserts + clave y nº de líneas esperadas + números de pantalla
    python -I scripts/pruebas_filtros_b.py preparar <carpeta>  # escribe <carpeta>/entrada/FB_*.txt
    python -I scripts/pruebas_filtros_b.py verificar <carpeta> # compara <carpeta>/salida/FB*.txt con lo esperado

Pantallas: Formatos (FiltroFormatosSignos), Formatos 123 (FiltroFormatos123), Pesos Numéricos
(FiltroPesosNumericos), Valoración (FiltroValoracionSignos), Análisis de signos (VSignosFrm), Saca Formatos
(CalculoFormatosFrm) y Analizador Formatos123 (AnalisisFormatos123Frm, solo pantalla).

Independiente de la app: cada regla se reimplementa con cadenas de texto y fuerza bruta (no copia las operaciones de
bits del motor). La regla sale del manual (docs/MANUAL_USUARIO.md §6) y, donde el manual no llega, del código del
motor, que por R1 es el del WinForms original; cada función dice de dónde sale.
"""
import collections
import itertools
import sys
from decimal import Decimal, ROUND_HALF_UP
from pathlib import Path

sys.dont_write_bytecode = True                      # no dejar scripts/__pycache__ en el repo
sys.path.insert(0, str(Path(__file__).resolve().parent))
import pruebas_windows as pw  # noqa: E402

B3 = pw.desarrollar(pw.P_B3)                       # 216 columnas, en el orden del producto cartesiano
SG = "1X2"

# ---------------------------------------------------------------------------------------------- datos de prueba
# Valoración 1/X/2 por partido. Órdenes distintos por partido (para que un error de índice de partido se note) y dos
# empates a propósito (P7: 1 = 2; P9: 1 = X) que resuelve la regla de desempate del código (ver rango123).
VAL_MIXTA = [(50, 30, 20), (70, 20, 10), (20, 30, 50), (25, 45, 30), (40, 35, 25), (30, 45, 25), (30, 40, 30),
             (35, 25, 40), (33, 33, 34), (60, 25, 15), (15, 25, 60), (45, 30, 25), (20, 50, 30), (55, 30, 15)]

GANADORA_VS = "1X2" + "-" * 11                      # columna ganadora del Análisis de signos (- = comodín)
VS_COLS = ["1X21X21X21X21X", "1X111111111111", "112XXXXXXXXXXX", "XX222222222222", "2221X1X1X1X1X1",
           "2X121212121212", "X12X2X2X2X2X2X", "X2111111XXXXX2", "1X222222111112", "1xxx1x1x1x1x1x"]
AF_COLS = [B3[0], B3[100], B3[215]]                 # Analizador Formatos123: 3 columnas del boleto B3
CF_COL_1 = "11X211X1X12111"                         # ejemplo del manual §6
CF_COL_2 = "11XX2212X11111"                         # contiene los 9 pares posibles


def val_excel(val):
    """Formato de «Pegar desde Excel» (como pw.VALORACIONES): cabecera + «nº, Partido n, 1, X, 2»."""
    return ["Part.\tPartido\t1\tX\t2"] + [f"{i}\tPartido {i}\t{a}\t{b}\t{c}" for i, (a, b, c) in enumerate(val, 1)]


ENTRADAS = {
    "FB_val_mixta.txt": lambda: val_excel(VAL_MIXTA),
    "FB_vs_entrada.txt": lambda: list(VS_COLS),
    "FB_af123.txt": lambda: list(AF_COLS),
}


# ---------------------------------------------------------------------------------------------- Formatos (1, X, 2, V, *)
COMODIN = {"1": "1", "X": "X", "2": "2", "V": "X2", "*": "1X2"}


def apariciones(col, formato):
    """Veces que aparece el formato en la columna, leída de izquierda a derecha y con solapes: «V» = X o 2,
    «*» = cualquier signo (manual §6: en 11X211X1X12111 el formato 1X aparece 3 veces y 1X2 una)."""
    n = len(formato)
    return sum(1 for k in range(len(col) - n + 1) if all(col[k + j] in COMODIN[formato[j]] for j in range(n)))


def cumple_formatos(col, relaciones):
    """relaciones = [(líneas [(formato, rango)], «Líneas», «Global»)]; vale si vale en TODAS las relaciones.
    Sin Líneas ni Global, una relación vale si todas sus líneas están en su rango. Con Líneas y/o Global (el manual no
    lo detalla; regla del motor, FormatosSignos.cs:38-85): Líneas = cuántas líneas están en su rango, Global = suma de
    apariciones de todas las líneas; cada campo rellenado debe contener su número y las líneas sueltas ya no se exigen."""
    for lineas, L, G in relaciones:
        ok = [apariciones(col, f) in pw.rango(r) for f, r in lineas]
        if not L and not G:
            if not all(ok):
                return False
            continue
        if L and sum(ok) not in pw.rango(L):
            return False
        if G and sum(apariciones(col, f) for f, _ in lineas) not in pw.rango(G):
            return False
    return True


FMT_LINEAS = [("X1", "2,3"), ("11", "1-3"), ("X*1", "1,2")]
FB1 = [(FMT_LINEAS, "", "")]
FB2 = [(FMT_LINEAS, "1,2", "5-7")]


# ---------------------------------------------------------------------------------------------- Formatos 123
def rango123(v):
    """Posición de cada signo (1, X, 2) en la valoración del partido: 1 = más valorado, 3 = menos (texto de la pantalla
    Formatos 123). Empates (el manual no los trata; regla del código, Formatos123FrmViewModel.cs:347-400, igual que el
    original): el 1 gana a X y a 2 si es >=; la X gana al 2 si es >=."""
    a, x, d = v
    if a >= x and a >= d:
        return ("1", "2", "3") if x >= d else ("1", "3", "2")
    if x > a and x >= d:
        return ("2", "1", "3") if a >= d else ("3", "1", "2")
    return ("2", "3", "1") if a >= x else ("3", "2", "1")


def a123(col, val):
    return "".join(rango123(val[i])[SG.index(s)] for i, s in enumerate(col))


def ocurrencias(texto, sub):
    return sum(1 for k in range(len(texto) - len(sub) + 1) if texto[k:k + len(sub)] == sub)


def cumple_123(col, val, formatos, aciertos, ignorar_rep=False):
    """formatos = [(formato 123, mín, máx)]. «Líneas acertadas»: cuántos formatos aparecen entre su mín y su máx.
    «Ignorar repeticiones» (= «Aciertos globales»): cuántos formatos aparecen al menos una vez (mín/máx no cuentan).
    La columna vale si ese número está en la lista de aciertos (FiltroFormatos123.cs:115-181). Lista vacía: no vale
    ninguna."""
    t = a123(col, val)
    if ignorar_rep:
        n = sum(1 for f, _, _ in formatos if ocurrencias(t, f) > 0)
    else:
        n = sum(1 for f, mn, mx in formatos if mn <= ocurrencias(t, f) <= mx)
    return n in aciertos


FB3_FMT = [("11", 6, 8), ("3", 1, 1), ("21", 2, 3)]
FB4_FMT = [("13", None, None), ("31", None, None), ("22", None, None)]


# ---------------------------------------------------------------------------------------------- Pesos Numéricos
# El manual (§6) solo dice «del 0 al 9»; la regla es la del motor (FiltroPesosNumericos.cs:58-60, 113-163, 226-241).
P_UNOS = [7, 5, 3, 1, 8, 6, 4, 2, 9, 7, 5, 3, 1, 8, 6, 4]
P_EQUIS = [5, 1, 6, 2, 7, 3, 8, 4, 9, 5, 1, 6, 2, 7, 3, 8]
P_DOSES = [1, 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4, 5, 6, 7]


def raiz(n):
    """Suma de cifras repetida hasta una cifra (0 solo si no hay ningún signo de ese tipo)."""
    while n > 9:
        n = sum(int(c) for c in str(n))
    return n


def pesos(col):
    """(1, X, 2, Var, Global): cada signo suma el número de su partido en su tabla; Var = raíz(X + 2);
    Global = raíz(Var + 1)."""
    u = raiz(sum(P_UNOS[i] for i, s in enumerate(col) if s == "1"))
    x = raiz(sum(P_EQUIS[i] for i, s in enumerate(col) if s == "X"))
    d = raiz(sum(P_DOSES[i] for i, s in enumerate(col) if s == "2"))
    v = raiz(x + d)
    return u, x, d, v, raiz(v + u)


def figura(p):
    return "-".join(str(n) for n in sorted(collections.Counter(p).values(), reverse=True))


def cumple_pesos(col, conj, tol=None, num_tol="0", figuras=None):
    """conj/tol: dict con claves G, V, 1, X, 2 -> texto «a,b,c» (vacío = todos en conj, ninguno en tol). Un peso fuera
    de su conjunto es un «fallo» que solo se perdona si está en su tolerancia; el nº de fallos debe estar en
    «Núm. Tolerancias». Con figuras marcadas, la figura de los 5 pesos debe ser una de ellas."""
    u, x, d, v, g = pesos(col)
    valor = {"G": g, "V": v, "1": u, "X": x, "2": d}
    tol = tol or {}
    fallos = 0
    for k, p in valor.items():
        if conj.get(k) and p not in pw.rango(conj[k]):
            if not tol.get(k) or p not in pw.rango(tol[k]):
                return False
            fallos += 1
    if fallos not in pw.rango(num_tol):
        return False
    return not figuras or len(figuras) == 5 or figura((u, x, d, v, g)) in figuras


FB5 = dict(conj={"G": "2,5,8", "1": "1,2,4"})
FB6 = dict(conj={"G": "2,5,8", "X": "1,3,5,7,9"}, tol={"G": "1,4", "X": "2,4"}, num_tol="0,1")
FB7 = dict(FB6, figuras={"2-2-1", "2-1-1-1"})


# ---------------------------------------------------------------------------------------------- Valoración
def valoracion(col, val, signo=None):
    """Suma de los valores de los signos de la columna (o solo de los del signo dado). Manual §6, «Valoración»."""
    return sum(val[i][SG.index(s)] for i, s in enumerate(col) if signo is None or s == signo)


def en_rangos(x, texto):
    """«a-b#c-d» (la pantalla usa # entre rangos; RangosOpciones.cs:31-58). Vacío = sin límite."""
    if not texto:
        return True
    for parte in texto.split("#"):
        a, _, b = parte.partition("-")
        if float(a) <= x <= float(b or a):
            return True
    return False


def cumple_valoracion(col, val, glob="", unos="", equis="", doses=""):
    return (en_rangos(valoracion(col, val), glob) and en_rangos(valoracion(col, val, "1"), unos)
            and en_rangos(valoracion(col, val, "X"), equis) and en_rangos(valoracion(col, val, "2"), doses))


FB8 = dict(glob="590-610#640-660")
FB9 = dict(glob="590-610#640-660", unos="300-380")


# ---------------------------------------------------------------------------------------------- Análisis de signos
def vs_aciertos(col, ganadora, pleno):
    """Aciertos frente a la columna ganadora (comodín = cualquier carácter que no sea 1/X/2). Con «Último partido como
    Pleno» = Sí, el último partido solo suma si se han acertado todos los anteriores (VSignosFrm original, AcCB)."""
    ok = [g not in "1X2" or g == c for g, c in zip(ganadora, col)]
    n = sum(ok[:-1])
    if not pleno or n == len(col) - 1:
        n += ok[-1]
    return n


def vs(cols, ganadora, aspiracion, pleno):
    """(procesadas, aceptadas en orden de fichero, premios remanentes «14-13-12-11-10»)."""
    cols = [c.strip().upper() for c in cols if len(c.strip()) >= 14]
    acept = [c for c in cols if vs_aciertos(c, ganadora, pleno) >= aspiracion]
    premios = [sum(1 for c in cols if vs_aciertos(c, ganadora, pleno) == k) for k in range(14, 9, -1)]
    return len(cols), acept, "-".join(map(str, premios))


def redondeo_f0(x):
    """ToString("f0") de .NET (mitad hacia arriba sobre el valor exacto); se exige que no haya mitades exactas."""
    d = Decimal(x)
    assert d - int(d) != Decimal("0.5"), f"empate de redondeo en {x}"
    return str(d.quantize(Decimal(1), rounding=ROUND_HALF_UP))


def vs_porcentajes(acept):
    """Una línea por partido: «%1, %X, %2» enteros (VSignosFrmViewModel.cs:318-333)."""
    out = []
    for i in range(14):
        n = collections.Counter(c[i] for c in acept)
        s = sum(n.values()) or 1
        out.append(", ".join(redondeo_f0(n[g] / s * 1e2) for g in SG))
    return out


# ---------------------------------------------------------------------------------------------- Saca Formatos
def informe_formatos(col):
    """Informe de «Saca Formatos (Indeciso)»: por tamaño 2..5, los formatos en orden de primera aparición con sus
    apariciones (con solapes) y cuántos distintos; después los contactos (pares de partidos seguidos sin orden:
    1V = 1X + 12, XV = X2 + XX, 2V = X2 + 22, VV = X2 + XX + 22). Del original CalculoFormatosFrm.cs:185-466."""
    out = []
    for nombre, n in (("Pares", 2), ("Trios", 3), ("Cuartetos", 4), ("Quintetos", 5)):
        trozos = [col[k:k + n] for k in range(len(col) - n + 1)]
        orden = list(dict.fromkeys(trozos))
        out.append(f"* {nombre} que aparecen")
        out += [f"{t}-{trozos.count(t)}" for t in orden]
        out.append(f"{nombre} distintos = {len(orden)}")
    pares = collections.Counter("".join(sorted(col[k:k + 2], key=SG.index)) for k in range(len(col) - 1))
    c = {k: pares[k] for k in ("1X", "12", "X2", "11", "XX", "22")}
    c["1V"] = c["1X"] + c["12"]
    c["XV"] = c["X2"] + c["XX"]
    c["2V"] = c["X2"] + c["22"]
    c["VV"] = c["X2"] + c["XX"] + c["22"]
    out.append("* Contactos")
    out += [f"{k}-{c[k]}" for k in ("1X", "12", "X2", "11", "XX", "22", "1V", "XV", "2V", "VV")]
    return out


# ---------------------------------------------------------------------------------------------- Analizador Formatos123
def todos_formatos(col, val):
    """«Mostrar todos»: todos los trozos de la columna traducida, por longitud y posición, agrupando los iguales en su
    primera aparición (AnalisisFormatos123FrmViewModel.cs:243-291)."""
    t = a123(col, val)
    trozos = [t[p:p + n] for n in range(1, len(t) + 1) for p in range(len(t) - n + 1)]
    cuenta = collections.Counter(trozos)
    return [(f, cuenta[f]) for f in dict.fromkeys(trozos)]


# ---------------------------------------------------------------------------------------------- esperados
def esperados():
    """{clave: (descripción, líneas esperadas como las graba la app, None)}. Mismo formato que pw.esperados()."""
    r = {}
    r["FB1"] = ("B3 + Formatos: X1 «2,3» · 11 «1-3» · X*1 «1,2»",
                [c for c in B3 if cumple_formatos(c, FB1)], None)
    r["FB2"] = ("B3 + Formatos: las líneas de FB1 con Líneas «1,2» y Global «5-7»",
                [c for c in B3 if cumple_formatos(c, FB2)], None)
    r["FB3"] = ("B3 + Formatos 123 (valoración FB_val_mixta): 11 6-8 · 3 1-1 · 21 2-3 · Líneas acertadas «2»",
                [c for c in B3 if cumple_123(c, VAL_MIXTA, FB3_FMT, pw.rango("2"))], None)
    r["FB4"] = ("B3 + Formatos 123 con «Ignorar repeticiones»: 13 · 31 · 22 · Aciertos globales «1»",
                [c for c in B3 if cumple_123(c, VAL_MIXTA, FB4_FMT, pw.rango("1"), True)], None)
    r["FB5"] = ("B3 + Pesos Numéricos: Global «2,5,8» · 1 «1,2,4»", [c for c in B3 if cumple_pesos(c, **FB5)], None)
    r["FB6"] = ("B3 + Pesos: Global «2,5,8» · X «1,3,5,7,9» · Tol. Global «1,4» · Tol. X «2,4» · Núm. tol. 0 y 1",
                [c for c in B3 if cumple_pesos(c, **FB6)], None)
    r["FB7"] = ("FB6 + Figuras 2-2-1 y 2-1-1-1", [c for c in B3 if cumple_pesos(c, **FB7)], None)
    r["FB8"] = ("B3 + Valoración por sumas (FB_val_mixta): Global «590-610#640-660»",
                [c for c in B3 if cumple_valoracion(c, VAL_MIXTA, **FB8)], None)
    r["FB9"] = ("FB8 + rango del 1 «300-380»", [c for c in B3 if cumple_valoracion(c, VAL_MIXTA, **FB9)], None)
    _, acept, _ = vs(VS_COLS, GANADORA_VS, 13, pleno=False)
    r["FB10"] = ("Análisis de signos: FB_vs_entrada, ganadora 1 X 2 - …, aspirando a 13, Pleno No, «columnas»",
                 acept, None)
    r["FB11"] = ("Análisis de signos: lo mismo en «% enteros»", vs_porcentajes(acept), None)
    r["FB12"] = (f"Saca Formatos: {CF_COL_1}", [l.upper() for l in informe_formatos(CF_COL_1)], None)
    r["FB13"] = (f"Saca Formatos: {CF_COL_2} (9 pares distintos)", [l.upper() for l in informe_formatos(CF_COL_2)],
                 None)
    return r


def pantalla():
    """Números que solo se ven en pantalla (plan_b.md)."""
    p = {}
    p["T1 Traducir (Formatos 123)"] = [(c, a123(c, VAL_MIXTA)) for c in AF_COLS]
    p["AF1 Analizar 11/3/21"] = [(c, [(f, ocurrencias(a123(c, VAL_MIXTA), f)) for f, _, _ in FB3_FMT]) for c in AF_COLS]
    tf = todos_formatos(AF_COLS[0], VAL_MIXTA)
    p["AF2 Mostrar todos (columna 1)"] = (len(tf), tf[:8])
    p["V1 Calcula (Valoración)"] = [(c, valoracion(c, VAL_MIXTA)) for c in AF_COLS]
    mins = sum(min(v) for v in VAL_MIXTA)
    maxs = sum(max(v) for v in VAL_MIXTA)
    p["V2 Mín-Máx Sumas / rangos que rellena"] = (f"{mins}-{maxs}", [f"0-{sum(v[k] for v in VAL_MIXTA)}" for k in range(3)])
    for nombre, gan, asp, pleno in (("VS0 al abrir (todo -, 14, Pleno Sí)", "-" * 14, 14, True),
                                    ("VS1 Pleno No", GANADORA_VS, 13, False), ("VS2 Pleno Sí", GANADORA_VS, 13, True)):
        n, acept, prem = vs(VS_COLS, gan, asp, pleno)
        cuenta = [tuple(sum(1 for c in acept if c[i] == g) for g in SG) for i in range(14)]
        p[nombre] = (n, len(acept), prem, cuenta)
    return p


# ---------------------------------------------------------------------------------------------- comprobaciones
def comprobar():
    # Manual §6, Formatos: en 11X211X1X12111 el formato 1X aparece 3 veces y 1X2 una.
    assert apariciones("11X211X1X12111", "1X") == 3 and apariciones("11X211X1X12111", "1X2") == 1
    assert apariciones("111", "11") == 2                              # con solapes
    assert apariciones("1X2", "V") == 2 and apariciones("1X2", "*") == 3
    # Manual §6, Valoración: dos triples a 50/30/20, suma 60-100 => fuera X2, 2X y 22 (50, 50 y 40).
    dos = [(50, 30, 20)] * 2
    fuera = sorted(a + b for a in SG for b in SG if not en_rangos(valoracion(a + b, dos), "60-100"))
    assert fuera == ["22", "2X", "X2"], fuera
    assert [valoracion(c, dos) for c in ("X2", "2X", "22")] == [50, 50, 40]
    # Pantalla Formatos 123: 1 = más valorado, 3 = menos.
    assert rango123((50, 30, 20)) == ("1", "2", "3") and rango123((20, 30, 50)) == ("3", "2", "1")
    assert rango123((0, 0, 0)) == ("1", "2", "3")                      # sin valoración: 1→1, X→2, 2→3
    # Saca Formatos: el par 1X del ejemplo del manual sale 3 veces.
    assert "1X-3" in informe_formatos(CF_COL_1) and "Pares distintos = 9" in informe_formatos(CF_COL_2)
    # Pesos: la nota de la pantalla (PesosNumFrmViewModel.cs:217-218) dice que las figuras 5 y 4-1 darían 0 columnas;
    # se comprueba sobre las 3^14 columnas (programación dinámica sobre las tres sumas).
    est = collections.Counter({(0, 0, 0): 1})
    for i in range(14):
        nuevo = collections.Counter()
        for (u, x, d), n in est.items():
            nuevo[(u + P_UNOS[i], x, d)] += n
            nuevo[(u, x + P_EQUIS[i], d)] += n
            nuevo[(u, x, d + P_DOSES[i])] += n
        est = nuevo
    figs = collections.Counter()
    for (u, x, d), n in est.items():
        U, X, D = raiz(u), raiz(x), raiz(d)
        figs[figura((U, X, D, raiz(X + D), raiz(raiz(X + D) + U)))] += n
    assert sum(figs.values()) == 3 ** 14 and not ({"5", "4-1"} & set(figs)), figs
    # Análisis de signos: N = 6 aceptadas, sin mitades exactas al redondear.
    assert len(vs(VS_COLS, GANADORA_VS, 13, False)[1]) == 6

    # Sensibilidad: cada prueba distingue el resultado correcto de un fallo verosímil de la app.
    e = {k: len(v[1]) for k, v in esperados().items()}
    rev = [c[::-1] for c in B3]                                          # leer la columna al revés
    assert sum(cumple_formatos(c, FB1) for c in rev) != e["FB1"]
    assert sum(cumple_123(c, VAL_MIXTA[::-1], FB3_FMT, {2}) for c in B3) != e["FB3"]          # valoración al revés
    assert sum(cumple_123(c, [(0, 0, 0)] * 14, FB3_FMT, {2}) for c in B3) != e["FB3"]         # sin valoración
    assert sum(cumple_pesos(c[::-1], **FB5) for c in B3) != e["FB5"]                           # pesos al revés
    assert sum(cumple_pesos(c, **dict(FB6, tol=None, num_tol="0")) for c in B3) != e["FB6"]    # sin tolerancias
    assert sum(cumple_valoracion(c, VAL_MIXTA[::-1], **FB8) for c in B3) != e["FB8"]
    assert sum(cumple_valoracion(c, VAL_MIXTA, glob="590-610") for c in B3) != e["FB8"]       # solo el 1er rango
    assert len(vs(VS_COLS, GANADORA_VS, 13, True)[1]) != e["FB10"]                            # Pleno Sí
    for k, n in e.items():
        assert n > 0, k
        if k in ("FB1", "FB2", "FB3", "FB4", "FB5", "FB6", "FB7", "FB8", "FB9"):
            assert 20 <= n <= 180, (k, n)


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
            print(f"·   {clave:5} sin salida                {desc}")
            falta += 1
            continue
        real = pw.leer(f)
        bien = pw.multiconjunto(real) == pw.multiconjunto(cols)
        det = f"{len(real)} líneas" + ("" if bien else f", se esperaban {len(cols)}")
        if not bien:
            sobran, faltan = sorted(set(real) - set(cols))[:3], sorted(set(cols) - set(real))[:3]
            det += (f"; sobran {sobran}" if sobran else "") + (f"; faltan {faltan}" if faltan else "")
        print(f"{'OK' if bien else 'MAL':3} {clave:5} {det:30} {desc}")
        ok += bien
        mal += not bien
    print(f"\n{ok} bien, {mal} mal, {falta} sin salida")
    return mal == 0


if __name__ == "__main__":
    sys.stdout.reconfigure(encoding="utf-8")
    orden = sys.argv[1] if len(sys.argv) > 1 else ""
    if orden == "preparar":
        preparar(sys.argv[2])
    elif orden == "verificar":
        sys.exit(0 if verificar(sys.argv[2]) else 1)
    else:
        comprobar()
        print("asserts OK")
        for clave, (desc, cols, _) in esperados().items():
            print(f"{clave:5} {len(cols):4} líneas  {desc}")
        print()
        for k, v in pantalla().items():
            print(f"{k}: {v}")
