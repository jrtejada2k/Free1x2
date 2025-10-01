using System;
using System.Windows.Forms;

namespace Free1X2.UI.Controls
{
	public class OptionNumTol0_13 : System.Windows.Forms.UserControl
	{
		private Free1X2.UI.Controls.BotonValorNum btnValor9;
		private Free1X2.UI.Controls.BotonValorNum btnValor8;
		private Free1X2.UI.Controls.BotonValorNum btnValor12;
		private Free1X2.UI.Controls.BotonValorNum btnValor13;
		private Free1X2.UI.Controls.BotonValorNum btnValor10;
		private Free1X2.UI.Controls.BotonValorNum btnValor11;
		private Free1X2.UI.Controls.BotonValorNum btnValor3;
		private Free1X2.UI.Controls.BotonValorNum btnValor2;
		private Free1X2.UI.Controls.BotonValorNum btnValor1;
		private Free1X2.UI.Controls.BotonValorNum btnValor0;
		private Free1X2.UI.Controls.BotonValorNum btnValor7;
		private Free1X2.UI.Controls.BotonValorNum btnValor6;
		private Free1X2.UI.Controls.BotonValorNum btnValor5;
		private System.Windows.Forms.Button btnSelTodos;
		private System.Windows.Forms.Button btnUnselTodos;
		private Free1X2.UI.Controls.BotonValorNum btnValor4;
		
		
		public OptionNumTol0_13()
		{
			InitializeComponent();
		}
		
		public string Valores
		{
			get
			{
				string valores = "";
				
				
				if(btnValor0.BotonActivo)
				{
					valores = btnValor0.ValorCombinado;
				}
								
				if(btnValor1.BotonActivo)
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
					valores += btnValor1.ValorCombinado;
				}
				
				if(btnValor2.BotonActivo)
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
					valores += btnValor2.ValorCombinado;
				}
				
				if(btnValor3.BotonActivo)
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
					valores += btnValor3.ValorCombinado;
				}
				
