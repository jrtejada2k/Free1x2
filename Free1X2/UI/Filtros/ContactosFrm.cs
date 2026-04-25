// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2003 Joan Duatis - duatis@coac.net
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

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI.Filtros
{
    /// <summary>
    /// Summary description for ContactosFrm.
    /// </summary>
    public class ContactosFrm : Form
    {
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private readonly MainForm parentFrm;
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components;
        private Controls.OptionNumTol0_14 std2V;
        private Controls.OptionNumTol0_14 std1V;
        private Controls.OptionNumTol0_14 stdXV;
        private Controls.OptionNumTol0_14 stdX2;
        private Controls.OptionNumTol0_14 stdVV;
        private Controls.OptionNumTol0_14 stdXX;
        private Controls.OptionNumTol0_14 std22;
        private Controls.OptionNumTol0_14 std11;
        private Controls.OptionNumTol0_14 std1X;
        private Controls.OptionNumTol0_14 std12;
        private Label label7;
        private Label label8;
        private Label label9;
        private Label label10;
        private Label label11;
        private Panel panel1;
        private Controls.MenuCondiciones menuCondiciones1;
        private Controls.ctrlAyuda ctrlAyuda1;
        private Button btnFiguras;
        protected List<long> figuras;
        protected string aciertosFiguras;
        FormulariosHelper formHelper = new FormulariosHelper();
		
        private Grupo grupo;

        public ContactosFrm(Grupo grupo, MainForm form)
        {
            InitializeComponent();
            this.grupo = grupo;
            MarcarValores();
            IndicarCondicionFiguras();
            parentFrm = form;
            formHelper.Redimensionar(this);
            compruebaPegar();
            ctrlAyuda1.TextoAyuda = "Se da un contacto entre dos signos\ncuando estos aparecen juntos en la columna,\nsin importar cual aparece en primer lugar";
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
            string nombreFiltro = Filtro.Contactos.ToString();
            FiltroContactos filtro = (FiltroContactos)grupo.GetFiltro( nombreFiltro );
					
            std1X.Valores = filtro.GetNum1X();
            std12.Valores = filtro.GetNum12();
            stdX2.Valores = filtro.GetNumX2();
            std11.Valores = filtro.GetNum11();
            stdXX.Valores = filtro.GetNumXX();

            std22.Valores =  filtro.GetNum22();
            std1V.Valores = filtro.GetNum1V();
            stdXV.Valores = filtro.GetNumXV();
            std2V.Valores = filtro.GetNum2V();
            stdVV.Valores = filtro.GetNumVV();

            figuras = filtro.Figuras;
            IndicarCondicionFiguras();
        }

        protected bool NecesitaGuardarDatos()
        {
            bool necesitaGuardar = true;
			
            if(std1X.Valores == "" && 
               std12.Valores == "" && 
               stdX2.Valores == "" && 
               std11.Valores == "" && 
               stdXX.Valores == "" &&
               std22.Valores == "" &&
               std1V.Valores == "" &&
               stdXV.Valores == "" &&
               std2V.Valores == "" &&
               stdVV.Valores == "")
            {
                if (figuras != null)
                {
                    if (figuras.Count == 0)
                    {
                        necesitaGuardar = false;
                    }
                }
                else
                {
                    necesitaGuardar = false;
                }
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
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.menuCondiciones1 = new Free1X2.UI.Controls.MenuCondiciones();
            this.btnFiguras = new System.Windows.Forms.Button();
            this.ctrlAyuda1 = new Free1X2.UI.Controls.ctrlAyuda();
            this.stdVV = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.stdX2 = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.std2V = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.stdXV = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.std1V = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.std22 = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.stdXX = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.std11 = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.std12 = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.std1X = new Free1X2.UI.Controls.OptionNumTol0_14();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(18, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "1X";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(18, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(24, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "12";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(18, 42);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(24, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "X2";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(18, 74);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 16);
            this.label4.TabIndex = 3;
            this.label4.Text = "11";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(18, 91);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(24, 16);
            this.label5.TabIndex = 4;
            this.label5.Text = "XX";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(18, 108);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(24, 16);
            this.label7.TabIndex = 13;
            this.label7.Text = "22";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(18, 140);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(24, 16);
            this.label8.TabIndex = 14;
            this.label8.Text = "1V";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(18, 174);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(24, 16);
            this.label9.TabIndex = 15;
            this.label9.Text = "2V";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(18, 191);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(24, 16);
            this.label10.TabIndex = 16;
            this.label10.Text = "VV";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            this.label11.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(18, 157);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(24, 16);
            this.label11.TabIndex = 17;
            this.label11.Text = "XV";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.Controls.Add(this.menuCondiciones1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 265);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(644, 48);
            this.panel1.TabIndex = 35;
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
            this.menuCondiciones1.BotonPegarEnabled = true;
            this.menuCondiciones1.Location = new System.Drawing.Point(262, 8);
            this.menuCondiciones1.Name = "menuCondiciones1";
            this.menuCondiciones1.NumBotones = 8;
            this.menuCondiciones1.Size = new System.Drawing.Size(306, 36);
            this.menuCondiciones1.TabIndex = 0;
            this.menuCondiciones1.BOk += new System.EventHandler(this.menuCondiciones1_BOk);
            this.menuCondiciones1.BEstadisticas += new System.EventHandler(this.menuCondiciones1_BEstadisticas);
            this.menuCondiciones1.BGuardar += new System.EventHandler(this.menuCondiciones1_BGuardar);
            this.menuCondiciones1.BAbrir += new System.EventHandler(this.menuCondiciones1_BAbrir);
            this.menuCondiciones1.BPegar += new System.EventHandler(this.menuCondiciones1_BPegar);
            this.menuCondiciones1.BBorrar += new System.EventHandler(this.menuCondiciones1_BBorrar);
            this.menuCondiciones1.BCancelar += new System.EventHandler(this.menuCondiciones1_BCancelar);
            this.menuCondiciones1.BCopiar += new System.EventHandler(this.menuCondiciones1_BCopiar);
            // 
            // btnFiguras
            // 
            this.btnFiguras.BackColor = System.Drawing.Color.LightSalmon;
            this.btnFiguras.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnFiguras.Location = new System.Drawing.Point(24, 221);
            this.btnFiguras.Name = "btnFiguras";
            this.btnFiguras.Size = new System.Drawing.Size(75, 23);
            this.btnFiguras.TabIndex = 37;
            this.btnFiguras.Text = "Figuras";
            this.btnFiguras.UseVisualStyleBackColor = false;
            this.btnFiguras.Click += new System.EventHandler(this.btnFiguras_Click);
            // 
            // ctrlAyuda1
            // 
            this.ctrlAyuda1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ctrlAyuda1.Location = new System.Drawing.Point(617, 0);
            this.ctrlAyuda1.Name = "ctrlAyuda1";
            this.ctrlAyuda1.Size = new System.Drawing.Size(16, 16);
            this.ctrlAyuda1.TabIndex = 36;
            this.ctrlAyuda1.TextoAyuda = "";
            // 
            // stdVV
            // 
            this.stdVV.BackColor = System.Drawing.Color.Wheat;
            this.stdVV.Location = new System.Drawing.Point(48, 191);
            this.stdVV.Maximo = 15;
            this.stdVV.Minimo = 0;
            this.stdVV.Name = "stdVV";
            this.stdVV.Size = new System.Drawing.Size(563, 16);
            this.stdVV.TabIndex = 34;
            this.stdVV.Valores = "0,1,2,3,4,5,6,7,8,9,10,11,12,13";
            // 
            // stdX2
            // 
            this.stdX2.BackColor = System.Drawing.Color.Wheat;
            this.stdX2.Location = new System.Drawing.Point(48, 42);
            this.stdX2.Maximo = 15;
            this.stdX2.Minimo = 0;
            this.stdX2.Name = "stdX2";
            this.stdX2.Size = new System.Drawing.Size(563, 16);
            this.stdX2.TabIndex = 33;
            this.stdX2.Valores = "0,1,2,3,4,5,6,7,8,9,10,11,12,13";
            // 
            // std2V
            // 
            this.std2V.BackColor = System.Drawing.Color.Wheat;
            this.std2V.Location = new System.Drawing.Point(48, 174);
            this.std2V.Maximo = 15;
            this.std2V.Minimo = 0;
            this.std2V.Name = "std2V";
            this.std2V.Size = new System.Drawing.Size(563, 16);
            this.std2V.TabIndex = 32;
            this.std2V.Valores = "0,1,2,3,4,5,6,7,8,9,10,11,12,13";
            // 
            // stdXV
            // 
            this.stdXV.BackColor = System.Drawing.Color.Wheat;
            this.stdXV.Location = new System.Drawing.Point(48, 157);
            this.stdXV.Maximo = 15;
            this.stdXV.Minimo = 0;
            this.stdXV.Name = "stdXV";
            this.stdXV.Size = new System.Drawing.Size(563, 16);
            this.stdXV.TabIndex = 31;
            this.stdXV.Valores = "0,1,2,3,4,5,6,7,8,9,10,11,12,13";
            // 
            // std1V
            // 
            this.std1V.BackColor = System.Drawing.Color.Wheat;
            this.std1V.Location = new System.Drawing.Point(48, 140);
            this.std1V.Maximo = 15;
            this.std1V.Minimo = 0;
            this.std1V.Name = "std1V";
            this.std1V.Size = new System.Drawing.Size(563, 16);
            this.std1V.TabIndex = 30;
            this.std1V.Valores = "0,1,2,3,4,5,6,7,8,9,10,11,12,13";
            // 
            // std22
            // 
            this.std22.BackColor = System.Drawing.Color.Wheat;
            this.std22.Location = new System.Drawing.Point(48, 108);
            this.std22.Maximo = 15;
            this.std22.Minimo = 0;
            this.std22.Name = "std22";
            this.std22.Size = new System.Drawing.Size(563, 16);
            this.std22.TabIndex = 29;
            this.std22.Valores = "0,1,2,3,4,5,6,7,8,9,10,11,12,13";
            // 
            // stdXX
            // 
            this.stdXX.BackColor = System.Drawing.Color.Wheat;
            this.stdXX.Location = new System.Drawing.Point(48, 91);
            this.stdXX.Maximo = 15;
            this.stdXX.Minimo = 0;
            this.stdXX.Name = "stdXX";
            this.stdXX.Size = new System.Drawing.Size(563, 16);
            this.stdXX.TabIndex = 28;
            this.stdXX.Valores = "0,1,2,3,4,5,6,7,8,9,10,11,12,13";
            // 
            // std11
            // 
            this.std11.BackColor = System.Drawing.Color.Wheat;
            this.std11.Location = new System.Drawing.Point(48, 74);
            this.std11.Maximo = 15;
            this.std11.Minimo = 0;
            this.std11.Name = "std11";
            this.std11.Size = new System.Drawing.Size(563, 16);
            this.std11.TabIndex = 27;
            this.std11.Valores = "0,1,2,3,4,5,6,7,8,9,10,11,12,13";
            // 
            // std12
            // 
            this.std12.BackColor = System.Drawing.Color.Wheat;
            this.std12.Location = new System.Drawing.Point(48, 25);
            this.std12.Maximo = 15;
            this.std12.Minimo = 0;
            this.std12.Name = "std12";
            this.std12.Size = new System.Drawing.Size(563, 16);
            this.std12.TabIndex = 24;
            this.std12.Valores = "0,1,2,3,4,5,6,7,8,9,10,11,12,13";
            // 
            // std1X
            // 
            this.std1X.BackColor = System.Drawing.Color.Wheat;
            this.std1X.Location = new System.Drawing.Point(48, 8);
            this.std1X.Maximo = 15;
            this.std1X.Minimo = 0;
            this.std1X.Name = "std1X";
            this.std1X.Size = new System.Drawing.Size(563, 16);
            this.std1X.TabIndex = 23;
            this.std1X.Valores = "0,1,2,3,4,5,6,7,8,9,10,11,12,13";
            // 
            // ContactosFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(644, 313);
            this.ControlBox = false;
            this.Controls.Add(this.btnFiguras);
            this.Controls.Add(this.ctrlAyuda1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.stdVV);
            this.Controls.Add(this.stdX2);
            this.Controls.Add(this.std2V);
            this.Controls.Add(this.stdXV);
            this.Controls.Add(this.std1V);
            this.Controls.Add(this.std22);
            this.Controls.Add(this.stdXX);
            this.Controls.Add(this.std11);
            this.Controls.Add(this.std12);
            this.Controls.Add(this.std1X);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ContactosFrm";
            this.Text = "Contactos";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion

        private void guardarDatos(FiltroContactos filtro)
        {
            string todosValores = "";
            for(int i=std1X.Minimo;i<=std1X.Maximo;i++)
            {
                todosValores +=i+",";
            }
            todosValores=todosValores.Substring(0,todosValores.Length-1);
            if( NecesitaGuardarDatos() )
            {
                if(!filtro.ContieneDatos)
                {
                    //primera vez guardando datos. 
                    //Activar condicion.
                    filtro.IsActive = true;				
                }
				
                filtro.ContieneDatos = true;
                filtro.Inicializa();

                if( std1X.Valores != "" )
                {
                    filtro.SetNum1X( std1X.Valores );
                }
                else
                {
                    filtro.SetNum1X( todosValores );
                }

                if( std12.Valores != "" )
                {
                    filtro.SetNum12( std12.Valores );
                }
                else
                {
                    filtro.SetNum12( todosValores );
                }
				
                if( stdX2.Valores != "" )
                {
                    filtro.SetNumX2( stdX2.Valores );
                }
                else
                {
                    filtro.SetNumX2( todosValores );
                }
				
                if( std11.Valores != "" )
                {
                    filtro.SetNum11( std11.Valores );
                }
                else
                {
                    filtro.SetNum11( todosValores );
                }
				
                if( stdXX.Valores != "" )
                {
                    filtro.SetNumXX( stdXX.Valores );
                }
                else
                {
                    filtro.SetNumXX( todosValores );
                }

                if( std22.Valores != "" )
                {
                    filtro.SetNum22( std22.Valores );
                }
                else
                {
                    filtro.SetNum22( todosValores );
                }

                if( std1V.Valores != "" )
                {
                    filtro.SetNum1V( std1V.Valores );
                }
                else
                {
                    filtro.SetNum1V( todosValores );
                }
				
                if( stdXV.Valores != "" )
                {
                    filtro.SetNumXV( stdXV.Valores );
                }
                else
                {
                    filtro.SetNumXV( todosValores );
                }
				
                if( std2V.Valores != "" )
                {
                    filtro.SetNum2V( std2V.Valores );
                }
                else
                {
                    filtro.SetNum2V( todosValores );
                }
				
                if( stdVV.Valores != "" )
                {
                    filtro.SetNumVV( stdVV.Valores );
                }
                else
                {
                    filtro.SetNumVV( todosValores );
                }
                if (figuras != null)
                {
                    if (figuras.Count > 0)
                    {
                        //Guardar y activar las figuras
                        filtro.Figuras = figuras;
                    }
                }
            }
            else
            {
                filtro.IsActive = false;
                filtro.ContieneDatos = false;
            }
        }

        private void menuCondiciones1_BOk(object sender, EventArgs e)
        {
            string nombreFiltro = Filtro.Contactos.ToString();
            FiltroContactos filtro = (FiltroContactos)grupo.GetFiltro( nombreFiltro );
            guardarDatos(filtro);
            filtro.UsaFiguras();
            FormPadre.analizador.GruposPartidos[FormPadre.pronosticos.GrupoPantalla].ActivaFiltro(filtro);
            CerrarVentana();
        }

        private void menuCondiciones1_BCancelar(object sender, EventArgs e)
        {
            CerrarVentana();
        }

        private void menuCondiciones1_BAbrir(object sender, EventArgs e)
        {
            IFiltro filtro=grupo.GetFiltro("Contactos");
            if(filtro.ContieneDatos)
            {
                if(MessageBox.Show("El filtro ya tiene datos introducidos. ¿Abrir igualmente?","Abrir condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
                    return;
            }
            OpenFileDialog abreCombDialog = new OpenFileDialog();
            abreCombDialog.InitialDirectory = "Condiciones\\" ;
            abreCombDialog.Filter = "Contactos(*.cont)|*.cont|Contactos(*.xml)|*.xml|Todos los archivos (*.*)|*.*";
            if(abreCombDialog.ShowDialog() == DialogResult.OK)
            {
                abrir(abreCombDialog.FileName);
            }
        }

        private void menuCondiciones1_BGuardar(object sender, EventArgs e)
        {
            // Lo primero, guarda los datos de pantalla al filtro
            string nombreFiltro = Filtro.Contactos.ToString();
            FiltroContactos f = (FiltroContactos)grupo.GetFiltro( nombreFiltro );
            guardarDatos(f);
            SaveFileDialog saveDialog = new SaveFileDialog();
            saveDialog.InitialDirectory = "Condiciones\\" ;
            saveDialog.Filter = "Contactos(*.cont)|*.cont|Contactos(*.xml)|*.xml|Todos los archivos (*.*)|*.*";
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
                MarcarValores();
            }
        }

        private void guardar(string nombreArchivo)
        {
            ArchivoCondiciones archComb = new ArchivoCondiciones();
            archComb.NombreArchivo=nombreArchivo;
            IFiltro filtro=grupo.GetFiltro("Contactos");
            archComb.GuardaArchivo(filtro);
        }

        private void menuCondiciones1_BBorrar(object sender, EventArgs e)
        {
            if(MessageBox.Show("¿Borrar los datos del filtro?","Borrar condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
                return;
            Grupo g=new Grupo();
            grupo.ActivaFiltro(g,"Contactos",true);
            MarcarValores();
        }

        private void menuCondiciones1_BCopiar(object sender, EventArgs e)
        {
            // Lo primero, guarda los datos de pantalla al filtro
            string nombreFiltro = Filtro.Contactos.ToString();
            FiltroContactos f = (FiltroContactos)grupo.GetFiltro( nombreFiltro );
            guardarDatos(f);
            
            // Crea un fichero temporal
            string nombreFichero=Application.StartupPath+"/Temp/"+"tmp.cont";
            guardar(nombreFichero);
            menuCondiciones1.BotonPegarEnabled=true;
        }

        private void menuCondiciones1_BPegar(object sender, EventArgs e)
        {
            IFiltro filtro=grupo.GetFiltro("Contactos");
            if(filtro.ContieneDatos)
            {
                if(MessageBox.Show("El filtro ya tiene datos introducidos. ¿Pegar igualmente?","Pegar condición",MessageBoxButtons.YesNo,MessageBoxIcon.Question,MessageBoxDefaultButton.Button2)==DialogResult.No)
                    return;
            }
            string nombreFichero=Application.StartupPath+"/Temp/"+"tmp.cont";
            abrir(nombreFichero);
        }

        private void compruebaPegar()
        {
            // Comprueba si el botón pegar es habilitable
            if (formHelper.ExisteFicheroTemporal("tmp.cont"))
                menuCondiciones1.BotonPegarEnabled=true;
            else
                menuCondiciones1.BotonPegarEnabled=false;
        }
        private void CerrarVentana()
        {
            Close();
        }
        protected void IndicarCondicionFiguras()
        {
            if (figuras != null)
            {
                if (figuras.Count > 0)
                {
                    btnFiguras.BackColor = Color.LightGreen;
                }
                else
                {
                    btnFiguras.BackColor = Color.Wheat;
                }
            }
            else
            {
                btnFiguras.BackColor = Color.Wheat;
            }
        }
        private void btnFiguras_Click(object sender, EventArgs e)
        {
            if (figuras == null)
            {
                figuras = new List<long>();
            }
            FigurasFiltrosFrm figurasFiltrosFrm = new FigurasFiltrosFrm(figuras, 10, new FiltroContactos());
            figurasFiltrosFrm.ShowDialog();
            IndicarCondicionFiguras();
        }

        private void menuCondiciones1_BEstadisticas(object sender, EventArgs e)
        {
            FiltroContactos filtroTemp = new FiltroContactos();

            guardarDatos(filtroTemp);
            filtroTemp.UsaFiguras();

            CalculadorEstadisticas calc = new CalculadorEstadisticas();

            List<Estadistica> lista = calc.EstadisticasFiltro(filtroTemp, Application.StartupPath + "/Ganadoras/");

            Estadisticas.VisorEstadisticas visor = new Estadisticas.VisorEstadisticas(lista);

            visor.ShowDialog();
        }
    }
}
