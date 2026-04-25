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

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI.Controls.Analisis
{
    public partial class CtrlAnalisisVX2 : UserControl
    {
        int noCasillasV;
        int noCasillasX;
        int noCasillas2;
        FiltroNoVariantes filtroVariantes;
        List<int> valoresV = new List<int>();
        List<int> valoresX = new List<int>();
        List<int> valores2 = new List<int>();
        int columnasV;
        int columnasX;
        int columnas2;
        public CtrlAnalisisVX2(int[,] valores, FiltroNoVariantes filtro, bool analisisExterno)
        {
            InitializeComponent();
            noCasillasV = valores.GetLength(1);
            noCasillasX = valores.GetLength(1);
            noCasillas2 = valores.GetLength(1);

            filtroVariantes = filtro;
            LlenarNumeros();
            LlenarValores(valores);

            if(analisisExterno)
            {
                btnMarcarCondicion.Enabled = false;
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
            LlenarVariantes(valores);
            LlenarEquis(valores);
            LlenarDoses(valores);
        }
        protected void LlenarVariantes(int[,] valores)
        {
            int x = lblVariantes.Location.X + lblVariantes.Size.Width + 1;
            int y = lblVariantes.Location.Y;
            int valor = 0;
            for (int i = 0; i < noCasillasV; i++)
            {
                string texto = valores[0, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Location = new Point(x, y);
                casilla.Etiqueta.Click += lblCasillaV_Click;
                casilla.Tipo = "V";
                casilla.Valor = valor;
                Controls.Add(casilla);
                x += casilla.Width + 1;
                valor++;
            }
            CtrlCasilla cCasillaSuma = new CtrlCasilla("");
            cCasillaSuma.Location = new Point(x, y);
            cCasillaSuma.Name = "SumaV";
            cCasillaSuma.SetColor(Color.NavajoWhite);
            Controls.Add(cCasillaSuma);
        }
        protected void LlenarEquis(int[,] valores)
        {
            int x = lblEquis.Location.X + lblEquis.Size.Width + 1;
            int y = lblEquis.Location.Y;
            int valor = 0;
            for (int i = 0; i < noCasillasX; i++)
            {
                string texto = valores[1, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Location = new Point(x, y);
                casilla.Etiqueta.Click += lblCasillaX_Click;
                casilla.Tipo = "X";
                casilla.Valor = valor;
                Controls.Add(casilla);
                x += casilla.Width + 1;
                valor++;
            }
            CtrlCasilla cCasillaSuma = new CtrlCasilla("");
            cCasillaSuma.Location = new Point(x, y);
            cCasillaSuma.Name = "SumaX";
            cCasillaSuma.SetColor(Color.NavajoWhite);
            Controls.Add(cCasillaSuma);
        }
        protected void LlenarDoses(int[,] valores)
        {
            int x = lblDoses.Location.X + lblDoses.Size.Width + 1;
            int y = lblDoses.Location.Y;
            int valor = 0;
            for (int i = 0; i < noCasillas2; i++)
            {
                string texto = valores[2, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Location = new Point(x, y);
                casilla.Etiqueta.Click += lblCasilla2_Click;
                casilla.Tipo = "2";
                casilla.Valor = valor;
                Controls.Add(casilla);
                x += casilla.Width + 1;
                valor++;
            }
            CtrlCasilla cCasillaSuma = new CtrlCasilla("");
            cCasillaSuma.Location = new Point(x, y);
            cCasillaSuma.Name = "Suma2";
            cCasillaSuma.SetColor(Color.NavajoWhite);
            Controls.Add(cCasillaSuma);

        }
        protected void LlenarNumeros()
        {
            int x = lblNoVX2.Location.X + lblNoVX2.Size.Width + 1;
            int y = lblNoVX2.Location.Y;
            int numero = 0;
            for (int i = 0; i < noCasillas2; i++)
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

        private void lblCasillaV_Click(object sender, EventArgs e)
        {
            //Obtener la etiqueta
            Label label = (Label)sender;
            //Obtener el control padre
            CtrlCasilla ctrlCas = (CtrlCasilla)label.Parent;
            int valor = ctrlCas.Valor;
            if (!valoresV.Contains(valor))
            {
                //No esta contenido, marcar y añadir
                valoresV.Add(valor);
                label.BackColor = Color.DarkSalmon;
                label.ForeColor = Color.White;
                columnasV += Convert.ToInt32(label.Text);
            }
            else
            {
                //Esta contenido, desmarcar y borrar
                valoresV.Remove(valor);
                label.BackColor = Color.White;
                label.ForeColor = Color.Black;
                columnasV -= Convert.ToInt32(label.Text);
            }
            Control[] controles = Controls.Find("SumaV", false);
            CtrlCasilla casTemp = (CtrlCasilla)controles[0];
            casTemp.Etiqueta.Text = columnasV.ToString();
        }
        private void lblCasillaX_Click(object sender, EventArgs e)
        {
            //Obtener la etiqueta
            Label label = (Label)sender;
            //Obtener el control padre
            CtrlCasilla ctrlCas = (CtrlCasilla)label.Parent;
            int valor = ctrlCas.Valor;
            if (!valoresX.Contains(valor))
            {
                //No esta contenido, marcar y añadir
                valoresX.Add(valor);
                label.BackColor = Color.DarkSalmon;
                label.ForeColor = Color.White;
                columnasX += Convert.ToInt32(label.Text);
            }
            else
            {
                //Esta contenido, desmarcar y borrar
                valoresX.Remove(valor);
                label.BackColor = Color.White;
                label.ForeColor = Color.Black;
                columnasX -= Convert.ToInt32(label.Text);
            }
            Control[] controles = Controls.Find("SumaX", false);
            CtrlCasilla casTemp = (CtrlCasilla)controles[0];
            casTemp.Etiqueta.Text = columnasX.ToString();
        }
        private void lblCasilla2_Click(object sender, EventArgs e)
        {
            //Obtener la etiqueta
            Label label = (Label)sender;
            //Obtener el control padre
            CtrlCasilla ctrlCas = (CtrlCasilla)label.Parent;
            int valor = ctrlCas.Valor;
            if (!valores2.Contains(valor))
            {
                //No esta contenido, marcar y añadir
                valores2.Add(valor);
                label.BackColor = Color.DarkSalmon;
                label.ForeColor = Color.White;
                columnas2 += Convert.ToInt32(label.Text);
            }
            else
            {
                //Esta contenido, desmarcar y borrar
                valores2.Remove(valor);
                label.BackColor = Color.White;
                label.ForeColor = Color.Black;
                columnas2 -= Convert.ToInt32(label.Text);
            }
            Control[] controles = Controls.Find("Suma2", false);
            CtrlCasilla casTemp = (CtrlCasilla)controles[0];
            casTemp.Etiqueta.Text = columnas2.ToString();
        }


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
                            case "V":
                                if (!valoresV.Contains(cas.Valor))
                                {
                                    valoresV.Add(cas.Valor);
                                    lblVariantes.BackColor = Color.DarkKhaki;
                                    columnasV += Convert.ToInt32(cas.Etiqueta.Text);

                                    Control[] controles = Controls.Find("SumaV", false);
                                    CtrlCasilla casTemp = (CtrlCasilla)controles[0];
                                    casTemp.Etiqueta.Text = columnasV.ToString();
                                }
                                break;
                            case "X":
                                if (!valoresX.Contains(cas.Valor))
                                {
                                    valoresX.Add(cas.Valor);
                                    lblEquis.BackColor = Color.DarkKhaki;
                                    columnasX += Convert.ToInt32(cas.Etiqueta.Text);

                                    Control[] controles = Controls.Find("SumaX", false);
                                    CtrlCasilla casTemp = (CtrlCasilla)controles[0];
                                    casTemp.Etiqueta.Text = columnasX.ToString();
                                }
                                break;
                            case "2":
                                if (!valores2.Contains(cas.Valor))
                                {
                                    valores2.Add(cas.Valor);
                                    lblDoses.BackColor = Color.DarkKhaki;
                                    columnas2 += Convert.ToInt32(cas.Etiqueta.Text);
                                    Control[] controles = Controls.Find("Suma2", false);
                                    CtrlCasilla casTemp = (CtrlCasilla)controles[0];
                                    casTemp.Etiqueta.Text = columnas2.ToString();
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
                            case "V":
                                if (valoresV.Contains(cas.Valor))
                                {
                                    valoresV.Remove(cas.Valor);
                                    lblVariantes.BackColor = Color.NavajoWhite;
                                    columnasV -= Convert.ToInt32(cas.Etiqueta.Text);

                                    Control[] controles = Controls.Find("SumaV", false);
                                    CtrlCasilla casTemp = (CtrlCasilla)controles[0];
                                    casTemp.Etiqueta.Text = columnasV.ToString();
                                }
                                break;
                            case "X":
                                if (valoresX.Contains(cas.Valor))
                                {
                                    valoresX.Remove(cas.Valor);
                                    lblEquis.BackColor = Color.NavajoWhite;
                                    columnasX -= Convert.ToInt32(cas.Etiqueta.Text);

                                    Control[] controles = Controls.Find("SumaX", false);
                                    CtrlCasilla casTemp = (CtrlCasilla)controles[0];
                                    casTemp.Etiqueta.Text = columnasX.ToString();
                                }
                                break;
                            case "2":
                                if (valores2.Contains(cas.Valor))
                                {
                                    valores2.Remove(cas.Valor);
                                    lblDoses.BackColor = Color.NavajoWhite;
                                    columnas2 -= Convert.ToInt32(cas.Etiqueta.Text);
                                    Control[] controles = Controls.Find("Suma2", false);
                                    CtrlCasilla casTemp = (CtrlCasilla)controles[0];
                                    casTemp.Etiqueta.Text = columnas2.ToString();
                                }
                                break;
                            
                            
                            
                            
                            
                           
                            
                        }
                        cas.Etiqueta.BackColor = Color.White;
                        cas.Etiqueta.ForeColor = Color.Black;
                    }
                }
            }
        }

        private void lblVariantes_Click(object sender, EventArgs e)
        {
            if (lblVariantes.BackColor == Color.NavajoWhite)
            {
                MarcarFila("V");
            }
            else
            {
                DesmarcarFila("V");
            }
        }

        private void lblEquis_Click(object sender, EventArgs e)
        {
            if (lblEquis.BackColor == Color.NavajoWhite)
            {
                MarcarFila("X");
            }
            else
            {
                DesmarcarFila("X");
            }
        }

        private void lblDoses_Click(object sender, EventArgs e)
        {
            if (lblDoses.BackColor == Color.NavajoWhite)
            {
                MarcarFila("2");
            }
            else
            {
                DesmarcarFila("2");
            }
        }

        private void btnMarcarCondicion_Click(object sender, EventArgs e)
        {
            //Antes que nada obtener el control Figuras
            Control[] controles = Controls.Find("ctrlFig", false);
            if (controles.Length > 0)
            {
                CtrlDataGridViewFiguras Ctrl = (CtrlDataGridViewFiguras)controles[0];
                filtroVariantes.Figuras = Ctrl.ObtenerFigurasMarcadas();
            }
            //Respetar los valores introducidos, si no hay ninguno, llenar todos

            if (!DatosMarcados())
            {
                filtroVariantes.LlenarTodosValores();
            }
            else
            {
                filtroVariantes.SetNoVariantes(ObtenerContactosMarcados("V"));
                filtroVariantes.SetNoEquis(ObtenerContactosMarcados("X"));
                filtroVariantes.SetNoDoses(ObtenerContactosMarcados("2"));
            }

            //Si el filtro no está activo, activarlo
            if (!filtroVariantes.IsActive)
            {
                filtroVariantes.IsActive = true;
            }
            if (!filtroVariantes.ContieneDatos)
            {
                filtroVariantes.ContieneDatos = true;
            }

            MessageBox.Show("Los datos se han marcado en la combinación", "Información", MessageBoxButtons.OK);

        }
        protected bool DatosMarcados()
        {
            if ((valoresV.Count > 0) ||
               (valoresX.Count > 0) ||
               (valores2.Count > 0))
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
                case "V":
                    cadena = Utils.UtilidadesEntradasValores.ObtenerTextoFromListAciertos(valoresV);
                    break;
                case "X":
                    cadena = Utils.UtilidadesEntradasValores.ObtenerTextoFromListAciertos(valoresX);
                    break;
                case "2":
                    cadena = Utils.UtilidadesEntradasValores.ObtenerTextoFromListAciertos(valores2);
                    break;
            }
            if (cadena == "")
            {
                return Utils.UtilidadesEntradasValores.ObtenerTodosValores();
            }
            return cadena;
        }

    }
}
