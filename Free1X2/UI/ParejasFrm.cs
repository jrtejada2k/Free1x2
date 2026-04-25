using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI
{
	public class ParejasFrm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox p111;
		private System.Windows.Forms.TextBox v113;
		private System.Windows.Forms.TextBox v21;
		private System.Windows.Forms.TextBox v23;
		private System.Windows.Forms.TextBox v22;
		private System.Windows.Forms.TextBox v25;
		private System.Windows.Forms.TextBox v24;
		private System.Windows.Forms.TextBox v27;
		private System.Windows.Forms.TextBox v26;
		private System.Windows.Forms.TextBox v29;
		private System.Windows.Forms.TextBox v28;
		private System.Windows.Forms.TextBox c31;
		private System.Windows.Forms.TextBox p41;
		private System.Windows.Forms.TextBox c62;
		private System.Windows.Forms.Label lval;
		private System.Windows.Forms.TextBox p42;
		private System.Windows.Forms.TextBox v31;
		private System.Windows.Forms.TextBox v32;
		private System.Windows.Forms.TextBox v33;
		private System.Windows.Forms.TextBox v34;
		private System.Windows.Forms.TextBox v35;
		private System.Windows.Forms.TextBox v36;
		private System.Windows.Forms.TextBox v37;
		private System.Windows.Forms.TextBox v38;
		private System.Windows.Forms.TextBox v39;
		private System.Windows.Forms.TextBox p82;
		private System.Windows.Forms.TextBox p51;
		private System.Windows.Forms.TextBox p52;
		private System.Windows.Forms.TextBox v03;
		private System.Windows.Forms.TextBox v02;
		private System.Windows.Forms.TextBox v01;
		private System.Windows.Forms.TextBox v07;
		private System.Windows.Forms.TextBox v06;
		private System.Windows.Forms.TextBox v05;
		private System.Windows.Forms.TextBox v04;
		private System.Windows.Forms.TextBox v123;
		private System.Windows.Forms.TextBox v124;
		private System.Windows.Forms.TextBox v09;
		private System.Windows.Forms.TextBox v08;
		private System.Windows.Forms.TextBox v127;
		private System.Windows.Forms.TextBox v128;
		private System.Windows.Forms.TextBox v129;
		private System.Windows.Forms.TextBox v136;
		private System.Windows.Forms.TextBox c12;
		private System.Windows.Forms.TextBox c11;
		private System.Windows.Forms.TextBox p62;
		private System.Windows.Forms.TextBox p61;
		private System.Windows.Forms.TextBox v12;
		private System.Windows.Forms.TextBox v13;
		private System.Windows.Forms.TextBox v11;
		private System.Windows.Forms.TextBox v16;
		private System.Windows.Forms.TextBox v17;
		private System.Windows.Forms.TextBox v14;
		private System.Windows.Forms.TextBox v15;
		private System.Windows.Forms.TextBox v18;
		private System.Windows.Forms.TextBox v19;
		private System.Windows.Forms.TextBox p101;
		private System.Windows.Forms.TextBox p102;
		private System.Windows.Forms.TextBox p72;
		private System.Windows.Forms.TextBox c71;
		private System.Windows.Forms.TextBox v65;
		private System.Windows.Forms.TextBox v64;
		private System.Windows.Forms.TextBox v67;
		private System.Windows.Forms.TextBox v66;
		private System.Windows.Forms.TextBox v61;
		private System.Windows.Forms.TextBox v102;
		private System.Windows.Forms.TextBox v63;
		private System.Windows.Forms.TextBox v62;
		private System.Windows.Forms.TextBox v101;
		private System.Windows.Forms.TextBox v106;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox v68;
		private System.Windows.Forms.TextBox v108;
		private System.Windows.Forms.TextBox v109;
		private System.Windows.Forms.TextBox v74;
		private System.Windows.Forms.TextBox v75;
		private System.Windows.Forms.TextBox v76;
		private System.Windows.Forms.TextBox v77;
		private System.Windows.Forms.TextBox v71;
		private System.Windows.Forms.TextBox v72;
		private System.Windows.Forms.TextBox v73;
		private System.Windows.Forms.TextBox v132;
		private System.Windows.Forms.TextBox v135;
		private System.Windows.Forms.TextBox v134;
		private System.Windows.Forms.TextBox v137;
		private System.Windows.Forms.TextBox v78;
		private System.Windows.Forms.TextBox v79;
		private System.Windows.Forms.TextBox v138;
		private System.Windows.Forms.TextBox p122;
		private System.Windows.Forms.TextBox p121;
		private System.Windows.Forms.TextBox v47;
		private System.Windows.Forms.TextBox v46;
		private System.Windows.Forms.TextBox v45;
		private System.Windows.Forms.TextBox v44;
		private System.Windows.Forms.TextBox v43;
		private System.Windows.Forms.TextBox v42;
		private System.Windows.Forms.TextBox v41;
		private System.Windows.Forms.TextBox v49;
		private System.Windows.Forms.TextBox v48;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.TextBox v122;
		private System.Windows.Forms.Label label23;
		private System.Windows.Forms.Label label25;
		private System.Windows.Forms.TextBox v126;
		private System.Windows.Forms.Button bLeer;
		private System.Windows.Forms.TextBox v56;
		private System.Windows.Forms.TextBox v57;
		private System.Windows.Forms.TextBox v54;
		private System.Windows.Forms.TextBox v55;
		private System.Windows.Forms.TextBox v52;
		private System.Windows.Forms.TextBox v53;
		private System.Windows.Forms.TextBox v51;
		private System.Windows.Forms.TextBox v117;
		private System.Windows.Forms.TextBox c32;
		private System.Windows.Forms.TextBox v115;
		private System.Windows.Forms.TextBox v114;
		private System.Windows.Forms.TextBox v58;
		private System.Windows.Forms.TextBox v119;
		private System.Windows.Forms.TextBox v118;
		private System.Windows.Forms.Label label30;
		private System.Windows.Forms.Label r7;
		private System.Windows.Forms.Label r6;
		private System.Windows.Forms.Label r5;
		private System.Windows.Forms.Label r4;
		private System.Windows.Forms.Label r3;
		private System.Windows.Forms.Label r2;
		private System.Windows.Forms.Label r1;
		private System.Windows.Forms.TextBox p32;
		private System.Windows.Forms.TextBox p31;
		private System.Windows.Forms.Label lproc;
		private System.Windows.Forms.TextBox c61;
		private System.Windows.Forms.TextBox p132;
		private System.Windows.Forms.TextBox v103;
		private System.Windows.Forms.TextBox v69;
		private System.Windows.Forms.TextBox v104;
		private System.Windows.Forms.TextBox v105;
		private System.Windows.Forms.Label ltime;
		private System.Windows.Forms.TextBox p02;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.TextBox v139;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.TextBox v89;
		private System.Windows.Forms.TextBox v88;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.TextBox v83;
		private System.Windows.Forms.TextBox v82;
		private System.Windows.Forms.TextBox v87;
		private System.Windows.Forms.TextBox v86;
		private System.Windows.Forms.Button bSalvar;
		private System.Windows.Forms.TextBox v98;
		private System.Windows.Forms.TextBox v99;
		private System.Windows.Forms.TextBox v92;
		private System.Windows.Forms.TextBox v93;
		private System.Windows.Forms.TextBox p22;
		private System.Windows.Forms.TextBox v96;
		private System.Windows.Forms.TextBox v97;
		private System.Windows.Forms.TextBox v94;
		private System.Windows.Forms.TextBox v95;
		private System.Windows.Forms.TextBox c42;
		private System.Windows.Forms.TextBox v112;
		private System.Windows.Forms.TextBox v111;
		private System.Windows.Forms.TextBox c41;
		private System.Windows.Forms.TextBox p131;
		private System.Windows.Forms.TextBox v116;
		private System.Windows.Forms.TextBox v59;
		private System.Windows.Forms.Button bCalcular;
		private System.Windows.Forms.TextBox p71;
		private System.Windows.Forms.TextBox v81;
		private System.Windows.Forms.TextBox c72;
		private System.Windows.Forms.TextBox v85;
		private System.Windows.Forms.TextBox v84;
		private System.Windows.Forms.TextBox p112;
		private System.Windows.Forms.TextBox v107;
		private System.Windows.Forms.TextBox p81;
		private System.Windows.Forms.Button bGrabar;
		private System.Windows.Forms.TextBox p01;
		private System.Windows.Forms.TextBox v91;
		private System.Windows.Forms.TextBox c21;
		private System.Windows.Forms.TextBox c22;
		private System.Windows.Forms.TextBox v131;
		private System.Windows.Forms.Button bCancelar;
		private System.Windows.Forms.TextBox v133;
		private System.Windows.Forms.TextBox p91;
		private System.Windows.Forms.TextBox p92;
		private System.Windows.Forms.TextBox p11;
		private System.Windows.Forms.TextBox p12;
		private System.Windows.Forms.TextBox c52;
		private System.Windows.Forms.TextBox c51;
		private System.Windows.Forms.TextBox tCG;
		private System.Windows.Forms.Button bAnalizar;
		private System.Windows.Forms.TextBox v121;
		private System.Windows.Forms.TextBox v125;
		private System.Windows.Forms.TextBox p21;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label9;
		public ParejasFrm()
		{
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

		
 		private bool val, salida = false;
		private DateTime time0, time9;
		private Timer elmeu;
		private int idx, ctproc, ctval;
		private string tmp, columna, filein, fileout;
 		private BitArray validas = new BitArray(4782969);
 		private int[,] nivells = new int[14,11];
 		private int[,] rks = new int[7,3];
		
		private void LeeCondis() {
			string tmp, filein; string[] aux = null;
			OpenFileDialog lee = new OpenFileDialog();
			lee.InitialDirectory = ".\\" ;
			lee.Filter = "Condiciones(*.par)|*.par|Todos los archivos (*.*)|*.*";
			if(lee.ShowDialog() == DialogResult.OK) {
		   		tmp = lee.FileName;
		   		filein = Path.GetFileName(tmp);
				StreamReader sr = new StreamReader(filein);
				tmp = sr.ReadLine(); aux = tmp.Split(',');
				p01.Text = aux[0]; p02.Text = aux[1];
				v01.Text = aux[2]; v02.Text = aux[3]; v03.Text = aux[4];
				v04.Text = aux[5]; v05.Text = aux[6]; v06.Text = aux[7];
				v07.Text = aux[8]; v08.Text = aux[9]; v09.Text = aux[10];
				tmp = sr.ReadLine(); aux = tmp.Split(',');
				p11.Text = aux[0]; p12.Text = aux[1];
				v11.Text = aux[2]; v12.Text = aux[3]; v13.Text = aux[4];
				v14.Text = aux[5]; v15.Text = aux[6]; v16.Text = aux[7];
				v17.Text = aux[8]; v18.Text = aux[9]; v19.Text = aux[10];
				tmp = sr.ReadLine(); aux = tmp.Split(',');
				p21.Text = aux[0]; p22.Text = aux[1];
				v21.Text = aux[2]; v22.Text = aux[3]; v23.Text = aux[4];
				v24.Text = aux[5]; v25.Text = aux[6]; v26.Text = aux[7];
				v27.Text = aux[8]; v28.Text = aux[9]; v29.Text = aux[10];
				tmp = sr.ReadLine(); aux = tmp.Split(',');
				p31.Text = aux[0]; p32.Text = aux[1];
				v31.Text = aux[2]; v32.Text = aux[3]; v33.Text = aux[4];
				v34.Text = aux[5]; v35.Text = aux[6]; v36.Text = aux[7];
				v37.Text = aux[8]; v38.Text = aux[9]; v39.Text = aux[10];
				tmp = sr.ReadLine(); aux = tmp.Split(',');
				p41.Text = aux[0]; p42.Text = aux[1];
				v41.Text = aux[2]; v42.Text = aux[3]; v43.Text = aux[4];
				v44.Text = aux[5]; v45.Text = aux[6]; v46.Text = aux[7];
				v47.Text = aux[8]; v48.Text = aux[9]; v49.Text = aux[10];
				tmp = sr.ReadLine(); aux = tmp.Split(',');
				p51.Text = aux[0]; p52.Text = aux[1];
				v51.Text = aux[2]; v52.Text = aux[3]; v53.Text = aux[4];
				v54.Text = aux[5]; v55.Text = aux[6]; v56.Text = aux[7];
				v57.Text = aux[8]; v58.Text = aux[9]; v59.Text = aux[10];
				tmp = sr.ReadLine(); aux = tmp.Split(',');
				p61.Text = aux[0]; p62.Text = aux[1];
				v61.Text = aux[2]; v62.Text = aux[3]; v63.Text = aux[4];
				v64.Text = aux[5]; v65.Text = aux[6]; v66.Text = aux[7];
				v67.Text = aux[8]; v68.Text = aux[9]; v69.Text = aux[10];
				tmp = sr.ReadLine(); aux = tmp.Split(',');
				p71.Text = aux[0]; p72.Text = aux[1];
				v71.Text = aux[2]; v72.Text = aux[3]; v73.Text = aux[4];
				v74.Text = aux[5]; v75.Text = aux[6]; v76.Text = aux[7];
				v77.Text = aux[8]; v78.Text = aux[9]; v79.Text = aux[10];
				tmp = sr.ReadLine(); aux = tmp.Split(',');
				p81.Text = aux[0]; p82.Text = aux[1];
				v81.Text = aux[2]; v82.Text = aux[3]; v83.Text = aux[4];
				v84.Text = aux[5]; v85.Text = aux[6]; v86.Text = aux[7];
				v87.Text = aux[8]; v88.Text = aux[9]; v89.Text = aux[10];
				tmp = sr.ReadLine(); aux = tmp.Split(',');
				p91.Text = aux[0]; p92.Text = aux[1];
				v91.Text = aux[2]; v92.Text = aux[3]; v93.Text = aux[4];
				v94.Text = aux[5]; v95.Text = aux[6]; v96.Text = aux[7];
				v97.Text = aux[8]; v98.Text = aux[9]; v99.Text = aux[10];
				tmp = sr.ReadLine(); aux = tmp.Split(',');
				p101.Text = aux[0]; p102.Text = aux[1];
				v101.Text = aux[2]; v102.Text = aux[3]; v103.Text = aux[4];
				v104.Text = aux[5]; v105.Text = aux[6]; v106.Text = aux[7];
				v107.Text = aux[8]; v108.Text = aux[9]; v109.Text = aux[10];
				tmp = sr.ReadLine(); aux = tmp.Split(',');
				p111.Text = aux[0]; p112.Text = aux[1];
				v111.Text = aux[2]; v112.Text = aux[3]; v113.Text = aux[4];
				v114.Text = aux[5]; v115.Text = aux[6]; v116.Text = aux[7];
				v117.Text = aux[8]; v118.Text = aux[9]; v119.Text = aux[10];
				tmp = sr.ReadLine(); aux = tmp.Split(',');
				p121.Text = aux[0]; p122.Text = aux[1];
				v129.Text = aux[2]; v122.Text = aux[3]; v123.Text = aux[4];
				v124.Text = aux[5]; v125.Text = aux[6]; v126.Text = aux[7];
				v127.Text = aux[8]; v128.Text = aux[9]; v129.Text = aux[10];
				tmp = sr.ReadLine(); aux = tmp.Split(',');
				p131.Text = aux[0]; p132.Text = aux[1];
				v131.Text = aux[2]; v132.Text = aux[3]; v133.Text = aux[4];
				v134.Text = aux[5]; v135.Text = aux[6]; v136.Text = aux[7];
				v137.Text = aux[8]; v138.Text = aux[9]; v139.Text = aux[10];
				tmp = sr.ReadLine(); aux = tmp.Split(',');
				c11.Text = aux[0]; c12.Text = aux[1];
				tmp = sr.ReadLine(); aux = tmp.Split(',');
				c21.Text = aux[0]; c22.Text = aux[1];
				tmp = sr.ReadLine(); aux = tmp.Split(',');
				c31.Text = aux[0]; c32.Text = aux[1];
				tmp = sr.ReadLine(); aux = tmp.Split(',');
				c41.Text = aux[0]; c42.Text = aux[1];
				tmp = sr.ReadLine(); aux = tmp.Split(',');
				c51.Text = aux[0]; c52.Text = aux[1];
				tmp = sr.ReadLine(); aux = tmp.Split(',');
				c61.Text = aux[0]; c62.Text = aux[1];
				tmp = sr.ReadLine(); aux = tmp.Split(',');
				c71.Text = aux[0]; c72.Text = aux[1];
				sr.Close();
			}
		}
		private void SalvaCondis() {
			string tmp, fileout;
			SaveFileDialog grabacondis = new SaveFileDialog();
			grabacondis.InitialDirectory = ".\\" ;
			grabacondis.Filter = "Condiciones(*.par)|*.par|Todos los archivos (*.*)|*.*";
			if(grabacondis.ShowDialog() == DialogResult.OK) {
		   		tmp = grabacondis.FileName;
		   		fileout = Path.GetFileName(tmp);
				StreamWriter sw = new StreamWriter(fileout);
				tmp=p01.Text+','+p02.Text+','+v01.Text+','+v02.Text+',';
				tmp+=v03.Text+','+v04.Text+','+v05.Text+','+v06.Text+',';
				tmp+=v07.Text+','+v08.Text+','+v09.Text;
				sw.WriteLine(tmp);
				tmp=p11.Text+','+p12.Text+','+v11.Text+','+v12.Text+',';
				tmp+=v13.Text+','+v14.Text+','+v15.Text+','+v16.Text+',';
				tmp+=v17.Text+','+v18.Text+','+v19.Text;
				sw.WriteLine(tmp);
				tmp=p21.Text+','+p22.Text+','+v21.Text+','+v22.Text+',';
				tmp+=v23.Text+','+v24.Text+','+v25.Text+','+v26.Text+',';
				tmp+=v27.Text+','+v28.Text+','+v29.Text;
				sw.WriteLine(tmp);
				tmp=p31.Text+','+p32.Text+','+v31.Text+','+v32.Text+',';
				tmp+=v33.Text+','+v34.Text+','+v35.Text+','+v36.Text+',';
				tmp+=v37.Text+','+v38.Text+','+v39.Text;
				sw.WriteLine(tmp);
				tmp=p41.Text+','+p42.Text+','+v41.Text+','+v42.Text+',';
				tmp+=v43.Text+','+v44.Text+','+v45.Text+','+v46.Text+',';
				tmp+=v47.Text+','+v48.Text+','+v49.Text;
				sw.WriteLine(tmp);
				tmp=p51.Text+','+p52.Text+','+v51.Text+','+v52.Text+',';
				tmp+=v53.Text+','+v54.Text+','+v55.Text+','+v56.Text+',';
				tmp+=v57.Text+','+v58.Text+','+v59.Text;
				sw.WriteLine(tmp);
				tmp=p61.Text+','+p62.Text+','+v61.Text+','+v62.Text+',';
				tmp+=v63.Text+','+v64.Text+','+v65.Text+','+v66.Text+',';
				tmp+=v67.Text+','+v68.Text+','+v69.Text;
				sw.WriteLine(tmp);
				tmp=p71.Text+','+p72.Text+','+v71.Text+','+v72.Text+',';
				tmp+=v73.Text+','+v74.Text+','+v75.Text+','+v76.Text+',';
				tmp+=v77.Text+','+v78.Text+','+v79.Text;
				sw.WriteLine(tmp);
				tmp=p81.Text+','+p82.Text+','+v81.Text+','+v82.Text+',';
				tmp+=v83.Text+','+v84.Text+','+v85.Text+','+v86.Text+',';
				tmp+=v87.Text+','+v88.Text+','+v89.Text;
				sw.WriteLine(tmp);
				tmp=p91.Text+','+p92.Text+','+v91.Text+','+v92.Text+',';
				tmp+=v93.Text+','+v94.Text+','+v95.Text+','+v96.Text+',';
				tmp+=v97.Text+','+v98.Text+','+v99.Text;
				sw.WriteLine(tmp);
				tmp=p101.Text+','+p102.Text+','+v101.Text+','+v102.Text+',';
				tmp+=v103.Text+','+v104.Text+','+v105.Text+','+v106.Text+',';
				tmp+=v107.Text+','+v108.Text+','+v109.Text;
				sw.WriteLine(tmp);
				tmp=p111.Text+','+p112.Text+','+v111.Text+','+v112.Text+',';
				tmp+=v113.Text+','+v114.Text+','+v115.Text+','+v116.Text+',';
				tmp+=v117.Text+','+v118.Text+','+v119.Text;
				sw.WriteLine(tmp);
				tmp=p121.Text+','+p122.Text+','+v121.Text+','+v122.Text+',';
				tmp+=v123.Text+','+v124.Text+','+v125.Text+','+v126.Text+',';
				tmp+=v127.Text+','+v128.Text+','+v129.Text;
				sw.WriteLine(tmp);
				tmp=p131.Text+','+p132.Text+','+v131.Text+','+v132.Text+',';
				tmp+=v133.Text+','+v134.Text+','+v135.Text+','+v136.Text+',';
				tmp+=v137.Text+','+v138.Text+','+v139.Text;
				sw.WriteLine(tmp);
				sw.WriteLine(c11.Text+','+c12.Text);
				sw.WriteLine(c21.Text+','+c22.Text);
				sw.WriteLine(c31.Text+','+c32.Text);
				sw.WriteLine(c41.Text+','+c42.Text);
				sw.WriteLine(c51.Text+','+c52.Text);
				sw.WriteLine(c61.Text+','+c62.Text);
				sw.WriteLine(c71.Text+','+c72.Text);
				sw.Close();
			}
		}
		private void veureelmeu() {
			lproc.Text = ""+ctproc;
			lval.Text = ""+ctval;
			time9 = DateTime.Now;
			string temp = (time9-time0).ToString()+"0000000000";
			ltime.Text = temp.Substring(0,10);
		}
		private void Calcular() {
 			bCalcular.Enabled=false;
 			bGrabar.Visible=false;
 			bCalcular.Text="calculando";
 			salida = false;
 			ctproc=ctval=0; 
 			ltime.Text=lproc.Text=lval.Text="0";
 			elmeu.Start(); time0 = DateTime.Now;
 			Application.DoEvents();
 			RecuperarPantalla();
			OpenFileDialog leeDialog = new OpenFileDialog();
			leeDialog.InitialDirectory = ".\\" ;
			leeDialog.Filter = "ColumnasEntrada(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
			if(leeDialog.ShowDialog() == DialogResult.OK) {
		   		tmp = leeDialog.FileName;
		   		filein = Path.GetFileName(tmp);
				StreamReader sr = new StreamReader(filein);
				while (sr.Peek()>0) {
					if (salida) break;
					columna = sr.ReadLine(); ctproc++;
					val = Valida(columna); 			
					if (val) {
						idx = s1n(columna);
						validas[idx]=true;
						ctval++;
					}
					Application.DoEvents();
				}
				sr.Close();
			}
			elmeu.Stop();
			veureelmeu();
 			bCalcular.Text="calcular";
 			bCalcular.Enabled=true;
 			bGrabar.Visible=true;
 			bGrabar.Enabled=true;
		}
		private void RecuperarPantalla() {
 			rks[0,0] = Convert.ToInt32(c11.Text);
 			rks[0,1] = Convert.ToInt32(c12.Text);
 			rks[1,0] = Convert.ToInt32(c21.Text);
 			rks[1,1] = Convert.ToInt32(c22.Text);
 			rks[2,0] = Convert.ToInt32(c31.Text);
 			rks[2,1] = Convert.ToInt32(c32.Text);
 			rks[3,0] = Convert.ToInt32(c41.Text);
 			rks[3,1] = Convert.ToInt32(c42.Text);
 			rks[4,0] = Convert.ToInt32(c51.Text);
 			rks[4,1] = Convert.ToInt32(c52.Text);
 			rks[5,0] = Convert.ToInt32(c61.Text);
 			rks[5,1] = Convert.ToInt32(c62.Text);
 			rks[6,0] = Convert.ToInt32(c71.Text);
 			rks[6,1] = Convert.ToInt32(c72.Text);
 			nivells[0,0] = Convert.ToInt32(p01.Text);
 			nivells[0,1] = Convert.ToInt32(p02.Text);
 			nivells[0,2] = Convert.ToInt32(v01.Text); 
 			nivells[0,3] = Convert.ToInt32(v02.Text);
 			nivells[0,4] = Convert.ToInt32(v03.Text);
 			nivells[0,5] = Convert.ToInt32(v04.Text);
 			nivells[0,6] = Convert.ToInt32(v05.Text);
 			nivells[0,7] = Convert.ToInt32(v06.Text);
 			nivells[0,8] = Convert.ToInt32(v07.Text);
 			nivells[0,9] = Convert.ToInt32(v08.Text);
 			nivells[0,10] = Convert.ToInt32(v09.Text); 
 			nivells[1,0] = Convert.ToInt32(p11.Text);
 			nivells[1,1] = Convert.ToInt32(p12.Text);
 			nivells[1,2] = Convert.ToInt32(v11.Text); 
 			nivells[1,3] = Convert.ToInt32(v12.Text);
 			nivells[1,4] = Convert.ToInt32(v13.Text);
 			nivells[1,5] = Convert.ToInt32(v14.Text);
 			nivells[1,6] = Convert.ToInt32(v15.Text);
 			nivells[1,7] = Convert.ToInt32(v16.Text);
 			nivells[1,8] = Convert.ToInt32(v17.Text);
 			nivells[1,9] = Convert.ToInt32(v18.Text);
 			nivells[1,10] = Convert.ToInt32(v19.Text);
 			nivells[2,0] = Convert.ToInt32(p21.Text);
 			nivells[2,1] = Convert.ToInt32(p22.Text);
 			nivells[2,2] = Convert.ToInt32(v21.Text); 
 			nivells[2,3] = Convert.ToInt32(v22.Text);
 			nivells[2,4] = Convert.ToInt32(v23.Text);
 			nivells[2,5] = Convert.ToInt32(v24.Text);
 			nivells[2,6] = Convert.ToInt32(v25.Text);
 			nivells[2,7] = Convert.ToInt32(v26.Text);
 			nivells[2,8] = Convert.ToInt32(v27.Text);
 			nivells[2,9] = Convert.ToInt32(v28.Text);
 			nivells[2,10] = Convert.ToInt32(v29.Text);
 			nivells[3,0] = Convert.ToInt32(p31.Text);
 			nivells[3,1] = Convert.ToInt32(p32.Text);
 			nivells[3,2] = Convert.ToInt32(v31.Text); 
 			nivells[3,3] = Convert.ToInt32(v32.Text);
 			nivells[3,4] = Convert.ToInt32(v33.Text);
 			nivells[3,5] = Convert.ToInt32(v34.Text);
 			nivells[3,6] = Convert.ToInt32(v35.Text);
 			nivells[3,7] = Convert.ToInt32(v36.Text);
 			nivells[3,8] = Convert.ToInt32(v37.Text);
 			nivells[3,9] = Convert.ToInt32(v38.Text);
 			nivells[3,10] = Convert.ToInt32(v39.Text);    
 			nivells[4,0] = Convert.ToInt32(p41.Text);
 			nivells[4,1] = Convert.ToInt32(p42.Text);
 			nivells[4,2] = Convert.ToInt32(v41.Text); 
 			nivells[4,3] = Convert.ToInt32(v42.Text);
 			nivells[4,4] = Convert.ToInt32(v43.Text);
 			nivells[4,5] = Convert.ToInt32(v44.Text);
 			nivells[4,6] = Convert.ToInt32(v45.Text);
 			nivells[4,7] = Convert.ToInt32(v46.Text);
 			nivells[4,8] = Convert.ToInt32(v47.Text);
 			nivells[4,9] = Convert.ToInt32(v48.Text);
 			nivells[4,10] = Convert.ToInt32(v49.Text);
 			nivells[5,0] = Convert.ToInt32(p51.Text);
 			nivells[5,1] = Convert.ToInt32(p52.Text);
 			nivells[5,2] = Convert.ToInt32(v51.Text); 
 			nivells[5,3] = Convert.ToInt32(v52.Text);
 			nivells[5,4] = Convert.ToInt32(v53.Text);
 			nivells[5,5] = Convert.ToInt32(v54.Text);
 			nivells[5,6] = Convert.ToInt32(v55.Text);
 			nivells[5,7] = Convert.ToInt32(v56.Text);
 			nivells[5,8] = Convert.ToInt32(v57.Text);
 			nivells[5,9] = Convert.ToInt32(v58.Text);
 			nivells[5,10] = Convert.ToInt32(v59.Text);
 			nivells[6,0] = Convert.ToInt32(p61.Text);
 			nivells[6,1] = Convert.ToInt32(p62.Text);
 			nivells[6,2] = Convert.ToInt32(v61.Text); 
 			nivells[6,3] = Convert.ToInt32(v62.Text);
 			nivells[6,4] = Convert.ToInt32(v63.Text);
 			nivells[6,5] = Convert.ToInt32(v64.Text);
 			nivells[6,6] = Convert.ToInt32(v65.Text);
 			nivells[6,7] = Convert.ToInt32(v66.Text);
 			nivells[6,8] = Convert.ToInt32(v67.Text);
 			nivells[6,9] = Convert.ToInt32(v68.Text);
 			nivells[6,10] = Convert.ToInt32(v69.Text); 
 			nivells[7,0] = Convert.ToInt32(p71.Text);
 			nivells[7,1] = Convert.ToInt32(p72.Text);
 			nivells[7,2] = Convert.ToInt32(v71.Text); 
 			nivells[7,3] = Convert.ToInt32(v72.Text);
 			nivells[7,4] = Convert.ToInt32(v73.Text);
 			nivells[7,5] = Convert.ToInt32(v74.Text);
 			nivells[7,6] = Convert.ToInt32(v75.Text);
 			nivells[7,7] = Convert.ToInt32(v76.Text);
 			nivells[7,8] = Convert.ToInt32(v77.Text);
 			nivells[7,9] = Convert.ToInt32(v78.Text);
 			nivells[7,10] = Convert.ToInt32(v79.Text); 
 			nivells[8,0] = Convert.ToInt32(p81.Text);
 			nivells[8,1] = Convert.ToInt32(p82.Text);
 			nivells[8,2] = Convert.ToInt32(v81.Text); 
 			nivells[8,3] = Convert.ToInt32(v82.Text);
 			nivells[8,4] = Convert.ToInt32(v83.Text);
 			nivells[8,5] = Convert.ToInt32(v84.Text);
 			nivells[8,6] = Convert.ToInt32(v85.Text);
 			nivells[8,7] = Convert.ToInt32(v86.Text);
 			nivells[8,8] = Convert.ToInt32(v87.Text);
 			nivells[8,9] = Convert.ToInt32(v88.Text);
 			nivells[8,10] = Convert.ToInt32(v89.Text); 
 			nivells[9,0] = Convert.ToInt32(p91.Text);
 			nivells[9,1] = Convert.ToInt32(p92.Text);
 			nivells[9,2] = Convert.ToInt32(v91.Text); 
 			nivells[9,3] = Convert.ToInt32(v92.Text);
 			nivells[9,4] = Convert.ToInt32(v93.Text);
 			nivells[9,5] = Convert.ToInt32(v94.Text);
 			nivells[9,6] = Convert.ToInt32(v95.Text);
 			nivells[9,7] = Convert.ToInt32(v96.Text);
 			nivells[9,8] = Convert.ToInt32(v97.Text);
 			nivells[9,9] = Convert.ToInt32(v98.Text);
 			nivells[9,10] = Convert.ToInt32(v99.Text); 
 			nivells[10,0] = Convert.ToInt32(p101.Text);
 			nivells[10,1] = Convert.ToInt32(p102.Text);
 			nivells[10,2] = Convert.ToInt32(v101.Text); 
 			nivells[10,3] = Convert.ToInt32(v102.Text);
 			nivells[10,4] = Convert.ToInt32(v103.Text);
 			nivells[10,5] = Convert.ToInt32(v104.Text);
 			nivells[10,6] = Convert.ToInt32(v105.Text);
 			nivells[10,7] = Convert.ToInt32(v106.Text);
 			nivells[10,8] = Convert.ToInt32(v107.Text);
 			nivells[10,9] = Convert.ToInt32(v108.Text);
 			nivells[10,10] = Convert.ToInt32(v109.Text);
 			nivells[11,0] = Convert.ToInt32(p111.Text);
 			nivells[11,1] = Convert.ToInt32(p112.Text);
 			nivells[11,2] = Convert.ToInt32(v111.Text); 
 			nivells[11,3] = Convert.ToInt32(v112.Text);
 			nivells[11,4] = Convert.ToInt32(v113.Text);
 			nivells[11,5] = Convert.ToInt32(v114.Text);
 			nivells[11,6] = Convert.ToInt32(v115.Text);
 			nivells[11,7] = Convert.ToInt32(v116.Text);
 			nivells[11,8] = Convert.ToInt32(v117.Text);
 			nivells[11,9] = Convert.ToInt32(v118.Text);
 			nivells[11,10] = Convert.ToInt32(v119.Text);
 			nivells[12,0] = Convert.ToInt32(p121.Text);
 			nivells[12,1] = Convert.ToInt32(p122.Text);
 			nivells[12,2] = Convert.ToInt32(v121.Text); 
 			nivells[12,3] = Convert.ToInt32(v122.Text);
 			nivells[12,4] = Convert.ToInt32(v123.Text);
 			nivells[12,5] = Convert.ToInt32(v124.Text);
 			nivells[12,6] = Convert.ToInt32(v125.Text);
 			nivells[12,7] = Convert.ToInt32(v126.Text);
 			nivells[12,8] = Convert.ToInt32(v127.Text);
 			nivells[12,9] = Convert.ToInt32(v128.Text);
 			nivells[12,10] = Convert.ToInt32(v129.Text);
 			nivells[13,0] = Convert.ToInt32(p131.Text);
 			nivells[13,1] = Convert.ToInt32(p132.Text);
 			nivells[13,2] = Convert.ToInt32(v131.Text); 
 			nivells[13,3] = Convert.ToInt32(v132.Text);
 			nivells[13,4] = Convert.ToInt32(v133.Text);
 			nivells[13,5] = Convert.ToInt32(v134.Text);
 			nivells[13,6] = Convert.ToInt32(v135.Text);
 			nivells[13,7] = Convert.ToInt32(v136.Text);
 			nivells[13,8] = Convert.ToInt32(v137.Text);
 			nivells[13,9] = Convert.ToInt32(v138.Text);
 			nivells[13,10] = Convert.ToInt32(v139.Text);
 		}
		private bool Valida(string columna) {
 			string par; int n1, n2, nv=0;
 			for (int nr=0; nr<7; nr++) rks[nr,2]=0;
 			for (int nr=0; nr<14; nr++) {
 				n1=nivells[nr,0]-1; n2=nivells[nr,1]-1;
 				if (n1<0 || n2<0) continue;
 				columna = columna.ToUpper();
 				par=columna.Substring(n1,1)+columna.Substring(n2,1);
 				switch (par) {
 					case "11": nv=nivells[nr,2]; break;
 					case "1X": nv=nivells[nr,3]; break;
 					case "12": nv=nivells[nr,4]; break;
 					case "X1": nv=nivells[nr,5]; break;
 					case "XX": nv=nivells[nr,6]; break;
 					case "X2": nv=nivells[nr,7]; break;
 					case "21": nv=nivells[nr,8]; break;
 					case "2X": nv=nivells[nr,9]; break;
 					case "22": nv=nivells[nr,10]; break;
 				}
 				if (nv>0) rks[nv-1,2]++;
 			}
 			for (int nr=0; nr<7; nr++) {
 				if (rks[nr,2]<rks[nr,0] || rks[nr,2]>rks[nr,1]) return false;
 			}
 			return true;
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
		private void GrabarCols() {
			bCalcular.Visible = false;
			bGrabar.Enabled = false;
			bGrabar.Text = "grabando";
			SaveFileDialog resul = new SaveFileDialog();
			resul.InitialDirectory = ".\\" ;
			resul.Filter = "Resultados(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
			if(resul.ShowDialog() == DialogResult.OK) {
				Application.DoEvents();
		   		tmp = resul.FileName;
		   		fileout = Path.GetFileName(tmp);
				StreamWriter sw = new StreamWriter(fileout);
				for (int nr=0; nr<4782969; nr++) {
					if (validas[nr]) {
						columna = n1s(nr);
						sw.WriteLine( columna );
					}
				}
				sw.Close();
			}
			bGrabar.Text = "grabar resultado";
			bCalcular.Visible = true;
		}
 		private void Analizar() {
 			RecuperarPantalla();
 			bool nx = Valida(tCG.Text);
 			r1.Text = ""+rks[0,2];
 			r2.Text = ""+rks[1,2];
 			r3.Text = ""+rks[2,2];
 			r4.Text = ""+rks[3,2];
 			r5.Text = ""+rks[4,2];
 			r6.Text = ""+rks[5,2];
 			r7.Text = ""+rks[6,2];
 		}
 		
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ParejasFrm));
            this.label9 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.p21 = new System.Windows.Forms.TextBox();
            this.v125 = new System.Windows.Forms.TextBox();
            this.v121 = new System.Windows.Forms.TextBox();
            this.bAnalizar = new System.Windows.Forms.Button();
            this.tCG = new System.Windows.Forms.TextBox();
            this.c51 = new System.Windows.Forms.TextBox();
            this.c52 = new System.Windows.Forms.TextBox();
            this.p12 = new System.Windows.Forms.TextBox();
            this.p11 = new System.Windows.Forms.TextBox();
            this.p92 = new System.Windows.Forms.TextBox();
            this.p91 = new System.Windows.Forms.TextBox();
            this.v133 = new System.Windows.Forms.TextBox();
            this.bCancelar = new System.Windows.Forms.Button();
            this.v131 = new System.Windows.Forms.TextBox();
            this.c22 = new System.Windows.Forms.TextBox();
            this.c21 = new System.Windows.Forms.TextBox();
            this.v91 = new System.Windows.Forms.TextBox();
            this.p01 = new System.Windows.Forms.TextBox();
            this.bGrabar = new System.Windows.Forms.Button();
            this.p81 = new System.Windows.Forms.TextBox();
            this.v107 = new System.Windows.Forms.TextBox();
            this.p112 = new System.Windows.Forms.TextBox();
            this.v84 = new System.Windows.Forms.TextBox();
            this.v85 = new System.Windows.Forms.TextBox();
            this.c72 = new System.Windows.Forms.TextBox();
            this.v81 = new System.Windows.Forms.TextBox();
            this.p71 = new System.Windows.Forms.TextBox();
            this.bCalcular = new System.Windows.Forms.Button();
            this.v59 = new System.Windows.Forms.TextBox();
            this.v116 = new System.Windows.Forms.TextBox();
            this.p131 = new System.Windows.Forms.TextBox();
            this.c41 = new System.Windows.Forms.TextBox();
            this.v111 = new System.Windows.Forms.TextBox();
            this.v112 = new System.Windows.Forms.TextBox();
            this.c42 = new System.Windows.Forms.TextBox();
            this.v95 = new System.Windows.Forms.TextBox();
            this.v94 = new System.Windows.Forms.TextBox();
            this.v97 = new System.Windows.Forms.TextBox();
            this.v96 = new System.Windows.Forms.TextBox();
            this.p22 = new System.Windows.Forms.TextBox();
            this.v93 = new System.Windows.Forms.TextBox();
            this.v92 = new System.Windows.Forms.TextBox();
            this.v99 = new System.Windows.Forms.TextBox();
            this.v98 = new System.Windows.Forms.TextBox();
            this.bSalvar = new System.Windows.Forms.Button();
            this.v86 = new System.Windows.Forms.TextBox();
            this.v87 = new System.Windows.Forms.TextBox();
            this.v82 = new System.Windows.Forms.TextBox();
            this.v83 = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.v88 = new System.Windows.Forms.TextBox();
            this.v89 = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.v139 = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.p02 = new System.Windows.Forms.TextBox();
            this.ltime = new System.Windows.Forms.Label();
            this.v105 = new System.Windows.Forms.TextBox();
            this.v104 = new System.Windows.Forms.TextBox();
            this.v69 = new System.Windows.Forms.TextBox();
            this.v103 = new System.Windows.Forms.TextBox();
            this.p132 = new System.Windows.Forms.TextBox();
            this.c61 = new System.Windows.Forms.TextBox();
            this.lproc = new System.Windows.Forms.Label();
            this.p31 = new System.Windows.Forms.TextBox();
            this.p32 = new System.Windows.Forms.TextBox();
            this.r1 = new System.Windows.Forms.Label();
            this.r2 = new System.Windows.Forms.Label();
            this.r3 = new System.Windows.Forms.Label();
            this.r4 = new System.Windows.Forms.Label();
            this.r5 = new System.Windows.Forms.Label();
            this.r6 = new System.Windows.Forms.Label();
            this.r7 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.v118 = new System.Windows.Forms.TextBox();
            this.v119 = new System.Windows.Forms.TextBox();
            this.v58 = new System.Windows.Forms.TextBox();
            this.v114 = new System.Windows.Forms.TextBox();
            this.v115 = new System.Windows.Forms.TextBox();
            this.c32 = new System.Windows.Forms.TextBox();
            this.v117 = new System.Windows.Forms.TextBox();
            this.v51 = new System.Windows.Forms.TextBox();
            this.v53 = new System.Windows.Forms.TextBox();
            this.v52 = new System.Windows.Forms.TextBox();
            this.v55 = new System.Windows.Forms.TextBox();
            this.v54 = new System.Windows.Forms.TextBox();
            this.v57 = new System.Windows.Forms.TextBox();
            this.v56 = new System.Windows.Forms.TextBox();
            this.bLeer = new System.Windows.Forms.Button();
            this.v126 = new System.Windows.Forms.TextBox();
            this.label25 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.v122 = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.v48 = new System.Windows.Forms.TextBox();
            this.v49 = new System.Windows.Forms.TextBox();
            this.v41 = new System.Windows.Forms.TextBox();
            this.v42 = new System.Windows.Forms.TextBox();
            this.v43 = new System.Windows.Forms.TextBox();
            this.v44 = new System.Windows.Forms.TextBox();
            this.v45 = new System.Windows.Forms.TextBox();
            this.v46 = new System.Windows.Forms.TextBox();
            this.v47 = new System.Windows.Forms.TextBox();
            this.p121 = new System.Windows.Forms.TextBox();
            this.p122 = new System.Windows.Forms.TextBox();
            this.v138 = new System.Windows.Forms.TextBox();
            this.v79 = new System.Windows.Forms.TextBox();
            this.v78 = new System.Windows.Forms.TextBox();
            this.v137 = new System.Windows.Forms.TextBox();
            this.v134 = new System.Windows.Forms.TextBox();
            this.v135 = new System.Windows.Forms.TextBox();
            this.v132 = new System.Windows.Forms.TextBox();
            this.v73 = new System.Windows.Forms.TextBox();
            this.v72 = new System.Windows.Forms.TextBox();
            this.v71 = new System.Windows.Forms.TextBox();
            this.v77 = new System.Windows.Forms.TextBox();
            this.v76 = new System.Windows.Forms.TextBox();
            this.v75 = new System.Windows.Forms.TextBox();
            this.v74 = new System.Windows.Forms.TextBox();
            this.v109 = new System.Windows.Forms.TextBox();
            this.v108 = new System.Windows.Forms.TextBox();
            this.v68 = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.v136 = new System.Windows.Forms.TextBox();
            this.v129 = new System.Windows.Forms.TextBox();
            this.v128 = new System.Windows.Forms.TextBox();
            this.v127 = new System.Windows.Forms.TextBox();
            this.v124 = new System.Windows.Forms.TextBox();
            this.v123 = new System.Windows.Forms.TextBox();
            this.v113 = new System.Windows.Forms.TextBox();
            this.p111 = new System.Windows.Forms.TextBox();
            this.v106 = new System.Windows.Forms.TextBox();
            this.v102 = new System.Windows.Forms.TextBox();
            this.v101 = new System.Windows.Forms.TextBox();
            this.p102 = new System.Windows.Forms.TextBox();
            this.p101 = new System.Windows.Forms.TextBox();
            this.p82 = new System.Windows.Forms.TextBox();
            this.p72 = new System.Windows.Forms.TextBox();
            this.v67 = new System.Windows.Forms.TextBox();
            this.v66 = new System.Windows.Forms.TextBox();
            this.v65 = new System.Windows.Forms.TextBox();
            this.v64 = new System.Windows.Forms.TextBox();
            this.v63 = new System.Windows.Forms.TextBox();
            this.v62 = new System.Windows.Forms.TextBox();
            this.v61 = new System.Windows.Forms.TextBox();
            this.p62 = new System.Windows.Forms.TextBox();
            this.p61 = new System.Windows.Forms.TextBox();
            this.p52 = new System.Windows.Forms.TextBox();
            this.p51 = new System.Windows.Forms.TextBox();
            this.p42 = new System.Windows.Forms.TextBox();
            this.p41 = new System.Windows.Forms.TextBox();
            this.v39 = new System.Windows.Forms.TextBox();
            this.v38 = new System.Windows.Forms.TextBox();
            this.v37 = new System.Windows.Forms.TextBox();
            this.v36 = new System.Windows.Forms.TextBox();
            this.v35 = new System.Windows.Forms.TextBox();
            this.v34 = new System.Windows.Forms.TextBox();
            this.v33 = new System.Windows.Forms.TextBox();
            this.v32 = new System.Windows.Forms.TextBox();
            this.v31 = new System.Windows.Forms.TextBox();
            this.v29 = new System.Windows.Forms.TextBox();
            this.v28 = new System.Windows.Forms.TextBox();
            this.v27 = new System.Windows.Forms.TextBox();
            this.v26 = new System.Windows.Forms.TextBox();
            this.v25 = new System.Windows.Forms.TextBox();
            this.v24 = new System.Windows.Forms.TextBox();
            this.v23 = new System.Windows.Forms.TextBox();
            this.v22 = new System.Windows.Forms.TextBox();
            this.v21 = new System.Windows.Forms.TextBox();
            this.v19 = new System.Windows.Forms.TextBox();
            this.v18 = new System.Windows.Forms.TextBox();
            this.v17 = new System.Windows.Forms.TextBox();
            this.v16 = new System.Windows.Forms.TextBox();
            this.v15 = new System.Windows.Forms.TextBox();
            this.v14 = new System.Windows.Forms.TextBox();
            this.v13 = new System.Windows.Forms.TextBox();
            this.v12 = new System.Windows.Forms.TextBox();
            this.v11 = new System.Windows.Forms.TextBox();
            this.v09 = new System.Windows.Forms.TextBox();
            this.v08 = new System.Windows.Forms.TextBox();
            this.v07 = new System.Windows.Forms.TextBox();
            this.v06 = new System.Windows.Forms.TextBox();
            this.v05 = new System.Windows.Forms.TextBox();
            this.v04 = new System.Windows.Forms.TextBox();
            this.v03 = new System.Windows.Forms.TextBox();
            this.v02 = new System.Windows.Forms.TextBox();
            this.v01 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.c71 = new System.Windows.Forms.TextBox();
            this.c62 = new System.Windows.Forms.TextBox();
            this.c31 = new System.Windows.Forms.TextBox();
            this.c11 = new System.Windows.Forms.TextBox();
            this.c12 = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.lval = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // label9
            // 
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label9.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label9.Location = new System.Drawing.Point(168, 24);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(32, 24);
            this.label9.TabIndex = 8;
            this.label9.Text = "x1";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label4.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label4.Location = new System.Drawing.Point(296, 24);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 24);
            this.label4.TabIndex = 3;
            this.label4.Text = "2x";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label5.Location = new System.Drawing.Point(104, 24);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 24);
            this.label5.TabIndex = 6;
            this.label5.Text = "1x";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label6.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label6.Location = new System.Drawing.Point(72, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(32, 24);
            this.label6.TabIndex = 5;
            this.label6.Text = "11";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label7.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label7.Location = new System.Drawing.Point(328, 24);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(32, 24);
            this.label7.TabIndex = 4;
            this.label7.Text = "22";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(8, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "Par";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label2.Location = new System.Drawing.Point(232, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 24);
            this.label2.TabIndex = 1;
            this.label2.Text = "x2";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label3.Location = new System.Drawing.Point(264, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(32, 24);
            this.label3.TabIndex = 2;
            this.label3.Text = "21";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // p21
            // 
            this.p21.BackColor = System.Drawing.Color.LemonChiffon;
            this.p21.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p21.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.p21.Location = new System.Drawing.Point(8, 96);
            this.p21.Name = "p21";
            this.p21.Size = new System.Drawing.Size(32, 20);
            this.p21.TabIndex = 32;
            this.p21.Text = "0";
            this.p21.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v125
            // 
            this.v125.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v125.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v125.Location = new System.Drawing.Point(200, 336);
            this.v125.Name = "v125";
            this.v125.Size = new System.Drawing.Size(32, 20);
            this.v125.TabIndex = 148;
            this.v125.Text = "0";
            this.v125.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v121
            // 
            this.v121.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v121.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v121.Location = new System.Drawing.Point(72, 336);
            this.v121.Name = "v121";
            this.v121.Size = new System.Drawing.Size(32, 20);
            this.v121.TabIndex = 144;
            this.v121.Text = "0";
            this.v121.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bAnalizar
            // 
            this.bAnalizar.BackColor = System.Drawing.Color.DarkSalmon;
            this.bAnalizar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.bAnalizar.Image = ((System.Drawing.Image)(resources.GetObject("bAnalizar.Image")));
            this.bAnalizar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bAnalizar.Location = new System.Drawing.Point(16, 80);
            this.bAnalizar.Name = "bAnalizar";
            this.bAnalizar.Size = new System.Drawing.Size(128, 32);
            this.bAnalizar.TabIndex = 148;
            this.bAnalizar.Text = "Analizar";
            this.bAnalizar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bAnalizar.UseVisualStyleBackColor = false;
            this.bAnalizar.Click += new System.EventHandler(this.BAnalizarClick);
            // 
            // tCG
            // 
            this.tCG.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tCG.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.tCG.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.tCG.Location = new System.Drawing.Point(8, 48);
            this.tCG.Name = "tCG";
            this.tCG.Size = new System.Drawing.Size(152, 20);
            this.tCG.TabIndex = 147;
            this.tCG.Text = "111X11X222X111";
            this.tCG.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // c51
            // 
            this.c51.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c51.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.c51.Location = new System.Drawing.Point(40, 169);
            this.c51.Name = "c51";
            this.c51.Size = new System.Drawing.Size(40, 20);
            this.c51.TabIndex = 9;
            this.c51.Text = "0";
            this.c51.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // c52
            // 
            this.c52.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c52.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.c52.Location = new System.Drawing.Point(81, 169);
            this.c52.Name = "c52";
            this.c52.Size = new System.Drawing.Size(40, 20);
            this.c52.TabIndex = 10;
            this.c52.Text = "10";
            this.c52.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // p12
            // 
            this.p12.BackColor = System.Drawing.Color.LemonChiffon;
            this.p12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p12.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.p12.Location = new System.Drawing.Point(40, 72);
            this.p12.Name = "p12";
            this.p12.Size = new System.Drawing.Size(32, 20);
            this.p12.TabIndex = 22;
            this.p12.Text = "0";
            this.p12.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // p11
            // 
            this.p11.BackColor = System.Drawing.Color.LemonChiffon;
            this.p11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p11.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.p11.Location = new System.Drawing.Point(8, 72);
            this.p11.Name = "p11";
            this.p11.Size = new System.Drawing.Size(32, 20);
            this.p11.TabIndex = 21;
            this.p11.Text = "0";
            this.p11.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // p92
            // 
            this.p92.BackColor = System.Drawing.Color.LemonChiffon;
            this.p92.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p92.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.p92.Location = new System.Drawing.Point(40, 264);
            this.p92.Name = "p92";
            this.p92.Size = new System.Drawing.Size(32, 20);
            this.p92.TabIndex = 110;
            this.p92.Text = "0";
            this.p92.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // p91
            // 
            this.p91.BackColor = System.Drawing.Color.LemonChiffon;
            this.p91.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p91.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.p91.Location = new System.Drawing.Point(8, 264);
            this.p91.Name = "p91";
            this.p91.Size = new System.Drawing.Size(32, 20);
            this.p91.TabIndex = 109;
            this.p91.Text = "0";
            this.p91.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v133
            // 
            this.v133.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v133.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v133.Location = new System.Drawing.Point(136, 360);
            this.v133.Name = "v133";
            this.v133.Size = new System.Drawing.Size(32, 20);
            this.v133.TabIndex = 157;
            this.v133.Text = "0";
            this.v133.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bCancelar
            // 
            this.bCancelar.BackColor = System.Drawing.Color.DarkSalmon;
            this.bCancelar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.bCancelar.Image = ((System.Drawing.Image)(resources.GetObject("bCancelar.Image")));
            this.bCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bCancelar.Location = new System.Drawing.Point(568, 168);
            this.bCancelar.Name = "bCancelar";
            this.bCancelar.Size = new System.Drawing.Size(100, 32);
            this.bCancelar.TabIndex = 5;
            this.bCancelar.Text = "Cancelar";
            this.bCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bCancelar.UseVisualStyleBackColor = false;
            this.bCancelar.Click += new System.EventHandler(this.BCancelarClick);
            // 
            // v131
            // 
            this.v131.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v131.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v131.Location = new System.Drawing.Point(72, 360);
            this.v131.Name = "v131";
            this.v131.Size = new System.Drawing.Size(32, 20);
            this.v131.TabIndex = 155;
            this.v131.Text = "0";
            this.v131.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // c22
            // 
            this.c22.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c22.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.c22.Location = new System.Drawing.Point(81, 97);
            this.c22.Name = "c22";
            this.c22.Size = new System.Drawing.Size(40, 20);
            this.c22.TabIndex = 4;
            this.c22.Text = "10";
            this.c22.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // c21
            // 
            this.c21.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c21.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.c21.Location = new System.Drawing.Point(40, 97);
            this.c21.Name = "c21";
            this.c21.Size = new System.Drawing.Size(40, 20);
            this.c21.TabIndex = 3;
            this.c21.Text = "0";
            this.c21.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v91
            // 
            this.v91.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v91.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v91.Location = new System.Drawing.Point(72, 264);
            this.v91.Name = "v91";
            this.v91.Size = new System.Drawing.Size(32, 20);
            this.v91.TabIndex = 111;
            this.v91.Text = "0";
            this.v91.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // p01
            // 
            this.p01.BackColor = System.Drawing.Color.LemonChiffon;
            this.p01.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p01.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.p01.Location = new System.Drawing.Point(8, 48);
            this.p01.Name = "p01";
            this.p01.Size = new System.Drawing.Size(32, 20);
            this.p01.TabIndex = 10;
            this.p01.Text = "0";
            this.p01.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bGrabar
            // 
            this.bGrabar.BackColor = System.Drawing.Color.DarkSalmon;
            this.bGrabar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.bGrabar.Image = ((System.Drawing.Image)(resources.GetObject("bGrabar.Image")));
            this.bGrabar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bGrabar.Location = new System.Drawing.Point(568, 208);
            this.bGrabar.Name = "bGrabar";
            this.bGrabar.Size = new System.Drawing.Size(100, 32);
            this.bGrabar.TabIndex = 6;
            this.bGrabar.Text = "Grabar";
            this.bGrabar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bGrabar.UseVisualStyleBackColor = false;
            this.bGrabar.Click += new System.EventHandler(this.BGrabarClick);
            // 
            // p81
            // 
            this.p81.BackColor = System.Drawing.Color.LemonChiffon;
            this.p81.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p81.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.p81.Location = new System.Drawing.Point(8, 240);
            this.p81.Name = "p81";
            this.p81.Size = new System.Drawing.Size(32, 20);
            this.p81.TabIndex = 98;
            this.p81.Text = "0";
            this.p81.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v107
            // 
            this.v107.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v107.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v107.Location = new System.Drawing.Point(264, 288);
            this.v107.Name = "v107";
            this.v107.Size = new System.Drawing.Size(32, 20);
            this.v107.TabIndex = 128;
            this.v107.Text = "0";
            this.v107.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // p112
            // 
            this.p112.BackColor = System.Drawing.Color.LemonChiffon;
            this.p112.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p112.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.p112.Location = new System.Drawing.Point(40, 312);
            this.p112.Name = "p112";
            this.p112.Size = new System.Drawing.Size(32, 20);
            this.p112.TabIndex = 132;
            this.p112.Text = "0";
            this.p112.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v84
            // 
            this.v84.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v84.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v84.Location = new System.Drawing.Point(168, 240);
            this.v84.Name = "v84";
            this.v84.Size = new System.Drawing.Size(32, 20);
            this.v84.TabIndex = 103;
            this.v84.Text = "0";
            this.v84.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v85
            // 
            this.v85.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v85.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v85.Location = new System.Drawing.Point(200, 240);
            this.v85.Name = "v85";
            this.v85.Size = new System.Drawing.Size(32, 20);
            this.v85.TabIndex = 104;
            this.v85.Text = "0";
            this.v85.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // c72
            // 
            this.c72.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c72.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.c72.Location = new System.Drawing.Point(81, 217);
            this.c72.Name = "c72";
            this.c72.Size = new System.Drawing.Size(40, 20);
            this.c72.TabIndex = 146;
            this.c72.Text = "10";
            this.c72.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v81
            // 
            this.v81.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v81.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v81.Location = new System.Drawing.Point(72, 240);
            this.v81.Name = "v81";
            this.v81.Size = new System.Drawing.Size(32, 20);
            this.v81.TabIndex = 100;
            this.v81.Text = "0";
            this.v81.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // p71
            // 
            this.p71.BackColor = System.Drawing.Color.LemonChiffon;
            this.p71.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p71.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.p71.Location = new System.Drawing.Point(8, 216);
            this.p71.Name = "p71";
            this.p71.Size = new System.Drawing.Size(32, 20);
            this.p71.TabIndex = 87;
            this.p71.Text = "0";
            this.p71.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bCalcular
            // 
            this.bCalcular.BackColor = System.Drawing.Color.DarkSalmon;
            this.bCalcular.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.bCalcular.Image = ((System.Drawing.Image)(resources.GetObject("bCalcular.Image")));
            this.bCalcular.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bCalcular.Location = new System.Drawing.Point(568, 24);
            this.bCalcular.Name = "bCalcular";
            this.bCalcular.Size = new System.Drawing.Size(100, 32);
            this.bCalcular.TabIndex = 4;
            this.bCalcular.Text = "Calcular";
            this.bCalcular.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bCalcular.UseVisualStyleBackColor = false;
            this.bCalcular.Click += new System.EventHandler(this.BCalcularClick);
            // 
            // v59
            // 
            this.v59.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v59.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v59.Location = new System.Drawing.Point(328, 168);
            this.v59.Name = "v59";
            this.v59.Size = new System.Drawing.Size(32, 20);
            this.v59.TabIndex = 75;
            this.v59.Text = "0";
            this.v59.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v116
            // 
            this.v116.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v116.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v116.Location = new System.Drawing.Point(232, 312);
            this.v116.Name = "v116";
            this.v116.Size = new System.Drawing.Size(32, 20);
            this.v116.TabIndex = 138;
            this.v116.Text = "0";
            this.v116.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // p131
            // 
            this.p131.BackColor = System.Drawing.Color.LemonChiffon;
            this.p131.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p131.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.p131.Location = new System.Drawing.Point(8, 360);
            this.p131.Name = "p131";
            this.p131.Size = new System.Drawing.Size(32, 20);
            this.p131.TabIndex = 153;
            this.p131.Text = "0";
            this.p131.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // c41
            // 
            this.c41.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c41.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.c41.Location = new System.Drawing.Point(40, 145);
            this.c41.Name = "c41";
            this.c41.Size = new System.Drawing.Size(40, 20);
            this.c41.TabIndex = 7;
            this.c41.Text = "0";
            this.c41.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v111
            // 
            this.v111.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v111.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v111.Location = new System.Drawing.Point(72, 312);
            this.v111.Name = "v111";
            this.v111.Size = new System.Drawing.Size(32, 20);
            this.v111.TabIndex = 133;
            this.v111.Text = "0";
            this.v111.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v112
            // 
            this.v112.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v112.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v112.Location = new System.Drawing.Point(104, 312);
            this.v112.Name = "v112";
            this.v112.Size = new System.Drawing.Size(32, 20);
            this.v112.TabIndex = 134;
            this.v112.Text = "0";
            this.v112.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // c42
            // 
            this.c42.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c42.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.c42.Location = new System.Drawing.Point(81, 145);
            this.c42.Name = "c42";
            this.c42.Size = new System.Drawing.Size(40, 20);
            this.c42.TabIndex = 8;
            this.c42.Text = "10";
            this.c42.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v95
            // 
            this.v95.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v95.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v95.Location = new System.Drawing.Point(200, 264);
            this.v95.Name = "v95";
            this.v95.Size = new System.Drawing.Size(32, 20);
            this.v95.TabIndex = 115;
            this.v95.Text = "0";
            this.v95.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v94
            // 
            this.v94.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v94.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v94.Location = new System.Drawing.Point(168, 264);
            this.v94.Name = "v94";
            this.v94.Size = new System.Drawing.Size(32, 20);
            this.v94.TabIndex = 114;
            this.v94.Text = "0";
            this.v94.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v97
            // 
            this.v97.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v97.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v97.Location = new System.Drawing.Point(264, 264);
            this.v97.Name = "v97";
            this.v97.Size = new System.Drawing.Size(32, 20);
            this.v97.TabIndex = 117;
            this.v97.Text = "0";
            this.v97.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v96
            // 
            this.v96.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v96.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v96.Location = new System.Drawing.Point(232, 264);
            this.v96.Name = "v96";
            this.v96.Size = new System.Drawing.Size(32, 20);
            this.v96.TabIndex = 116;
            this.v96.Text = "0";
            this.v96.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // p22
            // 
            this.p22.BackColor = System.Drawing.Color.LemonChiffon;
            this.p22.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p22.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.p22.Location = new System.Drawing.Point(40, 96);
            this.p22.Name = "p22";
            this.p22.Size = new System.Drawing.Size(32, 20);
            this.p22.TabIndex = 33;
            this.p22.Text = "0";
            this.p22.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v93
            // 
            this.v93.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v93.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v93.Location = new System.Drawing.Point(136, 264);
            this.v93.Name = "v93";
            this.v93.Size = new System.Drawing.Size(32, 20);
            this.v93.TabIndex = 113;
            this.v93.Text = "0";
            this.v93.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v92
            // 
            this.v92.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v92.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v92.Location = new System.Drawing.Point(104, 264);
            this.v92.Name = "v92";
            this.v92.Size = new System.Drawing.Size(32, 20);
            this.v92.TabIndex = 112;
            this.v92.Text = "0";
            this.v92.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v99
            // 
            this.v99.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v99.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v99.Location = new System.Drawing.Point(328, 264);
            this.v99.Name = "v99";
            this.v99.Size = new System.Drawing.Size(32, 20);
            this.v99.TabIndex = 119;
            this.v99.Text = "0";
            this.v99.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v98
            // 
            this.v98.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v98.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v98.Location = new System.Drawing.Point(296, 264);
            this.v98.Name = "v98";
            this.v98.Size = new System.Drawing.Size(32, 20);
            this.v98.TabIndex = 118;
            this.v98.Text = "0";
            this.v98.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bSalvar
            // 
            this.bSalvar.BackColor = System.Drawing.Color.DarkSalmon;
            this.bSalvar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.bSalvar.Image = ((System.Drawing.Image)(resources.GetObject("bSalvar.Image")));
            this.bSalvar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bSalvar.Location = new System.Drawing.Point(160, 408);
            this.bSalvar.Name = "bSalvar";
            this.bSalvar.Size = new System.Drawing.Size(128, 32);
            this.bSalvar.TabIndex = 2;
            this.bSalvar.Text = "Salvar Condiciones";
            this.bSalvar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bSalvar.UseVisualStyleBackColor = false;
            this.bSalvar.Click += new System.EventHandler(this.BSalvarClick);
            // 
            // v86
            // 
            this.v86.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v86.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v86.Location = new System.Drawing.Point(232, 240);
            this.v86.Name = "v86";
            this.v86.Size = new System.Drawing.Size(32, 20);
            this.v86.TabIndex = 105;
            this.v86.Text = "0";
            this.v86.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v87
            // 
            this.v87.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v87.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v87.Location = new System.Drawing.Point(264, 240);
            this.v87.Name = "v87";
            this.v87.Size = new System.Drawing.Size(32, 20);
            this.v87.TabIndex = 106;
            this.v87.Text = "0";
            this.v87.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v82
            // 
            this.v82.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v82.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v82.Location = new System.Drawing.Point(104, 240);
            this.v82.Name = "v82";
            this.v82.Size = new System.Drawing.Size(32, 20);
            this.v82.TabIndex = 101;
            this.v82.Text = "0";
            this.v82.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v83
            // 
            this.v83.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v83.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v83.Location = new System.Drawing.Point(136, 240);
            this.v83.Name = "v83";
            this.v83.Size = new System.Drawing.Size(32, 20);
            this.v83.TabIndex = 102;
            this.v83.Text = "0";
            this.v83.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label20
            // 
            this.label20.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label20.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label20.Location = new System.Drawing.Point(7, 193);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(32, 20);
            this.label20.TabIndex = 148;
            this.label20.Text = "6";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // v88
            // 
            this.v88.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v88.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v88.Location = new System.Drawing.Point(296, 240);
            this.v88.Name = "v88";
            this.v88.Size = new System.Drawing.Size(32, 20);
            this.v88.TabIndex = 107;
            this.v88.Text = "0";
            this.v88.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v89
            // 
            this.v89.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v89.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v89.Location = new System.Drawing.Point(328, 240);
            this.v89.Name = "v89";
            this.v89.Size = new System.Drawing.Size(32, 20);
            this.v89.TabIndex = 108;
            this.v89.Text = "0";
            this.v89.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label19
            // 
            this.label19.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label19.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label19.Location = new System.Drawing.Point(7, 121);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(32, 20);
            this.label19.TabIndex = 131;
            this.label19.Text = "3";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // v139
            // 
            this.v139.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v139.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v139.Location = new System.Drawing.Point(328, 360);
            this.v139.Name = "v139";
            this.v139.Size = new System.Drawing.Size(32, 20);
            this.v139.TabIndex = 163;
            this.v139.Text = "0";
            this.v139.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label15
            // 
            this.label15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label15.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label15.Location = new System.Drawing.Point(40, 48);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(40, 24);
            this.label15.TabIndex = 90;
            this.label15.Text = "min";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label14
            // 
            this.label14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label14.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label14.Location = new System.Drawing.Point(7, 48);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(32, 24);
            this.label14.TabIndex = 10;
            this.label14.Text = "niv";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label17
            // 
            this.label17.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label17.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label17.Location = new System.Drawing.Point(7, 73);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(32, 20);
            this.label17.TabIndex = 13;
            this.label17.Text = "1";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label16
            // 
            this.label16.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label16.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label16.Location = new System.Drawing.Point(81, 48);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(40, 24);
            this.label16.TabIndex = 80;
            this.label16.Text = "max";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            this.label11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label11.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label11.Location = new System.Drawing.Point(7, 97);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(32, 20);
            this.label11.TabIndex = 127;
            this.label11.Text = "2";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label10.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label10.Location = new System.Drawing.Point(136, 24);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(32, 24);
            this.label10.TabIndex = 7;
            this.label10.Text = "12";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label13
            // 
            this.label13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label13.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label13.Location = new System.Drawing.Point(122, 48);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(32, 24);
            this.label13.TabIndex = 70;
            this.label13.Text = "rdo";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label12
            // 
            this.label12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label12.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label12.Location = new System.Drawing.Point(7, 217);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(32, 20);
            this.label12.TabIndex = 150;
            this.label12.Text = "7";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // p02
            // 
            this.p02.BackColor = System.Drawing.Color.LemonChiffon;
            this.p02.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p02.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.p02.Location = new System.Drawing.Point(40, 48);
            this.p02.Name = "p02";
            this.p02.Size = new System.Drawing.Size(32, 20);
            this.p02.TabIndex = 11;
            this.p02.Text = "0";
            this.p02.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ltime
            // 
            this.ltime.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ltime.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.ltime.Location = new System.Drawing.Point(568, 121);
            this.ltime.Name = "ltime";
            this.ltime.Size = new System.Drawing.Size(100, 24);
            this.ltime.TabIndex = 143;
            this.ltime.Text = "Tiempo";
            this.ltime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // v105
            // 
            this.v105.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v105.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v105.Location = new System.Drawing.Point(200, 288);
            this.v105.Name = "v105";
            this.v105.Size = new System.Drawing.Size(32, 20);
            this.v105.TabIndex = 126;
            this.v105.Text = "0";
            this.v105.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v104
            // 
            this.v104.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v104.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v104.Location = new System.Drawing.Point(168, 288);
            this.v104.Name = "v104";
            this.v104.Size = new System.Drawing.Size(32, 20);
            this.v104.TabIndex = 125;
            this.v104.Text = "0";
            this.v104.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v69
            // 
            this.v69.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v69.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v69.Location = new System.Drawing.Point(328, 192);
            this.v69.Name = "v69";
            this.v69.Size = new System.Drawing.Size(32, 20);
            this.v69.TabIndex = 86;
            this.v69.Text = "0";
            this.v69.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v103
            // 
            this.v103.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v103.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v103.Location = new System.Drawing.Point(136, 288);
            this.v103.Name = "v103";
            this.v103.Size = new System.Drawing.Size(32, 20);
            this.v103.TabIndex = 124;
            this.v103.Text = "0";
            this.v103.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // p132
            // 
            this.p132.BackColor = System.Drawing.Color.LemonChiffon;
            this.p132.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p132.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.p132.Location = new System.Drawing.Point(40, 360);
            this.p132.Name = "p132";
            this.p132.Size = new System.Drawing.Size(32, 20);
            this.p132.TabIndex = 154;
            this.p132.Text = "0";
            this.p132.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // c61
            // 
            this.c61.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c61.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.c61.Location = new System.Drawing.Point(40, 193);
            this.c61.Name = "c61";
            this.c61.Size = new System.Drawing.Size(40, 20);
            this.c61.TabIndex = 143;
            this.c61.Text = "0";
            this.c61.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lproc
            // 
            this.lproc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lproc.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.lproc.Location = new System.Drawing.Point(568, 71);
            this.lproc.Name = "lproc";
            this.lproc.Size = new System.Drawing.Size(100, 24);
            this.lproc.TabIndex = 145;
            this.lproc.Text = "Procesadas";
            this.lproc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // p31
            // 
            this.p31.BackColor = System.Drawing.Color.LemonChiffon;
            this.p31.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p31.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.p31.Location = new System.Drawing.Point(8, 120);
            this.p31.Name = "p31";
            this.p31.Size = new System.Drawing.Size(32, 20);
            this.p31.TabIndex = 43;
            this.p31.Text = "0";
            this.p31.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // p32
            // 
            this.p32.BackColor = System.Drawing.Color.LemonChiffon;
            this.p32.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p32.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.p32.Location = new System.Drawing.Point(40, 120);
            this.p32.Name = "p32";
            this.p32.Size = new System.Drawing.Size(32, 20);
            this.p32.TabIndex = 44;
            this.p32.Text = "0";
            this.p32.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // r1
            // 
            this.r1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.r1.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.r1.Location = new System.Drawing.Point(122, 73);
            this.r1.Name = "r1";
            this.r1.Size = new System.Drawing.Size(32, 20);
            this.r1.TabIndex = 11;
            this.r1.Text = "-";
            this.r1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // r2
            // 
            this.r2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.r2.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.r2.Location = new System.Drawing.Point(122, 97);
            this.r2.Name = "r2";
            this.r2.Size = new System.Drawing.Size(32, 20);
            this.r2.TabIndex = 126;
            this.r2.Text = "-";
            this.r2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // r3
            // 
            this.r3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.r3.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.r3.Location = new System.Drawing.Point(122, 121);
            this.r3.Name = "r3";
            this.r3.Size = new System.Drawing.Size(32, 20);
            this.r3.TabIndex = 130;
            this.r3.Text = "-";
            this.r3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // r4
            // 
            this.r4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.r4.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.r4.Location = new System.Drawing.Point(122, 145);
            this.r4.Name = "r4";
            this.r4.Size = new System.Drawing.Size(32, 20);
            this.r4.TabIndex = 134;
            this.r4.Text = "-";
            this.r4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // r5
            // 
            this.r5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.r5.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.r5.Location = new System.Drawing.Point(122, 169);
            this.r5.Name = "r5";
            this.r5.Size = new System.Drawing.Size(32, 20);
            this.r5.TabIndex = 138;
            this.r5.Text = "-";
            this.r5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // r6
            // 
            this.r6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.r6.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.r6.Location = new System.Drawing.Point(122, 193);
            this.r6.Name = "r6";
            this.r6.Size = new System.Drawing.Size(32, 20);
            this.r6.TabIndex = 147;
            this.r6.Text = "-";
            this.r6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // r7
            // 
            this.r7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.r7.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.r7.Location = new System.Drawing.Point(122, 217);
            this.r7.Name = "r7";
            this.r7.Size = new System.Drawing.Size(32, 20);
            this.r7.TabIndex = 149;
            this.r7.Text = "-";
            this.r7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label30
            // 
            this.label30.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label30.Location = new System.Drawing.Point(8, 24);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(152, 24);
            this.label30.TabIndex = 150;
            this.label30.Text = "Columna Ganadora";
            this.label30.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // v118
            // 
            this.v118.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v118.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v118.Location = new System.Drawing.Point(296, 312);
            this.v118.Name = "v118";
            this.v118.Size = new System.Drawing.Size(32, 20);
            this.v118.TabIndex = 140;
            this.v118.Text = "0";
            this.v118.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v119
            // 
            this.v119.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v119.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v119.Location = new System.Drawing.Point(328, 312);
            this.v119.Name = "v119";
            this.v119.Size = new System.Drawing.Size(32, 20);
            this.v119.TabIndex = 141;
            this.v119.Text = "0";
            this.v119.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v58
            // 
            this.v58.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v58.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v58.Location = new System.Drawing.Point(296, 168);
            this.v58.Name = "v58";
            this.v58.Size = new System.Drawing.Size(32, 20);
            this.v58.TabIndex = 74;
            this.v58.Text = "0";
            this.v58.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v114
            // 
            this.v114.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v114.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v114.Location = new System.Drawing.Point(168, 312);
            this.v114.Name = "v114";
            this.v114.Size = new System.Drawing.Size(32, 20);
            this.v114.TabIndex = 136;
            this.v114.Text = "0";
            this.v114.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v115
            // 
            this.v115.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v115.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v115.Location = new System.Drawing.Point(200, 312);
            this.v115.Name = "v115";
            this.v115.Size = new System.Drawing.Size(32, 20);
            this.v115.TabIndex = 137;
            this.v115.Text = "0";
            this.v115.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // c32
            // 
            this.c32.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c32.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.c32.Location = new System.Drawing.Point(81, 121);
            this.c32.Name = "c32";
            this.c32.Size = new System.Drawing.Size(40, 20);
            this.c32.TabIndex = 6;
            this.c32.Text = "10";
            this.c32.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v117
            // 
            this.v117.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v117.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v117.Location = new System.Drawing.Point(264, 312);
            this.v117.Name = "v117";
            this.v117.Size = new System.Drawing.Size(32, 20);
            this.v117.TabIndex = 139;
            this.v117.Text = "0";
            this.v117.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v51
            // 
            this.v51.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v51.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v51.Location = new System.Drawing.Point(72, 168);
            this.v51.Name = "v51";
            this.v51.Size = new System.Drawing.Size(32, 20);
            this.v51.TabIndex = 67;
            this.v51.Text = "0";
            this.v51.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v53
            // 
            this.v53.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v53.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v53.Location = new System.Drawing.Point(136, 168);
            this.v53.Name = "v53";
            this.v53.Size = new System.Drawing.Size(32, 20);
            this.v53.TabIndex = 69;
            this.v53.Text = "0";
            this.v53.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v52
            // 
            this.v52.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v52.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v52.Location = new System.Drawing.Point(104, 168);
            this.v52.Name = "v52";
            this.v52.Size = new System.Drawing.Size(32, 20);
            this.v52.TabIndex = 68;
            this.v52.Text = "0";
            this.v52.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v55
            // 
            this.v55.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v55.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v55.Location = new System.Drawing.Point(200, 168);
            this.v55.Name = "v55";
            this.v55.Size = new System.Drawing.Size(32, 20);
            this.v55.TabIndex = 71;
            this.v55.Text = "0";
            this.v55.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v54
            // 
            this.v54.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v54.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v54.Location = new System.Drawing.Point(168, 168);
            this.v54.Name = "v54";
            this.v54.Size = new System.Drawing.Size(32, 20);
            this.v54.TabIndex = 70;
            this.v54.Text = "0";
            this.v54.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v57
            // 
            this.v57.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v57.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v57.Location = new System.Drawing.Point(264, 168);
            this.v57.Name = "v57";
            this.v57.Size = new System.Drawing.Size(32, 20);
            this.v57.TabIndex = 73;
            this.v57.Text = "0";
            this.v57.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v56
            // 
            this.v56.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v56.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v56.Location = new System.Drawing.Point(232, 168);
            this.v56.Name = "v56";
            this.v56.Size = new System.Drawing.Size(32, 20);
            this.v56.TabIndex = 72;
            this.v56.Text = "0";
            this.v56.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // bLeer
            // 
            this.bLeer.BackColor = System.Drawing.Color.DarkSalmon;
            this.bLeer.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.bLeer.Image = ((System.Drawing.Image)(resources.GetObject("bLeer.Image")));
            this.bLeer.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bLeer.Location = new System.Drawing.Point(16, 408);
            this.bLeer.Name = "bLeer";
            this.bLeer.Size = new System.Drawing.Size(128, 32);
            this.bLeer.TabIndex = 3;
            this.bLeer.Text = "Abrir Condiciones";
            this.bLeer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bLeer.UseVisualStyleBackColor = false;
            this.bLeer.Click += new System.EventHandler(this.BLeerClick);
            // 
            // v126
            // 
            this.v126.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v126.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v126.Location = new System.Drawing.Point(232, 336);
            this.v126.Name = "v126";
            this.v126.Size = new System.Drawing.Size(32, 20);
            this.v126.TabIndex = 149;
            this.v126.Text = "0";
            this.v126.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label25
            // 
            this.label25.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label25.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label25.Location = new System.Drawing.Point(40, 23);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(81, 24);
            this.label25.TabIndex = 142;
            this.label25.Text = "Límites";
            this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label23
            // 
            this.label23.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label23.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label23.Location = new System.Drawing.Point(7, 169);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(32, 20);
            this.label23.TabIndex = 139;
            this.label23.Text = "5";
            this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // v122
            // 
            this.v122.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v122.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v122.Location = new System.Drawing.Point(104, 336);
            this.v122.Name = "v122";
            this.v122.Size = new System.Drawing.Size(32, 20);
            this.v122.TabIndex = 145;
            this.v122.Text = "0";
            this.v122.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label21
            // 
            this.label21.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label21.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label21.Location = new System.Drawing.Point(7, 145);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(32, 20);
            this.label21.TabIndex = 135;
            this.label21.Text = "4";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // v48
            // 
            this.v48.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v48.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v48.Location = new System.Drawing.Point(296, 144);
            this.v48.Name = "v48";
            this.v48.Size = new System.Drawing.Size(32, 20);
            this.v48.TabIndex = 63;
            this.v48.Text = "0";
            this.v48.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v49
            // 
            this.v49.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v49.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v49.Location = new System.Drawing.Point(328, 144);
            this.v49.Name = "v49";
            this.v49.Size = new System.Drawing.Size(32, 20);
            this.v49.TabIndex = 64;
            this.v49.Text = "0";
            this.v49.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v41
            // 
            this.v41.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v41.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v41.Location = new System.Drawing.Point(72, 144);
            this.v41.Name = "v41";
            this.v41.Size = new System.Drawing.Size(32, 20);
            this.v41.TabIndex = 56;
            this.v41.Text = "0";
            this.v41.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v42
            // 
            this.v42.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v42.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v42.Location = new System.Drawing.Point(104, 144);
            this.v42.Name = "v42";
            this.v42.Size = new System.Drawing.Size(32, 20);
            this.v42.TabIndex = 57;
            this.v42.Text = "0";
            this.v42.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v43
            // 
            this.v43.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v43.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v43.Location = new System.Drawing.Point(136, 144);
            this.v43.Name = "v43";
            this.v43.Size = new System.Drawing.Size(32, 20);
            this.v43.TabIndex = 58;
            this.v43.Text = "0";
            this.v43.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v44
            // 
            this.v44.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v44.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v44.Location = new System.Drawing.Point(168, 144);
            this.v44.Name = "v44";
            this.v44.Size = new System.Drawing.Size(32, 20);
            this.v44.TabIndex = 59;
            this.v44.Text = "0";
            this.v44.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v45
            // 
            this.v45.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v45.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v45.Location = new System.Drawing.Point(200, 144);
            this.v45.Name = "v45";
            this.v45.Size = new System.Drawing.Size(32, 20);
            this.v45.TabIndex = 60;
            this.v45.Text = "0";
            this.v45.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v46
            // 
            this.v46.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v46.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v46.Location = new System.Drawing.Point(232, 144);
            this.v46.Name = "v46";
            this.v46.Size = new System.Drawing.Size(32, 20);
            this.v46.TabIndex = 61;
            this.v46.Text = "0";
            this.v46.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v47
            // 
            this.v47.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v47.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v47.Location = new System.Drawing.Point(264, 144);
            this.v47.Name = "v47";
            this.v47.Size = new System.Drawing.Size(32, 20);
            this.v47.TabIndex = 62;
            this.v47.Text = "0";
            this.v47.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // p121
            // 
            this.p121.BackColor = System.Drawing.Color.LemonChiffon;
            this.p121.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p121.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.p121.Location = new System.Drawing.Point(8, 336);
            this.p121.Name = "p121";
            this.p121.Size = new System.Drawing.Size(32, 20);
            this.p121.TabIndex = 142;
            this.p121.Text = "0";
            this.p121.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // p122
            // 
            this.p122.BackColor = System.Drawing.Color.LemonChiffon;
            this.p122.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p122.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.p122.Location = new System.Drawing.Point(40, 336);
            this.p122.Name = "p122";
            this.p122.Size = new System.Drawing.Size(32, 20);
            this.p122.TabIndex = 143;
            this.p122.Text = "0";
            this.p122.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v138
            // 
            this.v138.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v138.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v138.Location = new System.Drawing.Point(296, 360);
            this.v138.Name = "v138";
            this.v138.Size = new System.Drawing.Size(32, 20);
            this.v138.TabIndex = 162;
            this.v138.Text = "0";
            this.v138.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v79
            // 
            this.v79.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v79.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v79.Location = new System.Drawing.Point(328, 216);
            this.v79.Name = "v79";
            this.v79.Size = new System.Drawing.Size(32, 20);
            this.v79.TabIndex = 97;
            this.v79.Text = "0";
            this.v79.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v78
            // 
            this.v78.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v78.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v78.Location = new System.Drawing.Point(296, 216);
            this.v78.Name = "v78";
            this.v78.Size = new System.Drawing.Size(32, 20);
            this.v78.TabIndex = 96;
            this.v78.Text = "0";
            this.v78.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v137
            // 
            this.v137.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v137.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v137.Location = new System.Drawing.Point(264, 360);
            this.v137.Name = "v137";
            this.v137.Size = new System.Drawing.Size(32, 20);
            this.v137.TabIndex = 161;
            this.v137.Text = "0";
            this.v137.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v134
            // 
            this.v134.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v134.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v134.Location = new System.Drawing.Point(168, 360);
            this.v134.Name = "v134";
            this.v134.Size = new System.Drawing.Size(32, 20);
            this.v134.TabIndex = 158;
            this.v134.Text = "0";
            this.v134.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v135
            // 
            this.v135.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v135.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v135.Location = new System.Drawing.Point(200, 360);
            this.v135.Name = "v135";
            this.v135.Size = new System.Drawing.Size(32, 20);
            this.v135.TabIndex = 159;
            this.v135.Text = "0";
            this.v135.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v132
            // 
            this.v132.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v132.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v132.Location = new System.Drawing.Point(104, 360);
            this.v132.Name = "v132";
            this.v132.Size = new System.Drawing.Size(32, 20);
            this.v132.TabIndex = 156;
            this.v132.Text = "0";
            this.v132.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v73
            // 
            this.v73.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v73.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v73.Location = new System.Drawing.Point(136, 216);
            this.v73.Name = "v73";
            this.v73.Size = new System.Drawing.Size(32, 20);
            this.v73.TabIndex = 91;
            this.v73.Text = "0";
            this.v73.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v72
            // 
            this.v72.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v72.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v72.Location = new System.Drawing.Point(104, 216);
            this.v72.Name = "v72";
            this.v72.Size = new System.Drawing.Size(32, 20);
            this.v72.TabIndex = 90;
            this.v72.Text = "0";
            this.v72.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v71
            // 
            this.v71.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v71.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v71.Location = new System.Drawing.Point(72, 216);
            this.v71.Name = "v71";
            this.v71.Size = new System.Drawing.Size(32, 20);
            this.v71.TabIndex = 89;
            this.v71.Text = "0";
            this.v71.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v77
            // 
            this.v77.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v77.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v77.Location = new System.Drawing.Point(264, 216);
            this.v77.Name = "v77";
            this.v77.Size = new System.Drawing.Size(32, 20);
            this.v77.TabIndex = 95;
            this.v77.Text = "0";
            this.v77.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v76
            // 
            this.v76.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v76.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v76.Location = new System.Drawing.Point(232, 216);
            this.v76.Name = "v76";
            this.v76.Size = new System.Drawing.Size(32, 20);
            this.v76.TabIndex = 94;
            this.v76.Text = "0";
            this.v76.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v75
            // 
            this.v75.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v75.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v75.Location = new System.Drawing.Point(200, 216);
            this.v75.Name = "v75";
            this.v75.Size = new System.Drawing.Size(32, 20);
            this.v75.TabIndex = 93;
            this.v75.Text = "0";
            this.v75.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v74
            // 
            this.v74.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v74.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v74.Location = new System.Drawing.Point(168, 216);
            this.v74.Name = "v74";
            this.v74.Size = new System.Drawing.Size(32, 20);
            this.v74.TabIndex = 92;
            this.v74.Text = "0";
            this.v74.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v109
            // 
            this.v109.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v109.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v109.Location = new System.Drawing.Point(328, 288);
            this.v109.Name = "v109";
            this.v109.Size = new System.Drawing.Size(32, 20);
            this.v109.TabIndex = 130;
            this.v109.Text = "0";
            this.v109.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v108
            // 
            this.v108.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v108.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v108.Location = new System.Drawing.Point(296, 288);
            this.v108.Name = "v108";
            this.v108.Size = new System.Drawing.Size(32, 20);
            this.v108.TabIndex = 129;
            this.v108.Text = "0";
            this.v108.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v68
            // 
            this.v68.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v68.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v68.Location = new System.Drawing.Point(296, 192);
            this.v68.Name = "v68";
            this.v68.Size = new System.Drawing.Size(32, 20);
            this.v68.TabIndex = 85;
            this.v68.Text = "0";
            this.v68.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Transparent;
            this.groupBox1.Controls.Add(this.v139);
            this.groupBox1.Controls.Add(this.v138);
            this.groupBox1.Controls.Add(this.v137);
            this.groupBox1.Controls.Add(this.v136);
            this.groupBox1.Controls.Add(this.v135);
            this.groupBox1.Controls.Add(this.v134);
            this.groupBox1.Controls.Add(this.v133);
            this.groupBox1.Controls.Add(this.v132);
            this.groupBox1.Controls.Add(this.v131);
            this.groupBox1.Controls.Add(this.p132);
            this.groupBox1.Controls.Add(this.p131);
            this.groupBox1.Controls.Add(this.v129);
            this.groupBox1.Controls.Add(this.v128);
            this.groupBox1.Controls.Add(this.v127);
            this.groupBox1.Controls.Add(this.v126);
            this.groupBox1.Controls.Add(this.v125);
            this.groupBox1.Controls.Add(this.v124);
            this.groupBox1.Controls.Add(this.v123);
            this.groupBox1.Controls.Add(this.v122);
            this.groupBox1.Controls.Add(this.v121);
            this.groupBox1.Controls.Add(this.p122);
            this.groupBox1.Controls.Add(this.p121);
            this.groupBox1.Controls.Add(this.v119);
            this.groupBox1.Controls.Add(this.v118);
            this.groupBox1.Controls.Add(this.v117);
            this.groupBox1.Controls.Add(this.v116);
            this.groupBox1.Controls.Add(this.v115);
            this.groupBox1.Controls.Add(this.v114);
            this.groupBox1.Controls.Add(this.v113);
            this.groupBox1.Controls.Add(this.v112);
            this.groupBox1.Controls.Add(this.v111);
            this.groupBox1.Controls.Add(this.p112);
            this.groupBox1.Controls.Add(this.p111);
            this.groupBox1.Controls.Add(this.v109);
            this.groupBox1.Controls.Add(this.v108);
            this.groupBox1.Controls.Add(this.v107);
            this.groupBox1.Controls.Add(this.v106);
            this.groupBox1.Controls.Add(this.v105);
            this.groupBox1.Controls.Add(this.v104);
            this.groupBox1.Controls.Add(this.v103);
            this.groupBox1.Controls.Add(this.v102);
            this.groupBox1.Controls.Add(this.v101);
            this.groupBox1.Controls.Add(this.p102);
            this.groupBox1.Controls.Add(this.p101);
            this.groupBox1.Controls.Add(this.v99);
            this.groupBox1.Controls.Add(this.v98);
            this.groupBox1.Controls.Add(this.v97);
            this.groupBox1.Controls.Add(this.v96);
            this.groupBox1.Controls.Add(this.v95);
            this.groupBox1.Controls.Add(this.v94);
            this.groupBox1.Controls.Add(this.v93);
            this.groupBox1.Controls.Add(this.v92);
            this.groupBox1.Controls.Add(this.v91);
            this.groupBox1.Controls.Add(this.p92);
            this.groupBox1.Controls.Add(this.p91);
            this.groupBox1.Controls.Add(this.v89);
            this.groupBox1.Controls.Add(this.v88);
            this.groupBox1.Controls.Add(this.v87);
            this.groupBox1.Controls.Add(this.v86);
            this.groupBox1.Controls.Add(this.v85);
            this.groupBox1.Controls.Add(this.v84);
            this.groupBox1.Controls.Add(this.v83);
            this.groupBox1.Controls.Add(this.v82);
            this.groupBox1.Controls.Add(this.v81);
            this.groupBox1.Controls.Add(this.p82);
            this.groupBox1.Controls.Add(this.p81);
            this.groupBox1.Controls.Add(this.v79);
            this.groupBox1.Controls.Add(this.v78);
            this.groupBox1.Controls.Add(this.v77);
            this.groupBox1.Controls.Add(this.v76);
            this.groupBox1.Controls.Add(this.v75);
            this.groupBox1.Controls.Add(this.v74);
            this.groupBox1.Controls.Add(this.v73);
            this.groupBox1.Controls.Add(this.v72);
            this.groupBox1.Controls.Add(this.v71);
            this.groupBox1.Controls.Add(this.p72);
            this.groupBox1.Controls.Add(this.p71);
            this.groupBox1.Controls.Add(this.v69);
            this.groupBox1.Controls.Add(this.v68);
            this.groupBox1.Controls.Add(this.v67);
            this.groupBox1.Controls.Add(this.v66);
            this.groupBox1.Controls.Add(this.v65);
            this.groupBox1.Controls.Add(this.v64);
            this.groupBox1.Controls.Add(this.v63);
            this.groupBox1.Controls.Add(this.v62);
            this.groupBox1.Controls.Add(this.v61);
            this.groupBox1.Controls.Add(this.p62);
            this.groupBox1.Controls.Add(this.p61);
            this.groupBox1.Controls.Add(this.v59);
            this.groupBox1.Controls.Add(this.v58);
            this.groupBox1.Controls.Add(this.v57);
            this.groupBox1.Controls.Add(this.v56);
            this.groupBox1.Controls.Add(this.v55);
            this.groupBox1.Controls.Add(this.v54);
            this.groupBox1.Controls.Add(this.v53);
            this.groupBox1.Controls.Add(this.v52);
            this.groupBox1.Controls.Add(this.v51);
            this.groupBox1.Controls.Add(this.p52);
            this.groupBox1.Controls.Add(this.p51);
            this.groupBox1.Controls.Add(this.v49);
            this.groupBox1.Controls.Add(this.v48);
            this.groupBox1.Controls.Add(this.v47);
            this.groupBox1.Controls.Add(this.v46);
            this.groupBox1.Controls.Add(this.v45);
            this.groupBox1.Controls.Add(this.v44);
            this.groupBox1.Controls.Add(this.v43);
            this.groupBox1.Controls.Add(this.v42);
            this.groupBox1.Controls.Add(this.v41);
            this.groupBox1.Controls.Add(this.p42);
            this.groupBox1.Controls.Add(this.p41);
            this.groupBox1.Controls.Add(this.v39);
            this.groupBox1.Controls.Add(this.v38);
            this.groupBox1.Controls.Add(this.v37);
            this.groupBox1.Controls.Add(this.v36);
            this.groupBox1.Controls.Add(this.v35);
            this.groupBox1.Controls.Add(this.v34);
            this.groupBox1.Controls.Add(this.v33);
            this.groupBox1.Controls.Add(this.v32);
            this.groupBox1.Controls.Add(this.v31);
            this.groupBox1.Controls.Add(this.p32);
            this.groupBox1.Controls.Add(this.p31);
            this.groupBox1.Controls.Add(this.v29);
            this.groupBox1.Controls.Add(this.v28);
            this.groupBox1.Controls.Add(this.v27);
            this.groupBox1.Controls.Add(this.v26);
            this.groupBox1.Controls.Add(this.v25);
            this.groupBox1.Controls.Add(this.v24);
            this.groupBox1.Controls.Add(this.v23);
            this.groupBox1.Controls.Add(this.v22);
            this.groupBox1.Controls.Add(this.v21);
            this.groupBox1.Controls.Add(this.p22);
            this.groupBox1.Controls.Add(this.p21);
            this.groupBox1.Controls.Add(this.v19);
            this.groupBox1.Controls.Add(this.v18);
            this.groupBox1.Controls.Add(this.v17);
            this.groupBox1.Controls.Add(this.v16);
            this.groupBox1.Controls.Add(this.v15);
            this.groupBox1.Controls.Add(this.v14);
            this.groupBox1.Controls.Add(this.v13);
            this.groupBox1.Controls.Add(this.v12);
            this.groupBox1.Controls.Add(this.v11);
            this.groupBox1.Controls.Add(this.p12);
            this.groupBox1.Controls.Add(this.p11);
            this.groupBox1.Controls.Add(this.v09);
            this.groupBox1.Controls.Add(this.v08);
            this.groupBox1.Controls.Add(this.v07);
            this.groupBox1.Controls.Add(this.v06);
            this.groupBox1.Controls.Add(this.v05);
            this.groupBox1.Controls.Add(this.v04);
            this.groupBox1.Controls.Add(this.v03);
            this.groupBox1.Controls.Add(this.v02);
            this.groupBox1.Controls.Add(this.v01);
            this.groupBox1.Controls.Add(this.p02);
            this.groupBox1.Controls.Add(this.p01);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(16, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(376, 392);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Niveles";
            // 
            // v136
            // 
            this.v136.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v136.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v136.Location = new System.Drawing.Point(232, 360);
            this.v136.Name = "v136";
            this.v136.Size = new System.Drawing.Size(32, 20);
            this.v136.TabIndex = 160;
            this.v136.Text = "0";
            this.v136.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v129
            // 
            this.v129.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v129.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v129.Location = new System.Drawing.Point(328, 336);
            this.v129.Name = "v129";
            this.v129.Size = new System.Drawing.Size(32, 20);
            this.v129.TabIndex = 152;
            this.v129.Text = "0";
            this.v129.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v128
            // 
            this.v128.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v128.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v128.Location = new System.Drawing.Point(296, 336);
            this.v128.Name = "v128";
            this.v128.Size = new System.Drawing.Size(32, 20);
            this.v128.TabIndex = 151;
            this.v128.Text = "0";
            this.v128.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v127
            // 
            this.v127.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v127.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v127.Location = new System.Drawing.Point(264, 336);
            this.v127.Name = "v127";
            this.v127.Size = new System.Drawing.Size(32, 20);
            this.v127.TabIndex = 150;
            this.v127.Text = "0";
            this.v127.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v124
            // 
            this.v124.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v124.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v124.Location = new System.Drawing.Point(168, 336);
            this.v124.Name = "v124";
            this.v124.Size = new System.Drawing.Size(32, 20);
            this.v124.TabIndex = 147;
            this.v124.Text = "0";
            this.v124.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v123
            // 
            this.v123.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v123.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v123.Location = new System.Drawing.Point(136, 336);
            this.v123.Name = "v123";
            this.v123.Size = new System.Drawing.Size(32, 20);
            this.v123.TabIndex = 146;
            this.v123.Text = "0";
            this.v123.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v113
            // 
            this.v113.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v113.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v113.Location = new System.Drawing.Point(136, 312);
            this.v113.Name = "v113";
            this.v113.Size = new System.Drawing.Size(32, 20);
            this.v113.TabIndex = 135;
            this.v113.Text = "0";
            this.v113.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // p111
            // 
            this.p111.BackColor = System.Drawing.Color.LemonChiffon;
            this.p111.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p111.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.p111.Location = new System.Drawing.Point(8, 312);
            this.p111.Name = "p111";
            this.p111.Size = new System.Drawing.Size(32, 20);
            this.p111.TabIndex = 131;
            this.p111.Text = "0";
            this.p111.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v106
            // 
            this.v106.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v106.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v106.Location = new System.Drawing.Point(232, 288);
            this.v106.Name = "v106";
            this.v106.Size = new System.Drawing.Size(32, 20);
            this.v106.TabIndex = 127;
            this.v106.Text = "0";
            this.v106.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v102
            // 
            this.v102.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v102.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v102.Location = new System.Drawing.Point(104, 288);
            this.v102.Name = "v102";
            this.v102.Size = new System.Drawing.Size(32, 20);
            this.v102.TabIndex = 123;
            this.v102.Text = "0";
            this.v102.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v101
            // 
            this.v101.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v101.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v101.Location = new System.Drawing.Point(72, 288);
            this.v101.Name = "v101";
            this.v101.Size = new System.Drawing.Size(32, 20);
            this.v101.TabIndex = 122;
            this.v101.Text = "0";
            this.v101.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // p102
            // 
            this.p102.BackColor = System.Drawing.Color.LemonChiffon;
            this.p102.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p102.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.p102.Location = new System.Drawing.Point(40, 288);
            this.p102.Name = "p102";
            this.p102.Size = new System.Drawing.Size(32, 20);
            this.p102.TabIndex = 121;
            this.p102.Text = "0";
            this.p102.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // p101
            // 
            this.p101.BackColor = System.Drawing.Color.LemonChiffon;
            this.p101.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p101.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.p101.Location = new System.Drawing.Point(8, 288);
            this.p101.Name = "p101";
            this.p101.Size = new System.Drawing.Size(32, 20);
            this.p101.TabIndex = 120;
            this.p101.Text = "0";
            this.p101.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // p82
            // 
            this.p82.BackColor = System.Drawing.Color.LemonChiffon;
            this.p82.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p82.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.p82.Location = new System.Drawing.Point(40, 240);
            this.p82.Name = "p82";
            this.p82.Size = new System.Drawing.Size(32, 20);
            this.p82.TabIndex = 99;
            this.p82.Text = "0";
            this.p82.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // p72
            // 
            this.p72.BackColor = System.Drawing.Color.LemonChiffon;
            this.p72.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p72.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.p72.Location = new System.Drawing.Point(40, 216);
            this.p72.Name = "p72";
            this.p72.Size = new System.Drawing.Size(32, 20);
            this.p72.TabIndex = 88;
            this.p72.Text = "0";
            this.p72.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v67
            // 
            this.v67.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v67.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v67.Location = new System.Drawing.Point(264, 192);
            this.v67.Name = "v67";
            this.v67.Size = new System.Drawing.Size(32, 20);
            this.v67.TabIndex = 84;
            this.v67.Text = "0";
            this.v67.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v66
            // 
            this.v66.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v66.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v66.Location = new System.Drawing.Point(232, 192);
            this.v66.Name = "v66";
            this.v66.Size = new System.Drawing.Size(32, 20);
            this.v66.TabIndex = 83;
            this.v66.Text = "0";
            this.v66.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v65
            // 
            this.v65.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v65.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v65.Location = new System.Drawing.Point(200, 192);
            this.v65.Name = "v65";
            this.v65.Size = new System.Drawing.Size(32, 20);
            this.v65.TabIndex = 82;
            this.v65.Text = "0";
            this.v65.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v64
            // 
            this.v64.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v64.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v64.Location = new System.Drawing.Point(168, 192);
            this.v64.Name = "v64";
            this.v64.Size = new System.Drawing.Size(32, 20);
            this.v64.TabIndex = 81;
            this.v64.Text = "0";
            this.v64.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v63
            // 
            this.v63.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v63.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v63.Location = new System.Drawing.Point(136, 192);
            this.v63.Name = "v63";
            this.v63.Size = new System.Drawing.Size(32, 20);
            this.v63.TabIndex = 80;
            this.v63.Text = "0";
            this.v63.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v62
            // 
            this.v62.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v62.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v62.Location = new System.Drawing.Point(104, 192);
            this.v62.Name = "v62";
            this.v62.Size = new System.Drawing.Size(32, 20);
            this.v62.TabIndex = 79;
            this.v62.Text = "0";
            this.v62.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v61
            // 
            this.v61.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v61.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v61.Location = new System.Drawing.Point(72, 192);
            this.v61.Name = "v61";
            this.v61.Size = new System.Drawing.Size(32, 20);
            this.v61.TabIndex = 78;
            this.v61.Text = "0";
            this.v61.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // p62
            // 
            this.p62.BackColor = System.Drawing.Color.LemonChiffon;
            this.p62.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p62.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.p62.Location = new System.Drawing.Point(40, 192);
            this.p62.Name = "p62";
            this.p62.Size = new System.Drawing.Size(32, 20);
            this.p62.TabIndex = 77;
            this.p62.Text = "0";
            this.p62.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // p61
            // 
            this.p61.BackColor = System.Drawing.Color.LemonChiffon;
            this.p61.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p61.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.p61.Location = new System.Drawing.Point(8, 192);
            this.p61.Name = "p61";
            this.p61.Size = new System.Drawing.Size(32, 20);
            this.p61.TabIndex = 76;
            this.p61.Text = "0";
            this.p61.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // p52
            // 
            this.p52.BackColor = System.Drawing.Color.LemonChiffon;
            this.p52.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p52.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.p52.Location = new System.Drawing.Point(40, 168);
            this.p52.Name = "p52";
            this.p52.Size = new System.Drawing.Size(32, 20);
            this.p52.TabIndex = 66;
            this.p52.Text = "0";
            this.p52.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // p51
            // 
            this.p51.BackColor = System.Drawing.Color.LemonChiffon;
            this.p51.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p51.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.p51.Location = new System.Drawing.Point(8, 168);
            this.p51.Name = "p51";
            this.p51.Size = new System.Drawing.Size(32, 20);
            this.p51.TabIndex = 65;
            this.p51.Text = "0";
            this.p51.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // p42
            // 
            this.p42.BackColor = System.Drawing.Color.LemonChiffon;
            this.p42.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p42.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.p42.Location = new System.Drawing.Point(40, 144);
            this.p42.Name = "p42";
            this.p42.Size = new System.Drawing.Size(32, 20);
            this.p42.TabIndex = 55;
            this.p42.Text = "0";
            this.p42.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // p41
            // 
            this.p41.BackColor = System.Drawing.Color.LemonChiffon;
            this.p41.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.p41.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.p41.Location = new System.Drawing.Point(8, 144);
            this.p41.Name = "p41";
            this.p41.Size = new System.Drawing.Size(32, 20);
            this.p41.TabIndex = 54;
            this.p41.Text = "0";
            this.p41.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v39
            // 
            this.v39.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v39.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v39.Location = new System.Drawing.Point(328, 120);
            this.v39.Name = "v39";
            this.v39.Size = new System.Drawing.Size(32, 20);
            this.v39.TabIndex = 53;
            this.v39.Text = "0";
            this.v39.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v38
            // 
            this.v38.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v38.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v38.Location = new System.Drawing.Point(296, 120);
            this.v38.Name = "v38";
            this.v38.Size = new System.Drawing.Size(32, 20);
            this.v38.TabIndex = 52;
            this.v38.Text = "0";
            this.v38.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v37
            // 
            this.v37.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v37.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v37.Location = new System.Drawing.Point(264, 120);
            this.v37.Name = "v37";
            this.v37.Size = new System.Drawing.Size(32, 20);
            this.v37.TabIndex = 51;
            this.v37.Text = "0";
            this.v37.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v36
            // 
            this.v36.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v36.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v36.Location = new System.Drawing.Point(232, 120);
            this.v36.Name = "v36";
            this.v36.Size = new System.Drawing.Size(32, 20);
            this.v36.TabIndex = 50;
            this.v36.Text = "0";
            this.v36.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v35
            // 
            this.v35.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v35.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v35.Location = new System.Drawing.Point(200, 120);
            this.v35.Name = "v35";
            this.v35.Size = new System.Drawing.Size(32, 20);
            this.v35.TabIndex = 49;
            this.v35.Text = "0";
            this.v35.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v34
            // 
            this.v34.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v34.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v34.Location = new System.Drawing.Point(168, 120);
            this.v34.Name = "v34";
            this.v34.Size = new System.Drawing.Size(32, 20);
            this.v34.TabIndex = 48;
            this.v34.Text = "0";
            this.v34.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v33
            // 
            this.v33.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v33.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v33.Location = new System.Drawing.Point(136, 120);
            this.v33.Name = "v33";
            this.v33.Size = new System.Drawing.Size(32, 20);
            this.v33.TabIndex = 47;
            this.v33.Text = "0";
            this.v33.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v32
            // 
            this.v32.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v32.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v32.Location = new System.Drawing.Point(104, 120);
            this.v32.Name = "v32";
            this.v32.Size = new System.Drawing.Size(32, 20);
            this.v32.TabIndex = 46;
            this.v32.Text = "0";
            this.v32.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v31
            // 
            this.v31.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v31.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v31.Location = new System.Drawing.Point(72, 120);
            this.v31.Name = "v31";
            this.v31.Size = new System.Drawing.Size(32, 20);
            this.v31.TabIndex = 45;
            this.v31.Text = "0";
            this.v31.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v29
            // 
            this.v29.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v29.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v29.Location = new System.Drawing.Point(328, 96);
            this.v29.Name = "v29";
            this.v29.Size = new System.Drawing.Size(32, 20);
            this.v29.TabIndex = 42;
            this.v29.Text = "0";
            this.v29.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v28
            // 
            this.v28.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v28.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v28.Location = new System.Drawing.Point(296, 96);
            this.v28.Name = "v28";
            this.v28.Size = new System.Drawing.Size(32, 20);
            this.v28.TabIndex = 41;
            this.v28.Text = "0";
            this.v28.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v27
            // 
            this.v27.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v27.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v27.Location = new System.Drawing.Point(264, 96);
            this.v27.Name = "v27";
            this.v27.Size = new System.Drawing.Size(32, 20);
            this.v27.TabIndex = 40;
            this.v27.Text = "0";
            this.v27.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v26
            // 
            this.v26.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v26.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v26.Location = new System.Drawing.Point(232, 96);
            this.v26.Name = "v26";
            this.v26.Size = new System.Drawing.Size(32, 20);
            this.v26.TabIndex = 39;
            this.v26.Text = "0";
            this.v26.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v25
            // 
            this.v25.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v25.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v25.Location = new System.Drawing.Point(200, 96);
            this.v25.Name = "v25";
            this.v25.Size = new System.Drawing.Size(32, 20);
            this.v25.TabIndex = 38;
            this.v25.Text = "0";
            this.v25.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v24
            // 
            this.v24.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v24.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v24.Location = new System.Drawing.Point(168, 96);
            this.v24.Name = "v24";
            this.v24.Size = new System.Drawing.Size(32, 20);
            this.v24.TabIndex = 37;
            this.v24.Text = "0";
            this.v24.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v23
            // 
            this.v23.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v23.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v23.Location = new System.Drawing.Point(136, 96);
            this.v23.Name = "v23";
            this.v23.Size = new System.Drawing.Size(32, 20);
            this.v23.TabIndex = 36;
            this.v23.Text = "0";
            this.v23.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v22
            // 
            this.v22.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v22.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v22.Location = new System.Drawing.Point(104, 96);
            this.v22.Name = "v22";
            this.v22.Size = new System.Drawing.Size(32, 20);
            this.v22.TabIndex = 35;
            this.v22.Text = "0";
            this.v22.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v21
            // 
            this.v21.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v21.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v21.Location = new System.Drawing.Point(72, 96);
            this.v21.Name = "v21";
            this.v21.Size = new System.Drawing.Size(32, 20);
            this.v21.TabIndex = 34;
            this.v21.Text = "0";
            this.v21.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v19
            // 
            this.v19.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v19.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v19.Location = new System.Drawing.Point(328, 72);
            this.v19.Name = "v19";
            this.v19.Size = new System.Drawing.Size(32, 20);
            this.v19.TabIndex = 31;
            this.v19.Text = "0";
            this.v19.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v18
            // 
            this.v18.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v18.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v18.Location = new System.Drawing.Point(296, 72);
            this.v18.Name = "v18";
            this.v18.Size = new System.Drawing.Size(32, 20);
            this.v18.TabIndex = 30;
            this.v18.Text = "0";
            this.v18.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v17
            // 
            this.v17.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v17.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v17.Location = new System.Drawing.Point(264, 72);
            this.v17.Name = "v17";
            this.v17.Size = new System.Drawing.Size(32, 20);
            this.v17.TabIndex = 29;
            this.v17.Text = "0";
            this.v17.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v16
            // 
            this.v16.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v16.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v16.Location = new System.Drawing.Point(232, 72);
            this.v16.Name = "v16";
            this.v16.Size = new System.Drawing.Size(32, 20);
            this.v16.TabIndex = 28;
            this.v16.Text = "0";
            this.v16.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v15
            // 
            this.v15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v15.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v15.Location = new System.Drawing.Point(200, 72);
            this.v15.Name = "v15";
            this.v15.Size = new System.Drawing.Size(32, 20);
            this.v15.TabIndex = 27;
            this.v15.Text = "0";
            this.v15.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v14
            // 
            this.v14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v14.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v14.Location = new System.Drawing.Point(168, 72);
            this.v14.Name = "v14";
            this.v14.Size = new System.Drawing.Size(32, 20);
            this.v14.TabIndex = 26;
            this.v14.Text = "0";
            this.v14.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v13
            // 
            this.v13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v13.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v13.Location = new System.Drawing.Point(136, 72);
            this.v13.Name = "v13";
            this.v13.Size = new System.Drawing.Size(32, 20);
            this.v13.TabIndex = 25;
            this.v13.Text = "0";
            this.v13.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v12
            // 
            this.v12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v12.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v12.Location = new System.Drawing.Point(104, 72);
            this.v12.Name = "v12";
            this.v12.Size = new System.Drawing.Size(32, 20);
            this.v12.TabIndex = 24;
            this.v12.Text = "0";
            this.v12.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v11
            // 
            this.v11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v11.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v11.Location = new System.Drawing.Point(72, 72);
            this.v11.Name = "v11";
            this.v11.Size = new System.Drawing.Size(32, 20);
            this.v11.TabIndex = 23;
            this.v11.Text = "0";
            this.v11.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v09
            // 
            this.v09.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v09.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v09.Location = new System.Drawing.Point(328, 48);
            this.v09.Name = "v09";
            this.v09.Size = new System.Drawing.Size(32, 20);
            this.v09.TabIndex = 20;
            this.v09.Text = "0";
            this.v09.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v08
            // 
            this.v08.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v08.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v08.Location = new System.Drawing.Point(296, 48);
            this.v08.Name = "v08";
            this.v08.Size = new System.Drawing.Size(32, 20);
            this.v08.TabIndex = 19;
            this.v08.Text = "0";
            this.v08.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v07
            // 
            this.v07.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v07.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v07.Location = new System.Drawing.Point(264, 48);
            this.v07.Name = "v07";
            this.v07.Size = new System.Drawing.Size(32, 20);
            this.v07.TabIndex = 18;
            this.v07.Text = "0";
            this.v07.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v06
            // 
            this.v06.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v06.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v06.Location = new System.Drawing.Point(232, 48);
            this.v06.Name = "v06";
            this.v06.Size = new System.Drawing.Size(32, 20);
            this.v06.TabIndex = 17;
            this.v06.Text = "0";
            this.v06.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v05
            // 
            this.v05.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v05.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v05.Location = new System.Drawing.Point(200, 48);
            this.v05.Name = "v05";
            this.v05.Size = new System.Drawing.Size(32, 20);
            this.v05.TabIndex = 16;
            this.v05.Text = "0";
            this.v05.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v04
            // 
            this.v04.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v04.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v04.Location = new System.Drawing.Point(168, 48);
            this.v04.Name = "v04";
            this.v04.Size = new System.Drawing.Size(32, 20);
            this.v04.TabIndex = 15;
            this.v04.Text = "0";
            this.v04.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v03
            // 
            this.v03.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v03.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v03.Location = new System.Drawing.Point(136, 48);
            this.v03.Name = "v03";
            this.v03.Size = new System.Drawing.Size(32, 20);
            this.v03.TabIndex = 14;
            this.v03.Text = "0";
            this.v03.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v02
            // 
            this.v02.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v02.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v02.Location = new System.Drawing.Point(104, 48);
            this.v02.Name = "v02";
            this.v02.Size = new System.Drawing.Size(32, 20);
            this.v02.TabIndex = 13;
            this.v02.Text = "0";
            this.v02.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // v01
            // 
            this.v01.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.v01.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.v01.Location = new System.Drawing.Point(72, 48);
            this.v01.Name = "v01";
            this.v01.Size = new System.Drawing.Size(32, 20);
            this.v01.TabIndex = 12;
            this.v01.Text = "0";
            this.v01.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label8
            // 
            this.label8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label8.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.label8.Location = new System.Drawing.Point(200, 24);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(32, 24);
            this.label8.TabIndex = 9;
            this.label8.Text = "xx";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Transparent;
            this.groupBox2.Controls.Add(this.c71);
            this.groupBox2.Controls.Add(this.c72);
            this.groupBox2.Controls.Add(this.label12);
            this.groupBox2.Controls.Add(this.r7);
            this.groupBox2.Controls.Add(this.c61);
            this.groupBox2.Controls.Add(this.c62);
            this.groupBox2.Controls.Add(this.label20);
            this.groupBox2.Controls.Add(this.r6);
            this.groupBox2.Controls.Add(this.label25);
            this.groupBox2.Controls.Add(this.c51);
            this.groupBox2.Controls.Add(this.c52);
            this.groupBox2.Controls.Add(this.label23);
            this.groupBox2.Controls.Add(this.r5);
            this.groupBox2.Controls.Add(this.c41);
            this.groupBox2.Controls.Add(this.c42);
            this.groupBox2.Controls.Add(this.label21);
            this.groupBox2.Controls.Add(this.r4);
            this.groupBox2.Controls.Add(this.c31);
            this.groupBox2.Controls.Add(this.c32);
            this.groupBox2.Controls.Add(this.label19);
            this.groupBox2.Controls.Add(this.r3);
            this.groupBox2.Controls.Add(this.c21);
            this.groupBox2.Controls.Add(this.c22);
            this.groupBox2.Controls.Add(this.label11);
            this.groupBox2.Controls.Add(this.r2);
            this.groupBox2.Controls.Add(this.c11);
            this.groupBox2.Controls.Add(this.c12);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.label14);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.r1);
            this.groupBox2.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(400, 8);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(160, 248);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Condiciones";
            // 
            // c71
            // 
            this.c71.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c71.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.c71.Location = new System.Drawing.Point(40, 217);
            this.c71.Name = "c71";
            this.c71.Size = new System.Drawing.Size(40, 20);
            this.c71.TabIndex = 145;
            this.c71.Text = "0";
            this.c71.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // c62
            // 
            this.c62.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c62.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.c62.Location = new System.Drawing.Point(81, 193);
            this.c62.Name = "c62";
            this.c62.Size = new System.Drawing.Size(40, 20);
            this.c62.TabIndex = 144;
            this.c62.Text = "10";
            this.c62.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // c31
            // 
            this.c31.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c31.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.c31.Location = new System.Drawing.Point(40, 121);
            this.c31.Name = "c31";
            this.c31.Size = new System.Drawing.Size(40, 20);
            this.c31.TabIndex = 5;
            this.c31.Text = "0";
            this.c31.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // c11
            // 
            this.c11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c11.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.c11.Location = new System.Drawing.Point(40, 73);
            this.c11.Name = "c11";
            this.c11.Size = new System.Drawing.Size(40, 20);
            this.c11.TabIndex = 1;
            this.c11.Text = "0";
            this.c11.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // c12
            // 
            this.c12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.c12.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.c12.Location = new System.Drawing.Point(81, 73);
            this.c12.Name = "c12";
            this.c12.Size = new System.Drawing.Size(40, 20);
            this.c12.TabIndex = 2;
            this.c12.Text = "10";
            this.c12.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label30);
            this.groupBox3.Controls.Add(this.tCG);
            this.groupBox3.Controls.Add(this.bAnalizar);
            this.groupBox3.Location = new System.Drawing.Point(400, 272);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(176, 128);
            this.groupBox3.TabIndex = 149;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Análisis Resultados";
            // 
            // lval
            // 
            this.lval.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lval.Font = new System.Drawing.Font("Tahoma", 8F, System.Drawing.FontStyle.Bold);
            this.lval.Location = new System.Drawing.Point(568, 96);
            this.lval.Name = "lval";
            this.lval.Size = new System.Drawing.Size(100, 24);
            this.lval.TabIndex = 144;
            this.lval.Text = "Admitidas";
            this.lval.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ParejasFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(7, 16);
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(680, 454);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.lproc);
            this.Controls.Add(this.lval);
            this.Controls.Add(this.ltime);
            this.Controls.Add(this.bGrabar);
            this.Controls.Add(this.bCancelar);
            this.Controls.Add(this.bCalcular);
            this.Controls.Add(this.bLeer);
            this.Controls.Add(this.bSalvar);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ParejasFrm";
            this.Text = "Pares";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

		}
		#endregion
		
		void BLeerClick(object sender, System.EventArgs e) { LeeCondis(); }
		void BSalvarClick(object sender, System.EventArgs e) { SalvaCondis(); }
		void BCalcularClick(object sender, System.EventArgs e) { Calcular(); }
		void BCancelarClick(object sender, System.EventArgs e) { salida=true; }
		void BGrabarClick(object sender, System.EventArgs e) { GrabarCols(); }
		void BAnalizarClick(object sender, System.EventArgs e) { Analizar(); }
		void elmeuTimer(object sender, System.EventArgs e) { veureelmeu(); }


	}
}
