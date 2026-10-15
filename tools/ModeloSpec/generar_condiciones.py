"""Suite golden `formatos/condiciones` (07 F02/F03): XML de ida y vuelta y errores, por lectura de ArchivoCondiciones.cs,
ArchivoCombinacion.cs y las F*Data.cs. Las reglas de serialización son las de 07 §1.0.1 (sangría 2, " />")."""
import json, sys

def esc(v, attr=True):
    v = v.replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;")
    return v.replace('"', "&quot;") if attr else v

def ser(n, d=0):
    p = "  " * d
    if n[0] == "#c":
        return [p + "<!--" + n[1] + "-->"]
    tag, attrs, hijos = n
    a = "".join(f' {k}="{esc(v)}"' for k, v in attrs)
    if isinstance(hijos, str):
        return [p + f"<{tag}{a}>{esc(hijos, False)}</{tag}>"] if hijos else [p + f"<{tag}{a} />"]
    if not hijos:
        return [p + f"<{tag}{a} />"]
    r = [p + f"<{tag}{a}>"]
    for h in hijos:
        r += ser(h, d + 1)
    return r + [p + f"</{tag}>"]

def E(tag, *attrs, h=()):
    return (tag, list(attrs), list(h) if not isinstance(h, str) else h)

def xml(*cuerpo):
    return "\n".join(ser(E("xml", h=[E("file", ("version", "0.1"))] + list(cuerpo))))

def cond(*condiciones):
    return xml(E("Condiciones", h=condiciones))

def std(pares, tag="standard"):
    return E(tag, h=[E("valores", ("tipo", t), ("values", v)) for t, v in pares])

def figs(tag, lista):
    return E(tag, h=[E("figura", ("value", f)) for f in lista])

C = lambda id, activa, *h, extra=(): E("condicion", ("id", id), ("activa", activa), *extra, h=h)
todos = ",".join(str(i) for i in range(15))

