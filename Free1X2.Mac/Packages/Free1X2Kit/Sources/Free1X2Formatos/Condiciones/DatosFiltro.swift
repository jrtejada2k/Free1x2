import Free1X2Base
import Free1X2Motor

// C#: Free1X2.Domain/EntradaSalida/DatosFiltroHelper.cs, FiltroDatosBase.cs y las 14 F*Data.cs (07 F03.a; 07 §6.4).
// Leer muta la condición en su sitio, como el C#: lo leído antes de un error se queda.

/// `<condicion id=… activa=…>` de cada filtro.
public enum DatosFiltro {
    /// `PonerCondicionesFiltro`: `IsActive`, `ContieneDatos = true` y el contenido del filtro.
    public static func leer(
        _ tipo: TipoFiltro, _ n: NodoXML, en c: inout CondicionesGrupo, numeroPartidos: Int, cultura: Cultura
    ) throws(ErrorDominio) {
        let activa = try FormatoNET.convertToBoolean(try n.atributo("activa", "F*Data.cs: activa"))
        switch tipo {
        case .noVariantes: try leerNoVariantes(n, &c.noVariantes, activa)
        case .valoracionSignos: try leerValoracion(n, &c.valoracionSignos, activa, cultura)
        case .signosSeguidos: try leerSignosSeguidos(n, &c.signosSeguidos, activa)
        case .interrupciones: try leerInterrupciones(n, &c.interrupciones, activa)
        case .dibujos:
            c.dibujos.activa = activa
            c.dibujos.contieneDatos = true
            try c.dibujos.ponerDibujos(n.hijos.map(\.textoInterior))
        case .pesosNumericos: try leerPesos(n, &c.pesosNumericos, activa)
        case .distancias: try leerDistancias(n, &c.distancias, activa)
        case .gruposEquipos: try leerGruposEquipos(n, &c.gruposEquipos, activa, numeroPartidos)
        case .contactos: try leerContactos(n, &c.contactos, activa)
        case .formatosSignos: try leerFormatosSignos(n, &c.formatosSignos, activa)
        case .colProbables: try DatosColProbables.leer(n, &c.colProbables, activa, numeroPartidos: numeroPartidos)
        case .formatos123: try leerFormatos123(n, &c.formatos123, activa, cultura)
        case .simetrias: try leerSimetrias(n, &c.simetrias, activa)
        case .diferencias: try leerDiferencias(n, &c.diferencias, activa, numeroPartidos)
        }
    }

    /// `EscribirCondicionesFiltros(grupo)`: cada filtro con datos, en el orden de la lista del grupo.
    public static func escribir(_ c: inout CondicionesGrupo, _ w: inout EscritorXMLNET, cultura: Cultura) throws(ErrorDominio) {
        for tipo in TipoFiltro.allCases { try escribir(tipo, &c, &w, cultura: cultura) }
    }

