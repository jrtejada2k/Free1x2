using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port WinUI 3 del WinForms <c>DiferenciasFrm</c> (filtro "Diferencias").
/// El usuario define grupos de partidos (una línea por grupo, partidos separados por
/// "," o "-") y, por concepto (Variantes / Equis / Doses / Dibujos / Interrupciones /
/// Formatos), la cantidad o intervalo de valores DISTINTOS entre los grupos. Permite
/// guardar varios conjuntos (Diferencias) y navegar entre ellos, generar grupos por
/// atajos (Dúos..Octetos) y las acciones de menú (Aceptar, Estadísticas, Guardar,
/// Abrir, Copiar, Pegar, Borrar, Cancelar). La lógica de dominio (FiltroDiferencias,
/// Diferencia, ArchivoCondiciones, CalculadorEstadisticas) está marcada como TODO en el ViewModel.
/// </summary>
public sealed partial class DiferenciasFrmPage : Page
{
    public DiferenciasFrmViewModel ViewModel { get; } = new();

    public DiferenciasFrmPage()
    {
        this.InitializeComponent();
    }
}
