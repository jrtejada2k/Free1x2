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

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Free1X2.Analisis;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI.Controls.Analisis
{
    public partial class CtrlAnalisisSimetrias : UserControl
    {
        List<ContenedorSimetrias> casillas;

        public CtrlAnalisisSimetrias(List<ContenedorSimetrias> contenedor, bool esAnalisisExterno)
        {
            InitializeComponent();
            casillas = contenedor;
            if (contenedor.Count > 0)
            {
                LlenarNumeros();
                LlenarSimetrias();
            }
            else
            {
                lblColumnas.Enabled = false;
                lblNoAciertos.Enabled = false;
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

        protected void LlenarNumeros()
        {
            int x = lblNoAciertos.Location.X + lblNoAciertos.Size.Width + 1;
            int y = lblNoAciertos.Location.Y;
            for (int i = 0; i < casillas.Count; i++)
            {
                ContenedorSimetrias contenedorSim = casillas[i];
                int numero = contenedorSim.Aciertos;
                string texto = numero.ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Location = new Point(x, y);
                casilla.SetColor(Color.NavajoWhite);
                Controls.Add(casilla);
                x += casilla.Width + 1;
            }

        }
        protected void LlenarSimetrias()
        {
            int x = lblColumnas.Location.X + lblColumnas.Size.Width + 1;
            int y = lblColumnas.Location.Y;

            for (int i = 0; i < casillas.Count; i++)
            {
                ContenedorSimetrias contSim = casillas[i];
                string texto = contSim.Columnas.ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Location = new Point(x, y);
                Controls.Add(casilla);
                x += casilla.Width + 1;
            }
        }
    }
}
