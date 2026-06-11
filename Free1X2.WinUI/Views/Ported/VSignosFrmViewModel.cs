using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la pantalla "Análisis de signos" (legacy: VSignosFrm).
/// Procesa un fichero de columnas y contabiliza, partido a partido, cuántas veces
/// aparece cada signo (1/X/2). Permite elegir el formato de presentación
/// (% enteros, % con decimales, o columnas/recuentos), introducir una columna
/// ganadora de referencia y fijar un nivel de "aspiración" para el escrutinio parcial.
/// </summary>
public partial class VSignosFrmViewModel : ObservableObject
{
    /// <summary>Ruta del fichero de columnas a procesar (legacy: archivo / lFileIn).</summary>
    [ObservableProperty]
    private string _ficheroEntrada = string.Empty;

    /// <summary>Nombre del fichero de entrada mostrado al usuario (legacy: lFileIn.Text).</summary>
    [ObservableProperty]
    private string _nombreFicheroEntrada = "Fichero a procesar";

    /// <summary>
    /// Modo de presentación seleccionado (legacy: rbcen0 / rbcen2 / rbcol).
    /// Bindea a ComboBox.SelectedItem; los items vienen de <see cref="ModosPresentacion"/>.
    /// </summary>
    [ObservableProperty]
    private string _modoPresentacion = "% enteros";

    /// <summary>Opciones del modo de presentación (legacy: grupo "Ver" con 3 radios).</summary>
    public IReadOnlyList<string> ModosPresentacion { get; } = new[]
    {
        "% enteros",      // rbcen0
        "% con decimales",// rbcen2
        "columnas",       // rbcol
    };

    /// <summary>Considerar el último partido como Pleno al 15 (legacy: chkPleno).</summary>
    [ObservableProperty]
    private bool _considerarPleno = true;

    /// <summary>
    /// Nivel de aspiración: nº mínimo de aciertos para contabilizar una columna
    /// en el escrutinio parcial (legacy: lbasp + botones bMas/bMenos).
    /// NumberBox.Value es double.
    /// </summary>
    [ObservableProperty]
    private double _aspiracion;

    /// <summary>Número de partidos en juego (legacy: noPartidos / lbasp tope).</summary>
    [ObservableProperty]
    private double _numeroPartidos = 15;

    // --- Resultados / estado del proceso (solo lectura para la vista) ---

    /// <summary>Columnas procesadas del fichero (legacy: lproc / ctproc).</summary>
    [ObservableProperty]
    private string _columnasProcesadasTexto = "Procesadas: -";

    /// <summary>Tiempo empleado en el cálculo (legacy: ltime).</summary>
    [ObservableProperty]
    private string _tiempoTexto = "Tiempo: -";

    /// <summary>Premios remanentes por categoría (legacy: lp100viu).</summary>
    [ObservableProperty]
    private string _premiosRemanentesTexto = "Premios remanentes: -";

    // --- Acciones ---

    /// <summary>Selecciona el fichero de columnas de entrada (legacy: BFileInClick -> EntradaFichero).</summary>
    [RelayCommand]
    private void AbrirFichero()
    {
        // TODO[dominio]: abrir diálogo y leer el nº de signos del fichero.
        //   Legacy: VSignosFrm.EntradaFichero()
        //     - OpenFileDialog filtro "*.txt|*.*", directorio Application.StartupPath + "/".
        //       En WinUI usar Windows.Storage.Pickers.FileOpenPicker.
        //     - IArchivoColumnas aCol = new Free1X2.EntradaSalida.ArchivoColumnasTexto(ruta);
        //       noPartidos = aCol.ObtenNumSignos(); aCol.Cerrar();
        //     - NombreFicheroEntrada = Path.GetFileName(ruta);
        //     - AdaptarVistaAPartidos(); AdaptarVariables(); Calcular();
        //   Free1X2.EntradaSalida.IArchivoColumnas aún no migrado a Free1X2.Domain.
    }

    /// <summary>Procesa el fichero y contabiliza los signos por partido (legacy: BCalcularClick -> Calcular).</summary>
    [RelayCommand]
    private void Calcular()
    {
        // TODO[dominio]: recorrer las columnas del fichero y contabilizar signos.
        //   Legacy: VSignosFrm.Calcular() / AcCB() / Contabiliza() / PintaPantalla()
        //     - StreamReader sobre 'archivo'; por cada línea (columna en mayúsculas):
        //         aciertos = AcCB();  // compara contra la columna ganadora 'part[]'
        //         if (aciertos >= limac) Contabiliza(); // vals[nr,0..2] por signo 1/X/2
        //         if (aciertos > 9) premis[noPartidos - aciertos]++;
        //     - 'limac' = nivel de Aspiracion; ConsiderarPleno condiciona el tratamiento
        //       del último partido (pleno al 15).
        //     - PintaPantalla() formatea vals según ModoPresentacion
        //       (% enteros f0, % decimales f2, o recuentos crudos) y rellena la rejilla.
        //   El recuento por partido se mostrará en la rejilla de la vista (Recuentos).
    }

    /// <summary>Graba el resultado (valoración o columnas aceptadas) a fichero (legacy: BGrabarClick -> Grabar).</summary>
    [RelayCommand]
    private void Grabar()
    {
        // TODO[dominio]: guardar el resultado en un fichero de texto.
        //   Legacy: VSignosFrm.Grabar()
        //     - SaveFileDialog filtro "Valoración(*.txt)" o "Columnas(*.txt)" según modo.
        //       En WinUI usar Windows.Storage.Pickers.FileSavePicker.
        //     - Modo % (rbcen0/rbcen2): escribe los porcentajes f0/f2 por partido.
        //     - Modo columnas (rbcol): vuelca la lista 'aceptadas' línea a línea.
    }

    /// <summary>Reinicia la columna ganadora a guiones (legacy: BLimpClick -> ReinicializaColumnaGanadora).</summary>
    [RelayCommand]
    private void LimpiarColumnaGanadora()
    {
        // TODO[dominio]: poner todos los signos de la columna ganadora a "-".
        //   Legacy: VSignosFrm.ReinicializaColumnaGanadora() -> txts[i].Text = "-".
        //   Aquí debería limpiar la colección de signos editables de la vista.
    }

    /// <summary>Sube en 1 el nivel de aspiración (legacy: BMasClick -> AspMas).</summary>
    [RelayCommand]
    private void SubirAspiracion()
    {
        // Legacy: VSignosFrm.AspMas() -> if (naux < noPartidos) lbasp = naux + 1.
        if (Aspiracion < NumeroPartidos)
        {
            Aspiracion += 1;
        }
    }

    /// <summary>Baja en 1 el nivel de aspiración (legacy: BMenosClick -> AspMenos).</summary>
    [RelayCommand]
    private void BajarAspiracion()
    {
        // Legacy: VSignosFrm.AspMenos() -> if (naux > 0) lbasp = naux - 1.
        if (Aspiracion > 0)
        {
            Aspiracion -= 1;
        }
    }
}
