// created on 26/04/2006 at 19:38
// Free1X2 : Programa de quinielas "libre"
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
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.using System;

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text;
using System.IO;
using Free1X2.Utils ;
using Free1X2.EntradaSalida;
using Free1X2.UI.Controls;
using Free1X2.Analisis;


namespace Free1X2.UI
{
	/// <summary>
	/// Summary description for FrmDependenciaLineal.
	/// </summary>
	public class FrmDependenciaLineal : System.Windows.Forms.Form
	{
		private Free1X2.UI.Controls.ControlPorcentajes controlPorcentajesCombinacion;
		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.StatusBarPanel statusBarPanel1;
		private System.Windows.Forms.StatusBarPanel statusBarPanel2;
		private System.Windows.Forms.Button btAceptar;
		private System.Windows.Forms.Button btCancelar;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TextBox TxFicheroEntrada;
		private System.Windows.Forms.Label label1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private string archivoEntrada="";
		private string archivoSalida="";
		private BitArray Bits = new BitArray(4782969,false);
		private BitArray BitsCambiados = new BitArray(4782969,false);
		private int NumApuestas;
		private bool salidaBinaria=false;
		private byte PartidoATratar = 0;
		private bool[,] Pronosticos = new bool [15,3];
		private double[,] PorcentajesSignos = new double[15,3];
		private int[] pot = new int [] {1,3,9,27,81,243,729,2187,6561,19683,59049,177147,531441,1594323};
		private byte TerminoIndependiente=0;
		private byte TerminoCorrectorDobles=1;
		private byte Modulo = 3;
		private byte[] Coef = new byte[15];
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Panel panel5;
		private System.Windows.Forms.Panel panel6;
		private System.Windows.Forms.Panel panel7;
		private System.Windows.Forms.Panel panel8;
		private System.Windows.Forms.Panel panel9;
		private System.Windows.Forms.Label lblNewBase152;
		private System.Windows.Forms.Label lblNewBase15X;
		private System.Windows.Forms.Label lblNewBase151;
		private System.Windows.Forms.Label lblNewBase142;
		private System.Windows.Forms.Label lblNewBase132;
		private System.Windows.Forms.Label lblNewBase122;
		private System.Windows.Forms.Label lblNewBase112;
		private System.Windows.Forms.Label lblNewBase102;
		private System.Windows.Forms.Label lblNewBase92;
		private System.Windows.Forms.Label lblNewBase82;
		private System.Windows.Forms.Label lblNewBase72;
		private System.Windows.Forms.Label lblNewBase62;
		private System.Windows.Forms.Label lblNewBase52;
		private System.Windows.Forms.Label lblNewBase42;
		private System.Windows.Forms.Label lblNewBase32;
		private System.Windows.Forms.Label lblNewBase22;
		private System.Windows.Forms.Label lblNewBase14X;
		private System.Windows.Forms.Label lblNewBase13X;
		private System.Windows.Forms.Label lblNewBase12X;
		private System.Windows.Forms.Label lblNewBase11X;
		private System.Windows.Forms.Label lblNewBase10X;
		private System.Windows.Forms.Label lblNewBase9X;
		private System.Windows.Forms.Label lblNewBase8X;
		private System.Windows.Forms.Label lblNewBase7X;
		private System.Windows.Forms.Label lblNewBase6X;
		private System.Windows.Forms.Label lblNewBase5X;
		private System.Windows.Forms.Label lblNewBase4X;
		private System.Windows.Forms.Label lblNewBase3X;
		private System.Windows.Forms.Label lblNewBase2X;
		private System.Windows.Forms.Label lblNewBase1X;
		private System.Windows.Forms.Label lblNewBase141;
		private System.Windows.Forms.Label lblNewBase131;
		private System.Windows.Forms.Label lblNewBase121;
		private System.Windows.Forms.Label lblNewBase111;
		private System.Windows.Forms.Label lblNewBase101;
		private System.Windows.Forms.Label lblNewBase91;
		private System.Windows.Forms.Label lblNewBase81;
		private System.Windows.Forms.Label lblNewBase71;
		private System.Windows.Forms.Label lblNewBase61;
		private System.Windows.Forms.Label lblNewBase51;
		private System.Windows.Forms.Label lblNewBase41;
		private System.Windows.Forms.Label lblNewBase31;
		private System.Windows.Forms.Label lblNewBase21;
		private System.Windows.Forms.Label LblCoef14;
		private System.Windows.Forms.Label LblCoef13;
		private System.Windows.Forms.Label LblCoef12;
		private System.Windows.Forms.Label LblCoef11;
		private System.Windows.Forms.Label LblCoef10;
		private System.Windows.Forms.Label LblCoef9;
		private System.Windows.Forms.Label LblCoef8;
		private System.Windows.Forms.Label LblCoef7;
		private System.Windows.Forms.Label LblCoef6;
		private System.Windows.Forms.Label LblCoef5;
		private System.Windows.Forms.Label LblCoef4;
		private System.Windows.Forms.Label LblCoef3;
		private System.Windows.Forms.Label LblCoef2;
		private System.Windows.Forms.Label LblCoef1;
		private System.Windows.Forms.Label LblCoef15;
		private System.Windows.Forms.Label lblNewBase11;
		private System.Windows.Forms.Label lblNewBase12;
		private System.Windows.Forms.Label lblInforma;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.TextBox TxFicheroSalida;
		private System.Windows.Forms.Label label4;
		private Porcentajes Pct =new Porcentajes ();

