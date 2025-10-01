using System;
using System.IO;
using System.Windows.Forms;
using System.Collections;
using System.Data;
using Free1X2.EntradaSalida;

namespace Free1X2.UI {
	public class SelectorMS : Form {
		private Button bMenos;
		private Button bMenosR;
		private Label lFileOut;
		private Label lbCGR;
		private Button bFG;
		private Button bIniciar;
		private Button bAnalizar;
		private Button bMasR;
		private Label lbasp;
		private GroupBox groupBox3;
		private Button bCancelar;
		private DataGrid dataGrid1;
		private Label lFileIn;
		private TextBox tbColR;
		private Label label17;
		private Button bGrabarCols;
		private Label lTime;
		private Button bSumar;
		private Label lSumSel;
		private Label lCol;
		private Label lFGR;
        private Button bMas;
		private System.ComponentModel.Container components = null;
		public SelectorMS() {
			InitializeComponent();
			InicializaColumnasGrid();
			myTimer = new Timer();
			myTimer.Interval = 3000;
			myTimer.Tick += CalculoColumnas;
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}
		
		private BitArray existe = new BitArray(4782969);
		private BitArray repes = new BitArray(4782969);
		private int[] pot = new int[] {1,3,9,27,81,243,729,2187,6561,19683,59049,177147,531441,1594323};
		private int[,] tabla = new int[4782969,2];
		private string columna, tmp;
		private int conta, nx, ncalc, nref, ncg;
		private DateTime time0, time9;
		private int[] lbtab = new int[2913];
		private int[,] resuls = new int[2913,5];
		//private StreamWriter sw = null;
		private bool salida, stat;
		private Timer myTimer;
		private int[,] comtab = new int[2187,2187];
		private int c7p, c7u, n7p, n7u;
		private DataSet datasetDatos;
		DataGridTableStyle tableStyle = new DataGridTableStyle();
		private int limite, limsh;
		private int limcgsR, nrfCGR;
		private string[] colgsR = new string[3000];
		
