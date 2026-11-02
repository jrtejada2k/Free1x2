// Free1X2 · WinUI 3 — WIN3
using System;
using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported;

/// <summary>
/// Port WinUI 3 del WinForms <c>TramificarForm</c> ("Tramificar").
/// Reparte el universo de columnas (3^14) en tramos según su valoración, usando los
/// datos de premios de la L.A.E. de una jornada (temporada, jornada, recaudación e
/// importes 14..10), calcula la probabilidad acumulada y permite filtrar por las
/// posiciones mínimas/máximas de cada categoría de premio y localizar columnas
/// concretas. Toda la lógica de dominio (cálculo, escrutinio, persistencia y apertura
/// de otros forms) está implementada en el ViewModel.
/// </summary>
public sealed partial class TramificarFormPage : Page
{
    public TramificarFormViewModel ViewModel { get; } = new();

    public TramificarFormPage()
    {
        this.InitializeComponent();
        ViewModel.Navegar = tipo => Frame?.Navigate(tipo);
        ViewModel.Volver = () => { if (Frame?.CanGoBack == true) Frame.GoBack(); };

        // Flujo "Filtrar" legacy (btFiltrar_Click): el VM calcula los extremos y pide abrir
        // DialogoFiltrarPorLimitesFrm, que en el legacy es MODAL (ShowDialog) sobre Tramificar.
        // Se hospeda la página del diálogo en un ContentDialog (como DialogoGrabarBancoPruebasFrm
        // en BancoPruebasFrmPage) en vez de navegar a ella: al volver con Frame.GoBack() se creaba
        // un TramificarFormPage nuevo, con Ap14T vacío, y el filtro no se aplicaba nunca (además de
        // perder la tramificación y los datos tecleados).
        ViewModel.AbrirDialogoFiltrarPorLimites = extremos =>
            Services.AppServices.EjecutarObservandoErrores(() => FiltrarPorLimitesAsync(extremos), "Filtrar por límites");
    }

    /// <summary>
    /// Muestra «Filtrar por límites» como diálogo modal y, si se acepta (legacy:
    /// if (DialogoFiltrar.ValoresAceptados)), aplica el filtro con los extremos devueltos.
    /// </summary>
    private async System.Threading.Tasks.Task FiltrarPorLimitesAsync(int[,] extremos)
    {
        var dialogPage = new DialogoFiltrarPorLimitesFrmPage();
        dialogPage.ViewModel.Inicializar(extremos);

        var contentDialog = new ContentDialog
        {
            XamlRoot = this.XamlRoot,
            Content = dialogPage,
            RequestedTheme = Services.TemaApp.TemaDeElemento,
        };
        // La tabla del diálogo (4 columnas) no cabe en el ancho máximo por defecto de ContentDialog.
        contentDialog.Resources["ContentDialogMaxWidth"] = 720.0;

        // La VM del diálogo levanta CierreSolicitado al Aceptar/Cancelar (sus botones internos).
        void OnCierre(object? s, EventArgs e) => contentDialog.Hide();
        dialogPage.ViewModel.CierreSolicitado += OnCierre;
        try
        {
            await contentDialog.ShowAsync();
        }
        finally
        {
            dialogPage.ViewModel.CierreSolicitado -= OnCierre;
            // Los botones de la página publican también el handoff estático (pensado para la
            // navegación); aquí no se usa y no debe quedar pendiente.
            DialogoFiltrarPorLimitesFrmPage.Resultado = null;
        }

        if (dialogPage.ViewModel.ValoresAceptados)
        {
            await ViewModel.AplicarFiltroConExtremos(dialogPage.ViewModel.Extremos);
        }
    }
}
