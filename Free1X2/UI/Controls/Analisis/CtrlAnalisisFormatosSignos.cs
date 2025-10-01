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
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Free1X2.UI.Controls.Analisis
{
    public partial class CtrlAnalisisFormatosSignos : UserControl
    {
        SortedList<int,int> noGlobales;
        int[] noLineas;
        public CtrlAnalisisFormatosSignos(SortedList<int, int> global, int[] lineas, bool esAnalisisExterno)
        {
            InitializeComponent();
            noGlobales = global;
            noLineas = lineas;
            LlenarNumerosLineas();
            LlenarNumerosGlobales();
            if(esAnalisisExterno)
            {
                //Inhabilitar marcar
            }
        }
        protected void LlenarNumerosLineas()
        {
            int x = lblNoLineas.Location.X;
            int y = lblNoLineas.Location.Y + lblNoLineas.Size.Height + 1;

            for (int i = 0; i < noLineas.Length; i++)
            {
                CtrlCasilla casilla = new CtrlCasilla(i.ToString());
                CtrlCasilla casilla2 = new CtrlCasilla(noLineas[i].ToString());
                casilla.Location = new Point(x, y);
                casilla2.Location = new Point(x, y + casilla.Size.Height + 1);
                
                casilla.SetColor(Color.NavajoWhite);
                Controls.Add(casilla);
                Controls.Add(casilla2);
                x += casilla.Width + 1;
            }

        }
        protected void LlenarNumerosGlobales()
        {
            int x = lblGlobal.Location.X;
            int y = lblGlobal.Location.Y + lblGlobal.Size.Height + 1;

            for (int i = 0; i < noGlobales.Count; i++)
            {
                CtrlCasilla casilla = new CtrlCasilla(i.ToString());
                CtrlCasilla casilla2 = new CtrlCasilla(noGlobales[noGlobales.Keys[i]].ToString());
                casilla.Location = new Point(x, y);
                casilla2.Location = new Point(x, y + casilla.Size.Height + 1);

                casilla.SetColor(Color.NavajoWhite);
                Controls.Add(casilla);
                Controls.Add(casilla2);
                x += casilla.Width + 1;
            }

        }

        private void btnMarcarCondicion_Click(object sender, EventArgs e)
        {

        }

    }

}
