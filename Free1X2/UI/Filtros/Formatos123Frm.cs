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
using System.Windows.Forms;

using Free1X2.MotorCalculo;
using Free1X2.MotorCalculo.Estadisticas;
using Free1X2.EntradaSalida;
using Free1X2.UI.Controls;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI.Filtros
{
    /// <summary>
    /// Summary description for Formatos123Frm.
    /// </summary>
    public class Formatos123Frm : Form
    {
        private Panel panel1;
        private Label label41;
        private TextBox txtAciertos;
        protected List<Formato123> arrayFormatos = new List<Formato123>();
        protected List<int> arrayAciertos = new List<int>();
        private Grupo grupo;
        private MainForm parentFrm;
        private CheckBox chckPasoFijo;
        protected FiltroFormatos123 filtro;

        private MenuCondiciones menuCondiciones2;
        private ControlPorcentajes controlPorcentajes1;
        private GroupBox groupBox1;
        private GroupBox groupBox2;
        private TextBox txtColumna1x2;
        private Button btnTraducir;
        private TextBox txtColumna123;
        private Label label42;
        private Button btnAnalisis;
        private ContainerControl cctrl;
        private Label label1;
        private Label label2;
        private Label label3;
        private ctrlAyuda ctrlAyuda1;
        protected FormulariosHelper formHelper = new FormulariosHelper();

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        public Formatos123Frm(Grupo grupo, MainForm form)
        {
            InitializeComponent();
            this.grupo = grupo;
            filtro = (FiltroFormatos123)grupo.GetFiltro( Filtro.Formatos123.ToString());

            MarcarValores();
			
            parentFrm = form;
            compruebaPegar();
            LlenarControles(100);
            this.ctrlAyuda1.TextoAyuda = "Un Formato 123 es la representación de una\nsecuencia de signos en función de la valoración,\nde forma que el 1 corresponde al signo más valorado,\nel 2 al segundo signo más valorado\ny el 3 al signo menos valorado";

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
            int posicionY = 0 + (cctrl.Controls.Count * 16);
            int noControl = cctrl.Controls.Count + 1;
            for (int i = 0; i < noControles; i++)
            {
                CtrlFormato123 ctrlFormato = new CtrlFormato123();
                ctrlFormato.NumeroFormato = noControl.ToString();
                if (i == noControles - 1)
                {
                    //Es el último control, añadir evento
                    ctrlFormato.TxtFormato.Enter += Añadir_Enter;
                }
                AñadirControl(ctrlFormato, 0, posicionY);
                posicionY += 16;
                noControl ++;
            }
        }
        private void AñadirControl(CtrlFormato123 ctrlFormato, int posicionX, int posicionY)
        {
            ctrlFormato.Location = new Point(posicionX, posicionY);
            this.cctrl.Controls.Add(ctrlFormato);
            this.cctrl.AutoScroll = true;
        }
        protected bool CompruebaEntradas()
        {
            bool esValida = false;
            this.arrayFormatos.Clear();
            for(int i = 0; i < cctrl.Controls.Count; i++)
            {

                CtrlFormato123 ctrlFormato = (CtrlFormato123)cctrl.Controls[i];

                if ((ctrlFormato.Formato != "") && ((ctrlFormato.Formato.Length > 14) || (!CompruebaFormato(ctrlFormato.Formato)) || (!CompruebaAciertos(ctrlFormato.AciertosMin, ctrlFormato.AciertosMax))))
                {
                    esValida = false;
                    break;
                }
                else
                {

                    if (ctrlFormato.Formato != "")
                    {
                        Formato123 formato = new Formato123();
                        formato.Formato = ctrlFormato.Formato;
                        if(!this.chckPasoFijo.Checked)
                        {
                            //Permitimos repeticiones y se ha comprobado que la entrada es correcta (int)
                            formato.AciertosMax = Convert.ToInt32(ctrlFormato.AciertosMax);
                            formato.AciertosMin = Convert.ToInt32(ctrlFormato.AciertosMin);
                        }
                        else
                        {
                            //No las permitimos
                            formato.AciertosMax = 0;
                            formato.AciertosMin = 0;
                        }
                        AñadirFormato(formato);
                    }
                    esValida = true;
                }
            }
            return esValida;
        }
        protected bool CompruebaFormato(string formato)
        {
            bool esValido = false;
            char[] valores = formato.ToCharArray();
            for(int i = 0; i < valores.Length; i++)
            {
                if((valores[i] != '1')&&(valores[i] != '2')&&(valores[i] != '3'))
                {
                    esValido = false;
                    break;
                }
                else
                {
                    esValido = true;
                }
            }return esValido;
        }
        protected bool CompruebaColumna(string columna)
        {
            bool esValido = false;
            if (columna.Length == VariablesGlobales.NumeroPartidos)
            {
                char[] valores = columna.ToUpper().ToCharArray();
                for(int i = 0; i < valores.Length; i++)
                {
                    if((valores[i] != '1')&&(valores[i] != 'X')&&(valores[i] != '2'))
                    {
                        esValido = false;
                        break;
                    }
                    else
                    {
                        esValido = true;
                    }
                }
            }
            return esValido;
        }
        protected bool CompruebaAciertos(string aciertoMin, string aciertoMax)
        {
            try
            {
                if(this.chckPasoFijo.Checked)
                {
                    //No permitimos repeticiones, o sea que se permite que esté en blanco
                    return true;
                }
                else
                {
                    int acMin = Convert.ToInt32(aciertoMin);
                    int acMax = Convert.ToInt32(aciertoMax);
                    return true;
                }

				
            }
            catch
            {
                return false;
            }
        }
        protected void AñadirFormato(Formato123 formato)
        {
            if(!this.arrayFormatos.Contains(formato))
            {
                this.arrayFormatos.Add(formato);
            }
        }
        protected void AñadirAcierto(int acierto)
        {
            if((acierto >= 0)&&(acierto<=40))
            {
                this.arrayAciertos.Add(acierto);
            }
        }
        protected void ObtenerAciertosPermitidos()
        {
            this.arrayAciertos.Clear();
            if(this.txtAciertos.Text != "")
            {
                string[] aciertosTemp = txtAciertos.Text.Split(',');
                for(int i = 0; i<aciertosTemp.Length; i++)
                {
                    if(aciertosTemp[i].LastIndexOf('-')==-1)
                    {
                        //Es un acierto individual
						
                        AñadirAcierto(Convert.ToInt32(aciertosTemp[i]));
                    }
                    else
                    {
                        string[] aciertosIntervalo = aciertosTemp[i].Split('-');
                        for(int j = Convert.ToInt32(aciertosIntervalo[0]); j <= Convert.ToInt32(aciertosIntervalo[1]); j++)
                        {
                            AñadirAcierto(j);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            this.label41 = new System.Windows.Forms.Label();
            this.txtAciertos = new System.Windows.Forms.TextBox();
            this.chckPasoFijo = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnAnalisis = new System.Windows.Forms.Button();
            this.label42 = new System.Windows.Forms.Label();
            this.txtColumna123 = new System.Windows.Forms.TextBox();
            this.btnTraducir = new System.Windows.Forms.Button();
            this.txtColumna1x2 = new System.Windows.Forms.TextBox();
            this.cctrl = new System.Windows.Forms.ContainerControl();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.controlPorcentajes1 = new Free1X2.UI.Controls.ControlPorcentajes();
            this.ctrlAyuda1 = new Free1X2.UI.Controls.ctrlAyuda();
            this.panel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Bisque;
            this.panel1.Controls.Add(this.menuCondiciones2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 405);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(682, 48);
            this.panel1.TabIndex = 164;
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
            this.menuCondiciones2.BotonPegarEnabled = false;
            this.menuCondiciones2.Location = new System.Drawing.Point(352, 9);
            this.menuCondiciones2.Name = "menuCondiciones2";
            this.menuCondiciones2.NumBotones = 8;
            this.menuCondiciones2.Size = new System.Drawing.Size(306, 36);
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
            // label41
            // 
            this.label41.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label41.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label41.Location = new System.Drawing.Point(8, 16);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(115, 21);
            this.label41.TabIndex = 161;
            this.label41.Text = "Líneas Acertadas";
            this.label41.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtAciertos
            // 
            this.txtAciertos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtAciertos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtAciertos.Location = new System.Drawing.Point(129, 16);
            this.txtAciertos.Name = "txtAciertos";
            this.txtAciertos.Size = new System.Drawing.Size(95, 21);
            this.txtAciertos.TabIndex = 162;
            // 
            // chckPasoFijo
            // 
            this.chckPasoFijo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chckPasoFijo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chckPasoFijo.Location = new System.Drawing.Point(8, 40);
            this.chckPasoFijo.Name = "chckPasoFijo";
            this.chckPasoFijo.Size = new System.Drawing.Size(152, 24);
            this.chckPasoFijo.TabIndex = 163;
            this.chckPasoFijo.Text = "Ignorar Repeticiones";
            this.chckPasoFijo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.chckPasoFijo.CheckedChanged += new System.EventHandler(this.chckPasoFijo_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.chckPasoFijo);
            this.groupBox1.Controls.Add(this.txtAciertos);
            this.groupBox1.Controls.Add(this.label41);
            this.groupBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(427, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(231, 72);
            this.groupBox1.TabIndex = 165;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Aciertos";
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.btnAnalisis);
            this.groupBox2.Controls.Add(this.label42);
            this.groupBox2.Controls.Add(this.txtColumna123);
            this.groupBox2.Controls.Add(this.btnTraducir);
            this.groupBox2.Controls.Add(this.txtColumna1x2);
            this.groupBox2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(427, 92);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(231, 125);
            this.groupBox2.TabIndex = 166;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Utilidades";
            // 
            // btnAnalisis
            // 
            this.btnAnalisis.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnAnalisis.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAnalisis.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAnalisis.Location = new System.Drawing.Point(8, 88);
            this.btnAnalisis.Name = "btnAnalisis";
            this.btnAnalisis.Size = new System.Drawing.Size(217, 22);
            this.btnAnalisis.TabIndex = 167;
            this.btnAnalisis.Text = "Analizador de Formatos";
            this.btnAnalisis.UseVisualStyleBackColor = false;
            this.btnAnalisis.Click += new System.EventHandler(this.btnAnalisis_Click);
            // 
            // label42
            // 
            this.label42.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label42.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label42.Location = new System.Drawing.Point(8, 16);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(217, 20);
            this.label42.TabIndex = 166;
            this.label42.Text = "Traducir 1X2 a 123";
            this.label42.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtColumna123
            // 
            this.txtColumna123.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtColumna123.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtColumna123.Location = new System.Drawing.Point(129, 40);
            this.txtColumna123.MaxLength = 14;
            this.txtColumna123.Name = "txtColumna123";
            this.txtColumna123.Size = new System.Drawing.Size(95, 18);
            this.txtColumna123.TabIndex = 165;
            // 
            // btnTraducir
            // 
            this.btnTraducir.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnTraducir.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnTraducir.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTraducir.Location = new System.Drawing.Point(104, 40);
            this.btnTraducir.Name = "btnTraducir";
            this.btnTraducir.Size = new System.Drawing.Size(24, 18);
            this.btnTraducir.TabIndex = 164;
            this.btnTraducir.Text = ">";
            this.btnTraducir.UseVisualStyleBackColor = false;
            this.btnTraducir.Click += new System.EventHandler(this.btnTraducir_Click);
            // 
            // txtColumna1x2
            // 
            this.txtColumna1x2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtColumna1x2.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtColumna1x2.Location = new System.Drawing.Point(8, 40);
            this.txtColumna1x2.MaxLength = 14;
            this.txtColumna1x2.Name = "txtColumna1x2";
            this.txtColumna1x2.Size = new System.Drawing.Size(95, 18);
            this.txtColumna1x2.TabIndex = 163;
            // 
            // cctrl
            // 
            this.cctrl.BackColor = System.Drawing.Color.Bisque;
            this.cctrl.Location = new System.Drawing.Point(174, 52);
            this.cctrl.Name = "cctrl";
            this.cctrl.Size = new System.Drawing.Size(229, 332);
            this.cctrl.TabIndex = 167;
            this.cctrl.Text = "containerControl1";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(214, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 20);
            this.label1.TabIndex = 168;
            this.label1.Text = "Formato";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(316, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(33, 20);
            this.label2.TabIndex = 169;
            this.label2.Text = "Mín";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(345, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(33, 20);
            this.label3.TabIndex = 170;
            this.label3.Text = "Máx";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // controlPorcentajes1
            // 
            this.controlPorcentajes1.archivoPorcentajes = null;
            this.controlPorcentajes1.BackColor = System.Drawing.Color.Bisque;
            this.controlPorcentajes1.CaptionText = "  P O R C E N T A J E S";
            this.controlPorcentajes1.FormatoFicheroValoraciones = ((short)(0));
            this.controlPorcentajes1.Jornada = "01";
            this.controlPorcentajes1.Location = new System.Drawing.Point(8, 0);
            this.controlPorcentajes1.Name = "controlPorcentajes1";
            this.controlPorcentajes1.ReadOnly = false;
            this.controlPorcentajes1.Size = new System.Drawing.Size(160, 399);
            this.controlPorcentajes1.TabIndex = 0;
            this.controlPorcentajes1.Temporada = "2004/2005";
            // 
            // ctrlAyuda1
            // 
            this.ctrlAyuda1.Location = new System.Drawing.Point(659, 0);
            this.ctrlAyuda1.Name = "ctrlAyuda1";
            this.ctrlAyuda1.Size = new System.Drawing.Size(20, 22);
            this.ctrlAyuda1.TabIndex = 171;
            this.ctrlAyuda1.TextoAyuda = "";
            // 
            // Formatos123Frm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(682, 453);
            this.ControlBox = false;
            this.Controls.Add(this.ctrlAyuda1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cctrl);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.controlPorcentajes1);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Formatos123Frm";
            this.Text = "Formatos 123";
            this.Load += new System.EventHandler(this.Formatos123Frm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private void Formatos123Frm_Load(object sender, System.EventArgs e)
        {
		
        }
        protected void MarcarValores()
        {
            string nombreFiltro = Filtro.Formatos123.ToString();
            filtro = (FiltroFormatos123)grupo.GetFiltro( nombreFiltro );
            if(filtro.ContieneDatos)
            {
                MarcarFormatos(filtro.ArrayFormatos);
                MarcarAciertos(filtro.AciertosFiltro);
                MarcarValoracion(filtro.Valoracion);
                DeterminarRepeticiones(filtro.PasoFijo);
            }
        }
        protected void MarcarValores(FiltroFormatos123 filtro)
        {
            MarcarFormatos(filtro.ArrayFormatos);
            MarcarAciertos(filtro.AciertosFiltro);
            MarcarValoracion(filtro.Valoracion);
            DeterminarRepeticiones(filtro.PasoFijo);			
        }
        protected void MarcarFormatos(List<Formato123> formatos)
        {
            int posicionX = 0;
            int posicionY = 0;
            for(int i = 0; i < formatos.Count; i++)
            {
                Formato123 formato = formatos[i];
                int noControl = i + 1;
                CtrlFormato123 control = new CtrlFormato123(formato, noControl);

                AñadirControl(control, posicionX, posicionY);
                posicionY += 16;

            }
        }
        protected void MarcarAciertos(List<int> aciertos)
        {
            string aciertosString = "";
            for(int i = 0; i < aciertos.Count; i++)
            {
                int acierto = aciertos[i];
                aciertosString += acierto.ToString();
                if(i<aciertos.Count-1)
                {
                    aciertosString += ",";
                }
            }
            txtAciertos.Text = aciertosString;
        }
        protected void MarcarAciertos(string aciertos)
        {
            txtAciertos.Text = aciertos;
        }


        protected void DeterminarRepeticiones(bool noRep)
        {

            this.chckPasoFijo.Checked = noRep;

            if (chckPasoFijo.Checked)
            {
                DesactivarRepeticiones();
                label41.Text = "Aciertos Globales";
            }
            else
            {
                ActivarRepeticiones();
                label41.Text = "Líneas Acertadas";
            }
        }
        protected void MarcarValoracion(double[,] valoracion)
        {
            this.controlPorcentajes1.Valores = valoracion;
        }

        protected byte[,] TransformarValoracion(double[,] valoracion)
        {
            byte[,] valoresTransformados = new byte[VariablesGlobales.NumeroPartidos, 3];
            for (int i = 0; i < VariablesGlobales.NumeroPartidos; i++)
            {
                double[] valor = { valoracion[i, 0], valoracion[i, 1], valoracion[i, 2] };
                if ((valor[0] >= valor[1]) && (valor[0] >= valor[2]))
                {
                    if (valor[1] >= valor[2])
                    {
                        valoresTransformados[i, 0] = 4; //"1";
                        valoresTransformados[i, 1] = 2; //"2";
                        valoresTransformados[i, 2] = 1; //"3";
                    }
                    else if(valor[2] > valor[1])
                    {
                        valoresTransformados[i, 0] = 4; //"1";
                        valoresTransformados[i, 1] = 1; //"3";
                        valoresTransformados[i, 2] = 2; //"2";
                    }
                }
                else if ((valor[1] > valor[0]) && (valor[1] >= valor[2]))
                {
                    if (valor[0] >= valor[2])
                    {
                        valoresTransformados[i, 0] = 2; // "2";
                        valoresTransformados[i, 1] = 4; // "1";
                        valoresTransformados[i, 2] = 1; // "3";
                    }
                    else
                    {
                        valoresTransformados[i, 0] = 1; // "3";
                        valoresTransformados[i, 1] = 4; // "1";
                        valoresTransformados[i, 2] = 2; // "2";
                    }
                }
                else if ((valor[2] > valor[0]) && (valor[2] > valor[1]))
                {
                    if (valor[0] >= valor[1])
                    {
                        valoresTransformados[i, 0] = 2; // "2";
                        valoresTransformados[i, 1] = 1; // "3";
                        valoresTransformados[i, 2] = 4; // "1";

                    }
                    else
                    {
                        valoresTransformados[i, 0] = 1; //"3";
                        valoresTransformados[i, 1] = 2; //"2";
                        valoresTransformados[i, 2] = 4; //"1";
                    }
                }
            } return valoresTransformados;
        }
        private void DesactivarRepeticiones()
        {
            for (int i = 0; i < cctrl.Controls.Count; i++)
            {
                CtrlFormato123 control = (CtrlFormato123)cctrl.Controls[i];
                if (control.Formato != "")
                {
                    control.DesactivarDiferencias();
                }
            }
        }

        private void ActivarRepeticiones()
        {
            for(int i = 0; i<cctrl.Controls.Count; i++)
            {
                CtrlFormato123 control = (CtrlFormato123)cctrl.Controls[i];
                if (control.Formato != "")
                {
                    control.ActivarDiferencias();
                }
            }
        }
        private FiltroFormatos123 ObtenerFiltroTemporal()
        {
            FiltroFormatos123 filtroTemp = new FiltroFormatos123();
            CompruebaEntradas();
            ObtenerAciertosPermitidos();

            filtroTemp.ValoracionTranformada = TransformarValoracion(this.controlPorcentajes1.Valores);
            filtroTemp.Valoracion = this.controlPorcentajes1.Valores;
            if (this.arrayAciertos.Count > 0)
            {
                this.arrayAciertos.Sort();

                filtroTemp.AciertosMax = arrayAciertos[arrayAciertos.Count - 1];
                filtroTemp.AciertosMin = arrayAciertos[0];
            }
            filtroTemp.ArrayAciertos = this.arrayAciertos;
            filtroTemp.ArrayFormatos = this.arrayFormatos;
            filtroTemp.PasoFijo = this.chckPasoFijo.Checked;
            filtroTemp.IsActive = filtroTemp.NecesitaGuardar();
            filtroTemp.AciertosFiltro = txtAciertos.Text;
            if (filtroTemp.ContieneDatos == false)
            {
                filtroTemp.ContieneDatos = filtroTemp.NecesitaGuardar();
            }
            return filtroTemp;
        }
        private void CerrarVentana()
        {
            this.Close();
        }
        private void menuCondiciones1_BOk(object sender, System.EventArgs e)
        {
			
            string nombreFiltro = Filtro.Formatos123.ToString();
            filtro = (FiltroFormatos123)grupo.GetFiltro( nombreFiltro );
            CompruebaEntradas();
            ObtenerAciertosPermitidos();
	
            filtro.ValoracionTranformada = TransformarValoracion(this.controlPorcentajes1.Valores);
            filtro.Valoracion = this.controlPorcentajes1.Valores;
            if(this.arrayAciertos.Count > 0)
            {
                this.arrayAciertos.Sort();
			
                filtro.AciertosMax = arrayAciertos[arrayAciertos.Count - 1];
                filtro.AciertosMin = arrayAciertos[0];
            }
            filtro.ArrayAciertos = this.arrayAciertos;
            filtro.ArrayFormatos = this.arrayFormatos;
            filtro.PasoFijo = this.chckPasoFijo.Checked;
            filtro.IsActive = filtro.NecesitaGuardar();
            filtro.AciertosFiltro = txtAciertos.Text;
            if(filtro.ContieneDatos == false)
            {
                filtro.ContieneDatos = filtro.NecesitaGuardar();
            }
            FormPadre.analizador.GruposPartidos[FormPadre.pronosticos.GrupoPantalla].ActivaFiltro(filtro);
            CerrarVentana();
        }
        protected void ActualizarDatos()
        {
            if(CompruebaEntradas())
            {
                ObtenerAciertosPermitidos();

                filtro.IsActive = filtro.NecesitaGuardar();
                filtro.ValoracionTranformada = TransformarValoracion(this.controlPorcentajes1.Valores);
                filtro.Valoracion = this.controlPorcentajes1.Valores;
                if(this.arrayAciertos.Count > 0)
                {
                    this.arrayAciertos.Sort();
			
                    filtro.AciertosMax = arrayAciertos[arrayAciertos.Count - 1];
                    filtro.AciertosMin = arrayAciertos[0];
                }
                filtro.ArrayAciertos = this.arrayAciertos;
                filtro.ArrayFormatos = this.arrayFormatos;
                filtro.PasoFijo = this.chckPasoFijo.Checked;
                grupo.ActivaFiltro(filtro);
            }
        }
        protected void LimpiarPantalla()
        {

            this.cctrl.Controls.Clear();
            LlenarControles(100);


            txtAciertos.Text = "";
            chckPasoFijo.Checked = false;
        }

        private void menuCondiciones1_BBorrar(object sender, System.EventArgs e)
        {
            filtro =(FiltroFormatos123)grupo.GetFiltro("Formatos123");
            if(filtro.ContieneDatos==true)
            {
                if(MessageBox.Show("¿Borrar los datos del filtro?","Borrar condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
                    return;
            }
            filtro=new FiltroFormatos123();
            this.arrayAciertos.Clear();
            //grupo.ActivaFiltro(filtro);
            LimpiarPantalla();
        }
        private void menuCondiciones1_BCancelar(object sender, System.EventArgs e)
        {
            CerrarVentana();
        }
        private void menuCondiciones1_BAbrir(object sender, System.EventArgs e)
        {
            ActualizarDatos();
			
            if(filtro.ContieneDatos==true)
            {
                if(MessageBox.Show("El filtro ya tiene datos introducidos. ¿Abrir igualmente?","Abrir condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
                    return;
            }
            filtro = new FiltroFormatos123();
            grupo.ActivaFiltro(filtro);
            OpenFileDialog abreCombDialog = new OpenFileDialog();
            abreCombDialog.InitialDirectory = "Condiciones\\" ;
            abreCombDialog.Filter = "Formatos 123(*.123)|*.123|Formatos 123(*.xml)|*.xml|Todos los archivos (*.*)|*.*";
            if(abreCombDialog.ShowDialog() == DialogResult.OK)
                abrir(abreCombDialog.FileName);
        }
        private void menuCondiciones1_BGuardar(object sender, System.EventArgs e)
        {
            // Lo primero, guarda los datos de pantalla al filtro
            ActualizarDatos();
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.InitialDirectory = "Condiciones\\" ;
            saveDialog.Filter = "Formatos 123(*.123)|*.123|Formatos 123(*.xml)|*.xml|Todos los archivos (*.*)|*.*";
            if(saveDialog.ShowDialog() == DialogResult.OK)
                guardar(saveDialog.FileName);
        }
        private void guardar(string nombreArchivo)
        {
            ArchivoCondiciones archComb = new ArchivoCondiciones();
            archComb.NombreArchivo=nombreArchivo;
            archComb.GuardaArchivo(filtro);
        }
        private void abrir(string nombreArchivo)
        {
            //leer combinacion desde archivo
            ArchivoCondiciones archComb = new ArchivoCondiciones();
            if(archComb.AbrirArchivoCombinacion( nombreArchivo )==true)
            {
                Grupo g=archComb.LeeCondicion();
                filtro=(FiltroFormatos123)g.GetFiltro("Formatos123");
                MarcarValores(filtro);
            }
        }
        private void menuCondiciones1_BCopiar(object sender, System.EventArgs e)
        {
            // Lo primero, guarda los datos de pantalla al filtro
            ActualizarDatos();
            FiltroFormatos123 filtroTmp =(FiltroFormatos123)grupo.GetFiltro("Formatos123");
            // Crea un fichero temporal
            string nombreFichero=Application.StartupPath+"/Temp/"+"tmp.123";
            guardar(nombreFichero);
            menuCondiciones2.BotonPegarEnabled=true;
        }

        private void menuCondiciones1_BPegar(object sender, System.EventArgs e)
        {
            ActualizarDatos();
            if(filtro.NecesitaGuardar() && filtro.ContieneDatos==true)
            {
                if(MessageBox.Show("El filtro ya tiene datos introducidos. ¿Abrir igualmente?","Abrir condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
                    return;
            }
            filtro=new FiltroFormatos123();
            if(filtro!=null)
            {
                grupo.ActivaFiltro(filtro);
                LimpiarPantalla();
            }
            string nombreFichero=Application.StartupPath+"/Temp/"+"tmp.123";
            abrir(nombreFichero);
        }

        private void compruebaPegar()
        {
            // Comprueba si el botón pegar es habilitable
            if(formHelper.ExisteFicheroTemporal("tmp.123")==true)
                menuCondiciones2.BotonPegarEnabled=true;
            else
                menuCondiciones2.BotonPegarEnabled=false;
        }

        private void chckPasoFijo_CheckedChanged(object sender, System.EventArgs e)
        {
            DeterminarRepeticiones(chckPasoFijo.Checked);
        }

        private void btnTraducir_Click(object sender, System.EventArgs e)
        {
            if(CompruebaColumna(txtColumna1x2.Text.ToUpper()))
            {
                FiltroFormatos123 filtroFormatos123 = new FiltroFormatos123();
                filtroFormatos123.ValoracionTranformada = this.TransformarValoracion(this.controlPorcentajes1.Valores);
                filtroFormatos123.Valores = this.controlPorcentajes1.Valores;
                this.txtColumna123.Text = filtroFormatos123.Col1X2ToCol123(this.txtColumna1x2.Text.ToUpper());
            }
            else
            {
                txtColumna123.Text = "Error!!!!";
            }
        }

        private void btnAnalisis_Click(object sender, System.EventArgs e)
        {
            if(this.filtro.ArrayFormatos.Count > 0)
            {
                AnalisisFormatos123Frm analisisff = new AnalisisFormatos123Frm();
                analisisff.ArrayFormatos = this.filtro.ArrayFormatos;
                analisisff.ShowDialog();
            }
            else
            {
                MessageBox.Show("Guarde el Filtro Primero!!!");
            }
        }
        private void Añadir_Enter(object sender, System.EventArgs e)
        {
            //Obtener el último control, su número y su posicion
            CtrlFormato123 ctrlF123 = (CtrlFormato123)this.cctrl.Controls[cctrl.Controls.Count - 1];

            int posicionY = ctrlF123.Location.Y;
            int noControl = Convert.ToInt32(ctrlF123.NumeroFormato) + 1;
            //Eliminar el evento en el último control
            ctrlF123.TxtFormato.Enter -= new System.EventHandler(this.Añadir_Enter);
            CtrlFormato123 nuevoControlFormato123 = new CtrlFormato123();
            nuevoControlFormato123.NumeroFormato = noControl.ToString();
            nuevoControlFormato123.TxtFormato.Enter += new System.EventHandler(this.Añadir_Enter);
            AñadirControl(nuevoControlFormato123, 0, posicionY + 15);
        }

        private void menuCondiciones2_BEstadisticas(object sender, EventArgs e)
        {
            FiltroFormatos123 filtroTemp = ObtenerFiltroTemporal();
            CalculadorEstadisticas calc = new CalculadorEstadisticas();

            List<Estadistica> lista = calc.EstadisticasFiltro(filtroTemp, Application.StartupPath + "/Ganadoras/");

            Free1X2.UI.Estadisticas.VisorEstadisticas visor = new Free1X2.UI.Estadisticas.VisorEstadisticas(lista);

            visor.ShowDialog();
        }
    }
}
