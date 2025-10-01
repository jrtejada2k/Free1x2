// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2007 Morrison - morrison.ne@gmail.com
// Copyright (C) Toni Moreno
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
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using Free1X2.EntradaSalida;

namespace Free1X2.UI
{
	/// <summary>
	/// Descripción breve de ConfiguracionFrm.
	/// </summary>
	public class ConfiguracionFrm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox txtValorTriples;
		private System.Windows.Forms.TextBox txtValorDobles;
		private System.Windows.Forms.TextBox txtValorFijos;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox tblim6;
		private System.Windows.Forms.TextBox tblim5;
		private System.Windows.Forms.TextBox tblim4;
		private System.Windows.Forms.TextBox tblim3;
		private System.Windows.Forms.TextBox tblim2;
		private System.Windows.Forms.TextBox tblim1;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox txRecaudacion;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox txtNumPartidos;
		private System.Windows.Forms.TextBox txtSeparador;
		private System.Windows.Forms.TextBox txtDesplazamiento;
		private System.Windows.Forms.TextBox txtPct14;
		private System.Windows.Forms.TextBox txtPrApuesta;
		private System.Windows.Forms.Button btnGuardar;
		private System.Windows.Forms.Button btnToolBar;
        private Label label14;
        private CheckBox chkActualizador;
        private Label label15;
        private TextBox txtMoneda;
        private Label label16;
        private ComboBox cbbIdioma;
        private CheckBox chbSalir;
        private Label label17;
		/// <summary>
		/// Variable del diseñador requerida.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ConfiguracionFrm()
		{
			InitializeComponent();
            ObtenerListaDeIdiomas();
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}
        private void ObtenerListaDeIdiomas()
        {
            ArchivoIdioma ar = new ArchivoIdioma();
            string[] lista = ar.ObtenerListaDeIdiomasDisponibles();
            cbbIdioma.Items.Add("es-ES");
            for (int i = 0; i < lista.Length; i++)
            {
                cbbIdioma.Items.Add(lista[i]);
            }
        }
        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        protected override void Dispose(bool disposing)
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

