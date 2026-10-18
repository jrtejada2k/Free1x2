import Free1X2Base

// C#: Free1X2.Domain/Reduccion/JDC.cs, JDCdobleContador.cs, Redu1305Xfsf.cs y ReductorBase.cs (05 §1-§2).
// ReductorTM, XFSF, XfsfV3 y JLPM no se portan (DP-08, DEC-507).

/// Los reductores que ofrece la pantalla (`ReductorFrmViewModel.cs:174-181`).
public enum AlgoritmoReduccion: Sendable, Hashable, CaseIterable {
    /// «Menos tiempo»: `JDC`.
    case menosTiempo
    /// «Menos columnas»: `JDCDobleContador`.
    case menosColumnas
    /// «Grandes archivos»: `Redu1305Xfsf`.
    case grandesArchivos
}

/// Contadores y columnas reductoras. `lineas` vacía ⇒ el C# no crea ni toca el fichero de salida.
public struct ResultadoReduccion: Sendable, Hashable {
    public let iniciales: Int
    public let finales: Int
    public let procesadas: Int
    public let lineas: [String]
}

public enum Reduccion {
    /// `ComienzaReduccion(entrada, salida, nivel, maxCol, percent)` de una instancia nueva. `lineas` = columnas de la
    /// madre ya leídas (`LeeColumnaSinComas`: recortadas y en mayúsculas; las vacías cuentan como `1…1`); el nº de signos
    /// es el de la primera. `numeroPartidos` (configuración) da la anchura de la salida y la profundidad de
    /// «grandes archivos». Cancelar la tarea equivale a `Cancelar()`: la salida es la cobertura parcial.
    public static func reducir(
        _ algoritmo: AlgoritmoReduccion, lineas: [String], nivel: Int, maxCol: Int = 0, percent: Int = 100,
        externas: Bool = false, numeroPartidos: Int, progreso: ReportadorProgreso = .ninguno
    ) throws(ErrorDominio) -> ResultadoReduccion {
        let r = try Reductor(lineas, externas: externas, algoritmo: algoritmo, progreso: progreso)
        defer { r.liberar() }
        switch algoritmo {
        case .menosTiempo: try r.reduceJDC(nivel, maxCol, percent)
        case .menosColumnas: try r.reduceDoble(nivel, maxCol, percent)
        case .grandesArchivos: try r.reduce1305(nivel, maxCol, percent, numeroPartidos)
        }
        let conv = ConvertidorDeBases(numeroPartidos: numeroPartidos)
        var salida: [String] = []
        for i in try r.reductoras(algoritmo) { salida.append(try conv.columna(Int64(i))) }
        progreso.terminar(Progreso(procesadas: r.procesadas, total: r.iniciales, aceptadas: r.finales))
        return ResultadoReduccion(iniciales: r.iniciales, finales: r.finales, procesadas: r.procesadas, lineas: salida)
    }
}

/// Estado de una reducción sobre el espacio 3^n. Memoria propia (`flags` llega a 4,8 M enteros con 14 partidos) con
/// punteros sin comprobación en los recorridos: los índices de vecinos de un índice válido siempre son válidos.
private final class Reductor {
    static let pot: [Int] = [
        1, 3, 9, 27, 81, 243, 729, 2187, 6561, 19683, 59049, 177_147, 531_441, 1_594_323, 4_782_969, 14_348_907, 43_046_721,
    ]
    // Marcas de `flags` en JDC.
    static let muerta: Int32 = 20_000_000
    static let reducida: Int32 = 30_000_000
    static let reductora: Int32 = 40_000_000

