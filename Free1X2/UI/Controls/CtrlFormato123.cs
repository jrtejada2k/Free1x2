// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2007 Morrison - morrison.ne@gmail.com
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

using System.Drawing;
using System.Windows.Forms;

using Free1X2.MotorCalculo;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI.Controls
{
    public partial class CtrlFormato123 : UserControl
    {
        protected string formato;
        protected string aciertosMin;
        protected string aciertosMax;
        protected string numeroFormato;

        public CtrlFormato123()
        {
            InitializeComponent();
        }
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToControl(this);
        }

        public CtrlFormato123(Formato123 formato123, int noFormato123)
        {
            InitializeComponent();
            txtFormato1.Text = formato123.Formato;
            txtAciertosMin1.Text = formato123.AciertosMin.ToString();
            txtAciertosMax1.Text = formato123.AciertosMax.ToString();
            lblNumeroFormato.Text = noFormato123.ToString();
            
        }

        public string Formato
        {
            get { return txtFormato1.Text;}
            set { txtFormato1.Text = value; }
        }
        public string AciertosMin
        {
            get { return txtAciertosMin1.Text; }
            set { txtAciertosMin1.Text = value; }
        }
        public string AciertosMax
        {
            get { return txtAciertosMax1.Text; }
            set { txtAciertosMax1.Text = value; }
        }
        public string NumeroFormato
        {
            get { return lblNumeroFormato.Text; }
            set { lblNumeroFormato.Text = value; }
        }
        public void DesactivarDiferencias()
        {
            txtAciertosMax1.Enabled = false;
            txtAciertosMax1.BackColor = Color.Red;
            txtAciertosMin1.Enabled = false;
            txtAciertosMin1.BackColor = Color.Red;
        }
        public void ActivarDiferencias()
        {
            txtAciertosMax1.Enabled = true;
            txtAciertosMax1.BackColor = Color.White;
            txtAciertosMin1.Enabled = true;
            txtAciertosMin1.BackColor = Color.White;
        }
        public TextBox TxtFormato
        {
            get { return txtFormato1; }
            set { txtFormato1 = value; }
        }
    }
}
