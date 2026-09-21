// Free1X2 · WinUI 3 — WIN3
using System;
using System.IO;
using System.Text.Json;
using Microsoft.UI.Xaml;

namespace Free1X2.WinUI.Services;

/// <summary>
/// Preferencia de TEMA de la aplicación (Claro / Oscuro / Sistema) con persistencia entre
/// sesiones (U-01). La app trae la paleta oscura completa en <c>Themes/Tokens.xaml</c>
/// (diccionario de tema "Dark"), pero estaba inaccesible porque <c>App.xaml</c> fijaba
/// <c>RequestedTheme="Light"</c>; este servicio es el único punto que decide y aplica el tema.
///
/// Diseño:
///   · El tema se aplica con <see cref="FrameworkElement.RequestedTheme"/> sobre el elemento
///     RAÍZ del contenido de la ventana (no con <c>Application.RequestedTheme</c>, que solo se
///     puede fijar antes de crear la ventana y obligaría a reiniciar). Así el cambio surte
///     efecto EN VIVO: todos los <c>ThemeResource</c> de la app se reevalúan al asignarlo.
///   · "Sistema" = <see cref="ElementTheme.Default"/>: sin tema forzado, WinUI sigue el modo
///     claro/oscuro de Windows (y reacciona a un cambio en caliente del sistema).
///   · Persistencia: la app es DESEMPAQUETADA (<c>WindowsPackageType=None</c>), así que NO
///     existe <c>ApplicationData.Current.LocalSettings</c>. Se guarda un JSON mínimo en
///     <c>%LocalAppData%\Free1X2\tema.json</c>, reutilizando <see cref="JornadaCache.Carpeta"/>
///     (la misma carpeta escribible por usuario que usan la caché de jornada y <see cref="Log"/>).
///   · NUNCA lanza: fichero ausente, ilegible o corrupto ⇒ se cae al valor por defecto
///     ("Sistema") dejando traza con <see cref="Log"/>. Un fallo al guardar no rompe el cambio
///     de tema en vivo (solo no se recuerda).
/// </summary>
public static class TemaApp
{
    /// <summary>Opciones que ve el usuario en el menú «Ver → Tema».</summary>
    public enum Opcion
    {
        /// <summary>Sigue el modo claro/oscuro de Windows (valor por defecto).</summary>
        Sistema = 0,

        /// <summary>Fuerza la paleta clara ("Índigo &amp; Teal").</summary>
        Claro = 1,

        /// <summary>Fuerza la paleta oscura.</summary>
        Oscuro = 2,
    }

    /// <summary>Nombre del fichero de preferencia dentro de la carpeta de datos de la app.</summary>
    private const string NombreFichero = "tema.json";

    /// <summary>Ruta del fichero de preferencia (<c>%LocalAppData%\Free1X2\tema.json</c>).</summary>
    public static string Ruta => Path.Combine(JornadaCache.Carpeta, NombreFichero);

    /// <summary>
    /// Elemento raíz sobre el que se aplica el tema (el contenido de la ventana principal).
    /// Lo registra <see cref="Aplicar"/> al arrancar; <see cref="Cambiar"/> lo reutiliza para
    /// repintar en vivo sin reiniciar.
    /// </summary>
    private static FrameworkElement? _raiz;

    /// <summary>Opción activa. Se siembra en el arranque desde el fichero de preferencia.</summary>
    public static Opcion Actual { get; private set; } = Opcion.Sistema;

