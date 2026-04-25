using System;
using System.Windows.Forms;

using Free1X2.EntradaSalida;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI
{
    public partial class SalirFrm : Form
    {
        public bool exit = false;
        private AConfiguracion aConf;
        public SalirFrm()
        {
            InitializeComponent();
        }
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
        }


        private void btnCancel_Click(object sender, EventArgs e)
        {
            aConf = new AConfiguracion(Application.StartupPath);
            exit = false;
            aConf.GuardarConfiguracionAdvertenciaSalir(!chbNoMostrar.Checked);
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            aConf = new AConfiguracion(Application.StartupPath);
            exit = true;
            aConf.GuardarConfiguracionAdvertenciaSalir(!chbNoMostrar.Checked);
            this.Close();
        }

    }
}
