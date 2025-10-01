using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using Free1X2.EntradaSalida;

namespace Free1X2.UI {
    public class VSignosFrm : Form
    {
		private TextBox tb03;
		private TextBox tb02;
        private TextBox tb01;
		private TextBox tb07;
		private TextBox tb06;
        private TextBox tb05;
		private TextBox tb09;
		private TextBox tb08;
        private Label ltime;
        private GroupBox groupBox1;
        private TextBox tb12;
		private TextBox tb10;
        private TextBox tb11;
        private TextBox tb15;
        private Label label2;
        private Label label18;
		private Button bFileIn;
        private RadioButton rbcen0;
        private RadioButton rbcen2;
		private TextBox tb13;
		private Button bGrabar;
        private TextBox tb14;
		private Button bMas;
        private Label lproc;
        private Button bMenos;
        private Label lp100viu;
		private Button bCalcular;
        private TextBox tb04;
        private Label label17;
		private Label lbasp;
		private Label lFileIn;
        private RadioButton rbcol;
        private TextBox tb16;
        private Panel panel1;
        private int noPartidos = VariablesGlobales.NumeroPartidos;
        private string[] signos = new string[]{"1", "X", "2"};
        private TextBox[] txts = null;
        private CheckBox chkPleno;
        private Button bLimp;
		
		public VSignosFrm() 		{
			InitializeComponent();
			elmeu = new Timer();
			elmeu.Interval = 3000;
			elmeu.Tick += elmeuTimer;
            txts = new TextBox[] { tb01, tb02, tb03, tb04, tb05, tb06, tb07, tb08, tb09, tb10, tb11, tb12, tb13, tb14, tb15, tb16 };
            AdaptarVistaAPartidos();
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}

        private void AñadirControles()
        {
            panel1.Controls.Clear();

            int x = 10;
            int y = 5;

            for (int i = 1; i <= noPartidos; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Label lbl = new Label();
                    lbl.Name = "p_" + i.ToString("00") + "_" + signos[j].ToLower();
                    lbl.Text = "-";           
                    lbl.Size = new Size(70, 20);
                    lbl.Location = new Point(x, y);
                    lbl.BorderStyle = BorderStyle.FixedSingle;
                    lbl.TextAlign = ContentAlignment.MiddleCenter;
                    lbl.BackColor = Color.LightGoldenrodYellow;
                    lbl.Click +=lbl_Click;

                    panel1.Controls.Add(lbl);

                    x += lbl.Width + 1;
                }

                x = 10;
                y += 21;
            }
        }