    /// <summary>
    /// Lee la preferencia guardada. Devuelve <see cref="Opcion.Sistema"/> si no hay fichero,
    /// no se puede leer o el contenido no es válido (primera ejecución incluida). NUNCA lanza.
    /// </summary>
    public static Opcion Cargar()
    {
        try
        {
            string ruta = Ruta;
            if (!File.Exists(ruta)) return Opcion.Sistema; // primera ejecución: por defecto Sistema.

            using var doc = JsonDocument.Parse(File.ReadAllText(ruta));
            if (doc.RootElement.ValueKind == JsonValueKind.Object &&
                doc.RootElement.TryGetProperty("tema", out var prop) &&
                prop.ValueKind == JsonValueKind.String &&
                Enum.TryParse(prop.GetString(), ignoreCase: true, out Opcion opcion))
            {
                return opcion;
            }

            // Fichero legible pero con contenido inesperado: se trata como "sin preferencia".
            Log.Warn("TemaApp.Cargar: contenido no válido en " + ruta + "; se usa Sistema.");
            return Opcion.Sistema;
        }
        catch (Exception ex)
        {
            // JSON corrupto, permisos, disco… el fallback (Sistema) no cambia; queda traza.
            Log.Error("TemaApp.Cargar", ex);
            return Opcion.Sistema;
        }
    }

    /// <summary>
    /// Registra el elemento raíz del contenido de la ventana y le aplica la preferencia
    /// guardada. Se llama UNA vez al construir la ventana principal.
    /// </summary>
    public static void Aplicar(FrameworkElement raiz)
    {
        _raiz = raiz;
        Actual = Cargar();
        AplicarARaiz();
        Log.Info("TemaApp: tema inicial = " + Actual);
    }

    /// <summary>
    /// Cambia el tema en VIVO (sin reiniciar) y persiste la elección. Si la opción ya está
    /// activa no hace nada.
    /// </summary>
    public static void Cambiar(Opcion opcion)
    {
        if (opcion == Actual) return;
        Actual = opcion;
        AplicarARaiz();
        Guardar(opcion);
    }

    /// <summary>
    /// Tema que debe pedir un elemento creado FUERA del árbol de la ventana (los
    /// <c>ContentDialog</c> viven en un popup del <c>XamlRoot</c> y no heredan el
    /// <c>RequestedTheme</c> de la raíz). Con "Sistema" devuelve <see cref="ElementTheme.Default"/>,
    /// que es justo lo que hay que dejar para seguir a Windows.
    /// </summary>
    public static ElementTheme TemaDeElemento => AElementTheme(Actual);

    /// <summary>Traduce la opción del menú al tema de WinUI.</summary>
    private static ElementTheme AElementTheme(Opcion opcion) => opcion switch
    {
        Opcion.Claro => ElementTheme.Light,
        Opcion.Oscuro => ElementTheme.Dark,
        _ => ElementTheme.Default, // Sistema: sin forzar ⇒ sigue a Windows.
    };

    /// <summary>
    /// Asigna <c>RequestedTheme</c> a la raíz registrada. Al hacerlo, WinUI reevalúa todos los
    /// <c>ThemeResource</c> del árbol, por lo que la app se repinta al instante. No lanza.
    /// </summary>
    private static void AplicarARaiz()
    {
        var raiz = _raiz;
        if (raiz is null) return; // aún no hay ventana: se aplicará al registrarla.

        try
        {
            raiz.RequestedTheme = AElementTheme(Actual);
        }
        catch (Exception ex)
        {
            // No es crítico: la app sigue con el tema que tuviera. Queda traza.
            Log.Error("TemaApp.AplicarARaiz", ex);
        }
    }

    /// <summary>
    /// Escribe la preferencia. Escritura ATÓMICA (temporal + reemplazo), igual que
    /// <see cref="JornadaCache.Guardar"/>: un corte a mitad no deja un JSON truncado que la
    /// siguiente ejecución tendría que descartar. NUNCA lanza.
    /// </summary>
    private static void Guardar(Opcion opcion)
    {
        string ruta = Ruta;
        string tmp = ruta + ".tmp";
        try
        {
            Directory.CreateDirectory(JornadaCache.Carpeta);
            File.WriteAllText(tmp, "{\"tema\":\"" + opcion + "\"}");
            File.Move(tmp, ruta, overwrite: true);
        }
        catch (Exception ex)
        {
            // Permisos/disco lleno: el tema YA está aplicado en vivo; solo no se recordará.
            Log.Error("TemaApp.Guardar", ex);
            try { if (File.Exists(tmp)) File.Delete(tmp); } catch { /* best-effort */ }
        }
    }
}