    /// `EscribirCondicionesFiltros(filtro)` de un filtro: nada si no `ContieneDatos`.
    public static func escribir(
        _ tipo: TipoFiltro, _ c: inout CondicionesGrupo, _ w: inout EscritorXMLNET, cultura: Cultura
    ) throws(ErrorDominio) {
        let activa: Bool
        switch tipo {
        case .noVariantes: guard c.noVariantes.contieneDatos else { return }; activa = c.noVariantes.activa
        case .valoracionSignos: guard c.valoracionSignos.contieneDatos else { return }; activa = c.valoracionSignos.activa
        case .signosSeguidos: guard c.signosSeguidos.contieneDatos else { return }; activa = c.signosSeguidos.activa
        case .interrupciones: guard c.interrupciones.contieneDatos else { return }; activa = c.interrupciones.activa
        case .dibujos: guard c.dibujos.contieneDatos else { return }; activa = c.dibujos.activa
        case .pesosNumericos: guard c.pesosNumericos.contieneDatos else { return }; activa = c.pesosNumericos.activa
        case .distancias: guard c.distancias.contieneDatos else { return }; activa = c.distancias.activa
        case .gruposEquipos: guard c.gruposEquipos.contieneDatos else { return }; activa = c.gruposEquipos.activa
        case .contactos: guard c.contactos.contieneDatos else { return }; activa = c.contactos.activa
        case .formatosSignos: guard c.formatosSignos.contieneDatos else { return }; activa = c.formatosSignos.activa
        case .colProbables: guard c.colProbables.contieneDatos else { return }; activa = c.colProbables.activa
        case .formatos123: guard c.formatos123.contieneDatos else { return }; activa = c.formatos123.activa
        case .simetrias: guard c.simetrias.contieneDatos() else { return }; activa = c.simetrias.activa
        case .diferencias: guard c.diferencias.contieneDatos() else { return }; activa = c.diferencias.activa
        }
        w.iniciarElemento("condicion")
        w.atributo("id", tipo.rawValue)
        w.atributo("activa", bool(activa))
        switch tipo {
        case .noVariantes:
            let v = c.noVariantes
            standard(&w, [("var", v.getVariantes), ("equis", v.getEquis), ("doses", v.getDoses)])
        case .valoracionSignos: try escribirValoracion(c.valoracionSignos, &w, cultura)
        case .signosSeguidos:
            let s = c.signosSeguidos
            standard(&w, [("var", s.get(.variantes)), ("unos", s.get(.unos)), ("equis", s.get(.equis)), ("doses", s.get(.doses))])
            for (clase, nombre) in [(0, "figurasVariantes"), (1, "figurasUnos"), (2, "figurasEquis"), (3, "figurasDoses")] {
                try figuras(&w, nombre, s.figuras[clase])
            }
        case .interrupciones:
            let i = c.interrupciones
            standard(&w, zip(Self.tiposInterrupciones, CondicionInterrupciones.Contador.allCases).map { ($0, i.get($1)) })
        case .dibujos:
            for d in c.dibujos.dibujos {
                w.iniciarElemento("dibujo")
                w.texto(d)
                w.cerrarElemento()
            }
        case .pesosNumericos: try escribirPesos(c.pesosNumericos, &w)
        case .distancias:
            let d = c.distancias
            standard(&w, [("var", d.getIntVar), ("unos", d.getInt1), ("equis", d.getIntX), ("doses", d.getInt2)])
        case .gruposEquipos: escribirGruposEquipos(c.gruposEquipos, &w)
        case .contactos:
            let k = c.contactos
            standard(&w, CondicionContactos.Contacto.allCases.map { ($0.par, k.get($0)) })
            try figuras(&w, "figuras", k.figuras ?? [])
        case .formatosSignos:
            for (i, f) in c.formatosSignos.formatosSignos.enumerated() {
                w.iniciarElemento("grupoFormato")
                w.atributo("id", String(i))
                w.atributo("lineas", f.lineas)
                w.atributo("global", f.global)
                for (j, l) in f.lineasFormatos.enumerated() {
                    elemento(&w, "lineaFormato", [("id", String(j)), ("formato", l.formato), ("rango", l.rangoAparicion)])
                }
                w.cerrarElemento()
            }
        case .colProbables: try DatosColProbables.escribir(c.colProbables, &w)
        case .formatos123: try escribirFormatos123(c.formatos123, &w, cultura)
        case .simetrias:
            let s = c.simetrias
            w.iniciarElemento("gruposimetrias")
            w.atributo("id", "0")
            w.atributo("aciertos", s.aciertos.map(String.init).joined(separator: ","))
            for (j, x) in s.simetrias.enumerated() { elemento(&w, "simetrias", [("id", String(j)), ("simetria", x.partidos)]) }
            w.cerrarElemento()
        case .diferencias: try escribirDiferencias(c.diferencias, &w)
        }
        w.cerrarElemento()
    }

    // MARK: Ayudantes

    /// `bool.ToString()`.
    static func bool(_ b: Bool) -> String { b ? "True" : "False" }