filtros = {
    "NoVariantes": C("NoVariantes", "True", std([("var", "3,4A,5"), ("equis", "0,1,2"), ("doses", "2B,3")])),
    "ValoracionSignos": C("ValoracionSignos", "False",
        E("valores", ("id", "0"), h="1.5,2,0.25"), E("valores", ("id", "1"), h="10,20.75,-3"),
        extra=[("tipoValoracion", "suma"), ("valorGlobal", "10-20#25"), ("valorUnos", ""), ("valorEquis", "1,5-3"), ("valorDoses", "")]),
    "SignosSeguidos": C("SignosSeguidos", "True", std([("var", "0,1,2"), ("unos", "3A"), ("equis", "1"), ("doses", "4")]),
        figs("figurasUnos", ["1-2-3", "0"]), figs("figurasDoses", ["15-15"])),
    "NoInterrupciones": C("NoInterrupciones", "True", std(list(zip(
        ["global", "var", "unos", "equis", "doses", "globalSeg", "varSeg", "unosSeg", "equisSeg", "dosesSeg"],
        ["1,2", "3", "0A", "4", "5,6", "0,1", "2", "3B", "4", "5"])))),
    "Dibujos": C("Dibujos", "True", E("dibujo", h="3+2"), E("dibujo", h="0+0")),
    "PesosNumericos": C("PesosNumericos", "True",
        std([("global", "1,2,9"), ("variantes", "0"), ("unos", "3"), ("equis", "4,5"), ("doses", "6")]),
        E("tolerancias", ("noTolerancias", "0,1"), h=[E("valores", ("tipo", t), ("values", v)) for t, v in
            [("global", "1"), ("variantes", "2"), ("unos", "3"), ("equis", "4"), ("doses", "5")]]),
        figs("figuras", ["1-2"])),
    "Distancias": C("Distancias", "True", std([("var", "1,2"), ("unos", "3"), ("equis", "4A"), ("doses", "5")])),
    "GruposEquipos": C("GruposEquipos", "True",
        ("#c", " Para cada partido:"), ("#c", " 0 => ningun equipo seleccionado"), ("#c", " 1 => solo equipo casa seleccionado"),
        ("#c", " 2 => solo equipo fuera seleccionado"), ("#c", " 3 => equipos casa y fuera seleccionados"),
        E("grupoEquipos", ("id", "0"), h=[E("equipos", ("value", "12030000000000")), E("victorias", ("value", "1,3")),
            E("empates", ("value", "")), E("derrotas", ("value", "0")), E("sumaPuntos", ("value", "2,6"))]),
        E("Relacion1", ("id", "0"), h=[E("ge", ("values", "1,2")), E("victorias", ("values", "2")), E("empates", ("values", "")),
            E("derrotas", ("values", "")), E("puntos", ("values", "3,9"))])),
    "Contactos": C("Contactos", "True", std(list(zip(["1X", "12", "X2", "11", "XX", "22", "1V", "XV", "2V", "VV"],
        ["0", "1", "2A", "3", "4", "5", "6", "7", "8", "9"]))), figs("figuras", ["2-1"])),
    "FormatosSignos": C("FormatosSignos", "True", E("grupoFormato", ("id", "0"), ("lineas", "1"), ("global", "1"),
        h=[E("lineaFormato", ("id", "0"), ("formato", "1X2"), ("rango", "1,2"))])),
    "ColProbables": C("ColProbables", "True",
        E("cp", ("id", "0"), h=[E("pronostico", ("values", "1X,,X2,1X2,1,X,2,1,1,1,1,1,1,1")),
            E("standard", h=[E("aciertos", ("values", "0,1,2")), E("aciertosSeguidos", ("values", "0,1")),
                E("fallosSeguidos", ("values", "0,1,2,3")), E("puntos", ("values", "5,10"))]),
            E("tolerancias", ("noTolerancias", "1"), h=[E("aciertosTol", ("values", "3")),
                E("aciertosSeguidosTol", ("values", "2")), E("fallosSeguidosTol", ("values", "4"))])]),
        E("cp", ("id", "1"), h=[E("pronostico", ("values", "1,1,1,1,1,1,1,1,1,1,1,1,1,1")),
            E("standard", h=[E("aciertos", ("values", "10")), E("aciertosSeguidos", ("values", "2")),
                E("fallosSeguidos", ("values", "1")), E("puntos", ("values", ""))])]),
        E("relacion1", ("id", "0"), h=[E("columnas", ("values", "1,2")), E("sumas", ("values", "2,3")),
            E("recorridos", ("values", "0,1")), E("gruposCP", ("cantidadCP", "1,2"), ("noAC", "3"))]),
        E("relacion2", ("id", "0"), h=[E("columnasA", ("values", "1")), E("columnasB", ("values", "2")),
            E("aciertos", ("values", "0,1")), E("concepto", ("value", "AC")), E("cantidad", ("value", "1")),
            E("columnasA2", ("values", "")), E("columnasB2", ("values", "")), E("aciertos2", ("values", "")),
            E("concepto2", ("value", "")), E("cantidad2", ("value", ""))]),
        E("fallosCP", ("id", "0"), h=[E("columnas", ("values", "1,2")), E("tolerancias", ("values", "AB")),
            E("aciertos", ("values", "0,1"))]),
        E("controlFallos", ("noFallos", "0,1")),
        E("relacion3", ("id", "0"), h=[E("columnas", ("values", "1,2")), E("sandwichs", ("values", "0")),
            E("escalerasAsc", ("values", "0")), E("escalerasDesc", ("values", "")), E("escaleras", ("values", "0,1")),
            E("concepto", ("values", "AC")), E("agrupacionesPasoFijo", ("values", "1,2+2+0,1")),
            E("agrupacionesSolapadas", ("values", ""))])),
    "Formatos123": C("Formatos123", "True", E("grupoformato", ("id", "0"), ("aciertos", "1,2"), ("repeticiones", "True"),
        ("valoracion", "/".join(["50", "30", "20", "33,5", "33", "33,5"] * 7)),
        h=[E("formatos", ("id", "0"), ("formato", "123"), ("aciertosMin", "0"), ("aciertosMax", "3"))])),
    "Simetrias": C("Simetrias", "True", E("gruposimetrias", ("id", "0"), ("aciertos", "1,2"),
        h=[E("simetrias", ("id", "0"), ("simetria", "1,2")), E("simetrias", ("id", "1"), ("simetria", "3,4,5"))])),
    "Diferencias": C("Diferencias", "False", E("grupoDiferencias", ("id", "0"), h=[
        E("partidos", ("id", "0"), ("valores", "1,2,3")), E("partidos", ("id", "1"), ("valores", "4,5,6")),
        E("aciertos", ("aciertosV", "0,1"), ("aciertosX", "1"), ("aciertos2", "2"), ("aciertosDib", "0,2"),
            ("aciertosInt", ""), ("aciertosFormatos", "1,2"))])),
}

T = ["ESPEC_07 F03.a/F03.b", "Free1X2.Domain/EntradaSalida/ArchivoCondiciones.cs:77-83,262-273", "Free1X2.Domain/EntradaSalida/F*Data.cs"]
casos = []
def caso(id, comp, entrada, salida, traza=T):
    casos.append({"id": id, "fuente": "lectura", "componente": comp, "entrada": entrada, "salida": salida, "traza": traza})

