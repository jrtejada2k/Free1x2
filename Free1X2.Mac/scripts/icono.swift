import AppKit

// Icono de la app a partir de la imagen elegida por el dueño (17-oct-2026, `icono-original.png`: «FREE 1X2» azul neón,
// red naranja, marco plateado sobre base azul):
//   swift scripts/icono.swift scripts/icono-original.png App/Recursos/Assets.xcassets/AppIcon.appiconset \
//       ../Free1X2.WinUI/Assets/app.ico
// Las esquinas del marco de la imagen son más cerradas que las de macOS: recortarla con la forma de macOS mordería el
// anillo. Por eso se toma solo el panel interior de la imagen y la base azul y el anillo plateado se dibujan aquí con
// la rejilla de macOS (cuerpo de 824 px con esquinas de 185 dentro de 1024, con sombra). El tercer argumento, opcional,
// escribe el mismo icono para Windows (.ico de 16 a 256 px, con el cuerpo llenando el lienzo).

/// Panel interior de `icono-original.png` (1024×1024, fila 0 arriba), medido sobre la imagen: dentro del anillo plateado.
let panelOrigen = CGRect(x: 216, y: 240, width: 596, height: 540)

let origen = NSImage(contentsOfFile: CommandLine.arguments[1])!.cgImage(forProposedRect: nil, context: nil, hints: nil)!
let salida = URL(fileURLWithPath: CommandLine.arguments[2])
let panelImagen = origen.cropping(to: panelOrigen)!

func rgb(_ hex: UInt32, _ a: CGFloat = 1) -> CGColor {
    CGColor(srgbRed: CGFloat((hex >> 16) & 0xFF) / 255, green: CGFloat((hex >> 8) & 0xFF) / 255,
            blue: CGFloat(hex & 0xFF) / 255, alpha: a)
}

func degradado(_ c: [CGColor], _ l: [CGFloat]) -> CGGradient {
    CGGradient(colorsSpace: CGColorSpace(name: CGColorSpace.sRGB)!, colors: c as CFArray, locations: l)!
}

func redondo(_ r: CGRect, _ radio: CGFloat) -> CGPath {
    CGPath(roundedRect: r, cornerWidth: radio, cornerHeight: radio, transform: nil)
}

func rellenar(_ cg: CGContext, _ p: CGPath, _ g: CGGradient, _ r: CGRect) {
    cg.saveGState()
    cg.addPath(p)
    cg.clip()
    cg.drawLinearGradient(g, start: CGPoint(x: r.midX, y: r.maxY), end: CGPoint(x: r.midX, y: r.minY), options: [])
    cg.restoreGState()
}

func lienzo(_ lado: Int, _ dibujar: (CGContext) -> Void) -> Data {
    let rep = NSBitmapImageRep(bitmapDataPlanes: nil, pixelsWide: lado, pixelsHigh: lado, bitsPerSample: 8, samplesPerPixel: 4,
                               hasAlpha: true, isPlanar: false, colorSpaceName: .deviceRGB, bytesPerRow: 0, bitsPerPixel: 0)!
    NSGraphicsContext.saveGraphicsState()
    NSGraphicsContext.current = NSGraphicsContext(bitmapImageRep: rep)
    let cg = NSGraphicsContext.current!.cgContext
    cg.interpolationQuality = .high
    dibujar(cg)
    NSGraphicsContext.restoreGraphicsState()
    return rep.representation(using: .png, properties: [:])!
}

let maestro = lienzo(1024) { cg in
    let cuerpo = CGRect(x: 100, y: 100, width: 824, height: 824)
    let anillo = CGRect(x: 124, y: 150, width: 776, height: 750)
    let panel = anillo.insetBy(dx: 20, dy: 20)
    // Base azul en relieve (más gruesa abajo) con sombra.
    cg.saveGState()
    cg.setShadow(offset: CGSize(width: 0, height: -12), blur: 28, color: rgb(0x000000, 0.35))
    cg.addPath(redondo(cuerpo, 185))
    cg.setFillColor(rgb(0x0A2A7A))
    cg.fillPath()
    cg.restoreGState()
    rellenar(cg, redondo(cuerpo, 185), degradado([rgb(0x2462E0), rgb(0x0A2A7A)], [0, 1]), cuerpo)
    // Anillo plateado.
    rellenar(cg, redondo(anillo, 165),
             degradado([rgb(0xF5F7FA), rgb(0xA7B0BB), rgb(0xE6EAEE), rgb(0x7C8591)], [0, 0.45, 0.6, 1]), anillo)
    // Panel: el de la imagen.
    cg.saveGState()
    cg.addPath(redondo(panel, 148))
    cg.clip()
    cg.draw(panelImagen, in: panel)
    cg.restoreGState()
}
let imagenMaestro = NSImage(data: maestro)!.cgImage(forProposedRect: nil, context: nil, hints: nil)!

