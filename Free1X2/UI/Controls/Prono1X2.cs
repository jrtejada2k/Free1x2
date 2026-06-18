using System;
using System.Windows.Forms;
using System.Drawing;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI.Controls
{
	public class Prono1X2 : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Label lblSigno2;
		private System.Windows.Forms.Label lblSigno1;
		private System.Windows.Forms.Label lblSignoX;

		private Color colorFondo = System.Drawing.Color.Wheat;

		public Prono1X2()
		{
			InitializeComponent();
		}
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToControl(this);
        }


		public System.Drawing.Color ColorFondo
		{
			get{ return colorFondo;}
			set{ colorFondo = value;}		
		}
		
		public string Pronostico
		{
			get
			{
				string valores = "";
				
				if(lblSigno1.BackColor == System.Drawing.Color.LimeGreen)
				{
					valores = "1";
				}
				
				if(lblSignoX.BackColor == System.Drawing.Color.LimeGreen)
				{
					valores += "X";
				}
				
				if(lblSigno2.BackColor == System.Drawing.Color.LimeGreen)
				{
					valores += "2";
				}
				
				return valores;			
			}
			set
			{
				string valores = value;
				
				//reinicializar pronosticos
				this.lblSigno1.BackColor = colorFondo;
				this.lblSignoX.BackColor = colorFondo;
				this.lblSigno2.BackColor = colorFondo;
				
				foreach(char val in valores)
				{
					switch( val )
					{
						case '1':
							lblSigno1.BackColor = System.Drawing.Color.LimeGreen;
							break;
						case 'X':
							lblSignoX.BackColor = System.Drawing.Color.LimeGreen;
							break;
						case '2':
							lblSigno2.BackColor = System.Drawing.Color.LimeGreen;
							break;							
					}
				}
			
			}
		}
		
		
		
		
		void LblSigno1Click(object sender, System.EventArgs e)
		{
			if( this.lblSigno1.BackColor == colorFondo )
			{
				this.lblSigno1.BackColor = System.Drawing.Color.LimeGreen;
			}
			else
			{
				this.lblSigno1.BackColor = colorFondo;
			}	
		}
		
		void LblSignoXClick(object sender, System.EventArgs e)
		{
			if( this.lblSignoX.BackColor == colorFondo )
			{
				this.lblSignoX.BackColor = System.Drawing.Color.LimeGreen;
			}
			else
			{
				this.lblSignoX.BackColor = colorFondo;
			}	
		}
		
		void LblSigno2Click(object sender, System.EventArgs e)
		{
			if( this.lblSigno2.BackColor == colorFondo )
			{
				this.lblSigno2.BackColor = System.Drawing.Color.LimeGreen;
			}
			else
			{
				this.lblSigno2.BackColor = colorFondo;
			}	
		}
		
		void InitializeComponent() {
            this.lblSignoX = new System.Windows.Forms.Label();
            this.lblSigno1 = new System.Windows.Forms.Label();
            this.lblSigno2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblSignoX
            // 
            this.lblSignoX.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSignoX.Location = new System.Drawing.Point(21, 0);
            this.lblSignoX.Name = "lblSignoX";
            this.lblSignoX.Size = new System.Drawing.Size(20, 20);
            this.lblSignoX.TabIndex = 1;
            this.lblSignoX.Text = "X";
            this.lblSignoX.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSignoX.Click += new System.EventHandler(this.LblSignoXClick);
            // 
            // lblSigno1
            // 
            this.lblSigno1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSigno1.Location = new System.Drawing.Point(0, 0);
            this.lblSigno1.Name = "lblSigno1";
            this.lblSigno1.Size = new System.Drawing.Size(20, 20);
            this.lblSigno1.TabIndex = 0;
            this.lblSigno1.Text = "1";
            this.lblSigno1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSigno1.Click += new System.EventHandler(this.LblSigno1Click);
            // 
            // lblSigno2
            // 
            this.lblSigno2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSigno2.Location = new System.Drawing.Point(42, 0);
            this.lblSigno2.Name = "lblSigno2";
            this.lblSigno2.Size = new System.Drawing.Size(20, 20);
            this.lblSigno2.TabIndex = 2;
            this.lblSigno2.Text = "2";
            this.lblSigno2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSigno2.Click += new System.EventHandler(this.LblSigno2Click);
            // 
            // Prono1X2
            // 
            this.Controls.Add(this.lblSigno2);
            this.Controls.Add(this.lblSignoX);
            this.Controls.Add(this.lblSigno1);
            this.Name = "Prono1X2";
            this.Size = new System.Drawing.Size(64, 20);
            this.ResumeLayout(false);

		}
				
	}
}
