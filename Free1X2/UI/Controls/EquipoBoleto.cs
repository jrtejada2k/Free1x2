using System;
using System.Windows.Forms;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI.Controls
{
	/// <summary>
	/// Descripción breve de EquipoBoleto.
	/// </summary>
	public class EquipoBoleto : UserControl
	{
		private TextBox txtEquipo;
		private ComboBox cmbAbrir;
		/// <summary> 
		/// Variable del diseñador requerida.
		/// </summary>
		private System.ComponentModel.Container components = null;

        public EquipoBoleto()
        {
            InitializeComponent();
        }
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToControl(this);
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
            this.txtEquipo = new System.Windows.Forms.TextBox();
            this.cmbAbrir = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // txtEquipo
            // 
            this.txtEquipo.BackColor = System.Drawing.Color.Bisque;
            this.txtEquipo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtEquipo.Font = new System.Drawing.Font("Verdana", 8.25F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtEquipo.ForeColor = System.Drawing.Color.Maroon;
            this.txtEquipo.Location = new System.Drawing.Point(1, 3);
            this.txtEquipo.Margin = new System.Windows.Forms.Padding(0);
            this.txtEquipo.Name = "txtEquipo";
            this.txtEquipo.Size = new System.Drawing.Size(88, 14);
            this.txtEquipo.TabIndex = 0;
            // 
            // cmbAbrir
            // 
            this.cmbAbrir.BackColor = System.Drawing.Color.Bisque;
            this.cmbAbrir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbAbrir.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbAbrir.ForeColor = System.Drawing.Color.Maroon;
            this.cmbAbrir.Items.AddRange(new object[] {
            "1",
            "2",
            "2b",
            "Int"});
            this.cmbAbrir.Location = new System.Drawing.Point(91, 0);
            this.cmbAbrir.Name = "cmbAbrir";
            this.cmbAbrir.Size = new System.Drawing.Size(37, 20);
            this.cmbAbrir.TabIndex = 1;
            this.cmbAbrir.TabStop = false;
            this.cmbAbrir.Text = "?";
            this.cmbAbrir.SelectedIndexChanged += new System.EventHandler(this.cmbAbrir_SelectedIndexChanged);
            // 
            // EquipoBoleto
            // 
            this.BackColor = System.Drawing.Color.Bisque;
            this.Controls.Add(this.cmbAbrir);
            this.Controls.Add(this.txtEquipo);
            this.Name = "EquipoBoleto";
            this.Size = new System.Drawing.Size(127, 20);
            this.BackColorChanged += new System.EventHandler(this.EquipoBoleto_BackColorChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

		}
		#endregion

		public string Equipo
		{
			get{ return txtEquipo.Text; }
			set{txtEquipo.Text = value;}
		}

		private void EquipoBoleto_BackColorChanged(object sender, EventArgs e)
		{
			txtEquipo.BackColor=BackColor;
			cmbAbrir.BackColor=BackColor;
		}

		private void cmbAbrir_SelectedIndexChanged(object sender, EventArgs e)
		{
			// Al cargar en memoria el form pasamos el textBox y la categoría 
			// para que abra el fichero adecuado.
			ListaEquiposFrm f=new ListaEquiposFrm(txtEquipo, cmbAbrir.SelectedItem.ToString());
			// La ventana de selección se abre a la derecha del cursor y centrado 
			// verticalmente respecto al mismo.
			f.StartPosition=FormStartPosition.Manual;
			System.Drawing.Point p;
			p=Cursor.Position;
			p.Y-=Convert.ToInt16(f.Size.Height/2);
			f.DesktopLocation=p;
			// Abre el form
			f.ShowDialog();
			// Si se ha hecho doble clic ya tenemos equipo y liberamos el form
			f.Dispose();
		}
	}
}
