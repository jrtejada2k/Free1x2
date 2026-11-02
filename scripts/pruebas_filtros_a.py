"""Oráculo del grupo A de filtros: Distancias, Simetrías, Contactos, Dibujos, Diferencias y Figuras (de Contactos).

    python -I scripts/pruebas_filtros_a.py                      # asserts + clave y nº de columnas esperadas
    python -I scripts/pruebas_filtros_a.py preparar <carpeta>   # escribe <carpeta>/entrada/FA_*  (con pw.escribir, sin tocar el texto)
    python -I scripts/pruebas_filtros_a.py verificar <carpeta>  # compara <carpeta>/salida/FA*.txt con lo esperado

Independiente de la app: cada regla se programa aquí con posiciones y cadenas (definiciones del manual y, donde el
manual no llega, del motor C#, que se cita). Además, `emulacion_motor()` reprograma la lógica de bits del motor C#
(máscaras, desplazamientos) y comprueba que da lo mismo que la regla «de manual» en las 216 columnas de B3 y en 3.000
columnas al azar: así la regla del oráculo y la del motor son la misma, no solo parecidas.

Formato de salida: como pw.esperados() -> {clave: (descripción, columnas, None)}; columnas de 14 caracteres «1X2» en
mayúsculas, como las graba la app (UtilColumnas.ConvLongToStr usa "2X1" y pw.leer() normaliza igualmente).
"""
import random
import sys
from collections import Counter
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import pruebas_windows as pw  # noqa: E402

B3 = pw.desarrollar(pw.P_B3)                       # 216 columnas (3 triples, 3 dobles, 8 fijos)
TODOS = "0,1,2,3,4,5,6,7,8,9,10,11,12,13"          # lo que se escribe en una fila de Contactos «sin límite»


# ------------------------------------------------------------------------------------------- lectura de campos
def lista_comas(texto, todos=range(15)):
    """Campos de Distancias y Contactos: solo números separados por comas (SetNoIntVar / SetNum1X hacen
    Convert.ToInt32 de cada trozo; «3-6» lanza FormatException). Vacío = todos los valores (el VM rellena con
    ObtenerTodosValores)."""
    if not texto.strip():
        return set(todos)
    return {int(p) for p in texto.split(",")}


def lista_rangos(texto):
    """Campos que sí admiten rangos: Aciertos de Simetrías (SimetriasFrmViewModel.ObtenerAciertos) y los conceptos de
    Diferencias (UtilidadesEntradasValores.ObtenerListaFromTxtAciertos): «1,3,5-7»."""
    r = set()
    for p in texto.split(","):
        a, _, b = p.strip().partition("-")
        r.update(range(int(a), int(b or a) + 1))
    return r


def partidos(texto):
    """Fila de partidos «a,b-c» (Simetrías y Diferencias) -> lista ordenada de partidos 1..14."""
    return sorted(lista_rangos(texto))


# ------------------------------------------------------------------------------------------- reglas (manual)
def distancia(c, signos):
    """Manual §2 (MANUAL_USUARIO.md:84): «mayor separación entre dos apariciones del mismo signo». Con 0 o 1
    apariciones, 0 (FiltroDistancias.Distancia devuelve 0)."""
    pos = [i for i, s in enumerate(c) if s in signos]
    return max((b - a for a, b in zip(pos, pos[1:])), default=0)


def distancias(c):
    """«Var» = variantes, X o 2 (manual :84 «V»; motor MascVariantes = 011 en cada partido)."""
    return {"Var": distancia(c, "X2"), "1": distancia(c, "1"), "X": distancia(c, "X"), "2": distancia(c, "2")}


def aciertos_simetrias(c, grupos):
    """Manual §6 (:342): una simetría acierta si todos sus partidos llevan el mismo signo. Se cuenta cuántas aciertan
    (FiltroSimetrias.Analizar) y ese número debe estar en «Aciertos»."""
    return sum(1 for g in grupos if len({c[p - 1] for p in g}) == 1)


PAREJAS = ("1X", "12", "X2", "11", "XX", "22", "1V", "XV", "2V", "VV")


