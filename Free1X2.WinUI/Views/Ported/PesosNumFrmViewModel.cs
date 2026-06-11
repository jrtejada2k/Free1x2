using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Una opción seleccionable de tolerancia numérica (0..5) o de figura.
/// Equivale a las Label-casilla "optN" / "lblFigXX" del WinForms <c>PesosNumFrm</c>,
/// que alternan entre Wheat (inactiva) y LightGreen (activa) al hacer clic.
/// </summary>
public partial class OpcionSeleccionableViewModel : ObservableObject
{
    public OpcionSeleccionableViewModel(string etiqueta, bool seleccionada = false)
    {
        Etiqueta = etiqueta;
        _seleccionada = seleccionada;
    }

    /// <summary>Texto visible de la casilla (p. ej. "3" o "2-1-1-1").</summary>
    public string Etiqueta { get; }

    /// <summary>True si la casilla está activa (legacy: BackColor == LightGreen).</summary>
    [ObservableProperty]
    private bool _seleccionada;
}

/// <summary>
/// ViewModel de la pantalla portada del WinForms "PesosNumFrm".
/// Mantiene los conjuntos de Pesos Numéricos (Global / Variantes / 1 / X / 2),
/// sus tolerancias homónimas, las tolerancias numéricas (0..5) y las figuras
/// seleccionables. La conversión texto→valores, el cálculo y la persistencia se
/// delegan al dominio legacy (ver TODOs).
/// </summary>
public partial class PesosNumFrmViewModel : ObservableObject
{
    public PesosNumFrmViewModel()
    {
        // Tolerancias numéricas admitidas (legacy: opt0..opt5).
        ToleranciasNumericas = new ObservableCollection<OpcionSeleccionableViewModel>
        {
            new("0"), new("1"), new("2"), new("3"), new("4"), new("5"),
        };

        // Figuras seleccionables (legacy: lblFig32 / lblFig311 / lblFig221 / lblFig2111 / lblFig11111).
        Figuras = new ObservableCollection<OpcionSeleccionableViewModel>
        {
            new("3-2"), new("3-1-1"), new("2-2-1"), new("2-1-1-1"), new("1-1-1-1-1"),
        };

        // TODO(dominio): cargar valores desde FiltroPesosNumericos del Grupo.
        //   Legacy PesosNumFrm.MarcarValores():
        //     stdGlobal.Valores   = filtro.GetPNGlobal();   stdGlobalTol.Valores   = filtro.GetPNGlobalTol();
        //     stdVariantes.Valores= filtro.GetPNVariantes();stdVariantesTol.Valores= filtro.GetPNVariantesTol();
        //     stdUnos.Valores     = filtro.GetPNUnos();     stdUnosTol.Valores     = filtro.GetPNUnosTol();
        //     stdEquis.Valores    = filtro.GetPNEquis();    stdEquisTol.Valores    = filtro.GetPNEquisTol();
        //     stdDoses.Valores    = filtro.GetPNDoses();    stdDosesTol.Valores    = filtro.GetPNDosesTol();
        //     ToleranciaValores   = filtro.GetTolerancias();  figuras = filtro.Figuras;
        //     MarcarFigurasSeleccionadas();
    }

    // ===== Pesos Numéricos principales (legacy: stdGlobal/stdVariantes/stdUnos/stdEquis/stdDoses) =====
    // Cada cadena lista los dígitos 0..9 seleccionados en el control OptionNumsHoriz0_9 legacy.

    [ObservableProperty]
    private string _pesoGlobal = string.Empty;

    [ObservableProperty]
    private string _pesoVariantes = string.Empty;

    [ObservableProperty]
    private string _pesoUnos = string.Empty;

    [ObservableProperty]
    private string _pesoEquis = string.Empty;

    [ObservableProperty]
    private string _pesoDoses = string.Empty;

    // ===== Tolerancias homónimas (legacy: stdGlobalTol/stdVariantesTol/...) =====

    [ObservableProperty]
    private string _pesoGlobalTol = string.Empty;

    [ObservableProperty]
    private string _pesoVariantesTol = string.Empty;

    [ObservableProperty]
    private string _pesoUnosTol = string.Empty;

