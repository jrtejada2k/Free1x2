// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Input;
using Free1X2.WinUI.Views;
using Free1X2.WinUI.Views.Ported;
using Free1X2.WinUI.Navigation;
using Free1X2.EntradaSalida;

namespace Free1X2.WinUI;

public sealed partial class MainWindow : Window
{
    // FontFamily compartida para todos los glifos de iconos (menú + toolbar).
    private static readonly FontFamily IconFont = new("Segoe Fluent Icons");

    public MainWindow()
    {
        this.InitializeComponent();

        // Barra de título de Windows estándar: minimizar / maximizar / cerrar
        // visibles y operativos, igual que el MainForm WinForms original.
        // (No se extiende el contenido en la barra de título ni se usa AppTitleBar.)
        this.ExtendsContentIntoTitleBar = false;
        this.Title = "Free1X2";

        // Tema (U-01): aplica la preferencia guardada (Claro / Oscuro / Sistema) sobre la raíz
        // del contenido. Va ANTES de construir menús/toolbar para que el primer pintado ya use
        // la paleta elegida y no se vea un parpadeo claro→oscuro.
        Services.TemaApp.Aplicar(RaizVentana);

        AjustarTamanoVentana();

        // W8 (DT-82): panel lateral + barra con texto en lugar de los 55 iconos. Sin la barra
        // de iconos ya no se escriben al cerrar los flags MostrarTs*: parametros.free1x2
        // conserva sus valores de barras sin tocarlos (compatibilidad con el WinForms).
        ConstruirPanelLateral();
        ConstruirMenus();
        ConfigurarAtajosTeclado();
        ContentFrame.Navigate(typeof(MainPage));

        if (Environment.GetEnvironmentVariable("FREE1X2_SMOKE") == "1")
            IniciarSmokeTest();
    }

    // Tamaño inicial cercano al MainForm original (~1020 x 720). El alto se sube a 760
    // para que entren cómodamente la barra de menús + la barra de herramientas a DOS
    // filas (reserva MinHeight=63 del host) + el contenido, sin que la fila Auto del
    // Grid recorte la 2ª fila de botones.
    private void AjustarTamanoVentana()
    {
        try
        {
            this.AppWindow?.Resize(new Windows.Graphics.SizeInt32(1020, 760));

            // U-06: icono en la barra de título y en la barra de tareas. En apps DESEMPAQUETADAS
            // (WindowsPackageType=None) el AppWindow NO hereda automáticamente para el icono pequeño
            // de la barra de título el <ApplicationIcon> incrustado en el exe, así que se fija de forma
            // explícita al mismo Assets\app.ico (paridad con el icono del MainForm WinForms original).
            // Se aísla en su propio try para que un fallo de icono no impida el tamaño mínimo (U-05).
            try
            {
                string rutaIco = System.IO.Path.Combine(AppContext.BaseDirectory, "Assets", "app.ico");
                if (System.IO.File.Exists(rutaIco))
                    this.AppWindow?.SetIcon(rutaIco);
            }
            catch (Exception exIcono)
            {
                Services.Log.Error("MainWindow.SetIcon", exIcono);
            }

            // U-05: tamaño MÍNIMO de la ventana. Antes no había tope: al encoger, la barra de
            // ~55 botones a dos filas y el contenido quedaban recortados sin aviso. En Windows
            // App SDK 1.6 el OverlappedPresenter TODAVÍA no expone PreferredMinimumWidth/Height
            // (llegó en 1.7), así que el mínimo se impone interceptando WM_GETMINMAXINFO con un
            // subclass nativo de la ventana. 900x600 deja sitio a menús + barra a dos filas +
            // algo de contenido.
            InstalarTamanoMinimo();
        }
        catch (Exception ex)
        {
            // Sin AppWindow (entornos sin presentación): no es crítico, la ventana abre con su
            // tamaño por defecto. Solo se deja traza.
            Services.Log.Error("MainWindow.AjustarTamanoVentana", ex);
        }
    }

    // ===== U-05: tamaño mínimo de ventana vía WM_GETMINMAXINFO (WinAppSDK 1.6) =====

    private const int AnchoMinimoDip = 900;
    private const int AltoMinimoDip = 600;
    private const uint WM_GETMINMAXINFO = 0x0024;
    // El delegado se guarda en un campo para que el GC no lo recoja mientras la ventana vive.
    private SUBCLASSPROC _subclassProc;

    private void InstalarTamanoMinimo()
    {
        IntPtr hwnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
        _subclassProc = SubclassWndProc;
        SetWindowSubclass(hwnd, _subclassProc, IntPtr.Zero, IntPtr.Zero);
    }

