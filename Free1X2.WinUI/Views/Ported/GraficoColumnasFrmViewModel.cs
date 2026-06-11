using System.Collections.ObjectModel;
using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// ViewModel para la pantalla "Representación gráfica de la combinación" (legacy: GraficoColumnasFrm).
/// Abre un fichero de columnas (.txt), calcula los límites (mínimo/máximo) de la combinación
/// y la representa gráficamente en 10 franjas. La "Guía" muestra qué fila de la quiniela
/// corresponde a cada franja del gráfico.
/// </summary>
public partial class GraficoColumnasFrmViewModel : ObservableObject
{
    /// <summary>Ruta del fichero de columnas a representar (legacy: abreFicheroDialog.FileName).</summary>
    [ObservableProperty]
    private string _ficheroEntrada = string.Empty;

    /// <summary>Texto de estado / resumen tras abrir y representar la combinación.</summary>
    [ObservableProperty]
    private string _estadoTexto = "Selecciona un fichero de columnas y pulsa \"Abrir combinación\".";

    /// <summary>Valor mínimo de la combinación, como texto (legacy: campo 'minimo').</summary>
    [ObservableProperty]
    private string _minimoTexto = "—";

    /// <summary>Valor máximo de la combinación, como texto (legacy: campo 'maximo').</summary>
    [ObservableProperty]
    private string _maximoTexto = "—";

    /// <summary>Indica si hay una combinación ya representada (habilita la Guía).</summary>
    [ObservableProperty]
    private bool _hayCombinacion;

    /// <summary>
    /// Líneas de la guía: para cada una de las 10 franjas, la fila de la quiniela representada
    /// (legacy: btnGuia_Click, deBase10 + reemplazos 1->X, 0->1).
    /// </summary>
    public ObservableCollection<string> LineasGuia { get; } = new();

    /// <summary>
    /// Abre el fichero de columnas y dispara la representación gráfica.
    /// </summary>
    [RelayCommand]
    private void AbrirCombinacion()
    {
        // TODO[dominio]: abrir diálogo de fichero y representar la combinación.
        //   Legacy: GraficoColumnasFrm.btnAbrir_Click -> inicio=true; Refresh(); -> GraficoColumnasFrm_Paint
        //     - OpenFileDialog: InitialDirectory "Columnas\\", filtro "Columnas(*.txt)|*.txt|Todos (*.*)|*.*".
        //       En WinUI usar Windows.Storage.Pickers.FileOpenPicker.
        //     - IArchivoColumnas archComb = new Free1X2.EntradaSalida.ArchivoColumnasTexto(ruta);
        //       int[] matrizColumnas = archComb.LeerTodasColsANumero(); archComb.Cerrar();
        //     - Array.Sort(matrizColumnas); minimo = matrizColumnas[0]; maximo = matrizColumnas[^1];
        //       int diferencia = maximo - minimo;
        //     - Escala: si diferencia<9566 escala=1 (resto se rellena en rojo), si no escala=diferencia/9565.938.
        //       Para cada apuesta: num=(col-minimo)/escala; alto=(num/958)-0.5; ancho=num-(alto*958);
        //       dibujar línea vertical en (ancho+10, alto*25+51)->(ancho+10, alto*25+74) sobre 10 franjas de 959x25.
        //   En WinUI el dibujo iría a un Canvas / CanvasControl (Win2D) en el code-behind/control.
        //   Free1X2.EntradaSalida.IArchivoColumnas aún no está migrado a Free1X2.Domain.
    }

    /// <summary>
    /// Detiene la representación en curso (legacy: btnSalir_Click -> para = true).
    /// </summary>
    [RelayCommand]
    private void Cancelar()
    {
        // TODO[dominio]: cancelar el bucle de dibujo en curso.
        //   Legacy: GraficoColumnasFrm.btnSalir_Click establece 'para = true', que corta
        //   el bucle de pintado de líneas dentro de GraficoColumnasFrm_Paint.
    }

    /// <summary>
    /// Muestra la guía: qué fila de la quiniela corresponde a cada una de las 10 franjas.
    /// </summary>
    [RelayCommand]
    private void MostrarGuia()
    {
        // TODO[dominio]: calcular las 10 filas representadas y rellenar LineasGuia.
        //   Legacy: GraficoColumnasFrm.btnGuia_Click
        //     int diferencia = maximo - minimo;
        //     for (i=1..10):
        //       string tmp = deBase10(((i-1)*(diferencia/10)) + minimo, 3, 14);  // base 3, 14 dígitos
        //       tmp = tmp.Replace("1","X").Replace("0","1");                       // 0->1, 1->X, 2->2
        //       linea = i.ToString("00") + ")  " + tmp;
        //   'deBase10' (conversión a base 3) está en el propio GraficoColumnasFrm legacy.
        //   Requiere 'minimo'/'maximo' calculados al abrir la combinación.
    }
}
