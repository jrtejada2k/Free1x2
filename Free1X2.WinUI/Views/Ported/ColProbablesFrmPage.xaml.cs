using System;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// Page portada del WinForms legacy "ColProbablesFrm" (Free1X2.UI.Filtros.ColProbablesFrm).
    /// Editor del filtro "Columnas Probables" con pestañas Columnas / Relaciones I-III / Control Fallos.
    /// Recibe el Grupo a editar vía AppState.GrupoEnEdicion y escribe los cambios de vuelta
    /// al FiltroColProbables al Aceptar.
    /// </summary>
    public sealed partial class ColProbablesFrmPage : Page
    {
        public ColProbablesFrmViewModel ViewModel { get; } = new ColProbablesFrmViewModel();

        public ColProbablesFrmPage()
        {
            this.InitializeComponent();
            ViewModel.Volver = () => { if (Frame?.CanGoBack == true) Frame.GoBack(); };
            ViewModel.Navegar = (tipo, parametro) => Frame?.Navigate(tipo, parametro);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // Al volver del diálogo "Copiar Datos" (Frame.GoBack), aplicar el rango elegido sobre la
            // copia de trabajo en memoria — NO recargar desde el grupo, que descartaría la edición en
            // curso (réplica de la 2ª mitad de ColProbablesFrm.CopiaValoresCP). AplicarCopiaDatos sólo
            // actúa si esta pantalla había lanzado el diálogo; si no, recarga normal del grupo en edición.
            if (ViewModel.CopiaDatosPendiente)
            {
                ViewModel.AplicarCopiaDatos();
                return;
            }

            // Al volver del "Importador CPs" (Frame.GoBack), fusionar las columnas importadas con la
            // copia de trabajo — NO recargar desde el grupo. Réplica de la 2ª mitad de
            // ColProbablesFrm.ImportaColumnas (líneas 692-745), incluidos los prompts Sí/No de
            // sustituir/añadir, que aquí se muestran como ContentDialog (la página tiene XamlRoot).
            if (ViewModel.ImportacionPendiente)
            {
                _ = AplicarImportacionAsync();
                return;
            }

            ViewModel.CargarDesdeGrupo();
        }

        /// <summary>
        /// Orquesta la fusión tras volver del importador, replicando los prompts del WinForms:
        /// si nº importadas == existentes, pregunta "¿Sustituir...?"; si no, "¿Añadir...?".
        /// Cada decisión llama al método de fusión correspondiente del ViewModel (la lógica de motor
        /// vive en el VM; aquí solo van los diálogos de confirmación).
        /// </summary>
        private async Task AplicarImportacionAsync()
        {
            // Caso sin columnas importadas (cancelado): cierra el flujo y refresca (legacy: nada que fusionar).
            if (ImportadorCPsFrmViewModel.Resultado is null || ImportadorCPsFrmViewModel.Resultado.Count == 0)
            {
                ViewModel.FinalizarImportacion();
                return;
            }

            // legacy líneas 699-702: si coinciden los recuentos, preguntar si SUSTITUIR (manteniendo rangos).
            if (ViewModel.ImportacionRequiereConfirmarSustituir)
            {
                if (await ConfirmarAsync(
                        "Importar columnas",
                        "¿Sustituir las columnas existentes por las importadas (manteniendo los rangos de aciertos y tolerancias)?"))
                {
                    ViewModel.AplicarImportacionSustituir();
                    return;
                }
            }

            // legacy líneas 721-724: si hay existentes, preguntar si AÑADIR al final.
            if (ViewModel.ImportacionRequiereConfirmarAgregar)
            {
                if (await ConfirmarAsync(
                        "Importar columnas",
                        "¿Añadir las columnas importadas al final de las existentes? \r\n(Si se selecciona No, se sustituirán TODAS las columnas y sus rangos por las columnas importadas)"))
                {
                    ViewModel.AplicarImportacionAgregar();
                    return;
                }
            }

            // legacy líneas 733-740 (rama else): reemplazar TODAS las columnas por las importadas.
            ViewModel.AplicarImportacionReemplazar();
        }

        /// <summary>Muestra un ContentDialog Sí/No y devuelve true si el usuario eligió Sí.</summary>
        private async Task<bool> ConfirmarAsync(string titulo, string mensaje)
        {
            var dlg = new ContentDialog
            {
                Title = titulo,
                Content = mensaje,
                PrimaryButtonText = "Sí",
                CloseButtonText = "No",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot,
            };
            return await dlg.ShowAsync() == ContentDialogResult.Primary;
        }
    }
}
