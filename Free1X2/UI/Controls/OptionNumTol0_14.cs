using System;
using System.Windows.Forms;

namespace Free1X2.UI.Controls
{
	public class OptionNumTol0_14 : UserControl
	{
		private const int anchoBoton=32;
		private BotonValorNum btnValor9;
		private BotonValorNum btnValor8;
		private BotonValorNum btnValor12;
		private BotonValorNum btnValor13;
		private BotonValorNum btnValor10;
		private BotonValorNum btnValor11;
		private BotonValorNum btnValor14;
		private BotonValorNum btnValor3;
		private BotonValorNum btnValor2;
		private BotonValorNum btnValor1;
		private BotonValorNum btnValor0;
		private BotonValorNum btnValor7;
		private BotonValorNum btnValor6;
		private BotonValorNum btnValor5;
		private Button btnSelTodos;
		private Button btnUnselTodos;
		private BotonValorNum btnValor4;
		private int minimo;
		private BotonValorNum btnValor15;
		private BotonValorNum btnValor16;
        private int maximo;
        bool especial;
        private BotonValorNum[] arrayControles;

	    public OptionNumTol0_14(bool esp)
        {
            especial = esp;
            try
            {
                maximo = VariablesGlobales.NumeroPartidos - 1;
            }
            catch
            {
                maximo = 15;
            }

            InitializeComponent();
            InicializaArraysControles();
        }

        public OptionNumTol0_14()
        {
            try
            {
                maximo = VariablesGlobales.NumeroPartidos;
            }
            catch
            {
                maximo = 15;
            }
            InitializeComponent();
            InicializaArraysControles();
        }
        private void InicializaArraysControles()
        {
            arrayControles = new BotonValorNum[] {btnValor0, btnValor1, btnValor2, 
                btnValor3, btnValor4, btnValor5, btnValor6, 
                btnValor7, btnValor8, btnValor9, btnValor10, 
                btnValor11, btnValor12, btnValor13, btnValor14, 
                btnValor15, btnValor16 };
        }
		public int Minimo
		{
			get{return minimo;}
			set
			{
				minimo=value;
				cambiarControl();
			}
		}

        public int Maximo
        {
            get
            {
                try
                {
                    if (especial)
                    {
                        maximo = VariablesGlobales.NumeroPartidos - 1;
                    }
                    else
                    {
                        maximo = VariablesGlobales.NumeroPartidos;
                    }
                }
                catch
                {
                    maximo = 15;
                }
                return maximo;
            }
            set
            {
                maximo = value;
                cambiarControl();
            }
        }

		public string Valores
		{
			get
			{
				string valores = "";
				BotonValorNum boton;
                for (int i = Minimo; i <= Maximo; i++)
                {
                    boton = BuscarControl(i);
                    if (boton.BotonActivo)
                    {
                        if (!valores.Equals("")) valores += ",";
                        valores += boton.ValorCombinado;
                    }

                }
			    return valores;
			}
			set
			{
				string valores = value;
				string[] valArray = valores.Split(',');
			    BotonValorNum boton;
				for(int i=Minimo;i<=Maximo;i++)
				{
					boton=BuscarControl(i);
					if(boton!=null) boton.Reinizializa();
				}
			    for (int i = 0; i < valArray.Length; i++)
			    {
			        string val = valArray[i];
			        if (val != "")
			        {
			            int num = Convert.ToInt16(ObtenNumeroCasilla(val));
			            boton = BuscarControl(num);
			            if (boton != null) boton.ValorCombinado = val;
			        }
			    }
			}
		}
		
		private void cambiarControl()
		{
			int x=btnSelTodos.Location.X +btnSelTodos.Size.Width +1;
            for (int i = 0; i <= 16; i++)
            {
                BotonValorNum b = BuscarControl(i);
                if (i < Minimo || i > Maximo)
                {
                    b.Visible = false;
                }
                else
                {
                    b.Visible = true;
                    b.Location = new System.Drawing.Point(x, 0);
                    x += anchoBoton + 1;
                }

            }
		    // Ajusta el ancho del control
			btnUnselTodos.Location=new System.Drawing.Point(x,0);
			Size=new System.Drawing.Size(x+16,Height);
		}

		protected string ObtenNumeroCasilla(string valorCasilla)
		{
		    if(Char.IsLetter(valorCasilla[valorCasilla.Length-1]))
			{
				return valorCasilla.Substring(0,valorCasilla.Length-1);
			}
		    return valorCasilla;
		}
		
		
		
