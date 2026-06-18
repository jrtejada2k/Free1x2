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
}
