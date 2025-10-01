
using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;

namespace Free1X2.UI
{
	public class MultiplicadorFrm : System.Windows.Forms.Form {
		private System.Windows.Forms.TextBox tbcol11;
		private System.Windows.Forms.TextBox tbcol02;
		private System.Windows.Forms.TextBox tbcol14;
		private System.Windows.Forms.TextBox tbcol01;
		private System.Windows.Forms.Label lcols2;
		private System.Windows.Forms.TextBox tbcol08;
		private System.Windows.Forms.Button bEntra1;
		private System.Windows.Forms.Label lcolsresul;
		private System.Windows.Forms.Button bMultiplica;
		private System.Windows.Forms.TextBox tbcol12;
		private System.Windows.Forms.TextBox tbcol13;
		private System.Windows.Forms.TextBox tbcol03;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox tbcol07;
		private System.Windows.Forms.TextBox tbcol06;
		private System.Windows.Forms.TextBox tbcol05;
		private System.Windows.Forms.TextBox tbcol04;
		private System.Windows.Forms.TextBox tbcol09;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Button bEntra2;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label lcols1;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Button bGrabar;
		private System.Windows.Forms.TextBox tbcol10;
		public MultiplicadorFrm()
		{
			InitializeComponent();
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}
		
		private StreamReader sr = null;
		private StreamWriter sw = null;
		private string filein, fileout, scol1, scol2, scol3;
		private string[] ascols1 = new string[1594323];
		private string[] ascols2 = new string[1594323];
		private string[] ascols3 = new string[4782969];
		private int ncols1, ncols2, ncols3, idx;
		private int[] indices = new int[14];
		private char[] aux = new char[14];
		
