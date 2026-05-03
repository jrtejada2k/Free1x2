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
using System;

using Free1X2.MotorCalculo;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI.Controls.Analisis
{
    public partial class CtrlAnalisisPesos : UserControl
    {
        int noCasillas;
        List<FiguraCondicion> listaFiguras;
        FiltroPesosNumericos filtroPesos;

        List<int> valoresPG = new List<int>();
        List<int> valoresPV = new List<int>();
        List<int> valoresP1 = new List<int>();
        List<int> valoresPX = new List<int>();
        List<int> valoresP2 = new List<int>();

        private int columnasPG;
        private int columnasPV;
        private int columnasP1;
        private int columnasPX;
        private int columnasP2;

        public CtrlAnalisisPesos(int[,] valores, List<FiguraCondicion> lstFiguras, FiltroPesosNumericos filtro, bool esAnalisisExterno)
        {
            InitializeComponent();
            noCasillas = valores.GetLength(1);
            listaFiguras = lstFiguras;
            filtroPesos = filtro;
            LlenarNumeros();
            LlenarValores(valores);

            if (VariablesGlobales.AnalizarFigurasPesos)
            {
                MostrarFiguras();
            }
            if(esAnalisisExterno)
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
            LlenarPesoGlobal(valores);
            LlenarPesoVar(valores);
            LlenarPesoUnos(valores);
            LlenarPesoEquis(valores);
            LlenarPesoDoses(valores);
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
        protected void LlenarPesoGlobal(int[,] valores)
        {
            int x = lblPG.Location.X + lblPG.Size.Width + 1;
            int y = lblPG.Location.Y;
            int valor = 0;
            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[0, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Location = new Point(x, y);
                casilla.Etiqueta.Click += lblCasillaPG_Click;
                casilla.Tipo = "PG";
                casilla.Valor = valor;
                Controls.Add(casilla);
                x += casilla.Width + 1;
                valor++;
            }

            CtrlCasilla cCasillaSuma = new CtrlCasilla("");
            cCasillaSuma.Location = new Point(x, y);
            cCasillaSuma.Name = "SumaPG";
            cCasillaSuma.SetColor(Color.NavajoWhite);
            Controls.Add(cCasillaSuma);

        }
        protected void LlenarPesoVar(int[,] valores)
        {
            int x = lblPV.Location.X + lblPV.Size.Width + 1;
            int y = lblPV.Location.Y;
            int valor = 0;

            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[1, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Location = new Point(x, y);
                casilla.Etiqueta.Click += lblCasillaPV_Click;
                casilla.Tipo = "PV";
                casilla.Valor = valor;
                Controls.Add(casilla);
                x += casilla.Width + 1;
                valor++;
            }

            CtrlCasilla cCasillaSuma = new CtrlCasilla("");
            cCasillaSuma.Location = new Point(x, y);
            cCasillaSuma.Name = "SumaPV";
            cCasillaSuma.SetColor(Color.NavajoWhite);
            Controls.Add(cCasillaSuma);

        }
        protected void LlenarPesoUnos(int[,] valores)
        {
            int x = lblP1.Location.X + lblP1.Size.Width + 1;
            int y = lblP1.Location.Y;
            int valor = 0;

            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[2, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Location = new Point(x, y);
                casilla.Etiqueta.Click += lblCasillaP1_Click;
                casilla.Tipo = "P1";
                casilla.Valor = valor;
                Controls.Add(casilla);
                x += casilla.Width + 1;
                valor++;
            }

            CtrlCasilla cCasillaSuma = new CtrlCasilla("");
            cCasillaSuma.Location = new Point(x, y);
            cCasillaSuma.Name = "SumaP1";
            cCasillaSuma.SetColor(Color.NavajoWhite);
            Controls.Add(cCasillaSuma);

        }
        protected void LlenarPesoEquis(int[,] valores)
        {
            int x = lblPX.Location.X + lblPX.Size.Width + 1;
            int y = lblPX.Location.Y;
            int valor = 0;

            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[3, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Location = new Point(x, y);
                casilla.Etiqueta.Click += lblCasillaPX_Click;
                casilla.Tipo = "PX";
                casilla.Valor = valor;
                Controls.Add(casilla);
                x += casilla.Width + 1;
                valor++;
            }

            CtrlCasilla cCasillaSuma = new CtrlCasilla("");
            cCasillaSuma.Location = new Point(x, y);
            cCasillaSuma.Name = "SumaPX";
            cCasillaSuma.SetColor(Color.NavajoWhite);
            Controls.Add(cCasillaSuma);

        }
        protected void LlenarPesoDoses(int[,] valores)
        {
            int x = lblP2.Location.X + lblP2.Size.Width + 1;
            int y = lblP2.Location.Y;
            int valor = 0;

            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[4, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Location = new Point(x, y);
                casilla.Etiqueta.Click += lblCasillaP2_Click;
                casilla.Tipo = "P2";
                casilla.Valor = valor;
                Controls.Add(casilla);
                x += casilla.Width + 1;
                valor++;
            }

            CtrlCasilla cCasillaSuma = new CtrlCasilla("");
            cCasillaSuma.Location = new Point(x, y);
            cCasillaSuma.Name = "SumaP2";
            cCasillaSuma.SetColor(Color.NavajoWhite);
            Controls.Add(cCasillaSuma);

        }
        protected void MostrarFiguras()
        {
            CtrlDataGridViewFiguras ctrlFig = new CtrlDataGridViewFiguras(listaFiguras, filtroPesos);
            ctrlFig.Location = new Point(0, 120);
            ctrlFig.Name = "ctrlFig";
            Controls.Add(ctrlFig);
            btnMarcarCondicion.Location = new Point(0, 120 + ctrlFig.Size.Height + 1);
        }


        private void lblCasillaPG_Click(object sender, EventArgs e)
        {
            //Obtener la etiqueta
            Label label = (Label)sender;
            //Obtener el control padre
            CtrlCasilla ctrlCas = (CtrlCasilla)label.Parent;
            int valor = ctrlCas.Valor;
            if (!valoresPG.Contains(valor))
            {
                //No esta contenido, marcar y añadir
                valoresPG.Add(valor);
                label.BackColor = Color.DarkSalmon;
                label.ForeColor = Color.White;
                columnasPG += Convert.ToInt32(label.Text);
            }
            else
            {
                //Esta contenido, desmarcar y borrar
                valoresPG.Remove(valor);
                label.BackColor = Color.White;
                label.ForeColor = Color.Black;
                columnasPG -= Convert.ToInt32(label.Text);
            }
            Control[] controles = Controls.Find("SumaPG", false);
            CtrlCasilla casTemp = (CtrlCasilla)controles[0];
            casTemp.Etiqueta.Text = columnasPG.ToString();
        }
        private void lblCasillaPV_Click(object sender, EventArgs e)
        {
            //Obtener la etiqueta
            Label label = (Label)sender;
            //Obtener el control padre
            CtrlCasilla ctrlCas = (CtrlCasilla)label.Parent;
            int valor = ctrlCas.Valor;
            if (!valoresPV.Contains(valor))
            {
                //No esta contenido, marcar y añadir
                valoresPV.Add(valor);
                label.BackColor = Color.DarkSalmon;
                label.ForeColor = Color.White;
                columnasPV += Convert.ToInt32(label.Text);
            }
            else
            {
                //Esta contenido, desmarcar y borrar
                valoresPV.Remove(valor);
                label.BackColor = Color.White;
                label.ForeColor = Color.Black;
                columnasPV -= Convert.ToInt32(label.Text);
            }
            Control[] controles = Controls.Find("SumaPV", false);
            CtrlCasilla casTemp = (CtrlCasilla)controles[0];
            casTemp.Etiqueta.Text = columnasPV.ToString();
        }
        private void lblCasillaP1_Click(object sender, EventArgs e)
        {
            //Obtener la etiqueta
            Label label = (Label)sender;
            //Obtener el control padre
            CtrlCasilla ctrlCas = (CtrlCasilla)label.Parent;
            int valor = ctrlCas.Valor;
            if (!valoresP1.Contains(valor))
            {
                //No esta contenido, marcar y añadir
                valoresP1.Add(valor);
                label.BackColor = Color.DarkSalmon;
                label.ForeColor = Color.White;
                columnasP1 += Convert.ToInt32(label.Text);
            }
            else
            {
                //Esta contenido, desmarcar y borrar
                valoresP1.Remove(valor);
                label.BackColor = Color.White;
                label.ForeColor = Color.Black;
                columnasP1 -= Convert.ToInt32(label.Text);
            }
            Control[] controles = Controls.Find("SumaP1", false);
            CtrlCasilla casTemp = (CtrlCasilla)controles[0];
            casTemp.Etiqueta.Text = columnasP1.ToString();
        }
        private void lblCasillaPX_Click(object sender, EventArgs e)
        {
            //Obtener la etiqueta
            Label label = (Label)sender;
            //Obtener el control padre
            CtrlCasilla ctrlCas = (CtrlCasilla)label.Parent;
            int valor = ctrlCas.Valor;
            if (!valoresPX.Contains(valor))
            {
                //No esta contenido, marcar y añadir
                valoresPX.Add(valor);
                label.BackColor = Color.DarkSalmon;
                label.ForeColor = Color.White;
                columnasPX += Convert.ToInt32(label.Text);
            }
            else
            {
                //Esta contenido, desmarcar y borrar
                valoresPX.Remove(valor);
                label.BackColor = Color.White;
                label.ForeColor = Color.Black;
                columnasPX -= Convert.ToInt32(label.Text);
            }
            Control[] controles = Controls.Find("SumaPX", false);
            CtrlCasilla casTemp = (CtrlCasilla)controles[0];
            casTemp.Etiqueta.Text = columnasPX.ToString();
        }
        private void lblCasillaP2_Click(object sender, EventArgs e)
        {
            //Obtener la etiqueta
            Label label = (Label)sender;
            //Obtener el control padre
            CtrlCasilla ctrlCas = (CtrlCasilla)label.Parent;
            int valor = ctrlCas.Valor;
            if (!valoresP2.Contains(valor))
            {
                //No esta contenido, marcar y añadir
                valoresP2.Add(valor);
                label.BackColor = Color.DarkSalmon;
                label.ForeColor = Color.White;
                columnasP2 += Convert.ToInt32(label.Text);
            }
            else
            {
                //Esta contenido, desmarcar y borrar
                valoresP2.Remove(valor);
                label.BackColor = Color.White;
                label.ForeColor = Color.Black;
                columnasP2 -= Convert.ToInt32(label.Text);
            }
            Control[] controles = Controls.Find("SumaP2", false);
            CtrlCasilla casTemp = (CtrlCasilla)controles[0];
            casTemp.Etiqueta.Text = columnasP2.ToString();
        }

        private void btnMarcarCondicion_Click(object sender, EventArgs e)
        {
            //Respetar los valores introducidos, si no hay ninguno, llenar todos

            if (!DatosMarcados())
            {
                filtroPesos.LlenarTodosValores();
            }
            else
            {
                filtroPesos.SetPNGlobal(ObtenerPesosMarcados("PG"));
                filtroPesos.SetPNVar(ObtenerPesosMarcados("PV"));
                filtroPesos.SetPNUnos(ObtenerPesosMarcados("P1"));
                filtroPesos.SetPNEquis(ObtenerPesosMarcados("PX"));
                filtroPesos.SetPNDoses(ObtenerPesosMarcados("P2"));
            }

            //Si el filtro no está activo, activarlo
            if (!filtroPesos.IsActive)
            {
                filtroPesos.IsActive = true;
            }
            if (!filtroPesos.ContieneDatos)
            {
                filtroPesos.ContieneDatos = true;
            }

            MessageBox.Show("Los datos se han marcado en la combinación", "Información", MessageBoxButtons.OK);

        }

        protected bool DatosMarcados()
        {
            if ((valoresPG.Count > 0) ||
               (valoresPV.Count > 0) ||
               (valoresP1.Count > 0) ||
               (valoresPX.Count > 0) ||
               (valoresP2.Count > 0))
            {
                return true;
            }
            return false;
        }

        protected string ObtenerPesosMarcados(string tipo)
        {
            string cadena = "";
            switch (tipo)
            {
                case "PG":
                    cadena = Utils.UtilidadesEntradasValores.ObtenerTextoFromListAciertos(valoresPG);
                    break;
                case "PV":
                    cadena = Utils.UtilidadesEntradasValores.ObtenerTextoFromListAciertos(valoresPV);
                    break;
                case "P1":
                    cadena = Utils.UtilidadesEntradasValores.ObtenerTextoFromListAciertos(valoresP1);
                    break;
                case "PX":
                    cadena = Utils.UtilidadesEntradasValores.ObtenerTextoFromListAciertos(valoresPX);
                    break;
                case "P2":
                    cadena = Utils.UtilidadesEntradasValores.ObtenerTextoFromListAciertos(valoresP2);
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
