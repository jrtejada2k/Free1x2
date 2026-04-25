// Free1X2 : Programa de quinielas "libre"
// Copyright (C) xfsf
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
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI {
	public class AnaCombi : Form {
		private Button bGrabar;
		private Button bCalcular;
		private Label label3;
		private Label label2;
		private Label label1;
		private Label label7;
		private Label label6;
		private Label label5;
		private Label label4;
		private DataGrid dataGrid1;
		private Label label8;
		private Label lproc;
		private TextBox tb03;
		private TextBox tb02;
		private TextBox tb01;
		private Label label12;
		private TextBox tb07;
		private TextBox tb06;
		private TextBox tb05;
		private TextBox tb04;
		private Label label14;
		private TextBox tb09;
		private TextBox tb08;
		private Label label10;
		private Label label11;
		private TextBox tbfal;
		private Label label15;
		private Label label13;
		private Button bCancelar;
		private Button bLimp;
		private Label lFileIn;
		private Label label9;
		private Label ltime;
		private TextBox tb12;
		private TextBox tb13;
		private TextBox tb10;
		private TextBox tb11;
		private TextBox tb14;
		private Button bFileIn;
		public AnaCombi() {
			InitializeComponent();
			elmeu = new Timer();
			elmeu.Interval = 3000;
			elmeu.Tick += elmeuTimer;
			InitGrid();
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
        }

		
		private int ctproc, ctgrup, ctfal, admfal;
		private int[] grup = new int[14];
		private int[] comptes = new int[4782969];
		private int[] valides = new int[4782969];
		private DateTime dt0, dt9;
		private Timer elmeu;
		private bool salida;
		private string filein="", patron="";
		private DataSet dsDatos;
		private DataGridTableStyle tabla = new DataGridTableStyle();
		
		private void Grabar() {
			int idxg; string xcol, tmp;
			int nl = dsDatos.Tables["Resultados"].Rows.Count;
			bCalcular.Enabled = false;
			bGrabar.Visible = false;
			SaveFileDialog resul = new SaveFileDialog();
            resul.InitialDirectory = Application.StartupPath + "/" ;
			resul.Filter = "Resultados(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
			if(resul.ShowDialog() == DialogResult.OK) {
				Application.DoEvents();
				string fileout = Path.GetFileName(resul.FileName);
				StreamWriter sw = new StreamWriter(fileout);
				for (int idxc=0; idxc<4782969; idxc++) {
					idxg=valides[idxc];
					if (idxg<0) continue;
					xcol = n14s(idxg);
					xcol = xcol.Substring(0,ctgrup);
					for (int nr=0; nr<nl; nr++) {
						tmp = (string)dataGrid1[nr,0];
						if (xcol==tmp) {
							if (dataGrid1.IsSelected(nr)) {
								sw.WriteLine(n14s(idxc));
								break;
							}
						}
					}
				}
				sw.Close();
			}
			bGrabar.Visible = true;
			bCalcular.Enabled = true;
		}
		private void EntradaFichero() {
			OpenFileDialog abreFileIn = new OpenFileDialog();
            abreFileIn.InitialDirectory = Application.StartupPath + "/";
			abreFileIn.Filter = "F.Entrada(*.txt)|*.txt|Todos los archivos(*.*)|*.*" ;
			if(abreFileIn.ShowDialog() == DialogResult.OK) {
				filein = Path.GetFileName(abreFileIn.FileName);
				lFileIn.Text = filein;
			}
		}
		private void Calcular() {
			string columna; int idxn;
			StreamReader sr;
			bCalcular.Enabled = false;
			elmeu.Start(); dt0 = DateTime.Now;
			ctproc=0; salida=false;
			RecuperaGrupo();
			if (ctgrup==0) { MessageBox.Show("falta grupo"); goto fora; }
			for (int nr=0; nr<4782969; nr++) {
				comptes[nr]=(-1); valides[nr]=(-1);
			}
			string tmp = n14s(0); tmp = tmp.Substring(14-ctgrup, ctgrup)+"11111111111111";
			tmp = tmp.Substring(0,14); int idx0 = s14n(tmp);
			comptes[idx0]=0;
			for (int nr=1; nr<4782969; nr++) {
				tmp = n14s(nr); tmp = tmp.Substring(14-ctgrup, ctgrup)+"11111111111111";
				tmp = tmp.Substring(0,14); idxn = s14n(tmp);
				if (idx0==idxn) break;
				comptes[idxn]=0;
			}
			ltime.Text=lproc.Text = " ";
			try {
				sr = new StreamReader(filein);
			} catch { MessageBox.Show("falta fichero"); goto fora; }
			while (sr.Peek()>0)  {
				Application.DoEvents();
				if (salida) break;
				columna = Normaliza(sr.ReadLine()); ctproc++;
				if (columna.Length < 14) { MessageBox.Show("columna errónea = "+ctproc); break; }
				ctfal=0; 
				for (int nr=0; nr<14; nr++) {
					if (patron[nr]=='-') continue;
					if (columna[nr]!=patron[nr]) ctfal++;
				}
				if (ctfal<=admfal) Contabiliza(columna);
			}
			sr.Close();
			Mostraresuls();
		fora:
			elmeu.Stop(); veureelmeu();
			bCalcular.Enabled = true;
		}
		private void Mostraresuls() {
			int ncols; string tmp;
			DataRow row;
			dsDatos = new DataSet();
            DataTable newTable = new DataTable("Resultados");
			newTable.Columns.Add("G", typeof(string));
			newTable.Columns.Add("C", typeof(int));
			dsDatos.Tables.Add(newTable);
			for (int nr=0; nr<4782969; nr++) {
				ncols=comptes[nr];
				if (ncols>=0) {
					tmp = n14s(nr);
					tmp=tmp.Substring(0,ctgrup);
					row = dsDatos.Tables["Resultados"].NewRow();
					row["G"] = tmp;
					row["C"] = ncols;
                    dsDatos.Tables["Resultados"].Rows.Add(row);
				}
			}
            dataGrid1.SetDataBinding(dsDatos, "Resultados");
		}
		private void Contabiliza(string columna) {
			string xcol=""; int idxg, idxc;
			for (int nr=0; nr<ctgrup; nr++) {
				idxg=grup[nr];
				xcol+=columna[idxg];
			}
			for (int nr=ctgrup; nr<14; nr++) xcol+='1';
			idxg=s14n(xcol); comptes[idxg]++;
			idxc=s14n(columna); valides[idxc]=idxg;
		}
		private string Normaliza(string columna) {
			string chval = "12X", xcol; char ch;
			columna=columna.ToUpper();
			xcol = "";
			for (int nr=0; nr<columna.Length; nr++) {
				ch = columna[nr];
				if (chval.IndexOf(ch)>=0) xcol+=ch;
			}
			return xcol;
		}
		private int s14n(string ax) {
			int nx; string ch;
			nx=0;
			for (int nr=0; nr<14; nr++) {
				nx *= 3;
				ch = ax.Substring(nr,1);
				if (ch=="1") nx+=1;
				else if (ch=="2") nx+=2;
			}
			return nx;
		}
		private string n14s(int nx) {
			string ax = ""; int nx2;
			for (int nr=0; nr<14; nr++) {
				nx2 = nx%3; nx /= 3;
				if (nx2==1) ax = "1"+ax;
				else if (nx2==2) ax = "2"+ax;
				else ax = "X"+ax;
			}
			return ax;
		}
		private void veureelmeu() {
			dt9 = DateTime.Now;
			string temp = (dt9-dt0)+"0000000000";
			ltime.Text = temp.Substring(0,10);
			lproc.Text = ""+ctproc;
		}
		private void RecuperaGrupo() {
			string ch;
			ctgrup=0; patron="";
			ch=tb01.Text;
			if (ch=="1" || ch=="X" || ch=="2") patron+=ch; else patron+='-';
			if (ch=="G") { grup[ctgrup]=0; ctgrup++; }
			ch=tb02.Text;
			if (ch=="1" || ch=="X" || ch=="2") patron+=ch; else patron+='-';
			if (ch=="G") { grup[ctgrup]=1; ctgrup++; }
			ch=tb03.Text;
			if (ch=="1" || ch=="X" || ch=="2") patron+=ch; else patron+='-';
			if (ch=="G") { grup[ctgrup]=2; ctgrup++; }
			ch=tb04.Text;
			if (ch=="1" || ch=="X" || ch=="2") patron+=ch; else patron+='-';
			if (ch=="G") { grup[ctgrup]=3; ctgrup++; }
			ch=tb05.Text;
			if (ch=="1" || ch=="X" || ch=="2") patron+=ch; else patron+='-';
			if (ch=="G") { grup[ctgrup]=4; ctgrup++; }
			ch=tb06.Text;
			if (ch=="1" || ch=="X" || ch=="2") patron+=ch; else patron+='-';
			if (ch=="G") { grup[ctgrup]=5; ctgrup++; }
			ch=tb07.Text;
			if (ch=="1" || ch=="X" || ch=="2") patron+=ch; else patron+='-';
			if (ch=="G") { grup[ctgrup]=6; ctgrup++; }
			ch=tb08.Text;
			if (ch=="1" || ch=="X" || ch=="2") patron+=ch; else patron+='-';
			if (ch=="G") { grup[ctgrup]=7; ctgrup++; }
			ch=tb09.Text;
			if (ch=="1" || ch=="X" || ch=="2") patron+=ch; else patron+='-';
			if (ch=="G") { grup[ctgrup]=8; ctgrup++; }
			ch=tb10.Text;
			if (ch=="1" || ch=="X" || ch=="2") patron+=ch; else patron+='-';
			if (ch=="G") { grup[ctgrup]=9; ctgrup++; }
			ch=tb11.Text;
			if (ch=="1" || ch=="X" || ch=="2") patron+=ch; else patron+='-';
			if (ch=="G") { grup[ctgrup]=10; ctgrup++; }
			ch=tb12.Text;
			if (ch=="1" || ch=="X" || ch=="2") patron+=ch; else patron+='-';
			if (ch=="G") { grup[ctgrup]=11; ctgrup++; }
			ch=tb13.Text;
			if (ch=="1" || ch=="X" || ch=="2") patron+=ch; else patron+='-';
			if (ch=="G") { grup[ctgrup]=12; ctgrup++; }
			ch=tb14.Text;
			if (ch=="1" || ch=="X" || ch=="2") patron+=ch; else patron+='-';
			if (ch=="G") { grup[ctgrup]=13; ctgrup++; }
			try { admfal=Convert.ToInt32(tbfal.Text); }
			catch { admfal=0; }
		}
		private void InitGrid() {
            tabla.MappingName = "Resultados";

		    DataGridTextBoxColumn cs = new DataGridTextBoxColumn();
			cs.MappingName = "G";
			cs.HeaderText = "grupo";
			cs.Width = 80;
			cs.ReadOnly = true;
			cs.Alignment = HorizontalAlignment.Center;
			tabla.GridColumnStyles.Add(cs);
			
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "C";
			cs.HeaderText = "cols...";
			cs.Width = 60;
			cs.ReadOnly = true;
			cs.Alignment = HorizontalAlignment.Right;
			tabla.GridColumnStyles.Add(cs);
			
			dataGrid1.TableStyles.Clear();
			dataGrid1.TableStyles.Add(tabla);
		}
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnaCombi));
            bFileIn = new Button();
            tb14 = new TextBox();
            tb11 = new TextBox();
            tb10 = new TextBox();
            tb13 = new TextBox();
            tb12 = new TextBox();
            ltime = new Label();
            label9 = new Label();
            lFileIn = new Label();
            bLimp = new Button();
            bCancelar = new Button();
            label13 = new Label();
            label15 = new Label();
            tbfal = new TextBox();
            label11 = new Label();
            label10 = new Label();
            tb08 = new TextBox();
            tb09 = new TextBox();
            label14 = new Label();
            tb04 = new TextBox();
            tb05 = new TextBox();
            tb06 = new TextBox();
            tb07 = new TextBox();
            label12 = new Label();
            tb01 = new TextBox();
            tb02 = new TextBox();
            tb03 = new TextBox();
            lproc = new Label();
            label8 = new Label();
            dataGrid1 = new DataGrid();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            label7 = new Label();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            bCalcular = new Button();
            bGrabar = new Button();
            ((System.ComponentModel.ISupportInitialize)(dataGrid1)).BeginInit();
            SuspendLayout();
            // 
            // bFileIn
            // 
            bFileIn.BackColor = Color.DarkSalmon;
            bFileIn.FlatStyle = FlatStyle.Popup;
            bFileIn.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            bFileIn.Image = ((Image)(resources.GetObject("bFileIn.Image")));
            bFileIn.Location = new Point(73, 16);
            bFileIn.Name = "bFileIn";
            bFileIn.Size = new Size(32, 20);
            bFileIn.TabIndex = 80;
            bFileIn.UseVisualStyleBackColor = false;
            bFileIn.Click += BFileInClick;
            // 
            // tb14
            // 
            tb14.BackColor = SystemColors.Info;
            tb14.BorderStyle = BorderStyle.FixedSingle;
            tb14.CharacterCasing = CharacterCasing.Upper;
            tb14.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            tb14.Location = new Point(40, 326);
            tb14.MaxLength = 1;
            tb14.Name = "tb14";
            tb14.Size = new Size(32, 21);
            tb14.TabIndex = 58;
            tb14.Text = "-";
            tb14.TextAlign = HorizontalAlignment.Center;
            // 
            // tb11
            // 
            tb11.BorderStyle = BorderStyle.FixedSingle;
            tb11.CharacterCasing = CharacterCasing.Upper;
            tb11.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            tb11.Location = new Point(40, 260);
            tb11.MaxLength = 1;
            tb11.Name = "tb11";
            tb11.Size = new Size(32, 21);
            tb11.TabIndex = 55;
            tb11.Text = "-";
            tb11.TextAlign = HorizontalAlignment.Center;
            // 
            // tb10
            // 
            tb10.BorderStyle = BorderStyle.FixedSingle;
            tb10.CharacterCasing = CharacterCasing.Upper;
            tb10.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            tb10.Location = new Point(40, 238);
            tb10.MaxLength = 1;
            tb10.Name = "tb10";
            tb10.Size = new Size(32, 21);
            tb10.TabIndex = 54;
            tb10.Text = "-";
            tb10.TextAlign = HorizontalAlignment.Center;
            // 
            // tb13
            // 
            tb13.BackColor = SystemColors.Info;
            tb13.BorderStyle = BorderStyle.FixedSingle;
            tb13.CharacterCasing = CharacterCasing.Upper;
            tb13.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            tb13.Location = new Point(40, 304);
            tb13.MaxLength = 1;
            tb13.Name = "tb13";
            tb13.Size = new Size(32, 21);
            tb13.TabIndex = 57;
            tb13.Text = "-";
            tb13.TextAlign = HorizontalAlignment.Center;
            // 
            // tb12
            // 
            tb12.BackColor = SystemColors.Info;
            tb12.BorderStyle = BorderStyle.FixedSingle;
            tb12.CharacterCasing = CharacterCasing.Upper;
            tb12.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            tb12.Location = new Point(40, 282);
            tb12.MaxLength = 1;
            tb12.Name = "tb12";
            tb12.Size = new Size(32, 21);
            tb12.TabIndex = 56;
            tb12.Text = "-";
            tb12.TextAlign = HorizontalAlignment.Center;
            // 
            // ltime
            // 
            ltime.BackColor = SystemColors.Info;
            ltime.BorderStyle = BorderStyle.FixedSingle;
            ltime.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            ltime.Location = new Point(88, 105);
            ltime.Name = "ltime";
            ltime.Size = new Size(96, 21);
            ltime.TabIndex = 43;
            ltime.Text = "Tiempo";
            ltime.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            label9.BackColor = Color.Bisque;
            label9.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            label9.Location = new Point(8, 282);
            label9.Name = "label9";
            label9.Size = new Size(32, 21);
            label9.TabIndex = 115;
            label9.Text = "12";
            label9.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lFileIn
            // 
            lFileIn.BackColor = SystemColors.Info;
            lFileIn.BorderStyle = BorderStyle.FixedSingle;
            lFileIn.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            lFileIn.Location = new Point(106, 16);
            lFileIn.Name = "lFileIn";
            lFileIn.Size = new Size(272, 20);
            lFileIn.TabIndex = 79;
            lFileIn.Text = "Fichero a procesar";
            lFileIn.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // bLimp
            // 
            bLimp.BackColor = Color.DarkSalmon;
            bLimp.FlatStyle = FlatStyle.Popup;
            bLimp.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            bLimp.Image = ((Image)(resources.GetObject("bLimp.Image")));
            bLimp.Location = new Point(40, 16);
            bLimp.Name = "bLimp";
            bLimp.Size = new Size(32, 20);
            bLimp.TabIndex = 99;
            bLimp.UseVisualStyleBackColor = false;
            bLimp.Click += BLimpClick;
            // 
            // bCancelar
            // 
            bCancelar.BackColor = Color.DarkSalmon;
            bCancelar.FlatStyle = FlatStyle.Popup;
            bCancelar.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            bCancelar.Image = ((Image)(resources.GetObject("bCancelar.Image")));
            bCancelar.ImageAlign = ContentAlignment.MiddleLeft;
            bCancelar.Location = new Point(88, 128);
            bCancelar.Name = "bCancelar";
            bCancelar.Size = new Size(96, 32);
            bCancelar.TabIndex = 102;
            bCancelar.Text = "Cancelar";
            bCancelar.TextAlign = ContentAlignment.MiddleRight;
            bCancelar.UseVisualStyleBackColor = false;
            bCancelar.Click += BCancelarClick;
            // 
            // label13
            // 
            label13.BackColor = Color.Bisque;
            label13.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            label13.Location = new Point(8, 326);
            label13.Name = "label13";
            label13.Size = new Size(32, 21);
            label13.TabIndex = 117;
            label13.Text = "14";
            label13.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label15
            // 
            label15.BackColor = Color.Bisque;
            label15.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            label15.Location = new Point(96, 192);
            label15.Name = "label15";
            label15.Size = new Size(80, 40);
            label15.TabIndex = 118;
            label15.Text = "Fallos Admitidos";
            label15.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tbfal
            // 
            tbfal.BorderStyle = BorderStyle.FixedSingle;
            tbfal.CharacterCasing = CharacterCasing.Upper;
            tbfal.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            tbfal.Location = new Point(120, 232);
            tbfal.MaxLength = 1;
            tbfal.Name = "tbfal";
            tbfal.Size = new Size(32, 21);
            tbfal.TabIndex = 119;
            tbfal.Text = "0";
            tbfal.TextAlign = HorizontalAlignment.Center;
            // 
            // label11
            // 
            label11.BackColor = Color.Bisque;
            label11.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            label11.Location = new Point(8, 238);
            label11.Name = "label11";
            label11.Size = new Size(32, 21);
            label11.TabIndex = 113;
            label11.Text = "10";
            label11.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            label10.BackColor = Color.Bisque;
            label10.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            label10.Location = new Point(8, 260);
            label10.Name = "label10";
            label10.Size = new Size(32, 21);
            label10.TabIndex = 114;
            label10.Text = "11";
            label10.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tb08
            // 
            tb08.BackColor = SystemColors.Info;
            tb08.BorderStyle = BorderStyle.FixedSingle;
            tb08.CharacterCasing = CharacterCasing.Upper;
            tb08.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            tb08.Location = new Point(40, 194);
            tb08.MaxLength = 1;
            tb08.Name = "tb08";
            tb08.Size = new Size(32, 21);
            tb08.TabIndex = 52;
            tb08.Text = "-";
            tb08.TextAlign = HorizontalAlignment.Center;
            // 
            // tb09
            // 
            tb09.BorderStyle = BorderStyle.FixedSingle;
            tb09.CharacterCasing = CharacterCasing.Upper;
            tb09.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            tb09.Location = new Point(40, 216);
            tb09.MaxLength = 1;
            tb09.Name = "tb09";
            tb09.Size = new Size(32, 21);
            tb09.TabIndex = 53;
            tb09.Text = "-";
            tb09.TextAlign = HorizontalAlignment.Center;
            // 
            // label14
            // 
            label14.BackColor = Color.Bisque;
            label14.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            label14.Location = new Point(8, 304);
            label14.Name = "label14";
            label14.Size = new Size(32, 21);
            label14.TabIndex = 116;
            label14.Text = "13";
            label14.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tb04
            // 
            tb04.BorderStyle = BorderStyle.FixedSingle;
            tb04.CharacterCasing = CharacterCasing.Upper;
            tb04.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            tb04.Location = new Point(40, 106);
            tb04.MaxLength = 1;
            tb04.Name = "tb04";
            tb04.Size = new Size(32, 21);
            tb04.TabIndex = 48;
            tb04.Text = "-";
            tb04.TextAlign = HorizontalAlignment.Center;
            // 
            // tb05
            // 
            tb05.BackColor = SystemColors.Info;
            tb05.BorderStyle = BorderStyle.FixedSingle;
            tb05.CharacterCasing = CharacterCasing.Upper;
            tb05.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            tb05.Location = new Point(40, 128);
            tb05.MaxLength = 1;
            tb05.Name = "tb05";
            tb05.Size = new Size(32, 21);
            tb05.TabIndex = 49;
            tb05.Text = "-";
            tb05.TextAlign = HorizontalAlignment.Center;
            // 
            // tb06
            // 
            tb06.BackColor = SystemColors.Info;
            tb06.BorderStyle = BorderStyle.FixedSingle;
            tb06.CharacterCasing = CharacterCasing.Upper;
            tb06.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            tb06.Location = new Point(40, 150);
            tb06.MaxLength = 1;
            tb06.Name = "tb06";
            tb06.Size = new Size(32, 21);
            tb06.TabIndex = 50;
            tb06.Text = "-";
            tb06.TextAlign = HorizontalAlignment.Center;
            // 
            // tb07
            // 
            tb07.BackColor = SystemColors.Info;
            tb07.BorderStyle = BorderStyle.FixedSingle;
            tb07.CharacterCasing = CharacterCasing.Upper;
            tb07.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            tb07.Location = new Point(40, 172);
            tb07.MaxLength = 1;
            tb07.Name = "tb07";
            tb07.Size = new Size(32, 21);
            tb07.TabIndex = 51;
            tb07.Text = "-";
            tb07.TextAlign = HorizontalAlignment.Center;
            // 
            // label12
            // 
            label12.BackColor = Color.Bisque;
            label12.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            label12.Location = new Point(8, 216);
            label12.Name = "label12";
            label12.Size = new Size(32, 21);
            label12.TabIndex = 112;
            label12.Text = "9";
            label12.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // tb01
            // 
            tb01.BorderStyle = BorderStyle.FixedSingle;
            tb01.CharacterCasing = CharacterCasing.Upper;
            tb01.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            tb01.Location = new Point(40, 40);
            tb01.MaxLength = 1;
            tb01.Name = "tb01";
            tb01.Size = new Size(32, 21);
            tb01.TabIndex = 45;
            tb01.Text = "-";
            tb01.TextAlign = HorizontalAlignment.Center;
            // 
            // tb02
            // 
            tb02.BorderStyle = BorderStyle.FixedSingle;
            tb02.CharacterCasing = CharacterCasing.Upper;
            tb02.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            tb02.Location = new Point(40, 62);
            tb02.MaxLength = 1;
            tb02.Name = "tb02";
            tb02.Size = new Size(32, 21);
            tb02.TabIndex = 46;
            tb02.Text = "-";
            tb02.TextAlign = HorizontalAlignment.Center;
            // 
            // tb03
            // 
            tb03.BorderStyle = BorderStyle.FixedSingle;
            tb03.CharacterCasing = CharacterCasing.Upper;
            tb03.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            tb03.Location = new Point(40, 84);
            tb03.MaxLength = 1;
            tb03.Name = "tb03";
            tb03.Size = new Size(32, 21);
            tb03.TabIndex = 47;
            tb03.Text = "-";
            tb03.TextAlign = HorizontalAlignment.Center;
            // 
            // lproc
            // 
            lproc.BackColor = SystemColors.Info;
            lproc.BorderStyle = BorderStyle.FixedSingle;
            lproc.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            lproc.Location = new Point(88, 81);
            lproc.Name = "lproc";
            lproc.Size = new Size(96, 21);
            lproc.TabIndex = 42;
            lproc.Text = "Procesadas";
            lproc.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            label8.BackColor = Color.Bisque;
            label8.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            label8.Location = new Point(8, 128);
            label8.Name = "label8";
            label8.Size = new Size(32, 21);
            label8.TabIndex = 108;
            label8.Text = "5";
            label8.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // dataGrid1
            // 
            dataGrid1.CaptionText = "Resultados";
            dataGrid1.DataMember = "";
            dataGrid1.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            dataGrid1.HeaderForeColor = SystemColors.ControlText;
            dataGrid1.Location = new Point(200, 48);
            dataGrid1.Name = "dataGrid1";
            dataGrid1.RowHeaderWidth = 20;
            dataGrid1.Size = new Size(184, 288);
            dataGrid1.TabIndex = 103;
            dataGrid1.MouseUp += dataGrid1_MouseUp;
            // 
            // label4
            // 
            label4.BackColor = Color.Bisque;
            label4.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            label4.Location = new Point(8, 84);
            label4.Name = "label4";
            label4.Size = new Size(32, 21);
            label4.TabIndex = 106;
            label4.Text = "3";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            label5.BackColor = Color.Bisque;
            label5.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            label5.Location = new Point(8, 194);
            label5.Name = "label5";
            label5.Size = new Size(32, 21);
            label5.TabIndex = 111;
            label5.Text = "8";
            label5.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            label6.BackColor = Color.Bisque;
            label6.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            label6.Location = new Point(8, 172);
            label6.Name = "label6";
            label6.Size = new Size(32, 21);
            label6.TabIndex = 110;
            label6.Text = "7";
            label6.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            label7.BackColor = Color.Bisque;
            label7.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            label7.Location = new Point(8, 150);
            label7.Name = "label7";
            label7.Size = new Size(32, 21);
            label7.TabIndex = 109;
            label7.Text = "6";
            label7.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            label1.BackColor = Color.Bisque;
            label1.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            label1.Location = new Point(8, 40);
            label1.Name = "label1";
            label1.Size = new Size(32, 21);
            label1.TabIndex = 104;
            label1.Text = "1";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.BackColor = Color.Bisque;
            label2.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            label2.Location = new Point(8, 62);
            label2.Name = "label2";
            label2.Size = new Size(32, 21);
            label2.TabIndex = 105;
            label2.Text = "2";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            label3.BackColor = Color.Bisque;
            label3.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            label3.Location = new Point(8, 106);
            label3.Name = "label3";
            label3.Size = new Size(32, 21);
            label3.TabIndex = 107;
            label3.Text = "4";
            label3.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // bCalcular
            // 
            bCalcular.BackColor = Color.DarkSalmon;
            bCalcular.FlatStyle = FlatStyle.Popup;
            bCalcular.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            bCalcular.Image = ((Image)(resources.GetObject("bCalcular.Image")));
            bCalcular.ImageAlign = ContentAlignment.MiddleLeft;
            bCalcular.Location = new Point(88, 48);
            bCalcular.Name = "bCalcular";
            bCalcular.Size = new Size(96, 32);
            bCalcular.TabIndex = 44;
            bCalcular.Text = "Calcular";
            bCalcular.TextAlign = ContentAlignment.MiddleRight;
            bCalcular.UseVisualStyleBackColor = false;
            bCalcular.Click += BCalcularClick;
            // 
            // bGrabar
            // 
            bGrabar.BackColor = Color.DarkSalmon;
            bGrabar.FlatStyle = FlatStyle.Popup;
            bGrabar.Font = new Font("Verdana", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
            bGrabar.Image = ((Image)(resources.GetObject("bGrabar.Image")));
            bGrabar.ImageAlign = ContentAlignment.MiddleLeft;
            bGrabar.Location = new Point(88, 291);
            bGrabar.Name = "bGrabar";
            bGrabar.Size = new Size(96, 43);
            bGrabar.TabIndex = 82;
            bGrabar.Text = "Grabar    Resultados";
            bGrabar.TextAlign = ContentAlignment.MiddleRight;
            bGrabar.UseVisualStyleBackColor = false;
            bGrabar.Click += BGrabarClick;
            // 
            // AnaCombi
            // 
            AutoScaleBaseSize = new Size(5, 13);
            BackColor = Color.Bisque;
            ClientSize = new Size(400, 387);
            Controls.Add(tbfal);
            Controls.Add(label15);
            Controls.Add(label13);
            Controls.Add(label14);
            Controls.Add(label9);
            Controls.Add(label10);
            Controls.Add(label11);
            Controls.Add(label12);
            Controls.Add(label5);
            Controls.Add(label6);
            Controls.Add(label7);
            Controls.Add(label8);
            Controls.Add(label3);
            Controls.Add(label4);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(dataGrid1);
            Controls.Add(bCancelar);
            Controls.Add(bLimp);
            Controls.Add(bGrabar);
            Controls.Add(bFileIn);
            Controls.Add(lFileIn);
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
            Controls.Add(bCalcular);
            Controls.Add(ltime);
            Controls.Add(lproc);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AnaCombi";
            Text = "Análisis de grupos en combinación";
            ((System.ComponentModel.ISupportInitialize)(dataGrid1)).EndInit();
            ResumeLayout(false);
            PerformLayout();

		}
		#endregion
		
		void BCalcularClick(object sender, EventArgs e) { Calcular(); }
		void elmeuTimer(object sender, EventArgs e) { veureelmeu(); }
		void BFileInClick(object sender, EventArgs e) { EntradaFichero(); }
		void BGrabarClick(object sender, EventArgs e) { Grabar(); }
		void BLimpClick(object sender, EventArgs e) {
			tb01.Text=tb02.Text=tb03.Text=tb04.Text=tb05.Text="-";
			tb06.Text=tb07.Text=tb08.Text=tb09.Text=tb10.Text="-";
			tb11.Text=tb12.Text=tb13.Text=tb14.Text="-";
		}
		void BCancelarClick(object sender, EventArgs e) { salida=true; }
		void dataGrid1_MouseUp(object sender, MouseEventArgs e) {
		    Point pt = new Point(e.X, e.Y);
			DataGrid.HitTestInfo hti = dataGrid1.HitTest(pt);
			if(hti.Type == DataGrid.HitTestType.Cell) {
				dataGrid1.CurrentCell = new DataGridCell(hti.Row, hti.Column);
				bool marca = (bool)dataGrid1[hti.Row,1];
				if (marca) dataGrid1[hti.Row,1] = false;
				else dataGrid1[hti.Row,1] = true;
			}
		}
		
	}
}

