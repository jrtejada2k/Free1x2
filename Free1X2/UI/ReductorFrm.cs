using System;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using Free1X2.Reduccion;
using Free1X2.EntradaSalida;

namespace Free1X2.UI 
{
	public class ReductorFrm : Form
	{
		private GroupBox groupBox;
		private ComboBox cBoxMetodo;
		private Button btnCancel;
        private Button btnSelFile;
		private Label labMaxPercent;
		private TextBox tBoxMaxPercent;
		private Button btnReducir;
		private Label labMaxCol;
		private TextBox tBoxMaxCols;
		private Label lblSelFile;

		private string archivoEntrada = "";
		private string archivoSalida="";
		private int nivelReduccion;
		private int maxCol;
		private int percent;
		private System.Windows.Forms.Timer myTimer;
		private DateTime hora0, hora9;
		private Label lblFicheroSalida;
		private Button btnFileOutput;
		private CheckBox chkExternas;
		private Label lblColsIni;
		private Label labColsIni;
		private Label lblTempo;
		private Label labTempo;
		private Label lblPercent;
		private Label lblColsAdm;
		private Label lblColsProc;
		private Label labPercent;
		private Label labColsAdm;
		private Label labColsProc;
        private GroupBox grNivelReduccion;
		private IReduccion reductor; 
		private bool preparado;
		private Thread miHilo;
        private ComboBox cmbNivel;
        private GroupBox groupBox2;
        private GroupBox groupBox1;

	    protected void ComienzaReduccion()
		{
			btnReducir.Enabled = false;
			hora0 = DateTime.Now;
			lblColsIni.Text = "-";
			lblColsProc.Text = "-";
			lblColsAdm.Text = "-";
			lblPercent.Text = "-";
			lblTempo.Text = "-";
			Application.DoEvents();
            nivelReduccion = Convert.ToInt32(cmbNivel.Text);

			maxCol = Convert.ToInt32(tBoxMaxCols.Text);
			percent = Convert.ToInt32(tBoxMaxPercent.Text);
 			switch (cBoxMetodo.Text)
			{
//				case "Hamming":
//					reductor  = new JDCRapido(false);
//					break;
    			case "grandes archivos":
        			reductor  = new Redu1305Xfsf(chkExternas.Checked);
          			break;
    			case "menos tiempo":
					reductor =new JDC(chkExternas.Checked);
         			break;
				case "menos columnas":
					reductor  = new JDCDobleContador(chkExternas.Checked);
					break;
				case "menos columnas 2":
					if(reductor==null) reductor  = new ReductorTM();
					break;
   			} 

			InicializaTimer();
			while(preparado==false){}
 			reductor.ComienzaReduccion(archivoEntrada, archivoSalida, nivelReduccion, maxCol, percent);
			ParaTimer();
			int tmp1 = reductor.NoColumnasIniciales;
			int tmp2 = reductor.NoColumnasProcesadas;
			int tmp3 = reductor.NoColumnasFinales;
			lblColsIni.Text = " = "+tmp1;
			lblColsProc.Text = " = "+tmp2;
			lblColsAdm.Text = " = "+tmp3;
			lblPercent.Text = " = "+(tmp2*100/tmp1);
			hora9 = DateTime.Now;
			lblTempo.Text = Convert.ToString(hora9-hora0);
			btnReducir.Enabled = true;			
		}

		protected void InicializaTimer()
		{		
			myTimer = new System.Windows.Forms.Timer ();
   		    myTimer.Tick += TimerEventProcessor;
       		myTimer.Interval = 3000;
       		myTimer.Start();		
		}
		
		protected void ParaTimer()
		{
			myTimer.Stop();		
		}

