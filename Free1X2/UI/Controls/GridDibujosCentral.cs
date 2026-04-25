using System.Collections;
using System.Drawing;
using System.Windows.Forms;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI.Controls
{
	public class GridDibujosCentral : UserControl
	{
		
		private ArrayList dibujos;		
		
		public GridDibujosCentral()
		{
            int noPartidos;
            try
            {
                noPartidos = VariablesGlobales.NumeroPartidos;
            }
            catch
            {
                noPartidos = 15;
            }
            InitializeComponent();
            GenerarCasillas(noPartidos);
			InicializaVariables();
		}
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToControl(this);
        }

		
		protected void InicializaVariables()
		{
			dibujos = new ArrayList();
		}
        protected void GenerarCasillas(int noPartidos)
        {
            int x = 0;
            int y = 0;
            string texto;
            int numX = 0;
            int num2 = 0;
            int filas = 0;
            //Empezar por las equis
            for (int i = 0; i <= noPartidos; i++)
            {
                //Añadir un control
                OptionNum opNum = new OptionNum();
                texto = numX + "+" + num2;
                opNum.Valor = texto;
                opNum.Location = new Point(x, y);
                Controls.Add(opNum);
                //Aumentar y
                x += opNum.Width + 1;
                //Aumentar numero de doses
                num2++;
                //Si llegamos al tope, nueva columna
                if ((i == noPartidos) || (numX + num2 > noPartidos))
                {
                    i = 0;
                    numX++;
                    num2 = 0;
                    y += opNum.Height + 1;
                    x = 0;
                    filas++;
                }
                if (filas > noPartidos)
                {
                    break;
                }
            }
        }

		public void GetValores()
		{
		    //borrar dibujos
			dibujos.Clear();
			foreach(Control control in Controls)
			{
			    OptionNum dib = (OptionNum)control;

			    if( dib.ValorActivado )
				{
					dibujos.Add( dib.Valor );	
				}
			}
		}
		
		public void SeleccionaTodos()
		{
		    foreach(Control control in Controls)
		    {
		        OptionNum dib = (OptionNum)control;

		        dib.ValorActivado = true;
		    }
		}
		
		public void DeseleccionaTodos()
		{
		    foreach(Control control in Controls)
		    {
		        OptionNum dib = (OptionNum)control;

		        dib.ValorActivado = false;
		    }
		}
		
		public ArrayList Dibujos
		{
			get 
			{ 
				GetValores();
				return dibujos; 
			}
			set 
			{
				dibujos = value; 
				MarcarDibujos();
			}
		
		}
		
		protected void MarcarDibujos()
		{
			foreach(string str in dibujos)
			{
				MarcarOpcion( str );			
			}	
		}
		
		protected void MarcarOpcion(string strDibujo)
		{
		    foreach(Control control in Controls)
		    {
		        OptionNum dib = (OptionNum)control;

		        if( dib.Valor.Equals( strDibujo ) )
				{
					dib.ValorActivado = true;
					break;
				}
		    }
		}

		void InitializeComponent() {
            SuspendLayout();
            // 
            // GridDibujosCentral
            // 
            AutoSize = true;
            BackColor = Color.Bisque;
            Name = "GridDibujosCentral";
            this.Size = new Size(554, 304);
            ResumeLayout(false);
		}
	}
}
