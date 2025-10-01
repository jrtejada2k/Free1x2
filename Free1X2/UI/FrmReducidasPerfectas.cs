// created on 17/04/2006 at 11:58
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
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.

using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace Free1X2.UI
{
	/// <summary>
	/// Summary description for FrmReducidasPerfectas.
	/// </summary>
	public class FrmReducidasPerfectas : Form
	{
        private Label label4;
        private LinkLabel linkLabel1;
        private Label label2;
        private Label label25;
        private TextBox textBox1;
        private TextBox txLongColumna;
        private Label label22;
        private TextBox txtReduccionesContempladas;
        private Label lblSeContemplan;
        private Label label3;
        private Label lblNewBase152;
        private Label lblNewBase15X;
        private Label lblNewBase151;
        private Label lblBase14;
        private Label lblBase13;
        private Label lblBase12;
        private Label lblBase11;
        private Label lblBase10;
        private Label lblBase9;
        private Label lblBase8;
        private Label lblBase7;
        private Label lblBase6;
        private Label lblBase5;
        private Label lblBase4;
        private Label lblBase3;
        private Label lblBase2;
        private Label lblBase1;
        private Label lblNewBase142;
        private Label lblNewBase132;
        private Label lblNewBase122;
        private Label lblNewBase112;
        private Label lblNewBase102;
        private Label lblNewBase92;
        private Label lblNewBase82;
        private Label lblNewBase72;
        private Label lblNewBase62;
        private Label lblNewBase52;
        private Label lblNewBase42;
        private Label lblNewBase32;
        private Label lblNewBase22;
        private Label lblNewBase12;
        private Label lblNewBase14X;
        private Label lblNewBase13X;
        private Label lblNewBase12X;
        private Label lblNewBase11X;
        private Label lblNewBase10X;
        private Label lblNewBase9X;
        private Label lblNewBase8X;
        private Label lblNewBase7X;
        private Label lblNewBase6X;
        private Label lblNewBase5X;
        private Label lblNewBase4X;
        private Label lblNewBase3X;
        private Label lblNewBase2X;
        private Label lblNewBase1X;
        private Label lblNewBase141;
        private Label lblNewBase131;
        private Label lblNewBase121;
        private Label lblNewBase111;
        private Label lblNewBase101;
        private Label lblNewBase91;
        private Label lblNewBase81;
        private Label lblNewBase71;
        private Label lblNewBase61;
        private Label lblNewBase51;
        private Label lblNewBase41;
        private Label lblNewBase31;
        private Label lblNewBase21;
        private Label lblNewBase11;
        private Button btGenerar;
        private StatusBar statusBar1;
        private StatusBarPanel statusBarPanel1;
        private StatusBarPanel statusBarPanel2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private byte[,] M4TR13 = new byte[,] {{1,1},{1,2}};
		private byte[,] M13TR13 = new byte[,] {{0,0,1,1,1,1,1,1,1,1},{1,1,0,0,1,1,1,2,2,2 },{1,2,1,2,0,1,2,0,1,2}};
		private byte[,] M7DR13 = new byte[,] {{0,1,1,1},{1,0,1,1},{1,1,0,1}};
		private byte[,] M11TR12 = new byte[,] {{1,0,1,2,2,2},{1,1,0,1,1,2},{1,1,2,0,2,1},{1,2,1,1,0,1},{1,2,2,2,1,0}};
		private byte[,] M15DR13 = new byte[,] {{1,0,1,1,1,0,0,0,1,1,1},{1,1,0,1,1,0,1,1,0,0,1},{1,1,1,0,1,1,0,1,0,1,0},{1,1,1,1,0,1,1,0,1,0,0}};
		private bool[] Involucrados = new bool[15];
		private string nombreArchivo="";
		private int[] SignosBase = new int[15];
		private bool[,] Pronosticos = new bool [15,3];
		private Label label1;
		private Button btSeleccionarFichero;
		private TextBox txNombreArchivo;
	
		private System.ComponentModel.Container components = null;

		public FrmReducidasPerfectas()
		{
			InitializeComponent();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmReducidasPerfectas));
            this.label4 = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.txLongColumna = new System.Windows.Forms.TextBox();
            this.label22 = new System.Windows.Forms.Label();
            this.txtReduccionesContempladas = new System.Windows.Forms.TextBox();
            this.lblSeContemplan = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblNewBase152 = new System.Windows.Forms.Label();
            this.lblNewBase15X = new System.Windows.Forms.Label();
            this.lblNewBase151 = new System.Windows.Forms.Label();
            this.lblBase14 = new System.Windows.Forms.Label();
            this.lblBase13 = new System.Windows.Forms.Label();
            this.lblBase12 = new System.Windows.Forms.Label();
            this.lblBase11 = new System.Windows.Forms.Label();
            this.lblBase10 = new System.Windows.Forms.Label();
            this.lblBase9 = new System.Windows.Forms.Label();
            this.lblBase8 = new System.Windows.Forms.Label();
            this.lblBase7 = new System.Windows.Forms.Label();
            this.lblBase6 = new System.Windows.Forms.Label();
            this.lblBase5 = new System.Windows.Forms.Label();
            this.lblBase4 = new System.Windows.Forms.Label();
            this.lblBase3 = new System.Windows.Forms.Label();
            this.lblBase2 = new System.Windows.Forms.Label();
            this.lblBase1 = new System.Windows.Forms.Label();
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
            this.btGenerar = new System.Windows.Forms.Button();
            this.statusBar1 = new System.Windows.Forms.StatusBar();
            this.statusBarPanel1 = new System.Windows.Forms.StatusBarPanel();
            this.statusBarPanel2 = new System.Windows.Forms.StatusBarPanel();
            this.txNombreArchivo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btSeleccionarFichero = new System.Windows.Forms.Button();
            // StatusBarPanel controls don't support ISupportInitialize in .NET 8
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).BeginInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel2)).BeginInit();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(16, 340);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(236, 16);
            this.label4.TabIndex = 617;
            this.label4.Text = "M�todo de obtenci�n descrito por Fortuna en:";
            // 
            // linkLabel1
            // 
            this.linkLabel1.Location = new System.Drawing.Point(16, 356);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(236, 16);
            this.linkLabel1.TabIndex = 616;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "http://www.foro1x2.com/viewtopic.php?t=4445";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Maroon;
            this.label2.Location = new System.Drawing.Point(84, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 20);
            this.label2.TabIndex = 612;
            this.label2.Text = "Base";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label25
            // 
            this.label25.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.ForeColor = System.Drawing.Color.Maroon;
            this.label25.Location = new System.Drawing.Point(12, 44);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(68, 20);
            this.label25.TabIndex = 614;
            this.label25.Text = "Pron�stico";
            this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBox1
            // 
            this.textBox1.BackColor = System.Drawing.SystemColors.Info;
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox1.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(140, 208);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(264, 80);
            this.textBox1.TabIndex = 615;
            this.textBox1.Text = "En columna Base:\r\nClic sobre signos de la columna  base modifica el signo\r\n\r\nEn P" +
                "ronosticos:\r\nBot�n izquierdo marca un signo\r\nBot�n derecho invierte el pron�stic" +
                "o de los 3 signos";
            // 
            // txLongColumna
            // 
            this.txLongColumna.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txLongColumna.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txLongColumna.Location = new System.Drawing.Point(267, 308);
            this.txLongColumna.Name = "txLongColumna";
            this.txLongColumna.Size = new System.Drawing.Size(32, 21);
            this.txLongColumna.TabIndex = 611;
            this.txLongColumna.Text = "15";
            // 
            // label22
            // 
            this.label22.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.ForeColor = System.Drawing.Color.Maroon;
            this.label22.Location = new System.Drawing.Point(128, 308);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(133, 20);
            this.label22.TabIndex = 610;
            this.label22.Text = "N�mero de partidos";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtReduccionesContempladas
            // 
            this.txtReduccionesContempladas.BackColor = System.Drawing.Color.Bisque;
            this.txtReduccionesContempladas.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtReduccionesContempladas.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtReduccionesContempladas.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.txtReduccionesContempladas.Location = new System.Drawing.Point(123, 120);
            this.txtReduccionesContempladas.Multiline = true;
            this.txtReduccionesContempladas.Name = "txtReduccionesContempladas";
            this.txtReduccionesContempladas.Size = new System.Drawing.Size(285, 72);
            this.txtReduccionesContempladas.TabIndex = 609;
            this.txtReduccionesContempladas.Text = "  4 TRIPLES........ \treducidos al 13\r\n13 TRIPLES........ \treducidos al 13\r\n11 TRI" +
                "PLES........ \treducidos al 12\r\n  7 DOBLES ........\treducidos al 13\r\n15 DOBLES .." +
                "......\treducidos al 13";
            this.txtReduccionesContempladas.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblSeContemplan
            // 
            this.lblSeContemplan.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSeContemplan.ForeColor = System.Drawing.Color.Maroon;
            this.lblSeContemplan.Location = new System.Drawing.Point(120, 82);
            this.lblSeContemplan.Name = "lblSeContemplan";
            this.lblSeContemplan.Size = new System.Drawing.Size(288, 34);
            this.lblSeContemplan.TabIndex = 608;
            this.lblSeContemplan.Text = "Se contemplan las siguientes reducciones: ";
            this.lblSeContemplan.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.White;
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(88, 315);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(16, 16);
            this.label3.TabIndex = 607;
            this.label3.Tag = "14";
            this.label3.Text = "1";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label3.DoubleClick += new System.EventHandler(this.GenericBaseLabel_DoubleClick);
            this.label3.Click += new System.EventHandler(this.GenericBaseLabel_Click);
            // 
            // lblNewBase152
            // 
            this.lblNewBase152.BackColor = System.Drawing.Color.White;
            this.lblNewBase152.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase152.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase152.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase152.Location = new System.Drawing.Point(56, 315);
            this.lblNewBase152.Name = "lblNewBase152";
            this.lblNewBase152.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase152.TabIndex = 606;
            this.lblNewBase152.Tag = "14";
            this.lblNewBase152.Text = "2";
            this.lblNewBase152.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase152.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase15X
            // 
            this.lblNewBase15X.BackColor = System.Drawing.Color.White;
            this.lblNewBase15X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase15X.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase15X.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase15X.Location = new System.Drawing.Point(36, 315);
            this.lblNewBase15X.Name = "lblNewBase15X";
            this.lblNewBase15X.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase15X.TabIndex = 605;
            this.lblNewBase15X.Tag = "14";
            this.lblNewBase15X.Text = "X";
            this.lblNewBase15X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase15X.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblNewBase151
            // 
            this.lblNewBase151.BackColor = System.Drawing.Color.White;
            this.lblNewBase151.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase151.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase151.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase151.Location = new System.Drawing.Point(16, 315);
            this.lblNewBase151.Name = "lblNewBase151";
            this.lblNewBase151.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase151.TabIndex = 604;
            this.lblNewBase151.Tag = "14";
            this.lblNewBase151.Text = "1";
            this.lblNewBase151.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase151.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // lblBase14
            // 
            this.lblBase14.BackColor = System.Drawing.Color.White;
            this.lblBase14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBase14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBase14.ForeColor = System.Drawing.Color.Black;
            this.lblBase14.Location = new System.Drawing.Point(88, 295);
            this.lblBase14.Name = "lblBase14";
            this.lblBase14.Size = new System.Drawing.Size(16, 16);
            this.lblBase14.TabIndex = 603;
            this.lblBase14.Tag = "13";
            this.lblBase14.Text = "1";
            this.lblBase14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBase14.DoubleClick += new System.EventHandler(this.GenericBaseLabel_DoubleClick);
            this.lblBase14.Click += new System.EventHandler(this.GenericBaseLabel_Click);
            // 
            // lblBase13
            // 
            this.lblBase13.BackColor = System.Drawing.Color.White;
            this.lblBase13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBase13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBase13.ForeColor = System.Drawing.Color.Black;
            this.lblBase13.Location = new System.Drawing.Point(88, 278);
            this.lblBase13.Name = "lblBase13";
            this.lblBase13.Size = new System.Drawing.Size(16, 16);
            this.lblBase13.TabIndex = 602;
            this.lblBase13.Tag = "12";
            this.lblBase13.Text = "1";
            this.lblBase13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBase13.DoubleClick += new System.EventHandler(this.GenericBaseLabel_DoubleClick);
            this.lblBase13.Click += new System.EventHandler(this.GenericBaseLabel_Click);
            // 
            // lblBase12
            // 
            this.lblBase12.BackColor = System.Drawing.Color.White;
            this.lblBase12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBase12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBase12.ForeColor = System.Drawing.Color.Black;
            this.lblBase12.Location = new System.Drawing.Point(88, 261);
            this.lblBase12.Name = "lblBase12";
            this.lblBase12.Size = new System.Drawing.Size(16, 16);
            this.lblBase12.TabIndex = 601;
            this.lblBase12.Tag = "11";
            this.lblBase12.Text = "1";
            this.lblBase12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBase12.DoubleClick += new System.EventHandler(this.GenericBaseLabel_DoubleClick);
            this.lblBase12.Click += new System.EventHandler(this.GenericBaseLabel_Click);
            // 
            // lblBase11
            // 
            this.lblBase11.BackColor = System.Drawing.Color.White;
            this.lblBase11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBase11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBase11.ForeColor = System.Drawing.Color.Black;
            this.lblBase11.Location = new System.Drawing.Point(88, 241);
            this.lblBase11.Name = "lblBase11";
            this.lblBase11.Size = new System.Drawing.Size(16, 16);
            this.lblBase11.TabIndex = 600;
            this.lblBase11.Tag = "10";
            this.lblBase11.Text = "1";
            this.lblBase11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBase11.DoubleClick += new System.EventHandler(this.GenericBaseLabel_DoubleClick);
            this.lblBase11.Click += new System.EventHandler(this.GenericBaseLabel_Click);
            // 
            // lblBase10
            // 
            this.lblBase10.BackColor = System.Drawing.Color.White;
            this.lblBase10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBase10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBase10.ForeColor = System.Drawing.Color.Black;
            this.lblBase10.Location = new System.Drawing.Point(88, 224);
            this.lblBase10.Name = "lblBase10";
            this.lblBase10.Size = new System.Drawing.Size(16, 16);
            this.lblBase10.TabIndex = 599;
            this.lblBase10.Tag = "9";
            this.lblBase10.Text = "1";
            this.lblBase10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBase10.DoubleClick += new System.EventHandler(this.GenericBaseLabel_DoubleClick);
            this.lblBase10.Click += new System.EventHandler(this.GenericBaseLabel_Click);
            // 
            // lblBase9
            // 
            this.lblBase9.BackColor = System.Drawing.Color.White;
            this.lblBase9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBase9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBase9.ForeColor = System.Drawing.Color.Black;
            this.lblBase9.Location = new System.Drawing.Point(88, 207);
            this.lblBase9.Name = "lblBase9";
            this.lblBase9.Size = new System.Drawing.Size(16, 16);
            this.lblBase9.TabIndex = 598;
            this.lblBase9.Tag = "8";
            this.lblBase9.Text = "1";
            this.lblBase9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBase9.DoubleClick += new System.EventHandler(this.GenericBaseLabel_DoubleClick);
            this.lblBase9.Click += new System.EventHandler(this.GenericBaseLabel_Click);
            // 
            // lblBase8
            // 
            this.lblBase8.BackColor = System.Drawing.Color.White;
            this.lblBase8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBase8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBase8.ForeColor = System.Drawing.Color.Black;
            this.lblBase8.Location = new System.Drawing.Point(88, 187);
            this.lblBase8.Name = "lblBase8";
            this.lblBase8.Size = new System.Drawing.Size(16, 16);
            this.lblBase8.TabIndex = 597;
            this.lblBase8.Tag = "7";
            this.lblBase8.Text = "1";
            this.lblBase8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBase8.DoubleClick += new System.EventHandler(this.GenericBaseLabel_DoubleClick);
            this.lblBase8.Click += new System.EventHandler(this.GenericBaseLabel_Click);
            // 
            // lblBase7
            // 
            this.lblBase7.BackColor = System.Drawing.Color.White;
            this.lblBase7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBase7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBase7.ForeColor = System.Drawing.Color.Black;
            this.lblBase7.Location = new System.Drawing.Point(88, 170);
            this.lblBase7.Name = "lblBase7";
            this.lblBase7.Size = new System.Drawing.Size(16, 16);
            this.lblBase7.TabIndex = 596;
            this.lblBase7.Tag = "6";
            this.lblBase7.Text = "1";
            this.lblBase7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBase7.DoubleClick += new System.EventHandler(this.GenericBaseLabel_DoubleClick);
            this.lblBase7.Click += new System.EventHandler(this.GenericBaseLabel_Click);
            // 
            // lblBase6
            // 
            this.lblBase6.BackColor = System.Drawing.Color.White;
            this.lblBase6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBase6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBase6.ForeColor = System.Drawing.Color.Black;
            this.lblBase6.Location = new System.Drawing.Point(88, 153);
            this.lblBase6.Name = "lblBase6";
            this.lblBase6.Size = new System.Drawing.Size(16, 16);
            this.lblBase6.TabIndex = 595;
            this.lblBase6.Tag = "5";
            this.lblBase6.Text = "1";
            this.lblBase6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBase6.DoubleClick += new System.EventHandler(this.GenericBaseLabel_DoubleClick);
            this.lblBase6.Click += new System.EventHandler(this.GenericBaseLabel_Click);
            // 
            // lblBase5
            // 
            this.lblBase5.BackColor = System.Drawing.Color.White;
            this.lblBase5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBase5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBase5.ForeColor = System.Drawing.Color.Black;
            this.lblBase5.Location = new System.Drawing.Point(88, 136);
            this.lblBase5.Name = "lblBase5";
            this.lblBase5.Size = new System.Drawing.Size(16, 16);
            this.lblBase5.TabIndex = 594;
            this.lblBase5.Tag = "4";
            this.lblBase5.Text = "1";
            this.lblBase5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBase5.DoubleClick += new System.EventHandler(this.GenericBaseLabel_DoubleClick);
            this.lblBase5.Click += new System.EventHandler(this.GenericBaseLabel_Click);
            // 
            // lblBase4
            // 
            this.lblBase4.BackColor = System.Drawing.Color.White;
            this.lblBase4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBase4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBase4.ForeColor = System.Drawing.Color.Black;
            this.lblBase4.Location = new System.Drawing.Point(88, 116);
            this.lblBase4.Name = "lblBase4";
            this.lblBase4.Size = new System.Drawing.Size(16, 16);
            this.lblBase4.TabIndex = 593;
            this.lblBase4.Tag = "3";
            this.lblBase4.Text = "1";
            this.lblBase4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBase4.DoubleClick += new System.EventHandler(this.GenericBaseLabel_DoubleClick);
            this.lblBase4.Click += new System.EventHandler(this.GenericBaseLabel_Click);
            // 
            // lblBase3
            // 
            this.lblBase3.BackColor = System.Drawing.Color.White;
            this.lblBase3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBase3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBase3.ForeColor = System.Drawing.Color.Black;
            this.lblBase3.Location = new System.Drawing.Point(88, 99);
            this.lblBase3.Name = "lblBase3";
            this.lblBase3.Size = new System.Drawing.Size(16, 16);
            this.lblBase3.TabIndex = 592;
            this.lblBase3.Tag = "2";
            this.lblBase3.Text = "1";
            this.lblBase3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBase3.DoubleClick += new System.EventHandler(this.GenericBaseLabel_DoubleClick);
            this.lblBase3.Click += new System.EventHandler(this.GenericBaseLabel_Click);
            // 
            // lblBase2
            // 
            this.lblBase2.BackColor = System.Drawing.Color.White;
            this.lblBase2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBase2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBase2.ForeColor = System.Drawing.Color.Black;
            this.lblBase2.Location = new System.Drawing.Point(88, 82);
            this.lblBase2.Name = "lblBase2";
            this.lblBase2.Size = new System.Drawing.Size(16, 16);
            this.lblBase2.TabIndex = 591;
            this.lblBase2.Tag = "1";
            this.lblBase2.Text = "1";
            this.lblBase2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBase2.DoubleClick += new System.EventHandler(this.GenericBaseLabel_DoubleClick);
            this.lblBase2.Click += new System.EventHandler(this.GenericBaseLabel_Click);
            // 
            // lblBase1
            // 
            this.lblBase1.BackColor = System.Drawing.Color.White;
            this.lblBase1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBase1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBase1.ForeColor = System.Drawing.Color.Black;
            this.lblBase1.Location = new System.Drawing.Point(88, 65);
            this.lblBase1.Name = "lblBase1";
            this.lblBase1.Size = new System.Drawing.Size(16, 16);
            this.lblBase1.TabIndex = 590;
            this.lblBase1.Tag = "0";
            this.lblBase1.Text = "1";
            this.lblBase1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblBase1.DoubleClick += new System.EventHandler(this.GenericBaseLabel_DoubleClick);
            this.lblBase1.Click += new System.EventHandler(this.GenericBaseLabel_Click);
            // 
            // lblNewBase142
            // 
            this.lblNewBase142.BackColor = System.Drawing.Color.White;
            this.lblNewBase142.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase142.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase142.ForeColor = System.Drawing.Color.Pink;
            this.lblNewBase142.Location = new System.Drawing.Point(56, 295);
            this.lblNewBase142.Name = "lblNewBase142";
            this.lblNewBase142.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase142.TabIndex = 589;
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
            this.lblNewBase132.Location = new System.Drawing.Point(56, 278);
            this.lblNewBase132.Name = "lblNewBase132";
            this.lblNewBase132.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase132.TabIndex = 588;
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
            this.lblNewBase122.Location = new System.Drawing.Point(56, 261);
            this.lblNewBase122.Name = "lblNewBase122";
            this.lblNewBase122.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase122.TabIndex = 587;
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
            this.lblNewBase112.Location = new System.Drawing.Point(56, 241);
            this.lblNewBase112.Name = "lblNewBase112";
            this.lblNewBase112.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase112.TabIndex = 586;
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
            this.lblNewBase102.Location = new System.Drawing.Point(56, 224);
            this.lblNewBase102.Name = "lblNewBase102";
            this.lblNewBase102.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase102.TabIndex = 585;
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
            this.lblNewBase92.Location = new System.Drawing.Point(56, 207);
            this.lblNewBase92.Name = "lblNewBase92";
            this.lblNewBase92.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase92.TabIndex = 584;
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
            this.lblNewBase82.Location = new System.Drawing.Point(56, 187);
            this.lblNewBase82.Name = "lblNewBase82";
            this.lblNewBase82.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase82.TabIndex = 583;
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
            this.lblNewBase72.Location = new System.Drawing.Point(56, 170);
            this.lblNewBase72.Name = "lblNewBase72";
            this.lblNewBase72.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase72.TabIndex = 582;
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
            this.lblNewBase62.Location = new System.Drawing.Point(56, 153);
            this.lblNewBase62.Name = "lblNewBase62";
            this.lblNewBase62.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase62.TabIndex = 581;
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
            this.lblNewBase52.Location = new System.Drawing.Point(56, 136);
            this.lblNewBase52.Name = "lblNewBase52";
            this.lblNewBase52.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase52.TabIndex = 580;
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
            this.lblNewBase42.Location = new System.Drawing.Point(56, 116);
            this.lblNewBase42.Name = "lblNewBase42";
            this.lblNewBase42.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase42.TabIndex = 579;
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
            this.lblNewBase32.Location = new System.Drawing.Point(56, 99);
            this.lblNewBase32.Name = "lblNewBase32";
            this.lblNewBase32.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase32.TabIndex = 578;
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
            this.lblNewBase22.Location = new System.Drawing.Point(56, 82);
            this.lblNewBase22.Name = "lblNewBase22";
            this.lblNewBase22.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase22.TabIndex = 577;
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
            this.lblNewBase12.Location = new System.Drawing.Point(56, 65);
            this.lblNewBase12.Name = "lblNewBase12";
            this.lblNewBase12.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase12.TabIndex = 576;
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
            this.lblNewBase14X.Location = new System.Drawing.Point(36, 295);
            this.lblNewBase14X.Name = "lblNewBase14X";
            this.lblNewBase14X.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase14X.TabIndex = 575;
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
            this.lblNewBase13X.Location = new System.Drawing.Point(36, 278);
            this.lblNewBase13X.Name = "lblNewBase13X";
            this.lblNewBase13X.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase13X.TabIndex = 574;
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
            this.lblNewBase12X.Location = new System.Drawing.Point(36, 261);
            this.lblNewBase12X.Name = "lblNewBase12X";
            this.lblNewBase12X.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase12X.TabIndex = 573;
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
            this.lblNewBase11X.Location = new System.Drawing.Point(36, 241);
            this.lblNewBase11X.Name = "lblNewBase11X";
            this.lblNewBase11X.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase11X.TabIndex = 572;
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
            this.lblNewBase10X.Location = new System.Drawing.Point(36, 224);
            this.lblNewBase10X.Name = "lblNewBase10X";
            this.lblNewBase10X.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase10X.TabIndex = 571;
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
            this.lblNewBase9X.Location = new System.Drawing.Point(36, 207);
            this.lblNewBase9X.Name = "lblNewBase9X";
            this.lblNewBase9X.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase9X.TabIndex = 570;
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
            this.lblNewBase8X.Location = new System.Drawing.Point(36, 187);
            this.lblNewBase8X.Name = "lblNewBase8X";
            this.lblNewBase8X.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase8X.TabIndex = 569;
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
            this.lblNewBase7X.Location = new System.Drawing.Point(36, 170);
            this.lblNewBase7X.Name = "lblNewBase7X";
            this.lblNewBase7X.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase7X.TabIndex = 568;
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
            this.lblNewBase6X.Location = new System.Drawing.Point(36, 153);
            this.lblNewBase6X.Name = "lblNewBase6X";
            this.lblNewBase6X.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase6X.TabIndex = 567;
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
            this.lblNewBase5X.Location = new System.Drawing.Point(36, 136);
            this.lblNewBase5X.Name = "lblNewBase5X";
            this.lblNewBase5X.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase5X.TabIndex = 566;
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
            this.lblNewBase4X.Location = new System.Drawing.Point(36, 116);
            this.lblNewBase4X.Name = "lblNewBase4X";
            this.lblNewBase4X.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase4X.TabIndex = 565;
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
            this.lblNewBase3X.Location = new System.Drawing.Point(36, 99);
            this.lblNewBase3X.Name = "lblNewBase3X";
            this.lblNewBase3X.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase3X.TabIndex = 564;
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
            this.lblNewBase2X.Location = new System.Drawing.Point(36, 82);
            this.lblNewBase2X.Name = "lblNewBase2X";
            this.lblNewBase2X.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase2X.TabIndex = 563;
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
            this.lblNewBase1X.Location = new System.Drawing.Point(36, 65);
            this.lblNewBase1X.Name = "lblNewBase1X";
            this.lblNewBase1X.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase1X.TabIndex = 562;
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
            this.lblNewBase141.Location = new System.Drawing.Point(16, 295);
            this.lblNewBase141.Name = "lblNewBase141";
            this.lblNewBase141.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase141.TabIndex = 561;
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
            this.lblNewBase131.Location = new System.Drawing.Point(16, 278);
            this.lblNewBase131.Name = "lblNewBase131";
            this.lblNewBase131.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase131.TabIndex = 560;
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
            this.lblNewBase121.Location = new System.Drawing.Point(16, 261);
            this.lblNewBase121.Name = "lblNewBase121";
            this.lblNewBase121.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase121.TabIndex = 559;
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
            this.lblNewBase111.Location = new System.Drawing.Point(16, 241);
            this.lblNewBase111.Name = "lblNewBase111";
            this.lblNewBase111.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase111.TabIndex = 558;
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
            this.lblNewBase101.Location = new System.Drawing.Point(16, 224);
            this.lblNewBase101.Name = "lblNewBase101";
            this.lblNewBase101.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase101.TabIndex = 557;
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
            this.lblNewBase91.Location = new System.Drawing.Point(16, 207);
            this.lblNewBase91.Name = "lblNewBase91";
            this.lblNewBase91.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase91.TabIndex = 556;
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
            this.lblNewBase81.Location = new System.Drawing.Point(16, 187);
            this.lblNewBase81.Name = "lblNewBase81";
            this.lblNewBase81.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase81.TabIndex = 555;
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
            this.lblNewBase71.Location = new System.Drawing.Point(16, 170);
            this.lblNewBase71.Name = "lblNewBase71";
            this.lblNewBase71.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase71.TabIndex = 554;
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
            this.lblNewBase61.Location = new System.Drawing.Point(16, 153);
            this.lblNewBase61.Name = "lblNewBase61";
            this.lblNewBase61.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase61.TabIndex = 553;
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
            this.lblNewBase51.Location = new System.Drawing.Point(16, 136);
            this.lblNewBase51.Name = "lblNewBase51";
            this.lblNewBase51.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase51.TabIndex = 552;
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
            this.lblNewBase41.Location = new System.Drawing.Point(16, 116);
            this.lblNewBase41.Name = "lblNewBase41";
            this.lblNewBase41.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase41.TabIndex = 551;
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
            this.lblNewBase31.Location = new System.Drawing.Point(16, 99);
            this.lblNewBase31.Name = "lblNewBase31";
            this.lblNewBase31.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase31.TabIndex = 550;
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
            this.lblNewBase21.Location = new System.Drawing.Point(16, 82);
            this.lblNewBase21.Name = "lblNewBase21";
            this.lblNewBase21.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase21.TabIndex = 549;
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
            this.lblNewBase11.Location = new System.Drawing.Point(16, 65);
            this.lblNewBase11.Name = "lblNewBase11";
            this.lblNewBase11.Size = new System.Drawing.Size(16, 16);
            this.lblNewBase11.TabIndex = 548;
            this.lblNewBase11.Tag = "0";
            this.lblNewBase11.Text = "1";
            this.lblNewBase11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase11.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Generic_MouseDown);
            // 
            // btGenerar
            // 
            this.btGenerar.BackColor = System.Drawing.Color.DarkSalmon;
            this.btGenerar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btGenerar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btGenerar.ForeColor = System.Drawing.Color.Black;
            this.btGenerar.Image = ((System.Drawing.Image)(resources.GetObject("btGenerar.Image")));
            this.btGenerar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btGenerar.Location = new System.Drawing.Point(320, 336);
            this.btGenerar.Name = "btGenerar";
            this.btGenerar.Size = new System.Drawing.Size(80, 32);
            this.btGenerar.TabIndex = 547;
            this.btGenerar.Text = "Generar";
            this.btGenerar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btGenerar.UseVisualStyleBackColor = false;
            this.btGenerar.Click += new System.EventHandler(this.btGenerar_Click);
            // 
            // statusBar1
            // 
            this.statusBar1.Location = new System.Drawing.Point(0, 380);
            this.statusBar1.Name = "statusBar1";
            this.statusBar1.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.statusBarPanel1,
            this.statusBarPanel2});
            this.statusBar1.ShowPanels = true;
            this.statusBar1.Size = new System.Drawing.Size(420, 22);
            this.statusBar1.TabIndex = 618;
            this.statusBar1.Text = "statusBar1";
            // 
            // statusBarPanel1
            // 
            this.statusBarPanel1.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.statusBarPanel1.Icon = ((System.Drawing.Icon)(resources.GetObject("statusBarPanel1.Icon")));
            this.statusBarPanel1.Name = "statusBarPanel1";
            this.statusBarPanel1.Text = "El n� de triples o dobles debe coincidir con una de las reducciones contempladas";
            this.statusBarPanel1.Width = 441;
            // 
            // statusBarPanel2
            // 
            this.statusBarPanel2.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.statusBarPanel2.Icon = ((System.Drawing.Icon)(resources.GetObject("statusBarPanel2.Icon")));
            this.statusBarPanel2.Name = "statusBarPanel2";
            this.statusBarPanel2.Width = 31;
            // 
            // txNombreArchivo
            // 
            this.txNombreArchivo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txNombreArchivo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txNombreArchivo.Location = new System.Drawing.Point(156, 12);
            this.txNombreArchivo.Name = "txNombreArchivo";
            this.txNombreArchivo.ReadOnly = true;
            this.txNombreArchivo.Size = new System.Drawing.Size(228, 21);
            this.txNombreArchivo.TabIndex = 621;
            this.txNombreArchivo.Text = "(falta seleccionar fichero...)";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Maroon;
            this.label1.Location = new System.Drawing.Point(16, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 20);
            this.label1.TabIndex = 620;
            this.label1.Text = "Fichero de salida";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btSeleccionarFichero
            // 
            this.btSeleccionarFichero.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btSeleccionarFichero.BackColor = System.Drawing.Color.LightSalmon;
            this.btSeleccionarFichero.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btSeleccionarFichero.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btSeleccionarFichero.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btSeleccionarFichero.Image = ((System.Drawing.Image)(resources.GetObject("btSeleccionarFichero.Image")));
            this.btSeleccionarFichero.Location = new System.Drawing.Point(385, 12);
            this.btSeleccionarFichero.Name = "btSeleccionarFichero";
            this.btSeleccionarFichero.Size = new System.Drawing.Size(24, 21);
            this.btSeleccionarFichero.TabIndex = 622;
            this.btSeleccionarFichero.UseVisualStyleBackColor = false;
            this.btSeleccionarFichero.Click += new System.EventHandler(this.btSeleccionarFichero_Click);
            // 
            // FrmReducidasPerfectas
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(420, 402);
            this.Controls.Add(this.btSeleccionarFichero);
            this.Controls.Add(this.txNombreArchivo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.statusBar1);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label25);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.txLongColumna);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.txtReduccionesContempladas);
            this.Controls.Add(this.lblSeContemplan);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblNewBase152);
            this.Controls.Add(this.lblNewBase15X);
            this.Controls.Add(this.lblNewBase151);
            this.Controls.Add(this.lblBase14);
            this.Controls.Add(this.lblBase13);
            this.Controls.Add(this.lblBase12);
            this.Controls.Add(this.lblBase11);
            this.Controls.Add(this.lblBase10);
            this.Controls.Add(this.lblBase9);
            this.Controls.Add(this.lblBase8);
            this.Controls.Add(this.lblBase7);
            this.Controls.Add(this.lblBase6);
            this.Controls.Add(this.lblBase5);
            this.Controls.Add(this.lblBase4);
            this.Controls.Add(this.lblBase3);
            this.Controls.Add(this.lblBase2);
            this.Controls.Add(this.lblBase1);
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
            this.Controls.Add(this.btGenerar);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(428, 436);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(428, 436);
            this.Name = "FrmReducidasPerfectas";
            this.Text = "Reducciones Perfectas";
            // StatusBarPanel controls don't support ISupportInitialize in .NET 8
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).EndInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void btGenerar_Click(object sender, EventArgs e)
		{
			int NumDobles=0;
			int NumTriples=0;
			statusBarPanel2.Text ="";
			if(nombreArchivo=="")
			{
				statusBarPanel1.Text ="Es necesario indicar el archivo de salida";
				return;
			}
			if(TestSignos()==false)
			{
				statusBarPanel1.Text ="La columna base no est� incluida en el pron�stico";
				return;
			}

			ContarDoblesYtriples(ref NumDobles, ref NumTriples);

			if (NumTriples !=0 && NumDobles !=0)
			{
				statusBarPanel1.Text ="No se permite mezcla de dobles y triples";
				return;
			}
			if (NumDobles==0)
			{
				switch (NumTriples)
				{
					case 4: GeneraReduccion(M4TR13, true);break;
					case 13: GeneraReduccion(M13TR13, true);break;
					case 11: GeneraReduccion(M11TR12, true); break;
					default: statusBarPanel1.Text ="N� de triples incorrecto"; break;
				}
			}
			else
			{
				switch (NumDobles)
				{
					case 7:
				
						if (TestSignosDobles()) 
						{
							GeneraReduccion(M7DR13, false);
						}
						else
						{
							statusBarPanel1.Text ="Los signos base no forman parte del pronostico";
						}
						break;
					case 15:
						if (TestSignosDobles()) 
						{
							GeneraReduccion(M15DR13, false);
						}
						else
						{
							statusBarPanel1.Text ="Los signos base no forman parte del pronostico";
						}
						break;
			
					default:statusBarPanel1.Text ="N� de dobles incorrecto"; break;
				}
			}

		}
		private bool TestSignosDobles()
		{
			for (int i=0; i<15; i++)
			{
				if (Involucrados [i] && Pronosticos [i,SignosBase [i]]==false)
				{
					return false;
				}
			}
			return true;
		}

		private void GeneraReduccion(byte [,] Matriz, bool EsDeTriples)
		{
			int SignosAObtener = Matriz.GetUpperBound (0)+1;
			int NumTriples= Matriz.GetUpperBound(1)+1 + SignosAObtener;
			int i, z, j;
			int modulo=2;
			if (EsDeTriples) modulo =3;

		    string [] s = new string [] {"1","X","2"};
			int MaxLen = NumTriples;
			int ncol =0;

			try
			{MaxLen=Convert.ToInt32 (txLongColumna.Text);}
			catch
			{statusBarPanel1.Text ="Longitud incorrecta"; return;}

			if (NumTriples>MaxLen) {statusBarPanel1.Text ="La longitud de la columna es incorrecta"; return;}

			Cursor.Current = Cursors.WaitCursor;
			
			double MaxCol=  Math.Pow (3, NumTriples - SignosAObtener);

		    StreamWriter sw = File.CreateText( nombreArchivo );	
			
   
			for (i = 0 ;i<MaxCol;i++) // generamos las columnas 
			{
				int[] Signos = new Int32[MaxLen];
				bool Descartar = false;
				string columna = "";
				int[] suma = new int[5];
				//hacemos como si los triples estuvieran en los primeros partidos
				for (z = 0;z< (NumTriples - SignosAObtener); z++) // generamos los signos independientes
				{
					Signos[z] = Convert.ToInt32 (i / Math.Pow (3, z)) % 3;
					//los dobles los tratamos inicialmente a 1X descartamos el 2
					if (EsDeTriples==false && Signos[z]==2) {Descartar = true;break;}
					// calculamos los signos dependientes
					for (j=0; j<SignosAObtener;j++){suma[j] += Matriz[j,z]*Signos[z];}
				}
				if (Descartar == false)
				{
					for(j=0;j<SignosAObtener;j++) {Signos[NumTriples - SignosAObtener +j]=suma[j] % modulo ;}
					//lo trasladamos a los partidos involucrados
					int PartidoVirtual=0; 
					for (int Partido=0; Partido<15; Partido++)
					{
						if (Involucrados [Partido])
						{
							
							if (EsDeTriples)
							{
								columna=columna + s[(SignosBase [Partido] + Signos[PartidoVirtual++]) % 3];
							}
							else
							{
								//tratamiento de los signos dobles
								int[] SignosDoble = new int [2];
								SignosDoble[0] = SignosBase [Partido];
								SignosDoble[1] = (SignosBase [Partido]+1) % 3;
								if (Pronosticos [Partido, SignosDoble[1]] == false) SignosDoble[1] = (SignosDoble[1] +1) %3;
								columna=columna + s[SignosDoble[Signos[PartidoVirtual++]]];
							}
						}
						else
						{
							columna=columna + s[SignosBase [Partido]];
						}
						
					}
					columna = columna.Substring (0,MaxLen);
					sw.WriteLine(columna );
					ncol++;
				}
			}
			sw.Close ();
			Cursor.Current = Cursors.Default ;
			statusBarPanel1.Text ="Se han grabado " + ncol + " columnas";
			statusBarPanel2.Text ="Fichero de salida: " + txNombreArchivo.Text;
			statusBarPanel2.ToolTipText = nombreArchivo;
		}

	    private void GenericLabel_Click(object sender, EventArgs e)
		{
			Label MiLabel =(Label)sender;
			int Partido = Convert.ToInt16 (MiLabel.Tag );
			int Signo = "1X2".IndexOf (MiLabel.Text );
			Pronosticos[Partido,Signo]=PonerColorEnLabel (MiLabel);
		}
		private void GenericBaseLabel_Click(object sender, EventArgs e)
		{
			Label MiLabel =(Label)sender;
			switch (MiLabel.Text)
			{
				case "1" :
				{
					MiLabel.Text="X";
					SignosBase [Convert.ToInt16 (MiLabel.Tag )]=1;
					break;
				}
				case "X" :
				{
					MiLabel.Text="2";
					SignosBase [Convert.ToInt16 (MiLabel.Tag )]=2;
					break;
				}
				case "2":
				{
					MiLabel.Text="1";
					SignosBase [Convert.ToInt16 (MiLabel.Tag )]=0;
					break;
				}
			}
		}

	    private void GenericBaseLabel_DoubleClick(object sender, System.EventArgs e)
		{
			GenericBaseLabel_Click(sender,e);
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
		private void ContarDoblesYtriples(ref int NumDobles, ref int NumTriples)
		{
			NumDobles=0;
			NumTriples=0;
		    for (int i=0; i<15; i++)
		    {
		        int Suma = Convert.ToInt16 (Pronosticos [i,0])+Convert.ToInt16 (Pronosticos[i,1])+Convert.ToInt16 (Pronosticos[i,2]);
		        switch (Suma)
				{
					case 3: NumTriples++; Involucrados[i]=true; break;
					case 2: NumDobles++; Involucrados[i]=true; break;
					default: Involucrados[i]=false;break;
				}
		    }
		}

		private void Generic_MouseDown(object sender, MouseEventArgs e)
		{

			if (e.Button == MouseButtons.Right) 
			{ 
				Label MiLabel =(Label)sender;

				foreach (object Objeto in Controls )
				{
					try 
					{
						Label L = (Label) Objeto;
						if(L.Tag == MiLabel.Tag)
						{
							if(L.Name.IndexOf ("lblNewBase") !=-1)
							{
								GenericLabel_Click(L,e);
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
				GenericLabel_Click(sender,e);
			}
		}

		private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			System.Diagnostics.Process.Start("IExplore"," http://www.foro1x2.com/viewtopic.php?t=4445");
		}

		private void btSeleccionarFichero_Click(object sender, EventArgs e)
		{
			SaveFileDialog abreFiltroDialog = new SaveFileDialog();
			abreFiltroDialog.InitialDirectory = "Columnas\\" ;
			abreFiltroDialog.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			if(abreFiltroDialog.ShowDialog () == DialogResult.OK)
			{		    	
				nombreArchivo = abreFiltroDialog.FileName;		    	
				txNombreArchivo.Text = Path.GetFileName(nombreArchivo);
			}
		}
		private bool TestSignos()
		{
			bool ret=true;
			short num=Convert.ToInt16 (txLongColumna.Text);
			short i;
			for(i=0 ;i<num; i++)
			{
				if (Pronosticos [i,SignosBase [i]]==false) 
				{
					ret = false;
					break;
				}
			}
			return ret;
		}
	}
}
