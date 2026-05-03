using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI.Controls.Boleto
{
	/// <summary>
	/// Descripción breve de ControlColumnaBoleto.
	/// </summary>
	public class ControlColumnaBoleto : UserControl
	{
		private Label numero;


		private Label aciertos;
		/// <summary> 
		/// Variable del diseñador requerida.
		/// </summary>
		private Container components = null;

		public ControlColumnaBoleto(int partidos)
		{
			InitializeComponent();
            int x = 0;
            int y = numero.Location.Y + numero.Height + 1 ;
            for (int i = 0; i < partidos; i++)
            {
                ControlApuestaBoleto ap = new ControlApuestaBoleto();
                ap.Location = new Point(x, y);
                ap.Name = "apuesta_" + Convert.ToString(i + 1);
                Controls.Add(ap);

                y += ap.Height;

                if (i == 3 || i == 7 || i == 10 || i == 13)
                {
                    y += 1;
                }
            }
		}
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToControl(this);
        }


        /// <summary> 
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        protected override void Dispose(bool disposing)
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

		#region Component Designer generated code
		/// <summary> 
		/// Método necesario para admitir el Diseñador. No se puede modificar 
		/// el contenido del método con el editor de código.
		/// </summary>
		private void InitializeComponent()
		{
            this.numero = new System.Windows.Forms.Label();
            this.aciertos = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // numero
            // 
            this.numero.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.numero.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numero.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numero.Location = new System.Drawing.Point(0, 0);
            this.numero.Name = "numero";
            this.numero.Size = new System.Drawing.Size(41, 16);
            this.numero.TabIndex = 15;
            this.numero.Text = "0";
            this.numero.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            // 
            // aciertos
            // 
            this.aciertos.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.aciertos.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.aciertos.ForeColor = System.Drawing.Color.Silver;
            this.aciertos.Location = new System.Drawing.Point(0, 259);
            this.aciertos.Name = "aciertos";
            this.aciertos.Size = new System.Drawing.Size(42, 16);
            this.aciertos.TabIndex = 16;
            this.aciertos.Text = "0";
            this.aciertos.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // ControlColumnaBoleto
            // 
            this.BackColor = System.Drawing.Color.Bisque;
            this.Controls.Add(this.aciertos);
            this.Controls.Add(this.numero);
            this.Name = "ControlColumnaBoleto";
            this.Size = new System.Drawing.Size(42, 278);
            this.ResumeLayout(false);

		}
		#endregion

		public int Aciertos
		{
			get{ return Convert.ToInt16(aciertos.Text); }
			set
			{
				if(value>0)
				{
					aciertos.Text = (value).ToString();
					if(value<10)
					{
						aciertos.ForeColor=Color.Silver;
					}
					else
					{
						aciertos.ForeColor=Color.Blue;
					}

				}
				else
				{
					aciertos.Text = "";
				}
			}
		}

		public int NumColumna
		{
			get{ return Convert.ToInt16(numero.Text);}
			set
			{
				if(value>0)
				{
					numero.Text = (value).ToString();
				}
				else
				{
					numero.Text = "";
				}
			}
		}

        private ControlApuestaBoleto ObtenerControlApuestaBoleto(int partido)
        {
            return (ControlApuestaBoleto)Controls[partido + 1];
        }

        public void LlenarApuesta(int partido, char signo)
        {
            ControlApuestaBoleto ap = ObtenerControlApuestaBoleto(partido);

            switch (signo)
            {
                case '1':
                    ap.Uno = true;
                    break;
                case 'X':
                    ap.Equis = true;
                    break;
                case '2':
                    ap.Dos = true;
                    break;
            }
            ap.Visible = true;
        }

        public void LimpiarApuesta(int partido)
        {
            ControlApuestaBoleto ap = ObtenerControlApuestaBoleto(partido);
            ap.Uno = false;
            ap.Equis = false;
            ap.Dos = false;
        }

        public void OcultarApuesta(int partido)
        {
            ControlApuestaBoleto ap = ObtenerControlApuestaBoleto(partido);

            ap.Visible = false;
        }

    }
}