    /// `FiltroDatosBase.GetValores`: `""` ⇒ `nil`; si no, `Convert.ToInt32` de cada trozo **antes** de usarlos.
    static func valores(_ s: String) throws(ErrorDominio) -> [Int32]? {
        guard !s.isEmpty else { return nil }
        var r: [Int32] = []
        for t in trozos(s, ",") { r.append(try FormatoNET.convertToInt32(t)) }
        return r
    }

    static func elemento(_ w: inout EscritorXMLNET, _ nombre: String, _ atributos: [(String, String)]) {
        w.iniciarElemento(nombre)
        for (k, v) in atributos { w.atributo(k, v) }
        w.cerrarElemento()
    }

    /// `<standard>` con un `<valores tipo values>` por par.
    static func standard(_ w: inout EscritorXMLNET, _ pares: [(String, String)], elemento: String = "standard") {
        w.iniciarElemento(elemento)
        for (t, v) in pares { Self.elemento(&w, "valores", [("tipo", t), ("values", v)]) }
        w.cerrarElemento()
    }

    /// `<nombre><figura value="a-b"/>…</nombre>` solo si hay figuras.
    static func figuras(_ w: inout EscritorXMLNET, _ nombre: String, _ lista: [Int64]) throws(ErrorDominio) {
        guard !lista.isEmpty else { return }
        w.iniciarElemento(nombre)
        for f in lista {
            elemento(&w, "figura", [("value", try UtilidadesEntradasValores.obtenerTextoFiguraFromLong(f))])
        }
        w.cerrarElemento()
    }

    /// Hijos `figura value` ⇒ `List<long>`.
    static func leerFiguras(_ n: NodoXML, _ o: String) throws(ErrorDominio) -> [Int64] {
        var r: [Int64] = []
        for h in n.hijos { r.append(try UtilidadesEntradasValores.obtenerLongFiguraFromText(try h.atributo("value", o))) }
        return r
    }

    /// Cada hijo `valores`: lee `values` y `tipo` (en ese orden) y lo aplica antes de pasar al siguiente.
    static func recorrer(
        _ n: NodoXML, _ o: String, _ cuerpo: (_ tipo: String, _ valores: String) throws(ErrorDominio) -> Void
    ) throws(ErrorDominio) {
        for h in n.hijos {
            let v = try h.atributo("values", o)
            try cuerpo(try h.atributo("tipo", o), v)
        }
    }

    static let tiposInterrupciones = ["global", "var", "unos", "equis", "doses", "globalSeg", "varSeg", "unosSeg", "equisSeg", "dosesSeg"]

    // MARK: Lectores

    /// `FNoVariantesData` (:31-69): los hijos de **cualquier** hijo de `condicion`; `tipo` desconocido se ignora.
    static func leerNoVariantes(_ n: NodoXML, _ f: inout CondicionNoVariantes, _ activa: Bool) throws(ErrorDominio) {
        f.activa = activa
        f.contieneDatos = true
        for h in n.hijos {
            for g in h.hijos {
                let t = try g.atributo("tipo", "FNoVariantesData.cs:52")
                let v = try g.atributo("values", "FNoVariantesData.cs:54")
                switch t {
                case "var": try f.setNoVariantes(v)
                case "equis": try f.setNoEquis(v)
                case "doses": try f.setNoDoses(v)
                default: break
                }
            }
        }
    }

