using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Data;
using Free1X2.EntradaSalida;
using Free1X2.Utils;

namespace Free1X2.UI {
	public class CombinarFiltros : Form {
		private Label label;
		private Button bSalvaLista;
		private Button bCargaLista;
		private Button btnGrabarFiltro;
		private Label lCols;
		private Button btnCargarFiltro;
		private GroupBox groupBox2;
		private DataGrid dataGrid1;
		private Button bCancelar;
		private Button bSumaCols;
		private Button bIniciar;
		private Label label3;
		private Label label2;
		private Label label1;
		private Label lfilout;
        private CheckBox ckMD;
        private TextBox tbmin;
		private TextBox tbmax;
        private Button btnReiniciaTodo;
        private int partidosEnJuego;
        public CombinarFiltros()
        {
            InitializeComponent();
            InicializaColumnasGrid();
            InicializaFuenteDatos();
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
        }
		
		private int[] htCols = new int[Convert.ToInt32(Math.Pow(3, VariablesGlobales.NumeroPartidos))];
		private BitArray repes = new BitArray(Convert.ToInt32(Math.Pow(3, VariablesGlobales.NumeroPartidos)));
		private int indice, ctcols;
		private DataSet dsDatos;
		DataGridTableStyle tabla = new DataGridTableStyle();
		private int limite;
		private bool salida;

