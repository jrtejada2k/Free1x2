// Free1X2 · WinUI 3 — WIN3
using System;
using System.IO;
using System.Text.Json;
using Windows.Globalization;

namespace Free1X2.WinUI.Services;

/// <summary>
/// Preferencia de IDIOMA de la aplicación (Español / English) con persistencia entre sesiones
/// (U-10). Calcada de <see cref="TemaApp"/>: enum de opciones, <see cref="Cargar"/> /
/// <see cref="Cambiar"/>, persistencia en <c>%LocalAppData%\Free1X2\idioma.json</c> (reutiliza
/// <see cref="JornadaCache.Carpeta"/>) y tolerancia total a errores vía <see cref="Log"/>.
///
/// Diseño:
///   · El idioma se aplica fijando <see cref="ApplicationLanguages.PrimaryLanguageOverride"/>
///     con el código BCP-47 ("es" / "en"). Esto funciona en la app DESEMPAQUETADA
///     (<c>WindowsPackageType=None</c>), sin paquete MSIX. El override selecciona qué recursos
///     <c>.resw</c> resuelve el sistema PRI para los <c>x:Uid</c>.
///   · Como los <c>x:Uid</c> se resuelven al CARGAR la página (no en vivo), un cambio de idioma
///     solo se refleja al re-navegar / recargar la página; por eso <see cref="Aplicar"/> se llama
///     en el ARRANQUE (App.OnLaunched), ANTES de crear la ventana y navegar a la primera página,
///     para que el primer pintado ya use el idioma elegido.
///   · Por defecto <see cref="Opcion.Espanol"/>: la app sigue arrancando en español (la lengua
///     actual del programa). El inglés es adicional; solo la pantalla piloto está localizada.
///   · Persistencia: la app es DESEMPAQUETADA, así que NO existe
///     <c>ApplicationData.Current.LocalSettings</c>. Se guarda un JSON mínimo en
///     <c>%LocalAppData%\Free1X2\idioma.json</c> (misma carpeta escribible por usuario que la
///     caché de jornada, el tema y <see cref="Log"/>).
///   · NUNCA lanza: fichero ausente, ilegible o corrupto ⇒ se cae al valor por defecto (Español)
///     dejando traza con <see cref="Log"/>. Un fallo al guardar no rompe el cambio (solo no se
///     recuerda).
/// </summary>
public static class IdiomaApp
{
    /// <summary>Opciones que ve el usuario en el menú «Ver → Idioma».</summary>
    public enum Opcion
    {
        /// <summary>Español (valor por defecto: la app arranca en español).</summary>
        Espanol = 0,

        /// <summary>English (idioma adicional).</summary>
        Ingles = 1,
    }

    /// <summary>Nombre del fichero de preferencia dentro de la carpeta de datos de la app.</summary>
    private const string NombreFichero = "idioma.json";

    /// <summary>Ruta del fichero de preferencia (<c>%LocalAppData%\Free1X2\idioma.json</c>).</summary>
    public static string Ruta => Path.Combine(JornadaCache.Carpeta, NombreFichero);

    /// <summary>Opción activa. Se siembra en el arranque desde el fichero de preferencia.</summary>
    public static Opcion Actual { get; private set; } = Opcion.Espanol;

    /// <summary>
    /// Código BCP-47 que se pasa a <see cref="ApplicationLanguages.PrimaryLanguageOverride"/>
    /// (y con el que el sistema PRI resuelve la carpeta de recursos: <c>Strings\es</c> /
    /// <c>Strings\en-US</c>).
    /// </summary>
    public static string CodigoBcp47 => Actual == Opcion.Ingles ? "en" : "es";

    /// <summary>
    /// Lee la preferencia guardada. Devuelve <see cref="Opcion.Espanol"/> si no hay fichero, no se
    /// puede leer o el contenido no es válido (primera ejecución incluida). NUNCA lanza.
    /// </summary>
    public static Opcion Cargar()
    {
        try
        {
            string ruta = Ruta;
            if (!File.Exists(ruta)) return Opcion.Espanol; // primera ejecución: por defecto Español.

            using var doc = JsonDocument.Parse(File.ReadAllText(ruta));
            if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                doc.RootElement.TryGetProperty("idioma", out var prop) &&
                prop.ValueKind == JsonValueKind.String &&
                Enum.TryParse(prop.GetString(), ignoreCase: true, out Opcion opcion))
            {
                return opcion;
            }

            // Fichero legible pero con contenido inesperado: se trata como "sin preferencia".
            Log.Warn("IdiomaApp.Cargar: contenido no válido en " + ruta + "; se usa Español.");
            return Opcion.Espanol;
        }
        catch (Exception ex)
        {
            // JSON corrupto, permisos, disco… el fallback (Español) no cambia; queda traza.
            Log.Error("IdiomaApp.Cargar", ex);
            return Opcion.Espanol;
        }
    }

    /// <summary>
    /// Siembra la preferencia guardada y la aplica al override de idioma. Se llama UNA vez en el
    /// ARRANQUE (App.OnLaunched), antes de crear la ventana y navegar a la primera página.
    /// </summary>
    public static void Aplicar()
    {
        Actual = Cargar();
        AplicarOverride();
        Log.Info("IdiomaApp: idioma inicial = " + Actual + " (" + CodigoBcp47 + ")");
    }

    /// <summary>
    /// Cambia el idioma y persiste la elección. Si la opción ya está activa no hace nada. El
    /// override surte efecto al CARGAR una página, así que quien llama debe re-navegar / recargar
    /// la página actual para que el cambio se vea (los <c>x:Uid</c> no se repintan en vivo).
    /// </summary>
    public static void Cambiar(Opcion opcion)
    {
        if (opcion == Actual) return;
        Actual = opcion;
        AplicarOverride();
        Guardar(opcion);
    }

    /// <summary>
    /// Fija <see cref="ApplicationLanguages.PrimaryLanguageOverride"/> con el código del idioma
    /// activo. No es crítico: si falla, la app sigue con el idioma que resuelva el sistema y queda
    /// traza. NUNCA lanza.
    /// </summary>
    private static void AplicarOverride()
    {
        try
        {
            ApplicationLanguages.PrimaryLanguageOverride = CodigoBcp47;
        }
        catch (Exception ex)
        {
            Log.Error("IdiomaApp.AplicarOverride", ex);
        }
    }

    /// <summary>
    /// Escribe la preferencia. Escritura ATÓMICA (temporal + reemplazo), igual que
    /// <see cref="TemaApp"/> y <see cref="JornadaCache.Guardar"/>: un corte a mitad no deja un JSON
    /// truncado que la siguiente ejecución tendría que descartar. NUNCA lanza.
    /// </summary>
    private static void Guardar(Opcion opcion)
    {
        string ruta = Ruta;
        string tmp = ruta + ".tmp";
        try
        {
            Directory.CreateDirectory(JornadaCache.Carpeta);
            File.WriteAllText(tmp, "{\"idioma\":\"" + opcion + "\"}");
            File.Move(tmp, ruta, overwrite: true);
        }
        catch (Exception ex)
        {
            // Permisos/disco lleno: el idioma YA está aplicado; solo no se recordará.
            Log.Error("IdiomaApp.Guardar", ex);
            try { if (File.Exists(tmp)) File.Delete(tmp); } catch { /* best-effort */ }
        }
    }
}
