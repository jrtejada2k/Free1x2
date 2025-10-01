using System;
using System.Windows.Forms;

namespace Free1X2.UI.Controls
{
	public class OptionNumsHoriz0_9 : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.Label opt5;
		private System.Windows.Forms.Label opt2;
		private System.Windows.Forms.Label opt3;
		private System.Windows.Forms.Label opt0;
		private System.Windows.Forms.Label opt1;
		private System.Windows.Forms.Label opt7;
		private System.Windows.Forms.Label opt8;
		private System.Windows.Forms.Label opt9;
		private System.Windows.Forms.Label opt6;
		private System.Windows.Forms.Label opt4;
		
		public OptionNumsHoriz0_9()
		{
			InitializeComponent();
		}
		
		public string Valores
		{
			get
			{
				string valores = "";
				
				if(opt0.BackColor == System.Drawing.Color.LightGreen)
				{
					valores = "0";
				}
				
				if(opt1.BackColor == System.Drawing.Color.LightGreen)
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
					valores += "1";
				}
				
				if(opt2.BackColor == System.Drawing.Color.LightGreen)
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
					valores += "2";
				}
				
				if(opt3.BackColor == System.Drawing.Color.LightGreen)
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
					valores += "3";
				}
				
				if(opt4.BackColor == System.Drawing.Color.LightGreen)
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
					valores += "4";
				}
				
				if(opt5.BackColor == System.Drawing.Color.LightGreen)
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
					valores += "5";
				}
				
				if(opt6.BackColor == System.Drawing.Color.LightGreen)
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
					valores += "6";
				}
				
				if(opt7.BackColor == System.Drawing.Color.LightGreen)
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
					valores += "7";
				}
				
				if(opt8.BackColor == System.Drawing.Color.LightGreen)
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
					valores += "8";
				}
				
				if(opt9.BackColor == System.Drawing.Color.LightGreen)
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
					valores += "9";
				}
								
				//do not return an empty string!
				//if(valores.Equals(""))
				//{
				//	valores = "0,1,2,3,4,5,6,7,8,9,10,11,12,13,14";
				//}
				
				return valores;
			}
			set
			{
				string valores = value;
				string[] valArray = valores.Split(',');
				
				//reinicializa valores
				opt0.BackColor = System.Drawing.Color.Wheat;
				opt1.BackColor = System.Drawing.Color.Wheat;
				opt2.BackColor = System.Drawing.Color.Wheat;
				opt3.BackColor = System.Drawing.Color.Wheat;
				opt4.BackColor = System.Drawing.Color.Wheat;
				opt5.BackColor = System.Drawing.Color.Wheat;
				opt6.BackColor = System.Drawing.Color.Wheat;
				opt7.BackColor = System.Drawing.Color.Wheat;
				opt8.BackColor = System.Drawing.Color.Wheat;
				opt9.BackColor = System.Drawing.Color.Wheat;
								
				foreach(string val in valArray)
				{
					switch( val )
					{
						case "0":
							opt0.BackColor = System.Drawing.Color.LightGreen;
							break;
						case "1":
							opt1.BackColor = System.Drawing.Color.LightGreen;
							break;
						case "2":
							opt2.BackColor = System.Drawing.Color.LightGreen;
							break;
						case "3":
							opt3.BackColor = System.Drawing.Color.LightGreen;
							break;
						case "4":
							opt4.BackColor = System.Drawing.Color.LightGreen;
							break;
						case "5":
							opt5.BackColor = System.Drawing.Color.LightGreen;
							break;
						case "6":
							opt6.BackColor = System.Drawing.Color.LightGreen;
							break;
						case "7":
							opt7.BackColor = System.Drawing.Color.LightGreen;
							break;
						case "8":
							opt8.BackColor = System.Drawing.Color.LightGreen;
							break;
						case "9":
							opt9.BackColor = System.Drawing.Color.LightGreen;
							break;						
					}
				
				}			
					
			}
		}

		void Opt1Click(object sender, System.EventArgs e)
		{
			if( this.opt1.BackColor == System.Drawing.Color.Wheat )
			{
				this.opt1.BackColor = System.Drawing.Color.LightGreen;
			}
			else
			{
				this.opt1.BackColor = System.Drawing.Color.Wheat;
			}			
		}
		
		void Opt2Click(object sender, System.EventArgs e)
		{
			if( this.opt2.BackColor == System.Drawing.Color.Wheat )
			{
				this.opt2.BackColor = System.Drawing.Color.LightGreen;
			}
			else
			{
				this.opt2.BackColor = System.Drawing.Color.Wheat;
			}			
		}
		
		void Opt3Click(object sender, System.EventArgs e)
		{
			if( this.opt3.BackColor == System.Drawing.Color.Wheat )
			{
				this.opt3.BackColor = System.Drawing.Color.LightGreen;
			}
			else
			{
				this.opt3.BackColor = System.Drawing.Color.Wheat;
			}			
		}
		
		void Opt4Click(object sender, System.EventArgs e)
		{
			if( this.opt4.BackColor == System.Drawing.Color.Wheat )
			{
				this.opt4.BackColor = System.Drawing.Color.LightGreen;
			}
			else
			{
				this.opt4.BackColor = System.Drawing.Color.Wheat;
			}			
		}
		
		void Opt5Click(object sender, System.EventArgs e)
		{
			if( this.opt5.BackColor == System.Drawing.Color.Wheat )
			{
				this.opt5.BackColor = System.Drawing.Color.LightGreen;
			}
			else
			{
				this.opt5.BackColor = System.Drawing.Color.Wheat;
			}			
		}
		
		void Opt6Click(object sender, System.EventArgs e)
		{
			if( this.opt6.BackColor == System.Drawing.Color.Wheat )
			{
				this.opt6.BackColor = System.Drawing.Color.LightGreen;
			}
			else
			{
				this.opt6.BackColor = System.Drawing.Color.Wheat;
			}
		}
		
		void Opt7Click(object sender, System.EventArgs e)
		{
			if( this.opt7.BackColor == System.Drawing.Color.Wheat )
			{
				this.opt7.BackColor = System.Drawing.Color.LightGreen;
			}
			else
			{
				this.opt7.BackColor = System.Drawing.Color.Wheat;
			}			
		}
		
		void Opt8Click(object sender, System.EventArgs e)
		{
			if( this.opt8.BackColor == System.Drawing.Color.Wheat )
			{
				this.opt8.BackColor = System.Drawing.Color.LightGreen;
			}
			else
			{
				this.opt8.BackColor = System.Drawing.Color.Wheat;
			}
		}
		
		void Opt9Click(object sender, System.EventArgs e)
		{
			if( this.opt9.BackColor == System.Drawing.Color.Wheat )
			{
				this.opt9.BackColor = System.Drawing.Color.LightGreen;
			}
			else
			{
				this.opt9.BackColor = System.Drawing.Color.Wheat;
			}	
		}
				
		
		void Opt0Click(object sender, System.EventArgs e)
		{
			if( this.opt0.BackColor == System.Drawing.Color.Wheat )
			{
				this.opt0.BackColor = System.Drawing.Color.LightGreen;
			}
			else
			{
				this.opt0.BackColor = System.Drawing.Color.Wheat;
			}
			
		}
		void InitializeComponent() {
            this.opt4 = new System.Windows.Forms.Label();
            this.opt6 = new System.Windows.Forms.Label();
            this.opt9 = new System.Windows.Forms.Label();
            this.opt8 = new System.Windows.Forms.Label();
            this.opt7 = new System.Windows.Forms.Label();
            this.opt1 = new System.Windows.Forms.Label();
            this.opt0 = new System.Windows.Forms.Label();
            this.opt3 = new System.Windows.Forms.Label();
            this.opt2 = new System.Windows.Forms.Label();
            this.opt5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // opt4
            // 
            this.opt4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opt4.Location = new System.Drawing.Point(68, 0);
            this.opt4.Name = "opt4";
            this.opt4.Size = new System.Drawing.Size(16, 16);
            this.opt4.TabIndex = 3;
            this.opt4.Text = "4";
            this.opt4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.opt4.Click += new System.EventHandler(this.Opt4Click);
            // 
            // opt6
            // 
            this.opt6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opt6.Location = new System.Drawing.Point(102, 0);
            this.opt6.Name = "opt6";
            this.opt6.Size = new System.Drawing.Size(16, 16);
            this.opt6.TabIndex = 5;
            this.opt6.Text = "6";
            this.opt6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.opt6.Click += new System.EventHandler(this.Opt6Click);
            // 
            // opt9
            // 
            this.opt9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opt9.Location = new System.Drawing.Point(153, 0);
            this.opt9.Name = "opt9";
            this.opt9.Size = new System.Drawing.Size(16, 16);
            this.opt9.TabIndex = 8;
            this.opt9.Text = "9";
            this.opt9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.opt9.Click += new System.EventHandler(this.Opt9Click);
            // 
            // opt8
            // 
            this.opt8.BackColor = System.Drawing.Color.Wheat;
            this.opt8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opt8.Location = new System.Drawing.Point(136, 0);
            this.opt8.Name = "opt8";
            this.opt8.Size = new System.Drawing.Size(16, 16);
            this.opt8.TabIndex = 7;
            this.opt8.Text = "8";
            this.opt8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.opt8.Click += new System.EventHandler(this.Opt8Click);
            // 
            // opt7
            // 
            this.opt7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opt7.Location = new System.Drawing.Point(119, 0);
            this.opt7.Name = "opt7";
            this.opt7.Size = new System.Drawing.Size(16, 16);
            this.opt7.TabIndex = 6;
            this.opt7.Text = "7";
            this.opt7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.opt7.Click += new System.EventHandler(this.Opt7Click);
            // 
            // opt1
            // 
            this.opt1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opt1.Location = new System.Drawing.Point(17, 0);
            this.opt1.Name = "opt1";
            this.opt1.Size = new System.Drawing.Size(16, 16);
            this.opt1.TabIndex = 0;
            this.opt1.Text = "1";
            this.opt1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.opt1.Click += new System.EventHandler(this.Opt1Click);
            // 
            // opt0
            // 
            this.opt0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opt0.Location = new System.Drawing.Point(0, 0);
            this.opt0.Name = "opt0";
            this.opt0.Size = new System.Drawing.Size(16, 16);
            this.opt0.TabIndex = 14;
            this.opt0.Text = "0";
            this.opt0.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.opt0.Click += new System.EventHandler(this.Opt0Click);
            // 
            // opt3
            // 
            this.opt3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opt3.Location = new System.Drawing.Point(51, 0);
            this.opt3.Name = "opt3";
            this.opt3.Size = new System.Drawing.Size(16, 16);
            this.opt3.TabIndex = 2;
            this.opt3.Text = "3";
            this.opt3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.opt3.Click += new System.EventHandler(this.Opt3Click);
            // 
            // opt2
            // 
            this.opt2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opt2.Location = new System.Drawing.Point(34, 0);
            this.opt2.Name = "opt2";
            this.opt2.Size = new System.Drawing.Size(16, 16);
            this.opt2.TabIndex = 1;
            this.opt2.Text = "2";
            this.opt2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.opt2.Click += new System.EventHandler(this.Opt2Click);
            // 
            // opt5
            // 
            this.opt5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opt5.Location = new System.Drawing.Point(85, 0);
            this.opt5.Name = "opt5";
            this.opt5.Size = new System.Drawing.Size(16, 16);
            this.opt5.TabIndex = 4;
            this.opt5.Text = "5";
            this.opt5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.opt5.Click += new System.EventHandler(this.Opt5Click);
            // 
            // OptionNumsHoriz0_9
            // 
            this.BackColor = System.Drawing.Color.Wheat;
            this.Controls.Add(this.opt0);
            this.Controls.Add(this.opt9);
            this.Controls.Add(this.opt8);
            this.Controls.Add(this.opt7);
            this.Controls.Add(this.opt6);
            this.Controls.Add(this.opt5);
            this.Controls.Add(this.opt4);
            this.Controls.Add(this.opt3);
            this.Controls.Add(this.opt2);
            this.Controls.Add(this.opt1);
            this.Name = "OptionNumsHoriz0_9";
            this.Size = new System.Drawing.Size(172, 16);
            this.Load += new System.EventHandler(this.OptionNumsHoriz0_9_Load);
            this.ResumeLayout(false);

		}

        private void OptionNumsHoriz0_9_Load(object sender, EventArgs e)
        {

        }
	}
}