        private void AdaptarANumeroDePartidos()
        {
            htCols = new int[Convert.ToInt32(Math.Pow(3, partidosEnJuego))];
            repes = new BitArray(Convert.ToInt32(Math.Pow(3, partidosEnJuego)));

        }
		private void InicializaColumnasGrid() {
			tabla.MappingName = "Filtros";
		    DataGridTextBoxColumn cs = new DataGridTextBoxColumn();
			cs.MappingName = "N";
			cs.HeaderText = "N";
			cs.Width = 35;
			cs.ReadOnly = true;
			cs.Alignment = HorizontalAlignment.Center;
			tabla.GridColumnStyles.Add(cs);
			
            cs = new DataGridTextBoxColumn();
			cs.MappingName = "F";
			cs.HeaderText = "filtros";
			cs.Width = 110;
			cs.ReadOnly = true;
			cs.Alignment = HorizontalAlignment.Center;
			tabla.GridColumnStyles.Add(cs);

            cs = new DataGridTextBoxColumn();
            cs.MappingName = "FP";
            cs.HeaderText = "";
            //columna es invisible
            cs.Width = 0;
            cs.ReadOnly = true;
            cs.Alignment = HorizontalAlignment.Center;
            tabla.GridColumnStyles.Add(cs);
			
            DataGridBoolColumn csBool = new DataGridBoolColumn();
			csBool.MappingName = "A";
			csBool.HeaderText = "A";
			csBool.Width = 35;
			cs.Alignment = HorizontalAlignment.Center;
			tabla.GridColumnStyles.Add(csBool);
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "C";
			cs.HeaderText = "cols..";
			cs.Width = 60;
			cs.ReadOnly = true;
			cs.Alignment = HorizontalAlignment.Right;
			tabla.GridColumnStyles.Add(cs);
			dataGrid1.TableStyles.Clear();
			dataGrid1.TableStyles.Add(tabla);
		}
		private void InicializaFuenteDatos() {
			dsDatos = new DataSet();
			DataTable newTable = new DataTable("Filtros");
			newTable.Columns.Add("N", typeof(int));
			newTable.Columns.Add("F", typeof(string));
            newTable.Columns.Add("FP", typeof(string));
			newTable.Columns.Add("A", typeof(bool));
			newTable.Columns.Add("C", typeof(int));
			dsDatos.Tables.Add(newTable);
			dataGrid1.SetDataBinding(dsDatos, "Filtros");
		}
        private void IniciarProceso()
        {
            string columna;
            bIniciar.Enabled = false;
            btnCargarFiltro.Enabled = false;
            bCargaLista.Enabled = false;
            bSalvaLista.Enabled = false;
            btnGrabarFiltro.Enabled = false;
            bSumaCols.Enabled = false;
            ckMD.Enabled = false;
            salida = false;
            for (int nr = 0; nr < htCols.Length; nr++)
            {
                htCols[nr] = 0;
            }
            IArchivoColumnas sr;
            for (int nr = 0; nr < limite; nr++)
            {
                if (salida) break;
                dataGrid1[nr, 4] = ctcols = 0;
                if ((bool)dataGrid1[nr, 3])
                {
                    string filein = (string)dataGrid1[nr, 2];
                    try
                    {
                        sr = new ArchivoColumnasTexto(filein);
                    }
                    catch
                    {
                        continue;
                    }
                    repes.SetAll(false);
                    while (sr.SiguienteColumna())
                    {
                        if (salida) break;
                        columna = Pral.Normaliza(sr.LeeColumnaSinComas()); ctcols++;
                        if (columna.Length < 14)
                        {
                            MessageBox.Show("error columna=" + ctcols);
                            break;
                        }
                        indice = s2n(columna);
                        if (!repes[indice])
                        {
                            htCols[indice]++;
                            repes[indice] = true;
                        }
                        Application.DoEvents();
                    }
                    sr.Cerrar();
                    dataGrid1[nr, 4] = ctcols;
                }
            }
            bIniciar.Enabled = true;
            btnCargarFiltro.Enabled = true;
            bCargaLista.Enabled = true;
            bSalvaLista.Enabled = true;
            btnGrabarFiltro.Enabled = true;
            bSumaCols.Enabled = true;
            ckMD.Enabled = true;
        }
		private void Grabar() {
		    btnCargarFiltro.Enabled=false;
			btnGrabarFiltro.Enabled=false;
			SaveFileDialog saveDialog = new SaveFileDialog();
			saveDialog.InitialDirectory = "*\\";
			saveDialog.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
			if(saveDialog.ShowDialog() == DialogResult.OK) {
				string fileout = saveDialog.FileName;
				int tot = 0; lCols.Text=lfilout.Text="";
				int min = Convert.ToInt32(tbmin.Text);
				int max = Convert.ToInt32(tbmax.Text);
				if (min<1) min=1;
				if (max>limite) max=limite;
                IArchivoColumnas sw = new ArchivoColumnasTexto(fileout);
				for (int nr=0; nr<Math.Pow(3, partidosEnJuego); nr++)
				{
				    int qnt = htCols[nr];
				    if (qnt >= min && qnt <= max)
                    {
						string columna = n2s(nr);
						sw.GuardarCols(columna);
						tot++;
					}
				}
			    sw.Cerrar();
				lCols.Text = "" + tot;
				lfilout.Text = Path.GetFileName(fileout);
			}
			btnGrabarFiltro.Enabled=true;
			btnCargarFiltro.Enabled=true;
		}
		private void SumaCols() {
		    bSumaCols.Enabled=false;
			bSumaCols.Text="Sumando";
			int tot = 0; lCols.Text="";
			int min = Convert.ToInt32(tbmin.Text);
			int max = Convert.ToInt32(tbmax.Text);
			if (min<1) min=1;
			if (max>limite) max=limite;
			for (int nr=0; nr<Math.Pow(3, partidosEnJuego); nr++)
			{
			    int qnt = htCols[nr];
			    if (qnt >= min && qnt <= max)
                {
                    tot++;
                }
			}
		    lCols.Text=""+tot;
            bSumaCols.Text = "Sumar cols.";
			bSumaCols.Enabled=true;
		}

