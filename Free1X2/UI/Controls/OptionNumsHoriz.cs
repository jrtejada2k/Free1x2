using System;
using System.Windows.Forms;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI.Controls
{
	public class OptionNumsHoriz : UserControl
	{
		private Label opt12;
		private Label opt11;
		private Label opt10;
		private Label opt0;
		private Label opt1;
		private Label opt5;
		private Label opt2;
		private Label opt3;
		private Label opt13;
		private Label opt7;
		private Label opt8;
		private Label opt9;
		private Label opt6;
		private Label opt14;
        private Label opt16;
        private Label opt15;
		private Label opt4;
		public OptionNumsHoriz()
		{
			InitializeComponent();
            AdaptarNumeroTolerancias();
		}
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToControl(this);
        }

        protected void AdaptarNumeroTolerancias()
        {
            int noP;
            try
            {
                noP = VariablesGlobales.NumeroPartidos;
            }
            catch
            {
                noP = 15;
            }
            Label[] arrayLbls = new Label[] { opt0, opt1, opt2, opt3, opt4, opt5, opt6, opt7, opt8, opt9, opt10, opt11, opt12, opt13, opt14, opt15, opt16 };
            for (int i = 0; i < arrayLbls.Length; i++)
            {
                Label lbl = arrayLbls[i];

                if (i <= noP)
                {
                    lbl.Visible = true;
                }
                else
                {
                    lbl.Visible = false;
                }
            }
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
				
				if(opt10.BackColor == System.Drawing.Color.LightGreen)
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
					valores += "10";
				}
				
				if(opt11.BackColor == System.Drawing.Color.LightGreen)
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
					valores += "11";
				}
				if(opt12.BackColor == System.Drawing.Color.LightGreen)
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
					valores += "12";
				}
				if(opt13.BackColor == System.Drawing.Color.LightGreen)
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
					valores += "13";
				}
				if(opt14.BackColor == System.Drawing.Color.LightGreen)
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
					valores += "14";
				}
                if (opt15.BackColor == System.Drawing.Color.LightGreen)
                {
                    if (!valores.Equals(""))
                    {
                        valores += ",";
                    }
                    valores += "15";
                } if (opt16.BackColor == System.Drawing.Color.LightGreen)
                {
                    if (!valores.Equals(""))
                    {
                        valores += ",";
                    }
                    valores += "16";
                }
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
				opt10.BackColor = System.Drawing.Color.Wheat;
				opt11.BackColor = System.Drawing.Color.Wheat;
				opt12.BackColor = System.Drawing.Color.Wheat;
				opt13.BackColor = System.Drawing.Color.Wheat;
				opt14.BackColor = System.Drawing.Color.Wheat;
                opt15.BackColor = System.Drawing.Color.Wheat;
                opt16.BackColor = System.Drawing.Color.Wheat;

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
						case "10":
							opt10.BackColor = System.Drawing.Color.LightGreen;
							break;
						case "11":
							opt11.BackColor = System.Drawing.Color.LightGreen;
							break;
						case "12":
							opt12.BackColor = System.Drawing.Color.LightGreen;
							break;
						case "13":
							opt13.BackColor = System.Drawing.Color.LightGreen;
							break;
						case "14":
							opt14.BackColor = System.Drawing.Color.LightGreen;
							break;
                        case "15":
                            opt15.BackColor = System.Drawing.Color.LightGreen;
                            break;
                        case "16":
                            opt16.BackColor = System.Drawing.Color.LightGreen;
                            break;
					}
				
				}			
					
			}
		}

		void Opt1Click(object sender, EventArgs e)
		{
			if( opt1.BackColor == System.Drawing.Color.Wheat )
			{
				opt1.BackColor = System.Drawing.Color.LightGreen;
			}
			else
			{
				opt1.BackColor = System.Drawing.Color.Wheat;
			}			
		}
		
		void Opt2Click(object sender, EventArgs e)
		{
			if( opt2.BackColor == System.Drawing.Color.Wheat )
			{
				opt2.BackColor = System.Drawing.Color.LightGreen;
			}
			else
			{
				opt2.BackColor = System.Drawing.Color.Wheat;
			}			
		}
		
		void Opt3Click(object sender, EventArgs e)
		{
			if( opt3.BackColor == System.Drawing.Color.Wheat )
			{
				opt3.BackColor = System.Drawing.Color.LightGreen;
			}
			else
			{
				opt3.BackColor = System.Drawing.Color.Wheat;
			}			
		}
		
		void Opt4Click(object sender, EventArgs e)
		{
			if( opt4.BackColor == System.Drawing.Color.Wheat )
			{
				opt4.BackColor = System.Drawing.Color.LightGreen;
			}
			else
			{
				opt4.BackColor = System.Drawing.Color.Wheat;
			}			
		}
		
		void Opt5Click(object sender, EventArgs e)
		{
			if( opt5.BackColor == System.Drawing.Color.Wheat )
			{
				opt5.BackColor = System.Drawing.Color.LightGreen;
			}
			else
			{
				opt5.BackColor = System.Drawing.Color.Wheat;
			}			
		}
		
		void Opt6Click(object sender,EventArgs e)
		{
			if( opt6.BackColor == System.Drawing.Color.Wheat )
			{
				opt6.BackColor = System.Drawing.Color.LightGreen;
			}
			else
			{
				opt6.BackColor = System.Drawing.Color.Wheat;
			}
		}
		
		void Opt7Click(object sender, EventArgs e)
		{
			if( opt7.BackColor == System.Drawing.Color.Wheat )
			{
				opt7.BackColor = System.Drawing.Color.LightGreen;
			}
			else
			{
				opt7.BackColor = System.Drawing.Color.Wheat;
			}			
		}
		
		void Opt8Click(object sender, EventArgs e)
		{
			if( opt8.BackColor == System.Drawing.Color.Wheat )
			{
				opt8.BackColor = System.Drawing.Color.LightGreen;
			}
			else
			{
				opt8.BackColor = System.Drawing.Color.Wheat;
			}
		}
		
		void Opt9Click(object sender, EventArgs e)
		{
			if( opt9.BackColor == System.Drawing.Color.Wheat )
			{
				opt9.BackColor = System.Drawing.Color.LightGreen;
			}
			else
			{
				opt9.BackColor = System.Drawing.Color.Wheat;
			}	
		}
		
		void Opt10Click(object sender,EventArgs e)
		{
			if( opt10.BackColor == System.Drawing.Color.Wheat )
			{
				opt10.BackColor = System.Drawing.Color.LightGreen;
			}
			else
			{
				opt10.BackColor = System.Drawing.Color.Wheat;
			}
		}
		
		void Opt11Click(object sender, EventArgs e)
		{
			if( opt11.BackColor == System.Drawing.Color.Wheat )
			{
				opt11.BackColor = System.Drawing.Color.LightGreen;
			}
			else
			{
				opt11.BackColor = System.Drawing.Color.Wheat;
			}
		}
		
		void Opt12Click(object sender, EventArgs e)
		{
			if( opt12.BackColor == System.Drawing.Color.Wheat )
			{
				opt12.BackColor = System.Drawing.Color.LightGreen;
			}
			else
			{
				opt12.BackColor = System.Drawing.Color.Wheat;
			}
		}
		
		void Opt13Click(object sender, EventArgs e)
		{
			if( opt13.BackColor == System.Drawing.Color.Wheat )
			{
				opt13.BackColor = System.Drawing.Color.LightGreen;
			}
			else
			{
				opt13.BackColor = System.Drawing.Color.Wheat;
			}
		}
		
		void Opt14Click(object sender, EventArgs e)
		{
			if( opt14.BackColor == System.Drawing.Color.Wheat )
			{
				opt14.BackColor = System.Drawing.Color.LightGreen;
			}
			else
			{
				opt14.BackColor = System.Drawing.Color.Wheat;
			}
		}
		
		void Opt0Click(object sender, EventArgs e)
		{
			if( opt0.BackColor == System.Drawing.Color.Wheat )
			{
				opt0.BackColor = System.Drawing.Color.LightGreen;
			}
			else
			{
				opt0.BackColor = System.Drawing.Color.Wheat;
			}
			
		}
		
		void InitializeComponent() {
            this.opt4 = new System.Windows.Forms.Label();
            this.opt14 = new System.Windows.Forms.Label();
            this.opt6 = new System.Windows.Forms.Label();
            this.opt9 = new System.Windows.Forms.Label();
            this.opt8 = new System.Windows.Forms.Label();
            this.opt7 = new System.Windows.Forms.Label();
            this.opt13 = new System.Windows.Forms.Label();
            this.opt3 = new System.Windows.Forms.Label();
            this.opt2 = new System.Windows.Forms.Label();
            this.opt5 = new System.Windows.Forms.Label();
            this.opt1 = new System.Windows.Forms.Label();
            this.opt0 = new System.Windows.Forms.Label();
            this.opt10 = new System.Windows.Forms.Label();
            this.opt11 = new System.Windows.Forms.Label();
            this.opt12 = new System.Windows.Forms.Label();
            this.opt16 = new System.Windows.Forms.Label();
            this.opt15 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // opt4
            // 
            this.opt4.BackColor = System.Drawing.Color.Wheat;
            this.opt4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opt4.Location = new System.Drawing.Point(92, 0);
            this.opt4.Name = "opt4";
            this.opt4.Size = new System.Drawing.Size(22, 16);
            this.opt4.TabIndex = 3;
            this.opt4.Text = "4";
            this.opt4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.opt4.Click += new System.EventHandler(this.Opt4Click);
            // 
            // opt14
            // 
            this.opt14.BackColor = System.Drawing.Color.Wheat;
            this.opt14.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opt14.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.opt14.Location = new System.Drawing.Point(322, 0);
            this.opt14.Name = "opt14";
            this.opt14.Size = new System.Drawing.Size(22, 16);
            this.opt14.TabIndex = 13;
            this.opt14.Text = "14";
            this.opt14.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.opt14.Click += new System.EventHandler(this.Opt14Click);
            // 
            // opt6
            // 
            this.opt6.BackColor = System.Drawing.Color.Wheat;
            this.opt6.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opt6.Location = new System.Drawing.Point(138, 0);
            this.opt6.Name = "opt6";
            this.opt6.Size = new System.Drawing.Size(22, 16);
            this.opt6.TabIndex = 5;
            this.opt6.Text = "6";
            this.opt6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.opt6.Click += new System.EventHandler(this.Opt6Click);
            // 
            // opt9
            // 
            this.opt9.BackColor = System.Drawing.Color.Wheat;
            this.opt9.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opt9.Location = new System.Drawing.Point(207, 0);
            this.opt9.Name = "opt9";
            this.opt9.Size = new System.Drawing.Size(22, 16);
            this.opt9.TabIndex = 8;
            this.opt9.Text = "9";
            this.opt9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.opt9.Click += new System.EventHandler(this.Opt9Click);
            // 
            // opt8
            // 
            this.opt8.BackColor = System.Drawing.Color.Wheat;
            this.opt8.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opt8.Location = new System.Drawing.Point(184, 0);
            this.opt8.Name = "opt8";
            this.opt8.Size = new System.Drawing.Size(22, 16);
            this.opt8.TabIndex = 7;
            this.opt8.Text = "8";
            this.opt8.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.opt8.Click += new System.EventHandler(this.Opt8Click);
            // 
            // opt7
            // 
            this.opt7.BackColor = System.Drawing.Color.Wheat;
            this.opt7.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opt7.Location = new System.Drawing.Point(161, 0);
            this.opt7.Name = "opt7";
            this.opt7.Size = new System.Drawing.Size(22, 16);
            this.opt7.TabIndex = 6;
            this.opt7.Text = "7";
            this.opt7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.opt7.Click += new System.EventHandler(this.Opt7Click);
            // 
            // opt13
            // 
            this.opt13.BackColor = System.Drawing.Color.Wheat;
            this.opt13.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opt13.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.opt13.Location = new System.Drawing.Point(299, 0);
            this.opt13.Name = "opt13";
            this.opt13.Size = new System.Drawing.Size(22, 16);
            this.opt13.TabIndex = 12;
            this.opt13.Text = "13";
            this.opt13.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.opt13.Click += new System.EventHandler(this.Opt13Click);
            // 
            // opt3
            // 
            this.opt3.BackColor = System.Drawing.Color.Wheat;
            this.opt3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opt3.Location = new System.Drawing.Point(69, 0);
            this.opt3.Name = "opt3";
            this.opt3.Size = new System.Drawing.Size(22, 16);
            this.opt3.TabIndex = 2;
            this.opt3.Text = "3";
            this.opt3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.opt3.Click += new System.EventHandler(this.Opt3Click);
            // 
            // opt2
            // 
            this.opt2.BackColor = System.Drawing.Color.Wheat;
            this.opt2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opt2.Location = new System.Drawing.Point(46, 0);
            this.opt2.Name = "opt2";
            this.opt2.Size = new System.Drawing.Size(22, 16);
            this.opt2.TabIndex = 1;
            this.opt2.Text = "2";
            this.opt2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.opt2.Click += new System.EventHandler(this.Opt2Click);
            // 
            // opt5
            // 
            this.opt5.BackColor = System.Drawing.Color.Wheat;
            this.opt5.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opt5.Location = new System.Drawing.Point(115, 0);
            this.opt5.Name = "opt5";
            this.opt5.Size = new System.Drawing.Size(22, 16);
            this.opt5.TabIndex = 4;
            this.opt5.Text = "5";
            this.opt5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.opt5.Click += new System.EventHandler(this.Opt5Click);
            // 
            // opt1
            // 
            this.opt1.BackColor = System.Drawing.Color.Wheat;
            this.opt1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opt1.Location = new System.Drawing.Point(23, 0);
            this.opt1.Name = "opt1";
            this.opt1.Size = new System.Drawing.Size(22, 16);
            this.opt1.TabIndex = 0;
            this.opt1.Text = "1";
            this.opt1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.opt1.Click += new System.EventHandler(this.Opt1Click);
            // 
            // opt0
            // 
            this.opt0.BackColor = System.Drawing.Color.Wheat;
            this.opt0.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opt0.Location = new System.Drawing.Point(0, 0);
            this.opt0.Name = "opt0";
            this.opt0.Size = new System.Drawing.Size(22, 16);
            this.opt0.TabIndex = 14;
            this.opt0.Text = "0";
            this.opt0.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.opt0.Click += new System.EventHandler(this.Opt0Click);
            // 
            // opt10
            // 
            this.opt10.BackColor = System.Drawing.Color.Wheat;
            this.opt10.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opt10.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.opt10.Location = new System.Drawing.Point(230, 0);
            this.opt10.Name = "opt10";
            this.opt10.Size = new System.Drawing.Size(22, 16);
            this.opt10.TabIndex = 9;
            this.opt10.Text = "10";
            this.opt10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.opt10.Click += new System.EventHandler(this.Opt10Click);
            // 
            // opt11
            // 
            this.opt11.BackColor = System.Drawing.Color.Wheat;
            this.opt11.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opt11.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.opt11.Location = new System.Drawing.Point(253, 0);
            this.opt11.Name = "opt11";
            this.opt11.Size = new System.Drawing.Size(22, 16);
            this.opt11.TabIndex = 10;
            this.opt11.Text = "11";
            this.opt11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.opt11.Click += new System.EventHandler(this.Opt11Click);
            // 
            // opt12
            // 
            this.opt12.BackColor = System.Drawing.Color.Wheat;
            this.opt12.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opt12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.opt12.Location = new System.Drawing.Point(276, 0);
            this.opt12.Name = "opt12";
            this.opt12.Size = new System.Drawing.Size(22, 16);
            this.opt12.TabIndex = 11;
            this.opt12.Text = "12";
            this.opt12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.opt12.Click += new System.EventHandler(this.Opt12Click);
            // 
            // opt16
            // 
            this.opt16.BackColor = System.Drawing.Color.Wheat;
            this.opt16.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opt16.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.opt16.Location = new System.Drawing.Point(368, 0);
            this.opt16.Name = "opt16";
            this.opt16.Size = new System.Drawing.Size(22, 16);
            this.opt16.TabIndex = 16;
            this.opt16.Text = "16";
            this.opt16.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.opt16.Click += new System.EventHandler(this.opt16_Click);
            // 
            // opt15
            // 
            this.opt15.BackColor = System.Drawing.Color.Wheat;
            this.opt15.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.opt15.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.opt15.Location = new System.Drawing.Point(345, 0);
            this.opt15.Name = "opt15";
            this.opt15.Size = new System.Drawing.Size(22, 16);
            this.opt15.TabIndex = 15;
            this.opt15.Text = "15";
            this.opt15.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.opt15.Click += new System.EventHandler(this.opt15_Click);
            // 
            // OptionNumsHoriz
            // 
            this.BackColor = System.Drawing.Color.Bisque;
            this.Controls.Add(this.opt16);
            this.Controls.Add(this.opt15);
            this.Controls.Add(this.opt0);
            this.Controls.Add(this.opt14);
            this.Controls.Add(this.opt13);
            this.Controls.Add(this.opt12);
            this.Controls.Add(this.opt11);
            this.Controls.Add(this.opt10);
            this.Controls.Add(this.opt9);
            this.Controls.Add(this.opt8);
            this.Controls.Add(this.opt7);
            this.Controls.Add(this.opt6);
            this.Controls.Add(this.opt5);
            this.Controls.Add(this.opt4);
            this.Controls.Add(this.opt3);
            this.Controls.Add(this.opt2);
            this.Controls.Add(this.opt1);
            this.Name = "OptionNumsHoriz";
            this.Size = new System.Drawing.Size(441, 16);
            this.ResumeLayout(false);

		}

        private void opt15_Click(object sender, EventArgs e)
        {
            if (opt15.BackColor == System.Drawing.Color.Wheat)
            {
                opt15.BackColor = System.Drawing.Color.LightGreen;
            }
            else
            {
                opt15.BackColor = System.Drawing.Color.Wheat;
            }
        }

        private void opt16_Click(object sender, EventArgs e)
        {
            if (opt16.BackColor == System.Drawing.Color.Wheat)
            {
                opt16.BackColor = System.Drawing.Color.LightGreen;
            }
            else
            {
                opt16.BackColor = System.Drawing.Color.Wheat;
            }
        }
	}
}
