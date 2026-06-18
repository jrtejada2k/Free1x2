// Free1X2 · WinUI 3 — WIN3
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "EscrutiniosFrm" (título "Escrutinios").
/// Escruta uno o varios ficheros de columnas en tres modos: contra una columna
/// ganadora manual, contra otro fichero de referencia, o contra las jornadas del
/// histórico. El motor (Escrutador), la lectura de Jornadas/Resultados.txt, la
/// grabación de columnas y la lista de premiadas están cableados en el ViewModel.
/// </summary>
public sealed partial class EscrutiniosFrmPage : Page
{
    public EscrutiniosFrmViewModel ViewModel { get; } = new();

    public EscrutiniosFrmPage()
    {
        InitializeComponent();

        // La VM navega a través del Frame de la página (mismo patrón que MainPage/PosiblesPremios):
        //   PosiblesPremios -> PosiblesPremiosFrmPage; Cancelar -> GoBack (legacy Close()).
        ViewModel.Navegar = (tipo, parametro) => Frame?.Navigate(tipo, parametro);
        ViewModel.Volver = () => { if (Frame?.CanGoBack == true) Frame.GoBack(); };
    }

    /// <summary>
    /// Propaga al ViewModel las temporadas marcadas (legacy: lstTemporadas.SelectedIndices,
    /// MultiExtended). La selección múltiple vive en el control de UI.
    /// </summary>
    private void OnTemporadasSeleccionadas(object sender, SelectionChangedEventArgs e)
    {
        ViewModel.TemporadasSeleccionadas.Clear();
        foreach (var item in ListaTemporadas.SelectedItems)
        {
            if (item is string s && int.TryParse(s, out int temp))
            {
                ViewModel.TemporadasSeleccionadas.Add(temp);
            }
        }
    }

    // Inserción de marcadores /t y /j en la posición del cursor (legacy EscrutiniosFrm.incluirPrefijo_click).
    private void InsertarTemporada_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        => InsertarPrefijoEnCursor("/t");

    private void InsertarJornada_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
        => InsertarPrefijoEnCursor("/j");

    /// <summary>
    /// Inserta el prefijo ("/t" o "/j") en la posición del cursor del TextBox de plantilla y
    /// recoloca el cursor tras el marcador. Réplica fiel de EscrutiniosFrm.incluirPrefijo_click
    /// (Free1X2/UI/EscrutiniosFrm.cs:1531-1547): i = SelectionStart; Text = izq + prefijo + der;
    /// SelectionStart = i + 2; SelectionLength = 0. Si no hay cursor/selección válidos, añade al final.
    /// </summary>
    private void InsertarPrefijoEnCursor(string prefijo)
    {
        var caja = CajaPlantilla;
        string texto = caja.Text ?? string.Empty;

        // Posición del cursor; si fuera inválida (sin foco previo), se trata como "al final".
        int i = caja.SelectionStart;
        if (i < 0 || i > texto.Length) i = texto.Length;

        string nuevo = texto.Substring(0, i) + prefijo + texto.Substring(i);

        // Actualiza el texto (propaga a ViewModel.PlantillaNombreArchivo por el binding TwoWay).
        caja.Text = nuevo;

        // Recoloca el cursor justo después del marcador insertado (legacy: i + 2).
        caja.Focus(Microsoft.UI.Xaml.FocusState.Programmatic);
        caja.SelectionStart = i + prefijo.Length;
        caja.SelectionLength = 0;
    }
}
