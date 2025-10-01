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

namespace Free1X2.UI {
	class GeneraPim : System.Windows.Forms.Form 	{
		private System.Windows.Forms.Label l031;
		private System.Windows.Forms.Label l036;
		private System.Windows.Forms.Label l037;
		private System.Windows.Forms.Label l034;
		private System.Windows.Forms.Label l035;
		private System.Windows.Forms.TextBox tbmgreco;
		private System.Windows.Forms.Label l067;
		private System.Windows.Forms.Label l111;
		private System.Windows.Forms.Button bFG;
		private System.Windows.Forms.Label l113;
		private System.Windows.Forms.Label l112;
		private System.Windows.Forms.Button bMenosR;
		private System.Windows.Forms.Label l116;
		private Free1X2.UI.Controls.valors valors1;
		private System.Windows.Forms.Label lrk2;
		private System.Windows.Forms.Label lrk3;
		private System.Windows.Forms.Label lrk4;
		private System.Windows.Forms.Label lrk5;
		private System.Windows.Forms.Label lrk6;
		private System.Windows.Forms.Label lrk7;
		private System.Windows.Forms.TextBox tbrank3;
		private System.Windows.Forms.Label l045;
		private System.Windows.Forms.Label l044;
		private System.Windows.Forms.Label l047;
		private System.Windows.Forms.Label l046;
		private System.Windows.Forms.Label l122;
		private System.Windows.Forms.Label l123;
		private System.Windows.Forms.Label l043;
		private System.Windows.Forms.Label l042;
		private System.Windows.Forms.Label l126;
		private System.Windows.Forms.Button bExporCols;
		private System.Windows.Forms.Label l124;
		private System.Windows.Forms.Label l125;
		private System.Windows.Forms.Label l144;
		private System.Windows.Forms.Label l145;
		private System.Windows.Forms.Label l146;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label l071;
		private System.Windows.Forms.Label l092;
		private System.Windows.Forms.Label l093;
		private System.Windows.Forms.Button bLeerConds;
		private System.Windows.Forms.Label l054;
		private System.Windows.Forms.Label l055;
		private System.Windows.Forms.Label l056;
		private System.Windows.Forms.Label l057;
		private System.Windows.Forms.Button bSalvarConds;
		private System.Windows.Forms.Label l051;
		private System.Windows.Forms.Label l052;
		private System.Windows.Forms.Label l053;
		private System.Windows.Forms.Label l115;
		private System.Windows.Forms.Label l114;
		private System.Windows.Forms.Label l117;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbrank2;
		private System.Windows.Forms.Label l133;
		private System.Windows.Forms.Label l132;
		private System.Windows.Forms.Button bMasR;
		private System.Windows.Forms.Button bAnalizar;
		private System.Windows.Forms.Button bGrabaCols;
		private System.Windows.Forms.Label l014;
		private System.Windows.Forms.Label lrangos;
		private System.Windows.Forms.Label l017;
		private System.Windows.Forms.Label l103;
		private System.Windows.Forms.Label l066;
		private System.Windows.Forms.Label l065;
		private System.Windows.Forms.Label l064;
		private System.Windows.Forms.Label l063;
		private System.Windows.Forms.Label l062;
		private System.Windows.Forms.Label l061;
		private System.Windows.Forms.Label l147;
		private System.Windows.Forms.Label l141;
		private System.Windows.Forms.Label l142;
		private System.Windows.Forms.Label l143;
		private System.Windows.Forms.TextBox ltColR;
		private System.Windows.Forms.TextBox tbrank5;
		private System.Windows.Forms.TextBox tbrank4;
		private System.Windows.Forms.TextBox tbrank7;
		private System.Windows.Forms.TextBox tbrank6;
		private System.Windows.Forms.Label l081;
		private System.Windows.Forms.Label l083;
		private System.Windows.Forms.Label l082;
		private System.Windows.Forms.Label l085;
        private System.Windows.Forms.Label l084;
		private System.Windows.Forms.Label l086;
		private System.Windows.Forms.Label l076;
		private System.Windows.Forms.Label l077;
		private System.Windows.Forms.Label l074;
		private System.Windows.Forms.Label l075;
		private System.Windows.Forms.Label l072;
		private System.Windows.Forms.Button bCancelar;
		private System.Windows.Forms.Label l131;
		private System.Windows.Forms.Label lColsIni;
		private System.Windows.Forms.Label l137;
		private System.Windows.Forms.Label l136;
		private System.Windows.Forms.Label l135;
		private System.Windows.Forms.Label l134;
		private System.Windows.Forms.Label lrk1;
		private System.Windows.Forms.Label lFGR;
		private System.Windows.Forms.Label l091;
		private System.Windows.Forms.Label lColsAdm;
		private System.Windows.Forms.Label l101;
		private System.Windows.Forms.Label l094;
		private System.Windows.Forms.Label l095;
		private System.Windows.Forms.Label l096;
		private System.Windows.Forms.Label l097;
		private System.Windows.Forms.Label l087;
		private System.Windows.Forms.Label l011;
		private System.Windows.Forms.Label l012;
		private System.Windows.Forms.Label l013;
		private System.Windows.Forms.Label lTime;
		private System.Windows.Forms.Label l015;
		private System.Windows.Forms.Label l016;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button bCalcular;
		private System.Windows.Forms.Label l025;
		private System.Windows.Forms.Label l041;
		private System.Windows.Forms.Label lfile;
		private System.Windows.Forms.Label l073;
		private System.Windows.Forms.Label l023;
		private System.Windows.Forms.Label l022;
		private System.Windows.Forms.Label l021;
		private System.Windows.Forms.Label l027;
		private System.Windows.Forms.Label l026;
		private System.Windows.Forms.Label l102;
		private System.Windows.Forms.Label l024;
		private System.Windows.Forms.Label l104;
		private System.Windows.Forms.Label l105;
		private System.Windows.Forms.Label l106;
		private System.Windows.Forms.Label l107;
		private System.Windows.Forms.Label l121;
		private System.Windows.Forms.TextBox tbrank1;
		private System.Windows.Forms.Label l127;
		private System.Windows.Forms.Label lbCGR;
		private System.Windows.Forms.Label lreco;
		private System.Windows.Forms.Label l032;
		private System.Windows.Forms.Label l033;
		public GeneraPim() { 
			InitializeComponent();
			elmeu = new Timer();
			elmeu.Interval = 3000;
   		    elmeu.Tick += new EventHandler(elmeuTimer);
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}
		
		private double[,] nvals = new double[14,3];
		private int[,] rks = new int[8,2];
		private string[] scps = new string[7];
		private int[,] cps = new int[14,7];
		private DateTime time0, time9;
		private Timer elmeu;
		private bool salida = false;
		private string tmp;
		private int ctini, ctadm;
		private string[] validas = new string[4782969];
		private BitArray repes = new BitArray(4782969);
		private int limcgsR, nrfCGR;
		private string[] colgsR = new string[3000];
		
