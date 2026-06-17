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
    public MainWindow()
    {
        this.InitializeComponent();

        // Mica + barra de título integrada (look Win11 nativo)
        this.SystemBackdrop = new MicaBackdrop();
        this.ExtendsContentIntoTitleBar = true;
        this.SetTitleBar(AppTitleBar);
        this.Title = "Free1X2";

        ConstruirMenus();
        ContentFrame.Navigate(typeof(MainPage));

        if (Environment.GetEnvironmentVariable("FREE1X2_SMOKE") == "1")
            IniciarSmokeTest();
    }

    // ===== Barra de menús (misma organización que el programa WinForms original) =====
    // Cada entrada navega a la pantalla real portada. Las condiciones (Variantes, Dibujos,
    // etc.) no van aquí: se abren desde la rejilla de condiciones de la pantalla Inicio,
    // igual que en el original (campo Condiciones del MainForm).
    private void ConstruirMenus()
    {
        BarraMenu.Items.Add(Menu("Free1x2",
            ("Inicio", typeof(MainPage)),
            null,
            ("Configuración…", typeof(ConfiguracionFrmPage)),
            ("Configurar análisis…", typeof(ConfiguracionAnalisisFrmPage)),
            null,
            ("Acerca de…", typeof(AcercaDeFrmPage)),
            ("Créditos…", typeof(CreditosFrmPage)),
            null,
            ("Salir", typeof(SalirFrmPage))));

        BarraMenu.Items.Add(Menu("Archivo",
            ("Boleto / combinación (Inicio)", typeof(MainPage)),
            null,
            ("Gestión de equipos…", typeof(GestorEquiposFrmPage)),
            ("Importar / exportar columnas…", typeof(ImportExportFrmPage))));

        BarraMenu.Items.Add(Menu("Combinación",
            ("Calcular…", typeof(CalculaColumnasFrmPage)),
            ("Calcular varias…", typeof(CalculaColumnasMultipleFrmPage)),
            null,
            ("Ver boletos…", typeof(VerBoletosPage)),
            ("Imprimir boletos…", typeof(ImprimirBoletoFrmPage)),
            null,
            ("Reducir…", typeof(ReductorFrmPage)),
            ("Escrutinios…", typeof(EscrutiniosFrmPage)),
            null,
            ("Analizar combinación…", typeof(AnalizarCombinacionFrmPage)),
            ("Gráfico de columnas…", typeof(GraficoColumnasFrmPage)),
            ("Probabilidades…", typeof(ProbabilidadPremiosPage)),
            ("Estadísticas…", typeof(AnastaticsPage)),
            null,
            ("Añadir Pleno al 15…", typeof(AgregaP15FrmPage))));

        BarraMenu.Items.Add(Menu("Filtros",
            ("Combinar filtros…", typeof(CombinarFiltrosPage)),
            ("Diferencias entre filtros…", typeof(DiFiltrosPage)),
            null,
            ("Filtro Coincidencias…", typeof(CoincidenciasPage)),
            ("Filtro Aidomnou…", typeof(aidomnouPage)),
            ("Filtro Pim…", typeof(GeneraPimPage))));

        BarraMenu.Items.Add(Menu("Operaciones",
            ("Álgebra de columnas…", typeof(AlgebraColumnasFrmPage)),
            ("Transposición…", typeof(TransposicionFrmPage)),
            ("Multiplicador…", typeof(MultiplicadorFrmPage)),
            ("Fraccionador…", typeof(FraccionadorFrmPage)),
            ("Rotación de signos…", typeof(RotacionDeSignosFrmPage))));

        BarraMenu.Items.Add(Menu("Utilidades",
            ("Sube categoría…", typeof(SubirCategoriaFrmPage)),
            ("Modificador %…", typeof(ModificadorFrmPage)),
            null,
            ("Generador CP…", typeof(GenerarCPsPage)),
            ("Columnas GEPT…", typeof(GEPTFrmPage)),
            null,
            ("Diferencias entre columnas…", typeof(DifColsPage)),
            ("Ordenar por probabilidad…", typeof(OrdenarPorProbabilidadFrmPage)),
            null,
            ("Selector JuanM…", typeof(SelecJMPage)),
            ("Selector MarioSan…", typeof(SelectorMSPage)),
            null,
            ("Rentabilidad…", typeof(RentabilidadFrmPage)),
            ("Tramificar…", typeof(TramificarFormPage)),
            ("Premiadas…", typeof(PremiadasFrmPage)),
            ("Estimación de premios…", typeof(EstimadorPremiosFrmPage)),
            ("Banco de pruebas…", typeof(BancoPruebasFrmPage))));
    }

    // Crea un menú superior; un elemento null inserta un separador.
    private MenuBarItem Menu(string titulo, params (string label, Type page)?[] items)
    {
        var menu = new MenuBarItem { Title = titulo };
        foreach (var item in items)
        {
            if (item is null)
            {
                menu.Items.Add(new MenuFlyoutSeparator());
                continue;
            }
            var (label, page) = item.Value;
            var mfi = new MenuFlyoutItem { Text = label };
            var destino = page;
            mfi.Click += (_, _) => Navegar(destino);
            menu.Items.Add(mfi);
        }
        return menu;
    }

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
