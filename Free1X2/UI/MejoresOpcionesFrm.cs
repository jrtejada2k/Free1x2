using System;
using System.Collections.Generic;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

using Free1X2.Escrutinio;

namespace Free1X2.UI
{
	/// <summary>
	/// Summary description for MejoresOpcionesFrm.
	/// </summary>
	public class MejoresOpcionesFrm : Form
	{
		private Label label1;
		private Button btnCalcular;
		private CheckBox ckb2;
		private CheckBox ckb1;
		private CheckBox ckb4;
		private CheckBox ckb3;
		private CheckBox ckb8;
		private CheckBox ckb7;
		private CheckBox ckb6;
		private CheckBox ckb5;
		private CheckBox ckb11;
		private CheckBox ckb10;
		private CheckBox ckb9;
		private CheckBox ckb13;
		private CheckBox ckb12;
		private CheckBox ckb14;
        protected List<string> cGanadoras = new List<string>();
		protected List<string> archivoColumnas;
	    protected List<PosiblesPremiosContenedor> resumen = new List<PosiblesPremiosContenedor>();
		private TextBox txtResumen;
		private Label label2;
		private TextBox txtLimite;
		private Label label3;
		private Label label4;
		protected string columnaGanadora = "**************";
        int noPartidos;
        private CheckBox ckb16;
        private CheckBox ckb15;
        private bool ContemplaPleno;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private Container components = null;

