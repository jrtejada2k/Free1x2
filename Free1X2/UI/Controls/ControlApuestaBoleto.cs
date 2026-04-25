using Free1X2.UI.Modern.Theming;

namespace Free1X2.UI.Controls.Boleto
{
	/// <summary>
	/// Descripción breve de ControlApuestaBoleto.
	/// </summary>
	public class ControlApuestaBoleto : System.Windows.Forms.UserControl
	{

		private System.Windows.Forms.Label l_1;
		private System.Windows.Forms.Label l_2;
		private System.Windows.Forms.Label l_X;
		/// <summary>
		/// Variable del diseñador requerida.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ControlApuestaBoleto()
		{
			InitializeComponent();
		}

        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToControl(this);
        }

		public bool Uno
		{
			get{ return l_1.Visible; }
			set{ l_1.Visible = value;}
		}
		public bool Equis
		{
			get{ return l_X.Visible; }
			set{ l_X.Visible = value;}
		}
		public bool Dos
		{
			get{ return l_2.Visible; }
			set{ l_2.Visible = value;}
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

		#region Component Designer generated code
		/// <summary> 
		/// Método necesario para admitir el Diseñador. No se puede modificar 
		/// el contenido del método con el editor de código.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlApuestaBoleto));
            this.l_1 = new System.Windows.Forms.Label();
            this.l_2 = new System.Windows.Forms.Label();
            this.l_X = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // l_1
            // 
            this.l_1.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l_1.Location = new System.Drawing.Point(2, 2);
            this.l_1.Name = "l_1";
            this.l_1.Size = new System.Drawing.Size(10, 10);
            this.l_1.TabIndex = 0;
            this.l_1.Text = "x";
            this.l_1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.l_1.Visible = false;
            // 
            // l_2
            // 
            this.l_2.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l_2.Location = new System.Drawing.Point(28, 2);
            this.l_2.Name = "l_2";
            this.l_2.Size = new System.Drawing.Size(10, 10);
            this.l_2.TabIndex = 1;
            this.l_2.Text = "x";
            this.l_2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.l_2.Visible = false;
            // 
            // l_X
            // 
            this.l_X.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.l_X.Location = new System.Drawing.Point(15, 2);
            this.l_X.Name = "l_X";
            this.l_X.Size = new System.Drawing.Size(10, 10);
            this.l_X.TabIndex = 2;
            this.l_X.Text = "x";
            this.l_X.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.l_X.Visible = false;
            // 
            // ControlApuestaBoleto
            // 
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.Controls.Add(this.l_X);
            this.Controls.Add(this.l_2);
            this.Controls.Add(this.l_1);
            this.Name = "ControlApuestaBoleto";
            this.Size = new System.Drawing.Size(41, 14);
            this.ResumeLayout(false);

		}
		#endregion
	}
}
