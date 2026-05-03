// Free1X2 : Programa de quinielas "libre"
// Created 29-12-04 at 13:06 
// Copyright (C) 2004 Joan Duatis - duatis@coac.net
// 
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
using System.IO;
using Free1X2.EntradaSalida;
using Free1X2.Utils;


using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI
{
	/// <summary>
	/// Summary description for DialogoAnalisisMultipleDeTramosFrm.
	/// </summary>
	public class DialogoAnalisisMultipleDeTramosFrm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txNombreFicheroValoraciones;
		private System.Windows.Forms.Button btSeleccionarFichero;
		/// <summary>
		/// Required designer variable.
		/// </summary>
	
		public string FicheroValoracionesHistoricas="";
		public byte Es14Triples=0;
        public string FicheroCombinación = "";
		private ArrayList ListaTemporadas=null;
		public ArrayList ListaCombinaciones= new ArrayList();
		private	string[] ValorsJornada;
		private int Temporada;
		private int Jornada=1;
		private System.Windows.Forms.Button btCancelar;
		private System.Windows.Forms.Button btAceptar;
		private System.Windows.Forms.GroupBox grSeleccionJornada;
		public System.Windows.Forms.TextBox txPremioMinimoDe14;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tab14T;
		private System.Windows.Forms.TabPage tabFicheros;
		public System.Windows.Forms.CheckedListBox chkTemporadas;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txTemporada2;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Button btJornadaSiguiente;
		private System.Windows.Forms.Button btJornadaAnterior;
		private System.Windows.Forms.TextBox txJornada;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Button btTemporadaSiguiente;
		private System.Windows.Forms.Button btTemporadaAnterior;
		private System.Windows.Forms.TextBox txTemporada;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Button btAdd;
		private System.Windows.Forms.Button btEliminar;
		private System.Windows.Forms.Button btGuardar;
		private System.Windows.Forms.Button btLeer;
		public System.Windows.Forms.TextBox txPremioMaximoDe14;
		private System.Windows.Forms.Button btTodas;
		private System.Windows.Forms.Button btNinguna;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label11;
		public System.Windows.Forms.TextBox txPremioMaximoDe13;
		public System.Windows.Forms.TextBox txPremioMinimoDe13;
		public System.Windows.Forms.TextBox txPremioMaximoDe12;
		public System.Windows.Forms.TextBox txPremioMinimoDe12;
		private System.Windows.Forms.Label label5;
		public System.Windows.Forms.TextBox txRecaudacionMaxima;
		public System.Windows.Forms.TextBox txRecaudacionMinima;
		private System.Windows.Forms.Label label6;
		public System.Windows.Forms.TextBox txPremioMaximoDe10;
		public System.Windows.Forms.TextBox txPremioMinimoDe10;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label14;
		public System.Windows.Forms.TextBox txPremioMaximoDe11;
		private System.Windows.Forms.Label label15;
		public System.Windows.Forms.TextBox txPremioMinimoDe11;
		public System.Windows.Forms.CheckBox chkFicherosEn14T;
		private System.Windows.Forms.DataGrid dgListaFicheros;
        private Button btSeleccionarCombi;
        private TextBox txFichero;
        private RadioButton rbFichero;
        private RadioButton rb14Triples;

		private System.ComponentModel.Container components = null;

		public DialogoAnalisisMultipleDeTramosFrm(string pFicheroValoraciones)
		{
			InitializeComponent();
			FicheroValoracionesHistoricas=pFicheroValoraciones;
			txNombreFicheroValoraciones.Text =pFicheroValoraciones;
			CargarListaDeTemporadas();
			InicializaGridCombinaciones();
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DialogoAnalisisMultipleDeTramosFrm));
            this.label1 = new System.Windows.Forms.Label();
            this.txNombreFicheroValoraciones = new System.Windows.Forms.TextBox();
            this.btSeleccionarFichero = new System.Windows.Forms.Button();
            this.btCancelar = new System.Windows.Forms.Button();
            this.btAceptar = new System.Windows.Forms.Button();
            this.grSeleccionJornada = new System.Windows.Forms.GroupBox();
            this.txRecaudacionMaxima = new System.Windows.Forms.TextBox();
            this.txRecaudacionMinima = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txPremioMaximoDe10 = new System.Windows.Forms.TextBox();
            this.txPremioMinimoDe10 = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.txPremioMaximoDe11 = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.txPremioMinimoDe11 = new System.Windows.Forms.TextBox();
            this.txPremioMaximoDe12 = new System.Windows.Forms.TextBox();
            this.txPremioMinimoDe12 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txPremioMaximoDe13 = new System.Windows.Forms.TextBox();
            this.txPremioMinimoDe13 = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txPremioMaximoDe14 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txPremioMinimoDe14 = new System.Windows.Forms.TextBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tab14T = new System.Windows.Forms.TabPage();
            this.btSeleccionarCombi = new System.Windows.Forms.Button();
            this.txFichero = new System.Windows.Forms.TextBox();
            this.rbFichero = new System.Windows.Forms.RadioButton();
            this.rb14Triples = new System.Windows.Forms.RadioButton();
            this.btNinguna = new System.Windows.Forms.Button();
            this.btTodas = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.chkTemporadas = new System.Windows.Forms.CheckedListBox();
            this.tabFicheros = new System.Windows.Forms.TabPage();
            this.dgListaFicheros = new System.Windows.Forms.DataGrid();
            this.chkFicherosEn14T = new System.Windows.Forms.CheckBox();
            this.btLeer = new System.Windows.Forms.Button();
            this.btGuardar = new System.Windows.Forms.Button();
            this.btEliminar = new System.Windows.Forms.Button();
            this.btAdd = new System.Windows.Forms.Button();
            this.txTemporada2 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.btJornadaSiguiente = new System.Windows.Forms.Button();
            this.btJornadaAnterior = new System.Windows.Forms.Button();
            this.txJornada = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btTemporadaSiguiente = new System.Windows.Forms.Button();
            this.btTemporadaAnterior = new System.Windows.Forms.Button();
            this.txTemporada = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.grSeleccionJornada.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tab14T.SuspendLayout();
            this.tabFicheros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgListaFicheros)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(185, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "Fichero de valoraciones historicas:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txNombreFicheroValoraciones
            // 
            this.txNombreFicheroValoraciones.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txNombreFicheroValoraciones.BackColor = System.Drawing.Color.LemonChiffon;
            this.txNombreFicheroValoraciones.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txNombreFicheroValoraciones.Location = new System.Drawing.Point(196, 8);
            this.txNombreFicheroValoraciones.Name = "txNombreFicheroValoraciones";
            this.txNombreFicheroValoraciones.Size = new System.Drawing.Size(441, 21);
            this.txNombreFicheroValoraciones.TabIndex = 1;
            // 
            // btSeleccionarFichero
            // 
            this.btSeleccionarFichero.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSeleccionarFichero.BackColor = System.Drawing.Color.LightSalmon;
            this.btSeleccionarFichero.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btSeleccionarFichero.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btSeleccionarFichero.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btSeleccionarFichero.Image = ((System.Drawing.Image)(resources.GetObject("btSeleccionarFichero.Image")));
            this.btSeleccionarFichero.Location = new System.Drawing.Point(638, 8);
            this.btSeleccionarFichero.Name = "btSeleccionarFichero";
            this.btSeleccionarFichero.Size = new System.Drawing.Size(24, 21);
            this.btSeleccionarFichero.TabIndex = 11;
            this.btSeleccionarFichero.UseVisualStyleBackColor = false;
            this.btSeleccionarFichero.Click += new System.EventHandler(this.btSeleccionarFichero_Click);
            // 
            // btCancelar
            // 
            this.btCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btCancelar.BackColor = System.Drawing.Color.LightSalmon;
            this.btCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btCancelar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btCancelar.Image = ((System.Drawing.Image)(resources.GetObject("btCancelar.Image")));
            this.btCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btCancelar.Location = new System.Drawing.Point(583, 392);
            this.btCancelar.Name = "btCancelar";
            this.btCancelar.Size = new System.Drawing.Size(81, 24);
            this.btCancelar.TabIndex = 14;
            this.btCancelar.Text = "Cancelar";
            this.btCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btCancelar.UseVisualStyleBackColor = false;
            this.btCancelar.Click += new System.EventHandler(this.btCancelar_Click);
            // 
            // btAceptar
            // 
            this.btAceptar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btAceptar.BackColor = System.Drawing.Color.LightSalmon;
            this.btAceptar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btAceptar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btAceptar.Image = ((System.Drawing.Image)(resources.GetObject("btAceptar.Image")));
            this.btAceptar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btAceptar.Location = new System.Drawing.Point(583, 360);
            this.btAceptar.Name = "btAceptar";
            this.btAceptar.Size = new System.Drawing.Size(81, 24);
            this.btAceptar.TabIndex = 15;
            this.btAceptar.Text = "Aceptar";
            this.btAceptar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btAceptar.UseVisualStyleBackColor = false;
            this.btAceptar.Click += new System.EventHandler(this.btAceptar_Click);
            // 
            // grSeleccionJornada
            // 
            this.grSeleccionJornada.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.grSeleccionJornada.BackColor = System.Drawing.Color.Bisque;
            this.grSeleccionJornada.Controls.Add(this.txRecaudacionMaxima);
            this.grSeleccionJornada.Controls.Add(this.txRecaudacionMinima);
            this.grSeleccionJornada.Controls.Add(this.label6);
            this.grSeleccionJornada.Controls.Add(this.txPremioMaximoDe10);
            this.grSeleccionJornada.Controls.Add(this.txPremioMinimoDe10);
            this.grSeleccionJornada.Controls.Add(this.label12);
            this.grSeleccionJornada.Controls.Add(this.label13);
            this.grSeleccionJornada.Controls.Add(this.label14);
            this.grSeleccionJornada.Controls.Add(this.txPremioMaximoDe11);
            this.grSeleccionJornada.Controls.Add(this.label15);
            this.grSeleccionJornada.Controls.Add(this.txPremioMinimoDe11);
            this.grSeleccionJornada.Controls.Add(this.txPremioMaximoDe12);
            this.grSeleccionJornada.Controls.Add(this.txPremioMinimoDe12);
            this.grSeleccionJornada.Controls.Add(this.label5);
            this.grSeleccionJornada.Controls.Add(this.txPremioMaximoDe13);
            this.grSeleccionJornada.Controls.Add(this.txPremioMinimoDe13);
            this.grSeleccionJornada.Controls.Add(this.label11);
            this.grSeleccionJornada.Controls.Add(this.label7);
            this.grSeleccionJornada.Controls.Add(this.label4);
            this.grSeleccionJornada.Controls.Add(this.txPremioMaximoDe14);
            this.grSeleccionJornada.Controls.Add(this.label3);
            this.grSeleccionJornada.Controls.Add(this.txPremioMinimoDe14);
            this.grSeleccionJornada.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grSeleccionJornada.ForeColor = System.Drawing.Color.Maroon;
            this.grSeleccionJornada.Location = new System.Drawing.Point(8, 312);
            this.grSeleccionJornada.Name = "grSeleccionJornada";
            this.grSeleccionJornada.Size = new System.Drawing.Size(528, 108);
            this.grSeleccionJornada.TabIndex = 16;
            this.grSeleccionJornada.TabStop = false;
            this.grSeleccionJornada.Text = "Criterio de selección de jornadas";
            // 
            // txRecaudacionMaxima
            // 
            this.txRecaudacionMaxima.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txRecaudacionMaxima.ForeColor = System.Drawing.Color.Black;
            this.txRecaudacionMaxima.Location = new System.Drawing.Point(202, 37);
            this.txRecaudacionMaxima.Name = "txRecaudacionMaxima";
            this.txRecaudacionMaxima.Size = new System.Drawing.Size(56, 18);
            this.txRecaudacionMaxima.TabIndex = 36;
            this.txRecaudacionMaxima.Text = "25000000";
            // 
            // txRecaudacionMinima
            // 
            this.txRecaudacionMinima.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txRecaudacionMinima.ForeColor = System.Drawing.Color.Black;
            this.txRecaudacionMinima.Location = new System.Drawing.Point(145, 37);
            this.txRecaudacionMinima.Name = "txRecaudacionMinima";
            this.txRecaudacionMinima.Size = new System.Drawing.Size(56, 18);
            this.txRecaudacionMinima.TabIndex = 35;
            this.txRecaudacionMinima.Text = "0";
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.Black;
            this.label6.Location = new System.Drawing.Point(12, 37);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(132, 18);
            this.label6.TabIndex = 34;
            this.label6.Text = "Recaudación";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txPremioMaximoDe10
            // 
            this.txPremioMaximoDe10.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txPremioMaximoDe10.ForeColor = System.Drawing.Color.Black;
            this.txPremioMaximoDe10.Location = new System.Drawing.Point(450, 75);
            this.txPremioMaximoDe10.Name = "txPremioMaximoDe10";
            this.txPremioMaximoDe10.Size = new System.Drawing.Size(56, 18);
            this.txPremioMaximoDe10.TabIndex = 33;
            this.txPremioMaximoDe10.Text = "3000000";
            // 
            // txPremioMinimoDe10
            // 
            this.txPremioMinimoDe10.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txPremioMinimoDe10.ForeColor = System.Drawing.Color.Black;
            this.txPremioMinimoDe10.Location = new System.Drawing.Point(393, 75);
            this.txPremioMinimoDe10.Name = "txPremioMinimoDe10";
            this.txPremioMinimoDe10.Size = new System.Drawing.Size(56, 18);
            this.txPremioMinimoDe10.TabIndex = 32;
            this.txPremioMinimoDe10.Text = "0";
            // 
            // label12
            // 
            this.label12.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label12.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.Color.Black;
            this.label12.Location = new System.Drawing.Point(450, 18);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(56, 18);
            this.label12.TabIndex = 31;
            this.label12.Text = "Máximo";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label13
            // 
            this.label13.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label13.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.Color.Black;
            this.label13.Location = new System.Drawing.Point(393, 18);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(56, 18);
            this.label13.TabIndex = 30;
            this.label13.Text = "Minimo";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label14
            // 
            this.label14.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label14.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.ForeColor = System.Drawing.Color.Black;
            this.label14.Location = new System.Drawing.Point(260, 75);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(132, 18);
            this.label14.TabIndex = 29;
            this.label14.Text = "Importe premio de 10";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txPremioMaximoDe11
            // 
            this.txPremioMaximoDe11.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txPremioMaximoDe11.ForeColor = System.Drawing.Color.Black;
            this.txPremioMaximoDe11.Location = new System.Drawing.Point(450, 56);
            this.txPremioMaximoDe11.Name = "txPremioMaximoDe11";
            this.txPremioMaximoDe11.Size = new System.Drawing.Size(56, 18);
            this.txPremioMaximoDe11.TabIndex = 28;
            this.txPremioMaximoDe11.Text = "3000000";
            // 
            // label15
            // 
            this.label15.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label15.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.ForeColor = System.Drawing.Color.Black;
            this.label15.Location = new System.Drawing.Point(260, 56);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(132, 18);
            this.label15.TabIndex = 27;
            this.label15.Text = "Importe premio de 11";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txPremioMinimoDe11
            // 
            this.txPremioMinimoDe11.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txPremioMinimoDe11.ForeColor = System.Drawing.Color.Black;
            this.txPremioMinimoDe11.Location = new System.Drawing.Point(393, 56);
            this.txPremioMinimoDe11.Name = "txPremioMinimoDe11";
            this.txPremioMinimoDe11.Size = new System.Drawing.Size(56, 18);
            this.txPremioMinimoDe11.TabIndex = 26;
            this.txPremioMinimoDe11.Text = "0";
            // 
            // txPremioMaximoDe12
            // 
            this.txPremioMaximoDe12.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txPremioMaximoDe12.ForeColor = System.Drawing.Color.Black;
            this.txPremioMaximoDe12.Location = new System.Drawing.Point(450, 37);
            this.txPremioMaximoDe12.Name = "txPremioMaximoDe12";
            this.txPremioMaximoDe12.Size = new System.Drawing.Size(56, 18);
            this.txPremioMaximoDe12.TabIndex = 25;
            this.txPremioMaximoDe12.Text = "3000000";
            // 
            // txPremioMinimoDe12
            // 
            this.txPremioMinimoDe12.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txPremioMinimoDe12.ForeColor = System.Drawing.Color.Black;
            this.txPremioMinimoDe12.Location = new System.Drawing.Point(393, 37);
            this.txPremioMinimoDe12.Name = "txPremioMinimoDe12";
            this.txPremioMinimoDe12.Size = new System.Drawing.Size(56, 18);
            this.txPremioMinimoDe12.TabIndex = 24;
            this.txPremioMinimoDe12.Text = "0";
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(260, 37);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(132, 18);
            this.label5.TabIndex = 23;
            this.label5.Text = "Importe premio de 12";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txPremioMaximoDe13
            // 
            this.txPremioMaximoDe13.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txPremioMaximoDe13.ForeColor = System.Drawing.Color.Black;
            this.txPremioMaximoDe13.Location = new System.Drawing.Point(202, 75);
            this.txPremioMaximoDe13.Name = "txPremioMaximoDe13";
            this.txPremioMaximoDe13.Size = new System.Drawing.Size(56, 18);
            this.txPremioMaximoDe13.TabIndex = 22;
            this.txPremioMaximoDe13.Text = "3000000";
            // 
            // txPremioMinimoDe13
            // 
            this.txPremioMinimoDe13.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txPremioMinimoDe13.ForeColor = System.Drawing.Color.Black;
            this.txPremioMinimoDe13.Location = new System.Drawing.Point(145, 75);
            this.txPremioMinimoDe13.Name = "txPremioMinimoDe13";
            this.txPremioMinimoDe13.Size = new System.Drawing.Size(56, 18);
            this.txPremioMinimoDe13.TabIndex = 21;
            this.txPremioMinimoDe13.Text = "0";
            // 
            // label11
            // 
            this.label11.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label11.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.Black;
            this.label11.Location = new System.Drawing.Point(202, 18);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(56, 18);
            this.label11.TabIndex = 20;
            this.label11.Text = "Máximo";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label7.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.Black;
            this.label7.Location = new System.Drawing.Point(144, 18);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(56, 18);
            this.label7.TabIndex = 19;
            this.label7.Text = "Minimo";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(12, 75);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(132, 18);
            this.label4.TabIndex = 16;
            this.label4.Text = "Importe premio de 13";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txPremioMaximoDe14
            // 
            this.txPremioMaximoDe14.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txPremioMaximoDe14.ForeColor = System.Drawing.Color.Black;
            this.txPremioMaximoDe14.Location = new System.Drawing.Point(202, 56);
            this.txPremioMaximoDe14.Name = "txPremioMaximoDe14";
            this.txPremioMaximoDe14.Size = new System.Drawing.Size(56, 18);
            this.txPremioMaximoDe14.TabIndex = 15;
            this.txPremioMaximoDe14.Text = "6000000";
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(12, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(132, 18);
            this.label3.TabIndex = 14;
            this.label3.Text = "Importe premio de 14";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txPremioMinimoDe14
            // 
            this.txPremioMinimoDe14.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txPremioMinimoDe14.ForeColor = System.Drawing.Color.Black;
            this.txPremioMinimoDe14.Location = new System.Drawing.Point(145, 56);
            this.txPremioMinimoDe14.Name = "txPremioMinimoDe14";
            this.txPremioMinimoDe14.Size = new System.Drawing.Size(56, 18);
            this.txPremioMinimoDe14.TabIndex = 0;
            this.txPremioMinimoDe14.Text = "0";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tab14T);
            this.tabControl1.Controls.Add(this.tabFicheros);
            this.tabControl1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(4, 36);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(665, 264);
            this.tabControl1.TabIndex = 20;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tab14T
            // 
            this.tab14T.BackColor = System.Drawing.Color.Bisque;
            this.tab14T.Controls.Add(this.btSeleccionarCombi);
            this.tab14T.Controls.Add(this.txFichero);
            this.tab14T.Controls.Add(this.rbFichero);
            this.tab14T.Controls.Add(this.rb14Triples);
            this.tab14T.Controls.Add(this.btNinguna);
            this.tab14T.Controls.Add(this.btTodas);
            this.tab14T.Controls.Add(this.label2);
            this.tab14T.Controls.Add(this.chkTemporadas);
            this.tab14T.Location = new System.Drawing.Point(4, 22);
            this.tab14T.Name = "tab14T";
            this.tab14T.Size = new System.Drawing.Size(657, 238);
            this.tab14T.TabIndex = 0;
            this.tab14T.Text = "Combinación única";
            this.tab14T.UseVisualStyleBackColor = true;
            // 
            // btSeleccionarCombi
            // 
            this.btSeleccionarCombi.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSeleccionarCombi.BackColor = System.Drawing.Color.LightSalmon;
            this.btSeleccionarCombi.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btSeleccionarCombi.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btSeleccionarCombi.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btSeleccionarCombi.Image = ((System.Drawing.Image)(resources.GetObject("btSeleccionarCombi.Image")));
            this.btSeleccionarCombi.Location = new System.Drawing.Point(625, 80);
            this.btSeleccionarCombi.Name = "btSeleccionarCombi";
            this.btSeleccionarCombi.Size = new System.Drawing.Size(24, 21);
            this.btSeleccionarCombi.TabIndex = 21;
            this.btSeleccionarCombi.UseVisualStyleBackColor = false;
            this.btSeleccionarCombi.Click += new System.EventHandler(this.btSeleccionarCombi_Click);
            // 
            // txFichero
            // 
            this.txFichero.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txFichero.BackColor = System.Drawing.Color.LemonChiffon;
            this.txFichero.Location = new System.Drawing.Point(317, 80);
            this.txFichero.Name = "txFichero";
            this.txFichero.Size = new System.Drawing.Size(307, 21);
            this.txFichero.TabIndex = 20;
            // 
            // rbFichero
            // 
            this.rbFichero.AutoSize = true;
            this.rbFichero.BackColor = System.Drawing.Color.Transparent;
            this.rbFichero.Location = new System.Drawing.Point(298, 57);
            this.rbFichero.Name = "rbFichero";
            this.rbFichero.Size = new System.Drawing.Size(77, 17);
            this.rbFichero.TabIndex = 19;
            this.rbFichero.Text = "FICHERO";
            this.rbFichero.UseVisualStyleBackColor = false;
            // 
            // rb14Triples
            // 
            this.rb14Triples.AutoSize = true;
            this.rb14Triples.BackColor = System.Drawing.Color.Transparent;
            this.rb14Triples.Checked = true;
            this.rb14Triples.Location = new System.Drawing.Point(298, 34);
            this.rb14Triples.Name = "rb14Triples";
            this.rb14Triples.Size = new System.Drawing.Size(91, 17);
            this.rb14Triples.TabIndex = 18;
            this.rb14Triples.TabStop = true;
            this.rb14Triples.Text = "14 TRIPLES";
            this.rb14Triples.UseVisualStyleBackColor = false;
            // 
            // btNinguna
            // 
            this.btNinguna.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btNinguna.BackColor = System.Drawing.Color.LightSalmon;
            this.btNinguna.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btNinguna.Image = ((System.Drawing.Image)(resources.GetObject("btNinguna.Image")));
            this.btNinguna.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btNinguna.Location = new System.Drawing.Point(325, 204);
            this.btNinguna.Name = "btNinguna";
            this.btNinguna.Size = new System.Drawing.Size(105, 23);
            this.btNinguna.TabIndex = 17;
            this.btNinguna.Text = "Ninguna";
            this.btNinguna.UseVisualStyleBackColor = false;
            this.btNinguna.Click += new System.EventHandler(this.btNinguna_Click);
            // 
            // btTodas
            // 
            this.btTodas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btTodas.BackColor = System.Drawing.Color.LightSalmon;
            this.btTodas.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btTodas.Image = ((System.Drawing.Image)(resources.GetObject("btTodas.Image")));
            this.btTodas.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btTodas.Location = new System.Drawing.Point(325, 180);
            this.btTodas.Name = "btTodas";
            this.btTodas.Size = new System.Drawing.Size(105, 23);
            this.btTodas.TabIndex = 16;
            this.btTodas.Text = "Todas";
            this.btTodas.UseVisualStyleBackColor = false;
            this.btTodas.Click += new System.EventHandler(this.btTodas_Click);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(12, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 20);
            this.label2.TabIndex = 14;
            this.label2.Text = "Temporadas:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkTemporadas
            // 
            this.chkTemporadas.BackColor = System.Drawing.Color.WhiteSmoke;
            this.chkTemporadas.CheckOnClick = true;
            this.chkTemporadas.ForeColor = System.Drawing.Color.Black;
            this.chkTemporadas.Location = new System.Drawing.Point(12, 28);
            this.chkTemporadas.MultiColumn = true;
            this.chkTemporadas.Name = "chkTemporadas";
            this.chkTemporadas.Size = new System.Drawing.Size(266, 196);
            this.chkTemporadas.TabIndex = 13;
            // 
            // tabFicheros
            // 
            this.tabFicheros.BackColor = System.Drawing.Color.Bisque;
            this.tabFicheros.Controls.Add(this.dgListaFicheros);
            this.tabFicheros.Controls.Add(this.chkFicherosEn14T);
            this.tabFicheros.Controls.Add(this.btLeer);
            this.tabFicheros.Controls.Add(this.btGuardar);
            this.tabFicheros.Controls.Add(this.btEliminar);
            this.tabFicheros.Controls.Add(this.btAdd);
            this.tabFicheros.Controls.Add(this.txTemporada2);
            this.tabFicheros.Controls.Add(this.label8);
            this.tabFicheros.Controls.Add(this.btJornadaSiguiente);
            this.tabFicheros.Controls.Add(this.btJornadaAnterior);
            this.tabFicheros.Controls.Add(this.txJornada);
            this.tabFicheros.Controls.Add(this.label9);
            this.tabFicheros.Controls.Add(this.btTemporadaSiguiente);
            this.tabFicheros.Controls.Add(this.btTemporadaAnterior);
            this.tabFicheros.Controls.Add(this.txTemporada);
            this.tabFicheros.Controls.Add(this.label10);
            this.tabFicheros.Location = new System.Drawing.Point(4, 22);
            this.tabFicheros.Name = "tabFicheros";
            this.tabFicheros.Size = new System.Drawing.Size(657, 238);
            this.tabFicheros.TabIndex = 1;
            this.tabFicheros.Text = "Una combinación por jornada";
            this.tabFicheros.UseVisualStyleBackColor = true;
            // 
            // dgListaFicheros
            // 
            this.dgListaFicheros.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgListaFicheros.DataMember = "";
            this.dgListaFicheros.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgListaFicheros.Location = new System.Drawing.Point(0, 40);
            this.dgListaFicheros.Name = "dgListaFicheros";
            this.dgListaFicheros.Size = new System.Drawing.Size(657, 168);
            this.dgListaFicheros.TabIndex = 39;
            // 
            // chkFicherosEn14T
            // 
            this.chkFicherosEn14T.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkFicherosEn14T.Location = new System.Drawing.Point(152, 216);
            this.chkFicherosEn14T.Name = "chkFicherosEn14T";
            this.chkFicherosEn14T.Size = new System.Drawing.Size(332, 16);
            this.chkFicherosEn14T.TabIndex = 38;
            this.chkFicherosEn14T.Text = "Analizar ficheros dentro del desarrollo de 14 triples";
            // 
            // btLeer
            // 
            this.btLeer.BackColor = System.Drawing.Color.LightSalmon;
            this.btLeer.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btLeer.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btLeer.Image = ((System.Drawing.Image)(resources.GetObject("btLeer.Image")));
            this.btLeer.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btLeer.Location = new System.Drawing.Point(506, 13);
            this.btLeer.Name = "btLeer";
            this.btLeer.Size = new System.Drawing.Size(65, 21);
            this.btLeer.TabIndex = 37;
            this.btLeer.Text = "Leer";
            this.btLeer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btLeer.UseVisualStyleBackColor = false;
            this.btLeer.Click += new System.EventHandler(this.btLeer_Click);
            // 
            // btGuardar
            // 
            this.btGuardar.BackColor = System.Drawing.Color.LightSalmon;
            this.btGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btGuardar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btGuardar.Image = ((System.Drawing.Image)(resources.GetObject("btGuardar.Image")));
            this.btGuardar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btGuardar.Location = new System.Drawing.Point(572, 13);
            this.btGuardar.Name = "btGuardar";
            this.btGuardar.Size = new System.Drawing.Size(78, 21);
            this.btGuardar.TabIndex = 35;
            this.btGuardar.Text = "Guardar";
            this.btGuardar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btGuardar.UseVisualStyleBackColor = false;
            this.btGuardar.Click += new System.EventHandler(this.btGuardar_Click);
            // 
            // btEliminar
            // 
            this.btEliminar.BackColor = System.Drawing.Color.LightSalmon;
            this.btEliminar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btEliminar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btEliminar.Image = ((System.Drawing.Image)(resources.GetObject("btEliminar.Image")));
            this.btEliminar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btEliminar.Location = new System.Drawing.Point(424, 13);
            this.btEliminar.Name = "btEliminar";
            this.btEliminar.Size = new System.Drawing.Size(81, 21);
            this.btEliminar.TabIndex = 34;
            this.btEliminar.Text = "Eliminar";
            this.btEliminar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btEliminar.UseVisualStyleBackColor = false;
            this.btEliminar.Click += new System.EventHandler(this.btEliminar_Click);
            // 
            // btAdd
            // 
            this.btAdd.BackColor = System.Drawing.Color.LightSalmon;
            this.btAdd.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btAdd.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btAdd.Image = ((System.Drawing.Image)(resources.GetObject("btAdd.Image")));
            this.btAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btAdd.Location = new System.Drawing.Point(352, 13);
            this.btAdd.Name = "btAdd";
            this.btAdd.Size = new System.Drawing.Size(71, 21);
            this.btAdd.TabIndex = 32;
            this.btAdd.Text = "Añadir";
            this.btAdd.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btAdd.UseVisualStyleBackColor = false;
            this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
            // 
            // txTemporada2
            // 
            this.txTemporada2.BackColor = System.Drawing.Color.LemonChiffon;
            this.txTemporada2.Enabled = false;
            this.txTemporada2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txTemporada2.Location = new System.Drawing.Point(127, 13);
            this.txTemporada2.Name = "txTemporada2";
            this.txTemporada2.Size = new System.Drawing.Size(38, 21);
            this.txTemporada2.TabIndex = 31;
            this.txTemporada2.Text = "2005";
            this.txTemporada2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.Transparent;
            this.label8.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(115, 13);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(12, 21);
            this.label8.TabIndex = 30;
            this.label8.Text = "/";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btJornadaSiguiente
            // 
            this.btJornadaSiguiente.BackColor = System.Drawing.Color.LightSalmon;
            this.btJornadaSiguiente.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btJornadaSiguiente.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btJornadaSiguiente.Image = ((System.Drawing.Image)(resources.GetObject("btJornadaSiguiente.Image")));
            this.btJornadaSiguiente.Location = new System.Drawing.Point(322, 13);
            this.btJornadaSiguiente.Name = "btJornadaSiguiente";
            this.btJornadaSiguiente.Size = new System.Drawing.Size(24, 21);
            this.btJornadaSiguiente.TabIndex = 29;
            this.btJornadaSiguiente.UseVisualStyleBackColor = false;
            this.btJornadaSiguiente.Click += new System.EventHandler(this.btJornadaSiguiente_Click);
            // 
            // btJornadaAnterior
            // 
            this.btJornadaAnterior.BackColor = System.Drawing.Color.LightSalmon;
            this.btJornadaAnterior.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btJornadaAnterior.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btJornadaAnterior.Image = ((System.Drawing.Image)(resources.GetObject("btJornadaAnterior.Image")));
            this.btJornadaAnterior.Location = new System.Drawing.Point(297, 13);
            this.btJornadaAnterior.Name = "btJornadaAnterior";
            this.btJornadaAnterior.Size = new System.Drawing.Size(24, 21);
            this.btJornadaAnterior.TabIndex = 28;
            this.btJornadaAnterior.UseVisualStyleBackColor = false;
            this.btJornadaAnterior.Click += new System.EventHandler(this.btJornadaAnterior_Click);
            // 
            // txJornada
            // 
            this.txJornada.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txJornada.Location = new System.Drawing.Point(272, 13);
            this.txJornada.Name = "txJornada";
            this.txJornada.Size = new System.Drawing.Size(24, 21);
            this.txJornada.TabIndex = 27;
            this.txJornada.Text = "1";
            this.txJornada.TextChanged += new System.EventHandler(this.txJornada_TextChanged);
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label9.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(216, 13);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 21);
            this.label9.TabIndex = 26;
            this.label9.Text = "Jornada";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btTemporadaSiguiente
            // 
            this.btTemporadaSiguiente.BackColor = System.Drawing.Color.LightSalmon;
            this.btTemporadaSiguiente.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btTemporadaSiguiente.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btTemporadaSiguiente.Image = ((System.Drawing.Image)(resources.GetObject("btTemporadaSiguiente.Image")));
            this.btTemporadaSiguiente.Location = new System.Drawing.Point(191, 13);
            this.btTemporadaSiguiente.Name = "btTemporadaSiguiente";
            this.btTemporadaSiguiente.Size = new System.Drawing.Size(24, 21);
            this.btTemporadaSiguiente.TabIndex = 25;
            this.btTemporadaSiguiente.UseVisualStyleBackColor = false;
            this.btTemporadaSiguiente.Click += new System.EventHandler(this.btTemporadaSiguiente_Click);
            // 
            // btTemporadaAnterior
            // 
            this.btTemporadaAnterior.BackColor = System.Drawing.Color.LightSalmon;
            this.btTemporadaAnterior.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btTemporadaAnterior.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btTemporadaAnterior.Image = ((System.Drawing.Image)(resources.GetObject("btTemporadaAnterior.Image")));
            this.btTemporadaAnterior.Location = new System.Drawing.Point(166, 13);
            this.btTemporadaAnterior.Name = "btTemporadaAnterior";
            this.btTemporadaAnterior.Size = new System.Drawing.Size(24, 21);
            this.btTemporadaAnterior.TabIndex = 24;
            this.btTemporadaAnterior.UseVisualStyleBackColor = false;
            this.btTemporadaAnterior.Click += new System.EventHandler(this.btTemporadaAnterior_Click);
            // 
            // txTemporada
            // 
            this.txTemporada.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txTemporada.Location = new System.Drawing.Point(76, 13);
            this.txTemporada.Name = "txTemporada";
            this.txTemporada.Size = new System.Drawing.Size(38, 21);
            this.txTemporada.TabIndex = 23;
            this.txTemporada.Text = "2004";
            this.txTemporada.TextChanged += new System.EventHandler(this.txTemporada_TextChanged);
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label10.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(3, 13);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(76, 21);
            this.label10.TabIndex = 22;
            this.label10.Text = "Temporada";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DialogoAnalisisMultipleDeTramosFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(677, 426);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.grSeleccionJornada);
            this.Controls.Add(this.btAceptar);
            this.Controls.Add(this.btCancelar);
            this.Controls.Add(this.btSeleccionarFichero);
            this.Controls.Add(this.txNombreFicheroValoraciones);
            this.Controls.Add(this.label1);
            this.MinimumSize = new System.Drawing.Size(644, 460);
            this.Name = "DialogoAnalisisMultipleDeTramosFrm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DEFINICIÓN MULTIPLE DE JORNADAS Y FICHEROS";
            this.grSeleccionJornada.ResumeLayout(false);
            this.grSeleccionJornada.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tab14T.ResumeLayout(false);
            this.tab14T.PerformLayout();
            this.tabFicheros.ResumeLayout(false);
            this.tabFicheros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgListaFicheros)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void btSeleccionarFichero_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog abreFiltroDialog = new OpenFileDialog();
			abreFiltroDialog.InitialDirectory = "Columnas\\" ;
			abreFiltroDialog.Filter = "Valoraciones históricas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			if(abreFiltroDialog.ShowDialog () == DialogResult.OK)
			{		    	
				FicheroValoracionesHistoricas = abreFiltroDialog.FileName;		    	
				txNombreFicheroValoraciones.Text = Path.GetFileName(FicheroValoracionesHistoricas);
				CargarListaDeTemporadas();
			}
		}

		private bool CargarListaDeTemporadas()
		{
		    ListaTemporadas = new ArrayList();
			bool res =true;
			if (FicheroValoracionesHistoricas !="")
			{
				string TemporadaAnt="";

                IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(FicheroValoracionesHistoricas);
				while( comBaseCols.SiguienteColumna() )
				{
					ValorsJornada=  comBaseCols.LeeColumnaSinComas().Split ((char) 9);
					if (ValorsJornada.Length !=44)
					{
						res = false;
						break;
					}
					if(ValorsJornada[0]==TemporadaAnt) continue;
					TemporadaAnt=ValorsJornada[0];
					ListaTemporadas.Add (TemporadaAnt);
				}
                comBaseCols.Cerrar();
				if(res)
				{
					txTemporada.Text=TemporadaAnt.Substring (0,4);
					txJornada.Text ="1";
					Jornada=1;
					chkTemporadas.DataSource = ListaTemporadas;
				}
				else
				{
					FicheroValoracionesHistoricas="";
					chkTemporadas.Items.Clear ();
					MessageBox.Show ("El fichero no és de valoraciones históricas");
				}
			}
			return res;
		}

		private void btCancelar_Click(object sender, System.EventArgs e)
		{
			FicheroValoracionesHistoricas ="";
			this.Close ();
		}

		private void btAdd_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog abreFiltroDialog = new OpenFileDialog();
			abreFiltroDialog.InitialDirectory = "Columnas\\" ;
			abreFiltroDialog.Filter = "Combinaciones(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			abreFiltroDialog.Multiselect = true;
			if(abreFiltroDialog.ShowDialog () == DialogResult.OK)
			{		    	
				string FicheroColumnas = abreFiltroDialog.FileName;	
				if (EsFicheroDeColumnas(FicheroColumnas))
				{
//					listaFicheros.Items.Add (txTemporada.Text + "/" +this.txTemporada2.Text + " " + txJornada.Text.PadLeft (2,'0') + " " + FicheroColumnas);
					Combinacion Combi = new Combinacion (txTemporada.Text + "/" +this.txTemporada2.Text,txJornada.Text.PadLeft (2,'0'),FicheroColumnas);
					ListaCombinaciones.Add (Combi);
					
					GridDataBind();
					Jornada++;
					txJornada.Text = Jornada.ToString ();
				}
				else
				{
					MessageBox.Show ("El fichero seleccionado no es un fichero de columnas válido");
				}
			}
		}

		private void btEliminar_Click(object sender, System.EventArgs e)
		{
			DataGridCell selectedCell = dgListaFicheros.CurrentCell;
			for (int i=ListaCombinaciones.Count-1; i>-1 ;i--)
			{
				if(dgListaFicheros.IsSelected (i)) 	ListaCombinaciones.RemoveAt (i);
			}
			GridDataBind();
		}

		private void txTemporada_TextChanged(object sender, System.EventArgs e)
		{
			Temporada=Convert.ToInt32 (txTemporada.Text);
			int temporada2 = Temporada+1;
			txTemporada2.Text = temporada2.ToString ();
		}

		private void btTemporadaAnterior_Click(object sender, System.EventArgs e)
		{
			Temporada=Convert.ToInt32 (txTemporada.Text)-1;
			txTemporada.Text=Temporada.ToString();
		}

		private void btTemporadaSiguiente_Click(object sender, System.EventArgs e)
		{
			Temporada=Convert.ToInt32 (txTemporada.Text)+1;
			txTemporada.Text=Temporada.ToString();
		}

		private void btJornadaAnterior_Click(object sender, System.EventArgs e)
		{
			if(Jornada>0)
			{
				Jornada--;
				txJornada.Text = Jornada.ToString();
			}
		}

		private void btJornadaSiguiente_Click(object sender, System.EventArgs e)
		{
			if(Jornada<43)
			{
				Jornada++;
				txJornada.Text = Jornada.ToString();
			}
		}

		private void btGuardar_Click(object sender, System.EventArgs e)
		{
			
			SaveFileDialog abreFiltroDialog = new SaveFileDialog();
			abreFiltroDialog.InitialDirectory = "Lista\\" ;
			abreFiltroDialog.Filter = "Columnas(*.lst)|*.lst|Todos los archivos (*.*)|*.*" ;
			if(abreFiltroDialog.ShowDialog () == DialogResult.OK)
			{		    	
				string ListaArchivos = abreFiltroDialog.FileName;
                IArchivoColumnas comCols = new ArchivoColumnasTexto(ListaArchivos);
	
				foreach (Combinacion Combi in ListaCombinaciones)
				{
					comCols.GuardarCols(Combi.Temporada +" "+ Combi.Jornada +" " + Combi.Path );
				}
				comCols.Cerrar();
			}
		}

		private void btLeer_Click(object sender, System.EventArgs e)
		{
			ListaCombinaciones.Clear ();
			string aux= "";
		
			OpenFileDialog abreFiltroDialog = new OpenFileDialog();
			abreFiltroDialog.InitialDirectory = "Lista\\" ;
			abreFiltroDialog.Filter = "Lista(*.lst)|*.lst|Todos los archivos (*.*)|*.*" ;
			if(abreFiltroDialog.ShowDialog () == DialogResult.OK)
			
			{		    	
				string FicheroLista = abreFiltroDialog.FileName;
                IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(FicheroLista);
				while( comBaseCols.SiguienteColumna() )
				{
					aux=comBaseCols.LeeColumnaSinComas();
					Combinacion Combi = new Combinacion (aux.Substring (0,9),aux.Substring (10,2),aux.Substring(13,aux.Length -13));
					ListaCombinaciones.Add (Combi);
				}
				comBaseCols.Cerrar();					
				dgListaFicheros.DataSource =ListaCombinaciones;
			}
		}

		private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if(tabControl1.SelectedIndex ==0) 
			{
                if (rb14Triples.Checked) Es14Triples = 0; else Es14Triples = 1;
            } 
            else
            {
                Es14Triples=2;
            }
		}
		private bool EsFicheroDeColumnas(string pNombreFichero)
		{
			StreamReader srv = new StreamReader(pNombreFichero);
			bool resultado=true;
			string columna="";
			if (srv.Peek() > -1)
			{
				columna=srv.ReadLine().Trim();
				if (columna.Length < 14)
				{
					resultado=false;
				}
				else
				{
					for(int i=0;i<14;i++)
					{
						if("1xX2".IndexOf (columna[i]) ==-1) {resultado=false;break;}
					}
				}
			}
			else
			{
				resultado=false;
			}
			srv.Close ();
			return resultado;
		}

		private void btAceptar_Click(object sender, System.EventArgs e)
		{
			bool faltandatos=true;
			switch (tabControl1.SelectedTab.Name )
			{
				case "tab14T":
					if(FicheroValoracionesHistoricas !="" && chkTemporadas.CheckedItems .Count>0)
					{faltandatos=false;}
					break;
				case "tabFicheros":
					if(FicheroValoracionesHistoricas !="" && ListaCombinaciones != null)
					{faltandatos=false;}
					break;
			}
			if(faltandatos) 
			{
				MessageBox.Show ("No se ha introducido toda la información necesaria");
			}
			else
			{
				this.Close ();
			}
		}

		private void txJornada_TextChanged(object sender, System.EventArgs e)
		{
			if (txJornada.Text !="") Jornada=Convert.ToInt32 (txJornada.Text);
		}

		private void btTodas_Click(object sender, System.EventArgs e)
		{
			MarcarTodasLasJornadas();
		}
		private void MarcarTodasLasJornadas()
		{
			for (int i=0;i < chkTemporadas.Items.Count ; i++)
			{
				chkTemporadas.SetItemChecked(i, true);
			}
		}

		private void btNinguna_Click(object sender, System.EventArgs e)
		{
			for (int i=0;i < chkTemporadas.Items.Count ; i++)
			{
				chkTemporadas.SetItemChecked(i, false);
			}
		}
		protected void InicializaGridCombinaciones()
		{			
			DataGridTableStyle tableStyle = new DataGridTableStyle();
			tableStyle.MappingName = "ArrayList";
			tableStyle.ColumnHeadersVisible = true;
			
			// Crear Columnas 
			// MappingName tiene que ser igual a cada una de las "properties"
			// de la clase Combinacion.

            
			DataGridTextBoxColumn cs = null;
			//		Temporada
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "Temporada";
			cs.HeaderText = "Temporada";
			cs.Width = 65;
			tableStyle.GridColumnStyles.Add(cs);

			//		Jornada
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "Jornada";
			cs.HeaderText = "Jor.";
			cs.Width = 20;
			tableStyle.GridColumnStyles.Add(cs);
	
			//       Path
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "Path";
			cs.HeaderText = "Fichero";
			cs.Width = 550;
			tableStyle.GridColumnStyles.Add(cs);
			dgListaFicheros.TableStyles.Add(tableStyle);			
		}
		protected void GridDataBind()
		{
			dgListaFicheros.DataSource =null;
			dgListaFicheros.DataSource =ListaCombinaciones;;	
			dgListaFicheros.Refresh ();
		}



        private void btSeleccionarCombi_Click(object sender, EventArgs e)
        {
            OpenFileDialog abreFiltroDialog = new OpenFileDialog();
            abreFiltroDialog.InitialDirectory = "Columnas\\";
            abreFiltroDialog.Filter = "Combinación(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
            if (abreFiltroDialog.ShowDialog() == DialogResult.OK)
            {
                FicheroCombinación = abreFiltroDialog.FileName;
                txFichero.Text = Path.GetFileName(FicheroCombinación);
                rbFichero.Checked = true;
                rb14Triples.Checked = false;
                Es14Triples = 1;
            }
        }
	}
}