    let noPartidos: Int
    let indices: Int
    let admiteExternas: Bool
    let progreso: ReportadorProgreso
    /// Índices de la madre en orden de fichero (ordenados en JDCDobleContador), con duplicados; luego las externas.
    var columnas: [Int] = []
    var flags: UnsafeMutableBufferPointer<Int32>
    var bits: UnsafeMutableBufferPointer<Bool>
    var bitsExternos: UnsafeMutableBufferPointer<Bool>
    /// JDCDobleContador.
    var reducidaB: UnsafeMutableBufferPointer<Bool>
    var reductoraB: UnsafeMutableBufferPointer<Bool>
    var poorN: UnsafeMutableBufferPointer<Int32>
    var pobresTocados: [Int] = []
    var iniciales = 0
    var finales = 0
    var procesadas = 0
    var numExternas = 0
    var cancelado = false

    /// `EntradaDeDatos`: `flags[num] = 1` (no acumula duplicados), pero cada línea entra en `columnas`.
    init(_ lineas: [String], externas: Bool, algoritmo: AlgoritmoReduccion, progreso: ReportadorProgreso)
        throws(ErrorDominio)
    {
        let doble = algoritmo == .menosColumnas
        admiteExternas = externas
        self.progreso = progreso
        noPartidos = lineas.first?.utf16.count ?? 0
        indices = try Self.pot.leer(noPartidos, "JDC.cs:57")
        flags = .allocate(capacity: indices)
        flags.initialize(repeating: 0)
        bits = .allocate(capacity: indices)
        bits.initialize(repeating: false)
        bitsExternos = .allocate(capacity: indices)
        bitsExternos.initialize(repeating: false)
        reducidaB = .allocate(capacity: doble ? indices : 0)
        reducidaB.initialize(repeating: false)
        // `Reductora` en JDCDobleContador, `validas` en «grandes archivos».
        reductoraB = .allocate(capacity: algoritmo == .menosTiempo ? 0 : indices)
        reductoraB.initialize(repeating: false)
        poorN = .allocate(capacity: doble ? indices : 0)
        poorN.initialize(repeating: 0)
        let conv = ConvertidorDeBases(numeroPartidos: noPartidos)
        for l in lineas {
            let num = Int(try conv.numero(l))
            guard num >= 0 && num < indices else { throw .fueraDeRango("JDC.cs:229") }
            flags[num] = 1
            columnas.append(num)
            bits[num] = true
        }
        iniciales = columnas.count
        if doble { columnas.sort() }
    }

    func liberar() {
        for p in [flags, poorN] { p.deallocate() }
        for p in [bits, bitsExternos, reducidaB, reductoraB] { p.deallocate() }
    }

    /// `UiPump.Pump()` + `if (salida)`: progreso y cancelación cooperativa (una vez cancelada, sigue cancelada).
    func bombear() -> Bool {
        progreso.reportar(Progreso(procesadas: procesadas, total: iniciales, aceptadas: finales))
        if Task.isCancelled { cancelado = true }
        return cancelado
    }

    /// `ncent = procesadas·100 / iniciales` (división entera; con 0 iniciales, `DivideByZero`).
    func porcentaje() throws(ErrorDominio) -> Int {
        guard iniciales != 0 else { throw ErrorDominio(.divisionPorCero, "Intento de dividir por cero.", origen: "JDC.cs:135") }
        return procesadas * 100 / iniciales
    }

    /// Vecinos a distancia 1…`prof` (05 §1.3): partidos `desde…hasta` (1-based, inclusivo), los dos signos distintos
    /// del actual; `accion` decide si recursa tras actuar (`nivel < prof` va **después**: siempre al menos un nivel).
    @inline(__always)
    func vecinos(_ ini: Int, _ desde: Int, _ hasta: Int, _ cuerpo: (Int, Int) -> Void) {
        var p = desde
        while p <= hasta {
            let pot = Self.pot[p - 1]
            let sig = (ini / pot) % 3
            for z in 0..<3 where z != sig { cuerpo(ini + pot * (z - sig), p) }
            p += 1
        }
    }

    // MARK: JDC

