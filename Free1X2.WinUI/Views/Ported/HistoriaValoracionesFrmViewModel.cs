using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para HistoriaValoracionesFrmPage.
/// Porta el WinForms legacy "HistoriaValoracionesFrm" (UI/HistoriaValoracionesFrm.cs).
/// Proposito: guardar las valoraciones 1X2 (double[14,3]) de una jornada/temporada
/// en un fichero historico de valoraciones.
/// </summary>
public partial class HistoriaValoracionesFrmViewModel : ObservableObject
{
    // Temporada de inicio (legacy: int Temporada, txTemporada).
    [ObservableProperty]
    private double _temporada = 2004;

    // Jornada a guardar (legacy: int Jornada, txJornada). Rango legacy: 0..43.
    [ObservableProperty]
    private double _jornada = 1;

    // Nombre / ruta del fichero historico de salida (legacy: archivoSalida / txNombreFichero).
    [ObservableProperty]
    private string _nombreFichero = string.Empty;

    // Habilita el boton Guardar solo cuando hay fichero seleccionado (legacy: btGuardar.Enabled).
    [ObservableProperty]
    private bool _puedeGuardar;

    // Texto de la temporada siguiente (legacy: txTemporada2, deshabilitado, = Temporada+1).
    // Regla anti-crash #2: nunca bindear int/double directo a TextBlock.Text.
    public string TemporadaSiguienteTexto => ((int)Temporada + 1).ToString();

    partial void OnTemporadaChanged(double value) => OnPropertyChanged(nameof(TemporadaSiguienteTexto));

    // --- Spinners de temporada (legacy: btTemporadaAnterior_Click / btTemporadaSiguiente_Click) ---

    [RelayCommand]
    private void TemporadaAnterior()
    {
        Temporada -= 1;
    }

    [RelayCommand]
    private void TemporadaSiguiente()
    {
        Temporada += 1;
    }

    // --- Spinners de jornada (legacy: btJornadaAnterior_Click / btJornadaSiguiente_Click, rango 0..43) ---

    [RelayCommand]
    private void JornadaAnterior()
    {
        if (Jornada > 0)
            Jornada -= 1;
    }

    [RelayCommand]
    private void JornadaSiguiente()
    {
        if (Jornada < 43)
            Jornada += 1;
    }

    // --- Seleccionar fichero existente (legacy: button1_Click / btSeleccionarFichero_Click, OpenFileDialog) ---
    [RelayCommand]
    private void SeleccionarFichero()
    {
        // TODO: dominio. Legacy HistoriaValoracionesFrm.button1_Click:
        //   OpenFileDialog (filtro "Valoraciones historicas(*.txt)") sobre carpeta "Columnas\\".
        //   Asignar archivoSalida, mostrar Path.GetFileName, habilitar Guardar.
        //   En WinUI usar FileOpenPicker y luego: PuedeGuardar = true.
    }

    // --- Crear fichero nuevo (legacy: btNuevo_Click, SaveFileDialog) ---
    [RelayCommand]
    private void NuevoFichero()
    {
        // TODO: dominio. Legacy HistoriaValoracionesFrm.btNuevo_Click:
        //   SaveFileDialog (filtro "Valoraciones historicas(*.txt)") sobre "Columnas\\".
        //   Asignar archivoSalida, mostrar Path.GetFileName, habilitar Guardar.
        //   En WinUI usar FileSavePicker y luego: PuedeGuardar = true.
    }

    // --- Guardar valoraciones en el historico (legacy: btGuardar_Click) ---
    [RelayCommand]
    private void Guardar()
    {
        // TODO: dominio. Legacy HistoriaValoracionesFrm.btGuardar_Click:
        //   string tempo = Temporada + "/" + (Temporada+1);
        //   string jorna = Jornada.PadLeft(2,'0');
        //   Pct.GuardarValoraciones(archivoSalida, (char)9, valores1X2, tempo, jorna);  // Porcentajes.GuardarValoraciones
        //   Close();
        //   (valores1X2 = double[14,3] se recibe del form invocante / Escrutinio).
    }

    // --- Cancelar (legacy: button2_Click -> Close) ---
    [RelayCommand]
    private void Cancelar()
    {
        // TODO: dominio. Legacy HistoriaValoracionesFrm.button2_Click: Close().
        //   En WinUI: navegar atras / cerrar la Page contenedora.
    }
}