caso("F03-NV-EJEMPLO", "ArchivoCondiciones", {"archivo": "condicion", "tipo": "NoVariantes",
    "xml": cond(C("NoVariantes", "True", std([("var", todos), ("equis", todos), ("doses", todos)])))}, {"igual": True},
    T + ["ESPEC_07 F03, ejemplo mínimo de NoVariantes"])
for nombre, c in filtros.items():
    caso(f"F03-{nombre}-RT", "ArchivoCondiciones", {"archivo": "condicion", "tipo": nombre, "xml": cond(c)}, {"igual": True})

# Errores y peculiaridades.
caso("F03-NV-VACIO", "FNoVariantesData", {"archivo": "condicion", "tipo": "NoVariantes",
    "xml": cond(C("NoVariantes", "True", std([("var", "")])))}, {"error": "IndexOutOfRangeException"},
    T + ["SetNoVariantes(\"\") ⇒ str[-1] (MC/FiltroNoVariantes.cs:231-254)"])
caso("F03-PES-SINTOL", "FPesosNumericosData", {"archivo": "condicion", "tipo": "PesosNumericos",
    "xml": cond(C("PesosNumericos", "True", E("tolerancias", ("noTolerancias", ""))))}, {"error": "NullReferenceException"},
    T + ["PonerTolerancia(GetValores(\"\") = null) (ESPEC_02 §1 P8)"])
caso("F03-ACTIVA-FALTA", "FNoVariantesData", {"archivo": "condicion", "tipo": "NoVariantes",
    "xml": cond(E("condicion", ("id", "NoVariantes"), h=[]))}, {"error": "NullReferenceException"})
caso("F03-ACTIVA-1", "FNoVariantesData", {"archivo": "condicion", "tipo": "NoVariantes",
    "xml": cond(C("NoVariantes", "1"))}, {"error": "FormatException"}, T + ["Convert.ToBoolean(\"1\") (07 §1.0)"])
caso("F03-DIB-COMENTARIO", "FDibujosData", {"archivo": "condicion", "tipo": "Dibujos",
    "xml": cond(C("Dibujos", "True", E("dibujo", h="3+2"), ("#c", " x ")))}, {"error": "FormatException"},
    T + ["InnerText de todos los hijos, comentarios incluidos (07 F03.a)"])
caso("F03-INT-COMPAT", "FInterrupcionesData", {"archivo": "condicion", "tipo": "NoInterrupciones",
    "xml": cond(C("NoInterrupciones", "True", std([("global", "1")])))},
    {"xml": cond(C("NoInterrupciones", "True", std(list(zip(
        ["global", "var", "unos", "equis", "doses", "globalSeg", "varSeg", "unosSeg", "equisSeg", "dosesSeg"],
        ["1", "", "", "", "", todos, todos, todos, todos, todos])))))},
    T + ["sin *Seg ⇒ \"0,…,14\" (FInterrupcionesData.cs:125-134)"])
caso("F03-ID-DESCONOCIDO", "ArchivoCondiciones", {"archivo": "condicion", "tipo": "Diferencias",
    "xml": cond(C("Nada", "True", E("grupoDiferencias", ("id", "0"))))},
    {"error": "NullReferenceException"},
    T + ["GetFiltro(id) desconocido ⇒ el último filtro, Diferencias (MC/Grupo.cs:415-431): se lee sin error",
         "al guardar, Diferencia sin <aciertos> tiene Analiza* = true y Ac* = null ⇒ ObtenerTextoFromBool(null) (FDiferenciasData.cs:148)"])

# Grupo e If-Then.
grupo = xml(E("gruposPartidos", h=[E("grupo", ("id", "0"), ("nombre", "Mi grupo"), h=[
    E("partidosActivos", ("noPartidos", "1,3,5")), filtros["Dibujos"],
    E("tolerancias", h=[E("control", ("id", "0"), ("letras", "AB"), ("noAciertos", "0,1")), E("controlFallos", ("noFallos", "0"))])])]))
caso("F03-GRUPO-RT", "ArchivoCondiciones", {"archivo": "grupo", "xml": grupo}, {"igual": True},
    ["ESPEC_07 F03", "Free1X2.Domain/EntradaSalida/ArchivoCondiciones.cs:53-75,251-260,303-314"])