/// Windows: .ico con el cuerpo del maestro (sin el margen ni la sombra de macOS). 256 px en PNG; el resto en BMP de 32 bits
/// (BGRA de abajo arriba, sin premultiplicar, máscara AND vacía), que es lo que entienden todas las herramientas.
func ico(_ destino: URL) {
    let cuerpo = imagenMaestro.cropping(to: CGRect(x: 100, y: 100, width: 824, height: 824))!
    var imagenes: [(lado: Int, datos: Data)] = []
    for lado in [16, 24, 32, 48, 64, 128, 256] {
        let rep = NSBitmapImageRep(bitmapDataPlanes: nil, pixelsWide: lado, pixelsHigh: lado, bitsPerSample: 8, samplesPerPixel: 4,
                                   hasAlpha: true, isPlanar: false, colorSpaceName: .deviceRGB, bytesPerRow: lado * 4, bitsPerPixel: 32)!
        NSGraphicsContext.saveGraphicsState()
        NSGraphicsContext.current = NSGraphicsContext(bitmapImageRep: rep)
        NSGraphicsContext.current!.cgContext.interpolationQuality = .high
        NSGraphicsContext.current!.cgContext.draw(cuerpo, in: CGRect(x: 0, y: 0, width: lado, height: lado))
        NSGraphicsContext.restoreGraphicsState()
        if lado == 256 {
            imagenes.append((lado, rep.representation(using: .png, properties: [:])!))
            continue
        }
        var d = Data()
        func u32(_ v: Int) { withUnsafeBytes(of: UInt32(v).littleEndian) { d.append(contentsOf: $0) } }
        func u16(_ v: Int) { withUnsafeBytes(of: UInt16(v).littleEndian) { d.append(contentsOf: $0) } }
        let mascara = (lado + 31) / 32 * 4 * lado
        u32(40); u32(lado); u32(lado * 2); u16(1); u16(32); u32(0); u32(lado * lado * 4 + mascara); u32(0); u32(0); u32(0); u32(0)
        let p = rep.bitmapData!
        for y in (0..<lado).reversed() {
            for x in 0..<lado {
                let i = y * lado * 4 + x * 4
                let a = Int(p[i + 3])
                func c(_ v: UInt8) -> UInt8 { a == 0 ? 0 : UInt8(min(255, Int(v) * 255 / a)) }
                d.append(contentsOf: [c(p[i + 2]), c(p[i + 1]), c(p[i]), UInt8(a)])
            }
        }
        d.append(Data(count: mascara))
        imagenes.append((lado, d))
    }
    var f = Data()
    func u32(_ v: Int) { withUnsafeBytes(of: UInt32(v).littleEndian) { f.append(contentsOf: $0) } }
    func u16(_ v: Int) { withUnsafeBytes(of: UInt16(v).littleEndian) { f.append(contentsOf: $0) } }
    u16(0); u16(1); u16(imagenes.count)
    var desplazamiento = 6 + 16 * imagenes.count
    for (lado, datos) in imagenes {
        f.append(contentsOf: [UInt8(lado % 256), UInt8(lado % 256), 0, 0])
        u16(1); u16(32); u32(datos.count); u32(desplazamiento)
        desplazamiento += datos.count
    }
    for (_, datos) in imagenes { f.append(datos) }
    try! f.write(to: destino)
}
if CommandLine.arguments.count > 3 { ico(URL(fileURLWithPath: CommandLine.arguments[3])) }

try! FileManager.default.createDirectory(at: salida, withIntermediateDirectories: true)
var imagenes: [String] = []
for punto in [16, 32, 128, 256, 512] {
    for escala in [1, 2] {
        let lado = punto * escala
        let nombre = "icon_\(punto)x\(punto)\(escala == 2 ? "@2x" : "").png"
        try! lienzo(lado) { $0.draw(imagenMaestro, in: CGRect(x: 0, y: 0, width: lado, height: lado)) }
            .write(to: salida.appending(path: nombre))
        imagenes.append(#"    {"filename": "\#(nombre)", "idiom": "mac", "scale": "\#(escala)x", "size": "\#(punto)x\#(punto)"}"#)
    }
}
let contenido = "{\n  \"images\": [\n" + imagenes.joined(separator: ",\n") + "\n  ],\n  \"info\": {\"author\": \"xcode\", \"version\": 1}\n}\n"
try! contenido.write(to: salida.appending(path: "Contents.json"), atomically: true, encoding: .utf8)
print(salida.path)
