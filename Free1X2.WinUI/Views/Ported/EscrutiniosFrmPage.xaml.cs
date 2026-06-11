using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Page portada del WinForms legacy "EscrutiniosFrm" (título "Escrutinios").
/// Escruta uno o varios ficheros de columnas en tres modos: contra una columna
/// ganadora manual, contra otro fichero de referencia, o contra las jornadas del
/// histórico. La lógica de dominio (Escrutador, ArchivoColumnasTexto, lectura de
/// Jornadas/Resultados.txt, Free1X2WService, selección de archivos) queda como TODO.
/// </summary>
public sealed partial class EscrutiniosFrmPage : Page
{
    public EscrutiniosFrmViewModel ViewModel { get; } = new();

    public EscrutiniosFrmPage()
    {
        InitializeComponent();
    }
}
