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

using Free1X2.MotorCalculo;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI.Controls.Analisis
{
    public partial class CtrlAnalisisSSeguidos : UserControl
    {
        int noCasillas;
        List<FiguraCondicion> listaFigurasV;
        List<FiguraCondicion> listaFiguras1;
        List<FiguraCondicion> listaFigurasX;
        List<FiguraCondicion> listaFiguras2;

        FiltroSignosSeguidos filtroSSeguidos;
        public CtrlAnalisisSSeguidos(int[,] valores, List<FiguraCondicion> lstFigurasV, List<FiguraCondicion> lstFiguras1, List<FiguraCondicion> lstFigurasX, List<FiguraCondicion> lstFiguras2, FiltroSignosSeguidos filtro, bool esAnalisisExterno)
        {
            InitializeComponent();
            noCasillas = valores.GetLength(1);
            filtroSSeguidos = filtro;
            LlenarNumeros();
            LlenarValores(valores);
            listaFigurasV = lstFigurasV;
            listaFiguras1 = lstFiguras1;
            listaFigurasX = lstFigurasX;
            listaFiguras2 = lstFiguras2;

            if (VariablesGlobales.AnalizarFigurasV1X2)
            {
                MostrarFigurasV();
                MostrarFiguras1();
                MostrarFigurasX();
                MostrarFiguras2();
            }
            this.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));

            if(esAnalisisExterno)
            {
                //Marcar Condición Invisible
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
            LlenarVariantesSeg(valores);
            LlenarUnosSeg(valores);
            LlenarEquisSeg(valores);
            LlenarDosesSeg(valores);
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
        protected void LlenarVariantesSeg(int[,] valores)
        {
            int x = lblVariantesSeguidas.Location.X + lblVariantesSeguidas.Size.Width + 1;
            int y = lblVariantesSeguidas.Location.Y;

            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[0, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Location = new Point(x, y);
                Controls.Add(casilla);
                x += casilla.Width + 1;
            }

        }
        protected void LlenarEquisSeg(int[,] valores)
        {
            int x = lblEquisSeguidas.Location.X + lblEquisSeguidas.Size.Width + 1;
            int y = lblEquisSeguidas.Location.Y;

            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[2, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Location = new Point(x, y);
                Controls.Add(casilla);
                x += casilla.Width + 1;
            }

        }
        protected void LlenarDosesSeg(int[,] valores)
        {
            int x = lblDosesSeguidos.Location.X + lblDosesSeguidos.Size.Width + 1;
            int y = lblDosesSeguidos.Location.Y;

            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[3, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Location = new Point(x, y);
                Controls.Add(casilla);
                x += casilla.Width + 1;
            }

        }
        protected void LlenarUnosSeg(int[,] valores)
        {
            int x = lblUnosSeguidos.Location.X + lblUnosSeguidos.Size.Width + 1;
            int y = lblUnosSeguidos.Location.Y;

            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[1, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Location = new Point(x, y);
                Controls.Add(casilla);
                x += casilla.Width + 1;
            }

        }
        protected void MostrarFigurasV()
        {
            CtrlDataGridViewFiguras ctrlFig = new CtrlDataGridViewFiguras(listaFigurasV, filtroSSeguidos, "Figuras de Variantes");
            ctrlFig.Location = new Point(0, 100);
            ctrlFig.Name = "ctrlFig";
            Controls.Add(ctrlFig);
        }
        protected void MostrarFiguras1()
        {
            CtrlDataGridViewFiguras ctrlFig = new CtrlDataGridViewFiguras(listaFiguras1, filtroSSeguidos, "Figuras de 1");
            ctrlFig.Location = new Point(0, 320);
            ctrlFig.Name = "ctrlFig";
            Controls.Add(ctrlFig);
        }
        protected void MostrarFigurasX()
        {
            CtrlDataGridViewFiguras ctrlFig = new CtrlDataGridViewFiguras(listaFigurasX, filtroSSeguidos, "Figuras de X");
            ctrlFig.Location = new Point(0, 540);
            ctrlFig.Name = "ctrlFig";
            Controls.Add(ctrlFig);
        }
        protected void MostrarFiguras2()
        {
            CtrlDataGridViewFiguras ctrlFig = new CtrlDataGridViewFiguras(listaFiguras2, filtroSSeguidos, "Figuras de 2");
            ctrlFig.Location = new Point(0, 760);
            ctrlFig.Name = "ctrlFig";
            Controls.Add(ctrlFig);
        }

    }
}
