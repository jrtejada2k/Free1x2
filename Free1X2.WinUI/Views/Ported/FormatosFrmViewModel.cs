using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2;
using Free1X2.MotorCalculo;
using Free1X2.Utils;
using Free1X2.WinUI.Services;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Una línea de formato dentro de un <see cref="FormatosViewModel"/>:
/// la secuencia de signos (1/X/2/V/*) y su rango de apariciones Min-Max.
/// Equivale al tipo de dominio legacy Free1X2.MotorCalculo.FormatoSignos.
/// </summary>
public partial class LineaFormatoViewModel : ObservableObject
{
    [ObservableProperty]
    private string _formato = string.Empty;

    [ObservableProperty]
    private string _rangoAparicion = string.Empty;
}

/// <summary>
/// Un conjunto de líneas de formato (una "relación"), con sus límites
/// globales de Líneas y Global. Equivale a Free1X2.MotorCalculo.FormatosSignos.
/// </summary>
public partial class FormatosViewModel : ObservableObject
{
    public ObservableCollection<LineaFormatoViewModel> Lineas { get; } = new();

    /// <summary>Límite de líneas para esta relación de formatos.</summary>
    [ObservableProperty]
    private string _limiteLineas = string.Empty;

    /// <summary>Límite global para esta relación de formatos.</summary>
    [ObservableProperty]
    private string _global = string.Empty;

    public FormatosViewModel()
    {
        // El form legacy muestra 30 filas en blanco por relación.
        for (int i = 0; i < FilasPorRelacion; i++)
        {
            Lineas.Add(new LineaFormatoViewModel());
        }
    }

    public const int FilasPorRelacion = 30;
}

/// <summary>
/// ViewModel de la pantalla "Formatos (1,X,2,V,*)".
///
/// Un formato es una determinada secuencia de signos; esta condición controla
/// la repetición o aparición de diferentes formatos en las columnas generadas.
/// El usuario define una o varias relaciones de formatos navegables (1/N) y, por
/// cada una, hasta 30 líneas (secuencia + rango Min-Max) más los límites Líneas/Global.
///
/// Datos en memoria; la persistencia y el cálculo aún viven en el dominio legacy
/// (ver los TODO en FormatosFrmPage.xaml.cs).
/// </summary>
public partial class FormatosFrmViewModel : ObservableObject
{
    public ObservableCollection<FormatosViewModel> Relaciones { get; } = new();

    [ObservableProperty]
    private int _indiceRelacion;

    /// <summary>Acción para volver atrás (la cablea la página con Frame.GoBack()). CerrarVentana() legacy.</summary>
    public Action? Volver { get; set; }

    public FormatosFrmViewModel()
    {
        Relaciones.Add(new FormatosViewModel());
        IndiceRelacion = 0;
    }

    /// <summary>
    /// Vuelca las relaciones de formatos del FiltroFormatosSignos del grupo en edición.
    /// Equivale a FormatosFrm.InicializaDatos()/ObtenCopiaFormatos()/ActualizaDatosPantalla()
    /// (Free1X2/UI/Filtros/FormatosFrm.cs líneas 112-171).
    /// </summary>
    public void CargarDesdeGrupo()
    {
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) return;

        var filtro = (FiltroFormatosSignos)grupo.GetFiltro(Filtro.FormatosSignos.ToString());

        Relaciones.Clear();
        foreach (FormatosSignos formatos in filtro.FormatosSignos)
        {
            var rel = new FormatosViewModel();
            rel.Lineas.Clear();
            foreach (FormatoSignos linea in formatos.LineasFormatos)
            {
                rel.Lineas.Add(new LineaFormatoViewModel
                {
                    Formato = linea.Formato,
                    RangoAparicion = linea.RangoAparicion,
                });
            }
            // El form legacy completa hasta 30 filas en blanco para edición.
            while (rel.Lineas.Count < FormatosViewModel.FilasPorRelacion)
            {
                rel.Lineas.Add(new LineaFormatoViewModel());
            }
            rel.LimiteLineas = formatos.Lineas;
            rel.Global = formatos.Global;
            Relaciones.Add(rel);
        }

        if (Relaciones.Count == 0)
        {
            Relaciones.Add(new FormatosViewModel());
        }

