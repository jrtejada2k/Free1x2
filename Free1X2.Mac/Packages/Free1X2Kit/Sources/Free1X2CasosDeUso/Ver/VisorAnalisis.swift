import Foundation
import Free1X2Base
import Free1X2Motor

// C#: Views/Ported/VisorAnalisisColumnasFrmViewModel.cs (10 pantalla 102; CU-C53, CU-C54).

/// Una matriz del visor: filas con etiqueta y celdas ya en texto (`int.ToString()`).
public struct MatrizAnalisis: Sendable, Hashable {
    public struct Fila: Sendable, Hashable {
        public let etiqueta: String
        public let celdas: [String]
        public let esCabecera: Bool
    }

    public let filas: [Fila]
}

/// Bloque con título opcional (vacío = oculto).
public struct BloqueAnalisis: Sendable, Hashable {
    public let titulo: String
    public let matriz: MatrizAnalisis

    public init(titulo: String, matriz: MatrizAnalisis) { (self.titulo, self.matriz) = (titulo, matriz) }
}

/// Columna «Valor / Columnas» de la sección Valoración.
public struct ColumnaValoracionAnalisis: Sendable, Hashable {
    public struct Entrada: Sendable, Hashable {
        public let valor: String
        public let columnas: String
    }

    public let titulo: String
    public let entradas: [Entrada]
}

public enum TipoSeccionAnalisis: Sendable, Hashable {
    case vacia, matrices, valoracion
}

/// Una sección de la lista; todas llevan el marcador «●» (`Activo` nunca pasa a `false`).
public struct SeccionAnalisis: Sendable, Hashable, Identifiable {
    public let clave: String
    public let titulo: String
    public var tipo = TipoSeccionAnalisis.matrices
    public var detalle = ""
    public var bloques: [BloqueAnalisis] = []
    public var valoracion: [ColumnaValoracionAnalisis] = []
    public var id: String { clave }
}

public enum CasosVer {
    /// CU-C53 `ConstruirSecciones` (:139-196): en orden fijo, una por bandera `Analizar*` activa.
    public static func seccionesDeAnalisis(
        _ g: ContenedorAnalisisGlobal?, opciones o: OpcionesAnalisis, numeroPartidos n: Int
    ) -> [SeccionAnalisis] {
        let c = g?.analisisGrupos
        var s: [SeccionAnalisis] = []
        if o.vx2 { s.append(matriz("VX2", "VX2 — Variantes, Equis, Doses", c?.vx2, ["Variantes", "Equis", "Doses"])) }
        if o.seguidos {
            s.append(
                matriz(
                    "SignosSeguidos", "Signos Seguidos", c?.seguidos,
                    ["Variantes seguidas", "Unos seguidos", "Equis seguidas", "Doses seguidos"]))
        }
        if o.dibujos { s.append(matriz("Dibujos", "Dibujos", c?.dibujos, [])) }
        if o.interrupciones {
            let e = ["Globales", "Variantes", "Unos", "Equis", "Doses"]
            let seguidas = ["Globales seguidas", "Variantes seguidas", "Unos seguidos", "Equis seguidas", "Doses seguidos"]
            s.append(matriz("Interrupciones", "Interrupciones", c?.interrupciones, e + seguidas))
        }
        if o.contactos {
            s.append(matriz("Contactos", "Contactos", c?.contactos, ["1X", "12", "X2", "11", "XX", "22", "1V", "XV", "2V", "VV"]))
        }
        if o.pesos { s.append(matriz("Pesos", "Pesos Numéricos", c?.pesos, ["Global", "Variantes", "Unos", "Equis", "Doses"])) }
        if o.distancias { s.append(matriz("Distancias", "Distancias", c?.distancias, ["Variantes", "Unos", "Equis", "Doses"])) }
        if o.controlGrupos { s.append(vector("ControlGrupos", "Control Grupos", g?.columnasPorFallosDeGrupos)) }
        if o.controlConjuntos { s.append(vector("ControlConjuntos", "Control Conjuntos", g?.columnasPorFallosDeConjuntos)) }
        if o.simetrias { s.append(simetrias(c)) }
        if o.diferencias { s.append(diferencias(c)) }
        if o.valoracion { s.append(valoracion(c)) }
        if o.cps { s.append(columnasProbables(c, numeroPartidos: n)) }
        if o.formatos { s.append(formatos(c)) }
        if s.isEmpty {
            var v = SeccionAnalisis(clave: "SinAnalisis", titulo: "Sin análisis activos", tipo: .vacia)
            v.detalle =
                g == nil
                ? "No se ha recibido ningún análisis. Ejecuta un análisis para ver los datos."
                : "No hay ningún tipo de análisis activo (VariablesGlobales.Analizar*)."
            s.append(v)
        }
        return s
    }

