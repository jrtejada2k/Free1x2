using System.Windows.Forms;

namespace Free1X2.UI.Controls
{
    public partial class VisorCumplimiento : UserControl
    {
        public VisorCumplimiento(string archivo, float cumplimiento)
        {
            InitializeComponent();
            lblArchivo.Text = archivo;
            lblCumplimiento.Text = cumplimiento + " %";
        }
    }
}
