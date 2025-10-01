// project created on 25/04/2004 at 7:41
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
using System.Windows.Forms;

using Free1X2.MotorCalculo.Estadisticas;
using Free1X2.EntradaSalida;

namespace Free1X2.UI.Estadisticas 
{
	class Anastatics : Form
	{
		private RadioButton rdibrep;
		private RadioButton rOtras;
		private Button bOrigen;
		private GroupBox gbConds;
		private RadioButton rdib;
		private RadioButton rinter;
		private Button bCalcular;
		private Button bMostrar;
		private Label lColOrg;
		private Label lFileIn;
		private RadioButton rsigseg;

		private int numcol;
		private string[] columnas = new string[500000];
		private Dibujos dibs;
		private DibRepes dibrep;
		private StaInter inter;
		private StaSigSeg sigseg;
		private int[,] rsl = new int[15,15];

		private void Mostrar() {
			bMostrar.Enabled = false;
			bCalcular.Enabled = false;
			if (rdib.Checked) {
				DibForm dib = new DibForm(rsl,numcol);
				dib.Show();
			}
			else if (rdibrep.Checked) {
				DibRepFrm dibrep = new DibRepFrm(rsl, numcol);
				dibrep.Show();
			}
			else if (rinter.Checked) {
				StaInterFrm interrup = new StaInterFrm(rsl, numcol);
				interrup.Show();			
			}
			else if (rsigseg.Checked) {
				StaSSForm ss = new StaSSForm(rsl, numcol);
				ss.Show();			
			}
			bCalcular.Enabled = true;
		}
		private void Proceso() {
			int[] ind;
			gbConds.Enabled = false;
			bMostrar.Enabled = false;
			for (int nr1=0; nr1<15; nr1++)
				for (int nr2=0; nr2<15; nr2++) rsl[nr1, nr2]=0;
			if (rdib.Checked) {
				dibs = new Dibujos();
				for (int nr=0; nr<numcol; nr++) {
					ind=dibs.Procesar(columnas[nr]);
					rsl[ind[3], ind[4]]++;
				}
			}
			else if (rdibrep.Checked) {
				int numx, num2;
				int num1 = numx = num2 = 0;
				dibrep = new DibRepes();
				for (int nr=0; nr<numcol; nr++) {
					ind=dibrep.Procesar(columnas[nr]);
					rsl[0,ind[0]]++; 
					if (num1==ind[2]) rsl[1,ind[2]]++;
					if (numx==ind[3]) rsl[2,ind[3]]++;
					if (num2==ind[4]) rsl[3,ind[4]]++;
					if ((numx+num2)==(ind[3]+ind[4])) rsl[4,(numx+num2)]++;
					num1=ind[2]; numx=ind[3]; num2=ind[4];
				}
			}
			else if (rinter.Checked) {
				inter = new StaInter();
				for (int nr=0; nr<numcol; nr++) {
					ind=inter.Procesar(columnas[nr]);
					rsl[0,ind[0]]++; 
					rsl[1,ind[2]]++; 
					rsl[2,ind[3]]++; 
					rsl[3,ind[4]]++; 
					rsl[4,ind[1]]++;
				}
			}
			else if (rsigseg.Checked) {
				sigseg = new StaSigSeg();
				for (int nr=0; nr<numcol; nr++) {
					ind = sigseg.Procesar(columnas[nr]);
					rsl[0,ind[2]]++; 
					rsl[1,ind[3]]++; 
					rsl[2,ind[4]]++; 
					rsl[3,ind[1]]++;
				}
			}
			gbConds.Enabled = true;
			bMostrar.Enabled = true;
		}
		private void SelOrigen() {
			string tmp;
			gbConds.Enabled = false;
			bCalcular.Enabled = false;
			bMostrar.Enabled = false;
			OpenFileDialog openDialog = new OpenFileDialog();
			openDialog.InitialDirectory = Application.StartupPath + "/";
			openDialog.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			if(openDialog.ShowDialog() == DialogResult.OK) {
                string filein = Path.GetFullPath(openDialog.FileName);
                lFileIn.Text = Path.GetFileName(openDialog.FileName);
				numcol=0;
                IArchivoColumnas ac = new ArchivoColumnasTexto(filein);
				while (ac.SiguienteColumna()) {
					tmp = VerColumna(ac.LeeColumnaSinComas()); 
					if (tmp.Length==0) { MessageBox.Show("columna errónea = "+numcol); return; }
					columnas[numcol++] = tmp;
					lColOrg.Text = ""+numcol;
					if (numcol==500000) break;
					if (numcol%97==0) Application.DoEvents();
				}
				ac.Cerrar();
				lColOrg.Text = ""+numcol;
			}
			gbConds.Enabled = true;
			bCalcular.Enabled = true;
		}
		private string VerColumna (string columna) {
			string chval = "12xX";
		    if (columna.Length!=14) return "";
			for (int nr=0; nr<14; nr++)
			{
			    char ch = columna[nr];
			    if (chval.IndexOf(ch)<0) return "";
			}
		    return columna.Replace('x','X');
		}

