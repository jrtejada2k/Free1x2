using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI.Controls
{
	/// <summary>
	/// Descripción breve de PartidoBoleto.
	/// </summary>
	public class PartidoBoleto : System.Windows.Forms.UserControl
    {
        public Label lblNumPartido;
		private Free1X2.UI.Controls.EquipoBoleto equipoCasa;
		private Free1X2.UI.Controls.EquipoBoleto equipoFuera;
		private Free1X2.UI.Controls.Prono1X2 pronostico;
		private bool isEnabled=true;
		private System.Drawing.Color colorBase;
		private System.Windows.Forms.Panel panel1;
		/// <summary> 
		/// Variable del diseñador requerida.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public event EventHandler PronosticoChanged;

		public PartidoBoleto()
		{
			InitializeComponent();
			pronostico.PronosticoChanged += (s, e) => PronosticoChanged?.Invoke(this, EventArgs.Empty);
		}
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToControl(this);
        }


		// Propiedades
		public System.Drawing.Color ColorBase
		{
			get{ return colorBase; }
			set{colorBase = value;}
		}

		public string EquipoCasa
		{
			get{ return equipoCasa.Equipo; }
			set{equipoCasa.Equipo = value;}
		}

		public string EquipoFuera
		{
			get{ return equipoFuera.Equipo; }
			set{equipoFuera.Equipo = value;}
		}

		public int NumPartido
		{
			get{ return Convert.ToInt16(lblNumPartido.Text); }
			set{lblNumPartido.Text = value.ToString();}
		}

		public string Pronostico
		{
			get{ return pronostico.Pronostico; }
			set{pronostico.Pronostico = value;}
		}

		public bool IsEnabled
		{
			get{ return isEnabled; }
			set
			{
				isEnabled=value;
				pronostico.Enabled = value;
				equipoCasa.Enabled = value;
				equipoFuera.Enabled = value;
				if(value)
					this.BackColor=ColorBase;
				else
					this.BackColor=Color.Silver;
			}
		}

		/// <summary> 
		/// Limpiar los recursos que se estén utilizando.
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

		#region Código generado por el Diseñador de componentes
		/// <summary> 
		/// Método necesario para admitir el Diseñador. No se puede modificar 
		/// el contenido del método con el editor de código.
		/// </summary>
		private void InitializeComponent()
		{
            this.lblNumPartido = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.equipoFuera = new Free1X2.UI.Controls.EquipoBoleto();
            this.equipoCasa = new Free1X2.UI.Controls.EquipoBoleto();
            this.pronostico = new Free1X2.UI.Controls.Prono1X2();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblNumPartido
            // 
            this.lblNumPartido.BackColor = System.Drawing.Color.Bisque;
            this.lblNumPartido.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.lblNumPartido.Font = new System.Drawing.Font("Verdana", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNumPartido.Location = new System.Drawing.Point(0, 0);
            this.lblNumPartido.Name = "lblNumPartido";
            this.lblNumPartido.Size = new System.Drawing.Size(24, 20);
            this.lblNumPartido.TabIndex = 76;
            this.lblNumPartido.Text = "1";
            this.lblNumPartido.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblNumPartido.Click += new System.EventHandler(this.lblNumPartido_Click);
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.White;
            this.panel1.Controls.Add(this.equipoFuera);
            this.panel1.Controls.Add(this.equipoCasa);
            this.panel1.Controls.Add(this.pronostico);
            this.panel1.Controls.Add(this.lblNumPartido);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.panel1.Size = new System.Drawing.Size(342, 20);
            this.panel1.TabIndex = 80;
            this.panel1.Click += new System.EventHandler(this.panel1_Click);
            // 
            // equipoFuera
            // 
            this.equipoFuera.BackColor = System.Drawing.Color.Bisque;
            this.equipoFuera.Equipo = "";
            this.equipoFuera.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.equipoFuera.ForeColor = System.Drawing.Color.Brown;
            this.equipoFuera.Location = new System.Drawing.Point(153, 0);
            this.equipoFuera.Name = "equipoFuera";
            this.equipoFuera.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.equipoFuera.Size = new System.Drawing.Size(127, 20);
            this.equipoFuera.TabIndex = 79;
            // 
            // equipoCasa
            // 
            this.equipoCasa.BackColor = System.Drawing.Color.Bisque;
            this.equipoCasa.Equipo = "";
            this.equipoCasa.Font = new System.Drawing.Font("Verdana", 7F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.equipoCasa.ForeColor = System.Drawing.Color.Brown;
            this.equipoCasa.Location = new System.Drawing.Point(25, 0);
            this.equipoCasa.Name = "equipoCasa";
            this.equipoCasa.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.equipoCasa.Size = new System.Drawing.Size(127, 20);
            this.equipoCasa.TabIndex = 78;
            // 
            // pronostico
            // 
            this.pronostico.ColorFondo = System.Drawing.Color.Wheat;
            this.pronostico.Location = new System.Drawing.Point(280, 0);
            this.pronostico.Name = "pronostico";
            this.pronostico.Pronostico = "";
            this.pronostico.Size = new System.Drawing.Size(65, 20);
            this.pronostico.TabIndex = 77;
            // 
            // PartidoBoleto
            // 
            this.BackColor = System.Drawing.Color.Bisque;
            this.Controls.Add(this.panel1);
            this.Name = "PartidoBoleto";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Size = new System.Drawing.Size(342, 20);
            this.Load += new System.EventHandler(this.PartidoBoleto_Load);
            this.Click += new System.EventHandler(this.PartidoBoleto_Click);
            this.BackColorChanged += new System.EventHandler(this.PartidoBoleto_BackColorChanged);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void PartidoBoleto_BackColorChanged(object sender, System.EventArgs e)
		{
			equipoCasa.BackColor=this.BackColor;
			equipoFuera.BackColor=this.BackColor;
			lblNumPartido.BackColor=this.BackColor;
		}

		private void PartidoBoleto_Load(object sender, System.EventArgs e)
		{
			this.Size=new Size(342, 20);
		}

		private void PartidoBoleto_Click(object sender, System.EventArgs e)
		{
            lblNumPartido_Click(sender, e);
		}

		private void panel1_Click(object sender, System.EventArgs e)
		{
			PartidoBoleto_Click(sender, e);
		}

		private void lblNumPartido_Click(object sender, System.EventArgs e)
        {
            Pronosticos pr = (Pronosticos)this.Parent;
            if (pr.GrupoPantalla != 0)
            {
                IsEnabled = !IsEnabled;
            }
            pr.ComprobarPartidosActivos();
		}
	}
}
