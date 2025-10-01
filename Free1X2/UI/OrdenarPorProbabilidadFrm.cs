using System;
using System.Text;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using Free1X2.Utils ;
using Free1X2.Analisis;
using Free1X2.EntradaSalida;

namespace Free1X2.UI
{
	/// <summary>
	/// Summary description for OrdenarPorProbabilidadFrm.
	/// </summary>
	public class OrdenarPorProbabilidadFrm : System.Windows.Forms.Form
	{
        protected bool EstaInicializandoDatos { get; set; }
        private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.RadioButton rb14Triples;
		private System.Windows.Forms.RadioButton rbFichero;
		private System.Windows.Forms.TextBox TxFicheroEntrada;
		private System.Windows.Forms.Button btCalcular;
		private System.Windows.Forms.GroupBox groupSalida;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.TextBox TxFicheroSalida;
		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.StatusBarPanel statusBarPanel1;
		/// <summary>
		/// Required designer variable.
		private System.ComponentModel.Container components = null;
		private string archivoSalida="";
		private string archivoEntrada="";
		private float[,] p = new float [14,3];
		private float[,] Cr = new float [14,3];
		private double[,] v = new double [14,3];
		private double Recaudacion;
		private double PrecioApuesta;
		private double PorcentajeDestinadoAlPremiode14;
        private string moneda;
		private Porcentajes Pct =new Porcentajes ();
		private double _LN;
		private double _Probabilidad;
		private double _Premio;
		private double _Acertantes;
		private int Profundidad=0;
		private int NumApuestas;
		private int MaxColumnas =4782969 ;
		private float LimiteProbabilidadAcumulada;
		private float ProbabilidadAcumulada;
		private int c=0;
		private bool CalculoMultiple=false;
		private string numIter="";
		private ValidadorCadenas Valida= new ValidadorCadenas();
		private int[] pot = new int [] {1,3,9,27,81,243,729,2187,6561,19683,59049,177147,531441,1594323};
		private ApuestaProbableCentral[] Ap14T=new ApuestaProbableCentral[4782969] ;
		private BitArray Bits = new BitArray(4782969,false);
		private BitArray Admitidas = new BitArray(4782969,false);
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.GroupBox grLAE;
		private System.Windows.Forms.TextBox textBox5;
		private System.Windows.Forms.TextBox textBox4;
		private System.Windows.Forms.TextBox textBox3;
		private System.Windows.Forms.Label lblPctAl14;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Label lblPrecioApuesta;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label lblRecaudacion;
		private System.Windows.Forms.TextBox txRecaudacion;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox TxAcertantes;
		private System.Windows.Forms.RadioButton rbAcertantes;
		private System.Windows.Forms.TextBox TxPremio;
		private System.Windows.Forms.RadioButton rbPremio;
		private System.Windows.Forms.TextBox TxProbabilidad;
		private System.Windows.Forms.TextBox TxLN;
		private System.Windows.Forms.RadioButton rbProbabilidad;
		private System.Windows.Forms.RadioButton rbLN;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.StatusBarPanel statusBarPanel2;
		private System.Windows.Forms.StatusBarPanel statusBarPanel3;
		private System.Windows.Forms.StatusBarPanel statusBarPanel4;
		private System.Windows.Forms.TextBox txMaxColumnas;
		private System.Windows.Forms.Label lblMaxCol;
		private System.Windows.Forms.CheckBox checkValorOrdenacion;
		private System.Windows.Forms.TabPage Productos;
		private System.Windows.Forms.TabPage Sumas;
		private System.Windows.Forms.TextBox txtLimiteProbAcum;
		private System.Windows.Forms.Label label31;
		private System.Windows.Forms.CheckBox chkValorPremio14;
		private System.Windows.Forms.TextBox txLNminimo;
		private System.Windows.Forms.TextBox txLNmaximo;
		private System.Windows.Forms.TextBox txNumPuntos;
		private System.Windows.Forms.Label label34;
		private System.Windows.Forms.TextBox txNumColumnasPorTramo;
		private System.Windows.Forms.Label label35;
		private System.Windows.Forms.Label label36;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.RadioButton rbPorSumas;
		private System.Windows.Forms.RadioButton rbPorProbabilidad;
		private System.Windows.Forms.Label lblLNMax;
		private System.Windows.Forms.Label lblLNMin;
		private System.Windows.Forms.TabPage Multiple;
		private System.Windows.Forms.RadioButton rbColumna;
		private System.Windows.Forms.TextBox txColumna;
		private Free1X2.UI.Controls.ControlPorcentajes controlPorcentajes1;
		private System.Windows.Forms.TextBox TxValorCentralSumas;
		private bool salidaBinaria=false;
				
		/// </summary>