		private string[] Rellenar42(int nlin, string[]buff) {
			string[] aux=new string[42], clin=new string[50];
			int ir=0; char tab = (char) 9;
			if (nlin==1) {
				clin = buff[0].Split(',');
				if (clin.Length!=42) clin = buff[0].Split(' ');
				if (clin.Length!=42) clin = buff[0].Split(tab);
				if (clin.Length==42) {
					for (int nr=0; nr<42; nr++) {
						aux[nr] = clin[nr].Replace('.',',');
					}
				}
			}
			else if (nlin==14) {
				for (int nr=0; nr<14; nr++) {
					clin = buff[nr].Split(',');
					if (clin.Length!=3) clin = buff[nr].Split(' ');
					if (clin.Length!=3) clin = buff[nr].Split(tab);
					if (clin.Length!=3) { aux.Initialize(); break; }
					aux[ir]=clin[0].Replace('.',','); ir++;
					aux[ir]=clin[1].Replace('.',','); ir++;
					aux[ir]=clin[2].Replace('.',','); ir++;
				}
			}
			else if (nlin==42) {
				for (int nr=0; nr<42; nr++) aux[nr]=buff[nr].Replace('.',',');
			}
			return aux;
		}
		private void RecuperaPantalla() {
			double[,] nvalc = new double[14,3];
			RecuperaRangos();
			nvals = valors1.RetVals();;
			double max, val; int indf, indc;
			for (int nr=0; nr<14; nr++) {
				for (int nc=0; nc<3; nc++) nvalc[nr,nc] = nvals[nr,nc];
				for (int nc=0; nc<7; nc++) cps[nr,nc]=0;
			}
			for (int nr1=0; nr1<7; nr1++) {
				for (int nr2=0; nr2<6; nr2++) {
					max=(-1); indf=indc=0;
					for (int nc1=0; nc1<14; nc1++) for (int nc2=0; nc2<3; nc2++) {
						val = nvalc[nc1,nc2];
						if (val>max) {
							indf=nc1; indc=nc2; max=val;
						}
					}
					nvalc[indf,indc]=(-1);
					cps[indf,nr1]+=(indc==0?1:indc==1?4:2);
				}
			}
			for (int nc=0; nc<7; nc++) {
				tmp = "";
				for (int np=0; np<14; np++) tmp+=cps[np,nc];
				scps[nc] = tmp;	
			}
		}
		private void RecuperaRangos() {
			string[] aux = tbrank1.Text.Split('-');
			rks[0,0]=Convert.ToInt32(aux[0]); rks[0,1]=Convert.ToInt32(aux[1]);
			aux = tbrank2.Text.Split('-');
			rks[1,0]=Convert.ToInt32(aux[0]); rks[1,1]=Convert.ToInt32(aux[1]);
			aux = tbrank3.Text.Split('-');
			rks[2,0]=Convert.ToInt32(aux[0]); rks[2,1]=Convert.ToInt32(aux[1]);
			aux = tbrank4.Text.Split('-');
			rks[3,0]=Convert.ToInt32(aux[0]); rks[3,1]=Convert.ToInt32(aux[1]);
			aux = tbrank5.Text.Split('-');
			rks[4,0]=Convert.ToInt32(aux[0]); rks[4,1]=Convert.ToInt32(aux[1]);
			aux = tbrank6.Text.Split('-');
			rks[5,0]=Convert.ToInt32(aux[0]); rks[5,1]=Convert.ToInt32(aux[1]);
			aux = tbrank7.Text.Split('-');
			rks[6,0]=Convert.ToInt32(aux[0]); rks[6,1]=Convert.ToInt32(aux[1]);
			aux = tbmgreco.Text.Split('-');
			rks[7,0]=Convert.ToInt32(aux[0]); rks[7,1]=Convert.ToInt32(aux[1]);
		}
		private int[] valora(int percent, int ncol) {
			int[] rsl = new int[2]; int ind=0, val;
			val = ((ncol==0)?1:((ncol==2)?2:4));
			rsl[0] = ind; rsl[1] = val;
			return rsl;
		}
		private void PintaPantalla() {
			int nv;
			nv = cps[0,0]; l011.Text=Cambia(nv); 
			nv = cps[1,0]; l021.Text=Cambia(nv);
			nv = cps[2,0]; l031.Text=Cambia(nv);
			nv = cps[3,0]; l041.Text=Cambia(nv);
			nv = cps[4,0]; l051.Text=Cambia(nv);
			nv = cps[5,0]; l061.Text=Cambia(nv);
			nv = cps[6,0]; l071.Text=Cambia(nv); 
			nv = cps[7,0]; l081.Text=Cambia(nv);
			nv = cps[8,0]; l091.Text=Cambia(nv);
			nv = cps[9,0]; l101.Text=Cambia(nv);
			nv = cps[10,0]; l111.Text=Cambia(nv);
			nv = cps[11,0]; l121.Text=Cambia(nv);
			nv = cps[12,0]; l131.Text=Cambia(nv);
			nv = cps[13,0]; l141.Text=Cambia(nv); 
			nv = cps[0,1]; l012.Text=Cambia(nv); 
			nv = cps[1,1]; l022.Text=Cambia(nv); 
			nv = cps[2,1]; l032.Text=Cambia(nv);
			nv = cps[3,1]; l042.Text=Cambia(nv); 
			nv = cps[4,1]; l052.Text=Cambia(nv);
			nv = cps[5,1]; l062.Text=Cambia(nv);
			nv = cps[6,1]; l072.Text=Cambia(nv);
			nv = cps[7,1]; l082.Text=Cambia(nv); 
			nv = cps[8,1]; l092.Text=Cambia(nv);
			nv = cps[9,1]; l102.Text=Cambia(nv);
			nv = cps[10,1]; l112.Text=Cambia(nv);  
			nv = cps[11,1]; l122.Text=Cambia(nv);
			nv = cps[12,1]; l132.Text=Cambia(nv);
			nv = cps[13,1]; l142.Text=Cambia(nv);
			nv = cps[0,2]; l013.Text=Cambia(nv);
			nv = cps[1,2]; l023.Text=Cambia(nv);
			nv = cps[2,2]; l033.Text=Cambia(nv); 
			nv = cps[3,2]; l043.Text=Cambia(nv);
			nv = cps[4,2]; l053.Text=Cambia(nv); 
			nv = cps[5,2]; l063.Text=Cambia(nv);
			nv = cps[6,2]; l073.Text=Cambia(nv);
			nv = cps[7,2]; l083.Text=Cambia(nv);
			nv = cps[8,2]; l093.Text=Cambia(nv);
			nv = cps[9,2]; l103.Text=Cambia(nv);
			nv = cps[10,2]; l113.Text=Cambia(nv);
			nv = cps[11,2]; l123.Text=Cambia(nv);
			nv = cps[12,2]; l133.Text=Cambia(nv); 
			nv = cps[13,2]; l143.Text=Cambia(nv); 
			nv = cps[0,3]; l014.Text=Cambia(nv);
			nv = cps[1,3]; l024.Text=Cambia(nv); 
			nv = cps[2,3]; l034.Text=Cambia(nv);
			nv = cps[3,3]; l044.Text=Cambia(nv);
			nv = cps[4,3]; l054.Text=Cambia(nv); 
			nv = cps[5,3]; l064.Text=Cambia(nv);    
			nv = cps[6,3]; l074.Text=Cambia(nv);
			nv = cps[7,3]; l084.Text=Cambia(nv);
			nv = cps[8,3]; l094.Text=Cambia(nv);
			nv = cps[9,3]; l104.Text=Cambia(nv);
			nv = cps[10,3]; l114.Text=Cambia(nv); 
			nv = cps[11,3]; l124.Text=Cambia(nv); 
			nv = cps[12,3]; l134.Text=Cambia(nv);
			nv = cps[13,3]; l144.Text=Cambia(nv);
			nv = cps[0,4]; l015.Text=Cambia(nv);
			nv = cps[1,4]; l025.Text=Cambia(nv); 
			nv = cps[2,4]; l035.Text=Cambia(nv); 
			nv = cps[3,4]; l045.Text=Cambia(nv);
			nv = cps[4,4]; l055.Text=Cambia(nv); 
			nv = cps[5,4]; l065.Text=Cambia(nv);
			nv = cps[6,4]; l075.Text=Cambia(nv); 
			nv = cps[7,4]; l085.Text=Cambia(nv); 
			nv = cps[8,4]; l095.Text=Cambia(nv);
			nv = cps[9,4]; l105.Text=Cambia(nv);
			nv = cps[10,4]; l115.Text=Cambia(nv);
			nv = cps[11,4]; l125.Text=Cambia(nv); 
			nv = cps[12,4]; l135.Text=Cambia(nv);
			nv = cps[13,4]; l145.Text=Cambia(nv); 
			nv = cps[0,5]; l016.Text=Cambia(nv); 
			nv = cps[1,5]; l026.Text=Cambia(nv);
			nv = cps[2,5]; l036.Text=Cambia(nv);
			nv = cps[3,5]; l046.Text=Cambia(nv); 
			nv = cps[4,5]; l056.Text=Cambia(nv); 
			nv = cps[5,5]; l066.Text=Cambia(nv); 
			nv = cps[6,5]; l076.Text=Cambia(nv);  
			nv = cps[7,5]; l086.Text=Cambia(nv);
			nv = cps[8,5]; l096.Text=Cambia(nv);
			nv = cps[9,5]; l106.Text=Cambia(nv);
			nv = cps[10,5]; l116.Text=Cambia(nv);
			nv = cps[11,5]; l126.Text=Cambia(nv);
			nv = cps[12,5]; l136.Text=Cambia(nv); 
			nv = cps[13,5]; l146.Text=Cambia(nv);
			nv = cps[0,6]; l017.Text=Cambia(nv);
			nv = cps[1,6]; l027.Text=Cambia(nv);
			nv = cps[2,6]; l037.Text=Cambia(nv); 
			nv = cps[3,6]; l047.Text=Cambia(nv); 
			nv = cps[4,6]; l057.Text=Cambia(nv);
			nv = cps[5,6]; l067.Text=Cambia(nv); 
			nv = cps[6,6]; l077.Text=Cambia(nv);
			nv = cps[7,6]; l087.Text=Cambia(nv);
			nv = cps[8,6]; l097.Text=Cambia(nv); 
			nv = cps[9,6]; l107.Text=Cambia(nv); 
			nv = cps[10,6]; l117.Text=Cambia(nv); 
			nv = cps[11,6]; l127.Text=Cambia(nv); 
			nv = cps[12,6]; l137.Text=Cambia(nv);
			nv = cps[13,6]; l147.Text=Cambia(nv); 
		}
		private string Cambia(int valor) {
			string rsl;
			if (valor==0) rsl="-";
			else if (valor==1) rsl="1";
			else if (valor==2) rsl="2";
			else if (valor==3) rsl="12";
			else if (valor==4) rsl="X";
			else if (valor==5) rsl="1X";
			else if (valor==6) rsl="X2";
			else rsl="1X2";
			return rsl;
		}
		private void GrabaCols() {
			bCalcular.Enabled = false;
			bGrabaCols.Enabled = false;
			bGrabaCols.Text = "grabando";
			SaveFileDialog resul = new SaveFileDialog();
			resul.InitialDirectory = ".\\" ;
			resul.Filter = "Resultados(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
			if(resul.ShowDialog() == DialogResult.OK) {
				Application.DoEvents();
		   		tmp = resul.FileName;
		   		string fileout = Path.GetFileName(tmp);
				StreamWriter wr = new StreamWriter(fileout);
				for (int nr=0; nr<ctadm; nr++) 
					wr.WriteLine(validas[nr].Replace('4','X'));
				wr.Close();
				lfile.Text = fileout;
			}
			bGrabaCols.Text = "grabar resultado";
			bCalcular.Enabled = true;
		}
		private void veureelmeu() {
			lColsIni.Text = ""+ctini;
			lColsAdm.Text = ""+ctadm;
			time9 = DateTime.Now;
			tmp = (time9-time0).ToString()+"0000000000";
			lTime.Text = tmp.Substring(0,10);
		}
		private void Calcular() {
			bCalcular.Enabled = false;
			bGrabaCols.Enabled = false;
			elmeu.Start(); time0 = DateTime.Now;
			salida = false;
			lColsIni.Text = lColsAdm.Text = lTime.Text = " ";
			ctadm=ctini=0;
			Application.DoEvents();
			RecuperaPantalla();
			OpenFileDialog lee = new OpenFileDialog();
			lee.InitialDirectory = ".\\" ;
			lee.Filter = "Cols.Entrada(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
			if(lee.ShowDialog() == DialogResult.OK) {
				int idx;
				repes.SetAll(false);
		   		tmp = lee.FileName;
		   		string filein = Path.GetFileName(tmp);
				StreamReader sr = new StreamReader(filein);
				while (sr.Peek()>0) {
					if (salida) break;
					tmp = sr.ReadLine().Trim().ToUpper(); ctini++;
					if (tmp.Length < 14) { MessageBox.Show ("error longitud = "+tmp); break; }
					idx = s2n(tmp);
					if (repes[idx]==false) {
						repes[idx]=true;
						tmp = tmp.Replace('X','4');
						if (Valida(tmp)) validas[ctadm++]=tmp;
					}
					Application.DoEvents();
				}
			}
			elmeu.Stop(); veureelmeu();
			bCalcular.Enabled = true;
			bGrabaCols.Enabled = true;
		}
		private bool Valida(string columna) {
			int na, min7, max7, reco7; string temp;
			max7=0; min7=98;
			for (int nr=0; nr<7; nr++) {
				na=0;  
				temp=scps[nr]; 
				for (int nr2=0; nr2<14; nr2++) {
					char chp = temp[nr2];
					if (chp==48) continue;
					int ch3 = chp & columna[nr2];
					if (ch3>48) na++; 
				} 
				if (na<rks[nr,0] || na>rks[nr,1]) return false;
				if (na<min7) min7=na; if (na>max7) max7=na;
			}
			reco7 = max7 - min7;
			if (reco7<rks[7,0] || reco7>rks[7,1]) return false;
			return true;
		}
		private void SalvarConds() {
			SaveFileDialog resul = new SaveFileDialog();
			resul.InitialDirectory = ".\\" ;
			resul.Filter = "Condiciones(*.jb7)|*.jb7|Todos los archivos (*.*)|*.*";
			if(resul.ShowDialog() == DialogResult.OK) {
				Application.DoEvents();
		   		tmp = resul.FileName;
		   		string fileout = Path.GetFileName(tmp);
				StreamWriter wr = new StreamWriter(fileout);
				wr.WriteLine(tbrank1.Text);
				wr.WriteLine(tbrank2.Text);
				wr.WriteLine(tbrank3.Text);
				wr.WriteLine(tbrank4.Text);
				wr.WriteLine(tbrank5.Text);
				wr.WriteLine(tbrank6.Text);
				wr.WriteLine(tbrank7.Text);
				wr.WriteLine(tbmgreco.Text);
				wr.Close();
				lrangos.Text = fileout;
			}
		}
		private void LeerConds() {
			OpenFileDialog abreValIn = new OpenFileDialog();
			abreValIn.InitialDirectory = Application.StartupPath + "/";
			abreValIn.Filter = "F.Entrada(*.jb7)|*.jb7|Todos los archivos(*.*)|*.*";
			if(abreValIn.ShowDialog() == DialogResult.OK) {	
			    string filein = Path.GetFileName(abreValIn.FileName);
				StreamReader srv = new StreamReader(filein);
				tbrank1.Text = srv.ReadLine(); 
				tbrank2.Text = srv.ReadLine(); 
				tbrank3.Text = srv.ReadLine(); 
				tbrank4.Text = srv.ReadLine(); 
				tbrank5.Text = srv.ReadLine(); 
				tbrank6.Text = srv.ReadLine(); 
				tbrank7.Text = srv.ReadLine();
				tbmgreco.Text = srv.ReadLine();
				srv.Close();
				lrangos.Text = filein;
			}
			RecuperaPantalla();
			PintaPantalla();
		}
		private void Analizar() {
			bAnalizar.Enabled = false;
			bCalcular.Enabled = false;
			bGrabaCols.Enabled = false;
			int na, min7, max7; int[] minmax = new int[7];
			string columna = ltColR.Text.Replace('x','4');
			columna = columna.Replace('X','4');
			min7 = 99; max7 = 0;
			RecuperaPantalla();
			PintaPantalla();
			for (int nr=0; nr<7; nr++) {
				na=0;
				tmp=scps[nr]; 
				for (int nr2=0; nr2<14; nr2++) {
					char chp = tmp[nr2];
					if (chp==48) continue;
					int ch3 = chp & columna[nr2];
					if (ch3>48) na++;
				}
				minmax[nr]=na;
				if (na<min7) min7=na; if (na>max7) max7=na;
			}
			lrk1.Text = ""+minmax[0];
			lrk2.Text = ""+minmax[1];
			lrk3.Text = ""+minmax[2];
			lrk4.Text = ""+minmax[3];
			lrk5.Text = ""+minmax[4];
			lrk6.Text = ""+minmax[5];
			lrk7.Text = ""+minmax[6];
			lreco.Text = ""+(max7-min7);
			bCalcular.Enabled = true;
			bGrabaCols.Enabled = true;
			bAnalizar.Enabled = true;	
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
				lbCGR.Text=""+nrfCGR; ltColR.Text=colgsR[nrfCGR-1];
				bAnalizar.Enabled = true;
			}
		}
		private void GRMas() {
			if (nrfCGR<limcgsR) {
				nrfCGR++;
				lbCGR.Text=""+nrfCGR; ltColR.Text=colgsR[nrfCGR-1];
			}
		}
		private void GRMenos() {
			if (nrfCGR>1) {
				nrfCGR--;
				lbCGR.Text=""+nrfCGR; ltColR.Text=colgsR[nrfCGR-1];
			}
		}	
		private void ExporCols() {
			string fileout, tmp;
			SaveFileDialog svDialog = new SaveFileDialog();
			svDialog.InitialDirectory = "*\\";
			svDialog.Filter = "F.Salida(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			if(svDialog.ShowDialog() == DialogResult.OK) {
				fileout = Path.GetFileName(svDialog.FileName);
				StreamWriter sw = new StreamWriter(fileout);
				for (int nr=0; nr<7; nr++) {
					tmp = Cambia(cps[0,nr]);
					for (int np=1; np<14; np++) {
						tmp += ","+Cambia(cps[np,nr]);
					}
					sw.WriteLine(tmp);
				}
				sw.Close();
			}
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
				if (ax[nr]=='1') nx+=1;
				else if (ax[nr]=='2') nx+=2;
			}
			return nx;
		}
		
