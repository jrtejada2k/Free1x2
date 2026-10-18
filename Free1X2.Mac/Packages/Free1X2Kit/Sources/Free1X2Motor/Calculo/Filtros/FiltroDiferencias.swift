import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/FiltroDiferencias.cs y Diferencia.cs (03 §12-13).

final class FiltroDiferencias: FiltroColumna {
    /// Contadores de una `Diferencia` tras `analizar`: nº de valores distintos entre sub-grupos.
    struct Recuentos: Equatable {
        var v = 0, x = 0, doses = 0, dib = 0, int = 0, formatos = 0
    }

    let condicion: CondicionDiferencias
    let analisisActivo: Bool
    private(set) var recuentos: [Recuentos]

    init(_ condicion: CondicionDiferencias, analisisActivo: Bool) {
        self.condicion = condicion
        self.analisisActivo = analisisActivo
        recuentos = Array(repeating: Recuentos(), count: condicion.diferencias.count)
    }

    var tipo: TipoFiltro { .diferencias }
    var activa: Bool { condicion.activa }

    /// `Diferencia.Analizar` (:30-119).
    private func analizar(_ d: Diferencia, _ i: Int, _ col: Columna) throws(ErrorDominio) -> Bool {
        var r = Recuentos()
        var vistosV = [Bool](repeating: false, count: 17)
        var vistosX = vistosV
        var vistosD = vistosV
        var vistosI = vistosV
        var vistosDib = [Bool](repeating: false, count: 17 * 17)
        var formatos = Set<Int64>()
        let o = "Diferencia.cs:30-119"
        for sel in d.partidosSimetricos {
            guard let sel else { throw .referenciaNula(o) }
            let c = UtilColumnas.ignorarPartidos(sel, col)
            if d.analizaFormatos && formatos.insert(c).inserted { r.formatos += 1 }
            if d.analizaInterrupciones {
                let k = FiltroInterrupciones.contar(c)[0]
                if !(try vistosI.leer(k, o)) {
                    r.int += 1
                    vistosI[k] = true
                }
            }
            if d.analizaVX2Dib {
                let x = UtilColumnas.contarBitsA1(c & FiltroNoVariantes.mascaraEquis)
                let dd = UtilColumnas.contarBitsA1(c & FiltroNoVariantes.mascaraDoses)
                if !(try vistosV.leer(x + dd, o)) {
                    r.v += 1
                    vistosV[x + dd] = true
                }
                if !(try vistosX.leer(x, o)) {
                    r.x += 1
                    vistosX[x] = true
                }
                if !(try vistosD.leer(dd, o)) {
                    r.doses += 1
                    vistosD[dd] = true
                }
                guard x < 17 && dd < 17 else { throw .fueraDeRango(o) }
                if !vistosDib[x * 17 + dd] {
                    r.dib += 1
                    vistosDib[x * 17 + dd] = true
                }
            }
        }
        recuentos[i] = r
        var valida = true
        if d.analizaInterrupciones { valida = try Self.ac(d.acInt, r.int) }
        if d.analizaFormatos && valida { valida = try Self.ac(d.acFormatos, r.formatos) }
        if d.analizaVX2Dib && valida {
            guard try Self.ac(d.acV, r.v) else { return false }
            guard try Self.ac(d.acX, r.x) else { return false }
            guard try Self.ac(d.acDoses, r.doses) else { return false }
            valida = try Self.ac(d.acDib, r.dib)
        }
        return valida
    }

    /// `AcX[n]`: `nil` ⇒ `NullReference`; fuera de rango ⇒ `IndexOutOfRange`.
    private static func ac(_ a: [Bool]?, _ n: Int) throws(ErrorDominio) -> Bool {
        guard let a else { throw .referenciaNula("Diferencia.cs:108") }
        return try a.leer(n, "Diferencia.cs:108")
    }

    func analizar(_ c: Columna) throws(ErrorDominio) -> Bool {
        for (i, d) in condicion.diferencias.enumerated() where !(try analizar(d, i, c)) { return false }
        return true
    }

    /// (:38-82): solo el apartado Formatos añade `#`; el texto se parte sin quitar el último (DEC-309) [PRESERVAR].
    func analizarFallos(_ c: Columna) throws(ErrorDominio) -> [String]? {
        var texto = ""
        for (i, d) in condicion.diferencias.enumerated() where !(try analizar(d, i, c)) {
            let r = recuentos[i]
            texto += "Fallos en Diferencias \(i + 1): "
            if d.analizaVX2Dib {
                if !(try Self.ac(d.acV, r.v)) { texto += "Variantes (\(r.v)) " }
                if !(try Self.ac(d.acX, r.x)) { texto += "Equis (\(r.x)) " }
                if !(try Self.ac(d.acDoses, r.doses)) { texto += "Doses (\(r.doses)) " }
                if !(try Self.ac(d.acDib, r.dib)) { texto += "Dibujos (\(r.dib)) " }
            }
            if d.analizaInterrupciones, !(try Self.ac(d.acInt, r.int)) { texto += "Interrupciones (\(r.int)) " }
            if d.analizaFormatos, !(try Self.ac(d.acFormatos, r.formatos)) { texto += "Formatos (\(r.formatos))#" }
        }
        return texto.isEmpty ? nil : trozos(texto, "#")
    }

    /// (:124-127) [PRESERVAR] (DEC-104).
    func obtenNoAciertosTolerancias(_ letras: String) throws(ErrorDominio) -> Int {
        throw ErrorDominio(.general, "The method or operation is not implemented.", origen: "FiltroDiferencias.cs:126")
    }
}