    /// `FValoracionData` (:29-60): atributos extra obligatorios; un `valores` por hijo con `"v1,vX,v2"` (punto ⇒ coma).
    static func leerValoracion(
        _ n: NodoXML, _ f: inout CondicionValoracionSignos, _ activa: Bool, _ cultura: Cultura
    ) throws(ErrorDominio) {
        let o = "FValoracionData.cs:34-52"
        f.activa = activa
        f.contieneDatos = true
        f.tipoValoracion = try n.atributo("tipoValoracion", o)
        try f.ponerValorGlobal(try n.atributo("valorGlobal", o), cultura: cultura)
        try f.ponerValorUnos(try n.atributo("valorUnos", o), cultura: cultura)
        try f.ponerValorEquis(try n.atributo("valorEquis", o), cultura: cultura)
        try f.ponerValorDoses(try n.atributo("valorDoses", o), cultura: cultura)
        var v1: [Double] = []
        var vX: [Double] = []
        var v2: [Double] = []
        func numero(_ p: [String], _ i: Int) throws(ErrorDominio) -> Double {
            try FormatoNET.convertToDouble(try p.leer(i, o).replacingOccurrences(of: ".", with: ","), cultura: cultura)
        }
        for h in n.hijos {
            let p = trozos(FormatoNET.recortar(h.textoInterior), ",")
            v1.append(try numero(p, 0))
            vX.append(try numero(p, 1))
            v2.append(try numero(p, 2))
        }
        (f.valores1, f.valoresX, f.valores2) = (v1, vX, v2)
    }

    /// `FSignosSeguidosData` (:32-165).
    static func leerSignosSeguidos(_ n: NodoXML, _ f: inout CondicionSignosSeguidos, _ activa: Bool) throws(ErrorDominio) {
        let o = "FSignosSeguidosData.cs:69-159"
        f.activa = activa
        f.contieneDatos = true
        for h in n.hijos {
            switch h.nombre {
            case "standard":
                try recorrer(h, o) { t, v throws(ErrorDominio) in
                    switch t {
                    case "var": try f.set(.variantes, v)
                    case "unos": try f.set(.unos, v)
                    case "equis": try f.set(.equis, v)
                    case "doses": try f.set(.doses, v)
                    default: break
                    }
                }
            case "figurasVariantes": f.figuras[0] = try leerFiguras(h, o)
            case "figurasUnos": f.figuras[1] = try leerFiguras(h, o)
            case "figurasEquis": f.figuras[2] = try leerFiguras(h, o)
            case "figurasDoses": f.figuras[3] = try leerFiguras(h, o)
            default: break
            }
        }
    }

    /// `FInterrupcionesData` (:35-134): sin ningún `*Seg` (ficheros 0.23) se rellenan con `"0,…,14"` (fijo).
    static func leerInterrupciones(_ n: NodoXML, _ f: inout CondicionInterrupciones, _ activa: Bool) throws(ErrorDominio) {
        f.activa = activa
        f.contieneDatos = true
        for h in n.hijos where h.nombre == "standard" {
            var seguidas = false
            try recorrer(h, "FInterrupcionesData.cs:67-69") { t, v throws(ErrorDominio) in
                guard let k = tiposInterrupciones.firstIndex(of: t),
                    let contador = CondicionInterrupciones.Contador(rawValue: k)
                else { return }
                try f.set(contador, v)
                if k >= 5 { seguidas = true }
            }
            if !seguidas {
                for c in [CondicionInterrupciones.Contador.globalSeg, .variantesSeg, .unosSeg, .equisSeg, .dosesSeg] {
                    try f.set(c, "0,1,2,3,4,5,6,7,8,9,10,11,12,13,14")
                }
            }
        }
    }

    /// `FPesosNumericosData` (:32-154): `noTolerancias=""` ⇒ `PonerTolerancia(null)` ⇒ NRE [PRESERVAR] (02 §1 P8).
    static func leerPesos(_ n: NodoXML, _ f: inout CondicionPesosNumericos, _ activa: Bool) throws(ErrorDominio) {
        let o = "FPesosNumericosData.cs:66-148"
        let pesos = [
            "global": CondicionPesosNumericos.Peso.global, "variantes": .variantes, "unos": .unos, "equis": .equis, "doses": .doses,
        ]
        f.activa = activa
        f.contieneDatos = true
        for h in n.hijos {
            switch h.nombre {
            case "standard", "tolerancias":
                let tol = h.nombre == "tolerancias"
                if tol {
                    guard let t = try valores(try h.atributo("noTolerancias", o)) else {
                        throw .referenciaNula("FiltroPesosNumericos.cs:520")
                    }
                    for x in t { try f.ponerTolerancia(Int(x), true) }
                }
                for g in h.hijos {
                    let v = try valores(try g.atributo("values", o))
                    let t = try g.atributo("tipo", o)
                    guard let v, let p = pesos[t] else { continue }
                    let texto = v.map(String.init).joined(separator: ",")
                    if tol { try f.setPNTol(p, texto) } else { try f.setPN(p, texto) }
                }
            case "figuras": f.figuras = try leerFiguras(h, o)
            default: break
            }
        }
    }

