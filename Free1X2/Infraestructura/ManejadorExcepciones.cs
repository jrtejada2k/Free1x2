using System;
using System.IO;
using System.Windows.Forms;

namespace Free1X2.Debug
{
    public class ManejadorExcepciones
    {
        public void GuardarInformeErrorATxt(Exception ex, string nombreArchivo)
        {
            try
            {
                string ruta = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Free1X2", "Errores");

                Directory.CreateDirectory(ruta);

                string contenido =
                    $"Fecha: {DateTime.Now}\r\n" +
                    $"Error: {ex.Message}\r\n" +
                    $"Tipo: {ex.GetType().FullName}\r\n\r\n" +
                    $"StackTrace:\r\n{ex.StackTrace}\r\n";

                if (ex.InnerException != null)
                    contenido += $"\r\nCausa: {ex.InnerException.Message}\r\n{ex.InnerException.StackTrace}\r\n";

                File.WriteAllText(Path.Combine(ruta, nombreArchivo), contenido);
            }
            catch { /* no propagar errores en el manejador */ }
        }
    }

    public class InfoError : Form
    {
        public InfoError(string archivoInforme)
        {
            Text            = "Error inesperado — Free1X2";
            Size            = new System.Drawing.Size(480, 220);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            StartPosition   = FormStartPosition.CenterScreen;
            MaximizeBox     = false;
            MinimizeBox     = false;

            string ruta = string.IsNullOrEmpty(archivoInforme) ? string.Empty :
                Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "Free1X2", "Errores", archivoInforme);

            string msg = string.IsNullOrEmpty(archivoInforme)
                ? "Se ha producido un error inesperado. La aplicación debe cerrarse."
                : $"Se ha producido un error inesperado.\r\n\r\nInforme guardado en:\r\n{ruta}";

            var lbl = new Label
            {
                Text      = msg,
                Dock      = DockStyle.Fill,
                Padding   = new Padding(16),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
            };

            var btnOk = new Button
            {
                Text         = "Cerrar",
                DialogResult = DialogResult.OK,
                Dock         = DockStyle.Bottom,
                Height       = 36,
            };

            Controls.Add(lbl);
            Controls.Add(btnOk);
            AcceptButton = btnOk;
        }
    }
}
