using System;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Free1X2;
using Free1X2.MotorCalculo;
using Free1X2.WinUI.Services;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel del filtro "Signos Seguidos" (WinForms <c>SignosSeguidosFrm</c>).
/// El filtro mantiene cuatro juegos de valores que indican la cantidad admitida de
/// Variantes, signos 1, signos X y signos 2 SEGUIDOS dentro de cada combinación.
/// Cada propiedad almacena la lista de cantidades admitidas (rango 0..15) tal como
/// el control legacy <c>OptionNumTol0_14</c> (cadena de tolerancias separadas por comas).
/// Además, cada concepto puede tener una lista de "Figuras" asociada (List&lt;long&gt; en
/// el dominio legacy: FigurasV/Figuras1/FigurasX/Figuras2), gestionada vía FigurasFiltrosFrm.
/// </summary>
public partial class SignosSeguidosFrmViewModel : ObservableObject
{
    // Cantidades admitidas de "Variantes" seguidas. Equivale a filtro.GetVariantes()/SetNoVariantes().
    [ObservableProperty]
    private string _variantes = string.Empty;

    // Cantidades admitidas de signos "1" seguidos. Equivale a filtro.GetUnos()/SetNoUnos().
    [ObservableProperty]
    private string _unos = string.Empty;

    // Cantidades admitidas de signos "X" seguidos. Equivale a filtro.GetEquis()/SetNoEquis().
    [ObservableProperty]
    private string _equis = string.Empty;

    // Cantidades admitidas de signos "2" seguidos. Equivale a filtro.GetDoses()/SetNoDoses().
    [ObservableProperty]
    private string _doses = string.Empty;