    /// `FDistanciasData` (:35-87): solo `standard`.
    static func leerDistancias(_ n: NodoXML, _ f: inout CondicionDistancias, _ activa: Bool) throws(ErrorDominio) {
        f.activa = activa
        f.contieneDatos = true
        for h in n.hijos where h.nombre == "standard" {
            try recorrer(h, "FDistanciasData.cs:60-62") { t, v throws(ErrorDominio) in
                switch t {
                case "var": try f.setDistVariantes(v)
                case "unos": try f.setDistUnos(v)
                case "equis": try f.setDistEquis(v)
                case "doses": try f.setDistDoses(v)
                default: break
                }
            }
        }
    }

    /// `FGruposEquiposData` (:33-129): `Name.ToUpper()`; tras leer cada grupo, `CalcularLongPronosticos`.
    static func leerGruposEquipos(
        _ n: NodoXML, _ f: inout CondicionGruposEquipos, _ activa: Bool, _ numeroPartidos: Int
    ) throws(ErrorDominio) {
        let o = "FGruposEquiposData.cs:77-121"
        f.activa = activa
        f.contieneDatos = true
        var grupos: [GrupoEquiposConfig] = []
        var relaciones: [RelacionGE1Config] = []
        for h in n.hijos {
            switch FormatoNET.mayusculas(h.nombre) {
            case "GRUPOEQUIPOS":
                var ge = GrupoEquiposConfig(numeroPartidos: numeroPartidos)
                for g in h.hijos {
                    switch g.nombre {
                    case "equipos": ge.pronosticos = try g.atributo("value", o)
                    case "victorias": try ge.ponerVictorias(try g.atributo("value", o))
                    case "empates": try ge.ponerEmpates(try g.atributo("value", o))
                    case "derrotas": try ge.ponerDerrotas(try g.atributo("value", o))
                    case "sumaPuntos": try ge.ponerSumaPuntos(try g.atributo("value", o))
                    default: break
                    }
                }
                let pronosticos = ge.pronosticos
                ge.pronosticos = pronosticos  // CalcularLongPronosticos(), también sin `equipos`.
                grupos.append(ge)
            case "RELACION1":
                var r = RelacionGE1Config()
                for g in h.hijos {
                    switch g.nombre {
                    case "ge": try r.ponerGruposEquipos(try g.atributo("values", o))
                    case "victorias": try r.ponerSumaVictorias(try g.atributo("values", o))
                    case "empates": try r.ponerSumaEmpates(try g.atributo("values", o))
                    case "derrotas": try r.ponerSumaDerrotas(try g.atributo("values", o))
                    case "puntos": try r.ponerSumaPuntos(try g.atributo("values", o))
                    default: break
                    }
                }
                relaciones.append(r)
            default: break
            }
        }
        f.ponerGrupos(grupos)
        f.relaciones = relaciones
    }

    /// `FContactosData` (:35-137).
    static func leerContactos(_ n: NodoXML, _ f: inout CondicionContactos, _ activa: Bool) throws(ErrorDominio) {
        let o = "FContactosData.cs:65-128"
        f.activa = activa
        f.contieneDatos = true
        for h in n.hijos {
            switch h.nombre {
            case "standard":
                try recorrer(h, o) { t, v throws(ErrorDominio) in
                    if let c = CondicionContactos.Contacto.allCases.first(where: { $0.par == t }) { try f.set(c, v) }
                }
            case "figuras": f.figuras = try leerFiguras(h, o)
            default: break
            }
        }
    }

