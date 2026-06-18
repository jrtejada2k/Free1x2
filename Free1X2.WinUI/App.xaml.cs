// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml;
using Free1X2.WinUI.Services;
using Free1X2.WinUI.Views.Ported;

namespace Free1X2.WinUI;

public partial class App : Application
{
    public static Window? MainWindow { get; private set; }

    public App()
    {
        this.InitializeComponent();
        // Red de seguridad: una excepción no controlada en la UI no debe tumbar la app.
        // Se registra y se muestra al usuario en vez de hacer fail-fast.
        this.UnhandledException += static (s, e) =>
        {
            e.Handled = true;
            Services.AppServices.MostrarError("Se produjo un error inesperado:\n\n" + e.Exception.Message);
        };
    }

    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        AsegurarCarpetasDeTrabajo();
        SembrarJornadaDesdeCache();
        MainWindow = new MainWindow();
        AppServices.Inicializar(MainWindow);
        CablearHooksDominio();
        MainWindow.Activate();
    }

    /// <summary>
    /// Siembra <see cref="AppState.JornadaActual"/> con la jornada cacheada más reciente al
    /// arrancar, para que el boleto y "Grupos de Equipos" muestren los equipos REALES al instante
    /// y OFFLINE, sin ninguna petición de red. Es una simple lectura de un fichero local
    /// (<c>%LocalAppData%\Free1X2\jornada-{pais}.json</c>) reparseada con el parser defensivo.
    ///
    /// OFFLINE-FIRST estricto: NO contacta el servicio online (el único punto de red sigue siendo
    /// el botón "Actualizar jornada"). Si no hay caché, es un no-op silencioso (la app arranca en
    /// modo manual con los nombres de muestra, igual que antes). Cualquier error se traga: nunca
    /// debe impedir el arranque (de hecho, JornadaCache ya es a prueba de excepciones).
    /// </summary>
    private static void SembrarJornadaDesdeCache()
    {
        try
        {
            string? pais = Services.JornadaCache.PaisMasReciente();
            if (pais is null) return; // sin caché todavía: arranque manual normal (no-op).

            if (Services.JornadaCache.TryCargar(pais, out var jornada, out _) && jornada is not null)
            {
                Services.AppState.Instancia.JornadaActual = jornada;
            }
        }
        catch
        {
            // Robustez extra: el arranque jamás falla por la siembra de caché (sin red, local).
        }
    }

    /// <summary>
    /// Crea junto al ejecutable las carpetas de trabajo que el motor espera al guardar
    /// (columnas, combinaciones, condiciones, etc.). Los datos semilla (parametros.free1x2,
    /// Idioma, Equipos, Impresion) se copian vía &lt;Content&gt; del csproj; estas carpetas de
    /// salida se crean en runtime para no versionar directorios vacíos.
    /// </summary>
    private static void AsegurarCarpetasDeTrabajo()
    {
        try
        {
            string baseDir = System.AppContext.BaseDirectory;
            foreach (var d in new[] { "Columnas", "Combinaciones", "Condiciones", "Filtros",
                                      "Ganadoras", "Informes", "Jornadas", "Temp", "Comunicaciones",
                                      "Documentacion" })
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(baseDir, d));
            }
        }
        catch { /* no bloquear el arranque por permisos de carpeta */ }
    }

    /// <summary>
    /// Conecta los shims del dominio (Free1X2.Abstractions) con la UI de WinUI.
    /// Equivalente a Program.WireDomainHooks() del exe WinForms, pero usando
    /// DispatcherQueue/ContentDialog en lugar de Application.DoEvents/MessageBox.
    /// </summary>
    private static void CablearHooksDominio()
    {
        // El motor corre en hilos de fondo (Task.Run) en WinUI; no se bombea el
        // dispatcher desde un hilo de trabajo. No-op intencional.
        Free1X2.Abstractions.UiPump.Pump = static () => { };

        Free1X2.Abstractions.UserDialogs.ShowError = AppServices.MostrarError;
        Free1X2.Abstractions.UserDialogs.ShowInfo = AppServices.MostrarInfo;

        // Portapapeles: equivalente WinUI (DataPackage) del Clipboard de WinForms.
        // Escribir es síncrono; leer es async en WinRT, así que se espera el resultado
        // de forma sincrónica para respetar el contrato Func<string> del shim.
        Free1X2.Abstractions.Clipboard.Write = static texto =>
        {
            var paquete = new Windows.ApplicationModel.DataTransfer.DataPackage();
            paquete.SetText(texto ?? string.Empty);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(paquete);
        };
        Free1X2.Abstractions.Clipboard.Read = static () =>
        {
            try
            {
                var contenido = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();
                if (contenido.Contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.Text))
                {
                    return System.WindowsRuntimeSystemExtensions.AsTask(contenido.GetTextAsync()).GetAwaiter().GetResult();
                }
            }
            catch { /* portapapeles inaccesible: tratar como vacío */ }
            return string.Empty;
        };

        // Visor de análisis de columnas: el dominio (Analizador.AnalizaCombinacion) invoca este
        // hook con (ContenedorAnalisisGlobal, GruposPartidos[0]) tras analizar. Se deja el payload
        // en el handoff estático y se navega a la página del visor en el hilo de UI;
        // VisorAnalisisColumnasFrmViewModel lo consume en su constructor y reconstruye las
        // secciones desde el contenedor (equivalente a VisorAnalisisColumnasFrm.MostrarDatos del
        // WinForms). El producer real es, p. ej., AnalizarFicheroFrmViewModel.
        Free1X2.Abstractions.AnalisisUi.MostrarVisor = static (contenedor, grupo) =>
        {
            VisorAnalisisColumnasFrmViewModel.UltimoContenedor = contenedor;
            VisorAnalisisColumnasFrmViewModel.UltimoGrupo = grupo;
            AppServices.UiDispatcher?.TryEnqueue(static () =>
            {
                if (MainWindow?.Content is FrameworkElement fe &&
                    fe.FindName("ContentFrame") is Microsoft.UI.Xaml.Controls.Frame frame)
                {
                    frame.Navigate(typeof(VisorAnalisisColumnasFrmPage));
                }
            });
        };
    }
}