    /// CU-C54 `Refrescar`: la sección con la misma clave o, si no, la primera.
    public static func seleccionTrasRefrescar(_ clave: String?, en secciones: [SeccionAnalisis]) -> String? {
        secciones.first { $0.clave == clave }?.clave ?? secciones.first?.clave
    }

    private static func vacia(_ clave: String, _ titulo: String, _ detalle: String) -> SeccionAnalisis {
        SeccionAnalisis(clave: clave, titulo: titulo, tipo: .vacia, detalle: detalle)
    }

    private static func cabecera(_ etiqueta: String, _ celdas: [String]) -> MatrizAnalisis.Fila {
        MatrizAnalisis.Fila(etiqueta: etiqueta, celdas: celdas, esCabecera: true)
    }

    private static func fila(_ etiqueta: String, _ valores: some Sequence<Int>) -> MatrizAnalisis.Fila {
        MatrizAnalisis.Fila(etiqueta: etiqueta, celdas: valores.map(String.init), esCabecera: false)
    }

    /// `SeccionMatriz` + `ConstruirMatriz` (:199-243): cabecera `0…cols−1`; sin etiqueta, «Fila i».
    private static func matriz(_ clave: String, _ titulo: String, _ m: [[Int]]?, _ etiquetas: [String]) -> SeccionAnalisis {
        guard let m else { return vacia(clave, titulo, titulo + ": no hay datos para analizar") }
        let cols = m.first?.count ?? 0
        var filas = [cabecera("", (0..<cols).map(String.init))]
        for (i, f) in m.enumerated() { filas.append(fila(etiquetas[safe: i] ?? "Fila \(i)", f)) }
        return SeccionAnalisis(
            clave: clave, titulo: titulo,
            detalle: "\(titulo): \(m.count) conceptos × \(cols) valores (cada celda = nº de columnas).",
            bloques: [BloqueAnalisis(titulo: "", matriz: MatrizAnalisis(filas: filas))])
    }

    /// `SeccionVector` (:245-268).
    private static func vector(_ clave: String, _ titulo: String, _ v: [Int]?) -> SeccionAnalisis {
        guard let v else { return vacia(clave, titulo, titulo + ": no hay datos para analizar") }
        let filas = [cabecera("Fallos", v.indices.map(String.init)), fila("Columnas", v)]
        return SeccionAnalisis(
            clave: clave, titulo: titulo, detalle: "\(titulo): columnas por nº de fallos (\(v.count) valores).",
            bloques: [BloqueAnalisis(titulo: "", matriz: MatrizAnalisis(filas: filas))])
    }

    private static func simetrias(_ c: ContenedorAnalisis?) -> SeccionAnalisis {
        guard let l = c?.simetrias, !l.isEmpty else { return vacia("Simetrias", "Simetrías", "Simetrías: No hay datos para analizar") }
        let filas = [cabecera("Nº Aciertos", l.map { String($0.aciertos) }), fila("Columnas", l.map(\.columnas))]
        return SeccionAnalisis(
            clave: "Simetrias", titulo: "Simetrías", detalle: "Simetrías: \(l.count) grupos de aciertos.",
            bloques: [BloqueAnalisis(titulo: "", matriz: MatrizAnalisis(filas: filas))])
    }

    /// `SeccionDiferencias` (:299-334): el índice 0 no se muestra.
    private static func diferencias(_ c: ContenedorAnalisis?) -> SeccionAnalisis {
        guard let l = c?.diferencias, !l.isEmpty else {
            return vacia("Diferencias", "Diferencias", "Diferencias: No hay datos para analizar")
        }
        var s = SeccionAnalisis(clave: "Diferencias", titulo: "Diferencias", detalle: "Diferencias: \(l.count) conjuntos.")
        for (i, d) in l.enumerated() {
            let cuantos = d.numDiferenciasPosibles
            func f(_ e: String, _ a: [Int]) -> MatrizAnalisis.Fila { fila(e, a.prefix(cuantos).dropFirst()) }
            let filas = [
                cabecera("Nº", (1..<max(cuantos, 1)).map(String.init)), f("Variantes", d.variantes), f("Equis", d.equis),
                f("Dibujos", d.dibujos), f("Doses", d.doses), f("Interrupciones", d.interrupciones), f("Formatos", d.formatos),
            ]
            s.bloques.append(BloqueAnalisis(titulo: "Conjunto \(i + 1)", matriz: MatrizAnalisis(filas: filas)))
        }
        return s
    }

