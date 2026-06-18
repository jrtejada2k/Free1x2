// Free1X2 · WinUI 3 — WIN3
using System;
using System.IO;
using Free1X2.Online;

namespace Free1X2.WinUI.Services;

/// <summary>
/// Caché LOCAL en disco de la última jornada descargada con éxito, una por país. Hace que la
/// app funcione OFFLINE-FIRST: tras la primera descarga correcta, el boleto y "Grupos de Equipos"
/// muestran los equipos reales al instante en arranques posteriores SIN tocar la red; el servicio
/// online solo se contacta cuando el usuario pulsa explícitamente "Actualizar jornada" (acción
/// deliberada = buscar lo último) o cuando aún no existe caché. Esto respeta el límite de 60 req/min
/// del backend (docs/API_CLUBPROGOL.md): no se reconsulta en cada apertura de la pantalla.
///
/// Diseño:
///   · Se persiste el JSON CRUDO tal cual lo devolvió el servicio (los bytes ya validados/parseados),
///     no un volcado del modelo: así la lectura vuelve a pasar por el MISMO parser defensivo
///     (<see cref="JornadaQuinielaParser"/>) y la respuesta se sigue tratando SIEMPRE como datos.
///   · Un fichero por país en una carpeta escribible por usuario
///     (<c>%LocalAppData%\Free1X2\jornada-{pais}.json</c>); la marca de tiempo del "guardado" es
///     la fecha de última escritura del fichero (UTC).
///   · ROBUSTO ante todo error de E/S/permisos/JSON corrupto: nunca lanza hacia la UI. Una caché
///     ilegible o inválida se trata como "no hay caché" (la app cae a modo manual sin romperse).
/// </summary>
public static class JornadaCache
{
    /// <summary>Subcarpeta bajo LocalApplicationData donde viven los ficheros de caché.</summary>
    public const string NombreCarpeta = "Free1X2";

    /// <summary>Países soportados por la caché (mismos códigos que el servicio: "es"/"mx").</summary>
    private static readonly string[] PaisesSoportados = { "es", "mx" };

    /// <summary>
    /// Carpeta de caché: <c>%LocalAppData%\Free1X2</c>. No lanza si no se puede resolver/crear;
    /// devuelve la ruta calculada igualmente (las operaciones posteriores fallan en silencio).
    /// </summary>
    public static string Carpeta
    {
        get
        {
            string baseDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            return Path.Combine(baseDir, NombreCarpeta);
        }
    }

    /// <summary>Ruta del fichero de caché de un país ("es"/"mx"), o null si el país no es válido.</summary>
    private static string? RutaFichero(string pais)
    {
        string paisNorm = NormalizarPais(pais);
        if (paisNorm.Length == 0) return null;
        return Path.Combine(Carpeta, "jornada-" + paisNorm + ".json");
    }

    private static string NormalizarPais(string? pais)
    {
        string p = (pais ?? "").Trim().ToLowerInvariant();
        return (p == "es" || p == "mx") ? p : "";
    }

    /// <summary>
    /// Guarda el JSON CRUDO de una descarga correcta para el país indicado. Crea la carpeta si
    /// no existe. NUNCA lanza: cualquier error de E/S/permisos se ignora (la caché es un extra,
    /// no debe romper la descarga, que ya tuvo éxito).
    /// </summary>
    public static void Guardar(string pais, string rawJson)
    {
        if (string.IsNullOrWhiteSpace(rawJson)) return;
        string? ruta = RutaFichero(pais);
        if (ruta is null) return;

        try
        {
            Directory.CreateDirectory(Carpeta);
            File.WriteAllText(ruta, rawJson);
        }
        catch
        {
            // Permisos/disco lleno/etc.: la caché es opcional; se descarta en silencio.
        }
    }

    /// <summary>
    /// Intenta cargar y parsear la jornada cacheada de un país. Devuelve <c>true</c> y rellena
    /// <paramref name="jornada"/> + <paramref name="guardadoUtc"/> (fecha de última escritura del
    /// fichero, UTC) si existe una caché VÁLIDA; <c>false</c> si no hay fichero, no se puede leer
    /// o el JSON está corrupto/no cumple el contrato. NUNCA lanza: una caché corrupta = "no hay caché".
    /// </summary>
    public static bool TryCargar(string pais, out JornadaQuiniela? jornada, out DateTime guardadoUtc)
    {
        jornada = null;
        guardadoUtc = default;

        string? ruta = RutaFichero(pais);
        if (ruta is null) return false;

        try
        {
            if (!File.Exists(ruta)) return false;

            string raw = File.ReadAllText(ruta);
            // Re-parsea con el MISMO parser defensivo: la caché se trata como dato no confiable.
            jornada = JornadaQuinielaParser.Parse(raw);
            guardadoUtc = File.GetLastWriteTimeUtc(ruta);
            return true;
        }
        catch
        {
            // FormatException (JSON corrupto), IOException, UnauthorizedAccess, etc.: sin caché.
            jornada = null;
            guardadoUtc = default;
            return false;
        }
    }

    /// <summary>
    /// Devuelve el código del país ("es"/"mx") cuyo fichero de caché es el más reciente (mayor
    /// fecha de última escritura), o <c>null</c> si ninguno tiene caché. Se usa al arrancar para
    /// sembrar <c>AppState.JornadaActual</c> con la jornada guardada más nueva, sin red. NUNCA lanza.
    /// </summary>
    public static string? PaisMasReciente()
    {
        string? mejorPais = null;
        DateTime mejorFecha = DateTime.MinValue;

        foreach (string pais in PaisesSoportados)
        {
            string? ruta = RutaFichero(pais);
            if (ruta is null) continue;
            try
            {
                if (!File.Exists(ruta)) continue;
                DateTime fecha = File.GetLastWriteTimeUtc(ruta);
                if (fecha > mejorFecha)
                {
                    mejorFecha = fecha;
                    mejorPais = pais;
                }
            }
            catch
            {
                // Fichero inaccesible: se ignora (como si no existiera).
            }
        }

        return mejorPais;
    }
}
