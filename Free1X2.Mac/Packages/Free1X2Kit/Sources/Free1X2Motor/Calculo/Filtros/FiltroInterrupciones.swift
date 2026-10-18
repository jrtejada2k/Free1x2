import Free1X2Base

// C#: Free1X2.Domain/MotorCalculo/FiltroInterrupciones.cs (02 §2).

final class FiltroInterrupciones: FiltroColumna {
    let condicion: CondicionInterrupciones
    let analisisActivo: Bool
    /// Los 10 contadores en el orden de `CondicionInterrupciones.Contador`.
    private(set) var contadores = [Int](repeating: 0, count: 10)

    init(_ condicion: CondicionInterrupciones, analisisActivo: Bool) {
        self.condicion = condicion
        self.analisisActivo = analisisActivo
    }

    var tipo: TipoFiltro { .interrupciones }
    var activa: Bool { condicion.activa }

    /// `AnalizaColumna(long)` (`INT:233-337`): los huecos se saltan sin cortar las rachas.
    static func contar(_ columna: Columna) -> [Int] {
        var (glob, vari, n1, nX, n2) = (0, 0, 0, 0, 0)
        var (gSeg, vSeg, s1, sX, s2) = (0, 0, 0, 0, 0)
        var (ngs, nvs, nus, nxs, nds) = (0, 0, 0, 0, 0)
        var c = columna
        var ant: Int64 = 0
        while c != 0 {
            while ant == 0 {
                ant = c & 7
                c >>= 3
            }
            let act = c & 7
            if act == 0 {
                c >>= 3
                continue
            }
            if act == ant {
                gSeg = max(gSeg, ngs)
                s1 = max(s1, nus)
                s2 = max(s2, nds)
                sX = max(sX, nxs)
                vSeg = max(vSeg, nvs)
                (ngs, nus, nds, nxs, nvs) = (0, 0, 0, 0, 0)
            } else {
                glob += 1
                ngs += 1
                switch ant {
                case 4:
                    n1 += 1
                    nus += 1
                    if act == 1 {
                        sX = max(sX, nxs)
                        nxs = 0
                    } else {
                        s2 = max(s2, nds)
                        nds = 0
                    }
                case 1:
                    n2 += 1
                    nds += 1
                    if act == 4 {
                        vari += 1
                        nvs += 1
                        sX = max(sX, nxs)
                        nxs = 0
                    } else {
                        s1 = max(s1, nus)
                        vSeg = max(vSeg, nvs)
                        (nus, nvs) = (0, 0)
                    }
                default:
                    nX += 1
                    nxs += 1
                    if act == 4 {
                        vari += 1
                        nvs += 1
                        s2 = max(s2, nds)
                        nds = 0
                    } else {
                        s1 = max(s1, nus)
                        vSeg = max(vSeg, nvs)
                        (nus, nvs) = (0, 0)
                    }
                }
            }
            ant = act
            c >>= 3
        }
        gSeg = max(gSeg, ngs)
        s1 = max(s1, nus)
        s2 = max(s2, nds)
        sX = max(sX, nxs)
        vSeg = max(vSeg, nvs)
        return [glob, vari, n1, nX, n2, gSeg, vSeg, s1, sX, s2]
    }

    /// `NoInterrupciones` (Glob), lo que usa `Diferencia`.
    var noInterrupciones: Int { contadores[0] }

    func analizar(_ c: Columna) throws(ErrorDominio) -> Bool {
        contadores = Self.contar(c)
        for (i, n) in contadores.enumerated() where !(try condicion.valores[i].permite(n, "FiltroInterrupciones.cs:338-355")) {
            return false
        }
        return true
    }

    func analizarFallos(_ c: Columna) throws(ErrorDominio) -> [String]? {
        contadores = Self.contar(c)
        var texto = ""
        let o = "FiltroInterrupciones.cs:357-407"
        for k in CondicionInterrupciones.Contador.allCases {
            let n = contadores[k.rawValue]
            let v = condicion.valores[k.rawValue]
            if !(try v.permite(n, o)) { texto += "Fallo en \(k.nombre)  (\(n))#" }
            if try v.letra(n, o) != ValoresConTolerancia.sinLetra { texto += "!Tolerancia en \(k.nombre)  (\(n))#" }
        }
        return Self.fallos(texto)
    }

    func obtenNoAciertosTolerancias(_ letras: String) throws(ErrorDominio) -> Int {
        var n = 0
        for letra in letras.utf16 {
            for (i, valor) in contadores.enumerated()
            where try condicion.valores[i].letra(valor, "FiltroInterrupciones.cs:436-488") == letra {
                n += 1
            }
        }
        return n
    }
}
