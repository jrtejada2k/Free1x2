using System;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;

namespace Free1X2.UI.Controls {
	public class valors : UserControl {
		private Button bSalvaVal;
		private DataGrid dataGrid1;
		private Button bLeeVal;
		private Label lgrval;
		private Button bMenos;
		private Button bPaste;
		private Button bLeeGrupo;
		private Button bSalvaGrupo;
		private Button bMas;
		private Label lugar;
		private Button bCut;
        private int noPartidos;
        private double[,] GrVals = new double[1000,42];

		public valors() {
            try
            {
                noPartidos = VariablesGlobales.NumeroPartidos;
            }
            catch
            {
                noPartidos = 14;
            }
            GrVals = new double[1000, noPartidos * 3];
			InitializeComponent();
			InitGrid();

		}
		
		private ArrayList Valors;
		
		private string[] NomVals = new string[1000];
		private int ngrval, limgrval;
		
		public event EventHandler CrearCPs;
		private void InitGrid() {
            DataGridTableStyle tableStyle = new DataGridTableStyle();
            tableStyle.MappingName = "ArrayList";
            tableStyle.ColumnHeadersVisible = true;

		    //		unos
            DataGridTextBoxColumn cs = new DataGridTextBoxColumn();
            cs.MappingName = "Uno";
            cs.HeaderText = "  1";
            cs.Width = 40;
            tableStyle.GridColumnStyles.Add(cs);

            //		equis
            cs = new DataGridTextBoxColumn();
            cs.MappingName = "Equis";
            cs.HeaderText = "  X";
            cs.Width = 40;
            tableStyle.GridColumnStyles.Add(cs);

            //		doses
            cs = new DataGridTextBoxColumn();
            cs.MappingName = "Dos";
            cs.HeaderText = "  2";
            cs.Width = 40;
            tableStyle.GridColumnStyles.Add(cs);

            dataGrid1.TableStyles.Add(tableStyle);
			Valors = new ArrayList();
            for (int nr = 0; nr < noPartidos; nr++)
            {
                Valors.Add(new pct(50, 30, 20));
            }
			dataGrid1.DataSource = Valors;
		}
		private void LeeVals () {
            string[] buff = new string[50];
			int nlin = 0; string tmp;
			OpenFileDialog abreValIn = new OpenFileDialog();
			abreValIn.InitialDirectory = Application.StartupPath + "/";
			abreValIn.Filter = "F.Entrada(*.txt)|*.txt|Todos los archivos(*.*)|*.*";
			if(abreValIn.ShowDialog() == DialogResult.OK) {
				string filein = Path.GetFileName(abreValIn.FileName);
				StreamReader srv = new StreamReader(filein);
				while (srv.Peek()>0) {
					tmp = srv.ReadLine().Trim();
					if (tmp.Length>0) {
						buff[nlin] = tmp;
						nlin++;
					}
				}
				srv.Close();
				string[] aux = Rellenar42(nlin, buff);
				if (aux.Length!=42) MessageBox.Show ("error en fichero de valoraciones");
				else {
					lugar.Text = NomVals[limgrval] = filein;
					for (int nr=0; nr<42; nr++) GrVals[limgrval,nr]=Convert.ToDouble(aux[nr]);
					limgrval++; ngrval=limgrval; lgrval.Text=""+ngrval+"/"+limgrval;
					PintaGrid(aux);
				}
			}
			OnCrearCPs(EventArgs.Empty);
		}
		private void PintaGrid(string[] aux){
			for (int nr1=0; nr1<VariablesGlobales.NumeroPartidos; nr1++) {
				for (int nr2=0; nr2<3; nr2++) {
					dataGrid1[nr1,nr2] = Convert.ToDouble(aux[nr1*3+nr2]);
				}
			}
		}
		private string[] Rellenar42(int nlin, string[]buff) {
			string[] aux=new string[42], clin;
			int ir=0; char tab=(char) 9, bln=(char) 32, com=',';
			if (nlin==1) {
				clin = buff[0].Trim().Split(bln);
				if (clin.Length!=42) clin = buff[0].Split(tab);
				if (clin.Length!=42) clin = buff[0].Split(com);
				if (clin.Length==42) {
					for (int nr=0; nr<42; nr++) {
						aux[nr] = clin[nr].Replace('.',',');
					}
				}
			}
            else if (nlin == 14 || nlin == 15 || nlin == 16)
            {
				for (int nr=0; nr<VariablesGlobales.NumeroPartidos; nr++) {
					clin = buff[nr].Trim().Split(bln);
					if (clin.Length!=3) clin = buff[nr].Split(tab);
					if (clin.Length!=3) clin = buff[nr].Split(com);
					if (clin.Length!=3) break;
					aux[ir]=clin[0].Replace('.',','); ir++;
					aux[ir]=clin[1].Replace('.',','); ir++;
					aux[ir]=clin[2].Replace('.',','); ir++;
				}
			}
			else if (nlin==42) {
				for (int nr=0; nr<42; nr++) aux[nr]=buff[nr].Trim().Replace('.',',');
			}
		    for (int nr=0; nr<42; nr++)
		    {
		        string tmp = aux[nr].Trim();
		        if (tmp=="0") aux[nr]="0,1"; else aux[nr]=tmp;
		    }
		    return aux;
		}
		private void SalvaVals() {
		    char sep = (char) 9;
			SaveFileDialog saveValDialog = new SaveFileDialog();
			saveValDialog.InitialDirectory = "*\\";
			saveValDialog.Filter = "F.Salida(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			if(saveValDialog.ShowDialog() == DialogResult.OK) {
				string archVal = Path.GetFileName(saveValDialog.FileName);
				StreamWriter sw = new StreamWriter(archVal);
				for (int nr=0; nr<14; nr++)
					sw.WriteLine(""+dataGrid1[nr,0]+sep+dataGrid1[nr,1]+sep+dataGrid1[nr,2]);
				sw.Close();
				lugar.Text = NomVals[limgrval-1] = archVal;
			}
		}
		public double[,] RetVals() {
			double[,] aVals = new double[VariablesGlobales.NumeroPartidos,3];
			for (int nr1=0; nr1<VariablesGlobales.NumeroPartidos; nr1++) for (int nr2=0; nr2<3; nr2++)
				aVals[nr1,nr2] = (double)dataGrid1[nr1,nr2];
			return aVals;
		}
		public void HaciaPortapapeles() {
			char tab = (char) 9; char LF = (char) 10; char NL = (char) 13;
			string cadena = "";
			for(int i=0; i<VariablesGlobales.NumeroPartidos; i++)
				cadena += ""+dataGrid1[i,0]+tab+dataGrid1[i,1]+tab+dataGrid1[i,2]+NL+LF;
			Clipboard.SetDataObject (cadena, true);
		}
		public void DesdePortapapeles() {
			char LF = (char) 10; char NL = (char) 13;
		    string[] buff=new string[50];
		    IDataObject iData = Clipboard.GetDataObject();
			if (iData.GetDataPresent(DataFormats.Text)) {
				string cadena = (String)iData.GetData(DataFormats.Text);
				cadena = cadena.Replace(NL,' ');
				string[] aux = cadena.Split(LF); 
                int nr2 = aux.Length; 
                int nlin = 0;
				for (int nr=0; nr<nr2; nr++)
				{
				    string tmp = aux[nr].Trim();
				    if (tmp.Length>0) {
						buff[nlin]=tmp; nlin++;
					}
				}
			    aux = Rellenar42(nlin, buff);
				if (aux.Length!=42) MessageBox.Show ("error en fichero de valoraciones");
				else {
					lugar.Text = NomVals[limgrval] = "portapapeles";
					for (int nr=0; nr<42; nr++) GrVals[limgrval,nr]=Convert.ToDouble(aux[nr]);
					limgrval++; ngrval=limgrval; lgrval.Text=""+ngrval+"/"+limgrval;
					PintaGrid(aux);
				}
				OnCrearCPs(EventArgs.Empty);
			}
		}
		private void LeeGrupo() {
			string[] aux=new string[43];
			char tab = (char) 9;
			string buff;
			OpenFileDialog abreValIn = new OpenFileDialog();
			abreValIn.InitialDirectory = Application.StartupPath + "/";
			abreValIn.Filter = "F.Entrada(*.gval)|*.gval|Todos los archivos(*.*)|*.*";
			if(abreValIn.ShowDialog() == DialogResult.OK) {
				string filein = Path.GetFileName(abreValIn.FileName);
				StreamReader srv = new StreamReader(filein);
				limgrval=0;
				while (srv.Peek()>0) {
					buff = srv.ReadLine();
					aux = buff.Split(tab);
					NomVals[limgrval] = aux[0];
					for (int nr=0; nr<42; nr++) 
						GrVals[limgrval,nr] = Convert.ToDouble(aux[nr+1]);
					limgrval++;
				}
				srv.Close();
				ngrval=1;
				for (int nr=0; nr<42; nr++) aux[nr] = ""+GrVals[0,nr];
				lugar.Text = NomVals[0];
				lgrval.Text=""+ngrval+"/"+limgrval;
				PintaGrid(aux);
				OnCrearCPs(EventArgs.Empty);
			}
		}
		private void SalvaGrupo() {
		    char tab = (char) 9;
			SaveFileDialog saveValDialog = new SaveFileDialog();
			saveValDialog.InitialDirectory = "*\\";
			saveValDialog.Filter = "F.Salida(*.gval)|*.gval|Todos los archivos (*.*)|*.*" ;
			if(saveValDialog.ShowDialog() == DialogResult.OK) {
				string fileout = Path.GetFileName(saveValDialog.FileName);
				StreamWriter sw = new StreamWriter(fileout);
				for (int nr=0; nr<limgrval; nr++) {
					string tmp = NomVals[nr];
					for (int nr2=0; nr2<42; nr2++) { tmp+=tab; tmp+=GrVals[nr,nr2]; }
					sw.WriteLine(tmp);
				}
				sw.Close();
			}
		}
		private void GRMas() {
			string[] aux = new string[42];
			if (ngrval<limgrval) {
				ngrval++; lgrval.Text=""+ngrval+"/"+limgrval;
				for (int nr=0; nr<42; nr++) aux[nr]=""+GrVals[ngrval-1,nr];
				PintaGrid(aux); lugar.Text = NomVals[ngrval-1];
				OnCrearCPs(EventArgs.Empty);
			}
		}
		private void GRMenos() {
			string[] aux = new string[42];
			if (ngrval>1) {
				ngrval--;  lgrval.Text=""+ngrval+"/"+limgrval;
				for (int nr=0; nr<42; nr++) aux[nr]=""+GrVals[ngrval-1,nr];
				PintaGrid(aux); lugar.Text = NomVals[ngrval-1];
				OnCrearCPs(EventArgs.Empty);
			}
		}
		
