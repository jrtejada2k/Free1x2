// created on 03/12/2004 at 19:10
// Free1X2 : Programa de quinielas "libre"
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
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System;
using System.Text;
using System.IO;
using Free1X2.UI.Controls;
using Free1X2.Analisis;
using Free1X2.Utils;
using Free1X2.EntradaSalida;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI
{
	/// <summary>
	/// Summary description for EstimadorPremiosFrm.
	/// </summary>
	public class EstimadorPremiosFrm : System.Windows.Forms.Form
	{
		private float[,] p = new float [14,3];
		private float[] Cr = new float [14];
		private double[,] v = new double [14,3];
		private int[] Signos = new int[14];
		private double[,] Acertantes = new double [5,2]; //prevision i definitivos
		private double[,] Premios = new double [5,2]; //prevision i definitivos
		private double ProbabilidadCategoria14=1;
		private double Recaudacion=14000000;
        private string moneda = "";
		private double Bote;
		private double PrecioApuesta=0.5;
		private DateTime any;
		private	string[] ValorsJornada;
		private double [] PctDestinadoAPremiosCategoria= new double [5] {0.12, 0.08, 0.08, 0.08, 0.09};
		private double [] DestinadoAPremiosCategoria= new double [5];
		private Porcentajes Pct =new Porcentajes ();
		private int Profundidad=0;
		private int NumApuestas;
		private ValidadorCadenas Valida= new ValidadorCadenas();
		private int[] pot = new int [] {1,3,9,27,81,243,729,2187,6561,19683,59049,177147,531441,1594323};
		private ApuestaProbableCentral[] Ap14T=new ApuestaProbableCentral[4782969] ;
		private BitArray Bits = new BitArray(4782969,false);
		private System.Windows.Forms.Label lblNewBase141;
		private System.Windows.Forms.Label lblNewBase131;
		private System.Windows.Forms.Label lblNewBase121;
		private System.Windows.Forms.Label lblNewBase111;
		private System.Windows.Forms.Label lblNewBase101;
		private System.Windows.Forms.Label lblNewBase91;
		private System.Windows.Forms.Label lblNewBase81;
		private System.Windows.Forms.Label lblNewBase71;
		private System.Windows.Forms.Label lblNewBase61;
		private System.Windows.Forms.Label lblNewBase51;
		private System.Windows.Forms.Label lblNewBase41;
		private System.Windows.Forms.Label lblNewBase31;
		private System.Windows.Forms.Label lblNewBase21;
		private System.Windows.Forms.Label lblNewBase11;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label20;
		private System.Windows.Forms.Label label21;
		private System.Windows.Forms.Label label22;
		private System.Windows.Forms.Label label28;
		private System.Windows.Forms.Label label30;
		private System.Windows.Forms.Label label32;
		private System.Windows.Forms.Label label34;
		private System.Windows.Forms.Label label36;
		private System.Windows.Forms.Label label38;
		private System.Windows.Forms.Label label53;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label54;
		private System.Windows.Forms.Label label55;
		private System.Windows.Forms.Label label56;
		private System.Windows.Forms.Label label57;
		private System.Windows.Forms.Label label58;
		private System.Windows.Forms.Label label59;
		private System.Windows.Forms.Label label60;
		private System.Windows.Forms.Label label61;
		private System.Windows.Forms.Label label62;
		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.StatusBarPanel statusBarPanel1;
		private System.Windows.Forms.StatusBarPanel statusBarPanel2;
		private System.Windows.Forms.Label lblPrevAcertantes14;
		private System.Windows.Forms.Label lblPrevPremio14;
		private System.Windows.Forms.Label lblPrevPremio13;
		private System.Windows.Forms.Label lblPrevAcertantes13;
		private System.Windows.Forms.Label lblPrevPremio12;
		private System.Windows.Forms.Label lblPrevAcertantes12;
		private System.Windows.Forms.Label lblPrevPremio11;
		private System.Windows.Forms.Label lblPrevAcertantes11;
		private System.Windows.Forms.Label lblPrevPremio10;
		private System.Windows.Forms.Label lblPrevAcertantes10;
		private System.Windows.Forms.TextBox txRecaudacion;
		private System.Windows.Forms.TextBox txBote;
		private System.Windows.Forms.Label lbNumApuestas;
		private System.Windows.Forms.Label lblDestinadoAPremios;
		private System.Windows.Forms.TextBox txPorcentajeDestinadoAPremios;
		private System.Windows.Forms.TextBox txPorcentajeParaEl14;
		private System.Windows.Forms.Label lblParaEl14;
		private System.Windows.Forms.TextBox txPorcentajeParaEl13;
		private System.Windows.Forms.Label lblParaEl13;
		private System.Windows.Forms.TextBox txPorcentajeParaEl12;
		private System.Windows.Forms.Label lblParaEl12;
		private System.Windows.Forms.TextBox txPorcentajeParaEl11;
		private System.Windows.Forms.Label lblParaEl11;
		private System.Windows.Forms.TextBox txPorcentajeParaEl10;
		private System.Windows.Forms.Label lblParaEl10;
		private System.Windows.Forms.TextBox txPrecioApuesta;
		private System.Windows.Forms.Label label83;
		private System.Windows.Forms.Label label84;
		private System.Windows.Forms.Label label85;
		private System.Windows.Forms.Label label86;
		private System.Windows.Forms.TextBox txTemporada;
		private System.Windows.Forms.Label label87;
		private System.Windows.Forms.TextBox txJornada;
		private System.Windows.Forms.Label label88;
		private System.Windows.Forms.TextBox txAcertantes14;
		private System.Windows.Forms.TextBox txPremio14;
		private System.Windows.Forms.TextBox txPremio13;
		private System.Windows.Forms.TextBox txAcertantes13;
		private System.Windows.Forms.TextBox txPremio12;
		private System.Windows.Forms.TextBox txAcertantes12;
		private System.Windows.Forms.TextBox txPremio11;
		private System.Windows.Forms.TextBox txAcertantes11;
		private System.Windows.Forms.TextBox txPremio10;
		private System.Windows.Forms.TextBox txAcertantes10;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button btJornadaAnterior;
		private System.Windows.Forms.Button btJornadaPosterior;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label label18;
		private System.Windows.Forms.Label label23;
		private System.Windows.Forms.Label label24;
		private System.Windows.Forms.Label label29;
		private System.Windows.Forms.Label label31;
		private System.Windows.Forms.Label label33;
		private System.Windows.Forms.Label label35;
		private System.Windows.Forms.Button btCopiar;
		private System.Windows.Forms.LinkLabel linkLAE;
		private System.Windows.Forms.Button btGuardarEscrutinio;
		private Free1X2.UI.Controls.ControlPorcentajes controlPorcentajes1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public EstimadorPremiosFrm()
		{
			InitializeComponent();

			statusBar1.ShowPanels =true;
			any = DateTime.Now;
			int mes=any.Month;
			txRecaudacion.Text =Recaudacion.ToString ();
			if (mes<7)
			{
				txTemporada.Text = Convert.ToString (any.Year -1)+ "/" + any.Year.ToString () ;
			}
			else
			{
				txTemporada.Text = any.Year.ToString() + "/" + Convert.ToString (any.Year +1);
			}
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EstimadorPremiosFrm));
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.lblPrevAcertantes14 = new System.Windows.Forms.Label();
            this.lblPrevPremio14 = new System.Windows.Forms.Label();
            this.lblPrevPremio13 = new System.Windows.Forms.Label();
            this.lblPrevAcertantes13 = new System.Windows.Forms.Label();
            this.lblPrevPremio12 = new System.Windows.Forms.Label();
            this.lblPrevAcertantes12 = new System.Windows.Forms.Label();
            this.lblPrevPremio11 = new System.Windows.Forms.Label();
            this.lblPrevAcertantes11 = new System.Windows.Forms.Label();
            this.lblPrevPremio10 = new System.Windows.Forms.Label();
            this.lblPrevAcertantes10 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.txRecaudacion = new System.Windows.Forms.TextBox();
            this.txBote = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.lbNumApuestas = new System.Windows.Forms.Label();
            this.lblDestinadoAPremios = new System.Windows.Forms.Label();
            this.label28 = new System.Windows.Forms.Label();
            this.txPorcentajeDestinadoAPremios = new System.Windows.Forms.TextBox();
            this.txPorcentajeParaEl14 = new System.Windows.Forms.TextBox();
            this.lblParaEl14 = new System.Windows.Forms.Label();
            this.label30 = new System.Windows.Forms.Label();
            this.txPorcentajeParaEl13 = new System.Windows.Forms.TextBox();
            this.lblParaEl13 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.txPorcentajeParaEl12 = new System.Windows.Forms.TextBox();
            this.lblParaEl12 = new System.Windows.Forms.Label();
            this.label34 = new System.Windows.Forms.Label();
            this.txPorcentajeParaEl11 = new System.Windows.Forms.TextBox();
            this.lblParaEl11 = new System.Windows.Forms.Label();
            this.label36 = new System.Windows.Forms.Label();
            this.txPorcentajeParaEl10 = new System.Windows.Forms.TextBox();
            this.lblParaEl10 = new System.Windows.Forms.Label();
            this.label38 = new System.Windows.Forms.Label();
            this.txPrecioApuesta = new System.Windows.Forms.TextBox();
            this.label53 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label54 = new System.Windows.Forms.Label();
            this.label55 = new System.Windows.Forms.Label();
            this.label56 = new System.Windows.Forms.Label();
            this.label57 = new System.Windows.Forms.Label();
            this.label58 = new System.Windows.Forms.Label();
            this.label59 = new System.Windows.Forms.Label();
            this.label60 = new System.Windows.Forms.Label();
            this.label61 = new System.Windows.Forms.Label();
            this.label62 = new System.Windows.Forms.Label();
            this.statusBar1 = new System.Windows.Forms.StatusBar();
            this.statusBarPanel1 = new System.Windows.Forms.StatusBarPanel();
            this.statusBarPanel2 = new System.Windows.Forms.StatusBarPanel();
            this.label83 = new System.Windows.Forms.Label();
            this.label84 = new System.Windows.Forms.Label();
            this.label85 = new System.Windows.Forms.Label();
            this.label86 = new System.Windows.Forms.Label();
            this.txTemporada = new System.Windows.Forms.TextBox();
            this.label87 = new System.Windows.Forms.Label();
            this.txJornada = new System.Windows.Forms.TextBox();
            this.label88 = new System.Windows.Forms.Label();
            this.txAcertantes14 = new System.Windows.Forms.TextBox();
            this.txPremio14 = new System.Windows.Forms.TextBox();
            this.txPremio13 = new System.Windows.Forms.TextBox();
            this.txAcertantes13 = new System.Windows.Forms.TextBox();
            this.txPremio12 = new System.Windows.Forms.TextBox();
            this.txAcertantes12 = new System.Windows.Forms.TextBox();
            this.txPremio11 = new System.Windows.Forms.TextBox();
            this.txAcertantes11 = new System.Windows.Forms.TextBox();
            this.txPremio10 = new System.Windows.Forms.TextBox();
            this.txAcertantes10 = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.btJornadaAnterior = new System.Windows.Forms.Button();
            this.btJornadaPosterior = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label29 = new System.Windows.Forms.Label();
            this.label31 = new System.Windows.Forms.Label();
            this.label33 = new System.Windows.Forms.Label();
            this.label35 = new System.Windows.Forms.Label();
            this.btCopiar = new System.Windows.Forms.Button();
            this.linkLAE = new System.Windows.Forms.LinkLabel();
            this.btGuardarEscrutinio = new System.Windows.Forms.Button();
            this.controlPorcentajes1 = new Free1X2.UI.Controls.ControlPorcentajes();
            // StatusBarPanel controls don't support ISupportInitialize in .NET 8
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).BeginInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel2)).BeginInit();
            this.SuspendLayout();
            // 
            // lblNewBase141
            // 
            this.lblNewBase141.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase141.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase141.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase141.ForeColor = System.Drawing.Color.Brown;
            this.lblNewBase141.Location = new System.Drawing.Point(191, 395);
            this.lblNewBase141.Name = "lblNewBase141";
            this.lblNewBase141.Size = new System.Drawing.Size(15, 15);
            this.lblNewBase141.TabIndex = 274;
            this.lblNewBase141.Tag = "13";
            this.lblNewBase141.Text = "1";
            this.lblNewBase141.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase141.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase141.Click += new System.EventHandler(this.GenericLabel_Click);
            this.lblNewBase141.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            // 
            // lblNewBase131
            // 
            this.lblNewBase131.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase131.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase131.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase131.ForeColor = System.Drawing.Color.Brown;
            this.lblNewBase131.Location = new System.Drawing.Point(191, 379);
            this.lblNewBase131.Name = "lblNewBase131";
            this.lblNewBase131.Size = new System.Drawing.Size(15, 15);
            this.lblNewBase131.TabIndex = 273;
            this.lblNewBase131.Tag = "12";
            this.lblNewBase131.Text = "1";
            this.lblNewBase131.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase131.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase131.Click += new System.EventHandler(this.GenericLabel_Click);
            this.lblNewBase131.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            // 
            // lblNewBase121
            // 
            this.lblNewBase121.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase121.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase121.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase121.ForeColor = System.Drawing.Color.Brown;
            this.lblNewBase121.Location = new System.Drawing.Point(191, 363);
            this.lblNewBase121.Name = "lblNewBase121";
            this.lblNewBase121.Size = new System.Drawing.Size(15, 15);
            this.lblNewBase121.TabIndex = 272;
            this.lblNewBase121.Tag = "11";
            this.lblNewBase121.Text = "1";
            this.lblNewBase121.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase121.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase121.Click += new System.EventHandler(this.GenericLabel_Click);
            this.lblNewBase121.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            // 
            // lblNewBase111
            // 
            this.lblNewBase111.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase111.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase111.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase111.ForeColor = System.Drawing.Color.Brown;
            this.lblNewBase111.Location = new System.Drawing.Point(191, 343);
            this.lblNewBase111.Name = "lblNewBase111";
            this.lblNewBase111.Size = new System.Drawing.Size(15, 15);
            this.lblNewBase111.TabIndex = 271;
            this.lblNewBase111.Tag = "10";
            this.lblNewBase111.Text = "1";
            this.lblNewBase111.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase111.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase111.Click += new System.EventHandler(this.GenericLabel_Click);
            this.lblNewBase111.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            // 
            // lblNewBase101
            // 
            this.lblNewBase101.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase101.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase101.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase101.ForeColor = System.Drawing.Color.Brown;
            this.lblNewBase101.Location = new System.Drawing.Point(191, 327);
            this.lblNewBase101.Name = "lblNewBase101";
            this.lblNewBase101.Size = new System.Drawing.Size(15, 15);
            this.lblNewBase101.TabIndex = 270;
            this.lblNewBase101.Tag = "9";
            this.lblNewBase101.Text = "1";
            this.lblNewBase101.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase101.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase101.Click += new System.EventHandler(this.GenericLabel_Click);
            this.lblNewBase101.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            // 
            // lblNewBase91
            // 
            this.lblNewBase91.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase91.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase91.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase91.ForeColor = System.Drawing.Color.Brown;
            this.lblNewBase91.Location = new System.Drawing.Point(191, 311);
            this.lblNewBase91.Name = "lblNewBase91";
            this.lblNewBase91.Size = new System.Drawing.Size(15, 15);
            this.lblNewBase91.TabIndex = 269;
            this.lblNewBase91.Tag = "8";
            this.lblNewBase91.Text = "1";
            this.lblNewBase91.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase91.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase91.Click += new System.EventHandler(this.GenericLabel_Click);
            this.lblNewBase91.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            // 
            // lblNewBase81
            // 
            this.lblNewBase81.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase81.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase81.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase81.ForeColor = System.Drawing.Color.Brown;
            this.lblNewBase81.Location = new System.Drawing.Point(191, 291);
            this.lblNewBase81.Name = "lblNewBase81";
            this.lblNewBase81.Size = new System.Drawing.Size(15, 15);
            this.lblNewBase81.TabIndex = 268;
            this.lblNewBase81.Tag = "7";
            this.lblNewBase81.Text = "1";
            this.lblNewBase81.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase81.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase81.Click += new System.EventHandler(this.GenericLabel_Click);
            this.lblNewBase81.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            // 
            // lblNewBase71
            // 
            this.lblNewBase71.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase71.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase71.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase71.ForeColor = System.Drawing.Color.Brown;
            this.lblNewBase71.Location = new System.Drawing.Point(191, 275);
            this.lblNewBase71.Name = "lblNewBase71";
            this.lblNewBase71.Size = new System.Drawing.Size(15, 15);
            this.lblNewBase71.TabIndex = 267;
            this.lblNewBase71.Tag = "6";
            this.lblNewBase71.Text = "1";
            this.lblNewBase71.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase71.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase71.Click += new System.EventHandler(this.GenericLabel_Click);
            this.lblNewBase71.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            // 
            // lblNewBase61
            // 
            this.lblNewBase61.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase61.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase61.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase61.ForeColor = System.Drawing.Color.Brown;
            this.lblNewBase61.Location = new System.Drawing.Point(191, 259);
            this.lblNewBase61.Name = "lblNewBase61";
            this.lblNewBase61.Size = new System.Drawing.Size(15, 15);
            this.lblNewBase61.TabIndex = 266;
            this.lblNewBase61.Tag = "5";
            this.lblNewBase61.Text = "1";
            this.lblNewBase61.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase61.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase61.Click += new System.EventHandler(this.GenericLabel_Click);
            this.lblNewBase61.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            // 
            // lblNewBase51
            // 
            this.lblNewBase51.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.lblNewBase51.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase51.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase51.ForeColor = System.Drawing.Color.Brown;
            this.lblNewBase51.Location = new System.Drawing.Point(191, 243);
            this.lblNewBase51.Name = "lblNewBase51";
            this.lblNewBase51.Size = new System.Drawing.Size(15, 15);
            this.lblNewBase51.TabIndex = 265;
            this.lblNewBase51.Tag = "4";
            this.lblNewBase51.Text = "1";
            this.lblNewBase51.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase51.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase51.Click += new System.EventHandler(this.GenericLabel_Click);
            this.lblNewBase51.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            // 
            // lblNewBase41
            // 
            this.lblNewBase41.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase41.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase41.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase41.ForeColor = System.Drawing.Color.Brown;
            this.lblNewBase41.Location = new System.Drawing.Point(191, 222);
            this.lblNewBase41.Name = "lblNewBase41";
            this.lblNewBase41.Size = new System.Drawing.Size(15, 15);
            this.lblNewBase41.TabIndex = 264;
            this.lblNewBase41.Tag = "3";
            this.lblNewBase41.Text = "1";
            this.lblNewBase41.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase41.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase41.Click += new System.EventHandler(this.GenericLabel_Click);
            this.lblNewBase41.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            // 
            // lblNewBase31
            // 
            this.lblNewBase31.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase31.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase31.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase31.ForeColor = System.Drawing.Color.Brown;
            this.lblNewBase31.Location = new System.Drawing.Point(191, 206);
            this.lblNewBase31.Name = "lblNewBase31";
            this.lblNewBase31.Size = new System.Drawing.Size(15, 15);
            this.lblNewBase31.TabIndex = 263;
            this.lblNewBase31.Tag = "2";
            this.lblNewBase31.Text = "1";
            this.lblNewBase31.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase31.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase31.Click += new System.EventHandler(this.GenericLabel_Click);
            this.lblNewBase31.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            // 
            // lblNewBase21
            // 
            this.lblNewBase21.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase21.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase21.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase21.ForeColor = System.Drawing.Color.Brown;
            this.lblNewBase21.Location = new System.Drawing.Point(191, 190);
            this.lblNewBase21.Name = "lblNewBase21";
            this.lblNewBase21.Size = new System.Drawing.Size(15, 15);
            this.lblNewBase21.TabIndex = 262;
            this.lblNewBase21.Tag = "1";
            this.lblNewBase21.Text = "1";
            this.lblNewBase21.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase21.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase21.Click += new System.EventHandler(this.GenericLabel_Click);
            this.lblNewBase21.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            // 
            // lblNewBase11
            // 
            this.lblNewBase11.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblNewBase11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblNewBase11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewBase11.ForeColor = System.Drawing.Color.Brown;
            this.lblNewBase11.Location = new System.Drawing.Point(191, 174);
            this.lblNewBase11.Name = "lblNewBase11";
            this.lblNewBase11.Size = new System.Drawing.Size(15, 15);
            this.lblNewBase11.TabIndex = 261;
            this.lblNewBase11.Tag = "0";
            this.lblNewBase11.Text = "1";
            this.lblNewBase11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNewBase11.DoubleClick += new System.EventHandler(this.GenericLabel_DoubleClick);
            this.lblNewBase11.Click += new System.EventHandler(this.GenericLabel_Click);
            this.lblNewBase11.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GenericLabel_MouseMove);
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(232, 230);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(63, 32);
            this.label1.TabIndex = 444;
            this.label1.Text = "Categoría";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(296, 230);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(68, 32);
            this.label2.TabIndex = 445;
            this.label2.Text = "Acertantes";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(365, 230);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 32);
            this.label3.TabIndex = 446;
            this.label3.Text = "Premio";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(230, 264);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(66, 18);
            this.label4.TabIndex = 447;
            this.label4.Text = "14";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            this.label5.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(230, 284);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 18);
            this.label5.TabIndex = 448;
            this.label5.Text = "13";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label6
            // 
            this.label6.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label6.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(230, 304);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(66, 18);
            this.label6.TabIndex = 449;
            this.label6.Text = "12";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label7
            // 
            this.label7.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label7.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(230, 324);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(66, 18);
            this.label7.TabIndex = 450;
            this.label7.Text = "11";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label8
            // 
            this.label8.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label8.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(230, 344);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(66, 18);
            this.label8.TabIndex = 451;
            this.label8.Text = "10";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPrevAcertantes14
            // 
            this.lblPrevAcertantes14.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblPrevAcertantes14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPrevAcertantes14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrevAcertantes14.ForeColor = System.Drawing.Color.Brown;
            this.lblPrevAcertantes14.Location = new System.Drawing.Point(296, 264);
            this.lblPrevAcertantes14.Name = "lblPrevAcertantes14";
            this.lblPrevAcertantes14.Size = new System.Drawing.Size(68, 18);
            this.lblPrevAcertantes14.TabIndex = 452;
            this.lblPrevAcertantes14.Tag = "";
            this.lblPrevAcertantes14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPrevPremio14
            // 
            this.lblPrevPremio14.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblPrevPremio14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPrevPremio14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrevPremio14.ForeColor = System.Drawing.Color.Brown;
            this.lblPrevPremio14.Location = new System.Drawing.Point(365, 264);
            this.lblPrevPremio14.Name = "lblPrevPremio14";
            this.lblPrevPremio14.Size = new System.Drawing.Size(80, 18);
            this.lblPrevPremio14.TabIndex = 453;
            this.lblPrevPremio14.Tag = "";
            this.lblPrevPremio14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPrevPremio13
            // 
            this.lblPrevPremio13.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblPrevPremio13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPrevPremio13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrevPremio13.ForeColor = System.Drawing.Color.Brown;
            this.lblPrevPremio13.Location = new System.Drawing.Point(365, 284);
            this.lblPrevPremio13.Name = "lblPrevPremio13";
            this.lblPrevPremio13.Size = new System.Drawing.Size(80, 18);
            this.lblPrevPremio13.TabIndex = 455;
            this.lblPrevPremio13.Tag = "";
            this.lblPrevPremio13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPrevAcertantes13
            // 
            this.lblPrevAcertantes13.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblPrevAcertantes13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPrevAcertantes13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrevAcertantes13.ForeColor = System.Drawing.Color.Brown;
            this.lblPrevAcertantes13.Location = new System.Drawing.Point(296, 284);
            this.lblPrevAcertantes13.Name = "lblPrevAcertantes13";
            this.lblPrevAcertantes13.Size = new System.Drawing.Size(68, 18);
            this.lblPrevAcertantes13.TabIndex = 454;
            this.lblPrevAcertantes13.Tag = "";
            this.lblPrevAcertantes13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPrevPremio12
            // 
            this.lblPrevPremio12.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblPrevPremio12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPrevPremio12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrevPremio12.ForeColor = System.Drawing.Color.Brown;
            this.lblPrevPremio12.Location = new System.Drawing.Point(365, 304);
            this.lblPrevPremio12.Name = "lblPrevPremio12";
            this.lblPrevPremio12.Size = new System.Drawing.Size(80, 18);
            this.lblPrevPremio12.TabIndex = 457;
            this.lblPrevPremio12.Tag = "";
            this.lblPrevPremio12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPrevAcertantes12
            // 
            this.lblPrevAcertantes12.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblPrevAcertantes12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPrevAcertantes12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrevAcertantes12.ForeColor = System.Drawing.Color.Brown;
            this.lblPrevAcertantes12.Location = new System.Drawing.Point(296, 304);
            this.lblPrevAcertantes12.Name = "lblPrevAcertantes12";
            this.lblPrevAcertantes12.Size = new System.Drawing.Size(68, 18);
            this.lblPrevAcertantes12.TabIndex = 456;
            this.lblPrevAcertantes12.Tag = "";
            this.lblPrevAcertantes12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPrevPremio11
            // 
            this.lblPrevPremio11.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblPrevPremio11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPrevPremio11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrevPremio11.ForeColor = System.Drawing.Color.Brown;
            this.lblPrevPremio11.Location = new System.Drawing.Point(365, 324);
            this.lblPrevPremio11.Name = "lblPrevPremio11";
            this.lblPrevPremio11.Size = new System.Drawing.Size(80, 18);
            this.lblPrevPremio11.TabIndex = 459;
            this.lblPrevPremio11.Tag = "";
            this.lblPrevPremio11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPrevAcertantes11
            // 
            this.lblPrevAcertantes11.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblPrevAcertantes11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPrevAcertantes11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrevAcertantes11.ForeColor = System.Drawing.Color.Brown;
            this.lblPrevAcertantes11.Location = new System.Drawing.Point(296, 324);
            this.lblPrevAcertantes11.Name = "lblPrevAcertantes11";
            this.lblPrevAcertantes11.Size = new System.Drawing.Size(68, 18);
            this.lblPrevAcertantes11.TabIndex = 458;
            this.lblPrevAcertantes11.Tag = "";
            this.lblPrevAcertantes11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPrevPremio10
            // 
            this.lblPrevPremio10.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblPrevPremio10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPrevPremio10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrevPremio10.ForeColor = System.Drawing.Color.Brown;
            this.lblPrevPremio10.Location = new System.Drawing.Point(365, 344);
            this.lblPrevPremio10.Name = "lblPrevPremio10";
            this.lblPrevPremio10.Size = new System.Drawing.Size(80, 18);
            this.lblPrevPremio10.TabIndex = 461;
            this.lblPrevPremio10.Tag = "";
            this.lblPrevPremio10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblPrevAcertantes10
            // 
            this.lblPrevAcertantes10.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblPrevAcertantes10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPrevAcertantes10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPrevAcertantes10.ForeColor = System.Drawing.Color.Brown;
            this.lblPrevAcertantes10.Location = new System.Drawing.Point(296, 344);
            this.lblPrevAcertantes10.Name = "lblPrevAcertantes10";
            this.lblPrevAcertantes10.Size = new System.Drawing.Size(68, 18);
            this.lblPrevAcertantes10.TabIndex = 460;
            this.lblPrevAcertantes10.Tag = "";
            this.lblPrevAcertantes10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label20
            // 
            this.label20.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label20.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.Location = new System.Drawing.Point(176, 28);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(84, 21);
            this.label20.TabIndex = 463;
            this.label20.Text = "Recaudación";
            this.label20.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txRecaudacion
            // 
            this.txRecaudacion.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txRecaudacion.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txRecaudacion.ForeColor = System.Drawing.Color.SaddleBrown;
            this.txRecaudacion.Location = new System.Drawing.Point(264, 28);
            this.txRecaudacion.MaxLength = 11;
            this.txRecaudacion.Name = "txRecaudacion";
            this.txRecaudacion.Size = new System.Drawing.Size(84, 21);
            this.txRecaudacion.TabIndex = 464;
            this.txRecaudacion.Text = "13825260,50";
            this.txRecaudacion.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txRecaudacion.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            this.txRecaudacion.TextChanged += new System.EventHandler(this.GenericLAE_TextChanged);
            // 
            // txBote
            // 
            this.txBote.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txBote.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txBote.ForeColor = System.Drawing.Color.SaddleBrown;
            this.txBote.Location = new System.Drawing.Point(264, 52);
            this.txBote.MaxLength = 10;
            this.txBote.Name = "txBote";
            this.txBote.Size = new System.Drawing.Size(84, 21);
            this.txBote.TabIndex = 466;
            this.txBote.Text = "0,00";
            this.txBote.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txBote.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            this.txBote.TextChanged += new System.EventHandler(this.GenericLAE_TextChanged);
            // 
            // label21
            // 
            this.label21.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label21.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.Location = new System.Drawing.Point(176, 52);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(84, 21);
            this.label21.TabIndex = 465;
            this.label21.Text = "Bote";
            this.label21.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label22
            // 
            this.label22.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label22.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.Location = new System.Drawing.Point(176, 76);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(84, 18);
            this.label22.TabIndex = 467;
            this.label22.Text = "Nº columnas";
            this.label22.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lbNumApuestas
            // 
            this.lbNumApuestas.BackColor = System.Drawing.Color.LemonChiffon;
            this.lbNumApuestas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lbNumApuestas.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbNumApuestas.ForeColor = System.Drawing.Color.Brown;
            this.lbNumApuestas.Location = new System.Drawing.Point(264, 76);
            this.lbNumApuestas.Name = "lbNumApuestas";
            this.lbNumApuestas.Size = new System.Drawing.Size(84, 18);
            this.lbNumApuestas.TabIndex = 468;
            this.lbNumApuestas.Tag = "";
            this.lbNumApuestas.Text = "28000000";
            this.lbNumApuestas.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblDestinadoAPremios
            // 
            this.lblDestinadoAPremios.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblDestinadoAPremios.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDestinadoAPremios.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblDestinadoAPremios.ForeColor = System.Drawing.Color.Brown;
            this.lblDestinadoAPremios.Location = new System.Drawing.Point(536, 28);
            this.lblDestinadoAPremios.Name = "lblDestinadoAPremios";
            this.lblDestinadoAPremios.Size = new System.Drawing.Size(96, 18);
            this.lblDestinadoAPremios.TabIndex = 470;
            this.lblDestinadoAPremios.Tag = "";
            this.lblDestinadoAPremios.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label28
            // 
            this.label28.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label28.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label28.Location = new System.Drawing.Point(364, 28);
            this.label28.Name = "label28";
            this.label28.Size = new System.Drawing.Size(125, 20);
            this.label28.TabIndex = 469;
            this.label28.Text = "Destinado a premios";
            this.label28.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txPorcentajeDestinadoAPremios
            // 
            this.txPorcentajeDestinadoAPremios.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txPorcentajeDestinadoAPremios.Enabled = false;
            this.txPorcentajeDestinadoAPremios.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txPorcentajeDestinadoAPremios.ForeColor = System.Drawing.Color.SaddleBrown;
            this.txPorcentajeDestinadoAPremios.Location = new System.Drawing.Point(495, 28);
            this.txPorcentajeDestinadoAPremios.MaxLength = 2;
            this.txPorcentajeDestinadoAPremios.Name = "txPorcentajeDestinadoAPremios";
            this.txPorcentajeDestinadoAPremios.Size = new System.Drawing.Size(24, 21);
            this.txPorcentajeDestinadoAPremios.TabIndex = 471;
            this.txPorcentajeDestinadoAPremios.Text = "55";
            this.txPorcentajeDestinadoAPremios.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txPorcentajeDestinadoAPremios.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            this.txPorcentajeDestinadoAPremios.TextChanged += new System.EventHandler(this.GenericLAE_TextChanged);
            // 
            // txPorcentajeParaEl14
            // 
            this.txPorcentajeParaEl14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txPorcentajeParaEl14.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txPorcentajeParaEl14.ForeColor = System.Drawing.Color.SaddleBrown;
            this.txPorcentajeParaEl14.Location = new System.Drawing.Point(495, 52);
            this.txPorcentajeParaEl14.MaxLength = 2;
            this.txPorcentajeParaEl14.Name = "txPorcentajeParaEl14";
            this.txPorcentajeParaEl14.Size = new System.Drawing.Size(24, 21);
            this.txPorcentajeParaEl14.TabIndex = 474;
            this.txPorcentajeParaEl14.Tag = "0";
            this.txPorcentajeParaEl14.Text = "12";
            this.txPorcentajeParaEl14.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txPorcentajeParaEl14.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            this.txPorcentajeParaEl14.TextChanged += new System.EventHandler(this.GenericLAE_TextChanged);
            // 
            // lblParaEl14
            // 
            this.lblParaEl14.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblParaEl14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblParaEl14.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblParaEl14.ForeColor = System.Drawing.Color.Brown;
            this.lblParaEl14.Location = new System.Drawing.Point(536, 52);
            this.lblParaEl14.Name = "lblParaEl14";
            this.lblParaEl14.Size = new System.Drawing.Size(96, 18);
            this.lblParaEl14.TabIndex = 473;
            this.lblParaEl14.Tag = "";
            this.lblParaEl14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label30
            // 
            this.label30.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label30.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label30.Location = new System.Drawing.Point(388, 52);
            this.label30.Name = "label30";
            this.label30.Size = new System.Drawing.Size(101, 21);
            this.label30.TabIndex = 472;
            this.label30.Text = "Para 14 aciertos";
            this.label30.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txPorcentajeParaEl13
            // 
            this.txPorcentajeParaEl13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txPorcentajeParaEl13.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txPorcentajeParaEl13.ForeColor = System.Drawing.Color.SaddleBrown;
            this.txPorcentajeParaEl13.Location = new System.Drawing.Point(495, 76);
            this.txPorcentajeParaEl13.MaxLength = 2;
            this.txPorcentajeParaEl13.Name = "txPorcentajeParaEl13";
            this.txPorcentajeParaEl13.Size = new System.Drawing.Size(24, 21);
            this.txPorcentajeParaEl13.TabIndex = 477;
            this.txPorcentajeParaEl13.Tag = "1";
            this.txPorcentajeParaEl13.Text = "8";
            this.txPorcentajeParaEl13.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txPorcentajeParaEl13.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            this.txPorcentajeParaEl13.TextChanged += new System.EventHandler(this.GenericLAE_TextChanged);
            // 
            // lblParaEl13
            // 
            this.lblParaEl13.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblParaEl13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblParaEl13.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblParaEl13.ForeColor = System.Drawing.Color.Brown;
            this.lblParaEl13.Location = new System.Drawing.Point(536, 76);
            this.lblParaEl13.Name = "lblParaEl13";
            this.lblParaEl13.Size = new System.Drawing.Size(96, 18);
            this.lblParaEl13.TabIndex = 476;
            this.lblParaEl13.Tag = "";
            this.lblParaEl13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label32
            // 
            this.label32.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label32.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label32.Location = new System.Drawing.Point(388, 76);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(101, 21);
            this.label32.TabIndex = 475;
            this.label32.Text = "Para 13 aciertos";
            this.label32.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txPorcentajeParaEl12
            // 
            this.txPorcentajeParaEl12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txPorcentajeParaEl12.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txPorcentajeParaEl12.ForeColor = System.Drawing.Color.SaddleBrown;
            this.txPorcentajeParaEl12.Location = new System.Drawing.Point(495, 100);
            this.txPorcentajeParaEl12.MaxLength = 2;
            this.txPorcentajeParaEl12.Name = "txPorcentajeParaEl12";
            this.txPorcentajeParaEl12.Size = new System.Drawing.Size(24, 21);
            this.txPorcentajeParaEl12.TabIndex = 480;
            this.txPorcentajeParaEl12.Tag = "2";
            this.txPorcentajeParaEl12.Text = "8";
            this.txPorcentajeParaEl12.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txPorcentajeParaEl12.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            this.txPorcentajeParaEl12.TextChanged += new System.EventHandler(this.GenericLAE_TextChanged);
            // 
            // lblParaEl12
            // 
            this.lblParaEl12.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblParaEl12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblParaEl12.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblParaEl12.ForeColor = System.Drawing.Color.Brown;
            this.lblParaEl12.Location = new System.Drawing.Point(536, 100);
            this.lblParaEl12.Name = "lblParaEl12";
            this.lblParaEl12.Size = new System.Drawing.Size(96, 18);
            this.lblParaEl12.TabIndex = 479;
            this.lblParaEl12.Tag = "";
            this.lblParaEl12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label34
            // 
            this.label34.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label34.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label34.Location = new System.Drawing.Point(388, 100);
            this.label34.Name = "label34";
            this.label34.Size = new System.Drawing.Size(101, 21);
            this.label34.TabIndex = 478;
            this.label34.Text = "Para 12 aciertos";
            this.label34.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txPorcentajeParaEl11
            // 
            this.txPorcentajeParaEl11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txPorcentajeParaEl11.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txPorcentajeParaEl11.ForeColor = System.Drawing.Color.SaddleBrown;
            this.txPorcentajeParaEl11.Location = new System.Drawing.Point(495, 124);
            this.txPorcentajeParaEl11.MaxLength = 2;
            this.txPorcentajeParaEl11.Name = "txPorcentajeParaEl11";
            this.txPorcentajeParaEl11.Size = new System.Drawing.Size(24, 21);
            this.txPorcentajeParaEl11.TabIndex = 483;
            this.txPorcentajeParaEl11.Tag = "3";
            this.txPorcentajeParaEl11.Text = "8";
            this.txPorcentajeParaEl11.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txPorcentajeParaEl11.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            this.txPorcentajeParaEl11.TextChanged += new System.EventHandler(this.GenericLAE_TextChanged);
            // 
            // lblParaEl11
            // 
            this.lblParaEl11.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblParaEl11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblParaEl11.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblParaEl11.ForeColor = System.Drawing.Color.Brown;
            this.lblParaEl11.Location = new System.Drawing.Point(536, 124);
            this.lblParaEl11.Name = "lblParaEl11";
            this.lblParaEl11.Size = new System.Drawing.Size(96, 18);
            this.lblParaEl11.TabIndex = 482;
            this.lblParaEl11.Tag = "";
            this.lblParaEl11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label36
            // 
            this.label36.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label36.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label36.Location = new System.Drawing.Point(388, 124);
            this.label36.Name = "label36";
            this.label36.Size = new System.Drawing.Size(101, 21);
            this.label36.TabIndex = 481;
            this.label36.Text = "Para 11 aciertos";
            this.label36.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txPorcentajeParaEl10
            // 
            this.txPorcentajeParaEl10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txPorcentajeParaEl10.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txPorcentajeParaEl10.ForeColor = System.Drawing.Color.SaddleBrown;
            this.txPorcentajeParaEl10.Location = new System.Drawing.Point(495, 148);
            this.txPorcentajeParaEl10.MaxLength = 2;
            this.txPorcentajeParaEl10.Name = "txPorcentajeParaEl10";
            this.txPorcentajeParaEl10.Size = new System.Drawing.Size(24, 21);
            this.txPorcentajeParaEl10.TabIndex = 486;
            this.txPorcentajeParaEl10.Tag = "4";
            this.txPorcentajeParaEl10.Text = "9";
            this.txPorcentajeParaEl10.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txPorcentajeParaEl10.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            this.txPorcentajeParaEl10.TextChanged += new System.EventHandler(this.GenericLAE_TextChanged);
            // 
            // lblParaEl10
            // 
            this.lblParaEl10.BackColor = System.Drawing.Color.LemonChiffon;
            this.lblParaEl10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblParaEl10.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblParaEl10.ForeColor = System.Drawing.Color.Brown;
            this.lblParaEl10.Location = new System.Drawing.Point(536, 148);
            this.lblParaEl10.Name = "lblParaEl10";
            this.lblParaEl10.Size = new System.Drawing.Size(96, 18);
            this.lblParaEl10.TabIndex = 485;
            this.lblParaEl10.Tag = "";
            this.lblParaEl10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label38
            // 
            this.label38.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label38.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label38.Location = new System.Drawing.Point(388, 148);
            this.label38.Name = "label38";
            this.label38.Size = new System.Drawing.Size(101, 21);
            this.label38.TabIndex = 484;
            this.label38.Text = "Para 10 aciertos";
            this.label38.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txPrecioApuesta
            // 
            this.txPrecioApuesta.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txPrecioApuesta.ForeColor = System.Drawing.Color.SaddleBrown;
            this.txPrecioApuesta.Location = new System.Drawing.Point(320, 100);
            this.txPrecioApuesta.MaxLength = 3;
            this.txPrecioApuesta.Name = "txPrecioApuesta";
            this.txPrecioApuesta.Size = new System.Drawing.Size(28, 20);
            this.txPrecioApuesta.TabIndex = 488;
            this.txPrecioApuesta.Text = "0,5";
            this.txPrecioApuesta.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txPrecioApuesta.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            this.txPrecioApuesta.TextChanged += new System.EventHandler(this.GenericLAE_TextChanged);
            // 
            // label53
            // 
            this.label53.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label53.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label53.Location = new System.Drawing.Point(176, 100);
            this.label53.Name = "label53";
            this.label53.Size = new System.Drawing.Size(141, 20);
            this.label53.TabIndex = 487;
            this.label53.Text = "Precio apuesta";
            this.label53.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.DarkSalmon;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Image = ((System.Drawing.Image)(resources.GetObject("button1.Image")));
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.Location = new System.Drawing.Point(492, 380);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(128, 32);
            this.button1.TabIndex = 489;
            this.button1.Text = "Cancelar";
            this.button1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label54
            // 
            this.label54.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label54.Location = new System.Drawing.Point(348, 104);
            this.label54.Name = "label54";
            this.label54.Size = new System.Drawing.Size(16, 16);
            this.label54.TabIndex = 490;
            this.label54.Text = "";
            // 
            // label55
            // 
            this.label55.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label55.Location = new System.Drawing.Point(348, 32);
            this.label55.Name = "label55";
            this.label55.Size = new System.Drawing.Size(12, 16);
            this.label55.TabIndex = 491;
            this.label55.Text = "";
            // 
            // label56
            // 
            this.label56.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label56.Location = new System.Drawing.Point(348, 56);
            this.label56.Name = "label56";
            this.label56.Size = new System.Drawing.Size(12, 16);
            this.label56.TabIndex = 492;
            this.label56.Text = "";
            // 
            // label57
            // 
            this.label57.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label57.Location = new System.Drawing.Point(633, 28);
            this.label57.Name = "label57";
            this.label57.Size = new System.Drawing.Size(20, 16);
            this.label57.TabIndex = 493;
            this.label57.Text = "";
            // 
            // label58
            // 
            this.label58.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label58.Location = new System.Drawing.Point(633, 52);
            this.label58.Name = "label58";
            this.label58.Size = new System.Drawing.Size(20, 16);
            this.label58.TabIndex = 494;
            this.label58.Text = "";
            // 
            // label59
            // 
            this.label59.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label59.Location = new System.Drawing.Point(633, 76);
            this.label59.Name = "label59";
            this.label59.Size = new System.Drawing.Size(20, 16);
            this.label59.TabIndex = 495;
            this.label59.Text = "";
            // 
            // label60
            // 
            this.label60.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label60.Location = new System.Drawing.Point(633, 100);
            this.label60.Name = "label60";
            this.label60.Size = new System.Drawing.Size(20, 16);
            this.label60.TabIndex = 496;
            this.label60.Text = "";
            // 
            // label61
            // 
            this.label61.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label61.Location = new System.Drawing.Point(633, 124);
            this.label61.Name = "label61";
            this.label61.Size = new System.Drawing.Size(20, 16);
            this.label61.TabIndex = 497;
            this.label61.Text = "";
            // 
            // label62
            // 
            this.label62.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label62.Location = new System.Drawing.Point(633, 148);
            this.label62.Name = "label62";
            this.label62.Size = new System.Drawing.Size(20, 16);
            this.label62.TabIndex = 498;
            this.label62.Text = "";
            // 
            // statusBar1
            // 
            this.statusBar1.Location = new System.Drawing.Point(0, 459);
            this.statusBar1.Name = "statusBar1";
            this.statusBar1.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
            this.statusBarPanel1,
            this.statusBarPanel2});
            this.statusBar1.Size = new System.Drawing.Size(658, 22);
            this.statusBar1.TabIndex = 504;
            // 
            // statusBarPanel1
            // 
            this.statusBarPanel1.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.statusBarPanel1.BorderStyle = System.Windows.Forms.StatusBarPanelBorderStyle.Raised;
            this.statusBarPanel1.Name = "statusBarPanel1";
            this.statusBarPanel1.Text = "Fichero valoraciones";
            this.statusBarPanel1.Width = 119;
            // 
            // statusBarPanel2
            // 
            this.statusBarPanel2.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Contents;
            this.statusBarPanel2.Name = "statusBarPanel2";
            this.statusBarPanel2.Width = 10;
            // 
            // label83
            // 
            this.label83.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label83.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label83.Location = new System.Drawing.Point(541, 230);
            this.label83.Name = "label83";
            this.label83.Size = new System.Drawing.Size(80, 32);
            this.label83.TabIndex = 506;
            this.label83.Text = "Premio";
            this.label83.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label84
            // 
            this.label84.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label84.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label84.Location = new System.Drawing.Point(472, 230);
            this.label84.Name = "label84";
            this.label84.Size = new System.Drawing.Size(68, 32);
            this.label84.TabIndex = 505;
            this.label84.Text = "Acertantes";
            this.label84.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label85
            // 
            this.label85.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label85.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label85.Location = new System.Drawing.Point(268, 196);
            this.label85.Name = "label85";
            this.label85.Size = new System.Drawing.Size(176, 28);
            this.label85.TabIndex = 522;
            this.label85.Text = "Previsión de premios";
            this.label85.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label86
            // 
            this.label86.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label86.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label86.Location = new System.Drawing.Point(504, 196);
            this.label86.Name = "label86";
            this.label86.Size = new System.Drawing.Size(116, 28);
            this.label86.TabIndex = 523;
            this.label86.Text = "Escrutinio real";
            this.label86.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txTemporada
            // 
            this.txTemporada.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txTemporada.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txTemporada.ForeColor = System.Drawing.Color.SaddleBrown;
            this.txTemporada.Location = new System.Drawing.Point(91, 28);
            this.txTemporada.MaxLength = 9;
            this.txTemporada.Name = "txTemporada";
            this.txTemporada.Size = new System.Drawing.Size(70, 20);
            this.txTemporada.TabIndex = 525;
            this.txTemporada.Text = "2004/2005";
            this.txTemporada.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txTemporada.TextChanged += new System.EventHandler(this.TemporadaJornada_TextChanged);
            // 
            // label87
            // 
            this.label87.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label87.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label87.Location = new System.Drawing.Point(12, 28);
            this.label87.Name = "label87";
            this.label87.Size = new System.Drawing.Size(76, 20);
            this.label87.TabIndex = 524;
            this.label87.Text = "Temporada";
            this.label87.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txJornada
            // 
            this.txJornada.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txJornada.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txJornada.ForeColor = System.Drawing.Color.SaddleBrown;
            this.txJornada.Location = new System.Drawing.Point(91, 52);
            this.txJornada.MaxLength = 2;
            this.txJornada.Name = "txJornada";
            this.txJornada.Size = new System.Drawing.Size(28, 20);
            this.txJornada.TabIndex = 527;
            this.txJornada.Text = "0";
            this.txJornada.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txJornada.TextChanged += new System.EventHandler(this.TemporadaJornada_TextChanged);
            // 
            // label88
            // 
            this.label88.BackColor = System.Drawing.Color.PaleGoldenrod;
            this.label88.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label88.Location = new System.Drawing.Point(12, 52);
            this.label88.Name = "label88";
            this.label88.Size = new System.Drawing.Size(76, 20);
            this.label88.TabIndex = 526;
            this.label88.Text = "Jornada";
            this.label88.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txAcertantes14
            // 
            this.txAcertantes14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txAcertantes14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txAcertantes14.ForeColor = System.Drawing.Color.SaddleBrown;
            this.txAcertantes14.Location = new System.Drawing.Point(472, 264);
            this.txAcertantes14.MaxLength = 10;
            this.txAcertantes14.Name = "txAcertantes14";
            this.txAcertantes14.Size = new System.Drawing.Size(68, 20);
            this.txAcertantes14.TabIndex = 528;
            this.txAcertantes14.Tag = "0";
            this.txAcertantes14.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txAcertantes14.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            this.txAcertantes14.TextChanged += new System.EventHandler(this.GenericNumAcertantes_TextChanged);
            // 
            // txPremio14
            // 
            this.txPremio14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txPremio14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txPremio14.ForeColor = System.Drawing.Color.SaddleBrown;
            this.txPremio14.Location = new System.Drawing.Point(541, 264);
            this.txPremio14.MaxLength = 10;
            this.txPremio14.Name = "txPremio14";
            this.txPremio14.Size = new System.Drawing.Size(80, 20);
            this.txPremio14.TabIndex = 529;
            this.txPremio14.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txPremio14.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            // 
            // txPremio13
            // 
            this.txPremio13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txPremio13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txPremio13.ForeColor = System.Drawing.Color.SaddleBrown;
            this.txPremio13.Location = new System.Drawing.Point(541, 285);
            this.txPremio13.MaxLength = 10;
            this.txPremio13.Name = "txPremio13";
            this.txPremio13.Size = new System.Drawing.Size(80, 20);
            this.txPremio13.TabIndex = 531;
            this.txPremio13.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txPremio13.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            // 
            // txAcertantes13
            // 
            this.txAcertantes13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txAcertantes13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txAcertantes13.ForeColor = System.Drawing.Color.SaddleBrown;
            this.txAcertantes13.Location = new System.Drawing.Point(472, 285);
            this.txAcertantes13.MaxLength = 10;
            this.txAcertantes13.Name = "txAcertantes13";
            this.txAcertantes13.Size = new System.Drawing.Size(68, 20);
            this.txAcertantes13.TabIndex = 530;
            this.txAcertantes13.Tag = "1";
            this.txAcertantes13.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txAcertantes13.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            this.txAcertantes13.TextChanged += new System.EventHandler(this.GenericNumAcertantes_TextChanged);
            // 
            // txPremio12
            // 
            this.txPremio12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txPremio12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txPremio12.ForeColor = System.Drawing.Color.SaddleBrown;
            this.txPremio12.Location = new System.Drawing.Point(541, 306);
            this.txPremio12.MaxLength = 10;
            this.txPremio12.Name = "txPremio12";
            this.txPremio12.Size = new System.Drawing.Size(80, 20);
            this.txPremio12.TabIndex = 533;
            this.txPremio12.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txPremio12.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            // 
            // txAcertantes12
            // 
            this.txAcertantes12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txAcertantes12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txAcertantes12.ForeColor = System.Drawing.Color.SaddleBrown;
            this.txAcertantes12.Location = new System.Drawing.Point(472, 306);
            this.txAcertantes12.MaxLength = 10;
            this.txAcertantes12.Name = "txAcertantes12";
            this.txAcertantes12.Size = new System.Drawing.Size(68, 20);
            this.txAcertantes12.TabIndex = 532;
            this.txAcertantes12.Tag = "2";
            this.txAcertantes12.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txAcertantes12.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            this.txAcertantes12.TextChanged += new System.EventHandler(this.GenericNumAcertantes_TextChanged);
            // 
            // txPremio11
            // 
            this.txPremio11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txPremio11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txPremio11.ForeColor = System.Drawing.Color.SaddleBrown;
            this.txPremio11.Location = new System.Drawing.Point(541, 327);
            this.txPremio11.MaxLength = 10;
            this.txPremio11.Name = "txPremio11";
            this.txPremio11.Size = new System.Drawing.Size(80, 20);
            this.txPremio11.TabIndex = 535;
            this.txPremio11.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txPremio11.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            // 
            // txAcertantes11
            // 
            this.txAcertantes11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txAcertantes11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txAcertantes11.ForeColor = System.Drawing.Color.SaddleBrown;
            this.txAcertantes11.Location = new System.Drawing.Point(472, 327);
            this.txAcertantes11.MaxLength = 10;
            this.txAcertantes11.Name = "txAcertantes11";
            this.txAcertantes11.Size = new System.Drawing.Size(68, 20);
            this.txAcertantes11.TabIndex = 534;
            this.txAcertantes11.Tag = "3";
            this.txAcertantes11.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txAcertantes11.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            this.txAcertantes11.TextChanged += new System.EventHandler(this.GenericNumAcertantes_TextChanged);
            // 
            // txPremio10
            // 
            this.txPremio10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txPremio10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txPremio10.ForeColor = System.Drawing.Color.SaddleBrown;
            this.txPremio10.Location = new System.Drawing.Point(541, 348);
            this.txPremio10.MaxLength = 10;
            this.txPremio10.Name = "txPremio10";
            this.txPremio10.Size = new System.Drawing.Size(80, 20);
            this.txPremio10.TabIndex = 537;
            this.txPremio10.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txPremio10.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            // 
            // txAcertantes10
            // 
            this.txAcertantes10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txAcertantes10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txAcertantes10.ForeColor = System.Drawing.Color.SaddleBrown;
            this.txAcertantes10.Location = new System.Drawing.Point(472, 348);
            this.txAcertantes10.MaxLength = 10;
            this.txAcertantes10.Name = "txAcertantes10";
            this.txAcertantes10.Size = new System.Drawing.Size(68, 20);
            this.txAcertantes10.TabIndex = 536;
            this.txAcertantes10.Tag = "4";
            this.txAcertantes10.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txAcertantes10.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBoxgenerico_KeyPress);
            this.txAcertantes10.TextChanged += new System.EventHandler(this.GenericNumAcertantes_TextChanged);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.DarkSalmon;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Image = ((System.Drawing.Image)(resources.GetObject("button2.Image")));
            this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button2.Location = new System.Drawing.Point(340, 380);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(128, 32);
            this.button2.TabIndex = 538;
            this.button2.Text = "Calcular";
            this.button2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // btJornadaAnterior
            // 
            this.btJornadaAnterior.BackColor = System.Drawing.Color.LightSalmon;
            this.btJornadaAnterior.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btJornadaAnterior.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btJornadaAnterior.ForeColor = System.Drawing.SystemColors.WindowText;
            this.btJornadaAnterior.Image = ((System.Drawing.Image)(resources.GetObject("btJornadaAnterior.Image")));
            this.btJornadaAnterior.Location = new System.Drawing.Point(120, 52);
            this.btJornadaAnterior.Name = "btJornadaAnterior";
            this.btJornadaAnterior.Size = new System.Drawing.Size(20, 20);
            this.btJornadaAnterior.TabIndex = 539;
            this.btJornadaAnterior.UseVisualStyleBackColor = false;
            this.btJornadaAnterior.Click += new System.EventHandler(this.btJornadaAnterior_Click);
            // 
            // btJornadaPosterior
            // 
            this.btJornadaPosterior.BackColor = System.Drawing.Color.LightSalmon;
            this.btJornadaPosterior.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btJornadaPosterior.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btJornadaPosterior.ForeColor = System.Drawing.SystemColors.WindowText;
            this.btJornadaPosterior.Image = ((System.Drawing.Image)(resources.GetObject("btJornadaPosterior.Image")));
            this.btJornadaPosterior.Location = new System.Drawing.Point(141, 52);
            this.btJornadaPosterior.Name = "btJornadaPosterior";
            this.btJornadaPosterior.Size = new System.Drawing.Size(20, 20);
            this.btJornadaPosterior.TabIndex = 540;
            this.btJornadaPosterior.UseVisualStyleBackColor = false;
            this.btJornadaPosterior.Click += new System.EventHandler(this.btJornadaPosterior_Click);
            // 
            // label9
            // 
            this.label9.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.Location = new System.Drawing.Point(519, 32);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(15, 16);
            this.label9.TabIndex = 541;
            this.label9.Text = "%";
            // 
            // label10
            // 
            this.label10.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(519, 56);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(15, 16);
            this.label10.TabIndex = 542;
            this.label10.Text = "%";
            // 
            // label11
            // 
            this.label11.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(519, 80);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(15, 16);
            this.label11.TabIndex = 543;
            this.label11.Text = "%";
            // 
            // label12
            // 
            this.label12.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(519, 104);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(15, 16);
            this.label12.TabIndex = 544;
            this.label12.Text = "%";
            // 
            // label13
            // 
            this.label13.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.Location = new System.Drawing.Point(519, 128);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(15, 16);
            this.label13.TabIndex = 545;
            this.label13.Text = "%";
            // 
            // label14
            // 
            this.label14.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.Location = new System.Drawing.Point(519, 152);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(15, 16);
            this.label14.TabIndex = 546;
            this.label14.Text = "%";
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(622, 348);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(20, 16);
            this.label15.TabIndex = 551;
            this.label15.Text = "";
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(622, 327);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(20, 16);
            this.label16.TabIndex = 550;
            this.label16.Text = "";
            // 
            // label17
            // 
            this.label17.Location = new System.Drawing.Point(622, 310);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(20, 16);
            this.label17.TabIndex = 549;
            this.label17.Text = "";
            // 
            // label18
            // 
            this.label18.Location = new System.Drawing.Point(622, 289);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(20, 16);
            this.label18.TabIndex = 548;
            this.label18.Text = "";
            // 
            // label23
            // 
            this.label23.Location = new System.Drawing.Point(622, 268);
            this.label23.Name = "label23";
            this.label23.Size = new System.Drawing.Size(20, 16);
            this.label23.TabIndex = 547;
            this.label23.Text = "";
            // 
            // label24
            // 
            this.label24.Location = new System.Drawing.Point(445, 344);
            this.label24.Name = "label24";
            this.label24.Size = new System.Drawing.Size(20, 16);
            this.label24.TabIndex = 556;
            this.label24.Text = "";
            // 
            // label29
            // 
            this.label29.Location = new System.Drawing.Point(445, 324);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(20, 16);
            this.label29.TabIndex = 555;
            this.label29.Text = "";
            // 
            // label31
            // 
            this.label31.Location = new System.Drawing.Point(445, 308);
            this.label31.Name = "label31";
            this.label31.Size = new System.Drawing.Size(20, 16);
            this.label31.TabIndex = 554;
            this.label31.Text = "";
            // 
            // label33
            // 
            this.label33.Location = new System.Drawing.Point(445, 288);
            this.label33.Name = "label33";
            this.label33.Size = new System.Drawing.Size(20, 16);
            this.label33.TabIndex = 553;
            this.label33.Text = "";
            // 
            // label35
            // 
            this.label35.Location = new System.Drawing.Point(445, 268);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(20, 16);
            this.label35.TabIndex = 552;
            this.label35.Text = "";
            // 
            // btCopiar
            // 
            this.btCopiar.BackColor = System.Drawing.Color.LightSalmon;
            this.btCopiar.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btCopiar.Image = ((System.Drawing.Image)(resources.GetObject("btCopiar.Image")));
            this.btCopiar.Location = new System.Drawing.Point(236, 196);
            this.btCopiar.Name = "btCopiar";
            this.btCopiar.Size = new System.Drawing.Size(28, 28);
            this.btCopiar.TabIndex = 559;
            this.btCopiar.UseVisualStyleBackColor = false;
            this.btCopiar.Click += new System.EventHandler(this.btCopiar_Click);
            // 
            // linkLAE
            // 
            this.linkLAE.Location = new System.Drawing.Point(240, 152);
            this.linkLAE.Name = "linkLAE";
            this.linkLAE.Size = new System.Drawing.Size(148, 20);
            this.linkLAE.TabIndex = 560;
            this.linkLAE.TabStop = true;
            this.linkLAE.Text = "Resultados escrutinio oficial";
            this.linkLAE.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLAE_LinkClicked);
            // 
            // btGuardarEscrutinio
            // 
            this.btGuardarEscrutinio.BackColor = System.Drawing.Color.LightSalmon;
            this.btGuardarEscrutinio.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btGuardarEscrutinio.Image = ((System.Drawing.Image)(resources.GetObject("btGuardarEscrutinio.Image")));
            this.btGuardarEscrutinio.Location = new System.Drawing.Point(472, 196);
            this.btGuardarEscrutinio.Name = "btGuardarEscrutinio";
            this.btGuardarEscrutinio.Size = new System.Drawing.Size(28, 28);
            this.btGuardarEscrutinio.TabIndex = 561;
            this.btGuardarEscrutinio.UseVisualStyleBackColor = false;
            this.btGuardarEscrutinio.Click += new System.EventHandler(this.btGuardarEscrutinio_Click);
            // 
            // controlPorcentajes1
            // 
            this.controlPorcentajes1.archivoPorcentajes = null;
            this.controlPorcentajes1.BackColor = System.Drawing.Color.Bisque;
            this.controlPorcentajes1.CaptionText = "  P O R C E N T A J E S";
            this.controlPorcentajes1.FormatoFicheroValoraciones = ((short)(0));
            this.controlPorcentajes1.Jornada = "01";
            this.controlPorcentajes1.Location = new System.Drawing.Point(4, 80);
            this.controlPorcentajes1.Name = "controlPorcentajes1";
            this.controlPorcentajes1.ReadOnly = false;
            this.controlPorcentajes1.Size = new System.Drawing.Size(160, 373);
            this.controlPorcentajes1.TabIndex = 563;
            this.controlPorcentajes1.Temporada = "2004/2005";
            this.controlPorcentajes1.Modificado += new System.EventHandler(this.controlPorcentajes1_Modificado);
            // 
            // EstimadorPremiosFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(658, 481);
            this.Controls.Add(this.controlPorcentajes1);
            this.Controls.Add(this.btGuardarEscrutinio);
            this.Controls.Add(this.linkLAE);
            this.Controls.Add(this.btCopiar);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.label29);
            this.Controls.Add(this.label31);
            this.Controls.Add(this.label33);
            this.Controls.Add(this.label35);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.btJornadaPosterior);
            this.Controls.Add(this.btJornadaAnterior);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.txPremio10);
            this.Controls.Add(this.txAcertantes10);
            this.Controls.Add(this.txPremio11);
            this.Controls.Add(this.txAcertantes11);
            this.Controls.Add(this.txPremio12);
            this.Controls.Add(this.txAcertantes12);
            this.Controls.Add(this.txPremio13);
            this.Controls.Add(this.txAcertantes13);
            this.Controls.Add(this.txPremio14);
            this.Controls.Add(this.txAcertantes14);
            this.Controls.Add(this.txJornada);
            this.Controls.Add(this.label88);
            this.Controls.Add(this.txTemporada);
            this.Controls.Add(this.label87);
            this.Controls.Add(this.label86);
            this.Controls.Add(this.label85);
            this.Controls.Add(this.label83);
            this.Controls.Add(this.label84);
            this.Controls.Add(this.statusBar1);
            this.Controls.Add(this.label62);
            this.Controls.Add(this.label61);
            this.Controls.Add(this.label60);
            this.Controls.Add(this.label59);
            this.Controls.Add(this.label58);
            this.Controls.Add(this.label57);
            this.Controls.Add(this.label56);
            this.Controls.Add(this.label55);
            this.Controls.Add(this.label54);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txPrecioApuesta);
            this.Controls.Add(this.label53);
            this.Controls.Add(this.txPorcentajeParaEl10);
            this.Controls.Add(this.lblParaEl10);
            this.Controls.Add(this.label38);
            this.Controls.Add(this.txPorcentajeParaEl11);
            this.Controls.Add(this.lblParaEl11);
            this.Controls.Add(this.label36);
            this.Controls.Add(this.txPorcentajeParaEl12);
            this.Controls.Add(this.lblParaEl12);
            this.Controls.Add(this.label34);
            this.Controls.Add(this.txPorcentajeParaEl13);
            this.Controls.Add(this.lblParaEl13);
            this.Controls.Add(this.label32);
            this.Controls.Add(this.txPorcentajeParaEl14);
            this.Controls.Add(this.lblParaEl14);
            this.Controls.Add(this.label30);
            this.Controls.Add(this.txPorcentajeDestinadoAPremios);
            this.Controls.Add(this.lblDestinadoAPremios);
            this.Controls.Add(this.label28);
            this.Controls.Add(this.lbNumApuestas);
            this.Controls.Add(this.label22);
            this.Controls.Add(this.txBote);
            this.Controls.Add(this.label21);
            this.Controls.Add(this.txRecaudacion);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.lblPrevPremio10);
            this.Controls.Add(this.lblPrevAcertantes10);
            this.Controls.Add(this.lblPrevPremio11);
            this.Controls.Add(this.lblPrevAcertantes11);
            this.Controls.Add(this.lblPrevPremio12);
            this.Controls.Add(this.lblPrevAcertantes12);
            this.Controls.Add(this.lblPrevPremio13);
            this.Controls.Add(this.lblPrevAcertantes13);
            this.Controls.Add(this.lblPrevPremio14);
            this.Controls.Add(this.lblPrevAcertantes14);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblNewBase141);
            this.Controls.Add(this.lblNewBase131);
            this.Controls.Add(this.lblNewBase121);
            this.Controls.Add(this.lblNewBase111);
            this.Controls.Add(this.lblNewBase101);
            this.Controls.Add(this.lblNewBase91);
            this.Controls.Add(this.lblNewBase81);
            this.Controls.Add(this.lblNewBase71);
            this.Controls.Add(this.lblNewBase61);
            this.Controls.Add(this.lblNewBase51);
            this.Controls.Add(this.lblNewBase41);
            this.Controls.Add(this.lblNewBase31);
            this.Controls.Add(this.lblNewBase21);
            this.Controls.Add(this.lblNewBase11);
            this.MinimumSize = new System.Drawing.Size(652, 476);
            this.Name = "EstimadorPremiosFrm";
            this.Text = "Estimación de premios";
            // StatusBarPanel controls don't support ISupportInitialize in .NET 8
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).EndInit();
            // ((System.ComponentModel.ISupportInitialize)(this.statusBarPanel2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion


		private void textBoxgenerico_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
		{
			Valida.Generic_KeyPress (TipoValidacionString.SoloNumerosDecimales, sender, e);
		}
		private void GenericLabel_Click(object sender, System.EventArgs e)
		{
			Label MiLabel =(Label)sender;
			switch (MiLabel.Text)
			{
				case "1" :
				{
					MiLabel.Text="X";
					Signos [Convert.ToInt16 (MiLabel.Tag )]=1;
					break;
				}
				case "X" :
				{
					MiLabel.Text="2";
					Signos [Convert.ToInt16 (MiLabel.Tag )]=2;
					break;
				}
				case "2":
				{
					MiLabel.Text="1";
					Signos [Convert.ToInt16 (MiLabel.Tag )]=0;
					break;
				}
			}
			CalcularPremios();

		}

		private void button2_Click(object sender, System.EventArgs e)
		{
			CalcularPremios();
		}
		private void CalcularPremios()
		{
			int Partido;
			int i;

			//---Guardar valores L.A.E. --------------
			Recaudacion =Convert.ToDouble (txRecaudacion.Text);
			PrecioApuesta =Convert.ToDouble (txPrecioApuesta.Text );
			Bote =Convert.ToDouble (txBote.Text );
			PctDestinadoAPremiosCategoria[0] =Convert.ToDouble (txPorcentajeParaEl14.Text)/100;
			PctDestinadoAPremiosCategoria[1] =Convert.ToDouble (txPorcentajeParaEl13.Text)/100;
			PctDestinadoAPremiosCategoria[2] =Convert.ToDouble (txPorcentajeParaEl12.Text)/100;
			PctDestinadoAPremiosCategoria[3] =Convert.ToDouble (txPorcentajeParaEl11.Text)/100;
			PctDestinadoAPremiosCategoria[4] =Convert.ToDouble (txPorcentajeParaEl10.Text)/100;
			AConfiguracion ac = new AConfiguracion(Application.StartupPath);
			ac.GuardarValoresLAE(PrecioApuesta, PctDestinadoAPremiosCategoria[0]*100, Recaudacion, moneda);

			for (i=0 ;i<5;i++)
			{
				DestinadoAPremiosCategoria[i]=Recaudacion*PctDestinadoAPremiosCategoria[i];
			}
			DestinadoAPremiosCategoria[0] += Bote;

			//---Leer Porcentajes --------------
			v=controlPorcentajes1.Valores;
			//PonerValoracionEnVariables();
			Porcentajes Pct = new Porcentajes(v);
			p= Pct.ValoresBase100();

			//--- Leer datos de los textBox -------
			NumApuestas=Convert.ToInt32  (lbNumApuestas.Text );

			//'---- probabilidad del 14 de la 1ª apuesta i 
			// cálculo de valores complementarios --------

			ProbabilidadCategoria14=1;
			PonerSignosEnVariables();
			for(i=0;i<5;i++)Acertantes[i,0]=0;

			for (Partido = 0;Partido<14; Partido++)
			{
				ProbabilidadCategoria14 *=p[Partido, Signos [Partido]];
				Cr[Partido] = (1-p[Partido, Signos [Partido]])/p[Partido, Signos [Partido]];
			}
			Acertantes [0,0]= NumApuestas*ProbabilidadCategoria14;
			Premios[0,0]=Math.Round (DestinadoAPremiosCategoria[0]/Acertantes[0,0],2);

			CalcularAcertantes (Acertantes [0,0],0,4);

			for (i=1; i<5;i++)
			{
				Premios[i,0]=Math.Round (DestinadoAPremiosCategoria[i]/Acertantes[i,0],2);
				Acertantes[i,0]= Math.Round (Acertantes[i,0],0);
			}
			Acertantes [0,0]= Math.Round (Acertantes [0,0],1);
			CorreccionesDeCalculo();
			MostrarResultados();
		}
		private void PonerSignosEnVariables()
		{
			Signos [0]="1X2".IndexOf (lblNewBase11.Text);
			Signos [1]="1X2".IndexOf(lblNewBase21.Text );
			Signos [2]="1X2".IndexOf(lblNewBase31.Text );
			Signos [3]="1X2".IndexOf(lblNewBase41.Text );
			Signos [4]="1X2".IndexOf(lblNewBase51.Text );
			Signos [5]="1X2".IndexOf(lblNewBase61.Text );
			Signos [6]="1X2".IndexOf(lblNewBase71.Text );
			Signos [7]="1X2".IndexOf(lblNewBase81.Text );
			Signos [8]="1X2".IndexOf(lblNewBase91.Text );
			Signos [9]="1X2".IndexOf(lblNewBase101.Text );
			Signos [10]="1X2".IndexOf(lblNewBase111.Text );
			Signos [11]="1X2".IndexOf(lblNewBase121.Text );
			Signos [12]="1X2".IndexOf(lblNewBase131.Text );
			Signos [13]="1X2".IndexOf(lblNewBase141.Text );


		}
		private void CorreccionesDeCalculo()
		{
			if(Premios[0,0] > DestinadoAPremiosCategoria[0])Premios[0,0] = DestinadoAPremiosCategoria[0];

			//Aplicación de la nueva norma: los de 10 no cobran cuando el premio es < 1
			//             y no se modifica la cantidad a repartir en la categ. superior
			if(Premios[4,0]<1)Premios[4,0]=0;
			
		}
		private void MostrarResultados()
		{
			lblPrevAcertantes14.Text =Acertantes[0,0].ToString ();
			lblPrevAcertantes13.Text =Acertantes[1,0].ToString ();
			lblPrevAcertantes12.Text =Acertantes[2,0].ToString ();
			lblPrevAcertantes11.Text =Acertantes[3,0].ToString ();
			lblPrevAcertantes10.Text =Acertantes[4,0].ToString ();
			lblPrevPremio14.Text =Premios[0,0].ToString ();
			lblPrevPremio13.Text =Premios[1,0].ToString ();
			lblPrevPremio12.Text =Premios[2,0].ToString ();
			lblPrevPremio11.Text =Premios[3,0].ToString ();
			lblPrevPremio10.Text =Premios[4,0].ToString ();
		}
		private void CalcularAcertantes(double pProb, int PosicionInicial,int pProfundidad)
		{
			double Prob;
			Profundidad++;
       
			for (int Partido = PosicionInicial;Partido<14;Partido++)
			{
				Prob = pProb * Cr[Partido];
				Acertantes[Profundidad,0] +=Prob;
				if (Profundidad < pProfundidad)
				{
					CalcularAcertantes (Prob, Partido + 1, pProfundidad);
				}
			}
			Profundidad--;
		}

		private void GenericLabel_DoubleClick(object sender, System.EventArgs e)
		{
			GenericLabel_Click( sender, e);
		}
		private void TemporadaJornada_TextChanged(object sender, System.EventArgs e)
		{
		    string NombreFicheroJornadas=Application.StartupPath + "/Jornadas/InfoJornadasLAE.txt";
            IArchivoColumnas comBaseCols = new ArchivoColumnasTexto(NombreFicheroJornadas);
			while( comBaseCols.SiguienteColumna() )
			{
				ValorsJornada=  comBaseCols.LeeColumnaSinComas().Split ((char) 9);
				if(ValorsJornada[1]==txTemporada.Text  && ValorsJornada[2]==txJornada.Text.PadLeft (2,'0')) 
				{
					MostrarColumna (ValorsJornada[0]);
					//----------------------------------------------
					//Mostramos los premios reales de cada categoria
					//----------------------------------------------
					if (ValorsJornada[4]=="0,00") 
					{
						//no hubo acertantes: calculamos lo que hubiera cobrado 1
						double ParaEl14=PrecioApuesta*Convert.ToDouble (ValorsJornada[3]) *PctDestinadoAPremiosCategoria[0] ;
						txPremio14.Text =ParaEl14.ToString();
					}
					else
					{
						txPremio14.Text =(PrecioApuesta*Convert.ToDouble (ValorsJornada[4])).ToString ();
					}
					txRecaudacion.Text =(PrecioApuesta*Convert.ToDouble (ValorsJornada[3])).ToString ();
					txPremio13.Text =(PrecioApuesta*Convert.ToDouble (ValorsJornada[5])).ToString ();
					txPremio12.Text =(PrecioApuesta*Convert.ToDouble (ValorsJornada[6])).ToString ();
					txPremio11.Text =(PrecioApuesta*Convert.ToDouble (ValorsJornada[7])).ToString ();
					txPremio10.Text =(PrecioApuesta*Convert.ToDouble (ValorsJornada[8])).ToString ();
					//--------------------------
					//Obtenemos el nº acertantes
					//--------------------------
					for (int i=0;i<5;i++)
					{
						Acertantes[i,1]=(float) (Math.Round (DestinadoAPremiosCategoria[i]/Convert.ToDouble (ValorsJornada[i+4])/PrecioApuesta,0));
					}
					if(Convert.ToDouble (ValorsJornada[4])==0)
					{
						txAcertantes14.Text = "0";
					}
					else
					{
						txAcertantes14.Text =Acertantes[0,1].ToString ();
					}
					//--------------------------
					//Mostramos el nº acertantes
					//--------------------------
					txAcertantes13.Text =Acertantes[1,1].ToString ();
					txAcertantes12.Text =Acertantes[2,1].ToString ();
					txAcertantes11.Text =Acertantes[3,1].ToString ();
					txAcertantes10.Text =Acertantes[4,1].ToString ();

					//--------------------------
					//Adaptamos a la nueva norma
					//--------------------------
					if(PrecioApuesta*Convert.ToDouble (ValorsJornada[8])<1)
					{
						txPremio10.Text ="0.00";
					}

					//--------------------------
					//Actualizamos valoraciones
					//--------------------------
					if (controlPorcentajes1.FormatoFicheroValoraciones ==44)
					{
						CargarValoracionJornadaDesdeHistorico(ValorsJornada[1], ValorsJornada[2]);
					}
					break;
				}
			}
			comBaseCols.Cerrar();		
		}
		private void CargarValoracionJornadaDesdeHistorico(string tempo, string jorna)
		{
			controlPorcentajes1.Jornada =jorna;
			controlPorcentajes1.Temporada =tempo;
			controlPorcentajes1.Refresca();
			v=controlPorcentajes1.Valores;
		}

		private void MostrarColumna (string columna)
		{
			lblNewBase11.Text =columna[0].ToString ();
			lblNewBase21.Text =columna[1].ToString ();
			lblNewBase31.Text =columna[2].ToString ();
			lblNewBase41.Text =columna[3].ToString ();
			lblNewBase51.Text =columna[4].ToString ();
			lblNewBase61.Text =columna[5].ToString ();
			lblNewBase71.Text =columna[6].ToString ();
			lblNewBase81.Text =columna[7].ToString ();
			lblNewBase91.Text =columna[8].ToString ();
			lblNewBase101.Text =columna[9].ToString ();
			lblNewBase111.Text =columna[10].ToString ();
			lblNewBase121.Text =columna[11].ToString ();
			lblNewBase131.Text =columna[12].ToString ();
			lblNewBase141.Text =columna[13].ToString ();
		}

		private void GenericLAE_TextChanged(object sender, System.EventArgs e)
		{
			TextBox MiTextBox =(TextBox)sender;
			if (MiTextBox.Text !="")
			{
				switch (MiTextBox.Name)
				{
					case "txRecaudacion" :
					{
						Recaudacion =Convert.ToDouble (MiTextBox.Text);
						break;
					}
					case "txBote" :
					{
						Bote =Convert.ToDouble (MiTextBox.Text);
						break;
					}
					case "txPrecioApuesta":
					{
						PrecioApuesta  =Convert.ToDouble (MiTextBox.Text);
						break;
					}
					case "txPorcentajeParaEl14":
					case "txPorcentajeParaEl13":
					case "txPorcentajeParaEl12":
					case "txPorcentajeParaEl11":
					case "txPorcentajeParaEl10":
					{
						int indice=Convert.ToInt16 (MiTextBox.Tag );
						PctDestinadoAPremiosCategoria[indice] =Convert.ToDouble (MiTextBox.Text);
						break;
					}
				}
				NumApuestas =(int) (Recaudacion/PrecioApuesta);
				lbNumApuestas.Text =NumApuestas.ToString ();
				CalcularImportesDestinadosAPremios();
			}
		}
		private void CalcularImportesDestinadosAPremios()
		{
			double PctTotal =0;
			double TotalParaPremios=0;

			DestinadoAPremiosCategoria[0]=Math.Round (Recaudacion*PctDestinadoAPremiosCategoria[0]+Bote+0.005,2);
			DestinadoAPremiosCategoria[1]=Math.Round (Recaudacion*PctDestinadoAPremiosCategoria[1]+0.005,2);
			DestinadoAPremiosCategoria[2]=Math.Round (Recaudacion*PctDestinadoAPremiosCategoria[2]+0.005,2);
			DestinadoAPremiosCategoria[3]=Math.Round (Recaudacion*PctDestinadoAPremiosCategoria[3]+0.005,2);
			DestinadoAPremiosCategoria[4]=Math.Round (Recaudacion*PctDestinadoAPremiosCategoria[4]+0.005,2);
			lblParaEl14.Text =DestinadoAPremiosCategoria[0].ToString ();
			lblParaEl13.Text =DestinadoAPremiosCategoria[1].ToString ();
			lblParaEl12.Text =DestinadoAPremiosCategoria[2].ToString ();
			lblParaEl11.Text =DestinadoAPremiosCategoria[3].ToString ();
			lblParaEl10.Text =DestinadoAPremiosCategoria[4].ToString ();

			for (int i=0;i<5;i++)PctTotal += PctDestinadoAPremiosCategoria[i];
			TotalParaPremios=Recaudacion*PctTotal;
			lblDestinadoAPremios.Text =TotalParaPremios.ToString ();
		}

		private void btJornadaAnterior_Click(object sender, System.EventArgs e)
		{
			int Jornada=Convert.ToInt16 (txJornada .Text );
			if (Jornada>1) Jornada--;
			txJornada.Text =Jornada.ToString ();
		}

		private void btJornadaPosterior_Click(object sender, System.EventArgs e)
		{
			int Jornada=Convert.ToInt16 (txJornada .Text );
			if (Jornada<44) Jornada++;
			txJornada.Text =Jornada.ToString ();

		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			Close();
		}

		private void pictureBox1_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			Cursor.Current = Cursors.Hand ;
		}

		private void pictureBox1_Click(object sender, System.EventArgs e)
		{
			System.Diagnostics.Process.Start("IExplore"," http://onlae.terra.es/qsorteos.asp");
		}

		private void GenericNumAcertantes_TextChanged(object sender, System.EventArgs e)
		{
			TextBox MiTextBox =(TextBox)sender;
			if (MiTextBox.Text !="")
			{
				int indice = Convert.ToInt16 (MiTextBox.Tag);
				Acertantes [indice,1]=(float) Convert.ToDouble (MiTextBox.Text);
				Premios [indice,1] = Math.Round (DestinadoAPremiosCategoria[indice]/Acertantes[indice,1],2);
				switch (indice)
				{
					case 0 :
					{
						txPremio14.Text = Premios[0,1].ToString ();
						break;
					}
					case 1 :
					{
						txPremio13.Text = Premios[1,1].ToString ();
						break;
					}
					case 2:
					{
						txPremio12.Text = Premios[2,1].ToString ();
						break;
					}
					case 3:
					{
						txPremio11.Text = Premios[3,1].ToString ();
						break;
					}
					case 4:
					{
						txPremio10.Text = Premios[4,1].ToString ();
						break;		
					}
				}
			}		
		}

		private void btCopiar_Click(object sender, System.EventArgs e)
		{
			string cadena="Columna premiada: ";
			char sep = (char) 9;
			char LF = (char) 10;
			char NL = (char) 13;

			cadena = cadena + ColumnaPremiada();
			cadena = cadena + NL+LF + lblPrevAcertantes14.Text + sep + "de 14 a " + sep + lblPrevPremio14.Text + sep + "";
			cadena = cadena + NL+LF + lblPrevAcertantes13.Text + sep + "de 13 a " + sep + lblPrevPremio13.Text + sep + "";
			cadena = cadena + NL+LF + lblPrevAcertantes12.Text + sep + "de 12 a " + sep + lblPrevPremio12.Text + sep + "";
			cadena = cadena + NL+LF + lblPrevAcertantes11.Text + sep + "de 11 a " + sep + lblPrevPremio11.Text + sep + "";
			cadena = cadena + NL+LF + lblPrevAcertantes10.Text + sep + "de 10 a " + sep + lblPrevPremio10.Text + sep + "";

			Clipboard.SetDataObject (cadena, true);
		}

		private void linkLAE_LinkClicked(object sender, System.Windows.Forms.LinkLabelLinkClickedEventArgs e)
		{
						System.Diagnostics.Process.Start("IExplore"," http://onlae.terra.es/qsorteos.asp");
		}
		private void GenericLabel_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			Cursor.Current = Cursors.Hand ;
		}

		private void btGuardarEscrutinio_Click(object sender, System.EventArgs e)
		{
			LAE JornadaLAE = new LAE (txTemporada.Text, txJornada.Text, ColumnaPremiada(), Recaudacion);
			int[] acertantesreales=new int[5];
			for (int i=0;i<5;i++)
			{
				acertantesreales[i]=(int)Acertantes[i,1];
			}
			JornadaLAE.GrabarJornada (acertantesreales);
		}
		private string ColumnaPremiada()
		{
			return lblNewBase11.Text+lblNewBase21.Text + lblNewBase31.Text + lblNewBase41.Text + lblNewBase51.Text + lblNewBase61.Text +lblNewBase71.Text + lblNewBase81.Text +lblNewBase91.Text +lblNewBase101.Text +lblNewBase111.Text + lblNewBase121.Text + lblNewBase131.Text +lblNewBase141.Text;
		}

		private void controlPorcentajes1_Modificado(object sender, System.EventArgs e)
		{
			statusBarPanel2.Text =controlPorcentajes1.archivoPorcentajes;
			CalcularPremios();
			Application.DoEvents();
		}

	}
}
