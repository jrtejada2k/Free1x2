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
    public partial class CtrlAnalisisDistancias : UserControl
    {
        int noCasillas;

        public CtrlAnalisisDistancias(int[,] valores, bool esAnalisisExterno)
        {
            InitializeComponent();
            noCasillas = valores.GetLength(1);
            LlenarNumeros();
            LlenarValores(valores);
            if(esAnalisisExterno)
            {
                //Inhabilitar marcar condición
            }
        }
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToControl(this);
        }

        protected void LlenarValores(int[,] valores)
        {
            //Por cada valor a mostrar hay que insertar una CrtlCasilla inicializada
            LlenarDistVariantes(valores);
            LlenarDistUnos(valores);
            LlenarDistEquis(valores);
            LlenarDistDoses(valores);
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
        protected void LlenarDistVariantes(int[,] valores)
        {
            int x = lblVariantes.Location.X + lblVariantes.Size.Width + 1;
            int y = lblVariantes.Location.Y;

            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[0, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Location = new Point(x, y);
                Controls.Add(casilla);
                x += casilla.Width + 1;
            }

        }
        protected void LlenarDistUnos(int[,] valores)
        {
            int x = lblUnos.Location.X + lblUnos.Size.Width + 1;
            int y = lblUnos.Location.Y;

            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[1, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Location = new Point(x, y);
                Controls.Add(casilla);
                x += casilla.Width + 1;
            }

        }

        protected void LlenarDistEquis(int[,] valores)
        {
            int x = lblEquis.Location.X + lblEquis.Size.Width + 1;
            int y = lblEquis.Location.Y;

            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[2, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Location = new Point(x, y);
                Controls.Add(casilla);
                x += casilla.Width + 1;
            }

        }
        protected void LlenarDistDoses(int[,] valores)
        {
            int x = lblDoses.Location.X + lblDoses.Size.Width + 1;
            int y = lblDoses.Location.Y;

            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[3, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Location = new Point(x, y);
                Controls.Add(casilla);
                x += casilla.Width + 1;
            }

        }
    }
}
