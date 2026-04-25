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
	/// Summary description for RentabilidadFrm.
	/// </summary>
	public class RentabilidadFrm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button btnOK;
		private System.Windows.Forms.Button btnCancel;
		private System.ComponentModel.IContainer components=null;
		
		private double[,] p = new double [14,3];
		private double[,] v = new double [14,3];
		private float[,] pr = new float [14,3];
		private float[,] pa = new float [14,3];
		private int[] pot = new int [] {1,3,9,27,81,243,729,2187,6561,19683,59049,177147,531441,1594323};

		private float[,] Cra = new float [14,3];
		private float[,] Crp = new float [14,3];
		private ApuestaProbable[] Ap14T=new ApuestaProbable[(int)Math.Pow(3,VariablesGlobales.NumeroPartidos)];
		private BitArray Bits = new BitArray(4782969,false);
		private double Recaudacion;
		private double PrecioApuesta;
		private double PorcentajeDestinadoAlPremiode14;
        private string moneda = "";
		private float PremioTope;
		private float PremioDe14;
		private float Premio;
		private float Esperanza;
		private int Profundidad=0;
		private ValidadorCadenas Valida= new ValidadorCadenas();
		private double EMmin;
		private double EMmax;
		private int NumCols;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Button btnCalculoVal;
		private System.Windows.Forms.TextBox txtColumna;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label32;
		private System.Windows.Forms.Label label33;
		private System.Windows.Forms.GroupBox grLimitesEM;
		private System.Windows.Forms.Label label37;
		private System.Windows.Forms.TextBox txEMmin;
		private System.Windows.Forms.Label label38;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label36;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.TextBox txProbApostada;
		private System.Windows.Forms.TextBox txProbReal;
		private System.Windows.Forms.TextBox txEM;
		private System.Windows.Forms.TextBox txEMmax;
		private System.Windows.Forms.TextBox txFicheroSalida;
		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.StatusBarPanel statusBarPanel1;
		private System.Windows.Forms.StatusBarPanel statusBarPanel2;
		private System.Windows.Forms.StatusBarPanel statusBarPanel3;
		private System.Windows.Forms.StatusBarPanel statusBarPanel4;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.RadioButton rb14Triples;
		private System.Windows.Forms.RadioButton rbFichero;
		private System.Windows.Forms.TextBox txFicheroEntrada;
		private System.Windows.Forms.Button btOpenFicheroEntrada;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.CheckBox chkPonerEM;
		private System.Windows.Forms.CheckBox chkOrdenar;
		private System.Windows.Forms.StatusBarPanel statusBarPanel5;
		private System.Windows.Forms.StatusBarPanel statusBarPanel6;
		private System.Windows.Forms.Label lblRecaudacion;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label label34;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private Free1X2.UI.Controls.ControlPorcentajes controlPorcentajesApostados;
		private Free1X2.UI.Controls.ControlPorcentajes controlPorcentajesReales;
		private System.Windows.Forms.Label label39;
		private bool salidaBinaria=false;

		public RentabilidadFrm()
		{
			InitializeComponent();
			statusBar1.ShowPanels =true;

			PrecioApuesta = VariablesGlobales.PrecioApuesta;
		    PorcentajeDestinadoAlPremiode14 = VariablesGlobales.Porcentaje14;
		    Recaudacion = VariablesGlobales.Recaudacion;
		    moneda = VariablesGlobales.Moneda;
            
            textBox1.Text =Recaudacion.ToString ();
			PremioDe14 = (float)PrecioApuesta * (float)PorcentajeDestinadoAlPremiode14/100;
            var fH = new FormulariosHelper();
            fH.Traducir(this);
        }
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
        }


		protected void CalcularValoracionColumna()
		{
			float[] p14 = new float [2];
			string Apuesta=txtColumna.Text ;
			short Partido;

			
			// cargamos los % de las cajas de texto a variables
			
			v=controlPorcentajesApostados .Valores ;
			p=controlPorcentajesReales .Valores ;
			
			// -- convertimos a tanto por uno ---
			
			Porcentajes PctA = new Porcentajes(v);
			pa=  PctA.ValoresBase100();
			Porcentajes Pct = new Porcentajes(p);
			pr= Pct.ValoresBase100();
			
			// --- iniciamos las probabilidades a 1 ---
			
			p14[0] = 1; //Probabilidad apostada
			p14[1] = 1; //Probabilidad real
			
			//---- probabilidad del 14 de la apuesta -----
			
			for (Partido = 0;Partido<14;Partido++)
			{
				switch (Apuesta[Partido])
				{
					case '1':	p14[0] *=  pa[Partido, 0];
								p14[1] *=  pr[Partido, 0];
								break;
					case '2':	p14[0] *=  pa[Partido, 2];
								p14[1] *=  pr[Partido, 2];
								break;
					default:	p14[0] *=  pa[Partido, 1];
								p14[1] *=  pr[Partido, 1];
								break;
				}
			}

			Premio = PremioDe14 / p14[0];
			if( Premio > PremioTope) Premio = PremioTope;
			Esperanza = Premio * p14[1];

			txProbApostada.Text =(Math.Round (PrecioApuesta * this.PorcentajeDestinadoAlPremiode14 /100/p14[0],2)).ToString () + " ";
			txProbReal.Text =p14[1].ToString ();
			txEM.Text  =Esperanza.ToString () + " ";
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RentabilidadFrm));
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.txProbApostada = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.btnCalculoVal = new System.Windows.Forms.Button();
            this.txtColumna = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label33 = new System.Windows.Forms.Label();
            this.txEM = new System.Windows.Forms.TextBox();
            this.txProbReal = new System.Windows.Forms.TextBox();
            this.label32 = new System.Windows.Forms.Label();
            this.grLimitesEM = new System.Windows.Forms.GroupBox();
            this.txEMmax = new System.Windows.Forms.TextBox();
            this.label38 = new System.Windows.Forms.Label();
            this.txEMmin = new System.Windows.Forms.TextBox();
            this.label37 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btOpenFicheroEntrada = new System.Windows.Forms.Button();
            this.txFicheroEntrada = new System.Windows.Forms.TextBox();
            this.rbFichero = new System.Windows.Forms.RadioButton();
            this.rb14Triples = new System.Windows.Forms.RadioButton();
            this.txFicheroSalida = new System.Windows.Forms.TextBox();
            this.label36 = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.label39 = new System.Windows.Forms.Label();
            this.statusBar1 = new System.Windows.Forms.StatusBar();
            this.statusBarPanel1 = new System.Windows.Forms.StatusBarPanel();
            this.statusBarPanel2 = new System.Windows.Forms.StatusBarPanel();
            this.statusBarPanel3 = new System.Windows.Forms.StatusBarPanel();
            this.statusBarPanel4 = new System.Windows.Forms.StatusBarPanel();
            this.statusBarPanel5 = new System.Windows.Forms.StatusBarPanel();
            this.statusBarPanel6 = new System.Windows.Forms.StatusBarPanel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.chkPonerEM = new System.Windows.Forms.CheckBox();
            this.chkOrdenar = new System.Windows.Forms.CheckBox();
            this.lblRecaudacion = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label34 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.controlPorcentajesReales = new Free1X2.UI.Controls.ControlPorcentajes();
            this.controlPorcentajesApostados = new Free1X2.UI.Controls.ControlPorcentajes();
            this.groupBox2.SuspendLayout();
            this.grLimitesEM.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            // StatusBarPanel controls don't support ISupportInitialize in .NET 8
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).BeginInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel2)).BeginInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel3)).BeginInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel4)).BeginInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel5)).BeginInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel6)).BeginInit();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnOK.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnOK.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnOK.Image = ((System.Drawing.Image)(resources.GetObject("btnOK.Image")));
            this.btnOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOK.Location = new System.Drawing.Point(256, 496);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 32);
            this.btnOK.TabIndex = 56;
            this.btnOK.Text = "&Calcular";
            this.btnOK.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnOK.UseVisualStyleBackColor = false;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
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
            this.btnCancel.Location = new System.Drawing.Point(364, 496);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 32);
            this.btnCancel.TabIndex = 57;
            this.btnCancel.Text = "Cance&lar";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // txProbApostada
            // 
            this.txProbApostada.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txProbApostada.BackColor = System.Drawing.SystemColors.Info;
            this.txProbApostada.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txProbApostada.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txProbApostada.Location = new System.Drawing.Point(148, 49);
            this.txProbApostada.MaxLength = 20;
            this.txProbApostada.Name = "txProbApostada";
            this.txProbApostada.ReadOnly = true;
            this.txProbApostada.Size = new System.Drawing.Size(128, 20);
            this.txProbApostada.TabIndex = 5;
            // 
            // label21
            // 
            this.label21.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label21.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.Location = new System.Drawing.Point(16, 28);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(84, 20);
            this.label21.TabIndex = 4;
            this.label21.Text = "Columna";
            // 
            // label17
            // 
            this.label17.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label17.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(16, 49);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(132, 20);
            this.label17.TabIndex = 2;
            this.label17.Text = "Premio estimado de 14";
            // 
            // btnCalculoVal
            // 
            this.btnCalculoVal.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCalculoVal.BackColor = System.Drawing.Color.LightSalmon;
            this.btnCalculoVal.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCalculoVal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCalculoVal.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnCalculoVal.Image = ((System.Drawing.Image)(resources.GetObject("btnCalculoVal.Image")));
            this.btnCalculoVal.Location = new System.Drawing.Point(241, 28);
            this.btnCalculoVal.Name = "btnCalculoVal";
            this.btnCalculoVal.Size = new System.Drawing.Size(24, 20);
            this.btnCalculoVal.TabIndex = 1;
            this.btnCalculoVal.UseVisualStyleBackColor = false;
            this.btnCalculoVal.Click += new System.EventHandler(this.btnCalculoVal_Click);
            // 
            // txtColumna
            // 
            this.txtColumna.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtColumna.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtColumna.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txtColumna.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtColumna.Location = new System.Drawing.Point(100, 28);
            this.txtColumna.MaxLength = 14;
            this.txtColumna.Name = "txtColumna";
            this.txtColumna.Size = new System.Drawing.Size(140, 20);
            this.txtColumna.TabIndex = 0;
            this.txtColumna.TextChanged += new System.EventHandler(this.txtColumna_TextChanged);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.BackColor = System.Drawing.Color.Bisque;
            this.groupBox2.Controls.Add(this.label33);
            this.groupBox2.Controls.Add(this.txEM);
            this.groupBox2.Controls.Add(this.txProbReal);
            this.groupBox2.Controls.Add(this.label32);
            this.groupBox2.Controls.Add(this.txProbApostada);
            this.groupBox2.Controls.Add(this.label21);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.btnCalculoVal);
            this.groupBox2.Controls.Add(this.txtColumna);
            this.groupBox2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox2.Location = new System.Drawing.Point(208, 308);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(296, 124);
            this.groupBox2.TabIndex = 64;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Calcular Valoración de una columna";
            // 
            // label33
            // 
            this.label33.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label33.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label33.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label33.Location = new System.Drawing.Point(16, 91);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(160, 20);
            this.label33.TabIndex = 9;
            this.label33.Text = "Esperanza matemática";
            // 
            // txEM
            // 
            this.txEM.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txEM.BackColor = System.Drawing.SystemColors.Info;
            this.txEM.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txEM.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txEM.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txEM.Location = new System.Drawing.Point(176, 91);
            this.txEM.MaxLength = 14;
            this.txEM.Name = "txEM";
            this.txEM.ReadOnly = true;
            this.txEM.Size = new System.Drawing.Size(100, 20);
            this.txEM.TabIndex = 8;
            // 
            // txProbReal
            // 
            this.txProbReal.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txProbReal.BackColor = System.Drawing.SystemColors.Info;
            this.txProbReal.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txProbReal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txProbReal.Location = new System.Drawing.Point(148, 70);
            this.txProbReal.MaxLength = 20;
            this.txProbReal.Name = "txProbReal";
            this.txProbReal.ReadOnly = true;
            this.txProbReal.Size = new System.Drawing.Size(128, 20);
            this.txProbReal.TabIndex = 7;
            // 
            // label32
            // 
            this.label32.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label32.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label32.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label32.Location = new System.Drawing.Point(16, 70);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(132, 20);
            this.label32.TabIndex = 6;
            this.label32.Text = "Probabilidad Real";
            // 
            // grLimitesEM
            // 
            this.grLimitesEM.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grLimitesEM.BackColor = System.Drawing.Color.Bisque;
            this.grLimitesEM.Controls.Add(this.txEMmax);
            this.grLimitesEM.Controls.Add(this.label38);
            this.grLimitesEM.Controls.Add(this.txEMmin);
            this.grLimitesEM.Controls.Add(this.label37);
            this.grLimitesEM.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grLimitesEM.ForeColor = System.Drawing.Color.Maroon;
            this.grLimitesEM.Location = new System.Drawing.Point(208, 240);
            this.grLimitesEM.Name = "grLimitesEM";
            this.grLimitesEM.Size = new System.Drawing.Size(296, 64);
            this.grLimitesEM.TabIndex = 154;
            this.grLimitesEM.TabStop = false;
            this.grLimitesEM.Text = "Límites Esperanza Matemática";
            // 
            // txEMmax
            // 
            this.txEMmax.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txEMmax.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txEMmax.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txEMmax.Location = new System.Drawing.Point(208, 24);
            this.txEMmax.Name = "txEMmax";
            this.txEMmax.Size = new System.Drawing.Size(44, 20);
            this.txEMmax.TabIndex = 3;
            this.txEMmax.Text = "50";
            this.txEMmax.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            // 
            // label38
            // 
            this.label38.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label38.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label38.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label38.Location = new System.Drawing.Point(160, 24);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(48, 20);
            this.label38.TabIndex = 2;
            this.label38.Text = "Máximo";
            this.label38.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txEMmin
            // 
            this.txEMmin.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txEMmin.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txEMmin.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txEMmin.Location = new System.Drawing.Point(96, 24);
            this.txEMmin.Name = "txEMmin";
            this.txEMmin.Size = new System.Drawing.Size(44, 20);
            this.txEMmin.TabIndex = 1;
            this.txEMmin.Text = "0,133";
            this.txEMmin.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            // 
            // label37
            // 
            this.label37.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label37.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label37.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label37.Location = new System.Drawing.Point(40, 24);
            this.label37.Name = "label37";
            this.label37.Size = new System.Drawing.Size(56, 20);
            this.label37.TabIndex = 0;
            this.label37.Text = "Mínimo";
            this.label37.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.Color.Bisque;
            this.groupBox1.Controls.Add(this.groupBox3);
            this.groupBox1.Controls.Add(this.txFicheroSalida);
            this.groupBox1.Controls.Add(this.label36);
            this.groupBox1.Controls.Add(this.button4);
            this.groupBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox1.Location = new System.Drawing.Point(8, 8);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(692, 112);
            this.groupBox1.TabIndex = 155;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Ficheros";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.btOpenFicheroEntrada);
            this.groupBox3.Controls.Add(this.txFicheroEntrada);
            this.groupBox3.Controls.Add(this.rbFichero);
            this.groupBox3.Controls.Add(this.rb14Triples);
            this.groupBox3.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox3.Location = new System.Drawing.Point(16, 20);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(672, 52);
            this.groupBox3.TabIndex = 163;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Origen de las columnas";
            // 
            // btOpenFicheroEntrada
            // 
            this.btOpenFicheroEntrada.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btOpenFicheroEntrada.BackColor = System.Drawing.Color.Silver;
            this.btOpenFicheroEntrada.Enabled = false;
            this.btOpenFicheroEntrada.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btOpenFicheroEntrada.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btOpenFicheroEntrada.Image = ((System.Drawing.Image)(resources.GetObject("btOpenFicheroEntrada.Image")));
            this.btOpenFicheroEntrada.Location = new System.Drawing.Point(636, 20);
            this.btOpenFicheroEntrada.Name = "btOpenFicheroEntrada";
            this.btOpenFicheroEntrada.Size = new System.Drawing.Size(24, 20);
            this.btOpenFicheroEntrada.TabIndex = 157;
            this.btOpenFicheroEntrada.UseVisualStyleBackColor = false;
            this.btOpenFicheroEntrada.Click += new System.EventHandler(this.btOpenFicheroEntrada_Click);
            this.btOpenFicheroEntrada.EnabledChanged += new System.EventHandler(this.btOpenFicheroEntrada_EnabledChanged);
            // 
            // txFicheroEntrada
            // 
            this.txFicheroEntrada.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txFicheroEntrada.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txFicheroEntrada.Enabled = false;
            this.txFicheroEntrada.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txFicheroEntrada.Location = new System.Drawing.Point(168, 20);
            this.txFicheroEntrada.Name = "txFicheroEntrada";
            this.txFicheroEntrada.Size = new System.Drawing.Size(460, 20);
            this.txFicheroEntrada.TabIndex = 2;
            // 
            // rbFichero
            // 
            this.rbFichero.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rbFichero.Location = new System.Drawing.Point(108, 20);
            this.rbFichero.Name = "rbFichero";
            this.rbFichero.Size = new System.Drawing.Size(60, 16);
            this.rbFichero.TabIndex = 1;
            this.rbFichero.Text = "Fichero";
            this.rbFichero.CheckedChanged += new System.EventHandler(this.rbFichero_CheckedChangedGenerico);
            // 
            // rb14Triples
            // 
            this.rb14Triples.Checked = true;
            this.rb14Triples.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rb14Triples.Location = new System.Drawing.Point(12, 20);
            this.rb14Triples.Name = "rb14Triples";
            this.rb14Triples.Size = new System.Drawing.Size(84, 16);
            this.rb14Triples.TabIndex = 0;
            this.rb14Triples.TabStop = true;
            this.rb14Triples.Text = "14 Triples";
            this.rb14Triples.CheckedChanged += new System.EventHandler(this.rbFichero_CheckedChangedGenerico);
            // 
            // txFicheroSalida
            // 
            this.txFicheroSalida.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txFicheroSalida.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txFicheroSalida.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txFicheroSalida.Location = new System.Drawing.Point(184, 80);
            this.txFicheroSalida.Name = "txFicheroSalida";
            this.txFicheroSalida.Size = new System.Drawing.Size(460, 20);
            this.txFicheroSalida.TabIndex = 162;
            // 
            // label36
            // 
            this.label36.BackColor = System.Drawing.Color.Bisque;
            this.label36.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label36.Location = new System.Drawing.Point(16, 80);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(140, 20);
            this.label36.TabIndex = 161;
            this.label36.Text = "Salida de resultados";
            this.label36.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button4
            // 
            this.button4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button4.BackColor = System.Drawing.Color.LightSalmon;
            this.button4.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button4.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button4.Image = ((System.Drawing.Image)(resources.GetObject("button4.Image")));
            this.button4.Location = new System.Drawing.Point(652, 80);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(24, 20);
            this.button4.TabIndex = 156;
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // label39
            // 
            this.label39.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label39.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label39.ForeColor = System.Drawing.Color.Maroon;
            this.label39.Location = new System.Drawing.Point(196, 132);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(324, 76);
            this.label39.TabIndex = 156;
            this.label39.Text = "Esta utilidad obtiene la Esperanza matemática de premio (EM) a partir de la proba" +
                "bilidad real y las frecuencias apostadas";
            this.label39.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // statusBar1
            // 
            this.statusBar1.Location = new System.Drawing.Point(0, 540);
            this.statusBar1.Name = "statusBar1";
            this.statusBar1.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.statusBarPanel1,
            this.statusBarPanel2,
            this.statusBarPanel3,
            this.statusBarPanel4,
            this.statusBarPanel5,
            this.statusBarPanel6});
            this.statusBar1.Size = new System.Drawing.Size(712, 22);
            this.statusBar1.TabIndex = 164;
            this.statusBar1.Text = "statusBar1";
            // 
            // statusBarPanel1
            // 
            this.statusBarPanel1.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.statusBarPanel1.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.None;
            this.statusBarPanel1.Name = "statusBarPanel1";
            this.statusBarPanel1.Text = "Fich. % apost.";
            this.statusBarPanel1.Width = 86;
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
            this.statusBarPanel3.Text = "Fich. % Real";
            this.statusBarPanel3.Width = 78;
            // 
            // statusBarPanel4
            // 
            this.statusBarPanel4.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
            this.statusBarPanel4.Name = "statusBarPanel4";
            // 
            // statusBarPanel5
            // 
            this.statusBarPanel5.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.statusBarPanel5.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.None;
            this.statusBarPanel5.Name = "statusBarPanel5";
            this.statusBarPanel5.Text = "Estado";
            this.statusBarPanel5.Width = 49;
            // 
            // statusBarPanel6
            // 
            this.statusBarPanel6.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.statusBarPanel6.Name = "statusBarPanel6";
            this.statusBarPanel6.Width = 10;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.BackColor = System.Drawing.Color.Bisque;
            this.groupBox4.Controls.Add(this.chkPonerEM);
            this.groupBox4.Controls.Add(this.chkOrdenar);
            this.groupBox4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox4.Location = new System.Drawing.Point(184, 440);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(348, 48);
            this.groupBox4.TabIndex = 167;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Opciones de salida";
            // 
            // chkPonerEM
            // 
            this.chkPonerEM.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkPonerEM.Location = new System.Drawing.Point(184, 20);
            this.chkPonerEM.Name = "chkPonerEM";
            this.chkPonerEM.Size = new System.Drawing.Size(160, 20);
            this.chkPonerEM.TabIndex = 168;
            this.chkPonerEM.Text = "Añadir EM al fichero salida";
            // 
            // chkOrdenar
            // 
            this.chkOrdenar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkOrdenar.Location = new System.Drawing.Point(12, 19);
            this.chkOrdenar.Name = "chkOrdenar";
            this.chkOrdenar.Size = new System.Drawing.Size(156, 20);
            this.chkOrdenar.TabIndex = 167;
            this.chkOrdenar.Text = "Ordenar columnas por EM";
            // 
            // lblRecaudacion
            // 
            this.lblRecaudacion.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblRecaudacion.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblRecaudacion.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRecaudacion.Location = new System.Drawing.Point(208, 216);
            this.lblRecaudacion.Name = "lblRecaudacion";
            this.lblRecaudacion.Size = new System.Drawing.Size(164, 20);
            this.lblRecaudacion.TabIndex = 280;
            this.lblRecaudacion.Text = "Recaudación considerada";
            this.lblRecaudacion.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(372, 216);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(80, 21);
            this.textBox1.TabIndex = 281;
            this.textBox1.Text = "8000000";
            this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label34
            // 
            this.label34.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label34.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label34.Location = new System.Drawing.Point(456, 220);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(24, 16);
            this.label34.TabIndex = 282;
            this.label34.Text = "";
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 132);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(164, 24);
            this.label1.TabIndex = 850;
            this.label1.Text = "Valoraciones apostadas";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(532, 132);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(164, 24);
            this.label2.TabIndex = 851;
            this.label2.Text = "Valoraciones reales";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // controlPorcentajesReales
            // 
            this.controlPorcentajesReales.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.controlPorcentajesReales.archivoPorcentajes = null;
            this.controlPorcentajesReales.BackColor = System.Drawing.Color.Bisque;
            this.controlPorcentajesReales.CaptionText = "  P O R C E N T A J E S";
            this.controlPorcentajesReales.FormatoFicheroValoraciones = ((short)(0));
            this.controlPorcentajesReales.Jornada = "01";
            this.controlPorcentajesReales.Location = new System.Drawing.Point(536, 156);
            this.controlPorcentajesReales.Name = "controlPorcentajesReales";
            this.controlPorcentajesReales.ReadOnly = false;
            this.controlPorcentajesReales.Size = new System.Drawing.Size(160, 372);
            this.controlPorcentajesReales.TabIndex = 853;
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
            this.controlPorcentajesApostados.Location = new System.Drawing.Point(16, 156);
            this.controlPorcentajesApostados.Name = "controlPorcentajesApostados";
            this.controlPorcentajesApostados.ReadOnly = false;
            this.controlPorcentajesApostados.Size = new System.Drawing.Size(160, 372);
            this.controlPorcentajesApostados.TabIndex = 852;
            this.controlPorcentajesApostados.Temporada = "2004/2005";
            this.controlPorcentajesApostados.Modificado += new System.EventHandler(this.controlPorcentajesApostados_Modificado);
            // 
            // RentabilidadFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(712, 562);
            this.Controls.Add(this.controlPorcentajesReales);
            this.Controls.Add(this.controlPorcentajesApostados);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label34);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.lblRecaudacion);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.statusBar1);
            this.Controls.Add(this.label39);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.grLimitesEM);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(720, 596);
            this.Name = "RentabilidadFrm";
            this.ShowInTaskbar = false;
            this.Text = "Rentabilidad";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.grLimitesEM.ResumeLayout(false);
            this.grLimitesEM.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            // StatusBarPanel controls don't support ISupportInitialize in .NET 8
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).EndInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel2)).EndInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel3)).EndInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel4)).EndInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel5)).EndInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel6)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void btnCancel_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void btnOK_Click(object sender, System.EventArgs e)
		{

			Cursor.Current = Cursors.WaitCursor;
			
			//-- Inicializamos el array por si ejecutamos por 2ª vez --
			
			for (int i=0;i<4782969;i++)
			{
				Ap14T[i].Columna =i;
				Ap14T[i].Probabilidad =0;
			}
			
			// Cargamos las columnas a analizar
			
			if(this.rbFichero.Checked )
			{
				statusBarPanel6.Text ="Leyendo columnas...";
				Application.DoEvents ();
				LeerColumnas();		// Fichero
			}
			else
			{
				Bits.SetAll (true); // 14 triples
			}
			
			// Iniciamos las variables
			
			float[] p14 = new float [2];
			string Apuesta=txtColumna.Text ;
			short Partido;
			statusBarPanel6.Text ="Calculando...";
			Application.DoEvents ();

			
			// pasamos los % de las cajas de texto a las variables
			
			v=controlPorcentajesApostados .Valores ;
			p=controlPorcentajesReales .Valores ;
			
			// -- convertimos a tanto por uno ---
			
			Porcentajes PctA = new Porcentajes(v);
			pa=  PctA.ValoresBase100();
			Porcentajes Pct = new Porcentajes(p);
			pr= Pct.ValoresBase100();

  			p14[0] = 1; //Probabilidad apostada
			p14[1] = 1; //Probabilidad real
			
			//-- probabilidad de la apuesta 11111111111111 --
			

			for (Partido = 0;Partido<14;Partido++)
			{
				p14[0] *=  pa[Partido, 0];
				p14[1] *=  pr[Partido, 0];
				
				//--valores cuando se falla cada uno de los signos, a usar para evaluar los 13's-----
				
				Cra[Partido, 0] = pa[Partido, 1] / pa[Partido, 0];
				Cra[Partido, 1] = pa[Partido, 2] / pa[Partido, 0];
				Crp[Partido, 0] = pr[Partido, 1] / pr[Partido, 0];
				Crp[Partido, 1] = pr[Partido, 2] / pr[Partido, 0];
			}


			Profundidad=0;
			Premio = PremioDe14 / p14[0];
			if( Premio > PremioTope) Premio = PremioTope;
			Esperanza = Premio * p14[1];
			if (Bits[0])
			{
				Ap14T[0].Columna = 0;
				Ap14T[0].Probabilidad = -Esperanza;
			}
			else
			{
				Ap14T[0].Probabilidad = (float) 3E+7;
			}

			EncontrarDistantes1(p14[0], p14[1],0, 0,14);
	
			if(chkOrdenar.Checked ) 
			{
				statusBarPanel6.Text ="Ordenando...";
				Application.DoEvents ();
				ordena (0,4782968);
			}
			statusBarPanel6.Text ="Grabando...";
			Application.DoEvents ();

			GrabacionColumnas();
			statusBarPanel6.Text ="Finalizado ("+ NumCols.ToString() + " columnas)";
			Cursor.Current = Cursors.Default;
		}

		private void EncontrarDistantes1(float pProbA,float pProbR,int IndiceInicial, int PosicionInicial,int pProfundidad)
		{
			int Partido;
			int z;
			int Indice;
			float ProbA;
			float ProbR;
			Profundidad++;
        
			//'--encontramos las apuestas que se diferencian en un solo signo ----
			for (Partido = PosicionInicial;Partido<14;Partido++)
			{
				for (z = 0;z<2;z++)
				{
					Indice = IndiceInicial + pot[Partido] * (z+1);
					ProbA = pProbA * Cra[Partido, z];
					ProbR = pProbR * Crp[Partido, z];
					Premio = PremioDe14 / ProbA;
					if( Premio > PremioTope) Premio = PremioTope;
					Esperanza = Premio * ProbR;
					if (Bits[Indice])
					{
						Ap14T[Indice].Columna = Indice;
						Ap14T[Indice].Probabilidad = -Esperanza;
					}
					if (Profundidad < pProfundidad)
					{
						EncontrarDistantes1 (ProbA,ProbR, Indice, Partido + 1, pProfundidad);
					}
				}
			}
			Profundidad--;
		}


		private void btnCalculoVal_Click(object sender, System.EventArgs e)
		{
			CalcularValoracionColumna();
		}
		private void textBoxgenerico_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			Valida.Generic_KeyPress (TipoValidacionString.SoloNumerosDecimales, sender, e);
		}
		private void textBoxgenericoDeRangos_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			Valida.Generic_KeyPress (TipoValidacionString.SoloNumerosYsignosEsp, sender, e);
		}

		private void HabilitarCalcular()
		{
			btnOK.Enabled = false;
			btnCalculoVal.Enabled =false;
			if (this.rbFichero.Checked)
			{
				if (this.txFicheroEntrada.Text !="" && txFicheroSalida.Text != "" ) btnOK.Enabled = true;
				if (txtColumna.Text.Length ==14) btnCalculoVal.Enabled = true;
			}
			else
			{
				if (txFicheroSalida.Text != "" ) btnOK.Enabled = true;
				if (txtColumna.Text.Length ==14) btnCalculoVal.Enabled = true;
			}
		}

		private void button4_Click(object sender, System.EventArgs e)
		{
			SaveFileDialog abreFiltroDialog = new SaveFileDialog();
			abreFiltroDialog.InitialDirectory = "Columnas\\" ;
			abreFiltroDialog.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			if(abreFiltroDialog.ShowDialog () == DialogResult.OK)
			{
				salidaBinaria=abreFiltroDialog.FilterIndex==2;
				string archivoSalida = abreFiltroDialog.FileName;		    	
				txFicheroSalida.Text = archivoSalida;
			}
			HabilitarCalcular ();
		}
		private void rbFichero_CheckedChangedGenerico(object sender, System.EventArgs e)
		{
			if(rbFichero.Checked )
			{
				this.txFicheroEntrada.Enabled =true;
				btOpenFicheroEntrada.Enabled =true;
			}
			else
			{
				this.txFicheroEntrada.Enabled =false;
				btOpenFicheroEntrada.Enabled =false;
			}
		}

		private void txtColumna_TextChanged(object sender, System.EventArgs e)
		{
			HabilitarCalcular ();
		}
		private void LeerColumnas() 
		{
            IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(txFicheroEntrada.Text);
			Bits=comBaseCols.LeerTodasColsABitArray(14);
			comBaseCols.Cerrar();						
		}

		private void GrabacionColumnas()
		{						
			string archivoSalida=txFicheroSalida.Text ;

			EMmin= -Convert.ToDouble (txEMmin.Text);
			EMmax= -Convert.ToDouble (txEMmax.Text);
			ConvertidorDeBases col =new ConvertidorDeBases();
            IArchivoColumnas comCols = new ArchivoColumnasTexto(archivoSalida);
			NumCols=0;
			if (this.chkPonerEM.Checked )
			{
				for (int nr=0; nr< 4782969; nr++) 
				{
					if(Bits[Ap14T[nr].Columna] && Ap14T[nr].Probabilidad >= EMmax && Ap14T[nr].Probabilidad <= EMmin)
					{comCols.GuardarColsComa (col.ConvNumAColumna(Ap14T[nr].Columna) + (char) 9 + (-Ap14T[nr].Probabilidad ));NumCols++;}
				}	
			}
			else
			{
				for (int nr=0; nr< 4782969; nr++) 
				{
					if(Bits[Ap14T[nr].Columna] && Ap14T[nr].Probabilidad >= EMmax && Ap14T[nr].Probabilidad <= EMmin)
					{comCols.GuardarCols(col.ConvNumAColumna(Ap14T[nr].Columna));NumCols++;}
				}
			}
			comCols.Cerrar();	
		}
		private void ordena(int izq, int der)
		{ 
			int i = 0, j = 0; 
			ApuestaProbable x = new ApuestaProbable();
			ApuestaProbable aux= new ApuestaProbable();
			i = izq; 
			j = der; 
			x = Ap14T [ (izq + der) /2 ]; 
			
			do
			{ 
			while( (Ap14T[i].Probabilidad  < x.Probabilidad ) && (j <= der) )
			{
				i++;
			} 
			while( (x.Probabilidad  < Ap14T[j].Probabilidad ) && (j > izq) )
			{ 
				j--;
			} 
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

		private void btOpenFicheroEntrada_Click(object sender, System.EventArgs e)
		{
			OpenFileDialog abreFiltroDialog = new OpenFileDialog();
			abreFiltroDialog.InitialDirectory = "Columnas\\" ;
			abreFiltroDialog.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			if(abreFiltroDialog.ShowDialog () == DialogResult.OK)
			{		    	
				string archivoEntrada = abreFiltroDialog.FileName;		    	
				txFicheroEntrada.Text = archivoEntrada;
			}
			HabilitarCalcular ();
		}

		private void textBox1_TextChanged(object sender, System.EventArgs e)
		{
			Recaudacion=Convert.ToDouble (textBox1.Text);
			PremioTope=(float)Recaudacion * (float)PorcentajeDestinadoAlPremiode14/100;
		}

		private void controlPorcentajesApostados_Modificado(object sender, System.EventArgs e)
		{
			if(this.controlPorcentajesReales.archivoPorcentajes==null && controlPorcentajesApostados.FormatoFicheroValoraciones ==43)
			{
				controlPorcentajesReales.archivoPorcentajes = controlPorcentajesApostados.archivoPorcentajes  ;
				controlPorcentajesReales.FormatoFicheroValoraciones = controlPorcentajesApostados.FormatoFicheroValoraciones ;
				controlPorcentajesReales.Refresca() ;
			}
			statusBarPanel2.Text = Path.GetFileName(controlPorcentajesApostados.archivoPorcentajes);
			Application.DoEvents();
		}

		private void controlPorcentajesReales_Modificado(object sender, System.EventArgs e)
		{
			if(controlPorcentajesApostados.archivoPorcentajes==null && controlPorcentajesReales.FormatoFicheroValoraciones ==43)
			{
				controlPorcentajesApostados.archivoPorcentajes = controlPorcentajesReales.archivoPorcentajes  ;
				controlPorcentajesApostados.FormatoFicheroValoraciones = controlPorcentajesReales.FormatoFicheroValoraciones ;
				controlPorcentajesApostados.Refresca() ;
			}
			statusBarPanel4.Text = Path.GetFileName(controlPorcentajesReales.archivoPorcentajes);
			Application.DoEvents();

		}

		private void btOpenFicheroEntrada_EnabledChanged(object sender, System.EventArgs e)
		{
            FormulariosHelper f = new FormulariosHelper();
			f.CambiarFondoBoton(btOpenFicheroEntrada);
		}
	}
}