		private class pct {
			private double uno, equis, dos;
			public pct(double v1, double vx, double v2) {
				uno = v1; equis = vx; dos = v2;
			}
			public double Uno {
				get{ return uno; }
				set{ uno = value;}
			}
			public double Equis {
				get{ return equis; }
				set{ equis = value;}
			}
			public double Dos {
				get{ return dos; }
				set{ dos = value;}
			}
		}
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(valors));
            this.bCut = new System.Windows.Forms.Button();
            this.lugar = new System.Windows.Forms.Label();
            this.bMas = new System.Windows.Forms.Button();
            this.bSalvaGrupo = new System.Windows.Forms.Button();
            this.bLeeGrupo = new System.Windows.Forms.Button();
            this.bPaste = new System.Windows.Forms.Button();
            this.bMenos = new System.Windows.Forms.Button();
            this.lgrval = new System.Windows.Forms.Label();
            this.bLeeVal = new System.Windows.Forms.Button();
            this.dataGrid1 = new System.Windows.Forms.DataGrid();
            this.bSalvaVal = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).BeginInit();
            this.SuspendLayout();
            // 
            // bCut
            // 
            this.bCut.BackColor = System.Drawing.Color.DarkSalmon;
            this.bCut.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bCut.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bCut.Image = ((System.Drawing.Image)(resources.GetObject("bCut.Image")));
            this.bCut.Location = new System.Drawing.Point(128, 378);
            this.bCut.Name = "bCut";
            this.bCut.Size = new System.Drawing.Size(24, 24);
            this.bCut.TabIndex = 46;
            this.bCut.UseVisualStyleBackColor = false;
            this.bCut.Click += new System.EventHandler(this.BCutClick);
            // 
            // lugar
            // 
            this.lugar.BackColor = System.Drawing.SystemColors.Info;
            this.lugar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lugar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lugar.Location = new System.Drawing.Point(8, 346);
            this.lugar.Name = "lugar";
            this.lugar.Size = new System.Drawing.Size(152, 24);
            this.lugar.TabIndex = 47;
            this.lugar.Text = "Fichero";
            this.lugar.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bMas
            // 
            this.bMas.BackColor = System.Drawing.Color.DarkSalmon;
            this.bMas.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bMas.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bMas.Location = new System.Drawing.Point(88, 449);
            this.bMas.Name = "bMas";
            this.bMas.Size = new System.Drawing.Size(16, 16);
            this.bMas.TabIndex = 87;
            this.bMas.Text = "+";
            this.bMas.UseVisualStyleBackColor = false;
            this.bMas.Click += new System.EventHandler(this.BMasClick);
            // 
            // bSalvaGrupo
            // 
            this.bSalvaGrupo.BackColor = System.Drawing.Color.DarkSalmon;
            this.bSalvaGrupo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bSalvaGrupo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bSalvaGrupo.Image = ((System.Drawing.Image)(resources.GetObject("bSalvaGrupo.Image")));
            this.bSalvaGrupo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bSalvaGrupo.Location = new System.Drawing.Point(88, 410);
            this.bSalvaGrupo.Name = "bSalvaGrupo";
            this.bSalvaGrupo.Size = new System.Drawing.Size(72, 32);
            this.bSalvaGrupo.TabIndex = 49;
            this.bSalvaGrupo.Text = "Grabar";
            this.bSalvaGrupo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bSalvaGrupo.UseVisualStyleBackColor = false;
            this.bSalvaGrupo.Click += new System.EventHandler(this.BSalvaGrupoClick);
            // 
            // bLeeGrupo
            // 
            this.bLeeGrupo.BackColor = System.Drawing.Color.DarkSalmon;
            this.bLeeGrupo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bLeeGrupo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bLeeGrupo.Image = ((System.Drawing.Image)(resources.GetObject("bLeeGrupo.Image")));
            this.bLeeGrupo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bLeeGrupo.Location = new System.Drawing.Point(8, 410);
            this.bLeeGrupo.Name = "bLeeGrupo";
            this.bLeeGrupo.Size = new System.Drawing.Size(72, 32);
            this.bLeeGrupo.TabIndex = 48;
            this.bLeeGrupo.Text = "Leer";
            this.bLeeGrupo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bLeeGrupo.UseVisualStyleBackColor = false;
            this.bLeeGrupo.Click += new System.EventHandler(this.BLeeGrupoClick);
            // 
            // bPaste
            // 
            this.bPaste.BackColor = System.Drawing.Color.DarkSalmon;
            this.bPaste.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bPaste.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bPaste.Image = ((System.Drawing.Image)(resources.GetObject("bPaste.Image")));
            this.bPaste.Location = new System.Drawing.Point(48, 378);
            this.bPaste.Name = "bPaste";
            this.bPaste.Size = new System.Drawing.Size(24, 24);
            this.bPaste.TabIndex = 45;
            this.bPaste.UseVisualStyleBackColor = false;
            this.bPaste.Click += new System.EventHandler(this.BPasteClick);
            // 
            // bMenos
            // 
            this.bMenos.BackColor = System.Drawing.Color.DarkSalmon;
            this.bMenos.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bMenos.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bMenos.Location = new System.Drawing.Point(88, 466);
            this.bMenos.Name = "bMenos";
            this.bMenos.Size = new System.Drawing.Size(16, 16);
            this.bMenos.TabIndex = 89;
            this.bMenos.Text = "-";
            this.bMenos.UseVisualStyleBackColor = false;
            this.bMenos.Click += new System.EventHandler(this.BMenosClick);
            // 
            // lgrval
            // 
            this.lgrval.BackColor = System.Drawing.SystemColors.Info;
            this.lgrval.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lgrval.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lgrval.Location = new System.Drawing.Point(32, 450);
            this.lgrval.Name = "lgrval";
            this.lgrval.Size = new System.Drawing.Size(48, 32);
            this.lgrval.TabIndex = 90;
            this.lgrval.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bLeeVal
            // 
            this.bLeeVal.BackColor = System.Drawing.Color.DarkSalmon;
            this.bLeeVal.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bLeeVal.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bLeeVal.Image = ((System.Drawing.Image)(resources.GetObject("bLeeVal.Image")));
            this.bLeeVal.Location = new System.Drawing.Point(16, 378);
            this.bLeeVal.Name = "bLeeVal";
            this.bLeeVal.Size = new System.Drawing.Size(24, 24);
            this.bLeeVal.TabIndex = 43;
            this.bLeeVal.UseVisualStyleBackColor = false;
            this.bLeeVal.Click += new System.EventHandler(this.BLeeValClick);
            // 
            // dataGrid1
            // 
            this.dataGrid1.AllowSorting = false;
            this.dataGrid1.BackColor = System.Drawing.Color.Bisque;
            this.dataGrid1.BackgroundColor = System.Drawing.Color.Bisque;
            this.dataGrid1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGrid1.CaptionBackColor = System.Drawing.Color.SaddleBrown;
            this.dataGrid1.CaptionFont = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGrid1.CaptionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.dataGrid1.CaptionText = "Valoraciones";
            this.dataGrid1.DataMember = "";
            this.dataGrid1.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dataGrid1.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dataGrid1.Location = new System.Drawing.Point(8, 8);
            this.dataGrid1.Name = "dataGrid1";
            this.dataGrid1.PreferredColumnWidth = 40;
            this.dataGrid1.RowHeaderWidth = 30;
            this.dataGrid1.Size = new System.Drawing.Size(155, 335);
            this.dataGrid1.TabIndex = 0;
            this.dataGrid1.Paint += new System.Windows.Forms.PaintEventHandler(this.DataGrid1Paint);
            // 
            // bSalvaVal
            // 
            this.bSalvaVal.BackColor = System.Drawing.Color.DarkSalmon;
            this.bSalvaVal.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bSalvaVal.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bSalvaVal.Image = ((System.Drawing.Image)(resources.GetObject("bSalvaVal.Image")));
            this.bSalvaVal.Location = new System.Drawing.Point(96, 378);
            this.bSalvaVal.Name = "bSalvaVal";
            this.bSalvaVal.Size = new System.Drawing.Size(24, 24);
            this.bSalvaVal.TabIndex = 44;
            this.bSalvaVal.UseVisualStyleBackColor = false;
            this.bSalvaVal.Click += new System.EventHandler(this.BSalvaValClick);
            // 
            // valors
            // 
            this.BackColor = System.Drawing.Color.Bisque;
            this.Controls.Add(this.lgrval);
            this.Controls.Add(this.bMenos);
            this.Controls.Add(this.bMas);
            this.Controls.Add(this.bSalvaGrupo);
            this.Controls.Add(this.bLeeGrupo);
            this.Controls.Add(this.lugar);
            this.Controls.Add(this.bCut);
            this.Controls.Add(this.bPaste);
            this.Controls.Add(this.bSalvaVal);
            this.Controls.Add(this.bLeeVal);
            this.Controls.Add(this.dataGrid1);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "valors";
            this.Size = new System.Drawing.Size(168, 487);
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid1)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion
		
		void BLeeValClick(object sender, EventArgs e) { LeeVals(); }
		void DataGrid1Paint(object sender, PaintEventArgs e) {
			int row=0, pos=13;
		    SolidBrush Brocha;
			int yDelta = dataGrid1.GetCellBounds(row, 0).Height + 1;
			int y = dataGrid1.GetCellBounds(row, 0).Top + 2;
			CurrencyManager cm = (CurrencyManager) BindingContext[dataGrid1.DataSource, dataGrid1.DataMember];
			Font f = new Font(dataGrid1.Font.Name ,dataGrid1.Font.Size ,FontStyle.Bold );
			while(y < dataGrid1.Height - yDelta && row < cm.Count) {
				string text = string.Format("{0}", row+1);
				if(row>8) pos=9;
				double nv = (double)dataGrid1[row,0]+(double)dataGrid1[row,1]+(double)dataGrid1[row,2];
				Brocha = new SolidBrush((nv<99 || nv>101)?Color.Red:Color.Black);
				e.Graphics.DrawString(text, f , Brocha, pos, y);
				y += yDelta;
				row++;
			}
			Brocha = new SolidBrush(Color.FromArgb (80,255, 255, 128));
			int lg = dataGrid1.GetCellBounds(4, 2).Right;
			y = dataGrid1.GetCellBounds(4, 0).Top;
			int h = dataGrid1.GetCellBounds(7, 0).Bottom-y;
			e.Graphics.FillRectangle(Brocha, new Rectangle(1,y,lg,h));
			y = dataGrid1.GetCellBounds(11, 0).Top;
			h = dataGrid1.GetCellBounds(13, 0).Bottom-y;
			e.Graphics.FillRectangle(Brocha, new Rectangle(1,y,lg,h));
			Brocha.Dispose();
		}
		void BSalvaValClick(object sender, EventArgs e) { SalvaVals(); }
		void BPasteClick(object sender, EventArgs e) { DesdePortapapeles(); }
		void BCutClick(object sender, EventArgs e) { HaciaPortapapeles(); }
		void OnCrearCPs(EventArgs e) { if (CrearCPs != null) CrearCPs(this, e); }
		void BLeeGrupoClick(object sender, EventArgs e) { LeeGrupo(); }
		void BSalvaGrupoClick(object sender, EventArgs e) { SalvaGrupo(); }
		void BMasClick(object sender, EventArgs e) { GRMas(); }
		void BMenosClick(object sender, EventArgs e) { GRMenos(); }
		
	}
}
