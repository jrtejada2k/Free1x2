using System;
using System.IO;
using System.Windows.Forms;
using System.Collections;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI
{
	public class TransposicionFrm : Form 
	{
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox tbc13;
		private System.Windows.Forms.TextBox tbc4;
		private System.Windows.Forms.TextBox tbc5;
		private System.Windows.Forms.Button bTransponer;
		private System.Windows.Forms.TextBox tbc7;
		private System.Windows.Forms.TextBox tbc8;
		private System.Windows.Forms.TextBox tbc1;
		private System.Windows.Forms.TextBox tbc2;
		private System.Windows.Forms.TextBox tbc3;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox tbc6;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox tbc9;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbc11;
		private System.Windows.Forms.TextBox tbc10;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox tbc12;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox tbc14;

        private StreamReader sr;
        private StreamWriter sw;
        private BitArray verif = new BitArray(14);
        private int[] orde = new int[14];
        private char[] aux = new char[14];
        private string filein, fileout, columna, tmp;
        private bool AccesoExterno;
		public TransposicionFrm() 
		{
			InitializeComponent();
			AccesoExterno =false;
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
        }

		
		public TransposicionFrm(int[] s, string FileIn, string FileOut) 
		{
			InitializeComponent();
			orde=s;
			tbc1.Text= orde[0].ToString ();
			tbc2.Text= orde[1].ToString ();
			tbc3.Text= orde[2].ToString ();
			tbc4.Text= orde[3].ToString ();
			tbc5.Text= orde[4].ToString ();
			tbc6.Text= orde[5].ToString ();
			tbc7.Text= orde[6].ToString ();
			tbc8.Text= orde[7].ToString ();
			tbc9.Text= orde[8].ToString ();
			tbc10.Text=orde[9].ToString ();
			tbc11.Text=orde[10].ToString();
			tbc12.Text=orde[11].ToString();
			tbc13.Text=orde[12].ToString();
			tbc14.Text=orde[13].ToString();
			filein=FileIn;
			fileout =FileOut;
			AccesoExterno =true;
		}

		
		private void Transponer() 
		{
			bTransponer.Enabled = false;
			if (Verificar()==false) 
			{ 
				MessageBox.Show("Error en condiciones");
				bTransponer.Enabled = true; 
				return; 
			}

			if (SeleccionarFicheros()==false) 
			{
				bTransponer.Enabled = true; 
				return;
			}

			sr = new StreamReader(filein);
			sw = new StreamWriter(fileout);

			bTransponer.Text="Procesando...";
		    while (sr.Peek()>0) 
			{ 
				Application.DoEvents();
				columna = sr.ReadLine();
				for (int nr=0; nr<14; nr++)
				{
				    int nx = orde[nr];
				    aux[nr] = columna[nx];
				}
			    tmp = "";
				for (int nr=0; nr<14; nr++) tmp += aux[nr];
				sw.WriteLine(tmp);
			}
			sr.Close();	sw.Close();
			
			bTransponer.Text="Transponer";
			bTransponer.Enabled = true;
			if (AccesoExterno) Close();
		}
		private bool SeleccionarFicheros()
		{
			bool ok=true;
			if (AccesoExterno) return true;
			
			OpenFileDialog lee = new OpenFileDialog();
			lee.InitialDirectory = ".\\" ;
			lee.Filter = "ColumnasEntrada(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
			if(lee.ShowDialog() == DialogResult.OK) 
			{
				filein = lee.FileName;
				SaveFileDialog grabacols = new SaveFileDialog();
				grabacols.InitialDirectory = ".\\" ;
				grabacols.Filter = "ColumnasSalida(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
				if(grabacols.ShowDialog() == DialogResult.OK) 
				{
					fileout = grabacols.FileName;
				}
				else 
				{ 
					MessageBox.Show("Error en fichero de salida"); 
					ok=false;
				}
			}
			else
			{
				MessageBox.Show("Error en fichero de entrada"); 
				ok=false;
			}
			return ok;
		}
		private bool Verificar() {
			int nx;
			verif.SetAll(false);
			try {
				nx = Convert.ToInt32(tbc1.Text)-1;
				verif[nx] = true; orde[0] = nx;
				nx = Convert.ToInt32(tbc2.Text)-1;
				verif[nx] = true; orde[1] = nx;
				nx = Convert.ToInt32(tbc3.Text)-1;
				verif[nx] = true; orde[2] = nx;
				nx = Convert.ToInt32(tbc4.Text)-1;
				verif[nx] = true; orde[3] = nx;
				nx = Convert.ToInt32(tbc5.Text)-1;
				verif[nx] = true; orde[4] = nx;
				nx = Convert.ToInt32(tbc6.Text)-1;
				verif[nx] = true; orde[5] = nx;
				nx = Convert.ToInt32(tbc7.Text)-1;
				verif[nx] = true; orde[6] = nx;
				nx = Convert.ToInt32(tbc8.Text)-1;
				verif[nx] = true; orde[7] = nx;
				nx = Convert.ToInt32(tbc9.Text)-1;
				verif[nx] = true; orde[8] = nx;
				nx = Convert.ToInt32(tbc10.Text)-1;
				verif[nx] = true; orde[9] = nx;
				nx = Convert.ToInt32(tbc11.Text)-1;
				verif[nx] = true; orde[10] = nx;
				nx = Convert.ToInt32(tbc12.Text)-1;
				verif[nx] = true; orde[11] = nx;
				nx = Convert.ToInt32(tbc13.Text)-1;
				verif[nx] = true; orde[12] = nx;
				nx = Convert.ToInt32(tbc14.Text)-1;
				verif[nx] = true; orde[13] = nx;
			} catch { return false; }
			for (int nr=0; nr<14; nr++) {
				if (verif[nr]==false) return false;
				nx = orde[nr];
				if (nx<0 || nx>13) return false;
			}
			return true;
		}
						
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TransposicionFrm));
            this.tbc14 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tbc12 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.tbc10 = new System.Windows.Forms.TextBox();
            this.tbc11 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.tbc9 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.tbc6 = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.tbc3 = new System.Windows.Forms.TextBox();
            this.tbc2 = new System.Windows.Forms.TextBox();
            this.tbc1 = new System.Windows.Forms.TextBox();
            this.tbc8 = new System.Windows.Forms.TextBox();
            this.tbc7 = new System.Windows.Forms.TextBox();
            this.bTransponer = new System.Windows.Forms.Button();
            this.tbc5 = new System.Windows.Forms.TextBox();
            this.tbc4 = new System.Windows.Forms.TextBox();
            this.tbc13 = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tbc14
            // 
            this.tbc14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbc14.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbc14.Location = new System.Drawing.Point(581, 37);
            this.tbc14.Name = "tbc14";
            this.tbc14.Size = new System.Drawing.Size(32, 21);
            this.tbc14.TabIndex = 31;
            this.tbc14.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label8
            // 
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label8.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(350, 15);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(32, 21);
            this.label8.TabIndex = 8;
            this.label8.Text = "7";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label9.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(317, 15);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(32, 21);
            this.label9.TabIndex = 7;
            this.label9.Text = "6";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(185, 15);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 21);
            this.label4.TabIndex = 3;
            this.label4.Text = "2";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(218, 15);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 21);
            this.label5.TabIndex = 4;
            this.label5.Text = "3";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbc12
            // 
            this.tbc12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbc12.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbc12.Location = new System.Drawing.Point(515, 37);
            this.tbc12.Name = "tbc12";
            this.tbc12.Size = new System.Drawing.Size(32, 21);
            this.tbc12.TabIndex = 29;
            this.tbc12.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label7
            // 
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label7.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(383, 15);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 21);
            this.label7.TabIndex = 9;
            this.label7.Text = "8";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbc10
            // 
            this.tbc10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbc10.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbc10.Location = new System.Drawing.Point(449, 37);
            this.tbc10.Name = "tbc10";
            this.tbc10.Size = new System.Drawing.Size(32, 21);
            this.tbc10.TabIndex = 27;
            this.tbc10.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbc11
            // 
            this.tbc11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbc11.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbc11.Location = new System.Drawing.Point(482, 37);
            this.tbc11.Name = "tbc11";
            this.tbc11.Size = new System.Drawing.Size(32, 21);
            this.tbc11.TabIndex = 28;
            this.tbc11.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(8, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(144, 21);
            this.label2.TabIndex = 1;
            this.label2.Text = "Debe ser la salida: ";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(152, 15);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 21);
            this.label3.TabIndex = 2;
            this.label3.Text = "1";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label15
            // 
            this.label15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label15.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(548, 15);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(32, 21);
            this.label15.TabIndex = 17;
            this.label15.Text = "13";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label14
            // 
            this.label14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label14.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(416, 15);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(32, 21);
            this.label14.TabIndex = 10;
            this.label14.Text = "9";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbc9
            // 
            this.tbc9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbc9.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbc9.Location = new System.Drawing.Point(416, 37);
            this.tbc9.Name = "tbc9";
            this.tbc9.Size = new System.Drawing.Size(32, 21);
            this.tbc9.TabIndex = 26;
            this.tbc9.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label6
            // 
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(251, 15);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 21);
            this.label6.TabIndex = 5;
            this.label6.Text = "4";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            this.label11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label11.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(482, 15);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(32, 21);
            this.label11.TabIndex = 13;
            this.label11.Text = "11";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbc6
            // 
            this.tbc6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbc6.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbc6.Location = new System.Drawing.Point(317, 37);
            this.tbc6.Name = "tbc6";
            this.tbc6.Size = new System.Drawing.Size(32, 21);
            this.tbc6.TabIndex = 23;
            this.tbc6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(144, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "La entrada: ";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label12
            // 
            this.label12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label12.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(515, 15);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(32, 21);
            this.label12.TabIndex = 12;
            this.label12.Text = "12";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbc3
            // 
            this.tbc3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbc3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbc3.Location = new System.Drawing.Point(218, 37);
            this.tbc3.Name = "tbc3";
            this.tbc3.Size = new System.Drawing.Size(32, 21);
            this.tbc3.TabIndex = 20;
            this.tbc3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbc2
            // 
            this.tbc2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbc2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbc2.Location = new System.Drawing.Point(185, 37);
            this.tbc2.Name = "tbc2";
            this.tbc2.Size = new System.Drawing.Size(32, 21);
            this.tbc2.TabIndex = 19;
            this.tbc2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbc1
            // 
            this.tbc1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbc1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbc1.Location = new System.Drawing.Point(152, 37);
            this.tbc1.Name = "tbc1";
            this.tbc1.Size = new System.Drawing.Size(32, 21);
            this.tbc1.TabIndex = 18;
            this.tbc1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbc8
            // 
            this.tbc8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbc8.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbc8.Location = new System.Drawing.Point(383, 37);
            this.tbc8.Name = "tbc8";
            this.tbc8.Size = new System.Drawing.Size(32, 21);
            this.tbc8.TabIndex = 25;
            this.tbc8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbc7
            // 
            this.tbc7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbc7.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbc7.Location = new System.Drawing.Point(350, 37);
            this.tbc7.Name = "tbc7";
            this.tbc7.Size = new System.Drawing.Size(32, 21);
            this.tbc7.TabIndex = 24;
            this.tbc7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bTransponer
            // 
            this.bTransponer.BackColor = System.Drawing.Color.DarkSalmon;
            this.bTransponer.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bTransponer.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bTransponer.Image = ((System.Drawing.Image)(resources.GetObject("bTransponer.Image")));
            this.bTransponer.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bTransponer.Location = new System.Drawing.Point(272, 80);
            this.bTransponer.Name = "bTransponer";
            this.bTransponer.Size = new System.Drawing.Size(128, 32);
            this.bTransponer.TabIndex = 34;
            this.bTransponer.Text = "Transponer";
            this.bTransponer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bTransponer.UseVisualStyleBackColor = false;
            this.bTransponer.Click += new System.EventHandler(this.BTransponerClick);
            // 
            // tbc5
            // 
            this.tbc5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbc5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbc5.Location = new System.Drawing.Point(284, 37);
            this.tbc5.Name = "tbc5";
            this.tbc5.Size = new System.Drawing.Size(32, 21);
            this.tbc5.TabIndex = 22;
            this.tbc5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbc4
            // 
            this.tbc4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbc4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbc4.Location = new System.Drawing.Point(251, 37);
            this.tbc4.Name = "tbc4";
            this.tbc4.Size = new System.Drawing.Size(32, 21);
            this.tbc4.TabIndex = 21;
            this.tbc4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbc13
            // 
            this.tbc13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbc13.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbc13.Location = new System.Drawing.Point(548, 37);
            this.tbc13.Name = "tbc13";
            this.tbc13.Size = new System.Drawing.Size(32, 21);
            this.tbc13.TabIndex = 30;
            this.tbc13.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label10
            // 
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label10.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(284, 15);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(32, 21);
            this.label10.TabIndex = 6;
            this.label10.Text = "5";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label16
            // 
            this.label16.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label16.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(581, 15);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(32, 21);
            this.label16.TabIndex = 16;
            this.label16.Text = "14";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label13
            // 
            this.label13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label13.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(449, 15);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(32, 21);
            this.label13.TabIndex = 11;
            this.label13.Text = "10";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TransposicionFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(624, 134);
            this.Controls.Add(this.bTransponer);
            this.Controls.Add(this.tbc14);
            this.Controls.Add(this.tbc13);
            this.Controls.Add(this.tbc12);
            this.Controls.Add(this.tbc11);
            this.Controls.Add(this.tbc10);
            this.Controls.Add(this.tbc9);
            this.Controls.Add(this.tbc8);
            this.Controls.Add(this.tbc7);
            this.Controls.Add(this.tbc6);
            this.Controls.Add(this.tbc5);
            this.Controls.Add(this.tbc4);
            this.Controls.Add(this.tbc3);
            this.Controls.Add(this.tbc2);
            this.Controls.Add(this.tbc1);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "TransposicionFrm";
            this.Text = "Transposición de columnas";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion
		
		void BTransponerClick(object sender, EventArgs e) { Transponer(); }


	}
}
