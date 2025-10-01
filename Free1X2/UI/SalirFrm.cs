using System;
using System.Windows.Forms;

using Free1X2.EntradaSalida;

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