	    private void CargarFiltro()
        {
	        bIniciar.Enabled = false;
            btnGrabarFiltro.Enabled = false;
            OpenFileDialog abreFileIn = new OpenFileDialog();
            abreFileIn.InitialDirectory = Application.StartupPath + "/";
            abreFileIn.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
            abreFileIn.Multiselect = true;
            if (abreFileIn.ShowDialog() == DialogResult.OK)
            {
                string filein;
                
                int nf = 0;
                while (true)
                {
                      try
                      {
                          filein = abreFileIn.FileNames[nf];
                      }
                      catch { break; }
                    IArchivoColumnas aCol = new ArchivoColumnasTexto(abreFileIn.FileNames[nf]);
                    if (partidosEnJuego == 0)
                    {
                        partidosEnJuego = aCol.ObtenNumSignos();
                    }
                    else
                    {
                        if (partidosEnJuego != aCol.ObtenNumSignos())
                        {
                            MessageBox.Show("Los filtros deben tener el mismo número de partidos", "Error", MessageBoxButtons.OK);
                            return;
                        }
                    }
                    aCol.Cerrar();

                    DataRow row = dsDatos.Tables["Filtros"].NewRow();
                    row["N"] = limite + 1;
                    row["F"] = Path.GetFileName(filein);
                    row["FP"] = filein;
                    row["A"] = true;
                    row["C"] = 0;
                    dsDatos.Tables["Filtros"].Rows.Add(row);
                    limite++; nf++;
                    if (nf % 10 == 0)
                    {
                        Application.DoEvents();
                    }
                }
                AdaptarANumeroDePartidos();
            }
            btnGrabarFiltro.Enabled = true;
	        bIniciar.Enabled = true;
        }
		private void CargarLista() {
		    bIniciar.Enabled = false;
			limite = 0;
            int lineasLeidas = 0;
			dsDatos.Tables["Filtros"].Clear();
			OpenFileDialog svl = new OpenFileDialog();
			svl.InitialDirectory = "*\\";
			svl.Filter = "CargarLista(*.lst)|*.lst|Todos los archivos (*.*)|*.*" ;
			if(svl.ShowDialog() == DialogResult.OK)
            {
				string filein = svl.FileName;
				StreamReader sr = new StreamReader(filein);
                while (sr.Peek() > 0)
                {
                    filein = sr.ReadLine();
                    IArchivoColumnas aCol = new ArchivoColumnasTexto(filein);
                    int numP = aCol.ObtenNumSignos();
                    aCol.Cerrar();
                    if ((lineasLeidas == 0)&&(partidosEnJuego == 0))
                    {
                        //Es el primer archivo que se carga
                        partidosEnJuego = numP;
                    }
                    else
                    {
                        if (numP != partidosEnJuego)
                        {
                            MessageBox.Show("Los archivos no tienen el mismo número de partidos", "Error", MessageBoxButtons.OK);
                            return;
                        }
                    }
                    lineasLeidas++;

                    DataRow row = dsDatos.Tables["Filtros"].NewRow();
                    row["N"] = limite + 1;
                    row["F"] = Path.GetFileName(filein);
                    row["FP"] = filein;
                    row["A"] = true;
                    row["C"] = 0;
                    dsDatos.Tables["Filtros"].Rows.Add(row);
                    limite++;
                }
				sr.Close();
                AdaptarANumeroDePartidos();
			}
		    bIniciar.Enabled = true;
		}
		private void SalvarLista() {
			SaveFileDialog svl = new SaveFileDialog();
			svl.InitialDirectory = "*\\";
			svl.Filter = "SalvarLista(*.lst)|*.lst|Todos los archivos (*.*)|*.*" ;
            if (svl.ShowDialog() == DialogResult.OK)
            {
                string fileout = svl.FileName;
                StreamWriter sw = new StreamWriter(fileout);
                for (int nr = 0; nr < limite; nr++)
                {
                    sw.WriteLine(dataGrid1[nr, 2]);
                }
                sw.Close();
            }
		}
		private string n2s(int nx) {
			string ax = "";
		    for (int nr=0; nr<14; nr++) {
				int nx2 = nx%3; nx /= 3;
				if (nx2==1) ax = "1"+ax;
				else if (nx2==2) ax = "2"+ax;
				else ax = "X"+ax;
			}
			return ax;
		}
		private int s2n(string ax) {
			int nx = 0;
			for (int nr=0; nr<ax.Length; nr++) {
				nx *= 3;
				if (ax[nr]=='1') nx+=1;
				else if (ax[nr]=='2') nx+=2;
			}
			return nx;
		}
		private void MarcaDesmarca() {
			foreach(DataRow row in dsDatos.Tables["Filtros"].Rows) {
				if (ckMD.Checked) row["A"] = true;
				else row["A"] = false;
			}
		}

