using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using System.Data;

namespace Free1X2.UI {
	public class Coincidencias : System.Windows.Forms.Form {
		private System.Windows.Forms.Button bCalR;
		private System.Windows.Forms.Label lcolsout;
		private System.Windows.Forms.Label lTime;
		private System.Windows.Forms.Button bAMC1;
		private System.Windows.Forms.Button bAMC2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.DataGrid tabout;
		private System.Windows.Forms.Label lfileconds;
		private System.Windows.Forms.Label lFGR;
		private System.Windows.Forms.DataGrid tabrad;
		private System.Windows.Forms.Label lbAMC;
		private System.Windows.Forms.Button bMas;
		private System.Windows.Forms.Label lbCGA;
		private System.Windows.Forms.Label ltColR;
		private System.Windows.Forms.Button bLeerCondis;
		private System.Windows.Forms.Button bCalcular;
		private System.Windows.Forms.Button bAnalizar;
		private System.Windows.Forms.Label ltColA;
		private System.Windows.Forms.Label lbCGR;
		private System.Windows.Forms.Label lFGA;
		private System.Windows.Forms.Button bMenos;
		private System.Windows.Forms.Label lColsAdm;
		private System.Windows.Forms.Label lcolsin;
		private System.Windows.Forms.Button bMasR;
		private System.Windows.Forms.Button bCancelar;
		private System.Windows.Forms.Button bIR;
		private System.Windows.Forms.Button bExcel;
		private System.Windows.Forms.Button bFGA;
		private System.Windows.Forms.Button bSalvarCondis;
		private System.Windows.Forms.Label lfileout;
		private System.Windows.Forms.Label lfilein;
		private System.Windows.Forms.Button bEntrada;
		private System.Windows.Forms.Label lColsProc;
		private System.Windows.Forms.Button bFGR;
		private System.Windows.Forms.Button bMenosR;
		private System.Windows.Forms.Button bGrabar;
		private System.Windows.Forms.DataGrid tabcond;
		public Coincidencias() {
			InitializeComponent();
			InitTexs();
			InitTabRad();
			InitDsRadio();
			InitTabOut();
			InitDsOut();
			InitTabCond();
			InitDsCondis();
			elmeu = new Timer();
			elmeu.Interval = 3000;
			elmeu.Tick += new EventHandler(elmeuTimer);
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}
		
		private bool salida = false;
		private Timer elmeu = new Timer();
		private DateTime dt0, dt9;
		private int ctproc, ctadm;
		private BitArray entrada = new BitArray(4782969);
		private BitArray validas = new BitArray(4782969);
		private BitArray coin = new BitArray(108);
		private bool[,] GrN = new bool[100,109];
		private bool[,] rks = new bool[100,109];
		private int limGrN;
		private int[] condis = new int[108];
		private int[] rsls = new int[108];
		private int[] agrabarAC = new int[109];
		private int[] agrabarGR = new int[109];
		private int[,] agrabarAMC = new int[100,109];
		private int[] agrabarCOL = new int[4782969];
		private string[] texs = new string[108];
		private int limcgsA=0, limcgsR=0, nrfCGA, nrfCGR;
		private string[] colgsA = new string[3000];
		private string[] colgsR = new string[3000];
		private DataGridTableStyle tablain = new DataGridTableStyle();
		private DataGridTableStyle tablaout = new DataGridTableStyle();
		private DataGridTableStyle tablacondis = new DataGridTableStyle();
		private DataSet dsRadio, dsOut, dsCondis;
		private int xfil=0, xgan1=0, xcon=0, xgan2=0, xpro=0, xgra=0, xana=0;
		
