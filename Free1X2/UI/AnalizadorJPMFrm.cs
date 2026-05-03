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
using System.Collections;
using Free1X2;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI {
	public class AnalizadorJPM : System.Windows.Forms.Form {
		private System.Windows.Forms.CheckBox ck01;
		private System.Windows.Forms.CheckBox ck00;
		private System.Windows.Forms.Label lCol;
		private System.Windows.Forms.CheckBox ck02;
		private System.Windows.Forms.Label lb09;
		private System.Windows.Forms.Label label30;
		private System.Windows.Forms.Label label31;
		private System.Windows.Forms.Label label33;
		private System.Windows.Forms.Button bMenosR;
		private System.Windows.Forms.Label label35;
		private System.Windows.Forms.Label label36;
		private System.Windows.Forms.Label label37;
		private System.Windows.Forms.Label label38;
		private System.Windows.Forms.Label label39;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label lb05;
		private System.Windows.Forms.CheckBox ck05;
		private System.Windows.Forms.CheckBox ck04;
		private System.Windows.Forms.CheckBox ck07;
		private System.Windows.Forms.CheckBox ck06;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.CheckBox ck03;
		private System.Windows.Forms.Label lb30;
		private System.Windows.Forms.Label lb31;
		private System.Windows.Forms.Label lb32;
		private System.Windows.Forms.Label lb35;
		private System.Windows.Forms.Label lb34;
		private System.Windows.Forms.Button bAnalizar;
		private System.Windows.Forms.Label lFileOut;
		private System.Windows.Forms.Label lb29;
		private System.Windows.Forms.Label lb28;
		private System.Windows.Forms.Label lb19;
		private System.Windows.Forms.Label lSumSel;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Label lb21;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label lb23;
		private System.Windows.Forms.Label lb13;
		private System.Windows.Forms.Label lb25;
		private System.Windows.Forms.Label lb24;
		private System.Windows.Forms.Label lb27;
		private System.Windows.Forms.Label lb26;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button bCancelar;
		private System.Windows.Forms.Label lpr12;
		private System.Windows.Forms.Label label46;
		private System.Windows.Forms.Label lpr10;
		private System.Windows.Forms.CheckBox ck09;
		private System.Windows.Forms.Label lb01;
		private System.Windows.Forms.Label label40;
		private System.Windows.Forms.Label lb02;
		private System.Windows.Forms.Label lb04;
		private System.Windows.Forms.Label lb33;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.CheckBox ck29;
		private System.Windows.Forms.CheckBox ck28;
		private System.Windows.Forms.CheckBox ck27;
		private System.Windows.Forms.CheckBox ck26;
		private System.Windows.Forms.CheckBox ck25;
		private System.Windows.Forms.CheckBox ck24;
		private System.Windows.Forms.CheckBox ck23;
		private System.Windows.Forms.CheckBox ck22;
		private System.Windows.Forms.CheckBox ck21;
		private System.Windows.Forms.CheckBox ck20;
		private System.Windows.Forms.Label label24;
		private System.Windows.Forms.TextBox tbv6;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label28;
		private System.Windows.Forms.Button bSumar;
		private System.Windows.Forms.Label lb03;
		private System.Windows.Forms.Label lpr13;
		private System.Windows.Forms.Button bMasR;
		private System.Windows.Forms.Label lpr11;
		private System.Windows.Forms.Label lb07;
		private System.Windows.Forms.Label lb06;
		private System.Windows.Forms.Label lpr14;
		private System.Windows.Forms.Label label45;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.TextBox tbv5;
		private System.Windows.Forms.TextBox tbv4;
		private System.Windows.Forms.TextBox tbv7;
		private System.Windows.Forms.Button bGrabarCols;
		private System.Windows.Forms.TextBox tbv1;
		private System.Windows.Forms.TextBox tbv0;
		private System.Windows.Forms.TextBox tbv3;
		private System.Windows.Forms.TextBox tbv2;
		private System.Windows.Forms.Label lb18;
		private System.Windows.Forms.TextBox tbv8;
		private System.Windows.Forms.Label lFGR;
		private System.Windows.Forms.CheckBox ck11;
		private System.Windows.Forms.Label lb10;
		private System.Windows.Forms.Label lb11;
		private System.Windows.Forms.Label lb16;
		private System.Windows.Forms.Label lb17;
		private System.Windows.Forms.Label lb14;
		private System.Windows.Forms.Label lb15;
		private System.Windows.Forms.Label lb08;
		private System.Windows.Forms.Button bIniciar;
		private System.Windows.Forms.Label label25;
		private System.Windows.Forms.CheckBox ck18;
		private System.Windows.Forms.CheckBox ck19;
		private System.Windows.Forms.Label label59;
		private System.Windows.Forms.CheckBox ck14;
		private System.Windows.Forms.CheckBox ck15;
		private System.Windows.Forms.CheckBox ck16;
		private System.Windows.Forms.CheckBox ck17;
		private System.Windows.Forms.CheckBox ck10;
		private System.Windows.Forms.Label label47;
		private System.Windows.Forms.CheckBox ck12;
		private System.Windows.Forms.CheckBox ck13;
		private System.Windows.Forms.Label label44;
		private System.Windows.Forms.Label label43;
		private System.Windows.Forms.Label label42;
		private System.Windows.Forms.Label label41;
		private System.Windows.Forms.Label lTime;
		private System.Windows.Forms.Label label61;
		private System.Windows.Forms.Label label49;
		private System.Windows.Forms.Label label48;
		private System.Windows.Forms.Label lb20;
		private System.Windows.Forms.Label lb22;
		private System.Windows.Forms.Label lb12;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.TextBox tbCG;
		private System.Windows.Forms.Label label57;
		private System.Windows.Forms.Label label55;
		private System.Windows.Forms.Label label53;
		private System.Windows.Forms.Label label50;
		private System.Windows.Forms.Label label51;
		private System.Windows.Forms.CheckBox ck08;
		private System.Windows.Forms.Label lb00;
		private System.Windows.Forms.Label lFileIn;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.Label label23;
		private System.Windows.Forms.Label label22;
		private System.Windows.Forms.Label lbCG;
		private System.Windows.Forms.Label label27;
		private System.Windows.Forms.Label label26;
		private System.Windows.Forms.Label label29;
		private System.Windows.Forms.Label lbCGR;
		private System.Windows.Forms.CheckBox ck34;
		private System.Windows.Forms.CheckBox ck35;
		private System.Windows.Forms.CheckBox ck32;
		private System.Windows.Forms.CheckBox ck33;
		private System.Windows.Forms.CheckBox ck30;
		private System.Windows.Forms.CheckBox ck31;
		private System.Windows.Forms.Button bFG;
		public AnalizadorJPM() {
			InitializeComponent();
			myTimer = new Timer();
			myTimer.Interval = 3000;
			myTimer.Tick += new EventHandler(CalculoColumnas);
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
        }

		
		private int[] pot = new int[] {1,3,9,27,81,243,729,2187,6561,19683,59049,177147,531441,1594323};
		private int[] validas = new int[4782969];
		private BitArray repes = new BitArray(4782969);
		private int[] vals = new int[9];
		private int[] nprs = new int[5];
		private int[] colprs = new int[4782969];
		private string columna, tmp;
		private int conta, nx, nx2;
		private DateTime time0, time9;
		private int[] lbtab = new int[36];
		private StreamWriter sw = null;
		private bool[] marcas = new bool[36];
		private bool salida = false;
		private Timer myTimer;
		private int limcgsR, nrfCGR;
		private string[] colgsR = new string[3000];
		
		private void Iniciar() {
			bGrabarCols.Enabled=false;
			bIniciar.Enabled=false;
			lTime.Text = " ";
			salida=false;
			myTimer.Start(); time0 = DateTime.Now;
			lb00.Text=lb01.Text=lb02.Text=lb03.Text=lb04.Text=" ";
			lb05.Text=lb06.Text=lb07.Text=lb08.Text=lb09.Text=" ";
			lb10.Text=lb11.Text=lb12.Text=lb13.Text=lb14.Text=" ";
			lb15.Text=lb16.Text=lb17.Text=lb18.Text=lb19.Text=" ";
			lb20.Text=lb21.Text=lb22.Text=lb23.Text=lb24.Text=" ";
			lb25.Text=lb26.Text=lb27.Text=lb28.Text=lb29.Text=" ";
			lb30.Text=lb31.Text=lb32.Text=lb33.Text=lb34.Text=lb35.Text=" ";
			ck00.Checked=ck01.Checked=ck02.Checked=ck03.Checked=false;
			ck04.Checked=ck05.Checked=ck06.Checked=ck07.Checked=false;
			ck08.Checked=ck09.Checked=ck10.Checked=ck11.Checked=false;
			ck12.Checked=ck13.Checked=ck14.Checked=ck15.Checked=false;
			ck16.Checked=ck17.Checked=ck18.Checked=ck19.Checked=false;
			ck20.Checked=ck21.Checked=ck22.Checked=ck23.Checked=false;
			ck24.Checked=ck25.Checked=ck26.Checked=ck27.Checked=false;
			ck28.Checked=ck29.Checked=ck30.Checked=ck31.Checked=false;
			ck32.Checked=ck33.Checked=ck34.Checked=ck35.Checked=false;
			RecuperaValores();
			for (int nr=0; nr<4782969; nr++) validas[nr]=(-1);
			for (int nr=0; nr<36; nr++) lbtab[nr]=0;
			OpenFileDialog lee = new OpenFileDialog();
            lee.InitialDirectory = Application.StartupPath + "/";
			lee.Filter = "ColumnasEntrada(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
			if(lee.ShowDialog() == DialogResult.OK) {
				tmp = lee.FileName;
				string filein = Path.GetFileName(tmp);
				conta=0; lFileIn.Text = filein;
				repes.SetAll(false);
				StreamReader sr = new StreamReader(filein);
				bIniciar.Text="procesando...";
				while (sr.Peek()>0) {
					Application.DoEvents();
					if (salida) break;
					columna = sr.ReadLine().Trim();
					if (columna.Length < 14) {
						MessageBox.Show ("error de longitud="+columna);
						break;
					}
					nx = s1n(columna);
					if (repes[nx]==false) {
						nx2 = Puntuar(columna);
						validas[nx]=nx2;
						lbtab[nx2]++;
						conta++;
						repes[nx]=true;
					}
				}
				sr.Close();
				Application.DoEvents();
				Asignar();
			}
			myTimer.Stop();
			time9 = DateTime.Now;
			tmp = (time9-time0).ToString()+"00000000000";
			lTime.Text = tmp.Substring(0,11);
			lCol.Text = ""+(conta);
			bIniciar.Text="iniciar";
			bIniciar.Enabled=true;
			bGrabarCols.Enabled=true;
		}
		private int Puntuar(string col) {
			int rsl = 0; string tmp;
			for (int nr=0; nr<14; nr+=2) {
				tmp = col.Substring(nr,2);
				switch (tmp) {
						case "11": rsl += vals[0]; break;
						case "1X": rsl += vals[1]; break;
						case "12": rsl += vals[2]; break;
						case "X1": rsl += vals[3]; break;
						case "XX": rsl += vals[4]; break;
						case "X2": rsl += vals[5]; break;
						case "21": rsl += vals[6]; break;
						case "2X": rsl += vals[7]; break;
						case "22": rsl += vals[8]; break;
				}
			}
			return rsl;
		}
		private void Asignar() {
			lb00.Text=""+lbtab[0];
			lb01.Text=""+lbtab[1];
			lb02.Text=""+lbtab[2];
			lb03.Text=""+lbtab[3];
			lb04.Text=""+lbtab[4];
			lb05.Text=""+lbtab[5];
			lb06.Text=""+lbtab[6];
			lb07.Text=""+lbtab[7];
			lb08.Text=""+lbtab[8];
			lb09.Text=""+lbtab[9];
			lb10.Text=""+lbtab[10];
			lb11.Text=""+lbtab[11];
			lb12.Text=""+lbtab[12];
			lb13.Text=""+lbtab[13];
			lb14.Text=""+lbtab[14];
			lb15.Text=""+lbtab[15];
			lb16.Text=""+lbtab[16];
			lb17.Text=""+lbtab[17];
			lb18.Text=""+lbtab[18];
			lb19.Text=""+lbtab[19];
			lb20.Text=""+lbtab[20];
			lb21.Text=""+lbtab[21];
			lb22.Text=""+lbtab[22];
			lb23.Text=""+lbtab[23];
			lb24.Text=""+lbtab[24];
			lb25.Text=""+lbtab[25];
			lb26.Text=""+lbtab[26];
			lb27.Text=""+lbtab[27];
			lb28.Text=""+lbtab[28];
			lb29.Text=""+lbtab[29];
			lb30.Text=""+lbtab[30];
			lb31.Text=""+lbtab[31];
			lb32.Text=""+lbtab[32];
			lb33.Text=""+lbtab[33];
			lb34.Text=""+lbtab[34];
			lb35.Text=""+lbtab[35];
		}
		private void Grabar() {
			string fileout;
			bIniciar.Enabled=false;
			bGrabarCols.Enabled=false;
			bGrabarCols.Text = "grabando...";
			lFileOut.Text = "";
			SaveFileDialog grabacols = new SaveFileDialog();
            grabacols.InitialDirectory = Application.StartupPath + "/";
			grabacols.Filter = "ColumnasSalida(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
			if(grabacols.ShowDialog() == DialogResult.OK) {
				tmp = grabacols.FileName;
				fileout = Path.GetFileName(tmp);
				sw = new StreamWriter(fileout);
				for (int nr=0; nr<4782969; nr++) {
					nx=validas[nr];
					switch (nx) {
							case 0: if (ck00.Checked) Grab1Col(nr); break;
							case 1: if (ck01.Checked) Grab1Col(nr); break;
							case 2: if (ck02.Checked) Grab1Col(nr); break;
							case 3: if (ck03.Checked) Grab1Col(nr); break;
							case 4: if (ck04.Checked) Grab1Col(nr); break;
							case 5: if (ck05.Checked) Grab1Col(nr); break;
							case 6: if (ck06.Checked) Grab1Col(nr); break;
							case 7: if (ck07.Checked) Grab1Col(nr); break;
							case 8: if (ck08.Checked) Grab1Col(nr); break;
							case 9: if (ck09.Checked) Grab1Col(nr); break;
							case 10: if (ck10.Checked) Grab1Col(nr); break;
							case 11: if (ck11.Checked) Grab1Col(nr); break;
							case 12: if (ck12.Checked) Grab1Col(nr); break;
							case 13: if (ck13.Checked) Grab1Col(nr); break;
							case 14: if (ck14.Checked) Grab1Col(nr); break;
							case 15: if (ck15.Checked) Grab1Col(nr); break;
							case 16: if (ck16.Checked) Grab1Col(nr); break;
							case 17: if (ck17.Checked) Grab1Col(nr); break;
							case 18: if (ck18.Checked) Grab1Col(nr); break;
							case 19: if (ck19.Checked) Grab1Col(nr); break;
							case 20: if (ck20.Checked) Grab1Col(nr); break;
							case 21: if (ck21.Checked) Grab1Col(nr); break;
							case 22: if (ck22.Checked) Grab1Col(nr); break;
							case 23: if (ck23.Checked) Grab1Col(nr); break;
							case 24: if (ck24.Checked) Grab1Col(nr); break;
							case 25: if (ck25.Checked) Grab1Col(nr); break;
							case 26: if (ck26.Checked) Grab1Col(nr); break;
							case 27: if (ck27.Checked) Grab1Col(nr); break;
							case 28: if (ck28.Checked) Grab1Col(nr); break;
							case 29: if (ck29.Checked) Grab1Col(nr); break;
							case 30: if (ck30.Checked) Grab1Col(nr); break;
							case 31: if (ck31.Checked) Grab1Col(nr); break;
							case 32: if (ck32.Checked) Grab1Col(nr); break;
							case 33: if (ck33.Checked) Grab1Col(nr); break;
							case 34: if (ck34.Checked) Grab1Col(nr); break;
							case 35: if (ck35.Checked) Grab1Col(nr); break;
					}
					Application.DoEvents();
				}
				sw.Close();
				lFileOut.Text = fileout;
			}
			bGrabarCols.Text = "grabar";
			bGrabarCols.Enabled=true;
			bIniciar.Enabled=true;
		}
		private void Grab1Col(int nr) {
			columna = n1s(nr);
			sw.WriteLine(columna);
		}
		private void SumSel() {
			int suma=0;
			for (int nr=0; nr<36; nr++) {
				switch (nr) {
						case 0: if (ck00.Checked) suma+=lbtab[nr]; break;
						case 1: if (ck01.Checked) suma+=lbtab[nr]; break;
						case 2: if (ck02.Checked) suma+=lbtab[nr]; break;
						case 3: if (ck03.Checked) suma+=lbtab[nr]; break;
						case 4: if (ck04.Checked) suma+=lbtab[nr]; break;
						case 5: if (ck05.Checked) suma+=lbtab[nr]; break;
						case 6: if (ck06.Checked) suma+=lbtab[nr]; break;
						case 7: if (ck07.Checked) suma+=lbtab[nr]; break;
						case 8: if (ck08.Checked) suma+=lbtab[nr]; break;
						case 9: if (ck09.Checked) suma+=lbtab[nr]; break;
						case 10: if (ck10.Checked) suma+=lbtab[nr]; break;
						case 11: if (ck11.Checked) suma+=lbtab[nr]; break;
						case 12: if (ck12.Checked) suma+=lbtab[nr]; break;
						case 13: if (ck13.Checked) suma+=lbtab[nr]; break;
						case 14: if (ck14.Checked) suma+=lbtab[nr]; break;
						case 15: if (ck15.Checked) suma+=lbtab[nr]; break;
						case 16: if (ck16.Checked) suma+=lbtab[nr]; break;
						case 17: if (ck17.Checked) suma+=lbtab[nr]; break;
						case 18: if (ck18.Checked) suma+=lbtab[nr]; break;
						case 19: if (ck19.Checked) suma+=lbtab[nr]; break;
						case 20: if (ck20.Checked) suma+=lbtab[nr]; break;
						case 21: if (ck21.Checked) suma+=lbtab[nr]; break;
						case 22: if (ck22.Checked) suma+=lbtab[nr]; break;
						case 23: if (ck23.Checked) suma+=lbtab[nr]; break;
						case 24: if (ck24.Checked) suma+=lbtab[nr]; break;
						case 25: if (ck25.Checked) suma+=lbtab[nr]; break;
						case 26: if (ck26.Checked) suma+=lbtab[nr]; break;
						case 27: if (ck27.Checked) suma+=lbtab[nr]; break;
						case 28: if (ck28.Checked) suma+=lbtab[nr]; break;
						case 29: if (ck29.Checked) suma+=lbtab[nr]; break;
						case 30: if (ck30.Checked) suma+=lbtab[nr]; break;
						case 31: if (ck31.Checked) suma+=lbtab[nr]; break;
						case 32: if (ck32.Checked) suma+=lbtab[nr]; break;
						case 33: if (ck33.Checked) suma+=lbtab[nr]; break;
						case 34: if (ck34.Checked) suma+=lbtab[nr]; break;
						case 35: if (ck35.Checked) suma+=lbtab[nr]; break;
				}
			}
			lSumSel.Text = ""+suma;
			Application.DoEvents();
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
		private void Analizar() {
			bAnalizar.Visible=false;
			salida=false;
			RecuperaValores();
			columna = tbCG.Text.Trim().ToUpper();
			if (columna.Length < 14) {
				MessageBox.Show ("error de longitud="+columna);
				return;
			}
			for (int nr=0; nr<5; nr++) nprs[nr]=0;
			for (int nr=0; nr<4782969; nr++) colprs[nr]=0;
			int ncol = s1n(columna);
			nx2 = Puntuar(columna);
			if (marcas[nx2]) {
				colprs[ncol]=14; nprs[0]++;
			}
			lbCG.Text = ""+nx2;
			BuscaPremios(ncol,13);
			lpr14.Text=""+nprs[0];
			lpr13.Text=""+nprs[1];
			lpr12.Text=""+(nprs[2]-nprs[1]);
			lpr11.Text=""+(nprs[3]-nprs[2]);
			lpr10.Text=""+(nprs[4]-nprs[3]);
			bAnalizar.Visible=true;
		}
		private void BuscaPremios(int col0, int nprof) {
			int x1,z1,col1,sign1, pos;
			Application.DoEvents();
			if (salida) return;
			for (int nr0=0; nr0<14; nr0++) {
				x1 = pot[nr0];
				sign1 = (col0 / x1) % 3;
				for (z1=0; z1<3; z1++) {
					col1 = col0 + x1 * (z1 - sign1);
					if (colprs[col1]<nprof) {
						colprs[col1]=nprof;
						pos = Puntuar(n1s(col1));				// estas Dos lineas
						if (marcas[pos]) nprs[14-nprof]++;		// dependen de la condición
					}
					if (nprof>10) BuscaPremios(col1, nprof-1);
				}
			}
		}
		private void RecuperaValores() {
			vals[0] = Convert.ToInt32(tbv0.Text);
			vals[1] = Convert.ToInt32(tbv1.Text);
			vals[2] = Convert.ToInt32(tbv2.Text);
			vals[3] = Convert.ToInt32(tbv3.Text);
			vals[4] = Convert.ToInt32(tbv4.Text);
			vals[5] = Convert.ToInt32(tbv5.Text);
			vals[6] = Convert.ToInt32(tbv6.Text);
			vals[7] = Convert.ToInt32(tbv7.Text);
			vals[8] = Convert.ToInt32(tbv8.Text);
			for (int nr=0; nr<9; nr++) {
				if (vals[nr]>5) vals[nr]=5;
				if (vals[nr]<0) vals[nr]=0;
			}
			for (int nr=0; nr<36; nr++) marcas[nr]=false;
			if (ck00.Checked) marcas[0]=true;
			if (ck01.Checked) marcas[1]=true;
			if (ck02.Checked) marcas[2]=true;
			if (ck03.Checked) marcas[3]=true;
			if (ck04.Checked) marcas[4]=true;
			if (ck05.Checked) marcas[5]=true;
			if (ck06.Checked) marcas[6]=true;
			if (ck07.Checked) marcas[7]=true;
			if (ck08.Checked) marcas[8]=true;
			if (ck09.Checked) marcas[9]=true;
			if (ck10.Checked) marcas[10]=true;
			if (ck11.Checked) marcas[11]=true;
			if (ck12.Checked) marcas[12]=true;
			if (ck13.Checked) marcas[13]=true;
			if (ck14.Checked) marcas[14]=true;
			if (ck15.Checked) marcas[15]=true;
			if (ck16.Checked) marcas[16]=true;
			if (ck17.Checked) marcas[17]=true;
			if (ck18.Checked) marcas[18]=true;
			if (ck19.Checked) marcas[19]=true;
			if (ck20.Checked) marcas[20]=true;
			if (ck21.Checked) marcas[21]=true;
			if (ck22.Checked) marcas[22]=true;
			if (ck23.Checked) marcas[23]=true;
			if (ck24.Checked) marcas[24]=true;
			if (ck25.Checked) marcas[25]=true;
			if (ck26.Checked) marcas[26]=true;
			if (ck27.Checked) marcas[27]=true;
			if (ck28.Checked) marcas[28]=true;
			if (ck29.Checked) marcas[29]=true;
			if (ck30.Checked) marcas[30]=true;
			if (ck31.Checked) marcas[31]=true;
			if (ck32.Checked) marcas[32]=true;
			if (ck33.Checked) marcas[33]=true;
			if (ck34.Checked) marcas[34]=true;
			if (ck35.Checked) marcas[35]=true;
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
		private string VerColumna (string columna) {
			string chval = "12xX", xcol; char ch;
			xcol = columna.Trim();
			if (xcol.Length<14) return "";
			xcol = xcol.Substring(0,14);
			for (int nr=0; nr<14; nr++) {
				ch = xcol[nr];
				if (chval.IndexOf(ch)<0) return "";
			}
			return xcol;
		}
		
//		[STAThread]
//		public static void Main(string[] args) { Application.Run(new AnalizadorJPM()); }
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnalizadorJPM));
            this.bFG = new System.Windows.Forms.Button();
            this.ck31 = new System.Windows.Forms.CheckBox();
            this.ck30 = new System.Windows.Forms.CheckBox();
            this.ck33 = new System.Windows.Forms.CheckBox();
            this.ck32 = new System.Windows.Forms.CheckBox();
            this.ck35 = new System.Windows.Forms.CheckBox();
            this.ck34 = new System.Windows.Forms.CheckBox();
            this.lbCGR = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.lbCG = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.lFileIn = new System.Windows.Forms.Label();
            this.lb00 = new System.Windows.Forms.Label();
            this.ck08 = new System.Windows.Forms.CheckBox();
            this.label51 = new System.Windows.Forms.Label();
            this.label50 = new System.Windows.Forms.Label();
            this.label53 = new System.Windows.Forms.Label();
            this.label55 = new System.Windows.Forms.Label();
            this.label57 = new System.Windows.Forms.Label();
            this.tbCG = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.lb12 = new System.Windows.Forms.Label();
            this.lb22 = new System.Windows.Forms.Label();
            this.lb20 = new System.Windows.Forms.Label();
            this.label48 = new System.Windows.Forms.Label();
            this.label49 = new System.Windows.Forms.Label();
            this.label61 = new System.Windows.Forms.Label();
            this.lTime = new System.Windows.Forms.Label();
            this.label41 = new System.Windows.Forms.Label();
            this.label42 = new System.Windows.Forms.Label();
            this.label43 = new System.Windows.Forms.Label();
            this.label44 = new System.Windows.Forms.Label();
            this.ck13 = new System.Windows.Forms.CheckBox();
            this.ck12 = new System.Windows.Forms.CheckBox();
            this.label47 = new System.Windows.Forms.Label();
            this.ck10 = new System.Windows.Forms.CheckBox();
            this.ck17 = new System.Windows.Forms.CheckBox();
            this.ck16 = new System.Windows.Forms.CheckBox();
            this.ck15 = new System.Windows.Forms.CheckBox();
            this.ck14 = new System.Windows.Forms.CheckBox();
            this.label59 = new System.Windows.Forms.Label();
            this.ck19 = new System.Windows.Forms.CheckBox();
            this.ck18 = new System.Windows.Forms.CheckBox();
            this.label25 = new System.Windows.Forms.Label();
            this.bIniciar = new System.Windows.Forms.Button();
            this.lb08 = new System.Windows.Forms.Label();
            this.lb15 = new System.Windows.Forms.Label();
            this.lb14 = new System.Windows.Forms.Label();
            this.lb17 = new System.Windows.Forms.Label();
            this.lb16 = new System.Windows.Forms.Label();
            this.lb11 = new System.Windows.Forms.Label();
            this.lb10 = new System.Windows.Forms.Label();
            this.ck11 = new System.Windows.Forms.CheckBox();
            this.lFGR = new System.Windows.Forms.Label();
            this.tbv8 = new System.Windows.Forms.TextBox();
            this.lb18 = new System.Windows.Forms.Label();
            this.tbv2 = new System.Windows.Forms.TextBox();
            this.tbv3 = new System.Windows.Forms.TextBox();
            this.tbv0 = new System.Windows.Forms.TextBox();
            this.tbv1 = new System.Windows.Forms.TextBox();
            this.bGrabarCols = new System.Windows.Forms.Button();
            this.tbv7 = new System.Windows.Forms.TextBox();
            this.tbv4 = new System.Windows.Forms.TextBox();
            this.tbv5 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label45 = new System.Windows.Forms.Label();
            this.lpr14 = new System.Windows.Forms.Label();
            this.lb06 = new System.Windows.Forms.Label();
            this.lb07 = new System.Windows.Forms.Label();
            this.lpr11 = new System.Windows.Forms.Label();
            this.bMasR = new System.Windows.Forms.Button();
            this.lpr13 = new System.Windows.Forms.Label();
            this.lb03 = new System.Windows.Forms.Label();
            this.bSumar = new System.Windows.Forms.Button();
            this.label28 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.tbv6 = new System.Windows.Forms.TextBox();
            this.label24 = new System.Windows.Forms.Label();
            this.ck20 = new System.Windows.Forms.CheckBox();
            this.ck21 = new System.Windows.Forms.CheckBox();
            this.ck22 = new System.Windows.Forms.CheckBox();
            this.ck23 = new System.Windows.Forms.CheckBox();
            this.ck24 = new System.Windows.Forms.CheckBox();
            this.ck25 = new System.Windows.Forms.CheckBox();
            this.ck26 = new System.Windows.Forms.CheckBox();
            this.ck27 = new System.Windows.Forms.CheckBox();
            this.ck28 = new System.Windows.Forms.CheckBox();
            this.ck29 = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lb33 = new System.Windows.Forms.Label();
            this.lb04 = new System.Windows.Forms.Label();
            this.lb02 = new System.Windows.Forms.Label();
            this.label40 = new System.Windows.Forms.Label();
            this.lb01 = new System.Windows.Forms.Label();
            this.ck09 = new System.Windows.Forms.CheckBox();
            this.lpr10 = new System.Windows.Forms.Label();
            this.label46 = new System.Windows.Forms.Label();
            this.lpr12 = new System.Windows.Forms.Label();
            this.bCancelar = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label38 = new System.Windows.Forms.Label();
            this.lb26 = new System.Windows.Forms.Label();
            this.lb27 = new System.Windows.Forms.Label();
            this.lb24 = new System.Windows.Forms.Label();
            this.lb25 = new System.Windows.Forms.Label();
            this.lb13 = new System.Windows.Forms.Label();
            this.lb23 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label36 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.lb21 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.bMenosR = new System.Windows.Forms.Button();
            this.bAnalizar = new System.Windows.Forms.Button();
            this.lSumSel = new System.Windows.Forms.Label();
            this.lb19 = new System.Windows.Forms.Label();
            this.lb28 = new System.Windows.Forms.Label();
            this.lb29 = new System.Windows.Forms.Label();
            this.lFileOut = new System.Windows.Forms.Label();
            this.lb34 = new System.Windows.Forms.Label();
            this.lb35 = new System.Windows.Forms.Label();
            this.lb32 = new System.Windows.Forms.Label();
            this.lb31 = new System.Windows.Forms.Label();
            this.lb30 = new System.Windows.Forms.Label();
            this.ck03 = new System.Windows.Forms.CheckBox();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.ck06 = new System.Windows.Forms.CheckBox();
            this.ck07 = new System.Windows.Forms.CheckBox();
            this.ck04 = new System.Windows.Forms.CheckBox();
            this.ck05 = new System.Windows.Forms.CheckBox();
            this.lb05 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.lb09 = new System.Windows.Forms.Label();
            this.ck02 = new System.Windows.Forms.CheckBox();
            this.lCol = new System.Windows.Forms.Label();
            this.ck00 = new System.Windows.Forms.CheckBox();
            this.ck01 = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // bFG
            // 
            this.bFG.BackColor = System.Drawing.Color.DarkSalmon;
            this.bFG.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bFG.Image = ((System.Drawing.Image)(resources.GetObject("bFG.Image")));
            this.bFG.Location = new System.Drawing.Point(8, 24);
            this.bFG.Name = "bFG";
            this.bFG.Size = new System.Drawing.Size(24, 24);
            this.bFG.TabIndex = 87;
            this.bFG.UseVisualStyleBackColor = false;
            this.bFG.Click += new System.EventHandler(this.BFGClick);
            // 
            // ck31
            // 
            this.ck31.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck31.Location = new System.Drawing.Point(184, 232);
            this.ck31.Name = "ck31";
            this.ck31.Size = new System.Drawing.Size(16, 16);
            this.ck31.TabIndex = 107;
            // 
            // ck30
            // 
            this.ck30.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck30.Location = new System.Drawing.Point(184, 215);
            this.ck30.Name = "ck30";
            this.ck30.Size = new System.Drawing.Size(16, 16);
            this.ck30.TabIndex = 104;
            // 
            // ck33
            // 
            this.ck33.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck33.Location = new System.Drawing.Point(184, 266);
            this.ck33.Name = "ck33";
            this.ck33.Size = new System.Drawing.Size(16, 16);
            this.ck33.TabIndex = 113;
            // 
            // ck32
            // 
            this.ck32.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck32.Location = new System.Drawing.Point(184, 249);
            this.ck32.Name = "ck32";
            this.ck32.Size = new System.Drawing.Size(16, 16);
            this.ck32.TabIndex = 110;
            // 
            // ck35
            // 
            this.ck35.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck35.Location = new System.Drawing.Point(184, 300);
            this.ck35.Name = "ck35";
            this.ck35.Size = new System.Drawing.Size(16, 16);
            this.ck35.TabIndex = 119;
            // 
            // ck34
            // 
            this.ck34.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck34.Location = new System.Drawing.Point(184, 283);
            this.ck34.Name = "ck34";
            this.ck34.Size = new System.Drawing.Size(16, 16);
            this.ck34.TabIndex = 116;
            // 
            // lbCGR
            // 
            this.lbCGR.BackColor = System.Drawing.SystemColors.Info;
            this.lbCGR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbCGR.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCGR.Location = new System.Drawing.Point(120, 80);
            this.lbCGR.Name = "lbCGR";
            this.lbCGR.Size = new System.Drawing.Size(32, 32);
            this.lbCGR.TabIndex = 86;
            this.lbCGR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label29
            // 
            this.label29.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label29.Location = new System.Drawing.Point(8, 283);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(32, 16);
            this.label29.TabIndex = 52;
            this.label29.Text = "16";
            this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label26
            // 
            this.label26.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.Location = new System.Drawing.Point(152, 198);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(32, 16);
            this.label26.TabIndex = 99;
            this.label26.Text = "29";
            this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label27
            // 
            this.label27.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label27.Location = new System.Drawing.Point(8, 300);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(32, 16);
            this.label27.TabIndex = 55;
            this.label27.Text = "17";
            this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lbCG
            // 
            this.lbCG.BackColor = System.Drawing.SystemColors.Info;
            this.lbCG.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbCG.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCG.Location = new System.Drawing.Point(8, 48);
            this.lbCG.Name = "lbCG";
            this.lbCG.Size = new System.Drawing.Size(32, 24);
            this.lbCG.TabIndex = 11;
            this.lbCG.Text = "-";
            this.lbCG.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label22
            // 
            this.label22.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.Location = new System.Drawing.Point(152, 232);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(32, 16);
            this.label22.TabIndex = 105;
            this.label22.Text = "31";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label23
            // 
            this.label23.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.Location = new System.Drawing.Point(152, 28);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(32, 16);
            this.label23.TabIndex = 61;
            this.label23.Text = "19";
            this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label20
            // 
            this.label20.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(152, 249);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(32, 16);
            this.label20.TabIndex = 108;
            this.label20.Text = "32";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label21
            // 
            this.label21.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.Location = new System.Drawing.Point(8, 164);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(32, 16);
            this.label21.TabIndex = 31;
            this.label21.Text = "9";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lFileIn
            // 
            this.lFileIn.BackColor = System.Drawing.SystemColors.Info;
            this.lFileIn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lFileIn.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lFileIn.Location = new System.Drawing.Point(312, 152);
            this.lFileIn.Name = "lFileIn";
            this.lFileIn.Size = new System.Drawing.Size(104, 24);
            this.lFileIn.TabIndex = 94;
            this.lFileIn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb00
            // 
            this.lb00.BackColor = System.Drawing.SystemColors.Info;
            this.lb00.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb00.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb00.Location = new System.Drawing.Point(64, 11);
            this.lb00.Name = "lb00";
            this.lb00.Size = new System.Drawing.Size(80, 16);
            this.lb00.TabIndex = 1;
            this.lb00.Text = "0";
            this.lb00.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ck08
            // 
            this.ck08.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck08.Location = new System.Drawing.Point(40, 147);
            this.ck08.Name = "ck08";
            this.ck08.Size = new System.Drawing.Size(16, 16);
            this.ck08.TabIndex = 30;
            // 
            // label51
            // 
            this.label51.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label51.Location = new System.Drawing.Point(152, 130);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(32, 16);
            this.label51.TabIndex = 79;
            this.label51.Text = "25";
            this.label51.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label50
            // 
            this.label50.BackColor = System.Drawing.Color.Bisque;
            this.label50.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label50.Location = new System.Drawing.Point(272, 32);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(32, 16);
            this.label50.TabIndex = 22;
            this.label50.Text = "10";
            this.label50.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label53
            // 
            this.label53.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label53.Location = new System.Drawing.Point(152, 113);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(32, 16);
            this.label53.TabIndex = 76;
            this.label53.Text = "24";
            this.label53.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label55
            // 
            this.label55.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label55.Location = new System.Drawing.Point(152, 96);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(32, 16);
            this.label55.TabIndex = 73;
            this.label55.Text = "23";
            this.label55.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label57
            // 
            this.label57.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label57.Location = new System.Drawing.Point(152, 79);
            this.label57.Name = "label57";
            this.label57.Size = new System.Drawing.Size(32, 16);
            this.label57.TabIndex = 70;
            this.label57.Text = "22";
            this.label57.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbCG
            // 
            this.tbCG.BackColor = System.Drawing.SystemColors.Window;
            this.tbCG.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbCG.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tbCG.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbCG.Location = new System.Drawing.Point(32, 49);
            this.tbCG.MaxLength = 14;
            this.tbCG.Name = "tbCG";
            this.tbCG.Size = new System.Drawing.Size(144, 21);
            this.tbCG.TabIndex = 373;
            this.tbCG.Text = "COL.GANADORA";
            this.tbCG.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(66, 24);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(24, 23);
            this.label10.TabIndex = 141;
            this.label10.Text = "12";
            this.label10.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // label12
            // 
            this.label12.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(91, 24);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(24, 23);
            this.label12.TabIndex = 140;
            this.label12.Text = "x1";
            this.label12.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // lb12
            // 
            this.lb12.BackColor = System.Drawing.SystemColors.Info;
            this.lb12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb12.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb12.Location = new System.Drawing.Point(64, 215);
            this.lb12.Name = "lb12";
            this.lb12.Size = new System.Drawing.Size(80, 16);
            this.lb12.TabIndex = 41;
            this.lb12.Text = "0";
            this.lb12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lb22
            // 
            this.lb22.BackColor = System.Drawing.SystemColors.Info;
            this.lb22.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb22.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb22.Location = new System.Drawing.Point(208, 79);
            this.lb22.Name = "lb22";
            this.lb22.Size = new System.Drawing.Size(80, 16);
            this.lb22.TabIndex = 71;
            this.lb22.Text = "0";
            this.lb22.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lb20
            // 
            this.lb20.BackColor = System.Drawing.SystemColors.Info;
            this.lb20.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb20.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb20.Location = new System.Drawing.Point(208, 45);
            this.lb20.Name = "lb20";
            this.lb20.Size = new System.Drawing.Size(80, 16);
            this.lb20.TabIndex = 65;
            this.lb20.Text = "0";
            this.lb20.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label48
            // 
            this.label48.BackColor = System.Drawing.Color.Bisque;
            this.label48.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label48.Location = new System.Drawing.Point(200, 32);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(32, 16);
            this.label48.TabIndex = 21;
            this.label48.Text = "11";
            this.label48.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // label49
            // 
            this.label49.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label49.Location = new System.Drawing.Point(152, 147);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(32, 16);
            this.label49.TabIndex = 82;
            this.label49.Text = "26";
            this.label49.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label61
            // 
            this.label61.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label61.Location = new System.Drawing.Point(152, 45);
            this.label61.Name = "label61";
            this.label61.Size = new System.Drawing.Size(32, 16);
            this.label61.TabIndex = 64;
            this.label61.Text = "20";
            this.label61.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lTime
            // 
            this.lTime.BackColor = System.Drawing.SystemColors.Info;
            this.lTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lTime.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lTime.Location = new System.Drawing.Point(312, 202);
            this.lTime.Name = "lTime";
            this.lTime.Size = new System.Drawing.Size(104, 24);
            this.lTime.TabIndex = 6;
            this.lTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label41
            // 
            this.label41.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label41.Location = new System.Drawing.Point(8, 181);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(32, 16);
            this.label41.TabIndex = 34;
            this.label41.Text = "10";
            this.label41.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label42
            // 
            this.label42.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label42.Location = new System.Drawing.Point(191, 24);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(24, 23);
            this.label42.TabIndex = 144;
            this.label42.Text = "2x";
            this.label42.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // label43
            // 
            this.label43.BackColor = System.Drawing.Color.Bisque;
            this.label43.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label43.Location = new System.Drawing.Point(48, 32);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(32, 16);
            this.label43.TabIndex = 18;
            this.label43.Text = "14";
            this.label43.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // label44
            // 
            this.label44.BackColor = System.Drawing.Color.Bisque;
            this.label44.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label44.Location = new System.Drawing.Point(88, 32);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(32, 16);
            this.label44.TabIndex = 19;
            this.label44.Text = "13";
            this.label44.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // ck13
            // 
            this.ck13.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck13.Location = new System.Drawing.Point(40, 232);
            this.ck13.Name = "ck13";
            this.ck13.Size = new System.Drawing.Size(16, 16);
            this.ck13.TabIndex = 45;
            // 
            // ck12
            // 
            this.ck12.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck12.Location = new System.Drawing.Point(40, 215);
            this.ck12.Name = "ck12";
            this.ck12.Size = new System.Drawing.Size(16, 16);
            this.ck12.TabIndex = 42;
            // 
            // label47
            // 
            this.label47.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label47.Location = new System.Drawing.Point(152, 164);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(32, 16);
            this.label47.TabIndex = 85;
            this.label47.Text = "27";
            this.label47.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ck10
            // 
            this.ck10.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck10.Location = new System.Drawing.Point(40, 181);
            this.ck10.Name = "ck10";
            this.ck10.Size = new System.Drawing.Size(16, 16);
            this.ck10.TabIndex = 36;
            // 
            // ck17
            // 
            this.ck17.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck17.Location = new System.Drawing.Point(40, 300);
            this.ck17.Name = "ck17";
            this.ck17.Size = new System.Drawing.Size(16, 16);
            this.ck17.TabIndex = 57;
            // 
            // ck16
            // 
            this.ck16.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck16.Location = new System.Drawing.Point(40, 283);
            this.ck16.Name = "ck16";
            this.ck16.Size = new System.Drawing.Size(16, 16);
            this.ck16.TabIndex = 54;
            // 
            // ck15
            // 
            this.ck15.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck15.Location = new System.Drawing.Point(40, 266);
            this.ck15.Name = "ck15";
            this.ck15.Size = new System.Drawing.Size(16, 16);
            this.ck15.TabIndex = 51;
            // 
            // ck14
            // 
            this.ck14.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck14.Location = new System.Drawing.Point(40, 249);
            this.ck14.Name = "ck14";
            this.ck14.Size = new System.Drawing.Size(16, 16);
            this.ck14.TabIndex = 48;
            // 
            // label59
            // 
            this.label59.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label59.Location = new System.Drawing.Point(152, 62);
            this.label59.Name = "label59";
            this.label59.Size = new System.Drawing.Size(32, 16);
            this.label59.TabIndex = 67;
            this.label59.Text = "21";
            this.label59.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ck19
            // 
            this.ck19.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck19.Location = new System.Drawing.Point(184, 28);
            this.ck19.Name = "ck19";
            this.ck19.Size = new System.Drawing.Size(16, 16);
            this.ck19.TabIndex = 63;
            // 
            // ck18
            // 
            this.ck18.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck18.Location = new System.Drawing.Point(184, 11);
            this.ck18.Name = "ck18";
            this.ck18.Size = new System.Drawing.Size(16, 16);
            this.ck18.TabIndex = 60;
            // 
            // label25
            // 
            this.label25.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.Location = new System.Drawing.Point(152, 11);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(32, 16);
            this.label25.TabIndex = 58;
            this.label25.Text = "18";
            this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // bIniciar
            // 
            this.bIniciar.BackColor = System.Drawing.Color.DarkSalmon;
            this.bIniciar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bIniciar.Image = ((System.Drawing.Image)(resources.GetObject("bIniciar.Image")));
            this.bIniciar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bIniciar.Location = new System.Drawing.Point(312, 120);
            this.bIniciar.Name = "bIniciar";
            this.bIniciar.Size = new System.Drawing.Size(104, 32);
            this.bIniciar.TabIndex = 3;
            this.bIniciar.Text = "Iniciar";
            this.bIniciar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bIniciar.UseVisualStyleBackColor = false;
            this.bIniciar.Click += new System.EventHandler(this.BIniciarClick);
            // 
            // lb08
            // 
            this.lb08.BackColor = System.Drawing.SystemColors.Info;
            this.lb08.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb08.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb08.Location = new System.Drawing.Point(64, 147);
            this.lb08.Name = "lb08";
            this.lb08.Size = new System.Drawing.Size(80, 16);
            this.lb08.TabIndex = 29;
            this.lb08.Text = "0";
            this.lb08.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lb15
            // 
            this.lb15.BackColor = System.Drawing.SystemColors.Info;
            this.lb15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb15.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb15.Location = new System.Drawing.Point(64, 266);
            this.lb15.Name = "lb15";
            this.lb15.Size = new System.Drawing.Size(80, 16);
            this.lb15.TabIndex = 50;
            this.lb15.Text = "0";
            this.lb15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lb14
            // 
            this.lb14.BackColor = System.Drawing.SystemColors.Info;
            this.lb14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb14.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb14.Location = new System.Drawing.Point(64, 249);
            this.lb14.Name = "lb14";
            this.lb14.Size = new System.Drawing.Size(80, 16);
            this.lb14.TabIndex = 47;
            this.lb14.Text = "0";
            this.lb14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lb17
            // 
            this.lb17.BackColor = System.Drawing.SystemColors.Info;
            this.lb17.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb17.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb17.Location = new System.Drawing.Point(64, 300);
            this.lb17.Name = "lb17";
            this.lb17.Size = new System.Drawing.Size(80, 16);
            this.lb17.TabIndex = 56;
            this.lb17.Text = "0";
            this.lb17.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lb16
            // 
            this.lb16.BackColor = System.Drawing.SystemColors.Info;
            this.lb16.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb16.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb16.Location = new System.Drawing.Point(64, 283);
            this.lb16.Name = "lb16";
            this.lb16.Size = new System.Drawing.Size(80, 16);
            this.lb16.TabIndex = 53;
            this.lb16.Text = "0";
            this.lb16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lb11
            // 
            this.lb11.BackColor = System.Drawing.SystemColors.Info;
            this.lb11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb11.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb11.Location = new System.Drawing.Point(64, 198);
            this.lb11.Name = "lb11";
            this.lb11.Size = new System.Drawing.Size(80, 16);
            this.lb11.TabIndex = 38;
            this.lb11.Text = "0";
            this.lb11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lb10
            // 
            this.lb10.BackColor = System.Drawing.SystemColors.Info;
            this.lb10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb10.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb10.Location = new System.Drawing.Point(64, 181);
            this.lb10.Name = "lb10";
            this.lb10.Size = new System.Drawing.Size(80, 16);
            this.lb10.TabIndex = 35;
            this.lb10.Text = "0";
            this.lb10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ck11
            // 
            this.ck11.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck11.Location = new System.Drawing.Point(40, 198);
            this.ck11.Name = "ck11";
            this.ck11.Size = new System.Drawing.Size(16, 16);
            this.ck11.TabIndex = 39;
            // 
            // lFGR
            // 
            this.lFGR.BackColor = System.Drawing.SystemColors.Info;
            this.lFGR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lFGR.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lFGR.Location = new System.Drawing.Point(32, 24);
            this.lFGR.Name = "lFGR";
            this.lFGR.Size = new System.Drawing.Size(144, 24);
            this.lFGR.TabIndex = 88;
            this.lFGR.Text = "Fichero Ganadoras";
            this.lFGR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbv8
            // 
            this.tbv8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbv8.Location = new System.Drawing.Point(216, 48);
            this.tbv8.Name = "tbv8";
            this.tbv8.Size = new System.Drawing.Size(24, 23);
            this.tbv8.TabIndex = 132;
            this.tbv8.Text = "5";
            this.tbv8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lb18
            // 
            this.lb18.BackColor = System.Drawing.SystemColors.Info;
            this.lb18.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb18.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb18.Location = new System.Drawing.Point(208, 11);
            this.lb18.Name = "lb18";
            this.lb18.Size = new System.Drawing.Size(80, 16);
            this.lb18.TabIndex = 59;
            this.lb18.Text = "0";
            this.lb18.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tbv2
            // 
            this.tbv2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbv2.Location = new System.Drawing.Point(66, 48);
            this.tbv2.Name = "tbv2";
            this.tbv2.Size = new System.Drawing.Size(24, 23);
            this.tbv2.TabIndex = 124;
            this.tbv2.Text = "2";
            this.tbv2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbv3
            // 
            this.tbv3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbv3.Location = new System.Drawing.Point(91, 48);
            this.tbv3.Name = "tbv3";
            this.tbv3.Size = new System.Drawing.Size(24, 23);
            this.tbv3.TabIndex = 125;
            this.tbv3.Text = "1";
            this.tbv3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbv0
            // 
            this.tbv0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbv0.Location = new System.Drawing.Point(16, 48);
            this.tbv0.Name = "tbv0";
            this.tbv0.Size = new System.Drawing.Size(24, 23);
            this.tbv0.TabIndex = 122;
            this.tbv0.Text = "0";
            this.tbv0.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbv1
            // 
            this.tbv1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbv1.Location = new System.Drawing.Point(41, 48);
            this.tbv1.Name = "tbv1";
            this.tbv1.Size = new System.Drawing.Size(24, 23);
            this.tbv1.TabIndex = 123;
            this.tbv1.Text = "1";
            this.tbv1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bGrabarCols
            // 
            this.bGrabarCols.BackColor = System.Drawing.Color.DarkSalmon;
            this.bGrabarCols.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bGrabarCols.Image = ((System.Drawing.Image)(resources.GetObject("bGrabarCols.Image")));
            this.bGrabarCols.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bGrabarCols.Location = new System.Drawing.Point(448, 200);
            this.bGrabarCols.Name = "bGrabarCols";
            this.bGrabarCols.Size = new System.Drawing.Size(104, 32);
            this.bGrabarCols.TabIndex = 4;
            this.bGrabarCols.Text = "Grabar";
            this.bGrabarCols.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bGrabarCols.UseVisualStyleBackColor = false;
            this.bGrabarCols.Click += new System.EventHandler(this.BGrabarClick);
            // 
            // tbv7
            // 
            this.tbv7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbv7.Location = new System.Drawing.Point(191, 48);
            this.tbv7.Name = "tbv7";
            this.tbv7.Size = new System.Drawing.Size(24, 23);
            this.tbv7.TabIndex = 131;
            this.tbv7.Text = "4";
            this.tbv7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbv4
            // 
            this.tbv4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbv4.Location = new System.Drawing.Point(116, 48);
            this.tbv4.Name = "tbv4";
            this.tbv4.Size = new System.Drawing.Size(24, 23);
            this.tbv4.TabIndex = 126;
            this.tbv4.Text = "3";
            this.tbv4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbv5
            // 
            this.tbv5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbv5.Location = new System.Drawing.Point(141, 48);
            this.tbv5.Name = "tbv5";
            this.tbv5.Size = new System.Drawing.Size(24, 23);
            this.tbv5.TabIndex = 127;
            this.tbv5.Text = "4";
            this.tbv5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.NavajoWhite;
            this.button2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(160, 80);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(16, 16);
            this.button2.TabIndex = 84;
            this.button2.Text = "+";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.BMasRClick);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.NavajoWhite;
            this.button3.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(8, 24);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(24, 24);
            this.button3.TabIndex = 87;
            this.button3.Text = "L";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.BFGClick);
            // 
            // label45
            // 
            this.label45.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label45.Location = new System.Drawing.Point(152, 181);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(32, 16);
            this.label45.TabIndex = 88;
            this.label45.Text = "28";
            this.label45.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lpr14
            // 
            this.lpr14.BackColor = System.Drawing.SystemColors.Info;
            this.lpr14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lpr14.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lpr14.Location = new System.Drawing.Point(48, 48);
            this.lpr14.Name = "lpr14";
            this.lpr14.Size = new System.Drawing.Size(32, 24);
            this.lpr14.TabIndex = 16;
            this.lpr14.Text = "-";
            this.lpr14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb06
            // 
            this.lb06.BackColor = System.Drawing.SystemColors.Info;
            this.lb06.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb06.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb06.Location = new System.Drawing.Point(64, 113);
            this.lb06.Name = "lb06";
            this.lb06.Size = new System.Drawing.Size(80, 16);
            this.lb06.TabIndex = 23;
            this.lb06.Text = "0";
            this.lb06.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lb07
            // 
            this.lb07.BackColor = System.Drawing.SystemColors.Info;
            this.lb07.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb07.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb07.Location = new System.Drawing.Point(64, 130);
            this.lb07.Name = "lb07";
            this.lb07.Size = new System.Drawing.Size(80, 16);
            this.lb07.TabIndex = 26;
            this.lb07.Text = "0";
            this.lb07.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lpr11
            // 
            this.lpr11.BackColor = System.Drawing.SystemColors.Info;
            this.lpr11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lpr11.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lpr11.Location = new System.Drawing.Point(192, 48);
            this.lpr11.Name = "lpr11";
            this.lpr11.Size = new System.Drawing.Size(56, 24);
            this.lpr11.TabIndex = 13;
            this.lpr11.Text = "-";
            this.lpr11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bMasR
            // 
            this.bMasR.BackColor = System.Drawing.Color.DarkSalmon;
            this.bMasR.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bMasR.Location = new System.Drawing.Point(160, 80);
            this.bMasR.Name = "bMasR";
            this.bMasR.Size = new System.Drawing.Size(16, 16);
            this.bMasR.TabIndex = 84;
            this.bMasR.Text = "+";
            this.bMasR.UseVisualStyleBackColor = false;
            this.bMasR.Click += new System.EventHandler(this.BMasRClick);
            // 
            // lpr13
            // 
            this.lpr13.BackColor = System.Drawing.SystemColors.Info;
            this.lpr13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lpr13.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lpr13.Location = new System.Drawing.Point(88, 48);
            this.lpr13.Name = "lpr13";
            this.lpr13.Size = new System.Drawing.Size(40, 24);
            this.lpr13.TabIndex = 15;
            this.lpr13.Text = "-";
            this.lpr13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb03
            // 
            this.lb03.BackColor = System.Drawing.SystemColors.Info;
            this.lb03.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb03.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb03.Location = new System.Drawing.Point(64, 62);
            this.lb03.Name = "lb03";
            this.lb03.Size = new System.Drawing.Size(80, 16);
            this.lb03.TabIndex = 14;
            this.lb03.Text = "0";
            this.lb03.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // bSumar
            // 
            this.bSumar.BackColor = System.Drawing.Color.DarkSalmon;
            this.bSumar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bSumar.Image = ((System.Drawing.Image)(resources.GetObject("bSumar.Image")));
            this.bSumar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bSumar.Location = new System.Drawing.Point(448, 120);
            this.bSumar.Name = "bSumar";
            this.bSumar.Size = new System.Drawing.Size(104, 32);
            this.bSumar.TabIndex = 93;
            this.bSumar.Text = "Sumar";
            this.bSumar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bSumar.UseVisualStyleBackColor = false;
            this.bSumar.Click += new System.EventHandler(this.BSumarClick);
            // 
            // label28
            // 
            this.label28.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label28.Location = new System.Drawing.Point(116, 24);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(24, 23);
            this.label28.TabIndex = 143;
            this.label28.Text = "xx";
            this.label28.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // label6
            // 
            this.label6.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(16, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(24, 23);
            this.label6.TabIndex = 139;
            this.label6.Text = "11";
            this.label6.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // tbv6
            // 
            this.tbv6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbv6.Location = new System.Drawing.Point(166, 48);
            this.tbv6.Name = "tbv6";
            this.tbv6.Size = new System.Drawing.Size(24, 23);
            this.tbv6.TabIndex = 130;
            this.tbv6.Text = "2";
            this.tbv6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label24
            // 
            this.label24.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.Location = new System.Drawing.Point(152, 215);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(32, 16);
            this.label24.TabIndex = 102;
            this.label24.Text = "30";
            this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ck20
            // 
            this.ck20.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck20.Location = new System.Drawing.Point(184, 45);
            this.ck20.Name = "ck20";
            this.ck20.Size = new System.Drawing.Size(16, 16);
            this.ck20.TabIndex = 66;
            // 
            // ck21
            // 
            this.ck21.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck21.Location = new System.Drawing.Point(184, 62);
            this.ck21.Name = "ck21";
            this.ck21.Size = new System.Drawing.Size(16, 16);
            this.ck21.TabIndex = 69;
            // 
            // ck22
            // 
            this.ck22.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck22.Location = new System.Drawing.Point(184, 79);
            this.ck22.Name = "ck22";
            this.ck22.Size = new System.Drawing.Size(16, 16);
            this.ck22.TabIndex = 72;
            // 
            // ck23
            // 
            this.ck23.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck23.Location = new System.Drawing.Point(184, 96);
            this.ck23.Name = "ck23";
            this.ck23.Size = new System.Drawing.Size(16, 16);
            this.ck23.TabIndex = 75;
            // 
            // ck24
            // 
            this.ck24.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck24.Location = new System.Drawing.Point(184, 113);
            this.ck24.Name = "ck24";
            this.ck24.Size = new System.Drawing.Size(16, 16);
            this.ck24.TabIndex = 78;
            // 
            // ck25
            // 
            this.ck25.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck25.Location = new System.Drawing.Point(184, 130);
            this.ck25.Name = "ck25";
            this.ck25.Size = new System.Drawing.Size(16, 16);
            this.ck25.TabIndex = 81;
            // 
            // ck26
            // 
            this.ck26.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck26.Location = new System.Drawing.Point(184, 147);
            this.ck26.Name = "ck26";
            this.ck26.Size = new System.Drawing.Size(16, 16);
            this.ck26.TabIndex = 84;
            // 
            // ck27
            // 
            this.ck27.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck27.Location = new System.Drawing.Point(184, 164);
            this.ck27.Name = "ck27";
            this.ck27.Size = new System.Drawing.Size(16, 16);
            this.ck27.TabIndex = 87;
            // 
            // ck28
            // 
            this.ck28.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck28.Location = new System.Drawing.Point(184, 181);
            this.ck28.Name = "ck28";
            this.ck28.Size = new System.Drawing.Size(16, 16);
            this.ck28.TabIndex = 90;
            // 
            // ck29
            // 
            this.ck29.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck29.Location = new System.Drawing.Point(184, 198);
            this.ck29.Name = "ck29";
            this.ck29.Size = new System.Drawing.Size(16, 16);
            this.ck29.TabIndex = 101;
            // 
            // label8
            // 
            this.label8.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(41, 24);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(24, 23);
            this.label8.TabIndex = 138;
            this.label8.Text = "1x";
            this.label8.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(8, 62);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(32, 16);
            this.label9.TabIndex = 13;
            this.label9.Text = "3";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label15
            // 
            this.label15.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.Location = new System.Drawing.Point(8, 113);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(32, 16);
            this.label15.TabIndex = 22;
            this.label15.Text = "6";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label14
            // 
            this.label14.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(152, 300);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(32, 16);
            this.label14.TabIndex = 117;
            this.label14.Text = "35";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label17
            // 
            this.label17.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(8, 130);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(32, 16);
            this.label17.TabIndex = 25;
            this.label17.Text = "7";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(8, 28);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 16);
            this.label5.TabIndex = 7;
            this.label5.Text = "1";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label11
            // 
            this.label11.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(8, 79);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(32, 16);
            this.label11.TabIndex = 16;
            this.label11.Text = "4";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(8, 45);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 16);
            this.label7.TabIndex = 10;
            this.label7.Text = "2";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label13
            // 
            this.label13.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(8, 96);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(32, 16);
            this.label13.TabIndex = 19;
            this.label13.Text = "5";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "0";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lb33
            // 
            this.lb33.BackColor = System.Drawing.SystemColors.Info;
            this.lb33.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb33.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb33.Location = new System.Drawing.Point(208, 266);
            this.lb33.Name = "lb33";
            this.lb33.Size = new System.Drawing.Size(80, 16);
            this.lb33.TabIndex = 112;
            this.lb33.Text = "0";
            this.lb33.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lb04
            // 
            this.lb04.BackColor = System.Drawing.SystemColors.Info;
            this.lb04.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb04.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb04.Location = new System.Drawing.Point(64, 79);
            this.lb04.Name = "lb04";
            this.lb04.Size = new System.Drawing.Size(80, 16);
            this.lb04.TabIndex = 17;
            this.lb04.Text = "0";
            this.lb04.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lb02
            // 
            this.lb02.BackColor = System.Drawing.SystemColors.Info;
            this.lb02.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb02.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb02.Location = new System.Drawing.Point(64, 45);
            this.lb02.Name = "lb02";
            this.lb02.Size = new System.Drawing.Size(80, 16);
            this.lb02.TabIndex = 11;
            this.lb02.Text = "0";
            this.lb02.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label40
            // 
            this.label40.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label40.Location = new System.Drawing.Point(166, 24);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(24, 23);
            this.label40.TabIndex = 145;
            this.label40.Text = "21";
            this.label40.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // lb01
            // 
            this.lb01.BackColor = System.Drawing.SystemColors.Info;
            this.lb01.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb01.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb01.Location = new System.Drawing.Point(64, 28);
            this.lb01.Name = "lb01";
            this.lb01.Size = new System.Drawing.Size(80, 16);
            this.lb01.TabIndex = 8;
            this.lb01.Text = "0";
            this.lb01.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ck09
            // 
            this.ck09.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck09.Location = new System.Drawing.Point(40, 164);
            this.ck09.Name = "ck09";
            this.ck09.Size = new System.Drawing.Size(16, 16);
            this.ck09.TabIndex = 33;
            // 
            // lpr10
            // 
            this.lpr10.BackColor = System.Drawing.SystemColors.Info;
            this.lpr10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lpr10.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lpr10.Location = new System.Drawing.Point(256, 48);
            this.lpr10.Name = "lpr10";
            this.lpr10.Size = new System.Drawing.Size(64, 24);
            this.lpr10.TabIndex = 12;
            this.lpr10.Text = "-";
            this.lpr10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label46
            // 
            this.label46.BackColor = System.Drawing.Color.Bisque;
            this.label46.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label46.Location = new System.Drawing.Point(144, 32);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(32, 16);
            this.label46.TabIndex = 20;
            this.label46.Text = "12";
            this.label46.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // lpr12
            // 
            this.lpr12.BackColor = System.Drawing.SystemColors.Info;
            this.lpr12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lpr12.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lpr12.Location = new System.Drawing.Point(136, 48);
            this.lpr12.Name = "lpr12";
            this.lpr12.Size = new System.Drawing.Size(48, 24);
            this.lpr12.TabIndex = 14;
            this.lpr12.Text = "-";
            this.lpr12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bCancelar
            // 
            this.bCancelar.BackColor = System.Drawing.Color.DarkSalmon;
            this.bCancelar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bCancelar.Image = ((System.Drawing.Image)(resources.GetObject("bCancelar.Image")));
            this.bCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bCancelar.Location = new System.Drawing.Point(312, 226);
            this.bCancelar.Name = "bCancelar";
            this.bCancelar.Size = new System.Drawing.Size(104, 32);
            this.bCancelar.TabIndex = 92;
            this.bCancelar.Text = "Cancelar";
            this.bCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bCancelar.UseVisualStyleBackColor = false;
            this.bCancelar.Click += new System.EventHandler(this.BCancelarClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label50);
            this.groupBox1.Controls.Add(this.label48);
            this.groupBox1.Controls.Add(this.label46);
            this.groupBox1.Controls.Add(this.label44);
            this.groupBox1.Controls.Add(this.label43);
            this.groupBox1.Controls.Add(this.label38);
            this.groupBox1.Controls.Add(this.lpr14);
            this.groupBox1.Controls.Add(this.lpr13);
            this.groupBox1.Controls.Add(this.lpr12);
            this.groupBox1.Controls.Add(this.lpr11);
            this.groupBox1.Controls.Add(this.lpr10);
            this.groupBox1.Controls.Add(this.lbCG);
            this.groupBox1.Location = new System.Drawing.Point(16, 328);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(336, 88);
            this.groupBox1.TabIndex = 366;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Resultados del Análisis";
            // 
            // label38
            // 
            this.label38.BackColor = System.Drawing.Color.Bisque;
            this.label38.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label38.Location = new System.Drawing.Point(8, 32);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(32, 16);
            this.label38.TabIndex = 17;
            this.label38.Text = "Pos.";
            this.label38.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // lb26
            // 
            this.lb26.BackColor = System.Drawing.SystemColors.Info;
            this.lb26.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb26.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb26.Location = new System.Drawing.Point(208, 147);
            this.lb26.Name = "lb26";
            this.lb26.Size = new System.Drawing.Size(80, 16);
            this.lb26.TabIndex = 83;
            this.lb26.Text = "0";
            this.lb26.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lb27
            // 
            this.lb27.BackColor = System.Drawing.SystemColors.Info;
            this.lb27.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb27.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb27.Location = new System.Drawing.Point(208, 164);
            this.lb27.Name = "lb27";
            this.lb27.Size = new System.Drawing.Size(80, 16);
            this.lb27.TabIndex = 86;
            this.lb27.Text = "0";
            this.lb27.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lb24
            // 
            this.lb24.BackColor = System.Drawing.SystemColors.Info;
            this.lb24.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb24.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb24.Location = new System.Drawing.Point(208, 113);
            this.lb24.Name = "lb24";
            this.lb24.Size = new System.Drawing.Size(80, 16);
            this.lb24.TabIndex = 77;
            this.lb24.Text = "0";
            this.lb24.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lb25
            // 
            this.lb25.BackColor = System.Drawing.SystemColors.Info;
            this.lb25.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb25.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb25.Location = new System.Drawing.Point(208, 130);
            this.lb25.Name = "lb25";
            this.lb25.Size = new System.Drawing.Size(80, 16);
            this.lb25.TabIndex = 80;
            this.lb25.Text = "0";
            this.lb25.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lb13
            // 
            this.lb13.BackColor = System.Drawing.SystemColors.Info;
            this.lb13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb13.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb13.Location = new System.Drawing.Point(64, 232);
            this.lb13.Name = "lb13";
            this.lb13.Size = new System.Drawing.Size(80, 16);
            this.lb13.TabIndex = 44;
            this.lb13.Text = "0";
            this.lb13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lb23
            // 
            this.lb23.BackColor = System.Drawing.SystemColors.Info;
            this.lb23.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb23.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb23.Location = new System.Drawing.Point(208, 96);
            this.lb23.Name = "lb23";
            this.lb23.Size = new System.Drawing.Size(80, 16);
            this.lb23.TabIndex = 74;
            this.lb23.Text = "0";
            this.lb23.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label36);
            this.groupBox2.Controls.Add(this.label40);
            this.groupBox2.Controls.Add(this.label42);
            this.groupBox2.Controls.Add(this.label28);
            this.groupBox2.Controls.Add(this.label30);
            this.groupBox2.Controls.Add(this.label10);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.tbv8);
            this.groupBox2.Controls.Add(this.tbv7);
            this.groupBox2.Controls.Add(this.tbv6);
            this.groupBox2.Controls.Add(this.tbv5);
            this.groupBox2.Controls.Add(this.tbv4);
            this.groupBox2.Controls.Add(this.tbv3);
            this.groupBox2.Controls.Add(this.tbv2);
            this.groupBox2.Controls.Add(this.tbv1);
            this.groupBox2.Controls.Add(this.tbv0);
            this.groupBox2.Location = new System.Drawing.Point(304, 16);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(256, 88);
            this.groupBox2.TabIndex = 121;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Valores a usar (max.5)";
            // 
            // label36
            // 
            this.label36.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label36.Location = new System.Drawing.Point(216, 24);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(24, 23);
            this.label36.TabIndex = 147;
            this.label36.Text = "22";
            this.label36.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // label30
            // 
            this.label30.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label30.Location = new System.Drawing.Point(141, 24);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(24, 23);
            this.label30.TabIndex = 142;
            this.label30.Text = "x2";
            this.label30.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // lb21
            // 
            this.lb21.BackColor = System.Drawing.SystemColors.Info;
            this.lb21.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb21.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb21.Location = new System.Drawing.Point(208, 62);
            this.lb21.Name = "lb21";
            this.lb21.Size = new System.Drawing.Size(80, 16);
            this.lb21.TabIndex = 68;
            this.lb21.Text = "0";
            this.lb21.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // groupBox4
            // 
            this.groupBox4.BackColor = System.Drawing.Color.Bisque;
            this.groupBox4.Controls.Add(this.tbCG);
            this.groupBox4.Controls.Add(this.lFGR);
            this.groupBox4.Controls.Add(this.bFG);
            this.groupBox4.Controls.Add(this.lbCGR);
            this.groupBox4.Controls.Add(this.bMenosR);
            this.groupBox4.Controls.Add(this.bMasR);
            this.groupBox4.Controls.Add(this.button2);
            this.groupBox4.Controls.Add(this.button3);
            this.groupBox4.Controls.Add(this.bAnalizar);
            this.groupBox4.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(368, 280);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(192, 136);
            this.groupBox4.TabIndex = 365;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "analisis resultados";
            // 
            // bMenosR
            // 
            this.bMenosR.BackColor = System.Drawing.Color.DarkSalmon;
            this.bMenosR.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bMenosR.Location = new System.Drawing.Point(160, 96);
            this.bMenosR.Name = "bMenosR";
            this.bMenosR.Size = new System.Drawing.Size(16, 16);
            this.bMenosR.TabIndex = 85;
            this.bMenosR.Text = "-";
            this.bMenosR.UseVisualStyleBackColor = false;
            this.bMenosR.Click += new System.EventHandler(this.BMenosRClick);
            // 
            // bAnalizar
            // 
            this.bAnalizar.BackColor = System.Drawing.Color.DarkSalmon;
            this.bAnalizar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bAnalizar.Image = ((System.Drawing.Image)(resources.GetObject("bAnalizar.Image")));
            this.bAnalizar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bAnalizar.Location = new System.Drawing.Point(16, 80);
            this.bAnalizar.Name = "bAnalizar";
            this.bAnalizar.Size = new System.Drawing.Size(104, 32);
            this.bAnalizar.TabIndex = 9;
            this.bAnalizar.Text = "Analizar";
            this.bAnalizar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bAnalizar.UseVisualStyleBackColor = false;
            this.bAnalizar.Click += new System.EventHandler(this.BAnalizarClick);
            // 
            // lSumSel
            // 
            this.lSumSel.BackColor = System.Drawing.SystemColors.Info;
            this.lSumSel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lSumSel.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lSumSel.Location = new System.Drawing.Point(448, 152);
            this.lSumSel.Name = "lSumSel";
            this.lSumSel.Size = new System.Drawing.Size(104, 24);
            this.lSumSel.TabIndex = 97;
            this.lSumSel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb19
            // 
            this.lb19.BackColor = System.Drawing.SystemColors.Info;
            this.lb19.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb19.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb19.Location = new System.Drawing.Point(208, 28);
            this.lb19.Name = "lb19";
            this.lb19.Size = new System.Drawing.Size(80, 16);
            this.lb19.TabIndex = 62;
            this.lb19.Text = "0";
            this.lb19.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lb28
            // 
            this.lb28.BackColor = System.Drawing.SystemColors.Info;
            this.lb28.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb28.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb28.Location = new System.Drawing.Point(208, 181);
            this.lb28.Name = "lb28";
            this.lb28.Size = new System.Drawing.Size(80, 16);
            this.lb28.TabIndex = 89;
            this.lb28.Text = "0";
            this.lb28.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lb29
            // 
            this.lb29.BackColor = System.Drawing.SystemColors.Info;
            this.lb29.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb29.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb29.Location = new System.Drawing.Point(208, 198);
            this.lb29.Name = "lb29";
            this.lb29.Size = new System.Drawing.Size(80, 16);
            this.lb29.TabIndex = 100;
            this.lb29.Text = "0";
            this.lb29.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lFileOut
            // 
            this.lFileOut.BackColor = System.Drawing.SystemColors.Info;
            this.lFileOut.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lFileOut.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lFileOut.Location = new System.Drawing.Point(448, 232);
            this.lFileOut.Name = "lFileOut";
            this.lFileOut.Size = new System.Drawing.Size(104, 24);
            this.lFileOut.TabIndex = 98;
            this.lFileOut.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lb34
            // 
            this.lb34.BackColor = System.Drawing.SystemColors.Info;
            this.lb34.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb34.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb34.Location = new System.Drawing.Point(208, 283);
            this.lb34.Name = "lb34";
            this.lb34.Size = new System.Drawing.Size(80, 16);
            this.lb34.TabIndex = 115;
            this.lb34.Text = "0";
            this.lb34.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lb35
            // 
            this.lb35.BackColor = System.Drawing.SystemColors.Info;
            this.lb35.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb35.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb35.Location = new System.Drawing.Point(208, 300);
            this.lb35.Name = "lb35";
            this.lb35.Size = new System.Drawing.Size(80, 16);
            this.lb35.TabIndex = 118;
            this.lb35.Text = "0";
            this.lb35.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lb32
            // 
            this.lb32.BackColor = System.Drawing.SystemColors.Info;
            this.lb32.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb32.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb32.Location = new System.Drawing.Point(208, 249);
            this.lb32.Name = "lb32";
            this.lb32.Size = new System.Drawing.Size(80, 16);
            this.lb32.TabIndex = 109;
            this.lb32.Text = "0";
            this.lb32.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lb31
            // 
            this.lb31.BackColor = System.Drawing.SystemColors.Info;
            this.lb31.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb31.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb31.Location = new System.Drawing.Point(208, 232);
            this.lb31.Name = "lb31";
            this.lb31.Size = new System.Drawing.Size(80, 16);
            this.lb31.TabIndex = 106;
            this.lb31.Text = "0";
            this.lb31.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lb30
            // 
            this.lb30.BackColor = System.Drawing.SystemColors.Info;
            this.lb30.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb30.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb30.Location = new System.Drawing.Point(208, 215);
            this.lb30.Name = "lb30";
            this.lb30.Size = new System.Drawing.Size(80, 16);
            this.lb30.TabIndex = 103;
            this.lb30.Text = "0";
            this.lb30.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ck03
            // 
            this.ck03.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck03.Location = new System.Drawing.Point(40, 62);
            this.ck03.Name = "ck03";
            this.ck03.Size = new System.Drawing.Size(16, 16);
            this.ck03.TabIndex = 15;
            // 
            // label19
            // 
            this.label19.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(8, 147);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(32, 16);
            this.label19.TabIndex = 28;
            this.label19.Text = "8";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label18
            // 
            this.label18.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(152, 266);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(32, 16);
            this.label18.TabIndex = 111;
            this.label18.Text = "33";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ck06
            // 
            this.ck06.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck06.Location = new System.Drawing.Point(40, 113);
            this.ck06.Name = "ck06";
            this.ck06.Size = new System.Drawing.Size(16, 16);
            this.ck06.TabIndex = 24;
            // 
            // ck07
            // 
            this.ck07.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck07.Location = new System.Drawing.Point(40, 130);
            this.ck07.Name = "ck07";
            this.ck07.Size = new System.Drawing.Size(16, 16);
            this.ck07.TabIndex = 27;
            // 
            // ck04
            // 
            this.ck04.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck04.Location = new System.Drawing.Point(40, 79);
            this.ck04.Name = "ck04";
            this.ck04.Size = new System.Drawing.Size(16, 16);
            this.ck04.TabIndex = 18;
            // 
            // ck05
            // 
            this.ck05.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck05.Location = new System.Drawing.Point(40, 96);
            this.ck05.Name = "ck05";
            this.ck05.Size = new System.Drawing.Size(16, 16);
            this.ck05.TabIndex = 21;
            // 
            // lb05
            // 
            this.lb05.BackColor = System.Drawing.SystemColors.Info;
            this.lb05.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb05.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb05.Location = new System.Drawing.Point(64, 96);
            this.lb05.Name = "lb05";
            this.lb05.Size = new System.Drawing.Size(80, 16);
            this.lb05.TabIndex = 20;
            this.lb05.Text = "0";
            this.lb05.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label16
            // 
            this.label16.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.Location = new System.Drawing.Point(152, 283);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(32, 16);
            this.label16.TabIndex = 114;
            this.label16.Text = "34";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label39
            // 
            this.label39.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label39.Location = new System.Drawing.Point(8, 198);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(32, 16);
            this.label39.TabIndex = 37;
            this.label39.Text = "11";
            this.label39.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label37
            // 
            this.label37.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label37.Location = new System.Drawing.Point(8, 215);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(32, 16);
            this.label37.TabIndex = 40;
            this.label37.Text = "12";
            this.label37.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label35
            // 
            this.label35.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label35.Location = new System.Drawing.Point(8, 232);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(32, 16);
            this.label35.TabIndex = 43;
            this.label35.Text = "13";
            this.label35.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label33
            // 
            this.label33.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label33.Location = new System.Drawing.Point(8, 249);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(32, 16);
            this.label33.TabIndex = 46;
            this.label33.Text = "14";
            this.label33.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label31
            // 
            this.label31.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label31.Location = new System.Drawing.Point(8, 266);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(32, 16);
            this.label31.TabIndex = 49;
            this.label31.Text = "15";
            this.label31.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lb09
            // 
            this.lb09.BackColor = System.Drawing.SystemColors.Info;
            this.lb09.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lb09.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lb09.Location = new System.Drawing.Point(64, 164);
            this.lb09.Name = "lb09";
            this.lb09.Size = new System.Drawing.Size(80, 16);
            this.lb09.TabIndex = 32;
            this.lb09.Text = "0";
            this.lb09.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ck02
            // 
            this.ck02.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck02.Location = new System.Drawing.Point(40, 45);
            this.ck02.Name = "ck02";
            this.ck02.Size = new System.Drawing.Size(16, 16);
            this.ck02.TabIndex = 12;
            // 
            // lCol
            // 
            this.lCol.BackColor = System.Drawing.SystemColors.Info;
            this.lCol.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lCol.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lCol.Location = new System.Drawing.Point(312, 177);
            this.lCol.Name = "lCol";
            this.lCol.Size = new System.Drawing.Size(104, 24);
            this.lCol.TabIndex = 91;
            this.lCol.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ck00
            // 
            this.ck00.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck00.Location = new System.Drawing.Point(40, 11);
            this.ck00.Name = "ck00";
            this.ck00.Size = new System.Drawing.Size(16, 16);
            this.ck00.TabIndex = 2;
            // 
            // ck01
            // 
            this.ck01.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ck01.Location = new System.Drawing.Point(40, 28);
            this.ck01.Name = "ck01";
            this.ck01.Size = new System.Drawing.Size(16, 16);
            this.ck01.TabIndex = 9;
            // 
            // AnalizadorJPM
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(7, 16);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(576, 430);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.lb35);
            this.Controls.Add(this.lb34);
            this.Controls.Add(this.lb33);
            this.Controls.Add(this.lb32);
            this.Controls.Add(this.lb31);
            this.Controls.Add(this.lb30);
            this.Controls.Add(this.lb29);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.label26);
            this.Controls.Add(this.ck35);
            this.Controls.Add(this.ck34);
            this.Controls.Add(this.ck33);
            this.Controls.Add(this.ck32);
            this.Controls.Add(this.ck31);
            this.Controls.Add(this.ck30);
            this.Controls.Add(this.ck29);
            this.Controls.Add(this.lFileOut);
            this.Controls.Add(this.lSumSel);
            this.Controls.Add(this.lFileIn);
            this.Controls.Add(this.bSumar);
            this.Controls.Add(this.bCancelar);
            this.Controls.Add(this.lCol);
            this.Controls.Add(this.lb28);
            this.Controls.Add(this.lb27);
            this.Controls.Add(this.lb26);
            this.Controls.Add(this.lb25);
            this.Controls.Add(this.lb24);
            this.Controls.Add(this.lb23);
            this.Controls.Add(this.lb22);
            this.Controls.Add(this.lb21);
            this.Controls.Add(this.lb20);
            this.Controls.Add(this.lb19);
            this.Controls.Add(this.lb18);
            this.Controls.Add(this.lb17);
            this.Controls.Add(this.lb16);
            this.Controls.Add(this.lb15);
            this.Controls.Add(this.lb14);
            this.Controls.Add(this.lb13);
            this.Controls.Add(this.lb12);
            this.Controls.Add(this.lb11);
            this.Controls.Add(this.lb10);
            this.Controls.Add(this.lb09);
            this.Controls.Add(this.lb08);
            this.Controls.Add(this.lb07);
            this.Controls.Add(this.lb06);
            this.Controls.Add(this.lb05);
            this.Controls.Add(this.lb04);
            this.Controls.Add(this.lb03);
            this.Controls.Add(this.lb02);
            this.Controls.Add(this.lb01);
            this.Controls.Add(this.lTime);
            this.Controls.Add(this.bGrabarCols);
            this.Controls.Add(this.bIniciar);
            this.Controls.Add(this.lb00);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label45);
            this.Controls.Add(this.label47);
            this.Controls.Add(this.label49);
            this.Controls.Add(this.label51);
            this.Controls.Add(this.label53);
            this.Controls.Add(this.label55);
            this.Controls.Add(this.label57);
            this.Controls.Add(this.label59);
            this.Controls.Add(this.label61);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.label25);
            this.Controls.Add(this.label27);
            this.Controls.Add(this.label29);
            this.Controls.Add(this.label31);
            this.Controls.Add(this.label33);
            this.Controls.Add(this.label35);
            this.Controls.Add(this.label37);
            this.Controls.Add(this.label39);
            this.Controls.Add(this.label41);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.ck28);
            this.Controls.Add(this.ck27);
            this.Controls.Add(this.ck26);
            this.Controls.Add(this.ck25);
            this.Controls.Add(this.ck24);
            this.Controls.Add(this.ck23);
            this.Controls.Add(this.ck22);
            this.Controls.Add(this.ck21);
            this.Controls.Add(this.ck20);
            this.Controls.Add(this.ck19);
            this.Controls.Add(this.ck18);
            this.Controls.Add(this.ck17);
            this.Controls.Add(this.ck16);
            this.Controls.Add(this.ck15);
            this.Controls.Add(this.ck14);
            this.Controls.Add(this.ck13);
            this.Controls.Add(this.ck12);
            this.Controls.Add(this.ck11);
            this.Controls.Add(this.ck10);
            this.Controls.Add(this.ck09);
            this.Controls.Add(this.ck08);
            this.Controls.Add(this.ck07);
            this.Controls.Add(this.ck06);
            this.Controls.Add(this.ck05);
            this.Controls.Add(this.ck04);
            this.Controls.Add(this.ck03);
            this.Controls.Add(this.ck01);
            this.Controls.Add(this.ck00);
            this.Controls.Add(this.ck02);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "AnalizadorJPM";
            this.Text = "Sumas Pares Naturales (JPM)";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion
		
		void BIniciarClick(object sender, System.EventArgs e) { Iniciar(); }
		void BGrabarClick(object sender, System.EventArgs e) { Grabar(); }
		void BCancelarClick(object sender, System.EventArgs e) { salida=true; }
		void BSumarClick(object sender, System.EventArgs e) { SumSel(); }
		void CalculoColumnas(object sender, System.EventArgs e) {
			lCol.Text = ""+(conta);
			time9 = DateTime.Now;
			tmp = (time9-time0).ToString()+"00000000000";
			lTime.Text = tmp.Substring(0,11);
		}
		void BAnalizarClick(object sender, System.EventArgs e) { Analizar(); }
		void BFGClick(object sender, System.EventArgs e) { EntraCGsR(); }
		void BMasRClick(object sender, System.EventArgs e) { GRMas(); }
		void BMenosRClick(object sender, System.EventArgs e) { GRMenos(); }
		
	}
}
