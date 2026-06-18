using Microsoft.UI.Xaml.Controls;

namespace Free1X2.WinUI.Views.Ported
{
    /// <summary>
    /// Page WinUI 3 portada desde el WinForms legacy "ImportExportFrm"
    /// (UI/ImportExportFrm.cs): conversion de ficheros de columnas CSV <-> TXT.
    /// </summary>
    public sealed partial class ImportExportFrmPage : Page
    {
        public ImportExportFrmViewModel ViewModel { get; } = new ImportExportFrmViewModel();

        public ImportExportFrmPage()
        {
            this.InitializeComponent();
        }
    }
}