		public Anastatics()
		{
			InitializeComponent();
		}
	
		
		
		void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Anastatics));
            this.rsigseg = new System.Windows.Forms.RadioButton();
            this.lFileIn = new System.Windows.Forms.Label();
            this.lColOrg = new System.Windows.Forms.Label();
            this.bMostrar = new System.Windows.Forms.Button();
            this.bCalcular = new System.Windows.Forms.Button();
            this.rinter = new System.Windows.Forms.RadioButton();
            this.rdib = new System.Windows.Forms.RadioButton();
            this.gbConds = new System.Windows.Forms.GroupBox();
            this.rdibrep = new System.Windows.Forms.RadioButton();
            this.rOtras = new System.Windows.Forms.RadioButton();
            this.bOrigen = new System.Windows.Forms.Button();
            this.gbConds.SuspendLayout();
            this.SuspendLayout();
            // 
            // rsigseg
            // 
            this.rsigseg.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rsigseg.ForeColor = System.Drawing.Color.Black;
            this.rsigseg.Location = new System.Drawing.Point(16, 96);
            this.rsigseg.Name = "rsigseg";
            this.rsigseg.Size = new System.Drawing.Size(136, 24);
            this.rsigseg.TabIndex = 3;
            this.rsigseg.Text = "Signos seguidos";
            // 
            // lFileIn
            // 
            this.lFileIn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lFileIn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lFileIn.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lFileIn.Location = new System.Drawing.Point(200, 78);
            this.lFileIn.Name = "lFileIn";
            this.lFileIn.Size = new System.Drawing.Size(142, 26);
            this.lFileIn.TabIndex = 4;
            this.lFileIn.Text = "Fichero origen";
            this.lFileIn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lColOrg
            // 
            this.lColOrg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lColOrg.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lColOrg.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lColOrg.Location = new System.Drawing.Point(200, 112);
            this.lColOrg.Name = "lColOrg";
            this.lColOrg.Size = new System.Drawing.Size(142, 26);
            this.lColOrg.TabIndex = 5;
            this.lColOrg.Text = "Columnas";
            this.lColOrg.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bMostrar
            // 
            this.bMostrar.BackColor = System.Drawing.Color.DarkSalmon;
            this.bMostrar.Enabled = false;
            this.bMostrar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bMostrar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bMostrar.Image = ((System.Drawing.Image)(resources.GetObject("bMostrar.Image")));
            this.bMostrar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bMostrar.Location = new System.Drawing.Point(200, 216);
            this.bMostrar.Name = "bMostrar";
            this.bMostrar.Size = new System.Drawing.Size(142, 32);
            this.bMostrar.TabIndex = 3;
            this.bMostrar.Text = "Mostrar resultados";
            this.bMostrar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bMostrar.UseVisualStyleBackColor = false;
            this.bMostrar.Click += new System.EventHandler(this.BMostrarClick);
            // 
            // bCalcular
            // 
            this.bCalcular.BackColor = System.Drawing.Color.DarkSalmon;
            this.bCalcular.Enabled = false;
            this.bCalcular.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bCalcular.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bCalcular.Image = ((System.Drawing.Image)(resources.GetObject("bCalcular.Image")));
            this.bCalcular.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bCalcular.Location = new System.Drawing.Point(200, 152);
            this.bCalcular.Name = "bCalcular";
            this.bCalcular.Size = new System.Drawing.Size(142, 32);
            this.bCalcular.TabIndex = 2;
            this.bCalcular.Text = "Calcular";
            this.bCalcular.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bCalcular.UseVisualStyleBackColor = false;
            this.bCalcular.Click += new System.EventHandler(this.BCalcularClick);
            // 
            // rinter
            // 
            this.rinter.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rinter.ForeColor = System.Drawing.Color.Black;
            this.rinter.Location = new System.Drawing.Point(16, 72);
            this.rinter.Name = "rinter";
            this.rinter.Size = new System.Drawing.Size(128, 24);
            this.rinter.TabIndex = 0;
            this.rinter.Text = "Interrupciones";
            // 
            // rdib
            // 
            this.rdib.Checked = true;
            this.rdib.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdib.ForeColor = System.Drawing.Color.Black;
            this.rdib.Location = new System.Drawing.Point(16, 24);
            this.rdib.Name = "rdib";
            this.rdib.Size = new System.Drawing.Size(128, 24);
            this.rdib.TabIndex = 0;
            this.rdib.TabStop = true;
            this.rdib.Text = "Variantes, X, 2";
            // 
            // gbConds
            // 
            this.gbConds.Controls.Add(this.rsigseg);
            this.gbConds.Controls.Add(this.rdibrep);
            this.gbConds.Controls.Add(this.rOtras);
            this.gbConds.Controls.Add(this.rdib);
            this.gbConds.Controls.Add(this.rinter);
            this.gbConds.Enabled = false;
            this.gbConds.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gbConds.ForeColor = System.Drawing.Color.Maroon;
            this.gbConds.Location = new System.Drawing.Point(16, 16);
            this.gbConds.Name = "gbConds";
            this.gbConds.Size = new System.Drawing.Size(168, 256);
            this.gbConds.TabIndex = 1;
            this.gbConds.TabStop = false;
            this.gbConds.Text = "Condiciones";
            // 
            // rdibrep
            // 
            this.rdibrep.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rdibrep.ForeColor = System.Drawing.Color.Black;
            this.rdibrep.Location = new System.Drawing.Point(16, 48);
            this.rdibrep.Name = "rdibrep";
            this.rdibrep.Size = new System.Drawing.Size(144, 24);
            this.rdibrep.TabIndex = 2;
            this.rdibrep.Text = "Sus coincidencias";
            // 
            // rOtras
            // 
            this.rOtras.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rOtras.ForeColor = System.Drawing.Color.Black;
            this.rOtras.Location = new System.Drawing.Point(16, 216);
            this.rOtras.Name = "rOtras";
            this.rOtras.Size = new System.Drawing.Size(96, 24);
            this.rOtras.TabIndex = 1;
            this.rOtras.Text = "seguirá... ";
            this.rOtras.Visible = false;
            // 
            // bOrigen
            // 
            this.bOrigen.BackColor = System.Drawing.Color.DarkSalmon;
            this.bOrigen.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bOrigen.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bOrigen.Image = ((System.Drawing.Image)(resources.GetObject("bOrigen.Image")));
            this.bOrigen.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bOrigen.Location = new System.Drawing.Point(200, 24);
            this.bOrigen.Name = "bOrigen";
            this.bOrigen.Size = new System.Drawing.Size(142, 32);
            this.bOrigen.TabIndex = 0;
            this.bOrigen.Text = "Selección origen";
            this.bOrigen.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bOrigen.UseVisualStyleBackColor = false;
            this.bOrigen.Click += new System.EventHandler(this.BOrigenClick);
            // 
            // Anastatics
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(358, 290);
            this.Controls.Add(this.lColOrg);
            this.Controls.Add(this.lFileIn);
            this.Controls.Add(this.bMostrar);
            this.Controls.Add(this.bCalcular);
            this.Controls.Add(this.gbConds);
            this.Controls.Add(this.bOrigen);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Anastatics";
            this.Text = "Estadísticas 0.3.4";
            this.gbConds.ResumeLayout(false);
            this.ResumeLayout(false);

		}
			
		void BOrigenClick(object sender, EventArgs e) { SelOrigen(); }
		void BCalcularClick(object sender, EventArgs e) { Proceso(); }
		void BMostrarClick(object sender, EventArgs e) { Mostrar(); }


	}			
}