it = E("If-Then", h=[E("Condiciones", h=[E("Condicion", ("id", "0"), ("If", "Cantidad de Variantes: 3"), ("Then", "Dibujo: 3+2 (NO)")),
    E("Rangos", ("valor", "1"))]), E("Grupos", h=[E("Grupo", ("id", "0"), ("if", "1"), ("then", "2"), ("no-if", "False"), ("no-then", "True")),
    E("Rangos", ("valor", "0,1"))])])
TI = ["ESPEC_07 F02 (If-Then)", "Free1X2.Domain/EntradaSalida/ArchivoCondiciones.cs:85-146,329-374"]
caso("F03-IFTHEN-RT", "ArchivoCondiciones", {"archivo": "ifthen", "xml": xml(it)}, {"igual": True}, TI)
caso("F03-IFTHEN-VACIO", "ArchivoCondiciones", {"archivo": "ifthen", "xml": xml(E("If-Then", h=[E("Condiciones"), E("Grupos")]))},
    {"igual": True}, TI)
caso("F03-IFTHEN-SINRANGOS", "ArchivoCondiciones", {"archivo": "ifthen", "xml": xml(E("If-Then", h=[E("Condiciones", h=[
    E("Condicion", ("id", "0"), ("If", "a"), ("Then", "b"))])]))}, {"error": "NullReferenceException"},
    TI + ["el último hijo se lee siempre como Rangos (:108-112)"])
caso("F03-SIN-IFTHEN", "ArchivoCondiciones", {"archivo": "ifthen", "xml": xml()}, {"igual": True}, TI)

# Combinación.
equipos = ["Madrid - Barcelona", "A & B - \"C\" <D>", "Ñandú - Más"] + ["? - ?"] * 11
def comb(grupo0, extra=()):
    return xml(E("equipos", h=[E("partido", ("id", str(i)), ("equipos", e)) for i, e in enumerate(equipos)]),
        E("columnaBase", h=[E("partido", ("id", str(i)), ("pronostico", p)) for i, p in enumerate(["1,X,2", "1,X", "X", ""] + ["1"] * 10)]),
        E("filtroColumnas", ("nombreArchivo", "C:\\Datos\\filtro.txt")),
        E("gruposPartidos", h=[("#c", " grupo 0 es el boleto base "), grupo0,
            E("grupo", ("id", "1"), ("nombre", "g1"), h=[E("partidosActivos", ("noPartidos", "1,2")),
                E("filtroParcial", ("archivo", "parcial.txt")), filtros["NoVariantes"],
                E("tolerancias", h=[E("control", ("id", "0"), ("letras", "ABC"), ("noAciertos", "0,1")), E("controlFallos", ("noFallos", ""))])])]),
        E("controlGrupos", h=[E("control", ("id", "0"), ("grupos", "0"), ("fallos", "")), E("control", ("id", "1"), ("grupos", "0,1"), ("fallos", "0"))]),
        E("controlConjuntos", h=[E("control", ("id", "0"), ("conjuntos", "0"), ("fallos", "0")), E("control", ("id", "1"), ("conjuntos", "0,1"), ("fallos", "1"))]),
        *extra)
g0 = lambda *h: E("grupo", ("id", "0"), ("nombre", ""), h=list(h))
tol0 = E("tolerancias", h=[E("controlFallos", ("noFallos", ""))])
TC = ["ESPEC_07 F02, §6.2", "Free1X2.Domain/EntradaSalida/ArchivoCombinacion.cs:41-524", "Free1X2.WinUI/Views/MainPageViewModel.cs:577-648"]
caso("F02-COMB-RT", "ArchivoCombinacion", {"archivo": "comb", "xml": comb(g0(E("partidosActivos", ("noPartidos", todos[2:])),
    E("filtroParcial", ("archivo", "")), tol0), [it])}, {"igual": True}, TC)
caso("F02-COMB-FILTROPARCIAL", "ArchivoCombinacion", {"archivo": "comb", "xml": comb(g0(E("partidosActivos", ("noPartidos", "1,2")), tol0))},
    {"xml": comb(g0(E("partidosActivos", ("noPartidos", "1,2")), E("filtroParcial", ("archivo", "")), tol0))},
    TC + ["EsGrupoBase = (nº de hijos == 1): el grupo 0 se reescribe con filtroParcial (07 F02 peculiaridades)"])
caso("F02-COMB-SINBASE", "ArchivoCombinacion", {"archivo": "comb", "xml": xml(E("equipos"))}, {"error": "NullReferenceException"}, TC)

with open(sys.argv[1], "w") as f:
    for c in casos:
        f.write(json.dumps(c, ensure_ascii=False, separators=(",", ":")) + "\n")
print(len(casos))