    /// `FFormatosSignosData` (:33-70): todos los hijos de `condicion`, sin `standard`.
    static func leerFormatosSignos(_ n: NodoXML, _ f: inout CondicionFormatosSignos, _ activa: Bool) throws(ErrorDominio) {
        let o = "FFormatosSignosData.cs:48-59"
        f.activa = activa
        f.contieneDatos = true
        var lista: [FormatosSignos] = []
        for h in n.hijos {
            var fs = FormatosSignos()
            try fs.ponerLineas(try h.atributo("lineas", o))
            try fs.ponerGlobal(try h.atributo("global", o))
            for g in h.hijos {
                var l = FormatoSignos()
                l.ponerFormato(try g.atributo("formato", o))
                try l.ponerRangoAparicion(try g.atributo("rango", o))
                fs.lineasFormatos.append(l)
            }
            lista.append(fs)
        }
        f.ponerFormatosSignos(lista)
    }

    /// `FFormatos123Data` (:18-67): valoración fija de 14×3 en la cultura actual (sin cambiar el punto).
    static func leerFormatos123(
        _ n: NodoXML, _ f: inout CondicionFormatos123, _ activa: Bool, _ cultura: Cultura
    ) throws(ErrorDominio) {
        let o = "FFormatos123Data.cs:31-58"
        f.activa = activa
        var formatos: [Formato123] = []
        var valoracion = [[Double]](repeating: [0, 0, 0], count: 14)
        for h in n.hijos {
            let a = try h.atributo("aciertos", o)
            f.aciertosFiltro = a.isEmpty ? "0" : a
            f.pasoFijo = try FormatoNET.convertToBoolean(try h.atributo("repeticiones", o))
            for (l, v) in trozos(try h.atributo("valoracion", o), "/").enumerated() {
                let d = try FormatoNET.convertToDouble(v, cultura: cultura)
                guard l / 3 < 14 else { throw .fueraDeRango("FFormatos123Data.cs:42") }
                valoracion[l / 3][l % 3] = d
            }
            for g in h.hijos {
                let formato = try g.atributo("formato", o)
                let mn = Int(try FormatoNET.convertToInt32(try g.atributo("aciertosMin", o)))
                let mx = Int(try FormatoNET.convertToInt32(try g.atributo("aciertosMax", o)))
                formatos.append(Formato123(formato: formato, aciertosMin: mn, aciertosMax: mx))
            }
        }
        f.formatos = formatos
        guard let aciertos = f.aciertosFiltro else { throw .referenciaNula("FFormatos123Data.cs:200") }
        f.aciertos = try trozos(aciertos, ",").map { s throws(ErrorDominio) in Int(try FormatoNET.convertToInt32(s)) }
        f.valoracion = valoracion
        f.valoracionTransformada = try CondicionFormatos123.transformar(valoracion)
    }

    /// `FSimetriasData` (:16-47).
    static func leerSimetrias(_ n: NodoXML, _ f: inout CondicionSimetrias, _ activa: Bool) throws(ErrorDominio) {
        let o = "FSimetriasData.cs:28-38"
        f.activa = activa
        var lista: [Simetria] = []
        for h in n.hijos {
            let a = try h.atributo("aciertos", o)
            f.aciertosTexto = a.isEmpty ? "0" : a
            for g in h.hijos { lista.append(try Simetria(try g.atributo("simetria", o))) }
        }
        f.simetrias = lista
        f.aciertos = try trozos(f.aciertosTexto, ",").map { s throws(ErrorDominio) in Int(try FormatoNET.convertToInt32(s)) }
    }

