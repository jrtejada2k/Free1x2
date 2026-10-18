import Foundation
import Free1X2Base
import Free1X2Formatos

// C#: Views/Ported/ImprimirBoletoFrmViewModel.cs (09 pantalla 68; CU-B36) y ListaImpresorasViewModel.cs (pantalla 70;
// CU-B38).

/// Un boleto: cabecera y marcas (rectángulos) en la unidad de impresión del legacy.
public struct BoletoImpresion: Sendable, Hashable {
    public struct Marca: Sendable, Hashable {
        public let x, y, ancho, alto: Double
    }

    public var encabezado: String
    public var marcas: [Marca]
}

extension CasosCombinacion {
    public static func rutaConfigImpresion(_ entorno: Entorno) -> URL { entorno.rutas.base.appending(path: "Impresion/imprebol.cfg") }

    /// `RecuperarConfiguracionInterno`: `nil` = no se pudo (sin fichero, menos de 4 líneas o error).
    public static func leerConfigImpresion(
        previo: ArchivoConfigImpresion.Config, entorno: Entorno
    ) -> ArchivoConfigImpresion.Config? {
        let url = rutaConfigImpresion(entorno)
        guard entorno.archivos.existe(url), let d = try? entorno.archivos.leer(url) else { return nil }
        return ArchivoConfigImpresion.leer(d, previo: previo, cultura: entorno.cultura)
    }

    public static func guardarConfigImpresion(_ c: ArchivoConfigImpresion.Config, entorno: Entorno) throws(ErrorDominio) {
        let url = rutaConfigImpresion(entorno)
        try entorno.archivos.crearCarpeta(url.deletingLastPathComponent())
        try entorno.archivos.escribir(ArchivoConfigImpresion.datos(c), en: url, atomico: false)
    }

    /// Lista de impresoras conocidas (`impresoras.cfg` de los recursos), por modelo según la cultura.
    public static func impresorasConocidas(entorno: Entorno) throws(ErrorDominio) -> [ArchivoImpresoras.Impresora] {
        let d = try entorno.archivos.leer(entorno.rutas.recursos.appending(path: "Datos/Impresion/impresoras.cfg"))
        return try ArchivoImpresoras.leer(d).sorted { $0.modelo.compare($1.modelo, locale: .current) == .orderedAscending }
    }

    /// `BuscarImpresora`: por modelo; si no está, la última.
    public static func buscarImpresora(_ modelo: String, en lista: [ArchivoImpresoras.Impresora]) -> ArchivoImpresoras.Impresora? {
        lista.first { $0.modelo == modelo } ?? lista.last
    }

    /// `ComputarBoletos` (= `imprimirBoleto.cs:519-559`): 8 columnas por boleto; márgenes como `float`.
    public static func boletosImpresion(
        _ cols: [String], fichero: String, margenSuperior: Double, margenIzquierdo: Double, desde: Int, hasta: Int
    ) -> [BoletoImpresion] {
        let (mgy, mgx) = (Double(Float(margenSuperior)), Double(Float(margenIzquierdo)))
        let minbol = max(1, desde)
        let maxbol = max(minbol, hasta)
        let maxcol = min(maxbol * 8, cols.count)
        var mincol = (minbol - 1) * 8
        if mincol == maxcol { mincol -= 1 }
        var nwcol = max(0, mincol)
        var r: [BoletoImpresion] = []
        while nwcol < maxcol {
            var marcas: [BoletoImpresion.Marca] = []
            let primera = Array(cols[nwcol].utf16)
            let p15 = primera.count > 14 ? primera[14] : 0x31
            for nw8 in 0..<8 {
                let columna = Array(cols[nwcol].utf16)
                nwcol += 1
                for pa in 0..<min(14, columna.count) {
                    let inc = columna[pa] == 0x31 ? 0 : columna[pa] == 0x58 ? 14.764 : 29.528
                    marcas.append(
                        .init(x: mgx - 29 + Double(14 - pa) * 19.685, y: mgy + 83 + Double(nw8) * 44.291 + inc, ancho: 6, alto: 4))
                }
                if nwcol == cols.count { break }
                if nwcol + 2 == cols.count && nw8 == 6 { break }
            }
            marcas.append(.init(x: mgx - 30, y: p15 == 0x31 ? mgy + 245 : p15 == 0x32 ? mgy + 305 : mgy + 275, ancho: 6, alto: 4))
            r.append(BoletoImpresion(encabezado: "\(fichero)   Boleto \(minbol + r.count)", marcas: marcas))
        }
        return r
    }
}
