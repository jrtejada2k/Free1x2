using System;
using System.Text;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using Free1X2.Utils;
using Free1X2.Analisis;
using Free1X2.EntradaSalida;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI
{
	/// <summary>
	/// Summary description for TramificarForm.
	/// </summary>
	public class TramificarForm : System.Windows.Forms.Form
	{
	    private int numeroMaximoJornadas;
		private Button btCancelar;
		private Button btTramificar;
        private IContainer components;
		private string archivoSalida="";
		private string archivoPorcentajes="";
		private float[,] p = new float [14,3];
		private double[,] Cr = new double [14,3];
		private double[,] v = new double [14,3];
		private double LimiteInferiorAbsoluto;
		private double LimiteSuperiorAbsoluto=4782969;
		private double NumIntervalos;
		private double Incremento;
		private ArrayList Tramos;
		private ArrayList Jornadas;
		private ArrayList PrAcumulados;
		private bool escrutado;
		private bool afegirTram;
	    private int Profundidad;
	    private double ProbabilidadAcumulada;
		private int c;
		private ValidadorCadenas Valida= new ValidadorCadenas();
		private int[] pot = new int [] {1,3,9,27,81,243,729,2187,6561,19683,59049,177147,531441,1594323};
		private ApuestaProbEscrutada[] Ap14T=new ApuestaProbEscrutada[4782969] ;
		private double[] Premios = new double [5];
		private DateTime any;
		private	string[] ValorsJornada;
		private double PrecioApuesta=0.5;
		private int[,] MaxiMin = new int [5,2];
		private double[,] pctMaxiMin = new double [5,2];
		private BitArray Bits = new BitArray(4782969,false);
		private int noColumnasIniciales=4782969;
		private string ArchivoHistoricoDeValoraciones="";
		private	DialogoAnalisisMultipleDeTramosFrm DialogoAnalisiMultiple;
		private bool JornadaEncontrada;
		private short ContadorJornadas;
		private short FormatoFicheroValoraciones;
		private bool[] PremioBloqueado = new bool[5];
		private double _LN;
		private bool salida;
		private short NumAciertosABuscar=10;
        private GroupBox grDefTramo;
        private Label lblMin;
        private TextBox txValMin;
        private TextBox txValMax;
        private Label lblMax;
        private Label label1;
        private Label label2;
        private StatusBar statusBar1;
        private StatusBarPanel statusBarPanel1;
        private StatusBarPanel statusBarPanel2;
        private StatusBarPanel statusBarPanel3;
        private StatusBarPanel statusBarPanel4;
        private TextBox txIntervalo;
        private DataGrid dgResultados;
        private GroupBox groupBox1;
        private Label label31;
        private TextBox txColumna;
        private GroupBox grPremiosLAE;
        private Label label32;
        private TextBox tx14;
        private Label label33;
        private Label label34;
        private TextBox tx13;
        private Label label35;
        private Label label36;
        private TextBox tx12;
        private Label label37;
        private Label label38;
        private TextBox tx11;
        private Label label39;
        private Label label40;
        private TextBox tx10;
        private Label label41;
        private Label label42;
        private Label label43;
        private TextBox txTemporada;
        private ComboBox cmbNumTrams;
        private MainMenu mainMenu1;
        private MenuItem menuItem1;
        private MenuItem menuItem4;
        private MenuItem menuItem5;
        private MenuItem menuItem6;
        private MenuItem menuItem7;
        private MenuItem menuItem9;
        private GroupBox grMinMax;
        private Label label44;
        private TextBox txMin14;
        private TextBox txMax14;
        private TextBox txMax13;
        private TextBox txMin13;
        private Label label45;
        private Label label46;
        private TextBox txMax11;
        private TextBox txMin11;
        private Label label47;
        private TextBox txMax10;
        private TextBox txMin10;
        private Label label48;
        private TextBox txMax12;
        private TextBox txMin12;
        private GroupBox groupBox2;
        private Label label49;
        private TextBox TxColumnaEnPosicion;
        private NumericUpDown numericUpDown1;
        private Label label50;
        private TextBox txAciertos;
        private Button btAnterior;
        private Button btSiguiente;
        private Label label51;
        private Label label52;
        private Label label53;
        private TextBox txRecaudacion;
        private Button btFiltrar;
        private Button btGrabarFiltro;
        private MenuItem mnu14Triples;
        private MenuItem mnuFichero;
        private Button btBuscarColumna;
        private MenuItem menuItem2;
        private Label lblValoracion;
        private TextBox txProbabilidad;
        private CheckBox chkAcumular;
        private Label lbColumnasAGrabar;
        private MenuItem menuItem8;
        private MenuItem menuItem12;
        private MenuItem mnuJornadasMultiple;
        private MenuItem mnuJornadaSimple;
        private Label label54;
        private MenuItem mnuPorProductos;
        private MenuItem mnuPorSumas;
        private MenuItem mnuGuardarEnHistorico;
        private Button btGuardarLimites;
        private Button btLeerLimites;
        private Label label55;
        private Button btVerPorcentaje;
        private Button btUndo;
        private TextBox txTemporada2;
        private Label label56;
        private Button btTemporadaSiguiente;
        private Button btTemporadaAnterior;
        private Button btCopiar;
        private Button btPegar;
        private CheckBox chkey14;
        private CheckBox chkey13;
        private CheckBox chkey11;
        private CheckBox chkey12;
        private CheckBox chkey10;
        private TextBox txLNCentral;
        private Label lbLNCentral;
        private ComboBox cmbAciertosABuscar;
        private MenuItem mnuFicheroSobre14T;
        private MenuItem menuItem3;
        private MenuItem menuItem10;
        private Controls.ControlPorcentajes controlPorcentajes1;
        private Button btGrabarTramos;
        private Button btPrAcum;
        private DataGrid dgResultadosPrAcum;
        private NumericUpDown numJornada;

	    private class RegistroPrAcum
		{
			private string _Temporada;
			private string _Jornada;
			private double _PrAcumulada;
			private double _Premio;
			private int _NumColumnas;

			public RegistroPrAcum(string pTemporada, string pJornada, double pPrAcumulada, double pPremio, int pNumColumnas)
			{
			    _Jornada=pJornada;
				_PrAcumulada=pPrAcumulada;
				_Premio=pPremio;
				_NumColumnas=pNumColumnas;
			}
		}
		public TramificarForm()
		{
		    numeroMaximoJornadas = 70;
			InitializeComponent();
			InicializaGridResultados();
			InicializaGridResultadosPrAcum ();
			statusBar1.ShowPanels =true;
			statusBarPanel4.Text ="Faltan datos";
			any = DateTime.Now;
			int mes=any.Month;
			if (mes<7)
			{
				txTemporada.Text = Convert.ToString (any.Year -1);
			}
			else
			{
				txTemporada.Text = any.Year.ToString();
			}
			CargarUltimaJornada();
			txLNCentral.Text = _LN.ToString();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TramificarForm));
            this.btCancelar = new System.Windows.Forms.Button();
            this.btTramificar = new System.Windows.Forms.Button();
            this.grDefTramo = new System.Windows.Forms.GroupBox();
            this.cmbNumTrams = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txIntervalo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txValMax = new System.Windows.Forms.TextBox();
            this.lblMax = new System.Windows.Forms.Label();
            this.txValMin = new System.Windows.Forms.TextBox();
            this.lblMin = new System.Windows.Forms.Label();
            this.statusBar1 = new System.Windows.Forms.StatusBar();
            this.statusBarPanel1 = new System.Windows.Forms.StatusBarPanel();
            this.statusBarPanel2 = new System.Windows.Forms.StatusBarPanel();
            this.statusBarPanel3 = new System.Windows.Forms.StatusBarPanel();
            this.statusBarPanel4 = new System.Windows.Forms.StatusBarPanel();
            this.dgResultados = new System.Windows.Forms.DataGrid();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txTemporada2 = new System.Windows.Forms.TextBox();
            this.label56 = new System.Windows.Forms.Label();
            this.btTemporadaSiguiente = new System.Windows.Forms.Button();
            this.btTemporadaAnterior = new System.Windows.Forms.Button();
            this.numJornada = new System.Windows.Forms.NumericUpDown();
            this.label43 = new System.Windows.Forms.Label();
            this.txTemporada = new System.Windows.Forms.TextBox();
            this.label42 = new System.Windows.Forms.Label();
            this.grPremiosLAE = new System.Windows.Forms.GroupBox();
            this.chkey10 = new System.Windows.Forms.CheckBox();
            this.chkey11 = new System.Windows.Forms.CheckBox();
            this.chkey12 = new System.Windows.Forms.CheckBox();
            this.chkey13 = new System.Windows.Forms.CheckBox();
            this.chkey14 = new System.Windows.Forms.CheckBox();
            this.label53 = new System.Windows.Forms.Label();
            this.txRecaudacion = new System.Windows.Forms.TextBox();
            this.label40 = new System.Windows.Forms.Label();
            this.tx10 = new System.Windows.Forms.TextBox();
            this.label41 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.tx11 = new System.Windows.Forms.TextBox();
            this.label39 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.tx12 = new System.Windows.Forms.TextBox();
            this.label37 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.tx13 = new System.Windows.Forms.TextBox();
            this.label35 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.tx14 = new System.Windows.Forms.TextBox();
            this.label32 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.txColumna = new System.Windows.Forms.TextBox();
            this.btGrabarTramos = new System.Windows.Forms.Button();
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.mnu14Triples = new System.Windows.Forms.MenuItem();
            this.mnuFichero = new System.Windows.Forms.MenuItem();
            this.mnuFicheroSobre14T = new System.Windows.Forms.MenuItem();
            this.menuItem4 = new System.Windows.Forms.MenuItem();
            this.menuItem5 = new System.Windows.Forms.MenuItem();
            this.menuItem6 = new System.Windows.Forms.MenuItem();
            this.menuItem7 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.menuItem9 = new System.Windows.Forms.MenuItem();
            this.mnuPorProductos = new System.Windows.Forms.MenuItem();
            this.mnuPorSumas = new System.Windows.Forms.MenuItem();
            this.menuItem8 = new System.Windows.Forms.MenuItem();
            this.mnuGuardarEnHistorico = new System.Windows.Forms.MenuItem();
            this.menuItem12 = new System.Windows.Forms.MenuItem();
            this.mnuJornadasMultiple = new System.Windows.Forms.MenuItem();
            this.mnuJornadaSimple = new System.Windows.Forms.MenuItem();
            this.menuItem3 = new System.Windows.Forms.MenuItem();
            this.menuItem10 = new System.Windows.Forms.MenuItem();
            this.grMinMax = new System.Windows.Forms.GroupBox();
            this.btPegar = new System.Windows.Forms.Button();
            this.btCopiar = new System.Windows.Forms.Button();
            this.btUndo = new System.Windows.Forms.Button();
            this.btVerPorcentaje = new System.Windows.Forms.Button();
            this.label55 = new System.Windows.Forms.Label();
            this.btGuardarLimites = new System.Windows.Forms.Button();
            this.btLeerLimites = new System.Windows.Forms.Button();
            this.label54 = new System.Windows.Forms.Label();
            this.lbColumnasAGrabar = new System.Windows.Forms.Label();
            this.btGrabarFiltro = new System.Windows.Forms.Button();
            this.btFiltrar = new System.Windows.Forms.Button();
            this.label52 = new System.Windows.Forms.Label();
            this.label51 = new System.Windows.Forms.Label();
            this.txMax10 = new System.Windows.Forms.TextBox();
            this.txMin10 = new System.Windows.Forms.TextBox();
            this.label48 = new System.Windows.Forms.Label();
            this.txMax11 = new System.Windows.Forms.TextBox();
            this.txMin11 = new System.Windows.Forms.TextBox();
            this.label47 = new System.Windows.Forms.Label();
            this.txMax12 = new System.Windows.Forms.TextBox();
            this.txMin12 = new System.Windows.Forms.TextBox();
            this.label46 = new System.Windows.Forms.Label();
            this.txMax13 = new System.Windows.Forms.TextBox();
            this.txMin13 = new System.Windows.Forms.TextBox();
            this.label45 = new System.Windows.Forms.Label();
            this.txMax14 = new System.Windows.Forms.TextBox();
            this.txMin14 = new System.Windows.Forms.TextBox();
            this.label44 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmbAciertosABuscar = new System.Windows.Forms.ComboBox();
            this.txProbabilidad = new System.Windows.Forms.TextBox();
            this.lblValoracion = new System.Windows.Forms.Label();
            this.btBuscarColumna = new System.Windows.Forms.Button();
            this.btSiguiente = new System.Windows.Forms.Button();
            this.btAnterior = new System.Windows.Forms.Button();
            this.txAciertos = new System.Windows.Forms.TextBox();
            this.label50 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.TxColumnaEnPosicion = new System.Windows.Forms.TextBox();
            this.label49 = new System.Windows.Forms.Label();
            this.chkAcumular = new System.Windows.Forms.CheckBox();
            this.txLNCentral = new System.Windows.Forms.TextBox();
            this.lbLNCentral = new System.Windows.Forms.Label();
            this.controlPorcentajes1 = new Free1X2.UI.Controls.ControlPorcentajes();
            this.btPrAcum = new System.Windows.Forms.Button();
            this.dgResultadosPrAcum = new System.Windows.Forms.DataGrid();
            this.grDefTramo.SuspendLayout();
            // StatusBarPanel controls don't support ISupportInitialize in .NET 8
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).BeginInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel2)).BeginInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel3)).BeginInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgResultados)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numJornada)).BeginInit();
            this.grPremiosLAE.SuspendLayout();
            this.grMinMax.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgResultadosPrAcum)).BeginInit();
            this.SuspendLayout();
            // 
            // btCancelar
            // 
            this.btCancelar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btCancelar.BackColor = System.Drawing.Color.LightSalmon;
            this.btCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btCancelar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btCancelar.Image = ((System.Drawing.Image)(resources.GetObject("btCancelar.Image")));
            this.btCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btCancelar.Location = new System.Drawing.Point(700, 492);
            this.btCancelar.Name = "btCancelar";
            this.btCancelar.Size = new System.Drawing.Size(84, 23);
            this.btCancelar.TabIndex = 262;
            this.btCancelar.Text = "&Cancelar";
            this.btCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btCancelar.UseVisualStyleBackColor = false;
            this.btCancelar.Click += new System.EventHandler(this.btCancelar_Click);
            // 
            // btTramificar
            // 
            this.btTramificar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btTramificar.BackColor = System.Drawing.Color.Silver;
            this.btTramificar.Enabled = false;
            this.btTramificar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btTramificar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btTramificar.Image = ((System.Drawing.Image)(resources.GetObject("btTramificar.Image")));
            this.btTramificar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btTramificar.Location = new System.Drawing.Point(609, 492);
            this.btTramificar.Name = "btTramificar";
            this.btTramificar.Size = new System.Drawing.Size(90, 23);
            this.btTramificar.TabIndex = 263;
            this.btTramificar.Text = "&Tramificar";
            this.btTramificar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btTramificar.UseVisualStyleBackColor = false;
            this.btTramificar.Click += new System.EventHandler(this.btTramificar_Click);
            this.btTramificar.EnabledChanged += new System.EventHandler(this.btTramificar_EnabledChanged);
            // 
            // grDefTramo
            // 
            this.grDefTramo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.grDefTramo.BackColor = System.Drawing.Color.Bisque;
            this.grDefTramo.Controls.Add(this.cmbNumTrams);
            this.grDefTramo.Controls.Add(this.label2);
            this.grDefTramo.Controls.Add(this.txIntervalo);
            this.grDefTramo.Controls.Add(this.label1);
            this.grDefTramo.Controls.Add(this.txValMax);
            this.grDefTramo.Controls.Add(this.lblMax);
            this.grDefTramo.Controls.Add(this.txValMin);
            this.grDefTramo.Controls.Add(this.lblMin);
            this.grDefTramo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grDefTramo.ForeColor = System.Drawing.Color.Maroon;
            this.grDefTramo.Location = new System.Drawing.Point(452, 340);
            this.grDefTramo.Name = "grDefTramo";
            this.grDefTramo.Size = new System.Drawing.Size(152, 120);
            this.grDefTramo.TabIndex = 265;
            this.grDefTramo.TabStop = false;
            this.grDefTramo.Text = "Definición tramo";
            // 
            // cmbNumTrams
            // 
            this.cmbNumTrams.DisplayMember = "1";
            this.cmbNumTrams.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cmbNumTrams.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbNumTrams.ForeColor = System.Drawing.Color.Black;
            this.cmbNumTrams.Items.AddRange(new object[] {
            "9",
            "27",
            "81",
            "243",
            "729",
            "2187",
            "6561",
            "19683",
            "59049",
            "177147",
            "531441"});
            this.cmbNumTrams.Location = new System.Drawing.Point(84, 92);
            this.cmbNumTrams.Name = "cmbNumTrams";
            this.cmbNumTrams.Size = new System.Drawing.Size(60, 21);
            this.cmbNumTrams.TabIndex = 8;
            this.cmbNumTrams.Text = "9";
            this.cmbNumTrams.SelectedIndexChanged += new System.EventHandler(this.cmbNumTrams_SelectedIndexChanged);
            this.cmbNumTrams.SelectedValueChanged += new System.EventHandler(this.cmbNumTrams_SelectedIndexChanged);
            this.cmbNumTrams.TextChanged += new System.EventHandler(this.cmbNumTrams_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label2.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(8, 92);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 20);
            this.label2.TabIndex = 6;
            this.label2.Text = "Nº de tramos";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txIntervalo
            // 
            this.txIntervalo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txIntervalo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txIntervalo.ForeColor = System.Drawing.Color.Black;
            this.txIntervalo.Location = new System.Drawing.Point(92, 71);
            this.txIntervalo.Name = "txIntervalo";
            this.txIntervalo.Size = new System.Drawing.Size(52, 20);
            this.txIntervalo.TabIndex = 5;
            this.txIntervalo.Text = "531441";
            this.txIntervalo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenericoSinDecimales_KeyPress);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label1.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(8, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Cols./ tramo";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txValMax
            // 
            this.txValMax.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txValMax.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txValMax.ForeColor = System.Drawing.Color.Black;
            this.txValMax.Location = new System.Drawing.Point(92, 50);
            this.txValMax.Name = "txValMax";
            this.txValMax.Size = new System.Drawing.Size(52, 20);
            this.txValMax.TabIndex = 3;
            this.txValMax.Text = "4782969";
            this.txValMax.TextChanged += new System.EventHandler(this.txValMax_TextChanged);
            this.txValMax.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenericoSinDecimales_KeyPress);
            // 
            // lblMax
            // 
            this.lblMax.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblMax.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMax.ForeColor = System.Drawing.Color.Black;
            this.lblMax.Location = new System.Drawing.Point(8, 50);
            this.lblMax.Name = "lblMax";
            this.lblMax.Size = new System.Drawing.Size(84, 20);
            this.lblMax.TabIndex = 2;
            this.lblMax.Text = "Columna final";
            this.lblMax.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txValMin
            // 
            this.txValMin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txValMin.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txValMin.ForeColor = System.Drawing.Color.Black;
            this.txValMin.Location = new System.Drawing.Point(92, 29);
            this.txValMin.Name = "txValMin";
            this.txValMin.Size = new System.Drawing.Size(52, 20);
            this.txValMin.TabIndex = 1;
            this.txValMin.Text = "0";
            this.txValMin.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenericoSinDecimales_KeyPress);
            // 
            // lblMin
            // 
            this.lblMin.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblMin.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMin.ForeColor = System.Drawing.Color.Black;
            this.lblMin.Location = new System.Drawing.Point(8, 29);
            this.lblMin.Name = "lblMin";
            this.lblMin.Size = new System.Drawing.Size(84, 20);
            this.lblMin.TabIndex = 0;
            this.lblMin.Text = "Columna inicial";
            this.lblMin.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // statusBar1
            // 
            this.statusBar1.Location = new System.Drawing.Point(0, 544);
            this.statusBar1.Name = "statusBar1";
            this.statusBar1.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.statusBarPanel1,
            this.statusBarPanel2,
            this.statusBarPanel3,
            this.statusBarPanel4});
            this.statusBar1.Size = new System.Drawing.Size(792, 22);
            this.statusBar1.TabIndex = 267;
            // 
            // statusBarPanel1
            // 
            this.statusBarPanel1.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.statusBarPanel1.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.None;
            this.statusBarPanel1.Name = "statusBarPanel1";
            this.statusBarPanel1.Text = "Fich. %";
            this.statusBarPanel1.Width = 52;
            // 
            // statusBarPanel2
            // 
            this.statusBarPanel2.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            this.statusBarPanel2.Name = "statusBarPanel2";
            // 
            // statusBarPanel3
            // 
            this.statusBarPanel3.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.statusBarPanel3.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.None;
            this.statusBarPanel3.Name = "statusBarPanel3";
            this.statusBarPanel3.Width = 10;
            // 
            // statusBarPanel4
            // 
            this.statusBarPanel4.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.statusBarPanel4.Name = "statusBarPanel4";
            this.statusBarPanel4.Width = 10;
            // 
            // dgResultados
            // 
            this.dgResultados.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgResultados.CaptionVisible = false;
            this.dgResultados.DataMember = "";
            this.dgResultados.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgResultados.Location = new System.Drawing.Point(0, 0);
            this.dgResultados.Name = "dgResultados";
            this.dgResultados.ReadOnly = true;
            this.dgResultados.Size = new System.Drawing.Size(448, 407);
            this.dgResultados.TabIndex = 270;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.BackColor = System.Drawing.Color.Bisque;
            this.groupBox1.Controls.Add(this.txTemporada2);
            this.groupBox1.Controls.Add(this.label56);
            this.groupBox1.Controls.Add(this.btTemporadaSiguiente);
            this.groupBox1.Controls.Add(this.btTemporadaAnterior);
            this.groupBox1.Controls.Add(this.numJornada);
            this.groupBox1.Controls.Add(this.label43);
            this.groupBox1.Controls.Add(this.txTemporada);
            this.groupBox1.Controls.Add(this.label42);
            this.groupBox1.Controls.Add(this.grPremiosLAE);
            this.groupBox1.Controls.Add(this.label31);
            this.groupBox1.Controls.Add(this.txColumna);
            this.groupBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox1.Location = new System.Drawing.Point(608, 28);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(176, 308);
            this.groupBox1.TabIndex = 271;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Datos L.A.E.";
            // 
            // txTemporada2
            // 
            this.txTemporada2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.txTemporada2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txTemporada2.Enabled = false;
            this.txTemporada2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txTemporada2.ForeColor = System.Drawing.Color.Black;
            this.txTemporada2.Location = new System.Drawing.Point(92, 40);
            this.txTemporada2.Name = "txTemporada2";
            this.txTemporada2.Size = new System.Drawing.Size(52, 21);
            this.txTemporada2.TabIndex = 282;
            this.txTemporada2.Text = "2005";
            // 
            // label56
            // 
            this.label56.BackColor = System.Drawing.Color.Transparent;
            this.label56.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label56.ForeColor = System.Drawing.Color.Black;
            this.label56.Location = new System.Drawing.Point(80, 40);
            this.label56.Name = "label56";
            this.label56.Size = new System.Drawing.Size(12, 21);
            this.label56.TabIndex = 281;
            this.label56.Text = "/";
            this.label56.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btTemporadaSiguiente
            // 
            this.btTemporadaSiguiente.BackColor = System.Drawing.Color.LightSalmon;
            this.btTemporadaSiguiente.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btTemporadaSiguiente.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btTemporadaSiguiente.ForeColor = System.Drawing.Color.Black;
            this.btTemporadaSiguiente.Image = ((System.Drawing.Image)(resources.GetObject("btTemporadaSiguiente.Image")));
            this.btTemporadaSiguiente.Location = new System.Drawing.Point(145, 40);
            this.btTemporadaSiguiente.Name = "btTemporadaSiguiente";
            this.btTemporadaSiguiente.Size = new System.Drawing.Size(20, 21);
            this.btTemporadaSiguiente.TabIndex = 280;
            this.btTemporadaSiguiente.UseVisualStyleBackColor = false;
            this.btTemporadaSiguiente.Click += new System.EventHandler(this.btTemporadaSiguiente_Click);
            // 
            // btTemporadaAnterior
            // 
            this.btTemporadaAnterior.BackColor = System.Drawing.Color.LightSalmon;
            this.btTemporadaAnterior.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btTemporadaAnterior.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btTemporadaAnterior.ForeColor = System.Drawing.Color.Black;
            this.btTemporadaAnterior.Image = ((System.Drawing.Image)(resources.GetObject("btTemporadaAnterior.Image")));
            this.btTemporadaAnterior.Location = new System.Drawing.Point(7, 40);
            this.btTemporadaAnterior.Name = "btTemporadaAnterior";
            this.btTemporadaAnterior.Size = new System.Drawing.Size(20, 21);
            this.btTemporadaAnterior.TabIndex = 279;
            this.btTemporadaAnterior.UseVisualStyleBackColor = false;
            this.btTemporadaAnterior.Click += new System.EventHandler(this.btTemporadaAnterior_Click);
            // 
            // numJornada
            // 
            this.numJornada.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numJornada.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numJornada.ForeColor = System.Drawing.Color.Black;
            this.numJornada.Location = new System.Drawing.Point(117, 88);
            this.numJornada.Maximum = new decimal(new int[] {
            70,
            0,
            0,
            0});
            this.numJornada.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numJornada.Name = "numJornada";
            this.numJornada.Size = new System.Drawing.Size(52, 21);
            this.numJornada.TabIndex = 278;
            this.numJornada.Value = new decimal(new int[] {
            7,
            0,
            0,
            0});
            this.numJornada.TextChanged += new System.EventHandler(this.TemporadaJornada_TextChanged);
            this.numJornada.ValueChanged += new System.EventHandler(this.TemporadaJornada_TextChanged);
            // 
            // label43
            // 
            this.label43.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label43.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label43.ForeColor = System.Drawing.Color.Black;
            this.label43.Location = new System.Drawing.Point(8, 20);
            this.label43.Name = "label43";
            this.label43.Size = new System.Drawing.Size(156, 20);
            this.label43.TabIndex = 277;
            this.label43.Text = "Temporada";
            this.label43.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txTemporada
            // 
            this.txTemporada.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txTemporada.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txTemporada.ForeColor = System.Drawing.Color.Black;
            this.txTemporada.Location = new System.Drawing.Point(28, 40);
            this.txTemporada.MaxLength = 14;
            this.txTemporada.Name = "txTemporada";
            this.txTemporada.Size = new System.Drawing.Size(52, 21);
            this.txTemporada.TabIndex = 276;
            this.txTemporada.Text = "2004";
            this.txTemporada.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txTemporada.TextChanged += new System.EventHandler(this.TemporadaJornada_TextChanged);
            // 
            // label42
            // 
            this.label42.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label42.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label42.ForeColor = System.Drawing.Color.Black;
            this.label42.Location = new System.Drawing.Point(116, 68);
            this.label42.Name = "label42";
            this.label42.Size = new System.Drawing.Size(52, 20);
            this.label42.TabIndex = 274;
            this.label42.Text = "Jornada";
            this.label42.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // grPremiosLAE
            // 
            this.grPremiosLAE.BackColor = System.Drawing.Color.Bisque;
            this.grPremiosLAE.Controls.Add(this.chkey10);
            this.grPremiosLAE.Controls.Add(this.chkey11);
            this.grPremiosLAE.Controls.Add(this.chkey12);
            this.grPremiosLAE.Controls.Add(this.chkey13);
            this.grPremiosLAE.Controls.Add(this.chkey14);
            this.grPremiosLAE.Controls.Add(this.label53);
            this.grPremiosLAE.Controls.Add(this.txRecaudacion);
            this.grPremiosLAE.Controls.Add(this.label40);
            this.grPremiosLAE.Controls.Add(this.tx10);
            this.grPremiosLAE.Controls.Add(this.label41);
            this.grPremiosLAE.Controls.Add(this.label38);
            this.grPremiosLAE.Controls.Add(this.tx11);
            this.grPremiosLAE.Controls.Add(this.label39);
            this.grPremiosLAE.Controls.Add(this.label36);
            this.grPremiosLAE.Controls.Add(this.tx12);
            this.grPremiosLAE.Controls.Add(this.label37);
            this.grPremiosLAE.Controls.Add(this.label34);
            this.grPremiosLAE.Controls.Add(this.tx13);
            this.grPremiosLAE.Controls.Add(this.label35);
            this.grPremiosLAE.Controls.Add(this.label33);
            this.grPremiosLAE.Controls.Add(this.tx14);
            this.grPremiosLAE.Controls.Add(this.label32);
            this.grPremiosLAE.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grPremiosLAE.ForeColor = System.Drawing.Color.Maroon;
            this.grPremiosLAE.Location = new System.Drawing.Point(8, 116);
            this.grPremiosLAE.Name = "grPremiosLAE";
            this.grPremiosLAE.Size = new System.Drawing.Size(160, 180);
            this.grPremiosLAE.TabIndex = 272;
            this.grPremiosLAE.TabStop = false;
            this.grPremiosLAE.Text = "Premios";
            // 
            // chkey10
            // 
            this.chkey10.ForeColor = System.Drawing.Color.Black;
            this.chkey10.Image = ((System.Drawing.Image)(resources.GetObject("chkey10.Image")));
            this.chkey10.Location = new System.Drawing.Point(112, 104);
            this.chkey10.Name = "chkey10";
            this.chkey10.Size = new System.Drawing.Size(36, 20);
            this.chkey10.TabIndex = 278;
            this.chkey10.Tag = "4";
            this.chkey10.CheckedChanged += new System.EventHandler(this.genericochkey_CheckStateChanged);
            // 
            // chkey11
            // 
            this.chkey11.ForeColor = System.Drawing.Color.Black;
            this.chkey11.Image = ((System.Drawing.Image)(resources.GetObject("chkey11.Image")));
            this.chkey11.Location = new System.Drawing.Point(112, 83);
            this.chkey11.Name = "chkey11";
            this.chkey11.Size = new System.Drawing.Size(36, 20);
            this.chkey11.TabIndex = 277;
            this.chkey11.Tag = "3";
            this.chkey11.CheckedChanged += new System.EventHandler(this.genericochkey_CheckStateChanged);
            // 
            // chkey12
            // 
            this.chkey12.ForeColor = System.Drawing.Color.Black;
            this.chkey12.Image = ((System.Drawing.Image)(resources.GetObject("chkey12.Image")));
            this.chkey12.Location = new System.Drawing.Point(112, 62);
            this.chkey12.Name = "chkey12";
            this.chkey12.Size = new System.Drawing.Size(36, 20);
            this.chkey12.TabIndex = 276;
            this.chkey12.Tag = "2";
            this.chkey12.CheckedChanged += new System.EventHandler(this.genericochkey_CheckStateChanged);
            // 
            // chkey13
            // 
            this.chkey13.ForeColor = System.Drawing.Color.Black;
            this.chkey13.Image = ((System.Drawing.Image)(resources.GetObject("chkey13.Image")));
            this.chkey13.Location = new System.Drawing.Point(112, 41);
            this.chkey13.Name = "chkey13";
            this.chkey13.Size = new System.Drawing.Size(36, 20);
            this.chkey13.TabIndex = 275;
            this.chkey13.Tag = "1";
            this.chkey13.CheckedChanged += new System.EventHandler(this.genericochkey_CheckStateChanged);
            // 
            // chkey14
            // 
            this.chkey14.ForeColor = System.Drawing.Color.Black;
            this.chkey14.Image = ((System.Drawing.Image)(resources.GetObject("chkey14.Image")));
            this.chkey14.Location = new System.Drawing.Point(112, 20);
            this.chkey14.Name = "chkey14";
            this.chkey14.Size = new System.Drawing.Size(36, 20);
            this.chkey14.TabIndex = 274;
            this.chkey14.Tag = "0";
            this.chkey14.CheckedChanged += new System.EventHandler(this.genericochkey_CheckStateChanged);
            // 
            // label53
            // 
            this.label53.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label53.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label53.ForeColor = System.Drawing.Color.Black;
            this.label53.Location = new System.Drawing.Point(28, 132);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(104, 20);
            this.label53.TabIndex = 273;
            this.label53.Text = "Recaudación";
            this.label53.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txRecaudacion
            // 
            this.txRecaudacion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txRecaudacion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txRecaudacion.ForeColor = System.Drawing.Color.Black;
            this.txRecaudacion.Location = new System.Drawing.Point(28, 154);
            this.txRecaudacion.MaxLength = 14;
            this.txRecaudacion.Name = "txRecaudacion";
            this.txRecaudacion.Size = new System.Drawing.Size(104, 20);
            this.txRecaudacion.TabIndex = 272;
            this.txRecaudacion.Text = "7768049";
            this.txRecaudacion.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label40
            // 
            this.label40.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label40.ForeColor = System.Drawing.Color.Black;
            this.label40.Location = new System.Drawing.Point(100, 108);
            this.label40.Name = "label40";
            this.label40.Size = new System.Drawing.Size(20, 12);
            this.label40.TabIndex = 14;
            this.label40.Text = "";
            // 
            // tx10
            // 
            this.tx10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tx10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tx10.ForeColor = System.Drawing.Color.Black;
            this.tx10.Location = new System.Drawing.Point(28, 104);
            this.tx10.Name = "tx10";
            this.tx10.Size = new System.Drawing.Size(72, 20);
            this.tx10.TabIndex = 13;
            this.tx10.Text = "22,79";
            this.tx10.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tx10.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            // 
            // label41
            // 
            this.label41.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label41.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label41.ForeColor = System.Drawing.Color.Black;
            this.label41.Location = new System.Drawing.Point(7, 104);
            this.label41.Name = "label41";
            this.label41.Size = new System.Drawing.Size(20, 20);
            this.label41.TabIndex = 12;
            this.label41.Text = "10";
            this.label41.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label38
            // 
            this.label38.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label38.ForeColor = System.Drawing.Color.Black;
            this.label38.Location = new System.Drawing.Point(100, 87);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(20, 12);
            this.label38.TabIndex = 11;
            this.label38.Text = "";
            // 
            // tx11
            // 
            this.tx11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tx11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tx11.ForeColor = System.Drawing.Color.Black;
            this.tx11.Location = new System.Drawing.Point(28, 83);
            this.tx11.Name = "tx11";
            this.tx11.Size = new System.Drawing.Size(72, 20);
            this.tx11.TabIndex = 10;
            this.tx11.Text = "165,38";
            this.tx11.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tx11.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            // 
            // label39
            // 
            this.label39.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label39.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label39.ForeColor = System.Drawing.Color.Black;
            this.label39.Location = new System.Drawing.Point(7, 83);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(20, 20);
            this.label39.TabIndex = 9;
            this.label39.Text = "11";
            this.label39.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label36
            // 
            this.label36.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label36.ForeColor = System.Drawing.Color.Black;
            this.label36.Location = new System.Drawing.Point(100, 66);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(20, 12);
            this.label36.TabIndex = 8;
            this.label36.Text = "";
            // 
            // tx12
            // 
            this.tx12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tx12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tx12.ForeColor = System.Drawing.Color.Black;
            this.tx12.Location = new System.Drawing.Point(28, 62);
            this.tx12.Name = "tx12";
            this.tx12.Size = new System.Drawing.Size(72, 20);
            this.tx12.TabIndex = 7;
            this.tx12.Text = "1753,51";
            this.tx12.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tx12.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            // 
            // label37
            // 
            this.label37.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label37.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label37.ForeColor = System.Drawing.Color.Black;
            this.label37.Location = new System.Drawing.Point(7, 62);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(20, 20);
            this.label37.TabIndex = 6;
            this.label37.Text = "12";
            this.label37.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label34
            // 
            this.label34.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label34.ForeColor = System.Drawing.Color.Black;
            this.label34.Location = new System.Drawing.Point(100, 45);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(20, 12);
            this.label34.TabIndex = 5;
            this.label34.Text = "";
            // 
            // tx13
            // 
            this.tx13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tx13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tx13.ForeColor = System.Drawing.Color.Black;
            this.tx13.Location = new System.Drawing.Point(28, 41);
            this.tx13.Name = "tx13";
            this.tx13.Size = new System.Drawing.Size(72, 20);
            this.tx13.TabIndex = 4;
            this.tx13.Text = "29877,11";
            this.tx13.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tx13.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            // 
            // label35
            // 
            this.label35.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label35.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label35.ForeColor = System.Drawing.Color.Black;
            this.label35.Location = new System.Drawing.Point(7, 41);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(20, 20);
            this.label35.TabIndex = 3;
            this.label35.Text = "13";
            this.label35.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label33
            // 
            this.label33.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label33.ForeColor = System.Drawing.Color.Black;
            this.label33.Location = new System.Drawing.Point(100, 24);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(20, 12);
            this.label33.TabIndex = 2;
            this.label33.Text = "";
            // 
            // tx14
            // 
            this.tx14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tx14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tx14.ForeColor = System.Drawing.Color.Black;
            this.tx14.Location = new System.Drawing.Point(28, 20);
            this.tx14.Name = "tx14";
            this.tx14.Size = new System.Drawing.Size(72, 20);
            this.tx14.TabIndex = 1;
            this.tx14.Text = "1165207,35";
            this.tx14.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.tx14.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            // 
            // label32
            // 
            this.label32.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label32.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label32.ForeColor = System.Drawing.Color.Black;
            this.label32.Location = new System.Drawing.Point(7, 20);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(20, 20);
            this.label32.TabIndex = 0;
            this.label32.Text = "14";
            this.label32.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label31
            // 
            this.label31.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label31.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label31.ForeColor = System.Drawing.Color.Black;
            this.label31.Location = new System.Drawing.Point(8, 68);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(104, 20);
            this.label31.TabIndex = 271;
            this.label31.Text = "Columna premiada";
            this.label31.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txColumna
            // 
            this.txColumna.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txColumna.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txColumna.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txColumna.ForeColor = System.Drawing.Color.Black;
            this.txColumna.Location = new System.Drawing.Point(8, 88);
            this.txColumna.MaxLength = 14;
            this.txColumna.Name = "txColumna";
            this.txColumna.Size = new System.Drawing.Size(104, 21);
            this.txColumna.TabIndex = 270;
            this.txColumna.Text = "1X1222X2X12121";
            this.txColumna.TextChanged += new System.EventHandler(this.txColumna_TextChanged_1);
            this.txColumna.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txColumna_KeyPress);
            // 
            // btGrabarTramos
            // 
            this.btGrabarTramos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btGrabarTramos.BackColor = System.Drawing.Color.Silver;
            this.btGrabarTramos.Enabled = false;
            this.btGrabarTramos.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btGrabarTramos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btGrabarTramos.Image = ((System.Drawing.Image)(resources.GetObject("btGrabarTramos.Image")));
            this.btGrabarTramos.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btGrabarTramos.Location = new System.Drawing.Point(536, 492);
            this.btGrabarTramos.Name = "btGrabarTramos";
            this.btGrabarTramos.Size = new System.Drawing.Size(72, 23);
            this.btGrabarTramos.TabIndex = 272;
            this.btGrabarTramos.Text = "&Grabar";
            this.btGrabarTramos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btGrabarTramos.UseVisualStyleBackColor = false;
            this.btGrabarTramos.Click += new System.EventHandler(this.button1_Click);
            this.btGrabarTramos.EnabledChanged += new System.EventHandler(this.btGrabarTramos_EnabledChanged);
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.menuItem6,
            this.menuItem9,
            this.menuItem12,
            this.menuItem3});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnu14Triples,
            this.mnuFichero,
            this.mnuFicheroSobre14T,
            this.menuItem4,
            this.menuItem5});
            this.menuItem1.Text = "Columnas";
            // 
            // mnu14Triples
            // 
            this.mnu14Triples.Checked = true;
            this.mnu14Triples.DefaultItem = true;
            this.mnu14Triples.Index = 0;
            this.mnu14Triples.RadioCheck = true;
            this.mnu14Triples.Text = "14 Triples";
            this.mnu14Triples.Click += new System.EventHandler(this.mnu14Triples_Click);
            // 
            // mnuFichero
            // 
            this.mnuFichero.Index = 1;
            this.mnuFichero.RadioCheck = true;
            this.mnuFichero.Text = "Fichero";
            this.mnuFichero.Click += new System.EventHandler(this.menuItem8_Click);
            // 
            // mnuFicheroSobre14T
            // 
            this.mnuFicheroSobre14T.Index = 2;
            this.mnuFicheroSobre14T.Text = "Fichero sobre 14 triples";
            this.mnuFicheroSobre14T.Click += new System.EventHandler(this.mnuFicheroSobre14T_Click);
            // 
            // menuItem4
            // 
            this.menuItem4.Index = 3;
            this.menuItem4.Text = "-";
            // 
            // menuItem5
            // 
            this.menuItem5.Index = 4;
            this.menuItem5.Text = "Salir (Alt F4)";
            this.menuItem5.Click += new System.EventHandler(this.menuItem5_Click);
            // 
            // menuItem6
            // 
            this.menuItem6.Index = 1;
            this.menuItem6.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem7,
            this.menuItem2});
            this.menuItem6.Text = "L.A.E.";
            // 
            // menuItem7
            // 
            this.menuItem7.DefaultItem = true;
            this.menuItem7.Index = 0;
            this.menuItem7.Text = "Guardar Jornada";
            this.menuItem7.Click += new System.EventHandler(this.menuItem7_Click);
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 1;
            this.menuItem2.Text = "Enlace con la página de escrutinio oficial del L.A.E.";
            this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
            // 
            // menuItem9
            // 
            this.menuItem9.Index = 2;
            this.menuItem9.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuPorProductos,
            this.mnuPorSumas,
            this.menuItem8,
            this.mnuGuardarEnHistorico});
            this.menuItem9.Text = "Valoraciones";
            // 
            // mnuPorProductos
            // 
            this.mnuPorProductos.Checked = true;
            this.mnuPorProductos.Index = 0;
            this.mnuPorProductos.Text = "Por Productos";
            this.mnuPorProductos.Click += new System.EventHandler(this.mnuPorProductos_Click_1);
            // 
            // mnuPorSumas
            // 
            this.mnuPorSumas.Index = 1;
            this.mnuPorSumas.Text = "Por Sumas";
            this.mnuPorSumas.Click += new System.EventHandler(this.mnuPorSumas_Click_1);
            // 
            // menuItem8
            // 
            this.menuItem8.Index = 2;
            this.menuItem8.Text = "-";
            // 
            // mnuGuardarEnHistorico
            // 
            this.mnuGuardarEnHistorico.Index = 3;
            this.mnuGuardarEnHistorico.Text = "Guardar en fichero de valoraciones históricas";
            this.mnuGuardarEnHistorico.Click += new System.EventHandler(this.mnuGuardarEnHistorico_Click);
            // 
            // menuItem12
            // 
            this.menuItem12.Index = 3;
            this.menuItem12.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuJornadasMultiple,
            this.mnuJornadaSimple});
            this.menuItem12.Text = "Análisis";
            // 
            // mnuJornadasMultiple
            // 
            this.mnuJornadasMultiple.Index = 0;
            this.mnuJornadasMultiple.RadioCheck = true;
            this.mnuJornadasMultiple.Text = "Jornadas múltiples";
            this.mnuJornadasMultiple.Click += new System.EventHandler(this.menuItem13_Click);
            // 
            // mnuJornadaSimple
            // 
            this.mnuJornadaSimple.Checked = true;
            this.mnuJornadaSimple.Index = 1;
            this.mnuJornadaSimple.RadioCheck = true;
            this.mnuJornadaSimple.Text = "Jornada simple";
            this.mnuJornadaSimple.Click += new System.EventHandler(this.mnuJornadaSimple_Click);
            // 
            // menuItem3
            // 
            this.menuItem3.Index = 4;
            this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem10});
            this.menuItem3.Text = "Gráficas";
            // 
            // menuItem10
            // 
            this.menuItem10.Index = 0;
            this.menuItem10.Text = "Gráficas";
            this.menuItem10.Click += new System.EventHandler(this.menuItem10_Click_1);
            // 
            // grMinMax
            // 
            this.grMinMax.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.grMinMax.BackColor = System.Drawing.Color.Bisque;
            this.grMinMax.Controls.Add(this.btPegar);
            this.grMinMax.Controls.Add(this.btCopiar);
            this.grMinMax.Controls.Add(this.btUndo);
            this.grMinMax.Controls.Add(this.btVerPorcentaje);
            this.grMinMax.Controls.Add(this.label55);
            this.grMinMax.Controls.Add(this.btGuardarLimites);
            this.grMinMax.Controls.Add(this.btLeerLimites);
            this.grMinMax.Controls.Add(this.label54);
            this.grMinMax.Controls.Add(this.lbColumnasAGrabar);
            this.grMinMax.Controls.Add(this.btGrabarFiltro);
            this.grMinMax.Controls.Add(this.btFiltrar);
            this.grMinMax.Controls.Add(this.label52);
            this.grMinMax.Controls.Add(this.label51);
            this.grMinMax.Controls.Add(this.txMax10);
            this.grMinMax.Controls.Add(this.txMin10);
            this.grMinMax.Controls.Add(this.label48);
            this.grMinMax.Controls.Add(this.txMax11);
            this.grMinMax.Controls.Add(this.txMin11);
            this.grMinMax.Controls.Add(this.label47);
            this.grMinMax.Controls.Add(this.txMax12);
            this.grMinMax.Controls.Add(this.txMin12);
            this.grMinMax.Controls.Add(this.label46);
            this.grMinMax.Controls.Add(this.txMax13);
            this.grMinMax.Controls.Add(this.txMin13);
            this.grMinMax.Controls.Add(this.label45);
            this.grMinMax.Controls.Add(this.txMax14);
            this.grMinMax.Controls.Add(this.txMin14);
            this.grMinMax.Controls.Add(this.label44);
            this.grMinMax.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grMinMax.ForeColor = System.Drawing.Color.Maroon;
            this.grMinMax.Location = new System.Drawing.Point(4, 407);
            this.grMinMax.Name = "grMinMax";
            this.grMinMax.Size = new System.Drawing.Size(332, 131);
            this.grMinMax.TabIndex = 274;
            this.grMinMax.TabStop = false;
            this.grMinMax.Text = "Posiciones mínimas y máximas";
            // 
            // btPegar
            // 
            this.btPegar.BackColor = System.Drawing.Color.LightSalmon;
            this.btPegar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btPegar.ForeColor = System.Drawing.Color.Black;
            this.btPegar.Image = ((System.Drawing.Image)(resources.GetObject("btPegar.Image")));
            this.btPegar.Location = new System.Drawing.Point(81, 20);
            this.btPegar.Name = "btPegar";
            this.btPegar.Size = new System.Drawing.Size(24, 24);
            this.btPegar.TabIndex = 561;
            this.btPegar.UseVisualStyleBackColor = false;
            this.btPegar.Click += new System.EventHandler(this.btPegar_Click);
            // 
            // btCopiar
            // 
            this.btCopiar.BackColor = System.Drawing.Color.LightSalmon;
            this.btCopiar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btCopiar.ForeColor = System.Drawing.Color.Black;
            this.btCopiar.Image = ((System.Drawing.Image)(resources.GetObject("btCopiar.Image")));
            this.btCopiar.Location = new System.Drawing.Point(56, 20);
            this.btCopiar.Name = "btCopiar";
            this.btCopiar.Size = new System.Drawing.Size(24, 24);
            this.btCopiar.TabIndex = 560;
            this.btCopiar.UseVisualStyleBackColor = false;
            this.btCopiar.Click += new System.EventHandler(this.btCopiar_Click);
            // 
            // btUndo
            // 
            this.btUndo.BackColor = System.Drawing.Color.LightSalmon;
            this.btUndo.Enabled = false;
            this.btUndo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btUndo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btUndo.ForeColor = System.Drawing.Color.Black;
            this.btUndo.Location = new System.Drawing.Point(104, 40);
            this.btUndo.Name = "btUndo";
            this.btUndo.Size = new System.Drawing.Size(24, 24);
            this.btUndo.TabIndex = 281;
            this.btUndo.Text = "11";
            this.btUndo.UseVisualStyleBackColor = false;
            this.btUndo.Visible = false;
            this.btUndo.Click += new System.EventHandler(this.btUndo_Click);
            // 
            // btVerPorcentaje
            // 
            this.btVerPorcentaje.BackColor = System.Drawing.Color.LightSalmon;
            this.btVerPorcentaje.Enabled = false;
            this.btVerPorcentaje.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btVerPorcentaje.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btVerPorcentaje.ForeColor = System.Drawing.Color.Black;
            this.btVerPorcentaje.Location = new System.Drawing.Point(106, 20);
            this.btVerPorcentaje.Name = "btVerPorcentaje";
            this.btVerPorcentaje.Size = new System.Drawing.Size(24, 24);
            this.btVerPorcentaje.TabIndex = 280;
            this.btVerPorcentaje.Text = "%";
            this.btVerPorcentaje.UseVisualStyleBackColor = false;
            this.btVerPorcentaje.Visible = false;
            this.btVerPorcentaje.Click += new System.EventHandler(this.btVerPorcentaje_Click);
            // 
            // label55
            // 
            this.label55.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label55.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label55.ForeColor = System.Drawing.Color.Black;
            this.label55.Location = new System.Drawing.Point(4, 48);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(60, 16);
            this.label55.TabIndex = 279;
            this.label55.Text = "Posición";
            this.label55.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btGuardarLimites
            // 
            this.btGuardarLimites.BackColor = System.Drawing.Color.LightSalmon;
            this.btGuardarLimites.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btGuardarLimites.ForeColor = System.Drawing.Color.Black;
            this.btGuardarLimites.Image = ((System.Drawing.Image)(resources.GetObject("btGuardarLimites.Image")));
            this.btGuardarLimites.Location = new System.Drawing.Point(31, 20);
            this.btGuardarLimites.Name = "btGuardarLimites";
            this.btGuardarLimites.Size = new System.Drawing.Size(24, 24);
            this.btGuardarLimites.TabIndex = 278;
            this.btGuardarLimites.UseVisualStyleBackColor = false;
            this.btGuardarLimites.Click += new System.EventHandler(this.btGuardarLimites_Click);
            // 
            // btLeerLimites
            // 
            this.btLeerLimites.BackColor = System.Drawing.Color.LightSalmon;
            this.btLeerLimites.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btLeerLimites.ForeColor = System.Drawing.Color.Black;
            this.btLeerLimites.Image = ((System.Drawing.Image)(resources.GetObject("btLeerLimites.Image")));
            this.btLeerLimites.Location = new System.Drawing.Point(6, 20);
            this.btLeerLimites.Name = "btLeerLimites";
            this.btLeerLimites.Size = new System.Drawing.Size(24, 24);
            this.btLeerLimites.TabIndex = 277;
            this.btLeerLimites.UseVisualStyleBackColor = false;
            this.btLeerLimites.Click += new System.EventHandler(this.btLeerLimites_Click);
            // 
            // label54
            // 
            this.label54.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label54.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label54.ForeColor = System.Drawing.Color.Black;
            this.label54.Location = new System.Drawing.Point(186, 20);
            this.label54.Name = "label54";
            this.label54.Size = new System.Drawing.Size(86, 24);
            this.label54.TabIndex = 276;
            this.label54.Text = "Nº columnas filtro";
            this.label54.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbColumnasAGrabar
            // 
            this.lbColumnasAGrabar.BackColor = System.Drawing.Color.White;
            this.lbColumnasAGrabar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbColumnasAGrabar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbColumnasAGrabar.ForeColor = System.Drawing.Color.Black;
            this.lbColumnasAGrabar.Location = new System.Drawing.Point(276, 20);
            this.lbColumnasAGrabar.Name = "lbColumnasAGrabar";
            this.lbColumnasAGrabar.Size = new System.Drawing.Size(52, 24);
            this.lbColumnasAGrabar.TabIndex = 275;
            this.lbColumnasAGrabar.Text = "0";
            this.lbColumnasAGrabar.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btGrabarFiltro
            // 
            this.btGrabarFiltro.BackColor = System.Drawing.Color.Silver;
            this.btGrabarFiltro.Enabled = false;
            this.btGrabarFiltro.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btGrabarFiltro.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btGrabarFiltro.ForeColor = System.Drawing.Color.Black;
            this.btGrabarFiltro.Image = ((System.Drawing.Image)(resources.GetObject("btGrabarFiltro.Image")));
            this.btGrabarFiltro.Location = new System.Drawing.Point(156, 20);
            this.btGrabarFiltro.Name = "btGrabarFiltro";
            this.btGrabarFiltro.Size = new System.Drawing.Size(24, 24);
            this.btGrabarFiltro.TabIndex = 274;
            this.btGrabarFiltro.UseVisualStyleBackColor = false;
            this.btGrabarFiltro.Click += new System.EventHandler(this.button1_Click_1);
            this.btGrabarFiltro.EnabledChanged += new System.EventHandler(this.btGrabarFiltro_EnabledChanged);
            // 
            // btFiltrar
            // 
            this.btFiltrar.BackColor = System.Drawing.Color.Silver;
            this.btFiltrar.Enabled = false;
            this.btFiltrar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btFiltrar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btFiltrar.ForeColor = System.Drawing.Color.Black;
            this.btFiltrar.Image = ((System.Drawing.Image)(resources.GetObject("btFiltrar.Image")));
            this.btFiltrar.Location = new System.Drawing.Point(131, 20);
            this.btFiltrar.Name = "btFiltrar";
            this.btFiltrar.Size = new System.Drawing.Size(24, 24);
            this.btFiltrar.TabIndex = 273;
            this.btFiltrar.UseVisualStyleBackColor = false;
            this.btFiltrar.Click += new System.EventHandler(this.btFiltrar_Click);
            this.btFiltrar.EnabledChanged += new System.EventHandler(this.btFiltrar_EnabledChanged);
            // 
            // label52
            // 
            this.label52.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label52.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label52.ForeColor = System.Drawing.Color.Black;
            this.label52.Location = new System.Drawing.Point(4, 86);
            this.label52.Name = "label52";
            this.label52.Size = new System.Drawing.Size(60, 20);
            this.label52.TabIndex = 16;
            this.label52.Text = "Máxima";
            this.label52.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label51
            // 
            this.label51.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label51.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label51.ForeColor = System.Drawing.Color.Black;
            this.label51.Location = new System.Drawing.Point(4, 64);
            this.label51.Name = "label51";
            this.label51.Size = new System.Drawing.Size(60, 20);
            this.label51.TabIndex = 15;
            this.label51.Text = "Mínima";
            this.label51.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txMax10
            // 
            this.txMax10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txMax10.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txMax10.ForeColor = System.Drawing.Color.Black;
            this.txMax10.Location = new System.Drawing.Point(276, 86);
            this.txMax10.Name = "txMax10";
            this.txMax10.Size = new System.Drawing.Size(52, 21);
            this.txMax10.TabIndex = 14;
            // 
            // txMin10
            // 
            this.txMin10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txMin10.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txMin10.ForeColor = System.Drawing.Color.Black;
            this.txMin10.Location = new System.Drawing.Point(276, 64);
            this.txMin10.Name = "txMin10";
            this.txMin10.Size = new System.Drawing.Size(52, 21);
            this.txMin10.TabIndex = 13;
            // 
            // label48
            // 
            this.label48.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label48.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label48.ForeColor = System.Drawing.Color.Black;
            this.label48.Location = new System.Drawing.Point(276, 48);
            this.label48.Name = "label48";
            this.label48.Size = new System.Drawing.Size(52, 16);
            this.label48.TabIndex = 12;
            this.label48.Text = "de 10:";
            this.label48.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txMax11
            // 
            this.txMax11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txMax11.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txMax11.ForeColor = System.Drawing.Color.Black;
            this.txMax11.Location = new System.Drawing.Point(223, 86);
            this.txMax11.Name = "txMax11";
            this.txMax11.Size = new System.Drawing.Size(52, 21);
            this.txMax11.TabIndex = 11;
            // 
            // txMin11
            // 
            this.txMin11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txMin11.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txMin11.ForeColor = System.Drawing.Color.Black;
            this.txMin11.Location = new System.Drawing.Point(223, 64);
            this.txMin11.Name = "txMin11";
            this.txMin11.Size = new System.Drawing.Size(52, 21);
            this.txMin11.TabIndex = 10;
            // 
            // label47
            // 
            this.label47.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label47.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label47.ForeColor = System.Drawing.Color.Black;
            this.label47.Location = new System.Drawing.Point(223, 48);
            this.label47.Name = "label47";
            this.label47.Size = new System.Drawing.Size(52, 16);
            this.label47.TabIndex = 9;
            this.label47.Text = "de 11:";
            this.label47.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txMax12
            // 
            this.txMax12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txMax12.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txMax12.ForeColor = System.Drawing.Color.Black;
            this.txMax12.Location = new System.Drawing.Point(170, 86);
            this.txMax12.Name = "txMax12";
            this.txMax12.Size = new System.Drawing.Size(52, 21);
            this.txMax12.TabIndex = 8;
            // 
            // txMin12
            // 
            this.txMin12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txMin12.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txMin12.ForeColor = System.Drawing.Color.Black;
            this.txMin12.Location = new System.Drawing.Point(170, 64);
            this.txMin12.Name = "txMin12";
            this.txMin12.Size = new System.Drawing.Size(52, 21);
            this.txMin12.TabIndex = 7;
            // 
            // label46
            // 
            this.label46.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label46.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label46.ForeColor = System.Drawing.Color.Black;
            this.label46.Location = new System.Drawing.Point(170, 48);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(52, 16);
            this.label46.TabIndex = 6;
            this.label46.Text = "de 12:";
            this.label46.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txMax13
            // 
            this.txMax13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txMax13.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txMax13.ForeColor = System.Drawing.Color.Black;
            this.txMax13.Location = new System.Drawing.Point(117, 86);
            this.txMax13.Name = "txMax13";
            this.txMax13.Size = new System.Drawing.Size(52, 21);
            this.txMax13.TabIndex = 5;
            // 
            // txMin13
            // 
            this.txMin13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txMin13.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txMin13.ForeColor = System.Drawing.Color.Black;
            this.txMin13.Location = new System.Drawing.Point(117, 64);
            this.txMin13.Name = "txMin13";
            this.txMin13.Size = new System.Drawing.Size(52, 21);
            this.txMin13.TabIndex = 4;
            // 
            // label45
            // 
            this.label45.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label45.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label45.ForeColor = System.Drawing.Color.Black;
            this.label45.Location = new System.Drawing.Point(117, 48);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(52, 16);
            this.label45.TabIndex = 3;
            this.label45.Text = "de 13:";
            this.label45.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txMax14
            // 
            this.txMax14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txMax14.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txMax14.ForeColor = System.Drawing.Color.Black;
            this.txMax14.Location = new System.Drawing.Point(64, 86);
            this.txMax14.Name = "txMax14";
            this.txMax14.Size = new System.Drawing.Size(52, 21);
            this.txMax14.TabIndex = 2;
            // 
            // txMin14
            // 
            this.txMin14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txMin14.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txMin14.ForeColor = System.Drawing.Color.Black;
            this.txMin14.Location = new System.Drawing.Point(64, 64);
            this.txMin14.Name = "txMin14";
            this.txMin14.Size = new System.Drawing.Size(52, 21);
            this.txMin14.TabIndex = 1;
            // 
            // label44
            // 
            this.label44.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label44.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label44.ForeColor = System.Drawing.Color.Black;
            this.label44.Location = new System.Drawing.Point(64, 48);
            this.label44.Name = "label44";
            this.label44.Size = new System.Drawing.Size(52, 16);
            this.label44.TabIndex = 0;
            this.label44.Text = "de 14:";
            this.label44.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.BackColor = System.Drawing.Color.Bisque;
            this.groupBox2.Controls.Add(this.cmbAciertosABuscar);
            this.groupBox2.Controls.Add(this.txProbabilidad);
            this.groupBox2.Controls.Add(this.lblValoracion);
            this.groupBox2.Controls.Add(this.btBuscarColumna);
            this.groupBox2.Controls.Add(this.btSiguiente);
            this.groupBox2.Controls.Add(this.btAnterior);
            this.groupBox2.Controls.Add(this.txAciertos);
            this.groupBox2.Controls.Add(this.label50);
            this.groupBox2.Controls.Add(this.numericUpDown1);
            this.groupBox2.Controls.Add(this.TxColumnaEnPosicion);
            this.groupBox2.Controls.Add(this.label49);
            this.groupBox2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox2.Location = new System.Drawing.Point(608, 340);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(176, 120);
            this.groupBox2.TabIndex = 275;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Ver columna";
            // 
            // cmbAciertosABuscar
            // 
            this.cmbAciertosABuscar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbAciertosABuscar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cmbAciertosABuscar.ForeColor = System.Drawing.Color.Black;
            this.cmbAciertosABuscar.Items.AddRange(new object[] {
            "14",
            "13",
            "12",
            "11",
            "10"});
            this.cmbAciertosABuscar.Location = new System.Drawing.Point(104, 64);
            this.cmbAciertosABuscar.MaxLength = 2;
            this.cmbAciertosABuscar.Name = "cmbAciertosABuscar";
            this.cmbAciertosABuscar.Size = new System.Drawing.Size(44, 21);
            this.cmbAciertosABuscar.TabIndex = 280;
            this.cmbAciertosABuscar.SelectedIndexChanged += new System.EventHandler(this.cmbAciertosABuscar_SelectedIndexChanged);
            // 
            // txProbabilidad
            // 
            this.txProbabilidad.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txProbabilidad.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txProbabilidad.ForeColor = System.Drawing.Color.Black;
            this.txProbabilidad.Location = new System.Drawing.Point(60, 86);
            this.txProbabilidad.Name = "txProbabilidad";
            this.txProbabilidad.Size = new System.Drawing.Size(88, 20);
            this.txProbabilidad.TabIndex = 279;
            this.txProbabilidad.Text = "0";
            // 
            // lblValoracion
            // 
            this.lblValoracion.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblValoracion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValoracion.ForeColor = System.Drawing.Color.Black;
            this.lblValoracion.Location = new System.Drawing.Point(24, 86);
            this.lblValoracion.Name = "lblValoracion";
            this.lblValoracion.Size = new System.Drawing.Size(36, 20);
            this.lblValoracion.TabIndex = 278;
            this.lblValoracion.Text = "LN:";
            this.lblValoracion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btBuscarColumna
            // 
            this.btBuscarColumna.BackColor = System.Drawing.Color.LightSalmon;
            this.btBuscarColumna.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btBuscarColumna.ForeColor = System.Drawing.Color.Black;
            this.btBuscarColumna.Image = ((System.Drawing.Image)(resources.GetObject("btBuscarColumna.Image")));
            this.btBuscarColumna.Location = new System.Drawing.Point(129, 42);
            this.btBuscarColumna.Name = "btBuscarColumna";
            this.btBuscarColumna.Size = new System.Drawing.Size(23, 20);
            this.btBuscarColumna.TabIndex = 277;
            this.btBuscarColumna.UseVisualStyleBackColor = false;
            this.btBuscarColumna.Click += new System.EventHandler(this.button1_Click_2);
            // 
            // btSiguiente
            // 
            this.btSiguiente.BackColor = System.Drawing.Color.LightSalmon;
            this.btSiguiente.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btSiguiente.Font = new System.Drawing.Font("Arial Black", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btSiguiente.ForeColor = System.Drawing.Color.Black;
            this.btSiguiente.Image = ((System.Drawing.Image)(resources.GetObject("btSiguiente.Image")));
            this.btSiguiente.Location = new System.Drawing.Point(149, 64);
            this.btSiguiente.Name = "btSiguiente";
            this.btSiguiente.Size = new System.Drawing.Size(23, 23);
            this.btSiguiente.TabIndex = 276;
            this.btSiguiente.UseVisualStyleBackColor = false;
            this.btSiguiente.Click += new System.EventHandler(this.btSiguiente_Click);
            // 
            // btAnterior
            // 
            this.btAnterior.BackColor = System.Drawing.Color.LightSalmon;
            this.btAnterior.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btAnterior.Font = new System.Drawing.Font("Arial", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btAnterior.ForeColor = System.Drawing.Color.Black;
            this.btAnterior.Image = ((System.Drawing.Image)(resources.GetObject("btAnterior.Image")));
            this.btAnterior.Location = new System.Drawing.Point(2, 64);
            this.btAnterior.Name = "btAnterior";
            this.btAnterior.Size = new System.Drawing.Size(23, 23);
            this.btAnterior.TabIndex = 275;
            this.btAnterior.UseVisualStyleBackColor = false;
            this.btAnterior.Click += new System.EventHandler(this.btAnterior_Click);
            // 
            // txAciertos
            // 
            this.txAciertos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txAciertos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txAciertos.ForeColor = System.Drawing.Color.Black;
            this.txAciertos.Location = new System.Drawing.Point(76, 64);
            this.txAciertos.Name = "txAciertos";
            this.txAciertos.Size = new System.Drawing.Size(28, 21);
            this.txAciertos.TabIndex = 274;
            this.txAciertos.Text = "0";
            // 
            // label50
            // 
            this.label50.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label50.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label50.ForeColor = System.Drawing.Color.Black;
            this.label50.Location = new System.Drawing.Point(24, 64);
            this.label50.Name = "label50";
            this.label50.Size = new System.Drawing.Size(52, 23);
            this.label50.TabIndex = 273;
            this.label50.Text = "Aciertos:";
            this.label50.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numericUpDown1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numericUpDown1.ForeColor = System.Drawing.Color.Black;
            this.numericUpDown1.Location = new System.Drawing.Point(76, 20);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            4782968,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(72, 20);
            this.numericUpDown1.TabIndex = 272;
            this.numericUpDown1.TextChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            this.numericUpDown1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenericoSinDecimales_KeyPress);
            // 
            // TxColumnaEnPosicion
            // 
            this.TxColumnaEnPosicion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TxColumnaEnPosicion.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.TxColumnaEnPosicion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxColumnaEnPosicion.ForeColor = System.Drawing.Color.Black;
            this.TxColumnaEnPosicion.Location = new System.Drawing.Point(24, 42);
            this.TxColumnaEnPosicion.MaxLength = 14;
            this.TxColumnaEnPosicion.Name = "TxColumnaEnPosicion";
            this.TxColumnaEnPosicion.Size = new System.Drawing.Size(104, 20);
            this.TxColumnaEnPosicion.TabIndex = 271;
            this.TxColumnaEnPosicion.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TxColumnaEnPosicion.TextChanged += new System.EventHandler(this.TxColumnaEnPosicion_TextChanged);
            this.TxColumnaEnPosicion.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txColumna_KeyPress);
            // 
            // label49
            // 
            this.label49.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label49.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label49.ForeColor = System.Drawing.Color.Black;
            this.label49.Location = new System.Drawing.Point(24, 20);
            this.label49.Name = "label49";
            this.label49.Size = new System.Drawing.Size(52, 20);
            this.label49.TabIndex = 1;
            this.label49.Text = "Posición:";
            this.label49.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkAcumular
            // 
            this.chkAcumular.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.chkAcumular.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkAcumular.Location = new System.Drawing.Point(340, 464);
            this.chkAcumular.Name = "chkAcumular";
            this.chkAcumular.Size = new System.Drawing.Size(236, 20);
            this.chkAcumular.TabIndex = 276;
            this.chkAcumular.Text = "Acumular resultados de jornadas distintas";
            this.chkAcumular.Visible = false;
            // 
            // txLNCentral
            // 
            this.txLNCentral.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txLNCentral.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txLNCentral.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txLNCentral.Location = new System.Drawing.Point(452, 492);
            this.txLNCentral.Name = "txLNCentral";
            this.txLNCentral.Size = new System.Drawing.Size(76, 21);
            this.txLNCentral.TabIndex = 564;
            this.txLNCentral.Text = "0";
            this.txLNCentral.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txLNCentral.TextChanged += new System.EventHandler(this.txLNCentral_TextChanged);
            // 
            // lbLNCentral
            // 
            this.lbLNCentral.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lbLNCentral.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lbLNCentral.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbLNCentral.Location = new System.Drawing.Point(340, 492);
            this.lbLNCentral.Name = "lbLNCentral";
            this.lbLNCentral.Size = new System.Drawing.Size(112, 21);
            this.lbLNCentral.TabIndex = 565;
            this.lbLNCentral.Text = "LN central";
            this.lbLNCentral.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // controlPorcentajes1
            // 
            this.controlPorcentajes1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.controlPorcentajes1.archivoPorcentajes = null;
            this.controlPorcentajes1.BackColor = System.Drawing.Color.Bisque;
            this.controlPorcentajes1.CaptionText = "  P O R C E N T A J E S";
            this.controlPorcentajes1.FormatoFicheroValoraciones = ((short)(0));
            this.controlPorcentajes1.Jornada = "01";
            this.controlPorcentajes1.Location = new System.Drawing.Point(448, 4);
            this.controlPorcentajes1.Name = "controlPorcentajes1";
            this.controlPorcentajes1.ReadOnly = false;
            this.controlPorcentajes1.Size = new System.Drawing.Size(160, 332);
            this.controlPorcentajes1.TabIndex = 566;
            this.controlPorcentajes1.Temporada = "2004/2005";
            this.controlPorcentajes1.Modificado += new System.EventHandler(this.controlPorcentajes1_Modificado);
            // 
            // btPrAcum
            // 
            this.btPrAcum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btPrAcum.BackColor = System.Drawing.Color.DarkSalmon;
            this.btPrAcum.Enabled = false;
            this.btPrAcum.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btPrAcum.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btPrAcum.Location = new System.Drawing.Point(348, 432);
            this.btPrAcum.Name = "btPrAcum";
            this.btPrAcum.Size = new System.Drawing.Size(98, 24);
            this.btPrAcum.TabIndex = 567;
            this.btPrAcum.Text = "% PrAcum.14";
            this.btPrAcum.UseVisualStyleBackColor = false;
            this.btPrAcum.Click += new System.EventHandler(this.btPrAcum_Click);
            // 
            // dgResultadosPrAcum
            // 
            this.dgResultadosPrAcum.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgResultadosPrAcum.CaptionVisible = false;
            this.dgResultadosPrAcum.DataMember = "";
            this.dgResultadosPrAcum.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgResultadosPrAcum.Location = new System.Drawing.Point(0, 0);
            this.dgResultadosPrAcum.Name = "dgResultadosPrAcum";
            this.dgResultadosPrAcum.ReadOnly = true;
            this.dgResultadosPrAcum.Size = new System.Drawing.Size(444, 400);
            this.dgResultadosPrAcum.TabIndex = 568;
            this.dgResultadosPrAcum.Visible = false;
            // 
            // TramificarForm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(792, 566);
            this.Controls.Add(this.dgResultadosPrAcum);
            this.Controls.Add(this.btPrAcum);
            this.Controls.Add(this.controlPorcentajes1);
            this.Controls.Add(this.lbLNCentral);
            this.Controls.Add(this.txLNCentral);
            this.Controls.Add(this.statusBar1);
            this.Controls.Add(this.chkAcumular);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.grMinMax);
            this.Controls.Add(this.btGrabarTramos);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.dgResultados);
            this.Controls.Add(this.grDefTramo);
            this.Controls.Add(this.btTramificar);
            this.Controls.Add(this.btCancelar);
            // this.Menu = this.mainMenu1; // Legacy menu assignment - handled by compatibility layer
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "TramificarForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tramificar";
            this.grDefTramo.ResumeLayout(false);
            this.grDefTramo.PerformLayout();
            // StatusBarPanel controls don't support ISupportInitialize in .NET 8
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).EndInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel2)).EndInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel3)).EndInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgResultados)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numJornada)).EndInit();
            this.grPremiosLAE.ResumeLayout(false);
            this.grPremiosLAE.PerformLayout();
            this.grMinMax.ResumeLayout(false);
            this.grMinMax.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgResultadosPrAcum)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void CargarUltimaJornada()
		{
		    string NombreFicheroJornadas=Application.StartupPath + "/Jornadas/InfoJornadasLAE.txt";
			string Linea="";
            IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(NombreFicheroJornadas);
			while( comBaseCols.SiguienteColumna() )
			{
				Linea = comBaseCols.LeeColumnaSinComas();
			}
			comBaseCols.Cerrar();
			ValorsJornada=Linea.Split ((char) 9);
			numJornada.Value=Convert.ToInt32 (ValorsJornada[2]);
			controlPorcentajes1 .Jornada =ValorsJornada[2];
			controlPorcentajes1 .Temporada =ValorsJornada[1];
		}
		protected void InicializaGridResultados()
		{			
			DataGridTableStyle tableStyle = new DataGridTableStyle();
			tableStyle.MappingName = "ArrayList";
			tableStyle.ColumnHeadersVisible = true;
			
			// Crear Columnas 
			// MappingName tiene que ser igual a cada una de las "properties"
			// de la clase Tramo.


		    //		NumeroDeTramo
			DataGridTextBoxColumn cs = new DataGridTextBoxColumn();
			cs.MappingName = "NumeroDeTramo";
			cs.HeaderText = "Nº";
			cs.Width = 30;
			tableStyle.GridColumnStyles.Add(cs);

			//		ValorIzquierda
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "ValorIzquierda";
			cs.HeaderText = "Col. Inf.";
			cs.Width = 60;
			tableStyle.GridColumnStyles.Add(cs);
	

			//		ValorDerecha
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "ValorDerecha";
			cs.HeaderText = "Col. Sup.";
			cs.Width = 60;
			tableStyle.GridColumnStyles.Add(cs);

			//		NumColumnasTramo
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "NumColumnasTramo";
			cs.HeaderText = "Nº Cols.";
			cs.Width = 40;
			tableStyle.GridColumnStyles.Add(cs);

			//		ProbAcumulada
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "ProbAcumulada";
			cs.HeaderText = "Prob. Max.";
			cs.Width = 60;
			tableStyle.GridColumnStyles.Add(cs);

			//P14
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "P14";
			cs.HeaderText = "14";
			cs.Width = 20;
			tableStyle.GridColumnStyles.Add(cs);
			
			//P13
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "P13";
			cs.HeaderText = "13";
			cs.Width = 25;
			tableStyle.GridColumnStyles.Add(cs);
			
			//P12
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "P12";
			cs.HeaderText = "12";
			cs.Width = 30;
			tableStyle.GridColumnStyles.Add(cs);
			
			//P11
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "P11";
			cs.HeaderText = "11";
			cs.Width = 35;
			tableStyle.GridColumnStyles.Add(cs);
			
			//P10
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "P10";
			cs.HeaderText = "10";
			cs.Width = 40;
			tableStyle.GridColumnStyles.Add(cs);
			
			//ColumnasPremiadas
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "ColumnasPremiadas";
			cs.HeaderText = "NºPremios";
			cs.Width = 40;
			tableStyle.GridColumnStyles.Add(cs);

			//TotalImportePremios
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "TotalImportePremios";
			cs.HeaderText = "Imp.Premios";
			cs.Width = 60;
			tableStyle.GridColumnStyles.Add(cs);
	
			//Balance
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "Balance";
			cs.HeaderText = "Ingresos-Gastos";
			cs.Width = 60;
			tableStyle.GridColumnStyles.Add(cs);
			dgResultados.TableStyles.Add(tableStyle);			

			dgResultados.TableStyles.Add(tableStyle);			
		}
		protected void InicializaGridResultadosPrAcum()
		{			
			DataGridTableStyle tableStyle = new DataGridTableStyle();
			tableStyle.MappingName = "ArrayList";
			tableStyle.ColumnHeadersVisible = true;
			
			// Crear Columnas 
			// MappingName tiene que ser igual a cada una de las "properties"
			// de la clase RegistroPrAcum.

		    //Temporada
			DataGridTextBoxColumn cs = new DataGridTextBoxColumn();
			cs.MappingName = "Temporada";
			cs.HeaderText = "Temporada";
			cs.Width = 70;
			tableStyle.GridColumnStyles.Add(cs);

			//Jornada
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "Jornada";
			cs.HeaderText = "Jor.";
			cs.Width = 30;
			tableStyle.GridColumnStyles.Add(cs);
	
			//PrAcum
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "PrAcumulada";
			cs.HeaderText = "% acumulado";
			cs.Width = 50;
			tableStyle.GridColumnStyles.Add(cs);
			
			//Premio
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "Premio";
			cs.HeaderText = "Premio 14";
			cs.Width = 80;
			tableStyle.GridColumnStyles.Add(cs);
			
			//NumColumnas
			cs = new DataGridTextBoxColumn();
			cs.MappingName = "NumColumnas";
			cs.HeaderText = "Nº Columnas";
			cs.Width = 80;
			tableStyle.GridColumnStyles.Add(cs);

			dgResultadosPrAcum.TableStyles.Add(tableStyle);			
		}
		protected void GridDataBind()
		{
			dgResultados.DataSource = null;
			dgResultados.DataSource = Tramos;	
			dgResultados.Refresh ();
		}
		protected void GridDataBindPrAcum()
		{
			dgResultadosPrAcum.DataSource = null;
			dgResultadosPrAcum.DataSource = PrAcumulados;	
			dgResultadosPrAcum.Refresh ();
		}

		private void btTramificar_Click(object sender, EventArgs e)
		{
			Cursor.Current = Cursors.WaitCursor ;
			PrAcumulados =new ArrayList();
			int x;
			ContadorJornadas=0;
			short ContaNumJornadas=0;
			NumIntervalos=Convert.ToInt32 (cmbNumTrams.Text);
			if (noColumnasIniciales < NumIntervalos )
			{
				NumIntervalos=noColumnasIniciales;
				cmbNumTrams.Text =NumIntervalos.ToString ();
				txIntervalo.Text = "1";
				Application.DoEvents ();
			}
			btPrAcum.Enabled = false;
			dgResultadosPrAcum.Visible =false;
			dgResultados.Visible =true;
			
			chkAcumular.Visible =true;
            if (mnuJornadasMultiple.Checked)
            {
                if (FormatoFicheroValoraciones == 44)
                {
                    double[,] limites = new double[6, 2];
                    limites[0, 0] = Convert.ToDouble(DialogoAnalisiMultiple.txRecaudacionMinima.Text);
                    limites[0, 1] = Convert.ToDouble(DialogoAnalisiMultiple.txRecaudacionMaxima.Text);
                    limites[1, 0] = Convert.ToDouble(DialogoAnalisiMultiple.txPremioMinimoDe14.Text);
                    limites[1, 1] = Convert.ToDouble(DialogoAnalisiMultiple.txPremioMaximoDe14.Text);
                    limites[2, 0] = Convert.ToDouble(DialogoAnalisiMultiple.txPremioMinimoDe13.Text);
                    limites[2, 1] = Convert.ToDouble(DialogoAnalisiMultiple.txPremioMaximoDe13.Text);
                    limites[3, 0] = Convert.ToDouble(DialogoAnalisiMultiple.txPremioMinimoDe12.Text);
                    limites[3, 1] = Convert.ToDouble(DialogoAnalisiMultiple.txPremioMaximoDe12.Text);
                    limites[4, 0] = Convert.ToDouble(DialogoAnalisiMultiple.txPremioMinimoDe11.Text);
                    limites[4, 1] = Convert.ToDouble(DialogoAnalisiMultiple.txPremioMaximoDe11.Text);
                    limites[5, 0] = Convert.ToDouble(DialogoAnalisiMultiple.txPremioMinimoDe10.Text);
                    limites[5, 1] = Convert.ToDouble(DialogoAnalisiMultiple.txPremioMaximoDe10.Text);

                    chkAcumular.Checked = false;
                    switch (DialogoAnalisiMultiple.Es14Triples)
                    {
                        case 0:
                            {
                                Habilita14Triples();

                                for (int i = 0; i < DialogoAnalisiMultiple.chkTemporadas.Items.Count; i++)
                                {
                                    if (DialogoAnalisiMultiple.chkTemporadas.GetItemChecked(i))
                                    {
                                        txTemporada.Text = DialogoAnalisiMultiple.chkTemporadas.Items[i].ToString().Substring(0, 4);
                                        for (x = 1; x < numeroMaximoJornadas; x++)
                                        {
                                            numJornada.Value = x;
                                            if (JornadaEncontrada)
                                            {
                                                if (JornadaDentroDeLimites(limites))
                                                {
                                                    Tramifica();
                                                    ContadorJornadas++;
                                                    chkAcumular.Checked = true;
                                                    Application.DoEvents();
                                                }
                                                ContaNumJornadas++;
                                            }
                                        }
                                    }
                                    if (salida) break;
                                }
                                break;
                            }
                        case 1:
                            {
                                mnu14Triples.Checked = false;
                                mnuFichero.Checked = true;
                                mnuFicheroSobre14T.Checked = false;

                                string archivoEntrada = DialogoAnalisiMultiple.FicheroCombinación;
                                Bits.SetAll(true);
                                CargarFicheroDeColumnas(archivoEntrada);

                                for (int i = 0; i < DialogoAnalisiMultiple.chkTemporadas.Items.Count; i++)
                                {
                                    if (DialogoAnalisiMultiple.chkTemporadas.GetItemChecked(i))
                                    {
                                        txTemporada.Text = DialogoAnalisiMultiple.chkTemporadas.Items[i].ToString().Substring(0, 4);
                                        for (x = 1; x < 44; x++)
                                        {
                                            numJornada.Value = x;
                                            if (JornadaEncontrada)
                                            {
                                                if (JornadaDentroDeLimites(limites))
                                                {
                                                    Tramifica();
                                                    ContadorJornadas++;
                                                    chkAcumular.Checked = true;
                                                    Application.DoEvents();
                                                }
                                                ContaNumJornadas++;
                                            }
                                        }
                                    }

                                    if (salida) break;
                                }
                                break;
                            }

                        case 2:
                            {

                                Cursor.Current = Cursors.WaitCursor;
                                escrutado = false;
                                mnu14Triples.Checked = false;
                                if (DialogoAnalisiMultiple.chkFicherosEn14T.Checked)
                                {
                                    mnuFichero.Checked = false;
                                    mnuFicheroSobre14T.Checked = true;
                                }
                                else
                                {
                                    mnuFichero.Checked = true;
                                    mnuFicheroSobre14T.Checked = false;
                                }

                                foreach (Combinacion combi in DialogoAnalisiMultiple.ListaCombinaciones)
                                {
                                    string archivoEntrada = combi.Path;
                                    txTemporada.Text = combi.Temporada.Substring(0, 4);
                                    numJornada.Value = Convert.ToInt32(combi.Jornada);
                                    if (JornadaDentroDeLimites(limites))
                                    {
                                        Bits.SetAll(true);
                                        CargarFicheroDeColumnas(archivoEntrada);
                                        Tramifica();
                                        ContadorJornadas++;
                                        chkAcumular.Checked = true;
                                    }
                                    ContaNumJornadas++;
                                }
                                Cursor.Current = Cursors.Default;
                                break;
                            }
                    }
                    mnuJornadasMultiple.Checked = false;
                    mnuJornadaSimple.Checked = true;

                    double pcent = Math.Round((double)(ContadorJornadas * 100) / ContaNumJornadas, 2);
                    statusBarPanel4.Text = "Jornadas procesadas = " + ContadorJornadas.ToString() + " de " + ContaNumJornadas.ToString() + "(" + pcent.ToString() + "%)";
                }
            }
            else
            {
                Tramifica();
            }
			if(txLNCentral.Text  =="0" )
			{
				btPrAcum.Enabled =true;
			}
			else 
			{
				btPrAcum.Enabled =false;
			}
			Cursor.Current = Cursors.Default;
		}
		private bool JornadaDentroDeLimites(double[,] limites)
		{
			bool resultado=true;
			double [] valoresJornada= new double[6];
			valoresJornada[0]=Convert.ToDouble(txRecaudacion.Text);
			valoresJornada[1]=Convert.ToDouble(tx14.Text);
			valoresJornada[2]=Convert.ToDouble(tx13.Text);
			valoresJornada[3]=Convert.ToDouble(tx12.Text);
			valoresJornada[4]=Convert.ToDouble(tx11.Text);
			valoresJornada[5]=Convert.ToDouble(tx10.Text);
			for(int i=0;i<6;i++)
			{
				if(limites[i,0] > valoresJornada[i]  || limites[i,1] < valoresJornada[i]) 
				{
					resultado=false;
					break;
				}
			}
			return resultado;
		}
		private void Tramifica()
		{
			btGrabarTramos.Enabled =true;
			if(chkAcumular.Checked)
			{
				escrutado=false;
				afegirTram=true;
			}
			else
			{
				btFiltrar.Enabled =true;
				afegirTram=false;
				InicializarMaximosiMinimos();
			}
			
			if(mnuPorProductos.Checked)
			{
			    OrdenaPorProductos();
			}
			else
			{
			    OrdenaPorSumas();
			}
			PonerMaximosiMinimos(false);
			numericUpDown1.Value =1;
		}

		private void Escrutar()
		{
			int Num;
			for(int i=0;i<4782969;i++)
			{
				Ap14T[i].Aciertos =0 ;
				Ap14T[i].Columna =i;
				Ap14T[i].Probabilidad =0;
			}
			ConvertidorDeBases col= new ConvertidorDeBases();
			Num = col.ConvColumnaANumero(txColumna.Text);
			Ap14T [Num].Aciertos =14;
			ColumnasADistancia1(Num,0,4);
			escrutado=true;
		}
		private void OrdenaPorProductos()
		{
			int Partido;
			float Prob=0;

			//---Leer Porcentajes --------------
			PonerValoracionEnVariables();
			Porcentajes Pct = new Porcentajes(v);
			p= Pct.ValoresNeperianos();

			//'---- probabilidad del 14 de la 1ª apuesta y 
			// cálculo de valores complementarios --------
			// La primera apuesta es: 11111111111111
			for (Partido = 0;Partido<14; Partido++)
			{
				Prob +=p[Partido, 0];
				Cr[Partido, 1] = p[Partido, 1] - p[Partido, 0];
				Cr[Partido, 2] = p[Partido, 2] - p[Partido, 0];
			}

			Calcula14Triples(Prob);
		}
		private void OrdenaPorSumas()
		{
			int Partido;
			float Prob=0;
			if (Convert.ToInt32 (txValMax.Text)>noColumnasIniciales) txValMax.Text=noColumnasIniciales.ToString ();

			//---Leer Porcentajes --------------
			PonerValoracionEnVariables();

			//'---- Suma del 14 de la 1ª apuesta i 
			// cálculo de valores complementarios --------
			for (Partido = 0;Partido<14; Partido++)
			{
				p[Partido, 0]=(float) v[Partido, 0];
				p[Partido, 1]=(float) v[Partido, 1];
				p[Partido, 2]=(float) v[Partido, 2];
				Prob +=p[Partido, 0];
				Cr[Partido, 1] = p[Partido, 1] - p[Partido, 0];
				Cr[Partido, 2] = p[Partido, 2] - p[Partido, 0];
			}
			Calcula14Triples(Prob);
		}
		private void Calcula14Triples(double Prob)
		{
			if(escrutado==false) 
			{
				statusBarPanel4.Text ="Escrutando ...";
				Application.DoEvents();
				Escrutar();

				statusBarPanel4.Text ="Calculando probabilidades ...";
				Application.DoEvents();
				Profundidad = 0;
				EncontrarDistantes1 (Prob, 0, 0, 14);
				Ap14T[0].Probabilidad = Math.Abs (Prob - _LN);//-Prob;
				Ap14T[0].Columna = 0;

				statusBarPanel4.Text ="Ordenando apuestas ...";
				Application.DoEvents();
				EliminaColumnas();
				ordena (0, 4782968);
			}
			AddPrAcum();
			statusBarPanel4.Text ="Tramificando apuestas ...";
			Application.DoEvents();

			if (afegirTram) TramificarAdd(); else Tramificar();
		    
			statusBarPanel4.Text = "Finalizado";
		}
		private void EliminaColumnas()
		{
			if(mnuFichero.Checked)
			{
				for(int i=0;i<4782969;i++)
				{
					if (Bits[i]) Ap14T[i].Probabilidad =1E+16;
				}
			}
		}


		private void Tramificar()
		{	
			int nr;
			int nc;

		    LimiteInferiorAbsoluto = Convert.ToDouble (txValMin.Text);
			LimiteSuperiorAbsoluto =Convert.ToDouble (txValMax.Text);
			NumIntervalos = Convert.ToDouble (cmbNumTrams.Text);
			Incremento = Convert.ToDouble (txIntervalo.Text);
			Premios[0]= Convert.ToDouble (tx14.Text);
			Premios[1]= Convert.ToDouble (tx13.Text);
			Premios[2]= Convert.ToDouble (tx12.Text);
			Premios[3]= Convert.ToDouble (tx11.Text);
			Premios[4]= Convert.ToDouble (tx10.Text);
			Tramos =new ArrayList();

			ProbabilidadAcumulada=0;

			for ( nr=Convert.ToInt32 (LimiteInferiorAbsoluto); nr< Convert.ToInt32 (LimiteSuperiorAbsoluto); nr +=Convert.ToInt32 (Incremento)) 
			{
				Tramo tr= new Tramo(Premios);
				c=0;
				int c2 = 0;
				tr.ValorIzquierda = nr;
				int[] Aciertos=new int[5];

				for (nc=nr;nc<nr+Incremento; nc++)
				{
					if (nc>noColumnasIniciales-1) break;
					ProbabilidadAcumulada = -Ap14T[nc].Probabilidad ;

					if(!Bits[Ap14T[nc].Columna] )
					{
						c++;
					
						if(Ap14T[nc].Aciertos >9) 
						{
							int indiceAciertos = 14-Ap14T[nc].Aciertos;
							Aciertos[indiceAciertos]++;
							tr.ColumnasPremiadas++;

							if(MaxiMin [indiceAciertos,0]<nc)MaxiMin [indiceAciertos,0]=nc;
							if(MaxiMin [indiceAciertos,1]>nc)MaxiMin [indiceAciertos,1]=nc;
						}
					}
					else
					{
						//contamos las columnas del tramo que no forman parte de la combi
						c2++;
					}
				}
				tr.ProbAcumulada =ProbabilidadAcumulada;
				tr.NumColumnasTramo =c;
				tr.ValorDerecha += tr.ValorIzquierda +c+c2-1;
				tr.PonerAciertos (Aciertos);
				tr.NumeroDeTramo=Tramos.Count +1;
				tr.CalculaTotalImportePremios() ;
				Tramos.Add (tr);
			}
			GridDataBind();
		}
		private void AddPrAcum()
		{
			double pAc =0;
			int i;
			for(i=0; i<4782969; i++)
			{
				if(Ap14T[i].Aciertos == 14) break;
				pAc += Math.Exp (-Ap14T[i].Probabilidad);
			}
			pAc *=100;
			RegistroPrAcum Ac = new RegistroPrAcum (txTemporada.Text  + "/" + txTemporada2.Text,numJornada.Value.ToString ().PadLeft (2,'0'),pAc, Convert.ToDouble(tx14.Text),i);
			PrAcumulados.Add (Ac);
		}
		private void TramificarAdd()
		{	
			int nr;
			int nc;
		    LimiteInferiorAbsoluto = Convert.ToDouble (txValMin.Text);
			LimiteSuperiorAbsoluto =Convert.ToDouble (txValMax.Text);
			NumIntervalos = Convert.ToDouble (cmbNumTrams.Text);
			Incremento = Convert.ToDouble (txIntervalo.Text);
			Premios[0]= Convert.ToDouble (tx14.Text);
			Premios[1]= Convert.ToDouble (tx13.Text);
			Premios[2]= Convert.ToDouble (tx12.Text);
			Premios[3]= Convert.ToDouble (tx11.Text);
			Premios[4]= Convert.ToDouble (tx10.Text);
			ProbabilidadAcumulada=0;
			int NumTramo = 0;
			object[] Trams = Tramos.ToArray ();
			Tramos.Clear() ;

			for ( nr=Convert.ToInt32 (LimiteInferiorAbsoluto); nr< Convert.ToInt32 (LimiteSuperiorAbsoluto); nr +=Convert.ToInt32 (Incremento)) 
			{
				Tramo tr= new Tramo(Premios);
				c=0;
				int c2 = 0;
				tr.ValorIzquierda = nr;
				int[] Aciertos=new int[5];

				for (nc=nr;nc<nr+Incremento; nc++)
				{
					if (nc>4782968) break;
					if(!Bits[Ap14T[nc].Columna])
					{
						ProbabilidadAcumulada = -Ap14T[nc].Probabilidad ;
						c++;

						if(Ap14T[nc].Aciertos >9) 
						{
							int indiceAciertos = 14-Ap14T[nc].Aciertos;
							Aciertos[indiceAciertos]++;
							if(MaxiMin [indiceAciertos,0]<nc)MaxiMin [indiceAciertos,0]=nc;
							if(MaxiMin [indiceAciertos,1]>nc)MaxiMin [indiceAciertos,1]=nc;
							tr.ColumnasPremiadas++;
						}
					}
					else
					{
						//contamos las columnas del tramo que no forman parte de la combi
						c2++;
					}
				}
				tr.ProbAcumulada =ProbabilidadAcumulada;
				tr.NumColumnasTramo =c;
				tr.ValorDerecha += tr.ValorIzquierda +c+c2-1;
				tr.PonerAciertos (Aciertos);
				tr.CalculaTotalImportePremios() ;
				tr.AddTramo((Tramo) Trams[NumTramo]);
				tr.NumeroDeTramo=Tramos.Count +1;
				
				Tramos.Add (tr);

			    NumTramo++;
			}

			GridDataBind();
		}

		private void ColumnasADistancia1(int IndiceInicial, int PosicionInicial,int pProfundidad)
		{
		    Profundidad++;
        
			//'--encontramos las apuestas que se diferencian en un solo signo ----
			for (int Partido = PosicionInicial;Partido<14;Partido++)
			{
			    int SigIni = ((IndiceInicial / pot[Partido]) % 3);
			    for (int z = 0; z<3; z++)
				{
					if (z == SigIni) continue;
					int Indice = IndiceInicial + pot[Partido] * (z - SigIni);
					Ap14T[Indice].Aciertos=(byte) (14-Profundidad) ;
						
					if (Profundidad < pProfundidad)
					{
						ColumnasADistancia1 (Indice, Partido + 1, pProfundidad);
					}
				}
			}
		    Profundidad--;
		}
		private void EncontrarDistantes1(double pProb,int IndiceInicial, int PosicionInicial,int pProfundidad)
		{
		    Profundidad++;
        
			//'--encontramos las apuestas que se diferencian en un solo signo ----
			for (int Partido = PosicionInicial;Partido<14;Partido++)
			{
				for (int z = 1;z<3;z++)
				{
					int Indice = IndiceInicial + pot[Partido] * z;
					double Prob = pProb + Cr[Partido, z];
					Ap14T[Indice].Columna = Indice;
					Ap14T[Indice].Probabilidad = Math.Abs (Prob - _LN);//-Prob;
				
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
		    int i = izq; 
			int j = der; 
			ApuestaProbEscrutada x = Ap14T [ (izq + der) /2 ]; 
			
			do
			{ 
			while(Ap14T[i].Probabilidad  < x.Probabilidad  && j <= der)i++; 
			while(x.Probabilidad  < Ap14T[j].Probabilidad  && j > izq )j--;
			
				if( i <= j )
				{ 
					ApuestaProbEscrutada aux = Ap14T[i]; 
					Ap14T[i] = Ap14T[j]; 
					Ap14T[j] = aux; 
					i++;  j--; 
				} 
			}while( i <= j ); 
			if( izq < j ) ordena(izq, j); 
			if( i < der ) ordena(i, der); 
		}
		protected void PonerValoracionPantalla()
		{
			controlPorcentajes1 .Valores =v;
		}

		private void PonerValoracionEnVariables()
		{
			v=controlPorcentajes1 .Valores ;
		}

	    private void HabilitarCalcular()
		{
			btTramificar.Enabled = true;
			if(btTramificar.Enabled ) statusBarPanel4.Text ="Preparado"; else statusBarPanel4.Text ="Faltan datos";
			Application.DoEvents();
		}
		private void textBoxgenerico_KeyPress(object sender, KeyPressEventArgs e)
		{
			Valida.Generic_KeyPress (TipoValidacionString.SoloNumerosDecimales, sender, e);
		}
		private bool DialogoGuardar(ref int c1, ref int c2, ref int Paso, ref int NumColsPorPaso)
		{
			bool res = false;
			DialogoGrabarTramosFrm DialogoGrabarTramos = new DialogoGrabarTramosFrm(c1, c2);
			DialogoGrabarTramos.ShowDialog();
			c1=DialogoGrabarTramos.ColumnaInicial ;
			c2=DialogoGrabarTramos.ColumnaFinal ;
			Paso=DialogoGrabarTramos.Paso ;
			NumColsPorPaso=DialogoGrabarTramos.NumColsPorPaso;

			SaveFileDialog abreFiltroDialog = new SaveFileDialog();
			abreFiltroDialog.InitialDirectory = "Columnas\\" ;
			abreFiltroDialog.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			archivoSalida="";
			if(abreFiltroDialog.ShowDialog () == DialogResult.OK)
			{
			    archivoSalida = abreFiltroDialog.FileName;
				res = true;
			}
			return res;
		}


		private void btCancelar_Click(object sender, System.EventArgs e)
		{
			salida=true;
			this.Close();
		}

	    private void txValMax_TextChanged(object sender, EventArgs e)
		{
			Incremento = Convert.ToInt32 (txValMax.Text)/Convert.ToInt32(cmbNumTrams.Text );
			txIntervalo.Text  =Incremento.ToString();
		}
		private void txColumna_KeyPress(object sender, KeyPressEventArgs e)
		{
			Valida.Generic_KeyPress (TipoValidacionString.Solo1X2, sender, e);
		}
		private void txColumna_TextChanged_1(object sender, EventArgs e)
		{
			escrutado=false;
		}
		private void textBoxgenericoSinDecimales_KeyPress(object sender, KeyPressEventArgs e)
		{
			Valida.Generic_KeyPress (TipoValidacionString.SoloNumeros, sender, e);
		}

	    private void button1_Click(object sender, EventArgs e)
		{
			int c1=0;
			int c2=noColumnasIniciales ;
			bool primerTramo=true;


	        int Paso=1;
			int NumColsPorPaso=1;
			int nr=0;
			NumIntervalos=Tramos.Count;
			
			DataGridCell selectedCell = dgResultados.CurrentCell;
			
			if(!dgResultados.IsSelected (selectedCell.RowNumber))
			{
				MessageBox.Show("No se ha seleccionado ningún tramo de la tabla", "ATENCION!!", MessageBoxButtons.OK  , MessageBoxIcon.Stop , MessageBoxDefaultButton.Button1);
			}
			else
			{
				for (int i=0; i<NumIntervalos ;i++)
				{
					if(dgResultados.IsSelected (i))
					{
						object selectedItem = dgResultados[i, 1];
						if(primerTramo) 
						{
							c1 = Convert.ToInt32(selectedItem);
							primerTramo=false;
						}
						selectedItem = dgResultados[i, 2];
						c2 = Convert.ToInt32(selectedItem)+1;
					}
				}
				if (DialogoGuardar(ref c1, ref c2, ref Paso, ref NumColsPorPaso))
				{
					if (Paso > noColumnasIniciales/NumIntervalos )
					{
						MessageBox.Show ("No se pueden definir saltos mayores al nº de columnas por tramo");
					}
					else
					{
						c=0;
						int colInicial = c1;
						int colFinal = c2;
						ConvertidorDeBases col =new ConvertidorDeBases();
                        IArchivoColumnas comCols = new ArchivoColumnasTexto(archivoSalida);
						for (int i=0; i<NumIntervalos ;i++)
						{
							if(dgResultados.IsSelected (i))
							{
								object selectedItem = dgResultados[i, 1];
								c1 = Convert.ToInt32(selectedItem);
								selectedItem = dgResultados[i, 2];
								c2 = Convert.ToInt32(selectedItem)+1;
								if (c1 < colInicial) c1 =colInicial;
								if (c2 > colFinal) c2 =colFinal;
								if (c1<nr) c1=nr;

								for (nr=c1; nr< c2; nr+=Paso) 
								{
									for(int j=c % NumColsPorPaso; j<NumColsPorPaso; j++)
									{
										if(nr+j>c2) continue;
										if(Bits[Ap14T[nr+j].Columna]) continue;
										comCols.GuardarCols(col.ConvNumAColumna(Ap14T[nr+j].Columna));
										c++;
									}
								}
							}
						}
						comCols.Cerrar();
						statusBarPanel4.Text = "Grabadas " + c  + " columnas";
					}
				}
			}
		}

	    private void TemporadaJornadaTextChanged()
		{
		    JornadaEncontrada=false;
			string NombreFicheroJornadas=Application.StartupPath + "/Jornadas/InfoJornadasLAE.txt";
            IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(NombreFicheroJornadas);
			string Tempo=txTemporada.Text +"/" + txTemporada2.Text;
			while( comBaseCols.SiguienteColumna() )
			{
				ValorsJornada=  comBaseCols.LeeColumnaSinComas().Split ((char) 9);
				if(ValorsJornada[1]==Tempo  && ValorsJornada[2]==numJornada.Value.ToString ().PadLeft (2,'0')) 
				{
					JornadaEncontrada=true;
					txColumna.Text =ValorsJornada[0];
					if(!PremioBloqueado[0] )
					{
						if (ValorsJornada[4]=="0,00") 
						{
							double ParaEl14=PrecioApuesta*Convert.ToDouble (ValorsJornada[3]) *0.12;
							tx14.Text =ParaEl14.ToString();
						}
						else
						{
							tx14.Text =(PrecioApuesta*Convert.ToDouble (ValorsJornada[4])).ToString ();
						}
					}
					txRecaudacion.Text =(PrecioApuesta*Convert.ToDouble (ValorsJornada[3])).ToString ();
					if(!PremioBloqueado[1] )tx13.Text =(PrecioApuesta*Convert.ToDouble (ValorsJornada[5])).ToString ();
					if(!PremioBloqueado[2] )tx12.Text =(PrecioApuesta*Convert.ToDouble (ValorsJornada[6])).ToString ();
					if(!PremioBloqueado[3] )tx11.Text =(PrecioApuesta*Convert.ToDouble (ValorsJornada[7])).ToString ();
					if(!PremioBloqueado[4] )tx10.Text =(PrecioApuesta*Convert.ToDouble (ValorsJornada[8])).ToString ();
					if(FormatoFicheroValoraciones ==44) 
					{
						CargarValoracionJornadaDesdeHistorico(ValorsJornada[1],ValorsJornada[2]);
					}
					break;
				}
			}
			comBaseCols.Cerrar();
			if(!JornadaEncontrada) InicializarValoresJornada();
		}
		private void InicializarValoresJornada()
		{
			if(!PremioBloqueado[0])tx14.Text="5";
			if(!PremioBloqueado[1])tx13.Text="4";
			if(!PremioBloqueado[2])tx12.Text="3";
			if(!PremioBloqueado[3])tx11.Text="2";
			if(!PremioBloqueado[4])tx10.Text="1";
			txRecaudacion.Text = "14000000";
			txColumna.Text = "11111111111111";
			if(FormatoFicheroValoraciones ==44) 
			{
				for(int i=0;i<14;i++)
				{
					v[i,0]=33.33;
					v[i,1]=33.33;
					v[i,2]=33.33;
				}
			}
			PonerValoracionPantalla ();
		}
		private void TemporadaJornada_TextChanged(object sender, EventArgs e)
		{
			int Temporada =Convert.ToInt32 (txTemporada.Text )+1;
			txTemporada2.Text=Temporada.ToString() ;
			TemporadaJornadaTextChanged();
		}

	    private void cmbNumTrams_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (cmbNumTrams.Text !="")
			{
				int Ntrams = Convert.ToInt32(cmbNumTrams.Text);
				if (Ntrams>0)
				{
					Incremento = Convert.ToInt32 (txValMax.Text)/Ntrams;
					txIntervalo.Text  =Incremento.ToString();
					chkAcumular.Checked =false;
				}
			}
		}

		private void menuItem8_Click(object sender, EventArgs e)
		{
			OpenFileDialog abreFiltroDialog = new OpenFileDialog();
			abreFiltroDialog.InitialDirectory = "Columnas\\" ;
			abreFiltroDialog.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			if(abreFiltroDialog.ShowDialog () == DialogResult.OK)
			{		
				Cursor.Current = Cursors.WaitCursor ;
				Bits.SetAll (true);
				escrutado=false;
				mnu14Triples.Checked =false;
				mnuFichero.Checked =true;
				mnuFicheroSobre14T.Checked =false;

				string archivoEntrada = abreFiltroDialog.FileName;	
				CargarFicheroDeColumnas (archivoEntrada);
				Cursor.Current = Cursors.Default;
			}
		}
		private void PonerMaximosiMinimos(bool EnTantoPorCiento)
		{
			if(EnTantoPorCiento)
			{
				txMin14.Text =(Math.Round ((double)MaxiMin [0,1]*100/noColumnasIniciales,3))+"%";
				txMax14.Text =(Math.Round ((double)MaxiMin [0,0]*100/noColumnasIniciales,3))+"%";
				txMin13.Text =(Math.Round ((double)MaxiMin [1,1]*100/noColumnasIniciales,3))+"%";
				txMax13.Text =(Math.Round ((double)MaxiMin [1,0]*100/noColumnasIniciales,3))+"%";
				txMin12.Text =(Math.Round ((double)MaxiMin [2,1]*100/noColumnasIniciales,3))+"%";
				txMax12.Text =(Math.Round ((double)MaxiMin [2,0]*100/noColumnasIniciales,3))+"%";
				txMin11.Text =(Math.Round ((double)MaxiMin [3,1]*100/noColumnasIniciales,3))+"%";
				txMax11.Text =(Math.Round ((double)MaxiMin [3,0]*100/noColumnasIniciales,3))+"%";
				txMin10.Text =(Math.Round ((double)MaxiMin [4,1]*100/noColumnasIniciales,3))+"%";
				txMax10.Text =(Math.Round ((double)MaxiMin [4,0]*100/noColumnasIniciales,3))+"%";

			}
			else
			{
				txMin14.Text =(1+MaxiMin [0,1]).ToString ();
				txMax14.Text =(1+MaxiMin [0,0]).ToString ();
				txMin13.Text =(1+MaxiMin [1,1]).ToString ();
				txMax13.Text =(1+MaxiMin [1,0]).ToString ();
				txMin12.Text =(1+MaxiMin [2,1]).ToString ();
				txMax12.Text =(1+MaxiMin [2,0]).ToString ();
				txMin11.Text =(1+MaxiMin [3,1]).ToString ();
				txMax11.Text =(1+MaxiMin [3,0]).ToString ();
				txMin10.Text =(1+MaxiMin [4,1]).ToString ();
				txMax10.Text =(1+MaxiMin [4,0]).ToString ();
			}
		}
		private void InicializarMaximosiMinimos()
		{
			for(int i=0;i<5;i++)
			{
				MaxiMin [i,0]=0;
				MaxiMin [i,1]=noColumnasIniciales;
			}
			Array.Clear (pctMaxiMin ,0,10);
		}
		private void numericUpDown1_ValueChanged(object sender, EventArgs e)
		{
			ConvertidorDeBases col =new ConvertidorDeBases();
			int Posicio= (int) numericUpDown1.Value-1;
			if (Posicio >= 0 && Posicio<noColumnasIniciales)
			{
				TxColumnaEnPosicion .Text = col.ConvNumAColumna(Ap14T[Posicio].Columna) ;
				txProbabilidad.Text =(-Ap14T[Posicio].Probabilidad).ToString () ;
				if(Bits[Ap14T[Posicio].Columna])
				{
					TxColumnaEnPosicion.ForeColor = Color.Red;
				}
				else
				{
					TxColumnaEnPosicion.ForeColor = Color.Black;
				}
			}
		}

		private void btAnterior_Click(object sender, System.EventArgs e)
		{
			int Posicio= (int) numericUpDown1.Value-2;
			for (int i=Posicio; i>0;i--)
			{
				if(Bits[Ap14T[i].Columna]) continue;
				if (Ap14T[i].Aciertos>=NumAciertosABuscar)
				{
					numericUpDown1.Value=i+1;
					break;
				}
			}
		}

		private void btSiguiente_Click(object sender, System.EventArgs e)
		{
			int Posicio= (int) numericUpDown1.Value;
			for (int i=Posicio; i<noColumnasIniciales;i++)
			{
				if(Bits[Ap14T[i].Columna]) continue;
				if (Ap14T[i].Aciertos>=NumAciertosABuscar)
				{
					numericUpDown1.Value=i+1;
					break;
				}
			}
		}

		private void menuItem7_Click(object sender, System.EventArgs e)
		{
			//---Guardar datos del L.A.E. de la Jornada ------
			Jornadas =new ArrayList();
			
			string JornadaAGuardar=numJornada.Value.ToString().PadLeft (2,'0');
			string TemporadaDeLaJornadaAGuardar=txTemporada.Text+"/"+ txTemporada2.Text ;

			
			bool JornadaYaExiste =false;

		    string NombreFicheroJornadas=Application.StartupPath + "/Jornadas/InfoJornadasLAE.txt";
            IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(NombreFicheroJornadas);
			StringBuilder linea= new StringBuilder ("");
			while( comBaseCols.SiguienteColumna() )
			{
				linea.Remove (0,linea.Length );
				linea.Append (comBaseCols.LeeColumnaSinComas());
				ValorsJornada=  linea.ToString ().Split ((char) 9);
				
				if(ValorsJornada[1]==TemporadaDeLaJornadaAGuardar  && ValorsJornada[2]==JornadaAGuardar) 
				{
					JornadaYaExiste=true;
					linea.Remove (0,linea.Length );
					linea.Append ( MontaLinea(TemporadaDeLaJornadaAGuardar, JornadaAGuardar,txColumna.Text));
				}
				
				Jornadas.Add (linea.ToString());
			
			}
			if(!JornadaYaExiste)
			{
				linea.Remove (0,linea.Length );
				linea.Append ( MontaLinea(TemporadaDeLaJornadaAGuardar, JornadaAGuardar,txColumna.Text));
				Jornadas.Add (linea.ToString());
			}
			comBaseCols.Cerrar();
            IArchivoColumnas comCols = new ArchivoColumnasTexto(NombreFicheroJornadas);
			foreach(string str in Jornadas)
			{
				comCols.GuardarColsComa(str);
			}
			comCols.Cerrar();	
		}
		private string MontaLinea(string TemporadaDeLaJornadaAGuardar,string JornadaAGuardar,string Columna)
		{
			StringBuilder linea= new StringBuilder (Columna);
			char sep=(char)9;
			if (txRecaudacion.Text=="") txRecaudacion.Text="14000000";
			double Recaudacion =Convert.ToDouble (txRecaudacion.Text)/PrecioApuesta;
			double ParaEl14=Convert.ToDouble (tx14.Text)/PrecioApuesta;
			double ParaEl13=Convert.ToDouble (tx13.Text)/PrecioApuesta;
			double ParaEl12=Convert.ToDouble (tx12.Text)/PrecioApuesta;
			double ParaEl11=Convert.ToDouble (tx11.Text)/PrecioApuesta;
			double ParaEl10=Convert.ToDouble (tx10.Text)/PrecioApuesta;
					
			linea.Remove (0,linea.Length);
			linea.Append  (txColumna.Text);
			linea.Append( sep);
			linea.Append( TemporadaDeLaJornadaAGuardar);
			linea.Append( sep);
			linea.Append( JornadaAGuardar);
			linea.Append( sep);
			linea.Append( Recaudacion.ToString ().Replace (".",","));
			linea.Append( sep);
			linea.Append( ParaEl14.ToString ().Replace (".",","));
			linea.Append( sep);
			linea.Append( ParaEl13.ToString ().Replace (".",","));
			linea.Append( sep);
			linea.Append( ParaEl12.ToString ().Replace (".",","));
			linea.Append( sep);
			linea.Append( ParaEl11.ToString ().Replace (".",","));
			linea.Append( sep);
			linea.Append( ParaEl10.ToString ().Replace (".",","));
			return linea.ToString ();
		}

		private void btFiltrar_Click(object sender, System.EventArgs e)
		{
		    int c=0;


			Bits.SetAll (true);
			for(int x=0;x < noColumnasIniciales ;x++){Bits[Ap14T[x].Columna]=false;}
			int[,] extremos = ObtenerExtremos ();

			DialogoFiltrarPorLimitesFrm DialogoFiltrar;
			DialogoFiltrar = new DialogoFiltrarPorLimitesFrm(extremos);
			DialogoFiltrar.ShowDialog() ;
			if (DialogoFiltrar.ValoresAceptados)
			{
				//eliminamos columnas por diferencias

				Cursor.Current = Cursors.WaitCursor ;
				statusBarPanel4.Text ="Eliminando columnas ...";
				Application.DoEvents();

				extremos=DialogoFiltrar.extremos;

				for (int i=0;i<10;i++)
				{
					if(extremos[i,0] > 0) extremos[i,0]--;
					for(int x=extremos[i,0];x < extremos[i,1];x++)
					{
						if(extremos[i,2]==0) Bits[Ap14T [x].Columna]=true;
						if(extremos[i,3]>0) EliminarColumnas(Ap14T [x].Columna , 0, (short) extremos[i,3]);
					}
				}
			
				for (int nr=0; nr< 4782969; nr++) {if(Bits [nr]==false) c++;}

				btGrabarFiltro.Enabled =true;
				lbColumnasAGrabar.Text =c.ToString ();
				statusBarPanel4.Text ="Finalizado";
				Cursor.Current = Cursors.Default;
			}
		}
		private int[,] ObtenerExtremos ()
		{
			int [,] extremos =new int [10,4];
			extremos [0,0]=0;
			extremos [0,1]=Convert.ToInt32 (txMin10.Text)-1;

			extremos [1,0]=Convert.ToInt32 (txMin10.Text)-1;
			extremos [1,1]=Convert.ToInt32 (txMin11.Text)-1;
			
			extremos [2,0]=Convert.ToInt32 (txMin11.Text)-1;
			extremos [2,1]=Convert.ToInt32 (txMin12.Text)-1;

			extremos [3,0]=Convert.ToInt32 (txMin12.Text)-1;
			extremos [3,1]=Convert.ToInt32 (txMin13.Text)-1;

			extremos [4,0]=Convert.ToInt32 (txMin13.Text)-1;
			extremos [4,1]=Convert.ToInt32 (txMin14.Text)-1;

			extremos [5,0]=Convert.ToInt32 (txMax14.Text);
			extremos [5,1]=Convert.ToInt32 (txMax13.Text);

			extremos [6,0]=Convert.ToInt32 (txMax13.Text);
			extremos [6,1]=Convert.ToInt32 (txMax12.Text);

			extremos [7,0]=Convert.ToInt32 (txMax12.Text);
			extremos [7,1]=Convert.ToInt32 (txMax11.Text);

			extremos [8,0]=Convert.ToInt32 (txMax11.Text);
			extremos [8,1]=Convert.ToInt32 (txMax10.Text);

			extremos [9,0]=Convert.ToInt32 (txMax10.Text);
			extremos [9,1]=noColumnasIniciales-1;
			return extremos;
		}
		private void EliminarColumnas(int IndiceInicial, short PosicionInicial, short pProfundidad)
		{
		    //--encontramos las apuestas que se diferencian en un solo signo ----
			Profundidad++;
			for (short Partido = PosicionInicial; Partido<14; Partido++)
			{
			    int SigIni = ((IndiceInicial / pot[Partido]) % 3);
			    for (short z = 0; z<3; z++)
				{
					if (z == SigIni) continue;
					int Indice = IndiceInicial + pot[Partido] * (z - SigIni);
					Bits [Indice]=true; //las marcadas a true son las que eliminamos
					if (Profundidad < pProfundidad)
					{EliminarColumnas ( Indice, (short)(Partido + 1), pProfundidad);}
				}
			}
		    Profundidad--;
		}

		private void button1_Click_1(object sender, System.EventArgs e)
		{
			//
			// Grabamos las columnas filtradas en un fichero
			//
			int c1=0;
			int c2=Convert.ToInt32 (lbColumnasAGrabar.Text);
			int Paso=1;
			int NumColsPorPaso=1;

			DialogoGuardar(ref c1, ref c2, ref Paso, ref NumColsPorPaso);
			if(archivoSalida != "")
			{
				int c=0;
				Cursor.Current = Cursors.WaitCursor ;
				statusBarPanel4.Text ="Guardando columnas ...";
				Application.DoEvents();
				ConvertidorDeBases col =new ConvertidorDeBases();
                IArchivoColumnas comCols = new ArchivoColumnasTexto(archivoSalida);
				for (int nr=0; nr< 4782969; nr++) 
				{
					if(Bits [nr]==false)
					{
						if (c % Paso != 0) continue;
						comCols.GuardarCols(col.ConvNumAColumna(nr));
						c++;
					}
				}
				comCols.Cerrar();	
				statusBarPanel4.Text = "Grabadas " + c  + " columnas";
				Cursor.Current = Cursors.Default;
			}
		}

		private void mnu14Triples_Click(object sender, EventArgs e)
		{
			Habilita14Triples();
		}

		private void Habilita14Triples()
		{
			Bits.SetAll (false);
			mnu14Triples.Checked =true;
			mnuFichero.Checked =false;
			mnuFicheroSobre14T.Checked =false;
			statusBarPanel3.Text ="";
			escrutado=false;
			txValMin.Enabled =true;
			txValMax.Enabled =true;
			txValMax.Text ="4782969";
			noColumnasIniciales=4782969;
			numericUpDown1.Maximum =noColumnasIniciales;
		}
		private void button1_Click_2(object sender, EventArgs e)
		{
			ConvertidorDeBases col =new ConvertidorDeBases();
			if (TxColumnaEnPosicion.Text !="")
			{
				int Num =col.ConvColumnaANumero(TxColumnaEnPosicion.Text) ;
				for (int i=0;i<noColumnasIniciales ;i++)
				{
					if (Ap14T[i].Columna ==Num)
					{
						numericUpDown1.Value  =i+1;
						break;
					}
				}

			}
		}

		private void menuItem2_Click(object sender, EventArgs e)
		{
			System.Diagnostics.Process.Start("IExplore"," http://onlae.terra.es/qsorteos.asp");
		}

		private void menuItem5_Click(object sender, EventArgs e)
		{
			Close();
		}

	    private void TxColumnaEnPosicion_TextChanged(object sender, EventArgs e)
		{
			int aciertos=0;
			for(int i=0;i<TxColumnaEnPosicion.Text.Length ;i++)
			{
				if(txColumna.Text [i]==TxColumnaEnPosicion.Text[i]) aciertos++;
			}
			txAciertos.Text = aciertos.ToString ();
		}

	    private void AfegirAlHistoric()
		{
	        HistoriaValoracionesFrm HistoriaValFrm = new HistoriaValoracionesFrm(v,Convert.ToInt32 (txTemporada.Text) ,(int) numJornada.Value,ArchivoHistoricoDeValoraciones);
			HistoriaValFrm.ShowDialog() ;
			ArchivoHistoricoDeValoraciones=HistoriaValFrm.archivoSalida ;
		}
		private void CargarValoracionJornadaDesdeHistorico(string tempo, string jorna)
		{
			controlPorcentajes1.archivoPorcentajes = ArchivoHistoricoDeValoraciones;
			controlPorcentajes1.Jornada =jorna;
			controlPorcentajes1.Temporada =tempo;
			controlPorcentajes1.Refresca();
			v=controlPorcentajes1.Valores;
			statusBarPanel2.Text = Path.GetFileNameWithoutExtension (ArchivoHistoricoDeValoraciones) + " " + tempo + " J-" + jorna;
			Application.DoEvents();
			HabilitarCalcular ();
		}

	    private void CargarFicheroDeColumnas(string archivoEntrada)
		{
			statusBarPanel3.Text = Path.GetFileName(archivoEntrada);
			Application.DoEvents ();
	        IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(archivoEntrada);
			noColumnasIniciales = Convert.ToInt32(comBaseCols.ObtenNumCols());
			ConvertidorDeBases conv=new ConvertidorDeBases();
			while(comBaseCols.SiguienteColumna())
			{
			    int Num = conv.ConvColumnaANumero(comBaseCols.LeeColumnaSinComas());
			    Bits[Num]=false;
			}
	        comBaseCols.Cerrar();
			txValMin.Text ="0";
			txValMax.Text=noColumnasIniciales.ToString ();
			txValMin.Enabled =false;
			txValMax.Enabled =false;
			numericUpDown1.Maximum =noColumnasIniciales;
			if(!mnuFichero.Checked  )
			{
				txValMax.Text ="4782969";
				noColumnasIniciales=4782969;
				numericUpDown1.Maximum =noColumnasIniciales;
			}
		}

		private void menuItem13_Click(object sender, EventArgs e)
		{
			string Archivo="";
			if (FormatoFicheroValoraciones ==44) Archivo= archivoPorcentajes ;
			DialogoAnalisiMultiple = new DialogoAnalisisMultipleDeTramosFrm(Archivo);
			DialogoAnalisiMultiple.ShowDialog();	
			archivoPorcentajes =DialogoAnalisiMultiple.FicheroValoracionesHistoricas ;
			ArchivoHistoricoDeValoraciones=DialogoAnalisiMultiple.FicheroValoracionesHistoricas ;
			TemporadaJornadaTextChanged();
			mnuJornadasMultiple.Checked =true;
			mnuJornadaSimple.Checked =false;

			if (archivoPorcentajes !="")
			{
				btTramificar.Enabled =true;
				FormatoFicheroValoraciones=44;
			}
		}

		private void mnuJornadaSimple_Click(object sender, EventArgs e)
		{
			mnuJornadasMultiple.Checked =false;
			mnuJornadaSimple.Checked =true;
		}
		private void mnuPorSumas_Click_1(object sender, EventArgs e)
		{
		    mnuPorProductos.Checked =false;
			mnuPorSumas.Checked =true;
			lbLNCentral.Text ="Valor central";
			lblValoracion.Text ="Valor";
		}
		private void mnuPorProductos_Click_1(object sender, EventArgs e)
		{
		    mnuPorProductos.Checked =true;
			mnuPorSumas.Checked =false;
			lbLNCentral.Text ="LN central";
			lblValoracion.Text ="LN";
		}
		private void mnuGuardarEnHistorico_Click(object sender, EventArgs e)
		{
			AfegirAlHistoric();
		}
		private void btLeerLimites_Click(object sender, EventArgs e)
		{
			OpenFileDialog abreFiltroDialog = new OpenFileDialog();
			abreFiltroDialog.InitialDirectory = "Columnas\\" ;
			abreFiltroDialog.Filter = "Limites(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;

			if(abreFiltroDialog.ShowDialog() == DialogResult.OK) 
			{	
				StreamReader sw = new StreamReader(abreFiltroDialog.FileName);
	
				txMin14.Text =sw.ReadLine();
				txMax14.Text =sw.ReadLine();
				txMin13.Text =sw.ReadLine();
				txMax13.Text =sw.ReadLine();
				txMin12.Text =sw.ReadLine();
				txMax12.Text =sw.ReadLine();
				txMin11.Text =sw.ReadLine();
				txMax11.Text =sw.ReadLine();
				txMin10.Text =sw.ReadLine();
				txMax10.Text =sw.ReadLine();
				sw.Close ();
			}
		}

		private void btGuardarLimites_Click(object sender, EventArgs e)
		{
			SaveFileDialog abreFiltroDialog = new SaveFileDialog();
			abreFiltroDialog.InitialDirectory = "Columnas\\" ;
			abreFiltroDialog.Filter = "Limites(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;

			if(abreFiltroDialog.ShowDialog() == DialogResult.OK) 
			{
			    StreamWriter sw = File.CreateText( abreFiltroDialog.FileName );				

				sw.WriteLine(txMin14.Text);
				sw.WriteLine(txMax14.Text);
				sw.WriteLine(txMin13.Text);
				sw.WriteLine(txMax13.Text);
				sw.WriteLine(txMin12.Text);
				sw.WriteLine(txMax12.Text);
				sw.WriteLine(txMin11.Text);
				sw.WriteLine(txMax11.Text);
				sw.WriteLine(txMin10.Text);
				sw.WriteLine(txMax10.Text);
				sw.Close ();
			}
		}

		private void btUndo_Click(object sender, EventArgs e)
		{
			PonerMaximosiMinimos(false);
			btUndo.Visible =false;
			btVerPorcentaje.Visible =true;
		}

		private void btVerPorcentaje_Click(object sender, EventArgs e)
		{
			PonerMaximosiMinimos(true);
			btUndo.Visible =true;
			btVerPorcentaje.Visible =false;

		}

		private void btTemporadaAnterior_Click(object sender, EventArgs e)
		{
			txTemporada2.Text=txTemporada.Text ;
			int Temporada =Convert.ToInt32 (txTemporada.Text )-1;
			txTemporada.Text=Temporada.ToString();
		}

		private void btTemporadaSiguiente_Click(object sender, EventArgs e)
		{
			int Temporada =Convert.ToInt32 (txTemporada.Text )+1;
			txTemporada2.Text=(Temporada+1).ToString() ;
			txTemporada.Text=Temporada.ToString();
		}

		private void btCopiar_Click(object sender, EventArgs e)
		{
			string cadena=txMin14.Text ;
			char sep = (char) 9;
			char LF = (char) 10;
			char NL = (char) 13;

			cadena = cadena + sep + txMin13.Text ;
			cadena = cadena + sep + txMin12.Text ;
			cadena = cadena + sep + txMin11.Text ;
			cadena = cadena + sep + txMin10.Text ;
			cadena = cadena + NL+LF + txMax14.Text;
			cadena = cadena + sep + txMax13.Text;
			cadena = cadena + sep + txMax12.Text;
			cadena = cadena + sep + txMax11.Text;
			cadena = cadena + sep + txMax10.Text;
			Clipboard.SetDataObject (cadena, true);
		}

		private void btPegar_Click(object sender, EventArgs e)
		{
			char sep = (char) 9;
			char LF = (char) 10;
			char NL = (char) 13;
			string cadena ="";

			// Declares an IDataObject to hold the data returned from the clipboard.
			// Retrieves the data from the clipboard.
			IDataObject iData = Clipboard.GetDataObject();
 
			// Determines whether the data is in a format you can use.
			if(iData.GetDataPresent(DataFormats.Text)) 
			{
				// Yes it is, so display it in a text box.
				cadena = (String)iData.GetData(DataFormats.Text); 
			}



			cadena=cadena.Replace (LF,' ');
			string[] linea= cadena.Split (NL);
			string[] valorsMin =linea[0].Split (sep);
			string[] valorsMax =linea[1].Split (sep);
			txMin14.Text =valorsMin[0];
			txMin13.Text =valorsMin[1];
			txMin12.Text =valorsMin[2];
			txMin11.Text =valorsMin[3];
			txMin10.Text =valorsMin[4];

			txMax14.Text =valorsMax[0];
			txMax13.Text =valorsMax[1];
			txMax12.Text =valorsMax[2];
			txMax11.Text =valorsMax[3];
			txMax10.Text =valorsMax[4];

		}

	    private void genericochkey_CheckStateChanged(object sender, EventArgs e)
		{
			CheckBox MiCheckBox =(CheckBox)sender;
			PremioBloqueado[Convert.ToInt32 (MiCheckBox.Tag)]=MiCheckBox.Checked ;
		}

		private void txLNCentral_TextChanged(object sender, EventArgs e)
		{
			if (txLNCentral.Text !="-")_LN =Convert.ToDouble(txLNCentral.Text);
		}

		private void cmbAciertosABuscar_SelectedIndexChanged(object sender, EventArgs e)
		{
			NumAciertosABuscar=Convert.ToInt16 (cmbAciertosABuscar.Text) ;
		}

		private void mnuFicheroSobre14T_Click(object sender, EventArgs e)
		{
			OpenFileDialog abreFiltroDialog = new OpenFileDialog();
			abreFiltroDialog.InitialDirectory = "Columnas\\" ;
			abreFiltroDialog.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			if(abreFiltroDialog.ShowDialog () == DialogResult.OK)
			{		
				Cursor.Current = Cursors.WaitCursor ;
                Bits.SetAll (true);
				escrutado=false;
				mnu14Triples.Checked =false;
				mnuFichero.Checked =false;
				mnuFicheroSobre14T.Checked =true;
				statusBarPanel3.Text ="";
				string archivoEntrada = abreFiltroDialog.FileName;	
				CargarFicheroDeColumnas (archivoEntrada);
				txValMin.Enabled =true;
				txValMax.Enabled =true;
				Cursor.Current = Cursors.Default;
			}
		}

		private void menuItem10_Click_1(object sender, EventArgs e)
		{
			if(Tramos != null)
			{
				TramificarGraficasFrm Grafica = new TramificarGraficasFrm(Tramos);
				Grafica.ShowDialog();
			}
		}

		private void controlPorcentajes1_Modificado(object sender, EventArgs e)
		{
			v=controlPorcentajes1 .Valores ;
			FormatoFicheroValoraciones=controlPorcentajes1.FormatoFicheroValoraciones  ;
			archivoPorcentajes = controlPorcentajes1.archivoPorcentajes ;
			switch (FormatoFicheroValoraciones )
			{
				case 1:
				case 3:
				case 42:
					mnuJornadasMultiple.Checked=false;
					mnuJornadaSimple.Checked =true;
					break;
				case 43:
					mnuJornadasMultiple.Checked=false;
					mnuJornadaSimple.Checked =true;
					break;
				case 44:
					ArchivoHistoricoDeValoraciones=archivoPorcentajes;
					TemporadaJornadaTextChanged();
					break;
			}
			escrutado=false;
			statusBarPanel2.Text = Path.GetFileNameWithoutExtension (controlPorcentajes1.archivoPorcentajes);
			Application.DoEvents();
			HabilitarCalcular ();
		}

		private void cambiarBoton(Button boton)
		{
            FormulariosHelper f = new FormulariosHelper();
			f.CambiarFondoBoton(boton);
		}

		private void btFiltrar_EnabledChanged(object sender, EventArgs e)
		{
			cambiarBoton(btFiltrar);
		}

		private void btGrabarFiltro_EnabledChanged(object sender, EventArgs e)
		{
			cambiarBoton(btGrabarFiltro);
		}

		private void btGrabarTramos_EnabledChanged(object sender, EventArgs e)
		{
			cambiarBoton(btGrabarTramos);
		}

		private void btTramificar_EnabledChanged(object sender, EventArgs e)
		{
			cambiarBoton(btTramificar);
		}

		private void btPrAcum_Click(object sender, EventArgs e)
		{
			if (dgResultadosPrAcum.Visible==false)
			{
				dgResultadosPrAcum.Visible =true;
				dgResultados.Visible =false;
				GridDataBindPrAcum();
				btPrAcum.Text ="Tramos";
			}
			else
			{
				dgResultadosPrAcum.Visible =false;
				dgResultados.Visible =true;
				GridDataBind ();
				btPrAcum.Text ="% PrAcum. 14";
			}
		}
	}
}



