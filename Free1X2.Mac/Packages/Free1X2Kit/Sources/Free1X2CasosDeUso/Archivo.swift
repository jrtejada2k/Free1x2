import Foundation
import Free1X2Base
import Free1X2Formatos
import Free1X2Online

// C#: Views/Ported/GestorEquiposFrmViewModel.cs (09 pantalla 60), AgregarEquipoFrmViewModel.cs (08 pantalla 2) y el
// servicio online (`QuinielaOnlineService`, 07 §5).

/// Categorías de `Equipos/equipos{1,2,2b,Int}.dat`.
public enum CategoriaEquipos: Int, CaseIterable, Sendable {
    case primera, segunda, segundaB, internacional

    public var titulo: String { ["1ª", "2ª", "2ªB", "Int"][rawValue] }
    public var fichero: String { ["equipos1.dat", "equipos2.dat", "equipos2b.dat", "equiposInt.dat"][rawValue] }

    func ruta(_ entorno: Entorno) -> URL { entorno.rutas.base.appending(path: "Equipos/" + fichero) }
}

/// «Gestión de Equipos»: las cuatro listas en memoria hasta «Guardar archivos».
public struct EditorEquipos: Sendable {
    public private(set) var listas: [[String]] = Array(repeating: [], count: 4)
    public var estado = "Preparado"

    public init() {}

    /// Constructor del VM (:205-213): un fallo de lectura deja la lista vacía y lo cuenta en el estado.
    public static func cargar(entorno: Entorno) -> EditorEquipos {
        var e = EditorEquipos()
        for c in CategoriaEquipos.allCases {
            do throws(ErrorDominio) {
                e.listas[c.rawValue] = ArchivoEquiposCategoria.leer(try entorno.archivos.leer(c.ruta(entorno)))
            } catch {
                e.estado = "No se pudo cargar \(c.fichero): \(error.mensajeUsuario)"
            }
        }
        return e
    }

    public func lista(_ c: CategoriaEquipos) -> [String] { listas[c.rawValue] }

    /// `Clave` + `OrdinalIgnoreCase`.
    static func clave(_ s: String) -> String { CatalogoEquipos.mayusculaSimple(FormatoNET.recortar(s)) }

    func contiene(_ c: CategoriaEquipos, _ equipo: String) -> Bool {
        let k = Self.clave(equipo)
        return listas[c.rawValue].contains { Self.clave($0) == k }
    }

    /// `Mover`: no duplica en el destino; quita la primera aparición exacta del origen.
    public mutating func mover(_ seleccion: String?, de origen: CategoriaEquipos, a destino: CategoriaEquipos) {
        guard let s = seleccion, !s.isEmpty else { return estado = "Selecciona un equipo." }
        if !contiene(destino, s) { listas[destino.rawValue].append(s) }
        if let i = listas[origen.rawValue].firstIndex(of: s) { listas[origen.rawValue].remove(at: i) }
        estado = "Movido \"\(s)\"."
    }

    public mutating func eliminar(_ seleccion: String?, de c: CategoriaEquipos) {
        guard let s = seleccion, !s.isEmpty else { return estado = "Selecciona un equipo." }
        if let i = listas[c.rawValue].firstIndex(of: s) { listas[c.rawValue].remove(at: i) }
        estado = "Eliminado \"\(s)\"."
    }

    /// `NuevoEquipo`: devuelve si hay que vaciar el nombre (todo salvo el nombre en blanco).
    public mutating func nuevo(_ nombre: String, en c: CategoriaEquipos) -> Bool {
        guard !CasosCondiciones.blanco(nombre) else {
            estado = "Introduce un nombre de equipo."
            return false
        }
        if contiene(c, nombre) {
            estado = "El equipo \"\(nombre)\" ya existe en \(c.titulo)."
        } else {
            listas[c.rawValue].append(nombre)
            estado = "Equipo \"\(nombre)\" añadido a \(c.titulo)."
        }
        return true
    }

    /// `ImportarEquiposOnline` (:91-129): añade los que falten (sin distinguir mayúsculas).
    public mutating func importar(_ equipos: [String], en c: CategoriaEquipos) {
        var ya = Set(listas[c.rawValue].map(Self.clave))
        var n = 0
        for e in equipos where ya.insert(Self.clave(e)).inserted {
            listas[c.rawValue].append(e)
            n += 1
        }
        estado =
            n > 0
            ? "\(n) equipos importados (online) a \(c.titulo). Revisa y pulsa «Guardar archivos»."
            : "El catálogo online no añadió equipos nuevos a \(c.titulo) (ya estaban todos)."
    }

    /// Error del catálogo online: el del servicio tal cual; otro, con prefijo.
    public mutating func errorImportacion(_ error: any Error) {
        let detalle =
            (error as? ErrorOnline)?.mensaje ?? "No se pudo importar el catálogo: "
            + ((error as? ErrorDominio)?.mensajeUsuario ?? "\(error)")
        estado = detalle + " Los equipos no se modificaron."
    }