		public FrmDependenciaLineal()
		{
			InitializeComponent();
			
			statusBar1.ShowPanels =true;
			statusBarPanel2.Text ="Falta seleccionar fichero de columnas";
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmDependenciaLineal));
            this.controlPorcentajesCombinacion = new Free1X2.UI.Controls.ControlPorcentajes();
            this.statusBar1 = new System.Windows.Forms.StatusBar();
            this.statusBarPanel1 = new System.Windows.Forms.StatusBarPanel();
            this.statusBarPanel2 = new System.Windows.Forms.StatusBarPanel();
            this.btAceptar = new System.Windows.Forms.Button();
            this.btCancelar = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.TxFicheroEntrada = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.LblCoef14 = new System.Windows.Forms.Label();
            this.LblCoef13 = new System.Windows.Forms.Label();
            this.LblCoef12 = new System.Windows.Forms.Label();
            this.LblCoef11 = new System.Windows.Forms.Label();
            this.LblCoef10 = new System.Windows.Forms.Label();
            this.LblCoef9 = new System.Windows.Forms.Label();
            this.LblCoef8 = new System.Windows.Forms.Label();
            this.LblCoef7 = new System.Windows.Forms.Label();
            this.LblCoef6 = new System.Windows.Forms.Label();
            this.LblCoef5 = new System.Windows.Forms.Label();
            this.LblCoef4 = new System.Windows.Forms.Label();
            this.LblCoef3 = new System.Windows.Forms.Label();
            this.LblCoef2 = new System.Windows.Forms.Label();
            this.LblCoef1 = new System.Windows.Forms.Label();
            this.LblCoef15 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.panel8 = new System.Windows.Forms.Panel();
            this.panel9 = new System.Windows.Forms.Panel();
            this.lblNewBase152 = new System.Windows.Forms.Label();
            this.lblNewBase15X = new System.Windows.Forms.Label();
            this.lblNewBase151 = new System.Windows.Forms.Label();
            this.lblNewBase142 = new System.Windows.Forms.Label();
            this.lblNewBase132 = new System.Windows.Forms.Label();
            this.lblNewBase122 = new System.Windows.Forms.Label();
            this.lblNewBase112 = new System.Windows.Forms.Label();
            this.lblNewBase102 = new System.Windows.Forms.Label();
            this.lblNewBase92 = new System.Windows.Forms.Label();
            this.lblNewBase82 = new System.Windows.Forms.Label();
            this.lblNewBase72 = new System.Windows.Forms.Label();
            this.lblNewBase62 = new System.Windows.Forms.Label();
            this.lblNewBase52 = new System.Windows.Forms.Label();
            this.lblNewBase42 = new System.Windows.Forms.Label();
            this.lblNewBase32 = new System.Windows.Forms.Label();
            this.lblNewBase22 = new System.Windows.Forms.Label();
            this.lblNewBase12 = new System.Windows.Forms.Label();
            this.lblNewBase14X = new System.Windows.Forms.Label();
            this.lblNewBase13X = new System.Windows.Forms.Label();
            this.lblNewBase12X = new System.Windows.Forms.Label();
            this.lblNewBase11X = new System.Windows.Forms.Label();
            this.lblNewBase10X = new System.Windows.Forms.Label();
            this.lblNewBase9X = new System.Windows.Forms.Label();
            this.lblNewBase8X = new System.Windows.Forms.Label();
            this.lblNewBase7X = new System.Windows.Forms.Label();
            this.lblNewBase6X = new System.Windows.Forms.Label();
            this.lblNewBase5X = new System.Windows.Forms.Label();
            this.lblNewBase4X = new System.Windows.Forms.Label();
            this.lblNewBase3X = new System.Windows.Forms.Label();
            this.lblNewBase2X = new System.Windows.Forms.Label();
            this.lblNewBase1X = new System.Windows.Forms.Label();
            this.lblNewBase141 = new System.Windows.Forms.Label();
            this.lblNewBase131 = new System.Windows.Forms.Label();
            this.lblNewBase121 = new System.Windows.Forms.Label();
            this.lblNewBase111 = new System.Windows.Forms.Label();
            this.lblNewBase101 = new System.Windows.Forms.Label();
            this.lblNewBase91 = new System.Windows.Forms.Label();
            this.lblNewBase81 = new System.Windows.Forms.Label();
            this.lblNewBase71 = new System.Windows.Forms.Label();
            this.lblNewBase61 = new System.Windows.Forms.Label();
            this.lblNewBase51 = new System.Windows.Forms.Label();
            this.lblNewBase41 = new System.Windows.Forms.Label();
            this.lblNewBase31 = new System.Windows.Forms.Label();
            this.lblNewBase21 = new System.Windows.Forms.Label();
            this.lblNewBase11 = new System.Windows.Forms.Label();
            this.lblInforma = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.TxFicheroSalida = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            // StatusBarPanel controls don't support ISupportInitialize in .NET 8
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).BeginInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel2)).BeginInit();
            this.SuspendLayout();
            // 
            // controlPorcentajesCombinacion
            // 
            this.controlPorcentajesCombinacion.archivoPorcentajes = null;
            this.controlPorcentajesCombinacion.BackColor = System.Drawing.Color.Bisque;
            this.controlPorcentajesCombinacion.CaptionText = "         %  COMBINACIÓN";
            this.controlPorcentajesCombinacion.FormatoFicheroValoraciones = ((short)(0));
            this.controlPorcentajesCombinacion.Jornada = "01";
            this.controlPorcentajesCombinacion.Location = new System.Drawing.Point(72, 52);
            this.controlPorcentajesCombinacion.Name = "controlPorcentajesCombinacion";
            this.controlPorcentajesCombinacion.ReadOnly = true;
            this.controlPorcentajesCombinacion.Size = new System.Drawing.Size(160, 386);
            this.controlPorcentajesCombinacion.TabIndex = 573;
            this.controlPorcentajesCombinacion.Temporada = "2004/2005";
            // 
            // statusBar1
            // 
            this.statusBar1.Location = new System.Drawing.Point(0, 494);
            this.statusBar1.Name = "statusBar1";
            this.statusBar1.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.statusBarPanel1,
            this.statusBarPanel2});
            this.statusBar1.ShowPanels = true;
            this.statusBar1.Size = new System.Drawing.Size(420, 22);
            this.statusBar1.TabIndex = 572;
            this.statusBar1.Text = "statusBar1";
            // 
            // statusBarPanel1
            // 
            this.statusBarPanel1.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.statusBarPanel1.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.None;
            this.statusBarPanel1.Name = "statusBarPanel1";
            this.statusBarPanel1.Text = "Estado";
            this.statusBarPanel1.Width = 49;
            // 
            // statusBarPanel2
            // 
            this.statusBarPanel2.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.statusBarPanel2.Name = "statusBarPanel2";
            this.statusBarPanel2.Text = "Faltan datos";
            this.statusBarPanel2.Width = 76;
            // 
            // btAceptar
            // 
            this.btAceptar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btAceptar.BackColor = System.Drawing.Color.LightSalmon;
            this.btAceptar.Enabled = false;
            this.btAceptar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btAceptar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btAceptar.Image = ((System.Drawing.Image)(resources.GetObject("btAceptar.Image")));
            this.btAceptar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btAceptar.Location = new System.Drawing.Point(316, 426);
            this.btAceptar.Name = "btAceptar";
            this.btAceptar.Size = new System.Drawing.Size(88, 24);
            this.btAceptar.TabIndex = 571;
            this.btAceptar.Text = "Aceptar";
            this.btAceptar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btAceptar.UseVisualStyleBackColor = false;
            this.btAceptar.Click += new System.EventHandler(this.btAceptar_Click);
            // 
            // btCancelar
            // 
            this.btCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btCancelar.BackColor = System.Drawing.Color.LightSalmon;
            this.btCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btCancelar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btCancelar.Image = ((System.Drawing.Image)(resources.GetObject("btCancelar.Image")));
            this.btCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btCancelar.Location = new System.Drawing.Point(316, 458);
            this.btCancelar.Name = "btCancelar";
            this.btCancelar.Size = new System.Drawing.Size(88, 24);
            this.btCancelar.TabIndex = 570;
            this.btCancelar.Text = "Cancelar";
            this.btCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btCancelar.UseVisualStyleBackColor = false;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.BackColor = System.Drawing.Color.LightSalmon;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.Location = new System.Drawing.Point(388, 8);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(24, 18);
            this.button1.TabIndex = 569;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // TxFicheroEntrada
            // 
            this.TxFicheroEntrada.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TxFicheroEntrada.BackColor = System.Drawing.Color.LemonChiffon;
            this.TxFicheroEntrada.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TxFicheroEntrada.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxFicheroEntrada.Location = new System.Drawing.Point(104, 8);
            this.TxFicheroEntrada.Name = "TxFicheroEntrada";
            this.TxFicheroEntrada.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.TxFicheroEntrada.Size = new System.Drawing.Size(276, 18);
            this.TxFicheroEntrada.TabIndex = 568;
            this.TxFicheroEntrada.Text = "(falta selección)";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Bisque;
            this.label1.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(0, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 18);
            this.label1.TabIndex = 567;
            this.label1.Text = "Fichero de entrada";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.DarkSalmon;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)), true);
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(12, 104);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 40);
            this.label2.TabIndex = 604;
            this.label2.Text = "Pronóstico del Partido a tratar";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.DarkSalmon;
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)), true);
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(232, 104);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 40);
            this.label3.TabIndex = 605;
            this.label3.Text = "Coef.";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // LblCoef14
            // 
            this.LblCoef14.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.LblCoef14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LblCoef14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblCoef14.ForeColor = System.Drawing.Color.Silver;
            this.LblCoef14.Location = new System.Drawing.Point(252, 364);
            this.LblCoef14.Name = "LblCoef14";
            this.LblCoef14.Size = new System.Drawing.Size(22, 18);
            this.LblCoef14.TabIndex = 619;
            this.LblCoef14.Tag = "13";
            this.LblCoef14.Text = "0";
            this.LblCoef14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblCoef14.Click += new System.EventHandler(this.GenericLabel_Click);
            this.LblCoef14.TextChanged += new System.EventHandler(this.GenericLblCoef_TextChanged);
            // 
            // LblCoef13
            // 
            this.LblCoef13.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.LblCoef13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LblCoef13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblCoef13.ForeColor = System.Drawing.Color.Silver;
            this.LblCoef13.Location = new System.Drawing.Point(252, 348);
            this.LblCoef13.Name = "LblCoef13";
            this.LblCoef13.Size = new System.Drawing.Size(22, 18);
            this.LblCoef13.TabIndex = 618;
            this.LblCoef13.Tag = "12";
            this.LblCoef13.Text = "0";
            this.LblCoef13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblCoef13.Click += new System.EventHandler(this.GenericLabel_Click);
            this.LblCoef13.TextChanged += new System.EventHandler(this.GenericLblCoef_TextChanged);
            // 
            // LblCoef12
            // 
            this.LblCoef12.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.LblCoef12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LblCoef12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblCoef12.ForeColor = System.Drawing.Color.Silver;
            this.LblCoef12.Location = new System.Drawing.Point(252, 332);
            this.LblCoef12.Name = "LblCoef12";
            this.LblCoef12.Size = new System.Drawing.Size(22, 18);
            this.LblCoef12.TabIndex = 617;
            this.LblCoef12.Tag = "11";
            this.LblCoef12.Text = "0";
            this.LblCoef12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblCoef12.Click += new System.EventHandler(this.GenericLabel_Click);
            this.LblCoef12.TextChanged += new System.EventHandler(this.GenericLblCoef_TextChanged);
            // 
            // LblCoef11
            // 
            this.LblCoef11.BackColor = System.Drawing.Color.LemonChiffon;
            this.LblCoef11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LblCoef11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblCoef11.ForeColor = System.Drawing.Color.Silver;
            this.LblCoef11.Location = new System.Drawing.Point(252, 312);
            this.LblCoef11.Name = "LblCoef11";
            this.LblCoef11.Size = new System.Drawing.Size(22, 18);
            this.LblCoef11.TabIndex = 616;
            this.LblCoef11.Tag = "10";
            this.LblCoef11.Text = "0";
            this.LblCoef11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblCoef11.Click += new System.EventHandler(this.GenericLabel_Click);
            this.LblCoef11.TextChanged += new System.EventHandler(this.GenericLblCoef_TextChanged);
            // 
            // LblCoef10
            // 
            this.LblCoef10.BackColor = System.Drawing.Color.LemonChiffon;
            this.LblCoef10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LblCoef10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblCoef10.ForeColor = System.Drawing.Color.Silver;
            this.LblCoef10.Location = new System.Drawing.Point(252, 296);
            this.LblCoef10.Name = "LblCoef10";
            this.LblCoef10.Size = new System.Drawing.Size(22, 18);
            this.LblCoef10.TabIndex = 615;
            this.LblCoef10.Tag = "9";
            this.LblCoef10.Text = "0";
            this.LblCoef10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblCoef10.Click += new System.EventHandler(this.GenericLabel_Click);
            this.LblCoef10.TextChanged += new System.EventHandler(this.GenericLblCoef_TextChanged);
            // 
            // LblCoef9
            // 
            this.LblCoef9.BackColor = System.Drawing.Color.LemonChiffon;
            this.LblCoef9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LblCoef9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblCoef9.ForeColor = System.Drawing.Color.Silver;
            this.LblCoef9.Location = new System.Drawing.Point(252, 280);
            this.LblCoef9.Name = "LblCoef9";
            this.LblCoef9.Size = new System.Drawing.Size(22, 18);
            this.LblCoef9.TabIndex = 614;
            this.LblCoef9.Tag = "8";
            this.LblCoef9.Text = "0";
            this.LblCoef9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblCoef9.Click += new System.EventHandler(this.GenericLabel_Click);
            this.LblCoef9.TextChanged += new System.EventHandler(this.GenericLblCoef_TextChanged);
            // 
            // LblCoef8
            // 
            this.LblCoef8.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.LblCoef8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LblCoef8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblCoef8.ForeColor = System.Drawing.Color.Silver;
            this.LblCoef8.Location = new System.Drawing.Point(252, 260);
            this.LblCoef8.Name = "LblCoef8";
            this.LblCoef8.Size = new System.Drawing.Size(22, 18);
            this.LblCoef8.TabIndex = 613;
            this.LblCoef8.Tag = "7";
            this.LblCoef8.Text = "0";
            this.LblCoef8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblCoef8.Click += new System.EventHandler(this.GenericLabel_Click);
            this.LblCoef8.TextChanged += new System.EventHandler(this.GenericLblCoef_TextChanged);
            // 
            // LblCoef7
            // 
            this.LblCoef7.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.LblCoef7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LblCoef7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblCoef7.ForeColor = System.Drawing.Color.Silver;
            this.LblCoef7.Location = new System.Drawing.Point(252, 244);
            this.LblCoef7.Name = "LblCoef7";
            this.LblCoef7.Size = new System.Drawing.Size(22, 18);
            this.LblCoef7.TabIndex = 612;
            this.LblCoef7.Tag = "6";
            this.LblCoef7.Text = "0";
            this.LblCoef7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblCoef7.Click += new System.EventHandler(this.GenericLabel_Click);
            this.LblCoef7.TextChanged += new System.EventHandler(this.GenericLblCoef_TextChanged);
            // 
            // LblCoef6
            // 
            this.LblCoef6.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.LblCoef6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LblCoef6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblCoef6.ForeColor = System.Drawing.Color.Silver;
            this.LblCoef6.Location = new System.Drawing.Point(252, 228);
            this.LblCoef6.Name = "LblCoef6";
            this.LblCoef6.Size = new System.Drawing.Size(22, 18);
            this.LblCoef6.TabIndex = 611;
            this.LblCoef6.Tag = "5";
            this.LblCoef6.Text = "0";
            this.LblCoef6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblCoef6.Click += new System.EventHandler(this.GenericLabel_Click);
            this.LblCoef6.TextChanged += new System.EventHandler(this.GenericLblCoef_TextChanged);
            // 
            // LblCoef5
            // 
            this.LblCoef5.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.LblCoef5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LblCoef5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblCoef5.ForeColor = System.Drawing.Color.Silver;
            this.LblCoef5.Location = new System.Drawing.Point(252, 212);
            this.LblCoef5.Name = "LblCoef5";
            this.LblCoef5.Size = new System.Drawing.Size(22, 18);
            this.LblCoef5.TabIndex = 610;
            this.LblCoef5.Tag = "4";
            this.LblCoef5.Text = "0";
            this.LblCoef5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblCoef5.Click += new System.EventHandler(this.GenericLabel_Click);
            this.LblCoef5.TextChanged += new System.EventHandler(this.GenericLblCoef_TextChanged);
            // 
            // LblCoef4
            // 
            this.LblCoef4.BackColor = System.Drawing.Color.LemonChiffon;
            this.LblCoef4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LblCoef4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblCoef4.ForeColor = System.Drawing.Color.Silver;
            this.LblCoef4.Location = new System.Drawing.Point(252, 192);
            this.LblCoef4.Name = "LblCoef4";
            this.LblCoef4.Size = new System.Drawing.Size(22, 18);
            this.LblCoef4.TabIndex = 609;
            this.LblCoef4.Tag = "3";
            this.LblCoef4.Text = "0";
            this.LblCoef4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblCoef4.Click += new System.EventHandler(this.GenericLabel_Click);
            this.LblCoef4.TextChanged += new System.EventHandler(this.GenericLblCoef_TextChanged);
            // 
            // LblCoef3
            // 
            this.LblCoef3.BackColor = System.Drawing.Color.LemonChiffon;
            this.LblCoef3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LblCoef3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblCoef3.ForeColor = System.Drawing.Color.Silver;
            this.LblCoef3.Location = new System.Drawing.Point(252, 176);
            this.LblCoef3.Name = "LblCoef3";
            this.LblCoef3.Size = new System.Drawing.Size(22, 18);
            this.LblCoef3.TabIndex = 608;
            this.LblCoef3.Tag = "2";
            this.LblCoef3.Text = "0";
            this.LblCoef3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblCoef3.Click += new System.EventHandler(this.GenericLabel_Click);
            this.LblCoef3.TextChanged += new System.EventHandler(this.GenericLblCoef_TextChanged);
            // 
            // LblCoef2
            // 
            this.LblCoef2.BackColor = System.Drawing.Color.LemonChiffon;
            this.LblCoef2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LblCoef2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblCoef2.ForeColor = System.Drawing.Color.Silver;
            this.LblCoef2.Location = new System.Drawing.Point(252, 160);
            this.LblCoef2.Name = "LblCoef2";
            this.LblCoef2.Size = new System.Drawing.Size(22, 18);
            this.LblCoef2.TabIndex = 607;
            this.LblCoef2.Tag = "1";
            this.LblCoef2.Text = "0";
            this.LblCoef2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblCoef2.Click += new System.EventHandler(this.GenericLabel_Click);
            this.LblCoef2.TextChanged += new System.EventHandler(this.GenericLblCoef_TextChanged);
            // 
            // LblCoef1
            // 
            this.LblCoef1.BackColor = System.Drawing.Color.LemonChiffon;
            this.LblCoef1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LblCoef1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblCoef1.ForeColor = System.Drawing.Color.Silver;
            this.LblCoef1.Location = new System.Drawing.Point(252, 144);
            this.LblCoef1.Name = "LblCoef1";
            this.LblCoef1.Size = new System.Drawing.Size(22, 18);
            this.LblCoef1.TabIndex = 606;
            this.LblCoef1.Tag = "0";
            this.LblCoef1.Text = "0";
            this.LblCoef1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblCoef1.Click += new System.EventHandler(this.GenericLabel_Click);
            this.LblCoef1.TextChanged += new System.EventHandler(this.GenericLblCoef_TextChanged);
            // 
            // LblCoef15
            // 
            this.LblCoef15.BackColor = System.Drawing.Color.LemonChiffon;
            this.LblCoef15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LblCoef15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LblCoef15.ForeColor = System.Drawing.Color.Silver;
            this.LblCoef15.Location = new System.Drawing.Point(252, 384);
            this.LblCoef15.Name = "LblCoef15";
            this.LblCoef15.Size = new System.Drawing.Size(22, 18);
            this.LblCoef15.TabIndex = 620;
            this.LblCoef15.Tag = "14";
            this.LblCoef15.Text = "0";
            this.LblCoef15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.LblCoef15.Visible = false;
            this.LblCoef15.Click += new System.EventHandler(this.GenericLabel_Click);
            this.LblCoef15.TextChanged += new System.EventHandler(this.GenericLblCoef_TextChanged);
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.SystemColors.Control;
            this.panel5.Location = new System.Drawing.Point(232, 144);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(60, 68);
            this.panel5.TabIndex = 628;
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.SystemColors.Control;
            this.panel6.Location = new System.Drawing.Point(232, 280);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(60, 52);
            this.panel6.TabIndex = 627;
            // 
            // panel7
            // 
            this.panel7.BackColor = System.Drawing.Color.LemonChiffon;
            this.panel7.Location = new System.Drawing.Point(232, 332);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(60, 52);
            this.panel7.TabIndex = 626;
            // 
            // panel8
            // 
            this.panel8.BackColor = System.Drawing.Color.LemonChiffon;
            this.panel8.Location = new System.Drawing.Point(232, 212);
            this.panel8.Name = "panel8";
            this.panel8.Size = new System.Drawing.Size(60, 68);
            this.panel8.TabIndex = 625;
            // 
            // panel9
            // 
            this.panel9.BackColor = System.Drawing.SystemColors.Control;
            this.panel9.Location = new System.Drawing.Point(12, 444);
            this.panel9.Name = "panel9";
            this.panel9.Size = new System.Drawing.Size(280, 20);
            this.panel9.TabIndex = 629;
            this.panel9.Visible = false;
            // 
            // lblNewBase152
            // 
            this.lblNewBase152.BackColor = System.Drawing.Color.White;
            this.lblNewBase152.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase152.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase152.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase152.Location = new System.Drawing.Point(52, 384);
            this.lblNewBase152.Name = "lblNewBase152";
            this.lblNewBase152.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase152.TabIndex = 674;
            this.lblNewBase152.Tag = "14";
            this.lblNewBase152.Text = "2";
            this.lblNewBase152.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase152.Visible = false;
            this.lblNewBase152.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase15X
            // 
            this.lblNewBase15X.BackColor = System.Drawing.Color.White;
            this.lblNewBase15X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase15X.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase15X.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase15X.Location = new System.Drawing.Point(32, 384);
            this.lblNewBase15X.Name = "lblNewBase15X";
            this.lblNewBase15X.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase15X.TabIndex = 673;
            this.lblNewBase15X.Tag = "14";
            this.lblNewBase15X.Text = "X";
            this.lblNewBase15X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase15X.Visible = false;
            this.lblNewBase15X.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase151
            // 
            this.lblNewBase151.BackColor = System.Drawing.Color.White;
            this.lblNewBase151.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase151.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase151.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase151.Location = new System.Drawing.Point(12, 384);
            this.lblNewBase151.Name = "lblNewBase151";
            this.lblNewBase151.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase151.TabIndex = 672;
            this.lblNewBase151.Tag = "14";
            this.lblNewBase151.Text = "1";
            this.lblNewBase151.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase151.Visible = false;
            this.lblNewBase151.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase142
            // 
            this.lblNewBase142.BackColor = System.Drawing.Color.White;
            this.lblNewBase142.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase142.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase142.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase142.Location = new System.Drawing.Point(52, 364);
            this.lblNewBase142.Name = "lblNewBase142";
            this.lblNewBase142.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase142.TabIndex = 671;
            this.lblNewBase142.Tag = "13";
            this.lblNewBase142.Text = "2";
            this.lblNewBase142.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase142.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase132
            // 
            this.lblNewBase132.BackColor = System.Drawing.Color.White;
            this.lblNewBase132.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase132.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase132.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase132.Location = new System.Drawing.Point(52, 348);
            this.lblNewBase132.Name = "lblNewBase132";
            this.lblNewBase132.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase132.TabIndex = 670;
            this.lblNewBase132.Tag = "12";
            this.lblNewBase132.Text = "2";
            this.lblNewBase132.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase132.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase122
            // 
            this.lblNewBase122.BackColor = System.Drawing.Color.White;
            this.lblNewBase122.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase122.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase122.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase122.Location = new System.Drawing.Point(52, 332);
            this.lblNewBase122.Name = "lblNewBase122";
            this.lblNewBase122.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase122.TabIndex = 669;
            this.lblNewBase122.Tag = "11";
            this.lblNewBase122.Text = "2";
            this.lblNewBase122.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase122.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase112
            // 
            this.lblNewBase112.BackColor = System.Drawing.Color.White;
            this.lblNewBase112.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase112.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase112.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase112.Location = new System.Drawing.Point(52, 312);
            this.lblNewBase112.Name = "lblNewBase112";
            this.lblNewBase112.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase112.TabIndex = 668;
            this.lblNewBase112.Tag = "10";
            this.lblNewBase112.Text = "2";
            this.lblNewBase112.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase112.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase102
            // 
            this.lblNewBase102.BackColor = System.Drawing.Color.White;
            this.lblNewBase102.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase102.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase102.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase102.Location = new System.Drawing.Point(52, 296);
            this.lblNewBase102.Name = "lblNewBase102";
            this.lblNewBase102.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase102.TabIndex = 667;
            this.lblNewBase102.Tag = "9";
            this.lblNewBase102.Text = "2";
            this.lblNewBase102.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase102.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase92
            // 
            this.lblNewBase92.BackColor = System.Drawing.Color.White;
            this.lblNewBase92.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase92.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase92.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase92.Location = new System.Drawing.Point(52, 280);
            this.lblNewBase92.Name = "lblNewBase92";
            this.lblNewBase92.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase92.TabIndex = 666;
            this.lblNewBase92.Tag = "8";
            this.lblNewBase92.Text = "2";
            this.lblNewBase92.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase92.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase82
            // 
            this.lblNewBase82.BackColor = System.Drawing.Color.White;
            this.lblNewBase82.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase82.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase82.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase82.Location = new System.Drawing.Point(52, 260);
            this.lblNewBase82.Name = "lblNewBase82";
            this.lblNewBase82.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase82.TabIndex = 665;
            this.lblNewBase82.Tag = "7";
            this.lblNewBase82.Text = "2";
            this.lblNewBase82.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase82.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase72
            // 
            this.lblNewBase72.BackColor = System.Drawing.Color.White;
            this.lblNewBase72.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase72.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase72.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase72.Location = new System.Drawing.Point(52, 244);
            this.lblNewBase72.Name = "lblNewBase72";
            this.lblNewBase72.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase72.TabIndex = 664;
            this.lblNewBase72.Tag = "6";
            this.lblNewBase72.Text = "2";
            this.lblNewBase72.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase72.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase62
            // 
            this.lblNewBase62.BackColor = System.Drawing.Color.White;
            this.lblNewBase62.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase62.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase62.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase62.Location = new System.Drawing.Point(52, 228);
            this.lblNewBase62.Name = "lblNewBase62";
            this.lblNewBase62.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase62.TabIndex = 663;
            this.lblNewBase62.Tag = "5";
            this.lblNewBase62.Text = "2";
            this.lblNewBase62.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase62.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase52
            // 
            this.lblNewBase52.BackColor = System.Drawing.Color.White;
            this.lblNewBase52.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase52.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase52.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase52.Location = new System.Drawing.Point(52, 212);
            this.lblNewBase52.Name = "lblNewBase52";
            this.lblNewBase52.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase52.TabIndex = 662;
            this.lblNewBase52.Tag = "4";
            this.lblNewBase52.Text = "2";
            this.lblNewBase52.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase52.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase42
            // 
            this.lblNewBase42.BackColor = System.Drawing.Color.White;
            this.lblNewBase42.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase42.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase42.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase42.Location = new System.Drawing.Point(52, 192);
            this.lblNewBase42.Name = "lblNewBase42";
            this.lblNewBase42.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase42.TabIndex = 661;
            this.lblNewBase42.Tag = "3";
            this.lblNewBase42.Text = "2";
            this.lblNewBase42.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase42.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase32
            // 
            this.lblNewBase32.BackColor = System.Drawing.Color.White;
            this.lblNewBase32.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase32.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase32.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase32.Location = new System.Drawing.Point(52, 176);
            this.lblNewBase32.Name = "lblNewBase32";
            this.lblNewBase32.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase32.TabIndex = 660;
            this.lblNewBase32.Tag = "2";
            this.lblNewBase32.Text = "2";
            this.lblNewBase32.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase32.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase22
            // 
            this.lblNewBase22.BackColor = System.Drawing.Color.White;
            this.lblNewBase22.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase22.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase22.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase22.Location = new System.Drawing.Point(52, 160);
            this.lblNewBase22.Name = "lblNewBase22";
            this.lblNewBase22.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase22.TabIndex = 659;
            this.lblNewBase22.Tag = "1";
            this.lblNewBase22.Text = "2";
            this.lblNewBase22.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase22.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase12
            // 
            this.lblNewBase12.BackColor = System.Drawing.Color.White;
            this.lblNewBase12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase12.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase12.Location = new System.Drawing.Point(52, 144);
            this.lblNewBase12.Name = "lblNewBase12";
            this.lblNewBase12.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase12.TabIndex = 658;
            this.lblNewBase12.Tag = "0";
            this.lblNewBase12.Text = "2";
            this.lblNewBase12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase12.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase14X
            // 
            this.lblNewBase14X.BackColor = System.Drawing.Color.White;
            this.lblNewBase14X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase14X.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase14X.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase14X.Location = new System.Drawing.Point(32, 364);
            this.lblNewBase14X.Name = "lblNewBase14X";
            this.lblNewBase14X.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase14X.TabIndex = 657;
            this.lblNewBase14X.Tag = "13";
            this.lblNewBase14X.Text = "X";
            this.lblNewBase14X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase14X.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase13X
            // 
            this.lblNewBase13X.BackColor = System.Drawing.Color.White;
            this.lblNewBase13X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase13X.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase13X.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase13X.Location = new System.Drawing.Point(32, 348);
            this.lblNewBase13X.Name = "lblNewBase13X";
            this.lblNewBase13X.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase13X.TabIndex = 656;
            this.lblNewBase13X.Tag = "12";
            this.lblNewBase13X.Text = "X";
            this.lblNewBase13X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase13X.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase12X
            // 
            this.lblNewBase12X.BackColor = System.Drawing.Color.White;
            this.lblNewBase12X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase12X.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase12X.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase12X.Location = new System.Drawing.Point(32, 332);
            this.lblNewBase12X.Name = "lblNewBase12X";
            this.lblNewBase12X.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase12X.TabIndex = 655;
            this.lblNewBase12X.Tag = "11";
            this.lblNewBase12X.Text = "X";
            this.lblNewBase12X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase12X.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase11X
            // 
            this.lblNewBase11X.BackColor = System.Drawing.Color.White;
            this.lblNewBase11X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase11X.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase11X.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase11X.Location = new System.Drawing.Point(32, 312);
            this.lblNewBase11X.Name = "lblNewBase11X";
            this.lblNewBase11X.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase11X.TabIndex = 654;
            this.lblNewBase11X.Tag = "10";
            this.lblNewBase11X.Text = "X";
            this.lblNewBase11X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase11X.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase10X
            // 
            this.lblNewBase10X.BackColor = System.Drawing.Color.White;
            this.lblNewBase10X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase10X.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase10X.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase10X.Location = new System.Drawing.Point(32, 296);
            this.lblNewBase10X.Name = "lblNewBase10X";
            this.lblNewBase10X.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase10X.TabIndex = 653;
            this.lblNewBase10X.Tag = "9";
            this.lblNewBase10X.Text = "X";
            this.lblNewBase10X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase10X.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase9X
            // 
            this.lblNewBase9X.BackColor = System.Drawing.Color.White;
            this.lblNewBase9X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase9X.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase9X.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase9X.Location = new System.Drawing.Point(32, 280);
            this.lblNewBase9X.Name = "lblNewBase9X";
            this.lblNewBase9X.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase9X.TabIndex = 652;
            this.lblNewBase9X.Tag = "8";
            this.lblNewBase9X.Text = "X";
            this.lblNewBase9X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase9X.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase8X
            // 
            this.lblNewBase8X.BackColor = System.Drawing.Color.White;
            this.lblNewBase8X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase8X.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase8X.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase8X.Location = new System.Drawing.Point(32, 260);
            this.lblNewBase8X.Name = "lblNewBase8X";
            this.lblNewBase8X.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase8X.TabIndex = 651;
            this.lblNewBase8X.Tag = "7";
            this.lblNewBase8X.Text = "X";
            this.lblNewBase8X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase8X.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase7X
            // 
            this.lblNewBase7X.BackColor = System.Drawing.Color.White;
            this.lblNewBase7X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase7X.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase7X.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase7X.Location = new System.Drawing.Point(32, 244);
            this.lblNewBase7X.Name = "lblNewBase7X";
            this.lblNewBase7X.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase7X.TabIndex = 650;
            this.lblNewBase7X.Tag = "6";
            this.lblNewBase7X.Text = "X";
            this.lblNewBase7X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase7X.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase6X
            // 
            this.lblNewBase6X.BackColor = System.Drawing.Color.White;
            this.lblNewBase6X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase6X.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase6X.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase6X.Location = new System.Drawing.Point(32, 228);
            this.lblNewBase6X.Name = "lblNewBase6X";
            this.lblNewBase6X.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase6X.TabIndex = 649;
            this.lblNewBase6X.Tag = "5";
            this.lblNewBase6X.Text = "X";
            this.lblNewBase6X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase6X.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase5X
            // 
            this.lblNewBase5X.BackColor = System.Drawing.Color.White;
            this.lblNewBase5X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase5X.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase5X.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase5X.Location = new System.Drawing.Point(32, 212);
            this.lblNewBase5X.Name = "lblNewBase5X";
            this.lblNewBase5X.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase5X.TabIndex = 648;
            this.lblNewBase5X.Tag = "4";
            this.lblNewBase5X.Text = "X";
            this.lblNewBase5X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase5X.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase4X
            // 
            this.lblNewBase4X.BackColor = System.Drawing.Color.White;
            this.lblNewBase4X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase4X.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase4X.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase4X.Location = new System.Drawing.Point(32, 192);
            this.lblNewBase4X.Name = "lblNewBase4X";
            this.lblNewBase4X.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase4X.TabIndex = 647;
            this.lblNewBase4X.Tag = "3";
            this.lblNewBase4X.Text = "X";
            this.lblNewBase4X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase4X.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase3X
            // 
            this.lblNewBase3X.BackColor = System.Drawing.Color.White;
            this.lblNewBase3X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase3X.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase3X.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase3X.Location = new System.Drawing.Point(32, 176);
            this.lblNewBase3X.Name = "lblNewBase3X";
            this.lblNewBase3X.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase3X.TabIndex = 646;
            this.lblNewBase3X.Tag = "2";
            this.lblNewBase3X.Text = "X";
            this.lblNewBase3X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase3X.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase2X
            // 
            this.lblNewBase2X.BackColor = System.Drawing.Color.White;
            this.lblNewBase2X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase2X.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase2X.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase2X.Location = new System.Drawing.Point(32, 160);
            this.lblNewBase2X.Name = "lblNewBase2X";
            this.lblNewBase2X.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase2X.TabIndex = 645;
            this.lblNewBase2X.Tag = "1";
            this.lblNewBase2X.Text = "X";
            this.lblNewBase2X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase2X.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase1X
            // 
            this.lblNewBase1X.BackColor = System.Drawing.Color.White;
            this.lblNewBase1X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase1X.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase1X.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase1X.Location = new System.Drawing.Point(32, 144);
            this.lblNewBase1X.Name = "lblNewBase1X";
            this.lblNewBase1X.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase1X.TabIndex = 644;
            this.lblNewBase1X.Tag = "0";
            this.lblNewBase1X.Text = "X";
            this.lblNewBase1X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase1X.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase141
            // 
            this.lblNewBase141.BackColor = System.Drawing.Color.White;
            this.lblNewBase141.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase141.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase141.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase141.Location = new System.Drawing.Point(12, 364);
            this.lblNewBase141.Name = "lblNewBase141";
            this.lblNewBase141.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase141.TabIndex = 643;
            this.lblNewBase141.Tag = "13";
            this.lblNewBase141.Text = "1";
            this.lblNewBase141.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase141.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase131
            // 
            this.lblNewBase131.BackColor = System.Drawing.Color.White;
            this.lblNewBase131.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase131.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase131.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase131.Location = new System.Drawing.Point(12, 348);
            this.lblNewBase131.Name = "lblNewBase131";
            this.lblNewBase131.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase131.TabIndex = 642;
            this.lblNewBase131.Tag = "12";
            this.lblNewBase131.Text = "1";
            this.lblNewBase131.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase131.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase121
            // 
            this.lblNewBase121.BackColor = System.Drawing.Color.White;
            this.lblNewBase121.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase121.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase121.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase121.Location = new System.Drawing.Point(12, 332);
            this.lblNewBase121.Name = "lblNewBase121";
            this.lblNewBase121.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase121.TabIndex = 641;
            this.lblNewBase121.Tag = "11";
            this.lblNewBase121.Text = "1";
            this.lblNewBase121.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase121.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase111
            // 
            this.lblNewBase111.BackColor = System.Drawing.Color.White;
            this.lblNewBase111.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase111.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase111.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase111.Location = new System.Drawing.Point(12, 312);
            this.lblNewBase111.Name = "lblNewBase111";
            this.lblNewBase111.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase111.TabIndex = 640;
            this.lblNewBase111.Tag = "10";
            this.lblNewBase111.Text = "1";
            this.lblNewBase111.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase111.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase101
            // 
            this.lblNewBase101.BackColor = System.Drawing.Color.White;
            this.lblNewBase101.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase101.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase101.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase101.Location = new System.Drawing.Point(12, 296);
            this.lblNewBase101.Name = "lblNewBase101";
            this.lblNewBase101.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase101.TabIndex = 639;
            this.lblNewBase101.Tag = "9";
            this.lblNewBase101.Text = "1";
            this.lblNewBase101.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase101.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase91
            // 
            this.lblNewBase91.BackColor = System.Drawing.Color.White;
            this.lblNewBase91.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase91.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase91.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase91.Location = new System.Drawing.Point(12, 280);
            this.lblNewBase91.Name = "lblNewBase91";
            this.lblNewBase91.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase91.TabIndex = 638;
            this.lblNewBase91.Tag = "8";
            this.lblNewBase91.Text = "1";
            this.lblNewBase91.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase91.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase81
            // 
            this.lblNewBase81.BackColor = System.Drawing.Color.White;
            this.lblNewBase81.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase81.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase81.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase81.Location = new System.Drawing.Point(12, 260);
            this.lblNewBase81.Name = "lblNewBase81";
            this.lblNewBase81.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase81.TabIndex = 637;
            this.lblNewBase81.Tag = "7";
            this.lblNewBase81.Text = "1";
            this.lblNewBase81.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase81.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase71
            // 
            this.lblNewBase71.BackColor = System.Drawing.Color.White;
            this.lblNewBase71.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase71.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase71.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase71.Location = new System.Drawing.Point(12, 244);
            this.lblNewBase71.Name = "lblNewBase71";
            this.lblNewBase71.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase71.TabIndex = 636;
            this.lblNewBase71.Tag = "6";
            this.lblNewBase71.Text = "1";
            this.lblNewBase71.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase71.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase61
            // 
            this.lblNewBase61.BackColor = System.Drawing.Color.White;
            this.lblNewBase61.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase61.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase61.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase61.Location = new System.Drawing.Point(12, 228);
            this.lblNewBase61.Name = "lblNewBase61";
            this.lblNewBase61.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase61.TabIndex = 635;
            this.lblNewBase61.Tag = "5";
            this.lblNewBase61.Text = "1";
            this.lblNewBase61.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase61.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase51
            // 
            this.lblNewBase51.BackColor = System.Drawing.Color.White;
            this.lblNewBase51.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase51.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase51.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase51.Location = new System.Drawing.Point(12, 212);
            this.lblNewBase51.Name = "lblNewBase51";
            this.lblNewBase51.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase51.TabIndex = 634;
            this.lblNewBase51.Tag = "4";
            this.lblNewBase51.Text = "1";
            this.lblNewBase51.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase51.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase41
            // 
            this.lblNewBase41.BackColor = System.Drawing.Color.White;
            this.lblNewBase41.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase41.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase41.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase41.Location = new System.Drawing.Point(12, 192);
            this.lblNewBase41.Name = "lblNewBase41";
            this.lblNewBase41.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase41.TabIndex = 633;
            this.lblNewBase41.Tag = "3";
            this.lblNewBase41.Text = "1";
            this.lblNewBase41.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase41.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase31
            // 
            this.lblNewBase31.BackColor = System.Drawing.Color.White;
            this.lblNewBase31.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase31.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase31.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase31.Location = new System.Drawing.Point(12, 176);
            this.lblNewBase31.Name = "lblNewBase31";
            this.lblNewBase31.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase31.TabIndex = 632;
            this.lblNewBase31.Tag = "2";
            this.lblNewBase31.Text = "1";
            this.lblNewBase31.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase31.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase21
            // 
            this.lblNewBase21.BackColor = System.Drawing.Color.White;
            this.lblNewBase21.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase21.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase21.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase21.Location = new System.Drawing.Point(12, 160);
            this.lblNewBase21.Name = "lblNewBase21";
            this.lblNewBase21.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase21.TabIndex = 631;
            this.lblNewBase21.Tag = "1";
            this.lblNewBase21.Text = "1";
            this.lblNewBase21.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase21.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase11
            // 
            this.lblNewBase11.BackColor = System.Drawing.Color.White;
            this.lblNewBase11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase11.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase11.Location = new System.Drawing.Point(12, 144);
            this.lblNewBase11.Name = "lblNewBase11";
            this.lblNewBase11.Size = new System.Drawing.Size(18, 18);
            this.lblNewBase11.TabIndex = 630;
            this.lblNewBase11.Tag = "0";
            this.lblNewBase11.Text = "1";
            this.lblNewBase11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase11.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblInforma
            // 
            this.lblInforma.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInforma.ForeColor = System.Drawing.Color.Maroon;
            this.lblInforma.Location = new System.Drawing.Point(304, 108);
            this.lblInforma.Name = "lblInforma";
            this.lblInforma.Size = new System.Drawing.Size(112, 168);
            this.lblInforma.TabIndex = 676;
            this.lblInforma.Text = "Los signos del partido a tratar se calcularán a partir de los signos de los parti" +
                "Dos con coeficiente distinto de 0";
            this.lblInforma.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.BackColor = System.Drawing.Color.LightSalmon;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button2.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button2.Image = ((System.Drawing.Image)(resources.GetObject("button2.Image")));
            this.button2.Location = new System.Drawing.Point(388, 36);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(24, 18);
            this.button2.TabIndex = 678;
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // TxFicheroSalida
            // 
            this.TxFicheroSalida.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TxFicheroSalida.BackColor = System.Drawing.Color.LemonChiffon;
            this.TxFicheroSalida.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TxFicheroSalida.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxFicheroSalida.Location = new System.Drawing.Point(104, 36);
            this.TxFicheroSalida.Name = "TxFicheroSalida";
            this.TxFicheroSalida.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.TxFicheroSalida.Size = new System.Drawing.Size(276, 18);
            this.TxFicheroSalida.TabIndex = 677;
            this.TxFicheroSalida.Text = "(falta selección)";
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Bisque;
            this.label4.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(0, 36);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(102, 18);
            this.label4.TabIndex = 679;
            this.label4.Text = "Fichero de salida";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FrmDependenciaLineal
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(420, 516);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.TxFicheroSalida);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lblInforma);
            this.Controls.Add(this.lblNewBase152);
            this.Controls.Add(this.lblNewBase15X);
            this.Controls.Add(this.lblNewBase151);
            this.Controls.Add(this.lblNewBase142);
            this.Controls.Add(this.lblNewBase132);
            this.Controls.Add(this.lblNewBase122);
            this.Controls.Add(this.lblNewBase112);
            this.Controls.Add(this.lblNewBase102);
            this.Controls.Add(this.lblNewBase92);
            this.Controls.Add(this.lblNewBase82);
            this.Controls.Add(this.lblNewBase72);
            this.Controls.Add(this.lblNewBase62);
            this.Controls.Add(this.lblNewBase52);
            this.Controls.Add(this.lblNewBase42);
            this.Controls.Add(this.lblNewBase32);
            this.Controls.Add(this.lblNewBase22);
            this.Controls.Add(this.lblNewBase12);
            this.Controls.Add(this.lblNewBase14X);
            this.Controls.Add(this.lblNewBase13X);
            this.Controls.Add(this.lblNewBase12X);
            this.Controls.Add(this.lblNewBase11X);
            this.Controls.Add(this.lblNewBase10X);
            this.Controls.Add(this.lblNewBase9X);
            this.Controls.Add(this.lblNewBase8X);
            this.Controls.Add(this.lblNewBase7X);
            this.Controls.Add(this.lblNewBase6X);
            this.Controls.Add(this.lblNewBase5X);
            this.Controls.Add(this.lblNewBase4X);
            this.Controls.Add(this.lblNewBase3X);
            this.Controls.Add(this.lblNewBase2X);
            this.Controls.Add(this.lblNewBase1X);
            this.Controls.Add(this.lblNewBase141);
            this.Controls.Add(this.lblNewBase131);
            this.Controls.Add(this.lblNewBase121);
            this.Controls.Add(this.lblNewBase111);
            this.Controls.Add(this.lblNewBase101);
            this.Controls.Add(this.lblNewBase91);
            this.Controls.Add(this.lblNewBase81);
            this.Controls.Add(this.lblNewBase71);
            this.Controls.Add(this.lblNewBase61);
            this.Controls.Add(this.lblNewBase51);
            this.Controls.Add(this.lblNewBase41);
            this.Controls.Add(this.lblNewBase31);
            this.Controls.Add(this.lblNewBase21);
            this.Controls.Add(this.lblNewBase11);
            this.Controls.Add(this.TxFicheroEntrada);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LblCoef15);
            this.Controls.Add(this.LblCoef14);
            this.Controls.Add(this.LblCoef13);
            this.Controls.Add(this.LblCoef12);
            this.Controls.Add(this.LblCoef11);
            this.Controls.Add(this.LblCoef10);
            this.Controls.Add(this.LblCoef9);
            this.Controls.Add(this.LblCoef8);
            this.Controls.Add(this.LblCoef7);
            this.Controls.Add(this.LblCoef6);
            this.Controls.Add(this.LblCoef5);
            this.Controls.Add(this.LblCoef4);
            this.Controls.Add(this.LblCoef3);
            this.Controls.Add(this.LblCoef2);
            this.Controls.Add(this.LblCoef1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.controlPorcentajesCombinacion);
            this.Controls.Add(this.statusBar1);
            this.Controls.Add(this.btAceptar);
            this.Controls.Add(this.btCancelar);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.panel7);
            this.Controls.Add(this.panel8);
            this.Controls.Add(this.panel9);
            this.Name = "FrmDependenciaLineal";
            this.Text = "Dependencia lineal de signos";
            // StatusBarPanel controls don't support ISupportInitialize in .NET 8
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).EndInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion
		private void button1_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog abreFiltroDialog = new OpenFileDialog();
			abreFiltroDialog.InitialDirectory = "Columnas\\" ;
			abreFiltroDialog.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;

			if(abreFiltroDialog.ShowDialog() == DialogResult.OK) 
			{	
				string archVal = Path.GetFileName(abreFiltroDialog.FileName);
				archivoEntrada=abreFiltroDialog.FileName;
				TxFicheroEntrada .Text =Path.GetFileName(archivoEntrada);
				LeerColumnas();
				ProponerCoeficientes();
				Pronosticos [PartidoATratar,0]=true;
				Pronosticos [PartidoATratar,1]=true;
				Pronosticos [PartidoATratar,2]=true;
				statusBarPanel2.Text ="Seleccionar partido e indicar coeficientes";
				if(archivoSalida =="")
				{
					archivoSalida=archivoEntrada;
					TxFicheroSalida.Text =TxFicheroEntrada.Text;
				}
				btAceptar.Enabled =true;
			}
		}
		private void LeerColumnas() 
		{
			Cursor.Current = Cursors.WaitCursor ;
			statusBarPanel2.Text ="Leyendo columnas...";
			Application.DoEvents();
            IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(archivoEntrada);
			Bits.SetAll (false);
			NumApuestas=0;
			int Num;
			string Columna;
			Array.Clear (PorcentajesSignos,0,42);

			ConvertidorDeBases col= new ConvertidorDeBases();

			while( comBaseCols.SiguienteColumna() )
			{
				Columna = comBaseCols.LeeColumnaSinComas();
				Num = col.ConvColumnaANumero  (Columna);
				Bits[Num]=true;
				NumApuestas++;
				ContarSignos(Num);
			}
			comBaseCols.Cerrar();	
			statusBarPanel2.Text ="Num. apuestas " + NumApuestas.ToString ();
			Application.DoEvents();
			ConvertirAPorcentaje();
			MostrarPorcentajesSignos();

			Cursor.Current = Cursors.Default;
			Application.DoEvents();
		}

		private void ContarSignos(int Num)
		{
			for (int Partido = 0; Partido<14; Partido++)
			{
				PorcentajesSignos[Partido, ((Num / pot[Partido]) % 3)]++;
			}
		}
		private void MostrarPorcentajesSignos()
		{
			controlPorcentajesCombinacion.Valores =PorcentajesSignos;
		}
		private void ConvertirAPorcentaje ()
		{
			double Suma;
			for (int i=0;i<14;i++)
			{
				Suma=PorcentajesSignos[i,0]+PorcentajesSignos[i,1]+PorcentajesSignos[i,2];
				PorcentajesSignos[i,2]=Math.Round (PorcentajesSignos[i,2]*100/Suma,0);
				PorcentajesSignos[i,1]=Math.Round (PorcentajesSignos[i,1]*100/Suma,0);
				PorcentajesSignos[i,0]=100-PorcentajesSignos[i,2]-PorcentajesSignos[i,1];
			}
		}
		private void btAceptar_Click(object sender, System.EventArgs e)
		{
			int i;
			byte Partido=0;
			int Indice=0;
			int NuevoSigno=0;
			int Signo=0;
			int SigIni=0;
			if (!TestSignosPartidoATratar()) 
			{
				statusBarPanel2.Text ="Debe poner un triple o un doble en el partido a tratar";
				return;
			}
			BitsCambiados.SetAll (false);


			for(i=0;i<4782969;i++)
			{
				if (Bits[i])
				{
					NuevoSigno=0;
					SigIni=((i / pot[PartidoATratar ]) % 3);
					
					for(Partido=0;Partido<14;Partido++)
					{
						if(Partido == PartidoATratar ) continue;
						Signo = Coef[Partido]*((i / pot[Partido]) % 3);
						NuevoSigno += Signo;
					}
					NuevoSigno %= Modulo;
					NuevoSigno *= TerminoCorrectorDobles ;
					NuevoSigno += TerminoIndependiente;
					Indice = i+pot[PartidoATratar] * (NuevoSigno - SigIni);
					BitsCambiados[Indice]=true;
				}
			}
			statusBarPanel2.Text ="Grabando columnas...";
			Application.DoEvents();
			GrabarColumnas();
		}
		private bool TestSignosPartidoATratar()
		{
			bool ret = false;
			//----------- Pronostico a 1X2 -------------
			if(Pronosticos [PartidoATratar,0]==true 
				&& Pronosticos [PartidoATratar,1]==true 
				&& Pronosticos [PartidoATratar,2]==true)
			{
				Modulo =3;
				TerminoCorrectorDobles =1;
				TerminoIndependiente =0;
				ret =true;
			}
			//----------- Pronostico a 1X -------------
			if(Pronosticos [PartidoATratar,0]==true 
				&& Pronosticos [PartidoATratar,1]==true 
				&& Pronosticos [PartidoATratar,2]==false)
			{
				Modulo =2;
				this.TerminoCorrectorDobles =1;
				this.TerminoIndependiente =0;
				ret =true;
			}
			//----------- Pronostico a X2 -------------
			if(Pronosticos [PartidoATratar,0]==false 
				&& Pronosticos [PartidoATratar,1]==true 
				&& Pronosticos [PartidoATratar,2]==true)
			{
				Modulo =2;
				TerminoCorrectorDobles =1;
				TerminoIndependiente =1;
				ret =true;
			}
			//----------- Pronostico a 12 -------------
			if(Pronosticos [PartidoATratar,0]==true 
				&& Pronosticos [PartidoATratar,1]==false 
				&& Pronosticos [PartidoATratar,2]==true)
			{
				Modulo =2;
				TerminoCorrectorDobles =2;
				TerminoIndependiente =0;
				ret =true;
			}
			return ret;
		}
		private void GrabarColumnas()
		{						
			ConvertidorDeBases con =new ConvertidorDeBases();
            IArchivoColumnas Cols = new ArchivoColumnasTexto(this.archivoSalida);
			int c=0;

			for (int i=0; i<4782969; i++) 
			{
				if (BitsCambiados [i])
				{					
					Cols.GuardarCols(con.ConvNumAColumna(i) );
					c++;
				}
			}		
			Cols.Cerrar();	
			statusBarPanel2.Text ="Se han grabado "+ c.ToString () + " columnas";
			Application.DoEvents();
			LeerColumnas();
		}
		private void GenericLabel_Click(object sender, System.EventArgs e)
		{
			Label MiLabel =(Label)sender;
			switch (MiLabel.Text)
			{
				case "0" :
				{
					MiLabel.Text="1";
					break;
				}
				case "1" :
				{
					MiLabel.Text="2";
					break;
				}
				case "2":
				{
					MiLabel.Text="0";
					break;
				}
			}
			byte Partido = Convert.ToByte (MiLabel.Tag);
			Coef[Partido] = Convert.ToByte (MiLabel.Text) ;

		}
		private void Generic_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{

			if (e.Button == MouseButtons.Right) 
			{ 
				Label MiLabel =(Label)sender;

				foreach (object Objeto in Controls )
				{
					try 
					{
						Label L = (Label) Objeto;
						if(L.Name.IndexOf ("lblNewBase") !=-1)
						{
							if(L.Tag == MiLabel.Tag)
							{
								GenericLabelPronostico_Click(L,e);
							}
						}
					}
					catch
					{
						continue;
					}
				}
			}
			else
			{
				GenericLabelPronostico_Click(sender,e);
			}
		}
		private void GenericLabelPronostico_Click(object sender, System.EventArgs e)
		{
			Label MiLabel =(Label)sender;
			PartidoATratar = Convert.ToByte(MiLabel.Tag );
			int Signo = "1X2".IndexOf (MiLabel.Text );
			InicializarLabelsPronostico(PartidoATratar);
			Pronosticos[PartidoATratar,Signo]=PonerColorEnLabel (MiLabel);
		}
		private void InicializarLabelsPronostico(byte Partido)
		{
			foreach (object Objeto in Controls )
			{
				try 
				{
					Label L = (Label) Objeto;
					if(L.Name.IndexOf ("lblNewBase") !=-1)
					{
						byte Parti = Convert.ToByte (L.Tag);
						if(Partido == Parti) continue;
						Pronosticos[Parti,0]=false;
						Pronosticos[Parti,1]=false;
						Pronosticos[Parti,2]=false;
						L.ForeColor = Color.Pink   ;
						L.BackColor = Color.White ;
					}
				}
				catch
				{
					continue;
				}
			}
		}
		private bool PonerColorEnLabel (Label L)
		{
			if(L.BackColor == Color.Red )
			{
				L.ForeColor = Color.Pink   ;
				L.BackColor = Color.White ;
				return false;
			}
			else
			{
				L.ForeColor = Color.White   ;
				L.BackColor =Color.Red ;
				return true;
			}
		}
		private void ProponerCoeficientes()
		{
			byte i;
			PartidoATratar =255;
			for(i=0;i<15;i++)
			{
				if(PorcentajesSignos [i,0]*PorcentajesSignos [i,1]*PorcentajesSignos [i,2] !=0)
				{
					Coef[i]=1;
				}
				else
				{
					Coef[i]=0;
					if(PartidoATratar==255) PartidoATratar=i;
				}
			}
			foreach (object Objeto in Controls )
			{
				try 
				{
					Label L = (Label) Objeto;
					if(L.Name.IndexOf ("LblCoef") !=-1)
					{
						byte Parti = Convert.ToByte (L.Tag);
						L.Text = Coef[Parti].ToString ();
					}
					if(L.Name.IndexOf ("lblNewBase") !=-1)
					{
						byte Parti = Convert.ToByte (L.Tag);
						if(Parti==PartidoATratar)
						{
							L.ForeColor = Color.White   ;
							L.BackColor =Color.Red ;
						}
						else
						{
							L.ForeColor = Color.Pink   ;
							L.BackColor = Color.White ;
						}
					}
				}
				catch
				{
					continue;
				}
			}
		}

		private void GenericLblCoef_TextChanged(object sender, System.EventArgs e)
		{
			Label MiLabel = (Label) sender;
			if(MiLabel.Text=="0")
			{
				MiLabel.ForeColor =Color.Silver;
			}
			else
			{
				MiLabel.ForeColor =Color.Black;
			}
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
			SaveFileDialog abreFiltroDialog = new SaveFileDialog();
			abreFiltroDialog.InitialDirectory = "Columnas\\" ;
			abreFiltroDialog.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			if(abreFiltroDialog.ShowDialog () == DialogResult.OK)
			{
				salidaBinaria=abreFiltroDialog.FilterIndex==2;
				archivoSalida = abreFiltroDialog.FileName;
				TxFicheroSalida.Text = Path.GetFileName(archivoSalida);
			}
		}
	}
}
