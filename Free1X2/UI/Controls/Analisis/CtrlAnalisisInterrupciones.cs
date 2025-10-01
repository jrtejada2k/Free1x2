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
    public partial class CtrlAnalisisInterrupciones : UserControl
    {
        int noCasillas;
        FiltroInterrupciones filtroInterrupciones;
        List<int> valoresIntG = new List<int>();
        List<int> valoresIntV = new List<int>();
        List<int> valoresInt1 = new List<int>();
        List<int> valoresIntX = new List<int>();
        List<int> valoresInt2 = new List<int>();
        List<int> valoresIntG_Seg = new List<int>();
        List<int> valoresIntV_Seg = new List<int>();
        List<int> valoresInt1_Seg = new List<int>();
        List<int> valoresIntX_Seg = new List<int>();
        List<int> valoresInt2_Seg = new List<int>();

        private int columnasIntG;
        private int columnasIntV;
        private int columnasIntG_Seg;
        private int columnasIntV_Seg;
        private int columnasInt1;
        private int columnasInt1_Seg;
        private int columnasIntX;
        private int columnasIntX_Seg;
        private int columnasInt2;
        private int columnasInt2_Seg;

        public CtrlAnalisisInterrupciones(int[,] valores, FiltroInterrupciones filtro, bool esAnalisisExterno)
        {
            InitializeComponent();
            noCasillas = valores.GetLength(1);
            filtroInterrupciones = filtro;
            LlenarNumeros();
            LlenarValores(valores);
            if(esAnalisisExterno)
            {
                btnMarcarCondicion.Enabled = false;
            }
        }
        protected void LlenarValores(int[,] valores)
        {
            //Por cada valor a mostrar hay que insertar una CrtlCasilla inicializada
            LlenarIntGlobales(valores);
            LlenarIntVariantes(valores);
            LlenarIntUnos(valores);
            LlenarIntEquis(valores);
            LlenarIntDoses(valores);
            LlenarIntGlobalesSeg(valores);
            LlenarIntVariantesSeg(valores);
            LlenarIntUnosSeg(valores);
            LlenarIntEquisSeg(valores);
            LlenarIntDosesSeg(valores);
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
        protected void LlenarIntGlobales(int[,] valores)
        {
            int x = lblIntGlobales.Location.X + lblIntGlobales.Size.Width + 1;
            int y = lblIntGlobales.Location.Y;
            int valor = 0;
            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[0, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Location = new Point(x, y);
                casilla.Etiqueta.Click += lblCasillaIntG_Click;
                casilla.Tipo = "IntG";
                casilla.Valor = valor;
                Controls.Add(casilla);
                x += casilla.Width + 1;
                valor++;
            }

            CtrlCasilla cCasillaSuma = new CtrlCasilla("");
            cCasillaSuma.Location = new Point(x, y);
            cCasillaSuma.Name = "SumaIntG";
            cCasillaSuma.SetColor(Color.NavajoWhite);
            Controls.Add(cCasillaSuma);

        }
        protected void LlenarIntVariantes(int[,] valores)
        {
            int x = lblIntVar.Location.X + lblIntVar.Size.Width + 1;
            int y = lblIntVar.Location.Y;
            int valor = 0;
            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[1, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Location = new Point(x, y);
                casilla.Etiqueta.Click += lblCasillaIntV_Click;
                casilla.Tipo = "IntV";
                casilla.Valor = valor;
                Controls.Add(casilla);
                x += casilla.Width + 1;
                valor++;
            }

            CtrlCasilla cCasillaSuma = new CtrlCasilla("");
            cCasillaSuma.Location = new Point(x, y);
            cCasillaSuma.Name = "SumaIntV";
            cCasillaSuma.SetColor(Color.NavajoWhite);
            Controls.Add(cCasillaSuma);
        }
        protected void LlenarIntEquis(int[,] valores)
        {
            int x = lblIntEquis.Location.X + lblIntEquis.Size.Width + 1;
            int y = lblIntEquis.Location.Y;
            int valor = 0;
            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[3, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Location = new Point(x, y);
                casilla.Etiqueta.Click += lblCasillaIntX_Click;
                casilla.Tipo = "IntX";
                casilla.Valor = valor;
                Controls.Add(casilla);
                x += casilla.Width + 1;
                valor++;
            }

            CtrlCasilla cCasillaSuma = new CtrlCasilla("");
            cCasillaSuma.Location = new Point(x, y);
            cCasillaSuma.Name = "SumaIntX";
            cCasillaSuma.SetColor(Color.NavajoWhite);
            Controls.Add(cCasillaSuma);

        }
        protected void LlenarIntDoses(int[,] valores)
        {
            int x = lblIntDoses.Location.X + lblIntDoses.Size.Width + 1;
            int y = lblIntDoses.Location.Y;
            int valor = 0;
            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[4, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Location = new Point(x, y);
                casilla.Etiqueta.Click += lblCasillaInt2_Click;
                casilla.Tipo = "Int2";
                casilla.Valor = valor;
                Controls.Add(casilla);
                x += casilla.Width + 1;
                valor++;
            }

            CtrlCasilla cCasillaSuma = new CtrlCasilla("");
            cCasillaSuma.Location = new Point(x, y);
            cCasillaSuma.Name = "SumaInt2";
            cCasillaSuma.SetColor(Color.NavajoWhite);
            Controls.Add(cCasillaSuma);

        }
        protected void LlenarIntUnos(int[,] valores)
        {
            int x = lblIntUnos.Location.X + lblIntUnos.Size.Width + 1;
            int y = lblIntUnos.Location.Y;
            int valor = 0;
            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[2, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Location = new Point(x, y);
                casilla.Etiqueta.Click += lblCasillaInt1_Click;
                casilla.Tipo = "Int1";
                casilla.Valor = valor;
                Controls.Add(casilla);
                x += casilla.Width + 1;
                valor++;
            }

            CtrlCasilla cCasillaSuma = new CtrlCasilla("");
            cCasillaSuma.Location = new Point(x, y);
            cCasillaSuma.Name = "SumaInt1";
            cCasillaSuma.SetColor(Color.NavajoWhite);
            Controls.Add(cCasillaSuma);

        }
        protected void LlenarIntGlobalesSeg(int[,] valores)
        {
            int x = lblIntGSeg.Location.X + lblIntGSeg.Size.Width + 1;
            int y = lblIntGSeg.Location.Y;
            int valor = 0;
            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[5, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Location = new Point(x, y);
                casilla.Etiqueta.Click += lblCasillaIntG_Seg_Click;
                casilla.Tipo = "IntG_Seg";
                casilla.Valor = valor;
                Controls.Add(casilla);
                x += casilla.Width + 1;
                valor++;
            }

            CtrlCasilla cCasillaSuma = new CtrlCasilla("");
            cCasillaSuma.Location = new Point(x, y);
            cCasillaSuma.Name = "SumaIntG_Seg";
            cCasillaSuma.SetColor(Color.NavajoWhite);
            Controls.Add(cCasillaSuma);

        }
        protected void LlenarIntVariantesSeg(int[,] valores)
        {
            int x = lblIntVSeg.Location.X + lblIntVSeg.Size.Width + 1;
            int y = lblIntVSeg.Location.Y;
            int valor = 0;
            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[6, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Location = new Point(x, y);
                casilla.Etiqueta.Click += lblCasillaIntV_Seg_Click;
                casilla.Tipo = "IntV_Seg";
                casilla.Valor = valor;
                Controls.Add(casilla);
                x += casilla.Width + 1;
                valor++;
            }

            CtrlCasilla cCasillaSuma = new CtrlCasilla("");
            cCasillaSuma.Location = new Point(x, y);
            cCasillaSuma.Name = "SumaIntV_Seg";
            cCasillaSuma.SetColor(Color.NavajoWhite);
            Controls.Add(cCasillaSuma);

        }
        protected void LlenarIntEquisSeg(int[,] valores)
        {
            int x = lblIntXSeg.Location.X + lblIntXSeg.Size.Width + 1;
            int y = lblIntXSeg.Location.Y;
            int valor = 0;
            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[8, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Location = new Point(x, y);
                casilla.Etiqueta.Click += lblCasillaIntX_Seg_Click;
                casilla.Tipo = "IntX_Seg";
                casilla.Valor = valor;
                Controls.Add(casilla);
                x += casilla.Width + 1;
                valor++;
            }

            CtrlCasilla cCasillaSuma = new CtrlCasilla("");
            cCasillaSuma.Location = new Point(x, y);
            cCasillaSuma.Name = "SumaIntX_Seg";
            cCasillaSuma.SetColor(Color.NavajoWhite);
            Controls.Add(cCasillaSuma);

        }
        protected void LlenarIntDosesSeg(int[,] valores)
        {
            int x = lblIntDSeg.Location.X + lblIntDSeg.Size.Width + 1;
            int y = lblIntDSeg.Location.Y;
            int valor = 0;
            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[9, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Location = new Point(x, y);
                casilla.Etiqueta.Click += lblCasillaInt2_Seg_Click;
                casilla.Tipo = "Int2_Seg";
                casilla.Valor = valor;
                Controls.Add(casilla);
                x += casilla.Width + 1;
                valor++;
            }

            CtrlCasilla cCasillaSuma = new CtrlCasilla("");
            cCasillaSuma.Location = new Point(x, y);
            cCasillaSuma.Name = "SumaInt2_Seg";
            cCasillaSuma.SetColor(Color.NavajoWhite);
            Controls.Add(cCasillaSuma);

        }
        protected void LlenarIntUnosSeg(int[,] valores)
        {
            int x = lblIntUSeg.Location.X + lblIntUSeg.Size.Width + 1;
            int y = lblIntUSeg.Location.Y;
            int valor = 0;
            for (int i = 0; i < noCasillas; i++)
            {
                string texto = valores[7, i].ToString();
                CtrlCasilla casilla = new CtrlCasilla(texto);
                casilla.Location = new Point(x, y);
                casilla.Etiqueta.Click += lblCasillaInt1_Seg_Click;
                casilla.Tipo = "Int1_Seg";
                casilla.Valor = valor;
                Controls.Add(casilla);
                x += casilla.Width + 1;
                valor++;
            }

            CtrlCasilla cCasillaSuma = new CtrlCasilla("");
            cCasillaSuma.Location = new Point(x, y);
            cCasillaSuma.Name = "SumaInt1_Seg";
            cCasillaSuma.SetColor(Color.NavajoWhite);
            Controls.Add(cCasillaSuma);

        }


        private void btnMarcarCondicion_Click(object sender, EventArgs e)
        {
            //Respetar los valores introducidos, si no hay ninguno, llenar todos

            if (!DatosMarcados())
            {
                filtroInterrupciones.LlenarTodosValores();
            }
            else
            {
                filtroInterrupciones.SetNoIntGlobales(ObtenerInterrupcionesMarcadas("IntG"));
                filtroInterrupciones.SetNoIntGlobalSeg(ObtenerInterrupcionesMarcadas("IntG_Seg"));
                filtroInterrupciones.SetNoIntVar(ObtenerInterrupcionesMarcadas("IntV"));
                filtroInterrupciones.SetNoIntVarSeg(ObtenerInterrupcionesMarcadas("IntV_Seg"));
                filtroInterrupciones.SetNoInt1(ObtenerInterrupcionesMarcadas("Int1"));
                filtroInterrupciones.SetNoInt1Seg(ObtenerInterrupcionesMarcadas("Int1_Seg"));
                filtroInterrupciones.SetNoIntX(ObtenerInterrupcionesMarcadas("IntX"));
                filtroInterrupciones.SetNoIntXSeg(ObtenerInterrupcionesMarcadas("IntX_Seg"));
                filtroInterrupciones.SetNoInt2(ObtenerInterrupcionesMarcadas("Int2"));
                filtroInterrupciones.SetNoInt2Seg(ObtenerInterrupcionesMarcadas("Int2_Seg"));

            }

            //Si el filtro no está activo, activarlo
            if (!filtroInterrupciones.IsActive)
            {
                filtroInterrupciones.IsActive = true;
            }
            if (!filtroInterrupciones.ContieneDatos)
            {
                filtroInterrupciones.ContieneDatos = true;
            }

            MessageBox.Show("Los datos se han marcado en la combinación", "Información", MessageBoxButtons.OK);

        }

        protected bool DatosMarcados()
        {
            if ((valoresIntG.Count > 0) ||
               (valoresIntV.Count > 0) ||
               (valoresInt1.Count > 0) ||
               (valoresIntX.Count > 0) ||
               (valoresInt2.Count > 0) ||
               (valoresIntG_Seg.Count > 0) ||
               (valoresIntV_Seg.Count > 0) ||
               (valoresInt1_Seg.Count > 0) ||
               (valoresIntX_Seg.Count > 0) ||
               (valoresInt2_Seg.Count > 0))
            {
                return true;
            }
            return false;
        }

        protected string ObtenerInterrupcionesMarcadas(string tipo)
        {
            string cadena = "";
            switch (tipo)
            {
                case "IntG":
                    cadena = Utils.UtilidadesEntradasValores.ObtenerTextoFromListAciertos(valoresIntG);
                    break;
                case "IntG_Seg":
                    cadena = Utils.UtilidadesEntradasValores.ObtenerTextoFromListAciertos(valoresIntG_Seg);
                    break;
                case "IntV":
                    cadena = Utils.UtilidadesEntradasValores.ObtenerTextoFromListAciertos(valoresIntV);
                    break;
                case "IntV_Seg":
                    cadena = Utils.UtilidadesEntradasValores.ObtenerTextoFromListAciertos(valoresIntV_Seg);
                    break;
                case "Int1":
                    cadena = Utils.UtilidadesEntradasValores.ObtenerTextoFromListAciertos(valoresInt1);
                    break;
                case "Int1_Seg":
                    cadena = Utils.UtilidadesEntradasValores.ObtenerTextoFromListAciertos(valoresInt1_Seg);
                    break;
                case "IntX":
                    cadena = Utils.UtilidadesEntradasValores.ObtenerTextoFromListAciertos(valoresIntX);
                    break;
                case "IntX_Seg":
                    cadena = Utils.UtilidadesEntradasValores.ObtenerTextoFromListAciertos(valoresIntX_Seg);
                    break;
                case "Int2":
                    cadena = Utils.UtilidadesEntradasValores.ObtenerTextoFromListAciertos(valoresInt2);
                    break;
                case "Int2_Seg":
                    cadena = Utils.UtilidadesEntradasValores.ObtenerTextoFromListAciertos(valoresInt2_Seg);
                    break;
            }
            if(cadena=="")
            {
                return Utils.UtilidadesEntradasValores.ObtenerTodosValores();
            }
            return cadena;
        }



        private void lblCasillaIntG_Click(object sender, EventArgs e)
        {
            //Obtener la etiqueta
            Label label = (Label)sender;
            //Obtener el control padre
            CtrlCasilla ctrlCas = (CtrlCasilla)label.Parent;
            int valor = ctrlCas.Valor;
            if (!valoresIntG.Contains(valor))
            {
                //No esta contenido, marcar y añadir
                valoresIntG.Add(valor);
                label.BackColor = Color.DarkSalmon;
                label.ForeColor = Color.White;
                columnasIntG += Convert.ToInt32(label.Text);
            }
            else
            {
                //Esta contenido, desmarcar y borrar
                valoresIntG.Remove(valor);
                label.BackColor = Color.White;
                label.ForeColor = Color.Black;
                columnasIntG -= Convert.ToInt32(label.Text);
            }
            Control[] controles = Controls.Find("SumaIntG", false);
            CtrlCasilla casTemp = (CtrlCasilla)controles[0];
            casTemp.Etiqueta.Text = columnasIntG.ToString();
        }
        private void lblCasillaIntV_Click(object sender, EventArgs e)
        {
            //Obtener la etiqueta
            Label label = (Label)sender;
            //Obtener el control padre
            CtrlCasilla ctrlCas = (CtrlCasilla)label.Parent;
            int valor = ctrlCas.Valor;
            if (!valoresIntV.Contains(valor))
            {
                //No esta contenido, marcar y añadir
                valoresIntV.Add(valor);
                label.BackColor = Color.DarkSalmon;
                label.ForeColor = Color.White;
                columnasIntV += Convert.ToInt32(label.Text);
            }
            else
            {
                //Esta contenido, desmarcar y borrar
                valoresIntV.Remove(valor);
                label.BackColor = Color.White;
                label.ForeColor = Color.Black;
                columnasIntV -= Convert.ToInt32(label.Text);
            }
            Control[] controles = Controls.Find("SumaIntV", false);
            CtrlCasilla casTemp = (CtrlCasilla)controles[0];
            casTemp.Etiqueta.Text = columnasIntV.ToString();
        }
        private void lblCasillaInt1_Click(object sender, EventArgs e)
        {
            //Obtener la etiqueta
            Label label = (Label)sender;
            //Obtener el control padre
            CtrlCasilla ctrlCas = (CtrlCasilla)label.Parent;
            int valor = ctrlCas.Valor;
            if (!valoresInt1.Contains(valor))
            {
                //No esta contenido, marcar y añadir
                valoresInt1.Add(valor);
                label.BackColor = Color.DarkSalmon;
                label.ForeColor = Color.White;
                columnasInt1 += Convert.ToInt32(label.Text);
            }
            else
            {
                //Esta contenido, desmarcar y borrar
                valoresInt1.Remove(valor);
                label.BackColor = Color.White;
                label.ForeColor = Color.Black;
                columnasInt1 -= Convert.ToInt32(label.Text);
            }
            Control[] controles = Controls.Find("SumaInt1", false);
            CtrlCasilla casTemp = (CtrlCasilla)controles[0];
            casTemp.Etiqueta.Text = columnasInt1.ToString();
        }
        private void lblCasillaIntX_Click(object sender, EventArgs e)
        {
            //Obtener la etiqueta
            Label label = (Label)sender;
            //Obtener el control padre
            CtrlCasilla ctrlCas = (CtrlCasilla)label.Parent;
            int valor = ctrlCas.Valor;
            if (!valoresIntX.Contains(valor))
            {
                //No esta contenido, marcar y añadir
                valoresIntX.Add(valor);
                label.BackColor = Color.DarkSalmon;
                label.ForeColor = Color.White;
                columnasIntX += Convert.ToInt32(label.Text);
            }
            else
            {
                //Esta contenido, desmarcar y borrar
                valoresIntX.Remove(valor);
                label.BackColor = Color.White;
                label.ForeColor = Color.Black;
                columnasIntX -= Convert.ToInt32(label.Text);
            }
            Control[] controles = Controls.Find("SumaIntX", false);
            CtrlCasilla casTemp = (CtrlCasilla)controles[0];
            casTemp.Etiqueta.Text = columnasIntX.ToString();
        }
        private void lblCasillaInt2_Click(object sender, EventArgs e)
        {
            //Obtener la etiqueta
            Label label = (Label)sender;
            //Obtener el control padre
            CtrlCasilla ctrlCas = (CtrlCasilla)label.Parent;
            int valor = ctrlCas.Valor;
            if (!valoresInt2.Contains(valor))
            {
                //No esta contenido, marcar y añadir
                valoresInt2.Add(valor);
                label.BackColor = Color.DarkSalmon;
                label.ForeColor = Color.White;
                columnasInt2 += Convert.ToInt32(label.Text);
            }
            else
            {
                //Esta contenido, desmarcar y borrar
                valoresInt2.Remove(valor);
                label.BackColor = Color.White;
                label.ForeColor = Color.Black;
                columnasInt2 -= Convert.ToInt32(label.Text);
            }
            Control[] controles = Controls.Find("SumaInt2", false);
            CtrlCasilla casTemp = (CtrlCasilla)controles[0];
            casTemp.Etiqueta.Text = columnasInt2.ToString();
        }
        private void lblCasillaIntG_Seg_Click(object sender, EventArgs e)
        {
            //Obtener la etiqueta
            Label label = (Label)sender;
            //Obtener el control padre
            CtrlCasilla ctrlCas = (CtrlCasilla)label.Parent;
            int valor = ctrlCas.Valor;
            if (!valoresIntG_Seg.Contains(valor))
            {
                //No esta contenido, marcar y añadir
                valoresIntG_Seg.Add(valor);
                label.BackColor = Color.DarkSalmon;
                label.ForeColor = Color.White;
                columnasIntG_Seg += Convert.ToInt32(label.Text);
            }
            else
            {
                //Esta contenido, desmarcar y borrar
                valoresIntG_Seg.Remove(valor);
                label.BackColor = Color.White;
                label.ForeColor = Color.Black;
                columnasIntG_Seg -= Convert.ToInt32(label.Text);
            }
            Control[] controles = Controls.Find("SumaIntG_Seg", false);
            CtrlCasilla casTemp = (CtrlCasilla)controles[0];
            casTemp.Etiqueta.Text = columnasIntG_Seg.ToString();
        }
        private void lblCasillaIntV_Seg_Click(object sender, EventArgs e)
        {
            //Obtener la etiqueta
            Label label = (Label)sender;
            //Obtener el control padre
            CtrlCasilla ctrlCas = (CtrlCasilla)label.Parent;
            int valor = ctrlCas.Valor;
            if (!valoresIntV_Seg.Contains(valor))
            {
                //No esta contenido, marcar y añadir
                valoresIntV_Seg.Add(valor);
                label.BackColor = Color.DarkSalmon;
                label.ForeColor = Color.White;
                columnasIntV_Seg += Convert.ToInt32(label.Text);
            }
            else
            {
                //Esta contenido, desmarcar y borrar
                valoresIntV_Seg.Remove(valor);
                label.BackColor = Color.White;
                label.ForeColor = Color.Black;
                columnasIntV_Seg -= Convert.ToInt32(label.Text);
            }
            Control[] controles = Controls.Find("SumaIntV_Seg", false);
            CtrlCasilla casTemp = (CtrlCasilla)controles[0];
            casTemp.Etiqueta.Text = columnasIntV_Seg.ToString();
        }
        private void lblCasillaInt1_Seg_Click(object sender, EventArgs e)
        {
            //Obtener la etiqueta
            Label label = (Label)sender;
            //Obtener el control padre
            CtrlCasilla ctrlCas = (CtrlCasilla)label.Parent;
            int valor = ctrlCas.Valor;
            if (!valoresInt1_Seg.Contains(valor))
            {
                //No esta contenido, marcar y añadir
                valoresInt1_Seg.Add(valor);
                label.BackColor = Color.DarkSalmon;
                label.ForeColor = Color.White;
                columnasInt1_Seg += Convert.ToInt32(label.Text);
            }
            else
            {
                //Esta contenido, desmarcar y borrar
                valoresInt1_Seg.Remove(valor);
                label.BackColor = Color.White;
                label.ForeColor = Color.Black;
                columnasInt1_Seg -= Convert.ToInt32(label.Text);
            }
            Control[] controles = Controls.Find("SumaInt1_Seg", false);
            CtrlCasilla casTemp = (CtrlCasilla)controles[0];
            casTemp.Etiqueta.Text = columnasInt1_Seg.ToString();
        }
        private void lblCasillaIntX_Seg_Click(object sender, EventArgs e)
        {
            //Obtener la etiqueta
            Label label = (Label)sender;
            //Obtener el control padre
            CtrlCasilla ctrlCas = (CtrlCasilla)label.Parent;
            int valor = ctrlCas.Valor;
            if (!valoresIntX_Seg.Contains(valor))
            {
                //No esta contenido, marcar y añadir
                valoresIntX_Seg.Add(valor);
                label.BackColor = Color.DarkSalmon;
                label.ForeColor = Color.White;
                columnasIntX_Seg += Convert.ToInt32(label.Text);
            }
            else
            {
                //Esta contenido, desmarcar y borrar
                valoresIntX_Seg.Remove(valor);
                label.BackColor = Color.White;
                label.ForeColor = Color.Black;
                columnasIntX_Seg -= Convert.ToInt32(label.Text);
            }
            Control[] controles = Controls.Find("SumaIntX_Seg", false);
            CtrlCasilla casTemp = (CtrlCasilla)controles[0];
            casTemp.Etiqueta.Text = columnasIntX_Seg.ToString();
        }
        private void lblCasillaInt2_Seg_Click(object sender, EventArgs e)
        {
            //Obtener la etiqueta
            Label label = (Label)sender;
            //Obtener el control padre
            CtrlCasilla ctrlCas = (CtrlCasilla)label.Parent;
            int valor = ctrlCas.Valor;
            if (!valoresInt2_Seg.Contains(valor))
            {
                //No esta contenido, marcar y añadir
                valoresInt2_Seg.Add(valor);
                label.BackColor = Color.DarkSalmon;
                label.ForeColor = Color.White;
                columnasInt2_Seg += Convert.ToInt32(label.Text);
            }
            else
            {
                //Esta contenido, desmarcar y borrar
                valoresInt2_Seg.Remove(valor);
                label.BackColor = Color.White;
                label.ForeColor = Color.Black;
                columnasInt2_Seg -= Convert.ToInt32(label.Text);
            }
            Control[] controles = Controls.Find("SumaInt2_Seg", false);
            CtrlCasilla casTemp = (CtrlCasilla)controles[0];
            casTemp.Etiqueta.Text = columnasInt2_Seg.ToString();
        }

    }
}
