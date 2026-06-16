using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Free1X2.Utils;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Una casilla editable de figura. Equivale a un <c>CtrlCasillaFigura</c> dentro del
/// <c>CtrlFiguras</c> del WinForms <c>FigurasFiltrosFrm</c>: el usuario escribe el valor
/// de una figura (cada dígito hexadecimal admite "0".."16" o el comodín "*").
/// </summary>
public partial class CasillaFiguraViewModel : ObservableObject
{
    public CasillaFiguraViewModel(int indice, string texto = "")
    {
        Indice = indice;
        _texto = texto;
    }

    /// <summary>Posición de la casilla dentro de la rejilla (0..N-1). Solo informativa.</summary>
    public int Indice { get; }

    /// <summary>Etiqueta visible de la casilla (1-based) ya formateada como cadena.</summary>
    public string Etiqueta => (Indice + 1).ToString();

    /// <summary>Texto de la figura tal como lo escribe el usuario (p. ej. "1234*").</summary>
    [ObservableProperty]
    private string _texto = string.Empty;
}

/// <summary>
/// ViewModel de la pantalla portada del WinForms "FigurasFiltrosFrm".
/// Mantiene la rejilla de casillas de figuras y las acciones del form legacy
/// (Aceptar, Abrir desde archivo, Borrar, Cancelar). La conversión de texto a
/// figura (long) y la persistencia se delegan al dominio legacy (ver TODOs).
/// </summary>
public partial class FigurasFiltrosFrmViewModel : ObservableObject
{
    // Valores admitidos por dígito de figura (legacy: valoresPermitidos en FigurasFiltrosFrm).
    private const int NumeroCasillasPorDefecto = 12;

    /// <summary>
    /// Lista de figuras que el form padre (Contactos / SignosSeguidos / PesosNum) pasa POR REFERENCIA
    /// para que esta pantalla la edite (análogo al ctor legacy FigurasFiltrosFrm(List&lt;long&gt; figuras, ...)).
    /// Es un handoff estático al estilo de AppState.GrupoEnEdicion: el padre lo asigna antes de navegar
    /// y lee los cambios al volver. Mientras los padres no naveguen aquí, queda en null (lista local).
    /// </summary>
    public static List<long>? FigurasEnEdicion { get; set; }

    private List<long> _figurasCondicion = new();

    /// <summary>Acción para volver atrás (la cablea la página con Frame.GoBack()).</summary>
    public Action? Volver { get; set; }

    public FigurasFiltrosFrmViewModel()
    {
        Casillas = new ObservableCollection<CasillaFiguraViewModel>();
        for (int i = 0; i < NumeroCasillasPorDefecto; i++)
        {
            Casillas.Add(new CasillaFiguraViewModel(i));
        }
    }

    /// <summary>
    /// Vuelca la lista de figuras recibida (FigurasEnEdicion) en las casillas.
    /// Equivale al ctor legacy FigurasFiltrosFrm: new CtrlFiguras(figuras) (líneas 41-63).
    /// </summary>
    public void CargarDesdeHandoff()
    {
        _figurasCondicion = FigurasEnEdicion ?? new List<long>();

        // Asegurar suficientes casillas + algunas vacías de margen.
        while (Casillas.Count < _figurasCondicion.Count + 2)
        {
            Casillas.Add(new CasillaFiguraViewModel(Casillas.Count));
        }
        for (int i = 0; i < Casillas.Count; i++)
        {
            Casillas[i].Texto = i < _figurasCondicion.Count
                ? UtilidadesEntradasValores.ObtenerTextoFiguraFromLong(_figurasCondicion[i])
                : string.Empty;
        }
    }

    /// <summary>Casillas editables de la rejilla de figuras.</summary>
    public ObservableCollection<CasillaFiguraViewModel> Casillas { get; }

    /// <summary>
    /// Texto de ayuda con los valores permitidos por casilla.
    /// (Equivale a valoresPermitidos = 0..16 y "*" en el form legacy.)
    /// </summary>
    public string ValoresPermitidosTexto =>
        "Valores admitidos por casilla: 0 a 16, o el comodín *.";

    // Valida que cada dígito hexadecimal de la figura esté en 0..16 (legacy: valoresPermitidos).
    private static bool EsFiguraValida(long figura)
    {
        long temp = figura;
        while (temp != 0)
        {
            int valor = (int)(temp & 15);
            if (valor < 0 || valor > 16) return false;
            temp >>= 4;
        }
        return true;
    }

    [RelayCommand]
    private void Aceptar()
    {
        // Equivale a FigurasFiltrosFrm.ObtenerFiguras() + btnOk_Click() (líneas 70-110).
        _figurasCondicion.Clear();
        foreach (var casilla in Casillas)
        {
            string texto = (casilla.Texto ?? "").Trim();
            if (texto == "") continue;
            long figura;
            try
            {
                figura = UtilidadesEntradasValores.ObtenerLongFiguraFromText(texto);
            }
            catch
            {
                continue; // entrada no convertible (p. ej. comodín "*"): se omite.
            }
            if (EsFiguraValida(figura) && !_figurasCondicion.Contains(figura))
            {
                _figurasCondicion.Add(figura);
            }
        }
        // El padre comparte la misma referencia de lista (FigurasEnEdicion), así que ve los cambios.
        Volver?.Invoke();
    }

    [RelayCommand]
    private void Abrir()
    {
        // TODO[persistencia]: cargar figuras desde un archivo ".fig<Condicion>" con FileOpenPicker
        //   (FigurasFiltrosFrm.btnAbrir_Click líneas 253-280): leer líneas -> ObtenerLongFiguraFromText
        //   y recargar la rejilla. DeterminarCondicion() depende de NombreFiltro (Contactos / V1X2).
    }

    [RelayCommand]
    private void Borrar()
    {
        // Legacy FigurasFiltrosFrm.BorrarTodo(): vacía todas las casillas y la lista de figuras.
        foreach (var casilla in Casillas)
        {
            casilla.Texto = string.Empty;
        }
        _figurasCondicion.Clear();
    }

    [RelayCommand]
    private void Cancelar()
    {
        // Equivale a btnCancelar_Click -> this.Close() (sin aplicar cambios).
        Volver?.Invoke();
    }
}