    /// `Reduce` (`JDC.cs:73-138`).
    func reduceJDC(_ nivel: Int, _ maxColIn: Int, _ percent: Int) throws(ErrorDominio) {
        let d = noPartidos - nivel
        let maxCol = maxColIn == 0 ? iniciales : maxColIn
        for nr in 0..<iniciales {
            calculaJDC(columnas[nr], 1, d, 1, 1)
            procesadas = nr - iniciales
            if bombear() { break }
        }
        procesadas = 0
        var max: Int32 = -1
        var primera = 0
        var ultima = iniciales + numExternas
        while true {
            if bombear() { break }
            var nextcol = -1
            var min: Int32 = -1
            var nr = primera
            while nr < ultima {
                let nv = flags[columnas[nr]]
                if nv > Self.muerta {
                    nr += 1
                    continue
                }
                if nv == max && nr < iniciales {
                    min = nv
                    nextcol = nr
                    break
                }
                if nv > min {
                    min = nv
                    nextcol = nr
                }
                nr += 1
            }
            if nextcol < 0 { break }
            max = min
            while flags[columnas[primera]] > Self.muerta { primera += 1 }
            while flags[columnas[ultima - 1]] > Self.muerta { ultima -= 1 }
            let c = columnas[nextcol]
            flags[c] = Self.reductora
            finales += 1
            if bits[c] { procesadas += 1 }
            marcaJDC(c, 1, d, 1)
            let ncent = try porcentaje()
            if finales == maxCol || ncent >= percent { break }
        }
    }

    /// `CalculaLasQueReduce` (`JDC.cs:140-179`): las externas se dan de alta al vuelo.
    func calculaJDC(_ ini: Int, _ pos: Int, _ prof: Int, _ delta: Int32, _ nivel: Int) {
        vecinos(ini, pos, noPartidos) { idx, p in
            if bits[idx] {
                flags[idx] &+= delta
            } else if admiteExternas {
                if !bitsExternos[idx] {
                    numExternas += 1
                    bitsExternos[idx] = true
                    columnas.append(idx)
                }
                flags[idx] &+= delta
            }
            if nivel < prof { calculaJDC(idx, p + 1, prof, delta, nivel + 1) }
        }
    }

    /// `MarcarReducidas` (`JDC.cs:181-212`): resta 1 a **todo** visitado.
    func marcaJDC(_ ini: Int, _ pos: Int, _ prof: Int, _ nivel: Int) {
        vecinos(ini, pos, noPartidos) { idx, p in
            flags[idx] &-= 1
            if bits[idx] && flags[idx] <= Self.muerta {
                flags[idx] = Self.reducida
                procesadas += 1
                calculaJDC(idx, 1, prof, -1, 1)
            }
            if nivel < prof { marcaJDC(idx, p + 1, prof, nivel + 1) }
        }
    }

    // MARK: JDCDobleContador