		void InitializeComponent() {
            this.btnSelTodos = new System.Windows.Forms.Button();
            this.btnUnselTodos = new System.Windows.Forms.Button();
            this.btnValor15 = new Free1X2.UI.Controls.BotonValorNum();
            this.btnValor16 = new Free1X2.UI.Controls.BotonValorNum();
            this.btnValor0 = new Free1X2.UI.Controls.BotonValorNum();
            this.btnValor1 = new Free1X2.UI.Controls.BotonValorNum();
            this.btnValor2 = new Free1X2.UI.Controls.BotonValorNum();
            this.btnValor3 = new Free1X2.UI.Controls.BotonValorNum();
            this.btnValor4 = new Free1X2.UI.Controls.BotonValorNum();
            this.btnValor5 = new Free1X2.UI.Controls.BotonValorNum();
            this.btnValor6 = new Free1X2.UI.Controls.BotonValorNum();
            this.btnValor7 = new Free1X2.UI.Controls.BotonValorNum();
            this.btnValor8 = new Free1X2.UI.Controls.BotonValorNum();
            this.btnValor9 = new Free1X2.UI.Controls.BotonValorNum();
            this.btnValor10 = new Free1X2.UI.Controls.BotonValorNum();
            this.btnValor11 = new Free1X2.UI.Controls.BotonValorNum();
            this.btnValor12 = new Free1X2.UI.Controls.BotonValorNum();
            this.btnValor13 = new Free1X2.UI.Controls.BotonValorNum();
            this.btnValor14 = new Free1X2.UI.Controls.BotonValorNum();
            this.SuspendLayout();
            // 
            // btnSelTodos
            // 
            this.btnSelTodos.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnSelTodos.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSelTodos.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSelTodos.Location = new System.Drawing.Point(2, 0);
            this.btnSelTodos.Name = "btnSelTodos";
            this.btnSelTodos.Size = new System.Drawing.Size(16, 16);
            this.btnSelTodos.TabIndex = 29;
            this.btnSelTodos.Text = "+";
            this.btnSelTodos.UseVisualStyleBackColor = false;
            this.btnSelTodos.Click += new System.EventHandler(this.btnSelTodos_Click);
            // 
            // btnUnselTodos
            // 
            this.btnUnselTodos.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnUnselTodos.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnUnselTodos.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUnselTodos.Location = new System.Drawing.Point(580, 0);
            this.btnUnselTodos.Name = "btnUnselTodos";
            this.btnUnselTodos.Size = new System.Drawing.Size(16, 16);
            this.btnUnselTodos.TabIndex = 30;
            this.btnUnselTodos.Text = "-";
            this.btnUnselTodos.UseVisualStyleBackColor = false;
            this.btnUnselTodos.Click += new System.EventHandler(this.btnUnselTodos_Click);
            // 
            // btnValor15
            // 
            this.btnValor15.BackColor = System.Drawing.Color.Wheat;
            this.btnValor15.LetraTolerancia = "";
            this.btnValor15.Location = new System.Drawing.Point(514, 0);
            this.btnValor15.Name = "btnValor15";
            this.btnValor15.Size = new System.Drawing.Size(32, 16);
            this.btnValor15.TabIndex = 0;
            this.btnValor15.ValorCombinado = "15";
            this.btnValor15.ValorNumerico = "15";
            // 
            // btnValor16
            // 
            this.btnValor16.BackColor = System.Drawing.Color.Wheat;
            this.btnValor16.LetraTolerancia = "";
            this.btnValor16.Location = new System.Drawing.Point(547, 0);
            this.btnValor16.Name = "btnValor16";
            this.btnValor16.Size = new System.Drawing.Size(32, 16);
            this.btnValor16.TabIndex = 1;
            this.btnValor16.ValorCombinado = "16";
            this.btnValor16.ValorNumerico = "16";
            // 
            // btnValor0
            // 
            this.btnValor0.BackColor = System.Drawing.Color.Wheat;
            this.btnValor0.LetraTolerancia = "";
            this.btnValor0.Location = new System.Drawing.Point(19, 0);
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
            this.btnValor1.Location = new System.Drawing.Point(52, 0);
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
            this.btnValor2.Location = new System.Drawing.Point(85, 0);
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
            this.btnValor3.Location = new System.Drawing.Point(118, 0);
            this.btnValor3.Name = "btnValor3";
            this.btnValor3.Size = new System.Drawing.Size(32, 16);
            this.btnValor3.TabIndex = 17;
            this.btnValor3.ValorCombinado = "3";
            this.btnValor3.ValorNumerico = "3";
            // 
            // btnValor4
            // 
            this.btnValor4.BackColor = System.Drawing.Color.Wheat;
            this.btnValor4.LetraTolerancia = "";
            this.btnValor4.Location = new System.Drawing.Point(151, 0);
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
            this.btnValor5.Location = new System.Drawing.Point(184, 0);
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
            this.btnValor6.Location = new System.Drawing.Point(217, 0);
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
            this.btnValor7.Location = new System.Drawing.Point(250, 0);
            this.btnValor7.Name = "btnValor7";
            this.btnValor7.Size = new System.Drawing.Size(32, 16);
            this.btnValor7.TabIndex = 21;
            this.btnValor7.ValorCombinado = "7";
            this.btnValor7.ValorNumerico = "7";
            // 
            // btnValor8
            // 
            this.btnValor8.BackColor = System.Drawing.Color.Wheat;
            this.btnValor8.LetraTolerancia = "";
            this.btnValor8.Location = new System.Drawing.Point(283, 0);
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
            this.btnValor9.Location = new System.Drawing.Point(316, 0);
            this.btnValor9.Name = "btnValor9";
            this.btnValor9.Size = new System.Drawing.Size(32, 16);
            this.btnValor9.TabIndex = 23;
            this.btnValor9.ValorCombinado = "9";
            this.btnValor9.ValorNumerico = "9";
            // 
            // btnValor10
            // 
            this.btnValor10.BackColor = System.Drawing.Color.Wheat;
            this.btnValor10.LetraTolerancia = "";
            this.btnValor10.Location = new System.Drawing.Point(349, 0);
            this.btnValor10.Name = "btnValor10";
            this.btnValor10.Size = new System.Drawing.Size(32, 16);
            this.btnValor10.TabIndex = 24;
            this.btnValor10.ValorCombinado = "10";
            this.btnValor10.ValorNumerico = "10";
            // 
            // btnValor11
            // 
            this.btnValor11.BackColor = System.Drawing.Color.Wheat;
            this.btnValor11.LetraTolerancia = "";
            this.btnValor11.Location = new System.Drawing.Point(382, 0);
            this.btnValor11.Name = "btnValor11";
            this.btnValor11.Size = new System.Drawing.Size(32, 16);
            this.btnValor11.TabIndex = 25;
            this.btnValor11.ValorCombinado = "11";
            this.btnValor11.ValorNumerico = "11";
            // 
            // btnValor12
            // 
            this.btnValor12.BackColor = System.Drawing.Color.Wheat;
            this.btnValor12.LetraTolerancia = "";
            this.btnValor12.Location = new System.Drawing.Point(415, 0);
            this.btnValor12.Name = "btnValor12";
            this.btnValor12.Size = new System.Drawing.Size(32, 16);
            this.btnValor12.TabIndex = 26;
            this.btnValor12.ValorCombinado = "12";
            this.btnValor12.ValorNumerico = "12";
            // 
            // btnValor13
            // 
            this.btnValor13.BackColor = System.Drawing.Color.Wheat;
            this.btnValor13.LetraTolerancia = "";
            this.btnValor13.Location = new System.Drawing.Point(448, 0);
            this.btnValor13.Name = "btnValor13";
            this.btnValor13.Size = new System.Drawing.Size(32, 16);
            this.btnValor13.TabIndex = 27;
            this.btnValor13.ValorCombinado = "13";
            this.btnValor13.ValorNumerico = "13";
            // 
            // btnValor14
            // 
            this.btnValor14.BackColor = System.Drawing.Color.Wheat;
            this.btnValor14.LetraTolerancia = "";
            this.btnValor14.Location = new System.Drawing.Point(481, 0);
            this.btnValor14.Name = "btnValor14";
            this.btnValor14.Size = new System.Drawing.Size(32, 16);
            this.btnValor14.TabIndex = 28;
            this.btnValor14.ValorCombinado = "14";
            this.btnValor14.ValorNumerico = "14";
            // 
            // OptionNumTol0_14
            // 
            this.BackColor = System.Drawing.Color.Wheat;
            this.Controls.Add(this.btnValor15);
            this.Controls.Add(this.btnValor16);
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
            this.Controls.Add(this.btnValor14);
            this.Name = "OptionNumTol0_14";
            this.Size = new System.Drawing.Size(600, 16);
            this.Load += new System.EventHandler(this.OptionNumTol0_14_Load);
            this.ResumeLayout(false);

		}

		private void btnSelTodos_Click(object sender, EventArgs e)
		{
			string txt="";
			int i;
			for(i=Minimo;i<=Maximo;i++)
			{
				txt+=i+",";
			}
			Valores = txt.Substring(0,txt.Length-1);
		}

		private void btnUnselTodos_Click(object sender, EventArgs e)
		{
			Valores = "";
		}

		public BotonValorNum BuscarControl(int numero)
		{
            return arrayControles[numero];
		}

		private void OptionNumTol0_14_Load(object sender, EventArgs e)
		{
			cambiarControl();
		}

	}
}
