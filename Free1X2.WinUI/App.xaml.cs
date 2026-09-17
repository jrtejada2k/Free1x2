// Free1X2 · WinUI 3 — WIN3
using System;
using Microsoft.UI.Xaml;
using Free1X2.WinUI.Services;
using Free1X2.WinUI.Views.Ported;

namespace Free1X2.WinUI;

public partial class App : Application
{
    public static Window? MainWindow { get; private set; }

    /// <summary>
    /// Bandera de corte del manejador global (B-01). Si al intentar AVISAR de una excepción se
    /// produce otra, mostrar un segundo diálogo reentraría en el mismo fallo → bucle infinito
    /// (el caso descrito en el plan: ContentDialog ya abierto → InvalidOperationException →
    /// MostrarError → re-encola → vuelve a lanzar). Con la bandera puesta la segunda excepción
    /// solo se REGISTRA. El corte real del bucle está en <see cref="AppServices"/> (la cola
    /// serializa los diálogos y captura el fallo del ShowAsync); esto es la red de seguridad.
    /// </summary>
    private static bool _enManejadorDeExcepciones;

    public App()
    {
        this.InitializeComponent();
        // Red de seguridad: una excepción no controlada en la UI no debe tumbar la app.
        // Se registra y se muestra al usuario en vez de hacer fail-fast.
        this.UnhandledException += static (s, e) =>
        {
            e.Handled = true;

            // SIEMPRE se registra primero: antes de esto una excepción no controlada no dejaba
            // ningún rastro en disco y no había nada que mirar al reportar el fallo (C-12).
            Log.Error("App.UnhandledException", e.Exception);

            if (_enManejadorDeExcepciones) return; // ya estamos avisando de un error: no reentrar

            try
            {
                _enManejadorDeExcepciones = true;
                Services.AppServices.MostrarError("Se produjo un error inesperado:\n\n" + e.Exception.Message);
            }
            finally
            {
                _enManejadorDeExcepciones = false;
            }
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
        catch (Exception ex)
        {
            // Robustez extra: el arranque jamás falla por la siembra de caché (sin red, local).
            // El fallback (arrancar en modo manual) NO cambia; solo se deja traza.
            Log.Error("App.SembrarJornadaDesdeCache", ex);
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
        catch (Exception ex)
        {
            // No bloquear el arranque por permisos de carpeta (comportamiento intacto); se
            // registra para poder diagnosticar el caso "Guardar no hace nada" bajo Program Files.
            Log.Error("App.AsegurarCarpetasDeTrabajo", ex);
        }
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
            catch (Exception ex)
            {
                // Portapapeles inaccesible: se sigue tratando como vacío (fallback intacto),
                // pero ahora queda traza del "pegar no hace nada" que antes era silencioso.
                Log.Error("Clipboard.Read", ex);
            }
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
            var disp = AppServices.UiDispatcher;
            if (disp is null)
            {
                Log.Warn("AnalisisUi.MostrarVisor: sin DispatcherQueue de UI; no se abre el visor.");
                return;
            }

            // B-07: las asignaciones del handoff se hacen DENTRO de la lambda de UI, capturando
            // los valores. Antes se escribían en el hilo del dominio y la navegación se encolaba
            // aparte: dos MostrarVisor seguidos dejaban que el segundo productor pisara los
            // estáticos y que el primer visor los consumiera y anulara (el ctor del VM hace
            // consume-and-clear) → el SEGUNDO visor abría vacío. Ahora cada navegación encolada
            // publica su propio payload justo antes de navegar, en el mismo turno de UI.
            bool encolado = disp.TryEnqueue(() =>
            {
                VisorAnalisisColumnasFrmViewModel.UltimoContenedor = contenedor;
                VisorAnalisisColumnasFrmViewModel.UltimoGrupo = grupo;

                // Navegación por método público de la ventana (sin acoplamiento por string a
                // FindName("ContentFrame")).
                if (MainWindow is Free1X2.WinUI.MainWindow ventana)
                {
                    ventana.NavegarA(typeof(VisorAnalisisColumnasFrmPage));
                }
                else
                {
                    Log.Warn("AnalisisUi.MostrarVisor: la ventana principal no está disponible.");
                }
            });

            if (!encolado)
            {
                Log.Warn("AnalisisUi.MostrarVisor: la cola de UI rechazó la navegación (app cerrándose).");
            }
        };
    }
}
