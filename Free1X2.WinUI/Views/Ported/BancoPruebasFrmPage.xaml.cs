// Free1X2 · WinUI 3 — WIN3
using System;
using System.Threading.Tasks;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Página WinUI 3 portada del formulario WinForms "BancoPruebasFrm"
/// (Simulador de escrutinios — Banco de Pruebas).
///
/// Asistente de 4 pasos: Ficheros, Valoraciones, Simular 14's y Escrutinios.
/// La lógica de cálculo / persistencia está implementada en el ViewModel.
/// </summary>
public sealed partial class BancoPruebasFrmPage : Page
{
    public BancoPruebasFrmViewModel ViewModel { get; } = new();

    public BancoPruebasFrmPage()
    {
        this.InitializeComponent();

        // Legacy btGrabar_Click: new DialogoGrabarBancoPruebasFrm(c1, c2, c).ShowDialog(). El VM pide
        // el rango de grabación y aquí se muestra DialogoGrabarBancoPruebasFrmPage como ContentDialog
        // (modal, fiel al ShowDialog original), devolviendo el resultado al VM.
        ViewModel.SolicitarRangoGrabacion = MostrarDialogoRangoGrabacionAsync;
    }

    /// <summary>
    /// Muestra el diálogo de rango de grabación hospedando DialogoGrabarBancoPruebasFrmPage en un
    /// ContentDialog. Inicializa los campos (c1, c2, c) como el ctor legacy y, cuando la VM del
    /// diálogo solicita el cierre (Aceptar/Cancelar), recoge el resultado.
    /// </summary>
    private async Task<(bool aceptado, int filaInicial, int filaFinal, int numMaxColumnas, bool soloSeleccionadas)>
        MostrarDialogoRangoGrabacionAsync(int c1, int c2, int c)
    {
        var dialogPage = new DialogoGrabarBancoPruebasFrmPage();
        dialogPage.ViewModel.Inicializar(c1, c2, c);

        var contentDialog = new ContentDialog
        {
            XamlRoot = this.XamlRoot,
            Content = dialogPage,
        };

        // La VM del diálogo levanta CierreSolicitado al Aceptar/Cancelar (sus botones internos);
        // cerramos el ContentDialog para devolver el control al flujo de grabación.
        void OnCierre(object? s, EventArgs e) => contentDialog.Hide();
        dialogPage.ViewModel.CierreSolicitado += OnCierre;

        try
        {
            await contentDialog.ShowAsync();
        }
        finally
        {
            dialogPage.ViewModel.CierreSolicitado -= OnCierre;
        }

        var vm = dialogPage.ViewModel;
        return (vm.Aceptado, vm.FilaInicialResultado, vm.FilaFinalResultado, vm.NumMaxColumnasResultado, vm.SoloSeleccionadas);
    }

    private void BtnCancelar_Click(object sender, RoutedEventArgs e)
    {
        // El form legacy cerraba la ventana (btnCancel). Aquí se navega hacia atrás si es posible.
        if (Frame?.CanGoBack == true) Frame.GoBack();
    }
}
