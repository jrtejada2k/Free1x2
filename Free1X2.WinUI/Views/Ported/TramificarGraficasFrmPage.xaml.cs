using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;
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
    /// Page portada desde el WinForms Free1X2.UI.TramificarGraficasFrm.
    /// Visor de gráficas de los resultados del análisis de tramos.
    /// </summary>
    public sealed partial class TramificarGraficasFrmPage : Page
    {
        public TramificarGraficasFrmViewModel ViewModel { get; } = new();

        public TramificarGraficasFrmPage()
        {
            this.InitializeComponent();
            ViewModel.CopiaImagenSolicitada += async (_, _) => await CopiarImagenAsync();
        }

        /// <summary>
        /// Captura el control de la gráfica (GraficoLineasControl) con RenderTargetBitmap y lo
        /// coloca en el portapapeles (DataPackage con stream de bitmap), ofreciendo además
        /// guardarlo como PNG. Equivalente funcional WinUI del Clipboard.SetDataObject(Image)
        /// del legacy CopiarImagenEnClipboard(), sin System.Drawing.
        /// </summary>
        private async Task CopiarImagenAsync()
        {
            if (LienzoGrafica == null) return;

            try
            {
                // 1) Renderizar el control de la gráfica a un mapa de bits en memoria.
                var rtb = new RenderTargetBitmap();
                await rtb.RenderAsync(LienzoGrafica);
                byte[] pixeles = (await rtb.GetPixelsAsync()).ToArray();
                uint ancho = (uint)rtb.PixelWidth;
                uint alto = (uint)rtb.PixelHeight;

                // 2) Codificar un PNG en un stream en memoria a partir de los píxeles BGRA8.
                var stream = new InMemoryRandomAccessStream();
                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                encoder.SetPixelData(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Premultiplied,
                    ancho, alto, 96, 96, pixeles);
                await encoder.FlushAsync();

                // 3) Copiar al portapapeles como imagen (DataPackage con bitmap).
                stream.Seek(0);
                var paquete = new DataPackage();
                paquete.SetBitmap(RandomAccessStreamReference.CreateFromStream(stream));
                Clipboard.SetContent(paquete);

                ViewModel.EstadoTexto = "Imagen de la gráfica copiada al portapapeles.";

                // 4) Ofrecer además guardarla como PNG (FileSavePicker).
                await GuardarComoPngAsync(pixeles, ancho, alto, "grafica_tramos");
            }
            catch (Exception ex)
            {
                AppServices.MostrarError("No se ha podido copiar la imagen de la gráfica: " + ex.Message);
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
            if (file == null) return; // el usuario canceló: ya está en el portapapeles.

            using var fileStream = await file.OpenAsync(FileAccessMode.ReadWrite);
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);
            encoder.SetPixelData(
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Premultiplied,
                ancho, alto, 96, 96, pixeles);
            await encoder.FlushAsync();

            ViewModel.EstadoTexto = "Imagen de la gráfica guardada en " + file.Name + ".";
        }
    }
}