    private IntPtr SubclassWndProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, IntPtr dwRefData)
    {
        if (uMsg == WM_GETMINMAXINFO)
        {
            // El mínimo va en píxeles físicos, así que se escala por el DPI del monitor.
            uint dpi = GetDpiForWindow(hWnd);
            double escala = dpi > 0 ? dpi / 96.0 : 1.0;
            var info = System.Runtime.InteropServices.Marshal.PtrToStructure<MINMAXINFO>(lParam);
            info.ptMinTrackSize.X = (int)(AnchoMinimoDip * escala);
            info.ptMinTrackSize.Y = (int)(AltoMinimoDip * escala);
            System.Runtime.InteropServices.Marshal.StructureToPtr(info, lParam, false);
            return IntPtr.Zero;
        }
        return DefSubclassProc(hWnd, uMsg, wParam, lParam);
    }

    private delegate IntPtr SUBCLASSPROC(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam, IntPtr uIdSubclass, IntPtr dwRefData);

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    private struct POINT { public int X; public int Y; }

    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    private struct MINMAXINFO
    {
        public POINT ptReserved;
        public POINT ptMaxSize;
        public POINT ptMaxPosition;
        public POINT ptMinTrackSize;
        public POINT ptMaxTrackSize;
    }

    [System.Runtime.InteropServices.DllImport("comctl32.dll", SetLastError = true)]
    private static extern bool SetWindowSubclass(IntPtr hWnd, SUBCLASSPROC pfnSubclass, IntPtr uIdSubclass, IntPtr dwRefData);

    [System.Runtime.InteropServices.DllImport("comctl32.dll")]
    private static extern IntPtr DefSubclassProc(IntPtr hWnd, uint uMsg, IntPtr wParam, IntPtr lParam);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    private static extern uint GetDpiForWindow(IntPtr hwnd);

    // Convierte un codepoint hex (p.ej. "E80F") al glifo de Segoe Fluent Icons.
    // Usar codepoints en lugar de caracteres literales garantiza que el glifo se
    // resuelve bien (sin cuadros vacíos) sin depender de la codificación del .cs.
    private static string Glifo(string hex) => char.ConvertFromUtf32(Convert.ToInt32(hex, 16));

    // ===== Barra de menús (misma organización que el programa WinForms original) =====
    // Orden de menús idéntico al MainForm: Free1X2, Archivo, Ver, Combinación, Filtros,
    // Operaciones, Utilidades (mainMenu.Items.AddRange en MainForm.Designer.cs).
    // Cada entrada del flyout navega a la pantalla real portada y lleva su icono. Las
    // condiciones (Variantes, Dibujos, etc.) se abren desde la rejilla de condiciones de
    // la pantalla Inicio, igual que en el original (campo Condiciones).
    private void ConstruirMenus()
    {
        BarraMenu.Items.Add(Menu("Free1x2",
            ("E80F", "Inicio", typeof(MainPage)),
            null,
            ("E713", "Configuración…", typeof(ConfiguracionFrmPage)),
            ("E9D9", "Configurar análisis…", typeof(ConfiguracionAnalisisFrmPage)),
            null,
            ("E946", "Acerca de…", typeof(AcercaDeFrmPage)),
            ("E77B", "Créditos…", typeof(CreditosFrmPage)),
            null,
            ("E7E8", "Salir", null)));   // null => cierra la ventana.

        // W8 (DT-82): las acciones que solo estaban en la barra de iconos (tsArchivo) pasan a
        // este menú, en el mismo orden que el menú Archivo de la app Mac.
        var menuArchivo = new MenuBarItem { Title = "Archivo" };
        menuArchivo.Items.Add(ItemAccion("E8A5", "Nueva combinación", AccionInicio.NuevaCombinacion));
        menuArchivo.Items.Add(ItemAccion("E8E5", "Abrir combinación…", AccionInicio.AbrirCombinacion));
        menuArchivo.Items.Add(ItemAccion("E74E", "Guardar combinación", AccionInicio.GuardarCombinacion));
        menuArchivo.Items.Add(ItemAccion("E792", "Guardar combinación como…", AccionInicio.GuardarCombinacionComo));
        menuArchivo.Items.Add(new MenuFlyoutSeparator());
        menuArchivo.Items.Add(ItemAccion("E8E5", "Abrir equipos…", AccionInicio.AbrirEquipos));
        menuArchivo.Items.Add(ItemAccion("E74E", "Guardar equipos…", AccionInicio.GuardarEquipos));
        menuArchivo.Items.Add(new MenuFlyoutSeparator());
        menuArchivo.Items.Add(ItemFlyout("E8A5", "Boleto / combinación (Inicio)", typeof(MainPage)));
        menuArchivo.Items.Add(ItemFlyout("E716", "Gestión de equipos…", typeof(GestorEquiposFrmPage)));
        menuArchivo.Items.Add(ItemFlyout("E8B5", "Importar / exportar columnas…", typeof(ImportExportFrmPage)));
        menuArchivo.Items.Add(ItemFlyout("E896", "Obtener boletos online…", typeof(DescargaBoletoFrmPage)));
        menuArchivo.Items.Add(new MenuFlyoutSeparator());
        menuArchivo.Items.Add(ItemAccion("E74D", "Borrar temporales…", AccionInicio.BorrarTemporales));
        menuArchivo.Items.Add(ItemAccion("E74D", "Borrar informes…", AccionInicio.BorrarInformes));
        BarraMenu.Items.Add(menuArchivo);

        // Menú "Ver" en 3ª posición (tras Archivo), como en el MainForm original.
        // W8 (DT-82): sin el submenú "Barras de Herramientas" (ya no hay barra de iconos).
        var menuVer = Menu("Ver",
            ("E80F", "Inicio", typeof(MainPage)),
            ("E8A1", "Ver boletos…", typeof(VerBoletosPage)),
            ("E9D9", "Gráfico de columnas…", typeof(GraficoColumnasFrmPage)),
            ("E9D9", "Estadísticas…", typeof(AnastaticsPage)),
            null,
            ("E713", "Configuración…", typeof(ConfiguracionFrmPage)));
        menuVer.Items.Add(new MenuFlyoutSeparator());
        // "Listado de condiciones" — en el original es el item que sigue a "Barras de Herramientas"
        // dentro del menú Ver (menuVer.DropDownItems, MainForm.Designer.cs:1455-1457). Navega a la
        // página portada (handler legacy: listadoDeCondicionesToolStripMenuItem_Click → ListadoCondicionesFrm).
        menuVer.Items.Add(ItemFlyout("E9D5", "Listado de condiciones", typeof(ListadoCondicionesFrmPage)));
        // "Tema" (U-01) — AÑADIDO al FINAL del menú Ver, sin reordenar nada de lo anterior.
        // No existía en el MainForm original: es la entrada que da acceso a la paleta oscura
        // que Themes/Tokens.xaml ya define y que App.xaml mantenía apagada.
        menuVer.Items.Add(new MenuFlyoutSeparator());
        menuVer.Items.Add(ConstruirSubmenuTema());
        // "Idioma" (U-10) — AÑADIDO junto al submenú Tema, mismo patrón. No existía en el MainForm
        // original: es la entrada del selector de idioma (Español / English) que expone la
        // localización piloto de CreditosFrmPage.
        menuVer.Items.Add(ConstruirSubmenuIdioma());
        BarraMenu.Items.Add(menuVer);

        BarraMenu.Items.Add(Menu("Combinación",
            ("E950", "Calcular…", typeof(CalculaColumnasFrmPage)),
            ("E8EF", "Calcular varias…", typeof(CalculaColumnasMultipleFrmPage)),
            null,
            ("E8A1", "Ver boletos…", typeof(VerBoletosPage)),
            // "Ver boletos en editor de texto" — en el menú Combinación del original
            // (verBoletosEnEditorDeTextoToolStripMenuItem, MainForm.cs:781). Navega a la página
            // portada (handler legacy: abre fichero de columnas + VerBoletosEnEditorFrm).
            ("E8A1", "Ver boletos en editor de texto…", typeof(VerBoletosEnEditorFrmPage)),
            ("E749", "Imprimir boletos…", typeof(ImprimirBoletoFrmPage)),
            null,
            ("E74D", "Reducir…", typeof(ReductorFrmPage)),
            ("E73E", "Escrutinios…", typeof(EscrutiniosFrmPage)),
            // "Escrutar combinaciones" — en el menú Combinación del original
            // (escrutarCombinacionesToolStripMenuItem, MainForm.Designer.cs:1551). Navega a la
            // página portada (handler legacy: MEscrutinioComb → EscrutiniosFrm de combinaciones).
            ("E762", "Escrutar combinaciones…", typeof(EscrutarCombinacionesFrmPage)),
            null,
            ("E9F5", "Analizar combinación…", typeof(AnalizarCombinacionFrmPage)),
            ("E9D9", "Gráfico de columnas…", typeof(GraficoColumnasFrmPage)),
            ("E9D9", "Probabilidades…", typeof(ProbabilidadPremiosPage)),
            ("E9D9", "Estadísticas…", typeof(AnastaticsPage)),
            null,
            ("E710", "Añadir Pleno al 15…", typeof(AgregaP15FrmPage))));

        BarraMenu.Items.Add(Menu("Filtros",
            ("E71C", "Combinar filtros…", typeof(CombinarFiltrosPage)),
            ("E8AB", "Diferencias entre filtros…", typeof(DiFiltrosPage)),
            null,
            ("E71C", "Filtro Coincidencias…", typeof(CoincidenciasPage)),
            ("E71C", "Filtro Aidomnou…", typeof(aidomnouPage)),
            ("E71C", "Filtro Pim…", typeof(GeneraPimPage))));

        BarraMenu.Items.Add(Menu("Operaciones",
            ("E950", "Álgebra de columnas…", typeof(AlgebraColumnasFrmPage)),
            ("E8AB", "Transposición…", typeof(TransposicionFrmPage)),
            ("E950", "Multiplicador…", typeof(MultiplicadorFrmPage)),
            ("E950", "Fraccionador…", typeof(FraccionadorFrmPage)),
            ("E7AD", "Rotación de signos…", typeof(RotacionDeSignosFrmPage))));

        BarraMenu.Items.Add(Menu("Utilidades",
            ("E74A", "Sube categoría…", typeof(SubirCategoriaFrmPage)),
            ("E9E9", "Modificador %…", typeof(ModificadorFrmPage)),
            null,
            ("E710", "Generador CP…", typeof(GenerarCPsPage)),
            ("E8A1", "Columnas GEPT…", typeof(GEPTFrmPage)),
            null,
            ("E8AB", "Diferencias entre columnas…", typeof(DifColsPage)),
            ("E8CB", "Ordenar por probabilidad…", typeof(OrdenarPorProbabilidadFrmPage)),
            null,
            ("E762", "Selector JuanM…", typeof(SelecJMPage)),
            ("E762", "Selector MarioSan…", typeof(SelectorMSPage)),
            null,
            ("E9D9", "Rentabilidad…", typeof(RentabilidadFrmPage)),
            ("E9D9", "Tramificar…", typeof(TramificarFormPage)),
            ("E735", "Premiadas…", typeof(PremiadasFrmPage)),
            ("E9D9", "Estimación de premios…", typeof(EstimadorPremiosFrmPage)),
            ("E713", "Banco de pruebas…", typeof(BancoPruebasFrmPage)),
            null,
            // "Compresor z3q" y "EstuCol" — últimos items del menú Utilidades del original
            // (compresorToolStripMenuItem / estuColToolStripMenuItem, MainForm.Designer.cs:1875-1876).
            // Navegan a sus páginas portadas (handlers legacy: compresorToolStripMenuItem_Click →
            // Compresor; estuColToolStripMenuItem_Click → EstucolFrm).
            ("E8B7", "Compresor z3q…", typeof(CompresorPage)),
            ("E8B7", "EstuCol…", typeof(EstucolFrmPage))));
    }

    // Crea un menú superior. El título a nivel superior va SIN icono (MenuBarItem no
    // admite Icon y un glifo en el texto se dibujaría con la fuente del menú → cuadro
    // vacío). Cada item del flyout sí lleva su FontIcon (Segoe Fluent Icons lo dibuja
    // bien). Un elemento null inserta un separador; si la página de un item es null el
    // item cierra la ventana (entrada "Salir").
    private MenuBarItem Menu(string titulo, params (string glifoHex, string label, Type? page)?[] items)
    {
        var menu = new MenuBarItem { Title = titulo };
        foreach (var item in items)
        {
            if (item is null)
            {
                menu.Items.Add(new MenuFlyoutSeparator());
                continue;
            }
            var (gHex, label, page) = item.Value;
            var mfi = new MenuFlyoutItem
            {
                Text = label,
                Icon = new FontIcon { Glyph = Glifo(gHex), FontFamily = IconFont },
            };
            if (page is null)
                mfi.Click += (_, _) => this.Close();
            else
                mfi.Click += (_, _) => Navegar(page);
            menu.Items.Add(mfi);
        }
        return menu;
    }

    // Crea un único MenuFlyoutItem con icono que navega a 'page' (misma lógica que los items de
    // Menu(), para añadir entradas sueltas a un menú ya construido).
    private MenuFlyoutItem ItemFlyout(string glifoHex, string label, Type page)
    {
        var mfi = new MenuFlyoutItem
        {
            Text = label,
            Icon = new FontIcon { Glyph = Glifo(glifoHex), FontFamily = IconFont },
        };
        mfi.Click += (_, _) => Navegar(page);
        return mfi;
    }

    // Igual que ItemFlyout, pero ejecuta una acción sobre la pantalla Inicio (NavegarConAccion).
    private MenuFlyoutItem ItemAccion(string glifoHex, string label, AccionInicio accion)
    {
        var mfi = new MenuFlyoutItem
        {
            Text = label,
            Icon = new FontIcon { Glyph = Glifo(glifoHex), FontFamily = IconFont },
        };
        mfi.Click += (_, _) => NavegarConAccion(accion);
        return mfi;
    }

    /// <summary>
    /// Submenú «Ver → Tema» (U-01): las tres opciones de tema, mutuamente excluyentes, con la
    /// activa marcada (MenuFlyoutSubItem con icono + items construidos en bucle); se usa
    /// <see cref="RadioMenuFlyoutItem"/> porque la selección es EXCLUSIVA. El cambio se aplica en vivo y se persiste en
    /// <see cref="Services.TemaApp"/>.
    /// </summary>
    private MenuFlyoutSubItem ConstruirSubmenuTema()
    {
        var sub = new MenuFlyoutSubItem
        {
            Text = "Tema",
            Icon = new FontIcon { Glyph = Glifo("E706"), FontFamily = IconFont },
        };

        (Services.TemaApp.Opcion opcion, string label)[] items =
        {
            (Services.TemaApp.Opcion.Claro,   "Claro"),
            (Services.TemaApp.Opcion.Oscuro,  "Oscuro"),
            (Services.TemaApp.Opcion.Sistema, "Sistema"),
        };

        foreach (var (opcion, label) in items)
        {
            var item = new RadioMenuFlyoutItem
            {
                Text = label,
                GroupName = "TemaApp",                               // exclusividad entre los 3
                IsChecked = Services.TemaApp.Actual == opcion,        // refleja la preferencia viva
            };
            Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(item, "Tema " + label);
            item.Click += (_, _) => Services.TemaApp.Cambiar(opcion);
            sub.Items.Add(item);
        }
        return sub;
    }

    /// <summary>
    /// Submenú «Ver → Idioma» (U-10): las dos opciones de idioma, mutuamente excluyentes, con la
    /// activa marcada. Mismo patrón EXACTO que <see cref="ConstruirSubmenuTema"/>
    /// (MenuFlyoutSubItem con icono + <see cref="RadioMenuFlyoutItem"/> en bucle, selección
    /// exclusiva por GroupName). Al cambiar, se persiste en <see cref="Services.IdiomaApp"/> y se
    /// RE-NAVEGA a la página actual: los recursos <c>x:Uid</c> se resuelven al CARGAR la página, así
    /// que el cambio de idioma se refleja al recrearla (WinUI no repinta los <c>x:Uid</c> en vivo).
    /// </summary>
    private MenuFlyoutSubItem ConstruirSubmenuIdioma()
    {
        var sub = new MenuFlyoutSubItem
        {
            Text = "Idioma",
            Icon = new FontIcon { Glyph = Glifo("E774"), FontFamily = IconFont }, // Globe
        };

        (Services.IdiomaApp.Opcion opcion, string label)[] items =
        {
            (Services.IdiomaApp.Opcion.Espanol, "Español"),
            (Services.IdiomaApp.Opcion.Ingles,  "English"),
        };

        foreach (var (opcion, label) in items)
        {
            var item = new RadioMenuFlyoutItem
            {
                Text = label,
                GroupName = "IdiomaApp",                               // exclusividad entre los 2
                IsChecked = Services.IdiomaApp.Actual == opcion,        // refleja la preferencia viva
            };
            Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(item, "Idioma " + label);
            item.Click += (_, _) =>
            {
                Services.IdiomaApp.Cambiar(opcion);
                RecargarPaginaActual();
            };
            sub.Items.Add(item);
        }
        return sub;
    }

    /// <summary>
    /// Re-navega el Frame de contenido a la página que ya está mostrando, para que sus recursos
    /// <c>x:Uid</c> se re-resuelvan con el idioma recién elegido (U-10). Los <c>x:Uid</c> se aplican
    /// al CARGAR la página, no en vivo, así que un cambio de idioma solo se ve al recrear la página.
    /// Se elimina la entrada duplicada que la re-navegación deja en la pila de "atrás".
    /// LÍMITE: recrear la página REINICIA su estado en memoria (p. ej. un boleto en edición). El
    /// cambio de idioma es una acción deliberada y poco frecuente; aun así este comportamiento en
    /// runtime NO se ha podido verificar (R7: no se lanza la app). No lanza.
    /// </summary>
    private void RecargarPaginaActual()
    {
        var tipo = ContentFrame.CurrentSourcePageType;
        if (tipo is null) return;
        try
        {
            ContentFrame.Navigate(tipo);
            if (ContentFrame.BackStack.Count > 0)
                ContentFrame.BackStack.RemoveAt(ContentFrame.BackStack.Count - 1);
        }
        catch (Exception ex)
        {
            Services.Log.Error("MainWindow.RecargarPaginaActual", ex);
        }
    }

    // ===== Panel lateral (W8, DT-82) =====
    // Las pantallas de los menús y de la antigua barra de 55 iconos, por nombre y por grupo, con
    // buscador. Las entradas son exactamente las del panel de la app Mac (Lateral.swift), con el
    // PNG original de la barra (Assets/Toolbar) o, si no lo había, un glifo de Segoe Fluent Icons.

    private sealed record EntradaLateral(string Titulo, Type Pagina, string Icono);

    private static EntradaLateral E(string titulo, Type pagina, string icono) => new(titulo, pagina, icono);

    private static readonly (string Grupo, EntradaLateral[] Entradas)[] SeccionesLateral =
    {
        ("", new[] { E("Inicio", typeof(MainPage), "E80F") }),
        ("Combinación", new[]
        {
            E("Calcular columnas", typeof(CalculaColumnasFrmPage), "bCalcular"),
            E("Calcular varias combinaciones", typeof(CalculaColumnasMultipleFrmPage), "bCalcularM"),
            E("Ver boletos", typeof(VerBoletosPage), "bVerBoletos"),
            E("Ver boletos en editor de texto", typeof(VerBoletosEnEditorFrmPage), "E8A1"),
            E("Imprimir boletos", typeof(ImprimirBoletoFrmPage), "bImprimirBoletos"),
            E("Reducir", typeof(ReductorFrmPage), "bReducir"),
            E("Escrutinios", typeof(EscrutiniosFrmPage), "bEscrutinio"),
            E("Escrutar combinaciones", typeof(EscrutarCombinacionesFrmPage), "bEscrutarComb"),
            E("Analizar combinación", typeof(AnalizarCombinacionFrmPage), "E9F5"),
            E("Análisis de columnas", typeof(AnalizarFicheroFrmPage), "bAnalisisColumnas"),
            E("Análisis de fallos", typeof(ColGanadoraFrmPage), "bAnalisisFallos"),
            E("Análisis de signos", typeof(VSignosFrmPage), "bAnalisisSignos"),
            E("Gráfico de columnas", typeof(GraficoColumnasFrmPage), "E9D9"),
            E("Probabilidades", typeof(ProbabilidadPremiosPage), "bProbabilidades"),
            E("Estadísticas", typeof(AnastaticsPage), "bEstadisticas"),
            E("Añadir pleno al 15", typeof(AgregaP15FrmPage), "bP15"),
        }),
        ("Filtros", new[]
        {
            E("Combinar filtros", typeof(CombinarFiltrosPage), "bCombinarFiltros"),
            E("Diferencias entre filtros", typeof(DiFiltrosPage), "bDiferenciasFiltros"),
            E("Filtro Coincidencias", typeof(CoincidenciasPage), "bFiltroCoincidencias"),
            E("Filtro Aidomnou", typeof(aidomnouPage), "bFiltroAidomnou"),
            E("Filtro Pim", typeof(GeneraPimPage), "bFiltroPim"),
        }),
        ("Operaciones", new[]
        {
            E("Álgebra de columnas", typeof(AlgebraColumnasFrmPage), "bAlgebra"),
            E("Transposición", typeof(TransposicionFrmPage), "bTransposicion"),
            E("Multiplicador", typeof(MultiplicadorFrmPage), "bMultiplicacion"),
            E("Fraccionador", typeof(FraccionadorFrmPage), "bFraccionador"),
            E("Rotación de signos", typeof(RotacionDeSignosFrmPage), "bRotacion"),
        }),
        ("Utilidades", new[]
        {
            E("Subir categoría", typeof(SubirCategoriaFrmPage), "bSubeCategoria"),
            E("Modificador de porcentajes", typeof(ModificadorFrmPage), "bModificadorPct"),
            E("Generador de CPs", typeof(GenerarCPsPage), "bGeneradorCPs"),
            E("Columnas GEPT", typeof(GEPTFrmPage), "bColumnasGEPT"),
            E("Diferencias entre columnas", typeof(DifColsPage), "bDiferenciasColumnas"),
            E("Ordenar por probabilidad", typeof(OrdenarPorProbabilidadFrmPage), "bProbabilidad"),
            E("Selector JuanM", typeof(SelecJMPage), "bSelectorJuanM"),
            E("Selector MarioSan", typeof(SelectorMSPage), "bSelectorMarioSan"),
            E("Rentabilidad", typeof(RentabilidadFrmPage), "bRentabilidad"),
            E("Tramificar", typeof(TramificarFormPage), "bTramificar"),
            E("Premiadas", typeof(PremiadasFrmPage), "bPremiadas"),
            E("Estimación de premios", typeof(EstimadorPremiosFrmPage), "bEstimacion"),
            E("Banco de pruebas", typeof(BancoPruebasFrmPage), "bBancoPruebas"),
            E("Análisis de grupos", typeof(AnaCombiPage), "bAnalisisGrupos"),
            E("Reducciones perfectas", typeof(FrmReducidasPerfectasPage), "bRedPerfectas"),
            E("Dependencia lineal", typeof(FrmDependenciaLinealPage), "bDependenciaLineal"),
            E("Compresor z3q", typeof(CompresorPage), "E8B7"),
            E("EstuCol", typeof(EstucolFrmPage), "E80A"),
        }),
        ("Archivo", new[]
        {
            E("Gestión de equipos", typeof(GestorEquiposFrmPage), "bGestorEquipos"),
            E("Importar / exportar columnas", typeof(ImportExportFrmPage), "bImportExport"),
            E("Obtener boletos online", typeof(DescargaBoletoFrmPage), "bObtenerBoletos"),
        }),
        ("Ajustes y ayuda", new[]
        {
            E("Configuración", typeof(ConfiguracionFrmPage), "bConfig"),
            E("Configurar análisis", typeof(ConfiguracionAnalisisFrmPage), "bConfAnalisis"),
            E("Listado de condiciones", typeof(ListadoCondicionesFrmPage), "E9D5"),
            E("Ayuda", typeof(AyudaFrmPage), "bAyuda"),
            E("Acerca de", typeof(AcercaDeFrmPage), "bAcercaDe"),
        }),
    };

    private void ConstruirPanelLateral() => RellenarPanelLateral("");

    // Vuelve a crear las entradas que contienen el filtro (sin distinguir tildes ni mayúsculas).
    // Los grupos sin ninguna entrada visible no muestran su cabecera.
    private void RellenarPanelLateral(string filtro)
    {
        string f = Normalizar(filtro);
        PanelLateral.MenuItems.Clear();
        foreach (var (grupo, entradas) in SeccionesLateral)
        {
            var visibles = entradas.Where(e => f.Length == 0 || Normalizar(e.Titulo).Contains(f)).ToList();
            if (visibles.Count == 0) continue;
            if (grupo.Length > 0)
                PanelLateral.MenuItems.Add(new NavigationViewItemHeader { Content = grupo });
            foreach (var e in visibles)
            {
                var item = new NavigationViewItem { Content = e.Titulo, Tag = e, Icon = IconoLateral(e.Icono) };
                Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(item, e.Titulo);
                PanelLateral.MenuItems.Add(item);
            }
        }
        MarcarEntradaActual();
    }

    private static IconElement IconoLateral(string icono) =>
        icono.StartsWith("b", StringComparison.Ordinal)
            ? new BitmapIcon { UriSource = new Uri($"ms-appx:///Assets/Toolbar/{icono}.png"), ShowAsMonochrome = false }
            : new FontIcon { Glyph = Glifo(icono), FontFamily = IconFont };

    // Minúsculas y sin diacríticos: «algebra» encuentra «Álgebra de columnas».
    private static string Normalizar(string texto)
    {
        var sb = new System.Text.StringBuilder();
        foreach (char c in (texto ?? "").Trim().Normalize(System.Text.NormalizationForm.FormD))
        {
            if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                sb.Append(char.ToLowerInvariant(c));
        }
        return sb.ToString();
    }

    private IEnumerable<NavigationViewItem> EntradasVisibles() => PanelLateral.MenuItems.OfType<NavigationViewItem>();

    private void BuscadorPantallas_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            RellenarPanelLateral(sender.Text);
    }

    // Intro en el buscador: abre la primera pantalla que queda.
    private void BuscadorPantallas_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        RellenarPanelLateral(sender.Text);
        if (EntradasVisibles().FirstOrDefault()?.Tag is EntradaLateral e)
            IrARaiz(e.Pagina);
    }

    private void PanelLateral_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (args.InvokedItemContainer?.Tag is EntradaLateral e)
            IrARaiz(e.Pagina);
    }

    // Pulsar una entrada la pone como RAÍZ de la navegación (no apila), como en la app Mac.
    private void IrARaiz(Type page)
    {
        if (ContentFrame.CurrentSourcePageType != page)
            ContentFrame.Navigate(page);
        ContentFrame.BackStack.Clear();
        MarcarEntradaActual();
    }

    // Marca la pantalla raíz de la pila; en una subpantalla abierta desde otra, la de origen.
    private void MarcarEntradaActual()
    {
        Type? raiz = ContentFrame.BackStack.Count > 0
            ? ContentFrame.BackStack[0].SourcePageType
            : ContentFrame.CurrentSourcePageType;
        PanelLateral.SelectedItem = EntradasVisibles().FirstOrDefault(i => (i.Tag as EntradaLateral)?.Pagina == raiz);
    }

    // ===== Barra de acciones (W8, DT-82) =====
    // Calcular, Reducir y Jornada son de Inicio: solo se ven allí y actúan sobre la página viva.

    private void ContentFrame_Navigated(object sender, Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        var v = e.Content is MainPage ? Visibility.Visible : Visibility.Collapsed;
        BotonCalcular.Visibility = v;
        BotonReducir.Visibility = v;
        BotonJornada.Visibility = v;
        SeparadorInicio.Visibility = v;
        MarcarEntradaActual();
    }

    private void BotonCalcular_Click(object sender, RoutedEventArgs e)
    {
        if (ContentFrame.Content is MainPage p) p.ViewModel.CalcularCommand.Execute(null);
    }

    private void BotonReducir_Click(object sender, RoutedEventArgs e)
    {
        if (ContentFrame.Content is MainPage p) p.ViewModel.ReducirCommand.Execute(null);
    }

    private void Jornada_Click(object sender, RoutedEventArgs e)
    {
        if (ContentFrame.Content is MainPage p && sender is FrameworkElement { Tag: string pais }
            && p.ViewModel.CargarJornadaCommand.CanExecute(pais))
            p.ViewModel.CargarJornadaCommand.Execute(pais);
    }

    private void BotonNueva_Click(object sender, RoutedEventArgs e) => NavegarConAccion(AccionInicio.NuevaCombinacion);

    private void BotonAbrir_Click(object sender, RoutedEventArgs e) => NavegarConAccion(AccionInicio.AbrirCombinacion);

    private void BotonGuardar_Click(object sender, RoutedEventArgs e) => NavegarConAccion(AccionInicio.GuardarCombinacion);

    private void BotonEscrutinio_Click(object sender, RoutedEventArgs e) => Navegar(typeof(EscrutiniosFrmPage));

    private void Navegar(Type page)
    {
        if (ContentFrame.CurrentSourcePageType != page)
            ContentFrame.Navigate(page);
    }

    /// <summary>
    /// Navega el Frame de contenido a la página indicada. Punto de entrada PÚBLICO para el
    /// código que no tiene acceso al Frame (p. ej. el handoff del visor de análisis que cablea
    /// <c>App.CablearHooksDominio</c>), en lugar de buscarlo por nombre con
    /// <c>FindName("ContentFrame")</c> — acoplamiento por string que se rompía en silencio (B-07).
    /// Navega SIEMPRE, incluso si ya se está en esa página: el visor consume un payload nuevo en
    /// cada navegación, así que re-navegar es justo lo que hace falta (comportamiento previo).
    /// </summary>
    public void NavegarA(Type page) => ContentFrame.Navigate(page);

    // U-04: atajos de teclado. El MainForm WinForms original NO tenía ninguno; son
    // funcionalidad nueva (decisión del dueño). Se registran a nivel de ventana en la raíz del
    // contenido (RaizVentana), así que funcionan desde cualquier página. Las acciones de boleto
    // (Nueva/Abrir/Guardar) pasan por NavegarConAccion, que si no estás en Inicio navega allí y
    // ejecuta la acción — el mismo comportamiento que pulsar el botón de la barra.
    private void ConfigurarAtajosTeclado()
    {
        try
        {
            AgregarAtajo(Windows.System.VirtualKey.N, Windows.System.VirtualKeyModifiers.Control,
                () => NavegarConAccion(AccionInicio.NuevaCombinacion));
            AgregarAtajo(Windows.System.VirtualKey.O, Windows.System.VirtualKeyModifiers.Control,
                () => NavegarConAccion(AccionInicio.AbrirCombinacion));
            AgregarAtajo(Windows.System.VirtualKey.S, Windows.System.VirtualKeyModifiers.Control,
                () => NavegarConAccion(AccionInicio.GuardarCombinacion));
            AgregarAtajo(Windows.System.VirtualKey.F5, Windows.System.VirtualKeyModifiers.None,
                () => Navegar(typeof(CalculaColumnasFrmPage)));
            AgregarAtajo(Windows.System.VirtualKey.F1, Windows.System.VirtualKeyModifiers.None,
                () => Navegar(typeof(AyudaFrmPage)));
            AgregarAtajo(Windows.System.VirtualKey.Escape, Windows.System.VirtualKeyModifiers.None,
                () => { if (ContentFrame.CanGoBack) ContentFrame.GoBack(); });
        }
        catch (Exception ex)
        {
            Services.Log.Error("MainWindow.ConfigurarAtajosTeclado", ex);
        }
    }

    private void AgregarAtajo(Windows.System.VirtualKey tecla, Windows.System.VirtualKeyModifiers mod, Action accion)
    {
        var atajo = new KeyboardAccelerator { Key = tecla, Modifiers = mod };
        atajo.Invoked += (_, e) => { e.Handled = true; accion(); };
        RaizVentana.KeyboardAccelerators.Add(atajo);
    }

    // Ejecuta una acción de la barra "Archivo" sobre la pantalla Inicio. Si ya estamos en
    // MainPage, invoca el comando sobre la instancia VIVA (preserva el boleto en edición); si
    // no, navega a MainPage pasando el token como parámetro (OnNavigatedTo lo ejecuta tras
    // cargar). En ambos casos termina en MainPage, igual que el flujo original que operaba
    // sobre el boleto de la pantalla principal.
    private void NavegarConAccion(AccionInicio accion)
    {
        if (ContentFrame.Content is MainPage paginaViva)
        {
            // B-06: la Task se OBSERVA (antes `_ = ...`): una excepción en Abrir/Guardar
            // combinación se perdía con la Task descartada y la acción fallaba en silencio.
            Services.AppServices.EjecutarObservandoErrores(
                () => paginaViva.ViewModel.EjecutarAccionAsync(accion),
                "MainWindow.NavegarConAccion → " + accion);
        }
        else
        {
            ContentFrame.Navigate(typeof(MainPage), accion);
        }
    }

    // ===== Smoke test gated por FREE1X2_SMOKE (permanente, no-op sin la env var) =====

    private DispatcherQueueTimer _smokeTimer;
    private List<Type> _smokeRuta;
    private int _smokeIndice;
    private int _smokeOk;
    private int _smokeFail;
    private string _smokeLog = "";

    private void IniciarSmokeTest()
    {
        _smokeLog = Path.Combine(Path.GetTempPath(), "free1x2_smoke.log");
        try { File.WriteAllText(_smokeLog, "SMOKE START\r\n"); }
        catch (Exception ex) { Services.Log.Error("MainWindow.IniciarSmokeTest", ex); }

        _smokeRuta = new List<Type> { typeof(MainPage) };
        foreach (var p in PortedPagesRegistry.All)
            _smokeRuta.Add(p.PageType);

        _smokeIndice = 0;
        _smokeOk = 0;
        _smokeFail = 0;

        _smokeTimer = DispatcherQueue.CreateTimer();
        _smokeTimer.Interval = TimeSpan.FromMilliseconds(120);
        _smokeTimer.Tick += SmokeTimer_Tick;
        _smokeTimer.Start();
    }

    private void SmokeTimer_Tick(DispatcherQueueTimer sender, object args)
    {
        if (_smokeRuta == null) { return; }

        if (_smokeIndice >= _smokeRuta.Count)
        {
            _smokeTimer?.Stop();
            int total = _smokeOk + _smokeFail;
            SmokeAppend($"SMOKE DONE total={total} ok={_smokeOk} fail={_smokeFail}");
            Application.Current.Exit();
            return;
        }

        Type pageType = _smokeRuta[_smokeIndice++];
        try
        {
            ContentFrame.Navigate(pageType);
            _smokeOk++;
        }
        catch (Exception ex)
        {
            _smokeFail++;
            SmokeAppend($"FAIL {pageType.FullName}: {ex.GetType().Name}: {ex.Message}");
        }
    }

    private void SmokeAppend(string linea)
    {
        try { File.AppendAllText(_smokeLog, linea + "\r\n"); }
        catch (Exception ex) { Services.Log.Error("MainWindow.SmokeAppend", ex); }
    }
}
