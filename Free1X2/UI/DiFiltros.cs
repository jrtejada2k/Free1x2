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
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using System.Data;
using Free1X2.EntradaSalida;
using Free1X2.Utils;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI {
	public class DiFiltros : Form {
		private Button bSalvaLista;
        private TextBox tbCG;
		private Button bCargaLista;
		private Label lbCGR;
		private Button bFG;
        private Button bAnalizar;
		private GroupBox groupBox3;
		private Button bCancelar;
		private DataGrid dataGrid1;
		private Button bIniciar;
		private Label lTime;
		private Label lFileR;
		private Label lFGR;
		private CheckBox ckMD;
        private Button btnCargarFiltro;
        private Button bMenosR;
        private Button bMasR;
		private Button bGrabar;
		public DiFiltros() {
			InitializeComponent();
			InicializaColumnasGrid();
			elmeu = new Timer();
			elmeu.Interval = 3000;
			elmeu.Tick += elmeuTimer;
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
            AdaptaGrid();
		}
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
        }

        private void AdaptaGrid()
        {
            if (dsDatos.Tables.Count > 0)
            {
                dataGrid1.Enabled = (dsDatos.Tables[0].Rows.Count > 0);
            }
        }
		private int[] pot = new int[] {1,3,9,27,81,243,729,2187,6561,19683,59049,177147,531441,1594323};
		private BitArray filtro2 = new BitArray(4782969);
		private BitArray validas = new BitArray(4782969);
		private int indice, ctcols, ctFR, mincol;
		private DateTime dt0, dt9;
		private Timer elmeu;
		private bool tst0, tst1, tst2, tst3;
		private bool salida, analisis;
		private DataSet dsDatos;
		DataGridTableStyle tabla = new DataGridTableStyle();
		private int limcgsR, nrfCGR;
		private string[] colgsR = new string[3000];
		private int prm14;
		private int[] prm13 = new int[28];
		
		private void InicializaColumnasGrid() {
			tabla.MappingName = "Filtros";
		    DataGridTextBoxColumn cs = new DataGridTextBoxColumn();
			cs.MappingName = "N";
			cs.HeaderText = "N";
			cs.Width = 35;
			cs.ReadOnly = false;
			cs.Alignment = HorizontalAlignment.Center;
			tabla.GridColumnStyles.Add(cs);

            cs = new DataGridTextBoxColumn();
            cs.MappingName = "Path";
            cs.HeaderText = "";
            cs.Width = 0;
            cs.ReadOnly = true;
            cs.Alignment = HorizontalAlignment.Center;
            tabla.GridColumnStyles.Add(cs);

			cs = new DataGridTextBoxColumn();
			cs.MappingName = "F";
			cs.HeaderText = "Filtros";
			cs.Width = 85;
			cs.ReadOnly = true;
			cs.Alignment = HorizontalAlignment.Center;
			tabla.GridColumnStyles.Add(cs);
			DataGridBoolColumn csBool = new DataGridBoolColumn();
			csBool.MappingName = "A";
			csBool.HeaderText = "A";
			csBool.Width = 30;
			cs.Alignment = HorizontalAlignment.Center;
			tabla.GridColumnStyles.Add(csBool);
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "C";
			cs.HeaderText = "cols..";
			cs.Width = 60;
			cs.ReadOnly = true;
			cs.Alignment = HorizontalAlignment.Right;
			tabla.GridColumnStyles.Add(cs);
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "D";
			cs.HeaderText = "difs";
			cs.Width = 40;
			cs.ReadOnly = false;
			cs.Alignment = HorizontalAlignment.Center;
			tabla.GridColumnStyles.Add(cs);
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "M";
			cs.HeaderText = "min.";
			cs.Width = 40;
			cs.ReadOnly = false;
			cs.Alignment = HorizontalAlignment.Center;
			tabla.GridColumnStyles.Add(cs);
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "R";
			cs.HeaderText = "adm.";
			cs.Width = 60;
			cs.ReadOnly = true;
			cs.Alignment = HorizontalAlignment.Right;
			tabla.GridColumnStyles.Add(cs);
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "14";
			cs.HeaderText = "14";
			cs.Width = 30;
			cs.ReadOnly = true;
			cs.Alignment = HorizontalAlignment.Center;
			tabla.GridColumnStyles.Add(cs);
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "13";
			cs.HeaderText = "13";
			cs.Width = 30;
			cs.ReadOnly = true;
			cs.Alignment = HorizontalAlignment.Center;
			tabla.GridColumnStyles.Add(cs);
			dataGrid1.TableStyles.Clear();
			dataGrid1.TableStyles.Add(tabla);
			
			dsDatos = new DataSet();
			DataTable newTable = new DataTable("Filtros");
			newTable.Columns.Add("N", typeof(int));
            newTable.Columns.Add("Path", typeof(string));
			newTable.Columns.Add("F", typeof(string));
			newTable.Columns.Add("A", typeof(bool));
			newTable.Columns.Add("C", typeof(int));
			newTable.Columns.Add("D", typeof(string));
			newTable.Columns.Add("M", typeof(int));
			newTable.Columns.Add("R", typeof(int));
			newTable.Columns.Add("14", typeof(int));
			newTable.Columns.Add("13", typeof(int));
			dsDatos.Tables.Add(newTable);
			dataGrid1.SetDataBinding(dsDatos, "Filtros");
		}
		private void Calcular(int nf) {
			mincol = (int)dataGrid1[nf,6];
			if (mincol<1) mincol=1; if (mincol>3305) mincol=3305;
			string tmp = (string)dataGrid1[nf,5];
			tst0 = (tmp[0]=='1'?true:false);
			tst1 = (tmp[1]=='1'?true:false);
			tst2 = (tmp[2]=='1'?true:false);
			tst3 = (tmp[3]=='1'?true:false);
			for (int nc=0; nc<4782969; nc++) {
				Application.DoEvents();
				if (salida) break;
				if (validas[nc] && !Valida(nc)) {
					validas[nc]=false; ctFR--;
				}
			}
			dataGrid1[nf,7] = ctFR;
		}
		private bool Valida(int nsel) {
		    
			if (filtro2[nsel] && tst0) return true;
			if (tst1 || tst2 || tst3) {
                int na12, na11;
                int na13 = na12=na11=0;
				for (int nr=0; nr<14; nr++)
				{
				    int sign1 = (nsel / pot[nr]) % 3;
				    for (int z1=0; z1<3; z1++) {
						if (z1 == sign1) continue;
						int col1 = nsel + pot[nr] * (z1 - sign1);
						if (filtro2[col1] && tst1) na13++;
						if (tst2 || tst3) {
							for (int nr2=nr+1; nr2<14; nr2++)
							{
							    int sign2 = (col1 / pot[nr2]) % 3;
							    for (int z2=0; z2<3; z2++) {
									if (z2 == sign2) continue;
									int col2 = col1 + pot[nr2] * (z2 - sign2);
									if (filtro2[col2] && tst2) na12++;
									if (tst3) {
										for (int nr3=nr2+1; nr3<14; nr3++)
										{
										    int sign3 = (col2 / pot[nr3]) % 3;
										    for (int z3=0; z3<3; z3++) {
												if (z3 == sign3) continue;
												int col3 = col2 + pot[nr3] * (z3 - sign3);
												if (filtro2[col3]) na11++;
											}
										}
									    if ((na11+na12+na13)>=mincol) return true;
									}
								}
							}
						    if ((na11+na12+na13)>=mincol) return true;
						}
					}
				}
			    if ((na11+na12+na13)>=mincol) return true;
			}
			return false;
		}
		private void Grabar() {
			bGrabar.Enabled = false;
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.InitialDirectory = "Filtros\\" ;
			saveFileDialog.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			if(saveFileDialog.ShowDialog() == DialogResult.OK) {
				string fileout = Path.GetFileName(saveFileDialog.FileName);
                IArchivoColumnas sw = new ArchivoColumnasTexto(saveFileDialog.FileName);
				sw.GuardarTodasCols(validas);
				
				sw.Cerrar();
				lFileR.Text = fileout;
			}
			bGrabar.Enabled = true;
		}
		private void veureelmeu() {
			dt9 = DateTime.Now;
			string temp = (dt9-dt0)+"0000000000";
			lTime.Text = temp.Substring(0,10);
		}
		private void MarcaDesmarca() {
			foreach(DataRow row in dsDatos.Tables["Filtros"].Rows) {
				if (ckMD.Checked) row["A"] = true;
				else row["A"] = false;
			}
		}
		private void CargarFiltro() {
			string filein;
			btnCargarFiltro.Enabled=false;
			bGrabar.Enabled=false;
			OpenFileDialog abreFileIn = new OpenFileDialog();
			abreFileIn.InitialDirectory = Application.StartupPath + "/";
			abreFileIn.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
			abreFileIn.Multiselect=true;
			if(abreFileIn.ShowDialog() == DialogResult.OK) {
				int nf=0;
				while (true) {
                    try
                    {
                        filein = abreFileIn.FileNames[nf++];
                    }
                    catch { break; }
					int nl = dsDatos.Tables["Filtros"].Rows.Count;
					DataRow row = dsDatos.Tables["Filtros"].NewRow();
					row["N"] = nl+1;
                    row["Path"] = filein;
					row["F"] = Path.GetFileName(filein);
					row["A"] = true;
					row["C"] = 0;
					row["D"] = "1111";
					row["M"] = 1;
					dsDatos.Tables["Filtros"].Rows.Add(row);
				}
			}
			btnCargarFiltro.Enabled=true;
			bGrabar.Enabled=true;
            AdaptaGrid();
		}
		private void CargarLista() {
		    dsDatos.Tables["Filtros"].Clear();
			OpenFileDialog svl = new OpenFileDialog();
			svl.InitialDirectory = "*\\";
			svl.Filter = "CargarLista(*.lst)|*.lst|Todos los archivos (*.*)|*.*" ;
			if(svl.ShowDialog() == DialogResult.OK) {
			    StreamReader sr = new StreamReader(svl.FileName);
				int limite=1;
				while (sr.Peek()>0) {
					string tmp = sr.ReadLine();
					string[] aux = tmp.Split(';');
					DataRow row = dsDatos.Tables["Filtros"].NewRow();
					row["N"] = limite++;
                    row["Path"] = aux[0];
					row["F"] = aux[1];
					row["A"] = aux[2];
					row["C"] = 0;
					row["D"] = aux[3];
					row["M"] = aux[4];
					row["R"] = 0;
					row["14"] = 0;
					row["13"] = 0;
					dsDatos.Tables["Filtros"].Rows.Add(row);
				}
				sr.Close();
			}
            AdaptaGrid();
		}
		private void SalvarLista() {
		    SaveFileDialog svl = new SaveFileDialog();
			svl.InitialDirectory = "*\\";
			svl.Filter = "SalvarLista(*.lst)|*.lst|Todos los archivos (*.*)|*.*" ;
			if(svl.ShowDialog() == DialogResult.OK) {
				string fileout = Path.GetFileName(svl.FileName);
				StreamWriter sw = new StreamWriter(fileout);
				int nl = dsDatos.Tables["Filtros"].Rows.Count;
				for (int nr=0; nr<nl; nr++) {
                    string linea = "" + dataGrid1[nr, 1] + ";" + dataGrid1[nr, 2] + ";" + dataGrid1[nr, 3];
					linea += ";"+dataGrid1[nr,5]+";"+dataGrid1[nr,6];
					sw.WriteLine(linea);
				}
				sw.Close();
			}
		}
		private void IniciarProceso () {
		    bIniciar.Enabled = false;
			btnCargarFiltro.Enabled = false;
			bCargaLista.Enabled = false;
			bSalvaLista.Enabled = false;
			bGrabar.Enabled = false;
			ckMD.Enabled = false;
			salida = false;
			tabla.AllowSorting = false;
			elmeu.Start(); dt0 = DateTime.Now;
            IArchivoColumnas sr;

			validas.SetAll(false);
			for (int nf=0; nf < dsDatos.Tables["Filtros"].Rows.Count; nf++) {
				if (salida) break;
				try { dataGrid1[nf,4] = 0; }
				catch { break; }
				if ((bool)dataGrid1[nf,3]) {
					string filein = (string) dataGrid1[nf,1];
                    try
                    {
                        sr = new ArchivoColumnasTexto(filein);
                    }
                    catch
                    {
                        dataGrid1[nf, 3] = false;
                        continue;
                    }
					filtro2.SetAll(false); ctcols=0;
					while (sr.SiguienteColumna())
					{
						if (salida) break;
						string columna = sr.LeeColumnaSinComas();
						ctcols++;
						if (columna.Length < 14) {
							MessageBox.Show ("error de longitud");
							break;
						}
						indice = s2n(columna);
						if (nf==0) validas[indice]=true;
						else filtro2[indice]=true;
						Application.DoEvents();
					}
					sr.Cerrar();
					dataGrid1[nf,4] = ctcols;
					if (nf==0) { dataGrid1[nf,7] = ctFR = ctcols; }
					else Calcular(nf);
				}
				if (analisis) Escrutar(nf);
			}
			elmeu.Stop(); veureelmeu();
			tabla.AllowSorting = true;
			bIniciar.Enabled = true;
			btnCargarFiltro.Enabled = true;
			bCargaLista.Enabled = true;
			bSalvaLista.Enabled = true;
			bGrabar.Enabled = true;
			ckMD.Enabled = true;
		}

	    private int s2n(string ax) {
            ConvertidorDeBases conv = new ConvertidorDeBases();
            return conv.ConvColumnaANumero(ax);
		}
		private void Escrutar(int nf) {
			int ct13 = 0;
			dataGrid1[nf,8] = (validas[prm14]?1:0);
			for (int nr=0; nr<28; nr++)
			{
			    int n = prm13[nr];
			    if (validas[n]) ct13++;
			}
		    dataGrid1[nf,9] = ct13;
		}
		private void Analizar() {
			int idx=0;
			bAnalizar.Enabled = false;
			analisis = true;
			prm14 = s2n(tbCG.Text);
			for (int nr=0; nr<14; nr++)
			{
			    int sign1 = (prm14 / pot[nr]) % 3;
			    for (int z1=0; z1<3; z1++) {
					if (z1 == sign1) continue;
					int col1 = prm14 + pot[nr] * (z1 - sign1);
					prm13[idx] = col1; idx++;
				}
			}
		    IniciarProceso();
			analisis = false;
			bAnalizar.Enabled = true;
		}
		private string VerColumna (string columna) {
			string chval = "12xX";
		    string xcol = columna.Trim();
			if (xcol.Length<14) return "";
			xcol = xcol.Substring(0,14);
			for (int nr=0; nr<14; nr++)
			{
			    char ch = xcol[nr];
			    if (chval.IndexOf(ch)<0) return "";
			}
		    return xcol;
		}
		private void EntraCGsR() {
			string tmp;
			OpenFileDialog cgDialog = new OpenFileDialog();
			cgDialog.InitialDirectory = Application.StartupPath + "/";
			cgDialog.Filter = "F.Ganadoras(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			if(cgDialog.ShowDialog() == DialogResult.OK) {
				string filein = Path.GetFileName(cgDialog.FileName);
				limcgsR = 0;
                StreamReader sr = new StreamReader(cgDialog.FileName);
				while (sr.Peek()>0) {
					tmp = VerColumna(sr.ReadLine());
					if (tmp.Length==0) { MessageBox.Show("col.G. errónea"); return; }
					colgsR[limcgsR] = tmp;
					limcgsR++;
					Application.DoEvents();
				}
				sr.Close();
				nrfCGR = limcgsR; lFGR.Text = filein;
				lbCGR.Text=""+nrfCGR; tbCG.Text=colgsR[nrfCGR-1];
				bAnalizar.Enabled = true;
			}
		}
		private void GRMas() {
			if (nrfCGR<limcgsR) {
				nrfCGR++;
				lbCGR.Text=""+nrfCGR; tbCG.Text=colgsR[nrfCGR-1];
			}
		}
		private void GRMenos() {
			if (nrfCGR>1) {
				nrfCGR--;
				lbCGR.Text=""+nrfCGR; tbCG.Text=colgsR[nrfCGR-1];
			}
		}
		
		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DiFiltros));
            this.bGrabar = new System.Windows.Forms.Button();
            this.btnCargarFiltro = new System.Windows.Forms.Button();
            this.ckMD = new System.Windows.Forms.CheckBox();
            this.lFGR = new System.Windows.Forms.Label();
            this.lFileR = new System.Windows.Forms.Label();
            this.lTime = new System.Windows.Forms.Label();
            this.bIniciar = new System.Windows.Forms.Button();
            this.dataGrid1 = new System.Windows.Forms.DataGrid();
            this.bCancelar = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tbCG = new System.Windows.Forms.TextBox();
            this.bFG = new System.Windows.Forms.Button();
            this.lbCGR = new System.Windows.Forms.Label();
            this.bMenosR = new System.Windows.Forms.Button();
            this.bMasR = new System.Windows.Forms.Button();
            this.bAnalizar = new System.Windows.Forms.Button();
            this.bCargaLista = new System.Windows.Forms.Button();
            this.bSalvaLista = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // bGrabar
            // 
            this.bGrabar.BackColor = System.Drawing.Color.DarkSalmon;
            this.bGrabar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bGrabar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bGrabar.Image = ((System.Drawing.Image)(resources.GetObject("bGrabar.Image")));
            this.bGrabar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bGrabar.Location = new System.Drawing.Point(536, 243);
            this.bGrabar.Name = "bGrabar";
            this.bGrabar.Size = new System.Drawing.Size(134, 30);
            this.bGrabar.TabIndex = 11;
            this.bGrabar.Text = "Grabar";
            this.bGrabar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bGrabar.UseVisualStyleBackColor = false;
            this.bGrabar.Click += new System.EventHandler(this.bGrabarClick);
            // 
            // btnCargarFiltro
            // 
            this.btnCargarFiltro.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnCargarFiltro.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCargarFiltro.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCargarFiltro.Image = ((System.Drawing.Image)(resources.GetObject("btnCargarFiltro.Image")));
            this.btnCargarFiltro.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCargarFiltro.Location = new System.Drawing.Point(536, 7);
            this.btnCargarFiltro.Name = "btnCargarFiltro";
            this.btnCargarFiltro.Size = new System.Drawing.Size(134, 30);
            this.btnCargarFiltro.TabIndex = 36;
            this.btnCargarFiltro.Text = "Cargar Filtro";
            this.btnCargarFiltro.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCargarFiltro.UseVisualStyleBackColor = false;
            this.btnCargarFiltro.Click += new System.EventHandler(this.BtnCargarFiltroClick);
            // 
            // ckMD
            // 
            this.ckMD.Checked = true;
            this.ckMD.CheckState = System.Windows.Forms.CheckState.Checked;
            this.ckMD.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckMD.Location = new System.Drawing.Point(536, 43);
            this.ckMD.Name = "ckMD";
            this.ckMD.Size = new System.Drawing.Size(134, 30);
            this.ckMD.TabIndex = 37;
            this.ckMD.Text = "Activa / Desactiva";
            this.ckMD.CheckedChanged += new System.EventHandler(this.CkMDCheckedChanged);
            // 
            // lFGR
            // 
            this.lFGR.BackColor = System.Drawing.SystemColors.Info;
            this.lFGR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lFGR.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lFGR.Location = new System.Drawing.Point(40, 22);
            this.lFGR.Name = "lFGR";
            this.lFGR.Size = new System.Drawing.Size(144, 23);
            this.lFGR.TabIndex = 88;
            this.lFGR.Text = "Fichero ganadoras";
            this.lFGR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lFileR
            // 
            this.lFileR.BackColor = System.Drawing.SystemColors.Info;
            this.lFileR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lFileR.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lFileR.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lFileR.Location = new System.Drawing.Point(536, 276);
            this.lFileR.Name = "lFileR";
            this.lFileR.Size = new System.Drawing.Size(134, 21);
            this.lFileR.TabIndex = 42;
            this.lFileR.Text = "Fichero";
            this.lFileR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lTime
            // 
            this.lTime.BackColor = System.Drawing.SystemColors.Info;
            this.lTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lTime.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lTime.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lTime.Location = new System.Drawing.Point(536, 181);
            this.lTime.Name = "lTime";
            this.lTime.Size = new System.Drawing.Size(134, 21);
            this.lTime.TabIndex = 17;
            this.lTime.Text = "Tiempo";
            this.lTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bIniciar
            // 
            this.bIniciar.BackColor = System.Drawing.Color.DarkSalmon;
            this.bIniciar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bIniciar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bIniciar.Image = ((System.Drawing.Image)(resources.GetObject("bIniciar.Image")));
            this.bIniciar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bIniciar.Location = new System.Drawing.Point(536, 150);
            this.bIniciar.Name = "bIniciar";
            this.bIniciar.Size = new System.Drawing.Size(134, 30);
            this.bIniciar.TabIndex = 40;
            this.bIniciar.Text = "Iniciar Proceso";
            this.bIniciar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bIniciar.UseVisualStyleBackColor = false;
            this.bIniciar.Click += new System.EventHandler(this.BIniciarClick);
            // 
            // dataGrid1
            // 
            this.dataGrid1.CaptionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.dataGrid1.CaptionText = "Filtros";
            this.dataGrid1.DataMember = "";
            this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dataGrid1.Location = new System.Drawing.Point(8, 7);
            this.dataGrid1.Name = "dataGrid1";
            this.dataGrid1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.dataGrid1.Size = new System.Drawing.Size(472, 424);
            this.dataGrid1.TabIndex = 35;
            this.dataGrid1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.dataGrid1_MouseUp);
            // 
            // bCancelar
            // 
            this.bCancelar.BackColor = System.Drawing.Color.DarkSalmon;
            this.bCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bCancelar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bCancelar.Image = ((System.Drawing.Image)(resources.GetObject("bCancelar.Image")));
            this.bCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bCancelar.Location = new System.Drawing.Point(536, 203);
            this.bCancelar.Name = "bCancelar";
            this.bCancelar.Size = new System.Drawing.Size(134, 30);
            this.bCancelar.TabIndex = 41;
            this.bCancelar.Text = "Cancelar";
            this.bCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bCancelar.UseVisualStyleBackColor = false;
            this.bCancelar.Click += new System.EventHandler(this.BCancelarClick);
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.Bisque;
            this.groupBox3.Controls.Add(this.tbCG);
            this.groupBox3.Controls.Add(this.lFGR);
            this.groupBox3.Controls.Add(this.bFG);
            this.groupBox3.Controls.Add(this.lbCGR);
            this.groupBox3.Controls.Add(this.bMenosR);
            this.groupBox3.Controls.Add(this.bMasR);
            this.groupBox3.Controls.Add(this.bAnalizar);
            this.groupBox3.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(496, 306);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(200, 119);
            this.groupBox3.TabIndex = 363;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Analisis resultados";
            // 
            // tbCG
            // 
            this.tbCG.BackColor = System.Drawing.SystemColors.Window;
            this.tbCG.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbCG.Location = new System.Drawing.Point(40, 46);
            this.tbCG.MaxLength = 14;
            this.tbCG.Name = "tbCG";
            this.tbCG.Size = new System.Drawing.Size(144, 23);
            this.tbCG.TabIndex = 373;
            this.tbCG.Text = "Col. Ganadora";
            this.tbCG.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bFG
            // 
            this.bFG.BackColor = System.Drawing.Color.DarkSalmon;
            this.bFG.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bFG.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bFG.Image = ((System.Drawing.Image)(resources.GetObject("bFG.Image")));
            this.bFG.Location = new System.Drawing.Point(8, 22);
            this.bFG.Name = "bFG";
            this.bFG.Size = new System.Drawing.Size(24, 23);
            this.bFG.TabIndex = 87;
            this.bFG.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bFG.UseVisualStyleBackColor = false;
            this.bFG.Click += new System.EventHandler(this.BFGClick);
            // 
            // lbCGR
            // 
            this.lbCGR.BackColor = System.Drawing.SystemColors.Info;
            this.lbCGR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbCGR.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCGR.Location = new System.Drawing.Point(128, 74);
            this.lbCGR.Name = "lbCGR";
            this.lbCGR.Size = new System.Drawing.Size(32, 31);
            this.lbCGR.TabIndex = 86;
            this.lbCGR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bMenosR
            // 
            this.bMenosR.BackColor = System.Drawing.Color.DarkSalmon;
            this.bMenosR.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bMenosR.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bMenosR.Location = new System.Drawing.Point(161, 90);
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
            this.bMasR.Location = new System.Drawing.Point(161, 74);
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
            this.bAnalizar.Image = ((System.Drawing.Image)(resources.GetObject("bAnalizar.Image")));
            this.bAnalizar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bAnalizar.Location = new System.Drawing.Point(14, 74);
            this.bAnalizar.Name = "bAnalizar";
            this.bAnalizar.Size = new System.Drawing.Size(108, 30);
            this.bAnalizar.TabIndex = 27;
            this.bAnalizar.Text = "Analizar";
            this.bAnalizar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bAnalizar.UseVisualStyleBackColor = false;
            this.bAnalizar.Click += new System.EventHandler(this.BAnalizarClick);
            // 
            // bCargaLista
            // 
            this.bCargaLista.BackColor = System.Drawing.Color.DarkSalmon;
            this.bCargaLista.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bCargaLista.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bCargaLista.Image = ((System.Drawing.Image)(resources.GetObject("bCargaLista.Image")));
            this.bCargaLista.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bCargaLista.Location = new System.Drawing.Point(536, 110);
            this.bCargaLista.Name = "bCargaLista";
            this.bCargaLista.Size = new System.Drawing.Size(134, 30);
            this.bCargaLista.TabIndex = 39;
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
            this.bSalvaLista.Location = new System.Drawing.Point(536, 79);
            this.bSalvaLista.Name = "bSalvaLista";
            this.bSalvaLista.Size = new System.Drawing.Size(134, 30);
            this.bSalvaLista.TabIndex = 38;
            this.bSalvaLista.Text = "Salvar Lista";
            this.bSalvaLista.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bSalvaLista.UseVisualStyleBackColor = false;
            this.bSalvaLista.Click += new System.EventHandler(this.BSalvaListaClick);
            // 
            // DiFiltros
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(712, 464);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.lFileR);
            this.Controls.Add(this.bCancelar);
            this.Controls.Add(this.bIniciar);
            this.Controls.Add(this.bCargaLista);
            this.Controls.Add(this.bSalvaLista);
            this.Controls.Add(this.ckMD);
            this.Controls.Add(this.btnCargarFiltro);
            this.Controls.Add(this.dataGrid1);
            this.Controls.Add(this.lTime);
            this.Controls.Add(this.bGrabar);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DiFiltros";
            this.Text = "Diferencias entre filtros";
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion
		
		void BtnCargarFiltroClick(object sender, EventArgs e) { CargarFiltro(); }
		void CkMDCheckedChanged(object sender, EventArgs e) { MarcaDesmarca(); }
		void BSalvaListaClick(object sender, EventArgs e) { SalvarLista(); }
		void BCargaListaClick(object sender, EventArgs e) { CargarLista(); }
		void BIniciarClick(object sender, EventArgs e) { IniciarProceso(); }
		void BCancelarClick(object sender, EventArgs e) { salida=true; }
		void bGrabarClick(object sender, EventArgs e) { Grabar(); }
		void elmeuTimer(object sender, EventArgs e) { veureelmeu(); }
		void dataGrid1_MouseUp(object sender, MouseEventArgs e) {
			Point pt = new Point(e.X, e.Y);
			DataGrid.HitTestInfo hti = dataGrid1.HitTest(pt);
			if(hti.Type == DataGrid.HitTestType.Cell && hti.Row <= dsDatos.Tables[0].Rows.Count - 1) {
				dataGrid1.CurrentCell = new DataGridCell(hti.Row, hti.Column);
				if((bool)dataGrid1[hti.Row,3] == false) {   //dataGrid1[fila,columna]
					dataGrid1[hti.Row,3] = true;
				}
				else {
					dataGrid1[hti.Row,3] = false;
				}
			}
		}
		void BAnalizarClick(object sender, EventArgs e) { Analizar(); }
		void BFGClick(object sender, EventArgs e) { EntraCGsR(); }
		void BMasRClick(object sender, EventArgs e) { GRMas(); }
		void BMenosRClick(object sender, EventArgs e) { GRMenos(); }
	}
}