    /// `Reduce` (`JDCdobleContador.cs:94-200`).
    func reduceDoble(_ nivel: Int, _ maxColIn: Int, _ percent: Int) throws(ErrorDominio) {
        let d = noPartidos - nivel
        let maxCol = maxColIn == 0 ? iniciales : maxColIn
        for nr in 0..<iniciales {
            calculaDoble(columnas[nr], 1, d, reducidaB[columnas[nr]] ? -1 : 1, 1)
            procesadas = nr - iniciales
            if bombear() { break }
        }
        procesadas = 0
        var primera = 0
        var ultima = iniciales + numExternas
        while true {
            var max: Int32 = 0
            var rmin: Int32 = 100_000
            var rmayor: Int32 = 0
            var nr = primera
            while nr < iniciales {
                let v = columnas[nr]
                if !reducidaB[v] {
                    max = Swift.max(max, flags[v])
                    if flags[v] > 1 { rmin = Swift.min(rmin, flags[v]) }
                }
                nr += 1
            }
            if max == 0 { break }
            nr = primera
            while nr < ultima {
                let v = columnas[nr]
                // `14 − nivel` literal, no `noPartidos − nivel` (05 §2.2 P5).
                if bits[v] && !reducidaB[v] && flags[v] > 1 && flags[v] == rmin { dispersa(v, 1, 14 - nivel, 1) }
                nr += 1
            }
            if bombear() { break }
            var nextcol = -1
            var min = -1
            nr = primera
            while nr < ultima {
                let v = columnas[nr]
                let nv = flags[v]
                let nnr = Int(poorN[v]) + (nv == max ? 1 : 0)
                if !reductoraB[v] {
                    if nnr > min {
                        min = nnr
                        nextcol = nr
                        rmayor = nv
                    }
                    if nnr == min && nv > rmayor {
                        nextcol = nr
                        rmayor = nv
                    }
                }
                nr += 1
            }
            for t in pobresTocados { poorN[t] = 0 }
            pobresTocados.removeAll(keepingCapacity: true)
            if nextcol < 0 { break }
            while reducidaB[columnas[primera]] { primera += 1 }
            while reducidaB[columnas[ultima - 1]] { ultima -= 1 }
            let c = columnas[nextcol]
            if bits[c] && !reducidaB[c] { procesadas += 1 }
            reductoraB[c] = true
            reducidaB[c] = true
            finales += 1
            marcaDoble(c, 1, d, 1)
            let ncent = try porcentaje()
            if finales == maxCol || ncent >= percent { break }
        }
    }

    /// `DispersaColumnaPobre` (`JDCdobleContador.cs:210-229`).
    func dispersa(_ ini: Int, _ pos: Int, _ prof: Int, _ nivel: Int) {
        vecinos(ini, pos, noPartidos) { idx, p in
            poorN[idx] &+= 1
            pobresTocados.append(idx)
            if nivel < prof { dispersa(idx, p + 1, prof, nivel + 1) }
        }
    }

    /// `CalculaLasQueReduce` (`JDCdobleContador.cs:231-270`): las reductoras quedan congeladas (salvo externas).
    func calculaDoble(_ ini: Int, _ pos: Int, _ prof: Int, _ delta: Int32, _ nivel: Int) {
        vecinos(ini, pos, noPartidos) { idx, p in
            if bits[idx] {
                if !reductoraB[idx] { flags[idx] &+= delta }
            } else if admiteExternas {
                if !bitsExternos[idx] {
                    numExternas += 1
                    bitsExternos[idx] = true
                    columnas.append(idx)
                }
                flags[idx] &+= delta
            }
            if nivel < prof { calculaDoble(idx, p + 1, prof, delta, nivel + 1) }
        }
    }

    /// `MarcarReducidas` (`JDCdobleContador.cs:272-305`): **excluye el último partido** (`Partido < noPartidos`)
    /// [PRESERVAR, 05 §2.2 P1].
    func marcaDoble(_ ini: Int, _ pos: Int, _ prof: Int, _ nivel: Int) {
        vecinos(ini, pos, noPartidos - 1) { idx, p in
            if reducidaB[idx] { flags[idx] &-= 1 }
            if bits[idx] && !reducidaB[idx] {
                reducidaB[idx] = true
                flags[idx] &-= 2
                procesadas += 1
                calculaDoble(idx, 1, prof, -1, 1)
            }
            if nivel < prof { marcaDoble(idx, p + 1, prof, nivel + 1) }
        }
    }

    // MARK: Redu1305Xfsf

