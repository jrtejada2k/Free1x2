import Foundation
import Free1X2Base
import Free1X2Formatos
import Free1X2Motor

// C#: Views/Ported/_Helpers/FiltroArchivoViewModelBase.cs:30-115 y el patrón de condición (10 §1.10; ESPEC_13 §4.6).
// Cada pantalla arma su «filtro temporal» como un `CondicionesGrupo` nuevo con solo su condición.

public enum CasosCondiciones {
    /// `UtilidadesEntradasValores.ObtenerTodosValores()`: `"0,1,…,N"`.
    public static func todosValores(_ n: Int) -> String { (0...max(n, 0)).map(String.init).joined(separator: ",") }

    static func blanco(_ s: String) -> Bool { s.allSatisfy(\.isWhitespace) }

    /// `RutaTemporal`: `Temp/tmp.<ext>`, el «portapapeles» de condiciones (Copiar/Pegar).
    public static func rutaTemporal(_ extension: String, entorno: Entorno) -> URL {
        entorno.rutas.carpeta(.temp).appending(path: "tmp." + `extension`)
    }

    /// CU-A69 (Guardar y Copiar): `ArchivoCondiciones.GuardaArchivo(filtroTemp)`; crea la carpeta si falta.
    public static func exportarCondicion(
        _ tipo: TipoFiltro, _ c: CondicionesGrupo, en url: URL, entorno: Entorno
    ) throws(ErrorDominio) {
        try entorno.archivos.crearCarpeta(url.deletingLastPathComponent())
        try entorno.archivos.escribir(
            try ArchivoCondiciones.datos(filtro: tipo, de: c, cultura: entorno.cultura), en: url, atomico: true)
    }

    /// CU-A70 (Abrir y Pegar): `AbrirArchivoCombinacion` + `LeeCondicion`; `nil` si el XML no carga (no avisa).
    public static func importarCondicion(_ url: URL, numeroPartidos: Int, entorno: Entorno) throws(ErrorDominio) -> CondicionesGrupo? {
        guard let doc = ArchivoCondiciones.abrir(try entorno.archivos.leer(url)) else { return nil }
        return try ArchivoCondiciones.leerCondicion(doc, numeroPartidos: numeroPartidos, cultura: entorno.cultura).condiciones
    }

    /// CU-A71: cumplimiento del filtro temporal sobre cada `Ganadoras/*.txt` (orden de NTFS, DT-14). Los puntos CP se releen
    /// del fichero (DP-04).
    public static func estadisticasDeCondicion(
        _ tipo: TipoFiltro, _ c: CondicionesGrupo, configuracion: Configuracion, entorno: Entorno
    ) throws(ErrorDominio) -> [Estadistica] {
        var cfg = configuracion
        cfg.puntosCP = CasosAjustes.leerPuntosCP(entorno: entorno)
        let dir = entorno.rutas.carpeta(.ganadoras)
        var ficheros: [(nombre: String, lineas: [String])] = []
        for f in try entorno.archivos.listar(dir) where f.pathExtension.lowercased() == "txt" {
            ficheros.append((f.lastPathComponent, ArchivoColumnasTexto.columnas(try entorno.archivos.leer(f))))
        }
        return try CalculadorEstadisticas.estadisticas(
            tipo, condiciones: c, configuracion: cfg, cultura: entorno.cultura, ficheros: ficheros)
    }

    /// `Estadistica.Cumplimiento.ToString()` (`float`, T14) para el visor (CU-C55).
    public static func textoCumplimiento(_ e: Estadistica, cultura: Cultura) -> String {
        FormatoNET.toString(e.cumplimiento, cultura: cultura)
    }

    // MARK: Pantalla 75 · Variantes X y 2 (CU-C02)

    /// `AplicarCondicionNoVariantes` (`NoVariantesFrmViewModel.cs:82-114`): un campo vacío = todos los valores; sin datos
    /// la condición queda inactiva y vacía; con datos solo se activa la primera vez.
    public static func aplicarNoVariantes(
        variantes: String, equis: String, doses: String, en f: inout CondicionNoVariantes
    ) throws(ErrorDominio) {
        let todos = todosValores(f.numeroPartidos)
        f.reinicializaValores()
        guard !blanco(variantes) || !blanco(equis) || !blanco(doses) else {
            f.activa = false
            f.contieneDatos = false
            return
        }
        if !f.contieneDatos { f.activa = true }
        f.contieneDatos = true
        try f.setNoVariantes(blanco(variantes) ? todos : variantes)
        try f.setNoEquis(blanco(equis) ? todos : equis)
        try f.setNoDoses(blanco(doses) ? todos : doses)
    }
}
