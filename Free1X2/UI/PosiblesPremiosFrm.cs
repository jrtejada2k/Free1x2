using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Free1X2.EntradaSalida;
using Free1X2.Escrutinio;

namespace Free1X2.UI
{
	/// <summary>
	/// Summary description for PosiblesPremiosFrm.
	/// </summary>
	public class PosiblesPremiosFrm : Form
	{
        private Label lblP1C1;
        private Label lblP2C1;
        private Label lblP4C1;
        private Label lblP3C1;
        private Label lblP8C1;
        private Label lblP7C1;
        private Label lblP6C1;
        private Label lblP5C1;
        private Label lblP11C1;
        private Label lblP10C1;
        private Label lblP9C1;
        private Label lblP14C1;
        private Label lblP13C1;
        private Label lblP12C1;
        private Label lblP14C2;
        private Label lblP13C2;
        private Label lblP12C2;
        private Label lblP11C2;
        private Label lblP10C2;
        private Label lblP9C2;
        private Label lblP8C2;
        private Label lblP7C2;
        private Label lblP6C2;
        private Label lblP5C2;
        private Label lblP4C2;
        private Label lblP3C2;
        private Label lblP2C2;
        private Label lblP1C2;
        private Label lblP14C4;
        private Label lblP13C4;
        private Label lblP12C4;
        private Label lblP11C4;
        private Label lblP10C4;
        private Label lblP9C4;
        private Label lblP8C4;
        private Label lblP7C4;
        private Label lblP6C4;
        private Label lblP5C4;
        private Label lblP4C4;
        private Label lblP3C4;
        private Label lblP2C4;
        private Label lblP1C4;
        private Label lblP14C3;
        private Label lblP13C3;
        private Label lblP12C3;
        private Label lblP11C3;
        private Label lblP10C3;
        private Label lblP9C3;
        private Label lblP8C3;
        private Label lblP7C3;
        private Label lblP6C3;
        private Label lblP5C3;
        private Label lblP4C3;
        private Label lblP3C3;
        private Label lblP2C3;
        private Label lblP1C3;
        private Label lblP14C8;
        private Label lblP13C8;
        private Label lblP12C8;
        private Label lblP11C8;
        private Label lblP10C8;
        private Label lblP9C8;
        private Label lblP8C8;
        private Label lblP7C8;
        private Label lblP6C8;
        private Label lblP5C8;
        private Label lblP4C8;
        private Label lblP3C8;
        private Label lblP2C8;
        private Label lblP1C8;
        private Label lblP14C7;
        private Label lblP13C7;
        private Label lblP12C7;
        private Label lblP11C7;
        private Label lblP10C7;
        private Label lblP9C7;
        private Label lblP8C7;
        private Label lblP7C7;
        private Label lblP6C7;
        private Label lblP5C7;
        private Label lblP4C7;
        private Label lblP3C7;
        private Label lblP2C7;
        private Label lblP1C7;
        private Label lblP14C6;
        private Label lblP13C6;
        private Label lblP12C6;
        private Label lblP11C6;
        private Label lblP10C6;
        private Label lblP9C6;
        private Label lblP8C6;
        private Label lblP7C6;
        private Label lblP6C6;
        private Label lblP5C6;
        private Label lblP4C6;
        private Label lblP3C6;
        private Label lblP2C6;
        private Label lblP1C6;
        private Label lblP14C5;
        private Label lblP13C5;
        private Label lblP12C5;
        private Label lblP11C5;
        private Label lblP10C5;
        private Label lblP9C5;
        private Label lblP8C5;
        private Label lblP7C5;
        private Label lblP6C5;
        private Label lblP5C5;
        private Label lblP4C5;
        private Label lblP3C5;
        private Label lblP2C5;
        private Label lblP1C5;
        private Label lblP8;
        private Label lblP7;
        private Label lblP6;
        private Label lblP5;
        private Label lblP4;
        private Label lblP3;
        private Label lblP2;
        private Label lblP1;
        private TextBox txt1;
        private TextBox txt2;
        private TextBox txt4;
        private TextBox txt3;
        private TextBox txt8;
        private TextBox txt7;
        private TextBox txt6;
        private TextBox txt5;
        private TextBox txt11;
        private TextBox txt10;
        private TextBox txt9;
        private TextBox txt14;
        private TextBox txt13;
        private TextBox txt12;
        private Button btnAdelante;
        private Button btnAtras;
        private Button btnAbreArchivo;
        private Label lblNombreArchivo;
        private Button btnCalcular;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
        List<string> arrayColumnas = new List<string>();
		private Label lblColumnasConPremio;
        List<string> col16 = new List<string>();
        List<string> col15 = new List<string>();
        List<string> col14 = new List<string>();
        List<string> col13 = new List<string>();
        List<string> col12 = new List<string>();
        List<string> col11 = new List<string>();
	    private List<string> col10 = new List<string>();
		ArrayList arrayPremiadas = new ArrayList();
		List<PosiblesPremiosContenedor> resumen = new List<PosiblesPremiosContenedor>();
		int noBoleto = 1;
		int noCol;
		private Label lblPremios14;
		private Label lblPremios12;
		private Label lblPremios13;
		private Label lblPremios10;
		private Label lblPremios11;
		int noBoletos;
		private Button btnCopiar;
		private CheckBox ckbResumen;
		int signosNoDefinitivos;
		private Button btnVer;
        private Button btnGuardar;
		private Button btnMejoresOpciones;
        string columnaGanadora;
        private TextBox txt16;
        private TextBox txt15;
        private Label lblP16C8;
        private Label lblP15C8;
        private Label lblP16C7;
        private Label lblP15C7;
        private Label lblP16C6;
        private Label lblP15C6;
        private Label lblP16C5;
        private Label lblP15C5;
        private Label lblP16C4;
        private Label lblP15C4;
        private Label lblP16C3;
        private Label lblP15C3;
        private Label lblP16C2;
        private Label lblP15C2;
        private Label lblP16C1;
        private Label lblP15C1;
        private Label lblPremios15;
        private Label lblPremios16;
        private CheckBox chkPleno;
        private Label P1;
        private Label P2;
        private Label P4;
        private Label P3;
        private Label P8;
        private Label P7;
        private Label P6;
        private Label P5;
        private Label P11;
        private Label P10;
        private Label P9;
        private Label P14;
        private Label P13;
        private Label P12;
        private Label P16;
        private Label P15;
        int noPartidos;