    /// `Reduce`/`ReduceConExternas` (`Redu1305Xfsf.cs:66-155`, idénticos): elige en orden de índice, marca a distancia
    /// `N − nivel` y señala las influenciadas hasta el doble; después, las influenciadas que queden.
    func reduce1305(_ nivel: Int, _ maxColIn: Int, _ percent: Int, _ numeroPartidos: Int) throws(ErrorDominio) {
        let o = "Redu1305Xfsf.cs:66-155"
        let prof = numeroPartidos - nivel
        procesadas = 0
        finales = 0
        let maxCol = maxColIn == 0 ? iniciales : maxColIn
        var primera = 0
        var nsel = 0
        while nsel < indices && flags[nsel] != 1 { nsel += 1 }
        while true {
            if bombear() { break }
            // `validas[nsel]` con `nsel == indices` (madre vacía): `BitArray` lanza `ArgumentOutOfRange`.
            guard nsel < indices else { throw .fueraDeRangoLista(o) }
            reductoraB[nsel] = true
            finales += 1
            flags[nsel] = -1
            procesadas += 1
            marca1305(nsel, 0, prof, 1)
            nsel = primera
            while nsel < indices {
                if flags[nsel] == 1 {
                    primera = nsel + 1
                    break
                }
                nsel += 1
            }
            if nsel == indices { break }
            let ncent = try porcentaje()
            if finales >= maxCol || ncent >= percent { break }
        }
        for nr in 0..<indices where flags[nr] == 2 {
            if bombear() { break }
            reductoraB[nr] = true
            finales += 1
            flags[nr] = -1
            procesadas += 1
            marcar1(nr, 0, prof, 1)
            let ncent = try porcentaje()
            if finales >= maxCol || ncent >= percent { break }
        }
    }

    /// Vecinos de «grandes archivos»: partidos 0-based `desde..<noPartidos` (todos).
    @inline(__always)
    func vecinos0(_ ini: Int, _ desde: Int, _ cuerpo: (Int, Int) -> Void) {
        vecinos(ini, desde + 1, noPartidos) { idx, p in cuerpo(idx, p - 1) }
    }

    /// `MarcarReducidas` (`Redu1305Xfsf.cs:157-180`): el contador `Profundidad` es compartido con
    /// `MarcarInfluenciadas`, que sigue desde el nivel actual hasta `2·prof`.
    func marca1305(_ ini: Int, _ pos: Int, _ prof: Int, _ nivel: Int) {
        vecinos0(ini, pos) { idx, p in
            if flags[idx] > 0 {
                flags[idx] = -1
                procesadas += 1
            }
            if nivel < prof {
                marca1305(idx, p + 1, prof, nivel + 1)
            } else {
                influenciadas(idx, p + 1, 2 * prof, nivel + 1)
            }
        }
    }

    /// `Marcar1` (`Redu1305Xfsf.cs:181-201`).
    func marcar1(_ ini: Int, _ pos: Int, _ prof: Int, _ nivel: Int) {
        vecinos0(ini, pos) { idx, p in
            if flags[idx] > 0 {
                flags[idx] = -1
                procesadas += 1
            }
            if nivel < prof { marcar1(idx, p + 1, prof, nivel + 1) }
        }
    }

    /// `MarcarInfluenciadas` (`Redu1305Xfsf.cs:202-219`).
    func influenciadas(_ ini: Int, _ pos: Int, _ limite: Int, _ nivel: Int) {
        vecinos0(ini, pos) { idx, p in
            if flags[idx] > 0 { flags[idx] = 2 }
            if nivel < limite { influenciadas(idx, p + 1, limite, nivel + 1) }
        }
    }

    // MARK: Salida

    /// `GrabacionDeReductoras`: JDC en orden de `columnas` (fichero; ordenado en JDCDobleContador) repitiendo duplicados;
    /// «grandes archivos» en orden de índice.
    func reductoras(_ algoritmo: AlgoritmoReduccion) throws(ErrorDominio) -> [Int] {
        switch algoritmo {
        case .menosTiempo:
            return columnas.prefix(iniciales + numExternas).filter { flags[$0] > Self.reducida }
        case .menosColumnas:
            return columnas.prefix(iniciales + numExternas).filter { reductoraB[$0] }
        case .grandesArchivos:
            return (0..<indices).filter { reductoraB[$0] }
        }
    }
}
