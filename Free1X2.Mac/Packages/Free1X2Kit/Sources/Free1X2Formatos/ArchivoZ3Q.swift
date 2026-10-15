import Compression
import Foundation
import Free1X2Base
import zlib

// C#: Free1X2.Domain/Utils/CompresorZip.cs (06 §2.14; ESPEC_16 §3; F16). ZIP de una entrada con el bitmap de columnas:
// contenedor propio; escritura con `compress2` de la zlib del sistema (respeta el nivel, DT-62) y lectura con
// Compression.framework.

/// Ficheros `.z3q`.
public enum ArchivoZ3Q {
    /// `new BitArray(14348907)` del lector: 3^15.
    static let maximo = 14_348_907

    /// `Comprimir(bs, fichZip, fichBin, tamano, nivel)` (:41-85): `⌊tamano/8⌋ + 1` bytes con el bit `i` en el bit `i % 8`
    /// del byte `i / 8`; devuelve el fichero y esa longitud sin comprimir (lo que devuelve el C#).
    public static func comprimir(
        _ bits: ConjuntoBits, tamano: Int, nombreEntrada: String = "columnas.bin", fecha: Date, nivel: Int = 5
    ) throws(ErrorDominio) -> (datos: Data, tamano: Int) {
        var carga = [UInt8](repeating: 0, count: tamano / 8 + 1)
        // Los bits de más allá del `BitArray` lanzan y el `catch` vacío los deja a 0 (:52-61).
        for i in bits.indices where i < carga.count * 8 { carga[i >> 3] |= 1 << UInt8(i & 7) }
        let crc = crc32(carga)
        let comprimido = try deflate(carga, nivel: nivel)
        let nombre = Array(nombreEntrada.utf8)
        let (hora, dia) = fechaDOS(fecha)
        var z = Data()
        func u16(_ v: Int) { z.append(contentsOf: [UInt8(v & 0xFF), UInt8(v >> 8 & 0xFF)]) }
        func u32(_ v: UInt32) { z.append(contentsOf: (0..<4).map { UInt8(v >> (8 * $0) & 0xFF) }) }
        func comun() {
            u16(20)  // versión necesaria
            u16(0)  // banderas: tamaños y CRC en la cabecera, sin descriptor
            u16(8)  // Deflate
            u16(hora)
            u16(dia)
            u32(crc)
            u32(UInt32(comprimido.count))
            u32(UInt32(carga.count))
            u16(nombre.count)
            u16(0)
        }
        u32(0x0403_4B50)
        comun()
        z.append(contentsOf: nombre)
        z.append(contentsOf: comprimido)
        let inicioDirectorio = z.count
        u32(0x0201_4B50)
        u16(20)  // versión del creador
        comun()
        u16(0)  // comentario
        u16(0)  // disco
        u16(0)  // atributos internos
        u32(0)  // atributos externos
        u32(0)  // desplazamiento de la cabecera local
        z.append(contentsOf: nombre)
        let tamDirectorio = z.count - inicioDirectorio
        u32(0x0605_4B50)
        u16(0)
        u16(0)
        u16(1)
        u16(1)
        u32(UInt32(tamDirectorio))
        u32(UInt32(inicioDirectorio))
        u16(0)
        return (z, carga.count)
    }

    /// `Descomprimir(fichero)` (:88-136): la **primera** entrada (directorio ⇒ nada leído; ninguna ⇒ NRE); un bit a 1 en
    /// o más allá de 3^15 lanza (`BitArray.Set`); el resultado mide 3^15 si se leyeron más de 3^15 bits y si no 3^14.
    public static func descomprimir(_ datos: Data) throws(ErrorDominio) -> ConjuntoBits {
        let carga = try primeraEntrada([UInt8](datos))
        var bits = ConjuntoBits(cantidad: maximo)
        for (i, b) in carga.enumerated() where b != 0 {
            for p in 0..<8 where b & (1 << UInt8(p)) != 0 { try bits.poner(i * 8 + p) }
        }
        let real = carga.count * 8 > maximo ? maximo : 4_782_969
        var salida = ConjuntoBits(cantidad: real)
        for i in bits.indices where i < real { try salida.poner(i) }
        return salida
    }

    static func error(_ motivo: String) -> ErrorDominio {
        ErrorDominio(.entradaSalida, "El fichero comprimido no es válido: \(motivo).", origen: "CompresorZip.cs:94-120")
    }