    [ObservableProperty]
    private string _pesoEquisTol = string.Empty;

    [ObservableProperty]
    private string _pesoDosesTol = string.Empty;

    /// <summary>Tolerancias numéricas 0..5 (legacy: opt0..opt5).</summary>
    public ObservableCollection<OpcionSeleccionableViewModel> ToleranciasNumericas { get; }

    /// <summary>Figuras seleccionables (legacy: lblFig*).</summary>
    public ObservableCollection<OpcionSeleccionableViewModel> Figuras { get; }

    /// <summary>Nota informativa (legacy: label33).</summary>
    public string NotaFiguras =>
        "No se incluyen las figuras 5 y 4-1, que darían 0 columnas.";

    /// <summary>Ayuda del form (legacy: ctrlAyuda1.TextoAyuda).</summary>
    public string TextoAyuda =>
        "El Peso Numérico de una columna es una representación numérica de la columna. " +
        "También se puede expresar el Peso Numérico de Variantes, 1, X y 2.";

    [RelayCommand]
    private void Aceptar()
    {
        // TODO(dominio): volcar los valores a FiltroPesosNumericos, activar y cerrar.
        //   Legacy PesosNumFrm.menuCondiciones1_BOk(): ActualizarDatos(); grupo.ActivaFiltro(filtro); Close();
        //   ActualizarDatos() usa SetPNGlobal/SetPNVar/SetPNUnos/SetPNEquis/SetPNDoses (+ ...Tol),
        //   PonerTolerancia(ToleranciaValores) y filtro.Figuras = figuras.
    }

    [RelayCommand]
    private void Estadisticas()
    {
        // TODO(dominio): abrir el VisorEstadisticas con un filtro temporal.
        //   Legacy PesosNumFrm.menuCondiciones1_BEstadisticas():
        //     filtroTemp = ObtenerFiltroTemporal();
        //     lista = new CalculadorEstadisticas().EstadisticasFiltro(filtroTemp, ".../Ganadoras/");
        //     new VisorEstadisticas(lista).ShowDialog();
    }

    [RelayCommand]
    private void Guardar()
    {
        // TODO(dominio): ActualizarDatos() + SaveFileDialog (*.pes/*.xml) + ArchivoCondiciones.GuardaArchivo(filtro).
        //   Legacy PesosNumFrm.menuCondiciones1_BGuardar() / guardar().
    }

    [RelayCommand]
    private void Abrir()
    {
        // TODO(dominio): OpenFileDialog (*.pes/*.xml) + ArchivoCondiciones.LeeCondicion() + MarcarValores().
        //   Legacy PesosNumFrm.menuCondiciones1_BAbrir() / abrir().
    }

    [RelayCommand]
    private void Copiar()
    {
        // TODO(dominio): ActualizarDatos() + guardar a Temp/tmp.pes; habilita Pegar.
        //   Legacy PesosNumFrm.menuCondiciones1_BCopiar().
    }

    [RelayCommand]
    private void Pegar()
    {
        // TODO(dominio): abrir Temp/tmp.pes y recargar la pantalla.
        //   Legacy PesosNumFrm.menuCondiciones1_BPegar().
    }

    [RelayCommand]
    private void Borrar()
    {
        // Legacy PesosNumFrm.menuCondiciones1_BBorrar(): reinicia el filtro y limpia la pantalla.
        PesoGlobal = string.Empty;
        PesoVariantes = string.Empty;
        PesoUnos = string.Empty;
        PesoEquis = string.Empty;
        PesoDoses = string.Empty;
        PesoGlobalTol = string.Empty;
        PesoVariantesTol = string.Empty;
        PesoUnosTol = string.Empty;
        PesoEquisTol = string.Empty;
        PesoDosesTol = string.Empty;
        foreach (var t in ToleranciasNumericas) t.Seleccionada = false;
        foreach (var f in Figuras) f.Seleccionada = false;
        // TODO(dominio): filtro = new FiltroPesosNumericos();
    }

    [RelayCommand]
    private void Cancelar()
    {
        // TODO(dominio): cerrar sin aplicar cambios.
        //   Legacy PesosNumFrm.menuCondiciones1_BCancelar(): this.Close();
    }
}
