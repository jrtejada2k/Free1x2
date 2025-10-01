using System;
using System.Drawing;
using System.Collections.Generic;
using System.Windows.Forms;
using Free1X2.Escrutinio;
using Free1X2.UI.Controls;

namespace Free1X2.UI
{
	/// <summary>
	/// Summary description for VisorPosiblesPremios.
	/// </summary>
	public class VisorPosiblesPremios : Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components;
		private ContainerControl cctrl;
		private Label lblCG14;
		private Label lblCG13;
		private Label lblCG12;
		private Label lblCG11;
		private Label lblCG10;
		private Label lblCG9;
		private Label lblCG8;
		private Label lblCG7;
		private Label lblCG6;
		private Label lblCG5;
		private Label lblCG4;
		private Label lblCG3;
		private Label lblCG2;
		private Label lblCG1;
		private Button btnAdelante;
		private Button btnAtras;
		List<PosiblesPremiosContenedor> resumen;
		private Label lblContador;
		private Label lblCG;
        private Label lblCG15;
        private Label lblCG16;
		int grupoMostrado;

		public VisorPosiblesPremios(List<PosiblesPremiosContenedor> resumenPremios)
		{
			InitializeComponent();
			Resumen = resumenPremios;
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}

		public List<PosiblesPremiosContenedor> Resumen
		{
			get {return resumen;}
			set {resumen = value;}
		}
		protected int ObtenerNumeroControles(PosiblesPremiosContenedor contenedor)
		{
            return contenedor.Col16.Count + contenedor.Col15.Count + contenedor.Col14.Count + contenedor.Col13.Count + contenedor.Col12.Count + contenedor.Col11.Count + contenedor.Col10.Count;
		}
		protected void LimpiarPantalla()
		{
			cctrl.Controls.Clear();
		}
		protected void MostrarGrupos(int noGrupo)
		{
			LimpiarPantalla();
			PosiblesPremiosContenedor contenedor = Resumen[noGrupo];
            List<string> arrayColumnas = new List<string>();

            arrayColumnas.AddRange(contenedor.Col16);
            arrayColumnas.AddRange(contenedor.Col15);
			arrayColumnas.AddRange(contenedor.Col14);
			arrayColumnas.AddRange(contenedor.Col13);
			arrayColumnas.AddRange(contenedor.Col12);
			arrayColumnas.AddRange(contenedor.Col11);
			arrayColumnas.AddRange(contenedor.Col10);
			//Mostrar Ganadora
			IndicarColumnaGanadora(contenedor.ColGanadora);
			int posicionXInicial = 0;
			int posicionYInicial = 0;
			for(int i=0; i<arrayColumnas.Count; i++)
			{
				string columna = arrayColumnas[i];
				AñadirControl(posicionXInicial,posicionYInicial,columna,contenedor.ColGanadora);
				posicionXInicial += 25;
			}
		}
		protected void IndicarColumnaGanadora(string columnaGanadora)
		{
            Label[] labels = { lblCG1, lblCG2,lblCG3,lblCG4,lblCG5,lblCG6,lblCG7,lblCG8,lblCG9,lblCG10,lblCG11,lblCG12,lblCG13,lblCG14,lblCG15,lblCG16};
            for (int i = 0; i < columnaGanadora.Length; i++)
            {
                labels[i].Text = columnaGanadora[i].ToString();
                labels[i].Visible = true;
            }


		}
		private void AñadirControl(int posicionX, int posicionY, string columna, string columnaGanadora)
		{
			ControlPosiblesPremios ctrl = new ControlPosiblesPremios(columna,columnaGanadora);

			ctrl.Location = new Point(posicionX,posicionY);
			cctrl.Controls.Add(ctrl);
			cctrl.AutoScroll = true;
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
            this.cctrl = new System.Windows.Forms.ContainerControl();
            this.lblCG14 = new System.Windows.Forms.Label();
            this.lblCG13 = new System.Windows.Forms.Label();
            this.lblCG12 = new System.Windows.Forms.Label();
            this.lblCG11 = new System.Windows.Forms.Label();
            this.lblCG10 = new System.Windows.Forms.Label();
            this.lblCG9 = new System.Windows.Forms.Label();
            this.lblCG8 = new System.Windows.Forms.Label();
            this.lblCG7 = new System.Windows.Forms.Label();
            this.lblCG6 = new System.Windows.Forms.Label();
            this.lblCG5 = new System.Windows.Forms.Label();
            this.lblCG4 = new System.Windows.Forms.Label();
            this.lblCG3 = new System.Windows.Forms.Label();
            this.lblCG2 = new System.Windows.Forms.Label();
            this.lblCG1 = new System.Windows.Forms.Label();
            this.btnAdelante = new System.Windows.Forms.Button();
            this.btnAtras = new System.Windows.Forms.Button();
            this.lblContador = new System.Windows.Forms.Label();
            this.lblCG = new System.Windows.Forms.Label();
            this.lblCG15 = new System.Windows.Forms.Label();
            this.lblCG16 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // cctrl
            // 
            this.cctrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.cctrl.BackColor = System.Drawing.Color.Bisque;
            this.cctrl.Location = new System.Drawing.Point(40, 32);
            this.cctrl.Name = "cctrl";
            this.cctrl.Size = new System.Drawing.Size(336, 455);
            this.cctrl.TabIndex = 0;
            this.cctrl.Text = "containerControl1";
            this.cctrl.Click += new System.EventHandler(this.cctrl_Click);
            // 
            // lblCG14
            // 
            this.lblCG14.BackColor = System.Drawing.Color.White;
            this.lblCG14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCG14.Location = new System.Drawing.Point(8, 400);
            this.lblCG14.Name = "lblCG14";
            this.lblCG14.Size = new System.Drawing.Size(20, 20);
            this.lblCG14.TabIndex = 161;
            this.lblCG14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCG14.Visible = false;
            // 
            // lblCG13
            // 
            this.lblCG13.BackColor = System.Drawing.Color.White;
            this.lblCG13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCG13.Location = new System.Drawing.Point(8, 376);
            this.lblCG13.Name = "lblCG13";
            this.lblCG13.Size = new System.Drawing.Size(20, 20);
            this.lblCG13.TabIndex = 160;
            this.lblCG13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCG13.Visible = false;
            // 
            // lblCG12
            // 
            this.lblCG12.BackColor = System.Drawing.Color.White;
            this.lblCG12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCG12.Location = new System.Drawing.Point(8, 352);
            this.lblCG12.Name = "lblCG12";
            this.lblCG12.Size = new System.Drawing.Size(20, 20);
            this.lblCG12.TabIndex = 159;
            this.lblCG12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCG12.Visible = false;
            // 
            // lblCG11
            // 
            this.lblCG11.BackColor = System.Drawing.Color.White;
            this.lblCG11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCG11.Location = new System.Drawing.Point(8, 320);
            this.lblCG11.Name = "lblCG11";
            this.lblCG11.Size = new System.Drawing.Size(20, 20);
            this.lblCG11.TabIndex = 158;
            this.lblCG11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCG11.Visible = false;
            // 
            // lblCG10
            // 
            this.lblCG10.BackColor = System.Drawing.Color.White;
            this.lblCG10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCG10.Location = new System.Drawing.Point(8, 296);
            this.lblCG10.Name = "lblCG10";
            this.lblCG10.Size = new System.Drawing.Size(20, 20);
            this.lblCG10.TabIndex = 157;
            this.lblCG10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCG10.Visible = false;
            // 
            // lblCG9
            // 
            this.lblCG9.BackColor = System.Drawing.Color.White;
            this.lblCG9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCG9.Location = new System.Drawing.Point(8, 272);
            this.lblCG9.Name = "lblCG9";
            this.lblCG9.Size = new System.Drawing.Size(20, 20);
            this.lblCG9.TabIndex = 156;
            this.lblCG9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCG9.Visible = false;
            // 
            // lblCG8
            // 
            this.lblCG8.BackColor = System.Drawing.Color.White;
            this.lblCG8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCG8.Location = new System.Drawing.Point(8, 240);
            this.lblCG8.Name = "lblCG8";
            this.lblCG8.Size = new System.Drawing.Size(20, 20);
            this.lblCG8.TabIndex = 155;
            this.lblCG8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCG8.Visible = false;
            // 
            // lblCG7
            // 
            this.lblCG7.BackColor = System.Drawing.Color.White;
            this.lblCG7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCG7.Location = new System.Drawing.Point(8, 216);
            this.lblCG7.Name = "lblCG7";
            this.lblCG7.Size = new System.Drawing.Size(20, 20);
            this.lblCG7.TabIndex = 154;
            this.lblCG7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCG7.Visible = false;
            // 
            // lblCG6
            // 
            this.lblCG6.BackColor = System.Drawing.Color.White;
            this.lblCG6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCG6.Location = new System.Drawing.Point(8, 192);
            this.lblCG6.Name = "lblCG6";
            this.lblCG6.Size = new System.Drawing.Size(20, 20);
            this.lblCG6.TabIndex = 153;
            this.lblCG6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCG6.Visible = false;
            // 
            // lblCG5
            // 
            this.lblCG5.BackColor = System.Drawing.Color.White;
            this.lblCG5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCG5.Location = new System.Drawing.Point(8, 168);
            this.lblCG5.Name = "lblCG5";
            this.lblCG5.Size = new System.Drawing.Size(20, 20);
            this.lblCG5.TabIndex = 152;
            this.lblCG5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCG5.Visible = false;
            // 
            // lblCG4
            // 
            this.lblCG4.BackColor = System.Drawing.Color.White;
            this.lblCG4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCG4.Location = new System.Drawing.Point(8, 136);
            this.lblCG4.Name = "lblCG4";
            this.lblCG4.Size = new System.Drawing.Size(20, 20);
            this.lblCG4.TabIndex = 151;
            this.lblCG4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCG4.Visible = false;
            // 
            // lblCG3
            // 
            this.lblCG3.BackColor = System.Drawing.Color.White;
            this.lblCG3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCG3.Location = new System.Drawing.Point(8, 112);
            this.lblCG3.Name = "lblCG3";
            this.lblCG3.Size = new System.Drawing.Size(20, 20);
            this.lblCG3.TabIndex = 150;
            this.lblCG3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCG3.Visible = false;
            // 
            // lblCG2
            // 
            this.lblCG2.BackColor = System.Drawing.Color.White;
            this.lblCG2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCG2.Location = new System.Drawing.Point(8, 88);
            this.lblCG2.Name = "lblCG2";
            this.lblCG2.Size = new System.Drawing.Size(20, 20);
            this.lblCG2.TabIndex = 149;
            this.lblCG2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCG2.Visible = false;
            // 
            // lblCG1
            // 
            this.lblCG1.BackColor = System.Drawing.Color.White;
            this.lblCG1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCG1.Location = new System.Drawing.Point(8, 64);
            this.lblCG1.Name = "lblCG1";
            this.lblCG1.Size = new System.Drawing.Size(20, 20);
            this.lblCG1.TabIndex = 148;
            this.lblCG1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCG1.Visible = false;
            // 
            // btnAdelante
            // 
            this.btnAdelante.BackColor = System.Drawing.Color.LightSalmon;
            this.btnAdelante.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAdelante.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAdelante.Location = new System.Drawing.Point(241, 0);
            this.btnAdelante.Name = "btnAdelante";
            this.btnAdelante.Size = new System.Drawing.Size(24, 24);
            this.btnAdelante.TabIndex = 162;
            this.btnAdelante.Text = ">";
            this.btnAdelante.UseVisualStyleBackColor = false;
            this.btnAdelante.Click += new System.EventHandler(this.btnAdelante_Click);
            // 
            // btnAtras
            // 
            this.btnAtras.BackColor = System.Drawing.Color.LightSalmon;
            this.btnAtras.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAtras.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAtras.Location = new System.Drawing.Point(135, 0);
            this.btnAtras.Name = "btnAtras";
            this.btnAtras.Size = new System.Drawing.Size(24, 24);
            this.btnAtras.TabIndex = 163;
            this.btnAtras.Text = "<";
            this.btnAtras.UseVisualStyleBackColor = false;
            this.btnAtras.Click += new System.EventHandler(this.btnAtras_Click);
            // 
            // lblContador
            // 
            this.lblContador.BackColor = System.Drawing.Color.White;
            this.lblContador.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblContador.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblContador.Location = new System.Drawing.Point(160, 0);
            this.lblContador.Name = "lblContador";
            this.lblContador.Size = new System.Drawing.Size(80, 24);
            this.lblContador.TabIndex = 164;
            this.lblContador.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCG
            // 
            this.lblCG.BackColor = System.Drawing.Color.Bisque;
            this.lblCG.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCG.ForeColor = System.Drawing.Color.Maroon;
            this.lblCG.Location = new System.Drawing.Point(8, 40);
            this.lblCG.Name = "lblCG";
            this.lblCG.Size = new System.Drawing.Size(24, 20);
            this.lblCG.TabIndex = 165;
            this.lblCG.Text = "CG";
            this.lblCG.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblCG15
            // 
            this.lblCG15.BackColor = System.Drawing.Color.White;
            this.lblCG15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCG15.Location = new System.Drawing.Point(8, 432);
            this.lblCG15.Name = "lblCG15";
            this.lblCG15.Size = new System.Drawing.Size(20, 20);
            this.lblCG15.TabIndex = 166;
            this.lblCG15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCG15.Visible = false;
            // 
            // lblCG16
            // 
            this.lblCG16.BackColor = System.Drawing.Color.White;
            this.lblCG16.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblCG16.Location = new System.Drawing.Point(8, 456);
            this.lblCG16.Name = "lblCG16";
            this.lblCG16.Size = new System.Drawing.Size(20, 20);
            this.lblCG16.TabIndex = 162;
            this.lblCG16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblCG16.Visible = false;
            // 
            // VisorPosiblesPremios
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(384, 499);
            this.Controls.Add(this.lblCG16);
            this.Controls.Add(this.lblCG15);
            this.Controls.Add(this.lblCG);
            this.Controls.Add(this.lblContador);
            this.Controls.Add(this.cctrl);
            this.Controls.Add(this.lblCG1);
            this.Controls.Add(this.lblCG2);
            this.Controls.Add(this.lblCG3);
            this.Controls.Add(this.lblCG4);
            this.Controls.Add(this.lblCG5);
            this.Controls.Add(this.lblCG6);
            this.Controls.Add(this.lblCG7);
            this.Controls.Add(this.lblCG8);
            this.Controls.Add(this.lblCG9);
            this.Controls.Add(this.lblCG10);
            this.Controls.Add(this.lblCG11);
            this.Controls.Add(this.lblCG12);
            this.Controls.Add(this.lblCG13);
            this.Controls.Add(this.lblCG14);
            this.Controls.Add(this.btnAdelante);
            this.Controls.Add(this.btnAtras);
            this.Name = "VisorPosiblesPremios";
            this.Text = "Visor de Posibles Premios";
            this.Load += new System.EventHandler(this.VisorPosiblesPremios_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void VisorPosiblesPremios_Load(object sender, EventArgs e)
		{
			MostrarGrupos(grupoMostrado);
			lblContador.Text = Convert.ToString(grupoMostrado + 1) + " de " + Resumen.Count;
		}

		private void btnAdelante_Click(object sender, EventArgs e)
		{		
			if(grupoMostrado < Resumen.Count - 1)
			{
				grupoMostrado++;
				MostrarGrupos(grupoMostrado);
				lblContador.Text = Convert.ToString(grupoMostrado + 1) + " de " + Resumen.Count;
			}
		}

		private void btnAtras_Click(object sender, EventArgs e)
		{
			if(grupoMostrado > 0)
			{
				grupoMostrado--;
				MostrarGrupos(grupoMostrado);
				lblContador.Text = Convert.ToString(grupoMostrado + 1) + " de " + Resumen.Count;
			}
		}

		private void cctrl_Click(object sender, EventArgs e)
		{
		
		}
	}
}
