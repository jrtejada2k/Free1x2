using System;
using System.Windows.Forms;

namespace Free1X2.UI.Controls
{
	public class OptionNum : UserControl
	{
		private Label option;
		
		public OptionNum()
		{
			InitializeComponent();
		}
		
		public string Valor
		{
			get{ return option.Text; }
			set{ option.Text = value; }		
		}
		
		public bool ValorActivado
		{
			get
			{
				bool activo = false;
				
				if( option.BackColor == System.Drawing.Color.LightGreen )
				{
					activo = true;	
				}
				
				return activo;		
			}
			set
			{
				if( value )
				{
					option.BackColor = System.Drawing.Color.LightGreen;
				}
				else
				{
					option.BackColor = System.Drawing.Color.Wheat;
				}			
			}
		}

		void OptionClick(object sender, EventArgs e)
		{
			if( option.BackColor == System.Drawing.Color.Wheat )
			{
				option.BackColor = System.Drawing.Color.LightGreen;
			}
			else
			{
				option.BackColor = System.Drawing.Color.Wheat;
			}	
		}
		
		void InitializeComponent() {
            this.option = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // option
            // 
            this.option.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.option.Location = new System.Drawing.Point(0, 0);
            this.option.Name = "option";
            this.option.Size = new System.Drawing.Size(38, 16);
            this.option.TabIndex = 0;
            this.option.Text = "0";
            this.option.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.option.Click += new System.EventHandler(this.OptionClick);
            // 
            // OptionNum
            // 
            this.BackColor = System.Drawing.Color.Wheat;
            this.Controls.Add(this.option);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "OptionNum";
            this.Size = new System.Drawing.Size(38, 16);
            this.ResumeLayout(false);

		}
	}
}
