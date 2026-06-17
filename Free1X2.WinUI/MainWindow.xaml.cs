using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Free1X2.WinUI.Views;
using Free1X2.WinUI.Views.Ported;
using Free1X2.WinUI.Navigation;

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
        AjustarTamanoVentana();

        ConstruirMenus();
        ConstruirToolbar();
        ContentFrame.Navigate(typeof(MainPage));

        if (Environment.GetEnvironmentVariable("FREE1X2_SMOKE") == "1")
            IniciarSmokeTest();
    }

    // Tamaño inicial cercano al MainForm original (~1020 x 720).
    private void AjustarTamanoVentana()
    {
        try
        {
            this.AppWindow?.Resize(new Windows.Graphics.SizeInt32(1020, 720));
        }
        catch { /* sin AppWindow (entornos sin presentación): no es crítico */ }
    }

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
            ("E9D2", "Configurar análisis…", typeof(ConfiguracionAnalisisFrmPage)),
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
        BarraMenu.Items.Add(Menu("Ver",
            ("E80F", "Inicio", typeof(MainPage)),
            ("E8A1", "Ver boletos…", typeof(VerBoletosPage)),
            ("E9D9", "Gráfico de columnas…", typeof(GraficoColumnasFrmPage)),
            ("E9D2", "Estadísticas…", typeof(AnastaticsPage)),
            null,
            ("E713", "Configuración…", typeof(ConfiguracionFrmPage))));

        BarraMenu.Items.Add(Menu("Combinación",
            ("E945", "Calcular…", typeof(CalculaColumnasFrmPage)),
            ("E8EF", "Calcular varias…", typeof(CalculaColumnasMultipleFrmPage)),
            null,
            ("E8A1", "Ver boletos…", typeof(VerBoletosPage)),
            ("E749", "Imprimir boletos…", typeof(ImprimirBoletoFrmPage)),
            null,
            ("E74D", "Reducir…", typeof(ReductorFrmPage)),
            ("E73E", "Escrutinios…", typeof(EscrutiniosFrmPage)),
            null,
            ("E9F5", "Analizar combinación…", typeof(AnalizarCombinacionFrmPage)),
            ("E9D9", "Gráfico de columnas…", typeof(GraficoColumnasFrmPage)),
            ("E9D2", "Probabilidades…", typeof(ProbabilidadPremiosPage)),
            ("E9D2", "Estadísticas…", typeof(AnastaticsPage)),
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
            ("E948", "Álgebra de columnas…", typeof(AlgebraColumnasFrmPage)),
            ("E8AB", "Transposición…", typeof(TransposicionFrmPage)),
            ("E947", "Multiplicador…", typeof(MultiplicadorFrmPage)),
            ("E94D", "Fraccionador…", typeof(FraccionadorFrmPage)),
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
            ("E1D3", "Rentabilidad…", typeof(RentabilidadFrmPage)),
            ("E9D9", "Tramificar…", typeof(TramificarFormPage)),
            ("E735", "Premiadas…", typeof(PremiadasFrmPage)),
            ("E1D3", "Estimación de premios…", typeof(EstimadorPremiosFrmPage)),
            ("EC7A", "Banco de pruebas…", typeof(BancoPruebasFrmPage))));
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

    // ===== Barra de herramientas: réplica de los 6 ToolStrips del MainForm =====
    // Orden y tooltips idénticos a MainForm.Designer.cs (Items.AddRange de cada ToolStrip).
    // Grupos separados por un separador vertical, igual que los 6 ToolStrip independientes.
    // Acciones que en WinForms operan sobre 'pronosticos' (ficheros del boleto) se enrutan
    // a la pantalla Inicio (MainPage), donde vive el boleto, igual que el menú existente.
    private void ConstruirToolbar()
    {
        // --- ARCHIVO (tsArchivo) ---
        Herramienta("E74E", "Guardar equipos", typeof(MainPage));                       // Save
        Herramienta("E8A5", "Nueva combinación", typeof(MainPage));                     // Document
        Herramienta("E896", "Obtener Boletos Online", typeof(DescargaBoletoFrmPage));   // Download
        Herramienta("E8E5", "Abrir combinación", typeof(MainPage));                     // OpenFile
        Herramienta("E74E", "Guardar combinación", typeof(MainPage));                   // Save
        Herramienta("E792", "Guardar combinación como", typeof(MainPage));              // SaveAs
        Herramienta("E74D", "Borrar archivos temporales", typeof(MainPage));            // Delete (op. ficheros)
        Herramienta("E716", "Abrir equipos", typeof(MainPage));                         // People
        Herramienta("E74D", "Borrar Informes de Error", typeof(MainPage));              // Delete (op. ficheros)
        Herramienta("E716", "Gestión de Equipos", typeof(GestorEquiposFrmPage));        // People
        Separador();

        // --- COMBINACIÓN (tsCombinacion) ---
        Herramienta("E945", "Calcular combinación", typeof(CalculaColumnasFrmPage));            // Lightbulb
        Herramienta("E8EF", "Calcular múltiples combinaciones", typeof(CalculaColumnasMultipleFrmPage));// CopyTo
        Herramienta("E8A1", "Ver boletos", typeof(VerBoletosPage));                             // List
        Herramienta("E749", "Imprimir boletos", typeof(ImprimirBoletoFrmPage));                 // Print
        Herramienta("E74D", "Reducir", typeof(ReductorFrmPage));                                // Delete/Reduce
        Herramienta("E73E", "Escrutinio", typeof(EscrutiniosFrmPage));                          // CheckboxComposite
        Herramienta("E73A", "Escrutar combinaciones", typeof(EscrutarCombinacionesFrmPage));    // CheckList
        Herramienta("E9F5", "Análisis de columnas", typeof(AnalizarFicheroFrmPage));            // Processing
        Herramienta("E9F5", "Análisis de fallos", typeof(ColGanadoraFrmPage));                  // Processing
        Herramienta("E9D9", "Análisis gráfico", typeof(GraficoColumnasFrmPage));                // BarChart
        Herramienta("E8C1", "Análisis de signos", typeof(VSignosFrmPage));                      // ViewAll
        Herramienta("E9D2", "Probabilidades", typeof(ProbabilidadPremiosPage));                 // DataSense
        Herramienta("E9D2", "Estadísticas", typeof(AnastaticsPage));                            // DataSense
        Herramienta("E710", "Añadir pleno al 15", typeof(AgregaP15FrmPage));                    // Add
        Separador();

        // --- FILTROS (tsFiltros) ---
        Herramienta("E71C", "Combinar filtros", typeof(CombinarFiltrosPage));    // Filter
        Herramienta("E8AB", "Diferencias de filtros", typeof(DiFiltrosPage));    // Switch
        Herramienta("E71C", "Filtro de coincidencias", typeof(CoincidenciasPage));// Filter
        Herramienta("E71C", "Filtro Aidomnou", typeof(aidomnouPage));            // Filter
        Herramienta("E71C", "Filtro Pim", typeof(GeneraPimPage));                // Filter
        Separador();

        // --- OPERACIONES (tsOperaciones) ---
        Herramienta("E948", "Algebra", typeof(AlgebraColumnasFrmPage));          // Calculator
        Herramienta("E8AB", "Transposición", typeof(TransposicionFrmPage));      // Switch
        Herramienta("E947", "Multiplicación", typeof(MultiplicadorFrmPage));     // CalculatorMultiply
        Herramienta("E94D", "Fraccionar", typeof(FraccionadorFrmPage));          // CalculatorDivide
        Herramienta("E7AD", "Rotación de signos", typeof(RotacionDeSignosFrmPage));// Refresh
        Separador();

        // --- UTILIDADES (tsUtilidades) ---
        Herramienta("E74A", "Subir categoría", typeof(SubirCategoriaFrmPage));               // Up
        Herramienta("E9E9", "Modificador de porcentajes", typeof(ModificadorFrmPage));       // PieSingle
        Herramienta("E710", "Generador de CPs", typeof(GenerarCPsPage));                     // Add
        Herramienta("E8AB", "Diferencias entre columnas", typeof(DifColsPage));              // Switch
        Herramienta("E8CB", "Ordenar por probabilidad", typeof(OrdenarPorProbabilidadFrmPage));// Sort
        Herramienta("E762", "Selector JuanM", typeof(SelecJMPage));                          // Filter/People
        Herramienta("E762", "Selector MarioSan", typeof(SelectorMSPage));                    // Filter/People
        Herramienta("E1D3", "Rentabilidad", typeof(RentabilidadFrmPage));                    // Money
        Herramienta("E8A1", "Columnas GEPT", typeof(GEPTFrmPage));                           // List
        Herramienta("E9D9", "Tramificar", typeof(TramificarFormPage));                       // BarChart
        Herramienta("E735", "Premiadas", typeof(PremiadasFrmPage));                          // FavoriteStar
        Herramienta("E1D3", "Estimación de premios", typeof(EstimadorPremiosFrmPage));       // Money
        Herramienta("EC7A", "Banco de pruebas", typeof(BancoPruebasFrmPage));                // DeveloperTools
        Herramienta("E8B5", "Importar/Exportar", typeof(ImportExportFrmPage));               // Switch/Import
        Herramienta("E902", "Análisis de grupos", typeof(AnaCombiPage));                     // Group
        Herramienta("E74D", "Reducciones perfectas", typeof(FrmReducidasPerfectasPage));     // Delete/Reduce
        Herramienta("E9E9", "Dependencia lineal", typeof(FrmDependenciaLinealPage));         // chart
        Separador();

        // --- FREE / AYUDA (tsFree) ---
        Herramienta("E7E8", "Salir", null);                                          // PowerButton; null => cierra
        Herramienta("E713", "Configuración", typeof(ConfiguracionFrmPage));          // Settings
        Herramienta("E9D2", "Configurar Análisis", typeof(ConfiguracionAnalisisFrmPage));// DataSense
        Herramienta("E897", "Ayuda", typeof(AyudaFrmPage));                          // Help
        Herramienta("E946", "Acerca de", typeof(AcercaDeFrmPage));                   // Info
    }

    // Añade un botón icono-only compacto a la barra. page null => cierra la ventana (Salir).
    private void Herramienta(string glifoHex, string tooltip, Type? page)
    {
        var btn = new Button
        {
            Content = new FontIcon { Glyph = Glifo(glifoHex), FontSize = 15, FontFamily = IconFont },
            Width = 30,
            Height = 26,
            Padding = new Thickness(0),
            Background = new SolidColorBrush(Microsoft.UI.Colors.Transparent),
            BorderThickness = new Thickness(0),
        };
        ToolTipService.SetToolTip(btn, tooltip);
        Microsoft.UI.Xaml.Automation.AutomationProperties.SetName(btn, tooltip);
        if (page is null)
            btn.Click += (_, _) => this.Close();
        else
            btn.Click += (_, _) => Navegar(page);
        ToolbarPanel.Children.Add(btn);
    }

    private void Separador() => ToolbarPanel.Children.Add(new Border
    {
        Width = 1,
        Height = 18,
        Margin = new Thickness(4, 0, 4, 0),
        Background = (Brush)Application.Current.Resources["AppBorderBrush"],
        VerticalAlignment = VerticalAlignment.Center,
    });

    private void Navegar(Type page)
    {
        if (ContentFrame.CurrentSourcePageType != page)
            ContentFrame.Navigate(page);
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
        try { File.WriteAllText(_smokeLog, "SMOKE START\r\n"); } catch { }

        _smokeRuta = new List<Type> { typeof(MainPage), typeof(HomePage) };
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
        try { File.AppendAllText(_smokeLog, linea + "\r\n"); } catch { }
    }
}
