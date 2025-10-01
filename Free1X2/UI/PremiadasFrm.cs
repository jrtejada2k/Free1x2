using System;
using System.Windows.Forms;
using System.IO;

namespace Free1X2.UI
{
	public class PremiadasFrm : System.Windows.Forms.Form   
	{
		private System.Windows.Forms.RadioButton rb10;
		private System.Windows.Forms.RadioButton rb11;
		private System.Windows.Forms.RadioButton rb12;
		private System.Windows.Forms.RadioButton rb13;
		private System.Windows.Forms.Button bCalcular;
		private System.Windows.Forms.Button bFileIn;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label lFileIn;
		private System.Windows.Forms.ListBox lbSecuencia;
		private System.Windows.Forms.Button bAnaliza;
		private System.Windows.Forms.Label lTime;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label lProc;
		private System.Windows.Forms.Button bGrabar;
		private System.Windows.Forms.ListBox lbPremis;
		public PremiadasFrm()
		{
			InitializeComponent();
			elmeu = new Timer();
			elmeu.Interval = 3000;
   		    elmeu.Tick += new EventHandler(elmeuTimer);
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}
		
		private int[] validas = new int[4782969];
		private int idx, nmax, ctproc, jornada;
		private bool salida;
		private DateTime dt0, dt9;
		private Timer elmeu;
		private int[] pot = new int[] {1,3,9,27,81,243,729,2187,6561,19683,59049,177147,531441,1594323};
		private int[] qdc;
		private string faux, filein, fileout;
		
