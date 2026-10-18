import Foundation
import Free1X2Base
import Free1X2Formatos

// C#: Views/Ported/AgregaP15FrmViewModel.cs (08 pantalla 1; CU-A01).

/// Los cuatro modos de «Añade P15 (Indeciso)»; combinables como en el C# (DEC-801).
public struct OpcionesP15: Sendable, Hashable {
    public var fijo = false
    public var condicional = false
    public var eliminar = false
    public var copiar = false
    public var signoFijo = ""
    /// 1-based.
    public var partidoCondicion = 1
    public var signoCondicion = ""
    public var signoSi = ""
    public var signoNo = ""
    /// 1-based.
    public var partidoCopia = 1

    public init() {}
}

extension CasosCombinacion {
    /// `ComprobarEntradas` (:113-132): mensajes concatenados y recortados; `""` = correcto.
    public static func comprobarP15(entrada: String, salida: String, _ o: OpcionesP15) -> String {
        var error = ""
        if salida.isEmpty || entrada.isEmpty {
            error = "Uno de los archivos necesarios no ha sido especificado\n"
        } else if salida == entrada {
            error = "El archivo de salida debe ser distinto al de entrada\n"
        }
        if !o.fijo && !o.condicional && !o.eliminar && !o.copiar { error += "No se ha especificado qué hacer con el Pleno al 15" }
        return FormatoNET.recortar(error)
    }

    /// `EjecutarAgregaP15` (:194-353): devuelve las repetidas del modo 3. Los modos 2 y 4 sacan las columnas en bloques
    /// de 8 por signo del P15 y los restos al final (1, X, 2); un P15 que no es 1/X/2 se pierde. Sin columnas que grabar
    /// no se crea el fichero; con error no se graba nada.
    @concurrent public static func agregarP15(
        entrada: URL, salida: URL, opciones o: OpcionesP15, entorno: Entorno
    ) async throws(ErrorDominio) -> Int {
        let origen = "AgregaP15FrmViewModel.cs"
        var lineas: [String] = []
        var buffers: [[String]] = [[], [], []]
        var repetidas = 0
        var vistas = ConjuntoBits(cantidad: 4_782_969)
        let conv = ConvertidorDeBases(numeroPartidos: 14)

        func encolar(_ c: String) throws(ErrorDominio) {
            let u = Array(c.utf16)
            guard u.count >= 15 else { throw .fueraDeRangoLista(origen + ":Substring") }
            switch u[14] {
            case 0x31: buffers[0].append(c)
            case 0x58, 0x78: buffers[1].append(c)
            case 0x32: buffers[2].append(c)
            default: break
            }
            if let b = buffers.firstIndex(where: { $0.count == 8 }) {
                lineas += buffers[b]
                buffers[b] = []
            }
        }
        func signo(_ c: String, _ partido: Int) throws(ErrorDominio) -> UInt16 {
            try Array(c.utf16).leer(partido - 1, origen)
        }

        for var c in ArchivoColumnasTexto.columnas(try entorno.archivos.leer(entrada)) {
            if o.fijo {
                lineas.append(c + o.signoFijo)
                continue
            }
            if o.condicional {
                // `char.Parse(signoCondicion)`.
                let s = Array(o.signoCondicion.utf16)
                guard s.count == 1 else {
                    throw ErrorDominio(.formato, "La cadena debe tener una longitud de exactamente un carácter.", origen: origen)
                }
                c += try signo(c, o.partidoCondicion) == s[0] ? o.signoSi : o.signoNo
                try encolar(c)
            }
            if o.eliminar {
                guard c.utf16.count >= 14 else { throw .fueraDeRangoLista(origen + ":Substring") }
                c = String(decoding: c.utf16.prefix(14), as: UTF16.self)
                let n = Int(try conv.numero(c))
                if try vistas.contiene(n) {
                    repetidas += 1
                } else {
                    try vistas.poner(n)
                    lineas.append(c)
                }
            }
            if o.copiar {
                c += String(decoding: [try signo(c, o.partidoCopia)], as: UTF16.self)
                try encolar(c)
            }
        }
        lineas += buffers.joined()
        if let d = ArchivoColumnasTexto.datos(lineas) { try entorno.archivos.escribir(d, en: salida, atomico: false) }
        return repetidas
    }
}