        private void AñadirControles(double[,] valores)
        {
            panel1.Controls.Clear();

            int x = 10;
            int y = 5;
            int p = 1;
            for (int i = 0; i < noPartidos; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    Label lbl = new Label();
                    lbl.Name = "p_" + p.ToString("00") + "_" + signos[j].ToLower();
                    lbl.Text = valores[i, j].ToString();
                    lbl.Size = new Size(70, 20);
                    lbl.Location = new Point(x, y);
                    lbl.BorderStyle = BorderStyle.FixedSingle;
                    lbl.TextAlign = ContentAlignment.MiddleCenter;
                    lbl.BackColor = Color.LightGoldenrodYellow;
                    lbl.Click += lbl_Click;
                    panel1.Controls.Add(lbl);

                    x += lbl.Width + 1;
                }
                p++;
                x = 10;
                y += 21;
            }
        }


        private void AdaptarVistaAPartidos()
        {
            //Lo más intuitivo es que si hay 15 partidos se contemple el pleno
            if (noPartidos >= 15)
            {
                chkPleno.Checked = true;
            }
            else
            {
                chkPleno.Checked = false;
            }

            for (int i = 0; i < txts.Length; i++)
            {
                txts[i].Visible = false;
            }
            lbasp.Text = noPartidos.ToString();
            AñadirControles();

            for (int i = 0; i < noPartidos; i++)
            {
                txts[i].Visible = true;
            }
        }
		
		private string columna, filein, archivo="";
		private int ctproc, limac;
		private int[,] vals = new int[15,3];
		private char[] part = new char[14];
		private int[] premis = new int[5];
		private DateTime dt0, dt9;
		private Timer elmeu;
		private ArrayList aceptadas = new ArrayList();

        private void AdaptarVariables()
        {
            vals = new int[noPartidos, 3];
            part = new char[noPartidos];
            premis = new int[noPartidos - 9];
        }
		private void Grabar() {
		    double[,] vals2 = new double[noPartidos,3];
			string asp;
			bCalcular.Enabled = false;
			bGrabar.Enabled = false;
			SaveFileDialog resul = new SaveFileDialog();
			resul.InitialDirectory = ".\\" ;
			if (rbcen0.Checked || rbcen2.Checked)
				resul.Filter = "Valoración(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
			else
				resul.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
			if(resul.ShowDialog() == DialogResult.OK) {
				Application.DoEvents();
				string fileout = Path.GetFileName(resul.FileName);
				StreamWriter sw = new StreamWriter(fileout);
				for (int nr=0; nr<vals.GetLength(0); nr++) {
					double sum = (vals[nr,0]+vals[nr,1]+vals[nr,2]);
					if (sum==0) sum=1;
					vals2[nr,0] = ((vals[nr,0]/sum)*1e2);
					vals2[nr,1] = ((vals[nr,1]/sum)*1e2);
					vals2[nr,2] = ((vals[nr,2]/sum)*1e2);
				}
				if (rbcen0.Checked) {
					asp="{0:f0}, {1:f0}, {2:f0}";
                    for (int nr = 0; nr < vals2.GetLength(0); nr++)
                    {
						sw.WriteLine(asp,vals2[nr,0],vals2[nr,1],vals2[nr,2]);
					}
				}
				else if (rbcen2.Checked) {
					asp="{0:f2}"+((char)9)+"{1:f2}"+((char)9)+"{2:f2}";
                    for (int nr = 0; nr < vals2.GetLength(0); nr++)
                    {
						sw.WriteLine(asp,vals2[nr,0],vals2[nr,1],vals2[nr,2]);
					}
				}
				else {
					foreach (string col in aceptadas) sw.WriteLine(col);
				}
				sw.Close();
			}
			bGrabar.Enabled = true;
			bCalcular.Enabled = true;
		}
		private void EntradaFichero() {
			OpenFileDialog abreFileIn = new OpenFileDialog();
			abreFileIn.InitialDirectory = Application.StartupPath + "/";
			abreFileIn.Filter = "F.Entrada(*.txt)|*.txt|Todos los archivos(*.*)|*.*" ;
			if(abreFileIn.ShowDialog() == DialogResult.OK) {
                archivo = abreFileIn.FileName;
				filein = Path.GetFileName(archivo);
				lFileIn.Text = filein;
			}

            IArchivoColumnas aCol = new ArchivoColumnasTexto(abreFileIn.FileName);
            noPartidos = aCol.ObtenNumSignos();
            aCol.Cerrar();

            AdaptarVistaAPartidos();
            AdaptarVariables();

			Calcular();
		}
		private void Calcular() {
		    bCalcular.Enabled = false;
			elmeu.Start(); dt0 = DateTime.Now;
			LimpiaPantalla();
            for (int nr = 0; nr < vals.GetLength(0); nr++)
            {
                vals[nr, 0] = vals[nr, 1] = vals[nr, 2] = 0;
            }
            for (int i = 0; i < premis.Length; i++)
            {
                premis[i] = 0;
            }
			ctproc=0; aceptadas.Clear();
			RecuperaPantalla();
			ltime.Text=lproc.Text = " ";
            if (archivo != "")
            {
				StreamReader sr = new StreamReader(archivo);
				while (sr.Peek()>0)  {
					Application.DoEvents();
					columna = sr.ReadLine().Trim(); ctproc++;

                    columna = columna.ToUpper();

                    int aciertos = AcCB();
					if (aciertos>=limac) Contabiliza();
                    if (aciertos > 9 && aciertos <= noPartidos) premis[noPartidos - aciertos]++;
				}
				sr.Close();
				PintaPantalla();
			}
			elmeu.Stop();
			veureelmeu();
			bCalcular.Enabled = true;
		}
		private int AcCB() {
			int rsl = 0;
			char ch;
            for (int nr = 0; nr < noPartidos - 1; nr++)
            {
                ch = part[nr];
                if (ch == columna[nr]) rsl++;
                else if (ch == '1' || ch == 'X' || ch == '2') { }
                else rsl++;
            }
            if (chkPleno.Checked)
            {
                ch = part[part.Length - 1];
                if (rsl == noPartidos - 1)
                {
                    if (ch == columna[columna.Length - 1])
                    {
                        rsl++;
                    }
                    else if (ch == '1' || ch == 'X' || ch == '2')
                    {
                    }
                    else
                    {
                        rsl++;
                    }
                }
            }
            else
            {
                //Ignora el pleno
                ch = part[part.Length - 1];

                if (ch == columna[columna.Length - 1])
                {
                    rsl++;
                }
                else if (ch == '1' || ch == 'X' || ch == '2')
                {
                    
                }
                else
                {
                    rsl++;
                }
            }
			return rsl;
		}
		private void Contabiliza() {
            for (int nr = 0; nr < noPartidos; nr++)
            {
                char ch = columna[nr];
                if (ch == '1') vals[nr, 0]++;
                else if (ch == '2') vals[nr, 2]++;
                else vals[nr, 1]++;
            }
			aceptadas.Add(columna);
		}
		private void veureelmeu() {
			dt9 = DateTime.Now;
			string temp = (dt9-dt0)+"0000000000";
			ltime.Text = temp.Substring(0,10);
			lproc.Text = ""+ctproc;
		}
		private void PintaPantalla() {
			double sum;
            double[,] vals2 = new double[vals.GetLength(0), 3];
            
            if (rbcen0.Checked)
            {
                for (int nr = 0; nr < vals.GetLength(0); nr++)
                {
                    sum = (vals[nr, 0] + vals[nr, 1] + vals[nr, 2]);
                    if (sum == 0) sum = 1;
                    vals2[nr, 0] = Convert.ToDouble(String.Format("{0:f0}", (vals[nr, 0] / sum) * 1e2));
                    vals2[nr, 1] = Convert.ToDouble(String.Format("{0:f0}", (vals[nr, 1] / sum) * 1e2));
                    vals2[nr, 2] = Convert.ToDouble(String.Format("{0:f0}", (vals[nr, 2] / sum) * 1e2));
                }
            }
            else if (rbcen2.Checked)
            {
                for (int nr = 0; nr < vals.GetLength(0); nr++)
                {
                    sum = (vals[nr, 0] + vals[nr, 1] + vals[nr, 2]);
                    if (sum == 0) sum = 1;
                    vals2[nr, 0] = Convert.ToDouble(String.Format("{0:f2}", (vals[nr, 0] / sum) * 1e2));
                    vals2[nr, 1] = Convert.ToDouble(String.Format("{0:f2}", (vals[nr, 1] / sum) * 1e2));
                    vals2[nr, 2] = Convert.ToDouble(String.Format("{0:f2}", (vals[nr, 2] / sum) * 1e2));
                }
            }
            else
            {
                for (int nr = 0; nr < vals.GetLength(0); nr++)
                {
                    vals2[nr, 0] = vals[nr, 0];
                    vals2[nr, 1] = vals[nr, 1];
                    vals2[nr, 2] = vals[nr, 2];
                }
            }
		
            lp100viu.Text = "";

            for (int i = 0; i < premis.Length; i++)
            {
                lp100viu.Text += premis[i];
                if (i < premis.Length - 1)
                {
                    lp100viu.Text += "-";
                }
            }
            AñadirControles(vals2);
        }
        private void ReinicializaColumnaGanadora()
        {
            for (int i = 0; i < txts.Length; i++)
            {
                txts[i].Text = "-";
            }
        }
        private void RecuperaPantalla()
        {
            for (int i = 0; i < part.Length; i++)
            {
                switch (txts[i].Text)
                {
                    case "":
                        part[i] = '-';
                        break;
                    default:
                        part[i] = txts[i].Text[0];
                        break;
                }
            }
			limac = lbasp.Text=="" ? noPartidos : Convert.ToInt32(lbasp.Text);
		}
        private void LimpiaPantalla()
        {
            AñadirControles();
			lp100viu.Text = "";
		}
		private void AspMas() {
			int naux = Convert.ToInt32(lbasp.Text);
			if (naux<noPartidos ) lbasp.Text=""+(naux+1);
		}
		private void AspMenos() {
			int naux = Convert.ToInt32(lbasp.Text);
			if (naux>0) lbasp.Text=""+(naux-1);
		}
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VSignosFrm));
            bLimp = new Button();
            rbcol = new RadioButton();
            lFileIn = new Label();
            lbasp = new Label();
            label17 = new Label();
            tb04 = new TextBox();
            bCalcular = new Button();
            lp100viu = new Label();
            bMenos = new Button();
            lproc = new Label();
            bMas = new Button();
            tb14 = new TextBox();
            bGrabar = new Button();
            tb13 = new TextBox();
            rbcen2 = new RadioButton();
            rbcen0 = new RadioButton();
            bFileIn = new Button();
            label18 = new Label();
            label2 = new Label();
            tb15 = new TextBox();
            tb11 = new TextBox();
            tb10 = new TextBox();
            tb12 = new TextBox();
            groupBox1 = new GroupBox();
            ltime = new Label();
            tb08 = new TextBox();
            tb09 = new TextBox();
            tb05 = new TextBox();
            tb06 = new TextBox();
            tb07 = new TextBox();
            tb01 = new TextBox();
            tb02 = new TextBox();
            tb03 = new TextBox();
            tb16 = new TextBox();
            panel1 = new Panel();
            chkPleno = new CheckBox();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // bLimp
            // 
            bLimp.BackColor = Color.DarkSalmon;
            bLimp.FlatStyle = FlatStyle.Popup;
            bLimp.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            bLimp.Location = new Point(240, 14);
            bLimp.Name = "bLimp";
            bLimp.Size = new Size(32, 22);
            bLimp.TabIndex = 99;
            bLimp.Text = "R";
            bLimp.UseVisualStyleBackColor = false;
            bLimp.Click += BLimpClick;
            // 
            // rbcol
            // 
            rbcol.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            rbcol.ForeColor = Color.Black;
            rbcol.Location = new Point(16, 58);
            rbcol.Name = "rbcol";
            rbcol.Size = new Size(104, 22);
            rbcol.TabIndex = 1;
            rbcol.Text = "columnas";
            // 
            // lFileIn
            // 
            lFileIn.BackColor = Color.LightGoldenrodYellow;
            lFileIn.BorderStyle = BorderStyle.FixedSingle;
            lFileIn.Font = new Font("Verdana", 6.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lFileIn.Location = new Point(304, 156);
            lFileIn.Name = "lFileIn";
            lFileIn.Size = new Size(272, 22);
            lFileIn.TabIndex = 79;
            lFileIn.Text = "Fichero a procesar";
            lFileIn.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbasp
            // 
            lbasp.BackColor = SystemColors.Info;
            lbasp.BorderStyle = BorderStyle.FixedSingle;
            lbasp.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lbasp.Location = new Point(104, 80);
            lbasp.Name = "lbasp";
            lbasp.Size = new Size(32, 30);
            lbasp.TabIndex = 80;
            lbasp.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label17
            // 
            label17.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label17.ForeColor = Color.Black;
            label17.Location = new Point(8, 88);
            label17.Name = "label17";
            label17.Size = new Size(88, 22);
            label17.TabIndex = 75;
            label17.Text = "aspirando a...";
            label17.TextAlign = ContentAlignment.TopRight;
            // 
            // tb04
            // 
            tb04.BorderStyle = BorderStyle.FixedSingle;
            tb04.CharacterCasing = CharacterCasing.Upper;
            tb04.Font = new Font("Arial", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tb04.Location = new Point(240, 100);
            tb04.MaxLength = 1;
            tb04.Name = "tb04";
            tb04.Size = new Size(32, 20);
            tb04.TabIndex = 48;
            tb04.Text = "-";
            tb04.TextAlign = HorizontalAlignment.Center;
            tb04.Visible = false;
            tb04.Enter += resultado_Enter;
            tb04.TextChanged += resultado_TextChanged;
            // 
            // bCalcular
            // 
            bCalcular.BackColor = Color.DarkSalmon;
            bCalcular.FlatStyle = FlatStyle.Popup;
            bCalcular.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            bCalcular.Image = ((Image)(resources.GetObject("bCalcular.Image")));
            bCalcular.ImageAlign = ContentAlignment.MiddleLeft;
            bCalcular.Location = new Point(280, 224);
            bCalcular.Name = "bCalcular";
            bCalcular.Size = new Size(96, 29);
            bCalcular.TabIndex = 44;
            bCalcular.Text = "Calcular";
            bCalcular.TextAlign = ContentAlignment.MiddleRight;
            bCalcular.UseVisualStyleBackColor = false;
            bCalcular.Click += BCalcularClick;
            // 
            // lp100viu
            // 
            lp100viu.BackColor = Color.LightGoldenrodYellow;
            lp100viu.BorderStyle = BorderStyle.FixedSingle;
            lp100viu.Font = new Font("Verdana", 6.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lp100viu.Location = new Point(304, 179);
            lp100viu.Name = "lp100viu";
            lp100viu.Size = new Size(272, 23);
            lp100viu.TabIndex = 83;
            lp100viu.Text = "Premios remanentes";
            lp100viu.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // bMenos
            // 
            bMenos.BackColor = Color.LightSalmon;
            bMenos.FlatStyle = FlatStyle.Popup;
            bMenos.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            bMenos.Location = new Point(144, 95);
            bMenos.Name = "bMenos";
            bMenos.Size = new Size(16, 15);
            bMenos.TabIndex = 79;
            bMenos.Text = "-";
            bMenos.UseVisualStyleBackColor = false;
            bMenos.Click += BMenosClick;
            // 
            // lproc
            // 
            lproc.BackColor = Color.LightGoldenrodYellow;
            lproc.BorderStyle = BorderStyle.FixedSingle;
            lproc.Font = new Font("Verdana", 6.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lproc.Location = new Point(384, 216);
            lproc.Name = "lproc";
            lproc.Size = new Size(88, 23);
            lproc.TabIndex = 42;
            lproc.Text = "Procesadas";
            lproc.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // bMas
            // 
            bMas.BackColor = Color.LightSalmon;
            bMas.FlatStyle = FlatStyle.Popup;
            bMas.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            bMas.Location = new Point(144, 79);
            bMas.Name = "bMas";
            bMas.Size = new Size(16, 15);
            bMas.TabIndex = 78;
            bMas.Text = "+";
            bMas.UseVisualStyleBackColor = false;
            bMas.Click += BMasClick;
            // 
            // tb14
            // 
            tb14.BackColor = Color.LightGoldenrodYellow;
            tb14.BorderStyle = BorderStyle.FixedSingle;
            tb14.CharacterCasing = CharacterCasing.Upper;
            tb14.Font = new Font("Arial", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tb14.Location = new Point(240, 310);
            tb14.MaxLength = 1;
            tb14.Name = "tb14";
            tb14.Size = new Size(32, 20);
            tb14.TabIndex = 58;
            tb14.Text = "-";
            tb14.TextAlign = HorizontalAlignment.Center;
            tb14.Visible = false;
            tb14.Enter += resultado_Enter;
            tb14.TextChanged += resultado_TextChanged;
            // 
            // bGrabar
            // 
            bGrabar.BackColor = Color.DarkSalmon;
            bGrabar.FlatStyle = FlatStyle.Popup;
            bGrabar.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            bGrabar.Image = ((Image)(resources.GetObject("bGrabar.Image")));
            bGrabar.ImageAlign = ContentAlignment.MiddleLeft;
            bGrabar.Location = new Point(480, 224);
            bGrabar.Name = "bGrabar";
            bGrabar.Size = new Size(96, 29);
            bGrabar.TabIndex = 82;
            bGrabar.Text = "Grabar";
            bGrabar.TextAlign = ContentAlignment.MiddleRight;
            bGrabar.UseVisualStyleBackColor = false;
            bGrabar.Click += BGrabarClick;
            // 
            // tb13
            // 
            tb13.BackColor = Color.LightGoldenrodYellow;
            tb13.BorderStyle = BorderStyle.FixedSingle;
            tb13.CharacterCasing = CharacterCasing.Upper;
            tb13.Font = new Font("Arial", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tb13.Location = new Point(240, 289);
            tb13.MaxLength = 1;
            tb13.Name = "tb13";
            tb13.Size = new Size(32, 20);
            tb13.TabIndex = 57;
            tb13.Text = "-";
            tb13.TextAlign = HorizontalAlignment.Center;
            tb13.Visible = false;
            tb13.Enter += resultado_Enter;
            tb13.TextChanged += resultado_TextChanged;
            // 
            // rbcen2
            // 
            rbcen2.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            rbcen2.ForeColor = Color.Black;
            rbcen2.Location = new Point(16, 36);
            rbcen2.Name = "rbcen2";
            rbcen2.Size = new Size(104, 22);
            rbcen2.TabIndex = 77;
            rbcen2.Text = "% decimales";
            rbcen2.CheckedChanged += RbcolCheckedChanged;
            // 
            // rbcen0
            // 
            rbcen0.Checked = true;
            rbcen0.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            rbcen0.ForeColor = Color.Black;
            rbcen0.Location = new Point(16, 13);
            rbcen0.Name = "rbcen0";
            rbcen0.Size = new Size(104, 23);
            rbcen0.TabIndex = 0;
            rbcen0.TabStop = true;
            rbcen0.Text = "% enteros";
            rbcen0.CheckedChanged += RbcolCheckedChanged;
            // 
            // bFileIn
            // 
            bFileIn.BackColor = Color.LightSalmon;
            bFileIn.FlatStyle = FlatStyle.Popup;
            bFileIn.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            bFileIn.Image = ((Image)(resources.GetObject("bFileIn.Image")));
            bFileIn.Location = new Point(279, 156);
            bFileIn.Name = "bFileIn";
            bFileIn.Size = new Size(24, 22);
            bFileIn.TabIndex = 80;
            bFileIn.UseVisualStyleBackColor = false;
            bFileIn.Click += BFileInClick;
            // 
            // label18
            // 
            label18.BackColor = Color.LightGoldenrodYellow;
            label18.BorderStyle = BorderStyle.FixedSingle;
            label18.Font = new Font("Verdana", 6.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            label18.Location = new Point(288, 280);
            label18.Name = "label18";
            label18.Size = new Size(280, 92);
            label18.TabIndex = 77;
            label18.Text = "Nota: La columna \"R\" y la casilla de aspiración son para el escrutinio parcial de" +
                " las tardes de sábado y domingo";
            label18.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.Font = new Font("Verdana", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            label2.ForeColor = Color.Maroon;
            label2.Location = new Point(1, 15);
            label2.Name = "label2";
            label2.Size = new Size(231, 18);
            label2.TabIndex = 59;
            label2.Text = "%";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tb15
            // 
            tb15.BorderStyle = BorderStyle.FixedSingle;
            tb15.CharacterCasing = CharacterCasing.Upper;
            tb15.Font = new Font("Arial", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tb15.Location = new Point(240, 331);
            tb15.MaxLength = 1;
            tb15.Name = "tb15";
            tb15.Size = new Size(32, 20);
            tb15.TabIndex = 105;
            tb15.Text = "-";
            tb15.TextAlign = HorizontalAlignment.Center;
            tb15.Visible = false;
            tb15.Enter += resultado_Enter;
            tb15.TextChanged += resultado_TextChanged;
            // 
            // tb11
            // 
            tb11.BorderStyle = BorderStyle.FixedSingle;
            tb11.CharacterCasing = CharacterCasing.Upper;
            tb11.Font = new Font("Arial", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tb11.Location = new Point(240, 247);
            tb11.MaxLength = 1;
            tb11.Name = "tb11";
            tb11.Size = new Size(32, 20);
            tb11.TabIndex = 55;
            tb11.Text = "-";
            tb11.TextAlign = HorizontalAlignment.Center;
            tb11.Visible = false;
            tb11.Enter += resultado_Enter;
            tb11.TextChanged += resultado_TextChanged;
            // 
            // tb10
            // 
            tb10.BorderStyle = BorderStyle.FixedSingle;
            tb10.CharacterCasing = CharacterCasing.Upper;
            tb10.Font = new Font("Arial", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tb10.Location = new Point(240, 226);
            tb10.MaxLength = 1;
            tb10.Name = "tb10";
            tb10.Size = new Size(32, 20);
            tb10.TabIndex = 54;
            tb10.Text = "-";
            tb10.TextAlign = HorizontalAlignment.Center;
            tb10.Visible = false;
            tb10.Enter += resultado_Enter;
            tb10.TextChanged += resultado_TextChanged;
            // 
            // tb12
            // 
            tb12.BackColor = Color.LightGoldenrodYellow;
            tb12.BorderStyle = BorderStyle.FixedSingle;
            tb12.CharacterCasing = CharacterCasing.Upper;
            tb12.Font = new Font("Arial", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tb12.Location = new Point(240, 268);
            tb12.MaxLength = 1;
            tb12.Name = "tb12";
            tb12.Size = new Size(32, 20);
            tb12.TabIndex = 56;
            tb12.Text = "-";
            tb12.TextAlign = HorizontalAlignment.Center;
            tb12.Visible = false;
            tb12.Enter += resultado_Enter;
            tb12.TextChanged += resultado_TextChanged;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(lbasp);
            groupBox1.Controls.Add(bMenos);
            groupBox1.Controls.Add(bMas);
            groupBox1.Controls.Add(rbcen2);
            groupBox1.Controls.Add(rbcol);
            groupBox1.Controls.Add(rbcen0);
            groupBox1.Controls.Add(label17);
            groupBox1.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            groupBox1.ForeColor = Color.Maroon;
            groupBox1.Location = new Point(279, 26);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(297, 115);
            groupBox1.TabIndex = 78;
            groupBox1.TabStop = false;
            groupBox1.Text = "Ver";
            // 
            // ltime
            // 
            ltime.BackColor = Color.LightGoldenrodYellow;
            ltime.BorderStyle = BorderStyle.FixedSingle;
            ltime.Font = new Font("Verdana", 6.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            ltime.Location = new Point(384, 240);
            ltime.Name = "ltime";
            ltime.Size = new Size(88, 22);
            ltime.TabIndex = 43;
            ltime.Text = "Tiempo";
            ltime.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tb08
            // 
            tb08.BackColor = Color.LightGoldenrodYellow;
            tb08.BorderStyle = BorderStyle.FixedSingle;
            tb08.CharacterCasing = CharacterCasing.Upper;
            tb08.Font = new Font("Arial", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tb08.Location = new Point(240, 184);
            tb08.MaxLength = 1;
            tb08.Name = "tb08";
            tb08.Size = new Size(32, 20);
            tb08.TabIndex = 52;
            tb08.Text = "-";
            tb08.TextAlign = HorizontalAlignment.Center;
            tb08.Visible = false;
            tb08.Enter += resultado_Enter;
            tb08.TextChanged += resultado_TextChanged;
            // 
            // tb09
            // 
            tb09.BorderStyle = BorderStyle.FixedSingle;
            tb09.CharacterCasing = CharacterCasing.Upper;
            tb09.Font = new Font("Arial", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tb09.Location = new Point(240, 205);
            tb09.MaxLength = 1;
            tb09.Name = "tb09";
            tb09.Size = new Size(32, 20);
            tb09.TabIndex = 53;
            tb09.Text = "-";
            tb09.TextAlign = HorizontalAlignment.Center;
            tb09.Visible = false;
            tb09.Enter += resultado_Enter;
            tb09.TextChanged += resultado_TextChanged;
            // 
            // tb05
            // 
            tb05.BackColor = Color.LightGoldenrodYellow;
            tb05.BorderStyle = BorderStyle.FixedSingle;
            tb05.CharacterCasing = CharacterCasing.Upper;
            tb05.Font = new Font("Arial", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tb05.Location = new Point(240, 121);
            tb05.MaxLength = 1;
            tb05.Name = "tb05";
            tb05.Size = new Size(32, 20);
            tb05.TabIndex = 49;
            tb05.Text = "-";
            tb05.TextAlign = HorizontalAlignment.Center;
            tb05.Visible = false;
            tb05.Enter += resultado_Enter;
            tb05.TextChanged += resultado_TextChanged;
            // 
            // tb06
            // 
            tb06.BackColor = Color.LightGoldenrodYellow;
            tb06.BorderStyle = BorderStyle.FixedSingle;
            tb06.CharacterCasing = CharacterCasing.Upper;
            tb06.Font = new Font("Arial", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tb06.Location = new Point(240, 142);
            tb06.MaxLength = 1;
            tb06.Name = "tb06";
            tb06.Size = new Size(32, 20);
            tb06.TabIndex = 50;
            tb06.Text = "-";
            tb06.TextAlign = HorizontalAlignment.Center;
            tb06.Visible = false;
            tb06.Enter += resultado_Enter;
            tb06.TextChanged += resultado_TextChanged;
            // 
            // tb07
            // 
            tb07.BackColor = Color.LightGoldenrodYellow;
            tb07.BorderStyle = BorderStyle.FixedSingle;
            tb07.CharacterCasing = CharacterCasing.Upper;
            tb07.Font = new Font("Arial", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tb07.Location = new Point(240, 163);
            tb07.MaxLength = 1;
            tb07.Name = "tb07";
            tb07.Size = new Size(32, 20);
            tb07.TabIndex = 51;
            tb07.Text = "-";
            tb07.TextAlign = HorizontalAlignment.Center;
            tb07.Visible = false;
            tb07.Enter += resultado_Enter;
            tb07.TextChanged += resultado_TextChanged;
            // 
            // tb01
            // 
            tb01.BorderStyle = BorderStyle.FixedSingle;
            tb01.CharacterCasing = CharacterCasing.Upper;
            tb01.Font = new Font("Arial", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tb01.Location = new Point(240, 37);
            tb01.MaxLength = 1;
            tb01.Name = "tb01";
            tb01.Size = new Size(32, 20);
            tb01.TabIndex = 45;
            tb01.Text = "-";
            tb01.TextAlign = HorizontalAlignment.Center;
            tb01.Visible = false;
            tb01.Enter += resultado_Enter;
            tb01.TextChanged += resultado_TextChanged;
            // 
            // tb02
            // 
            tb02.BorderStyle = BorderStyle.FixedSingle;
            tb02.CharacterCasing = CharacterCasing.Upper;
            tb02.Font = new Font("Arial", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tb02.Location = new Point(240, 58);
            tb02.MaxLength = 1;
            tb02.Name = "tb02";
            tb02.Size = new Size(32, 20);
            tb02.TabIndex = 46;
            tb02.Text = "-";
            tb02.TextAlign = HorizontalAlignment.Center;
            tb02.Visible = false;
            tb02.Enter += resultado_Enter;
            tb02.TextChanged += resultado_TextChanged;
            // 
            // tb03
            // 
            tb03.BorderStyle = BorderStyle.FixedSingle;
            tb03.CharacterCasing = CharacterCasing.Upper;
            tb03.Font = new Font("Arial", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tb03.Location = new Point(240, 79);
            tb03.MaxLength = 1;
            tb03.Name = "tb03";
            tb03.Size = new Size(32, 20);
            tb03.TabIndex = 47;
            tb03.Text = "-";
            tb03.TextAlign = HorizontalAlignment.Center;
            tb03.Visible = false;
            tb03.Enter += resultado_Enter;
            tb03.TextChanged += resultado_TextChanged;
            // 
            // tb16
            // 
            tb16.BorderStyle = BorderStyle.FixedSingle;
            tb16.CharacterCasing = CharacterCasing.Upper;
            tb16.Font = new Font("Arial", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            tb16.Location = new Point(240, 352);
            tb16.MaxLength = 1;
            tb16.Name = "tb16";
            tb16.Size = new Size(32, 20);
            tb16.TabIndex = 106;
            tb16.Text = "-";
            tb16.TextAlign = HorizontalAlignment.Center;
            tb16.Visible = false;
            // 
            // panel1
            // 
            panel1.Location = new Point(1, 33);
            panel1.Name = "panel1";
            panel1.Size = new Size(233, 382);
            panel1.TabIndex = 107;
            // 
            // chkPleno
            // 
            chkPleno.AutoSize = true;
            chkPleno.Checked = true;
            chkPleno.CheckState = CheckState.Checked;
            chkPleno.Font = new Font("Verdana", 6.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            chkPleno.Location = new Point(290, 5);
            chkPleno.Name = "chkPleno";
            chkPleno.Size = new Size(212, 16);
            chkPleno.TabIndex = 108;
            chkPleno.Text = "Considerar último partido como Pleno";
            chkPleno.UseVisualStyleBackColor = true;
            // 
            // VSignosFrm
            // 
            AutoScaleBaseSize = new Size(5, 13);
            BackColor = Color.Bisque;
            ClientSize = new Size(584, 427);
            Controls.Add(chkPleno);
            Controls.Add(panel1);
            Controls.Add(tb16);
            Controls.Add(tb15);
            Controls.Add(tb13);
            Controls.Add(tb14);
            Controls.Add(tb11);
            Controls.Add(tb12);
            Controls.Add(tb09);
            Controls.Add(tb10);
            Controls.Add(tb07);
            Controls.Add(tb08);
            Controls.Add(tb05);
            Controls.Add(tb06);
            Controls.Add(tb03);
            Controls.Add(tb04);
            Controls.Add(tb01);
            Controls.Add(tb02);
            Controls.Add(bLimp);
            Controls.Add(lp100viu);
            Controls.Add(bGrabar);
            Controls.Add(bFileIn);
            Controls.Add(lFileIn);
            Controls.Add(groupBox1);
            Controls.Add(label18);
            Controls.Add(label2);
            Controls.Add(bCalcular);
            Controls.Add(ltime);
            Controls.Add(lproc);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "VSignosFrm";
            SizeGripStyle = SizeGripStyle.Hide;
            Text = "Análisis de signos";
            groupBox1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();

		}
		#endregion
		
		void BCalcularClick(object sender, EventArgs e)
        {
            Calcular();
        }
		void elmeuTimer(object sender, EventArgs e)
        { 
            veureelmeu();
        }
		void RbcolCheckedChanged(object sender, EventArgs e)
        { 
            PintaPantalla(); 
        }
		void BFileInClick(object sender, EventArgs e) 
        {
            EntradaFichero();
        }
		void BGrabarClick(object sender, EventArgs e) 
        { 
            Grabar();
        }

		void BMasClick(object sender, EventArgs e)
        { 
            AspMas();
        }

		void BMenosClick(object sender, EventArgs e) 
        { 
            AspMenos();
        }

		void BLimpClick(object sender, EventArgs e)
        {
            ReinicializaColumnaGanadora();
		}
		
		// añadidos de TM para estudiar
		private TextBox buscarCuadroPartido(int numPartido)
		{
			TextBox partido;
			string nombreControl="tb"+numPartido.ToString("00");
			for(int i = 0; i < Controls.Count; i++)
			{
				//usamos el "as" para convertir un obejto al tipo que queramos
				//Si el objeto es de ese tipo se convierte y se asigna a la
				//variable, si no, su valor sera null
				partido= Controls[i] as TextBox;
				//si el objeto no esta vacio, tenemos el tipo de objeto que buscamos...
				if(partido!= null)
				{
					if(nombreControl==partido.Name) return partido;
				}
			}
			return null;
		}

		private void borrarSeleccionado(int numPartido)
		{
			Color fondo1=Color.PeachPuff;
			Color fondo2=Color.LightGoldenrodYellow;
			Color fondo;
			switch(numPartido)
			{
				case 1:
				case 2:
				case 3:
				case 4:
				case 9:
				case 10:
				case 11:
				case 15:
					fondo=fondo1;
					break;
				default:
					fondo=fondo2;
					break;
			}
			Label l;
			l=buscarEtiqueta(numPartido,"1");
			l.BackColor=fondo;
			l=buscarEtiqueta(numPartido,"x");
			l.BackColor=fondo;
			l=buscarEtiqueta(numPartido,"2");
			l.BackColor=fondo;
		}
		private Label buscarEtiqueta(int numPartido, string signo)
		{
			Label partido;
			string nombreControl="p_"+numPartido.ToString("00")+"_"+signo.ToLower();
			for(int i = 0; i < panel1.Controls.Count; i++)
			{

				partido= (Label)panel1.Controls[i];

                
                if(nombreControl==partido.Name) return partido;
				
			}
			return null;
		}
		private void resultado_TextChanged(object sender, EventArgs e)
		{
			TextBox partido=(TextBox)sender;
			if(partido.Text!="1" && partido.Text!="X" && partido.Text!="2" && partido.Text!="-" && partido.Text!=" ")
			{
				MessageBox.Show("Hay carácteres no válidos en el resultado (sólo válidos 1, X, 2 y -).","Free1X2",MessageBoxButtons.OK,MessageBoxIcon.Error);
                partido.Text = "-";
				partido.Focus();
			}
			int numPartido=Convert.ToInt16(partido.Name.Substring(2));
			borrarSeleccionado(numPartido);
			if(partido.Text!="-" && partido.Text!=" ")
			{
				TextBox txt=buscarCuadroPartido(numPartido);
				txt.Text=partido.Text;
				Label etiqueta=buscarEtiqueta(numPartido,partido.Text);
                if (etiqueta != null)
                {
                    etiqueta.BackColor = Color.PaleTurquoise;
                }
			}
		}
		private void resultado_Enter(object sender, EventArgs e)
		{
			TextBox partido=(TextBox)sender;
			partido.SelectionStart=0;
			partido.SelectionLength=1;
		}

        private void lbl_Click(object sender, EventArgs e)
        {
            Label l = (Label)sender;
            string[] valores = l.Name.Split('_');

            //Tenemos
            //valores[0]= p
            //valores[1]=xx
            //valores[2]=signo
            int partido = Convert.ToInt32(valores[1]);
            int indice = partido - 1;
            txts[indice].Text = valores[2];
        }
		
	}
}

