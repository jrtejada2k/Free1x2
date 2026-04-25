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
using System.Timers;
using System.Collections;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI 
{
	public class DifCols : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label;
		private System.Windows.Forms.TextBox tbdifbase;
		private System.Windows.Forms.GroupBox groupBox;
		private System.Windows.Forms.Label lblFileIn;
		private System.Windows.Forms.TextBox tbdifresul;
		private System.Windows.Forms.Button bGrab;
		private System.Windows.Forms.RadioButton rbColsB;
		private System.Windows.Forms.RadioButton rb14T;
		private System.Windows.Forms.RadioButton rbFile;
		private System.Windows.Forms.TextBox tblimcol;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button bCancelar;
		private System.Windows.Forms.Label lblTime;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.RadioButton rbBase;
		private System.Windows.Forms.Label lblResul;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button bCalc;
		private System.Windows.Forms.Label lblFileCond;
		private System.Windows.Forms.TextBox tbColbase;
		private System.Windows.Forms.Button btnDiferencias;
		private System.Windows.Forms.Label label4;
		public DifCols()
		{
			InitializeComponent();
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
        }

		
		private ArrayList condis = new ArrayList();
		private ArrayList aceptadas = new ArrayList();
		private string difbase, difresul, mask;
		private bool salida = false;
		
		private string VerColumna (string columna) {
			string chval = "124xXF", xcol; char ch;
			if (columna.Length!=14) return "";
			mask="";
			for (int nr=0; nr<14; nr++) {
				ch = columna[nr];
				if (chval.IndexOf(ch)<0) return "";
				if (chval.IndexOf(ch)==5) mask+='F'; else mask+='V';
			}
			xcol = columna.Replace('x','4');
			xcol = xcol.Replace('X','4');
			return xcol;
		}
		private void Calcular() {
			tbColbase.Enabled = false;
			tbdifbase.Enabled = false;
			tbdifresul.Enabled = false;
			bCalc.Enabled = false;
			bGrab.Enabled = false;
			salida = false;
			DateTime time0 = DateTime.Now;
			PreparaDifs();
			string tmp, msg="";
			condis.Clear(); aceptadas.Clear();
			lblResul.Text = "calculando..."; lblTime.Text = " ";
			lblFileIn.Text = lblFileCond.Text = " ";
			if (rbBase.Checked) {
				tmp = VerColumna(tbColbase.Text);
				if (tmp.Length==0) msg="columna errónea";
				else condis.Add(tmp);
			}
			else {
				OpenFileDialog openDialog = new OpenFileDialog();
				openDialog.InitialDirectory = Application.StartupPath + "/";
				openDialog.Filter = "F.Condiciones(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
				if(openDialog.ShowDialog() == DialogResult.OK) {
					string filein = Path.GetFileName(openDialog.FileName);
					lblFileCond.Text = filein;
					StreamReader sr = new StreamReader(filein);
					while (sr.Peek()>0) {
						tmp = VerColumna(sr.ReadLine());
								if (tmp.Length==0) {msg="columna errónea"; break;}
								else condis.Add(tmp);
						Application.DoEvents();
					}
					sr.Close();
				}
			}
			if (msg=="") {
				if (rb14T.Checked) MetodoInterno(); else MetodoExterno();
			}
			else MessageBox.Show(msg);
			lblResul.Text=aceptadas.Count.ToString();
			DateTime time9 = DateTime.Now;
			lblTime.Text = (time9-time0).ToString().Substring(0,10);
			tbColbase.Enabled = true;
			tbdifbase.Enabled = true;
			tbdifresul.Enabled = true;
			bCalc.Enabled = true;
			bGrab.Enabled = true;
		}
		private void MetodoInterno() {
			string c1,c2,c3,c4,c5,c6,c7,c8,c9,c10,c11,c12,c13,c14,tmp;
			for (int nr1=1; nr1<4; nr1++) { c1=nr1.ToString();
				for (int nr2=1; nr2<4; nr2++) { c2=c1+nr2.ToString();
					for (int nr3=1; nr3<4; nr3++) { c3=c2+nr3.ToString();
						for (int nr4=1; nr4<4; nr4++) { c4=c3+nr4.ToString();
							for (int nr5=1; nr5<4; nr5++) { c5=c4+nr5.ToString();
								for (int nr6=1; nr6<4; nr6++) { c6=c5+nr6.ToString();
									for (int nr7=1; nr7<4; nr7++) { c7=c6+nr7.ToString();
										for (int nr8=1; nr8<4; nr8++) { c8=c7+nr8.ToString();
											for (int nr9=1; nr9<4; nr9++) { c9=c8+nr9.ToString();
												for (int nr10=1; nr10<4; nr10++) { c10=c9+nr10.ToString();
													for (int nr11=1; nr11<4; nr11++) { c11=c10+nr11.ToString();
														for (int nr12=1; nr12<4; nr12++) { c12=c11+nr12.ToString();
															for (int nr13=1; nr13<4; nr13++) { c13=c12+nr13.ToString();
																for (int nr14=1; nr14<4; nr14++) { c14=c13+nr14.ToString();
																	Application.DoEvents();
																	if (salida) return;
																	tmp=c14.Replace('3','4');
																	Proceso(tmp);
																}}}}}}}}}}}}}}
		}
		private void MetodoExterno() {
			string tmp;
			OpenFileDialog openDialog = new OpenFileDialog();
			openDialog.InitialDirectory = Application.StartupPath + "/";
			openDialog.Filter = "F.Entrada(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			if(openDialog.ShowDialog() == DialogResult.OK) {
				string filein = Path.GetFileName(openDialog.FileName);
				lblFileIn.Text = filein;
				StreamReader sr = new StreamReader(filein);
				while (sr.Peek()>0) {
					Application.DoEvents();
					if (salida) return;
					tmp = VerColumna(sr.ReadLine());
					if (tmp.Length==0) { MessageBox.Show("columna errónea"); return; }
					Proceso(tmp);
				}
				sr.Close();
			}
		}
		private void Proceso(string columna) {
			int nd=0; string tmp;
			foreach (string cnd in condis) {
				nd = compara(columna, cnd);
				if (difbase[nd]=='F') return;
			}
			foreach (string cnd in aceptadas) {
				nd = compara(columna, cnd);
				if (difresul[nd]=='F') return;
			}
			tmp="";
			for (int nr=0; nr<14; nr++) {
				if (mask[nr]=='F') tmp+='F';
				else tmp+=columna[nr];
			}
			aceptadas.Add(tmp);
			lblResul.Text=aceptadas.Count.ToString();
		}
		private int compara(string nova, string refer) {
			int nd=0, ch;
			for (int nr=0; nr<14; nr++) {
				ch = mask[nr];
				if (ch=='F') continue;
				ch = nova[nr] & refer[nr];
				if (ch==48) nd++;
			}
			return nd;
		}
		private string GetCol(int col) {
			string resul = ""; char ch;
			for (int nr=0; nr<14; nr++) {
				ch = Convert.ToChar((col%3)+48);
				if (ch=='0') ch='4';
				resul = ch+resul;
				col/=3;
			}
			return resul;
		}
		private void Grabar() {
			int limite=0, ngrab=0;
			bCalc.Enabled = false;
			bGrab.Enabled = false;
			SaveFileDialog saveDialog = new SaveFileDialog();
			saveDialog.InitialDirectory = Application.StartupPath + "/";
			saveDialog.Filter = "F.Salida(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			if(saveDialog.ShowDialog() == DialogResult.OK) {
				string fileout = Path.GetFileName(saveDialog.FileName);
				StreamWriter sw = new StreamWriter(fileout);
				try { limite = Convert.ToInt32(tblimcol.Text); }
				catch { limite=4782969; }
				foreach (string cps in aceptadas) {
					sw.WriteLine(cps.Replace('4','X'));
					ngrab++; if (ngrab>=limite) break;
				}
				sw.Close();
			}
			bCalc.Enabled = true;
			bGrab.Enabled = true;
		}
		private void PreparaDifs() {
			string[] mgc = null;
			string[] mgg = null;
			string tmp; int nv1, nv2;
			difbase = difresul = "FFFFFFFFFFFFFFF";
			mgc = tbdifbase.Text.Split(',');
			for (int nr2=0; nr2<mgc.Length; nr2++) {
				tmp=mgc[nr2];
				mgg = tmp.Split('-');
				nv1 = Convert.ToInt32(mgg[0]);
				if (nv1<0) nv1=0; if (nv1>14) nv1=14;
				if (mgg.Length==2) nv2 = Convert.ToInt32(mgg[1]);
				else nv2 = nv1;
				if (nv2<0) nv2=0; if (nv2>14) nv2=14;
				tmp = "";
				for (int nr=0; nr<nv1; nr++) tmp+=difbase[nr];
				for (int nr=nv1; nr<=nv2; nr++) tmp+='A';
				for (int nr=nv2+1; nr<15; nr++) tmp+=difbase[nr];
				difbase = tmp;
			}
			mgc = tbdifresul.Text.Split(',');
			for (int nr2=0; nr2<mgc.Length; nr2++) {
				tmp=mgc[nr2];
				mgg = tmp.Split('-');
				nv1 = Convert.ToInt32(mgg[0]);
				if (nv1<0) nv1=0; if (nv1>14) nv1=14;
				if (mgg.Length==2) nv2 = Convert.ToInt32(mgg[1]);
				else nv2 = nv1;
				if (nv2<0) nv2=0; if (nv2>14) nv2=14;
				tmp = "";
				for (int nr=0; nr<nv1; nr++) tmp+=difresul[nr];
				for (int nr=nv1; nr<=nv2; nr++) tmp+='A';
				for (int nr=nv2+1; nr<15; nr++) tmp+=difresul[nr];
				difresul = tmp;
			}
		}
		
		
		
		void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DifCols));
            this.label4 = new System.Windows.Forms.Label();
            this.tbColbase = new System.Windows.Forms.TextBox();
            this.lblFileCond = new System.Windows.Forms.Label();
            this.bCalc = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblResul = new System.Windows.Forms.Label();
            this.rbBase = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.lblTime = new System.Windows.Forms.Label();
            this.bCancelar = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label = new System.Windows.Forms.Label();
            this.tbdifresul = new System.Windows.Forms.TextBox();
            this.rbColsB = new System.Windows.Forms.RadioButton();
            this.tbdifbase = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnDiferencias = new System.Windows.Forms.Button();
            this.tblimcol = new System.Windows.Forms.TextBox();
            this.bGrab = new System.Windows.Forms.Button();
            this.rbFile = new System.Windows.Forms.RadioButton();
            this.rb14T = new System.Windows.Forms.RadioButton();
            this.lblFileIn = new System.Windows.Forms.Label();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(27, 107);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 21);
            this.label4.TabIndex = 8;
            this.label4.Text = "s/col.Base";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbColbase
            // 
            this.tbColbase.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbColbase.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tbColbase.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbColbase.ForeColor = System.Drawing.Color.Black;
            this.tbColbase.Location = new System.Drawing.Point(128, 24);
            this.tbColbase.MaxLength = 14;
            this.tbColbase.Name = "tbColbase";
            this.tbColbase.Size = new System.Drawing.Size(115, 21);
            this.tbColbase.TabIndex = 0;
            this.tbColbase.Text = "1X1212X2111121";
            this.tbColbase.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lblFileCond
            // 
            this.lblFileCond.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblFileCond.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFileCond.ForeColor = System.Drawing.Color.Black;
            this.lblFileCond.Location = new System.Drawing.Point(128, 48);
            this.lblFileCond.Name = "lblFileCond";
            this.lblFileCond.Size = new System.Drawing.Size(115, 16);
            this.lblFileCond.TabIndex = 4;
            // 
            // bCalc
            // 
            this.bCalc.BackColor = System.Drawing.Color.DarkSalmon;
            this.bCalc.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bCalc.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bCalc.ForeColor = System.Drawing.Color.Black;
            this.bCalc.Image = ((System.Drawing.Image)(resources.GetObject("bCalc.Image")));
            this.bCalc.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bCalc.Location = new System.Drawing.Point(16, 24);
            this.bCalc.Name = "bCalc";
            this.bCalc.Size = new System.Drawing.Size(148, 32);
            this.bCalc.TabIndex = 3;
            this.bCalc.Text = "Calcular";
            this.bCalc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bCalc.UseVisualStyleBackColor = false;
            this.bCalc.Click += new System.EventHandler(this.BCalcClick);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(16, 144);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(148, 16);
            this.label1.TabIndex = 8;
            this.label1.Text = "lím.col";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(27, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 21);
            this.label3.TabIndex = 3;
            this.label3.Text = "diferencias";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblResul
            // 
            this.lblResul.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblResul.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblResul.ForeColor = System.Drawing.Color.Black;
            this.lblResul.Location = new System.Drawing.Point(16, 63);
            this.lblResul.Name = "lblResul";
            this.lblResul.Size = new System.Drawing.Size(148, 16);
            this.lblResul.TabIndex = 4;
            this.lblResul.Text = "Admitidas";
            this.lblResul.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rbBase
            // 
            this.rbBase.Checked = true;
            this.rbBase.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbBase.ForeColor = System.Drawing.Color.Black;
            this.rbBase.Location = new System.Drawing.Point(16, 24);
            this.rbBase.Name = "rbBase";
            this.rbBase.Size = new System.Drawing.Size(104, 16);
            this.rbBase.TabIndex = 0;
            this.rbBase.TabStop = true;
            this.rbBase.Text = "Columna Base";
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Black;
            this.label5.Location = new System.Drawing.Point(27, 136);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(77, 21);
            this.label5.TabIndex = 9;
            this.label5.Text = "s/col.result.";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTime
            // 
            this.lblTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTime.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTime.ForeColor = System.Drawing.Color.Black;
            this.lblTime.Location = new System.Drawing.Point(16, 80);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(148, 16);
            this.lblTime.TabIndex = 6;
            this.lblTime.Text = "Tiempo";
            this.lblTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bCancelar
            // 
            this.bCancelar.BackColor = System.Drawing.Color.DarkSalmon;
            this.bCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bCancelar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bCancelar.ForeColor = System.Drawing.Color.Black;
            this.bCancelar.Image = ((System.Drawing.Image)(resources.GetObject("bCancelar.Image")));
            this.bCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bCancelar.Location = new System.Drawing.Point(16, 104);
            this.bCancelar.Name = "bCancelar";
            this.bCancelar.Size = new System.Drawing.Size(148, 32);
            this.bCancelar.TabIndex = 7;
            this.bCancelar.Text = "Cancelar";
            this.bCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bCancelar.UseVisualStyleBackColor = false;
            this.bCancelar.Click += new System.EventHandler(this.BCancelarClick);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label);
            this.groupBox2.Controls.Add(this.tbdifresul);
            this.groupBox2.Controls.Add(this.lblFileCond);
            this.groupBox2.Controls.Add(this.rbColsB);
            this.groupBox2.Controls.Add(this.rbBase);
            this.groupBox2.Controls.Add(this.tbColbase);
            this.groupBox2.Controls.Add(this.tbdifbase);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox2.Location = new System.Drawing.Point(16, 104);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(256, 176);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Condicionada por";
            // 
            // label
            // 
            this.label.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label.ForeColor = System.Drawing.Color.Black;
            this.label.Location = new System.Drawing.Point(110, 83);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(80, 21);
            this.label.TabIndex = 6;
            this.label.Text = "min(-,-)max";
            this.label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbdifresul
            // 
            this.tbdifresul.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbdifresul.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbdifresul.ForeColor = System.Drawing.Color.Black;
            this.tbdifresul.Location = new System.Drawing.Point(110, 136);
            this.tbdifresul.MaxLength = 20;
            this.tbdifresul.Name = "tbdifresul";
            this.tbdifresul.Size = new System.Drawing.Size(77, 21);
            this.tbdifresul.TabIndex = 5;
            this.tbdifresul.Text = "6-14";
            this.tbdifresul.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // rbColsB
            // 
            this.rbColsB.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbColsB.ForeColor = System.Drawing.Color.Black;
            this.rbColsB.Location = new System.Drawing.Point(16, 48);
            this.rbColsB.Name = "rbColsB";
            this.rbColsB.Size = new System.Drawing.Size(104, 16);
            this.rbColsB.TabIndex = 1;
            this.rbColsB.Text = "Fichero";
            // 
            // tbdifbase
            // 
            this.tbdifbase.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbdifbase.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbdifbase.ForeColor = System.Drawing.Color.Black;
            this.tbdifbase.Location = new System.Drawing.Point(110, 107);
            this.tbdifbase.MaxLength = 20;
            this.tbdifbase.Name = "tbdifbase";
            this.tbdifbase.Size = new System.Drawing.Size(77, 21);
            this.tbdifbase.TabIndex = 1;
            this.tbdifbase.Text = "0-5,6,7-14";
            this.tbdifbase.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnDiferencias);
            this.groupBox3.Controls.Add(this.tblimcol);
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.bCancelar);
            this.groupBox3.Controls.Add(this.bCalc);
            this.groupBox3.Controls.Add(this.lblResul);
            this.groupBox3.Controls.Add(this.bGrab);
            this.groupBox3.Controls.Add(this.lblTime);
            this.groupBox3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox3.Location = new System.Drawing.Point(288, 8);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(170, 272);
            this.groupBox3.TabIndex = 10;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Proceso";
            // 
            // btnDiferencias
            // 
            this.btnDiferencias.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnDiferencias.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDiferencias.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDiferencias.ForeColor = System.Drawing.Color.Black;
            this.btnDiferencias.Image = ((System.Drawing.Image)(resources.GetObject("btnDiferencias.Image")));
            this.btnDiferencias.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDiferencias.Location = new System.Drawing.Point(16, 232);
            this.btnDiferencias.Name = "btnDiferencias";
            this.btnDiferencias.Size = new System.Drawing.Size(148, 32);
            this.btnDiferencias.TabIndex = 88;
            this.btnDiferencias.Text = "CPs por diferencias";
            this.btnDiferencias.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnDiferencias.UseVisualStyleBackColor = false;
            this.btnDiferencias.Click += new System.EventHandler(this.btnDiferencias_Click);
            // 
            // tblimcol
            // 
            this.tblimcol.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tblimcol.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tblimcol.ForeColor = System.Drawing.Color.Black;
            this.tblimcol.Location = new System.Drawing.Point(16, 163);
            this.tblimcol.MaxLength = 14;
            this.tblimcol.Name = "tblimcol";
            this.tblimcol.Size = new System.Drawing.Size(148, 21);
            this.tblimcol.TabIndex = 9;
            this.tblimcol.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bGrab
            // 
            this.bGrab.BackColor = System.Drawing.Color.DarkSalmon;
            this.bGrab.Enabled = false;
            this.bGrab.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bGrab.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bGrab.ForeColor = System.Drawing.Color.Black;
            this.bGrab.Image = ((System.Drawing.Image)(resources.GetObject("bGrab.Image")));
            this.bGrab.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bGrab.Location = new System.Drawing.Point(16, 192);
            this.bGrab.Name = "bGrab";
            this.bGrab.Size = new System.Drawing.Size(148, 32);
            this.bGrab.TabIndex = 5;
            this.bGrab.Text = "Grabar";
            this.bGrab.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bGrab.UseVisualStyleBackColor = false;
            this.bGrab.Click += new System.EventHandler(this.BGrabClick);
            // 
            // rbFile
            // 
            this.rbFile.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbFile.ForeColor = System.Drawing.Color.Black;
            this.rbFile.Location = new System.Drawing.Point(16, 48);
            this.rbFile.Name = "rbFile";
            this.rbFile.Size = new System.Drawing.Size(80, 16);
            this.rbFile.TabIndex = 1;
            this.rbFile.Text = "Fichero";
            // 
            // rb14T
            // 
            this.rb14T.Checked = true;
            this.rb14T.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rb14T.ForeColor = System.Drawing.Color.Black;
            this.rb14T.Location = new System.Drawing.Point(16, 24);
            this.rb14T.Name = "rb14T";
            this.rb14T.Size = new System.Drawing.Size(80, 16);
            this.rb14T.TabIndex = 0;
            this.rb14T.TabStop = true;
            this.rb14T.Text = "14 triples";
            // 
            // lblFileIn
            // 
            this.lblFileIn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblFileIn.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFileIn.ForeColor = System.Drawing.Color.Black;
            this.lblFileIn.Location = new System.Drawing.Point(128, 48);
            this.lblFileIn.Name = "lblFileIn";
            this.lblFileIn.Size = new System.Drawing.Size(115, 16);
            this.lblFileIn.TabIndex = 2;
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(this.lblFileIn);
            this.groupBox.Controls.Add(this.rbFile);
            this.groupBox.Controls.Add(this.rb14T);
            this.groupBox.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox.Location = new System.Drawing.Point(16, 8);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(256, 80);
            this.groupBox.TabIndex = 2;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Generación por";
            // 
            // DifCols
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(470, 294);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DifCols";
            this.Text = "Diferencias entre columnas";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		
		void BCancelarClick(object sender, System.EventArgs e) { salida=true; }
		void BCalcClick(object sender, System.EventArgs e) { Calcular(); }
		void BGrabClick(object sender, System.EventArgs e) { Grabar(); }

		private void btnDiferencias_Click(object sender, System.EventArgs e)
		{
			GeneradorCPSDiferencias f=new GeneradorCPSDiferencias();
			f.ShowDialog();
		}

	}
}