    /// `FDiferenciasData` (:14-98): `aciertos` usa los `partidos` ya leídos; `""` ⇒ «no analizar» y todo a `true`.
    static func leerDiferencias(
        _ n: NodoXML, _ f: inout CondicionDiferencias, _ activa: Bool, _ numeroPartidos: Int
    ) throws(ErrorDominio) {
        let o = "FDiferenciasData.cs:33-87"
        f.activa = activa
        f.ponerContieneDatos(true)
        var lista: [Diferencia] = []
        for h in n.hijos {
            var d = Diferencia()
            for g in h.hijos {
                switch g.nombre {
                case "aciertos":
                    let t = d.partidosSimetricos.count + 1
                    func leer(_ a: String) throws(ErrorDominio) -> [Bool]? {
                        try UtilidadesEntradasValores.obtenerBoolArrayFromTxt(try g.atributo(a, o), longitud: t)
                    }
                    d.acV = try leer("aciertosV")
                    d.acX = try leer("aciertosX")
                    d.acDoses = try leer("aciertos2")
                    d.acDib = try leer("aciertosDib")
                    d.acInt = try leer("aciertosInt")
                    d.acFormatos = try leer("aciertosFormatos")
                    let todos = [Bool](repeating: true, count: t)
                    d.analizaVX2Dib = !(d.acV == nil && d.acX == nil && d.acDoses == nil && d.acDib == nil)
                    d.analizaInterrupciones = d.acInt != nil
                    d.analizaFormatos = d.acFormatos != nil
                    d.acInt = d.acInt ?? todos
                    d.acFormatos = d.acFormatos ?? todos
                    d.acV = d.acV ?? todos
                    d.acX = d.acX ?? todos
                    d.acDoses = d.acDoses ?? todos
                    d.acDib = d.acDib ?? todos
                case "partidos":
                    d.partidosSimetricos.append(
                        UtilidadesEntradasValores.obtenerBoolArrayFromTxt(
                            try g.atributo("valores", o), numeroPartidos: numeroPartidos))
                default: break
                }
            }
            lista.append(d)
        }
        f.diferencias = lista
    }

    // MARK: Escritores

    static func escribirValoracion(
        _ f: CondicionValoracionSignos, _ w: inout EscritorXMLNET, _ cultura: Cultura
    ) throws(ErrorDominio) {
        w.atributo("tipoValoracion", f.tipoValoracion)
        w.atributo("valorGlobal", f.valorGlobal)
        w.atributo("valorUnos", f.valorUnos)
        w.atributo("valorEquis", f.valorEquis)
        w.atributo("valorDoses", f.valorDoses)
        func texto(_ d: Double) -> String { FormatoNET.toString(d, cultura: cultura).replacingOccurrences(of: ",", with: ".") }
        for i in f.valores1.indices {
            let o = "FValoracionData.cs:93-94"
            let s = texto(f.valores1[i]) + "," + texto(try f.valoresX.leer(i, o)) + "," + texto(try f.valores2.leer(i, o))
            w.iniciarElemento("valores")
            w.atributo("id", String(i))
            w.texto(s)
            w.cerrarElemento()
        }
    }

    static func escribirPesos(_ f: CondicionPesosNumericos, _ w: inout EscritorXMLNET) throws(ErrorDominio) {
        let tipos: [(String, CondicionPesosNumericos.Peso)] = [
            ("global", .global), ("variantes", .variantes), ("unos", .unos), ("equis", .equis), ("doses", .doses),
        ]
        standard(&w, tipos.map { ($0.0, f.getPN($0.1)) })
        w.iniciarElemento("tolerancias")
        w.atributo("noTolerancias", f.getTolerancias)
        for (t, p) in tipos { elemento(&w, "valores", [("tipo", t), ("values", f.getPNTol(p))]) }
        w.cerrarElemento()
        try figuras(&w, "figuras", f.figuras)
    }