def contactos(c):
    """Pantalla Contactos: «contacto entre dos signos cuando aparecen juntos, sin importar el orden». 13 parejas de
    partidos seguidos. Derivadas como en FiltroContactos.AnalizaColumna: 1V = 1X+12, XV = XX+X2, 2V = 22+X2,
    VV = XX+X2+22."""
    n = Counter("".join(sorted(p, key="1X2".index)) for p in zip(c, c[1:]))
    r = {k: n[k] for k in ("1X", "12", "X2", "11", "XX", "22")}
    r["1V"] = r["1X"] + r["12"]
    r["XV"] = r["XX"] + r["X2"]
    r["2V"] = r["22"] + r["X2"]
    r["VV"] = r["XX"] + r["X2"] + r["22"]
    return r


def figura_contactos(c):
    """Figura de Contactos (FiltroContactos.ObtenerFiguraLongValor + ObtenerTextoFiguraFromLong): los 10 recuentos
    distintos de cero, de mayor a menor, separados por «-»."""
    return "-".join(str(v) for v in sorted((v for v in contactos(c).values() if v), reverse=True))


def dibujo(c):
    """Manual §2 (:85): dibujo X+Y = nº de equis + nº de doses."""
    return c.count("X"), c.count("2")


def interrupciones_n(s):
    return sum(1 for a, b in zip(s, s[1:]) if a != b)


def resultados_diferencia(c, grupos):
    """Diferencias (Diferencia.Analizar): por cada grupo de partidos se toma la subcolumna (partidos en orden) y, por
    concepto, se cuenta cuántos valores DISTINTOS salen entre los grupos: variantes, equis, doses, dibujo (X,2),
    interrupciones dentro del grupo y formato (la secuencia de signos)."""
    subs = ["".join(c[p - 1] for p in g) for g in grupos]
    return {"Variantes": len({sum(s != "1" for s in t) for t in subs}),
            "Equis": len({t.count("X") for t in subs}),
            "Doses": len({t.count("2") for t in subs}),
            "Dibujos": len({(t.count("X"), t.count("2")) for t in subs}),
            "Interrupciones": len({interrupciones_n(t) for t in subs}),
            "Formatos": len(set(subs))}


# ------------------------------------------------------------------------------------------- condiciones
def cumple_distancias(c, campos):
    d = distancias(c)
    return all(d[k] in lista_comas(v) for k, v in campos.items())


def cumple_simetrias(c, filas, aciertos):
    grupos = [partidos(f) for f in filas if f.strip()]
    ac = lista_rangos(aciertos) if aciertos.strip() else set()        # vacío: ninguna (ver plan, hallazgo)
    return aciertos_simetrias(c, grupos) in ac


def cumple_contactos(c, filas, figuras=()):
    d = contactos(c)
    if not all(d[k] in lista_comas(filas.get(k, ""), range(16)) for k in PAREJAS):
        return False
    return not figuras or figura_contactos(c) in figuras


def cumple_dibujos(c, marcados):
    return "%d+%d" % dibujo(c) in marcados


def cumple_diferencia(c, filas, conceptos):
    """Un «conjunto» de Diferencias. Concepto vacío = cualquier nº de valores distintos."""
    grupos = [partidos(f) for f in filas if f.strip()]
    r = resultados_diferencia(c, grupos)
    return all(r[k] in lista_rangos(v) for k, v in conceptos.items() if v.strip())


# ------------------------------------------------------------------------------------------- datos de las pruebas
FA1 = {"Var": "2,3", "1": "3,4", "X": "6", "2": "0,2,3,4,5,6,7"}
FA1b = {"Var": "", "1": "3,4", "X": "", "2": ""}
FA2_FILAS = ["1,4", "6-7", "3,11", "8-10"]
FA2_ACIERTOS = "2-3"
FA3 = dict({k: TODOS for k in PAREJAS}, **{"1X": "4,5,6", "11": "1,2", "2V": "1,2", "VV": "2,3"})
FA3b = {"11": "1,2"}                                   # las otras 9 filas vacías
FA4 = {"4+3", "3+4", "5+2"}
TRIOS = ["1-3", "4-6", "7-9", "10-12"]                  # Atajos ▸ Tríos ▸ Paso fijo
FA5 = {"Variantes": "", "Equis": "2", "Doses": "2-3", "Dibujos": "", "Interrupciones": "2", "Formatos": "4"}
MITADES = ["1-7", "8-14"]
FA5b_2 = {"Variantes": "1"}
FIGS = ("9-5-4-2-2-2-1-1-1", "8-5-3-3-2-2-1-1-1")
FA6_FILAS = {k: TODOS for k in PAREJAS}