		private void InitTabRad() {
			tablain.MappingName = "Grupos";
			DataGridTextBoxColumn cs = null;
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "C";
			cs.HeaderText = "C.";
			cs.Width = 30;
			cs.ReadOnly = true;
			cs.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
			tablain.GridColumnStyles.Add(cs);
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "D";
			cs.HeaderText = "desc.";
			cs.Width = 65;
			cs.ReadOnly = true;
			cs.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
			tablain.GridColumnStyles.Add(cs);
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "R";
			cs.HeaderText = "R";
			cs.Width = 25;
			cs.ReadOnly = true;
			cs.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
			tablain.GridColumnStyles.Add(cs);
			tabrad.TableStyles.Clear();
			tabrad.TableStyles.Add(tablain);
		}
		private void InitDsRadio() {
			DataRow row;
			dsRadio = new DataSet();
			DataTable newTable = new DataTable("Grupos");
			newTable.Columns.Add("C", typeof(int));
			newTable.Columns.Add("D", typeof(string));
			newTable.Columns.Add("R", typeof(int));
			dsRadio.Tables.Add(newTable);
			for (int nr=0; nr<108; nr++) {
				row = dsRadio.Tables["Grupos"].NewRow();
				row["C"] = nr+1;
				row["D"] = texs[nr];
				row["R"] = condis[nr];
				dsRadio.Tables["Grupos"].Rows.Add(row);
			}
			tabrad.SetDataBinding(dsRadio, "Grupos");
		}
		private void InitTexs() {
			texs[0] = "V14"; texs[1] = "X14"; texs[2] = "D14"; texs[3] = "V7P"; texs[4] = "X7P";
			texs[5] = "D7P"; texs[6] = "V7U"; texs[7] = "X7U"; texs[8] = "D7U"; texs[9] = "SSV14";
			texs[10] = "SSU14"; texs[11] = "SSX14"; texs[12] = "SSD14"; texs[13] = "SSV7P";
			texs[14] = "SSU7P"; texs[15] = "SSX7P"; texs[16] = "SSD7P"; texs[17] = "SSV7U";
			texs[18] = "SSU7U"; texs[19] = "SSX7U"; texs[20] = "SSD7U"; texs[21] = "disV14";
			texs[22] = "disU14"; texs[23] = "disX14"; texs[24] = "disD14"; texs[25] = "disV7P";
			texs[26] = "disU7P"; texs[27] = "disX7P"; texs[28] = "disD7P"; texs[29] = "disV7U";
			texs[30] = "disU7U"; texs[31] = "disX7U"; texs[32] = "disD7U"; texs[33] = "con(1x)14";
			texs[34] = "con(12)14"; texs[35] = "con(x2)14"; texs[36] = "con(11)14"; texs[37] = "con(xx)14";
			texs[38] = "con(22)14"; texs[39] = "con(1v)14"; texs[40] = "con(xv)14"; texs[41] = "con(2v)14";
			texs[42] = "con(vv)14"; texs[43] = "con(1x)7P"; texs[44] = "con(12)7P"; texs[45] = "con(x2)7P";
			texs[46] = "con(11)7P"; texs[47] = "con(xx)7P"; texs[48] = "con(22)7P"; texs[49] = "con(1v)7P";
			texs[50] = "con(xv)7P"; texs[51] = "con(2v)7P"; texs[52] = "con(vv)7P"; texs[53] = "con(1x)7U";
			texs[54] = "con(12)7U"; texs[55] = "con(x2)7U"; texs[56] = "con(11)7U"; texs[57] = "con(xx)7U";
			texs[58] = "con(22)7U"; texs[59] = "con(1v)7U"; texs[60] = "con(xv)7U"; texs[61] = "con(2v)7U";
			texs[62] = "con(vv)7U"; texs[63] = "PNG14"; texs[64] = "PNV14"; texs[65] = "PNU14";
			texs[66] = "PNX14"; texs[67] = "PND14"; texs[68] = "PNG7P"; texs[69] = "PNV7P";
			texs[70] = "PNU7P"; texs[71] = "PNX7P"; texs[72] = "PND7P"; texs[73] = "PNG7U";
			texs[74] = "PNV7U"; texs[75] = "PNU7U"; texs[76] = "PNX7U"; texs[77] = "PND7U";
			texs[78] = "IG14"; texs[79] = "IV14"; texs[80] = "IU14"; texs[81] = "IX14"; texs[82] = "ID14";
			texs[83] = "IG7P"; texs[84] = "IV7P"; texs[85] = "IU7P"; texs[86] = "IX7P"; texs[87] = "ID7P";
			texs[88] = "IG7U"; texs[89] = "IV7U"; texs[90] = "IU7U"; texs[91] = "IX7U"; texs[92] = "ID7U";
			texs[93] = "ISG14"; texs[94] = "ISV14"; texs[95] = "ISU14"; texs[96] = "ISX14";
			texs[97] = "ISD14"; texs[98] = "ISG7P"; texs[99] = "ISV7P"; texs[100] = "ISU7P";
			texs[101] = "ISX7P"; texs[102] = "ISD7P"; texs[103] = "ISG7U"; texs[104] = "ISV7U";
			texs[105] = "ISU7U"; texs[106] = "ISX7U"; texs[107] = "ISD7U";
		}
		private void Calcondis(string col) {
			col = col.Substring(0,14);
			for (int nr=0; nr<108; nr++) rsls[nr]=0;
			CalVX2(col); CalSS(col); CalDist(col); CalContac(col); CalPN(col); CalInterr(col);
		}
		private void CalVX2(string columna) {
			int nv7p, nx7p, n27p, nv7u, nx7u, n27u;
			char ch;
			nv7p=nv7u=nx7p=nx7u=n27p=n27u=0;
			for (int nr=0; nr<7; nr++) {
				ch=columna[nr];
				if (ch=='1') {}
				else if (ch=='2') {nv7p++; n27p++;}
				else {nv7p++; nx7p++;}
			}
			for (int nr=7; nr<14; nr++) {
				ch=columna[nr];
				if (ch=='1') {}
				else if (ch=='2') {nv7u++; n27u++;}
				else {nv7u++; nx7u++;}
			}
			rsls[0]=nv7p+nv7u; rsls[1]=nx7p+nx7u; rsls[2]=n27p+n27u;
			rsls[3]=nv7p; rsls[4]=nx7p; rsls[5]=n27p;
			rsls[6]=nv7u; rsls[7]=nx7u; rsls[8]=n27u;
		}
		private void CalSS(string columna) {
			int nv, nu, nx, nd, mv, mu, mx, md;
			char ch; string tmp;
			for (int pas=0; pas<3; pas++){
				if (pas==0) tmp=columna;
				else if (pas==1) tmp=columna.Substring(0,7);
				else tmp=columna.Substring(7);
				nv=nu=nx=nd=mv=mu=mx=md=0;
				for (int nr=0; nr<tmp.Length; nr++) {
					ch = tmp[nr];
					if (ch!='1') { nv++; if (nv>mv) mv=nv; } else nv=0;
					if (ch=='1') { nu++; if (nu>mu) mu=nu; } else nu=0;
					if (ch=='X') { nx++; if (nx>mx) mx=nx; } else nx=0;
					if (ch=='2') { nd++; if (nd>md) md=nd; } else nd=0;
				}
				if (pas==0) {
					rsls[9]=mv; rsls[10]=mu; rsls[11]=mx; rsls[12]=md;
				} else if (pas==1) {
					rsls[13]=mv; rsls[14]=mu; rsls[15]=mx; rsls[16]=md;
				} else {
					rsls[17]=mv; rsls[18]=mu; rsls[19]=mx; rsls[20]=md;
				}
			}
		}
		private void CalDist(string columna) {
			int act, ant, dv, du, dx, dd;
			string tmp;
			for (int pas=0; pas<3; pas++){
				if (pas==0) tmp=columna;
				else if (pas==1) tmp=columna.Substring(0,7);
				else tmp=columna.Substring(7);
				dv=du=dx=dd=0;
				ant=tmp.IndexOf ('1');
				do {
					act=tmp.IndexOf ('1',ant+1);
					if(act >=0) {
						du=Math.Max (act-ant,du);
						ant=act;
					}
				} while (act>=0);
				ant=tmp.IndexOf ('X');
				do {
					act=tmp.IndexOf ('X',ant+1);
					if(act >=0) {
						dx=Math.Max (act-ant,dx);
						ant=act;
					}
				} while (act>=0);
				ant=tmp.IndexOf ('2');
				do {
					act=tmp.IndexOf ('2',ant+1);
					if(act >=0) {
						dd=Math.Max (act-ant,dd);
						ant=act;
					}
				} while (act>=0);
				tmp=tmp.Replace('X','2');
				ant=tmp.IndexOf ('2');
				do {
					act=tmp.IndexOf ('2',ant+1);
					if(act >=0) {
						dv=Math.Max (act-ant,dv);
						ant=act;
					}
				} while (act>=0);
				if (pas==0) {
					rsls[21]=dv; rsls[22]=du; rsls[23]=dx; rsls[24]=dd;
				} else if (pas==1) {
					rsls[25]=dv; rsls[26]=du; rsls[27]=dx; rsls[28]=dd;
				} else {
					rsls[29]=dv; rsls[30]=du; rsls[31]=dx; rsls[32]=dd;
				}
			}
		}
		private void CalContac(string columna) {
			int n1x, n12, nx2, n11, nxx, n22, n1v, nxv, n2v, nvv;
			string tmp;
			for (int pas=0; pas<3; pas++){
				if (pas==0) tmp=columna.Substring(0,14);
				else if (pas==1) tmp=columna.Substring(0,7);
				else tmp=columna.Substring(7,7);
				n1x=n12=nx2=n11=nxx=n22=n1v=nxv=n2v=nvv=0;
				for (int nr=0; nr<(tmp.Length-1); nr++) {
					switch (tmp.Substring(nr,2)) {
						case "1X":
							case "X1": n1x++; n1v++; break;
						case "12":
							case "21": n12++; n1v++; break;
						case "X2":
							case "2X": nx2++; nvv++; nxv++; n2v++; break;
							case "11": n11++; break;
							case "XX": nxx++; nvv++; nxv++; break;
							case "22": n22++; nvv++; n2v++; break;
					}
				}
				if (pas==0) {
					rsls[33]=n1x; rsls[34]=n12; rsls[35]=nx2; rsls[36]=n11; rsls[37]=nxx;
					rsls[38]=n22; rsls[39]=n1v; rsls[40]=nxv; rsls[41]=n2v; rsls[42]=nvv;
				} else if (pas==1) {
					rsls[43]=n1x; rsls[44]=n12; rsls[45]=nx2; rsls[46]=n11; rsls[47]=nxx;
					rsls[48]=n22; rsls[49]=n1v; rsls[50]=nxv; rsls[51]=n2v; rsls[52]=nvv;
				} else {
					rsls[53]=n1x; rsls[54]=n12; rsls[55]=nx2; rsls[56]=n11; rsls[57]=nxx;
					rsls[58]=n22; rsls[59]=n1v; rsls[60]=nxv; rsls[61]=n2v; rsls[62]=nvv;
				}
			}
		}
		private void CalPN(string columna) {
			int pesoGlobal, pesoUnos, pesoVar, pesoEquis, pesoDoses;
			int[] indicesUnos =  new int[]{7,5,3,1,8,6,4,2,9,7,5,3,1,8};
			int[] indicesEquis = new int[]{5,1,6,2,7,3,8,4,9,5,1,6,2,7};
			int[] indicesDoses = new int[]{1,2,3,4,5,6,7,8,9,1,2,3,4,5};
			string tmp;
			int nSuma1, nSumaX, nSuma2;
			for (int pas=0; pas<3; pas++){
				if (pas==0) tmp=columna.Substring(0,14);
				else if (pas==1) tmp=columna.Substring(0,7);
				else tmp=columna.Substring(7,7);
				nSuma1 = nSumaX = nSuma2 = 0;
				for(int i = 0; i < tmp.Length; i++) {
					switch( tmp[i] ) {
							case '1': nSuma1 += indicesUnos[i]; break;
							case 'X': nSumaX += indicesEquis[i]; break;
							case '2': nSuma2 += indicesDoses[i]; break;
					}
				}
				pesoUnos = CalculaPeso( nSuma1 );
				pesoEquis = CalculaPeso( nSumaX );
				pesoDoses = CalculaPeso( nSuma2 );
				pesoVar = CalculaPeso( pesoEquis + pesoDoses );
				pesoGlobal = CalculaPeso( pesoVar + pesoUnos );
				if (pas==0) {
					rsls[63]=pesoGlobal; rsls[64]=pesoVar; rsls[65]=pesoUnos;
					rsls[66]=pesoEquis; rsls[67]=pesoDoses;
				} else if (pas==1) {
					rsls[68]=pesoGlobal; rsls[69]=pesoVar; rsls[70]=pesoUnos;
					rsls[71]=pesoEquis; rsls[72]=pesoDoses;
				} else {
					rsls[73]=pesoGlobal; rsls[74]=pesoVar; rsls[75]=pesoUnos;
					rsls[76]=pesoEquis; rsls[77]=pesoDoses;
				}
			}
		}
		private int CalculaPeso(int valor) {
			int peso = valor;
			int num1, num2;
			while( peso > 9) {
				num1 = peso/10;
				num2 = peso%10;
				peso = num1 + num2;
			}
			return peso;
		}
		private void CalInterr(string columna) {
			int noIntGlobal, noIntVar, noInt1, noIntX, noInt2 = 0;
			int noIntGlobalSeg, noIntVarSeg, noInt1Seg, noIntXSeg, noInt2Seg = 0;
			string tmp;
			int ngs, nvs, nus, nxs, nds;
			char act, ant;
			for (int pas=0; pas<3; pas++){
				if (pas==0) tmp=columna;
				else if (pas==1) tmp=columna.Substring(0,7);
				else tmp=columna.Substring(7);
				noIntGlobal = noIntVar = noInt1 = noIntX = noInt2 = 0;
				noIntGlobalSeg = noIntVarSeg = noInt1Seg = noIntXSeg = noInt2Seg = 0;
				ngs = nvs = nus = nxs = nds = 0;
				ant = tmp[0];
				for (int i=1; i < tmp.Length; i++) {
					act = tmp[i];
					if (act==ant) {
						if (ngs>noIntGlobalSeg) noIntGlobalSeg=ngs;
						if (nus>noInt1Seg) noInt1Seg=nus;
						if (nds>noInt2Seg) noInt2Seg=nds;
						if (nxs>noIntXSeg) noIntXSeg=nxs;
						if (nvs>noIntVarSeg) noIntVarSeg=nvs;
						ngs = nus = nds = nxs = nvs = 0;
					}
					else {
						noIntGlobal++; ngs++;
						if (ant=='1') {
							if (act=='2') {
								noInt1++; nus++;
								if (nxs>noIntXSeg) noIntXSeg=nxs;
								nxs = 0;
							}
							else {
								noInt1++; nus++;
								if (nds>noInt2Seg) noInt2Seg=nds;
								nds = 0;
							}
						}
						else if (ant=='2') {
							if (act=='1') {
								noInt2++; noIntVar++; nds++; nvs++;
								if (nxs>noIntXSeg) noIntXSeg=nxs;
								nxs = 0;
							}
							else {
								noInt2++; nds++;
								if (nus>noInt1Seg) noInt1Seg=nus;
								if (nvs>noIntVarSeg) noIntVarSeg=nvs;
								nus = nvs=0;
							}
						}
						else {
							if (act=='1') {
								noIntX++; noIntVar++; nxs++; nvs++;
								if (nds>noInt2Seg) noInt2Seg=nds;
								nds = 0;
							}
							else {
								noIntX++; nxs++;
								if (nus>noInt1Seg) noInt1Seg=nus;
								if (nvs>noIntVarSeg) noIntVarSeg=nvs;
								nus = nvs=0;
							}
						}
					}
					ant = act;
				}
				if (ngs>noIntGlobalSeg) noIntGlobalSeg=ngs;
				if (nus>noInt1Seg) noInt1Seg=nus;
				if (nds>noInt2Seg) noInt2Seg=nds;
				if (nxs>noIntXSeg) noIntXSeg=nxs;
				if (nvs>noIntVarSeg) noIntVarSeg=nvs;
				if (pas==0) {
					rsls[78]=noIntGlobal; rsls[93]=noIntGlobalSeg;
					rsls[79]=noIntVar; rsls[94]=noIntVarSeg;
					rsls[80]=noInt1; rsls[95]=noInt1Seg;
					rsls[81]=noIntX; rsls[96]=noIntXSeg;
					rsls[82]=noInt2; rsls[97]=noInt2Seg;
				} else if (pas==1) {
					rsls[83]=noIntGlobal; rsls[98]=noIntGlobalSeg;
					rsls[84]=noIntVar; rsls[99]=noIntVarSeg;
					rsls[85]=noInt1; rsls[100]=noInt1Seg;
					rsls[86]=noIntX; rsls[101]=noIntXSeg;
					rsls[87]=noInt2; rsls[102]=noInt2Seg;
				} else {
					rsls[88]=noIntGlobal; rsls[103]=noIntGlobalSeg;
					rsls[89]=noIntVar; rsls[104]=noIntVarSeg;
					rsls[90]=noInt1; rsls[105]=noInt1Seg;
					rsls[91]=noIntX; rsls[106]=noIntXSeg;
					rsls[92]=noInt2; rsls[107]=noInt2Seg;
				}
			}
		}
		private bool InitConds() {
			string tmp; string[] aux1, aux2;
			int n1, n2; limGrN=0;
			for (int nr=0; nr<100; nr++) {
				for (int nr2=0; nr2<109; nr2++) {
					GrN[nr,nr2]=false;
					rks[nr,nr2]=false;
				}
			}
			for (int nr=0; nr<1000; nr++) {
				try { tmp=tabcond[nr,0].ToString(); }
				catch { limGrN=nr; break; }
			}
			if (limGrN==0) return false;
			for (int nl=0; nl<limGrN; nl++) {
				tmp=tabcond[nl,0].ToString();
				aux1=tmp.Split(',');
				for (int nr2=0; nr2<aux1.Length; nr2++) {
					aux2 = aux1[nr2].Split('-');
					if (aux2.Length==1) {
						n1 = Convert.ToInt32(aux2[0]);
						GrN[nl,n1-1]=true;
					}
					else if (aux2.Length==2) {
						n1 = Convert.ToInt32(aux2[0]);
						n2 = Convert.ToInt32(aux2[1]);
						for (int nr3=n1-1; nr3<n2; nr3++) GrN[nl,nr3]=true;
					}
					else return false;
				}
				tmp=tabcond[nl,1].ToString();
				aux1=tmp.Split(',');
				for (int nl2=0; nl2<aux1.Length; nl2++) {
					aux2 = aux1[nl2].Split('-');
					if (aux2.Length==1) {
						n1 = Convert.ToInt32(aux2[0]);
						rks[nl,n1]=true;
					}
					else if (aux2.Length==2) {
						n1 = Convert.ToInt32(aux2[0]);
						n2 = Convert.ToInt32(aux2[1]);
						for (int nr3=n1; nr3<=n2; nr3++) rks[nl,nr3]=true;
					}
					else return false;
				}
			}
			return true;
		}
		private void LeerFileIn() {
			string columna; int idx;
			if (xpro>0) return;
			xfil=0;
			OpenFileDialog lee = new OpenFileDialog();
            lee.InitialDirectory = Application.StartupPath + "/";
			lee.Filter = "ColumnasEntrada(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
			if(lee.ShowDialog() == DialogResult.OK) {
				lfilein.Text="";
				bEntrada.Text="leyendo...";
				string filein = Path.GetFileName(lee.FileName);
				entrada.SetAll(false);
				StreamReader sr = new StreamReader(filein);
				while (sr.Peek()>0) {
					Application.DoEvents();
					if (salida) break;
					columna = sr.ReadLine().Trim().ToUpper(); ctproc++;
					if (columna.Length<14) { MessageBox.Show("error longitud="+columna); break; }
					idx = s2n(columna);
					entrada[idx]=true;
				}
				sr.Close();
				lfilein.Text=filein; lcolsin.Text=""+ctproc;
				bEntrada.Text="seleccionar f.entrada";
				xfil=1;
			}
		}
		private void CalRadioB() {
			string columna=colgsA[nrfCGA-1].Replace('x','X');
			InitTexs();
			Calcondis(columna);
			for (int nr=0; nr<108; nr++) condis[nr]=rsls[nr];
			InitDsRadio();
		}
		private void Grabar() {
			int nq = 0, ctgrab=0;
			if (xpro>0 || xana>0 || xgra>0) return;
			xgra=1;
			bGrabar.Text="grabando";
			Application.DoEvents();
			SaveFileDialog resul = new SaveFileDialog();
            resul.InitialDirectory = Application.StartupPath + "/";
			resul.Filter = "Resultados(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
			if(resul.ShowDialog() == DialogResult.OK) {
				string fileout = Path.GetFileName(resul.FileName);
				StreamWriter sw = new StreamWriter(fileout);
				for (int nr=0; nr<4782969; nr++) {
					Application.DoEvents();
					if (validas[nr]==true) {
						nq = agrabarCOL[nr];
						if ((bool)tabout[nq,1]) {
							sw.WriteLine(n2s(nr)); ctgrab++;
						}
					}
				}
				sw.Close();
				lfileout.Text=fileout; lcolsout.Text=""+ctgrab;
			}
			bGrabar.Text="grabar    resultado";
			xgra=0;
		}
		private void PrintRadio() {
			StreamWriter sw = new StreamWriter("radiografia.txt");
			sw.WriteLine("Radiografia de la columna = "+lbCGA.Text);
			for (int nr=0; nr<108; nr++) {
				sw.WriteLine(texs[nr]+" = "+condis[nr]);
			}
			sw.Close();
			sw = new StreamWriter("radioExcel.txt");			// para MarioSan
			for (int nr=0; nr<108; nr++) {
				if (nr==3 || nr==6 || nr==9 ||nr==13 || nr==17 || nr==21) sw.WriteLine(" ");
				else if (nr==25 || nr==29 || nr==33 ||nr==43 || nr==53 || nr==63) sw.WriteLine(" ");
				else if (nr==68 || nr==73 || nr==78 ||nr==83 || nr==88 || nr==93) sw.WriteLine(" ");
				else if (nr==98 || nr==103) sw.WriteLine(" ");
				if (nr==9 || nr==21 || nr==33 || nr==63 || nr==78 || nr==93) sw.WriteLine(" ");
				sw.WriteLine(condis[nr]);
			}
			sw.Close();
			MessageBox.Show("ver resultados en los ficheros : \nradiografia.txt y\nradioExcel.txt");
		}
		private void veureelmeu() {
			lColsProc.Text = ""+ctproc;
			lColsAdm.Text = ""+ctadm;
			dt9 = DateTime.Now;
			string temp = (dt9-dt0).ToString()+"0000000000";
			lTime.Text = temp.Substring(0,10);
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
		private void LeeCondis() {
			string tmp; string[] aux;
			xcon=0;
			OpenFileDialog condis = new OpenFileDialog();
            condis.InitialDirectory = Application.StartupPath + "/Columnas/";
			condis.Filter = "Condiciones(*.cnd)|*.cnd|Todos los archivos (*.*)|*.*";
			if(condis.ShowDialog() == DialogResult.OK) {
				Application.DoEvents();
				DataRow row;
				dsCondis.Tables["Condiciones"].Clear();
				string filein = Path.GetFileName(condis.FileName);
				StreamReader src = new StreamReader(filein);
				limGrN=0;
				while (src.Peek()>0) {
					tmp = src.ReadLine();
					aux = tmp.Split(';');
					row = dsCondis.Tables["Condiciones"].NewRow();
					row["G"] = aux[0];
					row["M"] = aux[1];
					row["R"] = 0;
					dsCondis.Tables["Condiciones"].Rows.Add(row);
					limGrN++;
				}
				src.Close();
				lfileconds.Text=filein;
			}
			xcon=1;
		}
		private void SalvaCondis() {
			SaveFileDialog resul = new SaveFileDialog();
            resul.InitialDirectory = Application.StartupPath + "/Columnas/";
            resul.Filter = "Condiciones(*.cnd)|*.cnd|Todos los archivos (*.*)|*.*";
			if(resul.ShowDialog() == DialogResult.OK) {
				Application.DoEvents();
				string fileout = Path.GetFileName(resul.FileName);
				StreamWriter swc = new StreamWriter(fileout);
				for (int nr=0; nr<1000; nr++) {
					try { swc.WriteLine(tabcond[nr,0]+";"+tabcond[nr,1]); }
					catch { limGrN=nr; break; }
				}
				swc.Close();
				lfileconds.Text=fileout;
			}
		}
		private void Analizar() {
			string columna;
			int na, st;
			xana = xfil*xgan1*xcon*xgan2; if (xana==0) return;
			na=st=0;
			if (limcgsA==0 || limcgsR==0) {
				MessageBox.Show("faltan ganadoras"); return;
			}
			if (!InitConds()) {
                MessageBox.Show("Error en Condiciones");
				return;
			}
			columna = colgsA[nrfCGA-1].Replace('x','X');
			Calcondis(columna);
			for (int nr=0; nr<108; nr++) condis[nr]=rsls[nr];
			columna = colgsR[nrfCGR-1].Replace('x','X');
			Calcondis(columna);
			coin.SetAll(false);
			for (int nr=0; nr<108; nr++) if (condis[nr]==rsls[nr]) coin[nr]=true;
			for (int nr=0; nr<limGrN; nr++) {
				na=0;
				for (int nr2=0; nr2<108; nr2++) {
					if (GrN[nr,nr2] && coin[nr2]) na++;
				}
				st+=na; tabcond[nr,2]=na; na=0;
			}
			xana=0;
		}
		private void EntraCGsR() {
			string tmp;
			xgan2=0;
			OpenFileDialog cgDialog = new OpenFileDialog();
            cgDialog.InitialDirectory = Application.StartupPath + "/Columnas/";
			cgDialog.Filter = "F.Ganadoras(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			if(cgDialog.ShowDialog() == DialogResult.OK) {
				string filein = Path.GetFileName(cgDialog.FileName);
				limcgsR = 0;
				StreamReader sr = new StreamReader(filein);
				while (sr.Peek()>0) {
					tmp = sr.ReadLine().Trim().ToUpper();
					if (tmp.Length<14) { MessageBox.Show("col.G. errónea="+tmp); return; }
					colgsR[limcgsR] = tmp.Substring(0,14);
					limcgsR++;
					Application.DoEvents();
				}
				sr.Close();
				nrfCGR = limcgsR; lFGR.Text = filein;
				lbCGR.Text=""+nrfCGR; ltColR.Text=colgsR[nrfCGR-1];
			}
			xgan2=1;
		}
		private void EntraCGsA() {
			string tmp;
			xgan1=0;
			OpenFileDialog cgDialog = new OpenFileDialog();
			cgDialog.InitialDirectory = Application.StartupPath + "/";
			cgDialog.Filter = "Ganadoras Anteriores(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			if(cgDialog.ShowDialog() == DialogResult.OK) {
				string filein = Path.GetFileName(cgDialog.FileName);
				limcgsA = 0;
				StreamReader sr = new StreamReader(filein);
				while (sr.Peek()>0) {
					tmp = sr.ReadLine().Trim().ToUpper();
					if (tmp.Length<14) { MessageBox.Show("col.G. errónea="+tmp); return; }
					colgsA[limcgsA] = tmp.Substring(0,14);
					limcgsA++;
					Application.DoEvents();
				}
				sr.Close();
				nrfCGA = limcgsA; lFGA.Text = filein;
				lbCGA.Text=""+nrfCGA; ltColA.Text=colgsA[nrfCGA-1];
			}
			xgan1=1;
		}
		private void GAMas() {
			if (nrfCGA<limcgsA) {
				nrfCGA++;
				lbCGA.Text=""+nrfCGA; ltColA.Text=colgsA[nrfCGA-1];
			}
		}
		private void GAMenos() {
			if (nrfCGA>1) {
				nrfCGA--;
				lbCGA.Text=""+nrfCGA; ltColA.Text=colgsA[nrfCGA-1];
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
		private void AMCmas() {
			int nact = Convert.ToInt32(lbAMC.Text);
			if (nact<limGrN) {
				nact++; lbAMC.Text=""+nact;
			}
			InitDsOut();
		}
		private void AMCmenos() {
			int nact = Convert.ToInt32(lbAMC.Text);
			if (nact>1) {
				nact--; lbAMC.Text=""+nact;
			}
			InitDsOut();
		}
		private void InitTabOut() {
			tablaout.MappingName = "Resultados";
			DataGridTextBoxColumn cs = null;
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "N";
			cs.HeaderText = "C";
			cs.Width = 30;
			cs.ReadOnly = true;
			cs.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
			tablaout.GridColumnStyles.Add(cs);
			DataGridBoolColumn csBool = new DataGridBoolColumn();
			csBool.MappingName = "S";
			csBool.HeaderText = "S";
			csBool.Width = 30;
			tablaout.GridColumnStyles.Add(csBool);
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "Q";
			cs.HeaderText = "s/AC";
			cs.Width = 58;
			cs.ReadOnly = true;
			cs.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
			tablaout.GridColumnStyles.Add(cs);
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "G";
			cs.HeaderText = "s/GR";
			cs.Width = 58;
			cs.ReadOnly = true;
			cs.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
			tablaout.GridColumnStyles.Add(cs);
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "M";
			cs.HeaderText = "AMC";
			cs.Width = 58;
			cs.ReadOnly = true;
			cs.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
			tablaout.GridColumnStyles.Add(cs);
			tabout.TableStyles.Clear();
			tabout.TableStyles.Add(tablaout);
		}
		private void InitDsOut() {
			int nAMC = Convert.ToInt32(lbAMC.Text)-1;
			DataRow row;
			dsOut = new DataSet();
			DataTable tabsal = new DataTable("Resultados");
			tabsal.Columns.Add("N", typeof(int));
			tabsal.Columns.Add("S", typeof(bool));
			tabsal.Columns.Add("Q", typeof(int));
			tabsal.Columns.Add("G", typeof(int));
			tabsal.Columns.Add("M", typeof(int));
			dsOut.Tables.Add(tabsal);
			for (int nr=0; nr<109; nr++) {
				row = dsOut.Tables["Resultados"].NewRow();
				row["N"] = nr;
				row["S"] = true;
				row["Q"] = agrabarAC[nr];
				row["G"] = agrabarGR[nr];
				row["M"] = agrabarAMC[nAMC,nr];
				dsOut.Tables["Resultados"].Rows.Add(row);
			}
			tabout.SetDataBinding(dsOut, "Resultados");
		}
		private void InitTabCond() {
			tablacondis.MappingName = "Condiciones";
			DataGridTextBoxColumn cs = null;
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "G";
			cs.HeaderText = "grupos";
			cs.Width = 82;
			cs.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
			tablacondis.GridColumnStyles.Add(cs);
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "M";
			cs.HeaderText = "rango";
			cs.Width = 60;
			cs.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
			tablacondis.GridColumnStyles.Add(cs);
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "R";
			cs.HeaderText = "rsl.";
			cs.Width = 40;
			cs.ReadOnly = true;
			cs.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
			tablacondis.GridColumnStyles.Add(cs);
			tabcond.TableStyles.Clear();
			tabcond.TableStyles.Add(tablacondis);
		}
		private void InitDsCondis() {
			dsCondis = new DataSet();
			DataTable tabcondis = new DataTable("Condiciones");
			tabcondis.Columns.Add("G", typeof(string));
			tabcondis.Columns.Add("M", typeof(string));
			tabcondis.Columns.Add("R", typeof(int));
			dsCondis.Tables.Add(tabcondis);
			tabcond.SetDataBinding(dsCondis, "Condiciones");
		}
		private void Calcular() {
			string columna, tmp;
			for (int nr=0; nr<1000; nr++) {
				try { tmp = tabcond[nr,0]+";"+tabcond[nr,1]; }
				catch { limGrN=nr; break; }
			}
			if (limGrN>0) xcon=1; else xcon=0;
			xpro = xfil*xgan1*xcon; if (xpro==0 || xana>0 || xgra>0) return;
			salida=false;
			elmeu.Start(); dt0 = DateTime.Now;
			bCalcular.Text="calcula...";
			lColsProc.Text=lColsAdm.Text=lTime.Text=lcolsout.Text=" ";
			ctproc=ctadm=0;
			InitTexs();
			if (!InitConds()) {
                MessageBox.Show("Error en Condiciones");
				return;
			}
			validas.SetAll(false);
			columna=colgsA[nrfCGA-1].Replace('x','X');
			Calcondis(columna);
			for (int nr=0; nr<108; nr++) condis[nr]=rsls[nr];
			InitDsRadio();
			for (int nr=0; nr<109; nr++) { agrabarAC[nr]=0; agrabarGR[nr]=0; }
			for (int nr=0; nr<100; nr++) {
				for (int nr2=0; nr2<109; nr2++) agrabarAMC[nr,nr2]=0;
			}
			for (int idx=0; idx<4782969; idx++) agrabarCOL[idx]=0;
			for (int idx=0; idx<4782969; idx++) {
				Application.DoEvents();
				if (salida) break;
				if (entrada[idx]) {
					columna=n2s(idx); ctproc++;
					Valida(columna);
				}
			}
			InitDsOut();
			elmeu.Stop(); veureelmeu();
			xpro=0;
			bCalcular.Text="calcular";
		}
		private void Valida(string columna) {
			int nc=0, idx=s2n(columna);
			Calcondis(columna);
			coin.SetAll(false);
			for (int nr=0; nr<108; nr++) if (condis[nr]==rsls[nr]) coin[nr]=true;
			for (int ng=0; ng<limGrN; ng++) {
				nc=0;
				for (int nr=0; nr<108; nr++) {
					if (GrN[ng,nr] && coin[nr]) nc++;
				}
				if (!rks[ng,nc]) return;
			}
			validas[idx]=true; ctadm++;
			agrabarCOL[idx]=nc;
			agrabarAC[nc]++;
			for (int nr=0; nr<108; nr++) if (coin[nr]) agrabarGR[nr+1]++;
			for (int ng=0; ng<limGrN; ng++) {
				nc=0;
				for (int nr=0; nr<109; nr++) {
					if (GrN[ng,nr] && coin[nr]) nc++;
				}
				agrabarAMC[ng,nc]++;
			}
		}
		private void GrabExcel() {
			char tab = (char) 9; string tmp;
			if (xpro>0 || xana>0 || xgra>0) return;
			xgra=1;
			bExcel.Text="grabando";
			Application.DoEvents();
			SaveFileDialog resul = new SaveFileDialog();
			resul.InitialDirectory = ".\\" ;
			resul.Filter = "Resultados(*.xls)|*.xls|Todos los archivos (*.*)|*.*";
			if(resul.ShowDialog() == DialogResult.OK) {
				string fileout = Path.GetFileName(resul.FileName);
				StreamWriter sw = new StreamWriter(fileout);
				tmp = "N"+tab+"s/AC"+tab+"N"+tab+"s/GR";
				for (int nr=0; nr<limGrN; nr++) tmp+=(""+tab+"N"+tab+"AMC"+(nr+1));
				sw.WriteLine(tmp);
				for (int nr=0; nr<109; nr++) {
					tmp = ""+nr+tab+agrabarAC[nr]+tab+nr+tab+agrabarGR[nr];
					for (int ng=0; ng<limGrN; ng++)
						tmp+=(""+tab+nr+tab+agrabarAMC[ng,nr]);
					sw.WriteLine(tmp);
				}
				sw.Close();
			}
			bExcel.Text="grabar     distribuciones         en EXCEL";
			xgra=0;
		}
				
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Coincidencias));
            this.tabcond = new System.Windows.Forms.DataGrid();
            this.bGrabar = new System.Windows.Forms.Button();
            this.bMenosR = new System.Windows.Forms.Button();
            this.bFGR = new System.Windows.Forms.Button();
            this.lColsProc = new System.Windows.Forms.Label();
            this.bEntrada = new System.Windows.Forms.Button();
            this.lfilein = new System.Windows.Forms.Label();
            this.lfileout = new System.Windows.Forms.Label();
            this.bSalvarCondis = new System.Windows.Forms.Button();
            this.bFGA = new System.Windows.Forms.Button();
            this.bExcel = new System.Windows.Forms.Button();
            this.bIR = new System.Windows.Forms.Button();
            this.bCancelar = new System.Windows.Forms.Button();
            this.bMasR = new System.Windows.Forms.Button();
            this.lcolsin = new System.Windows.Forms.Label();
            this.lColsAdm = new System.Windows.Forms.Label();
            this.bMenos = new System.Windows.Forms.Button();
            this.lFGA = new System.Windows.Forms.Label();
            this.lbCGR = new System.Windows.Forms.Label();
            this.ltColA = new System.Windows.Forms.Label();
            this.bAnalizar = new System.Windows.Forms.Button();
            this.bCalcular = new System.Windows.Forms.Button();
            this.bLeerCondis = new System.Windows.Forms.Button();
            this.ltColR = new System.Windows.Forms.Label();
            this.lbCGA = new System.Windows.Forms.Label();
            this.bMas = new System.Windows.Forms.Button();
            this.lbAMC = new System.Windows.Forms.Label();
            this.tabrad = new System.Windows.Forms.DataGrid();
            this.lFGR = new System.Windows.Forms.Label();
            this.lfileconds = new System.Windows.Forms.Label();
            this.tabout = new System.Windows.Forms.DataGrid();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.bAMC2 = new System.Windows.Forms.Button();
            this.bAMC1 = new System.Windows.Forms.Button();
            this.lTime = new System.Windows.Forms.Label();
            this.lcolsout = new System.Windows.Forms.Label();
            this.bCalR = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.tabcond)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabrad)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabout)).BeginInit();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabcond
            // 
            this.tabcond.CaptionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.tabcond.CaptionText = "Condiciones";
            this.tabcond.DataMember = "";
            this.tabcond.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.tabcond.Location = new System.Drawing.Point(8, 128);
            this.tabcond.Name = "tabcond";
            this.tabcond.RowHeaderWidth = 0;
            this.tabcond.Size = new System.Drawing.Size(220, 260);
            this.tabcond.TabIndex = 91;
            // 
            // bGrabar
            // 
            this.bGrabar.BackColor = System.Drawing.Color.DarkSalmon;
            this.bGrabar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bGrabar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bGrabar.Image = ((System.Drawing.Image)(resources.GetObject("bGrabar.Image")));
            this.bGrabar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bGrabar.Location = new System.Drawing.Point(427, 475);
            this.bGrabar.Name = "bGrabar";
            this.bGrabar.Size = new System.Drawing.Size(137, 29);
            this.bGrabar.TabIndex = 26;
            this.bGrabar.Text = "Grabar resultado";
            this.bGrabar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bGrabar.UseVisualStyleBackColor = false;
            this.bGrabar.Click += new System.EventHandler(this.BGrabarClick);
            // 
            // bMenosR
            // 
            this.bMenosR.BackColor = System.Drawing.Color.DarkSalmon;
            this.bMenosR.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bMenosR.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bMenosR.Location = new System.Drawing.Point(231, 53);
            this.bMenosR.Name = "bMenosR";
            this.bMenosR.Size = new System.Drawing.Size(16, 13);
            this.bMenosR.TabIndex = 85;
            this.bMenosR.Text = "-";
            this.bMenosR.UseVisualStyleBackColor = false;
            this.bMenosR.Click += new System.EventHandler(this.BMenosRClick);
            // 
            // bFGR
            // 
            this.bFGR.BackColor = System.Drawing.Color.DarkSalmon;
            this.bFGR.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bFGR.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bFGR.Image = ((System.Drawing.Image)(resources.GetObject("bFGR.Image")));
            this.bFGR.Location = new System.Drawing.Point(28, 38);
            this.bFGR.Name = "bFGR";
            this.bFGR.Size = new System.Drawing.Size(24, 21);
            this.bFGR.TabIndex = 87;
            this.bFGR.UseVisualStyleBackColor = false;
            this.bFGR.Click += new System.EventHandler(this.BFGRClick);
            // 
            // lColsProc
            // 
            this.lColsProc.BackColor = System.Drawing.SystemColors.Info;
            this.lColsProc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lColsProc.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lColsProc.Location = new System.Drawing.Point(266, 31);
            this.lColsProc.Name = "lColsProc";
            this.lColsProc.Size = new System.Drawing.Size(104, 21);
            this.lColsProc.TabIndex = 24;
            this.lColsProc.Text = "Procesadas";
            this.lColsProc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bEntrada
            // 
            this.bEntrada.BackColor = System.Drawing.Color.DarkSalmon;
            this.bEntrada.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bEntrada.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bEntrada.Image = ((System.Drawing.Image)(resources.GetObject("bEntrada.Image")));
            this.bEntrada.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bEntrada.Location = new System.Drawing.Point(7, 12);
            this.bEntrada.Name = "bEntrada";
            this.bEntrada.Size = new System.Drawing.Size(120, 21);
            this.bEntrada.TabIndex = 93;
            this.bEntrada.Text = "Fichero Entrada";
            this.bEntrada.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bEntrada.UseVisualStyleBackColor = false;
            this.bEntrada.Click += new System.EventHandler(this.BEntradaClick);
            // 
            // lfilein
            // 
            this.lfilein.BackColor = System.Drawing.SystemColors.Info;
            this.lfilein.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lfilein.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lfilein.Location = new System.Drawing.Point(7, 40);
            this.lfilein.Name = "lfilein";
            this.lfilein.Size = new System.Drawing.Size(217, 21);
            this.lfilein.TabIndex = 94;
            this.lfilein.Text = "Fichero entrada";
            this.lfilein.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lfileout
            // 
            this.lfileout.BackColor = System.Drawing.SystemColors.Info;
            this.lfileout.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lfileout.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lfileout.Location = new System.Drawing.Point(427, 505);
            this.lfileout.Name = "lfileout";
            this.lfileout.Size = new System.Drawing.Size(270, 20);
            this.lfileout.TabIndex = 96;
            this.lfileout.Text = "Fichero salida";
            this.lfileout.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bSalvarCondis
            // 
            this.bSalvarCondis.BackColor = System.Drawing.Color.DarkSalmon;
            this.bSalvarCondis.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bSalvarCondis.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bSalvarCondis.Image = ((System.Drawing.Image)(resources.GetObject("bSalvarCondis.Image")));
            this.bSalvarCondis.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bSalvarCondis.Location = new System.Drawing.Point(118, 392);
            this.bSalvarCondis.Name = "bSalvarCondis";
            this.bSalvarCondis.Size = new System.Drawing.Size(109, 30);
            this.bSalvarCondis.TabIndex = 30;
            this.bSalvarCondis.Text = "Grabar";
            this.bSalvarCondis.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bSalvarCondis.UseVisualStyleBackColor = false;
            this.bSalvarCondis.Click += new System.EventHandler(this.BSalvarCondisClick);
            // 
            // bFGA
            // 
            this.bFGA.BackColor = System.Drawing.Color.DarkSalmon;
            this.bFGA.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bFGA.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bFGA.Image = ((System.Drawing.Image)(resources.GetObject("bFGA.Image")));
            this.bFGA.Location = new System.Drawing.Point(7, 80);
            this.bFGA.Name = "bFGA";
            this.bFGA.Size = new System.Drawing.Size(24, 21);
            this.bFGA.TabIndex = 84;
            this.bFGA.UseVisualStyleBackColor = false;
            this.bFGA.Click += new System.EventHandler(this.BFGAClick);
            // 
            // bExcel
            // 
            this.bExcel.BackColor = System.Drawing.Color.DarkSalmon;
            this.bExcel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bExcel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bExcel.Image = ((System.Drawing.Image)(resources.GetObject("bExcel.Image")));
            this.bExcel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bExcel.Location = new System.Drawing.Point(427, 526);
            this.bExcel.Name = "bExcel";
            this.bExcel.Size = new System.Drawing.Size(270, 29);
            this.bExcel.TabIndex = 103;
            this.bExcel.Text = "Grabar distribuciones en Excel";
            this.bExcel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bExcel.UseVisualStyleBackColor = false;
            this.bExcel.Click += new System.EventHandler(this.BExcelClick);
            // 
            // bIR
            // 
            this.bIR.BackColor = System.Drawing.Color.DarkSalmon;
            this.bIR.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bIR.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bIR.Image = ((System.Drawing.Image)(resources.GetObject("bIR.Image")));
            this.bIR.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bIR.Location = new System.Drawing.Point(321, 392);
            this.bIR.Name = "bIR";
            this.bIR.Size = new System.Drawing.Size(77, 30);
            this.bIR.TabIndex = 20;
            this.bIR.Text = "Grabar";
            this.bIR.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bIR.UseVisualStyleBackColor = false;
            this.bIR.Click += new System.EventHandler(this.BIRClick);
            // 
            // bCancelar
            // 
            this.bCancelar.BackColor = System.Drawing.Color.DarkSalmon;
            this.bCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bCancelar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bCancelar.Image = ((System.Drawing.Image)(resources.GetObject("bCancelar.Image")));
            this.bCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bCancelar.Location = new System.Drawing.Point(266, 97);
            this.bCancelar.Name = "bCancelar";
            this.bCancelar.Size = new System.Drawing.Size(104, 22);
            this.bCancelar.TabIndex = 25;
            this.bCancelar.Text = "Cancelar";
            this.bCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bCancelar.UseVisualStyleBackColor = false;
            this.bCancelar.Click += new System.EventHandler(this.BCancelarClick);
            // 
            // bMasR
            // 
            this.bMasR.BackColor = System.Drawing.Color.DarkSalmon;
            this.bMasR.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bMasR.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bMasR.Location = new System.Drawing.Point(231, 38);
            this.bMasR.Name = "bMasR";
            this.bMasR.Size = new System.Drawing.Size(16, 14);
            this.bMasR.TabIndex = 84;
            this.bMasR.Text = "+";
            this.bMasR.UseVisualStyleBackColor = false;
            this.bMasR.Click += new System.EventHandler(this.BMasRClick);
            // 
            // lcolsin
            // 
            this.lcolsin.BackColor = System.Drawing.SystemColors.Info;
            this.lcolsin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lcolsin.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lcolsin.Location = new System.Drawing.Point(128, 12);
            this.lcolsin.Name = "lcolsin";
            this.lcolsin.Size = new System.Drawing.Size(96, 21);
            this.lcolsin.TabIndex = 95;
            this.lcolsin.Text = "Leídas";
            this.lcolsin.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lColsAdm
            // 
            this.lColsAdm.BackColor = System.Drawing.SystemColors.Info;
            this.lColsAdm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lColsAdm.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lColsAdm.Location = new System.Drawing.Point(266, 53);
            this.lColsAdm.Name = "lColsAdm";
            this.lColsAdm.Size = new System.Drawing.Size(104, 21);
            this.lColsAdm.TabIndex = 23;
            this.lColsAdm.Text = "Válidas";
            this.lColsAdm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bMenos
            // 
            this.bMenos.BackColor = System.Drawing.Color.DarkSalmon;
            this.bMenos.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bMenos.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bMenos.Location = new System.Drawing.Point(210, 95);
            this.bMenos.Name = "bMenos";
            this.bMenos.Size = new System.Drawing.Size(16, 14);
            this.bMenos.TabIndex = 82;
            this.bMenos.Text = "-";
            this.bMenos.UseVisualStyleBackColor = false;
            this.bMenos.Click += new System.EventHandler(this.BMenosClick);
            // 
            // lFGA
            // 
            this.lFGA.BackColor = System.Drawing.SystemColors.Info;
            this.lFGA.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lFGA.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lFGA.Location = new System.Drawing.Point(32, 72);
            this.lFGA.Name = "lFGA";
            this.lFGA.Size = new System.Drawing.Size(144, 21);
            this.lFGA.TabIndex = 34;
            this.lFGA.Text = "Fichero ganadoras";
            this.lFGA.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbCGR
            // 
            this.lbCGR.BackColor = System.Drawing.SystemColors.Info;
            this.lbCGR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbCGR.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCGR.Location = new System.Drawing.Point(198, 38);
            this.lbCGR.Name = "lbCGR";
            this.lbCGR.Size = new System.Drawing.Size(32, 28);
            this.lbCGR.TabIndex = 86;
            this.lbCGR.Text = "CG";
            this.lbCGR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ltColA
            // 
            this.ltColA.BackColor = System.Drawing.SystemColors.Info;
            this.ltColA.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ltColA.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ltColA.Location = new System.Drawing.Point(32, 94);
            this.ltColA.Name = "ltColA";
            this.ltColA.Size = new System.Drawing.Size(144, 21);
            this.ltColA.TabIndex = 90;
            this.ltColA.Text = "Col. Ganadora";
            this.ltColA.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bAnalizar
            // 
            this.bAnalizar.BackColor = System.Drawing.Color.DarkSalmon;
            this.bAnalizar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bAnalizar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bAnalizar.Image = ((System.Drawing.Image)(resources.GetObject("bAnalizar.Image")));
            this.bAnalizar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bAnalizar.Location = new System.Drawing.Point(248, 38);
            this.bAnalizar.Name = "bAnalizar";
            this.bAnalizar.Size = new System.Drawing.Size(104, 28);
            this.bAnalizar.TabIndex = 27;
            this.bAnalizar.Text = "Analizar";
            this.bAnalizar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bAnalizar.UseVisualStyleBackColor = false;
            this.bAnalizar.Click += new System.EventHandler(this.BAnalizarClick);
            // 
            // bCalcular
            // 
            this.bCalcular.BackColor = System.Drawing.Color.DarkSalmon;
            this.bCalcular.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bCalcular.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bCalcular.Image = ((System.Drawing.Image)(resources.GetObject("bCalcular.Image")));
            this.bCalcular.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bCalcular.Location = new System.Drawing.Point(266, 8);
            this.bCalcular.Name = "bCalcular";
            this.bCalcular.Size = new System.Drawing.Size(104, 22);
            this.bCalcular.TabIndex = 21;
            this.bCalcular.Text = "Calcular";
            this.bCalcular.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bCalcular.UseVisualStyleBackColor = false;
            this.bCalcular.Click += new System.EventHandler(this.BCalcularClick);
            // 
            // bLeerCondis
            // 
            this.bLeerCondis.BackColor = System.Drawing.Color.DarkSalmon;
            this.bLeerCondis.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bLeerCondis.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bLeerCondis.Image = ((System.Drawing.Image)(resources.GetObject("bLeerCondis.Image")));
            this.bLeerCondis.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bLeerCondis.Location = new System.Drawing.Point(8, 392);
            this.bLeerCondis.Name = "bLeerCondis";
            this.bLeerCondis.Size = new System.Drawing.Size(109, 30);
            this.bLeerCondis.TabIndex = 29;
            this.bLeerCondis.Text = "Leer";
            this.bLeerCondis.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bLeerCondis.UseVisualStyleBackColor = false;
            this.bLeerCondis.Click += new System.EventHandler(this.BLeerCondisClick);
            // 
            // ltColR
            // 
            this.ltColR.BackColor = System.Drawing.SystemColors.Info;
            this.ltColR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ltColR.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ltColR.Location = new System.Drawing.Point(53, 51);
            this.ltColR.Name = "ltColR";
            this.ltColR.Size = new System.Drawing.Size(144, 20);
            this.ltColR.TabIndex = 89;
            this.ltColR.Text = "Col. Ganadora";
            this.ltColR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbCGA
            // 
            this.lbCGA.BackColor = System.Drawing.SystemColors.Info;
            this.lbCGA.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbCGA.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbCGA.Location = new System.Drawing.Point(177, 80);
            this.lbCGA.Name = "lbCGA";
            this.lbCGA.Size = new System.Drawing.Size(32, 29);
            this.lbCGA.TabIndex = 83;
            this.lbCGA.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bMas
            // 
            this.bMas.BackColor = System.Drawing.Color.DarkSalmon;
            this.bMas.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bMas.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bMas.Location = new System.Drawing.Point(210, 80);
            this.bMas.Name = "bMas";
            this.bMas.Size = new System.Drawing.Size(16, 14);
            this.bMas.TabIndex = 81;
            this.bMas.Text = "+";
            this.bMas.UseVisualStyleBackColor = false;
            this.bMas.Click += new System.EventHandler(this.BMasClick);
            // 
            // lbAMC
            // 
            this.lbAMC.BackColor = System.Drawing.SystemColors.Info;
            this.lbAMC.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbAMC.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbAMC.Location = new System.Drawing.Point(648, 5);
            this.lbAMC.Name = "lbAMC";
            this.lbAMC.Size = new System.Drawing.Size(32, 29);
            this.lbAMC.TabIndex = 102;
            this.lbAMC.Text = "1";
            this.lbAMC.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabrad
            // 
            this.tabrad.CaptionFont = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabrad.CaptionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.tabrad.CaptionText = "Radiografía";
            this.tabrad.DataMember = "";
            this.tabrad.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.tabrad.Location = new System.Drawing.Point(242, 128);
            this.tabrad.Name = "tabrad";
            this.tabrad.PreferredColumnWidth = 40;
            this.tabrad.ReadOnly = true;
            this.tabrad.RowHeaderWidth = 0;
            this.tabrad.Size = new System.Drawing.Size(156, 260);
            this.tabrad.TabIndex = 34;
            // 
            // lFGR
            // 
            this.lFGR.BackColor = System.Drawing.SystemColors.Info;
            this.lFGR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lFGR.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lFGR.Location = new System.Drawing.Point(53, 28);
            this.lFGR.Name = "lFGR";
            this.lFGR.Size = new System.Drawing.Size(144, 22);
            this.lFGR.TabIndex = 88;
            this.lFGR.Text = "Fichero ganadoras";
            this.lFGR.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lfileconds
            // 
            this.lfileconds.BackColor = System.Drawing.SystemColors.Info;
            this.lfileconds.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lfileconds.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lfileconds.Location = new System.Drawing.Point(40, 424);
            this.lfileconds.Name = "lfileconds";
            this.lfileconds.Size = new System.Drawing.Size(152, 22);
            this.lfileconds.TabIndex = 92;
            this.lfileconds.Text = "Fichero condiciones";
            this.lfileconds.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabout
            // 
            this.tabout.CaptionForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.tabout.CaptionText = "Distribución de Aceptadas";
            this.tabout.DataMember = "";
            this.tabout.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.tabout.Location = new System.Drawing.Point(427, 40);
            this.tabout.Name = "tabout";
            this.tabout.RowHeaderWidth = 0;
            this.tabout.Size = new System.Drawing.Size(270, 432);
            this.tabout.TabIndex = 35;
            this.tabout.MouseUp += new System.Windows.Forms.MouseEventHandler(this.taboutMouseUp);
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.Bisque;
            this.groupBox3.Controls.Add(this.ltColR);
            this.groupBox3.Controls.Add(this.bFGR);
            this.groupBox3.Controls.Add(this.lbCGR);
            this.groupBox3.Controls.Add(this.bMenosR);
            this.groupBox3.Controls.Add(this.bMasR);
            this.groupBox3.Controls.Add(this.lFGR);
            this.groupBox3.Controls.Add(this.bAnalizar);
            this.groupBox3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(40, 456);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(380, 99);
            this.groupBox3.TabIndex = 31;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Analisis Resultados";
            // 
            // bAMC2
            // 
            this.bAMC2.BackColor = System.Drawing.Color.DarkSalmon;
            this.bAMC2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bAMC2.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bAMC2.Location = new System.Drawing.Point(681, 20);
            this.bAMC2.Name = "bAMC2";
            this.bAMC2.Size = new System.Drawing.Size(16, 14);
            this.bAMC2.TabIndex = 101;
            this.bAMC2.Text = "-";
            this.bAMC2.UseVisualStyleBackColor = false;
            this.bAMC2.Click += new System.EventHandler(this.BAMC2Click);
            // 
            // bAMC1
            // 
            this.bAMC1.BackColor = System.Drawing.Color.DarkSalmon;
            this.bAMC1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bAMC1.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bAMC1.Location = new System.Drawing.Point(681, 5);
            this.bAMC1.Name = "bAMC1";
            this.bAMC1.Size = new System.Drawing.Size(16, 14);
            this.bAMC1.TabIndex = 100;
            this.bAMC1.Text = "+";
            this.bAMC1.UseVisualStyleBackColor = false;
            this.bAMC1.Click += new System.EventHandler(this.BAMC1Click);
            // 
            // lTime
            // 
            this.lTime.BackColor = System.Drawing.SystemColors.Info;
            this.lTime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lTime.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lTime.Location = new System.Drawing.Point(266, 75);
            this.lTime.Name = "lTime";
            this.lTime.Size = new System.Drawing.Size(104, 21);
            this.lTime.TabIndex = 22;
            this.lTime.Text = "00:00:00.0";
            this.lTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lcolsout
            // 
            this.lcolsout.BackColor = System.Drawing.SystemColors.Info;
            this.lcolsout.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lcolsout.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lcolsout.Location = new System.Drawing.Point(565, 475);
            this.lcolsout.Name = "lcolsout";
            this.lcolsout.Size = new System.Drawing.Size(132, 29);
            this.lcolsout.TabIndex = 97;
            this.lcolsout.Text = "Grabadas";
            this.lcolsout.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bCalR
            // 
            this.bCalR.BackColor = System.Drawing.Color.DarkSalmon;
            this.bCalR.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.bCalR.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bCalR.Image = ((System.Drawing.Image)(resources.GetObject("bCalR.Image")));
            this.bCalR.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bCalR.Location = new System.Drawing.Point(242, 392);
            this.bCalR.Name = "bCalR";
            this.bCalR.Size = new System.Drawing.Size(77, 30);
            this.bCalR.TabIndex = 36;
            this.bCalR.Text = "Calcular";
            this.bCalR.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bCalR.UseVisualStyleBackColor = false;
            this.bCalR.Click += new System.EventHandler(this.BCalRClick);
            // 
            // Coincidencias
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(703, 560);
            this.Controls.Add(this.bExcel);
            this.Controls.Add(this.bAMC1);
            this.Controls.Add(this.bAMC2);
            this.Controls.Add(this.lbAMC);
            this.Controls.Add(this.lcolsout);
            this.Controls.Add(this.lfileout);
            this.Controls.Add(this.lcolsin);
            this.Controls.Add(this.lfilein);
            this.Controls.Add(this.bEntrada);
            this.Controls.Add(this.lfileconds);
            this.Controls.Add(this.bCalR);
            this.Controls.Add(this.tabout);
            this.Controls.Add(this.tabrad);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.bSalvarCondis);
            this.Controls.Add(this.bLeerCondis);
            this.Controls.Add(this.bGrabar);
            this.Controls.Add(this.bCancelar);
            this.Controls.Add(this.lTime);
            this.Controls.Add(this.lColsAdm);
            this.Controls.Add(this.lColsProc);
            this.Controls.Add(this.bCalcular);
            this.Controls.Add(this.bIR);
            this.Controls.Add(this.ltColA);
            this.Controls.Add(this.lFGA);
            this.Controls.Add(this.bMas);
            this.Controls.Add(this.bMenos);
            this.Controls.Add(this.lbCGA);
            this.Controls.Add(this.bFGA);
            this.Controls.Add(this.tabcond);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Coincidencias";
            this.Text = "Coincidencias";
            ((System.ComponentModel.ISupportInitialize)(this.tabcond)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabrad)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tabout)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion
		
		void BCalcularClick(object sender, System.EventArgs e) { Calcular(); }
		void BCancelarClick(object sender, System.EventArgs e) { salida=true; }
		void BGrabarClick(object sender, System.EventArgs e) { Grabar(); }
		void BIRClick(object sender, System.EventArgs e) { PrintRadio(); }
		void elmeuTimer(object sender, System.EventArgs e) { veureelmeu(); }
		void BLeerCondisClick(object sender, System.EventArgs e) { LeeCondis(); }
		void BSalvarCondisClick(object sender, System.EventArgs e) { SalvaCondis(); }
		void BAnalizarClick(object sender, System.EventArgs e) { Analizar(); }
		void BMasClick(object sender, System.EventArgs e) { GAMas(); }
		void BMenosClick(object sender, System.EventArgs e) { GAMenos(); }
		void BMasRClick(object sender, System.EventArgs e) { GRMas(); }
		void BMenosRClick(object sender, System.EventArgs e) { GRMenos(); }
		void BFGAClick(object sender, System.EventArgs e) { EntraCGsA(); }
		void BFGRClick(object sender, System.EventArgs e) { EntraCGsR(); }
		void taboutMouseUp(object sender, System.Windows.Forms.MouseEventArgs e) {
			Point pt = new Point(e.X, e.Y);
			DataGrid.HitTestInfo hti = tabout.HitTest(pt);
			if(hti.Type == DataGrid.HitTestType.Cell) {
				tabout.CurrentCell = new DataGridCell(hti.Row, hti.Column);
				tabout.Select(hti.Row);
				if((bool)tabout[hti.Row,1]) tabout[hti.Row,1] = false;
				else tabout[hti.Row,1] = true;
			}
		}
		void BCalRClick(object sender, System.EventArgs e) { CalRadioB(); }
		void BEntradaClick(object sender, System.EventArgs e) { LeerFileIn(); }
		void BAMC1Click(object sender, System.EventArgs e) { AMCmas(); }
		void BAMC2Click(object sender, System.EventArgs e) { AMCmenos(); }
		void BExcelClick(object sender, System.EventArgs e) { GrabExcel(); }
		
	}
}

