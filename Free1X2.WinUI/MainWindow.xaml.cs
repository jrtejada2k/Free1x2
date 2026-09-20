// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.IO;
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

        ConstruirToolbar();
        ConstruirMenus();
        ConfigurarAtajosTeclado();
        ContentFrame.Navigate(typeof(MainPage));

        // Persiste la visibilidad de las barras al cerrar, igual que el MainForm
        // WinForms original (Salir() -> AConfiguracion.GuardarToolBarsVisibles,
        // MainForm.cs:178). El motor lazy-carga los flags MostrarTs* desde
        // parametros.free1x2 al primer acceso, por lo que aquí solo se vuelven a
        // escribir los valores vivos (mismo orden de argumentos que el original).
        this.Closed += (_, _) => GuardarBarrasHerramientas();

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

        BarraMenu.Items.Add(Menu("Archivo",
            ("E8A5", "Boleto / combinación (Inicio)", typeof(MainPage)),
            null,
            ("E716", "Gestión de equipos…", typeof(GestorEquiposFrmPage)),
            ("E8B5", "Importar / exportar columnas…", typeof(ImportExportFrmPage))));

        // Menú "Ver" en 3ª posición (tras Archivo), como en el MainForm original.
        // Incluye el submenú "Barras de Herramientas" (personalizarToolStripMenuItem
        // en MainForm.Designer.cs:1475) con un item conmutable por cada uno de los 6
        // ToolStrips, igual que el original.
        var menuVer = Menu("Ver",
            ("E80F", "Inicio", typeof(MainPage)),
            ("E8A1", "Ver boletos…", typeof(VerBoletosPage)),
            ("E9D9", "Gráfico de columnas…", typeof(GraficoColumnasFrmPage)),
            ("E9D9", "Estadísticas…", typeof(AnastaticsPage)),
            null,
            ("E713", "Configuración…", typeof(ConfiguracionFrmPage)));
        menuVer.Items.Add(new MenuFlyoutSeparator());
        menuVer.Items.Add(ConstruirSubmenuBarrasHerramientas());
        // "Listado de condiciones" — en el original es el item que sigue a "Barras de Herramientas"
        // dentro del menú Ver (menuVer.DropDownItems, MainForm.Designer.cs:1455-1457). Navega a la
        // página portada (handler legacy: listadoDeCondicionesToolStripMenuItem_Click → ListadoCondicionesFrm).
        menuVer.Items.Add(ItemFlyout("E9D5", "Listado de condiciones", typeof(ListadoCondicionesFrmPage)));
        // "Tema" (U-01) — AÑADIDO al FINAL del menú Ver, sin reordenar nada de lo anterior.
        // No existía en el MainForm original: es la entrada que da acceso a la paleta oscura
        // que Themes/Tokens.xaml ya define y que App.xaml mantenía apagada.
        menuVer.Items.Add(new MenuFlyoutSeparator());
        menuVer.Items.Add(ConstruirSubmenuTema());
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

    // ===== Barra de herramientas: réplica 1:1 de los 6 ToolStrips del MainForm =====
    // El programa WinForms muestra por defecto los SEIS ToolStrips (config en
    // parametros.free1x2; los seis flags por defecto = true, ver
    // VariablesGlobales.SetDefaultValues líneas 162-167). El orden visual lo fija
    // ObtenerPosicionBarraHerramientas (MainForm.cs línea 78):
    //     tsFree, tsArchivo, tsOperaciones, tsCombinacion, tsFiltros, tsUtilidades
    // Cada strip se separa del siguiente con un separador vertical (son ToolStrip
    // independientes en el original). Los botones, su orden, su ToolTipText y su icono
    // se toman EXACTAMENTE de MainForm.Designer.cs (Items.AddRange de cada ToolStrip)
    // y MainForm.resx (imagen embebida por botón, extraída a Assets/Toolbar/<botón>.png).
    // No se inventan botones ni se reutiliza ningún icono.
    //
    // Mostrar/ocultar por grupo (feature "Ver -> Barras de Herramientas"): cada grupo
    // registra TODOS sus elementos (botones + separador final) en _gruposBarra para
    // poder mostrarlos/ocultarlos como una unidad, igual que tsX.Visible en WinForms.
    // La visibilidad inicial se siembra desde VariablesGlobales.MostrarTs* (réplica de
    // InicializarBarrasHerramientas, MainForm.cs:63-71).
    //
    // Acciones que en WinForms operan sobre 'pronosticos' (ficheros del boleto base) se
    // enrutan a la pantalla Inicio (MainPage), donde vive el boleto, igual que ya hacía
    // el menú: misma navegación/flujo que el original, solo cambia la capa visual.

    // Identificadores de grupo en el MISMO orden visual que ObtenerPosicionBarraHerramientas.
    private enum GrupoBarra { Free, Archivo, Operaciones, Combinacion, Filtros, Utilidades }

    // Elementos (botones + separador) de cada grupo, para mostrarlos/ocultarlos en bloque.
    private readonly Dictionary<GrupoBarra, List<FrameworkElement>> _gruposBarra = new();
    // Grupo en construcción: Herramienta()/Separador() añaden a su lista.
    private List<FrameworkElement>? _grupoActual;

    private void ConstruirToolbar()
    {
        // --- tsFree ---
        Grupo(GrupoBarra.Free);
        Herramienta("bSalir", "Salir", null);                                         // null => cierra la ventana
        Herramienta("bConfig", "Configuración", typeof(ConfiguracionFrmPage));
        Herramienta("bConfAnalisis", "Configurar Análisis", typeof(ConfiguracionAnalisisFrmPage));
        Herramienta("bAyuda", "Ayuda", typeof(AyudaFrmPage));
        Herramienta("bAcercaDe", "Acerca de ...", typeof(AcercaDeFrmPage));
        Separador();

        // --- tsArchivo ---
        // Estos botones ejecutan una acción sobre la pantalla Inicio (no solo navegan a ella):
        // se enrutan a MainPage con un token AccionInicio y la página invoca el comando que
        // replica el handler equivalente del menú "Archivo" del MainForm original.
        Grupo(GrupoBarra.Archivo);
        HerramientaAccion("bGuardarEquipos", "Guardar equipos", AccionInicio.GuardarEquipos);
        HerramientaAccion("bNuevo", "Nueva combinación", AccionInicio.NuevaCombinacion);
        Herramienta("bObtenerBoletos", "Obtener Boletos Online", typeof(DescargaBoletoFrmPage));
        HerramientaAccion("bAbrirCombinacion", "Abrir combinación", AccionInicio.AbrirCombinacion);
        HerramientaAccion("bGuardarCombinacion", "Guardar combinación", AccionInicio.GuardarCombinacion);
        HerramientaAccion("bGuardarCombinacionComo", "Guardar combinación como", AccionInicio.GuardarCombinacionComo);
        HerramientaAccion("bBorrarTemporales", "Borrar archivos temporales", AccionInicio.BorrarTemporales);
        HerramientaAccion("bAbrirEquipos", "Abrir equipos", AccionInicio.AbrirEquipos);
        HerramientaAccion("bBorrarInformes", "Borrar Informes de Error", AccionInicio.BorrarInformes);
        Herramienta("bGestorEquipos", "Gestión de Equipos", typeof(GestorEquiposFrmPage));
        Separador();

        // --- tsOperaciones ---
        Grupo(GrupoBarra.Operaciones);
        Herramienta("bAlgebra", "Algebra", typeof(AlgebraColumnasFrmPage));
        Herramienta("bTransposicion", "Transposición", typeof(TransposicionFrmPage));
        Herramienta("bMultiplicacion", "Multiplicación", typeof(MultiplicadorFrmPage));
        Herramienta("bFraccionador", "Fraccionar", typeof(FraccionadorFrmPage));
        Herramienta("bRotacion", "Rotación de signos", typeof(RotacionDeSignosFrmPage));
        Separador();

        // --- tsCombinacion ---
        Grupo(GrupoBarra.Combinacion);
        Herramienta("bCalcular", "Calcular combinación", typeof(CalculaColumnasFrmPage));
        Herramienta("bCalcularM", "Calcular múltiples combinaciones", typeof(CalculaColumnasMultipleFrmPage));
        Herramienta("bVerBoletos", "Ver boletos", typeof(VerBoletosPage));
        Herramienta("bImprimirBoletos", "Imprimir boletos", typeof(ImprimirBoletoFrmPage));
        Herramienta("bReducir", "Reducir", typeof(ReductorFrmPage));
        Herramienta("bEscrutinio", "Escrutinio", typeof(EscrutiniosFrmPage));
        // bEscrutarComb: en el original está Enabled=false (handler vacío); se replica deshabilitado.
        Herramienta("bEscrutarComb", "Escrutar combinaciones", null, deshabilitado: true);
        Herramienta("bAnalisisColumnas", "Análisis de columnas", typeof(AnalizarFicheroFrmPage));
        Herramienta("bAnalisisFallos", "Análisis de fallos", typeof(ColGanadoraFrmPage));
        // bAnalisisGrafico: en el original está Visible=false → NO se muestra (omitido a propósito).
        Herramienta("bAnalisisSignos", "Análisis de signos", typeof(VSignosFrmPage));
        Herramienta("bProbabilidades", "Probabilidades", typeof(ProbabilidadPremiosPage));
        Herramienta("bEstadisticas", "Estadísticas", typeof(AnastaticsPage));
        Herramienta("bP15", "Añadir pleno al 15", typeof(AgregaP15FrmPage));
        Separador();

        // --- tsFiltros ---
        Grupo(GrupoBarra.Filtros);
        Herramienta("bCombinarFiltros", "Combinar filtros", typeof(CombinarFiltrosPage));
        Herramienta("bDiferenciasFiltros", "Diferencias de filtros", typeof(DiFiltrosPage));
        Herramienta("bFiltroCoincidencias", "Filtro de coincidencias", typeof(CoincidenciasPage));
        Herramienta("bFiltroAidomnou", "Filtro Aidomnou", typeof(aidomnouPage));
        Herramienta("bFiltroPim", "Filtro Pim", typeof(GeneraPimPage));
        Separador();

        // --- tsUtilidades ---
        Grupo(GrupoBarra.Utilidades);
        Herramienta("bSubeCategoria", "Subir categoría", typeof(SubirCategoriaFrmPage));
        Herramienta("bModificadorPct", "Modificador de porcentajes", typeof(ModificadorFrmPage));
        Herramienta("bGeneradorCPs", "Generador de CPs", typeof(GenerarCPsPage));
        Herramienta("bDiferenciasColumnas", "Diferencias entre columnas", typeof(DifColsPage));
        Herramienta("bProbabilidad", "Ordenar por probabilidad", typeof(OrdenarPorProbabilidadFrmPage));
        Herramienta("bSelectorJuanM", "Selector JuanM", typeof(SelecJMPage));
        Herramienta("bSelectorMarioSan", "Selector MarioSan", typeof(SelectorMSPage));
        Herramienta("bRentabilidad", "Rentabilidad", typeof(RentabilidadFrmPage));
        Herramienta("bColumnasGEPT", "Columnas GEPT", typeof(GEPTFrmPage));
        Herramienta("bTramificar", "Tramificar", typeof(TramificarFormPage));
        Herramienta("bPremiadas", "Premiadas", typeof(PremiadasFrmPage));
        Herramienta("bEstimacion", "Estimación de premios", typeof(EstimadorPremiosFrmPage));
        Herramienta("bBancoPruebas", "Banco de pruebas", typeof(BancoPruebasFrmPage));
        Herramienta("bImportExport", "Importar / Exportar", typeof(ImportExportFrmPage));
        Herramienta("bAnalisisGrupos", "Análisis de grupos", typeof(AnaCombiPage));
        Herramienta("bRedPerfectas", "Reducciones perfectas", typeof(FrmReducidasPerfectasPage));
        Herramienta("bDependenciaLineal", "Dependencia lineal", typeof(FrmDependenciaLinealPage));

        // Réplica de InicializarBarrasHerramientas (MainForm.cs:63-71): la visibilidad
        // inicial de cada grupo proviene del flag MostrarTs* cargado de parametros.free1x2.
        // EXCEPCIÓN — grupo Filtros: se fuerza SIEMPRE visible al arrancar, con
        // independencia del valor cargado de la config. Una copia de trabajo antigua de
        // parametros.free1x2 (con tsFiltros=False) podría seguir ocultándolo en una
        // instalación ya existente; el usuario exige que las 5 herramientas de Filtros
        // (Combinar/Diferencias/Coincidencias/Aidomnou/Pim) aparezcan por defecto. El
        // toggle Ver -> Barras de Herramientas sigue pudiendo ocultarlo después.
        foreach (var g in _gruposBarra.Keys)
        {
            bool visible = g == GrupoBarra.Filtros ? true : MostrarGrupo(g);
            AplicarVisibilidadGrupo(g, visible);
        }
    }

    // Marca el inicio de un grupo; los siguientes Herramienta()/Separador() le pertenecen.
    private void Grupo(GrupoBarra grupo)
    {
        _grupoActual = new List<FrameworkElement>();
        _gruposBarra[grupo] = _grupoActual;
    }

    // Añade un botón icono-only compacto a la barra usando el icono ORIGINAL extraído del
    // resx (Assets/Toolbar/<icono>.png), no un glifo de fuente. 'icono' es el nombre del
    // botón en el Designer (p.ej. "bCalcular"). page null => cierra la ventana (Salir) salvo
    // que 'deshabilitado' sea true (botón inactivo sin acción, como bEscrutarComb).
    private void Herramienta(string icono, string tooltip, Type? page, bool deshabilitado = false)
    {
        var img = new Image
        {
            Source = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(
                new Uri($"ms-appx:///Assets/Toolbar/{icono}.png")),
            Width = 16,
            Height = 16,
            Stretch = Stretch.Uniform,
        };
        var btn = new Button
        {
            Content = img,
            Width = 26,
            Height = 26,
            Padding = new Thickness(0),
            Background = new SolidColorBrush(Microsoft.UI.Colors.Transparent),
            BorderThickness = new Thickness(0),
            IsEnabled = !deshabilitado,
        };
        ToolTipService.SetToolTip(btn, tooltip);
        Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(btn, tooltip);
        if (!deshabilitado)
        {
            if (page is null)
                btn.Click += (_, _) => this.Close();
            else
                btn.Click += (_, _) => Navegar(page);
        }
        ToolbarPanel.Children.Add(btn);
        _grupoActual?.Add(btn);
    }

    // Igual que Herramienta(), pero el clic ejecuta una acción sobre la pantalla Inicio
    // (NavegarConAccion) en lugar de solo navegar. Usa el mismo icono ORIGINAL del resx.
    private void HerramientaAccion(string icono, string tooltip, AccionInicio accion)
    {
        var img = new Image
        {
            Source = new Microsoft.UI.Xaml.Media.Imaging.BitmapImage(
                new Uri($"ms-appx:///Assets/Toolbar/{icono}.png")),
            Width = 16,
            Height = 16,
            Stretch = Stretch.Uniform,
        };
        var btn = new Button
        {
            Content = img,
            Width = 26,
            Height = 26,
            Padding = new Thickness(0),
            Background = new SolidColorBrush(Microsoft.UI.Colors.Transparent),
            BorderThickness = new Thickness(0),
        };
        ToolTipService.SetToolTip(btn, tooltip);
        Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(btn, tooltip);
        btn.Click += (_, _) => NavegarConAccion(accion);
        ToolbarPanel.Children.Add(btn);
        _grupoActual?.Add(btn);
    }

    private void Separador()
    {
        var sep = new Border
        {
            Width = 1,
            Height = 18,
            Margin = new Thickness(4, 0, 4, 0),
            Background = (Brush)Application.Current.Resources["AppBorderBrush"],
            VerticalAlignment = VerticalAlignment.Center,
        };
        ToolbarPanel.Children.Add(sep);
        _grupoActual?.Add(sep);
    }

    // Muestra/oculta todos los elementos de un grupo, equivalente a tsX.Visible en WinForms.
    private void AplicarVisibilidadGrupo(GrupoBarra grupo, bool visible)
    {
        if (!_gruposBarra.TryGetValue(grupo, out var elementos)) return;
        var v = visible ? Visibility.Visible : Visibility.Collapsed;
        foreach (var e in elementos)
            e.Visibility = v;
    }

    // ===== Ver -> Barras de Herramientas (mostrar/ocultar cada grupo) =====
    // Réplica del submenú "Barras de Herramientas" (personalizarToolStripMenuItem,
    // MainForm.Designer.cs:1475) con un item conmutable por grupo. Las etiquetas y el
    // orden de los items son los del original (DropDownItems.AddRange, Designer.cs:1465):
    //   Filtros, Free1X2, Operaciones, Utilidades, Combinación, Archivo.
    // Cada item arranca marcado/desmarcado según MostrarTs* (= visibilidad inicial del
    // grupo) y al pulsarlo muestra/oculta el grupo en vivo, igual que ToolStripMenuItemClick
    // (MainForm.cs:489-503). La persistencia se hace al cerrar (GuardarBarrasHerramientas).

    // Estado vivo de visibilidad por grupo (lo que se persistirá al cerrar). Se siembra
    // de MostrarTs* y se actualiza en cada toggle, igual que tsX.Visible en WinForms.
    private readonly Dictionary<GrupoBarra, bool> _visibleGrupo = new();

    // Valor de partida de cada grupo desde VariablesGlobales.MostrarTs* (parametros.free1x2).
    private static bool MostrarGrupo(GrupoBarra grupo) => grupo switch
    {
        GrupoBarra.Free        => VariablesGlobales.MostrarTsFree,
        GrupoBarra.Archivo     => VariablesGlobales.MostrarTsArchivo,
        GrupoBarra.Operaciones => VariablesGlobales.MostrarTsOperaciones,
        GrupoBarra.Combinacion => VariablesGlobales.MostrarTsCombinacion,
        GrupoBarra.Filtros     => VariablesGlobales.MostrarTsFiltros,
        GrupoBarra.Utilidades  => VariablesGlobales.MostrarTsUtilidades,
        _ => true,
    };

    private MenuFlyoutSubItem ConstruirSubmenuBarrasHerramientas()
    {
        var sub = new MenuFlyoutSubItem
        {
            Text = "Barras de Herramientas",
            Icon = new FontIcon { Glyph = Glifo("E700"), FontFamily = IconFont },
        };

        // Mismo orden de items que el original (Designer.cs:1465-1471).
        (GrupoBarra grupo, string label)[] items =
        {
            (GrupoBarra.Filtros,     "Filtros"),
            (GrupoBarra.Free,        "Free1X2"),
            (GrupoBarra.Operaciones, "Operaciones"),
            (GrupoBarra.Utilidades,  "Utilidades"),
            (GrupoBarra.Combinacion, "Combinación"),
            (GrupoBarra.Archivo,     "Archivo"),
        };

        foreach (var (grupo, label) in items)
        {
            // El grupo Filtros arranca SIEMPRE marcado/visible (igual que en
            // ConstruirToolbar), aunque la config cargada traiga tsFiltros=False, para
            // mantener el check del menú sincronizado con la barra realmente visible.
            bool inicial = grupo == GrupoBarra.Filtros ? true : MostrarGrupo(grupo);
            _visibleGrupo[grupo] = inicial;

            var item = new ToggleMenuFlyoutItem { Text = label, IsChecked = inicial };
            item.Click += (s, _) =>
            {
                // ToggleMenuFlyoutItem ya ha invertido IsChecked al llegar aquí.
                bool visible = ((ToggleMenuFlyoutItem)s).IsChecked;
                _visibleGrupo[grupo] = visible;
                AplicarVisibilidadGrupo(grupo, visible);   // muestra/oculta en vivo
            };
            sub.Items.Add(item);
        }
        return sub;
    }

    /// <summary>
    /// Submenú «Ver → Tema» (U-01): las tres opciones de tema, mutuamente excluyentes, con la
    /// activa marcada. Mismo patrón que <see cref="ConstruirSubmenuBarrasHerramientas"/>
    /// (MenuFlyoutSubItem con icono + items conmutables construidos en bucle); aquí se usa
    /// <see cref="RadioMenuFlyoutItem"/> porque la selección es EXCLUSIVA (el submenú de barras
    /// son conmutadores independientes). El cambio se aplica en vivo y se persiste en
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

    // Persiste la visibilidad de las barras al cerrar, mirror exacto de
    // AConfiguracion.GuardarToolBarsVisibles (MainForm.cs:178). Orden de argumentos del
    // original: tsFree, tsFiltros, tsCombinacion, tsOperaciones, tsArchivo, tsUtilidades.
    private void GuardarBarrasHerramientas()
    {
        try
        {
            bool Vis(GrupoBarra g) => _visibleGrupo.TryGetValue(g, out var v) ? v : MostrarGrupo(g);
            new AConfiguracion().GuardarToolBarsVisibles(
                Vis(GrupoBarra.Free),
                Vis(GrupoBarra.Filtros),
                Vis(GrupoBarra.Combinacion),
                Vis(GrupoBarra.Operaciones),
                Vis(GrupoBarra.Archivo),
                Vis(GrupoBarra.Utilidades));
        }
        catch (Exception ex)
        {
            // No bloquear el cierre por un error de E/S al guardar preferencias (comportamiento
            // intacto); antes se perdía sin rastro el motivo de "no recuerda mis barras".
            Services.Log.Error("MainWindow.GuardarBarrasHerramientas", ex);
        }
    }

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
