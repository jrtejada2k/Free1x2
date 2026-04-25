// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2003 Luis Fernandez - luifer@onetel.net.uk
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
using System.Windows.Forms;
using System.Collections.Generic;

using Free1X2.MotorCalculo;
using Free1X2.MotorCalculo.Estadisticas;
using Free1X2.EntradaSalida;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI.Filtros
{
    /// <summary>
    /// Summary description for InterrupcionesFrm.
    /// </summary>
    public class InterrupcionesFrm : Form
    {
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private System.ComponentModel.Container components = null;
        private Controls.OptionNumTol0_14 stdGlobal;
        private Controls.OptionNumTol0_14 stdVar;
        private Controls.OptionNumTol0_14 std1;
        private Controls.OptionNumTol0_14 stdX;
        private Controls.OptionNumTol0_14 std2;
        private Label label6;
        private Label label7;
        private Label label8;
        private Label label9;
        private Label label10;
        private Label label11;
        private Controls.OptionNumTol0_14 intGlobSeg;
        private Controls.OptionNumTol0_14 intVarSeg;
        private Controls.OptionNumTol0_14 int1Seg;
        private Controls.OptionNumTol0_14 intXSeg;
        private Controls.OptionNumTol0_14 int2Seg;
        private Panel panel1;
        private Controls.MenuCondiciones menuCondiciones1;
        private FiltroInterrupciones filtro;
        private Grupo grupo;
        private Controls.ctrlAyuda ctrlAyuda1;
        private readonly MainForm parentFrm;
        protected FormulariosHelper formHelper = new FormulariosHelper();

        public InterrupcionesFrm(Grupo grupo, MainForm frm)
        {
            InitializeComponent();
            this.grupo = grupo;
            filtro=(FiltroInterrupciones)grupo.GetFiltro("NoInterrupciones");
            MarcarValores();
            compruebaPegar();
            formHelper.Redimensionar(this);
            parentFrm=frm;
            ctrlAyuda1.TextoAyuda = "Una interrupción es cada cambio\nde signo que se produce a lo largo\nde la columna";
            ctrlAyuda1.Location = new Point(Size.Width - (ctrlAyuda1.Width + 15), ctrlAyuda1.Location.Y);

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

        protected void MarcarValores()
        {
            stdGlobal.Valores = filtro.GetIntGlobales();
            stdVar.Valores = filtro.GetIntVar();
            std1.Valores = filtro.GetInt1();
            stdX.Valores = filtro.GetIntX();
            std2.Valores = filtro.GetInt2();

            intGlobSeg.Valores =  filtro.GetIntGlobalSeg();
            intVarSeg.Valores = filtro.GetIntVarSeg();
            int1Seg.Valores = filtro.GetInt1Seg();
            intXSeg.Valores = filtro.GetIntXSeg();
            int2Seg.Valores = filtro.GetInt2Seg();
        }

        protected bool NecesitaGuardarDatos()
        {
            bool necesitaGuardar = true;
			
            if(stdGlobal.Valores == "" && 
               stdVar.Valores == "" && 
               std1.Valores == "" && 
               stdX.Valores == "" && 
               std2.Valores == "" &&
               intGlobSeg.Valores == "" &&
               intVarSeg.Valores == "" &&
               int1Seg.Valores == "" &&
               intXSeg.Valores == "" &&
               int2Seg.Valores == "")
            {
                necesitaGuardar = false;
            }		
		
            return necesitaGuardar;
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.stdGlobal = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.stdVar = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.std1 = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.stdX = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.std2 = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.intGlobSeg = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.intVarSeg = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.int1Seg = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.intXSeg = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.int2Seg = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.panel1 = new System.Windows.Forms.Panel();
            this.menuCondiciones1 = new Free1X2.UI.Controls.MenuCondiciones();
            this.ctrlAyuda1 = new Free1X2.UI.Controls.ctrlAyuda();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Global";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(19, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Var";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(27, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(24, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "1";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(27, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 16);
            this.label4.TabIndex = 3;
            this.label4.Text = "X";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(27, 75);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(24, 16);
            this.label5.TabIndex = 4;
            this.label5.Text = "2";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // stdGlobal
            // 
            this.stdGlobal.BackColor = System.Drawing.Color.Wheat;
            this.stdGlobal.Location = new System.Drawing.Point(57, 8);
            this.stdGlobal.Maximo = 15;
            this.stdGlobal.Minimo = 0;
            this.stdGlobal.Name = "stdGlobal";
            this.stdGlobal.Size = new System.Drawing.Size(560, 16);
            this.stdGlobal.TabIndex = 7;
            this.stdGlobal.Valores = "";
            // 
            // stdVar
            // 
            this.stdVar.BackColor = System.Drawing.Color.Wheat;
            this.stdVar.Location = new System.Drawing.Point(57, 25);
            this.stdVar.Maximo = 15;
            this.stdVar.Minimo = 0;
            this.stdVar.Name = "stdVar";
            this.stdVar.Size = new System.Drawing.Size(560, 16);
            this.stdVar.TabIndex = 8;
            this.stdVar.Valores = "";
            // 
            // std1
            // 
            this.std1.BackColor = System.Drawing.Color.Wheat;
            this.std1.Location = new System.Drawing.Point(57, 42);
            this.std1.Maximo = 15;
            this.std1.Minimo = 0;
            this.std1.Name = "std1";
            this.std1.Size = new System.Drawing.Size(560, 16);
            this.std1.TabIndex = 9;
            this.std1.Valores = "";
            // 
            // stdX
            // 
            this.stdX.BackColor = System.Drawing.Color.Wheat;
            this.stdX.Location = new System.Drawing.Point(57, 59);
            this.stdX.Maximo = 15;
            this.stdX.Minimo = 0;
            this.stdX.Name = "stdX";
            this.stdX.Size = new System.Drawing.Size(560, 16);
            this.stdX.TabIndex = 10;
            this.stdX.Valores = "";
            // 
            // std2
            // 
            this.std2.BackColor = System.Drawing.Color.Wheat;
            this.std2.Location = new System.Drawing.Point(57, 76);
            this.std2.Maximo = 15;
            this.std2.Minimo = 0;
            this.std2.Name = "std2";
            this.std2.Size = new System.Drawing.Size(560, 16);
            this.std2.TabIndex = 11;
            this.std2.Valores = "";
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(8, 105);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(100, 16);
            this.label6.TabIndex = 12;
            this.label6.Text = "Seguidas:";
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(0, 124);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(51, 16);
            this.label7.TabIndex = 13;
            this.label7.Text = "Global";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(6, 141);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(45, 16);
            this.label8.TabIndex = 14;
            this.label8.Text = "Var";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(6, 175);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(45, 16);
            this.label9.TabIndex = 15;
            this.label9.Text = "X";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(6, 192);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(45, 16);
            this.label10.TabIndex = 16;
            this.label10.Text = "2";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label11
            // 
            this.label11.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(6, 158);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(45, 16);
            this.label11.TabIndex = 17;
            this.label11.Text = "1";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // intGlobSeg
            // 
            this.intGlobSeg.BackColor = System.Drawing.Color.Wheat;
            this.intGlobSeg.Location = new System.Drawing.Point(57, 124);
            this.intGlobSeg.Maximo = 15;
            this.intGlobSeg.Minimo = 0;
            this.intGlobSeg.Name = "intGlobSeg";
            this.intGlobSeg.Size = new System.Drawing.Size(560, 16);
            this.intGlobSeg.TabIndex = 18;
            this.intGlobSeg.Valores = "";
            // 
            // intVarSeg
            // 
            this.intVarSeg.BackColor = System.Drawing.Color.Wheat;
            this.intVarSeg.Location = new System.Drawing.Point(57, 141);
            this.intVarSeg.Maximo = 15;
            this.intVarSeg.Minimo = 0;
            this.intVarSeg.Name = "intVarSeg";
            this.intVarSeg.Size = new System.Drawing.Size(560, 16);
            this.intVarSeg.TabIndex = 19;
            this.intVarSeg.Valores = "";
            // 
            // int1Seg
            // 
            this.int1Seg.BackColor = System.Drawing.Color.Wheat;
            this.int1Seg.Location = new System.Drawing.Point(57, 158);
            this.int1Seg.Maximo = 15;
            this.int1Seg.Minimo = 0;
            this.int1Seg.Name = "int1Seg";
            this.int1Seg.Size = new System.Drawing.Size(560, 16);
            this.int1Seg.TabIndex = 20;
            this.int1Seg.Valores = "";
            // 
            // intXSeg
            // 
            this.intXSeg.BackColor = System.Drawing.Color.Wheat;
            this.intXSeg.Location = new System.Drawing.Point(57, 175);
            this.intXSeg.Maximo = 15;
            this.intXSeg.Minimo = 0;
            this.intXSeg.Name = "intXSeg";
            this.intXSeg.Size = new System.Drawing.Size(560, 16);
            this.intXSeg.TabIndex = 21;
            this.intXSeg.Valores = "";
            // 
            // int2Seg
            // 
            this.int2Seg.BackColor = System.Drawing.Color.Wheat;
            this.int2Seg.Location = new System.Drawing.Point(57, 192);
            this.int2Seg.Maximo = 15;
            this.int2Seg.Minimo = 0;
            this.int2Seg.Name = "int2Seg";
            this.int2Seg.Size = new System.Drawing.Size(560, 16);
            this.int2Seg.TabIndex = 22;
            this.int2Seg.Valores = "";
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Bisque;
            this.panel1.Controls.Add(this.menuCondiciones1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 221);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(647, 48);
            this.panel1.TabIndex = 37;
            // 
            // menuCondiciones1
            // 
            this.menuCondiciones1.Alineacion = Free1X2.alignment.Horizontal;
            this.menuCondiciones1.AutoSize = true;
            this.menuCondiciones1.BackColor = System.Drawing.Color.Bisque;
            this.menuCondiciones1.BotonAbrir = true;
            this.menuCondiciones1.BotonAbrirEnabled = true;
            this.menuCondiciones1.BotonBorrar = true;
            this.menuCondiciones1.BotonBorrarEnabled = true;
            this.menuCondiciones1.BotonCancelar = true;
            this.menuCondiciones1.BotonCancelarEnabled = true;
            this.menuCondiciones1.BotonCopiar = true;
            this.menuCondiciones1.BotonCopiarEnabled = true;
            this.menuCondiciones1.BotonEstadisticas = true;
            this.menuCondiciones1.BotonEstadisticasEnabled = true;
            this.menuCondiciones1.BotonGuardar = true;
            this.menuCondiciones1.BotonGuardarEnabled = true;
            this.menuCondiciones1.BotonOk = true;
            this.menuCondiciones1.BotonOkEnabled = true;
            this.menuCondiciones1.BotonPegar = true;
            this.menuCondiciones1.BotonPegarEnabled = false;
            this.menuCondiciones1.Location = new System.Drawing.Point(288, 8);
            this.menuCondiciones1.Name = "menuCondiciones1";
            this.menuCondiciones1.NumBotones = 8;
            this.menuCondiciones1.Size = new System.Drawing.Size(306, 36);
            this.menuCondiciones1.TabIndex = 3;
            this.menuCondiciones1.BOk += new System.EventHandler(this.menuCondiciones1_BOk);
            this.menuCondiciones1.BEstadisticas += new System.EventHandler(this.menuCondiciones1_BEstadisticas);
            this.menuCondiciones1.BGuardar += new System.EventHandler(this.menuCondiciones1_BGuardar);
            this.menuCondiciones1.BAbrir += new System.EventHandler(this.menuCondiciones1_BAbrir);
            this.menuCondiciones1.BPegar += new System.EventHandler(this.menuCondiciones1_BPegar);
            this.menuCondiciones1.BBorrar += new System.EventHandler(this.menuCondiciones1_BBorrar);
            this.menuCondiciones1.BCancelar += new System.EventHandler(this.menuCondiciones1_BCancelar);
            this.menuCondiciones1.BCopiar += new System.EventHandler(this.menuCondiciones1_BCopiar);
            // 
            // ctrlAyuda1
            // 
            this.ctrlAyuda1.Location = new System.Drawing.Point(622, 0);
            this.ctrlAyuda1.Name = "ctrlAyuda1";
            this.ctrlAyuda1.Size = new System.Drawing.Size(20, 22);
            this.ctrlAyuda1.TabIndex = 38;
            this.ctrlAyuda1.TextoAyuda = "";
            // 
            // InterrupcionesFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(647, 269);
            this.ControlBox = false;
            this.Controls.Add(this.ctrlAyuda1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.int2Seg);
            this.Controls.Add(this.intXSeg);
            this.Controls.Add(this.int1Seg);
            this.Controls.Add(this.intVarSeg);
            this.Controls.Add(this.intGlobSeg);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.std2);
            this.Controls.Add(this.stdX);
            this.Controls.Add(this.std1);
            this.Controls.Add(this.stdVar);
            this.Controls.Add(this.stdGlobal);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "InterrupcionesFrm";
            this.Text = "Interrupciones";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        protected void ActualizarDatos()
        {
            string todosValores = "0,1,2,3,4,5,6,7,8,9,10,11,12,13,14";
            filtro.ReinicializaValores();
            if( NecesitaGuardarDatos() )
            {
                if(filtro.ContieneDatos == false)
                {
                    //primera vez guardando datos. 
                    //Activar condicion.
                    filtro.IsActive = true;				
                }
                filtro.ContieneDatos = true;

                if( stdGlobal.Valores != "" )
                {
                    filtro.SetNoIntGlobales( stdGlobal.Valores );
                }
                else
                {
                    filtro.SetNoIntGlobales( todosValores );
                }

                if( stdVar.Valores != "" )
                {
                    filtro.SetNoIntVar( stdVar.Valores );
                }
                else
                {
                    filtro.SetNoIntVar( todosValores );
                }
				
                if( std1.Valores != "" )
                {
                    filtro.SetNoInt1( std1.Valores );
                }
                else
                {
                    filtro.SetNoInt1( todosValores );
                }
				
                if( stdX.Valores != "" )
                {
                    filtro.SetNoIntX( stdX.Valores );
                }
                else
                {
                    filtro.SetNoIntX( todosValores );
                }
				
                if( std2.Valores != "" )
                {
                    filtro.SetNoInt2( std2.Valores );
                }
                else
                {
                    filtro.SetNoInt2( todosValores );
                }

                //interrupciones seguidas...
                if( intGlobSeg.Valores != "" )
                {
                    filtro.SetNoIntGlobalSeg( intGlobSeg.Valores );
                }
                else
                {
                    filtro.SetNoIntGlobalSeg( todosValores );
                }

                if( intVarSeg.Valores != "" )
                {
                    filtro.SetNoIntVarSeg( intVarSeg.Valores );
                }
                else
                {
                    filtro.SetNoIntVarSeg( todosValores );
                }
				
                if( int1Seg.Valores != "" )
                {
                    filtro.SetNoInt1Seg( int1Seg.Valores );
                }
                else
                {
                    filtro.SetNoInt1Seg( todosValores );
                }
				
                if( intXSeg.Valores != "" )
                {
                    filtro.SetNoIntXSeg( intXSeg.Valores );
                }
                else
                {
                    filtro.SetNoIntXSeg( todosValores );
                }
				
                if( int2Seg.Valores != "" )
                {
                    filtro.SetNoInt2Seg( int2Seg.Valores );
                }
                else
                {
                    filtro.SetNoInt2Seg( todosValores );
                }
            }
            else
            {
                filtro.IsActive = false;
                filtro.ContieneDatos = false;
            }
        }
        protected FiltroInterrupciones ObtenerFiltroTemporal()
        {
            FiltroInterrupciones filtroTemp = new FiltroInterrupciones();
            string todosValores = "0,1,2,3,4,5,6,7,8,9,10,11,12,13,14";
            filtroTemp.ReinicializaValores();
            if (NecesitaGuardarDatos())
            {
                if (filtroTemp.ContieneDatos == false)
                {
                    //primera vez guardando datos. 
                    //Activar condicion.
                    filtroTemp.IsActive = true;
                }
                filtroTemp.ContieneDatos = true;

                if (stdGlobal.Valores != "")
                {
                    filtroTemp.SetNoIntGlobales(stdGlobal.Valores);
                }
                else
                {
                    filtroTemp.SetNoIntGlobales(todosValores);
                }

                if (stdVar.Valores != "")
                {
                    filtroTemp.SetNoIntVar(stdVar.Valores);
                }
                else
                {
                    filtroTemp.SetNoIntVar(todosValores);
                }

                if (std1.Valores != "")
                {
                    filtroTemp.SetNoInt1(std1.Valores);
                }
                else
                {
                    filtroTemp.SetNoInt1(todosValores);
                }

                if (stdX.Valores != "")
                {
                    filtroTemp.SetNoIntX(stdX.Valores);
                }
                else
                {
                    filtroTemp.SetNoIntX(todosValores);
                }

                if (std2.Valores != "")
                {
                    filtroTemp.SetNoInt2(std2.Valores);
                }
                else
                {
                    filtroTemp.SetNoInt2(todosValores);
                }

                //interrupciones seguidas...
                if (intGlobSeg.Valores != "")
                {
                    filtroTemp.SetNoIntGlobalSeg(intGlobSeg.Valores);
                }
                else
                {
                    filtroTemp.SetNoIntGlobalSeg(todosValores);
                }

                if (intVarSeg.Valores != "")
                {
                    filtroTemp.SetNoIntVarSeg(intVarSeg.Valores);
                }
                else
                {
                    filtroTemp.SetNoIntVarSeg(todosValores);
                }

                if (int1Seg.Valores != "")
                {
                    filtroTemp.SetNoInt1Seg(int1Seg.Valores);
                }
                else
                {
                    filtroTemp.SetNoInt1Seg(todosValores);
                }

                if (intXSeg.Valores != "")
                {
                    filtroTemp.SetNoIntXSeg(intXSeg.Valores);
                }
                else
                {
                    filtroTemp.SetNoIntXSeg(todosValores);
                }

                if (int2Seg.Valores != "")
                {
                    filtroTemp.SetNoInt2Seg(int2Seg.Valores);
                }
                else
                {
                    filtroTemp.SetNoInt2Seg(todosValores);
                }
            }
            else
            {
                filtroTemp.IsActive = false;
                filtroTemp.ContieneDatos = false;
            }
            return filtroTemp;
        }

        private void menuCondiciones1_BOk(object sender, EventArgs e)
        {
            ActualizarDatos();
            FormPadre.analizador.GruposPartidos[FormPadre.pronosticos.GrupoPantalla].ActivaFiltro(filtro);
            CerrarVentana();
        }

        private void menuCondiciones1_BCancelar(object sender, EventArgs e)
        {
            CerrarVentana();
        }

        private void menuCondiciones1_BAbrir(object sender, EventArgs e)
        {
            ActualizarDatos();
            if(filtro.ContieneDatos)
            {
                if(MessageBox.Show("El filtro ya tiene datos introducidos. ¿Abrir igualmente?","Abrir condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
                    return;
            }
            OpenFileDialog abreCombDialog = new OpenFileDialog();
            abreCombDialog.InitialDirectory = "Condiciones\\" ;
            abreCombDialog.Filter = "Interrupciones(*.int)|*.int|Interrupciones(*.xml)|*.xml|Todos los archivos (*.*)|*.*";
            if(abreCombDialog.ShowDialog() == DialogResult.OK)
                abrir(abreCombDialog.FileName);
        }

        private void menuCondiciones1_BGuardar(object sender, EventArgs e)
        {
            // Lo primero, guarda los datos de pantalla al filtro
            ActualizarDatos();
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.InitialDirectory = "Condiciones\\" ;
            saveDialog.Filter = "Interrupciones(*.int)|*.int|Interrupciones(*.xml)|*.xml|Todos los archivos (*.*)|*.*";
            if(saveDialog.ShowDialog() == DialogResult.OK)
                guardar(saveDialog.FileName);
        }

        private void abrir(string nombreArchivo)
        {
            //leer combinacion desde archivo
            ArchivoCondiciones archComb = new ArchivoCondiciones();
            if(archComb.AbrirArchivoCombinacion( nombreArchivo ))
            {
                grupo=archComb.LeeCondicion();
                filtro=(FiltroInterrupciones)grupo.GetFiltro("NoInterrupciones");
                MarcarValores();
            }
        }

        private void guardar(string nombreArchivo)
        {
            ArchivoCondiciones archComb = new ArchivoCondiciones();
            archComb.NombreArchivo=nombreArchivo;
            archComb.GuardaArchivo(filtro);
        }

        private void menuCondiciones1_BBorrar(object sender, EventArgs e)
        {
            ActualizarDatos();
            filtro=(FiltroInterrupciones)grupo.GetFiltro("NoInterrupciones");
            if(filtro.ContieneDatos)
            {
                if(MessageBox.Show("¿Borrar los datos del filtro?","Borrar condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
                    return;
            }
            filtro=new FiltroInterrupciones();
            MarcarValores();
        }

        private void menuCondiciones1_BCopiar(object sender, EventArgs e)
        {
            // Lo primero, guarda los datos de pantalla al filtro
            ActualizarDatos();
            // Crea un fichero temporal
            string nombreFichero=Application.StartupPath+"/Temp/"+"tmp.int";
            guardar(nombreFichero);
            menuCondiciones1.BotonPegarEnabled=true;
        }

        private void menuCondiciones1_BPegar(object sender, EventArgs e)
        {
            ActualizarDatos();
            if(filtro.ContieneDatos)
            {
                if(MessageBox.Show("El filtro ya tiene datos introducidos. ¿Abrir igualmente?","Abrir condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
                    return;
            }
            string nombreFichero=Application.StartupPath+"/Temp/"+"tmp.int";
            abrir(nombreFichero);
        }

        private void compruebaPegar()
        {
            // Comprueba si el botón pegar es habilitable
            if (formHelper.ExisteFicheroTemporal("tmp.int"))
                menuCondiciones1.BotonPegarEnabled=true;
            else
                menuCondiciones1.BotonPegarEnabled=false;
        }
        private void CerrarVentana()
        {
            Close();
        }

        private void menuCondiciones1_BEstadisticas(object sender, EventArgs e)
        {
            FiltroInterrupciones filtroTemp = ObtenerFiltroTemporal();
            CalculadorEstadisticas calc = new CalculadorEstadisticas();

            List<Estadistica> lista = calc.EstadisticasFiltro(filtroTemp, Application.StartupPath + "/Ganadoras/");

            Estadisticas.VisorEstadisticas visor = new Estadisticas.VisorEstadisticas(lista);

            visor.ShowDialog();
        }
    }
}
