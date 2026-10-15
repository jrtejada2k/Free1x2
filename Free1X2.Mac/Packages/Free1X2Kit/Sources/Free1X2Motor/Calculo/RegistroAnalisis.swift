import Free1X2Base

// C#: Free1X2.Domain/Analisis/ContenedorAnalisis.cs:109-344 (06 §3.5) — la parte que mira los filtros vivos.

/// Estado de un análisis en curso: el contenedor y los filtros del grupo base registrados en la columna actual.
final class RegistroAnalisis {
    var global: ContenedorAnalisisGlobal
    let figurasContactos: Bool
    let figurasPesos: Bool
    let figurasV1X2: Bool
    /// `FiltrosTemp`: orden de inserción, sin repetir por identidad.
    private var filtrosTemp: [any FiltroColumna] = []
    private var vistos = Set<ObjectIdentifier>()

    init(_ global: ContenedorAnalisisGlobal, analisis: OpcionesAnalisis) {
        self.global = global
        figurasContactos = analisis.figurasContactos
        figurasPesos = analisis.figurasPesos
        figurasV1X2 = analisis.figurasV1X2
    }

    /// `AñadirFiltroTemp` (:338-344).
    func anadirFiltroTemp(_ f: any FiltroColumna) {
        if vistos.insert(ObjectIdentifier(f)).inserted { filtrosTemp.append(f) }
    }

    /// `VaciarInformacion` / `VaciarFiltrosTemp`.
    func vaciarInformacion() {
        filtrosTemp.removeAll()
        vistos.removeAll()
    }

    /// `IncrementarContador` (:109-334): lee el estado que dejó el último `analizar` de cada filtro registrado.
    func incrementarContador(_ columna: Columna) throws(ErrorDominio) {
        let o = "ContenedorAnalisis.cs:109-334"
        for f in filtrosTemp where f.analisisActivo {
            switch f {
            case let x as FiltroNoVariantes:
                try global.analisisGrupos.vx2.incrementar(0, x.noEquis + x.noDoses, o)
                try global.analisisGrupos.vx2.incrementar(1, x.noEquis, o)
                try global.analisisGrupos.vx2.incrementar(2, x.noDoses, o)
            case let x as FiltroContactos:
                for (k, n) in x.contadores.enumerated() { try global.analisisGrupos.contactos.incrementar(k, n, o) }
                if figurasContactos { global.analisisGrupos.figurasContactos.incrementar(x.figura) }
            case let x as FiltroDistancias:
                let d = [x.distVariantes, x.distUnos, x.distEquis, x.distDoses]
                for (k, n) in d.enumerated() { try global.analisisGrupos.distancias.incrementar(k, n, o) }
            case let x as FiltroInterrupciones:
                for (k, n) in x.contadores.enumerated() { try global.analisisGrupos.interrupciones.incrementar(k, n, o) }
            case let x as FiltroPesosNumericos:
                for (k, n) in x.pesos.enumerated() { try global.analisisGrupos.pesos.incrementar(k, n, o) }
                if figurasPesos {
                    x.obtenerFigura()
                    global.analisisGrupos.figurasPesos.incrementar(x.figura)
                }
            case let x as FiltroSignosSeguidos:
                for (k, n) in x.rachas.enumerated() { try global.analisisGrupos.seguidos.incrementar(k, n, o) }
                if figurasV1X2 {
                    // Las figuras se calculan sobre la columna entera, no la del grupo.
                    global.analisisGrupos.figurasV.incrementar(FiltroSignosSeguidos.figura(columna, .variantes))
                    global.analisisGrupos.figuras1.incrementar(FiltroSignosSeguidos.figura(columna, .unos))
                    global.analisisGrupos.figurasX.incrementar(FiltroSignosSeguidos.figura(columna, .equis))
                    global.analisisGrupos.figuras2.incrementar(FiltroSignosSeguidos.figura(columna, .doses))
                }
            case let x as FiltroDibujos:
                try global.analisisGrupos.dibujos.incrementar(x.noEquis, x.noDoses, o)
            case let x as FiltroSimetrias:
                global.analisisGrupos.incrementarColumnasSimetrias(x.aciertos)
            case let x as FiltroValoracionSignos where x.activa:
                try global.analisisGrupos.incrementarColumnasValoracion(
                    x.valoracion, x.valoracionUnos, x.valoracionEquis, x.valoracionDoses)
                global.analisisGrupos.tipoValoracion = x.condicion.tipoValoracion
            case let x as FiltroColProbables where x.activa:
                for (j, cp) in x.cps.enumerated() {
                    let c = "ContenedorAnalisis.cs:271-284"
                    guard global.analisisGrupos.columnasProbables.indices.contains(j) else { throw .fueraDeRangoLista(c) }
                    try global.analisisGrupos.columnasProbables[j].noAC.incrementar(cp.noAC, c)
                    try global.analisisGrupos.columnasProbables[j].noACS.incrementar(cp.noACS, c)
                    try global.analisisGrupos.columnasProbables[j].noFS.incrementar(cp.noFS, c)
                }
            case let x as FiltroFormatosSignos where x.activa:
                for j in x.lineasValidasGrupo.indices {
                    let c = "ContenedorAnalisis.cs:285-302"
                    guard global.analisisGrupos.formatosSignos.indices.contains(j) else { throw .fueraDeRangoLista(c) }
                    try global.analisisGrupos.formatosSignos[j].aciertosFormatosSignos.incrementar(x.lineasValidasGrupo[j], c)
                    global.analisisGrupos.formatosSignos[j].aciertosGlobalesFormatos.incrementar(x.aparicionesGrupo[j])
                }
            case let x as FiltroDiferencias where x.activa:
                for (j, d) in x.condicion.diferencias.enumerated() {
                    let c = "ContenedorAnalisis.cs:303-331"
                    guard global.analisisGrupos.diferencias.indices.contains(j) else { throw .fueraDeRangoLista(c) }
                    let r = x.recuentos[j]
                    if d.analizaVX2Dib {
                        try global.analisisGrupos.diferencias[j].variantes.incrementar(r.v, c)
                        try global.analisisGrupos.diferencias[j].equis.incrementar(r.x, c)
                        try global.analisisGrupos.diferencias[j].doses.incrementar(r.doses, c)
                        try global.analisisGrupos.diferencias[j].dibujos.incrementar(r.dib, c)
                    }
                    if d.analizaInterrupciones { try global.analisisGrupos.diferencias[j].interrupciones.incrementar(r.int, c) }
                    if d.analizaFormatos { try global.analisisGrupos.diferencias[j].formatos.incrementar(r.formatos, c) }
                }
            default:
                break
            }
        }
        vaciarInformacion()
    }
}