        IndiceRelacion = 0;
        OnPropertyChanged(nameof(RelacionActual));
        OnPropertyChanged(nameof(ContadorTexto));
        OnPropertyChanged(nameof(PuedeRetroceder));
        RetrocederCommand.NotifyCanExecuteChanged();
    }

    // Caracteres permitidos en un formato (FormatosFrm.CompruebaFomato línea 489).
    private static readonly char[] CaracteresFormato = { '1', 'X', '2', 'V', '*' };

    private static bool FormatoEsValido(string formato)
    {
        if (formato.Length > VariablesGlobales.NumeroPartidos) return false;
        foreach (char c in formato)
        {
            if (Array.IndexOf(CaracteresFormato, c) < 0) return false;
        }
        return true;
    }

    /// <summary>
    /// Construye la List&lt;FormatosSignos&gt; a partir de las relaciones de pantalla,
    /// replicando ObtenDatosGrid + GuardarDatosFormatos + NecesitaBorrarUltimoFormato del form.
    /// </summary>
    private List<FormatosSignos> ConstruirFormatos()
    {
        var resultado = new List<FormatosSignos>();
        foreach (var rel in Relaciones)
        {
            var formatos = new FormatosSignos();
            var lineas = new List<FormatoSignos>();
            foreach (var linea in rel.Lineas)
            {
                string formato = (linea.Formato ?? "").Trim().ToUpper();
                string rangos = (linea.RangoAparicion ?? "").Trim();
                if (FormatoEsValido(formato) && formato != "" && rangos != "")
                {
                    lineas.Add(new FormatoSignos { Formato = formato, RangoAparicion = rangos });
                }
            }
            formatos.LineasFormatos = lineas;
            // Lineas/Global solo si son numéricos (GuardarDatosFormatos líneas 331-347).
            string limLineas = (rel.LimiteLineas ?? "").Trim();
            formatos.Lineas = UtilidadesEntradasValores.SonTodosNumeros(limLineas) ? limLineas : "";
            string limGlobal = (rel.Global ?? "").Trim();
            formatos.Global = UtilidadesEntradasValores.SonTodosNumeros(limGlobal) ? limGlobal : "";
            resultado.Add(formatos);
        }

        // Borrar la última relación si no tiene ninguna línea con formato (NecesitaBorrarUltimoFormato).
        if (resultado.Count > 0)
        {
            var ultima = resultado[resultado.Count - 1];
            bool tieneFormato = false;
            foreach (var l in ultima.LineasFormatos)
            {
                if (l.Formato != "") { tieneFormato = true; break; }
            }
            if (!tieneFormato) resultado.RemoveAt(resultado.Count - 1);
        }

        return resultado;
    }

    [RelayCommand]
    private void Aceptar()
    {
        // Equivale a FormatosFrm.menuCondiciones1_BOk -> GuardarDatos() + ActivaFiltro
        //   (Free1X2/UI/Filtros/FormatosFrm.cs líneas 352-374, 917-922).
        var grupo = AppState.GrupoEnEdicion;
        if (grupo is null) { Volver?.Invoke(); return; }

        var filtro = (FiltroFormatosSignos)grupo.GetFiltro(Filtro.FormatosSignos.ToString());
        var grupoFormatos = ConstruirFormatos();

        if (filtro.ContieneDatos == false && grupoFormatos.Count > 0)
        {
            filtro.ContieneDatos = true;
            filtro.IsActive = true;
        }
        if (grupoFormatos.Count == 0)
        {
            filtro.ContieneDatos = false;
            filtro.IsActive = false;
        }
        filtro.FormatosSignos = grupoFormatos;

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

    /// <summary>Relación de formatos actualmente visible.</summary>
    public FormatosViewModel? RelacionActual =>
        IndiceRelacion >= 0 && IndiceRelacion < Relaciones.Count
            ? Relaciones[IndiceRelacion]
            : null;

    /// <summary>Contador "N/Total" mostrado en la cabecera (legacy lblNoFormatos).</summary>
    public string ContadorTexto =>
        Relaciones.Count == 0 ? "0/0" : $"{IndiceRelacion + 1}/{Relaciones.Count}";

    public bool PuedeRetroceder => IndiceRelacion > 0;

    partial void OnIndiceRelacionChanged(int value)
    {
        OnPropertyChanged(nameof(RelacionActual));
        OnPropertyChanged(nameof(ContadorTexto));
        OnPropertyChanged(nameof(PuedeRetroceder));
        RetrocederCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand(CanExecute = nameof(PuedeRetroceder))]
    private void Retroceder()
    {
        if (IndiceRelacion > 0)
        {
            IndiceRelacion--;
        }
    }

    [RelayCommand]
    private void Avanzar()
    {
        // El legacy crea una relación nueva al avanzar más allá de la última.
        if (IndiceRelacion + 1 >= Relaciones.Count)
        {
            Relaciones.Add(new FormatosViewModel());
            OnPropertyChanged(nameof(ContadorTexto));
        }
        IndiceRelacion++;
    }

    [RelayCommand]
    private void EliminarActual()
    {
        if (Relaciones.Count == 0)
        {
            return;
        }

        Relaciones.RemoveAt(IndiceRelacion);
        if (Relaciones.Count == 0)
        {
            Relaciones.Add(new FormatosViewModel());
        }

        if (IndiceRelacion >= Relaciones.Count)
        {
            IndiceRelacion = Relaciones.Count - 1;
        }

        OnPropertyChanged(nameof(RelacionActual));
        OnPropertyChanged(nameof(ContadorTexto));
        OnPropertyChanged(nameof(PuedeRetroceder));
        RetrocederCommand.NotifyCanExecuteChanged();
    }
}
