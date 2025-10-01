using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Text;
using System.IO;
using Free1X2.Utils ;
using Free1X2.EntradaSalida;
using Free1X2.UI.Controls;
using Free1X2.Analisis;


namespace Free1X2.UI
{
	/// <summary>
	/// Summary description for RotacionDeSignos.
	/// </summary>


	public class RotacionDeSignosFrm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.TextBox TxFicheroEntrada;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.TextBox TxFicheroSalida;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btCancelar;
		private System.Windows.Forms.Button btAceptar;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private string archivoEntrada="";
		private string archivoSalida="";
        private BitArray Bits = new BitArray(14348907, false);
        private BitArray BitsCambiados = new BitArray(14348907, false);
		private int NumApuestas;
		private int[] Signos = new int[16];
        private int[,] NuevosSignos = new int[16, 3];
        private double[,] PorcentajesSignos = new double[16, 3];
        private int[] pot = new int[] { 1, 3, 9, 27, 81, 243, 729, 2187, 6561, 19683, 59049, 177147, 531441, 1594323, 4782969, 14348907 };
		private string ColumnaBase;
        private double[,] v = new double[16, 3];
		private ValidadorCadenas Valida= new ValidadorCadenas();
		private Porcentajes Pct =new Porcentajes ();
		private System.Windows.Forms.Label lblBase9;
		private System.Windows.Forms.Label lblBase8;
		private System.Windows.Forms.Label lblBase7;
		private System.Windows.Forms.Label lblBase6;
		private System.Windows.Forms.Label lblBase5;
		private System.Windows.Forms.Label lblBase4;
		private System.Windows.Forms.Label lblBase3;
		private System.Windows.Forms.Label lblBase2;
		private System.Windows.Forms.Label lblBase1;
		private System.Windows.Forms.Label lblBase14;
		private System.Windows.Forms.Label lblBase13;
		private System.Windows.Forms.Label lblBase12;
		private System.Windows.Forms.Label lblBase11;
		private System.Windows.Forms.Label lblBase10;
		private System.Windows.Forms.Label lblNewBase141;
		private System.Windows.Forms.Label lblNewBase131;
		private System.Windows.Forms.Label lblNewBase121;
		private System.Windows.Forms.Label lblNewBase111;
		private System.Windows.Forms.Label lblNewBase101;
		private System.Windows.Forms.Label lblNewBase91;
		private System.Windows.Forms.Label lblNewBase81;
		private System.Windows.Forms.Label lblNewBase71;
		private System.Windows.Forms.Label lblNewBase61;
		private System.Windows.Forms.Label lblNewBase41;
		private System.Windows.Forms.Label lblNewBase31;
		private System.Windows.Forms.Label lblNewBase21;
		private System.Windows.Forms.Label lblNewBase11;
		private System.Windows.Forms.Label lblNewBase51;
		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.StatusBarPanel statusBarPanel1;
		private System.Windows.Forms.StatusBarPanel statusBarPanel2;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.Label lblNewBase14X;
		private System.Windows.Forms.Label lblNewBase13X;
		private System.Windows.Forms.Label lblNewBase12X;
		private System.Windows.Forms.Label lblNewBase11X;
        private System.Windows.Forms.Label lblNewBase10X;
		private System.Windows.Forms.Label lblNewBase8X;
		private System.Windows.Forms.Label lblNewBase7X;
		private System.Windows.Forms.Label lblNewBase6X;
		private System.Windows.Forms.Label lblNewBase5X;
		private System.Windows.Forms.Label lblNewBase4X;
		private System.Windows.Forms.Label lblNewBase3X;
		private System.Windows.Forms.Label lblNewBase2X;
		private System.Windows.Forms.Label lblNewBase1X;
		private System.Windows.Forms.Label lblNewBase142;
		private System.Windows.Forms.Label lblNewBase132;
		private System.Windows.Forms.Label lblNewBase122;
		private System.Windows.Forms.Label lblNewBase112;
		private System.Windows.Forms.Label lblNewBase102;
		private System.Windows.Forms.Label lblNewBase92;
		private System.Windows.Forms.Label lblNewBase82;
		private System.Windows.Forms.Label lblNewBase72;
		private System.Windows.Forms.Label lblNewBase62;
		private System.Windows.Forms.Label lblNewBase52;
		private System.Windows.Forms.Label lblNewBase42;
		private System.Windows.Forms.Label lblNewBase32;
		private System.Windows.Forms.Label lblNewBase22;
		private System.Windows.Forms.Label lblNewBase12;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.Label label23;
		private System.Windows.Forms.StatusBarPanel statusBarPanel3;
		private System.Windows.Forms.CheckBox chkGiros;
		private Vertical_Label label24;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Panel panel1;
		private Free1X2.UI.Controls.ControlPorcentajes controlPorcentajesCombinacion;
		private Free1X2.UI.Controls.ControlPorcentajes controlPorcentajesJornada;
		private bool salidaBinaria=false;
        private int partidos = 0;
        private Label lblNewBase162;
        private Label lblNewBase151;
        private Label lblNewBase152;
        private Label lblNewBase161;
        private Label lblNewBase16X;
        private Label lblNewBase15X;
        private Label lblNewBase9X;
        private Label lblBase15;
        private Label lblBase16;
        private Label label10;
        private Label label7;
        private Label label5;
        private Label label3;
        private Label[] Labels = null;
        private Label[] LabelsBase = null;

		private System.ComponentModel.Container components = null;