				if(btnValor4.BotonActivo)
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
					valores += btnValor4.ValorCombinado;
				}
				
				if(btnValor5.BotonActivo)
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
					valores += btnValor5.ValorCombinado;
				}
				
				if(btnValor6.BotonActivo)
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
					valores += btnValor6.ValorCombinado;
				}
				
				if(btnValor7.BotonActivo)
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
					valores += btnValor7.ValorCombinado;
				}
				
				if(btnValor8.BotonActivo)
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
					valores += btnValor8.ValorCombinado;
				}
				
				if(btnValor9.BotonActivo)
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
					valores += btnValor9.ValorCombinado;
				}
				
				if(btnValor10.BotonActivo)
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
					valores += btnValor10.ValorCombinado;
				}
				
				if(btnValor11.BotonActivo)
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
					valores += btnValor11.ValorCombinado;
				}
				if(btnValor12.BotonActivo)
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
					valores += btnValor12.ValorCombinado;
				}
				if(btnValor13.BotonActivo)
				{
					if( !valores.Equals("") )
					{
						valores += ",";
					}
					valores += btnValor13.ValorCombinado;
				}
				
				return valores;
			}
			set
			{
				string valores = value;
				string[] valArray = valores.Split(',');				
				
				//reinicializa valores
				btnValor0.Reinizializa();
				btnValor1.Reinizializa();
				btnValor2.Reinizializa();
				btnValor3.Reinizializa();
				btnValor4.Reinizializa();
				btnValor5.Reinizializa();
				btnValor6.Reinizializa();
				btnValor7.Reinizializa();
				btnValor8.Reinizializa();
				btnValor9.Reinizializa();
				btnValor10.Reinizializa();
				btnValor11.Reinizializa();
				btnValor12.Reinizializa();
				btnValor13.Reinizializa();
				
				foreach(string val in valArray)
				{
					if(val != "")
					{
						switch( ObtenNumeroCasilla(val) )
						{
							case "0":
								btnValor0.ValorCombinado = val;
								break;
							case "1":
								btnValor1.ValorCombinado = val;
								break;
							case "2":
								btnValor2.ValorCombinado = val;
								break;
							case "3":
								btnValor3.ValorCombinado = val;
								break;
							case "4":
								btnValor4.ValorCombinado = val;
								break;
							case "5":
								btnValor5.ValorCombinado = val;
								break;
							case "6":
								btnValor6.ValorCombinado = val;
								break;
							case "7":
								btnValor7.ValorCombinado = val;
								break;
							case "8":
								btnValor8.ValorCombinado = val;
								break;
							case "9":
								btnValor9.ValorCombinado = val;
								break;
							case "10":
								btnValor10.ValorCombinado = val;
								break;
							case "11":
								btnValor11.ValorCombinado = val;
								break;
							case "12":
								btnValor12.ValorCombinado = val;
								break;
							case "13":
								btnValor13.ValorCombinado = val;
								break;
						}
					}
				}			
			}
		}
		
		protected string ObtenNumeroCasilla(string valorCasilla)
		{
			string valor= "";
			
			if(Char.IsLetter(valorCasilla[valorCasilla.Length-1]))
			{
				valor = valorCasilla.Substring(0,valorCasilla.Length-1);
			}
			else
			{
				valor = valorCasilla;				
			}
				
			return valor;
		
		}
		
		
		
		void InitializeComponent() {
			this.btnValor4 = new Free1X2.UI.Controls.BotonValorNum();
			this.btnValor5 = new Free1X2.UI.Controls.BotonValorNum();
			this.btnValor6 = new Free1X2.UI.Controls.BotonValorNum();
			this.btnValor7 = new Free1X2.UI.Controls.BotonValorNum();
			this.btnValor0 = new Free1X2.UI.Controls.BotonValorNum();
			this.btnValor1 = new Free1X2.UI.Controls.BotonValorNum();
			this.btnValor2 = new Free1X2.UI.Controls.BotonValorNum();
			this.btnValor3 = new Free1X2.UI.Controls.BotonValorNum();
			this.btnValor11 = new Free1X2.UI.Controls.BotonValorNum();
			this.btnValor10 = new Free1X2.UI.Controls.BotonValorNum();
			this.btnValor13 = new Free1X2.UI.Controls.BotonValorNum();
			this.btnValor12 = new Free1X2.UI.Controls.BotonValorNum();
			this.btnValor8 = new Free1X2.UI.Controls.BotonValorNum();
			this.btnValor9 = new Free1X2.UI.Controls.BotonValorNum();
			this.btnSelTodos = new System.Windows.Forms.Button();
			this.btnUnselTodos = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// btnValor4
			// 
			this.btnValor4.BackColor = System.Drawing.Color.Wheat;
			this.btnValor4.LetraTolerancia = "";
			this.btnValor4.Location = new System.Drawing.Point(144, 0);
			this.btnValor4.Name = "btnValor4";
			this.btnValor4.Size = new System.Drawing.Size(32, 16);
			this.btnValor4.TabIndex = 18;
			this.btnValor4.ValorCombinado = "4";
			this.btnValor4.ValorNumerico = "4";
			// 
			// btnValor5
			// 
			this.btnValor5.BackColor = System.Drawing.Color.Wheat;
			this.btnValor5.LetraTolerancia = "";
			this.btnValor5.Location = new System.Drawing.Point(176, 0);
			this.btnValor5.Name = "btnValor5";
			this.btnValor5.Size = new System.Drawing.Size(32, 16);
			this.btnValor5.TabIndex = 19;
			this.btnValor5.ValorCombinado = "5";
			this.btnValor5.ValorNumerico = "5";
			// 
			// btnValor6
			// 
			this.btnValor6.BackColor = System.Drawing.Color.Wheat;
			this.btnValor6.LetraTolerancia = "";
			this.btnValor6.Location = new System.Drawing.Point(208, 0);
			this.btnValor6.Name = "btnValor6";
			this.btnValor6.Size = new System.Drawing.Size(32, 16);
			this.btnValor6.TabIndex = 20;
			this.btnValor6.ValorCombinado = "6";
			this.btnValor6.ValorNumerico = "6";
			// 
			// btnValor7
			// 
			this.btnValor7.BackColor = System.Drawing.Color.Wheat;
			this.btnValor7.LetraTolerancia = "";
			this.btnValor7.Location = new System.Drawing.Point(240, 0);
			this.btnValor7.Name = "btnValor7";
			this.btnValor7.Size = new System.Drawing.Size(32, 16);
			this.btnValor7.TabIndex = 21;
			this.btnValor7.ValorCombinado = "7";
			this.btnValor7.ValorNumerico = "7";
			// 
			// btnValor0
			// 
			this.btnValor0.BackColor = System.Drawing.Color.Wheat;
			this.btnValor0.LetraTolerancia = "";
			this.btnValor0.Location = new System.Drawing.Point(16, 0);
			this.btnValor0.Name = "btnValor0";
			this.btnValor0.Size = new System.Drawing.Size(32, 16);
			this.btnValor0.TabIndex = 15;
			this.btnValor0.ValorCombinado = "0";
			this.btnValor0.ValorNumerico = "0";
			// 
			// btnValor1
			// 
			this.btnValor1.BackColor = System.Drawing.Color.Wheat;
			this.btnValor1.LetraTolerancia = "";
			this.btnValor1.Location = new System.Drawing.Point(48, 0);
			this.btnValor1.Name = "btnValor1";
			this.btnValor1.Size = new System.Drawing.Size(32, 16);
			this.btnValor1.TabIndex = 15;
			this.btnValor1.ValorCombinado = "1";
			this.btnValor1.ValorNumerico = "1";
			// 
			// btnValor2
			// 
			this.btnValor2.BackColor = System.Drawing.Color.Wheat;
			this.btnValor2.LetraTolerancia = "";
			this.btnValor2.Location = new System.Drawing.Point(80, 0);
			this.btnValor2.Name = "btnValor2";
			this.btnValor2.Size = new System.Drawing.Size(32, 16);
			this.btnValor2.TabIndex = 16;
			this.btnValor2.ValorCombinado = "2";
			this.btnValor2.ValorNumerico = "2";
			// 
			// btnValor3
			// 
			this.btnValor3.BackColor = System.Drawing.Color.Wheat;
			this.btnValor3.LetraTolerancia = "";
			this.btnValor3.Location = new System.Drawing.Point(112, 0);
			this.btnValor3.Name = "btnValor3";
			this.btnValor3.Size = new System.Drawing.Size(32, 16);
			this.btnValor3.TabIndex = 17;
			this.btnValor3.ValorCombinado = "3";
			this.btnValor3.ValorNumerico = "3";
			// 
			// btnValor11
			// 
			this.btnValor11.BackColor = System.Drawing.Color.Wheat;
			this.btnValor11.LetraTolerancia = "";
			this.btnValor11.Location = new System.Drawing.Point(368, 0);
			this.btnValor11.Name = "btnValor11";
			this.btnValor11.Size = new System.Drawing.Size(32, 16);
			this.btnValor11.TabIndex = 25;
			this.btnValor11.ValorCombinado = "11";
			this.btnValor11.ValorNumerico = "11";
			// 
			// btnValor10
			// 
			this.btnValor10.BackColor = System.Drawing.Color.Wheat;
			this.btnValor10.LetraTolerancia = "";
			this.btnValor10.Location = new System.Drawing.Point(336, 0);
			this.btnValor10.Name = "btnValor10";
			this.btnValor10.Size = new System.Drawing.Size(32, 16);
			this.btnValor10.TabIndex = 24;
			this.btnValor10.ValorCombinado = "10";
			this.btnValor10.ValorNumerico = "10";
			// 
			// btnValor13
			// 
			this.btnValor13.BackColor = System.Drawing.Color.Wheat;
			this.btnValor13.LetraTolerancia = "";
			this.btnValor13.Location = new System.Drawing.Point(432, 0);
			this.btnValor13.Name = "btnValor13";
			this.btnValor13.Size = new System.Drawing.Size(32, 16);
			this.btnValor13.TabIndex = 27;
			this.btnValor13.ValorCombinado = "13";
			this.btnValor13.ValorNumerico = "13";
			// 
			// btnValor12
			// 
			this.btnValor12.BackColor = System.Drawing.Color.Wheat;
			this.btnValor12.LetraTolerancia = "";
			this.btnValor12.Location = new System.Drawing.Point(400, 0);
			this.btnValor12.Name = "btnValor12";
			this.btnValor12.Size = new System.Drawing.Size(32, 16);
			this.btnValor12.TabIndex = 26;
			this.btnValor12.ValorCombinado = "12";
			this.btnValor12.ValorNumerico = "12";
			// 
			// btnValor8
			// 
			this.btnValor8.BackColor = System.Drawing.Color.Wheat;
			this.btnValor8.LetraTolerancia = "";
			this.btnValor8.Location = new System.Drawing.Point(272, 0);
			this.btnValor8.Name = "btnValor8";
			this.btnValor8.Size = new System.Drawing.Size(32, 16);
			this.btnValor8.TabIndex = 22;
			this.btnValor8.ValorCombinado = "8";
			this.btnValor8.ValorNumerico = "8";
			// 
			// btnValor9
			// 
			this.btnValor9.BackColor = System.Drawing.Color.Wheat;
			this.btnValor9.LetraTolerancia = "";
			this.btnValor9.Location = new System.Drawing.Point(304, 0);
			this.btnValor9.Name = "btnValor9";
			this.btnValor9.Size = new System.Drawing.Size(32, 16);
			this.btnValor9.TabIndex = 23;
			this.btnValor9.ValorCombinado = "9";
			this.btnValor9.ValorNumerico = "9";
			// 
			// btnSelTodos
			// 
			this.btnSelTodos.BackColor = System.Drawing.Color.DarkSalmon;
			this.btnSelTodos.Location = new System.Drawing.Point(0, 0);
			this.btnSelTodos.Name = "btnSelTodos";
			this.btnSelTodos.Size = new System.Drawing.Size(16, 16);
			this.btnSelTodos.TabIndex = 28;
			this.btnSelTodos.Text = "+";
			this.btnSelTodos.Click += new System.EventHandler(this.btnSelTodos_Click);
			// 
			// btnUnselTodos
			// 
			this.btnUnselTodos.BackColor = System.Drawing.Color.DarkSalmon;
			this.btnUnselTodos.Location = new System.Drawing.Point(464, 0);
			this.btnUnselTodos.Name = "btnUnselTodos";
			this.btnUnselTodos.Size = new System.Drawing.Size(16, 16);
			this.btnUnselTodos.TabIndex = 29;
			this.btnUnselTodos.Text = "-";
			this.btnUnselTodos.Click += new System.EventHandler(this.btnUnselTodos_Click);
			// 
			// OptionNumTol0_13
			// 
			this.BackColor = System.Drawing.Color.Wheat;
			this.Controls.Add(this.btnUnselTodos);
			this.Controls.Add(this.btnSelTodos);
			this.Controls.Add(this.btnValor0);
			this.Controls.Add(this.btnValor1);
			this.Controls.Add(this.btnValor2);
			this.Controls.Add(this.btnValor3);
			this.Controls.Add(this.btnValor4);
			this.Controls.Add(this.btnValor5);
			this.Controls.Add(this.btnValor6);
			this.Controls.Add(this.btnValor7);
			this.Controls.Add(this.btnValor8);
			this.Controls.Add(this.btnValor9);
			this.Controls.Add(this.btnValor10);
			this.Controls.Add(this.btnValor11);
			this.Controls.Add(this.btnValor12);
			this.Controls.Add(this.btnValor13);
			this.Name = "OptionNumTol0_13";
			this.Size = new System.Drawing.Size(480, 16);
			this.ResumeLayout(false);

		}

		private void btnSelTodos_Click(object sender, System.EventArgs e)
		{
			this.Valores = "0,1,2,3,4,5,6,7,8,9,10,11,12,13";
		}

		private void btnUnselTodos_Click(object sender, System.EventArgs e)
		{
			this.Valores = "";
		}
		
		
	}
}