    // Indicadores de si cada concepto tiene "figuras" definidas (botón verde en el form legacy,
    // IndicarCondicionFiguras()). El contenido real (List<long>) lo gestiona el dominio legacy.
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FigurasVariantesVisibility))]
    private bool _figurasVariantesActivas;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FigurasUnosVisibility))]
    private bool _figurasUnosActivas;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FigurasEquisVisibility))]
    private bool _figurasEquisActivas;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(FigurasDosesVisibility))]
    private bool _figurasDosesActivas;

    // Visibilidad del indicador "Activas" por concepto. Se exponen como Visibility para no
    // bindear un bool directo a la UI (regla del proyecto).
    public Visibility FigurasVariantesVisibility =>
        FigurasVariantesActivas ? Visibility.Visible : Visibility.Collapsed;

    public Visibility FigurasUnosVisibility =>
        FigurasUnosActivas ? Visibility.Visible : Visibility.Collapsed;

    public Visibility FigurasEquisVisibility =>
        FigurasEquisActivas ? Visibility.Visible : Visibility.Collapsed;

    public Visibility FigurasDosesVisibility =>
        FigurasDosesActivas ? Visibility.Visible : Visibility.Collapsed;

    // El form legacy rellena los campos vacíos con "0,1,...,14" (SignosSeguidosFrm.ActualizarDatos línea 345).
    private const string TodosValores = "0,1,2,3,4,5,6,7,8,9,10,11,12,13,14";

    // Listas de figuras cargadas del filtro; se preservan al Aceptar (edición de figuras pendiente).
    private List<long>? _figurasV;
    private List<long>? _figuras1;
    private List<long>? _figurasX;
    private List<long>? _figuras2;

    /// <summary>Acción para volver atrás (la cablea la página con Frame.GoBack()). CerrarVentana() legacy.</summary>
    public Action? Volver { get; set; }

    // true si hay algún valor introducido (NecesitaGuardarDatos() del form legacy).
    public bool ContieneDatos =>
        !string.IsNullOrWhiteSpace(Variantes) ||
        !string.IsNullOrWhiteSpace(Unos) ||
        !string.IsNullOrWhiteSpace(Equis) ||
        !string.IsNullOrWhiteSpace(Doses);

    /// <summary>
    /// Vuelca los valores del FiltroSignosSeguidos del grupo en edición a la pantalla.
    /// Equivale a SignosSeguidosFrm.MarcarValores() (Free1X2/UI/Filtros/SignosSeguidosFrm.cs líneas 75-87).
    /// </summary>
    public void CargarDesdeGrupo()
    {
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) return;

        var filtro = (FiltroSignosSeguidos)grupo.GetFiltro(Filtro.SignosSeguidos.ToString());
        Variantes = filtro.GetVariantes();
        Unos = filtro.GetUnos();
        Equis = filtro.GetEquis();
        Doses = filtro.GetDoses();

        _figurasV = filtro.FigurasV;
        _figuras1 = filtro.Figuras1;
        _figurasX = filtro.FigurasX;
        _figuras2 = filtro.Figuras2;
        FigurasVariantesActivas = _figurasV != null && _figurasV.Count > 0;
        FigurasUnosActivas = _figuras1 != null && _figuras1.Count > 0;
        FigurasEquisActivas = _figurasX != null && _figurasX.Count > 0;
        FigurasDosesActivas = _figuras2 != null && _figuras2.Count > 0;
    }

    [RelayCommand]
    private void Aceptar()
    {
        // Equivale a SignosSeguidosFrm.menuCondiciones1_BOk -> ActualizarDatos() + ActivaFiltro
        //   (Free1X2/UI/Filtros/SignosSeguidosFrm.cs líneas 343-534).
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) { Volver?.Invoke(); return; }

        var filtro = (FiltroSignosSeguidos)grupo.GetFiltro(Filtro.SignosSeguidos.ToString());
        filtro.ReinicializaValores();

        if (ContieneDatos)
        {
            if (filtro.ContieneDatos == false)
            {
                filtro.IsActive = true;
            }
            filtro.ContieneDatos = true;

            filtro.SetNoVariantes(!string.IsNullOrWhiteSpace(Variantes) ? Variantes : TodosValores);
            filtro.SetNoUnos(!string.IsNullOrWhiteSpace(Unos) ? Unos : TodosValores);
            filtro.SetNoEquis(!string.IsNullOrWhiteSpace(Equis) ? Equis : TodosValores);
            filtro.SetNoDoses(!string.IsNullOrWhiteSpace(Doses) ? Doses : TodosValores);

            if (_figurasV != null && _figurasV.Count > 0) filtro.FigurasV = _figurasV;
            if (_figuras1 != null && _figuras1.Count > 0) filtro.Figuras1 = _figuras1;
            if (_figurasX != null && _figurasX.Count > 0) filtro.FigurasX = _figurasX;
            if (_figuras2 != null && _figuras2.Count > 0) filtro.Figuras2 = _figuras2;
        }
        else
        {
            filtro.IsActive = false;
            filtro.ContieneDatos = false;
        }

        grupo.ActivaFiltro(filtro);
        AppState.Instancia.NotificarCambio();
        Volver?.Invoke();
    }

    [RelayCommand]
    private void Cancelar()
    {
        // Equivale a menuCondiciones1_BCancelar -> CerrarVentana() (sin aplicar cambios).
        Volver?.Invoke();
    }

    [RelayCommand]
    private void Borrar()
    {
        // Equivale a menuCondiciones1_BBorrar -> reinstanciar FiltroSignosSeguidos + MarcarValores().
        Variantes = string.Empty;
        Unos = string.Empty;
        Equis = string.Empty;
        Doses = string.Empty;
        _figurasV = _figuras1 = _figurasX = _figuras2 = null;
        FigurasVariantesActivas = false;
        FigurasUnosActivas = false;
        FigurasEquisActivas = false;
        FigurasDosesActivas = false;
    }

    [RelayCommand]
    private void Guardar()
    {
        // TODO: Dominio legacy — ArchivoCondiciones.GuardaArchivo(FiltroSignosSeguidos)
        //   sobre un archivo *.seg (equivale a menuCondiciones1_BGuardar -> guardar() del SignosSeguidosFrm legacy).
    }

    [RelayCommand]
    private void Abrir()
    {
        // TODO: Dominio legacy — ArchivoCondiciones.AbrirArchivoCombinacion(...) + LeeCondicion()
        //   y volcar los valores del FiltroSignosSeguidos leído ("SignosSeguidos") a estas propiedades
        //   (equivale a menuCondiciones1_BAbrir -> abrir() del SignosSeguidosFrm legacy).
    }

    [RelayCommand]
    private void Copiar()
    {
        // TODO: Dominio legacy — ActualizarDatos() y guardar a "/Temp/tmp.seg"; habilitar Pegar
        //   (equivale a menuCondiciones1_BCopiar del SignosSeguidosFrm legacy).
    }

    [RelayCommand]
    private void Pegar()
    {
        // TODO: Dominio legacy — abrir "/Temp/tmp.seg" y volcar sus valores a estas propiedades
        //   (equivale a menuCondiciones1_BPegar del SignosSeguidosFrm legacy).
    }

    [RelayCommand]
    private void Estadisticas()
    {
        // TODO: Dominio legacy — construir FiltroSignosSeguidos temporal (ObtenerFiltroTemporal())
        //   y llamar CalculadorEstadisticas.EstadisticasFiltro(filtroTemp, ".../Ganadoras/")
        //   mostrando el VisorEstadisticas (equivale a menuCondiciones1_BEstadisticas).
    }

    [RelayCommand]
    private void FigurasVariantes()
    {
        // TODO: Dominio legacy — abrir FigurasFiltrosFrm(figurasV, 10, new FiltroSignosSeguidos())
        //   y tras cerrar actualizar FigurasVariantesActivas = (figurasV.Count > 0)
        //   (equivale a btnFigurasV_Click del SignosSeguidosFrm legacy).
    }

    [RelayCommand]
    private void FigurasUnos()
    {
        // TODO: Dominio legacy — abrir FigurasFiltrosFrm(figuras1, 10, new FiltroSignosSeguidos())
        //   y tras cerrar actualizar FigurasUnosActivas = (figuras1.Count > 0)
        //   (equivale a btnFiguras1_Click del SignosSeguidosFrm legacy).
    }

    [RelayCommand]
    private void FigurasEquis()
    {
        // TODO: Dominio legacy — abrir FigurasFiltrosFrm(figurasX, 10, new FiltroSignosSeguidos())
        //   y tras cerrar actualizar FigurasEquisActivas = (figurasX.Count > 0)
        //   (equivale a btnFigurasX_Click del SignosSeguidosFrm legacy).
    }

    [RelayCommand]
    private void FigurasDoses()
    {
        // TODO: Dominio legacy — abrir FigurasFiltrosFrm(figuras2, 10, new FiltroSignosSeguidos())
        //   y tras cerrar actualizar FigurasDosesActivas = (figuras2.Count > 0)
        //   (equivale a btnFiguras2_Click del SignosSeguidosFrm legacy).
    }
}