def cont_xml(filas, figuras):
    """Fichero de condición de Contactos (.cont) como lo escribe ArchivoCondiciones.GuardaArchivo(IFiltro) +
    FContactosData.EscribirCondicionesFiltros, para cargarlo con «Abrir» en la pantalla Contactos."""
    ls = ["<xml>", '  <file version="0.1" />', "  <Condiciones>", '    <condicion id="Contactos" activa="True">',
          "      <standard>"]
    ls += ['        <valores tipo="%s" values="%s" />' % (k, filas[k]) for k in PAREJAS]
    ls += ["      </standard>", "      <figuras>"]
    ls += ['        <figura value="%s" />' % f for f in figuras]
    ls += ["      </figuras>", "    </condicion>", "  </Condiciones>", "</xml>"]
    return ls


ENTRADAS = {
    "FA_contactos_figuras.cont": lambda: cont_xml(FA6_FILAS, FIGS),
}


# ------------------------------------------------------------------------------------------- esperado
def esperados():
    r = {}
    r["FA1"] = ("B3 + Distancias: Var 2,3 · 1 3,4 · X 6 · 2 0,2,3,4,5,6,7",
                [c for c in B3 if cumple_distancias(c, FA1)], None)
    r["FA1b"] = ("B3 + Distancias: solo 1 = 3,4 (Var, X y 2 vacíos = todos)",
                 [c for c in B3 if cumple_distancias(c, FA1b)], None)
    r["FA2"] = ("B3 + Simetrías: 1,4 · 6-7 · 3,11 · 8-10; Aciertos 2-3",
                [c for c in B3 if cumple_simetrias(c, FA2_FILAS, FA2_ACIERTOS)], None)
    r["FA3"] = ("B3 + Contactos: 1X 4,5,6 · 11 1,2 · 2V 1,2 · VV 2,3; las otras 6 filas 0..13",
                [c for c in B3 if cumple_contactos(c, FA3)], None)
    r["FA3b"] = ("B3 + Contactos: solo 11 = 1,2 (otras 9 filas vacías = todos, como el original)",
                 [c for c in B3 if cumple_contactos(c, FA3b)], None)
    r["FA4"] = ("B3 + Dibujos: 4+3, 3+4, 5+2", [c for c in B3 if cumple_dibujos(c, FA4)], None)
    r["FA5"] = ("B3 + Diferencias: Tríos 1-3,4-6,7-9,10-12; Equis 2 · Doses 2-3 · Interrupciones 2 · Formatos 4",
                [c for c in B3 if cumple_diferencia(c, TRIOS, FA5)], None)
    r["FA5b"] = ("FA5 + 2.º conjunto: 1-7, 8-14; Variantes 1",
                 [c for c in B3 if cumple_diferencia(c, TRIOS, FA5) and cumple_diferencia(c, MITADES, FA5b_2)], None)
    r["FA6"] = ("B3 + Contactos (10 filas 0..13) + Figuras por el editor: " + " y ".join(FIGS),
                [c for c in B3 if cumple_contactos(c, FA6_FILAS, FIGS)], None)
    r["FA7"] = ("B3 + Contactos abierto de FA_contactos_figuras.cont (mismas figuras que FA6)",
                [c for c in B3 if cumple_contactos(c, FA6_FILAS, FIGS)], None)
    r["FA8"] = ("Tras FA7: editor de figuras, se borra la 2.ª (queda " + FIGS[0] + ")",
                [c for c in B3 if cumple_contactos(c, FA6_FILAS, FIGS[:1])], None)
    return r