		private void Entrada1() {
			bEntra1.Enabled = false;
			bEntra2.Enabled = false;
			bMultiplica.Enabled = false;
			OpenFileDialog lee = new OpenFileDialog();
			lee.InitialDirectory = ".\\" ;
			lee.Filter = "ColumnasEntrada-1(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
			if(lee.ShowDialog() == DialogResult.OK) {
				filein = Path.GetFileName(lee.FileName);
				sr = new StreamReader(lee.FileName);
				ncols1=0;
				while (sr.Peek()>0) {
					Application.DoEvents();
					scol1 = sr.ReadLine()+"11111111111111";
					scol1 = scol1.Substring(0,14);
					ascols1[ncols1] = scol1.Trim();
					ncols1++;
				}
				sr.Close();
				lcols1.Text = ""+ncols1;
			}
			bMultiplica.Enabled = true;
			bEntra2.Enabled = true;
			bEntra1.Enabled = true;
		}
		private void Entrada2() {
			bEntra2.Enabled = false;
			bEntra1.Enabled = false;
			bMultiplica.Enabled = false;
			OpenFileDialog lee = new OpenFileDialog();
			lee.InitialDirectory = ".\\" ;
			lee.Filter = "ColumnasEntrada-2(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
			if(lee.ShowDialog() == DialogResult.OK) {
				filein = Path.GetFileName(lee.FileName);
				sr = new StreamReader(lee.FileName);
				ncols2=0;
				while (sr.Peek()>0) {
					Application.DoEvents();
					scol2 = sr.ReadLine()+"11111111111111";
					scol2 = scol2.Substring(0,14);
					ascols2[ncols2] = scol2.Trim();
					ncols2++;
				}
				sr.Close();
				lcols2.Text = ""+ncols2;
			}
			bMultiplica.Enabled = true;
			bEntra1.Enabled = true;
			bEntra2.Enabled = true;
		}
		private void Multiplicar() {
			bEntra2.Enabled = false;
			bEntra1.Enabled = false;
			bMultiplica.Enabled = false;
			if (!RecuperaPantalla()) MessageBox.Show("error en plantilla");
			ncols3 = 0;
			for (int nr1=0; nr1<ncols1; nr1++) {
				scol1 = ascols1[nr1];
				for (int nr2=0; nr2<ncols2; nr2++) {
					scol2 = ascols2[nr2];
					scol3 = scol1+scol2;
					for (int nr3=0; nr3<14; nr3++) {
						idx = indices[nr3]-1;
						aux[nr3] = scol3[idx];
					}
					scol3="";
					for (int nr3=0; nr3<14; nr3++) scol3+=aux[nr3];
					ascols3[ncols3]= scol3; ncols3++;
				}
			}
			lcolsresul.Text	= ""+ncols3;
			bMultiplica.Enabled = true;
			bEntra1.Enabled = true;
			bEntra2.Enabled = true;
		}
		private void Grabar() {
			bMultiplica.Enabled = false;
			bGrabar.Enabled = false;
			SaveFileDialog resul = new SaveFileDialog();
			resul.InitialDirectory = ".\\" ;
			resul.Filter = "Fichero resultados(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
			if(resul.ShowDialog() == DialogResult.OK) {
				fileout = Path.GetFileName(resul.FileName);
				sw = new StreamWriter(fileout);
				for (int nr=0; nr<ncols3; nr++) sw.WriteLine(ascols3[nr]);
				sw.Close();
			}
			bGrabar.Enabled = true;
			bMultiplica.Enabled = true;
		}
		private bool RecuperaPantalla() {
			indices[0]=Convert.ToInt32(tbcol01.Text);
			indices[1]=Convert.ToInt32(tbcol02.Text);
			indices[2]=Convert.ToInt32(tbcol03.Text);
			indices[3]=Convert.ToInt32(tbcol04.Text);
			indices[4]=Convert.ToInt32(tbcol05.Text);
			indices[5]=Convert.ToInt32(tbcol06.Text);
			indices[6]=Convert.ToInt32(tbcol07.Text);
			indices[7]=Convert.ToInt32(tbcol08.Text);
			indices[8]=Convert.ToInt32(tbcol09.Text);
			indices[9]=Convert.ToInt32(tbcol10.Text);
			indices[10]=Convert.ToInt32(tbcol11.Text);
			indices[11]=Convert.ToInt32(tbcol12.Text);
			indices[12]=Convert.ToInt32(tbcol13.Text);
			indices[13]=Convert.ToInt32(tbcol14.Text);
			for (int nr=0; nr<14; nr++) if (indices[nr]<1 || indices[nr]>28) return false;
			return true;				            
		}
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MultiplicadorFrm));
            this.tbcol10 = new System.Windows.Forms.TextBox();
            this.bGrabar = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.lcols1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.bEntra2 = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.tbcol09 = new System.Windows.Forms.TextBox();
            this.tbcol04 = new System.Windows.Forms.TextBox();
            this.tbcol05 = new System.Windows.Forms.TextBox();
            this.tbcol06 = new System.Windows.Forms.TextBox();
            this.tbcol07 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tbcol03 = new System.Windows.Forms.TextBox();
            this.tbcol13 = new System.Windows.Forms.TextBox();
            this.tbcol12 = new System.Windows.Forms.TextBox();
            this.bMultiplica = new System.Windows.Forms.Button();
            this.lcolsresul = new System.Windows.Forms.Label();
            this.bEntra1 = new System.Windows.Forms.Button();
            this.tbcol08 = new System.Windows.Forms.TextBox();
            this.lcols2 = new System.Windows.Forms.Label();
            this.tbcol01 = new System.Windows.Forms.TextBox();
            this.tbcol14 = new System.Windows.Forms.TextBox();
            this.tbcol02 = new System.Windows.Forms.TextBox();
            this.tbcol11 = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tbcol10
            // 
            this.tbcol10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol10.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol10.Location = new System.Drawing.Point(325, 195);
            this.tbcol10.Name = "tbcol10";
            this.tbcol10.Size = new System.Drawing.Size(32, 21);
            this.tbcol10.TabIndex = 13;
            this.tbcol10.Text = "19";
            this.tbcol10.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bGrabar
            // 
            this.bGrabar.BackColor = System.Drawing.Color.DarkSalmon;
            this.bGrabar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bGrabar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bGrabar.Image = ((System.Drawing.Image)(resources.GetObject("bGrabar.Image")));
            this.bGrabar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bGrabar.Location = new System.Drawing.Point(368, 264);
            this.bGrabar.Name = "bGrabar";
            this.bGrabar.Size = new System.Drawing.Size(132, 32);
            this.bGrabar.TabIndex = 29;
            this.bGrabar.Text = "Grabar resultado";
            this.bGrabar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bGrabar.UseVisualStyleBackColor = false;
            this.bGrabar.Click += new System.EventHandler(this.BGrabaClick);
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(14, 232);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(482, 24);
            this.label8.TabIndex = 26;
            this.label8.Text = "El cambio de orden de estas cifras permite transponer columnas al mismo tiempo";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lcols1
            // 
            this.lcols1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lcols1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lcols1.Location = new System.Drawing.Point(16, 49);
            this.lcols1.Name = "lcols1";
            this.lcols1.Size = new System.Drawing.Size(72, 24);
            this.lcols1.TabIndex = 22;
            this.lcols1.Text = "0";
            this.lcols1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(152, 16);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(344, 32);
            this.label4.TabIndex = 21;
            this.label4.Text = "Columnas numeradas de 1 a 14";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(152, 88);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(344, 32);
            this.label5.TabIndex = 23;
            this.label5.Text = "Columnas numeradas de 15 a 28";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // bEntra2
            // 
            this.bEntra2.BackColor = System.Drawing.Color.DarkSalmon;
            this.bEntra2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bEntra2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bEntra2.Image = ((System.Drawing.Image)(resources.GetObject("bEntra2.Image")));
            this.bEntra2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bEntra2.Location = new System.Drawing.Point(16, 88);
            this.bEntra2.Name = "bEntra2";
            this.bEntra2.Size = new System.Drawing.Size(128, 32);
            this.bEntra2.TabIndex = 2;
            this.bEntra2.Text = "Entrada Comb-2";
            this.bEntra2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bEntra2.UseVisualStyleBackColor = false;
            this.bEntra2.Click += new System.EventHandler(this.BEntra2Click);
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(232, 264);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(136, 32);
            this.label7.TabIndex = 28;
            this.label7.Text = "Columnas resultantes";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbcol09
            // 
            this.tbcol09.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol09.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol09.Location = new System.Drawing.Point(292, 195);
            this.tbcol09.Name = "tbcol09";
            this.tbcol09.Size = new System.Drawing.Size(32, 21);
            this.tbcol09.TabIndex = 12;
            this.tbcol09.Text = "18";
            this.tbcol09.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbcol04
            // 
            this.tbcol04.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol04.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol04.Location = new System.Drawing.Point(113, 195);
            this.tbcol04.Name = "tbcol04";
            this.tbcol04.Size = new System.Drawing.Size(32, 21);
            this.tbcol04.TabIndex = 7;
            this.tbcol04.Text = "4";
            this.tbcol04.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbcol05
            // 
            this.tbcol05.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol05.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol05.Location = new System.Drawing.Point(153, 195);
            this.tbcol05.Name = "tbcol05";
            this.tbcol05.Size = new System.Drawing.Size(32, 21);
            this.tbcol05.TabIndex = 8;
            this.tbcol05.Text = "5";
            this.tbcol05.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbcol06
            // 
            this.tbcol06.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol06.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol06.Location = new System.Drawing.Point(186, 195);
            this.tbcol06.Name = "tbcol06";
            this.tbcol06.Size = new System.Drawing.Size(32, 21);
            this.tbcol06.TabIndex = 9;
            this.tbcol06.Text = "15";
            this.tbcol06.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbcol07
            // 
            this.tbcol07.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol07.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol07.Location = new System.Drawing.Point(219, 195);
            this.tbcol07.Name = "tbcol07";
            this.tbcol07.Size = new System.Drawing.Size(32, 21);
            this.tbcol07.TabIndex = 10;
            this.tbcol07.Text = "16";
            this.tbcol07.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(24, 168);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(472, 24);
            this.label6.TabIndex = 25;
            this.label6.Text = "Plantilla de estructura de las columnas resultantes (cifras de ejemplo)";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbcol03
            // 
            this.tbcol03.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol03.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol03.Location = new System.Drawing.Point(80, 195);
            this.tbcol03.Name = "tbcol03";
            this.tbcol03.Size = new System.Drawing.Size(32, 21);
            this.tbcol03.TabIndex = 6;
            this.tbcol03.Text = "3";
            this.tbcol03.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbcol13
            // 
            this.tbcol13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol13.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol13.Location = new System.Drawing.Point(431, 195);
            this.tbcol13.Name = "tbcol13";
            this.tbcol13.Size = new System.Drawing.Size(32, 21);
            this.tbcol13.TabIndex = 17;
            this.tbcol13.Text = "13";
            this.tbcol13.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbcol12
            // 
            this.tbcol12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol12.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol12.Location = new System.Drawing.Point(398, 195);
            this.tbcol12.Name = "tbcol12";
            this.tbcol12.Size = new System.Drawing.Size(32, 21);
            this.tbcol12.TabIndex = 16;
            this.tbcol12.Text = "21";
            this.tbcol12.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bMultiplica
            // 
            this.bMultiplica.BackColor = System.Drawing.Color.DarkSalmon;
            this.bMultiplica.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bMultiplica.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bMultiplica.Image = ((System.Drawing.Image)(resources.GetObject("bMultiplica.Image")));
            this.bMultiplica.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bMultiplica.Location = new System.Drawing.Point(16, 264);
            this.bMultiplica.Name = "bMultiplica";
            this.bMultiplica.Size = new System.Drawing.Size(128, 32);
            this.bMultiplica.TabIndex = 20;
            this.bMultiplica.Text = "Multiplicar";
            this.bMultiplica.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bMultiplica.UseVisualStyleBackColor = false;
            this.bMultiplica.Click += new System.EventHandler(this.BMultiplicaClick);
            // 
            // lcolsresul
            // 
            this.lcolsresul.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lcolsresul.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lcolsresul.Location = new System.Drawing.Point(152, 272);
            this.lcolsresul.Name = "lcolsresul";
            this.lcolsresul.Size = new System.Drawing.Size(72, 24);
            this.lcolsresul.TabIndex = 27;
            this.lcolsresul.Text = "0";
            this.lcolsresul.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bEntra1
            // 
            this.bEntra1.BackColor = System.Drawing.Color.DarkSalmon;
            this.bEntra1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bEntra1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bEntra1.Image = ((System.Drawing.Image)(resources.GetObject("bEntra1.Image")));
            this.bEntra1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bEntra1.Location = new System.Drawing.Point(16, 16);
            this.bEntra1.Name = "bEntra1";
            this.bEntra1.Size = new System.Drawing.Size(128, 32);
            this.bEntra1.TabIndex = 1;
            this.bEntra1.Text = "Entrada Comb-1";
            this.bEntra1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bEntra1.UseVisualStyleBackColor = false;
            this.bEntra1.Click += new System.EventHandler(this.BEntra1Click);
            // 
            // tbcol08
            // 
            this.tbcol08.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol08.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol08.Location = new System.Drawing.Point(252, 195);
            this.tbcol08.Name = "tbcol08";
            this.tbcol08.Size = new System.Drawing.Size(32, 21);
            this.tbcol08.TabIndex = 11;
            this.tbcol08.Text = "17";
            this.tbcol08.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lcols2
            // 
            this.lcols2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lcols2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lcols2.Location = new System.Drawing.Point(16, 121);
            this.lcols2.Name = "lcols2";
            this.lcols2.Size = new System.Drawing.Size(72, 24);
            this.lcols2.TabIndex = 24;
            this.lcols2.Text = "0";
            this.lcols2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbcol01
            // 
            this.tbcol01.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol01.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol01.Location = new System.Drawing.Point(14, 195);
            this.tbcol01.Name = "tbcol01";
            this.tbcol01.Size = new System.Drawing.Size(32, 21);
            this.tbcol01.TabIndex = 4;
            this.tbcol01.Text = "1";
            this.tbcol01.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbcol14
            // 
            this.tbcol14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol14.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol14.Location = new System.Drawing.Point(464, 195);
            this.tbcol14.Name = "tbcol14";
            this.tbcol14.Size = new System.Drawing.Size(32, 21);
            this.tbcol14.TabIndex = 18;
            this.tbcol14.Text = "13";
            this.tbcol14.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbcol02
            // 
            this.tbcol02.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol02.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol02.Location = new System.Drawing.Point(47, 195);
            this.tbcol02.Name = "tbcol02";
            this.tbcol02.Size = new System.Drawing.Size(32, 21);
            this.tbcol02.TabIndex = 5;
            this.tbcol02.Text = "2";
            this.tbcol02.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbcol11
            // 
            this.tbcol11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbcol11.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbcol11.Location = new System.Drawing.Point(358, 195);
            this.tbcol11.Name = "tbcol11";
            this.tbcol11.Size = new System.Drawing.Size(32, 21);
            this.tbcol11.TabIndex = 14;
            this.tbcol11.Text = "20";
            this.tbcol11.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // MultiplicadorFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(512, 318);
            this.Controls.Add(this.bGrabar);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.bMultiplica);
            this.Controls.Add(this.lcolsresul);
            this.Controls.Add(this.tbcol14);
            this.Controls.Add(this.tbcol13);
            this.Controls.Add(this.tbcol12);
            this.Controls.Add(this.tbcol11);
            this.Controls.Add(this.tbcol10);
            this.Controls.Add(this.tbcol09);
            this.Controls.Add(this.tbcol08);
            this.Controls.Add(this.tbcol07);
            this.Controls.Add(this.tbcol06);
            this.Controls.Add(this.tbcol05);
            this.Controls.Add(this.tbcol04);
            this.Controls.Add(this.tbcol03);
            this.Controls.Add(this.tbcol02);
            this.Controls.Add(this.bEntra2);
            this.Controls.Add(this.lcols2);
            this.Controls.Add(this.tbcol01);
            this.Controls.Add(this.bEntra1);
            this.Controls.Add(this.lcols1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MultiplicadorFrm";
            this.Text = "Multiplicador";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion
		
		void BEntra1Click(object sender, System.EventArgs e) { Entrada1(); }
		void BEntra2Click(object sender, System.EventArgs e) { Entrada2(); }
		void BMultiplicaClick(object sender, System.EventArgs e) { Multiplicar(); }
		void BGrabaClick(object sender, System.EventArgs e) { Grabar(); }
		
	}
}