		public MejoresOpcionesFrm(bool contemplaPleno)
		{
			InitializeComponent();
            noPartidos = ColumnaGanadora.Length;
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
            ContemplaPleno = contemplaPleno;
		}
        protected void AdaptarInterfaz(int partidos)
        {
            ckb16.Visible = partidos >= 16;
            ckb15.Visible = partidos >= 15;
            ckb14.Visible = partidos >= 14;
            ckb13.Visible = partidos >= 13;
            ckb12.Visible = partidos >= 12;
            ckb11.Visible = partidos >= 11;
            ckb10.Visible = partidos >= 10;

        }
		public List<string> ArchivoColumnas
		{
			get {return archivoColumnas;}
			set {archivoColumnas = value;}
		}
		public string ColumnaGanadora
		{
			get {return columnaGanadora;}
			set {columnaGanadora = value;}
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MejoresOpcionesFrm));
            this.label1 = new System.Windows.Forms.Label();
            this.ckb2 = new System.Windows.Forms.CheckBox();
            this.ckb1 = new System.Windows.Forms.CheckBox();
            this.ckb4 = new System.Windows.Forms.CheckBox();
            this.ckb3 = new System.Windows.Forms.CheckBox();
            this.ckb8 = new System.Windows.Forms.CheckBox();
            this.ckb7 = new System.Windows.Forms.CheckBox();
            this.ckb6 = new System.Windows.Forms.CheckBox();
            this.ckb5 = new System.Windows.Forms.CheckBox();
            this.ckb11 = new System.Windows.Forms.CheckBox();
            this.ckb10 = new System.Windows.Forms.CheckBox();
            this.ckb9 = new System.Windows.Forms.CheckBox();
            this.ckb13 = new System.Windows.Forms.CheckBox();
            this.ckb12 = new System.Windows.Forms.CheckBox();
            this.ckb14 = new System.Windows.Forms.CheckBox();
            this.btnCalcular = new System.Windows.Forms.Button();
            this.txtResumen = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtLimite = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.ckb16 = new System.Windows.Forms.CheckBox();
            this.ckb15 = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Maroon;
            this.label1.Location = new System.Drawing.Point(12, 38);
            this.label1.Name = "label1";
            this.label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label1.Size = new System.Drawing.Size(152, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Partidos Involucrados";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ckb2
            // 
            this.ckb2.Location = new System.Drawing.Point(0, 88);
            this.ckb2.Name = "ckb2";
            this.ckb2.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ckb2.Size = new System.Drawing.Size(42, 24);
            this.ckb2.TabIndex = 1;
            this.ckb2.Text = "2";
            // 
            // ckb1
            // 
            this.ckb1.Location = new System.Drawing.Point(0, 64);
            this.ckb1.Name = "ckb1";
            this.ckb1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ckb1.Size = new System.Drawing.Size(42, 24);
            this.ckb1.TabIndex = 2;
            this.ckb1.Text = "1";
            // 
            // ckb4
            // 
            this.ckb4.Location = new System.Drawing.Point(0, 136);
            this.ckb4.Name = "ckb4";
            this.ckb4.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ckb4.Size = new System.Drawing.Size(42, 24);
            this.ckb4.TabIndex = 4;
            this.ckb4.Text = "4";
            // 
            // ckb3
            // 
            this.ckb3.Location = new System.Drawing.Point(0, 112);
            this.ckb3.Name = "ckb3";
            this.ckb3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ckb3.Size = new System.Drawing.Size(42, 24);
            this.ckb3.TabIndex = 3;
            this.ckb3.Text = "3";
            // 
            // ckb8
            // 
            this.ckb8.Location = new System.Drawing.Point(0, 240);
            this.ckb8.Name = "ckb8";
            this.ckb8.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ckb8.Size = new System.Drawing.Size(42, 24);
            this.ckb8.TabIndex = 8;
            this.ckb8.Text = "8";
            // 
            // ckb7
            // 
            this.ckb7.Location = new System.Drawing.Point(0, 216);
            this.ckb7.Name = "ckb7";
            this.ckb7.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ckb7.Size = new System.Drawing.Size(42, 24);
            this.ckb7.TabIndex = 7;
            this.ckb7.Text = "7";
            // 
            // ckb6
            // 
            this.ckb6.Location = new System.Drawing.Point(0, 192);
            this.ckb6.Name = "ckb6";
            this.ckb6.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ckb6.Size = new System.Drawing.Size(42, 24);
            this.ckb6.TabIndex = 6;
            this.ckb6.Text = "6";
            // 
            // ckb5
            // 
            this.ckb5.Location = new System.Drawing.Point(0, 168);
            this.ckb5.Name = "ckb5";
            this.ckb5.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ckb5.Size = new System.Drawing.Size(42, 24);
            this.ckb5.TabIndex = 5;
            this.ckb5.Text = "5";
            // 
            // ckb11
            // 
            this.ckb11.Location = new System.Drawing.Point(0, 320);
            this.ckb11.Name = "ckb11";
            this.ckb11.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ckb11.Size = new System.Drawing.Size(42, 24);
            this.ckb11.TabIndex = 11;
            this.ckb11.Text = "11";
            // 
            // ckb10
            // 
            this.ckb10.Location = new System.Drawing.Point(0, 296);
            this.ckb10.Name = "ckb10";
            this.ckb10.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ckb10.Size = new System.Drawing.Size(42, 24);
            this.ckb10.TabIndex = 10;
            this.ckb10.Text = "10";
            // 
            // ckb9
            // 
            this.ckb9.Location = new System.Drawing.Point(0, 272);
            this.ckb9.Name = "ckb9";
            this.ckb9.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ckb9.Size = new System.Drawing.Size(42, 24);
            this.ckb9.TabIndex = 9;
            this.ckb9.Text = "9";
            // 
            // ckb13
            // 
            this.ckb13.Location = new System.Drawing.Point(0, 376);
            this.ckb13.Name = "ckb13";
            this.ckb13.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ckb13.Size = new System.Drawing.Size(42, 24);
            this.ckb13.TabIndex = 13;
            this.ckb13.Text = "13";
            // 
            // ckb12
            // 
            this.ckb12.Location = new System.Drawing.Point(0, 352);
            this.ckb12.Name = "ckb12";
            this.ckb12.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ckb12.Size = new System.Drawing.Size(42, 24);
            this.ckb12.TabIndex = 12;
            this.ckb12.Text = "12";
            // 
            // ckb14
            // 
            this.ckb14.Location = new System.Drawing.Point(0, 400);
            this.ckb14.Name = "ckb14";
            this.ckb14.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ckb14.Size = new System.Drawing.Size(42, 24);
            this.ckb14.TabIndex = 14;
            this.ckb14.Text = "14";
            // 
            // btnCalcular
            // 
            this.btnCalcular.BackColor = System.Drawing.Color.LightSalmon;
            this.btnCalcular.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCalcular.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCalcular.Location = new System.Drawing.Point(309, 64);
            this.btnCalcular.Name = "btnCalcular";
            this.btnCalcular.Size = new System.Drawing.Size(75, 23);
            this.btnCalcular.TabIndex = 15;
            this.btnCalcular.Text = "Calcular";
            this.btnCalcular.UseVisualStyleBackColor = false;
            this.btnCalcular.Click += new System.EventHandler(this.button1_Click);
            // 
            // txtResumen
            // 
            this.txtResumen.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtResumen.Location = new System.Drawing.Point(56, 96);
            this.txtResumen.Multiline = true;
            this.txtResumen.Name = "txtResumen";
            this.txtResumen.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtResumen.Size = new System.Drawing.Size(608, 320);
            this.txtResumen.TabIndex = 16;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(8, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(672, 32);
            this.label2.TabIndex = 17;
            this.label2.Text = resources.GetString("label2.Text");
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtLimite
            // 
            this.txtLimite.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLimite.Location = new System.Drawing.Point(136, 66);
            this.txtLimite.Name = "txtLimite";
            this.txtLimite.Size = new System.Drawing.Size(40, 21);
            this.txtLimite.TabIndex = 18;
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(56, 66);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 21);
            this.label3.TabIndex = 19;
            this.label3.Text = "Mostrar los";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(182, 66);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(121, 21);
            this.label4.TabIndex = 20;
            this.label4.Text = "primeros resultados";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ckb16
            // 
            this.ckb16.Location = new System.Drawing.Point(0, 454);
            this.ckb16.Name = "ckb16";
            this.ckb16.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ckb16.Size = new System.Drawing.Size(42, 24);
            this.ckb16.TabIndex = 22;
            this.ckb16.Text = "16";
            // 
            // ckb15
            // 
            this.ckb15.Location = new System.Drawing.Point(0, 430);
            this.ckb15.Name = "ckb15";
            this.ckb15.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ckb15.Size = new System.Drawing.Size(42, 24);
            this.ckb15.TabIndex = 21;
            this.ckb15.Text = "15";
            // 
            // MejoresOpcionesFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(688, 499);
            this.Controls.Add(this.ckb16);
            this.Controls.Add(this.ckb15);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtLimite);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtResumen);
            this.Controls.Add(this.btnCalcular);
            this.Controls.Add(this.ckb14);
            this.Controls.Add(this.ckb13);
            this.Controls.Add(this.ckb12);
            this.Controls.Add(this.ckb11);
            this.Controls.Add(this.ckb10);
            this.Controls.Add(this.ckb9);
            this.Controls.Add(this.ckb8);
            this.Controls.Add(this.ckb7);
            this.Controls.Add(this.ckb6);
            this.Controls.Add(this.ckb5);
            this.Controls.Add(this.ckb4);
            this.Controls.Add(this.ckb3);
            this.Controls.Add(this.ckb1);
            this.Controls.Add(this.ckb2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.Name = "MejoresOpcionesFrm";
            this.Text = "Mis Mejores Opciones";
            this.Load += new System.EventHandler(this.MejoresOpcionesFrm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		private void MejoresOpcionesFrm_Load(object sender, System.EventArgs e)
		{
            AdaptarInterfaz(ColumnaGanadora.Length);
		}
		protected void ObtenGanadoras(string preString, int partidoNo)
		{			
			string[] signos = {"1","X","2"};
			string newPreString;
            bool[] partidosInvolucrados = { ckb1.Checked, ckb2.Checked, ckb3.Checked, ckb4.Checked, ckb5.Checked, ckb6.Checked, ckb7.Checked, ckb8.Checked, ckb9.Checked, ckb10.Checked, ckb11.Checked, ckb12.Checked, ckb13.Checked, ckb14.Checked, ckb15.Checked, ckb16.Checked };

			for( int i = 0; i < signos.Length; i++ )
			{
				if(this.columnaGanadora[partidoNo].ToString() != "*")
				{
					newPreString = preString + columnaGanadora[partidoNo];
					i=4;
				}
				else
				{
					if(partidosInvolucrados[partidoNo])
					{
						newPreString  = preString + signos[i];
					}
					else
					{
						newPreString = preString + "*";
						i=4;

					}
				}
				
								
				if( partidoNo < ColumnaGanadora.Length - 1)
				{
					ObtenGanadoras(newPreString, partidoNo+1);
				}
				else
				{
					cGanadoras.Add(newPreString);					
				}			
			}
					
		}
		
		protected int Escrutar(string cAnalizada, string cGanadora)
		{
			int aciertos = 0;
			int posiblesAciertos = noPartidos;
			for(int i = 0; i < cAnalizada.Length - 1; i++)
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
            if (ContemplaPleno)
            {
                if (aciertos == cAnalizada.Length - 1)
                {
                    if (cAnalizada[cAnalizada.Length - 1] == cGanadora[cGanadora.Length - 1] || cGanadora[cGanadora.Length - 1].ToString() == "*")
                    {
                        aciertos++;
                    }
                }
            }
			return aciertos;
		}

		protected void ObtenerResumen(string cGanadora)
		{
			PosiblesPremiosContenedor posiblesPremios = new PosiblesPremiosContenedor();
			for(int i=0; i< this.archivoColumnas.Count; i++)
			{
				string cAnalizada = archivoColumnas[i];
				int aciertos = Escrutar(cAnalizada,cGanadora);
				if(aciertos > 9)
				{
					posiblesPremios.ColGanadora = cGanadora;
					switch(aciertos)
					{
						case 10:
							posiblesPremios.Col10.Add(cAnalizada);
							break;
						case 11:
							posiblesPremios.Col11.Add(cAnalizada);
                            if (ContemplaPleno && cAnalizada.Length == 11)
                            {
                                posiblesPremios.Col14.Add(cAnalizada);
                            }
							break;
						case 12:
							posiblesPremios.Col12.Add(cAnalizada);
                            if (ContemplaPleno && cAnalizada.Length == 12)
                            {
                                posiblesPremios.Col14.Add(cAnalizada);
                            }
							break;
						case 13:
							posiblesPremios.Col13.Add(cAnalizada);
                            if (ContemplaPleno && cAnalizada.Length == 13)
                            {
                                posiblesPremios.Col14.Add(cAnalizada);
                            }
							break;
						case 14:
							posiblesPremios.Col14.Add(cAnalizada);
                            if (ContemplaPleno && cAnalizada.Length == 14)
                            {
                                posiblesPremios.Col14.Add(cAnalizada);
                            }
							break;
                        case 15:
                            posiblesPremios.Col15.Add(cAnalizada);
                            if (ContemplaPleno && cAnalizada.Length == 15)
                            {
                                posiblesPremios.Col14.Add(cAnalizada);
                            }
                            break;
                        case 16:
                            posiblesPremios.Col16.Add(cAnalizada);
                            if (ContemplaPleno && cAnalizada.Length == 16)
                            {
                                posiblesPremios.Col14.Add(cAnalizada);
                            }
                            break;
					}
				}

			}
			this.resumen.Add(posiblesPremios);
		}
		protected void OrdenarResumen()
		{
			this.resumen.Sort(new PosiblesPremiosComparer());
		}
		protected void MostrarResumen(int limiteResultados)
		{
			this.txtResumen.Text = "Mis mejores opciones son: \r\n";
			this.txtResumen.Text += "------------------------\r\n";	
			this.txtResumen.Text += "\r\n";
			for(int i = 0; i < this.resumen.Count; i++)
			{
				if(i < limiteResultados)
				{
					PosiblesPremiosContenedor contenedor = (PosiblesPremiosContenedor)resumen[i];
                    this.txtResumen.Text += contenedor.ColGanadora + ": ";
                    if (ColumnaGanadora.Length >= 16)
                    {
                        this.txtResumen.Text += contenedor.Col16.Count.ToString() + " de 16 + ";
                    }
                    if (ColumnaGanadora.Length >= 15)
                    {
                        this.txtResumen.Text += contenedor.Col15.Count.ToString() + " de 15 + ";
                    }
                    if (ColumnaGanadora.Length >= 14)
                    {
                        this.txtResumen.Text += contenedor.Col14.Count.ToString() + " de 14 + ";
                    }
                    if (ColumnaGanadora.Length >= 13)
                    {
                        this.txtResumen.Text += contenedor.Col13.Count.ToString() + " de 13 + ";
                    }
                    if (ColumnaGanadora.Length >= 12)
                    {
                        this.txtResumen.Text += contenedor.Col12.Count.ToString() + " de 12 + ";
                    }
                    if (ColumnaGanadora.Length >= 11)
                    {
                        this.txtResumen.Text += contenedor.Col11.Count.ToString() + " de 11 + ";
                    }
                    if (ColumnaGanadora.Length >= 10)
                    {
                        this.txtResumen.Text += contenedor.Col10.Count + " de 10";
                    } 
                    this.txtResumen.Text += "\r\n";
				}
				else
				{
					break;
				}
			}
		}
		protected int ObtenerLimiteResultados()
		{
			int limiteResultados;
			try
			{
				limiteResultados = Convert.ToInt32(txtLimite.Text);
			}
			catch
			{
				limiteResultados = 10;
				txtLimite.Text = "10";
			}
			return limiteResultados;
		}

		private void button1_Click(object sender, EventArgs e)
		{
			txtResumen.Text = "";
			resumen.Clear();
			//Hallar todas las posibilidades
			cGanadoras.Clear();
			ObtenGanadoras("",0);
			for(int i = 0; i < cGanadoras.Count; i++)
			{
				ObtenerResumen(cGanadoras[i]);
			}
			OrdenarResumen();
			MostrarResumen(ObtenerLimiteResultados());

		}
		

	}
}