		public PosiblesPremiosFrm()
		{
		    signosNoDefinitivos = 0;
		    noBoletos = 0;
		    noCol = 0;
		    noPartidos = 0;
		    InitializeComponent();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PosiblesPremiosFrm));
            this.lblP1C1 = new System.Windows.Forms.Label();
            this.lblP2C1 = new System.Windows.Forms.Label();
            this.lblP4C1 = new System.Windows.Forms.Label();
            this.lblP3C1 = new System.Windows.Forms.Label();
            this.lblP8C1 = new System.Windows.Forms.Label();
            this.lblP7C1 = new System.Windows.Forms.Label();
            this.lblP6C1 = new System.Windows.Forms.Label();
            this.lblP5C1 = new System.Windows.Forms.Label();
            this.lblP11C1 = new System.Windows.Forms.Label();
            this.lblP10C1 = new System.Windows.Forms.Label();
            this.lblP9C1 = new System.Windows.Forms.Label();
            this.lblP14C1 = new System.Windows.Forms.Label();
            this.lblP13C1 = new System.Windows.Forms.Label();
            this.lblP12C1 = new System.Windows.Forms.Label();
            this.lblP14C2 = new System.Windows.Forms.Label();
            this.lblP13C2 = new System.Windows.Forms.Label();
            this.lblP12C2 = new System.Windows.Forms.Label();
            this.lblP11C2 = new System.Windows.Forms.Label();
            this.lblP10C2 = new System.Windows.Forms.Label();
            this.lblP9C2 = new System.Windows.Forms.Label();
            this.lblP8C2 = new System.Windows.Forms.Label();
            this.lblP7C2 = new System.Windows.Forms.Label();
            this.lblP6C2 = new System.Windows.Forms.Label();
            this.lblP5C2 = new System.Windows.Forms.Label();
            this.lblP4C2 = new System.Windows.Forms.Label();
            this.lblP3C2 = new System.Windows.Forms.Label();
            this.lblP2C2 = new System.Windows.Forms.Label();
            this.lblP1C2 = new System.Windows.Forms.Label();
            this.lblP14C4 = new System.Windows.Forms.Label();
            this.lblP13C4 = new System.Windows.Forms.Label();
            this.lblP12C4 = new System.Windows.Forms.Label();
            this.lblP11C4 = new System.Windows.Forms.Label();
            this.lblP10C4 = new System.Windows.Forms.Label();
            this.lblP9C4 = new System.Windows.Forms.Label();
            this.lblP8C4 = new System.Windows.Forms.Label();
            this.lblP7C4 = new System.Windows.Forms.Label();
            this.lblP6C4 = new System.Windows.Forms.Label();
            this.lblP5C4 = new System.Windows.Forms.Label();
            this.lblP4C4 = new System.Windows.Forms.Label();
            this.lblP3C4 = new System.Windows.Forms.Label();
            this.lblP2C4 = new System.Windows.Forms.Label();
            this.lblP1C4 = new System.Windows.Forms.Label();
            this.lblP14C3 = new System.Windows.Forms.Label();
            this.lblP13C3 = new System.Windows.Forms.Label();
            this.lblP12C3 = new System.Windows.Forms.Label();
            this.lblP11C3 = new System.Windows.Forms.Label();
            this.lblP10C3 = new System.Windows.Forms.Label();
            this.lblP9C3 = new System.Windows.Forms.Label();
            this.lblP8C3 = new System.Windows.Forms.Label();
            this.lblP7C3 = new System.Windows.Forms.Label();
            this.lblP6C3 = new System.Windows.Forms.Label();
            this.lblP5C3 = new System.Windows.Forms.Label();
            this.lblP4C3 = new System.Windows.Forms.Label();
            this.lblP3C3 = new System.Windows.Forms.Label();
            this.lblP2C3 = new System.Windows.Forms.Label();
            this.lblP1C3 = new System.Windows.Forms.Label();
            this.lblP14C8 = new System.Windows.Forms.Label();
            this.lblP13C8 = new System.Windows.Forms.Label();
            this.lblP12C8 = new System.Windows.Forms.Label();
            this.lblP11C8 = new System.Windows.Forms.Label();
            this.lblP10C8 = new System.Windows.Forms.Label();
            this.lblP9C8 = new System.Windows.Forms.Label();
            this.lblP8C8 = new System.Windows.Forms.Label();
            this.lblP7C8 = new System.Windows.Forms.Label();
            this.lblP6C8 = new System.Windows.Forms.Label();
            this.lblP5C8 = new System.Windows.Forms.Label();
            this.lblP4C8 = new System.Windows.Forms.Label();
            this.lblP3C8 = new System.Windows.Forms.Label();
            this.lblP2C8 = new System.Windows.Forms.Label();
            this.lblP1C8 = new System.Windows.Forms.Label();
            this.lblP14C7 = new System.Windows.Forms.Label();
            this.lblP13C7 = new System.Windows.Forms.Label();
            this.lblP12C7 = new System.Windows.Forms.Label();
            this.lblP11C7 = new System.Windows.Forms.Label();
            this.lblP10C7 = new System.Windows.Forms.Label();
            this.lblP9C7 = new System.Windows.Forms.Label();
            this.lblP8C7 = new System.Windows.Forms.Label();
            this.lblP7C7 = new System.Windows.Forms.Label();
            this.lblP6C7 = new System.Windows.Forms.Label();
            this.lblP5C7 = new System.Windows.Forms.Label();
            this.lblP4C7 = new System.Windows.Forms.Label();
            this.lblP3C7 = new System.Windows.Forms.Label();
            this.lblP2C7 = new System.Windows.Forms.Label();
            this.lblP1C7 = new System.Windows.Forms.Label();
            this.lblP14C6 = new System.Windows.Forms.Label();
            this.lblP13C6 = new System.Windows.Forms.Label();
            this.lblP12C6 = new System.Windows.Forms.Label();
            this.lblP11C6 = new System.Windows.Forms.Label();
            this.lblP10C6 = new System.Windows.Forms.Label();
            this.lblP9C6 = new System.Windows.Forms.Label();
            this.lblP8C6 = new System.Windows.Forms.Label();
            this.lblP7C6 = new System.Windows.Forms.Label();
            this.lblP6C6 = new System.Windows.Forms.Label();
            this.lblP5C6 = new System.Windows.Forms.Label();
            this.lblP4C6 = new System.Windows.Forms.Label();
            this.lblP3C6 = new System.Windows.Forms.Label();
            this.lblP2C6 = new System.Windows.Forms.Label();
            this.lblP1C6 = new System.Windows.Forms.Label();
            this.lblP14C5 = new System.Windows.Forms.Label();
            this.lblP13C5 = new System.Windows.Forms.Label();
            this.lblP12C5 = new System.Windows.Forms.Label();
            this.lblP11C5 = new System.Windows.Forms.Label();
            this.lblP10C5 = new System.Windows.Forms.Label();
            this.lblP9C5 = new System.Windows.Forms.Label();
            this.lblP8C5 = new System.Windows.Forms.Label();
            this.lblP7C5 = new System.Windows.Forms.Label();
            this.lblP6C5 = new System.Windows.Forms.Label();
            this.lblP5C5 = new System.Windows.Forms.Label();
            this.lblP4C5 = new System.Windows.Forms.Label();
            this.lblP3C5 = new System.Windows.Forms.Label();
            this.lblP2C5 = new System.Windows.Forms.Label();
            this.lblP1C5 = new System.Windows.Forms.Label();
            this.lblP8 = new System.Windows.Forms.Label();
            this.lblP7 = new System.Windows.Forms.Label();
            this.lblP6 = new System.Windows.Forms.Label();
            this.lblP5 = new System.Windows.Forms.Label();
            this.lblP4 = new System.Windows.Forms.Label();
            this.lblP3 = new System.Windows.Forms.Label();
            this.lblP2 = new System.Windows.Forms.Label();
            this.lblP1 = new System.Windows.Forms.Label();
            this.txt1 = new System.Windows.Forms.TextBox();
            this.txt2 = new System.Windows.Forms.TextBox();
            this.txt4 = new System.Windows.Forms.TextBox();
            this.txt3 = new System.Windows.Forms.TextBox();
            this.txt8 = new System.Windows.Forms.TextBox();
            this.txt7 = new System.Windows.Forms.TextBox();
            this.txt6 = new System.Windows.Forms.TextBox();
            this.txt5 = new System.Windows.Forms.TextBox();
            this.txt11 = new System.Windows.Forms.TextBox();
            this.txt10 = new System.Windows.Forms.TextBox();
            this.txt9 = new System.Windows.Forms.TextBox();
            this.txt14 = new System.Windows.Forms.TextBox();
            this.txt13 = new System.Windows.Forms.TextBox();
            this.txt12 = new System.Windows.Forms.TextBox();
            this.btnAdelante = new System.Windows.Forms.Button();
            this.btnAtras = new System.Windows.Forms.Button();
            this.btnAbreArchivo = new System.Windows.Forms.Button();
            this.lblNombreArchivo = new System.Windows.Forms.Label();
            this.btnCalcular = new System.Windows.Forms.Button();
            this.lblColumnasConPremio = new System.Windows.Forms.Label();
            this.lblPremios14 = new System.Windows.Forms.Label();
            this.lblPremios12 = new System.Windows.Forms.Label();
            this.lblPremios13 = new System.Windows.Forms.Label();
            this.lblPremios10 = new System.Windows.Forms.Label();
            this.lblPremios11 = new System.Windows.Forms.Label();
            this.btnCopiar = new System.Windows.Forms.Button();
            this.ckbResumen = new System.Windows.Forms.CheckBox();
            this.btnVer = new System.Windows.Forms.Button();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnMejoresOpciones = new System.Windows.Forms.Button();
            this.txt16 = new System.Windows.Forms.TextBox();
            this.txt15 = new System.Windows.Forms.TextBox();
            this.lblP16C8 = new System.Windows.Forms.Label();
            this.lblP15C8 = new System.Windows.Forms.Label();
            this.lblP16C7 = new System.Windows.Forms.Label();
            this.lblP15C7 = new System.Windows.Forms.Label();
            this.lblP16C6 = new System.Windows.Forms.Label();
            this.lblP15C6 = new System.Windows.Forms.Label();
            this.lblP16C5 = new System.Windows.Forms.Label();
            this.lblP15C5 = new System.Windows.Forms.Label();
            this.lblP16C4 = new System.Windows.Forms.Label();
            this.lblP15C4 = new System.Windows.Forms.Label();
            this.lblP16C3 = new System.Windows.Forms.Label();
            this.lblP15C3 = new System.Windows.Forms.Label();
            this.lblP16C2 = new System.Windows.Forms.Label();
            this.lblP15C2 = new System.Windows.Forms.Label();
            this.lblP16C1 = new System.Windows.Forms.Label();
            this.lblP15C1 = new System.Windows.Forms.Label();
            this.lblPremios15 = new System.Windows.Forms.Label();
            this.lblPremios16 = new System.Windows.Forms.Label();
            this.chkPleno = new System.Windows.Forms.CheckBox();
            this.P1 = new System.Windows.Forms.Label();
            this.P2 = new System.Windows.Forms.Label();
            this.P4 = new System.Windows.Forms.Label();
            this.P3 = new System.Windows.Forms.Label();
            this.P8 = new System.Windows.Forms.Label();
            this.P7 = new System.Windows.Forms.Label();
            this.P6 = new System.Windows.Forms.Label();
            this.P5 = new System.Windows.Forms.Label();
            this.P11 = new System.Windows.Forms.Label();
            this.P10 = new System.Windows.Forms.Label();
            this.P9 = new System.Windows.Forms.Label();
            this.P14 = new System.Windows.Forms.Label();
            this.P13 = new System.Windows.Forms.Label();
            this.P12 = new System.Windows.Forms.Label();
            this.P16 = new System.Windows.Forms.Label();
            this.P15 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblP1C1
            // 
            this.lblP1C1.BackColor = System.Drawing.Color.Khaki;
            this.lblP1C1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP1C1.Location = new System.Drawing.Point(86, 95);
            this.lblP1C1.Name = "lblP1C1";
            this.lblP1C1.Size = new System.Drawing.Size(20, 20);
            this.lblP1C1.TabIndex = 0;
            this.lblP1C1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP2C1
            // 
            this.lblP2C1.BackColor = System.Drawing.Color.Khaki;
            this.lblP2C1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP2C1.Location = new System.Drawing.Point(86, 119);
            this.lblP2C1.Name = "lblP2C1";
            this.lblP2C1.Size = new System.Drawing.Size(20, 20);
            this.lblP2C1.TabIndex = 1;
            this.lblP2C1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP4C1
            // 
            this.lblP4C1.BackColor = System.Drawing.Color.Khaki;
            this.lblP4C1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP4C1.Location = new System.Drawing.Point(86, 167);
            this.lblP4C1.Name = "lblP4C1";
            this.lblP4C1.Size = new System.Drawing.Size(20, 20);
            this.lblP4C1.TabIndex = 3;
            this.lblP4C1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP3C1
            // 
            this.lblP3C1.BackColor = System.Drawing.Color.Khaki;
            this.lblP3C1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP3C1.Location = new System.Drawing.Point(86, 143);
            this.lblP3C1.Name = "lblP3C1";
            this.lblP3C1.Size = new System.Drawing.Size(20, 20);
            this.lblP3C1.TabIndex = 2;
            this.lblP3C1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP8C1
            // 
            this.lblP8C1.BackColor = System.Drawing.Color.Khaki;
            this.lblP8C1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP8C1.Location = new System.Drawing.Point(86, 271);
            this.lblP8C1.Name = "lblP8C1";
            this.lblP8C1.Size = new System.Drawing.Size(20, 20);
            this.lblP8C1.TabIndex = 7;
            this.lblP8C1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP7C1
            // 
            this.lblP7C1.BackColor = System.Drawing.Color.Khaki;
            this.lblP7C1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP7C1.Location = new System.Drawing.Point(86, 247);
            this.lblP7C1.Name = "lblP7C1";
            this.lblP7C1.Size = new System.Drawing.Size(20, 20);
            this.lblP7C1.TabIndex = 6;
            this.lblP7C1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP6C1
            // 
            this.lblP6C1.BackColor = System.Drawing.Color.Khaki;
            this.lblP6C1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP6C1.Location = new System.Drawing.Point(86, 223);
            this.lblP6C1.Name = "lblP6C1";
            this.lblP6C1.Size = new System.Drawing.Size(20, 20);
            this.lblP6C1.TabIndex = 5;
            this.lblP6C1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP5C1
            // 
            this.lblP5C1.BackColor = System.Drawing.Color.Khaki;
            this.lblP5C1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP5C1.Location = new System.Drawing.Point(86, 199);
            this.lblP5C1.Name = "lblP5C1";
            this.lblP5C1.Size = new System.Drawing.Size(20, 20);
            this.lblP5C1.TabIndex = 4;
            this.lblP5C1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP11C1
            // 
            this.lblP11C1.BackColor = System.Drawing.Color.Khaki;
            this.lblP11C1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP11C1.Location = new System.Drawing.Point(86, 351);
            this.lblP11C1.Name = "lblP11C1";
            this.lblP11C1.Size = new System.Drawing.Size(20, 20);
            this.lblP11C1.TabIndex = 10;
            this.lblP11C1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP10C1
            // 
            this.lblP10C1.BackColor = System.Drawing.Color.Khaki;
            this.lblP10C1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP10C1.Location = new System.Drawing.Point(86, 327);
            this.lblP10C1.Name = "lblP10C1";
            this.lblP10C1.Size = new System.Drawing.Size(20, 20);
            this.lblP10C1.TabIndex = 9;
            this.lblP10C1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP9C1
            // 
            this.lblP9C1.BackColor = System.Drawing.Color.Khaki;
            this.lblP9C1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP9C1.Location = new System.Drawing.Point(86, 303);
            this.lblP9C1.Name = "lblP9C1";
            this.lblP9C1.Size = new System.Drawing.Size(20, 20);
            this.lblP9C1.TabIndex = 8;
            this.lblP9C1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP14C1
            // 
            this.lblP14C1.BackColor = System.Drawing.Color.Khaki;
            this.lblP14C1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP14C1.Location = new System.Drawing.Point(86, 431);
            this.lblP14C1.Name = "lblP14C1";
            this.lblP14C1.Size = new System.Drawing.Size(20, 20);
            this.lblP14C1.TabIndex = 13;
            this.lblP14C1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP13C1
            // 
            this.lblP13C1.BackColor = System.Drawing.Color.Khaki;
            this.lblP13C1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP13C1.Location = new System.Drawing.Point(86, 407);
            this.lblP13C1.Name = "lblP13C1";
            this.lblP13C1.Size = new System.Drawing.Size(20, 20);
            this.lblP13C1.TabIndex = 12;
            this.lblP13C1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP12C1
            // 
            this.lblP12C1.BackColor = System.Drawing.Color.Khaki;
            this.lblP12C1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP12C1.Location = new System.Drawing.Point(86, 383);
            this.lblP12C1.Name = "lblP12C1";
            this.lblP12C1.Size = new System.Drawing.Size(20, 20);
            this.lblP12C1.TabIndex = 11;
            this.lblP12C1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP14C2
            // 
            this.lblP14C2.BackColor = System.Drawing.Color.Khaki;
            this.lblP14C2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP14C2.Location = new System.Drawing.Point(118, 431);
            this.lblP14C2.Name = "lblP14C2";
            this.lblP14C2.Size = new System.Drawing.Size(20, 20);
            this.lblP14C2.TabIndex = 27;
            this.lblP14C2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP13C2
            // 
            this.lblP13C2.BackColor = System.Drawing.Color.Khaki;
            this.lblP13C2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP13C2.Location = new System.Drawing.Point(118, 407);
            this.lblP13C2.Name = "lblP13C2";
            this.lblP13C2.Size = new System.Drawing.Size(20, 20);
            this.lblP13C2.TabIndex = 26;
            this.lblP13C2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP12C2
            // 
            this.lblP12C2.BackColor = System.Drawing.Color.Khaki;
            this.lblP12C2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP12C2.Location = new System.Drawing.Point(118, 383);
            this.lblP12C2.Name = "lblP12C2";
            this.lblP12C2.Size = new System.Drawing.Size(20, 20);
            this.lblP12C2.TabIndex = 25;
            this.lblP12C2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP11C2
            // 
            this.lblP11C2.BackColor = System.Drawing.Color.Khaki;
            this.lblP11C2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP11C2.Location = new System.Drawing.Point(118, 351);
            this.lblP11C2.Name = "lblP11C2";
            this.lblP11C2.Size = new System.Drawing.Size(20, 20);
            this.lblP11C2.TabIndex = 24;
            this.lblP11C2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP10C2
            // 
            this.lblP10C2.BackColor = System.Drawing.Color.Khaki;
            this.lblP10C2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP10C2.Location = new System.Drawing.Point(118, 327);
            this.lblP10C2.Name = "lblP10C2";
            this.lblP10C2.Size = new System.Drawing.Size(20, 20);
            this.lblP10C2.TabIndex = 23;
            this.lblP10C2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP9C2
            // 
            this.lblP9C2.BackColor = System.Drawing.Color.Khaki;
            this.lblP9C2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP9C2.Location = new System.Drawing.Point(118, 303);
            this.lblP9C2.Name = "lblP9C2";
            this.lblP9C2.Size = new System.Drawing.Size(20, 20);
            this.lblP9C2.TabIndex = 22;
            this.lblP9C2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP8C2
            // 
            this.lblP8C2.BackColor = System.Drawing.Color.Khaki;
            this.lblP8C2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP8C2.Location = new System.Drawing.Point(118, 271);
            this.lblP8C2.Name = "lblP8C2";
            this.lblP8C2.Size = new System.Drawing.Size(20, 20);
            this.lblP8C2.TabIndex = 21;
            this.lblP8C2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP7C2
            // 
            this.lblP7C2.BackColor = System.Drawing.Color.Khaki;
            this.lblP7C2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP7C2.Location = new System.Drawing.Point(118, 247);
            this.lblP7C2.Name = "lblP7C2";
            this.lblP7C2.Size = new System.Drawing.Size(20, 20);
            this.lblP7C2.TabIndex = 20;
            this.lblP7C2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP6C2
            // 
            this.lblP6C2.BackColor = System.Drawing.Color.Khaki;
            this.lblP6C2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP6C2.Location = new System.Drawing.Point(118, 223);
            this.lblP6C2.Name = "lblP6C2";
            this.lblP6C2.Size = new System.Drawing.Size(20, 20);
            this.lblP6C2.TabIndex = 19;
            this.lblP6C2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP5C2
            // 
            this.lblP5C2.BackColor = System.Drawing.Color.Khaki;
            this.lblP5C2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP5C2.Location = new System.Drawing.Point(118, 199);
            this.lblP5C2.Name = "lblP5C2";
            this.lblP5C2.Size = new System.Drawing.Size(20, 20);
            this.lblP5C2.TabIndex = 18;
            this.lblP5C2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP4C2
            // 
            this.lblP4C2.BackColor = System.Drawing.Color.Khaki;
            this.lblP4C2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP4C2.Location = new System.Drawing.Point(118, 167);
            this.lblP4C2.Name = "lblP4C2";
            this.lblP4C2.Size = new System.Drawing.Size(20, 20);
            this.lblP4C2.TabIndex = 17;
            this.lblP4C2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP3C2
            // 
            this.lblP3C2.BackColor = System.Drawing.Color.Khaki;
            this.lblP3C2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP3C2.Location = new System.Drawing.Point(118, 143);
            this.lblP3C2.Name = "lblP3C2";
            this.lblP3C2.Size = new System.Drawing.Size(20, 20);
            this.lblP3C2.TabIndex = 16;
            this.lblP3C2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP2C2
            // 
            this.lblP2C2.BackColor = System.Drawing.Color.Khaki;
            this.lblP2C2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP2C2.Location = new System.Drawing.Point(118, 119);
            this.lblP2C2.Name = "lblP2C2";
            this.lblP2C2.Size = new System.Drawing.Size(20, 20);
            this.lblP2C2.TabIndex = 15;
            this.lblP2C2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP1C2
            // 
            this.lblP1C2.BackColor = System.Drawing.Color.Khaki;
            this.lblP1C2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP1C2.Location = new System.Drawing.Point(118, 95);
            this.lblP1C2.Name = "lblP1C2";
            this.lblP1C2.Size = new System.Drawing.Size(20, 20);
            this.lblP1C2.TabIndex = 14;
            this.lblP1C2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP14C4
            // 
            this.lblP14C4.BackColor = System.Drawing.Color.Khaki;
            this.lblP14C4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP14C4.Location = new System.Drawing.Point(182, 431);
            this.lblP14C4.Name = "lblP14C4";
            this.lblP14C4.Size = new System.Drawing.Size(20, 20);
            this.lblP14C4.TabIndex = 55;
            this.lblP14C4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP13C4
            // 
            this.lblP13C4.BackColor = System.Drawing.Color.Khaki;
            this.lblP13C4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP13C4.Location = new System.Drawing.Point(182, 407);
            this.lblP13C4.Name = "lblP13C4";
            this.lblP13C4.Size = new System.Drawing.Size(20, 20);
            this.lblP13C4.TabIndex = 54;
            this.lblP13C4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP12C4
            // 
            this.lblP12C4.BackColor = System.Drawing.Color.Khaki;
            this.lblP12C4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP12C4.Location = new System.Drawing.Point(182, 383);
            this.lblP12C4.Name = "lblP12C4";
            this.lblP12C4.Size = new System.Drawing.Size(20, 20);
            this.lblP12C4.TabIndex = 53;
            this.lblP12C4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP11C4
            // 
            this.lblP11C4.BackColor = System.Drawing.Color.Khaki;
            this.lblP11C4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP11C4.Location = new System.Drawing.Point(182, 351);
            this.lblP11C4.Name = "lblP11C4";
            this.lblP11C4.Size = new System.Drawing.Size(20, 20);
            this.lblP11C4.TabIndex = 52;
            this.lblP11C4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP10C4
            // 
            this.lblP10C4.BackColor = System.Drawing.Color.Khaki;
            this.lblP10C4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP10C4.Location = new System.Drawing.Point(182, 327);
            this.lblP10C4.Name = "lblP10C4";
            this.lblP10C4.Size = new System.Drawing.Size(20, 20);
            this.lblP10C4.TabIndex = 51;
            this.lblP10C4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP9C4
            // 
            this.lblP9C4.BackColor = System.Drawing.Color.Khaki;
            this.lblP9C4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP9C4.Location = new System.Drawing.Point(182, 303);
            this.lblP9C4.Name = "lblP9C4";
            this.lblP9C4.Size = new System.Drawing.Size(20, 20);
            this.lblP9C4.TabIndex = 50;
            this.lblP9C4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP8C4
            // 
            this.lblP8C4.BackColor = System.Drawing.Color.Khaki;
            this.lblP8C4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP8C4.Location = new System.Drawing.Point(182, 271);
            this.lblP8C4.Name = "lblP8C4";
            this.lblP8C4.Size = new System.Drawing.Size(20, 20);
            this.lblP8C4.TabIndex = 49;
            this.lblP8C4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP7C4
            // 
            this.lblP7C4.BackColor = System.Drawing.Color.Khaki;
            this.lblP7C4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP7C4.Location = new System.Drawing.Point(182, 247);
            this.lblP7C4.Name = "lblP7C4";
            this.lblP7C4.Size = new System.Drawing.Size(20, 20);
            this.lblP7C4.TabIndex = 48;
            this.lblP7C4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP6C4
            // 
            this.lblP6C4.BackColor = System.Drawing.Color.Khaki;
            this.lblP6C4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP6C4.Location = new System.Drawing.Point(182, 223);
            this.lblP6C4.Name = "lblP6C4";
            this.lblP6C4.Size = new System.Drawing.Size(20, 20);
            this.lblP6C4.TabIndex = 47;
            this.lblP6C4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP5C4
            // 
            this.lblP5C4.BackColor = System.Drawing.Color.Khaki;
            this.lblP5C4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP5C4.Location = new System.Drawing.Point(182, 199);
            this.lblP5C4.Name = "lblP5C4";
            this.lblP5C4.Size = new System.Drawing.Size(20, 20);
            this.lblP5C4.TabIndex = 46;
            this.lblP5C4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP4C4
            // 
            this.lblP4C4.BackColor = System.Drawing.Color.Khaki;
            this.lblP4C4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP4C4.Location = new System.Drawing.Point(182, 167);
            this.lblP4C4.Name = "lblP4C4";
            this.lblP4C4.Size = new System.Drawing.Size(20, 20);
            this.lblP4C4.TabIndex = 45;
            this.lblP4C4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP3C4
            // 
            this.lblP3C4.BackColor = System.Drawing.Color.Khaki;
            this.lblP3C4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP3C4.Location = new System.Drawing.Point(182, 143);
            this.lblP3C4.Name = "lblP3C4";
            this.lblP3C4.Size = new System.Drawing.Size(20, 20);
            this.lblP3C4.TabIndex = 44;
            this.lblP3C4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP2C4
            // 
            this.lblP2C4.BackColor = System.Drawing.Color.Khaki;
            this.lblP2C4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP2C4.Location = new System.Drawing.Point(182, 119);
            this.lblP2C4.Name = "lblP2C4";
            this.lblP2C4.Size = new System.Drawing.Size(20, 20);
            this.lblP2C4.TabIndex = 43;
            this.lblP2C4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP1C4
            // 
            this.lblP1C4.BackColor = System.Drawing.Color.Khaki;
            this.lblP1C4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP1C4.Location = new System.Drawing.Point(182, 95);
            this.lblP1C4.Name = "lblP1C4";
            this.lblP1C4.Size = new System.Drawing.Size(20, 20);
            this.lblP1C4.TabIndex = 42;
            this.lblP1C4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP14C3
            // 
            this.lblP14C3.BackColor = System.Drawing.Color.Khaki;
            this.lblP14C3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP14C3.Location = new System.Drawing.Point(150, 431);
            this.lblP14C3.Name = "lblP14C3";
            this.lblP14C3.Size = new System.Drawing.Size(20, 20);
            this.lblP14C3.TabIndex = 41;
            this.lblP14C3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP13C3
            // 
            this.lblP13C3.BackColor = System.Drawing.Color.Khaki;
            this.lblP13C3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP13C3.Location = new System.Drawing.Point(150, 407);
            this.lblP13C3.Name = "lblP13C3";
            this.lblP13C3.Size = new System.Drawing.Size(20, 20);
            this.lblP13C3.TabIndex = 40;
            this.lblP13C3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP12C3
            // 
            this.lblP12C3.BackColor = System.Drawing.Color.Khaki;
            this.lblP12C3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP12C3.Location = new System.Drawing.Point(150, 383);
            this.lblP12C3.Name = "lblP12C3";
            this.lblP12C3.Size = new System.Drawing.Size(20, 20);
            this.lblP12C3.TabIndex = 39;
            this.lblP12C3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP11C3
            // 
            this.lblP11C3.BackColor = System.Drawing.Color.Khaki;
            this.lblP11C3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP11C3.Location = new System.Drawing.Point(150, 351);
            this.lblP11C3.Name = "lblP11C3";
            this.lblP11C3.Size = new System.Drawing.Size(20, 20);
            this.lblP11C3.TabIndex = 38;
            this.lblP11C3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP10C3
            // 
            this.lblP10C3.BackColor = System.Drawing.Color.Khaki;
            this.lblP10C3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP10C3.Location = new System.Drawing.Point(150, 327);
            this.lblP10C3.Name = "lblP10C3";
            this.lblP10C3.Size = new System.Drawing.Size(20, 20);
            this.lblP10C3.TabIndex = 37;
            this.lblP10C3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP9C3
            // 
            this.lblP9C3.BackColor = System.Drawing.Color.Khaki;
            this.lblP9C3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP9C3.Location = new System.Drawing.Point(150, 303);
            this.lblP9C3.Name = "lblP9C3";
            this.lblP9C3.Size = new System.Drawing.Size(20, 20);
            this.lblP9C3.TabIndex = 36;
            this.lblP9C3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP8C3
            // 
            this.lblP8C3.BackColor = System.Drawing.Color.Khaki;
            this.lblP8C3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP8C3.Location = new System.Drawing.Point(150, 271);
            this.lblP8C3.Name = "lblP8C3";
            this.lblP8C3.Size = new System.Drawing.Size(20, 20);
            this.lblP8C3.TabIndex = 35;
            this.lblP8C3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP7C3
            // 
            this.lblP7C3.BackColor = System.Drawing.Color.Khaki;
            this.lblP7C3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP7C3.Location = new System.Drawing.Point(150, 247);
            this.lblP7C3.Name = "lblP7C3";
            this.lblP7C3.Size = new System.Drawing.Size(20, 20);
            this.lblP7C3.TabIndex = 34;
            this.lblP7C3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP6C3
            // 
            this.lblP6C3.BackColor = System.Drawing.Color.Khaki;
            this.lblP6C3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP6C3.Location = new System.Drawing.Point(150, 223);
            this.lblP6C3.Name = "lblP6C3";
            this.lblP6C3.Size = new System.Drawing.Size(20, 20);
            this.lblP6C3.TabIndex = 33;
            this.lblP6C3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP5C3
            // 
            this.lblP5C3.BackColor = System.Drawing.Color.Khaki;
            this.lblP5C3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP5C3.Location = new System.Drawing.Point(150, 199);
            this.lblP5C3.Name = "lblP5C3";
            this.lblP5C3.Size = new System.Drawing.Size(20, 20);
            this.lblP5C3.TabIndex = 32;
            this.lblP5C3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP4C3
            // 
            this.lblP4C3.BackColor = System.Drawing.Color.Khaki;
            this.lblP4C3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP4C3.Location = new System.Drawing.Point(150, 167);
            this.lblP4C3.Name = "lblP4C3";
            this.lblP4C3.Size = new System.Drawing.Size(20, 20);
            this.lblP4C3.TabIndex = 31;
            this.lblP4C3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP3C3
            // 
            this.lblP3C3.BackColor = System.Drawing.Color.Khaki;
            this.lblP3C3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP3C3.Location = new System.Drawing.Point(150, 143);
            this.lblP3C3.Name = "lblP3C3";
            this.lblP3C3.Size = new System.Drawing.Size(20, 20);
            this.lblP3C3.TabIndex = 30;
            this.lblP3C3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP2C3
            // 
            this.lblP2C3.BackColor = System.Drawing.Color.Khaki;
            this.lblP2C3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP2C3.Location = new System.Drawing.Point(150, 119);
            this.lblP2C3.Name = "lblP2C3";
            this.lblP2C3.Size = new System.Drawing.Size(20, 20);
            this.lblP2C3.TabIndex = 29;
            this.lblP2C3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP1C3
            // 
            this.lblP1C3.BackColor = System.Drawing.Color.Khaki;
            this.lblP1C3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP1C3.Location = new System.Drawing.Point(150, 95);
            this.lblP1C3.Name = "lblP1C3";
            this.lblP1C3.Size = new System.Drawing.Size(20, 20);
            this.lblP1C3.TabIndex = 28;
            this.lblP1C3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP14C8
            // 
            this.lblP14C8.BackColor = System.Drawing.Color.Khaki;
            this.lblP14C8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP14C8.Location = new System.Drawing.Point(310, 431);
            this.lblP14C8.Name = "lblP14C8";
            this.lblP14C8.Size = new System.Drawing.Size(20, 20);
            this.lblP14C8.TabIndex = 111;
            this.lblP14C8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP13C8
            // 
            this.lblP13C8.BackColor = System.Drawing.Color.Khaki;
            this.lblP13C8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP13C8.Location = new System.Drawing.Point(310, 407);
            this.lblP13C8.Name = "lblP13C8";
            this.lblP13C8.Size = new System.Drawing.Size(20, 20);
            this.lblP13C8.TabIndex = 110;
            this.lblP13C8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP12C8
            // 
            this.lblP12C8.BackColor = System.Drawing.Color.Khaki;
            this.lblP12C8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP12C8.Location = new System.Drawing.Point(310, 383);
            this.lblP12C8.Name = "lblP12C8";
            this.lblP12C8.Size = new System.Drawing.Size(20, 20);
            this.lblP12C8.TabIndex = 109;
            this.lblP12C8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP11C8
            // 
            this.lblP11C8.BackColor = System.Drawing.Color.Khaki;
            this.lblP11C8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP11C8.Location = new System.Drawing.Point(310, 351);
            this.lblP11C8.Name = "lblP11C8";
            this.lblP11C8.Size = new System.Drawing.Size(20, 20);
            this.lblP11C8.TabIndex = 108;
            this.lblP11C8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP10C8
            // 
            this.lblP10C8.BackColor = System.Drawing.Color.Khaki;
            this.lblP10C8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP10C8.Location = new System.Drawing.Point(310, 327);
            this.lblP10C8.Name = "lblP10C8";
            this.lblP10C8.Size = new System.Drawing.Size(20, 20);
            this.lblP10C8.TabIndex = 107;
            this.lblP10C8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP9C8
            // 
            this.lblP9C8.BackColor = System.Drawing.Color.Khaki;
            this.lblP9C8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP9C8.Location = new System.Drawing.Point(310, 303);
            this.lblP9C8.Name = "lblP9C8";
            this.lblP9C8.Size = new System.Drawing.Size(20, 20);
            this.lblP9C8.TabIndex = 106;
            this.lblP9C8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP8C8
            // 
            this.lblP8C8.BackColor = System.Drawing.Color.Khaki;
            this.lblP8C8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP8C8.Location = new System.Drawing.Point(310, 271);
            this.lblP8C8.Name = "lblP8C8";
            this.lblP8C8.Size = new System.Drawing.Size(20, 20);
            this.lblP8C8.TabIndex = 105;
            this.lblP8C8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP7C8
            // 
            this.lblP7C8.BackColor = System.Drawing.Color.Khaki;
            this.lblP7C8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP7C8.Location = new System.Drawing.Point(310, 247);
            this.lblP7C8.Name = "lblP7C8";
            this.lblP7C8.Size = new System.Drawing.Size(20, 20);
            this.lblP7C8.TabIndex = 104;
            this.lblP7C8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP6C8
            // 
            this.lblP6C8.BackColor = System.Drawing.Color.Khaki;
            this.lblP6C8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP6C8.Location = new System.Drawing.Point(310, 223);
            this.lblP6C8.Name = "lblP6C8";
            this.lblP6C8.Size = new System.Drawing.Size(20, 20);
            this.lblP6C8.TabIndex = 103;
            this.lblP6C8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP5C8
            // 
            this.lblP5C8.BackColor = System.Drawing.Color.Khaki;
            this.lblP5C8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP5C8.Location = new System.Drawing.Point(310, 199);
            this.lblP5C8.Name = "lblP5C8";
            this.lblP5C8.Size = new System.Drawing.Size(20, 20);
            this.lblP5C8.TabIndex = 102;
            this.lblP5C8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP4C8
            // 
            this.lblP4C8.BackColor = System.Drawing.Color.Khaki;
            this.lblP4C8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP4C8.Location = new System.Drawing.Point(310, 167);
            this.lblP4C8.Name = "lblP4C8";
            this.lblP4C8.Size = new System.Drawing.Size(20, 20);
            this.lblP4C8.TabIndex = 101;
            this.lblP4C8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP3C8
            // 
            this.lblP3C8.BackColor = System.Drawing.Color.Khaki;
            this.lblP3C8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP3C8.Location = new System.Drawing.Point(310, 143);
            this.lblP3C8.Name = "lblP3C8";
            this.lblP3C8.Size = new System.Drawing.Size(20, 20);
            this.lblP3C8.TabIndex = 100;
            this.lblP3C8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP2C8
            // 
            this.lblP2C8.BackColor = System.Drawing.Color.Khaki;
            this.lblP2C8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP2C8.Location = new System.Drawing.Point(310, 119);
            this.lblP2C8.Name = "lblP2C8";
            this.lblP2C8.Size = new System.Drawing.Size(20, 20);
            this.lblP2C8.TabIndex = 99;
            this.lblP2C8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP1C8
            // 
            this.lblP1C8.BackColor = System.Drawing.Color.Khaki;
            this.lblP1C8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP1C8.Location = new System.Drawing.Point(310, 95);
            this.lblP1C8.Name = "lblP1C8";
            this.lblP1C8.Size = new System.Drawing.Size(20, 20);
            this.lblP1C8.TabIndex = 98;
            this.lblP1C8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP14C7
            // 
            this.lblP14C7.BackColor = System.Drawing.Color.Khaki;
            this.lblP14C7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP14C7.Location = new System.Drawing.Point(278, 431);
            this.lblP14C7.Name = "lblP14C7";
            this.lblP14C7.Size = new System.Drawing.Size(20, 20);
            this.lblP14C7.TabIndex = 97;
            this.lblP14C7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP13C7
            // 
            this.lblP13C7.BackColor = System.Drawing.Color.Khaki;
            this.lblP13C7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP13C7.Location = new System.Drawing.Point(278, 407);
            this.lblP13C7.Name = "lblP13C7";
            this.lblP13C7.Size = new System.Drawing.Size(20, 20);
            this.lblP13C7.TabIndex = 96;
            this.lblP13C7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP12C7
            // 
            this.lblP12C7.BackColor = System.Drawing.Color.Khaki;
            this.lblP12C7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP12C7.Location = new System.Drawing.Point(278, 383);
            this.lblP12C7.Name = "lblP12C7";
            this.lblP12C7.Size = new System.Drawing.Size(20, 20);
            this.lblP12C7.TabIndex = 95;
            this.lblP12C7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP11C7
            // 
            this.lblP11C7.BackColor = System.Drawing.Color.Khaki;
            this.lblP11C7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP11C7.Location = new System.Drawing.Point(278, 351);
            this.lblP11C7.Name = "lblP11C7";
            this.lblP11C7.Size = new System.Drawing.Size(20, 20);
            this.lblP11C7.TabIndex = 94;
            this.lblP11C7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP10C7
            // 
            this.lblP10C7.BackColor = System.Drawing.Color.Khaki;
            this.lblP10C7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP10C7.Location = new System.Drawing.Point(278, 327);
            this.lblP10C7.Name = "lblP10C7";
            this.lblP10C7.Size = new System.Drawing.Size(20, 20);
            this.lblP10C7.TabIndex = 93;
            this.lblP10C7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP9C7
            // 
            this.lblP9C7.BackColor = System.Drawing.Color.Khaki;
            this.lblP9C7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP9C7.Location = new System.Drawing.Point(278, 303);
            this.lblP9C7.Name = "lblP9C7";
            this.lblP9C7.Size = new System.Drawing.Size(20, 20);
            this.lblP9C7.TabIndex = 92;
            this.lblP9C7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP8C7
            // 
            this.lblP8C7.BackColor = System.Drawing.Color.Khaki;
            this.lblP8C7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP8C7.Location = new System.Drawing.Point(278, 271);
            this.lblP8C7.Name = "lblP8C7";
            this.lblP8C7.Size = new System.Drawing.Size(20, 20);
            this.lblP8C7.TabIndex = 91;
            this.lblP8C7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP7C7
            // 
            this.lblP7C7.BackColor = System.Drawing.Color.Khaki;
            this.lblP7C7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP7C7.Location = new System.Drawing.Point(278, 247);
            this.lblP7C7.Name = "lblP7C7";
            this.lblP7C7.Size = new System.Drawing.Size(20, 20);
            this.lblP7C7.TabIndex = 90;
            this.lblP7C7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP6C7
            // 
            this.lblP6C7.BackColor = System.Drawing.Color.Khaki;
            this.lblP6C7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP6C7.Location = new System.Drawing.Point(278, 223);
            this.lblP6C7.Name = "lblP6C7";
            this.lblP6C7.Size = new System.Drawing.Size(20, 20);
            this.lblP6C7.TabIndex = 89;
            this.lblP6C7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP5C7
            // 
            this.lblP5C7.BackColor = System.Drawing.Color.Khaki;
            this.lblP5C7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP5C7.Location = new System.Drawing.Point(278, 199);
            this.lblP5C7.Name = "lblP5C7";
            this.lblP5C7.Size = new System.Drawing.Size(20, 20);
            this.lblP5C7.TabIndex = 88;
            this.lblP5C7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP4C7
            // 
            this.lblP4C7.BackColor = System.Drawing.Color.Khaki;
            this.lblP4C7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP4C7.Location = new System.Drawing.Point(278, 167);
            this.lblP4C7.Name = "lblP4C7";
            this.lblP4C7.Size = new System.Drawing.Size(20, 20);
            this.lblP4C7.TabIndex = 87;
            this.lblP4C7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP3C7
            // 
            this.lblP3C7.BackColor = System.Drawing.Color.Khaki;
            this.lblP3C7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP3C7.Location = new System.Drawing.Point(278, 143);
            this.lblP3C7.Name = "lblP3C7";
            this.lblP3C7.Size = new System.Drawing.Size(20, 20);
            this.lblP3C7.TabIndex = 86;
            this.lblP3C7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP2C7
            // 
            this.lblP2C7.BackColor = System.Drawing.Color.Khaki;
            this.lblP2C7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP2C7.Location = new System.Drawing.Point(278, 119);
            this.lblP2C7.Name = "lblP2C7";
            this.lblP2C7.Size = new System.Drawing.Size(20, 20);
            this.lblP2C7.TabIndex = 85;
            this.lblP2C7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP1C7
            // 
            this.lblP1C7.BackColor = System.Drawing.Color.Khaki;
            this.lblP1C7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP1C7.Location = new System.Drawing.Point(278, 95);
            this.lblP1C7.Name = "lblP1C7";
            this.lblP1C7.Size = new System.Drawing.Size(20, 20);
            this.lblP1C7.TabIndex = 84;
            this.lblP1C7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP14C6
            // 
            this.lblP14C6.BackColor = System.Drawing.Color.Khaki;
            this.lblP14C6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP14C6.Location = new System.Drawing.Point(246, 431);
            this.lblP14C6.Name = "lblP14C6";
            this.lblP14C6.Size = new System.Drawing.Size(20, 20);
            this.lblP14C6.TabIndex = 83;
            this.lblP14C6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP13C6
            // 
            this.lblP13C6.BackColor = System.Drawing.Color.Khaki;
            this.lblP13C6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP13C6.Location = new System.Drawing.Point(246, 407);
            this.lblP13C6.Name = "lblP13C6";
            this.lblP13C6.Size = new System.Drawing.Size(20, 20);
            this.lblP13C6.TabIndex = 82;
            this.lblP13C6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP12C6
            // 
            this.lblP12C6.BackColor = System.Drawing.Color.Khaki;
            this.lblP12C6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP12C6.Location = new System.Drawing.Point(246, 383);
            this.lblP12C6.Name = "lblP12C6";
            this.lblP12C6.Size = new System.Drawing.Size(20, 20);
            this.lblP12C6.TabIndex = 81;
            this.lblP12C6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP11C6
            // 
            this.lblP11C6.BackColor = System.Drawing.Color.Khaki;
            this.lblP11C6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP11C6.Location = new System.Drawing.Point(246, 351);
            this.lblP11C6.Name = "lblP11C6";
            this.lblP11C6.Size = new System.Drawing.Size(20, 20);
            this.lblP11C6.TabIndex = 80;
            this.lblP11C6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP10C6
            // 
            this.lblP10C6.BackColor = System.Drawing.Color.Khaki;
            this.lblP10C6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP10C6.Location = new System.Drawing.Point(246, 327);
            this.lblP10C6.Name = "lblP10C6";
            this.lblP10C6.Size = new System.Drawing.Size(20, 20);
            this.lblP10C6.TabIndex = 79;
            this.lblP10C6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP9C6
            // 
            this.lblP9C6.BackColor = System.Drawing.Color.Khaki;
            this.lblP9C6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP9C6.Location = new System.Drawing.Point(246, 303);
            this.lblP9C6.Name = "lblP9C6";
            this.lblP9C6.Size = new System.Drawing.Size(20, 20);
            this.lblP9C6.TabIndex = 78;
            this.lblP9C6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP8C6
            // 
            this.lblP8C6.BackColor = System.Drawing.Color.Khaki;
            this.lblP8C6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP8C6.Location = new System.Drawing.Point(246, 271);
            this.lblP8C6.Name = "lblP8C6";
            this.lblP8C6.Size = new System.Drawing.Size(20, 20);
            this.lblP8C6.TabIndex = 77;
            this.lblP8C6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP7C6
            // 
            this.lblP7C6.BackColor = System.Drawing.Color.Khaki;
            this.lblP7C6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP7C6.Location = new System.Drawing.Point(246, 247);
            this.lblP7C6.Name = "lblP7C6";
            this.lblP7C6.Size = new System.Drawing.Size(20, 20);
            this.lblP7C6.TabIndex = 76;
            this.lblP7C6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP6C6
            // 
            this.lblP6C6.BackColor = System.Drawing.Color.Khaki;
            this.lblP6C6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP6C6.Location = new System.Drawing.Point(246, 223);
            this.lblP6C6.Name = "lblP6C6";
            this.lblP6C6.Size = new System.Drawing.Size(20, 20);
            this.lblP6C6.TabIndex = 75;
            this.lblP6C6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP5C6
            // 
            this.lblP5C6.BackColor = System.Drawing.Color.Khaki;
            this.lblP5C6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP5C6.Location = new System.Drawing.Point(246, 199);
            this.lblP5C6.Name = "lblP5C6";
            this.lblP5C6.Size = new System.Drawing.Size(20, 20);
            this.lblP5C6.TabIndex = 74;
            this.lblP5C6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP4C6
            // 
            this.lblP4C6.BackColor = System.Drawing.Color.Khaki;
            this.lblP4C6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP4C6.Location = new System.Drawing.Point(246, 167);
            this.lblP4C6.Name = "lblP4C6";
            this.lblP4C6.Size = new System.Drawing.Size(20, 20);
            this.lblP4C6.TabIndex = 73;
            this.lblP4C6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP3C6
            // 
            this.lblP3C6.BackColor = System.Drawing.Color.Khaki;
            this.lblP3C6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP3C6.Location = new System.Drawing.Point(246, 143);
            this.lblP3C6.Name = "lblP3C6";
            this.lblP3C6.Size = new System.Drawing.Size(20, 20);
            this.lblP3C6.TabIndex = 72;
            this.lblP3C6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP2C6
            // 
            this.lblP2C6.BackColor = System.Drawing.Color.Khaki;
            this.lblP2C6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP2C6.Location = new System.Drawing.Point(246, 119);
            this.lblP2C6.Name = "lblP2C6";
            this.lblP2C6.Size = new System.Drawing.Size(20, 20);
            this.lblP2C6.TabIndex = 71;
            this.lblP2C6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP1C6
            // 
            this.lblP1C6.BackColor = System.Drawing.Color.Khaki;
            this.lblP1C6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP1C6.Location = new System.Drawing.Point(246, 95);
            this.lblP1C6.Name = "lblP1C6";
            this.lblP1C6.Size = new System.Drawing.Size(20, 20);
            this.lblP1C6.TabIndex = 70;
            this.lblP1C6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP14C5
            // 
            this.lblP14C5.BackColor = System.Drawing.Color.Khaki;
            this.lblP14C5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP14C5.Location = new System.Drawing.Point(214, 431);
            this.lblP14C5.Name = "lblP14C5";
            this.lblP14C5.Size = new System.Drawing.Size(20, 20);
            this.lblP14C5.TabIndex = 69;
            this.lblP14C5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP13C5
            // 
            this.lblP13C5.BackColor = System.Drawing.Color.Khaki;
            this.lblP13C5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP13C5.Location = new System.Drawing.Point(214, 407);
            this.lblP13C5.Name = "lblP13C5";
            this.lblP13C5.Size = new System.Drawing.Size(20, 20);
            this.lblP13C5.TabIndex = 68;
            this.lblP13C5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP12C5
            // 
            this.lblP12C5.BackColor = System.Drawing.Color.Khaki;
            this.lblP12C5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP12C5.Location = new System.Drawing.Point(214, 383);
            this.lblP12C5.Name = "lblP12C5";
            this.lblP12C5.Size = new System.Drawing.Size(20, 20);
            this.lblP12C5.TabIndex = 67;
            this.lblP12C5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP11C5
            // 
            this.lblP11C5.BackColor = System.Drawing.Color.Khaki;
            this.lblP11C5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP11C5.Location = new System.Drawing.Point(214, 351);
            this.lblP11C5.Name = "lblP11C5";
            this.lblP11C5.Size = new System.Drawing.Size(20, 20);
            this.lblP11C5.TabIndex = 66;
            this.lblP11C5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP10C5
            // 
            this.lblP10C5.BackColor = System.Drawing.Color.Khaki;
            this.lblP10C5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP10C5.Location = new System.Drawing.Point(214, 327);
            this.lblP10C5.Name = "lblP10C5";
            this.lblP10C5.Size = new System.Drawing.Size(20, 20);
            this.lblP10C5.TabIndex = 65;
            this.lblP10C5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP9C5
            // 
            this.lblP9C5.BackColor = System.Drawing.Color.Khaki;
            this.lblP9C5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP9C5.Location = new System.Drawing.Point(214, 303);
            this.lblP9C5.Name = "lblP9C5";
            this.lblP9C5.Size = new System.Drawing.Size(20, 20);
            this.lblP9C5.TabIndex = 64;
            this.lblP9C5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP8C5
            // 
            this.lblP8C5.BackColor = System.Drawing.Color.Khaki;
            this.lblP8C5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP8C5.Location = new System.Drawing.Point(214, 271);
            this.lblP8C5.Name = "lblP8C5";
            this.lblP8C5.Size = new System.Drawing.Size(20, 20);
            this.lblP8C5.TabIndex = 63;
            this.lblP8C5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP7C5
            // 
            this.lblP7C5.BackColor = System.Drawing.Color.Khaki;
            this.lblP7C5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP7C5.Location = new System.Drawing.Point(214, 247);
            this.lblP7C5.Name = "lblP7C5";
            this.lblP7C5.Size = new System.Drawing.Size(20, 20);
            this.lblP7C5.TabIndex = 62;
            this.lblP7C5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP6C5
            // 
            this.lblP6C5.BackColor = System.Drawing.Color.Khaki;
            this.lblP6C5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP6C5.Location = new System.Drawing.Point(214, 223);
            this.lblP6C5.Name = "lblP6C5";
            this.lblP6C5.Size = new System.Drawing.Size(20, 20);
            this.lblP6C5.TabIndex = 61;
            this.lblP6C5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP5C5
            // 
            this.lblP5C5.BackColor = System.Drawing.Color.Khaki;
            this.lblP5C5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP5C5.Location = new System.Drawing.Point(214, 199);
            this.lblP5C5.Name = "lblP5C5";
            this.lblP5C5.Size = new System.Drawing.Size(20, 20);
            this.lblP5C5.TabIndex = 60;
            this.lblP5C5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP4C5
            // 
            this.lblP4C5.BackColor = System.Drawing.Color.Khaki;
            this.lblP4C5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP4C5.Location = new System.Drawing.Point(214, 167);
            this.lblP4C5.Name = "lblP4C5";
            this.lblP4C5.Size = new System.Drawing.Size(20, 20);
            this.lblP4C5.TabIndex = 59;
            this.lblP4C5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP3C5
            // 
            this.lblP3C5.BackColor = System.Drawing.Color.Khaki;
            this.lblP3C5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP3C5.Location = new System.Drawing.Point(214, 143);
            this.lblP3C5.Name = "lblP3C5";
            this.lblP3C5.Size = new System.Drawing.Size(20, 20);
            this.lblP3C5.TabIndex = 58;
            this.lblP3C5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP2C5
            // 
            this.lblP2C5.BackColor = System.Drawing.Color.Khaki;
            this.lblP2C5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP2C5.Location = new System.Drawing.Point(214, 119);
            this.lblP2C5.Name = "lblP2C5";
            this.lblP2C5.Size = new System.Drawing.Size(20, 20);
            this.lblP2C5.TabIndex = 57;
            this.lblP2C5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP1C5
            // 
            this.lblP1C5.BackColor = System.Drawing.Color.Khaki;
            this.lblP1C5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP1C5.Location = new System.Drawing.Point(214, 95);
            this.lblP1C5.Name = "lblP1C5";
            this.lblP1C5.Size = new System.Drawing.Size(20, 20);
            this.lblP1C5.TabIndex = 56;
            this.lblP1C5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP8
            // 
            this.lblP8.BackColor = System.Drawing.Color.White;
            this.lblP8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP8.Location = new System.Drawing.Point(310, 71);
            this.lblP8.Name = "lblP8";
            this.lblP8.Size = new System.Drawing.Size(20, 20);
            this.lblP8.TabIndex = 119;
            this.lblP8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP7
            // 
            this.lblP7.BackColor = System.Drawing.Color.White;
            this.lblP7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP7.Location = new System.Drawing.Point(278, 71);
            this.lblP7.Name = "lblP7";
            this.lblP7.Size = new System.Drawing.Size(20, 20);
            this.lblP7.TabIndex = 118;
            this.lblP7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP6
            // 
            this.lblP6.BackColor = System.Drawing.Color.White;
            this.lblP6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP6.Location = new System.Drawing.Point(246, 71);
            this.lblP6.Name = "lblP6";
            this.lblP6.Size = new System.Drawing.Size(20, 20);
            this.lblP6.TabIndex = 117;
            this.lblP6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP5
            // 
            this.lblP5.BackColor = System.Drawing.Color.White;
            this.lblP5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP5.Location = new System.Drawing.Point(214, 71);
            this.lblP5.Name = "lblP5";
            this.lblP5.Size = new System.Drawing.Size(20, 20);
            this.lblP5.TabIndex = 116;
            this.lblP5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP4
            // 
            this.lblP4.BackColor = System.Drawing.Color.White;
            this.lblP4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP4.Location = new System.Drawing.Point(182, 71);
            this.lblP4.Name = "lblP4";
            this.lblP4.Size = new System.Drawing.Size(20, 20);
            this.lblP4.TabIndex = 115;
            this.lblP4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP3
            // 
            this.lblP3.BackColor = System.Drawing.Color.White;
            this.lblP3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP3.Location = new System.Drawing.Point(150, 71);
            this.lblP3.Name = "lblP3";
            this.lblP3.Size = new System.Drawing.Size(20, 20);
            this.lblP3.TabIndex = 114;
            this.lblP3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP2
            // 
            this.lblP2.BackColor = System.Drawing.Color.White;
            this.lblP2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP2.Location = new System.Drawing.Point(118, 71);
            this.lblP2.Name = "lblP2";
            this.lblP2.Size = new System.Drawing.Size(20, 20);
            this.lblP2.TabIndex = 113;
            this.lblP2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP1
            // 
            this.lblP1.BackColor = System.Drawing.Color.White;
            this.lblP1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP1.Location = new System.Drawing.Point(86, 71);
            this.lblP1.Name = "lblP1";
            this.lblP1.Size = new System.Drawing.Size(20, 20);
            this.lblP1.TabIndex = 112;
            this.lblP1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txt1
            // 
            this.txt1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt1.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txt1.Location = new System.Drawing.Point(46, 95);
            this.txt1.MaxLength = 1;
            this.txt1.Name = "txt1";
            this.txt1.Size = new System.Drawing.Size(20, 20);
            this.txt1.TabIndex = 120;
            this.txt1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txt1.VisibleChanged += new System.EventHandler(this.txt1_VisibleChanged);
            // 
            // txt2
            // 
            this.txt2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt2.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txt2.Location = new System.Drawing.Point(46, 119);
            this.txt2.MaxLength = 1;
            this.txt2.Name = "txt2";
            this.txt2.Size = new System.Drawing.Size(20, 20);
            this.txt2.TabIndex = 121;
            this.txt2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txt2.VisibleChanged += new System.EventHandler(this.txt2_VisibleChanged);
            // 
            // txt4
            // 
            this.txt4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt4.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txt4.Location = new System.Drawing.Point(46, 167);
            this.txt4.MaxLength = 1;
            this.txt4.Name = "txt4";
            this.txt4.Size = new System.Drawing.Size(20, 20);
            this.txt4.TabIndex = 123;
            this.txt4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txt4.VisibleChanged += new System.EventHandler(this.txt4_VisibleChanged);
            // 
            // txt3
            // 
            this.txt3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt3.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txt3.Location = new System.Drawing.Point(46, 143);
            this.txt3.MaxLength = 1;
            this.txt3.Name = "txt3";
            this.txt3.Size = new System.Drawing.Size(20, 20);
            this.txt3.TabIndex = 122;
            this.txt3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txt3.VisibleChanged += new System.EventHandler(this.txt3_VisibleChanged);
            // 
            // txt8
            // 
            this.txt8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt8.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txt8.Location = new System.Drawing.Point(46, 271);
            this.txt8.MaxLength = 1;
            this.txt8.Name = "txt8";
            this.txt8.Size = new System.Drawing.Size(20, 20);
            this.txt8.TabIndex = 127;
            this.txt8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txt8.VisibleChanged += new System.EventHandler(this.txt8_VisibleChanged);
            // 
            // txt7
            // 
            this.txt7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt7.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txt7.Location = new System.Drawing.Point(46, 247);
            this.txt7.MaxLength = 1;
            this.txt7.Name = "txt7";
            this.txt7.Size = new System.Drawing.Size(20, 20);
            this.txt7.TabIndex = 126;
            this.txt7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txt7.VisibleChanged += new System.EventHandler(this.txt7_VisibleChanged);
            // 
            // txt6
            // 
            this.txt6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt6.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txt6.Location = new System.Drawing.Point(46, 223);
            this.txt6.MaxLength = 1;
            this.txt6.Name = "txt6";
            this.txt6.Size = new System.Drawing.Size(20, 20);
            this.txt6.TabIndex = 125;
            this.txt6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txt6.VisibleChanged += new System.EventHandler(this.txt6_VisibleChanged);
            // 
            // txt5
            // 
            this.txt5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt5.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txt5.Location = new System.Drawing.Point(46, 199);
            this.txt5.MaxLength = 1;
            this.txt5.Name = "txt5";
            this.txt5.Size = new System.Drawing.Size(20, 20);
            this.txt5.TabIndex = 124;
            this.txt5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txt5.VisibleChanged += new System.EventHandler(this.txt5_VisibleChanged);
            // 
            // txt11
            // 
            this.txt11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt11.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txt11.Location = new System.Drawing.Point(46, 351);
            this.txt11.MaxLength = 1;
            this.txt11.Name = "txt11";
            this.txt11.Size = new System.Drawing.Size(20, 20);
            this.txt11.TabIndex = 130;
            this.txt11.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txt11.VisibleChanged += new System.EventHandler(this.txt11_VisibleChanged);
            // 
            // txt10
            // 
            this.txt10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt10.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txt10.Location = new System.Drawing.Point(46, 327);
            this.txt10.MaxLength = 1;
            this.txt10.Name = "txt10";
            this.txt10.Size = new System.Drawing.Size(20, 20);
            this.txt10.TabIndex = 129;
            this.txt10.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txt10.VisibleChanged += new System.EventHandler(this.txt10_VisibleChanged);
            // 
            // txt9
            // 
            this.txt9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt9.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txt9.Location = new System.Drawing.Point(46, 303);
            this.txt9.MaxLength = 1;
            this.txt9.Name = "txt9";
            this.txt9.Size = new System.Drawing.Size(20, 20);
            this.txt9.TabIndex = 128;
            this.txt9.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txt9.VisibleChanged += new System.EventHandler(this.txt9_VisibleChanged);
            // 
            // txt14
            // 
            this.txt14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt14.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txt14.Location = new System.Drawing.Point(46, 431);
            this.txt14.MaxLength = 1;
            this.txt14.Name = "txt14";
            this.txt14.Size = new System.Drawing.Size(20, 20);
            this.txt14.TabIndex = 133;
            this.txt14.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txt14.VisibleChanged += new System.EventHandler(this.txt14_VisibleChanged);
            // 
            // txt13
            // 
            this.txt13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt13.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txt13.Location = new System.Drawing.Point(46, 407);
            this.txt13.MaxLength = 1;
            this.txt13.Name = "txt13";
            this.txt13.Size = new System.Drawing.Size(20, 20);
            this.txt13.TabIndex = 132;
            this.txt13.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txt13.VisibleChanged += new System.EventHandler(this.txt13_VisibleChanged);
            // 
            // txt12
            // 
            this.txt12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt12.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txt12.Location = new System.Drawing.Point(46, 383);
            this.txt12.MaxLength = 1;
            this.txt12.Name = "txt12";
            this.txt12.Size = new System.Drawing.Size(20, 20);
            this.txt12.TabIndex = 131;
            this.txt12.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txt12.VisibleChanged += new System.EventHandler(this.txt12_VisibleChanged);
            // 
            // btnAdelante
            // 
            this.btnAdelante.BackColor = System.Drawing.Color.LightSalmon;
            this.btnAdelante.Enabled = false;
            this.btnAdelante.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAdelante.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdelante.Location = new System.Drawing.Point(242, 38);
            this.btnAdelante.Name = "btnAdelante";
            this.btnAdelante.Size = new System.Drawing.Size(24, 24);
            this.btnAdelante.TabIndex = 134;
            this.btnAdelante.Text = ">";
            this.btnAdelante.UseVisualStyleBackColor = false;
            this.btnAdelante.Click += new System.EventHandler(this.btnAdelante_Click);
            // 
            // btnAtras
            // 
            this.btnAtras.BackColor = System.Drawing.Color.LightSalmon;
            this.btnAtras.Enabled = false;
            this.btnAtras.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAtras.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAtras.Location = new System.Drawing.Point(150, 38);
            this.btnAtras.Name = "btnAtras";
            this.btnAtras.Size = new System.Drawing.Size(24, 24);
            this.btnAtras.TabIndex = 135;
            this.btnAtras.Text = "<";
            this.btnAtras.UseVisualStyleBackColor = false;
            this.btnAtras.Click += new System.EventHandler(this.btnAtras_Click);
            // 
            // btnAbreArchivo
            // 
            this.btnAbreArchivo.BackColor = System.Drawing.Color.LightSalmon;
            this.btnAbreArchivo.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAbreArchivo.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAbreArchivo.Image = ((System.Drawing.Image)(resources.GetObject("btnAbreArchivo.Image")));
            this.btnAbreArchivo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAbreArchivo.Location = new System.Drawing.Point(9, 12);
            this.btnAbreArchivo.Name = "btnAbreArchivo";
            this.btnAbreArchivo.Size = new System.Drawing.Size(100, 23);
            this.btnAbreArchivo.TabIndex = 136;
            this.btnAbreArchivo.Text = "Archivo...";
            this.btnAbreArchivo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAbreArchivo.UseVisualStyleBackColor = false;
            this.btnAbreArchivo.Click += new System.EventHandler(this.btnAbreArchivo_Click);
            // 
            // lblNombreArchivo
            // 
            this.lblNombreArchivo.BackColor = System.Drawing.Color.White;
            this.lblNombreArchivo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNombreArchivo.Location = new System.Drawing.Point(110, 12);
            this.lblNombreArchivo.Name = "lblNombreArchivo";
            this.lblNombreArchivo.Size = new System.Drawing.Size(129, 23);
            this.lblNombreArchivo.TabIndex = 137;
            this.lblNombreArchivo.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnCalcular
            // 
            this.btnCalcular.BackColor = System.Drawing.Color.Silver;
            this.btnCalcular.Enabled = false;
            this.btnCalcular.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCalcular.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCalcular.Image = ((System.Drawing.Image)(resources.GetObject("btnCalcular.Image")));
            this.btnCalcular.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCalcular.Location = new System.Drawing.Point(240, 12);
            this.btnCalcular.Name = "btnCalcular";
            this.btnCalcular.Size = new System.Drawing.Size(100, 23);
            this.btnCalcular.TabIndex = 138;
            this.btnCalcular.Text = "Calcular";
            this.btnCalcular.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCalcular.UseVisualStyleBackColor = false;
            this.btnCalcular.Click += new System.EventHandler(this.btnCalcular_Click);
            this.btnCalcular.EnabledChanged += new System.EventHandler(this.btnCalcular_EnabledChanged);
            // 
            // lblColumnasConPremio
            // 
            this.lblColumnasConPremio.BackColor = System.Drawing.Color.White;
            this.lblColumnasConPremio.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblColumnasConPremio.Location = new System.Drawing.Point(344, 160);
            this.lblColumnasConPremio.Name = "lblColumnasConPremio";
            this.lblColumnasConPremio.Size = new System.Drawing.Size(176, 23);
            this.lblColumnasConPremio.TabIndex = 139;
            this.lblColumnasConPremio.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPremios14
            // 
            this.lblPremios14.BackColor = System.Drawing.Color.Bisque;
            this.lblPremios14.Location = new System.Drawing.Point(344, 253);
            this.lblPremios14.Name = "lblPremios14";
            this.lblPremios14.Size = new System.Drawing.Size(168, 23);
            this.lblPremios14.TabIndex = 140;
            this.lblPremios14.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPremios12
            // 
            this.lblPremios12.BackColor = System.Drawing.Color.Bisque;
            this.lblPremios12.Location = new System.Drawing.Point(344, 317);
            this.lblPremios12.Name = "lblPremios12";
            this.lblPremios12.Size = new System.Drawing.Size(168, 23);
            this.lblPremios12.TabIndex = 142;
            this.lblPremios12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPremios13
            // 
            this.lblPremios13.BackColor = System.Drawing.Color.Bisque;
            this.lblPremios13.Location = new System.Drawing.Point(344, 285);
            this.lblPremios13.Name = "lblPremios13";
            this.lblPremios13.Size = new System.Drawing.Size(168, 23);
            this.lblPremios13.TabIndex = 141;
            this.lblPremios13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPremios10
            // 
            this.lblPremios10.BackColor = System.Drawing.Color.Bisque;
            this.lblPremios10.Location = new System.Drawing.Point(344, 381);
            this.lblPremios10.Name = "lblPremios10";
            this.lblPremios10.Size = new System.Drawing.Size(168, 23);
            this.lblPremios10.TabIndex = 144;
            this.lblPremios10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPremios11
            // 
            this.lblPremios11.BackColor = System.Drawing.Color.Bisque;
            this.lblPremios11.Location = new System.Drawing.Point(344, 349);
            this.lblPremios11.Name = "lblPremios11";
            this.lblPremios11.Size = new System.Drawing.Size(168, 23);
            this.lblPremios11.TabIndex = 143;
            this.lblPremios11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnCopiar
            // 
            this.btnCopiar.BackColor = System.Drawing.Color.LightSalmon;
            this.btnCopiar.Enabled = false;
            this.btnCopiar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCopiar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCopiar.Image = ((System.Drawing.Image)(resources.GetObject("btnCopiar.Image")));
            this.btnCopiar.Location = new System.Drawing.Point(416, 413);
            this.btnCopiar.Name = "btnCopiar";
            this.btnCopiar.Size = new System.Drawing.Size(24, 24);
            this.btnCopiar.TabIndex = 146;
            this.btnCopiar.Tag = "";
            this.btnCopiar.UseVisualStyleBackColor = false;
            this.btnCopiar.Click += new System.EventHandler(this.btnCopiar_Click);
            // 
            // ckbResumen
            // 
            this.ckbResumen.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ckbResumen.Location = new System.Drawing.Point(346, 8);
            this.ckbResumen.Name = "ckbResumen";
            this.ckbResumen.Size = new System.Drawing.Size(155, 20);
            this.ckbResumen.TabIndex = 147;
            this.ckbResumen.Text = "Resumen";
            // 
            // btnVer
            // 
            this.btnVer.BackColor = System.Drawing.Color.LightSalmon;
            this.btnVer.Enabled = false;
            this.btnVer.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnVer.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVer.Image = ((System.Drawing.Image)(resources.GetObject("btnVer.Image")));
            this.btnVer.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnVer.Location = new System.Drawing.Point(344, 80);
            this.btnVer.Name = "btnVer";
            this.btnVer.Size = new System.Drawing.Size(176, 24);
            this.btnVer.TabIndex = 148;
            this.btnVer.Text = "Ver";
            this.btnVer.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnVer.UseVisualStyleBackColor = false;
            this.btnVer.Click += new System.EventHandler(this.btnVer_Click);
            // 
            // btnGuardar
            // 
            this.btnGuardar.BackColor = System.Drawing.Color.LightSalmon;
            this.btnGuardar.Enabled = false;
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGuardar.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardar.Image = ((System.Drawing.Image)(resources.GetObject("btnGuardar.Image")));
            this.btnGuardar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGuardar.Location = new System.Drawing.Point(344, 105);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new System.Drawing.Size(176, 24);
            this.btnGuardar.TabIndex = 149;
            this.btnGuardar.Text = "Guardar Resumen";
            this.btnGuardar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            // 
            // btnMejoresOpciones
            // 
            this.btnMejoresOpciones.BackColor = System.Drawing.Color.LightSalmon;
            this.btnMejoresOpciones.Enabled = false;
            this.btnMejoresOpciones.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnMejoresOpciones.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnMejoresOpciones.Image = ((System.Drawing.Image)(resources.GetObject("btnMejoresOpciones.Image")));
            this.btnMejoresOpciones.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnMejoresOpciones.Location = new System.Drawing.Point(344, 130);
            this.btnMejoresOpciones.Name = "btnMejoresOpciones";
            this.btnMejoresOpciones.Size = new System.Drawing.Size(176, 24);
            this.btnMejoresOpciones.TabIndex = 164;
            this.btnMejoresOpciones.Text = "Mis mejores opciones";
            this.btnMejoresOpciones.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnMejoresOpciones.UseVisualStyleBackColor = false;
            this.btnMejoresOpciones.Click += new System.EventHandler(this.btnMejoresOpciones_Click);
            // 
            // txt16
            // 
            this.txt16.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt16.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txt16.Location = new System.Drawing.Point(46, 487);
            this.txt16.MaxLength = 1;
            this.txt16.Name = "txt16";
            this.txt16.Size = new System.Drawing.Size(20, 20);
            this.txt16.TabIndex = 182;
            this.txt16.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txt16.VisibleChanged += new System.EventHandler(this.txt16_VisibleChanged);
            // 
            // txt15
            // 
            this.txt15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt15.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txt15.Location = new System.Drawing.Point(46, 463);
            this.txt15.MaxLength = 1;
            this.txt15.Name = "txt15";
            this.txt15.Size = new System.Drawing.Size(20, 20);
            this.txt15.TabIndex = 181;
            this.txt15.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txt15.VisibleChanged += new System.EventHandler(this.txt15_VisibleChanged);
            // 
            // lblP16C8
            // 
            this.lblP16C8.BackColor = System.Drawing.Color.Khaki;
            this.lblP16C8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP16C8.Location = new System.Drawing.Point(310, 487);
            this.lblP16C8.Name = "lblP16C8";
            this.lblP16C8.Size = new System.Drawing.Size(20, 20);
            this.lblP16C8.TabIndex = 180;
            this.lblP16C8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP15C8
            // 
            this.lblP15C8.BackColor = System.Drawing.Color.Khaki;
            this.lblP15C8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP15C8.Location = new System.Drawing.Point(310, 463);
            this.lblP15C8.Name = "lblP15C8";
            this.lblP15C8.Size = new System.Drawing.Size(20, 20);
            this.lblP15C8.TabIndex = 179;
            this.lblP15C8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP16C7
            // 
            this.lblP16C7.BackColor = System.Drawing.Color.Khaki;
            this.lblP16C7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP16C7.Location = new System.Drawing.Point(278, 487);
            this.lblP16C7.Name = "lblP16C7";
            this.lblP16C7.Size = new System.Drawing.Size(20, 20);
            this.lblP16C7.TabIndex = 178;
            this.lblP16C7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP15C7
            // 
            this.lblP15C7.BackColor = System.Drawing.Color.Khaki;
            this.lblP15C7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP15C7.Location = new System.Drawing.Point(278, 463);
            this.lblP15C7.Name = "lblP15C7";
            this.lblP15C7.Size = new System.Drawing.Size(20, 20);
            this.lblP15C7.TabIndex = 177;
            this.lblP15C7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP16C6
            // 
            this.lblP16C6.BackColor = System.Drawing.Color.Khaki;
            this.lblP16C6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP16C6.Location = new System.Drawing.Point(246, 487);
            this.lblP16C6.Name = "lblP16C6";
            this.lblP16C6.Size = new System.Drawing.Size(20, 20);
            this.lblP16C6.TabIndex = 176;
            this.lblP16C6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP15C6
            // 
            this.lblP15C6.BackColor = System.Drawing.Color.Khaki;
            this.lblP15C6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP15C6.Location = new System.Drawing.Point(246, 463);
            this.lblP15C6.Name = "lblP15C6";
            this.lblP15C6.Size = new System.Drawing.Size(20, 20);
            this.lblP15C6.TabIndex = 175;
            this.lblP15C6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP16C5
            // 
            this.lblP16C5.BackColor = System.Drawing.Color.Khaki;
            this.lblP16C5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP16C5.Location = new System.Drawing.Point(214, 487);
            this.lblP16C5.Name = "lblP16C5";
            this.lblP16C5.Size = new System.Drawing.Size(20, 20);
            this.lblP16C5.TabIndex = 174;
            this.lblP16C5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP15C5
            // 
            this.lblP15C5.BackColor = System.Drawing.Color.Khaki;
            this.lblP15C5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP15C5.Location = new System.Drawing.Point(214, 463);
            this.lblP15C5.Name = "lblP15C5";
            this.lblP15C5.Size = new System.Drawing.Size(20, 20);
            this.lblP15C5.TabIndex = 173;
            this.lblP15C5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP16C4
            // 
            this.lblP16C4.BackColor = System.Drawing.Color.Khaki;
            this.lblP16C4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP16C4.Location = new System.Drawing.Point(182, 487);
            this.lblP16C4.Name = "lblP16C4";
            this.lblP16C4.Size = new System.Drawing.Size(20, 20);
            this.lblP16C4.TabIndex = 172;
            this.lblP16C4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP15C4
            // 
            this.lblP15C4.BackColor = System.Drawing.Color.Khaki;
            this.lblP15C4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP15C4.Location = new System.Drawing.Point(182, 463);
            this.lblP15C4.Name = "lblP15C4";
            this.lblP15C4.Size = new System.Drawing.Size(20, 20);
            this.lblP15C4.TabIndex = 171;
            this.lblP15C4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP16C3
            // 
            this.lblP16C3.BackColor = System.Drawing.Color.Khaki;
            this.lblP16C3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP16C3.Location = new System.Drawing.Point(150, 487);
            this.lblP16C3.Name = "lblP16C3";
            this.lblP16C3.Size = new System.Drawing.Size(20, 20);
            this.lblP16C3.TabIndex = 170;
            this.lblP16C3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP15C3
            // 
            this.lblP15C3.BackColor = System.Drawing.Color.Khaki;
            this.lblP15C3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP15C3.Location = new System.Drawing.Point(150, 463);
            this.lblP15C3.Name = "lblP15C3";
            this.lblP15C3.Size = new System.Drawing.Size(20, 20);
            this.lblP15C3.TabIndex = 169;
            this.lblP15C3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP16C2
            // 
            this.lblP16C2.BackColor = System.Drawing.Color.Khaki;
            this.lblP16C2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP16C2.Location = new System.Drawing.Point(118, 487);
            this.lblP16C2.Name = "lblP16C2";
            this.lblP16C2.Size = new System.Drawing.Size(20, 20);
            this.lblP16C2.TabIndex = 168;
            this.lblP16C2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP15C2
            // 
            this.lblP15C2.BackColor = System.Drawing.Color.Khaki;
            this.lblP15C2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP15C2.Location = new System.Drawing.Point(118, 463);
            this.lblP15C2.Name = "lblP15C2";
            this.lblP15C2.Size = new System.Drawing.Size(20, 20);
            this.lblP15C2.TabIndex = 167;
            this.lblP15C2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP16C1
            // 
            this.lblP16C1.BackColor = System.Drawing.Color.Khaki;
            this.lblP16C1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP16C1.Location = new System.Drawing.Point(86, 487);
            this.lblP16C1.Name = "lblP16C1";
            this.lblP16C1.Size = new System.Drawing.Size(20, 20);
            this.lblP16C1.TabIndex = 166;
            this.lblP16C1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblP15C1
            // 
            this.lblP15C1.BackColor = System.Drawing.Color.Khaki;
            this.lblP15C1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblP15C1.Location = new System.Drawing.Point(86, 463);
            this.lblP15C1.Name = "lblP15C1";
            this.lblP15C1.Size = new System.Drawing.Size(20, 20);
            this.lblP15C1.TabIndex = 165;
            this.lblP15C1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPremios15
            // 
            this.lblPremios15.BackColor = System.Drawing.Color.Bisque;
            this.lblPremios15.Location = new System.Drawing.Point(344, 222);
            this.lblPremios15.Name = "lblPremios15";
            this.lblPremios15.Size = new System.Drawing.Size(168, 23);
            this.lblPremios15.TabIndex = 185;
            this.lblPremios15.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblPremios16
            // 
            this.lblPremios16.BackColor = System.Drawing.Color.Bisque;
            this.lblPremios16.Location = new System.Drawing.Point(344, 190);
            this.lblPremios16.Name = "lblPremios16";
            this.lblPremios16.Size = new System.Drawing.Size(168, 23);
            this.lblPremios16.TabIndex = 186;
            this.lblPremios16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // chkPleno
            // 
            this.chkPleno.Checked = true;
            this.chkPleno.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkPleno.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkPleno.Location = new System.Drawing.Point(346, 27);
            this.chkPleno.Name = "chkPleno";
            this.chkPleno.Size = new System.Drawing.Size(170, 35);
            this.chkPleno.TabIndex = 187;
            this.chkPleno.Text = "Considerar último partido como pleno";
            // 
            // P1
            // 
            this.P1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.P1.Location = new System.Drawing.Point(20, 95);
            this.P1.Name = "P1";
            this.P1.Size = new System.Drawing.Size(25, 20);
            this.P1.TabIndex = 188;
            this.P1.Text = "1";
            this.P1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.P1.Click += new System.EventHandler(this.P16_Click);
            // 
            // P2
            // 
            this.P2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.P2.Location = new System.Drawing.Point(20, 119);
            this.P2.Name = "P2";
            this.P2.Size = new System.Drawing.Size(25, 20);
            this.P2.TabIndex = 189;
            this.P2.Text = "2";
            this.P2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.P2.Click += new System.EventHandler(this.P16_Click);
            // 
            // P4
            // 
            this.P4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.P4.Location = new System.Drawing.Point(20, 167);
            this.P4.Name = "P4";
            this.P4.Size = new System.Drawing.Size(25, 20);
            this.P4.TabIndex = 191;
            this.P4.Text = "4";
            this.P4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.P4.Click += new System.EventHandler(this.P16_Click);
            // 
            // P3
            // 
            this.P3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.P3.Location = new System.Drawing.Point(20, 143);
            this.P3.Name = "P3";
            this.P3.Size = new System.Drawing.Size(25, 20);
            this.P3.TabIndex = 190;
            this.P3.Text = "3";
            this.P3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.P3.Click += new System.EventHandler(this.P16_Click);
            // 
            // P8
            // 
            this.P8.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.P8.Location = new System.Drawing.Point(20, 271);
            this.P8.Name = "P8";
            this.P8.Size = new System.Drawing.Size(25, 20);
            this.P8.TabIndex = 195;
            this.P8.Text = "8";
            this.P8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.P8.Click += new System.EventHandler(this.P16_Click);
            // 
            // P7
            // 
            this.P7.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.P7.Location = new System.Drawing.Point(20, 247);
            this.P7.Name = "P7";
            this.P7.Size = new System.Drawing.Size(25, 20);
            this.P7.TabIndex = 194;
            this.P7.Text = "7";
            this.P7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.P7.Click += new System.EventHandler(this.P16_Click);
            // 
            // P6
            // 
            this.P6.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.P6.Location = new System.Drawing.Point(20, 223);
            this.P6.Name = "P6";
            this.P6.Size = new System.Drawing.Size(25, 20);
            this.P6.TabIndex = 193;
            this.P6.Text = "6";
            this.P6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.P6.Click += new System.EventHandler(this.P16_Click);
            // 
            // P5
            // 
            this.P5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.P5.Location = new System.Drawing.Point(20, 199);
            this.P5.Name = "P5";
            this.P5.Size = new System.Drawing.Size(25, 20);
            this.P5.TabIndex = 192;
            this.P5.Text = "5";
            this.P5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.P5.Click += new System.EventHandler(this.P16_Click);
            // 
            // P11
            // 
            this.P11.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.P11.Location = new System.Drawing.Point(20, 351);
            this.P11.Name = "P11";
            this.P11.Size = new System.Drawing.Size(25, 20);
            this.P11.TabIndex = 198;
            this.P11.Text = "11";
            this.P11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.P11.Click += new System.EventHandler(this.P16_Click);
            // 
            // P10
            // 
            this.P10.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.P10.Location = new System.Drawing.Point(20, 327);
            this.P10.Name = "P10";
            this.P10.Size = new System.Drawing.Size(25, 20);
            this.P10.TabIndex = 197;
            this.P10.Text = "10";
            this.P10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.P10.Click += new System.EventHandler(this.P16_Click);
            // 
            // P9
            // 
            this.P9.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.P9.Location = new System.Drawing.Point(20, 303);
            this.P9.Name = "P9";
            this.P9.Size = new System.Drawing.Size(25, 20);
            this.P9.TabIndex = 196;
            this.P9.Text = "9";
            this.P9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.P9.Click += new System.EventHandler(this.P16_Click);
            // 
            // P14
            // 
            this.P14.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.P14.Location = new System.Drawing.Point(20, 432);
            this.P14.Name = "P14";
            this.P14.Size = new System.Drawing.Size(25, 20);
            this.P14.TabIndex = 201;
            this.P14.Text = "14";
            this.P14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.P14.Click += new System.EventHandler(this.P16_Click);
            // 
            // P13
            // 
            this.P13.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.P13.Location = new System.Drawing.Point(20, 408);
            this.P13.Name = "P13";
            this.P13.Size = new System.Drawing.Size(25, 20);
            this.P13.TabIndex = 200;
            this.P13.Text = "13";
            this.P13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.P13.Click += new System.EventHandler(this.P16_Click);
            // 
            // P12
            // 
            this.P12.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.P12.Location = new System.Drawing.Point(20, 384);
            this.P12.Name = "P12";
            this.P12.Size = new System.Drawing.Size(25, 20);
            this.P12.TabIndex = 199;
            this.P12.Text = "12";
            this.P12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.P12.Click += new System.EventHandler(this.P16_Click);
            // 
            // P16
            // 
            this.P16.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.P16.Location = new System.Drawing.Point(20, 487);
            this.P16.Name = "P16";
            this.P16.Size = new System.Drawing.Size(25, 20);
            this.P16.TabIndex = 203;
            this.P16.Text = "16";
            this.P16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.P16.Click += new System.EventHandler(this.P16_Click);
            // 
            // P15
            // 
            this.P15.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.P15.Location = new System.Drawing.Point(20, 463);
            this.P15.Name = "P15";
            this.P15.Size = new System.Drawing.Size(25, 20);
            this.P15.TabIndex = 202;
            this.P15.Text = "15";
            this.P15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.P15.Click += new System.EventHandler(this.P16_Click);
            // 
            // PosiblesPremiosFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(528, 533);
            this.Controls.Add(this.P16);
            this.Controls.Add(this.P15);
            this.Controls.Add(this.P14);
            this.Controls.Add(this.P13);
            this.Controls.Add(this.P12);
            this.Controls.Add(this.P11);
            this.Controls.Add(this.P10);
            this.Controls.Add(this.P9);
            this.Controls.Add(this.P8);
            this.Controls.Add(this.P7);
            this.Controls.Add(this.P6);
            this.Controls.Add(this.P5);
            this.Controls.Add(this.P4);
            this.Controls.Add(this.P3);
            this.Controls.Add(this.P2);
            this.Controls.Add(this.P1);
            this.Controls.Add(this.chkPleno);
            this.Controls.Add(this.lblPremios16);
            this.Controls.Add(this.lblPremios15);
            this.Controls.Add(this.txt16);
            this.Controls.Add(this.txt15);
            this.Controls.Add(this.lblP16C8);
            this.Controls.Add(this.lblP15C8);
            this.Controls.Add(this.lblP16C7);
            this.Controls.Add(this.lblP15C7);
            this.Controls.Add(this.lblP16C6);
            this.Controls.Add(this.lblP15C6);
            this.Controls.Add(this.lblP16C5);
            this.Controls.Add(this.lblP15C5);
            this.Controls.Add(this.lblP16C4);
            this.Controls.Add(this.lblP15C4);
            this.Controls.Add(this.lblP16C3);
            this.Controls.Add(this.lblP15C3);
            this.Controls.Add(this.lblP16C2);
            this.Controls.Add(this.lblP15C2);
            this.Controls.Add(this.lblP16C1);
            this.Controls.Add(this.lblP15C1);
            this.Controls.Add(this.btnMejoresOpciones);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.btnVer);
            this.Controls.Add(this.ckbResumen);
            this.Controls.Add(this.btnCopiar);
            this.Controls.Add(this.lblPremios10);
            this.Controls.Add(this.lblPremios11);
            this.Controls.Add(this.lblPremios12);
            this.Controls.Add(this.lblPremios13);
            this.Controls.Add(this.lblPremios14);
            this.Controls.Add(this.lblColumnasConPremio);
            this.Controls.Add(this.btnCalcular);
            this.Controls.Add(this.lblNombreArchivo);
            this.Controls.Add(this.btnAbreArchivo);
            this.Controls.Add(this.btnAtras);
            this.Controls.Add(this.btnAdelante);
            this.Controls.Add(this.txt14);
            this.Controls.Add(this.txt13);
            this.Controls.Add(this.txt12);
            this.Controls.Add(this.txt11);
            this.Controls.Add(this.txt10);
            this.Controls.Add(this.txt9);
            this.Controls.Add(this.txt8);
            this.Controls.Add(this.txt7);
            this.Controls.Add(this.txt6);
            this.Controls.Add(this.txt5);
            this.Controls.Add(this.txt4);
            this.Controls.Add(this.txt3);
            this.Controls.Add(this.txt2);
            this.Controls.Add(this.txt1);
            this.Controls.Add(this.lblP8);
            this.Controls.Add(this.lblP7);
            this.Controls.Add(this.lblP6);
            this.Controls.Add(this.lblP5);
            this.Controls.Add(this.lblP4);
            this.Controls.Add(this.lblP3);
            this.Controls.Add(this.lblP2);
            this.Controls.Add(this.lblP1);
            this.Controls.Add(this.lblP14C8);
            this.Controls.Add(this.lblP13C8);
            this.Controls.Add(this.lblP12C8);
            this.Controls.Add(this.lblP11C8);
            this.Controls.Add(this.lblP10C8);
            this.Controls.Add(this.lblP9C8);
            this.Controls.Add(this.lblP8C8);
            this.Controls.Add(this.lblP7C8);
            this.Controls.Add(this.lblP6C8);
            this.Controls.Add(this.lblP5C8);
            this.Controls.Add(this.lblP4C8);
            this.Controls.Add(this.lblP3C8);
            this.Controls.Add(this.lblP2C8);
            this.Controls.Add(this.lblP1C8);
            this.Controls.Add(this.lblP14C7);
            this.Controls.Add(this.lblP13C7);
            this.Controls.Add(this.lblP12C7);
            this.Controls.Add(this.lblP11C7);
            this.Controls.Add(this.lblP10C7);
            this.Controls.Add(this.lblP9C7);
            this.Controls.Add(this.lblP8C7);
            this.Controls.Add(this.lblP7C7);
            this.Controls.Add(this.lblP6C7);
            this.Controls.Add(this.lblP5C7);
            this.Controls.Add(this.lblP4C7);
            this.Controls.Add(this.lblP3C7);
            this.Controls.Add(this.lblP2C7);
            this.Controls.Add(this.lblP1C7);
            this.Controls.Add(this.lblP14C6);
            this.Controls.Add(this.lblP13C6);
            this.Controls.Add(this.lblP12C6);
            this.Controls.Add(this.lblP11C6);
            this.Controls.Add(this.lblP10C6);
            this.Controls.Add(this.lblP9C6);
            this.Controls.Add(this.lblP8C6);
            this.Controls.Add(this.lblP7C6);
            this.Controls.Add(this.lblP6C6);
            this.Controls.Add(this.lblP5C6);
            this.Controls.Add(this.lblP4C6);
            this.Controls.Add(this.lblP3C6);
            this.Controls.Add(this.lblP2C6);
            this.Controls.Add(this.lblP1C6);
            this.Controls.Add(this.lblP14C5);
            this.Controls.Add(this.lblP13C5);
            this.Controls.Add(this.lblP12C5);
            this.Controls.Add(this.lblP11C5);
            this.Controls.Add(this.lblP10C5);
            this.Controls.Add(this.lblP9C5);
            this.Controls.Add(this.lblP8C5);
            this.Controls.Add(this.lblP7C5);
            this.Controls.Add(this.lblP6C5);
            this.Controls.Add(this.lblP5C5);
            this.Controls.Add(this.lblP4C5);
            this.Controls.Add(this.lblP3C5);
            this.Controls.Add(this.lblP2C5);
            this.Controls.Add(this.lblP1C5);
            this.Controls.Add(this.lblP14C4);
            this.Controls.Add(this.lblP13C4);
            this.Controls.Add(this.lblP12C4);
            this.Controls.Add(this.lblP11C4);
            this.Controls.Add(this.lblP10C4);
            this.Controls.Add(this.lblP9C4);
            this.Controls.Add(this.lblP8C4);
            this.Controls.Add(this.lblP7C4);
            this.Controls.Add(this.lblP6C4);
            this.Controls.Add(this.lblP5C4);
            this.Controls.Add(this.lblP4C4);
            this.Controls.Add(this.lblP3C4);
            this.Controls.Add(this.lblP2C4);
            this.Controls.Add(this.lblP1C4);
            this.Controls.Add(this.lblP14C3);
            this.Controls.Add(this.lblP13C3);
            this.Controls.Add(this.lblP12C3);
            this.Controls.Add(this.lblP11C3);
            this.Controls.Add(this.lblP10C3);
            this.Controls.Add(this.lblP9C3);
            this.Controls.Add(this.lblP8C3);
            this.Controls.Add(this.lblP7C3);
            this.Controls.Add(this.lblP6C3);
            this.Controls.Add(this.lblP5C3);
            this.Controls.Add(this.lblP4C3);
            this.Controls.Add(this.lblP3C3);
            this.Controls.Add(this.lblP2C3);
            this.Controls.Add(this.lblP1C3);
            this.Controls.Add(this.lblP14C2);
            this.Controls.Add(this.lblP13C2);
            this.Controls.Add(this.lblP12C2);
            this.Controls.Add(this.lblP11C2);
            this.Controls.Add(this.lblP10C2);
            this.Controls.Add(this.lblP9C2);
            this.Controls.Add(this.lblP8C2);
            this.Controls.Add(this.lblP7C2);
            this.Controls.Add(this.lblP6C2);
            this.Controls.Add(this.lblP5C2);
            this.Controls.Add(this.lblP4C2);
            this.Controls.Add(this.lblP3C2);
            this.Controls.Add(this.lblP2C2);
            this.Controls.Add(this.lblP1C2);
            this.Controls.Add(this.lblP14C1);
            this.Controls.Add(this.lblP13C1);
            this.Controls.Add(this.lblP12C1);
            this.Controls.Add(this.lblP11C1);
            this.Controls.Add(this.lblP10C1);
            this.Controls.Add(this.lblP9C1);
            this.Controls.Add(this.lblP8C1);
            this.Controls.Add(this.lblP7C1);
            this.Controls.Add(this.lblP6C1);
            this.Controls.Add(this.lblP5C1);
            this.Controls.Add(this.lblP4C1);
            this.Controls.Add(this.lblP3C1);
            this.Controls.Add(this.lblP2C1);
            this.Controls.Add(this.lblP1C1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PosiblesPremiosFrm";
            this.Text = "Posibles Premios";
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

        protected void AdaptarInterfaz(int numPartidos)
        {
            if (numPartidos >= 15)
            {
                chkPleno.Checked = true;
            }
            else
            {
                chkPleno.Checked = false;
            }

            TextBox[] textBoxes = { txt1, txt2, txt3, txt4, txt5, txt6, txt7, txt8, txt9, txt10, txt11, txt12, txt13, txt14, txt15, txt16 };
            for (int i = 0; i < textBoxes.Length; i++)
            {
                textBoxes[i].Visible = true;
            }
            for (int i = textBoxes.Length - 1; i > numPartidos - 1; i--)
            {
                textBoxes[i].Visible = false;
                textBoxes[i].Text = "";
            }
        }
        protected void EntradaFichero() 
		{
			string columna;
			OpenFileDialog abreArchivo = new OpenFileDialog();
			abreArchivo.InitialDirectory = Application.StartupPath + "/";
			abreArchivo.Filter = "Columnas(*.txt)|*.txt|Columnas(*.cols)|*.cols|Todos los archivos(*.*)|*.*" ;
			if(abreArchivo.ShowDialog() == DialogResult.OK) 
			{
				btnMejoresOpciones.Enabled = true;
				lblNombreArchivo.Text = Path.GetFileName(abreArchivo.FileName);
                IArchivoColumnas ac = new ArchivoColumnasTexto(abreArchivo.FileName);
                noPartidos = ac.ObtenNumSignos();
                AdaptarInterfaz(noPartidos);
				while(ac.SiguienteColumna())
				{
					columna = ac.LeeColumnaSinComas();
					if(columna.Length != noPartidos)
					{
						MessageBox.Show("Error leyendo columnas");
						arrayColumnas.Clear();
						btnCalcular.Enabled = false;
						lblNombreArchivo.Text = "";
						break;
					}
				    arrayColumnas.Add(columna.Substring(0,noPartidos).ToUpper());
				}
				ac.Cerrar();
				btnCalcular.Enabled = true;
			}
		}

		protected void AnalizarColumnas(string colGanadora)
		{
			for(int i = 0; i < arrayColumnas.Count; i++)
			{
				string columnaAAnalizar = (string)arrayColumnas[i];
				int aciertos = Escrutar(columnaAAnalizar, colGanadora);
				if(aciertos > 9)
				{
					switch(aciertos)
                    {
                        case 16:
                            col16.Add(columnaAAnalizar + aciertos);
                            break;
                        case 15:
                            col15.Add(columnaAAnalizar + aciertos);
                            break;
						case 14:
							col14.Add(columnaAAnalizar + aciertos);
							break;
						case 13:
							col13.Add(columnaAAnalizar + aciertos);
							break;
						case 12:
							col12.Add(columnaAAnalizar + aciertos);
							break;
						case 11:
							col11.Add(columnaAAnalizar + aciertos);
							break;
						case 10:
							col10.Add(columnaAAnalizar + aciertos);
							break;
					}
				}

			}
		}
		protected void AnalizarColumnasResumen(string preString, int partidoNo)
		{			
			string[] signos = {"1","X","2"};
			string newPreString;

			for( int i = 0; i < signos.Length; i++ )
			{
				if(columnaGanadora[partidoNo].ToString() == "*")
				{
					newPreString  = preString + signos[i];
				}
				else
				{
					newPreString = preString + columnaGanadora[partidoNo];
					i=4;

				}
				
								
				if(( partidoNo < columnaGanadora.Length - 1)&&(partidoNo < noPartidos-1))
				{
					AnalizarColumnasResumen(newPreString, partidoNo+1);
				}
				else
				{
					ObtenerResumen(newPreString);						
				}			
			}
					
		}
		

		protected void ObtenerResumen(string cGanadora)
		{
			PosiblesPremiosContenedor contenedor = new PosiblesPremiosContenedor();
			contenedor.ColGanadora = cGanadora;
			for(int i = 0; i < arrayColumnas.Count; i++)
			{
				int aciertos = Escrutar((string)arrayColumnas[i], cGanadora);
				if(aciertos > 9)
				{	
					switch(aciertos)
					{
                        case 16:
                            contenedor.Col16.Add((string)arrayColumnas[i] + "16");
                            break;
                        case 15:
                            contenedor.Col15.Add((string)arrayColumnas[i] + "15");
                            break;
						case 14:
							contenedor.Col14.Add((string)arrayColumnas[i] + "14");
							break;
						case 13:
							contenedor.Col13.Add((string)arrayColumnas[i] + "13");
							break;
						case 12:
							contenedor.Col12.Add((string)arrayColumnas[i] + "12");
							break;
						case 11:
							contenedor.Col11.Add((string)arrayColumnas[i] + "11");
							break;
						case 10:
							contenedor.Col10.Add((string)arrayColumnas[i] + "10");
							break;
					}

				}
			}
            if ((contenedor.Col16.Count > 0) || (contenedor.Col15.Count > 0) || (contenedor.Col14.Count > 0) || (contenedor.Col13.Count > 0) || (contenedor.Col12.Count > 0) || (contenedor.Col11.Count > 0) || (contenedor.Col10.Count > 0))
				{
					resumen.Add(contenedor);
				}
		}
		protected int Escrutar(string cAnalizada, string cGanadora)
		{
			int aciertos = 0;
			int posiblesAciertos = noPartidos;
			for(int i = 0; i < noPartidos - 1; i++)
			{
				if(posiblesAciertos < 10){break;}
				if(cAnalizada[i] == cGanadora[i])
				{
					aciertos++;
				}
				else if(cGanadora[i].ToString() == "*")
				{
					aciertos++;
				}
				else
				{
					posiblesAciertos--;
				}
			}
            //Analizar el último partido
            if (chkPleno.Checked)
            {
                if (aciertos == (noPartidos - 1))
                {
                    if (cAnalizada[noPartidos - 1] == cGanadora[noPartidos - 1] || cGanadora[noPartidos -1].ToString()=="*")
                    {
                        aciertos++;
                    }
                }
            }
            else
            {
                if (cAnalizada[cAnalizada.Length - 1] == cGanadora[cGanadora.Length - 1] || cGanadora[noPartidos - 1].ToString() == "*")
                {
                    aciertos++;
                }
            }
			return aciertos;
		}
		protected void ObtenGanadora()
		{
			signosNoDefinitivos = 0;
			columnaGanadora = "";
            TextBox[] resultados = { txt1, txt2, txt3, txt4, txt5, txt6, txt7, txt8, txt9, txt10, txt11, txt12, txt13, txt14, txt15, txt16 };

			string[] colGanadora = {txt1.Text, txt2.Text,txt3.Text,txt4.Text,txt5.Text,txt6.Text,txt7.Text,txt8.Text,txt9.Text,txt10.Text,txt11.Text,txt12.Text,txt13.Text,txt14.Text,txt15.Text,txt16.Text};
			for(int i = 0; i < colGanadora.Length; i++)
			{
				if((colGanadora[i]=="1")||(colGanadora[i]=="x")||(colGanadora[i]=="X")||(colGanadora[i]=="2"))
				{
					columnaGanadora += colGanadora[i];
				}
				else
				{
					
                    if (resultados[i].Visible)
                    {
                        columnaGanadora += "*";
                        resultados[i].Text = columnaGanadora[i].ToString() ;
                        signosNoDefinitivos++;
                    }
				}
			}

		}
        protected void VaciarArrays()
		{
			arrayPremiadas.Clear();
            col16.Clear();
            col15.Clear();
			col14.Clear();
			col13.Clear();
			col12.Clear();
			col11.Clear();
			col10.Clear();
		}
		private void btnAbreArchivo_Click(object sender, EventArgs e)
		{
			arrayColumnas.Clear();
			EntradaFichero();
		}
		protected void SumarColumnas()
		{
			arrayPremiadas = new ArrayList();
            arrayPremiadas.AddRange(col16);
            arrayPremiadas.AddRange(col15);
			arrayPremiadas.AddRange(col14);
			arrayPremiadas.AddRange(col13);
			arrayPremiadas.AddRange(col12);
			arrayPremiadas.AddRange(col11);
			arrayPremiadas.AddRange(col10);
		}
		protected void MostrarColumnas(int numBoleto)
		{
			SumarColumnas();
			noCol = (numBoleto * 8) - 8;
			LimpiarPantalla();
			LlenarColumnas(noCol);		
		}
		protected void LimpiarPantalla()
		{
			lblP1C1.Text = "";
			lblP2C1.Text = "";
			lblP4C1.Text = "";
			lblP3C1.Text = "";
			lblP8C1.Text = "";
			lblP7C1.Text = "";
			lblP6C1.Text = "";
			lblP5C1.Text = "";
			lblP11C1.Text = "";
			lblP10C1.Text = "";
			lblP9C1.Text = "";
			lblP14C1.Text = "";
			lblP13C1.Text = "";
			lblP12C1.Text = "";
			lblP14C2.Text = "";
			lblP13C2.Text = "";
			lblP12C2.Text = "";
			lblP11C2.Text = "";
			lblP10C2.Text = "";
			lblP9C2.Text = "";
			lblP8C2.Text = "";
			lblP7C2.Text = "";
			lblP6C2.Text = "";
			lblP5C2.Text = "";
			lblP4C2.Text = "";
			lblP3C2.Text = "";
			lblP2C2.Text = "";
			lblP1C2.Text = "";
			lblP14C4.Text = "";
			lblP13C4.Text = "";
			lblP12C4.Text = "";
			lblP11C4.Text = "";
			lblP10C4.Text = "";
			lblP9C4.Text = "";
			lblP8C4.Text = "";
			lblP7C4.Text = "";
			lblP6C4.Text = "";
			lblP5C4.Text = "";
			lblP4C4.Text = "";
			lblP3C4.Text = "";
			lblP2C4.Text = "";
			lblP1C4.Text = "";
			lblP14C3.Text = "";
			lblP13C3.Text = "";
			lblP12C3.Text = "";
			lblP11C3.Text = "";
			lblP10C3.Text = "";
			lblP9C3.Text = "";
			lblP8C3.Text = "";
			lblP7C3.Text = "";
			lblP6C3.Text = "";
			lblP5C3.Text = "";
			lblP4C3.Text = "";
			lblP3C3.Text = "";
			lblP2C3.Text = "";
			lblP1C3.Text = "";
			lblP14C8.Text = "";
			lblP13C8.Text = "";
			lblP12C8.Text = "";
			lblP11C8.Text = "";
			lblP10C8.Text = "";
			lblP9C8.Text = "";
			lblP8C8.Text = "";
			lblP7C8.Text = "";
			lblP6C8.Text = "";
			lblP5C8.Text = "";
			lblP4C8.Text = "";
			lblP3C8.Text = "";
			lblP2C8.Text = "";
			lblP1C8.Text = "";
			lblP14C7.Text = "";
			lblP13C7.Text = "";
			lblP12C7.Text = "";
			lblP11C7.Text = "";
			lblP10C7.Text = "";
			lblP9C7.Text = "";
			lblP8C7.Text = "";
			lblP7C7.Text = "";
			lblP6C7.Text = "";
			lblP5C7.Text = "";
			lblP4C7.Text = "";
			lblP3C7.Text = "";
			lblP2C7.Text = "";
			lblP1C7.Text = "";
			lblP14C6.Text = "";
			lblP13C6.Text = "";
			lblP12C6.Text = "";
			lblP11C6.Text = "";
			lblP10C6.Text = "";
			lblP9C6.Text = "";
			lblP8C6.Text = "";
			lblP7C6.Text = "";
			lblP6C6.Text = "";
			lblP5C6.Text = "";
			lblP4C6.Text = "";
			lblP3C6.Text = "";
			lblP2C6.Text = "";
			lblP1C6.Text = "";
			lblP14C5.Text = "";
			lblP13C5.Text = "";
			lblP12C5.Text = "";
			lblP11C5.Text = "";
			lblP10C5.Text = "";
			lblP9C5.Text = "";
			lblP8C5.Text = "";
			lblP7C5.Text = "";
			lblP6C5.Text = "";
			lblP5C5.Text = "";
			lblP4C5.Text = "";
			lblP3C5.Text = "";
			lblP2C5.Text = "";
			lblP1C5.Text = "";
			lblP8.Text = "";
			lblP7.Text = "";
			lblP6.Text = "";
			lblP5.Text = "";
			lblP4.Text = "";
			lblP3.Text = "";
			lblP2.Text = "";
			lblP1.Text = "";

            lblP1C1.BackColor = Color.Khaki;
            lblP2C1.BackColor = Color.Khaki;
            lblP4C1.BackColor = Color.Khaki;
            lblP3C1.BackColor = Color.Khaki;
            lblP8C1.BackColor = Color.Khaki;
            lblP7C1.BackColor = Color.Khaki;
            lblP6C1.BackColor = Color.Khaki;
            lblP5C1.BackColor = Color.Khaki;
            lblP11C1.BackColor = Color.Khaki;
            lblP10C1.BackColor = Color.Khaki;
            lblP9C1.BackColor = Color.Khaki;
            lblP14C1.BackColor = Color.Khaki;
            lblP13C1.BackColor = Color.Khaki;
            lblP12C1.BackColor = Color.Khaki;
            lblP14C2.BackColor = Color.Khaki;
            lblP13C2.BackColor = Color.Khaki;
            lblP12C2.BackColor = Color.Khaki;
            lblP11C2.BackColor = Color.Khaki;
            lblP10C2.BackColor = Color.Khaki;
            lblP9C2.BackColor = Color.Khaki;
            lblP8C2.BackColor = Color.Khaki;
            lblP7C2.BackColor = Color.Khaki;
            lblP6C2.BackColor = Color.Khaki;
            lblP5C2.BackColor = Color.Khaki;
            lblP4C2.BackColor = Color.Khaki;
            lblP3C2.BackColor = Color.Khaki;
            lblP2C2.BackColor = Color.Khaki;
            lblP1C2.BackColor = Color.Khaki;
            lblP14C4.BackColor = Color.Khaki;
            lblP13C4.BackColor = Color.Khaki;
            lblP12C4.BackColor = Color.Khaki;
            lblP11C4.BackColor = Color.Khaki;
            lblP10C4.BackColor = Color.Khaki;
            lblP9C4.BackColor = Color.Khaki;
            lblP8C4.BackColor = Color.Khaki;
            lblP7C4.BackColor = Color.Khaki;
            lblP6C4.BackColor = Color.Khaki;
            lblP5C4.BackColor = Color.Khaki;
            lblP4C4.BackColor = Color.Khaki;
            lblP3C4.BackColor = Color.Khaki;
            lblP2C4.BackColor = Color.Khaki;
            lblP1C4.BackColor = Color.Khaki;
            lblP14C3.BackColor = Color.Khaki;
            lblP13C3.BackColor = Color.Khaki;
            lblP12C3.BackColor = Color.Khaki;
            lblP11C3.BackColor = Color.Khaki;
            lblP10C3.BackColor = Color.Khaki;
            lblP9C3.BackColor = Color.Khaki;
            lblP8C3.BackColor = Color.Khaki;
            lblP7C3.BackColor = Color.Khaki;
            lblP6C3.BackColor = Color.Khaki;
            lblP5C3.BackColor = Color.Khaki;
            lblP4C3.BackColor = Color.Khaki;
            lblP3C3.BackColor = Color.Khaki;
            lblP2C3.BackColor = Color.Khaki;
            lblP1C3.BackColor = Color.Khaki;
            lblP14C8.BackColor = Color.Khaki;
            lblP13C8.BackColor = Color.Khaki;
            lblP12C8.BackColor = Color.Khaki;
            lblP11C8.BackColor = Color.Khaki;
            lblP10C8.BackColor = Color.Khaki;
            lblP9C8.BackColor = Color.Khaki;
            lblP8C8.BackColor = Color.Khaki;
            lblP7C8.BackColor = Color.Khaki;
            lblP6C8.BackColor = Color.Khaki;
            lblP5C8.BackColor = Color.Khaki;
            lblP4C8.BackColor = Color.Khaki;
            lblP3C8.BackColor = Color.Khaki;
            lblP2C8.BackColor = Color.Khaki;
            lblP1C8.BackColor = Color.Khaki;
            lblP14C7.BackColor = Color.Khaki;
            lblP13C7.BackColor = Color.Khaki;
            lblP12C7.BackColor = Color.Khaki;
            lblP11C7.BackColor = Color.Khaki;
            lblP10C7.BackColor = Color.Khaki;
            lblP9C7.BackColor = Color.Khaki;
            lblP8C7.BackColor = Color.Khaki;
            lblP7C7.BackColor = Color.Khaki;
            lblP6C7.BackColor = Color.Khaki;
            lblP5C7.BackColor = Color.Khaki;
            lblP4C7.BackColor = Color.Khaki;
            lblP3C7.BackColor = Color.Khaki;
            lblP2C7.BackColor = Color.Khaki;
            lblP1C7.BackColor = Color.Khaki;
            lblP14C6.BackColor = Color.Khaki;
            lblP13C6.BackColor = Color.Khaki;
            lblP12C6.BackColor = Color.Khaki;
            lblP11C6.BackColor = Color.Khaki;
            lblP10C6.BackColor = Color.Khaki;
            lblP9C6.BackColor = Color.Khaki;
            lblP8C6.BackColor = Color.Khaki;
            lblP7C6.BackColor = Color.Khaki;
            lblP6C6.BackColor = Color.Khaki;
            lblP5C6.BackColor = Color.Khaki;
            lblP4C6.BackColor = Color.Khaki;
            lblP3C6.BackColor = Color.Khaki;
            lblP2C6.BackColor = Color.Khaki;
            lblP1C6.BackColor = Color.Khaki;
            lblP14C5.BackColor = Color.Khaki;
            lblP13C5.BackColor = Color.Khaki;
            lblP12C5.BackColor = Color.Khaki;
            lblP11C5.BackColor = Color.Khaki;
            lblP10C5.BackColor = Color.Khaki;
            lblP9C5.BackColor = Color.Khaki;
            lblP8C5.BackColor = Color.Khaki;
            lblP7C5.BackColor = Color.Khaki;
            lblP6C5.BackColor = Color.Khaki;
            lblP5C5.BackColor = Color.Khaki;
            lblP4C5.BackColor = Color.Khaki;
            lblP3C5.BackColor = Color.Khaki;
            lblP2C5.BackColor = Color.Khaki;
            lblP1C5.BackColor = Color.Khaki;

            lblP1C1.ForeColor = Color.Black;
            lblP2C1.ForeColor = Color.Black;
            lblP4C1.ForeColor = Color.Black;
            lblP3C1.ForeColor = Color.Black;
            lblP8C1.ForeColor = Color.Black;
            lblP7C1.ForeColor = Color.Black;
            lblP6C1.ForeColor = Color.Black;
            lblP5C1.ForeColor = Color.Black;
            lblP11C1.ForeColor = Color.Black;
            lblP10C1.ForeColor = Color.Black;
            lblP9C1.ForeColor = Color.Black;
            lblP14C1.ForeColor = Color.Black;
            lblP13C1.ForeColor = Color.Black;
            lblP12C1.ForeColor = Color.Black;
            lblP14C2.ForeColor = Color.Black;
            lblP13C2.ForeColor = Color.Black;
            lblP12C2.ForeColor = Color.Black;
            lblP11C2.ForeColor = Color.Black;
            lblP10C2.ForeColor = Color.Black;
            lblP9C2.ForeColor = Color.Black;
            lblP8C2.ForeColor = Color.Black;
            lblP7C2.ForeColor = Color.Black;
            lblP6C2.ForeColor = Color.Black;
            lblP5C2.ForeColor = Color.Black;
            lblP4C2.ForeColor = Color.Black;
            lblP3C2.ForeColor = Color.Black;
            lblP2C2.ForeColor = Color.Black;
            lblP1C2.ForeColor = Color.Black;
            lblP14C4.ForeColor = Color.Black;
            lblP13C4.ForeColor = Color.Black;
            lblP12C4.ForeColor = Color.Black;
            lblP11C4.ForeColor = Color.Black;
            lblP10C4.ForeColor = Color.Black;
            lblP9C4.ForeColor = Color.Black;
            lblP8C4.ForeColor = Color.Black;
            lblP7C4.ForeColor = Color.Black;
            lblP6C4.ForeColor = Color.Black;
            lblP5C4.ForeColor = Color.Black;
            lblP4C4.ForeColor = Color.Black;
            lblP3C4.ForeColor = Color.Black;
            lblP2C4.ForeColor = Color.Black;
            lblP1C4.ForeColor = Color.Black;
            lblP14C3.ForeColor = Color.Black;
            lblP13C3.ForeColor = Color.Black;
            lblP12C3.ForeColor = Color.Black;
            lblP11C3.ForeColor = Color.Black;
            lblP10C3.ForeColor = Color.Black;
            lblP9C3.ForeColor = Color.Black;
            lblP8C3.ForeColor = Color.Black;
            lblP7C3.ForeColor = Color.Black;
            lblP6C3.ForeColor = Color.Black;
            lblP5C3.ForeColor = Color.Black;
            lblP4C3.ForeColor = Color.Black;
            lblP3C3.ForeColor = Color.Black;
            lblP2C3.ForeColor = Color.Black;
            lblP1C3.ForeColor = Color.Black;
            lblP14C8.ForeColor = Color.Black;
            lblP13C8.ForeColor = Color.Black;
            lblP12C8.ForeColor = Color.Black;
            lblP11C8.ForeColor = Color.Black;
            lblP10C8.ForeColor = Color.Black;
            lblP9C8.ForeColor = Color.Black;
            lblP8C8.ForeColor = Color.Black;
            lblP7C8.ForeColor = Color.Black;
            lblP6C8.ForeColor = Color.Black;
            lblP5C8.ForeColor = Color.Black;
            lblP4C8.ForeColor = Color.Black;
            lblP3C8.ForeColor = Color.Black;
            lblP2C8.ForeColor = Color.Black;
            lblP1C8.ForeColor = Color.Black;
            lblP14C7.ForeColor = Color.Black;
            lblP13C7.ForeColor = Color.Black;
            lblP12C7.ForeColor = Color.Black;
            lblP11C7.ForeColor = Color.Black;
            lblP10C7.ForeColor = Color.Black;
            lblP9C7.ForeColor = Color.Black;
            lblP8C7.ForeColor = Color.Black;
            lblP7C7.ForeColor = Color.Black;
            lblP6C7.ForeColor = Color.Black;
            lblP5C7.ForeColor = Color.Black;
            lblP4C7.ForeColor = Color.Black;
            lblP3C7.ForeColor = Color.Black;
            lblP2C7.ForeColor = Color.Black;
            lblP1C7.ForeColor = Color.Black;
            lblP14C6.ForeColor = Color.Black;
            lblP13C6.ForeColor = Color.Black;
            lblP12C6.ForeColor = Color.Black;
            lblP11C6.ForeColor = Color.Black;
            lblP10C6.ForeColor = Color.Black;
            lblP9C6.ForeColor = Color.Black;
            lblP8C6.ForeColor = Color.Black;
            lblP7C6.ForeColor = Color.Black;
            lblP6C6.ForeColor = Color.Black;
            lblP5C6.ForeColor = Color.Black;
            lblP4C6.ForeColor = Color.Black;
            lblP3C6.ForeColor = Color.Black;
            lblP2C6.ForeColor = Color.Black;
            lblP1C6.ForeColor = Color.Black;
            lblP14C5.ForeColor = Color.Black;
            lblP13C5.ForeColor = Color.Black;
            lblP12C5.ForeColor = Color.Black;
            lblP11C5.ForeColor = Color.Black;
            lblP10C5.ForeColor = Color.Black;
            lblP9C5.ForeColor = Color.Black;
            lblP8C5.ForeColor = Color.Black;
            lblP7C5.ForeColor = Color.Black;
            lblP6C5.ForeColor = Color.Black;
            lblP5C5.ForeColor = Color.Black;
            lblP4C5.ForeColor = Color.Black;
            lblP3C5.ForeColor = Color.Black;
            lblP2C5.ForeColor = Color.Black;
            lblP1C5.ForeColor = Color.Black;
		}
		protected void LlenarColumnas(int numCol)
		{
			//Hay que obtener un arrayList a partir de los datos
			int columnasQueQuedan = arrayPremiadas.Count - numCol;
			ArrayList columnasAMostrar;
			if(columnasQueQuedan <= 8)
			{
				columnasAMostrar = arrayPremiadas.GetRange(numCol,columnasQueQuedan);
			}
			else
			{
				columnasAMostrar = arrayPremiadas.GetRange(numCol,8);
			}

            Label[] columnaUno = { lblP1C1, lblP2C1, lblP3C1, lblP4C1, lblP5C1, lblP6C1, lblP7C1, lblP8C1, lblP9C1, lblP10C1, lblP11C1, lblP12C1, lblP13C1, lblP14C1, lblP15C1, lblP16C1, lblP1 };
            Label[] columnaDos = { lblP1C2, lblP2C2, lblP3C2, lblP4C2, lblP5C2, lblP6C2, lblP7C2, lblP8C2, lblP9C2, lblP10C2, lblP11C2, lblP12C2, lblP13C2, lblP14C2, lblP15C2, lblP16C2, lblP2 };
            Label[] columnaTres = { lblP1C3, lblP2C3, lblP3C3, lblP4C3, lblP5C3, lblP6C3, lblP7C3, lblP8C3, lblP9C3, lblP10C3, lblP11C3, lblP12C3, lblP13C3, lblP14C3, lblP15C3, lblP16C3, lblP3 };
            Label[] columnaCuatro = { lblP1C4, lblP2C4, lblP3C4, lblP4C4, lblP5C4, lblP6C4, lblP7C4, lblP8C4, lblP9C4, lblP10C4, lblP11C4, lblP12C4, lblP13C4, lblP14C4, lblP15C4, lblP16C4, lblP4 };
            Label[] columnaCinco = { lblP1C5, lblP2C5, lblP3C5, lblP4C5, lblP5C5, lblP6C5, lblP7C5, lblP8C5, lblP9C5, lblP10C5, lblP11C5, lblP12C5, lblP13C5, lblP14C5, lblP15C5, lblP16C5, lblP5 };
            Label[] columnaSeis = { lblP1C6, lblP2C6, lblP3C6, lblP4C6, lblP5C6, lblP6C6, lblP7C6, lblP8C6, lblP9C6, lblP10C6, lblP11C6, lblP12C6, lblP13C6, lblP14C6, lblP15C6, lblP16C6, lblP6 };
			Label[] columnaSiete = {lblP1C7, lblP2C7,lblP3C7, lblP4C7,lblP5C7, lblP6C7,lblP7C7, lblP8C7,lblP9C7, lblP10C7,lblP11C7, lblP12C7,lblP13C7, lblP14C7, lblP15C7,lblP16C7, lblP7};
			Label[] columnaOcho = {lblP1C8, lblP2C8,lblP3C8, lblP4C8,lblP5C8, lblP6C8,lblP7C8, lblP8C8,lblP9C8, lblP10C8,lblP11C8, lblP12C8,lblP13C8, lblP14C8,lblP15C8,lblP16C8,  lblP8};
			Label[][] arrayDatos = {columnaUno, columnaDos, columnaTres, columnaCuatro, columnaCinco, columnaSeis, columnaSiete, columnaOcho};
			TextBox[] resultados = {txt1,txt2,txt3,txt4,txt5,txt6,txt7,txt8,txt9,txt10,txt11,txt12,txt13,txt14,txt15,txt16};
			for(int i = 0; i < columnasAMostrar.Count; i++)
			{
				
					string columnaPremiada = (string)columnasAMostrar[i];
					string columna = columnaPremiada.Substring(0,noPartidos);
					string aciertos = columnaPremiada.Substring(noPartidos,2);
					for(int j=0; j < columnaPremiada.Length - 1; j++)
					{
						
						if(j<noPartidos)
						{
							Label label = arrayDatos[i][j];
							label.Text = columna[j].ToString();
							TextBox txtBox = resultados[j];
							if(txtBox.Text != label.Text)
							{
								if((txtBox.Text != "*")&&(label.Text != ""))
								{
									label.BackColor = Color.Red;
									label.ForeColor = Color.White;
								}
							}
							else
							{
								label.BackColor = Color.Khaki;
								label.ForeColor = Color.Black;
							}
						}
						else
						{
							Label label = arrayDatos[i][16];
							label.Text = aciertos;
						}
					}
				
			}
		}
		protected int ObtenerNoBoletos()
		{
			int numBoletos;
			float totalPartes = Convert.ToInt32(arrayPremiadas.Count / 8);
			float resto = arrayPremiadas.Count % 8;
			if(resto > 0)
			{
				numBoletos = Convert.ToInt32(totalPartes + 1);
			}
			else
			{
				numBoletos = Convert.ToInt32(totalPartes);
			}
			return numBoletos;
			
		}
		protected void MostrarOpciones()
		{
            int p16=col16.Count;
            int p15 = col15.Count;
            int p14=col14.Count;
            int p13 = col13.Count;
            int p12=col12.Count;
            int p11=col11.Count;
            int p10=col10.Count;

            if (chkPleno.Checked)
            {
                p14 += p15;
            }

            lblPremios16.Text = "Optando a 16: " + p16;
            lblPremios15.Text = "Optando a 15: " + p15;
			lblPremios14.Text = "Optando a 14: " + p14;
			lblPremios13.Text = "Optando a 13: " + p13;
			lblPremios12.Text = "Optando a 12: " + p12;
			lblPremios11.Text = "Optando a 11: " + p11;
			lblPremios10.Text = "Optando a 10: " + p10;
		}
		protected void GrabarResumen()
		{
		    SaveFileDialog guardaArchivo = new SaveFileDialog();
			guardaArchivo.InitialDirectory = ".\\";
			guardaArchivo.Filter = "Resumen(*.txt)|*.txt";
			if(guardaArchivo.ShowDialog() == DialogResult.OK)
			{
				string nombreArchivoResumen = Path.GetFileName(guardaArchivo.FileName);
				StreamWriter sw = new StreamWriter(nombreArchivoResumen);
				sw.WriteLine("Posibles Premios a falta de " + signosNoDefinitivos + " partidos");
				for(int i = 0; i < resumen.Count; i++)
				{
					PosiblesPremiosContenedor contenedor = (PosiblesPremiosContenedor)resumen[i];
					
					sw.WriteLine("----------------------------------------------");
					sw.WriteLine("Columna Ganadora: " + contenedor.ColGanadora);
					sw.WriteLine("----------------------------------------------");
					for(int j = 0; j < contenedor.Col14.Count; j++)
					{
						string columna = (string)contenedor.Col14[j];
						sw.WriteLine(columna.Substring(0,14) + " 14 aciertos");
					}
					for(int j = 0; j < contenedor.Col13.Count; j++)
					{
						string columna = (string)contenedor.Col13[j];
						sw.WriteLine(columna.Substring(0,14) + " 13 aciertos");
					}
					for(int j = 0; j < contenedor.Col12.Count; j++)
					{
						string columna = (string)contenedor.Col12[j];
						sw.WriteLine(columna.Substring(0,14) + " 12 aciertos");
					}
					for(int j = 0; j < contenedor.Col11.Count; j++)
					{
						string columna = (string)contenedor.Col11[j];
						sw.WriteLine(columna.Substring(0,14) + " 11 aciertos");
					}
					for(int j = 0; j < contenedor.Col10.Count; j++)
					{
						string columna = (string)contenedor.Col10[j];
						sw.WriteLine(columna.Substring(0,14) + " 10 aciertos");
					}
					sw.WriteLine("----------------------------------------------");
					sw.WriteLine(" ");
				}
				sw.Close();
				System.Diagnostics.Process.Start(nombreArchivoResumen);
			}
			
		}
		private void btnCalcular_Click(object sender, EventArgs e)
		{			
			VaciarArrays();
			ObtenGanadora();
			AnalizarColumnas(columnaGanadora);
			SumarColumnas();
			noBoletos = ObtenerNoBoletos();
			AdaptarControlesDesplazamiento();
			int columnasPremiadas = arrayPremiadas.Count;
			if(ckbResumen.Checked)
			{
				//Limpiar un resumen anterior
				resumen.Clear();
				AnalizarColumnasResumen("",0);
				if(resumen.Count > 0)
				{
					resumen.Sort(new PosiblesPremiosComparer());
					btnGuardar.Enabled = true;
					btnVer.Enabled = true;
				}
				else
				{
					btnGuardar.Enabled = false;
					btnVer.Enabled = false;
				}
			}
						
			if(columnasPremiadas > 0)
			{			
				noBoleto = 1;
				MostrarColumnas(noBoleto);
				lblColumnasConPremio.Text = "Columnas con premio: " + columnasPremiadas;
				MostrarOpciones();
				btnCopiar.Enabled = true;
			}
			else
			{
				lblColumnasConPremio.Text = "No hay premios :(";
				MostrarOpciones();
				btnCopiar.Enabled = false;
			}

		}
		protected void AdaptarControlesDesplazamiento()
		{
			if(noBoletos == 0)
			{
				btnAdelante.Enabled = false;
				btnAtras.Enabled = false;
				LimpiarPantalla();
			}
			else if(noBoleto <= 1)
			{
				btnAdelante.Enabled = true;
				btnAtras.Enabled = false;
			}
			else if((noBoleto >= 1)&&(noBoleto < noBoletos))
			{
				btnAdelante.Enabled = true;
				btnAtras.Enabled = true;
			}
			else if(noBoleto >= noBoletos)
			{
				btnAdelante.Enabled = false;
				btnAtras.Enabled = true;
			}
		}
		private void btnAdelante_Click(object sender, EventArgs e)
		{
			noBoleto++;
			AdaptarControlesDesplazamiento();
			MostrarColumnas(noBoleto);
			
		}

		private void btnAtras_Click(object sender, EventArgs e)
		{
			noBoleto--;
			AdaptarControlesDesplazamiento();
			MostrarColumnas(noBoleto);
		}

		private void btnCalcular_EnabledChanged(object sender, EventArgs e)
		{
			if(btnCalcular.Enabled)
				btnCalcular.BackColor=Color.LightSalmon;
			else
				btnCalcular.BackColor=Color.Silver;
		}

		private void btnCopiar_Click(object sender, EventArgs e)
		{
			string info = "Posibles premios a falta de " + signosNoDefinitivos + " partidos:";
            if (noPartidos >= 16)
            {
                info += "\r\n" + lblPremios16.Text;
            }
            if (noPartidos >= 15)
            {
                info += "\r\n" + lblPremios15.Text;
            }
			info += "\r\n" + lblPremios14.Text;
			info += "\r\n" + lblPremios13.Text;
			info += "\r\n" + lblPremios12.Text;
			info += "\r\n" + lblPremios11.Text;
			info += "\r\n" + lblPremios10.Text;

			Clipboard.SetDataObject(info,true);
		}

		private void btnVer_Click(object sender, EventArgs e)
		{
			VisorPosiblesPremios visor = new VisorPosiblesPremios(resumen);
			visor.ShowDialog();
		}

		private void btnGuardar_Click(object sender, EventArgs e)
		{
			if(resumen.Count > 0)
			{
				GrabarResumen();
			}
		}

		private void btnMejoresOpciones_Click(object sender, EventArgs e)
		{
			ObtenGanadora();
			MejoresOpcionesFrm mejoresOpciones = new MejoresOpcionesFrm(chkPleno.Checked);
			mejoresOpciones.ArchivoColumnas = arrayColumnas;
			mejoresOpciones.ColumnaGanadora = columnaGanadora;
			mejoresOpciones.ShowDialog();
		}



        #region Eventos de Interfaz
        private void txt16_VisibleChanged(object sender, EventArgs e)
        {
            lblP16C1.Visible = txt16.Visible;
            lblP16C2.Visible = txt16.Visible;
            lblP16C3.Visible = txt16.Visible;
            lblP16C4.Visible = txt16.Visible;
            lblP16C5.Visible = txt16.Visible;
            lblP16C6.Visible = txt16.Visible;
            lblP16C7.Visible = txt16.Visible;
            lblP16C8.Visible = txt16.Visible;
            P16.Visible = txt16.Visible;
            lblPremios16.Visible = txt16.Visible;

        }
        private void txt15_VisibleChanged(object sender, EventArgs e)
        {
            lblP15C1.Visible = txt15.Visible;
            lblP15C2.Visible = txt15.Visible;
            lblP15C3.Visible = txt15.Visible;
            lblP15C4.Visible = txt15.Visible;
            lblP15C5.Visible = txt15.Visible;
            lblP15C6.Visible = txt15.Visible;
            lblP15C7.Visible = txt15.Visible;
            lblP15C8.Visible = txt15.Visible;
            P15.Visible = txt15.Visible;
            lblPremios15.Visible = txt15.Visible;

        }
        private void txt13_VisibleChanged(object sender, EventArgs e)
        {
            lblP13C1.Visible = txt13.Visible;
            lblP13C2.Visible = txt13.Visible;
            lblP13C3.Visible = txt13.Visible;
            lblP13C4.Visible = txt13.Visible;
            lblP13C5.Visible = txt13.Visible;
            lblP13C6.Visible = txt13.Visible;
            lblP13C7.Visible = txt13.Visible;
            lblP13C8.Visible = txt13.Visible;
            P13.Visible = txt13.Visible;
            lblPremios13.Visible = txt13.Visible;

        }
        private void txt14_VisibleChanged(object sender, EventArgs e)
        {
            lblP14C1.Visible = txt14.Visible;
            lblP14C2.Visible = txt14.Visible;
            lblP14C3.Visible = txt14.Visible;
            lblP14C4.Visible = txt14.Visible;
            lblP14C5.Visible = txt14.Visible;
            lblP14C6.Visible = txt14.Visible;
            lblP14C7.Visible = txt14.Visible;
            lblP14C8.Visible = txt14.Visible;
            P14.Visible = txt14.Visible;
            lblPremios14.Visible = txt14.Visible;

        }
        private void txt12_VisibleChanged(object sender, EventArgs e)
        {
            lblP12C1.Visible = txt12.Visible;
            lblP12C2.Visible = txt12.Visible;
            lblP12C3.Visible = txt12.Visible;
            lblP12C4.Visible = txt12.Visible;
            lblP12C5.Visible = txt12.Visible;
            lblP12C6.Visible = txt12.Visible;
            lblP12C7.Visible = txt12.Visible;
            lblP12C8.Visible = txt12.Visible;
            P12.Visible = txt12.Visible;
            lblPremios12.Visible = txt12.Visible;

        }
        private void txt11_VisibleChanged(object sender, EventArgs e)
        {
            lblP11C1.Visible = txt11.Visible;
            lblP11C2.Visible = txt11.Visible;
            lblP11C3.Visible = txt11.Visible;
            lblP11C4.Visible = txt11.Visible;
            lblP11C5.Visible = txt11.Visible;
            lblP11C6.Visible = txt11.Visible;
            lblP11C7.Visible = txt11.Visible;
            lblP11C8.Visible = txt11.Visible;
            P11.Visible = txt11.Visible;
            lblPremios11.Visible = txt11.Visible;

        }
        private void txt10_VisibleChanged(object sender, EventArgs e)
        {
            lblP10C1.Visible = txt10.Visible;
            lblP10C2.Visible = txt10.Visible;
            lblP10C3.Visible = txt10.Visible;
            lblP10C4.Visible = txt10.Visible;
            lblP10C5.Visible = txt10.Visible;
            lblP10C6.Visible = txt10.Visible;
            lblP10C7.Visible = txt10.Visible;
            lblP10C8.Visible = txt10.Visible;
            P10.Visible = txt10.Visible;
            lblPremios10.Visible = txt10.Visible;

        }
        private void txt9_VisibleChanged(object sender, EventArgs e)
        {
            lblP9C1.Visible = txt9.Visible;
            lblP9C2.Visible = txt9.Visible;
            lblP9C3.Visible = txt9.Visible;
            lblP9C4.Visible = txt9.Visible;
            lblP9C5.Visible = txt9.Visible;
            lblP9C6.Visible = txt9.Visible;
            lblP9C7.Visible = txt9.Visible;
            lblP9C8.Visible = txt9.Visible;
            P9.Visible = txt9.Visible;

        }
        private void txt8_VisibleChanged(object sender, EventArgs e)
        {
            lblP8C1.Visible = txt8.Visible;
            lblP8C2.Visible = txt8.Visible;
            lblP8C3.Visible = txt8.Visible;
            lblP8C4.Visible = txt8.Visible;
            lblP8C5.Visible = txt8.Visible;
            lblP8C6.Visible = txt8.Visible;
            lblP8C7.Visible = txt8.Visible;
            lblP8C8.Visible = txt8.Visible;
            P8.Visible = txt8.Visible;

        }
        private void txt7_VisibleChanged(object sender, EventArgs e)
        {
            lblP7C1.Visible = txt7.Visible;
            lblP7C2.Visible = txt7.Visible;
            lblP7C3.Visible = txt7.Visible;
            lblP7C4.Visible = txt7.Visible;
            lblP7C5.Visible = txt7.Visible;
            lblP7C6.Visible = txt7.Visible;
            lblP7C7.Visible = txt7.Visible;
            lblP7C8.Visible = txt7.Visible;
            P7.Visible = txt7.Visible;

        }
        private void txt6_VisibleChanged(object sender, EventArgs e)
        {
            lblP6C1.Visible = txt6.Visible;
            lblP6C2.Visible = txt6.Visible;
            lblP6C3.Visible = txt6.Visible;
            lblP6C4.Visible = txt6.Visible;
            lblP6C5.Visible = txt6.Visible;
            lblP6C6.Visible = txt6.Visible;
            lblP6C7.Visible = txt6.Visible;
            lblP6C8.Visible = txt6.Visible;
            P6.Visible = txt6.Visible;

        }
        private void txt5_VisibleChanged(object sender, EventArgs e)
        {
            lblP5C1.Visible = txt5.Visible;
            lblP5C2.Visible = txt5.Visible;
            lblP5C3.Visible = txt5.Visible;
            lblP5C4.Visible = txt5.Visible;
            lblP5C5.Visible = txt5.Visible;
            lblP5C6.Visible = txt5.Visible;
            lblP5C7.Visible = txt5.Visible;
            lblP5C8.Visible = txt5.Visible;
            P5.Visible = txt5.Visible;

        }
        private void txt4_VisibleChanged(object sender, EventArgs e)
        {
            lblP4C1.Visible = txt4.Visible;
            lblP4C2.Visible = txt4.Visible;
            lblP4C3.Visible = txt4.Visible;
            lblP4C4.Visible = txt4.Visible;
            lblP4C5.Visible = txt4.Visible;
            lblP4C6.Visible = txt4.Visible;
            lblP4C7.Visible = txt4.Visible;
            lblP4C8.Visible = txt4.Visible;
            P4.Visible = txt4.Visible;

        }
        private void txt3_VisibleChanged(object sender, EventArgs e)
        {
            lblP3C1.Visible = txt3.Visible;
            lblP3C2.Visible = txt3.Visible;
            lblP3C3.Visible = txt3.Visible;
            lblP3C4.Visible = txt3.Visible;
            lblP3C5.Visible = txt3.Visible;
            lblP3C6.Visible = txt3.Visible;
            lblP3C7.Visible = txt3.Visible;
            lblP3C8.Visible = txt3.Visible;
            P3.Visible = txt3.Visible;

        }
        private void txt2_VisibleChanged(object sender, EventArgs e)
        {
            lblP2C1.Visible = txt2.Visible;
            lblP2C2.Visible = txt2.Visible;
            lblP2C3.Visible = txt2.Visible;
            lblP2C4.Visible = txt2.Visible;
            lblP2C5.Visible = txt2.Visible;
            lblP2C6.Visible = txt2.Visible;
            lblP2C7.Visible = txt2.Visible;
            lblP2C8.Visible = txt2.Visible;
            P2.Visible = txt2.Visible;

        }
        private void txt1_VisibleChanged(object sender, EventArgs e)
        {
            lblP1C1.Visible = txt1.Visible;
            lblP1C2.Visible = txt1.Visible;
            lblP1C3.Visible = txt1.Visible;
            lblP1C4.Visible = txt1.Visible;
            lblP1C5.Visible = txt1.Visible;
            lblP1C6.Visible = txt1.Visible;
            lblP1C7.Visible = txt1.Visible;
            lblP1C8.Visible = txt1.Visible;
            P1.Visible = txt1.Visible;
        }
        
        #endregion

        private void P16_Click(object sender, EventArgs e)
        {
            TextBox[] textBoxes = { txt1, txt2, txt3, txt4, txt5, txt6, txt7, txt8, txt9, txt10, txt11, txt12, txt13, txt14, txt15, txt16 };
            Label txt = (Label)sender;
            int numPartido = Convert.ToInt32(txt.Text);

            switch (textBoxes[numPartido - 1].Text)
            {
                case "":
                case "*":
                    textBoxes[numPartido - 1].Text = "1";
                    break;
                case "1":
                    textBoxes[numPartido - 1].Text = "X";
                    break;
                case "X":
                    textBoxes[numPartido - 1].Text = "2";
                    break;
                case "2":
                    textBoxes[numPartido - 1].Text = "*";
                    break;
            }
        }     

	}
}