    /// Localiza el fin del directorio central, toma la primera entrada y la extrae (método 0 u 8; CRC comprobado).
    static func primeraEntrada(_ z: [UInt8]) throws(ErrorDominio) -> [UInt8] {
        func u16(_ i: Int) -> Int { i + 1 < z.count ? Int(z[i]) | Int(z[i + 1]) << 8 : -1 }
        func u32(_ i: Int) -> Int { i + 3 < z.count ? u16(i) | u16(i + 2) << 16 : -1 }
        let firma: [UInt8] = [0x50, 0x4B, 0x05, 0x06]
        guard z.count >= 22,
            let fin = stride(from: z.count - 22, through: max(0, z.count - 22 - 65_535), by: -1).first(where: {
                Array(z[$0..<$0 + 4]) == firma
            })
        else { throw error("sin directorio central") }
        guard u16(fin + 10) > 0 else { throw .referenciaNula("CompresorZip.cs:99") }
        let cd = u32(fin + 16)
        guard u32(cd) == 0x0201_4B50 else { throw error("directorio central dañado") }
        let metodo = u16(cd + 10)
        let crc = UInt32(truncatingIfNeeded: u32(cd + 16))
        let (tamC, tamU) = (u32(cd + 20), u32(cd + 24))
        let lnombre = u16(cd + 28)
        let local = u32(cd + 42)
        guard lnombre >= 0, cd + 46 + lnombre <= z.count else { throw error("directorio central dañado") }
        if z[cd + 46 + lnombre - 1] == 0x2F { return [] }  // directorio: no se lee nada
        guard tamC != 0xFFFF_FFFF, tamU != 0xFFFF_FFFF, tamU <= 64 << 20 else { throw error("tamaño no admitido") }
        guard u32(local) == 0x0403_4B50 else { throw error("cabecera local dañada") }
        let inicio = local + 30 + u16(local + 26) + u16(local + 28)
        guard inicio >= 0, inicio + tamC <= z.count else { throw error("datos truncados") }
        let datos = Array(z[inicio..<inicio + tamC])
        let carga: [UInt8]
        switch metodo {
        case 0: carga = datos
        case 8: carga = try inflate(datos, tamano: tamU)
        default: throw error("método \(metodo) no admitido")
        }
        guard carga.count == tamU, crc32(carga) == crc else { throw error("CRC incorrecto") }
        return carga
    }

    /// `SetLevel(nivel)` + Deflate: flujo zlib sin la cabecera (2 bytes) ni el Adler-32 final (4) = DEFLATE crudo.
    static func deflate(_ origen: [UInt8], nivel: Int) throws(ErrorDominio) -> [UInt8] {
        var n = compressBound(uLong(origen.count))
        var destino = [UInt8](repeating: 0, count: Int(n))
        guard compress2(&destino, &n, origen, uLong(origen.count), Int32(nivel)) == Z_OK, n >= 6 else {
            throw error("no se pudo comprimir")
        }
        return Array(destino[2..<Int(n) - 4])
    }

    static func inflate(_ origen: [UInt8], tamano: Int) throws(ErrorDominio) -> [UInt8] {
        guard tamano > 0 else { return [] }
        var destino = [UInt8](repeating: 0, count: tamano)
        let n = compression_decode_buffer(&destino, tamano, origen, origen.count, nil, COMPRESSION_ZLIB)
        return Array(destino.prefix(n))
    }

    /// CRC-32 de ZIP (polinomio reflejado `0xEDB88320`).
    static let tablaCRC: [UInt32] = (0..<256).map { n in
        (0..<8).reduce(UInt32(n)) { c, _ in c & 1 != 0 ? 0xEDB8_8320 ^ (c >> 1) : c >> 1 }
    }

    static func crc32(_ datos: [UInt8]) -> UInt32 {
        ~datos.reduce(~UInt32(0)) { c, b in tablaCRC[Int((c ^ UInt32(b)) & 0xFF)] ^ (c >> 8) }
    }

    /// Fecha y hora DOS en la zona local (como `ZipEntry.DateTime`).
    static func fechaDOS(_ fecha: Date) -> (hora: Int, dia: Int) {
        let c = Calendar.current.dateComponents([.year, .month, .day, .hour, .minute, .second], from: fecha)
        let hora = (c.hour ?? 0) << 11 | (c.minute ?? 0) << 5 | (c.second ?? 0) / 2
        let dia = max(0, (c.year ?? 1980) - 1980) << 9 | (c.month ?? 1) << 5 | (c.day ?? 1)
        return (hora, dia)
    }
}