		#region Código generado por el Diseñador de Windows Forms
		/// <summary>
		/// Método necesario para admitir el Diseñador. No se puede modificar
		/// el contenido del método con el editor de código.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConfiguracionFrm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtNumPartidos = new System.Windows.Forms.TextBox();
            this.txtSeparador = new System.Windows.Forms.TextBox();
            this.txtDesplazamiento = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.txtValorTriples = new System.Windows.Forms.TextBox();
            this.txtValorDobles = new System.Windows.Forms.TextBox();
            this.txtValorFijos = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tblim6 = new System.Windows.Forms.TextBox();
            this.tblim5 = new System.Windows.Forms.TextBox();
            this.tblim4 = new System.Windows.Forms.TextBox();
            this.tblim3 = new System.Windows.Forms.TextBox();
            this.tblim2 = new System.Windows.Forms.TextBox();
            this.tblim1 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtPct14 = new System.Windows.Forms.TextBox();
            this.txtPrApuesta = new System.Windows.Forms.TextBox();
            this.txRecaudacion = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.btnToolBar = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.chkActualizador = new System.Windows.Forms.CheckBox();
            this.label15 = new System.Windows.Forms.Label();
            this.txtMoneda = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.cbbIdioma = new System.Windows.Forms.ComboBox();
            this.chbSalir = new System.Windows.Forms.CheckBox();
            this.label17 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(40, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(210, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "Nº de partidos en boleto";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(40, 54);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(210, 21);
            this.label2.TabIndex = 1;
            this.label2.Text = "Separador de partidos";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Maroon;
            this.label3.Location = new System.Drawing.Point(16, 86);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(245, 21);
            this.label3.TabIndex = 2;
            this.label3.Text = "Desplazamiento de grupo/condición";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtNumPartidos
            // 
            this.txtNumPartidos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNumPartidos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNumPartidos.Location = new System.Drawing.Point(267, 30);
            this.txtNumPartidos.MaxLength = 2;
            this.txtNumPartidos.Name = "txtNumPartidos";
            this.txtNumPartidos.Size = new System.Drawing.Size(24, 21);
            this.txtNumPartidos.TabIndex = 3;
            this.txtNumPartidos.Text = "14";
            this.txtNumPartidos.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtSeparador
            // 
            this.txtSeparador.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSeparador.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSeparador.Location = new System.Drawing.Point(267, 54);
            this.txtSeparador.Name = "txtSeparador";
            this.txtSeparador.Size = new System.Drawing.Size(96, 21);
            this.txtSeparador.TabIndex = 4;
            this.txtSeparador.Text = "5,9,12";
            // 
            // txtDesplazamiento
            // 
            this.txtDesplazamiento.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtDesplazamiento.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDesplazamiento.Location = new System.Drawing.Point(267, 86);
            this.txtDesplazamiento.MaxLength = 2;
            this.txtDesplazamiento.Name = "txtDesplazamiento";
            this.txtDesplazamiento.Size = new System.Drawing.Size(32, 21);
            this.txtDesplazamiento.TabIndex = 5;
            this.txtDesplazamiento.Text = "3";
            this.txtDesplazamiento.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.DarkSalmon;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(322, 444);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(120, 32);
            this.button1.TabIndex = 26;
            this.button1.Text = "Volver";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGuardar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardar.Image = ((System.Drawing.Image)(resources.GetObject("btnGuardar.Image")));
            this.btnGuardar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGuardar.Location = new System.Drawing.Point(199, 444);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(120, 32);
            this.btnGuardar.TabIndex = 25;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Maroon;
            this.label4.Location = new System.Drawing.Point(16, 126);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(210, 21);
            this.label4.TabIndex = 27;
            this.label4.Text = "Puntuación en CPs";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtValorTriples
            // 
            this.txtValorTriples.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtValorTriples.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtValorTriples.Location = new System.Drawing.Point(363, 126);
            this.txtValorTriples.Name = "txtValorTriples";
            this.txtValorTriples.Size = new System.Drawing.Size(40, 21);
            this.txtValorTriples.TabIndex = 33;
            // 
            // txtValorDobles
            // 
            this.txtValorDobles.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtValorDobles.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtValorDobles.Location = new System.Drawing.Point(315, 126);
            this.txtValorDobles.Name = "txtValorDobles";
            this.txtValorDobles.Size = new System.Drawing.Size(40, 21);
            this.txtValorDobles.TabIndex = 32;
            // 
            // txtValorFijos
            // 
            this.txtValorFijos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtValorFijos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtValorFijos.Location = new System.Drawing.Point(267, 126);
            this.txtValorFijos.Name = "txtValorFijos";
            this.txtValorFijos.Size = new System.Drawing.Size(40, 21);
            this.txtValorFijos.TabIndex = 31;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(363, 110);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 16);
            this.label5.TabIndex = 30;
            this.label5.Text = "Triples";
            this.label5.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(315, 110);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(40, 16);
            this.label6.TabIndex = 29;
            this.label6.Text = "Dobles";
            this.label6.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(267, 110);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(40, 16);
            this.label7.TabIndex = 28;
            this.label7.Text = "Fijos";
            this.label7.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // tblim6
            // 
            this.tblim6.BackColor = System.Drawing.Color.White;
            this.tblim6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tblim6.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tblim6.Location = new System.Drawing.Point(427, 158);
            this.tblim6.MaxLength = 2;
            this.tblim6.Name = "tblim6";
            this.tblim6.Size = new System.Drawing.Size(24, 21);
            this.tblim6.TabIndex = 375;
            this.tblim6.Text = "61";
            this.tblim6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tblim5
            // 
            this.tblim5.BackColor = System.Drawing.Color.White;
            this.tblim5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tblim5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tblim5.Location = new System.Drawing.Point(395, 158);
            this.tblim5.MaxLength = 2;
            this.tblim5.Name = "tblim5";
            this.tblim5.Size = new System.Drawing.Size(24, 21);
            this.tblim5.TabIndex = 374;
            this.tblim5.Text = "48";
            this.tblim5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tblim4
            // 
            this.tblim4.BackColor = System.Drawing.Color.White;
            this.tblim4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tblim4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tblim4.Location = new System.Drawing.Point(363, 158);
            this.tblim4.MaxLength = 2;
            this.tblim4.Name = "tblim4";
            this.tblim4.Size = new System.Drawing.Size(24, 21);
            this.tblim4.TabIndex = 373;
            this.tblim4.Text = "36";
            this.tblim4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tblim3
            // 
            this.tblim3.BackColor = System.Drawing.Color.White;
            this.tblim3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tblim3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tblim3.Location = new System.Drawing.Point(331, 158);
            this.tblim3.MaxLength = 2;
            this.tblim3.Name = "tblim3";
            this.tblim3.Size = new System.Drawing.Size(24, 21);
            this.tblim3.TabIndex = 372;
            this.tblim3.Text = "29";
            this.tblim3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tblim2
            // 
            this.tblim2.BackColor = System.Drawing.Color.White;
            this.tblim2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tblim2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tblim2.Location = new System.Drawing.Point(299, 158);
            this.tblim2.MaxLength = 2;
            this.tblim2.Name = "tblim2";
            this.tblim2.Size = new System.Drawing.Size(24, 21);
            this.tblim2.TabIndex = 371;
            this.tblim2.Text = "22";
            this.tblim2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tblim1
            // 
            this.tblim1.BackColor = System.Drawing.Color.White;
            this.tblim1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tblim1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tblim1.Location = new System.Drawing.Point(267, 158);
            this.tblim1.MaxLength = 2;
            this.tblim1.Name = "tblim1";
            this.tblim1.Size = new System.Drawing.Size(24, 21);
            this.tblim1.TabIndex = 370;
            this.tblim1.Text = "15";
            this.tblim1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Verdana", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.Maroon;
            this.label8.Location = new System.Drawing.Point(16, 158);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(210, 21);
            this.label8.TabIndex = 376;
            this.label8.Text = "Separador de porcentajes JB";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtPct14
            // 
            this.txtPct14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPct14.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPct14.Location = new System.Drawing.Point(267, 258);
            this.txtPct14.Name = "txtPct14";
            this.txtPct14.Size = new System.Drawing.Size(24, 21);
            this.txtPct14.TabIndex = 379;
            this.txtPct14.Text = "15";
            // 
            // txtPrApuesta
            // 
            this.txtPrApuesta.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPrApuesta.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPrApuesta.Location = new System.Drawing.Point(267, 233);
            this.txtPrApuesta.Name = "txtPrApuesta";
            this.txtPrApuesta.Size = new System.Drawing.Size(32, 21);
            this.txtPrApuesta.TabIndex = 378;
            this.txtPrApuesta.Text = "0,50";
            // 
            // txRecaudacion
            // 
            this.txRecaudacion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txRecaudacion.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txRecaudacion.Location = new System.Drawing.Point(267, 208);
            this.txRecaudacion.Name = "txRecaudacion";
            this.txRecaudacion.Size = new System.Drawing.Size(64, 21);
            this.txRecaudacion.TabIndex = 377;
            this.txRecaudacion.Text = "15000000";
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Verdana", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.Maroon;
            this.label9.Location = new System.Drawing.Point(16, 187);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(210, 21);
            this.label9.TabIndex = 380;
            this.label9.Text = "Parámetros LAE";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(40, 258);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(202, 21);
            this.label10.TabIndex = 383;
            this.label10.Text = "% al 14";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label11
            // 
            this.label11.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(40, 233);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(210, 21);
            this.label11.TabIndex = 382;
            this.label11.Text = "Precio apuesta";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label12
            // 
            this.label12.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(40, 208);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(210, 21);
            this.label12.TabIndex = 381;
            this.label12.Text = "Recaudación";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label13
            // 
            this.label13.Font = new System.Drawing.Font("Verdana", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.Color.Maroon;
            this.label13.Location = new System.Drawing.Point(16, 8);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(210, 21);
            this.label13.TabIndex = 384;
            this.label13.Text = "Configuración del boleto";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnToolBar
            // 
            this.btnToolBar.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnToolBar.Enabled = false;
            this.btnToolBar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnToolBar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnToolBar.Image = ((System.Drawing.Image)(resources.GetObject("btnToolBar.Image")));
            this.btnToolBar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnToolBar.Location = new System.Drawing.Point(22, 444);
            this.btnToolBar.Name = "btnToolBar";
            this.btnToolBar.Size = new System.Drawing.Size(174, 32);
            this.btnToolBar.TabIndex = 385;
            this.btnToolBar.Text = "Barras de herramientas";
            this.btnToolBar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnToolBar.UseVisualStyleBackColor = false;
            // 
            // label14
            // 
            this.label14.Font = new System.Drawing.Font("Verdana", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.ForeColor = System.Drawing.Color.Maroon;
            this.label14.Location = new System.Drawing.Point(16, 285);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(210, 21);
            this.label14.TabIndex = 386;
            this.label14.Text = "Actualizador";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkActualizador
            // 
            this.chkActualizador.AutoSize = true;
            this.chkActualizador.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkActualizador.Location = new System.Drawing.Point(51, 309);
            this.chkActualizador.Name = "chkActualizador";
            this.chkActualizador.Size = new System.Drawing.Size(230, 17);
            this.chkActualizador.TabIndex = 387;
            this.chkActualizador.Text = "Comprobar actualizaciones al Inicio";
            this.chkActualizador.UseVisualStyleBackColor = true;
            // 
            // label15
            // 
            this.label15.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(303, 233);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(114, 21);
            this.label15.TabIndex = 388;
            this.label15.Text = "Símbolo Moneda";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txtMoneda
            // 
            this.txtMoneda.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMoneda.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtMoneda.Location = new System.Drawing.Point(419, 233);
            this.txtMoneda.Name = "txtMoneda";
            this.txtMoneda.Size = new System.Drawing.Size(25, 21);
            this.txtMoneda.TabIndex = 389;
            this.txtMoneda.Text = "";
            this.txtMoneda.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Enabled = false;
            this.label16.Font = new System.Drawing.Font("Verdana", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.ForeColor = System.Drawing.Color.Maroon;
            this.label16.Location = new System.Drawing.Point(16, 393);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(53, 13);
            this.label16.TabIndex = 390;
            this.label16.Text = "Idioma";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // cbbIdioma
            // 
            this.cbbIdioma.Enabled = false;
            this.cbbIdioma.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbbIdioma.FormattingEnabled = true;
            this.cbbIdioma.Location = new System.Drawing.Point(51, 410);
            this.cbbIdioma.Name = "cbbIdioma";
            this.cbbIdioma.Size = new System.Drawing.Size(121, 21);
            this.cbbIdioma.TabIndex = 391;
            // 
            // chbSalir
            // 
            this.chbSalir.AutoSize = true;
            this.chbSalir.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chbSalir.Location = new System.Drawing.Point(51, 362);
            this.chbSalir.Name = "chbSalir";
            this.chbSalir.Size = new System.Drawing.Size(135, 17);
            this.chbSalir.TabIndex = 393;
            this.chbSalir.Text = "Pedir Confirmación";
            this.chbSalir.UseVisualStyleBackColor = true;
            // 
            // label17
            // 
            this.label17.Font = new System.Drawing.Font("Verdana", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.ForeColor = System.Drawing.Color.Maroon;
            this.label17.Location = new System.Drawing.Point(16, 338);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(210, 21);
            this.label17.TabIndex = 392;
            this.label17.Text = "Al Salir";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ConfiguracionFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(464, 489);
            this.ControlBox = false;
            this.Controls.Add(this.chbSalir);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.cbbIdioma);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.txtMoneda);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.chkActualizador);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.btnToolBar);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txtPct14);
            this.Controls.Add(this.txtPrApuesta);
            this.Controls.Add(this.txRecaudacion);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.tblim6);
            this.Controls.Add(this.tblim5);
            this.Controls.Add(this.tblim4);
            this.Controls.Add(this.tblim3);
            this.Controls.Add(this.tblim2);
            this.Controls.Add(this.tblim1);
            this.Controls.Add(this.txtValorTriples);
            this.Controls.Add(this.txtValorDobles);
            this.Controls.Add(this.txtValorFijos);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtDesplazamiento);
            this.Controls.Add(this.txtSeparador);
            this.Controls.Add(this.txtNumPartidos);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.btnGuardar);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfiguracionFrm";
            this.Text = "Configuración";
            this.Load += new System.EventHandler(this.ConfiguracionFrm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion



		private void button1_Click(object sender, System.EventArgs e)
		{
            this.Close();
		}

		private void btnGuardar_Click(object sender, System.EventArgs e)
		{
			int n=0;
			string s="";
			string[] jb=new string[6];
			TextBox txt;
			// Comprueba que hayan datos pero no que sean correctos
			for(int c=0;c<this.Controls.Count-1;c++)
			{
				txt= this.Controls[c] as TextBox;
				if(txt != null)
				{
					if(txt.Text.Length==0)
					{
						MessageBox.Show("Este campo no se puede dejar vacío.","Configuración",MessageBoxButtons.OK,MessageBoxIcon.Error);
						txt.Focus();
						return;
					}
					// Llena la matriz del Separador JB con sus valores
					if(txt.Name.IndexOf("tblim")>=0)
					{
						s=txt.Name.Replace("tblim","");
						n=Convert.ToInt16(s)-1;
						jb[n]=txt.Text;
					}
				}
			}
			// Comprueba que los valores del separador JB sean válidos.
			for(int i=1;i<jb.Length;i++)
			{
				if(Convert.ToInt16(jb[i-1])>=Convert.ToInt16(jb[i]))
				{
					MessageBox.Show("El valor del "+Convert.ToString(i-1)+"º campo, no puede ser mayor que el/los siguiente/s.","Configuración",MessageBoxButtons.OK,MessageBoxIcon.Error);
					return;
				}
			}
			// Abre el archivo de configuracion
			AConfiguracion ac = new AConfiguracion(Application.StartupPath);
			// Separador JB
			ac.GuardarValoresSeparadorJB(jb);
			// Valores LAE
            double Recaudacion = float.Parse(txRecaudacion.Text, new System.Globalization.CultureInfo("es-ES"));
			double PrecioApuesta =float.Parse(txtPrApuesta.Text,new System.Globalization.CultureInfo("es-ES") );
            double PorcentajeDestinadoAlPremiode14 = float.Parse(txtPct14.Text, new System.Globalization.CultureInfo("es-ES"));
            string simboloMoneda = txtMoneda.Text;
			ac.GuardarValoresLAE(PrecioApuesta, PorcentajeDestinadoAlPremiode14, Recaudacion, simboloMoneda);
			// Puntos CPs
			int valorFijos = Convert.ToInt32(txtValorFijos.Text);
			int valorDobles = Convert.ToInt32(txtValorDobles.Text);
			int valorTriples = Convert.ToInt32(txtValorTriples.Text);
			ac.GuardarPuntosCP(valorFijos, valorDobles, valorTriples);
			// Configuración del Boleto
            int numP = 0;
            try
            {
                numP = Convert.ToInt16(txtNumPartidos.Text);
            }
            catch
            {
                numP = 14;
            }
            string sep = txtSeparador.Text;
			ac.GuardarConfiguracionBoleto(numP,sep);
			// Desplazamiento
			ac.GuardarDesplazamiento(txtDesplazamiento.Text);
            //Actualizaciones
            ac.GuardarConfiguracionActualizador(chkActualizador.Checked);
            //Idioma
            ac.GuardarIdioma(cbbIdioma.SelectedItem.ToString());
            //Salir
            ac.GuardarConfiguracionAdvertenciaSalir(chbSalir.Checked);
            VariablesGlobales.ReinicializarVariables();
            if (MessageBox.Show("Configuración guardada. Se recomienda reiniciar el programa. ¿Reiniciar?", "Configuración", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Information) == DialogResult.Yes)
            {
                Reiniciar();
            }
            
		}
        protected void Reiniciar()
        {
            try
            {
                Application.Restart();
            }
            catch
            {
                string ruta = Application.StartupPath;
                string ejecutable = ruta + "/Free1X2.exe";
                System.Diagnostics.Process.Start(ejecutable);
            }

        }		

		private void ConfiguracionFrm_Load(object sender, System.EventArgs e)
		{
			// Abre el archivo de configuracion
			AConfiguracion ac = new AConfiguracion(Application.StartupPath);
			// Separador JB
			string[] jb=new string[6];
			jb=ac.ObtenValoresUtilSeparadorJB().Split(',');
			tblim1.Text=jb[0];
			tblim2.Text=jb[1];
			tblim3.Text=jb[2];
			tblim4.Text=jb[3];
			tblim5.Text=jb[4];
			tblim6.Text=jb[5];
			// Valores LAE
			string Recaudacion ="";
			string PrecioApuesta ="";
			string PorcentajeDestinadoAlPremiode14 ="";
            string simboloMoneda = "";
			ac.ObtenValoresLAE(ref PrecioApuesta, ref PorcentajeDestinadoAlPremiode14, ref Recaudacion, ref simboloMoneda);
			txRecaudacion.Text=Recaudacion;
			txtPrApuesta.Text=PrecioApuesta;
			txtPct14.Text=PorcentajeDestinadoAlPremiode14;
            txtMoneda.Text = simboloMoneda;
			// Puntos CPs
			int valorFijos = 0;
			int valorDobles = 0;
			int valorTriples = 0;
			ac.ObtenPuntosCP(ref valorFijos, ref valorDobles, ref valorTriples);
			txtValorFijos.Text = valorFijos.ToString();
			txtValorDobles.Text = valorDobles.ToString();
			txtValorTriples.Text = valorTriples.ToString();
			// Desplazamiento
			int desplazamiento=3;
			ac.ObtenDesplazamiento(ref desplazamiento);
			txtDesplazamiento.Text=desplazamiento.ToString();
			// Configuración del Boleto
			int numP=0;
			string sep="";
			ac.ObtenNumPartidos(ref numP, ref sep);
			txtNumPartidos.Text=numP.ToString();
			txtSeparador.Text=sep;

            //Actualizador
            bool actualizarAlInicio = false;
            ac.ObtenConfiguracionActualizador(ref actualizarAlInicio);
            chkActualizador.Checked = actualizarAlInicio;

            //Idioma
            string idioma = "";
            ac.ObtenInfoIdioma(ref idioma);
            for (int i = 0; i < cbbIdioma.Items.Count; i++)
            {
                if (cbbIdioma.Items[i].ToString() == idioma)
                {
                    cbbIdioma.SelectedItem = cbbIdioma.Items[i];
                }
            }

            //Salir
            bool pedirConfirmacionAlSalir = false;
            ac.ObtenConfiguracionAdvertenciaSalir(ref pedirConfirmacionAlSalir);
            chbSalir.Checked = pedirConfirmacionAlSalir;
		}

        private void button2_Click(object sender, EventArgs e)
        {
            ConfiguracionAnalisisFrm conf = new ConfiguracionAnalisisFrm();
            conf.ShowDialog();
        }

	}
}
