// Free1X2 · WinUI 3 — WIN3
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Microsoft.UI;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Shapes;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Free1X2.MotorCalculo;
using Free1X2.WinUI.Services;

namespace Free1X2.WinUI.Views.Ported;

// Port de Free1X2.UI.ImprimirBoletoFrm (WinForms) a WinUI 3.
// Form legacy: "Impresión de boletos" — configura margenes, rango de boletos
// y la impresora para volcar columnas de quiniela sobre el boleto fisico.
//
// La impresión real del legacy (System.Drawing.Printing) se sustituye aquí por una
// composición nativa del boleto en un Canvas (mismas marcas 1/X/2 y geometría) que se
// captura con RenderTargetBitmap y se exporta como imagen PNG / al portapapeles.
public sealed partial class ImprimirBoletoFrmPage : Page
{
    // Alto reservado por boleto al apilarlos en vertical en el lienzo de exportación.
    // El legacy imprimía un boleto por página; aquí se apilan con separación generosa
    // (las marcas del legacy llegan hasta ~mgy+350).
    private const double AltoPorBoleto = 380;
    private const double MargenLienzo = 12;

    public ImprimirBoletoFrmViewModel ViewModel { get; } = new();

    public ImprimirBoletoFrmPage()
    {
        InitializeComponent();
        ViewModel.ExportacionSolicitada += async (_, _) => await ExportarBoletoAsync();
        ViewModel.SeleccionImpresoraSolicitada += (_, _) => SeleccionarImpresora();
    }

    /// <summary>
    /// Abre la lista de impresoras conocidas (legacy fiel: btnVerImpresoras_Click ->
    /// new ListaImpresoras(controlador).ShowDialog()). Navega a ListaImpresorasPage por el Frame;
    /// al elegir una impresora, esa página deja el controlador en el handoff estático
    /// ListaImpresorasViewModel.SeleccionResultado y vuelve (Frame.GoBack). OnNavigatedTo recoge el
    /// resultado y copia margenes/modelo/girar al formulario (réplica de la copia que hacía
    /// btnVerImpresoras_Click tras cerrar el diálogo).
    /// </summary>
    private void SeleccionarImpresora()
    {
        ListaImpresorasViewModel.SeleccionResultado = null; // limpiar un resultado previo.
        Frame?.Navigate(typeof(ListaImpresorasPage));
    }

    /// <summary>
    /// Al volver de ListaImpresorasPage (Frame.GoBack), aplica el controlador elegido a los campos
    /// del formulario, igual que el btnVerImpresoras_Click legacy copiaba controlador.* tras cerrar
    /// el diálogo. Si no hubo selección, no toca la configuración actual.
    /// </summary>
    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        base.OnNavigatedTo(e);

        if (ListaImpresorasViewModel.SeleccionResultado is { } controlador)
        {
            ListaImpresorasViewModel.SeleccionResultado = null;
            ViewModel.AplicarImpresora(
                controlador.Modelo,
                controlador.MargenSuperior,
                controlador.MargenIzquierda,
                controlador.Rotar);
        }
    }

    /// <summary>
    /// Dibuja los boletos computados por el VM (BoletosARenderizar) sobre el Canvas, lo captura
    /// con RenderTargetBitmap, lo copia al portapapeles (DataPackage con bitmap) y ofrece guardarlo
    /// como PNG. Equivalente funcional WinUI de Imprimir(PrintPageEventArgs) del legacy, sin
    /// System.Drawing.
    /// </summary>
    private async Task ExportarBoletoAsync()
    {
        if (LienzoBoleto == null || ViewModel.BoletosARenderizar.Count == 0) return;

        try
        {
            DibujarBoletos();
            HostExportacion.UpdateLayout();
            LienzoBoleto.UpdateLayout();

            var rtb = new RenderTargetBitmap();
            await rtb.RenderAsync(LienzoBoleto);
            byte[] pixeles = (await rtb.GetPixelsAsync()).ToArray();
            uint ancho = (uint)rtb.PixelWidth;
            uint alto = (uint)rtb.PixelHeight;

            if (ancho == 0 || alto == 0)
            {
                AppServices.MostrarError("No se ha podido generar la imagen del boleto.");
                return;
            }

            // Codificar PNG en memoria.
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
            await GuardarComoPngAsync(pixeles, ancho, alto, "boleto");
        }
        catch (Exception ex)
        {
            AppServices.MostrarError("No se ha podido exportar el boleto: " + ex.Message);
        }
    }

    /// <summary>
    /// Pinta en el Canvas los boletos computados por el VM, apilados en vertical. Réplica WinUI
    /// del dibujo por página del legacy Imprimir(PrintPageEventArgs): encabezado (fichero + boleto)
    /// + un rectángulo negro por cada marca 1/X/2 y por el pleno al 15.
    /// </summary>
    private void DibujarBoletos()
    {
        LienzoBoleto.Children.Clear();

        var negro = new SolidColorBrush(Colors.Black);
        double maxX = 0;
        int indice = 0;

        foreach (var boleto in ViewModel.BoletosARenderizar)
        {
            double offsetY = MargenLienzo + indice * AltoPorBoleto;

            // Encabezado del boleto (legacy: DrawString del fichero + "Boleto N").
            var cabecera = new TextBlock
            {
                Text = boleto.Encabezado,
                FontFamily = new FontFamily("Consolas"),
                FontSize = 14,
                Foreground = negro,
            };
            Canvas.SetLeft(cabecera, MargenLienzo);
            Canvas.SetTop(cabecera, offsetY);
            LienzoBoleto.Children.Add(cabecera);

            // Marcas 1/X/2 + pleno (legacy: FillRectangle(brush, cx, cy, 6, 4)).
            foreach (var (mx, my, mw, mh) in boleto.Marcas)
            {
                var rect = new Rectangle
                {
                    Width = mw,
                    Height = mh,
                    Fill = negro,
                };
                double left = MargenLienzo + mx;
                double top = offsetY + my;
                Canvas.SetLeft(rect, left);
                Canvas.SetTop(rect, top);
                LienzoBoleto.Children.Add(rect);
                if (left + mw > maxX) maxX = left + mw;
            }

            indice++;
        }

        LienzoBoleto.Width = maxX + MargenLienzo;
        LienzoBoleto.Height = MargenLienzo + indice * AltoPorBoleto;
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
