// Free1X2 : Programa de quinielas "libre"
// Created 10-02-05 at 20:03 
// Copyright (C) 2004 Joan Duatis - duatis@coac.net
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
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using Free1X2.MotorCalculo;
using Free1X2.Utils;
using Free1X2.Analisis;
using Free1X2.EntradaSalida;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI
{
	/// <summary>
	/// Summary description for BancoPruebasFrm.
	/// </summary>
	public class BancoPruebasFrm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.StatusBarPanel statusBarPanel1;
		private System.Windows.Forms.StatusBarPanel statusBarPanel2;
        private System.Windows.Forms.StatusBarPanel statusBarPanel3;
        private IContainer components;

		private double[,] p = new double [14,3];
		private double[,] v = new double [14,3];
		private float[] Cr = new float [14];
		private float[,] pa = new float [14,3];
		private int[] PotDe3 = new int [] {1,3,9,27,81,243,729,2187,6561,19683,59049,177147,531441,1594323};
		private int[] DosPotDe3 = new int[]{2,6,18,54,162,486,1458,4374,13122,39366,118098,354294,1062882,3188646};
		private float[,] Cra = new float [14,3];
		private ApuestaProbableCentral[] Ap14T=new ApuestaProbableCentral[4782969] ;
		private BitArray Bits = new BitArray(4782969,false);
		private int NumAleatorias = 1000;
		private double Recaudacion=14000000;
		private double PrecioApuesta=0.5;
		private int NumApuestas;
		private int Profundidad=0;
		private ValidadorCadenas Valida= new ValidadorCadenas();
		private ArrayList ColumnasAleatorias=null;
		private ArrayList Columnas=null;
		private double[] SumaProbabilidades = new double [5];
		private double[] Premios = new double [5]; 
		private double ProbabilidadCategoria14=1;
		private double [] PctDestinadoAPremiosCategoria= new double [5] {0.12, 0.08, 0.08, 0.08, 0.09};
		private double [] DestinadoAPremiosCategoria= new double [5];
		Resultados[] Res= null;
        ResultadosJornada[] ResultadoPorJornadas = null;
		ResultadosPorColumna[] ResCol= null;
		double _LN=-14.7;
		int CriterioOrdenacion =0;
		bool[] AcumularPremio = new bool [5]{false,false,false,false,true};
		private Porcentajes Pct =new Porcentajes ();
		private bool salida=false;
		private double[,] MaxMin = new double [18,2];
		private Point pointInCell00 = new Point (0,0);
		private bool DataGridVacia = true;
        private TabControl tabControl1;
        private TabPage tabPaso2;
        private TabPage tabPaso3;
        private TabPage tabPaso4;
        private GroupBox groupBox1;
        private TextBox txDesvTipica;
        private Label label32;
        private TextBox txLNmedia;
        private Label label21;
        private Free1X2.UI.BancoPruebasFrm.MyDataGrid dgResultadoEscrutinio;
        private TabPage tabPaso1;
        private TextBox txFicheroEntrada;
        private Label lblCombinacion;
        private Button btLeerColumnas;
        private Label label41;
        private Label lblRecuperacion;
        private Label label39;
        private Label lblPremioTotal;
        private Label lblGastoTotal;
        private Label label38;
        private Label lblNumColumnas;
        private Label label37;
        private Button btnOK;
        private GroupBox groupBox2;
        private Button btAbrirAleatorias;
        private TextBox txFicheroAleatorias;
        private Label label34;
        private TextBox txLNMin;
        private Label label33;
        private TextBox txDTdeseada;
        private Button btGuardarAleatorias;
        private Button btGenerarAleatorias;
        private Label label17;
        private TextBox txNumAleatorias;
        private Label label42;
        private Label label43;
        private Label label44;
        private Label label45;
        private GroupBox groupBox3;
        private Label label46;
        private Label label40;
        private Label label36;
        private Button btCalcularReales;
        private TextBox txNumCol;
        private TextBox txLN;
        private Label label35;
        private Label label47;
        private Label lblVecesPremiada;
        private Label label49;
        private Label lblVecesBeneficio;
        private RadioButton rbAleatorias;
        private RadioButton rbFichero;
        private Button btGrabar;
        private GroupBox grPremiosAacumular;
        private CheckBox chAcumula14;
        private CheckBox chAcumula13;
        private CheckBox chAcumula12;
        private CheckBox chAcumula11;
        private CheckBox chAcumula10;
        private ProgressBar progressBar1;
        private GroupBox grOpcionAnalisis;
        private RadioButton rbOpcionCombinacion;
        private RadioButton rbOpcionColumnas;
        private Button btsiguiente;
        private Button btanterior;
        private Label label50;
        private RadioButton rbPremio;
        private RadioButton rbLN;
        private TextBox txPremio;
        private TextBox txColumna;
        private Button btSeleccionJornadas;
        private Label label51;
        private Label label52;
        private Label lblNewBase142;
        private Label lblNewBase132;
        private Label lblNewBase122;
        private Label lblNewBase112;
        private Label lblNewBase102;
        private Label lblNewBase92;
        private Label lblNewBase82;
        private Label lblNewBase72;
        private Label lblNewBase62;
        private Label lblNewBase52;
        private Label lblNewBase42;
        private Label lblNewBase32;
        private Label lblNewBase22;
        private Label lblNewBase12;
        private Label lblNewBase14X;
        private Label lblNewBase13X;
        private Label lblNewBase12X;
        private Label lblNewBase11X;
        private Label lblNewBase10X;
        private Label lblNewBase9X;
        private Label lblNewBase8X;
        private Label lblNewBase7X;
        private Label lblNewBase6X;
        private Label lblNewBase5X;
        private Label lblNewBase4X;
        private Label lblNewBase3X;
        private Label lblNewBase2X;
        private Label lblNewBase1X;
        private Label lblNewBase141;
        private Label lblNewBase131;
        private Label lblNewBase121;
        private Label lblNewBase111;
        private Label lblNewBase101;
        private Label lblNewBase91;
        private Label lblNewBase81;
        private Label lblNewBase71;
        private Label lblNewBase61;
        private Label lblNewBase51;
        private Label lblNewBase41;
        private Label lblNewBase31;
        private Label lblNewBase21;
        private Label lblNewBase11;
        private Label label53;
        private TextBox txNumFila;
        private Button btFiltrar;
        private Label label1;
        private Label label2;
		private Free1X2.UI.Controls.ControlPorcentajes controlPorcentajesApostados;
		private System.Windows.Forms.RadioButton rbOptionEspecial;
        private GroupBox groupBox4;
        private Label lblContaSeleccionadas;
        private ToolTip toolTip1;
        private Label label4;
        private TextBox txDiferencias;
        private Label label3;
        private RadioButton rbParaCadaFila;
        private RadioButton rbSoloFilaActual;
        private Button btEliminarFilas;
        private RadioButton rbOptionJornadas;
		private Free1X2.UI.Controls.ControlPorcentajes controlPorcentajesReales;

		private class Resultados
		{
			private string _Concepto;
			private int _Valor;
			private double _Premios;

			public Resultados(string pConcepto, int pAciertos)
			{
				Concepto=pConcepto;
				Valor=pAciertos;
				Premios=0;
			}
			public string Concepto
			{
				get{ return _Concepto; }
				set{_Concepto = value;}
			}
			public int Valor
			{
				get{ return _Valor; }
				set{_Valor = value;}
			}
			public double Premios
			{
				get{ return _Premios; }
				set{_Premios = value;}
			}
			public double PremiosMedios
			{
				get
				{ 
					if(_Valor>0 )
					{
						return Math.Round (_Premios/_Valor,2); 
					}
					else
					{
						return 0;
					}
				}
			}
		}
        private class ResultadosJornada
        {
            private int _Jornada;
            private double _SaldoInicial;
            private double[] _Premios = new double[5];
            private int[] _Aciertos = new int[5];
            private double _Coste;

            public ResultadosJornada(int pJornada, double pSaldoInicial, double pCoste, double[] pPremios, int[] pAciertos)
            {
                Jornada = pJornada;
                SaldoInicial = pSaldoInicial;
                Coste = pCoste;
                Premios = pPremios;
                Aciertos = pAciertos;
            }
            public ResultadosJornada()
            {
            }
            public int Jornada
            {
                get { return _Jornada; }
                set { _Jornada = value; }
            }
            public double Saldo
            {
                get { return _SaldoInicial - _Coste + TotalPremios; }
            }
            public double SaldoInicial
            {
                get { return _SaldoInicial; }
                set { _SaldoInicial = value; }
            }
            public double Coste
            {
                get { return _Coste; }
                set { _Coste = value; }
            }
            public double[] Premios
            {
                get { return _Premios; }
                set { _Premios = value; }
            }
            public int[] Aciertos
            {
                get { return _Aciertos; }
                set { _Aciertos = value; }
            }
            public double TotalPremios
            {
                get
                {
                    double Resultado = 0;
                    for (int i = 0; i < 5; i++)
                    {
                        Resultado += _Premios[i];
                    }
                    return Resultado;
                }
            }
            public int AciertosDe14
            {
                get { return _Aciertos[0]; }
            }
            public int AciertosDe13
            {
                get { return _Aciertos[1]; }
            }
            public int AciertosDe12
            {
                get { return _Aciertos[2]; }
            }
            public int AciertosDe11
            {
                get { return _Aciertos[3]; }
            }
            public int AciertosDe10
            {
                get { return _Aciertos[4]; }
            }
        }

        private class ResultadosPorColumna
		{
			private int _Columna;
			private int[] _NumVeces = new int [5];
			private double[] _Premios= new double [5];
			private double[] _PremioUnitario= new double [5];
			private bool[] _Acumula=new bool [5];
			private string _Num;
			private double _Recuperacion;
			private int _NumEscrutinios;


			public ResultadosPorColumna(int pNumEscrutinios,int[] pNumVeces, double[] pPremios, bool[] pAcumula)
			{
				_Num="SUMAS ";
				_NumEscrutinios=pNumEscrutinios;
				for(int i=0;i<5;i++){_NumVeces[i]=pNumVeces[14-i];}
				_Premios=pPremios;
				_Acumula=pAcumula;
			}
			public ResultadosPorColumna(int pColumna, int pNumEscrutinios)
			{
				_Columna=pColumna;
				_NumEscrutinios=pNumEscrutinios;
			}
			public ResultadosPorColumna()
			{
			}

			public void ContarPremio(int categoria)
			{
				_NumVeces[categoria]++;
			}
			public void SumarPremio(int categoria, double premio)
			{
				_Premios[categoria]+=premio;
			}
			public double PremioUnitarioDe (int Indice)
			{
				return _PremioUnitario [Indice];
			}
			public double PremioDe (int Indice)
			{
				return _Premios [Indice];
			}
			public int NumVecesDe (int Indice)
			{
				return _NumVeces [Indice];
			}

			public string Num
			{
				get{ return _Num; }
				set{_Num = value;}
			}
			public bool[] Acumula
			{
				set{_Acumula = value;}
			}
			public int Columna
			{
				get{return _Columna;}
			}
			public double PremioDe14
			{
				get{ return Math.Round (_Premios[0]); }
			}
			public double PremioDe13
			{
				get{ return Math.Round (_Premios[1]); }
			}
			public double PremioDe12
			{
				get{ return Math.Round (_Premios[2]); }
			}
			public double PremioDe11
			{
				get{ return Math.Round (_Premios[3]); }
			}
			public double PremioDe10
			{
				get{ return Math.Round (_Premios[4]); }
			}
			public int Veces14
			{
				get{ return _NumVeces[0]; }
			}
			public int Veces13
			{
				get{ return _NumVeces[1]; }
			}
			public int Veces12
			{
				get{ return _NumVeces[2]; }
			}
			public int Veces11
			{
				get{ return _NumVeces[3]; }
			}
			public int Veces10
			{
				get{ return _NumVeces[4]; }
			}
			public double VecesAcumulado
			{
				get
				{ 
					double Suma=0;
					for (byte i=0;i<5;i++){if(_Acumula [i]) Suma+=_NumVeces [i];}
					return Suma;
				}
			}

			public double PremioUnitarioDe14
			{
				get{if(_NumVeces [0]>0) _PremioUnitario [0]= Math.Round (_Premios[0]/_NumVeces [0]);
					return _PremioUnitario [0]; }
			}
			public double PremioUnitarioDe13
			{
				get{ if(_NumVeces [1]>0) _PremioUnitario [1]= Math.Round (_Premios[1]/_NumVeces [1]);
				return _PremioUnitario [1];}
			}
			public double PremioUnitarioDe12
			{
				get{if(_NumVeces [2]>0) _PremioUnitario [2]=Math.Round (_Premios[2]/_NumVeces [2]); 
				return _PremioUnitario [2];}
			}
			public double PremioUnitarioDe11
			{
				get{if(_NumVeces [3]>0) _PremioUnitario [3]= Math.Round (_Premios[3]/_NumVeces [3]);
				return _PremioUnitario [3];}
			}
			public double PremioUnitarioDe10
			{
				get{if(_NumVeces [4]>0) _PremioUnitario [4]=Math.Round (_Premios[4]/_NumVeces [4]);
				return _PremioUnitario [4];}
			}
			public double PremioAcumulado
			{
				get
				{ 
					double Suma=0;
					for (byte i=0;i<5;i++){if(_Acumula [i]) Suma+=_Premios [i];}
					_Recuperacion=Math.Round (Suma*100/_NumEscrutinios/0.5,0);
					return Math.Round (Suma,1);
				}
			}
			public string Recuperacion
			{
				get
				{
					if(_Num =="SUMAS ")
					{return "";}
					else
					{return _Recuperacion.ToString() + " %"; }
				}
			}
			public double ComparaCon(ResultadosPorColumna ResColumna, int cabecera)
			{
				double result=0;
				switch (cabecera)
				{
					case 0:
						result= Convert.ToDouble (ResColumna.Num)-Convert.ToDouble (Num);
						break;
					case 1 :
					case 2 :
					case 3 :
					case 4 :
					case 5 :
						result= _NumVeces [cabecera-1]-ResColumna._NumVeces[cabecera-1];
						break;
					case 6 :
						result= VecesAcumulado - ResColumna.VecesAcumulado ;
						break;
					case 7 :
					case 8 :
					case 9 :
					case 10 :
					case 11 :
						result= _Premios [cabecera-7]-ResColumna.PremioDe (cabecera-7);
						break;
					case 12 :
					case 18 :
						result= PremioAcumulado -ResColumna.PremioAcumulado;
						break;
					case 13 :
					case 14 :
					case 15 :
					case 16 :
					case 17 :
						result= _PremioUnitario [cabecera-13]-ResColumna.PremioUnitarioDe (cabecera-13);
						break;
 					default: break;
				}
				return result;
			}
		}

		public BancoPruebasFrm()
		{
			InitializeComponent();
			InicializaGridResultadoEscrutinioPorColumnas();
			InicializaGridResultadoEscrutinio();
            InicializaGridResultadoEscrutinioPorJornadas();
			statusBar1.ShowPanels =true;
			txLN.Text ="-14,75";
			for (int i=0 ;i<5;i++)
			{
				DestinadoAPremiosCategoria[i]=Recaudacion*PctDestinadoAPremiosCategoria[i];
			}
			NumApuestas=(int)(Recaudacion/PrecioApuesta);
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
        }


		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BancoPruebasFrm));
            this.btnCancel = new System.Windows.Forms.Button();
            this.statusBar1 = new System.Windows.Forms.StatusBar();
            this.statusBarPanel1 = new System.Windows.Forms.StatusBarPanel();
            this.statusBarPanel2 = new System.Windows.Forms.StatusBarPanel();
            this.statusBarPanel3 = new System.Windows.Forms.StatusBarPanel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPaso1 = new System.Windows.Forms.TabPage();
            this.label42 = new System.Windows.Forms.Label();
            this.btLeerColumnas = new System.Windows.Forms.Button();
            this.txFicheroEntrada = new System.Windows.Forms.TextBox();
            this.lblCombinacion = new System.Windows.Forms.Label();
            this.tabPaso2 = new System.Windows.Forms.TabPage();
            this.controlPorcentajesReales = new Free1X2.UI.Controls.ControlPorcentajes();
            this.controlPorcentajesApostados = new Free1X2.UI.Controls.ControlPorcentajes();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btSeleccionJornadas = new System.Windows.Forms.Button();
            this.rbPremio = new System.Windows.Forms.RadioButton();
            this.rbLN = new System.Windows.Forms.RadioButton();
            this.txPremio = new System.Windows.Forms.TextBox();
            this.label50 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.btCalcularReales = new System.Windows.Forms.Button();
            this.txNumCol = new System.Windows.Forms.TextBox();
            this.label40 = new System.Windows.Forms.Label();
            this.txLN = new System.Windows.Forms.TextBox();
            this.label36 = new System.Windows.Forms.Label();
            this.label46 = new System.Windows.Forms.Label();
            this.label43 = new System.Windows.Forms.Label();
            this.tabPaso3 = new System.Windows.Forms.TabPage();
            this.label44 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label17 = new System.Windows.Forms.Label();
            this.txNumAleatorias = new System.Windows.Forms.TextBox();
            this.btGenerarAleatorias = new System.Windows.Forms.Button();
            this.btGuardarAleatorias = new System.Windows.Forms.Button();
            this.label34 = new System.Windows.Forms.Label();
            this.txLNMin = new System.Windows.Forms.TextBox();
            this.label33 = new System.Windows.Forms.Label();
            this.txDTdeseada = new System.Windows.Forms.TextBox();
            this.btAbrirAleatorias = new System.Windows.Forms.Button();
            this.txFicheroAleatorias = new System.Windows.Forms.TextBox();
            this.rbAleatorias = new System.Windows.Forms.RadioButton();
            this.rbFichero = new System.Windows.Forms.RadioButton();
            this.txDesvTipica = new System.Windows.Forms.TextBox();
            this.label32 = new System.Windows.Forms.Label();
            this.txLNmedia = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.tabPaso4 = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btEliminarFilas = new System.Windows.Forms.Button();
            this.rbParaCadaFila = new System.Windows.Forms.RadioButton();
            this.rbSoloFilaActual = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.txDiferencias = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.lblContaSeleccionadas = new System.Windows.Forms.Label();
            this.btFiltrar = new System.Windows.Forms.Button();
            this.txNumFila = new System.Windows.Forms.TextBox();
            this.label53 = new System.Windows.Forms.Label();
            this.lblNewBase142 = new System.Windows.Forms.Label();
            this.lblNewBase132 = new System.Windows.Forms.Label();
            this.lblNewBase122 = new System.Windows.Forms.Label();
            this.lblNewBase112 = new System.Windows.Forms.Label();
            this.lblNewBase102 = new System.Windows.Forms.Label();
            this.lblNewBase92 = new System.Windows.Forms.Label();
            this.lblNewBase82 = new System.Windows.Forms.Label();
            this.lblNewBase72 = new System.Windows.Forms.Label();
            this.lblNewBase62 = new System.Windows.Forms.Label();
            this.lblNewBase52 = new System.Windows.Forms.Label();
            this.lblNewBase42 = new System.Windows.Forms.Label();
            this.lblNewBase32 = new System.Windows.Forms.Label();
            this.lblNewBase22 = new System.Windows.Forms.Label();
            this.lblNewBase12 = new System.Windows.Forms.Label();
            this.lblNewBase14X = new System.Windows.Forms.Label();
            this.lblNewBase13X = new System.Windows.Forms.Label();
            this.lblNewBase12X = new System.Windows.Forms.Label();
            this.lblNewBase11X = new System.Windows.Forms.Label();
            this.lblNewBase10X = new System.Windows.Forms.Label();
            this.lblNewBase9X = new System.Windows.Forms.Label();
            this.lblNewBase8X = new System.Windows.Forms.Label();
            this.lblNewBase7X = new System.Windows.Forms.Label();
            this.lblNewBase6X = new System.Windows.Forms.Label();
            this.lblNewBase5X = new System.Windows.Forms.Label();
            this.lblNewBase4X = new System.Windows.Forms.Label();
            this.lblNewBase3X = new System.Windows.Forms.Label();
            this.lblNewBase2X = new System.Windows.Forms.Label();
            this.lblNewBase1X = new System.Windows.Forms.Label();
            this.lblNewBase141 = new System.Windows.Forms.Label();
            this.lblNewBase131 = new System.Windows.Forms.Label();
            this.lblNewBase121 = new System.Windows.Forms.Label();
            this.lblNewBase111 = new System.Windows.Forms.Label();
            this.lblNewBase101 = new System.Windows.Forms.Label();
            this.lblNewBase91 = new System.Windows.Forms.Label();
            this.lblNewBase81 = new System.Windows.Forms.Label();
            this.lblNewBase71 = new System.Windows.Forms.Label();
            this.lblNewBase61 = new System.Windows.Forms.Label();
            this.lblNewBase51 = new System.Windows.Forms.Label();
            this.lblNewBase41 = new System.Windows.Forms.Label();
            this.lblNewBase31 = new System.Windows.Forms.Label();
            this.lblNewBase21 = new System.Windows.Forms.Label();
            this.lblNewBase11 = new System.Windows.Forms.Label();
            this.label52 = new System.Windows.Forms.Label();
            this.label51 = new System.Windows.Forms.Label();
            this.txColumna = new System.Windows.Forms.TextBox();
            this.grOpcionAnalisis = new System.Windows.Forms.GroupBox();
            this.rbOptionJornadas = new System.Windows.Forms.RadioButton();
            this.rbOptionEspecial = new System.Windows.Forms.RadioButton();
            this.rbOpcionColumnas = new System.Windows.Forms.RadioButton();
            this.rbOpcionCombinacion = new System.Windows.Forms.RadioButton();
            this.btnOK = new System.Windows.Forms.Button();
            this.grPremiosAacumular = new System.Windows.Forms.GroupBox();
            this.chAcumula10 = new System.Windows.Forms.CheckBox();
            this.chAcumula11 = new System.Windows.Forms.CheckBox();
            this.chAcumula12 = new System.Windows.Forms.CheckBox();
            this.chAcumula13 = new System.Windows.Forms.CheckBox();
            this.chAcumula14 = new System.Windows.Forms.CheckBox();
            this.btGrabar = new System.Windows.Forms.Button();
            this.label49 = new System.Windows.Forms.Label();
            this.lblVecesBeneficio = new System.Windows.Forms.Label();
            this.label47 = new System.Windows.Forms.Label();
            this.lblVecesPremiada = new System.Windows.Forms.Label();
            this.label45 = new System.Windows.Forms.Label();
            this.label41 = new System.Windows.Forms.Label();
            this.lblRecuperacion = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.lblPremioTotal = new System.Windows.Forms.Label();
            this.lblGastoTotal = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.lblNumColumnas = new System.Windows.Forms.Label();
            this.label37 = new System.Windows.Forms.Label();
            this.dgResultadoEscrutinio = new Free1X2.UI.BancoPruebasFrm.MyDataGrid();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.btsiguiente = new System.Windows.Forms.Button();
            this.btanterior = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            // StatusBarPanel controls don't support ISupportInitialize in .NET 8
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).BeginInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel2)).BeginInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel3)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPaso1.SuspendLayout();
            this.tabPaso2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tabPaso3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPaso4.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.grOpcionAnalisis.SuspendLayout();
            this.grPremiosAacumular.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgResultadoEscrutinio)).BeginInit();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(668, 478);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 32);
            this.btnCancel.TabIndex = 631;
            this.btnCancel.Text = "&Cancelar";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // statusBar1
            // 
            this.statusBar1.Location = new System.Drawing.Point(0, 544);
            this.statusBar1.Name = "statusBar1";
            this.statusBar1.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.statusBarPanel1,
            this.statusBarPanel2,
            this.statusBarPanel3});
            this.statusBar1.ShowPanels = true;
            this.statusBar1.Size = new System.Drawing.Size(792, 22);
            this.statusBar1.TabIndex = 710;
            this.statusBar1.Text = "statusBar1";
            // 
            // statusBarPanel1
            // 
            this.statusBarPanel1.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.statusBarPanel1.Icon = ((System.Drawing.Icon)(resources.GetObject("statusBarPanel1.Icon")));
            this.statusBarPanel1.Name = "statusBarPanel1";
            this.statusBarPanel1.Style = System.Windows.Forms.StatusBarPanelStyle.OwnerDraw;
            this.statusBarPanel1.Width = 31;
            // 
            // statusBarPanel2
            // 
            this.statusBarPanel2.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            this.statusBarPanel2.Name = "statusBarPanel2";
            this.statusBarPanel2.Width = 734;
            // 
            // statusBarPanel3
            // 
            this.statusBarPanel3.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.statusBarPanel3.Name = "statusBarPanel3";
            this.statusBarPanel3.Width = 10;
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPaso1);
            this.tabControl1.Controls.Add(this.tabPaso2);
            this.tabControl1.Controls.Add(this.tabPaso3);
            this.tabControl1.Controls.Add(this.tabPaso4);
            this.tabControl1.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(8, 8);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(776, 512);
            this.tabControl1.TabIndex = 722;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // tabPaso1
            // 
            this.tabPaso1.AutoScroll = true;
            this.tabPaso1.BackColor = System.Drawing.Color.Bisque;
            this.tabPaso1.Controls.Add(this.label42);
            this.tabPaso1.Controls.Add(this.btLeerColumnas);
            this.tabPaso1.Controls.Add(this.txFicheroEntrada);
            this.tabPaso1.Controls.Add(this.lblCombinacion);
            this.tabPaso1.Location = new System.Drawing.Point(4, 23);
            this.tabPaso1.Name = "tabPaso1";
            this.tabPaso1.Size = new System.Drawing.Size(768, 485);
            this.tabPaso1.TabIndex = 0;
            this.tabPaso1.Text = "Paso 1 (Ficheros)";
            // 
            // label42
            // 
            this.label42.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label42.ForeColor = System.Drawing.Color.DarkRed;
            this.label42.Location = new System.Drawing.Point(13, 52);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(355, 25);
            this.label42.TabIndex = 711;
            this.label42.Text = "Selección del fichero de columnas a analizar";
            // 
            // btLeerColumnas
            // 
            this.btLeerColumnas.BackColor = System.Drawing.Color.LightSalmon;
            this.btLeerColumnas.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btLeerColumnas.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btLeerColumnas.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btLeerColumnas.Image = ((System.Drawing.Image)(resources.GetObject("btLeerColumnas.Image")));
            this.btLeerColumnas.Location = new System.Drawing.Point(302, 100);
            this.btLeerColumnas.Name = "btLeerColumnas";
            this.btLeerColumnas.Size = new System.Drawing.Size(22, 22);
            this.btLeerColumnas.TabIndex = 709;
            this.btLeerColumnas.UseVisualStyleBackColor = false;
            this.btLeerColumnas.Click += new System.EventHandler(this.btLeerColumnas_Click);
            // 
            // txFicheroEntrada
            // 
            this.txFicheroEntrada.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txFicheroEntrada.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txFicheroEntrada.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txFicheroEntrada.Location = new System.Drawing.Point(325, 100);
            this.txFicheroEntrada.Name = "txFicheroEntrada";
            this.txFicheroEntrada.Size = new System.Drawing.Size(377, 22);
            this.txFicheroEntrada.TabIndex = 708;
            // 
            // lblCombinacion
            // 
            this.lblCombinacion.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblCombinacion.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCombinacion.Location = new System.Drawing.Point(13, 100);
            this.lblCombinacion.Name = "lblCombinacion";
            this.lblCombinacion.Size = new System.Drawing.Size(283, 22);
            this.lblCombinacion.TabIndex = 707;
            this.lblCombinacion.Text = "Nombre del fichero (ruta completa incluida)";
            this.lblCombinacion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabPaso2
            // 
            this.tabPaso2.BackColor = System.Drawing.Color.Bisque;
            this.tabPaso2.Controls.Add(this.controlPorcentajesReales);
            this.tabPaso2.Controls.Add(this.controlPorcentajesApostados);
            this.tabPaso2.Controls.Add(this.label2);
            this.tabPaso2.Controls.Add(this.label1);
            this.tabPaso2.Controls.Add(this.groupBox3);
            this.tabPaso2.Controls.Add(this.label43);
            this.tabPaso2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPaso2.Location = new System.Drawing.Point(4, 23);
            this.tabPaso2.Name = "tabPaso2";
            this.tabPaso2.Size = new System.Drawing.Size(768, 485);
            this.tabPaso2.TabIndex = 1;
            this.tabPaso2.Text = "Paso 2 (valoraciones)";
            // 
            // controlPorcentajesReales
            // 
            this.controlPorcentajesReales.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.controlPorcentajesReales.archivoPorcentajes = null;
            this.controlPorcentajesReales.BackColor = System.Drawing.Color.Bisque;
            this.controlPorcentajesReales.CaptionText = "  P O R C E N T A J E S";
            this.controlPorcentajesReales.FormatoFicheroValoraciones = ((short)(0));
            this.controlPorcentajesReales.Jornada = "01";
            this.controlPorcentajesReales.Location = new System.Drawing.Point(572, 84);
            this.controlPorcentajesReales.Name = "controlPorcentajesReales";
            this.controlPorcentajesReales.ReadOnly = false;
            this.controlPorcentajesReales.Size = new System.Drawing.Size(168, 354);
            this.controlPorcentajesReales.TabIndex = 851;
            this.controlPorcentajesReales.Temporada = "2004/2005";
            this.controlPorcentajesReales.Modificado += new System.EventHandler(this.controlPorcentajesReales_Modificado);
            // 
            // controlPorcentajesApostados
            // 
            this.controlPorcentajesApostados.archivoPorcentajes = null;
            this.controlPorcentajesApostados.BackColor = System.Drawing.Color.Bisque;
            this.controlPorcentajesApostados.CaptionText = "  P O R C E N T A J E S";
            this.controlPorcentajesApostados.FormatoFicheroValoraciones = ((short)(0));
            this.controlPorcentajesApostados.Jornada = "01";
            this.controlPorcentajesApostados.Location = new System.Drawing.Point(40, 84);
            this.controlPorcentajesApostados.Name = "controlPorcentajesApostados";
            this.controlPorcentajesApostados.ReadOnly = false;
            this.controlPorcentajesApostados.Size = new System.Drawing.Size(160, 387);
            this.controlPorcentajesApostados.TabIndex = 850;
            this.controlPorcentajesApostados.Temporada = "2004/2005";
            this.controlPorcentajesApostados.Modificado += new System.EventHandler(this.controlPorcentajesApostados_Modificado);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Maroon;
            this.label2.Location = new System.Drawing.Point(568, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(164, 32);
            this.label2.TabIndex = 849;
            this.label2.Text = "VALORACIONES       REALES";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Maroon;
            this.label1.Location = new System.Drawing.Point(36, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(164, 32);
            this.label1.TabIndex = 848;
            this.label1.Text = "VALORACIONES APOSTADAS";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.groupBox3.BackColor = System.Drawing.Color.Bisque;
            this.groupBox3.Controls.Add(this.btSeleccionJornadas);
            this.groupBox3.Controls.Add(this.rbPremio);
            this.groupBox3.Controls.Add(this.rbLN);
            this.groupBox3.Controls.Add(this.txPremio);
            this.groupBox3.Controls.Add(this.label50);
            this.groupBox3.Controls.Add(this.label35);
            this.groupBox3.Controls.Add(this.btCalcularReales);
            this.groupBox3.Controls.Add(this.txNumCol);
            this.groupBox3.Controls.Add(this.label40);
            this.groupBox3.Controls.Add(this.txLN);
            this.groupBox3.Controls.Add(this.label36);
            this.groupBox3.Controls.Add(this.label46);
            this.groupBox3.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox3.Location = new System.Drawing.Point(240, 92);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(292, 252);
            this.groupBox3.TabIndex = 840;
            this.groupBox3.TabStop = false;
            // 
            // btSeleccionJornadas
            // 
            this.btSeleccionJornadas.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btSeleccionJornadas.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btSeleccionJornadas.Location = new System.Drawing.Point(240, 80);
            this.btSeleccionJornadas.Name = "btSeleccionJornadas";
            this.btSeleccionJornadas.Size = new System.Drawing.Size(28, 20);
            this.btSeleccionJornadas.TabIndex = 850;
            this.btSeleccionJornadas.Text = "...";
            this.btSeleccionJornadas.Visible = false;
            // 
            // rbPremio
            // 
            this.rbPremio.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbPremio.Location = new System.Drawing.Point(12, 108);
            this.rbPremio.Name = "rbPremio";
            this.rbPremio.Size = new System.Drawing.Size(24, 21);
            this.rbPremio.TabIndex = 849;
            this.rbPremio.CheckedChanged += new System.EventHandler(this.Generico_CheckedChanged);
            // 
            // rbLN
            // 
            this.rbLN.Checked = true;
            this.rbLN.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbLN.Location = new System.Drawing.Point(12, 130);
            this.rbLN.Name = "rbLN";
            this.rbLN.Size = new System.Drawing.Size(24, 21);
            this.rbLN.TabIndex = 848;
            this.rbLN.TabStop = true;
            this.rbLN.CheckedChanged += new System.EventHandler(this.Generico_CheckedChanged);
            // 
            // txPremio
            // 
            this.txPremio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txPremio.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txPremio.Enabled = false;
            this.txPremio.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txPremio.Location = new System.Drawing.Point(204, 108);
            this.txPremio.MaxLength = 12;
            this.txPremio.Name = "txPremio";
            this.txPremio.Size = new System.Drawing.Size(60, 21);
            this.txPremio.TabIndex = 847;
            this.txPremio.TextChanged += new System.EventHandler(this.txPremio_TextChanged);
            // 
            // label50
            // 
            this.label50.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label50.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label50.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label50.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label50.Location = new System.Drawing.Point(34, 108);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(168, 21);
            this.label50.TabIndex = 846;
            this.label50.Text = "Premio medio de 14";
            this.label50.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label35
            // 
            this.label35.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label35.Location = new System.Drawing.Point(17, 80);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(168, 20);
            this.label35.TabIndex = 845;
            this.label35.Text = "Parámetros:";
            // 
            // btCalcularReales
            // 
            this.btCalcularReales.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btCalcularReales.BackColor = System.Drawing.Color.DarkSalmon;
            this.btCalcularReales.Enabled = false;
            this.btCalcularReales.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btCalcularReales.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btCalcularReales.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btCalcularReales.Image = ((System.Drawing.Image)(resources.GetObject("btCalcularReales.Image")));
            this.btCalcularReales.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btCalcularReales.Location = new System.Drawing.Point(168, 196);
            this.btCalcularReales.Name = "btCalcularReales";
            this.btCalcularReales.Size = new System.Drawing.Size(100, 32);
            this.btCalcularReales.TabIndex = 843;
            this.btCalcularReales.Text = "Calcular";
            this.btCalcularReales.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btCalcularReales.UseVisualStyleBackColor = false;
            this.btCalcularReales.Click += new System.EventHandler(this.btCalcularReales_Click);
            // 
            // txNumCol
            // 
            this.txNumCol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txNumCol.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txNumCol.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txNumCol.Location = new System.Drawing.Point(204, 156);
            this.txNumCol.MaxLength = 5;
            this.txNumCol.Name = "txNumCol";
            this.txNumCol.Size = new System.Drawing.Size(60, 21);
            this.txNumCol.TabIndex = 842;
            this.txNumCol.Text = "50000";
            this.txNumCol.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txNumCol_KeyPress);
            // 
            // label40
            // 
            this.label40.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label40.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label40.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label40.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label40.Location = new System.Drawing.Point(34, 156);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(168, 21);
            this.label40.TabIndex = 841;
            this.label40.Text = "Nº columnas a considerar";
            this.label40.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txLN
            // 
            this.txLN.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txLN.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txLN.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txLN.Location = new System.Drawing.Point(204, 130);
            this.txLN.MaxLength = 12;
            this.txLN.Name = "txLN";
            this.txLN.Size = new System.Drawing.Size(60, 21);
            this.txLN.TabIndex = 840;
            this.txLN.Text = "-14,7";
            this.txLN.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txLN_KeyPress);
            this.txLN.TextChanged += new System.EventHandler(this.txLN_TextChanged);
            // 
            // label36
            // 
            this.label36.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label36.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label36.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label36.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.label36.Location = new System.Drawing.Point(34, 130);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(168, 21);
            this.label36.TabIndex = 839;
            this.label36.Text = "LN de la Prob. media del 14";
            this.label36.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label46
            // 
            this.label46.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label46.ForeColor = System.Drawing.Color.Brown;
            this.label46.Location = new System.Drawing.Point(12, 16);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(252, 48);
            this.label46.TabIndex = 0;
            this.label46.Text = "Obtención de la valoración real a partir de la apostada";
            // 
            // label43
            // 
            this.label43.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label43.ForeColor = System.Drawing.Color.DarkRed;
            this.label43.Location = new System.Drawing.Point(33, 16);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(282, 26);
            this.label43.TabIndex = 839;
            this.label43.Text = "Definición de la valoración real y apostada";
            // 
            // tabPaso3
            // 
            this.tabPaso3.BackColor = System.Drawing.Color.Bisque;
            this.tabPaso3.Controls.Add(this.label44);
            this.tabPaso3.Controls.Add(this.groupBox1);
            this.tabPaso3.Location = new System.Drawing.Point(4, 23);
            this.tabPaso3.Name = "tabPaso3";
            this.tabPaso3.Size = new System.Drawing.Size(768, 485);
            this.tabPaso3.TabIndex = 2;
            this.tabPaso3.Text = "Paso 3 (simular 14\'s)";
            // 
            // label44
            // 
            this.label44.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label44.ForeColor = System.Drawing.Color.DarkRed;
            this.label44.Location = new System.Drawing.Point(29, 36);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(343, 29);
            this.label44.TabIndex = 712;
            this.label44.Text = "Simulación de columnas de 14 aciertos";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.BackColor = System.Drawing.Color.Bisque;
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.txDesvTipica);
            this.groupBox1.Controls.Add(this.label32);
            this.groupBox1.Controls.Add(this.txLNmedia);
            this.groupBox1.Controls.Add(this.label21);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(32, 68);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(716, 355);
            this.groupBox1.TabIndex = 709;
            this.groupBox1.TabStop = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.txNumAleatorias);
            this.groupBox2.Controls.Add(this.btGenerarAleatorias);
            this.groupBox2.Controls.Add(this.btGuardarAleatorias);
            this.groupBox2.Controls.Add(this.label34);
            this.groupBox2.Controls.Add(this.txLNMin);
            this.groupBox2.Controls.Add(this.label33);
            this.groupBox2.Controls.Add(this.txDTdeseada);
            this.groupBox2.Controls.Add(this.btAbrirAleatorias);
            this.groupBox2.Controls.Add(this.txFicheroAleatorias);
            this.groupBox2.Controls.Add(this.rbAleatorias);
            this.groupBox2.Controls.Add(this.rbFichero);
            this.groupBox2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox2.Location = new System.Drawing.Point(88, 24);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(536, 208);
            this.groupBox2.TabIndex = 721;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Origen de las columnas";
            // 
            // label17
            // 
            this.label17.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label17.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.ForeColor = System.Drawing.Color.Black;
            this.label17.Location = new System.Drawing.Point(219, 76);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(158, 21);
            this.label17.TabIndex = 728;
            this.label17.Text = "Número de columnas";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txNumAleatorias
            // 
            this.txNumAleatorias.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txNumAleatorias.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txNumAleatorias.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txNumAleatorias.ForeColor = System.Drawing.Color.Black;
            this.txNumAleatorias.Location = new System.Drawing.Point(380, 76);
            this.txNumAleatorias.Name = "txNumAleatorias";
            this.txNumAleatorias.Size = new System.Drawing.Size(112, 21);
            this.txNumAleatorias.TabIndex = 727;
            this.txNumAleatorias.Text = "1000";
            this.txNumAleatorias.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txNumAleatorias_KeyPress);
            // 
            // btGenerarAleatorias
            // 
            this.btGenerarAleatorias.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btGenerarAleatorias.BackColor = System.Drawing.Color.DarkSalmon;
            this.btGenerarAleatorias.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btGenerarAleatorias.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btGenerarAleatorias.ForeColor = System.Drawing.Color.Black;
            this.btGenerarAleatorias.Image = ((System.Drawing.Image)(resources.GetObject("btGenerarAleatorias.Image")));
            this.btGenerarAleatorias.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btGenerarAleatorias.Location = new System.Drawing.Point(375, 167);
            this.btGenerarAleatorias.Name = "btGenerarAleatorias";
            this.btGenerarAleatorias.Size = new System.Drawing.Size(145, 32);
            this.btGenerarAleatorias.TabIndex = 726;
            this.btGenerarAleatorias.Text = "&Generar columnas";
            this.btGenerarAleatorias.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btGenerarAleatorias.UseVisualStyleBackColor = false;
            this.btGenerarAleatorias.Click += new System.EventHandler(this.btGenerarAleatorias_Click);
            // 
            // btGuardarAleatorias
            // 
            this.btGuardarAleatorias.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btGuardarAleatorias.BackColor = System.Drawing.Color.LightSalmon;
            this.btGuardarAleatorias.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btGuardarAleatorias.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btGuardarAleatorias.ForeColor = System.Drawing.Color.Black;
            this.btGuardarAleatorias.Image = ((System.Drawing.Image)(resources.GetObject("btGuardarAleatorias.Image")));
            this.btGuardarAleatorias.Location = new System.Drawing.Point(493, 76);
            this.btGuardarAleatorias.Name = "btGuardarAleatorias";
            this.btGuardarAleatorias.Size = new System.Drawing.Size(24, 21);
            this.btGuardarAleatorias.TabIndex = 725;
            this.btGuardarAleatorias.UseVisualStyleBackColor = false;
            this.btGuardarAleatorias.Click += new System.EventHandler(this.btGuardarAleatorias_Click);
            // 
            // label34
            // 
            this.label34.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label34.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label34.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label34.ForeColor = System.Drawing.Color.Black;
            this.label34.Location = new System.Drawing.Point(380, 144);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(92, 21);
            this.label34.TabIndex = 724;
            this.label34.Text = "LN mínimo";
            this.label34.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label34.Visible = false;
            // 
            // txLNMin
            // 
            this.txLNMin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txLNMin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txLNMin.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txLNMin.ForeColor = System.Drawing.Color.Black;
            this.txLNMin.Location = new System.Drawing.Point(472, 144);
            this.txLNMin.Name = "txLNMin";
            this.txLNMin.Size = new System.Drawing.Size(48, 21);
            this.txLNMin.TabIndex = 723;
            this.txLNMin.Text = "-11";
            this.txLNMin.Visible = false;
            this.txLNMin.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txLN_KeyPress);
            // 
            // label33
            // 
            this.label33.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label33.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label33.ForeColor = System.Drawing.Color.Black;
            this.label33.Location = new System.Drawing.Point(216, 104);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(161, 21);
            this.label33.TabIndex = 722;
            this.label33.Text = "Desviación Típica deseada";
            this.label33.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label33.Visible = false;
            // 
            // txDTdeseada
            // 
            this.txDTdeseada.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txDTdeseada.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txDTdeseada.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txDTdeseada.ForeColor = System.Drawing.Color.Black;
            this.txDTdeseada.Location = new System.Drawing.Point(380, 104);
            this.txDTdeseada.Name = "txDTdeseada";
            this.txDTdeseada.Size = new System.Drawing.Size(112, 21);
            this.txDTdeseada.TabIndex = 721;
            this.txDTdeseada.Text = "1,965561";
            this.txDTdeseada.Visible = false;
            this.txDTdeseada.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txLN_KeyPress);
            // 
            // btAbrirAleatorias
            // 
            this.btAbrirAleatorias.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btAbrirAleatorias.BackColor = System.Drawing.Color.LightSalmon;
            this.btAbrirAleatorias.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btAbrirAleatorias.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btAbrirAleatorias.ForeColor = System.Drawing.Color.Black;
            this.btAbrirAleatorias.Image = ((System.Drawing.Image)(resources.GetObject("btAbrirAleatorias.Image")));
            this.btAbrirAleatorias.Location = new System.Drawing.Point(493, 36);
            this.btAbrirAleatorias.Name = "btAbrirAleatorias";
            this.btAbrirAleatorias.Size = new System.Drawing.Size(24, 21);
            this.btAbrirAleatorias.TabIndex = 712;
            this.btAbrirAleatorias.UseVisualStyleBackColor = false;
            this.btAbrirAleatorias.Click += new System.EventHandler(this.btAbrirAleatorias_Click);
            // 
            // txFicheroAleatorias
            // 
            this.txFicheroAleatorias.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txFicheroAleatorias.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txFicheroAleatorias.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txFicheroAleatorias.ForeColor = System.Drawing.Color.Black;
            this.txFicheroAleatorias.Location = new System.Drawing.Point(84, 36);
            this.txFicheroAleatorias.Name = "txFicheroAleatorias";
            this.txFicheroAleatorias.Size = new System.Drawing.Size(408, 21);
            this.txFicheroAleatorias.TabIndex = 2;
            // 
            // rbAleatorias
            // 
            this.rbAleatorias.Checked = true;
            this.rbAleatorias.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbAleatorias.ForeColor = System.Drawing.Color.Black;
            this.rbAleatorias.Location = new System.Drawing.Point(16, 73);
            this.rbAleatorias.Name = "rbAleatorias";
            this.rbAleatorias.Size = new System.Drawing.Size(153, 20);
            this.rbAleatorias.TabIndex = 1;
            this.rbAleatorias.TabStop = true;
            this.rbAleatorias.Text = "Columnas aleatorias";
            this.rbAleatorias.CheckedChanged += new System.EventHandler(this.rbAleatorias_CheckedChanged);
            // 
            // rbFichero
            // 
            this.rbFichero.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbFichero.ForeColor = System.Drawing.Color.Black;
            this.rbFichero.Location = new System.Drawing.Point(16, 36);
            this.rbFichero.Name = "rbFichero";
            this.rbFichero.Size = new System.Drawing.Size(68, 20);
            this.rbFichero.TabIndex = 0;
            this.rbFichero.Text = "Fichero";
            this.rbFichero.CheckedChanged += new System.EventHandler(this.rbFichero_CheckedChanged);
            // 
            // txDesvTipica
            // 
            this.txDesvTipica.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txDesvTipica.BackColor = System.Drawing.Color.LemonChiffon;
            this.txDesvTipica.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txDesvTipica.Enabled = false;
            this.txDesvTipica.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txDesvTipica.Location = new System.Drawing.Point(536, 248);
            this.txDesvTipica.Name = "txDesvTipica";
            this.txDesvTipica.Size = new System.Drawing.Size(84, 18);
            this.txDesvTipica.TabIndex = 716;
            // 
            // label32
            // 
            this.label32.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label32.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label32.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label32.Location = new System.Drawing.Point(382, 246);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(148, 20);
            this.label32.TabIndex = 715;
            this.label32.Text = "Desviación Típica obtenida";
            this.label32.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txLNmedia
            // 
            this.txLNmedia.BackColor = System.Drawing.Color.LemonChiffon;
            this.txLNmedia.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txLNmedia.Enabled = false;
            this.txLNmedia.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txLNmedia.Location = new System.Drawing.Point(256, 248);
            this.txLNmedia.Name = "txLNmedia";
            this.txLNmedia.Size = new System.Drawing.Size(84, 18);
            this.txLNmedia.TabIndex = 714;
            this.txLNmedia.TextChanged += new System.EventHandler(this.txLNmedia_TextChanged);
            // 
            // label21
            // 
            this.label21.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label21.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.Location = new System.Drawing.Point(89, 246);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(168, 20);
            this.label21.TabIndex = 713;
            this.label21.Text = "LN Probabilidad Media obtenida";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tabPaso4
            // 
            this.tabPaso4.BackColor = System.Drawing.Color.Bisque;
            this.tabPaso4.Controls.Add(this.groupBox4);
            this.tabPaso4.Controls.Add(this.lblContaSeleccionadas);
            this.tabPaso4.Controls.Add(this.btFiltrar);
            this.tabPaso4.Controls.Add(this.txNumFila);
            this.tabPaso4.Controls.Add(this.label53);
            this.tabPaso4.Controls.Add(this.lblNewBase142);
            this.tabPaso4.Controls.Add(this.lblNewBase132);
            this.tabPaso4.Controls.Add(this.lblNewBase122);
            this.tabPaso4.Controls.Add(this.lblNewBase112);
            this.tabPaso4.Controls.Add(this.lblNewBase102);
            this.tabPaso4.Controls.Add(this.lblNewBase92);
            this.tabPaso4.Controls.Add(this.lblNewBase82);
            this.tabPaso4.Controls.Add(this.lblNewBase72);
            this.tabPaso4.Controls.Add(this.lblNewBase62);
            this.tabPaso4.Controls.Add(this.lblNewBase52);
            this.tabPaso4.Controls.Add(this.lblNewBase42);
            this.tabPaso4.Controls.Add(this.lblNewBase32);
            this.tabPaso4.Controls.Add(this.lblNewBase22);
            this.tabPaso4.Controls.Add(this.lblNewBase12);
            this.tabPaso4.Controls.Add(this.lblNewBase14X);
            this.tabPaso4.Controls.Add(this.lblNewBase13X);
            this.tabPaso4.Controls.Add(this.lblNewBase12X);
            this.tabPaso4.Controls.Add(this.lblNewBase11X);
            this.tabPaso4.Controls.Add(this.lblNewBase10X);
            this.tabPaso4.Controls.Add(this.lblNewBase9X);
            this.tabPaso4.Controls.Add(this.lblNewBase8X);
            this.tabPaso4.Controls.Add(this.lblNewBase7X);
            this.tabPaso4.Controls.Add(this.lblNewBase6X);
            this.tabPaso4.Controls.Add(this.lblNewBase5X);
            this.tabPaso4.Controls.Add(this.lblNewBase4X);
            this.tabPaso4.Controls.Add(this.lblNewBase3X);
            this.tabPaso4.Controls.Add(this.lblNewBase2X);
            this.tabPaso4.Controls.Add(this.lblNewBase1X);
            this.tabPaso4.Controls.Add(this.lblNewBase141);
            this.tabPaso4.Controls.Add(this.lblNewBase131);
            this.tabPaso4.Controls.Add(this.lblNewBase121);
            this.tabPaso4.Controls.Add(this.lblNewBase111);
            this.tabPaso4.Controls.Add(this.lblNewBase101);
            this.tabPaso4.Controls.Add(this.lblNewBase91);
            this.tabPaso4.Controls.Add(this.lblNewBase81);
            this.tabPaso4.Controls.Add(this.lblNewBase71);
            this.tabPaso4.Controls.Add(this.lblNewBase61);
            this.tabPaso4.Controls.Add(this.lblNewBase51);
            this.tabPaso4.Controls.Add(this.lblNewBase41);
            this.tabPaso4.Controls.Add(this.lblNewBase31);
            this.tabPaso4.Controls.Add(this.lblNewBase21);
            this.tabPaso4.Controls.Add(this.lblNewBase11);
            this.tabPaso4.Controls.Add(this.label52);
            this.tabPaso4.Controls.Add(this.label51);
            this.tabPaso4.Controls.Add(this.txColumna);
            this.tabPaso4.Controls.Add(this.grOpcionAnalisis);
            this.tabPaso4.Controls.Add(this.grPremiosAacumular);
            this.tabPaso4.Controls.Add(this.btGrabar);
            this.tabPaso4.Controls.Add(this.label49);
            this.tabPaso4.Controls.Add(this.lblVecesBeneficio);
            this.tabPaso4.Controls.Add(this.label47);
            this.tabPaso4.Controls.Add(this.lblVecesPremiada);
            this.tabPaso4.Controls.Add(this.label45);
            this.tabPaso4.Controls.Add(this.label41);
            this.tabPaso4.Controls.Add(this.lblRecuperacion);
            this.tabPaso4.Controls.Add(this.label39);
            this.tabPaso4.Controls.Add(this.lblPremioTotal);
            this.tabPaso4.Controls.Add(this.lblGastoTotal);
            this.tabPaso4.Controls.Add(this.label38);
            this.tabPaso4.Controls.Add(this.lblNumColumnas);
            this.tabPaso4.Controls.Add(this.label37);
            this.tabPaso4.Controls.Add(this.dgResultadoEscrutinio);
            this.tabPaso4.Location = new System.Drawing.Point(4, 23);
            this.tabPaso4.Name = "tabPaso4";
            this.tabPaso4.Size = new System.Drawing.Size(768, 485);
            this.tabPaso4.TabIndex = 3;
            this.tabPaso4.Text = "Paso 4 (Escrutinios)";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btEliminarFilas);
            this.groupBox4.Controls.Add(this.rbParaCadaFila);
            this.groupBox4.Controls.Add(this.rbSoloFilaActual);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.txDiferencias);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox4.Location = new System.Drawing.Point(12, 71);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(562, 84);
            this.groupBox4.TabIndex = 906;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Eliminación de filas";
            // 
            // btEliminarFilas
            // 
            this.btEliminarFilas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btEliminarFilas.BackColor = System.Drawing.Color.DarkSalmon;
            this.btEliminarFilas.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btEliminarFilas.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btEliminarFilas.ForeColor = System.Drawing.Color.Black;
            this.btEliminarFilas.Image = ((System.Drawing.Image)(resources.GetObject("btEliminarFilas.Image")));
            this.btEliminarFilas.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btEliminarFilas.Location = new System.Drawing.Point(9, 41);
            this.btEliminarFilas.Name = "btEliminarFilas";
            this.btEliminarFilas.Size = new System.Drawing.Size(96, 32);
            this.btEliminarFilas.TabIndex = 911;
            this.btEliminarFilas.Text = "&Simplificar";
            this.btEliminarFilas.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btEliminarFilas.UseVisualStyleBackColor = false;
            this.btEliminarFilas.Click += new System.EventHandler(this.btEliminarFilas_Click);
            // 
            // rbParaCadaFila
            // 
            this.rbParaCadaFila.AutoSize = true;
            this.rbParaCadaFila.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbParaCadaFila.ForeColor = System.Drawing.Color.Black;
            this.rbParaCadaFila.Location = new System.Drawing.Point(344, 50);
            this.rbParaCadaFila.Name = "rbParaCadaFila";
            this.rbParaCadaFila.Size = new System.Drawing.Size(217, 17);
            this.rbParaCadaFila.TabIndex = 910;
            this.rbParaCadaFila.Text = "Respecto a cada fila seleccionada";
            this.rbParaCadaFila.UseVisualStyleBackColor = true;
            // 
            // rbSoloFilaActual
            // 
            this.rbSoloFilaActual.AutoSize = true;
            this.rbSoloFilaActual.Checked = true;
            this.rbSoloFilaActual.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbSoloFilaActual.ForeColor = System.Drawing.Color.Black;
            this.rbSoloFilaActual.Location = new System.Drawing.Point(344, 18);
            this.rbSoloFilaActual.Name = "rbSoloFilaActual";
            this.rbSoloFilaActual.Size = new System.Drawing.Size(194, 17);
            this.rbSoloFilaActual.TabIndex = 909;
            this.rbSoloFilaActual.TabStop = true;
            this.rbSoloFilaActual.Text = "Sólo respecto de la fila actual";
            this.rbSoloFilaActual.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.Transparent;
            this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(252, 18);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(69, 21);
            this.label4.TabIndex = 908;
            this.label4.Text = "diferencias";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txDiferencias
            // 
            this.txDiferencias.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txDiferencias.ForeColor = System.Drawing.Color.Black;
            this.txDiferencias.Location = new System.Drawing.Point(220, 18);
            this.txDiferencias.Name = "txDiferencias";
            this.txDiferencias.Size = new System.Drawing.Size(29, 21);
            this.txDiferencias.TabIndex = 907;
            this.txDiferencias.Text = "2";
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.Transparent;
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(6, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(214, 21);
            this.label3.TabIndex = 906;
            this.label3.Text = "Eliminar las apuestas con menos de ";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblContaSeleccionadas
            // 
            this.lblContaSeleccionadas.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblContaSeleccionadas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblContaSeleccionadas.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContaSeleccionadas.Location = new System.Drawing.Point(62, 44);
            this.lblContaSeleccionadas.Name = "lblContaSeleccionadas";
            this.lblContaSeleccionadas.Size = new System.Drawing.Size(168, 24);
            this.lblContaSeleccionadas.TabIndex = 905;
            this.lblContaSeleccionadas.Text = "<-- Filtro de selección de filas";
            // 
            // btFiltrar
            // 
            this.btFiltrar.BackColor = System.Drawing.Color.Silver;
            this.btFiltrar.Enabled = false;
            this.btFiltrar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btFiltrar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btFiltrar.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btFiltrar.Image = ((System.Drawing.Image)(resources.GetObject("btFiltrar.Image")));
            this.btFiltrar.Location = new System.Drawing.Point(37, 44);
            this.btFiltrar.Name = "btFiltrar";
            this.btFiltrar.Size = new System.Drawing.Size(24, 24);
            this.btFiltrar.TabIndex = 904;
            this.toolTip1.SetToolTip(this.btFiltrar, "Filtro de selección de filas");
            this.btFiltrar.UseVisualStyleBackColor = false;
            this.btFiltrar.Click += new System.EventHandler(this.btFiltrar_Click);
            this.btFiltrar.EnabledChanged += new System.EventHandler(this.buttonEnabledChanged);
            // 
            // txNumFila
            // 
            this.txNumFila.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txNumFila.BackColor = System.Drawing.SystemColors.Info;
            this.txNumFila.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txNumFila.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txNumFila.Location = new System.Drawing.Point(684, 244);
            this.txNumFila.MaxLength = 14;
            this.txNumFila.Name = "txNumFila";
            this.txNumFila.Size = new System.Drawing.Size(76, 21);
            this.txNumFila.TabIndex = 903;
            // 
            // label53
            // 
            this.label53.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label53.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label53.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label53.ForeColor = System.Drawing.Color.Firebrick;
            this.label53.Location = new System.Drawing.Point(628, 244);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(56, 22);
            this.label53.TabIndex = 902;
            this.label53.Text = "Fila Nº";
            this.label53.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase142
            // 
            this.lblNewBase142.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase142.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase142.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase142.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase142.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase142.Location = new System.Drawing.Point(612, 400);
            this.lblNewBase142.Name = "lblNewBase142";
            this.lblNewBase142.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase142.TabIndex = 901;
            this.lblNewBase142.Tag = "2";
            this.lblNewBase142.Text = "2";
            this.lblNewBase142.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase132
            // 
            this.lblNewBase132.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase132.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase132.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase132.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase132.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase132.Location = new System.Drawing.Point(612, 384);
            this.lblNewBase132.Name = "lblNewBase132";
            this.lblNewBase132.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase132.TabIndex = 900;
            this.lblNewBase132.Tag = "2";
            this.lblNewBase132.Text = "2";
            this.lblNewBase132.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase122
            // 
            this.lblNewBase122.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase122.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase122.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase122.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase122.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase122.Location = new System.Drawing.Point(612, 368);
            this.lblNewBase122.Name = "lblNewBase122";
            this.lblNewBase122.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase122.TabIndex = 899;
            this.lblNewBase122.Tag = "2";
            this.lblNewBase122.Text = "2";
            this.lblNewBase122.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase112
            // 
            this.lblNewBase112.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase112.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase112.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase112.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase112.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase112.Location = new System.Drawing.Point(612, 348);
            this.lblNewBase112.Name = "lblNewBase112";
            this.lblNewBase112.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase112.TabIndex = 898;
            this.lblNewBase112.Tag = "2";
            this.lblNewBase112.Text = "2";
            this.lblNewBase112.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase102
            // 
            this.lblNewBase102.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase102.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase102.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase102.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase102.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase102.Location = new System.Drawing.Point(612, 332);
            this.lblNewBase102.Name = "lblNewBase102";
            this.lblNewBase102.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase102.TabIndex = 897;
            this.lblNewBase102.Tag = "2";
            this.lblNewBase102.Text = "2";
            this.lblNewBase102.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase92
            // 
            this.lblNewBase92.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase92.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase92.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase92.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase92.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase92.Location = new System.Drawing.Point(612, 316);
            this.lblNewBase92.Name = "lblNewBase92";
            this.lblNewBase92.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase92.TabIndex = 896;
            this.lblNewBase92.Tag = "2";
            this.lblNewBase92.Text = "2";
            this.lblNewBase92.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase82
            // 
            this.lblNewBase82.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase82.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase82.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase82.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase82.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase82.Location = new System.Drawing.Point(612, 296);
            this.lblNewBase82.Name = "lblNewBase82";
            this.lblNewBase82.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase82.TabIndex = 895;
            this.lblNewBase82.Tag = "2";
            this.lblNewBase82.Text = "2";
            this.lblNewBase82.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase72
            // 
            this.lblNewBase72.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase72.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase72.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase72.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase72.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase72.Location = new System.Drawing.Point(612, 280);
            this.lblNewBase72.Name = "lblNewBase72";
            this.lblNewBase72.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase72.TabIndex = 894;
            this.lblNewBase72.Tag = "2";
            this.lblNewBase72.Text = "2";
            this.lblNewBase72.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase62
            // 
            this.lblNewBase62.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase62.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase62.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase62.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase62.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase62.Location = new System.Drawing.Point(612, 264);
            this.lblNewBase62.Name = "lblNewBase62";
            this.lblNewBase62.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase62.TabIndex = 893;
            this.lblNewBase62.Tag = "2";
            this.lblNewBase62.Text = "2";
            this.lblNewBase62.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase52
            // 
            this.lblNewBase52.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase52.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase52.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase52.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase52.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase52.Location = new System.Drawing.Point(612, 248);
            this.lblNewBase52.Name = "lblNewBase52";
            this.lblNewBase52.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase52.TabIndex = 892;
            this.lblNewBase52.Tag = "2";
            this.lblNewBase52.Text = "2";
            this.lblNewBase52.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase42
            // 
            this.lblNewBase42.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase42.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase42.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase42.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase42.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase42.Location = new System.Drawing.Point(612, 228);
            this.lblNewBase42.Name = "lblNewBase42";
            this.lblNewBase42.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase42.TabIndex = 891;
            this.lblNewBase42.Tag = "2";
            this.lblNewBase42.Text = "2";
            this.lblNewBase42.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase32
            // 
            this.lblNewBase32.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase32.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase32.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase32.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase32.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase32.Location = new System.Drawing.Point(612, 212);
            this.lblNewBase32.Name = "lblNewBase32";
            this.lblNewBase32.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase32.TabIndex = 890;
            this.lblNewBase32.Tag = "2";
            this.lblNewBase32.Text = "2";
            this.lblNewBase32.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase22
            // 
            this.lblNewBase22.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase22.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase22.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase22.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase22.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase22.Location = new System.Drawing.Point(612, 196);
            this.lblNewBase22.Name = "lblNewBase22";
            this.lblNewBase22.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase22.TabIndex = 889;
            this.lblNewBase22.Tag = "2";
            this.lblNewBase22.Text = "2";
            this.lblNewBase22.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase12
            // 
            this.lblNewBase12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase12.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase12.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase12.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase12.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase12.Location = new System.Drawing.Point(612, 180);
            this.lblNewBase12.Name = "lblNewBase12";
            this.lblNewBase12.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase12.TabIndex = 888;
            this.lblNewBase12.Tag = "2";
            this.lblNewBase12.Text = "2";
            this.lblNewBase12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase14X
            // 
            this.lblNewBase14X.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase14X.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase14X.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase14X.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase14X.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase14X.Location = new System.Drawing.Point(596, 400);
            this.lblNewBase14X.Name = "lblNewBase14X";
            this.lblNewBase14X.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase14X.TabIndex = 887;
            this.lblNewBase14X.Tag = "X";
            this.lblNewBase14X.Text = "X";
            this.lblNewBase14X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase13X
            // 
            this.lblNewBase13X.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase13X.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase13X.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase13X.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase13X.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase13X.Location = new System.Drawing.Point(596, 384);
            this.lblNewBase13X.Name = "lblNewBase13X";
            this.lblNewBase13X.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase13X.TabIndex = 886;
            this.lblNewBase13X.Tag = "X";
            this.lblNewBase13X.Text = "X";
            this.lblNewBase13X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase12X
            // 
            this.lblNewBase12X.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase12X.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase12X.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase12X.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase12X.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase12X.Location = new System.Drawing.Point(596, 368);
            this.lblNewBase12X.Name = "lblNewBase12X";
            this.lblNewBase12X.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase12X.TabIndex = 885;
            this.lblNewBase12X.Tag = "X";
            this.lblNewBase12X.Text = "X";
            this.lblNewBase12X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase11X
            // 
            this.lblNewBase11X.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase11X.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase11X.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase11X.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase11X.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase11X.Location = new System.Drawing.Point(596, 348);
            this.lblNewBase11X.Name = "lblNewBase11X";
            this.lblNewBase11X.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase11X.TabIndex = 884;
            this.lblNewBase11X.Tag = "X";
            this.lblNewBase11X.Text = "X";
            this.lblNewBase11X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase10X
            // 
            this.lblNewBase10X.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase10X.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase10X.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase10X.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase10X.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase10X.Location = new System.Drawing.Point(596, 332);
            this.lblNewBase10X.Name = "lblNewBase10X";
            this.lblNewBase10X.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase10X.TabIndex = 883;
            this.lblNewBase10X.Tag = "X";
            this.lblNewBase10X.Text = "X";
            this.lblNewBase10X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase9X
            // 
            this.lblNewBase9X.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase9X.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase9X.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase9X.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase9X.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase9X.Location = new System.Drawing.Point(596, 316);
            this.lblNewBase9X.Name = "lblNewBase9X";
            this.lblNewBase9X.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase9X.TabIndex = 882;
            this.lblNewBase9X.Tag = "X";
            this.lblNewBase9X.Text = "X";
            this.lblNewBase9X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase8X
            // 
            this.lblNewBase8X.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase8X.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase8X.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase8X.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase8X.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase8X.Location = new System.Drawing.Point(596, 296);
            this.lblNewBase8X.Name = "lblNewBase8X";
            this.lblNewBase8X.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase8X.TabIndex = 881;
            this.lblNewBase8X.Tag = "X";
            this.lblNewBase8X.Text = "X";
            this.lblNewBase8X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase7X
            // 
            this.lblNewBase7X.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase7X.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase7X.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase7X.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase7X.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase7X.Location = new System.Drawing.Point(596, 280);
            this.lblNewBase7X.Name = "lblNewBase7X";
            this.lblNewBase7X.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase7X.TabIndex = 880;
            this.lblNewBase7X.Tag = "X";
            this.lblNewBase7X.Text = "X";
            this.lblNewBase7X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase6X
            // 
            this.lblNewBase6X.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase6X.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase6X.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase6X.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase6X.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase6X.Location = new System.Drawing.Point(596, 264);
            this.lblNewBase6X.Name = "lblNewBase6X";
            this.lblNewBase6X.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase6X.TabIndex = 879;
            this.lblNewBase6X.Tag = "X";
            this.lblNewBase6X.Text = "X";
            this.lblNewBase6X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase5X
            // 
            this.lblNewBase5X.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase5X.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase5X.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase5X.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase5X.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase5X.Location = new System.Drawing.Point(596, 248);
            this.lblNewBase5X.Name = "lblNewBase5X";
            this.lblNewBase5X.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase5X.TabIndex = 878;
            this.lblNewBase5X.Tag = "X";
            this.lblNewBase5X.Text = "X";
            this.lblNewBase5X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase4X
            // 
            this.lblNewBase4X.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase4X.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase4X.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase4X.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase4X.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase4X.Location = new System.Drawing.Point(596, 228);
            this.lblNewBase4X.Name = "lblNewBase4X";
            this.lblNewBase4X.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase4X.TabIndex = 877;
            this.lblNewBase4X.Tag = "X";
            this.lblNewBase4X.Text = "X";
            this.lblNewBase4X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase3X
            // 
            this.lblNewBase3X.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase3X.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase3X.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase3X.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase3X.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase3X.Location = new System.Drawing.Point(596, 212);
            this.lblNewBase3X.Name = "lblNewBase3X";
            this.lblNewBase3X.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase3X.TabIndex = 876;
            this.lblNewBase3X.Tag = "X";
            this.lblNewBase3X.Text = "X";
            this.lblNewBase3X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase2X
            // 
            this.lblNewBase2X.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase2X.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase2X.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase2X.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase2X.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase2X.Location = new System.Drawing.Point(596, 196);
            this.lblNewBase2X.Name = "lblNewBase2X";
            this.lblNewBase2X.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase2X.TabIndex = 875;
            this.lblNewBase2X.Tag = "X";
            this.lblNewBase2X.Text = "X";
            this.lblNewBase2X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase1X
            // 
            this.lblNewBase1X.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase1X.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase1X.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase1X.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase1X.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase1X.Location = new System.Drawing.Point(596, 180);
            this.lblNewBase1X.Name = "lblNewBase1X";
            this.lblNewBase1X.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase1X.TabIndex = 874;
            this.lblNewBase1X.Tag = "X";
            this.lblNewBase1X.Text = "X";
            this.lblNewBase1X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase141
            // 
            this.lblNewBase141.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase141.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase141.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase141.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase141.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase141.Location = new System.Drawing.Point(580, 400);
            this.lblNewBase141.Name = "lblNewBase141";
            this.lblNewBase141.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase141.TabIndex = 873;
            this.lblNewBase141.Tag = "1";
            this.lblNewBase141.Text = "1";
            this.lblNewBase141.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase131
            // 
            this.lblNewBase131.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase131.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase131.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase131.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase131.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase131.Location = new System.Drawing.Point(580, 384);
            this.lblNewBase131.Name = "lblNewBase131";
            this.lblNewBase131.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase131.TabIndex = 872;
            this.lblNewBase131.Tag = "1";
            this.lblNewBase131.Text = "1";
            this.lblNewBase131.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase121
            // 
            this.lblNewBase121.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase121.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase121.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase121.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase121.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase121.Location = new System.Drawing.Point(580, 368);
            this.lblNewBase121.Name = "lblNewBase121";
            this.lblNewBase121.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase121.TabIndex = 871;
            this.lblNewBase121.Tag = "1";
            this.lblNewBase121.Text = "1";
            this.lblNewBase121.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase111
            // 
            this.lblNewBase111.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase111.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase111.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase111.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase111.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase111.Location = new System.Drawing.Point(580, 348);
            this.lblNewBase111.Name = "lblNewBase111";
            this.lblNewBase111.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase111.TabIndex = 870;
            this.lblNewBase111.Tag = "1";
            this.lblNewBase111.Text = "1";
            this.lblNewBase111.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase101
            // 
            this.lblNewBase101.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase101.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase101.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase101.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase101.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase101.Location = new System.Drawing.Point(580, 332);
            this.lblNewBase101.Name = "lblNewBase101";
            this.lblNewBase101.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase101.TabIndex = 869;
            this.lblNewBase101.Tag = "1";
            this.lblNewBase101.Text = "1";
            this.lblNewBase101.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase91
            // 
            this.lblNewBase91.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase91.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase91.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase91.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase91.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase91.Location = new System.Drawing.Point(580, 316);
            this.lblNewBase91.Name = "lblNewBase91";
            this.lblNewBase91.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase91.TabIndex = 868;
            this.lblNewBase91.Tag = "1";
            this.lblNewBase91.Text = "1";
            this.lblNewBase91.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase81
            // 
            this.lblNewBase81.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase81.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase81.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase81.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase81.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase81.Location = new System.Drawing.Point(580, 296);
            this.lblNewBase81.Name = "lblNewBase81";
            this.lblNewBase81.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase81.TabIndex = 867;
            this.lblNewBase81.Tag = "1";
            this.lblNewBase81.Text = "1";
            this.lblNewBase81.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase71
            // 
            this.lblNewBase71.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase71.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase71.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase71.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase71.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase71.Location = new System.Drawing.Point(580, 280);
            this.lblNewBase71.Name = "lblNewBase71";
            this.lblNewBase71.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase71.TabIndex = 866;
            this.lblNewBase71.Tag = "1";
            this.lblNewBase71.Text = "1";
            this.lblNewBase71.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase61
            // 
            this.lblNewBase61.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase61.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase61.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase61.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase61.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase61.Location = new System.Drawing.Point(580, 264);
            this.lblNewBase61.Name = "lblNewBase61";
            this.lblNewBase61.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase61.TabIndex = 865;
            this.lblNewBase61.Tag = "1";
            this.lblNewBase61.Text = "1";
            this.lblNewBase61.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase51
            // 
            this.lblNewBase51.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase51.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase51.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase51.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase51.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase51.Location = new System.Drawing.Point(580, 248);
            this.lblNewBase51.Name = "lblNewBase51";
            this.lblNewBase51.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase51.TabIndex = 864;
            this.lblNewBase51.Tag = "1";
            this.lblNewBase51.Text = "1";
            this.lblNewBase51.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase41
            // 
            this.lblNewBase41.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase41.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase41.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase41.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase41.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase41.Location = new System.Drawing.Point(580, 228);
            this.lblNewBase41.Name = "lblNewBase41";
            this.lblNewBase41.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase41.TabIndex = 863;
            this.lblNewBase41.Tag = "1";
            this.lblNewBase41.Text = "1";
            this.lblNewBase41.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase31
            // 
            this.lblNewBase31.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase31.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase31.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase31.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase31.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase31.Location = new System.Drawing.Point(580, 212);
            this.lblNewBase31.Name = "lblNewBase31";
            this.lblNewBase31.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase31.TabIndex = 862;
            this.lblNewBase31.Tag = "1";
            this.lblNewBase31.Text = "1";
            this.lblNewBase31.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase21
            // 
            this.lblNewBase21.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase21.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase21.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase21.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase21.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase21.Location = new System.Drawing.Point(580, 196);
            this.lblNewBase21.Name = "lblNewBase21";
            this.lblNewBase21.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase21.TabIndex = 861;
            this.lblNewBase21.Tag = "1";
            this.lblNewBase21.Text = "1";
            this.lblNewBase21.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblNewBase11
            // 
            this.lblNewBase11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNewBase11.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase11.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblNewBase11.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase11.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase11.Location = new System.Drawing.Point(580, 180);
            this.lblNewBase11.Name = "lblNewBase11";
            this.lblNewBase11.Size = new System.Drawing.Size(14, 14);
            this.lblNewBase11.TabIndex = 860;
            this.lblNewBase11.Tag = "1";
            this.lblNewBase11.Text = "1";
            this.lblNewBase11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label52
            // 
            this.label52.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label52.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label52.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label52.ForeColor = System.Drawing.Color.Firebrick;
            this.label52.Location = new System.Drawing.Point(628, 180);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(132, 36);
            this.label52.TabIndex = 859;
            this.label52.Text = "Columna de la fila seleccionada";
            this.label52.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label51
            // 
            this.label51.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label51.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label51.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label51.ForeColor = System.Drawing.Color.Firebrick;
            this.label51.Location = new System.Drawing.Point(580, 12);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(180, 28);
            this.label51.TabIndex = 858;
            this.label51.Text = "Resultados globales";
            this.label51.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txColumna
            // 
            this.txColumna.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txColumna.BackColor = System.Drawing.SystemColors.Info;
            this.txColumna.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txColumna.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txColumna.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txColumna.Location = new System.Drawing.Point(628, 216);
            this.txColumna.MaxLength = 14;
            this.txColumna.Name = "txColumna";
            this.txColumna.Size = new System.Drawing.Size(132, 21);
            this.txColumna.TabIndex = 857;
            this.txColumna.TextChanged += new System.EventHandler(this.txColumna_TextChanged);
            // 
            // grOpcionAnalisis
            // 
            this.grOpcionAnalisis.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.grOpcionAnalisis.BackColor = System.Drawing.Color.Bisque;
            this.grOpcionAnalisis.Controls.Add(this.rbOptionJornadas);
            this.grOpcionAnalisis.Controls.Add(this.rbOptionEspecial);
            this.grOpcionAnalisis.Controls.Add(this.rbOpcionColumnas);
            this.grOpcionAnalisis.Controls.Add(this.rbOpcionCombinacion);
            this.grOpcionAnalisis.Controls.Add(this.btnOK);
            this.grOpcionAnalisis.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.grOpcionAnalisis.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grOpcionAnalisis.ForeColor = System.Drawing.Color.Maroon;
            this.grOpcionAnalisis.Location = new System.Drawing.Point(636, 277);
            this.grOpcionAnalisis.Name = "grOpcionAnalisis";
            this.grOpcionAnalisis.Size = new System.Drawing.Size(128, 166);
            this.grOpcionAnalisis.TabIndex = 856;
            this.grOpcionAnalisis.TabStop = false;
            this.grOpcionAnalisis.Text = "Tipo de análisis";
            // 
            // rbOptionJornadas
            // 
            this.rbOptionJornadas.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbOptionJornadas.ForeColor = System.Drawing.Color.Black;
            this.rbOptionJornadas.Location = new System.Drawing.Point(8, 103);
            this.rbOptionJornadas.Name = "rbOptionJornadas";
            this.rbOptionJornadas.Size = new System.Drawing.Size(84, 16);
            this.rbOptionJornadas.TabIndex = 734;
            this.rbOptionJornadas.Text = "Jornadas";
            // 
            // rbOptionEspecial
            // 
            this.rbOptionEspecial.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbOptionEspecial.ForeColor = System.Drawing.Color.Black;
            this.rbOptionEspecial.Location = new System.Drawing.Point(8, 53);
            this.rbOptionEspecial.Name = "rbOptionEspecial";
            this.rbOptionEspecial.Size = new System.Drawing.Size(112, 16);
            this.rbOptionEspecial.TabIndex = 733;
            this.rbOptionEspecial.Text = "Autoescrutinio";
            this.rbOptionEspecial.CheckedChanged += new System.EventHandler(this.rbOptionGeneric_CheckedChanged);
            // 
            // rbOpcionColumnas
            // 
            this.rbOpcionColumnas.Checked = true;
            this.rbOpcionColumnas.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbOpcionColumnas.ForeColor = System.Drawing.Color.Black;
            this.rbOpcionColumnas.Location = new System.Drawing.Point(8, 78);
            this.rbOpcionColumnas.Name = "rbOpcionColumnas";
            this.rbOpcionColumnas.Size = new System.Drawing.Size(84, 16);
            this.rbOpcionColumnas.TabIndex = 1;
            this.rbOpcionColumnas.TabStop = true;
            this.rbOpcionColumnas.Text = "Columnas";
            this.rbOpcionColumnas.CheckedChanged += new System.EventHandler(this.rbOptionGeneric_CheckedChanged);
            // 
            // rbOpcionCombinacion
            // 
            this.rbOpcionCombinacion.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbOpcionCombinacion.ForeColor = System.Drawing.Color.Black;
            this.rbOpcionCombinacion.Location = new System.Drawing.Point(8, 28);
            this.rbOpcionCombinacion.Name = "rbOpcionCombinacion";
            this.rbOpcionCombinacion.Size = new System.Drawing.Size(100, 16);
            this.rbOpcionCombinacion.TabIndex = 0;
            this.rbOpcionCombinacion.Text = "Combinación";
            this.rbOpcionCombinacion.CheckedChanged += new System.EventHandler(this.rbOptionGeneric_CheckedChanged);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnOK.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.ForeColor = System.Drawing.Color.Black;
            this.btnOK.Image = ((System.Drawing.Image)(resources.GetObject("btnOK.Image")));
            this.btnOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOK.Location = new System.Drawing.Point(20, 126);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(96, 32);
            this.btnOK.TabIndex = 732;
            this.btnOK.Text = "&Analizar";
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // grPremiosAacumular
            // 
            this.grPremiosAacumular.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grPremiosAacumular.BackColor = System.Drawing.Color.Bisque;
            this.grPremiosAacumular.Controls.Add(this.chAcumula10);
            this.grPremiosAacumular.Controls.Add(this.chAcumula11);
            this.grPremiosAacumular.Controls.Add(this.chAcumula12);
            this.grPremiosAacumular.Controls.Add(this.chAcumula13);
            this.grPremiosAacumular.Controls.Add(this.chAcumula14);
            this.grPremiosAacumular.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grPremiosAacumular.ForeColor = System.Drawing.Color.Maroon;
            this.grPremiosAacumular.Location = new System.Drawing.Point(300, 8);
            this.grPremiosAacumular.Name = "grPremiosAacumular";
            this.grPremiosAacumular.Size = new System.Drawing.Size(276, 60);
            this.grPremiosAacumular.TabIndex = 855;
            this.grPremiosAacumular.TabStop = false;
            this.grPremiosAacumular.Text = "Premios acumulados que se consideran";
            // 
            // chAcumula10
            // 
            this.chAcumula10.Checked = true;
            this.chAcumula10.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chAcumula10.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chAcumula10.ForeColor = System.Drawing.Color.Black;
            this.chAcumula10.Location = new System.Drawing.Point(220, 32);
            this.chAcumula10.Name = "chAcumula10";
            this.chAcumula10.Size = new System.Drawing.Size(40, 16);
            this.chAcumula10.TabIndex = 4;
            this.chAcumula10.Tag = "4";
            this.chAcumula10.Text = "10";
            this.chAcumula10.CheckedChanged += new System.EventHandler(this.genericAcumula_CheckedChanged);
            // 
            // chAcumula11
            // 
            this.chAcumula11.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chAcumula11.ForeColor = System.Drawing.Color.Black;
            this.chAcumula11.Location = new System.Drawing.Point(168, 32);
            this.chAcumula11.Name = "chAcumula11";
            this.chAcumula11.Size = new System.Drawing.Size(48, 16);
            this.chAcumula11.TabIndex = 3;
            this.chAcumula11.Tag = "3";
            this.chAcumula11.Text = "11";
            this.chAcumula11.CheckedChanged += new System.EventHandler(this.genericAcumula_CheckedChanged);
            // 
            // chAcumula12
            // 
            this.chAcumula12.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chAcumula12.ForeColor = System.Drawing.Color.Black;
            this.chAcumula12.Location = new System.Drawing.Point(116, 32);
            this.chAcumula12.Name = "chAcumula12";
            this.chAcumula12.Size = new System.Drawing.Size(48, 16);
            this.chAcumula12.TabIndex = 2;
            this.chAcumula12.Tag = "2";
            this.chAcumula12.Text = "12";
            this.chAcumula12.CheckedChanged += new System.EventHandler(this.genericAcumula_CheckedChanged);
            // 
            // chAcumula13
            // 
            this.chAcumula13.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chAcumula13.ForeColor = System.Drawing.Color.Black;
            this.chAcumula13.Location = new System.Drawing.Point(64, 32);
            this.chAcumula13.Name = "chAcumula13";
            this.chAcumula13.Size = new System.Drawing.Size(48, 16);
            this.chAcumula13.TabIndex = 1;
            this.chAcumula13.Tag = "1";
            this.chAcumula13.Text = "13";
            this.chAcumula13.CheckedChanged += new System.EventHandler(this.genericAcumula_CheckedChanged);
            // 
            // chAcumula14
            // 
            this.chAcumula14.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chAcumula14.ForeColor = System.Drawing.Color.Black;
            this.chAcumula14.Location = new System.Drawing.Point(12, 32);
            this.chAcumula14.Name = "chAcumula14";
            this.chAcumula14.Size = new System.Drawing.Size(48, 16);
            this.chAcumula14.TabIndex = 0;
            this.chAcumula14.Tag = "0";
            this.chAcumula14.Text = "14";
            this.chAcumula14.CheckedChanged += new System.EventHandler(this.genericAcumula_CheckedChanged);
            // 
            // btGrabar
            // 
            this.btGrabar.BackColor = System.Drawing.Color.Silver;
            this.btGrabar.Enabled = false;
            this.btGrabar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btGrabar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btGrabar.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btGrabar.Image = ((System.Drawing.Image)(resources.GetObject("btGrabar.Image")));
            this.btGrabar.Location = new System.Drawing.Point(12, 44);
            this.btGrabar.Name = "btGrabar";
            this.btGrabar.Size = new System.Drawing.Size(24, 24);
            this.btGrabar.TabIndex = 854;
            this.btGrabar.UseVisualStyleBackColor = false;
            this.btGrabar.Click += new System.EventHandler(this.btGrabar_Click);
            this.btGrabar.EnabledChanged += new System.EventHandler(this.buttonEnabledChanged);
            // 
            // label49
            // 
            this.label49.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label49.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label49.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label49.Location = new System.Drawing.Point(580, 128);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(88, 20);
            this.label49.TabIndex = 850;
            this.label49.Text = "Veces beneficio";
            // 
            // lblVecesBeneficio
            // 
            this.lblVecesBeneficio.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVecesBeneficio.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblVecesBeneficio.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblVecesBeneficio.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVecesBeneficio.Location = new System.Drawing.Point(668, 128);
            this.lblVecesBeneficio.Name = "lblVecesBeneficio";
            this.lblVecesBeneficio.Size = new System.Drawing.Size(92, 20);
            this.lblVecesBeneficio.TabIndex = 849;
            this.lblVecesBeneficio.Text = "0";
            // 
            // label47
            // 
            this.label47.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label47.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label47.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label47.Location = new System.Drawing.Point(580, 107);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(88, 20);
            this.label47.TabIndex = 848;
            this.label47.Text = "Veces premiada";
            // 
            // lblVecesPremiada
            // 
            this.lblVecesPremiada.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblVecesPremiada.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblVecesPremiada.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblVecesPremiada.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblVecesPremiada.Location = new System.Drawing.Point(668, 107);
            this.lblVecesPremiada.Name = "lblVecesPremiada";
            this.lblVecesPremiada.Size = new System.Drawing.Size(92, 20);
            this.lblVecesPremiada.TabIndex = 847;
            this.lblVecesPremiada.Text = "0";
            // 
            // label45
            // 
            this.label45.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label45.ForeColor = System.Drawing.Color.DarkRed;
            this.label45.Location = new System.Drawing.Point(9, 18);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(168, 21);
            this.label45.TabIndex = 733;
            this.label45.Text = "Resultados";
            // 
            // label41
            // 
            this.label41.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label41.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label41.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label41.Location = new System.Drawing.Point(580, 151);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(88, 20);
            this.label41.TabIndex = 731;
            this.label41.Text = "% recuperado";
            // 
            // lblRecuperacion
            // 
            this.lblRecuperacion.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRecuperacion.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblRecuperacion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblRecuperacion.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRecuperacion.Location = new System.Drawing.Point(668, 151);
            this.lblRecuperacion.Name = "lblRecuperacion";
            this.lblRecuperacion.Size = new System.Drawing.Size(92, 20);
            this.lblRecuperacion.TabIndex = 730;
            this.lblRecuperacion.Text = "0";
            // 
            // label39
            // 
            this.label39.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label39.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label39.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label39.Location = new System.Drawing.Point(580, 86);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(88, 20);
            this.label39.TabIndex = 729;
            this.label39.Text = "Premio total";
            // 
            // lblPremioTotal
            // 
            this.lblPremioTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblPremioTotal.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblPremioTotal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPremioTotal.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPremioTotal.Location = new System.Drawing.Point(668, 86);
            this.lblPremioTotal.Name = "lblPremioTotal";
            this.lblPremioTotal.Size = new System.Drawing.Size(92, 20);
            this.lblPremioTotal.TabIndex = 728;
            this.lblPremioTotal.Text = "0";
            // 
            // lblGastoTotal
            // 
            this.lblGastoTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblGastoTotal.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblGastoTotal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblGastoTotal.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblGastoTotal.Location = new System.Drawing.Point(668, 65);
            this.lblGastoTotal.Name = "lblGastoTotal";
            this.lblGastoTotal.Size = new System.Drawing.Size(92, 20);
            this.lblGastoTotal.TabIndex = 727;
            this.lblGastoTotal.Text = "0";
            // 
            // label38
            // 
            this.label38.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label38.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label38.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label38.Location = new System.Drawing.Point(580, 65);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(88, 20);
            this.label38.TabIndex = 726;
            this.label38.Text = "Gasto total";
            // 
            // lblNumColumnas
            // 
            this.lblNumColumnas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNumColumnas.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNumColumnas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNumColumnas.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNumColumnas.Location = new System.Drawing.Point(668, 44);
            this.lblNumColumnas.Name = "lblNumColumnas";
            this.lblNumColumnas.Size = new System.Drawing.Size(92, 20);
            this.lblNumColumnas.TabIndex = 725;
            this.lblNumColumnas.Text = "0";
            // 
            // label37
            // 
            this.label37.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label37.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label37.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label37.Location = new System.Drawing.Point(580, 44);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(88, 20);
            this.label37.TabIndex = 724;
            this.label37.Text = "Nº columnas";
            // 
            // dgResultadoEscrutinio
            // 
            this.dgResultadoEscrutinio.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgResultadoEscrutinio.CaptionVisible = false;
            this.dgResultadoEscrutinio.DataMember = "";
            this.dgResultadoEscrutinio.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.dgResultadoEscrutinio.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgResultadoEscrutinio.Location = new System.Drawing.Point(8, 161);
            this.dgResultadoEscrutinio.Name = "dgResultadoEscrutinio";
            this.dgResultadoEscrutinio.PreferredColumnWidth = 150;
            this.dgResultadoEscrutinio.Size = new System.Drawing.Size(568, 316);
            this.dgResultadoEscrutinio.TabIndex = 705;
            this.dgResultadoEscrutinio.Paint += new System.Windows.Forms.PaintEventHandler(this.dgResultadoEscrutinio_Paint);
            this.dgResultadoEscrutinio.CurrentCellChanged += new System.EventHandler(this.dgResultadoEscrutinio_CurrentCellChanged);
            this.dgResultadoEscrutinio.MouseDown += new System.Windows.Forms.MouseEventHandler(this.myDataGrid_MouseDown);
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.Location = new System.Drawing.Point(8, 520);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(776, 12);
            this.progressBar1.TabIndex = 845;
            // 
            // btsiguiente
            // 
            this.btsiguiente.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btsiguiente.BackColor = System.Drawing.Color.DarkSalmon;
            this.btsiguiente.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btsiguiente.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btsiguiente.Image = ((System.Drawing.Image)(resources.GetObject("btsiguiente.Image")));
            this.btsiguiente.Location = new System.Drawing.Point(628, 482);
            this.btsiguiente.Name = "btsiguiente";
            this.btsiguiente.Size = new System.Drawing.Size(28, 23);
            this.btsiguiente.TabIndex = 846;
            this.btsiguiente.UseVisualStyleBackColor = false;
            this.btsiguiente.Click += new System.EventHandler(this.btsiguiente_Click);
            // 
            // btanterior
            // 
            this.btanterior.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btanterior.BackColor = System.Drawing.Color.DarkSalmon;
            this.btanterior.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btanterior.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btanterior.Image = ((System.Drawing.Image)(resources.GetObject("btanterior.Image")));
            this.btanterior.Location = new System.Drawing.Point(596, 482);
            this.btanterior.Name = "btanterior";
            this.btanterior.Size = new System.Drawing.Size(28, 23);
            this.btanterior.TabIndex = 847;
            this.btanterior.UseVisualStyleBackColor = false;
            this.btanterior.Visible = false;
            this.btanterior.Click += new System.EventHandler(this.btanterior_Click);
            // 
            // BancoPruebasFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(792, 566);
            this.Controls.Add(this.btanterior);
            this.Controls.Add(this.btsiguiente);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusBar1);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "BancoPruebasFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Simulador de escrutinios (Banco de Pruebas)";
            // StatusBarPanel controls don't support ISupportInitialize in .NET 8
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).EndInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel2)).EndInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel3)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPaso1.ResumeLayout(false);
            this.tabPaso1.PerformLayout();
            this.tabPaso2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabPaso3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPaso4.ResumeLayout(false);
            this.tabPaso4.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.grOpcionAnalisis.ResumeLayout(false);
            this.grPremiosAacumular.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgResultadoEscrutinio)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void HabilitarCalcular()
		{
			btnOK.Enabled = false;
			if (txFicheroEntrada.Text !="" && ColumnasAleatorias !=null) btnOK.Enabled = true;
			if (txFicheroEntrada.Text !="" && rbOptionEspecial.Checked ==true) btnOK.Enabled = true;
			if (btnOK.Enabled ==false) statusBarPanel3.Text ="Faltan datos"; else statusBarPanel3.Text ="Preparado";
		}
		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			salida=true;
			this.Close ();
		}


		private void btnOK_Click(object sender, System.EventArgs e)
		{
            btEliminarFilas.Enabled = false;
			if (rbOpcionCombinacion.Checked) EscrutarCombinacion(); 
			if (rbOpcionColumnas.Checked) EscrutarColumnas();
			if (rbOptionEspecial.Checked) EscrutarColumnasAutoEscrutinio();
            if (rbOptionJornadas.Checked) EscrutarCombinacionPorJornadas();
		}
		private void EscrutarCombinacion()
		{
			Res= new Resultados [15];
			int[] aciertos = new int [15];
			double[] ingresos = new double[5] {0,0,0,0,0};
			Columnas = new ArrayList();
			int contador=0;
			int VecesPremiada=0;
			int VecesBeneficio=0;
			double PremioTotal=0;
			string archivoEntrada=txFicheroEntrada.Text;
			salida=false;
			CargarFicheroDeColumnas(archivoEntrada, ref Columnas);
			double CosteCombinacion= Columnas.Count *PrecioApuesta;
			lblNumColumnas.Text = Columnas.Count.ToString ();
			double GastoTotal =CosteCombinacion * NumAleatorias;
			double PremioCombinacion;
			progressBar1.Maximum = ColumnasAleatorias.Count;
			progressBar1.Value =0;
			lblGastoTotal.Text = GastoTotal.ToString();
			//---Leer Porcentajes --------------
			v=controlPorcentajesApostados.Valores ;
			Porcentajes Pct = new Porcentajes(v);
			pa= Pct.ValoresBase100();
			bool premiada;
			double beneficio;
			foreach (int i in ColumnasAleatorias)
			{
				contador++;
				progressBar1.Value =contador;
				CalcularPremios(i);
				PremioCombinacion=0;
				premiada=false;
				beneficio=-CosteCombinacion;
				foreach (int j in Columnas )
				{
                    byte NumAciertos = Aciertos(i, j);
                    aciertos[NumAciertos]++;
                    if (NumAciertos > 9) 
					{
                        ingresos[14 - NumAciertos] += Premios[14 - NumAciertos];
						premiada=true;
                        PremioCombinacion += ingresos[14 - NumAciertos];
                        beneficio += Premios[14 - NumAciertos];
					}
				}
				if (premiada==true)VecesPremiada++;
				if (beneficio>0)VecesBeneficio++;
				Application.DoEvents();
				if (salida) return;
			}
			for(int i=0;i<15;i++){Res[i]=new Resultados ((i).ToString (),aciertos[i]);}
			for(int i=10;i<15;i++)
			{
				Res[i].Premios= Math.Round (ingresos [14-i],0);
				PremioTotal +=ingresos [14-i];
			}
			PremioTotal=Math.Round (PremioTotal,0);
			lblPremioTotal.Text = PremioTotal.ToString ();
			
			lblVecesBeneficio.Text = VecesBeneficio.ToString ();
			lblVecesPremiada.Text = VecesPremiada.ToString ();

			double Recuperacion = Math.Round (100*PremioTotal/GastoTotal,0);
			lblRecuperacion.Text = Recuperacion.ToString () + "%";

			dgResultadoEscrutinioDataBind();

		}
        private void EscrutarCombinacionPorJornadas()
        {
            ResultadoPorJornadas = new ResultadosJornada[ColumnasAleatorias.Count];
            double SaldoInicial = 0;
            Columnas = new ArrayList();
            int contador = 0;

            string archivoEntrada = txFicheroEntrada.Text;
            salida = false;
            CargarFicheroDeColumnas(archivoEntrada, ref Columnas);
            double CosteCombinacion = Columnas.Count * PrecioApuesta;
            lblNumColumnas.Text = Columnas.Count.ToString();

            progressBar1.Maximum = ColumnasAleatorias.Count;
            progressBar1.Value = 0;

            //---Leer Porcentajes --------------
            v = controlPorcentajesApostados.Valores;
            Porcentajes Pct = new Porcentajes(v);
            pa = Pct.ValoresBase100();

            foreach (int i in ColumnasAleatorias)
            {
                int[] aciertos = new int[5];
                double[] ingresos = new double[5];
                int indice = contador;
                contador++;
                progressBar1.Value = contador;
                CalcularPremios(i);

                foreach (int j in Columnas)
                {
                    byte NumAciertos = Aciertos(i, j);
                    
                    if (NumAciertos > 9)
                    {
                        aciertos[14-NumAciertos]++;
                        ingresos[14 - NumAciertos] += Premios[14 - NumAciertos];
                    }
                }
                ResultadoPorJornadas[indice] = new ResultadosJornada(contador,SaldoInicial,CosteCombinacion,ingresos,aciertos);
                SaldoInicial = ResultadoPorJornadas[indice].Saldo;

                Application.DoEvents();
                if (salida) return;
            }

            dgResultadoEscrutinioPorJornadasDataBind();
        }
		protected void InicializaGridResultadoEscrutinio()
		{			
			DataGridTableStyle tableStyle = new DataGridTableStyle();
			tableStyle.MappingName = "Resultados[]";
			tableStyle.ColumnHeadersVisible = true;
			
			// Crear Columnas 
			// MappingName tiene que ser igual a cada una de las "properties"
			// de la clase Combinacion.

            
			DataGridTextBoxColumn cs = null;
			tableStyle.AlternatingBackColor = Color.LightGray;

			//		Concepto
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "Concepto";
			cs.HeaderText = "Aciertos";
			cs.Width = 45;
			tableStyle.GridColumnStyles.Add(cs);

			//		Valor
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "Valor";
			cs.HeaderText = "Nº veces";
			cs.Width = 60;
			tableStyle.GridColumnStyles.Add(cs);

			//		Premios
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "Premios";
			cs.HeaderText = "Premios";
			cs.Width = 60;
			tableStyle.GridColumnStyles.Add(cs);

			//		PremiosMedios
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "PremiosMedios";
			cs.HeaderText = "Premios Medios";
			cs.Width = 110;
			tableStyle.GridColumnStyles.Add(cs);
			

			dgResultadoEscrutinio.TableStyles.Add(tableStyle);			
		}

		protected void InicializaGridResultadoEscrutinioPorColumnas()
		{			
			DataGridTableStyle tableStyle = new DataGridTableStyle();
			tableStyle.MappingName = "ResultadosPorColumna[]";
			tableStyle.ColumnHeadersVisible = true;
			
			// Crear Columnas 
			// MappingName tiene que ser igual a cada una de las "properties"
			// de la clase Combinacion.

            
			DataGridTextBoxColumn cs = null;
			tableStyle.AlternatingBackColor = Color.LightGray;

			//		Num
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "Num";
			cs.HeaderText = "Nºcol.";
			cs.Width = 55;
			tableStyle.GridColumnStyles.Add(cs);

			//		Veces14
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "Veces14";
			cs.HeaderText = "Nº14";
			cs.Width = 33;
			tableStyle.GridColumnStyles.Add(cs);

			//		Veces13
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "Veces13";
			cs.HeaderText = "Nº13";
			cs.Width = 35;
			tableStyle.GridColumnStyles.Add(cs);

			//		Veces12
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "Veces12";
			cs.HeaderText = "Nº12";
			cs.Width = 37;
			tableStyle.GridColumnStyles.Add(cs);

			//		Veces11
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "Veces11";
			cs.HeaderText = "Nº11";
			cs.Width = 40;
			tableStyle.GridColumnStyles.Add(cs);

			//		Veces10
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "Veces10";
			cs.HeaderText = "Nº10";
			cs.Width = 45;
			tableStyle.GridColumnStyles.Add(cs);

			//		VecesAcumulado
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "VecesAcumulado";
			cs.HeaderText = "Nºacumulado";
			cs.Width = 45;
			tableStyle.GridColumnStyles.Add(cs);
			//		PremioDe14
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "PremioDe14";
			cs.HeaderText = "Premio 14";
			cs.Width = 60;
			tableStyle.GridColumnStyles.Add(cs);

			//		PremioDe13
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "PremioDe13";
			cs.HeaderText = "Premio 13";
			cs.Width = 57;
			tableStyle.GridColumnStyles.Add(cs);

			//		PremioDe12
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "PremioDe12";
			cs.HeaderText = "Premio 12";
			cs.Width = 55;
			tableStyle.GridColumnStyles.Add(cs);

			//		PremioDe11
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "PremioDe11";
			cs.HeaderText = "Premio 11";
			cs.Width = 53;
			tableStyle.GridColumnStyles.Add(cs);

			//		PremioDe10
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "PremioDe10";
			cs.HeaderText = "Premio 10";
			cs.Width = 50;
			tableStyle.GridColumnStyles.Add(cs);

			//		PremioAcumulado
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "PremioAcumulado";
			cs.HeaderText = "Premio Acumulado";
			cs.Width = 60;
			tableStyle.GridColumnStyles.Add(cs);

			//		PremioUnitarioDe14
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "PremioUnitarioDe14";
			cs.HeaderText = "14 medio";
			cs.Width = 57;
			tableStyle.GridColumnStyles.Add(cs);

			//		PremioUnitarioDe13
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "PremioUnitarioDe13";
			cs.HeaderText = "13 medio";
			cs.Width = 50;
			tableStyle.GridColumnStyles.Add(cs);

			//		PremioUnitarioDe12
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "PremioUnitarioDe12";
			cs.HeaderText = "12 medio";
			cs.Width = 40;
			tableStyle.GridColumnStyles.Add(cs);

			//		PremioUnitarioDe11
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "PremioUnitarioDe11";
			cs.HeaderText = "11 medio";
			cs.Width = 30;
			tableStyle.GridColumnStyles.Add(cs);

			//		PremioUnitarioDe10
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "PremioUnitarioDe10";
			cs.HeaderText = "10 medio";
			cs.Width = 20;
			tableStyle.GridColumnStyles.Add(cs);

			//	Recuperacion
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "Recuperacion";
			cs.HeaderText = "Recuperacion";
			cs.Width = 40;
			tableStyle.GridColumnStyles.Add(cs);


			dgResultadoEscrutinio.TableStyles.Add(tableStyle);			
		}

        protected void InicializaGridResultadoEscrutinioPorJornadas()
        {
            DataGridTableStyle tableStyle = new DataGridTableStyle();
            tableStyle.MappingName = "ResultadosJornada[]";
            tableStyle.ColumnHeadersVisible = true;

            // Crear Columnas 
            // MappingName tiene que ser igual a cada una de las "properties"
            // de la clase Combinacion.


            DataGridTextBoxColumn cs = null;
            tableStyle.AlternatingBackColor = Color.LightGray;

            //		Jornada
            cs = new DataGridTextBoxColumn();
            cs.MappingName = "Jornada";
            cs.HeaderText = "Jornada";
            cs.Width = 55;
            cs.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
            tableStyle.GridColumnStyles.Add(cs);

            //		AciertosDe14
            cs = new DataGridTextBoxColumn();
            cs.MappingName = "AciertosDe14";
            cs.HeaderText = "Ac. 14";
            cs.Width = 45;
            cs.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
            tableStyle.GridColumnStyles.Add(cs);

            //		AciertosDe13
            cs = new DataGridTextBoxColumn();
            cs.MappingName = "AciertosDe13";
            cs.HeaderText = "Ac. 13";
            cs.Width = 48;
            cs.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
            tableStyle.GridColumnStyles.Add(cs);

            //		AciertosDe12
            cs = new DataGridTextBoxColumn();
            cs.MappingName = "AciertosDe12";
            cs.HeaderText = "Ac. 12";
            cs.Width = 51;
            cs.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
            tableStyle.GridColumnStyles.Add(cs);

            //		AciertosDe11
            cs = new DataGridTextBoxColumn();
            cs.MappingName = "AciertosDe11";
            cs.HeaderText = "Ac. 11";
            cs.Width = 53;
            cs.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
            tableStyle.GridColumnStyles.Add(cs);

            //		AciertosDe10
            cs = new DataGridTextBoxColumn();
            cs.MappingName = "AciertosDe10";
            cs.HeaderText = "Ac. 10";
            cs.Width = 56;
            cs.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
            tableStyle.GridColumnStyles.Add(cs);

            //		TotalPremios
            cs = new DataGridTextBoxColumn();
            cs.MappingName = "TotalPremios";
            cs.HeaderText = "Importe premios";
            cs.Width = 75;
            cs.Format = "#,##0.00";
            cs.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
            tableStyle.GridColumnStyles.Add(cs);

            //		Saldo
            cs = new DataGridTextBoxColumn();
            cs.MappingName = "Saldo";
            cs.HeaderText = "Saldo";
            cs.Width = 60;
            cs.Format = "#,##0.00";
            cs.Alignment = System.Windows.Forms.HorizontalAlignment.Center;
            tableStyle.GridColumnStyles.Add(cs);

            dgResultadoEscrutinio.TableStyles.Add(tableStyle);
        }

        private void dgResultadoEscrutinioDataBind()
        {
            dgResultadoEscrutinio.DataSource = null;
            dgResultadoEscrutinio.DataSource = Res;
            pointInCell00 = new Point(dgResultadoEscrutinio.GetCellBounds(0, 0).X + 4, dgResultadoEscrutinio.GetCellBounds(0, 0).Y + 4);
            DataGridVacia = false;
            dgResultadoEscrutinio.Refresh();
        }
        private void dgResultadoEscrutinioPorJornadasDataBind()
		{
            if (ResultadoPorJornadas != null)
            {
                dgResultadoEscrutinio.DataSource = null;
                dgResultadoEscrutinio.DataSource = ResultadoPorJornadas;
                pointInCell00 = new Point(dgResultadoEscrutinio.GetCellBounds(0, 0).X + 4, dgResultadoEscrutinio.GetCellBounds(0, 0).Y + 4);
                DataGridVacia = false;
                dgResultadoEscrutinio.Refresh();
            }
		}

		private void dgResultadoEscrutinioDataBindPorColumnas()
		{	
			if(ResCol != null)
			{
				dgResultadoEscrutinio.DataSource =null;
				dgResultadoEscrutinio.DataSource =ResCol;	
				pointInCell00 = new Point(dgResultadoEscrutinio.GetCellBounds(0,0).X + 4, dgResultadoEscrutinio.GetCellBounds(0,0).Y + 4);
				DataGridVacia=false;
				dgResultadoEscrutinio.Refresh ();
			}
		}
		private void btGenerarAleatorias_Click(object sender, System.EventArgs e)
		{
			int z,b;
			byte j;
			double Prob=1;
			double Mpx=0;
			double LN=0;
			double LNM=0;
			double LNMedia=0;
			double LNVariMedia=0;
			double LNDTmedia=0;
			double DTDeseada=Convert.ToDouble (txDTdeseada.Text);
			double LNMin=Convert.ToDouble (txLNMin.Text);
			
			ColumnasAleatorias = new ArrayList();
			Random  aleatorio = new Random (unchecked((int) DateTime.Now.Ticks));
			NumAleatorias=Convert.ToInt32 (txNumAleatorias.Text);
			p=controlPorcentajesReales .Valores ;
			v=controlPorcentajesApostados .Valores ;
			for (z=0;z<NumAleatorias;z++)
			{
				Prob=1;
				b=0;
				for (j=0;j<14;j++)
				{
					double num=aleatorio.NextDouble();

					if(num<p[j,0]/100)
					{
						Prob *=v[j,0]/100;
					}
					else if(num<((p[j,0]+p[j,1])/100))
					{
						Prob *=v[j,1]/100;
						b+=PotDe3[j];
					}
					else
					{
						Prob *=v[j,2]/100;
						b+=DosPotDe3[j];
					}
				}


  				Mpx = (Mpx * z + Prob) / (z+1);
				LN = Math.Log(Prob);
				LNM = Math.Log(Mpx);
  				LNMedia = (LNMedia * z + LN) / (z+1);
  				LNVariMedia = (LNVariMedia * z + ((LN - LNM) * (LN - LNM))) / (z+1);
  				LNDTmedia = Math.Sqrt(LNVariMedia);
				ColumnasAleatorias.Add (b);
			}
			txDesvTipica.Text = LNDTmedia.ToString ();
			txLNmedia.Text =LNMedia.ToString ();
		}

		private void btLeerColumnas_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog abreFiltroDialog = new OpenFileDialog();
            abreFiltroDialog.InitialDirectory = Application.StartupPath + "/Columnas/";
			abreFiltroDialog.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			if(abreFiltroDialog.ShowDialog () == DialogResult.OK)
			{		    	
				string archivoEntrada = abreFiltroDialog.FileName;		    	
				txFicheroEntrada.Text = archivoEntrada;
			}
			HabilitarCalcular ();
		}

		private void CargarFicheroDeColumnas(string archivoEntrada, ref ArrayList Columns)
		{
			statusBarPanel3.Text = Path.GetFileName(archivoEntrada);
			Application.DoEvents ();
            IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(archivoEntrada);
			ConvertidorDeBases col= new ConvertidorDeBases();
			Columns =new ArrayList() ;
			while( comBaseCols.SiguienteColumna() )
			{
				Columns.Add (col.ConvColumnaANumero  (comBaseCols.LeeColumnaSinComas()));
			}
			comBaseCols.Cerrar();	
		}

		private void btAbrirAleatorias_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog abreFiltroDialog = new OpenFileDialog();
            abreFiltroDialog.InitialDirectory = Application.StartupPath + "/Columnas/";
			abreFiltroDialog.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			if(abreFiltroDialog.ShowDialog () == DialogResult.OK)
			{	
	    		if(ColumnasAleatorias !=null) ColumnasAleatorias.Clear();
				string archivoEntrada = abreFiltroDialog.FileName;		    	
				CargarFicheroDeColumnas(archivoEntrada, ref ColumnasAleatorias);
				txFicheroAleatorias.Text = archivoEntrada;
				NumAleatorias=ColumnasAleatorias.Count;
				txNumAleatorias.Text =NumAleatorias.ToString () ;
				CalculaDTMedia();
				rbAleatorias.Checked =false;
				rbFichero.Checked =true;
			}
			HabilitarCalcular ();

		}
		private byte Aciertos (int col1, int col2)
		{
			byte a=0;
			for (int Partido = 0;Partido<14;Partido++)
			{
				if(((col1 / PotDe3 [Partido]) % 3)==((col2 / PotDe3 [Partido]) % 3)) a++;;
			}
			return a;
		}
		private void CalcularPremios(int ColumnaGanadora)
		{
			int Partido;
			int i;
			int signo;

			//'---- probabilidad del 14 de la 1ª apuesta i 
			// cálculo de valores complementarios --------

			ProbabilidadCategoria14=1;
			for(i=0;i<5;i++)SumaProbabilidades[i]=0;

			for (Partido = 0;Partido<14; Partido++)
			{
				signo=(ColumnaGanadora / PotDe3 [Partido]) % 3;
				ProbabilidadCategoria14 *=pa[Partido, signo];
				Cr[Partido] = (1-pa[Partido, signo])/pa[Partido, signo];
			}
			Premios[0]=Math.Round (PctDestinadoAPremiosCategoria [0]*PrecioApuesta /ProbabilidadCategoria14,2);

			CalcularSumaProbabilidades (ProbabilidadCategoria14,0,4);

			for (i=1; i<5;i++)
			{
				Premios[i]=Math.Round (PctDestinadoAPremiosCategoria[i]*PrecioApuesta/SumaProbabilidades[i],2);
			}
			CorreccionesDeCalculo();
		}
		private void CalcularSumaProbabilidades(double pProb, int PosicionInicial,int pProfundidad)
		{
			double Prob=0;
			Profundidad++;
       
			for (int Partido = PosicionInicial;Partido<14;Partido++)
			{
				Prob = pProb * Cr[Partido];
				SumaProbabilidades[Profundidad] +=Prob;
				if (Profundidad < pProfundidad)
				{
					CalcularSumaProbabilidades (Prob, Partido + 1, pProfundidad);
				}
			}
			Profundidad--;
		}
        private void CorreccionesDeCalculo()
        {
            for (byte i = 0; i < 5; i++)
            {
                if (Premios[i] > DestinadoAPremiosCategoria[i]) Premios[i] = DestinadoAPremiosCategoria[i];
            }
            if (Premios[4] < 1) Premios[4] = 0;
        }

		private void btCalcularReales_Click(object sender, System.EventArgs e)
		{
			v=controlPorcentajesApostados.Valores ;
			Porcentajes Pct = new Porcentajes(v);
			float[,] va=  Pct.ValoresNeperianos();
			float Prob=0;
			//'---- probabilidad del 14 de la 1ª apuesta y 
			// cálculo de valores complementarios --------
			for (int Partido = 0;Partido<14; Partido++)
			{
				Prob +=va[Partido, 0];
				Cra[Partido,1] = (float) (va[Partido, 1] - va[Partido, 0]);
				Cra[Partido,2] = (float) (va[Partido, 2] - va[Partido, 0]);
			}
			Calcula14Triples(Prob);
			HabilitarCalcular ();
		}
		private void Calcula14Triples(float Prob)
		{
			
			Bits.SetAll (true);
			NumApuestas = 4782969;
			_LN =Convert.ToDouble(txLN.Text);
			int NumCol=Convert.ToInt32 (txNumCol.Text);
			progressBar1.Maximum = NumCol;
			progressBar1.Value = 0;
			statusBarPanel3.Text = "Calculando probabilidades ...";
			Application.DoEvents();
			Profundidad = 0;
			EncontrarDistantes1 (Prob, 0, 0, 14);
			Ap14T[0].ProbabilidadDiferencial  = Math.Abs (Prob - (float)_LN);
			Ap14T[0].Probabilidad = Prob;
			Ap14T[0].Columna = 0;

			statusBarPanel3.Text = "Ordenando apuestas ...";
			Application.DoEvents();
			ordena (0, 4782968);
			Array.Clear (p,0,42);
			for (int i=0; i<NumCol;i++)
			{
				for (byte partido=0;partido<14;partido++)
				{
					p[partido,(Ap14T[i].Columna / PotDe3 [partido]) % 3]++;
					progressBar1.Value =i;
				}
			}
			for (byte partido=0;partido<14;partido++)
			{
				p[partido,0] /=(NumCol/100);
				p[partido,1] /=(NumCol/100);
				p[partido,2] /=(NumCol/100);
			}
			statusBarPanel2.Text = "Valoración real obtenida a partir de % apostado";
			Application.DoEvents();

			controlPorcentajesReales .Valores =p;
		}
		private void EncontrarDistantes1(float pProb,int IndiceInicial, int PosicionInicial,int pProfundidad)
		{
			int Partido;
			int z;
			int Indice;
			float Prob;
			Profundidad++;
        
			//'--encontramos las apuestas que se diferencian en un solo signo ----
			for (Partido = PosicionInicial;Partido<14;Partido++)
			{
				for (z = 1;z<3;z++)
				{
					Indice = IndiceInicial + PotDe3 [Partido] * z;
					Prob = pProb + Cra[Partido, z];
					
					if (Bits[Indice])
					{
						Ap14T[Indice].Columna = Indice;
						Ap14T[Indice].ProbabilidadDiferencial  = Math.Abs(Prob - (float)_LN);
						Ap14T[Indice].Probabilidad = Prob;
					}
					else
					{
						Ap14T[Indice].ProbabilidadDiferencial = (float) 3E+7;
						Ap14T[Indice].Probabilidad = Prob;
					}

					
					if (Profundidad < pProfundidad)
					{
						EncontrarDistantes1 (Prob, Indice, Partido + 1, pProfundidad);
					}
				}
			}
			Profundidad--;
		}
		private void ordena(int izq, int der)
		{ 
			int i = 0, j = 0; 
			ApuestaProbableCentral x = new ApuestaProbableCentral();
			ApuestaProbableCentral aux= new ApuestaProbableCentral();
			i = izq; j = der; 
			x = Ap14T [ (izq + der) /2 ]; 
			do
			{ 
			while(Ap14T[i].ProbabilidadDiferencial < x.ProbabilidadDiferencial && j <= der)	i++; 
			while(x.ProbabilidadDiferencial  < Ap14T[j].ProbabilidadDiferencial && j > izq )j--; 
				if( i <= j )
				{ 
					aux = Ap14T[i]; 
					Ap14T[i] = Ap14T[j]; 
					Ap14T[j] = aux; 
					i++;  j--; 
				} 
			}while( i <= j ); 
			if( izq < j ) ordena(izq, j); 
			if( i < der ) ordena(i, der); 
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			tabControl1.SelectedIndex ++;
		}

		private void btSiguiente2_Click(object sender, System.EventArgs e)
		{
			tabControl1.SelectedIndex ++;
		}

		private void btAnterior2_Click(object sender, System.EventArgs e)
		{
			tabControl1.SelectedIndex --;
		}

		private void btAnterior3_Click(object sender, System.EventArgs e)
		{
			tabControl1.SelectedIndex --;
		}

		private void btAnterior4_Click(object sender, System.EventArgs e)
		{
			tabControl1.SelectedIndex --;
		}
		private double CalculaProbMedia()
		{
			int z;
			byte j;
			double Prob=1;
			double Mpx=0;
			double LN=0;
			double LNM=0;
			double LNMedia=0;
			double LNVariMedia=0;
			double LNDTmedia=0;
			Random  aleatorio = new Random (unchecked((int) DateTime.Now.Ticks));

			for (z=0;z<100000;z++)
			{
				Prob=1;
				for (j=0;j<14;j++)
				{
					double num=aleatorio.NextDouble();

					if(num<p[j,0]/100)
					{
						Prob *=v[j,0]/100;
					}
					else if(num<((p[j,0]+p[j,1])/100))
					{
						Prob *=v[j,1]/100;
					}
					else
					{
						Prob *=v[j,2]/100;
					}
				}

				Mpx = (Mpx * z + Prob) / (z+1);
				LN = Math.Log(Prob);
				LNM = Math.Log(Mpx);
				LNMedia = (LNMedia * z + LN) / (z+1);
				LNVariMedia = (LNVariMedia * z + ((LN - LNM) * (LN - LNM))) / (z+1);
				LNDTmedia = Math.Sqrt(LNVariMedia);
			}
			return LNMedia;
		}
		private void CalculaDTMedia()
		{
			byte j;
			double Prob=1;
			double Mpx=0;
			double LN=0;
			double LNM=0;
			double LNVariMedia=0;
			double LNDTmedia=0;
			double LNMedia=0;
			int signo;
			int contador=0;
			p=controlPorcentajesReales .Valores ;
			v=controlPorcentajesApostados .Valores ;

			foreach (int z in ColumnasAleatorias )
			{
				Prob=1;
				for (j=0;j<14;j++)
				{
					signo=((z / PotDe3 [j]) % 3);
					Prob *=v[j,signo]/100;
				}

				Mpx = (Mpx * contador + Prob) / (contador+1);
				LN = Math.Log(Prob);
				LNM = Math.Log(Mpx);
				LNMedia = (LNMedia * contador + LN) / (contador+1);
				LNVariMedia = (LNVariMedia * contador + ((LN - LNM) * (LN - LNM))) / (contador+1);
				LNDTmedia = Math.Sqrt(LNVariMedia);
				contador++;
			}
			txDesvTipica.Text = LNDTmedia.ToString ();
			txLNmedia.Text =LNMedia.ToString ();
		}

		private void txNumCol_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			Valida.Generic_KeyPress (TipoValidacionString.SoloNumeros, sender, e);
		}

		private void txNumAleatorias_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			Valida.Generic_KeyPress (TipoValidacionString.SoloNumeros, sender, e);
		}

		private void txLN_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			Valida.Generic_KeyPress (TipoValidacionString.SoloNumerosDecimalesConSigno, sender, e);
		}

		private void btGuardarAleatorias_Click(object sender, System.EventArgs e)
		{
			SaveFileDialog abreFiltroDialog = new SaveFileDialog();
            abreFiltroDialog.InitialDirectory = Application.StartupPath + "/Columnas/";
			abreFiltroDialog.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			if(abreFiltroDialog.ShowDialog () == DialogResult.OK)
			{
				string archivoSalida = abreFiltroDialog.FileName;		    	
				ConvertidorDeBases col =new ConvertidorDeBases();
                IArchivoColumnas comCols = new ArchivoColumnasTexto(archivoSalida);
				foreach (int nr in ColumnasAleatorias )
				{
					comCols.GuardarCols(nr);
				}
				comCols.Cerrar();	
			}
		}

		private void rbFichero_CheckedChanged(object sender, System.EventArgs e)
		{
			btGenerarAleatorias.Enabled =rbAleatorias.Checked ;
		}

		private void rbAleatorias_CheckedChanged(object sender, System.EventArgs e)
		{
			btGenerarAleatorias.Enabled =rbAleatorias.Checked ;
		}

		private void btEscrutarColumnas_Click(object sender, System.EventArgs e)
		{
			EscrutarColumnas();
		}
		private void EscrutarColumnas()
		{
			int[] aciertos = new int [15];
			double[] ingresos = new double[5] {0,0,0,0,0};
			Columnas = new ArrayList();
			int contador=0;
			int VecesPremiada=0;
			int VecesBeneficio=0;
			double PremioTotal=0;
			salida=false;
			string archivoEntrada=txFicheroEntrada.Text;
			CargarFicheroDeColumnas(archivoEntrada, ref Columnas);
			double CosteCombinacion= Columnas.Count *PrecioApuesta;
			NumApuestas=Columnas.Count;
			ResCol= new ResultadosPorColumna [NumApuestas+1];
			lblNumColumnas.Text = NumApuestas.ToString ();
			double GastoTotal =CosteCombinacion * NumAleatorias;
			double PremioCombinacion;
			int contacolumnas=0;
			progressBar1.Maximum = ColumnasAleatorias.Count;
			progressBar1.Value =0;
			lblGastoTotal.Text = GastoTotal.ToString();
			//---Leer Porcentajes --------------
			v=controlPorcentajesApostados .Valores ;
			Porcentajes Pct = new Porcentajes(v);
			pa= Pct.ValoresBase100();
			bool premiada;
			double beneficio;
			foreach (int j in Columnas )
			{
				ResCol[contador] = new ResultadosPorColumna (j,NumAleatorias);
				ResCol[contador].Num = (++contador).ToString ();
			}
			contador=0;
			foreach (int i in ColumnasAleatorias)
			{
				contador++;
				progressBar1.Value =contador;
				CalcularPremios(i);
				PremioCombinacion=0;
				premiada=false;
				beneficio=-CosteCombinacion;
				contacolumnas=0;
				foreach (int j in Columnas )
				{
					short Ac=Aciertos(i,j);
					aciertos[Ac]++;
					if(Ac>9) 
					{
						ResCol[contacolumnas].ContarPremio (14-Ac);
						ResCol[contacolumnas].SumarPremio (14-Ac,Premios [14-Ac]);
						premiada=true;
						
						PremioCombinacion +=ingresos[14-Ac];
						beneficio +=Premios [14-Ac];

					}
					contacolumnas++;
				}
				if (premiada==true)VecesPremiada++;
				if (beneficio>0)VecesBeneficio++;
				Application.DoEvents ();
				if (salida) return;
			}
			Array.Clear (Premios,0,5);
			for(int i=0;i<NumApuestas ;i++)
			{
				PremioTotal +=ResCol[i].PremioDe14+ResCol[i].PremioDe13+ResCol[i].PremioDe12+ResCol[i].PremioDe11+ResCol[i].PremioDe10 ;
				ResCol[i].Acumula = AcumularPremio;
				Premios[0] +=ResCol[i].PremioDe14;
				Premios[1] +=ResCol[i].PremioDe13;
				Premios[2] +=ResCol[i].PremioDe12;
				Premios[3] +=ResCol[i].PremioDe11;
				Premios[4] +=ResCol[i].PremioDe10;
			}
			//
			//---Obtención de maximos y mínimos ---------
			//
			MaxMin [0,0]=ResCol[0].Veces14;
			MaxMin [0,1]=MaxMin [0,0];
			MaxMin [1,0]=ResCol[0].Veces13;
			MaxMin [1,1]=MaxMin [1,0];
			MaxMin [2,0]=ResCol[0].Veces12;
			MaxMin [2,1]=MaxMin [2,0];
			MaxMin [3,0]=ResCol[0].Veces11;
			MaxMin [3,1]=MaxMin [3,0];
			MaxMin [4,0]=ResCol[0].Veces10;
			MaxMin [4,1]=MaxMin [4,0];
			MaxMin [5,0]=ResCol[0].VecesAcumulado ;
			MaxMin [5,1]=MaxMin [5,0];
			MaxMin [6,0]=ResCol[0].PremioDe14;
			MaxMin [6,1]=MaxMin [6,0];
			MaxMin [7,0]=ResCol[0].PremioDe13;
			MaxMin [7,1]=MaxMin [7,0];
			MaxMin [8,0]=ResCol[0].PremioDe12;
			MaxMin [8,1]=MaxMin [8,0];
			MaxMin [9,0]=ResCol[0].PremioDe11;
			MaxMin [9,1]=MaxMin [9,0];
			MaxMin [10,0]=ResCol[0].PremioDe10;
			MaxMin [10,1]=MaxMin [10,0];
			MaxMin [11,0]=ResCol[0].PremioAcumulado ;
			MaxMin [11,1]=MaxMin [11,0];
			MaxMin [12,0]=ResCol[0].PremioUnitarioDe14 ;
			MaxMin [12,1]=MaxMin [12,0];
			MaxMin [13,0]=ResCol[0].PremioUnitarioDe13 ;
			MaxMin [13,1]=MaxMin [13,0];
			MaxMin [14,0]=ResCol[0].PremioUnitarioDe12 ;
			MaxMin [14,1]=MaxMin [14,0];
			MaxMin [15,0]=ResCol[0].PremioUnitarioDe11 ;
			MaxMin [15,1]=MaxMin [15,0];
			MaxMin [16,0]=ResCol[0].PremioUnitarioDe10 ;
			MaxMin [16,1]=MaxMin [16,0];
			MaxMin [17,0]=Convert.ToDouble (ResCol[0].Recuperacion.Substring (0,ResCol[0].Recuperacion.Length - 2 ));
			MaxMin [17,1]=MaxMin [17,0];

			for(int i=0;i<NumApuestas; i++)
			{
				MaxMin [0,0]=Math.Min (ResCol[i].Veces14 , MaxMin[0,0]);
				MaxMin [0,1]=Math.Max (ResCol[i].Veces14 , MaxMin[0,1]);
				MaxMin [1,0]=Math.Min (ResCol[i].Veces13 , MaxMin[1,0]);
				MaxMin [1,1]=Math.Max (ResCol[i].Veces13 , MaxMin[1,1]);
				MaxMin [2,0]=Math.Min (ResCol[i].Veces12 , MaxMin[2,0]);
				MaxMin [2,1]=Math.Max (ResCol[i].Veces12 , MaxMin[2,1]);
				MaxMin [3,0]=Math.Min (ResCol[i].Veces11 , MaxMin[3,0]);
				MaxMin [3,1]=Math.Max (ResCol[i].Veces11 , MaxMin[3,1]);
				MaxMin [4,0]=Math.Min (ResCol[i].Veces10 , MaxMin[4,0]);
				MaxMin [4,1]=Math.Max (ResCol[i].Veces10 , MaxMin[4,1]);
				MaxMin [5,0]=Math.Min (ResCol[i].VecesAcumulado , MaxMin[5,0]);
				MaxMin [5,1]=Math.Max (ResCol[i].VecesAcumulado , MaxMin[5,1]);
				MaxMin [6,0]=Math.Min (ResCol[i].PremioDe14 , MaxMin[6,0]);
				MaxMin [6,1]=Math.Max (ResCol[i].PremioDe14 , MaxMin[6,1]);
				MaxMin [7,0]=Math.Min (ResCol[i].PremioDe13 , MaxMin[7,0]);
				MaxMin [7,1]=Math.Max (ResCol[i].PremioDe13 , MaxMin[7,1]);
				MaxMin [8,0]=Math.Min (ResCol[i].PremioDe12 , MaxMin[8,0]);
				MaxMin [8,1]=Math.Max (ResCol[i].PremioDe12 , MaxMin[8,1]);
				MaxMin [9,0]=Math.Min (ResCol[i].PremioDe11 , MaxMin[9,0]);
				MaxMin [9,1]=Math.Max (ResCol[i].PremioDe11 , MaxMin[9,1]);
				MaxMin [10,0]=Math.Min (ResCol[i].PremioDe10 , MaxMin[10,0]);
				MaxMin [10,1]=Math.Max (ResCol[i].PremioDe10 , MaxMin[10,1]);
				MaxMin [11,0]=Math.Min (ResCol[i].PremioAcumulado , MaxMin[11,0]);
				MaxMin [11,1]=Math.Max (ResCol[i].PremioAcumulado , MaxMin[11,1]);
				MaxMin [12,0]=Math.Min (ResCol[i].PremioUnitarioDe14 , MaxMin[12,0]);
				MaxMin [12,1]=Math.Max (ResCol[i].PremioUnitarioDe14 , MaxMin[12,1]);
				MaxMin [13,0]=Math.Min (ResCol[i].PremioUnitarioDe13 , MaxMin[13,0]);
				MaxMin [13,1]=Math.Max (ResCol[i].PremioUnitarioDe13 , MaxMin[13,1]);
				MaxMin [14,0]=Math.Min (ResCol[i].PremioUnitarioDe12 , MaxMin[14,0]);
				MaxMin [14,1]=Math.Max (ResCol[i].PremioUnitarioDe12 , MaxMin[14,1]);
				MaxMin [15,0]=Math.Min (ResCol[i].PremioUnitarioDe11 , MaxMin[15,0]);
				MaxMin [15,1]=Math.Max (ResCol[i].PremioUnitarioDe11 , MaxMin[15,1]);
				MaxMin [16,0]=Math.Min (ResCol[i].PremioUnitarioDe10 , MaxMin[16,0]);
				MaxMin [16,1]=Math.Max (ResCol[i].PremioUnitarioDe10 , MaxMin[16,1]);
				MaxMin [17,0]=Math.Min (Convert.ToDouble (ResCol[i].Recuperacion.Substring (0,ResCol[i].Recuperacion.Length - 2 )) , MaxMin[17,0]);
				MaxMin [17,1]=Math.Max (Convert.ToDouble (ResCol[i].Recuperacion.Substring (0,ResCol[i].Recuperacion.Length - 2 )) , MaxMin[17,1]);
			}
			ResCol[NumApuestas] = new ResultadosPorColumna (NumAleatorias,aciertos,Premios,AcumularPremio);
			PremioTotal=Math.Round (PremioTotal,0);
			lblPremioTotal.Text = PremioTotal.ToString ();
			
			lblVecesBeneficio.Text = VecesBeneficio.ToString ();
			lblVecesPremiada.Text = VecesPremiada.ToString ();

			double Recuperacion = Math.Round (100*PremioTotal/GastoTotal,0);
			lblRecuperacion.Text = Recuperacion.ToString () + "%";

			dgResultadoEscrutinioDataBindPorColumnas();
			ConvertidorDeBases col =new ConvertidorDeBases();
			txColumna.Text =col.ConvNumAColumna (Convert.ToInt32 (ResCol[0].Columna));
            btEliminarFilas.Enabled = true;
			btGrabar.Enabled =true;
			btFiltrar.Enabled =true;
			
		}
		private void EscrutarColumnasAutoEscrutinio()
		{
			int[] aciertos = new int [15];
			double[] ingresos = new double[5] {0,0,0,0,0};
			Columnas = new ArrayList();
			int contador=0;
			int VecesPremiada=0;
			int VecesBeneficio=0;
			double PremioTotal=0;
			salida=false;
			string archivoEntrada=txFicheroEntrada.Text;
			CargarFicheroDeColumnas(archivoEntrada, ref Columnas);
			double CosteCombinacion= Columnas.Count *PrecioApuesta;
			NumApuestas=Columnas.Count;
			ResCol= new ResultadosPorColumna [NumApuestas+1];
			lblNumColumnas.Text = NumApuestas.ToString ();
			double GastoTotal =CosteCombinacion;
			double PremioCombinacion;
			int contacolumnas=0;
			progressBar1.Maximum = NumApuestas;
			progressBar1.Value =0;
			lblGastoTotal.Text = GastoTotal.ToString();
			//---Leer Porcentajes --------------
			v=controlPorcentajesApostados .Valores ;
			Porcentajes Pct = new Porcentajes(v);
			pa= Pct.ValoresBase100();
			bool premiada;
			double beneficio;
			foreach (int j in Columnas )
			{
				ResCol[contador] = new ResultadosPorColumna (j,NumApuestas);
				ResCol[contador].Num = (++contador).ToString ();
			}
			contador=0;
			contacolumnas=0;
			foreach (int i in Columnas)
			{
				contador++;
				progressBar1.Value =contador;
				CalcularPremios(i);
				PremioCombinacion=0;
				premiada=false;
				beneficio=-CosteCombinacion;
				
				foreach (int j in Columnas )
				{
					short Ac=Aciertos(i,j);
					aciertos[Ac]++;
					if(Ac>9) 
					{
						ResCol[contacolumnas].ContarPremio (14-Ac);
						ResCol[contacolumnas].SumarPremio (14-Ac,Premios [14-Ac]);
						premiada=true;
						PremioCombinacion +=ingresos[14-Ac];
						beneficio +=Premios [14-Ac];
					}
				}
				contacolumnas++;
				if (premiada==true)VecesPremiada++;
				if (beneficio>0)VecesBeneficio++;
				Application.DoEvents ();
				if (salida) return;
			}
			Array.Clear (Premios,0,5);
			for(int i=0;i<NumApuestas ;i++)
			{
//				PremioTotal +=ResCol[i].PremioDe14+ResCol[i].PremioDe13+ResCol[i].PremioDe12+ResCol[i].PremioDe11+ResCol[i].PremioDe10 ;
				ResCol[i].Acumula = AcumularPremio;
				Premios[0] +=ResCol[i].PremioDe14;
				Premios[1] +=ResCol[i].PremioDe13;
				Premios[2] +=ResCol[i].PremioDe12;
				Premios[3] +=ResCol[i].PremioDe11;
				Premios[4] +=ResCol[i].PremioDe10;
			}
			//
			//---Obtención de maximos y mínimos ---------
			//
			MaxMin [0,0]=ResCol[0].Veces14;
			MaxMin [0,1]=MaxMin [0,0];
			MaxMin [1,0]=ResCol[0].Veces13;
			MaxMin [1,1]=MaxMin [1,0];
			MaxMin [2,0]=ResCol[0].Veces12;
			MaxMin [2,1]=MaxMin [2,0];
			MaxMin [3,0]=ResCol[0].Veces11;
			MaxMin [3,1]=MaxMin [3,0];
			MaxMin [4,0]=ResCol[0].Veces10;
			MaxMin [4,1]=MaxMin [4,0];
			MaxMin [5,0]=ResCol[0].VecesAcumulado ;
			MaxMin [5,1]=MaxMin [5,0];
			MaxMin [6,0]=ResCol[0].PremioDe14;
			MaxMin [6,1]=MaxMin [6,0];
			MaxMin [7,0]=ResCol[0].PremioDe13;
			MaxMin [7,1]=MaxMin [7,0];
			MaxMin [8,0]=ResCol[0].PremioDe12;
			MaxMin [8,1]=MaxMin [8,0];
			MaxMin [9,0]=ResCol[0].PremioDe11;
			MaxMin [9,1]=MaxMin [9,0];
			MaxMin [10,0]=ResCol[0].PremioDe10;
			MaxMin [10,1]=MaxMin [10,0];
			MaxMin [11,0]=ResCol[0].PremioAcumulado ;
			MaxMin [11,1]=MaxMin [11,0];
			MaxMin [12,0]=ResCol[0].PremioUnitarioDe14 ;
			MaxMin [12,1]=MaxMin [12,0];
			MaxMin [13,0]=ResCol[0].PremioUnitarioDe13 ;
			MaxMin [13,1]=MaxMin [13,0];
			MaxMin [14,0]=ResCol[0].PremioUnitarioDe12 ;
			MaxMin [14,1]=MaxMin [14,0];
			MaxMin [15,0]=ResCol[0].PremioUnitarioDe11 ;
			MaxMin [15,1]=MaxMin [15,0];
			MaxMin [16,0]=ResCol[0].PremioUnitarioDe10 ;
			MaxMin [16,1]=MaxMin [16,0];
			MaxMin [17,0]=Convert.ToDouble (ResCol[0].Recuperacion.Substring (0,ResCol[0].Recuperacion.Length - 2 ));
			MaxMin [17,1]=MaxMin [17,0];

			for(int i=0;i<NumApuestas; i++)
			{
				MaxMin [0,0]=Math.Min (ResCol[i].Veces14 , MaxMin[0,0]);
				MaxMin [0,1]=Math.Max (ResCol[i].Veces14 , MaxMin[0,1]);
				MaxMin [1,0]=Math.Min (ResCol[i].Veces13 , MaxMin[1,0]);
				MaxMin [1,1]=Math.Max (ResCol[i].Veces13 , MaxMin[1,1]);
				MaxMin [2,0]=Math.Min (ResCol[i].Veces12 , MaxMin[2,0]);
				MaxMin [2,1]=Math.Max (ResCol[i].Veces12 , MaxMin[2,1]);
				MaxMin [3,0]=Math.Min (ResCol[i].Veces11 , MaxMin[3,0]);
				MaxMin [3,1]=Math.Max (ResCol[i].Veces11 , MaxMin[3,1]);
				MaxMin [4,0]=Math.Min (ResCol[i].Veces10 , MaxMin[4,0]);
				MaxMin [4,1]=Math.Max (ResCol[i].Veces10 , MaxMin[4,1]);
				MaxMin [5,0]=Math.Min (ResCol[i].VecesAcumulado , MaxMin[5,0]);
				MaxMin [5,1]=Math.Max (ResCol[i].VecesAcumulado , MaxMin[5,1]);
				MaxMin [6,0]=Math.Min (ResCol[i].PremioDe14 , MaxMin[6,0]);
				MaxMin [6,1]=Math.Max (ResCol[i].PremioDe14 , MaxMin[6,1]);
				MaxMin [7,0]=Math.Min (ResCol[i].PremioDe13 , MaxMin[7,0]);
				MaxMin [7,1]=Math.Max (ResCol[i].PremioDe13 , MaxMin[7,1]);
				MaxMin [8,0]=Math.Min (ResCol[i].PremioDe12 , MaxMin[8,0]);
				MaxMin [8,1]=Math.Max (ResCol[i].PremioDe12 , MaxMin[8,1]);
				MaxMin [9,0]=Math.Min (ResCol[i].PremioDe11 , MaxMin[9,0]);
				MaxMin [9,1]=Math.Max (ResCol[i].PremioDe11 , MaxMin[9,1]);
				MaxMin [10,0]=Math.Min (ResCol[i].PremioDe10 , MaxMin[10,0]);
				MaxMin [10,1]=Math.Max (ResCol[i].PremioDe10 , MaxMin[10,1]);
				MaxMin [11,0]=Math.Min (ResCol[i].PremioAcumulado , MaxMin[11,0]);
				MaxMin [11,1]=Math.Max (ResCol[i].PremioAcumulado , MaxMin[11,1]);
				MaxMin [12,0]=Math.Min (ResCol[i].PremioUnitarioDe14 , MaxMin[12,0]);
				MaxMin [12,1]=Math.Max (ResCol[i].PremioUnitarioDe14 , MaxMin[12,1]);
				MaxMin [13,0]=Math.Min (ResCol[i].PremioUnitarioDe13 , MaxMin[13,0]);
				MaxMin [13,1]=Math.Max (ResCol[i].PremioUnitarioDe13 , MaxMin[13,1]);
				MaxMin [14,0]=Math.Min (ResCol[i].PremioUnitarioDe12 , MaxMin[14,0]);
				MaxMin [14,1]=Math.Max (ResCol[i].PremioUnitarioDe12 , MaxMin[14,1]);
				MaxMin [15,0]=Math.Min (ResCol[i].PremioUnitarioDe11 , MaxMin[15,0]);
				MaxMin [15,1]=Math.Max (ResCol[i].PremioUnitarioDe11 , MaxMin[15,1]);
				MaxMin [16,0]=Math.Min (ResCol[i].PremioUnitarioDe10 , MaxMin[16,0]);
				MaxMin [16,1]=Math.Max (ResCol[i].PremioUnitarioDe10 , MaxMin[16,1]);
				MaxMin [17,0]=Math.Min (Convert.ToDouble (ResCol[i].Recuperacion.Substring (0,ResCol[i].Recuperacion.Length - 2 )) , MaxMin[17,0]);
				MaxMin [17,1]=Math.Max (Convert.ToDouble (ResCol[i].Recuperacion.Substring (0,ResCol[i].Recuperacion.Length - 2 )) , MaxMin[17,1]);
			}
			ResCol[NumApuestas] = new ResultadosPorColumna (NumApuestas,aciertos,Premios,AcumularPremio);
			PremioTotal=Math.Round (PremioTotal,0);
			lblPremioTotal.Text = ""; //PremioTotal.ToString ();
			
			lblVecesBeneficio.Text = VecesBeneficio.ToString ();
			lblVecesPremiada.Text = ""; //VecesPremiada.ToString ();

			lblRecuperacion.Text = ""; //Recuperacion.ToString () + "%";

			dgResultadoEscrutinioDataBindPorColumnas();
			ConvertidorDeBases col =new ConvertidorDeBases();
			txColumna.Text =col.ConvNumAColumna (Convert.ToInt32 (ResCol[0].Columna));

			btGrabar.Enabled =true;
			btFiltrar.Enabled =true;
			
		}
		private void ordenaPorPremio(int izq, int der)
		{ 

			int i = 0, j = 0; 
			ResultadosPorColumna x = new ResultadosPorColumna();
			ResultadosPorColumna aux= new ResultadosPorColumna();
			i = izq; j = der; 
			x = ResCol [ (izq + der) /2 ]; 
			do
			{ 
				while(ResCol[i].ComparaCon (x,CriterioOrdenacion)>0 && j <= der)i++; 
				while(x.ComparaCon (ResCol[j],CriterioOrdenacion)>0 && j > izq )j--; 
				if( i <= j )
				{ 
					aux = ResCol[i]; 
					ResCol[i] = ResCol[j]; 
					ResCol[j] = aux; 
					i++;  j--; 
				} 
			}while( i <= j ); 
			if( izq < j ) ordenaPorPremio(izq, j); 
			if( i < der ) ordenaPorPremio(i, der); 
		}

		private void genericOrdenar_CheckedChanged(object sender, System.EventArgs e)
		{
			RadioButton MiRadioButon = (RadioButton) sender;
			CriterioOrdenacion = Convert.ToByte (MiRadioButon.Tag) ;
		}

		private void btOrdenar_Click(object sender, System.EventArgs e)
		{
			if(CriterioOrdenacion<5) ordenaPorPremio(0,NumApuestas-1 );
			dgResultadoEscrutinioDataBindPorColumnas();
		}

		private void btGrabar_Click(object sender, System.EventArgs e)
		{
            int c1=0;
			int c2=0;
			int c=0;

			bool primera=true;
			for (int i=0; i<NumApuestas ;i++)
			{
				if(dgResultadoEscrutinio.IsSelected (i))
				{
					if(primera) {c1=i;primera=false;}
					c2=i;
					c++;
				}
			}
			c1++;
			c2++;
			DialogoGrabarBancoPruebasFrm DialogoGrabar = new DialogoGrabarBancoPruebasFrm (c1,c2,c);
			DialogoGrabar.ShowDialog();
			if(!DialogoGrabar.Cancelado)
			{
				c1=DialogoGrabar.FilaInicial;
				c2=DialogoGrabar.FilaFinal+1;
				if(c2>NumApuestas -c1) c2=NumApuestas-c1;
				c=DialogoGrabar.NumMaxColumnas;
				c1--;
				c2--;
				DataGridCell selectedCell = dgResultadoEscrutinio.CurrentCell;
			
				SaveFileDialog abreFiltroDialog = new SaveFileDialog();
                abreFiltroDialog.InitialDirectory = Application.StartupPath + "/Columnas/";
				abreFiltroDialog.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
				int conta=0;
				if(abreFiltroDialog.ShowDialog () == DialogResult.OK)
				{		    	
					string archivoSalida = abreFiltroDialog.FileName;	
					ConvertidorDeBases col =new ConvertidorDeBases();
                    IArchivoColumnas comCols = new ArchivoColumnasTexto(archivoSalida);
				
					for (int i=c1; i<c2 ;i++)
					{
						if(DialogoGrabar.SoloSeleccionadas)
						{
							if(dgResultadoEscrutinio.IsSelected (i))
							{
								comCols.GuardarCols(col.ConvNumAColumna(Convert.ToInt32 (ResCol[i].Columna)));
								conta++;
							}
						}
						else
						{
							comCols.GuardarCols(col.ConvNumAColumna(Convert.ToInt32 (ResCol[i].Columna)));
							conta++;
						}
						if (conta==c) break;
					}
					comCols.Cerrar();
					statusBarPanel3.Text = "Grabadas " + conta.ToString()  + " columnas";
				}
			}
		}

		private void genericAcumula_CheckedChanged(object sender, System.EventArgs e)
		{
			CheckBox MiCheckBox = (CheckBox) sender;
			AcumularPremio [Convert.ToByte (MiCheckBox.Tag)]=MiCheckBox.Checked;
			dgResultadoEscrutinioDataBindPorColumnas();
		}

		private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			progressBar1.Value =0;
			switch(tabControl1.SelectedIndex)
			{
				case 0:btanterior.Visible =false; btsiguiente.Visible=true;break;
				case 3:btanterior.Visible =true; btsiguiente.Visible=false;break;
				default: btanterior.Visible =true; btsiguiente.Visible=true;break;
			}
		}

		private void btsiguiente_Click(object sender, System.EventArgs e)
		{
			tabControl1.SelectedIndex ++;
		}

		private void btanterior_Click(object sender, System.EventArgs e)
		{
			tabControl1.SelectedIndex --;
		}
		private void Generico_CheckedChanged(object sender, System.EventArgs e)
		{
			txLN.Enabled =false;
			txPremio.Enabled =false;
			RadioButton MiRadioButton =(RadioButton)sender;

			switch (MiRadioButton.Name )
			{
				case "rbLN" : txLN.Enabled =true; ;break;
				case "rbPremio" : txPremio.Enabled =true; break;
				default: break;
			}
		}

		private void txPremio_TextChanged(object sender, System.EventArgs e)
		{
			if (txPremio.Enabled ==true)
			{
				
				if (Pct.EsNumero (txPremio.Text))
				{
					double _Premio=Convert.ToDouble(txPremio.Text.Replace (".",","));
					double _Probabilidad=PrecioApuesta *PctDestinadoAPremiosCategoria [0]/_Premio;
					_LN=Math.Log (_Probabilidad);
					txLN.Text = _LN.ToString();
				}
			}
		}

		private void txLN_TextChanged(object sender, System.EventArgs e)
		{
			if (txLN.Enabled ==true)
			{
				if (Pct.EsNumero (txLN.Text))
				{	
					double _LN=Convert.ToDouble(txLN.Text.Replace (".",","));
					double _Premio = Math.Round (PrecioApuesta *PctDestinadoAPremiosCategoria [0]/Math.Exp(_LN),0);
					txPremio.Text = _Premio.ToString() ;
				}
			}		
		}

		private void txLNmedia_TextChanged(object sender, System.EventArgs e)
		{
			HabilitarCalcular();
		}

		private void dgResultadoEscrutinio_CurrentCellChanged(object sender, System.EventArgs e)
		{
			if(rbOpcionColumnas.Checked || rbOptionEspecial.Checked)
			{
				DataGridCell selectedCell = dgResultadoEscrutinio.CurrentCell;
				ConvertidorDeBases col =new ConvertidorDeBases();
				if (selectedCell.RowNumber != NumApuestas )
				{
					txColumna.Text =col.ConvNumAColumna (Convert.ToInt32 (ResCol[selectedCell.RowNumber].Columna));
					txNumFila.Text =(1+selectedCell.RowNumber).ToString ();
				}
				else
				{
					txColumna.Text = "";
					txNumFila.Text = "";
				}
			}
		}

		private void myDataGrid_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if (rbOpcionColumnas.Checked  || this.rbOptionEspecial.Checked )
			{
				DataGrid myGrid = (DataGrid) sender;
				System.Windows.Forms.DataGrid.HitTestInfo hti;
				hti = myGrid.HitTest(e.X, e.Y);

				switch (hti.Type) 
				{
					case System.Windows.Forms.DataGrid.HitTestType.ColumnHeader :
						CriterioOrdenacion= hti.Column;
						ordenaPorPremio(0,NumApuestas-1 );
						dgResultadoEscrutinioDataBindPorColumnas();
						break;
				}
			}
		}


		private void txColumna_TextChanged(object sender, System.EventArgs e)
		{
			string c=txColumna.Text ;
			if(c.Length >0)
			{
				if (c[0]=='1') lblNewBase11.ForeColor =Color.DarkRed; else lblNewBase11.ForeColor =Color.Silver ;
				if (c[0]=='X') lblNewBase1X.ForeColor =Color.DarkRed; else lblNewBase1X.ForeColor =Color.Silver ;
				if (c[0]=='2') lblNewBase12.ForeColor =Color.DarkRed; else lblNewBase12.ForeColor =Color.Silver ;

				if (c[1]=='1') lblNewBase21.ForeColor =Color.DarkRed; else lblNewBase21.ForeColor =Color.Silver ;
				if (c[1]=='X') lblNewBase2X.ForeColor =Color.DarkRed; else lblNewBase2X.ForeColor =Color.Silver ;
				if (c[1]=='2') lblNewBase22.ForeColor =Color.DarkRed; else lblNewBase22.ForeColor =Color.Silver ;

				if (c[2]=='1') lblNewBase31.ForeColor =Color.DarkRed; else lblNewBase31.ForeColor =Color.Silver ;
				if (c[2]=='X') lblNewBase3X.ForeColor =Color.DarkRed; else lblNewBase3X.ForeColor =Color.Silver ;
				if (c[2]=='2') lblNewBase32.ForeColor =Color.DarkRed; else lblNewBase32.ForeColor =Color.Silver ;

				if (c[3]=='1') lblNewBase41.ForeColor =Color.DarkRed; else lblNewBase41.ForeColor =Color.Silver ;
				if (c[3]=='X') lblNewBase4X.ForeColor =Color.DarkRed; else lblNewBase4X.ForeColor =Color.Silver ;
				if (c[3]=='2') lblNewBase42.ForeColor =Color.DarkRed; else lblNewBase42.ForeColor =Color.Silver ;

				if (c[4]=='1') lblNewBase51.ForeColor =Color.DarkRed; else lblNewBase51.ForeColor =Color.Silver ;
				if (c[4]=='X') lblNewBase5X.ForeColor =Color.DarkRed; else lblNewBase5X.ForeColor =Color.Silver ;
				if (c[4]=='2') lblNewBase52.ForeColor =Color.DarkRed; else lblNewBase52.ForeColor =Color.Silver ;

				if (c[5]=='1') lblNewBase61.ForeColor =Color.DarkRed; else lblNewBase61.ForeColor =Color.Silver ;
				if (c[5]=='X') lblNewBase6X.ForeColor =Color.DarkRed; else lblNewBase6X.ForeColor =Color.Silver ;
				if (c[5]=='2') lblNewBase62.ForeColor =Color.DarkRed; else lblNewBase62.ForeColor =Color.Silver ;

				if (c[6]=='1') lblNewBase71.ForeColor =Color.DarkRed; else lblNewBase71.ForeColor =Color.Silver ;
				if (c[6]=='X') lblNewBase7X.ForeColor =Color.DarkRed; else lblNewBase7X.ForeColor =Color.Silver ;
				if (c[6]=='2') lblNewBase72.ForeColor =Color.DarkRed; else lblNewBase72.ForeColor =Color.Silver ;

				if (c[7]=='1') lblNewBase81.ForeColor =Color.DarkRed; else lblNewBase81.ForeColor =Color.Silver ;
				if (c[7]=='X') lblNewBase8X.ForeColor =Color.DarkRed; else lblNewBase8X.ForeColor =Color.Silver ;
				if (c[7]=='2') lblNewBase82.ForeColor =Color.DarkRed; else lblNewBase82.ForeColor =Color.Silver ;

				if (c[8]=='1') lblNewBase91.ForeColor =Color.DarkRed; else lblNewBase91.ForeColor =Color.Silver ;
				if (c[8]=='X') lblNewBase9X.ForeColor =Color.DarkRed; else lblNewBase9X.ForeColor =Color.Silver ;
				if (c[8]=='2') lblNewBase92.ForeColor =Color.DarkRed; else lblNewBase92.ForeColor =Color.Silver ;

				if (c[9]=='1') lblNewBase101.ForeColor =Color.DarkRed; else lblNewBase101.ForeColor =Color.Silver ;
				if (c[9]=='X') lblNewBase10X.ForeColor =Color.DarkRed; else lblNewBase10X.ForeColor =Color.Silver ;
				if (c[9]=='2') lblNewBase102.ForeColor =Color.DarkRed; else lblNewBase102.ForeColor =Color.Silver ;

				if (c[10]=='1') lblNewBase111.ForeColor =Color.DarkRed; else lblNewBase111.ForeColor =Color.Silver ;
				if (c[10]=='X') lblNewBase11X.ForeColor =Color.DarkRed; else lblNewBase11X.ForeColor =Color.Silver ;
				if (c[10]=='2') lblNewBase112.ForeColor =Color.DarkRed; else lblNewBase112.ForeColor =Color.Silver ;

				if (c[11]=='1') lblNewBase121.ForeColor =Color.DarkRed; else lblNewBase121.ForeColor =Color.Silver ;
				if (c[11]=='X') lblNewBase12X.ForeColor =Color.DarkRed; else lblNewBase12X.ForeColor =Color.Silver ;
				if (c[11]=='2') lblNewBase122.ForeColor =Color.DarkRed; else lblNewBase122.ForeColor =Color.Silver ;

				if (c[12]=='1') lblNewBase131.ForeColor =Color.DarkRed; else lblNewBase131.ForeColor =Color.Silver ;
				if (c[12]=='X') lblNewBase13X.ForeColor =Color.DarkRed; else lblNewBase13X.ForeColor =Color.Silver ;
				if (c[12]=='2') lblNewBase132.ForeColor =Color.DarkRed; else lblNewBase132.ForeColor =Color.Silver ;

				if (c[13]=='1') lblNewBase141.ForeColor =Color.DarkRed; else lblNewBase141.ForeColor =Color.Silver ;
				if (c[13]=='X') lblNewBase14X.ForeColor =Color.DarkRed; else lblNewBase14X.ForeColor =Color.Silver ;
				if (c[13]=='2') lblNewBase142.ForeColor =Color.DarkRed; else lblNewBase142.ForeColor =Color.Silver ;
			}
		}

		private void btFiltrar_Click(object sender, System.EventArgs e)
		{
			DialogoSeleccionBancoPruebasFrm DialogoSeleccionar = new DialogoSeleccionBancoPruebasFrm (MaxMin );
			DialogoSeleccionar.ShowDialog();
			if(!DialogoSeleccionar.Cancelado)
			{
				int ContaSeleccionadas=0;
				for (int i=0; i<NumApuestas ;i++)
				{
					for(int concepto=0; concepto<18;concepto++)
					{
						TablaDeSeleccion TS = (TablaDeSeleccion) DialogoSeleccionar.ValoresMinimosyMaximos [concepto];
						if (TS.Checked)
						{
							switch (concepto)
							{
								case 0:
								case 1:
								case 2:
								case 3:
								case 4:
									if (ResCol[i].NumVecesDe(concepto) < TS.Minimo || ResCol[i].NumVecesDe (concepto) > TS.Maximo )
									{dgResultadoEscrutinio.UnSelect (i);concepto=17;}
									else{dgResultadoEscrutinio.Select (i);}
									break;
								case 5:
									if (ResCol[i].VecesAcumulado < TS.Minimo || ResCol[i].VecesAcumulado  > TS.Maximo )
									{dgResultadoEscrutinio.UnSelect (i);concepto=17;}
									else{dgResultadoEscrutinio.Select (i);}
									break;
								case 6:
								case 7:
								case 8:
								case 9:
								case 10:
									if (ResCol[i].PremioDe (concepto-6) < TS.Minimo || ResCol[i].PremioDe  (concepto-6) > TS.Maximo )
									{dgResultadoEscrutinio.UnSelect (i);concepto=17;}
									else{dgResultadoEscrutinio.Select (i);}
									break;
								case 11:
									if (ResCol[i].PremioAcumulado < TS.Minimo || ResCol[i].PremioAcumulado > TS.Maximo )
									{dgResultadoEscrutinio.UnSelect (i);concepto=17;}
									else{dgResultadoEscrutinio.Select (i);}
									break;

								case 12:
								case 13:
								case 14:
								case 15:
								case 16:
									if (ResCol[i].PremioUnitarioDe (concepto-12) < TS.Minimo || ResCol[i].PremioUnitarioDe  (concepto-12) > TS.Maximo )
									{dgResultadoEscrutinio.UnSelect (i);concepto=17;}
									else{dgResultadoEscrutinio.Select (i);}
									break;
								case 17:
									double Recuperacion = Convert.ToDouble (ResCol[i].Recuperacion.Substring (0,ResCol[i].Recuperacion.Length -2));
									if (Recuperacion < TS.Minimo || Recuperacion > TS.Maximo )
									{dgResultadoEscrutinio.UnSelect (i);}
									else{dgResultadoEscrutinio.Select (i);}
									break;

								default:
									break;
							}
						}
					}
					if (dgResultadoEscrutinio.IsSelected (i)) ContaSeleccionadas++;
				}
				lblContaSeleccionadas.Text =ContaSeleccionadas.ToString () + " filas seleccionadas";
			}
		}

		private void dgResultadoEscrutinio_Paint(object sender, System.Windows.Forms.PaintEventArgs e) 
		{ 
			if(DataGridVacia==false)
			{
				int row = TopRow(); 
				int yDelta = dgResultadoEscrutinio.GetCellBounds(row, 0).Height + 1; 
				int y = dgResultadoEscrutinio.GetCellBounds(row, 0).Top + 2; 
				CurrencyManager cm = (CurrencyManager) this.BindingContext[dgResultadoEscrutinio.DataSource, dgResultadoEscrutinio.DataMember]; 
			
				DataGridCell selectedCell = dgResultadoEscrutinio.CurrentCell;
				int CurrentRowNumber =selectedCell.RowNumber;
				Font f=dgResultadoEscrutinio.Font;
				int pos=12;

				while(y < dgResultadoEscrutinio.Height - yDelta && row < cm.Count) 
				{ 
					//get & draw the header text... 
					string text = string.Format("{0}", row+1); 
					if (row <1001)
					{
						pos=11;
					}
					else if (row<10001)
					{
						pos=7;
					}
					else if (row<100001)
					{
						pos=3;
					}
					else if (row<1000001)
					{
						f = new Font ("Microsoft Sans Serif",7);pos=1;
					}
					else if (row<10000001)
					{
						f = new Font ("Microsoft Sans Serif",6); pos=1;
					}

					if(CurrentRowNumber !=row)
					{
						e.Graphics.DrawString(text, f, new SolidBrush(Color.Black), pos, y); 
					}
					y += yDelta; 
					row++; 
				}
			}
		}
		public int TopRow()
		{
			DataGrid.HitTestInfo hti = dgResultadoEscrutinio.HitTest(pointInCell00);
			return hti.Row;
		}

		private void controlPorcentajesApostados_Modificado(object sender, System.EventArgs e)
		{
			if(controlPorcentajesReales.archivoPorcentajes==null && controlPorcentajesApostados.FormatoFicheroValoraciones ==43)
			{

				controlPorcentajesReales.archivoPorcentajes = controlPorcentajesApostados.archivoPorcentajes  ;
				controlPorcentajesReales.FormatoFicheroValoraciones = controlPorcentajesApostados.FormatoFicheroValoraciones ;
				controlPorcentajesReales.Refresca() ;
			}
			btCalcularReales.Enabled =true;
			HabilitarCalcular ();
		}

		private void controlPorcentajesReales_Modificado(object sender, System.EventArgs e)
		{
			if (controlPorcentajesReales.archivoPorcentajes != null)
			{
				HabilitarCalcular ();
				statusBarPanel2.Text = "Valoración real obtenida a partir de fichero externo";
				Application.DoEvents();
			}
		}

		public class MyDataGrid : DataGrid
		{
			protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
			{
				DataGrid.HitTestInfo hti = this.HitTest(new Point(e.X, e.Y));
				if(hti.Type == DataGrid.HitTestType.RowResize) 
				{
					return; //no baseclass call
				}
				base.OnMouseMove(e);
			}
		}

		private void buttonEnabledChanged(object sender, System.EventArgs e)
		{
			Button b =(Button)sender;
			FormulariosHelper f=new FormulariosHelper();
			f.CambiarFondoBoton(b);
		}

		private void rbOptionGeneric_CheckedChanged(object sender, System.EventArgs e)
		{
			HabilitarCalcular();
			if (rbOptionEspecial.Checked )
			{
				chAcumula10.Checked = true;
				chAcumula11.Checked = true;
				chAcumula12.Checked = true;
				chAcumula13.Checked = true;
				chAcumula14.Checked = true;
			}
		}

        private void btEliminarFilas_Click(object sender, EventArgs e)
        {
            byte Dif = (byte)(14-Convert.ToByte (txDiferencias.Text));
            ConvertidorDeBases con = new ConvertidorDeBases();
            int c=0;

            if (rbSoloFilaActual.Checked)
            {
                int CurrentCell = Convert.ToInt32 (txNumFila.Text);
                c = (con.ConvColumnaANumero(txColumna.Text));
                DeseleccionaColumnasPorDiferencias(c,0,Dif );
                dgResultadoEscrutinio.Select(CurrentCell);
            }
            else
            {
                progressBar1.Maximum = ResCol.Length;
                progressBar1.Value = 0;
                for (int i = 0; i < ResCol.Length; i++)
                {
                    if (dgResultadoEscrutinio.IsSelected(i))
                    {
                        DeseleccionaColumnasPorDiferencias(c, i, Dif);
                    }
                    progressBar1.Value = i;
                }
            }
        }

        private void DeseleccionaColumnasPorDiferencias(int c, int PosicionInicial, byte NumMinimoAciertos)
        {
            for (int i = PosicionInicial; i < ResCol.Length; i++)
            {
                if (dgResultadoEscrutinio.IsSelected(i))
                {
                    if(c == ResCol[i].Columna) continue;
                    if (Aciertos(c, ResCol[i].Columna) > NumMinimoAciertos)
                    {
                        dgResultadoEscrutinio.UnSelect(i);
                    }
                }
            }
        }
	}
}


