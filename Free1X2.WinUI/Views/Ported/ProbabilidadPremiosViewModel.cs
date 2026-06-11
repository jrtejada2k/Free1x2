using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Fila de resultados (solo lectura): categoría de aciertos y su probabilidad de premio.
/// Legacy: labels "lbl_NN" generados dinámicamente en ProbabilidadPremios.ButtonClick,
/// con texto "Premio de NN: pp,pp %".
/// </summary>
public partial class ProbabilidadPremioItem : ObservableObject
{
    [ObservableProperty]
    private string _categoria = string.Empty;

    // Regla anti-crash 2: el valor numérico se expone ya formateado como string.
    [ObservableProperty]
    private string _probabilidadTexto = "-";
}

/// <summary>
/// ViewModel de la pantalla "Probabilidades de Premios" (legacy: ProbabilidadPremios).
/// Selecciona dos ficheros de columnas (madre/hijas), un rango de aciertos y, al calcular,
/// produce la probabilidad de premio por cada categoría dentro del rango.
/// </summary>
public partial class ProbabilidadPremiosViewModel : ObservableObject
{
    // --- Selección de ficheros (legacy: archivoColsBase / archivoCols, labels lblCombMadre / lblCombHija) ---

    // Ruta completa del fichero de columnas "madre"/base. Legacy: campo archivoColsBase.
    private string _archivoColsMadre = string.Empty;

    // Ruta completa del fichero de columnas "hijas". Legacy: campo archivoCols.
    private string _archivoColsHija = string.Empty;

    // Nombre mostrado de la combinación madre. Legacy: lblCombMadre.Text (sin extensión).
    [ObservableProperty]
    private string _nombreColumnasMadre = "(seleccionar)";

    // Nombre mostrado de la combinación hija. Legacy: lblCombHija.Text (sin extensión).
    [ObservableProperty]
    private string _nombreColumnasHija = "(seleccionar)";

    // --- Rango de premios (legacy: txtRango) ---
    [ObservableProperty]
    private string _rangoPremios = string.Empty;

    // --- Habilitación del botón Calcular (legacy: button.Enabled gestionado por ActivarBotonCalculo) ---
    // Regla anti-crash 1: este bool se bindea a IsEnabled de un Control (Button), no de un panel.
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PuedeCalcular))]
    private bool _madreSeleccionada;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PuedeCalcular))]
    private bool _hijaSeleccionada;

    public bool PuedeCalcular => MadreSeleccionada && HijaSeleccionada;

    // --- Resultados (legacy: labels lbl_NN añadidos dinámicamente al form) ---
    public ObservableCollection<ProbabilidadPremioItem> Resultados { get; } = new();

    [RelayCommand]
    private void SeleccionarMadre()
    {
        // TODO [dominio]: abrir OpenFileDialog (filtro "Columnas (*.txt)") como en
        // ProbabilidadPremios.btnSelectMadreClick. En WinUI 3 usar FileOpenPicker.
        // Al confirmar: _archivoColsMadre = ruta; NombreColumnasMadre = Path.GetFileNameWithoutExtension(ruta);
        // MadreSeleccionada = true; (legacy: ActivarBotonCalculo()).
    }

    [RelayCommand]
    private void SeleccionarHija()
    {
        // TODO [dominio]: abrir OpenFileDialog (filtro "Columnas (*.txt)") como en
        // ProbabilidadPremios.btnSelectHijaClick. En WinUI 3 usar FileOpenPicker.
        // Al confirmar: _archivoColsHija = ruta; NombreColumnasHija = Path.GetFileNameWithoutExtension(ruta);
        // HijaSeleccionada = true; (legacy: ActivarBotonCalculo()).
    }

    [RelayCommand]
    private void Calcular()
    {
        // TODO [dominio]: replicar ProbabilidadPremios.ButtonClick.
        //  1. Parsear RangoPremios con Free1X2.Utils.UtilidadesEntradasValores.ObtenerListaFromTxtAciertos(RangoPremios),
        //     ordenar y tomar min/max; si vacío o error usar 10..14.
        //  2. Crear Free1X2.Analisis.Analizador(minimoPremio) y llamar
        //     ComparaCombinaciones(_archivoColsMadre, _archivoColsHija).
        //  3. Para i de min..max: analizador.ObtenProbabilidadPremios(i) formateado como "#,##0.00;0.00".
        //  Por ahora solo se limpia la colección; el cálculo real depende del dominio.
        Resultados.Clear();

        // Ejemplo de cómo se poblarían las filas una vez disponible el dominio:
        // foreach (var (cat, prob) in resultadosDominio)
        //     Resultados.Add(new ProbabilidadPremioItem
        //     {
        //         Categoria = $"Premio de {cat}",
        //         ProbabilidadTexto = $"{prob.ToString("#,##0.00;0.00")} %"
        //     });
    }
}