		private void InicializaColumnasGrid() {
			tableStyle.MappingName = "Resultados";
			tableStyle.AllowSorting =false;
		    DataGridTextBoxColumn cs = new DataGridTextBoxColumn();
			cs.MappingName = "Q";
			cs.HeaderText = "";
			cs.Width = 40;
			cs.ReadOnly = true;
			cs.Alignment = HorizontalAlignment.Right;
			tableStyle.GridColumnStyles.Add(cs);
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "C";
			cs.HeaderText = "cols..";
			cs.Width = 80;
			cs.ReadOnly = true;
			cs.Alignment = HorizontalAlignment.Right;
			tableStyle.GridColumnStyles.Add(cs);
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "P14";
			cs.HeaderText = "14 .";
			cs.Width = 25;
			cs.ReadOnly = true;
			cs.Alignment = HorizontalAlignment.Right;
			tableStyle.GridColumnStyles.Add(cs);
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "P13";
			cs.HeaderText = "13 .";
			cs.Width = 30;
			cs.ReadOnly = true;
			cs.Alignment = HorizontalAlignment.Right;
			tableStyle.GridColumnStyles.Add(cs);
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "P12";
			cs.HeaderText = "12 .";
			cs.Width = 30;
			cs.ReadOnly = true;
			cs.Alignment = HorizontalAlignment.Right;
			tableStyle.GridColumnStyles.Add(cs);
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "P11";
			cs.HeaderText = "11 .";
			cs.Width = 40;
			cs.ReadOnly = true;
			cs.Alignment = HorizontalAlignment.Right;
			tableStyle.GridColumnStyles.Add(cs);
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "P10";
			cs.HeaderText = "10 .";
			cs.Width = 40;
			cs.ReadOnly = true;
			cs.Alignment = HorizontalAlignment.Right;
			tableStyle.GridColumnStyles.Add(cs);
			
			dataGrid1.TableStyles.Clear();
			dataGrid1.TableStyles.Add(tableStyle);
		}
		private void InicializaFuenteDatos() {
		    datasetDatos = new DataSet();
			DataTable newTable = new DataTable("Resultados");
			Application.DoEvents();
			newTable.Columns.Add("Q", typeof(int));
			newTable.Columns.Add("C", typeof(int));
			newTable.Columns.Add("P14", typeof(int));
			newTable.Columns.Add("P13", typeof(int));
			newTable.Columns.Add("P12", typeof(int));
			newTable.Columns.Add("P11", typeof(int));
			newTable.Columns.Add("P10", typeof(int));
			datasetDatos.Tables.Add(newTable);
			if (limite==13) limsh=29; 
			else if (limite==12) limsh=365;
			else limsh=2913;
			for (int nr=0; nr<limsh; nr++) {
				DataRow row = datasetDatos.Tables["Resultados"].NewRow();
				row["Q"] = nr;
				row["C"] = lbtab[nr];
				row["P14"] = resuls[nr,0];
				row["P13"] = resuls[nr,1];
				row["P12"] = resuls[nr,2];
				row["P11"] = resuls[nr,3];
				row["P10"] = resuls[nr,4];
				datasetDatos.Tables["Resultados"].Rows.Add(row);
			}
			dataGrid1.SetDataBinding(datasetDatos, "Resultados");
		}
		private void Iniciar() {
			bGrabarCols.Enabled=false;
			bIniciar.Enabled=false;
			bAnalizar.Enabled=false;
			salida=false;
			myTimer.Start();
			stat=false;
			lTime.Text = " ";
			limite = Convert.ToInt32(lbasp.Text);
			OpenFileDialog lee = new OpenFileDialog();
			lee.InitialDirectory = ".\\" ;
			lee.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
			if(lee.ShowDialog() == DialogResult.OK) {
				time0 = DateTime.Now;
				tmp = lee.FileName;
				string filein = Path.GetFileName(tmp);
				conta=0; lFileIn.Text = filein;
                IArchivoColumnas sr = new ArchivoColumnasTexto(tmp);
				bIniciar.Text="leyendo...";
				existe.SetAll(false);
				while (sr.SiguienteColumna()) {
					Application.DoEvents();
					if (salida) break;
					columna = sr.LeeColumnaSinComas();
					nx = s2n(columna);
					if (existe[nx]==false) {
						tabla[conta,0]=nx;
						tabla[conta,1]=0;
						existe[nx]=true;
						conta++;
					}
				}
				sr.Cerrar();
				Application.DoEvents();
				stat=true;
				bIniciar.Text="calculando...";
				if (limite==13) calcular13();
				else if (limite==12) calcular12();
				else calcular11();
				bIniciar.Text="asignando...";
				for (int nr=0; nr<2913; nr++) lbtab[nr]=0;
				for (int nr=0; nr<conta; nr++) {
					if (salida) break;
					nx=tabla[nr,1];
					lbtab[nx]++;
					Application.DoEvents();
				}
				InicializaFuenteDatos();
			}
			time9 = DateTime.Now;
			tmp = (time9-time0)+"00000000000";
			lTime.Text = tmp.Substring(0,11);
			lCol.Text = ""+(conta);
			myTimer.Stop();
			bIniciar.Text="iniciar";
			bAnalizar.Enabled=true;
			bIniciar.Enabled=true;
			bGrabarCols.Enabled=true;
		}
		private void calcular13() {
			for (ncalc=0; ncalc<conta; ncalc++) {
				if (salida) break;
				ncg = tabla[ncalc,0];
				for (int nr=0; nr<14; nr++)
				{
				    int sign1 = (ncg / pot[nr]) % 3;
				    for (int z1=0; z1<3; z1++) {
						if (z1 == sign1) continue;
						int col1 = ncg + pot[nr] * (z1 - sign1);
						if (existe[col1]) tabla[ncalc,1]++;
					}
				}
			    Application.DoEvents();
			}
		}
		private void calcular12() {
			for (ncalc=0; ncalc<conta; ncalc++) {
				if (salida) break;
			    int z1, col1, sign1;
				repes.SetAll(false);
				ncg = tabla[ncalc,0];
				repes[ncg]=true;
				for (int nr=0; nr<14; nr++) {
					sign1 = (ncg / pot[nr]) % 3;
					for (z1=0; z1<3; z1++) {
						if (z1 == sign1) continue;
						col1 = ncg + pot[nr] * (z1 - sign1);
						repes[col1]=true;
					}
				}
				for (int nr=0; nr<14; nr++) {
					sign1 = (ncg / pot[nr]) % 3;
					for (z1=0; z1<3; z1++) {
						if (z1 == sign1) continue;
						col1 = ncg + pot[nr] * (z1 - sign1);
						for (int nr2=0; nr2<14; nr2++)
						{
						    int sign2 = (col1 / pot[nr2]) % 3;
						    for (int z2=0; z2<3; z2++) {
								if (z2 == sign2) continue;
								int col2 = col1 + pot[nr2] * (z2 - sign2);
								if (existe[col2] && repes[col2]==false) {
									repes[col2]=true;
									tabla[ncalc,1]++;
								}
							}
						}
					}
				}
				Application.DoEvents();
			}
		}
		private void calcular11() {
			for (ncalc=0; ncalc<conta; ncalc++) {
				if (salida) break;
				int z1,col1,sign1, z2,col2,sign2, z3;
				repes.SetAll(false);
				ncg = tabla[ncalc,0];
				repes[ncg]=true;
				for (int nr=0; nr<14; nr++) {
					sign1 = (ncg / pot[nr]) % 3;
					for (z1=0; z1<3; z1++) {
						if (z1 == sign1) continue;
						col1 = ncg + pot[nr] * (z1 - sign1);
						repes[col1]=true;
						for (int nr2=0; nr2<14; nr2++) {
							sign2 = (col1 / pot[nr2]) % 3;
							for (z2=0; z2<3; z2++) {
								if (z2 == sign2) continue;
								col2 = col1 + pot[nr2] * (z2 - sign2);
								repes[col2]=true;
							}
						}
					}
				}
				for (int nr=0; nr<14; nr++) {
					sign1 = (ncg / pot[nr]) % 3;
					for (z1=0; z1<3; z1++) {
						if (z1 == sign1) continue;
						col1 = ncg + pot[nr] * (z1 - sign1);
						for (int nr2=0; nr2<14; nr2++) {
							sign2 = (col1 / pot[nr2]) % 3;
							for (z2=0; z2<3; z2++) {
								if (z2 == sign2) continue;
								col2 = col1 + pot[nr2] * (z2 - sign2);
								for (int nr3=0; nr3<14; nr3++)
								{
								    int sign3 = (col2 / pot[nr3]) % 3;
								    for (z3=0; z3<3; z3++) {
										if (z3 == sign3) continue;
										int col3 = col2 + pot[nr3] * (z3 - sign3);
										if (existe[col3] && repes[col3]==false) {
											repes[col3]=true;
											tabla[ncalc,1]++;
										}
									}
								}
							}
						}
					}
				}
				Application.DoEvents();
			}
		}
		private void Grabar()
		{
			string fileout;
			bIniciar.Enabled=false;
			bGrabarCols.Enabled=false;
			bAnalizar.Enabled=false;
			bGrabarCols.Text = "grabando...";
			lFileOut.Text = "";
			SaveFileDialog grabacols = new SaveFileDialog();
			grabacols.InitialDirectory = ".\\" ;
			grabacols.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
			if(grabacols.ShowDialog() == DialogResult.OK) {
				tmp = grabacols.FileName;
				fileout = Path.GetFileName(tmp);
                IArchivoColumnas w = new ArchivoColumnasTexto(grabacols.FileName);
				for (int nr=0; nr<conta; nr++) {
					nx=tabla[nr,1];
					if (dataGrid1.IsSelected(nx)) w.GuardarCols(VerCol(nr));
					Application.DoEvents();
				}
				w.Cerrar();
				lFileOut.Text = fileout;
			}
			bGrabarCols.Text = "grabar";
			bAnalizar.Enabled=true;
			bGrabarCols.Enabled=true;
			bIniciar.Enabled=true;
		}