    /// `SeccionValoracion` (:347-387): desde el primer «nº de valores» y solo los valores con columnas.
    private static func valoracion(_ c: ContenedorAnalisis?) -> SeccionAnalisis {
        guard let c, c.usaValoraciones else { return vacia("Valoracion", "Valoración", "Valoración: No hay datos para analizar") }
        let tipo =
            switch c.tipoValoracion {
            case "suma": "Por Sumas"
            case "multiplo": "Por Productos"
            default: c.tipoValoracion
            }
        func columna(_ titulo: String, _ valores: [Int], _ noValores: [Int]) -> ColumnaValoracionAnalisis {
            let inicio = max(noValores.first ?? 0, 0)
            let e = (inicio..<max(valores.count, inicio)).filter { valores[$0] != 0 }.map {
                ColumnaValoracionAnalisis.Entrada(valor: String($0), columnas: String(valores[$0]))
            }
            return ColumnaValoracionAnalisis(titulo: titulo, entradas: e)
        }
        return SeccionAnalisis(
            clave: "Valoracion", titulo: "Valoración", tipo: .valoracion,
            detalle: "Valoración: distribución de columnas por valor (" + tipo + ").",
            valoracion: [
                columna("Global", c.valoracionGlobal, c.noValoresValoracionGlobal),
                columna("Unos", c.valoracionUnos, c.noValoresValoracionUnos),
                columna("Equis", c.valoracionEquis, c.noValoresValoracionEquis),
                columna("Doses", c.valoracionDoses, c.noValoresValoracionDoses),
            ])
    }

    /// `SeccionCPs` + `MatrizCPs` (:391-441): cabecera `0…N` inclusive; lo que falta, «0».
    private static func columnasProbables(_ c: ContenedorAnalisis?, numeroPartidos n: Int) -> SeccionAnalisis {
        guard let c, c.usaCPs else {
            return vacia("ColumnasProbables", "Columnas Probables", "Columnas probables: No hay datos para analizar")
        }
        let cps = c.columnasProbables
        func bloque(_ titulo: String, _ datos: (ContenedorColumnasProbables) -> [Int]) -> BloqueAnalisis {
            var filas = [cabecera("Nº", (0...max(n, 0)).map(String.init))]
            for (i, cp) in cps.enumerated() {
                let d = datos(cp)
                filas.append(
                    MatrizAnalisis.Fila(
                        etiqueta: "CP \(i + 1)", celdas: (0...max(n, 0)).map { d[safe: $0].map(String.init) ?? "0" }, esCabecera: false))
            }
            return BloqueAnalisis(titulo: titulo, matriz: MatrizAnalisis(filas: filas))
        }
        return SeccionAnalisis(
            clave: "ColumnasProbables", titulo: "Columnas Probables",
            detalle: "Columnas probables: \(cps.count) columnas (aciertos / aciertos seguidos / fallos seguidos).",
            bloques: [bloque("Aciertos", \.noAC), bloque("Aciertos seguidos", \.noACS), bloque("Fallos seguidos", \.noFS)])
    }

    /// `SeccionFormatos` (:445-505): la cabecera de «Globales» es la posición, no la clave.
    private static func formatos(_ c: ContenedorAnalisis?) -> SeccionAnalisis {
        guard let c, c.usaFormatos else { return vacia("Formatos", "Formatos", "Formatos: No hay datos para analizar") }
        var s = SeccionAnalisis(clave: "Formatos", titulo: "Formatos", detalle: "Formatos: \(c.formatosSignos.count) grupos de formatos.")
        for (i, f) in c.formatosSignos.enumerated() {
            let l = f.aciertosFormatosSignos
            s.bloques.append(
                BloqueAnalisis(
                    titulo: "Grupo \(i + 1) — Líneas",
                    matriz: MatrizAnalisis(filas: [cabecera("Líneas", l.indices.map(String.init)), fila("Columnas", l)])))
            let globales = ContenedorAnalisis.ordenadas(f.aciertosGlobalesFormatos)
            if !globales.isEmpty {
                s.bloques.append(
                    BloqueAnalisis(
                        titulo: "Grupo \(i + 1) — Globales",
                        matriz: MatrizAnalisis(filas: [
                            cabecera("Global", globales.indices.map(String.init)), fila("Columnas", globales.map(\.valor)),
                        ])))
            }
        }
        return s
    }
}
