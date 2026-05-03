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

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI.Controls.Analisis
{
    public partial class CtrlCasilla : UserControl
    {
        protected string tipo = "";
        protected int valor;
        
        public CtrlCasilla(string text)
        {
            InitializeComponent();
            lblCasilla.Text = text;
        }
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToControl(this);
        }

        public void SetColor(Color color)
        {
            lblCasilla.BackColor = color;
        }
        public Label Etiqueta
        {
            get { return lblCasilla; }
        }

        public string Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }
        public int Valor
        {
            get { return valor; }
            set { valor = value; }
        }

    }
}