    /// `Guardar` (:259-290): las cuatro categorías, Latin-1.
    public mutating func guardar(entorno: Entorno) throws(ErrorDominio) {
        do throws(ErrorDominio) {
            for c in CategoriaEquipos.allCases {
                try entorno.archivos.crearCarpeta(c.ruta(entorno).deletingLastPathComponent())
                try entorno.archivos.escribir(ArchivoEquiposCategoria.datos(listas[c.rawValue]), en: c.ruta(entorno), atomico: false)
            }
            estado = "Equipos guardados."
        } catch {
            estado = "Error al guardar: " + error.mensajeUsuario
            throw error
        }
    }
}

public enum CasosArchivo {
    /// CU-A02 `Anadir` (pantalla 2): sin fichero, lista vacía; comparación exacta. `false` = ya existía.
    public static func anadirEquipo(_ nombre: String, categoria c: CategoriaEquipos, entorno: Entorno) throws(ErrorDominio) -> Bool {
        let ruta = c.ruta(entorno)
        var equipos = entorno.archivos.existe(ruta) ? ArchivoEquiposCategoria.leer(try entorno.archivos.leer(ruta)) : []
        guard !equipos.contains(nombre) else { return false }
        equipos.append(nombre)
        try entorno.archivos.crearCarpeta(ruta.deletingLastPathComponent())
        try entorno.archivos.escribir(ArchivoEquiposCategoria.datos(equipos), en: ruta, atomico: false)
        return true
    }

    /// El servicio online de la app; `FREE1X2_API_BASE` cambia la base (solo desarrollo).
    public static func servicioOnline(entorno: Entorno) -> ServicioQuiniela {
        ServicioQuiniela(
            red: entorno.red, reloj: entorno.reloj,
            cache: CacheJornada(archivos: entorno.archivos, carpeta: entorno.rutas.base, registro: entorno.registro),
            base: ProcessInfo.processInfo.environment["FREE1X2_API_BASE"])
    }
}

// C#: Views/Ported/DescargaBoletoFrmViewModel.cs (08 pantalla 32).
extension CasosArchivo {
    static func cache(_ entorno: Entorno) -> CacheJornada {
        CacheJornada(archivos: entorno.archivos, carpeta: entorno.rutas.base, registro: entorno.registro)
    }

    /// `ResolverPaisInicial`: el de la caché más reciente o España.
    public static func paisInicial(entorno: Entorno) -> PaisQuiniela { cache(entorno).paisMasReciente() ?? .es }

    /// `FormatearFechaLocal`: `dd/MM/yyyy HH:mm` en hora local.
    static func fechaLocal(_ d: Date) -> String {
        let f = DateFormatter()
        f.locale = Locale(identifier: "en_US_POSIX")
        f.dateFormat = "dd/MM/yyyy HH:mm"
        return f.string(from: d)
    }

    /// `Descargar` (:62-118): jornada nueva (o la de la caché si falla la red) y el mensaje de la pantalla.
    public static func descargarJornada(
        _ pais: PaisQuiniela, servicio: ServicioQuiniela, entorno: Entorno
    ) async -> (jornada: JornadaQuiniela?, mensaje: String) {
        func sufijo(_ d: Date?, _ a: String, _ b: String) -> String { d.map { a + fechaLocal($0) + b } ?? "" }
        do {
            let r = try await servicio.obtenerJornada(pais: pais.rawValue)
            let resumen = "Jornada \(r.jornada.jornada) · \(r.jornada.partidos.count) partidos cargados"
            return (
                r.jornada,
                r.desdeCache
                    ? resumen + " — mostrando la última guardada" + sufijo(r.guardado, " (", ")") + "."
                    : resumen + sufijo(r.guardado, " · Última actualización: ", "")
            )
        } catch {
            if let c = cache(entorno).cargar(pais) {
                entorno.registro.registrar(
                    "Descarga de jornada fallida (\(pais.rawValue)); se usa la caché de \(fechaLocal(c.guardado)). Causa: \(error)",
                    nivel: .aviso)
                return (c.jornada, "Sin conexión: mostrando la jornada guardada (actualizada " + fechaLocal(c.guardado) + ").")
            }
            let detalle = (error as? ErrorOnline)?.mensaje ?? "No se pudo descargar la jornada: \(error.localizedDescription)"
            return (nil, detalle + " El boleto sigue en modo manual.")
        }
    }

    /// «Local - Visitante» de los `n` primeros partidos de la jornada; el 15º es el pleno (DT-81: el C# no rellena Inicio).
    public static func equiposDeJornada(_ j: JornadaQuiniela, partidos n: Int) -> [String] {
        (j.partidos + (j.pleno15.map { [$0] } ?? [])).prefix(n).map {
            CasosInicio.componerEquipos(local: $0.local, visitante: $0.visitante)
        }
    }
}
