// Free1X2 · WinUI 3 — WIN3
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Free1X2.WinUI.Services;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// Port WinUI de Free1X2.UI.VerBoletosEnEditorFrm.
    /// Muestra las columnas de apuesta formateadas como boletos verticales
    /// (editor de texto de solo lectura, fuente monoespaciada) y permite imprimirlas/exportarlas.
    /// </summary>
    public sealed partial class VerBoletosEnEditorFrmPage : Page
    {
        public VerBoletosEnEditorFrmViewModel ViewModel { get; } = new VerBoletosEnEditorFrmViewModel();

        public VerBoletosEnEditorFrmPage()
        {
            this.InitializeComponent();
            ViewModel.ImpresionSolicitada += async (_, _) => await ImprimirAsync();

            // Legacy: el constructor de VerBoletosEnEditorFrm recibía string[] columnas y las
            // formateaba. Como esta página se abre por navegación (sin argumento de constructor),
            // las columnas las carga el comando CargarFicheroAsync del ViewModel ("Cargar
            // columnas…"), que lee el fichero y llama a ViewModel.CargarColumnas(columnas).
        }

        /// <summary>
        /// Equivalente funcional WinUI de VerBoletosEnEditorFrm.ImprimirBoletos (que dibujaba
        /// txtBoletos.Lines línea a línea con System.Drawing.Printing). Renderiza el texto COMPLETO
        /// de los boletos (un TextBlock monoespaciado, sin scroll) a un mapa de bits con
        /// RenderTargetBitmap, lo copia al portapapeles (DataPackage con bitmap) y ofrece guardarlo
        /// como PNG. Sin System.Drawing.
        /// </summary>
        private async Task ImprimirAsync()
        {
            string texto = ViewModel.BoletosTexto;
            if (string.IsNullOrEmpty(texto)) return;

            // TextBlock con el texto completo, fuente monoespaciada (legacy txtBoletos: "Lucida Console").
            var bloque = new TextBlock
            {
                Text = texto,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 13,
                Foreground = new SolidColorBrush(Microsoft.UI.Colors.Black),
                TextWrapping = TextWrapping.NoWrap,
                IsTextSelectionEnabled = false,
            };

            try
            {
                // Hospedar el bloque en el host invisible y forzar el layout para que tenga tamaño.
                HostExportacion.Child = bloque;
                HostExportacion.UpdateLayout();
                bloque.UpdateLayout();

                // Render del bloque (no del host con Opacity=0; el render captura píxeles reales).
                var rtb = new RenderTargetBitmap();
                await rtb.RenderAsync(bloque);
                byte[] pixeles = (await rtb.GetPixelsAsync()).ToArray();
                uint ancho = (uint)rtb.PixelWidth;
                uint alto = (uint)rtb.PixelHeight;

                if (ancho == 0 || alto == 0)
                {
                    AppServices.MostrarError("No se ha podido generar la imagen de los boletos.");
                    return;
                }

                // Codificar PNG en memoria a partir de los píxeles BGRA8.
                var stream = new InMemoryRandomAccessStream();
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                encoder.SetPixelData(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Premultiplied,
                    ancho, alto, 96, 96, pixeles);
                await encoder.FlushAsync();

                // Copiar al portapapeles como imagen.
                stream.Seek(0);
                var paquete = new DataPackage();
                paquete.SetBitmap(RandomAccessStreamReference.CreateFromStream(stream));
                Clipboard.SetContent(paquete);

                // Ofrecer guardar como PNG.
                await GuardarComoPngAsync(pixeles, ancho, alto, "boletos");
            }
            catch (Exception ex)
            {
                AppServices.MostrarError("No se ha podido exportar los boletos: " + ex.Message);
            }
            finally
            {
                HostExportacion.Child = null; // liberar el bloque temporal.
            }
        }

        /// <summary>
        /// Guarda los píxeles BGRA8 ya renderizados como un fichero PNG elegido por el usuario.
        /// </summary>
        private async Task GuardarComoPngAsync(byte[] pixeles, uint ancho, uint alto, string nombreSugerido)
        {
            var picker = new FileSavePicker
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                SuggestedFileName = nombreSugerido,
            };
            picker.FileTypeChoices.Add("Imagen PNG", new[] { ".png" });
            WinRT.Interop.InitializeWithWindow.Initialize(picker, AppServices.WindowHandle);

            StorageFile? file = await picker.PickSaveFileAsync();
            if (file == null) return; // el usuario canceló: la imagen ya está en el portapapeles.

            using var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite);
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);
            encoder.SetPixelData(
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Premultiplied,
                ancho, alto, 96, 96, pixeles);
            await encoder.FlushAsync();
        }
    }
}