		public RotacionDeSignosFrm()
		{
			InitializeComponent();

            Labels = new Label[]{this.lblNewBase11, this.lblNewBase1X, this.lblNewBase12,
            this.lblNewBase21, this.lblNewBase2X, this.lblNewBase22,
            this.lblNewBase31, this.lblNewBase3X, this.lblNewBase32,
            this.lblNewBase41, this.lblNewBase4X, this.lblNewBase42,
            this.lblNewBase51, this.lblNewBase5X, this.lblNewBase52,
            this.lblNewBase61, this.lblNewBase6X, this.lblNewBase62,
            this.lblNewBase71, this.lblNewBase7X, this.lblNewBase72,
            this.lblNewBase81, this.lblNewBase8X, this.lblNewBase82,
            this.lblNewBase91, this.lblNewBase9X, this.lblNewBase92,
            this.lblNewBase101, this.lblNewBase10X, this.lblNewBase102,
            this.lblNewBase111, this.lblNewBase11X, this.lblNewBase112,
            this.lblNewBase121, this.lblNewBase12X, this.lblNewBase122,
            this.lblNewBase131, this.lblNewBase13X, this.lblNewBase132,
            this.lblNewBase141, this.lblNewBase14X, this.lblNewBase142,
            this.lblNewBase151, this.lblNewBase15X, this.lblNewBase152,
            this.lblNewBase161, this.lblNewBase16X, this.lblNewBase162};

            LabelsBase = new Label[]{this.lblBase1, this.lblBase2, this.lblBase3,
                this.lblBase4, this.lblBase5, this.lblBase6,
                this.lblBase7, this.lblBase8, this.lblBase9,
                this.lblBase10, this.lblBase11, this.lblBase12,
                this.lblBase13, this.lblBase14, this.lblBase15, this.lblBase16};
            
			statusBar1.ShowPanels =true;
			statusBarPanel2.Text ="Faltan datos";

            //controlPorcentajesCombinacion.Valores = PorcentajesSignos;
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RotacionDeSignosFrm));
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.TxFicheroEntrada = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.TxFicheroSalida = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
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
            this.lblNewBase14X = new System.Windows.Forms.Label();
            this.lblNewBase13X = new System.Windows.Forms.Label();
            this.lblNewBase12X = new System.Windows.Forms.Label();
            this.lblNewBase11X = new System.Windows.Forms.Label();
            this.lblNewBase10X = new System.Windows.Forms.Label();
            this.lblNewBase8X = new System.Windows.Forms.Label();
            this.lblNewBase7X = new System.Windows.Forms.Label();
            this.lblNewBase6X = new System.Windows.Forms.Label();
            this.lblNewBase5X = new System.Windows.Forms.Label();
            this.lblNewBase4X = new System.Windows.Forms.Label();
            this.lblNewBase3X = new System.Windows.Forms.Label();
            this.lblNewBase2X = new System.Windows.Forms.Label();
            this.lblNewBase1X = new System.Windows.Forms.Label();
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
            this.btCancelar = new System.Windows.Forms.Button();
            this.btAceptar = new System.Windows.Forms.Button();
            this.lblBase14 = new System.Windows.Forms.Label();
            this.lblBase13 = new System.Windows.Forms.Label();
            this.lblBase12 = new System.Windows.Forms.Label();
            this.lblBase11 = new System.Windows.Forms.Label();
            this.lblBase10 = new System.Windows.Forms.Label();
            this.lblBase9 = new System.Windows.Forms.Label();
            this.lblBase8 = new System.Windows.Forms.Label();
            this.lblBase7 = new System.Windows.Forms.Label();
            this.lblBase6 = new System.Windows.Forms.Label();
            this.lblBase5 = new System.Windows.Forms.Label();
            this.lblBase4 = new System.Windows.Forms.Label();
            this.lblBase3 = new System.Windows.Forms.Label();
            this.lblBase2 = new System.Windows.Forms.Label();
            this.lblBase1 = new System.Windows.Forms.Label();
            this.statusBar1 = new System.Windows.Forms.StatusBar();
            this.statusBarPanel1 = new System.Windows.Forms.StatusBarPanel();
            this.statusBarPanel2 = new System.Windows.Forms.StatusBarPanel();
            this.statusBarPanel3 = new System.Windows.Forms.StatusBarPanel();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.chkGiros = new System.Windows.Forms.CheckBox();
            this.button3 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblNewBase9X = new System.Windows.Forms.Label();
            this.lblNewBase162 = new System.Windows.Forms.Label();
            this.lblNewBase152 = new System.Windows.Forms.Label();
            this.lblNewBase16X = new System.Windows.Forms.Label();
            this.lblNewBase15X = new System.Windows.Forms.Label();
            this.lblNewBase161 = new System.Windows.Forms.Label();
            this.lblNewBase151 = new System.Windows.Forms.Label();
            this.lblBase15 = new System.Windows.Forms.Label();
            this.lblBase16 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.controlPorcentajesJornada = new Free1X2.UI.Controls.ControlPorcentajes();
            this.controlPorcentajesCombinacion = new Free1X2.UI.Controls.ControlPorcentajes();
            this.label24 = new Free1X2.UI.Controls.Vertical_Label();
            // StatusBarPanel controls don't support ISupportInitialize in .NET 8
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).BeginInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel2)).BeginInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel3)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Bisque;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(16, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(120, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "Fichero de entrada";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.BackColor = System.Drawing.Color.LightSalmon;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.Location = new System.Drawing.Point(598, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(24, 21);
            this.button1.TabIndex = 5;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // TxFicheroEntrada
            // 
            this.TxFicheroEntrada.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TxFicheroEntrada.BackColor = System.Drawing.Color.LemonChiffon;
            this.TxFicheroEntrada.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TxFicheroEntrada.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxFicheroEntrada.Location = new System.Drawing.Point(142, 12);
            this.TxFicheroEntrada.Name = "TxFicheroEntrada";
            this.TxFicheroEntrada.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.TxFicheroEntrada.Size = new System.Drawing.Size(455, 21);
            this.TxFicheroEntrada.TabIndex = 4;
            this.TxFicheroEntrada.Text = "(falta selección)";
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button2.BackColor = System.Drawing.Color.LightSalmon;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.SystemColors.ControlText;
            this.button2.Image = ((System.Drawing.Image)(resources.GetObject("button2.Image")));
            this.button2.Location = new System.Drawing.Point(598, 48);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(24, 21);
            this.button2.TabIndex = 7;
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // TxFicheroSalida
            // 
            this.TxFicheroSalida.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.TxFicheroSalida.BackColor = System.Drawing.Color.LemonChiffon;
            this.TxFicheroSalida.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TxFicheroSalida.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxFicheroSalida.Location = new System.Drawing.Point(142, 48);
            this.TxFicheroSalida.Name = "TxFicheroSalida";
            this.TxFicheroSalida.ScrollBars = System.Windows.Forms.ScrollBars.Horizontal;
            this.TxFicheroSalida.Size = new System.Drawing.Size(455, 21);
            this.TxFicheroSalida.TabIndex = 6;
            this.TxFicheroSalida.Text = "(falta selección)";
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.Bisque;
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(16, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(120, 21);
            this.label2.TabIndex = 8;
            this.label2.Text = "Fichero de salida";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblNewBase141
            // 
            this.lblNewBase141.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase141.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase141.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase141.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase141.Location = new System.Drawing.Point(4, 225);
            this.lblNewBase141.Name = "lblNewBase141";
            this.lblNewBase141.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase141.TabIndex = 201;
            this.lblNewBase141.Tag = "1";
            this.lblNewBase141.Text = "1";
            this.lblNewBase141.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase141.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase141.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase141.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase131
            // 
            this.lblNewBase131.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase131.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase131.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase131.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase131.Location = new System.Drawing.Point(4, 208);
            this.lblNewBase131.Name = "lblNewBase131";
            this.lblNewBase131.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase131.TabIndex = 200;
            this.lblNewBase131.Tag = "1";
            this.lblNewBase131.Text = "1";
            this.lblNewBase131.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase131.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase131.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase131.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase121
            // 
            this.lblNewBase121.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase121.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase121.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase121.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase121.Location = new System.Drawing.Point(4, 191);
            this.lblNewBase121.Name = "lblNewBase121";
            this.lblNewBase121.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase121.TabIndex = 199;
            this.lblNewBase121.Tag = "1";
            this.lblNewBase121.Text = "1";
            this.lblNewBase121.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase121.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase121.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase121.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase111
            // 
            this.lblNewBase111.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase111.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase111.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase111.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase111.Location = new System.Drawing.Point(4, 174);
            this.lblNewBase111.Name = "lblNewBase111";
            this.lblNewBase111.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase111.TabIndex = 198;
            this.lblNewBase111.Tag = "1";
            this.lblNewBase111.Text = "1";
            this.lblNewBase111.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase111.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase111.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase111.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase101
            // 
            this.lblNewBase101.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase101.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase101.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase101.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase101.Location = new System.Drawing.Point(4, 157);
            this.lblNewBase101.Name = "lblNewBase101";
            this.lblNewBase101.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase101.TabIndex = 197;
            this.lblNewBase101.Tag = "1";
            this.lblNewBase101.Text = "1";
            this.lblNewBase101.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase101.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase101.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase101.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase91
            // 
            this.lblNewBase91.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase91.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase91.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase91.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase91.Location = new System.Drawing.Point(4, 140);
            this.lblNewBase91.Name = "lblNewBase91";
            this.lblNewBase91.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase91.TabIndex = 196;
            this.lblNewBase91.Tag = "1";
            this.lblNewBase91.Text = "1";
            this.lblNewBase91.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase91.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase91.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase91.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase81
            // 
            this.lblNewBase81.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase81.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase81.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase81.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase81.Location = new System.Drawing.Point(4, 123);
            this.lblNewBase81.Name = "lblNewBase81";
            this.lblNewBase81.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase81.TabIndex = 195;
            this.lblNewBase81.Tag = "1";
            this.lblNewBase81.Text = "1";
            this.lblNewBase81.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase81.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase81.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase81.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase71
            // 
            this.lblNewBase71.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase71.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase71.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase71.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase71.Location = new System.Drawing.Point(4, 106);
            this.lblNewBase71.Name = "lblNewBase71";
            this.lblNewBase71.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase71.TabIndex = 194;
            this.lblNewBase71.Tag = "1";
            this.lblNewBase71.Text = "1";
            this.lblNewBase71.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase71.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase71.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase71.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase61
            // 
            this.lblNewBase61.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase61.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase61.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase61.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase61.Location = new System.Drawing.Point(4, 89);
            this.lblNewBase61.Name = "lblNewBase61";
            this.lblNewBase61.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase61.TabIndex = 193;
            this.lblNewBase61.Tag = "1";
            this.lblNewBase61.Text = "1";
            this.lblNewBase61.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase61.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase61.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase61.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase51
            // 
            this.lblNewBase51.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase51.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase51.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase51.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase51.Location = new System.Drawing.Point(4, 72);
            this.lblNewBase51.Name = "lblNewBase51";
            this.lblNewBase51.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase51.TabIndex = 192;
            this.lblNewBase51.Tag = "1";
            this.lblNewBase51.Text = "1";
            this.lblNewBase51.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase51.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase51.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase51.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase41
            // 
            this.lblNewBase41.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase41.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase41.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase41.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase41.Location = new System.Drawing.Point(4, 55);
            this.lblNewBase41.Name = "lblNewBase41";
            this.lblNewBase41.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase41.TabIndex = 191;
            this.lblNewBase41.Tag = "1";
            this.lblNewBase41.Text = "1";
            this.lblNewBase41.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase41.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase41.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase41.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase31
            // 
            this.lblNewBase31.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase31.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase31.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase31.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase31.Location = new System.Drawing.Point(4, 38);
            this.lblNewBase31.Name = "lblNewBase31";
            this.lblNewBase31.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase31.TabIndex = 190;
            this.lblNewBase31.Tag = "1";
            this.lblNewBase31.Text = "1";
            this.lblNewBase31.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase31.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase31.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase31.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase21
            // 
            this.lblNewBase21.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase21.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase21.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase21.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase21.Location = new System.Drawing.Point(4, 21);
            this.lblNewBase21.Name = "lblNewBase21";
            this.lblNewBase21.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase21.TabIndex = 189;
            this.lblNewBase21.Tag = "1";
            this.lblNewBase21.Text = "1";
            this.lblNewBase21.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase21.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase21.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase21.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase11
            // 
            this.lblNewBase11.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase11.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase11.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase11.Location = new System.Drawing.Point(4, 4);
            this.lblNewBase11.Name = "lblNewBase11";
            this.lblNewBase11.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase11.TabIndex = 188;
            this.lblNewBase11.Tag = "1";
            this.lblNewBase11.Text = "1";
            this.lblNewBase11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase11.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase11.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase11.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase14X
            // 
            this.lblNewBase14X.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase14X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase14X.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase14X.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase14X.Location = new System.Drawing.Point(23, 225);
            this.lblNewBase14X.Name = "lblNewBase14X";
            this.lblNewBase14X.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase14X.TabIndex = 215;
            this.lblNewBase14X.Tag = "X";
            this.lblNewBase14X.Text = "X";
            this.lblNewBase14X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase14X.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase14X.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase14X.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase13X
            // 
            this.lblNewBase13X.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase13X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase13X.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase13X.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase13X.Location = new System.Drawing.Point(23, 208);
            this.lblNewBase13X.Name = "lblNewBase13X";
            this.lblNewBase13X.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase13X.TabIndex = 214;
            this.lblNewBase13X.Tag = "X";
            this.lblNewBase13X.Text = "X";
            this.lblNewBase13X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase13X.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase13X.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase13X.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase12X
            // 
            this.lblNewBase12X.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase12X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase12X.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase12X.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase12X.Location = new System.Drawing.Point(23, 191);
            this.lblNewBase12X.Name = "lblNewBase12X";
            this.lblNewBase12X.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase12X.TabIndex = 213;
            this.lblNewBase12X.Tag = "X";
            this.lblNewBase12X.Text = "X";
            this.lblNewBase12X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase12X.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase12X.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase12X.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase11X
            // 
            this.lblNewBase11X.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase11X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase11X.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase11X.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase11X.Location = new System.Drawing.Point(23, 174);
            this.lblNewBase11X.Name = "lblNewBase11X";
            this.lblNewBase11X.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase11X.TabIndex = 212;
            this.lblNewBase11X.Tag = "X";
            this.lblNewBase11X.Text = "X";
            this.lblNewBase11X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase11X.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase11X.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase11X.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase10X
            // 
            this.lblNewBase10X.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase10X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase10X.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase10X.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase10X.Location = new System.Drawing.Point(23, 157);
            this.lblNewBase10X.Name = "lblNewBase10X";
            this.lblNewBase10X.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase10X.TabIndex = 211;
            this.lblNewBase10X.Tag = "X";
            this.lblNewBase10X.Text = "X";
            this.lblNewBase10X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase10X.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase10X.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase10X.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase8X
            // 
            this.lblNewBase8X.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase8X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase8X.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase8X.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase8X.Location = new System.Drawing.Point(23, 123);
            this.lblNewBase8X.Name = "lblNewBase8X";
            this.lblNewBase8X.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase8X.TabIndex = 209;
            this.lblNewBase8X.Tag = "X";
            this.lblNewBase8X.Text = "X";
            this.lblNewBase8X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase8X.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase8X.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase8X.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase7X
            // 
            this.lblNewBase7X.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase7X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase7X.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase7X.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase7X.Location = new System.Drawing.Point(23, 106);
            this.lblNewBase7X.Name = "lblNewBase7X";
            this.lblNewBase7X.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase7X.TabIndex = 208;
            this.lblNewBase7X.Tag = "X";
            this.lblNewBase7X.Text = "X";
            this.lblNewBase7X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase7X.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase7X.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase7X.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase6X
            // 
            this.lblNewBase6X.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase6X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase6X.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase6X.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase6X.Location = new System.Drawing.Point(23, 89);
            this.lblNewBase6X.Name = "lblNewBase6X";
            this.lblNewBase6X.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase6X.TabIndex = 207;
            this.lblNewBase6X.Tag = "X";
            this.lblNewBase6X.Text = "X";
            this.lblNewBase6X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase6X.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase6X.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase6X.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase5X
            // 
            this.lblNewBase5X.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase5X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase5X.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase5X.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase5X.Location = new System.Drawing.Point(23, 72);
            this.lblNewBase5X.Name = "lblNewBase5X";
            this.lblNewBase5X.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase5X.TabIndex = 206;
            this.lblNewBase5X.Tag = "X";
            this.lblNewBase5X.Text = "X";
            this.lblNewBase5X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase5X.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase5X.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase5X.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase4X
            // 
            this.lblNewBase4X.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase4X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase4X.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase4X.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase4X.Location = new System.Drawing.Point(23, 55);
            this.lblNewBase4X.Name = "lblNewBase4X";
            this.lblNewBase4X.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase4X.TabIndex = 205;
            this.lblNewBase4X.Tag = "X";
            this.lblNewBase4X.Text = "X";
            this.lblNewBase4X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase4X.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase4X.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase4X.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase3X
            // 
            this.lblNewBase3X.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase3X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase3X.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase3X.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase3X.Location = new System.Drawing.Point(23, 38);
            this.lblNewBase3X.Name = "lblNewBase3X";
            this.lblNewBase3X.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase3X.TabIndex = 204;
            this.lblNewBase3X.Tag = "X";
            this.lblNewBase3X.Text = "X";
            this.lblNewBase3X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase3X.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase3X.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase3X.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase2X
            // 
            this.lblNewBase2X.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase2X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase2X.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase2X.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase2X.Location = new System.Drawing.Point(23, 21);
            this.lblNewBase2X.Name = "lblNewBase2X";
            this.lblNewBase2X.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase2X.TabIndex = 203;
            this.lblNewBase2X.Tag = "X";
            this.lblNewBase2X.Text = "X";
            this.lblNewBase2X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase2X.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase2X.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase2X.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase1X
            // 
            this.lblNewBase1X.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase1X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase1X.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase1X.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase1X.Location = new System.Drawing.Point(23, 4);
            this.lblNewBase1X.Name = "lblNewBase1X";
            this.lblNewBase1X.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase1X.TabIndex = 202;
            this.lblNewBase1X.Tag = "X";
            this.lblNewBase1X.Text = "X";
            this.lblNewBase1X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase1X.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase1X.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase1X.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase142
            // 
            this.lblNewBase142.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase142.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase142.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase142.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase142.Location = new System.Drawing.Point(42, 225);
            this.lblNewBase142.Name = "lblNewBase142";
            this.lblNewBase142.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase142.TabIndex = 229;
            this.lblNewBase142.Tag = "2";
            this.lblNewBase142.Text = "2";
            this.lblNewBase142.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase142.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase142.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase142.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase132
            // 
            this.lblNewBase132.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase132.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase132.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase132.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase132.Location = new System.Drawing.Point(42, 208);
            this.lblNewBase132.Name = "lblNewBase132";
            this.lblNewBase132.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase132.TabIndex = 228;
            this.lblNewBase132.Tag = "2";
            this.lblNewBase132.Text = "2";
            this.lblNewBase132.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase132.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase132.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase132.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase122
            // 
            this.lblNewBase122.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase122.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase122.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase122.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase122.Location = new System.Drawing.Point(42, 191);
            this.lblNewBase122.Name = "lblNewBase122";
            this.lblNewBase122.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase122.TabIndex = 227;
            this.lblNewBase122.Tag = "2";
            this.lblNewBase122.Text = "2";
            this.lblNewBase122.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase122.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase122.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase122.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase112
            // 
            this.lblNewBase112.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase112.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase112.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase112.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase112.Location = new System.Drawing.Point(42, 174);
            this.lblNewBase112.Name = "lblNewBase112";
            this.lblNewBase112.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase112.TabIndex = 226;
            this.lblNewBase112.Tag = "2";
            this.lblNewBase112.Text = "2";
            this.lblNewBase112.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase112.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase112.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase112.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase102
            // 
            this.lblNewBase102.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase102.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase102.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase102.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase102.Location = new System.Drawing.Point(42, 157);
            this.lblNewBase102.Name = "lblNewBase102";
            this.lblNewBase102.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase102.TabIndex = 225;
            this.lblNewBase102.Tag = "2";
            this.lblNewBase102.Text = "2";
            this.lblNewBase102.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase102.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase102.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase102.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase92
            // 
            this.lblNewBase92.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase92.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase92.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase92.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase92.Location = new System.Drawing.Point(42, 140);
            this.lblNewBase92.Name = "lblNewBase92";
            this.lblNewBase92.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase92.TabIndex = 224;
            this.lblNewBase92.Tag = "2";
            this.lblNewBase92.Text = "2";
            this.lblNewBase92.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase92.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase92.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase92.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase82
            // 
            this.lblNewBase82.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase82.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase82.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase82.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase82.Location = new System.Drawing.Point(42, 123);
            this.lblNewBase82.Name = "lblNewBase82";
            this.lblNewBase82.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase82.TabIndex = 223;
            this.lblNewBase82.Tag = "2";
            this.lblNewBase82.Text = "2";
            this.lblNewBase82.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase82.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase82.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase82.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase72
            // 
            this.lblNewBase72.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase72.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase72.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase72.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase72.Location = new System.Drawing.Point(42, 106);
            this.lblNewBase72.Name = "lblNewBase72";
            this.lblNewBase72.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase72.TabIndex = 222;
            this.lblNewBase72.Tag = "2";
            this.lblNewBase72.Text = "2";
            this.lblNewBase72.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase72.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase72.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase72.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase62
            // 
            this.lblNewBase62.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase62.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase62.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase62.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase62.Location = new System.Drawing.Point(42, 89);
            this.lblNewBase62.Name = "lblNewBase62";
            this.lblNewBase62.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase62.TabIndex = 221;
            this.lblNewBase62.Tag = "2";
            this.lblNewBase62.Text = "2";
            this.lblNewBase62.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase62.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase62.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase62.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase52
            // 
            this.lblNewBase52.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase52.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase52.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase52.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase52.Location = new System.Drawing.Point(42, 72);
            this.lblNewBase52.Name = "lblNewBase52";
            this.lblNewBase52.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase52.TabIndex = 220;
            this.lblNewBase52.Tag = "2";
            this.lblNewBase52.Text = "2";
            this.lblNewBase52.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase52.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase52.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase52.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase42
            // 
            this.lblNewBase42.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase42.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase42.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase42.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase42.Location = new System.Drawing.Point(42, 55);
            this.lblNewBase42.Name = "lblNewBase42";
            this.lblNewBase42.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase42.TabIndex = 219;
            this.lblNewBase42.Tag = "2";
            this.lblNewBase42.Text = "2";
            this.lblNewBase42.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase42.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase42.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase42.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase32
            // 
            this.lblNewBase32.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase32.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase32.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase32.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase32.Location = new System.Drawing.Point(42, 38);
            this.lblNewBase32.Name = "lblNewBase32";
            this.lblNewBase32.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase32.TabIndex = 218;
            this.lblNewBase32.Tag = "2";
            this.lblNewBase32.Text = "2";
            this.lblNewBase32.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase32.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase32.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase32.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase22
            // 
            this.lblNewBase22.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase22.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase22.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase22.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase22.Location = new System.Drawing.Point(42, 21);
            this.lblNewBase22.Name = "lblNewBase22";
            this.lblNewBase22.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase22.TabIndex = 217;
            this.lblNewBase22.Tag = "2";
            this.lblNewBase22.Text = "2";
            this.lblNewBase22.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase22.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase22.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase22.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase12
            // 
            this.lblNewBase12.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase12.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase12.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase12.Location = new System.Drawing.Point(42, 4);
            this.lblNewBase12.Name = "lblNewBase12";
            this.lblNewBase12.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase12.TabIndex = 216;
            this.lblNewBase12.Tag = "2";
            this.lblNewBase12.Text = "2";
            this.lblNewBase12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase12.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase12.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase12.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // btCancelar
            // 
            this.btCancelar.BackColor = System.Drawing.Color.LightSalmon;
            this.btCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btCancelar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btCancelar.Image = ((System.Drawing.Image)(resources.GetObject("btCancelar.Image")));
            this.btCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btCancelar.Location = new System.Drawing.Point(508, 352);
            this.btCancelar.Name = "btCancelar";
            this.btCancelar.Size = new System.Drawing.Size(100, 24);
            this.btCancelar.TabIndex = 286;
            this.btCancelar.Text = "Cancelar";
            this.btCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btCancelar.UseVisualStyleBackColor = false;
            this.btCancelar.Click += new System.EventHandler(this.btCancelar_Click);
            // 
            // btAceptar
            // 
            this.btAceptar.BackColor = System.Drawing.Color.LightSalmon;
            this.btAceptar.Enabled = false;
            this.btAceptar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btAceptar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btAceptar.Image = ((System.Drawing.Image)(resources.GetObject("btAceptar.Image")));
            this.btAceptar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btAceptar.Location = new System.Drawing.Point(508, 320);
            this.btAceptar.Name = "btAceptar";
            this.btAceptar.Size = new System.Drawing.Size(100, 24);
            this.btAceptar.TabIndex = 287;
            this.btAceptar.Text = "Aceptar";
            this.btAceptar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btAceptar.UseVisualStyleBackColor = false;
            this.btAceptar.Click += new System.EventHandler(this.btAceptar_Click);
            // 
            // lblBase14
            // 
            this.lblBase14.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblBase14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBase14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBase14.Location = new System.Drawing.Point(18, 390);
            this.lblBase14.Name = "lblBase14";
            this.lblBase14.Size = new System.Drawing.Size(18, 16);
            this.lblBase14.TabIndex = 303;
            this.lblBase14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBase13
            // 
            this.lblBase13.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblBase13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBase13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBase13.Location = new System.Drawing.Point(18, 373);
            this.lblBase13.Name = "lblBase13";
            this.lblBase13.Size = new System.Drawing.Size(18, 16);
            this.lblBase13.TabIndex = 302;
            this.lblBase13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBase12
            // 
            this.lblBase12.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblBase12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBase12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBase12.Location = new System.Drawing.Point(18, 356);
            this.lblBase12.Name = "lblBase12";
            this.lblBase12.Size = new System.Drawing.Size(18, 16);
            this.lblBase12.TabIndex = 301;
            this.lblBase12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBase11
            // 
            this.lblBase11.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblBase11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBase11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBase11.Location = new System.Drawing.Point(18, 339);
            this.lblBase11.Name = "lblBase11";
            this.lblBase11.Size = new System.Drawing.Size(18, 16);
            this.lblBase11.TabIndex = 300;
            this.lblBase11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBase10
            // 
            this.lblBase10.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblBase10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBase10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBase10.Location = new System.Drawing.Point(18, 322);
            this.lblBase10.Name = "lblBase10";
            this.lblBase10.Size = new System.Drawing.Size(18, 16);
            this.lblBase10.TabIndex = 299;
            this.lblBase10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBase9
            // 
            this.lblBase9.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblBase9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBase9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBase9.Location = new System.Drawing.Point(18, 305);
            this.lblBase9.Name = "lblBase9";
            this.lblBase9.Size = new System.Drawing.Size(18, 16);
            this.lblBase9.TabIndex = 298;
            this.lblBase9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBase8
            // 
            this.lblBase8.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblBase8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBase8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBase8.Location = new System.Drawing.Point(18, 288);
            this.lblBase8.Name = "lblBase8";
            this.lblBase8.Size = new System.Drawing.Size(18, 16);
            this.lblBase8.TabIndex = 297;
            this.lblBase8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBase7
            // 
            this.lblBase7.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblBase7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBase7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBase7.Location = new System.Drawing.Point(18, 271);
            this.lblBase7.Name = "lblBase7";
            this.lblBase7.Size = new System.Drawing.Size(18, 16);
            this.lblBase7.TabIndex = 296;
            this.lblBase7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBase6
            // 
            this.lblBase6.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblBase6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBase6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBase6.Location = new System.Drawing.Point(18, 254);
            this.lblBase6.Name = "lblBase6";
            this.lblBase6.Size = new System.Drawing.Size(18, 16);
            this.lblBase6.TabIndex = 295;
            this.lblBase6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBase5
            // 
            this.lblBase5.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblBase5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBase5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBase5.Location = new System.Drawing.Point(18, 237);
            this.lblBase5.Name = "lblBase5";
            this.lblBase5.Size = new System.Drawing.Size(18, 16);
            this.lblBase5.TabIndex = 294;
            this.lblBase5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBase4
            // 
            this.lblBase4.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblBase4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBase4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBase4.Location = new System.Drawing.Point(18, 220);
            this.lblBase4.Name = "lblBase4";
            this.lblBase4.Size = new System.Drawing.Size(18, 16);
            this.lblBase4.TabIndex = 293;
            this.lblBase4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBase3
            // 
            this.lblBase3.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblBase3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBase3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBase3.Location = new System.Drawing.Point(18, 203);
            this.lblBase3.Name = "lblBase3";
            this.lblBase3.Size = new System.Drawing.Size(18, 16);
            this.lblBase3.TabIndex = 292;
            this.lblBase3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBase2
            // 
            this.lblBase2.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblBase2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBase2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBase2.Location = new System.Drawing.Point(18, 186);
            this.lblBase2.Name = "lblBase2";
            this.lblBase2.Size = new System.Drawing.Size(18, 16);
            this.lblBase2.TabIndex = 291;
            this.lblBase2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBase1
            // 
            this.lblBase1.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblBase1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBase1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBase1.Location = new System.Drawing.Point(18, 169);
            this.lblBase1.Name = "lblBase1";
            this.lblBase1.Size = new System.Drawing.Size(18, 16);
            this.lblBase1.TabIndex = 290;
            this.lblBase1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // statusBar1
            // 
            this.statusBar1.Location = new System.Drawing.Point(0, 459);
            this.statusBar1.Name = "statusBar1";
            this.statusBar1.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.statusBarPanel1,
            this.statusBarPanel2,
            this.statusBarPanel3});
            this.statusBar1.Size = new System.Drawing.Size(641, 22);
            this.statusBar1.TabIndex = 311;
            this.statusBar1.Text = "statusBar1";
            // 
            // statusBarPanel1
            // 
            this.statusBarPanel1.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.statusBarPanel1.Name = "statusBarPanel1";
            this.statusBarPanel1.Text = "Estado";
            this.statusBarPanel1.Width = 49;
            // 
            // statusBarPanel2
            // 
            this.statusBarPanel2.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.statusBarPanel2.Name = "statusBarPanel2";
            this.statusBarPanel2.Text = "Faltan datos";
            this.statusBarPanel2.Width = 76;
            // 
            // statusBarPanel3
            // 
            this.statusBarPanel3.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.statusBarPanel3.Name = "statusBarPanel3";
            this.statusBarPanel3.Width = 10;
            // 
            // label17
            // 
            this.label17.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label17.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.Location = new System.Drawing.Point(80, 141);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(18, 20);
            this.label17.TabIndex = 312;
            this.label17.Text = "1";
            this.label17.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label18
            // 
            this.label18.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label18.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.Location = new System.Drawing.Point(99, 141);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(18, 20);
            this.label18.TabIndex = 313;
            this.label18.Text = "X";
            this.label18.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label19
            // 
            this.label19.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label19.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.Location = new System.Drawing.Point(118, 141);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(18, 20);
            this.label19.TabIndex = 314;
            this.label19.Text = "2";
            this.label19.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label23
            // 
            this.label23.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label23.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.Location = new System.Drawing.Point(79, 113);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(57, 28);
            this.label23.TabIndex = 384;
            this.label23.Text = "Cambios de signo";
            this.label23.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkGiros
            // 
            this.chkGiros.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkGiros.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkGiros.Location = new System.Drawing.Point(502, 196);
            this.chkGiros.Name = "chkGiros";
            this.chkGiros.Size = new System.Drawing.Size(120, 48);
            this.chkGiros.TabIndex = 385;
            this.chkGiros.Text = "Corrección de fallos";
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.LightSalmon;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Image = ((System.Drawing.Image)(resources.GetObject("button3.Image")));
            this.button3.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button3.Location = new System.Drawing.Point(508, 288);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(100, 24);
            this.button3.TabIndex = 387;
            this.button3.Text = "Transponer";
            this.button3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.DarkSalmon;
            this.panel1.Controls.Add(this.lblNewBase162);
            this.panel1.Controls.Add(this.lblNewBase151);
            this.panel1.Controls.Add(this.lblNewBase152);
            this.panel1.Controls.Add(this.lblNewBase161);
            this.panel1.Controls.Add(this.lblNewBase16X);
            this.panel1.Controls.Add(this.lblNewBase15X);
            this.panel1.Controls.Add(this.lblNewBase11);
            this.panel1.Controls.Add(this.lblNewBase1X);
            this.panel1.Controls.Add(this.lblNewBase21);
            this.panel1.Controls.Add(this.lblNewBase31);
            this.panel1.Controls.Add(this.lblNewBase41);
            this.panel1.Controls.Add(this.lblNewBase51);
            this.panel1.Controls.Add(this.lblNewBase61);
            this.panel1.Controls.Add(this.lblNewBase71);
            this.panel1.Controls.Add(this.lblNewBase81);
            this.panel1.Controls.Add(this.lblNewBase91);
            this.panel1.Controls.Add(this.lblNewBase101);
            this.panel1.Controls.Add(this.lblNewBase111);
            this.panel1.Controls.Add(this.lblNewBase121);
            this.panel1.Controls.Add(this.lblNewBase131);
            this.panel1.Controls.Add(this.lblNewBase141);
            this.panel1.Controls.Add(this.lblNewBase2X);
            this.panel1.Controls.Add(this.lblNewBase3X);
            this.panel1.Controls.Add(this.lblNewBase4X);
            this.panel1.Controls.Add(this.lblNewBase5X);
            this.panel1.Controls.Add(this.lblNewBase6X);
            this.panel1.Controls.Add(this.lblNewBase7X);
            this.panel1.Controls.Add(this.lblNewBase8X);
            this.panel1.Controls.Add(this.lblNewBase142);
            this.panel1.Controls.Add(this.lblNewBase9X);
            this.panel1.Controls.Add(this.lblNewBase132);
            this.panel1.Controls.Add(this.lblNewBase10X);
            this.panel1.Controls.Add(this.lblNewBase122);
            this.panel1.Controls.Add(this.lblNewBase11X);
            this.panel1.Controls.Add(this.lblNewBase112);
            this.panel1.Controls.Add(this.lblNewBase12X);
            this.panel1.Controls.Add(this.lblNewBase102);
            this.panel1.Controls.Add(this.lblNewBase13X);
            this.panel1.Controls.Add(this.lblNewBase92);
            this.panel1.Controls.Add(this.lblNewBase14X);
            this.panel1.Controls.Add(this.lblNewBase82);
            this.panel1.Controls.Add(this.lblNewBase12);
            this.panel1.Controls.Add(this.lblNewBase72);
            this.panel1.Controls.Add(this.lblNewBase22);
            this.panel1.Controls.Add(this.lblNewBase62);
            this.panel1.Controls.Add(this.lblNewBase32);
            this.panel1.Controls.Add(this.lblNewBase52);
            this.panel1.Controls.Add(this.lblNewBase42);
            this.panel1.Location = new System.Drawing.Point(76, 165);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(64, 278);
            this.panel1.TabIndex = 389;
            // 
            // lblNewBase9X
            // 
            this.lblNewBase9X.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase9X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase9X.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase9X.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase9X.Location = new System.Drawing.Point(23, 140);
            this.lblNewBase9X.Name = "lblNewBase9X";
            this.lblNewBase9X.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase9X.TabIndex = 210;
            this.lblNewBase9X.Tag = "X";
            this.lblNewBase9X.Text = "X";
            this.lblNewBase9X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase9X.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase9X.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase9X.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase162
            // 
            this.lblNewBase162.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase162.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase162.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase162.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase162.Location = new System.Drawing.Point(42, 259);
            this.lblNewBase162.Name = "lblNewBase162";
            this.lblNewBase162.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase162.TabIndex = 573;
            this.lblNewBase162.Tag = "2";
            this.lblNewBase162.Text = "2";
            this.lblNewBase162.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase162.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase162.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase162.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase152
            // 
            this.lblNewBase152.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase152.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase152.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase152.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase152.Location = new System.Drawing.Point(42, 242);
            this.lblNewBase152.Name = "lblNewBase152";
            this.lblNewBase152.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase152.TabIndex = 572;
            this.lblNewBase152.Tag = "2";
            this.lblNewBase152.Text = "2";
            this.lblNewBase152.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase152.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase152.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase152.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase16X
            // 
            this.lblNewBase16X.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase16X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase16X.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase16X.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase16X.Location = new System.Drawing.Point(23, 259);
            this.lblNewBase16X.Name = "lblNewBase16X";
            this.lblNewBase16X.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase16X.TabIndex = 571;
            this.lblNewBase16X.Tag = "X";
            this.lblNewBase16X.Text = "X";
            this.lblNewBase16X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase16X.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase16X.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase16X.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase15X
            // 
            this.lblNewBase15X.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase15X.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase15X.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase15X.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase15X.Location = new System.Drawing.Point(23, 242);
            this.lblNewBase15X.Name = "lblNewBase15X";
            this.lblNewBase15X.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase15X.TabIndex = 570;
            this.lblNewBase15X.Tag = "X";
            this.lblNewBase15X.Text = "X";
            this.lblNewBase15X.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase15X.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase15X.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase15X.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase161
            // 
            this.lblNewBase161.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase161.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase161.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase161.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase161.Location = new System.Drawing.Point(4, 259);
            this.lblNewBase161.Name = "lblNewBase161";
            this.lblNewBase161.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase161.TabIndex = 569;
            this.lblNewBase161.Tag = "1";
            this.lblNewBase161.Text = "1";
            this.lblNewBase161.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase161.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase161.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase161.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblNewBase151
            // 
            this.lblNewBase151.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase151.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase151.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase151.ForeColor = System.Drawing.Color.Silver;
            this.lblNewBase151.Location = new System.Drawing.Point(4, 242);
            this.lblNewBase151.Name = "lblNewBase151";
            this.lblNewBase151.Size = new System.Drawing.Size(18, 16);
            this.lblNewBase151.TabIndex = 568;
            this.lblNewBase151.Tag = "1";
            this.lblNewBase151.Text = "1";
            this.lblNewBase151.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase151.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase151.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            this.lblNewBase151.Click += new System.EventHandler(this.GenericLabel_Click);
            // 
            // lblBase15
            // 
            this.lblBase15.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblBase15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBase15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBase15.Location = new System.Drawing.Point(18, 407);
            this.lblBase15.Name = "lblBase15";
            this.lblBase15.Size = new System.Drawing.Size(18, 16);
            this.lblBase15.TabIndex = 568;
            this.lblBase15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblBase16
            // 
            this.lblBase16.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblBase16.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblBase16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBase16.Location = new System.Drawing.Point(18, 424);
            this.lblBase16.Name = "lblBase16";
            this.lblBase16.Size = new System.Drawing.Size(18, 16);
            this.lblBase16.TabIndex = 569;
            this.lblBase16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            this.label10.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(18, 457);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(18, 18);
            this.label10.TabIndex = 569;
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.Silver;
            this.label7.Location = new System.Drawing.Point(4, 289);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(18, 18);
            this.label7.TabIndex = 569;
            this.label7.Tag = "1";
            this.label7.Text = "1";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.Silver;
            this.label5.Location = new System.Drawing.Point(23, 289);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(18, 18);
            this.label5.TabIndex = 571;
            this.label5.Tag = "X";
            this.label5.Text = "X";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.Silver;
            this.label3.Location = new System.Drawing.Point(42, 289);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(18, 18);
            this.label3.TabIndex = 573;
            this.label3.Tag = "2";
            this.label3.Text = "2";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // controlPorcentajesJornada
            // 
            this.controlPorcentajesJornada.archivoPorcentajes = null;
            this.controlPorcentajesJornada.BackColor = System.Drawing.Color.Bisque;
            this.controlPorcentajesJornada.CaptionText = "Valoraciones   JORNADA";
            this.controlPorcentajesJornada.FormatoFicheroValoraciones = ((short)(0));
            this.controlPorcentajesJornada.Jornada = "01";
            this.controlPorcentajesJornada.Location = new System.Drawing.Point(336, 80);
            this.controlPorcentajesJornada.Name = "controlPorcentajesJornada";
            this.controlPorcentajesJornada.ReadOnly = false;
            this.controlPorcentajesJornada.Size = new System.Drawing.Size(160, 372);
            this.controlPorcentajesJornada.TabIndex = 567;
            this.controlPorcentajesJornada.Temporada = "2004/2005";
            this.controlPorcentajesJornada.Modificado += new System.EventHandler(this.controlPorcentajesJornada_Modificado);
            // 
            // controlPorcentajesCombinacion
            // 
            this.controlPorcentajesCombinacion.archivoPorcentajes = null;
            this.controlPorcentajesCombinacion.BackColor = System.Drawing.Color.Bisque;
            this.controlPorcentajesCombinacion.CaptionText = "         %  COMBINACIÓN";
            this.controlPorcentajesCombinacion.FormatoFicheroValoraciones = ((short)(0));
            this.controlPorcentajesCombinacion.Jornada = "01";
            this.controlPorcentajesCombinacion.Location = new System.Drawing.Point(164, 80);
            this.controlPorcentajesCombinacion.Name = "controlPorcentajesCombinacion";
            this.controlPorcentajesCombinacion.ReadOnly = false;
            this.controlPorcentajesCombinacion.Size = new System.Drawing.Size(160, 372);
            this.controlPorcentajesCombinacion.TabIndex = 566;
            this.controlPorcentajesCombinacion.Temporada = "2004/2005";
            // 
            // label24
            // 
            this.label24.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label24.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.Location = new System.Drawing.Point(19, 113);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(17, 48);
            this.label24.TabIndex = 386;
            this.label24.Text = "Base";
            this.label24.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // RotacionDeSignosFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(641, 481);
            this.ControlBox = false;
            this.Controls.Add(this.lblBase16);
            this.Controls.Add(this.lblBase15);
            this.Controls.Add(this.controlPorcentajesJornada);
            this.Controls.Add(this.controlPorcentajesCombinacion);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.chkGiros);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.statusBar1);
            this.Controls.Add(this.lblBase14);
            this.Controls.Add(this.lblBase13);
            this.Controls.Add(this.lblBase12);
            this.Controls.Add(this.lblBase11);
            this.Controls.Add(this.lblBase10);
            this.Controls.Add(this.lblBase9);
            this.Controls.Add(this.lblBase8);
            this.Controls.Add(this.lblBase7);
            this.Controls.Add(this.lblBase6);
            this.Controls.Add(this.lblBase5);
            this.Controls.Add(this.lblBase4);
            this.Controls.Add(this.lblBase3);
            this.Controls.Add(this.lblBase2);
            this.Controls.Add(this.lblBase1);
            this.Controls.Add(this.btAceptar);
            this.Controls.Add(this.btCancelar);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.TxFicheroSalida);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.TxFicheroEntrada);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel1);
            this.MinimumSize = new System.Drawing.Size(596, 472);
            this.Name = "RotacionDeSignosFrm";
            this.Text = "Rotación de signos";
            this.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            // StatusBarPanel controls don't support ISupportInitialize in .NET 8
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).EndInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel2)).EndInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel3)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        private void InicializarValores()
        {
            controlPorcentajesJornada = new ControlPorcentajes(partidos);
            double[,] temporal = new double[partidos, 3];
            Array.Copy(this.controlPorcentajesJornada.Valores, 0, temporal, 0, partidos * 3);

            controlPorcentajesJornada.Valores = temporal;
            for (int i = 0; i < this.Labels.Length; i++)
            {
                if (i < (partidos * 3))
                {
                    Labels[i].Visible = true;
                }
                else
                {
                    Labels[i].Visible = false;
                }
            }

            for (int i = 0; i < this.LabelsBase.Length; i++)
            {
                if (i < (partidos))
                {
                    LabelsBase[i].Visible = true;
                }
                else
                {
                    LabelsBase[i].Visible = false;
                }
            }
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
				TxFicheroEntrada.Text =Path.GetFileName(archivoEntrada);
                //Ya sabemos el fichero de entrada, podemos especificar los partidos

                IArchivoColumnas aCol = new ArchivoColumnasTexto(abreFiltroDialog.FileName);
                partidos = aCol.ObtenNumSignos();
                aCol.Cerrar();
                InicializarValores();

				LeerColumnas();
				PropuestaAutomatica ();
				HabilitarCalcular ();
			}
		}
		private void HabilitarCalcular()
		{
			btAceptar.Enabled = false;
			if (archivoSalida != "" && archivoEntrada !="") btAceptar.Enabled = true;
			if(btAceptar.Enabled ==true) statusBarPanel2.Text ="Preparado";
			Application.DoEvents();
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
				TxFicheroSalida.Text = Path.GetFileName(archivoSalida);
			}
			HabilitarCalcular ();
		}

		private void btAceptar_Click(object sender, System.EventArgs e)
		{
			int Partido,z;
			int SigIni, Indice;
			BitsCambiados.SetAll (false);

			//los unos se substituyen por NuevosSignos [Partido,0]
			//las equis se substituyen por NuevosSignos [Partido,1]
			//los doses se substituyen por NuevosSignos [Partido,2]
			
			PonerValoresEnVariables();
			PonerValoracionEnVariables ();
			if (this.chkGiros.Checked )
			{
				BitsCambiados.Equals (Bits);
				for (Partido = 0; Partido<partidos; Partido++)
				{
                    for (int i = 0; i < Bits.Count; i++)
                    {
                        if (Bits[i])
                        {
                            SigIni = ((i / pot[Partido]) % 3);
                            z = NuevosSignos[Partido, SigIni];
                            Indice = i + pot[Partido] * (z - SigIni);
                            BitsCambiados[Indice] = true;
                        }
                    }
				}
			}
			else
			{
				if(Test())
				{
					for (int i=0;i<Bits.Count;i++)
					{
						if (Bits[i])
						{
							Indice=i;
							for (Partido = 0; Partido<partidos; Partido++)
							{
								SigIni = ((Indice / pot[Partido]) % 3);
								z=NuevosSignos [Partido,SigIni];
								Indice += pot[Partido] * (z - SigIni);
							}
							BitsCambiados[Indice]=true;
						}
					}
				}
			}
			statusBarPanel2.Text ="Grabando columnas...";
			Application.DoEvents();
			GrabarColumnas();
		}
		private bool Test()
		{
			bool Resultat=true;
			for (int i=0;i<partidos;i++)
			{	
				if(NuevosSignos [i,0]==NuevosSignos [i,1] || NuevosSignos [i,0]==NuevosSignos [i,2] || NuevosSignos [i,1]==NuevosSignos [i,2])
				{
					int Resposta=(int) MessageBox.Show ("El partido nº " + (i+1).ToString () + " tiene asignado mas de una vez el cambio de signo al mismo signo. ¿Desea continuar?","¡¡ ATENCION!!",MessageBoxButtons.YesNo,MessageBoxIcon.Warning);
					if (Resposta ==7)
					{
						Resultat=false;
						break;
					}
				}
			}
			return Resultat;
		}
		private void LeerColumnas() 
		{
			Cursor.Current = Cursors.WaitCursor ;
			statusBarPanel2.Text ="Leyendo columnas...";
			Application.DoEvents();
            IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(archivoEntrada);
			Bits.SetAll (false);
			NumApuestas=0;
			int Num;
			string Columna;
			Array.Clear (PorcentajesSignos,0,PorcentajesSignos.GetLength(0) * 3);
            PorcentajesSignos = new double[partidos, 3];
			ConvertidorDeBases col= new ConvertidorDeBases();

			while( comBaseCols.SiguienteColumna() )
			{
				Columna = comBaseCols.LeeColumnaSinComas();
				Num = col.ConvColumnaANumero  (Columna);
				Bits[Num]=true;
				NumApuestas++;
				if (NumApuestas==1) ColumnaBase=Columna;
				ContarSignos(Num);
			}
			comBaseCols.Cerrar();	
			statusBarPanel1.Text ="Num. apuestas " + NumApuestas.ToString ();
			statusBarPanel2.Text ="Calculando frecuencias de signos...";
			Application.DoEvents();
			ConvertirAPorcentaje();
			MostrarPorcentajesSignos();
			MostrarColumnaBase();
			if (archivoSalida =="")
			{
				statusBarPanel2.Text ="No se ha definido el fichero de salida";
			}
			else
			{
				statusBarPanel2.Text ="Preparado";
			}
			controlPorcentajesJornada.ReadOnly =false;
			Cursor.Current = Cursors.Default;
			Application.DoEvents();
		}

		private void ContarSignos(int Num)
		{
            for (int Partido = 0; Partido < PorcentajesSignos.GetLength(0); Partido++)
            {
                PorcentajesSignos[Partido, ((Num / pot[Partido]) % 3)]++;
            }
		}
		private void MostrarPorcentajesSignos()
		{
			controlPorcentajesCombinacion.Valores = PorcentajesSignos;
            controlPorcentajesCombinacion.Refresh();
            
		}

		private void MostrarColumnaBase()
		{
            for (int i = 0; i < ColumnaBase.Length; i++)
            {
                LabelsBase[i].Text = ColumnaBase[i].ToString();
            }
		}

		private void ConvertirAPorcentaje ()
		{
			double Suma;
            for (int i = 0; i < PorcentajesSignos.GetLength(0); i++)
            {
                Suma = PorcentajesSignos[i, 0] + PorcentajesSignos[i, 1] + PorcentajesSignos[i, 2];
                PorcentajesSignos[i, 2] = Math.Round(PorcentajesSignos[i, 2] * 100 / Suma, 0);
                PorcentajesSignos[i, 1] = Math.Round(PorcentajesSignos[i, 1] * 100 / Suma, 0);
                PorcentajesSignos[i, 0] = 100 - PorcentajesSignos[i, 2] - PorcentajesSignos[i, 1];
            }
		}

		private void GenericLabel_Click(object sender, System.EventArgs e)
		{
			Label MiLabel =(Label)sender;
			switch (MiLabel.Text)
			{
				case "1" :
				{
					MiLabel.Text="X";
					break;
				}
				case "X" :
				{
					MiLabel.Text="2";
					break;
				}
				case "2":
				{
					MiLabel.Text="1";
					break;
				}
			}
			PonerColorEnLabel (MiLabel);

		}
		private void GrabarColumnas()
		{
            ConvertidorDeBases con = new ConvertidorDeBases((byte)partidos);
            IArchivoColumnas Cols = new ArchivoColumnasTexto(archivoSalida, partidos);
			int c=0;

			for (int i=0; i<BitsCambiados.Count; i++) 
			{
				if (BitsCambiados [i])
				{					
					Cols.GuardarCols(con.ConvNumAColumna(i) );
					c++;
				}
			}		
			Cols.Cerrar();	
			statusBarPanel2.Text ="Se han grabado "+ c.ToString () + " columnas";
			Application.DoEvents();
		}
		private void PonerValoresEnVariables()
		{
			int i;
			for (i=0;i<partidos;i++)
			{
				NuevosSignos[i,0]=0;
				NuevosSignos[i,1]=1;
				NuevosSignos[i,2]=2;
			}
			NuevosSignos[0,0]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase11.Text));
			NuevosSignos[0,1]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase1X.Text));
			NuevosSignos[0,2]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase12.Text));

			NuevosSignos[1,0]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase21.Text));
			NuevosSignos[1,1]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase2X.Text));
			NuevosSignos[1,2]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase22.Text));

			NuevosSignos[2,0]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase31.Text));
			NuevosSignos[2,1]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase3X.Text));
			NuevosSignos[2,2]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase32.Text));

			NuevosSignos[3,0]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase41.Text));
			NuevosSignos[3,1]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase4X.Text));
			NuevosSignos[3,2]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase42.Text));

			NuevosSignos[4,0]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase51.Text));
			NuevosSignos[4,1]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase5X.Text));
			NuevosSignos[4,2]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase52.Text));

			NuevosSignos[5,0]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase61.Text));
			NuevosSignos[5,1]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase6X.Text));
			NuevosSignos[5,2]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase62.Text));

			NuevosSignos[6,0]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase71.Text));
			NuevosSignos[6,1]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase7X.Text));
			NuevosSignos[6,2]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase72.Text));

			NuevosSignos[7,0]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase81.Text));
			NuevosSignos[7,1]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase8X.Text));
			NuevosSignos[7,2]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase82.Text));

			NuevosSignos[8,0]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase91.Text));
			NuevosSignos[8,1]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase9X.Text));
			NuevosSignos[8,2]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase92.Text));

			NuevosSignos[9,0]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase101.Text));
			NuevosSignos[9,1]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase10X.Text));
			NuevosSignos[9,2]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase102.Text));

			NuevosSignos[10,0]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase111.Text));
			NuevosSignos[10,1]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase11X.Text));
			NuevosSignos[10,2]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase112.Text));

			NuevosSignos[11,0]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase121.Text));
			NuevosSignos[11,1]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase12X.Text));
			NuevosSignos[11,2]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase122.Text));
			
			NuevosSignos[12,0]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase131.Text));
			NuevosSignos[12,1]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase13X.Text));
			NuevosSignos[12,2]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase132.Text));

			NuevosSignos[13,0]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase141.Text));
			NuevosSignos[13,1]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase14X.Text));
			NuevosSignos[13,2]=Convert.ToInt16 ("1X2".IndexOf (lblNewBase142.Text));

            NuevosSignos[14, 0] = Convert.ToInt16("1X2".IndexOf(lblNewBase151.Text));
            NuevosSignos[14, 1] = Convert.ToInt16("1X2".IndexOf(lblNewBase15X.Text));
            NuevosSignos[14, 2] = Convert.ToInt16("1X2".IndexOf(lblNewBase152.Text));

            NuevosSignos[15, 0] = Convert.ToInt16("1X2".IndexOf(lblNewBase161.Text));
            NuevosSignos[15, 1] = Convert.ToInt16("1X2".IndexOf(lblNewBase16X.Text));
            NuevosSignos[15, 2] = Convert.ToInt16("1X2".IndexOf(lblNewBase162.Text));
		}
		private void PonerVariablesEnLabels()
		{
			string Sig ="1X2";
			lblNewBase11.Text=Sig[NuevosSignos[0,0]].ToString ();
			lblNewBase1X.Text=Sig[NuevosSignos[0,1]].ToString ();
			lblNewBase12.Text=Sig[NuevosSignos[0,2]].ToString ();

			lblNewBase21.Text=Sig[NuevosSignos[1,0]].ToString ();
			lblNewBase2X.Text=Sig[NuevosSignos[1,1]].ToString ();
			lblNewBase22.Text=Sig[NuevosSignos[1,2]].ToString ();

			lblNewBase31.Text=Sig[NuevosSignos[2,0]].ToString ();
			lblNewBase3X.Text=Sig[NuevosSignos[2,1]].ToString ();
			lblNewBase32.Text=Sig[NuevosSignos[2,2]].ToString ();

			lblNewBase41.Text=Sig[NuevosSignos[3,0]].ToString ();
			lblNewBase4X.Text=Sig[NuevosSignos[3,1]].ToString ();
			lblNewBase42.Text=Sig[NuevosSignos[3,2]].ToString ();

			lblNewBase51.Text=Sig[NuevosSignos[4,0]].ToString ();
			lblNewBase5X.Text=Sig[NuevosSignos[4,1]].ToString ();
			lblNewBase52.Text=Sig[NuevosSignos[4,2]].ToString ();

			lblNewBase61.Text=Sig[NuevosSignos[5,0]].ToString ();
			lblNewBase6X.Text=Sig[NuevosSignos[5,1]].ToString ();
			lblNewBase62.Text=Sig[NuevosSignos[5,2]].ToString ();

			lblNewBase71.Text=Sig[NuevosSignos[6,0]].ToString ();
			lblNewBase7X.Text=Sig[NuevosSignos[6,1]].ToString ();
			lblNewBase72.Text=Sig[NuevosSignos[6,2]].ToString ();

			lblNewBase81.Text=Sig[NuevosSignos[7,0]].ToString ();
			lblNewBase8X.Text=Sig[NuevosSignos[7,1]].ToString ();
			lblNewBase82.Text=Sig[NuevosSignos[7,2]].ToString ();

			lblNewBase91.Text=Sig[NuevosSignos[8,0]].ToString ();
			lblNewBase9X.Text=Sig[NuevosSignos[8,1]].ToString ();
			lblNewBase92.Text=Sig[NuevosSignos[8,2]].ToString ();

			lblNewBase101.Text=Sig[NuevosSignos[9,0]].ToString ();
			lblNewBase10X.Text=Sig[NuevosSignos[9,1]].ToString ();
			lblNewBase102.Text=Sig[NuevosSignos[9,2]].ToString ();

			lblNewBase111.Text=Sig[NuevosSignos[10,0]].ToString ();
			lblNewBase11X.Text=Sig[NuevosSignos[10,1]].ToString ();
			lblNewBase112.Text=Sig[NuevosSignos[10,2]].ToString ();

			lblNewBase121.Text=Sig[NuevosSignos[11,0]].ToString ();
			lblNewBase12X.Text=Sig[NuevosSignos[11,1]].ToString ();
			lblNewBase122.Text=Sig[NuevosSignos[11,2]].ToString ();

			lblNewBase131.Text=Sig[NuevosSignos[12,0]].ToString ();
			lblNewBase13X.Text=Sig[NuevosSignos[12,1]].ToString ();
			lblNewBase132.Text=Sig[NuevosSignos[12,2]].ToString ();

			lblNewBase141.Text=Sig[NuevosSignos[13,0]].ToString ();
			lblNewBase14X.Text=Sig[NuevosSignos[13,1]].ToString ();
			lblNewBase142.Text=Sig[NuevosSignos[13,2]].ToString ();

            lblNewBase151.Text = Sig[NuevosSignos[14, 0]].ToString();
            lblNewBase15X.Text = Sig[NuevosSignos[14, 1]].ToString();
            lblNewBase152.Text = Sig[NuevosSignos[14, 2]].ToString();

            lblNewBase161.Text = Sig[NuevosSignos[15, 0]].ToString();
            lblNewBase16X.Text = Sig[NuevosSignos[15, 1]].ToString();
            lblNewBase162.Text = Sig[NuevosSignos[15, 2]].ToString();
			
			PonerColorEnLabel (lblNewBase11);
			PonerColorEnLabel (lblNewBase1X);
			PonerColorEnLabel (lblNewBase12);

			PonerColorEnLabel (lblNewBase21);
			PonerColorEnLabel (lblNewBase2X);
			PonerColorEnLabel (lblNewBase22);

			PonerColorEnLabel (lblNewBase31);
			PonerColorEnLabel (lblNewBase3X);
			PonerColorEnLabel (lblNewBase32);

			PonerColorEnLabel (lblNewBase41);
			PonerColorEnLabel (lblNewBase4X);
			PonerColorEnLabel (lblNewBase42);

			PonerColorEnLabel (lblNewBase51);
			PonerColorEnLabel (lblNewBase5X);
			PonerColorEnLabel (lblNewBase52);

			PonerColorEnLabel (lblNewBase61);
			PonerColorEnLabel (lblNewBase6X);
			PonerColorEnLabel (lblNewBase62);

			PonerColorEnLabel (lblNewBase71);
			PonerColorEnLabel (lblNewBase7X);
			PonerColorEnLabel (lblNewBase72);

			PonerColorEnLabel (lblNewBase81);
			PonerColorEnLabel (lblNewBase8X);
			PonerColorEnLabel (lblNewBase82);

			PonerColorEnLabel (lblNewBase91);
			PonerColorEnLabel (lblNewBase9X);
			PonerColorEnLabel (lblNewBase92);

			PonerColorEnLabel (lblNewBase101);
			PonerColorEnLabel (lblNewBase10X);
			PonerColorEnLabel (lblNewBase102);

			PonerColorEnLabel (lblNewBase111);
			PonerColorEnLabel (lblNewBase11X);
			PonerColorEnLabel (lblNewBase112);

			PonerColorEnLabel (lblNewBase121);
			PonerColorEnLabel (lblNewBase12X);
			PonerColorEnLabel (lblNewBase122);

			PonerColorEnLabel (lblNewBase131);
			PonerColorEnLabel (lblNewBase13X);
			PonerColorEnLabel (lblNewBase132);

			PonerColorEnLabel (lblNewBase141);
			PonerColorEnLabel (lblNewBase14X);
			PonerColorEnLabel (lblNewBase142);

            PonerColorEnLabel(lblNewBase151);
            PonerColorEnLabel(lblNewBase15X);
            PonerColorEnLabel(lblNewBase152);

            PonerColorEnLabel(lblNewBase161);
            PonerColorEnLabel(lblNewBase16X);
            PonerColorEnLabel(lblNewBase162);
		}

		private void btCancelar_Click(object sender, System.EventArgs e)
		{
			this.Close ();
		}

		private void GenericLabel_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			Cursor.Current = Cursors.Hand ;
		}
		private int[] Ordenar3valores(double v1, double vX, double v2)
		{
			double [] x = new double [3] ;
			double [] originalValues = new double [3] ;
			int [] Resultado = new int [3] ;

			if(v1==vX) v1+=0.001;
			if(v2==vX) v2-=0.001;
			if(v1==v2) v1+=0.001;

			x[0]=-v1;
			x[1]=-vX;
			x[2]=-v2;

			originalValues[0]=v1;
			originalValues[1]=vX;
			originalValues[2]=v2;

			Array.Sort (x);

			for (int i=0;i<3;i++)
			{
				if (originalValues[0]==-x[i]) Resultado[i]=0;
				if (originalValues[1]==-x[i]) Resultado[i]=1;
				if (originalValues[2]==-x[i]) Resultado[i]=2;
			}

			return Resultado;
		}
        private void PropuestaAutomatica()
        {
            int[] x = new int[3];
            int[] y = new int[3];
            for (int i = 0; i < PorcentajesSignos.GetLength(0); i++)
            {
                x = Ordenar3valores(PorcentajesSignos[i, 0], PorcentajesSignos[i, 1], PorcentajesSignos[i, 2]);
                y = Ordenar3valores(v[i, 0], v[i, 1], v[i, 2]);
                for (int s = 0; s < 3; s++)
                {
                    NuevosSignos[i, x[s]] = y[s];
                }
            }
            PonerVariablesEnLabels();
        }
        private void PonerValoracionEnVariables()
        {
            v = this.controlPorcentajesCombinacion.Valores;

        }

		private void GenericLabel_DoubleClick(object sender, System.EventArgs e)
		{
			GenericLabel_Click(sender,e);
		}
		private void PonerColorEnLabel (Label L)
		{
			if (L.Text ==L.Tag.ToString () )
			{
				L.ForeColor = Color.Silver  ;
			}
			else
			{
				L.ForeColor = Color.Red  ;
			}
		}

		private void button3_Click(object sender, System.EventArgs e)
		{
			int[] ListaTasposicion = new int [14];
			if(!File.Exists (archivoEntrada)) return;
			if(archivoSalida==archivoEntrada) return;
			if(archivoSalida=="")
			{
				archivoSalida =Path.GetDirectoryName (archivoEntrada) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(archivoEntrada) + "T" +Path.GetExtension(archivoEntrada);
			}
			TransponedorAutomatico T = new TransponedorAutomatico(v,PorcentajesSignos);
			ListaTasposicion = T.Trasposicion ();
			for (int i=0;i<partidos;i++)
			{
				ListaTasposicion[i]++;
			}
			TransposicionFrm Tr = new TransposicionFrm(ListaTasposicion, archivoEntrada, archivoSalida);
			Tr.ShowDialog ();
			if(File.Exists (archivoSalida))
			{
				archivoEntrada=archivoSalida;
				TxFicheroEntrada.Text =	archivoEntrada;
				LeerColumnas();
				PropuestaAutomatica ();
			}
		}
		private void GenericTexBox_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			Valida.Generic_KeyPress (TipoValidacionString.SoloNumerosDecimales, sender, e);
		}


		private void controlPorcentajesJornada_Modificado(object sender, System.EventArgs e)
		{
			v=controlPorcentajesJornada .Valores;
			PropuestaAutomatica ();
			statusBarPanel3.Text =Path.GetFileName(controlPorcentajesJornada.archivoPorcentajes);
			Application.DoEvents();
		}

	}
}