    /// Los 5 comentarios fijos (empiezan por espacio y no acaban en él) y luego grupos y relaciones.
    static func escribirGruposEquipos(_ f: CondicionGruposEquipos, _ w: inout EscritorXMLNET) {
        for c in [
            " Para cada partido:", " 0 => ningun equipo seleccionado", " 1 => solo equipo casa seleccionado",
            " 2 => solo equipo fuera seleccionado", " 3 => equipos casa y fuera seleccionados",
        ] { w.comentario(c) }
        for (i, ge) in f.grupos.enumerated() {
            w.iniciarElemento("grupoEquipos")
            w.atributo("id", String(i))
            elemento(&w, "equipos", [("value", ge.pronosticos)])
            elemento(&w, "victorias", [("value", ge.victorias.texto)])
            elemento(&w, "empates", [("value", ge.empates.texto)])
            elemento(&w, "derrotas", [("value", ge.derrotas.texto)])
            elemento(&w, "sumaPuntos", [("value", ge.sumaPuntos.texto)])
            w.cerrarElemento()
        }
        for (i, r) in f.relaciones.enumerated() {
            w.iniciarElemento("Relacion1")
            w.atributo("id", String(i))
            elemento(&w, "ge", [("values", r.gruposEquipos)])
            elemento(&w, "victorias", [("values", r.sumaVictorias.texto)])
            elemento(&w, "empates", [("values", r.sumaEmpates.texto)])
            elemento(&w, "derrotas", [("values", r.sumaDerrotas.texto)])
            elemento(&w, "puntos", [("values", r.sumaPuntos.texto)])
            w.cerrarElemento()
        }
    }

    /// Un único `grupoformato`; la valoración son 14×3 `ToString()` en la cultura actual separados por `/`.
    static func escribirFormatos123(
        _ f: CondicionFormatos123, _ w: inout EscritorXMLNET, _ cultura: Cultura
    ) throws(ErrorDominio) {
        var v: [String] = []
        for i in 0..<14 {
            let fila = try f.valoracion.leer(i, "FFormatos123Data.cs:188")
            for j in 0..<3 { v.append(FormatoNET.toString(try fila.leer(j, "FFormatos123Data.cs:188"), cultura: cultura)) }
        }
        w.iniciarElemento("grupoformato")
        w.atributo("id", "0")
        w.atributo("aciertos", f.aciertos.map(String.init).joined(separator: ","))
        w.atributo("repeticiones", bool(f.pasoFijo))
        w.atributo("valoracion", v.joined(separator: "/"))
        for (j, x) in f.formatos.enumerated() {
            elemento(
                &w, "formatos",
                [("id", String(j)), ("formato", x.formato), ("aciertosMin", String(x.aciertosMin)), ("aciertosMax", String(x.aciertosMax))])
        }
        w.cerrarElemento()
    }

    static func escribirDiferencias(_ f: CondicionDiferencias, _ w: inout EscritorXMLNET) throws(ErrorDominio) {
        let o = "FDiferenciasData.cs:140-172"
        func texto(_ a: [Bool]?) throws(ErrorDominio) -> String {
            guard let a else { throw .referenciaNula(o) }
            return UtilidadesEntradasValores.obtenerTextoFromBool(a)
        }
        for (i, d) in f.diferencias.enumerated() {
            w.iniciarElemento("grupoDiferencias")
            w.atributo("id", String(i))
            for (j, p) in d.partidosSimetricos.enumerated() {
                elemento(&w, "partidos", [("id", String(j)), ("valores", try texto(p))])
            }
            w.iniciarElemento("aciertos")
            for (nombre, a) in [("aciertosV", d.acV), ("aciertosX", d.acX), ("aciertos2", d.acDoses), ("aciertosDib", d.acDib)] {
                w.atributo(nombre, d.analizaVX2Dib ? try texto(a) : "")
            }
            w.atributo("aciertosInt", d.analizaInterrupciones ? try texto(d.acInt) : "")
            w.atributo("aciertosFormatos", d.analizaFormatos ? try texto(d.acFormatos) : "")
            w.cerrarElemento()
            w.cerrarElemento()
        }
    }
}
