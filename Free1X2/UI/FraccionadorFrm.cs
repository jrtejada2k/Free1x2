// Free1X2 : Programa de quinielas "libre"
// Copyright (C) 2004 xfsf
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
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace Free1X2.UI
{
	public class FraccionadorFrm : System.Windows.Forms.Form {
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.RadioButton radioButton2;
		private System.Windows.Forms.TextBox tbcol15;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox gbcols;
		private System.Windows.Forms.TextBox tbcol03;
		private System.Windows.Forms.TextBox tbcol02;
		private System.Windows.Forms.TextBox tbcol01;
		private System.Windows.Forms.TextBox tbcol07;
		private System.Windows.Forms.TextBox tbcol06;
		private System.Windows.Forms.TextBox tbcol05;
		private System.Windows.Forms.TextBox tbcol04;
		private System.Windows.Forms.RadioButton rbcols;
		private System.Windows.Forms.TextBox tbcol09;
		private System.Windows.Forms.TextBox tbcol12;
		private System.Windows.Forms.TextBox tbcol13;
		private System.Windows.Forms.TextBox tbcol10;
		private System.Windows.Forms.TextBox tbcol11;
		private System.Windows.Forms.TextBox tbcol16;
		private System.Windows.Forms.TextBox tbcol17;
		private System.Windows.Forms.TextBox tbcol14;
		private System.Windows.Forms.TextBox tbqnts;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox tbcol18;
		private System.Windows.Forms.TextBox tbcol19;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label lcols;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.TextBox tbcol20;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.TextBox tbcol08;
		private System.Windows.Forms.Label lfiles;
		private System.Windows.Forms.Button bFraccionar;
		private System.Windows.Forms.Label ltime;
		private System.Windows.Forms.Button bEntrada;
		private System.Windows.Forms.GroupBox gbtrams;
		private System.Windows.Forms.Label label3;
		public FraccionadorFrm() {
			InitializeComponent();
			elmeu = new Timer();
			elmeu.Interval = 3000;
   		    elmeu.Tick += new EventHandler(elmeuTimer);
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}
		
		StreamReader sr = null;
		StreamWriter sw = null;
		string filein, fileout;
		string[] columnas = new string[4782969];
		int numcols, numfiles;
		private DateTime time0, time9;
		private Timer elmeu;
		
		private void Cambio() {
			if (rbcols.Checked) {
				gbcols.Enabled = true;
				gbtrams.Enabled = false;
			}
			else {
				gbcols.Enabled = false;
				gbtrams.Enabled = true;
			}
		}
		private void Entrada() {
			bEntrada.Enabled = false;
			bFraccionar.Enabled = false;
			OpenFileDialog lee = new OpenFileDialog();
			lee.InitialDirectory = ".\\" ;
			lee.Filter = "ColumnasEntrada(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
			if(lee.ShowDialog() == DialogResult.OK) {
	 			elmeu.Start(); time0 = DateTime.Now;
				filein = Path.GetFileName(lee.FileName);
				sr = new StreamReader(lee.FileName);
				numcols=0;
				while (sr.Peek()>0) {
					Application.DoEvents();
					columnas[numcols] = sr.ReadLine();
					numcols++;
				}
				sr.Close();
				elmeu.Stop(); veureelmeu();
			}
			bFraccionar.Enabled = true;
			bEntrada.Enabled = true;
			
		}
		private void Fraccionar() {
			bFraccionar.Enabled = false;
			bEntrada.Enabled = false;
			SaveFileDialog resul = new SaveFileDialog();
			resul.InitialDirectory = ".\\" ;
			resul.Filter = "Nombre BASE salida(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
			if(resul.ShowDialog() == DialogResult.OK) {
	 			elmeu.Start(); time0 = DateTime.Now;
				fileout = Path.GetFileNameWithoutExtension(resul.FileName);
				if (rbcols.Checked) FracCols(); else FracTrams();
			}
			elmeu.Stop(); veureelmeu();
			bEntrada.Enabled = true;
			bFraccionar.Enabled = true;
		}
		private void FracTrams() {
			int qnts, part, acum, lim; string sfile;
			qnts = Convert.ToInt32(tbqnts.Text);
			part = numcols/qnts;
			acum = 0; lim = part;
			for (numfiles=0; numfiles<qnts; numfiles++) {
				Application.DoEvents();
				sfile = String.Format(fileout+"{0:d2}.txt",(numfiles+1));
				sw = new StreamWriter(sfile);
				for (int nr=acum; nr<lim; nr++)
					sw.WriteLine(columnas[nr]);
				acum+=part; 
				lim=acum+part;
				if (numcols-lim<part) lim=numcols;
				lfiles.Text=""+(numfiles+1);
				sw.Close();
			}
		}
		private void FracCols() {
			int qnts, acum, lim; string sfile;
			acum = 0; qnts = 0;
			for (numfiles=0; numfiles<20; numfiles++) {
				Application.DoEvents();
				sfile = String.Format(fileout+"{0:d2}.txt",(numfiles+1));
				switch (numfiles) {
					case 0: qnts=Convert.ToInt32(tbcol01.Text); break;
					case 1: qnts=Convert.ToInt32(tbcol02.Text); break;
					case 2: qnts=Convert.ToInt32(tbcol03.Text); break;
					case 3: qnts=Convert.ToInt32(tbcol04.Text); break;
					case 4: qnts=Convert.ToInt32(tbcol05.Text); break;
					case 5: qnts=Convert.ToInt32(tbcol06.Text); break;
					case 6: qnts=Convert.ToInt32(tbcol07.Text); break;
					case 7: qnts=Convert.ToInt32(tbcol08.Text); break;
					case 8: qnts=Convert.ToInt32(tbcol09.Text); break;
					case 9: qnts=Convert.ToInt32(tbcol10.Text); break;
					case 10: qnts=Convert.ToInt32(tbcol11.Text); break;
					case 11: qnts=Convert.ToInt32(tbcol12.Text); break;
					case 12: qnts=Convert.ToInt32(tbcol13.Text); break;
					case 13: qnts=Convert.ToInt32(tbcol14.Text); break;
					case 14: qnts=Convert.ToInt32(tbcol15.Text); break;
					case 15: qnts=Convert.ToInt32(tbcol16.Text); break;
					case 16: qnts=Convert.ToInt32(tbcol17.Text); break;
					case 17: qnts=Convert.ToInt32(tbcol18.Text); break;
					case 18: qnts=Convert.ToInt32(tbcol19.Text); break;
					case 19: qnts=Convert.ToInt32(tbcol20.Text); break;
				}
				if (qnts==0) break;
				sw = new StreamWriter(sfile);
				lim=acum+qnts; if (lim>numcols) lim=numcols;
				for (int nr=acum; nr<lim; nr++) sw.WriteLine(columnas[nr]);
				acum+=qnts; 
				lfiles.Text=""+(numfiles+1);
				sw.Close();
			}
		}
		private void veureelmeu() {
			lcols.Text = ""+numcols;
			lfiles.Text = ""+numfiles;
			time9 = DateTime.Now;
			string temp = (time9-time0).ToString()+"0000000000";
			ltime.Text = temp.Substring(0,10);
		}
				
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FraccionadorFrm));
            this.label3 = new System.Windows.Forms.Label();
            this.gbtrams = new System.Windows.Forms.GroupBox();
            this.label21 = new System.Windows.Forms.Label();
            this.tbqnts = new System.Windows.Forms.TextBox();
            this.bEntrada = new System.Windows.Forms.Button();
            this.ltime = new System.Windows.Forms.Label();
            this.bFraccionar = new System.Windows.Forms.Button();
            this.lfiles = new System.Windows.Forms.Label();
            this.tbcol08 = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.tbcol20 = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.lcols = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.tbcol19 = new System.Windows.Forms.TextBox();
            this.tbcol18 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tbcol14 = new System.Windows.Forms.TextBox();
            this.tbcol17 = new System.Windows.Forms.TextBox();
            this.tbcol16 = new System.Windows.Forms.TextBox();
            this.tbcol11 = new System.Windows.Forms.TextBox();
            this.tbcol10 = new System.Windows.Forms.TextBox();
            this.tbcol13 = new System.Windows.Forms.TextBox();
            this.tbcol12 = new System.Windows.Forms.TextBox();
            this.tbcol09 = new System.Windows.Forms.TextBox();
            this.rbcols = new System.Windows.Forms.RadioButton();
            this.tbcol04 = new System.Windows.Forms.TextBox();
            this.tbcol05 = new System.Windows.Forms.TextBox();
            this.tbcol06 = new System.Windows.Forms.TextBox();
            this.tbcol07 = new System.Windows.Forms.TextBox();
            this.tbcol01 = new System.Windows.Forms.TextBox();
            this.tbcol02 = new System.Windows.Forms.TextBox();
            this.tbcol03 = new System.Windows.Forms.TextBox();
            this.gbcols = new System.Windows.Forms.GroupBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbcol15 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.gbtrams.SuspendLayout();
            this.gbcols.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(16, 354);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 21);
            this.label3.TabIndex = 23;
            this.label3.Text = "16";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // gbtrams
            // 
            this.gbtrams.Controls.Add(this.label21);
            this.gbtrams.Controls.Add(this.tbqnts);
            this.gbtrams.Enabled = false;
            this.gbtrams.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbtrams.ForeColor = System.Drawing.Color.Maroon;
            this.gbtrams.Location = new System.Drawing.Point(32, 240);
            this.gbtrams.Name = "gbtrams";
            this.gbtrams.Size = new System.Drawing.Size(96, 88);
            this.gbtrams.TabIndex = 6;
            this.gbtrams.TabStop = false;
            this.gbtrams.Text = "Por tramos";
            // 
            // label21
            // 
            this.label21.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.ForeColor = System.Drawing.Color.Black;
            this.label21.Location = new System.Drawing.Point(16, 24);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(64, 21);
            this.label21.TabIndex = 40;
            this.label21.Text = "¿cuántos?";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbqnts
            // 
            this.tbqnts.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbqnts.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbqnts.Location = new System.Drawing.Point(24, 48);
            this.tbqnts.Name = "tbqnts";
            this.tbqnts.Size = new System.Drawing.Size(42, 21);
            this.tbqnts.TabIndex = 39;
            this.tbqnts.Text = "0";
            this.tbqnts.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bEntrada
            // 
            this.bEntrada.BackColor = System.Drawing.Color.DarkSalmon;
            this.bEntrada.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bEntrada.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bEntrada.Image = ((System.Drawing.Image)(resources.GetObject("bEntrada.Image")));
            this.bEntrada.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bEntrada.Location = new System.Drawing.Point(16, 16);
            this.bEntrada.Name = "bEntrada";
            this.bEntrada.Size = new System.Drawing.Size(128, 32);
            this.bEntrada.TabIndex = 42;
            this.bEntrada.Text = "Entrada";
            this.bEntrada.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bEntrada.UseVisualStyleBackColor = false;
            this.bEntrada.Click += new System.EventHandler(this.BEntradaClick);
            // 
            // ltime
            // 
            this.ltime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ltime.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ltime.Location = new System.Drawing.Point(40, 448);
            this.ltime.Name = "ltime";
            this.ltime.Size = new System.Drawing.Size(80, 21);
            this.ltime.TabIndex = 40;
            this.ltime.Text = "00:00:00.0";
            this.ltime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bFraccionar
            // 
            this.bFraccionar.BackColor = System.Drawing.Color.DarkSalmon;
            this.bFraccionar.Enabled = false;
            this.bFraccionar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bFraccionar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bFraccionar.Image = ((System.Drawing.Image)(resources.GetObject("bFraccionar.Image")));
            this.bFraccionar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bFraccionar.Location = new System.Drawing.Point(16, 368);
            this.bFraccionar.Name = "bFraccionar";
            this.bFraccionar.Size = new System.Drawing.Size(128, 32);
            this.bFraccionar.TabIndex = 2;
            this.bFraccionar.Text = "Fraccionar";
            this.bFraccionar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bFraccionar.UseVisualStyleBackColor = false;
            this.bFraccionar.Click += new System.EventHandler(this.BFraccionarClick);
            // 
            // lfiles
            // 
            this.lfiles.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lfiles.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lfiles.Location = new System.Drawing.Point(40, 424);
            this.lfiles.Name = "lfiles";
            this.lfiles.Size = new System.Drawing.Size(80, 21);
            this.lfiles.TabIndex = 39;
            this.lfiles.Text = "0";
            this.lfiles.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbcol08
            // 
            this.tbcol08.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol08.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol08.Location = new System.Drawing.Point(48, 178);
            this.tbcol08.Name = "tbcol08";
            this.tbcol08.Size = new System.Drawing.Size(76, 21);
            this.tbcol08.TabIndex = 8;
            this.tbcol08.Text = "0";
            this.tbcol08.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label19
            // 
            this.label19.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(16, 420);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(32, 21);
            this.label19.TabIndex = 42;
            this.label19.Text = "19";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbcol20
            // 
            this.tbcol20.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol20.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol20.Location = new System.Drawing.Point(48, 442);
            this.tbcol20.Name = "tbcol20";
            this.tbcol20.Size = new System.Drawing.Size(76, 21);
            this.tbcol20.TabIndex = 20;
            this.tbcol20.Text = "0";
            this.tbcol20.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label20
            // 
            this.label20.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(16, 442);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(32, 21);
            this.label20.TabIndex = 39;
            this.label20.Text = "20";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label18
            // 
            this.label18.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(16, 134);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(32, 21);
            this.label18.TabIndex = 33;
            this.label18.Text = "6";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lcols
            // 
            this.lcols.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lcols.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lcols.Location = new System.Drawing.Point(40, 72);
            this.lcols.Name = "lcols";
            this.lcols.Size = new System.Drawing.Size(80, 21);
            this.lcols.TabIndex = 41;
            this.lcols.Text = "0";
            this.lcols.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label15
            // 
            this.label15.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(16, 68);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(32, 21);
            this.label15.TabIndex = 36;
            this.label15.Text = "3";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label14
            // 
            this.label14.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(16, 46);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(32, 21);
            this.label14.TabIndex = 37;
            this.label14.Text = "2";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label17
            // 
            this.label17.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(16, 112);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(32, 21);
            this.label17.TabIndex = 34;
            this.label17.Text = "5";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label16
            // 
            this.label16.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(16, 90);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(32, 21);
            this.label16.TabIndex = 35;
            this.label16.Text = "4";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            this.label11.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(16, 244);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(32, 21);
            this.label11.TabIndex = 28;
            this.label11.Text = "11";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(16, 222);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(32, 21);
            this.label10.TabIndex = 29;
            this.label10.Text = "10";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label13
            // 
            this.label13.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(16, 24);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(32, 21);
            this.label13.TabIndex = 38;
            this.label13.Text = "1";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label12
            // 
            this.label12.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(16, 266);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(32, 21);
            this.label12.TabIndex = 27;
            this.label12.Text = "12";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbcol19
            // 
            this.tbcol19.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol19.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol19.Location = new System.Drawing.Point(48, 420);
            this.tbcol19.Name = "tbcol19";
            this.tbcol19.Size = new System.Drawing.Size(76, 21);
            this.tbcol19.TabIndex = 19;
            this.tbcol19.Text = "0";
            this.tbcol19.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbcol18
            // 
            this.tbcol18.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol18.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol18.Location = new System.Drawing.Point(48, 398);
            this.tbcol18.Name = "tbcol18";
            this.tbcol18.Size = new System.Drawing.Size(76, 21);
            this.tbcol18.TabIndex = 18;
            this.tbcol18.Text = "0";
            this.tbcol18.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(16, 200);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(32, 21);
            this.label9.TabIndex = 30;
            this.label9.Text = "9";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbcol14
            // 
            this.tbcol14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol14.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol14.Location = new System.Drawing.Point(48, 310);
            this.tbcol14.Name = "tbcol14";
            this.tbcol14.Size = new System.Drawing.Size(76, 21);
            this.tbcol14.TabIndex = 14;
            this.tbcol14.Text = "0";
            this.tbcol14.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbcol17
            // 
            this.tbcol17.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol17.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol17.Location = new System.Drawing.Point(48, 376);
            this.tbcol17.Name = "tbcol17";
            this.tbcol17.Size = new System.Drawing.Size(76, 21);
            this.tbcol17.TabIndex = 17;
            this.tbcol17.Text = "0";
            this.tbcol17.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbcol16
            // 
            this.tbcol16.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol16.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol16.Location = new System.Drawing.Point(48, 354);
            this.tbcol16.Name = "tbcol16";
            this.tbcol16.Size = new System.Drawing.Size(76, 21);
            this.tbcol16.TabIndex = 16;
            this.tbcol16.Text = "0";
            this.tbcol16.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbcol11
            // 
            this.tbcol11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol11.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol11.Location = new System.Drawing.Point(48, 244);
            this.tbcol11.Name = "tbcol11";
            this.tbcol11.Size = new System.Drawing.Size(76, 21);
            this.tbcol11.TabIndex = 11;
            this.tbcol11.Text = "0";
            this.tbcol11.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbcol10
            // 
            this.tbcol10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol10.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol10.Location = new System.Drawing.Point(48, 222);
            this.tbcol10.Name = "tbcol10";
            this.tbcol10.Size = new System.Drawing.Size(76, 21);
            this.tbcol10.TabIndex = 10;
            this.tbcol10.Text = "0";
            this.tbcol10.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbcol13
            // 
            this.tbcol13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol13.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol13.Location = new System.Drawing.Point(48, 288);
            this.tbcol13.Name = "tbcol13";
            this.tbcol13.Size = new System.Drawing.Size(76, 21);
            this.tbcol13.TabIndex = 13;
            this.tbcol13.Text = "0";
            this.tbcol13.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbcol12
            // 
            this.tbcol12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol12.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol12.Location = new System.Drawing.Point(48, 266);
            this.tbcol12.Name = "tbcol12";
            this.tbcol12.Size = new System.Drawing.Size(76, 21);
            this.tbcol12.TabIndex = 12;
            this.tbcol12.Text = "0";
            this.tbcol12.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbcol09
            // 
            this.tbcol09.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol09.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol09.Location = new System.Drawing.Point(48, 200);
            this.tbcol09.Name = "tbcol09";
            this.tbcol09.Size = new System.Drawing.Size(76, 21);
            this.tbcol09.TabIndex = 9;
            this.tbcol09.Text = "0";
            this.tbcol09.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // rbcols
            // 
            this.rbcols.Checked = true;
            this.rbcols.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbcols.ForeColor = System.Drawing.Color.Black;
            this.rbcols.Location = new System.Drawing.Point(16, 24);
            this.rbcols.Name = "rbcols";
            this.rbcols.Size = new System.Drawing.Size(88, 24);
            this.rbcols.TabIndex = 3;
            this.rbcols.TabStop = true;
            this.rbcols.Text = "columnas";
            this.rbcols.CheckedChanged += new System.EventHandler(this.RbcolsCheckedChanged);
            // 
            // tbcol04
            // 
            this.tbcol04.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol04.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol04.Location = new System.Drawing.Point(48, 90);
            this.tbcol04.Name = "tbcol04";
            this.tbcol04.Size = new System.Drawing.Size(76, 21);
            this.tbcol04.TabIndex = 4;
            this.tbcol04.Text = "0";
            this.tbcol04.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbcol05
            // 
            this.tbcol05.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol05.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol05.Location = new System.Drawing.Point(48, 112);
            this.tbcol05.Name = "tbcol05";
            this.tbcol05.Size = new System.Drawing.Size(76, 21);
            this.tbcol05.TabIndex = 5;
            this.tbcol05.Text = "0";
            this.tbcol05.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbcol06
            // 
            this.tbcol06.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol06.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol06.Location = new System.Drawing.Point(48, 134);
            this.tbcol06.Name = "tbcol06";
            this.tbcol06.Size = new System.Drawing.Size(76, 21);
            this.tbcol06.TabIndex = 6;
            this.tbcol06.Text = "0";
            this.tbcol06.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbcol07
            // 
            this.tbcol07.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol07.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol07.Location = new System.Drawing.Point(48, 156);
            this.tbcol07.Name = "tbcol07";
            this.tbcol07.Size = new System.Drawing.Size(76, 21);
            this.tbcol07.TabIndex = 7;
            this.tbcol07.Text = "0";
            this.tbcol07.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbcol01
            // 
            this.tbcol01.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol01.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol01.Location = new System.Drawing.Point(48, 24);
            this.tbcol01.Name = "tbcol01";
            this.tbcol01.Size = new System.Drawing.Size(76, 21);
            this.tbcol01.TabIndex = 1;
            this.tbcol01.Text = "0";
            this.tbcol01.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbcol02
            // 
            this.tbcol02.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol02.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol02.Location = new System.Drawing.Point(48, 46);
            this.tbcol02.Name = "tbcol02";
            this.tbcol02.Size = new System.Drawing.Size(76, 21);
            this.tbcol02.TabIndex = 2;
            this.tbcol02.Text = "0";
            this.tbcol02.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbcol03
            // 
            this.tbcol03.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol03.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol03.Location = new System.Drawing.Point(48, 68);
            this.tbcol03.Name = "tbcol03";
            this.tbcol03.Size = new System.Drawing.Size(76, 21);
            this.tbcol03.TabIndex = 3;
            this.tbcol03.Text = "0";
            this.tbcol03.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // gbcols
            // 
            this.gbcols.Controls.Add(this.label19);
            this.gbcols.Controls.Add(this.tbcol19);
            this.gbcols.Controls.Add(this.tbcol20);
            this.gbcols.Controls.Add(this.label20);
            this.gbcols.Controls.Add(this.label13);
            this.gbcols.Controls.Add(this.label14);
            this.gbcols.Controls.Add(this.label15);
            this.gbcols.Controls.Add(this.label16);
            this.gbcols.Controls.Add(this.label17);
            this.gbcols.Controls.Add(this.label18);
            this.gbcols.Controls.Add(this.label7);
            this.gbcols.Controls.Add(this.label8);
            this.gbcols.Controls.Add(this.label9);
            this.gbcols.Controls.Add(this.label10);
            this.gbcols.Controls.Add(this.label11);
            this.gbcols.Controls.Add(this.label12);
            this.gbcols.Controls.Add(this.label4);
            this.gbcols.Controls.Add(this.label5);
            this.gbcols.Controls.Add(this.label6);
            this.gbcols.Controls.Add(this.label3);
            this.gbcols.Controls.Add(this.label2);
            this.gbcols.Controls.Add(this.tbcol15);
            this.gbcols.Controls.Add(this.tbcol16);
            this.gbcols.Controls.Add(this.tbcol17);
            this.gbcols.Controls.Add(this.tbcol18);
            this.gbcols.Controls.Add(this.tbcol14);
            this.gbcols.Controls.Add(this.tbcol11);
            this.gbcols.Controls.Add(this.tbcol12);
            this.gbcols.Controls.Add(this.tbcol13);
            this.gbcols.Controls.Add(this.tbcol07);
            this.gbcols.Controls.Add(this.tbcol08);
            this.gbcols.Controls.Add(this.tbcol09);
            this.gbcols.Controls.Add(this.tbcol10);
            this.gbcols.Controls.Add(this.tbcol06);
            this.gbcols.Controls.Add(this.tbcol02);
            this.gbcols.Controls.Add(this.tbcol03);
            this.gbcols.Controls.Add(this.tbcol04);
            this.gbcols.Controls.Add(this.tbcol05);
            this.gbcols.Controls.Add(this.tbcol01);
            this.gbcols.Controls.Add(this.label1);
            this.gbcols.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbcols.ForeColor = System.Drawing.Color.Maroon;
            this.gbcols.Location = new System.Drawing.Point(160, 8);
            this.gbcols.Name = "gbcols";
            this.gbcols.Size = new System.Drawing.Size(144, 480);
            this.gbcols.TabIndex = 5;
            this.gbcols.TabStop = false;
            this.gbcols.Text = "Por columnas";
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(16, 156);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 21);
            this.label7.TabIndex = 32;
            this.label7.Text = "7";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(16, 178);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(32, 21);
            this.label8.TabIndex = 31;
            this.label8.Text = "8";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(16, 288);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 21);
            this.label4.TabIndex = 26;
            this.label4.Text = "13";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(16, 310);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 21);
            this.label5.TabIndex = 25;
            this.label5.Text = "14";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(16, 332);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 21);
            this.label6.TabIndex = 24;
            this.label6.Text = "15";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(16, 376);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 21);
            this.label2.TabIndex = 22;
            this.label2.Text = "17";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbcol15
            // 
            this.tbcol15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol15.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol15.Location = new System.Drawing.Point(48, 332);
            this.tbcol15.Name = "tbcol15";
            this.tbcol15.Size = new System.Drawing.Size(76, 21);
            this.tbcol15.TabIndex = 15;
            this.tbcol15.Text = "0";
            this.tbcol15.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 398);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "18";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButton2);
            this.groupBox1.Controls.Add(this.rbcols);
            this.groupBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox1.Location = new System.Drawing.Point(16, 144);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(128, 80);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Fraccionar por";
            // 
            // radioButton2
            // 
            this.radioButton2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioButton2.ForeColor = System.Drawing.Color.Black;
            this.radioButton2.Location = new System.Drawing.Point(16, 48);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(88, 24);
            this.radioButton2.TabIndex = 4;
            this.radioButton2.Text = "tramos";
            // 
            // FraccionadorFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(320, 534);
            this.Controls.Add(this.bEntrada);
            this.Controls.Add(this.lcols);
            this.Controls.Add(this.ltime);
            this.Controls.Add(this.lfiles);
            this.Controls.Add(this.gbtrams);
            this.Controls.Add(this.gbcols);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.bFraccionar);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FraccionadorFrm";
            this.Text = "Fraccionador";
            this.gbtrams.ResumeLayout(false);
            this.gbtrams.PerformLayout();
            this.gbcols.ResumeLayout(false);
            this.gbcols.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion
		
		void RbcolsCheckedChanged(object sender, System.EventArgs e) { Cambio(); }
		void BFraccionarClick(object sender, System.EventArgs e) { Fraccionar(); }
		void elmeuTimer(object sender, System.EventArgs e) { veureelmeu(); }
		void BEntradaClick(object sender, System.EventArgs e) { Entrada(); }
	}
}