		private void Calcular() {
			bCalcular.Enabled = false;
			salida = false;
			elmeu.Start();
			dt0 = DateTime.Now;
			ctproc=nmax=0;
			for (int nr=0; nr<4782969; nr++) validas[nr]=0;
			filein = lFileIn.Text;
			StreamReader sr = new StreamReader(filein);
			while (sr.Peek()>0) {
				if (salida) break;
				idx = s1n(sr.ReadLine()); ctproc++;
				Genera(idx);
				Application.DoEvents();
			}
			sr.Close();
			Trasvasa();
			elmeu.Stop();
			veureelmeu();
			bCalcular.Enabled = true;
		}
		private void veureelmeu() {
			lProc.Text = ""+ctproc;
 			dt9 = DateTime.Now;
			string temp = (dt9-dt0).ToString()+"0000000000";
			lTime.Text = temp.Substring(0,10);
		}
 		private int s1n(string ax) {
			int nx = 0;
			for (int nr=0; nr<14; nr++) {
				nx *= 3;
				string ch = ax.Substring(nr,1);
				if (ch=="1") nx+=1;
				else if (ch=="2") nx+=2;
			}
			return nx;
		}
		private string n1s(int nx) {
			string ax = ""; int nx2;
			for (int nr=0; nr<14; nr++) {
				nx2 = nx%3; nx /= 3;
				if (nx2==1) ax = "1"+ax;
				else if (nx2==2) ax = "2"+ax;
				else ax = "X"+ax;
			}
			return ax;
		}
 		private void Genera(int nsel) {
 			int n, z1,col1,sign1, z2,col2,sign2, z3,col3,sign3, z4,col4,sign4;
			for (int nr=0; nr<14; nr++) {
				sign1 = (nsel / pot[nr]) % 3;
				for (z1=0; z1<3; z1++) {
					if (z1 == sign1) continue;
					col1 = nsel + pot[nr] * (z1 - sign1);
					if (rb13.Checked) {
						n=validas[col1]+1; if (n>nmax) nmax=n; validas[col1]=n;
					}
					for (int nr2=nr+1; nr2<14; nr2++) {
						sign2 = (col1 / pot[nr2]) % 3;
						for (z2=0; z2<3; z2++) {
							if (z2 == sign2) continue;
							col2 = col1 + pot[nr2] * (z2 - sign2);
							if (rb12.Checked) {
								n=validas[col2]+1; if (n>nmax) nmax=n; validas[col2]=n;
							}
							for (int nr3=nr2+1; nr3<14; nr3++) {
								sign3 = (col2 / pot[nr3]) % 3;
								for (z3=0; z3<3; z3++) {
									if (z3 == sign3) continue;
									col3 = col2 + pot[nr3] * (z3 - sign3);
									if (rb11.Checked) {
										n=validas[col3]+1; if (n>nmax) nmax=n; validas[col3]=n;
									}
									for (int nr4=nr3+1; nr4<14; nr4++) {
										sign4 = (col3 / pot[nr4]) % 3;
										for (z4=0; z4<3; z4++) {
											if (z4 == sign4) continue;
											col4 = col3 + pot[nr4] * (z4 - sign4);
											if (rb10.Checked) {
												n=validas[col4]+1; if (n>nmax) nmax=n; validas[col4]=n;
											}
										}
									}
								}
							}
						}
					}
				}
			}
 		}
		private void Trasvasa() {
			string tmp;
			lbPremis.Items.Clear();
			qdc = new int[nmax];
			foreach (int n in validas) {
				if (n>0) qdc[n-1]++;
			}
			for (int nr=0; nr<nmax; nr++) {
				tmp = String.Format("{0:d} = {1:d} veces",qdc[nr],(nr+1));
				lbPremis.Items.Add(tmp);
			}
		}
		private void Grabar() {
			bGrabar.Enabled=false;
			bCalcular.Visible=false;
			int n = lbPremis.SelectedIndex+1;
			SaveFileDialog sav = new SaveFileDialog();
			sav.InitialDirectory = ".\\" ;
			sav.Filter = "Cols.Salida(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
			if(sav.ShowDialog() == DialogResult.OK) {
		   		faux = sav.FileName;
		   		fileout = Path.GetFileName(faux);
				StreamWriter sw = new StreamWriter(fileout);
				for (int nr=0; nr<4782969; nr++) {
					if (validas[nr]==n) sw.WriteLine(n1s(nr));
				}
				sw.Close();
			}
			bCalcular.Visible=true;
			bGrabar.Enabled=true;
		}
		private void Analiza() {
			bAnaliza.Enabled=false;
			bCalcular.Visible=false;
			lbSecuencia.Items.Clear();
			jornada=0;
			StreamReader sr = new StreamReader(filein);
			while (sr.Peek()>0) {
				idx = s1n(sr.ReadLine()); jornada++;
				Examina(idx);
			}
			sr.Close();
			bAnaliza.Enabled=true;
			bCalcular.Visible=true;
		}
 		private void Examina(int nsel) {
 			int n, z1,col1,sign1, z2,col2,sign2, z3,col3,sign3, z4,col4,sign4;
			n = lbPremis.SelectedIndex+1;
			for (int nr=0; nr<14; nr++) {
				sign1 = (nsel / pot[nr]) % 3;
				for (z1=0; z1<3; z1++) {
					if (z1 == sign1) continue;
					col1 = nsel + pot[nr] * (z1 - sign1);
					if (rb13.Checked) {
						if (validas[col1]==n) lbSecuencia.Items.Add("sem."+jornada);
					}
					for (int nr2=nr+1; nr2<14; nr2++) {
						sign2 = (col1 / pot[nr2]) % 3;
						for (z2=0; z2<3; z2++) {
							if (z2 == sign2) continue;
							col2 = col1 + pot[nr2] * (z2 - sign2);
							if (rb12.Checked) {
								if (validas[col2]==n) lbSecuencia.Items.Add("sem."+jornada);
							}
							for (int nr3=nr2+1; nr3<14; nr3++) {
								sign3 = (col2 / pot[nr3]) % 3;
								for (z3=0; z3<3; z3++) {
									if (z3 == sign3) continue;
									col3 = col2 + pot[nr3] * (z3 - sign3);
									if (rb11.Checked) {
										if (validas[col3]==n) lbSecuencia.Items.Add("sem."+jornada);
									}
									for (int nr4=nr3+1; nr4<14; nr4++) {
										sign4 = (col3 / pot[nr4]) % 3;
										for (z4=0; z4<3; z4++) {
											if (z4 == sign4) continue;
											col4 = col3 + pot[nr4] * (z4 - sign4);
											if (rb10.Checked) {
												if (validas[col4]==n) lbSecuencia.Items.Add("sem."+jornada);
											}
										}
									}
								}
							}
						}
					}
				}
			}
 		}
		private void SelFileIn() {
			OpenFileDialog lee = new OpenFileDialog();
			lee.InitialDirectory = ".\\" ;
			lee.Filter = "Cols.Ganadoras(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
			if(lee.ShowDialog() == DialogResult.OK) {
		   		faux = lee.FileName;
		   		filein = Path.GetFileName(faux);
				lFileIn.Text = filein;
			}
		}
		

		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PremiadasFrm));
            this.lbPremis = new System.Windows.Forms.ListBox();
            this.bGrabar = new System.Windows.Forms.Button();
            this.lProc = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lTime = new System.Windows.Forms.Label();
            this.bAnaliza = new System.Windows.Forms.Button();
            this.lbSecuencia = new System.Windows.Forms.ListBox();
            this.lFileIn = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rb10 = new System.Windows.Forms.RadioButton();
            this.rb11 = new System.Windows.Forms.RadioButton();
            this.rb12 = new System.Windows.Forms.RadioButton();
            this.rb13 = new System.Windows.Forms.RadioButton();
            this.bFileIn = new System.Windows.Forms.Button();
            this.bCalcular = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbPremis
            // 
            this.lbPremis.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbPremis.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPremis.Location = new System.Drawing.Point(16, 40);
            this.lbPremis.Name = "lbPremis";
            this.lbPremis.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lbPremis.Size = new System.Drawing.Size(136, 288);
            this.lbPremis.TabIndex = 0;
            // 
            // bGrabar
            // 
            this.bGrabar.BackColor = System.Drawing.Color.DarkSalmon;
            this.bGrabar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bGrabar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bGrabar.Image = ((System.Drawing.Image)(resources.GetObject("bGrabar.Image")));
            this.bGrabar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bGrabar.Location = new System.Drawing.Point(293, 280);
            this.bGrabar.Name = "bGrabar";
            this.bGrabar.Size = new System.Drawing.Size(134, 32);
            this.bGrabar.TabIndex = 5;
            this.bGrabar.Text = "Grabar Selección";
            this.bGrabar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bGrabar.UseVisualStyleBackColor = false;
            this.bGrabar.Click += new System.EventHandler(this.BGrabarClick);
            // 
            // lProc
            // 
            this.lProc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lProc.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lProc.Location = new System.Drawing.Point(248, 223);
            this.lProc.Name = "lProc";
            this.lProc.Size = new System.Drawing.Size(88, 24);
            this.lProc.TabIndex = 2;
            this.lProc.Text = "0";
            this.lProc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Maroon;
            this.label1.Location = new System.Drawing.Point(16, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(136, 24);
            this.label1.TabIndex = 9;
            this.label1.Text = "Frecuencias";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Maroon;
            this.label2.Location = new System.Drawing.Point(432, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 24);
            this.label2.TabIndex = 10;
            this.label2.Text = "Jornadas";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lTime
            // 
            this.lTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lTime.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lTime.Location = new System.Drawing.Point(248, 248);
            this.lTime.Name = "lTime";
            this.lTime.Size = new System.Drawing.Size(88, 23);
            this.lTime.TabIndex = 4;
            this.lTime.Text = "00:00:00.0";
            this.lTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bAnaliza
            // 
            this.bAnaliza.BackColor = System.Drawing.Color.DarkSalmon;
            this.bAnaliza.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bAnaliza.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bAnaliza.Image = ((System.Drawing.Image)(resources.GetObject("bAnaliza.Image")));
            this.bAnaliza.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bAnaliza.Location = new System.Drawing.Point(157, 280);
            this.bAnaliza.Name = "bAnaliza";
            this.bAnaliza.Size = new System.Drawing.Size(134, 32);
            this.bAnaliza.TabIndex = 7;
            this.bAnaliza.Text = "Analiza Selección";
            this.bAnaliza.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bAnaliza.UseVisualStyleBackColor = false;
            this.bAnaliza.Click += new System.EventHandler(this.BAnalizaClick);
            // 
            // lbSecuencia
            // 
            this.lbSecuencia.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbSecuencia.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSecuencia.Location = new System.Drawing.Point(432, 40);
            this.lbSecuencia.Name = "lbSecuencia";
            this.lbSecuencia.Size = new System.Drawing.Size(88, 288);
            this.lbSecuencia.TabIndex = 8;
            // 
            // lFileIn
            // 
            this.lFileIn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lFileIn.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lFileIn.Location = new System.Drawing.Point(192, 136);
            this.lFileIn.Name = "lFileIn";
            this.lFileIn.Size = new System.Drawing.Size(200, 24);
            this.lFileIn.TabIndex = 12;
            this.lFileIn.Text = "Seleccionar Previamente";
            this.lFileIn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rb10);
            this.groupBox1.Controls.Add(this.rb11);
            this.groupBox1.Controls.Add(this.rb12);
            this.groupBox1.Controls.Add(this.rb13);
            this.groupBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox1.Location = new System.Drawing.Point(168, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(248, 56);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Premios a estudiar";
            // 
            // rb10
            // 
            this.rb10.Checked = true;
            this.rb10.ForeColor = System.Drawing.Color.Black;
            this.rb10.Location = new System.Drawing.Point(184, 24);
            this.rb10.Name = "rb10";
            this.rb10.Size = new System.Drawing.Size(48, 16);
            this.rb10.TabIndex = 3;
            this.rb10.TabStop = true;
            this.rb10.Text = "10";
            // 
            // rb11
            // 
            this.rb11.ForeColor = System.Drawing.Color.Black;
            this.rb11.Location = new System.Drawing.Point(128, 24);
            this.rb11.Name = "rb11";
            this.rb11.Size = new System.Drawing.Size(48, 16);
            this.rb11.TabIndex = 2;
            this.rb11.Text = "11";
            // 
            // rb12
            // 
            this.rb12.ForeColor = System.Drawing.Color.Black;
            this.rb12.Location = new System.Drawing.Point(72, 24);
            this.rb12.Name = "rb12";
            this.rb12.Size = new System.Drawing.Size(48, 16);
            this.rb12.TabIndex = 1;
            this.rb12.Text = "12";
            // 
            // rb13
            // 
            this.rb13.ForeColor = System.Drawing.Color.Black;
            this.rb13.Location = new System.Drawing.Point(16, 24);
            this.rb13.Name = "rb13";
            this.rb13.Size = new System.Drawing.Size(48, 16);
            this.rb13.TabIndex = 0;
            this.rb13.Text = "13";
            // 
            // bFileIn
            // 
            this.bFileIn.BackColor = System.Drawing.Color.DarkSalmon;
            this.bFileIn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bFileIn.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bFileIn.Image = ((System.Drawing.Image)(resources.GetObject("bFileIn.Image")));
            this.bFileIn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bFileIn.Location = new System.Drawing.Point(232, 70);
            this.bFileIn.Name = "bFileIn";
            this.bFileIn.Size = new System.Drawing.Size(141, 32);
            this.bFileIn.TabIndex = 11;
            this.bFileIn.Text = "Fichero Ganadoras";
            this.bFileIn.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bFileIn.UseVisualStyleBackColor = false;
            this.bFileIn.Click += new System.EventHandler(this.BFileInClick);
            // 
            // bCalcular
            // 
            this.bCalcular.BackColor = System.Drawing.Color.DarkSalmon;
            this.bCalcular.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bCalcular.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bCalcular.Image = ((System.Drawing.Image)(resources.GetObject("bCalcular.Image")));
            this.bCalcular.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bCalcular.Location = new System.Drawing.Point(232, 168);
            this.bCalcular.Name = "bCalcular";
            this.bCalcular.Size = new System.Drawing.Size(141, 32);
            this.bCalcular.TabIndex = 1;
            this.bCalcular.Text = "Calcular";
            this.bCalcular.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bCalcular.UseVisualStyleBackColor = false;
            this.bCalcular.Click += new System.EventHandler(this.BCalcularClick);
            // 
            // PremiadasFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(536, 346);
            this.Controls.Add(this.lFileIn);
            this.Controls.Add(this.bFileIn);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbSecuencia);
            this.Controls.Add(this.bAnaliza);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.bGrabar);
            this.Controls.Add(this.lTime);
            this.Controls.Add(this.lProc);
            this.Controls.Add(this.bCalcular);
            this.Controls.Add(this.lbPremis);
            this.Name = "PremiadasFrm";
            this.Text = "Análisis Premiadas";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion
		void BCalcularClick(object sender, System.EventArgs e) { Calcular(); }
		void elmeuTimer(object sender, System.EventArgs e) { veureelmeu(); }
		void BGrabarClick(object sender, System.EventArgs e) { Grabar(); }
		void BAnalizaClick(object sender, System.EventArgs e) { Analiza(); }
		void BFileInClick(object sender, System.EventArgs e) { SelFileIn(); }


		
	}
}