		public OrdenarPorProbabilidadFrm()
		{
			InitializeComponent();

            //este flag se usa para ignorar los eventos de 
            EstaInicializandoDatos = true;
            AConfiguracion ac = new AConfiguracion(Application.StartupPath);
			ac.ObtenValoresLAE(ref PrecioApuesta, ref PorcentajeDestinadoAlPremiode14, ref Recaudacion, ref moneda);
			TxAcertantes.Text ="1,5";
			txRecaudacion.Text  =Recaudacion.ToString ();
			this.textBox1.Text =PrecioApuesta.ToString ();
			this.textBox2.Text =PorcentajeDestinadoAlPremiode14.ToString ();
            EstaInicializandoDatos = false;

			statusBar1.ShowPanels =true;
			statusBarPanel4.Text ="Faltan datos";
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrdenarPorProbabilidadFrm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button1 = new System.Windows.Forms.Button();
            this.TxFicheroEntrada = new System.Windows.Forms.TextBox();
            this.rbFichero = new System.Windows.Forms.RadioButton();
            this.rb14Triples = new System.Windows.Forms.RadioButton();
            this.btCalcular = new System.Windows.Forms.Button();
            this.groupSalida = new System.Windows.Forms.GroupBox();
            this.chkValorPremio14 = new System.Windows.Forms.CheckBox();
            this.txtLimiteProbAcum = new System.Windows.Forms.TextBox();
            this.label31 = new System.Windows.Forms.Label();
            this.txMaxColumnas = new System.Windows.Forms.TextBox();
            this.lblMaxCol = new System.Windows.Forms.Label();
            this.checkValorOrdenacion = new System.Windows.Forms.CheckBox();
            this.button2 = new System.Windows.Forms.Button();
            this.TxFicheroSalida = new System.Windows.Forms.TextBox();
            this.statusBar1 = new System.Windows.Forms.StatusBar();
            this.statusBarPanel1 = new System.Windows.Forms.StatusBarPanel();
            this.statusBarPanel2 = new System.Windows.Forms.StatusBarPanel();
            this.statusBarPanel3 = new System.Windows.Forms.StatusBarPanel();
            this.statusBarPanel4 = new System.Windows.Forms.StatusBarPanel();
            this.button3 = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.Productos = new System.Windows.Forms.TabPage();
            this.grLAE = new System.Windows.Forms.GroupBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.lblPctAl14 = new System.Windows.Forms.Label();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.lblPrecioApuesta = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.lblRecaudacion = new System.Windows.Forms.Label();
            this.txRecaudacion = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txColumna = new System.Windows.Forms.TextBox();
            this.rbColumna = new System.Windows.Forms.RadioButton();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.TxAcertantes = new System.Windows.Forms.TextBox();
            this.rbAcertantes = new System.Windows.Forms.RadioButton();
            this.TxPremio = new System.Windows.Forms.TextBox();
            this.rbPremio = new System.Windows.Forms.RadioButton();
            this.TxProbabilidad = new System.Windows.Forms.TextBox();
            this.TxLN = new System.Windows.Forms.TextBox();
            this.rbProbabilidad = new System.Windows.Forms.RadioButton();
            this.rbLN = new System.Windows.Forms.RadioButton();
            this.Sumas = new System.Windows.Forms.TabPage();
            this.TxValorCentralSumas = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.Multiple = new System.Windows.Forms.TabPage();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.rbPorSumas = new System.Windows.Forms.RadioButton();
            this.rbPorProbabilidad = new System.Windows.Forms.RadioButton();
            this.label36 = new System.Windows.Forms.Label();
            this.txNumColumnasPorTramo = new System.Windows.Forms.TextBox();
            this.label35 = new System.Windows.Forms.Label();
            this.txNumPuntos = new System.Windows.Forms.TextBox();
            this.label34 = new System.Windows.Forms.Label();
            this.txLNmaximo = new System.Windows.Forms.TextBox();
            this.txLNminimo = new System.Windows.Forms.TextBox();
            this.lblLNMax = new System.Windows.Forms.Label();
            this.lblLNMin = new System.Windows.Forms.Label();
            this.controlPorcentajes1 = new Free1X2.UI.Controls.ControlPorcentajes();
            this.groupBox1.SuspendLayout();
            this.groupSalida.SuspendLayout();
            // StatusBarPanel controls don't support ISupportInitialize in .NET 8
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).BeginInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel2)).BeginInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel3)).BeginInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel4)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.Productos.SuspendLayout();
            this.grLAE.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.Sumas.SuspendLayout();
            this.Multiple.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.BackColor = System.Drawing.Color.Bisque;
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.TxFicheroEntrada);
            this.groupBox1.Controls.Add(this.rbFichero);
            this.groupBox1.Controls.Add(this.rb14Triples);
            this.groupBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox1.Location = new System.Drawing.Point(184, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(539, 80);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Origen de las columnas a ordenar";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.BackColor = System.Drawing.Color.LightSalmon;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.Black;
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.Location = new System.Drawing.Point(504, 48);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(24, 21);
            this.button1.TabIndex = 3;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // TxFicheroEntrada
            // 
            this.TxFicheroEntrada.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TxFicheroEntrada.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.TxFicheroEntrada.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TxFicheroEntrada.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxFicheroEntrada.ForeColor = System.Drawing.Color.Black;
            this.TxFicheroEntrada.Location = new System.Drawing.Point(80, 48);
            this.TxFicheroEntrada.Name = "TxFicheroEntrada";
            this.TxFicheroEntrada.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.TxFicheroEntrada.Size = new System.Drawing.Size(423, 21);
            this.TxFicheroEntrada.TabIndex = 2;
            this.TxFicheroEntrada.Text = "(falta selecci�n)";
            this.TxFicheroEntrada.Visible = false;
            this.TxFicheroEntrada.TextChanged += new System.EventHandler(this.TxFicheroEntrada_TextChanged);
            // 
            // rbFichero
            // 
            this.rbFichero.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbFichero.ForeColor = System.Drawing.Color.Black;
            this.rbFichero.Location = new System.Drawing.Point(16, 48);
            this.rbFichero.Name = "rbFichero";
            this.rbFichero.Size = new System.Drawing.Size(68, 24);
            this.rbFichero.TabIndex = 1;
            this.rbFichero.Text = "Fichero";
            this.rbFichero.CheckedChanged += new System.EventHandler(this.rbFichero_CheckedChanged);
            // 
            // rb14Triples
            // 
            this.rb14Triples.Checked = true;
            this.rb14Triples.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rb14Triples.ForeColor = System.Drawing.Color.Black;
            this.rb14Triples.Location = new System.Drawing.Point(16, 18);
            this.rb14Triples.Name = "rb14Triples";
            this.rb14Triples.Size = new System.Drawing.Size(121, 24);
            this.rb14Triples.TabIndex = 0;
            this.rb14Triples.TabStop = true;
            this.rb14Triples.Text = "14 Triples";
            this.rb14Triples.CheckedChanged += new System.EventHandler(this.rb14Triples_CheckedChanged);
            // 
            // btCalcular
            // 
            this.btCalcular.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btCalcular.BackColor = System.Drawing.Color.DarkSalmon;
            this.btCalcular.Enabled = false;
            this.btCalcular.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btCalcular.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btCalcular.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btCalcular.Image = ((System.Drawing.Image)(resources.GetObject("btCalcular.Image")));
            this.btCalcular.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btCalcular.Location = new System.Drawing.Point(623, 416);
            this.btCalcular.Name = "btCalcular";
            this.btCalcular.Size = new System.Drawing.Size(100, 32);
            this.btCalcular.TabIndex = 4;
            this.btCalcular.Text = "&Ordenar";
            this.btCalcular.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btCalcular.UseVisualStyleBackColor = false;
            this.btCalcular.Click += new System.EventHandler(this.btCalcular_Click);
            // 
            // groupSalida
            // 
            this.groupSalida.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupSalida.BackColor = System.Drawing.Color.Bisque;
            this.groupSalida.Controls.Add(this.chkValorPremio14);
            this.groupSalida.Controls.Add(this.txtLimiteProbAcum);
            this.groupSalida.Controls.Add(this.label31);
            this.groupSalida.Controls.Add(this.txMaxColumnas);
            this.groupSalida.Controls.Add(this.lblMaxCol);
            this.groupSalida.Controls.Add(this.checkValorOrdenacion);
            this.groupSalida.Controls.Add(this.button2);
            this.groupSalida.Controls.Add(this.TxFicheroSalida);
            this.groupSalida.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupSalida.ForeColor = System.Drawing.Color.Maroon;
            this.groupSalida.Location = new System.Drawing.Point(184, 92);
            this.groupSalida.Name = "groupSalida";
            this.groupSalida.Size = new System.Drawing.Size(539, 108);
            this.groupSalida.TabIndex = 5;
            this.groupSalida.TabStop = false;
            this.groupSalida.Text = "Fichero de salida";
            // 
            // chkValorPremio14
            // 
            this.chkValorPremio14.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkValorPremio14.ForeColor = System.Drawing.Color.Black;
            this.chkValorPremio14.Location = new System.Drawing.Point(280, 72);
            this.chkValorPremio14.Name = "chkValorPremio14";
            this.chkValorPremio14.Size = new System.Drawing.Size(229, 21);
            this.chkValorPremio14.TabIndex = 140;
            this.chkValorPremio14.Text = "a�adir Premio de 14 aciertos";
            // 
            // txtLimiteProbAcum
            // 
            this.txtLimiteProbAcum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLimiteProbAcum.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLimiteProbAcum.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLimiteProbAcum.ForeColor = System.Drawing.Color.Black;
            this.txtLimiteProbAcum.Location = new System.Drawing.Point(223, 72);
            this.txtLimiteProbAcum.MaxLength = 7;
            this.txtLimiteProbAcum.Name = "txtLimiteProbAcum";
            this.txtLimiteProbAcum.Size = new System.Drawing.Size(48, 21);
            this.txtLimiteProbAcum.TabIndex = 139;
            this.txtLimiteProbAcum.Text = "1";
            this.txtLimiteProbAcum.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            // 
            // label31
            // 
            this.label31.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label31.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label31.ForeColor = System.Drawing.Color.Black;
            this.label31.Location = new System.Drawing.Point(34, 72);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(183, 21);
            this.label31.TabIndex = 138;
            this.label31.Text = "L�mite de la prob. acumulada:";
            this.label31.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txMaxColumnas
            // 
            this.txMaxColumnas.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txMaxColumnas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txMaxColumnas.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txMaxColumnas.ForeColor = System.Drawing.Color.Black;
            this.txMaxColumnas.Location = new System.Drawing.Point(215, 48);
            this.txMaxColumnas.MaxLength = 7;
            this.txMaxColumnas.Name = "txMaxColumnas";
            this.txMaxColumnas.Size = new System.Drawing.Size(56, 21);
            this.txMaxColumnas.TabIndex = 137;
            this.txMaxColumnas.Text = "4782969";
            this.txMaxColumnas.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txMaxColumnas_KeyPress);
            this.txMaxColumnas.TextChanged += new System.EventHandler(this.txMaxColumnas_TextChanged_1);
            // 
            // lblMaxCol
            // 
            this.lblMaxCol.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMaxCol.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMaxCol.ForeColor = System.Drawing.Color.Black;
            this.lblMaxCol.Location = new System.Drawing.Point(37, 48);
            this.lblMaxCol.Name = "lblMaxCol";
            this.lblMaxCol.Size = new System.Drawing.Size(178, 21);
            this.lblMaxCol.TabIndex = 136;
            this.lblMaxCol.Text = "N� m�ximo de columnas:";
            this.lblMaxCol.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // checkValorOrdenacion
            // 
            this.checkValorOrdenacion.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.checkValorOrdenacion.ForeColor = System.Drawing.Color.Black;
            this.checkValorOrdenacion.Location = new System.Drawing.Point(280, 48);
            this.checkValorOrdenacion.Name = "checkValorOrdenacion";
            this.checkValorOrdenacion.Size = new System.Drawing.Size(229, 21);
            this.checkValorOrdenacion.TabIndex = 135;
            this.checkValorOrdenacion.Text = "a�adir probabilidad acumulada";
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.BackColor = System.Drawing.Color.LightSalmon;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button2.Image = ((System.Drawing.Image)(resources.GetObject("button2.Image")));
            this.button2.Location = new System.Drawing.Point(504, 18);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(24, 21);
            this.button2.TabIndex = 5;
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // TxFicheroSalida
            // 
            this.TxFicheroSalida.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TxFicheroSalida.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.TxFicheroSalida.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TxFicheroSalida.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxFicheroSalida.ForeColor = System.Drawing.Color.Black;
            this.TxFicheroSalida.Location = new System.Drawing.Point(48, 18);
            this.TxFicheroSalida.Name = "TxFicheroSalida";
            this.TxFicheroSalida.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.TxFicheroSalida.Size = new System.Drawing.Size(455, 21);
            this.TxFicheroSalida.TabIndex = 4;
            this.TxFicheroSalida.Text = "(falta selecci�n)";
            this.TxFicheroSalida.TextChanged += new System.EventHandler(this.TxFicheroSalida_TextChanged);
            // 
            // statusBar1
            // 
            this.statusBar1.Location = new System.Drawing.Point(0, 456);
            this.statusBar1.Name = "statusBar1";
            this.statusBar1.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.statusBarPanel1,
            this.statusBarPanel2,
            this.statusBarPanel3,
            this.statusBarPanel4});
            this.statusBar1.Size = new System.Drawing.Size(735, 22);
            this.statusBar1.TabIndex = 7;
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
            this.statusBarPanel3.Text = "Estado";
            this.statusBarPanel3.Width = 49;
            // 
            // statusBarPanel4
            // 
            this.statusBarPanel4.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.statusBarPanel4.Name = "statusBarPanel4";
            this.statusBarPanel4.Width = 10;
            // 
            // button3
            // 
            this.button3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button3.BackColor = System.Drawing.Color.DarkSalmon;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button3.Image = ((System.Drawing.Image)(resources.GetObject("button3.Image")));
            this.button3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button3.Location = new System.Drawing.Point(507, 416);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(100, 32);
            this.button3.TabIndex = 9;
            this.button3.Text = "&Cancelar";
            this.button3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.Productos);
            this.tabControl1.Controls.Add(this.Sumas);
            this.tabControl1.Controls.Add(this.Multiple);
            this.tabControl1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControl1.Location = new System.Drawing.Point(184, 204);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(537, 200);
            this.tabControl1.TabIndex = 132;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // Productos
            // 
            this.Productos.BackColor = System.Drawing.Color.Bisque;
            this.Productos.Controls.Add(this.grLAE);
            this.Productos.Controls.Add(this.groupBox2);
            this.Productos.Location = new System.Drawing.Point(4, 22);
            this.Productos.Name = "Productos";
            this.Productos.Size = new System.Drawing.Size(529, 174);
            this.Productos.TabIndex = 0;
            this.Productos.Text = "Por productos";
            // 
            // grLAE
            // 
            this.grLAE.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.grLAE.BackColor = System.Drawing.Color.Bisque;
            this.grLAE.Controls.Add(this.textBox5);
            this.grLAE.Controls.Add(this.textBox4);
            this.grLAE.Controls.Add(this.textBox3);
            this.grLAE.Controls.Add(this.lblPctAl14);
            this.grLAE.Controls.Add(this.textBox2);
            this.grLAE.Controls.Add(this.lblPrecioApuesta);
            this.grLAE.Controls.Add(this.textBox1);
            this.grLAE.Controls.Add(this.lblRecaudacion);
            this.grLAE.Controls.Add(this.txRecaudacion);
            this.grLAE.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grLAE.ForeColor = System.Drawing.Color.Maroon;
            this.grLAE.Location = new System.Drawing.Point(312, 8);
            this.grLAE.Name = "grLAE";
            this.grLAE.Size = new System.Drawing.Size(212, 156);
            this.grLAE.TabIndex = 12;
            this.grLAE.TabStop = false;
            this.grLAE.Text = "Configuraci�n L.A.E.";
            // 
            // textBox5
            // 
            this.textBox5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox5.Enabled = false;
            this.textBox5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox5.Location = new System.Drawing.Point(186, 36);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(16, 20);
            this.textBox5.TabIndex = 8;
            this.textBox5.Text = "�";
            // 
            // textBox4
            // 
            this.textBox4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox4.Enabled = false;
            this.textBox4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox4.Location = new System.Drawing.Point(186, 100);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(16, 20);
            this.textBox4.TabIndex = 7;
            this.textBox4.Text = "%";
            // 
            // textBox3
            // 
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox3.Enabled = false;
            this.textBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox3.Location = new System.Drawing.Point(186, 68);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(16, 20);
            this.textBox3.TabIndex = 6;
            this.textBox3.Text = "�";
            // 
            // lblPctAl14
            // 
            this.lblPctAl14.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPctAl14.ForeColor = System.Drawing.Color.Black;
            this.lblPctAl14.Location = new System.Drawing.Point(10, 100);
            this.lblPctAl14.Name = "lblPctAl14";
            this.lblPctAl14.Size = new System.Drawing.Size(143, 20);
            this.lblPctAl14.TabIndex = 5;
            this.lblPctAl14.Text = "% para los 14 aciertos";
            this.lblPctAl14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBox2
            // 
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox2.Location = new System.Drawing.Point(161, 100);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(24, 20);
            this.textBox2.TabIndex = 4;
            this.textBox2.Text = "15";
            this.textBox2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            this.textBox2.TextChanged += new System.EventHandler(this.txLAEgenerico_TextChanged);
            // 
            // lblPrecioApuesta
            // 
            this.lblPrecioApuesta.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrecioApuesta.ForeColor = System.Drawing.Color.Black;
            this.lblPrecioApuesta.Location = new System.Drawing.Point(10, 68);
            this.lblPrecioApuesta.Name = "lblPrecioApuesta";
            this.lblPrecioApuesta.Size = new System.Drawing.Size(134, 20);
            this.lblPrecioApuesta.TabIndex = 3;
            this.lblPrecioApuesta.Text = "Precio de una apuesta";
            this.lblPrecioApuesta.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // textBox1
            // 
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(161, 68);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(24, 20);
            this.textBox1.TabIndex = 2;
            this.textBox1.Text = "0,5";
            this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            this.textBox1.TextChanged += new System.EventHandler(this.txLAEgenerico_TextChanged);
            // 
            // lblRecaudacion
            // 
            this.lblRecaudacion.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRecaudacion.ForeColor = System.Drawing.Color.Black;
            this.lblRecaudacion.Location = new System.Drawing.Point(10, 36);
            this.lblRecaudacion.Name = "lblRecaudacion";
            this.lblRecaudacion.Size = new System.Drawing.Size(94, 20);
            this.lblRecaudacion.TabIndex = 1;
            this.lblRecaudacion.Text = "Recaudaci�n";
            this.lblRecaudacion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // txRecaudacion
            // 
            this.txRecaudacion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txRecaudacion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txRecaudacion.Location = new System.Drawing.Point(121, 36);
            this.txRecaudacion.Name = "txRecaudacion";
            this.txRecaudacion.Size = new System.Drawing.Size(64, 20);
            this.txRecaudacion.TabIndex = 0;
            this.txRecaudacion.Text = "15000000";
            this.txRecaudacion.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            this.txRecaudacion.TextChanged += new System.EventHandler(this.txLAEgenerico_TextChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.BackColor = System.Drawing.Color.Bisque;
            this.groupBox2.Controls.Add(this.txColumna);
            this.groupBox2.Controls.Add(this.rbColumna);
            this.groupBox2.Controls.Add(this.comboBox1);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.TxAcertantes);
            this.groupBox2.Controls.Add(this.rbAcertantes);
            this.groupBox2.Controls.Add(this.TxPremio);
            this.groupBox2.Controls.Add(this.rbPremio);
            this.groupBox2.Controls.Add(this.TxProbabilidad);
            this.groupBox2.Controls.Add(this.TxLN);
            this.groupBox2.Controls.Add(this.rbProbabilidad);
            this.groupBox2.Controls.Add(this.rbLN);
            this.groupBox2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.groupBox2.Location = new System.Drawing.Point(8, 8);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(296, 156);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Valor central de ordenaci�n";
            // 
            // txColumna
            // 
            this.txColumna.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txColumna.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txColumna.Enabled = false;
            this.txColumna.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txColumna.ForeColor = System.Drawing.Color.Black;
            this.txColumna.Location = new System.Drawing.Point(128, 120);
            this.txColumna.MaxLength = 14;
            this.txColumna.Name = "txColumna";
            this.txColumna.Size = new System.Drawing.Size(120, 21);
            this.txColumna.TabIndex = 29;
            this.txColumna.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txColumna_KeyPress);
            this.txColumna.TextChanged += new System.EventHandler(this.txColumna_TextChanged);
            // 
            // rbColumna
            // 
            this.rbColumna.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbColumna.ForeColor = System.Drawing.Color.Black;
            this.rbColumna.Location = new System.Drawing.Point(10, 120);
            this.rbColumna.Name = "rbColumna";
            this.rbColumna.Size = new System.Drawing.Size(106, 21);
            this.rbColumna.TabIndex = 28;
            this.rbColumna.Text = "Columna ";
            this.rbColumna.CheckedChanged += new System.EventHandler(this.Generico_CheckedChanged);
            // 
            // comboBox1
            // 
            this.comboBox1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.comboBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox1.ForeColor = System.Drawing.Color.Black;
            this.comboBox1.Items.AddRange(new object[] {
            "0",
            "10",
            "11",
            "12",
            "13",
            "14"});
            this.comboBox1.Location = new System.Drawing.Point(248, 96);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(40, 21);
            this.comboBox1.TabIndex = 27;
            this.comboBox1.Visible = false;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged_1);
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(251, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(16, 21);
            this.label2.TabIndex = 26;
            this.label2.Text = "�";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TxAcertantes
            // 
            this.TxAcertantes.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TxAcertantes.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxAcertantes.ForeColor = System.Drawing.Color.Black;
            this.TxAcertantes.Location = new System.Drawing.Point(128, 24);
            this.TxAcertantes.Name = "TxAcertantes";
            this.TxAcertantes.Size = new System.Drawing.Size(120, 21);
            this.TxAcertantes.TabIndex = 25;
            this.TxAcertantes.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            this.TxAcertantes.TextChanged += new System.EventHandler(this.TxAcertantes_TextChanged_1);
            // 
            // rbAcertantes
            // 
            this.rbAcertantes.Checked = true;
            this.rbAcertantes.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbAcertantes.ForeColor = System.Drawing.Color.Black;
            this.rbAcertantes.Location = new System.Drawing.Point(10, 24);
            this.rbAcertantes.Name = "rbAcertantes";
            this.rbAcertantes.Size = new System.Drawing.Size(106, 21);
            this.rbAcertantes.TabIndex = 24;
            this.rbAcertantes.TabStop = true;
            this.rbAcertantes.Text = "N� acertantes";
            this.rbAcertantes.CheckedChanged += new System.EventHandler(this.Generico_CheckedChanged);
            // 
            // TxPremio
            // 
            this.TxPremio.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TxPremio.Enabled = false;
            this.TxPremio.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxPremio.ForeColor = System.Drawing.Color.Black;
            this.TxPremio.Location = new System.Drawing.Point(128, 48);
            this.TxPremio.Name = "TxPremio";
            this.TxPremio.Size = new System.Drawing.Size(120, 21);
            this.TxPremio.TabIndex = 23;
            this.TxPremio.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            this.TxPremio.TextChanged += new System.EventHandler(this.TxPremio_TextChanged_1);
            // 
            // rbPremio
            // 
            this.rbPremio.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbPremio.ForeColor = System.Drawing.Color.Black;
            this.rbPremio.Location = new System.Drawing.Point(10, 48);
            this.rbPremio.Name = "rbPremio";
            this.rbPremio.Size = new System.Drawing.Size(106, 21);
            this.rbPremio.TabIndex = 22;
            this.rbPremio.Text = "Premio";
            this.rbPremio.CheckedChanged += new System.EventHandler(this.Generico_CheckedChanged);
            // 
            // TxProbabilidad
            // 
            this.TxProbabilidad.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TxProbabilidad.Enabled = false;
            this.TxProbabilidad.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxProbabilidad.ForeColor = System.Drawing.Color.Black;
            this.TxProbabilidad.Location = new System.Drawing.Point(128, 72);
            this.TxProbabilidad.Name = "TxProbabilidad";
            this.TxProbabilidad.Size = new System.Drawing.Size(160, 21);
            this.TxProbabilidad.TabIndex = 21;
            this.TxProbabilidad.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            this.TxProbabilidad.TextChanged += new System.EventHandler(this.TxProbabilidad_TextChanged_1);
            // 
            // TxLN
            // 
            this.TxLN.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TxLN.Enabled = false;
            this.TxLN.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxLN.ForeColor = System.Drawing.Color.Black;
            this.TxLN.Location = new System.Drawing.Point(128, 96);
            this.TxLN.Name = "TxLN";
            this.TxLN.Size = new System.Drawing.Size(120, 21);
            this.TxLN.TabIndex = 20;
            this.TxLN.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.GenericTexBox_KeyPress);
            this.TxLN.TextChanged += new System.EventHandler(this.TxLN_TextChanged_1);
            // 
            // rbProbabilidad
            // 
            this.rbProbabilidad.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbProbabilidad.ForeColor = System.Drawing.Color.Black;
            this.rbProbabilidad.Location = new System.Drawing.Point(10, 72);
            this.rbProbabilidad.Name = "rbProbabilidad";
            this.rbProbabilidad.Size = new System.Drawing.Size(106, 21);
            this.rbProbabilidad.TabIndex = 19;
            this.rbProbabilidad.Text = "Probabilidad";
            this.rbProbabilidad.CheckedChanged += new System.EventHandler(this.Generico_CheckedChanged);
            // 
            // rbLN
            // 
            this.rbLN.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbLN.ForeColor = System.Drawing.Color.Black;
            this.rbLN.Location = new System.Drawing.Point(10, 96);
            this.rbLN.Name = "rbLN";
            this.rbLN.Size = new System.Drawing.Size(115, 21);
            this.rbLN.TabIndex = 18;
            this.rbLN.Text = "Log. neperiano";
            this.rbLN.CheckedChanged += new System.EventHandler(this.Generico_CheckedChanged);
            // 
            // Sumas
            // 
            this.Sumas.BackColor = System.Drawing.Color.Bisque;
            this.Sumas.Controls.Add(this.TxValorCentralSumas);
            this.Sumas.Controls.Add(this.label1);
            this.Sumas.Location = new System.Drawing.Point(4, 22);
            this.Sumas.Name = "Sumas";
            this.Sumas.Size = new System.Drawing.Size(529, 174);
            this.Sumas.TabIndex = 1;
            this.Sumas.Text = "Por Sumas";
            // 
            // TxValorCentralSumas
            // 
            this.TxValorCentralSumas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TxValorCentralSumas.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxValorCentralSumas.Location = new System.Drawing.Point(324, 65);
            this.TxValorCentralSumas.MaxLength = 7;
            this.TxValorCentralSumas.Name = "TxValorCentralSumas";
            this.TxValorCentralSumas.Size = new System.Drawing.Size(67, 21);
            this.TxValorCentralSumas.TabIndex = 7;
            this.TxValorCentralSumas.Text = "0";
            this.TxValorCentralSumas.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.TxValorCentralSumas.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.GenericTexBox_KeyPress);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(137, 65);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(181, 21);
            this.label1.TabIndex = 6;
            this.label1.Text = "Valor central de ordenaci�n ";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Multiple
            // 
            this.Multiple.BackColor = System.Drawing.Color.Bisque;
            this.Multiple.Controls.Add(this.groupBox3);
            this.Multiple.Controls.Add(this.label36);
            this.Multiple.Controls.Add(this.txNumColumnasPorTramo);
            this.Multiple.Controls.Add(this.label35);
            this.Multiple.Controls.Add(this.txNumPuntos);
            this.Multiple.Controls.Add(this.label34);
            this.Multiple.Controls.Add(this.txLNmaximo);
            this.Multiple.Controls.Add(this.txLNminimo);
            this.Multiple.Controls.Add(this.lblLNMax);
            this.Multiple.Controls.Add(this.lblLNMin);
            this.Multiple.Location = new System.Drawing.Point(4, 22);
            this.Multiple.Name = "Multiple";
            this.Multiple.Size = new System.Drawing.Size(529, 174);
            this.Multiple.TabIndex = 2;
            this.Multiple.Text = "Multiple";
            // 
            // groupBox3
            // 
            this.groupBox3.BackColor = System.Drawing.Color.Bisque;
            this.groupBox3.Controls.Add(this.rbPorSumas);
            this.groupBox3.Controls.Add(this.rbPorProbabilidad);
            this.groupBox3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox3.Location = new System.Drawing.Point(80, 88);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(347, 62);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Tipo de Valoraci�n";
            // 
            // rbPorSumas
            // 
            this.rbPorSumas.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbPorSumas.Location = new System.Drawing.Point(223, 23);
            this.rbPorSumas.Name = "rbPorSumas";
            this.rbPorSumas.Size = new System.Drawing.Size(88, 16);
            this.rbPorSumas.TabIndex = 1;
            this.rbPorSumas.Text = "Por sumas";
            this.rbPorSumas.CheckedChanged += new System.EventHandler(this.GenericRB_CheckedChanged);
            // 
            // rbPorProbabilidad
            // 
            this.rbPorProbabilidad.Checked = true;
            this.rbPorProbabilidad.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbPorProbabilidad.Location = new System.Drawing.Point(35, 23);
            this.rbPorProbabilidad.Name = "rbPorProbabilidad";
            this.rbPorProbabilidad.Size = new System.Drawing.Size(136, 16);
            this.rbPorProbabilidad.TabIndex = 0;
            this.rbPorProbabilidad.TabStop = true;
            this.rbPorProbabilidad.Text = "Por probabilidad (LN)";
            this.rbPorProbabilidad.CheckedChanged += new System.EventHandler(this.GenericRB_CheckedChanged);
            // 
            // label36
            // 
            this.label36.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label36.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label36.ForeColor = System.Drawing.Color.Maroon;
            this.label36.Location = new System.Drawing.Point(80, 18);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(347, 20);
            this.label36.TabIndex = 10;
            this.label36.Text = "ORDENACI�N POR M�LTIPLES PUNTOS";
            this.label36.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txNumColumnasPorTramo
            // 
            this.txNumColumnasPorTramo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txNumColumnasPorTramo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txNumColumnasPorTramo.Location = new System.Drawing.Point(375, 62);
            this.txNumColumnasPorTramo.Name = "txNumColumnasPorTramo";
            this.txNumColumnasPorTramo.Size = new System.Drawing.Size(52, 21);
            this.txNumColumnasPorTramo.TabIndex = 9;
            this.txNumColumnasPorTramo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txMaxColumnas_KeyPress);
            // 
            // label35
            // 
            this.label35.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label35.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label35.Location = new System.Drawing.Point(236, 62);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(140, 20);
            this.label35.TabIndex = 8;
            this.label35.Text = "N� columnas por punto";
            // 
            // txNumPuntos
            // 
            this.txNumPuntos.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txNumPuntos.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txNumPuntos.Location = new System.Drawing.Point(375, 40);
            this.txNumPuntos.Name = "txNumPuntos";
            this.txNumPuntos.Size = new System.Drawing.Size(52, 21);
            this.txNumPuntos.TabIndex = 7;
            this.txNumPuntos.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txMaxColumnas_KeyPress);
            // 
            // label34
            // 
            this.label34.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label34.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label34.Location = new System.Drawing.Point(236, 40);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(140, 20);
            this.label34.TabIndex = 6;
            this.label34.Text = "N� puntos centrales";
            // 
            // txLNmaximo
            // 
            this.txLNmaximo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txLNmaximo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txLNmaximo.Location = new System.Drawing.Point(160, 62);
            this.txLNmaximo.Name = "txLNmaximo";
            this.txLNmaximo.Size = new System.Drawing.Size(76, 21);
            this.txLNmaximo.TabIndex = 5;
            this.txLNmaximo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.GenericTexBox_KeyPress);
            // 
            // txLNminimo
            // 
            this.txLNminimo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txLNminimo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txLNminimo.Location = new System.Drawing.Point(160, 40);
            this.txLNminimo.Name = "txLNminimo";
            this.txLNminimo.Size = new System.Drawing.Size(76, 21);
            this.txLNminimo.TabIndex = 4;
            this.txLNminimo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.GenericTexBox_KeyPress);
            // 
            // lblLNMax
            // 
            this.lblLNMax.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblLNMax.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLNMax.Location = new System.Drawing.Point(80, 62);
            this.lblLNMax.Name = "lblLNMax";
            this.lblLNMax.Size = new System.Drawing.Size(84, 20);
            this.lblLNMax.TabIndex = 3;
            this.lblLNMax.Text = "LN m�ximo";
            // 
            // lblLNMin
            // 
            this.lblLNMin.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblLNMin.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLNMin.Location = new System.Drawing.Point(80, 40);
            this.lblLNMin.Name = "lblLNMin";
            this.lblLNMin.Size = new System.Drawing.Size(84, 20);
            this.lblLNMin.TabIndex = 2;
            this.lblLNMin.Text = "LN m�nimo";
            // 
            // controlPorcentajes1
            // 
            this.controlPorcentajes1.archivoPorcentajes = null;
            this.controlPorcentajes1.BackColor = System.Drawing.Color.Bisque;
            this.controlPorcentajes1.CaptionText = "  P O R C E N T A J E S";
            this.controlPorcentajes1.FormatoFicheroValoraciones = ((short)(0));
            this.controlPorcentajes1.Jornada = "01";
            this.controlPorcentajes1.Location = new System.Drawing.Point(12, 72);
            this.controlPorcentajes1.Name = "controlPorcentajes1";
            this.controlPorcentajes1.ReadOnly = false;
            this.controlPorcentajes1.Size = new System.Drawing.Size(160, 366);
            this.controlPorcentajes1.TabIndex = 133;
            this.controlPorcentajes1.Temporada = "2004/2005";
            this.controlPorcentajes1.Modificado += new System.EventHandler(this.controlPorcentajes1_Modificado);
            // 
            // OrdenarPorProbabilidadFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(735, 478);
            this.Controls.Add(this.controlPorcentajes1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.statusBar1);
            this.Controls.Add(this.btCalcular);
            this.Controls.Add(this.groupSalida);
            this.Controls.Add(this.groupBox1);
            this.MinimumSize = new System.Drawing.Size(700, 500);
            this.Name = "OrdenarPorProbabilidadFrm";
            this.Text = "Ordenaci�n de columnas por probabilidad";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupSalida.ResumeLayout(false);
            this.groupSalida.PerformLayout();
            // StatusBarPanel controls don't support ISupportInitialize in .NET 8
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).EndInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel2)).EndInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel3)).EndInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel4)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.Productos.ResumeLayout(false);
            this.grLAE.ResumeLayout(false);
            this.grLAE.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.Sumas.ResumeLayout(false);
            this.Sumas.PerformLayout();
            this.Multiple.ResumeLayout(false);
            this.Multiple.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion
		

		private void Generico_CheckedChanged(object sender, System.EventArgs e)
		{
			TxLN.Enabled =false;
			TxProbabilidad.Enabled =false;
			TxPremio.Enabled =false;
			TxAcertantes.Enabled =false;
			txColumna.Enabled =false;
			statusBarPanel4.Text = "";
			comboBox1.Visible =false;
			RadioButton MiRadioButton =(RadioButton)sender;

			switch (MiRadioButton.Name )
			{
				case "rbLN" : TxLN.Enabled =true; comboBox1.Visible =true;break;
				case "rbProbabilidad" : TxProbabilidad.Enabled =true; break;
				case "rbPremio" : TxPremio.Enabled =true; break;
				case "rbAcertantes" : TxAcertantes.Enabled =true; break;
				case "rbColumna":txColumna.Enabled =true;break;
				default: break;
			}
		}

		private void btCalcular_Click(object sender, System.EventArgs e)
		{
			Cursor.Current = Cursors.WaitCursor ;
			

			switch(tabControl1.SelectedTab.Name)       
			{         
				case "Productos": 
					CalculoMultiple=false;
					OrdenaPorProductos();        
					break;
				case "Sumas":
					CalculoMultiple=false;
					OrdenaPorSumas();
					break;
				case "Multiple":
					CalculoMultiple=true;
					OrdenacionMultiple();
					break;
			}

			Cursor.Current = Cursors.Default  ;
		}
		private void InicializarApuestas()
		{
			for (int i=0;i<4782969;i++)
			{
				Ap14T[i].Columna=i;
				Ap14T[i].Probabilidad =0;
				Ap14T[i].ProbabilidadDiferencial =0;
			}
		}
		private void OrdenacionMultiple()
		{
			double probMin=Convert.ToDouble (txLNminimo .Text );
			double probMax=Convert.ToDouble (txLNmaximo .Text );
			int NumPuntos=Convert.ToInt16  (txNumPuntos .Text );
			int i=0;
			int x;

			string maxcol=txMaxColumnas.Text;
			if (probMax<probMin)
			{
				double aux=probMax;
				probMax=probMin;
				probMin=aux;
			}
			if(rbPorProbabilidad.Checked )
			{
				if(probMax>0)
				{
					MessageBox.Show ("Los LN deben ser valores negativos");
					return;
				}
			}

			double paso = (probMax-probMin)/(NumPuntos-1);
			txMaxColumnas.Text = txNumColumnasPorTramo.Text;
			Admitidas.SetAll (false);

			for (double prCentral = probMin; prCentral<=probMax; prCentral+=paso)
			{
				i++;
				numIter= i.ToString () + " ";
				if(rbPorProbabilidad.Checked )
				{
					_Probabilidad =Math.Exp (prCentral);
				}
				else
				{
					TxValorCentralSumas.Text=prCentral.ToString ();
				}
				if(i==1)
				{
					if(rbPorProbabilidad.Checked )
					{OrdenaPorProductos();}
					else
					{OrdenaPorSumas();}
				}
				else
				{
					for (x=0;x<4782969;x++)
					{
						if(Ap14T[x].ProbabilidadDiferencial == (float) 9E+10) continue;
						Ap14T[x].ProbabilidadDiferencial  = Math.Abs (Ap14T[x].Probabilidad  - (float)prCentral);
					}
					statusBarPanel4.Text =numIter + "Ordenando apuestas ...";
					Application.DoEvents();
					ordena (0, 4782968);
					for (int nr=0; nr< MaxColumnas; nr++) Admitidas[Ap14T[nr].Columna]=true;
				}
			}
			GrabarAdmitidasMultiples ();
			txMaxColumnas.Text=maxcol;
		}
		private void OrdenaPorProductos()
		{
			int Partido;
			float Prob=0;
			if (_Probabilidad>0)_LN=Math.Log (_Probabilidad); else _LN= -99999;
			InicializarApuestas();

			if (Convert.ToInt32 (txMaxColumnas.Text)>4782969) txMaxColumnas.Text="4782969";

			//---Leer Porcentajes --------------
			v=controlPorcentajes1.Valores;
			Porcentajes Pct = new Porcentajes(v);
			p= Pct.ValoresNeperianos();

			//'---- probabilidad del 14 de la 1� apuesta i 
			// c�lculo de valores complementarios --------
			for (Partido = 0;Partido<14; Partido++)
			{
				Prob +=p[Partido, 0];
				Cr[Partido, 1] = p[Partido, 1] - p[Partido, 0];
				Cr[Partido, 2] = p[Partido, 2] - p[Partido, 0];
			}

			if (rb14Triples.Checked ==true) 
			{
				Calcula14Triples(Prob);
			}
			else
			{
				CalculaFichero(Prob);
			}
		}
		private void OrdenaPorSumas()
		{
			int Partido;
			float Prob=0;
			InicializarApuestas();
			_LN=Convert.ToDouble (this.TxValorCentralSumas.Text);

			if (Convert.ToInt32 (txMaxColumnas.Text)>4782969) txMaxColumnas.Text="4782969";

			//---Leer Porcentajes --------------
			v=controlPorcentajes1.Valores;
			//'---- Suma del 14 de la 1� apuesta y 
			// c�lculo de valores complementarios --------
			for (Partido = 0;Partido<14; Partido++)
			{
				p[Partido, 0]=(float) v[Partido, 0];
				p[Partido, 1]=(float) v[Partido, 1];
				p[Partido, 2]=(float) v[Partido, 2];
				Prob +=p[Partido, 0];
				Cr[Partido, 1] = p[Partido, 1] - p[Partido, 0];
				Cr[Partido, 2] = p[Partido, 2] - p[Partido, 0];
			}

			if (rb14Triples.Checked ==true) 
			{
				Calcula14Triples(Prob);
			}
			else
			{
				CalculaFichero(Prob);
			}
		}

		private void Calcula14Triples(float Prob)
		{
			
			Bits.SetAll (true);
			NumApuestas = 4782969;
			statusBarPanel4.Text =numIter + "Calculando probabilidades ...";
			Application.DoEvents();
			Profundidad = 0;
			EncontrarDistantes1 (Prob, 0, 0, 14);
			Ap14T[0].ProbabilidadDiferencial  = Math.Abs (Prob - (float)_LN);
			Ap14T[0].Probabilidad = Prob;
			Ap14T[0].Columna = 0;

			statusBarPanel4.Text =numIter + "Ordenando apuestas ...";
			Application.DoEvents();
			ordena (0, 4782968);
		    
			statusBarPanel4.Text =numIter + "Guardando apuestas ...";
			Application.DoEvents();
			if(	CalculoMultiple==false)
			{
				GrabacionColumnas();
				statusBarPanel4.Text = "Finalizado " + c.ToString () + " columnas";
			}
			else
			{
				for (int nr=0; nr< MaxColumnas; nr++) Admitidas[Ap14T[nr].Columna]=true;
			}
		}

		private void CalculaFichero(float Prob)
		{
			LeerColumnas();
			statusBarPanel4.Text ="Calculando probabilidades ...";
			Application.DoEvents();
			Profundidad = 0;
			EncontrarDistantes1 (Prob, 0, 0, 14);
			if (Bits[0])
			{
				Ap14T[0].ProbabilidadDiferencial  = Math.Abs (Prob - (float)_LN);
				Ap14T[0].Probabilidad = Prob;
				Ap14T[0].Columna = 0;
			}
			else
			{
				Ap14T[0].ProbabilidadDiferencial = (float) 9E+10;
				Ap14T[0].Probabilidad = Prob;
			}
			statusBarPanel4.Text =numIter + "Ordenando apuestas ...";
			Application.DoEvents();
			ordena (0, 4782968);
		    
			statusBarPanel4.Text =numIter + "Guardando apuestas ...";
			Application.DoEvents();
			if(	CalculoMultiple==false)
			{
				GrabacionColumnas();
				statusBarPanel4.Text = "Finalizado " + c.ToString () + " columnas";
			}
			else
			{
				for (int nr=0; nr< MaxColumnas; nr++) Admitidas[Ap14T[nr].Columna]=true;
			}
		}
		private void LeerColumnas() 
		{
            IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(archivoEntrada);
			NumApuestas=Convert.ToInt32(comBaseCols.ObtenNumCols());
			Bits=comBaseCols.LeerTodasColsABitArray(14);
			comBaseCols.Cerrar();						
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
					Indice = IndiceInicial + pot[Partido] * z;
					Prob = pProb + Cr[Partido, z];
					
					if (Bits[Indice])
					{
						Ap14T[Indice].Columna = Indice;
						Ap14T[Indice].ProbabilidadDiferencial  = Math.Abs(Prob - (float)_LN);
						Ap14T[Indice].Probabilidad = Prob;
					}
					else
					{
						Ap14T[Indice].ProbabilidadDiferencial = (float) 9E+10;
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

		private void GrabacionColumnas()
		{						
			string archivoSalida=this.TxFicheroSalida.Text ;
			float pr=(float) (PrecioApuesta*PorcentajeDestinadoAlPremiode14/100);
			float prob;
			float premio;
			char sep=(char)9;
			float PremioMax = (float) (Recaudacion*PorcentajeDestinadoAlPremiode14/100);


			LimiteProbabilidadAcumulada=(float) Convert.ToDouble (this.txtLimiteProbAcum .Text );

			ConvertidorDeBases col =new ConvertidorDeBases();
            IArchivoColumnas comCols = new ArchivoColumnasTexto(archivoSalida);
			if (MaxColumnas <NumApuestas) NumApuestas =MaxColumnas;
			ProbabilidadAcumulada=0;
			c=0;
			for (int nr=0; nr< NumApuestas; nr++) 
			{
				StringBuilder linea= new StringBuilder (col.ConvNumAColumna(Ap14T[nr].Columna));
				if(tabControl1.SelectedIndex ==0)
				{
					prob=(float) Math.Exp (Ap14T[nr].Probabilidad) ;
					ProbabilidadAcumulada += prob ;
					if (ProbabilidadAcumulada > LimiteProbabilidadAcumulada) break;
				}
				else
				{
					prob=Ap14T[nr].Probabilidad ;
					ProbabilidadAcumulada = Ap14T[nr].Probabilidad;
				}
				if (checkValorOrdenacion.Checked ) 
				{
					linea.Append (sep);
					linea.Append (ProbabilidadAcumulada) ;
				}
				
				if (chkValorPremio14.Checked ) 
				{
					premio=pr/prob;
					if (premio>PremioMax) premio = PremioMax;
					linea.Append (sep);
					linea.Append (premio);
				}

				comCols.GuardarColsComa (linea.ToString () );

				c++;
				
			}	
			comCols.Cerrar();	
			if(tabControl1.SelectedIndex ==0)
			{
				statusBarPanel4.Text = "Finalizado " + c.ToString () + " columnas Pr. acum. =" +(100*ProbabilidadAcumulada).ToString () ;
			}
			else
			{
				statusBarPanel4.Text = "Finalizado " + c.ToString () + " columnas";
			}
		}

		private void GrabarAdmitidasMultiples()
		{						
			string archivoSalida=this.TxFicheroSalida.Text ;
			float pr=(float) (PrecioApuesta*PorcentajeDestinadoAlPremiode14/100);
			float prob;
			float premio;
			char sep=(char)9;
			float PremioMax = (float) (Recaudacion*PorcentajeDestinadoAlPremiode14/100);


			LimiteProbabilidadAcumulada=(float) Convert.ToDouble (this.txtLimiteProbAcum .Text );

			ConvertidorDeBases col =new ConvertidorDeBases();
            IArchivoColumnas comCols = new ArchivoColumnasTexto(archivoSalida);
			if (MaxColumnas <NumApuestas) NumApuestas =MaxColumnas;
			ProbabilidadAcumulada=0;
			c=0;
			for (int nr=0; nr< 4782969; nr++) 
			{
				if(Admitidas[nr]==false) continue;
				StringBuilder linea= new StringBuilder (col.ConvNumAColumna(nr));
				if(rbPorProbabilidad.Checked)
				{
					prob=(float) Math.Exp (Ap14T[nr].Probabilidad) ;
					ProbabilidadAcumulada += prob ;
					if (ProbabilidadAcumulada > LimiteProbabilidadAcumulada) break;
				}
				else
				{
					prob=Ap14T[nr].Probabilidad ;
					ProbabilidadAcumulada = Ap14T[nr].Probabilidad;
				}
				if (checkValorOrdenacion.Checked ) 
				{
					linea.Append (sep);
					linea.Append (ProbabilidadAcumulada) ;
				}
				
				if (chkValorPremio14.Checked ) 
				{
					premio=pr/prob;
					if (premio>PremioMax) premio = PremioMax;
					linea.Append (sep);
					linea.Append (premio);
				}

				comCols.GuardarColsComa (linea.ToString () );

				c++;
				
			}	
			comCols.Cerrar();	
			if(tabControl1.SelectedIndex ==0)
			{
				statusBarPanel4.Text = "Finalizado " + c.ToString () + " columnas Pr. acum. =" +(100*ProbabilidadAcumulada).ToString () ;
			}
			else
			{
				statusBarPanel4.Text = "Finalizado " + c.ToString () + " columnas";
			}
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
			SaveFileDialog abreFiltroDialog = new SaveFileDialog();
			abreFiltroDialog.InitialDirectory = "Columnas\\" ;
			abreFiltroDialog.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			if(abreFiltroDialog.ShowDialog () == DialogResult.OK)
			{
				salidaBinaria=abreFiltroDialog.FilterIndex==2;
				archivoSalida = abreFiltroDialog.FileName;		    	
				//TxFicheroSalida.Text = Path.GetFileName(archivoSalida);
				TxFicheroSalida.Text = archivoSalida;
				statusBarPanel4.Text = "";
			}
			HabilitarCalcular ();
		}

		private void rbFichero_CheckedChanged(object sender, System.EventArgs e)
		{
			button1.Visible =true;
			TxFicheroEntrada.Visible =true;
			HabilitarCalcular ();
		}

		private void rb14Triples_CheckedChanged(object sender, System.EventArgs e)
		{
			button1.Visible =false;
			TxFicheroEntrada.Visible =false;
			HabilitarCalcular ();
		}


		private void HabilitarCalcular()
		{
			btCalcular.Enabled = false;
			if (rb14Triples.Checked )
			{
				if (archivoSalida != "") this.btCalcular.Enabled = true;
			}
			else
			{
				if (archivoSalida != ""  && archivoEntrada !="") this.btCalcular.Enabled = true;
			}
			if(btCalcular.Enabled ) statusBarPanel4.Text ="Preparado"; else statusBarPanel4.Text ="Faltan datos";
			Application.DoEvents();
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog abreFiltroDialog = new OpenFileDialog();
			abreFiltroDialog.InitialDirectory = "Columnas\\" ;
			abreFiltroDialog.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;

			if(abreFiltroDialog.ShowDialog() == DialogResult.OK) 
			{	
				string archVal = Path.GetFileName(abreFiltroDialog.FileName);
				archivoEntrada=abreFiltroDialog.FileName;
				//TxFicheroEntrada .Text =Path.GetFileName(archivoEntrada);
				TxFicheroEntrada .Text =archivoEntrada;
				statusBarPanel4.Text = "";
			}
			HabilitarCalcular ();
		}

		private void button3_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void txLAEgenerico_TextChanged(object sender, System.EventArgs e)
		{
            if (!EstaInicializandoDatos)
            {
                Recaudacion = Convert.ToDouble(txRecaudacion.Text);
                PrecioApuesta = Convert.ToDouble(textBox1.Text);
                PorcentajeDestinadoAlPremiode14 = Convert.ToDouble(textBox2.Text);
                if (rbAcertantes.Checked) TxAcertantes_TextChanged_1(TxAcertantes, e);
                if (rbLN.Checked) TxLN_TextChanged_1(TxLN, e);
                if (rbPremio.Checked) TxPremio_TextChanged_1(TxPremio, e);
                if (rbProbabilidad.Checked) TxProbabilidad_TextChanged_1(TxProbabilidad, e);
            }
		}

		private void TxFicheroEntrada_TextChanged(object sender, System.EventArgs e)
		{
			archivoEntrada=TxFicheroEntrada.Text;
		}

		private void TxFicheroSalida_TextChanged(object sender, System.EventArgs e)
		{
			archivoSalida=TxFicheroSalida.Text;
		}

		private void GenericTexBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			Valida.Generic_KeyPress (TipoValidacionString.SoloNumerosDecimalesConSigno, sender, e);
		}

		private void txMaxColumnas_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			Valida.Generic_KeyPress (TipoValidacionString.SoloNumeros, sender, e);
		}

		private void AfegirAlHistoric()
		{
			HistoriaValoracionesFrm HistoriaValFrm;
			HistoriaValFrm = new HistoriaValoracionesFrm(v,2004 ,19, "");
			HistoriaValFrm.ShowDialog() ;	
		}

		private void textBoxgenerico_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			Valida.Generic_KeyPress (TipoValidacionString.SoloNumerosDecimales, sender, e);
		}

		private void generico_TextChanged(object sender, System.EventArgs e)
		{
			HabilitarCalcular();
		}

		private void TxPremio_TextChanged_1(object sender, System.EventArgs e)
		{
			if (TxPremio.Enabled ==true)
			{
				if (Pct.EsNumero (TxPremio.Text))
				{
					_Premio=Convert.ToDouble(TxPremio.Text.Replace (".",","));
					_Probabilidad=PrecioApuesta*PorcentajeDestinadoAlPremiode14/_Premio/100;
					_LN=Math.Log (_Probabilidad);
					_Acertantes =Recaudacion/PrecioApuesta*_Probabilidad;
					TxLN.Text = _LN.ToString();
					TxProbabilidad.Text = _Probabilidad.ToString() ;
					TxAcertantes.Text = _Acertantes.ToString ();
				}
			}
			if(_Acertantes<1) 
			{
				statusBarPanel4.Text ="Atenci�n el premio mostrado es superior a la cantidad destinada a premios de 14";
			}
			else
			{
				statusBarPanel4.Text ="";
			}
		}

		private void TxAcertantes_TextChanged_1(object sender, System.EventArgs e)
		{
			if (TxAcertantes.Enabled ==true)
			{
				if (Pct.EsNumero (TxAcertantes.Text))
				{
					_Acertantes=Convert.ToDouble(TxAcertantes.Text.Replace (".",","));
					_Premio=Recaudacion*PorcentajeDestinadoAlPremiode14/_Acertantes/100;
					_Probabilidad=PrecioApuesta*PorcentajeDestinadoAlPremiode14/_Premio/100;
					_LN=Math.Log (_Probabilidad);
					TxLN.Text = _LN.ToString();
					TxPremio.Text = _Premio.ToString();
					TxProbabilidad.Text = _Probabilidad.ToString() ;
				}
			}
		}

		private void TxProbabilidad_TextChanged_1(object sender, System.EventArgs e)
		{
			if (TxProbabilidad.Enabled ==true)
			{
				if (Pct.EsNumero (TxProbabilidad.Text))
				{
					_Probabilidad=Convert.ToDouble(TxProbabilidad.Text.Replace (".",","));
					_LN=Math.Log (_Probabilidad);
					_Premio = PrecioApuesta*PorcentajeDestinadoAlPremiode14/_Probabilidad/100;
					_Acertantes =Recaudacion/PrecioApuesta*_Probabilidad;
					TxLN.Text = _LN.ToString();
					TxPremio.Text = _Premio.ToString() ;
					TxAcertantes.Text = _Acertantes.ToString ();
				}
			}

		}

		private void TxLN_TextChanged_1(object sender, System.EventArgs e)
		{
			if (TxLN.Enabled ==true)
			{
				if (Pct.EsNumero (TxLN.Text))
				{	
					_LN=Convert.ToDouble(TxLN.Text.Replace (".",","));
					_Probabilidad=Math.Exp(_LN);
					_Premio = PrecioApuesta*PorcentajeDestinadoAlPremiode14/_Probabilidad/100;
					_Acertantes =Recaudacion/PrecioApuesta*_Probabilidad;
					TxProbabilidad.Text = _Probabilidad.ToString();
					TxPremio.Text = _Premio.ToString() ;
					TxAcertantes.Text = _Acertantes.ToString ();
				}
			}		
		}

		private void comboBox1_SelectedIndexChanged_1(object sender, System.EventArgs e)
		{
			ComboBox MiCombo = (ComboBox) sender;
			switch (MiCombo.SelectedIndex )
			{
				case 0:TxLN.Text = "0"; break;
				case 1:TxLN.Text = "-14,307956"; break;
				case 2:TxLN.Text = "-14,384536"; break;
				case 3:TxLN.Text = "-14,436448"; break;
				case 4:TxLN.Text = "-14,557393"; break;
				case 5:TxLN.Text = "-14,766477"; break;
				default: break;
			}
			statusBarPanel4.Text = "";

		}

		private void tabControl1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			TabControl MiTab = (TabControl) sender;
			switch (MiTab.SelectedIndex )
			{
				case 0://lblApostados.Text = "% apostados";
					txtLimiteProbAcum.Visible =true;
					checkValorOrdenacion.Text ="A�adir probabilidad acumulada";
					chkValorPremio14 .Visible =true;
					break;
				case 1://lblApostados.Text = "valoraciones"; 
					checkValorOrdenacion.Text ="A�adir valoraci�n columnas";
					chkValorPremio14.Checked =false;
					txtLimiteProbAcum.Visible =false;
					chkValorPremio14.Visible =false;
					break;
				default: break;
			}
		}

		private void txMaxColumnas_TextChanged_1(object sender, System.EventArgs e)
		{
			if (txMaxColumnas.Text !="") MaxColumnas = Convert.ToInt32 (txMaxColumnas.Text);
		}

		private void GenericRB_CheckedChanged(object sender, System.EventArgs e)
		{
			if(rbPorProbabilidad.Checked )
			{
				lblLNMin.Text ="LN m�nimo";
				lblLNMax.Text ="LN m�ximo";
			}
			else
			{
				lblLNMin.Text ="Suma m�nima";
				lblLNMax.Text ="Suma m�xima";
			}
		}

		private void txColumna_TextChanged(object sender, System.EventArgs e)
		{
			if (txColumna.Enabled ==true)
			{
				_Probabilidad=1;
				string columna=txColumna.Text ;
				v=controlPorcentajes1 .Valores ;
				for (int i=0;i<columna.Length ;i++)
				{
					switch(columna[i])
					{
						case '1': _Probabilidad *= v [i,0]/100;break;
						case '2': _Probabilidad *= v [i,2]/100;break;
						default: _Probabilidad *= v [i,1]/100;break;
					}
				}
				_LN=Math.Log (_Probabilidad);
				_Premio = PrecioApuesta*PorcentajeDestinadoAlPremiode14/_Probabilidad/100;
				_Acertantes =Recaudacion/PrecioApuesta*_Probabilidad;
				TxLN.Text = _LN.ToString();
				TxPremio.Text = _Premio.ToString() ;
				TxAcertantes.Text = _Acertantes.ToString ();
				TxProbabilidad.Text =_Probabilidad.ToString ();
			}
		}

		private void txColumna_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			Valida.Generic_KeyPress (TipoValidacionString.Solo1X2, sender, e);
		}

		private void controlPorcentajes1_Modificado(object sender, System.EventArgs e)
		{
			statusBarPanel2.Text = Path.GetFileNameWithoutExtension (controlPorcentajes1 .archivoPorcentajes);
			Application.DoEvents();
		}

	}
}
