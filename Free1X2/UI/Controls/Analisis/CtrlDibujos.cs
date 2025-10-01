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
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

using Free1X2.MotorCalculo;

namespace Free1X2.UI.Controls.Analisis
{
    public partial class CtrlDibujos : UserControl
    {
        int noCasillas;
        FiltroDibujos filtroDibujos;
        ArrayList dib;
        public CtrlDibujos(int[,] valores, FiltroDibujos filtro, bool esAnalisisExterno)
        {
            InitializeComponent();
            noCasillas = valores.GetLength(1);
            dib = new ArrayList();
            filtroDibujos = filtro;
            LlenarNumeros();
            AñadirCasillaPpal();
            LlenarDibujos(valores);

            if(esAnalisisExterno)
            {
                //Marcar Condición Invisible
            }
        }
        protected void AñadirCasillaPpal()
        {
            CtrlCasilla casilla = new CtrlCasilla("X/2");
            casilla.SetColor(Color.NavajoWhite);
            casilla.Location = new Point(0,35); 
            Controls.Add(casilla);
        }
        protected void LlenarNumeros()
        {
            int x2 = 46;
            int y2 = 35;
            int xX = 0;
            int yX = 54;
            int numero = 0;
            for (int i = 0; i < noCasillas; i++)
            {
                string texto = numero.ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                CtrlCasilla casilla2 = new CtrlCasilla(texto);

                casilla.Location = new Point(xX, yX);
                casilla2.Location = new Point(x2, y2);

                casilla.SetColor(Color.NavajoWhite);
                casilla2.SetColor(Color.NavajoWhite);

                Controls.Add(casilla);
                Controls.Add(casilla2);

                yX += casilla.Size.Height +1;
                x2 += 46;
                numero++;
            }

        }
        protected void LlenarDibujos(int[,] valores)
        {
            //Empezamos por X=0
            //Localización de la primera casilla X=0 --> (0,56), "y" se mantiene
            //"x" se incrementa en 46 cada vez
            int x = 0;
            int y = 54;
            for (int i = 0; i < noCasillas; i++)
            {
                for (int j = 0; j < noCasillas; j++)
                {
                    x += 46;
                    string texto = valores[i,j].ToString();
                    CtrlCasilla casilla = new CtrlCasilla(texto);
                    casilla.Location = new Point(x, y);
                    casilla.Etiqueta.Click += lblCasilla_Click;
                    if ((i + j) <= VariablesGlobales.NumeroPartidos)
                    {
                        Controls.Add(casilla);
                    }
                }
                //Bajamos, incrementamos y
                y += 19;
                x = 0;
            }
        }

        private void lblCasilla_Click(object sender, EventArgs e)
        {
            //Obtener la etiqueta
            Label label = (Label)sender;
            //Obtener el control padre
            CtrlCasilla ctrlCas = (CtrlCasilla)label.Parent;

            int doses = ctrlCas.Location.X / 46;
            doses--;
            int equis = (ctrlCas.Location.Y - 54) / 19;

            string dibujo = equis + "+" + doses;
            if (!dib.Contains(dibujo))
            {
                //No esta contenido, marcar y añadir
                dib.Add(dibujo);
                label.BackColor = Color.DarkSalmon;
                label.ForeColor = Color.White;
            }
            else
            {
                //Esta contenido, desmarcar y borrar
                dib.Remove(dibujo);
                label.BackColor = Color.White;
                label.ForeColor = Color.Black;
            }
        }

        private void btnMarcarCondicion_Click(object sender, EventArgs e)
        {
            if (dib.Count > 0)
            {
                filtroDibujos.Dibujos = dib;
                if (!filtroDibujos.IsActive)
                {
                    filtroDibujos.IsActive = true;
                }
                if (!filtroDibujos.ContieneDatos)
                {
                    filtroDibujos.ContieneDatos = (filtroDibujos.Dibujos.Count > 0);
                }
            }

            MessageBox.Show("Los datos se han marcado en la combinación", "Información", MessageBoxButtons.OK);
        }

    }
}
