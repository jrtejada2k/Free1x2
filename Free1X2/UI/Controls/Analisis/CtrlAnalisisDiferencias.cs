using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Free1X2.Analisis;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI.Controls.Analisis
{
    public partial class CtrlAnalisisDiferencias : UserControl
    {

        public CtrlAnalisisDiferencias(List<ContenedorDiferencias> contenedor, bool esAnalisisExterno)
        {
            InitializeComponent();
            int x = 1;
            int y = 1;
            if (contenedor.Count > 0)
            {
                for (int i = 0; i < contenedor.Count; i++)
                {
                    CtrlAnalisisDiferencias_Individual ctrl = new CtrlAnalisisDiferencias_Individual(contenedor[i], i + 1);
                    ctrl.Location = new Point(x, y);
                    Controls.Add(ctrl);
                    y += ctrl.Height + 1;
                }
            }
            if(esAnalisisExterno)
            {
                //Inhabilitar marcar
            }
        }
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToControl(this);
        }

    }
}