//		[STAThread]
//		public static void Main(string[] args) { Application.Run(new GeneraPim()); }
		
		
		
		void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GeneraPim));
            this.l033 = new System.Windows.Forms.Label();
            this.l032 = new System.Windows.Forms.Label();
            this.lreco = new System.Windows.Forms.Label();
            this.lbCGR = new System.Windows.Forms.Label();
            this.l127 = new System.Windows.Forms.Label();
            this.tbrank1 = new System.Windows.Forms.TextBox();
            this.l121 = new System.Windows.Forms.Label();
            this.l107 = new System.Windows.Forms.Label();
            this.l106 = new System.Windows.Forms.Label();
            this.l105 = new System.Windows.Forms.Label();
            this.l104 = new System.Windows.Forms.Label();
            this.l024 = new System.Windows.Forms.Label();
            this.l102 = new System.Windows.Forms.Label();
            this.l026 = new System.Windows.Forms.Label();
            this.l027 = new System.Windows.Forms.Label();
            this.l021 = new System.Windows.Forms.Label();
            this.l022 = new System.Windows.Forms.Label();
            this.l023 = new System.Windows.Forms.Label();
            this.l073 = new System.Windows.Forms.Label();
            this.lfile = new System.Windows.Forms.Label();
            this.l041 = new System.Windows.Forms.Label();
            this.l025 = new System.Windows.Forms.Label();
            this.bCalcular = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.l016 = new System.Windows.Forms.Label();
            this.l015 = new System.Windows.Forms.Label();
            this.lTime = new System.Windows.Forms.Label();
            this.l013 = new System.Windows.Forms.Label();
            this.l012 = new System.Windows.Forms.Label();
            this.l011 = new System.Windows.Forms.Label();
            this.l087 = new System.Windows.Forms.Label();
            this.l097 = new System.Windows.Forms.Label();
            this.l096 = new System.Windows.Forms.Label();
            this.l095 = new System.Windows.Forms.Label();
            this.l094 = new System.Windows.Forms.Label();
            this.l101 = new System.Windows.Forms.Label();
            this.lColsAdm = new System.Windows.Forms.Label();
            this.l091 = new System.Windows.Forms.Label();
            this.lFGR = new System.Windows.Forms.Label();
            this.lrk1 = new System.Windows.Forms.Label();
            this.l134 = new System.Windows.Forms.Label();
            this.l135 = new System.Windows.Forms.Label();
            this.l136 = new System.Windows.Forms.Label();
            this.l137 = new System.Windows.Forms.Label();
            this.lColsIni = new System.Windows.Forms.Label();
            this.l131 = new System.Windows.Forms.Label();
            this.bCancelar = new System.Windows.Forms.Button();
            this.l072 = new System.Windows.Forms.Label();
            this.l075 = new System.Windows.Forms.Label();
            this.l074 = new System.Windows.Forms.Label();
            this.l077 = new System.Windows.Forms.Label();
            this.l076 = new System.Windows.Forms.Label();
            this.l086 = new System.Windows.Forms.Label();
            this.l084 = new System.Windows.Forms.Label();
            this.l085 = new System.Windows.Forms.Label();
            this.l082 = new System.Windows.Forms.Label();
            this.l083 = new System.Windows.Forms.Label();
            this.l081 = new System.Windows.Forms.Label();
            this.tbrank6 = new System.Windows.Forms.TextBox();
            this.tbrank7 = new System.Windows.Forms.TextBox();
            this.tbrank4 = new System.Windows.Forms.TextBox();
            this.tbrank5 = new System.Windows.Forms.TextBox();
            this.ltColR = new System.Windows.Forms.TextBox();
            this.l143 = new System.Windows.Forms.Label();
            this.l142 = new System.Windows.Forms.Label();
            this.l141 = new System.Windows.Forms.Label();
            this.l147 = new System.Windows.Forms.Label();
            this.l061 = new System.Windows.Forms.Label();
            this.l062 = new System.Windows.Forms.Label();
            this.l063 = new System.Windows.Forms.Label();
            this.l064 = new System.Windows.Forms.Label();
            this.l065 = new System.Windows.Forms.Label();
            this.l066 = new System.Windows.Forms.Label();
            this.l103 = new System.Windows.Forms.Label();
            this.l017 = new System.Windows.Forms.Label();
            this.lrangos = new System.Windows.Forms.Label();
            this.l014 = new System.Windows.Forms.Label();
            this.bGrabaCols = new System.Windows.Forms.Button();
            this.bAnalizar = new System.Windows.Forms.Button();
            this.bMasR = new System.Windows.Forms.Button();
            this.l132 = new System.Windows.Forms.Label();
            this.l133 = new System.Windows.Forms.Label();
            this.tbrank2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.l117 = new System.Windows.Forms.Label();
            this.l114 = new System.Windows.Forms.Label();
            this.l115 = new System.Windows.Forms.Label();
            this.l053 = new System.Windows.Forms.Label();
            this.l052 = new System.Windows.Forms.Label();
            this.l051 = new System.Windows.Forms.Label();
            this.bSalvarConds = new System.Windows.Forms.Button();
            this.l057 = new System.Windows.Forms.Label();
            this.l056 = new System.Windows.Forms.Label();
            this.l055 = new System.Windows.Forms.Label();
            this.l054 = new System.Windows.Forms.Label();
            this.bLeerConds = new System.Windows.Forms.Button();
            this.l093 = new System.Windows.Forms.Label();
            this.l092 = new System.Windows.Forms.Label();
            this.l071 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tbmgreco = new System.Windows.Forms.TextBox();
            this.lrk7 = new System.Windows.Forms.Label();
            this.lrk6 = new System.Windows.Forms.Label();
            this.lrk5 = new System.Windows.Forms.Label();
            this.lrk4 = new System.Windows.Forms.Label();
            this.lrk3 = new System.Windows.Forms.Label();
            this.lrk2 = new System.Windows.Forms.Label();
            this.tbrank3 = new System.Windows.Forms.TextBox();
            this.l067 = new System.Windows.Forms.Label();
            this.l037 = new System.Windows.Forms.Label();
            this.l047 = new System.Windows.Forms.Label();
            this.l146 = new System.Windows.Forms.Label();
            this.l116 = new System.Windows.Forms.Label();
            this.l126 = new System.Windows.Forms.Label();
            this.l036 = new System.Windows.Forms.Label();
            this.l046 = new System.Windows.Forms.Label();
            this.l145 = new System.Windows.Forms.Label();
            this.l125 = new System.Windows.Forms.Label();
            this.l035 = new System.Windows.Forms.Label();
            this.l045 = new System.Windows.Forms.Label();
            this.l144 = new System.Windows.Forms.Label();
            this.l124 = new System.Windows.Forms.Label();
            this.l034 = new System.Windows.Forms.Label();
            this.l044 = new System.Windows.Forms.Label();
            this.l113 = new System.Windows.Forms.Label();
            this.l123 = new System.Windows.Forms.Label();
            this.l043 = new System.Windows.Forms.Label();
            this.l112 = new System.Windows.Forms.Label();
            this.l122 = new System.Windows.Forms.Label();
            this.l042 = new System.Windows.Forms.Label();
            this.l111 = new System.Windows.Forms.Label();
            this.l031 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.bFG = new System.Windows.Forms.Button();
            this.bMenosR = new System.Windows.Forms.Button();
            this.bExporCols = new System.Windows.Forms.Button();
            this.valors1 = new Free1X2.UI.Controls.valors();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // l033
            // 
            this.l033.BackColor = System.Drawing.SystemColors.Window;
            this.l033.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l033.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l033.Location = new System.Drawing.Point(88, 64);
            this.l033.Name = "l033";
            this.l033.Size = new System.Drawing.Size(32, 15);
            this.l033.TabIndex = 275;
            this.l033.Text = "-";
            this.l033.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l032
            // 
            this.l032.BackColor = System.Drawing.SystemColors.Window;
            this.l032.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l032.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l032.Location = new System.Drawing.Point(48, 64);
            this.l032.Name = "l032";
            this.l032.Size = new System.Drawing.Size(32, 15);
            this.l032.TabIndex = 261;
            this.l032.Text = "-";
            this.l032.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lreco
            // 
            this.lreco.BackColor = System.Drawing.SystemColors.Info;
            this.lreco.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lreco.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lreco.Location = new System.Drawing.Point(88, 349);
            this.lreco.Name = "lreco";
            this.lreco.Size = new System.Drawing.Size(32, 18);
            this.lreco.TabIndex = 371;
            this.lreco.Text = "-";
            this.lreco.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbCGR
            // 
            this.lbCGR.BackColor = System.Drawing.SystemColors.Info;
            this.lbCGR.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lbCGR.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCGR.ForeColor = System.Drawing.Color.Black;
            this.lbCGR.Location = new System.Drawing.Point(192, 22);
            this.lbCGR.Name = "lbCGR";
            this.lbCGR.Size = new System.Drawing.Size(32, 31);
            this.lbCGR.TabIndex = 86;
            this.lbCGR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l127
            // 
            this.l127.BackColor = System.Drawing.SystemColors.Info;
            this.l127.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l127.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l127.Location = new System.Drawing.Point(248, 226);
            this.l127.Name = "l127";
            this.l127.Size = new System.Drawing.Size(32, 15);
            this.l127.TabIndex = 338;
            this.l127.Text = "-";
            this.l127.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbrank1
            // 
            this.tbrank1.BackColor = System.Drawing.Color.PaleGreen;
            this.tbrank1.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbrank1.ForeColor = System.Drawing.Color.Maroon;
            this.tbrank1.Location = new System.Drawing.Point(8, 305);
            this.tbrank1.Name = "tbrank1";
            this.tbrank1.Size = new System.Drawing.Size(32, 18);
            this.tbrank1.TabIndex = 343;
            this.tbrank1.Text = "3-6";
            this.tbrank1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // l121
            // 
            this.l121.BackColor = System.Drawing.SystemColors.Info;
            this.l121.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l121.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l121.Location = new System.Drawing.Point(8, 226);
            this.l121.Name = "l121";
            this.l121.Size = new System.Drawing.Size(32, 15);
            this.l121.TabIndex = 254;
            this.l121.Text = "-";
            this.l121.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l107
            // 
            this.l107.BackColor = System.Drawing.SystemColors.Window;
            this.l107.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l107.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l107.Location = new System.Drawing.Point(248, 188);
            this.l107.Name = "l107";
            this.l107.Size = new System.Drawing.Size(32, 15);
            this.l107.TabIndex = 340;
            this.l107.Text = "-";
            this.l107.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l106
            // 
            this.l106.BackColor = System.Drawing.SystemColors.Window;
            this.l106.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l106.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l106.Location = new System.Drawing.Point(208, 188);
            this.l106.Name = "l106";
            this.l106.Size = new System.Drawing.Size(32, 15);
            this.l106.TabIndex = 326;
            this.l106.Text = "-";
            this.l106.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l105
            // 
            this.l105.BackColor = System.Drawing.SystemColors.Window;
            this.l105.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l105.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l105.Location = new System.Drawing.Point(168, 188);
            this.l105.Name = "l105";
            this.l105.Size = new System.Drawing.Size(32, 15);
            this.l105.TabIndex = 312;
            this.l105.Text = "-";
            this.l105.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l104
            // 
            this.l104.BackColor = System.Drawing.SystemColors.Window;
            this.l104.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l104.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l104.Location = new System.Drawing.Point(128, 188);
            this.l104.Name = "l104";
            this.l104.Size = new System.Drawing.Size(32, 15);
            this.l104.TabIndex = 298;
            this.l104.Text = "-";
            this.l104.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l024
            // 
            this.l024.BackColor = System.Drawing.SystemColors.Window;
            this.l024.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l024.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l024.Location = new System.Drawing.Point(128, 48);
            this.l024.Name = "l024";
            this.l024.Size = new System.Drawing.Size(32, 15);
            this.l024.TabIndex = 290;
            this.l024.Text = "-";
            this.l024.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l102
            // 
            this.l102.BackColor = System.Drawing.SystemColors.Window;
            this.l102.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l102.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l102.Location = new System.Drawing.Point(48, 188);
            this.l102.Name = "l102";
            this.l102.Size = new System.Drawing.Size(32, 15);
            this.l102.TabIndex = 270;
            this.l102.Text = "-";
            this.l102.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l026
            // 
            this.l026.BackColor = System.Drawing.SystemColors.Window;
            this.l026.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l026.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l026.Location = new System.Drawing.Point(208, 48);
            this.l026.Name = "l026";
            this.l026.Size = new System.Drawing.Size(32, 15);
            this.l026.TabIndex = 318;
            this.l026.Text = "-";
            this.l026.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l027
            // 
            this.l027.BackColor = System.Drawing.SystemColors.Window;
            this.l027.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l027.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l027.Location = new System.Drawing.Point(248, 48);
            this.l027.Name = "l027";
            this.l027.Size = new System.Drawing.Size(32, 15);
            this.l027.TabIndex = 332;
            this.l027.Text = "-";
            this.l027.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l021
            // 
            this.l021.BackColor = System.Drawing.SystemColors.Window;
            this.l021.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l021.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l021.Location = new System.Drawing.Point(8, 48);
            this.l021.Name = "l021";
            this.l021.Size = new System.Drawing.Size(32, 15);
            this.l021.TabIndex = 248;
            this.l021.Text = "-";
            this.l021.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l022
            // 
            this.l022.BackColor = System.Drawing.SystemColors.Window;
            this.l022.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l022.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l022.Location = new System.Drawing.Point(48, 48);
            this.l022.Name = "l022";
            this.l022.Size = new System.Drawing.Size(32, 15);
            this.l022.TabIndex = 262;
            this.l022.Text = "-";
            this.l022.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l023
            // 
            this.l023.BackColor = System.Drawing.SystemColors.Window;
            this.l023.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l023.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l023.Location = new System.Drawing.Point(88, 48);
            this.l023.Name = "l023";
            this.l023.Size = new System.Drawing.Size(32, 15);
            this.l023.TabIndex = 276;
            this.l023.Text = "-";
            this.l023.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l073
            // 
            this.l073.BackColor = System.Drawing.SystemColors.Info;
            this.l073.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l073.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l073.Location = new System.Drawing.Point(88, 134);
            this.l073.Name = "l073";
            this.l073.Size = new System.Drawing.Size(32, 15);
            this.l073.TabIndex = 279;
            this.l073.Text = "-";
            this.l073.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lfile
            // 
            this.lfile.BackColor = System.Drawing.SystemColors.Info;
            this.lfile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lfile.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lfile.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lfile.Location = new System.Drawing.Point(572, 213);
            this.lfile.Name = "lfile";
            this.lfile.Size = new System.Drawing.Size(132, 22);
            this.lfile.TabIndex = 362;
            this.lfile.Text = "Fichero";
            this.lfile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l041
            // 
            this.l041.BackColor = System.Drawing.SystemColors.Window;
            this.l041.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l041.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l041.Location = new System.Drawing.Point(8, 80);
            this.l041.Name = "l041";
            this.l041.Size = new System.Drawing.Size(32, 15);
            this.l041.TabIndex = 246;
            this.l041.Text = "-";
            this.l041.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l025
            // 
            this.l025.BackColor = System.Drawing.SystemColors.Window;
            this.l025.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l025.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l025.Location = new System.Drawing.Point(168, 48);
            this.l025.Name = "l025";
            this.l025.Size = new System.Drawing.Size(32, 15);
            this.l025.TabIndex = 304;
            this.l025.Text = "-";
            this.l025.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bCalcular
            // 
            this.bCalcular.BackColor = System.Drawing.Color.DarkSalmon;
            this.bCalcular.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bCalcular.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bCalcular.Image = ((System.Drawing.Image)(resources.GetObject("bCalcular.Image")));
            this.bCalcular.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bCalcular.Location = new System.Drawing.Point(636, 16);
            this.bCalcular.Name = "bCalcular";
            this.bCalcular.Size = new System.Drawing.Size(104, 32);
            this.bCalcular.TabIndex = 334;
            this.bCalcular.Text = "Calcular";
            this.bCalcular.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bCalcular.UseVisualStyleBackColor = false;
            this.bCalcular.Click += new System.EventHandler(this.BCalcularClick);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.NavajoWhite;
            this.button3.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.ForeColor = System.Drawing.Color.Black;
            this.button3.Location = new System.Drawing.Point(8, 22);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(24, 23);
            this.button3.TabIndex = 87;
            this.button3.Text = "L";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.BFGClick);
            // 
            // l016
            // 
            this.l016.BackColor = System.Drawing.SystemColors.Window;
            this.l016.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l016.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l016.Location = new System.Drawing.Point(208, 33);
            this.l016.Name = "l016";
            this.l016.Size = new System.Drawing.Size(32, 14);
            this.l016.TabIndex = 315;
            this.l016.Text = "-";
            this.l016.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l015
            // 
            this.l015.BackColor = System.Drawing.SystemColors.Window;
            this.l015.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l015.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l015.Location = new System.Drawing.Point(168, 33);
            this.l015.Name = "l015";
            this.l015.Size = new System.Drawing.Size(32, 14);
            this.l015.TabIndex = 301;
            this.l015.Text = "-";
            this.l015.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lTime
            // 
            this.lTime.BackColor = System.Drawing.SystemColors.Info;
            this.lTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lTime.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lTime.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lTime.Location = new System.Drawing.Point(636, 95);
            this.lTime.Name = "lTime";
            this.lTime.Size = new System.Drawing.Size(104, 22);
            this.lTime.TabIndex = 333;
            this.lTime.Text = "Tiempo";
            this.lTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l013
            // 
            this.l013.BackColor = System.Drawing.SystemColors.Window;
            this.l013.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l013.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l013.Location = new System.Drawing.Point(88, 33);
            this.l013.Name = "l013";
            this.l013.Size = new System.Drawing.Size(32, 14);
            this.l013.TabIndex = 273;
            this.l013.Text = "-";
            this.l013.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l012
            // 
            this.l012.BackColor = System.Drawing.SystemColors.Window;
            this.l012.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l012.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l012.Location = new System.Drawing.Point(48, 33);
            this.l012.Name = "l012";
            this.l012.Size = new System.Drawing.Size(32, 14);
            this.l012.TabIndex = 259;
            this.l012.Text = "-";
            this.l012.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l011
            // 
            this.l011.BackColor = System.Drawing.SystemColors.Window;
            this.l011.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l011.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l011.Location = new System.Drawing.Point(8, 33);
            this.l011.Name = "l011";
            this.l011.Size = new System.Drawing.Size(32, 14);
            this.l011.TabIndex = 245;
            this.l011.Text = "-";
            this.l011.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l087
            // 
            this.l087.BackColor = System.Drawing.SystemColors.Info;
            this.l087.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l087.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l087.Location = new System.Drawing.Point(248, 150);
            this.l087.Name = "l087";
            this.l087.Size = new System.Drawing.Size(32, 15);
            this.l087.TabIndex = 334;
            this.l087.Text = "-";
            this.l087.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l097
            // 
            this.l097.BackColor = System.Drawing.SystemColors.Window;
            this.l097.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l097.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l097.Location = new System.Drawing.Point(248, 172);
            this.l097.Name = "l097";
            this.l097.Size = new System.Drawing.Size(32, 15);
            this.l097.TabIndex = 337;
            this.l097.Text = "-";
            this.l097.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l096
            // 
            this.l096.BackColor = System.Drawing.SystemColors.Window;
            this.l096.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l096.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l096.Location = new System.Drawing.Point(208, 172);
            this.l096.Name = "l096";
            this.l096.Size = new System.Drawing.Size(32, 15);
            this.l096.TabIndex = 323;
            this.l096.Text = "-";
            this.l096.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l095
            // 
            this.l095.BackColor = System.Drawing.SystemColors.Window;
            this.l095.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l095.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l095.Location = new System.Drawing.Point(168, 172);
            this.l095.Name = "l095";
            this.l095.Size = new System.Drawing.Size(32, 15);
            this.l095.TabIndex = 309;
            this.l095.Text = "-";
            this.l095.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l094
            // 
            this.l094.BackColor = System.Drawing.SystemColors.Window;
            this.l094.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l094.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l094.Location = new System.Drawing.Point(128, 172);
            this.l094.Name = "l094";
            this.l094.Size = new System.Drawing.Size(32, 15);
            this.l094.TabIndex = 295;
            this.l094.Text = "-";
            this.l094.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l101
            // 
            this.l101.BackColor = System.Drawing.SystemColors.Window;
            this.l101.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l101.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l101.Location = new System.Drawing.Point(8, 188);
            this.l101.Name = "l101";
            this.l101.Size = new System.Drawing.Size(32, 15);
            this.l101.TabIndex = 256;
            this.l101.Text = "-";
            this.l101.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lColsAdm
            // 
            this.lColsAdm.BackColor = System.Drawing.SystemColors.Info;
            this.lColsAdm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lColsAdm.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lColsAdm.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lColsAdm.Location = new System.Drawing.Point(636, 72);
            this.lColsAdm.Name = "lColsAdm";
            this.lColsAdm.Size = new System.Drawing.Size(104, 22);
            this.lColsAdm.TabIndex = 331;
            this.lColsAdm.Text = "Admitidas";
            this.lColsAdm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l091
            // 
            this.l091.BackColor = System.Drawing.SystemColors.Window;
            this.l091.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l091.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l091.Location = new System.Drawing.Point(8, 172);
            this.l091.Name = "l091";
            this.l091.Size = new System.Drawing.Size(32, 15);
            this.l091.TabIndex = 253;
            this.l091.Text = "-";
            this.l091.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lFGR
            // 
            this.lFGR.BackColor = System.Drawing.SystemColors.Info;
            this.lFGR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lFGR.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lFGR.ForeColor = System.Drawing.Color.Black;
            this.lFGR.Location = new System.Drawing.Point(40, 22);
            this.lFGR.Name = "lFGR";
            this.lFGR.Size = new System.Drawing.Size(144, 23);
            this.lFGR.TabIndex = 88;
            this.lFGR.Text = "Fichero Ganadoras";
            this.lFGR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lrk1
            // 
            this.lrk1.BackColor = System.Drawing.SystemColors.Info;
            this.lrk1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lrk1.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lrk1.Location = new System.Drawing.Point(8, 327);
            this.lrk1.Name = "lrk1";
            this.lrk1.Size = new System.Drawing.Size(32, 18);
            this.lrk1.TabIndex = 350;
            this.lrk1.Text = "-";
            this.lrk1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l134
            // 
            this.l134.BackColor = System.Drawing.SystemColors.Info;
            this.l134.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l134.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l134.Location = new System.Drawing.Point(128, 242);
            this.l134.Name = "l134";
            this.l134.Size = new System.Drawing.Size(32, 15);
            this.l134.TabIndex = 300;
            this.l134.Text = "-";
            this.l134.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l135
            // 
            this.l135.BackColor = System.Drawing.SystemColors.Info;
            this.l135.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l135.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l135.Location = new System.Drawing.Point(168, 242);
            this.l135.Name = "l135";
            this.l135.Size = new System.Drawing.Size(32, 15);
            this.l135.TabIndex = 314;
            this.l135.Text = "-";
            this.l135.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l136
            // 
            this.l136.BackColor = System.Drawing.SystemColors.Info;
            this.l136.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l136.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l136.Location = new System.Drawing.Point(208, 242);
            this.l136.Name = "l136";
            this.l136.Size = new System.Drawing.Size(32, 15);
            this.l136.TabIndex = 328;
            this.l136.Text = "-";
            this.l136.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l137
            // 
            this.l137.BackColor = System.Drawing.SystemColors.Info;
            this.l137.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l137.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l137.Location = new System.Drawing.Point(248, 242);
            this.l137.Name = "l137";
            this.l137.Size = new System.Drawing.Size(32, 15);
            this.l137.TabIndex = 342;
            this.l137.Text = "-";
            this.l137.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lColsIni
            // 
            this.lColsIni.BackColor = System.Drawing.SystemColors.Info;
            this.lColsIni.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lColsIni.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lColsIni.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lColsIni.Location = new System.Drawing.Point(636, 49);
            this.lColsIni.Name = "lColsIni";
            this.lColsIni.Size = new System.Drawing.Size(104, 22);
            this.lColsIni.TabIndex = 332;
            this.lColsIni.Text = "Procesadas";
            this.lColsIni.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l131
            // 
            this.l131.BackColor = System.Drawing.SystemColors.Info;
            this.l131.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l131.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l131.Location = new System.Drawing.Point(8, 242);
            this.l131.Name = "l131";
            this.l131.Size = new System.Drawing.Size(32, 15);
            this.l131.TabIndex = 258;
            this.l131.Text = "-";
            this.l131.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bCancelar
            // 
            this.bCancelar.BackColor = System.Drawing.Color.DarkSalmon;
            this.bCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bCancelar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bCancelar.Location = new System.Drawing.Point(636, 118);
            this.bCancelar.Name = "bCancelar";
            this.bCancelar.Size = new System.Drawing.Size(104, 32);
            this.bCancelar.TabIndex = 335;
            this.bCancelar.Text = "Cancelar";
            this.bCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bCancelar.UseVisualStyleBackColor = false;
            this.bCancelar.Click += new System.EventHandler(this.BCancelarClick);
            // 
            // l072
            // 
            this.l072.BackColor = System.Drawing.SystemColors.Info;
            this.l072.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l072.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l072.Location = new System.Drawing.Point(48, 134);
            this.l072.Name = "l072";
            this.l072.Size = new System.Drawing.Size(32, 15);
            this.l072.TabIndex = 265;
            this.l072.Text = "-";
            this.l072.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l075
            // 
            this.l075.BackColor = System.Drawing.SystemColors.Info;
            this.l075.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l075.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l075.Location = new System.Drawing.Point(168, 134);
            this.l075.Name = "l075";
            this.l075.Size = new System.Drawing.Size(32, 15);
            this.l075.TabIndex = 307;
            this.l075.Text = "-";
            this.l075.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l074
            // 
            this.l074.BackColor = System.Drawing.SystemColors.Info;
            this.l074.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l074.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l074.Location = new System.Drawing.Point(128, 134);
            this.l074.Name = "l074";
            this.l074.Size = new System.Drawing.Size(32, 15);
            this.l074.TabIndex = 293;
            this.l074.Text = "-";
            this.l074.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l077
            // 
            this.l077.BackColor = System.Drawing.SystemColors.Info;
            this.l077.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l077.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l077.Location = new System.Drawing.Point(248, 134);
            this.l077.Name = "l077";
            this.l077.Size = new System.Drawing.Size(32, 15);
            this.l077.TabIndex = 335;
            this.l077.Text = "-";
            this.l077.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l076
            // 
            this.l076.BackColor = System.Drawing.SystemColors.Info;
            this.l076.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l076.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l076.Location = new System.Drawing.Point(208, 134);
            this.l076.Name = "l076";
            this.l076.Size = new System.Drawing.Size(32, 15);
            this.l076.TabIndex = 321;
            this.l076.Text = "-";
            this.l076.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l086
            // 
            this.l086.BackColor = System.Drawing.SystemColors.Info;
            this.l086.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l086.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l086.Location = new System.Drawing.Point(208, 150);
            this.l086.Name = "l086";
            this.l086.Size = new System.Drawing.Size(32, 15);
            this.l086.TabIndex = 320;
            this.l086.Text = "-";
            this.l086.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l084
            // 
            this.l084.BackColor = System.Drawing.SystemColors.Info;
            this.l084.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l084.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l084.Location = new System.Drawing.Point(128, 150);
            this.l084.Name = "l084";
            this.l084.Size = new System.Drawing.Size(32, 15);
            this.l084.TabIndex = 292;
            this.l084.Text = "-";
            this.l084.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l085
            // 
            this.l085.BackColor = System.Drawing.SystemColors.Info;
            this.l085.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l085.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l085.Location = new System.Drawing.Point(168, 150);
            this.l085.Name = "l085";
            this.l085.Size = new System.Drawing.Size(32, 15);
            this.l085.TabIndex = 306;
            this.l085.Text = "-";
            this.l085.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l082
            // 
            this.l082.BackColor = System.Drawing.SystemColors.Info;
            this.l082.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l082.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l082.Location = new System.Drawing.Point(48, 150);
            this.l082.Name = "l082";
            this.l082.Size = new System.Drawing.Size(32, 15);
            this.l082.TabIndex = 264;
            this.l082.Text = "-";
            this.l082.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l083
            // 
            this.l083.BackColor = System.Drawing.SystemColors.Info;
            this.l083.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l083.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l083.Location = new System.Drawing.Point(88, 150);
            this.l083.Name = "l083";
            this.l083.Size = new System.Drawing.Size(32, 15);
            this.l083.TabIndex = 278;
            this.l083.Text = "-";
            this.l083.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l081
            // 
            this.l081.BackColor = System.Drawing.SystemColors.Info;
            this.l081.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l081.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l081.Location = new System.Drawing.Point(8, 150);
            this.l081.Name = "l081";
            this.l081.Size = new System.Drawing.Size(32, 15);
            this.l081.TabIndex = 250;
            this.l081.Text = "-";
            this.l081.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbrank6
            // 
            this.tbrank6.BackColor = System.Drawing.Color.PaleGreen;
            this.tbrank6.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbrank6.ForeColor = System.Drawing.Color.Maroon;
            this.tbrank6.Location = new System.Drawing.Point(208, 305);
            this.tbrank6.Name = "tbrank6";
            this.tbrank6.Size = new System.Drawing.Size(32, 18);
            this.tbrank6.TabIndex = 348;
            this.tbrank6.Text = "0-1";
            this.tbrank6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbrank7
            // 
            this.tbrank7.BackColor = System.Drawing.Color.PaleGreen;
            this.tbrank7.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbrank7.ForeColor = System.Drawing.Color.Maroon;
            this.tbrank7.Location = new System.Drawing.Point(248, 305);
            this.tbrank7.Name = "tbrank7";
            this.tbrank7.Size = new System.Drawing.Size(32, 18);
            this.tbrank7.TabIndex = 349;
            this.tbrank7.Text = "0-1";
            this.tbrank7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbrank4
            // 
            this.tbrank4.BackColor = System.Drawing.Color.PaleGreen;
            this.tbrank4.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbrank4.ForeColor = System.Drawing.Color.Maroon;
            this.tbrank4.Location = new System.Drawing.Point(128, 305);
            this.tbrank4.Name = "tbrank4";
            this.tbrank4.Size = new System.Drawing.Size(32, 18);
            this.tbrank4.TabIndex = 346;
            this.tbrank4.Text = "2-6";
            this.tbrank4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbrank5
            // 
            this.tbrank5.BackColor = System.Drawing.Color.PaleGreen;
            this.tbrank5.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbrank5.ForeColor = System.Drawing.Color.Maroon;
            this.tbrank5.Location = new System.Drawing.Point(168, 305);
            this.tbrank5.Name = "tbrank5";
            this.tbrank5.Size = new System.Drawing.Size(32, 18);
            this.tbrank5.TabIndex = 347;
            this.tbrank5.Text = "2-6";
            this.tbrank5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ltColR
            // 
            this.ltColR.BackColor = System.Drawing.SystemColors.Window;
            this.ltColR.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.ltColR.ForeColor = System.Drawing.Color.Black;
            this.ltColR.Location = new System.Drawing.Point(120, 67);
            this.ltColR.MaxLength = 14;
            this.ltColR.Name = "ltColR";
            this.ltColR.Size = new System.Drawing.Size(128, 21);
            this.ltColR.TabIndex = 373;
            this.ltColR.Text = "COL.GANADORA";
            this.ltColR.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // l143
            // 
            this.l143.BackColor = System.Drawing.SystemColors.Info;
            this.l143.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l143.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l143.Location = new System.Drawing.Point(88, 258);
            this.l143.Name = "l143";
            this.l143.Size = new System.Drawing.Size(32, 15);
            this.l143.TabIndex = 285;
            this.l143.Text = "-";
            this.l143.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l142
            // 
            this.l142.BackColor = System.Drawing.SystemColors.Info;
            this.l142.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l142.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l142.Location = new System.Drawing.Point(48, 258);
            this.l142.Name = "l142";
            this.l142.Size = new System.Drawing.Size(32, 15);
            this.l142.TabIndex = 271;
            this.l142.Text = "-";
            this.l142.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l141
            // 
            this.l141.BackColor = System.Drawing.SystemColors.Info;
            this.l141.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l141.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l141.Location = new System.Drawing.Point(8, 258);
            this.l141.Name = "l141";
            this.l141.Size = new System.Drawing.Size(32, 15);
            this.l141.TabIndex = 257;
            this.l141.Text = "-";
            this.l141.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l147
            // 
            this.l147.BackColor = System.Drawing.SystemColors.Info;
            this.l147.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l147.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l147.Location = new System.Drawing.Point(248, 258);
            this.l147.Name = "l147";
            this.l147.Size = new System.Drawing.Size(32, 15);
            this.l147.TabIndex = 341;
            this.l147.Text = "-";
            this.l147.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l061
            // 
            this.l061.BackColor = System.Drawing.SystemColors.Info;
            this.l061.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l061.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l061.Location = new System.Drawing.Point(8, 118);
            this.l061.Name = "l061";
            this.l061.Size = new System.Drawing.Size(32, 15);
            this.l061.TabIndex = 252;
            this.l061.Text = "-";
            this.l061.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l062
            // 
            this.l062.BackColor = System.Drawing.SystemColors.Info;
            this.l062.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l062.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l062.Location = new System.Drawing.Point(48, 118);
            this.l062.Name = "l062";
            this.l062.Size = new System.Drawing.Size(32, 15);
            this.l062.TabIndex = 266;
            this.l062.Text = "-";
            this.l062.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l063
            // 
            this.l063.BackColor = System.Drawing.SystemColors.Info;
            this.l063.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l063.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l063.Location = new System.Drawing.Point(88, 118);
            this.l063.Name = "l063";
            this.l063.Size = new System.Drawing.Size(32, 15);
            this.l063.TabIndex = 280;
            this.l063.Text = "-";
            this.l063.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l064
            // 
            this.l064.BackColor = System.Drawing.SystemColors.Info;
            this.l064.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l064.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l064.Location = new System.Drawing.Point(128, 118);
            this.l064.Name = "l064";
            this.l064.Size = new System.Drawing.Size(32, 15);
            this.l064.TabIndex = 294;
            this.l064.Text = "-";
            this.l064.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l065
            // 
            this.l065.BackColor = System.Drawing.SystemColors.Info;
            this.l065.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l065.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l065.Location = new System.Drawing.Point(168, 118);
            this.l065.Name = "l065";
            this.l065.Size = new System.Drawing.Size(32, 15);
            this.l065.TabIndex = 308;
            this.l065.Text = "-";
            this.l065.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l066
            // 
            this.l066.BackColor = System.Drawing.SystemColors.Info;
            this.l066.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l066.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l066.Location = new System.Drawing.Point(208, 118);
            this.l066.Name = "l066";
            this.l066.Size = new System.Drawing.Size(32, 15);
            this.l066.TabIndex = 322;
            this.l066.Text = "-";
            this.l066.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l103
            // 
            this.l103.BackColor = System.Drawing.SystemColors.Window;
            this.l103.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l103.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l103.Location = new System.Drawing.Point(88, 188);
            this.l103.Name = "l103";
            this.l103.Size = new System.Drawing.Size(32, 15);
            this.l103.TabIndex = 284;
            this.l103.Text = "-";
            this.l103.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l017
            // 
            this.l017.BackColor = System.Drawing.SystemColors.Window;
            this.l017.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l017.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l017.Location = new System.Drawing.Point(248, 33);
            this.l017.Name = "l017";
            this.l017.Size = new System.Drawing.Size(32, 14);
            this.l017.TabIndex = 329;
            this.l017.Text = "-";
            this.l017.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lrangos
            // 
            this.lrangos.BackColor = System.Drawing.SystemColors.Info;
            this.lrangos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lrangos.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lrangos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lrangos.Location = new System.Drawing.Point(480, 49);
            this.lrangos.Name = "lrangos";
            this.lrangos.Size = new System.Drawing.Size(138, 22);
            this.lrangos.TabIndex = 368;
            this.lrangos.Text = "Fichero";
            this.lrangos.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l014
            // 
            this.l014.BackColor = System.Drawing.SystemColors.Window;
            this.l014.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l014.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l014.Location = new System.Drawing.Point(128, 33);
            this.l014.Name = "l014";
            this.l014.Size = new System.Drawing.Size(32, 14);
            this.l014.TabIndex = 287;
            this.l014.Text = "-";
            this.l014.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bGrabaCols
            // 
            this.bGrabaCols.BackColor = System.Drawing.Color.DarkSalmon;
            this.bGrabaCols.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bGrabaCols.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bGrabaCols.Image = ((System.Drawing.Image)(resources.GetObject("bGrabaCols.Image")));
            this.bGrabaCols.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bGrabaCols.Location = new System.Drawing.Point(572, 179);
            this.bGrabaCols.Name = "bGrabaCols";
            this.bGrabaCols.Size = new System.Drawing.Size(132, 32);
            this.bGrabaCols.TabIndex = 197;
            this.bGrabaCols.Text = "Grabar resultado";
            this.bGrabaCols.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bGrabaCols.UseVisualStyleBackColor = false;
            this.bGrabaCols.Click += new System.EventHandler(this.BGrabaColsClick);
            // 
            // bAnalizar
            // 
            this.bAnalizar.BackColor = System.Drawing.Color.DarkSalmon;
            this.bAnalizar.Enabled = false;
            this.bAnalizar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bAnalizar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bAnalizar.ForeColor = System.Drawing.Color.Black;
            this.bAnalizar.Image = ((System.Drawing.Image)(resources.GetObject("bAnalizar.Image")));
            this.bAnalizar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bAnalizar.Location = new System.Drawing.Point(8, 59);
            this.bAnalizar.Name = "bAnalizar";
            this.bAnalizar.Size = new System.Drawing.Size(104, 32);
            this.bAnalizar.TabIndex = 27;
            this.bAnalizar.Text = "Analizar";
            this.bAnalizar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bAnalizar.UseVisualStyleBackColor = false;
            this.bAnalizar.Click += new System.EventHandler(this.BAnalizarClick);
            // 
            // bMasR
            // 
            this.bMasR.BackColor = System.Drawing.Color.DarkSalmon;
            this.bMasR.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bMasR.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bMasR.ForeColor = System.Drawing.Color.Black;
            this.bMasR.Location = new System.Drawing.Point(232, 22);
            this.bMasR.Name = "bMasR";
            this.bMasR.Size = new System.Drawing.Size(16, 15);
            this.bMasR.TabIndex = 84;
            this.bMasR.Text = "+";
            this.bMasR.UseVisualStyleBackColor = false;
            this.bMasR.Click += new System.EventHandler(this.BMasRClick);
            // 
            // l132
            // 
            this.l132.BackColor = System.Drawing.SystemColors.Info;
            this.l132.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l132.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l132.Location = new System.Drawing.Point(48, 242);
            this.l132.Name = "l132";
            this.l132.Size = new System.Drawing.Size(32, 15);
            this.l132.TabIndex = 272;
            this.l132.Text = "-";
            this.l132.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l133
            // 
            this.l133.BackColor = System.Drawing.SystemColors.Info;
            this.l133.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l133.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l133.Location = new System.Drawing.Point(88, 242);
            this.l133.Name = "l133";
            this.l133.Size = new System.Drawing.Size(32, 15);
            this.l133.TabIndex = 286;
            this.l133.Text = "-";
            this.l133.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbrank2
            // 
            this.tbrank2.BackColor = System.Drawing.Color.PaleGreen;
            this.tbrank2.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbrank2.ForeColor = System.Drawing.Color.Maroon;
            this.tbrank2.Location = new System.Drawing.Point(48, 305);
            this.tbrank2.Name = "tbrank2";
            this.tbrank2.Size = new System.Drawing.Size(32, 18);
            this.tbrank2.TabIndex = 344;
            this.tbrank2.Text = "2-6";
            this.tbrank2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Bisque;
            this.label2.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(8, 349);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(80, 19);
            this.label2.TabIndex = 370;
            this.label2.Text = "recorrido:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // l117
            // 
            this.l117.BackColor = System.Drawing.SystemColors.Window;
            this.l117.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l117.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l117.Location = new System.Drawing.Point(248, 204);
            this.l117.Name = "l117";
            this.l117.Size = new System.Drawing.Size(32, 15);
            this.l117.TabIndex = 339;
            this.l117.Text = "-";
            this.l117.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l114
            // 
            this.l114.BackColor = System.Drawing.SystemColors.Window;
            this.l114.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l114.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l114.Location = new System.Drawing.Point(128, 204);
            this.l114.Name = "l114";
            this.l114.Size = new System.Drawing.Size(32, 15);
            this.l114.TabIndex = 297;
            this.l114.Text = "-";
            this.l114.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l115
            // 
            this.l115.BackColor = System.Drawing.SystemColors.Window;
            this.l115.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l115.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l115.Location = new System.Drawing.Point(168, 204);
            this.l115.Name = "l115";
            this.l115.Size = new System.Drawing.Size(32, 15);
            this.l115.TabIndex = 311;
            this.l115.Text = "-";
            this.l115.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l053
            // 
            this.l053.BackColor = System.Drawing.SystemColors.Info;
            this.l053.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l053.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l053.Location = new System.Drawing.Point(88, 102);
            this.l053.Name = "l053";
            this.l053.Size = new System.Drawing.Size(32, 15);
            this.l053.TabIndex = 277;
            this.l053.Text = "-";
            this.l053.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l052
            // 
            this.l052.BackColor = System.Drawing.SystemColors.Info;
            this.l052.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l052.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l052.Location = new System.Drawing.Point(48, 102);
            this.l052.Name = "l052";
            this.l052.Size = new System.Drawing.Size(32, 15);
            this.l052.TabIndex = 263;
            this.l052.Text = "-";
            this.l052.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l051
            // 
            this.l051.BackColor = System.Drawing.SystemColors.Info;
            this.l051.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l051.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l051.Location = new System.Drawing.Point(8, 102);
            this.l051.Name = "l051";
            this.l051.Size = new System.Drawing.Size(32, 15);
            this.l051.TabIndex = 249;
            this.l051.Text = "-";
            this.l051.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bSalvarConds
            // 
            this.bSalvarConds.BackColor = System.Drawing.Color.DarkSalmon;
            this.bSalvarConds.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bSalvarConds.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bSalvarConds.Image = ((System.Drawing.Image)(resources.GetObject("bSalvarConds.Image")));
            this.bSalvarConds.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bSalvarConds.Location = new System.Drawing.Point(480, 16);
            this.bSalvarConds.Name = "bSalvarConds";
            this.bSalvarConds.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.bSalvarConds.Size = new System.Drawing.Size(138, 32);
            this.bSalvarConds.TabIndex = 336;
            this.bSalvarConds.Text = "Salvar rangos";
            this.bSalvarConds.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bSalvarConds.UseVisualStyleBackColor = false;
            this.bSalvarConds.Click += new System.EventHandler(this.BSalvarCondsClick);
            // 
            // l057
            // 
            this.l057.BackColor = System.Drawing.SystemColors.Info;
            this.l057.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l057.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l057.Location = new System.Drawing.Point(248, 102);
            this.l057.Name = "l057";
            this.l057.Size = new System.Drawing.Size(32, 15);
            this.l057.TabIndex = 333;
            this.l057.Text = "-";
            this.l057.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l056
            // 
            this.l056.BackColor = System.Drawing.SystemColors.Info;
            this.l056.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l056.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l056.Location = new System.Drawing.Point(208, 102);
            this.l056.Name = "l056";
            this.l056.Size = new System.Drawing.Size(32, 15);
            this.l056.TabIndex = 319;
            this.l056.Text = "-";
            this.l056.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l055
            // 
            this.l055.BackColor = System.Drawing.SystemColors.Info;
            this.l055.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l055.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l055.Location = new System.Drawing.Point(168, 102);
            this.l055.Name = "l055";
            this.l055.Size = new System.Drawing.Size(32, 15);
            this.l055.TabIndex = 305;
            this.l055.Text = "-";
            this.l055.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l054
            // 
            this.l054.BackColor = System.Drawing.SystemColors.Info;
            this.l054.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l054.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l054.Location = new System.Drawing.Point(128, 102);
            this.l054.Name = "l054";
            this.l054.Size = new System.Drawing.Size(32, 15);
            this.l054.TabIndex = 291;
            this.l054.Text = "-";
            this.l054.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bLeerConds
            // 
            this.bLeerConds.BackColor = System.Drawing.Color.DarkSalmon;
            this.bLeerConds.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bLeerConds.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bLeerConds.Image = ((System.Drawing.Image)(resources.GetObject("bLeerConds.Image")));
            this.bLeerConds.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bLeerConds.Location = new System.Drawing.Point(480, 72);
            this.bLeerConds.Name = "bLeerConds";
            this.bLeerConds.Size = new System.Drawing.Size(138, 32);
            this.bLeerConds.TabIndex = 337;
            this.bLeerConds.Text = "Recuperar rangos";
            this.bLeerConds.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bLeerConds.UseVisualStyleBackColor = false;
            this.bLeerConds.Click += new System.EventHandler(this.BLeerCondsClick);
            // 
            // l093
            // 
            this.l093.BackColor = System.Drawing.SystemColors.Window;
            this.l093.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l093.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l093.Location = new System.Drawing.Point(88, 172);
            this.l093.Name = "l093";
            this.l093.Size = new System.Drawing.Size(32, 15);
            this.l093.TabIndex = 281;
            this.l093.Text = "-";
            this.l093.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l092
            // 
            this.l092.BackColor = System.Drawing.SystemColors.Window;
            this.l092.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l092.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l092.Location = new System.Drawing.Point(48, 172);
            this.l092.Name = "l092";
            this.l092.Size = new System.Drawing.Size(32, 15);
            this.l092.TabIndex = 267;
            this.l092.Text = "-";
            this.l092.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l071
            // 
            this.l071.BackColor = System.Drawing.SystemColors.Info;
            this.l071.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l071.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l071.Location = new System.Drawing.Point(8, 134);
            this.l071.Name = "l071";
            this.l071.Size = new System.Drawing.Size(32, 15);
            this.l071.TabIndex = 251;
            this.l071.Text = "-";
            this.l071.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.lreco);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.tbmgreco);
            this.groupBox2.Controls.Add(this.lrk7);
            this.groupBox2.Controls.Add(this.lrk6);
            this.groupBox2.Controls.Add(this.lrk5);
            this.groupBox2.Controls.Add(this.lrk4);
            this.groupBox2.Controls.Add(this.lrk3);
            this.groupBox2.Controls.Add(this.lrk2);
            this.groupBox2.Controls.Add(this.lrk1);
            this.groupBox2.Controls.Add(this.tbrank7);
            this.groupBox2.Controls.Add(this.tbrank6);
            this.groupBox2.Controls.Add(this.tbrank5);
            this.groupBox2.Controls.Add(this.tbrank4);
            this.groupBox2.Controls.Add(this.tbrank3);
            this.groupBox2.Controls.Add(this.tbrank2);
            this.groupBox2.Controls.Add(this.tbrank1);
            this.groupBox2.Controls.Add(this.l137);
            this.groupBox2.Controls.Add(this.l147);
            this.groupBox2.Controls.Add(this.l107);
            this.groupBox2.Controls.Add(this.l117);
            this.groupBox2.Controls.Add(this.l127);
            this.groupBox2.Controls.Add(this.l097);
            this.groupBox2.Controls.Add(this.l067);
            this.groupBox2.Controls.Add(this.l077);
            this.groupBox2.Controls.Add(this.l087);
            this.groupBox2.Controls.Add(this.l057);
            this.groupBox2.Controls.Add(this.l027);
            this.groupBox2.Controls.Add(this.l037);
            this.groupBox2.Controls.Add(this.l047);
            this.groupBox2.Controls.Add(this.l017);
            this.groupBox2.Controls.Add(this.l136);
            this.groupBox2.Controls.Add(this.l146);
            this.groupBox2.Controls.Add(this.l106);
            this.groupBox2.Controls.Add(this.l116);
            this.groupBox2.Controls.Add(this.l126);
            this.groupBox2.Controls.Add(this.l096);
            this.groupBox2.Controls.Add(this.l066);
            this.groupBox2.Controls.Add(this.l076);
            this.groupBox2.Controls.Add(this.l086);
            this.groupBox2.Controls.Add(this.l056);
            this.groupBox2.Controls.Add(this.l026);
            this.groupBox2.Controls.Add(this.l036);
            this.groupBox2.Controls.Add(this.l046);
            this.groupBox2.Controls.Add(this.l016);
            this.groupBox2.Controls.Add(this.l135);
            this.groupBox2.Controls.Add(this.l145);
            this.groupBox2.Controls.Add(this.l105);
            this.groupBox2.Controls.Add(this.l115);
            this.groupBox2.Controls.Add(this.l125);
            this.groupBox2.Controls.Add(this.l095);
            this.groupBox2.Controls.Add(this.l065);
            this.groupBox2.Controls.Add(this.l075);
            this.groupBox2.Controls.Add(this.l085);
            this.groupBox2.Controls.Add(this.l055);
            this.groupBox2.Controls.Add(this.l025);
            this.groupBox2.Controls.Add(this.l035);
            this.groupBox2.Controls.Add(this.l045);
            this.groupBox2.Controls.Add(this.l015);
            this.groupBox2.Controls.Add(this.l134);
            this.groupBox2.Controls.Add(this.l144);
            this.groupBox2.Controls.Add(this.l104);
            this.groupBox2.Controls.Add(this.l114);
            this.groupBox2.Controls.Add(this.l124);
            this.groupBox2.Controls.Add(this.l094);
            this.groupBox2.Controls.Add(this.l064);
            this.groupBox2.Controls.Add(this.l074);
            this.groupBox2.Controls.Add(this.l084);
            this.groupBox2.Controls.Add(this.l054);
            this.groupBox2.Controls.Add(this.l024);
            this.groupBox2.Controls.Add(this.l034);
            this.groupBox2.Controls.Add(this.l044);
            this.groupBox2.Controls.Add(this.l014);
            this.groupBox2.Controls.Add(this.l133);
            this.groupBox2.Controls.Add(this.l143);
            this.groupBox2.Controls.Add(this.l103);
            this.groupBox2.Controls.Add(this.l113);
            this.groupBox2.Controls.Add(this.l123);
            this.groupBox2.Controls.Add(this.l093);
            this.groupBox2.Controls.Add(this.l063);
            this.groupBox2.Controls.Add(this.l073);
            this.groupBox2.Controls.Add(this.l083);
            this.groupBox2.Controls.Add(this.l053);
            this.groupBox2.Controls.Add(this.l023);
            this.groupBox2.Controls.Add(this.l033);
            this.groupBox2.Controls.Add(this.l043);
            this.groupBox2.Controls.Add(this.l013);
            this.groupBox2.Controls.Add(this.l132);
            this.groupBox2.Controls.Add(this.l142);
            this.groupBox2.Controls.Add(this.l102);
            this.groupBox2.Controls.Add(this.l112);
            this.groupBox2.Controls.Add(this.l122);
            this.groupBox2.Controls.Add(this.l092);
            this.groupBox2.Controls.Add(this.l062);
            this.groupBox2.Controls.Add(this.l072);
            this.groupBox2.Controls.Add(this.l082);
            this.groupBox2.Controls.Add(this.l052);
            this.groupBox2.Controls.Add(this.l022);
            this.groupBox2.Controls.Add(this.l032);
            this.groupBox2.Controls.Add(this.l042);
            this.groupBox2.Controls.Add(this.l012);
            this.groupBox2.Controls.Add(this.l131);
            this.groupBox2.Controls.Add(this.l141);
            this.groupBox2.Controls.Add(this.l101);
            this.groupBox2.Controls.Add(this.l111);
            this.groupBox2.Controls.Add(this.l121);
            this.groupBox2.Controls.Add(this.l091);
            this.groupBox2.Controls.Add(this.l061);
            this.groupBox2.Controls.Add(this.l071);
            this.groupBox2.Controls.Add(this.l081);
            this.groupBox2.Controls.Add(this.l051);
            this.groupBox2.Controls.Add(this.l021);
            this.groupBox2.Controls.Add(this.l031);
            this.groupBox2.Controls.Add(this.l041);
            this.groupBox2.Controls.Add(this.l011);
            this.groupBox2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox2.Location = new System.Drawing.Point(184, 16);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(288, 394);
            this.groupBox2.TabIndex = 189;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Columnas Probables";
            this.groupBox2.Enter += new System.EventHandler(this.groupBox2_Enter);
            // 
            // tbmgreco
            // 
            this.tbmgreco.BackColor = System.Drawing.Color.PaleGreen;
            this.tbmgreco.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbmgreco.Location = new System.Drawing.Point(120, 349);
            this.tbmgreco.MaxLength = 14;
            this.tbmgreco.Name = "tbmgreco";
            this.tbmgreco.Size = new System.Drawing.Size(48, 18);
            this.tbmgreco.TabIndex = 372;
            this.tbmgreco.Text = "0-14";
            this.tbmgreco.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lrk7
            // 
            this.lrk7.BackColor = System.Drawing.SystemColors.Info;
            this.lrk7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lrk7.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lrk7.Location = new System.Drawing.Point(248, 327);
            this.lrk7.Name = "lrk7";
            this.lrk7.Size = new System.Drawing.Size(32, 18);
            this.lrk7.TabIndex = 356;
            this.lrk7.Text = "-";
            this.lrk7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lrk6
            // 
            this.lrk6.BackColor = System.Drawing.SystemColors.Info;
            this.lrk6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lrk6.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lrk6.Location = new System.Drawing.Point(208, 327);
            this.lrk6.Name = "lrk6";
            this.lrk6.Size = new System.Drawing.Size(32, 18);
            this.lrk6.TabIndex = 355;
            this.lrk6.Text = "-";
            this.lrk6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lrk5
            // 
            this.lrk5.BackColor = System.Drawing.SystemColors.Info;
            this.lrk5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lrk5.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lrk5.Location = new System.Drawing.Point(168, 327);
            this.lrk5.Name = "lrk5";
            this.lrk5.Size = new System.Drawing.Size(32, 18);
            this.lrk5.TabIndex = 354;
            this.lrk5.Text = "-";
            this.lrk5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lrk4
            // 
            this.lrk4.BackColor = System.Drawing.SystemColors.Info;
            this.lrk4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lrk4.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lrk4.Location = new System.Drawing.Point(128, 327);
            this.lrk4.Name = "lrk4";
            this.lrk4.Size = new System.Drawing.Size(32, 18);
            this.lrk4.TabIndex = 353;
            this.lrk4.Text = "-";
            this.lrk4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lrk3
            // 
            this.lrk3.BackColor = System.Drawing.SystemColors.Info;
            this.lrk3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lrk3.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lrk3.Location = new System.Drawing.Point(88, 327);
            this.lrk3.Name = "lrk3";
            this.lrk3.Size = new System.Drawing.Size(32, 18);
            this.lrk3.TabIndex = 352;
            this.lrk3.Text = "-";
            this.lrk3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lrk2
            // 
            this.lrk2.BackColor = System.Drawing.SystemColors.Info;
            this.lrk2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lrk2.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lrk2.Location = new System.Drawing.Point(48, 327);
            this.lrk2.Name = "lrk2";
            this.lrk2.Size = new System.Drawing.Size(32, 18);
            this.lrk2.TabIndex = 351;
            this.lrk2.Text = "-";
            this.lrk2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbrank3
            // 
            this.tbrank3.BackColor = System.Drawing.Color.PaleGreen;
            this.tbrank3.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbrank3.ForeColor = System.Drawing.Color.Maroon;
            this.tbrank3.Location = new System.Drawing.Point(88, 305);
            this.tbrank3.Name = "tbrank3";
            this.tbrank3.Size = new System.Drawing.Size(32, 18);
            this.tbrank3.TabIndex = 345;
            this.tbrank3.Text = "2-6";
            this.tbrank3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // l067
            // 
            this.l067.BackColor = System.Drawing.SystemColors.Info;
            this.l067.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l067.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l067.Location = new System.Drawing.Point(248, 118);
            this.l067.Name = "l067";
            this.l067.Size = new System.Drawing.Size(32, 15);
            this.l067.TabIndex = 336;
            this.l067.Text = "-";
            this.l067.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l037
            // 
            this.l037.BackColor = System.Drawing.SystemColors.Window;
            this.l037.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l037.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l037.Location = new System.Drawing.Point(248, 64);
            this.l037.Name = "l037";
            this.l037.Size = new System.Drawing.Size(32, 15);
            this.l037.TabIndex = 331;
            this.l037.Text = "-";
            this.l037.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l047
            // 
            this.l047.BackColor = System.Drawing.SystemColors.Window;
            this.l047.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l047.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l047.Location = new System.Drawing.Point(248, 80);
            this.l047.Name = "l047";
            this.l047.Size = new System.Drawing.Size(32, 15);
            this.l047.TabIndex = 330;
            this.l047.Text = "-";
            this.l047.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l146
            // 
            this.l146.BackColor = System.Drawing.SystemColors.Info;
            this.l146.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l146.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l146.Location = new System.Drawing.Point(208, 258);
            this.l146.Name = "l146";
            this.l146.Size = new System.Drawing.Size(32, 15);
            this.l146.TabIndex = 327;
            this.l146.Text = "-";
            this.l146.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l116
            // 
            this.l116.BackColor = System.Drawing.SystemColors.Window;
            this.l116.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l116.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l116.Location = new System.Drawing.Point(208, 204);
            this.l116.Name = "l116";
            this.l116.Size = new System.Drawing.Size(32, 15);
            this.l116.TabIndex = 325;
            this.l116.Text = "-";
            this.l116.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l126
            // 
            this.l126.BackColor = System.Drawing.SystemColors.Info;
            this.l126.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l126.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l126.Location = new System.Drawing.Point(208, 226);
            this.l126.Name = "l126";
            this.l126.Size = new System.Drawing.Size(32, 15);
            this.l126.TabIndex = 324;
            this.l126.Text = "-";
            this.l126.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l036
            // 
            this.l036.BackColor = System.Drawing.SystemColors.Window;
            this.l036.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l036.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l036.Location = new System.Drawing.Point(208, 64);
            this.l036.Name = "l036";
            this.l036.Size = new System.Drawing.Size(32, 15);
            this.l036.TabIndex = 317;
            this.l036.Text = "-";
            this.l036.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l046
            // 
            this.l046.BackColor = System.Drawing.SystemColors.Window;
            this.l046.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l046.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l046.Location = new System.Drawing.Point(208, 80);
            this.l046.Name = "l046";
            this.l046.Size = new System.Drawing.Size(32, 15);
            this.l046.TabIndex = 316;
            this.l046.Text = "-";
            this.l046.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l145
            // 
            this.l145.BackColor = System.Drawing.SystemColors.Info;
            this.l145.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l145.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l145.Location = new System.Drawing.Point(168, 258);
            this.l145.Name = "l145";
            this.l145.Size = new System.Drawing.Size(32, 15);
            this.l145.TabIndex = 313;
            this.l145.Text = "-";
            this.l145.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l125
            // 
            this.l125.BackColor = System.Drawing.SystemColors.Info;
            this.l125.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l125.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l125.Location = new System.Drawing.Point(168, 226);
            this.l125.Name = "l125";
            this.l125.Size = new System.Drawing.Size(32, 15);
            this.l125.TabIndex = 310;
            this.l125.Text = "-";
            this.l125.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l035
            // 
            this.l035.BackColor = System.Drawing.SystemColors.Window;
            this.l035.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l035.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l035.Location = new System.Drawing.Point(168, 64);
            this.l035.Name = "l035";
            this.l035.Size = new System.Drawing.Size(32, 15);
            this.l035.TabIndex = 303;
            this.l035.Text = "-";
            this.l035.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l045
            // 
            this.l045.BackColor = System.Drawing.SystemColors.Window;
            this.l045.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l045.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l045.Location = new System.Drawing.Point(168, 80);
            this.l045.Name = "l045";
            this.l045.Size = new System.Drawing.Size(32, 15);
            this.l045.TabIndex = 302;
            this.l045.Text = "-";
            this.l045.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l144
            // 
            this.l144.BackColor = System.Drawing.SystemColors.Info;
            this.l144.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l144.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l144.Location = new System.Drawing.Point(128, 258);
            this.l144.Name = "l144";
            this.l144.Size = new System.Drawing.Size(32, 15);
            this.l144.TabIndex = 299;
            this.l144.Text = "-";
            this.l144.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l124
            // 
            this.l124.BackColor = System.Drawing.SystemColors.Info;
            this.l124.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l124.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l124.Location = new System.Drawing.Point(128, 226);
            this.l124.Name = "l124";
            this.l124.Size = new System.Drawing.Size(32, 15);
            this.l124.TabIndex = 296;
            this.l124.Text = "-";
            this.l124.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l034
            // 
            this.l034.BackColor = System.Drawing.SystemColors.Window;
            this.l034.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l034.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l034.Location = new System.Drawing.Point(128, 64);
            this.l034.Name = "l034";
            this.l034.Size = new System.Drawing.Size(32, 15);
            this.l034.TabIndex = 289;
            this.l034.Text = "-";
            this.l034.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l044
            // 
            this.l044.BackColor = System.Drawing.SystemColors.Window;
            this.l044.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l044.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l044.Location = new System.Drawing.Point(128, 80);
            this.l044.Name = "l044";
            this.l044.Size = new System.Drawing.Size(32, 15);
            this.l044.TabIndex = 288;
            this.l044.Text = "-";
            this.l044.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l113
            // 
            this.l113.BackColor = System.Drawing.SystemColors.Window;
            this.l113.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l113.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l113.Location = new System.Drawing.Point(88, 204);
            this.l113.Name = "l113";
            this.l113.Size = new System.Drawing.Size(32, 15);
            this.l113.TabIndex = 283;
            this.l113.Text = "-";
            this.l113.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l123
            // 
            this.l123.BackColor = System.Drawing.SystemColors.Info;
            this.l123.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l123.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l123.Location = new System.Drawing.Point(88, 226);
            this.l123.Name = "l123";
            this.l123.Size = new System.Drawing.Size(32, 15);
            this.l123.TabIndex = 282;
            this.l123.Text = "-";
            this.l123.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l043
            // 
            this.l043.BackColor = System.Drawing.SystemColors.Window;
            this.l043.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l043.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l043.Location = new System.Drawing.Point(88, 80);
            this.l043.Name = "l043";
            this.l043.Size = new System.Drawing.Size(32, 15);
            this.l043.TabIndex = 274;
            this.l043.Text = "-";
            this.l043.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l112
            // 
            this.l112.BackColor = System.Drawing.SystemColors.Window;
            this.l112.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l112.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l112.Location = new System.Drawing.Point(48, 204);
            this.l112.Name = "l112";
            this.l112.Size = new System.Drawing.Size(32, 15);
            this.l112.TabIndex = 269;
            this.l112.Text = "-";
            this.l112.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l122
            // 
            this.l122.BackColor = System.Drawing.SystemColors.Info;
            this.l122.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l122.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l122.Location = new System.Drawing.Point(48, 226);
            this.l122.Name = "l122";
            this.l122.Size = new System.Drawing.Size(32, 15);
            this.l122.TabIndex = 268;
            this.l122.Text = "-";
            this.l122.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l042
            // 
            this.l042.BackColor = System.Drawing.SystemColors.Window;
            this.l042.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l042.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l042.Location = new System.Drawing.Point(48, 80);
            this.l042.Name = "l042";
            this.l042.Size = new System.Drawing.Size(32, 15);
            this.l042.TabIndex = 260;
            this.l042.Text = "-";
            this.l042.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l111
            // 
            this.l111.BackColor = System.Drawing.SystemColors.Window;
            this.l111.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l111.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l111.Location = new System.Drawing.Point(8, 204);
            this.l111.Name = "l111";
            this.l111.Size = new System.Drawing.Size(32, 15);
            this.l111.TabIndex = 255;
            this.l111.Text = "-";
            this.l111.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l031
            // 
            this.l031.BackColor = System.Drawing.SystemColors.Window;
            this.l031.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.l031.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l031.Location = new System.Drawing.Point(8, 64);
            this.l031.Name = "l031";
            this.l031.Size = new System.Drawing.Size(32, 15);
            this.l031.TabIndex = 247;
            this.l031.Text = "-";
            this.l031.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.Bisque;
            this.groupBox3.Controls.Add(this.ltColR);
            this.groupBox3.Controls.Add(this.lFGR);
            this.groupBox3.Controls.Add(this.bFG);
            this.groupBox3.Controls.Add(this.lbCGR);
            this.groupBox3.Controls.Add(this.bMenosR);
            this.groupBox3.Controls.Add(this.bMasR);
            this.groupBox3.Controls.Add(this.bAnalizar);
            this.groupBox3.Controls.Add(this.button3);
            this.groupBox3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox3.Location = new System.Drawing.Point(480, 299);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(264, 111);
            this.groupBox3.TabIndex = 360;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Análisis Resultados";
            // 
            // bFG
            // 
            this.bFG.BackColor = System.Drawing.Color.DarkSalmon;
            this.bFG.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bFG.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bFG.ForeColor = System.Drawing.Color.Black;
            this.bFG.Image = ((System.Drawing.Image)(resources.GetObject("bFG.Image")));
            this.bFG.Location = new System.Drawing.Point(8, 22);
            this.bFG.Name = "bFG";
            this.bFG.Size = new System.Drawing.Size(24, 23);
            this.bFG.TabIndex = 87;
            this.bFG.UseVisualStyleBackColor = false;
            this.bFG.Click += new System.EventHandler(this.BFGClick);
            // 
            // bMenosR
            // 
            this.bMenosR.BackColor = System.Drawing.Color.DarkSalmon;
            this.bMenosR.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bMenosR.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bMenosR.ForeColor = System.Drawing.Color.Black;
            this.bMenosR.Location = new System.Drawing.Point(232, 38);
            this.bMenosR.Name = "bMenosR";
            this.bMenosR.Size = new System.Drawing.Size(16, 15);
            this.bMenosR.TabIndex = 85;
            this.bMenosR.Text = "-";
            this.bMenosR.UseVisualStyleBackColor = false;
            this.bMenosR.Click += new System.EventHandler(this.BMenosRClick);
            // 
            // bExporCols
            // 
            this.bExporCols.BackColor = System.Drawing.Color.DarkSalmon;
            this.bExporCols.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bExporCols.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bExporCols.Image = ((System.Drawing.Image)(resources.GetObject("bExporCols.Image")));
            this.bExporCols.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bExporCols.Location = new System.Drawing.Point(480, 118);
            this.bExporCols.Name = "bExporCols";
            this.bExporCols.Size = new System.Drawing.Size(138, 32);
            this.bExporCols.TabIndex = 367;
            this.bExporCols.Text = "Exportar columnas";
            this.bExporCols.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bExporCols.UseVisualStyleBackColor = false;
            this.bExporCols.Click += new System.EventHandler(this.BExporColsClick);
            // 
            // valors1
            // 
            this.valors1.BackColor = System.Drawing.Color.Bisque;
            this.valors1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.valors1.Location = new System.Drawing.Point(8, 0);
            this.valors1.Name = "valors1";
            this.valors1.Size = new System.Drawing.Size(168, 492);
            this.valors1.TabIndex = 361;
            this.valors1.CrearCPs += new System.EventHandler(this.valors1CreaCPs);
            // 
            // GeneraPim
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(752, 516);
            this.Controls.Add(this.lrangos);
            this.Controls.Add(this.bExporCols);
            this.Controls.Add(this.lfile);
            this.Controls.Add(this.valors1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.bLeerConds);
            this.Controls.Add(this.bSalvarConds);
            this.Controls.Add(this.bCancelar);
            this.Controls.Add(this.bCalcular);
            this.Controls.Add(this.lTime);
            this.Controls.Add(this.lColsIni);
            this.Controls.Add(this.lColsAdm);
            this.Controls.Add(this.bGrabaCols);
            this.Controls.Add(this.groupBox2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GeneraPim";
            this.Text = "Filtro Pim";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

		}

		void BGrabaColsClick(object sender, System.EventArgs e) { GrabaCols(); }
		void elmeuTimer(object sender, System.EventArgs e) { veureelmeu(); }
		void BCancelarClick(object sender, System.EventArgs e) { salida=true; }
		void BCalcularClick(object sender, System.EventArgs e) { Calcular(); }
		void BSalvarCondsClick(object sender, System.EventArgs e) { SalvarConds(); }
		void BLeerCondsClick(object sender, System.EventArgs e) { LeerConds(); }
		void BAnalizarClick(object sender, System.EventArgs e) { Analizar(); }
		void BFGClick(object sender, System.EventArgs e) { EntraCGsR(); }
		void BMasRClick(object sender, System.EventArgs e) { GRMas(); }
		void BMenosRClick(object sender, System.EventArgs e) { GRMenos(); }	
		void valors1CreaCPs(object sender, System.EventArgs e) {
			RecuperaPantalla(); PintaPantalla();
		}
		void BExporColsClick(object sender, System.EventArgs e) { ExporCols(); }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }		
	}			
}
