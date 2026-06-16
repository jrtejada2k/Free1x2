using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2;
using Free1X2.MotorCalculo;
using Free1X2.WinUI.Services;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Celda de un "dibujo" X+2 (figura formada por el número de equis y el número de doses).
/// Por ejemplo, 2 equis y 3 doses forman el dibujo "2+3".
/// </summary>
public partial class DibujoCeldaViewModel : ObservableObject
{
    private readonly DibujosFrmViewModel _owner;

    public DibujoCeldaViewModel(DibujosFrmViewModel owner, int numX, int num2)
    {
        _owner = owner;
        NumX = numX;
        Num2 = num2;
    }

    /// <summary>Número de equis del dibujo.</summary>
    public int NumX { get; }

    /// <summary>Número de doses del dibujo.</summary>
    public int Num2 { get; }

    /// <summary>Etiqueta del dibujo, p. ej. "2+3" (equivale a OptionNum.Valor en WinForms).</summary>
    public string Etiqueta => $"{NumX}+{Num2}";

    [ObservableProperty]
    private bool _seleccionado;

    partial void OnSeleccionadoChanged(bool value) => _owner.RecalcularResumen();
}

/// <summary>
/// ViewModel del filtro de Dibujos X+2.
/// Genera la malla de dibujos posibles (NumX + Num2 &lt;= número de partidos) y permite
/// seleccionarlos para construir el filtro <c>FiltroDibujos</c> del dominio.
/// Datos de malla generados en memoria; la persistencia/cálculo aún no depende del dominio.
/// </summary>
public partial class DibujosFrmViewModel : ObservableObject
{
    /// <summary>
    /// Número de partidos de la quiniela. En WinForms se lee de
    /// <c>VariablesGlobales.NumeroPartidos</c> (con respaldo 15).
    /// </summary>
    public const int NumPartidos = 15;

    public ObservableCollection<DibujoCeldaViewModel> Dibujos { get; } = new();

    [ObservableProperty]
    private int _seleccionados;

    /// <summary>Acción para volver atrás (la cablea la página con Frame.GoBack()). CerrarVentana() legacy.</summary>
    public Action? Volver { get; set; }

    public DibujosFrmViewModel()
    {
        GenerarCasillas();
        RecalcularResumen();
    }

    /// <summary>
    /// Vuelca los dibujos seleccionados del FiltroDibujos del grupo en edición a la malla.
    /// Equivale a DibujosFrm.MarcarValores() -> gridDibujosCentral.Dibujos = filtro.Dibujos.
    /// </summary>
    public void CargarDesdeGrupo()
    {
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) return;

        var filtro = (FiltroDibujos)grupo.GetFiltro(Filtro.Dibujos.ToString());

        // filtro.Dibujos es un ArrayList de etiquetas "X+2".
        var seleccionados = new HashSet<string>();
        if (filtro.Dibujos != null)
        {
            foreach (var item in filtro.Dibujos)
            {
                if (item is string s) seleccionados.Add(s.Trim());
            }
        }

