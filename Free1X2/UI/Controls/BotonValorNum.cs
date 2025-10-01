using System;
using System.Windows.Forms;

namespace Free1X2.UI.Controls
{
	public class BotonValorNum : UserControl
	{
		private Label lblValor;
		
		private string valorNumerico = "14";
		private string letraTolerancia = "";
				
		public BotonValorNum()
		{
			InitializeComponent();
		}
		
		protected void PonerValorOpcion()
		{
			lblValor.Text = valorNumerico + " " + letraTolerancia;		
		}
		
		
		
		protected void BotonDerechoApretado()
		{
		    if( lblValor.BackColor == System.Drawing.Color.Wheat )
			{
				lblValor.BackColor = System.Drawing.Color.DarkOrange;
				letraTolerancia = "Z";
			}
			else if (lblValor.BackColor == System.Drawing.Color.LightGreen) 
			{
				lblValor.BackColor = System.Drawing.Color.Wheat;		
				letraTolerancia = "";
			}
			else 
			{
				
				if(letraTolerancia == "A")
				{
					letraTolerancia = "";
					lblValor.BackColor = System.Drawing.Color.LightGreen;
				}
				else
				{
				    int valorNumericoToleracia = Convert.ToInt32(letraTolerancia[0]);
				    letraTolerancia = Convert.ToChar( valorNumericoToleracia - 1 ).ToString();
				}
			}
			
			PonerValorOpcion();
		
		}
		
		protected void BotonIzquierdoApretado()
		{
		    if( lblValor.BackColor == System.Drawing.Color.Wheat )
			{
				lblValor.BackColor = System.Drawing.Color.LightGreen;
			}
			else if (lblValor.BackColor == System.Drawing.Color.LightGreen) 
			{
				lblValor.BackColor = System.Drawing.Color.DarkOrange;		
				letraTolerancia = "A";
			}
			else 
			{
				if(letraTolerancia == "Z")
				{
					letraTolerancia = "";
					lblValor.BackColor = System.Drawing.Color.Wheat;
				}
				else
				{
				    int valorNumericoToleracia = Convert.ToInt32(letraTolerancia[0]);
				    letraTolerancia = Convert.ToChar( valorNumericoToleracia + 1 ).ToString();
				}
			}
			
			PonerValorOpcion();
		
		}
		
		public void Reinizializa()
		{
			lblValor.BackColor = System.Drawing.Color.Wheat;
			letraTolerancia = "";
			PonerValorOpcion();
		}
		
		public bool BotonActivo
		{
			get
			{
			    if(lblValor.BackColor == System.Drawing.Color.Wheat)
				{
					return false;
				}
			    return true;
			}
		}
		
		public string ValorNumerico
		{
			get { return valorNumerico; }
			set 
			{ 
				valorNumerico = value;
				PonerValorOpcion();
			}
		}
		
		public string LetraTolerancia
		{
			get { return letraTolerancia; }
			set 
			{ 
				letraTolerancia = value;
				PonerValorOpcion();
			}
		}
		
		public string ValorCombinado
		{
			get { return valorNumerico + letraTolerancia; }
			set
			{
				string val = value;
				
				if(Char.IsLetter(val[val.Length-1]))
				{
					valorNumerico = val.Substring(0,val.Length-1);
					letraTolerancia = val.Substring(val.Length-1,1);
					lblValor.BackColor = System.Drawing.Color.DarkOrange;
				}
				else
				{
					valorNumerico = val;
					letraTolerancia = "";
					lblValor.BackColor = System.Drawing.Color.LightGreen;
				}
				
				PonerValorOpcion();				
			}
		}

		void InitializeComponent() {
            this.lblValor = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblValor
            // 
            this.lblValor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblValor.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblValor.Location = new System.Drawing.Point(0, 0);
            this.lblValor.Name = "lblValor";
            this.lblValor.Size = new System.Drawing.Size(32, 16);
            this.lblValor.TabIndex = 0;
            this.lblValor.Text = "14 A";
            this.lblValor.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblValor.MouseDown += new System.Windows.Forms.MouseEventHandler(this.LblValorMouseDown);
            // 
            // BotonValorNum
            // 
            this.BackColor = System.Drawing.Color.Wheat;
            this.Controls.Add(this.lblValor);
            this.Name = "BotonValorNum";
            this.Size = new System.Drawing.Size(32, 16);
            this.ResumeLayout(false);

		}
								
		void LblValorMouseDown(object sender, MouseEventArgs e)
		{
			
			switch(e.Button)
			{
				case MouseButtons.Left:
					BotonIzquierdoApretado();
					break;
				case MouseButtons.Right:
					BotonDerechoApretado();
					break;				
			}				
		}
		
	}
}


