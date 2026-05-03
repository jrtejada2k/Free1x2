using System.Windows.Forms;

using Free1X2.UI.Modern.Theming;
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
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToControl(this);
        }

    }
}
