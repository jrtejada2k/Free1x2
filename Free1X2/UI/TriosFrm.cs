/*
 * Created by SharpDevelop.
 * User: Francisco
 * Date: 24/10/2004
 * Time: 6:47
 */
using System;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI 
{
	public class TriosFrm : Form
	{
		private TextBox v614;
		private TextBox v615;
		private TextBox v616;
		private TextBox v617;
		private TextBox v611;
		private TextBox p62;
		private TextBox v613;
		private TextBox v618;
		private TextBox v322;
		private TextBox v323;
		private TextBox v113;
		private TextBox v321;
		private TextBox v326;
		private TextBox v327;
		private TextBox v324;
		private TextBox v325;
		private TextBox va24;
		private TextBox va25;
		private TextBox v328;
		private TextBox v329;
		private TextBox va28;
		private TextBox v118;
		private TextBox v012;
		private TextBox v013;
		private TextBox v627;
		private TextBox v626;
		private TextBox v625;
		private TextBox v624;
		private TextBox v623;
		private TextBox v622;
		private TextBox p73;
		private TextBox p71;
		private TextBox v019;
		private TextBox v311;
		private TextBox v629;
		private TextBox v628;
		private TextBox v312;
		private TextBox v315;
		private TextBox v314;
		private TextBox v317;
		private TextBox v316;
		private TextBox v319;
		private TextBox v318;
		private TextBox v731;
		private TextBox v211;
		private TextBox v212;
		private TextBox v739;
		private TextBox v214;
		private TextBox v215;
		private TextBox v216;
		private TextBox v217;
		private TextBox v632;
		private TextBox v219;
		private TextBox v631;
		private TextBox p01;
		private TextBox c52;
		private TextBox p03;
		private TextBox p02;
		private TextBox va13;
		private TextBox v638;
		private TextBox v639;
		private TextBox va17;
		private TextBox va16;
		private TextBox va15;
		private TextBox va14;
		private TextBox va19;
		private TextBox va18;
		private TextBox v223;
		private TextBox v222;
		private TextBox v221;
		private TextBox v227;
		private TextBox v226;
		private TextBox v225;
		private TextBox v224;
		private TextBox v229;
		private TextBox v228;
		private TextBox p11;
		private TextBox p12;
		private TextBox p13;
		private TextBox v333;
		private TextBox v332;
		private TextBox v331;
		private TextBox v337;
		private TextBox v336;
		private TextBox v335;
		private TextBox va31;
		private TextBox va21;
		private TextBox va22;
		private TextBox v338;
		private TextBox va34;
		private TextBox va37;
		private TextBox va26;
		private TextBox v232;
		private TextBox v233;
		private TextBox vb11;
		private TextBox v231;
		private TextBox v236;
		private TextBox v237;
		private TextBox v234;
		private TextBox v235;
		private TextBox vb14;
		private TextBox vb12;
		private TextBox v238;
		private TextBox v239;
		private TextBox vb15;
		private TextBox p23;
		private TextBox p22;
		private TextBox p21;
		private TextBox vb13;
		private TextBox vb17;
		private TextBox vb18;
		private TextBox vb16;
		private TextBox c32;
		private TextBox vb19;
		private TextBox p63;
		private TextBox p61;
		private TextBox vb23;
		private TextBox vb22;
		private TextBox vb21;
		private TextBox vb27;
		private TextBox vb26;
		private TextBox vb25;
		private TextBox vb24;
		private TextBox vb29;
		private TextBox vb28;
		private Label r3;
		private Label r2;
		private Label r1;
		private TextBox p32;
		private TextBox p33;
		private TextBox p31;
		private Button bCancelar;
		private TextBox p72;
		private TextBox va12;
		private TextBox va11;
		private TextBox vb32;
		private TextBox vb33;
		private TextBox vb31;
		private TextBox vb36;
		private TextBox vb37;
		private TextBox vb34;
		private TextBox vb35;
		private TextBox vb38;
		private TextBox vb39;
		private TextBox c12;
		private TextBox c11;
		private Button bAnalizar;
		private TextBox va33;
		private TextBox va32;
		private TextBox va35;
		private TextBox va27;
		private TextBox va36;
		private TextBox va39;
		private TextBox va38;
		private Label ltime;
		private Label label21;
		private Label label20;
		private Label label23;
		private Label label22;
		private Label label25;
		private Label label24;
		private Label label27;
		private Label label26;
		private Label label29;
		private Label label28;
		private Label lproc;
		private Label label30;
		private Label label31;
		private Label label32;
		private Label label33;
		private Label label34;
		private Label label35;
		private Label label36;
		private Label label37;
		private Label label38;
		private Label label39;
		private TextBox p81;
		private TextBox p83;
		private TextBox p82;
		private TextBox v116;
		private TextBox v115;
		private TextBox p91;
		private Label label12;
		private TextBox p93;
		private Label label10;
		private Label label11;
		private Label label16;
		private Label label17;
		private Label label14;
		private Label label15;
		private Label label18;
		private Label label19;
		private Label r5;
		private Label r4;
		private TextBox v724;
		private TextBox v723;
		private TextBox v439;
		private Label label42;
		private Label label41;
		private Label label40;
		private TextBox v633;
		private TextBox v417;
		private TextBox p92;
		private TextBox v018;
		private Label label3;
		private Label label2;
		private Label label1;
		private Label label7;
		private Label label6;
		private Label label5;
		private Label label4;
		private Label label9;
		private Label label8;
		private TextBox v026;
		private TextBox v028;
		private TextBox c51;
		private Button bSalvar;
		private TextBox v031;
		private TextBox v032;
		private TextBox v034;
		private TextBox v037;
		private TextBox v038;
		private TextBox v039;
		private TextBox v832;
		private TextBox v833;
		private TextBox v834;
		private TextBox v835;
		private TextBox v836;
		private TextBox v213;
		private TextBox v218;
		private TextBox tCG;
		private TextBox pa1;
		private TextBox pa2;
		private TextBox pa3;
		private TextBox v313;
		private TextBox c31;
		private GroupBox groupBox3;
		private Label lval;
		private TextBox v011;
		private TextBox v016;
		private TextBox v017;
		private TextBox v014;
		private TextBox v015;
		private TextBox pb3;
		private TextBox pb2;
		private TextBox pb1;
		private TextBox v432;
		private TextBox v433;
		private TextBox v112;
		private TextBox v111;
		private TextBox v438;
		private TextBox v117;
		private TextBox v726;
		private TextBox v727;
		private TextBox v114;
		private TextBox v725;
		private TextBox v722;
		private TextBox v119;
		private TextBox v721;
		private TextBox v532;
		private TextBox v539;
		private TextBox v538;
		private TextBox v612;
		private TextBox v619;
		private TextBox v334;
		private TextBox v339;
		private TextBox v917;
		private TextBox v916;
		private TextBox v621;
		private TextBox v414;
		private TextBox v412;
		private TextBox v413;
		private TextBox v838;
		private TextBox v419;
		private TextBox c42;
		private TextBox v517;
		private TextBox v516;
		private TextBox v515;
		private TextBox v514;
		private TextBox v513;
		private TextBox v512;
		private TextBox v837;
		private TextBox v636;
		private TextBox v637;
		private TextBox v634;
		private TextBox v635;
		private TextBox v519;
		private TextBox v518;
		private Button bLeer;
		private TextBox v928;
		private TextBox v929;
		private TextBox v737;
		private TextBox v736;
		private TextBox v735;
		private TextBox v734;
		private TextBox v733;
		private TextBox v732;
		private TextBox v921;
		private TextBox v922;
		private TextBox v923;
		private TextBox v924;
		private TextBox v925;
		private TextBox v926;
		private TextBox v927;
		private TextBox v738;
		private GroupBox groupBox2;
		private GroupBox groupBox1;
		private Button bCalcular;
		private Label label13;
		private TextBox c21;
		private TextBox c22;
		private TextBox v919;
		private Button bGrabar;
		private TextBox v524;
		private TextBox v525;
		private TextBox v526;
		private TextBox v527;
		private TextBox v915;
		private TextBox v521;
		private TextBox v522;
		private TextBox v523;
		private TextBox v528;
		private TextBox v529;
		private TextBox v829;
		private TextBox v828;
		private TextBox v821;
		private TextBox v823;
		private TextBox v822;
		private TextBox v825;
		private TextBox v824;
		private TextBox v827;
		private TextBox v826;
		private TextBox v939;
		private TextBox v938;
		private TextBox v931;
		private TextBox v933;
		private TextBox v932;
		private TextBox v935;
		private TextBox v934;
		private TextBox v937;
		private TextBox v936;
		private TextBox v839;
		private TextBox v831;
		private TextBox v121;
		private TextBox v122;
		private TextBox v123;
		private TextBox v124;
		private TextBox v125;
		private TextBox v126;
		private TextBox v127;
		private TextBox v128;
		private TextBox v129;
		private TextBox v425;
		private TextBox v424;
		private TextBox v427;
		private TextBox v426;
		private TextBox v421;
		private TextBox v423;
		private TextBox v422;
		private TextBox v429;
		private TextBox v428;
		private TextBox v535;
		private TextBox v534;
		private TextBox v537;
		private TextBox v536;
		private TextBox v531;
		private TextBox v533;
		private TextBox v918;
		private TextBox v913;
		private TextBox v912;
		private TextBox v911;
		private TextBox v434;
		private TextBox v435;
		private TextBox v436;
		private TextBox v437;
		private TextBox v914;
		private TextBox v431;
		private TextBox v818;
		private TextBox v819;
		private TextBox v812;
		private TextBox v813;
		private TextBox va23;
		private TextBox v811;
		private TextBox v816;
		private TextBox v817;
		private TextBox v814;
		private TextBox v815;
		private TextBox va29;
		private TextBox v021;
		private TextBox v023;
		private TextBox v022;
		private TextBox v025;
		private TextBox v024;
		private TextBox v027;
		private TextBox p41;
		private TextBox v029;
		private TextBox p43;
		private TextBox p42;
		private TextBox v131;
		private TextBox v133;
		private TextBox v132;
		private TextBox v135;
		private TextBox v134;
		private TextBox v137;
		private TextBox v136;
		private TextBox v139;
		private TextBox v138;
		private TextBox v511;
		private TextBox v728;
		private TextBox v729;
		private TextBox v033;
		private TextBox v416;
		private TextBox v035;
		private TextBox v036;
		private TextBox v415;
		private TextBox p51;
		private TextBox p52;
		private TextBox p53;
		private TextBox v411;
		private TextBox c41;
		private TextBox v418;
		private TextBox v715;
		private TextBox v714;
		private TextBox v717;
		private TextBox v716;
		private TextBox v711;
		private TextBox v713;
		private TextBox v712;
		private TextBox v719;
		private TextBox v718;
		public TriosFrm()
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
 		private int[,] nivells = new int[12,30];
 		private int[,] rks = new int[5,3];
		
		private void LeeCondis() {
			string tmp, filein; string[] aux = null;
			OpenFileDialog lee = new OpenFileDialog();
			lee.InitialDirectory = ".\\" ;
			lee.Filter = "Condiciones(*.tri)|*.tri|Todos los archivos (*.*)|*.*";
			if(lee.ShowDialog() == DialogResult.OK) {
		   		tmp = lee.FileName;
		   		filein = Path.GetFileName(tmp);
				StreamReader sr = new StreamReader(filein);
				tmp = sr.ReadLine(); aux = tmp.Split(',');
				p01.Text = aux[0]; p02.Text = aux[1]; p03.Text = aux[2]; 
				v011.Text = aux[03]; v012.Text = aux[04]; v013.Text = aux[05]; v014.Text = aux[06]; 
				v015.Text = aux[07]; v016.Text = aux[08]; v017.Text = aux[09]; v018.Text = aux[10]; 
				v019.Text = aux[11]; v021.Text = aux[12]; v022.Text = aux[13]; v023.Text = aux[14]; 
				v024.Text = aux[15]; v025.Text = aux[16]; v026.Text = aux[17]; v027.Text = aux[18]; 
				v028.Text = aux[19]; v029.Text = aux[20]; v031.Text = aux[21]; v032.Text = aux[22]; 
				v033.Text = aux[23]; v034.Text = aux[24]; v035.Text = aux[25]; v036.Text = aux[26]; 
				v037.Text = aux[27]; v038.Text = aux[28]; v039.Text = aux[29];
				tmp = sr.ReadLine(); aux = tmp.Split(',');
				p11.Text = aux[0]; p12.Text = aux[1]; p13.Text = aux[2]; 
				v111.Text = aux[03]; v112.Text = aux[04]; v113.Text = aux[05]; v114.Text = aux[06]; 
				v115.Text = aux[07]; v116.Text = aux[08]; v117.Text = aux[09]; v118.Text = aux[10]; 
				v119.Text = aux[11]; v121.Text = aux[12]; v122.Text = aux[13]; v123.Text = aux[14]; 
				v124.Text = aux[15]; v125.Text = aux[16]; v126.Text = aux[17]; v127.Text = aux[18]; 
				v128.Text = aux[19]; v129.Text = aux[20]; v131.Text = aux[21]; v132.Text = aux[22]; 
				v133.Text = aux[23]; v134.Text = aux[24]; v135.Text = aux[25]; v136.Text = aux[26]; 
				v137.Text = aux[27]; v138.Text = aux[28]; v139.Text = aux[29];
				tmp = sr.ReadLine(); aux = tmp.Split(',');
				p21.Text = aux[0]; p22.Text = aux[1]; p33.Text = aux[2]; 
				v211.Text = aux[03]; v212.Text = aux[04]; v213.Text = aux[05]; v214.Text = aux[06]; 
				v215.Text = aux[07]; v216.Text = aux[08]; v217.Text = aux[09]; v218.Text = aux[10]; 
				v219.Text = aux[11]; v221.Text = aux[12]; v222.Text = aux[13]; v223.Text = aux[14]; 
				v224.Text = aux[15]; v225.Text = aux[16]; v226.Text = aux[17]; v227.Text = aux[18]; 
				v228.Text = aux[19]; v229.Text = aux[20]; v231.Text = aux[21]; v232.Text = aux[22]; 
				v233.Text = aux[23]; v234.Text = aux[24]; v235.Text = aux[25]; v236.Text = aux[26]; 
				v237.Text = aux[27]; v238.Text = aux[28]; v239.Text = aux[29];
				tmp = sr.ReadLine(); aux = tmp.Split(',');
				p31.Text = aux[0]; p32.Text = aux[1]; p33.Text = aux[2]; 
				v311.Text = aux[03]; v312.Text = aux[04]; v313.Text = aux[05]; v314.Text = aux[06]; 
				v315.Text = aux[07]; v316.Text = aux[08]; v317.Text = aux[09]; v318.Text = aux[10]; 
				v319.Text = aux[11]; v321.Text = aux[12]; v322.Text = aux[13]; v323.Text = aux[14]; 
				v324.Text = aux[15]; v325.Text = aux[16]; v326.Text = aux[17]; v327.Text = aux[18]; 
				v328.Text = aux[19]; v329.Text = aux[20]; v331.Text = aux[21]; v332.Text = aux[22]; 
				v333.Text = aux[23]; v334.Text = aux[24]; v335.Text = aux[25]; v336.Text = aux[26]; 
				v337.Text = aux[27]; v338.Text = aux[28]; v339.Text = aux[29];
				tmp = sr.ReadLine(); aux = tmp.Split(',');
				p41.Text = aux[0]; p42.Text = aux[1]; p43.Text = aux[2]; 
				v411.Text = aux[03]; v412.Text = aux[04]; v413.Text = aux[05]; v414.Text = aux[06]; 
				v415.Text = aux[07]; v416.Text = aux[08]; v417.Text = aux[09]; v418.Text = aux[10]; 
				v419.Text = aux[11]; v421.Text = aux[12]; v422.Text = aux[13]; v423.Text = aux[14]; 
				v424.Text = aux[15]; v425.Text = aux[16]; v426.Text = aux[17]; v427.Text = aux[18]; 
				v428.Text = aux[19]; v429.Text = aux[20]; v431.Text = aux[21]; v432.Text = aux[22]; 
				v433.Text = aux[23]; v434.Text = aux[24]; v435.Text = aux[25]; v436.Text = aux[26]; 
				v437.Text = aux[27]; v438.Text = aux[28]; v439.Text = aux[29];
				tmp = sr.ReadLine(); aux = tmp.Split(',');
				p51.Text = aux[0]; p52.Text = aux[1]; p53.Text = aux[2]; 
				v511.Text = aux[03]; v512.Text = aux[04]; v513.Text = aux[05]; v514.Text = aux[06]; 
				v515.Text = aux[07]; v516.Text = aux[08]; v517.Text = aux[09]; v518.Text = aux[10]; 
				v519.Text = aux[11]; v521.Text = aux[12]; v522.Text = aux[13]; v523.Text = aux[14]; 
				v524.Text = aux[15]; v525.Text = aux[16]; v526.Text = aux[17]; v527.Text = aux[18]; 
				v528.Text = aux[19]; v529.Text = aux[20]; v531.Text = aux[21]; v532.Text = aux[22]; 
				v533.Text = aux[23]; v534.Text = aux[24]; v535.Text = aux[25]; v536.Text = aux[26]; 
				v537.Text = aux[27]; v538.Text = aux[28]; v539.Text = aux[29];
				tmp = sr.ReadLine(); aux = tmp.Split(',');
				p61.Text = aux[0]; p62.Text = aux[1]; p63.Text = aux[2]; 
				v611.Text = aux[03]; v612.Text = aux[04]; v613.Text = aux[05]; v614.Text = aux[06]; 
				v615.Text = aux[07]; v616.Text = aux[08]; v617.Text = aux[09]; v618.Text = aux[10]; 
				v619.Text = aux[11]; v621.Text = aux[12]; v622.Text = aux[13]; v623.Text = aux[14]; 
				v624.Text = aux[15]; v625.Text = aux[16]; v626.Text = aux[17]; v627.Text = aux[18]; 
				v628.Text = aux[19]; v629.Text = aux[20]; v631.Text = aux[21]; v632.Text = aux[22]; 
				v633.Text = aux[23]; v634.Text = aux[24]; v635.Text = aux[25]; v636.Text = aux[26]; 
				v637.Text = aux[27]; v638.Text = aux[28]; v639.Text = aux[29];
				tmp = sr.ReadLine(); aux = tmp.Split(',');
				p71.Text = aux[0]; p72.Text = aux[1]; p73.Text = aux[2]; 
				v711.Text = aux[03]; v712.Text = aux[04]; v713.Text = aux[05]; v714.Text = aux[06]; 
				v715.Text = aux[07]; v716.Text = aux[08]; v717.Text = aux[09]; v718.Text = aux[10]; 
				v719.Text = aux[11]; v721.Text = aux[12]; v722.Text = aux[13]; v723.Text = aux[14]; 
				v724.Text = aux[15]; v725.Text = aux[16]; v726.Text = aux[17]; v727.Text = aux[18]; 
				v728.Text = aux[19]; v729.Text = aux[20]; v731.Text = aux[21]; v732.Text = aux[22]; 
				v733.Text = aux[23]; v734.Text = aux[24]; v735.Text = aux[25]; v736.Text = aux[26]; 
				v737.Text = aux[27]; v738.Text = aux[28]; v739.Text = aux[29];
				tmp = sr.ReadLine(); aux = tmp.Split(',');
				p81.Text = aux[0]; p82.Text = aux[1]; p83.Text = aux[2]; 
				v811.Text = aux[03]; v812.Text = aux[04]; v813.Text = aux[05]; v814.Text = aux[06]; 
				v815.Text = aux[07]; v816.Text = aux[08]; v817.Text = aux[09]; v818.Text = aux[10]; 
				v819.Text = aux[11]; v821.Text = aux[12]; v822.Text = aux[13]; v823.Text = aux[14]; 
				v824.Text = aux[15]; v825.Text = aux[16]; v826.Text = aux[17]; v827.Text = aux[18]; 
				v828.Text = aux[19]; v829.Text = aux[20]; v831.Text = aux[21]; v832.Text = aux[22]; 
				v833.Text = aux[23]; v834.Text = aux[24]; v835.Text = aux[25]; v836.Text = aux[26]; 
				v837.Text = aux[27]; v838.Text = aux[28]; v839.Text = aux[29];
				tmp = sr.ReadLine(); aux = tmp.Split(',');
				p91.Text = aux[0]; p92.Text = aux[1]; p93.Text = aux[2]; 
				v911.Text = aux[03]; v912.Text = aux[04]; v913.Text = aux[05]; v914.Text = aux[06]; 
				v915.Text = aux[07]; v916.Text = aux[08]; v917.Text = aux[09]; v918.Text = aux[10]; 
				v919.Text = aux[11]; v921.Text = aux[12]; v922.Text = aux[13]; v923.Text = aux[14]; 
				v924.Text = aux[15]; v925.Text = aux[16]; v926.Text = aux[17]; v927.Text = aux[18]; 
				v928.Text = aux[19]; v929.Text = aux[20]; v931.Text = aux[21]; v932.Text = aux[22]; 
				v933.Text = aux[23]; v934.Text = aux[24]; v935.Text = aux[25]; v936.Text = aux[26]; 
				v937.Text = aux[27]; v938.Text = aux[28]; v939.Text = aux[29];
				tmp = sr.ReadLine(); aux = tmp.Split(',');
				pa1.Text = aux[0]; pa2.Text = aux[1]; pa3.Text = aux[2]; 
				va11.Text = aux[03]; va12.Text = aux[04]; va13.Text = aux[05]; va14.Text = aux[06]; 
				va15.Text = aux[07]; va16.Text = aux[08]; va17.Text = aux[09]; va18.Text = aux[10]; 
				va19.Text = aux[11]; va21.Text = aux[12]; va22.Text = aux[13]; va23.Text = aux[14]; 
				va24.Text = aux[15]; va25.Text = aux[16]; va26.Text = aux[17]; va27.Text = aux[18]; 
				va28.Text = aux[19]; va29.Text = aux[20]; va31.Text = aux[21]; va32.Text = aux[22]; 
				va33.Text = aux[23]; va34.Text = aux[24]; va35.Text = aux[25]; va36.Text = aux[26]; 
				va37.Text = aux[27]; va38.Text = aux[28]; va39.Text = aux[29];
				tmp = sr.ReadLine(); aux = tmp.Split(',');
				pb1.Text = aux[0]; pb2.Text = aux[1]; pb3.Text = aux[2]; 
				vb11.Text = aux[03]; vb12.Text = aux[04]; vb13.Text = aux[05]; vb14.Text = aux[06]; 
				vb15.Text = aux[07]; vb16.Text = aux[08]; vb17.Text = aux[09]; vb18.Text = aux[10]; 
				vb19.Text = aux[11]; vb21.Text = aux[12]; vb22.Text = aux[13]; vb23.Text = aux[14]; 
				vb24.Text = aux[15]; vb25.Text = aux[16]; vb26.Text = aux[17]; vb27.Text = aux[18]; 
				vb28.Text = aux[19]; vb29.Text = aux[20]; vb31.Text = aux[21]; vb32.Text = aux[22]; 
				vb33.Text = aux[23]; vb34.Text = aux[24]; vb35.Text = aux[25]; vb36.Text = aux[26]; 
				vb37.Text = aux[27]; vb38.Text = aux[28]; vb39.Text = aux[29];
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
				sr.Close();
			}
		}
		private void SalvaCondis() {
			string tmp, fileout;
			SaveFileDialog grabacondis = new SaveFileDialog();
			grabacondis.InitialDirectory = ".\\" ;
			grabacondis.Filter = "Condiciones(*.tri)|*.tri|Todos los archivos (*.*)|*.*";
			if(grabacondis.ShowDialog() == DialogResult.OK) {
		   		tmp = grabacondis.FileName;
		   		fileout = Path.GetFileName(tmp);
				StreamWriter sw = new StreamWriter(fileout);
				tmp=p01.Text+','+p02.Text+','+p03.Text+',';
				tmp+=v011.Text+','+v012.Text+','+v013.Text+','+v014.Text+','+v015.Text+',';
				tmp+=v016.Text+','+v017.Text+','+v018.Text+','+v019.Text+',';
				tmp+=v021.Text+','+v022.Text+','+v023.Text+','+v024.Text+','+v025.Text+',';
				tmp+=v026.Text+','+v027.Text+','+v028.Text+','+v029.Text+',';
				tmp+=v031.Text+','+v032.Text+','+v033.Text+','+v034.Text+','+v035.Text+',';
				tmp+=v036.Text+','+v037.Text+','+v038.Text+','+v039.Text;
				sw.WriteLine(tmp);
				tmp=p11.Text+','+p12.Text+','+p13.Text+',';
				tmp+=v111.Text+','+v112.Text+','+v113.Text+','+v114.Text+','+v115.Text+',';
				tmp+=v116.Text+','+v117.Text+','+v118.Text+','+v119.Text+',';
				tmp+=v121.Text+','+v122.Text+','+v123.Text+','+v124.Text+','+v125.Text+',';
				tmp+=v126.Text+','+v127.Text+','+v128.Text+','+v129.Text+',';
				tmp+=v131.Text+','+v132.Text+','+v133.Text+','+v134.Text+','+v135.Text+',';
				tmp+=v136.Text+','+v137.Text+','+v138.Text+','+v139.Text;
				sw.WriteLine(tmp);
				tmp=p21.Text+','+p22.Text+','+p23.Text+',';
				tmp+=v211.Text+','+v212.Text+','+v213.Text+','+v214.Text+','+v215.Text+',';
				tmp+=v216.Text+','+v217.Text+','+v218.Text+','+v219.Text+',';
				tmp+=v221.Text+','+v222.Text+','+v223.Text+','+v224.Text+','+v225.Text+',';
				tmp+=v226.Text+','+v227.Text+','+v228.Text+','+v229.Text+',';
				tmp+=v231.Text+','+v232.Text+','+v233.Text+','+v234.Text+','+v235.Text+',';
				tmp+=v236.Text+','+v237.Text+','+v238.Text+','+v239.Text;
				sw.WriteLine(tmp);
				tmp=p31.Text+','+p32.Text+','+p33.Text+',';
				tmp+=v311.Text+','+v312.Text+','+v313.Text+','+v314.Text+','+v315.Text+',';
				tmp+=v316.Text+','+v317.Text+','+v318.Text+','+v319.Text+',';
				tmp+=v321.Text+','+v322.Text+','+v323.Text+','+v324.Text+','+v325.Text+',';
				tmp+=v326.Text+','+v327.Text+','+v328.Text+','+v329.Text+',';
				tmp+=v331.Text+','+v332.Text+','+v333.Text+','+v334.Text+','+v335.Text+',';
				tmp+=v336.Text+','+v337.Text+','+v338.Text+','+v339.Text;
				sw.WriteLine(tmp);
				tmp=p41.Text+','+p42.Text+','+p43.Text+',';
				tmp+=v411.Text+','+v412.Text+','+v413.Text+','+v414.Text+','+v415.Text+',';
				tmp+=v416.Text+','+v417.Text+','+v418.Text+','+v419.Text+',';
				tmp+=v421.Text+','+v422.Text+','+v423.Text+','+v424.Text+','+v425.Text+',';
				tmp+=v426.Text+','+v427.Text+','+v428.Text+','+v429.Text+',';
				tmp+=v431.Text+','+v432.Text+','+v433.Text+','+v434.Text+','+v435.Text+',';
				tmp+=v436.Text+','+v437.Text+','+v438.Text+','+v439.Text;
				sw.WriteLine(tmp);
				tmp=p51.Text+','+p52.Text+','+p53.Text+',';
				tmp+=v511.Text+','+v512.Text+','+v513.Text+','+v514.Text+','+v515.Text+',';
				tmp+=v516.Text+','+v517.Text+','+v518.Text+','+v519.Text+',';
				tmp+=v521.Text+','+v522.Text+','+v523.Text+','+v524.Text+','+v525.Text+',';
				tmp+=v526.Text+','+v527.Text+','+v528.Text+','+v529.Text+',';
				tmp+=v531.Text+','+v532.Text+','+v533.Text+','+v534.Text+','+v535.Text+',';
				tmp+=v536.Text+','+v537.Text+','+v538.Text+','+v539.Text;
				sw.WriteLine(tmp);
				tmp=p61.Text+','+p62.Text+','+p63.Text+',';
				tmp+=v611.Text+','+v612.Text+','+v613.Text+','+v614.Text+','+v615.Text+',';
				tmp+=v616.Text+','+v617.Text+','+v618.Text+','+v619.Text+',';
				tmp+=v621.Text+','+v622.Text+','+v623.Text+','+v624.Text+','+v625.Text+',';
				tmp+=v626.Text+','+v627.Text+','+v628.Text+','+v629.Text+',';
				tmp+=v631.Text+','+v632.Text+','+v633.Text+','+v634.Text+','+v635.Text+',';
				tmp+=v636.Text+','+v637.Text+','+v638.Text+','+v639.Text;
				sw.WriteLine(tmp);
				tmp=p71.Text+','+p72.Text+','+p73.Text+',';
				tmp+=v711.Text+','+v712.Text+','+v713.Text+','+v714.Text+','+v715.Text+',';
				tmp+=v716.Text+','+v717.Text+','+v718.Text+','+v719.Text+',';
				tmp+=v721.Text+','+v722.Text+','+v723.Text+','+v724.Text+','+v725.Text+',';
				tmp+=v726.Text+','+v727.Text+','+v728.Text+','+v729.Text+',';
				tmp+=v731.Text+','+v732.Text+','+v733.Text+','+v734.Text+','+v735.Text+',';
				tmp+=v736.Text+','+v737.Text+','+v738.Text+','+v739.Text;
				sw.WriteLine(tmp);
				tmp=p81.Text+','+p82.Text+','+p83.Text+',';
				tmp+=v811.Text+','+v812.Text+','+v813.Text+','+v814.Text+','+v815.Text+',';
				tmp+=v816.Text+','+v817.Text+','+v818.Text+','+v819.Text+',';
				tmp+=v821.Text+','+v822.Text+','+v823.Text+','+v824.Text+','+v825.Text+',';
				tmp+=v826.Text+','+v827.Text+','+v828.Text+','+v829.Text+',';
				tmp+=v831.Text+','+v832.Text+','+v833.Text+','+v834.Text+','+v835.Text+',';
				tmp+=v836.Text+','+v837.Text+','+v838.Text+','+v839.Text;
				sw.WriteLine(tmp);
				tmp=p91.Text+','+p92.Text+','+p93.Text+',';
				tmp+=v911.Text+','+v912.Text+','+v913.Text+','+v914.Text+','+v915.Text+',';
				tmp+=v916.Text+','+v917.Text+','+v918.Text+','+v919.Text+',';
				tmp+=v921.Text+','+v922.Text+','+v923.Text+','+v924.Text+','+v925.Text+',';
				tmp+=v926.Text+','+v927.Text+','+v928.Text+','+v929.Text+',';
				tmp+=v931.Text+','+v932.Text+','+v933.Text+','+v934.Text+','+v935.Text+',';
				tmp+=v936.Text+','+v937.Text+','+v938.Text+','+v939.Text;
				sw.WriteLine(tmp);
				tmp=pa1.Text+','+pa2.Text+','+pa3.Text+',';
				tmp+=va11.Text+','+va12.Text+','+va13.Text+','+va14.Text+','+va15.Text+',';
				tmp+=va16.Text+','+va17.Text+','+va18.Text+','+va19.Text+',';
				tmp+=va21.Text+','+va22.Text+','+va23.Text+','+va24.Text+','+va25.Text+',';
				tmp+=va26.Text+','+va27.Text+','+va28.Text+','+va29.Text+',';
				tmp+=va31.Text+','+va32.Text+','+va33.Text+','+va34.Text+','+va35.Text+',';
				tmp+=va36.Text+','+va37.Text+','+va38.Text+','+va39.Text;
				sw.WriteLine(tmp);
				tmp=pb1.Text+','+pb2.Text+','+pb3.Text+',';
				tmp+=vb11.Text+','+vb12.Text+','+vb13.Text+','+vb14.Text+','+vb15.Text+',';
				tmp+=vb16.Text+','+vb17.Text+','+vb18.Text+','+vb19.Text+',';
				tmp+=vb21.Text+','+vb22.Text+','+vb23.Text+','+vb24.Text+','+vb25.Text+',';
				tmp+=vb26.Text+','+vb27.Text+','+vb28.Text+','+vb29.Text+',';
				tmp+=vb31.Text+','+vb32.Text+','+vb33.Text+','+vb34.Text+','+vb35.Text+',';
				tmp+=vb36.Text+','+vb37.Text+','+vb38.Text+','+vb39.Text;
				sw.WriteLine(tmp);
				sw.WriteLine(c11.Text+','+c12.Text);
				sw.WriteLine(c21.Text+','+c22.Text);
				sw.WriteLine(c31.Text+','+c32.Text);
				sw.WriteLine(c41.Text+','+c42.Text);
				sw.WriteLine(c51.Text+','+c52.Text);
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
 			validas.SetAll(false);
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
 			nivells[0,0] = Convert.ToInt32(p01.Text);
 			nivells[0,1] = Convert.ToInt32(p02.Text);
 			nivells[0,2] = Convert.ToInt32(p03.Text); 
 			nivells[0,3] = Convert.ToInt32(v011.Text);
 			nivells[0,4] = Convert.ToInt32(v012.Text);
 			nivells[0,5] = Convert.ToInt32(v013.Text);
 			nivells[0,6] = Convert.ToInt32(v014.Text);
 			nivells[0,7] = Convert.ToInt32(v015.Text);
 			nivells[0,8] = Convert.ToInt32(v016.Text);
 			nivells[0,9] = Convert.ToInt32(v017.Text);
 			nivells[0,10] = Convert.ToInt32(v018.Text);
 			nivells[0,11] = Convert.ToInt32(v019.Text);
 			nivells[0,12] = Convert.ToInt32(v021.Text);
 			nivells[0,13] = Convert.ToInt32(v022.Text);
 			nivells[0,14] = Convert.ToInt32(v023.Text);
 			nivells[0,15] = Convert.ToInt32(v024.Text);
 			nivells[0,16] = Convert.ToInt32(v025.Text);
 			nivells[0,17] = Convert.ToInt32(v026.Text);
 			nivells[0,18] = Convert.ToInt32(v027.Text);
 			nivells[0,19] = Convert.ToInt32(v028.Text);
 			nivells[0,20] = Convert.ToInt32(v029.Text);
 			nivells[0,21] = Convert.ToInt32(v031.Text);
 			nivells[0,22] = Convert.ToInt32(v032.Text);
 			nivells[0,23] = Convert.ToInt32(v033.Text);
 			nivells[0,24] = Convert.ToInt32(v034.Text);
 			nivells[0,25] = Convert.ToInt32(v035.Text);
 			nivells[0,26] = Convert.ToInt32(v036.Text);
 			nivells[0,27] = Convert.ToInt32(v037.Text);
 			nivells[0,28] = Convert.ToInt32(v038.Text);
 			nivells[0,29] = Convert.ToInt32(v039.Text);
 			nivells[1,0] = Convert.ToInt32(p11.Text);
 			nivells[1,1] = Convert.ToInt32(p12.Text);
 			nivells[1,2] = Convert.ToInt32(p13.Text); 
 			nivells[1,3] = Convert.ToInt32(v111.Text);
 			nivells[1,4] = Convert.ToInt32(v112.Text);
 			nivells[1,5] = Convert.ToInt32(v113.Text);
 			nivells[1,6] = Convert.ToInt32(v114.Text);
 			nivells[1,7] = Convert.ToInt32(v115.Text);
 			nivells[1,8] = Convert.ToInt32(v116.Text);
 			nivells[1,9] = Convert.ToInt32(v117.Text);
 			nivells[1,10] = Convert.ToInt32(v118.Text);
 			nivells[1,11] = Convert.ToInt32(v119.Text);
 			nivells[1,12] = Convert.ToInt32(v121.Text);
 			nivells[1,13] = Convert.ToInt32(v122.Text);
 			nivells[1,14] = Convert.ToInt32(v123.Text);
 			nivells[1,15] = Convert.ToInt32(v124.Text);
 			nivells[1,16] = Convert.ToInt32(v125.Text);
 			nivells[1,17] = Convert.ToInt32(v126.Text);
 			nivells[1,18] = Convert.ToInt32(v127.Text);
 			nivells[1,19] = Convert.ToInt32(v128.Text);
 			nivells[1,20] = Convert.ToInt32(v129.Text);
 			nivells[1,21] = Convert.ToInt32(v131.Text);
 			nivells[1,22] = Convert.ToInt32(v132.Text);
 			nivells[1,23] = Convert.ToInt32(v133.Text);
 			nivells[1,24] = Convert.ToInt32(v134.Text);
 			nivells[1,25] = Convert.ToInt32(v135.Text);
 			nivells[1,26] = Convert.ToInt32(v136.Text);
 			nivells[1,27] = Convert.ToInt32(v137.Text);
 			nivells[1,28] = Convert.ToInt32(v138.Text);
 			nivells[1,29] = Convert.ToInt32(v139.Text);
 			nivells[2,0] = Convert.ToInt32(p21.Text);
 			nivells[2,1] = Convert.ToInt32(p22.Text);
 			nivells[2,2] = Convert.ToInt32(p23.Text); 
 			nivells[2,3] = Convert.ToInt32(v211.Text);
 			nivells[2,4] = Convert.ToInt32(v212.Text);
 			nivells[2,5] = Convert.ToInt32(v213.Text);
 			nivells[2,6] = Convert.ToInt32(v214.Text);
 			nivells[2,7] = Convert.ToInt32(v215.Text);
 			nivells[2,8] = Convert.ToInt32(v216.Text);
 			nivells[2,9] = Convert.ToInt32(v217.Text);
 			nivells[2,10] = Convert.ToInt32(v218.Text);
 			nivells[2,11] = Convert.ToInt32(v219.Text);
 			nivells[2,12] = Convert.ToInt32(v221.Text);
 			nivells[2,13] = Convert.ToInt32(v222.Text);
 			nivells[2,14] = Convert.ToInt32(v223.Text);
 			nivells[2,15] = Convert.ToInt32(v224.Text);
 			nivells[2,16] = Convert.ToInt32(v225.Text);
 			nivells[2,17] = Convert.ToInt32(v226.Text);
 			nivells[2,18] = Convert.ToInt32(v227.Text);
 			nivells[2,19] = Convert.ToInt32(v228.Text);
 			nivells[2,20] = Convert.ToInt32(v229.Text);
 			nivells[2,21] = Convert.ToInt32(v231.Text);
 			nivells[2,22] = Convert.ToInt32(v232.Text);
 			nivells[2,23] = Convert.ToInt32(v233.Text);
 			nivells[2,24] = Convert.ToInt32(v234.Text);
 			nivells[2,25] = Convert.ToInt32(v235.Text);
 			nivells[2,26] = Convert.ToInt32(v236.Text);
 			nivells[2,27] = Convert.ToInt32(v237.Text);
 			nivells[2,28] = Convert.ToInt32(v238.Text);
 			nivells[2,29] = Convert.ToInt32(v239.Text);
 			nivells[3,0] = Convert.ToInt32(p31.Text);
 			nivells[3,1] = Convert.ToInt32(p32.Text);
 			nivells[3,2] = Convert.ToInt32(p33.Text); 
 			nivells[3,3] = Convert.ToInt32(v311.Text);
 			nivells[3,4] = Convert.ToInt32(v312.Text);
 			nivells[3,5] = Convert.ToInt32(v313.Text);
 			nivells[3,6] = Convert.ToInt32(v314.Text);
 			nivells[3,7] = Convert.ToInt32(v315.Text);
 			nivells[3,8] = Convert.ToInt32(v316.Text);
 			nivells[3,9] = Convert.ToInt32(v317.Text);
 			nivells[3,10] = Convert.ToInt32(v318.Text);
 			nivells[3,11] = Convert.ToInt32(v319.Text);
 			nivells[3,12] = Convert.ToInt32(v321.Text);
 			nivells[3,13] = Convert.ToInt32(v322.Text);
 			nivells[3,14] = Convert.ToInt32(v323.Text);
 			nivells[3,15] = Convert.ToInt32(v324.Text);
 			nivells[3,16] = Convert.ToInt32(v325.Text);
 			nivells[3,17] = Convert.ToInt32(v326.Text);
 			nivells[3,18] = Convert.ToInt32(v327.Text);
 			nivells[3,19] = Convert.ToInt32(v328.Text);
 			nivells[3,20] = Convert.ToInt32(v329.Text);
 			nivells[3,21] = Convert.ToInt32(v331.Text);
 			nivells[3,22] = Convert.ToInt32(v332.Text);
 			nivells[3,23] = Convert.ToInt32(v333.Text);
 			nivells[3,24] = Convert.ToInt32(v334.Text);
 			nivells[3,25] = Convert.ToInt32(v335.Text);
 			nivells[3,26] = Convert.ToInt32(v336.Text);
 			nivells[3,27] = Convert.ToInt32(v337.Text);
 			nivells[3,28] = Convert.ToInt32(v338.Text);
 			nivells[3,29] = Convert.ToInt32(v339.Text);
 			nivells[4,0] = Convert.ToInt32(p41.Text);
 			nivells[4,1] = Convert.ToInt32(p42.Text);
 			nivells[4,2] = Convert.ToInt32(p43.Text); 
 			nivells[4,3] = Convert.ToInt32(v411.Text);
 			nivells[4,4] = Convert.ToInt32(v412.Text);
 			nivells[4,5] = Convert.ToInt32(v413.Text);
 			nivells[4,6] = Convert.ToInt32(v414.Text);
 			nivells[4,7] = Convert.ToInt32(v415.Text);
 			nivells[4,8] = Convert.ToInt32(v416.Text);
 			nivells[4,9] = Convert.ToInt32(v417.Text);
 			nivells[4,10] = Convert.ToInt32(v418.Text);
 			nivells[4,11] = Convert.ToInt32(v419.Text);
 			nivells[4,12] = Convert.ToInt32(v421.Text);
 			nivells[4,13] = Convert.ToInt32(v422.Text);
 			nivells[4,14] = Convert.ToInt32(v423.Text);
 			nivells[4,15] = Convert.ToInt32(v424.Text);
 			nivells[4,16] = Convert.ToInt32(v425.Text);
 			nivells[4,17] = Convert.ToInt32(v426.Text);
 			nivells[4,18] = Convert.ToInt32(v427.Text);
 			nivells[4,19] = Convert.ToInt32(v428.Text);
 			nivells[4,20] = Convert.ToInt32(v429.Text);
 			nivells[4,21] = Convert.ToInt32(v431.Text);
 			nivells[4,22] = Convert.ToInt32(v432.Text);
 			nivells[4,23] = Convert.ToInt32(v433.Text);
 			nivells[4,24] = Convert.ToInt32(v434.Text);
 			nivells[4,25] = Convert.ToInt32(v435.Text);
 			nivells[4,26] = Convert.ToInt32(v436.Text);
 			nivells[4,27] = Convert.ToInt32(v437.Text);
 			nivells[4,28] = Convert.ToInt32(v438.Text);
 			nivells[4,29] = Convert.ToInt32(v439.Text);
 			nivells[5,0] = Convert.ToInt32(p51.Text);
 			nivells[5,1] = Convert.ToInt32(p52.Text);
 			nivells[5,2] = Convert.ToInt32(p53.Text); 
 			nivells[5,3] = Convert.ToInt32(v511.Text);
 			nivells[5,4] = Convert.ToInt32(v512.Text);
 			nivells[5,5] = Convert.ToInt32(v513.Text);
 			nivells[5,6] = Convert.ToInt32(v514.Text);
 			nivells[5,7] = Convert.ToInt32(v515.Text);
 			nivells[5,8] = Convert.ToInt32(v516.Text);
 			nivells[5,9] = Convert.ToInt32(v517.Text);
 			nivells[5,10] = Convert.ToInt32(v518.Text);
 			nivells[5,11] = Convert.ToInt32(v519.Text);
 			nivells[5,12] = Convert.ToInt32(v521.Text);
 			nivells[5,13] = Convert.ToInt32(v522.Text);
 			nivells[5,14] = Convert.ToInt32(v523.Text);
 			nivells[5,15] = Convert.ToInt32(v524.Text);
 			nivells[5,16] = Convert.ToInt32(v525.Text);
 			nivells[5,17] = Convert.ToInt32(v526.Text);
 			nivells[5,18] = Convert.ToInt32(v527.Text);
 			nivells[5,19] = Convert.ToInt32(v528.Text);
 			nivells[5,20] = Convert.ToInt32(v529.Text);
 			nivells[5,21] = Convert.ToInt32(v531.Text);
 			nivells[5,22] = Convert.ToInt32(v532.Text);
 			nivells[5,23] = Convert.ToInt32(v533.Text);
 			nivells[5,24] = Convert.ToInt32(v534.Text);
 			nivells[5,25] = Convert.ToInt32(v535.Text);
 			nivells[5,26] = Convert.ToInt32(v536.Text);
 			nivells[5,27] = Convert.ToInt32(v537.Text);
 			nivells[5,28] = Convert.ToInt32(v538.Text);
 			nivells[5,29] = Convert.ToInt32(v539.Text);
 			nivells[6,0] = Convert.ToInt32(p61.Text);
 			nivells[6,1] = Convert.ToInt32(p62.Text);
 			nivells[6,2] = Convert.ToInt32(p63.Text); 
 			nivells[6,3] = Convert.ToInt32(v611.Text);
 			nivells[6,4] = Convert.ToInt32(v612.Text);
 			nivells[6,5] = Convert.ToInt32(v613.Text);
 			nivells[6,6] = Convert.ToInt32(v614.Text);
 			nivells[6,7] = Convert.ToInt32(v615.Text);
 			nivells[6,8] = Convert.ToInt32(v616.Text);
 			nivells[6,9] = Convert.ToInt32(v617.Text);
 			nivells[6,10] = Convert.ToInt32(v618.Text);
 			nivells[6,11] = Convert.ToInt32(v619.Text);
 			nivells[6,12] = Convert.ToInt32(v621.Text);
 			nivells[6,13] = Convert.ToInt32(v622.Text);
 			nivells[6,14] = Convert.ToInt32(v623.Text);
 			nivells[6,15] = Convert.ToInt32(v624.Text);
 			nivells[6,16] = Convert.ToInt32(v625.Text);
 			nivells[6,17] = Convert.ToInt32(v626.Text);
 			nivells[6,18] = Convert.ToInt32(v627.Text);
 			nivells[6,19] = Convert.ToInt32(v628.Text);
 			nivells[6,20] = Convert.ToInt32(v629.Text);
 			nivells[6,21] = Convert.ToInt32(v631.Text);
 			nivells[6,22] = Convert.ToInt32(v632.Text);
 			nivells[6,23] = Convert.ToInt32(v633.Text);
 			nivells[6,24] = Convert.ToInt32(v634.Text);
 			nivells[6,25] = Convert.ToInt32(v635.Text);
 			nivells[6,26] = Convert.ToInt32(v636.Text);
 			nivells[6,27] = Convert.ToInt32(v637.Text);
 			nivells[6,28] = Convert.ToInt32(v638.Text);
 			nivells[6,29] = Convert.ToInt32(v639.Text);
 			nivells[7,0] = Convert.ToInt32(p71.Text);
 			nivells[7,1] = Convert.ToInt32(p72.Text);
 			nivells[7,2] = Convert.ToInt32(p73.Text); 
 			nivells[7,3] = Convert.ToInt32(v711.Text);
 			nivells[7,4] = Convert.ToInt32(v712.Text);
 			nivells[7,5] = Convert.ToInt32(v713.Text);
 			nivells[7,6] = Convert.ToInt32(v714.Text);
 			nivells[7,7] = Convert.ToInt32(v715.Text);
 			nivells[7,8] = Convert.ToInt32(v716.Text);
 			nivells[7,9] = Convert.ToInt32(v717.Text);
 			nivells[7,10] = Convert.ToInt32(v718.Text);
 			nivells[7,11] = Convert.ToInt32(v719.Text);
 			nivells[7,12] = Convert.ToInt32(v721.Text);
 			nivells[7,13] = Convert.ToInt32(v722.Text);
 			nivells[7,14] = Convert.ToInt32(v723.Text);
 			nivells[7,15] = Convert.ToInt32(v724.Text);
 			nivells[7,16] = Convert.ToInt32(v725.Text);
 			nivells[7,17] = Convert.ToInt32(v726.Text);
 			nivells[7,18] = Convert.ToInt32(v727.Text);
 			nivells[7,19] = Convert.ToInt32(v728.Text);
 			nivells[7,20] = Convert.ToInt32(v729.Text);
 			nivells[7,21] = Convert.ToInt32(v731.Text);
 			nivells[7,22] = Convert.ToInt32(v732.Text);
 			nivells[7,23] = Convert.ToInt32(v733.Text);
 			nivells[7,24] = Convert.ToInt32(v734.Text);
 			nivells[7,25] = Convert.ToInt32(v735.Text);
 			nivells[7,26] = Convert.ToInt32(v736.Text);
 			nivells[7,27] = Convert.ToInt32(v737.Text);
 			nivells[7,28] = Convert.ToInt32(v738.Text);
 			nivells[7,29] = Convert.ToInt32(v739.Text);
 			nivells[8,0] = Convert.ToInt32(p81.Text);
 			nivells[8,1] = Convert.ToInt32(p82.Text);
 			nivells[8,2] = Convert.ToInt32(p83.Text); 
 			nivells[8,3] = Convert.ToInt32(v811.Text);
 			nivells[8,4] = Convert.ToInt32(v812.Text);
 			nivells[8,5] = Convert.ToInt32(v813.Text);
 			nivells[8,6] = Convert.ToInt32(v814.Text);
 			nivells[8,7] = Convert.ToInt32(v815.Text);
 			nivells[8,8] = Convert.ToInt32(v816.Text);
 			nivells[8,9] = Convert.ToInt32(v817.Text);
 			nivells[8,10] = Convert.ToInt32(v818.Text);
 			nivells[8,11] = Convert.ToInt32(v819.Text);
 			nivells[8,12] = Convert.ToInt32(v821.Text);
 			nivells[8,13] = Convert.ToInt32(v822.Text);
 			nivells[8,14] = Convert.ToInt32(v823.Text);
 			nivells[8,15] = Convert.ToInt32(v824.Text);
 			nivells[8,16] = Convert.ToInt32(v825.Text);
 			nivells[8,17] = Convert.ToInt32(v826.Text);
 			nivells[8,18] = Convert.ToInt32(v827.Text);
 			nivells[8,19] = Convert.ToInt32(v828.Text);
 			nivells[8,20] = Convert.ToInt32(v829.Text);
 			nivells[8,21] = Convert.ToInt32(v831.Text);
 			nivells[8,22] = Convert.ToInt32(v832.Text);
 			nivells[8,23] = Convert.ToInt32(v833.Text);
 			nivells[8,24] = Convert.ToInt32(v834.Text);
 			nivells[8,25] = Convert.ToInt32(v835.Text);
 			nivells[8,26] = Convert.ToInt32(v836.Text);
 			nivells[8,27] = Convert.ToInt32(v837.Text);
 			nivells[8,28] = Convert.ToInt32(v838.Text);
 			nivells[8,29] = Convert.ToInt32(v839.Text);
 			nivells[9,0] = Convert.ToInt32(p91.Text);
 			nivells[9,1] = Convert.ToInt32(p92.Text);
 			nivells[9,2] = Convert.ToInt32(p93.Text); 
 			nivells[9,3] = Convert.ToInt32(v911.Text);
 			nivells[9,4] = Convert.ToInt32(v912.Text);
 			nivells[9,5] = Convert.ToInt32(v913.Text);
 			nivells[9,6] = Convert.ToInt32(v914.Text);
 			nivells[9,7] = Convert.ToInt32(v915.Text);
 			nivells[9,8] = Convert.ToInt32(v916.Text);
 			nivells[9,9] = Convert.ToInt32(v917.Text);
 			nivells[9,10] = Convert.ToInt32(v918.Text);
 			nivells[9,11] = Convert.ToInt32(v919.Text);
 			nivells[9,12] = Convert.ToInt32(v921.Text);
 			nivells[9,13] = Convert.ToInt32(v922.Text);
 			nivells[9,14] = Convert.ToInt32(v923.Text);
 			nivells[9,15] = Convert.ToInt32(v924.Text);
 			nivells[9,16] = Convert.ToInt32(v925.Text);
 			nivells[9,17] = Convert.ToInt32(v926.Text);
 			nivells[9,18] = Convert.ToInt32(v927.Text);
 			nivells[9,19] = Convert.ToInt32(v928.Text);
 			nivells[9,20] = Convert.ToInt32(v929.Text);
 			nivells[9,21] = Convert.ToInt32(v931.Text);
 			nivells[9,22] = Convert.ToInt32(v932.Text);
 			nivells[9,23] = Convert.ToInt32(v933.Text);
 			nivells[9,24] = Convert.ToInt32(v934.Text);
 			nivells[9,25] = Convert.ToInt32(v935.Text);
 			nivells[9,26] = Convert.ToInt32(v936.Text);
 			nivells[9,27] = Convert.ToInt32(v937.Text);
 			nivells[9,28] = Convert.ToInt32(v938.Text);
 			nivells[9,29] = Convert.ToInt32(v939.Text);
 			nivells[10,0] = Convert.ToInt32(pa1.Text);
 			nivells[10,1] = Convert.ToInt32(pa2.Text);
 			nivells[10,2] = Convert.ToInt32(pa3.Text); 
 			nivells[10,3] = Convert.ToInt32(va11.Text);
 			nivells[10,4] = Convert.ToInt32(va12.Text);
 			nivells[10,5] = Convert.ToInt32(va13.Text);
 			nivells[10,6] = Convert.ToInt32(va14.Text);
 			nivells[10,7] = Convert.ToInt32(va15.Text);
 			nivells[10,8] = Convert.ToInt32(va16.Text);
 			nivells[10,9] = Convert.ToInt32(va17.Text);
 			nivells[10,10] = Convert.ToInt32(va18.Text);
 			nivells[10,11] = Convert.ToInt32(va19.Text);
 			nivells[10,12] = Convert.ToInt32(va21.Text);
 			nivells[10,13] = Convert.ToInt32(va22.Text);
 			nivells[10,14] = Convert.ToInt32(va23.Text);
 			nivells[10,15] = Convert.ToInt32(va24.Text);
 			nivells[10,16] = Convert.ToInt32(va25.Text);
 			nivells[10,17] = Convert.ToInt32(va26.Text);
 			nivells[10,18] = Convert.ToInt32(va27.Text);
 			nivells[10,19] = Convert.ToInt32(va28.Text);
 			nivells[10,20] = Convert.ToInt32(va29.Text);
 			nivells[10,21] = Convert.ToInt32(va31.Text);
 			nivells[10,22] = Convert.ToInt32(va32.Text);
 			nivells[10,23] = Convert.ToInt32(va33.Text);
 			nivells[10,24] = Convert.ToInt32(va34.Text);
 			nivells[10,25] = Convert.ToInt32(va35.Text);
 			nivells[10,26] = Convert.ToInt32(va36.Text);
 			nivells[10,27] = Convert.ToInt32(va37.Text);
 			nivells[10,28] = Convert.ToInt32(va38.Text);
 			nivells[10,29] = Convert.ToInt32(va39.Text);
 			nivells[11,0] = Convert.ToInt32(pb1.Text);
 			nivells[11,1] = Convert.ToInt32(pb2.Text);
 			nivells[11,2] = Convert.ToInt32(pb3.Text); 
 			nivells[11,3] = Convert.ToInt32(vb11.Text);
 			nivells[11,4] = Convert.ToInt32(vb12.Text);
 			nivells[11,5] = Convert.ToInt32(vb13.Text);
 			nivells[11,6] = Convert.ToInt32(vb14.Text);
 			nivells[11,7] = Convert.ToInt32(vb15.Text);
 			nivells[11,8] = Convert.ToInt32(vb16.Text);
 			nivells[11,9] = Convert.ToInt32(vb17.Text);
 			nivells[11,10] = Convert.ToInt32(vb18.Text);
 			nivells[11,11] = Convert.ToInt32(vb19.Text);
 			nivells[11,12] = Convert.ToInt32(vb21.Text);
 			nivells[11,13] = Convert.ToInt32(vb22.Text);
 			nivells[11,14] = Convert.ToInt32(vb23.Text);
 			nivells[11,15] = Convert.ToInt32(vb24.Text);
 			nivells[11,16] = Convert.ToInt32(vb25.Text);
 			nivells[11,17] = Convert.ToInt32(vb26.Text);
 			nivells[11,18] = Convert.ToInt32(vb27.Text);
 			nivells[11,19] = Convert.ToInt32(vb28.Text);
 			nivells[11,20] = Convert.ToInt32(vb29.Text);
 			nivells[11,21] = Convert.ToInt32(vb31.Text);
 			nivells[11,22] = Convert.ToInt32(vb32.Text);
 			nivells[11,23] = Convert.ToInt32(vb33.Text);
 			nivells[11,24] = Convert.ToInt32(vb34.Text);
 			nivells[11,25] = Convert.ToInt32(vb35.Text);
 			nivells[11,26] = Convert.ToInt32(vb36.Text);
 			nivells[11,27] = Convert.ToInt32(vb37.Text);
 			nivells[11,28] = Convert.ToInt32(vb38.Text);
 			nivells[11,29] = Convert.ToInt32(vb39.Text);
 		}
		private bool Valida(string columna) {
 			string trio; int n1, n2, n3, nv=0;
 			for (int nr=0; nr<5; nr++) rks[nr,2]=0;
 			for (int nr=0; nr<12; nr++) {
 				n1=nivells[nr,0]-1; n2=nivells[nr,1]-1; n3=nivells[nr,2]-1;
 				if (n1<0 || n2<0 || n3<0) continue;
 				columna = columna.ToUpper();
 				trio=columna.Substring(n1,1)+columna.Substring(n2,1)+columna.Substring(n3,1);
 				switch (trio) {
 					case "111": nv=nivells[nr,3]; break;
 					case "11X": nv=nivells[nr,4]; break;
 					case "112": nv=nivells[nr,5]; break;
 					case "1X1": nv=nivells[nr,6]; break;
 					case "1XX": nv=nivells[nr,7]; break;
 					case "1X2": nv=nivells[nr,8]; break;
 					case "121": nv=nivells[nr,9]; break;
 					case "12X": nv=nivells[nr,10]; break;
 					case "122": nv=nivells[nr,11]; break;
 					case "X11": nv=nivells[nr,12]; break;
 					case "X1X": nv=nivells[nr,13]; break;
 					case "X12": nv=nivells[nr,14]; break;
 					case "XX1": nv=nivells[nr,15]; break;
 					case "XXX": nv=nivells[nr,16]; break;
 					case "XX2": nv=nivells[nr,17]; break;
 					case "X21": nv=nivells[nr,18]; break;
 					case "X2X": nv=nivells[nr,19]; break;
 					case "X22": nv=nivells[nr,20]; break;
 					case "211": nv=nivells[nr,21]; break;
 					case "21X": nv=nivells[nr,22]; break;
 					case "212": nv=nivells[nr,23]; break;
 					case "2X1": nv=nivells[nr,24]; break;
 					case "2XX": nv=nivells[nr,25]; break;
 					case "2X2": nv=nivells[nr,26]; break;
 					case "221": nv=nivells[nr,27]; break;
 					case "22X": nv=nivells[nr,28]; break;
 					case "222": nv=nivells[nr,29]; break;
 				}
 				if (nv>0) rks[nv-1,2]++;
 			}
 			for (int nr=0; nr<5; nr++) {
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
 		}
 		
		
		#region Windows Forms Designer generated code
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TriosFrm));
            this.v718 = new TextBox();
            this.v719 = new TextBox();
            this.v712 = new TextBox();
            this.v713 = new TextBox();
            this.v711 = new TextBox();
            this.v716 = new TextBox();
            this.v717 = new TextBox();
            this.v714 = new TextBox();
            this.v715 = new TextBox();
            this.v418 = new TextBox();
            this.c41 = new TextBox();
            this.v411 = new TextBox();
            this.p53 = new TextBox();
            this.p52 = new TextBox();
            this.p51 = new TextBox();
            this.v415 = new TextBox();
            this.v036 = new TextBox();
            this.v035 = new TextBox();
            this.v416 = new TextBox();
            this.v033 = new TextBox();
            this.v729 = new TextBox();
            this.v728 = new TextBox();
            this.v511 = new TextBox();
            this.v138 = new TextBox();
            this.v139 = new TextBox();
            this.v136 = new TextBox();
            this.v137 = new TextBox();
            this.v134 = new TextBox();
            this.v135 = new TextBox();
            this.v132 = new TextBox();
            this.v133 = new TextBox();
            this.v131 = new TextBox();
            this.p42 = new TextBox();
            this.p43 = new TextBox();
            this.v029 = new TextBox();
            this.p41 = new TextBox();
            this.v027 = new TextBox();
            this.v024 = new TextBox();
            this.v025 = new TextBox();
            this.v022 = new TextBox();
            this.v023 = new TextBox();
            this.v021 = new TextBox();
            this.va29 = new TextBox();
            this.v815 = new TextBox();
            this.v814 = new TextBox();
            this.v817 = new TextBox();
            this.v816 = new TextBox();
            this.v811 = new TextBox();
            this.va23 = new TextBox();
            this.v813 = new TextBox();
            this.v812 = new TextBox();
            this.v819 = new TextBox();
            this.v818 = new TextBox();
            this.v431 = new TextBox();
            this.v914 = new TextBox();
            this.v437 = new TextBox();
            this.v436 = new TextBox();
            this.v435 = new TextBox();
            this.v434 = new TextBox();
            this.v911 = new TextBox();
            this.v912 = new TextBox();
            this.v913 = new TextBox();
            this.v918 = new TextBox();
            this.v533 = new TextBox();
            this.v531 = new TextBox();
            this.v536 = new TextBox();
            this.v537 = new TextBox();
            this.v534 = new TextBox();
            this.v535 = new TextBox();
            this.v428 = new TextBox();
            this.v429 = new TextBox();
            this.v422 = new TextBox();
            this.v423 = new TextBox();
            this.v421 = new TextBox();
            this.v426 = new TextBox();
            this.v427 = new TextBox();
            this.v424 = new TextBox();
            this.v425 = new TextBox();
            this.v129 = new TextBox();
            this.v128 = new TextBox();
            this.v127 = new TextBox();
            this.v126 = new TextBox();
            this.v125 = new TextBox();
            this.v124 = new TextBox();
            this.v123 = new TextBox();
            this.v122 = new TextBox();
            this.v121 = new TextBox();
            this.v831 = new TextBox();
            this.v839 = new TextBox();
            this.v936 = new TextBox();
            this.v937 = new TextBox();
            this.v934 = new TextBox();
            this.v935 = new TextBox();
            this.v932 = new TextBox();
            this.v933 = new TextBox();
            this.v931 = new TextBox();
            this.v938 = new TextBox();
            this.v939 = new TextBox();
            this.v826 = new TextBox();
            this.v827 = new TextBox();
            this.v824 = new TextBox();
            this.v825 = new TextBox();
            this.v822 = new TextBox();
            this.v823 = new TextBox();
            this.v821 = new TextBox();
            this.v828 = new TextBox();
            this.v829 = new TextBox();
            this.v529 = new TextBox();
            this.v528 = new TextBox();
            this.v523 = new TextBox();
            this.v522 = new TextBox();
            this.v521 = new TextBox();
            this.v915 = new TextBox();
            this.v527 = new TextBox();
            this.v526 = new TextBox();
            this.v525 = new TextBox();
            this.v524 = new TextBox();
            this.bGrabar = new Button();
            this.v919 = new TextBox();
            this.c22 = new TextBox();
            this.c21 = new TextBox();
            this.label13 = new Label();
            this.bCalcular = new Button();
            this.groupBox1 = new GroupBox();
            this.vb39 = new TextBox();
            this.vb38 = new TextBox();
            this.vb37 = new TextBox();
            this.vb36 = new TextBox();
            this.vb35 = new TextBox();
            this.vb34 = new TextBox();
            this.vb33 = new TextBox();
            this.vb32 = new TextBox();
            this.vb31 = new TextBox();
            this.va39 = new TextBox();
            this.va38 = new TextBox();
            this.va37 = new TextBox();
            this.va36 = new TextBox();
            this.va35 = new TextBox();
            this.va34 = new TextBox();
            this.va33 = new TextBox();
            this.va32 = new TextBox();
            this.va31 = new TextBox();
            this.vb29 = new TextBox();
            this.vb28 = new TextBox();
            this.vb27 = new TextBox();
            this.vb26 = new TextBox();
            this.vb25 = new TextBox();
            this.vb24 = new TextBox();
            this.vb23 = new TextBox();
            this.vb22 = new TextBox();
            this.vb21 = new TextBox();
            this.va28 = new TextBox();
            this.va27 = new TextBox();
            this.va26 = new TextBox();
            this.va25 = new TextBox();
            this.va24 = new TextBox();
            this.va22 = new TextBox();
            this.va21 = new TextBox();
            this.pb3 = new TextBox();
            this.pa3 = new TextBox();
            this.vb19 = new TextBox();
            this.vb18 = new TextBox();
            this.vb17 = new TextBox();
            this.vb16 = new TextBox();
            this.vb15 = new TextBox();
            this.vb14 = new TextBox();
            this.vb13 = new TextBox();
            this.vb12 = new TextBox();
            this.vb11 = new TextBox();
            this.pb2 = new TextBox();
            this.pb1 = new TextBox();
            this.va19 = new TextBox();
            this.va18 = new TextBox();
            this.va17 = new TextBox();
            this.va16 = new TextBox();
            this.va15 = new TextBox();
            this.va14 = new TextBox();
            this.va13 = new TextBox();
            this.va12 = new TextBox();
            this.va11 = new TextBox();
            this.pa2 = new TextBox();
            this.pa1 = new TextBox();
            this.label42 = new Label();
            this.label41 = new Label();
            this.label40 = new Label();
            this.v838 = new TextBox();
            this.v837 = new TextBox();
            this.v836 = new TextBox();
            this.v835 = new TextBox();
            this.v834 = new TextBox();
            this.v833 = new TextBox();
            this.v832 = new TextBox();
            this.v739 = new TextBox();
            this.v738 = new TextBox();
            this.v737 = new TextBox();
            this.v736 = new TextBox();
            this.v735 = new TextBox();
            this.v734 = new TextBox();
            this.v733 = new TextBox();
            this.v732 = new TextBox();
            this.v731 = new TextBox();
            this.v639 = new TextBox();
            this.v638 = new TextBox();
            this.v637 = new TextBox();
            this.v636 = new TextBox();
            this.v635 = new TextBox();
            this.v634 = new TextBox();
            this.v633 = new TextBox();
            this.v632 = new TextBox();
            this.v631 = new TextBox();
            this.v539 = new TextBox();
            this.v538 = new TextBox();
            this.v532 = new TextBox();
            this.v439 = new TextBox();
            this.v438 = new TextBox();
            this.v433 = new TextBox();
            this.v432 = new TextBox();
            this.v339 = new TextBox();
            this.v338 = new TextBox();
            this.v337 = new TextBox();
            this.v336 = new TextBox();
            this.v335 = new TextBox();
            this.v334 = new TextBox();
            this.v333 = new TextBox();
            this.v332 = new TextBox();
            this.v331 = new TextBox();
            this.v239 = new TextBox();
            this.v238 = new TextBox();
            this.v237 = new TextBox();
            this.v236 = new TextBox();
            this.v235 = new TextBox();
            this.v234 = new TextBox();
            this.v233 = new TextBox();
            this.v232 = new TextBox();
            this.v231 = new TextBox();
            this.v039 = new TextBox();
            this.v038 = new TextBox();
            this.v037 = new TextBox();
            this.v034 = new TextBox();
            this.v032 = new TextBox();
            this.v031 = new TextBox();
            this.label31 = new Label();
            this.label32 = new Label();
            this.label33 = new Label();
            this.label34 = new Label();
            this.label35 = new Label();
            this.label36 = new Label();
            this.label37 = new Label();
            this.label38 = new Label();
            this.label39 = new Label();
            this.v929 = new TextBox();
            this.v928 = new TextBox();
            this.v927 = new TextBox();
            this.v926 = new TextBox();
            this.v925 = new TextBox();
            this.v924 = new TextBox();
            this.v923 = new TextBox();
            this.v922 = new TextBox();
            this.v921 = new TextBox();
            this.v727 = new TextBox();
            this.v726 = new TextBox();
            this.v725 = new TextBox();
            this.v724 = new TextBox();
            this.v723 = new TextBox();
            this.v722 = new TextBox();
            this.v721 = new TextBox();
            this.v629 = new TextBox();
            this.v628 = new TextBox();
            this.v627 = new TextBox();
            this.v626 = new TextBox();
            this.v625 = new TextBox();
            this.v624 = new TextBox();
            this.v623 = new TextBox();
            this.v622 = new TextBox();
            this.v621 = new TextBox();
            this.v329 = new TextBox();
            this.v328 = new TextBox();
            this.v327 = new TextBox();
            this.v326 = new TextBox();
            this.v325 = new TextBox();
            this.v324 = new TextBox();
            this.v323 = new TextBox();
            this.v322 = new TextBox();
            this.v321 = new TextBox();
            this.v229 = new TextBox();
            this.v228 = new TextBox();
            this.v227 = new TextBox();
            this.v226 = new TextBox();
            this.v225 = new TextBox();
            this.v224 = new TextBox();
            this.v223 = new TextBox();
            this.v222 = new TextBox();
            this.v221 = new TextBox();
            this.v028 = new TextBox();
            this.v026 = new TextBox();
            this.label12 = new Label();
            this.label18 = new Label();
            this.label20 = new Label();
            this.label22 = new Label();
            this.label24 = new Label();
            this.label26 = new Label();
            this.label27 = new Label();
            this.label28 = new Label();
            this.label29 = new Label();
            this.p93 = new TextBox();
            this.p83 = new TextBox();
            this.p73 = new TextBox();
            this.p63 = new TextBox();
            this.p33 = new TextBox();
            this.p23 = new TextBox();
            this.p13 = new TextBox();
            this.p03 = new TextBox();
            this.v917 = new TextBox();
            this.v916 = new TextBox();
            this.p92 = new TextBox();
            this.p91 = new TextBox();
            this.p82 = new TextBox();
            this.p81 = new TextBox();
            this.p72 = new TextBox();
            this.p71 = new TextBox();
            this.v619 = new TextBox();
            this.v618 = new TextBox();
            this.v617 = new TextBox();
            this.v616 = new TextBox();
            this.v615 = new TextBox();
            this.v614 = new TextBox();
            this.v613 = new TextBox();
            this.v612 = new TextBox();
            this.v611 = new TextBox();
            this.p62 = new TextBox();
            this.p61 = new TextBox();
            this.v519 = new TextBox();
            this.v518 = new TextBox();
            this.v517 = new TextBox();
            this.v516 = new TextBox();
            this.v515 = new TextBox();
            this.v514 = new TextBox();
            this.v513 = new TextBox();
            this.v512 = new TextBox();
            this.v419 = new TextBox();
            this.v417 = new TextBox();
            this.v414 = new TextBox();
            this.v413 = new TextBox();
            this.v412 = new TextBox();
            this.v319 = new TextBox();
            this.v318 = new TextBox();
            this.v317 = new TextBox();
            this.v316 = new TextBox();
            this.v315 = new TextBox();
            this.v314 = new TextBox();
            this.v313 = new TextBox();
            this.v312 = new TextBox();
            this.v311 = new TextBox();
            this.p32 = new TextBox();
            this.p31 = new TextBox();
            this.v219 = new TextBox();
            this.v218 = new TextBox();
            this.v217 = new TextBox();
            this.v216 = new TextBox();
            this.v215 = new TextBox();
            this.v214 = new TextBox();
            this.v213 = new TextBox();
            this.v212 = new TextBox();
            this.v211 = new TextBox();
            this.p22 = new TextBox();
            this.p21 = new TextBox();
            this.v119 = new TextBox();
            this.v118 = new TextBox();
            this.v117 = new TextBox();
            this.v116 = new TextBox();
            this.v115 = new TextBox();
            this.v114 = new TextBox();
            this.v113 = new TextBox();
            this.v112 = new TextBox();
            this.v111 = new TextBox();
            this.p12 = new TextBox();
            this.p11 = new TextBox();
            this.v019 = new TextBox();
            this.v018 = new TextBox();
            this.v017 = new TextBox();
            this.v016 = new TextBox();
            this.v015 = new TextBox();
            this.v014 = new TextBox();
            this.v013 = new TextBox();
            this.v012 = new TextBox();
            this.v011 = new TextBox();
            this.p02 = new TextBox();
            this.p01 = new TextBox();
            this.label8 = new Label();
            this.label9 = new Label();
            this.label10 = new Label();
            this.label5 = new Label();
            this.label6 = new Label();
            this.label7 = new Label();
            this.label4 = new Label();
            this.label3 = new Label();
            this.label2 = new Label();
            this.label1 = new Label();
            this.groupBox2 = new GroupBox();
            this.label25 = new Label();
            this.c51 = new TextBox();
            this.c52 = new TextBox();
            this.label23 = new Label();
            this.r5 = new Label();
            this.c42 = new TextBox();
            this.label21 = new Label();
            this.r4 = new Label();
            this.c31 = new TextBox();
            this.c32 = new TextBox();
            this.label19 = new Label();
            this.r3 = new Label();
            this.label11 = new Label();
            this.r2 = new Label();
            this.c11 = new TextBox();
            this.c12 = new TextBox();
            this.label17 = new Label();
            this.label14 = new Label();
            this.label15 = new Label();
            this.label16 = new Label();
            this.r1 = new Label();
            this.bLeer = new Button();
            this.bSalvar = new Button();
            this.lval = new Label();
            this.groupBox3 = new GroupBox();
            this.label30 = new Label();
            this.tCG = new TextBox();
            this.bAnalizar = new Button();
            this.lproc = new Label();
            this.ltime = new Label();
            this.bCancelar = new Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // v718
            // 
            this.v718.BackColor = System.Drawing.Color.Khaki;
            this.v718.BorderStyle = BorderStyle.FixedSingle;
            this.v718.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v718.Location = new System.Drawing.Point(263, 217);
            this.v718.Name = "v718";
            this.v718.Size = new System.Drawing.Size(24, 21);
            this.v718.TabIndex = 718;
            this.v718.Text = "0";
            this.v718.TextAlign = HorizontalAlignment.Center;
            // 
            // v719
            // 
            this.v719.BackColor = System.Drawing.Color.Khaki;
            this.v719.BorderStyle = BorderStyle.FixedSingle;
            this.v719.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v719.Location = new System.Drawing.Point(288, 217);
            this.v719.Name = "v719";
            this.v719.Size = new System.Drawing.Size(24, 21);
            this.v719.TabIndex = 719;
            this.v719.Text = "0";
            this.v719.TextAlign = HorizontalAlignment.Center;
            // 
            // v712
            // 
            this.v712.BackColor = System.Drawing.Color.Khaki;
            this.v712.BorderStyle = BorderStyle.FixedSingle;
            this.v712.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v712.Location = new System.Drawing.Point(113, 217);
            this.v712.Name = "v712";
            this.v712.Size = new System.Drawing.Size(24, 21);
            this.v712.TabIndex = 712;
            this.v712.Text = "0";
            this.v712.TextAlign = HorizontalAlignment.Center;
            // 
            // v713
            // 
            this.v713.BackColor = System.Drawing.Color.Khaki;
            this.v713.BorderStyle = BorderStyle.FixedSingle;
            this.v713.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v713.Location = new System.Drawing.Point(138, 217);
            this.v713.Name = "v713";
            this.v713.Size = new System.Drawing.Size(24, 21);
            this.v713.TabIndex = 713;
            this.v713.Text = "0";
            this.v713.TextAlign = HorizontalAlignment.Center;
            // 
            // v711
            // 
            this.v711.BackColor = System.Drawing.Color.Khaki;
            this.v711.BorderStyle = BorderStyle.FixedSingle;
            this.v711.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v711.Location = new System.Drawing.Point(88, 217);
            this.v711.Name = "v711";
            this.v711.Size = new System.Drawing.Size(24, 21);
            this.v711.TabIndex = 711;
            this.v711.Text = "0";
            this.v711.TextAlign = HorizontalAlignment.Center;
            // 
            // v716
            // 
            this.v716.BackColor = System.Drawing.Color.Khaki;
            this.v716.BorderStyle = BorderStyle.FixedSingle;
            this.v716.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v716.Location = new System.Drawing.Point(213, 217);
            this.v716.Name = "v716";
            this.v716.Size = new System.Drawing.Size(24, 21);
            this.v716.TabIndex = 716;
            this.v716.Text = "0";
            this.v716.TextAlign = HorizontalAlignment.Center;
            // 
            // v717
            // 
            this.v717.BackColor = System.Drawing.Color.Khaki;
            this.v717.BorderStyle = BorderStyle.FixedSingle;
            this.v717.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v717.Location = new System.Drawing.Point(238, 217);
            this.v717.Name = "v717";
            this.v717.Size = new System.Drawing.Size(24, 21);
            this.v717.TabIndex = 717;
            this.v717.Text = "0";
            this.v717.TextAlign = HorizontalAlignment.Center;
            // 
            // v714
            // 
            this.v714.BackColor = System.Drawing.Color.Khaki;
            this.v714.BorderStyle = BorderStyle.FixedSingle;
            this.v714.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v714.Location = new System.Drawing.Point(163, 217);
            this.v714.Name = "v714";
            this.v714.Size = new System.Drawing.Size(24, 21);
            this.v714.TabIndex = 714;
            this.v714.Text = "0";
            this.v714.TextAlign = HorizontalAlignment.Center;
            // 
            // v715
            // 
            this.v715.BackColor = System.Drawing.Color.Khaki;
            this.v715.BorderStyle = BorderStyle.FixedSingle;
            this.v715.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v715.Location = new System.Drawing.Point(188, 217);
            this.v715.Name = "v715";
            this.v715.Size = new System.Drawing.Size(24, 21);
            this.v715.TabIndex = 715;
            this.v715.Text = "0";
            this.v715.TextAlign = HorizontalAlignment.Center;
            // 
            // v418
            // 
            this.v418.BackColor = System.Drawing.Color.Khaki;
            this.v418.BorderStyle = BorderStyle.FixedSingle;
            this.v418.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v418.Location = new System.Drawing.Point(263, 151);
            this.v418.Name = "v418";
            this.v418.Size = new System.Drawing.Size(24, 21);
            this.v418.TabIndex = 418;
            this.v418.Text = "0";
            this.v418.TextAlign = HorizontalAlignment.Center;
            // 
            // c41
            // 
            this.c41.BorderStyle = BorderStyle.FixedSingle;
            this.c41.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.c41.ForeColor = System.Drawing.Color.Black;
            this.c41.Location = new System.Drawing.Point(41, 139);
            this.c41.Name = "c41";
            this.c41.Size = new System.Drawing.Size(40, 21);
            this.c41.TabIndex = 7;
            this.c41.Text = "0";
            this.c41.TextAlign = HorizontalAlignment.Center;
            // 
            // v411
            // 
            this.v411.BackColor = System.Drawing.Color.Khaki;
            this.v411.BorderStyle = BorderStyle.FixedSingle;
            this.v411.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v411.Location = new System.Drawing.Point(88, 151);
            this.v411.Name = "v411";
            this.v411.Size = new System.Drawing.Size(24, 21);
            this.v411.TabIndex = 411;
            this.v411.Text = "0";
            this.v411.TextAlign = HorizontalAlignment.Center;
            // 
            // p53
            // 
            this.p53.BackColor = System.Drawing.Color.LemonChiffon;
            this.p53.BorderStyle = BorderStyle.FixedSingle;
            this.p53.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p53.Location = new System.Drawing.Point(57, 172);
            this.p53.Name = "p53";
            this.p53.Size = new System.Drawing.Size(24, 21);
            this.p53.TabIndex = 503;
            this.p53.Text = "0";
            this.p53.TextAlign = HorizontalAlignment.Center;
            // 
            // p52
            // 
            this.p52.BackColor = System.Drawing.Color.LemonChiffon;
            this.p52.BorderStyle = BorderStyle.FixedSingle;
            this.p52.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p52.Location = new System.Drawing.Point(32, 172);
            this.p52.Name = "p52";
            this.p52.Size = new System.Drawing.Size(24, 21);
            this.p52.TabIndex = 502;
            this.p52.Text = "0";
            this.p52.TextAlign = HorizontalAlignment.Center;
            // 
            // p51
            // 
            this.p51.BackColor = System.Drawing.Color.LemonChiffon;
            this.p51.BorderStyle = BorderStyle.FixedSingle;
            this.p51.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p51.Location = new System.Drawing.Point(7, 172);
            this.p51.Name = "p51";
            this.p51.Size = new System.Drawing.Size(24, 21);
            this.p51.TabIndex = 501;
            this.p51.Text = "0";
            this.p51.TextAlign = HorizontalAlignment.Center;
            // 
            // v415
            // 
            this.v415.BackColor = System.Drawing.Color.Khaki;
            this.v415.BorderStyle = BorderStyle.FixedSingle;
            this.v415.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v415.Location = new System.Drawing.Point(188, 151);
            this.v415.Name = "v415";
            this.v415.Size = new System.Drawing.Size(24, 21);
            this.v415.TabIndex = 415;
            this.v415.Text = "0";
            this.v415.TextAlign = HorizontalAlignment.Center;
            // 
            // v036
            // 
            this.v036.BackColor = System.Drawing.Color.Khaki;
            this.v036.BorderStyle = BorderStyle.FixedSingle;
            this.v036.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v036.Location = new System.Drawing.Point(669, 63);
            this.v036.Name = "v036";
            this.v036.Size = new System.Drawing.Size(24, 21);
            this.v036.TabIndex = 36;
            this.v036.Text = "0";
            this.v036.TextAlign = HorizontalAlignment.Center;
            // 
            // v035
            // 
            this.v035.BackColor = System.Drawing.Color.Khaki;
            this.v035.BorderStyle = BorderStyle.FixedSingle;
            this.v035.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v035.Location = new System.Drawing.Point(644, 63);
            this.v035.Name = "v035";
            this.v035.Size = new System.Drawing.Size(24, 21);
            this.v035.TabIndex = 35;
            this.v035.Text = "0";
            this.v035.TextAlign = HorizontalAlignment.Center;
            // 
            // v416
            // 
            this.v416.BackColor = System.Drawing.Color.Khaki;
            this.v416.BorderStyle = BorderStyle.FixedSingle;
            this.v416.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v416.Location = new System.Drawing.Point(213, 151);
            this.v416.Name = "v416";
            this.v416.Size = new System.Drawing.Size(24, 21);
            this.v416.TabIndex = 416;
            this.v416.Text = "0";
            this.v416.TextAlign = HorizontalAlignment.Center;
            // 
            // v033
            // 
            this.v033.BackColor = System.Drawing.Color.Khaki;
            this.v033.BorderStyle = BorderStyle.FixedSingle;
            this.v033.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v033.Location = new System.Drawing.Point(594, 63);
            this.v033.Name = "v033";
            this.v033.Size = new System.Drawing.Size(24, 21);
            this.v033.TabIndex = 33;
            this.v033.Text = "0";
            this.v033.TextAlign = HorizontalAlignment.Center;
            // 
            // v729
            // 
            this.v729.BorderStyle = BorderStyle.FixedSingle;
            this.v729.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v729.Location = new System.Drawing.Point(516, 217);
            this.v729.Name = "v729";
            this.v729.Size = new System.Drawing.Size(24, 21);
            this.v729.TabIndex = 729;
            this.v729.Text = "0";
            this.v729.TextAlign = HorizontalAlignment.Center;
            // 
            // v728
            // 
            this.v728.BorderStyle = BorderStyle.FixedSingle;
            this.v728.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v728.Location = new System.Drawing.Point(491, 217);
            this.v728.Name = "v728";
            this.v728.Size = new System.Drawing.Size(24, 21);
            this.v728.TabIndex = 728;
            this.v728.Text = "0";
            this.v728.TextAlign = HorizontalAlignment.Center;
            // 
            // v511
            // 
            this.v511.BackColor = System.Drawing.Color.Khaki;
            this.v511.BorderStyle = BorderStyle.FixedSingle;
            this.v511.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v511.Location = new System.Drawing.Point(88, 173);
            this.v511.Name = "v511";
            this.v511.Size = new System.Drawing.Size(24, 21);
            this.v511.TabIndex = 511;
            this.v511.Text = "0";
            this.v511.TextAlign = HorizontalAlignment.Center;
            // 
            // v138
            // 
            this.v138.BackColor = System.Drawing.Color.Khaki;
            this.v138.BorderStyle = BorderStyle.FixedSingle;
            this.v138.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v138.Location = new System.Drawing.Point(719, 85);
            this.v138.Name = "v138";
            this.v138.Size = new System.Drawing.Size(24, 21);
            this.v138.TabIndex = 138;
            this.v138.Text = "0";
            this.v138.TextAlign = HorizontalAlignment.Center;
            // 
            // v139
            // 
            this.v139.BackColor = System.Drawing.Color.Khaki;
            this.v139.BorderStyle = BorderStyle.FixedSingle;
            this.v139.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v139.Location = new System.Drawing.Point(744, 85);
            this.v139.Name = "v139";
            this.v139.Size = new System.Drawing.Size(24, 21);
            this.v139.TabIndex = 139;
            this.v139.Text = "0";
            this.v139.TextAlign = HorizontalAlignment.Center;
            // 
            // v136
            // 
            this.v136.BackColor = System.Drawing.Color.Khaki;
            this.v136.BorderStyle = BorderStyle.FixedSingle;
            this.v136.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v136.Location = new System.Drawing.Point(669, 85);
            this.v136.Name = "v136";
            this.v136.Size = new System.Drawing.Size(24, 21);
            this.v136.TabIndex = 136;
            this.v136.Text = "0";
            this.v136.TextAlign = HorizontalAlignment.Center;
            // 
            // v137
            // 
            this.v137.BackColor = System.Drawing.Color.Khaki;
            this.v137.BorderStyle = BorderStyle.FixedSingle;
            this.v137.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v137.Location = new System.Drawing.Point(694, 85);
            this.v137.Name = "v137";
            this.v137.Size = new System.Drawing.Size(24, 21);
            this.v137.TabIndex = 137;
            this.v137.Text = "0";
            this.v137.TextAlign = HorizontalAlignment.Center;
            // 
            // v134
            // 
            this.v134.BackColor = System.Drawing.Color.Khaki;
            this.v134.BorderStyle = BorderStyle.FixedSingle;
            this.v134.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v134.Location = new System.Drawing.Point(619, 85);
            this.v134.Name = "v134";
            this.v134.Size = new System.Drawing.Size(24, 21);
            this.v134.TabIndex = 134;
            this.v134.Text = "0";
            this.v134.TextAlign = HorizontalAlignment.Center;
            // 
            // v135
            // 
            this.v135.BackColor = System.Drawing.Color.Khaki;
            this.v135.BorderStyle = BorderStyle.FixedSingle;
            this.v135.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v135.Location = new System.Drawing.Point(644, 85);
            this.v135.Name = "v135";
            this.v135.Size = new System.Drawing.Size(24, 21);
            this.v135.TabIndex = 135;
            this.v135.Text = "0";
            this.v135.TextAlign = HorizontalAlignment.Center;
            // 
            // v132
            // 
            this.v132.BackColor = System.Drawing.Color.Khaki;
            this.v132.BorderStyle = BorderStyle.FixedSingle;
            this.v132.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v132.Location = new System.Drawing.Point(569, 85);
            this.v132.Name = "v132";
            this.v132.Size = new System.Drawing.Size(24, 21);
            this.v132.TabIndex = 132;
            this.v132.Text = "0";
            this.v132.TextAlign = HorizontalAlignment.Center;
            // 
            // v133
            // 
            this.v133.BackColor = System.Drawing.Color.Khaki;
            this.v133.BorderStyle = BorderStyle.FixedSingle;
            this.v133.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v133.Location = new System.Drawing.Point(594, 85);
            this.v133.Name = "v133";
            this.v133.Size = new System.Drawing.Size(24, 21);
            this.v133.TabIndex = 133;
            this.v133.Text = "0";
            this.v133.TextAlign = HorizontalAlignment.Center;
            // 
            // v131
            // 
            this.v131.BackColor = System.Drawing.Color.Khaki;
            this.v131.BorderStyle = BorderStyle.FixedSingle;
            this.v131.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v131.Location = new System.Drawing.Point(544, 85);
            this.v131.Name = "v131";
            this.v131.Size = new System.Drawing.Size(24, 21);
            this.v131.TabIndex = 131;
            this.v131.Text = "0";
            this.v131.TextAlign = HorizontalAlignment.Center;
            // 
            // p42
            // 
            this.p42.BackColor = System.Drawing.Color.LemonChiffon;
            this.p42.BorderStyle = BorderStyle.FixedSingle;
            this.p42.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p42.Location = new System.Drawing.Point(32, 150);
            this.p42.Name = "p42";
            this.p42.Size = new System.Drawing.Size(24, 21);
            this.p42.TabIndex = 402;
            this.p42.Text = "0";
            this.p42.TextAlign = HorizontalAlignment.Center;
            // 
            // p43
            // 
            this.p43.BackColor = System.Drawing.Color.LemonChiffon;
            this.p43.BorderStyle = BorderStyle.FixedSingle;
            this.p43.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p43.Location = new System.Drawing.Point(57, 150);
            this.p43.Name = "p43";
            this.p43.Size = new System.Drawing.Size(24, 21);
            this.p43.TabIndex = 403;
            this.p43.Text = "0";
            this.p43.TextAlign = HorizontalAlignment.Center;
            // 
            // v029
            // 
            this.v029.BorderStyle = BorderStyle.FixedSingle;
            this.v029.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v029.Location = new System.Drawing.Point(516, 63);
            this.v029.Name = "v029";
            this.v029.Size = new System.Drawing.Size(24, 21);
            this.v029.TabIndex = 29;
            this.v029.Text = "0";
            this.v029.TextAlign = HorizontalAlignment.Center;
            // 
            // p41
            // 
            this.p41.BackColor = System.Drawing.Color.LemonChiffon;
            this.p41.BorderStyle = BorderStyle.FixedSingle;
            this.p41.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p41.Location = new System.Drawing.Point(7, 150);
            this.p41.Name = "p41";
            this.p41.Size = new System.Drawing.Size(24, 21);
            this.p41.TabIndex = 401;
            this.p41.Text = "0";
            this.p41.TextAlign = HorizontalAlignment.Center;
            // 
            // v027
            // 
            this.v027.BorderStyle = BorderStyle.FixedSingle;
            this.v027.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v027.Location = new System.Drawing.Point(466, 63);
            this.v027.Name = "v027";
            this.v027.Size = new System.Drawing.Size(24, 21);
            this.v027.TabIndex = 27;
            this.v027.Text = "0";
            this.v027.TextAlign = HorizontalAlignment.Center;
            // 
            // v024
            // 
            this.v024.BorderStyle = BorderStyle.FixedSingle;
            this.v024.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v024.Location = new System.Drawing.Point(391, 63);
            this.v024.Name = "v024";
            this.v024.Size = new System.Drawing.Size(24, 21);
            this.v024.TabIndex = 24;
            this.v024.Text = "0";
            this.v024.TextAlign = HorizontalAlignment.Center;
            // 
            // v025
            // 
            this.v025.BorderStyle = BorderStyle.FixedSingle;
            this.v025.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v025.Location = new System.Drawing.Point(416, 63);
            this.v025.Name = "v025";
            this.v025.Size = new System.Drawing.Size(24, 21);
            this.v025.TabIndex = 25;
            this.v025.Text = "0";
            this.v025.TextAlign = HorizontalAlignment.Center;
            // 
            // v022
            // 
            this.v022.BorderStyle = BorderStyle.FixedSingle;
            this.v022.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v022.Location = new System.Drawing.Point(341, 63);
            this.v022.Name = "v022";
            this.v022.Size = new System.Drawing.Size(24, 21);
            this.v022.TabIndex = 22;
            this.v022.Text = "0";
            this.v022.TextAlign = HorizontalAlignment.Center;
            // 
            // v023
            // 
            this.v023.BorderStyle = BorderStyle.FixedSingle;
            this.v023.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v023.Location = new System.Drawing.Point(366, 63);
            this.v023.Name = "v023";
            this.v023.Size = new System.Drawing.Size(24, 21);
            this.v023.TabIndex = 23;
            this.v023.Text = "0";
            this.v023.TextAlign = HorizontalAlignment.Center;
            // 
            // v021
            // 
            this.v021.BorderStyle = BorderStyle.FixedSingle;
            this.v021.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v021.Location = new System.Drawing.Point(316, 63);
            this.v021.Name = "v021";
            this.v021.Size = new System.Drawing.Size(24, 21);
            this.v021.TabIndex = 21;
            this.v021.Text = "0";
            this.v021.TextAlign = HorizontalAlignment.Center;
            // 
            // va29
            // 
            this.va29.BorderStyle = BorderStyle.FixedSingle;
            this.va29.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.va29.Location = new System.Drawing.Point(516, 283);
            this.va29.Name = "va29";
            this.va29.Size = new System.Drawing.Size(24, 21);
            this.va29.TabIndex = 1003;
            this.va29.Text = "0";
            this.va29.TextAlign = HorizontalAlignment.Center;
            // 
            // v815
            // 
            this.v815.BackColor = System.Drawing.Color.Khaki;
            this.v815.BorderStyle = BorderStyle.FixedSingle;
            this.v815.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v815.Location = new System.Drawing.Point(188, 239);
            this.v815.Name = "v815";
            this.v815.Size = new System.Drawing.Size(24, 21);
            this.v815.TabIndex = 815;
            this.v815.Text = "0";
            this.v815.TextAlign = HorizontalAlignment.Center;
            // 
            // v814
            // 
            this.v814.BackColor = System.Drawing.Color.Khaki;
            this.v814.BorderStyle = BorderStyle.FixedSingle;
            this.v814.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v814.Location = new System.Drawing.Point(163, 239);
            this.v814.Name = "v814";
            this.v814.Size = new System.Drawing.Size(24, 21);
            this.v814.TabIndex = 814;
            this.v814.Text = "0";
            this.v814.TextAlign = HorizontalAlignment.Center;
            // 
            // v817
            // 
            this.v817.BackColor = System.Drawing.Color.Khaki;
            this.v817.BorderStyle = BorderStyle.FixedSingle;
            this.v817.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v817.Location = new System.Drawing.Point(238, 239);
            this.v817.Name = "v817";
            this.v817.Size = new System.Drawing.Size(24, 21);
            this.v817.TabIndex = 817;
            this.v817.Text = "0";
            this.v817.TextAlign = HorizontalAlignment.Center;
            // 
            // v816
            // 
            this.v816.BackColor = System.Drawing.Color.Khaki;
            this.v816.BorderStyle = BorderStyle.FixedSingle;
            this.v816.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v816.Location = new System.Drawing.Point(213, 239);
            this.v816.Name = "v816";
            this.v816.Size = new System.Drawing.Size(24, 21);
            this.v816.TabIndex = 816;
            this.v816.Text = "0";
            this.v816.TextAlign = HorizontalAlignment.Center;
            // 
            // v811
            // 
            this.v811.BackColor = System.Drawing.Color.Khaki;
            this.v811.BorderStyle = BorderStyle.FixedSingle;
            this.v811.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v811.Location = new System.Drawing.Point(88, 239);
            this.v811.Name = "v811";
            this.v811.Size = new System.Drawing.Size(24, 21);
            this.v811.TabIndex = 811;
            this.v811.Text = "0";
            this.v811.TextAlign = HorizontalAlignment.Center;
            // 
            // va23
            // 
            this.va23.BorderStyle = BorderStyle.FixedSingle;
            this.va23.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.va23.Location = new System.Drawing.Point(366, 283);
            this.va23.Name = "va23";
            this.va23.Size = new System.Drawing.Size(24, 21);
            this.va23.TabIndex = 997;
            this.va23.Text = "0";
            this.va23.TextAlign = HorizontalAlignment.Center;
            // 
            // v813
            // 
            this.v813.BackColor = System.Drawing.Color.Khaki;
            this.v813.BorderStyle = BorderStyle.FixedSingle;
            this.v813.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v813.Location = new System.Drawing.Point(138, 239);
            this.v813.Name = "v813";
            this.v813.Size = new System.Drawing.Size(24, 21);
            this.v813.TabIndex = 813;
            this.v813.Text = "0";
            this.v813.TextAlign = HorizontalAlignment.Center;
            // 
            // v812
            // 
            this.v812.BackColor = System.Drawing.Color.Khaki;
            this.v812.BorderStyle = BorderStyle.FixedSingle;
            this.v812.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v812.Location = new System.Drawing.Point(113, 239);
            this.v812.Name = "v812";
            this.v812.Size = new System.Drawing.Size(24, 21);
            this.v812.TabIndex = 812;
            this.v812.Text = "0";
            this.v812.TextAlign = HorizontalAlignment.Center;
            // 
            // v819
            // 
            this.v819.BackColor = System.Drawing.Color.Khaki;
            this.v819.BorderStyle = BorderStyle.FixedSingle;
            this.v819.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v819.Location = new System.Drawing.Point(288, 239);
            this.v819.Name = "v819";
            this.v819.Size = new System.Drawing.Size(24, 21);
            this.v819.TabIndex = 819;
            this.v819.Text = "0";
            this.v819.TextAlign = HorizontalAlignment.Center;
            // 
            // v818
            // 
            this.v818.BackColor = System.Drawing.Color.Khaki;
            this.v818.BorderStyle = BorderStyle.FixedSingle;
            this.v818.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v818.Location = new System.Drawing.Point(263, 239);
            this.v818.Name = "v818";
            this.v818.Size = new System.Drawing.Size(24, 21);
            this.v818.TabIndex = 818;
            this.v818.Text = "0";
            this.v818.TextAlign = HorizontalAlignment.Center;
            // 
            // v431
            // 
            this.v431.BackColor = System.Drawing.Color.Khaki;
            this.v431.BorderStyle = BorderStyle.FixedSingle;
            this.v431.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v431.Location = new System.Drawing.Point(544, 151);
            this.v431.Name = "v431";
            this.v431.Size = new System.Drawing.Size(24, 21);
            this.v431.TabIndex = 431;
            this.v431.Text = "0";
            this.v431.TextAlign = HorizontalAlignment.Center;
            // 
            // v914
            // 
            this.v914.BackColor = System.Drawing.Color.Khaki;
            this.v914.BorderStyle = BorderStyle.FixedSingle;
            this.v914.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v914.Location = new System.Drawing.Point(163, 261);
            this.v914.Name = "v914";
            this.v914.Size = new System.Drawing.Size(24, 21);
            this.v914.TabIndex = 914;
            this.v914.Text = "0";
            this.v914.TextAlign = HorizontalAlignment.Center;
            // 
            // v437
            // 
            this.v437.BackColor = System.Drawing.Color.Khaki;
            this.v437.BorderStyle = BorderStyle.FixedSingle;
            this.v437.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v437.Location = new System.Drawing.Point(694, 151);
            this.v437.Name = "v437";
            this.v437.Size = new System.Drawing.Size(24, 21);
            this.v437.TabIndex = 437;
            this.v437.Text = "0";
            this.v437.TextAlign = HorizontalAlignment.Center;
            // 
            // v436
            // 
            this.v436.BackColor = System.Drawing.Color.Khaki;
            this.v436.BorderStyle = BorderStyle.FixedSingle;
            this.v436.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v436.Location = new System.Drawing.Point(669, 151);
            this.v436.Name = "v436";
            this.v436.Size = new System.Drawing.Size(24, 21);
            this.v436.TabIndex = 436;
            this.v436.Text = "0";
            this.v436.TextAlign = HorizontalAlignment.Center;
            // 
            // v435
            // 
            this.v435.BackColor = System.Drawing.Color.Khaki;
            this.v435.BorderStyle = BorderStyle.FixedSingle;
            this.v435.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v435.Location = new System.Drawing.Point(644, 151);
            this.v435.Name = "v435";
            this.v435.Size = new System.Drawing.Size(24, 21);
            this.v435.TabIndex = 435;
            this.v435.Text = "0";
            this.v435.TextAlign = HorizontalAlignment.Center;
            // 
            // v434
            // 
            this.v434.BackColor = System.Drawing.Color.Khaki;
            this.v434.BorderStyle = BorderStyle.FixedSingle;
            this.v434.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v434.Location = new System.Drawing.Point(619, 151);
            this.v434.Name = "v434";
            this.v434.Size = new System.Drawing.Size(24, 21);
            this.v434.TabIndex = 434;
            this.v434.Text = "0";
            this.v434.TextAlign = HorizontalAlignment.Center;
            // 
            // v911
            // 
            this.v911.BackColor = System.Drawing.Color.Khaki;
            this.v911.BorderStyle = BorderStyle.FixedSingle;
            this.v911.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v911.Location = new System.Drawing.Point(88, 261);
            this.v911.Name = "v911";
            this.v911.Size = new System.Drawing.Size(24, 21);
            this.v911.TabIndex = 911;
            this.v911.Text = "0";
            this.v911.TextAlign = HorizontalAlignment.Center;
            // 
            // v912
            // 
            this.v912.BackColor = System.Drawing.Color.Khaki;
            this.v912.BorderStyle = BorderStyle.FixedSingle;
            this.v912.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v912.Location = new System.Drawing.Point(113, 261);
            this.v912.Name = "v912";
            this.v912.Size = new System.Drawing.Size(24, 21);
            this.v912.TabIndex = 912;
            this.v912.Text = "0";
            this.v912.TextAlign = HorizontalAlignment.Center;
            // 
            // v913
            // 
            this.v913.BackColor = System.Drawing.Color.Khaki;
            this.v913.BorderStyle = BorderStyle.FixedSingle;
            this.v913.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v913.Location = new System.Drawing.Point(138, 261);
            this.v913.Name = "v913";
            this.v913.Size = new System.Drawing.Size(24, 21);
            this.v913.TabIndex = 913;
            this.v913.Text = "0";
            this.v913.TextAlign = HorizontalAlignment.Center;
            // 
            // v918
            // 
            this.v918.BackColor = System.Drawing.Color.Khaki;
            this.v918.BorderStyle = BorderStyle.FixedSingle;
            this.v918.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v918.Location = new System.Drawing.Point(263, 261);
            this.v918.Name = "v918";
            this.v918.Size = new System.Drawing.Size(24, 21);
            this.v918.TabIndex = 918;
            this.v918.Text = "0";
            this.v918.TextAlign = HorizontalAlignment.Center;
            // 
            // v533
            // 
            this.v533.BackColor = System.Drawing.Color.Khaki;
            this.v533.BorderStyle = BorderStyle.FixedSingle;
            this.v533.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v533.Location = new System.Drawing.Point(594, 173);
            this.v533.Name = "v533";
            this.v533.Size = new System.Drawing.Size(24, 21);
            this.v533.TabIndex = 533;
            this.v533.Text = "0";
            this.v533.TextAlign = HorizontalAlignment.Center;
            // 
            // v531
            // 
            this.v531.BackColor = System.Drawing.Color.Khaki;
            this.v531.BorderStyle = BorderStyle.FixedSingle;
            this.v531.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v531.Location = new System.Drawing.Point(544, 173);
            this.v531.Name = "v531";
            this.v531.Size = new System.Drawing.Size(24, 21);
            this.v531.TabIndex = 531;
            this.v531.Text = "0";
            this.v531.TextAlign = HorizontalAlignment.Center;
            // 
            // v536
            // 
            this.v536.BackColor = System.Drawing.Color.Khaki;
            this.v536.BorderStyle = BorderStyle.FixedSingle;
            this.v536.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v536.Location = new System.Drawing.Point(669, 173);
            this.v536.Name = "v536";
            this.v536.Size = new System.Drawing.Size(24, 21);
            this.v536.TabIndex = 536;
            this.v536.Text = "0";
            this.v536.TextAlign = HorizontalAlignment.Center;
            // 
            // v537
            // 
            this.v537.BackColor = System.Drawing.Color.Khaki;
            this.v537.BorderStyle = BorderStyle.FixedSingle;
            this.v537.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v537.Location = new System.Drawing.Point(694, 173);
            this.v537.Name = "v537";
            this.v537.Size = new System.Drawing.Size(24, 21);
            this.v537.TabIndex = 537;
            this.v537.Text = "0";
            this.v537.TextAlign = HorizontalAlignment.Center;
            // 
            // v534
            // 
            this.v534.BackColor = System.Drawing.Color.Khaki;
            this.v534.BorderStyle = BorderStyle.FixedSingle;
            this.v534.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v534.Location = new System.Drawing.Point(619, 173);
            this.v534.Name = "v534";
            this.v534.Size = new System.Drawing.Size(24, 21);
            this.v534.TabIndex = 534;
            this.v534.Text = "0";
            this.v534.TextAlign = HorizontalAlignment.Center;
            // 
            // v535
            // 
            this.v535.BackColor = System.Drawing.Color.Khaki;
            this.v535.BorderStyle = BorderStyle.FixedSingle;
            this.v535.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v535.Location = new System.Drawing.Point(644, 173);
            this.v535.Name = "v535";
            this.v535.Size = new System.Drawing.Size(24, 21);
            this.v535.TabIndex = 535;
            this.v535.Text = "0";
            this.v535.TextAlign = HorizontalAlignment.Center;
            // 
            // v428
            // 
            this.v428.BorderStyle = BorderStyle.FixedSingle;
            this.v428.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v428.Location = new System.Drawing.Point(491, 151);
            this.v428.Name = "v428";
            this.v428.Size = new System.Drawing.Size(24, 21);
            this.v428.TabIndex = 428;
            this.v428.Text = "0";
            this.v428.TextAlign = HorizontalAlignment.Center;
            // 
            // v429
            // 
            this.v429.BorderStyle = BorderStyle.FixedSingle;
            this.v429.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v429.Location = new System.Drawing.Point(516, 151);
            this.v429.Name = "v429";
            this.v429.Size = new System.Drawing.Size(24, 21);
            this.v429.TabIndex = 429;
            this.v429.Text = "0";
            this.v429.TextAlign = HorizontalAlignment.Center;
            // 
            // v422
            // 
            this.v422.BorderStyle = BorderStyle.FixedSingle;
            this.v422.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v422.Location = new System.Drawing.Point(341, 151);
            this.v422.Name = "v422";
            this.v422.Size = new System.Drawing.Size(24, 21);
            this.v422.TabIndex = 422;
            this.v422.Text = "0";
            this.v422.TextAlign = HorizontalAlignment.Center;
            // 
            // v423
            // 
            this.v423.BorderStyle = BorderStyle.FixedSingle;
            this.v423.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v423.Location = new System.Drawing.Point(366, 151);
            this.v423.Name = "v423";
            this.v423.Size = new System.Drawing.Size(24, 21);
            this.v423.TabIndex = 423;
            this.v423.Text = "0";
            this.v423.TextAlign = HorizontalAlignment.Center;
            // 
            // v421
            // 
            this.v421.BorderStyle = BorderStyle.FixedSingle;
            this.v421.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v421.Location = new System.Drawing.Point(316, 151);
            this.v421.Name = "v421";
            this.v421.Size = new System.Drawing.Size(24, 21);
            this.v421.TabIndex = 421;
            this.v421.Text = "0";
            this.v421.TextAlign = HorizontalAlignment.Center;
            // 
            // v426
            // 
            this.v426.BorderStyle = BorderStyle.FixedSingle;
            this.v426.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v426.Location = new System.Drawing.Point(441, 151);
            this.v426.Name = "v426";
            this.v426.Size = new System.Drawing.Size(24, 21);
            this.v426.TabIndex = 426;
            this.v426.Text = "0";
            this.v426.TextAlign = HorizontalAlignment.Center;
            // 
            // v427
            // 
            this.v427.BorderStyle = BorderStyle.FixedSingle;
            this.v427.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v427.Location = new System.Drawing.Point(466, 151);
            this.v427.Name = "v427";
            this.v427.Size = new System.Drawing.Size(24, 21);
            this.v427.TabIndex = 427;
            this.v427.Text = "0";
            this.v427.TextAlign = HorizontalAlignment.Center;
            // 
            // v424
            // 
            this.v424.BorderStyle = BorderStyle.FixedSingle;
            this.v424.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v424.Location = new System.Drawing.Point(391, 151);
            this.v424.Name = "v424";
            this.v424.Size = new System.Drawing.Size(24, 21);
            this.v424.TabIndex = 424;
            this.v424.Text = "0";
            this.v424.TextAlign = HorizontalAlignment.Center;
            // 
            // v425
            // 
            this.v425.BorderStyle = BorderStyle.FixedSingle;
            this.v425.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v425.Location = new System.Drawing.Point(416, 151);
            this.v425.Name = "v425";
            this.v425.Size = new System.Drawing.Size(24, 21);
            this.v425.TabIndex = 425;
            this.v425.Text = "0";
            this.v425.TextAlign = HorizontalAlignment.Center;
            // 
            // v129
            // 
            this.v129.BorderStyle = BorderStyle.FixedSingle;
            this.v129.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v129.Location = new System.Drawing.Point(516, 85);
            this.v129.Name = "v129";
            this.v129.Size = new System.Drawing.Size(24, 21);
            this.v129.TabIndex = 129;
            this.v129.Text = "0";
            this.v129.TextAlign = HorizontalAlignment.Center;
            // 
            // v128
            // 
            this.v128.BorderStyle = BorderStyle.FixedSingle;
            this.v128.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v128.Location = new System.Drawing.Point(491, 85);
            this.v128.Name = "v128";
            this.v128.Size = new System.Drawing.Size(24, 21);
            this.v128.TabIndex = 128;
            this.v128.Text = "0";
            this.v128.TextAlign = HorizontalAlignment.Center;
            // 
            // v127
            // 
            this.v127.BorderStyle = BorderStyle.FixedSingle;
            this.v127.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v127.Location = new System.Drawing.Point(466, 85);
            this.v127.Name = "v127";
            this.v127.Size = new System.Drawing.Size(24, 21);
            this.v127.TabIndex = 127;
            this.v127.Text = "0";
            this.v127.TextAlign = HorizontalAlignment.Center;
            // 
            // v126
            // 
            this.v126.BorderStyle = BorderStyle.FixedSingle;
            this.v126.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v126.Location = new System.Drawing.Point(441, 85);
            this.v126.Name = "v126";
            this.v126.Size = new System.Drawing.Size(24, 21);
            this.v126.TabIndex = 126;
            this.v126.Text = "0";
            this.v126.TextAlign = HorizontalAlignment.Center;
            // 
            // v125
            // 
            this.v125.BorderStyle = BorderStyle.FixedSingle;
            this.v125.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v125.Location = new System.Drawing.Point(416, 85);
            this.v125.Name = "v125";
            this.v125.Size = new System.Drawing.Size(24, 21);
            this.v125.TabIndex = 125;
            this.v125.Text = "0";
            this.v125.TextAlign = HorizontalAlignment.Center;
            // 
            // v124
            // 
            this.v124.BorderStyle = BorderStyle.FixedSingle;
            this.v124.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v124.Location = new System.Drawing.Point(391, 85);
            this.v124.Name = "v124";
            this.v124.Size = new System.Drawing.Size(24, 21);
            this.v124.TabIndex = 124;
            this.v124.Text = "0";
            this.v124.TextAlign = HorizontalAlignment.Center;
            // 
            // v123
            // 
            this.v123.BorderStyle = BorderStyle.FixedSingle;
            this.v123.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v123.Location = new System.Drawing.Point(366, 85);
            this.v123.Name = "v123";
            this.v123.Size = new System.Drawing.Size(24, 21);
            this.v123.TabIndex = 123;
            this.v123.Text = "0";
            this.v123.TextAlign = HorizontalAlignment.Center;
            // 
            // v122
            // 
            this.v122.BorderStyle = BorderStyle.FixedSingle;
            this.v122.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v122.Location = new System.Drawing.Point(341, 85);
            this.v122.Name = "v122";
            this.v122.Size = new System.Drawing.Size(24, 21);
            this.v122.TabIndex = 122;
            this.v122.Text = "0";
            this.v122.TextAlign = HorizontalAlignment.Center;
            // 
            // v121
            // 
            this.v121.BorderStyle = BorderStyle.FixedSingle;
            this.v121.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v121.Location = new System.Drawing.Point(316, 85);
            this.v121.Name = "v121";
            this.v121.Size = new System.Drawing.Size(24, 21);
            this.v121.TabIndex = 121;
            this.v121.Text = "0";
            this.v121.TextAlign = HorizontalAlignment.Center;
            // 
            // v831
            // 
            this.v831.BackColor = System.Drawing.Color.Khaki;
            this.v831.BorderStyle = BorderStyle.FixedSingle;
            this.v831.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v831.Location = new System.Drawing.Point(544, 239);
            this.v831.Name = "v831";
            this.v831.Size = new System.Drawing.Size(24, 21);
            this.v831.TabIndex = 831;
            this.v831.Text = "0";
            this.v831.TextAlign = HorizontalAlignment.Center;
            // 
            // v839
            // 
            this.v839.BackColor = System.Drawing.Color.Khaki;
            this.v839.BorderStyle = BorderStyle.FixedSingle;
            this.v839.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v839.Location = new System.Drawing.Point(744, 239);
            this.v839.Name = "v839";
            this.v839.Size = new System.Drawing.Size(24, 21);
            this.v839.TabIndex = 839;
            this.v839.Text = "0";
            this.v839.TextAlign = HorizontalAlignment.Center;
            // 
            // v936
            // 
            this.v936.BackColor = System.Drawing.Color.Khaki;
            this.v936.BorderStyle = BorderStyle.FixedSingle;
            this.v936.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v936.Location = new System.Drawing.Point(669, 261);
            this.v936.Name = "v936";
            this.v936.Size = new System.Drawing.Size(24, 21);
            this.v936.TabIndex = 936;
            this.v936.Text = "0";
            this.v936.TextAlign = HorizontalAlignment.Center;
            // 
            // v937
            // 
            this.v937.BackColor = System.Drawing.Color.Khaki;
            this.v937.BorderStyle = BorderStyle.FixedSingle;
            this.v937.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v937.Location = new System.Drawing.Point(694, 261);
            this.v937.Name = "v937";
            this.v937.Size = new System.Drawing.Size(24, 21);
            this.v937.TabIndex = 937;
            this.v937.Text = "0";
            this.v937.TextAlign = HorizontalAlignment.Center;
            // 
            // v934
            // 
            this.v934.BackColor = System.Drawing.Color.Khaki;
            this.v934.BorderStyle = BorderStyle.FixedSingle;
            this.v934.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v934.Location = new System.Drawing.Point(619, 261);
            this.v934.Name = "v934";
            this.v934.Size = new System.Drawing.Size(24, 21);
            this.v934.TabIndex = 934;
            this.v934.Text = "0";
            this.v934.TextAlign = HorizontalAlignment.Center;
            // 
            // v935
            // 
            this.v935.BackColor = System.Drawing.Color.Khaki;
            this.v935.BorderStyle = BorderStyle.FixedSingle;
            this.v935.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v935.Location = new System.Drawing.Point(644, 261);
            this.v935.Name = "v935";
            this.v935.Size = new System.Drawing.Size(24, 21);
            this.v935.TabIndex = 935;
            this.v935.Text = "0";
            this.v935.TextAlign = HorizontalAlignment.Center;
            // 
            // v932
            // 
            this.v932.BackColor = System.Drawing.Color.Khaki;
            this.v932.BorderStyle = BorderStyle.FixedSingle;
            this.v932.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v932.Location = new System.Drawing.Point(569, 261);
            this.v932.Name = "v932";
            this.v932.Size = new System.Drawing.Size(24, 21);
            this.v932.TabIndex = 932;
            this.v932.Text = "0";
            this.v932.TextAlign = HorizontalAlignment.Center;
            // 
            // v933
            // 
            this.v933.BackColor = System.Drawing.Color.Khaki;
            this.v933.BorderStyle = BorderStyle.FixedSingle;
            this.v933.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v933.Location = new System.Drawing.Point(594, 261);
            this.v933.Name = "v933";
            this.v933.Size = new System.Drawing.Size(24, 21);
            this.v933.TabIndex = 933;
            this.v933.Text = "0";
            this.v933.TextAlign = HorizontalAlignment.Center;
            // 
            // v931
            // 
            this.v931.BackColor = System.Drawing.Color.Khaki;
            this.v931.BorderStyle = BorderStyle.FixedSingle;
            this.v931.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v931.Location = new System.Drawing.Point(544, 261);
            this.v931.Name = "v931";
            this.v931.Size = new System.Drawing.Size(24, 21);
            this.v931.TabIndex = 931;
            this.v931.Text = "0";
            this.v931.TextAlign = HorizontalAlignment.Center;
            // 
            // v938
            // 
            this.v938.BackColor = System.Drawing.Color.Khaki;
            this.v938.BorderStyle = BorderStyle.FixedSingle;
            this.v938.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v938.Location = new System.Drawing.Point(719, 261);
            this.v938.Name = "v938";
            this.v938.Size = new System.Drawing.Size(24, 21);
            this.v938.TabIndex = 938;
            this.v938.Text = "0";
            this.v938.TextAlign = HorizontalAlignment.Center;
            // 
            // v939
            // 
            this.v939.BackColor = System.Drawing.Color.Khaki;
            this.v939.BorderStyle = BorderStyle.FixedSingle;
            this.v939.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v939.Location = new System.Drawing.Point(744, 261);
            this.v939.Name = "v939";
            this.v939.Size = new System.Drawing.Size(24, 21);
            this.v939.TabIndex = 939;
            this.v939.Text = "0";
            this.v939.TextAlign = HorizontalAlignment.Center;
            // 
            // v826
            // 
            this.v826.BorderStyle = BorderStyle.FixedSingle;
            this.v826.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v826.Location = new System.Drawing.Point(441, 239);
            this.v826.Name = "v826";
            this.v826.Size = new System.Drawing.Size(24, 21);
            this.v826.TabIndex = 826;
            this.v826.Text = "0";
            this.v826.TextAlign = HorizontalAlignment.Center;
            // 
            // v827
            // 
            this.v827.BorderStyle = BorderStyle.FixedSingle;
            this.v827.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v827.Location = new System.Drawing.Point(466, 239);
            this.v827.Name = "v827";
            this.v827.Size = new System.Drawing.Size(24, 21);
            this.v827.TabIndex = 827;
            this.v827.Text = "0";
            this.v827.TextAlign = HorizontalAlignment.Center;
            // 
            // v824
            // 
            this.v824.BorderStyle = BorderStyle.FixedSingle;
            this.v824.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v824.Location = new System.Drawing.Point(391, 239);
            this.v824.Name = "v824";
            this.v824.Size = new System.Drawing.Size(24, 21);
            this.v824.TabIndex = 824;
            this.v824.Text = "0";
            this.v824.TextAlign = HorizontalAlignment.Center;
            // 
            // v825
            // 
            this.v825.BorderStyle = BorderStyle.FixedSingle;
            this.v825.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v825.Location = new System.Drawing.Point(416, 239);
            this.v825.Name = "v825";
            this.v825.Size = new System.Drawing.Size(24, 21);
            this.v825.TabIndex = 825;
            this.v825.Text = "0";
            this.v825.TextAlign = HorizontalAlignment.Center;
            // 
            // v822
            // 
            this.v822.BorderStyle = BorderStyle.FixedSingle;
            this.v822.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v822.Location = new System.Drawing.Point(341, 239);
            this.v822.Name = "v822";
            this.v822.Size = new System.Drawing.Size(24, 21);
            this.v822.TabIndex = 822;
            this.v822.Text = "0";
            this.v822.TextAlign = HorizontalAlignment.Center;
            // 
            // v823
            // 
            this.v823.BorderStyle = BorderStyle.FixedSingle;
            this.v823.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v823.Location = new System.Drawing.Point(366, 239);
            this.v823.Name = "v823";
            this.v823.Size = new System.Drawing.Size(24, 21);
            this.v823.TabIndex = 823;
            this.v823.Text = "0";
            this.v823.TextAlign = HorizontalAlignment.Center;
            // 
            // v821
            // 
            this.v821.BorderStyle = BorderStyle.FixedSingle;
            this.v821.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v821.Location = new System.Drawing.Point(316, 239);
            this.v821.Name = "v821";
            this.v821.Size = new System.Drawing.Size(24, 21);
            this.v821.TabIndex = 821;
            this.v821.Text = "0";
            this.v821.TextAlign = HorizontalAlignment.Center;
            // 
            // v828
            // 
            this.v828.BorderStyle = BorderStyle.FixedSingle;
            this.v828.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v828.Location = new System.Drawing.Point(491, 239);
            this.v828.Name = "v828";
            this.v828.Size = new System.Drawing.Size(24, 21);
            this.v828.TabIndex = 828;
            this.v828.Text = "0";
            this.v828.TextAlign = HorizontalAlignment.Center;
            // 
            // v829
            // 
            this.v829.BorderStyle = BorderStyle.FixedSingle;
            this.v829.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v829.Location = new System.Drawing.Point(516, 239);
            this.v829.Name = "v829";
            this.v829.Size = new System.Drawing.Size(24, 21);
            this.v829.TabIndex = 829;
            this.v829.Text = "0";
            this.v829.TextAlign = HorizontalAlignment.Center;
            // 
            // v529
            // 
            this.v529.BorderStyle = BorderStyle.FixedSingle;
            this.v529.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v529.Location = new System.Drawing.Point(516, 173);
            this.v529.Name = "v529";
            this.v529.Size = new System.Drawing.Size(24, 21);
            this.v529.TabIndex = 529;
            this.v529.Text = "0";
            this.v529.TextAlign = HorizontalAlignment.Center;
            // 
            // v528
            // 
            this.v528.BorderStyle = BorderStyle.FixedSingle;
            this.v528.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v528.Location = new System.Drawing.Point(491, 173);
            this.v528.Name = "v528";
            this.v528.Size = new System.Drawing.Size(24, 21);
            this.v528.TabIndex = 528;
            this.v528.Text = "0";
            this.v528.TextAlign = HorizontalAlignment.Center;
            // 
            // v523
            // 
            this.v523.BorderStyle = BorderStyle.FixedSingle;
            this.v523.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v523.Location = new System.Drawing.Point(366, 173);
            this.v523.Name = "v523";
            this.v523.Size = new System.Drawing.Size(24, 21);
            this.v523.TabIndex = 523;
            this.v523.Text = "0";
            this.v523.TextAlign = HorizontalAlignment.Center;
            // 
            // v522
            // 
            this.v522.BorderStyle = BorderStyle.FixedSingle;
            this.v522.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v522.Location = new System.Drawing.Point(341, 173);
            this.v522.Name = "v522";
            this.v522.Size = new System.Drawing.Size(24, 21);
            this.v522.TabIndex = 522;
            this.v522.Text = "0";
            this.v522.TextAlign = HorizontalAlignment.Center;
            // 
            // v521
            // 
            this.v521.BorderStyle = BorderStyle.FixedSingle;
            this.v521.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v521.Location = new System.Drawing.Point(316, 173);
            this.v521.Name = "v521";
            this.v521.Size = new System.Drawing.Size(24, 21);
            this.v521.TabIndex = 521;
            this.v521.Text = "0";
            this.v521.TextAlign = HorizontalAlignment.Center;
            // 
            // v915
            // 
            this.v915.BackColor = System.Drawing.Color.Khaki;
            this.v915.BorderStyle = BorderStyle.FixedSingle;
            this.v915.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v915.Location = new System.Drawing.Point(188, 261);
            this.v915.Name = "v915";
            this.v915.Size = new System.Drawing.Size(24, 21);
            this.v915.TabIndex = 915;
            this.v915.Text = "0";
            this.v915.TextAlign = HorizontalAlignment.Center;
            // 
            // v527
            // 
            this.v527.BorderStyle = BorderStyle.FixedSingle;
            this.v527.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v527.Location = new System.Drawing.Point(466, 173);
            this.v527.Name = "v527";
            this.v527.Size = new System.Drawing.Size(24, 21);
            this.v527.TabIndex = 527;
            this.v527.Text = "0";
            this.v527.TextAlign = HorizontalAlignment.Center;
            // 
            // v526
            // 
            this.v526.BorderStyle = BorderStyle.FixedSingle;
            this.v526.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v526.Location = new System.Drawing.Point(441, 173);
            this.v526.Name = "v526";
            this.v526.Size = new System.Drawing.Size(24, 21);
            this.v526.TabIndex = 526;
            this.v526.Text = "0";
            this.v526.TextAlign = HorizontalAlignment.Center;
            // 
            // v525
            // 
            this.v525.BorderStyle = BorderStyle.FixedSingle;
            this.v525.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v525.Location = new System.Drawing.Point(416, 173);
            this.v525.Name = "v525";
            this.v525.Size = new System.Drawing.Size(24, 21);
            this.v525.TabIndex = 525;
            this.v525.Text = "0";
            this.v525.TextAlign = HorizontalAlignment.Center;
            // 
            // v524
            // 
            this.v524.BorderStyle = BorderStyle.FixedSingle;
            this.v524.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v524.Location = new System.Drawing.Point(391, 173);
            this.v524.Name = "v524";
            this.v524.Size = new System.Drawing.Size(24, 21);
            this.v524.TabIndex = 524;
            this.v524.Text = "0";
            this.v524.TextAlign = HorizontalAlignment.Center;
            // 
            // bGrabar
            // 
            this.bGrabar.BackColor = System.Drawing.Color.DarkSalmon;
            this.bGrabar.FlatStyle = FlatStyle.Popup;
            this.bGrabar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bGrabar.Image = ((System.Drawing.Image)(resources.GetObject("bGrabar.Image")));
            this.bGrabar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bGrabar.Location = new System.Drawing.Point(330, 490);
            this.bGrabar.Name = "bGrabar";
            this.bGrabar.Size = new System.Drawing.Size(137, 32);
            this.bGrabar.TabIndex = 6;
            this.bGrabar.Text = "Grabar Resultado";
            this.bGrabar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bGrabar.UseVisualStyleBackColor = false;
            this.bGrabar.Click += new System.EventHandler(this.BGrabarClick);
            // 
            // v919
            // 
            this.v919.BackColor = System.Drawing.Color.Khaki;
            this.v919.BorderStyle = BorderStyle.FixedSingle;
            this.v919.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v919.Location = new System.Drawing.Point(288, 261);
            this.v919.Name = "v919";
            this.v919.Size = new System.Drawing.Size(24, 21);
            this.v919.TabIndex = 919;
            this.v919.Text = "0";
            this.v919.TextAlign = HorizontalAlignment.Center;
            // 
            // c22
            // 
            this.c22.BorderStyle = BorderStyle.FixedSingle;
            this.c22.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.c22.ForeColor = System.Drawing.Color.Black;
            this.c22.Location = new System.Drawing.Point(82, 95);
            this.c22.Name = "c22";
            this.c22.Size = new System.Drawing.Size(40, 21);
            this.c22.TabIndex = 4;
            this.c22.Text = "270";
            this.c22.TextAlign = HorizontalAlignment.Center;
            // 
            // c21
            // 
            this.c21.BorderStyle = BorderStyle.FixedSingle;
            this.c21.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.c21.ForeColor = System.Drawing.Color.Black;
            this.c21.Location = new System.Drawing.Point(41, 95);
            this.c21.Name = "c21";
            this.c21.Size = new System.Drawing.Size(40, 21);
            this.c21.TabIndex = 3;
            this.c21.Text = "0";
            this.c21.TextAlign = HorizontalAlignment.Center;
            // 
            // label13
            // 
            this.label13.BorderStyle = BorderStyle.FixedSingle;
            this.label13.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.Color.Black;
            this.label13.Location = new System.Drawing.Point(123, 48);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(32, 24);
            this.label13.TabIndex = 70;
            this.label13.Text = "rdo";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bCalcular
            // 
            this.bCalcular.BackColor = System.Drawing.Color.DarkSalmon;
            this.bCalcular.FlatStyle = FlatStyle.Popup;
            this.bCalcular.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bCalcular.Image = ((System.Drawing.Image)(resources.GetObject("bCalcular.Image")));
            this.bCalcular.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bCalcular.Location = new System.Drawing.Point(330, 382);
            this.bCalcular.Name = "bCalcular";
            this.bCalcular.Size = new System.Drawing.Size(137, 32);
            this.bCalcular.TabIndex = 4;
            this.bCalcular.Text = "Calcular";
            this.bCalcular.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bCalcular.UseVisualStyleBackColor = false;
            this.bCalcular.Click += new System.EventHandler(this.BCalcularClick);
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Bisque;
            this.groupBox1.Controls.Add(this.vb39);
            this.groupBox1.Controls.Add(this.vb38);
            this.groupBox1.Controls.Add(this.vb37);
            this.groupBox1.Controls.Add(this.vb36);
            this.groupBox1.Controls.Add(this.vb35);
            this.groupBox1.Controls.Add(this.vb34);
            this.groupBox1.Controls.Add(this.vb33);
            this.groupBox1.Controls.Add(this.vb32);
            this.groupBox1.Controls.Add(this.vb31);
            this.groupBox1.Controls.Add(this.va39);
            this.groupBox1.Controls.Add(this.va38);
            this.groupBox1.Controls.Add(this.va37);
            this.groupBox1.Controls.Add(this.va36);
            this.groupBox1.Controls.Add(this.va35);
            this.groupBox1.Controls.Add(this.va34);
            this.groupBox1.Controls.Add(this.va33);
            this.groupBox1.Controls.Add(this.va32);
            this.groupBox1.Controls.Add(this.va31);
            this.groupBox1.Controls.Add(this.vb29);
            this.groupBox1.Controls.Add(this.vb28);
            this.groupBox1.Controls.Add(this.vb27);
            this.groupBox1.Controls.Add(this.vb26);
            this.groupBox1.Controls.Add(this.vb25);
            this.groupBox1.Controls.Add(this.vb24);
            this.groupBox1.Controls.Add(this.vb23);
            this.groupBox1.Controls.Add(this.vb22);
            this.groupBox1.Controls.Add(this.vb21);
            this.groupBox1.Controls.Add(this.va29);
            this.groupBox1.Controls.Add(this.va28);
            this.groupBox1.Controls.Add(this.va27);
            this.groupBox1.Controls.Add(this.va26);
            this.groupBox1.Controls.Add(this.va25);
            this.groupBox1.Controls.Add(this.va24);
            this.groupBox1.Controls.Add(this.va23);
            this.groupBox1.Controls.Add(this.va22);
            this.groupBox1.Controls.Add(this.va21);
            this.groupBox1.Controls.Add(this.pb3);
            this.groupBox1.Controls.Add(this.pa3);
            this.groupBox1.Controls.Add(this.vb19);
            this.groupBox1.Controls.Add(this.vb18);
            this.groupBox1.Controls.Add(this.vb17);
            this.groupBox1.Controls.Add(this.vb16);
            this.groupBox1.Controls.Add(this.vb15);
            this.groupBox1.Controls.Add(this.vb14);
            this.groupBox1.Controls.Add(this.vb13);
            this.groupBox1.Controls.Add(this.vb12);
            this.groupBox1.Controls.Add(this.vb11);
            this.groupBox1.Controls.Add(this.pb2);
            this.groupBox1.Controls.Add(this.pb1);
            this.groupBox1.Controls.Add(this.va19);
            this.groupBox1.Controls.Add(this.va18);
            this.groupBox1.Controls.Add(this.va17);
            this.groupBox1.Controls.Add(this.va16);
            this.groupBox1.Controls.Add(this.va15);
            this.groupBox1.Controls.Add(this.va14);
            this.groupBox1.Controls.Add(this.va13);
            this.groupBox1.Controls.Add(this.va12);
            this.groupBox1.Controls.Add(this.va11);
            this.groupBox1.Controls.Add(this.pa2);
            this.groupBox1.Controls.Add(this.pa1);
            this.groupBox1.Controls.Add(this.label42);
            this.groupBox1.Controls.Add(this.label41);
            this.groupBox1.Controls.Add(this.label40);
            this.groupBox1.Controls.Add(this.v939);
            this.groupBox1.Controls.Add(this.v938);
            this.groupBox1.Controls.Add(this.v937);
            this.groupBox1.Controls.Add(this.v936);
            this.groupBox1.Controls.Add(this.v935);
            this.groupBox1.Controls.Add(this.v934);
            this.groupBox1.Controls.Add(this.v933);
            this.groupBox1.Controls.Add(this.v932);
            this.groupBox1.Controls.Add(this.v931);
            this.groupBox1.Controls.Add(this.v839);
            this.groupBox1.Controls.Add(this.v838);
            this.groupBox1.Controls.Add(this.v837);
            this.groupBox1.Controls.Add(this.v836);
            this.groupBox1.Controls.Add(this.v835);
            this.groupBox1.Controls.Add(this.v834);
            this.groupBox1.Controls.Add(this.v833);
            this.groupBox1.Controls.Add(this.v832);
            this.groupBox1.Controls.Add(this.v831);
            this.groupBox1.Controls.Add(this.v739);
            this.groupBox1.Controls.Add(this.v738);
            this.groupBox1.Controls.Add(this.v737);
            this.groupBox1.Controls.Add(this.v736);
            this.groupBox1.Controls.Add(this.v735);
            this.groupBox1.Controls.Add(this.v734);
            this.groupBox1.Controls.Add(this.v733);
            this.groupBox1.Controls.Add(this.v732);
            this.groupBox1.Controls.Add(this.v731);
            this.groupBox1.Controls.Add(this.v639);
            this.groupBox1.Controls.Add(this.v638);
            this.groupBox1.Controls.Add(this.v637);
            this.groupBox1.Controls.Add(this.v636);
            this.groupBox1.Controls.Add(this.v635);
            this.groupBox1.Controls.Add(this.v634);
            this.groupBox1.Controls.Add(this.v633);
            this.groupBox1.Controls.Add(this.v632);
            this.groupBox1.Controls.Add(this.v631);
            this.groupBox1.Controls.Add(this.v539);
            this.groupBox1.Controls.Add(this.v538);
            this.groupBox1.Controls.Add(this.v537);
            this.groupBox1.Controls.Add(this.v536);
            this.groupBox1.Controls.Add(this.v535);
            this.groupBox1.Controls.Add(this.v534);
            this.groupBox1.Controls.Add(this.v533);
            this.groupBox1.Controls.Add(this.v532);
            this.groupBox1.Controls.Add(this.v531);
            this.groupBox1.Controls.Add(this.v439);
            this.groupBox1.Controls.Add(this.v438);
            this.groupBox1.Controls.Add(this.v437);
            this.groupBox1.Controls.Add(this.v436);
            this.groupBox1.Controls.Add(this.v435);
            this.groupBox1.Controls.Add(this.v434);
            this.groupBox1.Controls.Add(this.v433);
            this.groupBox1.Controls.Add(this.v432);
            this.groupBox1.Controls.Add(this.v431);
            this.groupBox1.Controls.Add(this.v339);
            this.groupBox1.Controls.Add(this.v338);
            this.groupBox1.Controls.Add(this.v337);
            this.groupBox1.Controls.Add(this.v336);
            this.groupBox1.Controls.Add(this.v335);
            this.groupBox1.Controls.Add(this.v334);
            this.groupBox1.Controls.Add(this.v333);
            this.groupBox1.Controls.Add(this.v332);
            this.groupBox1.Controls.Add(this.v331);
            this.groupBox1.Controls.Add(this.v239);
            this.groupBox1.Controls.Add(this.v238);
            this.groupBox1.Controls.Add(this.v237);
            this.groupBox1.Controls.Add(this.v236);
            this.groupBox1.Controls.Add(this.v235);
            this.groupBox1.Controls.Add(this.v234);
            this.groupBox1.Controls.Add(this.v233);
            this.groupBox1.Controls.Add(this.v232);
            this.groupBox1.Controls.Add(this.v231);
            this.groupBox1.Controls.Add(this.v139);
            this.groupBox1.Controls.Add(this.v138);
            this.groupBox1.Controls.Add(this.v137);
            this.groupBox1.Controls.Add(this.v136);
            this.groupBox1.Controls.Add(this.v135);
            this.groupBox1.Controls.Add(this.v134);
            this.groupBox1.Controls.Add(this.v133);
            this.groupBox1.Controls.Add(this.v132);
            this.groupBox1.Controls.Add(this.v131);
            this.groupBox1.Controls.Add(this.v039);
            this.groupBox1.Controls.Add(this.v038);
            this.groupBox1.Controls.Add(this.v037);
            this.groupBox1.Controls.Add(this.v036);
            this.groupBox1.Controls.Add(this.v035);
            this.groupBox1.Controls.Add(this.v034);
            this.groupBox1.Controls.Add(this.v033);
            this.groupBox1.Controls.Add(this.v032);
            this.groupBox1.Controls.Add(this.v031);
            this.groupBox1.Controls.Add(this.label31);
            this.groupBox1.Controls.Add(this.label32);
            this.groupBox1.Controls.Add(this.label33);
            this.groupBox1.Controls.Add(this.label34);
            this.groupBox1.Controls.Add(this.label35);
            this.groupBox1.Controls.Add(this.label36);
            this.groupBox1.Controls.Add(this.label37);
            this.groupBox1.Controls.Add(this.label38);
            this.groupBox1.Controls.Add(this.label39);
            this.groupBox1.Controls.Add(this.v929);
            this.groupBox1.Controls.Add(this.v928);
            this.groupBox1.Controls.Add(this.v927);
            this.groupBox1.Controls.Add(this.v926);
            this.groupBox1.Controls.Add(this.v925);
            this.groupBox1.Controls.Add(this.v924);
            this.groupBox1.Controls.Add(this.v923);
            this.groupBox1.Controls.Add(this.v922);
            this.groupBox1.Controls.Add(this.v921);
            this.groupBox1.Controls.Add(this.v829);
            this.groupBox1.Controls.Add(this.v828);
            this.groupBox1.Controls.Add(this.v827);
            this.groupBox1.Controls.Add(this.v826);
            this.groupBox1.Controls.Add(this.v825);
            this.groupBox1.Controls.Add(this.v824);
            this.groupBox1.Controls.Add(this.v823);
            this.groupBox1.Controls.Add(this.v822);
            this.groupBox1.Controls.Add(this.v821);
            this.groupBox1.Controls.Add(this.v729);
            this.groupBox1.Controls.Add(this.v728);
            this.groupBox1.Controls.Add(this.v727);
            this.groupBox1.Controls.Add(this.v726);
            this.groupBox1.Controls.Add(this.v725);
            this.groupBox1.Controls.Add(this.v724);
            this.groupBox1.Controls.Add(this.v723);
            this.groupBox1.Controls.Add(this.v722);
            this.groupBox1.Controls.Add(this.v721);
            this.groupBox1.Controls.Add(this.v629);
            this.groupBox1.Controls.Add(this.v628);
            this.groupBox1.Controls.Add(this.v627);
            this.groupBox1.Controls.Add(this.v626);
            this.groupBox1.Controls.Add(this.v625);
            this.groupBox1.Controls.Add(this.v624);
            this.groupBox1.Controls.Add(this.v623);
            this.groupBox1.Controls.Add(this.v622);
            this.groupBox1.Controls.Add(this.v621);
            this.groupBox1.Controls.Add(this.v529);
            this.groupBox1.Controls.Add(this.v528);
            this.groupBox1.Controls.Add(this.v527);
            this.groupBox1.Controls.Add(this.v526);
            this.groupBox1.Controls.Add(this.v525);
            this.groupBox1.Controls.Add(this.v524);
            this.groupBox1.Controls.Add(this.v523);
            this.groupBox1.Controls.Add(this.v522);
            this.groupBox1.Controls.Add(this.v521);
            this.groupBox1.Controls.Add(this.v429);
            this.groupBox1.Controls.Add(this.v428);
            this.groupBox1.Controls.Add(this.v427);
            this.groupBox1.Controls.Add(this.v426);
            this.groupBox1.Controls.Add(this.v425);
            this.groupBox1.Controls.Add(this.v424);
            this.groupBox1.Controls.Add(this.v423);
            this.groupBox1.Controls.Add(this.v422);
            this.groupBox1.Controls.Add(this.v421);
            this.groupBox1.Controls.Add(this.v329);
            this.groupBox1.Controls.Add(this.v328);
            this.groupBox1.Controls.Add(this.v327);
            this.groupBox1.Controls.Add(this.v326);
            this.groupBox1.Controls.Add(this.v325);
            this.groupBox1.Controls.Add(this.v324);
            this.groupBox1.Controls.Add(this.v323);
            this.groupBox1.Controls.Add(this.v322);
            this.groupBox1.Controls.Add(this.v321);
            this.groupBox1.Controls.Add(this.v229);
            this.groupBox1.Controls.Add(this.v228);
            this.groupBox1.Controls.Add(this.v227);
            this.groupBox1.Controls.Add(this.v226);
            this.groupBox1.Controls.Add(this.v225);
            this.groupBox1.Controls.Add(this.v224);
            this.groupBox1.Controls.Add(this.v223);
            this.groupBox1.Controls.Add(this.v222);
            this.groupBox1.Controls.Add(this.v221);
            this.groupBox1.Controls.Add(this.v129);
            this.groupBox1.Controls.Add(this.v128);
            this.groupBox1.Controls.Add(this.v127);
            this.groupBox1.Controls.Add(this.v126);
            this.groupBox1.Controls.Add(this.v125);
            this.groupBox1.Controls.Add(this.v124);
            this.groupBox1.Controls.Add(this.v123);
            this.groupBox1.Controls.Add(this.v122);
            this.groupBox1.Controls.Add(this.v121);
            this.groupBox1.Controls.Add(this.v029);
            this.groupBox1.Controls.Add(this.v028);
            this.groupBox1.Controls.Add(this.v027);
            this.groupBox1.Controls.Add(this.v026);
            this.groupBox1.Controls.Add(this.v025);
            this.groupBox1.Controls.Add(this.v024);
            this.groupBox1.Controls.Add(this.v023);
            this.groupBox1.Controls.Add(this.v022);
            this.groupBox1.Controls.Add(this.v021);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.label20);
            this.groupBox1.Controls.Add(this.label22);
            this.groupBox1.Controls.Add(this.label24);
            this.groupBox1.Controls.Add(this.label26);
            this.groupBox1.Controls.Add(this.label27);
            this.groupBox1.Controls.Add(this.label28);
            this.groupBox1.Controls.Add(this.label29);
            this.groupBox1.Controls.Add(this.p93);
            this.groupBox1.Controls.Add(this.p83);
            this.groupBox1.Controls.Add(this.p73);
            this.groupBox1.Controls.Add(this.p63);
            this.groupBox1.Controls.Add(this.p53);
            this.groupBox1.Controls.Add(this.p43);
            this.groupBox1.Controls.Add(this.p33);
            this.groupBox1.Controls.Add(this.p23);
            this.groupBox1.Controls.Add(this.p13);
            this.groupBox1.Controls.Add(this.p03);
            this.groupBox1.Controls.Add(this.v919);
            this.groupBox1.Controls.Add(this.v918);
            this.groupBox1.Controls.Add(this.v917);
            this.groupBox1.Controls.Add(this.v916);
            this.groupBox1.Controls.Add(this.v915);
            this.groupBox1.Controls.Add(this.v914);
            this.groupBox1.Controls.Add(this.v913);
            this.groupBox1.Controls.Add(this.v912);
            this.groupBox1.Controls.Add(this.v911);
            this.groupBox1.Controls.Add(this.p92);
            this.groupBox1.Controls.Add(this.p91);
            this.groupBox1.Controls.Add(this.v819);
            this.groupBox1.Controls.Add(this.v818);
            this.groupBox1.Controls.Add(this.v817);
            this.groupBox1.Controls.Add(this.v816);
            this.groupBox1.Controls.Add(this.v815);
            this.groupBox1.Controls.Add(this.v814);
            this.groupBox1.Controls.Add(this.v813);
            this.groupBox1.Controls.Add(this.v812);
            this.groupBox1.Controls.Add(this.v811);
            this.groupBox1.Controls.Add(this.p82);
            this.groupBox1.Controls.Add(this.p81);
            this.groupBox1.Controls.Add(this.v719);
            this.groupBox1.Controls.Add(this.v718);
            this.groupBox1.Controls.Add(this.v717);
            this.groupBox1.Controls.Add(this.v716);
            this.groupBox1.Controls.Add(this.v715);
            this.groupBox1.Controls.Add(this.v714);
            this.groupBox1.Controls.Add(this.v713);
            this.groupBox1.Controls.Add(this.v712);
            this.groupBox1.Controls.Add(this.v711);
            this.groupBox1.Controls.Add(this.p72);
            this.groupBox1.Controls.Add(this.p71);
            this.groupBox1.Controls.Add(this.v619);
            this.groupBox1.Controls.Add(this.v618);
            this.groupBox1.Controls.Add(this.v617);
            this.groupBox1.Controls.Add(this.v616);
            this.groupBox1.Controls.Add(this.v615);
            this.groupBox1.Controls.Add(this.v614);
            this.groupBox1.Controls.Add(this.v613);
            this.groupBox1.Controls.Add(this.v612);
            this.groupBox1.Controls.Add(this.v611);
            this.groupBox1.Controls.Add(this.p62);
            this.groupBox1.Controls.Add(this.p61);
            this.groupBox1.Controls.Add(this.v519);
            this.groupBox1.Controls.Add(this.v518);
            this.groupBox1.Controls.Add(this.v517);
            this.groupBox1.Controls.Add(this.v516);
            this.groupBox1.Controls.Add(this.v515);
            this.groupBox1.Controls.Add(this.v514);
            this.groupBox1.Controls.Add(this.v513);
            this.groupBox1.Controls.Add(this.v512);
            this.groupBox1.Controls.Add(this.v511);
            this.groupBox1.Controls.Add(this.p52);
            this.groupBox1.Controls.Add(this.p51);
            this.groupBox1.Controls.Add(this.v419);
            this.groupBox1.Controls.Add(this.v418);
            this.groupBox1.Controls.Add(this.v417);
            this.groupBox1.Controls.Add(this.v416);
            this.groupBox1.Controls.Add(this.v415);
            this.groupBox1.Controls.Add(this.v414);
            this.groupBox1.Controls.Add(this.v413);
            this.groupBox1.Controls.Add(this.v412);
            this.groupBox1.Controls.Add(this.v411);
            this.groupBox1.Controls.Add(this.p42);
            this.groupBox1.Controls.Add(this.p41);
            this.groupBox1.Controls.Add(this.v319);
            this.groupBox1.Controls.Add(this.v318);
            this.groupBox1.Controls.Add(this.v317);
            this.groupBox1.Controls.Add(this.v316);
            this.groupBox1.Controls.Add(this.v315);
            this.groupBox1.Controls.Add(this.v314);
            this.groupBox1.Controls.Add(this.v313);
            this.groupBox1.Controls.Add(this.v312);
            this.groupBox1.Controls.Add(this.v311);
            this.groupBox1.Controls.Add(this.p32);
            this.groupBox1.Controls.Add(this.p31);
            this.groupBox1.Controls.Add(this.v219);
            this.groupBox1.Controls.Add(this.v218);
            this.groupBox1.Controls.Add(this.v217);
            this.groupBox1.Controls.Add(this.v216);
            this.groupBox1.Controls.Add(this.v215);
            this.groupBox1.Controls.Add(this.v214);
            this.groupBox1.Controls.Add(this.v213);
            this.groupBox1.Controls.Add(this.v212);
            this.groupBox1.Controls.Add(this.v211);
            this.groupBox1.Controls.Add(this.p22);
            this.groupBox1.Controls.Add(this.p21);
            this.groupBox1.Controls.Add(this.v119);
            this.groupBox1.Controls.Add(this.v118);
            this.groupBox1.Controls.Add(this.v117);
            this.groupBox1.Controls.Add(this.v116);
            this.groupBox1.Controls.Add(this.v115);
            this.groupBox1.Controls.Add(this.v114);
            this.groupBox1.Controls.Add(this.v113);
            this.groupBox1.Controls.Add(this.v112);
            this.groupBox1.Controls.Add(this.v111);
            this.groupBox1.Controls.Add(this.p12);
            this.groupBox1.Controls.Add(this.p11);
            this.groupBox1.Controls.Add(this.v019);
            this.groupBox1.Controls.Add(this.v018);
            this.groupBox1.Controls.Add(this.v017);
            this.groupBox1.Controls.Add(this.v016);
            this.groupBox1.Controls.Add(this.v015);
            this.groupBox1.Controls.Add(this.v014);
            this.groupBox1.Controls.Add(this.v013);
            this.groupBox1.Controls.Add(this.v012);
            this.groupBox1.Controls.Add(this.v011);
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
            this.groupBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox1.Location = new System.Drawing.Point(2, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(797, 352);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Niveles";
            // 
            // vb39
            // 
            this.vb39.BackColor = System.Drawing.Color.Khaki;
            this.vb39.BorderStyle = BorderStyle.FixedSingle;
            this.vb39.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vb39.Location = new System.Drawing.Point(744, 305);
            this.vb39.Name = "vb39";
            this.vb39.Size = new System.Drawing.Size(24, 21);
            this.vb39.TabIndex = 1042;
            this.vb39.Text = "0";
            this.vb39.TextAlign = HorizontalAlignment.Center;
            // 
            // vb38
            // 
            this.vb38.BackColor = System.Drawing.Color.Khaki;
            this.vb38.BorderStyle = BorderStyle.FixedSingle;
            this.vb38.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vb38.Location = new System.Drawing.Point(719, 305);
            this.vb38.Name = "vb38";
            this.vb38.Size = new System.Drawing.Size(24, 21);
            this.vb38.TabIndex = 1041;
            this.vb38.Text = "0";
            this.vb38.TextAlign = HorizontalAlignment.Center;
            // 
            // vb37
            // 
            this.vb37.BackColor = System.Drawing.Color.Khaki;
            this.vb37.BorderStyle = BorderStyle.FixedSingle;
            this.vb37.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vb37.Location = new System.Drawing.Point(694, 305);
            this.vb37.Name = "vb37";
            this.vb37.Size = new System.Drawing.Size(24, 21);
            this.vb37.TabIndex = 1040;
            this.vb37.Text = "0";
            this.vb37.TextAlign = HorizontalAlignment.Center;
            // 
            // vb36
            // 
            this.vb36.BackColor = System.Drawing.Color.Khaki;
            this.vb36.BorderStyle = BorderStyle.FixedSingle;
            this.vb36.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vb36.Location = new System.Drawing.Point(669, 305);
            this.vb36.Name = "vb36";
            this.vb36.Size = new System.Drawing.Size(24, 21);
            this.vb36.TabIndex = 1039;
            this.vb36.Text = "0";
            this.vb36.TextAlign = HorizontalAlignment.Center;
            // 
            // vb35
            // 
            this.vb35.BackColor = System.Drawing.Color.Khaki;
            this.vb35.BorderStyle = BorderStyle.FixedSingle;
            this.vb35.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vb35.Location = new System.Drawing.Point(644, 305);
            this.vb35.Name = "vb35";
            this.vb35.Size = new System.Drawing.Size(24, 21);
            this.vb35.TabIndex = 1038;
            this.vb35.Text = "0";
            this.vb35.TextAlign = HorizontalAlignment.Center;
            // 
            // vb34
            // 
            this.vb34.BackColor = System.Drawing.Color.Khaki;
            this.vb34.BorderStyle = BorderStyle.FixedSingle;
            this.vb34.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vb34.Location = new System.Drawing.Point(619, 305);
            this.vb34.Name = "vb34";
            this.vb34.Size = new System.Drawing.Size(24, 21);
            this.vb34.TabIndex = 1037;
            this.vb34.Text = "0";
            this.vb34.TextAlign = HorizontalAlignment.Center;
            // 
            // vb33
            // 
            this.vb33.BackColor = System.Drawing.Color.Khaki;
            this.vb33.BorderStyle = BorderStyle.FixedSingle;
            this.vb33.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vb33.Location = new System.Drawing.Point(594, 305);
            this.vb33.Name = "vb33";
            this.vb33.Size = new System.Drawing.Size(24, 21);
            this.vb33.TabIndex = 1036;
            this.vb33.Text = "0";
            this.vb33.TextAlign = HorizontalAlignment.Center;
            // 
            // vb32
            // 
            this.vb32.BackColor = System.Drawing.Color.Khaki;
            this.vb32.BorderStyle = BorderStyle.FixedSingle;
            this.vb32.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vb32.Location = new System.Drawing.Point(569, 305);
            this.vb32.Name = "vb32";
            this.vb32.Size = new System.Drawing.Size(24, 21);
            this.vb32.TabIndex = 1035;
            this.vb32.Text = "0";
            this.vb32.TextAlign = HorizontalAlignment.Center;
            // 
            // vb31
            // 
            this.vb31.BackColor = System.Drawing.Color.Khaki;
            this.vb31.BorderStyle = BorderStyle.FixedSingle;
            this.vb31.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vb31.Location = new System.Drawing.Point(544, 305);
            this.vb31.Name = "vb31";
            this.vb31.Size = new System.Drawing.Size(24, 21);
            this.vb31.TabIndex = 1034;
            this.vb31.Text = "0";
            this.vb31.TextAlign = HorizontalAlignment.Center;
            // 
            // va39
            // 
            this.va39.BackColor = System.Drawing.Color.Khaki;
            this.va39.BorderStyle = BorderStyle.FixedSingle;
            this.va39.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.va39.Location = new System.Drawing.Point(744, 283);
            this.va39.Name = "va39";
            this.va39.Size = new System.Drawing.Size(24, 21);
            this.va39.TabIndex = 1012;
            this.va39.Text = "0";
            this.va39.TextAlign = HorizontalAlignment.Center;
            // 
            // va38
            // 
            this.va38.BackColor = System.Drawing.Color.Khaki;
            this.va38.BorderStyle = BorderStyle.FixedSingle;
            this.va38.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.va38.Location = new System.Drawing.Point(719, 283);
            this.va38.Name = "va38";
            this.va38.Size = new System.Drawing.Size(24, 21);
            this.va38.TabIndex = 1011;
            this.va38.Text = "0";
            this.va38.TextAlign = HorizontalAlignment.Center;
            // 
            // va37
            // 
            this.va37.BackColor = System.Drawing.Color.Khaki;
            this.va37.BorderStyle = BorderStyle.FixedSingle;
            this.va37.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.va37.Location = new System.Drawing.Point(694, 283);
            this.va37.Name = "va37";
            this.va37.Size = new System.Drawing.Size(24, 21);
            this.va37.TabIndex = 1010;
            this.va37.Text = "0";
            this.va37.TextAlign = HorizontalAlignment.Center;
            // 
            // va36
            // 
            this.va36.BackColor = System.Drawing.Color.Khaki;
            this.va36.BorderStyle = BorderStyle.FixedSingle;
            this.va36.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.va36.Location = new System.Drawing.Point(669, 283);
            this.va36.Name = "va36";
            this.va36.Size = new System.Drawing.Size(24, 21);
            this.va36.TabIndex = 1009;
            this.va36.Text = "0";
            this.va36.TextAlign = HorizontalAlignment.Center;
            // 
            // va35
            // 
            this.va35.BackColor = System.Drawing.Color.Khaki;
            this.va35.BorderStyle = BorderStyle.FixedSingle;
            this.va35.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.va35.Location = new System.Drawing.Point(644, 283);
            this.va35.Name = "va35";
            this.va35.Size = new System.Drawing.Size(24, 21);
            this.va35.TabIndex = 1008;
            this.va35.Text = "0";
            this.va35.TextAlign = HorizontalAlignment.Center;
            // 
            // va34
            // 
            this.va34.BackColor = System.Drawing.Color.Khaki;
            this.va34.BorderStyle = BorderStyle.FixedSingle;
            this.va34.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.va34.Location = new System.Drawing.Point(619, 283);
            this.va34.Name = "va34";
            this.va34.Size = new System.Drawing.Size(24, 21);
            this.va34.TabIndex = 1007;
            this.va34.Text = "0";
            this.va34.TextAlign = HorizontalAlignment.Center;
            // 
            // va33
            // 
            this.va33.BackColor = System.Drawing.Color.Khaki;
            this.va33.BorderStyle = BorderStyle.FixedSingle;
            this.va33.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.va33.Location = new System.Drawing.Point(594, 283);
            this.va33.Name = "va33";
            this.va33.Size = new System.Drawing.Size(24, 21);
            this.va33.TabIndex = 1006;
            this.va33.Text = "0";
            this.va33.TextAlign = HorizontalAlignment.Center;
            // 
            // va32
            // 
            this.va32.BackColor = System.Drawing.Color.Khaki;
            this.va32.BorderStyle = BorderStyle.FixedSingle;
            this.va32.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.va32.Location = new System.Drawing.Point(569, 283);
            this.va32.Name = "va32";
            this.va32.Size = new System.Drawing.Size(24, 21);
            this.va32.TabIndex = 1005;
            this.va32.Text = "0";
            this.va32.TextAlign = HorizontalAlignment.Center;
            // 
            // va31
            // 
            this.va31.BackColor = System.Drawing.Color.Khaki;
            this.va31.BorderStyle = BorderStyle.FixedSingle;
            this.va31.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.va31.Location = new System.Drawing.Point(544, 283);
            this.va31.Name = "va31";
            this.va31.Size = new System.Drawing.Size(24, 21);
            this.va31.TabIndex = 1004;
            this.va31.Text = "0";
            this.va31.TextAlign = HorizontalAlignment.Center;
            // 
            // vb29
            // 
            this.vb29.BorderStyle = BorderStyle.FixedSingle;
            this.vb29.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vb29.Location = new System.Drawing.Point(516, 305);
            this.vb29.Name = "vb29";
            this.vb29.Size = new System.Drawing.Size(24, 21);
            this.vb29.TabIndex = 1033;
            this.vb29.Text = "0";
            this.vb29.TextAlign = HorizontalAlignment.Center;
            // 
            // vb28
            // 
            this.vb28.BorderStyle = BorderStyle.FixedSingle;
            this.vb28.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vb28.Location = new System.Drawing.Point(491, 305);
            this.vb28.Name = "vb28";
            this.vb28.Size = new System.Drawing.Size(24, 21);
            this.vb28.TabIndex = 1032;
            this.vb28.Text = "0";
            this.vb28.TextAlign = HorizontalAlignment.Center;
            // 
            // vb27
            // 
            this.vb27.BorderStyle = BorderStyle.FixedSingle;
            this.vb27.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vb27.Location = new System.Drawing.Point(466, 305);
            this.vb27.Name = "vb27";
            this.vb27.Size = new System.Drawing.Size(24, 21);
            this.vb27.TabIndex = 1031;
            this.vb27.Text = "0";
            this.vb27.TextAlign = HorizontalAlignment.Center;
            // 
            // vb26
            // 
            this.vb26.BorderStyle = BorderStyle.FixedSingle;
            this.vb26.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vb26.Location = new System.Drawing.Point(441, 305);
            this.vb26.Name = "vb26";
            this.vb26.Size = new System.Drawing.Size(24, 21);
            this.vb26.TabIndex = 1030;
            this.vb26.Text = "0";
            this.vb26.TextAlign = HorizontalAlignment.Center;
            // 
            // vb25
            // 
            this.vb25.BorderStyle = BorderStyle.FixedSingle;
            this.vb25.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vb25.Location = new System.Drawing.Point(416, 305);
            this.vb25.Name = "vb25";
            this.vb25.Size = new System.Drawing.Size(24, 21);
            this.vb25.TabIndex = 1029;
            this.vb25.Text = "0";
            this.vb25.TextAlign = HorizontalAlignment.Center;
            // 
            // vb24
            // 
            this.vb24.BorderStyle = BorderStyle.FixedSingle;
            this.vb24.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vb24.Location = new System.Drawing.Point(391, 305);
            this.vb24.Name = "vb24";
            this.vb24.Size = new System.Drawing.Size(24, 21);
            this.vb24.TabIndex = 1028;
            this.vb24.Text = "0";
            this.vb24.TextAlign = HorizontalAlignment.Center;
            // 
            // vb23
            // 
            this.vb23.BorderStyle = BorderStyle.FixedSingle;
            this.vb23.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vb23.Location = new System.Drawing.Point(366, 305);
            this.vb23.Name = "vb23";
            this.vb23.Size = new System.Drawing.Size(24, 21);
            this.vb23.TabIndex = 1027;
            this.vb23.Text = "0";
            this.vb23.TextAlign = HorizontalAlignment.Center;
            // 
            // vb22
            // 
            this.vb22.BorderStyle = BorderStyle.FixedSingle;
            this.vb22.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vb22.Location = new System.Drawing.Point(341, 305);
            this.vb22.Name = "vb22";
            this.vb22.Size = new System.Drawing.Size(24, 21);
            this.vb22.TabIndex = 1026;
            this.vb22.Text = "0";
            this.vb22.TextAlign = HorizontalAlignment.Center;
            // 
            // vb21
            // 
            this.vb21.BorderStyle = BorderStyle.FixedSingle;
            this.vb21.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vb21.Location = new System.Drawing.Point(316, 305);
            this.vb21.Name = "vb21";
            this.vb21.Size = new System.Drawing.Size(24, 21);
            this.vb21.TabIndex = 1025;
            this.vb21.Text = "0";
            this.vb21.TextAlign = HorizontalAlignment.Center;
            // 
            // va28
            // 
            this.va28.BorderStyle = BorderStyle.FixedSingle;
            this.va28.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.va28.Location = new System.Drawing.Point(491, 283);
            this.va28.Name = "va28";
            this.va28.Size = new System.Drawing.Size(24, 21);
            this.va28.TabIndex = 1002;
            this.va28.Text = "0";
            this.va28.TextAlign = HorizontalAlignment.Center;
            // 
            // va27
            // 
            this.va27.BorderStyle = BorderStyle.FixedSingle;
            this.va27.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.va27.Location = new System.Drawing.Point(466, 283);
            this.va27.Name = "va27";
            this.va27.Size = new System.Drawing.Size(24, 21);
            this.va27.TabIndex = 1001;
            this.va27.Text = "0";
            this.va27.TextAlign = HorizontalAlignment.Center;
            // 
            // va26
            // 
            this.va26.BorderStyle = BorderStyle.FixedSingle;
            this.va26.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.va26.Location = new System.Drawing.Point(441, 283);
            this.va26.Name = "va26";
            this.va26.Size = new System.Drawing.Size(24, 21);
            this.va26.TabIndex = 1000;
            this.va26.Text = "0";
            this.va26.TextAlign = HorizontalAlignment.Center;
            // 
            // va25
            // 
            this.va25.BorderStyle = BorderStyle.FixedSingle;
            this.va25.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.va25.Location = new System.Drawing.Point(416, 283);
            this.va25.Name = "va25";
            this.va25.Size = new System.Drawing.Size(24, 21);
            this.va25.TabIndex = 999;
            this.va25.Text = "0";
            this.va25.TextAlign = HorizontalAlignment.Center;
            // 
            // va24
            // 
            this.va24.BorderStyle = BorderStyle.FixedSingle;
            this.va24.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.va24.Location = new System.Drawing.Point(391, 283);
            this.va24.Name = "va24";
            this.va24.Size = new System.Drawing.Size(24, 21);
            this.va24.TabIndex = 998;
            this.va24.Text = "0";
            this.va24.TextAlign = HorizontalAlignment.Center;
            // 
            // va22
            // 
            this.va22.BorderStyle = BorderStyle.FixedSingle;
            this.va22.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.va22.Location = new System.Drawing.Point(341, 283);
            this.va22.Name = "va22";
            this.va22.Size = new System.Drawing.Size(24, 21);
            this.va22.TabIndex = 996;
            this.va22.Text = "0";
            this.va22.TextAlign = HorizontalAlignment.Center;
            // 
            // va21
            // 
            this.va21.BorderStyle = BorderStyle.FixedSingle;
            this.va21.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.va21.Location = new System.Drawing.Point(316, 283);
            this.va21.Name = "va21";
            this.va21.Size = new System.Drawing.Size(24, 21);
            this.va21.TabIndex = 995;
            this.va21.Text = "0";
            this.va21.TextAlign = HorizontalAlignment.Center;
            // 
            // pb3
            // 
            this.pb3.BackColor = System.Drawing.Color.LemonChiffon;
            this.pb3.BorderStyle = BorderStyle.FixedSingle;
            this.pb3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pb3.Location = new System.Drawing.Point(57, 304);
            this.pb3.Name = "pb3";
            this.pb3.Size = new System.Drawing.Size(24, 21);
            this.pb3.TabIndex = 1015;
            this.pb3.Text = "0";
            this.pb3.TextAlign = HorizontalAlignment.Center;
            // 
            // pa3
            // 
            this.pa3.BackColor = System.Drawing.Color.LemonChiffon;
            this.pa3.BorderStyle = BorderStyle.FixedSingle;
            this.pa3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pa3.Location = new System.Drawing.Point(57, 282);
            this.pa3.Name = "pa3";
            this.pa3.Size = new System.Drawing.Size(24, 21);
            this.pa3.TabIndex = 985;
            this.pa3.Text = "0";
            this.pa3.TextAlign = HorizontalAlignment.Center;
            // 
            // vb19
            // 
            this.vb19.BackColor = System.Drawing.Color.Khaki;
            this.vb19.BorderStyle = BorderStyle.FixedSingle;
            this.vb19.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vb19.Location = new System.Drawing.Point(288, 305);
            this.vb19.Name = "vb19";
            this.vb19.Size = new System.Drawing.Size(24, 21);
            this.vb19.TabIndex = 1024;
            this.vb19.Text = "0";
            this.vb19.TextAlign = HorizontalAlignment.Center;
            // 
            // vb18
            // 
            this.vb18.BackColor = System.Drawing.Color.Khaki;
            this.vb18.BorderStyle = BorderStyle.FixedSingle;
            this.vb18.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vb18.Location = new System.Drawing.Point(263, 305);
            this.vb18.Name = "vb18";
            this.vb18.Size = new System.Drawing.Size(24, 21);
            this.vb18.TabIndex = 1023;
            this.vb18.Text = "0";
            this.vb18.TextAlign = HorizontalAlignment.Center;
            // 
            // vb17
            // 
            this.vb17.BackColor = System.Drawing.Color.Khaki;
            this.vb17.BorderStyle = BorderStyle.FixedSingle;
            this.vb17.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vb17.Location = new System.Drawing.Point(238, 305);
            this.vb17.Name = "vb17";
            this.vb17.Size = new System.Drawing.Size(24, 21);
            this.vb17.TabIndex = 1022;
            this.vb17.Text = "0";
            this.vb17.TextAlign = HorizontalAlignment.Center;
            // 
            // vb16
            // 
            this.vb16.BackColor = System.Drawing.Color.Khaki;
            this.vb16.BorderStyle = BorderStyle.FixedSingle;
            this.vb16.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vb16.Location = new System.Drawing.Point(213, 305);
            this.vb16.Name = "vb16";
            this.vb16.Size = new System.Drawing.Size(24, 21);
            this.vb16.TabIndex = 1021;
            this.vb16.Text = "0";
            this.vb16.TextAlign = HorizontalAlignment.Center;
            // 
            // vb15
            // 
            this.vb15.BackColor = System.Drawing.Color.Khaki;
            this.vb15.BorderStyle = BorderStyle.FixedSingle;
            this.vb15.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vb15.Location = new System.Drawing.Point(188, 305);
            this.vb15.Name = "vb15";
            this.vb15.Size = new System.Drawing.Size(24, 21);
            this.vb15.TabIndex = 1020;
            this.vb15.Text = "0";
            this.vb15.TextAlign = HorizontalAlignment.Center;
            // 
            // vb14
            // 
            this.vb14.BackColor = System.Drawing.Color.Khaki;
            this.vb14.BorderStyle = BorderStyle.FixedSingle;
            this.vb14.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vb14.Location = new System.Drawing.Point(163, 305);
            this.vb14.Name = "vb14";
            this.vb14.Size = new System.Drawing.Size(24, 21);
            this.vb14.TabIndex = 1019;
            this.vb14.Text = "0";
            this.vb14.TextAlign = HorizontalAlignment.Center;
            // 
            // vb13
            // 
            this.vb13.BackColor = System.Drawing.Color.Khaki;
            this.vb13.BorderStyle = BorderStyle.FixedSingle;
            this.vb13.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vb13.Location = new System.Drawing.Point(138, 305);
            this.vb13.Name = "vb13";
            this.vb13.Size = new System.Drawing.Size(24, 21);
            this.vb13.TabIndex = 1018;
            this.vb13.Text = "0";
            this.vb13.TextAlign = HorizontalAlignment.Center;
            // 
            // vb12
            // 
            this.vb12.BackColor = System.Drawing.Color.Khaki;
            this.vb12.BorderStyle = BorderStyle.FixedSingle;
            this.vb12.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vb12.Location = new System.Drawing.Point(113, 305);
            this.vb12.Name = "vb12";
            this.vb12.Size = new System.Drawing.Size(24, 21);
            this.vb12.TabIndex = 1017;
            this.vb12.Text = "0";
            this.vb12.TextAlign = HorizontalAlignment.Center;
            // 
            // vb11
            // 
            this.vb11.BackColor = System.Drawing.Color.Khaki;
            this.vb11.BorderStyle = BorderStyle.FixedSingle;
            this.vb11.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.vb11.Location = new System.Drawing.Point(88, 305);
            this.vb11.Name = "vb11";
            this.vb11.Size = new System.Drawing.Size(24, 21);
            this.vb11.TabIndex = 1016;
            this.vb11.Text = "0";
            this.vb11.TextAlign = HorizontalAlignment.Center;
            // 
            // pb2
            // 
            this.pb2.BackColor = System.Drawing.Color.LemonChiffon;
            this.pb2.BorderStyle = BorderStyle.FixedSingle;
            this.pb2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pb2.Location = new System.Drawing.Point(32, 304);
            this.pb2.Name = "pb2";
            this.pb2.Size = new System.Drawing.Size(24, 21);
            this.pb2.TabIndex = 1014;
            this.pb2.Text = "0";
            this.pb2.TextAlign = HorizontalAlignment.Center;
            // 
            // pb1
            // 
            this.pb1.BackColor = System.Drawing.Color.LemonChiffon;
            this.pb1.BorderStyle = BorderStyle.FixedSingle;
            this.pb1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pb1.Location = new System.Drawing.Point(7, 304);
            this.pb1.Name = "pb1";
            this.pb1.Size = new System.Drawing.Size(24, 21);
            this.pb1.TabIndex = 1013;
            this.pb1.Text = "0";
            this.pb1.TextAlign = HorizontalAlignment.Center;
            // 
            // va19
            // 
            this.va19.BackColor = System.Drawing.Color.Khaki;
            this.va19.BorderStyle = BorderStyle.FixedSingle;
            this.va19.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.va19.Location = new System.Drawing.Point(288, 283);
            this.va19.Name = "va19";
            this.va19.Size = new System.Drawing.Size(24, 21);
            this.va19.TabIndex = 994;
            this.va19.Text = "0";
            this.va19.TextAlign = HorizontalAlignment.Center;
            // 
            // va18
            // 
            this.va18.BackColor = System.Drawing.Color.Khaki;
            this.va18.BorderStyle = BorderStyle.FixedSingle;
            this.va18.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.va18.Location = new System.Drawing.Point(263, 283);
            this.va18.Name = "va18";
            this.va18.Size = new System.Drawing.Size(24, 21);
            this.va18.TabIndex = 993;
            this.va18.Text = "0";
            this.va18.TextAlign = HorizontalAlignment.Center;
            // 
            // va17
            // 
            this.va17.BackColor = System.Drawing.Color.Khaki;
            this.va17.BorderStyle = BorderStyle.FixedSingle;
            this.va17.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.va17.Location = new System.Drawing.Point(238, 283);
            this.va17.Name = "va17";
            this.va17.Size = new System.Drawing.Size(24, 21);
            this.va17.TabIndex = 992;
            this.va17.Text = "0";
            this.va17.TextAlign = HorizontalAlignment.Center;
            // 
            // va16
            // 
            this.va16.BackColor = System.Drawing.Color.Khaki;
            this.va16.BorderStyle = BorderStyle.FixedSingle;
            this.va16.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.va16.Location = new System.Drawing.Point(213, 283);
            this.va16.Name = "va16";
            this.va16.Size = new System.Drawing.Size(24, 21);
            this.va16.TabIndex = 991;
            this.va16.Text = "0";
            this.va16.TextAlign = HorizontalAlignment.Center;
            // 
            // va15
            // 
            this.va15.BackColor = System.Drawing.Color.Khaki;
            this.va15.BorderStyle = BorderStyle.FixedSingle;
            this.va15.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.va15.Location = new System.Drawing.Point(188, 283);
            this.va15.Name = "va15";
            this.va15.Size = new System.Drawing.Size(24, 21);
            this.va15.TabIndex = 990;
            this.va15.Text = "0";
            this.va15.TextAlign = HorizontalAlignment.Center;
            // 
            // va14
            // 
            this.va14.BackColor = System.Drawing.Color.Khaki;
            this.va14.BorderStyle = BorderStyle.FixedSingle;
            this.va14.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.va14.Location = new System.Drawing.Point(163, 283);
            this.va14.Name = "va14";
            this.va14.Size = new System.Drawing.Size(24, 21);
            this.va14.TabIndex = 989;
            this.va14.Text = "0";
            this.va14.TextAlign = HorizontalAlignment.Center;
            // 
            // va13
            // 
            this.va13.BackColor = System.Drawing.Color.Khaki;
            this.va13.BorderStyle = BorderStyle.FixedSingle;
            this.va13.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.va13.Location = new System.Drawing.Point(138, 283);
            this.va13.Name = "va13";
            this.va13.Size = new System.Drawing.Size(24, 21);
            this.va13.TabIndex = 988;
            this.va13.Text = "0";
            this.va13.TextAlign = HorizontalAlignment.Center;
            // 
            // va12
            // 
            this.va12.BackColor = System.Drawing.Color.Khaki;
            this.va12.BorderStyle = BorderStyle.FixedSingle;
            this.va12.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.va12.Location = new System.Drawing.Point(113, 283);
            this.va12.Name = "va12";
            this.va12.Size = new System.Drawing.Size(24, 21);
            this.va12.TabIndex = 987;
            this.va12.Text = "0";
            this.va12.TextAlign = HorizontalAlignment.Center;
            // 
            // va11
            // 
            this.va11.BackColor = System.Drawing.Color.Khaki;
            this.va11.BorderStyle = BorderStyle.FixedSingle;
            this.va11.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.va11.Location = new System.Drawing.Point(88, 283);
            this.va11.Name = "va11";
            this.va11.Size = new System.Drawing.Size(24, 21);
            this.va11.TabIndex = 986;
            this.va11.Text = "0";
            this.va11.TextAlign = HorizontalAlignment.Center;
            // 
            // pa2
            // 
            this.pa2.BackColor = System.Drawing.Color.LemonChiffon;
            this.pa2.BorderStyle = BorderStyle.FixedSingle;
            this.pa2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pa2.Location = new System.Drawing.Point(32, 282);
            this.pa2.Name = "pa2";
            this.pa2.Size = new System.Drawing.Size(24, 21);
            this.pa2.TabIndex = 984;
            this.pa2.Text = "0";
            this.pa2.TextAlign = HorizontalAlignment.Center;
            // 
            // pa1
            // 
            this.pa1.BackColor = System.Drawing.Color.LemonChiffon;
            this.pa1.BorderStyle = BorderStyle.FixedSingle;
            this.pa1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pa1.Location = new System.Drawing.Point(7, 282);
            this.pa1.Name = "pa1";
            this.pa1.Size = new System.Drawing.Size(24, 21);
            this.pa1.TabIndex = 983;
            this.pa1.Text = "0";
            this.pa1.TextAlign = HorizontalAlignment.Center;
            // 
            // label42
            // 
            this.label42.BackColor = System.Drawing.Color.AntiqueWhite;
            this.label42.BorderStyle = BorderStyle.FixedSingle;
            this.label42.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label42.Location = new System.Drawing.Point(544, 11);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(224, 24);
            this.label42.TabIndex = 982;
            this.label42.Text = "2..";
            this.label42.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label41
            // 
            this.label41.BackColor = System.Drawing.Color.AntiqueWhite;
            this.label41.BorderStyle = BorderStyle.FixedSingle;
            this.label41.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label41.Location = new System.Drawing.Point(316, 11);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(224, 24);
            this.label41.TabIndex = 981;
            this.label41.Text = "X..";
            this.label41.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label40
            // 
            this.label40.BackColor = System.Drawing.Color.AntiqueWhite;
            this.label40.BorderStyle = BorderStyle.FixedSingle;
            this.label40.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label40.Location = new System.Drawing.Point(88, 11);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(224, 24);
            this.label40.TabIndex = 980;
            this.label40.Text = "1..";
            this.label40.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // v838
            // 
            this.v838.BackColor = System.Drawing.Color.Khaki;
            this.v838.BorderStyle = BorderStyle.FixedSingle;
            this.v838.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v838.Location = new System.Drawing.Point(719, 239);
            this.v838.Name = "v838";
            this.v838.Size = new System.Drawing.Size(24, 21);
            this.v838.TabIndex = 838;
            this.v838.Text = "0";
            this.v838.TextAlign = HorizontalAlignment.Center;
            // 
            // v837
            // 
            this.v837.BackColor = System.Drawing.Color.Khaki;
            this.v837.BorderStyle = BorderStyle.FixedSingle;
            this.v837.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v837.Location = new System.Drawing.Point(694, 239);
            this.v837.Name = "v837";
            this.v837.Size = new System.Drawing.Size(24, 21);
            this.v837.TabIndex = 837;
            this.v837.Text = "0";
            this.v837.TextAlign = HorizontalAlignment.Center;
            // 
            // v836
            // 
            this.v836.BackColor = System.Drawing.Color.Khaki;
            this.v836.BorderStyle = BorderStyle.FixedSingle;
            this.v836.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v836.Location = new System.Drawing.Point(669, 239);
            this.v836.Name = "v836";
            this.v836.Size = new System.Drawing.Size(24, 21);
            this.v836.TabIndex = 836;
            this.v836.Text = "0";
            this.v836.TextAlign = HorizontalAlignment.Center;
            // 
            // v835
            // 
            this.v835.BackColor = System.Drawing.Color.Khaki;
            this.v835.BorderStyle = BorderStyle.FixedSingle;
            this.v835.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v835.Location = new System.Drawing.Point(644, 239);
            this.v835.Name = "v835";
            this.v835.Size = new System.Drawing.Size(24, 21);
            this.v835.TabIndex = 835;
            this.v835.Text = "0";
            this.v835.TextAlign = HorizontalAlignment.Center;
            // 
            // v834
            // 
            this.v834.BackColor = System.Drawing.Color.Khaki;
            this.v834.BorderStyle = BorderStyle.FixedSingle;
            this.v834.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v834.Location = new System.Drawing.Point(619, 239);
            this.v834.Name = "v834";
            this.v834.Size = new System.Drawing.Size(24, 21);
            this.v834.TabIndex = 834;
            this.v834.Text = "0";
            this.v834.TextAlign = HorizontalAlignment.Center;
            // 
            // v833
            // 
            this.v833.BackColor = System.Drawing.Color.Khaki;
            this.v833.BorderStyle = BorderStyle.FixedSingle;
            this.v833.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v833.Location = new System.Drawing.Point(594, 239);
            this.v833.Name = "v833";
            this.v833.Size = new System.Drawing.Size(24, 21);
            this.v833.TabIndex = 833;
            this.v833.Text = "0";
            this.v833.TextAlign = HorizontalAlignment.Center;
            // 
            // v832
            // 
            this.v832.BackColor = System.Drawing.Color.Khaki;
            this.v832.BorderStyle = BorderStyle.FixedSingle;
            this.v832.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v832.Location = new System.Drawing.Point(569, 239);
            this.v832.Name = "v832";
            this.v832.Size = new System.Drawing.Size(24, 21);
            this.v832.TabIndex = 832;
            this.v832.Text = "0";
            this.v832.TextAlign = HorizontalAlignment.Center;
            // 
            // v739
            // 
            this.v739.BackColor = System.Drawing.Color.Khaki;
            this.v739.BorderStyle = BorderStyle.FixedSingle;
            this.v739.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v739.Location = new System.Drawing.Point(744, 217);
            this.v739.Name = "v739";
            this.v739.Size = new System.Drawing.Size(24, 21);
            this.v739.TabIndex = 739;
            this.v739.Text = "0";
            this.v739.TextAlign = HorizontalAlignment.Center;
            // 
            // v738
            // 
            this.v738.BackColor = System.Drawing.Color.Khaki;
            this.v738.BorderStyle = BorderStyle.FixedSingle;
            this.v738.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v738.Location = new System.Drawing.Point(719, 217);
            this.v738.Name = "v738";
            this.v738.Size = new System.Drawing.Size(24, 21);
            this.v738.TabIndex = 738;
            this.v738.Text = "0";
            this.v738.TextAlign = HorizontalAlignment.Center;
            // 
            // v737
            // 
            this.v737.BackColor = System.Drawing.Color.Khaki;
            this.v737.BorderStyle = BorderStyle.FixedSingle;
            this.v737.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v737.Location = new System.Drawing.Point(694, 217);
            this.v737.Name = "v737";
            this.v737.Size = new System.Drawing.Size(24, 21);
            this.v737.TabIndex = 737;
            this.v737.Text = "0";
            this.v737.TextAlign = HorizontalAlignment.Center;
            // 
            // v736
            // 
            this.v736.BackColor = System.Drawing.Color.Khaki;
            this.v736.BorderStyle = BorderStyle.FixedSingle;
            this.v736.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v736.Location = new System.Drawing.Point(669, 217);
            this.v736.Name = "v736";
            this.v736.Size = new System.Drawing.Size(24, 21);
            this.v736.TabIndex = 736;
            this.v736.Text = "0";
            this.v736.TextAlign = HorizontalAlignment.Center;
            // 
            // v735
            // 
            this.v735.BackColor = System.Drawing.Color.Khaki;
            this.v735.BorderStyle = BorderStyle.FixedSingle;
            this.v735.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v735.Location = new System.Drawing.Point(644, 217);
            this.v735.Name = "v735";
            this.v735.Size = new System.Drawing.Size(24, 21);
            this.v735.TabIndex = 735;
            this.v735.Text = "0";
            this.v735.TextAlign = HorizontalAlignment.Center;
            // 
            // v734
            // 
            this.v734.BackColor = System.Drawing.Color.Khaki;
            this.v734.BorderStyle = BorderStyle.FixedSingle;
            this.v734.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v734.Location = new System.Drawing.Point(619, 217);
            this.v734.Name = "v734";
            this.v734.Size = new System.Drawing.Size(24, 21);
            this.v734.TabIndex = 734;
            this.v734.Text = "0";
            this.v734.TextAlign = HorizontalAlignment.Center;
            // 
            // v733
            // 
            this.v733.BackColor = System.Drawing.Color.Khaki;
            this.v733.BorderStyle = BorderStyle.FixedSingle;
            this.v733.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v733.Location = new System.Drawing.Point(594, 217);
            this.v733.Name = "v733";
            this.v733.Size = new System.Drawing.Size(24, 21);
            this.v733.TabIndex = 733;
            this.v733.Text = "0";
            this.v733.TextAlign = HorizontalAlignment.Center;
            // 
            // v732
            // 
            this.v732.BackColor = System.Drawing.Color.Khaki;
            this.v732.BorderStyle = BorderStyle.FixedSingle;
            this.v732.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v732.Location = new System.Drawing.Point(569, 217);
            this.v732.Name = "v732";
            this.v732.Size = new System.Drawing.Size(24, 21);
            this.v732.TabIndex = 732;
            this.v732.Text = "0";
            this.v732.TextAlign = HorizontalAlignment.Center;
            // 
            // v731
            // 
            this.v731.BackColor = System.Drawing.Color.Khaki;
            this.v731.BorderStyle = BorderStyle.FixedSingle;
            this.v731.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v731.Location = new System.Drawing.Point(544, 217);
            this.v731.Name = "v731";
            this.v731.Size = new System.Drawing.Size(24, 21);
            this.v731.TabIndex = 731;
            this.v731.Text = "0";
            this.v731.TextAlign = HorizontalAlignment.Center;
            // 
            // v639
            // 
            this.v639.BackColor = System.Drawing.Color.Khaki;
            this.v639.BorderStyle = BorderStyle.FixedSingle;
            this.v639.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v639.Location = new System.Drawing.Point(744, 195);
            this.v639.Name = "v639";
            this.v639.Size = new System.Drawing.Size(24, 21);
            this.v639.TabIndex = 639;
            this.v639.Text = "0";
            this.v639.TextAlign = HorizontalAlignment.Center;
            // 
            // v638
            // 
            this.v638.BackColor = System.Drawing.Color.Khaki;
            this.v638.BorderStyle = BorderStyle.FixedSingle;
            this.v638.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v638.Location = new System.Drawing.Point(719, 195);
            this.v638.Name = "v638";
            this.v638.Size = new System.Drawing.Size(24, 21);
            this.v638.TabIndex = 638;
            this.v638.Text = "0";
            this.v638.TextAlign = HorizontalAlignment.Center;
            // 
            // v637
            // 
            this.v637.BackColor = System.Drawing.Color.Khaki;
            this.v637.BorderStyle = BorderStyle.FixedSingle;
            this.v637.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v637.Location = new System.Drawing.Point(694, 195);
            this.v637.Name = "v637";
            this.v637.Size = new System.Drawing.Size(24, 21);
            this.v637.TabIndex = 637;
            this.v637.Text = "0";
            this.v637.TextAlign = HorizontalAlignment.Center;
            // 
            // v636
            // 
            this.v636.BackColor = System.Drawing.Color.Khaki;
            this.v636.BorderStyle = BorderStyle.FixedSingle;
            this.v636.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v636.Location = new System.Drawing.Point(669, 195);
            this.v636.Name = "v636";
            this.v636.Size = new System.Drawing.Size(24, 21);
            this.v636.TabIndex = 636;
            this.v636.Text = "0";
            this.v636.TextAlign = HorizontalAlignment.Center;
            // 
            // v635
            // 
            this.v635.BackColor = System.Drawing.Color.Khaki;
            this.v635.BorderStyle = BorderStyle.FixedSingle;
            this.v635.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v635.Location = new System.Drawing.Point(644, 195);
            this.v635.Name = "v635";
            this.v635.Size = new System.Drawing.Size(24, 21);
            this.v635.TabIndex = 635;
            this.v635.Text = "0";
            this.v635.TextAlign = HorizontalAlignment.Center;
            // 
            // v634
            // 
            this.v634.BackColor = System.Drawing.Color.Khaki;
            this.v634.BorderStyle = BorderStyle.FixedSingle;
            this.v634.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v634.Location = new System.Drawing.Point(619, 195);
            this.v634.Name = "v634";
            this.v634.Size = new System.Drawing.Size(24, 21);
            this.v634.TabIndex = 634;
            this.v634.Text = "0";
            this.v634.TextAlign = HorizontalAlignment.Center;
            // 
            // v633
            // 
            this.v633.BackColor = System.Drawing.Color.Khaki;
            this.v633.BorderStyle = BorderStyle.FixedSingle;
            this.v633.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v633.Location = new System.Drawing.Point(594, 195);
            this.v633.Name = "v633";
            this.v633.Size = new System.Drawing.Size(24, 21);
            this.v633.TabIndex = 633;
            this.v633.Text = "0";
            this.v633.TextAlign = HorizontalAlignment.Center;
            // 
            // v632
            // 
            this.v632.BackColor = System.Drawing.Color.Khaki;
            this.v632.BorderStyle = BorderStyle.FixedSingle;
            this.v632.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v632.Location = new System.Drawing.Point(569, 195);
            this.v632.Name = "v632";
            this.v632.Size = new System.Drawing.Size(24, 21);
            this.v632.TabIndex = 632;
            this.v632.Text = "0";
            this.v632.TextAlign = HorizontalAlignment.Center;
            // 
            // v631
            // 
            this.v631.BackColor = System.Drawing.Color.Khaki;
            this.v631.BorderStyle = BorderStyle.FixedSingle;
            this.v631.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v631.Location = new System.Drawing.Point(544, 195);
            this.v631.Name = "v631";
            this.v631.Size = new System.Drawing.Size(24, 21);
            this.v631.TabIndex = 631;
            this.v631.Text = "0";
            this.v631.TextAlign = HorizontalAlignment.Center;
            // 
            // v539
            // 
            this.v539.BackColor = System.Drawing.Color.Khaki;
            this.v539.BorderStyle = BorderStyle.FixedSingle;
            this.v539.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v539.Location = new System.Drawing.Point(744, 173);
            this.v539.Name = "v539";
            this.v539.Size = new System.Drawing.Size(24, 21);
            this.v539.TabIndex = 539;
            this.v539.Text = "0";
            this.v539.TextAlign = HorizontalAlignment.Center;
            // 
            // v538
            // 
            this.v538.BackColor = System.Drawing.Color.Khaki;
            this.v538.BorderStyle = BorderStyle.FixedSingle;
            this.v538.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v538.Location = new System.Drawing.Point(719, 173);
            this.v538.Name = "v538";
            this.v538.Size = new System.Drawing.Size(24, 21);
            this.v538.TabIndex = 538;
            this.v538.Text = "0";
            this.v538.TextAlign = HorizontalAlignment.Center;
            // 
            // v532
            // 
            this.v532.BackColor = System.Drawing.Color.Khaki;
            this.v532.BorderStyle = BorderStyle.FixedSingle;
            this.v532.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v532.Location = new System.Drawing.Point(569, 173);
            this.v532.Name = "v532";
            this.v532.Size = new System.Drawing.Size(24, 21);
            this.v532.TabIndex = 532;
            this.v532.Text = "0";
            this.v532.TextAlign = HorizontalAlignment.Center;
            // 
            // v439
            // 
            this.v439.BackColor = System.Drawing.Color.Khaki;
            this.v439.BorderStyle = BorderStyle.FixedSingle;
            this.v439.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v439.Location = new System.Drawing.Point(744, 151);
            this.v439.Name = "v439";
            this.v439.Size = new System.Drawing.Size(24, 21);
            this.v439.TabIndex = 439;
            this.v439.Text = "0";
            this.v439.TextAlign = HorizontalAlignment.Center;
            // 
            // v438
            // 
            this.v438.BackColor = System.Drawing.Color.Khaki;
            this.v438.BorderStyle = BorderStyle.FixedSingle;
            this.v438.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v438.Location = new System.Drawing.Point(719, 151);
            this.v438.Name = "v438";
            this.v438.Size = new System.Drawing.Size(24, 21);
            this.v438.TabIndex = 438;
            this.v438.Text = "0";
            this.v438.TextAlign = HorizontalAlignment.Center;
            // 
            // v433
            // 
            this.v433.BackColor = System.Drawing.Color.Khaki;
            this.v433.BorderStyle = BorderStyle.FixedSingle;
            this.v433.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v433.Location = new System.Drawing.Point(594, 151);
            this.v433.Name = "v433";
            this.v433.Size = new System.Drawing.Size(24, 21);
            this.v433.TabIndex = 433;
            this.v433.Text = "0";
            this.v433.TextAlign = HorizontalAlignment.Center;
            // 
            // v432
            // 
            this.v432.BackColor = System.Drawing.Color.Khaki;
            this.v432.BorderStyle = BorderStyle.FixedSingle;
            this.v432.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v432.Location = new System.Drawing.Point(569, 151);
            this.v432.Name = "v432";
            this.v432.Size = new System.Drawing.Size(24, 21);
            this.v432.TabIndex = 432;
            this.v432.Text = "0";
            this.v432.TextAlign = HorizontalAlignment.Center;
            // 
            // v339
            // 
            this.v339.BackColor = System.Drawing.Color.Khaki;
            this.v339.BorderStyle = BorderStyle.FixedSingle;
            this.v339.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v339.Location = new System.Drawing.Point(744, 129);
            this.v339.Name = "v339";
            this.v339.Size = new System.Drawing.Size(24, 21);
            this.v339.TabIndex = 339;
            this.v339.Text = "0";
            this.v339.TextAlign = HorizontalAlignment.Center;
            // 
            // v338
            // 
            this.v338.BackColor = System.Drawing.Color.Khaki;
            this.v338.BorderStyle = BorderStyle.FixedSingle;
            this.v338.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v338.Location = new System.Drawing.Point(719, 129);
            this.v338.Name = "v338";
            this.v338.Size = new System.Drawing.Size(24, 21);
            this.v338.TabIndex = 338;
            this.v338.Text = "0";
            this.v338.TextAlign = HorizontalAlignment.Center;
            // 
            // v337
            // 
            this.v337.BackColor = System.Drawing.Color.Khaki;
            this.v337.BorderStyle = BorderStyle.FixedSingle;
            this.v337.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v337.Location = new System.Drawing.Point(694, 129);
            this.v337.Name = "v337";
            this.v337.Size = new System.Drawing.Size(24, 21);
            this.v337.TabIndex = 337;
            this.v337.Text = "0";
            this.v337.TextAlign = HorizontalAlignment.Center;
            // 
            // v336
            // 
            this.v336.BackColor = System.Drawing.Color.Khaki;
            this.v336.BorderStyle = BorderStyle.FixedSingle;
            this.v336.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v336.Location = new System.Drawing.Point(669, 129);
            this.v336.Name = "v336";
            this.v336.Size = new System.Drawing.Size(24, 21);
            this.v336.TabIndex = 336;
            this.v336.Text = "0";
            this.v336.TextAlign = HorizontalAlignment.Center;
            // 
            // v335
            // 
            this.v335.BackColor = System.Drawing.Color.Khaki;
            this.v335.BorderStyle = BorderStyle.FixedSingle;
            this.v335.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v335.Location = new System.Drawing.Point(644, 129);
            this.v335.Name = "v335";
            this.v335.Size = new System.Drawing.Size(24, 21);
            this.v335.TabIndex = 335;
            this.v335.Text = "0";
            this.v335.TextAlign = HorizontalAlignment.Center;
            // 
            // v334
            // 
            this.v334.BackColor = System.Drawing.Color.Khaki;
            this.v334.BorderStyle = BorderStyle.FixedSingle;
            this.v334.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v334.Location = new System.Drawing.Point(619, 129);
            this.v334.Name = "v334";
            this.v334.Size = new System.Drawing.Size(24, 21);
            this.v334.TabIndex = 334;
            this.v334.Text = "0";
            this.v334.TextAlign = HorizontalAlignment.Center;
            // 
            // v333
            // 
            this.v333.BackColor = System.Drawing.Color.Khaki;
            this.v333.BorderStyle = BorderStyle.FixedSingle;
            this.v333.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v333.Location = new System.Drawing.Point(594, 129);
            this.v333.Name = "v333";
            this.v333.Size = new System.Drawing.Size(24, 21);
            this.v333.TabIndex = 333;
            this.v333.Text = "0";
            this.v333.TextAlign = HorizontalAlignment.Center;
            // 
            // v332
            // 
            this.v332.BackColor = System.Drawing.Color.Khaki;
            this.v332.BorderStyle = BorderStyle.FixedSingle;
            this.v332.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v332.Location = new System.Drawing.Point(569, 129);
            this.v332.Name = "v332";
            this.v332.Size = new System.Drawing.Size(24, 21);
            this.v332.TabIndex = 332;
            this.v332.Text = "0";
            this.v332.TextAlign = HorizontalAlignment.Center;
            // 
            // v331
            // 
            this.v331.BackColor = System.Drawing.Color.Khaki;
            this.v331.BorderStyle = BorderStyle.FixedSingle;
            this.v331.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v331.Location = new System.Drawing.Point(544, 129);
            this.v331.Name = "v331";
            this.v331.Size = new System.Drawing.Size(24, 21);
            this.v331.TabIndex = 331;
            this.v331.Text = "0";
            this.v331.TextAlign = HorizontalAlignment.Center;
            // 
            // v239
            // 
            this.v239.BackColor = System.Drawing.Color.Khaki;
            this.v239.BorderStyle = BorderStyle.FixedSingle;
            this.v239.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v239.Location = new System.Drawing.Point(744, 107);
            this.v239.Name = "v239";
            this.v239.Size = new System.Drawing.Size(24, 21);
            this.v239.TabIndex = 239;
            this.v239.Text = "0";
            this.v239.TextAlign = HorizontalAlignment.Center;
            // 
            // v238
            // 
            this.v238.BackColor = System.Drawing.Color.Khaki;
            this.v238.BorderStyle = BorderStyle.FixedSingle;
            this.v238.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v238.Location = new System.Drawing.Point(719, 107);
            this.v238.Name = "v238";
            this.v238.Size = new System.Drawing.Size(24, 21);
            this.v238.TabIndex = 238;
            this.v238.Text = "0";
            this.v238.TextAlign = HorizontalAlignment.Center;
            // 
            // v237
            // 
            this.v237.BackColor = System.Drawing.Color.Khaki;
            this.v237.BorderStyle = BorderStyle.FixedSingle;
            this.v237.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v237.Location = new System.Drawing.Point(694, 107);
            this.v237.Name = "v237";
            this.v237.Size = new System.Drawing.Size(24, 21);
            this.v237.TabIndex = 237;
            this.v237.Text = "0";
            this.v237.TextAlign = HorizontalAlignment.Center;
            // 
            // v236
            // 
            this.v236.BackColor = System.Drawing.Color.Khaki;
            this.v236.BorderStyle = BorderStyle.FixedSingle;
            this.v236.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v236.Location = new System.Drawing.Point(669, 107);
            this.v236.Name = "v236";
            this.v236.Size = new System.Drawing.Size(24, 21);
            this.v236.TabIndex = 236;
            this.v236.Text = "0";
            this.v236.TextAlign = HorizontalAlignment.Center;
            // 
            // v235
            // 
            this.v235.BackColor = System.Drawing.Color.Khaki;
            this.v235.BorderStyle = BorderStyle.FixedSingle;
            this.v235.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v235.Location = new System.Drawing.Point(644, 107);
            this.v235.Name = "v235";
            this.v235.Size = new System.Drawing.Size(24, 21);
            this.v235.TabIndex = 235;
            this.v235.Text = "0";
            this.v235.TextAlign = HorizontalAlignment.Center;
            // 
            // v234
            // 
            this.v234.BackColor = System.Drawing.Color.Khaki;
            this.v234.BorderStyle = BorderStyle.FixedSingle;
            this.v234.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v234.Location = new System.Drawing.Point(619, 107);
            this.v234.Name = "v234";
            this.v234.Size = new System.Drawing.Size(24, 21);
            this.v234.TabIndex = 234;
            this.v234.Text = "0";
            this.v234.TextAlign = HorizontalAlignment.Center;
            // 
            // v233
            // 
            this.v233.BackColor = System.Drawing.Color.Khaki;
            this.v233.BorderStyle = BorderStyle.FixedSingle;
            this.v233.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v233.Location = new System.Drawing.Point(594, 107);
            this.v233.Name = "v233";
            this.v233.Size = new System.Drawing.Size(24, 21);
            this.v233.TabIndex = 233;
            this.v233.Text = "0";
            this.v233.TextAlign = HorizontalAlignment.Center;
            // 
            // v232
            // 
            this.v232.BackColor = System.Drawing.Color.Khaki;
            this.v232.BorderStyle = BorderStyle.FixedSingle;
            this.v232.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v232.Location = new System.Drawing.Point(569, 107);
            this.v232.Name = "v232";
            this.v232.Size = new System.Drawing.Size(24, 21);
            this.v232.TabIndex = 232;
            this.v232.Text = "0";
            this.v232.TextAlign = HorizontalAlignment.Center;
            // 
            // v231
            // 
            this.v231.BackColor = System.Drawing.Color.Khaki;
            this.v231.BorderStyle = BorderStyle.FixedSingle;
            this.v231.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v231.Location = new System.Drawing.Point(544, 107);
            this.v231.Name = "v231";
            this.v231.Size = new System.Drawing.Size(24, 21);
            this.v231.TabIndex = 231;
            this.v231.Text = "0";
            this.v231.TextAlign = HorizontalAlignment.Center;
            // 
            // v039
            // 
            this.v039.BackColor = System.Drawing.Color.Khaki;
            this.v039.BorderStyle = BorderStyle.FixedSingle;
            this.v039.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v039.Location = new System.Drawing.Point(744, 63);
            this.v039.Name = "v039";
            this.v039.Size = new System.Drawing.Size(24, 21);
            this.v039.TabIndex = 39;
            this.v039.Text = "0";
            this.v039.TextAlign = HorizontalAlignment.Center;
            // 
            // v038
            // 
            this.v038.BackColor = System.Drawing.Color.Khaki;
            this.v038.BorderStyle = BorderStyle.FixedSingle;
            this.v038.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v038.Location = new System.Drawing.Point(719, 63);
            this.v038.Name = "v038";
            this.v038.Size = new System.Drawing.Size(24, 21);
            this.v038.TabIndex = 38;
            this.v038.Text = "0";
            this.v038.TextAlign = HorizontalAlignment.Center;
            // 
            // v037
            // 
            this.v037.BackColor = System.Drawing.Color.Khaki;
            this.v037.BorderStyle = BorderStyle.FixedSingle;
            this.v037.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v037.Location = new System.Drawing.Point(694, 63);
            this.v037.Name = "v037";
            this.v037.Size = new System.Drawing.Size(24, 21);
            this.v037.TabIndex = 37;
            this.v037.Text = "0";
            this.v037.TextAlign = HorizontalAlignment.Center;
            // 
            // v034
            // 
            this.v034.BackColor = System.Drawing.Color.Khaki;
            this.v034.BorderStyle = BorderStyle.FixedSingle;
            this.v034.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v034.Location = new System.Drawing.Point(619, 63);
            this.v034.Name = "v034";
            this.v034.Size = new System.Drawing.Size(24, 21);
            this.v034.TabIndex = 34;
            this.v034.Text = "0";
            this.v034.TextAlign = HorizontalAlignment.Center;
            // 
            // v032
            // 
            this.v032.BackColor = System.Drawing.Color.Khaki;
            this.v032.BorderStyle = BorderStyle.FixedSingle;
            this.v032.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v032.Location = new System.Drawing.Point(569, 63);
            this.v032.Name = "v032";
            this.v032.Size = new System.Drawing.Size(24, 21);
            this.v032.TabIndex = 32;
            this.v032.Text = "0";
            this.v032.TextAlign = HorizontalAlignment.Center;
            // 
            // v031
            // 
            this.v031.BackColor = System.Drawing.Color.Khaki;
            this.v031.BorderStyle = BorderStyle.FixedSingle;
            this.v031.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v031.Location = new System.Drawing.Point(544, 63);
            this.v031.Name = "v031";
            this.v031.Size = new System.Drawing.Size(24, 21);
            this.v031.TabIndex = 31;
            this.v031.Text = "0";
            this.v031.TextAlign = HorizontalAlignment.Center;
            // 
            // label31
            // 
            this.label31.BackColor = System.Drawing.Color.AntiqueWhite;
            this.label31.BorderStyle = BorderStyle.FixedSingle;
            this.label31.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label31.Location = new System.Drawing.Point(644, 37);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(24, 24);
            this.label31.TabIndex = 975;
            this.label31.Text = "xx";
            this.label31.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label32
            // 
            this.label32.BackColor = System.Drawing.Color.AntiqueWhite;
            this.label32.BorderStyle = BorderStyle.FixedSingle;
            this.label32.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label32.Location = new System.Drawing.Point(619, 37);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(24, 24);
            this.label32.TabIndex = 974;
            this.label32.Text = "x1";
            this.label32.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label33
            // 
            this.label33.BackColor = System.Drawing.Color.AntiqueWhite;
            this.label33.BorderStyle = BorderStyle.FixedSingle;
            this.label33.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label33.Location = new System.Drawing.Point(594, 37);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(24, 24);
            this.label33.TabIndex = 973;
            this.label33.Text = "12";
            this.label33.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label34
            // 
            this.label34.BackColor = System.Drawing.Color.AntiqueWhite;
            this.label34.BorderStyle = BorderStyle.FixedSingle;
            this.label34.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label34.Location = new System.Drawing.Point(569, 37);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(24, 24);
            this.label34.TabIndex = 972;
            this.label34.Text = "1x";
            this.label34.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label35
            // 
            this.label35.BackColor = System.Drawing.Color.AntiqueWhite;
            this.label35.BorderStyle = BorderStyle.FixedSingle;
            this.label35.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label35.Location = new System.Drawing.Point(544, 37);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(24, 24);
            this.label35.TabIndex = 971;
            this.label35.Text = "11";
            this.label35.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label36
            // 
            this.label36.BackColor = System.Drawing.Color.AntiqueWhite;
            this.label36.BorderStyle = BorderStyle.FixedSingle;
            this.label36.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label36.Location = new System.Drawing.Point(744, 37);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(24, 24);
            this.label36.TabIndex = 979;
            this.label36.Text = "22";
            this.label36.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label37
            // 
            this.label37.BackColor = System.Drawing.Color.AntiqueWhite;
            this.label37.BorderStyle = BorderStyle.FixedSingle;
            this.label37.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label37.Location = new System.Drawing.Point(719, 37);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(24, 24);
            this.label37.TabIndex = 978;
            this.label37.Text = "2x";
            this.label37.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label38
            // 
            this.label38.BackColor = System.Drawing.Color.AntiqueWhite;
            this.label38.BorderStyle = BorderStyle.FixedSingle;
            this.label38.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label38.Location = new System.Drawing.Point(694, 37);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(24, 24);
            this.label38.TabIndex = 977;
            this.label38.Text = "21";
            this.label38.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label39
            // 
            this.label39.BackColor = System.Drawing.Color.AntiqueWhite;
            this.label39.BorderStyle = BorderStyle.FixedSingle;
            this.label39.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label39.Location = new System.Drawing.Point(669, 37);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(24, 24);
            this.label39.TabIndex = 976;
            this.label39.Text = "x2";
            this.label39.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // v929
            // 
            this.v929.BorderStyle = BorderStyle.FixedSingle;
            this.v929.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v929.Location = new System.Drawing.Point(516, 261);
            this.v929.Name = "v929";
            this.v929.Size = new System.Drawing.Size(24, 21);
            this.v929.TabIndex = 929;
            this.v929.Text = "0";
            this.v929.TextAlign = HorizontalAlignment.Center;
            // 
            // v928
            // 
            this.v928.BorderStyle = BorderStyle.FixedSingle;
            this.v928.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v928.Location = new System.Drawing.Point(491, 261);
            this.v928.Name = "v928";
            this.v928.Size = new System.Drawing.Size(24, 21);
            this.v928.TabIndex = 928;
            this.v928.Text = "0";
            this.v928.TextAlign = HorizontalAlignment.Center;
            // 
            // v927
            // 
            this.v927.BorderStyle = BorderStyle.FixedSingle;
            this.v927.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v927.Location = new System.Drawing.Point(466, 261);
            this.v927.Name = "v927";
            this.v927.Size = new System.Drawing.Size(24, 21);
            this.v927.TabIndex = 927;
            this.v927.Text = "0";
            this.v927.TextAlign = HorizontalAlignment.Center;
            // 
            // v926
            // 
            this.v926.BorderStyle = BorderStyle.FixedSingle;
            this.v926.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v926.Location = new System.Drawing.Point(441, 261);
            this.v926.Name = "v926";
            this.v926.Size = new System.Drawing.Size(24, 21);
            this.v926.TabIndex = 926;
            this.v926.Text = "0";
            this.v926.TextAlign = HorizontalAlignment.Center;
            // 
            // v925
            // 
            this.v925.BorderStyle = BorderStyle.FixedSingle;
            this.v925.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v925.Location = new System.Drawing.Point(416, 261);
            this.v925.Name = "v925";
            this.v925.Size = new System.Drawing.Size(24, 21);
            this.v925.TabIndex = 925;
            this.v925.Text = "0";
            this.v925.TextAlign = HorizontalAlignment.Center;
            // 
            // v924
            // 
            this.v924.BorderStyle = BorderStyle.FixedSingle;
            this.v924.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v924.Location = new System.Drawing.Point(391, 261);
            this.v924.Name = "v924";
            this.v924.Size = new System.Drawing.Size(24, 21);
            this.v924.TabIndex = 924;
            this.v924.Text = "0";
            this.v924.TextAlign = HorizontalAlignment.Center;
            // 
            // v923
            // 
            this.v923.BorderStyle = BorderStyle.FixedSingle;
            this.v923.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v923.Location = new System.Drawing.Point(366, 261);
            this.v923.Name = "v923";
            this.v923.Size = new System.Drawing.Size(24, 21);
            this.v923.TabIndex = 923;
            this.v923.Text = "0";
            this.v923.TextAlign = HorizontalAlignment.Center;
            // 
            // v922
            // 
            this.v922.BorderStyle = BorderStyle.FixedSingle;
            this.v922.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v922.Location = new System.Drawing.Point(341, 261);
            this.v922.Name = "v922";
            this.v922.Size = new System.Drawing.Size(24, 21);
            this.v922.TabIndex = 922;
            this.v922.Text = "0";
            this.v922.TextAlign = HorizontalAlignment.Center;
            // 
            // v921
            // 
            this.v921.BorderStyle = BorderStyle.FixedSingle;
            this.v921.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v921.Location = new System.Drawing.Point(316, 261);
            this.v921.Name = "v921";
            this.v921.Size = new System.Drawing.Size(24, 21);
            this.v921.TabIndex = 921;
            this.v921.Text = "0";
            this.v921.TextAlign = HorizontalAlignment.Center;
            // 
            // v727
            // 
            this.v727.BorderStyle = BorderStyle.FixedSingle;
            this.v727.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v727.Location = new System.Drawing.Point(466, 217);
            this.v727.Name = "v727";
            this.v727.Size = new System.Drawing.Size(24, 21);
            this.v727.TabIndex = 727;
            this.v727.Text = "0";
            this.v727.TextAlign = HorizontalAlignment.Center;
            // 
            // v726
            // 
            this.v726.BorderStyle = BorderStyle.FixedSingle;
            this.v726.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v726.Location = new System.Drawing.Point(441, 217);
            this.v726.Name = "v726";
            this.v726.Size = new System.Drawing.Size(24, 21);
            this.v726.TabIndex = 726;
            this.v726.Text = "0";
            this.v726.TextAlign = HorizontalAlignment.Center;
            // 
            // v725
            // 
            this.v725.BorderStyle = BorderStyle.FixedSingle;
            this.v725.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v725.Location = new System.Drawing.Point(416, 217);
            this.v725.Name = "v725";
            this.v725.Size = new System.Drawing.Size(24, 21);
            this.v725.TabIndex = 725;
            this.v725.Text = "0";
            this.v725.TextAlign = HorizontalAlignment.Center;
            // 
            // v724
            // 
            this.v724.BorderStyle = BorderStyle.FixedSingle;
            this.v724.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v724.Location = new System.Drawing.Point(391, 217);
            this.v724.Name = "v724";
            this.v724.Size = new System.Drawing.Size(24, 21);
            this.v724.TabIndex = 724;
            this.v724.Text = "0";
            this.v724.TextAlign = HorizontalAlignment.Center;
            // 
            // v723
            // 
            this.v723.BorderStyle = BorderStyle.FixedSingle;
            this.v723.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v723.Location = new System.Drawing.Point(366, 217);
            this.v723.Name = "v723";
            this.v723.Size = new System.Drawing.Size(24, 21);
            this.v723.TabIndex = 723;
            this.v723.Text = "0";
            this.v723.TextAlign = HorizontalAlignment.Center;
            // 
            // v722
            // 
            this.v722.BorderStyle = BorderStyle.FixedSingle;
            this.v722.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v722.Location = new System.Drawing.Point(341, 217);
            this.v722.Name = "v722";
            this.v722.Size = new System.Drawing.Size(24, 21);
            this.v722.TabIndex = 722;
            this.v722.Text = "0";
            this.v722.TextAlign = HorizontalAlignment.Center;
            // 
            // v721
            // 
            this.v721.BorderStyle = BorderStyle.FixedSingle;
            this.v721.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v721.Location = new System.Drawing.Point(316, 217);
            this.v721.Name = "v721";
            this.v721.Size = new System.Drawing.Size(24, 21);
            this.v721.TabIndex = 721;
            this.v721.Text = "0";
            this.v721.TextAlign = HorizontalAlignment.Center;
            // 
            // v629
            // 
            this.v629.BorderStyle = BorderStyle.FixedSingle;
            this.v629.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v629.Location = new System.Drawing.Point(516, 195);
            this.v629.Name = "v629";
            this.v629.Size = new System.Drawing.Size(24, 21);
            this.v629.TabIndex = 629;
            this.v629.Text = "0";
            this.v629.TextAlign = HorizontalAlignment.Center;
            // 
            // v628
            // 
            this.v628.BorderStyle = BorderStyle.FixedSingle;
            this.v628.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v628.Location = new System.Drawing.Point(491, 195);
            this.v628.Name = "v628";
            this.v628.Size = new System.Drawing.Size(24, 21);
            this.v628.TabIndex = 628;
            this.v628.Text = "0";
            this.v628.TextAlign = HorizontalAlignment.Center;
            // 
            // v627
            // 
            this.v627.BorderStyle = BorderStyle.FixedSingle;
            this.v627.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v627.Location = new System.Drawing.Point(466, 195);
            this.v627.Name = "v627";
            this.v627.Size = new System.Drawing.Size(24, 21);
            this.v627.TabIndex = 627;
            this.v627.Text = "0";
            this.v627.TextAlign = HorizontalAlignment.Center;
            // 
            // v626
            // 
            this.v626.BorderStyle = BorderStyle.FixedSingle;
            this.v626.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v626.Location = new System.Drawing.Point(441, 195);
            this.v626.Name = "v626";
            this.v626.Size = new System.Drawing.Size(24, 21);
            this.v626.TabIndex = 626;
            this.v626.Text = "0";
            this.v626.TextAlign = HorizontalAlignment.Center;
            // 
            // v625
            // 
            this.v625.BorderStyle = BorderStyle.FixedSingle;
            this.v625.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v625.Location = new System.Drawing.Point(416, 195);
            this.v625.Name = "v625";
            this.v625.Size = new System.Drawing.Size(24, 21);
            this.v625.TabIndex = 625;
            this.v625.Text = "0";
            this.v625.TextAlign = HorizontalAlignment.Center;
            // 
            // v624
            // 
            this.v624.BorderStyle = BorderStyle.FixedSingle;
            this.v624.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v624.Location = new System.Drawing.Point(391, 195);
            this.v624.Name = "v624";
            this.v624.Size = new System.Drawing.Size(24, 21);
            this.v624.TabIndex = 624;
            this.v624.Text = "0";
            this.v624.TextAlign = HorizontalAlignment.Center;
            // 
            // v623
            // 
            this.v623.BorderStyle = BorderStyle.FixedSingle;
            this.v623.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v623.Location = new System.Drawing.Point(366, 195);
            this.v623.Name = "v623";
            this.v623.Size = new System.Drawing.Size(24, 21);
            this.v623.TabIndex = 623;
            this.v623.Text = "0";
            this.v623.TextAlign = HorizontalAlignment.Center;
            // 
            // v622
            // 
            this.v622.BorderStyle = BorderStyle.FixedSingle;
            this.v622.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v622.Location = new System.Drawing.Point(341, 195);
            this.v622.Name = "v622";
            this.v622.Size = new System.Drawing.Size(24, 21);
            this.v622.TabIndex = 622;
            this.v622.Text = "0";
            this.v622.TextAlign = HorizontalAlignment.Center;
            // 
            // v621
            // 
            this.v621.BorderStyle = BorderStyle.FixedSingle;
            this.v621.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v621.Location = new System.Drawing.Point(316, 195);
            this.v621.Name = "v621";
            this.v621.Size = new System.Drawing.Size(24, 21);
            this.v621.TabIndex = 621;
            this.v621.Text = "0";
            this.v621.TextAlign = HorizontalAlignment.Center;
            // 
            // v329
            // 
            this.v329.BorderStyle = BorderStyle.FixedSingle;
            this.v329.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v329.Location = new System.Drawing.Point(516, 129);
            this.v329.Name = "v329";
            this.v329.Size = new System.Drawing.Size(24, 21);
            this.v329.TabIndex = 329;
            this.v329.Text = "0";
            this.v329.TextAlign = HorizontalAlignment.Center;
            // 
            // v328
            // 
            this.v328.BorderStyle = BorderStyle.FixedSingle;
            this.v328.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v328.Location = new System.Drawing.Point(491, 129);
            this.v328.Name = "v328";
            this.v328.Size = new System.Drawing.Size(24, 21);
            this.v328.TabIndex = 328;
            this.v328.Text = "0";
            this.v328.TextAlign = HorizontalAlignment.Center;
            // 
            // v327
            // 
            this.v327.BorderStyle = BorderStyle.FixedSingle;
            this.v327.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v327.Location = new System.Drawing.Point(466, 129);
            this.v327.Name = "v327";
            this.v327.Size = new System.Drawing.Size(24, 21);
            this.v327.TabIndex = 327;
            this.v327.Text = "0";
            this.v327.TextAlign = HorizontalAlignment.Center;
            // 
            // v326
            // 
            this.v326.BorderStyle = BorderStyle.FixedSingle;
            this.v326.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v326.Location = new System.Drawing.Point(441, 129);
            this.v326.Name = "v326";
            this.v326.Size = new System.Drawing.Size(24, 21);
            this.v326.TabIndex = 326;
            this.v326.Text = "0";
            this.v326.TextAlign = HorizontalAlignment.Center;
            // 
            // v325
            // 
            this.v325.BorderStyle = BorderStyle.FixedSingle;
            this.v325.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v325.Location = new System.Drawing.Point(416, 129);
            this.v325.Name = "v325";
            this.v325.Size = new System.Drawing.Size(24, 21);
            this.v325.TabIndex = 325;
            this.v325.Text = "0";
            this.v325.TextAlign = HorizontalAlignment.Center;
            // 
            // v324
            // 
            this.v324.BorderStyle = BorderStyle.FixedSingle;
            this.v324.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v324.Location = new System.Drawing.Point(391, 129);
            this.v324.Name = "v324";
            this.v324.Size = new System.Drawing.Size(24, 21);
            this.v324.TabIndex = 324;
            this.v324.Text = "0";
            this.v324.TextAlign = HorizontalAlignment.Center;
            // 
            // v323
            // 
            this.v323.BorderStyle = BorderStyle.FixedSingle;
            this.v323.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v323.Location = new System.Drawing.Point(366, 129);
            this.v323.Name = "v323";
            this.v323.Size = new System.Drawing.Size(24, 21);
            this.v323.TabIndex = 323;
            this.v323.Text = "0";
            this.v323.TextAlign = HorizontalAlignment.Center;
            // 
            // v322
            // 
            this.v322.BorderStyle = BorderStyle.FixedSingle;
            this.v322.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v322.Location = new System.Drawing.Point(341, 129);
            this.v322.Name = "v322";
            this.v322.Size = new System.Drawing.Size(24, 21);
            this.v322.TabIndex = 322;
            this.v322.Text = "0";
            this.v322.TextAlign = HorizontalAlignment.Center;
            // 
            // v321
            // 
            this.v321.BorderStyle = BorderStyle.FixedSingle;
            this.v321.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v321.Location = new System.Drawing.Point(316, 129);
            this.v321.Name = "v321";
            this.v321.Size = new System.Drawing.Size(24, 21);
            this.v321.TabIndex = 321;
            this.v321.Text = "0";
            this.v321.TextAlign = HorizontalAlignment.Center;
            // 
            // v229
            // 
            this.v229.BorderStyle = BorderStyle.FixedSingle;
            this.v229.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v229.Location = new System.Drawing.Point(516, 107);
            this.v229.Name = "v229";
            this.v229.Size = new System.Drawing.Size(24, 21);
            this.v229.TabIndex = 229;
            this.v229.Text = "0";
            this.v229.TextAlign = HorizontalAlignment.Center;
            // 
            // v228
            // 
            this.v228.BorderStyle = BorderStyle.FixedSingle;
            this.v228.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v228.Location = new System.Drawing.Point(491, 107);
            this.v228.Name = "v228";
            this.v228.Size = new System.Drawing.Size(24, 21);
            this.v228.TabIndex = 228;
            this.v228.Text = "0";
            this.v228.TextAlign = HorizontalAlignment.Center;
            // 
            // v227
            // 
            this.v227.BorderStyle = BorderStyle.FixedSingle;
            this.v227.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v227.Location = new System.Drawing.Point(466, 107);
            this.v227.Name = "v227";
            this.v227.Size = new System.Drawing.Size(24, 21);
            this.v227.TabIndex = 227;
            this.v227.Text = "0";
            this.v227.TextAlign = HorizontalAlignment.Center;
            // 
            // v226
            // 
            this.v226.BorderStyle = BorderStyle.FixedSingle;
            this.v226.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v226.Location = new System.Drawing.Point(441, 107);
            this.v226.Name = "v226";
            this.v226.Size = new System.Drawing.Size(24, 21);
            this.v226.TabIndex = 226;
            this.v226.Text = "0";
            this.v226.TextAlign = HorizontalAlignment.Center;
            // 
            // v225
            // 
            this.v225.BorderStyle = BorderStyle.FixedSingle;
            this.v225.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v225.Location = new System.Drawing.Point(416, 107);
            this.v225.Name = "v225";
            this.v225.Size = new System.Drawing.Size(24, 21);
            this.v225.TabIndex = 225;
            this.v225.Text = "0";
            this.v225.TextAlign = HorizontalAlignment.Center;
            // 
            // v224
            // 
            this.v224.BorderStyle = BorderStyle.FixedSingle;
            this.v224.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v224.Location = new System.Drawing.Point(391, 107);
            this.v224.Name = "v224";
            this.v224.Size = new System.Drawing.Size(24, 21);
            this.v224.TabIndex = 224;
            this.v224.Text = "0";
            this.v224.TextAlign = HorizontalAlignment.Center;
            // 
            // v223
            // 
            this.v223.BorderStyle = BorderStyle.FixedSingle;
            this.v223.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v223.Location = new System.Drawing.Point(366, 107);
            this.v223.Name = "v223";
            this.v223.Size = new System.Drawing.Size(24, 21);
            this.v223.TabIndex = 223;
            this.v223.Text = "0";
            this.v223.TextAlign = HorizontalAlignment.Center;
            // 
            // v222
            // 
            this.v222.BorderStyle = BorderStyle.FixedSingle;
            this.v222.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v222.Location = new System.Drawing.Point(341, 107);
            this.v222.Name = "v222";
            this.v222.Size = new System.Drawing.Size(24, 21);
            this.v222.TabIndex = 222;
            this.v222.Text = "0";
            this.v222.TextAlign = HorizontalAlignment.Center;
            // 
            // v221
            // 
            this.v221.BorderStyle = BorderStyle.FixedSingle;
            this.v221.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v221.Location = new System.Drawing.Point(316, 107);
            this.v221.Name = "v221";
            this.v221.Size = new System.Drawing.Size(24, 21);
            this.v221.TabIndex = 221;
            this.v221.Text = "0";
            this.v221.TextAlign = HorizontalAlignment.Center;
            // 
            // v028
            // 
            this.v028.BorderStyle = BorderStyle.FixedSingle;
            this.v028.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v028.Location = new System.Drawing.Point(491, 63);
            this.v028.Name = "v028";
            this.v028.Size = new System.Drawing.Size(24, 21);
            this.v028.TabIndex = 28;
            this.v028.Text = "0";
            this.v028.TextAlign = HorizontalAlignment.Center;
            // 
            // v026
            // 
            this.v026.BorderStyle = BorderStyle.FixedSingle;
            this.v026.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v026.Location = new System.Drawing.Point(441, 63);
            this.v026.Name = "v026";
            this.v026.Size = new System.Drawing.Size(24, 21);
            this.v026.TabIndex = 26;
            this.v026.Text = "0";
            this.v026.TextAlign = HorizontalAlignment.Center;
            // 
            // label12
            // 
            this.label12.BackColor = System.Drawing.Color.AntiqueWhite;
            this.label12.BorderStyle = BorderStyle.FixedSingle;
            this.label12.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(416, 37);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(24, 24);
            this.label12.TabIndex = 965;
            this.label12.Text = "xx";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label18
            // 
            this.label18.BackColor = System.Drawing.Color.AntiqueWhite;
            this.label18.BorderStyle = BorderStyle.FixedSingle;
            this.label18.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(391, 37);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(24, 24);
            this.label18.TabIndex = 964;
            this.label18.Text = "x1";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label20
            // 
            this.label20.BackColor = System.Drawing.Color.AntiqueWhite;
            this.label20.BorderStyle = BorderStyle.FixedSingle;
            this.label20.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(366, 37);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(24, 24);
            this.label20.TabIndex = 963;
            this.label20.Text = "12";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label22
            // 
            this.label22.BackColor = System.Drawing.Color.AntiqueWhite;
            this.label22.BorderStyle = BorderStyle.FixedSingle;
            this.label22.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.Location = new System.Drawing.Point(341, 37);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(24, 24);
            this.label22.TabIndex = 962;
            this.label22.Text = "1x";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label24
            // 
            this.label24.BackColor = System.Drawing.Color.AntiqueWhite;
            this.label24.BorderStyle = BorderStyle.FixedSingle;
            this.label24.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.Location = new System.Drawing.Point(316, 37);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(24, 24);
            this.label24.TabIndex = 961;
            this.label24.Text = "11";
            this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label26
            // 
            this.label26.BackColor = System.Drawing.Color.AntiqueWhite;
            this.label26.BorderStyle = BorderStyle.FixedSingle;
            this.label26.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.Location = new System.Drawing.Point(516, 37);
            this.label26.Name = "label26";
            this.label26.Size = new System.Drawing.Size(24, 24);
            this.label26.TabIndex = 969;
            this.label26.Text = "22";
            this.label26.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label27
            // 
            this.label27.BackColor = System.Drawing.Color.AntiqueWhite;
            this.label27.BorderStyle = BorderStyle.FixedSingle;
            this.label27.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label27.Location = new System.Drawing.Point(491, 37);
            this.label27.Name = "label27";
            this.label27.Size = new System.Drawing.Size(24, 24);
            this.label27.TabIndex = 968;
            this.label27.Text = "2x";
            this.label27.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label28
            // 
            this.label28.BackColor = System.Drawing.Color.AntiqueWhite;
            this.label28.BorderStyle = BorderStyle.FixedSingle;
            this.label28.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label28.Location = new System.Drawing.Point(466, 37);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(24, 24);
            this.label28.TabIndex = 967;
            this.label28.Text = "21";
            this.label28.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label29
            // 
            this.label29.BackColor = System.Drawing.Color.AntiqueWhite;
            this.label29.BorderStyle = BorderStyle.FixedSingle;
            this.label29.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label29.Location = new System.Drawing.Point(441, 37);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(24, 24);
            this.label29.TabIndex = 966;
            this.label29.Text = "x2";
            this.label29.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // p93
            // 
            this.p93.BackColor = System.Drawing.Color.LemonChiffon;
            this.p93.BorderStyle = BorderStyle.FixedSingle;
            this.p93.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p93.Location = new System.Drawing.Point(57, 260);
            this.p93.Name = "p93";
            this.p93.Size = new System.Drawing.Size(24, 21);
            this.p93.TabIndex = 903;
            this.p93.Text = "0";
            this.p93.TextAlign = HorizontalAlignment.Center;
            // 
            // p83
            // 
            this.p83.BackColor = System.Drawing.Color.LemonChiffon;
            this.p83.BorderStyle = BorderStyle.FixedSingle;
            this.p83.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p83.Location = new System.Drawing.Point(57, 238);
            this.p83.Name = "p83";
            this.p83.Size = new System.Drawing.Size(24, 21);
            this.p83.TabIndex = 803;
            this.p83.Text = "0";
            this.p83.TextAlign = HorizontalAlignment.Center;
            // 
            // p73
            // 
            this.p73.BackColor = System.Drawing.Color.LemonChiffon;
            this.p73.BorderStyle = BorderStyle.FixedSingle;
            this.p73.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p73.Location = new System.Drawing.Point(57, 216);
            this.p73.Name = "p73";
            this.p73.Size = new System.Drawing.Size(24, 21);
            this.p73.TabIndex = 703;
            this.p73.Text = "0";
            this.p73.TextAlign = HorizontalAlignment.Center;
            // 
            // p63
            // 
            this.p63.BackColor = System.Drawing.Color.LemonChiffon;
            this.p63.BorderStyle = BorderStyle.FixedSingle;
            this.p63.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p63.Location = new System.Drawing.Point(57, 194);
            this.p63.Name = "p63";
            this.p63.Size = new System.Drawing.Size(24, 21);
            this.p63.TabIndex = 603;
            this.p63.Text = "0";
            this.p63.TextAlign = HorizontalAlignment.Center;
            // 
            // p33
            // 
            this.p33.BackColor = System.Drawing.Color.LemonChiffon;
            this.p33.BorderStyle = BorderStyle.FixedSingle;
            this.p33.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p33.Location = new System.Drawing.Point(57, 128);
            this.p33.Name = "p33";
            this.p33.Size = new System.Drawing.Size(24, 21);
            this.p33.TabIndex = 303;
            this.p33.Text = "0";
            this.p33.TextAlign = HorizontalAlignment.Center;
            // 
            // p23
            // 
            this.p23.BackColor = System.Drawing.Color.LemonChiffon;
            this.p23.BorderStyle = BorderStyle.FixedSingle;
            this.p23.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p23.Location = new System.Drawing.Point(57, 106);
            this.p23.Name = "p23";
            this.p23.Size = new System.Drawing.Size(24, 21);
            this.p23.TabIndex = 203;
            this.p23.Text = "0";
            this.p23.TextAlign = HorizontalAlignment.Center;
            // 
            // p13
            // 
            this.p13.BackColor = System.Drawing.Color.LemonChiffon;
            this.p13.BorderStyle = BorderStyle.FixedSingle;
            this.p13.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p13.Location = new System.Drawing.Point(57, 84);
            this.p13.Name = "p13";
            this.p13.Size = new System.Drawing.Size(24, 21);
            this.p13.TabIndex = 103;
            this.p13.Text = "0";
            this.p13.TextAlign = HorizontalAlignment.Center;
            // 
            // p03
            // 
            this.p03.BackColor = System.Drawing.Color.LemonChiffon;
            this.p03.BorderStyle = BorderStyle.FixedSingle;
            this.p03.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p03.Location = new System.Drawing.Point(57, 62);
            this.p03.Name = "p03";
            this.p03.Size = new System.Drawing.Size(24, 21);
            this.p03.TabIndex = 3;
            this.p03.Text = "0";
            this.p03.TextAlign = HorizontalAlignment.Center;
            // 
            // v917
            // 
            this.v917.BackColor = System.Drawing.Color.Khaki;
            this.v917.BorderStyle = BorderStyle.FixedSingle;
            this.v917.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v917.Location = new System.Drawing.Point(238, 261);
            this.v917.Name = "v917";
            this.v917.Size = new System.Drawing.Size(24, 21);
            this.v917.TabIndex = 917;
            this.v917.Text = "0";
            this.v917.TextAlign = HorizontalAlignment.Center;
            // 
            // v916
            // 
            this.v916.BackColor = System.Drawing.Color.Khaki;
            this.v916.BorderStyle = BorderStyle.FixedSingle;
            this.v916.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v916.Location = new System.Drawing.Point(213, 261);
            this.v916.Name = "v916";
            this.v916.Size = new System.Drawing.Size(24, 21);
            this.v916.TabIndex = 916;
            this.v916.Text = "0";
            this.v916.TextAlign = HorizontalAlignment.Center;
            // 
            // p92
            // 
            this.p92.BackColor = System.Drawing.Color.LemonChiffon;
            this.p92.BorderStyle = BorderStyle.FixedSingle;
            this.p92.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p92.Location = new System.Drawing.Point(32, 260);
            this.p92.Name = "p92";
            this.p92.Size = new System.Drawing.Size(24, 21);
            this.p92.TabIndex = 902;
            this.p92.Text = "0";
            this.p92.TextAlign = HorizontalAlignment.Center;
            // 
            // p91
            // 
            this.p91.BackColor = System.Drawing.Color.LemonChiffon;
            this.p91.BorderStyle = BorderStyle.FixedSingle;
            this.p91.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p91.Location = new System.Drawing.Point(7, 260);
            this.p91.Name = "p91";
            this.p91.Size = new System.Drawing.Size(24, 21);
            this.p91.TabIndex = 901;
            this.p91.Text = "0";
            this.p91.TextAlign = HorizontalAlignment.Center;
            // 
            // p82
            // 
            this.p82.BackColor = System.Drawing.Color.LemonChiffon;
            this.p82.BorderStyle = BorderStyle.FixedSingle;
            this.p82.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p82.Location = new System.Drawing.Point(32, 238);
            this.p82.Name = "p82";
            this.p82.Size = new System.Drawing.Size(24, 21);
            this.p82.TabIndex = 802;
            this.p82.Text = "0";
            this.p82.TextAlign = HorizontalAlignment.Center;
            // 
            // p81
            // 
            this.p81.BackColor = System.Drawing.Color.LemonChiffon;
            this.p81.BorderStyle = BorderStyle.FixedSingle;
            this.p81.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p81.Location = new System.Drawing.Point(7, 238);
            this.p81.Name = "p81";
            this.p81.Size = new System.Drawing.Size(24, 21);
            this.p81.TabIndex = 801;
            this.p81.Text = "0";
            this.p81.TextAlign = HorizontalAlignment.Center;
            // 
            // p72
            // 
            this.p72.BackColor = System.Drawing.Color.LemonChiffon;
            this.p72.BorderStyle = BorderStyle.FixedSingle;
            this.p72.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p72.Location = new System.Drawing.Point(32, 216);
            this.p72.Name = "p72";
            this.p72.Size = new System.Drawing.Size(24, 21);
            this.p72.TabIndex = 702;
            this.p72.Text = "0";
            this.p72.TextAlign = HorizontalAlignment.Center;
            // 
            // p71
            // 
            this.p71.BackColor = System.Drawing.Color.LemonChiffon;
            this.p71.BorderStyle = BorderStyle.FixedSingle;
            this.p71.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p71.Location = new System.Drawing.Point(7, 216);
            this.p71.Name = "p71";
            this.p71.Size = new System.Drawing.Size(24, 21);
            this.p71.TabIndex = 701;
            this.p71.Text = "0";
            this.p71.TextAlign = HorizontalAlignment.Center;
            // 
            // v619
            // 
            this.v619.BackColor = System.Drawing.Color.Khaki;
            this.v619.BorderStyle = BorderStyle.FixedSingle;
            this.v619.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v619.Location = new System.Drawing.Point(288, 195);
            this.v619.Name = "v619";
            this.v619.Size = new System.Drawing.Size(24, 21);
            this.v619.TabIndex = 619;
            this.v619.Text = "0";
            this.v619.TextAlign = HorizontalAlignment.Center;
            // 
            // v618
            // 
            this.v618.BackColor = System.Drawing.Color.Khaki;
            this.v618.BorderStyle = BorderStyle.FixedSingle;
            this.v618.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v618.Location = new System.Drawing.Point(263, 195);
            this.v618.Name = "v618";
            this.v618.Size = new System.Drawing.Size(24, 21);
            this.v618.TabIndex = 618;
            this.v618.Text = "0";
            this.v618.TextAlign = HorizontalAlignment.Center;
            // 
            // v617
            // 
            this.v617.BackColor = System.Drawing.Color.Khaki;
            this.v617.BorderStyle = BorderStyle.FixedSingle;
            this.v617.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v617.Location = new System.Drawing.Point(238, 195);
            this.v617.Name = "v617";
            this.v617.Size = new System.Drawing.Size(24, 21);
            this.v617.TabIndex = 617;
            this.v617.Text = "0";
            this.v617.TextAlign = HorizontalAlignment.Center;
            // 
            // v616
            // 
            this.v616.BackColor = System.Drawing.Color.Khaki;
            this.v616.BorderStyle = BorderStyle.FixedSingle;
            this.v616.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v616.Location = new System.Drawing.Point(213, 195);
            this.v616.Name = "v616";
            this.v616.Size = new System.Drawing.Size(24, 21);
            this.v616.TabIndex = 616;
            this.v616.Text = "0";
            this.v616.TextAlign = HorizontalAlignment.Center;
            // 
            // v615
            // 
            this.v615.BackColor = System.Drawing.Color.Khaki;
            this.v615.BorderStyle = BorderStyle.FixedSingle;
            this.v615.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v615.Location = new System.Drawing.Point(188, 195);
            this.v615.Name = "v615";
            this.v615.Size = new System.Drawing.Size(24, 21);
            this.v615.TabIndex = 615;
            this.v615.Text = "0";
            this.v615.TextAlign = HorizontalAlignment.Center;
            // 
            // v614
            // 
            this.v614.BackColor = System.Drawing.Color.Khaki;
            this.v614.BorderStyle = BorderStyle.FixedSingle;
            this.v614.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v614.Location = new System.Drawing.Point(163, 195);
            this.v614.Name = "v614";
            this.v614.Size = new System.Drawing.Size(24, 21);
            this.v614.TabIndex = 614;
            this.v614.Text = "0";
            this.v614.TextAlign = HorizontalAlignment.Center;
            // 
            // v613
            // 
            this.v613.BackColor = System.Drawing.Color.Khaki;
            this.v613.BorderStyle = BorderStyle.FixedSingle;
            this.v613.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v613.Location = new System.Drawing.Point(138, 195);
            this.v613.Name = "v613";
            this.v613.Size = new System.Drawing.Size(24, 21);
            this.v613.TabIndex = 613;
            this.v613.Text = "0";
            this.v613.TextAlign = HorizontalAlignment.Center;
            // 
            // v612
            // 
            this.v612.BackColor = System.Drawing.Color.Khaki;
            this.v612.BorderStyle = BorderStyle.FixedSingle;
            this.v612.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v612.Location = new System.Drawing.Point(113, 195);
            this.v612.Name = "v612";
            this.v612.Size = new System.Drawing.Size(24, 21);
            this.v612.TabIndex = 612;
            this.v612.Text = "0";
            this.v612.TextAlign = HorizontalAlignment.Center;
            // 
            // v611
            // 
            this.v611.BackColor = System.Drawing.Color.Khaki;
            this.v611.BorderStyle = BorderStyle.FixedSingle;
            this.v611.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v611.Location = new System.Drawing.Point(88, 195);
            this.v611.Name = "v611";
            this.v611.Size = new System.Drawing.Size(24, 21);
            this.v611.TabIndex = 611;
            this.v611.Text = "0";
            this.v611.TextAlign = HorizontalAlignment.Center;
            // 
            // p62
            // 
            this.p62.BackColor = System.Drawing.Color.LemonChiffon;
            this.p62.BorderStyle = BorderStyle.FixedSingle;
            this.p62.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p62.Location = new System.Drawing.Point(32, 194);
            this.p62.Name = "p62";
            this.p62.Size = new System.Drawing.Size(24, 21);
            this.p62.TabIndex = 602;
            this.p62.Text = "0";
            this.p62.TextAlign = HorizontalAlignment.Center;
            // 
            // p61
            // 
            this.p61.BackColor = System.Drawing.Color.LemonChiffon;
            this.p61.BorderStyle = BorderStyle.FixedSingle;
            this.p61.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p61.Location = new System.Drawing.Point(7, 194);
            this.p61.Name = "p61";
            this.p61.Size = new System.Drawing.Size(24, 21);
            this.p61.TabIndex = 601;
            this.p61.Text = "0";
            this.p61.TextAlign = HorizontalAlignment.Center;
            // 
            // v519
            // 
            this.v519.BackColor = System.Drawing.Color.Khaki;
            this.v519.BorderStyle = BorderStyle.FixedSingle;
            this.v519.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v519.Location = new System.Drawing.Point(288, 173);
            this.v519.Name = "v519";
            this.v519.Size = new System.Drawing.Size(24, 21);
            this.v519.TabIndex = 519;
            this.v519.Text = "0";
            this.v519.TextAlign = HorizontalAlignment.Center;
            // 
            // v518
            // 
            this.v518.BackColor = System.Drawing.Color.Khaki;
            this.v518.BorderStyle = BorderStyle.FixedSingle;
            this.v518.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v518.Location = new System.Drawing.Point(263, 173);
            this.v518.Name = "v518";
            this.v518.Size = new System.Drawing.Size(24, 21);
            this.v518.TabIndex = 518;
            this.v518.Text = "0";
            this.v518.TextAlign = HorizontalAlignment.Center;
            // 
            // v517
            // 
            this.v517.BackColor = System.Drawing.Color.Khaki;
            this.v517.BorderStyle = BorderStyle.FixedSingle;
            this.v517.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v517.Location = new System.Drawing.Point(238, 173);
            this.v517.Name = "v517";
            this.v517.Size = new System.Drawing.Size(24, 21);
            this.v517.TabIndex = 517;
            this.v517.Text = "0";
            this.v517.TextAlign = HorizontalAlignment.Center;
            // 
            // v516
            // 
            this.v516.BackColor = System.Drawing.Color.Khaki;
            this.v516.BorderStyle = BorderStyle.FixedSingle;
            this.v516.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v516.Location = new System.Drawing.Point(213, 173);
            this.v516.Name = "v516";
            this.v516.Size = new System.Drawing.Size(24, 21);
            this.v516.TabIndex = 516;
            this.v516.Text = "0";
            this.v516.TextAlign = HorizontalAlignment.Center;
            // 
            // v515
            // 
            this.v515.BackColor = System.Drawing.Color.Khaki;
            this.v515.BorderStyle = BorderStyle.FixedSingle;
            this.v515.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v515.Location = new System.Drawing.Point(188, 173);
            this.v515.Name = "v515";
            this.v515.Size = new System.Drawing.Size(24, 21);
            this.v515.TabIndex = 515;
            this.v515.Text = "0";
            this.v515.TextAlign = HorizontalAlignment.Center;
            // 
            // v514
            // 
            this.v514.BackColor = System.Drawing.Color.Khaki;
            this.v514.BorderStyle = BorderStyle.FixedSingle;
            this.v514.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v514.Location = new System.Drawing.Point(163, 173);
            this.v514.Name = "v514";
            this.v514.Size = new System.Drawing.Size(24, 21);
            this.v514.TabIndex = 514;
            this.v514.Text = "0";
            this.v514.TextAlign = HorizontalAlignment.Center;
            // 
            // v513
            // 
            this.v513.BackColor = System.Drawing.Color.Khaki;
            this.v513.BorderStyle = BorderStyle.FixedSingle;
            this.v513.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v513.Location = new System.Drawing.Point(138, 173);
            this.v513.Name = "v513";
            this.v513.Size = new System.Drawing.Size(24, 21);
            this.v513.TabIndex = 513;
            this.v513.Text = "0";
            this.v513.TextAlign = HorizontalAlignment.Center;
            // 
            // v512
            // 
            this.v512.BackColor = System.Drawing.Color.Khaki;
            this.v512.BorderStyle = BorderStyle.FixedSingle;
            this.v512.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v512.Location = new System.Drawing.Point(113, 173);
            this.v512.Name = "v512";
            this.v512.Size = new System.Drawing.Size(24, 21);
            this.v512.TabIndex = 512;
            this.v512.Text = "0";
            this.v512.TextAlign = HorizontalAlignment.Center;
            // 
            // v419
            // 
            this.v419.BackColor = System.Drawing.Color.Khaki;
            this.v419.BorderStyle = BorderStyle.FixedSingle;
            this.v419.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v419.Location = new System.Drawing.Point(288, 151);
            this.v419.Name = "v419";
            this.v419.Size = new System.Drawing.Size(24, 21);
            this.v419.TabIndex = 419;
            this.v419.Text = "0";
            this.v419.TextAlign = HorizontalAlignment.Center;
            // 
            // v417
            // 
            this.v417.BackColor = System.Drawing.Color.Khaki;
            this.v417.BorderStyle = BorderStyle.FixedSingle;
            this.v417.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v417.Location = new System.Drawing.Point(238, 151);
            this.v417.Name = "v417";
            this.v417.Size = new System.Drawing.Size(24, 21);
            this.v417.TabIndex = 417;
            this.v417.Text = "0";
            this.v417.TextAlign = HorizontalAlignment.Center;
            // 
            // v414
            // 
            this.v414.BackColor = System.Drawing.Color.Khaki;
            this.v414.BorderStyle = BorderStyle.FixedSingle;
            this.v414.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v414.Location = new System.Drawing.Point(163, 151);
            this.v414.Name = "v414";
            this.v414.Size = new System.Drawing.Size(24, 21);
            this.v414.TabIndex = 414;
            this.v414.Text = "0";
            this.v414.TextAlign = HorizontalAlignment.Center;
            // 
            // v413
            // 
            this.v413.BackColor = System.Drawing.Color.Khaki;
            this.v413.BorderStyle = BorderStyle.FixedSingle;
            this.v413.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v413.Location = new System.Drawing.Point(138, 151);
            this.v413.Name = "v413";
            this.v413.Size = new System.Drawing.Size(24, 21);
            this.v413.TabIndex = 413;
            this.v413.Text = "0";
            this.v413.TextAlign = HorizontalAlignment.Center;
            // 
            // v412
            // 
            this.v412.BackColor = System.Drawing.Color.Khaki;
            this.v412.BorderStyle = BorderStyle.FixedSingle;
            this.v412.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v412.Location = new System.Drawing.Point(113, 151);
            this.v412.Name = "v412";
            this.v412.Size = new System.Drawing.Size(24, 21);
            this.v412.TabIndex = 412;
            this.v412.Text = "0";
            this.v412.TextAlign = HorizontalAlignment.Center;
            // 
            // v319
            // 
            this.v319.BackColor = System.Drawing.Color.Khaki;
            this.v319.BorderStyle = BorderStyle.FixedSingle;
            this.v319.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v319.Location = new System.Drawing.Point(288, 129);
            this.v319.Name = "v319";
            this.v319.Size = new System.Drawing.Size(24, 21);
            this.v319.TabIndex = 319;
            this.v319.Text = "0";
            this.v319.TextAlign = HorizontalAlignment.Center;
            // 
            // v318
            // 
            this.v318.BackColor = System.Drawing.Color.Khaki;
            this.v318.BorderStyle = BorderStyle.FixedSingle;
            this.v318.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v318.Location = new System.Drawing.Point(263, 129);
            this.v318.Name = "v318";
            this.v318.Size = new System.Drawing.Size(24, 21);
            this.v318.TabIndex = 318;
            this.v318.Text = "0";
            this.v318.TextAlign = HorizontalAlignment.Center;
            // 
            // v317
            // 
            this.v317.BackColor = System.Drawing.Color.Khaki;
            this.v317.BorderStyle = BorderStyle.FixedSingle;
            this.v317.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v317.Location = new System.Drawing.Point(238, 129);
            this.v317.Name = "v317";
            this.v317.Size = new System.Drawing.Size(24, 21);
            this.v317.TabIndex = 317;
            this.v317.Text = "0";
            this.v317.TextAlign = HorizontalAlignment.Center;
            // 
            // v316
            // 
            this.v316.BackColor = System.Drawing.Color.Khaki;
            this.v316.BorderStyle = BorderStyle.FixedSingle;
            this.v316.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v316.Location = new System.Drawing.Point(213, 129);
            this.v316.Name = "v316";
            this.v316.Size = new System.Drawing.Size(24, 21);
            this.v316.TabIndex = 316;
            this.v316.Text = "0";
            this.v316.TextAlign = HorizontalAlignment.Center;
            // 
            // v315
            // 
            this.v315.BackColor = System.Drawing.Color.Khaki;
            this.v315.BorderStyle = BorderStyle.FixedSingle;
            this.v315.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v315.Location = new System.Drawing.Point(188, 129);
            this.v315.Name = "v315";
            this.v315.Size = new System.Drawing.Size(24, 21);
            this.v315.TabIndex = 315;
            this.v315.Text = "0";
            this.v315.TextAlign = HorizontalAlignment.Center;
            // 
            // v314
            // 
            this.v314.BackColor = System.Drawing.Color.Khaki;
            this.v314.BorderStyle = BorderStyle.FixedSingle;
            this.v314.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v314.Location = new System.Drawing.Point(163, 129);
            this.v314.Name = "v314";
            this.v314.Size = new System.Drawing.Size(24, 21);
            this.v314.TabIndex = 314;
            this.v314.Text = "0";
            this.v314.TextAlign = HorizontalAlignment.Center;
            // 
            // v313
            // 
            this.v313.BackColor = System.Drawing.Color.Khaki;
            this.v313.BorderStyle = BorderStyle.FixedSingle;
            this.v313.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v313.Location = new System.Drawing.Point(138, 129);
            this.v313.Name = "v313";
            this.v313.Size = new System.Drawing.Size(24, 21);
            this.v313.TabIndex = 313;
            this.v313.Text = "0";
            this.v313.TextAlign = HorizontalAlignment.Center;
            // 
            // v312
            // 
            this.v312.BackColor = System.Drawing.Color.Khaki;
            this.v312.BorderStyle = BorderStyle.FixedSingle;
            this.v312.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v312.Location = new System.Drawing.Point(113, 129);
            this.v312.Name = "v312";
            this.v312.Size = new System.Drawing.Size(24, 21);
            this.v312.TabIndex = 312;
            this.v312.Text = "0";
            this.v312.TextAlign = HorizontalAlignment.Center;
            // 
            // v311
            // 
            this.v311.BackColor = System.Drawing.Color.Khaki;
            this.v311.BorderStyle = BorderStyle.FixedSingle;
            this.v311.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v311.Location = new System.Drawing.Point(88, 129);
            this.v311.Name = "v311";
            this.v311.Size = new System.Drawing.Size(24, 21);
            this.v311.TabIndex = 311;
            this.v311.Text = "0";
            this.v311.TextAlign = HorizontalAlignment.Center;
            // 
            // p32
            // 
            this.p32.BackColor = System.Drawing.Color.LemonChiffon;
            this.p32.BorderStyle = BorderStyle.FixedSingle;
            this.p32.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p32.Location = new System.Drawing.Point(32, 128);
            this.p32.Name = "p32";
            this.p32.Size = new System.Drawing.Size(24, 21);
            this.p32.TabIndex = 302;
            this.p32.Text = "0";
            this.p32.TextAlign = HorizontalAlignment.Center;
            // 
            // p31
            // 
            this.p31.BackColor = System.Drawing.Color.LemonChiffon;
            this.p31.BorderStyle = BorderStyle.FixedSingle;
            this.p31.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p31.Location = new System.Drawing.Point(7, 128);
            this.p31.Name = "p31";
            this.p31.Size = new System.Drawing.Size(24, 21);
            this.p31.TabIndex = 301;
            this.p31.Text = "0";
            this.p31.TextAlign = HorizontalAlignment.Center;
            // 
            // v219
            // 
            this.v219.BackColor = System.Drawing.Color.Khaki;
            this.v219.BorderStyle = BorderStyle.FixedSingle;
            this.v219.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v219.Location = new System.Drawing.Point(288, 107);
            this.v219.Name = "v219";
            this.v219.Size = new System.Drawing.Size(24, 21);
            this.v219.TabIndex = 219;
            this.v219.Text = "0";
            this.v219.TextAlign = HorizontalAlignment.Center;
            // 
            // v218
            // 
            this.v218.BackColor = System.Drawing.Color.Khaki;
            this.v218.BorderStyle = BorderStyle.FixedSingle;
            this.v218.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v218.Location = new System.Drawing.Point(263, 107);
            this.v218.Name = "v218";
            this.v218.Size = new System.Drawing.Size(24, 21);
            this.v218.TabIndex = 218;
            this.v218.Text = "0";
            this.v218.TextAlign = HorizontalAlignment.Center;
            // 
            // v217
            // 
            this.v217.BackColor = System.Drawing.Color.Khaki;
            this.v217.BorderStyle = BorderStyle.FixedSingle;
            this.v217.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v217.Location = new System.Drawing.Point(238, 107);
            this.v217.Name = "v217";
            this.v217.Size = new System.Drawing.Size(24, 21);
            this.v217.TabIndex = 217;
            this.v217.Text = "0";
            this.v217.TextAlign = HorizontalAlignment.Center;
            // 
            // v216
            // 
            this.v216.BackColor = System.Drawing.Color.Khaki;
            this.v216.BorderStyle = BorderStyle.FixedSingle;
            this.v216.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v216.Location = new System.Drawing.Point(213, 107);
            this.v216.Name = "v216";
            this.v216.Size = new System.Drawing.Size(24, 21);
            this.v216.TabIndex = 216;
            this.v216.Text = "0";
            this.v216.TextAlign = HorizontalAlignment.Center;
            // 
            // v215
            // 
            this.v215.BackColor = System.Drawing.Color.Khaki;
            this.v215.BorderStyle = BorderStyle.FixedSingle;
            this.v215.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v215.Location = new System.Drawing.Point(188, 107);
            this.v215.Name = "v215";
            this.v215.Size = new System.Drawing.Size(24, 21);
            this.v215.TabIndex = 215;
            this.v215.Text = "0";
            this.v215.TextAlign = HorizontalAlignment.Center;
            // 
            // v214
            // 
            this.v214.BackColor = System.Drawing.Color.Khaki;
            this.v214.BorderStyle = BorderStyle.FixedSingle;
            this.v214.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v214.Location = new System.Drawing.Point(163, 107);
            this.v214.Name = "v214";
            this.v214.Size = new System.Drawing.Size(24, 21);
            this.v214.TabIndex = 214;
            this.v214.Text = "0";
            this.v214.TextAlign = HorizontalAlignment.Center;
            // 
            // v213
            // 
            this.v213.BackColor = System.Drawing.Color.Khaki;
            this.v213.BorderStyle = BorderStyle.FixedSingle;
            this.v213.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v213.Location = new System.Drawing.Point(138, 107);
            this.v213.Name = "v213";
            this.v213.Size = new System.Drawing.Size(24, 21);
            this.v213.TabIndex = 213;
            this.v213.Text = "0";
            this.v213.TextAlign = HorizontalAlignment.Center;
            // 
            // v212
            // 
            this.v212.BackColor = System.Drawing.Color.Khaki;
            this.v212.BorderStyle = BorderStyle.FixedSingle;
            this.v212.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v212.Location = new System.Drawing.Point(113, 107);
            this.v212.Name = "v212";
            this.v212.Size = new System.Drawing.Size(24, 21);
            this.v212.TabIndex = 212;
            this.v212.Text = "0";
            this.v212.TextAlign = HorizontalAlignment.Center;
            // 
            // v211
            // 
            this.v211.BackColor = System.Drawing.Color.Khaki;
            this.v211.BorderStyle = BorderStyle.FixedSingle;
            this.v211.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v211.Location = new System.Drawing.Point(88, 107);
            this.v211.Name = "v211";
            this.v211.Size = new System.Drawing.Size(24, 21);
            this.v211.TabIndex = 211;
            this.v211.Text = "0";
            this.v211.TextAlign = HorizontalAlignment.Center;
            // 
            // p22
            // 
            this.p22.BackColor = System.Drawing.Color.LemonChiffon;
            this.p22.BorderStyle = BorderStyle.FixedSingle;
            this.p22.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p22.Location = new System.Drawing.Point(32, 106);
            this.p22.Name = "p22";
            this.p22.Size = new System.Drawing.Size(24, 21);
            this.p22.TabIndex = 202;
            this.p22.Text = "0";
            this.p22.TextAlign = HorizontalAlignment.Center;
            // 
            // p21
            // 
            this.p21.BackColor = System.Drawing.Color.LemonChiffon;
            this.p21.BorderStyle = BorderStyle.FixedSingle;
            this.p21.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p21.Location = new System.Drawing.Point(7, 106);
            this.p21.Name = "p21";
            this.p21.Size = new System.Drawing.Size(24, 21);
            this.p21.TabIndex = 201;
            this.p21.Text = "0";
            this.p21.TextAlign = HorizontalAlignment.Center;
            // 
            // v119
            // 
            this.v119.BackColor = System.Drawing.Color.Khaki;
            this.v119.BorderStyle = BorderStyle.FixedSingle;
            this.v119.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v119.Location = new System.Drawing.Point(288, 85);
            this.v119.Name = "v119";
            this.v119.Size = new System.Drawing.Size(24, 21);
            this.v119.TabIndex = 119;
            this.v119.Text = "0";
            this.v119.TextAlign = HorizontalAlignment.Center;
            // 
            // v118
            // 
            this.v118.BackColor = System.Drawing.Color.Khaki;
            this.v118.BorderStyle = BorderStyle.FixedSingle;
            this.v118.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v118.Location = new System.Drawing.Point(263, 85);
            this.v118.Name = "v118";
            this.v118.Size = new System.Drawing.Size(24, 21);
            this.v118.TabIndex = 118;
            this.v118.Text = "0";
            this.v118.TextAlign = HorizontalAlignment.Center;
            // 
            // v117
            // 
            this.v117.BackColor = System.Drawing.Color.Khaki;
            this.v117.BorderStyle = BorderStyle.FixedSingle;
            this.v117.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v117.Location = new System.Drawing.Point(238, 85);
            this.v117.Name = "v117";
            this.v117.Size = new System.Drawing.Size(24, 21);
            this.v117.TabIndex = 117;
            this.v117.Text = "0";
            this.v117.TextAlign = HorizontalAlignment.Center;
            // 
            // v116
            // 
            this.v116.BackColor = System.Drawing.Color.Khaki;
            this.v116.BorderStyle = BorderStyle.FixedSingle;
            this.v116.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v116.Location = new System.Drawing.Point(213, 85);
            this.v116.Name = "v116";
            this.v116.Size = new System.Drawing.Size(24, 21);
            this.v116.TabIndex = 116;
            this.v116.Text = "0";
            this.v116.TextAlign = HorizontalAlignment.Center;
            // 
            // v115
            // 
            this.v115.BackColor = System.Drawing.Color.Khaki;
            this.v115.BorderStyle = BorderStyle.FixedSingle;
            this.v115.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v115.Location = new System.Drawing.Point(188, 85);
            this.v115.Name = "v115";
            this.v115.Size = new System.Drawing.Size(24, 21);
            this.v115.TabIndex = 115;
            this.v115.Text = "0";
            this.v115.TextAlign = HorizontalAlignment.Center;
            // 
            // v114
            // 
            this.v114.BackColor = System.Drawing.Color.Khaki;
            this.v114.BorderStyle = BorderStyle.FixedSingle;
            this.v114.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v114.Location = new System.Drawing.Point(163, 85);
            this.v114.Name = "v114";
            this.v114.Size = new System.Drawing.Size(24, 21);
            this.v114.TabIndex = 114;
            this.v114.Text = "0";
            this.v114.TextAlign = HorizontalAlignment.Center;
            // 
            // v113
            // 
            this.v113.BackColor = System.Drawing.Color.Khaki;
            this.v113.BorderStyle = BorderStyle.FixedSingle;
            this.v113.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v113.Location = new System.Drawing.Point(138, 85);
            this.v113.Name = "v113";
            this.v113.Size = new System.Drawing.Size(24, 21);
            this.v113.TabIndex = 113;
            this.v113.Text = "0";
            this.v113.TextAlign = HorizontalAlignment.Center;
            // 
            // v112
            // 
            this.v112.BackColor = System.Drawing.Color.Khaki;
            this.v112.BorderStyle = BorderStyle.FixedSingle;
            this.v112.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v112.Location = new System.Drawing.Point(113, 85);
            this.v112.Name = "v112";
            this.v112.Size = new System.Drawing.Size(24, 21);
            this.v112.TabIndex = 112;
            this.v112.Text = "0";
            this.v112.TextAlign = HorizontalAlignment.Center;
            // 
            // v111
            // 
            this.v111.BackColor = System.Drawing.Color.Khaki;
            this.v111.BorderStyle = BorderStyle.FixedSingle;
            this.v111.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v111.Location = new System.Drawing.Point(88, 85);
            this.v111.Name = "v111";
            this.v111.Size = new System.Drawing.Size(24, 21);
            this.v111.TabIndex = 111;
            this.v111.Text = "0";
            this.v111.TextAlign = HorizontalAlignment.Center;
            // 
            // p12
            // 
            this.p12.BackColor = System.Drawing.Color.LemonChiffon;
            this.p12.BorderStyle = BorderStyle.FixedSingle;
            this.p12.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p12.Location = new System.Drawing.Point(32, 84);
            this.p12.Name = "p12";
            this.p12.Size = new System.Drawing.Size(24, 21);
            this.p12.TabIndex = 102;
            this.p12.Text = "0";
            this.p12.TextAlign = HorizontalAlignment.Center;
            // 
            // p11
            // 
            this.p11.BackColor = System.Drawing.Color.LemonChiffon;
            this.p11.BorderStyle = BorderStyle.FixedSingle;
            this.p11.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p11.Location = new System.Drawing.Point(7, 84);
            this.p11.Name = "p11";
            this.p11.Size = new System.Drawing.Size(24, 21);
            this.p11.TabIndex = 101;
            this.p11.Text = "0";
            this.p11.TextAlign = HorizontalAlignment.Center;
            // 
            // v019
            // 
            this.v019.BackColor = System.Drawing.Color.Khaki;
            this.v019.BorderStyle = BorderStyle.FixedSingle;
            this.v019.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v019.Location = new System.Drawing.Point(288, 63);
            this.v019.Name = "v019";
            this.v019.Size = new System.Drawing.Size(24, 21);
            this.v019.TabIndex = 19;
            this.v019.Text = "0";
            this.v019.TextAlign = HorizontalAlignment.Center;
            // 
            // v018
            // 
            this.v018.BackColor = System.Drawing.Color.Khaki;
            this.v018.BorderStyle = BorderStyle.FixedSingle;
            this.v018.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v018.Location = new System.Drawing.Point(263, 63);
            this.v018.Name = "v018";
            this.v018.Size = new System.Drawing.Size(24, 21);
            this.v018.TabIndex = 18;
            this.v018.Text = "0";
            this.v018.TextAlign = HorizontalAlignment.Center;
            // 
            // v017
            // 
            this.v017.BackColor = System.Drawing.Color.Khaki;
            this.v017.BorderStyle = BorderStyle.FixedSingle;
            this.v017.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v017.Location = new System.Drawing.Point(238, 63);
            this.v017.Name = "v017";
            this.v017.Size = new System.Drawing.Size(24, 21);
            this.v017.TabIndex = 17;
            this.v017.Text = "0";
            this.v017.TextAlign = HorizontalAlignment.Center;
            // 
            // v016
            // 
            this.v016.BackColor = System.Drawing.Color.Khaki;
            this.v016.BorderStyle = BorderStyle.FixedSingle;
            this.v016.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v016.Location = new System.Drawing.Point(213, 63);
            this.v016.Name = "v016";
            this.v016.Size = new System.Drawing.Size(24, 21);
            this.v016.TabIndex = 16;
            this.v016.Text = "0";
            this.v016.TextAlign = HorizontalAlignment.Center;
            // 
            // v015
            // 
            this.v015.BackColor = System.Drawing.Color.Khaki;
            this.v015.BorderStyle = BorderStyle.FixedSingle;
            this.v015.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v015.Location = new System.Drawing.Point(188, 63);
            this.v015.Name = "v015";
            this.v015.Size = new System.Drawing.Size(24, 21);
            this.v015.TabIndex = 15;
            this.v015.Text = "0";
            this.v015.TextAlign = HorizontalAlignment.Center;
            // 
            // v014
            // 
            this.v014.BackColor = System.Drawing.Color.Khaki;
            this.v014.BorderStyle = BorderStyle.FixedSingle;
            this.v014.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v014.Location = new System.Drawing.Point(163, 63);
            this.v014.Name = "v014";
            this.v014.Size = new System.Drawing.Size(24, 21);
            this.v014.TabIndex = 14;
            this.v014.Text = "0";
            this.v014.TextAlign = HorizontalAlignment.Center;
            // 
            // v013
            // 
            this.v013.BackColor = System.Drawing.Color.Khaki;
            this.v013.BorderStyle = BorderStyle.FixedSingle;
            this.v013.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v013.Location = new System.Drawing.Point(138, 63);
            this.v013.Name = "v013";
            this.v013.Size = new System.Drawing.Size(24, 21);
            this.v013.TabIndex = 13;
            this.v013.Text = "0";
            this.v013.TextAlign = HorizontalAlignment.Center;
            // 
            // v012
            // 
            this.v012.BackColor = System.Drawing.Color.Khaki;
            this.v012.BorderStyle = BorderStyle.FixedSingle;
            this.v012.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v012.Location = new System.Drawing.Point(113, 63);
            this.v012.Name = "v012";
            this.v012.Size = new System.Drawing.Size(24, 21);
            this.v012.TabIndex = 12;
            this.v012.Text = "0";
            this.v012.TextAlign = HorizontalAlignment.Center;
            // 
            // v011
            // 
            this.v011.BackColor = System.Drawing.Color.Khaki;
            this.v011.BorderStyle = BorderStyle.FixedSingle;
            this.v011.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.v011.Location = new System.Drawing.Point(88, 63);
            this.v011.Name = "v011";
            this.v011.Size = new System.Drawing.Size(24, 21);
            this.v011.TabIndex = 11;
            this.v011.Text = "0";
            this.v011.TextAlign = HorizontalAlignment.Center;
            // 
            // p02
            // 
            this.p02.BackColor = System.Drawing.Color.LemonChiffon;
            this.p02.BorderStyle = BorderStyle.FixedSingle;
            this.p02.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p02.Location = new System.Drawing.Point(32, 62);
            this.p02.Name = "p02";
            this.p02.Size = new System.Drawing.Size(24, 21);
            this.p02.TabIndex = 2;
            this.p02.Text = "0";
            this.p02.TextAlign = HorizontalAlignment.Center;
            // 
            // p01
            // 
            this.p01.BackColor = System.Drawing.Color.LemonChiffon;
            this.p01.BorderStyle = BorderStyle.FixedSingle;
            this.p01.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.p01.Location = new System.Drawing.Point(7, 62);
            this.p01.Name = "p01";
            this.p01.Size = new System.Drawing.Size(24, 21);
            this.p01.TabIndex = 1;
            this.p01.Text = "0";
            this.p01.TextAlign = HorizontalAlignment.Center;
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.AntiqueWhite;
            this.label8.BorderStyle = BorderStyle.FixedSingle;
            this.label8.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(188, 37);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(24, 24);
            this.label8.TabIndex = 955;
            this.label8.Text = "xx";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.Color.AntiqueWhite;
            this.label9.BorderStyle = BorderStyle.FixedSingle;
            this.label9.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(163, 37);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(24, 24);
            this.label9.TabIndex = 954;
            this.label9.Text = "x1";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.Color.AntiqueWhite;
            this.label10.BorderStyle = BorderStyle.FixedSingle;
            this.label10.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(138, 37);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(24, 24);
            this.label10.TabIndex = 953;
            this.label10.Text = "12";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.AntiqueWhite;
            this.label5.BorderStyle = BorderStyle.FixedSingle;
            this.label5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(113, 37);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(24, 24);
            this.label5.TabIndex = 952;
            this.label5.Text = "1x";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.AntiqueWhite;
            this.label6.BorderStyle = BorderStyle.FixedSingle;
            this.label6.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(88, 37);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(24, 24);
            this.label6.TabIndex = 950;
            this.label6.Text = "11";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.AntiqueWhite;
            this.label7.BorderStyle = BorderStyle.FixedSingle;
            this.label7.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(288, 37);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(24, 24);
            this.label7.TabIndex = 959;
            this.label7.Text = "22";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.AntiqueWhite;
            this.label4.BorderStyle = BorderStyle.FixedSingle;
            this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(263, 37);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 24);
            this.label4.TabIndex = 958;
            this.label4.Text = "2x";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.AntiqueWhite;
            this.label3.BorderStyle = BorderStyle.FixedSingle;
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(238, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(24, 24);
            this.label3.TabIndex = 957;
            this.label3.Text = "21";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.AntiqueWhite;
            this.label2.BorderStyle = BorderStyle.FixedSingle;
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(213, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(24, 24);
            this.label2.TabIndex = 956;
            this.label2.Text = "x2";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.AntiqueWhite;
            this.label1.BorderStyle = BorderStyle.FixedSingle;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(7, 37);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "Trío";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox2
            // 
            this.groupBox2.BackColor = System.Drawing.Color.Bisque;
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
            this.groupBox2.Controls.Add(this.bLeer);
            this.groupBox2.Controls.Add(this.bSalvar);
            this.groupBox2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox2.Location = new System.Drawing.Point(2, 352);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(322, 200);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Condiciones";
            // 
            // label25
            // 
            this.label25.BorderStyle = BorderStyle.FixedSingle;
            this.label25.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.ForeColor = System.Drawing.Color.Black;
            this.label25.Location = new System.Drawing.Point(41, 23);
            this.label25.Name = "label25";
            this.label25.Size = new System.Drawing.Size(81, 24);
            this.label25.TabIndex = 142;
            this.label25.Text = "Límites";
            this.label25.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // c51
            // 
            this.c51.BorderStyle = BorderStyle.FixedSingle;
            this.c51.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.c51.ForeColor = System.Drawing.Color.Black;
            this.c51.Location = new System.Drawing.Point(41, 161);
            this.c51.Name = "c51";
            this.c51.Size = new System.Drawing.Size(40, 21);
            this.c51.TabIndex = 9;
            this.c51.Text = "0";
            this.c51.TextAlign = HorizontalAlignment.Center;
            // 
            // c52
            // 
            this.c52.BorderStyle = BorderStyle.FixedSingle;
            this.c52.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.c52.ForeColor = System.Drawing.Color.Black;
            this.c52.Location = new System.Drawing.Point(82, 161);
            this.c52.Name = "c52";
            this.c52.Size = new System.Drawing.Size(40, 21);
            this.c52.TabIndex = 10;
            this.c52.Text = "270";
            this.c52.TextAlign = HorizontalAlignment.Center;
            // 
            // label23
            // 
            this.label23.BorderStyle = BorderStyle.FixedSingle;
            this.label23.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.ForeColor = System.Drawing.Color.Black;
            this.label23.Location = new System.Drawing.Point(8, 161);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(32, 21);
            this.label23.TabIndex = 139;
            this.label23.Text = "5";
            this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // r5
            // 
            this.r5.BorderStyle = BorderStyle.FixedSingle;
            this.r5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.r5.ForeColor = System.Drawing.Color.Black;
            this.r5.Location = new System.Drawing.Point(123, 161);
            this.r5.Name = "r5";
            this.r5.Size = new System.Drawing.Size(32, 21);
            this.r5.TabIndex = 138;
            this.r5.Text = "-";
            this.r5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // c42
            // 
            this.c42.BorderStyle = BorderStyle.FixedSingle;
            this.c42.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.c42.ForeColor = System.Drawing.Color.Black;
            this.c42.Location = new System.Drawing.Point(82, 139);
            this.c42.Name = "c42";
            this.c42.Size = new System.Drawing.Size(40, 21);
            this.c42.TabIndex = 8;
            this.c42.Text = "270";
            this.c42.TextAlign = HorizontalAlignment.Center;
            // 
            // label21
            // 
            this.label21.BorderStyle = BorderStyle.FixedSingle;
            this.label21.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.ForeColor = System.Drawing.Color.Black;
            this.label21.Location = new System.Drawing.Point(8, 139);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(32, 21);
            this.label21.TabIndex = 135;
            this.label21.Text = "4";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // r4
            // 
            this.r4.BorderStyle = BorderStyle.FixedSingle;
            this.r4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.r4.ForeColor = System.Drawing.Color.Black;
            this.r4.Location = new System.Drawing.Point(123, 139);
            this.r4.Name = "r4";
            this.r4.Size = new System.Drawing.Size(32, 21);
            this.r4.TabIndex = 134;
            this.r4.Text = "-";
            this.r4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // c31
            // 
            this.c31.BorderStyle = BorderStyle.FixedSingle;
            this.c31.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.c31.ForeColor = System.Drawing.Color.Black;
            this.c31.Location = new System.Drawing.Point(41, 117);
            this.c31.Name = "c31";
            this.c31.Size = new System.Drawing.Size(40, 21);
            this.c31.TabIndex = 5;
            this.c31.Text = "0";
            this.c31.TextAlign = HorizontalAlignment.Center;
            // 
            // c32
            // 
            this.c32.BorderStyle = BorderStyle.FixedSingle;
            this.c32.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.c32.ForeColor = System.Drawing.Color.Black;
            this.c32.Location = new System.Drawing.Point(82, 117);
            this.c32.Name = "c32";
            this.c32.Size = new System.Drawing.Size(40, 21);
            this.c32.TabIndex = 6;
            this.c32.Text = "270";
            this.c32.TextAlign = HorizontalAlignment.Center;
            // 
            // label19
            // 
            this.label19.BorderStyle = BorderStyle.FixedSingle;
            this.label19.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.ForeColor = System.Drawing.Color.Black;
            this.label19.Location = new System.Drawing.Point(8, 117);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(32, 21);
            this.label19.TabIndex = 131;
            this.label19.Text = "3";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // r3
            // 
            this.r3.BorderStyle = BorderStyle.FixedSingle;
            this.r3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.r3.ForeColor = System.Drawing.Color.Black;
            this.r3.Location = new System.Drawing.Point(123, 117);
            this.r3.Name = "r3";
            this.r3.Size = new System.Drawing.Size(32, 21);
            this.r3.TabIndex = 130;
            this.r3.Text = "-";
            this.r3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label11
            // 
            this.label11.BorderStyle = BorderStyle.FixedSingle;
            this.label11.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.Black;
            this.label11.Location = new System.Drawing.Point(8, 95);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(32, 21);
            this.label11.TabIndex = 127;
            this.label11.Text = "2";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // r2
            // 
            this.r2.BorderStyle = BorderStyle.FixedSingle;
            this.r2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.r2.ForeColor = System.Drawing.Color.Black;
            this.r2.Location = new System.Drawing.Point(123, 95);
            this.r2.Name = "r2";
            this.r2.Size = new System.Drawing.Size(32, 21);
            this.r2.TabIndex = 126;
            this.r2.Text = "-";
            this.r2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // c11
            // 
            this.c11.BorderStyle = BorderStyle.FixedSingle;
            this.c11.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.c11.ForeColor = System.Drawing.Color.Black;
            this.c11.Location = new System.Drawing.Point(41, 73);
            this.c11.Name = "c11";
            this.c11.Size = new System.Drawing.Size(40, 21);
            this.c11.TabIndex = 1;
            this.c11.Text = "0";
            this.c11.TextAlign = HorizontalAlignment.Center;
            // 
            // c12
            // 
            this.c12.BorderStyle = BorderStyle.FixedSingle;
            this.c12.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.c12.ForeColor = System.Drawing.Color.Black;
            this.c12.Location = new System.Drawing.Point(82, 73);
            this.c12.Name = "c12";
            this.c12.Size = new System.Drawing.Size(40, 21);
            this.c12.TabIndex = 2;
            this.c12.Text = "270";
            this.c12.TextAlign = HorizontalAlignment.Center;
            // 
            // label17
            // 
            this.label17.BorderStyle = BorderStyle.FixedSingle;
            this.label17.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.ForeColor = System.Drawing.Color.Black;
            this.label17.Location = new System.Drawing.Point(8, 73);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(32, 21);
            this.label17.TabIndex = 13;
            this.label17.Text = "1";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label14
            // 
            this.label14.BorderStyle = BorderStyle.FixedSingle;
            this.label14.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.ForeColor = System.Drawing.Color.Black;
            this.label14.Location = new System.Drawing.Point(8, 48);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(32, 24);
            this.label14.TabIndex = 10;
            this.label14.Text = "niv";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label15
            // 
            this.label15.BorderStyle = BorderStyle.FixedSingle;
            this.label15.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.ForeColor = System.Drawing.Color.Black;
            this.label15.Location = new System.Drawing.Point(41, 48);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(40, 24);
            this.label15.TabIndex = 90;
            this.label15.Text = "min";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label16
            // 
            this.label16.BorderStyle = BorderStyle.FixedSingle;
            this.label16.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.ForeColor = System.Drawing.Color.Black;
            this.label16.Location = new System.Drawing.Point(82, 48);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(40, 24);
            this.label16.TabIndex = 80;
            this.label16.Text = "max";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // r1
            // 
            this.r1.BorderStyle = BorderStyle.FixedSingle;
            this.r1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.r1.ForeColor = System.Drawing.Color.Black;
            this.r1.Location = new System.Drawing.Point(123, 73);
            this.r1.Name = "r1";
            this.r1.Size = new System.Drawing.Size(32, 21);
            this.r1.TabIndex = 11;
            this.r1.Text = "-";
            this.r1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bLeer
            // 
            this.bLeer.BackColor = System.Drawing.Color.DarkSalmon;
            this.bLeer.FlatStyle = FlatStyle.Popup;
            this.bLeer.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bLeer.ForeColor = System.Drawing.Color.Black;
            this.bLeer.Image = ((System.Drawing.Image)(resources.GetObject("bLeer.Image")));
            this.bLeer.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bLeer.Location = new System.Drawing.Point(160, 67);
            this.bLeer.Name = "bLeer";
            this.bLeer.Size = new System.Drawing.Size(152, 32);
            this.bLeer.TabIndex = 3;
            this.bLeer.Text = "Abrir Condiciones";
            this.bLeer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bLeer.UseVisualStyleBackColor = false;
            this.bLeer.Click += new System.EventHandler(this.BLeerClick);
            // 
            // bSalvar
            // 
            this.bSalvar.BackColor = System.Drawing.Color.DarkSalmon;
            this.bSalvar.FlatStyle = FlatStyle.Popup;
            this.bSalvar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bSalvar.ForeColor = System.Drawing.Color.Black;
            this.bSalvar.Image = ((System.Drawing.Image)(resources.GetObject("bSalvar.Image")));
            this.bSalvar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bSalvar.Location = new System.Drawing.Point(160, 112);
            this.bSalvar.Name = "bSalvar";
            this.bSalvar.Size = new System.Drawing.Size(152, 32);
            this.bSalvar.TabIndex = 2;
            this.bSalvar.Text = "Salvar Condiciones";
            this.bSalvar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bSalvar.UseVisualStyleBackColor = false;
            this.bSalvar.Click += new System.EventHandler(this.BSalvarClick);
            // 
            // lval
            // 
            this.lval.BorderStyle = BorderStyle.FixedSingle;
            this.lval.FlatStyle = FlatStyle.Popup;
            this.lval.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lval.Location = new System.Drawing.Point(330, 440);
            this.lval.Name = "lval";
            this.lval.Size = new System.Drawing.Size(137, 24);
            this.lval.TabIndex = 144;
            this.lval.Text = "Admitidas";
            this.lval.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label30);
            this.groupBox3.Controls.Add(this.tCG);
            this.groupBox3.Controls.Add(this.bAnalizar);
            this.groupBox3.FlatStyle = FlatStyle.Popup;
            this.groupBox3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox3.Location = new System.Drawing.Point(490, 424);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(168, 128);
            this.groupBox3.TabIndex = 149;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Análisis Resultados";
            // 
            // label30
            // 
            this.label30.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label30.ForeColor = System.Drawing.Color.Black;
            this.label30.Location = new System.Drawing.Point(8, 16);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(152, 24);
            this.label30.TabIndex = 150;
            this.label30.Text = "columna ganadora";
            this.label30.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tCG
            // 
            this.tCG.BorderStyle = BorderStyle.FixedSingle;
            this.tCG.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tCG.ForeColor = System.Drawing.Color.Black;
            this.tCG.Location = new System.Drawing.Point(8, 40);
            this.tCG.Name = "tCG";
            this.tCG.Size = new System.Drawing.Size(144, 21);
            this.tCG.TabIndex = 147;
            this.tCG.Text = "111x11x222x111";
            this.tCG.TextAlign = HorizontalAlignment.Center;
            // 
            // bAnalizar
            // 
            this.bAnalizar.BackColor = System.Drawing.Color.DarkSalmon;
            this.bAnalizar.FlatStyle = FlatStyle.Popup;
            this.bAnalizar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bAnalizar.ForeColor = System.Drawing.Color.Black;
            this.bAnalizar.Image = ((System.Drawing.Image)(resources.GetObject("bAnalizar.Image")));
            this.bAnalizar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bAnalizar.Location = new System.Drawing.Point(16, 72);
            this.bAnalizar.Name = "bAnalizar";
            this.bAnalizar.Size = new System.Drawing.Size(128, 32);
            this.bAnalizar.TabIndex = 148;
            this.bAnalizar.Text = "Analizar";
            this.bAnalizar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bAnalizar.UseVisualStyleBackColor = false;
            this.bAnalizar.Click += new System.EventHandler(this.BAnalizarClick);
            // 
            // lproc
            // 
            this.lproc.BorderStyle = BorderStyle.FixedSingle;
            this.lproc.FlatStyle = FlatStyle.Popup;
            this.lproc.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lproc.Location = new System.Drawing.Point(330, 415);
            this.lproc.Name = "lproc";
            this.lproc.Size = new System.Drawing.Size(137, 24);
            this.lproc.TabIndex = 145;
            this.lproc.Text = "Procesadas";
            this.lproc.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ltime
            // 
            this.ltime.BorderStyle = BorderStyle.FixedSingle;
            this.ltime.FlatStyle = FlatStyle.Popup;
            this.ltime.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ltime.Location = new System.Drawing.Point(330, 465);
            this.ltime.Name = "ltime";
            this.ltime.Size = new System.Drawing.Size(137, 24);
            this.ltime.TabIndex = 143;
            this.ltime.Text = "Tiempo";
            this.ltime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // bCancelar
            // 
            this.bCancelar.BackColor = System.Drawing.Color.DarkSalmon;
            this.bCancelar.FlatStyle = FlatStyle.Popup;
            this.bCancelar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.bCancelar.Image = ((System.Drawing.Image)(resources.GetObject("bCancelar.Image")));
            this.bCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.bCancelar.Location = new System.Drawing.Point(501, 382);
            this.bCancelar.Name = "bCancelar";
            this.bCancelar.Size = new System.Drawing.Size(128, 32);
            this.bCancelar.TabIndex = 5;
            this.bCancelar.Text = "Cancelar";
            this.bCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.bCancelar.UseVisualStyleBackColor = false;
            this.bCancelar.Click += new System.EventHandler(this.BCancelarClick);
            // 
            // TriosFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(7, 16);
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(811, 558);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.lproc);
            this.Controls.Add(this.lval);
            this.Controls.Add(this.ltime);
            this.Controls.Add(this.bGrabar);
            this.Controls.Add(this.bCancelar);
            this.Controls.Add(this.bCalcular);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "TriosFrm";
            this.Text = "Tríos";
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
