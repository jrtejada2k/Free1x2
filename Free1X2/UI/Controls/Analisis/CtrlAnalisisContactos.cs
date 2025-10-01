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

using Free1X2.MotorCalculo;

namespace Free1X2.UI.Controls.Analisis
{       
    
    public partial class CtrlAnalisisContactos : UserControl
    {
        int noCasillas;
        List<FiguraCondicion> listaFiguras;
        FiltroContactos filtroContactos;
        #region Variables que almacenan los contactos marcados
        List<int> valores1X = new List<int>();
        List<int> valores12 = new List<int>();
        List<int> valoresX2 = new List<int>();
        List<int> valores11 = new List<int>();
        List<int> valoresXX = new List<int>();
        List<int> valores22 = new List<int>();
        List<int> valores1V = new List<int>();
        List<int> valoresXV = new List<int>();
        List<int> valores2V = new List<int>();
        List<int> valoresVV = new List<int>(); 
        #endregion
        #region Variables que almacenan las columnas seleccionadas
        int columnas1X;
        int columnas12;
        int columnasX2;

        int columnas11;
        int columnasXX;
        int columnas22;

        int columnas1V;
        int columnasXV;
        int columnas2V;
        int columnasVV; 
        #endregion

        public CtrlAnalisisContactos(int[,] valores, List<FiguraCondicion> lstFiguras, FiltroContactos filtro, bool esAnalisisExterno)
        {
            InitializeComponent();
            noCasillas = valores.GetLength(1);
            filtroContactos = filtro;
            LlenarNumeros();
            LlenarValores(valores);
            listaFiguras = lstFiguras;
            if (VariablesGlobales.AnalizarFigurasContactos)
            {
                MostrarFiguras();
            }
            if(esAnalisisExterno)
            {
                btnMarcarCondicion.Enabled = false;
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
        protected void LlenarValores(int[,] valores)
        {
            //Por cada valor a mostrar hay que insertar una CrtlCasilla inicializada
            Llenar1X(valores);
            Llenar12(valores);
            LlenarX2(valores);
            Llenar11(valores);
            LlenarXX(valores);
            Llenar22(valores);
            Llenar1V(valores);
            LlenarXV(valores);
            Llenar2V(valores);
            LlenarVV(valores);
        }
        protected void Llenar1X(int[,] valores)
        {
            int x = lbl1X.Location.X + lbl1X.Size.Width + 1;
            int y = lbl1X.Location.Y;
            int valor = 0;
            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[0, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Tipo = "1X";
                casilla.Valor = valor;
                casilla.Location = new Point(x, y);
                casilla.Etiqueta.Click += lblCasilla1X_Click;
                Controls.Add(casilla);
                x += casilla.Width + 1;
                valor++;
            }
            //Añadir un control para la suma
            CtrlCasilla cCasillaSuma = new CtrlCasilla("");
            cCasillaSuma.Location = new Point(x, y);
            cCasillaSuma.Name = "Suma1X";
            cCasillaSuma.SetColor(Color.NavajoWhite);
            Controls.Add(cCasillaSuma);

        }
        protected void Llenar12(int[,] valores)
        {
            int x = lbl12.Location.X + lbl12.Size.Width + 1;
            int y = lbl12.Location.Y;
            int valor = 0;
            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[1, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Tipo = "12";
                casilla.Valor = valor;
                casilla.Location = new Point(x, y);
                casilla.Etiqueta.Click += lblCasilla12_Click;
                Controls.Add(casilla);
                x += casilla.Width + 1;
                valor++;
            }
            //Añadir un control para la suma
            CtrlCasilla cCasillaSuma = new CtrlCasilla("");
            cCasillaSuma.Location = new Point(x, y);
            cCasillaSuma.Name = "Suma12";
            cCasillaSuma.SetColor(Color.NavajoWhite);
            Controls.Add(cCasillaSuma);
        }
        protected void LlenarX2(int[,] valores)
        {
            int x = lblX2.Location.X + lblX2.Size.Width + 1;
            int y = lblX2.Location.Y;
            int valor = 0;
            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[2, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Tipo = "X2";
                casilla.Valor = valor;
                casilla.Location = new Point(x, y);
                casilla.Etiqueta.Click += lblCasillaX2_Click;
                Controls.Add(casilla);
                x += casilla.Width + 1;
                valor++;
            }
            //Añadir un control para la suma
            CtrlCasilla cCasillaSuma = new CtrlCasilla("");
            cCasillaSuma.Location = new Point(x, y);
            cCasillaSuma.Name = "SumaX2";
            cCasillaSuma.SetColor(Color.NavajoWhite);
            Controls.Add(cCasillaSuma);
        }
        protected void Llenar11(int[,] valores)
        {
            int x = lbl11.Location.X + lbl11.Size.Width + 1;
            int y = lbl11.Location.Y;
            int valor = 0;
            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[3, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Tipo = "11";
                casilla.Valor = valor;
                casilla.Location = new Point(x, y);
                casilla.Etiqueta.Click += lblCasilla11_Click;
                Controls.Add(casilla);
                x += casilla.Width + 1;
                valor++;
            }
            //Añadir un control para la suma
            CtrlCasilla cCasillaSuma = new CtrlCasilla("");
            cCasillaSuma.Location = new Point(x, y);
            cCasillaSuma.Name = "Suma11";
            cCasillaSuma.SetColor(Color.NavajoWhite);
            Controls.Add(cCasillaSuma);
        }
        protected void LlenarXX(int[,] valores)
        {
            int x = lblXX.Location.X + lblXX.Size.Width + 1;
            int y = lblXX.Location.Y;
            int valor = 0;
            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[4, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Location = new Point(x, y);
                casilla.Tipo = "XX";
                casilla.Valor = valor;
                casilla.Location = new Point(x, y);
                casilla.Etiqueta.Click += lblCasillaXX_Click;
                Controls.Add(casilla);
                x += casilla.Width + 1;
                valor++;
            }
            //Añadir un control para la suma
            CtrlCasilla cCasillaSuma = new CtrlCasilla("");
            cCasillaSuma.Location = new Point(x, y);
            cCasillaSuma.Name = "SumaXX";
            cCasillaSuma.SetColor(Color.NavajoWhite);
            Controls.Add(cCasillaSuma);
        }
        protected void Llenar22(int[,] valores)
        {
            int x = lbl22.Location.X + lbl22.Size.Width + 1;
            int y = lbl22.Location.Y;
            int valor = 0;
            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[5, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Tipo = "22";
                casilla.Valor = valor;
                casilla.Location = new Point(x, y);
                casilla.Etiqueta.Click += lblCasilla22_Click;
                Controls.Add(casilla);
                x += casilla.Width + 1;
                valor++;
            }
            //Añadir un control para la suma
            CtrlCasilla cCasillaSuma = new CtrlCasilla("");
            cCasillaSuma.Location = new Point(x, y);
            cCasillaSuma.Name = "Suma22";
            cCasillaSuma.SetColor(Color.NavajoWhite);
            Controls.Add(cCasillaSuma);
        }
        protected void Llenar1V(int[,] valores)
        {
            int x = lbl1V.Location.X + lbl1V.Size.Width + 1;
            int y = lbl1V.Location.Y;
            int valor = 0;
            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[6, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Location = new Point(x, y);
                casilla.Tipo = "1V";
                casilla.Valor = valor;
                casilla.Location = new Point(x, y);
                casilla.Etiqueta.Click += lblCasilla1V_Click;
                Controls.Add(casilla);
                x += casilla.Width + 1;
                valor++;
            }
            //Añadir un control para la suma
            CtrlCasilla cCasillaSuma = new CtrlCasilla("");
            cCasillaSuma.Location = new Point(x, y);
            cCasillaSuma.Name = "Suma1V";
            cCasillaSuma.SetColor(Color.NavajoWhite);
            Controls.Add(cCasillaSuma);
        }
        protected void LlenarXV(int[,] valores)
        {
            int x = lblXV.Location.X + lblXV.Size.Width + 1;
            int y = lblXV.Location.Y;
            int valor = 0;
            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[7, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Location = new Point(x, y);
                casilla.Tipo = "XV";
                casilla.Valor = valor;
                casilla.Location = new Point(x, y);
                casilla.Etiqueta.Click += lblCasillaXV_Click;
                Controls.Add(casilla);
                x += casilla.Width + 1;
                valor++;
            }
            //Añadir un control para la suma
            CtrlCasilla cCasillaSuma = new CtrlCasilla("");
            cCasillaSuma.Location = new Point(x, y);
            cCasillaSuma.Name = "SumaXV";
            cCasillaSuma.SetColor(Color.NavajoWhite);
            Controls.Add(cCasillaSuma);
        }
        protected void Llenar2V(int[,] valores)
        {
            int x = lbl2V.Location.X + lbl2V.Size.Width + 1;
            int y = lbl2V.Location.Y;
            int valor = 0;
            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[8, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Tipo = "2V";
                casilla.Valor = valor;
                casilla.Location = new Point(x, y);
                casilla.Etiqueta.Click += lblCasilla2V_Click;
                Controls.Add(casilla);
                x += casilla.Width + 1;
                valor++;
            }
            //Añadir un control para la suma
            CtrlCasilla cCasillaSuma = new CtrlCasilla("");
            cCasillaSuma.Location = new Point(x, y);
            cCasillaSuma.Name = "Suma2V";
            cCasillaSuma.SetColor(Color.NavajoWhite);
            Controls.Add(cCasillaSuma);
        }
        protected void LlenarVV(int[,] valores)
        {
            int x = lblVV.Location.X + lblVV.Size.Width + 1;
            int y = lblVV.Location.Y;
            int valor = 0;
            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[9, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Tipo = "VV";
                casilla.Valor = valor;
                casilla.Location = new Point(x, y);
                casilla.Etiqueta.Click += lblCasillaVV_Click;
                Controls.Add(casilla);
                x += casilla.Width + 1;
                valor++;
            }
            //Añadir un control para la suma
            CtrlCasilla cCasillaSuma = new CtrlCasilla("");
            cCasillaSuma.Location = new Point(x, y);
            cCasillaSuma.Name = "SumaVV";
            cCasillaSuma.SetColor(Color.NavajoWhite);
            Controls.Add(cCasillaSuma);
        }
        protected void MostrarFiguras()
        {
            CtrlDataGridViewFiguras ctrlFig = new CtrlDataGridViewFiguras(listaFiguras, filtroContactos);
            ctrlFig.Location = new Point(0, 220);
            ctrlFig.Name = "ctrlFig";
            Controls.Add(ctrlFig);
        }

        #region EventosCasillas
        private void lblCasilla1X_Click(object sender, EventArgs e)
        {
            //Obtener la etiqueta
            Label label = (Label)sender;
            //Obtener el control padre
            CtrlCasilla ctrlCas = (CtrlCasilla)label.Parent;
            int valor = ctrlCas.Valor;
            if (!valores1X.Contains(valor))
            {
                //No esta contenido, marcar y añadir
                valores1X.Add(valor);
                label.BackColor = Color.DarkSalmon;
                label.ForeColor = Color.White;
                columnas1X += Convert.ToInt32(label.Text);
            }
            else
            {
                //Esta contenido, desmarcar y borrar
                valores1X.Remove(valor);
                label.BackColor = Color.White;
                label.ForeColor = Color.Black;
                columnas1X -= Convert.ToInt32(label.Text);
            }
            Control[] controles = Controls.Find("Suma1X", false);
            CtrlCasilla casTemp = (CtrlCasilla)controles[0];
            casTemp.Etiqueta.Text = columnas1X.ToString();
        }
        private void lblCasilla12_Click(object sender, EventArgs e)
        {
            //Obtener la etiqueta
            Label label = (Label)sender;
            //Obtener el control padre
            CtrlCasilla ctrlCas = (CtrlCasilla)label.Parent;
            int valor = ctrlCas.Valor;
            if (!valores12.Contains(valor))
            {
                //No esta contenido, marcar y añadir
                valores12.Add(valor);
                label.BackColor = Color.DarkSalmon;
                label.ForeColor = Color.White;
                columnas12 += Convert.ToInt32(label.Text);
            }
            else
            {
                //Esta contenido, desmarcar y borrar
                valores12.Remove(valor);
                label.BackColor = Color.White;
                label.ForeColor = Color.Black;
                columnas12 -= Convert.ToInt32(label.Text);
            }
            Control[] controles = Controls.Find("Suma12", false);
            CtrlCasilla casTemp = (CtrlCasilla)controles[0];
            casTemp.Etiqueta.Text = columnas12.ToString();
        }
        private void lblCasillaX2_Click(object sender, EventArgs e)
        {
            //Obtener la etiqueta
            Label label = (Label)sender;
            //Obtener el control padre
            CtrlCasilla ctrlCas = (CtrlCasilla)label.Parent;

            int valor = ctrlCas.Valor;
            if (!valoresX2.Contains(valor))
            {
                //No esta contenido, marcar y añadir
                valoresX2.Add(valor);
                label.BackColor = Color.DarkSalmon;
                label.ForeColor = Color.White;
                columnasX2 += Convert.ToInt32(label.Text);
            }
            else
            {
                //Esta contenido, desmarcar y borrar
                valoresX2.Remove(valor);
                label.BackColor = Color.White;
                label.ForeColor = Color.Black;
                columnasX2 -= Convert.ToInt32(label.Text);
            }
            Control[] controles = Controls.Find("SumaX2", false);
            CtrlCasilla casTemp = (CtrlCasilla)controles[0];
            casTemp.Etiqueta.Text = columnasX2.ToString();
        }
        private void lblCasilla11_Click(object sender, EventArgs e)
        {
            //Obtener la etiqueta
            Label label = (Label)sender;
            //Obtener el control padre
            CtrlCasilla ctrlCas = (CtrlCasilla)label.Parent;
            int valor = ctrlCas.Valor;
            if (!valores11.Contains(valor))
            {
                //No esta contenido, marcar y añadir
                valores11.Add(valor);
                label.BackColor = Color.DarkSalmon;
                label.ForeColor = Color.White;
                columnas11 += Convert.ToInt32(label.Text);
            }
            else
            {
                //Esta contenido, desmarcar y borrar
                valores11.Remove(valor);
                label.BackColor = Color.White;
                label.ForeColor = Color.Black;
                columnas11 -= Convert.ToInt32(label.Text);
            }
            Control[] controles = Controls.Find("Suma11", false);
            CtrlCasilla casTemp = (CtrlCasilla)controles[0];
            casTemp.Etiqueta.Text = columnas11.ToString();
        }
        private void lblCasillaXX_Click(object sender, EventArgs e)
        {
            //Obtener la etiqueta
            Label label = (Label)sender;
            //Obtener el control padre
            CtrlCasilla ctrlCas = (CtrlCasilla)label.Parent;
            int valor = ctrlCas.Valor;
            if (!valoresXX.Contains(valor))
            {
                //No esta contenido, marcar y añadir
                valoresXX.Add(valor);
                label.BackColor = Color.DarkSalmon;
                label.ForeColor = Color.White;
                columnasXX += Convert.ToInt32(label.Text);

            }
            else
            {
                //Esta contenido, desmarcar y borrar
                valoresXX.Remove(valor);
                label.BackColor = Color.White;
                label.ForeColor = Color.Black;
                columnasXX -= Convert.ToInt32(label.Text);

            }
            Control[] controles = Controls.Find("SumaXX", false);
            CtrlCasilla casTemp = (CtrlCasilla)controles[0];
            casTemp.Etiqueta.Text = columnasXX.ToString();
        }
        private void lblCasilla22_Click(object sender, EventArgs e)
        {
            //Obtener la etiqueta
            Label label = (Label)sender;
            //Obtener el control padre
            CtrlCasilla ctrlCas = (CtrlCasilla)label.Parent;
            int valor = ctrlCas.Valor;
            if (!valores22.Contains(valor))
            {
                //No esta contenido, marcar y añadir
                valores22.Add(valor);
                label.BackColor = Color.DarkSalmon;
                label.ForeColor = Color.White;
                columnas22 += Convert.ToInt32(label.Text);

            }
            else
            {
                //Esta contenido, desmarcar y borrar
                valores22.Remove(valor);
                label.BackColor = Color.White;
                label.ForeColor = Color.Black;
                columnas22 -= Convert.ToInt32(label.Text);
            }
            Control[] controles = Controls.Find("Suma22", false);
            CtrlCasilla casTemp = (CtrlCasilla)controles[0];
            casTemp.Etiqueta.Text = columnas22.ToString();
        }
        private void lblCasilla1V_Click(object sender, EventArgs e)
        {
            //Obtener la etiqueta
            Label label = (Label)sender;
            //Obtener el control padre
            CtrlCasilla ctrlCas = (CtrlCasilla)label.Parent;
            int valor = ctrlCas.Valor;
            if (!valores1V.Contains(valor))
            {
                //No esta contenido, marcar y añadir
                valores1V.Add(valor);
                label.BackColor = Color.DarkSalmon;
                label.ForeColor = Color.White;
                columnas1V += Convert.ToInt32(label.Text);
            }
            else
            {
                //Esta contenido, desmarcar y borrar
                valores1V.Remove(valor);
                label.BackColor = Color.White;
                label.ForeColor = Color.Black;
                columnas1V -= Convert.ToInt32(label.Text);
            }
            Control[] controles = Controls.Find("Suma1V", false);
            CtrlCasilla casTemp = (CtrlCasilla)controles[0];
            casTemp.Etiqueta.Text = columnas1V.ToString();
        }
        private void lblCasillaXV_Click(object sender, EventArgs e)
        {
            //Obtener la etiqueta
            Label label = (Label)sender;
            //Obtener el control padre
            CtrlCasilla ctrlCas = (CtrlCasilla)label.Parent;
            int valor = ctrlCas.Valor;
            if (!valoresXV.Contains(valor))
            {
                //No esta contenido, marcar y añadir
                valoresXV.Add(valor);
                label.BackColor = Color.DarkSalmon;
                label.ForeColor = Color.White;
                columnasXV += Convert.ToInt32(label.Text);
            }
            else
            {
                //Esta contenido, desmarcar y borrar
                valoresXV.Remove(valor);
                label.BackColor = Color.White;
                label.ForeColor = Color.Black;
                columnasXV -= Convert.ToInt32(label.Text);
            }
            Control[] controles = Controls.Find("SumaXV", false);
            CtrlCasilla casTemp = (CtrlCasilla)controles[0];
            casTemp.Etiqueta.Text = columnasXV.ToString();
        }
        private void lblCasilla2V_Click(object sender, EventArgs e)
        {
            //Obtener la etiqueta
            Label label = (Label)sender;
            //Obtener el control padre
            CtrlCasilla ctrlCas = (CtrlCasilla)label.Parent;
            int valor = ctrlCas.Valor;
            if (!valores2V.Contains(valor))
            {
                //No esta contenido, marcar y añadir
                valores2V.Add(valor);
                label.BackColor = Color.DarkSalmon;
                label.ForeColor = Color.White;
                columnas2V += Convert.ToInt32(label.Text);
            }
            else
            {
                //Esta contenido, desmarcar y borrar
                valores2V.Remove(valor);
                label.BackColor = Color.White;
                label.ForeColor = Color.Black;
                columnas2V -= Convert.ToInt32(label.Text);
            }
            Control[] controles = Controls.Find("Suma2V", false);
            CtrlCasilla casTemp = (CtrlCasilla)controles[0];
            casTemp.Etiqueta.Text = columnas2V.ToString();
        }
        private void lblCasillaVV_Click(object sender, EventArgs e)
        {
            //Obtener la etiqueta
            Label label = (Label)sender;
            //Obtener el control padre
            CtrlCasilla ctrlCas = (CtrlCasilla)label.Parent;
            int valor = ctrlCas.Valor;
            if (!valoresVV.Contains(valor))
            {
                //No esta contenido, marcar y añadir
                valoresVV.Add(valor);
                label.BackColor = Color.DarkSalmon;
                label.ForeColor = Color.White;
                columnasVV += Convert.ToInt32(label.Text);
            }
            else
            {
                //Esta contenido, desmarcar y borrar
                valoresVV.Remove(valor);
                label.BackColor = Color.White;
                label.ForeColor = Color.Black;
                columnasVV -= Convert.ToInt32(label.Text);
            }
            Control[] controles = Controls.Find("SumaVV", false);
            CtrlCasilla casTemp = (CtrlCasilla)controles[0];
            casTemp.Etiqueta.Text = columnasVV.ToString();
        } 
        #endregion

        protected void MarcarFila(string tipo)
        {
            CtrlCasilla casillaTemp = new CtrlCasilla("");

            for (int i = 0; i < Controls.Count; i++)
            {
                if (Controls[i].GetType() == casillaTemp.GetType())
                {
                    CtrlCasilla cas = (CtrlCasilla)Controls[i];
                    if (cas.Tipo == tipo)
                    {
                        switch (tipo)
                        {
                            case "1X":
                                if (!valores1X.Contains(cas.Valor))
                                {
                                    valores1X.Add(cas.Valor);
                                    lbl1X.BackColor = Color.DarkKhaki;
                                    columnas1X += Convert.ToInt32(cas.Etiqueta.Text);

                                    Control[] controles = Controls.Find("Suma1X", false);
                                    CtrlCasilla casTemp = (CtrlCasilla)controles[0];
                                    casTemp.Etiqueta.Text = columnas1X.ToString();
                                }
                                break;
                            case "12":
                                if (!valores12.Contains(cas.Valor))
                                {
                                    valores12.Add(cas.Valor);
                                    lbl12.BackColor = Color.DarkKhaki;
                                    columnas12 += Convert.ToInt32(cas.Etiqueta.Text);

                                    Control[] controles = Controls.Find("Suma12", false);
                                    CtrlCasilla casTemp = (CtrlCasilla)controles[0];
                                    casTemp.Etiqueta.Text = columnas12.ToString();
                                }
                                break;
                            case "X2":
                                if (!valoresX2.Contains(cas.Valor))
                                {
                                    valoresX2.Add(cas.Valor);
                                    lblX2.BackColor = Color.DarkKhaki;
                                    columnasX2 += Convert.ToInt32(cas.Etiqueta.Text);
                                    Control[] controles = Controls.Find("SumaX2", false);
                                    CtrlCasilla casTemp = (CtrlCasilla)controles[0];
                                    casTemp.Etiqueta.Text = columnasX2.ToString();
                                }
                                break;
                            case "11":
                                if (!valores11.Contains(cas.Valor))
                                {
                                    valores11.Add(cas.Valor);
                                    lbl11.BackColor = Color.DarkKhaki;
                                    columnas11 += Convert.ToInt32(cas.Etiqueta.Text);

                                    Control[] controles = Controls.Find("Suma11", false);
                                    CtrlCasilla casTemp = (CtrlCasilla)controles[0];
                                    casTemp.Etiqueta.Text = columnas11.ToString();
                                }
                                break;
                            case "XX":
                                if (!valoresXX.Contains(cas.Valor))
                                {
                                    valoresXX.Add(cas.Valor);
                                    lblXX.BackColor = Color.DarkKhaki;
                                    columnasXX += Convert.ToInt32(cas.Etiqueta.Text);

                                    Control[] controles = Controls.Find("SumaXX", false);
                                    CtrlCasilla casTemp = (CtrlCasilla)controles[0];
                                    casTemp.Etiqueta.Text = columnasXX.ToString();
                                }
                                break;
                            case "22":
                                if (!valores22.Contains(cas.Valor))
                                {
                                    valores22.Add(cas.Valor);
                                    lbl22.BackColor = Color.DarkKhaki;
                                    columnas22 += Convert.ToInt32(cas.Etiqueta.Text);

                                    Control[] controles = Controls.Find("Suma22", false);
                                    CtrlCasilla casTemp = (CtrlCasilla)controles[0];
                                    casTemp.Etiqueta.Text = columnas22.ToString();
                                }
                                break;
                            case "1V":
                                if (!valores1V.Contains(cas.Valor))
                                {
                                    valores1V.Add(cas.Valor);
                                    lbl1V.BackColor = Color.DarkKhaki;
                                    columnas1V += Convert.ToInt32(cas.Etiqueta.Text);

                                    Control[] controles = Controls.Find("Suma1V", false);
                                    CtrlCasilla casTemp = (CtrlCasilla)controles[0];
                                    casTemp.Etiqueta.Text = columnas1V.ToString();
                                }
                                break;
                            case "XV":
                                if (!valoresXV.Contains(cas.Valor))
                                {
                                    valoresXV.Add(cas.Valor);
                                    lblXV.BackColor = Color.DarkKhaki;
                                    columnasXV += Convert.ToInt32(cas.Etiqueta.Text);

                                    Control[] controles = Controls.Find("SumaXV", false);
                                    CtrlCasilla casTemp = (CtrlCasilla)controles[0];
                                    casTemp.Etiqueta.Text = columnasXV.ToString();
                                }
                                break;
                            case "2V":
                                if (!valores2V.Contains(cas.Valor))
                                {
                                    valores2V.Add(cas.Valor);
                                    lbl2V.BackColor = Color.DarkKhaki;
                                    columnas2V += Convert.ToInt32(cas.Etiqueta.Text);

                                    Control[] controles = Controls.Find("Suma2V", false);
                                    CtrlCasilla casTemp = (CtrlCasilla)controles[0];
                                    casTemp.Etiqueta.Text = columnas2V.ToString();
                                }
                                break;
                            case "VV":
                                if (!valoresVV.Contains(cas.Valor))
                                {
                                    valoresVV.Add(cas.Valor);
                                    lblVV.BackColor = Color.DarkKhaki;
                                    columnasVV += Convert.ToInt32(cas.Etiqueta.Text);

                                    Control[] controles = Controls.Find("SumaVV", false);
                                    CtrlCasilla casTemp = (CtrlCasilla)controles[0];
                                    casTemp.Etiqueta.Text = columnasVV.ToString();
                                }
                                break;
                        }
                        cas.Etiqueta.BackColor = Color.DarkSalmon;
                        cas.Etiqueta.ForeColor = Color.White;
                    }
                }
            }
        }
        protected void DesmarcarFila(string tipo)
        {
            CtrlCasilla casillaTemp = new CtrlCasilla("");
            for (int i = 0; i < Controls.Count; i++)
            {
                if (Controls[i].GetType() == casillaTemp.GetType())
                {
                    CtrlCasilla cas = (CtrlCasilla)Controls[i];
                    if (cas.Tipo == tipo)
                    {
                        switch (tipo)
                        {
                            case "1X":
                                if (valores1X.Contains(cas.Valor))
                                {
                                    valores1X.Remove(cas.Valor);
                                    lbl1X.BackColor = Color.NavajoWhite;
                                    columnas1X -= Convert.ToInt32(cas.Etiqueta.Text);

                                    Control[] controles = Controls.Find("Suma1X", false);
                                    CtrlCasilla casTemp = (CtrlCasilla)controles[0];
                                    casTemp.Etiqueta.Text = columnas1X.ToString();
                                }
                                break;
                            case "12":
                                if (valores12.Contains(cas.Valor))
                                {
                                    valores12.Remove(cas.Valor); 
                                    lbl12.BackColor = Color.NavajoWhite;
                                    columnas12 -= Convert.ToInt32(cas.Etiqueta.Text);

                                    Control[] controles = Controls.Find("Suma12", false);
                                    CtrlCasilla casTemp = (CtrlCasilla)controles[0];
                                    casTemp.Etiqueta.Text = columnas12.ToString();
                                }
                                break;
                            case "X2":
                                if (valoresX2.Contains(cas.Valor))
                                {
                                    valoresX2.Remove(cas.Valor);
                                    lblX2.BackColor = Color.NavajoWhite;
                                    columnasX2 -= Convert.ToInt32(cas.Etiqueta.Text);
                                    Control[] controles = Controls.Find("SumaX2", false);
                                    CtrlCasilla casTemp = (CtrlCasilla)controles[0];
                                    casTemp.Etiqueta.Text = columnasX2.ToString();
                                }
                                break;
                            case "11":
                                if (valores11.Contains(cas.Valor))
                                {
                                    valores11.Remove(cas.Valor);
                                    lbl11.BackColor = Color.NavajoWhite;
                                    columnas11 -= Convert.ToInt32(cas.Etiqueta.Text);
                                    Control[] controles = Controls.Find("Suma11", false);
                                    CtrlCasilla casTemp = (CtrlCasilla)controles[0];
                                    casTemp.Etiqueta.Text = columnas11.ToString();
                                }
                                break;
                            case "XX":
                                if (valoresXX.Contains(cas.Valor))
                                {
                                    valoresXX.Remove(cas.Valor);
                                    lblXX.BackColor = Color.NavajoWhite;
                                    columnasXX -= Convert.ToInt32(cas.Etiqueta.Text);
                                    Control[] controles = Controls.Find("SumaXX", false);
                                    CtrlCasilla casTemp = (CtrlCasilla)controles[0];
                                    casTemp.Etiqueta.Text = columnasXX.ToString();
                                }
                                break;
                            case "22":
                                if (valores22.Contains(cas.Valor))
                                {
                                    valores22.Remove(cas.Valor);
                                    lbl22.BackColor = Color.NavajoWhite;
                                    columnas22 -= Convert.ToInt32(cas.Etiqueta.Text);
                                    Control[] controles = Controls.Find("Suma22", false);
                                    CtrlCasilla casTemp = (CtrlCasilla)controles[0];
                                    casTemp.Etiqueta.Text = columnas22.ToString();
                                }
                                break;
                            case "1V":
                                if (valores1V.Contains(cas.Valor))
                                {
                                    valores1V.Remove(cas.Valor);
                                    lbl1V.BackColor = Color.NavajoWhite;
                                    columnas1V -= Convert.ToInt32(cas.Etiqueta.Text);
                                    Control[] controles = Controls.Find("Suma1V", false);
                                    CtrlCasilla casTemp = (CtrlCasilla)controles[0];
                                    casTemp.Etiqueta.Text = columnas1V.ToString();
                                }
                                break;
                            case "XV":
                                if (valoresXV.Contains(cas.Valor))
                                {
                                    valoresXV.Remove(cas.Valor);
                                    lblXV.BackColor = Color.NavajoWhite;
                                    columnasXV -= Convert.ToInt32(cas.Etiqueta.Text);
                                    Control[] controles = Controls.Find("SumaXV", false);
                                    CtrlCasilla casTemp = (CtrlCasilla)controles[0];
                                    casTemp.Etiqueta.Text = columnasXV.ToString();
                                }
                                break;
                            case "2V":
                                if (valores2V.Contains(cas.Valor))
                                {
                                    valores2V.Remove(cas.Valor);
                                    lbl2V.BackColor = Color.NavajoWhite;
                                    columnas2V -= Convert.ToInt32(cas.Etiqueta.Text);
                                    Control[] controles = Controls.Find("Suma2V", false);
                                    CtrlCasilla casTemp = (CtrlCasilla)controles[0];
                                    casTemp.Etiqueta.Text = columnas2V.ToString();
                                }
                                break;
                            case "VV":
                                if (valoresVV.Contains(cas.Valor))
                                {
                                    valoresVV.Remove(cas.Valor);
                                    lblVV.BackColor = Color.NavajoWhite;
                                    columnasVV -= Convert.ToInt32(cas.Etiqueta.Text);
                                    Control[] controles = Controls.Find("SumaVV", false);
                                    CtrlCasilla casTemp = (CtrlCasilla)controles[0];
                                    casTemp.Etiqueta.Text = columnasVV.ToString();
                                }
                                break;
                        }
                        cas.Etiqueta.BackColor = Color.White;
                        cas.Etiqueta.ForeColor = Color.Black;
                    }
                }
            }
        }

        #region EventosFilas
        private void lbl1X_Click(object sender, EventArgs e)
        {
            if (lbl1X.BackColor == Color.NavajoWhite)
            {
                MarcarFila("1X");
            }
            else
            {
                DesmarcarFila("1X");
            }
        }
        private void lbl12_Click(object sender, EventArgs e)
        {
            if (lbl12.BackColor == Color.NavajoWhite)
            {
                MarcarFila("12");
            }
            else
            {
                DesmarcarFila("12");
            }
        }
        private void lblX2_Click(object sender, EventArgs e)
        {
            if (lblX2.BackColor == Color.NavajoWhite)
            {
                MarcarFila("X2");
            }
            else
            {
                DesmarcarFila("X2");
            }
        }
        private void lbl11_Click(object sender, EventArgs e)
        {
            if (lbl11.BackColor == Color.NavajoWhite)
            {
                MarcarFila("11");
            }
            else
            {
                DesmarcarFila("11");
            }
        }
        private void lblXX_Click(object sender, EventArgs e)
        {
            if (lblXX.BackColor == Color.NavajoWhite)
            {
                MarcarFila("XX");
            }
            else
            {
                DesmarcarFila("XX");
            }
        }
        private void lbl22_Click(object sender, EventArgs e)
        {
            if (lbl22.BackColor == Color.NavajoWhite)
            {
                MarcarFila("22");
            }
            else
            {
                DesmarcarFila("22");
            }
        }
        private void lbl1V_Click(object sender, EventArgs e)
        {
            if (lbl1V.BackColor == Color.NavajoWhite)
            {
                MarcarFila("1V");
            }
            else
            {
                DesmarcarFila("1V");
            }
        }
        private void lblXV_Click(object sender, EventArgs e)
        {
            if (lblXV.BackColor == Color.NavajoWhite)
            {
                MarcarFila("XV");
            }
            else
            {
                DesmarcarFila("XV");
            }
        }
        private void lbl2V_Click(object sender, EventArgs e)
        {
            if (lbl2V.BackColor == Color.NavajoWhite)
            {
                MarcarFila("2V");
            }
            else
            {
                DesmarcarFila("2V");
            }
        }
        private void lblVV_Click(object sender, EventArgs e)
        {
            if (lblVV.BackColor == Color.NavajoWhite)
            {
                MarcarFila("VV");
            }
            else
            {
                DesmarcarFila("VV");
            }
        } 
        #endregion

        private void btnMarcarCondicion_Click(object sender, EventArgs e)
        {
            //Antes que nada obtener el control Figuras
            Control[] controles = Controls.Find("ctrlFig", false);
            if (controles.Length > 0)
            {
                CtrlDataGridViewFiguras Ctrl = (CtrlDataGridViewFiguras)controles[0];
                filtroContactos.Figuras = Ctrl.ObtenerFigurasMarcadas();
            }
            //Respetar los valores introducidos, si no hay ninguno, llenar todos

            if (!DatosMarcados())
            {
                filtroContactos.LlenarTodosValores();
            }
            else
            {
                filtroContactos.SetNum1X(ObtenerContactosMarcados("1X"));
                filtroContactos.SetNum12(ObtenerContactosMarcados("12"));
                filtroContactos.SetNumX2(ObtenerContactosMarcados("X2"));
                filtroContactos.SetNum11(ObtenerContactosMarcados("11"));
                filtroContactos.SetNumXX(ObtenerContactosMarcados("XX"));
                filtroContactos.SetNum22(ObtenerContactosMarcados("22"));
                filtroContactos.SetNum1V(ObtenerContactosMarcados("1V"));
                filtroContactos.SetNumXV(ObtenerContactosMarcados("XV"));
                filtroContactos.SetNum2V(ObtenerContactosMarcados("2V"));
                filtroContactos.SetNumVV(ObtenerContactosMarcados("VV"));

            }

            //Si el filtro no está activo, activarlo
            if (!filtroContactos.IsActive)
            {
                filtroContactos.IsActive = true;
            }
            if (!filtroContactos.ContieneDatos)
            {
                filtroContactos.ContieneDatos = true;
            }

            MessageBox.Show("Los datos se han marcado en la combinación", "Información", MessageBoxButtons.OK);

        }
        protected bool DatosMarcados()
        {
            if ((valores1X.Count > 0) ||
               (valores12.Count > 0) ||
               (valoresX2.Count > 0) ||
               (valores11.Count > 0) ||
               (valoresXX.Count > 0) ||
               (valores22.Count > 0) ||
               (valores1V.Count > 0) ||
               (valoresXV.Count > 0) ||
               (valores2V.Count > 0) ||
               (valoresVV.Count > 0))
            {
                return true;
            }
            return false;
        }

        protected string ObtenerContactosMarcados(string tipo)
        {
            string cadena = "";
            switch (tipo)
            {
                case "1X":
                    cadena = ObtenerStringFromList(valores1X);
                    break;
                case "12":
                    cadena = ObtenerStringFromList(valores12);
                    break;
                case "X2":
                    cadena = ObtenerStringFromList(valoresX2);
                    break;
                case "11":
                    cadena = ObtenerStringFromList(valores11);
                    break;
                case "XX":
                    cadena = ObtenerStringFromList(valoresXX);
                    break;
                case "22":
                    cadena = ObtenerStringFromList(valores22);
                    break;
                case "1V":
                    cadena = ObtenerStringFromList(valores1V);
                    break;
                case "XV":
                    cadena = ObtenerStringFromList(valoresXV);
                    break;
                case "2V":
                    cadena = ObtenerStringFromList(valores2V);
                    break;
                case "VV":
                    cadena = ObtenerStringFromList(valoresVV);
                    break;
            }
            return cadena;
        }
        protected string ObtenerStringFromList(List<int> valores)
        {
            string cadena = "";
            if (valores.Count == 0)
            {
                cadena = "0,1,2,3,4,5,6,7,8,9,10,11,12,13";
            }
            else
            {
                for (int i = 0; i < valores.Count; i++)
                {
                    if (i != valores.Count - 1)
                    {
                        cadena += valores[i] + ",";
                    }
                    else
                    {
                        cadena += valores[i].ToString();
                    }
                }
            }
            return cadena;
        }
    }
}
