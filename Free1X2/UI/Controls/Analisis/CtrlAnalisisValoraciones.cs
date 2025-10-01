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

using Free1X2.Analisis;

namespace Free1X2.UI.Controls.Analisis
{
    public partial class CtrlAnalisisValoraciones : UserControl
    {
        ContenedorAnalisis contenedor;
        public CtrlAnalisisValoraciones(ContenedorAnalisis c, bool esAnalisisExterno)
        {
            InitializeComponent();
            contenedor = c;
            if (contenedor.UsaValoraciones)
            {
                LlenarValoracionesGlobales();
                LlenarValoracionesUnos();
                LlenarValoracionesEquis();
                LlenarValoracionesDoses();
                PonerTipoValoracion();
            }
            if(esAnalisisExterno)
            {
                //inhabilitar marcar
            }
         }
        protected void LlenarValoracionesGlobales()
        {
            int x = 26;
            int y = 0;
            for (int i = contenedor.NoValoresValoracionGlobal[0]; i < contenedor.ValoracionesGlobales.Count; i++)
            {
                int valor = contenedor.ValoracionesGlobales[i];

                CtrlCasilla casillaValor = new CtrlCasilla(i.ToString());
                CtrlCasilla casillaColumnas = new CtrlCasilla(valor.ToString());

                if (valor != 0)
                {
                    casillaValor.Location = new Point(x, y);
                    casillaColumnas.Location = new Point(casillaValor.Location.X + casillaValor.Width + 1, y);
                    casillaValor.SetColor(Color.NavajoWhite);

                    ContControl.Controls.Add(casillaValor);
                    ContControl.Controls.Add(casillaColumnas);

                    y += casillaValor.Height + 1;
                }
            }
        }
        protected void LlenarValoracionesUnos()
        {
            int x = 159;
            int y = 0;
            for (int i = contenedor.NoValoresValoracionUnos[0]; i < contenedor.ValoracionesUnos.Count; i++)
            {
                int valor = contenedor.ValoracionesUnos[i];

                CtrlCasilla casillaValor = new CtrlCasilla(i.ToString());
                CtrlCasilla casillaColumnas = new CtrlCasilla(valor.ToString());

                if (valor != 0)
                {
                    casillaValor.Location = new Point(x, y);
                    casillaValor.SetColor(Color.NavajoWhite);
                    casillaColumnas.Location = new Point(casillaValor.Location.X + casillaValor.Width + 1, y);

                    ContControl.Controls.Add(casillaValor);
                    ContControl.Controls.Add(casillaColumnas);

                    y += casillaValor.Height + 1;
                }
            }
        }
        protected void LlenarValoracionesEquis()
        {
            int x = 292;
            int y = 0;
            for (int i = contenedor.NoValoresValoracionEquis[0]; i < contenedor.ValoracionesEquis.Count; i++)
            {
                int valor = contenedor.ValoracionesEquis[i];

                CtrlCasilla casillaValor = new CtrlCasilla(i.ToString());
                CtrlCasilla casillaColumnas = new CtrlCasilla(valor.ToString());

                if (valor != 0)
                {
                    casillaValor.Location = new Point(x, y);
                    casillaColumnas.Location = new Point(casillaValor.Location.X + casillaValor.Width + 1, y);
                    casillaValor.SetColor(Color.NavajoWhite);

                    ContControl.Controls.Add(casillaValor);
                    ContControl.Controls.Add(casillaColumnas);

                    y += casillaValor.Height + 1;
                }
            }
        }
        protected void LlenarValoracionesDoses()
        {
            int x = 425;
            int y = 0;
            for (int i = contenedor.NoValoresValoracionDoses[0]; i < contenedor.ValoracionesDoses.Count; i++)
            {
                int valor = contenedor.ValoracionesDoses[i];

                CtrlCasilla casillaValor = new CtrlCasilla(i.ToString());
                CtrlCasilla casillaColumnas = new CtrlCasilla(valor.ToString());

                if (valor != 0)
                {
                    casillaValor.Location = new Point(x, y);
                    casillaColumnas.Location = new Point(casillaValor.Location.X + casillaValor.Width + 1, y);
                    casillaValor.SetColor(Color.NavajoWhite);

                    ContControl.Controls.Add(casillaValor);
                    ContControl.Controls.Add(casillaColumnas);

                    y += casillaValor.Height + 1;
                }
            }
        }
        protected void PonerTipoValoracion()
        {
            switch (contenedor.TipoValoracion)
            {
                case "suma":
                    lblTipo.Text = "Por Sumas";
                    break;
                case "multiplo":
                    lblTipo.Text = "Por Productos";
                    break;
            }
        }

    }
}