# ------------------------------------------------------------------------------------------- comprobaciones
def ejemplos_manual():
    col = "11X211X1X12111"                                 # columna de ejemplo del manual §2
    assert distancias(col) == {"1": 3, "X": 4, "2": 7, "Var": 3}, distancias(col)       # MANUAL_USUARIO.md:84
    assert dibujo(col) == (3, 2)                                                       # :85
    assert sum(s != "1" for s in col[:7]) == 3 and sum(s != "1" for s in col[7:]) == 2  # :88 mitades 7+7, figura «3+2»
    # Contactos (sin ejemplo en el manual; a mano): 11X2·11X1X1·2111 -> 11:4, 1X:5, X2:1, 12:3
    assert contactos(col) == {"1X": 5, "12": 3, "X2": 1, "11": 4, "XX": 0, "22": 0,
                              "1V": 8, "XV": 1, "2V": 1, "VV": 1}, contactos(col)
    assert sum(contactos(col)[k] for k in ("1X", "12", "X2", "11", "XX", "22")) == 13
    assert figura_contactos(col) == "8-5-4-3-1-1-1-1"                  # X2, XV, 2V y VV valen 1
    # Simetrías (a mano): 1,2 = 11 sí · 3,7 = XX sí · 4,11 = 22 sí · 1,3 = 1X no -> 3 aciertos
    assert aciertos_simetrias(col, [[1, 2], [3, 7], [4, 11], [1, 3]]) == 3
    # Diferencias (manual :344 «misma cantidad…»): mitades 11X211X / 1X12111
    r = resultados_diferencia(col, [partidos("1-7"), partidos("8-14")])
    assert r == {"Variantes": 2, "Equis": 2, "Doses": 1, "Dibujos": 2, "Interrupciones": 1, "Formatos": 2}, r
    # Lectura de campos
    assert lista_rangos("1,3,5-7") == {1, 3, 5, 6, 7} and partidos("8-10") == [8, 9, 10]
    assert lista_comas("") == set(range(15))
    try:
        lista_comas("3-6")
        raise AssertionError("«3-6» no debe admitirse en Distancias/Contactos")
    except ValueError:
        pass


# --- emulación literal del motor C# (bits) para comprobar que la regla de arriba es la del motor
COD = {"1": 4, "X": 2, "2": 1}                     # UtilColumnas: "2X1"[(L & 7) >> 1]; partido 1 en los bits bajos


def a_long(c):
    return sum(COD[s] << (3 * i) for i, s in enumerate(c))


def distancia_cs(col):                              # FiltroDistancias.Distancia
    dist = 0
    if col:
        while col & 7 == 0:
            col >>= 3
        while True:
            col >>= 3
            act = 0
            while col & 7 == 0 and col:
                act += 1
                col >>= 3
            if col:
                dist = max(act + 1, dist)
            if not col:
                return dist
    return dist


def contactos_cs(col):                              # FiltroContactos.AnalizaColumna
    sin = 0
    while col:
        s = col & 7
        if s:
            sin = sin << 3 | s
        col >>= 3
    n = dict.fromkeys(("1X", "12", "X2", "11", "XX", "22"), 0)
    s1 = sin & 7
    while sin:
        sin >>= 3
        s2 = sin & 7
        if s2:
            n[{6: "1X", 5: "12", 3: "X2", 4: "11", 2: "XX", 1: "22"}[s1 | s2]] += 1
            s1 = s2
    n["1V"] = n["1X"] + n["12"]
    n["VV"] = n["XX"] + n["X2"] + n["22"]
    n["2V"] = n["22"] + n["X2"]
    n["XV"] = n["XX"] + n["X2"]
    v = sorted(n[k] for k in ("1X", "12", "X2", "11", "XX", "22", "1V", "XV", "2V", "VV"))
    fig = 0
    for x in reversed(v):                           # ObtenerFiguraLongValor
        if x > 0:
            fig = fig << 4 | x
    txt = ""
    while fig:                                      # ObtenerTextoFiguraFromLong
        txt = "-" + str(fig & 15) + txt
        fig >>= 4
    return n, txt[1:] or "0"


