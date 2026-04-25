using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;

using Free1X2.UI.Modern.Theming;
namespace Free1X2.UI
{
	/// <summary>
	/// Descripción breve de ResultadosCalculoMultipleFrm.
	/// </summary>
	public class ColumnasPremiadasFrm : System.Windows.Forms.Form
	{
		public System.Windows.Forms.ListView listaResumen;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ColumnHeader columnHeader6;
        private Button btnGuardarTodas;
        private Button btnGuardarSeleccionadas;
		/// <summary>
		/// Variable del diseñador requerida.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ColumnasPremiadasFrm()
		{
            InitializeComponent();
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColumnasPremiadasFrm));
            this.listaResumen = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
            this.btnGuardarTodas = new System.Windows.Forms.Button();
            this.btnGuardarSeleccionadas = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listaResumen
            // 
            this.listaResumen.AllowColumnReorder = true;
            this.listaResumen.BackColor = System.Drawing.Color.White;
            this.listaResumen.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.listaResumen.Font = new System.Drawing.Font("Verdana", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listaResumen.FullRowSelect = true;
            this.listaResumen.GridLines = true;
            this.listaResumen.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listaResumen.Location = new System.Drawing.Point(0, 0);
            this.listaResumen.Name = "listaResumen";
            this.listaResumen.Size = new System.Drawing.Size(506, 265);
            this.listaResumen.Sorting = System.Windows.Forms.SortOrder.Descending;
            this.listaResumen.TabIndex = 37;
            this.listaResumen.UseCompatibleStateImageBehavior = false;
            this.listaResumen.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Arch.Columnas";
            this.columnHeader1.Width = 150;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Jorn.";
            this.columnHeader2.Width = 40;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Columna";
            this.columnHeader3.Width = 120;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Premio";
            this.columnHeader4.Width = 45;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Nº Col.";
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Nº Boleto";
            this.columnHeader6.Width = 70;
            // 
            // btnGuardarTodas
            // 
            this.btnGuardarTodas.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnGuardarTodas.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGuardarTodas.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardarTodas.Image = ((System.Drawing.Image)(resources.GetObject("btnGuardarTodas.Image")));
            this.btnGuardarTodas.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGuardarTodas.Location = new System.Drawing.Point(12, 273);
            this.btnGuardarTodas.Name = "btnGuardarTodas";
            this.btnGuardarTodas.Size = new System.Drawing.Size(123, 32);
            this.btnGuardarTodas.TabIndex = 38;
            this.btnGuardarTodas.Text = "Guardar Todas";
            this.btnGuardarTodas.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGuardarTodas.UseVisualStyleBackColor = false;
            this.btnGuardarTodas.Click += new System.EventHandler(this.btnGuardarTodas_Click);
            // 
            // btnGuardarSeleccionadas
            // 
            this.btnGuardarSeleccionadas.BackColor = System.Drawing.Color.DarkSalmon;
            this.btnGuardarSeleccionadas.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnGuardarSeleccionadas.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardarSeleccionadas.Image = ((System.Drawing.Image)(resources.GetObject("btnGuardarSeleccionadas.Image")));
            this.btnGuardarSeleccionadas.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGuardarSeleccionadas.Location = new System.Drawing.Point(328, 273);
            this.btnGuardarSeleccionadas.Name = "btnGuardarSeleccionadas";
            this.btnGuardarSeleccionadas.Size = new System.Drawing.Size(166, 32);
            this.btnGuardarSeleccionadas.TabIndex = 39;
            this.btnGuardarSeleccionadas.Text = "Guardar Seleccionadas";
            this.btnGuardarSeleccionadas.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGuardarSeleccionadas.UseVisualStyleBackColor = false;
            this.btnGuardarSeleccionadas.Click += new System.EventHandler(this.btnGuardarSeleccionadas_Click);
            // 
            // ColumnasPremiadasFrm
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.BackColor = System.Drawing.Color.Bisque;
            this.ClientSize = new System.Drawing.Size(506, 317);
            this.Controls.Add(this.btnGuardarSeleccionadas);
            this.Controls.Add(this.btnGuardarTodas);
            this.Controls.Add(this.listaResumen);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ColumnasPremiadasFrm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Columnas Premiadas";
            this.ResumeLayout(false);

		}
		#endregion

        private void btnGuardarTodas_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                btnGuardarTodas.Text = "Guardando...";
                string nombre = saveFile.FileName;
                StreamWriter writer = new StreamWriter(nombre);
                for (int i = 0; i < listaResumen.Items.Count; i++)
                {
                    writer.WriteLine(listaResumen.Items[i].SubItems[2].Text);
                }
                writer.Close();
            }
            saveFile.Dispose();
            btnGuardarTodas.Text = "Guardar Todas";
        }

        private void btnGuardarSeleccionadas_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFile = new SaveFileDialog();
            saveFile.Filter = "Columnas(*.txt)|*.txt|Todos los archivos (*.*)|*.*";
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                btnGuardarSeleccionadas.Text = "Guardando...";
                string nombre = saveFile.FileName;
                StreamWriter writer = new StreamWriter(nombre);
                for (int i = 0; i < listaResumen.Items.Count; i++)
                {
                    if (listaResumen.Items[i].Selected)
                    {
                        writer.WriteLine(listaResumen.Items[i].SubItems[2].Text);
                    }
                }
                writer.Close();
            }
            saveFile.Dispose();
            btnGuardarSeleccionadas.Text = "Guardar Seleccionadas";
        }
	}
}
