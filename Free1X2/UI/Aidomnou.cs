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
	class aidomnou : Form 	{
		private Label l031;
		private Label l036;
		private Button bFG;
		private Label l034;
		private Label l035;
		private TextBox tbmgreco;
		private Label l065;
		private Label l113;
		private Label l112;
		private Label l062;
		private Label l061;
		private Label l116;
		private Label lrangos;
		private Label lrk2;
		private Label l084;
		private Label lrk4;
		private Label lrk5;
		private Label lrk6;
		private Label l045;
		private Label l044;
		private Label l046;
		private Label l041;
		private Label l123;
		private Label l043;
		private Label l121;
		private Label l126;
		private Button bExporCols;
		private Label l124;
		private Label l125;
		private Label l144;
		private Label l145;
		private GroupBox groupBox3;
		private GroupBox groupBox2;
		private Label l071;
		private Label l092;
		private Label l095;
		private Button bMenosR;
		private Label l054;
		private Label l055;
		private Label l056;
		private Label l111;
		private Label l051;
		private Label l052;
		private Button bLeeCondis;
		private Label l115;
		private Label l114;
		private Label label2;
		private Label l133;
		private Label l132;
		private Label label4;
		private Label l064;
		private Button bAnalizar;
		private Button bGrabaCols;
		private Label l014;
		private TextBox tbmg1;
		private Label l066;
		private TextBox tbmg2;
		private TextBox tbmg5;
		private TextBox tbmg4;
		private Label lfile;
		private Label l146;
		private TextBox tbColR;
		private Label l141;
		private Label l142;
		private Label l143;
		private Button bMasR;
		private Label l081;
		private Label l083;
		private Label l082;
		private Label l085;
		private Label l026;
		private Label l025;
		private Label l086;
		private TextBox tbmg6;
		private Label l076;
		private Label l074;
		private Label l075;
		private Label l072;
		private Label l073;
		private Label l131;
		private Label lColsIni;
		private Label l136;
		private Label l135;
		private Label l134;
		private Label lrk1;
		private Label lFGR;
		private Label lrk3;
		private TextBox tbmg3;
		private Label l091;
		private Label lColsAdm;
		private Label l093;
		private Label l094;
		private Label l103;
		private Label l096;
		private Label l122;
		private Label l063;
		private Label lNota;
		private Controls.valors valors1;
		private Label l011;
		private Label l012;
		private Label l013;
		private Label lTime;
		private Label l015;
		private Label l016;
		private Button bCalcular;
		private Label lsuma;
		private Label l042;
		private TextBox tbmgsuma;
		private Label l023;
		private Label l022;
		private Label l021;
		private Label l101;
		private Label l102;
		private Label l024;
		private Label l104;
		private Label l105;
		private Label l106;
		private Button bSalvaCondis;
		private Label l053;
		private Button bCancelar;
		private Label lbCGR;
		private Label lreco;
		private Label l032;
		private Label l033;
		public aidomnou() {
			InitializeComponent();
			elmeu = new Timer();
			elmeu.Interval = 3000;
			elmeu.Tick += new EventHandler(elmeuTimer);
			if (!GetConfig()) SetConfig();
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
        }

		
		private double[,] nvals = new double[14,3];
		private int[] lims = {145,11,9,8,65,9,6,5};
		private int[,] rks = new int[8,2];
		private string[] scps = new string[6];
		private int[,] cps = new int[14,6];
		private DateTime time0, time9;
		private Timer elmeu;
		private bool salida = false;
		private string tmp;
		private int ctini, ctadm;
		private BitArray validas = new BitArray(4782969);
		private int limcgsR, nrfCGR;
		private string[] colgsR = new string[3000];
		
		private bool GetConfig() {
		    string pat = Application.StartupPath+"/aidomnou.cfg";
			StreamReader sr;
			try {
				sr = new StreamReader(pat);
			}
			catch { return false; }
			sr.ReadLine();
			sr.ReadLine();
			sr.ReadLine();
			sr.ReadLine();
			string tmp = sr.ReadLine(); tmp = tmp.Substring(0,(tmp.IndexOf("//")));
			lims[0] = Convert.ToInt32(tmp);
			tmp = sr.ReadLine(); tmp = tmp.Substring(0,(tmp.IndexOf("//")));
			string[] aux = tmp.Split(',');
			lims[1] = Convert.ToInt32(aux[0]);
			lims[2] = Convert.ToInt32(aux[1]);
			lims[3] = Convert.ToInt32(aux[2]);
			sr.ReadLine();
			tmp = sr.ReadLine(); tmp = tmp.Substring(0,(tmp.IndexOf("//")));
			lims[4] = Convert.ToInt32(tmp);
			tmp = sr.ReadLine(); tmp = tmp.Substring(0,(tmp.IndexOf("//")));
			aux = tmp.Split(',');
			lims[5] = Convert.ToInt32(aux[0]);
			lims[6] = Convert.ToInt32(aux[1]);
			lims[7] = Convert.ToInt32(aux[2]);
			sr.Close();
			return false;
		}
		private void SetConfig() {
			char sp = ',';
		    string pat = Application.StartupPath+"/aidomnou.cfg";
			StreamWriter sw = new StreamWriter(pat);
			sw.WriteLine("en caso de edición, no retirar los comentarios de las lineas");
			sw.WriteLine();
			sw.WriteLine("parámetros para creación de 6 columnas");
			sw.WriteLine("   // en todos los casos se escogen las casillas más valoradas");
			sw.WriteLine(lims[0]+"   // columna-1 = 10 puntos para doble y 15 para triple");
			sw.WriteLine(""+lims[1]+sp+lims[2]+sp+lims[3]+"   // columna-2 = cantidad de unos, equis y doses");
			sw.WriteLine("   // columna-3 = 14 dobles");
			sw.WriteLine(lims[4]+"   // columna-4 = 10 puntos para doble y 15 para triple");
			sw.WriteLine(""+lims[5]+sp+lims[6]+sp+lims[7]+"   // columna-5 = cantidad de unos, equis y doses");
			sw.WriteLine("   // columna-6 = 14 fijos");
			sw.Close();
		}
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
			string[] aux = null;
			nvals = valors1.RetVals();
			aux = tbmg1.Text.Split('-'); rks[0,0]=Convert.ToInt32(aux[0]); rks[0,1]=Convert.ToInt32(aux[1]);
			aux = tbmg2.Text.Split('-'); rks[1,0]=Convert.ToInt32(aux[0]); rks[1,1]=Convert.ToInt32(aux[1]);
			aux = tbmg3.Text.Split('-'); rks[2,0]=Convert.ToInt32(aux[0]); rks[2,1]=Convert.ToInt32(aux[1]);
			aux = tbmg4.Text.Split('-'); rks[3,0]=Convert.ToInt32(aux[0]); rks[3,1]=Convert.ToInt32(aux[1]);
			aux = tbmg5.Text.Split('-'); rks[4,0]=Convert.ToInt32(aux[0]); rks[4,1]=Convert.ToInt32(aux[1]);
			aux = tbmg6.Text.Split('-'); rks[5,0]=Convert.ToInt32(aux[0]); rks[5,1]=Convert.ToInt32(aux[1]);
			aux = tbmgsuma.Text.Split('-'); rks[6,0]=Convert.ToInt32(aux[0]); rks[6,1]=Convert.ToInt32(aux[1]);
			aux = tbmgreco.Text.Split('-'); rks[7,0]=Convert.ToInt32(aux[0]); rks[7,1]=Convert.ToInt32(aux[1]);
		}
		private void PreparaColumnas() {
			string tmp;
			for (int nr=0; nr<14; nr++) for (int nc=0; nc<6; nc++) cps[nr,nc]=0;
			PreCol1(); PreCol2();
			PreCol3(); PreCol4();
			PreCol5(); PreCol6();
			for (int nr1=0; nr1<6; nr1++) {
				tmp="";
				for (int nr2=0; nr2<14; nr2++) tmp+=cps[nr2,nr1];
				scps[nr1]=tmp;
			}
		}
		private void PreCol1() {
			int nq1, nq2; double nv;
			double[,] pvals = new double[14,3];
			for (int nr1=0; nr1<14; nr1++)
				for (int nr2=0; nr2<3; nr2++) pvals[nr1,nr2]=nvals[nr1,nr2];
			for (int nt=0; nt<42; nt++) {
				nq1=nq2=0; nv=0;
				for (int nr1=0; nr1<14; nr1++) {
					for (int nr2=0; nr2<3; nr2++) {
						if (pvals[nr1,nr2]>=nv) {
							nv=pvals[nr1,nr2]; nq1=nr1; nq2=nr2;
						}
					}
				}
				cps[nq1,0]+=(nq2==0?1:nq2==1?4:2);
				pvals[nq1,nq2]=(-1);
				nv=0;
				for (int nr=0; nr<14; nr++) {
					switch (cps[nr,0]) {
						case 3:
						case 5:
							case 6: nv+=10; break;
							case 7: nv+=15; break;
					}
				}
				if (nv>=lims[0]) break;
			}
		}
		private void PreCol4() {
			int nq1, nq2; double nv;
			double[,] pvals = new double[14,3];
			for (int nr1=0; nr1<14; nr1++)
				for (int nr2=0; nr2<3; nr2++) pvals[nr1,nr2]=nvals[nr1,nr2];
			while (true) {
				nq1=nq2=0; nv=0;
				for (int nr1=0; nr1<14; nr1++) {
					for (int nr2=0; nr2<3; nr2++) {
						if (pvals[nr1,nr2]>=nv) {
							nv=pvals[nr1,nr2]; nq1=nr1; nq2=nr2;
						}
					}
				}
				cps[nq1,3]+=(nq2==0?1:nq2==1?4:2);
				pvals[nq1,nq2]=(-1);
				nv=0;
				for (int nr=0; nr<14; nr++) {
					switch (cps[nr,3]) {
						case 3:
						case 5:
							case 6: nv+=10; break;
							case 7: nv+=15; break;
					}
				}
				if (nv>=lims[4]) break;
			}
		}
		private void PreCol2() {
			int nq; double nv;
			for (int nr1=0; nr1<lims[1]; nr1++) {
				nq=0; nv=0;
				for (int nr2=0; nr2<14; nr2++) {
					if (cps[nr2,1]<1 && nvals[nr2,0]>=nv) {
						nv=nvals[nr2,0]; nq=nr2;
					}
				}
				cps[nq,1]+=1;
			}
			for (int nr1=0; nr1<lims[3]; nr1++) {
				nq=0; nv=0;
				for (int nr2=0; nr2<14; nr2++) {
					if (cps[nr2,1]<2 && nvals[nr2,2]>nv) {
						nv=nvals[nr2,2]; nq=nr2;
					}
				}
				cps[nq,1]+=2;
			}
			for (int nr1=0; nr1<lims[2]; nr1++) {
				nq=0; nv=0;
				for (int nr2=0; nr2<14; nr2++) {
					if (cps[nr2,1]<4 && nvals[nr2,1]>nv) {
						nv=nvals[nr2,1]; nq=nr2;
					}
				}
				cps[nq,1]+=4;
			}
		}
		private void PreCol5() {
			int nq; double nv;
			for (int nr1=0; nr1<lims[5]; nr1++) {
				nq=0; nv=0;
				for (int nr2=0; nr2<14; nr2++) {
					if (cps[nr2,4]<1 && nvals[nr2,0]>=nv) {
						nv=nvals[nr2,0]; nq=nr2;
					}
				}
				cps[nq,4]+=1;
			}
			for (int nr1=0; nr1<lims[7]; nr1++) {
				nq=0; nv=0;
				for (int nr2=0; nr2<14; nr2++) {
					if (cps[nr2,4]<2 && nvals[nr2,2]>nv) {
						nv=nvals[nr2,2]; nq=nr2;
					}
				}
				cps[nq,4]+=2;
			}
			for (int nr1=0; nr1<lims[6]; nr1++) {
				nq=0; nv=0;
				for (int nr2=0; nr2<14; nr2++) {
					if (cps[nr2,4]<4 && nvals[nr2,1]>nv) {
						nv=nvals[nr2,1]; nq=nr2;
					}
				}
				cps[nq,4]+=4;
			}
		}
		private void PreCol6() {           // 14 fijos
			for (int nr1=0; nr1<14; nr1++) {
				int nq=0; double nv=0;
				for (int nr2=0; nr2<3; nr2++) {
					if (nvals[nr1,nr2]>nv) { nv=nvals[nr1,nr2]; nq=nr2; }
				}
				cps[nr1,5]=(nq==0?1:(nq==1?4:2));
			}
		}
		private void PreCol3() {             // 14 dobles
			for (int nr1=0; nr1<14; nr1++) {
				int nq=0; double nv=100;
				for (int nr2=0; nr2<3; nr2++) {
					if (nvals[nr1,nr2]<nv) { nv=nvals[nr1,nr2]; nq=nr2; }
				}
				cps[nr1,2]=(nq==0?6:(nq==1?3:5));
			}
		}
		private void PintaPantalla() {
			int nv, nq = 0;
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
			
			nv = cps[0,1]; if (nv>0) nq++; l012.Text=Cambia(nv);
			nv = cps[1,1]; if (nv>0) nq++; l022.Text=Cambia(nv);
			nv = cps[2,1]; if (nv>0) nq++; l032.Text=Cambia(nv);
			nv = cps[3,1]; if (nv>0) nq++; l042.Text=Cambia(nv);
			nv = cps[4,1]; if (nv>0) nq++; l052.Text=Cambia(nv);
			nv = cps[5,1]; if (nv>0) nq++; l062.Text=Cambia(nv);
			nv = cps[6,1]; if (nv>0) nq++; l072.Text=Cambia(nv);
			nv = cps[7,1]; if (nv>0) nq++; l082.Text=Cambia(nv);
			nv = cps[8,1]; if (nv>0) nq++; l092.Text=Cambia(nv);
			nv = cps[9,1]; if (nv>0) nq++; l102.Text=Cambia(nv);
			nv = cps[10,1]; if (nv>0) nq++; l112.Text=Cambia(nv);
			nv = cps[11,1]; if (nv>0) nq++; l122.Text=Cambia(nv);
			nv = cps[12,1]; if (nv>0) nq++; l132.Text=Cambia(nv);
			nv = cps[13,1]; if (nv>0) nq++; l142.Text=Cambia(nv);
			
			nv = cps[0,2]; if (nv>0) nq++; l013.Text=Cambia(nv);
			nv = cps[1,2]; if (nv>0) nq++; l023.Text=Cambia(nv);
			nv = cps[2,2]; if (nv>0) nq++; l033.Text=Cambia(nv);
			nv = cps[3,2]; if (nv>0) nq++; l043.Text=Cambia(nv);
			nv = cps[4,2]; if (nv>0) nq++; l053.Text=Cambia(nv);
			nv = cps[5,2]; if (nv>0) nq++; l063.Text=Cambia(nv);
			nv = cps[6,2]; if (nv>0) nq++; l073.Text=Cambia(nv);
			nv = cps[7,2]; if (nv>0) nq++; l083.Text=Cambia(nv);
			nv = cps[8,2]; if (nv>0) nq++; l093.Text=Cambia(nv);
			nv = cps[9,2]; if (nv>0) nq++; l103.Text=Cambia(nv);
			nv = cps[10,2]; if (nv>0) nq++; l113.Text=Cambia(nv);
			nv = cps[11,2]; if (nv>0) nq++; l123.Text=Cambia(nv);
			nv = cps[12,2]; if (nv>0) nq++; l133.Text=Cambia(nv);
			nv = cps[13,2]; if (nv>0) nq++; l143.Text=Cambia(nv);
			
			nv = cps[0,3]; if (nv>0) nq++; l014.Text=Cambia(nv);
			nv = cps[1,3]; if (nv>0) nq++; l024.Text=Cambia(nv);
			nv = cps[2,3]; if (nv>0) nq++; l034.Text=Cambia(nv);
			nv = cps[3,3]; if (nv>0) nq++; l044.Text=Cambia(nv);
			nv = cps[4,3]; if (nv>0) nq++; l054.Text=Cambia(nv);
			nv = cps[5,3]; if (nv>0) nq++; l064.Text=Cambia(nv);
			nv = cps[6,3]; if (nv>0) nq++; l074.Text=Cambia(nv);
			nv = cps[7,3]; if (nv>0) nq++; l084.Text=Cambia(nv);
			nv = cps[8,3]; if (nv>0) nq++; l094.Text=Cambia(nv);
			nv = cps[9,3]; if (nv>0) nq++; l104.Text=Cambia(nv);
			nv = cps[10,3]; if (nv>0) nq++; l114.Text=Cambia(nv);
			nv = cps[11,3]; if (nv>0) nq++; l124.Text=Cambia(nv);
			nv = cps[12,3]; if (nv>0) nq++; l134.Text=Cambia(nv);
			nv = cps[13,3]; if (nv>0) nq++; l144.Text=Cambia(nv);
			
			nv = cps[0,4]; if (nv>0) nq++; l015.Text=Cambia(nv);
			nv = cps[1,4]; if (nv>0) nq++; l025.Text=Cambia(nv);
			nv = cps[2,4]; if (nv>0) nq++; l035.Text=Cambia(nv);
			nv = cps[3,4]; if (nv>0) nq++; l045.Text=Cambia(nv);
			nv = cps[4,4]; if (nv>0) nq++; l055.Text=Cambia(nv);
			nv = cps[5,4]; if (nv>0) nq++; l065.Text=Cambia(nv);
			nv = cps[6,4]; if (nv>0) nq++; l075.Text=Cambia(nv);
			nv = cps[7,4]; if (nv>0) nq++; l085.Text=Cambia(nv);
			nv = cps[8,4]; if (nv>0) nq++; l095.Text=Cambia(nv);
			nv = cps[9,4]; if (nv>0) nq++; l105.Text=Cambia(nv);
			nv = cps[10,4]; if (nv>0) nq++; l115.Text=Cambia(nv);
			nv = cps[11,4]; if (nv>0) nq++; l125.Text=Cambia(nv);
			nv = cps[12,4]; if (nv>0) nq++; l135.Text=Cambia(nv);
			nv = cps[13,4]; if (nv>0) nq++; l145.Text=Cambia(nv);
			
			nv = cps[0,5]; if (nv>0) nq++; l016.Text=Cambia(nv);
			nv = cps[1,5]; if (nv>0) nq++; l026.Text=Cambia(nv);
			nv = cps[2,5]; if (nv>0) nq++; l036.Text=Cambia(nv);
			nv = cps[3,5]; if (nv>0) nq++; l046.Text=Cambia(nv);
			nv = cps[4,5]; if (nv>0) nq++; l056.Text=Cambia(nv);
			nv = cps[5,5]; if (nv>0) nq++; l066.Text=Cambia(nv);
			nv = cps[6,5]; if (nv>0) nq++; l076.Text=Cambia(nv);
			nv = cps[7,5]; if (nv>0) nq++; l086.Text=Cambia(nv);
			nv = cps[8,5]; if (nv>0) nq++; l096.Text=Cambia(nv);
			nv = cps[9,5]; if (nv>0) nq++; l106.Text=Cambia(nv);
			nv = cps[10,5]; if (nv>0) nq++; l116.Text=Cambia(nv);
			nv = cps[11,5]; if (nv>0) nq++; l126.Text=Cambia(nv);
			nv = cps[12,5]; if (nv>0) nq++; l136.Text=Cambia(nv);
			nv = cps[13,5]; if (nv>0) nq++; l146.Text=Cambia(nv);
			
			tbmg1.Text = ""+rks[0,0]+"-"+rks[0,1];
			tbmg2.Text = ""+rks[1,0]+"-"+rks[1,1];
			tbmg3.Text = ""+rks[2,0]+"-"+rks[2,1];
			tbmg4.Text = ""+rks[3,0]+"-"+rks[3,1];
			tbmg5.Text = ""+rks[4,0]+"-"+rks[4,1];
			tbmg6.Text = ""+rks[5,0]+"-"+rks[5,1];
			tbmgsuma.Text = ""+rks[6,0]+"-"+rks[6,1];
			tbmgreco.Text = ""+rks[7,0]+"-"+rks[7,1];
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
			string fileout, tmp;
			bCalcular.Enabled = false;
			bGrabaCols.Enabled = false;
			bGrabaCols.Text = "grabando";
			SaveFileDialog resul = new SaveFileDialog();
            resul.InitialDirectory = Application.StartupPath + "/" ;
			resul.Filter = "Resultados(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
			if(resul.ShowDialog() == DialogResult.OK) {
				Application.DoEvents();
				tmp = resul.FileName;
				fileout = Path.GetFileName(tmp);
				StreamWriter wr = new StreamWriter(fileout);
				for (int nr=0; nr<4782969; nr++) {
					if (validas[nr]) {
						tmp = n2s(nr,14);
						wr.WriteLine(tmp);
					}
				}
				wr.Close();
				lfile.Text = fileout;
			}
			bGrabaCols.Text = "grabar resultado";
			bGrabaCols.Enabled = true;
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
			int idx;
			bCalcular.Enabled = false;
			bGrabaCols.Enabled = false;
			elmeu.Start(); time0 = DateTime.Now;
			salida = false;
			lColsIni.Text = lColsAdm.Text =  lTime.Text = " ";
			ctadm=ctini=0;
			Application.DoEvents();
			RecuperaPantalla();
			PreparaColumnas();
			PintaPantalla();
			OpenFileDialog lee = new OpenFileDialog();
            lee.InitialDirectory = Application.StartupPath + "/" ;
			lee.Filter = "Cols.Entrada(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
			if(lee.ShowDialog() == DialogResult.OK) {
				tmp = lee.FileName;
				string filein = Path.GetFileName(tmp);
				validas.SetAll(false); ctadm=0;
				StreamReader sr = new StreamReader(tmp);
				while (sr.Peek()>0) {
					if (salida) break;
					tmp = sr.ReadLine().Trim(); ctini++;
					if (tmp.Length < 14) {
						MessageBox.Show ("error de longitud="+tmp);
						break;
					}
					tmp = tmp.Replace('x','4');
					tmp = tmp.Replace('X','4');
					if (Valida(tmp)) {
						idx = s2n(tmp,14);
						if (validas[idx]==false) {
							validas[idx]=true;
							ctadm++;
						}
					}
					Application.DoEvents();
				}
			}
			elmeu.Stop(); veureelmeu();
			bCalcular.Enabled = true;
			bGrabaCols.Enabled = true;
		}
		private bool Valida(string columna) {
			int na, sum6, min6, max6, reco6; string temp;
			sum6=max6=0; min6=84;
			for (int nr=0; nr<6; nr++) {
				na=0;
				temp=scps[nr];
				for (int nr2=0; nr2<14; nr2++) {
					char chp = temp[nr2];
					if (chp==48) continue;
					int ch3 = chp & columna[nr2];
					if (ch3>48) na++;
				}
				if (na<rks[nr,0] || na>rks[nr,1]) return false;
				sum6+=na; if (na<min6) min6=na; if (na>max6) max6=na;
			}
			if (sum6<rks[6,0] || sum6>rks[6,1]) return false;
			reco6 = max6 - min6;
			if (reco6<rks[7,0] || reco6>rks[7,1]) return false;
			return true;
		}
		private void Analizar() {
			bAnalizar.Enabled = false;
			bCalcular.Enabled = false;
			bGrabaCols.Enabled = false;
			int na, min6, max6, sum6; int[] minmax = new int[6];
			string columna = tbColR.Text.Replace('x','4');
			columna = columna.Replace('X','4');
			min6=99; max6=sum6=0;
			RecuperaPantalla();
			PreparaColumnas();
			PintaPantalla();
			for (int nr=0; nr<6; nr++) {
				na=0;
				tmp=scps[nr];
				for (int nr2=0; nr2<14; nr2++) {
					char chp = tmp[nr2];
					if (chp==48) continue;
					int ch3 = chp & columna[nr2];
					if (ch3>48) na++;
				}
				minmax[nr]=na;
				if (na<min6) min6=na; if (na>max6) max6=na; sum6+=na;
			}
			lrk1.Text = ""+minmax[0];
			lrk2.Text = ""+minmax[1];
			lrk3.Text = ""+minmax[2];
			lrk4.Text = ""+minmax[3];
			lrk5.Text = ""+minmax[4];
			lrk6.Text = ""+minmax[5];
			lsuma.Text = ""+sum6;
			lreco.Text = ""+(max6-min6);
			bCalcular.Enabled = true;
			bGrabaCols.Enabled = true;
			bAnalizar.Enabled = true;
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
		private void ExporCols() {
			string fileout, tmp;
			SaveFileDialog svDialog = new SaveFileDialog();
            svDialog.InitialDirectory = Application.StartupPath + "/";
			svDialog.Filter = "F.Salida(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			if(svDialog.ShowDialog() == DialogResult.OK) {
				fileout = Path.GetFileName(svDialog.FileName);
				StreamWriter sw = new StreamWriter(fileout);
				for (int nr=0; nr<6; nr++) {
					tmp = Cambia(cps[0,nr]);
					for (int np=1; np<14; np++) {
						tmp += ","+Cambia(cps[np,nr]);
					}
					sw.WriteLine(tmp);
				}
				sw.Close();
			}
		}
		private void SalvaCondis() {
			SaveFileDialog svDialog = new SaveFileDialog();
            svDialog.InitialDirectory = Application.StartupPath + "/";
			svDialog.Filter = "Rangos(*.cnd)|*.cnd|Todos los archivos (*.*)|*.*" ;
			if(svDialog.ShowDialog() == DialogResult.OK) {
				string fileout = Path.GetFileName(svDialog.FileName);
				StreamWriter sw = new StreamWriter(fileout);
				sw.WriteLine(tbmg1.Text);
				sw.WriteLine(tbmg2.Text);
				sw.WriteLine(tbmg3.Text);
				sw.WriteLine(tbmg4.Text);
				sw.WriteLine(tbmg5.Text);
				sw.WriteLine(tbmg6.Text);
				sw.WriteLine(tbmgsuma.Text);
				sw.WriteLine(tbmgreco.Text);
				sw.Close();
				lrangos.Text = fileout;
			}
		}
		private void LeeCondis() {
			OpenFileDialog abreValIn = new OpenFileDialog();
            abreValIn.InitialDirectory = Application.StartupPath + "/";
			abreValIn.Filter = "Condiciones(*.cnd)|*.cnd|Todos los archivos(*.*)|*.*";
			if(abreValIn.ShowDialog() == DialogResult.OK) {
				string filein = Path.GetFileName(abreValIn.FileName);
                StreamReader srv = new StreamReader(abreValIn.FileName);
				tbmg1.Text = srv.ReadLine();
				tbmg2.Text = srv.ReadLine();
				tbmg3.Text = srv.ReadLine();
				tbmg4.Text = srv.ReadLine();
				tbmg5.Text = srv.ReadLine();
				tbmg6.Text = srv.ReadLine();
				tbmgsuma.Text = srv.ReadLine();
				tbmgreco.Text = srv.ReadLine();
				srv.Close();
				lrangos.Text = filein;
			}
		}
		private int s2n(string ax, int lim) {
			int nx = 0;
			for (int nr=0; nr<lim; nr++) {
				nx *= 3;
				string ch = ax.Substring(nr,1);
				switch (ch)
				{
				    case "1":
				        nx+=1;
				        break;
				    case "2":
				        nx+=2;
				        break;
				}
			}
			return nx;
		}
		private string n2s(int nx, int lim) {
			string ax = ""; int nx2;
			for (int nr=0; nr<lim; nr++) {
				nx2 = nx%3; nx /= 3;
				switch (nx2)
				{
				    case 1:
				        ax = "1"+ax;
				        break;
				    case 2:
				        ax = "2"+ax;
				        break;
				    default:
				        ax = "X"+ax;
				        break;
				}
			}
			return ax;
		}
		

		void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(aidomnou));
            l033 = new Label();
            l032 = new Label();
            lreco = new Label();
            lbCGR = new Label();
            bCancelar = new Button();
            l053 = new Label();
            bSalvaCondis = new Button();
            l106 = new Label();
            l105 = new Label();
            l104 = new Label();
            l024 = new Label();
            l102 = new Label();
            l101 = new Label();
            l021 = new Label();
            l022 = new Label();
            l023 = new Label();
            tbmgsuma = new TextBox();
            l042 = new Label();
            lsuma = new Label();
            bCalcular = new Button();
            l016 = new Label();
            l015 = new Label();
            lTime = new Label();
            l013 = new Label();
            l012 = new Label();
            l011 = new Label();
            valors1 = new Free1X2.UI.Controls.valors();
            lNota = new Label();
            l063 = new Label();
            l122 = new Label();
            l096 = new Label();
            l103 = new Label();
            l094 = new Label();
            l093 = new Label();
            lColsAdm = new Label();
            l091 = new Label();
            tbmg3 = new TextBox();
            lrk3 = new Label();
            lFGR = new Label();
            lrk1 = new Label();
            l134 = new Label();
            l135 = new Label();
            l136 = new Label();
            lColsIni = new Label();
            l131 = new Label();
            l073 = new Label();
            l072 = new Label();
            l075 = new Label();
            l074 = new Label();
            l076 = new Label();
            tbmg6 = new TextBox();
            l086 = new Label();
            l025 = new Label();
            l026 = new Label();
            l085 = new Label();
            l082 = new Label();
            l083 = new Label();
            l081 = new Label();
            bMasR = new Button();
            l143 = new Label();
            l142 = new Label();
            l141 = new Label();
            tbColR = new TextBox();
            l146 = new Label();
            lfile = new Label();
            tbmg4 = new TextBox();
            tbmg5 = new TextBox();
            tbmg2 = new TextBox();
            l066 = new Label();
            tbmg1 = new TextBox();
            l014 = new Label();
            bGrabaCols = new Button();
            bAnalizar = new Button();
            l064 = new Label();
            label4 = new Label();
            l132 = new Label();
            l133 = new Label();
            label2 = new Label();
            l114 = new Label();
            l115 = new Label();
            bLeeCondis = new Button();
            l052 = new Label();
            l051 = new Label();
            l111 = new Label();
            l056 = new Label();
            l055 = new Label();
            l054 = new Label();
            bMenosR = new Button();
            l095 = new Label();
            l092 = new Label();
            l071 = new Label();
            groupBox2 = new GroupBox();
            lrk6 = new Label();
            lrk5 = new Label();
            lrk4 = new Label();
            lrk2 = new Label();
            l116 = new Label();
            l126 = new Label();
            l036 = new Label();
            l046 = new Label();
            l145 = new Label();
            l125 = new Label();
            l065 = new Label();
            l035 = new Label();
            l045 = new Label();
            l144 = new Label();
            l124 = new Label();
            l084 = new Label();
            l034 = new Label();
            l044 = new Label();
            l113 = new Label();
            l123 = new Label();
            l043 = new Label();
            l112 = new Label();
            l062 = new Label();
            l121 = new Label();
            l061 = new Label();
            l031 = new Label();
            l041 = new Label();
            tbmgreco = new TextBox();
            groupBox3 = new GroupBox();
            bFG = new Button();
            bExporCols = new Button();
            lrangos = new Label();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // l033
            // 
            l033.BackColor = System.Drawing.SystemColors.Window;
            l033.BorderStyle = BorderStyle.FixedSingle;
            l033.ForeColor = System.Drawing.SystemColors.ControlText;
            l033.Location = new System.Drawing.Point(112, 65);
            l033.Name = "l033";
            l033.Size = new System.Drawing.Size(32, 15);
            l033.TabIndex = 275;
            l033.Text = "-";
            l033.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l032
            // 
            l032.BackColor = System.Drawing.SystemColors.Window;
            l032.BorderStyle = BorderStyle.FixedSingle;
            l032.ForeColor = System.Drawing.SystemColors.ControlText;
            l032.Location = new System.Drawing.Point(64, 65);
            l032.Name = "l032";
            l032.Size = new System.Drawing.Size(32, 15);
            l032.TabIndex = 261;
            l032.Text = "-";
            l032.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lreco
            // 
            lreco.BackColor = System.Drawing.SystemColors.Info;
            lreco.BorderStyle = BorderStyle.FixedSingle;
            lreco.ForeColor = System.Drawing.SystemColors.ControlText;
            lreco.Location = new System.Drawing.Point(216, 334);
            lreco.Name = "lreco";
            lreco.Size = new System.Drawing.Size(32, 21);
            lreco.TabIndex = 365;
            lreco.Text = "-";
            lreco.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbCGR
            // 
            lbCGR.BackColor = System.Drawing.SystemColors.Info;
            lbCGR.BorderStyle = BorderStyle.FixedSingle;
            lbCGR.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lbCGR.ForeColor = System.Drawing.Color.Black;
            lbCGR.Location = new System.Drawing.Point(184, 22);
            lbCGR.Name = "lbCGR";
            lbCGR.Size = new System.Drawing.Size(32, 30);
            lbCGR.TabIndex = 86;
            lbCGR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bCancelar
            // 
            bCancelar.BackColor = System.Drawing.Color.DarkSalmon;
            bCancelar.FlatStyle = FlatStyle.Popup;
            bCancelar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            bCancelar.Image = ((System.Drawing.Image)(resources.GetObject("bCancelar.Image")));
            bCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            bCancelar.Location = new System.Drawing.Point(632, 173);
            bCancelar.Name = "bCancelar";
            bCancelar.Size = new System.Drawing.Size(104, 32);
            bCancelar.TabIndex = 335;
            bCancelar.Text = "Cancelar";
            bCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            bCancelar.UseVisualStyleBackColor = false;
            bCancelar.Click += new System.EventHandler(BCancelarClick);
            // 
            // l053
            // 
            l053.BackColor = System.Drawing.SystemColors.Info;
            l053.BorderStyle = BorderStyle.FixedSingle;
            l053.ForeColor = System.Drawing.SystemColors.ControlText;
            l053.Location = new System.Drawing.Point(112, 103);
            l053.Name = "l053";
            l053.Size = new System.Drawing.Size(32, 15);
            l053.TabIndex = 277;
            l053.Text = "-";
            l053.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bSalvaCondis
            // 
            bSalvaCondis.BackColor = System.Drawing.Color.DarkSalmon;
            bSalvaCondis.FlatStyle = FlatStyle.Popup;
            bSalvaCondis.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            bSalvaCondis.Image = ((System.Drawing.Image)(resources.GetObject("bSalvaCondis.Image")));
            bSalvaCondis.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            bSalvaCondis.Location = new System.Drawing.Point(496, 71);
            bSalvaCondis.Name = "bSalvaCondis";
            bSalvaCondis.Size = new System.Drawing.Size(130, 32);
            bSalvaCondis.TabIndex = 364;
            bSalvaCondis.Text = "Salvar Límites";
            bSalvaCondis.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            bSalvaCondis.UseVisualStyleBackColor = false;
            bSalvaCondis.Click += new System.EventHandler(BSalvaCondisClick);
            // 
            // l106
            // 
            l106.BackColor = System.Drawing.SystemColors.Window;
            l106.BorderStyle = BorderStyle.FixedSingle;
            l106.ForeColor = System.Drawing.SystemColors.ControlText;
            l106.Location = new System.Drawing.Point(256, 189);
            l106.Name = "l106";
            l106.Size = new System.Drawing.Size(32, 15);
            l106.TabIndex = 326;
            l106.Text = "-";
            l106.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l105
            // 
            l105.BackColor = System.Drawing.SystemColors.Window;
            l105.BorderStyle = BorderStyle.FixedSingle;
            l105.ForeColor = System.Drawing.SystemColors.ControlText;
            l105.Location = new System.Drawing.Point(208, 189);
            l105.Name = "l105";
            l105.Size = new System.Drawing.Size(32, 15);
            l105.TabIndex = 312;
            l105.Text = "-";
            l105.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l104
            // 
            l104.BackColor = System.Drawing.SystemColors.Window;
            l104.BorderStyle = BorderStyle.FixedSingle;
            l104.ForeColor = System.Drawing.SystemColors.ControlText;
            l104.Location = new System.Drawing.Point(160, 189);
            l104.Name = "l104";
            l104.Size = new System.Drawing.Size(32, 15);
            l104.TabIndex = 298;
            l104.Text = "-";
            l104.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l024
            // 
            l024.BackColor = System.Drawing.SystemColors.Window;
            l024.BorderStyle = BorderStyle.FixedSingle;
            l024.ForeColor = System.Drawing.SystemColors.ControlText;
            l024.Location = new System.Drawing.Point(160, 49);
            l024.Name = "l024";
            l024.Size = new System.Drawing.Size(32, 15);
            l024.TabIndex = 290;
            l024.Text = "-";
            l024.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l102
            // 
            l102.BackColor = System.Drawing.SystemColors.Window;
            l102.BorderStyle = BorderStyle.FixedSingle;
            l102.ForeColor = System.Drawing.SystemColors.ControlText;
            l102.Location = new System.Drawing.Point(64, 189);
            l102.Name = "l102";
            l102.Size = new System.Drawing.Size(32, 15);
            l102.TabIndex = 270;
            l102.Text = "-";
            l102.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l101
            // 
            l101.BackColor = System.Drawing.SystemColors.Window;
            l101.BorderStyle = BorderStyle.FixedSingle;
            l101.ForeColor = System.Drawing.SystemColors.ControlText;
            l101.Location = new System.Drawing.Point(16, 189);
            l101.Name = "l101";
            l101.Size = new System.Drawing.Size(32, 15);
            l101.TabIndex = 256;
            l101.Text = "-";
            l101.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l021
            // 
            l021.BackColor = System.Drawing.SystemColors.Window;
            l021.BorderStyle = BorderStyle.FixedSingle;
            l021.ForeColor = System.Drawing.SystemColors.ControlText;
            l021.Location = new System.Drawing.Point(16, 49);
            l021.Name = "l021";
            l021.Size = new System.Drawing.Size(32, 15);
            l021.TabIndex = 248;
            l021.Text = "-";
            l021.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l022
            // 
            l022.BackColor = System.Drawing.SystemColors.Window;
            l022.BorderStyle = BorderStyle.FixedSingle;
            l022.ForeColor = System.Drawing.SystemColors.ControlText;
            l022.Location = new System.Drawing.Point(64, 49);
            l022.Name = "l022";
            l022.Size = new System.Drawing.Size(32, 15);
            l022.TabIndex = 262;
            l022.Text = "-";
            l022.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l023
            // 
            l023.BackColor = System.Drawing.SystemColors.Window;
            l023.BorderStyle = BorderStyle.FixedSingle;
            l023.ForeColor = System.Drawing.SystemColors.ControlText;
            l023.Location = new System.Drawing.Point(112, 49);
            l023.Name = "l023";
            l023.Size = new System.Drawing.Size(32, 15);
            l023.TabIndex = 276;
            l023.Text = "-";
            l023.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbmgsuma
            // 
            tbmgsuma.BackColor = System.Drawing.Color.PaleGreen;
            tbmgsuma.ForeColor = System.Drawing.SystemColors.ControlText;
            tbmgsuma.Location = new System.Drawing.Point(88, 334);
            tbmgsuma.MaxLength = 14;
            tbmgsuma.Name = "tbmgsuma";
            tbmgsuma.Size = new System.Drawing.Size(48, 21);
            tbmgsuma.TabIndex = 370;
            tbmgsuma.Text = "0-84";
            tbmgsuma.TextAlign = HorizontalAlignment.Center;
            // 
            // l042
            // 
            l042.BackColor = System.Drawing.SystemColors.Window;
            l042.BorderStyle = BorderStyle.FixedSingle;
            l042.ForeColor = System.Drawing.SystemColors.ControlText;
            l042.Location = new System.Drawing.Point(64, 81);
            l042.Name = "l042";
            l042.Size = new System.Drawing.Size(32, 15);
            l042.TabIndex = 260;
            l042.Text = "-";
            l042.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lsuma
            // 
            lsuma.BackColor = System.Drawing.SystemColors.Info;
            lsuma.BorderStyle = BorderStyle.FixedSingle;
            lsuma.ForeColor = System.Drawing.SystemColors.ControlText;
            lsuma.Location = new System.Drawing.Point(55, 334);
            lsuma.Name = "lsuma";
            lsuma.Size = new System.Drawing.Size(32, 21);
            lsuma.TabIndex = 363;
            lsuma.Text = "-";
            lsuma.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bCalcular
            // 
            bCalcular.BackColor = System.Drawing.Color.DarkSalmon;
            bCalcular.FlatStyle = FlatStyle.Popup;
            bCalcular.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            bCalcular.Image = ((System.Drawing.Image)(resources.GetObject("bCalcular.Image")));
            bCalcular.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            bCalcular.Location = new System.Drawing.Point(632, 71);
            bCalcular.Name = "bCalcular";
            bCalcular.Size = new System.Drawing.Size(104, 32);
            bCalcular.TabIndex = 334;
            bCalcular.Text = "Calcular";
            bCalcular.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            bCalcular.UseVisualStyleBackColor = false;
            bCalcular.Click += new System.EventHandler(BCalcularClick);
            // 
            // l016
            // 
            l016.BackColor = System.Drawing.SystemColors.Window;
            l016.BorderStyle = BorderStyle.FixedSingle;
            l016.ForeColor = System.Drawing.SystemColors.ControlText;
            l016.Location = new System.Drawing.Point(256, 34);
            l016.Name = "l016";
            l016.Size = new System.Drawing.Size(32, 14);
            l016.TabIndex = 315;
            l016.Text = "-";
            l016.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l015
            // 
            l015.BackColor = System.Drawing.SystemColors.Window;
            l015.BorderStyle = BorderStyle.FixedSingle;
            l015.ForeColor = System.Drawing.SystemColors.ControlText;
            l015.Location = new System.Drawing.Point(208, 34);
            l015.Name = "l015";
            l015.Size = new System.Drawing.Size(32, 14);
            l015.TabIndex = 301;
            l015.Text = "-";
            l015.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lTime
            // 
            lTime.BackColor = System.Drawing.SystemColors.Info;
            lTime.BorderStyle = BorderStyle.FixedSingle;
            lTime.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lTime.Location = new System.Drawing.Point(632, 150);
            lTime.Name = "lTime";
            lTime.Size = new System.Drawing.Size(104, 22);
            lTime.TabIndex = 360;
            lTime.Text = "Tiempo";
            lTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l013
            // 
            l013.BackColor = System.Drawing.SystemColors.Window;
            l013.BorderStyle = BorderStyle.FixedSingle;
            l013.ForeColor = System.Drawing.SystemColors.ControlText;
            l013.Location = new System.Drawing.Point(112, 34);
            l013.Name = "l013";
            l013.Size = new System.Drawing.Size(32, 14);
            l013.TabIndex = 273;
            l013.Text = "-";
            l013.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l012
            // 
            l012.BackColor = System.Drawing.SystemColors.Window;
            l012.BorderStyle = BorderStyle.FixedSingle;
            l012.ForeColor = System.Drawing.SystemColors.ControlText;
            l012.Location = new System.Drawing.Point(64, 34);
            l012.Name = "l012";
            l012.Size = new System.Drawing.Size(32, 14);
            l012.TabIndex = 259;
            l012.Text = "-";
            l012.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l011
            // 
            l011.BackColor = System.Drawing.SystemColors.Window;
            l011.BorderStyle = BorderStyle.FixedSingle;
            l011.ForeColor = System.Drawing.SystemColors.ControlText;
            l011.Location = new System.Drawing.Point(16, 34);
            l011.Name = "l011";
            l011.Size = new System.Drawing.Size(32, 14);
            l011.TabIndex = 245;
            l011.Text = "-";
            l011.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // valors1
            // 
            valors1.BackColor = System.Drawing.Color.Bisque;
            valors1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            valors1.Location = new System.Drawing.Point(8, 7);
            valors1.Name = "valors1";
            valors1.Size = new System.Drawing.Size(168, 489);
            valors1.TabIndex = 362;
            valors1.CrearCPs += new System.EventHandler(Valors1Creacps);
            // 
            // lNota
            // 
            lNota.BackColor = System.Drawing.SystemColors.Info;
            lNota.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lNota.Location = new System.Drawing.Point(184, 392);
            lNota.Name = "lNota";
            lNota.Size = new System.Drawing.Size(304, 74);
            lNota.TabIndex = 333;
            lNota.Text = "NOTA: Las condiciones de generación, después de una primera ejecución, aparecen e" +
                "n un fichero editable:   \"aidomnou.cfg\"";
            lNota.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l063
            // 
            l063.BackColor = System.Drawing.SystemColors.Info;
            l063.BorderStyle = BorderStyle.FixedSingle;
            l063.ForeColor = System.Drawing.SystemColors.ControlText;
            l063.Location = new System.Drawing.Point(112, 119);
            l063.Name = "l063";
            l063.Size = new System.Drawing.Size(32, 15);
            l063.TabIndex = 280;
            l063.Text = "-";
            l063.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l122
            // 
            l122.BackColor = System.Drawing.SystemColors.Info;
            l122.BorderStyle = BorderStyle.FixedSingle;
            l122.ForeColor = System.Drawing.SystemColors.ControlText;
            l122.Location = new System.Drawing.Point(64, 227);
            l122.Name = "l122";
            l122.Size = new System.Drawing.Size(32, 15);
            l122.TabIndex = 268;
            l122.Text = "-";
            l122.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l096
            // 
            l096.BackColor = System.Drawing.SystemColors.Window;
            l096.BorderStyle = BorderStyle.FixedSingle;
            l096.ForeColor = System.Drawing.SystemColors.ControlText;
            l096.Location = new System.Drawing.Point(256, 173);
            l096.Name = "l096";
            l096.Size = new System.Drawing.Size(32, 15);
            l096.TabIndex = 323;
            l096.Text = "-";
            l096.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l103
            // 
            l103.BackColor = System.Drawing.SystemColors.Window;
            l103.BorderStyle = BorderStyle.FixedSingle;
            l103.ForeColor = System.Drawing.SystemColors.ControlText;
            l103.Location = new System.Drawing.Point(112, 189);
            l103.Name = "l103";
            l103.Size = new System.Drawing.Size(32, 15);
            l103.TabIndex = 284;
            l103.Text = "-";
            l103.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l094
            // 
            l094.BackColor = System.Drawing.SystemColors.Window;
            l094.BorderStyle = BorderStyle.FixedSingle;
            l094.ForeColor = System.Drawing.SystemColors.ControlText;
            l094.Location = new System.Drawing.Point(160, 173);
            l094.Name = "l094";
            l094.Size = new System.Drawing.Size(32, 15);
            l094.TabIndex = 295;
            l094.Text = "-";
            l094.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l093
            // 
            l093.BackColor = System.Drawing.SystemColors.Window;
            l093.BorderStyle = BorderStyle.FixedSingle;
            l093.ForeColor = System.Drawing.SystemColors.ControlText;
            l093.Location = new System.Drawing.Point(112, 173);
            l093.Name = "l093";
            l093.Size = new System.Drawing.Size(32, 15);
            l093.TabIndex = 281;
            l093.Text = "-";
            l093.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lColsAdm
            // 
            lColsAdm.BackColor = System.Drawing.SystemColors.Info;
            lColsAdm.BorderStyle = BorderStyle.FixedSingle;
            lColsAdm.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lColsAdm.Location = new System.Drawing.Point(632, 127);
            lColsAdm.Name = "lColsAdm";
            lColsAdm.Size = new System.Drawing.Size(104, 22);
            lColsAdm.TabIndex = 331;
            lColsAdm.Text = "Válidas";
            lColsAdm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l091
            // 
            l091.BackColor = System.Drawing.SystemColors.Window;
            l091.BorderStyle = BorderStyle.FixedSingle;
            l091.ForeColor = System.Drawing.SystemColors.ControlText;
            l091.Location = new System.Drawing.Point(16, 173);
            l091.Name = "l091";
            l091.Size = new System.Drawing.Size(32, 15);
            l091.TabIndex = 253;
            l091.Text = "-";
            l091.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbmg3
            // 
            tbmg3.BackColor = System.Drawing.Color.PaleGreen;
            tbmg3.ForeColor = System.Drawing.SystemColors.ControlText;
            tbmg3.Location = new System.Drawing.Point(105, 290);
            tbmg3.MaxLength = 14;
            tbmg3.Name = "tbmg3";
            tbmg3.Size = new System.Drawing.Size(48, 21);
            tbmg3.TabIndex = 369;
            tbmg3.Text = "0-14";
            tbmg3.TextAlign = HorizontalAlignment.Center;
            // 
            // lrk3
            // 
            lrk3.BackColor = System.Drawing.SystemColors.Info;
            lrk3.BorderStyle = BorderStyle.FixedSingle;
            lrk3.ForeColor = System.Drawing.SystemColors.ControlText;
            lrk3.Location = new System.Drawing.Point(112, 312);
            lrk3.Name = "lrk3";
            lrk3.Size = new System.Drawing.Size(32, 19);
            lrk3.TabIndex = 352;
            lrk3.Text = "-";
            lrk3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lFGR
            // 
            lFGR.BackColor = System.Drawing.SystemColors.Info;
            lFGR.BorderStyle = BorderStyle.FixedSingle;
            lFGR.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lFGR.ForeColor = System.Drawing.Color.Black;
            lFGR.Location = new System.Drawing.Point(40, 22);
            lFGR.Name = "lFGR";
            lFGR.Size = new System.Drawing.Size(136, 23);
            lFGR.TabIndex = 88;
            lFGR.Text = "Fichero Ganadoras";
            lFGR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lrk1
            // 
            lrk1.BackColor = System.Drawing.SystemColors.Info;
            lrk1.BorderStyle = BorderStyle.FixedSingle;
            lrk1.ForeColor = System.Drawing.SystemColors.ControlText;
            lrk1.Location = new System.Drawing.Point(16, 312);
            lrk1.Name = "lrk1";
            lrk1.Size = new System.Drawing.Size(32, 19);
            lrk1.TabIndex = 350;
            lrk1.Text = "-";
            lrk1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l134
            // 
            l134.BackColor = System.Drawing.SystemColors.Info;
            l134.BorderStyle = BorderStyle.FixedSingle;
            l134.ForeColor = System.Drawing.SystemColors.ControlText;
            l134.Location = new System.Drawing.Point(160, 243);
            l134.Name = "l134";
            l134.Size = new System.Drawing.Size(32, 15);
            l134.TabIndex = 300;
            l134.Text = "-";
            l134.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l135
            // 
            l135.BackColor = System.Drawing.SystemColors.Info;
            l135.BorderStyle = BorderStyle.FixedSingle;
            l135.ForeColor = System.Drawing.SystemColors.ControlText;
            l135.Location = new System.Drawing.Point(208, 243);
            l135.Name = "l135";
            l135.Size = new System.Drawing.Size(32, 15);
            l135.TabIndex = 314;
            l135.Text = "-";
            l135.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l136
            // 
            l136.BackColor = System.Drawing.SystemColors.Info;
            l136.BorderStyle = BorderStyle.FixedSingle;
            l136.ForeColor = System.Drawing.SystemColors.ControlText;
            l136.Location = new System.Drawing.Point(256, 243);
            l136.Name = "l136";
            l136.Size = new System.Drawing.Size(32, 15);
            l136.TabIndex = 328;
            l136.Text = "-";
            l136.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lColsIni
            // 
            lColsIni.BackColor = System.Drawing.SystemColors.Info;
            lColsIni.BorderStyle = BorderStyle.FixedSingle;
            lColsIni.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lColsIni.Location = new System.Drawing.Point(632, 104);
            lColsIni.Name = "lColsIni";
            lColsIni.Size = new System.Drawing.Size(104, 22);
            lColsIni.TabIndex = 332;
            lColsIni.Text = "Procesadas";
            lColsIni.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l131
            // 
            l131.BackColor = System.Drawing.SystemColors.Info;
            l131.BorderStyle = BorderStyle.FixedSingle;
            l131.ForeColor = System.Drawing.SystemColors.ControlText;
            l131.Location = new System.Drawing.Point(16, 243);
            l131.Name = "l131";
            l131.Size = new System.Drawing.Size(32, 15);
            l131.TabIndex = 258;
            l131.Text = "-";
            l131.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l073
            // 
            l073.BackColor = System.Drawing.SystemColors.Info;
            l073.BorderStyle = BorderStyle.FixedSingle;
            l073.ForeColor = System.Drawing.SystemColors.ControlText;
            l073.Location = new System.Drawing.Point(112, 135);
            l073.Name = "l073";
            l073.Size = new System.Drawing.Size(32, 15);
            l073.TabIndex = 279;
            l073.Text = "-";
            l073.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l072
            // 
            l072.BackColor = System.Drawing.SystemColors.Info;
            l072.BorderStyle = BorderStyle.FixedSingle;
            l072.ForeColor = System.Drawing.SystemColors.ControlText;
            l072.Location = new System.Drawing.Point(64, 135);
            l072.Name = "l072";
            l072.Size = new System.Drawing.Size(32, 15);
            l072.TabIndex = 265;
            l072.Text = "-";
            l072.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l075
            // 
            l075.BackColor = System.Drawing.SystemColors.Info;
            l075.BorderStyle = BorderStyle.FixedSingle;
            l075.ForeColor = System.Drawing.SystemColors.ControlText;
            l075.Location = new System.Drawing.Point(208, 135);
            l075.Name = "l075";
            l075.Size = new System.Drawing.Size(32, 15);
            l075.TabIndex = 307;
            l075.Text = "-";
            l075.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l074
            // 
            l074.BackColor = System.Drawing.SystemColors.Info;
            l074.BorderStyle = BorderStyle.FixedSingle;
            l074.ForeColor = System.Drawing.SystemColors.ControlText;
            l074.Location = new System.Drawing.Point(160, 135);
            l074.Name = "l074";
            l074.Size = new System.Drawing.Size(32, 15);
            l074.TabIndex = 293;
            l074.Text = "-";
            l074.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l076
            // 
            l076.BackColor = System.Drawing.SystemColors.Info;
            l076.BorderStyle = BorderStyle.FixedSingle;
            l076.ForeColor = System.Drawing.SystemColors.ControlText;
            l076.Location = new System.Drawing.Point(256, 135);
            l076.Name = "l076";
            l076.Size = new System.Drawing.Size(32, 15);
            l076.TabIndex = 321;
            l076.Text = "-";
            l076.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbmg6
            // 
            tbmg6.BackColor = System.Drawing.Color.PaleGreen;
            tbmg6.ForeColor = System.Drawing.SystemColors.ControlText;
            tbmg6.Location = new System.Drawing.Point(252, 290);
            tbmg6.MaxLength = 14;
            tbmg6.Name = "tbmg6";
            tbmg6.Size = new System.Drawing.Size(48, 21);
            tbmg6.TabIndex = 372;
            tbmg6.Text = "0-14";
            tbmg6.TextAlign = HorizontalAlignment.Center;
            // 
            // l086
            // 
            l086.BackColor = System.Drawing.SystemColors.Info;
            l086.BorderStyle = BorderStyle.FixedSingle;
            l086.ForeColor = System.Drawing.SystemColors.ControlText;
            l086.Location = new System.Drawing.Point(256, 151);
            l086.Name = "l086";
            l086.Size = new System.Drawing.Size(32, 15);
            l086.TabIndex = 320;
            l086.Text = "-";
            l086.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l025
            // 
            l025.BackColor = System.Drawing.SystemColors.Window;
            l025.BorderStyle = BorderStyle.FixedSingle;
            l025.ForeColor = System.Drawing.SystemColors.ControlText;
            l025.Location = new System.Drawing.Point(208, 49);
            l025.Name = "l025";
            l025.Size = new System.Drawing.Size(32, 15);
            l025.TabIndex = 304;
            l025.Text = "-";
            l025.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l026
            // 
            l026.BackColor = System.Drawing.SystemColors.Window;
            l026.BorderStyle = BorderStyle.FixedSingle;
            l026.ForeColor = System.Drawing.SystemColors.ControlText;
            l026.Location = new System.Drawing.Point(256, 49);
            l026.Name = "l026";
            l026.Size = new System.Drawing.Size(32, 15);
            l026.TabIndex = 318;
            l026.Text = "-";
            l026.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l085
            // 
            l085.BackColor = System.Drawing.SystemColors.Info;
            l085.BorderStyle = BorderStyle.FixedSingle;
            l085.ForeColor = System.Drawing.SystemColors.ControlText;
            l085.Location = new System.Drawing.Point(208, 151);
            l085.Name = "l085";
            l085.Size = new System.Drawing.Size(32, 15);
            l085.TabIndex = 306;
            l085.Text = "-";
            l085.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l082
            // 
            l082.BackColor = System.Drawing.SystemColors.Info;
            l082.BorderStyle = BorderStyle.FixedSingle;
            l082.ForeColor = System.Drawing.SystemColors.ControlText;
            l082.Location = new System.Drawing.Point(64, 151);
            l082.Name = "l082";
            l082.Size = new System.Drawing.Size(32, 15);
            l082.TabIndex = 264;
            l082.Text = "-";
            l082.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l083
            // 
            l083.BackColor = System.Drawing.SystemColors.Info;
            l083.BorderStyle = BorderStyle.FixedSingle;
            l083.ForeColor = System.Drawing.SystemColors.ControlText;
            l083.Location = new System.Drawing.Point(112, 151);
            l083.Name = "l083";
            l083.Size = new System.Drawing.Size(32, 15);
            l083.TabIndex = 278;
            l083.Text = "-";
            l083.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l081
            // 
            l081.BackColor = System.Drawing.SystemColors.Info;
            l081.BorderStyle = BorderStyle.FixedSingle;
            l081.ForeColor = System.Drawing.SystemColors.ControlText;
            l081.Location = new System.Drawing.Point(16, 151);
            l081.Name = "l081";
            l081.Size = new System.Drawing.Size(32, 15);
            l081.TabIndex = 250;
            l081.Text = "-";
            l081.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bMasR
            // 
            bMasR.BackColor = System.Drawing.Color.DarkSalmon;
            bMasR.FlatStyle = FlatStyle.Popup;
            bMasR.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            bMasR.ForeColor = System.Drawing.Color.Black;
            bMasR.Location = new System.Drawing.Point(224, 21);
            bMasR.Name = "bMasR";
            bMasR.Size = new System.Drawing.Size(16, 15);
            bMasR.TabIndex = 84;
            bMasR.Text = "+";
            bMasR.UseVisualStyleBackColor = false;
            bMasR.Click += new System.EventHandler(BMasRClick);
            // 
            // l143
            // 
            l143.BackColor = System.Drawing.SystemColors.Info;
            l143.BorderStyle = BorderStyle.FixedSingle;
            l143.ForeColor = System.Drawing.SystemColors.ControlText;
            l143.Location = new System.Drawing.Point(112, 259);
            l143.Name = "l143";
            l143.Size = new System.Drawing.Size(32, 15);
            l143.TabIndex = 285;
            l143.Text = "-";
            l143.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l142
            // 
            l142.BackColor = System.Drawing.SystemColors.Info;
            l142.BorderStyle = BorderStyle.FixedSingle;
            l142.ForeColor = System.Drawing.SystemColors.ControlText;
            l142.Location = new System.Drawing.Point(64, 259);
            l142.Name = "l142";
            l142.Size = new System.Drawing.Size(32, 15);
            l142.TabIndex = 271;
            l142.Text = "-";
            l142.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l141
            // 
            l141.BackColor = System.Drawing.SystemColors.Info;
            l141.BorderStyle = BorderStyle.FixedSingle;
            l141.ForeColor = System.Drawing.SystemColors.ControlText;
            l141.Location = new System.Drawing.Point(16, 259);
            l141.Name = "l141";
            l141.Size = new System.Drawing.Size(32, 15);
            l141.TabIndex = 257;
            l141.Text = "-";
            l141.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbColR
            // 
            tbColR.BorderStyle = BorderStyle.FixedSingle;
            tbColR.CharacterCasing = CharacterCasing.Upper;
            tbColR.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            tbColR.ForeColor = System.Drawing.Color.Black;
            tbColR.Location = new System.Drawing.Point(120, 67);
            tbColR.MaxLength = 14;
            tbColR.Name = "tbColR";
            tbColR.Size = new System.Drawing.Size(120, 21);
            tbColR.TabIndex = 89;
            tbColR.Text = "2222XXXX222XXX";
            tbColR.TextAlign = HorizontalAlignment.Center;
            // 
            // l146
            // 
            l146.BackColor = System.Drawing.SystemColors.Info;
            l146.BorderStyle = BorderStyle.FixedSingle;
            l146.ForeColor = System.Drawing.SystemColors.ControlText;
            l146.Location = new System.Drawing.Point(256, 259);
            l146.Name = "l146";
            l146.Size = new System.Drawing.Size(32, 15);
            l146.TabIndex = 327;
            l146.Text = "-";
            l146.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lfile
            // 
            lfile.BackColor = System.Drawing.SystemColors.Info;
            lfile.BorderStyle = BorderStyle.FixedSingle;
            lfile.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lfile.Location = new System.Drawing.Point(546, 286);
            lfile.Name = "lfile";
            lfile.Size = new System.Drawing.Size(154, 22);
            lfile.TabIndex = 363;
            lfile.Text = "Fichero";
            lfile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbmg4
            // 
            tbmg4.BackColor = System.Drawing.Color.PaleGreen;
            tbmg4.ForeColor = System.Drawing.SystemColors.ControlText;
            tbmg4.Location = new System.Drawing.Point(154, 290);
            tbmg4.MaxLength = 14;
            tbmg4.Name = "tbmg4";
            tbmg4.Size = new System.Drawing.Size(48, 21);
            tbmg4.TabIndex = 370;
            tbmg4.Text = "0-14";
            tbmg4.TextAlign = HorizontalAlignment.Center;
            // 
            // tbmg5
            // 
            tbmg5.BackColor = System.Drawing.Color.PaleGreen;
            tbmg5.ForeColor = System.Drawing.SystemColors.ControlText;
            tbmg5.Location = new System.Drawing.Point(203, 290);
            tbmg5.MaxLength = 14;
            tbmg5.Name = "tbmg5";
            tbmg5.Size = new System.Drawing.Size(48, 21);
            tbmg5.TabIndex = 371;
            tbmg5.Text = "0-14";
            tbmg5.TextAlign = HorizontalAlignment.Center;
            // 
            // tbmg2
            // 
            tbmg2.BackColor = System.Drawing.Color.PaleGreen;
            tbmg2.ForeColor = System.Drawing.SystemColors.ControlText;
            tbmg2.Location = new System.Drawing.Point(56, 290);
            tbmg2.MaxLength = 14;
            tbmg2.Name = "tbmg2";
            tbmg2.Size = new System.Drawing.Size(48, 21);
            tbmg2.TabIndex = 368;
            tbmg2.Text = "0-14";
            tbmg2.TextAlign = HorizontalAlignment.Center;
            // 
            // l066
            // 
            l066.BackColor = System.Drawing.SystemColors.Info;
            l066.BorderStyle = BorderStyle.FixedSingle;
            l066.ForeColor = System.Drawing.SystemColors.ControlText;
            l066.Location = new System.Drawing.Point(256, 119);
            l066.Name = "l066";
            l066.Size = new System.Drawing.Size(32, 15);
            l066.TabIndex = 322;
            l066.Text = "-";
            l066.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbmg1
            // 
            tbmg1.BackColor = System.Drawing.Color.PaleGreen;
            tbmg1.ForeColor = System.Drawing.SystemColors.ControlText;
            tbmg1.Location = new System.Drawing.Point(7, 290);
            tbmg1.MaxLength = 14;
            tbmg1.Name = "tbmg1";
            tbmg1.Size = new System.Drawing.Size(48, 21);
            tbmg1.TabIndex = 367;
            tbmg1.Text = "0-14";
            tbmg1.TextAlign = HorizontalAlignment.Center;
            // 
            // l014
            // 
            l014.BackColor = System.Drawing.SystemColors.Window;
            l014.BorderStyle = BorderStyle.FixedSingle;
            l014.ForeColor = System.Drawing.SystemColors.ControlText;
            l014.Location = new System.Drawing.Point(160, 34);
            l014.Name = "l014";
            l014.Size = new System.Drawing.Size(32, 14);
            l014.TabIndex = 287;
            l014.Text = "-";
            l014.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bGrabaCols
            // 
            bGrabaCols.BackColor = System.Drawing.Color.DarkSalmon;
            bGrabaCols.FlatStyle = FlatStyle.Popup;
            bGrabaCols.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            bGrabaCols.Image = ((System.Drawing.Image)(resources.GetObject("bGrabaCols.Image")));
            bGrabaCols.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            bGrabaCols.Location = new System.Drawing.Point(546, 253);
            bGrabaCols.Name = "bGrabaCols";
            bGrabaCols.Size = new System.Drawing.Size(154, 32);
            bGrabaCols.TabIndex = 197;
            bGrabaCols.Text = "Grabar Resultado";
            bGrabaCols.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            bGrabaCols.UseVisualStyleBackColor = false;
            bGrabaCols.Click += new System.EventHandler(BGrabaColsClick);
            // 
            // bAnalizar
            // 
            bAnalizar.BackColor = System.Drawing.Color.DarkSalmon;
            bAnalizar.Enabled = false;
            bAnalizar.FlatStyle = FlatStyle.Popup;
            bAnalizar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            bAnalizar.ForeColor = System.Drawing.Color.Black;
            bAnalizar.Image = ((System.Drawing.Image)(resources.GetObject("bAnalizar.Image")));
            bAnalizar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            bAnalizar.Location = new System.Drawing.Point(8, 59);
            bAnalizar.Name = "bAnalizar";
            bAnalizar.Size = new System.Drawing.Size(104, 32);
            bAnalizar.TabIndex = 27;
            bAnalizar.Text = "Analizar";
            bAnalizar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            bAnalizar.UseVisualStyleBackColor = false;
            bAnalizar.Click += new System.EventHandler(BAnalizarClick);
            // 
            // l064
            // 
            l064.BackColor = System.Drawing.SystemColors.Info;
            l064.BorderStyle = BorderStyle.FixedSingle;
            l064.ForeColor = System.Drawing.SystemColors.ControlText;
            l064.Location = new System.Drawing.Point(160, 119);
            l064.Name = "l064";
            l064.Size = new System.Drawing.Size(32, 15);
            l064.TabIndex = 294;
            l064.Text = "-";
            l064.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            label4.BackColor = System.Drawing.Color.Bisque;
            label4.ForeColor = System.Drawing.SystemColors.ControlText;
            label4.Location = new System.Drawing.Point(8, 334);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(48, 21);
            label4.TabIndex = 362;
            label4.Text = "suma:";
            label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // l132
            // 
            l132.BackColor = System.Drawing.SystemColors.Info;
            l132.BorderStyle = BorderStyle.FixedSingle;
            l132.ForeColor = System.Drawing.SystemColors.ControlText;
            l132.Location = new System.Drawing.Point(64, 243);
            l132.Name = "l132";
            l132.Size = new System.Drawing.Size(32, 15);
            l132.TabIndex = 272;
            l132.Text = "-";
            l132.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l133
            // 
            l133.BackColor = System.Drawing.SystemColors.Info;
            l133.BorderStyle = BorderStyle.FixedSingle;
            l133.ForeColor = System.Drawing.SystemColors.ControlText;
            l133.Location = new System.Drawing.Point(112, 243);
            l133.Name = "l133";
            l133.Size = new System.Drawing.Size(32, 15);
            l133.TabIndex = 286;
            l133.Text = "-";
            l133.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            label2.BackColor = System.Drawing.Color.Bisque;
            label2.ForeColor = System.Drawing.SystemColors.ControlText;
            label2.Location = new System.Drawing.Point(144, 334);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(72, 21);
            label2.TabIndex = 364;
            label2.Text = "recorrido:";
            label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // l114
            // 
            l114.BackColor = System.Drawing.SystemColors.Window;
            l114.BorderStyle = BorderStyle.FixedSingle;
            l114.ForeColor = System.Drawing.SystemColors.ControlText;
            l114.Location = new System.Drawing.Point(160, 205);
            l114.Name = "l114";
            l114.Size = new System.Drawing.Size(32, 15);
            l114.TabIndex = 297;
            l114.Text = "-";
            l114.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l115
            // 
            l115.BackColor = System.Drawing.SystemColors.Window;
            l115.BorderStyle = BorderStyle.FixedSingle;
            l115.ForeColor = System.Drawing.SystemColors.ControlText;
            l115.Location = new System.Drawing.Point(208, 205);
            l115.Name = "l115";
            l115.Size = new System.Drawing.Size(32, 15);
            l115.TabIndex = 311;
            l115.Text = "-";
            l115.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bLeeCondis
            // 
            bLeeCondis.BackColor = System.Drawing.Color.DarkSalmon;
            bLeeCondis.FlatStyle = FlatStyle.Popup;
            bLeeCondis.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            bLeeCondis.Image = ((System.Drawing.Image)(resources.GetObject("bLeeCondis.Image")));
            bLeeCondis.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            bLeeCondis.Location = new System.Drawing.Point(496, 127);
            bLeeCondis.Name = "bLeeCondis";
            bLeeCondis.RightToLeft = RightToLeft.No;
            bLeeCondis.Size = new System.Drawing.Size(130, 32);
            bLeeCondis.TabIndex = 365;
            bLeeCondis.Text = "Recuperar Límites";
            bLeeCondis.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            bLeeCondis.UseVisualStyleBackColor = false;
            bLeeCondis.Click += new System.EventHandler(BLeeCondisClick);
            // 
            // l052
            // 
            l052.BackColor = System.Drawing.SystemColors.Info;
            l052.BorderStyle = BorderStyle.FixedSingle;
            l052.ForeColor = System.Drawing.SystemColors.ControlText;
            l052.Location = new System.Drawing.Point(64, 103);
            l052.Name = "l052";
            l052.Size = new System.Drawing.Size(32, 15);
            l052.TabIndex = 263;
            l052.Text = "-";
            l052.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l051
            // 
            l051.BackColor = System.Drawing.SystemColors.Info;
            l051.BorderStyle = BorderStyle.FixedSingle;
            l051.ForeColor = System.Drawing.SystemColors.ControlText;
            l051.Location = new System.Drawing.Point(16, 103);
            l051.Name = "l051";
            l051.Size = new System.Drawing.Size(32, 15);
            l051.TabIndex = 249;
            l051.Text = "-";
            l051.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l111
            // 
            l111.BackColor = System.Drawing.SystemColors.Window;
            l111.BorderStyle = BorderStyle.FixedSingle;
            l111.ForeColor = System.Drawing.SystemColors.ControlText;
            l111.Location = new System.Drawing.Point(16, 205);
            l111.Name = "l111";
            l111.Size = new System.Drawing.Size(32, 15);
            l111.TabIndex = 255;
            l111.Text = "-";
            l111.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l056
            // 
            l056.BackColor = System.Drawing.SystemColors.Info;
            l056.BorderStyle = BorderStyle.FixedSingle;
            l056.ForeColor = System.Drawing.SystemColors.ControlText;
            l056.Location = new System.Drawing.Point(256, 103);
            l056.Name = "l056";
            l056.Size = new System.Drawing.Size(32, 15);
            l056.TabIndex = 319;
            l056.Text = "-";
            l056.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l055
            // 
            l055.BackColor = System.Drawing.SystemColors.Info;
            l055.BorderStyle = BorderStyle.FixedSingle;
            l055.ForeColor = System.Drawing.SystemColors.ControlText;
            l055.Location = new System.Drawing.Point(208, 103);
            l055.Name = "l055";
            l055.Size = new System.Drawing.Size(32, 15);
            l055.TabIndex = 305;
            l055.Text = "-";
            l055.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l054
            // 
            l054.BackColor = System.Drawing.SystemColors.Info;
            l054.BorderStyle = BorderStyle.FixedSingle;
            l054.ForeColor = System.Drawing.SystemColors.ControlText;
            l054.Location = new System.Drawing.Point(160, 103);
            l054.Name = "l054";
            l054.Size = new System.Drawing.Size(32, 15);
            l054.TabIndex = 291;
            l054.Text = "-";
            l054.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bMenosR
            // 
            bMenosR.BackColor = System.Drawing.Color.DarkSalmon;
            bMenosR.FlatStyle = FlatStyle.Popup;
            bMenosR.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            bMenosR.ForeColor = System.Drawing.Color.Black;
            bMenosR.Location = new System.Drawing.Point(224, 37);
            bMenosR.Name = "bMenosR";
            bMenosR.Size = new System.Drawing.Size(16, 15);
            bMenosR.TabIndex = 85;
            bMenosR.Text = "-";
            bMenosR.UseVisualStyleBackColor = false;
            bMenosR.Click += new System.EventHandler(BMenosRClick);
            // 
            // l095
            // 
            l095.BackColor = System.Drawing.SystemColors.Window;
            l095.BorderStyle = BorderStyle.FixedSingle;
            l095.ForeColor = System.Drawing.SystemColors.ControlText;
            l095.Location = new System.Drawing.Point(208, 173);
            l095.Name = "l095";
            l095.Size = new System.Drawing.Size(32, 15);
            l095.TabIndex = 309;
            l095.Text = "-";
            l095.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l092
            // 
            l092.BackColor = System.Drawing.SystemColors.Window;
            l092.BorderStyle = BorderStyle.FixedSingle;
            l092.ForeColor = System.Drawing.SystemColors.ControlText;
            l092.Location = new System.Drawing.Point(64, 173);
            l092.Name = "l092";
            l092.Size = new System.Drawing.Size(32, 15);
            l092.TabIndex = 267;
            l092.Text = "-";
            l092.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l071
            // 
            l071.BackColor = System.Drawing.SystemColors.Info;
            l071.BorderStyle = BorderStyle.FixedSingle;
            l071.ForeColor = System.Drawing.SystemColors.ControlText;
            l071.Location = new System.Drawing.Point(16, 135);
            l071.Name = "l071";
            l071.Size = new System.Drawing.Size(32, 15);
            l071.TabIndex = 251;
            l071.Text = "-";
            l071.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(tbmg6);
            groupBox2.Controls.Add(tbmg5);
            groupBox2.Controls.Add(tbmg4);
            groupBox2.Controls.Add(tbmg3);
            groupBox2.Controls.Add(tbmg2);
            groupBox2.Controls.Add(lreco);
            groupBox2.Controls.Add(label2);
            groupBox2.Controls.Add(lrk6);
            groupBox2.Controls.Add(lrk5);
            groupBox2.Controls.Add(lrk4);
            groupBox2.Controls.Add(lrk3);
            groupBox2.Controls.Add(lrk2);
            groupBox2.Controls.Add(lrk1);
            groupBox2.Controls.Add(l136);
            groupBox2.Controls.Add(l146);
            groupBox2.Controls.Add(l106);
            groupBox2.Controls.Add(l116);
            groupBox2.Controls.Add(l126);
            groupBox2.Controls.Add(l096);
            groupBox2.Controls.Add(l066);
            groupBox2.Controls.Add(l076);
            groupBox2.Controls.Add(l086);
            groupBox2.Controls.Add(l056);
            groupBox2.Controls.Add(l026);
            groupBox2.Controls.Add(l036);
            groupBox2.Controls.Add(l046);
            groupBox2.Controls.Add(l016);
            groupBox2.Controls.Add(l135);
            groupBox2.Controls.Add(l145);
            groupBox2.Controls.Add(l105);
            groupBox2.Controls.Add(l115);
            groupBox2.Controls.Add(l125);
            groupBox2.Controls.Add(l095);
            groupBox2.Controls.Add(l065);
            groupBox2.Controls.Add(l075);
            groupBox2.Controls.Add(l085);
            groupBox2.Controls.Add(l055);
            groupBox2.Controls.Add(l025);
            groupBox2.Controls.Add(l035);
            groupBox2.Controls.Add(l045);
            groupBox2.Controls.Add(l015);
            groupBox2.Controls.Add(l134);
            groupBox2.Controls.Add(l144);
            groupBox2.Controls.Add(l104);
            groupBox2.Controls.Add(l114);
            groupBox2.Controls.Add(l124);
            groupBox2.Controls.Add(l094);
            groupBox2.Controls.Add(l064);
            groupBox2.Controls.Add(l074);
            groupBox2.Controls.Add(l084);
            groupBox2.Controls.Add(l054);
            groupBox2.Controls.Add(l024);
            groupBox2.Controls.Add(l034);
            groupBox2.Controls.Add(l044);
            groupBox2.Controls.Add(l014);
            groupBox2.Controls.Add(l133);
            groupBox2.Controls.Add(l143);
            groupBox2.Controls.Add(l103);
            groupBox2.Controls.Add(l113);
            groupBox2.Controls.Add(l123);
            groupBox2.Controls.Add(l093);
            groupBox2.Controls.Add(l063);
            groupBox2.Controls.Add(l073);
            groupBox2.Controls.Add(l083);
            groupBox2.Controls.Add(l053);
            groupBox2.Controls.Add(l023);
            groupBox2.Controls.Add(l033);
            groupBox2.Controls.Add(l043);
            groupBox2.Controls.Add(l013);
            groupBox2.Controls.Add(l132);
            groupBox2.Controls.Add(l142);
            groupBox2.Controls.Add(l102);
            groupBox2.Controls.Add(l112);
            groupBox2.Controls.Add(l122);
            groupBox2.Controls.Add(l092);
            groupBox2.Controls.Add(l062);
            groupBox2.Controls.Add(l072);
            groupBox2.Controls.Add(l082);
            groupBox2.Controls.Add(l052);
            groupBox2.Controls.Add(l022);
            groupBox2.Controls.Add(l032);
            groupBox2.Controls.Add(l042);
            groupBox2.Controls.Add(l012);
            groupBox2.Controls.Add(l131);
            groupBox2.Controls.Add(l141);
            groupBox2.Controls.Add(l101);
            groupBox2.Controls.Add(l111);
            groupBox2.Controls.Add(l121);
            groupBox2.Controls.Add(l091);
            groupBox2.Controls.Add(l061);
            groupBox2.Controls.Add(l071);
            groupBox2.Controls.Add(l081);
            groupBox2.Controls.Add(l051);
            groupBox2.Controls.Add(l021);
            groupBox2.Controls.Add(l031);
            groupBox2.Controls.Add(l041);
            groupBox2.Controls.Add(l011);
            groupBox2.Controls.Add(tbmg1);
            groupBox2.Controls.Add(lsuma);
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(tbmgsuma);
            groupBox2.Controls.Add(tbmgreco);
            groupBox2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            groupBox2.ForeColor = System.Drawing.Color.Maroon;
            groupBox2.Location = new System.Drawing.Point(184, 24);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new System.Drawing.Size(304, 364);
            groupBox2.TabIndex = 189;
            groupBox2.TabStop = false;
            groupBox2.Text = "Columnas Probables";
            // 
            // lrk6
            // 
            lrk6.BackColor = System.Drawing.SystemColors.Info;
            lrk6.BorderStyle = BorderStyle.FixedSingle;
            lrk6.ForeColor = System.Drawing.SystemColors.ControlText;
            lrk6.Location = new System.Drawing.Point(256, 312);
            lrk6.Name = "lrk6";
            lrk6.Size = new System.Drawing.Size(32, 19);
            lrk6.TabIndex = 355;
            lrk6.Text = "-";
            lrk6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lrk5
            // 
            lrk5.BackColor = System.Drawing.SystemColors.Info;
            lrk5.BorderStyle = BorderStyle.FixedSingle;
            lrk5.ForeColor = System.Drawing.SystemColors.ControlText;
            lrk5.Location = new System.Drawing.Point(208, 312);
            lrk5.Name = "lrk5";
            lrk5.Size = new System.Drawing.Size(32, 19);
            lrk5.TabIndex = 354;
            lrk5.Text = "-";
            lrk5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lrk4
            // 
            lrk4.BackColor = System.Drawing.SystemColors.Info;
            lrk4.BorderStyle = BorderStyle.FixedSingle;
            lrk4.ForeColor = System.Drawing.SystemColors.ControlText;
            lrk4.Location = new System.Drawing.Point(160, 312);
            lrk4.Name = "lrk4";
            lrk4.Size = new System.Drawing.Size(32, 19);
            lrk4.TabIndex = 353;
            lrk4.Text = "-";
            lrk4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lrk2
            // 
            lrk2.BackColor = System.Drawing.SystemColors.Info;
            lrk2.BorderStyle = BorderStyle.FixedSingle;
            lrk2.ForeColor = System.Drawing.SystemColors.ControlText;
            lrk2.Location = new System.Drawing.Point(64, 312);
            lrk2.Name = "lrk2";
            lrk2.Size = new System.Drawing.Size(32, 19);
            lrk2.TabIndex = 351;
            lrk2.Text = "-";
            lrk2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l116
            // 
            l116.BackColor = System.Drawing.SystemColors.Window;
            l116.BorderStyle = BorderStyle.FixedSingle;
            l116.ForeColor = System.Drawing.SystemColors.ControlText;
            l116.Location = new System.Drawing.Point(256, 205);
            l116.Name = "l116";
            l116.Size = new System.Drawing.Size(32, 15);
            l116.TabIndex = 325;
            l116.Text = "-";
            l116.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l126
            // 
            l126.BackColor = System.Drawing.SystemColors.Info;
            l126.BorderStyle = BorderStyle.FixedSingle;
            l126.ForeColor = System.Drawing.SystemColors.ControlText;
            l126.Location = new System.Drawing.Point(256, 227);
            l126.Name = "l126";
            l126.Size = new System.Drawing.Size(32, 15);
            l126.TabIndex = 324;
            l126.Text = "-";
            l126.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l036
            // 
            l036.BackColor = System.Drawing.SystemColors.Window;
            l036.BorderStyle = BorderStyle.FixedSingle;
            l036.ForeColor = System.Drawing.SystemColors.ControlText;
            l036.Location = new System.Drawing.Point(256, 65);
            l036.Name = "l036";
            l036.Size = new System.Drawing.Size(32, 15);
            l036.TabIndex = 317;
            l036.Text = "-";
            l036.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l046
            // 
            l046.BackColor = System.Drawing.SystemColors.Window;
            l046.BorderStyle = BorderStyle.FixedSingle;
            l046.ForeColor = System.Drawing.SystemColors.ControlText;
            l046.Location = new System.Drawing.Point(256, 81);
            l046.Name = "l046";
            l046.Size = new System.Drawing.Size(32, 15);
            l046.TabIndex = 316;
            l046.Text = "-";
            l046.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l145
            // 
            l145.BackColor = System.Drawing.SystemColors.Info;
            l145.BorderStyle = BorderStyle.FixedSingle;
            l145.ForeColor = System.Drawing.SystemColors.ControlText;
            l145.Location = new System.Drawing.Point(208, 259);
            l145.Name = "l145";
            l145.Size = new System.Drawing.Size(32, 15);
            l145.TabIndex = 313;
            l145.Text = "-";
            l145.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l125
            // 
            l125.BackColor = System.Drawing.SystemColors.Info;
            l125.BorderStyle = BorderStyle.FixedSingle;
            l125.ForeColor = System.Drawing.SystemColors.ControlText;
            l125.Location = new System.Drawing.Point(208, 227);
            l125.Name = "l125";
            l125.Size = new System.Drawing.Size(32, 15);
            l125.TabIndex = 310;
            l125.Text = "-";
            l125.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l065
            // 
            l065.BackColor = System.Drawing.SystemColors.Info;
            l065.BorderStyle = BorderStyle.FixedSingle;
            l065.ForeColor = System.Drawing.SystemColors.ControlText;
            l065.Location = new System.Drawing.Point(208, 119);
            l065.Name = "l065";
            l065.Size = new System.Drawing.Size(32, 15);
            l065.TabIndex = 308;
            l065.Text = "-";
            l065.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l035
            // 
            l035.BackColor = System.Drawing.SystemColors.Window;
            l035.BorderStyle = BorderStyle.FixedSingle;
            l035.ForeColor = System.Drawing.SystemColors.ControlText;
            l035.Location = new System.Drawing.Point(208, 65);
            l035.Name = "l035";
            l035.Size = new System.Drawing.Size(32, 15);
            l035.TabIndex = 303;
            l035.Text = "-";
            l035.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l045
            // 
            l045.BackColor = System.Drawing.SystemColors.Window;
            l045.BorderStyle = BorderStyle.FixedSingle;
            l045.ForeColor = System.Drawing.SystemColors.ControlText;
            l045.Location = new System.Drawing.Point(208, 81);
            l045.Name = "l045";
            l045.Size = new System.Drawing.Size(32, 15);
            l045.TabIndex = 302;
            l045.Text = "-";
            l045.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l144
            // 
            l144.BackColor = System.Drawing.SystemColors.Info;
            l144.BorderStyle = BorderStyle.FixedSingle;
            l144.ForeColor = System.Drawing.SystemColors.ControlText;
            l144.Location = new System.Drawing.Point(160, 259);
            l144.Name = "l144";
            l144.Size = new System.Drawing.Size(32, 15);
            l144.TabIndex = 299;
            l144.Text = "-";
            l144.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l124
            // 
            l124.BackColor = System.Drawing.SystemColors.Info;
            l124.BorderStyle = BorderStyle.FixedSingle;
            l124.ForeColor = System.Drawing.SystemColors.ControlText;
            l124.Location = new System.Drawing.Point(160, 227);
            l124.Name = "l124";
            l124.Size = new System.Drawing.Size(32, 15);
            l124.TabIndex = 296;
            l124.Text = "-";
            l124.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l084
            // 
            l084.BackColor = System.Drawing.SystemColors.Info;
            l084.BorderStyle = BorderStyle.FixedSingle;
            l084.ForeColor = System.Drawing.SystemColors.ControlText;
            l084.Location = new System.Drawing.Point(160, 151);
            l084.Name = "l084";
            l084.Size = new System.Drawing.Size(32, 15);
            l084.TabIndex = 292;
            l084.Text = "-";
            l084.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l034
            // 
            l034.BackColor = System.Drawing.SystemColors.Window;
            l034.BorderStyle = BorderStyle.FixedSingle;
            l034.ForeColor = System.Drawing.SystemColors.ControlText;
            l034.Location = new System.Drawing.Point(160, 65);
            l034.Name = "l034";
            l034.Size = new System.Drawing.Size(32, 15);
            l034.TabIndex = 289;
            l034.Text = "-";
            l034.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l044
            // 
            l044.BackColor = System.Drawing.SystemColors.Window;
            l044.BorderStyle = BorderStyle.FixedSingle;
            l044.ForeColor = System.Drawing.SystemColors.ControlText;
            l044.Location = new System.Drawing.Point(160, 81);
            l044.Name = "l044";
            l044.Size = new System.Drawing.Size(32, 15);
            l044.TabIndex = 288;
            l044.Text = "-";
            l044.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l113
            // 
            l113.BackColor = System.Drawing.SystemColors.Window;
            l113.BorderStyle = BorderStyle.FixedSingle;
            l113.ForeColor = System.Drawing.SystemColors.ControlText;
            l113.Location = new System.Drawing.Point(112, 205);
            l113.Name = "l113";
            l113.Size = new System.Drawing.Size(32, 15);
            l113.TabIndex = 283;
            l113.Text = "-";
            l113.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l123
            // 
            l123.BackColor = System.Drawing.SystemColors.Info;
            l123.BorderStyle = BorderStyle.FixedSingle;
            l123.ForeColor = System.Drawing.SystemColors.ControlText;
            l123.Location = new System.Drawing.Point(112, 227);
            l123.Name = "l123";
            l123.Size = new System.Drawing.Size(32, 15);
            l123.TabIndex = 282;
            l123.Text = "-";
            l123.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l043
            // 
            l043.BackColor = System.Drawing.SystemColors.Window;
            l043.BorderStyle = BorderStyle.FixedSingle;
            l043.ForeColor = System.Drawing.SystemColors.ControlText;
            l043.Location = new System.Drawing.Point(112, 81);
            l043.Name = "l043";
            l043.Size = new System.Drawing.Size(32, 15);
            l043.TabIndex = 274;
            l043.Text = "-";
            l043.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l112
            // 
            l112.BackColor = System.Drawing.SystemColors.Window;
            l112.BorderStyle = BorderStyle.FixedSingle;
            l112.ForeColor = System.Drawing.SystemColors.ControlText;
            l112.Location = new System.Drawing.Point(64, 205);
            l112.Name = "l112";
            l112.Size = new System.Drawing.Size(32, 15);
            l112.TabIndex = 269;
            l112.Text = "-";
            l112.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l062
            // 
            l062.BackColor = System.Drawing.SystemColors.Info;
            l062.BorderStyle = BorderStyle.FixedSingle;
            l062.ForeColor = System.Drawing.SystemColors.ControlText;
            l062.Location = new System.Drawing.Point(64, 119);
            l062.Name = "l062";
            l062.Size = new System.Drawing.Size(32, 15);
            l062.TabIndex = 266;
            l062.Text = "-";
            l062.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l121
            // 
            l121.BackColor = System.Drawing.SystemColors.Info;
            l121.BorderStyle = BorderStyle.FixedSingle;
            l121.ForeColor = System.Drawing.SystemColors.ControlText;
            l121.Location = new System.Drawing.Point(16, 227);
            l121.Name = "l121";
            l121.Size = new System.Drawing.Size(32, 15);
            l121.TabIndex = 254;
            l121.Text = "-";
            l121.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l061
            // 
            l061.BackColor = System.Drawing.SystemColors.Info;
            l061.BorderStyle = BorderStyle.FixedSingle;
            l061.ForeColor = System.Drawing.SystemColors.ControlText;
            l061.Location = new System.Drawing.Point(16, 119);
            l061.Name = "l061";
            l061.Size = new System.Drawing.Size(32, 15);
            l061.TabIndex = 252;
            l061.Text = "-";
            l061.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l031
            // 
            l031.BackColor = System.Drawing.SystemColors.Window;
            l031.BorderStyle = BorderStyle.FixedSingle;
            l031.ForeColor = System.Drawing.SystemColors.ControlText;
            l031.Location = new System.Drawing.Point(16, 65);
            l031.Name = "l031";
            l031.Size = new System.Drawing.Size(32, 15);
            l031.TabIndex = 247;
            l031.Text = "-";
            l031.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // l041
            // 
            l041.BackColor = System.Drawing.SystemColors.Window;
            l041.BorderStyle = BorderStyle.FixedSingle;
            l041.ForeColor = System.Drawing.SystemColors.ControlText;
            l041.Location = new System.Drawing.Point(16, 81);
            l041.Name = "l041";
            l041.Size = new System.Drawing.Size(32, 15);
            l041.TabIndex = 246;
            l041.Text = "-";
            l041.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbmgreco
            // 
            tbmgreco.BackColor = System.Drawing.Color.PaleGreen;
            tbmgreco.ForeColor = System.Drawing.SystemColors.ControlText;
            tbmgreco.Location = new System.Drawing.Point(249, 334);
            tbmgreco.MaxLength = 14;
            tbmgreco.Name = "tbmgreco";
            tbmgreco.Size = new System.Drawing.Size(48, 21);
            tbmgreco.TabIndex = 369;
            tbmgreco.Text = "0-14";
            tbmgreco.TextAlign = HorizontalAlignment.Center;
            // 
            // groupBox3
            // 
            groupBox3.BackColor = System.Drawing.Color.Bisque;
            groupBox3.Controls.Add(tbColR);
            groupBox3.Controls.Add(lFGR);
            groupBox3.Controls.Add(bFG);
            groupBox3.Controls.Add(lbCGR);
            groupBox3.Controls.Add(bMenosR);
            groupBox3.Controls.Add(bMasR);
            groupBox3.Controls.Add(bAnalizar);
            groupBox3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            groupBox3.ForeColor = System.Drawing.Color.Maroon;
            groupBox3.Location = new System.Drawing.Point(496, 360);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new System.Drawing.Size(248, 104);
            groupBox3.TabIndex = 361;
            groupBox3.TabStop = false;
            groupBox3.Text = "Análisis Resultados";
            // 
            // bFG
            // 
            bFG.BackColor = System.Drawing.Color.LightSalmon;
            bFG.FlatStyle = FlatStyle.Popup;
            bFG.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            bFG.ForeColor = System.Drawing.Color.Black;
            bFG.Image = ((System.Drawing.Image)(resources.GetObject("bFG.Image")));
            bFG.Location = new System.Drawing.Point(15, 22);
            bFG.Name = "bFG";
            bFG.Size = new System.Drawing.Size(24, 23);
            bFG.TabIndex = 87;
            bFG.UseVisualStyleBackColor = false;
            bFG.Click += new System.EventHandler(BFGClick);
            // 
            // bExporCols
            // 
            bExporCols.BackColor = System.Drawing.Color.DarkSalmon;
            bExporCols.FlatStyle = FlatStyle.Popup;
            bExporCols.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            bExporCols.Image = ((System.Drawing.Image)(resources.GetObject("bExporCols.Image")));
            bExporCols.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            bExporCols.Location = new System.Drawing.Point(546, 220);
            bExporCols.Name = "bExporCols";
            bExporCols.Size = new System.Drawing.Size(154, 32);
            bExporCols.TabIndex = 366;
            bExporCols.Text = "Exportar Columnas";
            bExporCols.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            bExporCols.UseVisualStyleBackColor = false;
            bExporCols.Click += new System.EventHandler(BExporColsClick);
            // 
            // lrangos
            // 
            lrangos.BackColor = System.Drawing.SystemColors.Info;
            lrangos.BorderStyle = BorderStyle.FixedSingle;
            lrangos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            lrangos.Location = new System.Drawing.Point(496, 104);
            lrangos.Name = "lrangos";
            lrangos.Size = new System.Drawing.Size(130, 22);
            lrangos.TabIndex = 367;
            lrangos.Text = "Fichero";
            lrangos.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // aidomnou
            // 
            AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            BackColor = System.Drawing.Color.Bisque;
            ClientSize = new System.Drawing.Size(760, 557);
            Controls.Add(lrangos);
            Controls.Add(bExporCols);
            Controls.Add(bLeeCondis);
            Controls.Add(bSalvaCondis);
            Controls.Add(lfile);
            Controls.Add(valors1);
            Controls.Add(groupBox3);
            Controls.Add(lTime);
            Controls.Add(bCancelar);
            Controls.Add(bCalcular);
            Controls.Add(lNota);
            Controls.Add(lColsIni);
            Controls.Add(lColsAdm);
            Controls.Add(bGrabaCols);
            Controls.Add(groupBox2);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "aidomnou";
            Text = "Filtro Aidomnou";
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);

		}
		
		void BGrabaColsClick(object sender, System.EventArgs e) { GrabaCols(); }
		void elmeuTimer(object sender, System.EventArgs e) { veureelmeu(); }
		void BCancelarClick(object sender, System.EventArgs e) { salida=true; }
		void BCalcularClick(object sender, System.EventArgs e) { Calcular(); }
		void BAnalizarClick(object sender, System.EventArgs e) { Analizar(); }
		void BFGClick(object sender, System.EventArgs e) { EntraCGsR(); }
		void BMasRClick(object sender, System.EventArgs e) { GRMas(); }
		void BMenosRClick(object sender, System.EventArgs e) { GRMenos(); }
		void Valors1Creacps(object sender, System.EventArgs e) {
			RecuperaPantalla(); PreparaColumnas(); PintaPantalla();
		}
		void BExporColsClick(object sender, System.EventArgs e) { ExporCols(); }
		void BSalvaCondisClick(object sender, System.EventArgs e) { SalvaCondis(); }
		void BLeeCondisClick(object sender, System.EventArgs e) { LeeCondis(); }
		
	}
}