def ignorar_partidos(grupo, col):                   # UtilColumnas.IgnorarPartidos (conserva los marcados)
    m = 0
    for i in sorted(grupo, reverse=True):
        m = m << 3 | (col >> 3 * (i - 1)) & 7
    return m


def interrupciones_cs(col):                         # FiltroInterrupciones.AnalizaColumna(long), solo el global
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


def diferencia_cs(col, grupos):                     # Diferencia.Analizar
    V, X, D, Dib, I, F = set(), set(), set(), set(), set(), set()
    for g in grupos:
        sub = ignorar_partidos(g, col)
        x = bin(sub & 80421421917330).count("1")    # FiltroNoVariantes: máscaras de X y de 2
        d = bin(sub & 5026338869833).count("1")
        V.add(x + d), X.add(x), D.add(d), Dib.add((x, d)), I.add(interrupciones_cs(sub)), F.add(sub)
    return {"Variantes": len(V), "Equis": len(X), "Doses": len(D), "Dibujos": len(Dib),
            "Interrupciones": len(I), "Formatos": len(F)}


def emulacion_motor():
    rnd = random.Random(2026)
    muestra = B3 + ["".join(rnd.choice("1X2") for _ in range(14)) for _ in range(3000)]
    mascaras = {"Var": 3952873730080618203, "1": 5270498306774157604, "X": 2635249153387078802,
                "2": 1317624576693539401}
    grupos = [partidos(t) for t in TRIOS] + [[1, 5, 14]]
    for c in muestra:
        L = a_long(c)
        assert {k: distancia_cs(L & m) for k, m in mascaras.items()} == distancias(c), c
        n, fig = contactos_cs(L)
        assert n == contactos(c) and fig == (figura_contactos(c) or "0"), c
        assert (bin(L & 80421421917330).count("1"), bin(L & 40210710958665).count("1")) == dibujo(c), c
        for gs in (grupos, [partidos(t) for t in MITADES]):
            assert diferencia_cs(L, gs) == resultados_diferencia(c, gs), c
    return len(muestra)


def sensibilidad():
    """Cada campo usado en una prueba debe importar: quitándolo, el recuento cambia (si no, la prueba no lo prueba)."""
    def cuenta(f):
        return sum(1 for c in B3 if f(c))
    sal = []
    for k in FA1:
        if FA1[k]:
            sin = dict(FA1, **{k: ""})
            sal.append(("FA1 sin " + k, cuenta(lambda c: cumple_distancias(c, sin))))
    for k in ("1X", "11", "2V", "VV"):
        sin = dict(FA3, **{k: TODOS})
        sal.append(("FA3 sin " + k, cuenta(lambda c: cumple_contactos(c, sin))))
    for k in [k for k, v in FA5.items() if v]:
        sin = dict(FA5, **{k: ""})
        sal.append(("FA5 sin " + k, cuenta(lambda c: cumple_diferencia(c, TRIOS, sin))))
    sal.append(("FA2 Aciertos 2", cuenta(lambda c: cumple_simetrias(c, FA2_FILAS, "2"))))
    sal.append(("FA2 sin fila 8-10", cuenta(lambda c: cumple_simetrias(c, FA2_FILAS[:3], FA2_ACIERTOS))))
    return sal


def main():
    sys.stdout.reconfigure(encoding="utf-8")
    ejemplos_manual()
    n = emulacion_motor()
    res = esperados()
    for clave, (desc, cols, _) in res.items():
        assert all(len(c) == 14 and set(c) <= set("1X2") for c in cols)
        assert 0 < len(cols) < 216 and len(set(cols)) == len(cols), clave
    assert res["FA6"][1] == res["FA7"][1]
    print(f"asserts OK (ejemplos del manual + motor emulado en {n} columnas)\n")
    for clave, (desc, cols, _) in res.items():
        print(f"{clave:5} {len(cols):4} col.  {desc}")
    print("\nSensibilidad (recuento al quitar un campo; debe ser distinto del de la prueba):")
    for nombre, v in sensibilidad():
        print(f"  {nombre:22} {v}")


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
