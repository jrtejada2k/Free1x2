using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;
using System.Text;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI
{
	/// <summary>
	/// Descripción breve de ListaEquiposFrm.
	/// </summary>
	public class ListaEquiposFrm : System.Windows.Forms.Form
	{
		protected TextBox txt;
		protected string cat;
		private System.Windows.Forms.ListBox listBox1;
		private StreamReader sr = null;
		private string fichero;

		/// <summary>
		/// Variable del diseñador requerida.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ListaEquiposFrm(TextBox miTxt, string categoria)
		{
			InitializeComponent();
			txt=miTxt;
			cat=categoria;
            FormulariosHelper fH = new FormulariosHelper();
            fH.Traducir(this);
		}
        protected override void OnLoad(System.EventArgs e)
        {
            base.OnLoad(e);
            ModernTheme.ApplyToForm(this);
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

		#region Código generado por el Diseñador de Windows Forms
		/// <summary>
		/// Método necesario para admitir el Diseñador. No se puede modificar
		/// el contenido del método con el editor de código.
		/// </summary>
		private void InitializeComponent()
		{
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // listBox1
            // 
            this.listBox1.BackColor = System.Drawing.Color.Bisque;
            this.listBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.ScrollAlwaysVisible = true;
            this.listBox1.Size = new System.Drawing.Size(216, 260);
            this.listBox1.Sorted = true;
            this.listBox1.TabIndex = 0;
            this.listBox1.TabStop = false;
            this.listBox1.DoubleClick += new System.EventHandler(this.listBox1_DoubleClick);
            // 
            // ListaEquiposFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(216, 260);
            this.Controls.Add(this.listBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ListaEquiposFrm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Lista de Equipos";
            this.Load += new System.EventHandler(this.ListaEquiposFrm_Load);
            this.ResumeLayout(false);

		}
		#endregion

		private void listBox1_DoubleClick(object sender, System.EventArgs e)
		{
			txt.Text=listBox1.SelectedItem.ToString();
			this.Hide();
		}

		private void ListaEquiposFrm_Load(object sender, System.EventArgs e)
		{
            if (File.Exists(Application.StartupPath + "/Equipos/equipos" + cat + ".dat"))
            {
                fichero = Application.StartupPath + "/Equipos/equipos" + cat + ".dat";
                listBox1.Items.Clear();
                while (HaySiguiente())
                {
                    listBox1.Items.Add(LeeEquipos());
                }
                if (sr != null)
                {
                    sr.Close();
                }
            }
            else
            {
                MessageBox.Show("No se encuentra el archivo " + Application.StartupPath +  "/Equipos/equipos" + cat + ".dat", "Error");
            }
		}

		private string LeeEquipos()
		{		
			string nombreEquipo = "";
			nombreEquipo = sr.ReadLine().Trim();
			return nombreEquipo;
		}

		private bool HaySiguiente()
		{
			bool tieneSiguiente = false;			
			if( sr == null )
			{
				sr = new StreamReader( fichero, System.Text.Encoding.Default );
			}
			if( sr.Peek() >= 0 )
			{
				tieneSiguiente = true;
			}
			return tieneSiguiente;		
		}

	}
}
