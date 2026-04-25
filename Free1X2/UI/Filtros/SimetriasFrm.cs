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
using System.Drawing;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;

using Free1X2.UI.Controls;
using Free1X2.MotorCalculo;
using Free1X2.MotorCalculo.Estadisticas;
using Free1X2.EntradaSalida;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI.Filtros
{
    /// <summary>
    /// Summary description for SimetriasFrm.
    /// </summary>
    public class SimetriasFrm : Form
    {
        private Panel panel1;
        private MenuCondiciones menuCondiciones2;
        private ContainerControl cctrl;
        int controlesAAñadir = 20;
        private Label label1;
        protected List<Simetria> arraySimetrias;
        protected FiltroSimetrias filtro;
        private readonly Grupo grupo;
        private readonly MainForm parentFrm;
        private TextBox txtAciertos;
        protected List<int> arrayAciertos = new List<int>();
        private ctrlAyuda ctrlAyuda1;
        private IContainer components;
        protected FormulariosHelper formHelper = new FormulariosHelper();

        public SimetriasFrm(Grupo grupo, MainForm form)
        {
            InitializeComponent();
            this.grupo = grupo;
            filtro = (FiltroSimetrias)grupo.GetFiltro( Filtro.Simetrias.ToString());
            if(filtro.ArraySimetrias.Count > 0)
            {
                if(filtro.ArraySimetrias.Count > 30)
                {
                    //necesitamos más controles
                    LlenarControles(filtro.ArraySimetrias.Count);
                }
                else
                {
                    LlenarControles(30);
                }
                MarcarValores();
            }
            else
            {
                LlenarControles(30);
            }
            parentFrm = form;
            compruebaPegar();
            ctrlAyuda1.TextoAyuda = "Se da una Simetría entre Dos o más\npartidos cuando su signo es el mismo\nDebe especificar los partidos separados por \ncomas(a,b), guiones (a-b) o una mezcla de ambos (a,b-c)";
        }
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
        }

        public MainForm FormPadre
        {
            get{ return parentFrm; }
        }
        protected void LlenarControles(int noControles)
        {
            int posicionY = 0;
            int noControl = 1;
            for(int i = 0; i < noControles; i++)
            {
                CtrlSimetria ctrlSimetria = new CtrlSimetria(noControl);

                AñadirControl(ctrlSimetria, 0, posicionY);
                posicionY += 14;
                noControl ++;
            }
        }
        private void AñadirControl(CtrlSimetria ctrlSimetria, int posicionX, int posicionY)
        {
            ctrlSimetria.Location = new Point(posicionX,posicionY);
            cctrl.Controls.Add(ctrlSimetria);
            cctrl.AutoScroll = true;
			
            if(ctrlSimetria.LblNum.Text == cctrl.Controls.Count.ToString())
            {
                controlesAAñadir++;
                ctrlSimetria.TxtSimetria.Enter += Añadir_Enter;
                int indicePre = cctrl.Controls.Count-2;
                if(indicePre >= 0)
                {
                    CtrlSimetria ctrlSimetriaPre = (CtrlSimetria)cctrl.Controls[indicePre];
                    ctrlSimetriaPre.TxtSimetria.Enter -= Añadir_Enter;
                }
            }
        }
        protected override void Dispose( bool disposing )
        {
            if( disposing )
            {
                if(components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.menuCondiciones2 = new Free1X2.UI.Controls.MenuCondiciones();
            this.cctrl = new System.Windows.Forms.ContainerControl();
            this.txtAciertos = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.ctrlAyuda1 = new Free1X2.UI.Controls.ctrlAyuda();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Bisque;
            this.panel1.Controls.Add(this.menuCondiciones2);
            this.panel1.Location = new System.Drawing.Point(0, 335);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(327, 43);
            this.panel1.TabIndex = 132;
            // 
            // menuCondiciones2
            // 
            this.menuCondiciones2.Alineacion = Free1X2.alignment.Horizontal;
            this.menuCondiciones2.AutoSize = true;
            this.menuCondiciones2.BackColor = System.Drawing.Color.Bisque;
            this.menuCondiciones2.BotonAbrir = true;
            this.menuCondiciones2.BotonAbrirEnabled = true;
            this.menuCondiciones2.BotonBorrar = true;
            this.menuCondiciones2.BotonBorrarEnabled = true;
            this.menuCondiciones2.BotonCancelar = true;
            this.menuCondiciones2.BotonCancelarEnabled = true;
            this.menuCondiciones2.BotonCopiar = true;
            this.menuCondiciones2.BotonCopiarEnabled = true;
            this.menuCondiciones2.BotonEstadisticas = true;
            this.menuCondiciones2.BotonEstadisticasEnabled = true;
            this.menuCondiciones2.BotonGuardar = true;
            this.menuCondiciones2.BotonGuardarEnabled = true;
            this.menuCondiciones2.BotonOk = true;
            this.menuCondiciones2.BotonOkEnabled = true;
            this.menuCondiciones2.BotonPegar = true;
            this.menuCondiciones2.BotonPegarEnabled = true;
            this.menuCondiciones2.Location = new System.Drawing.Point(87, 4);
            this.menuCondiciones2.MaximumSize = new System.Drawing.Size(237, 36);
            this.menuCondiciones2.Name = "menuCondiciones2";
            this.menuCondiciones2.NumBotones = 8;
            this.menuCondiciones2.Size = new System.Drawing.Size(237, 36);
            this.menuCondiciones2.TabIndex = 0;
            this.menuCondiciones2.BOk += new System.EventHandler(this.menuCondiciones1_BOk);
            this.menuCondiciones2.BEstadisticas += new System.EventHandler(this.menuCondiciones2_BEstadisticas);
            this.menuCondiciones2.BGuardar += new System.EventHandler(this.menuCondiciones1_BGuardar);
            this.menuCondiciones2.BAbrir += new System.EventHandler(this.menuCondiciones1_BAbrir);
            this.menuCondiciones2.BPegar += new System.EventHandler(this.menuCondiciones1_BPegar);
            this.menuCondiciones2.BBorrar += new System.EventHandler(this.menuCondiciones1_BBorrar);
            this.menuCondiciones2.BCancelar += new System.EventHandler(this.menuCondiciones1_BCancelar);
            this.menuCondiciones2.BCopiar += new System.EventHandler(this.menuCondiciones1_BCopiar);
            // 
            // cctrl
            // 
            this.cctrl.BackColor = System.Drawing.Color.Bisque;
            this.cctrl.Location = new System.Drawing.Point(12, 25);
            this.cctrl.Name = "cctrl";
            this.cctrl.Size = new System.Drawing.Size(217, 304);
            this.cctrl.TabIndex = 133;
            this.cctrl.Text = "containerControl1";
            // 
            // txtAciertos
            // 
            this.txtAciertos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAciertos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAciertos.Location = new System.Drawing.Point(236, 48);
            this.txtAciertos.Name = "txtAciertos";
            this.txtAciertos.Size = new System.Drawing.Size(80, 21);
            this.txtAciertos.TabIndex = 168;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Maroon;
            this.label1.Location = new System.Drawing.Point(242, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 20);
            this.label1.TabIndex = 167;
            this.label1.Text = "Aciertos";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ctrlAyuda1
            // 
            this.ctrlAyuda1.Location = new System.Drawing.Point(304, 0);
            this.ctrlAyuda1.Name = "ctrlAyuda1";
            this.ctrlAyuda1.Size = new System.Drawing.Size(20, 22);
            this.ctrlAyuda1.TabIndex = 169;
            this.ctrlAyuda1.TextoAyuda = "";
            // 
            // SimetriasFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(324, 376);
            this.ControlBox = false;
            this.Controls.Add(this.ctrlAyuda1);
            this.Controls.Add(this.txtAciertos);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cctrl);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(472, 448);
            this.MinimizeBox = false;
            this.Name = "SimetriasFrm";
            this.Text = "Simetrías";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        protected bool ObtenerSimetrias()
        {
            bool datosCorrectos = true;
            arraySimetrias = new List<Simetria>();
            for(int i = 0; i < cctrl.Controls.Count; i++)
            {
				
                CtrlSimetria ctrlSim = (CtrlSimetria)cctrl.Controls[i];
                if(ctrlSim.TxtSimetria.Text != "")
                {

                    if (CompruebaEntradas(ctrlSim.TxtSimetria.Text))
                    {
                        Simetria simetria = new Simetria(Utils.UtilidadesEntradasValores.ObtenerValoresSeparadosPorComas(ctrlSim.TxtSimetria.Text));
                        arraySimetrias.Add(simetria);
                        
                    }
                    else
                    {
                        datosCorrectos = false;        
                    }
                }

            } return datosCorrectos;
        }
        protected void LimpiarPantalla()
        {
            arraySimetrias = new List<Simetria>();
            for(int i = 0; i < cctrl.Controls.Count; i++)
            {
				
                CtrlSimetria ctrlSim = (CtrlSimetria)cctrl.Controls[i];
                ctrlSim.TxtSimetria.Text = "";
				
            }
        }
        protected bool CompruebaEntradas(string partidosSimetria)
        {
            bool esValida = false;
            if (partidosSimetria != "")
            {
                try
                {
                    string[] partes = partidosSimetria.Split(',');
                    for (int i = 0; i < partes.Length; i++)
                    {
                        string[] partes2 = partes[i].Split('-');
                        for (int j = 0; j < partes2.Length; j++)
                        {
                            int a = Convert.ToInt32(partes2[j]);
                            if ((a <= 0) || (a > VariablesGlobales.NumeroPartidos))
                            {
                                esValida = false;
                                break;
                            }
                            esValida = true;
                        }
                    }
                }
                catch
                {
                    esValida = false;
                }
            }
            return esValida;
        }
        protected bool ObtenerAciertos()
        {
            //Antes que nada borrar los aciertos
            arrayAciertos.Clear();
            bool datosCorrectos = true;
            if(txtAciertos.Text != "")
            {
                try
                {
                    string[] aciertosTemp = txtAciertos.Text.Split(',');
                    for (int i = 0; i < aciertosTemp.Length; i++)
                    {
                        if (aciertosTemp[i].LastIndexOf('-') == -1)
                        {
                            //Es un acierto individual

                            AñadirAcierto(Convert.ToInt32(aciertosTemp[i]));
                        }
                        else
                        {
                            string[] aciertosIntervalo = aciertosTemp[i].Split('-');
                            for (int j = Convert.ToInt32(aciertosIntervalo[0]); j <= Convert.ToInt32(aciertosIntervalo[1]); j++)
                            {
                                AñadirAcierto(j);
                            }
                        }
                    }
                }
                catch
                {
                    datosCorrectos = false;
                }
            } return datosCorrectos;
        }

        protected void AñadirAcierto(int acierto)
        {
            if((acierto >= 0)&&(acierto<=40))
            {
                arrayAciertos.Add(acierto);
            }
        }
        protected void MarcarValores()
        {
            string nombreFiltro = Filtro.Simetrias.ToString();
            filtro = (FiltroSimetrias)grupo.GetFiltro( nombreFiltro );
            if(filtro.ContieneDatos)
            {
                MarcarSimetrias(filtro.ArraySimetrias);
                MarcarAciertos(filtro.Aciertos);
            }
        }
        protected void MarcarValores(FiltroSimetrias filtroSim)
        {
            if (filtro.ContieneDatos)
            {
                MarcarSimetrias(filtroSim.ArraySimetrias);
                MarcarAciertos(filtroSim.Aciertos);
            }
        }
        protected void MarcarSimetrias(List<Simetria> arraySim)
        {
            for(int i = 0; i < arraySim.Count; i++)
            {
                Simetria simetria = arraySim[i];
                CtrlSimetria ctrlSim = (CtrlSimetria)cctrl.Controls[i];
                ctrlSim.TxtSimetria.Text = simetria.Partidos;
            }
        }
        protected void MarcarAciertos(string aciertos)
        {
            txtAciertos.Text = aciertos;
        }
        private FiltroSimetrias ObtenerFiltroTemporal()
        {
            FiltroSimetrias filtroTemp = new FiltroSimetrias();
            if ((ObtenerSimetrias()) && (ObtenerAciertos()))
            {
                //ObtenerAciertos();
                filtroTemp.ArraySimetrias = arraySimetrias;
                filtroTemp.ArrayAciertos = arrayAciertos;
                filtroTemp.Aciertos = txtAciertos.Text;
                if (filtroTemp.Aciertos == "")
                {
                    filtroTemp.Aciertos = "0";
                }
                filtroTemp.IsActive = filtroTemp.ContieneDatos;
            }
            else
            {
                MessageBox.Show("Verifique que ha introducido una simetria correcta: partido 1, partido 2, ... , partido n, \ny que ha introducido un número válido de aciertos", "Error");
            }
            return filtroTemp;
        }
        private void menuCondiciones1_BOk(object sender, EventArgs e)
        {
			
            string nombreFiltro = Filtro.Simetrias.ToString();
            filtro = (FiltroSimetrias)grupo.GetFiltro( nombreFiltro );
            if ((ObtenerSimetrias()) && (ObtenerAciertos()))
            {
                filtro.ArraySimetrias = arraySimetrias;
                filtro.ArrayAciertos = arrayAciertos;
                filtro.Aciertos = txtAciertos.Text;
                if (filtro.Aciertos == "")
                {
                    filtro.Aciertos = "0";
                }
                filtro.IsActive = filtro.ContieneDatos;
                FormPadre.analizador.GruposPartidos[FormPadre.pronosticos.GrupoPantalla].ActivaFiltro(filtro);
                CerrarVentana();
            }
            else
            {
                MessageBox.Show("Verifique que ha introducido una simetria correcta: (a,b) ó (a-b) ó (a,b-c), \ny que ha introducido un número válido de aciertos", "Error");
            }

        }
        private void menuCondiciones1_BBorrar(object sender, EventArgs e)
        {
            filtro =(FiltroSimetrias)grupo.GetFiltro("Simetrias");
            if(filtro.ContieneDatos)
            {
                if(MessageBox.Show("¿Borrar los datos del filtro?","Borrar condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
                    return;
            }
            filtro=new FiltroSimetrias();
            arraySimetrias = new List<Simetria>();
            arraySimetrias.Clear();
            txtAciertos.Text = "";
            LimpiarPantalla();
        }
        private void menuCondiciones1_BCancelar(object sender, EventArgs e)
        {
            CerrarVentana();
        }

        private void ActualizarDatos()
        {
            if ((ObtenerSimetrias()) && (ObtenerAciertos()))
            {
                ObtenerAciertos();
                filtro.ArraySimetrias = arraySimetrias;
                filtro.ArrayAciertos = arrayAciertos;
                filtro.Aciertos = txtAciertos.Text;
                if (filtro.Aciertos == "")
                {
                    filtro.Aciertos = "0";
                }
                filtro.IsActive = filtro.ContieneDatos;
                FormPadre.analizador.GruposPartidos[FormPadre.pronosticos.GrupoPantalla].ActivaFiltro(filtro);
            }
        }
        private void menuCondiciones1_BAbrir(object sender, EventArgs e)
        {
            ActualizarDatos();

            if (filtro.ContieneDatos)
            {
                if (MessageBox.Show("El filtro ya tiene datos introducidos. ¿Abrir igualmente?", "Abrir condición", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                    return;
            }
            filtro = new FiltroSimetrias();
            grupo.ActivaFiltro(filtro);
            OpenFileDialog abreCombDialog = new OpenFileDialog();
            abreCombDialog.InitialDirectory = "Condiciones\\";
            abreCombDialog.Filter = "Simetrias(*.sim)|*.sim|Simetrias(*.xml)|*.xml|Todos los archivos (*.*)|*.*";
            if (abreCombDialog.ShowDialog() == DialogResult.OK)
                abrir(abreCombDialog.FileName);
        }
        private void abrir(string nombreArchivo)
        {
            //leer combinacion desde archivo
            ArchivoCondiciones archComb = new ArchivoCondiciones();
            if (archComb.AbrirArchivoCombinacion(nombreArchivo))
            {
                Grupo g = archComb.LeeCondicion();
                filtro = (FiltroSimetrias)g.GetFiltro("Simetrias");
                MarcarValores(filtro);
            }
        }
        private void menuCondiciones1_BGuardar(object sender, EventArgs e)
        {
            // Lo primero, guarda los datos de pantalla al filtro
            ActualizarDatos();
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.InitialDirectory = "Condiciones\\";
            saveDialog.Filter = "Simetrias(*.sim)|*.sim|Simetrias(*.xml)|*.xml|Todos los archivos (*.*)|*.*";
            if (saveDialog.ShowDialog() == DialogResult.OK)
                guardar(saveDialog.FileName);
        }
        private void guardar(string nombreArchivo)
        {
            ArchivoCondiciones archComb = new ArchivoCondiciones();
            archComb.NombreArchivo = nombreArchivo;
            archComb.GuardaArchivo(filtro);
        }
        private void menuCondiciones1_BCopiar(object sender, EventArgs e)
        {
            // Lo primero, guarda los datos de pantalla al filtro
            ActualizarDatos();
            string nombreFichero = Application.StartupPath + "/Temp/" + "tmp.sim";
            guardar(nombreFichero);
            menuCondiciones2.BotonPegarEnabled = true;
        }
        private void menuCondiciones1_BPegar(object sender, EventArgs e)
        {
            ActualizarDatos();
            if (filtro.ContieneDatos)
            {
                if (MessageBox.Show("El filtro ya tiene datos introducidos. ¿Abrir igualmente?", "Abrir condición", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
                    return;
            }
            filtro = new FiltroSimetrias();
            if (filtro != null)
            {
                grupo.ActivaFiltro(filtro);
                LimpiarPantalla();
            }
            string nombreFichero = Application.StartupPath + "/Temp/" + "tmp.sim";
            abrir(nombreFichero);
        }
        private void compruebaPegar()
        {
            // Comprueba si el botón pegar es habilitable
            if (formHelper.ExisteFicheroTemporal("tmp.sim"))
                menuCondiciones2.BotonPegarEnabled = true;
            else
                menuCondiciones2.BotonPegarEnabled = false;
        }
        private void CerrarVentana()
        {
            Close();
        }
        private void Añadir_Enter(object sender, EventArgs e)
        {
            //Obtener el último control, su número y su posicion
            CtrlSimetria ctrlSim = (CtrlSimetria)cctrl.Controls[cctrl.Controls.Count-1];

            int posicionY = ctrlSim.Location.Y;
            int noControl = Convert.ToInt32( ctrlSim.LblNum.Text);
            //Eliminar el evento en el último control
            ctrlSim.TxtSimetria.Enter -= Añadir_Enter;
            CtrlSimetria ctrlSimetria = new CtrlSimetria(noControl+1);
            AñadirControl(ctrlSimetria, 0, posicionY+15);
        }

        private void menuCondiciones2_BEstadisticas(object sender, EventArgs e)
        {
            FiltroSimetrias filtroTemp = ObtenerFiltroTemporal();

            CalculadorEstadisticas calc = new CalculadorEstadisticas();

            List<Estadistica> lista = calc.EstadisticasFiltro(filtroTemp, Application.StartupPath + "/Ganadoras/");

            Estadisticas.VisorEstadisticas visor = new Estadisticas.VisorEstadisticas(lista);

            visor.ShowDialog();
        }
    }
}