		void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CombinarFiltros));
            this.tbmax = new System.Windows.Forms.TextBox();
            this.tbmin = new System.Windows.Forms.TextBox();
            this.ckMD = new System.Windows.Forms.CheckBox();
            this.lfilout = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.bIniciar = new System.Windows.Forms.Button();
            this.bSumaCols = new System.Windows.Forms.Button();
            this.bCancelar = new System.Windows.Forms.Button();
            this.dataGrid1 = new System.Windows.Forms.DataGrid();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnGrabarFiltro = new System.Windows.Forms.Button();
            this.label = new System.Windows.Forms.Label();
            this.lCols = new System.Windows.Forms.Label();
            this.btnCargarFiltro = new System.Windows.Forms.Button();
            this.bCargaLista = new System.Windows.Forms.Button();
            this.bSalvaLista = new System.Windows.Forms.Button();
            this.btnReiniciaTodo = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tbmax
            // 
            this.tbmax.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbmax.Location = new System.Drawing.Point(78, 59);
            this.tbmax.Name = "tbmax";
            this.tbmax.Size = new System.Drawing.Size(56, 21);
            this.tbmax.TabIndex = 2;
            this.tbmax.Text = "1";
            this.tbmax.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbmin
            // 
            this.tbmin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbmin.Location = new System.Drawing.Point(21, 59);
            this.tbmin.Name = "tbmin";
            this.tbmin.Size = new System.Drawing.Size(56, 21);
            this.tbmin.TabIndex = 1;
            this.tbmin.Text = "1";
            this.tbmin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ckMD
            // 
            this.ckMD.Checked = true;
            this.ckMD.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckMD.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.ckMD.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ckMD.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckMD.Location = new System.Drawing.Point(318, 45);
            this.ckMD.Name = "ckMD";
            this.ckMD.Size = new System.Drawing.Size(124, 37);
            this.ckMD.TabIndex = 6;
            this.ckMD.Text = "Activa /  Desactiva";
            this.ckMD.CheckedChanged += new System.EventHandler(this.CkMDCheckedChanged);
            // 
            // lfilout
            // 
            this.lfilout.BackColor = System.Drawing.SystemColors.Info;
            this.lfilout.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lfilout.Location = new System.Drawing.Point(21, 175);
            this.lfilout.Name = "lfilout";
            this.lfilout.Size = new System.Drawing.Size(113, 22);
            this.lfilout.TabIndex = 7;
            this.lfilout.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(22, 45);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 11);
            this.label1.TabIndex = 5;
            this.label1.Text = "Mínimo";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(78, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(56, 11);
            this.label2.TabIndex = 6;
            this.label2.Text = "Máximo";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.SystemColors.Info;
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(448, 45);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(160, 89);
            this.label3.TabIndex = 11;
            this.label3.Text = "NOTA: Cualquier variación en el cuadro \"Filtros\" requiere repetir el proceso ante" +
                "s de sumar o grabar columnas.";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bIniciar
            // 
            this.bIniciar.BackColor = System.Drawing.Color.DarkSalmon;
            this.bIniciar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bIniciar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bIniciar.Image = ((System.Drawing.Image)(resources.GetObject("bIniciar.Image")));
            this.bIniciar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bIniciar.Location = new System.Drawing.Point(318, 183);
            this.bIniciar.Name = "bIniciar";
            this.bIniciar.Size = new System.Drawing.Size(118, 29);
            this.bIniciar.TabIndex = 9;
            this.bIniciar.Text = "Iniciar Proceso";
            this.bIniciar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bIniciar.UseVisualStyleBackColor = false;
            this.bIniciar.Click += new System.EventHandler(this.BIniciarClick);
            // 
            // bSumaCols
            // 
            this.bSumaCols.BackColor = System.Drawing.Color.DarkSalmon;
            this.bSumaCols.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bSumaCols.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bSumaCols.Image = ((System.Drawing.Image)(resources.GetObject("bSumaCols.Image")));
            this.bSumaCols.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bSumaCols.Location = new System.Drawing.Point(21, 86);
            this.bSumaCols.Name = "bSumaCols";
            this.bSumaCols.Size = new System.Drawing.Size(113, 30);
            this.bSumaCols.TabIndex = 4;
            this.bSumaCols.Text = "Sumar cols.";
            this.bSumaCols.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bSumaCols.UseVisualStyleBackColor = false;
            this.bSumaCols.Click += new System.EventHandler(this.BSumaColsClick);
            // 
            // bCancelar
            // 
            this.bCancelar.BackColor = System.Drawing.Color.DarkSalmon;
            this.bCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bCancelar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bCancelar.Image = ((System.Drawing.Image)(resources.GetObject("bCancelar.Image")));
            this.bCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bCancelar.Location = new System.Drawing.Point(318, 213);
            this.bCancelar.Name = "bCancelar";
            this.bCancelar.Size = new System.Drawing.Size(118, 29);
            this.bCancelar.TabIndex = 10;
            this.bCancelar.Text = "Cancelar";
            this.bCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bCancelar.UseVisualStyleBackColor = false;
            this.bCancelar.Click += new System.EventHandler(this.BCancelarClick);
            // 
            // dataGrid1
            // 
            this.dataGrid1.CaptionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.dataGrid1.CaptionText = "Filtros";
            this.dataGrid1.DataMember = "";
            this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dataGrid1.Location = new System.Drawing.Point(16, 15);
            this.dataGrid1.Name = "dataGrid1";
            this.dataGrid1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dataGrid1.Size = new System.Drawing.Size(296, 408);
            this.dataGrid1.TabIndex = 4;
            this.dataGrid1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dataGrid1_MouseUp);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lfilout);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.btnGrabarFiltro);
            this.groupBox2.Controls.Add(this.tbmax);
            this.groupBox2.Controls.Add(this.tbmin);
            this.groupBox2.Controls.Add(this.label);
            this.groupBox2.Controls.Add(this.lCols);
            this.groupBox2.Controls.Add(this.bSumaCols);
            this.groupBox2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(451, 147);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(157, 240);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Salida";
            // 
            // btnGrabarFiltro
            // 
            this.btnGrabarFiltro.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnGrabarFiltro.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGrabarFiltro.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGrabarFiltro.Image = ((System.Drawing.Image)(resources.GetObject("btnGrabarFiltro.Image")));
            this.btnGrabarFiltro.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGrabarFiltro.Location = new System.Drawing.Point(21, 144);
            this.btnGrabarFiltro.Name = "btnGrabarFiltro";
            this.btnGrabarFiltro.Size = new System.Drawing.Size(113, 30);
            this.btnGrabarFiltro.TabIndex = 3;
            this.btnGrabarFiltro.Text = "Grabar cols.";
            this.btnGrabarFiltro.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGrabarFiltro.UseVisualStyleBackColor = false;
            this.btnGrabarFiltro.Click += new System.EventHandler(this.BtnGrabarFiltroClick);
            // 
            // label
            // 
            this.label.Location = new System.Drawing.Point(14, 22);
            this.label.Name = "label";
            this.label.Size = new System.Drawing.Size(128, 15);
            this.label.TabIndex = 0;
            this.label.Text = "Filtros Acertados";
            this.label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lCols
            // 
            this.lCols.BackColor = System.Drawing.SystemColors.Info;
            this.lCols.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lCols.Location = new System.Drawing.Point(21, 119);
            this.lCols.Name = "lCols";
            this.lCols.Size = new System.Drawing.Size(113, 22);
            this.lCols.TabIndex = 4;
            this.lCols.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnCargarFiltro
            // 
            this.btnCargarFiltro.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnCargarFiltro.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCargarFiltro.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCargarFiltro.Image = ((System.Drawing.Image)(resources.GetObject("btnCargarFiltro.Image")));
            this.btnCargarFiltro.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCargarFiltro.Location = new System.Drawing.Point(318, 15);
            this.btnCargarFiltro.Name = "btnCargarFiltro";
            this.btnCargarFiltro.Size = new System.Drawing.Size(118, 29);
            this.btnCargarFiltro.TabIndex = 0;
            this.btnCargarFiltro.Text = "Cargar Filtro";
            this.btnCargarFiltro.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCargarFiltro.UseVisualStyleBackColor = false;
            this.btnCargarFiltro.Click += new System.EventHandler(this.BtnCargarFiltroClick);
            // 
            // bCargaLista
            // 
            this.bCargaLista.BackColor = System.Drawing.Color.DarkSalmon;
            this.bCargaLista.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bCargaLista.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bCargaLista.Image = ((System.Drawing.Image)(resources.GetObject("bCargaLista.Image")));
            this.bCargaLista.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bCargaLista.Location = new System.Drawing.Point(318, 112);
            this.bCargaLista.Name = "bCargaLista";
            this.bCargaLista.Size = new System.Drawing.Size(118, 29);
            this.bCargaLista.TabIndex = 8;
            this.bCargaLista.Text = "Cargar Lista";
            this.bCargaLista.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bCargaLista.UseVisualStyleBackColor = false;
            this.bCargaLista.Click += new System.EventHandler(this.BCargaListaClick);
            // 
            // bSalvaLista
            // 
            this.bSalvaLista.BackColor = System.Drawing.Color.DarkSalmon;
            this.bSalvaLista.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bSalvaLista.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bSalvaLista.Image = ((System.Drawing.Image)(resources.GetObject("bSalvaLista.Image")));
            this.bSalvaLista.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bSalvaLista.Location = new System.Drawing.Point(318, 82);
            this.bSalvaLista.Name = "bSalvaLista";
            this.bSalvaLista.Size = new System.Drawing.Size(118, 29);
            this.bSalvaLista.TabIndex = 7;
            this.bSalvaLista.Text = "Salvar Lista";
            this.bSalvaLista.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bSalvaLista.UseVisualStyleBackColor = false;
            this.bSalvaLista.Click += new System.EventHandler(this.BSalvaListaClick);
            // 
            // btnReiniciaTodo
            // 
            this.btnReiniciaTodo.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnReiniciaTodo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnReiniciaTodo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReiniciaTodo.Image = ((System.Drawing.Image)(resources.GetObject("btnReiniciaTodo.Image")));
            this.btnReiniciaTodo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReiniciaTodo.Location = new System.Drawing.Point(318, 355);
            this.btnReiniciaTodo.Name = "btnReiniciaTodo";
            this.btnReiniciaTodo.Size = new System.Drawing.Size(118, 30);
            this.btnReiniciaTodo.TabIndex = 12;
            this.btnReiniciaTodo.Text = "Reiniciar Todo";
            this.btnReiniciaTodo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnReiniciaTodo.UseVisualStyleBackColor = false;
            this.btnReiniciaTodo.Click += new System.EventHandler(this.btnReiniciaTodo_Click);
            // 
            // CombinarFiltros
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(632, 430);
            this.Controls.Add(this.btnReiniciaTodo);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.bCancelar);
            this.Controls.Add(this.bIniciar);
            this.Controls.Add(this.bCargaLista);
            this.Controls.Add(this.bSalvaLista);
            this.Controls.Add(this.ckMD);
            this.Controls.Add(this.dataGrid1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnCargarFiltro);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CombinarFiltros";
            this.Text = "Combinación de Filtros";
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

		}
		
		void BtnCargarFiltroClick(object sender, EventArgs e) { CargarFiltro(); }
		void BtnGrabarFiltroClick(object sender, EventArgs e) { Grabar(); }
		void BSumaColsClick(object sender, EventArgs e) { SumaCols(); }
		void CkMDCheckedChanged(object sender, EventArgs e) { MarcaDesmarca(); }
		void BSalvaListaClick(object sender, EventArgs e) { SalvarLista(); }
		void BCargaListaClick(object sender, EventArgs e) { CargarLista(); }
		void BIniciarClick(object sender, EventArgs e)
        {
            if (limite > 0)
            {
                IniciarProceso();
            }
        }
		void BCancelarClick(object sender, EventArgs e) { salida = true; }
		void dataGrid1_MouseUp(object sender, MouseEventArgs e) {
			Point pt = new Point(e.X, e.Y);
			DataGrid.HitTestInfo hti = dataGrid1.HitTest(pt);
			if(hti.Type == DataGrid.HitTestType.Cell) {
				dataGrid1.CurrentCell = new DataGridCell(hti.Row, hti.Column);
				dataGrid1.Select(hti.Row);	//seleccionamos la fila entera
				if((bool)dataGrid1[hti.Row,3] == false) {   //dataGrid1[fila,columna]
					dataGrid1[hti.Row,3] = true;
				}
				else {
					dataGrid1[hti.Row,3] = false;
				}
			}
		}

        private void btnReiniciaTodo_Click(object sender, EventArgs e)
        {
            InicializaFuenteDatos();
            partidosEnJuego = 0;
            limite = 0;
        }
		
	}
}
