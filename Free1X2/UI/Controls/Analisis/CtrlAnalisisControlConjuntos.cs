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

namespace Free1X2.UI.Controls.Analisis
{
    public partial class CtrlAnalisisControlConjuntos : UserControl
    {
        int noCasillas;
        public CtrlAnalisisControlConjuntos(int[] valores, bool esAnalisisExterno)
        {
            InitializeComponent();
            noCasillas = valores.Length;
            LlenarNumeros();
            LlenarColumnas(valores);
            if(esAnalisisExterno)
            {
                //inhabilitar marcar
            }
        }
        protected void LlenarNumeros()
        {
            int x = lblNo.Location.X + lblNo.Size.Width + 1;
            int y = lblNo.Location.Y;
            int numero = 0;
            for (int i = 0; i < noCasillas; i++)
            {
                string texto = numero.ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Location = new Point(x, y);
                casilla.SetColor(Color.NavajoWhite);
                Controls.Add(casilla);
                x += casilla.Width + 1;
                numero++;
            }

        }
        protected void LlenarColumnas(int[] valores)
        {
            int x = lblColumnas.Location.X + lblColumnas.Size.Width + 1;
            int y = lblColumnas.Location.Y;

            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Location = new Point(x, y);
                Controls.Add(casilla);
                x += casilla.Width + 1;
            }
        }
    }
}
