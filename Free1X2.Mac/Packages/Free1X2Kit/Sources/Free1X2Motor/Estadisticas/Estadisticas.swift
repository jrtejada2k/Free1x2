import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/Estadisticas/*.cs (03 §16-§20) y la agregación de `AnastaticsViewModel.cs:222-298`.

/// Cumplimiento de un filtro sobre un fichero de columnas (porcentaje en `Float`, T3).
public struct Estadistica: Sendable, Hashable {
    public var archivo: String
    public var cumplimiento: Float

    public init(archivo: String, cumplimiento: Float) {
        self.archivo = archivo
        self.cumplimiento = cumplimiento
    }
}

public enum CalculadorEstadisticas {
    /// `EstadisticasFiltro(filtro, directorio)` (:37-52) con el filtro temporal de la pantalla: el de `tipo` construido
    /// con `condiciones`, que se analiza **sin mirar si está activo**. `ficheros` = nombre y columnas ya leídas
    /// (`LeeColumnaSinComas`), en el orden que fije el llamador.
    public static func estadisticas(
        _ tipo: TipoFiltro, condiciones: CondicionesGrupo, configuracion: Configuracion, cultura: Cultura = .esES,
        ficheros: [(nombre: String, lineas: [String])]
    ) throws(ErrorDominio) -> [Estadistica] {
        let x = ContextoFiltros(
            numeroPartidos: configuracion.boleto.numeroPartidos, analisis: configuracion.analisis, cultura: cultura,
            puntosCP: configuracion.puntosCP)
        guard let f = Grupo.filtros(condiciones, x).first(where: { $0.tipo == tipo }) else { return [] }
        var r: [Estadistica] = []
        for (nombre, lineas) in ficheros {
            r.append(Estadistica(archivo: nombre, cumplimiento: try cumplimiento(f, lineas)))
        }
        return r
    }

    /// `ObtenerEstadisticasFiltro` (:11-36): una línea en blanco cuenta (columna 0); `validas / (float)total · 100`.
    static func cumplimiento(_ f: any FiltroColumna, _ lineas: [String]) throws(ErrorDominio) -> Float {
        var validas = 0
        for l in lineas where try f.analizar(UtilColumnas.convStrToLong(l)) { validas += 1 }
        guard !lineas.isEmpty else { return 0 }
        return Float(validas) / Float(lineas.count) * 100
    }
}

/// Estadísticas de «Anastatics» sobre columnas de texto de 14 signos (03 §17-§20.1).
public enum Anastatics {
    public enum Modo: Int, Sendable, CaseIterable {
        case dibujos, dibRepes, staInter, staSigSeg
    }

    /// Tope de columnas leídas (`AnastaticsViewModel.cs:107`).
    public static let maximoColumnas = 500_000

    /// `VerColumna`: exactamente 14 signos de `12xX`, con `x` → `X`; `nil` si es errónea.
    public static func verColumna(_ c: String) -> String? {
        let u = Array(c.utf16)
        guard u.count == 14, u.allSatisfy({ [0x31, 0x32, 0x78, 0x58].contains($0) }) else { return nil }
        return String(decoding: u.map { $0 == 0x78 ? 0x58 : $0 }, as: UTF16.self)
    }

    /// `Proceso(modo, columnas)`: matriz 15 × 15 acumulada con la salida del procesador de cada columna.
    public static func proceso(_ modo: Modo, columnas: [String]) throws(ErrorDominio) -> [[Int]] {
        var rsl = [[Int]](repeating: [Int](repeating: 0, count: 15), count: 15)
        let o = "AnastaticsViewModel.cs:222-283"
        switch modo {
        case .dibujos:
            for c in columnas {
                let ind = try dibujos(c)
                try rsl.incrementar(ind[3], ind[4], o)
            }
        case .dibRepes:
            var rep = DibRepes()
            var (num1, numx, num2) = (0, 0, 0)
            for c in columnas {
                let ind = try rep.procesar(c)
                try rsl.incrementar(0, ind[0], o)
                if num1 == ind[2] { try rsl.incrementar(1, ind[2], o) }
                if numx == ind[3] { try rsl.incrementar(2, ind[3], o) }
                if num2 == ind[4] { try rsl.incrementar(3, ind[4], o) }
                if numx + num2 == ind[3] + ind[4] { try rsl.incrementar(4, numx + num2, o) }
                (num1, numx, num2) = (ind[2], ind[3], ind[4])
            }
        case .staInter:
            for c in columnas {
                let ind = try staInter(c)
                for (fila, k) in [(0, 0), (1, 2), (2, 3), (3, 4), (4, 1)] { try rsl.incrementar(fila, ind[k], o) }
            }
        case .staSigSeg:
            for c in columnas {
                let ind = try staSigSeg(c)
                for (fila, k) in [(0, 2), (1, 3), (2, 4), (3, 1)] { try rsl.incrementar(fila, ind[k], o) }
            }
        }
        return rsl
    }

    /// Los 14 primeros signos; menos ⇒ `IndexOutOfRange`.
    static func signos(_ c: String) throws(ErrorDominio) -> [UInt16] {
        let u = Array(c.utf16)
        guard u.count >= 14 else { throw .fueraDeRango("Estadisticas:14") }
        return Array(u.prefix(14))
    }

    /// `Dibujos.Procesar` (`Dibujos.cs:22-35`): `[0, 0, 0, nX, n2]`; todo lo que no es `1` ni `2` cuenta como `X`.
    public static func dibujos(_ c: String) throws(ErrorDominio) -> [Int] {
        var (nx, n2) = (0, 0)
        for ch in try signos(c) where ch != 0x31 {
            if ch == 0x32 { n2 += 1 } else { nx += 1 }
        }
        return [0, 0, 0, nx, n2]
    }

    /// `DibRepes` (`DibRepes.cs:21-40`): coincidencias exactas con la columna anterior (la primera, con 14 ceros).
    public struct DibRepes: Sendable {
        var anterior = Array("00000000000000".utf16)

        public init() {}

        /// `[coincidencias, 0, n1, nX, n2]`.
        public mutating func procesar(_ c: String) throws(ErrorDominio) -> [Int] {
            let s = try signos(c)
            var (np, n1, nx, n2) = (0, 0, 0, 0)
            for (i, ch) in s.enumerated() {
                if ch == anterior[i] { np += 1 }
                switch ch {
                case 0x31: n1 += 1
                case 0x32: n2 += 1
                default: nx += 1
                }
            }
            anterior = Array(c.utf16)
            return [np, 0, n1, nx, n2]
        }
    }

    /// `StaInter.Procesar` (`StaInter.cs:22-39`): `[cambios, hacia 1, desde 1, desde X, desde 2]`.
    public static func staInter(_ c: String) throws(ErrorDominio) -> [Int] {
        let s = try signos(c)
        var ch0 = s[0]
        var (ng, nv, n1, nx, n2) = (0, 0, 0, 0, 0)
        for ch in s.dropFirst() where ch != ch0 {
            switch ch0 {
            case 0x31: n1 += 1
            case 0x32: n2 += 1
            default: nx += 1
            }
            ng += 1
            if ch == 0x31 { nv += 1 }
            ch0 = ch
        }
        return [ng, nv, n1, nx, n2]
    }

    /// `StaSigSeg.Procesar` (`StaSigSeg.cs:22-75`), máquina de estados literal: `[0, mV, m1, mX, m2]`.
    public static func staSigSeg(_ c: String) throws(ErrorDominio) -> [Int] {
        let s = try signos(c)
        let uno: UInt16 = 0x31
        let dos: UInt16 = 0x32
        var (m1, mx, m2, mv) = (0, 0, 0, 0)
        var (n1, nx, n2, nv) = (0, 0, 0, 0)
        var ch0 = s[0]
        if ch0 == uno { n1 += 1 } else if ch0 == dos { n2 += 1 } else { nx += 1 }
        if ch0 != uno { nv += 1 }
        for ch in s.dropFirst() {
            if ch == ch0 {
                if ch == uno { n1 += 1 } else if ch == dos { n2 += 1 } else { nx += 1 }
                if ch != uno { nv += 1 }
            } else if ch0 == uno {
                m1 = max(m1, n1)
                n1 = 0
                nv = 1
                if ch == dos { n2 = 1 } else { nx = 1 }
            } else if ch0 == dos {
                m2 = max(m2, n2)
                n2 = 0
                if ch == uno {
                    mv = max(mv, nv)
                    n1 = 1
                    nv = 0
                } else {
                    nx = 1
                    nv += 1
                }
            } else {
                mx = max(mx, nx)
                nx = 0
                if ch == dos {
                    n2 = 1
                    nv += 1
                } else {
                    mv = max(mv, nv)
                    n1 = 1
                    nv = 0
                }
            }
            ch0 = ch
        }
        return [0, max(mv, nv), max(m1, n1), max(mx, nx), max(m2, n2)]
    }
}
