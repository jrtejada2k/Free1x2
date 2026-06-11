using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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

    public FigurasFiltrosFrmViewModel()
    {
        Casillas = new ObservableCollection<CasillaFiguraViewModel>();
        for (int i = 0; i < NumeroCasillasPorDefecto; i++)
        {
            Casillas.Add(new CasillaFiguraViewModel(i));
        }

        // TODO(dominio): rellenar las casillas con las figuras recibidas.
        //   Legacy FigurasFiltrosFrm(List<long> figuras, int longitudFigura, IFiltro filtro):
        //   crea un CtrlFiguras(figuras) y vuelca cada figura en una CtrlCasillaFigura.
    }

    /// <summary>Casillas editables de la rejilla de figuras.</summary>
    public ObservableCollection<CasillaFiguraViewModel> Casillas { get; }

    /// <summary>
    /// Texto de ayuda con los valores permitidos por casilla.
    /// (Equivale a valoresPermitidos = 0..16 y "*" en el form legacy.)
    /// </summary>
    public string ValoresPermitidosTexto =>
        "Valores admitidos por casilla: 0 a 16, o el comodín *.";

    [RelayCommand]
    private void Aceptar()
    {
        // TODO(dominio): recoger las figuras válidas y cerrar.
        //   Legacy FigurasFiltrosFrm.ObtenerFiguras() / btnOk_Click():
        //     foreach casilla -> Utils.UtilidadesEntradasValores.ObtenerLongFiguraFromText(texto);
        //     if (EsFiguraValida(figura) && !figurasCondicion.Contains(figura)) figurasCondicion.Add(figura);
        //     this.Close();
    }

    [RelayCommand]
    private void Abrir()
    {
        // TODO(dominio): cargar figuras desde un archivo ".fig<Condicion>" y recargar la rejilla.
        //   Legacy FigurasFiltrosFrm.btnAbrir_Click():
        //     OpenFileDialog (carpeta /Condiciones, filtro "*.fig" + DeterminarCondicion());
        //     StreamReader -> ObtenerLongFiguraFromText(linea) por cada línea;
        //     BorrarCasillas(); recrear CtrlFiguras(figurasCondicion).
        //   DeterminarCondicion() depende de _filtro.NombreFiltro (Contactos -> "Contactos", SignosSeguidos -> "V1X2").
    }

    [RelayCommand]
    private void Borrar()
    {
        // Legacy FigurasFiltrosFrm.BorrarTodo(): vacía todas las casillas y la lista de figuras.
        foreach (var casilla in Casillas)
        {
            casilla.Texto = string.Empty;
        }
        // TODO(dominio): figurasCondicion.Clear();
    }

    [RelayCommand]
    private void Cancelar()
    {
        // TODO(dominio): cerrar sin aplicar cambios.
        //   Legacy FigurasFiltrosFrm.btnCancelar_Click(): this.Close();
    }
}
