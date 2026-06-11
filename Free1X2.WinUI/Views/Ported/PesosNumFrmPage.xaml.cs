using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página portada del WinForms "PesosNumFrm".
/// Edita una condición de filtro de Pesos Numéricos: conjuntos de dígitos (0..9) para
/// Global, Variantes, 1, X y 2 (más sus tolerancias homónimas), las tolerancias numéricas
/// 0..5 y las figuras seleccionables (3-2, 3-1-1, 2-2-1, 2-1-1-1, 1-1-1-1-1), con acciones
/// Aceptar, Estadísticas, Guardar, Abrir, Copiar, Pegar, Borrar y Cancelar.
/// El cálculo, la conversión texto→valores/figuras y la persistencia dependen del dominio
/// legacy (FiltroPesosNumericos / ArchivoCondiciones / CalculadorEstadisticas) y están
/// marcados como TODO en el ViewModel.
/// </summary>
public sealed partial class PesosNumFrmPage : Page
{
    public PesosNumFrmViewModel ViewModel { get; } = new();

    public PesosNumFrmPage()
    {
        this.InitializeComponent();
    }
}