		protected void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
		{
			int tmp1 = reductor.NoColumnasIniciales;
			int tmp2 = reductor.NoColumnasProcesadas;
			int tmp3 = reductor.NoColumnasFinales;
			lblColsIni.Text = " = "+tmp1;
			lblColsProc.Text = " = "+tmp2;
			lblColsAdm.Text = " = "+tmp3;
			lblPercent.Text = " = "+(tmp2*100/tmp1);
			hora9 = DateTime.Now;
			lblTempo.Text = "t = "+(hora9-hora0);
		}

		public ReductorFrm()
		{
			InitializeComponent();
			InicializarCampos();
			btnReducir.Enabled = false;
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}

		protected void InicializarCampos()
		{
//			cBoxMetodo.Items.Add("Hamming");
			cBoxMetodo.Items.Add("menos tiempo");
			cBoxMetodo.Items.Add("menos columnas");
//			cBoxMetodo.Items.Add("menos columnas 2");
			cBoxMetodo.Items.Add("grandes archivos");
			cBoxMetodo.SelectedIndex = 0;
            for(int i = VariablesGlobales.NumeroPartidos - 1; i > 0; i--)
            {
                cmbNivel.Items.Add(i);
            }
            cmbNivel.SelectedIndex = 0;
		}



        void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReductorFrm));
            this.lblSelFile = new System.Windows.Forms.Label();
            this.tBoxMaxCols = new System.Windows.Forms.TextBox();
            this.labMaxCol = new System.Windows.Forms.Label();
            this.btnReducir = new System.Windows.Forms.Button();
            this.tBoxMaxPercent = new System.Windows.Forms.TextBox();
            this.labMaxPercent = new System.Windows.Forms.Label();
            this.btnSelFile = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.cBoxMetodo = new System.Windows.Forms.ComboBox();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.btnFileOutput = new System.Windows.Forms.Button();
            this.lblFicheroSalida = new System.Windows.Forms.Label();
            this.grNivelReduccion = new System.Windows.Forms.GroupBox();
            this.cmbNivel = new System.Windows.Forms.ComboBox();
            this.chkExternas = new System.Windows.Forms.CheckBox();
            this.lblColsIni = new System.Windows.Forms.Label();
            this.labColsIni = new System.Windows.Forms.Label();
            this.lblTempo = new System.Windows.Forms.Label();
            this.labTempo = new System.Windows.Forms.Label();
            this.lblPercent = new System.Windows.Forms.Label();
            this.lblColsAdm = new System.Windows.Forms.Label();
            this.lblColsProc = new System.Windows.Forms.Label();
            this.labPercent = new System.Windows.Forms.Label();
            this.labColsAdm = new System.Windows.Forms.Label();
            this.labColsProc = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox.SuspendLayout();
            this.grNivelReduccion.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblSelFile
            // 
            this.lblSelFile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblSelFile.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblSelFile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSelFile.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSelFile.Location = new System.Drawing.Point(132, 32);
            this.lblSelFile.Name = "lblSelFile";
            this.lblSelFile.Size = new System.Drawing.Size(314, 23);
            this.lblSelFile.TabIndex = 1;
            this.lblSelFile.Text = "(falta selección)";
            this.lblSelFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tBoxMaxCols
            // 
            this.tBoxMaxCols.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tBoxMaxCols.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tBoxMaxCols.Location = new System.Drawing.Point(112, 24);
            this.tBoxMaxCols.Name = "tBoxMaxCols";
            this.tBoxMaxCols.Size = new System.Drawing.Size(100, 21);
            this.tBoxMaxCols.TabIndex = 6;
            this.tBoxMaxCols.Text = "0";
            this.tBoxMaxCols.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labMaxCol
            // 
            this.labMaxCol.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labMaxCol.Location = new System.Drawing.Point(16, 24);
            this.labMaxCol.Name = "labMaxCol";
            this.labMaxCol.Size = new System.Drawing.Size(96, 21);
            this.labMaxCol.TabIndex = 3;
            this.labMaxCol.Text = "Máx. Columnas";
            this.labMaxCol.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnReducir
            // 
            this.btnReducir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReducir.BackColor = System.Drawing.Color.Silver;
            this.btnReducir.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnReducir.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReducir.Image = ((System.Drawing.Image)(resources.GetObject("btnReducir.Image")));
            this.btnReducir.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReducir.Location = new System.Drawing.Point(398, 301);
            this.btnReducir.Name = "btnReducir";
            this.btnReducir.Size = new System.Drawing.Size(82, 23);
            this.btnReducir.TabIndex = 2;
            this.btnReducir.Text = "Reducir";
            this.btnReducir.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnReducir.UseVisualStyleBackColor = false;
            this.btnReducir.Click += new System.EventHandler(this.BtnReducirClick);
            this.btnReducir.EnabledChanged += new System.EventHandler(this.btnReducir_EnabledChanged);
            // 
            // tBoxMaxPercent
            // 
            this.tBoxMaxPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tBoxMaxPercent.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tBoxMaxPercent.Location = new System.Drawing.Point(304, 22);
            this.tBoxMaxPercent.Name = "tBoxMaxPercent";
            this.tBoxMaxPercent.Size = new System.Drawing.Size(100, 21);
            this.tBoxMaxPercent.TabIndex = 7;
            this.tBoxMaxPercent.Text = "100";
            this.tBoxMaxPercent.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // labMaxPercent
            // 
            this.labMaxPercent.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labMaxPercent.Location = new System.Drawing.Point(243, 22);
            this.labMaxPercent.Name = "labMaxPercent";
            this.labMaxPercent.Size = new System.Drawing.Size(56, 21);
            this.labMaxPercent.TabIndex = 4;
            this.labMaxPercent.Text = "Máx. %";
            this.labMaxPercent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnSelFile
            // 
            this.btnSelFile.BackColor = System.Drawing.Color.LightSalmon;
            this.btnSelFile.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSelFile.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelFile.Image = ((System.Drawing.Image)(resources.GetObject("btnSelFile.Image")));
            this.btnSelFile.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSelFile.Location = new System.Drawing.Point(6, 32);
            this.btnSelFile.Name = "btnSelFile";
            this.btnSelFile.Size = new System.Drawing.Size(125, 23);
            this.btnSelFile.TabIndex = 0;
            this.btnSelFile.Text = "Archivo entrada";
            this.btnSelFile.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSelFile.UseVisualStyleBackColor = false;
            this.btnSelFile.Click += new System.EventHandler(this.BtnSelFileClick);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCancel.BackColor = System.Drawing.Color.LightSalmon;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCancel.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnCancel.Image")));
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(315, 301);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(82, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancelar";
            this.btnCancel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancel.UseVisualStyleBackColor = false;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancelClick);
            // 
            // cBoxMetodo
            // 
            this.cBoxMetodo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cBoxMetodo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cBoxMetodo.Location = new System.Drawing.Point(11, 23);
            this.cBoxMetodo.Name = "cBoxMetodo";
            this.cBoxMetodo.Size = new System.Drawing.Size(120, 21);
            this.cBoxMetodo.TabIndex = 9;
            this.cBoxMetodo.Text = "Menos tiempo";
            this.cBoxMetodo.SelectedIndexChanged += new System.EventHandler(this.cBoxMetodo_SelectedIndexChanged);
            // 
            // groupBox
            // 
            this.groupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox.Controls.Add(this.btnFileOutput);
            this.groupBox.Controls.Add(this.lblFicheroSalida);
            this.groupBox.Controls.Add(this.lblSelFile);
            this.groupBox.Controls.Add(this.btnSelFile);
            this.groupBox.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox.Location = new System.Drawing.Point(16, 9);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(465, 123);
            this.groupBox.TabIndex = 0;
            this.groupBox.TabStop = false;
            this.groupBox.Text = "Datos de Entrada";
            // 
            // btnFileOutput
            // 
            this.btnFileOutput.BackColor = System.Drawing.Color.LightSalmon;
            this.btnFileOutput.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnFileOutput.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnFileOutput.Image = ((System.Drawing.Image)(resources.GetObject("btnFileOutput.Image")));
            this.btnFileOutput.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFileOutput.Location = new System.Drawing.Point(6, 72);
            this.btnFileOutput.Name = "btnFileOutput";
            this.btnFileOutput.Size = new System.Drawing.Size(125, 23);
            this.btnFileOutput.TabIndex = 12;
            this.btnFileOutput.Text = "Archivo salida";
            this.btnFileOutput.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnFileOutput.UseVisualStyleBackColor = false;
            this.btnFileOutput.Click += new System.EventHandler(this.btnFileOutput_Click);
            // 
            // lblFicheroSalida
            // 
            this.lblFicheroSalida.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.lblFicheroSalida.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblFicheroSalida.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblFicheroSalida.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblFicheroSalida.Location = new System.Drawing.Point(132, 72);
            this.lblFicheroSalida.Name = "lblFicheroSalida";
            this.lblFicheroSalida.Size = new System.Drawing.Size(314, 23);
            this.lblFicheroSalida.TabIndex = 11;
            this.lblFicheroSalida.Text = "(falta selección)";
            this.lblFicheroSalida.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // grNivelReduccion
            // 
            this.grNivelReduccion.Controls.Add(this.cmbNivel);
            this.grNivelReduccion.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grNivelReduccion.ForeColor = System.Drawing.Color.Maroon;
            this.grNivelReduccion.Location = new System.Drawing.Point(11, 146);
            this.grNivelReduccion.Name = "grNivelReduccion";
            this.grNivelReduccion.Size = new System.Drawing.Size(127, 64);
            this.grNivelReduccion.TabIndex = 13;
            this.grNivelReduccion.TabStop = false;
            this.grNivelReduccion.Text = "Nivel Reduccción";
            // 
            // cmbNivel
            // 
            this.cmbNivel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.cmbNivel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbNivel.Location = new System.Drawing.Point(39, 22);
            this.cmbNivel.Name = "cmbNivel";
            this.cmbNivel.Size = new System.Drawing.Size(48, 21);
            this.cmbNivel.TabIndex = 10;
            // 
            // chkExternas
            // 
            this.chkExternas.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.chkExternas.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkExternas.Location = new System.Drawing.Point(137, 16);
            this.chkExternas.Name = "chkExternas";
            this.chkExternas.Size = new System.Drawing.Size(181, 32);
            this.chkExternas.TabIndex = 4;
            this.chkExternas.Text = "Admitir columnas externas";
            // 
            // lblColsIni
            // 
            this.lblColsIni.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblColsIni.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblColsIni.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblColsIni.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblColsIni.Location = new System.Drawing.Point(95, 332);
            this.lblColsIni.Name = "lblColsIni";
            this.lblColsIni.Size = new System.Drawing.Size(64, 24);
            this.lblColsIni.TabIndex = 6;
            this.lblColsIni.Text = "0";
            this.lblColsIni.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labColsIni
            // 
            this.labColsIni.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labColsIni.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labColsIni.Location = new System.Drawing.Point(5, 332);
            this.labColsIni.Name = "labColsIni";
            this.labColsIni.Size = new System.Drawing.Size(84, 23);
            this.labColsIni.TabIndex = 5;
            this.labColsIni.Text = "Col. Iniciales";
            this.labColsIni.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTempo
            // 
            this.lblTempo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTempo.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblTempo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTempo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTempo.Location = new System.Drawing.Point(364, 357);
            this.lblTempo.Name = "lblTempo";
            this.lblTempo.Size = new System.Drawing.Size(111, 24);
            this.lblTempo.TabIndex = 17;
            this.lblTempo.Text = "0";
            this.lblTempo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labTempo
            // 
            this.labTempo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labTempo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labTempo.Location = new System.Drawing.Point(303, 357);
            this.labTempo.Name = "labTempo";
            this.labTempo.Size = new System.Drawing.Size(55, 24);
            this.labTempo.TabIndex = 16;
            this.labTempo.Text = "Tiempo ";
            this.labTempo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPercent
            // 
            this.lblPercent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblPercent.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblPercent.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPercent.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPercent.Location = new System.Drawing.Point(237, 357);
            this.lblPercent.Name = "lblPercent";
            this.lblPercent.Size = new System.Drawing.Size(64, 23);
            this.lblPercent.TabIndex = 15;
            this.lblPercent.Text = "0";
            this.lblPercent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblColsAdm
            // 
            this.lblColsAdm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblColsAdm.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblColsAdm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblColsAdm.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblColsAdm.Location = new System.Drawing.Point(237, 333);
            this.lblColsAdm.Name = "lblColsAdm";
            this.lblColsAdm.Size = new System.Drawing.Size(64, 23);
            this.lblColsAdm.TabIndex = 14;
            this.lblColsAdm.Text = "0";
            this.lblColsAdm.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblColsProc
            // 
            this.lblColsProc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblColsProc.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblColsProc.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblColsProc.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblColsProc.Location = new System.Drawing.Point(95, 357);
            this.lblColsProc.Name = "lblColsProc";
            this.lblColsProc.Size = new System.Drawing.Size(64, 23);
            this.lblColsProc.TabIndex = 13;
            this.lblColsProc.Text = "0";
            this.lblColsProc.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labPercent
            // 
            this.labPercent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labPercent.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labPercent.Location = new System.Drawing.Point(165, 357);
            this.labPercent.Name = "labPercent";
            this.labPercent.Size = new System.Drawing.Size(73, 23);
            this.labPercent.TabIndex = 12;
            this.labPercent.Text = "Porcentaje";
            this.labPercent.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labColsAdm
            // 
            this.labColsAdm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labColsAdm.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labColsAdm.Location = new System.Drawing.Point(167, 333);
            this.labColsAdm.Name = "labColsAdm";
            this.labColsAdm.Size = new System.Drawing.Size(64, 23);
            this.labColsAdm.TabIndex = 11;
            this.labColsAdm.Text = "Admitidas";
            this.labColsAdm.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // labColsProc
            // 
            this.labColsProc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labColsProc.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labColsProc.Location = new System.Drawing.Point(5, 357);
            this.labColsProc.Name = "labColsProc";
            this.labColsProc.Size = new System.Drawing.Size(84, 23);
            this.labColsProc.TabIndex = 10;
            this.labColsProc.Text = "Procesadas";
            this.labColsProc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cBoxMetodo);
            this.groupBox1.Controls.Add(this.chkExternas);
            this.groupBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox1.Location = new System.Drawing.Point(144, 146);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(337, 64);
            this.groupBox1.TabIndex = 14;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Método Reduccción";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.labMaxCol);
            this.groupBox2.Controls.Add(this.labMaxPercent);
            this.groupBox2.Controls.Add(this.tBoxMaxCols);
            this.groupBox2.Controls.Add(this.tBoxMaxPercent);
            this.groupBox2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.ForeColor = System.Drawing.Color.Maroon;
            this.groupBox2.Location = new System.Drawing.Point(11, 216);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(470, 67);
            this.groupBox2.TabIndex = 15;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Opciones";
            // 
            // ReductorFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(491, 387);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.lblTempo);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.labTempo);
            this.Controls.Add(this.grNivelReduccion);
            this.Controls.Add(this.lblPercent);
            this.Controls.Add(this.lblColsAdm);
            this.Controls.Add(this.lblColsProc);
            this.Controls.Add(this.labPercent);
            this.Controls.Add(this.labColsAdm);
            this.Controls.Add(this.labColsProc);
            this.Controls.Add(this.lblColsIni);
            this.Controls.Add(this.labColsIni);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnReducir);
            this.Controls.Add(this.groupBox);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReductorFrm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Reductor";
            this.groupBox.ResumeLayout(false);
            this.grNivelReduccion.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }


		void BtnSelFileClick(object sender, EventArgs e)
		{
			OpenFileDialog abreFiltroDialog = new OpenFileDialog();
			abreFiltroDialog.InitialDirectory = "Columnas\\" ;
			abreFiltroDialog.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			if(abreFiltroDialog.ShowDialog() == DialogResult.OK)
		    {
		    	archivoEntrada = abreFiltroDialog.FileName;

                if (EsArchivoEntradaValido(archivoEntrada))
                {
                    lblSelFile.Text = Path.GetFileName(archivoEntrada);
                    if (cBoxMetodo.Text == "menos columnas 2") comprobarReduccionTM();
                }
                else 
                {
                    MessageBox.Show("El archivo de entrada debe tener 14 partidos.\nCompruebe además que no hay líneas en blanco adicionales al final del archivo.", "Error", MessageBoxButtons.OK,MessageBoxIcon.Warning);
                    
                }
				
			}
		    if (archivoSalida != "" && archivoEntrada != "") btnReducir.Enabled = true;
		}

        private bool EsArchivoEntradaValido(string aEntrada)
        {
            IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(aEntrada);
            try
            {
                comBaseCols.LeerTodasColsANumero();
            }
            catch
            {
                return false;
            }
            comBaseCols = new ArchivoColumnasTexto(aEntrada);
            int numSignos = comBaseCols.ObtenNumSignos();
            comBaseCols.Cerrar();
            return numSignos == 14;
        }

		private void inicializa()
		{
            nivelReduccion = Convert.ToInt32(cmbNivel.Text);
			reductor.Inicializa(archivoEntrada,nivelReduccion);
		}

		void BtnCancelClick(object sender, EventArgs e)
		{
			if (reductor!=null) reductor.Cancelar();
			Close();
		}

		void BtnReducirClick(object sender, EventArgs e)
		{
			ComienzaReduccion();
		}


		private void btnFileOutput_Click(object sender, EventArgs e)
		{
			SaveFileDialog abreFiltroDialog = new SaveFileDialog();
			abreFiltroDialog.InitialDirectory = "Columnas\\" ;
			abreFiltroDialog.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*" ;
			if(abreFiltroDialog.ShowDialog () == DialogResult.OK)
			{
				archivoSalida = abreFiltroDialog.FileName;		    	
				lblFicheroSalida.Text = Path.GetFileName(archivoSalida);
			}
			if (archivoSalida != "" && archivoEntrada != "") btnReducir.Enabled = true;
		}

		private void cBoxMetodo_SelectedIndexChanged(object sender, EventArgs e)
		{
			if(cBoxMetodo.Text =="grandes archivos" || cBoxMetodo.Text=="menos columnas 2")
			{
				chkExternas.Visible =false;
			}
			else
			{
				chkExternas.Visible =true;
			}
			// Si se cambia a Menos columnas 2, prepara la combinación
			comprobarReduccionTM();
		}

		private void btnReducir_EnabledChanged(object sender, EventArgs e)
		{
            FormulariosHelper f = new FormulariosHelper();
			f.CambiarFondoBoton(btnReducir);
		}

	    private void comprobarReduccionTM()
		{
			// Si se cambia a Menos columnas 2, prepara la combinación
			if(cBoxMetodo.Text=="menos columnas 2")
			{
				preparado=false;
				reductor  = new ReductorTM();
				miHilo=new Thread(inicializa);
				miHilo.Priority=ThreadPriority.BelowNormal ;
				miHilo.Start();
				while(miHilo.ThreadState==ThreadState.Running){}
			}
			preparado=true;
		}
	}
}