        foreach (var celda in Dibujos)
        {
            celda.Seleccionado = seleccionados.Contains(celda.Etiqueta);
        }
        RecalcularResumen();
    }

    /// <summary>Texto resumen, p. ej. "Dibujos seleccionados: 0".</summary>
    public string Resumen => $"Dibujos seleccionados: {Seleccionados}";

    /// <summary>Indica si hay al menos un dibujo marcado (el filtro estaría activo).</summary>
    public bool TieneSeleccion => Seleccionados > 0;

    /// <summary>
    /// Genera todas las casillas X+2 posibles donde NumX + Num2 &lt;= NumPartidos.
    /// Equivale a <c>GridDibujosCentral.GenerarCasillas</c> en WinForms.
    /// </summary>
    private void GenerarCasillas()
    {
        for (int numX = 0; numX <= NumPartidos; numX++)
        {
            for (int num2 = 0; num2 <= NumPartidos; num2++)
            {
                if (numX + num2 > NumPartidos)
                    break;

                Dibujos.Add(new DibujoCeldaViewModel(this, numX, num2));
            }
        }
    }

    /// <summary>Recalcula el contador de dibujos seleccionados.</summary>
    public void RecalcularResumen()
    {
        Seleccionados = Dibujos.Count(d => d.Seleccionado);
        OnPropertyChanged(nameof(Resumen));
        OnPropertyChanged(nameof(TieneSeleccion));
    }

    /// <summary>Selecciona todos los dibujos (equivale a btnMarcarTodos / SeleccionaTodos).</summary>
    [RelayCommand]
    private void MarcarTodos()
    {
        foreach (var d in Dibujos)
            d.Seleccionado = true;
        RecalcularResumen();
    }

    /// <summary>Deselecciona todos los dibujos (equivale a btnDesmarcarTodos / DeseleccionaTodos).</summary>
    [RelayCommand]
    private void DesmarcarTodos()
    {
        foreach (var d in Dibujos)
            d.Seleccionado = false;
        RecalcularResumen();
    }

    /// <summary>
    /// Aplica el filtro de dibujos al grupo activo y cierra la condición.
    /// </summary>
    [RelayCommand]
    private void Aceptar()
    {
        // Equivale a DibujosFrm.menuCondiciones1_BOk (Free1X2/UI/Filtros/DibujosFrm.cs líneas 254-277).
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) { Volver?.Invoke(); return; }

        var filtro = (FiltroDibujos)grupo.GetFiltro(Filtro.Dibujos.ToString());

        // El setter de filtro.Dibujos reconstruye la matriz interna y pone ContieneDatos.
        var lista = new ArrayList();
        foreach (var celda in Dibujos.Where(d => d.Seleccionado))
        {
            lista.Add(celda.Etiqueta);
        }

        if (lista.Count > 0)
        {
            filtro.ContieneDatos = true;
            filtro.IsActive = true;
            filtro.Dibujos = lista;
        }
        else
        {
            filtro.IsActive = false;
            filtro.ContieneDatos = false;
            filtro.Dibujos = lista; // vacía
        }

        grupo.ActivaFiltro(filtro);
        AppState.Instancia.NotificarCambio();
        Volver?.Invoke();
    }

    /// <summary>Calcula estadísticas del filtro temporal de dibujos.</summary>
    [RelayCommand]
    private void Estadisticas()
    {
        // TODO (dominio): construir un FiltroDibujos temporal (ObtenerFiltroTemporal) y llamar a
        //   CalculadorEstadisticas.EstadisticasFiltro(filtroTemp, ".../Ganadoras/")
        //   mostrando el resultado en VisorEstadisticas.
    }

    /// <summary>Guarda la condición de dibujos a un archivo .dbj/.xml.</summary>
    [RelayCommand]
    private void Guardar()
    {
        // TODO (dominio): ArchivoCondiciones.GuardaArchivo(filtroDibujos) tras elegir ruta.
    }

    /// <summary>Abre una condición de dibujos desde un archivo .dbj/.xml.</summary>
    [RelayCommand]
    private void Abrir()
    {
        // TODO (dominio): ArchivoCondiciones.AbrirArchivoCombinacion(ruta) + LeeCondicion(),
        //   luego volcar los dibujos del filtro a la malla (MarcarValores/MarcarDibujos).
    }

    /// <summary>Borra los datos del filtro de dibujos.</summary>
    [RelayCommand]
    private void Borrar()
    {
        // TODO (dominio): grupo.ActivaFiltro(new Grupo(), "Dibujos", true);
        DesmarcarTodos();
    }

    /// <summary>Cancela y descarta los cambios de pantalla.</summary>
    [RelayCommand]
    private void Cancelar()
    {
        // Equivale a menuCondiciones1_BCancelar -> CerrarVentana() (sin aplicar cambios).
        Volver?.Invoke();
    }
}
