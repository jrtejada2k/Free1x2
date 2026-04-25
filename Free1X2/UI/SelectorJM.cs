using System;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using Free1X2;
using Free1X2.Utils;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI {
	public class SelecJM : System.Windows.Forms.Form {
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Button bMasR;
		private System.Windows.Forms.TextBox tbgr2;
		private System.Windows.Forms.TextBox tbgr3;
		private System.Windows.Forms.TextBox g12;
		private System.Windows.Forms.TextBox tbgr1;
		private System.Windows.Forms.TextBox g10;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.Label lfile;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button3;
		private System.Windows.Forms.TextBox g02;
		private System.Windows.Forms.TextBox g03;
		private System.Windows.Forms.TextBox g14;
		private System.Windows.Forms.TextBox g06;
		private System.Windows.Forms.TextBox g07;
		private System.Windows.Forms.TextBox g04;
		private System.Windows.Forms.TextBox g05;
		private System.Windows.Forms.Button bFG;
		private System.Windows.Forms.TextBox g08;
		private System.Windows.Forms.TextBox g09;
		private System.Windows.Forms.Button bAnalizar;
		private System.Windows.Forms.RadioButton rbvalor;
		private System.Windows.Forms.Label lbCGR;
		private System.Windows.Forms.Label angt;
		private System.Windows.Forms.Label lfcond;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button bGrabaCols;
		private System.Windows.Forms.TextBox tbtot;
		private System.Windows.Forms.Label lpc1;
		private System.Windows.Forms.Button bCalcular;
		private System.Windows.Forms.Label lvalidas;
		private System.Windows.Forms.Button bCancelar;
		private System.Windows.Forms.TextBox tbCG;
		private System.Windows.Forms.Label lpc2;
		private System.Windows.Forms.Label lpc3;
		private System.Windows.Forms.Label lFGR;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label ang1;
		private System.Windows.Forms.Label ang3;
		private System.Windows.Forms.Label ang2;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Button bLeeCondis;
		private System.Windows.Forms.TextBox g11;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.Label label23;
		private System.Windows.Forms.Label label22;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label24;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.TextBox g01;
		private System.Windows.Forms.TextBox g13;
		private System.Windows.Forms.Label label4;
		private Free1X2.UI.Controls.valors valors1;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.Label ltime;
		private System.Windows.Forms.Label lproc;
		private System.Windows.Forms.RadioButton rbcorte;
		private System.Windows.Forms.Button bMenosR;
		private System.Windows.Forms.Button bSalvaCondis;
		private System.Windows.Forms.Label label3;
		public SelecJM() {
			InitializeComponent();
			elmeu = new Timer();
			elmeu.Interval = 3000;
			elmeu.Tick += new EventHandler(elmeuTimer);
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
        }
 
		
		private bool salida = false;
		private DateTime dt0, dt9;
		private Timer elmeu;
		private double[,] nvals = new double[14,3];
		private double[,] wvals = new double[5,3];
		private int[] ngrups = new int[14];
		private double[] pg1 = new double[243];
		private int[] id1 = new int[243];
		private BitArray tablim1 = new BitArray(243);
		private double[] pg2 = new double[243];
		private int[] id2 = new int[243];
		private BitArray tablim2 = new BitArray(243);
		private double[] pg3 = new double[81];
		private int[] id3 = new int[81];
		private BitArray tablim3 = new BitArray(81);
		private BitArray tablimtt = new BitArray(567);
		private BitArray validas = new BitArray(4782969);
		private int ctproc, ctval, limcgsR, nrfCGR;
		private string[] colgsR = new string[300];
		
		private void SalvaCondis() {
			string archVal, tmp;
			SaveFileDialog saveValDialog = new SaveFileDialog();
			saveValDialog.InitialDirectory = "*\\";
			saveValDialog.Filter = "F.Condiciones(*.cnd)|*.cnd|Todos los archivos (*.*)|*.*" ;
			if(saveValDialog.ShowDialog() == DialogResult.OK) {
				archVal = Path.GetFileName(saveValDialog.FileName);
				StreamWriter sw = new StreamWriter(archVal);
				tmp = g01.Text+g02.Text+g03.Text+g04.Text+g05.Text+g06.Text+g07.Text;
				tmp += g08.Text+g09.Text+g10.Text+g11.Text+g12.Text+g13.Text+g14.Text;
				sw.WriteLine(tmp);
				sw.WriteLine(tbgr1.Text);
				sw.WriteLine(tbgr2.Text);
				sw.WriteLine(tbgr3.Text);
				sw.WriteLine(tbtot.Text);
				sw.WriteLine((rbcorte.Checked?"1":"0"));
				sw.Close();
				lfcond.Text = archVal;
			}
		}
		private void LeeCondis() {
			string tmp, archVal;
			string[] buff = new string[6];
			int nlin = 0;
			OpenFileDialog abreValIn = new OpenFileDialog();
			abreValIn.InitialDirectory = Application.StartupPath + "/";
			abreValIn.Filter = "F.Condiciones(*.cnd)|*.cnd|Todos los archivos(*.*)|*.*";
			if(abreValIn.ShowDialog() == DialogResult.OK) {
				archVal = Path.GetFileName(abreValIn.FileName);
				StreamReader srv = new StreamReader(archVal);
				while (srv.Peek()>0) {
					buff[nlin] = srv.ReadLine();
					nlin++;
				}
				srv.Close();
				lfcond.Text = archVal;
				if (nlin!=6) { MessageBox.Show("fichero de condiciones erróneo"); return; }
				tmp = buff[0];
				g01.Text = tmp[0].ToString(); g02.Text = tmp[1].ToString();
				g03.Text = tmp[2].ToString(); g04.Text = tmp[3].ToString();
				g05.Text = tmp[4].ToString(); g06.Text = tmp[5].ToString();
				g07.Text = tmp[6].ToString(); g08.Text = tmp[7].ToString();
				g09.Text = tmp[8].ToString(); g10.Text = tmp[9].ToString();
				g11.Text = tmp[10].ToString(); g12.Text = tmp[11].ToString();
				g13.Text = tmp[12].ToString(); g14.Text = tmp[13].ToString();
				tbgr1.Text = buff[1];
				tbgr2.Text = buff[2];
				tbgr3.Text = buff[3];
				tbtot.Text = buff[4];
				if (buff[5]=="0") rbvalor.Checked=true; else rbcorte.Checked=true;
			}
		}
		private void Calcular() {
			bCalcular.Enabled = false;
			bGrabaCols.Enabled = false;
			bGrabaCols.Visible = false;
			salida = false;
			elmeu.Start();
			dt0 = DateTime.Now;
			ltime.Text=lvalidas.Text=lproc.Text=lfile.Text = " ";
			RecuperaPantalla();
			Calgr1();
			Calgr2();
			Calgr3();
			if (rbcorte.Checked) BuscaCorte();
			else BuscaValor();
			elmeu.Stop();
			veureelmeu();
			bCalcular.Enabled = true;
			bGrabaCols.Enabled = true;
			bGrabaCols.Visible = true;
		}
		private void veureelmeu() {
			dt9 = DateTime.Now;
			string temp = (dt9-dt0).ToString()+"0000000000";
			ltime.Text = temp.Substring(0,10);
			lvalidas.Text = ""+ctval;
			lproc.Text = ""+ctproc;
		}
		private void GrabaCols() {
			string tmp, columna, fileout;
			int num; double val; char tab = (char) 9;
			StreamWriter wr = null;
			bCalcular.Enabled = false;
			bCalcular.Visible = false;
			bGrabaCols.Enabled = false;
			bGrabaCols.Text = "grabando";
			lfile.Text = "";
			wr = new StreamWriter("tablasJM.xls");
			wr.WriteLine("grupo-1");
			for (int nr=0; nr<243; nr++) {
				val = pg1[nr]; num = id1[nr]; tmp=n2s(num, 5);
				tmp = ""+(nr+1)+tab+tmp+tab+(""+val);
				wr.WriteLine(tmp);
			}
			wr.WriteLine("grupo-2");
			for (int nr=0; nr<243; nr++) {
				val = pg2[nr]; num = id2[nr]; tmp=n2s(num, 5);
				tmp = ""+(nr+1)+tab+tmp+tab+(""+val);
				wr.WriteLine(tmp);
			}
			wr.WriteLine("grupo-3");
			for (int nr=0; nr<81; nr++) {
				val = pg3[nr]; num = id3[nr]; tmp=n2s(num, 4);
				tmp = ""+(nr+1)+tab+tmp+tab+(""+val);
				wr.WriteLine(tmp);
			}
			wr.Close();
			SaveFileDialog resul = new SaveFileDialog();
			resul.InitialDirectory = ".\\" ;
			resul.Filter = "Resultados(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
			if(resul.ShowDialog() == DialogResult.OK) {
				Application.DoEvents();
				tmp = resul.FileName;
				fileout = Path.GetFileName(tmp);
				wr = new StreamWriter(fileout);
				for	(int nr=0; nr<4782969; nr++) {
					if (validas[nr]==true) {
						columna = n2s(nr,14);
						wr.WriteLine( columna );
					}
				}
				wr.Close();
				lfile.Text = fileout;
			}
			bGrabaCols.Text = "grabar columnas";
			bGrabaCols.Enabled = true;
			bCalcular.Enabled = true;
			bCalcular.Visible = true;
		}
		private void RecuperaPantalla() {
			string[] aux1 = null, aux2 = null;
			nvals = valors1.RetVals();
			for (int nr1=0; nr1<14; nr1++) {
				for (int nr2=0; nr2<3; nr2++) {
					if (nvals[nr1,nr2]==0) nvals[nr1,nr2]=1E-1;
				}
			}
			ngrups[0] = Convert.ToInt32(g01.Text);
			ngrups[1] = Convert.ToInt32(g02.Text);
			ngrups[2] = Convert.ToInt32(g03.Text);
			ngrups[3] = Convert.ToInt32(g04.Text);
			ngrups[4] = Convert.ToInt32(g05.Text);
			ngrups[5] = Convert.ToInt32(g06.Text);
			ngrups[6] = Convert.ToInt32(g07.Text);
			ngrups[7] = Convert.ToInt32(g08.Text);
			ngrups[8] = Convert.ToInt32(g09.Text);
			ngrups[9] = Convert.ToInt32(g10.Text);
			ngrups[10] = Convert.ToInt32(g11.Text);
			ngrups[11] = Convert.ToInt32(g12.Text);
			ngrups[12] = Convert.ToInt32(g13.Text);
			ngrups[13] = Convert.ToInt32(g14.Text);
			int n1,n2,n3;
			n1=n2=n3=0;
			for (int nr=0; nr<14; nr++) {
				switch (ngrups[nr]) {
						case 1: n1++; break;
						case 2: n2++; break;
						case 3: n3++; break;
				}
			}
			if (n1!=5 || n2!=5 || n3!=4) {
				MessageBox.Show("error en grupos");
				salida = true; return;
			}
			bool nerr = false;
			int ni, ns, nx; ni=ns=0;
			tablim1.SetAll(false);
			aux1=tbgr1.Text.Split(',');
			if (aux1.Length==0) nerr=true;
			for (int nr1=0; nr1<aux1.Length; nr1++) {
				aux2=aux1[nr1].Split('-');
				if (aux2.Length==0) continue;
				if (aux2.Length==1) {
					try { ni=Convert.ToInt32(aux2[0]); }
					catch { nerr=true; break; }
					if (ni<1) ni=1; if (ni>243) ni=243;
					tablim1[ni-1]=true;
				}
				else if (aux2.Length==2) {
					try { ni=Convert.ToInt32(aux2[0]); }
					catch { nerr=true; break; }
					try { ns=Convert.ToInt32(aux2[1]); }
					catch { nerr=true; break; }
					if (ni>ns) { nx=ni; ni=ns; ns=nx; }
					if (ni<1) ni=1; if (ni>243) ni=243;
					if (ns<1) ns=1; if (ns>243) ns=243;
					for (int nr=ni-1; nr<ns; nr++) tablim1[nr]=true;
				}
				else { nerr=true; break; }
			}
			tablim2.SetAll(false);
			aux1=tbgr2.Text.Split(',');
			if (aux1.Length==0) nerr=true;
			for (int nr1=0; nr1<aux1.Length; nr1++) {
				aux2=aux1[nr1].Split('-');
				if (aux2.Length==0) continue;
				if (aux2.Length==1) {
					try { ni=Convert.ToInt32(aux2[0]); }
					catch { nerr=true; break; }
					if (ni<1) ni=1; if (ni>243) ni=243;
					tablim2[ni-1]=true;
				}
				else if (aux2.Length==2) {
					try { ni=Convert.ToInt32(aux2[0]); }
					catch { nerr=true; break; }
					try { ns=Convert.ToInt32(aux2[1]); }
					catch { nerr=true; break; }
					if (ni>ns) { nx=ni; ni=ns; ns=nx; }
					if (ni<1) ni=1; if (ni>243) ni=243;
					if (ns<1) ns=1; if (ns>243) ns=243;
					for (int nr=ni-1; nr<ns; nr++) tablim2[nr]=true;
				}
				else { nerr=true; break; }
			}
			tablim3.SetAll(false);
			aux1=tbgr3.Text.Split(',');
			if (aux1.Length==0) nerr=true;
			for (int nr1=0; nr1<aux1.Length; nr1++) {
				aux2=aux1[nr1].Split('-');
				if (aux2.Length==0) continue;
				if (aux2.Length==1) {
					try { ni=Convert.ToInt32(aux2[0]); }
					catch { nerr=true; break; }
					if (ni<1) ni=1; if (ni>81) ni=81;
					tablim3[ni-1]=true;
				}
				else if (aux2.Length==2) {
					try { ni=Convert.ToInt32(aux2[0]); }
					catch { nerr=true; break; }
					try { ns=Convert.ToInt32(aux2[1]); }
					catch { nerr=true; break; }
					if (ni>ns) { nx=ni; ni=ns; ns=nx; }
					if (ni<1) ni=1; if (ni>81) ni=81;
					if (ns<1) ns=1; if (ns>81) ns=81;
					for (int nr=ni-1; nr<ns; nr++) tablim3[nr]=true;
				}
				else { nerr=true; break; }
			}
			tablimtt.SetAll(false);
			aux1=tbtot.Text.Split(',');
			if (aux1.Length==0) nerr=true;
			for (int nr1=0; nr1<aux1.Length; nr1++) {
				aux2=aux1[nr1].Split('-');
				if (aux2.Length==0) continue;
				if (aux2.Length==1) {
					try { ni=Convert.ToInt32(aux2[0]); }
					catch { nerr=true; break; }
					if (ni<1) ni=1; if (ni>567) ni=567;
					tablimtt[ni-1]=true;
				}
				else if (aux2.Length==2) {
					try { ni=Convert.ToInt32(aux2[0]); }
					catch { nerr=true; break; }
					try { ns=Convert.ToInt32(aux2[1]); }
					catch { nerr=true; break; }
					if (ni>ns) { nx=ni; ni=ns; ns=nx; }
					if (ni<1) ni=1; if (ni>567) ni=567;
					if (ns<1) ns=1; if (ns>567) ns=567;
					for (int nr=ni-1; nr<ns; nr++) tablimtt[nr]=true;
				}
				else { nerr=true; break; }
			}
			if (nerr) {
				MessageBox.Show("error en limites");
				salida = true;
			}
		}
		private void mxsort(double[] ls1, int[] ls2, int inf, int sup) {
			int s, t; double refer, tr1;
			for (int i=inf; i<sup; i++) {
				refer = tr1 = ls1[i]; s = i;
				for (int nr=i+1; nr<=sup; nr++) {
					if (ls1[nr]>refer) {
						s=nr; refer=ls1[s];
					}
				}
				ls1[i]=refer; ls1[s]=tr1;
				t=ls2[i]; ls2[i]=ls2[s]; ls2[s]=t;
			}
		}
		private void Calgr1() {
			for (int nr=0, nx=0; nr<14; nr++) {
				if (ngrups[nr]==1) {
					wvals[nx,1] = nvals[nr,0];
					wvals[nx,0] = nvals[nr,1];
					wvals[nx,2] = nvals[nr,2];
					nx++;
				}
			}
			int idx=0; double vl1, vl2, vl3, vl4, vl5;
			for (int n1=0; n1<3; n1++) {
				vl1 = wvals[0,n1];
				for (int n2=0; n2<3; n2++) {
					vl2 = vl1*wvals[1,n2];
					for (int n3=0; n3<3; n3++) {
						vl3 = vl2*wvals[2,n3];
						for (int n4=0; n4<3; n4++) {
							vl4 = vl3*wvals[3,n4];
							for (int n5=0; n5<3; n5++) {
								vl5 = vl4*wvals[4,n5];
								pg1[idx]=vl5;
								id1[idx]=idx;
								idx++;
							}
						}
					}
				}
			}
			mxsort(pg1, id1, 0, 242);
			double pc1=0;
			for (int nr=0; nr<243; nr++) {
				if (tablim1[nr]) pc1 += pg1[nr];
			}
			lpc1.Text = String.Format("{0:f2}",(pc1/1e8));
		}
		private void Calgr2() {
			for (int nr=0, nx=0; nr<14; nr++) {
				if (ngrups[nr]==2) {
					wvals[nx,1] = nvals[nr,0];
					wvals[nx,0] = nvals[nr,1];
					wvals[nx,2] = nvals[nr,2];
					nx++;
				}
			}
			int idx=0; double vl1, vl2, vl3, vl4, vl5;
			for (int n1=0; n1<3; n1++) {
				vl1 = wvals[0,n1];
				for (int n2=0; n2<3; n2++) {
					vl2 = vl1*wvals[1,n2];
					for (int n3=0; n3<3; n3++) {
						vl3 = vl2*wvals[2,n3];
						for (int n4=0; n4<3; n4++) {
							vl4 = vl3*wvals[3,n4];
							for (int n5=0; n5<3; n5++) {
								vl5 = vl4*wvals[4,n5];
								pg2[idx]=vl5;
								id2[idx]=idx;
								idx++;
							}
						}
					}
				}
			}
			mxsort(pg2, id2, 0, 242);
			double pc2=0;
			for (int nr=0; nr<243; nr++) {
				if (tablim2[nr]) pc2 += pg2[nr];
			}
			lpc2.Text = String.Format("{0:f2}",(pc2/1e8));
		}
		private void Calgr3() {
			for (int nr=0, nx=0; nr<14; nr++) {
				if (ngrups[nr]==3) {
					wvals[nx,1] = nvals[nr,0];
					wvals[nx,0] = nvals[nr,1];
					wvals[nx,2] = nvals[nr,2];
					nx++;
				}
			}
			int idx=0; double vl1, vl2, vl3, vl4;
			for (int n1=0; n1<3; n1++) {
				vl1 = wvals[0,n1];
				for (int n2=0; n2<3; n2++) {
					vl2 = vl1*wvals[1,n2];
					for (int n3=0; n3<3; n3++) {
						vl3 = vl2*wvals[2,n3];
						for (int n4=0; n4<3; n4++) {
							vl4 = vl3*wvals[3,n4];
							pg3[idx]=vl4;
							id3[idx]=idx;
							idx++;
						}
					}
				}
			}
			mxsort(pg3, id3, 0, 80);
			double pc3=0;
			for (int nr=0; nr<81; nr++) {
				if (tablim3[nr]) pc3 += pg3[nr];
			}
			lpc3.Text = String.Format("{0:f2}",(pc3/1e6));
		}
		private int s2n(string ax, int lim) {
			int nx = 0;
			for (int nr=0; nr<lim; nr++) {
				nx *= 3;
				string ch = ax.Substring(nr,1);
				if (ch=="1") nx+=1;
				else if (ch=="2") nx+=2;
			}
			return nx;
		}
		private string n2s(int nx, int lim) {
			string ax = ""; int nx2;
			for (int nr=0; nr<lim; nr++) {
				nx2 = nx%3; nx /= 3;
				if (nx2==1) ax = "1"+ax;
				else if (nx2==2) ax = "2"+ax;
				else ax = "X"+ax;
			}
			return ax;
		}
		private void BuscaCorte() {
			string tmp14, col1, col2, col3, tmp1, tmp2, tmp3;
			validas.SetAll(false); ctval=ctproc=0;
			for (int nr1=0; nr1<243; nr1++) {
				Application.DoEvents();
				if (salida) break;
				if (tablim1[nr1]) {
					col1=n2s(id1[nr1],5);
					for (int nr2=0; nr2<243; nr2++) {
						if (tablim2[nr2]) {
							col2=n2s(id2[nr2],5);
							for (int nr3=0; nr3<81; nr3++) {
								if (tablim3[nr3] && tablimtt[nr1+nr2+nr3+2]) {
									col3=n2s(id3[nr3],4);
									tmp14=""; tmp1=col1; tmp2=col2; tmp3=col3;
									for (int nr4=0; nr4<14; nr4++) {
										if (ngrups[nr4]==1) {
											tmp14+=tmp1.Substring(0,1);
											tmp1=tmp1.Substring(1);
										}
										else if (ngrups[nr4]==2) {
											tmp14+=tmp2.Substring(0,1);
											tmp2=tmp2.Substring(1);
										}
										else {
											tmp14+=tmp3.Substring(0,1);
											tmp3=tmp3.Substring(1);
										}
									}
									validas[s2n(tmp14,14)]=true; ctval++;
								}
								ctproc++;
							}
						}
						else ctproc+=81;
					}
				}
				else ctproc+=19683;
			}
		}
		private void BuscaValor() {
			string tmp1;
			char[] aux = new char[14];
			char ch;
			double n1, n2, n3, nv;
			validas.SetAll(false); ctval=ctproc=0;
			for (int nr0=0; nr0<4782969; nr0++) {
				Application.DoEvents();
				if (salida) break;
				tmp1 = n2s(nr0,14);
				n1=n2=n3=1;
				for (int nr=0; nr<14; nr++) {
					ch = tmp1[nr];
					if (ch=='1') nv=nvals[nr,0];
					else if (ch=='2') nv=nvals[nr,2];
					else nv=nvals[nr,1];
					if (ngrups[nr]==1) n1*=nv;
					else if (ngrups[nr]==2) n2*=nv;
					else n3*=nv;
				}
				for (int nr1=0; nr1<243; nr1++) {
					if (pg1[nr1]==n1 && tablim1[nr1]) {
						for (int nr2=0; nr2<243; nr2++) {
							if (pg2[nr2]==n2 && tablim2[nr2]) {
								for (int nr3=0; nr3<81; nr3++) {
									if (pg3[nr3]==n3 && tablim3[nr3]) {
										if (tablimtt[nr1+nr2+nr3+2]) {
											if (validas[nr0]==false) {
												validas[nr0]=true; ctval++;
											}
										}
									}
								}
							}
						}
					}
				}
				ctproc++;
			}
		}
		private void Analizar() {
			if (rbcorte.Checked) AnaCorte();
			else AnaValor();
		}
		private void AnaCorte() {
			string columna, col1, col2, col3;
			char ch; int id, i1, i2, i3;
			RecuperaPantalla();
			Calgr1();
			Calgr2();
			Calgr3();
			columna = tbCG.Text.ToUpper();
			col1=col2=col3="";
			for (int nr=0; nr<14; nr++) {
				ch = columna[nr];
				id = ngrups[nr];
				if (id==1) col1+=ch;
				else if (id==2) col2+=ch;
				else col3+=ch;
			}
			i1=s2n(col1,5); i2=s2n(col2,5); i3=s2n(col3,4);
			for (int nr=0; nr<243; nr++) {
				if (id1[nr]==i1) { i1=nr; break; }
			}
			for (int nr=0; nr<243; nr++) {
				if (id2[nr]==i2) { i2=nr; break; }
			}
			for (int nr=0; nr<81; nr++) {
				if (id3[nr]==i3) { i3=nr; break; }
			}
			ang1.Text=""+(i1+1); ang2.Text=""+(i2+1); ang3.Text=""+(i3+1);
			angt.Text=""+(i1+i2+i3+3);
		}
		private void AnaValor() {
			string tmp1;
			char[] aux = new char[14];
			char ch;
			RecuperaPantalla();
			Calgr1();
			Calgr2();
			Calgr3();
			tmp1 = tbCG.Text;
			double n1, n2, n3, nv;
			n1=n2=n3=1;
			for (int nr=0; nr<14; nr++) {
				ch = tmp1[nr];
				if (ch=='1') nv=nvals[nr,0];
				else if (ch=='2') nv=nvals[nr,2];
				else nv=nvals[nr,1];
				if (nv==0) nv=1;
				if (ngrups[nr]==1) n1*=nv;
				else if (ngrups[nr]==2) n2*=nv;
				else n3*=nv;
			}
			int i1, i2, i3; i1=i2=i3=0;
			for (int nr=0; nr<243; nr++) {
				if (pg1[nr]==n1) { i1=nr; break; }
			}
			for (int nr=0; nr<243; nr++) {
				if (pg2[nr]==n2) { i2=nr; break; }
			}
			for (int nr=0; nr<81; nr++) {
				if (pg3[nr]==n3) { i3=nr; break; }
			}
			ang1.Text=""+(i1+1); ang2.Text=""+(i2+1); ang3.Text=""+(i3+1);
			angt.Text=""+(i1+i2+i3+3);
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
					tmp = Pral.Normaliza(sr.ReadLine());
					if (tmp.Length<14) { 
						MessageBox.Show("col.G. errónea="+(limcgsR+1));
						break;
					}
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

//		[STAThread]
//		public static void Main(string[] args) { Application.Run(new SelecJM()); }
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelecJM));
            this.label3 = new System.Windows.Forms.Label();
            this.bSalvaCondis = new System.Windows.Forms.Button();
            this.bMenosR = new System.Windows.Forms.Button();
            this.rbcorte = new System.Windows.Forms.RadioButton();
            this.lproc = new System.Windows.Forms.Label();
            this.ltime = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.valors1 = new Free1X2.UI.Controls.valors();
            this.label4 = new System.Windows.Forms.Label();
            this.g13 = new System.Windows.Forms.TextBox();
            this.g01 = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.label21 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.g11 = new System.Windows.Forms.TextBox();
            this.bLeeCondis = new System.Windows.Forms.Button();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.ang2 = new System.Windows.Forms.Label();
            this.ang3 = new System.Windows.Forms.Label();
            this.ang1 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.lFGR = new System.Windows.Forms.Label();
            this.lpc3 = new System.Windows.Forms.Label();
            this.lpc2 = new System.Windows.Forms.Label();
            this.tbCG = new System.Windows.Forms.TextBox();
            this.bCancelar = new System.Windows.Forms.Button();
            this.lvalidas = new System.Windows.Forms.Label();
            this.bCalcular = new System.Windows.Forms.Button();
            this.lpc1 = new System.Windows.Forms.Label();
            this.tbtot = new System.Windows.Forms.TextBox();
            this.bGrabaCols = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lfcond = new System.Windows.Forms.Label();
            this.angt = new System.Windows.Forms.Label();
            this.lbCGR = new System.Windows.Forms.Label();
            this.rbvalor = new System.Windows.Forms.RadioButton();
            this.bAnalizar = new System.Windows.Forms.Button();
            this.g09 = new System.Windows.Forms.TextBox();
            this.g08 = new System.Windows.Forms.TextBox();
            this.bFG = new System.Windows.Forms.Button();
            this.g05 = new System.Windows.Forms.TextBox();
            this.g04 = new System.Windows.Forms.TextBox();
            this.g07 = new System.Windows.Forms.TextBox();
            this.g06 = new System.Windows.Forms.TextBox();
            this.g14 = new System.Windows.Forms.TextBox();
            this.g03 = new System.Windows.Forms.TextBox();
            this.g02 = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.g12 = new System.Windows.Forms.TextBox();
            this.g10 = new System.Windows.Forms.TextBox();
            this.lfile = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.tbgr1 = new System.Windows.Forms.TextBox();
            this.tbgr2 = new System.Windows.Forms.TextBox();
            this.tbgr3 = new System.Windows.Forms.TextBox();
            this.bMasR = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(8, 72);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 24);
            this.label3.TabIndex = 56;
            this.label3.Text = "gr.3";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bSalvaCondis
            // 
            this.bSalvaCondis.BackColor = System.Drawing.Color.DarkSalmon;
            this.bSalvaCondis.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bSalvaCondis.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.bSalvaCondis.ForeColor = System.Drawing.Color.Black;
            this.bSalvaCondis.Image = ((System.Drawing.Image)(resources.GetObject("bSalvaCondis.Image")));
            this.bSalvaCondis.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bSalvaCondis.Location = new System.Drawing.Point(100, 265);
            this.bSalvaCondis.Name = "bSalvaCondis";
            this.bSalvaCondis.Size = new System.Drawing.Size(122, 32);
            this.bSalvaCondis.TabIndex = 51;
            this.bSalvaCondis.Text = "Salvar condiciones";
            this.bSalvaCondis.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bSalvaCondis.UseVisualStyleBackColor = false;
            this.bSalvaCondis.Click += new System.EventHandler(this.BSalvaCondisClick);
            // 
            // bMenosR
            // 
            this.bMenosR.BackColor = System.Drawing.Color.DarkSalmon;
            this.bMenosR.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bMenosR.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bMenosR.Location = new System.Drawing.Point(245, 36);
            this.bMenosR.Name = "bMenosR";
            this.bMenosR.Size = new System.Drawing.Size(16, 16);
            this.bMenosR.TabIndex = 85;
            this.bMenosR.Text = "-";
            this.bMenosR.UseVisualStyleBackColor = false;
            this.bMenosR.Click += new System.EventHandler(this.BMenosRClick);
            // 
            // rbcorte
            // 
            this.rbcorte.Checked = true;
            this.rbcorte.Location = new System.Drawing.Point(8, 24);
            this.rbcorte.Name = "rbcorte";
            this.rbcorte.Size = new System.Drawing.Size(96, 24);
            this.rbcorte.TabIndex = 0;
            this.rbcorte.TabStop = true;
            this.rbcorte.Text = "por corte";
            // 
            // lproc
            // 
            this.lproc.BackColor = System.Drawing.SystemColors.Info;
            this.lproc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lproc.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lproc.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lproc.ForeColor = System.Drawing.Color.Black;
            this.lproc.Location = new System.Drawing.Point(552, 49);
            this.lproc.Name = "lproc";
            this.lproc.Size = new System.Drawing.Size(116, 24);
            this.lproc.TabIndex = 366;
            this.lproc.Text = "Procesadas";
            this.lproc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ltime
            // 
            this.ltime.BackColor = System.Drawing.SystemColors.Info;
            this.ltime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ltime.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ltime.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ltime.ForeColor = System.Drawing.Color.Black;
            this.ltime.Location = new System.Drawing.Point(552, 101);
            this.ltime.Name = "ltime";
            this.ltime.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ltime.Size = new System.Drawing.Size(116, 24);
            this.ltime.TabIndex = 54;
            this.ltime.Text = "Tiempo";
            this.ltime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label19
            // 
            this.label19.Location = new System.Drawing.Point(8, 114);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(24, 17);
            this.label19.TabIndex = 62;
            this.label19.Text = "6";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // valors1
            // 
            this.valors1.BackColor = System.Drawing.Color.Bisque;
            this.valors1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.valors1.Location = new System.Drawing.Point(8, 0);
            this.valors1.Name = "valors1";
            this.valors1.Size = new System.Drawing.Size(176, 484);
            this.valors1.TabIndex = 365;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(8, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 24);
            this.label4.TabIndex = 380;
            this.label4.Text = "tot.";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // g13
            // 
            this.g13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.g13.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.g13.Location = new System.Drawing.Point(32, 251);
            this.g13.Name = "g13";
            this.g13.Size = new System.Drawing.Size(32, 18);
            this.g13.TabIndex = 53;
            this.g13.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // g01
            // 
            this.g01.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.g01.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.g01.Location = new System.Drawing.Point(32, 17);
            this.g01.Name = "g01";
            this.g01.Size = new System.Drawing.Size(32, 18);
            this.g01.TabIndex = 41;
            this.g01.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(8, 133);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(24, 17);
            this.label16.TabIndex = 63;
            this.label16.Text = "7";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(8, 17);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(24, 17);
            this.label11.TabIndex = 55;
            this.label11.Text = "1";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label24
            // 
            this.label24.Location = new System.Drawing.Point(8, 173);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(24, 17);
            this.label24.TabIndex = 69;
            this.label24.Text = "9";
            this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(8, 36);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(24, 17);
            this.label13.TabIndex = 58;
            this.label13.Text = "2";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label22
            // 
            this.label22.Location = new System.Drawing.Point(8, 192);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(24, 17);
            this.label22.TabIndex = 65;
            this.label22.Text = "10";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label23
            // 
            this.label23.Location = new System.Drawing.Point(8, 211);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(24, 17);
            this.label23.TabIndex = 66;
            this.label23.Text = "11";
            this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label20
            // 
            this.label20.Location = new System.Drawing.Point(8, 232);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(24, 17);
            this.label20.TabIndex = 67;
            this.label20.Text = "12";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label21
            // 
            this.label21.Location = new System.Drawing.Point(8, 251);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(24, 17);
            this.label21.TabIndex = 68;
            this.label21.Text = "13";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label18
            // 
            this.label18.Location = new System.Drawing.Point(8, 95);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(24, 17);
            this.label18.TabIndex = 61;
            this.label18.Text = "5";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // g11
            // 
            this.g11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.g11.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.g11.Location = new System.Drawing.Point(32, 211);
            this.g11.Name = "g11";
            this.g11.Size = new System.Drawing.Size(32, 18);
            this.g11.TabIndex = 51;
            this.g11.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bLeeCondis
            // 
            this.bLeeCondis.BackColor = System.Drawing.Color.DarkSalmon;
            this.bLeeCondis.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bLeeCondis.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.bLeeCondis.ForeColor = System.Drawing.Color.Black;
            this.bLeeCondis.Image = ((System.Drawing.Image)(resources.GetObject("bLeeCondis.Image")));
            this.bLeeCondis.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bLeeCondis.Location = new System.Drawing.Point(100, 207);
            this.bLeeCondis.Name = "bLeeCondis";
            this.bLeeCondis.Size = new System.Drawing.Size(122, 32);
            this.bLeeCondis.TabIndex = 52;
            this.bLeeCondis.Text = "Leer condiciones";
            this.bLeeCondis.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bLeeCondis.UseVisualStyleBackColor = false;
            this.bLeeCondis.Click += new System.EventHandler(this.BLeeCondisClick);
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(8, 74);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(24, 17);
            this.label15.TabIndex = 60;
            this.label15.Text = "4";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(8, 55);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(24, 17);
            this.label14.TabIndex = 59;
            this.label14.Text = "3";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label17
            // 
            this.label17.Location = new System.Drawing.Point(8, 152);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(24, 17);
            this.label17.TabIndex = 64;
            this.label17.Text = "8";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ang2
            // 
            this.ang2.BackColor = System.Drawing.SystemColors.Info;
            this.ang2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ang2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ang2.Location = new System.Drawing.Point(168, 48);
            this.ang2.Name = "ang2";
            this.ang2.Size = new System.Drawing.Size(48, 23);
            this.ang2.TabIndex = 378;
            this.ang2.Text = "-";
            this.ang2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ang3
            // 
            this.ang3.BackColor = System.Drawing.SystemColors.Info;
            this.ang3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ang3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ang3.Location = new System.Drawing.Point(168, 72);
            this.ang3.Name = "ang3";
            this.ang3.Size = new System.Drawing.Size(48, 23);
            this.ang3.TabIndex = 379;
            this.ang3.Text = "-";
            this.ang3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ang1
            // 
            this.ang1.BackColor = System.Drawing.SystemColors.Info;
            this.ang1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ang1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ang1.Location = new System.Drawing.Point(168, 24);
            this.ang1.Name = "ang1";
            this.ang1.Size = new System.Drawing.Size(48, 23);
            this.ang1.TabIndex = 377;
            this.ang1.Text = "-";
            this.ang1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(8, 270);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(24, 17);
            this.label12.TabIndex = 56;
            this.label12.Text = "14";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lFGR
            // 
            this.lFGR.BackColor = System.Drawing.SystemColors.Info;
            this.lFGR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lFGR.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lFGR.Location = new System.Drawing.Point(61, 24);
            this.lFGR.Name = "lFGR";
            this.lFGR.Size = new System.Drawing.Size(144, 24);
            this.lFGR.TabIndex = 88;
            this.lFGR.Text = "Fichero ganadoras";
            this.lFGR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lpc3
            // 
            this.lpc3.BackColor = System.Drawing.SystemColors.Info;
            this.lpc3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lpc3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lpc3.Location = new System.Drawing.Point(152, 128);
            this.lpc3.Name = "lpc3";
            this.lpc3.Size = new System.Drawing.Size(48, 24);
            this.lpc3.TabIndex = 70;
            this.lpc3.Text = "%3";
            this.lpc3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lpc2
            // 
            this.lpc2.BackColor = System.Drawing.SystemColors.Info;
            this.lpc2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lpc2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lpc2.Location = new System.Drawing.Point(96, 128);
            this.lpc2.Name = "lpc2";
            this.lpc2.Size = new System.Drawing.Size(48, 24);
            this.lpc2.TabIndex = 69;
            this.lpc2.Text = "%2";
            this.lpc2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbCG
            // 
            this.tbCG.BackColor = System.Drawing.SystemColors.Window;
            this.tbCG.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbCG.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbCG.Location = new System.Drawing.Point(61, 49);
            this.tbCG.MaxLength = 14;
            this.tbCG.Name = "tbCG";
            this.tbCG.Size = new System.Drawing.Size(144, 21);
            this.tbCG.TabIndex = 373;
            this.tbCG.Text = "Col. ganadora";
            this.tbCG.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bCancelar
            // 
            this.bCancelar.BackColor = System.Drawing.Color.DarkSalmon;
            this.bCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bCancelar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bCancelar.ForeColor = System.Drawing.Color.Black;
            this.bCancelar.Image = ((System.Drawing.Image)(resources.GetObject("bCancelar.Image")));
            this.bCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bCancelar.Location = new System.Drawing.Point(552, 126);
            this.bCancelar.Name = "bCancelar";
            this.bCancelar.Size = new System.Drawing.Size(116, 32);
            this.bCancelar.TabIndex = 51;
            this.bCancelar.Text = "Cancelar";
            this.bCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bCancelar.UseVisualStyleBackColor = false;
            this.bCancelar.Click += new System.EventHandler(this.BCancelarClick);
            // 
            // lvalidas
            // 
            this.lvalidas.BackColor = System.Drawing.SystemColors.Info;
            this.lvalidas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lvalidas.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lvalidas.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lvalidas.ForeColor = System.Drawing.Color.Black;
            this.lvalidas.Location = new System.Drawing.Point(552, 75);
            this.lvalidas.Name = "lvalidas";
            this.lvalidas.Size = new System.Drawing.Size(116, 24);
            this.lvalidas.TabIndex = 53;
            this.lvalidas.Text = "Válidas";
            this.lvalidas.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bCalcular
            // 
            this.bCalcular.BackColor = System.Drawing.Color.DarkSalmon;
            this.bCalcular.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bCalcular.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bCalcular.ForeColor = System.Drawing.Color.Black;
            this.bCalcular.Image = ((System.Drawing.Image)(resources.GetObject("bCalcular.Image")));
            this.bCalcular.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bCalcular.Location = new System.Drawing.Point(552, 16);
            this.bCalcular.Name = "bCalcular";
            this.bCalcular.Size = new System.Drawing.Size(116, 32);
            this.bCalcular.TabIndex = 50;
            this.bCalcular.Text = "Calcular";
            this.bCalcular.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bCalcular.UseVisualStyleBackColor = false;
            this.bCalcular.Click += new System.EventHandler(this.BCalcularClick);
            // 
            // lpc1
            // 
            this.lpc1.BackColor = System.Drawing.SystemColors.Info;
            this.lpc1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lpc1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lpc1.Location = new System.Drawing.Point(40, 128);
            this.lpc1.Name = "lpc1";
            this.lpc1.Size = new System.Drawing.Size(48, 24);
            this.lpc1.TabIndex = 68;
            this.lpc1.Text = "%1";
            this.lpc1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbtot
            // 
            this.tbtot.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbtot.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbtot.Location = new System.Drawing.Point(56, 96);
            this.tbtot.Name = "tbtot";
            this.tbtot.Size = new System.Drawing.Size(104, 21);
            this.tbtot.TabIndex = 381;
            this.tbtot.Text = "1-567";
            this.tbtot.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bGrabaCols
            // 
            this.bGrabaCols.BackColor = System.Drawing.Color.DarkSalmon;
            this.bGrabaCols.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bGrabaCols.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bGrabaCols.ForeColor = System.Drawing.Color.Black;
            this.bGrabaCols.Image = ((System.Drawing.Image)(resources.GetObject("bGrabaCols.Image")));
            this.bGrabaCols.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bGrabaCols.Location = new System.Drawing.Point(552, 208);
            this.bGrabaCols.Name = "bGrabaCols";
            this.bGrabaCols.Size = new System.Drawing.Size(116, 32);
            this.bGrabaCols.TabIndex = 52;
            this.bGrabaCols.Text = "Grabar";
            this.bGrabaCols.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bGrabaCols.UseVisualStyleBackColor = false;
            this.bGrabaCols.Click += new System.EventHandler(this.BGrabaColsClick);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(8, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 24);
            this.label1.TabIndex = 54;
            this.label1.Text = "gr.1";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(8, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 24);
            this.label2.TabIndex = 55;
            this.label2.Text = "gr.2";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lfcond
            // 
            this.lfcond.BackColor = System.Drawing.SystemColors.Info;
            this.lfcond.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lfcond.ForeColor = System.Drawing.Color.Black;
            this.lfcond.Location = new System.Drawing.Point(100, 240);
            this.lfcond.Name = "lfcond";
            this.lfcond.Size = new System.Drawing.Size(122, 24);
            this.lfcond.TabIndex = 60;
            this.lfcond.Text = "Fichero";
            this.lfcond.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // angt
            // 
            this.angt.BackColor = System.Drawing.SystemColors.Info;
            this.angt.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.angt.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.angt.Location = new System.Drawing.Point(168, 96);
            this.angt.Name = "angt";
            this.angt.Size = new System.Drawing.Size(48, 23);
            this.angt.TabIndex = 382;
            this.angt.Text = "-";
            this.angt.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbCGR
            // 
            this.lbCGR.BackColor = System.Drawing.SystemColors.Info;
            this.lbCGR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbCGR.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCGR.Location = new System.Drawing.Point(211, 24);
            this.lbCGR.Name = "lbCGR";
            this.lbCGR.Size = new System.Drawing.Size(32, 24);
            this.lbCGR.TabIndex = 86;
            this.lbCGR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // rbvalor
            // 
            this.rbvalor.Location = new System.Drawing.Point(8, 48);
            this.rbvalor.Name = "rbvalor";
            this.rbvalor.Size = new System.Drawing.Size(96, 24);
            this.rbvalor.TabIndex = 1;
            this.rbvalor.Text = "por valor";
            // 
            // bAnalizar
            // 
            this.bAnalizar.BackColor = System.Drawing.Color.DarkSalmon;
            this.bAnalizar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bAnalizar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bAnalizar.ForeColor = System.Drawing.Color.Black;
            this.bAnalizar.Image = ((System.Drawing.Image)(resources.GetObject("bAnalizar.Image")));
            this.bAnalizar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bAnalizar.Location = new System.Drawing.Point(93, 81);
            this.bAnalizar.Name = "bAnalizar";
            this.bAnalizar.Size = new System.Drawing.Size(104, 32);
            this.bAnalizar.TabIndex = 67;
            this.bAnalizar.Text = "Analizar";
            this.bAnalizar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bAnalizar.UseVisualStyleBackColor = false;
            this.bAnalizar.Click += new System.EventHandler(this.BAnalizarClick);
            // 
            // g09
            // 
            this.g09.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.g09.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.g09.Location = new System.Drawing.Point(32, 173);
            this.g09.Name = "g09";
            this.g09.Size = new System.Drawing.Size(32, 18);
            this.g09.TabIndex = 49;
            this.g09.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // g08
            // 
            this.g08.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.g08.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.g08.Location = new System.Drawing.Point(32, 152);
            this.g08.Name = "g08";
            this.g08.Size = new System.Drawing.Size(32, 18);
            this.g08.TabIndex = 48;
            this.g08.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bFG
            // 
            this.bFG.BackColor = System.Drawing.Color.DarkSalmon;
            this.bFG.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bFG.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bFG.Image = ((System.Drawing.Image)(resources.GetObject("bFG.Image")));
            this.bFG.Location = new System.Drawing.Point(36, 24);
            this.bFG.Name = "bFG";
            this.bFG.Size = new System.Drawing.Size(24, 24);
            this.bFG.TabIndex = 87;
            this.bFG.UseVisualStyleBackColor = false;
            this.bFG.Click += new System.EventHandler(this.BFGClick);
            // 
            // g05
            // 
            this.g05.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.g05.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.g05.Location = new System.Drawing.Point(32, 95);
            this.g05.Name = "g05";
            this.g05.Size = new System.Drawing.Size(32, 18);
            this.g05.TabIndex = 45;
            this.g05.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // g04
            // 
            this.g04.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.g04.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.g04.Location = new System.Drawing.Point(32, 74);
            this.g04.Name = "g04";
            this.g04.Size = new System.Drawing.Size(32, 18);
            this.g04.TabIndex = 44;
            this.g04.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // g07
            // 
            this.g07.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.g07.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.g07.Location = new System.Drawing.Point(32, 133);
            this.g07.Name = "g07";
            this.g07.Size = new System.Drawing.Size(32, 18);
            this.g07.TabIndex = 47;
            this.g07.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // g06
            // 
            this.g06.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.g06.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.g06.Location = new System.Drawing.Point(32, 114);
            this.g06.Name = "g06";
            this.g06.Size = new System.Drawing.Size(32, 18);
            this.g06.TabIndex = 46;
            this.g06.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // g14
            // 
            this.g14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.g14.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.g14.Location = new System.Drawing.Point(32, 270);
            this.g14.Name = "g14";
            this.g14.Size = new System.Drawing.Size(32, 18);
            this.g14.TabIndex = 54;
            this.g14.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // g03
            // 
            this.g03.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.g03.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.g03.Location = new System.Drawing.Point(32, 55);
            this.g03.Name = "g03";
            this.g03.Size = new System.Drawing.Size(32, 18);
            this.g03.TabIndex = 43;
            this.g03.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // g02
            // 
            this.g02.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.g02.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.g02.Location = new System.Drawing.Point(32, 36);
            this.g02.Name = "g02";
            this.g02.Size = new System.Drawing.Size(32, 18);
            this.g02.TabIndex = 42;
            this.g02.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.NavajoWhite;
            this.button3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(37, 24);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(24, 24);
            this.button3.TabIndex = 87;
            this.button3.Text = "L";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.BFGClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbvalor);
            this.groupBox1.Controls.Add(this.rbcorte);
            this.groupBox1.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox1.Location = new System.Drawing.Point(232, 216);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(112, 80);
            this.groupBox1.TabIndex = 366;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Examinar";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label24);
            this.groupBox2.Controls.Add(this.label20);
            this.groupBox2.Controls.Add(this.label21);
            this.groupBox2.Controls.Add(this.label22);
            this.groupBox2.Controls.Add(this.label23);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.label19);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.g14);
            this.groupBox2.Controls.Add(this.g13);
            this.groupBox2.Controls.Add(this.g12);
            this.groupBox2.Controls.Add(this.g11);
            this.groupBox2.Controls.Add(this.g10);
            this.groupBox2.Controls.Add(this.g09);
            this.groupBox2.Controls.Add(this.g08);
            this.groupBox2.Controls.Add(this.g07);
            this.groupBox2.Controls.Add(this.g06);
            this.groupBox2.Controls.Add(this.g05);
            this.groupBox2.Controls.Add(this.g04);
            this.groupBox2.Controls.Add(this.g03);
            this.groupBox2.Controls.Add(this.g02);
            this.groupBox2.Controls.Add(this.g01);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox2.Location = new System.Drawing.Point(8, 22);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(80, 300);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Grupos";
            // 
            // g12
            // 
            this.g12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.g12.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.g12.Location = new System.Drawing.Point(32, 232);
            this.g12.Name = "g12";
            this.g12.Size = new System.Drawing.Size(32, 18);
            this.g12.TabIndex = 52;
            this.g12.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // g10
            // 
            this.g10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.g10.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.g10.Location = new System.Drawing.Point(32, 192);
            this.g10.Name = "g10";
            this.g10.Size = new System.Drawing.Size(32, 18);
            this.g10.TabIndex = 50;
            this.g10.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lfile
            // 
            this.lfile.BackColor = System.Drawing.SystemColors.Info;
            this.lfile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lfile.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lfile.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lfile.ForeColor = System.Drawing.Color.Black;
            this.lfile.Location = new System.Drawing.Point(552, 241);
            this.lfile.Name = "lfile";
            this.lfile.Size = new System.Drawing.Size(116, 24);
            this.lfile.TabIndex = 59;
            this.lfile.Text = "Fichero";
            this.lfile.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lfcond);
            this.groupBox4.Controls.Add(this.groupBox5);
            this.groupBox4.Controls.Add(this.bLeeCondis);
            this.groupBox4.Controls.Add(this.bSalvaCondis);
            this.groupBox4.Controls.Add(this.groupBox2);
            this.groupBox4.Controls.Add(this.groupBox1);
            this.groupBox4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox4.Location = new System.Drawing.Point(184, 8);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(352, 332);
            this.groupBox4.TabIndex = 56;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Condiciones";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.angt);
            this.groupBox5.Controls.Add(this.tbtot);
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Controls.Add(this.tbgr1);
            this.groupBox5.Controls.Add(this.tbgr2);
            this.groupBox5.Controls.Add(this.tbgr3);
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Controls.Add(this.ang1);
            this.groupBox5.Controls.Add(this.ang2);
            this.groupBox5.Controls.Add(this.ang3);
            this.groupBox5.Controls.Add(this.lpc3);
            this.groupBox5.Controls.Add(this.lpc1);
            this.groupBox5.Controls.Add(this.lpc2);
            this.groupBox5.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox5.Location = new System.Drawing.Point(112, 24);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(232, 168);
            this.groupBox5.TabIndex = 53;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Límites";
            // 
            // tbgr1
            // 
            this.tbgr1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbgr1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbgr1.Location = new System.Drawing.Point(56, 24);
            this.tbgr1.Name = "tbgr1";
            this.tbgr1.Size = new System.Drawing.Size(104, 21);
            this.tbgr1.TabIndex = 65;
            this.tbgr1.Text = "1-243";
            this.tbgr1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbgr2
            // 
            this.tbgr2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbgr2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbgr2.Location = new System.Drawing.Point(56, 48);
            this.tbgr2.Name = "tbgr2";
            this.tbgr2.Size = new System.Drawing.Size(104, 21);
            this.tbgr2.TabIndex = 66;
            this.tbgr2.Text = "1-243";
            this.tbgr2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // tbgr3
            // 
            this.tbgr3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tbgr3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbgr3.Location = new System.Drawing.Point(56, 72);
            this.tbgr3.Name = "tbgr3";
            this.tbgr3.Size = new System.Drawing.Size(104, 21);
            this.tbgr3.TabIndex = 67;
            this.tbgr3.Text = "1-81";
            this.tbgr3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bMasR
            // 
            this.bMasR.BackColor = System.Drawing.Color.DarkSalmon;
            this.bMasR.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bMasR.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bMasR.Location = new System.Drawing.Point(245, 19);
            this.bMasR.Name = "bMasR";
            this.bMasR.Size = new System.Drawing.Size(16, 16);
            this.bMasR.TabIndex = 84;
            this.bMasR.Text = "+";
            this.bMasR.UseVisualStyleBackColor = false;
            this.bMasR.Click += new System.EventHandler(this.BMasRClick);
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
            this.groupBox3.Controls.Add(this.button3);
            this.groupBox3.Controls.Add(this.bAnalizar);
            this.groupBox3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox3.Location = new System.Drawing.Point(240, 364);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(296, 128);
            this.groupBox3.TabIndex = 364;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Análisis Resultados";
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.SystemColors.Info;
            this.label6.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.Maroon;
            this.label6.Location = new System.Drawing.Point(552, 364);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(116, 120);
            this.label6.TabIndex = 55;
            this.label6.Text = "Debe haber 5 partidos en cada uno de los grupos 1 y 2; y 4 partidos en el grupo 3" +
                "";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SelecJM
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(7, 16);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(680, 504);
            this.Controls.Add(this.lproc);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.lfile);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.bCalcular);
            this.Controls.Add(this.bCancelar);
            this.Controls.Add(this.bGrabaCols);
            this.Controls.Add(this.ltime);
            this.Controls.Add(this.lvalidas);
            this.Controls.Add(this.valors1);
            this.Controls.Add(this.label6);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SelecJM";
            this.Text = "Selección 5+5+4 de JuanM";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion
		
		void BLeeCondisClick(object sender, System.EventArgs e) { LeeCondis(); }
		void BSalvaCondisClick(object sender, System.EventArgs e) { SalvaCondis(); }
		void BCalcularClick(object sender, System.EventArgs e) { Calcular(); }
		void BCancelarClick(object sender, System.EventArgs e) { salida=true; }
		void BGrabaColsClick(object sender, System.EventArgs e) { GrabaCols(); }
		void elmeuTimer(object sender, System.EventArgs e) { veureelmeu(); }
		void BAnalizarClick(object sender, System.EventArgs e) { Analizar(); }
		void BFGClick(object sender, System.EventArgs e) { EntraCGsR(); }
		void BMasRClick(object sender, System.EventArgs e) { GRMas(); }
		void BMenosRClick(object sender, System.EventArgs e) { GRMenos(); }
		
	}
}