		private string VerCol(int nr)
		{
			return n2s(tabla[nr,0]);
		}

		private void SumSel() {
			int suma=0;
			if (conta==0) return;
			if (limite==13) limsh=29; 
			else if (limite==12) limsh=365;
			else limsh=2913;
			for (int nr=0; nr<limsh; nr++) {
				if (dataGrid1.IsSelected(nr)) {
					suma += (int)dataGrid1[nr,1];
				}
			}
			lSumSel.Text = ""+suma;
			Application.DoEvents();
		}
		private void Analizar() {
			int na;
			if (conta==0) return;
			bAnalizar.Enabled=false;
			bIniciar.Enabled=false;
			bGrabarCols.Enabled=false;
			bSumar.Enabled=false;
			if (limite==13) limsh=29; 
			else if (limite==12) limsh=365;
			else limsh=2913;
			for (int nr=0; nr<limsh; nr++) {
				resuls[nr,0]=resuls[nr,1]=resuls[nr,2]=resuls[nr,3]=resuls[nr,4]=0;
			}
			Comparador2();
			ncg = s2n(tbColR.Text);
			c7p = ncg/2187; c7u = ncg%2187;
			for (int nr=0; nr<conta; nr++) {
				nx=tabla[nr,0]; nref=tabla[nr,1];
				na = neq(nx);
				if (na>9) {
					resuls[nref,(14-na)]++;
				}
			}
			InicializaFuenteDatos();
			bAnalizar.Enabled=true;
			bIniciar.Enabled=true;
			bGrabarCols.Enabled=true;
			bSumar.Enabled=true;
		}
		private string n7s(int nx) {
			string ax = ""; int nx2;
			for (int nr=0; nr<7; nr++) {
				nx2 = nx%3; nx /= 3;
				if (nx2==1) ax = "1"+ax;
				else if (nx2==2) ax = "2"+ax;
				else ax = "X"+ax;
			}
			return ax;
		}
		private string n2s(int nx) {
			string ax = ""; int nx2;
			for (int nr=0; nr<14; nr++) {
				nx2 = nx%3; nx /= 3;
				if (nx2==1) ax = "1"+ax;
				else if (nx2==2) ax = "2"+ax;
				else ax = "X"+ax;
			}
			return ax;
		}
		private int s2n(string ax) {
			int nx = 0;
			for (int nr=0; nr<14; nr++) {
				nx *= 3;
				string ch = ax.Substring(nr,1);
				if (ch=="1") nx+=1;
				else if (ch=="2") nx+=2;
			}
			return nx;
		}
		private void Comparador2() {
			for (int nr=0; nr<2187;nr++) {
				string ax1 = n7s(nr);
				comtab[nr,nr]=7;
				for (int nr2=nr+1; nr2<2187; nr2++) {
					string ax2 = n7s(nr2);
					int na = neq(ax1, ax2);
					comtab[nr,nr2]=na;
					comtab[nr2,nr]=na;
				}
			}
		}
		private int neq(int colnum) {
			n7p = colnum/2187; n7u = colnum%2187;
			return comtab[c7p,n7p]+comtab[c7u,n7u];
		}
		private int neq(string ax1, string ax2) {
			int na = 0;
			for (int nr=0; nr<7; nr++) {
				if (ax1[nr]==ax2[nr]) na++;
			}
			return na;
		}
		private void AspMas() {
			int naux = Convert.ToInt32(lbasp.Text);
			if (naux<13) lbasp.Text=""+(naux+1);
		}
		private void AspMenos() {
			int naux = Convert.ToInt32(lbasp.Text);
			if (naux>11) lbasp.Text=""+(naux-1);
		}
		private void EntraCGsR() {
			string tmp;
			OpenFileDialog cgDialog = new OpenFileDialog();
			cgDialog.InitialDirectory = Application.StartupPath + "/";
			cgDialog.Filter = "F.Ganadoras(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			if(cgDialog.ShowDialog() == DialogResult.OK) {
				string filein = Path.GetFileName(cgDialog.FileName);
				limcgsR = 0;
				StreamReader sr = new StreamReader(filein);
				while (sr.Peek()>0) {
					tmp = sr.ReadLine().Trim().ToUpper();
					if (tmp.Length<14) { MessageBox.Show("col.G. errónea="+tmp); return; }
					colgsR[limcgsR] = tmp;
					limcgsR++;
					Application.DoEvents();
				}
				sr.Close();
				nrfCGR = limcgsR; lFGR.Text = filein;
				lbCGR.Text=""+nrfCGR; tbColR.Text=colgsR[nrfCGR-1];
				bAnalizar.Enabled = true;
			}
		}
		private void GRMas() {
			if (nrfCGR<limcgsR) {
				nrfCGR++;
				lbCGR.Text=""+nrfCGR; tbColR.Text=colgsR[nrfCGR-1];
			}
		}
		private void GRMenos() {
			if (nrfCGR>1) {
				nrfCGR--;
				lbCGR.Text=""+nrfCGR; tbColR.Text=colgsR[nrfCGR-1];
			}
		}
		
//		[STAThread]
//		public static void Main(string[] args) { Application.Run(new SelectorMS()); }
		protected override void Dispose( bool disposing ) {
			if( disposing ) {
				if (components != null)  {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectorMS));
            this.bMas = new System.Windows.Forms.Button();
            this.lFGR = new System.Windows.Forms.Label();
            this.lCol = new System.Windows.Forms.Label();
            this.lSumSel = new System.Windows.Forms.Label();
            this.bSumar = new System.Windows.Forms.Button();
            this.lTime = new System.Windows.Forms.Label();
            this.bGrabarCols = new System.Windows.Forms.Button();
            this.label17 = new System.Windows.Forms.Label();
            this.tbColR = new System.Windows.Forms.TextBox();
            this.lFileIn = new System.Windows.Forms.Label();
            this.dataGrid1 = new System.Windows.Forms.DataGrid();
            this.bCancelar = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.bFG = new System.Windows.Forms.Button();
            this.lbCGR = new System.Windows.Forms.Label();
            this.bMenosR = new System.Windows.Forms.Button();
            this.bMasR = new System.Windows.Forms.Button();
            this.bAnalizar = new System.Windows.Forms.Button();
            this.lbasp = new System.Windows.Forms.Label();
            this.bIniciar = new System.Windows.Forms.Button();
            this.lFileOut = new System.Windows.Forms.Label();
            this.bMenos = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // bMas
            // 
            this.bMas.BackColor = System.Drawing.Color.DarkSalmon;
            this.bMas.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bMas.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bMas.Location = new System.Drawing.Point(504, 14);
            this.bMas.Name = "bMas";
            this.bMas.Size = new System.Drawing.Size(16, 15);
            this.bMas.TabIndex = 248;
            this.bMas.Text = "+";
            this.bMas.UseVisualStyleBackColor = false;
            this.bMas.Click += new System.EventHandler(this.BMasClick);
            // 
            // lFGR
            // 
            this.lFGR.BackColor = System.Drawing.SystemColors.Info;
            this.lFGR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lFGR.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lFGR.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lFGR.Location = new System.Drawing.Point(16, 64);
            this.lFGR.Name = "lFGR";
            this.lFGR.Size = new System.Drawing.Size(168, 23);
            this.lFGR.TabIndex = 88;
            this.lFGR.Text = "Fichero Ganadoras";
            this.lFGR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lCol
            // 
            this.lCol.BackColor = System.Drawing.SystemColors.Info;
            this.lCol.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lCol.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lCol.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lCol.Location = new System.Drawing.Point(424, 104);
            this.lCol.Name = "lCol";
            this.lCol.Size = new System.Drawing.Size(104, 22);
            this.lCol.TabIndex = 91;
            this.lCol.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lSumSel
            // 
            this.lSumSel.BackColor = System.Drawing.SystemColors.Info;
            this.lSumSel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lSumSel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lSumSel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lSumSel.Location = new System.Drawing.Point(424, 264);
            this.lSumSel.Name = "lSumSel";
            this.lSumSel.Size = new System.Drawing.Size(104, 23);
            this.lSumSel.TabIndex = 97;
            this.lSumSel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bSumar
            // 
            this.bSumar.BackColor = System.Drawing.Color.DarkSalmon;
            this.bSumar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bSumar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bSumar.Image = ((System.Drawing.Image)(resources.GetObject("bSumar.Image")));
            this.bSumar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bSumar.Location = new System.Drawing.Point(424, 231);
            this.bSumar.Name = "bSumar";
            this.bSumar.Size = new System.Drawing.Size(104, 32);
            this.bSumar.TabIndex = 93;
            this.bSumar.Text = "Sumar";
            this.bSumar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bSumar.UseVisualStyleBackColor = false;
            this.bSumar.Click += new System.EventHandler(this.BSumarClick);
            // 
            // lTime
            // 
            this.lTime.BackColor = System.Drawing.SystemColors.Info;
            this.lTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lTime.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lTime.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lTime.Location = new System.Drawing.Point(424, 127);
            this.lTime.Name = "lTime";
            this.lTime.Size = new System.Drawing.Size(104, 23);
            this.lTime.TabIndex = 6;
            this.lTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bGrabarCols
            // 
            this.bGrabarCols.BackColor = System.Drawing.Color.DarkSalmon;
            this.bGrabarCols.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bGrabarCols.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bGrabarCols.Image = ((System.Drawing.Image)(resources.GetObject("bGrabarCols.Image")));
            this.bGrabarCols.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bGrabarCols.Location = new System.Drawing.Point(424, 295);
            this.bGrabarCols.Name = "bGrabarCols";
            this.bGrabarCols.Size = new System.Drawing.Size(104, 32);
            this.bGrabarCols.TabIndex = 4;
            this.bGrabarCols.Text = "Grabar";
            this.bGrabarCols.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bGrabarCols.UseVisualStyleBackColor = false;
            this.bGrabarCols.Click += new System.EventHandler(this.BGrabarClick);
            // 
            // label17
            // 
            this.label17.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.label17.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(432, 24);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(32, 23);
            this.label17.TabIndex = 247;
            this.label17.Text = "Ver:";
            this.label17.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // tbColR
            // 
            this.tbColR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbColR.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbColR.ForeColor = System.Drawing.SystemColors.ControlText;
            this.tbColR.Location = new System.Drawing.Point(16, 88);
            this.tbColR.MaxLength = 14;
            this.tbColR.Name = "tbColR";
            this.tbColR.Size = new System.Drawing.Size(168, 21);
            this.tbColR.TabIndex = 90;
            this.tbColR.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lFileIn
            // 
            this.lFileIn.BackColor = System.Drawing.SystemColors.Info;
            this.lFileIn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lFileIn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lFileIn.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lFileIn.Location = new System.Drawing.Point(424, 81);
            this.lFileIn.Name = "lFileIn";
            this.lFileIn.Size = new System.Drawing.Size(104, 22);
            this.lFileIn.TabIndex = 94;
            this.lFileIn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dataGrid1
            // 
            this.dataGrid1.AllowSorting = false;
            this.dataGrid1.BackgroundColor = System.Drawing.SystemColors.Info;
            this.dataGrid1.CaptionBackColor = System.Drawing.SystemColors.InactiveCaptionText;
            this.dataGrid1.CaptionFont = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGrid1.CaptionForeColor = System.Drawing.SystemColors.ControlText;
            this.dataGrid1.CaptionText = "Distribución";
            this.dataGrid1.DataMember = "";
            this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dataGrid1.Location = new System.Drawing.Point(16, 8);
            this.dataGrid1.Name = "dataGrid1";
            this.dataGrid1.ReadOnly = true;
            this.dataGrid1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dataGrid1.Size = new System.Drawing.Size(352, 536);
            this.dataGrid1.TabIndex = 246;
            // 
            // bCancelar
            // 
            this.bCancelar.BackColor = System.Drawing.Color.DarkSalmon;
            this.bCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bCancelar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bCancelar.Image = ((System.Drawing.Image)(resources.GetObject("bCancelar.Image")));
            this.bCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bCancelar.Location = new System.Drawing.Point(424, 151);
            this.bCancelar.Name = "bCancelar";
            this.bCancelar.Size = new System.Drawing.Size(104, 32);
            this.bCancelar.TabIndex = 92;
            this.bCancelar.Text = "Cancelar";
            this.bCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bCancelar.UseVisualStyleBackColor = false;
            this.bCancelar.Click += new System.EventHandler(this.BCancelarClick);
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.Bisque;
            this.groupBox3.Controls.Add(this.tbColR);
            this.groupBox3.Controls.Add(this.lFGR);
            this.groupBox3.Controls.Add(this.bFG);
            this.groupBox3.Controls.Add(this.lbCGR);
            this.groupBox3.Controls.Add(this.bMenosR);
            this.groupBox3.Controls.Add(this.bMasR);
            this.groupBox3.Controls.Add(this.bAnalizar);
            this.groupBox3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox3.Location = new System.Drawing.Point(376, 384);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(200, 160);
            this.groupBox3.TabIndex = 251;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Análisis Resultados";
            // 
            // bFG
            // 
            this.bFG.BackColor = System.Drawing.Color.DarkSalmon;
            this.bFG.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bFG.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bFG.ForeColor = System.Drawing.SystemColors.ControlText;
            this.bFG.Image = ((System.Drawing.Image)(resources.GetObject("bFG.Image")));
            this.bFG.Location = new System.Drawing.Point(16, 24);
            this.bFG.Name = "bFG";
            this.bFG.Size = new System.Drawing.Size(32, 32);
            this.bFG.TabIndex = 87;
            this.bFG.UseVisualStyleBackColor = false;
            this.bFG.Click += new System.EventHandler(this.BFGClick);
            // 
            // lbCGR
            // 
            this.lbCGR.BackColor = System.Drawing.SystemColors.Info;
            this.lbCGR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbCGR.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCGR.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lbCGR.Location = new System.Drawing.Point(128, 24);
            this.lbCGR.Name = "lbCGR";
            this.lbCGR.Size = new System.Drawing.Size(32, 30);
            this.lbCGR.TabIndex = 86;
            this.lbCGR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bMenosR
            // 
            this.bMenosR.BackColor = System.Drawing.Color.DarkSalmon;
            this.bMenosR.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bMenosR.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bMenosR.ForeColor = System.Drawing.SystemColors.ControlText;
            this.bMenosR.Location = new System.Drawing.Point(168, 40);
            this.bMenosR.Name = "bMenosR";
            this.bMenosR.Size = new System.Drawing.Size(16, 15);
            this.bMenosR.TabIndex = 85;
            this.bMenosR.Text = "-";
            this.bMenosR.UseVisualStyleBackColor = false;
            this.bMenosR.Click += new System.EventHandler(this.BMenosRClick);
            // 
            // bMasR
            // 
            this.bMasR.BackColor = System.Drawing.Color.DarkSalmon;
            this.bMasR.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bMasR.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bMasR.Location = new System.Drawing.Point(168, 24);
            this.bMasR.Name = "bMasR";
            this.bMasR.Size = new System.Drawing.Size(16, 15);
            this.bMasR.TabIndex = 84;
            this.bMasR.Text = "+";
            this.bMasR.UseVisualStyleBackColor = false;
            this.bMasR.Click += new System.EventHandler(this.BMasRClick);
            // 
            // bAnalizar
            // 
            this.bAnalizar.BackColor = System.Drawing.Color.DarkSalmon;
            this.bAnalizar.Enabled = false;
            this.bAnalizar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bAnalizar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bAnalizar.ForeColor = System.Drawing.SystemColors.ControlText;
            this.bAnalizar.Image = ((System.Drawing.Image)(resources.GetObject("bAnalizar.Image")));
            this.bAnalizar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bAnalizar.Location = new System.Drawing.Point(48, 120);
            this.bAnalizar.Name = "bAnalizar";
            this.bAnalizar.Size = new System.Drawing.Size(104, 32);
            this.bAnalizar.TabIndex = 27;
            this.bAnalizar.Text = "Analizar";
            this.bAnalizar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bAnalizar.UseVisualStyleBackColor = false;
            this.bAnalizar.Click += new System.EventHandler(this.BAnalizarClick);
            // 
            // lbasp
            // 
            this.lbasp.BackColor = System.Drawing.SystemColors.Info;
            this.lbasp.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbasp.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lbasp.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbasp.Location = new System.Drawing.Point(464, 15);
            this.lbasp.Name = "lbasp";
            this.lbasp.Size = new System.Drawing.Size(32, 30);
            this.lbasp.TabIndex = 250;
            this.lbasp.Text = "13";
            this.lbasp.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bIniciar
            // 
            this.bIniciar.BackColor = System.Drawing.Color.DarkSalmon;
            this.bIniciar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bIniciar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bIniciar.Image = ((System.Drawing.Image)(resources.GetObject("bIniciar.Image")));
            this.bIniciar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bIniciar.Location = new System.Drawing.Point(424, 48);
            this.bIniciar.Name = "bIniciar";
            this.bIniciar.Size = new System.Drawing.Size(104, 32);
            this.bIniciar.TabIndex = 3;
            this.bIniciar.Text = "Iniciar";
            this.bIniciar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bIniciar.UseVisualStyleBackColor = false;
            this.bIniciar.Click += new System.EventHandler(this.BIniciarClick);
            // 
            // lFileOut
            // 
            this.lFileOut.BackColor = System.Drawing.SystemColors.Info;
            this.lFileOut.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lFileOut.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lFileOut.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lFileOut.Location = new System.Drawing.Point(424, 328);
            this.lFileOut.Name = "lFileOut";
            this.lFileOut.Size = new System.Drawing.Size(104, 23);
            this.lFileOut.TabIndex = 98;
            this.lFileOut.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bMenos
            // 
            this.bMenos.BackColor = System.Drawing.Color.DarkSalmon;
            this.bMenos.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bMenos.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bMenos.Location = new System.Drawing.Point(504, 30);
            this.bMenos.Name = "bMenos";
            this.bMenos.Size = new System.Drawing.Size(16, 15);
            this.bMenos.TabIndex = 249;
            this.bMenos.Text = "-";
            this.bMenos.UseVisualStyleBackColor = false;
            this.bMenos.Click += new System.EventHandler(this.BMenosClick);
            // 
            // SelectorMS
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(592, 556);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.lbasp);
            this.Controls.Add(this.bMenos);
            this.Controls.Add(this.bMas);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.dataGrid1);
            this.Controls.Add(this.lFileOut);
            this.Controls.Add(this.lSumSel);
            this.Controls.Add(this.lFileIn);
            this.Controls.Add(this.bSumar);
            this.Controls.Add(this.bCancelar);
            this.Controls.Add(this.lCol);
            this.Controls.Add(this.lTime);
            this.Controls.Add(this.bGrabarCols);
            this.Controls.Add(this.bIniciar);
            this.Name = "SelectorMS";
            this.Text = "Selector MarioSan";
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion
		void BIniciarClick(object sender, System.EventArgs e) { Iniciar(); }
		void BGrabarClick(object sender, System.EventArgs e) { Grabar(); }
		void BCancelarClick(object sender, System.EventArgs e) { salida=true; }
		void BSumarClick(object sender, System.EventArgs e) { SumSel(); }
		void CalculoColumnas(object sender, System.EventArgs e) {
			lCol.Text = ""+(stat?ncalc:conta);
			time9 = DateTime.Now;
			tmp = (time9-time0)+"00000000000";
			lTime.Text = tmp.Substring(0,11);
		}
		void BAnalizarClick(object sender, System.EventArgs e) { Analizar(); }
		void BMasClick(object sender, System.EventArgs e) { AspMas(); }
		void BMenosClick(object sender, System.EventArgs e) { AspMenos(); }
		void BFGClick(object sender, System.EventArgs e) { EntraCGsR(); }
		void BMasRClick(object sender, System.EventArgs e) { GRMas(); }
		void BMenosRClick(object sender, System.EventArgs e) { GRMenos(); }
		
	}
}

